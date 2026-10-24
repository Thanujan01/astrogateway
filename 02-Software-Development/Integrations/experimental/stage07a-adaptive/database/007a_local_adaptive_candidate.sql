-- Stage07A candidate; LOCAL DISPOSABLE DATABASE ONLY. Never execute remotely without new review/approval.
-- Depends on exact Stage05B2 verified-JWT training schema and NOLOGIN owner.
BEGIN;
CREATE SCHEMA training_ai AUTHORIZATION astrogateway_schema_owner;
REVOKE ALL ON SCHEMA training_ai FROM PUBLIC,anon,authenticated;
GRANT USAGE ON SCHEMA training_ai TO authenticated;
CREATE TABLE training_ai.events (
 user_id uuid NOT NULL REFERENCES training.profiles(user_id),
 event_id uuid NOT NULL,
 mission_session_id uuid NOT NULL REFERENCES training.mission_sessions(id),
 elapsed_s numeric NOT NULL CHECK(elapsed_s>=0 AND elapsed_s<=7200),
 payload jsonb NOT NULL CHECK(jsonb_typeof(payload)='object' AND octet_length(payload::text)<=2048 AND payload->>'kind' IN ('movement','reaction','resource','equipment','emergency','mistake','completion') AND NOT(payload ?| ARRAY['user_id','score','final_score','attested','trusted'])),
 PRIMARY KEY(user_id,event_id),UNIQUE(mission_session_id,elapsed_s)
);
CREATE TABLE training_ai.attestations (
 mission_session_id uuid PRIMARY KEY REFERENCES training.mission_sessions(id),
 user_id uuid NOT NULL REFERENCES training.profiles(user_id),
 success boolean NOT NULL,aggregate numeric NOT NULL CHECK(aggregate BETWEEN 0 AND 100),
 difficulty text NOT NULL CHECK(difficulty IN ('Easy','Medium','Hard')),
 skill_scores jsonb NOT NULL CHECK(jsonb_typeof(skill_scores)='object'),
 cycle integer NOT NULL DEFAULT 0 CHECK(cycle BETWEEN 0 AND 10000),
 training_day smallint CHECK(training_day BETWEEN 1 AND 30)
);
ALTER TABLE training_ai.events ENABLE ROW LEVEL SECURITY;
ALTER TABLE training_ai.attestations ENABLE ROW LEVEL SECURITY;
ALTER TABLE training_ai.events OWNER TO astrogateway_schema_owner;
ALTER TABLE training_ai.attestations OWNER TO astrogateway_schema_owner;
REVOKE ALL ON ALL TABLES IN SCHEMA training_ai FROM PUBLIC,anon,authenticated;
GRANT SELECT ON training_ai.events,training_ai.attestations TO authenticated;
GRANT INSERT(user_id,event_id,mission_session_id,elapsed_s,payload) ON training_ai.events TO authenticated;
CREATE POLICY own_events ON training_ai.events FOR SELECT TO authenticated USING(user_id=(SELECT auth.uid()) AND (SELECT training.session_allowed()));
CREATE POLICY append_own_events ON training_ai.events FOR INSERT TO authenticated WITH CHECK(
 user_id=(SELECT auth.uid()) AND (SELECT training.session_allowed()) AND EXISTS(
 SELECT 1 FROM training.mission_sessions m WHERE m.id=mission_session_id AND m.user_id=auth.uid() AND m.auth_session_id=training.claim_session_id() AND m.expires_at>now() AND m.status='initialized'));
CREATE POLICY own_attestations ON training_ai.attestations FOR SELECT TO authenticated USING(user_id=(SELECT auth.uid()) AND (SELECT training.session_allowed()));
-- No authenticated INSERT/UPDATE/DELETE to attestations. No runtime signing/admin credential.
CREATE FUNCTION training_ai.valid_event(e jsonb) RETURNS boolean LANGUAGE plpgsql IMMUTABLE SECURITY INVOKER SET search_path='' AS $$
DECLARE fields text[];k text;n numeric;
BEGIN
 fields:=ARRAY['event_id','elapsed_s','kind'];
 CASE e->>'kind'
 WHEN 'movement' THEN fields:=fields||ARRAY['x_m','z_m','speed_mps'];
 WHEN 'reaction' THEN fields:=fields||ARRAY['reaction_s'];
 WHEN 'resource' THEN fields:=fields||ARRAY['oxygen_used_l','water_used_l','power_used_wh'];
 WHEN 'equipment' THEN fields:=fields||ARRAY['equipment','correct'];
 WHEN 'emergency' THEN fields:=fields||ARRAY['incident','resolved','response_s'];
 WHEN 'mistake' THEN fields:=fields||ARRAY['category','retries'];
 WHEN 'completion' THEN fields:=fields||ARRAY['completed'];
 ELSE RETURN false;
 END CASE;
 IF jsonb_typeof(e)<>'object' OR NOT(e ?& fields) OR EXISTS(SELECT 1 FROM jsonb_object_keys(e) key WHERE NOT(key=ANY(fields))) THEN RETURN false; END IF;
 PERFORM (e->>'event_id')::uuid;
 IF jsonb_typeof(e->'elapsed_s')<>'number' OR (e->>'elapsed_s')::numeric NOT BETWEEN 0 AND 7200 THEN RETURN false; END IF;
 FOREACH k IN ARRAY ARRAY['x_m','z_m','speed_mps','reaction_s','oxygen_used_l','water_used_l','power_used_wh','response_s','retries'] LOOP
 IF e ? k THEN
  IF jsonb_typeof(e->k)<>'number' THEN RETURN false; END IF;n:=(e->>k)::numeric;
  IF k IN ('x_m','z_m') AND n NOT BETWEEN -125 AND 125 THEN RETURN false; END IF;
  IF k='speed_mps' AND n NOT BETWEEN 0 AND 4 THEN RETURN false; END IF;
  IF k='reaction_s' AND n NOT BETWEEN .1 AND 60 THEN RETURN false; END IF;
  IF k='response_s' AND n NOT BETWEEN .1 AND 600 THEN RETURN false; END IF;
  IF k='oxygen_used_l' AND n NOT BETWEEN 0 AND 1000 THEN RETURN false; END IF;
  IF k='water_used_l' AND n NOT BETWEEN 0 AND 20 THEN RETURN false; END IF;
  IF k='power_used_wh' AND n NOT BETWEEN 0 AND 5000 THEN RETURN false; END IF;
  IF k='retries' AND (n NOT BETWEEN 0 AND 5 OR n<>trunc(n)) THEN RETURN false; END IF;
 END IF;
 END LOOP;
 FOREACH k IN ARRAY ARRAY['correct','resolved','completed'] LOOP IF e ? k AND jsonb_typeof(e->k)<>'boolean' THEN RETURN false; END IF; END LOOP;
 IF e ? 'equipment' AND e->>'equipment' NOT IN ('airlock','solar_array','rover','water_recycler','sampler') THEN RETURN false; END IF;
 IF e ? 'incident' AND e->>'incident' NOT IN ('dust','power_loss','oxygen_leak','rover_fault') THEN RETURN false; END IF;
 IF e ? 'category' AND e->>'category' NOT IN ('navigation','survival','decision_making','equipment','emergency') THEN RETURN false; END IF;
 RETURN true;
EXCEPTION WHEN invalid_text_representation OR numeric_value_out_of_range THEN RETURN false;
END $$;
REVOKE ALL ON FUNCTION training_ai.valid_event(jsonb) FROM PUBLIC,anon,authenticated;
GRANT EXECUTE ON FUNCTION training_ai.valid_event(jsonb) TO authenticated;
ALTER FUNCTION training_ai.valid_event(jsonb) OWNER TO astrogateway_schema_owner;
ALTER TABLE training_ai.events ADD CHECK(training_ai.valid_event(payload));
CREATE FUNCTION mission_api.ai_ingest(p_mission uuid,p_events jsonb) RETURNS jsonb LANGUAGE plpgsql SECURITY INVOKER SET search_path='' AS $$
DECLARE m training.mission_sessions;e jsonb;last_t numeric;new_count integer:=0;replayed integer:=0;old jsonb;old_mission uuid;completed boolean;
BEGIN
 PERFORM training.assert_identity();
 IF NOT training.session_allowed() THEN RAISE SQLSTATE 'PT401' USING MESSAGE='Session revoked'; END IF;
 SELECT * INTO m FROM training.mission_sessions WHERE id=p_mission AND user_id=auth.uid() AND auth_session_id=training.claim_session_id() FOR UPDATE;
 IF NOT FOUND THEN RAISE SQLSTATE 'PT404' USING MESSAGE='Mission unavailable'; END IF;
 IF m.expires_at<=now() OR m.status<>'initialized' THEN RAISE SQLSTATE 'PT409' USING MESSAGE='Telemetry lease closed'; END IF;
 IF jsonb_typeof(p_events)<>'array' OR jsonb_array_length(p_events) NOT BETWEEN 1 AND 128 OR octet_length(p_events::text)>131072 THEN RAISE SQLSTATE 'PT422' USING MESSAGE='Invalid event batch'; END IF;
 -- Atomic whole-batch replay is allowed; altered or partial replay conflicts.
 FOR e IN SELECT value FROM jsonb_array_elements(p_events) LOOP
  SELECT payload,mission_session_id INTO old,old_mission FROM training_ai.events WHERE user_id=auth.uid() AND event_id=(e->>'event_id')::uuid;
  IF FOUND THEN
   IF old<>e OR old_mission<>p_mission THEN RAISE SQLSTATE 'PT409' USING MESSAGE='Changed replay'; END IF;
   replayed:=replayed+1;
  END IF;
 END LOOP;
 IF replayed=jsonb_array_length(p_events) THEN RETURN jsonb_build_object('accepted',0,'replayed',replayed,'provenance','client_reported'); END IF;
 IF replayed>0 THEN RAISE SQLSTATE 'PT409' USING MESSAGE='Partial replay'; END IF;
 IF (SELECT count(*) FROM training_ai.events WHERE user_id=auth.uid())+jsonb_array_length(p_events)>2048 THEN RAISE SQLSTATE 'PT409' USING MESSAGE='Local telemetry capacity'; END IF;
 SELECT max(elapsed_s),bool_or(payload->>'kind'='completion') INTO last_t,completed FROM training_ai.events WHERE mission_session_id=p_mission;
 IF completed THEN RAISE SQLSTATE 'PT409' USING MESSAGE='Telemetry closed'; END IF;
 FOR e IN SELECT value FROM jsonb_array_elements(p_events) LOOP
  IF jsonb_typeof(e->'elapsed_s')<>'number' OR e->>'event_id' IS NULL OR e->>'kind' IS NULL OR (last_t IS NOT NULL AND (e->>'elapsed_s')::numeric<=last_t) THEN RAISE SQLSTATE 'PT422' USING MESSAGE='Invalid event order'; END IF;
  IF completed THEN RAISE SQLSTATE 'PT422' USING MESSAGE='Completion must be last'; END IF;
  INSERT INTO training_ai.events(user_id,event_id,mission_session_id,elapsed_s,payload) VALUES(auth.uid(),(e->>'event_id')::uuid,p_mission,(e->>'elapsed_s')::numeric,e);
  last_t:=(e->>'elapsed_s')::numeric;completed:=e->>'kind'='completion';new_count:=new_count+1;
 END LOOP;
 RETURN jsonb_build_object('accepted',new_count,'replayed',0,'provenance','client_reported');
END $$;
CREATE FUNCTION mission_api.ai_snapshot() RETURNS jsonb LANGUAGE plpgsql STABLE SECURITY INVOKER SET search_path='' AS $$
BEGIN
 PERFORM training.assert_identity();
 IF NOT training.session_allowed() THEN RAISE SQLSTATE 'PT401' USING MESSAGE='Session revoked'; END IF;
 RETURN jsonb_build_object(
 'missions',COALESCE((SELECT jsonb_agg(jsonb_build_object('id',id::text) ORDER BY created_at,id) FROM training.mission_sessions WHERE user_id=auth.uid()),'[]'::jsonb),
 'events',COALESCE((SELECT jsonb_agg(jsonb_build_object('mission_session_id',mission_session_id::text,'payload',payload) ORDER BY mission_session_id,elapsed_s) FROM training_ai.events WHERE user_id=auth.uid()),'[]'::jsonb),
 'attestations',COALESCE((SELECT jsonb_agg(jsonb_build_object('mission_session_id',mission_session_id::text,'success',success,'aggregate',aggregate,'difficulty',difficulty,'skill_scores',skill_scores,'cycle',cycle,'training_day',training_day)) FROM training_ai.attestations WHERE user_id=auth.uid()),'[]'::jsonb));
END $$;
REVOKE ALL ON FUNCTION mission_api.ai_ingest(uuid,jsonb),mission_api.ai_snapshot() FROM PUBLIC,anon,authenticated;
GRANT EXECUTE ON FUNCTION mission_api.ai_ingest(uuid,jsonb),mission_api.ai_snapshot() TO authenticated;
ALTER FUNCTION mission_api.ai_ingest(uuid,jsonb) OWNER TO astrogateway_schema_owner;
ALTER FUNCTION mission_api.ai_snapshot() OWNER TO astrogateway_schema_owner;
COMMIT;
-- Expose ONLY existing mission_api after separate approval; training_ai stays private.
-- No schema cache mutation in this candidate. Local test harness NOTIFY is disposable only.
