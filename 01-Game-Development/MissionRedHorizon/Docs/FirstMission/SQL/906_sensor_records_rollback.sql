-- Rollback for 006 (candidate, not executed). Removes only objects 006 created.
BEGIN;
DROP FUNCTION IF EXISTS mission_api.submit_sensor_records(uuid,jsonb);
DROP TABLE IF EXISTS training.sensor_records;
COMMIT;
