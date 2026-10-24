# Manual launch
Open existing project `/Users/kapilash/Projects/MissionRedHorizon` in Unity Hub with Editor 6000.6.4f1. Open `Assets/AstroGateway/ProductionVisual/Scenes/Mars_ProductionVisual_v1.unity` and press Play. Do not overwrite an earlier scene or change build settings for this preview.

Equivalent project launch:
```sh
/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity -projectPath /Users/kapilash/Projects/MissionRedHorizon
```

WASD moves; Shift runs; RMB orbits; E boards/exits when near/stopped and exit clear; Space brakes; K scans; 1/2/3 weather; C overview/return; Esc pause/back; F1 HUD visibility. On-screen joystick/camera zone and context actions share original commands. CHASE DISTANCE toggles rover framing. CONTROL TUNE configures dead zone/opacity/button size; SET configures common HUD and graphics.

Approach existing research targets within3m and use SCAN; reports are labelled simulated. SAVE/LOAD/EXPORT remain local. Habitat interiors sealed. Future robot unavailable. Mission events provisional.

Tests ran in the cached isolated validation project `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/work/stage08b/PlayModeLab`. Logs/captures under `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/ProductionVisual`. Do not copy its modified legacy test harnesses into main. ProductionVisualBuilder.Build refuses overwrite; reruns should use Validate, not Build. Build/test automation is Editor-only and uses the output path recorded in its source.
