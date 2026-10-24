# Android renderer diagnosis — 2026-10-10

## Confirmed configurations
Stage08C reports, all six report files, final screenshots, APK and runtime evidence were inspected. Actual Unity log identifies ANGLE over Vulkan **SwiftShader Device (LLVM 10.0.0)**. Graphics are software rendered. Hypervisor.Framework acceleration is available for virtual CPU execution; it does not imply GPU hardware acceleration.

Stage08C1 copied AVD: `work/stage08c/EmulatorAVD`, Android16/API36 ARM64, 4 configured CPU cores. Original AVD, Android host and Unity scenes are preserved. Application is the standalone disposable `com.sk2lb.astrogateway.stage08cprobe`, not the authenticated host. AVD configuration is not a physical device specification. `dumpsys meminfo` reported about2.53 GB total guest RAM; Stage08C's configured1536 MiB figure should not be treated as measured usable RAM.

## Host GPU experiment
Existing emulator `-help-gpu` explicitly supports `host`. Launching the same APK with `-gpu host` detected Apple M4 Pro via Vulkan/MoltenVK. Unity reported Vulkan vendor Apple/device type1. **FAIL:** startup SIGSEGV in UnityGfxDeviceW, fault0x40, process3773, before performance output. See Host_Startup_Logcat.txt.

Supported installed UnityPlayerGameActivity source accepts the intent extra `unity`. Cold launch with `--es unity -force-gles31`, followed by `-force-gles30`, forced OpenGL ES rather than Vulkan. **BLOCKED:** no timing output; initialization stopped around ES3.1 context request. Emulator advertises ES3.0 and rejects ES3.2/3.1 contexts. GLES30_App_Logcat.txt retains actual last messages; do not call this successful hardware rendering or assign FPS. No project graphics API settings were changed.

## Software recovery
First counter-build software attempt showed an actual System UI non-response dialog, losing application focus. Screenshot Software_Startup.png. `dumpsys activity lastanr` returned no registered ANR despite the visible dialog; both pieces of evidence are retained. Guest memory status was normal; memory pressure is not proven as its cause. A clean emulator reboot cleared the dialog (Emulator_After_Reboot.png). The subsequent focused player completed all four weather samples. No measurements from the blocked startup were used.

## Diagnosis
The successful focused run reports main CPU work around20–23 ms versus software-renderer GPU timing around230 ms. Software rasterization/rendering is the strongest measured bottleneck in this setup. These timing counters are platform-reported, not physical Android GPU timings. A reversible resolution comparison is documented separately. Terrain, UI, shadows and opaque rendering remain common costs; the single particle on/off trial does not establish a causal particle cost.

No physical Android device was returned by the fresh authorized ADB inventory. Hardware-phone performance and sustained thermal readiness remain BLOCKED.
