# Stage05B.2 — Impersonation fix report

2026-10-09. **Original DB-credential identity-setting flaw: closed in local tests. Remote execution: BLOCKED.** No live Supabase changes/users/requests were used by validation.

## Change and evidence

Replaced the shared asyncpg repository and custom astrogateway user/session GUC writes with a fixed typed PostgREST repository. FastAPI no longer has an SQL pool, raw-DB fallback or runtime DB dependency; nonempty DATABASE_URL/DB certificate settings fail configuration. Independent gateway receives original user JWT and verifies it before native RLS. Existing API endpoint/body/response contracts are preserved. Request-level JWT verification remains in FastAPI as a second check, not the sole authorization boundary.

Candidate SQL creates private training + exposed mission_api invoker functions and dedicated NOLOGIN schema ownership. It derives identity/session/revocation expiry from gateway claims, independently checks confirmed/non-anonymous Auth state, and denies caller owner/session overrides. Retiring legacy roles removes table and column ACLs, memberships and raw login; exact legacy sessions are terminated to close retained SET ROLE authority. Prototype rehearsal caught an index-column revoke filter and a PL/pgSQL variable alias conflict; both were corrected before final testing. Completion RPC now validates context before locking so the existing wrong-session conflict contract remains available while update RLS still binds it.

Evidence: `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B2/Evidence/native-setup.json` records old credential could impersonate before quarantine, then saved password rejected and stale authorized session closed. `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B2/Evidence/security-regression.json` records 53 passing gateway/native attack checks. `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B2/Evidence/api-gateway-integration.json` records 16 passing API/gateway checks, including bypassed host verifier rejection. `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B2/Evidence/repeat-rehearsal.json` records 8 passing collision/rollback/reinstall checks. Backend active suite: 48 PASS. No unexecuted test is labelled pass.

## Actual local files changed

Under `/Users/kapilash/Projects/AstroGatewayAndroid/backend/`: database.py, main.py, run_local.py, .env.example, requirements.txt, README.md, tests/test_security_contract.py, tests/test_hardening.py; added tests/test_postgrest_repository.py. No new production dependency; asyncpg removed from app requirements, retained only in the existing private test tooling for native DB rehearsal. Old direct-role/TLS tests are archived in `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B2/Before` because that production path was removed; active JWT/contract tests retained and new repository/configuration checks added. Current and prior source copies are reviewable in Source/Before with hashes.

Private auth.local.properties, backend/.env and database/admin.local.env were not edited or displayed. Original migration source remains untouched; new candidate is separately pinned. Android/Kotlin UI/callback/navigation, Unity bridge/scenes, Mars assets/design and React source were not modified. No APK rebuild/runtime Android validation was needed or claimed for this backend-only redesign.

The stale localhost8000 pre-redesign FastAPI process was identified and stopped; no real-service replacement was started. The actual project startup diagnostic correctly rejects its readable admin-file layout (`/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Stage05B2/Evidence/actual-service-isolation.json`). It is NOT a sandbox or proof a compromised same-account process cannot read credentials.

## Remaining blockers and risks

1. Establish separate service OS/container filesystem authority with no readable administration/gateway/signing files, then verify confinement. Do not bypass the guard or delete operator credentials just to start it. Current same-account deployment cannot satisfy the full-compromise threat model.
2. Valid bearer theft/replay remains possible; B bearer replay was demonstrated. This design prevents inventing B authority from arbitrary IDs/retired DB credentials, not misuse of B authority already stolen. If the requirement includes preventing all misuse by a backend that sees B tokens, stronger client/data isolation is needed; do not claim this resolved.
3. Refresh target read-only catalogue and all exposed-schema RPC/default/grant inventory; validate provider Auth/role/helper compatibility and private definer owners. Hosted stack/version/DDL rights are not proven by local native tests.
4. Obtain backup/recovery and target-specific rollback plan. Fresh candidate stops on existing training/mission_api/owner objects. Do not apply it as an in-place old-schema upgrade or revert to insecure GUC policies.
5. Remote redirects/email/live signup/refresh/isolation/Android→Unity and full-path hosted TLS remain unverified. Real test accounts require separate approval/inbox access.

**Stop:** local implementation/rehearsal is complete; native lab services shut down. No remote SQL/roles/configuration/accounts/production changes. A separate explicit approval of the revised hashes/role-session termination/Data API exposure and rollback scope is required after deployment gates pass.
