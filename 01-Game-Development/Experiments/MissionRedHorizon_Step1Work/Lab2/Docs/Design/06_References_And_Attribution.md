**AstroGateway: Mission Red Horizon · SK2LB-AstroLab · Stage 02 · 8 October 2026**

Status: proposed blueprint for approval. Documentation only; no Unity implementation authorized by this stage.

Evidence labels: **F** = verified source statement; **E** = proposed engineering concept; **G** = fictional gameplay/art/production assumption. Source IDs resolve in [06_References_And_Attribution.md](06_References_And_Attribution.md). All unlabelled design specifications below are **G**, not NASA measurements or engineering certification.

# References and attribution

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
