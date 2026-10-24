# Current project audit
Status: safe local slice; full visual milestone PARTIAL. Unity 6000.6.4f1, URP 17.6.0.

The actual baseline is 692 files in Assets, Packages and ProjectSettings, independently backed up under `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/work/productionvisual/Backup`, with SHA-256 manifest `work/productionvisual/Before_Hashes.json`. Historical file counts are not used as current preservation evidence.

Existing scenes found: Stage08B playable, NativeControlSlice, Mission01Milestone, MarsEnvironment/Scenes/Mars_RealisticResearchBase_v1, MarsVisualPolish/Scenes/Mars_VisualPolish_v2 and CinematicHUD/Scenes/Mars_CinematicHUD_v1. The new scene derives from the latter. Original layouts, colliders, actor references and gameplay scripts are protected.

## Actual limitations
The astronaut is primitive geometry with procedural limb motion, not a licensed humanoid asset or animation rig. The crew rover uses a kinematic CharacterController, not wheel/suspension physics. Habitat interiors are sealed/unavailable. Scanning is explicitly synthetic. Saves are local partial snapshots. No cloud synchronization or authoritative server mission witness is implemented here.

Three original proximity objectives and five research stations are retained. Oxygen/suit power and rover telemetry are simulated source values. Water, food, radiation, temperature, wind and Sol clock are unavailable and labelled accordingly.

## Architecture risks
Private lifecycle forwarding in Cinematic adapters is Unity-version-pinned and reflection dependent; existing link.xml retention does not prove Android IL2CPP execution. Original MarsPrototype and EnvironmentPresentation both retain camera code; ProductionCameraRig runs last on the same camera. This is isolated presentation arbitration, not deletion of old camera code. Several HUD text/layout operations poll at 5Hz; allocation optimization remains. Input Action asset exists, but current runtime routes Input System polling through HUDInputRouter.

The mandatory desired Reference A was not supplied/found. Reference B is freshly captured under Baseline/HUD/Screenshots. No reference-match claim is possible.
