# Unified Web + Android account experience — final report

## Local implementation and validation complete; live acceptance BLOCKED

Implemented responsive web welcome, signup/login, email-verification handling, password recovery/callback, protected dashboard, profile view/edit and logout. Added Android display-name editing with save/cancel, validation, busy/error states and canonical profile refresh. Both clients retain the same Supabase project and Auth UUID contract.

Backend adds only POST /v1/me/display-name using verified original JWT → independent PostgREST → own-row RLS. New SQL was rehearsed solely on disposable localhost PostgreSQL; no remote migration, role, account or hosted setting was changed. Existing Unity integration and Mars sources are preserved. The Android game export is still its temporary integration scene.

Local results: 34 web tests; 68 backend tests (also repeated under confinement); 30 database/gateway checks; 21 isolated API integration checks; 18 runtime-isolation probes; six corrected synthetic Android editor UI checks. Web build/lint and Android debug/test APK builds pass; emulator installation succeeds. Initial Android instrumentation startup ANR is retained and explained in the Android report; the corrected run passes. These counts include different test layers and are not proof of live cross-platform authentication.

Open [account workspace](http://127.0.0.1:5173/welcome). Mars remains at [original root](http://127.0.0.1:5173/) and [Mars route](http://127.0.0.1:5173/mars). Confined local API health/proxy startup passed, but its established local-only egress prevents hosted Auth/data verification. No broad network exception was introduced.

## Deliverables

- [Architecture_Report.md](/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/UnifiedAuth/Architecture_Report.md)
- [Web_Implementation_Report.md](/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/UnifiedAuth/Web_Implementation_Report.md)
- [Android_Profile_Edit_Report.md](/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/UnifiedAuth/Android_Profile_Edit_Report.md)
- [Shared_Identity_Contract.md](/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/UnifiedAuth/Shared_Identity_Contract.md)
- [Security_Test_Report.md](/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/UnifiedAuth/Security_Test_Report.md)
- [Cross_Platform_Test_Report.md](/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/UnifiedAuth/Cross_Platform_Test_Report.md)
- [Screenshots](/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/UnifiedAuth/Screenshots)
- [004_profile_display_name.sql](/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/UnifiedAuth/SQL/004_profile_display_name.sql) and [904_profile_display_name_rollback.sql](/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/UnifiedAuth/SQL/904_profile_display_name_rollback.sql)
- [modified-source-manifest.json](/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/UnifiedAuth/Evidence/modified-source-manifest.json) lists exact changed source paths/hashes.

## Remaining operator/hosted dependencies

1. Resolve Stage05B3 hosted service-isolation/network/readiness gates with a supported approved environment. Current local confinement intentionally does not permit hosted egress.
2. Review the existing revised base migration plus new display-name SQL, target collisions, grants/schema exposure, backups/restoration and rollback. Obtain separate explicit approval before any remote SQL/cache/exposure change.
3. Independently verify Email Auth/confirmation policy and email delivery. Proposed web allowlist URL: `http://127.0.0.1:5173/auth/callback`; existing Android callback: `astrogateway://auth/callback`. No dashboard evidence is available and no settings were changed. Use the exact current local origin if choosing another port.
4. Approve two disposable non-production test accounts with verification/recovery inbox access. No account/password is invented or requested through logs/reports.
5. Then test same-account web↔Android login, live canonical profile synchronization, reset/refresh/logout/expiry, two-user hosted RLS and authenticated Unity launch/return/relaunch. Do not treat synthetic UI states or local RSA fixtures as live acceptance.

Production CSP/HTTPS, supported service confinement, browser token-storage risk review and physical-device QA remain outside this local stage. Stopped without remote changes.
