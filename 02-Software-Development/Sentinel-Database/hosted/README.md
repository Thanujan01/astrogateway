# Hosted deployment — project `trzxbhzgjwbihpzdgtfu`

**This is the first time the AstroGateway schema has been installed on a real hosted database.**
Everything before this was local rehearsal only. Deployed 2026-11-13 to a Supabase project in
`ap-south-1`, PostgreSQL 17.11 — the same major/minor the local validator signed off on.

| | |
|---|---|
| Project ref | `trzxbhzgjwbihpzdgtfu` |
| API URL | `https://trzxbhzgjwbihpzdgtfu.supabase.co` |
| Publishable key | `sb_publishable_X4mt2dEJKHv9wiuQ0GLzWg_ihLkTIre` (public by design) |
| Exposed schemas | `public, graphql_public, mission_api` — `training` stays private |

These files are the chain as actually applied. They are copies of the authoritative sources
(listed in `../README.md`) with the project ref substituted and the deviations noted below.
Every file except `001` is otherwise byte-identical to the validated original.

## Deviations from the source migrations

Each is marked `HOSTED DEVIATION` in the SQL.

1. **`001` — issuer repointed.** `training.claims_valid()` hardcodes the Auth issuer. It pointed at
   the old project `jvkwofmxtxjhdwtwdboz`; it now pins
   `https://trzxbhzgjwbihpzdgtfu.supabase.co/auth/v1`. **A token from any other project is
   rejected.** Repoint this if the project ever changes.
2. **`001` — `GRANT astrogateway_schema_owner TO postgres`.** Supabase's `postgres` is not a
   superuser, so without membership it can neither `ALTER DEFAULT PRIVILEGES FOR ROLE` that role
   nor transfer ownership to it. Without this the migration fails with
   `42501: permission denied to change default privileges`.
3. **`006` — ownership made consistent.** The source never transfers ownership of
   `training.sensor_records` or `mission_api.submit_sensor_records`, contradicting 001's stated
   rule that all app objects are owned by `astrogateway_schema_owner`.

`mission_api` is exposed via `ALTER ROLE authenticator SET pgrst.db_schemas = ...` rather than the
dashboard toggle. **Changing Exposed Schemas in the dashboard will overwrite that role setting**,
so re-apply it if the API starts returning `PGRST106` for `mission_api`.

## Verified on the live database

Post-install assertions, all passing:

| Check | Result |
|---|---|
| `training` tables with RLS enabled | 6 / 6 |
| RLS policies (`training` / storage avatars) | 11 / 4 |
| `mission_api` functions | 13 |
| **`SECURITY DEFINER` functions in the exposed schema** | **0** |
| Backend RPC names present (the 11 `database.py` calls) | 11 / 11 |
| `anon` can execute `current_profile` | no |
| `authenticated` can execute `current_profile` | yes |
| Tables not owned by `astrogateway_schema_owner` | 0 |
| `profile-photos` bucket private | yes |

Behavioural tests against the live project:

- `training` schema is unreachable over REST — `PGRST106 Invalid schema: training`.
- `mission_api` reachable, but `anon` gets `42501 permission denied for schema mission_api`.
- Full mission flow works: profile read/update, session init, 4 commands with revision
  increments, state, report, skill evidence, completion.
- Rejections return the exact codes `database.py` maps: `PC422` (bad payload), `PC409` (stale
  revision), `PS422` (client-supplied `BACKEND-ANALYZED` provenance).
- **Cross-user isolation holds.** A second user targeting the first user's session id was blocked
  on all five of read state, read report, inject command, write sensor record and hijack
  completion — each `PT404`, which does not even confirm the session exists.
- End-to-end through FastAPI (`/v1/me`, `/v1/mission-sessions`, `/v1/reports/{id}`, `/v1/skills`,
  `/v1/auth-session/revoke`) against this database: all pass. 253 backend unit tests pass.

## Test accounts

Created directly in SQL so that **no confirmation email was sent to anyone**. Delete before any
real use:

| Email | Password | Purpose |
|---|---|---|
| `astro.trainee@sk2lb.local` | `TestPassphrase-2026!` | primary test trainee |
| `astro.intruder@sk2lb.local` | `OtherPassphrase-2026!` | isolation / negative tests |

```sql
DELETE FROM auth.users WHERE email IN ('astro.trainee@sk2lb.local','astro.intruder@sk2lb.local');
```

Profiles, sessions, commands and sensor records cascade from `auth.users`.

Note for anyone creating users this way: GoTrue cannot scan SQL `NULL` into its string columns, so
`confirmation_token`, `recovery_token`, `email_change`, `email_change_token_new`,
`email_change_token_current`, `reauthentication_token`, `phone_change` and `phone_change_token`
must be set to `''`, not left NULL. Otherwise login fails with
`500 Database error querying schema`.

## Still not done

- Email confirmation is on, and `training.confirmed_user()` requires it, so real sign-ups need a
  configured SMTP sender before they can use the API.
- The `account-login` Edge broker (`../../Auth-Broker`) is **not deployed**, so username login
  stays disabled. It needs `SUPABASE_SERVICE_ROLE_KEY`, `ACCOUNT_TURNSTILE_SECRET` and
  `ACCOUNT_LOGIN_PEPPER` set as function secrets — never in FastAPI or the web client.
- No avatar has been uploaded, so the storage policies in `008` are installed but unexercised.
- The Unity client still runs fully offline; it does not call this API.
