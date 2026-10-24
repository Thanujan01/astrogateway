import asyncio,json,sys,time,subprocess,os
from pathlib import Path
sys.path.insert(0,str(Path('work/stage05b2').resolve()))
from lab_common import connect,CFG,ROOT,A,B,C,SA,SB,signed_token
import httpx,asyncpg,ssl
import lab_common
# Disposable localhost CA renewal; strict chain and hostname validation unchanged.
def local_context():
 ctx=ssl.create_default_context(cafile=str(Path('work/unifiedauth/tls/ca.crt').resolve()));ctx.verify_flags|=ssl.VERIFY_X509_STRICT;ctx.check_hostname=True;ctx.verify_mode=ssl.CERT_REQUIRED;return ctx
lab_common.context=local_context
OUT=Path('outputs/UnifiedAuth/Evidence'); checks=[]
def mark(name,ok):
 checks.append({'name':name,'result':'PASS' if ok else 'FAIL'});print(checks[-1]['result'],name)
 if not ok:raise AssertionError(name)
def claims(uid=A,sid=SA,expired=False):return json.dumps({'iss':'https://jvkwofmxtxjhdwtwdboz.supabase.co/auth/v1','aud':'authenticated','role':'authenticated','sub':uid,'session_id':sid,'exp':int(time.time())+(-10 if expired else 600),'is_anonymous':False})
async def main():
 c=await connect();mark('lab is localhost database',await c.fetchval("SELECT inet_server_addr() <<= inet '127.0.0.0/8' OR inet_server_addr() <<= inet '::1/128'"))
 original={str(r['user_id']):r['display_name'] for r in await c.fetch('SELECT user_id,display_name FROM training.profiles')}
 migration=Path('outputs/UnifiedAuth/SQL/004_profile_display_name.sql').read_text();down=Path('outputs/UnifiedAuth/SQL/904_profile_display_name_rollback.sql').read_text()
 await c.execute(migration);mark('local migration applies',True)
 try:await c.execute(migration);mark('repeat refuses collision',False)
 except asyncpg.DuplicateObjectError:await c.execute('ROLLBACK');mark('repeat refuses collision transactionally',True)
 async def run(sql,who=A,expired=False):
  async with c.transaction():
   await c.execute('SET LOCAL ROLE authenticated');await c.execute("SELECT set_config('request.jwt.claims',$1,true)",claims(who,SB if who==B else SA,expired));return await c.fetchval(sql)
 mark('A own display-name update retains UUID',json.loads(await run("SELECT mission_api.update_display_name('A Updated')"))['user_id']==A)
 mark('B own update isolated',json.loads(await run("SELECT mission_api.update_display_name('B Updated')",B))['user_id']==B)
 mark('A reads only A',await run("SELECT count(*)=1 AND bool_and(user_id=auth.uid()) FROM training.profiles"))
 mark('B reads only B',await run("SELECT count(*)=1 AND bool_and(user_id=auth.uid()) FROM training.profiles",B))
 mark('A cannot update B even with direct WHERE override',await run("WITH x AS (UPDATE training.profiles SET display_name='Hijack' WHERE user_id='"+B+"' RETURNING *) SELECT count(*)=0 FROM x"))
 for sql,label in [("UPDATE training.profiles SET training_level=10 WHERE user_id=auth.uid() RETURNING training_level",'training level'),("UPDATE training.profiles SET user_id='"+B+"' WHERE user_id=auth.uid() RETURNING user_id",'user UUID')]:
  denied=False
  try:await run(sql)
  except asyncpg.InsufficientPrivilegeError:denied=True
  mark('cannot alter '+label,denied)
 for literal,label in [("'x'",'short'),("repeat('a',65)",'oversize'),("E'ab\\ncd'",'control'),('NULL','null')]:
  denied=False
  try:await run('SELECT mission_api.update_display_name('+literal+')')
  except asyncpg.PostgresError as e:denied=e.sqlstate=='PT422'
  mark('invalid '+label+' name rejected',denied)
 for who,exp,label in [(A,True,'expired'),(C,False,'unconfirmed')]:
  denied=False
  try:await run("SELECT mission_api.update_display_name('Bad Update')",who,exp)
  except asyncpg.PostgresError as e:denied=e.sqlstate=='PT401'
  mark(label+' identity denied',denied)
 mark('authenticated remains NOLOGIN/NOBYPASSRLS',await c.fetchval("SELECT NOT rolcanlogin AND NOT rolbypassrls AND NOT rolsuper FROM pg_roles WHERE rolname='authenticated'"))
 mark('exposed updater is invoker with fixed empty search path',await c.fetchval("SELECT NOT prosecdef AND proconfig @> ARRAY['search_path=\"\"'] FROM pg_proc WHERE oid='mission_api.update_display_name(text)'::regprocedure"))
 mark('anon/PUBLIC cannot execute updater',not await c.fetchval("SELECT has_function_privilege('anon','mission_api.update_display_name(text)','EXECUTE')"))
 attacker=await connect('lab_attacker');await attacker.execute("SELECT set_config('request.jwt.claims',$1,false)",claims(B))
 denied=False
 try:await attacker.fetchval("SELECT mission_api.update_display_name('Impersonation')")
 except asyncpg.InsufficientPrivilegeError:denied=True
 mark('untrusted DB role session settings grant no profile authority',denied);await attacker.close()
 mark('retired runtime credentials still NOLOGIN',await c.fetchval("SELECT NOT rolcanlogin FROM pg_roles WHERE rolname='astrogateway_runtime'"))
 # Start existing local PostgREST with private fixture configuration, never print its credential-bearing content.
 log=(ROOT/'private/postgrest-unified.log').open('w');gateway=subprocess.Popen([str(ROOT/'native/postgrest/postgrest'),str(Path('work/unifiedauth/postgrest.local.conf').resolve())],stdout=log,stderr=subprocess.STDOUT)
 (OUT/'local-gateway-pid.txt').write_text(str(gateway.pid))
 rest=f'http://127.0.0.1:{CFG["rest_port"]}'
 async with httpx.AsyncClient(timeout=10,trust_env=False) as h:
  for _ in range(30):
   try:await h.get(rest+'/');break
   except httpx.HTTPError:await asyncio.sleep(.1)
  def header(token):return {'Authorization':'Bearer '+token,'Content-Profile':'mission_api','Accept-Profile':'mission_api'}
  ta,tb=signed_token(A),signed_token(B,SB)
  for tok,uid,name in [(ta,A,'Gateway A'),(tb,B,'Gateway B')]:
   r=await h.post(rest+'/rpc/update_display_name',headers=header(tok),json={'p_display_name':name})
   mark('real independent JWT gateway own update '+name,r.status_code==200 and r.json()['user_id']==uid and r.json()['display_name']==name)
  r=await h.post(rest+'/rpc/update_display_name',headers=header(ta),json={'p_display_name':'Override','p_user_id':B});mark('gateway rejects user-ID injection',r.status_code>=400)
  forged=ta[:-8]+'forgedxx'
  for tok,label in [(forged,'forged'),(signed_token(A,exp=int(time.time())-30),'expired'),('invalid','malformed')]:
   r=await h.post(rest+'/rpc/update_display_name',headers=header(tok),json={'p_display_name':'Attack'});mark('independent gateway rejects '+label+' JWT',r.status_code==401)
  r=await h.post(rest+'/rpc/update_display_name',json={'p_display_name':'Attack'},headers={'Content-Profile':'mission_api'});mark('unauthenticated gateway edit denied',r.status_code>=400)
 await c.execute(down);mark('rollback removes edit privileges and function',not await c.fetchval("SELECT has_column_privilege('authenticated','training.profiles','display_name','UPDATE')") and await c.fetchval("SELECT to_regprocedure('mission_api.update_display_name(text)') IS NULL"))
 for uid,name in original.items():await c.execute('UPDATE training.profiles SET display_name=$1 WHERE user_id=$2',name,__import__('uuid').UUID(uid))
 await c.execute(migration);mark('local migration reapplies after rollback',True)
 await c.execute("NOTIFY pgrst,'reload schema'");await c.close()
 (OUT/'local-profile-rehearsal.json').write_text(json.dumps({'local_only':True,'synthetic_users_only':True,'remote_writes':False,'checks':checks},indent=2))
try:asyncio.run(main())
except Exception as e:
 (OUT/'local-profile-rehearsal.json').write_text(json.dumps({'local_only':True,'checks':checks,'failure_type':type(e).__name__},indent=2));print('FAIL sanitized failure:',type(e).__name__);raise SystemExit(1)
