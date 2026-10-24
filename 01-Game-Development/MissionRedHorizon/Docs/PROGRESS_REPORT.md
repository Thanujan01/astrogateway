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

## Stage 2 (approved 2026-10-16: next-stage Steps 1 and 2 only) — DONE, awaiting owner review
Safety first: git repository created in the project (`main`, baseline commit before changes; `.gitignore` excludes Library/Temp/Logs/UserSettings). Verified tar backup + SHA-256 list: `MissionRedHorizon_Step1Work/backup/Stage2_PreChange_20261016_2225/`.

### Step 1 — regression of older scenes after the MarsPrototype exit fix: PASS, no regressions
Earlier sessions' own suites, run on a lab copy, outputs redirected to `MissionRedHorizon_Step1Work/results/Regression` (never into the old evidence folders). Copies in `Docs/Regression/`.
| Suite (scene) | Now | Baseline (before fix) |
|---|---|---|
| CinematicHUD functional (production scene) | 42/42 | 42/42 |
| Responsive layout (4 Game View sizes) | 44/44 | 44/44 |
| Native control slice | 26/26 | 26/26 |
| Stage08B Mars prototype | 20/20 | 20/20 |
| Mission01 Earth launch v4 | 13/13 | 13/13 |
| Visual Polish v2 | 34/34 | 34/34 |
| Step 1 gameplay suite (production scene, re-run on new code) | 77/77 | 77/77 |
Harness note: the Stage08B suite hangs if its Evidence folder is missing (create it first). The VisualPolish harness re-saves Mars_VisualPolish_v2.unity, and running these suites leaves the lab copy unusable for timing (dome contact dropped to ~0.1 fps there; a clean copy runs at 30 fps). Never run the old harnesses on the main project; use a fresh lab copy for later tests.

### Step 2 — Intelligent mission integration: PASS 26/26 (`Docs/Step2/Mission_Integration_Results.txt`)
- New isolated scene `Assets/AstroGateway/MissionIntegration/Scenes/Mars_MissionIntegration_v1.unity` (copy of Production Visual v1; original byte-identical).
- Wired: account (offline), mission session + semantic command queue, skill evidence, weather feed, oxygen model adapter, recommendation; small "MISSION LINK • PROVISIONAL" panel (I key toggles, hides with HUD).
- Honesty: backend shown NOT CONNECTED, account "not signed in", score "not scored • rubric pending", oxygen model "unapproved", recommendation "offline default". No network code, no sync claim, no survival or scoring rules added.
- Default oxygen model reproduces legacy drain exactly (0.2803% vs 0.2803%); a test model plugs in without code changes.
- Command body matches the existing FastAPI snake_case style: `{"command_id","expected_revision","kind","payload"}`.
- Game View captures (1920×1080): `Docs/Step2/Screenshots/01..05`.
- Re-run: build `AstroGateway.MissionIntegration.MissionIntegrationBuilder.Build` (refuses overwrite), test `...Validate` with env `STEP2_OUT`, on a lab copy.

### Files changed in Stage 2 (see `git log` / `git diff` once committed)
Changed: `Integration/Contracts/IntegrationContracts.cs`, `Integration/Local/LocalServices.cs`, `Integration/Local/GameplayEventBridge.cs`, `Verification/Editor/Step1GameplayVerification.cs`, `Docs/Integration_Architecture.md`, `Docs/PROGRESS_REPORT.md`, `Docs/Step1/Step1_Gameplay_Results.txt`.
Added: `Integration/Local/LifeSupportModelAdapter.cs`, `MissionIntegration/` (Scripts, Editor, Scenes), `Docs/Backend_Mission_Endpoints_Plan.md`, `Docs/Step2/`, `Docs/Regression/`, `.gitignore`.
Not touched: original scenes, controllers, rover, HUD, backend, saved data (verified by hashes).

### Remaining limitations
- Nothing is sent to a backend; queue is memory-only (lost on quit).
- Weather "vis" is the rendered fog distance; wind/temperature not connected.
- Oxygen at 0% still has no consequence; scoring/recommendation rules are placeholders by design (Step 4).
- Mission id mismatch between backend (`mission-red-horizon-poc`) and Unity registries (`mission.red_horizon.*`).
- Human checks still open: control feel, Android touch/performance, HUD readability; placeholder art awaits the design team.

## Step 3 — Local backend mission endpoints (approved 2026-10-17) — DONE locally, awaiting review
Full report: `Docs/Step3/Step3_Report.md`.
- 5 new endpoints in the existing FastAPI app (`AstroGatewayAndroid/backend`): commands, state, report, skills, recommendation. Same Supabase-verified auth + revocation + per-user RPC design.
- No scoring, oxygen, weather or recommendation rules: reports/skills return evidence counts with `score`/`levels` null and `pending_approval`; recommendation is a labelled placeholder.
- Tests: backend 146/146 (68 existing + 78 new incl. authorization); Unity HTTP client vs local server 18/18; Unity project compiles.
- Candidate SQL 005 + rollback in `Docs/Step3/SQL/` — NOT executed (no PostgreSQL on this Mac).
- Unity client (`Integration/Http/`) exists but is not wired into any scene. Nothing deployed.
- Backend is not under git; its pre-change backup is in `MissionRedHorizon_Step1Work/backup/Step3_Backend_PreChange_*`. Changed: `main.py`, `database.py`, `README.md`; added two test files.

## Step 4 — Mission rules design and validation (approved 2026-10-17) — DONE, isolated, awaiting review
Design document: `Docs/Step4/Mission_Rules_Design.md` (formulas, config values, weights, level criteria, sample scenarios, acceptance tests, approvals needed).
- Mission id proposal: canonical `ag.mars.red-horizon.surface-ops` (version 1); read-only alias map for `mission-red-horizon-poc` and the 5 Unity registry ids. Nothing rewritten.
- One rules config (`mission_rules_v1.json`, PROPOSED) shared by backend `AstroGatewayAndroid/backend/rules/` and Unity `Assets/AstroGateway/MissionRules/Config/`.
- Backend engines (server authority): oxygen, weather scenarios, transparent scoring from server-timestamped commands, skill levels from repeated evidence, rules-based recommendations behind a replaceable interface. Not imported by any endpoint.
- Unity models: config-driven `ConfiguredLifeSupportModel` (implements ILifeSupportModel) and `ConfiguredWeatherModel`, `MissionCatalog`. Not used by any scene or prefab (verified by test).
- Tests: backend 182/182 (incl. 36 rule tests); Unity rule test 12/12 incl. 15/15 oxygen and 18/18 weather vectors identical to the backend.
- Penalties and weather effects are disabled in config. No migration, deployment or production data change.

## Laptop acceptance playtest (2026-10-17) — DONE, PARTIAL, awaiting owner hands-on check
Report: `Docs/LaptopPlaytest/Laptop_Acceptance_Report.md`.
- Real graphical Editor (6000.6.4f1) on the MacBook Pro M4 Pro; Mars_ProductionVisual_v1 in Play Mode, driven through virtual Input System keyboard/mouse by `Verification/Editor/LaptopPlaytest.cs` (menu AstroGateway → Laptop Playtest). 42 PASS / 1 FAIL. 11 real Game View screenshots at 1512×982.
- Claude could not see the screen (macOS screen-capture permission denied), so control feel/smoothness still need a human.
- FAIL: camera ends inside Crew Dome airlock when the astronaut backs onto it (0.35 m min clamp in ProductionCameraRig). Fix proposed, not applied.
- HUD: saved PlayerPrefs profile hides the HUD (F1); weather "Session" line and "local only" footer clipped; white sky line in overview; touch controls shown on Mac.
- Perf (Editor): Medium/Low ~29.5 fps (game cap 30), High/Ultra ~57 fps (cap 60); GPU 1–5 ms.
- Standalone dev .app built from an isolated copy (productName MissionRedHorizon_LaptopBuild) in `MissionRedHorizon_Step1Work/MacBuild/`; launches, but URP post-processing shaders are stripped in the player. Gameplay in standalone untested.
- No scene/settings/save changes (SHA-256 verified). Git branch `laptop-playtest`.

## Visual & UI upgrade v1 (2026-10-18) — DONE, isolated, awaiting owner review
Report: `Docs/VisualUpgrade/Visual_Upgrade_Report.md`. Scene: `Assets/AstroGateway/VisualUpgrade/Scenes/Mars_VisualUpgrade_v1.unity` (built from Production Visual v1 by `VisualUpgradeBuilder.Build`; original untouched).
- Backup: `MissionRedHorizon_Step1Work/backup/VisualUpgrade_PreChange_20261018_1005/` (verified). Branch `visual-upgrade`, files untracked, not pushed.
- Phase 2 fixes: airlock camera (0 inside frames), 3 clipped HUD texts, MENU label, rover exit hint, touch controls hidden on desktop, standalone post-processing (scene profile keeps DoF/Panini shaders; no project setting changed). White sky line = Editor Game View gizmo, not in the game.
- Temporary procedural art: Mars ground shader, dunes/craters, pebbles, sky, ACES/SSAO/bloom/DoF, astronaut suit + walk/run motion, rover lights/cockpit, station instruments, habitat lights, comms tower, desktop HUD restyle.
- Playtest 47/0; Mac player 29.7 / 59.7 / 59.3 fps on Medium/High/Ultra at 1512x982.
- Harness note: Editor runs flip `runInBackground` in ProjectSettings; restore with `git checkout -- ProjectSettings/ProjectSettings.asset`.

## Proposed next (needs owner approval — STOP here)
- Science review of every proposed default; owner sign-off on canonical id, oxygen consequences, weather effects, `station_id` in scan commands, team scoring rule.
- Then, separately approved: wire engines into `/v1/reports` + `/v1/skills`, DB alias migration (local rehearsal first, needs PostgreSQL), and enabling rules in the MissionIntegration scene copy only.

## Session log
- 2026-10-18 (Claude, visual upgrade v1): isolated Mars_VisualUpgrade_v1 with Phase 2 fixes and temporary procedural visuals/HUD; playtest 47/0; Mac player verified; nothing pushed.
- 2026-10-17 (Claude, laptop acceptance): real Editor Play Mode playtest 42/43, screenshots, Editor FPS, isolated dev macOS build; no project content changed.
- 2026-10-16 (Claude, status review): read docs, stage folders, outputs/ProductionVisual reports and logs. No project files changed except creating this report.
- 2026-10-16 (Claude, Step 1): gameplay verification 77/77 pass on lab copy; fixed rover-exit-onto-roof bug (MarsPrototype.cs, one line); added Integration + Verification folders and Docs/Integration_Architecture.md, Docs/Step1/. Previous next-steps list superseded by the plan above.
- 2026-10-17 (Claude, Stage 2): git repo + verified backup; 179/179 older-scene checks and 77/77 gameplay checks pass; isolated Mars_MissionIntegration_v1 scene with integration seams, 26/26 checks and 5 Game View screenshots; backend endpoint plan written. Waiting for approval before Step 3/4.
- 2026-10-17 (Claude, Step 3): 5 local backend endpoints + candidate SQL + Unity HTTP client; backend 146/146, client 18/18; nothing deployed. Waiting for approval before Step 4.
- 2026-10-17 (Claude, Step 4): canonical mission id + alias map; config-driven oxygen/weather/scoring/skills/recommendation engines (backend) and oxygen/weather models (Unity), all isolated; backend 182/182, Unity rules 12/12. Stopped before enabling anything.
