# Database migration security review — 2026-10-09

**Review conclusion: remote execution is NOT ready.** The baseline supports a trusted FastAPI broker with RLS as defence against query/ownership mistakes. It does not prevent a holder of the runtime DB credential from supplying arbitrary user/session context. Strict exclusion from unrelated schemas is also not established because PUBLIC privileges apply to every role. Resolve those scope decisions and runtime TLS handling before authorizing execution.

Target: approved non-production `astrogateway-test`, reference `jvkwofmxtxjhdwtwdboz`. No migrations, roles or accounts were created. Android/UI/Unity/Mars/React source unchanged.

## Evidence and compatibility

Read the latest readiness report, original delivered SQL and current FastAPI source. Verified remote metadata inside an explicitly read-only transaction using the existing private admin credential and verified TLS.

`Review_Evidence/catalogue.json` records PostgreSQL 17.11; auth.users and gen_random_uuid() present; required id UUID, created_at timestamptz and raw_user_meta_data JSONB columns present. anon/authenticated exist as non-login/non-BYPASSRLS roles. supabase_auth_admin exists and can create roles: runtime must never become its member. No training objects, API/runtime role or existing non-internal auth.users trigger was observed. This is a snapshot; repeat collision checks just before execution.

PUBLIC has USAGE on public schema. No PUBLIC table grants were found in the inspected auth/storage/public/vault schemas. A PUBLIC-executable security-definer function `public.rls_auto_enable` belongs to postgres; its return type is event_trigger, so it is not an ordinary directly callable RPC. Do not label its existence an exploitable RPC; provider functions must not be revoked blindly. The ACL scan is not an exhaustive effective-privilege proof for a role that does not exist yet. Check inherited roles, defaults, every relevant schema/routine and column/sequence/object privilege after provisioning in a disposable environment.

Prior local evidence: 21 PostgreSQL WASM migration/RLS checks and 20 mocked API contracts passed in Stage05B. They used Auth fixtures and synthetic verification, not live users or this remote installation. No new dry-run migration or live RLS/user tests ran in this review.

## All migration objects

Original file: `outputs/Stage05B/database/001_identity_and_mission_sessions.sql`. SHA256: `269a983fe659e98f6083f3a8e46197ebe166d32e77eecc4a520539f1c686b59d`. Project copy matches; neither was edited.

| Object | Review |
|---|---|
| astrogateway_api | NOLOGIN, NOSUPERUSER, NOCREATEDB, NOCREATEROLE, NOINHERIT, NOBYPASSRLS; explicit NOREPLICATION would improve clarity; default is false. Not an owner |
| training schema | Private; PUBLIC/anon/authenticated revoked; API USAGE only, no CREATE |
| profiles | auth.users UUID FK with deletion cascade; bounded name, fixed trainee category, level0 default/range0–10, provider creation timestamp; no API profile writes |
| auth_session_revocations | Auth-session UUID primary key, profile owner FK, revoke/expiry times; API may insert current session and read its owner's rows; no DELETE/UPDATE |
| mission_sessions | Owner/profile FK, Auth-session/launch UUID, unique launch, fixed POC mission, constrained lifecycle, server times and 30-minute lease; API insert columns and update status/completed_at only |
| mission_sessions_user_created_idx | Owner/date lookup; revocation PK supports session check; no extra unique identity provider |
| RLS ENABLE on all three tables | Default denial outside policies; owners/BYPASSRLS still bypass. FORCE RLS is absent intentionally; runtime must not own tables. Admin-owned trigger backfill can insert |
| request_user_id(), request_auth_session_id() | SQL STABLE/invoker, empty search_path, read transaction-local custom settings; no JWT verification in PostgreSQL |
| session_allowed() | Invoker, empty search_path; non-null context and no matching unexpired revocation. Revocation read policy does not call this helper, avoiding recursive policy evaluation |
| create_auth_profile() | SECURITY DEFINER, empty search_path, schema-qualified insert; reads only bounded display metadata, never training level/role. Owned by migration executor by default; high-trust privilege surface |
| astrogateway_auth_user_profile trigger | AFTER INSERT auth.users; must be installed by authorized administrator. Function failures can abort signup. Existing Auth users backfilled once |

Creation dependencies: role/schema first; profiles before FK dependents; helper functions before dependent policies; trigger function before trigger; backfill last. gen_random_uuid is available. Current aggregate check found zero Auth users and zero null creation timestamps. Repeat this before execution; validate existing Auth rows' created_at because destination timestamp is NOT NULL; source type alone does not prove non-null data. No Auth password update or new account appears in SQL.

Definer hardening: no runtime/public execute permission; PostgreSQL invokes trigger function through the trigger. Empty search_path removes object-shadowing risk. Owning it as postgres is broader than necessary; a separately reviewed NOLOGIN profile-writer owner with narrowly scoped INSERT and appropriate RLS/ownership treatment could reduce blast radius, but is not implemented and must not be improvised. Never grant its ownership or trigger administration to runtime. Do not expose training in Data API.

## Grants and RLS policy audit

API SELECT: all three tables. API INSERT: only mission owner/Auth-session/launch/mission columns and revocation Auth-session/owner/expiry columns. API UPDATE: only mission status/completed_at. No profile writes, DELETE, TRUNCATE, ALTER, CREATE, REFERENCES or TRIGGER privilege granted. Table defaults/constraints bind fields API cannot supply.

| Policy | Enforced condition / limit |
|---|---|
| revocation_read_own | Owner matches custom canonical user setting; can read own revocations |
| revocation_insert_current | Owner and Auth session match supplied context. Expiry is supplied by trusted verifier's JWT expiry; database cannot independently validate JWT |
| profile_read_own | Own profile and allowed session only |
| mission_read_own | Own missions and allowed session; reads can span the owner's Auth sessions, completion cannot |
| mission_insert_current | Canonical owner/current Auth session, allowed session, initialized state, null completion and future lease; limited columns prevent changing defaults/expiry |
| mission_complete_current | Same owner/current Auth session, allowed session; initialized/unexpired or already completed. WITH CHECK requires completed timestamp no more than five seconds ahead. Column privileges prevent identity/lease edits; already-completed rows support SELECT FOR UPDATE/idempotent receipt |

No assessment score accepted; only integration-check receipt. Database policy permits a same-owner runtime caller to rewrite completion timestamps within allowed bounds; API idempotency preserves receipt time. This is another distinction between SQL authority and the narrower API contract. Unique launch collision yields conflict, not an automatic retry receipt for initialization.

## Identity and impersonation trust boundary

FastAPI bounds bearer size, permits known algorithms, asks configured Supabase /auth/v1/user to authenticate it, then validates issuer/audience/authenticated role/expiry/sub against returned UUID/session UUID and verified email. Decoding alone does not authenticate. Endpoint bodies reject extra fields and never accept a caller-selected owner ID.

Each DB operation uses a transaction, SET LOCAL ROLE astrogateway_api and parameterized set_config(user_id/session_id, true). Repository queries are fixed/parameterized. Local settings and role reset on commit/rollback; verify pooled reuse in rehearsal. Native passwords/refresh tokens never reach DB, Unity or application logs. Unity receives only bounded mission identifiers via the private host bridge.

**RLS does not cryptographically bind these settings to a JWT.** Runtime can run set_config for user B while possessing account A's identity, or impersonate a user without any JWT if it holds arbitrary SQL access. Thus RLS prevents forged owner fields from ordinary HTTP users only while the trusted verifier/broker and runtime credential are uncompromised. A role credential or SQL-injection compromise defeats that identity boundary. Do not claim absolute DB-level impersonation prevention.

Decision required: accept/document this trusted-backend threat model and prohibit arbitrary SQL/SQL injection, tightly protect the credential and ingress; or explicitly approve redesign to independently validate identity in the database/credential broker. Narrow RPCs alone do not solve this if the same untrusted caller can assert arbitrary user IDs. A stronger design is separate work requiring review and approval, not silently added here.

## Restricted runtime design and unrelated-schema limit

Planned login: astrogateway_runtime with LOGIN, NOINHERIT, NOSUPERUSER, NOCREATEDB, NOCREATEROLE, NOREPLICATION, NOBYPASSRLS, connection limit5. Grant CONNECT only plus membership in API role: ADMIN false, INHERIT false, SET true on PG17. No owner/Admin/Auth/service_role/provider membership, grant option, schema CREATE, Auth table privileges or signing secret. Password set through a hidden approved channel and stored server-only; postgres URI remains only in admin.local.env.

NOINHERIT and REVOKE FROM runtime **do not deny PUBLIC grants**. Observed public-schema USAGE means current baseline cannot meet literal “no unrelated schema access.” Remote metadata confirms PUBLIC database TEMPORARY and CONNECT. Those defaults and callable routines also need effective-ACL inventory. Schema visibility is not the same as access to unrelated data, but that weaker guarantee must not replace the user's requirement silently.

For strict isolation: prepare a separate provider-compatible ACL change that removes PUBLIC grants from unrelated application schemas/routines as needed and explicitly preserves required platform/client grants to a fully reviewed allowlist excluding API/runtime. Audit defaults so future objects do not reintroduce access. This can affect Supabase services; rehearse first, do not execute a blanket global REVOKE in this shared project. Built-in pg_catalog functions/metadata necessary for SQL operations are explicit exceptions, not application/Auth data grants.

Current FastAPI startup checks non-superuser/non-BYPASSRLS, training table ownership and API membership. It does not fully check CREATEROLE/CREATEDB/REPLICATION, privileged memberships, effective PUBLIC access, arbitrary object ownership or exact privilege inventory. Harden that guard before claiming least-privilege startup enforcement. Draft role SQL is fail-closed on unresolved PUBLIC schema access and is not ready to run here.

## Certificate exception and TLS boundaries

Operator-supplied Supabase Root2021 CA has valid dates through 2031 and SHA256 `700723581420dd1ac98fd7e9ac529f0ef210eadcaf87fc868a3ad7d114c2f3b7`. Initial filename corrected by copy; original retained. Trust provenance depends on approved dashboard/operator delivery, not filename alone.

Python's strict X509 profile rejected missing key-usage extension (error92). Prior admin probes removed VERIFY_X509_STRICT only, kept CERT_REQUIRED/check_hostname=true and successfully authenticated under TLS1.3. This relaxes strict certificate-profile checks beyond that single error; it is a scoped compatibility exception, not equivalent to strict-profile compliance. Prefer provider-issued updated compliant CA when available; pin/audit the approved root, expiry and hostname, avoid changing global trust/system SSL policy. Never CERT_NONE, require-only without hostname validation, or fallback to plaintext. Review/approve exception before deploying runtime.

Existing backend passes ssl=True, not the verified explicit CA/compatibility SSLContext used by the admin probe. A DSN alone must not be assumed to reproduce that trust behavior. Runtime TLS setup/verification is an unresolved prerequisite and no backend code was changed during review.

Actual client→pooler TLS1.3/chain/hostname passed. pg_stat_ssl observed backend ssl=false; this is the pooler→DB leg, not client transport. Full-path TLS is NOT established. Obtain provider topology/enforcement assurance or a verified direct-TLS alternative before claiming encrypted every hop. No provider SSL setting was changed.

## Redirect and decision summary

No accessible signed-in Supabase dashboard URL Configuration page was found. `astrogateway://auth/callback` remains **unverified** in the remote allowlist. Public settings API previously confirmed Email Auth enabled/confirmation not automatic; it cannot verify redirects, templates, SMTP or actual verification/recovery delivery.

Remote execution gates: trust-boundary decision; provider-compatible PUBLIC ACL plan; runtime startup/TLS hardening and certificate-exception decision; full-path TLS limitation acknowledged; refreshed catalogue/backfill checks and backups; disposable rehearsal; explicit migration/role approval. No live Auth/RLS account tests passed.

References: [PostgreSQL RLS](https://www.postgresql.org/docs/current/ddl-rowsecurity.html), [PG17 grants/PUBLIC and role options](https://www.postgresql.org/docs/17/sql-grant.html), [Supabase Auth-linked profile triggers](https://supabase.com/docs/guides/auth/managing-user-data), [Supabase security-definer functions](https://supabase.com/docs/guides/database/functions), [Python SSL profile checks](https://docs.python.org/3/library/ssl.html).
