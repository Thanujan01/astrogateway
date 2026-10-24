-- LOCAL FIXTURE ONLY, destructive application teardown; not an authorized remote rollback.
BEGIN;
DROP TRIGGER astrogateway_auth_user_profile ON auth.users;
DROP FUNCTION mission_api.current_profile();
DROP FUNCTION mission_api.session_revoked();
DROP FUNCTION mission_api.initialize_mission(uuid,text);
DROP FUNCTION mission_api.complete_mission(uuid,uuid,text);
DROP FUNCTION mission_api.revoke_session();
DROP FUNCTION training.assert_identity();
DROP FUNCTION training.create_auth_profile();
DROP TABLE training.mission_sessions;
DROP POLICY profile_read_own ON training.profiles;
DROP FUNCTION training.session_allowed();
DROP TABLE training.auth_session_revocations;
DROP TABLE training.profiles;
DROP FUNCTION training.claims_valid();
DROP FUNCTION training.claim_session_id();
DROP FUNCTION training.confirmed_user();
DROP SCHEMA mission_api;
DROP SCHEMA training;
ALTER DEFAULT PRIVILEGES FOR ROLE astrogateway_schema_owner GRANT EXECUTE ON FUNCTIONS TO PUBLIC;
DROP ROLE astrogateway_schema_owner;
COMMIT;
-- Auth users/provider fixture roles preserved. Legacy role quarantine remains (never re-enable unsafe path).
