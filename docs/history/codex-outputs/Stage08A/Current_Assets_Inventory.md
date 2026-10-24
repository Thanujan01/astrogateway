# Current Unity assets inventory

Actual project: `/Users/kapilash/Projects/MissionRedHorizon`. 128 files under Assets excluding .meta; complete paths/counts in Evidence/Asset_Inventory.json. Package-cache samples are not counted as authored project assets.

## Scenes

- `Assets/Scenes/SampleScene.unity`
- `Assets/AstroGateway/Scenes/MarsOutpost_Environment_v01.unity`
- `Assets/AstroGateway/Greybox/MarsOutpost_Greybox.unity`
- `Assets/AstroGateway/Greybox/PreGreybox_Untitled_Backup.unity`

Selected main environment scene: `/Users/kapilash/Projects/MissionRedHorizon/Assets/AstroGateway/Scenes/MarsOutpost_Environment_v01.unity`. Greybox and pre-greybox backup retained. SampleScene remains the legacy enabled build scene; custom prior build workflows may explicitly provide a different scene. No build configuration changed.

## Environment prefabs

- `Assets/AstroGateway/Environment/Prefabs/G_Greenhouse.prefab`
- `Assets/AstroGateway/Environment/Prefabs/AngularBasalt.prefab`
- `Assets/AstroGateway/Environment/Prefabs/S_SolarArray.prefab`
- `Assets/AstroGateway/Environment/Prefabs/R_RoverBay.prefab`
- `Assets/AstroGateway/Environment/Prefabs/RegolithStone.prefab`
- `Assets/AstroGateway/Environment/Prefabs/H_CommandHabitat.prefab`
- `Assets/AstroGateway/Environment/Prefabs/P_PowerStation.prefab`
- `Assets/AstroGateway/Environment/Prefabs/LayeredDelta.prefab`
- `Assets/AstroGateway/Environment/Prefabs/ErodedLandmark.prefab`
- `Assets/AstroGateway/Environment/Prefabs/C_ScienceStaging.prefab`

Terrain is a procedural continuous mesh/MeshCollider, not an imported Mars DEM. Original regolith, basalt/layered rock and architecture assets remain. 20 total materials across greybox/environment;11 art URP Lit surfaces plus custom DustSky material/shader in the art area.9 PNG assets;75 .asset files (mixed meshes/settings/etc.; not75 models). All external scene GUIDs and material shader GUIDs resolved against Assets and installed PackageCache; this is a static reference scan, not a full runtime shader-platform test.

## Scene components

| Component | Serialized count |
|---|---:|
| Camera | 6 |
| Light | 1 |
| Animator | 0 |
| Animation | 0 |
| CharacterController | 0 |
| Canvas | 0 |
| ParticleSystem | 0 |
| NavMeshAgent | 0 |
| MonoBehaviour | 7 |
| MeshRenderer | 247 |
| MeshCollider | 1 |
| LODGroup | 75 |

6 composition-validation cameras; Arrival enabled runtime camera. One directional Sun at intensity1.5; scene fog enabled, custom gradient sky.75 LOD groups and247 renderer components include inactive LOD alternatives, not247 draw calls. MeshCollider count is1; primitive box colliders are separate. Monobehaviours resolve to installed components; their presence does not establish gameplay.

No imported FBX/OBJ/BLEND/glTF models in Assets;0 .anim/.controller assets. StaticRoverChassis_NoDriving exists under R_RoverBay. No astronaut rig, Animator, Animation, CharacterController, NavMeshAgent, ParticleSystem or Canvas in main scene. Unity navigation/input/animation modules being installed does not mean these systems are implemented.

## Project C# scripts

- `Assets/TutorialInfo/Readme.cs`
- `Assets/TutorialInfo/Editor/ReadmeEditor.cs`
- `Assets/AstroGateway/Environment/Editor/MarsEnvironmentBuilder.cs`
- `Assets/AstroGateway/Greybox/Editor/MarsGreyboxBuilder.cs`

Two environment/greybox builders are Editor-only; tutorial Readme scripts provide template documentation. No astronaut, vehicle, mission HUD or weather runtime scripts found in Assets. Android integration scenes/bridge in separate POC workflows are not this main scene's gameplay.

## Packages and pipeline

All non-engine manifest dependencies match installed package.json versions, including URP17.6.0, Input System1.20.0, AI Navigation2.0.14, Timeline6.6.0 and UGUI2.6.0. Engine modules are supplied by Editor.QualitySettings references Mobile_RPAsset (WebGL default quality0) and PC_RPAsset; GraphicsSettings global custom pipeline field is null, but quality overrides are valid URP assets. This is not a Built-in scene relabeled URP. The actual Game View and render-request captures use current pipeline.

No asset import, dependency install, character creation or scene replacement performed. Inventory reflects this project on2026-10-09, not prior design aspirations.
