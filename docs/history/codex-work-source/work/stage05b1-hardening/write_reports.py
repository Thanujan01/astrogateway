from pathlib import Path
import json,hashlib,shutil
base=Path.cwd(); out=base/'outputs/Stage05B1'; hard=out/'Hardening'; sql=hard/'SQL'; ev=hard/'Evidence'
r=json.loads((ev/'sql-rehearsal.json').read_text()); passed=sum(t['status']=='PASS' for t in r['tests']);failed=sum(t['status']=='FAIL' for t in r['tests'])
for name in ('rehearse.mjs','tls_fixtures.py'):
 shutil.copyfile(base/'work/stage05b1-hardening'/name,hard/name)
# A user-facing reproduction copy uses the pinned workspace lab import; work copy remains authoritative runner.
p=hard/'rehearse.mjs';p.write_text(p.read_text().replace("'./pg17/node_modules/@electric-sql/pglite/dist/index.js'",repr(str(base/'work/stage05b1-hardening/pg17/node_modules/@electric-sql/pglite/dist/index.js'))))
shutil.copyfile(base/'work/stage05b1-hardening/pg17/package-lock.json',hard/'pg17-package-lock.json')
files=['000_read_only_preflight.sql','001_identity_and_mission_sessions.sql','002_public_acl_isolation.sql','003_runtime_role.sql','004_post_migration_metadata.sql']
manifest={name:hashlib.sha256((sql/name).read_bytes()).hexdigest() for name in files}
(ev/'sql-manifest.json').write_text(json.dumps({'target':'jvkwofmxtxjhdwtwdboz','remote_authorized':False,'files':manifest},indent=2)+'\n')
source={name:hashlib.sha256((hard/'Source'/name).read_bytes()).hexdigest() for name in ('database.py','main.py','run_local.py','.env.example','tests/test_hardening.py')}
(ev/'source-after.json').write_text(json.dumps(source,indent=2)+'\n')
(out/'Security_Hardening_Report.md').write_text(f'''# Stage 05B.1 — Security hardening report

2026-10-09. **Overall: BLOCKED for remote execution.** Local preparation/rehearsal completed; the database-level impersonation requirement FAILS under runtime/backend compromise. No remote DB connection was made in this stage, no remote SQL/roles/privileges/accounts changed, and no live Auth validation is claimed.

## Local changes

Modified only the Android project's FastAPI backend:
- `/Users/kapilash/Projects/AstroGatewayAndroid/backend/main.py`: issuer locked to configured Supabase project, duplicate JWT JSON keys rejected, bearer bounded at verifier as well as HTTP boundary, integer expiry and not-before checks, controlled claim failures; explicit TLS settings passed to pool.
- `/Users/kapilash/Projects/AstroGatewayAndroid/backend/database.py`: verified SSLContext and fail-closed runtime-role boundary checks.
- `/Users/kapilash/Projects/AstroGatewayAndroid/backend/run_local.py`: accepts the two documented TLS settings in its owner-only environment parser.
- `/Users/kapilash/Projects/AstroGatewayAndroid/backend/.env.example`: safe TLS template, no secrets.
- `/Users/kapilash/Projects/AstroGatewayAndroid/backend/tests/test_hardening.py`: additional local security tests.

Copies of new source and prior source are in `{hard/'Source'}` and `{hard/'Before'}`; hashes are in `{ev/'source-after.json'}` and `{ev/'source-before.json'}`. Existing private credential files were not rewritten, shown or committed. Existing original migration files were not replaced; the reviewed candidate SQL is separately under `{sql}`. No Android/Kotlin/UI/Unity/Mars/React source changed. No production endpoint or database was contacted. The running local server was not restarted against the remote database.

## Verified identity boundary — PASS for tested HTTP contract, FAIL under DB compromise

Supabase remains the single issuer. FastAPI submits the bearer token over verified HTTPS to the configured `/auth/v1/user`, with redirects disabled, and rejects failure before constructing a Principal. Decoding is not authentication. Subject must equal the verified provider user UUID; issuer, authenticated role/audience, expiry, session UUID and verified email are checked. Owner fields are not accepted in mission request bodies (extra fields forbidden). Repository SQL is parameterized and uses only the verified Principal. No passwords or refresh tokens enter this API. Supported server verification follows [Supabase getUser](https://supabase.com/docs/reference/javascript/auth-getuser).

Transaction-local role/user/session settings keep ordinary A requests scoped to A, and commit/rollback clears that context in the rehearsal. **The database does not validate JWT signatures.** In the actual local compromise test, runtime changed A context to B and read B's profile without B's JWT. That control is recorded **FAIL**, not disguised as a successful negative test. A compromised FastAPI process or stolen runtime credential with arbitrary SQL can impersonate users and assert arbitrary session identifiers, including evading revocation by choosing an unrevoked SID. JWT validation at the HTTP boundary does not fix this database boundary.

This is a **blocking security risk** under the requested compromise model. A follow-up design must keep identity verification/signing authority outside the compromised runtime's control, or use a separately trusted token-verifying gateway with no equivalent raw DB credential available to the host. Mere RPCs taking a user UUID, extra GUC names, UUID type checks or an HMAC secret colocated in the compromised backend do not solve it. No unreviewed database JWT cryptography or second identity provider was added. Do not authorize remote execution as “impersonation resolved.”

## Permissions — PASS in fixture, provider compatibility BLOCKED

Candidate `001` adds explicit NOREPLICATION to the original API role; other baseline objects and RLS policy logic remain intact. Candidate `002` replaces existing PUBLIC privileges on unrelated application schemas and database TEMPORARY with equivalent named grants to existing non-system roles, excluding API/runtime. It leaves pg_catalog/information_schema and PostgreSQL internal schemas as necessary SQL metadata exceptions. Existing table/function grants behind denied schemas are not broadened. Runtime cannot use Auth/storage/public schemas or invoke the fixture public definer routine.

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

Local backend: **54 PASS**. SQL on PostgreSQL17.5 WASM: **{passed} PASS, {failed} FAIL**, with the failure being demonstrated compromise impersonation. No live acceptance claimed. See `{out/'Local_Rehearsal_Results.md'}` for reproduction, scope and blocked checks. Remote execution is gated on removing the identity blocker, provider ACL/default compatibility, backup/recovery readiness, actual runtime TLS validation and separate explicit operator approval of a revised manifest. This stage stops here.
''')
(out/'Local_Rehearsal_Results.md').write_text(f'''# Stage 05B.1 — Local rehearsal results

2026-10-09. **Local-only evidence. Overall remote readiness BLOCKED.**

## Environment and provenance

Final SQL engine: `{r['engine']}`; pinned PGlite0.4.6 in isolated workspace lab. Target historical server is PG17.11; major matches, patch/platform/extension/service stack do not. An exploratory run on the previously installed PG18.3 was retained separately and is not the final PG17 acceptance evidence. The [upstream changelog](https://github.com/electric-sql/pglite/blob/main/packages/pglite/CHANGELOG.md) identifies engine changes; actual SELECT version() is recorded in final JSON.

Database was in-memory/disposable, closed after execution; no TCP listener, remote DSN, administrative credential file or live Supabase user was used by the SQL runner. Minimal Auth/service roles and auth.users are explicit fixtures. A/B are invented **local fixture UUIDs**, never live accounts. Session authorization is simulated from an initial local superuser to enforce runtime role checks; returning to administrator is a harness operation. Native LOGIN/password authentication and initial-session-user restrictions are not proven by this setup. No system-wide PostgreSQL installation was made. One pinned test dependency was installed only in the workspace to match PG17; existing project dependencies were not replaced.

## Results

| Check | Result | Evidence / limit |
|---|---|---|
| FastAPI original security contracts + new hardening tests | PASS — 54 tests | `{ev/'project-backend-tests.log'}`; mock Auth/repository, no live Supabase |
| Signed HMAC fixture accepted, altered signature rejected | PASS | Synthetic Auth transport verifies a real local fixture HMAC; never a deployed signing key or live verification |
| Extra owner/score fields, wrong issuer/sub/audience/role, invalid/expired/not-before claims, malformed/duplicate claims | PASS for covered contract cases | Same test log; unimplemented live flows not inferred |
| Strict local TLS chain/hostname positive and wrong-host/untrusted-chain negative handshakes | PASS | Real OpenSSL TLS via memory BIO; no DB transport implied |
| Unsafe TLS DSN/admin login/missing CA/legacy wrong fingerprint | PASS — rejected | Same tests; strict default and scoped pinned exception checked |
| Startup unsafe role flags/owner/membership/schema/create guard | PASS — mocked guard cases | Actual guard-to-network pool untested |
| Candidate preflight + migration + ACL + runtime provisioning + metadata | PASS | `{ev/'sql-rehearsal.json'}` and `.log` |
| A/B own reads and mission initialization/completion | PASS | Actual PostgreSQL/RLS fixture queries |
| Cross-account reads/inserts/completion, profile escalation, delete/truncate/owner/lease edits | PASS — denied/zero rows | Actual PostgreSQL permission/RLS checks |
| Auth table access, unrelated table/definer routine, schema/temporary object/role creation, role grants and admin SET ROLE | PASS — denied | Restricted session role, no native TCP login |
| anon/authenticated cannot access private training | PASS | Fixture roles |
| Fixture Auth admin insert and default profile creation | PASS | Preserved local provider analogue; not full Supabase service proof |
| Future owner functions/tables do not automatically grant API execution/read | PASS | Global current-owner function defaults; not all future creators |
| No identity after commits/error rollback | PASS | Single embedded session reuse; not asyncpg pool cancellation/concurrency test |
| Revoked SID denies profile; other SID remains permitted | PASS | Fixture SID claims, no signed JWT binding inside DB |
| Runtime changing its own GUC context to B | **FAIL — blocking impersonation observed** | B profile returned without B JWT; expected demonstration, still a failed required security property |
| ACL SQL repeat | PASS | Repeat runs successfully |
| 001/003 repeat on installed state | PASS for fail-closed collision requirement | Intentionally non-idempotent; error42710, rollback, data preserved |
| Existing-role collision / late trigger collision / null-created_at backfill failure | PASS — transaction atomicity verified | No partial training schema/API role after rollback |
| Local down, fixture ACL/default recovery, Auth user retention, clean reinstall | PASS | Local-specific effective-privilege recovery, not bitwise remote ACL restoration or provider recovery acceptance |
| Runtime pooler login and full-path TLS | BLOCKED / unexecuted | Remote changes forbidden, runtime role absent; no end-to-end proof |
| Native PostgreSQL TCP login/pool reset/cancellation/concurrency | BLOCKED / unexecuted | Embedded single-process lab cannot establish these |
| Full provider Auth/storage/PostgREST/realtime/maintenance compatibility | BLOCKED / unexecuted | Minimal fixture only |
| Redirect allowlist/live email/signup/recovery/session-refresh/Android→Unity | BLOCKED / unexecuted | No live accounts/dashboard evidence; no live tests in scope |

Final SQL count: **{passed} PASS / {failed} FAIL**. Blocks above are separately listed, not fabricated as executed SQL assertions. One backend test warning is Starlette/httpx TestClient deprecation; no application crash/ANR claim is made. Evidence retains an initial harness error: RESET SESSION AUTHORIZATION did not restore the administrator as assumed in the embedded engine; harness now explicitly restores its original administrator between cases. Final PG17 run completed; that harness correction is not a product permission change.

## Exact reproduction

Run from `{base}`. Tools/dependencies are local test tools, never remote deployment runners:
```
node work/stage05b1-hardening/rehearse.mjs
work/stage05b/venv/bin/python work/stage05b1-hardening/tls_fixtures.py
PYTHONPATH=/Users/kapilash/Projects/AstroGatewayAndroid/backend PYTHONDONTWRITEBYTECODE=1 work/stage05b/venv/bin/python -m pytest -p no:cacheprovider /Users/kapilash/Projects/AstroGatewayAndroid/backend/tests -q
```
Runner SQL entrypoints are explicit files; no network/remote credential loading occurs. Regenerating local TLS fixtures produces new disposable keys only under private work storage, not deliverables. Capture pytest output to a local evidence log if re-running; never run live tokens through command arguments. A user-facing runner copy is `{hard/'rehearse.mjs'}`; its pinned lab module path assumes this workspace. Package integrity lock: `{hard/'pg17-package-lock.json'}`.

## Impersonation reproduction (disposable ONLY)

As simulated runtime, BEGIN→SET LOCAL ROLE astrogateway_api→set A user/SID→overwrite the same two custom settings with B user/SID→SELECT training.profiles. One B row is returned. Commit resets context, but it cannot undo disclosure. These settings are SQL caller assertions; lack of BYPASSRLS does not authenticate them. Do not run this against real accounts or use the result to claim compromised-backend isolation passed.
''')
rows='\n'.join(f'| {i} | `{sql/name}` | `{manifest[name]}` | '+('Read-only' if i in (0,4) else 'Writes; local rehearsal only')+' |' for i,name in enumerate(files))
(out/'Final_SQL_Manifest.md').write_text(f'''# Stage 05B.1 — Final review SQL manifest

2026-10-09. **Final artifact list for this preparation stage; NOT approved/cleared for remote execution.** Identity-compromise protection FAILS; provider-wide ACL/default compatibility, native runtime connection and recovery prerequisites remain BLOCKED. “Final” pins review bytes, not permission or a safety certification.

Approved future target only: `astrogateway-test`, ref `jvkwofmxtxjhdwtwdboz`. Administrative postgres credentials are server/operator-only; restricted runtime must never borrow them. SQL does not independently prove project reference (many projects use database postgres): any future runner must verify approved endpoint/ref/TLS identity before executing. No remote runner was started.

## Intended candidate order and SHA256

| Order | Exact file | SHA256 | Status |
|---|---|---|---|
{rows}

Machine-readable hashes: `{ev/'sql-manifest.json'}`. Both the original migration and project copy remain unchanged. Candidate001 differs only by explicit NOREPLICATION; it preserves the baseline policies and therefore the documented database impersonation defect. A design that resolves the defect will require new SQL/test evidence/hashes and a **new review manifest**, not silent replacement.

000 is a metadata preflight, not a backup/effective-privilege proof. Refresh inventory of every schema/default grant/grantor/provider role/trigger/object and null Auth timestamps; validate collisions before001.001 is transactional CREATE, no overwrite/IF NOT EXISTS; profile trigger/backfill could abort signup if incompatible.002 is transactional PUBLIC isolation and owner-global function defaults; inspect its affected roles/schemas/defaults first.003 rejects remaining unrelated PUBLIC schema usage and PUBLIC database TEMP; creates runtime with no password/admin rights; explicit SET-only API membership.004 is read-only metadata inspection; catalogue output still needs assertion review and real user/native connection tests.

**Never run 001/002/003 merely because local tests passed.** Execute neither live signup nor migrations concurrently. Never wrap001 inside a BEGIN→ROLLBACK remote “dry run”: it contains COMMIT. Establish bounded locks/statement timeouts outside scripts, stop on any error, no unattended retry after unknown commit.

## Explicit exclusions

`{sql/'900_local_fixture_only.sql'}` creates fake provider/Auth objects and is **local-only, NEVER Supabase**. `{sql/'910_local_down_only.sql'}` removes application objects in dependency order only in the disposable fixture, and does not restore real provider ACL/default baselines; **NEVER use it as a remote down migration**. Rehearsal code and fixture users are not deployment assets/accounts.

Original `outputs/Stage05B/database/001_identity_and_mission_sessions.sql` and older runtime drafts are superseded only as *review candidates* by this manifest; do not mix old/new bytes. No copy was applied to the remote database or project migration source.

## Required next gates and approval

1. Resolve database-level impersonation under backend/runtime compromise, then re-run negative tests. It is blocking, not resolved by server JWT validation or an approval to provision a role.
2. Review current provider-specific privilege inventory and named-role/default preservation against all services; isolated provider-stack/native PG17 testing required. Existing local engine17.5 does not certify remote17.11 patch/service behaviour.
3. Approve runtime trust configuration/compliant CA or pinned legacy profile exception; verify runtime TCP hostname/chain, credential separation and pool resets; maintain full-path TLS limitation until separately evidenced.
4. Establish private recoverable backups and a target-specific ACL/default/membership restoration manifest. No remote backup or restoration proof exists in this stage.
5. Obtain separate **explicit operator approval** naming the non-production reference, revised file hashes/order, roles/privilege changes and rollback scope before any remote change. Live account creation/email tests need their own approval/inbox access. Current authorization covers local preparation only.

Remote command examples/password-bearing SQL intentionally omitted. Runtime password must be generated/set through an approved hidden operator channel after separate provisioning approval, stored only in private backend configuration; Android/Unity contain no DB or long-lived credential. No migration command is embedded in FastAPI startup.
''')
(out/'Rollback_And_Recovery_Plan.md').write_text(f'''# Stage 05B.1 — Rollback and recovery plan

2026-10-09. **No remote changes occurred; remote rollback is not authorized.** Local recovery rehearsed; production/test-provider recovery readiness remains BLOCKED. Target any future separately approved execution only to `jvkwofmxtxjhdwtwdboz`.

## Local evidence and source recovery

SQL rehearsal applied candidate001→002→003 in a disposable PG17.5 fixture, tested collisions and late trigger/backfill failures, removed application objects using `{sql/'910_local_down_only.sql'}`, restored schema privileges from captured schema ACLs and TEMP/function-default privileges from known fixture baseline rules, preserved all fixture Auth rows, then reinstalled successfully. This proves the exercised local order and transaction handling, not provider recovery. Initial and final logs are in `{ev}`.

FastAPI prior source copies/hashes: `{hard/'Before'}` and `{ev/'source-before.json'}`. After stopping the local server, restore only the four prior backend source/template files if an operator chooses code rollback; remove the newly added hardening test only if reverting that stage. Keep existing secrets unchanged. Retest original contracts. Restoring old database.py restores the former weaker TLS/startup checks, so it is recovery to baseline, not a recommended secure deployment. No Android/Unity/Mars/React rollback needed because those sources were untouched.

## Before any future remote write

Require separately approved, private provider snapshot/PITR or database backup with successful restoration evidence and retention. Do not place Auth rows/password hashes, role-password dumps, DSNs or tokens in outputs. Capture exact schema/table/function/trigger/policy ownership and definitions, schema/database/table/column/routine grants with grantors/options, role attributes/memberships, owner-specific global/schema default ACLs, collision state and data counts. Use least-data access; all current approval is local, not remote backup administration.

002 converts PUBLIC grants into named-role grants and changes owner-global function execution defaults. Record **full prior and resulting ACL/default state**, including preexisting named grants; otherwise one cannot distinguish existing grants from added grants on rollback. Role additions after the snapshot and changes by other operators are conflicts requiring fresh review. A simple GRANT PUBLIC or blanket REVOKE is not an exact baseline restoration. Native provider recovery must compare effective access for Auth/PostgREST/storage/realtime/maintenance users and application roles.

## Failure handling

- **Before COMMIT:** stop on first error, explicitly ROLLBACK. A transaction can leave a session aborted until rolled back. Local early-role and late-trigger/backfill failure tests left no partial training schema/API role. Do not continue selected statements or hide collisions.
- **Unknown commit/network loss:** stop API/signup writes; reconnect only with verified admin TLS under read-only inspection; compare object hashes/catalogue/data and the execution journal. Do not rerun CREATE scripts blindly. Request approval for a diagnosed recovery action.
- **001 committed, later stage failed:** do not place postgres credentials in runtime or skip its isolation guard. Keep app runtime disabled; retain data for diagnosis; choose reviewed roll-forward or precise rollback after separate approval.
- **Trigger failure affects signup:** pause signup and review disabling/removing only `astrogateway_auth_user_profile` under approved rollback. Existing Auth users must not be deleted as cleanup. Restoration/backfill requires separately approved reviewed SQL and data checks.

## Post-commit remote rollback — procedure to finalize, not executable SQL

After separate explicit approval: stop runtime connections and signup/API writes; snapshot resulting data privately. Disable only the dedicated runtime login and terminate only its approved sessions. Remove the specific newly installed Auth trigger first; preserve Auth users/provider triggers. Remove mission/policy/helper/profile objects in known dependency order only when retention decision and exact ownership/version match. Revoke dedicated membership/settings/CONNECT and drop dedicated roles only once dependencies are removed. Check unexpected dependencies and stop; do not DROP SCHEMA CASCADE, DROP OWNED or revoke provider roles indiscriminately.

Restore ACL/default/membership baselines from the target-specific pre-change snapshot. Preserve preexisting named grants; restore default ACLs for the exact owner/schema/object type. Review unrelated objects created after002 because global defaults affected them: restoring defaults alone does not fix their already materialized ACLs. Compare grants with saved baseline, inspect effective privileges and test provider services. Auth rows are preserved; any account/data deletion needs separate explicit approval.

If exact restoration is unavailable, request approved provider snapshot restore or recreate **only** the designated disposable/non-production target after retention review. Neither operation is authorized by this plan. Never touch production.

## Acceptance and stop gates

Verify schema/role/trigger/ACL/default definitions match the chosen baseline; Auth/profile insert functionality and A/B isolation in the restored/repaired design; target identity, certificate/hostname verification and role separation; no new system/API permission escalation. Restore runtime traffic only after successful tests and operator approval. Publish sanitized counts/results, never credentials/backup contents. Full-path TLS remains unestablished unless independently verified. No actual remote recovery test or approved target-specific down SQL exists yet; these remain blockers.
''')
print('Four requested reports and pinned candidate manifest written.')
