-- Candidate: after revised base + 004 profile policy; not deployed to hosted Supabase.
BEGIN;
ALTER TABLE training.profiles ADD COLUMN username text,
 ADD COLUMN avatar_path text,
 ADD CONSTRAINT profile_username_shape CHECK (username IS NULL OR username ~ '^[a-z][a-z0-9_]{2,23}$'),
 ADD CONSTRAINT profile_avatar_owner CHECK (avatar_path IS NULL OR avatar_path=user_id::text||'/avatar.png');
CREATE UNIQUE INDEX profiles_username_unique ON training.profiles(username) WHERE username IS NOT NULL;
GRANT UPDATE (username,avatar_path) ON training.profiles TO authenticated;
CREATE FUNCTION mission_api.set_username(p_username text,p_expected_user uuid DEFAULT NULL) RETURNS jsonb
LANGUAGE plpgsql SECURITY INVOKER SET search_path='' AS $$
DECLARE clean text:=lower(btrim(p_username)); result jsonb;
BEGIN
 PERFORM training.assert_identity();
 IF p_expected_user IS NOT NULL AND p_expected_user IS DISTINCT FROM auth.uid() THEN RAISE SQLSTATE 'PT401' USING MESSAGE='Account changed'; END IF;
 IF training.session_allowed() IS NOT TRUE THEN RAISE SQLSTATE 'PT401' USING MESSAGE='Session unavailable'; END IF;
 IF clean IS NULL OR clean !~ '^[a-z][a-z0-9_]{2,23}$' THEN RAISE SQLSTATE 'PA422' USING MESSAGE='Invalid username'; END IF;
 UPDATE training.profiles SET username=clean WHERE user_id=auth.uid() RETURNING to_jsonb(profiles.*) INTO result;
 IF result IS NULL THEN RAISE SQLSTATE 'PT404' USING MESSAGE='Profile unavailable'; END IF;
 RETURN result;
EXCEPTION WHEN unique_violation THEN RAISE SQLSTATE 'PA409' USING MESSAGE='Username unavailable';
END $$;
CREATE FUNCTION mission_api.set_avatar(p_avatar_path text,p_expected_user uuid DEFAULT NULL) RETURNS jsonb
LANGUAGE plpgsql SECURITY INVOKER SET search_path='' AS $$
DECLARE result jsonb;
BEGIN
 PERFORM training.assert_identity();
 IF p_expected_user IS NOT NULL AND p_expected_user IS DISTINCT FROM auth.uid() THEN RAISE SQLSTATE 'PT401' USING MESSAGE='Account changed'; END IF;
 IF training.session_allowed() IS NOT TRUE THEN RAISE SQLSTATE 'PT401' USING MESSAGE='Session unavailable'; END IF;
 IF p_avatar_path IS NOT NULL AND p_avatar_path IS DISTINCT FROM auth.uid()::text||'/avatar.png' THEN RAISE SQLSTATE 'PA422' USING MESSAGE='Invalid avatar path'; END IF;
 UPDATE training.profiles SET avatar_path=p_avatar_path WHERE user_id=auth.uid() RETURNING to_jsonb(profiles.*) INTO result;
 IF result IS NULL THEN RAISE SQLSTATE 'PT404' USING MESSAGE='Profile unavailable'; END IF;
 RETURN result;
END $$;
REVOKE ALL ON FUNCTION mission_api.set_username(text,uuid),mission_api.set_avatar(text,uuid) FROM PUBLIC,anon;
GRANT EXECUTE ON FUNCTION mission_api.set_username(text,uuid),mission_api.set_avatar(text,uuid) TO authenticated;
ALTER FUNCTION mission_api.set_username(text,uuid) OWNER TO astrogateway_schema_owner;
ALTER FUNCTION mission_api.set_avatar(text,uuid) OWNER TO astrogateway_schema_owner;
COMMIT;
