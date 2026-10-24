# Stage09A preliminary graphics audit — 2026-10-11

Status: BLOCKED at the asset rendering gate. This is a read-only audit, not a completed Editor validation.

Existing project is Unity 6000.6.4f1 with URP 17.6.0 and Input System 1.20.0. The main manifest does not include glTFast; the isolated evaluation does. Copying its GLB assets or 36 importer-generated shader materials into the main project is not a validated integration workflow.

Serialized PC pipeline: scale 1, MSAA 1, main shadow map 2048, distance 50 m, four cascades, additional-light shadows enabled. Mobile: scale 0.8, MSAA 1, main shadow map 1024, distance 50 m, one cascade, additional-light shadows disabled. These are asset settings, not proof of the active runtime configuration. MarsPrototype.OnEnable selects its previewPipeline; a new quality controller must coordinate with this override and its OnDisable restoration.

Reviewed Stage08D2 final, material and LOD reports, the actual prior LOD0 camera render, Stage08C1 final report and current gameplay source. The image shows preserved wheels/chassis, folded rover configuration and a sparsely composed ground plane. No new Editor render, shader compilation audit or comprehensive asset inventory was executed.

Rover cost indicators: 68 meshes, 43 materials, 91,916 triangles and estimated 97.38 MiB shared RGBA32+mip texture storage. These indicate potential cost; no new profiling establishes the current scene's bottleneck. Prior software-emulator resolution measurements indicate software rendering cost, not a physical Android bottleneck.

The playable scene and controller hashes still match the historical baseline. Preservation_Audit.json records current hashes. No Unity files were changed.
