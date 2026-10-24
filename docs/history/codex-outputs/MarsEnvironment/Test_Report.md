# Automated and baseline evidence

Initial audit used actual scene/script/dependency inspection and a verified file backup. The earlier 26/20/13 counts were treated as historical; no separate pre-change Play Mode baseline was executed. Fresh preserved-scene suites were run during this milestone after environment code integration in the validation copy.

|Suite|Evidence|Result|
|New research scene|Controlled_PlayMode_Tests.txt; Unity_PlayMode_Ground_Contact.log|34 PASS, 0 FAIL|
|Native slice|NativeRegression/Controlled_PlayMode_Tests.txt; Native_Regression_Unity.log|26 PASS, 0 FAIL|
|Mars|MarsRegression/Evidence/Scripted_Play_Mode_Tests.txt; Mars_Regression_Unity.log|20 PASS, 0 FAIL|
|Mission01|Mission01Regression/PlayMode_Controlled_Tests.txt; Mission01_Regression_Unity.log|13 PASS, 0 FAIL|
|Original files|Preservation_Check.json|420 checked, 0 changed/missing|
|Acquired sources|Approved_Source_Hash_Checks.json|27 matched acquisition hashes|

Actual Unity Editor batch Play Mode in work/stage08b/PlayModeLab. Inputs are synthetic/input-bus tests and controlled proximity arrangements, not hands-on gameplay. New suite covers save namespace, dependencies, physical habitat ray blocking, spawn ground, runtime quality/light settings, camera state/follow, terrain walking/grounding, boarding, driving/braking/exit, persistent astronaut, wheel clearance, boot-rig attachment, telemetry, scans/export/save/load, range gating, supported shaders, target association and pause/resume clock stability.

Visual inspection separately reviewed real habitat/overview/rover camera renders and actual Play Mode HUD image. Presence/physics assertions are not substituted for that inspection. No claim of finished visual quality, exhaustive route/camera safety or real touch testing.

Failures retained: missing research references in first trial, one wall-clock movement assertion and QuickSearch indexing exceptions. Component separation/reload repair and simulation-time-based harness with timeouts produced passing final checks. No original gameplay script was changed to force a test PASS.

No physical Android connected; no Android player validation or new packaged build. Final AO import/color captures are visual checks, not FPS measurements. Compilation succeeded; QuickSearch means the Editor Console cannot be described as error-free.
