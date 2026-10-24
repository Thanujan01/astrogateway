# Stage05B.1 — Supabase connection readiness

**Latest update (2026-10-09): Secure read-only PostgreSQL authentication passed. Client-to-pooler TLS 1.3, chain and hostname validation confirmed. Restricted roles and training schema are absent; FastAPI runtime remains unconfigured. See the latest verification section below.**

**Public configuration and Auth API connectivity verified. Full live authentication remains BLOCKED. No remote database modification or account creation occurred.**

Approved test project: `astrogateway-test`, reference `jvkwofmxtxjhdwtwdboz`. Inspection and public read-only probes performed 2026-10-08. Publishable-key values are intentionally omitted from this report.

## Configuration status

| Item | Status |
|---|---|
| Android `supabase.url` / `supabase.publishableKey` | Saved to approved project; no DB/admin/service-role credentials added |
| Android `auth.backendUrl` | `http://10.0.2.2:8000` (debug emulator only) |
| Backend `SUPABASE_URL` / `SUPABASE_PUBLISHABLE_KEY` | Same approved public configuration saved |
| Backend test environment/project guard | `ASTROGATEWAY_ENVIRONMENT=test`; exact approved project reference saved |
| Backend `DATABASE_URL` | Blank; dedicated restricted runtime credentials unavailable |
| Administrative configuration | Approved reference saved separately; `DB_ADMIN_DSN` remains blank |
| Test credentials | No approved account credentials or inbox access supplied |
| Permissions | Android/backend/admin local files have mode 0600 |
| Git exclusion | Verified four private paths match `.gitignore` using an isolated scratch Git directory; actual Android project has no Git repository. No Git init/commit performed in it |

Exact local paths:
- `/Users/kapilash/Projects/AstroGatewayAndroid/auth.local.properties`
- `/Users/kapilash/Projects/AstroGatewayAndroid/backend/.env`
- `/Users/kapilash/Projects/AstroGatewayAndroid/database/admin.local.env`
- `/Users/kapilash/Projects/AstroGatewayAndroid/live.test-secrets.json`

Android alternative environment keys: `ASTROGATEWAY_SUPABASE_URL`, `ASTROGATEWAY_SUPABASE_PUBLISHABLE_KEY`, `ASTROGATEWAY_AUTH_BACKEND_URL`. Server optional `SUPABASE_JWT_ISSUER` and `CORS_ALLOWED_ORIGINS` remain blank. Native Android does not require CORS. No token/password/key value was printed in result logs or reports.

## Executed connection and build checks

| Check | Actual result |
|---|---|
| Approved Supabase `/auth/v1/health` with configured publishable key | HTTP 200 |
| Approved Supabase `/auth/v1/settings` | HTTP 200; selected nonsecret flags recorded |
| Email authentication flag | `external.email=true` — verified through the public Auth settings API |
| Signup availability flag | `disable_signup=false` — verified through the public Auth settings API |
| Confirmation flag | `mailer_autoconfirm=false`; actual confirmation-email delivery/link exchange remains untested |
| Host FastAPI `/health` | HTTP 200, `configuration_required` |
| Host `/v1/me` without credentials | HTTP 401 |
| Emulator HTTP to `10.0.2.2:8000` | `/health` 200 configuration_required; `/v1/me` 401 |
| Direct DB DNS lookup | Failed through this Mac's system resolver for both IPv4 and IPv6 (error code 8); no PostgreSQL login attempt possible |
| Android debug build | BUILD SUCCESSFUL with approved public configuration |
| Generated BuildConfig | URL and public-key matches checked as booleans, without exposing key values |
| Debug install / native host opening | See `Connection_Readiness/android-build-install.json`; no signup/sign-in or authenticated Unity launch attempted |

The running FastAPI instance predates these configuration edits and remains in connectivity-only mode. Its health response is not a live DB readiness result. The existing secure launcher also rejects partial configured settings: supply approved restricted DB access before restarting it. The root `/` 404 is expected; use `/health`. Emulator probes prove network reachability, not authenticated Android-to-API operation.

Evidence directory: `outputs/Stage05B1/Connection_Readiness/` contains presence-only local readiness, API status/selected nonsecret flags, ignore/DNS checks, emulator HTTP results and Android build/install evidence. Existing UI, navigation, Unity bridge, Mars scenes and React source were preserved. Unity export was not regenerated. No CLI installation/dependency reinstall was needed.

## Authentication dashboard prerequisites

Android intent filter matches `astrogateway://auth/callback`, and the SDK signup and password-reset requests use exactly that URL. Receiver expects same-installation PKCE code + encrypted pending verifier and rejects token-bearing URL fragments. Unity receives no Auth client/session.

**Remote redirect allowlist, email templates, SMTP settings and actual mail delivery have not been verified.** Public settings flags do not expose or prove them. A signed-in test-project dashboard/explicit operator confirmation is required.

Operator actions in the selected test project:
1. Open Authentication → URL Configuration → Redirect URLs and add exact `astrogateway://auth/callback`. Preserve the approved Site URL. The FastAPI origin is not an Auth callback endpoint.
2. Review Confirm signup and Reset password templates to preserve the provider verification link/ConfirmationURL and requested mobile redirect. A direct token_hash link is not supported by the current callback; actual PKCE code exchange must be tested on the originating installation.
3. Verify mail delivery to the two approved inboxes. Built-in Supabase SMTP restricts eligible addresses; use approved team inboxes or approved custom SMTP. Do not disable confirmation/admin-confirm accounts as evidence of real verification.
4. Record project session lifetime/refresh settings; do not assert a successful refresh from a local fixture or alter policies without approval.

[Supabase redirect guidance](https://supabase.com/docs/guides/auth/redirect-urls), [SMTP restrictions](https://supabase.com/docs/guides/auth/auth-smtp).

## Supabase CLI decision

CLI is not available on PATH; `supabase/config.toml` is absent in the Android host. No `supabase init` or `supabase link` ran. Initialization is not necessary to run this Auth SDK/FastAPI architecture. If CLI-managed migrations are later approved, inspect/choose a dedicated backend configuration location and initialize only there without overwriting project files. Authenticate locally and verify target before linking. Do not install a CLI, link or execute remote migrations merely to satisfy connectivity; none is performed here.

## Database migration readiness and security review

Existing project SQL matches the delivered migration exactly (SHA256 `269a983fe659e98f6083f3a8e46197ebe166d32e77eecc4a520539f1c686b59d`). Reviewed source; not executed on the remote database.

- Requires Supabase `auth.users` with id/created_at/raw_user_meta_data, anon/authenticated roles, `gen_random_uuid()` and authorized schema/role/function/table/trigger privileges. A plain unrelated PostgreSQL DB is not equivalent.
- Creates private `training` tables and NOLOGIN/NOBYPASSRLS API role. Grants API SELECT and limited mission/revocation INSERT/UPDATE; profile/training-level writes are excluded. Public/anon/authenticated have no private-schema access. Keep `training` unexposed in Data API.
- RLS matches verified canonical user and Auth session transaction context, restricts completion ownership/expiry and denies recorded revoked sessions. Profile trigger has fixed empty search_path and bounds untrusted display metadata; metadata cannot set a training level.
- Auth trigger runs on future signups: privilege/dependency/trigger failures can block signup. Existing users are backfilled. Check the approved test catalogue, existing trigger interactions and migration history before application.
- Script is deliberately not idempotent: role/schema/trigger/table collisions must be reviewed. No DROP statements. BEGIN/COMMIT permit rollback of an unsuccessful uncommitted migration, but rollback after successful commit requires a separately reviewed plan; do not drop populated objects blindly. Snapshot/back up the test target as appropriate before approval.
- Runtime DB role must not own tables, be superuser/BYPASSRLS or have elevated memberships. Startup checks this. Testing via postgres/admin would not demonstrate runtime RLS enforcement.

The supplied `postgres` account is **administrative only**, never FastAPI runtime. Keep its URI/password exclusively in the owner-only admin file or an approved secure DB client. No password has been obtained or generated. After explicit migration/provisioning approval, create a separate restricted login (planned name `astrogateway_runtime`) and store only its URI in server `DATABASE_URL`. Android and Unity never receive either DB credential.

Direct host DNS failed here; obtain the actual Session pooler host/port from astrogateway-test → Connect and verify its approved project reference. Shared session pooler uses `postgres.PROJECT_REF` for administration and `astrogateway_runtime.PROJECT_REF` for runtime, normally port 5432; use actual dashboard values rather than guessing region/host. Password characters must be URI-encoded and the whole value quoted in `.env`. Prefer session/direct mode for this long-lived API and connection-level startup role checks. TLS must remain enabled. [Official PostgreSQL connection guide](https://supabase.com/docs/guides/database/connecting-to-postgres).

No direct/pooler PostgreSQL authentication, remote catalogue, migration application or live RLS verification was performed. This is readiness review, not approval to mutate the DB.

## Exact next operator actions / stopping point

1. Add/confirm the exact mobile redirect and inspect verification/recovery templates and SMTP in astrogateway-test. Share the dashboard URL/reference or nonsecret confirmations only; never callback links with one-use codes.
2. Obtain the approved Session pooler endpoint or resolve direct DNS/connectivity. Choose user-assisted SQL Editor for read-only preflight, or securely populate `database/admin.local.env` with the admin DSN. Obtain the password locally from the operator using a secure editor/client; do not put it in chat, terminal arguments or logs. Report only that the file is ready.
3. Run/authorize only the metadata preflight from `outputs/Stage05B1/Supabase_Configuration/Read_Only_Database_Preflight.sql`. Confirm this project's objects/roles/trigger state. **Then request explicit approval for the exact migration and separate runtime-role provisioning before any modification.** No approval has been inferred.
4. Once restricted runtime role exists, securely fill backend `DATABASE_URL` with its own password, keeping admin credentials separate. Restart the test server through `backend/run_local.py` after all required settings exist; review actual TLS/role/health results.
5. Approve two disposable test identities and signup/reset/password-change operations, and provide inbox-opening access/method. Fill owner-only ignored `live.test-secrets.json` locally; no inbox password required for user-assisted links.
6. Only then execute signup → verification → login, wrong-password rejection, recovery, session refresh/expiry/restart/logout, profile persistence, two-user RLS, protected navigation and authenticated Unity message/return/relaunch. Inspect logs privately for sensitive output and publish sanitized evidence only.

Unity's existing version-1 handoff fields are version, launchId, missionId, missionSessionId and userId; an expiring lease stays with the native Activity. No access token, refresh token or password field is passed. Static inspection is preserved evidence, not a new authenticated runtime pass.

**Stopping before remote modification and credential-dependent validation.** All account/authenticated Unity/live DB tests remain blocked; prior mocked/local tests are not relabeled as live passes.


## Session pooler update — 2026-10-09

Approved pooler metadata saved in owner-only `database/admin.local.env`:
- Host: `aws-0-ap-southeast-2.pooler.supabase.com`
- Port: 5432; database: postgres
- Admin username: `postgres.jvkwofmxtxjhdwtwdboz`
- Requested SSL mode: require

**DNS and TCP passed.** PostgreSQL SSLRequest returned TLS support. Verified TLS handshake failed with certificate validation error 19, “self-signed certificate in certificate chain.” No credentials were transmitted and no PostgreSQL authentication occurred. Obtain the trusted Supabase CA certificate through the approved dashboard/operator channel and retry with certificate validation; do not treat encryption support alone as verified server identity. Evidence: `Connection_Readiness/session-pooler-probe.json`.

`backend/.env` retains the correct public provider settings; `DATABASE_URL` remains blank. The administrative postgres account was intentionally not assigned to FastAPI. Current FastAPI remains configuration_required until a separately approved restricted role and TLS-capable runtime connection exist. Presence/absence of remote `astrogateway_api` and `astrogateway_runtime` roles is **unknown**, not confirmed missing: no authenticated catalogue inspection was possible.

### Secure operator password entry

Run directly in your local Terminal:

```sh
/Users/kapilash/Projects/AstroGatewayAndroid/backend/.venv/bin/python /Users/kapilash/Projects/AstroGatewayAndroid/tools/configure_test_db_admin.py
```

The helper uses hidden `getpass`, refuses noninteractive input, checks the exact approved test pooler metadata, URI-encodes the password, and atomically saves only `DB_ADMIN_DSN` in the owner-only ignored admin file. It does not connect or execute SQL, never prints credentials and will not overwrite an existing admin URI. Do not place passwords in command arguments, chat, reports or Android properties. Helper syntax and noninteractive refusal were checked; the operator password prompt has not been completed.

### Remaining exact actions

1. Enter the database password using the local hidden prompt and report only that it is saved.
2. Obtain the trusted project/pooler CA certificate via Supabase's SSL configuration/dashboard and provide its local file path. Resolve certificate validation before any credential-bearing connection.
3. Authorize/read-only inspect the approved test catalogue after authentication succeeds: Auth dependencies, schema/tables/policies, role privilege/ownership/memberships and existing trigger/migration state. No DB modifications are authorized by this step.
4. Review the exact migration target and catalogue results; obtain **separate explicit approval** before applying the migration or creating a restricted runtime role.
5. Configure only the restricted role's URI in server DATABASE_URL, verify TLS and startup role checks, then restart FastAPI. Keep admin credentials separate.
6. Confirm `astrogateway://auth/callback` in the remote Redirect URLs list. It remains **unverified** without dashboard evidence. Email Auth/confirmation flags were previously observed through the public settings API; actual verification/recovery flow is still untested.
7. Approve two disposable accounts and inbox access before signup/recovery/RLS/authenticated Unity tests. None were created.

Migration review remains unchanged: transactional creation, non-idempotent object/role names, auth.users trigger affecting signup, backfill, restricted grants and user/session RLS. Confirm Supabase Auth dependencies and existing objects first. An uncommitted failed transaction can roll back; after successful commit any reversal needs a reviewed backup/cleanup plan and approval, not blind DROP commands. No migration or role/account creation was executed.

Android UI, Unity bridge, Mars scenes and React source remain unchanged. Stopped at credentials and TLS trust prerequisites.

Certificate download/verified SSL reference: [Supabase PostgreSQL SSL connection guide](https://supabase.com/docs/guides/database/psql). Download the root certificate from Database Settings → SSL Configuration and use it for verified hostname/trust checking. SSL require alone encrypts but does not verify server identity.


## Secure read-only verification — 2026-10-09 (supersedes password/CA blockers)

Saved private admin credentials were usable, matched the approved test pooler/reference and were never printed. No password was requested again. The exact supplied CA filename was initially absent; the directory contained the operator-downloaded `prod-ca-2021 (1).crt`. It identifies Supabase Root 2021 CA, is valid from 2021-04-28 through 2031-04-26 and loaded as a CA. After verified pooler chain/hostname testing, it was copied to the requested `backend/certs/supabase-ca.crt` path; original preserved. File SHA256: `700723581420dd1ac98fd7e9ac529f0ef210eadcaf87fc868a3ad7d114c2f3b7`. Local subject/file metadata alone does not independently attest download provenance; successful chain/hostname verification establishes compatibility with this endpoint using that operator-supplied trust anchor.

Python's newer X509_STRICT profile check rejected this legacy root's missing key-usage extension (error 92). The verification script used standard OpenSSL certificate validation by removing only VERIFY_X509_STRICT, while retaining CERT_REQUIRED, hostname checking, expiry/chain checks and trusted CA loading. No CERT_NONE or check_hostname=false was used. This compatibility adjustment is disclosed; no server certificate identity verification was bypassed. The FastAPI source was not modified.

Connection used an in-memory URI with `sslmode=verify-full`, the supplied `sslrootcert`, and an explicit verified SSLContext. Password stayed in private memory/configuration and was omitted from all result data. Actual authenticated asyncpg transport reported TLSv1.3 / TLS_AES_256_GCM_SHA384. Chain and hostname validation succeeded for `aws-0-ap-southeast-2.pooler.supabase.com`.

Read-only transaction results:

| Query/check | Actual result |
|---|---|
| PostgreSQL password authentication | PASS |
| SELECT 1 | 1 |
| current_user | postgres |
| current_database() | postgres |
| transaction_read_only | on |
| Client → Session pooler TLS | Active, verified TLSv1.3 |
| pg_stat_ssl for PostgreSQL backend | ssl=false; version/cipher null |
| astrogateway_api role | Not present |
| astrogateway_runtime role | Not present |
| training schema | Not present |
| Backend DATABASE_URL | Blank |

**TLS boundary:** pg_stat_ssl observes the database backend connection behind the pooler, not the authenticated client's TLS transport. It reports that backend leg as non-TLS. Client-to-pooler TLS is directly verified; end-to-end encryption of every pooler/database leg is not claimed. Investigate provider topology/SSL enforcement before representing the full path as encrypted; no SSL settings were changed.

Evidence: `Connection_Readiness/secure-database-verification.json` and `ca-discovery-verification.json`. All SQL was SELECT/SHOW inside a read-only transaction. The second connection only inspected its TLS transport and closed. No migration, role creation, account creation or database settings change occurred.

FastAPI runtime is blocked by the absent private schema/API/runtime roles and blank runtime URI. The admin postgres connection was used only for authorized read-only verification and was not assigned to FastAPI. Future runtime setup must load the approved root and address the legacy CA profile compatibility explicitly; do not disable chain or hostname verification. Redirect allowlist and actual email/Unity authentication flows remain unverified.

Next action requires separate explicit approval for reviewed migration and restricted-role provisioning, preceded by any further approved catalogue/dependency/trigger checks. No such approval is inferred. Stop after this read-only milestone; Android UI, Unity bridge, Mars scenes and React source are unchanged.
