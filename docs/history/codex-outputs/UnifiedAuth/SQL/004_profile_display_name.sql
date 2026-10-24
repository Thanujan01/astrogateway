-- LOCAL REHEARSAL / OPERATOR REVIEW ONLY. Not executed on hosted Supabase.
-- Requires Stage05B2 001_verified_jwt_training.sql, not the obsolete Stage05B migration.
-- Existing-object collisions abort; run once transactionally. No credential/owner-ID inputs.
BEGIN;
GRANT UPDATE (display_name) ON training.profiles TO authenticated;
CREATE POLICY profile_update_own ON training.profiles FOR UPDATE TO authenticated
 USING (user_id=(SELECT auth.uid()) AND (SELECT training.session_allowed()))
 WITH CHECK (user_id=(SELECT auth.uid()) AND (SELECT training.session_allowed()));
CREATE FUNCTION mission_api.update_display_name(p_display_name text) RETURNS jsonb
 LANGUAGE plpgsql SECURITY INVOKER SET search_path='' AS $$
DECLARE result jsonb; clean_name text := btrim(p_display_name);
BEGIN
 PERFORM training.assert_identity();
 IF NOT training.session_allowed() THEN RAISE SQLSTATE 'PT401' USING MESSAGE='Session revoked'; END IF;
 IF clean_name IS NULL OR char_length(clean_name) NOT BETWEEN 2 AND 64 OR clean_name ~ '[[:cntrl:]]' THEN
  RAISE SQLSTATE 'PT422' USING MESSAGE='Invalid display name';
 END IF;
 UPDATE training.profiles SET display_name=clean_name WHERE user_id=auth.uid() RETURNING to_jsonb(profiles.*) INTO result;
 IF result IS NULL THEN RAISE SQLSTATE 'PT404' USING MESSAGE='Profile unavailable'; END IF;
 RETURN result;
END $$;
REVOKE ALL ON FUNCTION mission_api.update_display_name(text) FROM PUBLIC,anon,authenticated;
GRANT EXECUTE ON FUNCTION mission_api.update_display_name(text) TO authenticated;
ALTER FUNCTION mission_api.update_display_name(text) OWNER TO astrogateway_schema_owner;
COMMIT;
-- A separately approved gateway schema-cache reload will be required when hosted execution is authorized.
