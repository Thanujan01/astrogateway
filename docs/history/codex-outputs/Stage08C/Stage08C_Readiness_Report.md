# Stage08C readiness — 2026-10-10

## Decision
PARTIAL / NOT MOBILE-READY. Touch implementation and local automated regressions pass. Complete hands-on gameplay verification is BLOCKED by the native input automation tool; physical Android usability/performance remain unverified. The software-rendered emulator is far below a proposed30 FPS target. Do not promote this stage to production or trusted mission evidence.

|Work|Result|
|---|---|
|Actual main Editor E entry/W rover acceleration|PASS, partial baseline controls|
|Complete manual WASD/Shift/RMB/E/routes/collision validation|BLOCKED after unsupported W+D native chord; UI testing stopped|
|Touch controls and mobile HUD|IMPLEMENTED;12 automated checks PASS|
|Stage08B controlled regression|20/20 PASS after correcting test clock; earlier19/20 trial retained|
|Stage07A provisional schema compatibility|PASS, actual8/19-event QA batches|
|ARM64 Android debug APK build/install/launch|PASS on isolated emulator|
|Actual Android terrain/HUD/touch visual rendering|PASS for inspected stationary views;0 unsupported material renderers reported|
|Android performance|MEASURED4.42–4.45 FPS; below target on software renderer|
|Physical-device FPS/thermal/memory/touch|BLOCKED, no connected physical device|
|Original scene/asset preservation|PASS,373 files compared; only intentional existing-source hook changed|
|Backend/Supabase/Android host/React changes|NONE|

## Paths
Unity additions: `/Users/kapilash/Projects/MissionRedHorizon/Assets/AstroGateway/Stage08C/`.
Modified hook: `/Users/kapilash/Projects/MissionRedHorizon/Assets/AstroGateway/Stage08B/MarsPrototype.cs`.
Scene remains `/Users/kapilash/Projects/MissionRedHorizon/Assets/AstroGateway/Stage08B/MarsOutpost_Playable_Stage08B.unity`, byte-identical to pre-stage.
Pre-stage controller backup: `work/stage08c/MarsPrototype.before.cs`. No original scene/material/prefab/package/settings rewrite.

Deliverables in this directory:
- Manual_Gameplay_Verification.md
- Android_Touch_Control_Report.md
- Mobile_Performance_Report.md
- Visual_QA_Report.md
- Regression_Test_Results.md
- Stage08C_Readiness_Report.md
- Screenshots/ (actual Editor renders and Android framebuffer captures)
- Android_Runtime_Preview.mp4 (10.22 s stationary actual runtime preview, not manual gameplay)
- AstroGateway_Stage08C_Probe.apk (55 MB, standalone disposable test package)
- Evidence/ (build/runtime logs, profiler JSON, memory snapshots, test assertions, checksums and preservation proof)

## Security and trust
Runtime telemetry remains provisional, in memory, bounded and offline. No witnessed/authoritative scoring, training-data certification, authentication or user IDs are added. Profiling APK uses a compile-gated diagnostic harness; ordinary builds do not enable it. Build utility refuses main-project execution; settings changes occur only in the copied lab. The Android host/Unity bridge was preserved but not regression-tested by this standalone APK.

## Remaining actions
1. Operator completes real post-change desktop controls: diagonals, running, RMB orbit, steering/braking/exit, walls/slopes/occlusion/bounds and all three mission targets. CUA does not support the attempted simultaneous non-modifier chord. Scripted tests are not substitutes.
2. Connect an authorized physical Android device for actual multi-finger controls, cancellation/resume, cutouts, gesture navigation, landscape ergonomics and integrated Android host lifecycle.
3. Profile CPU/GPU, textures, draw calls/LOD and sustained thermal behavior on hardware. Emulator fallback logs include ASTC decompression and graphics/post-processing limitations; performance cannot establish physical-device readiness. Review nonfatal Play Core AssetPackManager lookup before shipping.
4. Retain placeholder labels. Character feet/rig and rover collision/suspension remain prototype quality. No environment art or backend stage was started.

Testing stopped after the local run and reports. No remote deployment/migration or live accounts.
