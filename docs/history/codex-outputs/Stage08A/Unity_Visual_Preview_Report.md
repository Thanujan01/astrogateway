# Unity visual preview — Stage08A

Date: 2026-10-09. **Play Mode launched successfully** in the actual MissionRedHorizon project. This is an environment-art preview, not a completed playable astronaut mission.

Project: `/Users/kapilash/Projects/MissionRedHorizon`
Scene: `/Users/kapilash/Projects/MissionRedHorizon/Assets/AstroGateway/Scenes/MarsOutpost_Environment_v01.unity`
Editor: Unity 6.6 / 6000.6.4f1, matching ProjectVersion.txt. URP:17.6.0, installed package version matches manifest. Metal Editor; active platform Web. Existing editor already held this scene and was initially in Play Mode; a redundant CLI launch exited. We selected the existing correct Editor, exited Play Mode, entered a fresh Play Mode using Command-P and observed its blue Stop button and actual rendered Game View. No Editor upgrade or project conversion.

## Actually visible

Game View: ochre regolith and worn route surface, pale habitat/airlock with cyan edge accents and mast, greenhouse module, cyan route posts, distant layered delta/low relief, warm sky, directional sunlight and shadows. Six fresh URP camera renders cover arrival, habitat, solar/power, delta, return and overview. Procedural structures and geology are an early environment art pass, not finished production assets or a scientific reconstruction. The rover is an explicitly static chassis/wheel proxy; its presence is confirmed in the serialized scene and Hierarchy/Search. Dedicated close-up rover rendering was not obtained.

Missing: astronaut model/rig, character animation clips/controller, player movement, follow/orbit camera controller, rover driving/navigation, runtime dust-storm/weather simulation, mission logic and mission HUD/oxygen/power indicators. Static sky and fog are not a dust-storm event. Backend AI simulation is not connected to this Unity environment. No placeholder is represented as a completed gameplay feature.

## Actual evidence

UI captures (3024×1750):

- Screenshots/Game_View_Fresh_Play_Mode.png — fresh Play Mode, habitat arrival view.
- Screenshots/Game_View_Play_Mode.png — initially running scene.
- Screenshots/Console_Play_Mode.png — visible Console counters:0 errors,0 warnings,0 logs at capture.
- Screenshots/Scene_View_Current_State.png — actual Scene View showing outlines/camera gizmos and washed-out geometry, not a polished beauty capture.

Fresh 1600×900 Editor URP renders: Screenshots/Editor_URP_View_A_Arrival.png through View_F_Overview.png. Generated through the pre-existing MarsEnvironmentBuilder capture command while the real scene was loaded; timestamp: Evidence/Camera_Capture_Timestamp.txt. These are render-request images, not UI screenshots or a gameplay video. Prior Stage04 capture files were backed up and restored byte-for-byte; no new code entered Unity.

No astronaut/storm screenshots exist because those components do not exist. No gameplay preview video: exposed automation has no recording API and Unity Recorder is not an installed dependency; no recorder installed or artificial video made.

## Quality and limitations

Game View has visible stair-stepped edges; template Mobile pipeline is selected for Web by existing quality settings. Static scene rendering and shadows work; no magenta missing-material surfaces observed in arrival view. Overview shows strong fog washout and the finite rectangular terrain boundary. Scene View remains an unhelpful outline/flat view; its display configuration could not be corrected through unreliable coordinate interaction. This is a preview tooling limitation, not evidence that Game View rendering failed. No complete route/LOD/shadow sweep or physical movement test occurred.

Coordinate automation repeatedly failed with noWindowsAvailable/windowNotFoundAtPosition and state-change/timeout errors. Keyboard and menu access worked sufficiently for fresh Play Mode and Console checks. Stats overlay and dedicated rover close-up remain blocked; current FPS not measured. Historical Stage04 browser FPS is not reused as current Editor performance.

No Play Mode compilation blocker was observed; no local source fix required. Smallest operator-only viewport checks: click Game→Stats for FPS; click Scene drawing-mode dropdown→Shaded, clear selection/filter, select R_RoverBay and press F; orbit with Alt+mouse drag. These affect viewing only. Do not save changes to transforms or scene. Legacy EditorBuildSettings still enables SampleScene only; this does not block manually opening Mars scene, but future builds must deliberately select the intended scene. Build settings were preserved.

Preservation: 305 Assets/Packages/ProjectSettings files compared before/after camera capture:0 changed,0 added. Baseline was captured during preview before camera capture, not before the initial Editor selection. No scene Save command or authoring command invoked. Android, backend, AI and React not modified; no remote changes. Editor remains open on the Mars Game View in Play Mode at the last observed state.

See Current_Assets_Inventory.md and Play_Mode_Test_Results.md for counts, dependencies, reproduction and blocked checks. Stop after local preview.
