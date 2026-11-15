"""MongoDB local-prototype entry point; it never uses or changes Supabase credentials."""
from datetime import datetime
import os
import re
from typing import Literal
from urllib.parse import urlparse
from uuid import UUID
from fastapi import Depends, HTTPException, Request, Response
from fastapi.responses import JSONResponse
from fastapi.exceptions import RequestValidationError
from pydantic import BaseModel, ConfigDict, Field, field_validator
from pymongo.errors import PyMongoError
from main import Settings, ProfileUpdate, authorized_user, create_app, repository
from mongo_backend import MongoRepository

class Registration(BaseModel):
    model_config = ConfigDict(extra='forbid')
    display_name: str
    email: str = Field(max_length=254)
    password: str = Field(min_length=12, max_length=128)
    username: str | None = None
    @field_validator('display_name')
    @classmethod
    def name(cls, value): return ProfileUpdate.validate_name(value)
    @field_validator('email')
    @classmethod
    def email_value(cls, value):
        value = value.strip().casefold()
        if not re.fullmatch(r'[^\s@]+@[^\s@]+\.[^\s@]+', value): raise ValueError('Enter a valid email address.')
        return value
    @field_validator('username')
    @classmethod
    def handle(cls, value):
        if not value: return None
        value = value.strip().lower()
        if not re.fullmatch(r'[a-z][a-z0-9_]{2,23}', value): raise ValueError('Username must be 3-24 letters, numbers or underscores, starting with a letter.')
        return value

class Login(BaseModel):
    model_config = ConfigDict(extra='forbid')
    identifier: str = Field(min_length=1, max_length=254)
    password: str = Field(min_length=1, max_length=128)
    @field_validator('identifier')
    @classmethod
    def normalized(cls, value): return value.strip().casefold()

class DiscoverableIn(BaseModel):
    model_config = ConfigDict(extra='forbid')
    discoverable: bool

class TalkRequestIn(BaseModel):
    model_config = ConfigDict(extra='forbid')
    to_user_id: UUID

class RequestAction(BaseModel):
    model_config = ConfigDict(extra='forbid')
    action: Literal['accept', 'decline']

class ChatMessageIn(BaseModel):
    model_config = ConfigDict(extra='forbid')
    body: str = Field(min_length=1, max_length=2000)
    @field_validator('body')
    @classmethod
    def clean(cls, value):
        value = value.strip()
        if not value: raise ValueError('Message cannot be empty.')
        if any(ord(c) < 32 and c != '\n' for c in value): raise ValueError('Message contains invalid characters.')
        return value

def create_mongo_app(uri=None, database=None):
    uri = uri or os.environ.get('MONGODB_URL', 'mongodb://127.0.0.1:27018/?directConnection=true')
    # This entry point is intentionally local-only. A hosted deployment needs its own reviewed TLS/configuration.
    parsed = urlparse(uri)
    if parsed.scheme != 'mongodb' or parsed.hostname not in ('127.0.0.1', 'localhost') or ',' in parsed.netloc:
        raise ValueError('Local MongoDB mode requires a loopback MongoDB endpoint.')
    name = database or os.environ.get('MONGODB_DATABASE', 'astrogateway_local')
    async def provider(): return await MongoRepository.connect(uri, name)
    app = create_app(Settings(), provider_factory=provider)
    app.title = 'AstroGateway MongoDB Local Prototype API'
    @app.middleware('http')
    async def local_boundary(request: Request, call_next):
        origin = request.headers.get('origin')
        if request.method not in ('GET', 'HEAD', 'OPTIONS') and origin and origin not in ('http://127.0.0.1:5173', 'http://localhost:5173', 'http://127.0.0.1:8000'):
            return JSONResponse({'detail': 'Origin not allowed.'}, status_code=403)
        response = await call_next(request)
        response.headers['Cache-Control'] = 'no-store'
        return response
    @app.exception_handler(RequestValidationError)
    async def invalid(request, error): return JSONResponse({'detail': 'Invalid request.'}, status_code=422)
    @app.exception_handler(PyMongoError)
    async def unavailable(request, error): return JSONResponse({'detail': 'Local database unavailable. Retry when running.'}, status_code=503)
    @app.get('/v1/auth/provider')
    async def status(): return dict(provider='mongodb-local', email_delivery=False, profile_photos=False)
    def cookie(response, token):
        response.set_cookie('ag_mongo_session', token, max_age=8*3600, httponly=True, samesite='strict', secure=False, path='/account-api')
    @app.post('/v1/auth/register', status_code=201)
    async def register(payload: Registration, request: Request, response: Response):
        repo = request.app.state.repository
        await repo.throttle(request.client.host, payload.email)
        result = await repo.register(payload.display_name, payload.email, payload.password, payload.username)
        cookie(response, result['access_token'])
        return {k:v for k,v in result.items() if k != 'access_token'} if request.headers.get('origin') else result
    @app.post('/v1/auth/login')
    async def login(payload: Login, request: Request, response: Response):
        repo = request.app.state.repository
        await repo.throttle(request.client.host, payload.identifier)
        result = await repo.login(payload.identifier, payload.password)
        cookie(response, result['access_token'])
        return {k:v for k,v in result.items() if k != 'access_token'} if request.headers.get('origin') else result
    @app.post('/v1/auth/logout')
    async def logout(request: Request, response: Response):
        token = request.cookies.get('ag_mongo_session') or request.headers.get('authorization', '').removeprefix('Bearer ')
        if token:
            try: await request.app.state.repository.revoke(await request.app.state.verifier.verify(token))
            except HTTPException as error:
                if error.status_code != 401: raise
        response.delete_cookie('ag_mongo_session', path='/account-api', httponly=True, samesite='strict')
        return {'revoked': True}
    # Player discovery and talk-request/chat lifecycle. Local-prototype only: cross-user queries
    # like this need the local database, not the per-user Supabase/PostgREST adapter in main.py.
    @app.post('/v1/me/discoverable')
    async def set_discoverable(payload: DiscoverableIn, user=Depends(authorized_user), db=Depends(repository)):
        return await db.set_discoverable(user, payload.discoverable)
    @app.get('/v1/players/matches')
    async def matches(user=Depends(authorized_user), db=Depends(repository)):
        return {'matches': await db.list_matches(user)}
    @app.get('/v1/players/requests')
    async def requests_list(user=Depends(authorized_user), db=Depends(repository)):
        return await db.list_requests(user)
    @app.post('/v1/players/requests', status_code=201)
    async def request_create(payload: TalkRequestIn, user=Depends(authorized_user), db=Depends(repository)):
        row = await db.create_request(user, payload.to_user_id)
        return {'request_id': row['_id'], 'status': row['status']}
    @app.post('/v1/players/requests/{request_id}/respond')
    async def request_respond(request_id: UUID, payload: RequestAction, user=Depends(authorized_user), db=Depends(repository)):
        row = await db.respond_request(user, request_id, payload.action == 'accept')
        return {'request_id': row['_id'], 'status': row['status']}
    @app.get('/v1/players/{other_user_id}/messages')
    async def messages_list(other_user_id: UUID, since: str | None = None, user=Depends(authorized_user), db=Depends(repository)):
        since_at = None
        if since:
            try: since_at = datetime.fromisoformat(since)
            except ValueError: raise HTTPException(422, 'Invalid timestamp.') from None
        rows = await db.list_messages(user, other_user_id, since_at)
        return {'messages': [{'message_id': r['_id'], 'from_user_id': r['from_user_id'], 'body': r['body'], 'created_at': r['created_at']} for r in rows]}
    @app.post('/v1/players/{other_user_id}/messages', status_code=201)
    async def messages_send(other_user_id: UUID, payload: ChatMessageIn, user=Depends(authorized_user), db=Depends(repository)):
        row = await db.send_message(user, other_user_id, payload.body)
        return {'message_id': row['_id'], 'from_user_id': row['from_user_id'], 'body': row['body'], 'created_at': row['created_at']}
    return app

app = create_mongo_app()
