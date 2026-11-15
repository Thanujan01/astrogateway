-- Hosted Storage candidate, requires 007. Review existing policies before applying.
-- Private bucket; only the confirmed, non-revoked owner can access one fixed PNG object.
BEGIN;
INSERT INTO storage.buckets(id,name,public,file_size_limit,allowed_mime_types)
VALUES ('profile-photos','profile-photos',false,2097152,ARRAY['image/png']);
CREATE POLICY avatar_owner_read ON storage.objects FOR SELECT TO authenticated
USING (bucket_id='profile-photos' AND name=auth.uid()::text||'/avatar.png' AND training.session_allowed());
CREATE POLICY avatar_owner_insert ON storage.objects FOR INSERT TO authenticated
WITH CHECK (bucket_id='profile-photos' AND name=auth.uid()::text||'/avatar.png' AND training.session_allowed());
CREATE POLICY avatar_owner_update ON storage.objects FOR UPDATE TO authenticated
USING (bucket_id='profile-photos' AND name=auth.uid()::text||'/avatar.png' AND training.session_allowed())
WITH CHECK (bucket_id='profile-photos' AND name=auth.uid()::text||'/avatar.png' AND training.session_allowed());
CREATE POLICY avatar_owner_delete ON storage.objects FOR DELETE TO authenticated
USING (bucket_id='profile-photos' AND name=auth.uid()::text||'/avatar.png' AND training.session_allowed());
COMMIT;
