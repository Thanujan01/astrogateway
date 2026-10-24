# Graphics and performance status

|Preset|Scale|Main shadow map|Distance m|MSAA|Mip limit|LOD bias|Requested FPS cap|
|---|---:|---:|---:|---:|---:|---:|---:|
|Low|0.65|512|25|1|2|0.65|30|
|Medium|0.8|1024|45|2|1|1|30|
|High|1|2048|65|4|0|1.5|60|
|Ultra|1|4096|90|4|0|2|60|

Configuration readback and runtime selection assertions passed. Low disables gameplay post-processing; higher settings enable the restrained grade. Dust density scales with quality and existing weather; fixed particle ceilings remain. Rock meshes have real LOD groups. Texture imports use compressed 1024 limits with Android ASTC overrides. This is configuration evidence, not an Android build or GPU-residency test.

Editor_Asset_Metrics.txt is a source/object snapshot: 16 unique meshes, 98,964 unique-asset triangles, 302 renderers, 22 materials, 9 Texture2Ds and roughly 6.74MiB reported Texture2D runtime-object allocation. It is not total memory, draw calls, visible per-frame triangles or physical GPU memory. First structural seam attempt 313,920 triangles; selected version 47,088. Triangle savings do not establish FPS improvement.

FPS, CPU/GPU frame time, actual batches/draw calls, thermal/sustained behavior and per-preset frame-rate benchmarks are UNMEASURED. ADB lists no devices; Android performance UNTESTED. Ultra is a desktop/Editor preview, not an Android target achievement.

A wall-clock movement check was sensitive to rendered/simulated frame cadence. Existing controller caps gameplay delta at 0.05s. Tests now verify displacement over recorded simulation time; the cause of the earlier real-time delay was not profiled. Do not treat batch-test frame cadence as normal user performance.

Next profiling: warm shader caches, use identical camera/weather/route per quality, collect median/p95 frame times and CPU/GPU markers where supported on the main Editor, then profile an identified physical handset. Review renderer/material/LOD costs and texture residency based on actual counters. No 30FPS readiness claim.
