import asyncio,ssl,shlex,json,certifi
from pathlib import Path
from urllib.parse import urlparse,urlunparse,urlencode
import asyncpg
root=Path('/Users/kapilash/Projects/AstroGatewayAndroid');out=Path('outputs/Stage05B1/Review_Evidence/catalogue.json')
queries={
 'database_public_privileges':"SELECT a.privilege_type FROM pg_database d CROSS JOIN LATERAL aclexplode(COALESCE(d.datacl,acldefault('d',d.datdba))) a WHERE d.datname=current_database() AND a.grantee=0",
 'public_definer_return_types':"SELECT n.nspname,p.proname,p.prorettype::regtype::text AS return_type FROM pg_proc p JOIN pg_namespace n ON n.oid=p.pronamespace CROSS JOIN LATERAL aclexplode(COALESCE(p.proacl,acldefault('f',p.proowner))) a WHERE a.grantee=0 AND a.privilege_type='EXECUTE' AND p.prosecdef AND n.nspname='public'",
 'auth_backfill_preflight':"SELECT count(*) AS auth_user_count,count(*) FILTER (WHERE created_at IS NULL) AS null_creation_dates FROM auth.users",
 'server_version':'SHOW server_version',
 'dependencies':"SELECT to_regclass('auth.users') IS NOT NULL AS auth_users, to_regprocedure('gen_random_uuid()') IS NOT NULL AS uuid_generator",
 'auth_columns':"SELECT column_name,data_type FROM information_schema.columns WHERE table_schema='auth' AND table_name='users' AND column_name IN ('id','created_at','raw_user_meta_data') ORDER BY column_name",
 'roles':"SELECT rolname,rolcanlogin,rolsuper,rolbypassrls,rolcreaterole,rolcreatedb FROM pg_roles WHERE rolname IN ('anon','authenticated','supabase_auth_admin','astrogateway_api','astrogateway_runtime') ORDER BY rolname",
 'existing_training_objects':"SELECT n.nspname,c.relname FROM pg_class c JOIN pg_namespace n ON n.oid=c.relnamespace WHERE n.nspname='training'",
 'auth_triggers':"SELECT tgname FROM pg_trigger WHERE tgrelid=to_regclass('auth.users') AND NOT tgisinternal",
 'public_schema_privileges':"SELECT n.nspname,a.privilege_type FROM pg_namespace n CROSS JOIN LATERAL aclexplode(COALESCE(n.nspacl,acldefault('n',n.nspowner))) a WHERE a.grantee=0 AND n.nspname NOT LIKE 'pg_%' AND n.nspname<>'information_schema' ORDER BY 1,2",
 'public_table_privileges':"SELECT n.nspname,c.relname,a.privilege_type FROM pg_class c JOIN pg_namespace n ON n.oid=c.relnamespace CROSS JOIN LATERAL aclexplode(COALESCE(c.relacl,acldefault('r',c.relowner))) a WHERE a.grantee=0 AND n.nspname IN ('auth','storage','public','vault') ORDER BY 1,2,3",
 'public_definer_functions':"SELECT n.nspname,p.proname,pg_get_userbyid(p.proowner) AS owner FROM pg_proc p JOIN pg_namespace n ON n.oid=p.pronamespace CROSS JOIN LATERAL aclexplode(COALESCE(p.proacl,acldefault('f',p.proowner))) a WHERE a.grantee=0 AND a.privilege_type='EXECUTE' AND p.prosecdef AND n.nspname NOT LIKE 'pg_%' AND n.nspname<>'information_schema' ORDER BY 1,2",
}
async def run():
 r={'read_only':True,'remote_modified':False};conn=None
 try:
  v={}
  for l in (root/'database/admin.local.env').read_text().splitlines():
   if '=' in l and not l.lstrip().startswith('#'):
    k,s=l.split('=',1);parts=shlex.split(s,comments=True);v[k]=parts[0] if parts else ''
  u=urlparse(v['DB_ADMIN_DSN']);assert u.hostname=='aws-0-ap-southeast-2.pooler.supabase.com' and u.username=='postgres.jvkwofmxtxjhdwtwdboz'
  ca=root/'backend/certs/supabase-ca.crt';ctx=ssl.create_default_context(cafile=certifi.where());ctx.load_verify_locations(cafile=str(ca));ctx.verify_flags &= ~ssl.VERIFY_X509_STRICT
  conn=await asyncpg.connect(urlunparse(u._replace(query=urlencode({'sslmode':'verify-full','sslrootcert':str(ca)}))),ssl=ctx,timeout=15,command_timeout=10,statement_cache_size=0)
  async with conn.transaction(readonly=True):
   for name,q in queries.items():r[name]=[dict(row) for row in await conn.fetch(q)]
  r['completed']=True
 except Exception as e:r['completed']=False;r['error_type']=type(e).__name__;r['sqlstate']=getattr(e,'sqlstate',None)
 finally:
  if conn:await conn.close()
 out.write_text(json.dumps(r,indent=2));print(json.dumps(r,indent=2))
asyncio.run(run())
