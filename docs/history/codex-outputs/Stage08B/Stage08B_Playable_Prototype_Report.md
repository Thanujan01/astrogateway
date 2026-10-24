# Stage08B playable prototype — 2026-10-09

## Main Editor follow-up — operator unlocked Mac

This update supersedes the earlier Mac-lock-only status below. Unity Play Mode launched in the actual MissionRedHorizon project. A CUA-held W key visibly moved the astronaut; key3 selected DUST STORM and the HUD showed changing oxygen/power. All20 controlled regression checks passed again in the main project; Evidence/Scripted_Play_Mode_Tests.txt and Console_Counts.txt are now the main-project run (test-window0 errors/0 warnings). The isolated evidence is retained in Evidence/Final_Isolated_Run/.

Actual Game View screenshots with the overlay HUD now exist: Screenshots/01_Walk_Game.png,02_Rover_Game.png,03_LightDust_Game.png,04_Storm_Game.png,05_Clear_Game.png. Additional full Editor screenshots are06_MainEditor_GameHUD_Storm.png and07_MainEditor_GameHUD.png. The prior NoHUD renders remain valid actual camera captures. No video recorded.

Mouse click automation failed with `noWindowsAvailable`; RMB orbit remains unverified. The Mac locked again before the final E-key rover entry/drive/brake test. That hands-on sequence, comprehensive WASD/Shift testing, Scene View capture and displayed FPS remain blocked/unverified. Rover regression uses controlled inputs/direct interaction calls. MainEditor.log includes a later audio-device initialization/reset message, outside the clean test window; audio is not validated. Android performance remains unmeasured. No further Unity source changes were made in this follow-up. Pre-existing315 files remain unchanged.


## Outcome
Local implementation complete; controlled Play Mode validation PASS (20/20). Hands-on controls and Android performance BLOCKED/unmeasured. This is a visibly primitive exploration prototype, not finished character/environment art.

## Scene and preservation
New scene: `/Users/kapilash/Projects/MissionRedHorizon/Assets/AstroGateway/Stage08B/MarsOutpost_Playable_Stage08B.unity`.
Original MarsOutpost_Environment_v01 preserved. SHA-256 comparison of 315 pre-existing Assets, Packages, ProjectSettings and Docs files found zero modifications. No backend, Supabase, React or Android changes, packages or third-party assets were introduced.

## Hierarchy and content
- 01_PreservedEnvironment_Stage08BCopy: original 250 × 250 m terrain, six zones, copied renderers/materials, afternoon lighting and sky; original static rover and cameras disabled only in this copy.
- 02_LocalProvisionalExploration: MarsPrototype, primitive astronaut with authored idle/walk/run clips, six-wheel kinematic rover, additive exterior details, follow camera, bounded dust and mission HUD.

The terrain retains existing textured detail; materials were cloned before smoothness adjustments. Three dark basalt tints and modest habitat service ribs provide variation. Basalt variants are flat-color prototypes and need texture/LOD consistency refinement. Existing southwest afternoon sunlight/shadows remain; haze varies with weather. No cinematic or expensive volumetric effects.

## Controls
WASD walk/drive; Left Shift run; hold right mouse and drag to orbit; E enter nearby rover/exit once stopped; Space brake; 1 clear, 2 light dust, 3 storm; Escape release cursor. Command/Ctrl shortcuts suppress movement/weather inputs. Open the new scene and press Play in Unity after unlocking the Mac.

## Local validation
Validation was performed by Unity 6000.6.4f1 in Play Mode in a source-identical isolated local copy (`work/stage08b/PlayModeLab`). Final_Lab_Provenance.json records zero Stage08B file mismatches. Tests inject controlled movement/look values and call the interaction method; they do not prove physical keyboard/mouse handling. The Mac was locked and UI automation could not unlock it. Hands-on validation is BLOCKED pending operator unlock. No Android device/emulator gameplay or displayed FPS test was performed.

The isolated Editor exited 0. Console_Counts.txt recorded errors=0, warnings=0 during the test window. The full Isolated_Editor.log contains an Editor QuickSearch startup ArgumentOutOfRangeException before that window; it remains an Editor indexing issue, not a clean full-session log. Earlier failed trials are retained under Evidence/Trial01 and Trial02. The particle reseeding authoring error was fixed by stopping/clearing before assigning the seed. Tiny-step movement uses minMoveDistance=0. These are local prototype changes, not authentication changes.

```
walk displacement: PASS
walk speed: PASS
grounded: PASS
walk animator: PASS
run speed: PASS
run displacement: PASS
camera yaw: PASS
enter rover: PASS
rover acceleration: PASS
rover terrain travel: PASS
rover grounded: PASS
exit while moving rejected: PASS
rover steering: PASS
rover braking: PASS
exit rover: PASS
light dust: PASS
storm particles bounded: PASS
storm visibility: PASS
HUD resources: PASS
clear transition: PASS
```

Stage07A Batch and movement/resource validation passed for two actual synthetic QA exports (8 walking events, 19 rover/weather events). They carry schema_version=1 and local UUIDs, no owner IDs, credentials or final scores. No transmission/persistence occurs during normal play. Test exports are explicit Editor QA artifacts. Mission events remain provisional until Stage07C independently verifies witnesses. A local seed or event UUID provides no anti-tampering guarantee.

## Files
All Unity additions are under `/Users/kapilash/Projects/MissionRedHorizon/Assets/AstroGateway/Stage08B/`: MarsPrototype.cs, Editor/Stage08BBuilder.cs, the new scene, Materials/, Animation/, Stage08B_MobilePreview.asset and Unity metadata. Evidence and five reports are under this outputs/Stage08B directory. The Editor helper contains workstation-specific output paths and should be made portable before shared development.

## Actual captures
Screenshots/01_Walk_URP_NoHUD.png; 02_Rover_URP_NoHUD.png; 03_LightDust_URP_NoHUD.png; 04_Storm_URP_NoHUD.png; 05_Clear_URP_NoHUD.png. These are actual 1280 × 720 URP camera render requests during Unity Play Mode in the isolated copy, not generated images. They omit the Screen Space Overlay HUD and are not GUI Scene/Game View screenshots. Batch ScreenCapture did not produce Game View images. No video was recorded; a supported recording workflow was not available. The live Editor preview before locking showed the astronaut and HUD but was not saved as final-version evidence.

## Missing and next work
Unlock and manually test WASD, diagonals, RMB, E, steep terrain, walls and camera occlusion; save HUD/Game View screenshots. Add Android touch controls and device profiling before claiming mobile readiness. Replace original primitive proxies with licensed rigged assets after approval; refine feet, rover suspension/terrain alignment, collider shapes and rock materials. Mission completion through all three proximity targets and boundary/camera collision edge cases are implemented but not comprehensively tested. Do not promote provisional events into trusted training evidence.
