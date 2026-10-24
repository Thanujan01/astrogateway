-- LOCAL DISPOSABLE FIXTURE ONLY. Not a remote rollback/ACL restore script.
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
