from pathlib import Path
import json,hashlib,collections
out=Path('outputs/Stage08D1');p=Path('work/stage08d1/VisualEvaluation');a=p/'Assets/ApprovedAssets'
rows=[]
for f in sorted(a.glob('*.glb.meta')):
 s=f.read_text();r=s[s.index('  reportItems:'):];rows.append({'file':f.name,'reportItemsYAML':r,'cleanImportReport':r.strip()=='reportItems: []'})
(out/'Importer_Reports.json').write_text(json.dumps(rows,indent=2))
m=json.load(open('work/stage08d/Optimized/Optimization_Metrics.json'));sourcecheck={f:Path(f).exists() and hashlib.sha256(Path(f).read_bytes()).hexdigest()==h for f,h in m['sourceHashes'].items()};(out/'Acquired_Source_Preservation.json').write_text(json.dumps(sourcecheck,indent=2))
root=Path.cwd();project=str((root/p).resolve())
(out/'Import_Validation_Report.md').write_text(f'''# Import validation — 11 October 2026

**FAIL / STOP: critical rover texture-extension incompatibility.**

Isolated project: `{project}`. Unity 6000.6.4f1, URP 17.6.0. Original MissionRedHorizon project was not changed. Official registry package `com.unity.cloud.gltfast` 6.20.0 (Unity minimum 6000.0) installed only in this new evaluation project, with registry dependencies recorded in Packages/packages-lock.json. Package provenance: https://packages.unity.com/com.unity.cloud.gltfast . No replacement artist assets downloaded, no astronaut imported, no account routes accessed.

## Actual workflow

Copied nine approved optimized GLBs and approved Sandy Gravel maps into Assets/ApprovedAssets. Created comparison copies of the three source meshes through existing staging NodeIO/Draco decoder; removed Draco extension after decoding, without modifying acquired originals. The original rover comparison retained material extension/texture encoding information. No FBX conversion or Blender installation occurred.

Initial Unity batch import exited 0, **but that is not an import-success verdict**: glTFast logged failed rover imports. Scene-building execution compiled and then exited 1 with `Exception: Import missing Perseverance_LOD0`. The builder checks for a usable GameObject prefab rather than proceeding with missing visuals. No evaluation scene was saved.

## Per-asset evidence

- Rock07 and Rock09: all six optimized GLBs and both original comparison GLBs have glTFast `reportItems: []` in importer metadata. This supports clean importer diagnostics, not visually verified PBR fidelity.
- Ground: four approved 2K PNG maps imported. Local URP/Lit Ground.mat created; generated smoothness texture in evaluation copy from 1−roughness; data maps configured linear, normal map marked NormalMap. Final appearance not evaluated.
- Rover LOD0/1/2: required `EXT_texture_webp`; embedded images include WebP. glTFast records LogType.Error / code20 / EXT_texture_webp; GameObject load fails.
- Original rover comparison: optional WebP plus IOR/specular extensions trigger diagnostics, followed by additional errors. No usable visual comparison completed. Do not interpret stripped Draco as full Unity conversion success.

Evidence: Initial_Import.log, Scene_Build.log, Importer_Reports.json, GLB_Compatibility_Inspection.json. Earlier zero Khronos validation errors mean legal glTF, not importer support for every extension.

## Proposed smallest fix — not executed

Make additional derivatives from preserved sources: transcode every WebP texture to lossless PNG, rewire core image/texture references, remove EXT_texture_webp only after all replacement references are valid. Keep normal/data maps linear and preserve alpha. Validate again with Khronos validator, inspect importer reports, then retry isolated scene build. Do not merely delete the required extension: that can silently discard textures. Review optional original IOR/specular handling separately; do not claim exact original material parity if approximated. No new artist asset or login needed for this conversion.

The user explicitly requested stopping on critical import issues, so no transcode workaround or further scene construction was performed. Sources unchanged per Acquired_Source_Preservation.json; three recorded original Unity baseline hashes match Preservation_Check.json.
''')
(out/'Rover_Visual_Review.md').write_text('''# Rover review

**BLOCKED: rover did not produce a usable Unity prefab.** No actual rendered rover fidelity, skin deformation, wheel articulation, material appearance, NASA markings or scale acceptance test passed.

Source: 199,521 triangles, 47 materials, 68 meshes, one mechanical skin, 23 animation objects. Optimized LODs: 91,916 / 47,280 / 27,088 triangles, each 43 materials. Metadata inspection confirms required WebP extension in optimized derivatives, the critical importer failure.

Khronos validator warnings from Stage08D remain 36 / 36 / 35. Warning classes include runtime-generated tangent space and a skinned mesh node below a parent transform; glTF validity does not guarantee matching Unity transforms. Need normal-map/tangent comparison and rest-pose/skinned bounds validation after texture import succeeds. No automatic joining of animated nodes performed. Count of retained clips is not proof of playable mechanical animation.

Official NASA repository README permits use of model resources and points to usage guidelines; NASA identifiers cannot imply endorsement. No publishing or brand approval was inferred. Rights source: https://github.com/nasa/NASA-3D-Resources and https://www.nasa.gov/nasa-brand-center/images-and-media/ . This approved model is an uncrewed robotic rover, not astronaut transport.

Stop issue: WebP conversion required; original reference also needs optional material-extension review. Physical performance and material draw-call consolidation untested.
''')
(out/'LOD_Quality_Comparison.md').write_text('''# LOD quality comparison

**BLOCKED: no identical-camera Unity comparison captures generated.** The evaluation builder stopped before camera creation. No visual fidelity result is invented.

| Asset | Original | LOD0 | LOD1 | LOD2 |
|---|---:|---:|---:|---:|
| Rock07 triangles | 14,844 | 5,936 | 2,226 | 742 |
| Rock09 triangles | 12,416 | 4,966 | 1,862 | 620 |
| Rover triangles | 199,521 | 91,916 | 47,280 | 27,088 |

Imported files are staged, but **LODGroups are not configured in a saved scene**. Builder proposes projected-height thresholds .5/.2/.035 and restores automatic LOD after forced-LOD0 captures. Those thresholds require visual tuning for small stones and rover silhouette; not approved as measured quality settings. Texture derivatives cap edges at 1024/512/256; source references retain originals.

After resolving WebP, compare source and LOD0 at fixed position, FOV42 and 1600×1000 resolution, identical sunlight. Check rock silhouette and normal-detail retention, rover fine instruments, wire/antenna continuity, alpha, moving pivots and resting wheel transforms. Capture each lower LOD as well before accepting distant simplification. No performance benefit inferred from triangle reduction alone.
''')
(out/'URP_Scene_Validation.md').write_text('''# URP scene validation

PASS: isolated project resolved Unity 6000.6.4f1 + URP17.6.0 + official glTFast6.20.0; editor builder compiled with deprecated FindObjectsByType overload warnings (no C# compile errors observed). Rock importer diagnostics empty; approved ground maps/material imported.

FAIL: rover GameObject missing following unsupported required WebP extension; builder aborted with exit1. **Assets/Evaluation/Stage08D1_Visual_Evaluation.unity was not created.** Ground.mat and an unsaved partial setup are not a completed scene.

Prepared local builder: Assets/Editor/Stage08D1Builder.cs. Proposed 60m material test surface, genuine small stones, robotic rover, one directional afternoon light, inexpensive exponential-squared fog, restrained saturation/contrast and Neutral tonemapping. No weather particles or astronaut. None of these final composition/lighting/rendering checks passed because build stopped before scene save and rendering.

BLOCKED: missing-material/shader rendering checks, mesh transform and skin validation, final LODGroups, actual camera captures and scene reload. UI screenshot attempt reached the original project only, then Mac lock prevented selecting/inspecting the evaluation window. Original project was not used as evaluation proof.

Preservation: original playable scene, MarsPrototype.cs and manifest.json match prior recorded SHA256 hashes. All acquired source-file hashes unchanged. No Android/React/backend/Supabase modifications.
''')
(out/'Android_Performance_Status.md').write_text('''# Android performance status

**PHYSICAL DEVICE UNTESTED.** Approved installed ADB queried with `adb devices -l`: list empty. An initial sandbox-restricted daemon start failed; rerun with local device-access permission successfully started ADB and still found no device.

No Android evaluation build, install or profiling performed because the critical rover import blocked scene creation. No average FPS, GPU time, CPU time, memory, thermal, draw-call or rendering improvement claim. Prior software-emulator results are not measurements of this evaluation scene.

Next: after import/scene validation, connect an authorized physical device and record model/SoC/Android version, resolution and URP profile; collect repeatable clear-view measurements with the same camera, compare original/LOD variants, then tune texture compression and lighting based on evidence.
''')
(out/'Stage08D1_Final_Report.md').write_text(f'''# Stage08D.1 final report

**Stopped on critical import incompatibility, as requested.**

Evaluation project: `{project}`.

| Check | Result |
|---|---|
| Original scene/source assets preserved | PASS, recorded hashes unchanged |
| Isolated official importer and URP setup | PASS |
| Rock07/Rock09 original and LOD import diagnostics | PASS (empty reports; visual review pending) |
| Ground map import and URP material preparation | PASS preparation, appearance untested |
| Rover LOD0/1/2 conversion/import | FAIL, required EXT_texture_webp unsupported |
| Complete saved evaluation scene / LODGroups | BLOCKED by rover import |
| Original versus LOD0 visual comparison | BLOCKED |
| Actual evaluation screenshots | BLOCKED, scene failure and Mac locked |
| Physical Android profiling | UNTESTED, no ADB device |

All seven requested Markdown reports are in this folder. Logs and importer metadata provide actual failure evidence. Screenshots/ is intentionally empty; no existing gameplay view or generated image is presented as evaluation imagery.

Smallest next action: authorize continuation after reviewing the failure, then create PNG-texture derivatives of the already approved rover (preserving sources), handle extension references correctly, revalidate glTF and retry isolated import. Unlock Mac for actual Editor visual QA. Optional original material-extension diagnostics and skin/tangent warnings must remain explicit until tested. No new artist asset approval needed for those approved-rover derivatives, but this turn stops under the stated critical-issue condition.
''')
print('Reports written. Sources unchanged:',all(sourcecheck.values()),'Scene exists:',(p/'Assets/Evaluation/Stage08D1_Visual_Evaluation.unity').exists())
