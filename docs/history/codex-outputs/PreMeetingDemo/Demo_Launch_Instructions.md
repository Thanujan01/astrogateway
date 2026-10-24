# Demo launch instructions

## Main playable game — primary presentation

The main Editor is already open on the existing scene. Do not save the pre-existing unsaved scene changes during demo preparation. Click Play manually, click the Game View, then use WASD, Shift, RMB orbit, E near rover, and keys 1/2/3 for clear/light dust/storm. Mission targets are habitat (-45,0,-60), power (25,0,-45) and delta (0,0,75); terrain adjusts the walkable elevation. Show HUD and provisional completion. Stop Play afterward. These are implemented controls; combined-key/orbit coverage and camera occlusion still require operator checks.

If Editor is closed:
```sh
"/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity" -projectPath "/Users/kapilash/Projects/MissionRedHorizon"
```
Open Assets/AstroGateway/Stage08B/MarsOutpost_Playable_Stage08B.unity in Project view. Do not open a second Editor against the same project.

## Separate approved-asset visual preview

```sh
"/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity" -projectPath "/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/work/stage08d1/VisualEvaluation"
```
Open Assets/PreMeeting/PreMeeting_Mars_Visual_Preview_v2.unity. This is an isolated robotic rover/ground/rocks visual scene, not the playable astronaut mission. In Play Mode its four buttons are intended to select presets, but live-button interaction is not yet tested. For a reliable meeting fallback show Screenshots/Preview_2.png and Graphics_Validation.txt.

## Web UI

Currently running: http://127.0.0.1:5173/welcome
Routes: /signup, /login, /forgot-password, /mars. Show UI without submitting live accounts. Dashboard/profile require real authentication; do not bypass them for a demo.

Restart only if needed:
```sh
cd /Users/kapilash/Projects/astrogateway
npm run dev -- --host 127.0.0.1 --port 5173 --strictPort
```

## Android native UI

Connected emulator: emulator-5556. Existing installed host:
```sh
"/Applications/Unity/Hub/Editor/6000.6.4f1/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb" -s emulator-5556 shell am start -n com.sk2lb.astrogateway.poc/.MainActivity
```
Existing debug APK: /Users/kapilash/Projects/AstroGatewayAndroid/app/build/outputs/apk/debug/app-debug.apk. It was not rebuilt or reinstalled in this task. Do not promise authenticated Unity entry without a verified session.

## Local backend

Currently running http://127.0.0.1:8000/health. If stopped, preserve the confined launcher:
```sh
cd "/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp"
work/stage05b/venv/bin/python work/unifiedauth/start_api.py
```
Do not start a second server on occupied port 8000. The existing launcher has restricted local gateway egress; health success does not establish hosted Supabase access. No administrative credentials should enter the runtime.

## Suggested 6-minute presentation

1. Web and Android account UI (1 minute).
2. Main Editor playable prototype, operator-driven (3 minutes); use prior clearly labeled screenshots if UI fails.
3. Separate detailed rover/rocks preview and preset evidence (1 minute).
4. State blockers and next priorities (1 minute).
