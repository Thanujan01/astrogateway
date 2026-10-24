# AstroGateway Stage 05B — authentication foundation

Implemented the native authentication path and private profile/mission API. **Live authentication acceptance remains blocked**: no existing Supabase test-project URL/publishable key, configured private database/API or approved test accounts were supplied. The APK deliberately has blank account configuration, displays that state, and offers no demo authenticated workspace. No cloud migration or deployment was performed.

## Architecture

Supabase Auth is the sole issuer; `auth.users.id` is the canonical account identity. Existing Stage05A Kotlin Views, private Unity Activity, separate `:unity` process, bridge protocol version 1, Back callbacks and warm relaunch remain. No second Unity login or React changes.

Native host implements email/password registration/sign-in, inbox confirmation handling, same-device PKCE recovery and password replacement, sign-out, persisted session, serialized refresh, retry/offline/error states and authenticated route gates. Registration enforces a 12-character client minimum; sign-in permits existing account passwords and leaves verification to Supabase. SDK is Supabase-documented community `supabase-kt` 3.2.6, the compatible stable pin for this Kotlin 2.2 toolchain, rather than the newest release that requires a compiler upgrade.

SDK session, PKCE verifier and pending flow are encrypted using Android Keystore AES-GCM in AtomicFile/no-backup storage. Passwords are transient, cleared on submission and excluded from saved state and credential-screen screenshots. Logging is disabled for the SDK; no credentials enter bridge messages or application logs. Logout blocks subsequent session-cache writes before remote revocation. Loss/tampering fails closed.

Following the existing platform plan, FastAPI owns access to private `training.profiles`, `mission_sessions` and `auth_session_revocations`. Online Supabase Auth verification precedes claim checks and transaction-local canonical identity. Restricted non-owner/NOBYPASSRLS runtime role and SQL RLS enforce owner access; clients cannot write training level or access the schema through Data API. Current-user profile, mission-init, integration-receipt and Auth-session-revocation endpoints are the entire backend scope. A receipt earns no score.

Unity receives only mission ID, launch UUID, user UUID, mission-session UUID and a native expiring lease. No password, JWT, refresh token, database credential or service-role key is passed. The existing C# receiver accepts the additive version-1 fields unchanged. Native validates owner/correlation, mediates submission, and asks Unity to return when local authorization is revoked or the lease expires.

## Files created / modified

Project root: `/Users/kapilash/Projects/AstroGatewayAndroid`.

Modified:
- `app/build.gradle`: public-only configuration validation, BuildConfig and compatible Auth SDK/OkHttp dependencies; instrumentation runner.
- `app/src/main/AndroidManifest.xml`: Internet permission, exact PKCE callback, cleartext/network policy; existing private Unity entry retained.
- `app/src/main/java/com/sk2lb/astrogateway/poc/MainActivity.kt`: account forms, recovery, protected routes, canonical profile and brokered mission lifecycle.
- `app/src/main/java/com/sk2lb/astrogateway/poc/PocUnityActivity.kt`: noncredential context/lease validation and revocation handling.
- `.gitignore`: local authentication configuration and server secrets/caches excluded.

Created:
- `app/src/main/java/com/sk2lb/astrogateway/poc/AuthCoordinator.kt`
- `app/src/main/java/com/sk2lb/astrogateway/poc/SecureSessionStore.kt`
- `app/src/main/res/xml/network_security.xml`
- `app/src/debug/res/xml/network_security.xml`
- `app/src/androidTest/java/com/sk2lb/astrogateway/poc/SecuritySmokeInstrumentation.kt` — fixtures in test APK only, no production auth bypass.
- `auth.local.properties.example`
- `backend/main.py`, `backend/database.py`, `.env.example`, `requirements.txt`, `requirements-dev.txt`, `README.md`, `tests/test_security_contract.py`
- `database/001_identity_and_mission_sessions.sql`

Prior host source backup is in project `Stage05B/backup/`. Deliverable source snapshot is in this report's `source/` directory. The original Unity scene/asset/design sources and React source hashes are unchanged. One open-Editor user layout file, `MissionRedHorizon/UserSettings/Layouts/default-6000.dwlt`, changed during the session; no task code wrote it and it was not restored over the user's layout.

## Validation evidence

| Check | Result / evidence |
|---|---|
| Debug app + test APK | BUILD SUCCESSFUL; `build.log`, existing Unity Java17/AGP9.1.1/Gradle9.3.1, ARM64 target36/min26 |
| Installation | Both APKs installed successfully on authorized Android16/API36 ARM64 Google emulator `sdk_gphone64_arm64`, `emulator-5556`; physical device not tested |
| Encrypted storage | Emulator test passed persistence across store instances, ciphertext inspection, tamper rejection, key-loss rejection, logout write gate and encrypted PKCE |
| Unity transport | Two actual launch/message/Back-return/relaunch cycles passed; native message reached C#, C# ACK/RESULT arrived; synthetic nonsecret mission context, **not an authenticated-dashboard test** |
| Backend contract | 20 passed with isolated synthetic tokens/mock Auth transport; not a live identity test |
| Local database RLS | 21 passed in PGlite PostgreSQL WASM, including cross-account denial, privilege escalation denial, expiry, revocation, idempotency and transaction-context isolation; not deployed/cloud RLS |
| Backend HTTP startup | Actual localhost server: `/health` 200 configuration_required; `/v1/me` 401 without bearer |
| Unauthenticated navigation/access | 10 checks passed: signup/signin/reset validation, system Back, no demo entry, forged return denied, unsolicited callback denied, restart and empty-session signout; `navigation-validation.json` |
| Crash/ANR review | No AstroGateway crash or ANR observed; exit history records test force-stops. One UIAutomator tool process crashed during overlapping dumps; serial retry passed. Nonfatal binder-freeze warning at instrumentation shutdown. |
| Real registration, email verification, login/wrong password | Not tested; approved Supabase configuration/accounts unavailable |
| Real session persistence/refresh/expiration/sign-out | Implementation present; live provider behavior untested. Offline vault/claim/revocation tests do not prove live SDK refresh |
| Authenticated Unity launch/dashboard return/relaunch | Blocked by live account/profile/API configuration; transport regression separately passed |
| Live second-account isolation | Blocked; local SQL RLS and API contracts passed |

`instrumentation.log`, `runtime-logcat.log`, `rls-validation.json`, `rls-tests.log`, `backend-tests.log`, `backend-http-smoke.json` contain the evidence. Credential-bearing screens intentionally use FLAG_SECURE, so their screenshots are not captured. See `screenshots/` for credential-free Welcome, recovery request and signed-out states.

## Manual configuration / remaining acceptance

Read `Configuration_And_Security.md` for exact setup. Reuse the authorized test Supabase project; configure email/password, confirmation and SMTP; authorize `astrogateway://auth/callback`; review PKCE email templates and token lifetime. Provision a restricted server-only login, review and approve the additive SQL, apply to a disposable test project, then configure the HTTPS API and ignored Android public properties. No secrets belong in BuildConfig except the intentionally public publishable/anon key.

Run two approved disposable accounts through signup/confirmation, correct/incorrect login, recovery, force-stop/restart, refresh across actual expiry, online/offline logout and account isolation. Then test authenticated Unity launch, messages, dashboard return/relaunch, Back and active-mission revocation. Never substitute fixtures for these acceptance tests.

Before public release, replace custom-scheme callbacks with verified HTTPS App Links on an owned domain, add ingress rate limits/monitoring with redaction, verify actual deployment TLS/role grants, and perform physical-device QA. SDK sign-out cannot retroactively invalidate all existing Supabase access JWTs; the API's revocation ledger covers this app's current Auth session. Offline server revocation cannot be promised; short provider expiry limits the residual window. No mission grade or earned progress is fabricated.

Known warnings: existing Unity Play Asset Delivery class lookup and unavailable depth-of-field shaders; these did not prevent the transport tests. Gradle reports future Gradle10 deprecations. Backend tests report a Starlette TestClient/httpx deprecation (test harness only). No dependency reinstall or Unity re-export was required for this stage.

## Primary implementation references

- [Supabase Kotlin installation](https://supabase.com/docs/reference/kotlin/installing): SDK setup; community SDK attribution.
- [Supabase Kotlin SDK source](https://github.com/supabase-community/supabase-kt): exact compatible API and version pin.
- [Supabase Auth sessions](https://supabase.com/docs/guides/auth/sessions): expiry/refresh policy.
- [Supabase sign-out](https://supabase.com/docs/guides/auth/signout): access JWT lifetime after sign-out.
- [Supabase JWTs](https://supabase.com/docs/guides/auth/jwts): authenticated claim checks.
- [Supabase user data](https://supabase.com/docs/guides/auth/managing-user-data): auth-linked profile trigger and fixed search path.
- [Supabase RLS](https://supabase.com/docs/guides/database/postgres/row-level-security): ownership policies and untrusted metadata.
- [PGlite](https://pglite.dev/docs/about): isolated PostgreSQL WASM validation engine.

Stage 05B implementation stops here. Authentication/backends are not declared live-validated or production-ready.
