# Rover presentation — PARTIAL
Original kinematic driving/boarding/exit, collision, telemetry and astronaut identity are retained. Original wheel renderers are disabled only in the new scene; six new wheel LOD Groups are children of the existing wheel transforms. New chamfered chassis/cab enclosures, original mechanical frame, grouser tires, rear battery box and comms support add geometry rather than just recolouring primitives.

| Shared wheel mesh | Triangles per wheel | Reduction vs LOD0 |
|---|---:|---:|
| LOD0 | 832 | — |
| LOD1 | 512 | 38.5% |
| LOD2 | 256 | 69.2% |

Six-wheel wheel-only totals: 4,992 / 3,072 / 1,536 triangles. Thresholds .20/.075/.02 screen-relative height; not distance in metres. Static mechanical frame 384 triangles; enclosure mesh 24 triangles. Detailed inventory: New_Mesh_Inventory.txt.

Wheel visual roll now derives from actual signed forward displacement, not commanded speed; radius .3744m. Large teleports are excluded. Controlled test recorded 2.395157m travel and 0.0000305° angle difference from expected accumulated travel. This does not model wheel slip, articulated suspension, or per-wheel contact.

Static rocker links do not follow terrain independently because existing physics supplies no suspension state. Rover cab/glazing remain opaque stylized surfaces. No transmission/refraction, NASA scientific equipment integration, cockpit view or production wear textures. Rendered close-up remains visibly procedural; realistic rover acceptance is NOT achieved. Original source assets and physics are preserved.
