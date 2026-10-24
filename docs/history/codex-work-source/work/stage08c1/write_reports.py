from pathlib import Path
import json
B=Path('outputs/Stage08C1');rows=json.loads((B/'Evidence/Baseline_Summary.json').read_text());ed=json.loads((B/'Evidence/Editor_Performance.json').read_text())
def write(n,s):(B/n).write_text(s)
write('Android_Renderer_Diagnosis.md',"""# Android renderer diagnosis — 2026-10-10

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
""")
table='|Condition|Loop FPS|p95 frame ms|CPU main ms|CPU render ms|Reported software GPU ms|SetPass|Triangles|Peak allocation MiB|PSS MiB|Particles|\n|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|\n'
for r in rows:table+=f"|{r['condition']}|{r['fps']:.2f}|{r['p95_ms']:.1f}|{r['main_work_ms']:.2f}|{r['render_thread_ms']:.2f}|{r['reported_software_gpu_ms']:.2f}|{r['setpass']:.0f}|{r['triangles']:.0f}|{r['unity_peak_mib']:.1f}|{r['pss_mib']:.1f}|{r['particles']}|\n"
write('Unity_Profiler_Baseline.md',f"""# Unity profiler baseline

## Method and validity
Android diagnostic APK: Unity6000.6.4f1, URP17.6.0, Development IL2CPP ARM64. Original scene/runtime source preserved. Counter instrumentation exists only in the copied lab and compile-gated profiling APK. ProfilerRecorder enumerates available metrics, records raw units/sample counts, and disposes recorders. Frame Timing Stats enabled only in the copied lab build. Warmup5 real seconds,25 seconds per stationary condition,2-second transitions. Framebuffer captures and PSS snapshots follow each window. No concurrent recording, builds or main-Editor Play during the successful Android run.

FPS is Unity Update count divided by actual elapsed seconds. p95 is Time.unscaledDeltaTime. These are not independently verified compositor presentation rates. Main Thread marker includes waits; CPU Main Thread Frame Time and CPU Render Thread Frame Time have different semantics. Do not add CPU/GPU durations into a serial frame budget. GPU counter under SwiftShader represents software-renderer reporting, not a phone GPU.

{table}

Stage08C previously measured4.42–4.45 FPS without these extra counters. Current4.27–4.34 FPS is the instrumented baseline; do not claim a gameplay regression solely from different diagnostic overhead.

## Main Editor comparison
Actual main project, Metal / Apple M4 Pro, active Game View. Passive Editor-only observer captured {ed['frames']} player-loop frames across {ed['seconds']:.3f}s: **{ed['fps']:.2f} updates/s**, not monitor presentation FPS. CPU/GPU counters and exact raw samples: Evidence/Editor_Performance.json. Earlier concurrent-lab trial is separately retained and excluded from the preferred comparison. The repeat had no active lab build/Play, though the idle Android emulator remained open. Editor memory includes editor/imported assets and cannot be compared directly with player PSS. Neither uncapped Editor update rate nor Metal timings qualify a phone.

## Render statistics and terrain
Scene inspection:272 serialized MeshRenderers,75 LODGroups,1 ParticleSystem, **0 Unity Terrain components**. Ground is a procedural mesh:10,201 vertices /60,000 indices /20,000 triangles. The reported whole-frame triangle count includes rendering passes and other meshes; it is not unique scene geometry or terrain-only timing.

Batches Count, Draw Calls Count and Vertices Count returned zero despite visible geometry and nonzero triangles/SetPass. These are **unusable counters in this capture**, not evidence of zero draw calls. Render Thread marker was unavailable. Frame Debugger's12 event groups seen initially are not a complete draw-call count. Reliable batches/draw calls and terrain-only GPU attribution remain BLOCKED pending usable profiler capture on the target graphics path.

URP scene-owned profile: renderScale0.85, MSAA2, HDRoff, main shadow map1024, shadowDistance55m,2 cascades; post processing disabled by camera. Default scene materials render without pink surfaces in inspected captures; probe unsupported renderer count0. Texture inventory includes1024px macro terrain map,512px material maps,256px normal. ASTC6x6 unsupported on this emulator and decompressed at runtime; disk PNG size is not resident GPU memory. Cookie-format fallback and unused post-processing shader messages remain compatibility issues.

## Memory and GC
PSS values are post-window snapshots, not peaks. Unity allocation peak is not full process/GPU memory. GC allocation counter averages are in Baseline_Summary.json, approximately several KB/frame including diagnostics and prototype HUD/IMGUI. GC.Collect returned zero timing in Android windows; cumulative GC.CollectionCount fields include startup and are **not per-window collection counts**. No zero-GC or leak-free claim. Editor GC and allocations include Editor activity.

Virtual thermalservice reports status0 and synthetic battery/skin temperatures around30°C. No physical thermal, throttling or battery measurement exists. Android gfxinfo covers window buffers, not a validated Unity presentation-FPS baseline.

References: [Unity6 ProfilerRecorder](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Unity.Profiling.ProfilerRecorder.html), [FrameTimingManager](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/FrameTimingManager.html). Installed Editor/Activity source used for version-specific behavior.
""")
write('Gameplay_Input_Verification.md',"""# Actual gameplay input verification

Actual main Unity Editor project/scene was used. No scripted teleport, testControl or virtual keyboard substitutes were used for these manual-input observations. The passive Editor-only trace records testControl=false throughout. Native automation delivers real keyboard events; this is not a human usability test. Scripted results are listed separately.

|Control/check|Result|Actual evidence and limits|
|---|---|---|
|W forward|PASS|Terrain landmarks moved; astronaut advanced with visible animation. Editor_Walk.png and passive trace|
|A/D/S cardinal movement|PARTIAL PASS|Individual inputs delivered; D/S route reached rover at approximately(-14.3,-91.2). Full direction/slope coverage not claimed|
|W+D diagonal|BLOCKED|Prior tool rejected two non-modifier keys; operator assistance requested this turn|
|Shift running|BLOCKED / not demonstrated|Shift+W accepted by tool but on-foot observed max approximately2.205 m/s, not3.8. Screenshot name Editor_Run identifies attempted test only. Could be modifier delivery; do not call the game mechanic failed or passed without operator input|
|RMB camera orbit|BLOCKED|Coordinate clicks returned noWindowsAvailable; no reliable held RMB drag|
|E rover entry|PASS|HUD switched to driving, astronaut hidden, passive driving transition true|
|Acceleration|PASS|W moved rover; HUD9.4km/h captured; passive maximum3.8m/s|
|Steering|PARTIAL PASS|D during coasting changed travel heading and x-position. Sustained W+D steering remains blocked|
|Space braking|PASS, limited|W then Space sequence stopped rover; speed0 in screenshot/trace. Quantitative braking distance not measured|
|E exit|PASS|Stopped rover exited, astronaut reappeared, trace driving=false|
|Camera occlusion|FAIL observed near rover bay|Large dark surface obscured most scene during entry/driving. Editor_Rover_Acceleration_Occlusion.png. Later exit returned clear view. Cause needs collider/camera investigation|
|Terrain slopes/wall collisions|BLOCKED comprehensive check|Ground traversal to rover observed; varied slopes and explicit wall collision sequence not completed|
|Terrain boundaries|BLOCKED|Not manually exercised at each edge|
|Weather1/2/3 and HUD|PASS limited visual|Actual main Editor light dust/storm/clear changed visibility and HUD; oxygen/power updated|
|Three mission targets|PARTIAL|HUD1/3 at habitat. Power/delta traversal not completed in these checks|

## Reproduction and operator checklist
Open Assets/AstroGateway/Stage08B/MarsOutpost_Playable_Stage08B.unity in the main project, enter Play, ensure Pause and Frame Debugger disabled, focus Game View. Test W+D against W for normalized speed; hold physical Left Shift+W and compare2.2 versus3.8m/s. Hold RMB and move mouse in both axes. Start rover near(-15,-93); enter with E, drive/steer/brake, test moving-exit rejection, stop and exit. Inspect rover-bay obstruction and camera against walls/slopes. Walk to targets(-45,-60),(25,-45),(0,75); confirm HUD3/3. Exercise±119m edges without passing through geometry.

Report each result with actual input and observation. Requested operator-assisted checks have no supplied results at report time and remain BLOCKED. No unexecuted check is marked passed.

## Controlled regressions
20/20 gameplay assertions and12/12 synthetic touch/layout checks passed in the isolated lab after diagnostic changes. Test-window Console0 errors/0 warnings. Full log retains known QuickSearch startup exception. These tests are not manual-control evidence. TouchChecks wrote its new output into the previous Stage08C evidence path; the freshly produced file (17:11:22) was copied into this stage and the routing limitation is retained. Stage07 provisional event behavior preserved; no witnessed scores or backend changes.
""")
low=[json.loads(p.read_text()) for p in sorted((B/'Resolution_Trial/Evidence').glob('Android_Profile_*.json'))]
rev=json.loads((B/'Resolution_Reversion/Evidence/Android_Profile_0.json').read_text())
opt='|State|2340×1080 FPS|1560×720 FPS|Lower-resolution p95 ms|\n|---|---:|---:|---:|\n'
for a,b in zip(rows,low):opt+=f"|{a['condition']}|{a['fps']:.2f}|{b['average_fps']:.2f}|{b['p95_frame_ms']:.1f}|\n"
write('Performance_Optimization_Results.md',f"""# Measured reversible experiment

## Single-factor framebuffer test
Same diagnostic APK, copied AVD, SwiftShader renderer, stationary spawn view, renderScale0.85, shadows/MSAA/weather/gameplay unchanged. Changed only disposable Android display override from2340×1080 to1560×720 using `adb shell wm size 720x1560` (portrait setting; player uses landscape). This reduces output pixels by55.6%. Warmup5s, four25s windows, no video or main-Editor Play/build concurrently. Fresh disposable app data prevented stale metrics. Actual dimensions stored in every JSON.

{opt}

Clear reported software GPU timing fell from231.59 to127.44ms; render thread216.72 to120.76ms. Clear FPS improved approximately81.4%. After `wm size reset` and fresh cold launch, clear at2340×1080 returned to **{rev['average_fps']:.2f} FPS** across{rev['seconds']:.2f}s. This A/B/A supports a resolution-related rendering cost. Single trials have no confidence interval; effects under physical GPU or release build are not established.

This is a **diagnostic emulator display experiment**, not an adopted game optimization or achieved Android readiness. Both resolutions remain below30FPS. No Unity scenes, pipeline assets, materials, textures, gameplay or project settings were modified. Original resolution restored. No visual feature removed permanently.

## Particles
Full-resolution storm4.34FPS versus storm-particles-disabled4.27FPS; lower-resolution7.95 versus7.92FPS. The disabled comparison retains storm fog. This single sequential comparison provides no demonstrated particle-removal benefit. Do not infer negative particle cost, remove weather or convert these differences into precise GPU particle timings.

## Candidate next changes, not applied
1. On a physical target, evaluate internal720p rendering or scene-owned renderScale adjustments while preserving readable native HUD. Render-scale-only changes require a new measured A/B; display override changes HUD dimensions too.
2. Check ASTC support and compare supported compression formats; do not change every texture based on an emulator fallback.
3. Capture usable batches/draw-call data before changing mesh/LOD batching; ground is one20k-triangle mesh and many props already have LOD/static combined meshes.
4. Measure HUD/IMGUI allocations and a10Hz text refresh before adoption. Diagnostic steady samples suggest roughly3.4KB/frame but do not identify allocation stacks or isolate HUD cost.
5. Fix rover-bay camera obstruction separately from rendering performance. No visual-art upgrade undertaken.

## Regression/visual scope
Counter-instrumented lab:20 gameplay and12 touch regression checks PASS, test-window Console0/0. Runtime gameplay/touch source unchanged, so the regression validates the diagnostic build's source rather than a nonexistent game optimization. Android screenshots at both resolutions show mission HUD/touch controls and weather, unsupported-material renderer count0. Human multitouch input was not tested; responsive screenshots and synthetic touch checks do not establish usability. Remaining manual checks are listed in Gameplay_Input_Verification.md.
""")
write('Device_Readiness_Matrix.md',"""# Device readiness matrix

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
""")
write('Stage08C1_Final_Report.md',f"""# Stage08C1 final report — 2026-10-10

## Decision
PARTIAL / NOT ANDROID-READY. Software rendering is proven and resolution cost is measured. Hardware-emulator startup fails; physical-device performance and complete manual input verification remain blocked. No30FPS target achievement claimed.

## Results
- PASS: inspected six Stage08C reports, actual final screenshots, runtime/build evidence and current URP/scene/runtime assets. Runtime video report and file were inspected, but frame decoding failed due local Swift/toolchain problems and native viewing was subsequently blocked by Mac lock; no new claim of watching the entire video.
- PASS: actual main Editor Play opened and real-key walking/cardinal route, rover entry/acceleration/coasting steering/braking/exit, weather and HUD were observed. Passive observer never enables testControl or changes transforms. Full input coverage is incomplete.
- FAIL: severe camera occlusion near rover bay captured in an actual main-Editor screenshot.
- BLOCKED: W+D chord, reliably delivered Shift modifier, RMB drag, comprehensive slopes/walls/boundaries and all three targets. Operator-assisted testing requested. Mac lock later stopped further UI-dependent checks.
- PASS: diagnostic Android build/install and four-state focused software profiling after recovering a System UI blocking dialog.
- MEASURED:2340×1080 software baseline4.27–4.34FPS;1560×7207.84–7.95FPS; full-resolution clear reversion{rev['average_fps']:.2f}FPS. CPU/GPU-reported counters support software rendering as dominant cost.
- FAIL: host GPU Vulkan startup SIGSEGV; GLES alternatives yielded no valid timing. Exact logs retained.
- PASS:20/20 controlled gameplay checks,12/12 synthetic touch checks, two Stage07A provisional telemetry batches validated; no transmission or authoritative witness claims. Test-window Console0 errors/0 warnings; startup QuickSearch exception retained in full lab log.
- BLOCKED: physical Android/mobile performance, real touch ergonomics, sustained thermal/memory soak, independent presented FPS and usable batches/draw-call counters.

## Files and preservation
Main project additions only:
`/Users/kapilash/Projects/MissionRedHorizon/Assets/AstroGateway/Stage08C1/Editor/EditorPerformanceObserver.cs`
`/Users/kapilash/Projects/MissionRedHorizon/Assets/AstroGateway/Stage08C1/Editor/ManualInputObserver.cs`
Unity-generated metadata accompanies them. Both are Editor-only, explicit request-file gated, locally recorded diagnostic observers. They do not ship in players. Request flags removed when testing stops.

Existing383 project files compared against pre-stage hashes: no changed or missing files at preservation check. Original scene, all assets, gameplay and saved settings preserved. Lab-only AndroidProfileProbe/AndroidProbeBuild/Stage08BBuilder edits and backups live in `work/stage08c1`; Android settings changes were limited to the copied lab and disposable emulator display override. No Android-host, React, backend, AI security or Supabase changes. No credentials accessed by observers.

APK: `outputs/Stage08C1/AstroGateway_Stage08C1_Diagnostic.apk`. Not a replacement for the integrated host or release app. Package matches the previously isolated profiling probe.

## Evidence and deliverables
All six requested documents are in this directory. Evidence includes raw counter JSON with units/sample counts, memory/thermal snapshots, device properties, host crash/stall logs, build/regression logs, actual manual input trace, preservation hashes and telemetry compatibility results. `Screenshots/` contains actual Editor/framebuffer captures, including failure views. `Resolution_Trial/` and `Resolution_Reversion/` contain fresh measurements/captures for the reversible display experiment.

## Prioritized follow-up
1. Unlock Mac; operator completes unresolved physical keyboard/mouse checks and reproduces rover-bay obstruction. Diagnose spawn/collider/camera handling before camera polish.
2. Authorized physical reference handset:30FPS target, CPU/GPU timing, usable render counters, sustained15-minute weather/driving/thermal and real touch validation.
3. Only adopt internal-resolution/HUD/texture/shadow changes after physical-device measured comparisons; no visual upgrade started here.

Stopped after local diagnosis/verification. No remote or production actions.
""")
