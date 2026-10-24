# AstroGateway — Stage 05A

## Result
Native Android foundation implemented around the existing Unity-as-a-Library integration. Authentication is **not implemented**. All training/profile figures are demo data. The mission launch opens the existing temporary Unity cube integration scene, not the production Mars environment.

Project: `/Users/kapilash/Projects/AstroGatewayAndroid`
Unity source preserved: `/Users/kapilash/Projects/MissionRedHorizon`
React source preserved: `/Users/kapilash/Projects/astrogateway`

## Screens

| Screen | Implemented behavior |
|---|---|
| Splash | Branded launch content, then welcome; does not initialize Unity in advance |
| Welcome/onboarding | Scientific training introduction; explicit preview notice; demo, sign-in and account UI routes |
| Sign-up | Name/email/password UI, disabled to prevent credential collection; unavailable-auth dialog; explicit demo entry |
| Sign-in | Disabled email/password UI; unavailable-auth dialog; explicit demo entry |
| Dashboard | Cadet 07 placeholder, illustrative readiness, mission, scientific skills, recent-mission empty state, bridge status/result and training/profile/settings actions |
| Mission selection | Mission Red Horizon preview; unavailable future missions |
| Mission launch | Integration-scene disclosure; existing Activity launch with original mission descriptor; handshake status/result |
| Astronaut profile | Clearly identified demo identity and learning focus |
| Training progress | Illustrative skill progress; no real scores, certification or completed mission claims |
| Settings | Persisted reduced-motion preference; privacy/version information; simulator graphics/audio controls explicitly unconnected |

## UI and architecture

- Existing Kotlin native Views architecture, application ID and two-Activity/process model retained. No new dependencies, Unity export or integration scene changes.
- Shared text, label, card, button, field and progress helpers provide one visual system across the ten screens: deep navy, pale text, muted secondary text and restrained ochre accents. Android system typography; no imported assets or fonts.
- Scrollable, centered content capped at 640 dp; minimum 56 dp action targets; system-bar/cutout insets; scalable text; disabled account fields have accessibility descriptions.
- `MainActivity` owns a small saved navigation stack. Demo entry and Unity return reset to dashboard; root Back backgrounds the task. The stack and active launch identifier survive normal Activity recreation.
- Android 13+ native Back uses `OnBackInvokedCallback`; Android 14+ uses `OnBackAnimationCallback` with cancel/reset handling and reduced-motion support. Older Android keeps the legacy Activity Back path.
- `PocUnityActivity` retains Unity warm and paused on return. Its native dashboard overlay owns an overlay-priority Back callback; legacy Back key events are consumed before Unity injection. Android navigation remains visible beside the return control.
- Mission ID `mission-red-horizon-poc`, JSON protocol version 1, UUID correlation, bounded messages, package-restricted broadcast, and `UnityPlayer.UnitySendMessage` receiver remain intact. No credentials enter the bridge.
- Existing READY/ACK/RESULT/ERROR states remain available; launching shows the pending handshake; missing Activity provides an error. Account actions explain the unavailable service. No backend requests exist.

## Files changed

Relative to `/Users/kapilash/Projects/AstroGatewayAndroid`:

- `app/src/main/java/com/sk2lb/astrogateway/poc/MainActivity.kt`: native screens, reusable UI helpers, navigation and retained host bridge handling.
- `app/src/main/java/com/sk2lb/astrogateway/poc/PocUnityActivity.kt`: dark return overlay and scoped Back/navigation fixes; original bridge and Unity lifecycle preserved.
- `app/src/main/AndroidManifest.xml`: product label and native dark theme.
- `app/src/main/res/values/astro_theme.xml`: new native theme.
- Generated `app/build/outputs/apk/debug/app-debug.apk` and ordinary Gradle intermediates.
- Prior Kotlin/manifest copies retained under `Stage05A/backup/`.

Validation scripts are workspace artifacts under `work/stage05a/`; evidence, APK copy and screenshots are under this report's directory. No original Unity/React source was edited. `preservation.json` compares 337 original files with the prior validation baseline: zero changes.

## Validation evidence

Final build and runtime results: see `build.log`, `validation.json`, `validation-run.log`, `runtime-logcat.log`, `responsive-validation.json`, `final-overlay-validation.json`, `final-overlay-logcat.log`, `final-exit-info.log` and `final-system-errors.log`.

| Check | Observed result |
|---|---|
| Debug build / emulator install | Successful; final `build.log` reports BUILD SUCCESSFUL |
| All ten screens | Opened and captured; unchanged native-screen source in the final APK |
| Native Back / navigation stack | Child Back, mission stack, profile edge gesture, root task background and launcher re-entry passed |
| Unity bridge and relaunch | Three full-screen regression cycles passed: button, Back key and edge gesture; both host and Unity process IDs retained |
| Final inset correction | Two additional final-APK cycles passed: button and gesture, including ACK, RESULT and process reuse; full 56 dp return target observed at bounds [675,827,1665,981] |
| Responsive / preferences | 1.3× text and 1280×800 dp emulated tablet checked; reduced-motion preference survived re-entry; emulator settings restored |
| Crash / ANR checks | No fatal exception, native fatal signal or app ANR observed in test windows; nonfatal Unity warnings remain |

`validation.json` records the complete native-screen and three-cycle test on APK SHA-256 `8565d2e9acd90d572fe2c143c189346934e11a3a1aa188ded7767d52ae3f7d49`. The only subsequent source edit moved the Unity return overlay above navigation insets. That final APK is separately tested in `final-overlay-validation.json` and `final-overlay-logcat.log`; its SHA-256 is `56d812ce1af014320e821a8b27ee9533ab7c0eabd4cf81ae1d604060545b7ade`. The final APK copy is `AstroGateway-Stage05A-debug.apk`. The actual project build output remains `/Users/kapilash/Projects/AstroGatewayAndroid/app/build/outputs/apk/debug/app-debug.apk`.

Screenshots: [gallery](Screenshots.html), [dashboard](screenshots/05-dashboard.png), [mission launch](screenshots/07-launch.png), [embedded Unity](screenshots/11-unity-bridge.png), [tablet viewport](screenshots/15-dashboard-tablet.png), [displayed bridge result](screenshots/17-dashboard-bridge-evidence.png). Every required screen appears in the gallery.

Testing uses the already installed, authorized `emulator-5556`: `sdk_gphone64_arm64`, Android 16 / API 36, ARM64 Google APIs image. These are emulator results; no physical-device testing is claimed.

The initial synthetic edge swipe started at x=0 and did not dispatch Back. The emulator SystemUI dump showed an enabled 82-pixel edge region and a narrower 33-pixel ML region. An injected swipe starting at x=60 was then observed returning successfully in both native and Unity views. The test coordinate was corrected, and the final regression run uses that coordinate. The original failed attempt is preserved in `initial-validation/`; it is not counted as a pass.

## Known limitations

- No real authentication, backend, mission gameplay, production Mars scene launch or account persistence. Demo entry is explicitly separate from authentication.
- APK remains an ARM64 debug build; package ID keeps the existing `.poc` suffix for integration continuity. Release signing, store distribution and other ABIs are outside this stage.
- Android 16 emulator validation does not establish physical-device GPU/performance behavior or runtime compatibility across every supported Android version. TalkBack, exhaustive process-death recovery and predictive gesture cancellation are not fully tested.
- Existing Unity export logs a nonfatal missing Play Core asset-pack class, cookie texture fallback and unsupported/stripped post-processing warning. This POC does not use asset packs. No dependencies were added solely to suppress these logs.
- Main Activity screen/bridge code is deliberately compact and dependency-free; broader feature development should extract screen presenters and resource-based localization before adding real account workflows.

## Reference for Back handling

Android's official [OnBackInvokedDispatcher reference](https://developer.android.com/reference/android/window/OnBackInvokedDispatcher) defines overlay priority for overlay controls; the native dashboard return control uses this priority ahead of Unity's own callback. [OnBackAnimationCallback](https://developer.android.com/reference/android/window/OnBackAnimationCallback) supplies gesture progress and cancellation for the native interface.

Stage 05A ends here. Real authentication, backend integration and subsequent implementation require the next approval.
