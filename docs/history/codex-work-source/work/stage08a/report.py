from pathlib import Path
import json
r=Path('outputs/Stage08A');i=json.loads((r/'Evidence/Asset_Inventory.json').read_text());p=json.loads((r/'Evidence/Preservation_Check.json').read_text())
project=Path(i['project']);scene=i['main_scene']
(r/'Unity_Visual_Preview_Report.md').write_text(f'''# Unity visual preview — Stage08A

Date: 2026-10-09. **Play Mode launched successfully** in the actual MissionRedHorizon project. This is an environment-art preview, not a completed playable astronaut mission.

Project: `{project}`\nScene: `{scene}`\nEditor: Unity 6.6 / 6000.6.4f1, matching ProjectVersion.txt. URP:17.6.0, installed package version matches manifest. Metal Editor; active platform Web. Existing editor already held this scene and was initially in Play Mode; a redundant CLI launch exited. We selected the existing correct Editor, exited Play Mode, entered a fresh Play Mode using Command-P and observed its blue Stop button and actual rendered Game View. No Editor upgrade or project conversion.

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

Preservation: {p['files_checked']} Assets/Packages/ProjectSettings files compared before/after camera capture:0 changed,0 added. Baseline was captured during preview before camera capture, not before the initial Editor selection. No scene Save command or authoring command invoked. Android, backend, AI and React not modified; no remote changes. Editor remains open on the Mars Game View in Play Mode at the last observed state.

See Current_Assets_Inventory.md and Play_Mode_Test_Results.md for counts, dependencies, reproduction and blocked checks. Stop after local preview.
''')
scenes='\n'.join('- `'+s+'`' for s in i['scenes']);prefabs='\n'.join('- `'+s+'`' for s in i['prefabs']);scripts='\n'.join('- `'+s+'`' for s in i['scripts'])
(r/'Current_Assets_Inventory.md').write_text(f'''# Current Unity assets inventory

Actual project: `{project}`. {i['asset_count_excluding_meta']} files under Assets excluding .meta; complete paths/counts in Evidence/Asset_Inventory.json. Package-cache samples are not counted as authored project assets.

## Scenes

{scenes}

Selected main environment scene: `{scene}`. Greybox and pre-greybox backup retained. SampleScene remains the legacy enabled build scene; custom prior build workflows may explicitly provide a different scene. No build configuration changed.

## Environment prefabs

{prefabs}

Terrain is a procedural continuous mesh/MeshCollider, not an imported Mars DEM. Original regolith, basalt/layered rock and architecture assets remain. 20 total materials across greybox/environment;11 art URP Lit surfaces plus custom DustSky material/shader in the art area.9 PNG assets;75 .asset files (mixed meshes/settings/etc.; not75 models). All external scene GUIDs and material shader GUIDs resolved against Assets and installed PackageCache; this is a static reference scan, not a full runtime shader-platform test.

## Scene components

| Component | Serialized count |
|---|---:|
'''+''.join(f'| {k} | {v} |\n' for k,v in i['scene_component_counts'].items())+f'''
6 composition-validation cameras; Arrival enabled runtime camera. One directional Sun at intensity1.5; scene fog enabled, custom gradient sky.75 LOD groups and247 renderer components include inactive LOD alternatives, not247 draw calls. MeshCollider count is1; primitive box colliders are separate. Monobehaviours resolve to installed components; their presence does not establish gameplay.

No imported FBX/OBJ/BLEND/glTF models in Assets;0 .anim/.controller assets. StaticRoverChassis_NoDriving exists under R_RoverBay. No astronaut rig, Animator, Animation, CharacterController, NavMeshAgent, ParticleSystem or Canvas in main scene. Unity navigation/input/animation modules being installed does not mean these systems are implemented.

## Project C# scripts

{scripts}

Two environment/greybox builders are Editor-only; tutorial Readme scripts provide template documentation. No astronaut, vehicle, mission HUD or weather runtime scripts found in Assets. Android integration scenes/bridge in separate POC workflows are not this main scene's gameplay.

## Packages and pipeline

All non-engine manifest dependencies match installed package.json versions, including URP17.6.0, Input System1.20.0, AI Navigation2.0.14, Timeline6.6.0 and UGUI2.6.0. Engine modules are supplied by Editor.QualitySettings references Mobile_RPAsset (WebGL default quality0) and PC_RPAsset; GraphicsSettings global custom pipeline field is null, but quality overrides are valid URP assets. This is not a Built-in scene relabeled URP. The actual Game View and render-request captures use current pipeline.

No asset import, dependency install, character creation or scene replacement performed. Inventory reflects this project on2026-10-09, not prior design aspirations.
''')
(r/'Play_Mode_Test_Results.md').write_text('''# Play Mode results

Actual existing project and MarsOutpost_Environment_v01 scene; Unity6000.6.4f1/URP17.6.0;2026-10-09.

| Check | Result | Evidence / limits |
|---|---|---|
| Editor/project compatibility | PASS | Window title and ProjectVersion match; package versions match |
| Main Mars scene open | PASS | Native window URL identifies exact scene |
| Fresh Play Mode entry | PASS | Command-P exit, then Command-P entry; blue Stop button and fresh Game View capture |
| Terrain/outpost/lighting/sky render | PASS for arrival view | Actual UI capture and six new URP camera renders; no complete visual sweep |
| Console | PASS at observed capture |0 errors,0 warnings; no Console Clear used; later capture adds utility log |
| Scene/material external references | PASS static scan |0 unresolved GUIDs; not exhaustive platform validation |
| Astronaut close-up/animations/movement | BLOCKED: absent |0 character model/animation/controller assets; no implementation |
| Rover presence | PASS inventory/Hierarchy | StaticRoverChassis_NoDriving; no production vehicle quality claim |
| Rover driving/navigation | BLOCKED: absent | No controller or NavMeshAgent |
| Dedicated rover close-up | BLOCKED: tooling | Coordinate targeting and search-selection automation failed; overview not presented as close-up |
| Dust storm/weather | BLOCKED: absent | No ParticleSystem/weather code; sky/fog only |
| Mission HUD/oxygen/power | BLOCKED: absent | No Canvas or HUD runtime code |
| Camera controls | BLOCKED: absent runtime controller | Arrival is fixed camera; Editor camera controls are not player controls |
| FPS | UNVERIFIED | Stats overlay could not be opened; no current measurement or old FPS reused |
| Gameplay video | NOT RECORDED | No exposed recording operation/installed Recorder used |
| Asset preservation | PASS scoped comparison | Assets/Packages/ProjectSettings baseline before camera capture unchanged afterward |

Reproduction: open project with matching Editor; open Assets/AstroGateway/Scenes/MarsOutpost_Environment_v01.unity; press Command-2 for Game, Command-P to enter Play; inspect Arrival view; use Window→General→Console. Stats must be enabled manually for current FPS. Runtime interaction cannot be validated until those components exist. No gameplay feature added to satisfy preview.

Evidence/Editor_Diagnostics.txt records actual Playmode entries, existing reload-domain/reload-scene-disabled configuration and ASTROGATEWAY_ART_CAPTURE_OK. Those reload settings may matter once stateful gameplay is added; no settings changed. A redundant launch's licensing refresh message is not treated as a compile or Play Mode failure: existing Editor rendered successfully. Full local logs remain in project Logs/Editor.log; deliverable diagnostics exclude licensing/environment details.

Initial Scene View was flat/outline and current Scene capture remains so. Game View correctly rendered. Smallest safe viewing adjustment is manual Shaded mode/Frame Selected, no source changes; full underlying Scene View issue not isolated. No crashes or compilation errors observed during successful Play/render capture, but no long-duration stress or gameplay test claimed.

Local preview stopped after evidence/report creation. No remote DB/deployment, authentication/security change, scene Save, asset replacement or source implementation.
''')
shots=sorted((r/'Screenshots').glob('*.png'))
(r/'Screenshot_Index.md').write_text('# Actual capture index\n\nAll files are captured this stage from Unity UI or existing Unity URP cameras. No generated/mock images.\n\n'+'\n\n'.join(f'[{p.name}]({p.resolve()})' for p in shots)+'\n\nScene screenshot documents the outline/display limitation. No astronaut/storm/rover-closeup image or video is claimed.\n')
print('Three reports and actual screenshot index created.')
