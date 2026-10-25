# Sentinel database validation

[README](../README.md) · [Team plan](TEAM_PLAN.md) · [Responsibilities](DEVELOPER_RESPONSIBILITIES.md) · [Architecture](ARCHITECTURE.md)

## Current decision

**Local validation gate: PASS — 35 checks passed and 0 failed on PostgreSQL 17.11.** Six fixture/candidate SQL files installed successfully, and all 11 RPC names expected by the current backend were present. This is a disposable local rehearsal with synthetic users and claim helpers, not a live Supabase certification. The temporary cluster was stopped and removed. Candidate command/sensor SQL was updated to reject missing/fractional ticks and invalid sensor batches, and to return the backend-expected SQLSTATE codes. No hosted database was changed.

[Machine-readable results](../02-Software-Development/Sentinel-Database/validation/latest_results.json) · [Reproducible validator](../02-Software-Development/Sentinel-Database/validation/validate_local.py)

## What Sentinel needs in the software workstream

Database validation is a distinct acceptance gate, not just form validation in the web/Android UI. Proposed owner: **Bawantha** for database/backend validation, with **Kapilash** reviewing Unity payloads and integration behavior. **Layanthini and Shinojan** should use the actual results/limitations in the project documents and PPT. This recommendation does not change repository permissions or assign a production database administrator.

| Layer | Required evidence |
|---|---|
| Schema and dependencies | Auth/schema prerequisites, expected tables/functions, migration order, FK relationships, required fields, valid status transitions and collision behavior. |
| Data integrity | Display-name bounds, training-level bounds, unique launch IDs, typed command fields, valid sensor batches, ownership references and idempotency/conflict handling. |
| Access and ownership | RLS enabled, user A cannot read/write user B's records, anonymous access denied, exposed RPCs use intended privilege boundaries, legacy runtime roles retired. |
| Identity lifecycle | Valid identity, expired/revoked identity and confirmed-email behavior; actual JWT signature verification must be tested through the independent gateway, not by synthetic SQL claims alone. |
| Backend contract | Every backend-required RPC exists; argument/response/error-code behavior agrees with the adapter and clients. Function-name coverage alone is insufficient. |
| Migration and recovery | Fresh install/collision behavior, reviewed transition from existing schemas, rollback scope, tested backup/restore and deployment-specific preflight. |
| Concurrency and limits | Command revisions, idempotency, sensor/command caps under simultaneous requests, and bounded response/performance behavior. |

## Baseline compatibility finding

The original [Sentinel SQL](../02-Software-Development/Sentinel-Database/001_identity_and_mission_sessions.sql) is an older Stage05B candidate. It creates a raw-runtime role/custom transaction-setting design and **no `mission_api` RPC definitions**. The [current backend adapter](../02-Software-Development/Backend/astrogateway-api/database.py) expects 11 typed `mission_api` RPCs. That older file by itself is not a current-backend installation recipe.

The [Stage05B2 revised manifest](history/codex-outputs/Stage05B2/Revised_Final_SQL_Manifest.md) supersedes the old design as a review candidate. This validator rehearses the revised base, retirement of local legacy roles, profile update, mission commands/reports and sensor-record candidates together in a fresh cluster. This establishes local compatibility for the exercised paths; it does not approve a hosted deployment order or an in-place transition of existing data.

## SQL used by the local rehearsal

| File | Local installation |
|---|---|
| [900_local_fixture_only.sql](../docs/history/codex-outputs/Stage05B2/SQL/900_local_fixture_only.sql) | PASS |
| [001_verified_jwt_training.sql](../docs/history/codex-outputs/Stage05B2/SQL/001_verified_jwt_training.sql) | PASS |
| [002_retire_direct_runtime.sql](../docs/history/codex-outputs/Stage05B2/SQL/002_retire_direct_runtime.sql) | PASS |
| [004_profile_display_name.sql](../docs/history/codex-outputs/UnifiedAuth/SQL/004_profile_display_name.sql) | PASS |
| [005_mission_commands_reports.sql](../01-Game-Development/MissionRedHorizon/Docs/Step3/SQL/005_mission_commands_reports.sql) | PASS |
| [006_sensor_records.sql](../01-Game-Development/MissionRedHorizon/Docs/FirstMission/SQL/006_sensor_records.sql) | PASS |

`900_local_fixture_only.sql` creates synthetic Auth helpers and fixture roles only in the new local cluster. It is not a Supabase migration. The retirement script can terminate the exact legacy-role sessions; in this harness it runs only inside that new isolated cluster, never against a supplied database. Each source SQL hash is recorded in the JSON report.

## Original six failures — fixed and retested locally

| Finding | Expected | Original observation | Fix applied |
|---|---|---|---|
| Scan command omits `tick` and substitutes an extra key | `PC422` rejection | Accepted and stored as a command | Require the exact payload keys and explicit non-null/true validation. Review SQL NULL propagation; a NULL predicate must not become acceptance. |
| Scan command uses fractional `tick` | Integer tick, matching the API model | `1.5` accepted | Enforce integer semantics as well as numeric range in SQL, with matching negative tests. |
| Sensor event replay has changed content | Conflict SQLSTATE `PS409` expected by backend | SQLSTATE `P0001`, message `PS409` | Make SQL/gateway/backend error contracts agree and test the complete response path. |
| Sensor provenance claims backend analysis | Rejection using expected `PS422` contract | Rejected with SQLSTATE `P0001`, message `PS422` | Retain the rejection and align its error contract; this is not an observed provenance-write bypass. |
| Sensor batch argument is SQL NULL | Invalid batch rejection | Returned an empty successful result `[]` | Reject NULL explicitly before array/length checks. |
| User B submits a sensor record into user A's session | Expected denial code `PT404` | Denied with SQLSTATE `P0001`, message `PT404` | Retain ownership denial and align its error contract; this is not an observed cross-user write bypass. |

The SQLSTATE observations are measured. The backend source branches on gateway response `code`, not its message; the implications for actual HTTP responses still need a real local/approved gateway-to-API test. No hosted HTTP result is claimed here. Direct SQL/RPC negative tests are important because client/API validation alone does not prove the exposed database surface rejects malformed requests.

## All executed checks

| Check | Result |
|---|---|
| Current backend RPC names are present | **PASS** |
| RLS enabled on all five training tables | **PASS** |
| No exposed SECURITY DEFINER RPCs | **PASS** |
| Anonymous cannot execute current_profile | **PASS** |
| Legacy login role retired | **PASS** |
| Profile returns the current owner | **PASS** |
| Other user profile is invisible | **PASS** |
| Cross-user mission insert denied by RLS | **PASS** |
| Profile RPC rejects short display name | **PASS** |
| Database enforces display-name length | **PASS** |
| Database enforces training-level range | **PASS** |
| Profile requires an Auth user FK | **PASS** |
| Duplicate mission launch is rejected | **PASS** |
| Completion status requires completed_at | **PASS** |
| Expired SQL claims are rejected | **PASS** |
| First semantic command accepted | **PASS** |
| Identical command replay is idempotent | **PASS** |
| Stale command revision rejected | **PASS** |
| Unknown command kind rejected | **PASS** |
| Scan command missing tick is rejected | **PASS** |
| Scan command fractional tick is rejected | **PASS** |
| SQL-layer sensor fixture accepted | **PASS** |
| Identical sensor replay is idempotent | **PASS** |
| Sensor conflict uses backend-expected SQLSTATE | **PASS** |
| Forged analysis provenance uses expected rejection | **PASS** |
| Null sensor batch rejected | **PASS** |
| Object/empty sensor batch rejected | **PASS** |
| Null sensor element rejected | **PASS** |
| Missing sensor provenance rejected | **PASS** |
| Cross-user sensor RPC uses expected denial code | **PASS** |
| Cross-user sensor read returns no rows | **PASS** |
| Auth session can be revoked | **PASS** |
| Revoked session cannot read its profile | **PASS** |
| Fresh-install candidate refuses reapplication/collisions | **PASS** |

The accepted sensor fixture deliberately exercises only the SQL layer's minimal shape/idempotency contract. It is not a complete catalog-valid FastAPI sensor request or proof of scientific data quality. The RLS checks cover the particular ownership cases listed, not every possible database policy/privilege path.

## Reproduce locally

Requirements: a complete checkout, Python standard library, and PostgreSQL 17 binaries (`initdb`, `pg_ctl`, `psql`, `postgres`). The measured run used PostgreSQL **17.11**. The validator has no remote DSN or existing-host option. It initializes a short private temporary directory, disables TCP listening, uses a private Unix socket, creates synthetic fixtures, stops its own cluster and removes its temporary data. PostgreSQL connection environment variables are cleared for child processes.

```bash
# Set SENTINEL_PG_BIN to your PostgreSQL 17 binary directory first.
# Run from the repository root; this overwrites the local result snapshot.
python3 02-Software-Development/Sentinel-Database/validation/validate_local.py \
  --pg-bin "$SENTINEL_PG_BIN" \
  --report 02-Software-Development/Sentinel-Database/validation/latest_results.json
```

Exit **0** means every implemented check passed; exit **1** means a failed expectation or harness/setup failure. The committed run exits 0; all 35 checks pass, including four additional malformed sensor batch/provenance cases. Do not change expected results to turn a failing contract into a pass. Review source SQL changes separately, then rerun the same negative tests and affected integration tests. Generic setup failures are recorded separately from individual check failures.

## Sign-off sequence

1. Bawantha and Kapilash agree the current schema/RPC/payload contract and the appropriate reviewed candidate baseline.
2. Review the candidate SQL fixes for the six original failures; ownership and authentication checks remain enforced.
3. Rerun all local positive/negative checks; add concurrency, cap and transaction-atomicity tests.
4. Exercise an isolated gateway/API/client path and verify exact error/status/response behavior. Synthetic `auth.uid`/`auth.jwt` helpers do not verify signatures.
5. Record backup/restore, transition/rollback and target compatibility evidence before any live migration decision.
6. Only then consider separately authorized target-specific live checks/deployment. Keep unexecuted items marked NOT TESTED.

## Still unverified

- Hosted Supabase JWT verification, PostgREST configuration, RLS/Auth semantics, schema exposure and transport/TLS.
- Live authenticated accounts, gateway-to-API HTTP error mapping and Unity/Android/web end-to-end behavior.
- Concurrent writes, per-session limits under races, load/performance and complete abuse-case coverage.
- Backup restoration, production rollback and migration from an existing older schema.
- Stage07A experimental schema; it is not included in this primary-backend rehearsal.

Current conclusion: **35 local checks pass; hosted integration and deployment readiness remain unverified.**
