# Rover material comparison

**BLOCKED: full material-channel fidelity cannot be certified with this importer path.** WebP→PNG itself is lossless in decoded pixels and preserves source material JSON. Unity's optional material extension support remains incomplete.

Inspected actual Unity camera renders: Rover_Original.png, Rover_LOD0.png, Rover_LOD1.png, Rover_LOD2.png and Rover_Rear.png. LOD0 retains source-reference silhouette, chassis, six wheels, instrument details and overall pale/metallic/dark material placement. No magenta missing-shader areas or obvious entire flipped surfaces observed in these static angles. This is limited visual inspection, not exhaustive transparency/backface/animation testing.

LOD1 has visible front-housing shading/faceting changes and reduced wheel detail; LOD2 has pronounced wheel/front-housing simplification artifacts. These are inherited from previously optimized geometry, not introduced by PNG conversion (indices/normals preserved byte-for-byte). Low LODs should not be accepted at close-view distances; retune thresholds or redo part-aware simplification in a separately scoped iteration.

## Remaining material support gap

Unity warnings: KHR_materials_ior and KHR_materials_specular. Seven LOD material records carry non-default values: transparent(ior1.45/specular0), glass_trans(ior1.39/specularColorTexture index5), glass lens(ior1.39/colored specular factor), blades(ior1.45/specular0), mars_2020_06(ior1.45/specular≈.3091), glass_2(ior1.39/specularColorTexture index5), mars_2020_06_n(ior1.45/specular≈.3091). The named specular texture is retained in GLB but not proof of a rendered channel. No extension was silently deleted to hide warnings.

Source-reference screenshots use the same Unity renderer with the same limitation, so agreement cannot prove correct original glTF glass/specular behavior. Alpha/transmission data retained, but exact glass appearance, IOR and specular response are not accepted. A supported material adaptation/shader route or reference renderer comparison is required before claiming all materials render correctly.

## Measured image differences

Identical1600×1000 camera renders compared to converted original reference. Full-frame mean absolute8-bit RGB channel differences: LOD0=1.155, LOD1=2.004, LOD2=2.752. Channels differing by>10:3.064%,4.586%,6.087%. Includes geometry, shadows, texture resolution and background; **not** isolated conversion error, perceptual-quality score or mobile performance measurement. Pixel equality tests on decoded texture data are the direct conversion-fidelity evidence.

File textures retain original LOD maxima1024/512/256 pixels; no resize in this continuation. Source reference retains original image sizes. Detailed WebP sizes in inventory and Unity-imported sizes in Unity_Asset_Checks.txt. No device FPS claim.
