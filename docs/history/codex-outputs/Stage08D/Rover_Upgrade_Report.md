**Stage08D · 10 October 2026 · Audit and asset research only. Replacement imports and implementation await operator approval.**

# Rover upgrade preparation

PENDING acquisition/import approval. V1 official NASA Perseverance model is preferred for detailed robotic presentation. It is an uncrewed science rover: it cannot plausibly become a seated astronaut vehicle without new licensed crew-vehicle content. Operator must choose robotic rover remote-operation interpretation for the existing E enter/exit mode, or supply/approve a crew rover model. Do not claim the NASA rover transports people.

Preserve current controller/acceleration/steering/brake/exit clearance/camera/HUD behavior in a new scene. Attach cleared visual mesh under roverVisual; normalize pivots, wheel axes and real model dimensions; reassess collision envelope instead of squeezing model into old box scale. Confirm existing visual-wheel logic cannot rotate an arbitrary imported hierarchy. Wheel animation/suspension may need a visual adapter, while physics remains the provisional kinematic prototype. Use simple dedicated collision meshes, avoid detailed mesh colliders and preserve actor layer exclusions.

NASA glTF acquisition may need external FBX conversion or an approved supported importer; no package installed or converter assumed available. Proposed close rover budget20–50ktriangles with lower LODs and2K/1K maps; exact count/material usage unknown. No model imported, driving regression or performance result for replacement.

## Approved acquisition follow-up —10 October2026
Operator approved T1/R1/R2 and V1 acquisition/evaluation, robotic interpretation, and A2 stylized astronaut evaluation. Approved assets are staged outside Unity under work/stage08d/Acquired/. No replacement imported and no scene/source/package changed in this follow-up.

PASS: ground/rock files acquired with2K diffuse/normal/roughness/AO maps. Every downloaded file covered by the initial Poly Haven metadata MD5 checks matched. SHA256 and URLs are recorded in work/stage08d/Acquired/Acquisition_Manifest.json. The selected glTF variants measured Rock07=14844triangles and Rock09=12416triangles, each1mesh/1material/no skin or animation; these differ from whole-listing headline counts and do not imply an imported Unity LOD setup. Bounds show genuinely small stones, not cliff-scale geology. FBX variants acquired for later Unity compatibility evaluation.

PASS acquisition / NOT Android-ready: official NASA repository Perseverance GLB4987176bytes,199521triangles,68meshes,47materials,44images,1skin and23animation objects. Skin/animations belong to rover content, not a humanoid rig. Most parsed embedded textures range64–1024px; some image dimensions remain unidentified by the simple parser. Rights reference NASA repository README and earlier usage review; third-party/protected markings and exact texture inspection remain pending. No Unity glTF importer installed and no Blender found at /Applications/Blender.app. Conversion/importer approach is unresolved; heavy material/drawcall count needs optimization.

BLOCKED: Muko_Art download opens Sketchfab login dialog. Operator sign-in or locally supplied original archive is required; no account created, password requested or licensed-access bypass attempted. Therefore bone hierarchy, Humanoid compatibility, included texture dimensions and idle/walk/run are still unverified. Viewer exposes a mixamo.com animation entry; separate animation provenance/terms must be checked if present in the archive, and animation outputs must stay out of AI training/testing.

NEXT: operator signs in to Sketchfab or supplies local archive path. Then inspect license/readme, skeleton and maps; complete material/marking review and evaluate assets in an isolated Unity copy before replacing visuals in a separate development scene. Current existing controllers and scene remain intact. No before/after improvement or performance test passed.
