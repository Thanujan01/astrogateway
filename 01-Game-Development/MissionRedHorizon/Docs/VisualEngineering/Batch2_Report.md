# HUD batch 2 — report (2026-10-20)

Approved by SSL on 2026-10-19 (low-risk HUD/UI only). Scene: `Mars_VisualUpgrade_v1`, branch `visual-upgrade`, on top of `43843eb`. Not pushed, merged or deployed.

## What changed
| Item | Change |
|---|---|
| A. Desktop cleanup | Joystick size, joystick side and fixed/floating settings hidden when touch controls are hidden; remaining rows close up. Shown exactly as before on phones, touchscreens or with `showTouchControls`. Rover telemetry no longer repeats the speed; the speed gauge is the single speed readout. |
| B. Side buttons | II / SET / MAP / BAG → PAUSE / SETTINGS / MAP / INVENTORY on the same rounded pill as other HUD buttons, 11–14 px, aligned column. Same buttons, same handlers. |
| C. Modal dimming | Gameplay HUD fades to 30% behind pause, settings and report cards (CanvasGroup alpha only, no blur). Input stays blocked by the existing modal shield and paused simulation (checked at all three sizes). |
| D. Recent Scan card | Taller card in the empty space under the minimap; three rows: scan text (best-fit 12–16), OPEN REPORT button at its own height, local-only footer on one line. No 9 px regression (audit: 0 text under 11 px). |
| Found by the new audit | "LOCAL TRAINING • PROVISIONAL" line was cut off at 1920×1080 and 2560×1440 (branding now best-fit); recent-scan text was cut at 1512×982 (fixed by D). |
| Found in captures | Prompt pill still said "[K] SCAN" during a scan; now hidden while scanning (the scan card shows progress). |
| Test tooling | New `VisualUpgradeResponsiveAudit` (Editor only): 3 resolutions × HUD states, screenshots, text/button/panel geometry, input-blocking and touch-config checks. Both harnesses now disable the Mac's physical keyboard/mouse during a run (a run on 2026-10-19 failed 4 playtest checks because someone used the Mac while it ran). |

## Exact files changed
- `Assets/AstroGateway/VisualUpgrade/Scripts/VisualUpgradeHUDStyle.cs` (scene-only presentation script)
- `Assets/AstroGateway/VisualUpgrade/Editor/VisualUpgradeResponsiveAudit.cs` (new, Editor only)
- `Assets/AstroGateway/VisualUpgrade/Editor/VisualUpgradePlaytest.cs` (Editor only: physical input disabled during runs)
- Docs: this report, `Docs/VisualUpgrade/Batch2/` evidence.
No scene, asset, shared script, gameplay, camera, URP or ProjectSettings change (`runInBackground` restored after each run).

## Responsive HUD audit (`Docs/VisualUpgrade/Batch2/{Before,After}/Responsive_HUD_Audit.txt`)
| | Before (43843eb) | After |
|---|---|---|
| Layout issues (clipped/truncated text, < 11 px, off-screen buttons, overlapping panels) | 20 (branding truncated at 1920/2560 in every state; recent-scan text truncated at 1512) | **0** at 1512×982, 1920×1080, 2560×1440 |
| Touch-only settings hidden on desktop | FAIL ×3 | PASS ×3 |
| Touch controls + touch settings return in touch config | PASS | PASS |
| Input blocked under pause card / resume | PASS | PASS ×3 |
States per size: on foot, near rover, driving, scanning, after scan, pause, settings (+ touch HUD and touch settings at 1512).
Note: the "before" scanning shot (04) was taken after the scan had finished (a tool timing bug fixed before the "after" run), so compare the scanning UI with 05.

## Gameplay regression
`Docs/VisualUpgrade/Batch2/Playtest/`: **47 PASS / 0 FAIL / 0 UNTESTED**, 0 console errors, 0 warnings, physical input disabled (Keyboard, Mouse, Pen).

## Mac standalone performance (Apple M4 Pro, Metal, 1512×982 window, dev build; `MissionRedHorizon_Step1Work/VisualUpgradeMacBuild/Output_Batch2_{Before,After}/standalone_probe.txt`)
| Run | Before batch 2: fps / p50 / p95 ms | After batch 2 | CPU ms (after) | GPU ms (after) |
|---|---|---|---|---|
| Medium (cap 30) | 29.0 / 34.1 / 35.3 | 29.0 / 34.1 / 35.3 | 34.5 | 4.8 |
| High (cap 60) | 56.5 / 17.0 / 18.6 | 56.6 / 17.1 / 18.6 | 18.3 | 6.2 |
| Ultra (cap 60) | 57.4 / 17.0 / 18.6 | 57.4 / 17.0 / 18.6 | 17.4 | 6.4 |
| High, HUD hidden → full (UI A/B) | 57.7 → 57.4 fps | 57.3 → 57.6 fps | — | — |
| Ultra cinematic | 51.5 / 17.6 / 26.6 | 57.3 / 17.1 / 18.6 | 17.4 | 8.1 |
UI overhead (after): HUD adds ~9 SetPass calls (43 → 52) and ~0.15 ms canvas update (0.026 → 0.175 ms). GC ≈ 4.8 KB/frame, memory 181 MB allocated, 153–174 MB graphics — same before/after. Draw-call/batch counters return 0 in this player (not available); the pause-card row is excluded because the Mac stalled ~15 min during that measurement in both runs (p50 17.1 ms either way).
**Conclusion: no measurable performance change from batch 2.** Both runs are ~2–3 fps below the 2026-10-18 v1 reference (59.4/59.6 at High/Ultra) on the *same* code before and after this batch, so that gap is not caused by batch 2 (the earlier probe ran unfocused; machine state differed). No improvement is claimed.

## Remaining visual defects (HUD)
UI-08 settings card still touch-sized (needs the shared controller), UI-10 mode label backing, UI-12 compact empty Recent Scan card, UI-13 repeated resource value, UI-16 square minimap in round mask, CONTROL TUNE developer window shown to players.

## Rollback
`git revert <batch-2 commit>` or `git checkout 43843eb -- Assets/AstroGateway/VisualUpgrade/` then remove `VisualUpgradeResponsiveAudit.cs(.meta)`. Full backup: `~/Projects/MissionRedHorizon_Backups/visual-upgrade_20261019_1508.bundle` (verified). Unity's crash-recovery copy of the scene (identical to the scene file) was moved out of `Assets/_Recovery` to `~/Projects/MissionRedHorizon_Backups/Unity_Recovery_20261019/`.

## Batch 3
SSL asked on 2026-10-19 to improve 3D objects and animation: plan in `Batch3_3D_Animation_Plan.md` (dish stripe artifact and dead rover body-sway are confirmed bugs with known causes).
