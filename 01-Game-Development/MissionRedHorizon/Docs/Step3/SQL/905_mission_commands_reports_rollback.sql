-- Rollback for 005 (local rehearsal / operator review only). Drops only the objects 005 created.
BEGIN;
DROP FUNCTION mission_api.skill_evidence();
DROP FUNCTION mission_api.mission_report(uuid);
DROP FUNCTION mission_api.mission_state(uuid);
DROP FUNCTION mission_api.submit_mission_command(uuid,uuid,integer,text,jsonb);
DROP FUNCTION training.command_evidence(uuid);
DROP FUNCTION training.command_payload_valid(text,jsonb);
DROP TABLE training.mission_commands;
COMMIT;
