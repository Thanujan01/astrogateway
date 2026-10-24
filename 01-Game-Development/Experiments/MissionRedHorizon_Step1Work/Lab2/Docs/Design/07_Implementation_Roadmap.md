**AstroGateway: Mission Red Horizon · SK2LB-AstroLab · Stage 02 · 8 October 2026**

Status: proposed blueprint for approval. Documentation only; no Unity implementation authorized by this stage.

Evidence labels: **F** = verified source statement; **E** = proposed engineering concept; **G** = fictional gameplay/art/production assumption. Source IDs resolve in [06_References_And_Attribution.md](06_References_And_Attribution.md). All unlabelled design specifications below are **G**, not NASA measurements or engineering certification.

# Implementation roadmap

## Capacity and gates

Seven-day plan begins only after blueprint approval; exact sprint dates and remaining hours are unknown. Planning assumption: one environment artist (about 42 focused hours), one Unity developer (42 hours), and a part-time science/QA reviewer (8–12 hours). Roles are staffing assumptions, not authorization to delegate or proof that those people exist. Asset estimates in document 03 exceed this art capacity at their upper end; reuse, simple suit procurement, and cuts below are mandatory. If fewer than seven days remain, prioritize the greybox and one polished route before any P1 zone.

**Gate 0:** Approve this layout and the core power→sample→dust-return loop; settle target devices, artist capacity, and character provenance. The last observed Editor window required Software Terms acceptance; confirm whether it has since been resolved, then verify Editor opening before implementation. No stage begins automatically on document completion.

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
