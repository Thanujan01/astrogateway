# Android emulator performance — 2026-10-10

## Measured environment
Authorized emulator-5554, sdk_gphone64_arm64, Android16/API36, ARM64. Copied isolated AVD,4 virtual CPU cores,1536 MiB configured RAM; SwiftShader/software graphics. Actual scene rendered at2340×1080 landscape. Standalone development IL2CPP ARM64 APK, package com.sk2lb.astrogateway.stage08cprobe, Unity6000.6.4f1/URP17.6.0. This does not measure a physical GPU, thermal behavior, Android host integration or final release performance.

Profiler harness warms5 real seconds, then samples25 seconds each in a stationary camera/character setup. FPS=Update count/actual elapsed seconds; p95/max use Time.unscaledDeltaTime. It is Unity player-loop frame rate, not an independently measured compositor/presented FPS. targetFrameRate60 is requested, not achieved. The final run had no concurrent video recording. Framebuffer snapshots occur after each sample, with2-second transitions. ADB memory snapshots are taken when each sample completes. Unity allocated memory is a sampled peak; Android PSS/RSS are snapshots, not peaks or complete GPU memory.

|Condition|Avg FPS|p95 ms|max ms|Peak Unity allocation MiB|PSS snapshot MiB|Peak particles|
|---|---:|---:|---:|---:|---:|---:|
|clear|4.45|233.3|450.0|91.3|291.8|0|
|light_dust|4.43|233.3|450.0|91.4|298.7|43|
|storm|4.42|233.3|250.0|91.4|298.8|195|
|storm_particles_disabled|4.45|233.3|450.0|91.4|299.0|0|

## Assessment
FAIL for a proposed30 FPS target on this emulator; physical-device readiness BLOCKED/unmeasured. All states averaged approximately4.4 FPS. Do not extrapolate this software renderer to physical Android hardware. Storm particles195 of240 maximum; disabled comparison0. The small on/off difference from single sequential samples cannot establish causal GPU particle cost. No repeated trials/confidence interval or GPU profiler trace exists. The common scene/software-render cost dominates this observed workload.

Terrain/materials visually rendered without pink surfaces in inspected captures. The probe found0 renderers referencing unsupported shaders. This does not validate every off-camera asset or prove all URP internal shaders are supported.

## Errors and limitations
Final app process completed all four states; no fatal crash or ANR marker found in the captured app-specific log. This is a short smoke window, not a comprehensive stability test. Startup logs include nonfatal ClassNotFoundException for Play Core AssetPackManager, fallback from attempted Vulkan/ES3.2, ASTC texture decompression, light-cookie format fallback and stripped/unsupported post-processing shaders. Post-processing is disabled in the prototype. These messages remain unresolved compatibility/packaging risks; no clean Android Console claim.

First trial screenshot writes failed because Android prepends persistentDataPath; corrected probe uses relative filenames. Final screenshots succeeded with no screenshot-write failures. First trial logs/live capture retain the error overlay as diagnostic evidence. An Android run-as external-storage read restriction was handled by reading only this app's non-sensitive profiler files directly via ADB. Unity's Editor exit restarted ADB; reconnect restored authorization without dependency installs.

Next: real-device touch/usability and GPU/CPU profiling; sustained30 FPS acceptance, memory/thermal soak, texture compression choice, geometry/LOD/draw calls and shader stripping/asset-pack startup review. No production settings, environment assets or Android host sources were changed for these experiments.
