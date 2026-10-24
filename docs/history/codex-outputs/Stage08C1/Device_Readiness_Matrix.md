# Device readiness matrix

## Defined target class and acceptance
Proposed reference class: physical ARM64 Android12+ handset,6GB RAM minimum, hardware OpenGL ES3.1 or supported Vulkan GPU,60Hz display, landscape. Measure with a fixed internal scene resolution no higher than1280×720 and native readable HUD. This defines a configuration target, not a guarantee that every6GB handset has equivalent performance. Select and record an actual manufacturer/model/SoC before acceptance.

Target:30FPS sustained across a15-minute walking/driving/weather run, with33.3ms nominal frame budget. Record average/median/p95/p99/max, frames over50ms, CPU main/render and valid GPU timing, PSS/RSS/Unity memory, GC allocations/stalls, thermal status and battery/power mode. Proposed gate: average at least29.5FPS under a30FPS cap, p95≤40ms, no recurring stalls, crashes, ANRs or touch failures; operator must approve final thresholds and reference model. Not achieved or tested on a physical phone.

|Environment|Rendering|Actual result|Readiness|
|---|---|---|---|
|Main Unity Editor, M4 Pro|Metal hardware|Actual Game View, uncapped545.76 updates/s; CPU/GPU counters collected|Desktop prototype smoke only; not phone FPS|
|Android16/API36 ARM64 emulator2340×1080|SwiftShader software|4.27–4.34 loop FPS; reverted clear4.39|Below target, no Android hardware claim|
|Same emulator1560×720|SwiftShader software|7.84–7.95 loop FPS|Below target; reversible diagnostic only|
|Host-GPU emulator / M4 Pro|Vulkan device detected|SIGSEGV UnityGfxDeviceW on startup|FAIL; no valid FPS|
|Host-GPU emulator forced GLES|Host GLES advertised3.0|GLES31/30 attempts did not initialize a measured player|BLOCKED compatibility|
|Physical Android phone|None connected|No authorized physical serial in initial ADB list|BLOCKED all performance/touch/thermal acceptance|

## Constraints
Emulator CPU virtualization is hardware accelerated via Hypervisor.Framework. SwiftShader still uses CPU software rasterization. Guest thermal temperatures are virtual; M4 host GPU performance would still not model a midrange phone. Development instrumentation and Editor updates are not release presentation measurements. No validated batches/draw calls or physical GPU trace yet. Authentication/Unity-as-a-Library host was preserved but not exercised by the standalone probe.

## Next operator actions
Unlock the Mac and focus the main Game View; complete operator-assisted diagonals, physical Shift running, RMB/camera occlusion, varied slopes/walls/boundaries and all three targets. Connect an authorized phone, identify its model/SoC/Android version, and permit only this disposable test APK. Run sustained target-class profiling and integrated-host touch/lifecycle regression before visual upgrades or Android-ready claims.
