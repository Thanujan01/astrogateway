# Staging optimization results — 11 October 2026

Completed outside Unity. Source assets remain in `work/stage08d/Acquired/`; derivatives are in `work/stage08d/Optimized/` under this workspace. No playable scene, package, source script or replacement import changed. The three recorded Unity source hashes match their earlier baseline. All acquired-file SHA256 hashes match before/after optimization.

## Measured results

| Asset | Source triangles | LOD | Output triangles | Reduction | Materials | GLB bytes | Maximum texture edge |
|---|---:|---:|---:|---:|---:|---:|---:|
| rock_07 | 14,844 | 0 | 5,936 | 60.0% | 1 | 749,036 | 1024 px |
| rock_07 | 14,844 | 1 | 2,226 | 85.0% | 1 | 240,812 | 512 px |
| rock_07 | 14,844 | 2 | 742 | 95.0% | 1 | 74,260 | 256 px |
| rock_09 | 12,416 | 0 | 4,966 | 60.0% | 1 | 667,300 | 1024 px |
| rock_09 | 12,416 | 1 | 1,862 | 85.0% | 1 | 216,952 | 512 px |
| rock_09 | 12,416 | 2 | 620 | 95.0% | 1 | 66,808 | 256 px |
| Perseverance | 199,521 | 0 | 91,916 | 53.9% | 43 | 7,274,556 | 1024 px |
| Perseverance | 199,521 | 1 | 47,280 | 76.3% | 43 | 3,747,848 | 512 px |
| Perseverance | 199,521 | 2 | 27,088 | 86.4% | 43 | 2,275,888 | 256 px |

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
