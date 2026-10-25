-- LOCAL LAB ONLY. Minimal Storage catalog for SQL ownership-policy tests, not Storage HTTP validation.
CREATE ROLE service_role NOLOGIN;
ALTER TABLE auth.users ADD COLUMN email text;
CREATE SCHEMA storage;
CREATE TABLE storage.buckets(id text PRIMARY KEY,name text,public boolean,file_size_limit bigint,allowed_mime_types text[]);
CREATE TABLE storage.objects(bucket_id text REFERENCES storage.buckets(id),name text,PRIMARY KEY(bucket_id,name));
ALTER TABLE storage.objects ENABLE ROW LEVEL SECURITY;
GRANT USAGE ON SCHEMA storage TO authenticated;
GRANT SELECT,INSERT,UPDATE,DELETE ON storage.objects TO authenticated;
