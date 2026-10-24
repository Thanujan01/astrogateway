# Explorer upgrade — Stage 1 audit (2026-10-21)

Project `~/Projects/MissionRedHorizon`, branch `visual-upgrade` @ `5e0c223`. Tracked files are clean and nothing else was running. Backup: `~/Projects/MissionRedHorizon_Backups/Explorer_PreChange_20261021/`.
Active scenes: First Mission v1 (`Mars_FirstMission_v1`, original HUD) and HUD v2 (`Mars_FirstMission_HUDv2`, premium HUD). This upgrade goes into a **new scene `Mars_FirstMission_Explorer`**, so v1 and v2 stay as rollback points.

## Current vs missing
| Area | What exists today | Gap for this request |
|---|---|---|
| HUD | HUD v2 always shows a top bar with 5 tabs, a mission card, a rover status card and a navigation card | Clutter; needs a compact HUD, a map icon and a ⋯ drawer |
| Map | A small minimap in the nav card, a one-time top-down render of the real scene | No fullscreen map, gestures, layers, selection or routes |
| Locations | 5 research stations (`ResearchTarget`, ids site_0…4). Placed objects found by name: Crew Dome, Laboratory Dome, Life Support Dome (+ sealed airlocks), habitat connector, solar farm (Zone C, 15 modules), power distribution cabinet, communications mast + reflector, 4 life-support tanks, 4 storage crates, rover start point | No registry. **Not in the world:** research tents, a power generator, a charging station, repair locations, rover dock, checkpoints. Those can only be listed as **proposals** |
| Routing | **No NavMesh** (scene NavMeshData empty). Colliders exist on the terrain, domes, stations, rocks and solar farm | Build a traversability grid (raycasts and overlaps once at load) + A* route; real obstacle avoidance, not a straight line |
| Navigate to rover | Board/exit within 5 m (existing rule), BOARD button | No waypoint, route, distance or arrival guidance |
| Astronaut controls | Joystick, run (hold/toggle, auto-sprint), context interact, camera views, drag-look, settings | No navigate-to-rover or map/menu access; jump still needs approval |
| Rover | **Kinematic CharacterController** (`MarsPrototype.Drive`): max 3.8 m/s forward = 13.7 km/h, 1.8 m/s reverse, ramps 2 m/s², brake 6 m/s², yaw 48°/s scaled by speed, gravity 3.71. **No wheel colliders, traction, torque or rollover model.** The six ARES-VI wheels, rocker and bogie are visual only (they roll, and tilt to ground raycasts). The displayed speed is the controller's commanded speed, which matches motion within ~0.05 m/s | Drive profiles: PRECISION and NORMAL are possible as input profiles. **TRAVERSE (faster) needs a change to the shared controller's hard-coded 3.8 m/s → BLOCKED pending your approval.** Speed display will use measured motion |
| Science | Rover instruments → sensor records (SIMULATED / UNAVAILABLE), local queue, AI NOT CONNECTED. On-foot scan = existing synthetic scan (no measurement values) | No result popup, record icon or record view |
| Repair | No repairable state anywhere in the game | A repair state machine would be new gameplay → proposal only |
| Android | Landscape, safe area handled by the HUD, CanvasScaler 1920×1080 match height | — |
| Tests | First Mission playtest (v1 155/0, HUD v2 187/0), Visual Upgrade regression 50/0, backend 232/232, local client 12/12 | New tests for map, navigation, profiles and popups |

## Plan
1. **Stage 2:** compact HUD (thin compass, small objective chip, map icon, ⋯ systems drawer). The cards move into the drawer.
2. **Stage 3:** fullscreen circular map (pan, pinch, double-tap, reset, centre on astronaut or rover, north-up or heading-up, compass, scale bar, layers, selectable markers) + location registry from real scene objects.
3. **Stage 4:** traversability grid + A* routes with statuses (available, calculating, blocked, unreachable, unavailable); waypoint HUD with distance and progress; NAVIGATE TO ROVER; arrival → existing BOARD.
4. **Stages 5–6:** astronaut layout additions; drive profiles PRECISION/NORMAL (TRAVERSE shown as needing approval); measured speed.
5. **Stage 7:** scan result popup (4 s) → record icon → detailed record view.
6. **Stage 8:** infrastructure markers, with proposals listed separately.
7. **Stage 9:** tests, screenshots, phone build.

Blocked and needing your decision: TRAVERSE speed above 13.7 km/h (shared controller change), jump/crouch (movement physics), repair gameplay and any new buildings (design proposals only).
