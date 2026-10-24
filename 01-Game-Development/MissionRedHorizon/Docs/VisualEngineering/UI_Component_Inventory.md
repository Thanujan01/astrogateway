# UI component inventory — Mars_VisualUpgrade_v1

The HUD is **built at runtime** by `CinematicHUD/Scripts/AstroGatewayHUDController.cs` (shared, not modified) on a uGUI canvas ("AstroGateway responsive Canvas" → "Safe Area"). `ProductionPresentation` (440) re-lays it out; `VisualUpgradeHUDFixes` (600) and `VisualUpgradeHUDStyle` (620) restyle it for this scene only. No UI Toolkit, no TextMeshPro: legacy `Text` with Avenir Next loaded from macOS (not embedded). One `HUD EventSystem` (Input System UI module).

Visibility profiles (F1, `settings.visibility`): 0 Full, 1 Compact, 2 Minimal, 3 Hidden. Settings persist in PlayerPrefs (key shared with older scenes — known issue).

Status legend for this pass: VERIFIED UPGRADED · VERIFIED UNCHANGED · PARTIAL · UNTESTED.

| # | Component (path under Safe Area) | Purpose / data source | Shown | Findings (1512×982) | Status |
|---|---|---|---|---|---|
| 1 | Mission branding | Static title, base, "LOCAL TRAINING • PROVISIONAL" | Full | Tight padding, fine | VERIFIED UNCHANGED |
| 2 | Survival resources / oxygen, water, food, power, radiation | `HUDTelemetryAdapter` snapshots; water/food/radiation unavailable | Full, Compact | Empty bars for NOT CONNECTED read as 0%; value and % repeat | VERIFIED UPGRADED (bars dimmed) |
| 3 | Weather and clock | Weather name + session time; temp/wind and Sol clock honestly "not connected/implemented" | Full | OK after v1 fix | VERIFIED UNCHANGED |
| 4 | Mission objectives / Objectives live | Objective text, station count, scans, ORBIS upload state | Full | Last line clipped at right edge | VERIFIED UPGRADED (best-fit) |
| 5 | Collapse objectives (–/+) | Toggles objectives | Full | Works; label "MISSION OBJECTIVES – / +" is crowded | VERIFIED UNCHANGED |
| 6 | VU Mission progress | `objectiveScans / targets.Length` (local) | Full | OK | VERIFIED UNCHANGED |
| 7 | Production overview / chase / control tuning buttons | Camera modes, tuning window | Full | OK; tuning is a dev tool shown to players | VERIFIED UNCHANGED |
| 8 | Mode and camera label | Current mode text | Full | Floats without backing, overlaps scene | VERIFIED UNCHANGED (plan B2) |
| 9 | Action 0–3 (II, SET, MAP, BAG) | Pause, settings, expanded map, inventory | Full | Cryptic abbreviations, no tooltips | VERIFIED UNCHANGED (plan B2) |
| 10 | Circular minimap panel | Contour map, player heading, station/rover markers | Full, Compact | Square map inside round mask (pre-existing) | VERIFIED UNCHANGED (needs approval) |
| 11 | VU Scanner status | `CurrentTarget()`, `scanning`, scan fill | Full, Compact | OK | VERIFIED UNCHANGED |
| 12 | Recent scientific scan (+ Recent scan live, Truthful local sync, Scan detail "OPEN REPORT") | Last `ScientificScan`, `syncStatus` | Full | Footer wraps to 2 lines at ~11 px; big empty area before first scan | VERIFIED UNCHANGED (attempt reverted, plan B2) |
| 13 | VU Interaction prompt (pill) | Context text from HUDFixes: [K] scan / [E] board/exit / stop rover + `lastResult` | ≤ Minimal, not in modal/cinematic | Always showed idle noise + duplicated "No scientific samples recorded" | VERIFIED UPGRADED (actionable text only, hides when idle) |
| 14 | VU Rover speed gauge | `prototype.speed` | Full, Compact | Narrower and offset from the column | VERIFIED UPGRADED (aligned) |
| 15 | Rover telemetry / Rover live telemetry | Suit or rover telemetry text | Full | Speed duplicated with gauge while driving | VERIFIED UNCHANGED (plan B2) |
| 16 | Shortcut 0–3 (ROVER, SAMPLES, INVENTORY, BASE) | Open modal reports | Full | INVENTORY tight but fits | VERIFIED UNCHANGED |
| 17 | Touch controls (joystick, run, brake, scan, interact) | Touch input | Hidden on desktop (CanvasGroup) | Correct | VERIFIED UNCHANGED |
| 18 | Scan presentation progress | Scan fill during scan | While scanning | Not re-captured this pass | UNTESTED (visual) |
| 19 | Modal: Simulation paused (pause card) | Save/Load/Export local, controls reference | Esc | Controls text shrank to ~12 px in an empty card | VERIFIED UPGRADED (larger area/size) |
| 20 | Modal: HUD & GRAPHICS SETTINGS | Opacity, UI/text scale, joystick, sensitivity, map zoom, presets | SET | Touch-only settings on desktop; touch-sized sliders; GRAPHICS duplicates preset row | VERIFIED UNCHANGED (plan B2/approval) |
| 21 | Modal: Scientific sample records / Inventory / Base / Rover reports | Scrollable report text | Shortcuts | Not re-captured this pass | UNTESTED (visual) |
| 22 | Modal: Expanded map | Paused snapshot map | MAP | Not re-captured this pass | UNTESTED (visual) |
| 23 | Control tuning window | Dev tuning | CONTROL TUNE | Developer UI exposed to players | VERIFIED UNCHANGED |

Not present in this scene (so not audited): startup/loading screen, mission-complete screen, notification/toast system, audio UI feedback, tooltips, keyboard/gamepad UI navigation.
