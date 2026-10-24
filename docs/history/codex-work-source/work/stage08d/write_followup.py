from pathlib import Path
import json,hashlib,collections
root=Path.cwd();out=root/'outputs/Stage08D';opt=root/'work/stage08d/Optimized'
m=json.loads((opt/'Optimization_Metrics.json').read_text());v=json.loads((opt/'Validation_Results.json').read_text())
preserved={p:hashlib.sha256(Path(p).read_bytes()).hexdigest()==h for p,h in json.loads((out/'Inspected_Source_Hashes.json').read_text()).items()}
(out/'Preservation_Check_2026-10-11.json').write_text(json.dumps(preserved,indent=2))
(out/'Staging_Optimization_Results.md').write_text('''# Staging optimization results — 11 October 2026

Completed outside Unity. Source assets remain in `work/stage08d/Acquired/`; derivatives are in `work/stage08d/Optimized/` under this workspace. No playable scene, package, source script or replacement import changed. The three recorded Unity source hashes match their earlier baseline. All acquired-file SHA256 hashes match before/after optimization.

## Measured results

| Asset | Source triangles | LOD | Output triangles | Reduction | Materials | GLB bytes | Maximum texture edge |
|---|---:|---:|---:|---:|---:|---:|---:|
'''+''.join(f"| {r['name']} | {r['original']['triangles']:,} | {r['lod']} | {r['triangles']:,} | {100*(1-r['triangles']/r['original']['triangles']):.1f}% | {r['materials']} | {r['bytes']:,} | {max(max(d) for d in r['textureDimensions'])} px |\n" for r in m['results'])+'''
## Method and checks

Reproducible script: `work/stage08d/tools/optimize.mjs`; locked dependencies in that folder's package-lock.json. glTF Transform 4.5.1, meshoptimizer 1.3.0 and Sharp 0.35.5. Each LOD starts independently from source. Identical-resource deduplication, welding and bounded-error simplification; requested ratios .40/.15/.05 with error .005/.01/.02. Ratios are requests, not guaranteed geometric error measurements. Limits constrain simplification; the rover cannot reach the requested triangle ratios without relaxing quality or editing parts manually.

Textures resized with aspect preservation to 1024/512/256 maximum edge, JPEG chroma subsampling disabled for noncolor maps. Original formats retained. These are glTF textures, not Unity ASTC-compressed imports. Source 2K maps remain intact. Deduplication reduced rover materials 47→43; no atlas bake or animation-node joining performed. Original Draco geometry decoded; derived GLBs are uncompressed for easier inspection, so file size can increase despite fewer triangles. File bytes are not GPU memory or FPS.

The rover retains 68 meshes, 88 nodes, one skin and 23 animation objects. Small floating-point weight-sum errors discovered by validation were normalized in derivatives. Animation counts do not prove correct wheel/joint playback. Need visual deformation and articulation tests before adoption.

Khronos glTF validator: all nine final GLBs have zero errors. Rocks each have one warning; rover variants have 36/36/35 warnings. Warnings include runtime-generated tangent space and non-root skinned mesh nodes. Full issue details: `work/stage08d/Optimized/Validation_Results.json`. Do not equate format validity with correct Unity rendering. Tangents and skinned transforms require importer/visual review.

## Proposed budgets, not implemented Unity settings

Rocks: current LODs are candidate upper bounds; small source bounds mean these are stones, not delta cliffs. Use projected-height thresholds .35/.15/.04 and cull tiny distant stones; tune against fixed camera screenshots. Reuse one material per rock family and instance static copies. Retain source 2K hero option if 1K normal detail is inadequate. Recommended ASTC 6×6 color, 4×4 normal for Android after visible-quality review; mipmaps on. A 1K ASTC 6×6 map is approximately .45 MiB including mip chain; this is a format estimate, not measured residency.

Rover: 91.9K close LOD is still heavy with 43 materials. Propose next-stage part-aware atlas baking to 4–8 opaque material groups, separate visor/glass where actually required, preserve animated pivots and mechanical skin. Target roughly 60K/30K/12K triangles only after silhouette and articulation review; not achieved here. Use one or two 2K atlases for close inspection, 1K mobile variant; do not erase instruments/markings blindly. NASA imagery/logos need usage review, not a blanket CC0 assumption.

Collision proposal: static rocks use a fitted convex hull ≤32 vertices, or box collider for very small stones; omit nonblocking pebbles. Do not use render LOD meshes as dynamic colliders. Rover uses one chassis box and six wheel capsules/cylindrical convex proxies, no antenna/instrument collision, dedicated interaction trigger. Preserve existing driving collision until proxy shape is tested. No collision meshes or physics behavior were imported or tested.

## Status

PASS: generated nine separate derivatives, hash preservation, zero format-validation errors. PENDING: visual fidelity, tangent generation, skin transform behavior, atlas consolidation, collider fitting, Unity importer compatibility and device measurements. No Android FPS improvement or finished art pass is claimed.
''')
(out/'No_Login_Astronaut_Candidates.md').write_text('''# Astronaut alternatives — 11 October 2026

No candidate has been verified to satisfy **realistic + rigged + clear redistribution rights + downloadable without authentication + Unity-ready idle/walk/run**. No new astronaut archive was downloaded or imported. Public product pages and metadata were researched; no account/login link was followed, no credentials used and no authentication bypass attempted. Public pages can contain unused login widgets; that is not evidence of a download entitlement.

## Candidates and exact primary evidence

| Candidate / exact URL | License evidence | Rig / animation evidence | Textures / format / Unity | Decision |
|---|---|---|---|---|
| [EMU suit, jgilhutton](https://blendswap.com/blend/12622) | Listing says CC-BY, requests credit; license version not specified in displayed metadata. Obtain exact archive license before redistribution. | Author says fully rigged, helmet controls, bone fixes; finger problems documented. No idle/walk/run promise. | Blender 2.6x / Cycles, 18.9 MB; packed textures claimed, normal-map artifacts documented. Map sizes and triangles unknown. Export FBX with unpacked maps and rebuild URP materials; no Humanoid validation. | Closest realistic research lead, but no verified anonymous download; **not acquisition-ready**. Do not follow account routes. |
| [Astronaut, soph](https://blendswap.com/blend/22902) | Listing CC0. | Rig and basic walk claimed; idle/run not established. | 5.35 MB .blend; description says Blender 2.8 but metadata 2.7x. Textures/polycount unknown. FBX export and Humanoid retargeting required. | Futuristic rather than verified realistic EVA; anonymous download unverified. Hold. |
| [Rigged astronaut, Nico Antoniazzi](https://www.artstation.com/marketplace/p/PkJD/rigged-astronaut) | Displayed Standard Use license limits personal use/one commercial project to 2,000 sales or 20,000 views; not unrestricted redistribution. | Rigged low-poly version claimed; clips not listed. | Rigged FBX 540 KB, rigged .blend 110 MB, diffuse.zip 22 MB; map dimensions/rig topology unknown. | Paid/restricted marketplace; not a verified anonymous acquisition. **Do not purchase/import**. |
| [MrLentereng rigged astronaut](https://mrlentereng.itch.io/rigged-low-poly-astronaut) | Author explicitly CC0 1.0 for original and rigged version; credits ThunderWare original. | Standard humanoid skeleton and skin weights claimed; exact idle/walk/run clip inventory not stated. | BLEND+FBX ZIP 422 KB; textures mentioned but no resolution/polycount. Unity Humanoid plausible, untested. | Public name-your-price download listing, no required account indicated; actual archive access not exercised. Explicitly stylized low-poly: **fails realistic brief; not proposed as substitute**. |
| [zisongbr mobile astronaut](https://opengameart.org/content/low-polymobile-astronaut-rigged-and-blend) | CC0 linked to [legal deed](https://creativecommons.org/publicdomain/zero/1.0/); creator notice AstroDev/Fernando Ferreira. Separate MK Toon shader rights not covered. | Rig advertised; clip names not specified. | .blend ZIP 3.8 MB; Unity demonstration with third-party toon shader. Texture dimensions/tris/rig unverified. | Public attached archive listing; toon art fails realism. Do not import shader or character as replacement. |

No table entry has passed local skeleton, texture, license-archive or Unity compatibility inspection; publisher statements are explicitly separated from binary evidence. Preview imagery is available on the linked primary listing pages; no preview is represented as AstroGateway gameplay.

## Rejected sources

[ThunderWare original](https://mariusmopar.itch.io/astronaut-low-poly): CC0 but explicitly unrigged; reject. [Open3DModel space suit](https://open3dmodel.com/3d-models/astronaut-in-space-suit_354637.html): NonCommercial and unrigged; reject. [Grab3D public API](https://grab3d.com/api/v1/models.json) inspected anonymously: its astronaut match is only a stylized helmet, rigged=null; catalog metadata saved in `work/stage08d/grab3d_catalog.json`. [API documentation](https://grab3d.com/developers/) documents keyless access and CC0; that does not create a suitable astronaut. NASA static astronaut downloads are not accepted as rigged substitutes. Muko_Art remains blocked and is not revisited through account routes.

## Approval decisions needed

Recommended: operator supplies a locally licensed realistic rigged EVA FBX/GLB archive, including redistribution license and idle/walk/run provenance, or approves commissioning one with those delivery requirements. A directly supplied archive avoids account navigation and allows actual measurements. No available fallback is approved merely by listing it here.

If the operator explicitly relaxes realism, MrLentereng is a potential CC0 rigged evaluation candidate, but that would change the brief and still needs download/evaluation approval. Until then astronaut replacement remains BLOCKED. Do not silently downgrade to low-poly/toon or unrigged geometry.
''')
(out/'URP_Staging_Integration_Plan.md').write_text('''# Separate URP integration plan — 11 October 2026

Planning only: no scene created and no imports performed.

1. Work in a disposable Unity project copy or isolated asset-evaluation project using 6000.6.4f1 and matching URP. Choose a supported glTF importer after reviewing its package/license or export FBX through an approved Blender toolchain. Current project has no verified glTF importer; do not install one implicitly. Preserve mechanical animation hierarchy, skin and meters; validate bounds and NASA rover scale rather than copying primitive dimensions.
2. Once file/rights checks pass, create `Assets/AstroGateway/Stage08D/MarsOutpost_Visual_Stage08D.unity` as a copy of the existing scene. Original Stage08B scene remains unchanged. Hierarchy groups: ExistingGameplay, ApprovedEnvironmentVisuals, RoboticRoverVisual, Lighting, QA_Cameras. Preserve player/controller, touch UI, provisional event handling and mission targets.
3. Convert approved textures to URP/Lit explicitly. Color maps sRGB; normal/roughness/AO data linear; unpack glTF ARM channels and invert roughness to URP smoothness. Do not merely attach glTF packed maps to arbitrary shader slots. Inspect normal orientation and regenerate tangents. Retain source rights/manifest alongside evaluated derivatives.
4. Use optimized rocks as small stones around existing route edges; preserve approved zone positions, terrain and path clearances. Ground source stays 2K in acquisition staging; terrain material tiling must avoid obvious repetition. These stones do not replace scientifically referenced layered delta geology.
5. NASA Perseverance represents an uncrewed research rover with remote robotic operation. Never seat the astronaut inside it. Preserve original crewed prototype and driving controller until a clearly separated remote-operation presentation is approved. Keep wheel articulation and entry/exit semantics explicit; installing rover visuals alone cannot prove functional integration.
6. Add LODGroups only after same-angle visual comparison. Start thresholds .35/.15/.04 for rocks; rover .5/.2/.07, retain close source option for QA. These are provisional settings. No cross-node mesh joining on moving parts; atlas bake needs a separate measured iteration. Colliders follow the recipe in Staging_Optimization_Results.md.
7. Lock before/after camera transforms, FOV, resolution and light direction. Check shadows, normal detail, rock silhouette, suit deformation and rover animation. Run existing desktop/touch/route regression tests. Report missing astronaut as blocked rather than replace it with primitives.
8. Measure batches, draw calls, triangles, CPU/GPU timing and memory on a named physical Android device. Software emulator FPS is diagnostic only. Compare clear/dust/storm at identical views; establish baseline before changing lighting or particle budgets. No mobile-readiness claim until measured.

## Gates

Current approval covers rock/ground/robotic rover acquisition and staging evaluation. Next operator decisions: approve importer/conversion approach and separate Unity staging evaluation; approve specific realistic astronaut archive only after license evidence. Any replacement not already approved remains outside Unity. Logo/texture usage review, animation validation and zero Unity Console errors are required before adopting rover derivatives. No backend, Android, React or authentication changes.
''')
(out/'Stage08D_Continuation_Report.md').write_text('''# Stage08D continuation — 11 October 2026

Completed staging optimization and anonymous-source research. Unity remains unchanged. No account pages visited and no replacement astronaut acquired.

- PASS: nine rock/rover LOD GLBs generated; source SHA256 preservation; glTF validation zero errors.
- PENDING: rover warning review, visual fidelity/articulation, atlas baking, collisions, Unity import and Android measurements.
- BLOCKED: no fully verified realistic rigged astronaut with anonymous acquisition and idle/walk/run found. Publisher evidence does not prove archive or Humanoid compatibility.

Details: No_Login_Astronaut_Candidates.md, Staging_Optimization_Results.md, URP_Staging_Integration_Plan.md. Machine evidence: work/stage08d/Optimized/Optimization_Metrics.json and Validation_Results.json; reproducible scripts/package lock in work/stage08d/tools/. Preservation_Check_2026-10-11.json checks the three recorded Unity baseline files.

Next operator action: supply a licensed realistic rigged EVA archive or approve commissioning; choose importer/conversion and isolated Unity evaluation. No new replacement imports until those decisions. Existing reports describing Sketchfab sign-in as the next step are superseded: this continuation forbids account navigation and does not pursue it.
''')
print('Reports written; source unchanged:',m['sourceUnchanged'],'Unity baselines:',preserved)
