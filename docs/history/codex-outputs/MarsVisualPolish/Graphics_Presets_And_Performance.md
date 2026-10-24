# Presets and performance

| Setting | Low | Medium | High | Desktop |
|---|---:|---:|---:|---:|
| Render scale | .65 | .8 | 1 | 1 |
| Main shadow map | 512 | 1024 | 2048 | 4096 |
| Shadow distance m | 25 | 45 | 65 | 90 |
| MSAA samples | 1 | 2 | 4 | 4 |
| Global mip limit | 2 | 1 | 0 | 0 |
| LOD bias | .65 | 1 | 1.5 | 2 |
| Target FPS cap | 30 | 30 | 60 | 60 |
| Dust rate multiplier | .3 | .6 | 1 | 1.3 |
| Rover dust rate per speed | 1 | 2 | 4 | 6 |

Uses the existing cloned URP runtime asset; original pipeline assets/project settings are not edited. Presets are measured configuration changes, not performance guarantees. Maximum existing main dust particles 240 and wheel dust 100 are retained. High/Desktop HDR pipeline flags do not prove the camera has HDR enabled. Desktop is not an Android target.

Editor_Frame_Cadence.txt records the actual batch Play Mode measurement protocol and results when completed. NewSceneTests/Editor_Asset_Metrics.txt is an object/asset snapshot, not drawn-frame triangles, draw calls or GPU allocation. Its final two seam lines query the old source mesh paths and must NOT be interpreted as v2 seam geometry. Unique mesh total is valid for the loaded scene but does not count instanced/drawn geometry.

CPU/GPU timings, batches, thermal behavior, allocation spikes and physical-device FPS are UNMEASURED. No Android device appeared in adb devices -l. No APK rebuild or mobile benchmark was performed. No performance improvement claim follows from geometry counts or Editor cadence.
