"""Local MongoDB account/session/mission provider. Supabase is not contacted in this mode."""
import asyncio
from datetime import datetime, timedelta, timezone
import hashlib
import json
import re
import secrets
from uuid import UUID, uuid4

from argon2 import PasswordHasher
from argon2.exceptions import VerificationError
from fastapi import HTTPException
from pymongo import AsyncMongoClient, ReturnDocument
from pymongo.errors import DuplicateKeyError

from main import Principal
from sensor_records import LIMITS, record_fingerprint

HASHER = PasswordHasher()
DUMMY_HASH = HASHER.hash(secrets.token_urlsafe(32))

def now(): return datetime.now(timezone.utc)
def digest(value): return hashlib.sha256(value.encode()).hexdigest()
def conflict(): return HTTPException(409, 'Mission revision or operation conflict.')

def public_profile(row):
    result = {k: row[k] for k in ('user_id', 'display_name', 'username', 'avatar_path', 'trainee_profile', 'training_level', 'created_at')}
    result['discoverable'] = row.get('discoverable', True)
    return result

def player_card(row):
    """Narrow projection for other players: never email, password_hash or training evidence totals."""
    return {'user_id': row['user_id'], 'display_name': row['display_name'], 'username': row.get('username')}

def pair_key(a, b): return ':'.join(sorted((a, b)))

class MongoRepository:
    cookie_auth = True
    def __init__(self, client, database):
        self.client, self.db = client, database
    @classmethod
    async def connect(cls, uri, name):
        if not re.fullmatch(r'[a-z][a-z0-9_]{2,63}', name): raise ValueError('Invalid MongoDB database name')
        client = AsyncMongoClient(uri, serverSelectionTimeoutMS=5000, tz_aware=True)
        try:
            await client.admin.command('ping')
            repo = cls(client, client[name])
            await repo.db.users.create_index('email', unique=True)
            await repo.db.users.create_index('username', unique=True, partialFilterExpression={'username': {'$type': 'string'}})
            await repo.db.auth_sessions.create_index('expires_at', expireAfterSeconds=0)
            await repo.db.auth_sessions.create_index('auth_session_id', unique=True)
            await repo.db.missions.create_index([('user_id', 1), ('launch_id', 1)], unique=True)
            await repo.db.attempts.create_index('expires_at', expireAfterSeconds=0)
            # Only one active (pending or accepted) talk relationship per pair at a time; a
            # declined/cancelled request is terminal and excluded, so a fresh request can follow.
            await repo.db.talk_requests.create_index('pair_key', unique=True, partialFilterExpression={'status': {'$in': ['pending', 'accepted']}})
            await repo.db.talk_requests.create_index([('to_user_id', 1), ('status', 1)])
            await repo.db.talk_requests.create_index([('from_user_id', 1), ('status', 1)])
            await repo.db.chat_messages.create_index([('pair_key', 1), ('created_at', 1)])
            return repo
        except Exception:
            await client.close()
            raise
    async def close(self): await self.client.close()
    async def throttle(self, peer, identifier):
        window = int(now().timestamp()) // 600
        for key, limit in ((digest(f'ip:{peer}:{window}'), 80), (digest(f'login:{peer}:{identifier}:{window}'), 12)):
            row = await self.db.attempts.find_one_and_update({'_id': key}, {'$inc': {'count': 1}, '$setOnInsert': {'expires_at': now() + timedelta(minutes=20)}}, upsert=True, return_document=ReturnDocument.AFTER)
            if row['count'] > limit: raise HTTPException(429, 'Too many account requests. Try later.')
    async def register(self, name, email, password, username=None):
        row = dict(_id=str(uuid4()), display_name=name, email=email, username=username, avatar_path=None,
                   trainee_profile='astronaut_candidate', training_level=0, created_at=now(), discoverable=True,
                   password_hash=await asyncio.to_thread(HASHER.hash, password))
        row['user_id'] = row['_id']
        try: await self.db.users.insert_one(row)
        except DuplicateKeyError: raise HTTPException(409, 'Account cannot be created with these details.') from None
        return await self.new_session(row)
    async def login(self, identifier, password):
        query = {'email': identifier} if '@' in identifier else {'username': identifier}
        row = await self.db.users.find_one(query)
        try: valid = await asyncio.to_thread(HASHER.verify, row['password_hash'] if row else DUMMY_HASH, password)
        except VerificationError: valid = False
        if not row or not valid: raise HTTPException(401, 'Invalid login details.')
        if HASHER.check_needs_rehash(row['password_hash']):
            await self.db.users.update_one({'_id': row['_id']}, {'$set': {'password_hash': await asyncio.to_thread(HASHER.hash, password)}})
        return await self.new_session(row)
    async def new_session(self, row):
        token = secrets.token_urlsafe(32)
        expiry = now() + timedelta(hours=8)
        await self.db.auth_sessions.insert_one(dict(_id=digest(token), user_id=row['user_id'], auth_session_id=str(uuid4()), expires_at=expiry, revoked=False))
        return dict(access_token=token, token_type='bearer', expires_at=expiry.isoformat(), profile=public_profile(row), provider='mongodb-local', email_verified=False)
    async def verify(self, token):
        if not re.fullmatch(r'[A-Za-z0-9_-]{43}', token): raise HTTPException(401, 'Session invalid or expired.')
        row = await self.db.auth_sessions.find_one({'_id': digest(token), 'revoked': False, 'expires_at': {'$gt': now()}})
        if not row: raise HTTPException(401, 'Session invalid or expired.')
        return Principal(UUID(row['user_id']), UUID(row['auth_session_id']), row['expires_at'], token)
    async def revoked(self, user):
        return not bool(await self.db.auth_sessions.find_one({'_id': digest(user.access_token), 'user_id': str(user.user_id), 'auth_session_id': str(user.auth_session_id), 'revoked': False, 'expires_at': {'$gt': now()}}))
    async def revoke(self, user):
        await self.db.auth_sessions.update_one({'_id': digest(user.access_token), 'user_id': str(user.user_id)}, {'$set': {'revoked': True}})
    async def profile(self, user):
        row = await self.db.users.find_one({'_id': str(user.user_id)})
        if not row: raise HTTPException(404, 'Profile unavailable.')
        return public_profile(row)
    async def update_profile(self, user, name):
        await self.db.users.update_one({'_id': str(user.user_id)}, {'$set': {'display_name': name}})
        return await self.profile(user)
    async def set_discoverable(self, user, value):
        await self.db.users.update_one({'_id': str(user.user_id)}, {'$set': {'discoverable': bool(value)}})
        return await self.profile(user)
    async def initialize(self, user, payload):
        existing = await self.db.missions.find_one({'user_id': str(user.user_id), 'launch_id': str(payload.launch_id)})
        if existing:
            if existing['auth_session_id'] != str(user.auth_session_id) or existing['expires_at'] <= now(): raise conflict()
            return existing
        timestamp = now()
        row = dict(_id=str(uuid4()), user_id=str(user.user_id), auth_session_id=str(user.auth_session_id),
                   launch_id=str(payload.launch_id), mission_id=payload.mission_id, status='initialized',
                   created_at=timestamp, expires_at=min(timestamp + timedelta(hours=1), user.expires_at), completed_at=None,
                   revision=0, sensor_revision=0, paused=False, last_command_at=None, commands=[], records=[])
        row['id'] = row['_id']
        try: await self.db.missions.insert_one(row)
        except DuplicateKeyError: return await self.initialize(user, payload)
        return row
    async def own(self, user, session, active=False):
        if await self.revoked(user): raise HTTPException(401, 'Session invalid or expired.')
        row = await self.db.missions.find_one({'_id': str(session), 'user_id': str(user.user_id)})
        if not row: raise HTTPException(404, 'Mission unavailable.')
        if active and (row['auth_session_id'] != str(user.auth_session_id) or row['status'] != 'initialized' or row['expires_at'] <= now()): raise conflict()
        return row
    async def mission_state(self, user, session): return await self.own(user, session)
    async def complete(self, user, session, payload):
        row = await self.own(user, session)
        if row['auth_session_id'] != str(user.auth_session_id) or row['launch_id'] != str(payload.launch_id): raise conflict()
        if row['status'] == 'integration_check_complete': return row
        if row['expires_at'] <= now(): raise conflict()
        result = await self.db.missions.find_one_and_update({'_id': row['_id'], 'user_id': str(user.user_id), 'status': 'initialized'}, {'$set': {'status': 'integration_check_complete', 'completed_at': now()}}, return_document=ReturnDocument.AFTER)
        return result or await self.own(user, session)
    async def submit_command(self, user, session, command):
        body = command.model_dump(mode='json'); fp = json.dumps(body, sort_keys=True, separators=(',', ':'))
        for _ in range(3):
            row = await self.own(user, session, active=True)
            previous = next((c for c in row['commands'] if c['command_id'] == str(command.command_id)), None)
            if previous:
                if previous['fingerprint'] != fp: raise conflict()
                return dict(previous, session_id=str(session), duplicate=True)
            if row['revision'] != command.expected_revision: raise conflict()
            if row['revision'] >= 500: raise HTTPException(429, 'Mission command limit reached.')
            if (command.kind == 'resume' and not row['paused']) or (command.kind != 'resume' and row['paused']) : raise conflict()
            timestamp = now()
            entry = dict(command_id=str(command.command_id), kind=command.kind, payload=command.payload,
                         fingerprint=fp, revision=row['revision'] + 1, accepted_at=timestamp)
            result = await self.db.missions.update_one({'_id': row['_id'], 'user_id': str(user.user_id), 'revision': row['revision'], 'status': 'initialized'},
                {'$push': {'commands': entry}, '$inc': {'revision': 1}, '$set': {'paused': command.kind == 'pause', 'last_command_at': timestamp}})
            if result.modified_count: return dict(entry, session_id=str(session), duplicate=False)
        raise conflict()
    @staticmethod
    def evidence(row):
        kinds = [c['kind'] for c in row['commands']]
        return dict(simulated_scans=kinds.count('scan_target'), rover_board_exit_cycles=min(kinds.count('board_rover'), kinds.count('exit_rover')), weather_changes=kinds.count('weather_changed'))
    async def mission_report(self, user, session):
        row = await self.own(user, session)
        return dict(session_id=row['id'], user_id=row['user_id'], mission_id=row['mission_id'], status=row['status'], evidence=self.evidence(row))
    async def skill_evidence(self, user):
        rows = await self.db.missions.find({'user_id': str(user.user_id)}).to_list(length=None)
        evidence = {k: 0 for k in ('simulated_scans', 'rover_board_exit_cycles', 'weather_changes')}
        for row in rows:
            for key, value in self.evidence(row).items(): evidence[key] += value
        return dict(user_id=str(user.user_id), sessions=len(rows), evidence=evidence)
    async def submit_sensor_records(self, user, session, records):
        for _ in range(5):
            row = await self.own(user, session, active=True)
            stored = {r['event_id']: r for r in row['records']}
            additions, acknowledgments = [], []
            for record in records:
                event = str(record.event_id); fp = record_fingerprint(record); previous = stored.get(event)
                if previous and previous['fingerprint'] != fp: raise HTTPException(409, 'Sensor record conflict.')
                timestamp = previous['stored_at'] if previous else now()
                if not previous: additions.append(dict(event_id=event, fingerprint=fp, stored_at=timestamp, data=record.model_dump(mode='json')))
                acknowledgments.append(dict(session_id=str(session), event_id=event, duplicate=bool(previous), stored_at=timestamp))
            if len(stored) + len(additions) > LIMITS['max_records_per_session']: raise HTTPException(429, 'Sensor record limit reached.')
            if not additions: return acknowledgments
            result = await self.db.missions.update_one({'_id': row['_id'], 'user_id': str(user.user_id), 'sensor_revision': row['sensor_revision'], 'status': 'initialized'}, {'$push': {'records': {'$each': additions}}, '$inc': {'sensor_revision': 1}})
            if result.modified_count: return acknowledgments
        raise conflict()
    async def list_matches(self, user, limit=20):
        """Directory of discoverable players, ranked by session-count proximity.

        The scoring rubric is still `pending_approval` (see SkillsSummary), so this is an
        honest session-count heuristic, not a skill-based rubric match like rules/peers.py.
        """
        me = str(user.user_id)
        own_sessions = await self.db.missions.count_documents({'user_id': me})
        candidates = await self.db.users.find({'_id': {'$ne': me}, 'discoverable': True}, {'password_hash': 0, 'email': 0}).to_list(length=200)
        if not candidates: return []
        ids = [c['_id'] for c in candidates]
        counts = {row['_id']: row['n'] for row in await self.db.missions.aggregate([{'$match': {'user_id': {'$in': ids}}}, {'$group': {'_id': '$user_id', 'n': {'$sum': 1}}}]).to_list(length=None)}
        relations = await self.db.talk_requests.find({'$or': [{'from_user_id': me, 'to_user_id': {'$in': ids}}, {'to_user_id': me, 'from_user_id': {'$in': ids}}], 'status': {'$in': ['pending', 'accepted']}}).to_list(length=None)
        status_by_peer = {}
        for row in relations:
            other = row['to_user_id'] if row['from_user_id'] == me else row['from_user_id']
            status_by_peer[other] = 'connected' if row['status'] == 'accepted' else ('pending_outgoing' if row['from_user_id'] == me else 'pending_incoming')
        rows = []
        for candidate in candidates:
            cid = candidate['_id']
            sessions = counts.get(cid, 0)
            rows.append({'user_id': cid, 'display_name': candidate['display_name'], 'username': candidate.get('username'),
                         'sessions': sessions, 'status': status_by_peer.get(cid, 'none'), '_proximity': abs(sessions - own_sessions)})
        rows.sort(key=lambda row: (row['_proximity'], row['display_name']))
        for row in rows: del row['_proximity']
        return rows[:limit]
    async def create_request(self, user, target_id):
        me, target = str(user.user_id), str(target_id)
        if me == target: raise HTTPException(400, 'You cannot send a request to yourself.')
        target_row = await self.db.users.find_one({'_id': target, 'discoverable': True})
        if not target_row: raise HTTPException(404, 'Player not found.')
        row = dict(_id=str(uuid4()), from_user_id=me, to_user_id=target, pair_key=pair_key(me, target), status='pending', created_at=now(), responded_at=None)
        try: await self.db.talk_requests.insert_one(row)
        except DuplicateKeyError: raise HTTPException(409, 'A request already exists with this player.') from None
        return row
    async def respond_request(self, user, request_id, accept):
        row = await self.db.talk_requests.find_one_and_update(
            {'_id': str(request_id), 'to_user_id': str(user.user_id), 'status': 'pending'},
            {'$set': {'status': 'accepted' if accept else 'declined', 'responded_at': now()}},
            return_document=ReturnDocument.AFTER)
        if not row: raise HTTPException(404, 'Request not found or already answered.')
        return row
    async def list_requests(self, user):
        me = str(user.user_id)
        rows = await self.db.talk_requests.find({'$or': [{'from_user_id': me}, {'to_user_id': me}]}).to_list(length=200)
        others = {r['to_user_id'] if r['from_user_id'] == me else r['from_user_id'] for r in rows}
        cards = {p['_id']: player_card({**p, 'user_id': p['_id']}) for p in await self.db.users.find({'_id': {'$in': list(others)}}).to_list(length=None)} if others else {}
        incoming, outgoing, contacts = [], [], []
        for row in rows:
            other = row['to_user_id'] if row['from_user_id'] == me else row['from_user_id']
            entry = {'request_id': row['_id'], 'player': cards.get(other, {'user_id': other, 'display_name': 'Former player', 'username': None}),
                     'created_at': row['created_at'], 'status': row['status']}
            if row['status'] == 'accepted': contacts.append(entry)
            elif row['status'] == 'pending' and row['to_user_id'] == me: incoming.append(entry)
            elif row['status'] == 'pending' and row['from_user_id'] == me: outgoing.append(entry)
        return {'incoming': incoming, 'outgoing': outgoing, 'contacts': contacts}
    async def connected(self, a, b): return await self.db.talk_requests.find_one({'pair_key': pair_key(a, b), 'status': 'accepted'})
    async def send_message(self, user, other_id, body):
        me, other = str(user.user_id), str(other_id)
        if me == other: raise HTTPException(400, 'Invalid recipient.')
        if not await self.connected(me, other): raise HTTPException(403, 'You are not connected with this player yet.')
        row = dict(_id=str(uuid4()), pair_key=pair_key(me, other), from_user_id=me, to_user_id=other, body=body, created_at=now())
        await self.db.chat_messages.insert_one(row)
        return row
    async def list_messages(self, user, other_id, since=None):
        me, other = str(user.user_id), str(other_id)
        if not await self.connected(me, other): raise HTTPException(403, 'You are not connected with this player yet.')
        query = {'pair_key': pair_key(me, other)}
        if since: query['created_at'] = {'$gt': since}
        return await self.db.chat_messages.find(query).sort('created_at', 1).to_list(length=200)
