# URP material conversion

Seven new native Universal Render Pipeline/Lit materials use the specular workflow. The other 36 material definitions retain glTFast-generated shaders. Identical materials are shared across the three scene LODs; the final rover has 43 distinct material references. Source GLB material definitions and textures remain intact.

## Mapping

In linear color space, dielectric normal-incidence reflectance is F0=((IOR−1)/(IOR+1))². It is approximately 0.033736 for IOR 1.45 and 0.026628 for IOR 1.39. Specular factors and colored specular textures modulate that value. Metallic blending uses the source base color as metallic reflectance. Smoothness is 1−roughness and is stored in the derived specular texture alpha.

Let C be source base RGB including its factor, D the factored dielectric reflectance, and m metallic. The baked specular map contains F=D(1−m)+Cm. The derived base RGB is C(1−m)(1−max(D))/(1−max(F)), compensating URP Lit's specular-workflow diffuse energy reduction. This preserves an approximate normal-incidence energy response rather than treating a partially metallic material as a dielectric. Specular-workflow metallic behavior is encoded in the two maps; the inspector metallic scalar is not authoritative for this workflow.

Base texture alpha multiplied by the source alpha factor is retained. Original OPAQUE modes, double-sided rendering, texture coordinates, assignments and normal scale are preserved. The source normal texture is extracted for mars_2020_06_n and imported as a Unity normal map. These seven materials have no metallic-roughness, AO or emission texture maps to silently omit; the adapter checks this assumption. Other materials retain their importer texture assignments.

Original base color data is preserved in the GLBs. The native Lit base map is a derived energy-compensated representation, not a byte-identical base-color texture. Base/specular color PNGs use sRGB imports; normal maps use the normal importer. Smoothness alpha remains linear. Textures were not further resized or compressed for this fidelity evaluation.

## Unavoidable differences and blockers

- Eight-bit baked maps introduce quantization; the diffuse compensation is an approximation.
- URP Lit's angular reflectance and glTF's extension combination are not guaranteed identical.
- glass_trans, glass lens and glass_2 have transmission=1; standard Lit conversion does not reproduce their refraction/transmission. They retain OPAQUE alpha semantics rather than silently changing alpha to fake glass.
- Optional extension warnings remain on GLB imports because original extension metadata is intentionally preserved. Scene material overrides address the supported reflectance approximation, not importer extension support.

A faithful transmission shader or explicit acceptance of these documented glass differences is needed before full material-fidelity sign-off. No unapproved shader package was installed.

## References

[Unity URP Lit documentation](https://docs.unity3d.com/6000.0/Documentation/Manual/urp/lit-shader.html) describes the specular workflow. [Khronos KHR_materials_specular](https://github.com/KhronosGroup/glTF/blob/main/extensions/2.0/Khronos/KHR_materials_specular/README.md) defines the source extension. The local URP BRDF implementation was also inspected for diffuse energy handling.

Implementation: work/stage08d/tools/material_adapter.mjs and isolated Assets/Editor/Stage08D2Repair.cs. Material_Adapter_Evidence.json records source properties, output maps and conversion assumptions.
