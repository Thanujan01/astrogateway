# Measured reversible experiment

## Single-factor framebuffer test
Same diagnostic APK, copied AVD, SwiftShader renderer, stationary spawn view, renderScale0.85, shadows/MSAA/weather/gameplay unchanged. Changed only disposable Android display override from2340×1080 to1560×720 using `adb shell wm size 720x1560` (portrait setting; player uses landscape). This reduces output pixels by55.6%. Warmup5s, four25s windows, no video or main-Editor Play/build concurrently. Fresh disposable app data prevented stale metrics. Actual dimensions stored in every JSON.

|State|2340×1080 FPS|1560×720 FPS|Lower-resolution p95 ms|
|---|---:|---:|---:|
|clear|4.32|7.84|150.0|
|light_dust|4.33|7.94|133.3|
|storm|4.34|7.95|133.3|
|storm_particles_disabled|4.27|7.92|133.3|


Clear reported software GPU timing fell from231.59 to127.44ms; render thread216.72 to120.76ms. Clear FPS improved approximately81.4%. After `wm size reset` and fresh cold launch, clear at2340×1080 returned to **4.39 FPS** across25.05s. This A/B/A supports a resolution-related rendering cost. Single trials have no confidence interval; effects under physical GPU or release build are not established.

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
