# Rover repair status

PARTIAL: three NEW transparent material derivatives in isolated Assets/PreMeeting/Materials. glass_trans, glass_lens and glass_2 clones use URP Lit alpha blend, alpha 0.28, transparent queue 3000 and no depth writes. Base/specular maps, normal assignments and all source materials are retained; source alpha semantics intentionally differ in these clearly labeled approximation derivatives. 36 renderer slots across three LODs mapped. Scene import/save/reload and real camera capture passed.

This is alpha transparency, not physically faithful transmission/refraction, thickness, absorption or lens optics. Sorting/overlapping transparent surfaces and close-up glass need further review. The new preview does not resolve full Stage08D2 material-fidelity sign-off.

Kept accepted repaired geometry in the demo: 91,916 / 91,902 / 91,792 triangles and 43 source material definitions. Candidate derivatives at work/premeeting/LOD_Candidates: 88,274 / 86,362 triangles, preserving exact bytes of 30 protected wheel/chassis/equipment meshes, validator zero errors. Savings only 3.96% / 6.04%; not meaningful mobile reductions. Candidate non-protected geometry/UV quality has not been visually inspected, so candidates were not imported. No source overwritten. Original NASA rover is robotic, RTG-powered, supplied in folded/rest pose; no crew vehicle or solar-array claim.

Next: authored component-level LOD reduction, all-angle UV/silhouette review, dedicated transparent optical tests and physical-device measurements. Texture storage prior estimate remains 97.38 MiB RGBA32+mips, not measured GPU usage.
