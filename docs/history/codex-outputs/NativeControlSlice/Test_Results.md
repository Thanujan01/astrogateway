# Test results

|Suite|Method|Result|
|Native slice|Actual Unity batch Play Mode, synthetic fingers and input bus|26 PASS / 0 FAIL in final run|
|Mission01|Existing controlled Play Mode methods|13 PASS / 0 FAIL|
|Mars|Existing controlled Play Mode simulation suite|20 PASS / 0 FAIL|
|Preservation|SHA-256 comparison|403 original files unchanged|
|Android hardware|ADB inventory|No connected devices; testing UNTESTED|

Native coverage: future-mode rejection, far boarding, independent touch owners, release, mode clearing, handedness/safe-area, synthetic touch-driven walking, boarding, same-object identity/seating, rover travel, moving-exit rejection, seated save/load, braking/exit, battery consumption, typed telemetry, scan, state restore, export, corrupt snapshot and bounded history.

Files: Controlled_PlayMode_Tests.txt; Unity_PlayMode_Final_QA.log; Scene_Validation.txt; Milestone01Regression/PlayMode_Controlled_Tests.txt; Milestone01_Regression_Unity.log; MarsRegression/Evidence/Scripted_Play_Mode_Tests.txt; Mars_Regression_Unity.log; Preservation_Check.json.

No fresh physical-finger test, main-Editor human input/camera test, thermal test, FPS benchmark, 30-Sol progression, robot parameter validation or hosted Auth test occurred. Tests cannot establish those results. Original mission-target completion evidence remains historical; the current Mars controlled suite is locomotion/rover/weather/HUD coverage.

QuickSearch indexing exceptions remain in Editor logs. Final slice gameplay assertions and process exit were successful; that is not a zero-error Console claim.
