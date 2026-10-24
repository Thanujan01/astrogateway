# AstroGateway — Laptop Acceptance Report (2026-10-17)

## How this was verified (read first)
- Real graphical Unity Editor was launched on this MacBook Pro (Apple M4 Pro, 24 GB, Metal), not batch mode.
- Play Mode was entered on the real project and driven by `Assets/AstroGateway/Verification/Editor/LaptopPlaytest.cs`, which sends key presses and mouse drags through **virtual Input System devices**, so they travel the same path as a physical keyboard (HUDInputRouter, ProductionModeCoordinator, MarsPrototype). Some setup steps teleport the astronaut/rover to test spots.
- Claude could **not** see or touch the screen: macOS denied screen capture to the session ("could not create image from display"). Screenshots are Unity's own Game View captures (`ScreenCapture.CaptureScreenshot`), so they are real renders, but nobody watched the game live. Control *feel*, smoothness and responsiveness still need a human.

## Summary
| Area | Result |
|---|---|
| Unity version | 6000.6.4f1 (12bfff696524) — matches ProjectVersion.txt |
| Scene | `Assets/AstroGateway/ProductionVisual/Scenes/Mars_ProductionVisual_v1.unity` (chosen over MissionIntegration_v1, which carries the provisional mission-link panel) |
| Editor Play Mode launch | **PASS** — opens in 89–122 ms, Play Mode ready in 1.2 s, 0 console errors, 0 warnings |
| Astronaut | **PASS** (scripted) — walk 2.2 m/s, run 3.8 m/s, A/S/D, grounded, camera follows, dome collision, ±119 m boundary, never below terrain (0 / 4 823 frames) |
| Rover | **PASS** (scripted) — 9 m board refused, E boards ≤5 m, W 3.8 m/s, A/D steer ±68°, Space stops, S reverses −1.8 m/s, wheels roll, blocked by Laboratory Dome, exit refused while moving |
| Rover-exit roof bug (Step 1 fix) | **PASS** — exit beside Laboratory Dome lands 0.07 m above ground |
| Science scanning | **PASS** — all 5 stations recorded via K; K with no station → "No valid target in range"; walking >3 m away mid-scan → rejected; objective → "Local exploration complete — provisional"; Research stations 5/5 |
| Cameras | **PARTIAL** — third-person, CHASE DISTANCE (8.1 → 6.15 m), C cinematic, pause/resume PASS; **camera clipping FAIL** (below) |
| HUD at 1512×982 | **PARTIAL** — readable and laid out; 3 clipped texts + items listed below |
| Keyboard / mouse | **PASS** (scripted) — every mapped key tested; hand feel UNTESTED |
| Performance | **PARTIAL** — real Editor numbers below; standalone FPS UNTESTED |
| Standalone macOS dev build | **PARTIAL** — builds (2 m 36 s, 189 MB) and launches (Metal, 1512×982, ~350 MB RSS, no errors); gameplay/controls UNTESTED; post-processing shaders stripped |
| Screenshots | 11 real Game View captures in `Docs/LaptopPlaytest/Screenshots/` |

Scripted run result: **42 PASS, 1 FAIL, 0 UNTESTED** (`LaptopPlaytest_Results.txt`).

## Controls (verified from code, then tested)
| Input | Action | Test |
|---|---|---|
| W / A / S / D | Walk; in rover: throttle, steer, reverse | PASS |
| Left Shift (hold) | Run on foot | PASS (3.8 vs 2.2 m/s) |
| Space (hold) | Rover brake | PASS |
| E | Board / exit rover, only within 5 m of the rover; exit refused while moving or blocked | PASS |
| K | Scan nearest research station within 3 m (0.8 s, cancels if you walk away) | PASS |
| C | Toggle cinematic overview | PASS |
| Esc | Pause modal / close any modal; also frees the mouse cursor | PASS |
| F1 | Cycle HUD profile (full → … → hidden); saved to PlayerPrefs | PASS (1→2→3→0) |
| 1 / 2 / 3 | Weather preview: clear / light dust / dust storm (visual only) | PASS |
| Right mouse + drag | Orbit camera | PASS (+28.8°) |
| Mouse move without right button | Nothing (by design) | Confirmed 0° |
| Mouse click | On-screen buttons (OVERVIEW, CHASE DISTANCE, CONTROL TUNE, II/SET/MAP/BAG, ROVER/SAMPLES/INVENTORY/BASE, SCAN/INTERACT) | CHASE DISTANCE and SAMPLES tested |

Not implemented (not invented): jump, interact-with-airlock (shows "Sealed"), item pickup, robot mode.
UI vs gameplay input: while paused, scanning or in a modal, gameplay input is blocked (verified: holding W while paused did nothing).
Code note (not tested): `MarsEnvironment/EnvironmentPresentation.cs` also lets **E** record an instant scan when you are more than 5 m from the rover and next to a station, so E and K are two scan paths.

## Bugs / issues found
1. **Camera clips into Crew Dome airlock (FAIL).** With the astronaut pressed against the Crew Dome airlock and facing away, the camera stayed inside `Crew Dome sealed airlock` for 22 frames (~0.4 s). Cause (inferred from `ProductionCameraRig.cs`): the obstruction pull-in is clamped to a 0.35 m minimum, so when the wall is closer than that the camera is placed inside it. Proposed fix (needs approval, camera behaviour): allow a smaller minimum or raise/side-shift the camera when the clamp is hit. Not changed.
2. **Saved HUD profile on this Mac hides the HUD.** PlayerPrefs has `visibility: 3`, so the game launches with only the "HUD / F1" pill (see `Run1_SavedHUDHidden/`). Press F1 once to get the full HUD. Left as is (it is your saved setting).
3. **HUD text clipped at 1512×982** (see `08_full_hud.png`, `HUD_Audit.txt`):
   - Weather panel (top right): "Temperature / wind: not connected" wraps, pushing "Session mm:ss" out of the box.
   - Recent-scan footer: "Pending Export • local only" shows as "Pending Export • local".
   - INVENTORY label is ~9 px wider than its button.
4. **"MENU" mode label bleeds through modal header** (faint, top centre of `09_mission_status.png`).
5. **"Stop before exiting" hint** shows right after boarding while the rover is at 0.0 m/s (`04_boarding_rover.png`).
6. **Long thin white line across the sky** in overview/cinematic views (`01`, `07`, `10`), from the comms-truss area; looks like a stretched cable/line renderer. Needs a visual check.
7. **Touch controls on Mac**: joystick, RUN/ACCELERATE/BRAKE, SCAN/INTERACT are always shown on desktop. Not wrong, but they occupy space; for the design team.
8. **Standalone build: post-processing disabled.** Player log: `GaussianDepthOfField`, `BokehDepthOfField`, `PaniniProjection` shaders "stripped from the build … PostProcessing render passes will not execute". The built app will look different from the Editor (no bloom/grading). Fix needs a project-wide graphics/shader-stripping change → approval required.
9. Unity asset import worker 1 crashed once during the session (`Logs/AssetImportWorkerHW1.log`); the Editor recovered, no effect seen.

## Simulated / provisional data shown in the HUD (not production)
Oxygen and suit power are a local drain with no consequence at 0 %. Water, food, radiation: "NOT CONNECTED". Weather: manual 3-state preview; temperature/wind and Sol clock not connected. Scans: "synthetic index (dimensionless)", "No chemical measurement available". Rover battery/condition: simulated. ORBIS upload: unavailable. No scoring, skills or backend are active in this scene.

## Performance (real measurements, Editor Play Mode, Game View 1512×982)
Measured on Apple M4 Pro / Metal. The game itself caps Low/Medium at 30 fps and High/Ultra at 60 fps (`Application.targetFrameRate`). The Editor window was **not focused** for most samples (you were in another app), which adds Editor throttling, so treat spikes as upper bounds.

| Scenario | Mean fps | p50 ms | p95 ms | p99 ms | Frames >50 ms | GPU frame |
|---|---|---|---|---|---|---|
| Medium (saved preset), walking | 24.2 (one 1.09 s hitch) | 33.3 | 55.0 | 77.0 | 15 / 257 | 1.4 ms |
| Medium, running | 29.6 | 33.3 | 50.4 | 70.4 | 9 / 178 | 1.5 ms |
| Medium, rover driving | 29.5 | 33.3 | 49.4 | 72.3 | 14 / 296 | 1.4 ms |
| Low (cap 30) | 29.5 | 33.3 | 47.4 | 79.3 | 6 / 148 | 2.9 ms |
| High (cap 60) | 57.2 | 16.7 | 18.1 | 52.3 | 5 / 287 | 1.2 ms |
| Desktop/Ultra (cap 60) | 57.3 | 16.6 | 18.0 | 48.3 | 2 / 287 | 1.6 ms |

- GPU is not the bottleneck (1–5 ms per frame); frame rate is set by the game's own cap.
- Memory (Editor process, includes Editor): Unity-allocated 3.1–3.4 GB, graphics driver 630–716 MB, managed heap ~66 MB. Standalone app: ~350 MB resident after 40 s.
- Scene load: 89–122 ms (Editor open); Play Mode to playable 1.2 s.
- Presets were changed on the runtime pipeline copy only and restored to Medium.
- UNTESTED: standalone FPS, input latency, thermals over a long session, camera smoothness by eye.

## Files changed
- Added `Assets/AstroGateway/Verification/Editor/LaptopPlaytest.cs` (+ .meta) — Editor-only playtest; menu **AstroGateway → Laptop Playtest (Production Visual v1)**.
- Added `Docs/LaptopPlaytest/` (this report, results, HUD audit, screenshots, run 1 with your saved hidden-HUD profile).
- Updated `Docs/PROGRESS_REPORT.md`.
- No scene, prefab, gameplay script, ProjectSettings, save or PlayerPrefs change. Verified by SHA-256 against `MissionRedHorizon_Step1Work/backup/LaptopPlaytest_PreChange_20261017_2218/` (scene + ProjectSettings OK, save files OK, HUD PlayerPrefs string identical). Unity itself updated its own session counters in the plist and analytics files, as it does on every launch.
- Standalone build was made from a separate copy (`MissionRedHorizon_Step1Work/MacBuild/Project`, productName `MissionRedHorizon_LaptopBuild` so its saves stay separate). Output: `MissionRedHorizon_Step1Work/MacBuild/Output_20261017_2232/AstroGateway_Dev.app`. Build Settings of the main project unchanged.
- Committed on local git branch `laptop-playtest` (not pushed, no remote changes).

## Manual verification steps for you
1. The Unity Editor is open on `Mars_ProductionVisual_v1`. Click into the **Game** tab; the test set it to a 1512×982 render size; change it in the size dropdown if you prefer Free Aspect.
2. Press **Play** (▶ at the top centre).
3. If you only see "HUD / F1", press **F1** until the full HUD appears (one press from your saved profile).
4. Click inside the Game view so it has keyboard focus. Walk with **W A S D**, hold **Shift** to run, hold the **right mouse button and drag** to turn the camera.
5. Walk to the rover (within 5 m) and press **E**. Drive with **W/S**, steer **A/D**, brake with **Space**; stop, then **E** to exit. Try it beside the Laboratory Dome.
6. Walk to a research station (green dots on the minimap; within 3 m) and press **K**. Then press **K** away from stations to see it rejected.
7. Press **C** for the cinematic view and **C** again to return; click **CHASE DISTANCE** while driving.
8. Press **Esc** to pause, **Esc** again to resume.
9. Judge by eye: control feel, camera smoothness, HUD readability, and whether the white sky line (issue 6) is visible.
10. Standalone: open `MissionRedHorizon_Step1Work/MacBuild/Output_20261017_2232/AstroGateway_Dev.app`. It is ad-hoc signed, so macOS Gatekeeper rejects it on first open: right-click → **Open** → **Open**. Repeat steps 3–8 there; Esc does not quit (use ⌘Q).

To re-run the scripted playtest: Unity menu **AstroGateway → Laptop Playtest (Production Visual v1)** (about 2 minutes; writes to `Docs/LaptopPlaytest/`, overwriting the latest results).
