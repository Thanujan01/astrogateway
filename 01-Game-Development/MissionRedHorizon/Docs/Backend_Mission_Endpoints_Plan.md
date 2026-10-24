# Backend mission endpoints (Step 3) — implemented and tested locally 2026-10-17

Result: see `Docs/Step3/Step3_Report.md`. Original plan kept below for reference.

Status 2026-10-16: proposal only. No backend code, SQL, hosted setting or cloud service has been changed.
Existing service: `/Users/kapilash/Projects/AstroGatewayAndroid/backend` (FastAPI, Supabase Auth bearer verification, PostgREST with the user's own JWT, `mission_api` RPCs, RLS). It already has `GET /v1/me`, `POST /v1/me/display-name`, `POST /v1/mission-sessions`, `POST /v1/mission-sessions/{id}/complete`, `POST /v1/auth-session/revoke`, `GET /health`.

## What the Unity client now sends (offline queue, never transmitted yet)
`MissionCommand.ToBrokerJson()`:
```json
{"command_id":"<uuid>","expected_revision":12,"kind":"scan_target","payload":{"sample_id":"<uuid>","tick":42,"analysis":"unverified_simulated"}}
```
Kinds: `board_rover`, `exit_rover`, `scan_target`, `weather_changed`, `pause`, `resume`, `complete`. No score, oxygen total or resource value is ever sent as authority.

## Proposed endpoints (extend the existing app, same auth + RLS pattern)
| # | Method / path | Purpose | Notes |
|---|---|---|---|
| 1 | `POST /v1/mission-sessions/{id}/commands` | Append one semantic command | Idempotent on `(session_id, command_id)`; 409 + current revision on `expected_revision` mismatch; strict per-kind payload schema; size and rate limits |
| 2 | `GET /v1/mission-sessions/{id}` | Canonical state + revision for resume | Owner only (404 otherwise) |
| 3 | `POST /v1/mission-sessions/{id}/pause` and `/resume` | Persist pause; pause must not consume simulated life support | Could be command kinds instead of routes; pick one |
| 4 | Extend `/complete` | Server validates required evidence, writes result + skill evidence in one transaction | Rubric stays "pending approval" until Step 4 decisions; result stores evidence counts only |
| 5 | `GET /v1/reports/{session_id}` | Report: evidence, provisional/approved flag, rubric version | Score field null until a rubric is approved |
| 6 | `GET /v1/skills` | Evidence counts per skill | No mastery levels until approved |
| 7 | `GET /v1/recommendations` | Deterministic next mission + reason | Initially "first mission" rule; real rules in Step 4 |

## Data (candidate SQL, local rehearsal only)
New tables in the private training schema behind `mission_api` RPCs: `mission_commands (session_id, command_id, expected_revision, kind, payload jsonb, accepted_at)`, `mission_results`, `skill_evidence`; add `revision` to `mission_sessions`. Fresh-install candidate + rollback script, rehearsed on disposable localhost PostgreSQL like the UnifiedAuth work. No remote migration.

## Unity side
1. `HttpMissionSessionService : IMissionSessionService` using `UnityWebRequest`; replays the offline `Pending` queue in order; handles 401/404/409/422/429; `Connected` becomes true only after the server acknowledges.
2. `ShellIdentityProvider : IIdentityProvider` that receives the session from the Android/web shell (the existing native→Unity hand-off), never stores the token.
3. Swap via `IntegrationServices` only; scenes and gameplay unchanged.

## Known gaps to resolve first
- Mission id mismatch: backend literal `mission-red-horizon-poc` vs Unity registry `mission.red_horizon.slice`. Needs one canonical id.
- Backend currently refuses to start in the operator-owned dev layout until isolation is set up (see backend README). Real-service runs stay blocked until that is approved.
- Hosted Supabase changes, remote migrations and any paid infrastructure need separate approval.

## Testing plan
pytest for each endpoint (idempotency, revision conflict, ownership, payload validation, rate limit), local PostgreSQL/PostgREST rehearsal of the SQL, and a Unity batch test that replays a recorded command queue against a local mock server.
