-- REVIEW ONLY: Stage05B.3 forbids ALL remote SQL execution, including read-only.
-- Execute only after separate operator authorization; no secret-bearing catalogs.
BEGIN TRANSACTION READ ONLY;
SET LOCAL statement_timeout='10s';
SET LOCAL lock_timeout='2s';
SELECT current_database(),current_user,current_setting('server_version_num');
SELECT column_name,data_type,is_nullable FROM information_schema.columns
 WHERE table_schema='auth' AND table_name='users'
 AND column_name IN ('id','created_at','raw_user_meta_data','email_confirmed_at','is_anonymous');
SELECT to_regprocedure('auth.uid()'),to_regprocedure('auth.jwt()'),to_regprocedure('gen_random_uuid()');
SELECT rolname,rolcanlogin,rolsuper,rolbypassrls,rolcreaterole,rolcreatedb,rolreplication
 FROM pg_roles WHERE rolname IN ('authenticated','anon','authenticator','postgres','supabase_auth_admin','astrogateway_runtime','astrogateway_api','astrogateway_schema_owner');
SELECT parent.rolname AS parent_role,child.rolname AS member_role,m.admin_option
 FROM pg_auth_members m JOIN pg_roles parent ON parent.oid=m.roleid JOIN pg_roles child ON child.oid=m.member
 WHERE child.rolname IN ('authenticated','astrogateway_runtime','astrogateway_api','astrogateway_schema_owner');
SELECT nspname,pg_get_userbyid(nspowner) AS owner,nspacl FROM pg_namespace
 WHERE nspname IN ('training','mission_api','public','graphql_public');
SELECT n.nspname,c.relname,c.relkind,c.relrowsecurity,c.relforcerowsecurity,pg_get_userbyid(c.relowner) AS owner,c.relacl
 FROM pg_class c JOIN pg_namespace n ON n.oid=c.relnamespace WHERE n.nspname IN ('training','mission_api');
SELECT schemaname,tablename,policyname,roles,cmd,qual,with_check FROM pg_policies WHERE schemaname IN ('training','mission_api');
-- Add every dashboard-exposed schema to this inventory before approval. Dashboard list is authoritative.
SELECT n.nspname,p.proname,pg_get_function_identity_arguments(p.oid) AS args,p.prosecdef,p.proconfig,p.proacl,pg_get_userbyid(p.proowner) AS owner
 FROM pg_proc p JOIN pg_namespace n ON n.oid=p.pronamespace
 WHERE n.nspname IN ('training','mission_api','public','graphql_public','auth') ORDER BY n.nspname,p.proname;
SELECT pg_get_userbyid(d.defaclrole) AS owner,COALESCE(n.nspname,'GLOBAL') AS schema,d.defaclobjtype,d.defaclacl
 FROM pg_default_acl d LEFT JOIN pg_namespace n ON n.oid=d.defaclnamespace;
SELECT tgname,pg_get_triggerdef(oid) FROM pg_trigger WHERE tgrelid=to_regclass('auth.users') AND NOT tgisinternal;
COMMIT;
-- No pg_authid, tokens, password hashes, user rows, role DDL, NOTIFY or config changes.
