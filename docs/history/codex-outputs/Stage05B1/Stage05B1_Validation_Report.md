# Stage 05B.1 — Live Authentication Validation

**Result: BLOCKED. Completion criteria are not met.**

Inspection performed on 2026-10-08. No approved Supabase test-project configuration, test-database access, API origin or disposable accounts/inbox access is available in the inspected configuration. A request for secure local configuration paths has been sent. A missing prerequisite is not an authentication failure or a passed live test.

## Current inspection results

| Check | Result | Evidence |
|---|---|---|
| Read Stage05B report and security/configuration instructions | PASS | Both requested documents read |
| Inspect Android and FastAPI configuration | PASS — configuration absent | `configuration-inspection.json`, `build-config-presence.json` |
| Identify required settings without exposing their values | PASS | Presence-only inspection; table below |
| Review migration source | PASS — source review only | Project SQL matches Stage05B delivered SQL, SHA256 `269a983fe659e98f6083f3a8e46197ebe166d32e77eecc4a520539f1c686b59d` |
| Check Unity handoff source | PASS — static inspection only | Version-1 JSON fields: version, launchId, missionId, missionSessionId, userId. Native Intent also contains expiresAtMillis. No password/access-token/refresh-token field. Auth SDK logging is disabled |
| Apply migration to approved test database | BLOCKED | No identified test target or authorized connection; no remote catalogue/role inspection possible |
| Configure Android against approved project | BLOCKED | URL/publishable key/API origin missing |
| Signup and email verification | BLOCKED | Provider and approved disposable registration account/inbox missing |
| Correct login / incorrect-password rejection | BLOCKED | No approved real account credentials/provider |
| Logout / remote revocation | BLOCKED | No real session or configured training API/database |
| Password reset and recovered-password login | BLOCKED | No account/inbox access or confirmed redirect/email configuration |
| Session persistence after restart and refresh | BLOCKED | No real provider session; actual expiry/rotation cannot be exercised |
| Expired-session behavior | BLOCKED | No issued session/provider expiry policy |
| Profile creation and persistence | BLOCKED | No migrated approved database or registered user |
| Two-account PostgreSQL RLS isolation | BLOCKED | No test DB/runtime role or two approved accounts |
| Authenticated dashboard → Unity → return → relaunch | BLOCKED | Authentication/profile/mission API prerequisites absent |
| Unauthorized navigation and network failures with live auth | BLOCKED | No live authenticated state; prior offline results are not new live evidence |
| Configured debug APK build/install | BLOCKED | Building again with empty configuration cannot fulfill this test. Prior Stage05B APK remains available |
| Live runtime credential/log inspection, crashes and ANRs | BLOCKED | No live configured execution occurred in this stage |

No live test executed or failed. No migration, cloud request, production change, application-source change, Unity-scene change or React-source change was performed. Bridge/navigation remain untouched.

## Exactly what is missing

- Identity of the **approved non-production Supabase project**, its HTTPS project origin and publishable/legacy anon key. No service-role key is required or requested.
- Authorized test-database administration access to inspect existing objects, roles and triggers and apply the reviewed migration to this specific test target.
- A server-only PostgreSQL runtime DSN for a dedicated non-owner, non-superuser, non-BYPASSRLS login that may assume `astrogateway_api`; SSL-capable connectivity. This is separate from migration administration access.
- Reachable test FastAPI origin, or server-only environment configuration enabling a local test API. Android needs this API for profile validation and mission initialization even if Supabase Auth itself is reachable.
- Two approved disposable test-account identities/passwords for isolation testing, plus authorization for a fresh disposable signup and password-reset/change testing. Provide passwords only through ignored local files or a secure secret mechanism.
- Access to verification/recovery inboxes, or user-assisted link opening on the emulator/device. Provider email-confirmation policy, redirect allowlist/templates and refresh/expiry settings must be inspected on the approved project.

Local evidence: project `auth.local.properties` and `backend/.env` do not exist; only examples exist. No `*.test-secrets.json` found in the Android project or task workspace. The required environment variables are absent from this execution environment. Generated debug BuildConfig has empty Supabase URL, public key and backend origin. Standard root `.env`, `.env.local` and `supabase/config.toml` files are also absent in the Unity and React projects. This does not assert that no Supabase project exists externally; its approved configuration has not been made accessible here.

## Required configuration names

| Consumer | Required setting | Secret handling |
|---|---|---|
| Android ignored `auth.local.properties` | `supabase.url`, `supabase.publishableKey`, `auth.backendUrl` | Public configuration only; never service-role or DB credentials |
| Android alternative build environment | `ASTROGATEWAY_SUPABASE_URL`, `ASTROGATEWAY_SUPABASE_PUBLISHABLE_KEY`, `ASTROGATEWAY_AUTH_BACKEND_URL` | Same public values; no logging |
| FastAPI server environment | `SUPABASE_URL`, `SUPABASE_PUBLISHABLE_KEY`, `DATABASE_URL` | DSN/password server-only, not in APK/chat/logs/committed files |
| Optional server environment | `SUPABASE_JWT_ISSUER`, `CORS_ALLOWED_ORIGINS` | Default issuer belongs to same project; do not override speculatively |

The backend does not automatically load `.env`; inject its approved values into the server process securely. Test credentials do not belong in these public Android properties. No secret values were printed or copied into this report.

## Migration review and application gate

Reviewed `/Users/kapilash/Projects/AstroGatewayAndroid/database/001_identity_and_mission_sessions.sql`. It is transactional and creates a private `training` schema, three RLS-protected tables, NOLOGIN/NOBYPASSRLS API role, transaction-context helpers, owner/session policies and an auth.users profile trigger/backfill. It neither drops objects nor changes account passwords. API privileges exclude profile/training-level edits. Public/anon/authenticated clients cannot access the private schema; runtime identity comes from verified FastAPI context. Trigger has fixed empty search_path and uses user metadata only for a bounded display name.

The migration is intentionally not rerunnable: existing role/schema/trigger names can conflict. Before application, inspect the approved database catalogue and ownership, verify this is not production, review existing auth.users triggers and backup/rollback arrangements, and determine whether migration is already installed. Creating the auth trigger affects subsequent signups; backfill touches existing profile mappings. Review/apply once only against the authorized test database. The runtime login must not own these tables. Source review and prior local SQL tests do not prove these policies are installed or effective on a live database.

## Reproduction / resume procedure

1. Supply secure local configuration paths and explicit identification of the non-production target. Do not put passwords, JWTs, DSNs or signing/service-role keys into chat, screenshots or command arguments. Confirm test-account/reset permissions and how inbox links will be opened.
2. Inspect test DB objects/roles and Auth settings. Apply the reviewed migration only if absent and appropriate. Provision the restricted runtime login separately through authorized administration. Keep `training` unexposed in Data API. Configure exact callback `astrogateway://auth/callback` and supported PKCE email templates.
3. Start the existing FastAPI code with the same approved project and runtime environment, access logging disabled. Check health and confirm unauthenticated `/v1/me` rejects access. Configure Android public properties and rebuild with existing `tools/build.sh`; install on the existing approved emulator. Record APK hash, device and installation evidence.
4. Register approved disposable account A. Verify actual confirmation policy: before confirmation private data must not open; open email callback on the originating installation, verify success and sign in. Reject wrong password. Recover password via same-device request/email, reject old password and accept new one; never log or screenshot credentials.
5. Check canonical user/profile UUID, default trainee/level and creation timestamp. Force-stop/restart and verify persisted session through live Auth and API. Observe actual access expiry and refresh, checking session continuity and rotated storage without emitting token values. Verify terminal expiry/revocation gates private routes and mission launch.
6. With accounts A/B, verify each sees only its own profile/missions through API and restricted PostgreSQL RLS. Cross-account reads/mutations must be denied/hidden; profile and training-level privilege escalation must fail. Do not use an owner/service-role connection as evidence that RLS works.
7. After account authentication, launch Unity from dashboard/mission screen, correlate mission/launch/session identifiers with ACK/RESULT, return to the authenticated dashboard and relaunch. Check Back and sign-out during an active mission. Inspect captured logs and bridge data privately for secrets; publish only redacted results/identifier field names.
8. Exercise network loss during sign-in, profile validation, refresh, mission launch and logout. Record actual failures and recovery. Offline logout must gate local state; remote revocation must not be claimed if unconfirmed. Restore network and verify behavior.
9. Capture credential-free dashboard/mission/return screenshots, app-scoped crash/ANR evidence and sanitized test receipts. Mark every required live check passed/failed/blocked with reproduction steps. Completion requires all specified live checks, not prior fixture tests.

## Screenshots and prior evidence

**No new live screenshots exist**, because no configured execution occurred. Prior Stage05B screenshots show the unconfigured app and are historical only: [Stage05B screenshots](../Stage05B/Screenshots.html). Previous build, 20 mocked backend checks, 21 local PostgreSQL WASM checks and two synthetic-context Unity transport cycles remain historical evidence; they are not Stage05B.1 live passes.

Stopped affected live tests at the missing-configuration gate. Stage05B.1 remains blocked pending the listed inputs.
