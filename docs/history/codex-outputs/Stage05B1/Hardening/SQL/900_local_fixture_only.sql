-- NEVER RUN ON SUPABASE. Auth/service roles are minimal disposable fixtures only.
CREATE ROLE anon NOLOGIN;
CREATE ROLE authenticated NOLOGIN;
CREATE ROLE supabase_auth_admin LOGIN CREATEROLE;
CREATE ROLE service_role NOLOGIN BYPASSRLS;
CREATE SCHEMA auth;
CREATE TABLE auth.users(id uuid PRIMARY KEY,created_at timestamptz NOT NULL DEFAULT now(),raw_user_meta_data jsonb);
GRANT USAGE ON SCHEMA auth TO supabase_auth_admin;
GRANT SELECT,INSERT ON auth.users TO supabase_auth_admin;
-- Representative unrelated PUBLIC usage plus a security-definer callable surface.
CREATE SCHEMA storage; GRANT USAGE ON SCHEMA storage TO PUBLIC;
CREATE TABLE public.unrelated(secret text); INSERT INTO public.unrelated VALUES('disposable fixture');
CREATE FUNCTION public.fixture_read() RETURNS text LANGUAGE sql SECURITY DEFINER SET search_path='' AS $$SELECT secret FROM public.unrelated LIMIT 1$$;
CREATE SCHEMA pgcustom; GRANT USAGE ON SCHEMA pgcustom TO PUBLIC; CREATE TABLE pgcustom.unrelated(id int);
