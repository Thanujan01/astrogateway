# AstroGateway — Visual & UI Upgrade v1 (2026-10-18)

**Upgraded isolated scene:** `Assets/AstroGateway/VisualUpgrade/Scenes/Mars_VisualUpgrade_v1.unity`
**Original scene (unchanged):** `Assets/AstroGateway/ProductionVisual/Scenes/Mars_ProductionVisual_v1.unity`
**Branch:** local `visual-upgrade` (from `laptop-playtest`), committed locally, not pushed.

## How this was verified (read first)
- Real graphical Unity 6000.6.4f1 Editor on the MacBook Pro (Apple M4 Pro, Metal); scripted Play Mode through virtual Input System devices (same path as a keyboard/mouse).
- Screenshots are Unity's own captures (`ScreenCapture.CaptureScreenshot`) of the Game View, or of the macOS player. Nothing is AI-generated or painted over. Before/after images are the two real captures placed side by side.
- Editor captures came out at the Game View's own size (1796×1582) even when 1512×982 was requested; the Mac player captures and FPS are at a real 1512×982 window, so treat those as the laptop reference.
- Control feel and smoothness by hand were **not** judged by a person.

## Phase 1 — preserve
- Verified backup before any change: `MissionRedHorizon_Step1Work/backup/VisualUpgrade_PreChange_20261018_1005/` (tar of Assets/Packages/ProjectSettings/Docs, SHA-256 manifest of 937 files re-verified after a test restore, `git bundle` of all branches, PlayerPrefs plist, save folder).
- The original scene, shared scripts, shared materials and ProjectSettings are byte-identical: `git status` shows only new files. Unity flips `runInBackground` in ProjectSettings during Editor runs; it was restored after every run.

## Phase 2 — confirmed defects
| Issue | Result | How |
|---|---|---|
| Camera inside Crew Dome airlock | **Fixed** — playtest 47/47, 0 camera-inside frames of 4,889 (original scene: FAIL, 21 frames) | `VisualUpgradeCameraRig`: never clamps inside walls; rises over the shoulder (up to 3 m) when the wall is close; immediate pull-in, smoothed release |
| 3 clipped HUD texts | **Fixed** | weather card rewritten to 3 short lines; "local only" footer and INVENTORY fit |
| MENU label over modal headers | **Fixed** | hidden while any modal/pause/tuning card is open |
| White line in the sky | **Not a game bug** — Editor gizmo | appears with Game View *Gizmos* on, gone with them off (`Playtest_Run1/Screenshots/10a…` vs `10…`); not in the Mac build. Turn off **Gizmos** in the Game View toolbar |
| "Stop before exiting" at 0 m/s | **Fixed** | shows only above 0.2 m/s (the real exit rule); "[E] EXIT ROVER" when stopped |
| Touch controls on desktop | **Fixed** | hidden on Mac (CanvasGroup), still shown on phones/touchscreens |
| Standalone post-processing stripped | **Fixed without any project setting** | URP strips DoF/Panini shaders unless a VolumeProfile uses them, then skips *all* post. The scene's own profile stores them disabled. Mac player log: 0 strip errors (was 4) |

## Phases 3–7 — visual systems (all procedural, clearly labelled TEMPORARY art; no downloads)
- **Mars ground:** new URP shader (`MarsTerrain.shader`) on the existing terrain mesh (collider untouched): two-scale regolith texture, rock exposure by noise/slope, dune-scale colour variation, SSAO-ready.
- **Far terrain:** 1.2 km dunes + 5 craters outside the playable square (visual only); 760 pebbles combined into 36 chunks, no colliders.
- **Sky & light:** new sky shader (gradient, haze band, sun disk, cool halo); retuned sun presets; ACES tonemapping, grade, white balance, bloom, vignette, split toning; half-res SSAO; realtime reflection probe; drifting dust haze; Gaussian DoF on far background in the cinematic overview (High/Ultra).
- **Astronaut:** primitive renderers hidden (objects kept); suit with helmet, gold visor, lamps, life-support backpack, bearings, boots, provisional emblem; procedural idle/walk/run blend from measured speed. While driving the walking suit is hidden (it is taller than the cab) and a compact seated driver is shown in the cockpit instead.
- **Rover:** tinted cockpit glass with seats/dashboard/screens, headlights (2 spot lights, on while crewed), tail/brake lights, beacon, shocks, fenders, antenna, softer wheel dust.
- **Base:** 5 research-station instruments (tripod, sensor, panel, dish, status light; colliders and ResearchTarget unchanged), habitat window glow, foundation and airlock lights, airlock floodlights, comms dish/feed/guy wires/beacon, solar panel material.
- **HUD (desktop):** dark translucent panels, cyan/orange accents, Avenir Next (macOS system font, not embedded), tighter layout; new read-only readouts: rover speed / suit pace gauge, scanner status, mission progress bar, interaction prompt pill, controls reference in the pause card, LOW/MEDIUM/HIGH/ULTRA buttons in settings. Every value is local simulation and labelled so.
- **Cameras:** same modes as before (no new modes); cinematic enter/exit now eases.

## Gameplay regression (`Playtest_Final/`)
**47 PASS, 0 FAIL, 0 UNTESTED**, 0 console errors/warnings: walk/run/A/S/D, grounding, camera follow, dome collision, world boundary, board range, boarding, drive/steer/brake/reverse, wheel roll, rover vs dome, exit while moving refused, exit beside Laboratory Dome (roof bug), 5 scans + range/cancel rules, cinematic, pause/resume, modal, F1 profiles, weather keys, airlock camera ×4. HUD audit: no clipped, overlapping or undersized text.

## Performance (measured, not estimated)
Mac standalone player, 1512×982 window, Apple M4 Pro (`Output_Final/standalone_probe.txt`):
| Preset | FPS | p50 ms | p95 ms | max ms | Cap |
|---|---|---|---|---|---|
| Medium | 29.9 | 33.3 | 34.2 | 36.1 | 30 |
| High | 59.4 | 16.7 | 17.4 | 26.4 | 60 |
| Ultra | 59.6 | 16.7 | 17.3 | 22.2 | 60 |
| Ultra, cinematic + DoF | 59.6 | 16.7 | 17.3 | 22.1 | 60 |

Editor Play Mode, same harness on both scenes (`FPS_Before_ProductionVisual/`, `FPS_After_VisualUpgrade/`): both hold the game's caps (p50 33.3 ms on Low/Medium, 16.6 ms on High/Ultra); both show ~10 Editor hitches >50 ms per run (Editor overhead, absent in the player). Single-frame GPU samples rose from ~2–6 ms to ~5–20 ms (Medium/Ultra), still inside the frame budget. **No performance improvement is claimed.**
Not measured: draw-call counts, texture memory, thermals over a long session. The probe reported `focused=False` (window not key) although the player was rendering at full rate. In the Editor the Game View could not be kept focused reliably while the scripted runs were in progress (one run reported focused=True; repeat runs did not), so the Mac player numbers are the reference.

## Screenshots (real)
`Captures/After_Final/` 01 landscape, 02 astronaut, 03 rover, 04 habitat, 05 science station, 06 gameplay HUD, 07 rover HUD, 08 cinematic, 09 pause, 10 settings · `Captures/BeforeAfter/` side-by-side for 01–08 · Mac player: `MissionRedHorizon_Step1Work/VisualUpgradeMacBuild/Output_Final/standalone_*.png`

## Remaining issues
1. Astronaut and rover are procedural placeholders; the design team's rigged astronaut and final rover were not found in the project.
2. The seated driver is a simplified stand-in; a proper seated pose needs the final rigged astronaut and rover cab.
3. HUD settings (quality, opacity, profile) still share one PlayerPrefs key with the older scenes, as before.
4. Minimap still shows a square map inside the round mask (pre-existing).
5. Temperature/wind and Sol clock remain "not connected / not implemented" (honest labels kept).
6. Nothing pushed or deployed; no backend touched.

## Mac standalone verification
Built from an isolated copy (`MissionRedHorizon_Step1Work/VisualUpgradeMacBuild/Project`, productName `AstroGateway_VisualUpgradeBuild`, separate saves) → `Output_Final/AstroGateway_VisualUpgrade.app` (215 MB, dev build). Launches on Metal at 1512×982, post-processing runs, 0 exceptions. It is ad-hoc signed: right-click → Open the first time.

## Rebuild / re-run
- Rebuild scene from the original (overwrites only the Visual Upgrade scene/assets): `Unity -batchmode -quit -projectPath <project> -executeMethod AstroGateway.VisualUpgrade.EditorTools.VisualUpgradeBuilder.Build`
- Playtest (graphical Editor): menu **AstroGateway → Visual Upgrade Playtest (v1)** or `-executeMethod AstroGateway.VisualUpgrade.EditorTools.VisualUpgradePlaytest.Run` (env `VU_SCENE` targets another scene).
- Captures: `-executeMethod AstroGateway.VisualUpgrade.EditorTools.VisualUpgradeCapture.Run` (env `VU_OUT`, `VU_SCENE`, `VU_QUALITY`).

## Rollback
Nothing existing was modified, so rollback is removal:
1. Close Unity.
2. `cd /Users/kapilash/Projects/MissionRedHorizon && git checkout laptop-playtest` — this removes every Visual Upgrade file from the working tree (they exist only on the `visual-upgrade` branch). Run `git status` to confirm a clean tree.
3. Keep the branch for later, or delete it with `git branch -D visual-upgrade`.
4. Full restore if ever needed: `MissionRedHorizon_Step1Work/backup/VisualUpgrade_PreChange_20261018_1005/` (`tar -xzf project_assets_settings_docs.tgz`, verify with `shasum -a 256 -c manifest.sha256`; `repo.bundle` restores git history).
