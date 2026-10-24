# astrogateway-test — configuration status

The project name is approved by the user. Its project reference/dashboard URL and live configuration have not yet been supplied. **No migrations, roles or accounts were created. No live authentication validation passed.**

## Ready versus pending

| Item | Status |
|---|---|
| Local FastAPI dependencies and secure test launcher | Ready |
| FastAPI host endpoint | Running on `http://127.0.0.1:8000` in unconfigured mode |
| Emulator network | Rechecked: `http://10.0.2.2:8000/health` 200; `/v1/me` without authorization 401 |
| Android debug API origin | Set to `http://10.0.2.2:8000`; existing APK needs rebuilding after final configuration |
| Local private config files | Present with owner-only 0600 permissions; live values blank |
| Supabase project reference, URL and public key | User action required |
| Remote Email Auth/confirmation/redirect settings | Not verified; no available signed-in dashboard tab |
| Test-database catalogue/admin access | User action required |
| Restricted runtime role/password | Not created; requires explicit approval and reviewed test migration/provisioning |
| Approved accounts and inbox method | User action required; no accounts created |

Root `/` returning 404 is expected: this backend has no landing page. Use `/health`. A 200 health response containing `configuration_required` proves connectivity, not Supabase readiness.

## Enter the public Supabase values locally

1. Open the **astrogateway-test** dashboard and confirm the selected project is the approved test environment. Its URL has `/project/PROJECT_REF`; this generated reference is different from the display name. Share only that dashboard URL/reference for read-only inspection, never a key or password in chat.
2. In **Connect**, obtain the client Project URL. In **Settings → API Keys**, copy the **publishable** key (`sb_publishable_…`) or existing legacy **anon** key. Never choose secret/service-role. [Supabase API-key guidance](https://supabase.com/docs/guides/getting-started/api-keys).
3. Edit `/Users/kapilash/Projects/AstroGatewayAndroid/auth.local.properties` locally:

```properties
supabase.url=https://YOUR_TEST_PROJECT_REF.supabase.co
supabase.publishableKey=YOUR_PUBLISHABLE_KEY
auth.backendUrl=http://10.0.2.2:8000
```

4. Edit `/Users/kapilash/Projects/AstroGatewayAndroid/backend/.env` locally:

```dotenv
ASTROGATEWAY_ENVIRONMENT=test
ASTROGATEWAY_APPROVED_PROJECT_REF=YOUR_TEST_PROJECT_REF
SUPABASE_URL="https://YOUR_TEST_PROJECT_REF.supabase.co"
SUPABASE_PUBLISHABLE_KEY="YOUR_PUBLISHABLE_KEY"
DATABASE_URL=
SUPABASE_JWT_ISSUER=
CORS_ALLOWED_ORIGINS=
```

The URL/key must belong to the same project in both files. The publishable key is intended client configuration, but will still not be echoed in our reports. Leave database URI blank until restricted runtime access exists. **Do not restart the launcher with partial server settings:** it deliberately requires URL/key/DB URI together for configured mode. Current unconfigured process can continue connectivity checks while files are being filled.

Supported alternative Android environment names are `ASTROGATEWAY_SUPABASE_URL`, `ASTROGATEWAY_SUPABASE_PUBLISHABLE_KEY` and `ASTROGATEWAY_AUTH_BACKEND_URL`; local properties take precedence. The backend uses the uppercase `.env` names above through `run_local.py`, which parses values as data rather than sourcing shell code.

## Email Auth and redirects: required, not yet verified

In the selected project's Authentication settings, verify Email/password is enabled, signup is permitted for the approved disposable workflow and **Confirm Email** is enabled for the intended verification test. Record session expiry/refresh and rate-limit policy without changing settings speculatively.

Under **Authentication → URL Configuration**, authorize exactly:

```text
astrogateway://auth/callback
```

The existing Android manifest and SDK use that callback for both signup confirmation and recovery. Preserve a valid approved Site URL; native code explicitly requests the mobile redirect. `http://10.0.2.2:8000` is an API address, not an Auth callback. No localhost/FastAPI Auth callback endpoint is implemented. Avoid wildcard redirects. [Official redirect documentation](https://supabase.com/docs/guides/auth/redirect-urls).

Review **Confirm signup** and **Reset password** email templates. Preserve the provider verification link (`ConfirmationURL`) so the provider verifies the email action before returning a PKCE code to the app. Do not substitute direct token_hash/access-token links to our callback: the existing native receiver requires a code and a pending verifier. Open the link on the originating Android installation; do not share one-use links in logs/chat.

Verify email delivery. Supabase's built-in sender restricts recipients to project team addresses; arbitrary aliases may be rejected. Use approved eligible test inboxes or approved custom SMTP, with its credentials kept only in the provider configuration. [Official SMTP restrictions](https://supabase.com/docs/guides/auth/auth-smtp). SMTP/provider settings and actual delivery remain unverified.

## Separate database administration and runtime access

**Administration:** `/Users/kapilash/Projects/AstroGatewayAndroid/database/admin.local.env` (0600, ignored). Prepared keys:

```dotenv
ASTROGATEWAY_ENVIRONMENT=test
ASTROGATEWAY_APPROVED_PROJECT_NAME=astrogateway-test
ASTROGATEWAY_APPROVED_PROJECT_REF=
DB_ADMIN_DSN=
```

Prefer user-assisted Supabase SQL Editor for initial read-only catalogue review. A direct administration DSN is optional and must be supplied securely only if authorized. This file is not loaded by FastAPI or Android. Do not put an admin/postgres DSN into backend `.env`.

**Runtime:** server-only `DATABASE_URL` in backend `.env`. After approved migration and role provisioning, obtain the host/port/database from **Connect → Session pooler**, replace the default username with `astrogateway_runtime.PROJECT_REF` and use that restricted role's password, percent-encoded inside a quoted URI. Direct connections use `astrogateway_runtime` and `db.PROJECT_REF.supabase.co`. Use the actual dashboard endpoint. The role must be non-owner, non-superuser, NOBYPASSRLS and authorized to assume NOLOGIN `astrogateway_api`; it must have no privileged memberships. No service-role key is needed. TLS verification must not be disabled.

No passwords were generated here: generating credentials is not evidence that a DB role exists. Creating runtime role/password and applying SQL will require your explicit approval after target/catalogue review.

## Migration review and risks before approval

Reviewed unchanged `/Users/kapilash/Projects/AstroGatewayAndroid/database/001_identity_and_mission_sessions.sql`.

- Requires a Supabase Auth-backed database (`auth.users`, anon/authenticated roles), `gen_random_uuid()` and sufficient authorized DDL/trigger rights. A plain separate PostgreSQL instance lacks these dependencies.
- Creates private `training` schema, API role, tables, functions and an auth.users trigger; names may collide. Script is intentionally not idempotent. Inspect existing migration state first.
- Backfills profiles for existing Auth users. Keep the approved test DB free of production identities; do not silently overwrite mappings or reuse production.
- Trigger failure can fail future signups. Validate privileges and trigger interaction before live signup.
- All changes are transactional and there are no DROP statements, but source review does not prove successful execution or RLS installation.
- Keep `training` out of Data API exposed schemas. RLS protects verified transaction-local user/session ownership; privileged/owner connections bypass protections and cannot serve as runtime RLS evidence.
- Runtime role is distinct from migration administrator and cannot edit profile/training-level fields. Test two approved accounts after installation.

`Read_Only_Database_Preflight.sql` in this deliverable directory contains metadata-only queries for an authorized test SQL Editor. It has not been executed. After preflight, show the exact target and changes for approval; no migrations/accounts may proceed before approval.

## Endpoint and test workflow

The already-running PID 32534 uses blank configuration. After all approved settings and restricted database access are ready, stop that specific process (verify identity/port first) and run:

```sh
/Users/kapilash/Projects/AstroGatewayAndroid/backend/.venv/bin/python /Users/kapilash/Projects/AstroGatewayAndroid/backend/run_local.py
```

It reads owner-only `.env`, enforces the declared test reference/provider/database match and disables access logs. It never applies a migration. After configuration succeeds, rebuild Android through its existing `tools/build.sh`. Native Views, Unity architecture and scene/export remain unchanged.

Test-account workflow file: `/Users/kapilash/Projects/AstroGatewayAndroid/live.test-secrets.json` (0600, ignored). Supply two disposable identities/passwords locally, then explicitly approve fresh signup and recovery/password changes. Keep inbox access user-assisted unless another secure method is approved. Do not create/admin-confirm accounts as evidence of real email verification. Account creation, confirmation, recovery, session refresh, live RLS and authenticated Unity tests remain pending.

No application implementation changes were made. Only separate administration template/private blank configuration slots and this documentation/evidence were added. Emulator was stopped after the successful network check; AVD preserved. Server remains running in connectivity-only mode.
