# Native control, survival and telemetry foundation — 2026-10-13

## Implemented first vertical slice

Scene: /Users/kapilash/Projects/MissionRedHorizon/Assets/AstroGateway/NativeControlSlice/Mars_NativeControl_VerticalSlice.unity

Astronaut movement through native touch input → approach/board existing prototype rover → seated persistent astronaut → drive/brake → live battery/speed/health → safe exit → simulated scan → data panel → separate local save/load.

Only Assets/AstroGateway/NativeControlSlice was added. 403 pre-existing Assets/Packages/ProjectSettings files matched their backup hashes; original Mission01 and Mars content preserved. Web, Android host, backend, Auth and remote services were not edited.

MissionRegistry is an editable ScriptableObject with stable mission ID, display/vehicle names, descriptions, objective labels, difficulty and provisional reward. Names are configurable proposals, not actual NASA vehicle identities. Keep missionId stable once saves exist. Registry is a single slice definition, not a full multi-mission database.

ControlModeManager is the central input bus. Astronaut/Rover/Menu exist; Robot/Cockpit are explicitly rejected until implemented. AdaptiveTouch uses Unity Input System EnhancedTouch, floating joystick, separate camera-finger ownership, contextual run/acceleration/brake/board/scan, focus clearing, safe-area bounds, Android landscape orientation, inspector left-handed/size/sensitivity/opacity configuration. PC WASD/Shift/RMB/E/Space brake/K scan/1–3 weather preserved in the new scene. No additional packages.

The existing MarsPrototype override-input hook supplies locomotion and chase camera behavior. Boarding attaches the same astronaut object to a seat with its controller disabled and visible; exit detaches it and enables locomotion at the validated exit location. It is not a finished boarding animation or cockpit-camera implementation. Original scene behavior stays intact.

Telemetry has stable field/source IDs, units, simulation ticks and valid_simulated quality. Bounded 256-record history and 1 Hz sampling; rover battery consumption is synthetic (0.04 simulation percentage points/second while aboard), speed derives from the existing controller, health starts at 100 without a damage model. Existing oxygen/power remain prototype resources. None are calibrated engineering predictions or server-authorized scores.

Scan records include GUID, location, tick, placeholder sol 1, simulated instrument/type/analysis and deterministic synthetic index. JSON/CSV scan export and JSON telemetry-history export are user-triggered local files. No upload, credentials, account IDs or automatic share action. Native OS share sheets are deferred.

## Actual results

- New slice: 26/26 controlled Unity Play Mode checks PASS.
- Preserved Mission01: 13/13 controlled checks PASS.
- Preserved Mars: 20/20 controlled checks PASS.
- New scene dependency checks, save/reload and compilation PASS in refreshed project copy.
- Screenshot: Screenshots/NativeSlice_Game.png is an actual Unity Play Mode framebuffer capture, including prototype UI. The negative save test's invalid-snapshot notice is expected; it is not presented as a successful load screenshot.
- Physical Android/multitouch ergonomics/FPS UNTESTED. ADB listed no devices. No APK generated or installed.

These are automated synthetic touch/input and simulation tests in work/stage08b/PlayModeLab, not physical fingers or freshly executed hands-on main-Editor tests. Copying validated assets into the main project does not establish Android or production readiness.

## Deferred stages

30-Sol survival, configurable physical resource/replenishment model, failure/recovery/research completion, robotics/upgrades/tuning, full camera suite, animated transitions, ORBIS diagnostics, launch/cockpit/countdown/flight/landing integration and native sharing remain architecture/planned work. Original Mission01 launch flow remains separate; it was regression-tested, not connected to new flight systems. No trained model or authoritative mission result claimed.

Initial EnhancedTouch naming and obsolete GetInstanceID compiler errors were fixed before integration. The final log retains a Unity QuickSearch indexing exception; do not claim a fully clean Editor Console. HUD overlap discovered in the first actual capture was corrected and retested. All derivative evidence/logs retained.

See Architecture_And_Plan.md, Telemetry_And_Save_Contract.md, Android_Manual_Test_Plan.md, Test_Results.md, Launch_Instructions.md and Known_Limitations.md. Stop after this slice.
