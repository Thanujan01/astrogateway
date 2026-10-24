# Stage 05B.1 — Local rehearsal results

2026-10-09. **Local-only evidence. Overall remote readiness BLOCKED.**

## Environment and provenance

Final SQL engine: `PostgreSQL 17.5 on wasm32-unknown-linux-gnu, compiled by emcc (Emscripten gcc/clang-like replacement + linker emulating GNU ld) 3.1.74 (1092ec30a3fb1d46b1782ff1b4db5094d3d06ae5), 32-bit`; pinned PGlite0.4.6 in isolated workspace lab. Target historical server is PG17.11; major matches, patch/platform/extension/service stack do not. An exploratory run on the previously installed PG18.3 was retained separately and is not the final PG17 acceptance evidence. The [upstream changelog](https://github.com/electric-sql/pglite/blob/main/packages/pglite/CHANGELOG.md) identifies engine changes; actual SELECT version() is recorded in final JSON.

Database was in-memory/disposable, closed after execution; no TCP listener, remote DSN, administrative credential file or live Supabase user was used by the SQL runner. Minimal Auth/service roles and auth.users are explicit fixtures. A/B are invented **local fixture UUIDs**, never live accounts. Session authorization is simulated from an initial local superuser to enforce runtime role checks; returning to administrator is a harness operation. Native LOGIN/password authentication and initial-session-user restrictions are not proven by this setup. No system-wide PostgreSQL installation was made. One pinned test dependency was installed only in the workspace to match PG17; existing project dependencies were not replaced.

## Results

| Check | Result | Evidence / limit |
|---|---|---|
| FastAPI original security contracts + new hardening tests | PASS — 54 tests | `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B1/Hardening/Evidence/project-backend-tests.log`; mock Auth/repository, no live Supabase |
| Signed HMAC fixture accepted, altered signature rejected | PASS | Synthetic Auth transport verifies a real local fixture HMAC; never a deployed signing key or live verification |
| Extra owner/score fields, wrong issuer/sub/audience/role, invalid/expired/not-before claims, malformed/duplicate claims | PASS for covered contract cases | Same test log; unimplemented live flows not inferred |
| Strict local TLS chain/hostname positive and wrong-host/untrusted-chain negative handshakes | PASS | Real OpenSSL TLS via memory BIO; no DB transport implied |
| Unsafe TLS DSN/admin login/missing CA/legacy wrong fingerprint | PASS — rejected | Same tests; strict default and scoped pinned exception checked |
| Startup unsafe role flags/owner/membership/schema/create guard | PASS — mocked guard cases | Actual guard-to-network pool untested |
| Candidate preflight + migration + ACL + runtime provisioning + metadata | PASS | `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B1/Hardening/Evidence/sql-rehearsal.json` and `.log` |
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

Final SQL count: **55 PASS / 1 FAIL**. Blocks above are separately listed, not fabricated as executed SQL assertions. One backend test warning is Starlette/httpx TestClient deprecation; no application crash/ANR claim is made. Evidence retains an initial harness error: RESET SESSION AUTHORIZATION did not restore the administrator as assumed in the embedded engine; harness now explicitly restores its original administrator between cases. Final PG17 run completed; that harness correction is not a product permission change.

## Exact reproduction

Run from `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp`. Tools/dependencies are local test tools, never remote deployment runners:
```
node work/stage05b1-hardening/rehearse.mjs
work/stage05b/venv/bin/python work/stage05b1-hardening/tls_fixtures.py
PYTHONPATH=/Users/kapilash/Projects/AstroGatewayAndroid/backend PYTHONDONTWRITEBYTECODE=1 work/stage05b/venv/bin/python -m pytest -p no:cacheprovider /Users/kapilash/Projects/AstroGatewayAndroid/backend/tests -q
```
Runner SQL entrypoints are explicit files; no network/remote credential loading occurs. Regenerating local TLS fixtures produces new disposable keys only under private work storage, not deliverables. Capture pytest output to a local evidence log if re-running; never run live tokens through command arguments. A user-facing runner copy is `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B1/Hardening/rehearse.mjs`; its pinned lab module path assumes this workspace. Package integrity lock: `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B1/Hardening/pg17-package-lock.json`.

## Impersonation reproduction (disposable ONLY)

As simulated runtime, BEGIN→SET LOCAL ROLE astrogateway_api→set A user/SID→overwrite the same two custom settings with B user/SID→SELECT training.profiles. One B row is returned. Commit resets context, but it cannot undo disclosure. These settings are SQL caller assertions; lack of BYPASSRLS does not authenticate them. Do not run this against real accounts or use the result to claim compromised-backend isolation passed.
