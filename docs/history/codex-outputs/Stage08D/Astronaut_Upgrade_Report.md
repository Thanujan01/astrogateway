**Stage08D · 10 October 2026 · Audit and asset research only. Replacement imports and implementation await operator approval.**

# Astronaut upgrade preparation

BLOCKED pending suitable hero asset and import approval. No character replaced. A1/A2 candidate facts and shortcomings are in Licensed_Asset_Shortlist.md. A1 visibly improves suit continuity but does not satisfy premium character acceptance; A2 is explicitly stylized. Do not infer rig validity from a “rigged” tag.

After approval, inspect skeleton, neutral pose, skinned bounds, weight deformation, gloves/boots/visor/backpack, author license and marks. Validate Humanoid Avatar and in-place idle/walk/run before scene replacement. Retain1.85m CharacterController and current movement, interaction/touch/HUD contracts. Current Generic clips animate primitive transform paths and cannot simply be reused on arbitrary Humanoid bones. Introduce a scene-local visual adapter instead of rewriting original MarsPrototype or old scene; retain animator speed parameter, test0/2.2/3.8m/s and transitions. Disable root motion when the controller owns translation. Keep visibility toggling on rover entry, layer8 actor exclusion and camera anchor behavior.

Proposed mobile targets, not measured:15–30k hero triangles,2–3material slots,1–2Kmaps with2K face/suit priority, lower LODs. Do not remove silhouette detail solely to reach these suggestions. No purchased asset, skeleton import, animation export or new test performed.

## Approved acquisition follow-up —10 October2026
Operator approved T1/R1/R2 and V1 acquisition/evaluation, robotic interpretation, and A2 stylized astronaut evaluation. Approved assets are staged outside Unity under work/stage08d/Acquired/. No replacement imported and no scene/source/package changed in this follow-up.

PASS: ground/rock files acquired with2K diffuse/normal/roughness/AO maps. Every downloaded file covered by the initial Poly Haven metadata MD5 checks matched. SHA256 and URLs are recorded in work/stage08d/Acquired/Acquisition_Manifest.json. The selected glTF variants measured Rock07=14844triangles and Rock09=12416triangles, each1mesh/1material/no skin or animation; these differ from whole-listing headline counts and do not imply an imported Unity LOD setup. Bounds show genuinely small stones, not cliff-scale geology. FBX variants acquired for later Unity compatibility evaluation.

PASS acquisition / NOT Android-ready: official NASA repository Perseverance GLB4987176bytes,199521triangles,68meshes,47materials,44images,1skin and23animation objects. Skin/animations belong to rover content, not a humanoid rig. Most parsed embedded textures range64–1024px; some image dimensions remain unidentified by the simple parser. Rights reference NASA repository README and earlier usage review; third-party/protected markings and exact texture inspection remain pending. No Unity glTF importer installed and no Blender found at /Applications/Blender.app. Conversion/importer approach is unresolved; heavy material/drawcall count needs optimization.

BLOCKED: Muko_Art download opens Sketchfab login dialog. Operator sign-in or locally supplied original archive is required; no account created, password requested or licensed-access bypass attempted. Therefore bone hierarchy, Humanoid compatibility, included texture dimensions and idle/walk/run are still unverified. Viewer exposes a mixamo.com animation entry; separate animation provenance/terms must be checked if present in the archive, and animation outputs must stay out of AI training/testing.

NEXT: operator signs in to Sketchfab or supplies local archive path. Then inspect license/readme, skeleton and maps; complete material/marking review and evaluate assets in an isolated Unity copy before replacing visuals in a separate development scene. Current existing controllers and scene remain intact. No before/after improvement or performance test passed.
