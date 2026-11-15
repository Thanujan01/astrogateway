-- Stage05B.2 FRESH INSTALL candidate. Local rehearsal only; remote execution NOT approved.
-- Requires Supabase auth.uid/auth.jwt, verified gateway, NOLOGIN authenticated role.
-- Collisions abort. DO NOT apply on an installed old training schema; use reviewed transition instead.
BEGIN;
DO $$ BEGIN
 IF NOT EXISTS(SELECT 1 FROM pg_roles WHERE rolname='authenticated' AND NOT rolcanlogin AND NOT rolsuper AND NOT rolbypassrls AND NOT rolcreaterole AND NOT rolcreatedb AND NOT rolreplication) THEN
  RAISE EXCEPTION 'Expected safe NOLOGIN authenticated role missing';
 END IF;
 IF EXISTS(SELECT 1 FROM pg_auth_members WHERE member=(SELECT oid FROM pg_roles WHERE rolname='authenticated')) THEN RAISE EXCEPTION 'Unexpected authenticated parent role; review required'; END IF;
 IF to_regprocedure('auth.uid()') IS NULL OR to_regprocedure('auth.jwt()') IS NULL THEN RAISE EXCEPTION 'Supabase JWT helpers missing'; END IF;
END $$;
CREATE ROLE astrogateway_schema_owner NOLOGIN NOINHERIT NOSUPERUSER NOCREATEDB NOCREATEROLE NOREPLICATION NOBYPASSRLS;
-- HOSTED DEVIATION: Supabase's `postgres` is not a superuser, so it must be a member of the
-- new owner role to set its default privileges and to transfer object ownership to it below.
GRANT astrogateway_schema_owner TO postgres;
ALTER DEFAULT PRIVILEGES FOR ROLE astrogateway_schema_owner REVOKE EXECUTE ON FUNCTIONS FROM PUBLIC;
CREATE SCHEMA training;
REVOKE ALL ON SCHEMA training FROM PUBLIC, anon, authenticated;
GRANT USAGE ON SCHEMA training TO authenticated;

CREATE TABLE training.profiles (
    user_id uuid PRIMARY KEY REFERENCES auth.users(id) ON DELETE CASCADE,
    display_name text NOT NULL CHECK (char_length(display_name) BETWEEN 2 AND 64),
    trainee_profile text NOT NULL DEFAULT 'astronaut_candidate' CHECK (trainee_profile='astronaut_candidate'),
    training_level smallint NOT NULL DEFAULT 0 CHECK (training_level BETWEEN 0 AND 10),
    created_at timestamptz NOT NULL DEFAULT now()
);
CREATE TABLE training.auth_session_revocations (
    auth_session_id uuid PRIMARY KEY,
    user_id uuid NOT NULL REFERENCES training.profiles(user_id) ON DELETE CASCADE,
    revoked_at timestamptz NOT NULL DEFAULT now(),
    expires_at timestamptz NOT NULL
);
CREATE TABLE training.mission_sessions (
    id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id uuid NOT NULL REFERENCES training.profiles(user_id) ON DELETE CASCADE,
    auth_session_id uuid NOT NULL,
    launch_id uuid NOT NULL UNIQUE,
    mission_id text NOT NULL CHECK (mission_id='mission-red-horizon-poc'),
    status text NOT NULL DEFAULT 'initialized' CHECK (status IN ('initialized','integration_check_complete')),
    created_at timestamptz NOT NULL DEFAULT now(),
    expires_at timestamptz NOT NULL DEFAULT (now()+interval '30 minutes'),
    completed_at timestamptz,
    CHECK ((status='initialized' AND completed_at IS NULL) OR (status='integration_check_complete' AND completed_at IS NOT NULL))
);
CREATE INDEX mission_sessions_user_created_idx ON training.mission_sessions(user_id,created_at DESC);
ALTER TABLE training.profiles ENABLE ROW LEVEL SECURITY;
ALTER TABLE training.auth_session_revocations ENABLE ROW LEVEL SECURITY;
ALTER TABLE training.mission_sessions ENABLE ROW LEVEL SECURITY;
REVOKE ALL ON ALL TABLES IN SCHEMA training FROM PUBLIC,anon,authenticated;
GRANT SELECT ON training.profiles,training.mission_sessions,training.auth_session_revocations TO authenticated;
GRANT INSERT (user_id,auth_session_id,launch_id,mission_id) ON training.mission_sessions TO authenticated;
GRANT UPDATE (status,completed_at) ON training.mission_sessions TO authenticated;
GRANT INSERT (auth_session_id,user_id,expires_at) ON training.auth_session_revocations TO authenticated;


-- Only the independently JWT-verifying gateway has SET access to authenticated.
-- No host raw DB credential or client-controlled arbitrary SQL/RPC setter is allowed.
CREATE FUNCTION training.claim_session_id() RETURNS uuid LANGUAGE sql STABLE SECURITY INVOKER SET search_path='' AS $$
 SELECT NULLIF(auth.jwt()->>'session_id','')::uuid;
$$;
CREATE FUNCTION training.claims_valid() RETURNS boolean LANGUAGE plpgsql STABLE SECURITY INVOKER SET search_path='' AS $$
DECLARE c jsonb:=auth.jwt();
BEGIN
 RETURN auth.uid() IS NOT NULL AND training.claim_session_id() IS NOT NULL
 AND c->>'iss'='https://trzxbhzgjwbihpzdgtfu.supabase.co/auth/v1'
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
CREATE FUNCTION training.create_auth_profile() RETURNS trigger LANGUAGE plpgsql SECURITY DEFINER SET search_path='' AS $$
DECLARE display text;
BEGIN
    display := left(COALESCE(NULLIF(btrim(NEW.raw_user_meta_data->>'display_name'),''),'Astronaut'),64);
    IF char_length(display)<2 THEN display := 'Astronaut'; END IF;
    INSERT INTO training.profiles(user_id,display_name,created_at) VALUES(NEW.id,display,NEW.created_at);
    RETURN NEW;
END;
$$;
REVOKE ALL ON FUNCTION training.create_auth_profile() FROM PUBLIC,anon,authenticated;
CREATE TRIGGER astrogateway_auth_user_profile AFTER INSERT ON auth.users FOR EACH ROW EXECUTE FUNCTION training.create_auth_profile();
INSERT INTO training.profiles(user_id,display_name,created_at)
SELECT id,CASE WHEN char_length(btrim(COALESCE(raw_user_meta_data->>'display_name','')))<2 THEN 'Astronaut'
    ELSE left(btrim(raw_user_meta_data->>'display_name'),64) END,created_at FROM auth.users;

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
 SELECT * INTO result FROM training.mission_sessions WHERE id=p_session_id AND user_id=auth.uid();
 IF NOT FOUND THEN RAISE SQLSTATE 'PT404' USING MESSAGE='Mission unavailable'; END IF;
 IF p_outcome IS DISTINCT FROM 'integration_check' OR result.launch_id IS DISTINCT FROM p_launch_id OR result.auth_session_id IS DISTINCT FROM training.claim_session_id() THEN
  RAISE SQLSTATE 'PT409' USING MESSAGE='Mission context mismatch';
 END IF;
 -- Validate context before locking; UPDATE RLS would otherwise hide another own session at FOR UPDATE.
 SELECT * INTO result FROM training.mission_sessions WHERE id=p_session_id AND user_id=auth.uid() FOR UPDATE;
 IF NOT FOUND THEN RAISE SQLSTATE 'PT409' USING MESSAGE='Mission changed'; END IF;
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
-- Dedicated non-login ownership avoids changing shared provider owners' global defaults.
ALTER SCHEMA training OWNER TO astrogateway_schema_owner;
ALTER SCHEMA mission_api OWNER TO astrogateway_schema_owner;
ALTER TABLE training.profiles OWNER TO astrogateway_schema_owner;
ALTER TABLE training.auth_session_revocations OWNER TO astrogateway_schema_owner;
ALTER TABLE training.mission_sessions OWNER TO astrogateway_schema_owner;
ALTER FUNCTION training.claim_session_id() OWNER TO astrogateway_schema_owner;
ALTER FUNCTION training.claims_valid() OWNER TO astrogateway_schema_owner;
ALTER FUNCTION training.session_allowed() OWNER TO astrogateway_schema_owner;
ALTER FUNCTION training.assert_identity() OWNER TO astrogateway_schema_owner;
ALTER FUNCTION mission_api.session_revoked() OWNER TO astrogateway_schema_owner;
ALTER FUNCTION mission_api.current_profile() OWNER TO astrogateway_schema_owner;
ALTER FUNCTION mission_api.initialize_mission(uuid,text) OWNER TO astrogateway_schema_owner;
ALTER FUNCTION mission_api.complete_mission(uuid,uuid,text) OWNER TO astrogateway_schema_owner;
ALTER FUNCTION mission_api.revoke_session() OWNER TO astrogateway_schema_owner;
-- Private confirmed_user/profile-trigger definers retain reviewed administrator ownership.
-- All future app functions must be created/owned by schema_owner and explicitly granted.
COMMIT;
-- Separately expose mission_api (ONLY) in approved Data API settings; leave training/auth private.
-- No SECURITY DEFINER in exposed schema, generic SQL, set_config RPC or owner-ID parameter.
