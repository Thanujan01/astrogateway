# Stage05B.1 — local test environment setup

Local server setup and emulator network checks are complete. **Supabase configuration, database access, email delivery and approved test-account access remain pending. No live authentication passed; no migration was applied.**

## What was prepared

Project: `/Users/kapilash/Projects/AstroGatewayAndroid`.

- Existing backend runtime requirements installed in `backend/.venv`, isolated from the system Python.
- `backend/run_local.py`: loads owner-only `.env` as data, never shell commands. Allows only expected keys; requires explicit test environment. Configured provider origin must match approved project reference, and database endpoint must match that reference through direct host or pooler username. It never runs migrations. API access logs disabled; config values not printed.
- `tools/setup_backend.sh`: repeatable project-only virtualenv dependency setup.
- Updated safe `backend/.env.example`; created blank `backend/.env` with mode 0600. All required live settings remain blank.
- Created blank `auth.local.properties`, mode 0600, with only emulator API origin configured. Public URL/key remain blank.
- Created `test-accounts.example.json` and ignored `live.test-secrets.json`, mode 0600. Credentials blank, signup/reset permission flags false.
- `.gitignore` covers private config, test-secret JSON and backend caches. Project currently has no Git repository; these ignore rules are preparation for future version control, not proof of an existing tracked-file audit.

No changes to the Android UI, navigation, Unity bridge, Mars scenes or React source. Existing app APK was not rebuilt against blank settings.

## Dashboard → exact local destination

Use an existing explicitly approved **non-production** project. If none exists, create/select a test project yourself and confirm its reference before configuration; no project creation, billing action or production change was performed here.

| Dashboard value | Where to find it | Local destination |
|---|---|---|
| Project reference | Selected project's dashboard URL contains `/project/PROJECT_REF`; corroborate selected project name/environment | `backend/.env`: `ASTROGATEWAY_APPROVED_PROJECT_REF` |
| HTTPS project URL | Project **Connect** application/client panel, or project **Settings → Data API** project URL | `backend/.env`: `SUPABASE_URL`; project-root `auth.local.properties`: `supabase.url` |
| Publishable API key (`sb_publishable_…`) | **Settings → API Keys → Publishable and secret API keys**; choose publishable only | `backend/.env`: `SUPABASE_PUBLISHABLE_KEY`; `auth.local.properties`: `supabase.publishableKey` |
| DB host, port, database and project username suffix | **Connect → Session pooler** (5432), or direct URI (5432) if IPv6 connectivity works | `backend/.env`: `DATABASE_URL`, using the dedicated runtime role, not the default postgres username/password |
| Dedicated runtime role password | Set during authorized test DB role provisioning; distinct from API keys and postgres/admin password | Percent-encoded inside the quoted server-only `DATABASE_URL` |
| Local API origin | Prepared here | Android `auth.backendUrl=http://10.0.2.2:8000`; host `http://127.0.0.1:8000` |
| Auth-session expiry and refresh settings | Selected project's Authentication/session configuration; record actual values | Validation checklist only; do not invent expiry or change it without approval |

Supabase documents publishable keys under [Settings → API Keys](https://supabase.com/docs/guides/getting-started/migrating-to-new-api-keys), and client URL/key use in its [client setup](https://supabase.com/docs/guides/getting-started/quickstarts/reactjs). This reference does not request or modify the React project.

The database URI is available from **Connect**. Prefer direct access for a long-running server when supported, or session pooler for IPv4. [Official connection guide](https://supabase.com/docs/guides/database/connecting-to-postgres), [pooling guidance](https://supabase.com/docs/guides/database/connecting-to-postgres/pooling-and-limits). Existing repository disables prepared-statement caching, but this setup recommends session/direct access rather than using transaction pooling for connection-level startup role checks.

Do not copy a secret API key (`sb_secret_…`), service-role key, signing secret, refresh token or password into Android properties. Only the server holds DB credentials. Keep all values out of chat, screenshot evidence, source commits and terminal command arguments.

## Fill the files locally

Edit the already-created owner-only file:
`/Users/kapilash/Projects/AstroGatewayAndroid/backend/.env`

```dotenv
ASTROGATEWAY_ENVIRONMENT=test
ASTROGATEWAY_APPROVED_PROJECT_REF=YOUR_APPROVED_TEST_REF
SUPABASE_URL="https://YOUR_APPROVED_TEST_REF.supabase.co"
SUPABASE_PUBLISHABLE_KEY="YOUR_PUBLISHABLE_KEY"
DATABASE_URL="postgresql://astrogateway_runtime.YOUR_APPROVED_TEST_REF:URL_ENCODED_RUNTIME_PASSWORD@YOUR_SESSION_POOLER_HOST:5432/postgres"
SUPABASE_JWT_ISSUER=
CORS_ALLOWED_ORIGINS=
```

Above is illustrative; no real credential is included. For direct DB connection, username is `astrogateway_runtime` and host is `db.YOUR_APPROVED_TEST_REF.supabase.co`; use the actual dashboard endpoint. Shared pooler username is `astrogateway_runtime.YOUR_APPROVED_TEST_REF`. Percent-encode URI password characters such as `@`, `:`, `/`, `#` and `%` through a secure local editor/tool; quote the entire URI. The password is the dedicated runtime role's database password, not the API key. The launcher checks URI/project correspondence, then existing backend verifies TLS and role restrictions at startup.

Fill project-root `/Users/kapilash/Projects/AstroGatewayAndroid/auth.local.properties`:

```properties
supabase.url=https://YOUR_APPROVED_TEST_REF.supabase.co
supabase.publishableKey=YOUR_PUBLISHABLE_KEY
auth.backendUrl=http://10.0.2.2:8000
```

No `export`, shell substitutions or multiline values in `.env`. Do not `source` it. `run_local.py` loads it securely; direct `uvicorn main:app` still does not automatically read `.env`. Optional issuer override and CORS stay blank for this native test.

## Database prerequisites and review

Migration: `/Users/kapilash/Projects/AstroGatewayAndroid/database/001_identity_and_mission_sessions.sql`.

Before applying anything, confirm target project/environment and inspect catalogue for existing `training` schema, `astrogateway_api` role, profile trigger or previous migration. Script is transactional but deliberately not idempotent; never rerun blindly. Required Supabase objects/capabilities include `auth.users` (id, created_at, raw_user_meta_data), anon/authenticated roles, `gen_random_uuid()`, and authorized rights to create a schema/role/tables/functions and an auth.users trigger. A generic blank PostgreSQL database lacks Supabase Auth dependencies and is not a substitute for this test project.

The migration creates private RLS-protected profiles, mission sessions and session revocations. Security-definer profile trigger has fixed search_path and ignores user metadata for privilege/level. Backfill affects existing account-profile mappings. Review existing triggers, backups and name collisions before running under the authorized test administrator. **No production migration, destructive modification or migration execution occurs in this setup.**

After reviewed migration application, provision the dedicated role under authorized administration:

```sql
CREATE ROLE astrogateway_runtime LOGIN NOINHERIT NOSUPERUSER
  NOCREATEDB NOCREATEROLE NOBYPASSRLS;
GRANT astrogateway_api TO astrogateway_runtime;
```

These are preparation commands, not executed commands. Do not create a duplicate existing role. Set a strong runtime password through a secure database administrator tool/password prompt (e.g. trusted psql `\password astrogateway_runtime`); do not paste password-bearing SQL into shared logs/history. Role must not own `training` tables and must not have privileged memberships. Keep schema unexposed in Data API. Native callers use FastAPI, never direct DB access. Server rejects superuser/BYPASSRLS/table-owner runtime access. Administrative credentials remain separate from `backend/.env`.

Live connectivity/RLS and migration rights cannot be verified until approved access is supplied. Do not weaken TLS verification to work around a certificate failure; inspect the actual connection and trust requirements first.

## Email and account workflow

On the selected test project:
1. In Authentication sign-in/provider settings enable Email/password and Confirm Email for genuine verification testing.
2. In **Authentication → URL Configuration**, authorize exact `astrogateway://auth/callback` redirect. Keep existing approved Site URL; explicit mobile redirect is passed by native code. Review confirmation/recovery email templates against the supported PKCE flow; retain the provider-generated confirmation URL rather than replacing it with a raw access-token URL. [Redirect documentation](https://supabase.com/docs/guides/auth/redirect-urls).
3. Verify SMTP delivery to the two approved test inboxes. Default Supabase mail delivery is restricted to project team addresses and rate-limited; arbitrary disposable aliases may not receive mail. Configure approved test SMTP if needed, storing SMTP credentials only in the dashboard, or use approved eligible team inboxes. [Official SMTP restrictions](https://supabase.com/docs/guides/auth/auth-smtp), [rate limits](https://supabase.com/docs/guides/auth/rate-limits). Do not disable confirmation or admin-confirm accounts as evidence of email verification.
4. Fill `/Users/kapilash/Projects/AstroGatewayAndroid/live.test-secrets.json` locally with two disposable account identities and passwords; confirm approved project reference and explicitly authorize disposable signup and password reset/change through its flags if intended. These fields are workflow metadata, not a second auth system. No testing code automatically reads/executes this file yet.
5. Do not share inbox passwords. Use user-assisted opening of verification/recovery emails on the originating emulator installation, or provide a separately approved secure mechanism. PKCE verifier lives on that installation; a callback URL may itself contain a one-use credential, so do not paste full links into chat/logs.
6. Create fresh account A through Android's signup form, verify delivered email, sign in; repeat isolation flow with account B. Use at least 12 characters for new passwords. Test wrong password, profile default/persistence, restart, real refresh/expiry, recovery (new password succeeds, old fails), logout and two-account DB isolation. Then perform authenticated Unity launch/message/return/relaunch. Publish only sanitized evidence and credential-free screens.

## Start, stop and rebuild

Dependencies are already installed. To reproduce setup only:

```sh
zsh /Users/kapilash/Projects/AstroGatewayAndroid/tools/setup_backend.sh
```

After filling approved settings, restart the local test server so it reads the new `.env`:

```sh
/Users/kapilash/Projects/AstroGatewayAndroid/backend/.venv/bin/python /Users/kapilash/Projects/AstroGatewayAndroid/backend/run_local.py
```

It binds loopback only at port 8000; press Ctrl+C in your terminal to stop it. A tool-managed connectivity-only instance is currently running as PID **32534**. Stop that specific instance with `kill 32534` before starting your configured instance; if it has already exited, confirm port ownership rather than killing an unrelated process. No shell tracing or HTTP access logs.

Only after public provider settings are filled, rebuild Android using existing `tools/build.sh` and install its debug APK on the emulator. Debug network policy permits HTTP only to `10.0.2.2`; release must use HTTPS. No configured app build/live authentication was performed here.

## Connectivity evidence and limits

Actual authorized emulator `sdk_gphone64_arm64` accessed FastAPI through `10.0.2.2:8000`:
- `/health`: HTTP 200, `configuration_required`.
- `/v1/me`: HTTP 401, `Sign-in required.`

The host produced the same responses. Evidence: `connectivity-validation.json`. The Android emulator's host-loopback alias is documented by [Android Developers](https://developer.android.com/studio/run/emulator-networking-address). Test used an emulator shell HTTP socket probe; this proves network reachability, not authenticated app HTTP behavior. Initial probe closed stdin too soon and returned no health response; final probe kept the stream open until response and passed.

Parser checks passed owner-only permission enforcement, literal quoted value handling without shell expansion, unexpected-key rejection and syntax validation (`helper-validation.json`). Dependencies installed successfully (`dependency-setup.log`). No live Supabase/Auth/DB request or migration occurred. No live-auth screenshot exists. Emulator is stopped after the connectivity check; its existing AVD is preserved.

## Inputs still needed from the user

Confirm the approved non-production project name/reference, then fill the local public/provider settings and restricted DB URI. Provide authorized test-db catalogue/migration access (user-assisted SQL Editor is sufficient for initial review/provisioning), approval for two disposable accounts/password-reset changes, and the inbox-opening method. Do not send passwords/DSNs/API secrets in chat. Return only confirmation of the selected test project and which secure local files are ready.
