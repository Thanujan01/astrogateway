# AstroGateway Stage05B.2 identity API

Supabase Auth remains the only issuer. Kotlin owns signup/sign-in/PKCE and encrypted session/refresh state; Unity receives mission identifiers only. FastAPI verifies each bounded bearer using the configured Supabase Auth `/auth/v1/user` and validates canonical subject, issuer/audience/role, expiry/session UUID and confirmed email.

User data operations now forward the ORIGINAL user JWT over verified HTTPS to Supabase PostgREST, using the fixed `mission_api` schema and five typed invoker RPCs. The independent gateway validates the signature and establishes the PostgreSQL `authenticated` identity. Private training RLS uses native auth.uid/auth.jwt from that trusted gateway, not FastAPI-supplied owner IDs. FastAPI has no SQL pool, signing key, service-role key or direct PostgreSQL credential. No raw SQL or identity-setting RPC exists in the selected surface.

## Configuration and execution gates

Provide only approved HTTPS Supabase URL/publishable key, optional same-project issuer and HTTPS CORS origins via the existing owner-only test environment. DATABASE_URL and DATABASE_SSL_ROOT_CERT must be empty; DATABASE_ALLOW_LEGACY_CA must be false. Direct DB settings fail closed. The old database migration and role-provisioning instructions are superseded by outputs/Stage05B2/Revised_Final_SQL_Manifest.md in the Codex workspace. No migration executes at application startup.

Run the service in an isolated OS/container filesystem that cannot read operator admin credentials, gateway DB credentials or issuer signing keys. The known project admin.local.env startup guard refuses this operator-owned development layout; it is a diagnostic, not a containment boundary. Do not delete/expose admin credentials to bypass the guard. Never mount the database administration directory or private test-lab files into the service. Current real-service startup remains blocked until isolation is established.

Remote schema/role/configuration changes need separate explicit approval. Candidate SQL is fresh-install only; collisions stop. Expose mission_api only as the new application surface, never training/auth; audit other existing exposed schemas before leaving them enabled. Retire legacy raw-runtime roles, including stale sessions that already selected authenticated. No new app DB login is required. Do not apply old Stage05B/05B1 SQL bundles or combine them with the revised candidate.

## Contracts and security limits

GET /v1/me; POST /v1/mission-sessions; POST /v1/mission-sessions/{id}/complete; POST /v1/auth-session/revoke retain their existing JSON contracts. Health indicates public configuration presence, not live DB/schema readiness. Native-to-Unity IDs/return/relaunch protocol and Android callback/auth UI are unchanged.

Stolen retired DB credentials cannot authorize training access in the tested design. A compromised backend can misuse valid user bearers it observes until expiry/revocation; it cannot be certified harmless. Gateway/admin/signing-authority compromise remains outside this guarantee. No admin/gateway credential may be readable by the app. Production Android/API transport must use HTTPS; local debug HTTP is not a production security configuration. Hosted provider full-path TLS remains unverified.

Install requirements-dev.txt and run pytest tests -q. Active backend tests use local/mock Auth transports, not live Supabase accounts. Stage05B2 evidence additionally includes native PostgreSQL17.11/PostgREST16.4 JWT and RLS tests; it does not certify the hosted provider version/configuration. Local lab credentials and issuer keys stay private. Live email/refresh/Android→Unity and remote allowlist checks remain pending approval/configuration.

## Step 3 mission endpoints (2026-10-17, local only)

Added with the same bearer → Auth verification → revocation check → `mission_api` RPC chain: `POST /v1/mission-sessions/{id}/commands` (semantic commands: board_rover, exit_rover, scan_target, weather_changed, pause, resume; idempotent on command_id, 409 on revision conflict, 500 per session), `GET /v1/mission-sessions/{id}`, `GET /v1/reports/{id}`, `GET /v1/skills`, `GET /v1/recommendations`. Reports and skills return evidence counts only (`score`/`levels` are null, `rubric_status: pending_approval`); the recommendation is a labelled placeholder. Candidate SQL: `MissionRedHorizon/Docs/Step3/SQL/005_mission_commands_reports.sql` (+ `905` rollback), not executed anywhere. Tests: `tests/test_mission_commands.py`. `tests/local_mock_server.py` serves the real routes with mock Auth/in-memory data on 127.0.0.1 for client tests only.

## Step 4 mission rules (2026-10-17, isolated, proposed defaults)

`rules/` holds config-driven engines for suit oxygen, Mars weather scenarios, mission scoring, skill levels and rules-based recommendations (`mission_rules_v1.json`, labelled PROPOSED). They are not imported by `main.py`; no endpoint, migration or scene uses them yet. Scores are computed only from server-recorded commands and server timestamps. Design: `MissionRedHorizon/Docs/Step4/Mission_Rules_Design.md`. Tests: `tests/test_rules.py`; `PYTHONPATH=. python tests/test_rules.py` regenerates `rules/test_vectors_v1.json` for the Unity cross-check.
