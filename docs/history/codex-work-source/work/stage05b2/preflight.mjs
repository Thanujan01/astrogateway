import{PGlite}from '../stage05b1-hardening/pg17/node_modules/@electric-sql/pglite/dist/index.js';import{readFileSync}from'node:fs';let d=new PGlite();try{
await d.exec(`CREATE ROLE anon NOLOGIN;CREATE ROLE authenticated NOLOGIN NOSUPERUSER NOBYPASSRLS NOCREATEDB NOCREATEROLE;CREATE SCHEMA auth;
CREATE TABLE auth.users(id uuid PRIMARY KEY,created_at timestamptz NOT NULL DEFAULT now(),raw_user_meta_data jsonb,email_confirmed_at timestamptz,is_anonymous boolean);
CREATE FUNCTION auth.jwt() RETURNS jsonb LANGUAGE sql STABLE AS $$SELECT NULLIF(current_setting('request.jwt.claims',true),'')::jsonb$$;
CREATE FUNCTION auth.uid() RETURNS uuid LANGUAGE sql STABLE AS $$SELECT NULLIF(auth.jwt()->>'sub','')::uuid$$;
GRANT USAGE ON SCHEMA auth TO authenticated;GRANT EXECUTE ON FUNCTION auth.jwt(),auth.uid() TO authenticated;`);
await d.exec(readFileSync('outputs/Stage05B2/SQL/001_verified_jwt_training.sql','utf8'));console.log('Candidate SQL parses and installs in local PG17 fixture.');
}finally{await d.close();}
