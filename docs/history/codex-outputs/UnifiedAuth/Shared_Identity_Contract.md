# Shared identity and profile contract

The canonical identity is Supabase Auth `auth.users.id` (UUID) from the approved test project `jvkwofmxtxjhdwtwdboz`. Web and Android use the same public project configuration. Display names are mutable and are not unique login handles. Email/password remain in Supabase Auth; no password column or independent account issuer is introduced.

FastAPI verifies the original bearer with the configured Auth server, checks issuer/audience/role/expiry/confirmed email, then forwards that original JWT to PostgREST. The gateway independently verifies its signature and applies native RLS. A user ID in a request body, header, local UI state or arbitrary PostgreSQL setting is never accepted as authorization proof. The retired direct database runtime role remains NOLOGIN. Valid stolen JWTs still carry their owner's authority until expiry/revocation.


## API

GET `/v1/me` and POST `/v1/me/display-name` require `Authorization: Bearer <original Supabase access JWT>`.

Update body:
```json
{"display_name":"Commander Tamil"}
```

Response shape:
```json
{"user_id":"11111111-1111-4111-8111-111111111111","display_name":"Commander Tamil","trainee_profile":"astronaut_candidate","training_level":0,"created_at":"2026-01-01T00:00:00Z"}
```

The UUID above is a local synthetic example, not a live account. Extra fields—including owner ID, email or training level—are rejected. Statuses: 401 invalid/expired/revoked session; 403 unconfirmed/unauthorized identity; 422 invalid body/name; 502 invalid/foreign profile response; 503 unavailable Auth/data service. Server errors are sanitized. Name edits update the canonical training profile; signup Auth metadata is only a creation-time name hint and is not treated as the current profile source.

## SQL candidate, local only

[004_profile_display_name.sql](/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/UnifiedAuth/SQL/004_profile_display_name.sql) depends on [001_verified_jwt_training.sql](/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B2/SQL/001_verified_jwt_training.sql), not the obsolete Stage05B SQL. It adds an own-row UPDATE policy, a column-only display_name UPDATE grant, and a SECURITY INVOKER RPC without owner-ID arguments. Existing confirmed-email/session/revocation predicates apply. No password/email columns or privilege escalations are added. The RPC owner remains the safe NOLOGIN schema owner, but invoker execution cannot bypass RLS.

Fresh-object collisions abort transactionally; this is not a blind idempotent remote script. Local repeatability means refusal without overwrite, rollback, and successful reapply. [904_profile_display_name_rollback.sql](/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/UnifiedAuth/SQL/904_profile_display_name_rollback.sql) removes edit capability while preserving names/data/Auth users. Rollback does not automatically undo legitimate name changes; use reviewed backups if those must be restored.

Remote deployment would require the existing reviewed Stage05B2/3 prerequisites, approved exact SQL/order/hashes, collision checks, backup/restoration rehearsal and mission_api gateway cache/exposure review. None was performed remotely here.
