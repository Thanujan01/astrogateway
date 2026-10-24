# Stage 05B.1 — Security hardening report

2026-10-09. **Overall: BLOCKED for remote execution.** Local preparation/rehearsal completed; the database-level impersonation requirement FAILS under runtime/backend compromise. No remote DB connection was made in this stage, no remote SQL/roles/privileges/accounts changed, and no live Auth validation is claimed.

## Local changes

Modified only the Android project's FastAPI backend:
- `/Users/kapilash/Projects/AstroGatewayAndroid/backend/main.py`: issuer locked to configured Supabase project, duplicate JWT JSON keys rejected, bearer bounded at verifier as well as HTTP boundary, integer expiry and not-before checks, controlled claim failures; explicit TLS settings passed to pool.
- `/Users/kapilash/Projects/AstroGatewayAndroid/backend/database.py`: verified SSLContext and fail-closed runtime-role boundary checks.
- `/Users/kapilash/Projects/AstroGatewayAndroid/backend/run_local.py`: accepts the two documented TLS settings in its owner-only environment parser.
- `/Users/kapilash/Projects/AstroGatewayAndroid/backend/.env.example`: safe TLS template, no secrets.
- `/Users/kapilash/Projects/AstroGatewayAndroid/backend/tests/test_hardening.py`: additional local security tests.

Copies of new source and prior source are in `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B1/Hardening/Source` and `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B1/Hardening/Before`; hashes are in `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B1/Hardening/Evidence/source-after.json` and `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B1/Hardening/Evidence/source-before.json`. Existing private credential files were not rewritten, shown or committed. Existing original migration files were not replaced; the reviewed candidate SQL is separately under `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B1/Hardening/SQL`. No Android/Kotlin/UI/Unity/Mars/React source changed. No production endpoint or database was contacted. The running local server was not restarted against the remote database.

## Verified identity boundary — PASS for tested HTTP contract, FAIL under DB compromise

Supabase remains the single issuer. FastAPI submits the bearer token over verified HTTPS to the configured `/auth/v1/user`, with redirects disabled, and rejects failure before constructing a Principal. Decoding is not authentication. Subject must equal the verified provider user UUID; issuer, authenticated role/audience, expiry, session UUID and verified email are checked. Owner fields are not accepted in mission request bodies (extra fields forbidden). Repository SQL is parameterized and uses only the verified Principal. No passwords or refresh tokens enter this API. Supported server verification follows [Supabase getUser](https://supabase.com/docs/reference/javascript/auth-getuser).

Transaction-local role/user/session settings keep ordinary A requests scoped to A, and commit/rollback clears that context in the rehearsal. **The database does not validate JWT signatures.** In the actual local compromise test, runtime changed A context to B and read B's profile without B's JWT. That control is recorded **FAIL**, not disguised as a successful negative test. A compromised FastAPI process or stolen runtime credential with arbitrary SQL can impersonate users and assert arbitrary session identifiers, including evading revocation by choosing an unrevoked SID. JWT validation at the HTTP boundary does not fix this database boundary.

This is a **blocking security risk** under the requested compromise model. A follow-up design must keep identity verification/signing authority outside the compromised runtime's control, or use a separately trusted token-verifying gateway with no equivalent raw DB credential available to the host. Mere RPCs taking a user UUID, extra GUC names, UUID type checks or an HMAC secret colocated in the compromised backend do not solve it. No unreviewed database JWT cryptography or second identity provider was added. Do not authorize remote execution as “impersonation resolved.”

## Permissions — PASS in fixture, provider compatibility BLOCKED

Candidate `001` adds explicit NOREPLICATION to the original API role; other baseline objects and RLS policy logic remain intact. Candidate `002` replaces existing PUBLIC privileges on unrelated application schemas and database TEMPORARY with equivalent named grants to existing non-system roles, excluding API/runtime. It leaves pg_catalog/information_schema and PostgreSQL internal schemas as necessary SQL metadata exceptions. Existing table/function grants behind denied schemas are not broadened. The internal prefix check uses literal `pg_`, not a LIKE underscore wildcard; a lookalike `pgcustom` schema was explicitly denied in rehearsal. Runtime cannot use Auth/storage/public schemas or invoke the fixture public definer routine.

Named-role preservation maintains the fixture Auth administrator's INSERT ability and profile trigger; it is **not proof of full Supabase service compatibility**. The algorithm must have its exact affected schemas, recipient roles, grantors and default ACLs reviewed against a fresh provider inventory. It preserves access equivalent to existing PUBLIC grants for current roles, not new future roles. It does not silently promise future provider roles access. PostgreSQL schema privilege behaviour is documented in [PG17 schemas](https://www.postgresql.org/docs/17/ddl-schemas.html).

Future owner function EXECUTE defaults require a **global owner-level** PUBLIC revoke; a per-schema revoke cannot remove the built-in global PUBLIC default. Candidate `002` preserves execution defaults for current named non-application roles; shared migration owner's future routines in other schemas are affected. Review this provider-wide effect before any remote execution. New training tables/functions grant no access automatically to API/runtime; explicit reviewed grants remain necessary. Only the executing migration owner's default ACL is changed; other creators/default ACLs require inventory, and no blanket claim about all future creators is made.

Runtime is LOGIN/NOINHERIT/NOSUPERUSER/NOCREATEDB/NOCREATEROLE/NOREPLICATION/NOBYPASSRLS, non-owner, with CONNECT and API membership only: ADMIN=false, INHERIT=false, SET=true. It cannot inherit Auth/admin/provider roles. API remains NOLOGIN and non-owner. Startup rejects administrative attributes, unexpected membership options/API parent roles, object ownership, database CREATE/TEMP, unrelated schema access, training CREATE, and missing expected RLS tables. This is a fail-closed architecture check, not a cryptographic identity check or exhaustive substitute for ACL/policy inventory. Its Python guard tests use a mock connection; real SQL privileges are separately rehearsed in PostgreSQL.

## TLS — PASS locally; remote runtime/full path BLOCKED

Runtime now passes an explicit SSLContext to asyncpg: trusted CA file required; CERT_REQUIRED; hostname verification on; minimum TLS1.2; strict X509 profile default. Non-runtime/admin usernames, socket-only endpoints, missing CA and DSN SSL overrides other than one `sslmode=verify-full` are rejected. No prefer/allow/disable/plaintext fallback. The installed asyncpg implementation treats an SSLContext as required TLS and does not use its advisory prefer retry. Driver exceptions are sanitized, never surfaced with a DSN/password.

Two new settings, in the existing private backend environment only:
```
DATABASE_SSL_ROOT_CERT=/Users/kapilash/Projects/AstroGatewayAndroid/backend/certs/supabase-ca.crt
DATABASE_ALLOW_LEGACY_CA=false
```
The template documents them; existing private `.env` was left unchanged. Enable the compatibility flag only after explicit certificate-profile review, or obtain a compliant provider root. The reviewed Root2021 public certificate lacks the key-usage extension required by strict OpenSSL profile validation. An opt-in exception removes VERIFY_X509_STRICT only for that exact previously reviewed CA file fingerprint; CERT_REQUIRED/hostname verification remain. It relaxes more certificate-profile checks than the one observed missing extension and is not strict-profile compliance. This is a local context exception, not a global trust change. An unexpected CA fingerprint refuses the exception.

Real local OpenSSL memory-BIO handshakes accepted the disposable approved chain/localhost SAN and rejected wrong hostname and an untrusted chain. This proves the configured local TLS controls, not a successful PostgreSQL pooler connection. Prior client→pooler TLS1.3/chain/hostname verification remains historical evidence. Backend `pg_stat_ssl=false` was previously observed. **Full-path TLS is not established.** No new remote runtime connection, pooler→DB enforcement proof, or CA-exception live handshake ran here.

## Android callback inspection — source reviewed, live flow BLOCKED

Reviewed manifest, AuthCoordinator, MainActivity, SecureSessionStore and bridge sources. Expected URI is `astrogateway://auth/callback`; manifest restricts scheme/host/path. Handler rejects other scheme/host/path and all fragments, requires bounded nonempty code, a locally pending verification/recovery flow and encrypted PKCE verifier, then asks Supabase to exchange the code. It does not import callback access/refresh tokens. Provider SDK logging is NONE. MainActivity removes URI data from incoming intent and does not save callback URI in instance state. Session and verifier use AES-GCM/Android Keystore in no-backup storage; no password serialization. Recovery stays outside authenticated dashboard until completed.

Remaining limits: source inspection is not Android runtime callback evidence; additional query parameters and authority user-info/port are not explicitly rejected by this handler (they are not used as token/redirect sources). Custom schemes can be claimed by another app; PKCE binds a code to the initiating verifier but does not prevent interception/denial of service. A later reviewed App Links design can improve routing; not implemented here. Pending flow expires after one hour, so lost/expired verifier/cross-device recovery needs a new same-device request. Do not print real callback URLs/codes/tokens in ADB/logcat.

Remote Supabase redirect allowlist, SMTP/templates and real verification/recovery delivery remain **unverified**. No dashboard change, live accounts, Android rebuild or authenticated Unity cycle was performed in this preparation stage. Unity bridge unchanged; source protocol still contains bounded mission/session/launch/user identifiers, not credentials.

## Outcome and stop

Local backend: **54 PASS**. SQL on PostgreSQL17.5 WASM: **55 PASS, 1 FAIL**, with the failure being demonstrated compromise impersonation. No live acceptance claimed. See `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B1/Local_Rehearsal_Results.md` for reproduction, scope and blocked checks. Remote execution is gated on removing the identity blocker, provider ACL/default compatibility, backup/recovery readiness, actual runtime TLS validation and separate explicit operator approval of a revised manifest. This stage stops here.
