**Stage08D · 10 October 2026 · Audit and asset research only. Replacement imports and implementation await operator approval.**

# Android optimization preparation

No new build or measurement in Stage08D audit. Stage08C1 software SwiftShader emulator measured about4.27–4.34FPS at2340×1080 and7.84–7.95FPS at1560×720; restored clear sample4.39FPS. Host Vulkan startup crashed; GLES attempts stalled. These do not establish phone performance or30FPS readiness. No physical Android device benchmark is available.

Inspected scene preview URP asset: MSAA2,renderScale0.85,HDRoff,main shadow1024,55mdistance,2cascades,soft shadows off,SRPBatcher enabled. Preserve this baseline; changes require A/B visual captures and timings. Proposed reference acceptance device: Snapdragon778G/Adreno642L-class physical phone,6GBRAM,Android13+,landscape1280×720 output,30FPS (33.3ms frame budget); device availability and target achievement remain unverified.

Evaluate ASTC support on actual target before choosing6×6/8×8; fallback requires supported format, not silent decompression. Keep mipmaps and correct sRGB/linear maps. Convert roughness to smoothness and pack URP metallic/smoothness correctly; Poly Haven ARM channels are not directly the URP metallic map. Use shared rock materials and instancing for static repeated meshes; skinning requires separate evaluation. LOD hero silhouettes deliberately; preserve close EVA/rover fidelity. Prefer one directional shadow light and probes over many real-time lights. Keep weather particle cap240 initially, measure overdraw before changes.

Record average/p95frametime, CPU/GPU where supported, memory,GC,batches/drawcalls, clear/light/storm and10min sustained thermal behavior on the actual device. Previous zero draw-call recorder results were unusable, not proof of zero draws. No performance gain claimed by an asset budget alone.

## Approved acquisition follow-up —10 October2026
Operator approved T1/R1/R2 and V1 acquisition/evaluation, robotic interpretation, and A2 stylized astronaut evaluation. Approved assets are staged outside Unity under work/stage08d/Acquired/. No replacement imported and no scene/source/package changed in this follow-up.

PASS: ground/rock files acquired with2K diffuse/normal/roughness/AO maps. Every downloaded file covered by the initial Poly Haven metadata MD5 checks matched. SHA256 and URLs are recorded in work/stage08d/Acquired/Acquisition_Manifest.json. The selected glTF variants measured Rock07=14844triangles and Rock09=12416triangles, each1mesh/1material/no skin or animation; these differ from whole-listing headline counts and do not imply an imported Unity LOD setup. Bounds show genuinely small stones, not cliff-scale geology. FBX variants acquired for later Unity compatibility evaluation.

PASS acquisition / NOT Android-ready: official NASA repository Perseverance GLB4987176bytes,199521triangles,68meshes,47materials,44images,1skin and23animation objects. Skin/animations belong to rover content, not a humanoid rig. Most parsed embedded textures range64–1024px; some image dimensions remain unidentified by the simple parser. Rights reference NASA repository README and earlier usage review; third-party/protected markings and exact texture inspection remain pending. No Unity glTF importer installed and no Blender found at /Applications/Blender.app. Conversion/importer approach is unresolved; heavy material/drawcall count needs optimization.

BLOCKED: Muko_Art download opens Sketchfab login dialog. Operator sign-in or locally supplied original archive is required; no account created, password requested or licensed-access bypass attempted. Therefore bone hierarchy, Humanoid compatibility, included texture dimensions and idle/walk/run are still unverified. Viewer exposes a mixamo.com animation entry; separate animation provenance/terms must be checked if present in the archive, and animation outputs must stay out of AI training/testing.

NEXT: operator signs in to Sketchfab or supplies local archive path. Then inspect license/readme, skeleton and maps; complete material/marking review and evaluate assets in an isolated Unity copy before replacing visuals in a separate development scene. Current existing controllers and scene remain intact. No before/after improvement or performance test passed.
