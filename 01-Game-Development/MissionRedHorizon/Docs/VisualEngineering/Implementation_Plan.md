# AstroGateway visual engineering — Phase 0 and implementation plan (2026-10-19)

Scope: `Assets/AstroGateway/VisualUpgrade/Scenes/Mars_VisualUpgrade_v1.unity` on local branch `visual-upgrade`.
No push, merge, reset or deletion. AutoVRL and Sentinel untouched.

## Phase 0 — safety checks
| Check | Result |
|---|---|
| Branch | `visual-upgrade`, 2 local commits (`80b8310`, `e3c67bf`) on top of `laptop-playtest`; not pushed |
| Working tree | clean before this pass (`git status --porcelain` = 0 lines) |
| Unity running | no Unity process; stale `Temp/UnityLockfile` only |
| Backup | `~/Projects/MissionRedHorizon_Backups/visual-upgrade_20261019_1508.bundle` (394 MB, all branches), `git bundle verify` = okay |
| Disk | 263 GB free |
| Baseline | `Docs/VisualUpgrade/Playtest_Final`: 47 PASS / 0 FAIL / 0 UNTESTED |

Inventory, UI audit and defects are in `Object_Inventory.md`, `UI_Component_Inventory.md` and `Visual_Defect_Register.md` (IDs below refer to it).

## Prioritized plan
**Batch 1 (UI foundations/HUD, low risk) — applied in this pass.** One scene-only script, `VisualUpgradeHUDStyle.cs`; no shared script, asset, scene, renderer or setting changed: UI-01, 02, 03, 04, 06 (UI-05 attempted and reverted after the HUD audit flagged OPEN REPORT at 9 px).

**Batch 2 (HUD, low risk) — next, no approval needed:** UI-05, 07, 09, 10, 11, 12, 13, 14; tokens moved into `VisualUpgradeHUDTheme.asset`; multi-resolution HUD audit (1512×982, 1920×1080, 2560×1440, 1280×800, 4:3) in the Visual Upgrade harness (UI-17); Profiler numbers in the Mac player (PF-01).

**Batch 3 (environment/material, medium risk, Visual Upgrade folder only):** 3D-01 dish artifact, 3D-04 habitat panels, 3D-05/06 terrain and massifs, 3D-07 station scan feedback. Each with before/after captures, 47-check regression and FPS.

**Needs your approval (higher risk)**
- UI-08 settings card redesign for desktop: rebuilds controls created by the shared HUD controller.
- UI-16 minimap fix: touches the shared HUD controller / map texture.
- Final astronaut / rover art, scene re-layout, renderer or URP changes, separate HUD PlayerPrefs key: scene, asset, renderer or architecture changes.

## Design-team asset specification (blocked items 3D-02, 3D-03)
**EVA astronaut:** FBX, humanoid rig (Unity Humanoid-compatible, 1.80 m with suit, Y-up, metres); 25–40k tris LOD0, LOD1 ~12k, LOD2 ~4k; PBR metal/rough 2K sets for suit, helmet/visor (gold coated, separate transparent material), backpack, gloves, boots; white / grey / restrained orange; clips: idle, walk (~1.4 m/s), run (~3 m/s), seated-drive pose, scan (hold 2–3 s). Must fit the existing CharacterController (radius 0.32, height 1.85, from the scene) — controller stays.
**Rover:** FBX, 6 separate wheel pivots on the existing axle positions (±1.10 x, 0 / ±1.05 z, radius 0.36), cab seat socket for the seated driver, headlight/tail-light/beacon empties, 40–60k tris LOD0 + 2 LODs, 2K PBR sets (body panel, graphite, aluminium, tyre, glass). Root aligned to the current kinematic controller (CharacterController radius 1.15, height 2.3); colliders stay as they are.

## Batch 1 results (2026-10-19)
Changed file: `Assets/AstroGateway/VisualUpgrade/Scripts/VisualUpgradeHUDStyle.cs` only (scene-only presentation script; shared HUD controller, scenes, assets, URP and ProjectSettings untouched — `runInBackground` restored after each Editor run).

| Fix | Evidence |
|---|---|
| UI-01 objectives fit (best-fit 13–19) | `UIPass2/BeforeAfter/BeforeAfter_06_gameplay_hud.png` |
| UI-02 NOT CONNECTED tracks dimmed to 18% | same |
| UI-03 prompt pill shows only actionable text, hides when idle | 06 (hidden on foot), 07 ("Stop the rover to exit") |
| UI-04 pill, speed gauge, telemetry share one .36–.64 column | 06, 07 |
| UI-06 pause controls 17 px max, more room, 1.25 line spacing | `BeforeAfter_09_pause_menu.png` |
| UI-05 footer | attempted, HUD audit flagged OPEN REPORT at 9 px, reverted; moved to batch 2 |

Verification (graphical Unity 6000.6.4f1 Editor, Apple M4 Pro, Metal):
- Playtest run 2 (final code): **47 PASS / 0 FAIL / 0 UNTESTED**, 0 console errors, 0 warnings (`UIPass2/Playtest/`). Run 1 (with the reverted footer change) was also 47/47 but its HUD audit had 1 ISSUE (`UIPass2/Playtest_Run1/`).
- HUD audit run 2: 0 ISSUE, 0 OVERLAP at the Editor Game View size (1796×1582; 1512×982 audit still open, UI-17).
- Captures: `UIPass2/Captures_After/` 01–10, same harness and quality 2 as `Captures/After_Final`.
- Editor FPS (same harness): frame caps held on all presets (p50 33.3 ms Low/Medium, 16.7 ms High/Ultra). Editor Play Mode numbers are noisy; no performance change is claimed for a UI-only batch. Mac player not re-measured for this batch.

Rollback: `git revert <batch-1 commit>` (or `git checkout e3c67bf -- Assets/AstroGateway/VisualUpgrade/Scripts/VisualUpgradeHUDStyle.cs`). Full backup: `~/Projects/MissionRedHorizon_Backups/visual-upgrade_20261019_1508.bundle`.
