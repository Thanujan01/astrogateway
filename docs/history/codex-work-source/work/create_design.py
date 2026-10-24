from pathlib import Path
root=Path('outputs/Design')
root.mkdir(parents=True,exist_ok=True)
header='**AstroGateway: Mission Red Horizon · SK2LB-AstroLab · Stage 02 · 8 October 2026**\n\nStatus: proposed blueprint for approval. Documentation only; no Unity implementation authorized by this stage.\n\nEvidence labels: **F** = verified source statement; **E** = proposed engineering concept; **G** = fictional gameplay/art/production assumption. Source IDs resolve in [06_References_And_Attribution.md](06_References_And_Attribution.md). All unlabelled design specifications below are **G**, not NASA measurements or engineering certification.\n\n'
docs={}
docs['01_Vision_And_Art_Direction.md']='''# Vision and art direction

## Experience and visual identity

A lone trainee steps from a practical field habitat into a quiet, layered Martian landscape. Equipment is legible, maintenance has consequences, and rock observations guide scientific reasoning. Deliver one 8–12 minute mission with a clear beginning, equipment decision, sample investigation, and return to safety. Favor intentional silhouettes, restrained materials, and readable light over dense decoration.

**F:** Mars has varied brown, gold, and tan surface colors, iron-oxidized dust, a thin mainly carbon-dioxide atmosphere, and a cold environment [S01]. **F:** The Jezero panorama records sedimentary rocks at an ancient river delta [S02]. **F:** Rover observations also identify water-altered igneous rocks on the crater floor [S13]. **G:** Our station occupies an invented site inspired by these settings; it is neither a surveyed reconstruction nor a claim that a human base exists there.

## Palette and material hierarchy

These sRGB hex values are art swatches, not spectrometer data. Review in the final browser color pipeline.

| Role | Swatch | Use |
|---|---|---|
| Dust ochre | #A87450 | Broad soil base; avoid uniform red saturation |
| Sand tan | #C39B70 | Ripples and sheltered deposits |
| Layered stone | #806B58 | Delta ledges; vary value by bedding |
| Dark rock | #494744 | Sparse floor-rock contrast |
| Dusty sky | #C9A687 | Daylight horizon gradient |
| Habitat ceramic | #D8D6C9 | Sun-facing panels and suit separation |
| Engineering graphite | #303A3D | Frames, radiator fins, equipment recesses |
| Instrument cyan | #60C6D0 | Small operational accents and map selection |
| Caution amber | #E7B24C | Caution labels, paired with icon and text |

Aim for roughly 70% earth tones, 25% neutral engineering surfaces, and 5% semantic accents in hero compositions. Mineral identity must come from observation/instrument content, not color alone. No fluorescent red dunes, forests, liquid-water ponds, or random neon structures.

## Art style and quality targets

Use restrained PBR realism with simplified geometry, authored wear, and coherent scale. Dust collects in seams and sheltered bases; do not apply identical noise everywhere. Establish one trim/atlas family for the station, one rock family, and three terrain material roles. Repeat modular assets with purposeful orientation and scale variation. Avoid stretched bedding and visibly mirrored erosion on hero rocks.

Primary composition: pale habitat in the southwest, solar rows in the southeast, layered outcrop in the north. Near-ground pebbles frame the astronaut; middle-distance equipment provides scale; an inexpensive distant ridge provides depth. Main landmarks remain identifiable at 80–120 m. The hero view must survive the low quality profile, grayscale inspection, and a 720p viewport.

## Reference imagery and interpretation

| Reference | What to study | Limit |
|---|---|---|
| [Jezero delta panorama, PIA24921](https://science.nasa.gov/photojournal/detailed-panorama-of-mars-jezero-crater-delta/) [S02] | Layering, broken rock masses, sedimentary context | NASA/JPL-Caltech/ASU/MSSS; caption states color enhanced. Do not treat it as literal human-eye color calibration |
| [Jezero landing-site orbital image](https://science.nasa.gov/resource/jezero-crater-mars-2020s-landing-site/) [S03] | Fan morphology and regional context | NASA/JPL-Caltech/MSSS/JHU-APL; orbital scale does not define game scale |
| [USGS regional geologic map](https://pubs.usgs.gov/publication/sim3464) [S06] | Geologic context and mapping conventions | Publication metadata accessed; full map/PDF retrieval failed in this research pass |
| [Mars sunset explanation](https://science.nasa.gov/solar-system/planets/mars/what-does-a-sunrise-sunset-look-like-on-mars/) [S07] | Dust scattering and localized blue near the Sun | Twilight reference, not an all-day blue sky |

Link references in review boards; no source images or models have been downloaded/imported. Before production, compare the hero view against the panorama at matched eye height; label image processing and record credit when reproducing imagery.

## Lighting and atmosphere

**F:** Fine dust affects sky color; blue becomes prominent near the Sun at twilight [S07]. **F:** Mean orbital solar irradiance is 586.2 W/m² in the fact sheet [S05]; this is not a surface irradiance measurement. **G:** Fixed mid-afternoon lighting: Sun elevation 30°, light from southwest, shadows northeast. Use warm-neutral direct light, a muted dusty sky, readable shadow fill, and restrained aerial perspective. Shadow length for a 4 m object is approximately 6.9 m at this chosen angle; use it as an art consistency check. No numerical conversion from irradiance to Unity light intensity is asserted.

Use a gradient sky and distance fog first. A modest dust event reduces contrast and solar output; it does not become a hurricane. Twilight, moving time of day, volumetric clouds, and full weather simulation are stretch scope. Interiors are restricted to an airlock alcove; emissive practical fixtures supply navigation cues without many real-time lights.

## UI visual language

Compact flight-instrument typography, strong alignment, SI units, labels alongside icons, and clear focus/hover states. Use SK2LB-AstroLab identity rather than NASA branding. Status uses words and shape in addition to color. Provide large-text and reduced-motion options; avoid opaque HUD panels covering rocks or the return route. Numbers labelled “training model” must never look like live NASA telemetry. Detailed HUD and tactical map requirements are in document 05.

## Approved-scope target

Core: exterior station, rock investigation, one power incident, return route. Stretch: greenhouse cutaway, rover motion, sunset. Exclude a complete habitat interior, open-world traversal, imported high-resolution Mars mosaics, and procedurally simulated weather from the seven-day commitment.
'''
docs['02_Mars_World_Layout.md']='''# Mars world layout

## Coordinate contract

Playable bounds: **250 m × 250 m**, X = −125…125 m, Z = −125…125 m. Unity 1 unit = 1 m; +Z is map north, +X east, +Y up. Origin is station survey datum, not a georeferenced Mars coordinate. Ground Y = 0 m around the station. Zone dimensions are axis-aligned reserved footprints including work aprons, not all building footprints. All positions and terrain heights are proposed.

| ID / zone | Center (X,Z), m | Reserved width X × depth Z | Elevation | Purpose / interaction point (X,Z) |
|---|---|---|---|---|
| H Habitat / airlock | (−65,−60) | 36 × 28 m | 0 | Spawn (−45,−60), return terminal (−48,−60) |
| G Greenhouse research module | (−65,−10) | 30 × 22 m | 0…1 | Closed-loop monitor (−47,−10); exterior access only |
| P Power / storage | (25,−45) | 24 × 24 m | 0 | Load-priority console (13,−45) |
| S Solar field | (75,−55) | 48 × 40 m | 0 | Inspection/cleaning rail (53,−55) |
| R Rover bay | (−15,−95) | 32 × 20 m | 0 | Static rover readiness panel (−15,−82) |
| C Science staging | (−5,20) | 24 × 20 m | 1 | Sample logging desk (−5,10) |
| D Delta study terrace | (0,85) | 70 × 40 m | 4…8 | Sample target (0,75), observation point (15,70) |
| B Dark-rock comparison site | (75,35) | 30 × 26 m | 1…2 | Comparison target (65,35) |

The solar reservation is a teaching demonstrator, not a verified power system sized to sustain a crew. The base is staged away from the sampling terrace, but real contamination-control distances require mission-specific assessment.

## Top-down diagram

Diagram is schematic; the coordinate table is authoritative. North is up. Dots represent circulation, not cables.

```text
                 NORTH +Z
      X=-125         X=0           X=125
Z=125 +--------------------------------+
      |     distant ledge silhouette   |
      |          [D DELTA]             |
      |              :                 |
      |          [C SCIENCE]....[B ROCK]|
      |              :          :      |
      | [G GREENHOUSE]:          :      |
      |      :       :          :      |
      | [H HAB]......[P POWER]..[S SOLAR]|
      |   spawn      :                 |
      |         [R ROVER]              |
Z=-125+--------------------------------+
           EAST +X ->
```

## Navigation and mission circulation

Primary learning route: spawn → power → solar → power → science staging → delta → habitat. Waypoints: (−45,−60), (0,−60), (13,−45), (53,−55), (13,−45), (0,0), (−5,10), (0,55), (0,75), (−35,40), (−45,0), (−45,−60). Approximate polyline length 410 m; interaction time makes the full mission 8–12 minutes. Walk speed proposal is 2.5 m/s, so uninterrupted movement is roughly 2.7 minutes. Validate in greybox rather than treating this estimate as final pacing.

Secondary loop: C → B → S → P → C, approximately 260 m, introduces material comparison and offers a clear return without a dead end. Greenhouse and rover are optional 30–60 second exterior stops; keep them out of mandatory scoring in the first slice.

Main paths 4 m clear, equipment aisles 2 m minimum, airlock entry 2 m clear. Reserve 5 m around consoles for camera orbit and accessibility. Low marker posts every 15–20 m on the return corridor. Cable conduits remain protected and outside foot lanes; no mandatory stepping over cables. Informal paths are subtle disturbed dust, not asphalt highways.

## Sightlines and landmarks

From spawn, reveal power mast and delta silhouette before prompting movement. Keep the H→P view corridor clear below 1.2 m; solar rows sit to its east. From D, habitat roof and mast remain visible even when most ground detail is hidden. Habitat mast at (−65,−60) is proposed 8 m high; avoid bright beacons flooding the landscape. Greenhouse is a rounded low module, power has a compact vertical service mast, and delta has horizontal layering: distinct silhouettes without arbitrary decoration.

Use small rock clusters to frame turns; never place a large rock in the H–P corridor or at a console backing space. During dust, posts and the map carry navigation once distant landmarks disappear.

## Terrain elevation strategy

Flat working pad within X −90…105, Z −110…0; local undulations ≤0.5 m. Northward rise from Y 1 at C to Y 4 at Z 55, with a broad approach ramp ≤8°; terrace working surface around Y 4. Layered non-traversable faces reach Y 8–12 near Z 105–120. No sheer drop beside a required interaction. East comparison site uses shallow relief and sparse sand pockets.

Small ripples are material/normal detail first; mesh relief only where silhouettes or collision justify it. Main walking slopes ≤12°, optional slopes ≤20°, no jump-required route. Transition out of bounds with broken ledges, instrument warning, and a clearly marked training limit. A decorative horizon ring outside the playable bounds may be proposed later; do not enlarge the traversable world.

**F:** Jezero contains a delta and a diverse geologic record [S02, S06, S13]. **G:** The nearby compressed exposures and 250 m training arena are compositional conveniences; no exact local stratigraphic section, DEM, or surveyed slope is claimed.

## Greybox review tests

Walk all routes at intended eye/camera heights. Check footprints do not overlap, required consoles are accessible, no critical path leaves bounds, and the farthest required target has a readable return cue. Capture spawn, power, and delta viewpoints in both profiles. A first-time tester should reach power without opening the map and find habitat during dust using the return markers. Revise coordinates here before approving detailed art.
'''
docs['03_Environment_Asset_List.md']='''# Environment asset list

## Procurement and priority policy

P0 is required for the mission; P1 supports polish/learning if capacity permits; P2 is deferred. Effort is focused person-hours for a simple cohesive asset including UVs, materials, LODs, colliders, and review; it excludes gameplay programming and assumes experienced creators using reusable modules. It is an estimate, not a vendor quote. “Team-authored” describes a proposed source, not an already created or verified asset.

| Asset / quantity | Priority | Requirements | Proposed source / rights status | Hours |
|---|---|---|---|---|
| Habitat shell + airlock, 1 kit | P0 | 14 × 8 m shell in H apron; sealed joins, insulation cues, supports, shielded rear berm; no full interior | Team-authored; ownership/provenance pending | 6–8 |
| Solar row, 1 reusable kit / 4–6 rows | P0 | Framed tilted cells, supports, dust variant, cable endpoint; clean aisle | Team-authored; pending | 3–4 |
| Battery/power cabinets + console, 1 kit | P0 | Isolators, guarded connectors, ventilation/radiator cues, readable states | Team-authored; pending | 3–4 |
| Greenhouse shell, 1 | P1 | Opaque insulated cylinder, service hatch, monitoring panel; optional tiny viewing port | Team-authored; concept informed by S09; pending | 3–5 |
| Rover bay canopy + static rover | P1 | Wheel/suspension silhouette, maintenance apron; vehicle is not drivable | Team-authored simple proxy; NASA 3D candidate S04 not selected or cleared | 5–8 |
| Terrain surface / 3 material roles | P0 | Dust, ripple sand, exposed rock; shared masks and tileable normals | Team-authored textures; pending; NASA images are references only | 5–7 |
| Layered outcrop, 3 modules | P0 | Bedding-ledges, broken faces, terrace silhouette; separate simplified colliders | Team-authored from S02 context; pending | 5–7 |
| Dark rock set, 4 meshes + small scatter | P0 | Distinct massive/blocky forms; sparse scatter, not uniform confetti | Team-authored, informed by S13; pending | 3–4 |
| Science console + sample target, 1 kit | P0 | Instrument housing, labelled sample point, sample-case prop | Team-authored; pending | 2–3 |
| Posts, conduits, crates, signs, 1 kit | P0 | Shared atlas; labels visible at interaction distance; footprints clear | Team-authored; font rights pending selection | 2–3 |
| Astronaut, 1 rigged character | P0 | 1.8–2 m silhouette, white/graphite suit, dust wear, visor, pack, joint articulation | Approved rig/asset to be selected; no provider or license verified | 6–10* |
| Core animation set | P0 | Idle, walk, brisk walk, turn, inspect/reach, sample crouch, return to idle | Team-authored or approved clips; no library selected | 4–6* |
| Equipment state motion | P0 | Switch lever, indicator states; do not animate entire solar array | Team-authored; pending | 1–2 |
| Dust VFX + ambience | P0 | One localized particle family, fog transition, suit breathing/radio sounds | Team-authored or separately cleared audio; pending | 3–5 |
| Greenhouse trays / rover motion / sunset | P2 | Interior set, driving animations, time-of-day transition | Deferred | 10+ |

*Character/animation estimates assume a suitable legally cleared rig exists before Day 2. A bespoke character can exceed the sprint; fallback is an approved simple suit rig, not an unlicensed download. P0 subtotal is roughly 43–63 hours; all P1 add another 11–18 hours. Reuse is essential. Do not commit every upper-bound estimate on a single artist's seven-day schedule.

## Astronaut and animation acceptance

Target 18–25k triangles at desktop LOD0, 8–12k mobile; one rig, ≤65 bones, ≤4 weights/vertex, 1–2 materials. Verify full-body third-person visibility, elbows/knees clear the suit, no visor clipping, and pack stays attached. No claim that this fictional suit is flight-ready. Rig scale matches camera and console height (console center approximately 1.1–1.3 m).

Core locomotion clips in place; movement owns translation. Foot contact should match 2.5 m/s walking and 4 m/s brisk movement proposals. Sampling uses a short stationary reach/crouch; if a clip is unavailable, use an inspect pose plus equipment action instead of a poorly blended sequence. Jumping, ladders, ragdolls, and zero-gravity animations are excluded. Equipment actions require one visible transition and one understandable end state.

## Geometry and materials

Use document 04 budgets. Habitat geometry should spend triangles on silhouette and bevels, not invisible fittings. Produce LOD0/1/2 where the screen-size difference matters. Every exterior module needs a simplified collider and consistent pivot. Solar cells are a material pattern rather than individual geometry. Keep greenhouse glass minimal to avoid transparent overdraw. Distant rock backdrops use opaque meshes.

## Approval gate and asset manifest

No external asset is approved by this document. Before importing any candidate, record exact item URL, author, version/hash, license text/date, commercial/game/Web redistribution permissions, attribution, modification rights, third-party textures, and rig/format compatibility. Verify the downloaded file matches that record. NASA catalog availability is not item-level clearance. Credit real rover reference sources without suggesting the fictional rover is NASA equipment.

Keep a per-file provenance ledger in design documentation during procurement. Unclear ownership blocks that asset's import; use original simple geometry until resolved. NASA logos, mission patches, copyrighted third-party media, and identifiable people need separate review under S11. A marketplace “free” price is not license evidence.
'''
docs['04_Unity_URP_Technical_Standards.md']='''# Unity URP technical standards

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
'''
docs['05_Gameplay_Environment_Integration.md']='''# Gameplay and environment integration

## Learning loop and evidence boundaries

Observe → predict → act → compare outcome → explain. Score reasoned choices and safe return rather than speed alone. Every numeric simulation display uses a “training model” label. No telemetry is fetched from NASA, and no real EVA certification or operational safety procedure is implied.

| Zone | STEM learning | Player action / feedback | Scope |
|---|---|---|---|
| H Habitat | Environmental isolation and operational readiness | Read suit checklist, identify airlock as safe destination; return and debrief | Core exterior/alcove |
| P Power storage | Energy vs power and load prioritization | Choose critical load priority; see reserve change and explain trade-off | Core |
| S Solar | Dust, illumination, generation | Inspect a dusted row; compare clean/dirty training outputs | Core |
| C Science staging | Evidence records and sample provenance | Log target, observation, location, and interpretation separately | Core |
| D Delta terrace | Sediment deposition and ancient water | Inspect bedding, collect virtual sample, explain why a delta is relevant | Core |
| B Floor-rock comparison | Geological diversity; color alone is insufficient | Compare texture and provided instrument result with delta sample | Optional |
| G Greenhouse | Controlled environments and resource recycling | Inspect water/air/light flow diagram and a monitor | P1 exterior station |
| R Rover bay | Mobility, maintenance, and energy reserve | Read readiness checklist and reserve consequence | P1 static display |

**F:** Jezero delta sedimentary observations [S02] and floor igneous observations [S13] support different rock stories. **G:** The game provides simplified, authored instrument results; do not present them as actual measurements at these invented coordinates. A sample is evidence of a past environment, not proof that life existed.

## Movement and camera contract

Third-person suit scale 1.8–2 m; proposed walk 2.5 m/s, brisk walk 4 m/s, no required jump. Controller gravity target −3.73 m/s² follows the fact sheet's mean surface gravity [S05]; adhesion, turning, acceleration, and animations remain tuned gameplay approximations. Lower gravity does not justify lunar hopping or fast unencumbered sprinting. Suit mass/biomechanics are not physically simulated.

Camera starts 3.5 m behind and 1.6 m above player pivot, proposed vertical FOV 55°, pitch −20°…55°, orbit sensitivity adjustable. Collision pulls the camera forward smoothly without exposing mesh interiors; line-of-sight to the player takes priority. No head-bob or mandatory camera shake. Desktop mouse/keyboard and a later touch joystick/orbit area must reach the same interactions; reduced motion removes incidental sway.

Consoles use a 1.8 m interaction radius, clear facing prompt, visible highlight, cancel option, and 0.3 second input debounce proposal. Never require pixel hunting on a small label. Interacting stops locomotion but preserves exit and camera control; avoid placing the player in hazardous collision gaps.

## Mission sequence and simplified energy model

1. At H, learn the objectives and acknowledge training-model limits.
2. At P, predict which service should retain power during reduced generation.
3. At S, inspect and clean one row; compare outputs, then confirm allocation at P.
4. At C/D, record a layered-rock observation and select a cautious scientific interpretation.
5. Trigger a narrated dust advisory after sampling; return along marked corridor.
6. At H, review energy consequence, evidence quality, and navigation decision.

Proposed fictional initial values: battery capacity 20 kWh, reserve 12 kWh, solar generation 4 kW clear / 1 kW dusty, essential load 2 kW, research load 1 kW. This is an educational demonstrator, not a crew-base power design. Track energy with E_next = clamp(E + (generation − loads) × elapsed_hours, 0, capacity). Display power in kW and energy in kWh distinctly. A short real-time storm cannot drain this reserve dramatically; if visible change is needed, show a separately labelled one-hour forecast rather than silently accelerating the physics. Cleaning restores only the panel-soiling component; it cannot remove airborne attenuation.

**E:** Architecture uses prefabricated pressure shells, protected service conduits, regolith berm cues, and a shielded shelter. These are illustrative engineering proposals. **E:** A greenhouse may use controlled lighting and shielding; NASA describes terrestrial prototype research rather than an operating Mars greenhouse [S09]. No exact shielding thickness, pressure rating, oxygen production, or autonomous sustainability is certified here.

## Hazards and dust presentation

**F:** Low surface pressure and mostly CO₂ atmosphere [S05] motivate a sealed suit and airlock. **F:** Radiation protection is a mission-design concern [S10]. **F:** Dust affects solar power, while low atmospheric density limits wind force relative to Earth storms [S08]. Cold, energy reserve, visibility, and equipment contamination shape the environment; radiation remains educational context rather than an arbitrary rapid health meter.

**G:** A 90-second local dust episode begins with 10-second warning, ramps for 15 seconds, and holds until return/debrief. Clear-view design target 150 m; event target approximately 35–50 m. These values and timing are authored, not Jezero weather forecasts. Use restrained particles, reduced contrast, lower solar display output, and amber/text advisories. Keep immediate path and posts readable at ≥15 m. No flying habitat wreckage, player knockback, lightning spectacle, or claim that dust storms cause radiation storms. Radiation and dust advisories remain distinct.

Audio prioritizes suit breathing, radio, and equipment cues. Wind-like ambience is a restrained dramatization, not a measured Mars soundscape. Add captions and an independent effects volume; do not rely on sound alone. A reduced-effects mode lowers particles and removes shake without reducing mission information.

## Tactical map design

Use a north-up schematic map built from the layout data, rather than a second real-time world-rendering camera. Panel shows player heading, labelled H/P/S/C/D landmarks, objective, return path, training boundary, and the dust visibility advisory. Coordinates convert with u=(X+125)/250 and v=(Z+125)/250; invert v only for a UI system whose origin is top-left. Calibration test checks all four corners and origin.

Position marker updates at 5 Hz initially; objective/status changes on events. Map scale 250 m across with 50 m grid intervals; current target range shown in meters. Support keyboard focus, 44 CSS-pixel equivalent touch targets, and large text. Cyan selection, amber caution, and explicit words/icons must work without color discrimination. Dust obscures world sightlines, not known map coordinates; accuracy is a fictional station-local navigation system, not GPS on Mars. Optional uncertainty overlays are deferred.

## Engineering decisions and success criteria

Power trade-off: preserve essential loads versus research throughput; greenhouse research may pause, not instantly kill crops. Shielding trade-off: more protection/mass versus visibility and construction effort. Science trade-off: collect a valuable observation versus unnecessary EVA distance. Teach why, not a single magic fix.

First slice passes when a new tester completes all core interactions within 8–12 minutes, can explain kW vs kWh, describes bedding without claiming proof of life, and returns in dust without a hidden-path failure. Incorrect decisions produce clear feedback and a retry; no lethal timer is required. Full atmosphere, crop growth, rover driving, and medical simulations are deferred.
'''
docs['06_References_And_Attribution.md']='''# References and attribution

## Research ledger

Research accessed 8 October 2026. “Verified” means the listed content was accessible, not that the asset is licensed for every use or that SK2LB-AstroLab is endorsed. Pages may change; verify current terms and source captions again at import/release. No source files were imported.

| ID | Source / organization | Supports / access status |
|---|---|---|
| S01 | [Mars Facts](https://science.nasa.gov/mars/facts/) — NASA Science | Surface color diversity, thin atmosphere, cold/dusty setting; page verified. Broad overview, not local weather or all current scientific debates |
| S02 | [Detailed Panorama of Mars’ Jezero Crater Delta, PIA24921](https://science.nasa.gov/photojournal/detailed-panorama-of-mars-jezero-crater-delta/) — NASA/JPL-Caltech/ASU/MSSS | Sedimentary delta context and reference imagery; enhanced-color main view, natural-color Figure A identified in caption; verified |
| S03 | [Jezero Crater, Mars 2020's Landing Site](https://science.nasa.gov/resource/jezero-crater-mars-2020s-landing-site/) — NASA/JPL-Caltech/MSSS/JHU-APL | MRO orbital context image; page and credit verified |
| S04 | [3D Resources](https://science.nasa.gov/3d-resources/) — NASA Science | Model discovery catalog and link to usage guidance; catalog verified, no individual model selected |
| S05 | [Mars Fact Sheet](https://nssdc.gsfc.nasa.gov/planetary/factsheet/marsfact.html) — NASA Goddard/NSSDCA, D. R. Williams | Mean surface gravity 3.73 m/s²; mean-radius pressure 6.36 mb; CO₂ 95.1% by volume; mean orbital irradiance 586.2 W/m²; verified, last update shown 19 May 2025. Values are not local Jezero conditions |
| S06 | [Geologic map of Jezero crater and Nili Planum, SIM 3464](https://pubs.usgs.gov/publication/sim3464) — USGS, Sun & Stack (2020) | Publication/search metadata confirms regional geological mapping; DOI [10.3133/sim3464](https://doi.org/10.3133/sim3464), scale 1:75,000. Direct publication open and [pamphlet](https://pubs.usgs.gov/sim/3464/sim3464_pamphlet.pdf) retrieval failed. No detailed map-unit or DEM claims relied on it |
| S07 | [What Do Sunrises and Sunsets Look Like on Mars?](https://science.nasa.gov/solar-system/planets/mars/what-does-a-sunrise-sunset-look-like-on-mars/) — NASA Science | Dust-dependent sky color and blue near sunset Sun; accessible official search content verified |
| S08 | [The Fact and Fiction of Martian Dust Storms](https://www.nasa.gov/solar-system/the-fact-and-fiction-of-martian-dust-storms/?site=insight) — NASA/Goddard | Low wind-force context, adhered dust, and solar-power challenges; page verified |
| S09 | [Lunar, Martian Greenhouses Designed to Mimic Those on Earth](https://www.nasa.gov/science-research/lunar-martian-greenhouses-designed-to-mimic-those-on-earth/) — NASA Kennedy, 2017 | Terrestrial prototype closed-loop greenhouse research, shielding/lighting concepts; page verified. Not proof of deployment on Mars |
| S10 | [Real Martians: How to Protect Astronauts from Space Radiation on Mars](https://www.nasa.gov/science-research/heliophysics/real-martians-how-to-protect-astronauts-from-space-radiation-on-mars/) — NASA Goddard, 2015 | Radiation hazard and shelter/shielding concepts; page verified. Historical technology discussion, not a current mission safety specification |
| S11 | [NASA Images and Media Usage Guidelines](https://www.nasa.gov/nasa-brand-center/images-and-media/) — NASA | General educational/informational-use guidance, acknowledgement, third-party exceptions, persons and branding constraints; page verified |
| S13 | [Aqueously altered igneous rocks sampled on the floor of Jezero crater, Mars](https://www.usgs.gov/publications/aqueously-altered-igneous-rocks-sampled-floor-jezero-crater-mars) — USGS-hosted paper record, Farley et al., 2022 | Rover evidence for water-altered igneous crater-floor rocks; abstract verified. DOI [10.1126/science.abo2196](https://doi.org/10.1126/science.abo2196) |
| U01 | [Graphics performance and profiling in URP](https://docs.unity.com/en-us/engine/6000.6/manual/analysis/graphics-performance-profiling/in-urp) — Unity 6.6 manual | Feature/platform profiling and optimization-resource directory; page verified |
| U02 | [Web browser compatibility](https://docs.unity.com/en-us/engine/6000.6/manual/platform-specific/webgl/intro/browsercompatibility) — Unity 6.6 manual | Desktop WebGL 2/WebAssembly requirements and selected mobile-browser support; page verified. Support does not guarantee game performance |
| U03 | [SRP Batcher in URP](https://docs.unity.com/en-us/engine/6000.6/manual/analysis/graphics-performance-profiling/in-urp/reduce-draw-calls-urp/srpbatcher) — Unity | Official search entry confirms compatibility-check documentation exists; direct open failed. Installed-version behavior requires implementation-time verification |
| U04 | [Web technical limitations](https://docs.unity.com/en-us/engine/6000.6/manual/platform-specific/webgl/intro/technical-overview) — Unity | Direct open failed; related search surfaced a 6000.7 page, which was not used to assert 6000.6-specific behavior. Check through U02 navigation before relying on thread/filesystem/audio specifics |

## Verified facts versus design inventions

Facts: delta sedimentary context, crater-floor igneous diversity, thin CO₂ atmosphere, mean gravity, dust-dependent lighting, solar-power challenges, and radiation hazard. Engineering concepts: sealed modules, protected utilities, berm/shelter protection cues, controlled-environment greenhouse, and storage-backed solar demonstration. These concepts are plausible illustrations, not validated base designs.

Gameplay assumptions: invented station location, compressed rock-site adjacency, all dimensions and zone coordinates, 30° Sun angle, color swatches, controller behavior, local navigation system, sample instrument results, power values, rapid dust event, UI, scoring, and timing. Performance limits are project targets awaiting browser tests. No asset license or NASA endorsement is implied by scientific referencing.

## Asset and rights status

| Item | Current verification | Import / publication status |
|---|---|---|
| S02 panorama | Caption, ID, credit and processing description verified | Reference link only; reproduction requires credit and check for item-specific restrictions |
| S03 orbital image | Page credit verified | Reference link only; no download/import |
| NASA 3D rover/astronaut candidates | Catalog reviewed; item provenance, rig, texture rights, and complexity unverified | Not approved or imported |
| USGS map/DEM candidates | Publication identified; data file, metadata and reproduction rights not verified | Not approved or imported |
| Team habitat/rock/suit concepts | Proposed original work only; no production files authored | Author/provenance ledger required once created |
| Third-party animation, font, sound, texture packs | No specific assets selected | Unverified; cannot import based on this blueprint |

NASA guidance generally permits educational/informational use of its content with acknowledgement, subject to its guidelines. Third-party material appearing on a NASA page does not inherit that permission. Identifiable people, branding, and promotional uses need separate checks [S11]. The 3D catalog directs users to these guidelines [S04]; do not reduce this to a blanket “public domain” license for all files.

For reproduced panorama imagery use its full stated credit: **NASA/JPL-Caltech/ASU/MSSS**, with title and PIA24921. Orbital-image credit: **NASA/JPL-Caltech/MSSS/JHU-APL**. Retain processing/crop notes and source links. For a reproduced map verify the file's own credit/terms and cite Sun & Stack (2020), SIM 3464. For modified models record original author, source file/version, applicable terms, and modifications such as retopology and textures. Use an educational attribution screen/footer and an accompanying distribution ledger; do not place NASA marks on fictional suits or habitats.

## Research limits

No HiRISE/DEM download, spectral color calibration, local weather model, radiation calculation, greenhouse life-support sizing, detailed habitat engineering, or third-party procurement was performed. Search-accessible publication metadata is distinguished from fully read documents above. A science reviewer should approve learner-facing text; an asset owner should verify rights before import. All citations are primary organizational sources; community forum claims are excluded.
'''
docs['07_Implementation_Roadmap.md']='''# Implementation roadmap

## Capacity and gates

Seven-day plan begins only after blueprint approval; exact sprint dates and remaining hours are unknown. Planning assumption: one environment artist (about 42 focused hours), one Unity developer (42 hours), and a part-time science/QA reviewer (8–12 hours). Roles are staffing assumptions, not authorization to delegate or proof that those people exist. Asset estimates in document 03 exceed this art capacity at their upper end; reuse, simple suit procurement, and cuts below are mandatory. If fewer than seven days remain, prioritize the greybox and one polished route before any P1 zone.

**Gate 0:** Approve this layout and the core power→sample→dust-return loop; settle target devices, artist capacity, and character provenance. Resolve the Unity terms prompt and verify Editor opening before implementation. No stage begins automatically on document completion.

| Stage / day | Intended work after approval | Acceptance criteria / evidence | Main owner |
|---|---|---|---|
| Greybox / Day 1 | Bounds, simple footprints, routes, camera clearance; rough delta silhouette and 3 material proxies | All coordinates/clearances match doc 02; route traversable without jumping; H/P/D silhouettes readable; first Web smoke build loads; screenshots + traversal review | Level designer/developer |
| Environment art / Days 2–3 | P0 habitat kit, power/solar, two rock families, ground materials, simple approved suit; science props | Consistent scale/palette; geology cues distinct; provenance complete for imported files; LOD/collider checks pass; H/P/D hero views coherent | Environment artist |
| Lighting / Day 3 | Fixed Sun, muted sky, fog, minimal bake/probes, shadow tuning | No shadow acne or floating contact; readable suit and consoles in both profiles; low profile retains composition; before/after screenshots | Technical artist |
| Interaction / Days 4–5 | Core mission flow, power forecast, observation/sample logging, return/debrief, tactical map | Learning labels separate observed evidence from inference; energy units correct; each core action shows feedback/retry; map corners/origin verified; 3 fresh testers finish | Developer + science reviewer |
| VFX/audio / Day 5 | One dust advisory/event, restrained particles, ambience/radio, captions and reduced effects | Return posts visible; no hurricane behavior; audio starts correctly after gesture; equivalent information with muted audio/reduced effects; dust and radiation not conflated | Technical artist/developer |
| Optimization / Day 6 | Browser profiling, LOD/culling/material cleanup, compressed build and device matrix | Doc 04 route measured on required devices; targets met or explicit approved downgrade; no crashes/lost context; first/cached load and 10-minute mobile test documented | Developer/QA |
| Acceptance and contingency / Day 7 | Bug fixing, learning-text/credits audit, demo rehearsal, build freeze | Core 8–12 minute slice reliable; no unverified shipped assets; attribution complete; no blocking camera/input issue; final repeatable hosted browser run and known-issues list | Whole team |

Lighting and optimization are checked throughout, not postponed until their named days. Terrain representation and build-load feasibility must be settled at Day 1. Allow at least half of Day 7 for unexpected browser or hosting defects. No new packages are assumed; any later package/asset decision requires its own scoped review.

## Scope cuts and contingency

If a cleared astronaut rig is unavailable at end of Day 1, select a simple original rig solution and remove bespoke animation requirements; adjust the demo promise. If Day 3 art exceeds budget, keep greenhouse/rover as labelled exterior silhouettes or omit their optional interaction. Do not sacrifice the core H–P–S–C–D return route to populate the whole map.

If browser timing fails: follow the ordered reduction plan in document 04, then shorten incidental rock density/view detail. If mobile memory fails, deliver desktop first and explicitly mark mobile as experimental until tested; do not claim parity from the Editor. If hosting compression fails, fix delivery before adding visual features. If science review is unavailable, keep modest cited claims and remove unsupported numerical hazards or life-detection assertions.

P1 additions require all P0 acceptance criteria passing with contingency time left. P2 features—full interiors, driving, procedural terrain/weather, greenhouse growth, dynamic day/night—are outside this sprint's production commitment.

## First recommended implementation milestone

**M1: browser-tested composition greybox.** In a later approved stage, build only the 250 m bounds, eight reserved footprints, primary return corridor, habitat/power/delta silhouettes, rough terrain rise, and a simple camera-scale proxy. Produce three hero screenshots, measured route timing, and one Web load/performance smoke-test record. Use original primitives/proxies; no unapproved imports. Interaction programming is a later milestone unless separately approved.

M1 succeeds when the level is legible at eye height and overview, every footprint and approach fits, no main route is blocked, and browser feasibility is demonstrated. A beautiful still image without a traversable, browser-tested route does not pass.

## Open decisions to resolve at approval

- How many sprint days/person-hours remain, and who owns environment art, coding, science review, and QA?
- Which Windows laptop, iPhone, and Android device define minimum acceptance? Is 60/30 fps acceptable?
- Is the core mission loop and static exterior-only habitat acceptable? Can greenhouse/rover stay optional?
- Is a legally cleared suit rig available, and is a simple stylized fallback acceptable?
- Which hosting origin supports the intended compression/browser delivery? Is the game embedded in an iframe?
- Who will approve learner-facing science wording and asset attribution before release?

No answer is required to complete Stage 02 documentation; answers are required before the dependent implementation decisions are finalized.
'''
for name, body in docs.items():
    (root/name).write_text(header+body)
print(f'Created {len(docs)} documents in {root.resolve()}')
