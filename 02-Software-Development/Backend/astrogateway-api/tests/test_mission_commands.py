"""Step 3 mission command/state/report/skills/recommendation endpoints. Local mock Auth + repository only."""
import json
from datetime import datetime,timezone,timedelta
from uuid import UUID,uuid4
import httpx,pytest
from fastapi import HTTPException
from fastapi.testclient import TestClient
from main import Settings,MissionCommandIn,create_app,MAX_COMMANDS_PER_SESSION
from database import Database
from test_security_contract import Repo,U,URL,token
from test_postgrest_repository import user

OTHER=UUID('22222222-2222-4222-8222-222222222222')
class MissionRepo(Repo):
    """In-memory mirror of the 005 SQL semantics (ownership, idempotency, revision, open session, limit)."""
    def __init__(self):super().__init__();self.commands={}
    def own(self,u,s):
        r=self.rows.get(s)
        if not r or r['user_id']!=u.user_id:raise HTTPException(404,'Mission or profile unavailable.')
        return r
    async def submit_command(self,u,s,c):
        r=self.own(u,s);log=self.commands.setdefault(s,[])
        for e in log:
            if e['command_id']==c.command_id:
                if (e['kind'],e['payload'],e['revision'])==(c.kind,c.payload,c.expected_revision):return {**self.ack(s,e),'duplicate':True}
                raise HTTPException(409,'Mission revision or command conflict.')
        if r['auth_session_id']!=u.auth_session_id or r['status']!='initialized' or r['expires_at']<=datetime.now(timezone.utc):raise HTTPException(409,'Mission launch/session conflict.')
        if len(log)>=MAX_COMMANDS_PER_SESSION:raise HTTPException(429,'Mission command limit reached.')
        if c.expected_revision!=len(log):raise HTTPException(409,'Mission revision or command conflict.')
        e=dict(command_id=c.command_id,kind=c.kind,payload=c.payload,revision=len(log),accepted_at=datetime.now(timezone.utc));log.append(e)
        return {**self.ack(s,e),'duplicate':False}
    def ack(self,s,e):return dict(session_id=s,command_id=e['command_id'],kind=e['kind'],revision=e['revision']+1,accepted_at=e['accepted_at'])
    def evidence(self,logs):
        kinds=[e['kind'] for log in logs for e in log]
        return dict(simulated_scans=kinds.count('scan_target'),rover_board_exit_cycles=min(kinds.count('board_rover'),kinds.count('exit_rover')),weather_changes=kinds.count('weather_changed'))
    async def mission_state(self,u,s):
        r=self.own(u,s);log=self.commands.get(s,[]);pauses=[e['kind'] for e in log if e['kind'] in ('pause','resume')]
        row={k:v for k,v in r.items() if k not in ('auth_session_id','launch_id')}
        return {**row,'revision':len(log),'paused':bool(pauses) and pauses[-1]=='pause','last_command_at':log[-1]['accepted_at'] if log else None}
    async def mission_report(self,u,s):
        r=self.own(u,s)
        return dict(session_id=s,user_id=u.user_id,mission_id=r['mission_id'],status=r['status'],evidence=self.evidence([self.commands.get(s,[])]),score=None,rubric_status='pending_approval')
    async def skill_evidence(self,u):
        mine=[s for s,r in self.rows.items() if r['user_id']==u.user_id]
        return dict(user_id=u.user_id,sessions=len(mine),evidence=self.evidence([self.commands.get(s,[]) for s in mine]),levels=None,rubric_status='pending_approval')

def setup(confirmed=True):
    repo=MissionRepo();current={'id':U}
    def auth(req):return httpx.Response(200,json={'id':str(current['id']),'email_confirmed_at':'2026-01-01' if confirmed else None})
    return create_app(Settings(URL,'sb_publishable_fixture'),httpx.MockTransport(auth),repo),repo,current
H=lambda **k:{'Authorization':'Bearer '+token(**k)}
def start(c,h):
    r=c.post('/v1/mission-sessions',headers=h,json={'launch_id':str(uuid4()),'mission_id':'mission-red-horizon-poc'});assert r.status_code==201;return r.json()['id']
def cmd(rev,kind,payload=None,cid=None):return {'command_id':str(cid or uuid4()),'expected_revision':rev,'kind':kind,'payload':payload or {}}
SCAN={'sample_id':str(uuid4()),'tick':12,'analysis':'unverified_simulated'}

NEW_ENDPOINTS=[('post','/v1/mission-sessions/{s}/commands'),('get','/v1/mission-sessions/{s}'),('get','/v1/reports/{s}'),('get','/v1/skills'),('get','/v1/recommendations')]
@pytest.mark.parametrize('method,path',NEW_ENDPOINTS)
@pytest.mark.parametrize('headers,expected',[({},401),({'Authorization':'Bearer broken'},401),({'Authorization':'Basic abc'},401),(H(exp=1),401),(H(role='service_role'),401),(H(iss='https://evil/auth/v1'),401),(H(sub=str(uuid4())),401)])
def test_new_endpoints_require_verified_session(method,path,headers,expected):
    app,_,_=setup()
    with TestClient(app) as c:
        kw={'json':cmd(0,'board_rover')} if method=='post' else {}
        assert getattr(c,method)(path.format(s=uuid4()),headers=headers,**kw).status_code==expected
@pytest.mark.parametrize('method,path',NEW_ENDPOINTS)
def test_new_endpoints_do_not_require_confirmed_email(method,path):
    """Email verification was removed: sign up, then sign in with the same credentials."""
    app,_,_=setup(confirmed=False)
    with TestClient(app) as c:
        kw={'json':cmd(0,'board_rover')} if method=='post' else {}
        assert getattr(c,method)(path.format(s=uuid4()),headers=H(),**kw).status_code!=403
@pytest.mark.parametrize('method,path',NEW_ENDPOINTS)
def test_revoked_session_is_refused(method,path):
    app,_,_=setup();h=H()
    with TestClient(app) as c:
        s=start(c,h);assert c.post('/v1/auth-session/revoke',headers=h).status_code==200
        kw={'json':cmd(0,'board_rover')} if method=='post' else {}
        assert getattr(c,method)(path.format(s=s),headers=h,**kw).status_code==401

def test_command_flow_state_report_skills():
    app,repo,_=setup();h=H()
    with TestClient(app) as c:
        s=start(c,h);base='/v1/mission-sessions/'+s
        kinds=[('board_rover',{}),('exit_rover',{}),('scan_target',SCAN),('weather_changed',{'dust':2}),('pause',{})]
        for rev,(kind,payload) in enumerate(kinds):
            r=c.post(base+'/commands',headers=h,json=cmd(rev,kind,payload));assert r.status_code==200,r.text
            assert r.json()['revision']==rev+1 and r.json()['duplicate'] is False
        st=c.get(base,headers=h).json()
        assert st['revision']==5 and st['paused'] is True and st['user_id']==str(U) and 'auth_session_id' not in st and 'launch_id' not in st
        assert c.post(base+'/commands',headers=h,json=cmd(5,'resume')).status_code==200
        assert c.get(base,headers=h).json()['paused'] is False
        rep=c.get('/v1/reports/'+s,headers=h).json()
        assert rep['evidence']=={'simulated_scans':1,'rover_board_exit_cycles':1,'weather_changes':1}
        assert rep['score'] is None and rep['rubric_status']=='pending_approval'
        sk=c.get('/v1/skills',headers=h).json()
        assert sk['sessions']==1 and sk['levels'] is None and sk['evidence']['simulated_scans']==1

def test_idempotent_replay_and_conflicts():
    app,_,_=setup();h=H()
    with TestClient(app) as c:
        s=start(c,h);path='/v1/mission-sessions/'+s+'/commands';body=cmd(0,'scan_target',SCAN)
        a=c.post(path,headers=h,json=body);b=c.post(path,headers=h,json=body)
        assert a.status_code==b.status_code==200 and b.json()['duplicate'] is True and a.json()['revision']==b.json()['revision']==1
        assert c.post(path,headers=h,json={**body,'kind':'board_rover','payload':{}}).status_code==409   # reused id, new content
        assert c.post(path,headers=h,json=cmd(0,'board_rover')).status_code==409                       # stale revision
        assert c.post(path,headers=h,json=cmd(5,'board_rover')).status_code==409                       # future revision
        assert c.post(path,headers=h,json=cmd(1,'board_rover')).status_code==200

@pytest.mark.parametrize('body',[
    {'command_id':str(uuid4()),'expected_revision':0,'kind':'board_rover','payload':{},'user_id':str(OTHER)},
    {'command_id':str(uuid4()),'expected_revision':0,'kind':'board_rover','payload':{},'score':100},
    cmd(0,'complete'),cmd(0,'set_oxygen',{'oxygen':100}),cmd(-1,'board_rover'),cmd(MAX_COMMANDS_PER_SESSION+1,'board_rover'),
    cmd(0,'board_rover',{'speed':5}),cmd(0,'scan_target',{**SCAN,'analysis':'verified'}),cmd(0,'scan_target',{**SCAN,'score':10}),
    cmd(0,'scan_target',{'tick':1,'analysis':'unverified_simulated'}),cmd(0,'scan_target',{**SCAN,'sample_id':'not-a-uuid'}),
    cmd(0,'weather_changed',{'dust':3}),cmd(0,'weather_changed',{'dust':1,'oxygen_rate':0.5}),cmd(0,'resume',{'note':'x'*2000}),
    {'command_id':'not-a-uuid','expected_revision':0,'kind':'pause','payload':{}},{'expected_revision':0,'kind':'pause','payload':{}},
    {'command_id':str(uuid4()),'expected_revision':0,'kind':'pause','payload':[]}])
def test_invalid_commands_rejected_before_storage(body):
    app,repo,_=setup();h=H()
    with TestClient(app) as c:
        s=start(c,h)
        assert c.post('/v1/mission-sessions/'+s+'/commands',headers=h,json=body).status_code==422
    assert repo.commands.get(UUID(s),[])==[]

def test_other_users_session_is_not_visible():
    app,_,current=setup()
    with TestClient(app) as c:
        s=start(c,H())
        current['id']=OTHER;other=H(sub=str(OTHER),session_id=str(uuid4()))
        assert c.post('/v1/mission-sessions/'+s+'/commands',headers=other,json=cmd(0,'board_rover')).status_code==404
        assert c.get('/v1/mission-sessions/'+s,headers=other).status_code==404
        assert c.get('/v1/reports/'+s,headers=other).status_code==404
        assert c.get('/v1/skills',headers=other).json()['sessions']==0

def test_commands_need_the_creating_auth_session_and_an_open_mission():
    app,repo,_=setup();h=H()
    with TestClient(app) as c:
        s=start(c,h)
        assert c.post('/v1/mission-sessions/'+s+'/commands',headers=H(session_id=str(uuid4())),json=cmd(0,'board_rover')).status_code==409
        repo.rows[UUID(s)]['expires_at']=datetime.now(timezone.utc)-timedelta(seconds=1)
        assert c.post('/v1/mission-sessions/'+s+'/commands',headers=h,json=cmd(0,'board_rover')).status_code==409

def test_command_limit():
    app,repo,_=setup();h=H()
    with TestClient(app) as c:
        s=start(c,h);repo.commands[UUID(s)]=[dict(command_id=uuid4(),kind='pause',payload={},revision=i,accepted_at=datetime.now(timezone.utc)) for i in range(MAX_COMMANDS_PER_SESSION)]
        assert c.post('/v1/mission-sessions/'+s+'/commands',headers=h,json=cmd(MAX_COMMANDS_PER_SESSION,'resume')).status_code==429

def test_recommendation_is_labelled_placeholder():
    app,_,_=setup()
    with TestClient(app) as c:
        r=c.get('/v1/recommendations',headers=H()).json()
    assert r=={'mission_id':'mission-red-horizon-poc','reason':'Only published mission. Recommendation rules are pending approval.','rule_status':'pending_approval'}

def test_responses_cannot_carry_scores_or_levels():
    app,_,_=setup();schemas=app.openapi()['components']['schemas']
    assert set(schemas['MissionCommandIn']['properties'])=={'command_id','expected_revision','kind','payload'}
    assert schemas['MissionReport']['properties']['score'].get('type')=='null'
    assert schemas['SkillsSummary']['properties']['levels'].get('type')=='null'

def test_forged_ownership_in_repository_response_is_rejected():
    app,repo,_=setup();h=H()
    async def forged(u,s):return {'session_id':s,'user_id':OTHER,'mission_id':'mission-red-horizon-poc','status':'initialized','evidence':{'simulated_scans':0,'rover_board_exit_cycles':0,'weather_changes':0}}
    repo.mission_report=forged
    with TestClient(app) as c:assert c.get('/v1/reports/'+start(c,h),headers=h).status_code==502

# ---------- repository (PostgREST) layer ----------
@pytest.mark.asyncio
async def test_new_rpcs_forward_only_the_original_token():
    calls=[]
    def gateway(request):
        calls.append((request.url.path,dict(request.headers),json.loads(request.content)));return httpx.Response(200,json={'ok':True})
    async with httpx.AsyncClient(transport=httpx.MockTransport(gateway)) as http:
        db=Database(http,Settings(URL,'sb_publishable_fixture'));u=user();s=uuid4()
        await db.submit_command(u,s,MissionCommandIn(**cmd(0,'scan_target',SCAN)));await db.mission_state(u,s);await db.mission_report(u,s);await db.skill_evidence(u)
    assert [p.rsplit('/',1)[1] for p,_,_ in calls]==['submit_mission_command','mission_state','mission_report','skill_evidence']
    for path,headers,body in calls:
        assert headers['authorization']=='Bearer '+u.access_token and headers['content-profile']==headers['accept-profile']=='mission_api'
        assert not any(k in body for k in ('user_id','auth_session_id','p_user_id','p_auth_session_id','score','p_score'))
    assert calls[0][2]['p_payload']==SCAN and calls[0][2]['p_kind']=='scan_target' and calls[0][2]['p_expected_revision']==0

@pytest.mark.asyncio
@pytest.mark.parametrize('code,expected',[('PC422',422),('PC409',409),('PC429',429),('PT404',404),('PT409',409),('PT401',401),('23505',409)])
async def test_new_sql_codes_mapped_and_sanitized(code,expected):
    def fail(request):return httpx.Response(400,json={'code':code,'message':'sensitive-fixture-canary','details':'never display'})
    async with httpx.AsyncClient(transport=httpx.MockTransport(fail)) as http:
        with pytest.raises(HTTPException) as e:await Database(http,Settings(URL,'sb_publishable_fixture')).submit_command(user(),uuid4(),MissionCommandIn(**cmd(0,'pause')))
    assert e.value.status_code==expected and 'sensitive-fixture-canary' not in e.value.detail
