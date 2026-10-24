# ARES-VI upgrade: showroom copy (20 Oct 2026)

**Result:** the upgraded ARES-VI now renders correctly in Unity 6 URP and has a real articulated hierarchy. All seven requested animations exist and play through the Animator. It uses 56% fewer draw calls than the original import. Everything below was measured in Unity in this session: Editor Play Mode on Apple M4 Pro / Metal, with real Game View captures.

It is still a showroom asset and is **not in the playable Mars scene**. That step needs your approval.

## What changed
| Area | Original import | Upgraded |
|---|---|---|
| Faces / normals | 45% of triangles inside-out, so holes in the hull, tyres, canopy and dish | **0 of 23,036** triangles oppose their normals; 126 inside-out pieces flipped, open sheets made two-sided |
| Hierarchy / pivots | 211 flat parts, every pivot at the rover origin | 32 transforms: Chassis → Rocker/Bogie → 6 wheels; Arm Yaw → Upper → Fore → Wrist → Drill; Mast Pan → Tilt; 4 hover modules → vane ring + plume. Wheels spin in place (0.0 mm centre drift) |
| Animations | none | 7 clips + Animator controller (see below) |
| Geometry clashes | mast column passed through the canopy glass; arm shoulder sat inside it | mast moved to the rear-right deck, arm moved forward onto a new hull bracket; 0 overlaps with the canopy bounds |
| Suspension | straight bars at axle height (could not articulate) | new rocker-bogie linkage with joints, knuckles, steering actuators, non-rotating brake calipers |
| Tyres | lugs floating outside the tyre | new lathed tyres with rounded shoulders and 44 chevron lugs fused to the tread |
| Materials | 8 flat colours, no textures | 11 URP Lit materials; 24 procedural 1024 px maps (albedo, normal, metal/smoothness, AO): panel lines and rivets, Mars dust, brushed titanium, crinkled gold foil, dusty rubber |
| Extra detail | – | LED light bars, differential bar, plasma plumes, Mars regolith ground and sky preset, post-processing (ACES, bloom, vignette) in the showroom only |
| Renderers / draw calls | 211 renderers, 729 draw calls | 30 renderers (82 submeshes), **321 draw calls** |

## Animation results (sampled + live Animator)
| Clip | Measured | Result |
|---|---|---|
| Wheel rotation (2 s loop) | 90.0° at 0.5 s; centre drift 0.0 mm; lowest point 0.000 m | PASS |
| Rocker-bogie (4 s loop) | rocker ±6°, bogie ±9°, body lifts up to 0.121 m to keep wheels on the floor | PASS, **with a 7 mm dip below the floor between keys** |
| Science arm deploy (4.8 s) | stowed across the front → swings forward → unfolds → drill spins; drill never below 0.10 m; 0 forearm/wheel overlaps | PASS |
| ChemCam scan (6 s loop) | pan ±55°, tilt −12/+10°, two laser pulses | PASS |
| Hover activation (3 s) | body rises 0.38 m, wheels tuck, lowest point 0.274 m, plumes and coils ramp up | PASS |
| Hover stabilization (4 s loop) | bob 0.35–0.41 m, pitch ±1.2°, nacelle counter-tilt, vane ring turns | PASS |
| Landing (3.5 s) | descends to 0.000 m, small 12 mm bounce, plumes fade; never below the floor | PASS |

All 7 states also played in the live Animator (state active, transforms moving). These are hand-keyed clips made for the showroom. They are not driven by physics or rover controls.

## Performance (same camera, quality "PC", Game View 3024×1584)
| Case | FPS | Draw calls | SetPass | Tris drawn | GPU p50 |
|---|---|---|---|---|---|
| Upgraded, static | 512.8 | 321 | 42 | 84,890 | 2.71 ms |
| Upgraded, animating | 469.4 | 321 | 54 | 84,890 | 1.90 ms |
| Original import | 463.4 | 729 | 38 | 55,522 | 1.48 ms |
| Empty stage | 525.5 | 33 | 33 | 2,186 | 2.61 ms |
| Mars Visual Upgrade scene (baseline, 30 FPS cap) | 29.3 | 799 | 63 | 1,353,076 | 5.89 ms |

Rover memory: 48.0 MB textures (24 × 1024², compressed) and 6.6 MB meshes. Editor GPU timings are noisy: the empty stage measured higher than the rover. Treat them as "a few ms". No player build was measured.

## Still open
- The Mars sky preset reads slightly green near the horizon (procedural skybox plus ACES); it is a showroom preset only.
- The rocker-bogie clip dips 7 mm below the floor between keys.
- No LODs. The source LOD1/LOD2 were unusable, and at 23k triangles none are strictly needed.
- The hard-surface shapes are still the source's simple boxes and cylinders; the detail comes from textures, the new tyres and the new suspension. Matching a hero vehicle would need a modelling pass in Blender or similar.
- Upgraded meshes and textures are generated from the original OBJ by `AresViUpgrade.Build`. The OBJ itself was not modified.

## Files
- Prefab: `Assets/AstroGateway/VehicleShowroom/ARES_VI/Upgraded/ARES_VI_Upgraded.prefab` (meshes, materials, textures and the `Animation/` folder sit beside it).
- Scene: `Assets/AstroGateway/VehicleShowroom/Scenes/ARES_VI_Showroom.unity`. The original import is kept inactive for before/after.
- Tools: `Editor/AresViUpgrade.cs` (Build, batch-safe) and `Editor/AresViUpgradeVerify.cs` (Run, graphical Editor, `ARES_QUIT=1`).
- Evidence: `Docs/VehicleShowroom/ARES_VI/Upgrade/` contains `Upgrade_Build_Report.txt`, `Verification_Log.txt` and 24 screenshots (B06 before, U01–U13 views, A02–A07 animation poses, M01–M03 Mars preset).

## Preservation
Only `Assets/AstroGateway/VehicleShowroom/` and `Docs/VehicleShowroom/` changed. The production rover, gameplay, controls, boarding, mission systems and Mars scenes are untouched; the Mars scene was opened in Play Mode only, never saved. The harness's `runInBackground` flip was restored with git checkout. Nothing was committed, pushed, merged or deployed. The backup from the first pass is `~/Projects/MissionRedHorizon_Backups/ARES_VI_Showroom_PreChange_20261020/`.
