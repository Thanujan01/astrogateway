# Stage05B.3 — Security test results
2026-10-09. Local only. No final executed assertion failed.

| Suite | Final result | Evidence |
|---|---:|---|
| Backend contracts/repository/JWT/configuration | 48 PASS | `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B3/Evidence/backend-unit.log` |
| Same backend suite inside kernel confinement | 48 PASS, repeated suite, not additional unique coverage | `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B3/Evidence/isolated-backend-unit.log` |
| Native PostgreSQL17.11 + PostgREST16.4 attacks | 53 PASS | `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B3/Evidence/security-regression.json` and native-regression.log |
| FastAPI inside sandbox → real local PostgREST | 16 PASS | `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B3/Evidence/isolated-api-integration.json` and isolated-api.log |
| Local collision/rollback/fresh reinstall | 8 PASS | `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B3/Evidence/repeat-rehearsal.json` |
| Actual runtime boundary | 18 PASS | `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B3/Evidence/isolation-probe.json` |
| Actual application lifespan/HTTP health | PASS, connectivity-only | `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B3/Evidence/isolated-service-health.json` |
| Strict local native database TLS | PASS | `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B3/Evidence/local-tls.json` |

The previous 125 functional/security checks were repeated, plus 18 isolation checks. Do not count duplicate backend runs twice. TestClient has one existing Starlette/httpx deprecation warning; no unnecessary dependency rebuild/install.

## Attack coverage and residuals
Invalid/expired/unsigned/forged JWTs, changed signed subject, wrong issuer/audience/role/SID, malicious identity headers/RPC arguments, B access to A missions, fake internal Principal B carrying token A, arbitrary PostgreSQL identity GUCs, SET ROLE/DDL/RLS disable, owner/level/expiry column writes, raw stolen retired password, direct private schema/set_config RPC, pooled A/B isolation and revocation all tested. Gateway independently verifies RSA signature; host verifier bypass cannot mint B authority. Runtime holds no raw DB credential. Native attacker may set GUCs but lacks data/membership privileges. Privileged native tests are performed by operator harness, not by passing admin credentials into confined runtime.

Valid B bearer replay succeeded as the explicitly expected bearer-token residual; that test is not proof replay is prevented. Logout revocation is token-expiry bounded and provider refresh/logout semantics remain a live validation gate. No real account, hosted key rotation/HS256 gateway or Android Unity cycle was tested here.

## Failures during preparation
Early profiles either used unsupported numeric-host sandbox syntax or denied interpreter startup; none counted as passes. Python required system runtime reads, so final policy permits system-readable files with excluded user/temporary/keychain roots and exact runtime exceptions. Probe corrected macOS cat path and accepted process-exec PermissionError as the expected inspection denial.

First native regression on reused Stage05B.2 fixture data failed the revocation assertion: fixed fixture SID had a pre-existing expiration-bound revocation while a newly minted lab token reused that SID. It was retained as `security-regression-failure.json`; the approved local teardown/reinstall reset those fixtures, all 8 recovery checks and final 53 native checks then passed. Final results are security-regression.json; this is not evidence of live refresh semantics. Isolated API tests use fresh random SIDs and passed twice.

## Reproduction / stop
Authoritative runners `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/work/stage05b3/launch.py`, `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/work/stage05b3/test_isolated_api.py`, `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/work/stage05b3/regression.py`, `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/work/stage05b3/repeat_rehearsal.py`; native runners use PYTHONPATH=work/stage05b2 and existing work/stage05b/venv. Lab config/keys stay private outside outputs. The operator signer delivers test USER tokens over stdin, never private signing key/database password. Review scripts are not turnkey portable or remote runners. Do not restart against reused revoked fixture SIDs without the approved local reset. No remote DSN support is added.

Hosted endpoint/roles/exposed schemas/redirect allowlist/inbox flows, actual hosted service confinement, live user isolation and Android Unity regression are **BLOCKED / NOT EXECUTED**. Cleanup evidence records local services stopped. No production changes.
