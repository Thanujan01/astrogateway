# AstroGateway Explorer upgrade — gameplay UX, navigation, rover and science (2026-10-21)

New scene **`Assets/AstroGateway/FirstMission/Scenes/Mars_FirstMission_Explorer.unity`**, built with `FirstMissionBuilder.BuildExplorer`. The v1 and HUD v2 scenes are unchanged; they remain rollback points and still pass their tests. Nothing was pushed, merged, deployed or migrated. The Stage 1 audit is in `Explorer_Stage1_Audit.md`.

## What the player can do now
| Success criterion | Status | Evidence |
|---|---|---|
| Explore without large obstructive panels | **VERIFIED** | Top bar, mission card, status card and nav card removed. Thin compass, objective chip, map icon, ⋯ menu and pause remain. Test: no HUD element in the central 40 % of the screen on foot. Shot 40 |
| Tap a small map icon → fully interactive map | **VERIFIED** | Fullscreen circular map. Gameplay input is blocked while it's open (the rover does not move). Shot 43/45 |
| Pan, zoom, rotate | **VERIFIED** | One-finger pan, pinch in/out (3.0 → 5.7 → 1.2), double-tap zoom, mouse wheel, reset, centre on astronaut/rover, north-up / heading-up (the map rotates to the rover heading), live compass needle, scale bar (5–100 m) |
| Find the rover and infrastructure | **VERIFIED** | Registry built from real scene objects: rover, 5 research stations, Crew / Laboratory / Life Support domes, 3 airlocks, solar farm, power distribution cabinet, communications mast, life-support tanks, storage crates. Layers: terrain, base, mission, rover, science, collected, unvisited, hazards, routes |
| Select a destination and follow a meaningful route | **VERIFIED** | A 2 m traversability grid built at load from the real colliders (125×125 cells, domes/rocks/steep ground blocked) + A* + line-of-sight smoothing. Test: the route stays on walkable cells (88 m route vs 70 m straight line around obstacles). Status: available / calculating / blocked / unreachable / unavailable / arrived. Re-planning at most once a second, only when off-route > 4 m or the target moved > 3 m |
| Navigate to the rover and board it | **VERIFIED** | NAVIGATE TO ROVER targets the live rover transform; a waypoint marker over the rover (edge arrow when off-screen), live distance, route chip and compass pin appear. It guides only and never moves the player. Test: the player walked the route (36.7 m, 1 re-plan) → arrival → BOARD ROVER (existing 5 m rule) → rover controls, mode pill, rover camera. Shot 41 |
| Drive the six-wheel rover with clear controls | **VERIFIED** | Glass pedals, D/R pill, joystick, CAMERA / VIEW / RECENTER, measured-speed gauge |
| Change driving modes | **PARTIAL** | PRECISION (45 % throttle = 1.7 m/s, finer steering) and NORMAL (3.8 m/s) work. **TRAVERSE is BLOCKED**: it needs the shared controller's hard-coded 3.8 m/s maximum raised, which needs your approval. Tapping it says so |
| See actual speed | **VERIFIED** | The gauge uses the controller speed; the drawer also shows speed measured from rover displacement (test: within 0.4 m/s) |
| Scan targets → short popup → reopen full records | **VERIFIED** | The rover observation popup shows readings, SIMULATED/UNAVAILABLE labels, LOCAL ONLY, sync state and time. After about 4.5 s it collapses into a record icon with a count badge, which opens the detailed record (metadata, units, provenance, storage, sync, AI NOT CONNECTED, related objective). An on-foot scan shows an honest popup: "no measurement values are modelled". Shots 47/48 |
| Diagnostics in a compact menu | **VERIFIED** | The ⋯ drawer has Mission, Astronaut, Rover, Sensors, Power, Comms, Data queue, Science AI, Navigation, Controls (settings) and Graphics. Every value is tagged LIVE / SIMULATED / LOCAL ONLY / NOT CONNECTED / UNAVAILABLE. The rover battery is never confused with the phone battery. Shot 46 |

## Not built (needs your decision)
- **TRAVERSE speed:** raising 3.8 m/s changes the shared kinematic controller.
- **Jump / crouch / precision movement:** new movement physics.
- **Repair gameplay, generator, charging station, tents, dock, checkpoints:** none exist in the world. The map lists them only as "DESIGN PROPOSALS (not in the world)", with no position and no navigation.
- **Six-wheel physics:** the rover is a kinematic CharacterController. The ARES-VI wheels roll and the rocker/bogie tilts to the ground visually; there are no wheel colliders, traction, torque or rollover model. This is documented in the drawer's Rover section.

## Architecture (new files in `Assets/AstroGateway/FirstMission/Scripts`)
- `Nav/FirstMissionNavigation.cs`:
  - `WorldLocationRegistry`: discovery by exact scene object names; proposals are kept separate.
  - `MapCoordinateConverter`: scene x,z ±125 m ↔ map 0..1; +Z is scene north, and nothing is labelled as Mars GPS.
  - `TraversabilityGrid`: built over frames.
  - `RoutePlanner` (A*).
  - `NavigationTargetManager`: target, re-planning, progress, arrival.
- `HUD/ExplorerHUD.cs`: compact HUD, compass, map + `MapGestures`, waypoint presenter, NAVIGATE TO ROVER, mode and profile pills, systems drawer, scan popups and record view.
- Changed:
  - `FirstMissionControlLayer.cs`: `uiBlocking`, drive-profile throttle/steer caps.
  - `HUD/PremiumHUD.cs`: protected helpers, `ShowClassicPanels`, no per-frame active-state churn.
  - `HUD/HudWidgets.cs`: capture-only minimap mode, 1024 px capture.
  - `HUD/HudIcons.cs`: 13 new icons.
  - `Editor/FirstMissionBuilder.cs`: `BuildExplorer`.
  - `Editor/FirstMissionPlaytest.cs`: about 50 new Explorer checks.

## Motion and CGI pass (2026-10-21, Explorer scene only)
The request was "more motion and cji"; I read "cji" as CGI (cinematic visual effects). Everything lives in `HUD/ExplorerCinematics.cs`. All of it is light (one 60-particle system, a few LineRenderers, one unlit cylinder), and the Reduced motion setting turns off the camera motion, intro and shimmer.
- Intro fly-in: a 4.5 s camera sweep down to the astronaut, with the HUD fading in. Any tap or key skips it. Shot 50.
- Waypoint beacon: a soft orange light pillar with a pulsing ground ring over the navigation target. Shot 41.
- Scan FX: a cyan beam from the mast (or arm drill for the sampler) to the station, plus an expanding ground ring and a shimmer when the scan completes. Shot 51.
- Ambient dust motes drift around the camera.
- Driving camera: a small FOV kick (up to +5°) with speed and a subtle terrain shake (≤5 cm).
- UI motion: the map, systems drawer, scan popup and record card slide, fade and scale in (0.28 s).
- Tests: Explorer playtest **245 / 0** (8 new cinematics checks). APK rebuilt; it was not installed because the phone was not connected.

## Tests
| Suite | Result |
|---|---|
| Explorer scene: full playtest (driving, keyboard, touch, science, data, cameras, airlock, settings, HUD v2 checks + 50 Explorer checks for map, routing, navigate-to-rover, boarding, profiles, measured speed, drawer, popups, records, touch targets at 2400×1080 / 1920×1080 / 1512×982) | **237 PASS / 0 FAIL / 0 UNTESTED** |
| First Mission v1 (unchanged) | **155 / 0** |
| Visual Upgrade regression | Run 1: 49/1 ("W walks" timing flake, 0.74 m/s). Run 2: **50/0** |
| Unity compile | clean (batch build exit 0) |

## Android
- APK: `~/Projects/MissionRedHorizon_Step1Work/FirstMissionAndroid/Build/Explorer/AstroGateway_FirstMission_Explorer_dev.apk` (development build, IL2CPP ARM64, landscape).
- It is installed on the moto g stylus 5G and launches; screenshot `Device/Explorer/01_launch.png` shows the compact HUD with NAVIGATE TO ROVER. There are no Unity errors in logcat.
- I did not send taps to the phone, so on-device map gestures, routing, boarding and FPS are **NOT TESTED** on hardware.

## Known limitations
1. The map image is a one-time top-down render of the scene (moving objects appear only as markers). The hazard layer is the route grid: red = blocked, amber = steep.
2. Route following is guidance only; there is no auto-navigate.
3. Marker labels can overlap when zoomed out.
4. Toasts can appear behind the map overlay.
5. "Live route progress" can read 0 % while you drive away from the target.
6. The traversability grid uses a 0.85 m clearance capsule (astronaut-sized), so a few gaps routable on foot may be too tight for the rover.

## Rollback
- Explorer only: delete `Scenes/Mars_FirstMission_Explorer.unity`, `Scripts/Nav/`, `Scripts/HUD/ExplorerHUD.cs`.
- Everything back to before this upgrade: delete `Assets/AstroGateway/FirstMission` and extract `~/Projects/MissionRedHorizon_Backups/Explorer_PreChange_20261021/firstmission_untracked.tgz` into the project.
- Phone: reinstall the HUD v2 APK, or run `adb uninstall com.astrogateway.firstmission.dev`.
