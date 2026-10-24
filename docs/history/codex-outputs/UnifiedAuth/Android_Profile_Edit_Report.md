# Android profile editing

The existing Profile screen now opens a protected Edit profile screen. The current display name is prefilled; Save validates 2–64 Unicode code points after trimming and rejects control characters. Cancel discards the draft and returns through the existing navigation stack. Save/Cancel are disabled while busy. The draft survives state-driven redraws; safe errors are visible with retry enabled.

AuthCoordinator verifies the current Supabase user and canonical profile before POST `/v1/me/display-name`, checks returned ownership, then GETs `/v1/me` again. The refreshed profile updates the existing dashboard/profile. Session generation and user-ID checks prevent a late result from being applied to a changed account. No user ID, training level or email is submitted by the edit form.

Changed production files: [MainActivity.kt](/Users/kapilash/Projects/AstroGatewayAndroid/app/src/main/java/com/sk2lb/astrogateway/poc/MainActivity.kt), [AuthCoordinator.kt](/Users/kapilash/Projects/AstroGatewayAndroid/app/src/main/java/com/sk2lb/astrogateway/poc/AuthCoordinator.kt). Existing signup/login, password recovery, session vault, Back handling and Unity bridge are retained. App Gradle adds an optional test-runner property; its default remains the original security/Unity runner.

Debug APK build and emulator installation: PASS. APK: /Users/kapilash/Projects/AstroGatewayAndroid/app/build/outputs/apk/debug/app-debug.apk. Emulator: sdk_gphone64_arm64, Android 16/API36, software-rendered; no physical-device result is claimed.

Six corrected instrumentation checks passed: prefill, cancel/no mutation, invalid short name, loading disabled controls, synthetic error/retry, and signed-out protection. The TEST APK temporarily injects synthetic UI StateFlow data via reflection; it installs no SDK session and never contacts hosted Auth. Test code is excluded from the application APK. It does not prove a successful Android save round-trip or real authentication.

Initial synchronous-start instrumentation ANRed with “failed to complete startup”; the emulator killed it. The retry used asynchronous activity monitoring and passed with INSTRUMENTATION_CODE -1. No Java exception was captured; the underlying initial startup failure is not conclusively attributed. Evidence and limitations remain recorded rather than erased.

Actual screenshot: [Android_Profile_Editor_SYNTHETIC.png](/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/UnifiedAuth/Screenshots/Android_Profile_Editor_SYNTHETIC.png), explicitly marked “NOT LIVE AUTHENTICATION” in the UI. The app was returned to its unauthenticated welcome state afterward. Normal welcome startup was separately observed. Successful authenticated device save and shared-account Unity regression remain BLOCKED pending hosted prerequisites; no live credentials were entered.
