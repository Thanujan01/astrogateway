# Asset and file inventory

Reused: existing Unity 6000.6.4f1 project, installed URP/Input System packages, CharacterController and sphere-cast approach; existing scenes retained for regression. No Mars prefabs/materials were edited or imported into Earth.

New folder Assets/AstroGateway/Mission01Milestone:
- Mission01Flow.cs: menu, setup, briefing, local save, Earth movement and interaction.
- Editor/Mission01Builder.cs: separate scene authoring, explicit menu opener and batch-only controlled validation.
- Mission01_EarthLaunch_Milestone_v4.unity: accepted latest scene; failed earlier scene trials remain only in validation copy.
- Prefabs/Trainee_Blockout.prefab and Boarding_Console.prefab: reusable placeholders.
- Materials/Mission01_Pale, Graphite, Ocean, Continents and Safety: native URP Lit materials.
- Unity metadata files preserve scene references.

Geometry is generated from Unity primitives, colors are authored locally, land patches are illustrative, ambient PCM is original procedural sine synthesis. No third-party download/license dependency. These are new prototype assets, not finished realistic art.

New_Files.json lists exact added paths. Existing-file preservation is recorded in Preservation_Check.json. Backup is work/mission01/Backup; do not restore it over newer operator work automatically.
