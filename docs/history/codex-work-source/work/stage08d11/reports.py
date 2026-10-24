from pathlib import Path
import json,hashlib,struct,collections
root=Path.cwd();o=root/'outputs/Stage08D11';e=json.loads((o/'Conversion_Evidence.json').read_text());proj=root/'work/stage08d1/VisualEvaluation';assets=proj/'Assets/ApprovedAssets'
base=json.load(open('outputs/Stage08D/Inspected_Source_Hashes.json'));checks={p:hashlib.sha256(Path(p).read_bytes()).hexdigest()==h for p,h in base.items()}
metrics=json.load(open('work/stage08d/Optimized/Optimization_Metrics.json'));checks.update({p:hashlib.sha256(Path(p).read_bytes()).hexdigest()==h for p,h in metrics['sourceHashes'].items()})
for r in e: checks[r['source']]=hashlib.sha256(Path(r['source']).read_bytes()).hexdigest()==r['sourceSha256']
(o/'Source_Preservation_Checks.json').write_text(json.dumps(checks,indent=2))
imported=[]
for i in range(3):
 p=assets/f'Perseverance_LOD{i}_PNG.glb.meta';s=p.read_text();imported.append({'lod':i,'reportItemsYAML':s[s.index('  reportItems:'):]})
(o/'Unity_Importer_Diagnostics.json').write_text(json.dumps(imported,indent=2))
text='''# WebP texture inventory — 11 October 2026

Inspected acquired original GLB, predecoded source-reference GLB and three optimized GLBs. Acquired/source-reference assets contain 22 WebP images each; optimized variants contain 21 each. No new acquired-original derivative was saved: the acquired original was inventoried only. The already decoded reference copy was converted separately for Unity comparison.

All WebP image, texture, material-path, bufferView, byte offset, dimensions and conversion records are in Conversion_Evidence.json. The table below covers every WebP in every inspected input. Texture indices are zero-based; image references promoted from EXT_texture_webp.source to core texture.source. Existing material indices and texture slot definitions unchanged.

| Input | Image | Source bufferView | Offset / bytes | Texture indices | PNG dimensions | Material texture references |
|---|---:|---:|---|---|---|---|
'''
for r in e:
 for im in r['webpImages']:
  text+=f"| {r['name']} | {im['image']} | {im['oldBufferView']} | {im['oldByteOffset']} / {im['oldByteLength']} | {','.join(map(str,im['textures'])) or 'none'} | {im['width']}×{im['height']} | {'; '.join(im['materialReferences']) or 'unused/fallback image'} |\n"
(o/'WebP_Texture_Inventory.md').write_text(text)
(o/'GLB_Conversion_Report.md').write_text('''# GLB conversion report

**PASS: texture encoding conversion and structural preservation for all three LODs.** New derivatives in `work/stage08d11/Converted/`, copied as new filenames to the isolated project's Assets/ApprovedAssets. Original GLBs untouched.

Script: `work/stage08d/tools/convert_webp.mjs`, existing staged Sharp decoder/encoder and Khronos validator. It parses GLB JSON/BIN directly, avoiding mesh export/reimport. Each WebP becomes PNG at identical dimensions and decoded RGBA pixels (byte equality asserted). Original BIN prefix remains byte-identical; PNG payloads appended at 4-byte aligned offsets with new bufferViews. PNG MIME and image bufferView updated, buffer length and GLB chunk lengths rebuilt. Texture WebP-extension source promoted to core source; extension removed from texture objects and global declarations only after every referenced image is PNG. Other texture extensions, samplers, material channels and assignments retained.

Assertions: meshes, accessors, nodes, skins, animations, materials and samplers JSON unchanged; original binary prefix identical; decoded pixel equality for every converted image. Thus this conversion does not change UVs, normals, tangents (including their source absence), indices, bone data, transforms or existing LOD simplification. No normal regeneration or geometry repair performed. Other JPEG/PNG payloads are preserved. Old image buffer bytes remain as unreferenced source payload in the new GLB: conservative preservation increases file size, and is not a mobile-size optimization.

Original reference copy converted too, with original 47 materials and 199,521 triangles; its inherited non-normalized skin weights remain documented. No original file normalized or overwritten. SHA256 source checks in Source_Preservation_Checks.json all true, including acquired assets, optimized sources and recorded main Unity scene/script/package baselines.

No unsupported **required** extension remains in the three new LOD files. Optional IOR/specular data is preserved but not implemented by glTFast's selected Unity material path. This renderer limitation prevents full material-fidelity acceptance; see Rover_Material_Comparison.md. No performance claim.
''')
(o/'GLTF_Validation_Results.md').write_text('''# glTF validator results

Khronos glTF Validator 2.0.0-dev.3.10, validateBytes on final output bytes. Complete diagnostics embedded in Conversion_Evidence.json.

| Asset | Errors | Warnings | Required extensions |
|---|---:|---:|---|
'''+''.join(f"| {r['name']} PNG | {r['validation']['numErrors']} | {r['validation']['numWarnings']} | {', '.join(r['remainingRequired']) or 'none'} |\n" for r in e if r.get('validation'))+'''
All three optimized rover derivatives have zero errors. Existing warning classes include generated tangent space and non-root skinned-mesh nodes; unused data can generate informational diagnostics. These are not proofs of accurate articulation.

The converted original reference has inherited skin-weight normalization errors. It is included to document source limitations, not falsely labeled validator-clean. Conversion left source geometry/weights untouched. It imported and rendered as a static comparison, but mechanical animation acceptance is outside these checks. Acquired Draco original was inventoried only, not validated as a converted output.
''')
(o/'Unity_Import_Results.md').write_text(f'''# Unity import results

**PASS: three converted rover prefabs imported and saved scene reload succeeded.**

Project: `{proj}`. Unity6000.6.4f1, URP17.6.0, official glTFast6.20.0. No new package added in this continuation. Batch build exit0; separate reload/asset probe exit0. Evidence: Unity_Import_Build.log, Scene_Reload_Validation.log, Unity_Asset_Checks.txt, Unity_Importer_Diagnostics.json.

Unity measured each LOD: 68 meshes, 43 materials, 21 imported Texture2D subassets, zero null material slots. Triangle counts91,916 /47,280 /27,088. All loaded shaders supported. The GLBs retain44 image records; Unity creates21 textures used by the supported material path, not44 rendered maps. Texture dimensions/formats and material shaders/render queues recorded individually in Unity_Asset_Checks.txt.

All three importer reports retain warnings for KHR_materials_ior and KHR_materials_specular; no importer error in converted LOD reports. Original failed WebP files remain untouched in the project and retain historical diagnostics, but the scene references new PNG files.

Saved NEW scene: `{proj}/Assets/Evaluation/Stage08D1_Visual_Evaluation.unity`. Three rover LOD renderer lists checked nonempty/non-null after scene reload.49 LODGroups in composition (rover+48stones). Root rover bounds center(0,.92,0), extents(1.35,.92,1.55), derived from imported resting pose. No astronaut and no change to original playable scene.

Actual screenshots are Unity Editor camera renders produced in batch using Camera.Render/RenderTexture/ReadPixels at1600×1000, not generated artwork, native Editor chrome captures or Play Mode testing.11 PNGs captured, including source reference and all three forced LODs at identical camera pose/FOV/light. Native UI handle exposed the main project, which was preserved and not used as this evaluation's visual evidence.

A material naming warning and deprecated FindObjectsByType warnings occurred in local evaluation tooling; no compile failure. Optional material fidelity and low-LOD artifacts remain blockers to complete visual acceptance.
''')
(o/'Rover_Material_Comparison.md').write_text('''# Rover material comparison

**BLOCKED: full material-channel fidelity cannot be certified with this importer path.** WebP→PNG itself is lossless in decoded pixels and preserves source material JSON. Unity's optional material extension support remains incomplete.

Inspected actual Unity camera renders: Rover_Original.png, Rover_LOD0.png, Rover_LOD1.png, Rover_LOD2.png and Rover_Rear.png. LOD0 retains source-reference silhouette, chassis, six wheels, instrument details and overall pale/metallic/dark material placement. No magenta missing-shader areas or obvious entire flipped surfaces observed in these static angles. This is limited visual inspection, not exhaustive transparency/backface/animation testing.

LOD1 has visible front-housing shading/faceting changes and reduced wheel detail; LOD2 has pronounced wheel/front-housing simplification artifacts. These are inherited from previously optimized geometry, not introduced by PNG conversion (indices/normals preserved byte-for-byte). Low LODs should not be accepted at close-view distances; retune thresholds or redo part-aware simplification in a separately scoped iteration.

## Remaining material support gap

Unity warnings: KHR_materials_ior and KHR_materials_specular. Seven LOD material records carry non-default values: transparent(ior1.45/specular0), glass_trans(ior1.39/specularColorTexture index5), glass lens(ior1.39/colored specular factor), blades(ior1.45/specular0), mars_2020_06(ior1.45/specular≈.3091), glass_2(ior1.39/specularColorTexture index5), mars_2020_06_n(ior1.45/specular≈.3091). The named specular texture is retained in GLB but not proof of a rendered channel. No extension was silently deleted to hide warnings.

Source-reference screenshots use the same Unity renderer with the same limitation, so agreement cannot prove correct original glTF glass/specular behavior. Alpha/transmission data retained, but exact glass appearance, IOR and specular response are not accepted. A supported material adaptation/shader route or reference renderer comparison is required before claiming all materials render correctly.

## Measured image differences

Identical1600×1000 camera renders compared to converted original reference. Full-frame mean absolute8-bit RGB channel differences: LOD0=1.155, LOD1=2.004, LOD2=2.752. Channels differing by>10:3.064%,4.586%,6.087%. Includes geometry, shadows, texture resolution and background; **not** isolated conversion error, perceptual-quality score or mobile performance measurement. Pixel equality tests on decoded texture data are the direct conversion-fidelity evidence.

File textures retain original LOD maxima1024/512/256 pixels; no resize in this continuation. Source reference retains original image sizes. Detailed WebP sizes in inventory and Unity-imported sizes in Unity_Asset_Checks.txt. No device FPS claim.
''')
(o/'Stage08D11_Final_Report.md').write_text(f'''# Stage08D.1.1 final report

**Overall BLOCKED for full visual acceptance; WebP compatibility fix PASS.**

| Completion check | Result |
|---|---|
| All three LODs import successfully | PASS, usable prefabs and zero null material slots |
| No unsupported required extensions | PASS, required list empty |
| PNG pixels/geometry/material definitions preserved | PASS, byte/structure assertions |
| Source files and original playable scene preserved | PASS, SHA256 evidence |
| Isolated evaluation scene saved and reloaded | PASS |
| Textures/materials render fully correctly | BLOCKED, non-default optional IOR/specular unsupported |

No further material adaptation performed after identifying the remaining channel fidelity issue. LOD1/2 visible simplification defects documented separately; conversion did not change their geometry. No unrelated assets, accounts, backend, Supabase or authentication touched. No mobile performance claim.

Scene: `{proj}/Assets/Evaluation/Stage08D1_Visual_Evaluation.unity`.
Derivatives: `{root}/work/stage08d11/Converted/Perseverance_LOD0_PNG.glb`, LOD1/LOD2 equivalents and original-reference PNG copy.
Screenshots: `{o}/Screenshots/` (11 actual Editor camera renders; no native UI screenshot represented). Main-project UI was not changed.

Next review decision: choose a supported URP material adaptation/reference-renderer evaluation for non-default IOR/specular and a part-aware low-LOD refinement. Current saved scene is an isolated diagnostic scene, not production-ready artwork. Stop here with remaining blockers explicit.
''')
print('All hashes preserved:',all(checks.values()),'scene saved:',(proj/'Assets/Evaluation/Stage08D1_Visual_Evaluation.unity').exists())
