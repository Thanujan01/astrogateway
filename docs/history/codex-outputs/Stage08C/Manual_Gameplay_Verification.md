# Manual gameplay verification — 2026-10-10

## Method and mandatory stop
Actual main MissionRedHorizon Unity6000.6.4f1 Editor, existing MarsOutpost_Playable_Stage08B scene already running. Native key events delivered through CUA; observations read from actual Game View, not testControl or scripted movement. This baseline verification preceded installation of the new touch layer. It is not a post-change complete desktop certification.

|Check|Result|Actual observation|
|---|---|---|
|E rover entry|PASS|Astronaut disappeared; follow view switched to rover; HUD changed to E exit/drive/brake|
|W rover acceleration|PASS, partial|Held W2200 ms; rover visibly moved from bay; HUD11.2 km/h|
|WASD walking/diagonal|BLOCKED|Full sequence not completed|
|Shift running|BLOCKED|Not executed in this stage|
|RMB orbit|BLOCKED|Not executed in this stage|
|Rover steering/braking/exit|BLOCKED|Next native W+D chord was rejected by automation before delivery|
|Slopes/walls/camera occlusion/bounds|BLOCKED|No hands-on traversal|
|Three proximity targets|BLOCKED|Not traversed; initial HUD1/3 only|
|Weather transitions/HUD|PARTIAL|Clear HUD, oxygen88.2%, power86.6% initially observed; complete state sequence not manually exercised|

Exact automation blocker: `keyPressIncludedMultipleNonModifierKeys` for native W+D. Per user instruction, all UI-dependent testing stopped immediately. This is an input-tool limitation, not evidence that Unity diagonal movement failed. No scripted regression is substituted for these missing manual results. Earlier Stage08B observations remain historical evidence only.

Reproduction/remaining operator steps: focus Unity Game View; verify W/A/S/D, W+D and Shift+W, hold RMB and drag, E enter, W+D steering, Space braking then E exit; walk against habitat walls, through north terrain slopes and along ±119 m bounds; move camera against obstacles; approach habitat(-45,-60), power(25,-45), delta(0,75) within9 m; press1/2/3 and inspect HUD. Capture results using actual controls after the touch change. No authentication, remote services or trusted mission completion are involved.
