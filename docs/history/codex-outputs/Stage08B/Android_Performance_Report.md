# Android performance

## Main Editor follow-up — operator unlocked Mac

This update supersedes the earlier Mac-lock-only status below. Unity Play Mode launched in the actual MissionRedHorizon project. A CUA-held W key visibly moved the astronaut; key3 selected DUST STORM and the HUD showed changing oxygen/power. All20 controlled regression checks passed again in the main project; Evidence/Scripted_Play_Mode_Tests.txt and Console_Counts.txt are now the main-project run (test-window0 errors/0 warnings). The isolated evidence is retained in Evidence/Final_Isolated_Run/.

Actual Game View screenshots with the overlay HUD now exist: Screenshots/01_Walk_Game.png,02_Rover_Game.png,03_LightDust_Game.png,04_Storm_Game.png,05_Clear_Game.png. Additional full Editor screenshots are06_MainEditor_GameHUD_Storm.png and07_MainEditor_GameHUD.png. The prior NoHUD renders remain valid actual camera captures. No video recorded.

Mouse click automation failed with `noWindowsAvailable`; RMB orbit remains unverified. The Mac locked again before the final E-key rover entry/drive/brake test. That hands-on sequence, comprehensive WASD/Shift testing, Scene View capture and displayed FPS remain blocked/unverified. Rover regression uses controlled inputs/direct interaction calls. MainEditor.log includes a later audio-device initialization/reset message, outside the clean test window; audio is not validated. Android performance remains unmeasured. No further Unity source changes were made in this follow-up. Pre-existing315 files remain unchanged.


## Status
BLOCKED / NOT MEASURED: no Android APK/device run, displayed FPS, memory, thermal, draw-call or GPU timing evidence for Stage08B. Existing Android application and Unity-as-a-Library bridge were not modified or regression-tested in this stage. No mobile performance claim.

## Implemented budget choices
Scene-owned URP preview asset:0.85 render scale,2×MSAA, HDR off, post-processing off, shadow distance55 m and2 cascades. Existing LOD groups retained; dust capped240 with no volumetrics; original primitive character and rover; no additional packages. Quality pipeline is applied during this prototype and restored on disable, without changing saved global quality/project settings. Existing original renderer resources are reused.

Editor_Frame_Samples.csv is batch-mode CPU player-loop timing from the automated lab, including startup/capture stalls. It is not rendered/display FPS and cannot predict Android performance. Do not convert its very short frame times into a marketed FPS value.

## Required device validation
Add/verify touch controls before an Android playability claim; build a separate development player after approval/available tooling, test lowest supported physical Android device, profile CPU/GPU frame time, peak memory, shader compatibility, particles/overdraw, shadows and sustained thermal performance. Record resolution, hardware, OS and conditions. Proposed target30 FPS sustained is a target only. Inspect original terrain/rock colliders and draw calls before increasing art complexity.

## Evidence and limitations
20 controlled Editor Play Mode checks passed; final test window Console0 errors/0 warnings. Full Editor log retains a QuickSearch startup exception. No Android crash/ANR testing occurred; Windows/Web performance also unmeasured. Mac lock blocks main-Editor hands-on control and final Game View/HUD capture. Primitive animation/vehicle assets remain placeholders.
