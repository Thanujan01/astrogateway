**Stage08D · 10 October 2026 · Audit and asset research only. Replacement imports and implementation await operator approval.**

# Environment upgrade preparation

PENDING approval. Existing original meshes and materials remain untouched. Retain250×250m layout, all six documented zones and preserved corridors. Create Assets/AstroGateway/Stage08D/MarsOutpost_Visual_Stage08D.unity only after import approval; preserve original08B and environmentv01.

Propose approved ground/rock assets with authored macro masks, local rubble clusters and varied fractured silhouettes. The delta needs irregular erosion, fractured bedding and talus, not oversized pebble scans. NASA [Jezero delta panorama](https://science.nasa.gov/photojournal/detailed-panorama-of-mars-jezero-crater-delta/) supports layered sedimentary context; distinguish enhanced panorama color from FigureA natural color. Terrestrial scans supply visual detail only, not measured Martian geology.

Keep sparse dark floor-rock contrast, tan/ochre soil, pale equipment and restrained cyan/amber wayfinding from the design. Establish foreground/midground/distant ridge separation. Preserve traversable elevation; detailed geometry stays separate from simple collision. Add purposeful airlock seals, service panels, radiator fins and cable paths using cleared modular content or proper authored meshes; no additional primitive-placeholder pass presented as final art. Habitats and any crew vehicle remain fictional engineering concepts, not NASA-certified designs.

Use afternoon directional light, controlled ambient value and economical horizon haze. First address rover-bay camera obstruction in the separate scene, then compare contact/shadows and suit material response. No HDRI of Earth's sky or biologically patterned ground. No scene edits or tests yet.

## Approved acquisition follow-up —10 October2026
Operator approved T1/R1/R2 and V1 acquisition/evaluation, robotic interpretation, and A2 stylized astronaut evaluation. Approved assets are staged outside Unity under work/stage08d/Acquired/. No replacement imported and no scene/source/package changed in this follow-up.

PASS: ground/rock files acquired with2K diffuse/normal/roughness/AO maps. Every downloaded file covered by the initial Poly Haven metadata MD5 checks matched. SHA256 and URLs are recorded in work/stage08d/Acquired/Acquisition_Manifest.json. The selected glTF variants measured Rock07=14844triangles and Rock09=12416triangles, each1mesh/1material/no skin or animation; these differ from whole-listing headline counts and do not imply an imported Unity LOD setup. Bounds show genuinely small stones, not cliff-scale geology. FBX variants acquired for later Unity compatibility evaluation.

PASS acquisition / NOT Android-ready: official NASA repository Perseverance GLB4987176bytes,199521triangles,68meshes,47materials,44images,1skin and23animation objects. Skin/animations belong to rover content, not a humanoid rig. Most parsed embedded textures range64–1024px; some image dimensions remain unidentified by the simple parser. Rights reference NASA repository README and earlier usage review; third-party/protected markings and exact texture inspection remain pending. No Unity glTF importer installed and no Blender found at /Applications/Blender.app. Conversion/importer approach is unresolved; heavy material/drawcall count needs optimization.

BLOCKED: Muko_Art download opens Sketchfab login dialog. Operator sign-in or locally supplied original archive is required; no account created, password requested or licensed-access bypass attempted. Therefore bone hierarchy, Humanoid compatibility, included texture dimensions and idle/walk/run are still unverified. Viewer exposes a mixamo.com animation entry; separate animation provenance/terms must be checked if present in the archive, and animation outputs must stay out of AI training/testing.

NEXT: operator signs in to Sketchfab or supplies local archive path. Then inspect license/readme, skeleton and maps; complete material/marking review and evaluate assets in an isolated Unity copy before replacing visuals in a separate development scene. Current existing controllers and scene remain intact. No before/after improvement or performance test passed.
