# ARES-VI isolated Unity showroom verification (20 Oct 2026)

**Verdict: not yet a usable, visually acceptable Unity asset.** The files are real and import into Unity 6 URP without errors, at a sensible scale, with all 8 materials resolving. But 45% of triangles face inward (holes under normal back-face culling). The model also has no animations, no part pivots and no textures, and its LOD1/LOD2 are not usable. The rover was not placed in the playable Mars scene.

Everything below was measured in this run: by script on the source files, in the Unity 6000.6.4f1 importer, or in Play Mode on Apple M4 Pro / Metal. Nothing is taken from the earlier ARES-VI report.

## 1. Source files (independently checked)
Location: `~/.gemini/antigravity/scratch/3d-game-assets/` (a Gemini/Antigravity scratch folder, not inside the Unity project).
| File | SHA-256 (first 12) | Finding |
|---|---|---|
| ares_vi_aaa_master.obj | 7029563c0dac | 8,912 v, 15,518 tris, 211 groups, 8 materials, UVs present |
| ares_vi_aaa_lod0.obj | 7029563c0dac | **byte-identical to master** |
| ares_vi_aaa_lod1.obj | ca7a4655fe7a | 9,950 tris, 198 groups, **no UVs**; reuses master's full vertex list (2,184 unused), 5,246 open edges |
| ares_vi_aaa_lod2.obj | a875060a0fa2 | 1,185 tris, 73 groups, **no UVs**; 6,920 unused vertices, 3,331 open edges |
| ares_vi_aaa_master.mtl | ce4e6c9b1df3 | 8 colour-only materials (Kd/Ks/Ns/d/Ke); **no texture maps** |
| ares_vi_aaa_master.glb | 4fd5f45d3f45 | glTF 2.0, **1 node, 1 mesh, 0 materials, 0 textures, 0 animations, 0 skins**; Z-up although glTF is Y-up |

There are no texture files anywhere in the folder. `AresViLODConfig.json` claims LOD0 = 1,401 tris and 4.82 × 2.84 × 2.45 m. Measured: 15,518 tris and 3.206 L × 2.899 W × 2.757 H m. `ARES_VI_AAA_Variant.prefab` is hand-written YAML that references meshes and a script GUID that don't exist. Neither file was imported.

Geometry script results (master): 0 degenerate triangles; 7,010 triangles (45.2%) whose vertex normals oppose their winding; **185 of 211 parts have inside-out winding** (negative signed volume); 212 non-manifold edges.

## 2. Unity import (`Unity_Import_Report.txt`)
Imported into new folder `Assets/AstroGateway/VehicleShowroom/ARES_VI/Source/`; scene `Assets/AstroGateway/VehicleShowroom/Scenes/ARES_VI_Showroom.unity`; builder/harness `Assets/AstroGateway/VehicleShowroom/Editor/AresViShowroom.cs`.
- OBJ imports cleanly at scale 1 (metres). Model is **Z-up, nose on +Y**. The showroom parent rotates it (270°, 180°, 0); measured result: mast on top, bumper at +Z, FL wheel at −X (not mirrored).
- LOD0 in Unity: 211 renderers, 15,518 tris, 38,110 vertices after split, **flat hierarchy (depth 1), all 211 pivots at the model origin**.
- Materials: importer made URP Lit materials by default. I replaced them with 8 URP Lit materials built from the MTL values (`ARES_VI/Materials/`). In Play Mode: 211 slots, 0 missing, 0 non-URP. They are flat colours: no albedo, normal, roughness or AO maps exist to import.
- GLB: Unity sees it as `DefaultAsset`. No glTF importer package is installed, and I did not add one because that is a project-wide change. Even with an importer it has no materials or hierarchy.

## 3. Visual inspection (`Screenshots/`, real Game View captures, 3024×1584)
01 front · 02 rear · 03 left · 04 right · 05 top · 06 three-quarter · 07 undercarriage · 08 cockpit · 09 left suspension · 10 science arm · 11 ChemCam mast · 12 hover modules from below · 13 diagnostic · 14 LOD lineup.

Defects seen:
- **Missing surfaces everywhere (inside-out faces).** Hull side panels, tyre sidewalls, canopy glass, the gold antenna dish and the seat backs are culled. Compare 06 with 13: 13 is the same shot with Render Face = Both, as a diagnostic only, not a fix.
- Tread lugs float outside the tyres as separate spikes (09, 01). The drill bit floats below its housing with a visible gap (10).
- The cockpit reads as a flat deck with fin-like seats and no visible glass from outside (08).
- LOD lineup (14, screen left to right = LOD2, LOD1, LOD0): **LOD2 is broken** (detached fragments, half the body missing). LOD1 drops parts and has no UVs.
- Scale is plausible (wheel Ø 0.76 m, rover 2.76 m tall), and normals are flat-shaded (only 396 unique normals).

## 4. Animation
| Required | Result |
|---|---|
| Wheel rotation | **MISSING**: 0 clips; wheel pivot at rover origin, so a 90° spin moves the tyre 1.753 m |
| Rocker-bogie suspension | **MISSING**: 0 clips; rocker pivot at origin (moves 1.089 m) |
| Science arm deployment | **MISSING**: 0 clips; arm pivot at origin (moves 2.230 m) |
| ChemCam movement | **MISSING**: 0 clips; ChemCam head pivot at origin (moves 3.770 m) |
| Hover activation | **MISSING**: 0 clips; vane pivot at origin (moves 1.095 m) |
| Hover stabilization | **MISSING**: 0 clips |
| Landing | **MISSING**: 0 clips |

Unity found 0 Animator, 0 Animation components and 0 clips in any OBJ, and the GLB has 0 animations. `AresViHoverVisualController.cs` in the source folder is a script, not imported animation. It was not imported or tested.

## 5. Performance (`Verification_Log.txt`)
Editor Play Mode, quality "PC", Game View 3024×1584, fixed three-quarter camera, 360 frames each.
| Case | FPS | CPU frame p50/p95 ms | GPU p50 ms | Draw calls | SetPass | Tris rendered |
|---|---|---|---|---|---|---|
| Showroom, ARES-VI LOD0 | 725.8 | 1.09 / 2.37 | 1.37 | **705** | 18 | 55,078 |
| Showroom, empty stage | 823.2 | 0.94 / 2.02 | 2.46* | 13 | 13 | 2,166 |
| Mars Visual Upgrade scene (baseline, idle, capped 30) | 26.9 | 33.35 / 68.91 | 11.60 | 799 | 63 | 1,353,056 |

\*Only 198 of 360 GPU samples were returned in the empty-stage case.

Memory with rover: 2,227 MB allocated and 526 MB graphics driver, against 2,218 MB / 531 MB without it (Editor-wide figures, so the difference is within noise). The Mars scene used 2,613 MB / 579 MB.

**Takeaway:** triangles are cheap, but the rover alone adds about 692 draw calls, close to the whole Mars scene's 799. That is because it is 211 separate renderers, each also drawn in the shadow passes. It would need merging by material (about 8 draw calls) before it goes into the game. The empty-stage GPU median being higher than with the rover is Editor noise; treat the Editor GPU numbers as indicative only. No player build was measured.

## 6. Export/import limitations
OBJ cannot carry hierarchy, pivots or animation. The GLB carries none either. No glTF importer is installed. MTL gives colour only. LOD files are face-subsets of the master, not decimated meshes.

## 7. What it would take to be usable
Re-export from the authoring tool as FBX or glTF, with:
- outward-consistent winding and smooth normals;
- a parented hierarchy with real pivots (wheel axles, rocker and bogie joints, arm joints, mast pan/tilt, hover vanes);
- UVs on every LOD, with real decimated LODs;
- texture sets;
- the seven animation clips (or a rig that code can drive).

## Preservation
- Only new folders were added: `Assets/AstroGateway/VehicleShowroom/` and `Docs/VehicleShowroom/`. The production rover, gameplay, controls, boarding, mission systems and the Mars scenes are untouched, and the Mars scene was opened in Play Mode only, never saved.
- The harness flipped `runInBackground` in ProjectSettings; I restored it with git checkout, and `git status` shows no other tracked change.
- Backup taken first: `~/Projects/MissionRedHorizon_Backups/ARES_VI_Showroom_PreChange_20261020/` (verified git bundle of all branches, status, settings hashes).
- Nothing was committed, pushed, merged or deployed. Rollback: delete the two new folders and `Assets/AstroGateway/VehicleShowroom.meta`.
