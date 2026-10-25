-- Isolated Edge broker ONLY. Never expose service-role credentials to FastAPI or clients.
-- public schema RPC intentionally executable only by service_role, never anon/authenticated.
BEGIN;
CREATE TABLE training.login_attempt_windows (
 key_hash text PRIMARY KEY CHECK (key_hash ~ '^[a-f0-9]{64}$'), window_started timestamptz NOT NULL, attempts integer NOT NULL
);
ALTER TABLE training.login_attempt_windows ENABLE ROW LEVEL SECURITY;
REVOKE ALL ON training.login_attempt_windows FROM PUBLIC,anon,authenticated,service_role;
CREATE FUNCTION public.broker_login_identity(p_identifier text,p_key_hash text) RETURNS jsonb
LANGUAGE plpgsql SECURITY DEFINER SET search_path='' AS $$
DECLARE n integer; email_address text;
BEGIN
 IF p_identifier IS NULL OR char_length(p_identifier) NOT BETWEEN 3 AND 254 OR p_key_hash IS NULL OR p_key_hash !~ '^[a-f0-9]{64}$' THEN RETURN NULL; END IF;
 INSERT INTO training.login_attempt_windows AS w VALUES(p_key_hash,clock_timestamp(),1)
 ON CONFLICT(key_hash) DO UPDATE SET
 attempts=CASE WHEN w.window_started < clock_timestamp()-interval '10 minutes' THEN 1 ELSE w.attempts+1 END,
 window_started=CASE WHEN w.window_started < clock_timestamp()-interval '10 minutes' THEN clock_timestamp() ELSE w.window_started END
 RETURNING attempts INTO n;
 IF n>6 THEN RETURN jsonb_build_object('limited',true); END IF;
 -- Work bound by request, and rows older than a day contain no identifiers or passwords.
 DELETE FROM training.login_attempt_windows WHERE key_hash IN
 (SELECT key_hash FROM training.login_attempt_windows WHERE window_started < clock_timestamp()-interval '1 day' LIMIT 100);
 IF position('@' in p_identifier)>0 THEN
  SELECT u.email INTO email_address FROM auth.users u WHERE lower(u.email)=lower(p_identifier) LIMIT 1;
 ELSE
  SELECT u.email INTO email_address FROM training.profiles p JOIN auth.users u ON u.id=p.user_id WHERE p.username=lower(p_identifier);
 END IF;
 RETURN jsonb_build_object('email',email_address);
END $$;
REVOKE ALL ON FUNCTION public.broker_login_identity(text,text) FROM PUBLIC,anon,authenticated;
GRANT EXECUTE ON FUNCTION public.broker_login_identity(text,text) TO service_role;
COMMIT;
