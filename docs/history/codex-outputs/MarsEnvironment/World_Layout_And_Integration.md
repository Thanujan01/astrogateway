# World layout and integration

1 Unity unit = 1m. Playable terrain is 250×250m; unchanged controller bounds limit active traversal to about 238×238m (±119m), with a margin. Decorative 1km ground and distant massifs extend the view but are not playable. Terrain heights are authored noise/crater functions, not DEM data or erosion simulation.

|Zone|Approximate location (x,z) m|Purpose|
|---|---|---|
|A Habitat|Crew (-60,-56), Lab (-31,-38), Life support (-65,-27)|Closed living/science/service architecture|
|B Rover parking|(-15,-93)|Board/drive/exit; existing rover retained|
|C Solar farm|(37,-57), power cabinet (25,-45)|15 panels and simulated inspection|
|D Sampling field|(35,23), crater (65,37)|Rock/soil observations|
|E Exploration ridge|(0,74), rising north terrain|Ridge target and views|
|F Communications/storage|(-16,-24)|Mast, dish, crates and inspection|

Astronaut spawn (-42,-68). Graded construction apron and broad open routes retain legacy target coordinates habitat (-45,-60), power (25,-45), delta (0,75). No full route-clearance proof or all-target human traversal was executed in this scene; manual QA is required.

Dome radii/heights: 11/8m crew, 9/7m lab, 6/5m life support. Sealed doors display exterior-only feedback nearby. Sphere colliders are conservative closed-structure proxies rather than exact interior geometry. Solar modules are 4×2.8m tilted southward with supports; cabinet/tank dimensions are conceptual and not engineering-certified.

The new scene is derived from NativeControlSlice, replacing only copied scenery and retaining gameplay objects/references. Added renderer details do not replace actors or driving logic. New-scene rover collider alignment is a serialized override. Original scene files/prefabs/scripts remain intact.

Lighting/presentation adapter runs after existing weather updates, composes fog/dust, adds a bounded 100-particle rover trail, and controls a cloned runtime pipeline. Original local dust remains capped at 240 particles. Quality controls alter rates, not a promise of frame-rate improvement. No realtime volumetrics, SSAO or bloom added; grade is restrained color/tone mapping.

Native telemetry, scan records and exports remain provisional. Research target association adds stable local field IDs; weather-dependent solar index is synthetic 1/0.6/0.15, not watts or measured habitat output. No Stage07 remote-schema compatibility or backend submission is claimed.

Save namespace and cloned mission configuration are separate from NativeSlice originals. Pause disables input provider updates, clears inputs and stops Time.timeScale; resume restores mode/time. Scenic view locks inputs but is not itself a survival pause; use Pause to freeze simulation.
