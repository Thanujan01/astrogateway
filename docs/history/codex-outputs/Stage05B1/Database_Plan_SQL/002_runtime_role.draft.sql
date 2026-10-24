-- DRAFT ONLY. Not executed. Approved target: jvkwofmxtxjhdwtwdboz / astrogateway-test.
-- Run only after 001 and separately approved PUBLIC ACL isolation have passed rehearsal.
-- Deliberately refuses execution while public schema USAGE remains granted to PUBLIC.
-- It creates no password: set one later via an approved hidden password prompt/secret manager.
BEGIN;
DO $$
BEGIN
  IF EXISTS(SELECT 1 FROM pg_namespace n
    CROSS JOIN LATERAL aclexplode(COALESCE(n.nspacl,acldefault('n',n.nspowner))) a
    WHERE n.nspname NOT IN ('training','pg_catalog','information_schema')
      AND n.nspname NOT LIKE 'pg_%' AND a.grantee=0
      AND a.privilege_type IN ('USAGE','CREATE')) THEN
    RAISE EXCEPTION 'Unresolved PUBLIC schema access. Stop and review provider-compatible ACL isolation.';
  END IF;
  IF EXISTS(SELECT 1 FROM pg_database d
    CROSS JOIN LATERAL aclexplode(COALESCE(d.datacl,acldefault('d',d.datdba))) a
    WHERE d.datname=current_database() AND a.grantee=0 AND a.privilege_type='TEMPORARY') THEN
    RAISE EXCEPTION 'Unresolved PUBLIC database TEMPORARY access; approve a provider-compatible least-privilege plan.';
  END IF;
  IF NOT EXISTS(SELECT 1 FROM pg_roles WHERE rolname='astrogateway_api'
    AND NOT rolcanlogin AND NOT rolsuper AND NOT rolbypassrls
    AND NOT rolcreaterole AND NOT rolcreatedb AND NOT rolreplication) THEN
    RAISE EXCEPTION 'Expected restricted API role missing or unsafe.';
  END IF;
END;
$$;
CREATE ROLE astrogateway_runtime LOGIN NOINHERIT NOSUPERUSER
  NOCREATEDB NOCREATEROLE NOREPLICATION NOBYPASSRLS CONNECTION LIMIT 5;
GRANT CONNECT ON DATABASE postgres TO astrogateway_runtime;
GRANT astrogateway_api TO astrogateway_runtime WITH ADMIN FALSE;
GRANT astrogateway_api TO astrogateway_runtime WITH INHERIT FALSE;
GRANT astrogateway_api TO astrogateway_runtime WITH SET TRUE;
ALTER ROLE astrogateway_runtime IN DATABASE postgres SET search_path TO pg_catalog,training;
COMMIT;
-- No membership in postgres, service_role, supabase_auth_admin, owners or platform roles.
-- No grants on auth/storage/vault/public, no DDL, no admin option, no broad table grants.
-- NOINHERIT does not remove PUBLIC grants. Guard above is a stop condition, not ACL repair.
