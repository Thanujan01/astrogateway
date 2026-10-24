# Mission Red Horizon — Progress Report

Purpose: a single resume point so work can continue after a usage limit or a new session.
Update this file at the end of every work step. Never delete earlier entries; add new ones on top of the log.

## How to resume
1. Read "Current state" and "Next steps" below.
2. Detailed evidence for each stage lives outside the project in
   `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/<Stage>/`
   (latest: `ProductionVisual/Final_Report.md`, `Known_Limitations.md`).
3. Project is NOT under git. Preservation is checked by SHA-256 manifests
   (`outputs/ProductionVisual/Preservation_Results.json`). Make a manifest/backup before any change.
4. Editor: Unity 6000.6.4f1, URP. Open `Assets/AstroGateway/ProductionVisual/Scenes/Mars_ProductionVisual_v1.unity` for the newest slice.

## Ground rules (from project owner)
- Never delete, overwrite or restructure existing scenes, controls, rover systems, UI, backend or saved data.
- New work goes in new, isolated folders/scenes.
- Report missing context instead of guessing; wait for go-ahead before major changes.

## Current state (as of 2026-10-16)

### Stage history (oldest → newest, by folder in Assets/AstroGateway)
| Stage | Folder / scene | Date |
|---|---|---|
| Design blueprint (docs 01–07) | Docs/Design | 10-08 |
| Greybox / Environment v01 (Stage 03–04) | Greybox, Environment, Scenes/MarsOutpost_Environment_v01 | 10-08 |
| Playable prototype (Stage 08B), Android probes (08C/08C1) | Stage08B, Stage08C, Stage08C1 | 10-09–10-10 |
| Mission 01 Earth launch milestone v4 | Mission01Milestone | 10-12 |
| Native control vertical slice | NativeControlSlice | 10-13 |
| Realistic research base v1 | MarsEnvironment | 10-14/15 |
| Visual polish v2 | MarsVisualPolish | 10-15 |
| Cinematic HUD v1 | CinematicHUD | 10-16 00:13 |
| **Production visual v1 (last session)** | ProductionVisual/Scenes/Mars_ProductionVisual_v1 | 10-16 15:46 |

Stages without project folders (outputs only): Stage05A/B, 07A/B, 08A, 08D/D1/D11/D2, UnifiedAuth, Stage09A, PreMeetingDemo, AndroidPOC/Validation, Architecture.

### Last session (ProductionVisual) — verdict PARTIAL
DONE
- New isolated scene with original geodesic dome panels/frame, chamfered airlocks/enclosures, rover frame, 3-LOD wheels, derived smoothness textures.
- Existing player/rover physics retained; wheel roll, compact HUD, camera damping/obstruction, scanner/overview/chase modes.
- Automated: 43/43 production checks PASS, 179/179 regression PASS. Main-project import/reload audit PASS.
- Preservation: 692 baseline + 420 historical originals unchanged; 138 new files only in the new folder.

PARTIAL
- Habitats/rover surfaces visibly unfinished (procedural).
- HUD still grey/translucent, not reference-quality.
- Hands-on control coverage incomplete; Low-preset benchmark had a stall.

BLOCKED (needs owner input)
- "Reference A" target image is missing.
- No licensed, rigged EVA astronaut + animations; primitive astronaut remains.

UNTESTED
- Physical Android (ADB showed no device), IL2CPP, thermals, GPU timings.
- Manual traversal/camera/collision play-through.

Loose ends noticed on 2026-10-16
- `ProductionVisual/Documentation/` and `ProductionVisual/Prefabs/` are empty (no in-project README, unlike CinematicHUD/MarsEnvironment).
- Build Settings still list only `Assets/Scenes/SampleScene.unity`.
- Editor was opened again at 19:09 (Main_Editor_Preview_Unity.log); no compile errors/exceptions in that log. Unity is not running now.

## Step 1 — gameplay verification (2026-10-16, approved by owner) — DONE
Owner direction: the design team is producing the final astronaut, rover, environment and cinematic UI. Do not replace them with temporary models; procedural parts are placeholders. Reference A and production assets will come later. No visual redesign or cloud deployment without separate approval.

Result: 77/77 automated gameplay checks pass on Mars_ProductionVisual_v1 (after one fix). Details: `Docs/Step1/Step1_Gameplay_Results.txt`.
- How it was run: batch Unity on a lab copy at `/Users/kapilash/Projects/MissionRedHorizon_Step1Work/Lab` (productName changed to `MissionRedHorizon_Step1Lab` so saves/prefs stay separate). Test: `Assets/AstroGateway/Verification/Editor/Step1GameplayVerification.cs`, entry `AstroGateway.Verification.Step1GameplayVerification.Run`, output folder from env `STEP1_OUT`.
  Re-run: `STEP1_OUT=<folder> <Unity 6000.6.4f1> -batchmode -projectPath <lab> -executeMethod AstroGateway.Verification.Step1GameplayVerification.Run -logFile <log>` (~12 min). Do not run it on the main project: it enters Play Mode and writes a local save.
- Covered: walking/running/heading, stop, dome collision, world boundary, board range, seat, drive/steer/brake/reverse, wheel roll, rover vs dome, exit rules (moving, blocked, beside obstacles/dome), 5 scans + range/cancel/lock, chase/near/cinematic/wall camera, pause/resume, tuning modal, save/load, real keyboard path (W/E/Space/K/C/Esc), 20 s terrain drive; per-frame: camera never inside geometry, no actor overlap, nothing below ground, zero logged errors.
- BUG FIXED: exiting the rover beside a dome/rock placed the astronaut on its roof (5.2 m up beside Laboratory Dome). Fix: one guard line in `Assets/AstroGateway/Stage08B/MarsPrototype.cs` Interact() rejects exit points more than 1.2 m above/below the rover. Original saved at `MissionRedHorizon_Step1Work/backup/MarsPrototype.cs.original`. This file is shared with older scenes (Stage08B, Native, CinematicHUD, VisualPolish); their old suites were NOT re-run because they write into the previous session's evidence folder.
- Test-side issues only (not game bugs): virtual keyboard needs `editorInputBehaviorInPlayMode=AllDeviceInputAlwaysGoesToGameView` in batch mode.
- Preservation: SHA-256 of Assets/Packages/ProjectSettings before/after (`MissionRedHorizon_Step1Work/backup/`): 0 removed, 1 changed (MarsPrototype.cs), 13 added (Integration + Verification folders and .meta). Saved data folder and PlayerPrefs plist byte-identical to backup. Main project compiles (batch import, exit 0).

Observations (not changed, for the owner):
- Keyboard E only acts within 5 m of the rover; the on-screen INTERACT button also inspects targets/airlocks.
- Oxygen/suit power drain to 0 with no consequence; rover health never changes; weather is 3 manual states (keys 1/2/3).
- Scoring, skills and recommendations do not exist in the game yet; scans are synthetic.

Needs human verification (cannot be proven by automated tests): control feel and camera smoothness by hand, touch controls and IL2CPP on a physical Android device, frame rate/thermals on target devices, HUD readability, audio, visual look of the placeholder assets.

## Integration seams for backend work (Step 1 item 4) — DONE, not wired into scenes
See `Docs/Integration_Architecture.md`. New folder `Assets/AstroGateway/Integration/`: contracts for identity, mission session + semantic commands, assessment report, weather, life support, recommendations; offline defaults reproduce current behaviour; `GameplayEventBridge` observes gameplay read-only. Verified by 5 checks in the Step 1 suite. No network code, no scene changes.

## Next stage plan (await owner go-ahead)
A. Owner/human checks: short manual play session (list above); confirm the exit fix feels right.
B. Regression of older scenes against the MarsPrototype fix, using copies of their harnesses that write to the Step1Work folder (not the old evidence folder).
C. Wire the bridge into a NEW copy of the production scene (Mars_ProductionVisual_v1 untouched) and route HUD oxygen/weather/score readouts through the contracts.
D. Backend (needs approval + hosted environment): mission-session/commands/complete/report endpoints in the existing FastAPI service (`/Users/kapilash/Projects/AstroGatewayAndroid/backend`), local DB rehearsal first; Unity HTTP client with offline queue replay; token hand-off from the web/Android shell.
E. Gameplay systems needing science/design decisions: oxygen consequence and refill rules, weather timeline, scoring rubric weights, recommendation rules.
F. When the design team delivers: swap astronaut/rover/environment/UI via prefab references only, keep physics/controllers.
G. Physical Android test once a device is connected.

## Session log
- 2026-10-16 (Claude, status review): read docs, stage folders, outputs/ProductionVisual reports and logs. No project files changed except creating this report.
- 2026-10-16 (Claude, Step 1): gameplay verification 77/77 pass on lab copy; fixed rover-exit-onto-roof bug (MarsPrototype.cs, one line); added Integration + Verification folders and Docs/Integration_Architecture.md, Docs/Step1/. Previous next-steps list superseded by the plan above.
