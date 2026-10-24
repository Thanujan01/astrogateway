-- CANDIDATE ONLY. Local rehearsal approved; remote execution NOT approved.
-- Converts existing PUBLIC schema access/TEMP to explicit grants for existing roles.
-- Must inventory every preserved role/default ACL and approve provider compatibility first.
-- New provider roles need explicit provisioning; no universal future-role grant.
BEGIN;
DO $$
DECLARE s record; r record; privileges text; dbname text:=current_database();
BEGIN
 IF current_user IN ('astrogateway_api','astrogateway_runtime') THEN RAISE EXCEPTION 'Administrator required'; END IF;
 FOR s IN SELECT n.oid,n.nspname,n.nspowner FROM pg_namespace n
  WHERE n.nspname NOT IN ('training','pg_catalog','information_schema') AND left(n.nspname,3) <> 'pg_'
 LOOP
  SELECT string_agg(a.privilege_type,', ') INTO privileges
   FROM aclexplode(COALESCE((SELECT nspacl FROM pg_namespace WHERE oid=s.oid),acldefault('n',s.nspowner))) a
   WHERE a.grantee=0;
  IF privileges IS NOT NULL THEN
   FOR r IN SELECT rolname FROM pg_roles WHERE rolname NOT IN ('astrogateway_api','astrogateway_runtime') AND left(rolname,3) <> 'pg_'
   LOOP EXECUTE format('GRANT %s ON SCHEMA %I TO %I',privileges,s.nspname,r.rolname); END LOOP;
   EXECUTE format('REVOKE ALL ON SCHEMA %I FROM PUBLIC',s.nspname);
  END IF;
 END LOOP;
 IF EXISTS(SELECT 1 FROM pg_database d CROSS JOIN LATERAL aclexplode(COALESCE(d.datacl,acldefault('d',d.datdba))) a
  WHERE d.datname=dbname AND a.grantee=0 AND a.privilege_type='TEMPORARY') THEN
  FOR r IN SELECT rolname FROM pg_roles WHERE rolname NOT IN ('astrogateway_api','astrogateway_runtime') AND left(rolname,3) <> 'pg_'
  LOOP EXECUTE format('GRANT TEMPORARY ON DATABASE %I TO %I',dbname,r.rolname); END LOOP;
  EXECUTE format('REVOKE TEMPORARY ON DATABASE %I FROM PUBLIC',dbname);
 END IF;
END $$;
-- Function EXECUTE defaults are global; per-schema REVOKE cannot remove the built-in PUBLIC default.
-- Applies only to current migration owner, affects its future functions in ALL schemas.
-- Preserve existing named-role execution semantics while excluding application roles.
ALTER DEFAULT PRIVILEGES REVOKE EXECUTE ON FUNCTIONS FROM PUBLIC;
DO $$ DECLARE r record; BEGIN
 FOR r IN SELECT rolname FROM pg_roles WHERE rolname NOT IN ('astrogateway_api','astrogateway_runtime') AND left(rolname,3) <> 'pg_'
 LOOP EXECUTE format('ALTER DEFAULT PRIVILEGES GRANT EXECUTE ON FUNCTIONS TO %I',r.rolname); END LOOP;
END $$;
REVOKE ALL ON ALL TABLES IN SCHEMA training FROM PUBLIC;
REVOKE ALL ON ALL FUNCTIONS IN SCHEMA training FROM PUBLIC;
COMMIT;
