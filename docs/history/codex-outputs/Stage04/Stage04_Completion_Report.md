# Stage 04 — Environment Art Pass 01

AstroGateway: Mission Red Horizon · SK2LB-AstroLab · 8 October 2026

The separate environment scene was created, reopened and geometrically validated in Unity 6000.6.4f1 / URP 17.6.0. Its release Web build succeeded and actually rendered in the Codex browser on Apple M4 Pro. This is an original procedural exterior art pass, with further visual refinement explicitly identified below. No character or gameplay implementation was added.

## Scene and preservation

Exact scene path:

`/Users/kapilash/Projects/MissionRedHorizon/Assets/AstroGateway/Scenes/MarsOutpost_Environment_v01.unity`

The scene began as an AssetDatabase copy of `Assets/AstroGateway/Greybox/MarsOutpost_Greybox.unity`. New meshes/materials replaced the copy's proxies; shared greybox assets were not edited. The 250 × 250 m terrain bounds, eight zone centers and all approved route waypoints remain intact. The route measures **421.62 m** against the actual ground mesh at 857 samples; maximum sampled slope **4.87°**. At the explicit test speed **2.5 m/s**, traversal is **168.6 s (2 min 49 s)**, excluding interaction time.

SHA-256 comparisons found **zero changes among 63 baseline files** covering the greybox assets, seven approved design documents, package manifests/lockfile and project settings. Evidence: `Preservation_Check.json`. Existing SampleScene and unrelated systems were not targeted.

| Zone | Center X, Y, Z (m) | Reserved footprint |
|---|---|---|
| Command Habitat | −65, 0, −60 | 36 × 28 m |
| Greenhouse | −65, 0, −10 | 30 × 22 m |
| Rover Bay | −15, 0, −95 | 32 × 20 m |
| Power Station | 25, 0, −45 | 24 × 24 m |
| Solar Array | 75, 0, −55 | 48 × 40 m |
| Science Staging | −5, 1, 20 | 24 × 20 m |
| Layered Delta | 0, 4, 85 | 70 × 40 m |
| Rock Comparison | 75, 1.05, 35 | 30 × 26 m |

As in Stage 03, northern bedding extends behind the reserved delta staging footprint. It remains within the 250 m world bounds; the zone center was not moved. Footprint outlines and diagnostic colored route lines were removed from the art scene's presentation. Waypoint transforms remain for validation.

## Hierarchy and new assets

Root: `MarsOutpost_Environment_v01_1unit_1meter`

- `01_Terrain_250x250m`: original bounds with a newly owned regolith mesh and collider.
- `02_EnvironmentZones`: H/G/R/P/S/C/D/B groups at approved coordinates; descriptive colliders and combined material assemblies.
- `03_ApprovedRoute_Waypoints_421p62m`: documented route points, no floating markers.
- `04_Lighting`: single afternoon directional Sun.
- `05_CompositionValidationCameras`: five gameplay-height views plus overview; Arrival is the only enabled runtime camera.
- `06_Geology_ClusteredScatter`: reusable rock LOD instances and combined stone chunks.
- `UtilityRoutePosts`: grounded reflector posts offset from the clear corridor.
- `07_FunctionalExteriorDetails`: recessed habitat panels, pressure-service vessel/conduits, power vents/dust aprons and approach stone clusters.

New authoring is under `Assets/AstroGateway/Environment/{Materials,Terrain,Rocks,Architecture,Props,Prefabs,Editor}`. There are **98 non-meta Environment files**, fully listed with size/provenance in `Assets_And_Attribution.md` and hashes in `Asset_Manifest.json`. The scene and additive Stage 04 documentation sit outside that Environment manifest.

Ten reusable prefabs in `Environment/Prefabs/`:

`AngularBasalt`, `ErodedLandmark`, `LayeredDelta`, `RegolithStone`, `H_CommandHabitat`, `G_Greenhouse`, `R_RoverBay`, `P_PowerStation`, `S_SolarArray`, `C_ScienceStaging`.

Eleven URP Lit surface materials: `Mars_Regolith`, `Mars_Regolith_Dark`, `Mars_Rock_Basaltic`, `Mars_Rock_Layered`, `Habitat_OffWhite`, `Habitat_CompositeDark`, `Metal_Industrial`, `SolarPanel`, `Glass_Utility`, `Accent_Cyan`, `Accent_SafetyOrange`. One additional original `Mars_DustSky` material uses the lightweight `DustSky.shader`.

## Terrain and geological work

Macro scale: retain the approved rise to the northern working terrace; add restrained off-route undulations and a shallow irregular channel. Protected buffers exclude changes near routes and reserved pads. The continuous mesh stays on its 2.5 m grid; no Terrain package or imported DEM was introduced.

Meso scale: dark igneous clusters, a separate rock-comparison site, fractured layered northern outcrops and low eroded toes. Delta assets were revised after capture review and embedded against the sloping ground to avoid visible suspended bases. Angular, landmark and layered families each have three mesh LODs; small stones share a low-cost mesh and are combined in chunks rather than being individually expensive objects.

Micro scale: a 1024 px macro color/path-wear map, 512 px grain/detail and surface textures, and a 256 px micro normal map. Paths read as gently worn ground instead of a solid HUD stripe. Scatter is clustered and excluded from the mission corridor. Compressed imports use mipmaps and disable CPU texture readability. No 4K/8K textures were used.

These are artistic Jezero-inspired interpretations, not calibrated Mars colors, measured rock morphology or an exact reconstruction of Jezero. Reference distinctions follow the approved evidence labels.

## Architecture and materials

Habitat: preserve the 14 × 8 m shell envelope, with chamfered pressure shell, structural rings, foundation feet, dark service skirt, east airlock, cyan identification, original H01 stencil, recessed access panels, exterior utility connections and 8 m communications mast.

Greenhouse: shielded modular exterior, roof lighting/service cover, small opaque utility-glass observation inserts, water-buffer interface and service console. No interior or open glass dome.

Rover Bay: retain the 18 × 10 m shelter; roof ribs, braced supports, staging crates and a static chassis/wheel proxy. No vehicle driving or character content.

Power: preserve the north-side battery/distribution enclosure and approach clearance; service panels, warning strips, vents, dust aprons and 6 m mast. Low-profile utility conduits connect toward the array without placing a blocking cabinet on the mission route.

Solar: four tilted rows, 32 cell-patterned cassettes, edge frames, supports/braces, row cables and combiner equipment. Slight dust variation appears in the panel surface texture.

Science: sample bench, sealed cases/latches, storage, protected display/console and instrument mast.

Materials use restrained smoothness: ground/rock 0.12; painted/composite surfaces about 0.24; industrial metal 0.35 with metallic 0.75; panel surface 0.48 with metallic 0.15. Glass inserts are **opaque** for browser cost and readability in this pass; no transparent-overdraw system or interiors are promised. Cyan/orange accents are reflective colors, with no excessive emission. Surface dust comes primarily from albedo variation and geometry; bespoke packed roughness maps are not included yet.

Shared-material pieces are combined per assembly to reduce submissions. Rock materials enable instancing; actual batching behavior depends on the URP render path and is measured below. Building prefabs contain the primary assemblies; the later exterior detail kit is a separate scene group.

## Lighting and composition

The directional Sun remains **30° above the southwest**, rays travelling northeast/down and shadows extending northeast. Intensity 1.5 is an artistic Unity value, not measured Mars irradiance. Warm restrained Sun color, soft shadows, modest bias, trilight ambient fill, a muted original gradient sky and linear atmospheric fog (100–340 m) provide distance separation. No volumetric effects, local real-time light network or post-processing was added. Pipeline assets, quality settings and packages remain unchanged; Web retains the template Mobile_RPAsset profile, render scale 0.8 and 50 m shadow distance.

All five views use camera height **1.7 m above the local ground**, without a controller:

| Capture | Composition |
|---|---|
| `View_A_Arrival.png` | Habitat airlock foreground, greenhouse and worn route midground, delta/science background; camera near spawn at X −42 / Z −69 |
| `View_B_Habitat.png` | Pressure-shell/airlock hero, utility detail and agricultural module behind |
| `View_C_SolarPower.png` | Approach stones foreground, power enclosure left, panel rows right, distant terrain behind; wider 74° vertical FOV to include both systems |
| `View_D_Delta.png` | Near bedding/toe foreground, fractured exposures midground and rising terrain behind |
| `View_E_Return.png` | Stone clusters foreground, reflector route and recognizable habitat mast/airlock on return |
| `View_F_Overview.png` | Full layout inspection; not a gameplay-height camera |

The gallery `Review.html` labels captures as Editor URP renders. The copied Stage 03 spawn image is a structural reference from a different camera pose, not a matched before/after render. Actual browser rendering was separately observed in the running release build and archived in `Browser_Run.png` (uninstrumented release page).

## Validation results

- Saved scene reopened twice through EditorSceneManager; final missing-script/material/shader checks passed.
- Unity C# and sky shader compiled; final build summary has **0 errors**.
- Geometry validation: **0 errors**, eight centers correct, bounds correct, actual mesh route unchanged.
- Conservative collider-bound checks retain a 4 m corridor over 857 route samples. No controller, NavMesh or human walking trial was claimed.
- Spawn→power mast clear; spawn→north delta sight ray clear; delta→habitat roof first hit is the target shell. Gameplay-height captures also show the relevant landmarks. The ray tests are not a full visibility sweep.
- Scene Console was cleared of earlier resolved authoring messages before final reopen validation; immediate recorded counts were zero errors/warnings. Full authoring/build logs remain available, including resolved messages.
- Release Web build actually loaded and rendered at `http://127.0.0.1:8767/Web/index.html`; materials, sky and Sun shadows appeared correctly. No browser error was observed in the captured log.

## Measured performance — limits included

| Measurement | Obtained result / meaning |
|---|---|
| BuildPipeline reported final build size | **13,164,632 bytes / 12.55 MiB**, Brotli-compressed build; QA HTML/JS are separate additions |
| Build result | Succeeded; 0 errors; 3 warnings |
| Unique referenced mesh assets | **28,366 vertices / 35,474 triangles**, all LOD levels combined; not a visible-frame total |
| Angular family LOD0/1/2 | 140 / 60 / 26 triangles |
| Landmark family LOD0/1/2 | 252 / 96 / 32 triangles |
| Layered family LOD0/1/2 | 568 / 192 / 60 triangles |
| Render components | 247 renderers, including alternative LOD renderers; 75 LOD groups; not 247 active draw calls |
| Editor mesh-object resident estimate | 3,610,008 bytes via Profiler.GetRuntimeMemorySizeLong |
| Editor referenced texture-object estimate | 4,725,888 bytes via the same API |
| Mesh + texture subset | **7.95 MiB**; excludes objects, colliders, pipeline buffers, allocator overhead and runtime heap |
| Browser sample | Apple M4 Pro / ANGLE Metal; Chrome 155 reported UA; 1920 × 1200 drawing buffer, 960 × 600 CSS area, DPR 2 |
| Stationary arrival-view frame cadence | 5 s warmup + 20 s sample; 2400 samples; **120.01 approximate presented fps**, mean **8.33 ms**, p50 8.33 ms, p95 **9.26 ms** |
| Intercepted WebGL submissions | Mean **61 draw submissions/frame**, **56,428 submitted triangles/frame** including extra passes, **11 useProgram calls/frame** |
| Unity SetPass / engine batches | Unavailable; useProgram and GL submissions are not renamed as these counters |
| Web texture GPU memory / total scene or app memory | Unavailable; Editor resident-object estimates above are not Web GPU figures |

The separate `Web/performance.html` QA harness intercepts WebGL draws and samples frame presentation intervals; it is not a runtime gameplay script or final HUD. Instrumentation overhead is present. The normal `index.html` is uninstrumented. The 120 fps result is a **single stationary view on this high-end Mac**, not full-route, 1080p target-device, mobile, CPU/GPU-timing or sustained thermal acceptance. No mobile performance claim is made. Raw results: `Browser_Performance.json`; per-asset counts: `Asset_Statistics.md`.

## Provenance, warnings and limitations

All new meshes, textures, shader code and stencil markings are original procedural project work. Unity primitives/URP assets are provided by the installed engine. No paid assets, third-party textures/fonts/models, NASA media/model files or unclear-license content were imported. Full source/provenance ledger: `Assets_And_Attribution.md`.

Scientific reference: [NASA PIA24921 Jezero delta panorama](https://science.nasa.gov/photojournal/detailed-panorama-of-mars-jezero-crater-delta/) (NASA/JPL-Caltech/ASU/MSSS; enhanced main image, natural-color Figure A), [NASA Mars facts](https://science.nasa.gov/mars/facts/), and [USGS Jezero igneous floor record](https://www.usgs.gov/publications/aqueously-altered-igneous-rocks-sampled-floor-jezero-crater-mars). The controlled agricultural exterior also follows the concept distinction in [NASA's terrestrial greenhouse prototype research](https://www.nasa.gov/science-research/lunar-martian-greenhouses-designed-to-mimic-those-on-earth/). Fictional architecture is not a NASA design, endorsement or validated engineering system.

The release build summary reports three toolchain warnings. The initial compilation log identifies the Firefox/WebGPU compatibility notice, deprecated JavaScript `$stackTrace` library symbol, and Emscripten's JS-warning summary. The tested scene runs through WebGL 2; no packages or renderer features were changed to silence these messages.

The browser repeats the inherited **unsupported/stripped Edge Adaptive Spatial Upsampling shader** warning in Blit FSR Upscaling, saying post-processing passes will not execute. The current cameras have no post-processing and the scene renders. Installed URP source resolves Automatic with scale 0.8 to linear filtering; the exact unused-FSR resource/stripping cause is not fully isolated. See `FSR_Investigation.md`. Future post-processing changes need renewed validation; the warning is not declared universally harmless.

One transient authoring command arrived before its updated Editor assembly was refreshed and was rejected as unknown; it was rerun successfully after Refresh. Evidence is retained in `Resolved_Authoring_Command_Error.txt`; it is not an unresolved scene or compilation error. The authoring utility uses machine-local QA paths and is Editor-only.

Remaining visual weaknesses: broad ground still needs more carefully authored meso erosion and local material transitions; rock shape/strata variation is limited and some bedding reads as stylized procedural geometry; greenhouse/rover/science are simpler than the habitat; large assemblies use combined geometry rather than architectural distance LODs; no light bake, bespoke occlusion/roughness atlas or final anti-aliasing polish; map-edge horizon composition is constrained by the approved footprint. The airlock remains a sealed visual exterior, and the rover remains a static proxy. Small stones deliberately have no gameplay colliders. Some generated intermediate assets are not referenced by the final build.

No approved layout redesign, character/controller, mission logic, backend, AI, React changes, interiors, driving, full storm, animation, package install or final HUD work occurred.

Recommended next art milestone, subject to approval: improve fractured delta morphology and ground-to-rock transitions, add architecture LODs, then perform a representative camera-route browser profile on the actual minimum desktop/mobile devices. Character/gameplay work has not begun. **Stopped after Environment Art Pass 01.**
