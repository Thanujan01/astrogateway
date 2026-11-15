# Sentinel-Database

Candidate PostgreSQL / Supabase schema for AstroGateway. **No migration here has ever been
applied to a hosted database.** Everything is validated only against a disposable local
PostgreSQL 17 instance — see [the validation report](../../docs/SENTINEL_DATABASE_VALIDATION.md).

## ⚠ This folder does not contain the whole schema

The authoritative migration chain is split across three locations for historical reasons.
Read it in this order; the numbers are the real install order:

| # | File |
|---|---|
| 001 | `docs/history/codex-outputs/Stage05B2/SQL/001_verified_jwt_training.sql` |
| 002 | `docs/history/codex-outputs/Stage05B2/SQL/002_retire_direct_runtime.sql` |
| 004 | `docs/history/codex-outputs/UnifiedAuth/SQL/004_profile_display_name.sql` |
| 005 | `01-Game-Development/MissionRedHorizon/Docs/Step3/SQL/005_mission_commands_reports.sql` |
| 006 | `01-Game-Development/MissionRedHorizon/Docs/FirstMission/SQL/006_sensor_records.sql` |
| 007 | `migrations/007_account_profile.sql` |
| 008 | `migrations/008_private_avatars.sql` |
| 009 | `migrations/009_private_login_broker.sql` |

`validation/validate_local.py` is the single source of truth for that list — see its
`CANDIDATES` constant. If you change the chain, change it there.

Fixture-only files (`900_*`, `901_*`) create synthetic `auth.users` / `auth.uid()` stand-ins so
the chain can install without a real Supabase. **Never apply a fixture to a hosted project.**
Rollback counterparts are numbered `9xx` next to their forward migration.

## ⚠ `001_identity_and_mission_sessions.sql` in this folder is SUPERSEDED

Despite its name and position, the `001_*.sql` sitting in this directory is **not** step 001 of
the chain above. It is the abandoned Stage 05B design, built on a custom `astrogateway_api` role
and `current_setting('astrogateway.user_id')` transaction settings. The current backend
(`database.py`) talks to `mission_api.*` RPCs that rely on `auth.uid()` / `auth.jwt()` and the
`authenticated` role. **Applying it yields a schema the API cannot use.** The file is retained
because `validate_local.py` reports on it, and it carries a header saying all of this.

## Schema shape

Two schemas. `training` holds the tables and stays private; `mission_api` holds the RPCs and is
the **only** schema that may be exposed in Supabase Data API settings.

```
training.profiles                  user_id PK -> auth.users, display_name, username,
                                   avatar_path, trainee_profile, training_level 0-10
training.auth_session_revocations  auth_session_id PK, user_id, expires_at
training.mission_sessions          id, user_id, auth_session_id, launch_id UNIQUE,
                                   mission_id, status, expires_at, completed_at
training.mission_commands          (session_id, command_id) PK, UNIQUE(session_id, revision)
training.sensor_records            (session_id, event_id) PK, sensor_id, provenance, record
training.login_attempt_windows     key_hash PK, window_started, attempts   (broker only)
storage.buckets 'profile-photos'   private, 2 MB, image/png only
```

Security invariants that must not regress:

- RLS enabled on every `training` table, with `USING` **and** `WITH CHECK` on writes.
- Column-level `GRANT`s, never table-level, so clients cannot set `status` / `completed_at`.
- Every function in `mission_api` is `SECURITY INVOKER`, so RLS always applies. The validator
  asserts `No exposed SECURITY DEFINER RPCs`.
- Clients may write only `provenance IN ('SIMULATED','UNAVAILABLE')`. `MODEL-ESTIMATED` and
  `BACKEND-ANALYZED` have no client write path at any layer.
- `public.broker_login_identity` is executable by `service_role` only — never `anon` or
  `authenticated`. It belongs to the isolated Edge broker in `../Auth-Broker`, not to FastAPI.

## Running the validation

Needs a local PostgreSQL 17 (`initdb` / `pg_ctl` on PATH) and `psql`. It creates a throwaway
cluster, installs the chain, runs the assertions and writes `validation/latest_results.json`.

```bash
python3 validation/validate_local.py
```

Last recorded run: **59 PASS / 0 FAIL**, PostgreSQL 17.11, `remote_database_access: false`.
