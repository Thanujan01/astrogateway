# Actual gameplay input verification

Actual main Unity Editor project/scene was used. No scripted teleport, testControl or virtual keyboard substitutes were used for these manual-input observations. The passive Editor-only trace records testControl=false throughout. Native automation delivers real keyboard events; this is not a human usability test. Scripted results are listed separately.

|Control/check|Result|Actual evidence and limits|
|---|---|---|
|W forward|PASS|Terrain landmarks moved; astronaut advanced with visible animation. Editor_Walk.png and passive trace|
|A/D/S cardinal movement|PARTIAL PASS|Individual inputs delivered; D/S route reached rover at approximately(-14.3,-91.2). Full direction/slope coverage not claimed|
|W+D diagonal|BLOCKED|Prior tool rejected two non-modifier keys; operator assistance requested this turn|
|Shift running|BLOCKED / not demonstrated|Shift+W accepted by tool but on-foot observed max approximately2.205 m/s, not3.8. Screenshot name Editor_Run identifies attempted test only. Could be modifier delivery; do not call the game mechanic failed or passed without operator input|
|RMB camera orbit|BLOCKED|Coordinate clicks returned noWindowsAvailable; no reliable held RMB drag|
|E rover entry|PASS|HUD switched to driving, astronaut hidden, passive driving transition true|
|Acceleration|PASS|W moved rover; HUD9.4km/h captured; passive maximum3.8m/s|
|Steering|PARTIAL PASS|D during coasting changed travel heading and x-position. Sustained W+D steering remains blocked|
|Space braking|PASS, limited|W then Space sequence stopped rover; speed0 in screenshot/trace. Quantitative braking distance not measured|
|E exit|PASS|Stopped rover exited, astronaut reappeared, trace driving=false|
|Camera occlusion|FAIL observed near rover bay|Large dark surface obscured most scene during entry/driving. Editor_Rover_Acceleration_Occlusion.png. Later exit returned clear view. Cause needs collider/camera investigation|
|Terrain slopes/wall collisions|BLOCKED comprehensive check|Ground traversal to rover observed; varied slopes and explicit wall collision sequence not completed|
|Terrain boundaries|BLOCKED|Not manually exercised at each edge|
|Weather1/2/3 and HUD|PASS limited visual|Actual main Editor light dust/storm/clear changed visibility and HUD; oxygen/power updated|
|Three mission targets|PASS, local provisional completion|Unlocked-Mac follow-up: real keyboard walking advanced HUD1/3 →2/3 at power →“Local exploration complete — provisional” at delta. Screenshots Editor_Resume_Power_2of3.png and Editor_Resume_Mission_Complete.png|

## Reproduction and operator checklist
Open Assets/AstroGateway/Stage08B/MarsOutpost_Playable_Stage08B.unity in the main project, enter Play, ensure Pause and Frame Debugger disabled, focus Game View. Test W+D against W for normalized speed; hold physical Left Shift+W and compare2.2 versus3.8m/s. Hold RMB and move mouse in both axes. Start rover near(-15,-93); enter with E, drive/steer/brake, test moving-exit rejection, stop and exit. Inspect rover-bay obstruction and camera against walls/slopes. Walk to targets(-45,-60),(25,-45),(0,75); confirm HUD3/3. Exercise±119m edges without passing through geometry.

Report each result with actual input and observation. Requested operator-assisted checks have no supplied results at report time and remain BLOCKED. No unexecuted check is marked passed.

## Controlled regressions
20/20 gameplay assertions and12/12 synthetic touch/layout checks passed in the isolated lab after diagnostic changes. Test-window Console0 errors/0 warnings. Full log retains known QuickSearch startup exception. These tests are not manual-control evidence. TouchChecks wrote its new output into the previous Stage08C evidence path; the freshly produced file (17:11:22) was copied into this stage and the routing limitation is retained. Stage07 provisional event behavior preserved; no witnessed scores or backend changes.

## Unlocked-Mac follow-up — 2026-10-10
The actual main Editor resumed Play Mode successfully. From fresh spawn, native D held30s then W10s reached power proximity (HUD2/3); A11s, W30s and W24s reached delta and the HUD displayed local provisional completion. These were real input events, without teleporting or scripted mission completion. No new position/speed trace was enabled, so approximate timing does not establish exact route distance or movement speed.

Right-mouse coordinate input retried on the unlocked Mac and still returned `Computer Use server error -10005: noWindowsAvailable`. The Mac lock is no longer the active blocker; mouse automation remains unavailable. Diagonal/running/sustained steering and comprehensive slope, wall and boundary checks require operator observations; the operator volunteered but supplied no results yet. Camera obstruction remains unresolved. The Inspector also displayed a mesh collision pre-baking warning for Regolith_250m_PreservedCorridor; this is a future-build warning, not evidence of failed current collision.

Play Mode was stopped and the inactive Play button visually confirmed. No scene, asset or gameplay source changes were made in this follow-up. No new Android performance run or Console audit was performed.
