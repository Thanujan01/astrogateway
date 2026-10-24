**AstroGateway: Mission Red Horizon · SK2LB-AstroLab · Stage 02 · 8 October 2026**

Status: proposed blueprint for approval. Documentation only; no Unity implementation authorized by this stage.

Evidence labels: **F** = verified source statement; **E** = proposed engineering concept; **G** = fictional gameplay/art/production assumption. Source IDs resolve in [06_References_And_Attribution.md](06_References_And_Attribution.md). All unlabelled design specifications below are **G**, not NASA measurements or engineering certification.

# Environment asset list

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
