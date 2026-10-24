**AstroGateway: Mission Red Horizon · SK2LB-AstroLab · Stage 02 · 8 October 2026**

Status: proposed blueprint for approval. Documentation only; no Unity implementation authorized by this stage.

Evidence labels: **F** = verified source statement; **E** = proposed engineering concept; **G** = fictional gameplay/art/production assumption. Source IDs resolve in [06_References_And_Attribution.md](06_References_And_Attribution.md). All unlabelled design specifications below are **G**, not NASA measurements or engineering certification.

# Gameplay and environment integration

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
