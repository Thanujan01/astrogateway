-- LOCAL NATIVE LAB ONLY. Never execute on Supabase. Minimal Auth roles/tables/JWT helpers.
CREATE ROLE anon NOLOGIN;
CREATE ROLE authenticated NOLOGIN NOINHERIT NOSUPERUSER NOBYPASSRLS NOCREATEDB NOCREATEROLE NOREPLICATION;
CREATE ROLE lab_gateway LOGIN NOINHERIT NOSUPERUSER NOBYPASSRLS NOCREATEDB NOCREATEROLE NOREPLICATION;
GRANT authenticated,anon TO lab_gateway;
CREATE ROLE supabase_auth_admin LOGIN CREATEROLE;
CREATE ROLE astrogateway_api NOLOGIN NOINHERIT NOSUPERUSER NOBYPASSRLS;
CREATE ROLE astrogateway_runtime LOGIN NOINHERIT NOSUPERUSER NOBYPASSRLS NOCREATEDB NOCREATEROLE NOREPLICATION;
GRANT astrogateway_api TO astrogateway_runtime;
CREATE SCHEMA auth;
CREATE TABLE auth.users(id uuid PRIMARY KEY,created_at timestamptz NOT NULL DEFAULT now(),raw_user_meta_data jsonb,email_confirmed_at timestamptz,is_anonymous boolean NOT NULL DEFAULT false);
CREATE FUNCTION auth.jwt() RETURNS jsonb LANGUAGE sql STABLE AS $$SELECT COALESCE(NULLIF(current_setting('request.jwt.claim',true),''),NULLIF(current_setting('request.jwt.claims',true),''))::jsonb$$;
CREATE FUNCTION auth.uid() RETURNS uuid LANGUAGE sql STABLE AS $$SELECT COALESCE(NULLIF(current_setting('request.jwt.claim.sub',true),''),(NULLIF(current_setting('request.jwt.claims',true),'')::jsonb->>'sub'))::uuid$$;
GRANT USAGE ON SCHEMA auth TO authenticated,supabase_auth_admin;
GRANT EXECUTE ON FUNCTION auth.jwt(),auth.uid() TO authenticated;
GRANT INSERT,SELECT ON auth.users TO supabase_auth_admin;
