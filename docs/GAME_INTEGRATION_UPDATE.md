# Missions game and native host integration update

[README](../README.md) · [Setup](SETUP_GUIDE.md) · [Account identity](ACCOUNT_IDENTITY.md) · [Team co-op scope](TEAM_COORDINATION.md)

## Actual result — 27 October 2026

The real Missions gameplay scene now has a reproducible native account-host export/build path. The duplicate mission-retry serialization field is fixed. Development APK compilation/package checks passed. The full graphical mission suite ran twice: **292 PASS / 1 FAIL / 1 UNTESTED** each time. **This is not an all-green release sign-off.** Original project/source folders were preserved; builds/tests used separate staging copies of the repository source.

| Check | Observed result |
|---|---|
| Unity compilation + Android IL2CPP export | PASS; `ASTROGATEWAY_MISSIONS_BUILD Succeeded` |
| All six mission chain | PASS: EVA pickup, boarding/driving, charging, generator repair, storm/survival/retry, checkpoints, final docking and completion state/save |
| Mission retry serialization regression | PASS; derived mission retry is explicitly runtime-only, separate from inherited sensor-sync retry |
| Native descriptor checks | PASS: bounded UUID context accepted; expiry and invalid owner rejected |
| Controls/HUD/navigation/settings regression suite | Included in 292 passing checks |
| No-console-errors gate | FAIL: Unity Editor Search startup `ArgumentOutOfRangeException` in `UnityEditor.Search.SearchDatabase`; repeated after disposable Search cache regeneration |
| Virtual touchscreen BRAKE press | UNTESTED: Editor did not route the synthetic touch; direct widget handler checks passed |
| Native host Gradle APK | PASS, including Kotlin compilation and actual Missions library |
| Export wiring regression | PASS: fresh settings supersede old backup, repeated wiring is stable, missing Missions export stamp is rejected |
| Physical Android account/game lifecycle | NOT TESTED: no device/emulator available |
| Live account/score synchronization | NOT VERIFIED: existing Supabase target remains unavailable; no hosted configuration was changed |

The first batch-mode attempt was stopped when its rendering-driven harness stalled; it is not counted as a completed run. Graphical runs were resumed after the Editor's Search exception triggered Error Pause. The error was **not filtered out**, and the no-console-errors check remains failed. See the local validation artifacts for the complete results; raw machine logs, account configuration, generated export artifacts and APKs are not committed.

## Source changes

- [MissionsHUD](../01-Game-Development/MissionRedHorizon/Assets/AstroGateway/FirstMission/Scripts/HUD/MissionsHUD.cs): the mission failure retry remains a separate runtime-created control; Unity no longer tries to serialize two inherited fields named `RetryButton`.
- [NativeMissionBridge](../01-Game-Development/MissionRedHorizon/Assets/AstroGateway/FirstMission/Scripts/NativeMissionBridge.cs): installs only in Android when hosted by the private native Unity Activity and running the Missions scene. Scene roots wait before Start until a native descriptor arrives. Originally inactive roots remain inactive.
- The descriptor carries version, launch/session/user UUIDs, the existing compatible mission identifier and an expiry at most one hour away. No password/access/refresh token is sent to Unity. UUID/expiry checking alone is **not JWT authentication**; the private native host remains responsible for account/session validation.
- FirstMission progress, Missions progress and sensor files use an account-specific local namespace. Settings remain device-local. Old anonymous saves are preserved in their old namespace and are not assigned to an account automatically. Existing legacy world-state/preferences are not claimed to be a cloud profile or a complete cross-device save system.
- A warm new native launch reloads scene objects before rebinding; expiry disables the scene. Six local missions emit `LOCAL_MISSIONS_COMPLETE`, never the old immediate `POC_RESULT_OK`. The native app does not submit that as a server-verified score or as an integration-test completion.
- [MissionsAndroidBuild](../01-Game-Development/MissionRedHorizon/Assets/AstroGateway/Integration/Editor/MissionsAndroidBuild.cs): builds/exports the actual Missions scene with Android Activity entry and ARM64, sets product identity/version, and stamps successful exports. The host build refuses the historical cube export by default.
- [Native host tools](../02-Software-Development/Integrations/android-unity-host-poc/tools): fresh Unity settings backup/wiring, configurable installed tool paths and an export-wiring regression test. The historical cube-only device script stops by default because its immediate-result expectations do not apply to real missions.
- The main project's default enabled build scene is now Missions, replacing SampleScene. Company/product identity and Android package/version settings are explicit in the repository project as well as in the exporter.

## APK identity and migration limits

- Application ID: **`com.sk2lb.astrogateway`**; internal Kotlin namespace stays `com.sk2lb.astrogateway.poc` to preserve Activity/JNI class compatibility.
- Native host: **versionCode 3 / versionName `0.3-missions`**; Unity product version `0.3.0`.
- Main launcher: `com.sk2lb.astrogateway.poc.MainActivity`; embedded Unity Activity remains private in `:unity`.
- ARM64; minimum SDK26 and target SDK36 in development configuration.
- **Debug/development signing only.** No release keystore was read/created, no store release was published and no production readiness is implied.
- The old `.poc` app is a different application. Existing account sessions/local data are not silently migrated between app IDs. Users must sign in again in the new app. The company/product namespace change also means old standalone local preferences are not silently imported.

## Reproduce safely

Use a complete staging checkout/copy with the installed Unity 6000.6.4f1 Android modules. Do not run an exporter against an original source folder that must remain unchanged. `ASTROGATEWAY_GAME_PROJECT` must point at the staging primary game project. The native host's `tools/export.sh` exports that game to its sibling `unity-export`, then `tools/build.sh` produces `app/build/outputs/apk/debug/app-debug.apk`.

The checked-in historical `unity-export` is not the new Missions build. Regenerate it before building; the new stamp guard intentionally rejects stale cube exports. `tools/tests/test_prepare_host.py` can be run with Python's standard library, without a device or network.

For the graphical suite, set `FM_SCENE=Assets/AstroGateway/FirstMission/Scenes/Mars_FirstMission_Missions.unity`, `FM_OUT` to a private external report folder, and `FM_QUIT=1`; invoke `AstroGateway.FirstMission.EditorTools.FirstMissionPlaytest.Run` in a **graphical** staging Editor. Test results include mocked uploader checks; they do not exercise real Supabase or Android touch input.

## Device and live acceptance still required

1. Resolve/confirm the approved Supabase project and use an HTTPS backend reachable by the phone. `10.0.2.2` is an emulator address, not a physical-phone backend.
2. With real verified test accounts, launch from the dashboard; confirm the Missions scene ACK. Return, relaunch, expire/revoke the session and switch between two account UUIDs. Confirm local mission/sensor data does not carry over and no old scene remains accessible after a new context.
3. Complete the full mission on Android; verify the local-completion message and confirm **no server training score is falsely awarded**. Check native/Unity process death and recovery.
4. Test touch/brake/joystick/camera, supported devices, frame time, memory, battery/thermal behavior, safe area and Android Back. Editor measurements are not phone benchmarks.
5. Fix or isolate the Unity Search startup defect through a verified Editor/project solution, then rerun the unchanged no-console-errors gate. Do not label the present result clean.

## Major scope that remains unfinished

Game API adapters/authoritative score synchronization are still not wired into the actual gameplay session; the existing rubric has not become a verified live scoring pipeline through this update. Same-world 2–4 player multiplayer, invitation/team persistence, chat, voice, screenshot sharing and screen sharing remain **not implemented/deployed**. They require the separately documented [team architecture and acceptance gates](TEAM_COORDINATION.md), a reachable identity/database target, game-server hosting and media infrastructure. Native username/photo UI and final art acceptance also remain separate work. No completion is claimed for those features in this commit.
