"""Supabase Auth + private training API. No password endpoints or second identity issuer."""
from contextlib import asynccontextmanager
from dataclasses import dataclass, field
from datetime import datetime, timezone
import base64
import json
import os
from typing import Literal
from urllib.parse import urlparse
from uuid import UUID
import httpx
from fastapi import Depends, FastAPI, HTTPException, Request
from fastapi.middleware.cors import CORSMiddleware
from fastapi.security import HTTPAuthorizationCredentials, HTTPBearer
from pydantic import BaseModel, ConfigDict, Field, field_validator, model_validator
from database import Database
import sensor_records
from pathlib import Path

def _strict_bool(value):
    if value not in ('true','false'):raise ValueError('Boolean settings require true or false.')
    return value=='true'

def _unique_json(value):
    def pairs(items):
        result={}
        for key,item in items:
            if key in result:raise ValueError('Duplicate JSON key.')
            result[key]=item
        return result
    return json.loads(value,object_pairs_hook=pairs)

@dataclass(frozen=True)
class Settings:
    supabase_url: str=''
    publishable_key: str=field(default='',repr=False)
    database_url: str=field(default='',repr=False)
    cors_origins: tuple[str,...]=()
    jwt_issuer: str=''
    database_ssl_root_cert: str=''
    database_allow_legacy_ca: bool=False
    def __post_init__(self):
        if self.supabase_url or self.publishable_key:
            url=urlparse(self.supabase_url)
            if url.scheme!='https' or not url.netloc or url.username or url.password or url.query or url.fragment or url.path not in ('','/'):
                raise ValueError('Supabase requires an HTTPS project origin.')
            public=self.publishable_key.startswith('sb_publishable_')
            if not public:
                try:
                    body=self.publishable_key.split('.')[1]
                    public=json.loads(base64.urlsafe_b64decode(body+'='*(-len(body)%4)))['role']=='anon'
                except (ValueError,KeyError,IndexError,TypeError):pass
            if not public:raise ValueError('Only publishable/anon keys are allowed. No service-role key is needed.')
        if self.database_url:raise ValueError('Direct database credentials are forbidden in the user API.')
        if self.database_allow_legacy_ca or self.database_ssl_root_cert:
            raise ValueError('Database TLS settings are retired; the user API uses verified HTTPS only.')
        for origin in self.cors_origins:
            parsed=urlparse(origin)
            if parsed.scheme!='https' or not parsed.netloc or parsed.path not in ('','/') or parsed.query or parsed.fragment:
                raise ValueError('CORS requires explicit HTTPS origins.')
        if self.jwt_issuer and self.jwt_issuer!=self.supabase_url.rstrip('/')+'/auth/v1':raise ValueError('JWT issuer must match the configured project.')
    @property
    def configured(self):return bool(self.supabase_url and self.publishable_key)
    @property
    def expected_issuer(self):return self.jwt_issuer or self.supabase_url.rstrip('/')+'/auth/v1'
    @classmethod
    def from_env(cls):
        return cls(os.getenv('SUPABASE_URL','').rstrip('/'),os.getenv('SUPABASE_PUBLISHABLE_KEY',''),os.getenv('DATABASE_URL',''),
                   tuple(x.strip() for x in os.getenv('CORS_ALLOWED_ORIGINS','').split(',') if x.strip()),os.getenv('SUPABASE_JWT_ISSUER',''),os.getenv('DATABASE_SSL_ROOT_CERT',''),_strict_bool(os.getenv('DATABASE_ALLOW_LEGACY_CA','false')))

@dataclass(frozen=True)
class Principal:
    user_id: UUID
    auth_session_id: UUID
    expires_at: datetime
    access_token: str=field(repr=False)

class Profile(BaseModel):
    user_id: UUID
    display_name: str
    username: str | None = None
    avatar_path: str | None = None
    trainee_profile: str
    training_level: int
    created_at: datetime
class ProfileUpdate(BaseModel):
    model_config=ConfigDict(extra='forbid')
    display_name: str
    @field_validator('display_name')
    @classmethod
    def validate_name(cls,value):
        value=value.strip()
        if not 2<=len(value)<=64 or any(ord(c)<32 or 127<=ord(c)<=159 for c in value):
            raise ValueError('Display name must contain 2–64 characters without control characters.')
        return value
class MissionInit(BaseModel):
    model_config=ConfigDict(extra='forbid')
    launch_id: UUID
    mission_id: Literal['mission-red-horizon-poc']
class MissionSession(BaseModel):
    id: UUID
    user_id: UUID
    launch_id: UUID
    mission_id: Literal['mission-red-horizon-poc']
    status: Literal['initialized','integration_check_complete']
    created_at: datetime
    expires_at: datetime
    completed_at: datetime|None=None
class Completion(BaseModel):
    model_config=ConfigDict(extra='forbid')
    launch_id: UUID
    outcome: Literal['integration_check']

# Semantic gameplay commands from the Unity client. The client never sends scores, resource totals or owner IDs.
# Completion stays on the existing /complete route; pause/resume are commands, not separate routes.
CommandKind=Literal['board_rover','exit_rover','scan_target','weather_changed','pause','resume']
MAX_COMMANDS_PER_SESSION=500
class ScanPayload(BaseModel):
    model_config=ConfigDict(extra='forbid')
    sample_id: UUID
    tick: int=Field(ge=0,le=1_000_000_000)
    analysis: Literal['unverified_simulated']
class WeatherPayload(BaseModel):
    model_config=ConfigDict(extra='forbid')
    dust: int=Field(ge=0,le=2)
class EmptyPayload(BaseModel):
    model_config=ConfigDict(extra='forbid')
PAYLOADS={'scan_target':ScanPayload,'weather_changed':WeatherPayload}
class MissionCommandIn(BaseModel):
    model_config=ConfigDict(extra='forbid')
    command_id: UUID
    expected_revision: int=Field(ge=0,le=MAX_COMMANDS_PER_SESSION)
    kind: CommandKind
    payload: dict
    @model_validator(mode='after')
    def validate_payload(self):
        if len(json.dumps(self.payload,separators=(',',':')))>1024:raise ValueError('Command payload too large.')
        self.payload=PAYLOADS.get(self.kind,EmptyPayload).model_validate(self.payload).model_dump(mode='json')
        return self
class MissionCommandAck(BaseModel):
    session_id: UUID
    command_id: UUID
    kind: CommandKind
    revision: int
    accepted_at: datetime
    duplicate: bool
class MissionState(BaseModel):
    id: UUID
    user_id: UUID
    mission_id: Literal['mission-red-horizon-poc']
    status: Literal['initialized','integration_check_complete']
    revision: int
    paused: bool
    created_at: datetime
    expires_at: datetime
    completed_at: datetime|None=None
    last_command_at: datetime|None=None
# Evidence is a count of server-recorded commands. No score, level or survival rule exists until approved (Step 4).
class Evidence(BaseModel):
    simulated_scans: int
    rover_board_exit_cycles: int
    weather_changes: int
class MissionReport(BaseModel):
    session_id: UUID
    user_id: UUID
    mission_id: Literal['mission-red-horizon-poc']
    status: Literal['initialized','integration_check_complete']
    evidence: Evidence
    score: None=None
    rubric_status: Literal['pending_approval']='pending_approval'
class SkillsSummary(BaseModel):
    user_id: UUID
    sessions: int
    evidence: Evidence
    levels: None=None
    rubric_status: Literal['pending_approval']='pending_approval'
class Recommendation(BaseModel):
    mission_id: Literal['mission-red-horizon-poc']
    reason: str
    rule_status: Literal['pending_approval']='pending_approval'

class AuthVerifier:
    def __init__(self,settings,http):self.settings,self.http=settings,http
    async def verify(self,token):
        if not self.settings.supabase_url or not self.settings.publishable_key:raise HTTPException(503,'Account configuration required.')
        try:
            if not isinstance(token,str) or not 1<=len(token)<=8192:raise ValueError()
            parts=token.split('.')
            if len(parts)!=3:raise ValueError()
            decode=lambda p: _unique_json(base64.urlsafe_b64decode(p+'='*(-len(p)%4)))
            header,claims=decode(parts[0]),decode(parts[1])
            if header.get('alg') not in ('ES256','RS256','HS256'):raise ValueError()
        except (ValueError,KeyError,TypeError,AttributeError):
            raise HTTPException(401,'Invalid session.',headers={'WWW-Authenticate':'Bearer'}) from None
        # Decoding above is NOT authentication. The configured issuer must verify the token next.
        # This official Auth-server path supports legacy HS256 without copying signing secrets.
        try:
            response=await self.http.get(self.settings.supabase_url+'/auth/v1/user',headers={'apikey':self.settings.publishable_key,'Authorization':f'Bearer {token}'})
        except httpx.HTTPError:raise HTTPException(503,'Account service unavailable. Retry when connected.') from None
        if response.status_code in (400,401,403):raise HTTPException(401,'Session invalid or expired.',headers={'WWW-Authenticate':'Bearer'})
        if response.status_code==429:raise HTTPException(429,'Too many requests. Retry later.')
        if response.status_code!=200 or len(response.content)>65536:raise HTTPException(503,'Account service unavailable.')
        try:
            user=response.json(); user_id=UUID(user['id']); session_id=UUID(claims['session_id'])
            if type(claims['exp']) is not int:raise ValueError()
            expires=datetime.fromtimestamp(claims['exp'],timezone.utc)
            if 'nbf' in claims and (type(claims['nbf']) is not int or claims['nbf']>datetime.now(timezone.utc).timestamp()):raise ValueError()
            aud=claims.get('aud'); audience=aud if isinstance(aud,list) else [aud]
            if claims.get('iss')!=self.settings.expected_issuer or claims.get('role')!='authenticated' or 'authenticated' not in audience or UUID(claims['sub'])!=user_id or expires<=datetime.now(timezone.utc):raise ValueError()
        except (ValueError,KeyError,TypeError,OverflowError,AttributeError,OSError):raise HTTPException(401,'Invalid session claims.',headers={'WWW-Authenticate':'Bearer'}) from None
        if not user.get('email_confirmed_at') or user.get('is_anonymous',False):raise HTTPException(403,'A verified email account is required.')
        return Principal(user_id,session_id,expires,token)

bearer=HTTPBearer(auto_error=False)
def repository(request:Request):
    if request.app.state.repository is None:raise HTTPException(503,'Private training database configuration required.')
    return request.app.state.repository
async def verified_user(request:Request,credentials:HTTPAuthorizationCredentials|None=Depends(bearer)):
    if credentials is None and getattr(request.app.state.verifier, 'cookie_auth', False):
        token = request.cookies.get('ag_mongo_session', '')
        if token: return await request.app.state.verifier.verify(token)
    if credentials is None or credentials.scheme.lower()!='bearer' or not 1<=len(credentials.credentials)<=8192:
        raise HTTPException(401,'Sign-in required.',headers={'WWW-Authenticate':'Bearer'})
    return await request.app.state.verifier.verify(credentials.credentials)
async def authorized_user(user:Principal=Depends(verified_user),db=Depends(repository)):
    if await db.revoked(user):raise HTTPException(401,'This training session has been signed out.',headers={'WWW-Authenticate':'Bearer'})
    return user

def create_app(settings:Settings|None=None,transport:httpx.AsyncBaseTransport|None=None,test_repository=None,provider_factory=None):
    settings=settings or Settings.from_env()
    @asynccontextmanager
    async def lifespan(app):
        if provider_factory is not None:
            provider = await provider_factory()
            app.state.verifier = provider
            app.state.repository = provider
            app.state.provider_ready = True
            try: yield
            finally: await provider.close()
            return
        if test_repository is None and transport is None and settings.configured:
            # An operator-owned admin file must not be readable by this service account.
            # This guard covers the known development layout, not arbitrary secret discovery.
            admin=Path(__file__).resolve().parent.parent/'database'/'admin.local.env'
            if admin.exists() and os.access(admin,os.R_OK):
                raise RuntimeError('Run the user API in an isolated service environment without readable administrative credentials.')
        async with httpx.AsyncClient(timeout=10,follow_redirects=False,transport=transport) as http:
            app.state.verifier=AuthVerifier(settings,http)
            app.state.repository=test_repository if test_repository is not None else Database(http,settings) if settings.configured else None
            yield
    app=FastAPI(title='AstroGateway Identity API',version='0.4',lifespan=lifespan,debug=False)
    if settings.cors_origins:app.add_middleware(CORSMiddleware,allow_origins=list(settings.cors_origins),allow_credentials=False,allow_methods=['GET','POST'],allow_headers=['Authorization','Content-Type'])
    @app.get('/health')
    def health():return {'status':'ok' if getattr(app.state,'provider_ready',settings.configured) else 'configuration_required'}
    @app.get('/v1/me',response_model=Profile)
    async def me(user:Principal=Depends(authorized_user),db=Depends(repository)):
        profile=await db.profile(user)
        if str(profile['user_id'])!=str(user.user_id):raise HTTPException(502,'Profile ownership validation failed.')
        return profile
    @app.post('/v1/me/display-name',response_model=Profile)
    async def update_profile(payload:ProfileUpdate,user:Principal=Depends(authorized_user),db=Depends(repository)):
        profile=await db.update_profile(user,payload.display_name)
        if str(profile['user_id'])!=str(user.user_id):raise HTTPException(502,'Profile ownership validation failed.')
        return profile
    @app.post('/v1/mission-sessions',response_model=MissionSession,status_code=201)
    async def initialize(payload:MissionInit,user:Principal=Depends(authorized_user),db=Depends(repository)):
        await db.profile(user)
        session=await db.initialize(user,payload)
        if str(session['user_id'])!=str(user.user_id):raise HTTPException(502,'Mission ownership validation failed.')
        return session
    @app.post('/v1/mission-sessions/{session_id}/complete',response_model=MissionSession)
    async def complete(session_id:UUID,payload:Completion,user:Principal=Depends(authorized_user),db=Depends(repository)):
        session=await db.complete(user,session_id,payload)
        if str(session['user_id'])!=str(user.user_id):raise HTTPException(502,'Mission ownership validation failed.')
        return session
    @app.post('/v1/mission-sessions/{session_id}/commands',response_model=MissionCommandAck)
    async def command(session_id:UUID,payload:MissionCommandIn,user:Principal=Depends(authorized_user),db=Depends(repository)):
        ack=await db.submit_command(user,session_id,payload)
        if str(ack['session_id'])!=str(session_id) or str(ack['command_id'])!=str(payload.command_id):raise HTTPException(502,'Mission command validation failed.')
        return ack
    @app.get('/v1/mission-sessions/{session_id}',response_model=MissionState)
    async def state(session_id:UUID,user:Principal=Depends(authorized_user),db=Depends(repository)):
        row=await db.mission_state(user,session_id)
        if str(row['user_id'])!=str(user.user_id) or str(row['id'])!=str(session_id):raise HTTPException(502,'Mission ownership validation failed.')
        return row
    @app.get('/v1/reports/{session_id}',response_model=MissionReport)
    async def report(session_id:UUID,user:Principal=Depends(authorized_user),db=Depends(repository)):
        row=await db.mission_report(user,session_id)
        if str(row['user_id'])!=str(user.user_id) or str(row['session_id'])!=str(session_id):raise HTTPException(502,'Mission ownership validation failed.')
        return row
    @app.get('/v1/skills',response_model=SkillsSummary)
    async def skills(user:Principal=Depends(authorized_user),db=Depends(repository)):
        row=await db.skill_evidence(user)
        if str(row['user_id'])!=str(user.user_id):raise HTTPException(502,'Skill ownership validation failed.')
        return row
    @app.get('/v1/recommendations',response_model=Recommendation)
    async def recommendations(user:Principal=Depends(authorized_user)):
        # Placeholder until recommendation rules are approved: the only published mission, labelled as such.
        return Recommendation(mission_id='mission-red-horizon-poc',reason='Only published mission. Recommendation rules are pending approval.')
    # First Mission rover sensor records (local only); see sensor_records.py.
    sensor_records.register(app,authorized_user,repository)
    @app.post('/v1/auth-session/revoke')
    async def revoke(user:Principal=Depends(verified_user),db=Depends(repository)):
        await db.revoke(user);return {'revoked':True}
    return app

app=create_app()
