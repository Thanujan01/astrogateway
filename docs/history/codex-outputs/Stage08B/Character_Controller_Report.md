# Character controller

## Main Editor follow-up — operator unlocked Mac

This update supersedes the earlier Mac-lock-only status below. Unity Play Mode launched in the actual MissionRedHorizon project. A CUA-held W key visibly moved the astronaut; key3 selected DUST STORM and the HUD showed changing oxygen/power. All20 controlled regression checks passed again in the main project; Evidence/Scripted_Play_Mode_Tests.txt and Console_Counts.txt are now the main-project run (test-window0 errors/0 warnings). The isolated evidence is retained in Evidence/Final_Isolated_Run/.

Actual Game View screenshots with the overlay HUD now exist: Screenshots/01_Walk_Game.png,02_Rover_Game.png,03_LightDust_Game.png,04_Storm_Game.png,05_Clear_Game.png. Additional full Editor screenshots are06_MainEditor_GameHUD_Storm.png and07_MainEditor_GameHUD.png. The prior NoHUD renders remain valid actual camera captures. No video recorded.

Mouse click automation failed with `noWindowsAvailable`; RMB orbit remains unverified. The Mac locked again before the final E-key rover entry/drive/brake test. That hands-on sequence, comprehensive WASD/Shift testing, Scene View capture and displayed FPS remain blocked/unverified. Rover regression uses controlled inputs/direct interaction calls. MainEditor.log includes a later audio-device initialization/reset message, outside the clean test window; audio is not validated. Android performance remains unmeasured. No further Unity source changes were made in this follow-up. Pre-existing315 files remain unchanged.


Original Unity-primitive astronaut proxy, pale suit, graphite backpack/visor and orange chest panel; no commercial asset or humanoid skeleton. Generic Animator blend tree uses authored looping limb clips: idle, walk ±28°, run ±44°. Animation is simple rigid-segment motion, not production locomotion or foot IK.

CharacterController: radius 0.32 m, height 1.85 m, slope limit 32°, step 0.3 m, skin 0.04 m, minMoveDistance 0. Walk 2.2 m/s, run 3.8 m/s; diagonal input normalized; downward gravity 3.71 m/s² (Mars approximation), no jump. Ground contact and grounded walk/run motion passed. Animator speed parameter passed; detailed limb pose/foot sliding and steep-slope testing remain pending. Spawn (-42, sampled ground, -68) m.

Follow camera FOV62°, near0.15 m/far330 m, distance4.5 m, target1.25 m high, orbit pitch -8°…55°. Sphere cast on environment layer retracts camera; actors excluded. Position smoothing is time-based. Camera yaw passed via injected look input; manual mouse rotation and occlusion edge cases not verified.

Validation was performed by Unity 6000.6.4f1 in Play Mode in a source-identical isolated local copy (`work/stage08b/PlayModeLab`). Final_Lab_Provenance.json records zero Stage08B file mismatches. Tests inject controlled movement/look values and call the interaction method; they do not prove physical keyboard/mouse handling. The Mac was locked and UI automation could not unlock it. Hands-on validation is BLOCKED pending operator unlock. No Android device/emulator gameplay or displayed FPS test was performed.

The isolated Editor exited 0. Console_Counts.txt recorded errors=0, warnings=0 during the test window. The full Isolated_Editor.log contains an Editor QuickSearch startup ArgumentOutOfRangeException before that window; it remains an Editor indexing issue, not a clean full-session log. Earlier failed trials are retained under Evidence/Trial01 and Trial02. The particle reseeding authoring error was fixed by stopping/clearing before assigning the seed. Tiny-step movement uses minMoveDistance=0. These are local prototype changes, not authentication changes.

No touch/gamepad bindings or final movement controller integration outside the new scene. Original scene and input assets unchanged.
