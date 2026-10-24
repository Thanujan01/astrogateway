# Import validation — 11 October 2026

**FAIL / STOP: critical rover texture-extension incompatibility.**

Isolated project: `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/work/stage08d1/VisualEvaluation`. Unity 6000.6.4f1, URP 17.6.0. Original MissionRedHorizon project was not changed. Official registry package `com.unity.cloud.gltfast` 6.20.0 (Unity minimum 6000.0) installed only in this new evaluation project, with registry dependencies recorded in Packages/packages-lock.json. Package provenance: https://packages.unity.com/com.unity.cloud.gltfast . No replacement artist assets downloaded, no astronaut imported, no account routes accessed.

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
