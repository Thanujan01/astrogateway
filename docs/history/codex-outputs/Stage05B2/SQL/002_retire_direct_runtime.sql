-- Idempotent quarantine for any legacy raw-DB application roles. LOCAL ONLY until approved.
BEGIN;
DO $$
DECLARE r text; member_rec record; c record;
BEGIN
 -- Stop on unexpected ownership/privileged role collisions before changing anything.
 IF EXISTS(SELECT 1 FROM pg_roles WHERE rolname IN ('astrogateway_runtime','astrogateway_api') AND (rolsuper OR rolbypassrls OR rolcreaterole OR rolcreatedb OR rolreplication)) THEN
  RAISE EXCEPTION 'Unexpected privileged legacy role; review before quarantine';
 END IF;
 FOREACH r IN ARRAY ARRAY['astrogateway_runtime','astrogateway_api'] LOOP
  IF EXISTS(SELECT 1 FROM pg_roles WHERE rolname=r) THEN
   EXECUTE format('ALTER ROLE %I NOLOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE NOREPLICATION NOBYPASSRLS NOINHERIT CONNECTION LIMIT 0',r);
   FOR member_rec IN SELECT parent.rolname FROM pg_auth_members m JOIN pg_roles parent ON parent.oid=m.roleid JOIN pg_roles child ON child.oid=m.member WHERE child.rolname=r
   LOOP EXECUTE format('REVOKE %I FROM %I',member_rec.rolname,r); END LOOP;
   EXECUTE format('REVOKE ALL ON SCHEMA training,mission_api FROM %I',r);
   EXECUTE format('REVOKE ALL ON ALL TABLES IN SCHEMA training FROM %I',r);
   EXECUTE format('REVOKE ALL ON ALL FUNCTIONS IN SCHEMA training FROM %I',r);
   EXECUTE format('REVOKE ALL ON ALL FUNCTIONS IN SCHEMA mission_api FROM %I',r);
   FOR c IN SELECT n.nspname,t.relname,a.attname FROM pg_attribute a JOIN pg_class t ON t.oid=a.attrelid JOIN pg_namespace n ON n.oid=t.relnamespace
    WHERE n.nspname='training' AND t.relkind IN ('r','p') AND a.attnum>0 AND NOT a.attisdropped LOOP
     EXECUTE format('REVOKE ALL PRIVILEGES (%I) ON TABLE %I.%I FROM %I',c.attname,c.nspname,c.relname,r);
   END LOOP;
   IF EXISTS(SELECT 1 FROM pg_class t JOIN pg_namespace n ON n.oid=t.relnamespace WHERE n.nspname IN ('training','mission_api') AND pg_get_userbyid(t.relowner)=r)
    OR EXISTS(SELECT 1 FROM pg_proc p JOIN pg_namespace n ON n.oid=p.pronamespace WHERE n.nspname IN ('training','mission_api') AND pg_get_userbyid(p.proowner)=r)
    OR EXISTS(SELECT 1 FROM pg_namespace WHERE nspname IN ('training','mission_api') AND pg_get_userbyid(nspowner)=r) THEN
      RAISE EXCEPTION 'Legacy role owns protected objects; stop for ownership review';
   END IF;
  END IF;
 END LOOP;
 -- An existing session can retain an already SET role after membership revocation.
 -- Kill only the retired roles' sessions (initial login identity), never provider/admin sessions.
 PERFORM pg_terminate_backend(pid) FROM pg_stat_activity
 WHERE usename IN ('astrogateway_runtime','astrogateway_api') AND pid<>pg_backend_pid();
END $$;
COMMIT;
-- No DROP OWNED/CASCADE. Session termination is an explicit reviewed part of future quarantine approval.
-- NOLOGIN alone does not terminate sessions; the exact-role termination above closes stale SET ROLE sessions.
