# Object inventory — Mars_VisualUpgrade_v1

Source: parsed from `Assets/AstroGateway/VisualUpgrade/Scenes/Mars_VisualUpgrade_v1.unity` at HEAD `e3c67bf` (read-only, no Unity needed).
782 GameObjects, 1 inactive, 0 missing scripts. Runtime-only objects (HUD panels, dust particles spawned at runtime) are covered in UI_Component_Inventory.md.

Status this pass is about *this* pass: "VERIFIED UNCHANGED" means not modified and still covered by the regression run.

## Category assessment

| Category | Objects | Purpose | Current quality | Proposed upgrade | Risk | Priority | Verification | Status |
|---|---|---|---|---|---|---|---|---|
| Rocks / far terrain | 269 | Approved rock_07/09 clusters, 12 distant massifs, 36 pebble chunks | rock_07/09 acceptable; massifs low-detail and same material; pebbles fine | Massif silhouette variety + material tint; terrain-contact darkening on rock bases | Medium (asset/material) | P3 | Captures 01/04, draw-call count | VERIFIED UNCHANGED (this pass) |
| Research station | 131 | 5 scan targets (ResearchTarget) + procedural instruments | Procedural tripod/sensor/panel/dish; status light only feedback | Scan-state light colours, readable label plate, scan pulse VFX | Medium (scene) | P2 | Capture 05, scan checks 5/5 | VERIFIED UNCHANGED (this pass) |
| Habitat | 113 | 3 geodesic domes, sealed airlocks, foundations, lights | Readable silhouettes; panels flat-shaded, little surface detail; airlock lights good | Panel normal/roughness variation, decals, door detail; colliders untouched | Medium-High (scene/asset) | P3 | Captures 04, airlock camera checks | VERIFIED UNCHANGED (this pass) |
| Rover | 99 | Drivable kinematic rover + procedural detail | Box chassis, primitive wheels with LODs, procedural cockpit; rover dish shows the same striped artifact | Needs final rover model; interim: dish material, panel bevels | High (asset) | P1 - BLOCKED on asset | Captures 03/07 | BLOCKED |
| Astronaut | 77 | Player character (CharacterController) + procedural suit | Procedural placeholder: capsule limbs, no gloves/fingers, no rig; reads as toy figure in close-ups | Needs design-team rigged EVA suit (spec in plan); keep controller | High (asset/architecture) | P1 - BLOCKED on asset | Captures 02/06 | BLOCKED |
| Solar farm | 46 | Zone C panel rows | Photovoltaic material ok; frames basic | Frame bevel + cell normal detail | Low-Medium (material) | P4 | Capture 01 | VERIFIED UNCHANGED (this pass) |
| Communications | 14 | Mast, truss, reflector, guy wires, beacon | Dish renders with a harsh black-and-white striped artifact in captures 06/07/09 (cause not yet diagnosed: texture vs shadow acne); truss ok | Replace dish material with clean white/grey PBR; check guy wire ends at ground | Low-Medium (material) | P2 | Captures 06/07 dish close-up | VERIFIED UNCHANGED (this pass) |
| Props | 9 | Tanks, crates, power cabinet | Primitive-based, acceptable at distance | Bevels + decals | Low | P4 | Capture 04 | VERIFIED UNCHANGED (this pass) |
| Systems | 6 | Gameplay, presentation, adapters | Not visual | None | - | - | 47 checks | VERIFIED UNCHANGED |
| UI | 5 | Runtime HUD canvas + EventSystem | See UI_Component_Inventory.md | See UI plan | Low (scene-only scripts) | P1 | HUD audit + captures | PARTIAL (batch 1 applied) |
| Light | 4 | Sun + 3 airlock floods + 2 headlights, reflection probe | Coherent warm sun, soft shadows | Minor: sun shadow distance review | High if project-wide | P4 | Captures | VERIFIED UNCHANGED |
| Terrain | 3 | Walkable 250 m ground + far dunes/dust plain | Good at mid range (MarsTerrain shader, 2-scale regolith); repetition visible at low angles; far plain flat | Detail-normal variation, macro tint break-up, crater rims in far terrain | Medium (shader/material) | P3 | Captures 01/08, FPS probe | VERIFIED UNCHANGED (this pass) |
| Other | 3 | Scene roots / containers / reflection probe | - | - | - | - | - | VERIFIED UNCHANGED |
| VFX | 2 | Bounded dust + wheel dust | Subtle, bounded | Scan pulse VFX (new) | Low | P3 | Captures | VERIFIED UNCHANGED |
| Camera | 1 | Follow camera + scenic overview | Obstruction handling fixed in v1 | None now | - | - | Airlock camera checks | VERIFIED UNCHANGED |

Performance note: 673 MeshRenderers, 97 LODGroups, 5 non-shadow spot lights + 1 shadowed sun. Draw calls / SetPass were **not measured** yet (Phase 12 item).

## Full checklist (every GameObject)

| # | Category | Path | Active | Components | Materials | Collider |
|---|---|---|---|---|---|---|
| 1 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype` | yes | CharacterController |  | CharacterController |
| 2 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig` | yes | Animator |  |  |
| 3 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/Backpack` | yes | (renderer off), MeshRenderer | Inherited_17 |  |
| 4 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/ChestPanel` | yes | (renderer off), MeshRenderer | Inherited_7 |  |
| 5 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/DarkVisor` | yes | (renderer off), MeshRenderer | Inherited_17 |  |
| 6 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/Helmet` | yes | (renderer off), MeshRenderer | Inherited_12 |  |
| 7 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/LeftArm` | yes |  |  |  |
| 8 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/LeftArm/SuitSegment` | yes | (renderer off), MeshRenderer | Inherited_12 |  |
| 9 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/LeftArm/Visual Upgrade arm (temporary)` | yes |  |  |  |
| 10 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/LeftArm/Visual Upgrade arm (temporary)/Commander stripe` | yes | MeshRenderer | VU_Accent_orange |  |
| 11 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/LeftArm/Visual Upgrade arm (temporary)/Elbow bearing` | yes | MeshRenderer | VU_Suit_joint_bearing |  |
| 12 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/LeftArm/Visual Upgrade arm (temporary)/Forearm` | yes | MeshRenderer | VU_Suit_fabric |  |
| 13 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/LeftArm/Visual Upgrade arm (temporary)/Glove` | yes | MeshRenderer | VU_Suit_soft_grey |  |
| 14 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/LeftArm/Visual Upgrade arm (temporary)/Shoulder bearing` | yes | MeshRenderer | VU_Suit_fabric |  |
| 15 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/LeftArm/Visual Upgrade arm (temporary)/Shoulder emblem (provisional)` | yes | MeshRenderer | VU_Mission_emblem_(provisional) |  |
| 16 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/LeftArm/Visual Upgrade arm (temporary)/Upper arm` | yes | MeshRenderer | VU_Suit_fabric |  |
| 17 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/LeftArm/Visual Upgrade arm (temporary)/Wrist bearing` | yes | MeshRenderer | VU_Suit_joint_bearing |  |
| 18 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/LeftArm/Visual Upgrade arm (temporary)/Wrist display` | yes | MeshRenderer | VU_Status_cyan |  |
| 19 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/LeftLeg` | yes |  |  |  |
| 20 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/LeftLeg/Added EVA boot` | yes | (renderer off), MeshRenderer | Inherited_3 |  |
| 21 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/LeftLeg/SuitSegment` | yes | (renderer off), MeshRenderer | Inherited_12 |  |
| 22 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/LeftLeg/Visual Upgrade leg (temporary)` | yes |  |  |  |
| 23 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/LeftLeg/Visual Upgrade leg (temporary)/Ankle bearing` | yes | MeshRenderer | VU_Suit_joint_bearing |  |
| 24 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/LeftLeg/Visual Upgrade leg (temporary)/Boot sole` | yes | MeshRenderer | VU_Suit_joint_bearing |  |
| 25 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/LeftLeg/Visual Upgrade leg (temporary)/EVA boot` | yes | MeshRenderer | VU_Suit_soft_grey |  |
| 26 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/LeftLeg/Visual Upgrade leg (temporary)/Knee bearing` | yes | MeshRenderer | VU_Suit_joint_bearing |  |
| 27 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/LeftLeg/Visual Upgrade leg (temporary)/Knee pad` | yes | MeshRenderer | VU_Suit_soft_grey |  |
| 28 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/LeftLeg/Visual Upgrade leg (temporary)/Shin` | yes | MeshRenderer | VU_Suit_fabric |  |
| 29 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/LeftLeg/Visual Upgrade leg (temporary)/Thigh` | yes | MeshRenderer | VU_Suit_fabric |  |
| 30 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/RightArm` | yes |  |  |  |
| 31 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/RightArm/SuitSegment` | yes | (renderer off), MeshRenderer | Inherited_12 |  |
| 32 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/RightArm/Visual Upgrade arm (temporary)` | yes |  |  |  |
| 33 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/RightArm/Visual Upgrade arm (temporary)/Elbow bearing` | yes | MeshRenderer | VU_Suit_joint_bearing |  |
| 34 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/RightArm/Visual Upgrade arm (temporary)/Forearm` | yes | MeshRenderer | VU_Suit_fabric |  |
| 35 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/RightArm/Visual Upgrade arm (temporary)/Glove` | yes | MeshRenderer | VU_Suit_soft_grey |  |
| 36 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/RightArm/Visual Upgrade arm (temporary)/Shoulder bearing` | yes | MeshRenderer | VU_Suit_fabric |  |
| 37 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/RightArm/Visual Upgrade arm (temporary)/Upper arm` | yes | MeshRenderer | VU_Suit_fabric |  |
| 38 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/RightArm/Visual Upgrade arm (temporary)/Wrist bearing` | yes | MeshRenderer | VU_Suit_joint_bearing |  |
| 39 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/RightArm/Visual Upgrade arm (temporary)/Wrist display` | yes | MeshRenderer | VU_Suit_soft_grey |  |
| 40 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/RightLeg` | yes |  |  |  |
| 41 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/RightLeg/Added EVA boot` | yes | (renderer off), MeshRenderer | Inherited_3 |  |
| 42 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/RightLeg/SuitSegment` | yes | (renderer off), MeshRenderer | Inherited_12 |  |
| 43 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/RightLeg/Visual Upgrade leg (temporary)` | yes |  |  |  |
| 44 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/RightLeg/Visual Upgrade leg (temporary)/Ankle bearing` | yes | MeshRenderer | VU_Suit_joint_bearing |  |
| 45 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/RightLeg/Visual Upgrade leg (temporary)/Boot sole` | yes | MeshRenderer | VU_Suit_joint_bearing |  |
| 46 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/RightLeg/Visual Upgrade leg (temporary)/EVA boot` | yes | MeshRenderer | VU_Suit_soft_grey |  |
| 47 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/RightLeg/Visual Upgrade leg (temporary)/Knee bearing` | yes | MeshRenderer | VU_Suit_joint_bearing |  |
| 48 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/RightLeg/Visual Upgrade leg (temporary)/Knee pad` | yes | MeshRenderer | VU_Suit_soft_grey |  |
| 49 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/RightLeg/Visual Upgrade leg (temporary)/Shin` | yes | MeshRenderer | VU_Suit_fabric |  |
| 50 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/RightLeg/Visual Upgrade leg (temporary)/Thigh` | yes | MeshRenderer | VU_Suit_fabric |  |
| 51 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/Torso` | yes | (renderer off), MeshRenderer | Inherited_12 |  |
| 52 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/Visual Upgrade suit — TEMPORARY procedural art (design-team asset pending)` | yes |  |  |  |
| 53 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/Visual Upgrade suit — TEMPORARY procedural art (design-team asset pending)/Backpack antenna` | yes | MeshRenderer | VU_Suit_joint_bearing |  |
| 54 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/Visual Upgrade suit — TEMPORARY procedural art (design-team asset pending)/Backpack beacon` | yes | MeshRenderer | VU_Status_amber |  |
| 55 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/Visual Upgrade suit — TEMPORARY procedural art (design-team asset pending)/Backpack service panel` | yes | MeshRenderer | VU_Suit_soft_grey |  |
| 56 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/Visual Upgrade suit — TEMPORARY procedural art (design-team asset pending)/Backpack top cap` | yes | MeshRenderer | VU_Suit_soft_grey |  |
| 57 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/Visual Upgrade suit — TEMPORARY procedural art (design-team asset pending)/Chest emblem (provisional)` | yes | MeshRenderer | VU_Mission_emblem_(provisional) |  |
| 58 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/Visual Upgrade suit — TEMPORARY procedural art (design-team asset pending)/DCM status light 0` | yes | MeshRenderer | VU_Status_cyan |  |
| 59 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/Visual Upgrade suit — TEMPORARY procedural art (design-team asset pending)/DCM status light 1` | yes | MeshRenderer | VU_Status_amber |  |
| 60 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/Visual Upgrade suit — TEMPORARY procedural art (design-team asset pending)/DCM status light 2` | yes | MeshRenderer | VU_Status_cyan |  |
| 61 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/Visual Upgrade suit — TEMPORARY procedural art (design-team asset pending)/Display and control module` | yes | MeshRenderer | VU_Suit_soft_grey |  |
| 62 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/Visual Upgrade suit — TEMPORARY procedural art (design-team asset pending)/Gold visor` | yes | MeshRenderer | VU_Visor_gold_coating |  |
| 63 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/Visual Upgrade suit — TEMPORARY procedural art (design-team asset pending)/Helmet lamp -1` | yes | MeshRenderer | VU_Helmet_lamp |  |
| 64 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/Visual Upgrade suit — TEMPORARY procedural art (design-team asset pending)/Helmet lamp 1` | yes | MeshRenderer | VU_Helmet_lamp |  |
| 65 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/Visual Upgrade suit — TEMPORARY procedural art (design-team asset pending)/Helmet lamp housing -1` | yes | MeshRenderer | VU_Suit_soft_grey |  |
| 66 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/Visual Upgrade suit — TEMPORARY procedural art (design-team asset pending)/Helmet lamp housing 1` | yes | MeshRenderer | VU_Suit_soft_grey |  |
| 67 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/Visual Upgrade suit — TEMPORARY procedural art (design-team asset pending)/Helmet shell` | yes | MeshRenderer | VU_Suit_hard_shell |  |
| 68 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/Visual Upgrade suit — TEMPORARY procedural art (design-team asset pending)/Hip brief` | yes | MeshRenderer | VU_Suit_fabric |  |
| 69 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/Visual Upgrade suit — TEMPORARY procedural art (design-team asset pending)/Hose -1` | yes | MeshRenderer | VU_Suit_joint_bearing |  |
| 70 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/Visual Upgrade suit — TEMPORARY procedural art (design-team asset pending)/Hose 1` | yes | MeshRenderer | VU_Suit_joint_bearing |  |
| 71 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/Visual Upgrade suit — TEMPORARY procedural art (design-team asset pending)/Life-support backpack` | yes | MeshRenderer | VU_Suit_hard_shell |  |
| 72 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/Visual Upgrade suit — TEMPORARY procedural art (design-team asset pending)/Neck ring` | yes | MeshRenderer | VU_Suit_joint_bearing |  |
| 73 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/Visual Upgrade suit — TEMPORARY procedural art (design-team asset pending)/Oxygen bottle -1` | yes | MeshRenderer | VU_Suit_soft_grey |  |
| 74 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/Visual Upgrade suit — TEMPORARY procedural art (design-team asset pending)/Oxygen bottle 1` | yes | MeshRenderer | VU_Suit_soft_grey |  |
| 75 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/Visual Upgrade suit — TEMPORARY procedural art (design-team asset pending)/Torso` | yes | MeshRenderer | VU_Suit_fabric |  |
| 76 | Astronaut | `02_LocalProvisionalExploration/Astronaut_OriginalPrimitivePrototype/SuitRig/Visual Upgrade suit — TEMPORARY procedural art (design-team asset pending)/Waist bearing` | yes | MeshRenderer | VU_Suit_joint_bearing |  |
| 77 | Astronaut | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/Persistent astronaut seat anchor` | yes |  |  |  |
| 78 | Camera | `Non-gameplay scenic overview` | yes | Camera, MonoBehaviour |  |  |
| 79 | Communications | `Mars Research Base — 250m playable world/Communication reflector` | yes | (renderer off), MeshRenderer | Inherited_9 |  |
| 80 | Communications | `Mars Research Base — 250m playable world/Communications node mast` | yes | CapsuleCollider, MeshRenderer | Inherited_3 | Capsule |
| 81 | Communications | `Mars Research Base — 250m playable world/Communications support truss` | yes | MeshRenderer | Machined graphite alloy |  |
| 82 | Communications | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Communications tower detail (visual only)` | yes |  |  |  |
| 83 | Communications | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Communications tower detail (visual only)/Anchor 0` | yes | MeshRenderer | VU_Rover_graphite |  |
| 84 | Communications | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Communications tower detail (visual only)/Anchor 1` | yes | MeshRenderer | VU_Rover_graphite |  |
| 85 | Communications | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Communications tower detail (visual only)/Anchor 2` | yes | MeshRenderer | VU_Rover_graphite |  |
| 86 | Communications | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Communications tower detail (visual only)/Aviation beacon` | yes | MeshRenderer | VU_Beacon_red |  |
| 87 | Communications | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Communications tower detail (visual only)/Feed horn` | yes | MeshRenderer | VU_Rover_graphite |  |
| 88 | Communications | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Communications tower detail (visual only)/Feed support` | yes | MeshRenderer | VU_Brushed_aluminium |  |
| 89 | Communications | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Communications tower detail (visual only)/Guy wire 0` | yes | MeshRenderer | VU_Rover_graphite |  |
| 90 | Communications | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Communications tower detail (visual only)/Guy wire 1` | yes | MeshRenderer | VU_Rover_graphite |  |
| 91 | Communications | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Communications tower detail (visual only)/Guy wire 2` | yes | MeshRenderer | VU_Rover_graphite |  |
| 92 | Communications | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Communications tower detail (visual only)/Parabolic dish` | yes | MeshRenderer | VU_Equipment_white |  |
| 93 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Crew Dome exterior detail (visual only)` | yes |  |  |  |
| 94 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Crew Dome exterior detail (visual only)/Airlock emblem (provisional)` | yes | MeshRenderer | VU_Mission_emblem_(provisional) |  |
| 95 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Crew Dome exterior detail (visual only)/Airlock hazard stripe` | yes | MeshRenderer | VU_Accent_orange |  |
| 96 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Crew Dome exterior detail (visual only)/Airlock status light` | yes | MeshRenderer | VU_Status_cyan |  |
| 97 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Crew Dome exterior detail (visual only)/Floodlight housing` | yes | MeshRenderer | VU_Rover_graphite |  |
| 98 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Crew Dome exterior detail (visual only)/Floodlight lens` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 99 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Crew Dome exterior detail (visual only)/Foundation light 0` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 100 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Crew Dome exterior detail (visual only)/Foundation light 1` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 101 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Crew Dome exterior detail (visual only)/Foundation light 10` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 102 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Crew Dome exterior detail (visual only)/Foundation light 11` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 103 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Crew Dome exterior detail (visual only)/Foundation light 12` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 104 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Crew Dome exterior detail (visual only)/Foundation light 13` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 105 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Crew Dome exterior detail (visual only)/Foundation light 14` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 106 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Crew Dome exterior detail (visual only)/Foundation light 15` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 107 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Crew Dome exterior detail (visual only)/Foundation light 2` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 108 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Crew Dome exterior detail (visual only)/Foundation light 3` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 109 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Crew Dome exterior detail (visual only)/Foundation light 4` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 110 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Crew Dome exterior detail (visual only)/Foundation light 5` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 111 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Crew Dome exterior detail (visual only)/Foundation light 6` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 112 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Crew Dome exterior detail (visual only)/Foundation light 7` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 113 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Crew Dome exterior detail (visual only)/Foundation light 8` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 114 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Crew Dome exterior detail (visual only)/Foundation light 9` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 115 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Laboratory Dome exterior detail (visual only)` | yes |  |  |  |
| 116 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Laboratory Dome exterior detail (visual only)/Airlock emblem (provisional)` | yes | MeshRenderer | VU_Mission_emblem_(provisional) |  |
| 117 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Laboratory Dome exterior detail (visual only)/Airlock hazard stripe` | yes | MeshRenderer | VU_Accent_orange |  |
| 118 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Laboratory Dome exterior detail (visual only)/Airlock status light` | yes | MeshRenderer | VU_Status_cyan |  |
| 119 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Laboratory Dome exterior detail (visual only)/Floodlight housing` | yes | MeshRenderer | VU_Rover_graphite |  |
| 120 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Laboratory Dome exterior detail (visual only)/Floodlight lens` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 121 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Laboratory Dome exterior detail (visual only)/Foundation light 0` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 122 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Laboratory Dome exterior detail (visual only)/Foundation light 1` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 123 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Laboratory Dome exterior detail (visual only)/Foundation light 10` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 124 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Laboratory Dome exterior detail (visual only)/Foundation light 11` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 125 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Laboratory Dome exterior detail (visual only)/Foundation light 12` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 126 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Laboratory Dome exterior detail (visual only)/Foundation light 13` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 127 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Laboratory Dome exterior detail (visual only)/Foundation light 14` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 128 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Laboratory Dome exterior detail (visual only)/Foundation light 15` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 129 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Laboratory Dome exterior detail (visual only)/Foundation light 2` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 130 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Laboratory Dome exterior detail (visual only)/Foundation light 3` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 131 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Laboratory Dome exterior detail (visual only)/Foundation light 4` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 132 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Laboratory Dome exterior detail (visual only)/Foundation light 5` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 133 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Laboratory Dome exterior detail (visual only)/Foundation light 6` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 134 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Laboratory Dome exterior detail (visual only)/Foundation light 7` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 135 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Laboratory Dome exterior detail (visual only)/Foundation light 8` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 136 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Laboratory Dome exterior detail (visual only)/Foundation light 9` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 137 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Life Support Dome exterior detail (visual only)` | yes |  |  |  |
| 138 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Life Support Dome exterior detail (visual only)/Airlock emblem (provisional)` | yes | MeshRenderer | VU_Mission_emblem_(provisional) |  |
| 139 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Life Support Dome exterior detail (visual only)/Airlock hazard stripe` | yes | MeshRenderer | VU_Accent_orange |  |
| 140 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Life Support Dome exterior detail (visual only)/Airlock status light` | yes | MeshRenderer | VU_Status_cyan |  |
| 141 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Life Support Dome exterior detail (visual only)/Floodlight housing` | yes | MeshRenderer | VU_Rover_graphite |  |
| 142 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Life Support Dome exterior detail (visual only)/Floodlight lens` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 143 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Life Support Dome exterior detail (visual only)/Foundation light 0` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 144 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Life Support Dome exterior detail (visual only)/Foundation light 1` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 145 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Life Support Dome exterior detail (visual only)/Foundation light 10` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 146 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Life Support Dome exterior detail (visual only)/Foundation light 11` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 147 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Life Support Dome exterior detail (visual only)/Foundation light 12` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 148 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Life Support Dome exterior detail (visual only)/Foundation light 13` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 149 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Life Support Dome exterior detail (visual only)/Foundation light 14` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 150 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Life Support Dome exterior detail (visual only)/Foundation light 15` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 151 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Life Support Dome exterior detail (visual only)/Foundation light 2` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 152 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Life Support Dome exterior detail (visual only)/Foundation light 3` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 153 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Life Support Dome exterior detail (visual only)/Foundation light 4` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 154 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Life Support Dome exterior detail (visual only)/Foundation light 5` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 155 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Life Support Dome exterior detail (visual only)/Foundation light 6` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 156 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Life Support Dome exterior detail (visual only)/Foundation light 7` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 157 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Life Support Dome exterior detail (visual only)/Foundation light 8` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 158 | Habitat | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Life Support Dome exterior detail (visual only)/Foundation light 9` | yes | MeshRenderer | VU_Habitat_marker_light |  |
| 159 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex` | yes |  |  |  |
| 160 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Batched triangulated structural seams` | yes | (renderer off), MeshRenderer | Inherited_5 |  |
| 161 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Batched triangulated structural seams` | yes | (renderer off), MeshRenderer | Inherited_5 |  |
| 162 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Batched triangulated structural seams` | yes | (renderer off), MeshRenderer | Inherited_5 |  |
| 163 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Crew Dome` | yes | MeshRenderer, SphereCollider | VU_Habitat_shell, VU_Habitat_window_glow | Sphere |
| 164 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Crew Dome closed door` | yes | MeshRenderer | Inherited_3 |  |
| 165 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Crew Dome closed door/Airlock pressure frame` | yes | MeshRenderer | Inherited_5 |  |
| 166 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Crew Dome closed door/Airlock pressure frame` | yes | MeshRenderer | Inherited_5 |  |
| 167 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Crew Dome closed door/Door center seal` | yes | MeshRenderer | Inherited_11 |  |
| 168 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Crew Dome closed door/Door identification stripe` | yes | MeshRenderer | Inherited_8 |  |
| 169 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Crew Dome foundation rim` | yes | MeshRenderer | Inherited_9 |  |
| 170 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Crew Dome observation panel 0` | yes | (renderer off), MeshRenderer | Inherited_6 |  |
| 171 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Crew Dome observation panel 1` | yes | (renderer off), MeshRenderer | Inherited_6 |  |
| 172 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Crew Dome observation panel 2` | yes | (renderer off), MeshRenderer | Inherited_6 |  |
| 173 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Crew Dome observation panel 3` | yes | (renderer off), MeshRenderer | Inherited_6 |  |
| 174 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Crew Dome sealed airlock` | yes | BoxCollider, MeshRenderer | Pressure shell ceramic composite | Box |
| 175 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Crew Dome sealed airlock/Service rails — non-colliding trim` | yes | MeshRenderer | Brushed aluminum |  |
| 176 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Crew Dome/Geodesic structural frame` | yes | MeshRenderer | Machined graphite alloy |  |
| 177 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Laboratory Dome` | yes | MeshRenderer, SphereCollider | VU_Habitat_shell, VU_Habitat_window_glow | Sphere |
| 178 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Laboratory Dome closed door` | yes | MeshRenderer | Inherited_3 |  |
| 179 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Laboratory Dome closed door/Airlock pressure frame` | yes | MeshRenderer | Inherited_5 |  |
| 180 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Laboratory Dome closed door/Airlock pressure frame` | yes | MeshRenderer | Inherited_5 |  |
| 181 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Laboratory Dome closed door/Door center seal` | yes | MeshRenderer | Inherited_11 |  |
| 182 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Laboratory Dome closed door/Door identification stripe` | yes | MeshRenderer | Inherited_8 |  |
| 183 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Laboratory Dome foundation rim` | yes | MeshRenderer | Inherited_9 |  |
| 184 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Laboratory Dome observation panel 0` | yes | (renderer off), MeshRenderer | Inherited_6 |  |
| 185 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Laboratory Dome observation panel 1` | yes | (renderer off), MeshRenderer | Inherited_6 |  |
| 186 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Laboratory Dome observation panel 2` | yes | (renderer off), MeshRenderer | Inherited_6 |  |
| 187 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Laboratory Dome observation panel 3` | yes | (renderer off), MeshRenderer | Inherited_6 |  |
| 188 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Laboratory Dome sealed airlock` | yes | BoxCollider, MeshRenderer | Pressure shell ceramic composite | Box |
| 189 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Laboratory Dome sealed airlock/Service rails — non-colliding trim` | yes | MeshRenderer | Brushed aluminum |  |
| 190 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Laboratory Dome/Geodesic structural frame` | yes | MeshRenderer | Machined graphite alloy |  |
| 191 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Life Support Dome` | yes | MeshRenderer, SphereCollider | VU_Habitat_shell, VU_Habitat_window_glow | Sphere |
| 192 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Life Support Dome closed door` | yes | MeshRenderer | Inherited_3 |  |
| 193 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Life Support Dome closed door/Airlock pressure frame` | yes | MeshRenderer | Inherited_5 |  |
| 194 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Life Support Dome closed door/Airlock pressure frame` | yes | MeshRenderer | Inherited_5 |  |
| 195 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Life Support Dome closed door/Door center seal` | yes | MeshRenderer | Inherited_11 |  |
| 196 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Life Support Dome closed door/Door identification stripe` | yes | MeshRenderer | Inherited_8 |  |
| 197 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Life Support Dome foundation rim` | yes | MeshRenderer | Inherited_9 |  |
| 198 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Life Support Dome observation panel 0` | yes | (renderer off), MeshRenderer | Inherited_6 |  |
| 199 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Life Support Dome observation panel 1` | yes | (renderer off), MeshRenderer | Inherited_6 |  |
| 200 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Life Support Dome observation panel 2` | yes | (renderer off), MeshRenderer | Inherited_6 |  |
| 201 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Life Support Dome observation panel 3` | yes | (renderer off), MeshRenderer | Inherited_6 |  |
| 202 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Life Support Dome sealed airlock` | yes | BoxCollider, MeshRenderer | Pressure shell ceramic composite | Box |
| 203 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Life Support Dome sealed airlock/Service rails — non-colliding trim` | yes | MeshRenderer | Brushed aluminum |  |
| 204 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Life Support Dome/Geodesic structural frame` | yes | MeshRenderer | Machined graphite alloy |  |
| 205 | Habitat | `Mars Research Base — 250m playable world/Zone A — ARESYN habitat complex/Sealed habitat connector` | yes | BoxCollider, MeshRenderer | Inherited_9 | Box |
| 206 | Light | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Crew Dome exterior detail (visual only)/Airlock floodlight` | yes | Light |  |  |
| 207 | Light | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Laboratory Dome exterior detail (visual only)/Airlock floodlight` | yes | Light |  |  |
| 208 | Light | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Life Support Dome exterior detail (visual only)/Airlock floodlight` | yes | Light |  |  |
| 209 | Light | `Warm directional sunlight` | yes | Light |  |  |
| 210 | Other | `Mars Research Base — 250m playable world` | yes |  |  |  |
| 211 | Other | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)` | yes |  |  |  |
| 212 | Other | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Base reflection probe (realtime, refreshed on start)` | yes | ReflectionProbe |  |  |
| 213 | Props | `Mars Research Base — 250m playable world/Life support storage tank 0` | yes | CapsuleCollider, MeshRenderer | Inherited_9 | Capsule |
| 214 | Props | `Mars Research Base — 250m playable world/Life support storage tank 1` | yes | CapsuleCollider, MeshRenderer | Inherited_9 | Capsule |
| 215 | Props | `Mars Research Base — 250m playable world/Life support storage tank 2` | yes | CapsuleCollider, MeshRenderer | Inherited_9 | Capsule |
| 216 | Props | `Mars Research Base — 250m playable world/Life support storage tank 3` | yes | CapsuleCollider, MeshRenderer | Inherited_9 | Capsule |
| 217 | Props | `Mars Research Base — 250m playable world/Power distribution cabinet` | yes | BoxCollider, MeshRenderer | Inherited_9 | Box |
| 218 | Props | `Mars Research Base — 250m playable world/Storage crate 0` | yes | BoxCollider, MeshRenderer | Inherited_9 | Box |
| 219 | Props | `Mars Research Base — 250m playable world/Storage crate 1` | yes | BoxCollider, MeshRenderer | Inherited_9 | Box |
| 220 | Props | `Mars Research Base — 250m playable world/Storage crate 2` | yes | BoxCollider, MeshRenderer | Inherited_9 | Box |
| 221 | Props | `Mars Research Base — 250m playable world/Storage crate 3` | yes | BoxCollider, MeshRenderer | Inherited_9 | Box |
| 222 | Research station | `Mars Research Base — 250m playable world/Research station 0` | yes | (renderer off), CapsuleCollider, MeshRenderer, MonoBehaviour | Inherited_14 | Capsule |
| 223 | Research station | `Mars Research Base — 250m playable world/Research station 1` | yes | (renderer off), CapsuleCollider, MeshRenderer, MonoBehaviour | Inherited_14 | Capsule |
| 224 | Research station | `Mars Research Base — 250m playable world/Research station 2` | yes | (renderer off), CapsuleCollider, MeshRenderer, MonoBehaviour | Inherited_14 | Capsule |
| 225 | Research station | `Mars Research Base — 250m playable world/Research station 3` | yes | (renderer off), CapsuleCollider, MeshRenderer, MonoBehaviour | Inherited_14 | Capsule |
| 226 | Research station | `Mars Research Base — 250m playable world/Research station 4` | yes | (renderer off), CapsuleCollider, MeshRenderer, MonoBehaviour | Inherited_14 | Capsule |
| 227 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)` | yes |  |  |  |
| 228 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Communications inspection` | yes |  |  |  |
| 229 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Communications inspection/Dish mast` | yes | MeshRenderer | VU_Brushed_aluminium |  |
| 230 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Communications inspection/Foot 0` | yes | MeshRenderer | VU_Rover_graphite |  |
| 231 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Communications inspection/Foot 1` | yes | MeshRenderer | VU_Rover_graphite |  |
| 232 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Communications inspection/Foot 2` | yes | MeshRenderer | VU_Rover_graphite |  |
| 233 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Communications inspection/Head stripe` | yes | MeshRenderer | VU_Accent_orange |  |
| 234 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Communications inspection/Lens ring` | yes | MeshRenderer | VU_Rover_graphite |  |
| 235 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Communications inspection/Panel arm` | yes | MeshRenderer | VU_Brushed_aluminium |  |
| 236 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Communications inspection/Relay dish` | yes | MeshRenderer | VU_Equipment_white |  |
| 237 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Communications inspection/Sample case` | yes | MeshRenderer | VU_Accent_orange |  |
| 238 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Communications inspection/Sensor head` | yes | MeshRenderer | VU_Equipment_white |  |
| 239 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Communications inspection/Solar panel` | yes | MeshRenderer | VU_Rover_solar_cells |  |
| 240 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Communications inspection/Spectrometer lens` | yes | MeshRenderer | VU_Sensor_lens |  |
| 241 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Communications inspection/Status light` | yes | MeshRenderer | VU_Status_cyan |  |
| 242 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Communications inspection/Survey marker 0` | yes | MeshRenderer | VU_Accent_orange |  |
| 243 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Communications inspection/Survey marker 1` | yes | MeshRenderer | VU_Equipment_white |  |
| 244 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Communications inspection/Survey marker 2` | yes | MeshRenderer | VU_Accent_orange |  |
| 245 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Communications inspection/Survey marker 3` | yes | MeshRenderer | VU_Equipment_white |  |
| 246 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Communications inspection/Survey marker 4` | yes | MeshRenderer | VU_Accent_orange |  |
| 247 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Communications inspection/Survey marker 5` | yes | MeshRenderer | VU_Equipment_white |  |
| 248 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Communications inspection/Survey marker 6` | yes | MeshRenderer | VU_Accent_orange |  |
| 249 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Communications inspection/Survey marker 7` | yes | MeshRenderer | VU_Equipment_white |  |
| 250 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Communications inspection/Tripod leg 0` | yes | MeshRenderer | VU_Brushed_aluminium |  |
| 251 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Communications inspection/Tripod leg 1` | yes | MeshRenderer | VU_Brushed_aluminium |  |
| 252 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Communications inspection/Tripod leg 2` | yes | MeshRenderer | VU_Brushed_aluminium |  |
| 253 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Ridge mineral sensor` | yes |  |  |  |
| 254 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Ridge mineral sensor/Dish mast` | yes | MeshRenderer | VU_Brushed_aluminium |  |
| 255 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Ridge mineral sensor/Foot 0` | yes | MeshRenderer | VU_Rover_graphite |  |
| 256 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Ridge mineral sensor/Foot 1` | yes | MeshRenderer | VU_Rover_graphite |  |
| 257 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Ridge mineral sensor/Foot 2` | yes | MeshRenderer | VU_Rover_graphite |  |
| 258 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Ridge mineral sensor/Head stripe` | yes | MeshRenderer | VU_Accent_orange |  |
| 259 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Ridge mineral sensor/Lens ring` | yes | MeshRenderer | VU_Rover_graphite |  |
| 260 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Ridge mineral sensor/Panel arm` | yes | MeshRenderer | VU_Brushed_aluminium |  |
| 261 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Ridge mineral sensor/Relay dish` | yes | MeshRenderer | VU_Equipment_white |  |
| 262 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Ridge mineral sensor/Sample case` | yes | MeshRenderer | VU_Accent_orange |  |
| 263 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Ridge mineral sensor/Sensor head` | yes | MeshRenderer | VU_Equipment_white |  |
| 264 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Ridge mineral sensor/Solar panel` | yes | MeshRenderer | VU_Rover_solar_cells |  |
| 265 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Ridge mineral sensor/Spectrometer lens` | yes | MeshRenderer | VU_Sensor_lens |  |
| 266 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Ridge mineral sensor/Status light` | yes | MeshRenderer | VU_Status_cyan |  |
| 267 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Ridge mineral sensor/Survey marker 0` | yes | MeshRenderer | VU_Accent_orange |  |
| 268 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Ridge mineral sensor/Survey marker 1` | yes | MeshRenderer | VU_Equipment_white |  |
| 269 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Ridge mineral sensor/Survey marker 2` | yes | MeshRenderer | VU_Accent_orange |  |
| 270 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Ridge mineral sensor/Survey marker 3` | yes | MeshRenderer | VU_Equipment_white |  |
| 271 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Ridge mineral sensor/Survey marker 4` | yes | MeshRenderer | VU_Accent_orange |  |
| 272 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Ridge mineral sensor/Survey marker 5` | yes | MeshRenderer | VU_Equipment_white |  |
| 273 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Ridge mineral sensor/Survey marker 6` | yes | MeshRenderer | VU_Accent_orange |  |
| 274 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Ridge mineral sensor/Survey marker 7` | yes | MeshRenderer | VU_Equipment_white |  |
| 275 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Ridge mineral sensor/Tripod leg 0` | yes | MeshRenderer | VU_Brushed_aluminium |  |
| 276 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Ridge mineral sensor/Tripod leg 1` | yes | MeshRenderer | VU_Brushed_aluminium |  |
| 277 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Ridge mineral sensor/Tripod leg 2` | yes | MeshRenderer | VU_Brushed_aluminium |  |
| 278 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Rock sample station` | yes |  |  |  |
| 279 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Rock sample station/Dish mast` | yes | MeshRenderer | VU_Brushed_aluminium |  |
| 280 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Rock sample station/Foot 0` | yes | MeshRenderer | VU_Rover_graphite |  |
| 281 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Rock sample station/Foot 1` | yes | MeshRenderer | VU_Rover_graphite |  |
| 282 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Rock sample station/Foot 2` | yes | MeshRenderer | VU_Rover_graphite |  |
| 283 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Rock sample station/Head stripe` | yes | MeshRenderer | VU_Accent_orange |  |
| 284 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Rock sample station/Lens ring` | yes | MeshRenderer | VU_Rover_graphite |  |
| 285 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Rock sample station/Panel arm` | yes | MeshRenderer | VU_Brushed_aluminium |  |
| 286 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Rock sample station/Relay dish` | yes | MeshRenderer | VU_Equipment_white |  |
| 287 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Rock sample station/Sample case` | yes | MeshRenderer | VU_Accent_orange |  |
| 288 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Rock sample station/Sensor head` | yes | MeshRenderer | VU_Equipment_white |  |
| 289 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Rock sample station/Solar panel` | yes | MeshRenderer | VU_Rover_solar_cells |  |
| 290 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Rock sample station/Spectrometer lens` | yes | MeshRenderer | VU_Sensor_lens |  |
| 291 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Rock sample station/Status light` | yes | MeshRenderer | VU_Status_cyan |  |
| 292 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Rock sample station/Survey marker 0` | yes | MeshRenderer | VU_Accent_orange |  |
| 293 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Rock sample station/Survey marker 1` | yes | MeshRenderer | VU_Equipment_white |  |
| 294 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Rock sample station/Survey marker 2` | yes | MeshRenderer | VU_Accent_orange |  |
| 295 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Rock sample station/Survey marker 3` | yes | MeshRenderer | VU_Equipment_white |  |
| 296 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Rock sample station/Survey marker 4` | yes | MeshRenderer | VU_Accent_orange |  |
| 297 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Rock sample station/Survey marker 5` | yes | MeshRenderer | VU_Equipment_white |  |
| 298 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Rock sample station/Survey marker 6` | yes | MeshRenderer | VU_Accent_orange |  |
| 299 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Rock sample station/Survey marker 7` | yes | MeshRenderer | VU_Equipment_white |  |
| 300 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Rock sample station/Tripod leg 0` | yes | MeshRenderer | VU_Brushed_aluminium |  |
| 301 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Rock sample station/Tripod leg 1` | yes | MeshRenderer | VU_Brushed_aluminium |  |
| 302 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Rock sample station/Tripod leg 2` | yes | MeshRenderer | VU_Brushed_aluminium |  |
| 303 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Soil crater sampler` | yes |  |  |  |
| 304 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Soil crater sampler/Dish mast` | yes | MeshRenderer | VU_Brushed_aluminium |  |
| 305 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Soil crater sampler/Foot 0` | yes | MeshRenderer | VU_Rover_graphite |  |
| 306 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Soil crater sampler/Foot 1` | yes | MeshRenderer | VU_Rover_graphite |  |
| 307 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Soil crater sampler/Foot 2` | yes | MeshRenderer | VU_Rover_graphite |  |
| 308 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Soil crater sampler/Head stripe` | yes | MeshRenderer | VU_Accent_orange |  |
| 309 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Soil crater sampler/Lens ring` | yes | MeshRenderer | VU_Rover_graphite |  |
| 310 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Soil crater sampler/Panel arm` | yes | MeshRenderer | VU_Brushed_aluminium |  |
| 311 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Soil crater sampler/Relay dish` | yes | MeshRenderer | VU_Equipment_white |  |
| 312 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Soil crater sampler/Sample case` | yes | MeshRenderer | VU_Accent_orange |  |
| 313 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Soil crater sampler/Sensor head` | yes | MeshRenderer | VU_Equipment_white |  |
| 314 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Soil crater sampler/Solar panel` | yes | MeshRenderer | VU_Rover_solar_cells |  |
| 315 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Soil crater sampler/Spectrometer lens` | yes | MeshRenderer | VU_Sensor_lens |  |
| 316 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Soil crater sampler/Status light` | yes | MeshRenderer | VU_Status_cyan |  |
| 317 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Soil crater sampler/Survey marker 0` | yes | MeshRenderer | VU_Accent_orange |  |
| 318 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Soil crater sampler/Survey marker 1` | yes | MeshRenderer | VU_Equipment_white |  |
| 319 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Soil crater sampler/Survey marker 2` | yes | MeshRenderer | VU_Accent_orange |  |
| 320 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Soil crater sampler/Survey marker 3` | yes | MeshRenderer | VU_Equipment_white |  |
| 321 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Soil crater sampler/Survey marker 4` | yes | MeshRenderer | VU_Accent_orange |  |
| 322 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Soil crater sampler/Survey marker 5` | yes | MeshRenderer | VU_Equipment_white |  |
| 323 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Soil crater sampler/Survey marker 6` | yes | MeshRenderer | VU_Accent_orange |  |
| 324 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Soil crater sampler/Survey marker 7` | yes | MeshRenderer | VU_Equipment_white |  |
| 325 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Soil crater sampler/Tripod leg 0` | yes | MeshRenderer | VU_Brushed_aluminium |  |
| 326 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Soil crater sampler/Tripod leg 1` | yes | MeshRenderer | VU_Brushed_aluminium |  |
| 327 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Soil crater sampler/Tripod leg 2` | yes | MeshRenderer | VU_Brushed_aluminium |  |
| 328 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Solar inspection` | yes |  |  |  |
| 329 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Solar inspection/Dish mast` | yes | MeshRenderer | VU_Brushed_aluminium |  |
| 330 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Solar inspection/Foot 0` | yes | MeshRenderer | VU_Rover_graphite |  |
| 331 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Solar inspection/Foot 1` | yes | MeshRenderer | VU_Rover_graphite |  |
| 332 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Solar inspection/Foot 2` | yes | MeshRenderer | VU_Rover_graphite |  |
| 333 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Solar inspection/Head stripe` | yes | MeshRenderer | VU_Accent_orange |  |
| 334 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Solar inspection/Lens ring` | yes | MeshRenderer | VU_Rover_graphite |  |
| 335 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Solar inspection/Panel arm` | yes | MeshRenderer | VU_Brushed_aluminium |  |
| 336 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Solar inspection/Relay dish` | yes | MeshRenderer | VU_Equipment_white |  |
| 337 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Solar inspection/Sample case` | yes | MeshRenderer | VU_Accent_orange |  |
| 338 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Solar inspection/Sensor head` | yes | MeshRenderer | VU_Equipment_white |  |
| 339 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Solar inspection/Solar panel` | yes | MeshRenderer | VU_Rover_solar_cells |  |
| 340 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Solar inspection/Spectrometer lens` | yes | MeshRenderer | VU_Sensor_lens |  |
| 341 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Solar inspection/Status light` | yes | MeshRenderer | VU_Status_cyan |  |
| 342 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Solar inspection/Survey marker 0` | yes | MeshRenderer | VU_Accent_orange |  |
| 343 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Solar inspection/Survey marker 1` | yes | MeshRenderer | VU_Equipment_white |  |
| 344 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Solar inspection/Survey marker 2` | yes | MeshRenderer | VU_Accent_orange |  |
| 345 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Solar inspection/Survey marker 3` | yes | MeshRenderer | VU_Equipment_white |  |
| 346 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Solar inspection/Survey marker 4` | yes | MeshRenderer | VU_Accent_orange |  |
| 347 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Solar inspection/Survey marker 5` | yes | MeshRenderer | VU_Equipment_white |  |
| 348 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Solar inspection/Survey marker 6` | yes | MeshRenderer | VU_Accent_orange |  |
| 349 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Solar inspection/Survey marker 7` | yes | MeshRenderer | VU_Equipment_white |  |
| 350 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Solar inspection/Tripod leg 0` | yes | MeshRenderer | VU_Brushed_aluminium |  |
| 351 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Solar inspection/Tripod leg 1` | yes | MeshRenderer | VU_Brushed_aluminium |  |
| 352 | Research station | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Research station instruments (visual only)/Instrument — Solar inspection/Tripod leg 2` | yes | MeshRenderer | VU_Brushed_aluminium |  |
| 353 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 0` | yes | BoxCollider, LODGroup |  | Box |
| 354 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 0/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 355 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 0/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 356 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 0/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 357 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 10` | yes | BoxCollider, LODGroup |  | Box |
| 358 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 10/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 359 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 10/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 360 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 10/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 361 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 14` | yes | BoxCollider, LODGroup |  | Box |
| 362 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 14/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 363 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 14/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 364 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 14/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 365 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 16` | yes | BoxCollider, LODGroup |  | Box |
| 366 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 16/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 367 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 16/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 368 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 16/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 369 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 2` | yes | BoxCollider, LODGroup |  | Box |
| 370 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 2/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 371 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 2/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 372 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 2/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 373 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 22` | yes | BoxCollider, LODGroup |  | Box |
| 374 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 22/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 375 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 22/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 376 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 22/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 377 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 24` | yes | BoxCollider, LODGroup |  | Box |
| 378 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 24/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 379 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 24/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 380 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 24/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 381 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 26` | yes | BoxCollider, LODGroup |  | Box |
| 382 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 26/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 383 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 26/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 384 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 26/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 385 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 28` | yes | BoxCollider, LODGroup |  | Box |
| 386 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 28/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 387 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 28/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 388 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 28/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 389 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 30` | yes | BoxCollider, LODGroup |  | Box |
| 390 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 30/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 391 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 30/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 392 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 30/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 393 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 32` | yes | BoxCollider, LODGroup |  | Box |
| 394 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 32/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 395 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 32/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 396 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 32/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 397 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 34` | yes | BoxCollider, LODGroup |  | Box |
| 398 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 34/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 399 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 34/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 400 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 34/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 401 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 36` | yes | BoxCollider, LODGroup |  | Box |
| 402 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 36/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 403 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 36/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 404 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 36/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 405 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 38` | yes | BoxCollider, LODGroup |  | Box |
| 406 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 38/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 407 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 38/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 408 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 38/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 409 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 4` | yes | BoxCollider, LODGroup |  | Box |
| 410 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 4/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 411 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 4/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 412 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 4/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 413 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 40` | yes | BoxCollider, LODGroup |  | Box |
| 414 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 40/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 415 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 40/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 416 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 40/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 417 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 42` | yes | BoxCollider, LODGroup |  | Box |
| 418 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 42/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 419 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 42/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 420 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 42/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 421 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 50` | yes | BoxCollider, LODGroup |  | Box |
| 422 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 50/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 423 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 50/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 424 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 50/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 425 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 52` | yes | BoxCollider, LODGroup |  | Box |
| 426 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 52/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 427 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 52/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 428 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 52/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 429 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 58` | yes | BoxCollider, LODGroup |  | Box |
| 430 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 58/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 431 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 58/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 432 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 58/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 433 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 6` | yes | BoxCollider, LODGroup |  | Box |
| 434 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 6/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 435 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 6/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 436 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 6/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 437 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 60` | yes | BoxCollider, LODGroup |  | Box |
| 438 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 60/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 439 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 60/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 440 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 60/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 441 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 62` | yes | BoxCollider, LODGroup |  | Box |
| 442 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 62/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 443 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 62/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 444 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 62/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 445 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 64` | yes | BoxCollider, LODGroup |  | Box |
| 446 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 64/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 447 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 64/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 448 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 64/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 449 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 66` | yes | BoxCollider, LODGroup |  | Box |
| 450 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 66/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 451 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 66/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 452 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 66/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 453 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 68` | yes | BoxCollider, LODGroup |  | Box |
| 454 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 68/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 455 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 68/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 456 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 68/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 457 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 72` | yes | BoxCollider, LODGroup |  | Box |
| 458 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 72/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 459 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 72/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 460 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 72/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 461 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 78` | yes | BoxCollider, LODGroup |  | Box |
| 462 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 78/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 463 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 78/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 464 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 78/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 465 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 8` | yes | BoxCollider, LODGroup |  | Box |
| 466 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 8/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 467 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 8/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 468 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 8/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 469 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 82` | yes | BoxCollider, LODGroup |  | Box |
| 470 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 82/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 471 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 82/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 472 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 82/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 473 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 86` | yes | BoxCollider, LODGroup |  | Box |
| 474 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 86/LOD0` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 475 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 86/LOD1` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 476 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_07 cluster 86/LOD2` | yes | MeshRenderer | VisualUpgradeRock07 |  |
| 477 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 1` | yes | BoxCollider, LODGroup |  | Box |
| 478 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 1/LOD0` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 479 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 1/LOD1` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 480 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 1/LOD2` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 481 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 11` | yes | BoxCollider, LODGroup |  | Box |
| 482 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 11/LOD0` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 483 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 11/LOD1` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 484 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 11/LOD2` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 485 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 13` | yes | BoxCollider, LODGroup |  | Box |
| 486 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 13/LOD0` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 487 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 13/LOD1` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 488 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 13/LOD2` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 489 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 15` | yes | BoxCollider, LODGroup |  | Box |
| 490 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 15/LOD0` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 491 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 15/LOD1` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 492 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 15/LOD2` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 493 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 17` | yes | BoxCollider, LODGroup |  | Box |
| 494 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 17/LOD0` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 495 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 17/LOD1` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 496 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 17/LOD2` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 497 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 29` | yes | BoxCollider, LODGroup |  | Box |
| 498 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 29/LOD0` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 499 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 29/LOD1` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 500 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 29/LOD2` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 501 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 3` | yes | BoxCollider, LODGroup |  | Box |
| 502 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 3/LOD0` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 503 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 3/LOD1` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 504 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 3/LOD2` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 505 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 31` | yes | BoxCollider, LODGroup |  | Box |
| 506 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 31/LOD0` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 507 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 31/LOD1` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 508 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 31/LOD2` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 509 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 35` | yes | BoxCollider, LODGroup |  | Box |
| 510 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 35/LOD0` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 511 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 35/LOD1` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 512 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 35/LOD2` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 513 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 39` | yes | BoxCollider, LODGroup |  | Box |
| 514 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 39/LOD0` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 515 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 39/LOD1` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 516 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 39/LOD2` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 517 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 41` | yes | BoxCollider, LODGroup |  | Box |
| 518 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 41/LOD0` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 519 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 41/LOD1` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 520 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 41/LOD2` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 521 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 43` | yes | BoxCollider, LODGroup |  | Box |
| 522 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 43/LOD0` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 523 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 43/LOD1` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 524 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 43/LOD2` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 525 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 49` | yes | BoxCollider, LODGroup |  | Box |
| 526 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 49/LOD0` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 527 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 49/LOD1` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 528 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 49/LOD2` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 529 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 51` | yes | BoxCollider, LODGroup |  | Box |
| 530 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 51/LOD0` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 531 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 51/LOD1` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 532 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 51/LOD2` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 533 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 53` | yes | BoxCollider, LODGroup |  | Box |
| 534 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 53/LOD0` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 535 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 53/LOD1` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 536 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 53/LOD2` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 537 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 55` | yes | BoxCollider, LODGroup |  | Box |
| 538 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 55/LOD0` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 539 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 55/LOD1` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 540 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 55/LOD2` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 541 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 61` | yes | BoxCollider, LODGroup |  | Box |
| 542 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 61/LOD0` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 543 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 61/LOD1` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 544 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 61/LOD2` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 545 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 63` | yes | BoxCollider, LODGroup |  | Box |
| 546 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 63/LOD0` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 547 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 63/LOD1` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 548 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 63/LOD2` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 549 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 7` | yes | BoxCollider, LODGroup |  | Box |
| 550 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 7/LOD0` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 551 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 7/LOD1` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 552 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 7/LOD2` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 553 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 79` | yes | BoxCollider, LODGroup |  | Box |
| 554 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 79/LOD0` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 555 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 79/LOD1` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 556 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 79/LOD2` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 557 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 81` | yes | BoxCollider, LODGroup |  | Box |
| 558 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 81/LOD0` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 559 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 81/LOD1` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 560 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 81/LOD2` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 561 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 83` | yes | BoxCollider, LODGroup |  | Box |
| 562 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 83/LOD0` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 563 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 83/LOD1` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 564 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 83/LOD2` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 565 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 85` | yes | BoxCollider, LODGroup |  | Box |
| 566 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 85/LOD0` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 567 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 85/LOD1` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 568 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 85/LOD2` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 569 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 87` | yes | BoxCollider, LODGroup |  | Box |
| 570 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 87/LOD0` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 571 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 87/LOD1` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 572 | Rocks / far terrain | `Mars Research Base — 250m playable world/Approved rock_09 cluster 87/LOD2` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 573 | Rocks / far terrain | `Mars Research Base — 250m playable world/Distant rocky massif 0` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 574 | Rocks / far terrain | `Mars Research Base — 250m playable world/Distant rocky massif 1` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 575 | Rocks / far terrain | `Mars Research Base — 250m playable world/Distant rocky massif 10` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 576 | Rocks / far terrain | `Mars Research Base — 250m playable world/Distant rocky massif 11` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 577 | Rocks / far terrain | `Mars Research Base — 250m playable world/Distant rocky massif 2` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 578 | Rocks / far terrain | `Mars Research Base — 250m playable world/Distant rocky massif 3` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 579 | Rocks / far terrain | `Mars Research Base — 250m playable world/Distant rocky massif 4` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 580 | Rocks / far terrain | `Mars Research Base — 250m playable world/Distant rocky massif 5` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 581 | Rocks / far terrain | `Mars Research Base — 250m playable world/Distant rocky massif 6` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 582 | Rocks / far terrain | `Mars Research Base — 250m playable world/Distant rocky massif 7` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 583 | Rocks / far terrain | `Mars Research Base — 250m playable world/Distant rocky massif 8` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 584 | Rocks / far terrain | `Mars Research Base — 250m playable world/Distant rocky massif 9` | yes | MeshRenderer | VisualUpgradeRock09 |  |
| 585 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)` | yes |  |  |  |
| 586 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_0_0` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 587 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_0_1` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 588 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_0_2` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 589 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_0_3` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 590 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_0_4` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 591 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_0_5` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 592 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_1_0` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 593 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_1_1` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 594 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_1_2` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 595 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_1_3` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 596 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_1_4` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 597 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_1_5` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 598 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_2_0` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 599 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_2_1` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 600 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_2_2` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 601 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_2_3` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 602 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_2_4` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 603 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_2_5` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 604 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_3_0` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 605 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_3_1` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 606 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_3_2` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 607 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_3_3` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 608 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_3_4` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 609 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_3_5` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 610 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_4_0` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 611 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_4_1` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 612 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_4_2` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 613 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_4_3` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 614 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_4_4` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 615 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_4_5` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 616 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_5_0` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 617 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_5_1` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 618 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_5_2` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 619 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_5_3` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 620 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_5_4` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 621 | Rocks / far terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Surface pebble scatter (combined, no colliders)/VisualUpgradeScatter_5_5` | yes | LODGroup, MeshRenderer | VisualUpgradeRock07, VisualUpgradeRock09 |  |
| 622 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype` | yes | CharacterController |  | CharacterController |
| 623 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/Bounded rover wheel dust` | yes | ParticleSystem, ParticleSystemRenderer | VisualUpgradeWheelDust |  |
| 624 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual` | yes |  |  |  |
| 625 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Battery enclosure` | yes | MeshRenderer | Pressure shell ceramic composite |  |
| 626 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Cab` | yes | MeshRenderer | Pressure shell ceramic composite, VU_Rover_cockpit_glass |  |
| 627 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Camera sensor` | yes | MeshRenderer | Inherited_6 |  |
| 628 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Chassis` | yes | MeshRenderer | Pressure shell ceramic composite |  |
| 629 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Headlight -1` | yes | (renderer off), MeshRenderer | Inherited_6 |  |
| 630 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Headlight 1` | yes | (renderer off), MeshRenderer | Inherited_6 |  |
| 631 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Protective flank -1` | yes | MeshRenderer | Inherited_9 |  |
| 632 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Protective flank 1` | yes | MeshRenderer | Inherited_9 |  |
| 633 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/RoofMarker` | yes | MeshRenderer | VU_Rover_solar_cells |  |
| 634 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Scientific sensor mast` | yes | MeshRenderer | Inherited_3 |  |
| 635 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Suspension link -1_0` | yes | MeshRenderer | Inherited_3 |  |
| 636 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Suspension link -1_1` | yes | MeshRenderer | Inherited_3 |  |
| 637 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Suspension link -1_2` | yes | MeshRenderer | Inherited_3 |  |
| 638 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Suspension link 1_0` | yes | MeshRenderer | Inherited_3 |  |
| 639 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Suspension link 1_1` | yes | MeshRenderer | Inherited_3 |  |
| 640 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Suspension link 1_2` | yes | MeshRenderer | Inherited_3 |  |
| 641 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/TERRAVEX static rocker and protective frame` | yes | MeshRenderer | Machined graphite alloy |  |
| 642 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)` | yes |  |  |  |
| 643 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Antenna mast` | yes | MeshRenderer | VU_Rover_graphite |  |
| 644 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Boarding step` | yes | MeshRenderer | VU_Rover_graphite |  |
| 645 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Cockpit display 0` | yes | MeshRenderer | VU_Rover_screen |  |
| 646 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Cockpit display 1` | yes | MeshRenderer | VU_Rover_screen |  |
| 647 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Cockpit display 2` | yes | MeshRenderer | VU_Rover_screen |  |
| 648 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Dashboard` | yes | MeshRenderer | VU_Rover_graphite |  |
| 649 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Fender -1_0` | yes | MeshRenderer | VU_Rover_body_panel |  |
| 650 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Fender -1_1` | yes | MeshRenderer | VU_Rover_body_panel |  |
| 651 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Fender -1_2` | yes | MeshRenderer | VU_Rover_body_panel |  |
| 652 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Fender 1_0` | yes | MeshRenderer | VU_Rover_body_panel |  |
| 653 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Fender 1_1` | yes | MeshRenderer | VU_Rover_body_panel |  |
| 654 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Fender 1_2` | yes | MeshRenderer | VU_Rover_body_panel |  |
| 655 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Front bumper and winch` | yes | MeshRenderer | VU_Rover_graphite |  |
| 656 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Hand rail -1` | yes | MeshRenderer | VU_Rover_graphite |  |
| 657 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Hand rail 1` | yes | MeshRenderer | VU_Rover_graphite |  |
| 658 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Headlight housing -1` | yes | MeshRenderer | VU_Rover_graphite |  |
| 659 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Headlight housing 1` | yes | MeshRenderer | VU_Rover_graphite |  |
| 660 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Headlight lens -1` | yes | MeshRenderer | VU_Rover_headlight_lens |  |
| 661 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Headlight lens 1` | yes | MeshRenderer | VU_Rover_headlight_lens |  |
| 662 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Headlight spot -1` | yes | Light |  |  |
| 663 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Headlight spot 1` | yes | Light |  |  |
| 664 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/High-gain antenna` | yes | MeshRenderer | VU_Rover_body_panel |  |
| 665 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Mast beacon` | yes | MeshRenderer | VU_Rover_beacon |  |
| 666 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Rear equipment rack` | yes | MeshRenderer | VU_Rover_graphite |  |
| 667 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Roof solar frame` | yes | MeshRenderer | VU_Rover_graphite |  |
| 668 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Seat back -1` | yes | MeshRenderer | VU_Rover_seat |  |
| 669 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Seat back 1` | yes | MeshRenderer | VU_Rover_seat |  |
| 670 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Seat base -1` | yes | MeshRenderer | VU_Rover_seat |  |
| 671 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Seat base 1` | yes | MeshRenderer | VU_Rover_seat |  |
| 672 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Seated driver (visual only, TEMPORARY)` | **no** |  |  |  |
| 673 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Seated driver (visual only, TEMPORARY)/Driver arm -1` | yes | MeshRenderer | VU_Suit_fabric |  |
| 674 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Seated driver (visual only, TEMPORARY)/Driver arm 1` | yes | MeshRenderer | VU_Suit_fabric |  |
| 675 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Seated driver (visual only, TEMPORARY)/Driver helmet` | yes | MeshRenderer | VU_Suit_hard_shell |  |
| 676 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Seated driver (visual only, TEMPORARY)/Driver torso` | yes | MeshRenderer | VU_Suit_fabric |  |
| 677 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Seated driver (visual only, TEMPORARY)/Driver visor` | yes | MeshRenderer | VU_Visor_gold_coating |  |
| 678 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Shock absorber -1_0` | yes | MeshRenderer | VU_Rover_graphite |  |
| 679 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Shock absorber -1_1` | yes | MeshRenderer | VU_Rover_graphite |  |
| 680 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Shock absorber -1_2` | yes | MeshRenderer | VU_Rover_graphite |  |
| 681 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Shock absorber 1_0` | yes | MeshRenderer | VU_Rover_graphite |  |
| 682 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Shock absorber 1_1` | yes | MeshRenderer | VU_Rover_graphite |  |
| 683 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Shock absorber 1_2` | yes | MeshRenderer | VU_Rover_graphite |  |
| 684 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Shock spring -1_0` | yes | MeshRenderer | VU_Accent_orange |  |
| 685 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Shock spring -1_1` | yes | MeshRenderer | VU_Accent_orange |  |
| 686 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Shock spring -1_2` | yes | MeshRenderer | VU_Accent_orange |  |
| 687 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Shock spring 1_0` | yes | MeshRenderer | VU_Accent_orange |  |
| 688 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Shock spring 1_1` | yes | MeshRenderer | VU_Accent_orange |  |
| 689 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Shock spring 1_2` | yes | MeshRenderer | VU_Accent_orange |  |
| 690 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Side identification stripe -1` | yes | MeshRenderer | VU_Accent_orange |  |
| 691 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Side identification stripe 1` | yes | MeshRenderer | VU_Accent_orange |  |
| 692 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Side storage locker -1` | yes | MeshRenderer | VU_Rover_body_panel |  |
| 693 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Side storage locker 1` | yes | MeshRenderer | VU_Rover_body_panel |  |
| 694 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Tail light -1` | yes | MeshRenderer | VU_Rover_tail_light |  |
| 695 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)/Tail light 1` | yes | MeshRenderer | VU_Rover_tail_light |  |
| 696 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Wheel_-1_0` | yes | (renderer off), LODGroup, MeshRenderer | Inherited_16 |  |
| 697 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Wheel_-1_0/Detailed wheel LOD0` | yes | MeshRenderer | Brushed aluminum, Dusty tire elastomer |  |
| 698 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Wheel_-1_0/Detailed wheel LOD1` | yes | MeshRenderer | Brushed aluminum, Dusty tire elastomer |  |
| 699 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Wheel_-1_0/Detailed wheel LOD2` | yes | MeshRenderer | Brushed aluminum, Dusty tire elastomer |  |
| 700 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Wheel_-1_1` | yes | (renderer off), LODGroup, MeshRenderer | Inherited_23 |  |
| 701 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Wheel_-1_1/Detailed wheel LOD0` | yes | MeshRenderer | Brushed aluminum, Dusty tire elastomer |  |
| 702 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Wheel_-1_1/Detailed wheel LOD1` | yes | MeshRenderer | Brushed aluminum, Dusty tire elastomer |  |
| 703 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Wheel_-1_1/Detailed wheel LOD2` | yes | MeshRenderer | Brushed aluminum, Dusty tire elastomer |  |
| 704 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Wheel_-1_2` | yes | (renderer off), LODGroup, MeshRenderer | Inherited_4 |  |
| 705 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Wheel_-1_2/Detailed wheel LOD0` | yes | MeshRenderer | Brushed aluminum, Dusty tire elastomer |  |
| 706 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Wheel_-1_2/Detailed wheel LOD1` | yes | MeshRenderer | Brushed aluminum, Dusty tire elastomer |  |
| 707 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Wheel_-1_2/Detailed wheel LOD2` | yes | MeshRenderer | Brushed aluminum, Dusty tire elastomer |  |
| 708 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Wheel_1_0` | yes | (renderer off), LODGroup, MeshRenderer | Inherited_22 |  |
| 709 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Wheel_1_0/Detailed wheel LOD0` | yes | MeshRenderer | Brushed aluminum, Dusty tire elastomer |  |
| 710 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Wheel_1_0/Detailed wheel LOD1` | yes | MeshRenderer | Brushed aluminum, Dusty tire elastomer |  |
| 711 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Wheel_1_0/Detailed wheel LOD2` | yes | MeshRenderer | Brushed aluminum, Dusty tire elastomer |  |
| 712 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Wheel_1_1` | yes | (renderer off), LODGroup, MeshRenderer | Inherited_24 |  |
| 713 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Wheel_1_1/Detailed wheel LOD0` | yes | MeshRenderer | Brushed aluminum, Dusty tire elastomer |  |
| 714 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Wheel_1_1/Detailed wheel LOD1` | yes | MeshRenderer | Brushed aluminum, Dusty tire elastomer |  |
| 715 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Wheel_1_1/Detailed wheel LOD2` | yes | MeshRenderer | Brushed aluminum, Dusty tire elastomer |  |
| 716 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Wheel_1_2` | yes | (renderer off), LODGroup, MeshRenderer | Inherited_13 |  |
| 717 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Wheel_1_2/Detailed wheel LOD0` | yes | MeshRenderer | Brushed aluminum, Dusty tire elastomer |  |
| 718 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Wheel_1_2/Detailed wheel LOD1` | yes | MeshRenderer | Brushed aluminum, Dusty tire elastomer |  |
| 719 | Rover | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype/RoverVisual/Wheel_1_2/Detailed wheel LOD2` | yes | MeshRenderer | Brushed aluminum, Dusty tire elastomer |  |
| 720 | Rover | `02_LocalProvisionalExploration/ThirdPerson_RoverFollowCamera` | yes | AudioListener, Camera, MonoBehaviour |  |  |
| 721 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm` | yes |  |  |  |
| 722 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Panel support` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 723 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Panel support` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 724 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Panel support` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 725 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Panel support` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 726 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Panel support` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 727 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Panel support` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 728 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Panel support` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 729 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Panel support` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 730 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Panel support` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 731 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Panel support` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 732 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Panel support` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 733 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Panel support` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 734 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Panel support` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 735 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Panel support` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 736 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Panel support` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 737 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 0_0` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 738 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 0_0/Photovoltaic cells` | yes | MeshRenderer | VisualUpgradePhotovoltaic |  |
| 739 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 0_1` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 740 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 0_1/Photovoltaic cells` | yes | MeshRenderer | VisualUpgradePhotovoltaic |  |
| 741 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 0_2` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 742 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 0_2/Photovoltaic cells` | yes | MeshRenderer | VisualUpgradePhotovoltaic |  |
| 743 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 0_3` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 744 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 0_3/Photovoltaic cells` | yes | MeshRenderer | VisualUpgradePhotovoltaic |  |
| 745 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 0_4` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 746 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 0_4/Photovoltaic cells` | yes | MeshRenderer | VisualUpgradePhotovoltaic |  |
| 747 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 1_0` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 748 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 1_0/Photovoltaic cells` | yes | MeshRenderer | VisualUpgradePhotovoltaic |  |
| 749 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 1_1` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 750 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 1_1/Photovoltaic cells` | yes | MeshRenderer | VisualUpgradePhotovoltaic |  |
| 751 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 1_2` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 752 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 1_2/Photovoltaic cells` | yes | MeshRenderer | VisualUpgradePhotovoltaic |  |
| 753 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 1_3` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 754 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 1_3/Photovoltaic cells` | yes | MeshRenderer | VisualUpgradePhotovoltaic |  |
| 755 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 1_4` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 756 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 1_4/Photovoltaic cells` | yes | MeshRenderer | VisualUpgradePhotovoltaic |  |
| 757 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 2_0` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 758 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 2_0/Photovoltaic cells` | yes | MeshRenderer | VisualUpgradePhotovoltaic |  |
| 759 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 2_1` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 760 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 2_1/Photovoltaic cells` | yes | MeshRenderer | VisualUpgradePhotovoltaic |  |
| 761 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 2_2` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 762 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 2_2/Photovoltaic cells` | yes | MeshRenderer | VisualUpgradePhotovoltaic |  |
| 763 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 2_3` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 764 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 2_3/Photovoltaic cells` | yes | MeshRenderer | VisualUpgradePhotovoltaic |  |
| 765 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 2_4` | yes | BoxCollider, MeshRenderer | Inherited_3 | Box |
| 766 | Solar farm | `Mars Research Base — 250m playable world/Zone C — solar farm/Solar module 2_4/Photovoltaic cells` | yes | MeshRenderer | VisualUpgradePhotovoltaic |  |
| 767 | Systems | `02_LocalProvisionalExploration` | yes | MonoBehaviour |  |  |
| 768 | Systems | `Adaptive touch input provider` | yes | MonoBehaviour |  |  |
| 769 | Systems | `Configurable Mars presentation` | yes | MonoBehaviour |  |  |
| 770 | Systems | `Native control and provisional telemetry` | yes | MonoBehaviour |  |  |
| 771 | Systems | `Production presentation adapters — protected gameplay` | yes | MonoBehaviour |  |  |
| 772 | Systems | `Restrained color grade` | yes | MonoBehaviour |  |  |
| 773 | Terrain | `Mars Research Base — 250m playable world/Distant dust plain` | yes | (renderer off), MeshRenderer | Inherited_20 |  |
| 774 | Terrain | `Mars Research Base — 250m playable world/Traversable terrain` | yes | MeshCollider, MeshRenderer | VisualUpgradeMarsGround | Mesh |
| 775 | Terrain | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Far terrain — dunes and craters (visual only)` | yes | MeshRenderer | VisualUpgradeMarsGround |  |
| 776 | UI | `02_LocalProvisionalExploration/MissionHUD_Prototype` | yes | Canvas, MonoBehaviour |  |  |
| 777 | UI | `02_LocalProvisionalExploration/MissionHUD_Prototype/InstrumentPanel` | yes | CanvasRenderer, MonoBehaviour |  |  |
| 778 | UI | `02_LocalProvisionalExploration/MissionHUD_Prototype/InstrumentPanel/Readouts` | yes | CanvasRenderer, MonoBehaviour |  |  |
| 779 | UI | `AstroGateway cinematic HUD adapters` | yes | MonoBehaviour |  |  |
| 780 | UI | `HUD EventSystem` | yes | MonoBehaviour |  |  |
| 781 | VFX | `02_LocalProvisionalExploration/BoundedLocalDust_NoVolumetrics` | yes | ParticleSystem, ParticleSystemRenderer | Inherited_25 |  |
| 782 | VFX | `Mars Research Base — 250m playable world/Visual Upgrade v1 — environment (temporary art, no colliders)/Distant dust haze (follows player, presentation only)` | yes | ParticleSystem, ParticleSystemRenderer | VisualUpgradeHaze |  |
