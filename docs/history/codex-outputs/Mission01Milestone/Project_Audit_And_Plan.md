# Existing project audit and dependency plan — 2026-10-12

Main project: /Users/kapilash/Projects/MissionRedHorizon. Unity 6000.6.4f1; URP 17.6.0; Input System 1.20.0. Existing assets total approximately 11 MiB on disk, not runtime memory.

Implemented existing systems: Stage08B playable scene with CharacterController astronaut/rover, keyboard and mouse control, Stage08C mobile controls, oxygen/power HUD, deterministic weather, three proximity targets and memory-only provisional event batches. Existing environment and greybox scenes and habitat/power/solar/rover-bay prefabs remain. Separate Stage08D evaluation has approved terrain/rocks/NASA robotic rover; it is not integrated into the playable game. Rover transmission fidelity and effective LOD reduction remain open.

Existing Web/Android Supabase identity belongs to the host, with FastAPI verified-JWT/PostgREST authorization. This milestone must not create another login or link local callsign data to an invented authenticated UUID. No backend, host or telemetry-contract changes are required.

No existing Earth onboarding/main-menu mission flow was found. MarsPrototype is coupled to rover/weather/HUD; using it for Earth would require false dependency objects. Reuse Input System, URP, CharacterController approach and existing project configuration; add a separately scoped Earth movement component instead of altering the working Mars controller.

Backup before implementation: work/mission01/Backup/{Assets,Packages,ProjectSettings}; pre-change SHA-256 manifest work/mission01/Before_Hashes.json. Validation occurs in refreshed work/stage08b/PlayModeLab before only new milestone files are copied into the main project.

Dependency order:
1. Separate local menu/setup/briefing state machine and validated local checkpoint.
2. Earth blockout scene, collision, third-person camera, boarding-console interaction.
3. Compilation, saved-scene reload and controlled Unity Play Mode tests.
4. Existing Mars controlled regression in the validation copy and preservation hashes.
5. Copy new assets only; provide launch instructions and honest limitations.

Stop here after the first playable milestone. Rocket flight, system-check cockpit, journey, landing, extended Mars objectives/resources, inventory/map/journal/repair/flashlight/crouch, dynamic ChallengeManager and account-bound persistence are later dependencies, not completed systems. No remote changes or new external asset acquisitions.
