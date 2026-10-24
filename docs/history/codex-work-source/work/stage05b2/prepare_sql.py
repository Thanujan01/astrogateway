from pathlib import Path
out=Path('outputs/Stage05B2/SQL');original=Path('outputs/Stage05B/database/001_identity_and_mission_sessions.sql').read_text()
s=original[original.index('CREATE SCHEMA training;'):original.index('-- These transaction-local settings')]
s=s.replace('astrogateway_api','authenticated')
s=s.replace('GRANT INSERT (user_id,auth_session_id,launch_id,mission_id) ON training.mission_sessions TO authenticated;','GRANT INSERT (user_id,auth_session_id,launch_id,mission_id) ON training.mission_sessions TO authenticated;')
# Re-grant only columns/select; no API role and no raw login.
head='''-- Stage05B.2 FRESH INSTALL candidate. Local rehearsal only; remote execution NOT approved.
-- Requires Supabase auth.uid/auth.jwt, verified gateway, NOLOGIN authenticated role.
-- Collisions abort. DO NOT apply on an installed old training schema; use reviewed transition instead.
BEGIN;
DO $$ BEGIN
 IF NOT EXISTS(SELECT 1 FROM pg_roles WHERE rolname='authenticated' AND NOT rolcanlogin AND NOT rolsuper AND NOT rolbypassrls AND NOT rolcreaterole AND NOT rolcreatedb AND NOT rolreplication) THEN
  RAISE EXCEPTION 'Expected safe NOLOGIN authenticated role missing';
 END IF;
 IF to_regprocedure('auth.uid()') IS NULL OR to_regprocedure('auth.jwt()') IS NULL THEN RAISE EXCEPTION 'Supabase JWT helpers missing'; END IF;
END $$;
'''
helpers='''
-- Only the independently JWT-verifying gateway has SET access to authenticated.
-- No host raw DB credential or client-controlled arbitrary SQL/RPC setter is allowed.
CREATE FUNCTION training.claim_session_id() RETURNS uuid LANGUAGE sql STABLE SECURITY INVOKER SET search_path='' AS $$
 SELECT NULLIF(auth.jwt()->>'session_id','')::uuid;
$$;
CREATE FUNCTION training.claims_valid() RETURNS boolean LANGUAGE plpgsql STABLE SECURITY INVOKER SET search_path='' AS $$
DECLARE c jsonb:=auth.jwt();
BEGIN
 RETURN auth.uid() IS NOT NULL AND training.claim_session_id() IS NOT NULL
 AND c->>'iss'='https://jvkwofmxtxjhdwtwdboz.supabase.co/auth/v1'
 AND c->>'role'='authenticated' AND (c->>'aud'='authenticated' OR c->'aud' @> '["authenticated"]'::jsonb)
 AND jsonb_typeof(c->'exp')='number' AND (c->>'exp')::bigint>extract(epoch FROM now())
 AND (NOT(c ? 'nbf') OR (jsonb_typeof(c->'nbf')='number' AND (c->>'nbf')::bigint<=extract(epoch FROM now())))
 AND COALESCE((c->>'is_anonymous')::boolean,false)=false;
EXCEPTION WHEN invalid_text_representation OR numeric_value_out_of_range THEN RETURN false;
END $$;
-- Private, bounded boolean only. No UUID arguments, no Auth row disclosure, no dynamic SQL.
-- Owner is migration administrator; runtime has no grant. Required to enforce confirmed email
-- even if the FastAPI process is compromised or caller accesses gateway directly.
CREATE FUNCTION training.confirmed_user() RETURNS boolean LANGUAGE sql STABLE SECURITY DEFINER SET search_path='' AS $$
 SELECT EXISTS(SELECT 1 FROM auth.users u WHERE u.id=auth.uid() AND u.email_confirmed_at IS NOT NULL AND NOT COALESCE(u.is_anonymous,false));
$$;
CREATE POLICY revocation_read_own ON training.auth_session_revocations FOR SELECT TO authenticated
 USING(user_id=(SELECT auth.uid()) AND (SELECT training.claims_valid()) AND (SELECT training.confirmed_user()));
CREATE POLICY revocation_insert_current ON training.auth_session_revocations FOR INSERT TO authenticated
 WITH CHECK(user_id=(SELECT auth.uid()) AND auth_session_id=(SELECT training.claim_session_id())
 AND (SELECT training.claims_valid()) AND (SELECT training.confirmed_user()) AND expires_at=to_timestamp((auth.jwt()->>'exp')::bigint));
CREATE FUNCTION training.session_allowed() RETURNS boolean LANGUAGE sql STABLE SECURITY INVOKER SET search_path='' AS $$
 SELECT training.claims_valid() AND training.confirmed_user() AND NOT EXISTS(SELECT 1 FROM training.auth_session_revocations r
 WHERE r.user_id=auth.uid() AND r.auth_session_id=training.claim_session_id() AND r.expires_at>now());
$$;
CREATE FUNCTION training.assert_identity() RETURNS void LANGUAGE plpgsql STABLE SECURITY INVOKER SET search_path='' AS $$
BEGIN
 IF current_user<>'authenticated' OR NOT COALESCE(training.claims_valid(),false) OR NOT training.confirmed_user() THEN
  RAISE SQLSTATE 'PT401' USING MESSAGE='Session invalid or expired';
 END IF;
END $$;
CREATE POLICY profile_read_own ON training.profiles FOR SELECT TO authenticated
 USING(user_id=(SELECT auth.uid()) AND (SELECT training.session_allowed()));
CREATE POLICY mission_read_own ON training.mission_sessions FOR SELECT TO authenticated
 USING(user_id=(SELECT auth.uid()) AND (SELECT training.session_allowed()));
CREATE POLICY mission_insert_current ON training.mission_sessions FOR INSERT TO authenticated
 WITH CHECK(user_id=(SELECT auth.uid()) AND auth_session_id=(SELECT training.claim_session_id())
 AND (SELECT training.session_allowed()) AND status='initialized' AND completed_at IS NULL AND expires_at>now());
CREATE POLICY mission_complete_current ON training.mission_sessions FOR UPDATE TO authenticated
 USING(user_id=(SELECT auth.uid()) AND auth_session_id=(SELECT training.claim_session_id()) AND (SELECT training.session_allowed())
 AND ((status='initialized' AND expires_at>now()) OR status='integration_check_complete'))
 WITH CHECK(user_id=(SELECT auth.uid()) AND auth_session_id=(SELECT training.claim_session_id()) AND (SELECT training.session_allowed())
 AND status='integration_check_complete' AND completed_at IS NOT NULL AND completed_at<=now()+interval '5 seconds');
REVOKE ALL ON ALL FUNCTIONS IN SCHEMA training FROM PUBLIC,anon,authenticated;
GRANT EXECUTE ON FUNCTION training.claim_session_id(),training.claims_valid(),training.confirmed_user(),training.session_allowed(),training.assert_identity() TO authenticated;
'''
trigger=original[original.index('CREATE FUNCTION training.create_auth_profile()'):original.index('COMMIT;')]
trigger=trigger.replace(',astrogateway_api','')
rpc='''
CREATE SCHEMA mission_api;
REVOKE ALL ON SCHEMA mission_api FROM PUBLIC,anon,authenticated;
GRANT USAGE ON SCHEMA mission_api TO authenticated;
CREATE FUNCTION mission_api.session_revoked() RETURNS boolean LANGUAGE plpgsql STABLE SECURITY INVOKER SET search_path='' AS $$
BEGIN
 PERFORM training.assert_identity();
 RETURN EXISTS(SELECT 1 FROM training.auth_session_revocations WHERE user_id=auth.uid() AND auth_session_id=training.claim_session_id() AND expires_at>now());
END $$;
CREATE FUNCTION mission_api.current_profile() RETURNS jsonb LANGUAGE plpgsql STABLE SECURITY INVOKER SET search_path='' AS $$
DECLARE result jsonb;
BEGIN
 PERFORM training.assert_identity();
 IF NOT training.session_allowed() THEN RAISE SQLSTATE 'PT401' USING MESSAGE='Session revoked'; END IF;
 SELECT to_jsonb(p) INTO result FROM training.profiles p WHERE p.user_id=auth.uid();
 IF result IS NULL THEN RAISE SQLSTATE 'PT404' USING MESSAGE='Profile unavailable'; END IF;
 RETURN result;
END $$;
CREATE FUNCTION mission_api.initialize_mission(p_launch_id uuid,p_mission_id text) RETURNS jsonb LANGUAGE plpgsql SECURITY INVOKER SET search_path='' AS $$
DECLARE result training.mission_sessions;
BEGIN
 PERFORM training.assert_identity();
 IF NOT training.session_allowed() THEN RAISE SQLSTATE 'PT401' USING MESSAGE='Session revoked'; END IF;
 IF p_launch_id IS NULL OR p_mission_id IS DISTINCT FROM 'mission-red-horizon-poc' THEN RAISE SQLSTATE 'PT409' USING MESSAGE='Invalid mission'; END IF;
 INSERT INTO training.mission_sessions(user_id,auth_session_id,launch_id,mission_id)
 VALUES(auth.uid(),training.claim_session_id(),p_launch_id,p_mission_id) RETURNING * INTO result;
 RETURN to_jsonb(result)-'auth_session_id';
END $$;
CREATE FUNCTION mission_api.complete_mission(p_session_id uuid,p_launch_id uuid,p_outcome text) RETURNS jsonb LANGUAGE plpgsql SECURITY INVOKER SET search_path='' AS $$
DECLARE result training.mission_sessions;
BEGIN
 PERFORM training.assert_identity();
 IF NOT training.session_allowed() THEN RAISE SQLSTATE 'PT401' USING MESSAGE='Session revoked'; END IF;
 SELECT * INTO result FROM training.mission_sessions WHERE id=p_session_id AND user_id=auth.uid() FOR UPDATE;
 IF NOT FOUND THEN RAISE SQLSTATE 'PT404' USING MESSAGE='Mission unavailable'; END IF;
 IF p_outcome IS DISTINCT FROM 'integration_check' OR result.launch_id IS DISTINCT FROM p_launch_id OR result.auth_session_id IS DISTINCT FROM training.claim_session_id() THEN
  RAISE SQLSTATE 'PT409' USING MESSAGE='Mission context mismatch';
 END IF;
 IF result.status='integration_check_complete' THEN RETURN to_jsonb(result)-'auth_session_id'; END IF;
 IF result.expires_at<=now() THEN RAISE SQLSTATE 'PT409' USING MESSAGE='Mission expired'; END IF;
 UPDATE training.mission_sessions SET status='integration_check_complete',completed_at=now()
 WHERE id=p_session_id AND user_id=auth.uid() AND auth_session_id=training.claim_session_id() RETURNING * INTO result;
 IF NOT FOUND THEN RAISE SQLSTATE 'PT409' USING MESSAGE='Mission changed'; END IF;
 RETURN to_jsonb(result)-'auth_session_id';
END $$;
CREATE FUNCTION mission_api.revoke_session() RETURNS boolean LANGUAGE plpgsql SECURITY INVOKER SET search_path='' AS $$
BEGIN
 PERFORM training.assert_identity();
 INSERT INTO training.auth_session_revocations(auth_session_id,user_id,expires_at)
 VALUES(training.claim_session_id(),auth.uid(),to_timestamp((auth.jwt()->>'exp')::bigint)) ON CONFLICT(auth_session_id) DO NOTHING;
 RETURN true;
END $$;
REVOKE ALL ON ALL FUNCTIONS IN SCHEMA mission_api FROM PUBLIC,anon,authenticated;
GRANT EXECUTE ON FUNCTION mission_api.session_revoked(),mission_api.current_profile(),mission_api.initialize_mission(uuid,text),mission_api.complete_mission(uuid,uuid,text),mission_api.revoke_session() TO authenticated;
COMMIT;
-- Separately expose mission_api (ONLY) in approved Data API settings; leave training/auth private.
-- No SECURITY DEFINER in exposed schema, generic SQL, set_config RPC or owner-ID parameter.
'''
(out/'001_verified_jwt_training.sql').write_text(head+s+helpers+trigger+rpc)
(out/'002_retire_direct_runtime.sql').write_text('''-- Idempotent quarantine for any legacy raw-DB application roles. LOCAL ONLY until approved.
BEGIN;
DO $$
DECLARE r text; p record; c record;
BEGIN
 FOREACH r IN ARRAY ARRAY['astrogateway_runtime','astrogateway_api'] LOOP
  IF EXISTS(SELECT 1 FROM pg_roles WHERE rolname=r) THEN
   EXECUTE format('ALTER ROLE %I NOLOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE NOREPLICATION NOBYPASSRLS NOINHERIT CONNECTION LIMIT 0',r);
   FOR p IN SELECT parent.rolname FROM pg_auth_members m JOIN pg_roles parent ON parent.oid=m.roleid JOIN pg_roles child ON child.oid=m.member WHERE child.rolname=r
   LOOP EXECUTE format('REVOKE %I FROM %I',p.rolname,r); END LOOP;
   EXECUTE format('REVOKE ALL ON SCHEMA training,mission_api FROM %I',r);
   EXECUTE format('REVOKE ALL ON ALL TABLES IN SCHEMA training FROM %I',r);
   EXECUTE format('REVOKE ALL ON ALL FUNCTIONS IN SCHEMA training FROM %I',r);
   EXECUTE format('REVOKE ALL ON ALL FUNCTIONS IN SCHEMA mission_api FROM %I',r);
   FOR c IN SELECT n.nspname,t.relname,a.attname FROM pg_attribute a JOIN pg_class t ON t.oid=a.attrelid JOIN pg_namespace n ON n.oid=t.relnamespace
    WHERE n.nspname='training' AND a.attnum>0 AND NOT a.attisdropped LOOP
     EXECUTE format('REVOKE ALL PRIVILEGES (%I) ON TABLE %I.%I FROM %I',c.attname,c.nspname,c.relname,r);
   END LOOP;
   IF EXISTS(SELECT 1 FROM pg_class t JOIN pg_namespace n ON n.oid=t.relnamespace WHERE n.nspname IN ('training','mission_api') AND pg_get_userbyid(t.relowner)=r)
    OR EXISTS(SELECT 1 FROM pg_proc p JOIN pg_namespace n ON n.oid=p.pronamespace WHERE n.nspname IN ('training','mission_api') AND pg_get_userbyid(p.proowner)=r)
    OR EXISTS(SELECT 1 FROM pg_namespace WHERE nspname IN ('training','mission_api') AND pg_get_userbyid(nspowner)=r) THEN
      RAISE EXCEPTION 'Legacy role owns protected objects; stop for ownership review';
   END IF;
  END IF;
 END LOOP;
END $$;
COMMIT;
-- No DROP OWNED/CASCADE. Existing connections must be stopped/terminated separately under approval.
-- NOLOGIN alone does not terminate sessions. Revoked schema/table/column/function privileges deny data.
''')
(out/'003_read_only_verification.sql').write_text('''BEGIN TRANSACTION READ ONLY;
SELECT current_database(),current_user,current_setting('server_version');
SELECT rolname,rolcanlogin,rolsuper,rolbypassrls,rolcreaterole,rolcreatedb,rolreplication FROM pg_roles WHERE rolname IN ('authenticated','astrogateway_runtime','astrogateway_api','authenticator');
SELECT member.rolname,granted.rolname,m.admin_option,m.inherit_option,m.set_option FROM pg_auth_members m JOIN pg_roles member ON member.oid=m.member JOIN pg_roles granted ON granted.oid=m.roleid WHERE member.rolname IN ('astrogateway_runtime','astrogateway_api');
SELECT n.nspname,c.relname,c.relrowsecurity,c.relforcerowsecurity,pg_get_userbyid(c.relowner) FROM pg_class c JOIN pg_namespace n ON n.oid=c.relnamespace WHERE n.nspname='training' AND c.relkind='r';
SELECT n.nspname,p.proname,p.prosecdef,p.proconfig,p.proacl FROM pg_proc p JOIN pg_namespace n ON n.oid=p.pronamespace WHERE n.nspname IN ('training','mission_api');
SELECT schemaname,tablename,policyname,roles,cmd,qual,with_check FROM pg_policies WHERE schemaname='training';
COMMIT;
''')
print('Fresh verified-gateway SQL, legacy role quarantine, read-only verification prepared.')
