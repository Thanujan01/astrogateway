# Launch

Existing Unity project: /Users/kapilash/Projects/MissionRedHorizon
Unity 6000.6.4f1. Open /Users/kapilash/Projects/MissionRedHorizon/Assets/AstroGateway/NativeControlSlice/Mars_NativeControl_VerticalSlice.unity, or AstroGateway → Native Slice → Open Vertical Slice. Preserve any unsaved scene changes before switching. Click Play and focus Game View.

PC: WASD move/steer, Shift run, RMB orbit, E board/exit, Space brake, K scan, keys 1–3 weather. Mobile: floating move joystick, independent camera drag, RUN/ACCELERATE hold, BRAKE, BOARD/EXIT and SCAN. Touch controls are previewed in Editor, but synthetic tests do not certify mouse simulation of physical fingers.

Drive only after boarding near the prototype rover. Stop before exit. Data panel shows simulated telemetry and scans; Save/Load uses a separate native-slice local file. Export writes local JSON/CSV. These actions do not sign in, upload or grant rewards on a server.

MissionRegistry.asset contains proposed display names/objectives/difficulty/reward. Do not edit stable missionId after saved states are created without a version/migration plan. AdaptiveTouch.prefab is a reusable provider that must be bound to the scene's modes and game references before enabling; it is not a complete standalone HUD.

Existing original scenes remain separate:
- Assets/AstroGateway/Stage08B/MarsOutpost_Playable_Stage08B.unity
- Assets/AstroGateway/Mission01Milestone/Mission01_EarthLaunch_Milestone_v4.unity

No build settings, Android host export or packaged release changed. For repeat controlled tests use NativeSliceBuilder.ValidateExisting only in the disposable validation copy; it writes test fixture snapshots and evidence to this workspace. BuildAndTest refuses existing scene overwrite.

Backup: work/nativecontrol/Backup. Do not restore over subsequent operator work automatically. New_Files.json identifies added files; Preservation_Check.json verifies originals.
