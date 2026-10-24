# Batch 3 — 3D objects and animation (plan, 2026-10-19)

Go-ahead: SSL in the project chat, 2026-10-19 ("objects 3d animetions … best aha improve" — improve the 3D objects and animations as much as possible).
Scope rules kept: work only in `Assets/AstroGateway/VisualUpgrade/` (builder scripts, Visual Upgrade presentation scripts, generated meshes/materials) and the isolated `Mars_VisualUpgrade_v1` scene, which the builder regenerates from the untouched Production Visual scene. No gameplay, controller, collider, camera, scoring, backend or project-wide URP change. Placeholder art stays labelled TEMPORARY; production astronaut/rover still need design-team assets (spec in `Implementation_Plan.md`).

## Confirmed defects found while planning (with cause)
| ID | Defect | Cause (verified in code/scene) | Fix |
|---|---|---|---|
| 3D-01 | Black/white striped artifact on comms dish, rover high-gain antenna, station relay dishes | `VisualUpgradeKit.Dish()` makes the bowl double-sided by re-using the same vertices with reversed triangles, so `RecalculateNormals` averages front and back normals to ~zero → broken lighting and shadow acne | separate back-face vertices with flipped normals; add a rim |
| 3D-08 | Rover body never sways (pitch on accel/brake, roll in turns) although the code exists | `VisualUpgradeRoverPresentation.body` is never assigned by the builder (`body: {fileID: 0}` in the scene) | add a body-sway pivot holding chassis/cab/detail (wheels stay direct children, as `MarsPrototype` rolls them by name) and wire it |

## Animation upgrades (visual only; read MarsPrototype/HUD state, never write it)
- **Astronaut gait:** knee and elbow joints (child pivots under the existing limb pivots), Mars low-gravity gait (longer float, softer landing), foot lift, counter-rotating shoulders/hips, head that settles toward the camera heading, smoother idle↔walk↔run blend, breathing idle.
- **Scan pose:** while `hud.scanning`, the right arm raises the scanner toward the station and the helmet tilts to it; returns on finish/cancel.
- **Footstep dust:** small bounded puffs at foot plants (one shared particle system, no colliders).
- **Rover:** body sway wired (3D-08), visual suspension bob per wheel side, front/rear wheel steer angle from yaw rate (visual child pivot so the gameplay roll is untouched), antenna sway, high-gain antenna slow tracking, cockpit screen flicker.
- **Stations:** status light state (idle amber → scanning pulsing cyan → recorded green), expanding scan ring during a scan.

## 3D object upgrades
- Dish meshes rebuilt (3D-01) with rim and correct normals.
- Astronaut: segmented torso hard-upper-torso shell, helmet neck seal, glove fingers block, boot treads, hose routing on chest.
- Rover: wheel hub caps and tread blocks on the existing LOD wheels' children (visual), cab grab handles, chassis bevel strip.
- Stations/habitats: panel-seam variation and decals deferred unless time allows (material changes need their own captures).

## Verification per step
47-check playtest, responsive HUD audit, real captures (astronaut front/back, walk/run frames, scan pose, rover exterior/cockpit, station scanning, comms dish, habitat), Mac standalone probe vs batch-2 numbers. Commit locally only after verification.

## Results (2026-10-20)
Done in this batch: 3D-01 dish normals (comms tower, rover high-gain antenna, 5 station relay dishes), 3D-08 rover body sway wired, per-wheel visual suspension (±9 cm, raycast to ground), camera-sensor nod and high-gain antenna tracking, cockpit screen flicker; astronaut knee and elbow joints, Mars lope, scan pose, idle sway, footstep dust; station status lights (amber / cyan pulse / green) and scan ring.
Not done: astronaut torso/helmet/glove geometry detail, rover hub caps, habitat panel variation (deferred; still placeholders pending design-team assets).

Files: Scripts `VisualUpgradeAstronautMotion.cs`, `VisualUpgradeRoverPresentation.cs`, new `VisualUpgradeStationStatus.cs`; Editor `VisualUpgradeKit.cs`, `VisualUpgradeCharacters.cs`, `VisualUpgradeProps.cs`, `VisualUpgradeCapture.cs` (shots 11–14). Scene and its generated meshes/materials/settings rebuilt by `VisualUpgradeBuilder.Build` from the untouched Production Visual scene (material/settings diffs are regenerated sub-asset ids; scene GUID kept).

Verification (`Docs/VisualUpgrade/Batch3/`): scene build PASS; playtest **47/47**, 0 errors; responsive HUD audit 0 issues at 3 sizes; real captures Before/After 01–14 and side-by-side in `BeforeAfter/` (14 = comms dish: stripes gone; 12 = running with bent knees and dust; 13 = scan pose). Animation smoothness was not judged by a person; rover sway/suspension are motion effects that still frames cannot show. Mac standalone not re-profiled for this batch (≈20 extra low-poly parts, 5 hidden rings, one 80-particle system).
