import sys,json,base64,subprocess,os,uuid,time
from pathlib import Path
import httpx
from fastapi.testclient import TestClient
from main import create_app,Settings
from adaptive.engine import SKILLS
INPUT=json.loads(sys.stdin.read());URL=INPUT['url'];A=INPUT['A'];B=INPUT['B'];port=INPUT['port'];tokens=INPUT['tokens'];tests=[]
def check(n,ok):
 tests.append({'name':n,'status':'PASS' if ok else 'FAIL'})
 if not ok:raise AssertionError(n)
class Transport(httpx.AsyncBaseTransport):
 async def handle_async_request(self,req):
  if req.url.path=='/auth/v1/user':
   token=req.headers.get('Authorization','').removeprefix('Bearer ')
   try:
    h,b,s=token.split('.');header=json.loads(base64.urlsafe_b64decode(h+'='*(-len(h)%4)));claims=json.loads(base64.urlsafe_b64decode(b+'='*(-len(b)%4)))
    sig=Path(os.environ['TMPDIR'])/'sign.bin';sig.write_bytes(base64.urlsafe_b64decode(s+'='*(-len(s)%4)))
    verify=subprocess.run(['/usr/bin/openssl','dgst','-sha256','-verify',str(Path(__file__).parent/'issuer.pub'),'-signature',str(sig)],input=(h+'.'+b).encode(),capture_output=True)
    if verify.returncode or header.get('alg')!='RS256':return httpx.Response(401)
    return httpx.Response(200,json={'id':claims['sub'],'email_confirmed_at':'2026-01-01'})
   except Exception:return httpx.Response(401)
  if req.url.path.startswith('/rest/v1/rpc/'):
   async with httpx.AsyncClient(trust_env=False) as c:
    res=await c.post('http://127.0.0.1:'+str(port)+req.url.path.removeprefix('/rest/v1'),headers=dict(req.headers),content=req.content)
   return httpx.Response(res.status_code,content=res.content)
  raise AssertionError('Unexpected URL')
def event(kind='movement',elapsed=1,**kw):
 data={'movement':dict(x_m=-45,z_m=-60,speed_mps=2.5),'reaction':dict(reaction_s=2),'resource':dict(oxygen_used_l=50,water_used_l=.2,power_used_wh=100),'equipment':dict(equipment='solar_array',correct=True),'emergency':dict(incident='dust',resolved=True,response_s=12),'mistake':dict(category='navigation',retries=0),'completion':dict(completed=True)}
 return dict(event_id=str(uuid.uuid4()),kind=kind,elapsed_s=elapsed,**{**data[kind],**kw})
ha={'Authorization':'Bearer '+tokens['A']};hb={'Authorization':'Bearer '+tokens['B']}
try:
 with TestClient(create_app(Settings(URL,'sb_publishable_local_fixture'),Transport())) as c:
  for name,t in [('invalid','broken'),('expired',tokens['expired']),('forged',tokens['forged'])]:check(name+' token denied',c.get('/ai/skills/me',headers={'Authorization':'Bearer '+t}).status_code==401)
  check('missing auth denied',c.get('/ai/skills/me').status_code==401)
  check('AI endpoint contract mounted',all(p in c.app.openapi()['paths'] for p in ['/ai/telemetry','/ai/skills/me','/ai/mission/recommend','/ai/training-plan/me','/ai/debrief']))
  check('empty skills insufficient',all(s['score'] is None for s in c.get('/ai/skills/me',headers=ha).json()['skills'].values()))
  r=c.post('/v1/mission-sessions',headers=ha,json={'launch_id':str(uuid.uuid4()),'mission_id':'mission-red-horizon-poc'});check('existing mission init intact',r.status_code==201);mid=r.json()['id']
  body={'mission_session_id':mid,'events':[event()]}
  check('B cannot ingest A mission',c.post('/ai/telemetry',headers=hb,json=body).status_code==404)
  check('owner injection rejected',c.post('/ai/telemetry',headers=ha,json={**body,'user_id':B}).status_code==422)
  check('client score rejected',c.post('/ai/telemetry',headers=ha,json={**body,'events':[event(score=100)]}).status_code==422)
  check('attestation injection rejected',c.post('/ai/telemetry',headers=ha,json={**body,'events':[event(attested=True)]}).status_code==422)
  check('valid event persisted by RLS',c.post('/ai/telemetry',headers=ha,json=body).json()['accepted']==1)
  check('whole batch idempotent',c.post('/ai/telemetry',headers=ha,json=body).json()['replayed']==1)
  changed=json.loads(json.dumps(body));changed['events'][0]['speed_mps']=3
  check('changed event replay conflict',c.post('/ai/telemetry',headers=ha,json=changed).status_code==409)
  check('cross-batch impossible movement rejected',c.post('/ai/telemetry',headers=ha,json={'mission_session_id':mid,'events':[event(elapsed=2,x_m=125)]}).status_code==422)
  check('B skills isolated',c.get('/ai/skills/me',headers=hb).json()['skills']['navigation']['score'] is None)
  check('A score derived provisional',c.get('/ai/skills/me',headers=ha).json()['skills']['navigation']['basis']=='client_reported')
  check('B debrief denied',c.post('/ai/debrief',headers=hb,json={'mission_session_id':mid}).status_code==404)
  events=[event('reaction',2),event('resource',3),event('equipment',4),event('emergency',5),event('mistake',6),event('completion',7)]
  check('all schema types accepted',c.post('/ai/telemetry',headers=ha,json={'mission_session_id':mid,'events':events}).json()['accepted']==6)
  check('late events after completion denied',c.post('/ai/telemetry',headers=ha,json={'mission_session_id':mid,'events':[event(elapsed=8)]}).status_code==409)
  rec=c.post('/ai/mission/recommend',headers=ha,json={'previous':'Hard'}).json();check('fake previous cannot skip tiers',rec['difficulty']=='Easy' and not rec['eligible_promotions'])
  pa=c.get('/ai/training-plan/me',headers=ha).json();pb=c.get('/ai/training-plan/me',headers=hb).json()
  check('30 day plan repeatable',len(pa['days'])==30 and pa==c.get('/ai/training-plan/me',headers=ha).json())
  check('user scoped seed variation',pa['seed']!=pb['seed'])
  check('client completion cannot mark trusted progress',pa['completed_days']==0 and pa['provisional_completed_missions']==1)
  d=c.post('/ai/debrief',headers=ha,json={'mission_session_id':mid}).json();check('grounded ARIA cites source without model claims',d['science'][0]['url']=='https://science.nasa.gov/mars/facts/' and not d['generated_by_model'])
  check('revocation blocks AI',c.post('/v1/auth-session/revoke',headers=ha).status_code==200 and c.get('/ai/skills/me',headers=ha).status_code==401)
  check('B remains accessible',c.get('/ai/skills/me',headers=hb).status_code==200)
 print(json.dumps({'tests':tests,'local_only':True,'kernel_sandbox':True,'auth':'public RSA fixture','data':'real PostgREST16.4/native PostgreSQL17.11','no_client_final_scores':True},indent=2))
except Exception as err:
 print(json.dumps({'tests':tests,'failure_type':type(err).__name__},indent=2));raise SystemExit(1)
