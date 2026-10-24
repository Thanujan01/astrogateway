# Launch instructions

Existing project /Users/kapilash/Projects/MissionRedHorizon, Unity 6000.6.4f1 / URP 17.6.0.

```sh
"/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity" -projectPath "/Users/kapilash/Projects/MissionRedHorizon"
```
Reuse an already open Editor. Preserve unsaved work under a separate backup before switching. Open /Users/kapilash/Projects/MissionRedHorizon/Assets/AstroGateway/MarsEnvironment/Scenes/Mars_RealisticResearchBase_v1.unity, or AstroGateway → Mars Environment → Open Research Base. Click Play and focus Game View.

WASD move/steer, Shift run, RMB orbit, E board/exit, Space brake, K scan, keys 1/2/3 weather. Near an orange research station, E records a range-gated scan when away from the rover; mobile SCAN/K also associates nearby targets. Stop before exit. Use native data panel for scans/save/load/export. Use left panel for lighting, graphics, Pause/menu and scenic camera; resume gameplay camera afterward.

The actor starts (-42,-68); rover (-15,-93). Closed habitats are southwest; solar southeast; sampling east; ridge north. Doors are sealed, no functional interiors. Runtime save file is Application.persistentDataPath/AstroGateway_MarsResearchBase_v1.json; original native-slice/Mission01 saves remain separate.

Mission names/settings: Documentation/MissionRegistry.asset. Lighting presets: Lighting/*.asset. Do not casually change stable mission ID after creating snapshots. Original scenes remain at their prior paths and are not replaced in build settings or Android host.

Controlled developer methods are in Tests/Editor/MarsResearchBaseBuilder.cs. Use a disposable validation copy: Build refuses overwrite, repair methods are development-only, ValidateExisting creates test outputs and can save new-scene setup references. No new APK, Web build, release executable or backend deployment was produced.

Backups: work/marsenvironment/Backup; hashes: work/marsenvironment/Before_Hashes.json. Do not restore over later operator work automatically. New_Files.json and Preservation_Check.json record integration.
