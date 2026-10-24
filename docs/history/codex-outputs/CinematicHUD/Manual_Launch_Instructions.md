# Launch the isolated HUD scene

```sh
"/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity" -projectPath "/Users/kapilash/Projects/MissionRedHorizon"
```

If Unity is already open, use that instance. Preserve any unsaved work before changing scenes. Open:

`/Users/kapilash/Projects/MissionRedHorizon/Assets/AstroGateway/CinematicHUD/Scenes/Mars_CinematicHUD_v1.unity`

Enter Play Mode and focus Game View. WASD walk/drive; Shift run; RMB orbit; E enter/exit near rover; Space brake; K starts scanning when an existing target is within 3m; 1/2/3 weather. Existing E equipment scan behavior is retained. Escape opens/closes the pause dialog; F1 cycles Full/Compact/Minimal/Hidden. Hidden mode has a restore button.

Mobile-style controls: joystick, separate camera drag zone, run/accelerate hold, brake hold, contextual board/exit/inspect and scan. Settings opens actual sliders/profile toggles. MAP opens a paused world map; BAG/Inventory displays the actual list or empty state. Rover/Base shortcuts select actual waypoints and show details; Samples opens local records. Pause/details include Save Local / Load Local / Export Local. Exit the rover only after stopping with sufficient clearance.

All mission events and scientific values remain local simulated/provisional. Save path is a separate CinematicHUD snapshot namespace; earlier scene saves remain untouched and are not automatically migrated. No cloud synchronization, real Sol clock, radiation/food/water model or ORBIS upload is included.

Reusable prefab requires the paired scene adapters and theme, exactly one HUD instance. Existing scenes are not automatically converted.
