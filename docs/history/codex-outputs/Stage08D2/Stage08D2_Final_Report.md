# Stage08D.2 final report

## Overall: BLOCKED for full material fidelity; local geometry/import repair completed

All three new rover derivatives import, render and reload in the saved isolated scene. The severe lower-LOD wheel and housing artifacts visible in the prior captures are removed in the inspected matched-angle views. Original assets, the prior evaluation scene and the main playable scene remain unchanged by final hash verification.

Seven unsupported IOR/specular materials now have explicit URP Lit specular-workflow adapters. Three of them—glass_trans, glass lens and glass_2—require transmission/refraction that this Lit conversion cannot faithfully reproduce. Their limitation is documented rather than declaring complete fidelity PASS.

Triangles are 91,916 / 91,902 / 91,792, with 68 meshes and 43 materials per imported level. The conservative repair intentionally sacrifices aggressive reduction to preserve important geometry. It does not establish Android suitability. Shared rover texture storage has a format-normalized estimate of 97.38 MiB; no physical-device profiling occurred.

## Outputs

- Material_Fidelity_Audit.md
- URP_Material_Conversion_Report.md
- LOD_Artifact_Repair_Report.md
- Scene_Validation_Report.md
- Before_After_Screenshots/Before_LOD0.png, Before_LOD1.png, Before_LOD2.png
- Before_After_Screenshots/After_LOD0.png, After_LOD1.png, After_LOD2.png, After_Rear.png, Adapted_Source_Reference.png
- LOD_Repair_Evidence.json, Material_Adapter_Evidence.json, Source_Preservation_Checks.json
- Unity_Regression_Checks.txt and Unity_Final_Shared_Materials.log

Scene: /Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/work/stage08d1/VisualEvaluation/Assets/Stage08D2/Scenes/Stage08D2_Rover_Fidelity_Evaluation.unity

Screenshots are actual Unity Editor camera renders, not Play Mode or Android evidence. The adapted source reference shares the adapter where names match; it is not an independent authoritative glTF optical rendering comparison.

## Decisions needed

Accept the documented opaque-glass approximation for this evaluation, or approve a separately scoped transmission material solution. Further mobile LOD reduction should use authored, component-aware topology and be visually reviewed before device profiling. No further implementation or original-scene integration has been performed.
