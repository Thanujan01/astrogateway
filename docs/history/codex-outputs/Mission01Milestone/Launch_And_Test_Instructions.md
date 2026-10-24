# Launch and manual testing

Open the existing project with Unity 6000.6.4f1:
```sh
"/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity" -projectPath "/Users/kapilash/Projects/MissionRedHorizon"
```
If already open, reuse that Editor. Stop Play Mode first. Preserve any unsaved work under a separate backup scene before changing scenes; do not overwrite approved scenes. Open Assets/AstroGateway/Mission01Milestone/Mission01_EarthLaunch_Milestone_v4.unity, or use AstroGateway → Mission01 → Open First Milestone.

Click Play, then New Mission. Enter a local callsign of 2–32 characters, select suit color, Confirm and Continue. Read the briefing, then Enter Earth Launch Site. Click Game View and walk straight toward the orange console in front of the rocket. WASD walk, Shift run, Space jump, RMB camera orbit, E confirm boarding within 2.8 m, Esc pause. Completion should show a saved local boarding checkpoint. Return to menu and Continue Mission should restore it.

Local save: Application.persistentDataPath/AstroGateway_Mission01_Local.json. This is separate from Auth profiles and existing mission data; no password, token or Supabase UUID is stored. Saves are version/shape/name/color validated but not cryptographically authoritative. Do not use local progress as an approved server score.

Manual checklist still required:
- Click every menu button at PC and small window resolutions.
- New Mission → setup → briefing → Earth launch → E completion.
- Confirm out-of-range E rejection; test diagonal, sprint, jump, walls, RMB and camera occlusion.
- Pause freezes movement; resume and menu return.
- Restart Play Mode and validate Continue; inspect audio volume and fades.
- Confirm shared Web/Android UI and Mars mission remain accessible through their existing launch paths.

Automated methods are in Editor/Mission01Builder.cs. BuildAndTest refuses to overwrite an existing milestone scene; run only in a disposable validation copy with its generated scene absent. Do not run it against the main scene hoping to rebuild it. Test output path is this workspace's outputs/Mission01Milestone. No build settings were changed and no new APK/PC executable was produced.
