# Weather and visuals

## Main Editor follow-up — operator unlocked Mac

This update supersedes the earlier Mac-lock-only status below. Unity Play Mode launched in the actual MissionRedHorizon project. A CUA-held W key visibly moved the astronaut; key3 selected DUST STORM and the HUD showed changing oxygen/power. All20 controlled regression checks passed again in the main project; Evidence/Scripted_Play_Mode_Tests.txt and Console_Counts.txt are now the main-project run (test-window0 errors/0 warnings). The isolated evidence is retained in Evidence/Final_Isolated_Run/.

Actual Game View screenshots with the overlay HUD now exist: Screenshots/01_Walk_Game.png,02_Rover_Game.png,03_LightDust_Game.png,04_Storm_Game.png,05_Clear_Game.png. Additional full Editor screenshots are06_MainEditor_GameHUD_Storm.png and07_MainEditor_GameHUD.png. The prior NoHUD renders remain valid actual camera captures. No video recorded.

Mouse click automation failed with `noWindowsAvailable`; RMB orbit remains unverified. The Mac locked again before the final E-key rover entry/drive/brake test. That hands-on sequence, comprehensive WASD/Shift testing, Scene View capture and displayed FPS remain blocked/unverified. Rover regression uses controlled inputs/direct interaction calls. MainEditor.log includes a later audio-device initialization/reset message, outside the clean test window; audio is not validated. Android performance remains unmeasured. No further Unity source changes were made in this follow-up. Pre-existing315 files remain unchanged.


Three manual states use linear fog and URP Unlit transparent dust; no volumetrics or fullscreen effects.

|State|Fog start/end m|Particles/second|
|---|---|---|
|Clear|75/280|0|
|Light dust|25/130|14|
|Dust storm|8/48|65|

Particle lifetime3 s; max240; emitter16×7×16 m around active actor; world-space particles; seed808 + state. Stop/clear precedes reseeding. Repeated state selection resets deterministic particle randomness; exact cross-platform physics/render replay is not established. Weather logic is synthetic local presentation, not live NASA weather or the complete Stage07A simulator. Clear→light→storm→clear and bounded particles passed.

Lighting retains the original afternoon composition. Scene-owned mobile preview pipeline bounds shadow distance55 m with2 cascades, MSAA2, renderScale0.85 and no HDR/post-processing. Copied material roughness and basalt tints plus habitat ribs are modest additive changes. Dust has simple untextured quads and will benefit from soft alpha sprites; no final texture pass is claimed.

HUD oxygen/power decrease with explicitly synthetic rates: oxygen0.015 percentage points/s normally,0.035 in storm; power0.018 walking/on foot,0.035 driving. Weather does not currently enforce rover speed or equipment failures. Seeded visual dust, local proximity objectives and dust-resolution events are provisional; schema compatibility does not certify physical accuracy or witness authenticity.

Actual camera captures are in Screenshots/. Overlay HUD is excluded; Game View/HUD capture and manual weather-key tests are pending Mac unlock. No gameplay video.
