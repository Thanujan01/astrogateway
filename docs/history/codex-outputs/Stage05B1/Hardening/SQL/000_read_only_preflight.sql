-- Run only in the explicitly approved astrogateway-test project's SQL Editor.
-- Read-only metadata inspection; no migration/provisioning/account creation.
BEGIN TRANSACTION READ ONLY;
SELECT current_database(), current_user, current_setting('server_version');
SELECT to_regclass('auth.users') IS NOT NULL AS auth_users_present,
       to_regprocedure('gen_random_uuid()') IS NOT NULL AS uuid_generator_present;
SELECT column_name,data_type FROM information_schema.columns
 WHERE table_schema='auth' AND table_name='users'
 AND column_name IN ('id','created_at','raw_user_meta_data');
SELECT rolname,rolsuper,rolbypassrls,rolcreaterole
 FROM pg_roles WHERE rolname IN ('anon','authenticated','astrogateway_api','astrogateway_runtime');
SELECT n.nspname,c.relname,c.relrowsecurity,pg_get_userbyid(c.relowner) AS owner
 FROM pg_class c JOIN pg_namespace n ON n.oid=c.relnamespace
 WHERE n.nspname='training' AND c.relkind='r';
SELECT schemaname,tablename,policyname,roles,cmd FROM pg_policies WHERE schemaname='training';
SELECT tgname FROM pg_trigger WHERE tgrelid=to_regclass('auth.users') AND NOT tgisinternal;
COMMIT;
