-- Revoke editing capability; preserve existing profile names/data and all Auth identities.
BEGIN;
DROP FUNCTION mission_api.update_display_name(text);
DROP POLICY profile_update_own ON training.profiles;
REVOKE UPDATE (display_name) ON training.profiles FROM authenticated;
COMMIT;
