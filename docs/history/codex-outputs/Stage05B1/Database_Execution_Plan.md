# Database execution plan — NOT AUTHORIZED / NOT RUN

Target only: `astrogateway-test`, project `jvkwofmxtxjhdwtwdboz`; Session pooler `aws-0-ap-southeast-2.pooler.supabase.com:5432`, database postgres. This document is a conditional plan, not permission to mutate a remote database. Read `Database_Migration_Security_Review.md` first. Current baseline is **not cleared for execution** until the PUBLIC-access/trusted-broker/TLS decisions are resolved.

## Approval and readiness gates

1. Operator acknowledges the canonical-identity trust boundary: native/HTTP users cannot choose owner IDs, but a compromised trusted FastAPI process/runtime DB credential can set custom identity GUCs. If protection from that compromise is required, approve a separately designed identity-verifying broker/DB boundary before this plan proceeds.
2. Approve preparation and disposable rehearsal of provider-compatible effective-privilege hardening. Current PUBLIC public-schema USAGE and DB TEMPORARY cannot be denied merely by REVOKE from runtime/NOINHERIT. Need a complete named-role preservation allowlist and ownership/default-ACL inventory; do not indiscriminately revoke provider privileges.
3. Approve the scoped legacy-CA profile exception or obtain a compliant provider root. Implement/review runtime verified SSLContext handling and stronger startup role/effective-privilege checks as a separately approved backend security fix. Existing ssl=True is not demonstrated equivalent to the admin probe. Full-path TLS is not established and must not be represented as verified.
4. Confirm exact project target, snapshot/catalogue checks and rehearsal pass. Only then request **explicit approval for the identified migration SHA and privilege/provisioning SQL**. Role provisioning is separate from migration approval. Account creation/reset/deletion requires separate explicit approval, not implied by database approval.
5. Unresolved prerequisite means stop. Never use admin credentials temporarily as the app runtime or grant service_role/auth administration to make tests pass.

No migration/role/account creation executed in this review. No approval is assumed from “prepare,” “review,” or prior read-only connection authorization.

## Credential separation

- Admin: existing `database/admin.local.env`, mode0600, ignored. Only an approved administrator executes reviewed DDL/preflight/backup; postgres remains admin. Hidden password entry, no DSN in command arguments or logs.
- Runtime: only dedicated `astrogateway_runtime` URI in `backend/.env`, mode0600, ignored. Pooler username appends the approved project reference. NOINHERIT/non-owner/non-admin/NOBYPASSRLS/NOREPLICATION; only SET access to API role, no ADMIN option or provider memberships. Password provision via hidden DB-client password prompt/secret manager; never literal password-bearing SQL in a recorded script.
- Android: URL/publishable key/API origin only. Unity: mission/launch/session/user identifiers only. No DB secret, access/refresh token or password in Unity.
- Backups: private storage outside user-facing outputs, mode0600/encrypted as appropriate. Never publish Auth table dumps, password hashes, role passwords or JWT-related settings. Ordinary pg_dump table backup and separate role/ACL inventory are different tasks; neither exists yet.

## Pre-migration checks and backups

Repeat with authenticated verified TLS and READ ONLY transaction: server17 compatibility; Supabase Auth columns/UUID generator/anon+authenticated roles; absence or expected versions of training objects/API/runtime roles and auth trigger; owner/ACL/default privileges, all existing noninternal Auth triggers and definer functions; aggregate user count and null-created_at rows; direct/pooler target identity. Current snapshot: no training/API/runtime roles, no Auth users/noninternal Auth triggers; not an eternal guarantee.

Check provider migration/DDL rights explicitly in disposable rehearsal (catalogue SELECT does not prove GRANT/CREATE ROLE/trigger permission). Pause test signup traffic during the eventual trigger/backfill commit window. Set bounded lock/statement timeouts per approved runner outside migration; no unattended retry after unknown commit status.

Before changes, obtain a provider snapshot/PITR or approved test backup; verify restoration capability/retention, record baseline schema/role/membership/grant/default-ACL/trigger definitions and migration SHA in a redacted manifest. Fresh test DB can be recreated only with approval. If backup or recovery strategy unavailable, stop rather than silently accepting data-loss risk. No production users/data should be in this target.

## Exact SQL artifacts and intended order

Paths below are relative to the task workspace `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp`.

| Order | Exact artifact | Execution / status |
|---|---|---|
| 0 | `outputs/Stage05B1/Supabase_Configuration/Read_Only_Database_Preflight.sql` | Read-only preflight. Supplement with full effective ACL/owner/membership/default privileges and backfill aggregate checks recorded in Review_Evidence; no DDL |
| Gate | Provider-compatible PUBLIC ACL hardening SQL manifest | **Not yet authored/approved.** Must list exact preserved platform/client roles and affected objects/defaults. Assign/hash the final file before any execution authorization. No placeholder/global REVOKE is executable under this plan |
| 1 | `outputs/Stage05B/database/001_identity_and_mission_sessions.sql` | Original reviewed transactional migration; unchanged. SHA `269a983fe659e98f6083f3a8e46197ebe166d32e77eecc4a520539f1c686b59d`; conditional on all gates |
| 2 | `outputs/Stage05B1/Database_Plan_SQL/002_runtime_role.draft.sql` | Draft restricted login/membership attributes only, no password. Guard refuses unresolved PUBLIC schema USAGE/CREATE and database TEMPORARY. **It will refuse current project state.** Not rehearsed/executed/approved |
| 3 | `outputs/Stage05B1/Database_Plan_SQL/003_post_migration_metadata.sql` | Read-only catalogue verification after approved install/provisioning |
| 4 | Two-user API/RLS exercise below | After separately approved disposable accounts. No “auto-create user” SQL/script included |

The Gate is deliberately unresolved: the plan is not a complete runnable remote deployment bundle yet. This prevents accidental execution of the original migration while the strict isolation requirement is unsatisfied. Do not skip Gate or edit away the role-script guard. Once hardened ACL SQL exists and passes rehearsal, pin its exact file/order/hash with the baseline and role draft and obtain new explicit approval for that complete manifest. Existing project migration source remains unchanged.

001 contains BEGIN/COMMIT; run it as a complete script, not selected snippets. The role draft has its own transaction. Plan stop between transactions for acceptance checks; if role stage fails, keep the app disabled and follow reviewed rollback/recovery, not blind reruns. Assign secure runtime password only after role creation has passed approval and checks.

## Disposable rehearsal / dry-run plan

Do **not** run a BEGIN→001→ROLLBACK “dry run” on the remote approved test DB: 001 has its own COMMIT and such wrapping can commit unexpectedly. DDL/role rehearsals are writes even if rolled back and require approval.

Preferred: separately approved disposable Supabase test project or isolated PostgreSQL17 instance with an Auth-schema fixture, separate credentials and private backups. Local fixture tests model SQL semantics, not actual Supabase Auth hooks/mail/platform grants. Real disposable Supabase rehearsal is required for provider trigger/provisioning rights, PUBLIC ACL compatibility and mail flow. Existing PGlite21/mocked20 checks remain historical, not this rehearsal.

Rehearse exact full file manifest, confirm one success transaction, then repeat to demonstrate deliberate collision failure/rollback without losing first installation. Force failures at trigger/function/backfill/role stages and verify no unintended partial installation. Test unknown-success retry handling by comparing schema manifest before any rerun. Verify provider Auth signup/storage/client behavior after ACL hardening; no provider outage can be accepted as isolation success.

Run read-only metadata verification and restricted-role tests, including attempted Auth SELECT/INSERT/UPDATE, role/schema creation, elevated SET ROLE, DROP/ALTER training, grants, unrelated-schema/routine access, profile-level/owner/lease modifications, TRUNCATE and any PUBLIC/default-derived privileges. Denials must be under actual runtime login/API SET ROLE, not admin. Check direct login's NOINHERIT and exact role membership options. No role passwords or Auth records in reports.

## Post-migration metadata and application validation

Require three expected tables with RLS enabled, six named policies, four functions with intended owners/search_path and correct invoker/definer properties, one intended auth.users trigger, expected indexes/constraints/FKs/defaults. API/runtime must not own any training table or have superuser/BYPASSRLS/create-role/create-db/replication/provider memberships/admin option. Ensure only API is SET-able and no unexpected direct grants or PUBLIC object access remains. Verify provider roles retained only their intended services.

Require explicit runtime CA/context configuration, CERT_REQUIRED, hostname=true, approved root fingerprint and disclosed profile exception if retained. Inspect real runtime TLS transport. Keep pg_stat_ssl backend ssl=false/full-path limitation visible until resolved. No plaintext fallback. Start FastAPI with runtime URI only; verify startup rejects deliberately unsafe roles in rehearsal. Health response alone is not acceptance: query verified own profile and mission with real Auth.

## Two distinct users: RLS/API acceptance matrix

Create no users until explicit account approval and inbox access. Approved disposable A and B must be separate Supabase UUIDs; confirm email through actual delivery, not admin-confirm. Store secrets privately; operator-assisted callbacks, no full links/tokens in logs. Record labels/redacted UUID correlation rather than passwords or tokens.

1. Register A/B through Android; verify exactly one profile each, default trainee/level0 and provider creation timestamp. Reconnect/restart and read persistent own profile.
2. A's JWT returns A only; B returns B only. Supplying owner/user fields in HTTP payload is rejected; tampered/unsigned/expired/wrong-issuer/wrong-audience/unverified-email requests denied. Backend must derive both UUID and Auth-session UUID from issuer-verified claims.
3. Under runtime role with trusted A context, SELECT B profile/missions returns no rows; B update/completion changes nothing or is denied. Reverse A/B. Insert B owner/current mismatched SID under A context denied. Profile writes, training-level escalation, identity/lease edits, DELETE/TRUNCATE denied.
4. A initializes mission, completes with same launch/SID; repeat identical receipt returns original completion time. Wrong launch, other account, other A SID and expired initializations denied as specified. Revoking SID denies it while another active SID can continue. Test concurrent logout/completion race and stale JWT expiry behavior.
5. Pool reuse: A transaction commits/rolls back, then no-context access denied, then B sees B only. Force errors/cancellation and prove GUC/role do not leak. SQL statements remain bound/parameterized.
6. **Trust-boundary test:** in disposable environment, demonstrate direct runtime SQL can SET custom GUCs to B; document this as the trusted-broker limitation, not a passed anti-impersonation control. If the requirement forbids this, baseline fails and cannot ship without the separately approved stronger design.
7. Network loss, signup-confirmation failures, refresh/expiry, reset old/new password, restart, local/remote logout, Back navigation and private screen gates. Then authenticated dashboard→mission→Unity ACK/RESULT→dashboard→relaunch. Correlate mission IDs only; never send credential fields. Integration receipt earns no score.

Record passed/failed/blocked with reproduction and sanitized evidence. All checks above are **planned, not executed live**.

## Recovery and rollback

Before commit: any migration error aborts transaction; explicitly ROLLBACK and compare baseline. Do not suppress errors or use broad IF NOT EXISTS to accept unknown objects.

Unknown commit/network failure: stop app traffic; inspect the exact catalogue against the pinned manifest and commit markers. Do not rerun creation scripts blindly. If 001 committed but role provisioning failed, leave API disabled, diagnose ACL/rights, and choose separately approved roll-forward or rollback.

After commit: there is no automatic down-migration. Snapshot schema/rows privately; stop signup/API access; disable runtime login/terminate only its approved sessions if recovery is authorized. Remove the specific new Auth trigger first under approved rollback, then dependent reviewed objects in dependency order only if reverting is intended and test data retention is handled. Remove role memberships/grants/default ACL changes using recorded baseline; drop dedicated roles only after all dependencies are accounted for. Do not DROP SCHEMA CASCADE, DROP OWNED or drop shared/provider roles blindly. Never delete Auth users as “rollback” unless separately approved.

Restore provider ACL baseline and verify Auth/storage/client behavior, then restore from approved snapshot or recreate only the disposable target if recovery requires it. Reverify TLS and nonproduction identity. Rollback is another remote change requiring explicit operator approval; backup restore credentials never become runtime credentials.

## Operator approval request

Approve the trust-boundary decision and hardening/rehearsal scope first. After the complete SQL manifest, backup/recovery evidence and rehearsal results are ready, explicitly approve **only the pinned astrogateway-test migration and restricted-runtime provisioning**, or decline. Separate approval is needed for any account creation/reset, PUBLIC ACL/provider setting changes, and rollback. This review does not authorize any remote changes, and none were executed.
