import json,asyncio,base64,uuid,sys,time
from pathlib import Path
import httpx
from fastapi.testclient import TestClient
from lab_common import URL,CFG,A,B,C,signed_token,fixture_authenticate
from main import Settings,create_app,Principal
from database import Database
from datetime import datetime,timezone,timedelta
OUT=Path('outputs/Stage05B2/Evidence');results=[]
class GatewayTransport(httpx.AsyncBaseTransport):
 def __init__(self,bypass_auth=False):self.bypass_auth=bypass_auth;self.calls=[]
 async def handle_async_request(self,request):
  if request.url.path=='/auth/v1/user':
   token=request.headers.get('Authorization','').removeprefix('Bearer ')
   if self.bypass_auth:
    try:body=token.split('.')[1];claims=json.loads(base64.urlsafe_b64decode(body+'='*(-len(body)%4)))
    except Exception:return httpx.Response(401)
   else:claims=fixture_authenticate(token)
   if not claims:return httpx.Response(401)
   return httpx.Response(200,json={'id':claims['sub'],'email_confirmed_at':None if claims['sub']==C else '2026-01-01','is_anonymous':False})
  if request.url.path.startswith('/rest/v1/rpc/'):
   body=json.loads(request.content);self.calls.append({'rpc':request.url.path.rsplit('/',1)[1],'argument_names':sorted(body),'identity_header_override':any(k in request.headers for k in ('x-user-id','request.jwt.claim.sub','request.jwt.claims')),'schema':request.headers.get('content-profile')})
   async with httpx.AsyncClient(timeout=10,trust_env=False) as client:
    r=await client.post('http://127.0.0.1:'+str(CFG['rest_port'])+request.url.path.removeprefix('/rest/v1'),headers=dict(request.headers),content=request.content)
   return httpx.Response(r.status_code,content=r.content,headers={'content-type':'application/json'})
  raise AssertionError('Unexpected service request')
def mark(name,ok):
 results.append({'name':name,'status':'PASS' if ok else 'FAIL'});print('PASS' if ok else 'FAIL',name)
 if not ok:raise AssertionError(name)
settings=Settings(URL,'sb_publishable_local_fixture');transport=GatewayTransport();sid=str(uuid.uuid4());ta=signed_token(A,sid);tb=signed_token(B,str(uuid.uuid4()));ha={'Authorization':'Bearer '+ta};hb={'Authorization':'Bearer '+tb}
try:
 with TestClient(create_app(settings,transport)) as client:
  mark('FastAPI health reflects public configuration only, no DB credential',client.get('/health').json()['status']=='ok')
  mark('Android-compatible /v1/me contract survives actual gateway',client.get('/v1/me',headers=ha).json()['user_id']==A)
  r=client.get('/v1/me',headers={**ha,'x-user-id':B,'request.jwt.claim.sub':B});mark('HTTP attacker identity headers ignored by backend',r.status_code==200 and r.json()['user_id']==A)
  launch=str(uuid.uuid4());payload={'launch_id':launch,'mission_id':'mission-red-horizon-poc'}
  r=client.post('/v1/mission-sessions',headers=ha,json={**payload,'user_id':B});mark('API owner field still forbidden',r.status_code==422)
  r=client.post('/v1/mission-sessions',headers=ha,json=payload);mark('existing mission initialization contract returns201 and IDs only',r.status_code==201 and r.json()['user_id']==A and 'auth_session_id' not in r.json());mid=r.json()['id']
  path='/v1/mission-sessions/'+mid+'/complete';body={'launch_id':launch,'outcome':'integration_check'}
  mark('B cannot complete A via unchanged FastAPI endpoint',client.post(path,headers=hb,json=body).status_code==404)
  mark('wrong launch returns conflict',client.post(path,headers=ha,json={**body,'launch_id':str(uuid.uuid4())}).status_code==409)
  r=client.post(path,headers=ha,json=body);repeat=client.post(path,headers=ha,json=body);mark('completion and idempotent repeat contract preserved',r.status_code==repeat.status_code==200 and r.json()==repeat.json())
  mark('email confirmation still required at FastAPI',client.get('/v1/me',headers={'Authorization':'Bearer '+signed_token(C,str(uuid.uuid4()))}).status_code==403)
  mark('expired original token rejected before data access',client.get('/v1/me',headers={'Authorization':'Bearer '+signed_token(exp=int(time.time())-1)}).status_code==401)
  mark('signout/revocation contract preserved',client.post('/v1/auth-session/revoke',headers=ha).status_code==200 and client.get('/v1/me',headers=ha).status_code==401)
  mark('B unaffected after A signout',client.get('/v1/me',headers=hb).status_code==200)
 mark('repository forwards no caller identity headers or owner/session parameters',all(not r['identity_header_override'] and r['schema']=='mission_api' and not any('user_id' in key or 'auth_session' in key for key in r['argument_names']) for r in transport.calls))
 # Deliberately bypass host verifier: independent gateway must still reject a forged subject.
 valid=signed_token(A,str(uuid.uuid4()));h,b,s=valid.split('.');claims=json.loads(base64.urlsafe_b64decode(b+'='*(-len(b)%4)));claims['sub']=B
 forged=h+'.'+base64.urlsafe_b64encode(json.dumps(claims).encode()).decode().rstrip('=')+'.'+s
 with TestClient(create_app(settings,GatewayTransport(True))) as client:
  mark('even deliberately bypassed FastAPI verifier cannot forge B at independent gateway',client.get('/v1/me',headers={'Authorization':'Bearer '+forged}).status_code==401)
 async def mismatched():
  fresh=signed_token(A,str(uuid.uuid4()));principal=Principal(uuid.UUID(B),uuid.uuid4(),datetime.now(timezone.utc)+timedelta(minutes=10),fresh)
  async with httpx.AsyncClient(transport=GatewayTransport()) as http:
   row=await Database(http,settings).profile(principal)
  mark('forged internal Principal B with token A yields only A from gateway',row['user_id']==A)
 asyncio.run(mismatched())
 # Honest residual: possession of a legitimate observed B bearer confers B authority.
 with TestClient(create_app(settings,GatewayTransport())) as client:
  mark('valid B bearer replay returns B as expected bearer-token residual',client.get('/v1/me',headers=hb).json()['user_id']==B)
 (OUT/'api-gateway-integration.json').write_text(json.dumps({'live_supabase':False,'auth_service':'local RSA fixture','data_gateway':'real local PostgREST16.4','tests':results,'sanitized_rpc_calls':transport.calls,'bearer_replay_risk_observed':True},indent=2))
except Exception as e:
 (OUT/'api-integration-failure.json').write_text(json.dumps({'error_type':type(e).__name__,'tests':results},indent=2));print('FAIL local API integration; sanitized failure recorded.');raise SystemExit(1)
