**AstroGateway: Mission Red Horizon · SK2LB-AstroLab · Stage 02 · 8 October 2026**

Status: proposed blueprint for approval. Documentation only; no Unity implementation authorized by this stage.

Evidence labels: **F** = verified source statement; **E** = proposed engineering concept; **G** = fictional gameplay/art/production assumption. Source IDs resolve in [06_References_And_Attribution.md](06_References_And_Attribution.md). All unlabelled design specifications below are **G**, not NASA measurements or engineering certification.

# Vision and art direction

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
