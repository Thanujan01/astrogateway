# Mars Visual Polish Milestone — PARTIAL

## Ready

New scene saved/reloaded and installed:

`/Users/kapilash/Projects/MissionRedHorizon/Assets/AstroGateway/MarsVisualPolish/Scenes/Mars_VisualPolish_v2.unity`

- Independent material, sky, lighting/volume and registry derivatives; sparse dome structural meshes, airlock trim, original solar-cell texture, adjusted distant rock composition.
- Warm sunset and bounded existing dust/haze, four runtime graphics presets, on-foot/rover FOV presentation, translucent styling of existing live HUD.
- All 547 existing files preserved byte-for-byte, including the 420 previously reported originals. Original scenes/gameplay source untouched.
- 34 new-scene + 26 Native + 20 Mars + 13 Mission 01 controlled checks passed (93 total); final new-scene suite repeated after styling.
- Ten actual matched-position before/after Unity Game View images plus ten CameraOnly supplements. Saved under Screenshots/. No generated gameplay images.

## Partial

This remains a visually improved procedural prototype. The astronaut and rover are visibly primitive; closed habitats/airlocks are simplified. It does not meet a realistic production-character or cinematic environment quality bar. Terrain PBR roughness remains approximate. Native HUD layout is retained, including large touch preview boxes and duplicate Ultra/Desktop aliases. Existing simplified collision and partial-snapshot limitations remain.

Camera improvements are FOV-only; original smoothing, orbit and collision algorithms remain. Full manual gameplay-camera acceptance was unavailable due native UI timeout. Controls were exercised through existing scripted test hooks, not claimed as hands-on passes.

## Measured performance

Apple M4 Pro, Metal, Unity Editor batch Play Mode, 3024 × 1600 Game View, fixed spawn camera, 2s warmup + at least 5s per preset:

| Preset | Effective FPS | Mean ms | P95 ms |
|---|---:|---:|---:|
| Low | 26.05 | 38.39 | 34.65 |
| Medium | 30.00 | 33.33 | 34.98 |
| High | 59.92 | 16.69 | 17.76 |
| Desktop | 59.93 | 16.69 | 18.24 |

Capped short Editor cadence only; Low includes a large outlier (mean can exceed p95). Not CPU/GPU timings, not an optimization comparison and not Android FPS. See Editor_Frame_Cadence.txt for actual frame counts/settings. The object snapshot reports 59,940 unique mesh-asset triangles, 314 renderers, 26 materials and 7,123,168 texture-object bytes; not drawn-frame counts or complete GPU memory. No connected Android device; physical performance UNTESTED.

## Blocked / next actions

Primary reference image unavailable: provide it for a visual-match review. Production astronaut and detailed drivable rover remain future asset work. Direct native Editor UI timed out; hands-on control/camera acceptance requires operator verification. Quick Search startup indexing exception remains documented. No 30-Sol system, backend, authentication, remote deployment or database changes.

## Deliverables

- Visual_Audit_And_Comparison.md
- Asset_Provenance_Report.md
- Lighting_Camera_And_UI.md
- Graphics_Presets_And_Performance.md
- Test_Report.md and raw suite logs/results
- Known_Limitations.md
- Manual_Launch_Instructions.md
- Preservation_Check.json / New_Files.json
- Screenshots/

Launch instructions give the exact installed editor command and new scene path. Stop at this milestone; no next system was started.
