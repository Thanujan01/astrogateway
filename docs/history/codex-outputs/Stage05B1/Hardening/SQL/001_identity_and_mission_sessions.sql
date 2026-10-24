-- Stage 05B additive, transactional migration. NOT applied to any live Supabase project.
-- Review existing training schema/role names first; collisions fail without overwriting data.
-- Run as the Supabase migration owner. auth.users remains the only identity provider.
BEGIN;
CREATE ROLE astrogateway_api NOLOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE NOINHERIT NOREPLICATION NOBYPASSRLS;
CREATE SCHEMA training;
REVOKE ALL ON SCHEMA training FROM PUBLIC, anon, authenticated;
GRANT USAGE ON SCHEMA training TO astrogateway_api;

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
GRANT SELECT ON training.profiles,training.mission_sessions,training.auth_session_revocations TO astrogateway_api;
GRANT INSERT (user_id,auth_session_id,launch_id,mission_id) ON training.mission_sessions TO astrogateway_api;
GRANT UPDATE (status,completed_at) ON training.mission_sessions TO astrogateway_api;
GRANT INSERT (auth_session_id,user_id,expires_at) ON training.auth_session_revocations TO astrogateway_api;

-- These transaction-local settings are supplied only by FastAPI AFTER issuer verification.
-- API login credentials stay server-side; the role is never available to public Data API clients.
CREATE FUNCTION training.request_user_id() RETURNS uuid LANGUAGE sql STABLE SET search_path='' AS $$
    SELECT NULLIF(current_setting('astrogateway.user_id',true),'')::uuid;
$$;
CREATE FUNCTION training.request_auth_session_id() RETURNS uuid LANGUAGE sql STABLE SET search_path='' AS $$
    SELECT NULLIF(current_setting('astrogateway.auth_session_id',true),'')::uuid;
$$;
CREATE POLICY revocation_read_own ON training.auth_session_revocations FOR SELECT TO astrogateway_api
    USING (user_id=(SELECT training.request_user_id()));
CREATE POLICY revocation_insert_current ON training.auth_session_revocations FOR INSERT TO astrogateway_api
    WITH CHECK (user_id=(SELECT training.request_user_id()) AND auth_session_id=(SELECT training.request_auth_session_id()));
CREATE FUNCTION training.session_allowed() RETURNS boolean LANGUAGE sql STABLE SET search_path='' AS $$
    SELECT training.request_user_id() IS NOT NULL AND training.request_auth_session_id() IS NOT NULL
      AND NOT EXISTS (SELECT 1 FROM training.auth_session_revocations r
        WHERE r.user_id=training.request_user_id() AND r.auth_session_id=training.request_auth_session_id() AND r.expires_at>now());
$$;
REVOKE ALL ON ALL FUNCTIONS IN SCHEMA training FROM PUBLIC,anon,authenticated;
GRANT EXECUTE ON FUNCTION training.request_user_id(),training.request_auth_session_id(),training.session_allowed() TO astrogateway_api;
CREATE POLICY profile_read_own ON training.profiles FOR SELECT TO astrogateway_api
    USING (user_id=(SELECT training.request_user_id()) AND (SELECT training.session_allowed()));
CREATE POLICY mission_read_own ON training.mission_sessions FOR SELECT TO astrogateway_api
    USING (user_id=(SELECT training.request_user_id()) AND (SELECT training.session_allowed()));
CREATE POLICY mission_insert_current ON training.mission_sessions FOR INSERT TO astrogateway_api
    WITH CHECK (user_id=(SELECT training.request_user_id()) AND auth_session_id=(SELECT training.request_auth_session_id())
      AND (SELECT training.session_allowed()) AND status='initialized' AND completed_at IS NULL AND expires_at>now());
CREATE POLICY mission_complete_current ON training.mission_sessions FOR UPDATE TO astrogateway_api
    USING (user_id=(SELECT training.request_user_id()) AND auth_session_id=(SELECT training.request_auth_session_id())
      AND (SELECT training.session_allowed()) AND ((status='initialized' AND expires_at>now()) OR status='integration_check_complete'))
    WITH CHECK (user_id=(SELECT training.request_user_id()) AND auth_session_id=(SELECT training.request_auth_session_id())
      AND (SELECT training.session_allowed()) AND status='integration_check_complete' AND completed_at IS NOT NULL AND completed_at<=now()+interval '5 seconds');

CREATE FUNCTION training.create_auth_profile() RETURNS trigger LANGUAGE plpgsql SECURITY DEFINER SET search_path='' AS $$
DECLARE display text;
BEGIN
    display := left(COALESCE(NULLIF(btrim(NEW.raw_user_meta_data->>'display_name'),''),'Astronaut'),64);
    IF char_length(display)<2 THEN display := 'Astronaut'; END IF;
    INSERT INTO training.profiles(user_id,display_name,created_at) VALUES(NEW.id,display,NEW.created_at);
    RETURN NEW;
END;
$$;
REVOKE ALL ON FUNCTION training.create_auth_profile() FROM PUBLIC,anon,authenticated,astrogateway_api;
CREATE TRIGGER astrogateway_auth_user_profile AFTER INSERT ON auth.users FOR EACH ROW EXECUTE FUNCTION training.create_auth_profile();
INSERT INTO training.profiles(user_id,display_name,created_at)
SELECT id,CASE WHEN char_length(btrim(COALESCE(raw_user_meta_data->>'display_name','')))<2 THEN 'Astronaut'
    ELSE left(btrim(raw_user_meta_data->>'display_name'),64) END,created_at FROM auth.users;
COMMIT;
-- Separately provision a dedicated LOGIN role with a secret managed on the SERVER:
-- CREATE ROLE your_runtime_login LOGIN NOINHERIT NOSUPERUSER NOCREATEDB NOCREATEROLE NOBYPASSRLS;
-- Assign a generated password using an approved secret-management mechanism; do not commit it.
-- GRANT astrogateway_api TO your_runtime_login;
-- Do not expose training in Supabase Data API settings. Never use postgres/service_role in the APK.
