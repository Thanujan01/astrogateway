import asyncpg,asyncio,subprocess,json,os,time
from pathlib import Path
from urllib.parse import quote
from lab_common import ROOT,CFG,connect,A,B,C,SA,SB
OUT=Path('outputs/Stage05B2/Evidence');SQL=Path('outputs/Stage05B2/SQL')
async def main():
 c=await connect();evidence={'server':await c.fetchval('SELECT version()'),'client_tls_verified':True,'backend_tls':dict(await c.fetchrow('SELECT ssl,version,cipher FROM pg_stat_ssl WHERE pid=pg_backend_pid()'))}
 if not await c.fetchval("SELECT to_regclass('auth.users') IS NOT NULL"):
  await c.execute((SQL/'900_local_fixture_only.sql').read_text())
  # Passwords are LOCAL disposable fixtures; no command-line/log/report inclusion.
  for role,key in [('lab_gateway','gateway_password'),('astrogateway_runtime','old_password')]:await c.execute('ALTER ROLE '+role+" PASSWORD '"+CFG[key].replace("'","''")+"'")
  await c.execute("INSERT INTO auth.users(id,raw_user_meta_data,email_confirmed_at,is_anonymous) VALUES($1,'{\"display_name\":\"A trainee\",\"training_level\":10}',now(),false),($2,'{\"display_name\":\"B trainee\"}',now(),false),($3,'{}',NULL,false)",__import__('uuid').UUID(A),__import__('uuid').UUID(B),__import__('uuid').UUID(C))
  await c.execute((SQL/'001_verified_jwt_training.sql').read_text())
 # Worst-case legacy connection: already SET to an authorized shared role.
 await c.execute('ALTER ROLE astrogateway_runtime LOGIN CONNECTION LIMIT -1')
 await c.execute('GRANT authenticated TO astrogateway_runtime')
 stale=await connect('astrogateway_runtime');await stale.execute('SET ROLE authenticated')
 await stale.fetchval("SELECT set_config('request.jwt.claims',$1,false)",json.dumps({'iss':'https://jvkwofmxtxjhdwtwdboz.supabase.co/auth/v1','role':'authenticated','aud':'authenticated','sub':B,'session_id':SB,'exp':int(time.time())+600}))
 pre=await stale.fetchval('SELECT mission_api.current_profile()');evidence['legacy_credential_before_quarantine_can_impersonate']=json.loads(pre)['user_id']==B
 await c.execute((SQL/'002_retire_direct_runtime.sql').read_text())
 await asyncio.sleep(.2);evidence['stale_legacy_session_terminated']=stale.is_closed();await stale.close()
 await c.execute((SQL/'002_retire_direct_runtime.sql').read_text());evidence['quarantine_repeat_pass']=True
 try:
  bad=await connect('astrogateway_runtime');await bad.close();evidence['retired_password_login_denied']=False
 except asyncpg.InvalidAuthorizationSpecificationError:evidence['retired_password_login_denied']=True
 # LOGIN test attacker has same absence of data/membership rights; allows arbitrary SET attempts.
 await c.execute("CREATE ROLE lab_attacker LOGIN NOINHERIT NOSUPERUSER NOBYPASSRLS NOCREATEDB NOCREATEROLE NOREPLICATION PASSWORD '"+CFG['old_password'].replace("'","''")+"'")
 await c.execute((SQL/'003_read_only_verification.sql').read_text())
 await c.close();OUT.mkdir(parents=True,exist_ok=True);(OUT/'native-setup.json').write_text(json.dumps(evidence,indent=2))
 assert evidence['client_tls_verified'] and evidence['backend_tls']['ssl'] and evidence['legacy_credential_before_quarantine_can_impersonate'] and evidence['stale_legacy_session_terminated'] and evidence['retired_password_login_denied']
 # Gateway gets PUBLIC verification key only, isolated fixture DB credential; FastAPI gets neither.
 uri=f"postgresql://lab_gateway:{quote(CFG['gateway_password'],safe='')}@localhost:{CFG['port']}/postgres?sslmode=verify-full&sslrootcert={quote(CFG['ca_file'],safe='/')}"
 conf=ROOT/'private/postgrest.conf';conf.write_text(f'db-uri = "{uri}"\ndb-schemas = "mission_api"\ndb-anon-role = "anon"\ndb-pool = 3\njwt-secret = "@{CFG["public_jwks"]}"\njwt-aud = "authenticated"\nserver-host = "127.0.0.1"\nserver-port = {CFG["rest_port"]}\nlog-level = "crit"\n');conf.chmod(0o600)
 log=(ROOT/'private/postgrest.log').open('w');p=subprocess.Popen([str(ROOT/'native/postgrest/postgrest'),str(conf)],stdout=log,stderr=subprocess.STDOUT)
 (ROOT/'private/postgrest.pid').write_text(str(p.pid));print('Native SQL installed; quarantine blocks saved runtime credentials and closes stale authorized sessions. Local PostgREST started.')
asyncio.run(main())
