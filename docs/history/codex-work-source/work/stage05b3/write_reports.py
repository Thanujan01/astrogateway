from pathlib import Path
import json,hashlib,shutil
root=Path.cwd();out=root/'outputs/Stage05B3';e=out/'Evidence';iso=out/'Isolation';lab=root/'work/stage05b3'
for n in ('launch.py','prepare_isolation.py','service_health.py','test_isolated_api.py','prepare_runtime_tests.py'):shutil.copy2(lab/n,iso/n)
shutil.copy2(lab/'runtime.sb',iso/'runtime.sb')
for n in ('probe.py','integration_probe.py','serve.py','unit_probe.py'):shutil.copy2(lab/'runtime'/n,iso/n)
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
manifest={'date':'2026-10-09','remote_operations':False,'review_artifacts':{p.name:sha(p) for p in iso.iterdir() if p.is_file()},'sql_manifest_unchanged':json.loads((root/'outputs/Stage05B2/Evidence/revised-sql-manifest.json').read_text())}
(e/'artifact-manifest.json').write_text(json.dumps(manifest,indent=2))
pre=root/'outputs/Stage05B2/SQL'
(out/'Stage05B3_Service_Isolation_Report.md').write_text(f'''# Stage05B.3 — Service isolation report
2026-10-09. **PASS: tested local runtime confinement. BLOCKED: hosted deployment readiness.** No remote SQL, API validation calls, migrations, roles or accounts. No system account or system service was created. Android, Unity/Mars and React source and private operator configuration were preserved. All four Stage05B.2 reports and SQL candidates were reviewed.

## Actual boundary implemented
The current Mac has sandbox-exec, but no Docker/Podman executable. The isolated Python/FastAPI process remains UID501; equivalent enforcement is macOS kernel sandbox confinement, not a different UID or chmod claim. Its descendants inherit confinement. An unsandboxed same-UID service still has the original exposure and remains prohibited. Start only through the fixed operator-owned launcher; the backend's diagnostic guard was not removed.

Authoritative local launcher: `{lab/'launch.py'}`. Kernel profile: `{lab/'runtime.sb'}`. Actual app source staged at `{lab/'runtime'}` is byte-identical to original main.py/database.py, recorded in `{e/'staged-source.json'}`. The operator's administration tool files remain in their private location; they are neither mounted nor copied into the staged runtime. No private .env, database directory, gateway password or signing key is staged. Review copies under `{iso}` require path adaptation, not automatic deployment.

Default deny; only staged code/dependencies/system runtime reads, scratch writes, local gateway outbound TCP and local health socket are granted. User home/Volumes/temporary user directories and system keychains are excluded except exact staged/venv/scratch reads. General file metadata is visible; secret file CONTENT is denied. System-readable files are allowed for interpreter startup, so no administration secrets may be stored in those system-readable locations or dependencies. Only macOS notification-center lookup is granted; broad helper IPC, process inspection/signalling and privileged execution are not granted. The local gateway permission is a loopback PORT boundary, not executable authentication: a malicious process occupying that port is outside the trusted local-lab assumption. Production must use approved HTTPS origin verification.

Launcher builds a new environment (PATH/HOME/TMPDIR/LANG, Python user-site/cache controls); it never merges parent env or passes private configuration. stdin is empty except deliberately supplied local user-bearer test fixture JSON; close_fds defaults true. Parent canary values were injected into DB/admin/service-role/DYLD/PYTHONPATH variables and absent inside the actual runtime. No secret values appear in reports. The actual health service uses DEVNULL stdin/outputs, cleared environment and closed inherited FDs. Client sockets/token protocol in integration fixture use original bearers only.

## Verification
18 probes PASS: real saved admin/backend config and lab signer/gateway/config content denied, child cat denied, symlink into admin denied, nested sandbox cannot widen, parent environment inspection and signalling denied, code/policy writes denied, sudo unavailable, non-gateway/remote outbound TCP denied, poisoned environment absent, real app imports. `{e/'isolation-probe.json'}` identifies actual UID and observed outcomes. The staged real application's lifespan + HTTP health succeeded in connectivity-only mode (`configuration_required`), then stopped. This is not Supabase authentication evidence.

48 backend unit tests passed both outside and inside confinement; 16 isolated real-gateway API checks passed. Operator-side native database harness remains deliberately outside confinement to administer/sign the disposable fixtures; it is never the runtime. No real admin password was read or handed to that service.

## Limits / deployment gate
This macOS profile is a locally tested development boundary, not a supported hosted container/systemd deployment or a claim against kernel vulnerabilities. No permanent launchd/service account installation was made. The trusted operator can change files/policy or launch unsandboxed code; the compromised confined child cannot. Independent service setup on the chosen host must repeat actual identity tests: immutable code/dependencies, non-root isolated UID, no host/project/operator mounts, no container/SSH/admin socket, minimal network/IPC, read-only root + bounded scratch, no inherited privileged FDs/env. Runtime can steal/replay valid user JWTs it observes until expiry/revocation. A universal backend-compromise immunity claim remains false.
''')
(out/'Stage05B3_Security_Test_Results.md').write_text(f'''# Stage05B.3 — Security test results
2026-10-09. Local only. No final executed assertion failed.

| Suite | Final result | Evidence |
|---|---:|---|
| Backend contracts/repository/JWT/configuration | 48 PASS | `{e/'backend-unit.log'}` |
| Same backend suite inside kernel confinement | 48 PASS, repeated suite, not additional unique coverage | `{e/'isolated-backend-unit.log'}` |
| Native PostgreSQL17.11 + PostgREST16.4 attacks | 53 PASS | `{e/'security-regression.json'}` and native-regression.log |
| FastAPI inside sandbox → real local PostgREST | 16 PASS | `{e/'isolated-api-integration.json'}` and isolated-api.log |
| Local collision/rollback/fresh reinstall | 8 PASS | `{e/'repeat-rehearsal.json'}` |
| Actual runtime boundary | 18 PASS | `{e/'isolation-probe.json'}` |
| Actual application lifespan/HTTP health | PASS, connectivity-only | `{e/'isolated-service-health.json'}` |
| Strict local native database TLS | PASS | `{e/'local-tls.json'}` |

The previous 125 functional/security checks were repeated, plus 18 isolation checks. Do not count duplicate backend runs twice. TestClient has one existing Starlette/httpx deprecation warning; no unnecessary dependency rebuild/install.

## Attack coverage and residuals
Invalid/expired/unsigned/forged JWTs, changed signed subject, wrong issuer/audience/role/SID, malicious identity headers/RPC arguments, B access to A missions, fake internal Principal B carrying token A, arbitrary PostgreSQL identity GUCs, SET ROLE/DDL/RLS disable, owner/level/expiry column writes, raw stolen retired password, direct private schema/set_config RPC, pooled A/B isolation and revocation all tested. Gateway independently verifies RSA signature; host verifier bypass cannot mint B authority. Runtime holds no raw DB credential. Native attacker may set GUCs but lacks data/membership privileges. Privileged native tests are performed by operator harness, not by passing admin credentials into confined runtime.

Valid B bearer replay succeeded as the explicitly expected bearer-token residual; that test is not proof replay is prevented. Logout revocation is token-expiry bounded and provider refresh/logout semantics remain a live validation gate. No real account, hosted key rotation/HS256 gateway or Android Unity cycle was tested here.

## Failures during preparation
Early profiles either used unsupported numeric-host sandbox syntax or denied interpreter startup; none counted as passes. Python required system runtime reads, so final policy permits system-readable files with excluded user/temporary/keychain roots and exact runtime exceptions. Probe corrected macOS cat path and accepted process-exec PermissionError as the expected inspection denial.

First native regression on reused Stage05B.2 fixture data failed the revocation assertion: fixed fixture SID had a pre-existing expiration-bound revocation while a newly minted lab token reused that SID. It was retained as `security-regression-failure.json`; the approved local teardown/reinstall reset those fixtures, all 8 recovery checks and final 53 native checks then passed. Final results are security-regression.json; this is not evidence of live refresh semantics. Isolated API tests use fresh random SIDs and passed twice.

## Reproduction / stop
Authoritative runners `{lab/'launch.py'}`, `{lab/'test_isolated_api.py'}`, `{lab/'regression.py'}`, `{lab/'repeat_rehearsal.py'}`; native runners use PYTHONPATH=work/stage05b2 and existing work/stage05b/venv. Lab config/keys stay private outside outputs. The operator signer delivers test USER tokens over stdin, never private signing key/database password. Review scripts are not turnkey portable or remote runners. Do not restart against reused revoked fixture SIDs without the approved local reset. No remote DSN support is added.

Hosted endpoint/roles/exposed schemas/redirect allowlist/inbox flows, actual hosted service confinement, live user isolation and Android Unity regression are **BLOCKED / NOT EXECUTED**. Cleanup evidence records local services stopped. No production changes.
''')
(out/'Stage05B3_Hosted_Compatibility_Plan.md').write_text(f'''# Stage05B.3 — Hosted compatibility plan
2026-10-09. Approved future test target only: astrogateway-test / jvkwofmxtxjhdwtwdboz. **Plan only: no remote SQL, including read-only, executed.** Public documentation research is not target evidence.

## Before any future approved read-only check
Confirm target from dashboard/account, verify certificate/hostname, use operator-only administration tool outside service, no credentials in argv/logs. Obtain separate authorization for hosted read-only SQL. Prepared `{iso/'Hosted_Read_Only_Inventory.sql'}` begins READ ONLY with bounded statement/lock timeouts; no Auth user rows/password hashes/pg_authid/token content. Review all statements before running. Results can contain object definitions/ACLs; retain privately until sanitized. Add every dashboard-exposed schema to its function inventory. Check actual server version before using version-specific catalogue columns; prepared inventory avoids PG16 membership-option columns.

## Compatibility gates
1. Auth dependencies: auth.users UUID id, created_at, raw_user_meta_data, email_confirmed_at, is_anonymous; native auth.uid()/auth.jwt() and gen_random_uuid(). Inspect helper semantics without exporting user data; signup trigger owner must have required Auth read/app write access and empty search_path. Provider insert/update triggers must coexist. Backfill must have no null created_at; operator-only bounded metadata aggregates require separate approval.
2. Roles: authenticated NOLOGIN/non-super/non-bypass/non-admin and no unexpected parent roles. Provider authenticator switching authority stays outside FastAPI. Administration must be able to create dedicated NOLOGIN schema owner/transfer ownership/create Auth trigger and terminate only retired login sessions; metadata cannot prove every DDL capability. Do not modify provider role memberships to force compatibility without new review.
3. Fresh install collision gate: training, mission_api, schema_owner and named Auth trigger must be absent. Existing objects/data require a separate migration plan; never drop or overwrite. New owner defaults are owner-specific. Explicit helper/RPC ACLs revoke PUBLIC/anon execution; review column ACLs and all default ACLs. Protected tables enable RLS; owner is non-login, service has no membership. Table owners/administrator bypass remain high-trust boundaries.
4. Exactly five exposed mission_api routines, SECURITY INVOKER + empty search_path + typed bounded args. training/auth must not be Data API exposed. Private confirmed_user/profile-trigger definers retain reviewed admin owner and no public RPC exposure. Audit every other exposed schema/routine (including generic SQL/session setters or role-switching definers), not just mission_api. PUBLIC execute defaults on unrelated provider objects must not be blindly changed.
5. JWT expectation: issuer exactly `https://jvkwofmxtxjhdwtwdboz.supabase.co/auth/v1`; audience authenticated (string or containing array); authenticated role, UUID sub/session_id, strict exp/nbf, confirmed non-anonymous user. Verify actual provider claims/JWKS/signing rotation and gateway audience/issuer enforcement from approved evidence. Do not export real tokens to reports. FastAPI forwarding original bearer and provider independent verification remain mandatory; no service-role substitution. Local16.4 gateway is not actual hosted-version proof. Native policies add issuer/audience/strict expiry checks; GUC values alone are not signatures.

## Android callback readiness
Read-only source evidence: MainActivity manifest VIEW/BROWSABLE/DEFAULT has scheme astrogateway, host auth, path /callback; AuthCoordinator.CALLBACK is `astrogateway://auth/callback`. Handler requires same scheme/host/path, rejects fragments, requires bounded code, saved pending flow and PKCE verifier before SDK exchange. Sign-up/recovery use that callback. UI/bridge untouched.

**Remote allowlist UNVERIFIED.** Operator: open exact test project → Authentication → URL Configuration → inspect Redirect URLs for exact `astrogateway://auth/callback`; capture sanitized project-ref/settings evidence, no token-bearing URLs. If absent, adding it is a separately approved remote configuration change. Inspect Email provider enabled/confirmation policy, correct templates respecting redirect and controlled Site URL fallback. Do not alter Site URL or unrelated wildcard settings blindly. With separate account/inbox approval, exercise fresh same-device signup/recovery PKCE, wrong/stale link rejection, code replay, app return and password-reset completion. Custom schemes can be claimed by another app; PKCE reduces stolen-code misuse, verified HTTPS App Links are a future design decision, not silently added here.

## TLS boundaries
User API uses HTTPS certificate-chain/hostname verification through HTTPX defaults; no verify=False/raw-DB TLS fallback. Cleared env excludes proxy/certificate override variables. Local Auth is a public-RSA fixture and gateway HTTP is loopback; never call that production TLS. Local operator-to-native-DB strict TLS1.3 is freshly verified.

Prior Stage05B.1 evidence says client→session pooler TLS1.3 chain/hostname validated, with a legacy certificate-profile compatibility exception. No remote connection/certificate refresh occurred in this stage. That historical strict-profile relaxation must remain admin-only and separately reviewed; it is not equivalent to bypassing chain/hostname and not needed in FastAPI. Do not reintroduce it into runtime. Pooler terminates client TLS: client handshake does not prove pooler→database verification/encryption. Even pg_stat_ssl of a DB backend cannot alone tie both hops or prove peer hostname validation. Obtain provider configuration/attestation and hop evidence before any full-path claim. Full-path TLS remains **UNESTABLISHED**.

## Which checks require further approval
Read-only target inventory/dashboard access: separately authorized future work. SQL001/002, role creation/ACL changes, exact legacy-session termination, Auth trigger/backfill, API schema exposure/cache reload: separate explicit mutation approval after preflight/backup. Hosted live users/mission writes/email/recovery/RLS/Unity test cycles: separate approved disposable account/inbox workflow. Local SQL fixture900/down910 are never remote tools. No hosted action was taken now.

References reviewed: [PostgREST authentication](https://docs.postgrest.org/en/stable/references/auth.html), [Supabase API hardening](https://supabase.com/docs/guides/api/securing-your-api), [custom schemas](https://supabase.com/docs/guides/api/using-custom-schemas), [redirects/mobile deep links](https://supabase.com/docs/guides/auth/redirect-urls), [SSL verification](https://supabase.com/docs/guides/platform/ssl-enforcement). These document mechanisms, not this project's remote settings.
''')
(out/'Stage05B3_Remote_Execution_Readiness.md').write_text(f'''# Stage05B.3 — Remote execution readiness
2026-10-09. **NO-GO for remote execution. Local stage complete.**

| Gate | Result |
|---|---|
| Local actual confined runtime cannot read known admin credentials | PASS, kernel sandbox UID501 with inherited child restriction |
| Local environment/process/code/network boundary | PASS, 18 probes |
| User JWT → independent gateway/native RLS | PASS local, 125 regression checks repeated; bearer theft remains |
| Actual application startup | PASS connectivity-only; no live authentication |
| SQL pinned bytes | PASS unchanged from Stage05B.2 |
| Supported hosted service identity/container confinement | BLOCKED, not provisioned/tested |
| Hosted metadata/Auth/role/grant/RPC/default/schema compatibility | BLOCKED, no remote SQL authorized or executed |
| Callback allowlist/email provider/template evidence | UNVERIFIED |
| Backup/restoration/target-specific rollback | BLOCKED, not verified |
| Hosted full-path TLS | UNESTABLISHED |
| Remote execution approval | NOT GRANTED |

## Candidate manifest remains unchanged
Review `{root/'outputs/Stage05B2/Revised_Final_SQL_Manifest.md'}` and hashes `{e/'artifact-manifest.json'}`. Future intended order remains:
1. `{pre/'000_read_only_preflight.sql'}` after separate read-only authorization; supplement Stage05B3 read-only inventory.
2. `{pre/'001_verified_jwt_training.sql'}` fresh installation only, transactional app schema/tables/roles/RLS/trigger/backfill; collisions abort.
3. `{pre/'002_retire_direct_runtime.sql'}` exact legacy role quarantine, revoke memberships/table/column/function/schema rights, nontransactional exact-session termination.
4. `{pre/'003_read_only_verification.sql'}` followed by approved real-gateway/two-user verification; catalogue alone is not acceptance.

No old Stage05B/05B1 SQL combined, no fixture900 or down910 on Supabase. Separate Data API exposed-schema settings/cache handling are also mutations. Quarantine termination cannot be undone by transaction rollback. Outer BEGIN→ROLLBACK is not a safe remote rehearsal of files containing COMMIT.

## Exact next operator decisions/actions
Choose deployment host and supported isolation mechanism. Supply no administrator/gateway/signing keys to service; deploy immutable backend/public config only with non-admin independent identity and no operator-home/shared-project/container-engine mounts. Repeat denied reads using that real service identity, including env/FD inheritance, parent inspection, symlink/code writes and admin network operations. Keep management credentials in a separate compartment.

Approve hosted **read-only** compatibility/dashboard inspection separately if desired; current instruction prohibits all remote SQL. Confirm approved project, source/candidate hashes, collision-free inventory, provider Auth semantics and all exposed RPCs. Capture sanitized callback/Email settings evidence. Test private backup restore and target-specific reversible recovery/traffic pause; retain no secrets in output files. If any gate fails, keep remote mutation blocked and review changes before granting write approval.

After gates pass, request a separate explicit operator approval naming project ref, exact SQL hashes/order, retired-session termination, new schema exposure/cache changes and recovery scope. Current local approval grants none of these. Approved live test accounts/inbox/email workflow require their own authorization. Do not ask to blindly execute this bundle now.

Residual risk acceptance must cover valid user-bearer theft/replay by a compromised service; this architecture does not prevent it. If the required threat model forbids that too, redesign is still required and remote use stays blocked. No service-role key/raw database password is introduced to work around any blocker.

Stopped at local verification/readiness review. Android UI/navigation/Unity bridge/Mars/React preserved; no hosted writes, roles, SQL or live accounts created.
''')
print('Four Stage05B3 deliverables and hashed review artifacts created.')
