import asyncio,base64,json,time,sys,uuid
from pathlib import Path
import httpx,asyncpg
from lab_common import ROOT,CFG,connect,A,B,C,SA,SB,URL,signed_token
OUT=Path('outputs/Stage07A/Evidence');results=[]
REST=f'http://127.0.0.1:{CFG["rest_port"]}'
def mark(name,value,detail=None):
 result={'name':name,'status':'PASS' if value else 'FAIL'}
 if detail:result['detail']=detail
 results.append(result);print(result['status'],name)
 if not value:raise AssertionError(name)
async def rpc(client,name,token=None,body=None,headers=None):
 h={'Content-Profile':'mission_api','Accept-Profile':'mission_api'}
 if token:h['Authorization']='Bearer '+token
 if headers:h.update(headers)
 return await client.post(REST+'/rpc/'+name,json=body or {},headers=h)
async def main():
 async with httpx.AsyncClient(timeout=10,trust_env=False) as client:
  for _ in range(20):
   try:
    ready=await client.get(REST+'/');
    if ready.status_code in (200,401,403):break
   except httpx.HTTPError:pass
   await asyncio.sleep(.1)
  else:raise RuntimeError('Local gateway did not become ready')
  ta,tb=signed_token(A,SA),signed_token(B,SB)
  ra=await rpc(client,'current_profile',ta);rb=await rpc(client,'current_profile',tb)
  mark('real PostgREST independently verifies RSA JWT A/B and returns own profile',ra.status_code==rb.status_code==200 and ra.json()['user_id']==A and rb.json()['user_id']==B)
  mark('untrusted training metadata cannot set training level',ra.json()['training_level']==0)
  no=await rpc(client,'current_profile');mark('anonymous has no mission RPC execution',no.status_code in (401,403))
  unconfirmed=await rpc(client,'current_profile',signed_token(C,SA));mark('gateway signed but unconfirmed account rejected independently of FastAPI',unconfirmed.status_code==401)
  exp=await rpc(client,'current_profile',signed_token(exp=int(time.time())-1));mark('expired token rejected despite gateway clock skew',exp.status_code==401)
  for label,t in [('future nbf',signed_token(nbf=int(time.time())+100)),('wrong audience',signed_token(aud='other')),('wrong issuer',signed_token(iss='https://other.example/auth/v1')),('missing subject',signed_token(sub=None)),('missing session',signed_token(session_id=None)),('privileged role',signed_token(role='astrogateway_schema_owner'))]:
   res=await rpc(client,'current_profile',t);mark(label+' rejected',res.status_code in (400,401,403))
  head,body,sig=ta.split('.');claims=json.loads(base64.urlsafe_b64decode(body+'='*(-len(body)%4)));claims['sub']=B;forged=head+'.'+base64.urlsafe_b64encode(json.dumps(claims).encode()).decode().rstrip('=')+'.'+sig
  for name,t in [('modified signed sub',forged),('invalid signature',ta[:-8]+'xxxxxxxx'),('unsigned algorithm',base64.urlsafe_b64encode(b'{"alg":"none"}').decode().rstrip('=')+'.'+body+'.'),('invalid token','not-a-jwt')]:
   res=await rpc(client,'current_profile',t);mark(name+' cannot forge gateway identity',res.status_code==401)
  res=await rpc(client,'current_profile',ta,headers={'request.jwt.claim.sub':B,'request.jwt.claims':json.dumps({'sub':B,'role':'authenticated'}),'x-user-id':B});mark('HTTP claim/identity headers cannot override signed subject',res.status_code==200 and res.json()['user_id']==A)
  res=await rpc(client,'current_profile',ta,{'user_id':B});mark('RPC rejects user-ID override arguments',res.status_code in (400,404))
  res=await rpc(client,'set_config',ta,{'setting_name':'request.jwt.claim.sub','new_value':B,'is_local':True});mark('no exposed session-setting RPC',res.status_code==404)
  res=await client.get(REST+'/profiles',headers={'Authorization':'Bearer '+ta,'Accept-Profile':'training'});mark('private training schema/table not exposed in data API',res.status_code in (404,406))
  launch=str(uuid.uuid4());create=await rpc(client,'initialize_mission',ta,{'p_launch_id':launch,'p_mission_id':'mission-red-horizon-poc'});mark('A initializes mission through typed invoker RPC',create.status_code==200 and create.json()['user_id']==A and 'auth_session_id' not in create.json());mid=create.json()['id']
  r=await rpc(client,'initialize_mission',ta,{'p_launch_id':launch,'p_mission_id':'mission-red-horizon-poc'});mark('duplicate launch rejected',r.status_code==409)
  r=await rpc(client,'complete_mission',tb,{'p_session_id':mid,'p_launch_id':launch,'p_outcome':'integration_check'});mark('B cannot find or complete A mission through gateway',r.status_code==404)
  r=await rpc(client,'complete_mission',signed_token(A,SB),{'p_session_id':mid,'p_launch_id':launch,'p_outcome':'integration_check'});mark('same user different signed Auth session cannot complete launch',r.status_code==409)
  complete={'p_session_id':mid,'p_launch_id':launch,'p_outcome':'integration_check'};r=await rpc(client,'complete_mission',ta,complete);repeat=await rpc(client,'complete_mission',ta,complete);mark('A completion succeeds and receipt is idempotent',r.status_code==repeat.status_code==200 and r.json()==repeat.json())
  r=await rpc(client,'initialize_mission',ta,{'p_launch_id':str(uuid.uuid4()),'p_mission_id':'mission-red-horizon-poc','p_user_id':B});mark('mission RPC rejects owner override',r.status_code in (400,404))
  # Native DB permissions: attacker successfully authenticates over TLS, may set variables, still no access.
  attacker=await connect('lab_attacker');mark('native restricted attacker password login succeeds for actual privilege tests',await attacker.fetchval('SELECT 1')==1)
  await attacker.fetchval("SELECT set_config('astrogateway.user_id',$1,false)",B)
  await attacker.fetchval("SELECT set_config('request.jwt.claim.sub',$1,false)",B)
  await attacker.fetchval("SELECT set_config('request.jwt.claims',$1,false)",json.dumps({'sub':B,'role':'authenticated','iss':URL+'/auth/v1','aud':'authenticated','session_id':SB,'exp':int(time.time())+600}))
  for name,sql in [('forged context profile','SELECT * FROM training.profiles'),('forged context mission','SELECT * FROM training.mission_sessions'),('forged context RPC','SELECT mission_api.current_profile()'),('Auth users','SELECT * FROM auth.users'),('SET authenticated','SET ROLE authenticated'),('SET owner','SET ROLE astrogateway_schema_owner'),('SET gateway','SET ROLE lab_gateway'),('schema creation','CREATE SCHEMA attacker_bad'),('role creation','CREATE ROLE attacker_bad'),('disable RLS','ALTER TABLE training.profiles DISABLE ROW LEVEL SECURITY')]:
   denied=False
   try:await attacker.execute(sql)
   except asyncpg.InsufficientPrivilegeError:denied=True
   mark('restricted raw credential denied '+name,denied)
  await attacker.execute("SET row_security=off");denied=False
  try:await attacker.fetch('SELECT * FROM training.profiles')
  except asyncpg.InsufficientPrivilegeError:denied=True
  mark('row_security off is not an authorization bypass',denied);await attacker.close()
  admin=await connect();mark('exposed RPCs are invokers with fixed paths',await admin.fetchval("SELECT count(*)=7 AND bool_and(NOT p.prosecdef AND p.proconfig IS NOT NULL) FROM pg_proc p JOIN pg_namespace n ON n.oid=p.pronamespace WHERE n.nspname='mission_api'"))
  mark('schema/table owner is non-login and non-bypass',await admin.fetchval("SELECT NOT rolcanlogin AND NOT rolsuper AND NOT rolbypassrls FROM pg_roles WHERE rolname='astrogateway_schema_owner'"))
  for role in ('authenticated','astrogateway_schema_owner'):
   blocked=False
   try:conn=await connect(role);await conn.close()
   except (asyncpg.InvalidPasswordError,asyncpg.InvalidAuthorizationSpecificationError):blocked=True
   mark(role+' has no raw password login',blocked)
  # Legitimate database-side RLS evaluation simulated from admin only (separate from gateway proofs).
  async def trusted(uid,sid,sql,*args):
   async with admin.transaction():
    await admin.execute('SET LOCAL ROLE authenticated');await admin.fetchval("SELECT set_config('request.jwt.claims',$1,true)",json.dumps({'sub':uid,'role':'authenticated','iss':URL+'/auth/v1','aud':'authenticated','session_id':sid,'exp':int(time.time())+600}));return await admin.fetch(sql,*args)
  mark('native RLS A cannot select B profile',len(await trusted(A,SA,'SELECT * FROM training.profiles WHERE user_id=$1',uuid.UUID(B)))==0)
  mark('native RLS B cannot select A missions',len(await trusted(B,SB,'SELECT * FROM training.mission_sessions WHERE user_id=$1',uuid.UUID(A)))==0)
  for name,sql in [('profile elevation','UPDATE training.profiles SET training_level=10'),('truncate','TRUNCATE training.profiles'),('delete','DELETE FROM training.mission_sessions'),('owner change',"UPDATE training.mission_sessions SET user_id='22222222-2222-4222-8222-222222222222'"),('expiry change',"UPDATE training.mission_sessions SET expires_at=now()+interval '1 year'")]:
   denied=False
   try:await trusted(A,SA,sql)
   except asyncpg.InsufficientPrivilegeError:denied=True
   mark('authenticated least privilege denies '+name,denied)
  async with admin.transaction():
   await admin.execute('SET LOCAL ROLE astrogateway_schema_owner');await admin.execute("CREATE FUNCTION mission_api.future_probe() RETURNS int LANGUAGE sql AS 'SELECT 1'")
  mark('dedicated owner defaults deny future RPC execution',not await admin.fetchval("SELECT has_function_privilege('authenticated','mission_api.future_probe()','EXECUTE') OR has_function_privilege('anon','mission_api.future_probe()','EXECUTE')"));await admin.execute('DROP FUNCTION mission_api.future_probe()')
  await admin.close()
  r=await rpc(client,'revoke_session',ta);mark('revoke derived signed session succeeds',r.status_code==200 and r.json() is True)
  r=await rpc(client,'current_profile',ta);mark('revoked original JWT denied at data boundary without FastAPI',r.status_code==401)
  r=await rpc(client,'current_profile',signed_token(A,SB));mark('independent user session unaffected by revocation',r.status_code==200)
  # Gateway pool reuse interleaves A/B; no header/client DB state persists across requests.
  tokens=[signed_token(A,SB),tb]*10
  rows=await asyncio.gather(*(rpc(client,'current_profile',t) for t in tokens))
  mark('20 interleaved pooled gateway requests retain signed-user isolation',all(r.status_code==200 and r.json()['user_id']==(A if i%2==0 else B) for i,r in enumerate(rows)))
  (OUT/'final-native-regression.json').write_text(json.dumps({'native_postgres':True,'gateway':'PostgREST16.4','live_supabase':False,'remote_write':False,'tests':results,'remaining_risks':['Trusted provider gateway/admin credentials and signing authority excluded','Compromised app can misuse observed valid user tokens','Hosted service isolation remains unverified; local confined AI runtime tested separately']},indent=2))
try:asyncio.run(main())
except Exception as e:
 (OUT/'final-native-failure.json').write_text(json.dumps({'failure_type':type(e).__name__,'tests':results},indent=2));print('FAIL local regression; sanitized failure recorded.');raise SystemExit(1)
