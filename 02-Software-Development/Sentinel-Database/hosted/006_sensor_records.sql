-- First Mission candidate (2026-10-20). LOCAL REHEARSAL / OPERATOR REVIEW ONLY. Tested in disposable local PostgreSQL only; no hosted deployment.
-- Requires Step 3 005_mission_commands_reports.sql. Existing objects are NOT altered.
-- Append-only rover sensor records. The client may only store SIMULATED or UNAVAILABLE observations;
-- analysis results (MODEL-ESTIMATED / BACKEND-ANALYZED) have no client write path at all.
BEGIN;
CREATE TABLE training.sensor_records (
    session_id uuid NOT NULL REFERENCES training.mission_sessions(id) ON DELETE CASCADE,
    event_id uuid NOT NULL,
    user_id uuid NOT NULL REFERENCES training.profiles(user_id) ON DELETE CASCADE,
    auth_session_id uuid NOT NULL,
    sensor_id text NOT NULL CHECK (sensor_id ~ '^[a-z]+\.[a-z_]{2,48}$'),
    provenance text NOT NULL CHECK (provenance IN ('SIMULATED','UNAVAILABLE')),
    record jsonb NOT NULL CHECK (jsonb_typeof(record)='object' AND octet_length(record::text)<=2048),
    stored_at timestamptz NOT NULL DEFAULT now(),
    PRIMARY KEY (session_id,event_id)
);
CREATE INDEX sensor_records_user_idx ON training.sensor_records(user_id,session_id);
ALTER TABLE training.sensor_records ENABLE ROW LEVEL SECURITY;
REVOKE ALL ON training.sensor_records FROM PUBLIC,anon,authenticated;
GRANT SELECT ON training.sensor_records TO authenticated;
GRANT INSERT (session_id,event_id,user_id,auth_session_id,sensor_id,provenance,record) ON training.sensor_records TO authenticated;
CREATE POLICY sensor_read_own ON training.sensor_records FOR SELECT TO authenticated
 USING(user_id=(SELECT auth.uid()) AND (SELECT training.session_allowed()));
CREATE POLICY sensor_insert_current ON training.sensor_records FOR INSERT TO authenticated
 WITH CHECK(user_id=(SELECT auth.uid()) AND auth_session_id=(SELECT training.claim_session_id()) AND (SELECT training.session_allowed())
 AND EXISTS(SELECT 1 FROM training.mission_sessions s WHERE s.id=session_id AND s.user_id=(SELECT auth.uid())
  AND s.auth_session_id=(SELECT training.claim_session_id()) AND s.status='initialized' AND s.expires_at>now()));

-- Batch submit, idempotent on (session_id,event_id): identical resubmission = duplicate, different body = PS409.
-- The FastAPI layer has already validated every record against sensor_catalog_v1.json; this re-checks shape limits.
CREATE FUNCTION mission_api.submit_sensor_records(p_session_id uuid,p_records jsonb) RETURNS jsonb
 LANGUAGE plpgsql SECURITY INVOKER SET search_path='' AS $$
DECLARE r jsonb; out jsonb:='[]'::jsonb; existing training.sensor_records; n integer;
BEGIN
 IF NOT (SELECT training.session_allowed()) THEN RAISE SQLSTATE 'PT401' USING MESSAGE='Authentication required'; END IF;
 IF jsonb_typeof(p_records) IS DISTINCT FROM 'array' THEN RAISE SQLSTATE 'PS422' USING MESSAGE='Invalid sensor batch'; END IF;
 IF jsonb_array_length(p_records) NOT BETWEEN 1 AND 50 THEN RAISE SQLSTATE 'PS422' USING MESSAGE='PS422'; END IF;
 IF NOT EXISTS(SELECT 1 FROM training.mission_sessions s WHERE s.id=p_session_id AND s.user_id=(SELECT auth.uid())) THEN RAISE SQLSTATE 'PT404' USING MESSAGE='PT404'; END IF;
 SELECT count(*) INTO n FROM training.sensor_records WHERE session_id=p_session_id;
 FOR r IN SELECT * FROM jsonb_array_elements(p_records) LOOP
  IF jsonb_typeof(r) IS DISTINCT FROM 'object' OR r->>'provenance' IS NULL OR (r->>'provenance') NOT IN ('SIMULATED','UNAVAILABLE') OR octet_length(r::text)>2048 THEN RAISE SQLSTATE 'PS422' USING MESSAGE='PS422'; END IF;
  SELECT * INTO existing FROM training.sensor_records WHERE session_id=p_session_id AND event_id=(r->>'event_id')::uuid;
  IF FOUND THEN
   IF existing.record<>r THEN RAISE SQLSTATE 'PS409' USING MESSAGE='PS409'; END IF;
   out:=out||jsonb_build_object('session_id',p_session_id,'event_id',existing.event_id,'duplicate',true,'stored_at',existing.stored_at);
  ELSE
   IF n>=2000 THEN RAISE SQLSTATE 'PS429' USING MESSAGE='PS429'; END IF;
   INSERT INTO training.sensor_records(session_id,event_id,user_id,auth_session_id,sensor_id,provenance,record)
    VALUES(p_session_id,(r->>'event_id')::uuid,(SELECT auth.uid()),(SELECT training.claim_session_id()),r->>'sensor_id',r->>'provenance',r)
    RETURNING * INTO existing;
   n:=n+1;
   out:=out||jsonb_build_object('session_id',p_session_id,'event_id',existing.event_id,'duplicate',false,'stored_at',existing.stored_at);
  END IF;
 END LOOP;
 RETURN out;
END $$;
REVOKE ALL ON FUNCTION mission_api.submit_sensor_records(uuid,jsonb) FROM PUBLIC,anon;
GRANT EXECUTE ON FUNCTION mission_api.submit_sensor_records(uuid,jsonb) TO authenticated;
-- HOSTED DEVIATION: the source migration never transfers ownership of these two objects, so they
-- would stay owned by the migration role. 001 states every app object must be owned by
-- astrogateway_schema_owner, so ownership is made consistent here.
ALTER TABLE training.sensor_records OWNER TO astrogateway_schema_owner;
ALTER FUNCTION mission_api.submit_sensor_records(uuid,jsonb) OWNER TO astrogateway_schema_owner;
COMMIT;
