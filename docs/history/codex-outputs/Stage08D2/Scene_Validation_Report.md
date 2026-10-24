# Scene validation

## Executed results

| Check | Result | Evidence |
|---|---|---|
| Final Unity batch execution | PASS, exit 0 | Unity_Final_Shared_Materials.log; STAGE08D2_BUILD_AND_RELOAD_PASS |
| LOD0/1/2 import | PASS | Unity_Regression_Checks.txt; 68 renderers per level |
| Shader support / material references | PASS | Supported shaders, no null rover material slots |
| Saved scene reopen | PASS | Three valid LOD renderer reference lists after reload |
| Matched static geometry inspection | PASS within captured views | Before/After_LOD0/1/2.png |
| Physical transmission fidelity | BLOCKED | Three transmission materials remain Lit approximations |
| Source preservation | PASS | 40 final SHA-256 checks unchanged |
| Physical Android performance | UNTESTED | No device profiling performed |

The new scene is /Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/work/stage08d1/VisualEvaluation/Assets/Stage08D2/Scenes/Stage08D2_Rover_Fidelity_Evaluation.unity. Existing Stage08D1 scene was opened as a starting point and preserved; only the new Stage08D2 scene was saved. The main project's playable scene, controller and package manifest hashes remain unchanged. Original acquisitions and the Stage08D11 LOD0 source also match their recorded hashes.

LODGroup screen-relative thresholds are 0.5, 0.25 and 0.08. Captures force the chosen LOD for comparison and restore automatic selection. Automatic transitions were not evaluated through hands-on gameplay. Eight 1600×1000 images are actual batch Unity Editor Camera.Render readbacks with three warm-up renders; they are not native-window, Play Mode or Android screenshots.

The first helper run failed with a MissingComponentException while trying to force an LODGroup on a source-reference object without that component. An explicit Unity null check fixed this tool issue. Both subsequent runs succeeded. Historical log files are retained; the final run is authoritative. Optional IOR/specular importer warnings and inherited glTF tangent/skinning warnings remain; this report does not claim the whole isolated project's historical Console is empty.

## Resource estimates

Final rover references 43 shared materials and 33 unique texture objects across all three levels. The summed RGBA32-plus-full-mip estimate is 102,105,087 bytes (97.38 MiB), using width×height×4×4/3 per texture. It is a conservative format-normalized estimate, not measured resident GPU memory; actual RGB24/ARGB32, mip presence, normal packing, compression, source readability and driver overhead differ. Dimensions and formats are listed in Unity_Regression_Checks.txt. This excludes scene ground, rocks, render targets and other assets. Mesh memory was not profiled.

Sharing identical materials/textures removes duplicate scene references; no FPS, frame-time or device-memory improvement is asserted. Textures remain uncompressed for comparison.
