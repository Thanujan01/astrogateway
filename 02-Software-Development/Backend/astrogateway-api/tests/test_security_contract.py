"""Offline contract tests. Synthetic tokens accepted only by an isolated mock Auth transport."""
import base64,json,time
from uuid import UUID,uuid4
from datetime import datetime,timezone,timedelta
import httpx,pytest
from fastapi import HTTPException
from fastapi.testclient import TestClient
from main import Settings,create_app
U=UUID('11111111-1111-4111-8111-111111111111'); S=uuid4()
URL='https://fixture.supabase.co'
def token(**changes):
    claims=dict(iss=URL+'/auth/v1',aud='authenticated',role='authenticated',sub=str(U),session_id=str(S),exp=int(time.time())+300);claims.update(changes)
    enc=lambda x:base64.urlsafe_b64encode(json.dumps(x).encode()).decode().rstrip('=')
    return enc({'alg':'ES256'})+'.'+enc(claims)+'.fixture-not-a-signature'
class Repo:
    def __init__(self):self.rows={};self.revocations=set()
    async def revoked(self,u):return u.auth_session_id in self.revocations
    async def profile(self,u):return dict(user_id=u.user_id,display_name='Fixture',trainee_profile='astronaut_candidate',training_level=0,created_at=datetime.now(timezone.utc))
    async def initialize(self,u,p):
        now=datetime.now(timezone.utc);r=dict(id=uuid4(),user_id=u.user_id,auth_session_id=u.auth_session_id,launch_id=p.launch_id,mission_id=p.mission_id,status='initialized',created_at=now,expires_at=now+timedelta(minutes=30),completed_at=None);self.rows[r['id']]=r;return r
    async def complete(self,u,s,p):
        r=self.rows.get(s)
        if not r or r['user_id']!=u.user_id:raise HTTPException(404,'Unavailable')
        if r['launch_id']!=p.launch_id or r['auth_session_id']!=u.auth_session_id:raise HTTPException(409,'Mismatch')
        r.update(status='integration_check_complete',completed_at=r['completed_at'] or datetime.now(timezone.utc));return r
    async def revoke(self,u):self.revocations.add(u.auth_session_id)
def setup(status=200,confirmed=True):
    repo=Repo()
    def auth(req):return httpx.Response(status,json={'id':str(U),'email_confirmed_at':'2026-01-01' if confirmed else None})
    return create_app(Settings(URL,'sb_publishable_fixture'),httpx.MockTransport(auth),repo),repo
@pytest.mark.parametrize('claims',[{'exp':1},{'iss':'https://evil/auth/v1'},{'aud':'other'},{'role':'service_role'},{'sub':str(uuid4())},{'session_id':'invalid'}])
def test_bad_claims(claims):
    app,_=setup()
    with TestClient(app) as c:assert c.get('/v1/me',headers={'Authorization':'Bearer '+token(**claims)}).status_code==401
@pytest.mark.parametrize('status,expected',[(401,401),(403,401),(429,429),(500,503)])
def test_auth_server_rejections(status,expected):
    app,_=setup(status)
    with TestClient(app) as c:assert c.get('/v1/me',headers={'Authorization':'Bearer '+token()}).status_code==expected
@pytest.mark.parametrize('value',['','Bearer broken','Bearer '+('x'*8193),'Basic abc'])
def test_missing_malformed_authorization(value):
    app,_=setup()
    with TestClient(app) as c:assert c.get('/v1/me',headers={'Authorization':value}).status_code==401

def test_unconfirmed_email_is_allowed():
    """Email verification was removed: sign up, then sign in with the same credentials."""
    app,_=setup(confirmed=False)
    with TestClient(app) as c:assert c.get('/v1/me',headers={'Authorization':'Bearer '+token()}).status_code==200

def test_profile_mission_completion_and_revocation():
    app,_=setup();h={'Authorization':'Bearer '+token()};launch=str(uuid4())
    with TestClient(app) as c:
        assert c.get('/v1/me',headers=h).json()['user_id']==str(U)
        body={'launch_id':launch,'mission_id':'mission-red-horizon-poc'}
        assert c.post('/v1/mission-sessions',headers=h,json={**body,'user_id':str(uuid4())}).status_code==422
        r=c.post('/v1/mission-sessions',headers=h,json=body);assert r.status_code==201
        row=r.json();assert 'auth_session_id' not in row
        path='/v1/mission-sessions/'+row['id']+'/complete'
        assert c.post(path,headers=h,json={'launch_id':str(uuid4()),'outcome':'integration_check'}).status_code==409
        assert c.post(path,headers=h,json={'launch_id':launch,'outcome':'integration_check','score':100}).status_code==422
        body={'launch_id':launch,'outcome':'integration_check'}
        a=c.post(path,headers=h,json=body);b=c.post(path,headers=h,json=body);assert a.status_code==200 and a.json()==b.json()
        assert c.post('/v1/auth-session/revoke',headers=h).status_code==200
        assert c.get('/v1/me',headers=h).status_code==401
        assert c.get('/v1/me',headers={'Authorization':'Bearer '+token(session_id=str(uuid4()))}).status_code==200

def test_unconfigured_fails_closed():
    with TestClient(create_app(Settings())) as c:
        assert c.get('/health').json()['status']=='configuration_required'
        assert c.get('/v1/me').status_code==401
        assert c.get('/v1/me',headers={'Authorization':'Bearer '+token()}).status_code==503
@pytest.mark.parametrize('key',['sb_secret_fixture','not-a-public-key'])
def test_secrets_rejected(key):
    with pytest.raises(ValueError):Settings(URL,key)

def test_no_password_or_second_identity_endpoints():
    app,_=setup()
    paths=app.openapi()['paths'];assert not any('password' in p or 'signup' in p or 'login' in p for p in paths)
