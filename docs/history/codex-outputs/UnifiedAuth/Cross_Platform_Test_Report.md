# Cross-platform validation

| Requirement | Local evidence | Hosted status |
|---|---|---|
| Signup / email verification | Web SDK calls and PKCE verification tested with mocks; existing Android implementation inspected | BLOCKED |
| Login / invalid credentials | Web UI + backend safe rejection tests | BLOCKED |
| Recovery routing / new password | Pending-flow binding, malformed/unbound callback rejection, StrictMode single exchange, synthetic password update tests | BLOCKED email delivery/allowlist |
| Protected screens | Actual signed-out browser dashboard gate; Android synthetic protected editor; backend 401 | PASS locally |
| Profile save/persistence | Web mock save + canonical reread; real local SQL/gateway two-user persistence | BLOCKED on hosted Android save |
| Cancel / loading / errors | Web simulated save/error/cancel tests; six Android UI checks | PASS locally |
| Logout / refresh / expiry | Web simulated SDK refresh, expiry denial, recovery gate and logout/refresh race; backend revocation | BLOCKED hosted lifecycle |
| Cross-user isolation | Real local RLS/gateway and API tests | BLOCKED hosted RLS |
| Shared UUID/project | Public configuration comparison and both client ownership contracts use auth.users UUID | Contract PASS; real shared account unverified |
| Mars preservation | Original App.tsx and Unity playable scene/controller SHA checks unchanged; actual browser render captured | PASS preservation |
| Android ↔ Unity authenticated flow | Bridge source untouched; mission API regression passes locally | No new authenticated device Unity test |

Browser captures are actual local UI with no credentials entered. The dashboard capture shows the signed-out protection state, not a fabricated logged-in screen. Android editor screenshot is an actual rendered test screen with explicitly synthetic data. No live Supabase account registration, verification email, recovery email or cross-platform login was performed.

Responsive test: 390×844 requested viewport, signup document width 390 (no horizontal overflow), temporary override reset. Actual `/mars` render and welcome/login/signup/recovery navigation were inspected. Local FastAPI /health and Vite proxy return ok; unauthenticated /v1/me returns401. Health indicates public settings/startup only; provider connectivity remains constrained and unverified.

## Reproduction

Web: `cd /Users/kapilash/Projects/astrogateway` then `npm test`, `npm run build`, `npm run lint`, `npm run dev -- --host 127.0.0.1 --port 5173`. Open `/welcome`; `/` remains Mars. Unit tests substitute Auth/fetch and never create accounts.

Backend mock tests: `PYTHONPATH=/Users/kapilash/Projects/AstroGatewayAndroid/backend /Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/work/stage05b/venv/bin/python -m pytest /Users/kapilash/Projects/AstroGatewayAndroid/backend/tests -q`. The existing backend/.venv lacks pytest; the established test environment was reused rather than reinstalling dependencies.

Local rehearsal requires the existing disposable PostgreSQL lab and renewed localhost TLS: [rehearse_profile.py](/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/work/unifiedauth/rehearse_profile.py). It is a one-shot fresh candidate application; an installed-object collision intentionally aborts. Review current lab state and roll back using the local-only SQL before a repeat; never point it at Supabase. [isolated_regression.py](/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/work/unifiedauth/isolated_regression.py) runs confined backend/probe/gateway tests with synthetic JWTs.

Android: build with `zsh /Users/kapilash/Projects/AstroGatewayAndroid/tools/build.sh`. Test APK only: Gradle `:app:assembleDebugAndroidTest -PastroGatewayTestRunner=com.sk2lb.astrogateway.poc.ProfileEditSmokeInstrumentation`. Install APKs on the existing authorized emulator, then `adb shell am instrument -w com.sk2lb.astrogateway.poc.test/com.sk2lb.astrogateway.poc.ProfileEditSmokeInstrumentation`. The default test runner remains the previous security/Unity runner.
