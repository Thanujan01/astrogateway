# Measured graphics status
M4 Pro / Metal / Unity6000.6.4f1, actual batch Editor Play Mode. Stationary spawn, clear weather, full HUD, 1920×1080 framebuffer. Two-second warmup + at least four-second sample per preset. Short capped measurements, not a mobile readiness claim.

| Preset | Mean ms | P95 ms | Max ms | Effective FPS | Target cap |
|---|---:|---:|---:|---:|---:|
| Low | 40.09 |34.13|735.94|24.94|30|
| Medium |33.33|33.93|37.04|30.00|30|
| High |16.67|17.28|20.55|60.00|60|
| Desktop / Ultra Preview |16.67|17.34|19.15|60.01|60|

Low includes a736ms stall; the probe’s broad <2s validity flag is not evidence of a stable30FPS baseline. No improvement claim or sustained thermal benchmark. Main Thread recorder includes capped frame pacing and whole Editor work; it is not isolated CPU compute cost. GPU timing was unavailable to the automated recorder; batch counter for batches was unavailable. Recorded triangle counters include Editor scope, approximately177k/191k/198k/212k, not unique asset totals.

A later native Editor Statistics single snapshot read28.5FPS,35.1ms global frame,5.5ms GPU,181.3k triangles,33SetPass and176draw calls (157instances). This is an observed instant at a different viewport/scale, not an averaged benchmark or valid direct comparison with1920×1080 samples. Native UI automation then failed.

Full/Compact/Hidden HUD samples at Medium:30.13/30.01/30.00FPS, with90/54/3 active graphics. Whole-Editor GC recorder315650/357388/347008 bytes/frame. These are not UI-only allocations and do not prove HUD efficiency. Four-preset total allocated-memory readings306–314MB are Editor managed/native allocation counters, not process RSS or Android memory.

| Control | Low | Medium | High | Desktop/Ultra Preview |
|---|---:|---:|---:|---:|
| Render scale |.65|.8|1|1|
| Shadow resolution |512|1024|2048|4096|
| Shadow distance m |25|45|65|90|
| MSAA samples |1|2|4|4|
| Texture mip limit |2|1|0|0|
| LOD bias |.65|1|1.5|2|
| FPS cap |30|30|60|60|

Existing weather controls use bounded particle counts; no expensive volumetric simulation added. New wheel geometry has explicit three-level LODs. Material derivatives keep instancing where inherited/enabled; no claim of measured batching gain. New roughness-derived maps: three512²RGBA textures; theoretical uncompressed mip-chain budget about4MiB total. Android ASTC6×6 importer overrides configured; actual device format/residency unmeasured.

Raw evidence: Four_Preset_Editor_Performance.txt; Regression/HUD/HUD_Editor_Performance.txt; Unity logs. Android physical FPS, sustained GPU/CPU, thermals and APK memory are UNTESTED.
