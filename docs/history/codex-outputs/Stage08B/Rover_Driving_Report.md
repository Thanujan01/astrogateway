# Rover driving

## Main Editor follow-up — operator unlocked Mac

This update supersedes the earlier Mac-lock-only status below. Unity Play Mode launched in the actual MissionRedHorizon project. A CUA-held W key visibly moved the astronaut; key3 selected DUST STORM and the HUD showed changing oxygen/power. All20 controlled regression checks passed again in the main project; Evidence/Scripted_Play_Mode_Tests.txt and Console_Counts.txt are now the main-project run (test-window0 errors/0 warnings). The isolated evidence is retained in Evidence/Final_Isolated_Run/.

Actual Game View screenshots with the overlay HUD now exist: Screenshots/01_Walk_Game.png,02_Rover_Game.png,03_LightDust_Game.png,04_Storm_Game.png,05_Clear_Game.png. Additional full Editor screenshots are06_MainEditor_GameHUD_Storm.png and07_MainEditor_GameHUD.png. The prior NoHUD renders remain valid actual camera captures. No video recorded.

Mouse click automation failed with `noWindowsAvailable`; RMB orbit remains unverified. The Mac locked again before the final E-key rover entry/drive/brake test. That hands-on sequence, comprehensive WASD/Shift testing, Scene View capture and displayed FPS remain blocked/unverified. Rover regression uses controlled inputs/direct interaction calls. MainEditor.log includes a later audio-device initialization/reset message, outside the clean test window; audio is not validated. Android performance remains unmeasured. No further Unity source changes were made in this follow-up. Pre-existing315 files remain unchanged.


Original six-wheel primitive rover proxy at (-15, sampled ground, -93) m. Kinematic CharacterController collision, not a WheelCollider vehicle simulation. Chassis roughly1.9 ×3.2 m; rounded capsule collision configured radius1.15 m and height1.5 m (Unity capsule bounds constrain effective height). Collider footprint is an approximation and needs tighter geometry/terrain validation. Wheels are visual only; no suspension, wheel traction, roll or slope-aligned body.

Forward max3.8 m/s (13.68 km/h), reverse1.8 m/s. Acceleration2 m/s², coasting1.8 m/s², braking6 m/s²; turn48°/s scaled by speed and reversal. Enter within5 m; exit requires speed≤0.2 m/s and a grounded, clear capsule position3 m to either side. Follow distance7 m, target1.4 m. HUD shows absolute rover speed while occupied. Unoccupied rover remains grounded at its last position.

PASS: entry, acceleration, travel, grounded state, moving-exit rejection, steering, braking and exit in controlled Play Mode. These checks call Interact directly, not a physical E-key event. No crash/damage simulation or passenger animation.

Validation was performed by Unity 6000.6.4f1 in Play Mode in a source-identical isolated local copy (`work/stage08b/PlayModeLab`). Final_Lab_Provenance.json records zero Stage08B file mismatches. Tests inject controlled movement/look values and call the interaction method; they do not prove physical keyboard/mouse handling. The Mac was locked and UI automation could not unlock it. Hands-on validation is BLOCKED pending operator unlock. No Android device/emulator gameplay or displayed FPS test was performed.

The isolated Editor exited 0. Console_Counts.txt recorded errors=0, warnings=0 during the test window. The full Isolated_Editor.log contains an Editor QuickSearch startup ArgumentOutOfRangeException before that window; it remains an Editor indexing issue, not a clean full-session log. Earlier failed trials are retained under Evidence/Trial01 and Trial02. The particle reseeding authoring error was fixed by stopping/clearing before assigning the seed. Tiny-step movement uses minMoveDistance=0. These are local prototype changes, not authentication changes.

Next: hands-on maneuvering, obstacle collision, exit beside walls/slopes, slope conformity and Android controls. This is an exploration vehicle prototype, not an engineering simulator.
