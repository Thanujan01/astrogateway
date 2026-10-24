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
with TestClient(create_app(Settings(URL,'sb_publishable_local_fixture'),Transport())) as c:
 result=c.get('/ai/skills/me',headers=ha).json()['skills']
 check('six private attestations determine assessed skills',all(x['score']==90 and x['basis']=='server_attested' and x['confidence']==.6 for x in result.values()))
 check('three Easy then three Medium unlock Hard',c.post('/ai/mission/recommend',headers=ha,json={'previous':'Easy'}).json()['difficulty']=='Hard')
 check('private attestations absent for B',all(x['score'] is None for x in c.get('/ai/skills/me',headers=hb).json()['skills'].values()))
 p=c.get('/ai/training-plan/me',headers=ha).json()
 check('trusted progress recovered from PostgreSQL',p['completed_days']==6 and p['level']=='intermediate')
 check('oversized telemetry body rejected',c.post('/ai/telemetry',headers=ha,content='x'*131073).status_code==413)
 check('recommended difficulty cannot jump from fresh B previous',c.post('/ai/mission/recommend',headers=hb,json={'previous':'Hard'}).json()['difficulty']=='Easy')
print(json.dumps({'tests':tests,'local_only':True,'synthetic_server_attestations':True,'kernel_sandbox':True},indent=2))
