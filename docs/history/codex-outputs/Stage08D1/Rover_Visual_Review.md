# Rover review

**BLOCKED: rover did not produce a usable Unity prefab.** No actual rendered rover fidelity, skin deformation, wheel articulation, material appearance, NASA markings or scale acceptance test passed.

Source: 199,521 triangles, 47 materials, 68 meshes, one mechanical skin, 23 animation objects. Optimized LODs: 91,916 / 47,280 / 27,088 triangles, each 43 materials. Metadata inspection confirms required WebP extension in optimized derivatives, the critical importer failure.

Khronos validator warnings from Stage08D remain 36 / 36 / 35. Warning classes include runtime-generated tangent space and a skinned mesh node below a parent transform; glTF validity does not guarantee matching Unity transforms. Need normal-map/tangent comparison and rest-pose/skinned bounds validation after texture import succeeds. No automatic joining of animated nodes performed. Count of retained clips is not proof of playable mechanical animation.

Official NASA repository README permits use of model resources and points to usage guidelines; NASA identifiers cannot imply endorsement. No publishing or brand approval was inferred. Rights source: https://github.com/nasa/NASA-3D-Resources and https://www.nasa.gov/nasa-brand-center/images-and-media/ . This approved model is an uncrewed robotic rover, not astronaut transport.

Stop issue: WebP conversion required; original reference also needs optional material-extension review. Physical performance and material draw-call consolidation untested.
