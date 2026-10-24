**AstroGateway: Mission Red Horizon · SK2LB-AstroLab · Stage 02 · 8 October 2026**

Status: proposed blueprint for approval. Documentation only; no Unity implementation authorized by this stage.

Evidence labels: **F** = verified source statement; **E** = proposed engineering concept; **G** = fictional gameplay/art/production assumption. Source IDs resolve in [06_References_And_Attribution.md](06_References_And_Attribution.md). All unlabelled design specifications below are **G**, not NASA measurements or engineering certification.

# Unity URP technical standards

## Baseline and interpretation

Read-only inspection: Unity project version is 6000.6.4f1; manifest contains URP 17.6.0. Two default quality assets reference Mobile_RPAsset and PC_RPAsset; WebGL defaults to quality index 0 in the template. Stage 01's GUI opening validation remains pending at Editor Software Terms. No settings have been changed in Stage 02.

All numerical budgets below are **G** proposed starting limits, not measured results or Unity guarantees. Unity's URP documentation recommends profiling features, scene contents, and target platforms [U01]. Its browser compatibility reference supports WebGL 2/WebAssembly desktop browsers and selected mobile browsers [U02]. Validate the installed release's actual renderer on hardware before adopting optional features.

## Proposed quality profiles

Both profiles use one Universal Renderer in Forward mode, opaque PBR materials, fixed Sun, and no custom renderer features for the first slice. Do not depend on WebGPU, compute-based VFX, GPU occlusion, or a particular upscaler without a separate platform test.

| Setting / budget | Desktop first | Mobile secondary |
|---|---|---|
| Acceptance resolution / fps | 1920×1080 CSS viewport, render cap 1920×1080; 60 fps | Landscape render cap 1280×720; 30 fps |
| Render scale | 1.0 start | 0.75–1.0 after readability test |
| HDR / antialiasing | HDR off initially; MSAA 2× | HDR off; MSAA off or 2× if affordable |
| Main-light shadows | 2048 atlas, 2 cascades, 50 m distance | 1024 atlas, 1 cascade, 25 m distance |
| Additional lights | Baked/emissive by default; ≤2 unshadowed visible | Baked/emissive; 0 real-time additional |
| SSAO / bloom / depth effects | SSAO off initially; subtle bloom only after budget pass | Off |
| Visible triangle budget | ≤350k peak including character | ≤150k peak |
| Draw submissions, including shadow passes | ≤180 peak | ≤100 peak |
| Texture GPU-residency planning limit | ≤192 MiB | ≤96 MiB |
| Total observed application memory planning limit | ≤512 MiB | ≤256 MiB; browser overhead tracked separately |
| Compressed initial payload goal | ≤60 MiB | ≤35 MiB with alternate reduced-content build if needed |
| Visible dust particles | ≤250 | ≤80 |

These are review thresholds, not hard cross-device limits. Do not double-count render scale and resolution caps incorrectly; report actual render dimensions and DPR. If mobile cannot meet its memory/payload target with shared content, reduce asset quality or ship a separate build; do not promise automatic desktop/mobile texture residency reduction from quality settings alone.

## Materials, textures, and batching

URP Lit/Simple Lit for principal materials; Unlit for instruments/signs where appropriate. Linear color workflow proposed. Albedo in sRGB; normals/masks in linear import mode. Terrain starts with three layers and a single pass where supported; review Terrain shader cost against a modest chunked mesh alternative during greybox. No terrain holes, tessellation, parallax, or high-frequency displacement in the core slice.

Shared environment atlases: 1024² default; up to two 2048² hero sheets desktop, downsampled for mobile builds. Small props 256–512² shared; suit 2048² desktop / 1024² mobile. Mipmaps for 3D world textures, anisotropy 2–4 for ground, no readable CPU copies unless justified. Pack compatible scalar masks; disable unused shader keywords. A 2048² RGBA8 texture with mipmaps is about 21.3 MiB uncompressed, so budget from actual imported residency, not file size. Compression must be tested for target browsers; select supported texture formats and fallback behavior using Unity's Web texture documentation linked from U02.

Use SRP-compatible shaders/material organization [U03]. SRP Batcher reduces CPU setup work; it does not automatically merge every draw into one. Evaluate instancing for repeated rock/solar meshes rather than assuming it combines with all batching paths. Avoid creating unique materials for each prop. Static batching trades memory for submissions; measure before accepting it.

## Lighting and terrain

One fixed directional Sun. Baked indirect/static lighting where practical, reflection probe budget 1–2, and light probes for the moving astronaut. Start with one 1024² lightmap atlas; allow a second only after memory review. Maintain coherent exposure; no full-screen lens grime or excessive bloom. Match lighting in document 01. Bias/tuning must avoid detached shadows and acne at ground eye height. During dust, reduce main-light contrast consistently with fog, instead of overlaying opaque dust sheets.

## LOD and collision

Rocks: LOD0 within close inspection, LOD1 ~50% triangles, LOD2 ~15–25%; initial screen-height thresholds 0.25/0.10/0.03, then tune per silhouette. Large habitat keeps silhouette at lower LOD; tiny bolts can disappear. Cull small pebbles before hero landmarks. Avoid alpha cross-fades initially on mobile; assess popping from required camera views. Screen thresholds are starting values, not fixed distance promises.

Terrain collider or simplified ground mesh; primitive/compound colliders for equipment; low-detail static mesh colliders for ledges. No non-convex dynamic mesh colliders. Separate collision from visual pebbles; camera collision must see habitat, big rocks, and consoles without snagging on decorative cables. Terrain seams cannot generate steps on a required route. Use proposed layers for Player, Ground, StaticObstacle, Interaction, and CameraObstacle during implementation review; no layer changes now.

## Web build and hosting considerations

Select Web explicitly during implementation and verify the correct URP asset for both selectable quality profiles. Default Web quality must not accidentally use an expensive desktop profile. Browser capability is separate from acceptable performance [U02]. Test current Safari and Chrome on the M4 Pro plus a Windows integrated-GPU laptop, iPhone Safari, and mid-range Android Chrome; exact devices/versions remain an owner decision.

Serve builds over HTTP(S), test compression response headers and MIME types, first load and cache reload. Start audio only after a user gesture; test pointer lock exit/re-entry, fullscreen refusal, resizing, touch input, orientation change, and tab suspension. Do not rely on local file launches, native plugins, or desktop filesystem access. Threads and cross-origin isolation are deferred pending hosting capability and current technical-limit documentation; no server configuration is authorized now. U04's direct page could not be retrieved, so confirm its details before implementing.

## Performance validation plan

Capture development-build profiler data plus browser memory/network observations, then confirm release-build timing. Measure the same 90-second route: spawn → power/solar → delta overview → dust return. Warm up for 30 seconds, repeat three times; record device, browser, viewport, actual render size, quality, build hash, triangles, submissions, frame time, memory, payload, and load time. Record p50/p95, not only an average FPS.

Desktop target p95 ≤16.7 ms; mobile ≤33.3 ms after warmup, with no recurrent >100 ms hitches. Suggested load goal ≤15 seconds on an explicitly throttled 20 Mbps connection; bandwidth, transfer size, compilation, and cache state must be reported. Run mobile for 10 minutes to reveal thermal throttling and memory pressure. A crash, lost context, or unreadable interaction fails acceptance even at target FPS.

If over budget: remove expensive full-screen effects and transparent coverage, reduce shadow distance, cut small-object density, downsample textures, lower render dimensions, then reconsider geometry. Change one major factor per comparison. Keep images of the same hero viewpoints to detect quality regressions. No profiling/build/setting mutation was performed in this documentation stage.
