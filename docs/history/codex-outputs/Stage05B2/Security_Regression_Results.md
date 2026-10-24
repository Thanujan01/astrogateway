# Stage05B.2 — Security regression results

2026-10-09. Local-only final results: **48 backend unit PASS; 53 native gateway/DB PASS; 16 API-to-gateway PASS; 8 rollback/reinstall PASS.** No final assertion failed. BLOCKED live/deployment checks below are not counted as executed tests.

## Environment

Native server: `PostgreSQL 17.11 on aarch64-apple-darwin25.6.0, compiled by Apple clang version 21.0.0 (clang-2100.3.34.2), 64-bit`. Source17.11 verified SHA256 `dd27f2b3c59e73ed14aa3324901242bf69a032a6347805f274e6260322d42979`; built entirely under work/stage05b2/pg-install with installed compiler/OpenSSL, no system service/installation. Relocated Homebrew bottle resource paths prevented init; that attempt was replaced by a properly prefixed upstream source build. Logs retained. No PGlite-only result is used as native credential/gateway proof.

Official PostgREST16.4 ARM64 release archive SHA checked against GitHub release digest; local load path changed only to use workspace libpq and ad-hoc signed for the lab. Hosted Supabase version is unknown. Gateway uses PUBLIC RSA verification key only; local issuer private key/fixture DB passwords stay in owner-only work/private files, never outputs. Minimal Auth fixtures follow provider UUID/JWT helpers and confirmed/anonymous columns; not a full GoTrue/Supabase stack. Four fixture rows are local inventions, not live accounts.

Native admin/gateway DB transport uses verified chain/hostname, SCRAM and TLS1.3; pg_stat_ssl=true. API/gateway test HTTP is loopback-only, not production HTTPS or hosted end-to-end encryption. The Auth transport is a local RSA fixture; data requests reach the real local PostgREST independently. Deliberately bypassed Auth verification is an attack test, not production code.

## Evidence and coverage

| Evidence | PASS | What it proves / limit |
|---|---:|---|
| `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B2/Evidence/project-backend-unit.log` | 48 | Original JWT/contract cases, new no-DB configuration, typed request forwarding, bounded/sanitized errors, network no-fallback and known admin-file diagnostic. Mock transports, not live Auth |
| `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B2/Evidence/security-regression.json` / native-regression.log | 53 | Real gateway RSA signature/claims; wrong issuer/audience/session/role/expiry; invalid/unsigned/forged tokens; headers/RPC owner override denied; A/B mission isolation; RLS/column privilege negatives; native LOGIN attacker GUC tampering has no data authority; retired role/authenticated/owner no raw login; revocation/pool isolation |
| `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B2/Evidence/api-gateway-integration.json` / api-integration.log | 16 | Actual updated FastAPI source through local gateway; profile/init201/complete/idempotency/logout contracts; extra owner rejected; B cannot complete A; bypassed host verifier and forged Principal cannot substitute B authority; bearer replay residual explicitly demonstrated |
| `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B2/Evidence/repeat-rehearsal.json` / repeat-rehearsal.log | 8 | Fail-closed repeat/collision, local down retains Auth rows, late trigger/null-backfill atomic rollback, clean reinstall/re-quarantine, Auth insert/default profile and retained quarantine |
| `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B2/Evidence/native-setup.json` | Setup evidence | Saved old password had impersonation ability before quarantine; native login denied after; stale selected-role session terminated; quarantine repeat succeeds; local TLS peer + backend leg verified |
| `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B2/Evidence/actual-service-isolation.json` | Diagnostic PASS, deployment BLOCKED | Real service refuses known readable operator admin file; no remote request. Not general secret containment |

Twenty interleaved gateway requests tested pool identity isolation. Authenticated column grants deny level/owner/lease changes, delete/truncate; arbitrary raw attacker settings and row_security=off still cannot access training. Missing/set_config RPC and private-schema REST requests are rejected. Fresh candidate is intentionally non-idempotent: existing-object error is the expected collision test, never a skipped unsafe overwrite. Quarantine is repeatable.

One test-tool Starlette/httpx deprecation warning; no unrelated dependency replacement. Prototype/harness errors were corrected, followed by final fresh install and full regression. A retained security-regression-failure.json is an earlier wrong-session status failure, not final evidence; final results are security-regression.json and final logs. No Android crash/ANR/live-account validation is claimed.

## BLOCKED / not executed

Actual service OS/container isolation; hosted Supabase catalogue/role/default/RPC/exposed-schema compatibility; real provider Auth/JWKS rotation or legacy HS256 gateway configuration; live email/verification/recovery/session refresh; remote RLS with approved accounts; Android authenticated Unity return/relaunch; native app production HTTPS/physical device; hosted full-path TLS; remote backups/restoration. No remote migration/role creation/write occurred.

## Reproduction and recovery

Rehearsal scripts copied under `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B2`; authoritative local runner module paths used `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/work/stage05b2` and the existing work/stage05b/venv. Private lab credentials/config stay outside outputs. Existing initialized cluster/start_lab intentionally refuses overwriting; recreate only a separately approved fresh disposable directory or explicitly restart the stopped existing cluster. Do not point a runner at remote DSNs: connect() is hardcoded to localhost and fixture role allowlist. Source/tool scripts are for review, not turnkey deployment; adapt paths only in another isolated lab.

Run regression.py, api_integration.py and repeat_rehearsal.py only with this local gateway/cluster and pinned candidate. API tests use PYTHONPATH pointing to the updated backend plus lab module. Never publish real token-bearing requests/config logs or private key/password files. Native lab shutdown is recorded in `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B2/Evidence/lab-shutdown.json`; stale FastAPI stopped, replacement remains blocked.

Local down910 is local-only destructive teardown, not remote rollback. Any future recovery requires private backup, stopped app/signup traffic, exact object/ACL/default/membership snapshots and separate approval. Preserve Auth users and quarantine; remove only new trigger/dependent app objects after retention review. Do not DROP SCHEMA CASCADE/DROP OWNED or re-enable old impersonation policies. Unknown commit status: read-only inspect, never blind rerun. Provider rollback SQL is not authorized here.
