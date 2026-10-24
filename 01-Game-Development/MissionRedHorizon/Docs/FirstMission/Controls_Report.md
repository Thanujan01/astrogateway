# AstroGateway control system upgrade — report (2026-10-20)

Requested by SSL on 2026-10-20. All work is in the isolated First Mission scene. Shared controllers (MarsPrototype, NativeSliceGame, HUDInputRouter, AstroGatewayHUDController, VisualUpgradeControlAssist, VisualUpgradeCameraRig), the production and Visual Upgrade scenes, project settings and the backend were not modified. Nothing was pushed, merged or deployed.

## Phase 0: recovery
- Repository `~/Projects/MissionRedHorizon`, branch `visual-upgrade` @ `5e0c223`. Tracked files were clean, and First Mission was untracked as before.
- Nothing was running (no Unity, mock server or uvicorn), so no process was changing files.
- New verified backup: `~/Projects/MissionRedHorizon_Backups/Controls_PreChange_20261020/`. It holds `repo_all.bundle` (verify okay), `firstmission_untracked.tgz` (346 entries), `backend_snapshot.tgz` and `SHA256SUMS.txt`.
- "Before" baseline: First Mission playtest 105/0/1 and Visual Upgrade playtest 50/0, both from earlier today.

## Phase 1: input architecture
Unity Input System 1.20.0. Player setting `activeInputHandler: 1` means the Input System only. `InputSystem_Actions.inputactions` (Unity template) is **not used** by any script; all input is read directly from devices. uGUI 2.6 with the Input System UI module.

Order of execution each frame in the First Mission scene:

| Order | Component | Reads | Writes |
|---|---|---|---|
| -200 | HUDInputRouter (shared) | Keyboard (WASD, Shift, Space, E, K, 1-3, Esc, F1), right-mouse look, `hud.movement/lookDelta/runHeld/brakeHeld` | `modes.Submit(NativeInput)` |
| -100 | CinematicGameAdapter → NativeSliceGame (shared) | `modes.Input` (its own keyboard read is suppressed by the adapter) | `MarsPrototype.testMove/testLook/testRun/testBrake` |
| -50 | VisualUpgradeControlAssist (scene) | arrows, gamepad | eases keyboard walk / steer |
| **-40** | **FirstMissionControlLayer (new)** | the above | control state, look sensitivity / inversion / reduced motion, auto-sprint, speed-sensitive steering, UI-state input blocking |
| 0 | MarsPrototype (shared) | `test*` | walk/drive physics, oxygen, boundaries (unchanged) |
| 420 | ProductionModeCoordinator (shared) | C key | cinematic toggle |
| **520** | **FirstMissionCameraRig (new; replaces VisualUpgradeCameraRig in this scene only)** | yaw/pitch, views, zoom | camera |
| **720** | **FirstMissionHUD (new)** | touch widgets | `hud.movement/lookDelta/runHeld/brakeHeld` |

### Compatibility matrix
| Action | Keyboard / mouse | Gamepad | Touch (First Mission HUD) | Status |
|---|---|---|---|---|
| Walk / steer | WASD, arrows | left stick | MOVE / STEER stick (analog) | VERIFIED (keyboard, widget) |
| Run | Shift | RT | RUN, hold or toggle (setting), optional auto-sprint | VERIFIED |
| Jump | — | — | — | BLOCKED: no jump exists. It would be new traversal physics, which needs your approval |
| Board / exit | E (5 m rule) | A | context INTERACT button / EXIT ROVER | VERIFIED |
| On-foot scan | K | X | context INTERACT ("SCAN …") | VERIFIED |
| Throttle / brake / reverse | W/S, Space | RT / LT, B | ACCEL (progressive), BRAKE, REVERSE | VERIFIED |
| Camera orbit | right-mouse drag | right stick | drag on the gameplay zone | VERIFIED (sensitivity, invert) |
| Camera zoom | mouse wheel | — | two-finger pinch | VERIFIED (pinch through the zone's handlers) |
| Camera view / recenter | C (cinematic) | Y (cinematic) | CAMERA, RECENTER, VIEW | VERIFIED |
| Rover panels | Tab returns to DRIVE | — | DRIVE / SCIENCE / DATA / MISSION / SYSTEMS tabs | VERIFIED |
| Pause | Esc | Start | II button / Android back | VERIFIED |
| Settings | — | — | SYSTEMS › SETTINGS | VERIFIED |

Known defects found during the audit (not changed, shared code):
- The template `InputSystem_Actions.inputactions` is unused, so rebinding isn't possible.
- The game allocates about 300 KB of GC per frame in Editor Play Mode. The First Mission layers add roughly 0 to 7 KB of that, measured by switching them off and on. The main suspects are per-frame string building in shared code (for example the `MarsPrototype` status text). This needs a separate, reviewed fix.
- MarsPrototype's own keyboard path is dormant (`testControl`), which is correct but easy to misread.

## Phases 2–4: control layer (`FirstMission/Scripts/FirstMissionControlLayer.cs`, HUD changes)
| Feature | Status | Evidence |
|---|---|---|
| Explicit states ASTRONAUT / ROVER_DRIVE / ROVER_SCIENCE / DATA_REVIEW / PAUSE_MENU; each state passes only its own inputs | VERIFIED | 5 state checks; W does not drive in DATA or SCIENCE |
| Analog movement, camera-relative direction, smooth rotation, eased accel/decel, slope handling | VERIFIED (existing controller) | movement follows camera yaw within 25° |
| Walk / run toggle, optional auto-sprint | VERIFIED | run 3.80 vs walk ~2 m/s; auto-sprint above 3.2 m/s |
| One context-sensitive INTERACT button (BOARD / SCAN / INSPECT AIRLOCK) | VERIFIED | label and action checks |
| Jump, jump buffering, coyote time | **BLOCKED** | needs your approval (new traversal physics) |
| Adjustable input sensitivity, camera inversion, reduced motion | VERIFIED | x2 sensitivity rotates more than 1.6× as far; inversion flips the sign |
| Rover progressive throttle (0→1 in ~0.7 s) | VERIFIED | 0.3 s ≈ 0.4, 1.1 s = 1.0 |
| Speed-sensitive steering (stick authority falls to 60 % at top speed; turn-rate formula unchanged) | VERIFIED | 0.80 at 2.7 m/s matches the curve |
| Recenter, contextual "SCIENCE AT STATION" shortcut | VERIFIED | |
| Science: TARGET selection, CANCEL, range feedback, sensor status, sync state | VERIFIED | selected target is used in the records; cancel stores nothing |
| Speeds, turn rates, gravity, boundaries, oxygen, exit rules | unchanged | Visual Upgrade regression 50/0 |

## Phase 5: camera (`FirstMission/Scripts/FirstMissionCameraRig.cs`)
It reuses the Visual Upgrade collision solver (pull-in, pivot raise, sweep, never-inside fallback) and adds:
- Astronaut views: FOLLOW, SHOULDER (left/right setting) and INSPECT (first person; hides the astronaut).
- Rover views: CHASE, CHASE NEAR and **MAST CAM**. The ARES-VI canopy has no interior: a cockpit eye point rendered black (screenshot evidence), so the first-person rover view sits on the navigation mast instead. A true cockpit camera is PARTIAL and needs an interior model.
- Zoom ×0.6–1.6 (pinch or mouse wheel), recenter, and ROVER_SCIENCE framing of the selected station.
- View changes glide at no more than 9 m/s (measured peak 9.0 m/s; largest frame move 0.72 m).
- Reduced motion uses a fixed FOV and slower blends.
- **Crew Dome airlock regression: PASS.** Backing onto "Crew Dome closed door" for 67 frames, the camera was never inside geometry and the fallback was never used.

## Phases 6 and 8: mobile UI and settings
SYSTEMS › SETTINGS has 14 large buttons, saved locally in PlayerPrefs `AstroGateway.FirstMission.v1.Settings` with no secrets:
- left/right-handed layout
- joystick position (low / standard / raised)
- button size, opacity and look speed
- camera inversion and reduced motion
- auto-sprint and run hold/toggle
- shoulder side and graphics preset

The gameplay touch zone is separate from the buttons. Multi-touch uses pointer ownership per widget. There is an action-feedback toast. All touch targets are at least 84 reference px and stay inside the safe area at 2400×1080, 1920×1080, 2340×1080 and 1600×720, with a simulated cutout, and in left-handed mode (`Playtest/Touch_Target_Audit.txt`).

## Phase 9: performance (Editor, Mac M4 Pro; not Android)
- Per-frame cost of the new layers is 0.05 ms in total: camera rig 0.015 ms, control layer 0.001 ms, HUD 0.036 ms (max 0.19 ms).
- Touch-to-controller latency is 2 frames.
- Editor frame rate is about 27.5 fps against the game's 30 fps cap.
- GC is described above.
- On the phone, a third-party FPS overlay that was already on the device showed 26 fps; I didn't measure that myself. Memory was 365 MB PSS (32 MB graphics), with no Unity errors in logcat.

## Phase 10: tests
| Suite | Result |
|---|---|
| First Mission playtest (graphical Editor; virtual keyboard, virtual touchscreen and widget handlers; now with focus-independent input) | **155 PASS / 0 FAIL / 0 UNTESTED** (`Playtest/FirstMission_Playtest_Results.txt`) |
| Visual Upgrade regression (unchanged scene and shared code) | **50 PASS / 0 FAIL** (`Regression/VisualUpgrade_controls/`) |
| Backend | not touched in this batch (last run 232/232) |

Bugs found by the tests and fixed:
- Settings buttons kept a stale settings object after a reset.
- Two header buttons were below 84 px.
- View changes jumped up to 2.5 m in one frame on slow frames; they now glide with a speed cap.
- The cockpit view rendered black; it was replaced by the mast cam.

## Screenshots (`Playtest/Screenshots/`, real Game View 2400×1080)
- 20 mast cam
- 21 science target framing
- 22 shoulder view
- 23 first-person inspect
- 24 airlock camera
- 25 settings applied (raised joystick, lower opacity, larger buttons)
- 26 settings panel
- 01–13 as before

Phone: `Device/Controls/01_launch.png`.

## Android
- A new dev APK was built in the isolated copy (IL2CPP ARM64, landscape): `~/Projects/MissionRedHorizon_Step1Work/FirstMissionAndroid/Build/AstroGateway_FirstMission_dev.apk` (0.84 GB, mostly development symbols).
- It was installed on the moto g stylus 5G and launches and renders, with the new CAMERA / RECENTER buttons visible.
- **I did not inject touches:** the phone was in use, with a call active and the status bar over the top-left of the game. So the touch controls are VERIFIED in the Editor only. On the device they are UNTESTED, apart from the earlier observation that the stick moved the astronaut.
- The Android graphics fix (quality levels use the scene's URP asset) is still applied in the copy only. It still needs your approval for the main project.

## Remaining limitations
1. Jump, buffering and coyote time are BLOCKED pending approval.
2. There's no rover cockpit interior, so the mast cam is used instead.
3. A hands-on device check of every control is still needed, along with device FPS and thermals.
4. The shared code's GC churn needs a separate fix.
5. There is no input rebinding (the action asset is unused).
6. The status bar may overlay the top-left chip when Android shows system UI (seen during a call). Worth checking with immersive mode.

## Rollback
- New files are `FirstMission/Scripts/FirstMissionControlLayer.cs` and `FirstMissionCameraRig.cs`. Changed First Mission files are `FirstMissionHUD.cs`, `FirstMissionController.cs`, `RoverSensorSuite.cs`, `Editor/FirstMissionBuilder.cs`, `Editor/FirstMissionPlaytest.cs` and the rebuilt scene.
- To return to the pre-controls First Mission state: `tar -xzf ~/Projects/MissionRedHorizon_Backups/Controls_PreChange_20261020/firstmission_untracked.tgz -C ~/Projects/MissionRedHorizon` (after deleting `Assets/AstroGateway/FirstMission`), then reopen Unity.
- Removing First Mission entirely: delete `Assets/AstroGateway/FirstMission` (+ `.meta`). Nothing else references it.
- Player data: delete PlayerPrefs `AstroGateway.FirstMission.v1.*`.
- Phone: `adb uninstall com.astrogateway.firstmission.dev`.
