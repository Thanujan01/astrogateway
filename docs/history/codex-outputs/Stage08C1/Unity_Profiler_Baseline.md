# Unity profiler baseline

## Method and validity
Android diagnostic APK: Unity6000.6.4f1, URP17.6.0, Development IL2CPP ARM64. Original scene/runtime source preserved. Counter instrumentation exists only in the copied lab and compile-gated profiling APK. ProfilerRecorder enumerates available metrics, records raw units/sample counts, and disposes recorders. Frame Timing Stats enabled only in the copied lab build. Warmup5 real seconds,25 seconds per stationary condition,2-second transitions. Framebuffer captures and PSS snapshots follow each window. No concurrent recording, builds or main-Editor Play during the successful Android run.

FPS is Unity Update count divided by actual elapsed seconds. p95 is Time.unscaledDeltaTime. These are not independently verified compositor presentation rates. Main Thread marker includes waits; CPU Main Thread Frame Time and CPU Render Thread Frame Time have different semantics. Do not add CPU/GPU durations into a serial frame budget. GPU counter under SwiftShader represents software-renderer reporting, not a phone GPU.

|Condition|Loop FPS|p95 frame ms|CPU main ms|CPU render ms|Reported software GPU ms|SetPass|Triangles|Peak allocation MiB|PSS MiB|Particles|
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
|clear|4.32|250.0|20.51|216.72|231.59|26|100903|91.4|290.3|0|
|light_dust|4.33|233.3|22.96|218.19|230.68|27|100987|91.5|305.0|42|
|storm|4.34|233.3|18.36|223.04|230.42|27|101272|91.5|295.8|195|
|storm_particles_disabled|4.27|250.0|18.65|221.75|233.67|26|100913|91.5|296.2|0|


Stage08C previously measured4.42–4.45 FPS without these extra counters. Current4.27–4.34 FPS is the instrumented baseline; do not claim a gameplay regression solely from different diagnostic overhead.

## Main Editor comparison
Actual main project, Metal / Apple M4 Pro, active Game View. Passive Editor-only observer captured 13644 player-loop frames across 25.000s: **545.76 updates/s**, not monitor presentation FPS. CPU/GPU counters and exact raw samples: Evidence/Editor_Performance.json. Earlier concurrent-lab trial is separately retained and excluded from the preferred comparison. The repeat had no active lab build/Play, though the idle Android emulator remained open. Editor memory includes editor/imported assets and cannot be compared directly with player PSS. Neither uncapped Editor update rate nor Metal timings qualify a phone.

## Render statistics and terrain
Scene inspection:272 serialized MeshRenderers,75 LODGroups,1 ParticleSystem, **0 Unity Terrain components**. Ground is a procedural mesh:10,201 vertices /60,000 indices /20,000 triangles. The reported whole-frame triangle count includes rendering passes and other meshes; it is not unique scene geometry or terrain-only timing.

Batches Count, Draw Calls Count and Vertices Count returned zero despite visible geometry and nonzero triangles/SetPass. These are **unusable counters in this capture**, not evidence of zero draw calls. Render Thread marker was unavailable. Frame Debugger's12 event groups seen initially are not a complete draw-call count. Reliable batches/draw calls and terrain-only GPU attribution remain BLOCKED pending usable profiler capture on the target graphics path.

URP scene-owned profile: renderScale0.85, MSAA2, HDRoff, main shadow map1024, shadowDistance55m,2 cascades; post processing disabled by camera. Default scene materials render without pink surfaces in inspected captures; probe unsupported renderer count0. Texture inventory includes1024px macro terrain map,512px material maps,256px normal. ASTC6x6 unsupported on this emulator and decompressed at runtime; disk PNG size is not resident GPU memory. Cookie-format fallback and unused post-processing shader messages remain compatibility issues.

## Memory and GC
PSS values are post-window snapshots, not peaks. Unity allocation peak is not full process/GPU memory. GC allocation counter averages are in Baseline_Summary.json, approximately several KB/frame including diagnostics and prototype HUD/IMGUI. GC.Collect returned zero timing in Android windows; cumulative GC.CollectionCount fields include startup and are **not per-window collection counts**. No zero-GC or leak-free claim. Editor GC and allocations include Editor activity.

Virtual thermalservice reports status0 and synthetic battery/skin temperatures around30°C. No physical thermal, throttling or battery measurement exists. Android gfxinfo covers window buffers, not a validated Unity presentation-FPS baseline.

References: [Unity6 ProfilerRecorder](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Unity.Profiling.ProfilerRecorder.html), [FrameTimingManager](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/FrameTimingManager.html). Installed Editor/Activity source used for version-specific behavior.
