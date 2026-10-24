# Stage08C1 final report — 2026-10-10

## Decision
PARTIAL / NOT ANDROID-READY. Software rendering is proven and resolution cost is measured. Hardware-emulator startup fails; physical-device performance and complete manual input verification remain blocked. No30FPS target achievement claimed.

## Results
- PASS: inspected six Stage08C reports, actual final screenshots, runtime/build evidence and current URP/scene/runtime assets. Runtime video report and file were inspected, but frame decoding failed due local Swift/toolchain problems and native viewing was subsequently blocked by Mac lock; no new claim of watching the entire video.
- PASS: actual main Editor Play opened and real-key walking/cardinal route, rover entry/acceleration/coasting steering/braking/exit, weather and HUD were observed. Passive observer never enables testControl or changes transforms. Full input coverage is incomplete.
- FAIL: severe camera occlusion near rover bay captured in an actual main-Editor screenshot.
- BLOCKED: W+D chord, reliably delivered Shift modifier, RMB drag, comprehensive slopes/walls/boundaries and all three targets. Operator-assisted testing requested. Mac lock later stopped further UI-dependent checks.
- PASS: diagnostic Android build/install and four-state focused software profiling after recovering a System UI blocking dialog.
- MEASURED:2340×1080 software baseline4.27–4.34FPS;1560×7207.84–7.95FPS; full-resolution clear reversion4.39FPS. CPU/GPU-reported counters support software rendering as dominant cost.
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

## Follow-up — unlocked Mac, 2026-10-10
Main Editor Play Mode resumed successfully. Real keyboard walking completed all three mission proximity targets; HUD advanced to2/3 and then “Local exploration complete — provisional”. Evidence: Screenshots/Editor_Resume_Power_2of3.png and Screenshots/Editor_Resume_Mission_Complete.png. This supersedes the earlier incomplete-target result. Play Mode was stopped after verification.

Mouse coordinate input still failed with noWindowsAvailable despite the unlocked Mac. Operator-assisted combined-key, orbit and comprehensive boundary/collision checks remain pending; no operator results have been supplied. Prior camera-occlusion failure and Android performance limitations remain unchanged. No project source changes, fresh performance measurements or new Console-clean claim are included.
