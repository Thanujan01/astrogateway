# Step 3 — Local backend mission endpoints (2026-10-17)

Status: DONE locally. Not deployed. No hosted Supabase, remote database or paid service touched. Stop for owner approval before Step 4.

## What was added
Backend (`/Users/kapilash/Projects/AstroGatewayAndroid/backend`, same FastAPI app, same auth chain: bearer → Supabase Auth `/auth/v1/user` verification + claim checks → confirmed email → revocation check → `mission_api` RPC with the user's own JWT):
| Endpoint | Purpose |
|---|---|
| `POST /v1/mission-sessions/{id}/commands` | Append a semantic command. Body `{command_id, expected_revision, kind, payload}`; extra fields (e.g. `score`, `user_id`) → 422. Idempotent on `command_id`; reused id with new content → 409; stale/future revision → 409; >500 per session → 429. Only the creating Auth session, only while the mission is open. |
| `GET /v1/mission-sessions/{id}` | Owner's state: revision, paused flag (last pause/resume command), timestamps. 404 for others. |
| `GET /v1/reports/{id}` | Evidence counts (scans, board/exit cycles, weather changes). `score: null`, `rubric_status: pending_approval`. |
| `GET /v1/skills` | Evidence totals across the user's sessions. `levels: null`, `rubric_status: pending_approval`. |
| `GET /v1/recommendations` | Labelled placeholder: only published mission, `rule_status: pending_approval`. |
Payload rules per kind (scan: `sample_id` UUID, `tick` 0–1e9, `analysis` = `unverified_simulated`; weather: `dust` 0–2; others: empty object), 1 KB cap — enforced in FastAPI and again in SQL.

Changed backend files: `main.py`, `database.py` (+ `README.md` note). Added: `tests/test_mission_commands.py`, `tests/local_mock_server.py`. Backend `.venv`, `.env`, certs untouched. Pre-change verified backup: `MissionRedHorizon_Step1Work/backup/Step3_Backend_PreChange_*`.

Database candidate (not executed): `Docs/Step3/SQL/005_mission_commands_reports.sql` and rollback `905_...`. Append-only `training.mission_commands` with RLS (own rows, current Auth session, open mission), four SECURITY INVOKER `mission_api` functions, no change to existing tables.

Unity (not wired into any scene): `Assets/AstroGateway/Integration/Http/HttpMissionSessionService.cs` (offline queue → ordered upload, revision resync on 409, drops server-rejected commands, HTTPS required except localhost, `Connected` only after server ack) and `ShellIdentityProvider` (token in memory only; identity marked verified only after the server accepts it).

## Test results
| Suite | Result | File |
|---|---|---|
| Backend existing tests (baseline before changes) | 68/68 | — |
| Backend all tests after changes | 146/146 (68 existing + 78 new) | `Backend_pytest_results.txt` |
| Unity client ↔ real FastAPI routes on 127.0.0.1 (mock Auth, in-memory data) | 18/18 | `Step3_Client_Results.txt` |
| Uvicorn smoke (unconfigured): new routes return 401 without sign-in; health = configuration_required | pass | — |
| Unity main project compile | pass | — |

Authorization verified by tests: missing/malformed/Basic token, expired, wrong issuer, service_role, mismatched subject → 401; unconfirmed email → 403; revoked session → 401 on every new route; another user's session → 404; different Auth session → 409 for commands; forged ownership in a repository response → 502; only the original user JWT is forwarded, no owner/score fields in RPC bodies; SQL error details never echoed.

## Not verified (needs approval or tools)
- The SQL has NOT been executed: PostgreSQL/PostgREST are not installed on this Mac. The mock repository mirrors its rules; a local rehearsal needs PostgreSQL 17 + PostgREST installed (ask first).
- No hosted Supabase test: live Auth tokens, gateway schema reload and RLS on the provider are untested.
- Mission id: backend accepts only `mission-red-horizon-poc`; Unity registries use `mission.red_horizon.*`. The client maps to the backend id by default — needs a decision.
- The Unity client is not added to any scene; the shell (Android/web) token hand-off is not implemented.
- Test tooling: pytest was installed in a separate venv (`MissionRedHorizon_Step1Work/step3-venv`); the backend's own `.venv` was not modified.
