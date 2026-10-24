-- Read-only future preflight. No remote execution authorized by Stage05B.2.
BEGIN TRANSACTION READ ONLY;
SELECT current_database(),current_user,current_setting('server_version');
SELECT column_name,data_type,is_nullable FROM information_schema.columns WHERE table_schema='auth' AND table_name='users' AND column_name IN ('id','created_at','raw_user_meta_data','email_confirmed_at','is_anonymous');
SELECT to_regprocedure('auth.uid()'),to_regprocedure('auth.jwt()');
SELECT rolname,rolcanlogin,rolinherit,rolsuper,rolbypassrls,rolcreaterole,rolcreatedb,rolreplication FROM pg_roles WHERE rolname IN ('authenticated','anon','authenticator','astrogateway_runtime','astrogateway_api','astrogateway_schema_owner');
SELECT parent.rolname,child.rolname,m.admin_option,m.inherit_option,m.set_option FROM pg_auth_members m JOIN pg_roles parent ON parent.oid=m.roleid JOIN pg_roles child ON child.oid=m.member WHERE child.rolname IN ('authenticated','astrogateway_runtime','astrogateway_api');
SELECT nspname,nspacl,pg_get_userbyid(nspowner) FROM pg_namespace WHERE nspname IN ('training','mission_api');
SELECT tgname FROM pg_trigger WHERE tgrelid=to_regclass('auth.users') AND NOT tgisinternal;
SELECT count(*) AS users,count(*) FILTER(WHERE created_at IS NULL) AS null_created_at FROM auth.users;
-- Inventory ALL exposed-schema functions/defaults/ACLs separately before enabling Data API.
COMMIT;
