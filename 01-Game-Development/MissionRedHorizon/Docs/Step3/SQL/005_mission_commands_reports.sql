-- Step 3 candidate (2026-10-17). LOCAL REHEARSAL / OPERATOR REVIEW ONLY. Not executed anywhere yet.
-- Requires Stage05B2 001_verified_jwt_training.sql (+ UnifiedAuth 004). Existing objects are NOT altered.
-- Adds an append-only semantic command log and read-only evidence summaries.
-- No score, level, oxygen, weather or recommendation rule is implemented (pending owner approval).
BEGIN;
CREATE TABLE training.mission_commands (
    session_id uuid NOT NULL REFERENCES training.mission_sessions(id) ON DELETE CASCADE,
    command_id uuid NOT NULL,
    user_id uuid NOT NULL REFERENCES training.profiles(user_id) ON DELETE CASCADE,
    auth_session_id uuid NOT NULL,
    revision integer NOT NULL CHECK (revision BETWEEN 0 AND 499),
    kind text NOT NULL CHECK (kind IN ('board_rover','exit_rover','scan_target','weather_changed','pause','resume')),
    payload jsonb NOT NULL CHECK (jsonb_typeof(payload)='object' AND octet_length(payload::text)<=1024),
    accepted_at timestamptz NOT NULL DEFAULT now(),
    PRIMARY KEY (session_id,command_id),
    UNIQUE (session_id,revision)
);
CREATE INDEX mission_commands_user_idx ON training.mission_commands(user_id,session_id);
ALTER TABLE training.mission_commands ENABLE ROW LEVEL SECURITY;
REVOKE ALL ON training.mission_commands FROM PUBLIC,anon,authenticated;
GRANT SELECT ON training.mission_commands TO authenticated;
GRANT INSERT (session_id,command_id,user_id,auth_session_id,revision,kind,payload) ON training.mission_commands TO authenticated;
CREATE POLICY command_read_own ON training.mission_commands FOR SELECT TO authenticated
 USING(user_id=(SELECT auth.uid()) AND (SELECT training.session_allowed()));
-- Inserts only into the caller's own, still-open session created by the same Auth session.
CREATE POLICY command_insert_current ON training.mission_commands FOR INSERT TO authenticated
 WITH CHECK(user_id=(SELECT auth.uid()) AND auth_session_id=(SELECT training.claim_session_id()) AND (SELECT training.session_allowed())
 AND EXISTS(SELECT 1 FROM training.mission_sessions s WHERE s.id=session_id AND s.user_id=(SELECT auth.uid())
  AND s.auth_session_id=(SELECT training.claim_session_id()) AND s.status='initialized' AND s.expires_at>now()));

-- Per-kind payload shape. Mirrors the FastAPI models (defense in depth).
CREATE FUNCTION training.command_payload_valid(p_kind text,p_payload jsonb) RETURNS boolean LANGUAGE plpgsql IMMUTABLE SECURITY INVOKER SET search_path='' AS $$
BEGIN
 IF p_payload IS NULL OR jsonb_typeof(p_payload)<>'object' OR octet_length(p_payload::text)>1024 THEN RETURN false; END IF;
 IF p_kind='scan_target' THEN
  RETURN (SELECT count(*) FROM jsonb_object_keys(p_payload))=3
   AND (p_payload->>'sample_id') ~ '^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$'
   AND jsonb_typeof(p_payload->'tick')='number' AND (p_payload->>'tick')::numeric BETWEEN 0 AND 1000000000
   AND p_payload->>'analysis'='unverified_simulated';
 ELSIF p_kind='weather_changed' THEN
  RETURN (SELECT count(*) FROM jsonb_object_keys(p_payload))=1 AND jsonb_typeof(p_payload->'dust')='number' AND (p_payload->>'dust') IN ('0','1','2');
 ELSIF p_kind IN ('board_rover','exit_rover','pause','resume') THEN
  RETURN p_payload='{}'::jsonb;
 END IF;
 RETURN false;
END $$;

CREATE FUNCTION training.command_evidence(p_session uuid) RETURNS jsonb LANGUAGE sql STABLE SECURITY INVOKER SET search_path='' AS $$
 SELECT jsonb_build_object(
  'simulated_scans',count(*) FILTER (WHERE c.kind='scan_target'),
  'rover_board_exit_cycles',LEAST(count(*) FILTER (WHERE c.kind='board_rover'),count(*) FILTER (WHERE c.kind='exit_rover')),
  'weather_changes',count(*) FILTER (WHERE c.kind='weather_changed'))
 FROM training.mission_commands c WHERE c.user_id=auth.uid() AND (p_session IS NULL OR c.session_id=p_session);
$$;

CREATE FUNCTION mission_api.submit_mission_command(p_session_id uuid,p_command_id uuid,p_expected_revision integer,p_kind text,p_payload jsonb)
 RETURNS jsonb LANGUAGE plpgsql SECURITY INVOKER SET search_path='' AS $$
DECLARE s training.mission_sessions; existing training.mission_commands; current_revision integer; result training.mission_commands;
BEGIN
 PERFORM training.assert_identity();
 IF NOT training.session_allowed() THEN RAISE SQLSTATE 'PT401' USING MESSAGE='Session revoked'; END IF;
 SELECT * INTO s FROM training.mission_sessions WHERE id=p_session_id AND user_id=auth.uid();
 IF NOT FOUND THEN RAISE SQLSTATE 'PT404' USING MESSAGE='Mission unavailable'; END IF;
 IF p_command_id IS NULL OR p_expected_revision IS NULL OR NOT training.command_payload_valid(p_kind,p_payload) THEN
  RAISE SQLSTATE 'PC422' USING MESSAGE='Invalid mission command';
 END IF;
 -- Idempotency: the same command id with identical content returns the saved acknowledgement.
 SELECT * INTO existing FROM training.mission_commands WHERE session_id=p_session_id AND command_id=p_command_id;
 IF FOUND THEN
  IF existing.kind=p_kind AND existing.payload=p_payload AND existing.revision=p_expected_revision THEN
   RETURN jsonb_build_object('session_id',existing.session_id,'command_id',existing.command_id,'kind',existing.kind,'revision',existing.revision+1,'accepted_at',existing.accepted_at,'duplicate',true);
  END IF;
  RAISE SQLSTATE 'PC409' USING MESSAGE='Command id reused with different content';
 END IF;
 IF s.auth_session_id IS DISTINCT FROM training.claim_session_id() OR s.status<>'initialized' OR s.expires_at<=now() THEN
  RAISE SQLSTATE 'PT409' USING MESSAGE='Mission not open for commands';
 END IF;
 SELECT count(*) INTO current_revision FROM training.mission_commands WHERE session_id=p_session_id;
 IF current_revision>=500 THEN RAISE SQLSTATE 'PC429' USING MESSAGE='Command limit reached'; END IF;
 IF p_expected_revision<>current_revision THEN RAISE SQLSTATE 'PC409' USING MESSAGE='Revision conflict'; END IF;
 -- A concurrent insert at the same revision fails on UNIQUE(session_id,revision) and surfaces as 409.
 INSERT INTO training.mission_commands(session_id,command_id,user_id,auth_session_id,revision,kind,payload)
 VALUES(p_session_id,p_command_id,auth.uid(),training.claim_session_id(),current_revision,p_kind,p_payload) RETURNING * INTO result;
 RETURN jsonb_build_object('session_id',result.session_id,'command_id',result.command_id,'kind',result.kind,'revision',result.revision+1,'accepted_at',result.accepted_at,'duplicate',false);
END $$;

CREATE FUNCTION mission_api.mission_state(p_session_id uuid) RETURNS jsonb LANGUAGE plpgsql STABLE SECURITY INVOKER SET search_path='' AS $$
DECLARE s training.mission_sessions; v_revision integer; last_at timestamptz; last_pause text;
BEGIN
 PERFORM training.assert_identity();
 IF NOT training.session_allowed() THEN RAISE SQLSTATE 'PT401' USING MESSAGE='Session revoked'; END IF;
 SELECT * INTO s FROM training.mission_sessions WHERE id=p_session_id AND user_id=auth.uid();
 IF NOT FOUND THEN RAISE SQLSTATE 'PT404' USING MESSAGE='Mission unavailable'; END IF;
 SELECT count(*),max(accepted_at) INTO v_revision,last_at FROM training.mission_commands WHERE session_id=p_session_id AND user_id=auth.uid();
 SELECT kind INTO last_pause FROM training.mission_commands WHERE session_id=p_session_id AND user_id=auth.uid() AND kind IN ('pause','resume') ORDER BY revision DESC LIMIT 1;
 RETURN (to_jsonb(s)-'auth_session_id'-'launch_id')||jsonb_build_object('revision',v_revision,'paused',COALESCE(last_pause='pause',false),'last_command_at',last_at);
END $$;

CREATE FUNCTION mission_api.mission_report(p_session_id uuid) RETURNS jsonb LANGUAGE plpgsql STABLE SECURITY INVOKER SET search_path='' AS $$
DECLARE s training.mission_sessions;
BEGIN
 PERFORM training.assert_identity();
 IF NOT training.session_allowed() THEN RAISE SQLSTATE 'PT401' USING MESSAGE='Session revoked'; END IF;
 SELECT * INTO s FROM training.mission_sessions WHERE id=p_session_id AND user_id=auth.uid();
 IF NOT FOUND THEN RAISE SQLSTATE 'PT404' USING MESSAGE='Mission unavailable'; END IF;
 RETURN jsonb_build_object('session_id',s.id,'user_id',s.user_id,'mission_id',s.mission_id,'status',s.status,
  'evidence',training.command_evidence(s.id),'score',NULL,'rubric_status','pending_approval');
END $$;

CREATE FUNCTION mission_api.skill_evidence() RETURNS jsonb LANGUAGE plpgsql STABLE SECURITY INVOKER SET search_path='' AS $$
BEGIN
 PERFORM training.assert_identity();
 IF NOT training.session_allowed() THEN RAISE SQLSTATE 'PT401' USING MESSAGE='Session revoked'; END IF;
 RETURN jsonb_build_object('user_id',auth.uid(),
  'sessions',(SELECT count(*) FROM training.mission_sessions WHERE user_id=auth.uid()),
  'evidence',training.command_evidence(NULL),'levels',NULL,'rubric_status','pending_approval');
END $$;

REVOKE ALL ON FUNCTION training.command_payload_valid(text,jsonb),training.command_evidence(uuid) FROM PUBLIC,anon,authenticated;
GRANT EXECUTE ON FUNCTION training.command_payload_valid(text,jsonb),training.command_evidence(uuid) TO authenticated;
REVOKE ALL ON FUNCTION mission_api.submit_mission_command(uuid,uuid,integer,text,jsonb),mission_api.mission_state(uuid),
 mission_api.mission_report(uuid),mission_api.skill_evidence() FROM PUBLIC,anon,authenticated;
GRANT EXECUTE ON FUNCTION mission_api.submit_mission_command(uuid,uuid,integer,text,jsonb),mission_api.mission_state(uuid),
 mission_api.mission_report(uuid),mission_api.skill_evidence() TO authenticated;
ALTER TABLE training.mission_commands OWNER TO astrogateway_schema_owner;
ALTER FUNCTION training.command_payload_valid(text,jsonb) OWNER TO astrogateway_schema_owner;
ALTER FUNCTION training.command_evidence(uuid) OWNER TO astrogateway_schema_owner;
ALTER FUNCTION mission_api.submit_mission_command(uuid,uuid,integer,text,jsonb) OWNER TO astrogateway_schema_owner;
ALTER FUNCTION mission_api.mission_state(uuid) OWNER TO astrogateway_schema_owner;
ALTER FUNCTION mission_api.mission_report(uuid) OWNER TO astrogateway_schema_owner;
ALTER FUNCTION mission_api.skill_evidence() OWNER TO astrogateway_schema_owner;
COMMIT;
-- A separately approved gateway schema-cache reload is required when hosted execution is authorized.
