from pathlib import Path
out=Path('outputs/Stage05B1/Hardening/SQL')
base=Path('outputs/Stage05B/database/001_identity_and_mission_sessions.sql').read_text()
(out/'001_identity_and_mission_sessions.sql').write_text(base.replace('NOINHERIT NOBYPASSRLS;','NOINHERIT NOREPLICATION NOBYPASSRLS;'))
(out/'002_public_acl_isolation.sql').write_text('''-- CANDIDATE ONLY. Local rehearsal approved; remote execution NOT approved.
-- Converts existing PUBLIC schema access/TEMP to explicit grants for existing roles.
-- Must inventory every preserved role/default ACL and approve provider compatibility first.
-- New provider roles need explicit provisioning; no universal future-role grant.
BEGIN;
DO $$
DECLARE s record; r record; privileges text; dbname text:=current_database();
BEGIN
 IF current_user IN ('astrogateway_api','astrogateway_runtime') THEN RAISE EXCEPTION 'Administrator required'; END IF;
 FOR s IN SELECT n.oid,n.nspname,n.nspowner FROM pg_namespace n
  WHERE n.nspname NOT IN ('training','pg_catalog','information_schema') AND n.nspname NOT LIKE 'pg_%'
 LOOP
  SELECT string_agg(a.privilege_type,', ') INTO privileges
   FROM aclexplode(COALESCE((SELECT nspacl FROM pg_namespace WHERE oid=s.oid),acldefault('n',s.nspowner))) a
   WHERE a.grantee=0;
  IF privileges IS NOT NULL THEN
   FOR r IN SELECT rolname FROM pg_roles WHERE rolname NOT IN ('astrogateway_api','astrogateway_runtime') AND rolname NOT LIKE 'pg_%'
   LOOP EXECUTE format('GRANT %s ON SCHEMA %I TO %I',privileges,s.nspname,r.rolname); END LOOP;
   EXECUTE format('REVOKE ALL ON SCHEMA %I FROM PUBLIC',s.nspname);
  END IF;
 END LOOP;
 IF EXISTS(SELECT 1 FROM pg_database d CROSS JOIN LATERAL aclexplode(COALESCE(d.datacl,acldefault('d',d.datdba))) a
  WHERE d.datname=dbname AND a.grantee=0 AND a.privilege_type='TEMPORARY') THEN
  FOR r IN SELECT rolname FROM pg_roles WHERE rolname NOT IN ('astrogateway_api','astrogateway_runtime') AND rolname NOT LIKE 'pg_%'
  LOOP EXECUTE format('GRANT TEMPORARY ON DATABASE %I TO %I',dbname,r.rolname); END LOOP;
  EXECUTE format('REVOKE TEMPORARY ON DATABASE %I FROM PUBLIC',dbname);
 END IF;
END $$;
-- Function EXECUTE defaults are global; per-schema REVOKE cannot remove the built-in PUBLIC default.
-- Applies only to current migration owner, affects its future functions in ALL schemas.
-- Preserve existing named-role execution semantics while excluding application roles.
ALTER DEFAULT PRIVILEGES REVOKE EXECUTE ON FUNCTIONS FROM PUBLIC;
DO $$ DECLARE r record; BEGIN
 FOR r IN SELECT rolname FROM pg_roles WHERE rolname NOT IN ('astrogateway_api','astrogateway_runtime') AND rolname NOT LIKE 'pg_%'
 LOOP EXECUTE format('ALTER DEFAULT PRIVILEGES GRANT EXECUTE ON FUNCTIONS TO %I',r.rolname); END LOOP;
END $$;
REVOKE ALL ON ALL TABLES IN SCHEMA training FROM PUBLIC;
REVOKE ALL ON ALL FUNCTIONS IN SCHEMA training FROM PUBLIC;
COMMIT;
''')
role=Path('outputs/Stage05B1/Database_Plan_SQL/002_runtime_role.draft.sql').read_text().replace('-- DRAFT ONLY. Not executed.', '-- CANDIDATE ONLY. Rehearsed locally; never executed remotely.').replace('Run only after 001 and separately approved PUBLIC ACL isolation', 'Run only after 001 and 002 plus separately approved PUBLIC ACL isolation')
(out/'003_runtime_role.sql').write_text(role)
(out/'004_post_migration_metadata.sql').write_text(Path('outputs/Stage05B1/Database_Plan_SQL/003_post_migration_metadata.sql').read_text())
(out/'900_local_fixture_only.sql').write_text('''-- NEVER RUN ON SUPABASE. Auth/service roles are minimal disposable fixtures only.
CREATE ROLE anon NOLOGIN;
CREATE ROLE authenticated NOLOGIN;
CREATE ROLE supabase_auth_admin LOGIN CREATEROLE;
CREATE ROLE service_role NOLOGIN BYPASSRLS;
CREATE SCHEMA auth;
CREATE TABLE auth.users(id uuid PRIMARY KEY,created_at timestamptz NOT NULL DEFAULT now(),raw_user_meta_data jsonb);
GRANT USAGE ON SCHEMA auth TO supabase_auth_admin;
GRANT SELECT,INSERT ON auth.users TO supabase_auth_admin;
-- Representative unrelated PUBLIC usage plus a security-definer callable surface.
CREATE SCHEMA storage; GRANT USAGE ON SCHEMA storage TO PUBLIC;
CREATE TABLE public.unrelated(secret text); INSERT INTO public.unrelated VALUES('disposable fixture');
CREATE FUNCTION public.fixture_read() RETURNS text LANGUAGE sql SECURITY DEFINER SET search_path='' AS $$SELECT secret FROM public.unrelated LIMIT 1$$;
''')
(out/'910_local_down_only.sql').write_text('''-- LOCAL DISPOSABLE FIXTURE ONLY. Not a remote rollback/ACL restore script.
BEGIN;
DROP TRIGGER astrogateway_auth_user_profile ON auth.users;
DROP FUNCTION training.create_auth_profile();
DROP TABLE training.mission_sessions;
DROP POLICY profile_read_own ON training.profiles;
DROP FUNCTION training.session_allowed();
DROP TABLE training.auth_session_revocations;
DROP TABLE training.profiles;
DROP FUNCTION training.request_user_id();
DROP FUNCTION training.request_auth_session_id();
REVOKE astrogateway_api FROM astrogateway_runtime;
REVOKE CONNECT ON DATABASE postgres FROM astrogateway_runtime;
ALTER ROLE astrogateway_runtime IN DATABASE postgres RESET search_path;
DROP ROLE astrogateway_runtime;
REVOKE USAGE ON SCHEMA training FROM astrogateway_api;
DROP SCHEMA training;
DROP ROLE astrogateway_api;
COMMIT;
-- Restore ACL/default-ACL snapshot separately; SQL here only removes owned application objects.
''')
