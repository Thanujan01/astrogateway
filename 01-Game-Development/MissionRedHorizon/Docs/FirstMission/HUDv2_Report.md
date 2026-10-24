# Premium Android HUD (v2) — report (2026-10-21)

Requested by SSL on 2026-10-20. **The reference image never arrived** (not on the message, not in the project files). So the layout follows the zones written in the request (top bar, left, right, bottom-left, bottom-centre, bottom-right) and the colours given. A pixel-level match to the image is BLOCKED until you send it.

HUD v2 lives in a **separate scene**, `Assets/AstroGateway/FirstMission/Scenes/Mars_FirstMission_HUDv2.unity`. The First Mission v1 scene and its HUD are unchanged; its playtest still passes 155/0. Nothing was pushed, merged or deployed, and there was no backend change or migration.

## Phase 0 audit
- Repository and branch: `~/Projects/MissionRedHorizon`, `visual-upgrade` @ `5e0c223`. Tracked files are clean and nothing was running.
- Backup: `~/Projects/MissionRedHorizon_Backups/HUD_PreChange_20261021/` (repo bundle verified okay, First Mission tarball, SHA256SUMS).
- No new git branch: all First Mission work is untracked files, so switching branches would neither isolate nor protect anything. This also avoids disturbing the read-only GitHub thread.

### Status sources
| Category | Source used | Shown as |
|---|---|---|
| Power | `NativeSliceGame.battery` (game model, drains while driving) | % + **SIMULATED** (not the phone battery) |
| Vehicle temperature | none exists | **NOT AVAILABLE** (N/A) |
| Communication link | sync uploader (no hosted backend) | **NOT CONNECTED** |
| Data storage | local sensor store count / 400 | **LOCAL** |
| Speed | `MarsPrototype.speed` | km/h, **GAME STATE** |
| Steering | `testMove.x` + measured yaw rate | LEFT / CENTRE / RIGHT, **GAME STATE** |
| Driving mode | control layer state | **GAME STATE** |
| Sensor availability | sensor catalog (5 of 6 instruments available) | **SIMULATED** |
| Mission progress | First Mission objectives (local, unscored) | **LOCAL** |
| Backend sync | queue states → LOCAL ONLY / QUEUED / SYNCING / SYNCED / FAILED | **NOT CONNECTED** |

Navigation uses Unity transforms (scene metres, labelled "scene frame, not GPS"). Terrain slope, roughness and traversability come from the validated `RoverSensorSuite`. Mission id `ag.mars.red-horizon.surface-ops` v1 resolves through the existing alias map, and no historical id was changed.

## What was built (`Assets/AstroGateway/FirstMission/Scripts/HUD/`)
| Component | File | Status |
|---|---|---|
| Design tokens (your palette, spacing, type sizes, motion times) | `HudStatus.cs` › `HudTokens` | VERIFIED |
| Status model: reading has label/value/unit/icon/source/level/timestamp/accessible text; `IStatusSource`; 10-category `RoverStatusProvider` | `HudStatus.cs` | VERIFIED (provenance checks) |
| Reusable status widget: smooth value and colour transitions, N/A or NOT CONNECTED for missing data, bar, source tag | `HudWidgets.cs` › `HudStatusWidget` | VERIFIED |
| Procedural icon set (24 icons) + rounded glass panel sprites; no external art, works on Android | `HudIcons.cs` | VERIFIED |
| Navigation: compass strip, waypoint pin, minimap **rendered once from the real scene** (top-down camera), live rover/astronaut/station/waypoint markers, waypoint distance/bearing, terrain summary, scene position | `HudWidgets.cs` › `HudMinimap`, `PremiumHUD.cs` | VERIFIED |
| HUD layout: top bar (branding, mode, 5 tabs, session time, link, rover power, pause); left mission card + rover status (4 widgets, ALL expands to 10); right navigation; bottom-left joystick + CAMERA + VIEW; bottom-centre speedometer arc, D/R/N state, steering bar; bottom-right ACCEL, BRAKE, REVERSE, EXIT, RECENTER, SCIENCE AT STATION | `PremiumHUD.cs` | VERIFIED |
| SCIENCE mode: instruments (unavailable ones labelled and refused), target selection, range bar, SCAN with progress, CANCEL, result summary and collection status | `PremiumHUD.cs` | VERIFIED |
| DATA mode: records table (time, sensor, station, type, value, unit, source, queue, backend, AI), the 5 sync states, AI always NOT CONNECTED, paging, RETRY | `PremiumHUD.cs` | VERIFIED |
| MISSION mode: objectives with DONE / ACTIVE / PENDING / INFO states, completion pulse, active highlight, waypoint, completion state, sync indicator, prompt | `PremiumHUD.cs` | VERIFIED |
| SYSTEMS: all 10 status widgets + settings grid; pause card also shows the settings | `PremiumHUD.cs` | VERIFIED |
| Smart visibility: drive / science / data / paused each show only what they need; collapsible mission and nav panels; transient critical alerts (low simulated power, storage full, sync failed, terrain hazard) | `PremiumHUD.cs` | VERIFIED |
| Vehicle lights / horn | not implemented in the game | not shown (by your rule) |
| Matching the reference image exactly | — | **BLOCKED** (image missing) |

The architecture keeps data and presentation apart. `RoverStatusProvider` reads game and data state, widgets only present `StatusReading`, and input still goes through the v1 HUD's input feeding and the control layer. The UI never calls the backend transport. HUD v2 inherits from the v1 HUD class. The only v1 change was making its members `protected`, which leaves behaviour the same; the v1 test confirms 155/0.

## Tests
| Suite | Result |
|---|---|
| HUD v2 scene, full playtest (driving, boarding/exit, keyboard, touch, science, data, cameras, airlock, settings + 32 HUD v2 checks) | **187 PASS / 0 FAIL / 0 UNTESTED** (`PlaytestHUDv2/FirstMission_Playtest_Results.txt`) |
| First Mission v1 scene (unchanged HUD) | **155 / 0 / 0** |
| Visual Upgrade regression | **50 / 0** |
| Unity console errors during runs | 0 |

HUD v2 checks cover:
- honest sources (power SIMULATED and equal to the game battery; temperature N/A; link NOT CONNECTED; storage = real count; speed = controller)
- all 10 categories and accessible text
- visibility per mode
- DATA rows matching the newest records, and the sync label mapping
- the minimap rendered from the scene
- waypoint distance = transform distance
- collapse/expand, alert show and auto-hide, settings in pause
- no overlap between panels and drive controls
- touch targets ≥ 84 reference px inside the safe area at 2400×1080, 1512×982, 1920×1080, 2560×1440 and 1600×720, with a simulated cutout and left-handed layout

Fixed during testing:
- Small hit areas: tabs, pause and collapse buttons.
- A status panel overlapped the raised joystick.
- Waypoint text overlapped the map.
- The speed arc was rotated wrong.
- Panels touched the top bar.
- A Unity "fake null" bug (`GetComponent() ??`) in both HUDs was latent in v1 and has been fixed.

## Screenshots
**Before (v1):** `Playtest/Screenshots/02_drive_tab_moving.png`, `05_science_tab.png`, `06_data_tab_offline.png`, `07_mission_tab.png`

**After (v2), in `PlaytestHUDv2/Screenshots/`:**
- 38 drive (2400×1080)
- 37 drive at 1512×982, 1920×1080 and 2560×1440
- 30 science
- 31 data
- 32 mission
- 33 systems
- 34 collapsed/expanded
- 35 alert
- 36 paused + settings

**Phone:** `Device/HUDv2/01_launch.png`

## Android
- APK: `~/Projects/MissionRedHorizon_Step1Work/FirstMissionAndroid/Build/HUDv2/AstroGateway_FirstMission_HUDv2_dev.apk` (development build, IL2CPP ARM64, landscape, 0.86 GB mostly debug symbols).
- Installed on the moto g stylus 5G, **replacing the earlier dev build (same package id)**.
- It launches and renders HUD v2. The safe-area offset for the camera cutout and the real-scene minimap are visible. There are no Unity errors in logcat, and memory is 371 MB PSS.
- **I didn't send taps to the phone**, so touch on the device is UNTESTED. Device FPS is UNTESTED.
- The Android graphics fix is still in the build copy only (needs your approval for the main project).

## Performance (Editor, Mac; not Android)
The HUD v2 widgets poll at 4–5 Hz and animate per frame without allocating. The minimap renders once, not every frame. Per-frame cost and GC measurements are in the playtest file. The game's existing GC churn (~300 KB/frame in Editor) is in shared code and is not caused by the HUD.

## Known limitations
1. Reference-image match is BLOCKED: please send the image.
2. The "ALL" button sits over the power widget's source tag (cosmetic).
3. The minimap is a one-time snapshot, so moving objects are markers only.
4. Icons are simple procedural glyphs. The typeface is Unity's built-in (LegacyRuntime), because Avenir is macOS-only.
5. No lights or horn exist in the game, so neither is shown.
6. Device touch/FPS checks, the main-project Android graphics fix and jump approval are still open.

## Rollback
- HUD v2 only: delete `Assets/AstroGateway/FirstMission/Scripts/HUD/` and `Scenes/Mars_FirstMission_HUDv2.unity` (+ `.meta`). Also revert `FirstMissionHUD.cs` / `FirstMissionBuilder.cs` / `FirstMissionPlaytest.cs` / `FirstMissionAndroidBuild.cs` from `~/Projects/MissionRedHorizon_Backups/HUD_PreChange_20261021/firstmission_untracked.tgz`.
- Whole First Mission state before HUD v2: delete `Assets/AstroGateway/FirstMission`, then extract that tarball into the project.
- Phone: `adb uninstall com.astrogateway.firstmission.dev`, or reinstall the v1 APK from `.../FirstMissionAndroid/Build/AstroGateway_FirstMission_dev.apk`.

## Update 2026-10-21: high-end control visuals (asked by SSL: "upgrade it to the high level higher end UI")
New `Scripts/HUD/HudFx.cs` adds a premium visual kit. Sprites are generated once, with no blur pass and no per-frame allocation, and reduced motion is respected:
- Panels: soft drop shadows, gradient glass with a top rim highlight, thin cyan edge, aerospace corner brackets.
- Joystick: dark glass well, 32-tick bezel, four direction chevrons, a bezel that lights up while held, a direction glow that follows the stick, a lit thumb with a halo.
- Driving controls:
  - ACCEL is a tall pedal whose fill shows the live progressive throttle.
  - BRAKE is a pedal that lights while held.
  - REVERSE is a D | R switch.
  - Every button now has a glass finish, a press glow and a scale-in animation.
- Speedometer: 270° tick dial, glowing arc (orange in reverse), large digital readout, D/R/N state, steering bar.
- Tabs: a sliding glass pill behind the selected tab; unselected tabs are dimmed.
- Header toggles: 94 px transparent hit areas with the glyph kept in the header.

Results:
- Gameplay behaviour is unchanged: playtest still **187/0** on the HUD v2 scene.
- New APK built and launched on the moto g stylus 5G: `Device/HUDv2/02_premium_controls.png`. Touch on the device is still UNTESTED.
- Editor capture: `PlaytestHUDv2/Screenshots/02_drive_tab_moving.png`.

## Update 2026-10-21 (2): bottom HUD to the "telemetry glass" spec
Every item from SSL's six-point spec, mapped from CSS to Unity uGUI:
1. **Theme:**
   - Control fill is exactly rgba(10,20,30,.55).
   - Borders are 1-px hairline sprites tinted rgba(0,210,255,.3).
   - New thin-stroke icons (cam2, eye2, recenter2, exit2, scan2, target2) are drawn 1:1, with preserveAspect so they never stretch.
   - **backdrop-filter blur(8px) is not applied.** A live blur needs a full-screen grab pass every frame, which is too expensive on mobile GPUs. The 55 % dark fill provides the separation instead.
2. **STEER joystick:** 25 % smaller (360 → 270 ref px). It has a 1-px outer ring at .35 alpha, subtle crosshair ticks, and a matte dark-cyan thumb (no neon blur). The ring brightens slightly while held.
3. **CAMERA and VIEW:** 44 dp circular icon-only buttons (116 ref px on a 1080-px-tall phone) with no text.
4. **Speedometer:** no box, fill or brackets. It floats as a semicircle arc gauge with tick marks, a glowing cyan fill (amber in reverse), crisp `13.7` + `km/h`, a slim steering line, and a pill badge for the gear (`D  DRIVE`, `R  REVERSE`, `N  NEUTRAL`).
5. **ACCEL and BRAKE:** vertical glass pedals with 7 horizontal grip lines and a slim level bar (live throttle on ACCEL). The outline and soft glow appear only while pressed: cyan for ACCEL, #FF8C00 for BRAKE. The solid cyan background is gone.
6. **D | R:** a segmented pill; the active gear gets a subtle cyan (D) or amber (R) background. RECENTER is a 44 dp circular button with a recenter/compass icon. EXIT ROVER and SCIENCE AT STATION are matching glass pills. The astronaut RUN and INTERACT controls use the same theme.

Results:
- HUD v2 playtest still **187/0**, including all touch-target (≥ 84 ref px, inside the safe area) and overlap checks.
- Editor capture: `PlaytestHUDv2/Screenshots/02_drive_tab_moving.png`.
- APK built: `.../FirstMissionAndroid/Build/HUDv2/AstroGateway_FirstMission_HUDv2_dev.apk`. **Not installed**, because the phone was not connected (`adb devices` empty).
