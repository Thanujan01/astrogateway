# Cinematic HUD architecture — 2026-10-16

Local Unity 6000.6.4f1 / existing URP and uGUI/Input System dependencies. No added packages. The source is Mars_VisualPolish_v2, saved under a new path. All existing scene files remain unchanged.

## Ownership and adapters

- CinematicGameAdapter subclasses NativeSliceGame, suppressing legacy OnGUI. Serialized public field references are transferred and scene references remapped only in the new scene. Astronaut/rover GameObjects, CharacterControllers, animation references and seat remain the existing instances; no player is spawned on mode changes.
- The adapter caches delegates to the existing private Start/Update methods. The original update executes with its old input polling disabled, preserving battery, clock, telemetry and public gameplay functions. HUDInputRouter submits the sole input vector to the existing ControlModeManager before the original controller consumes it.
- CinematicEnvironmentAdapter suppresses legacy OnGUI and delegates its existing Start/Update/LateUpdate, retaining weather, research association, quality and pause logic.
- Old AdaptiveTouch is disabled in the new scene. Its reference remains for source pause/cleanup compatibility. MarsPrototype's old mobile/input path remains disabled by the original NativeSliceGame startup.
- AstroGatewayHUDController creates actual uGUI components once at startup. There is no screenshot background, image-generated UI, IMGUI replacement HUD or per-frame hierarchy reconstruction.
- HUDTelemetryAdapter subscribes to existing telemetry/mode events and exposes IResourceTelemetryProvider / ResourceTelemetrySnapshot. State without source events is sampled at configurable 1–10Hz (default 5). Resource fill interpolation does not recreate widgets.
- Resource and map subcanvases isolate changing geometry. Foreground raycasters block camera gestures. The minimap uses a generated terrain-height schematic and world-position icons, not an extra camera.

## Compatibility risk

The two lifecycle adapters intentionally depend on private methods because the original components couple their input/update and OnGUI methods and must remain unmodified. This is a version-pinned integration seam, not a stable public Unity extension API. Local tests validate this exact Editor version. link.xml preserves the original methods for player stripping, but an Android IL2CPP build/device run is still required. Revalidate after changing those original classes or upgrading Unity. Do not call this production/mobile validated.

## Prefab and theme

Prefabs/AstroGatewayCinematicHUD.prefab contains the reusable bootstrap components and theme reference. The Canvas is generated at runtime; it is not a fully designer-authored nested Canvas prefab. Use exactly one instance and bind the paired game/environment adapters (or allow their fallback lookup). It is not a drop-in replacement for legacy scenes without the scene adapter conversion. Missing configuration logs an explicit error and disables the input adapter.

Themes/AstroGatewayHUDTheme.asset stores colors, opacity, font and four original sprites. Layout is anchored in code. UI settings are stored under a unique PlayerPrefs key; no authentication or credentials are accessed.
