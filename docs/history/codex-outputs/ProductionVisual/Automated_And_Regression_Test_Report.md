# Executed validation
Final controlled production run: **43 PASS, 0 FAIL**. Post-integration regressions: **179 PASS, 0 FAIL**. Total 222 accepted assertions, plus actual-main import/reload audit. This is automated local Editor validation, not physical Android or exhaustive hands-on play.

| Suite | Checks | Scope |
|---|---:|---|
| Production scene | 43 | Scene references, render support, LOD reduction, modes, PC event routing, camera wall fixture, ten rover cycles, drive/brake/exit, scans/report/save, tuning |
| HUD functional | 42 | New scene, real UI/actions/providers/input/persistence |
| Responsive | 44 | New scene, four actual landscape Game View sizes / essential containment |
| Native Control | 26 | Original Native scene, original gameplay source |
| Mars | 20 | Original Stage08B scene |
| Mission 01 | 13 | Original Mission01 scene |
| Visual Polish | 34 | Existing v2 visual scene |

Full assertions: Production_Controlled_Tests.txt and Regression/** test text. Execution exit records: Regression/Execution.json. Baseline was run before implementation: same179 accepted assertions after one Native retry.

## Failures / corrections retained
- First baseline Native run:25 PASS/1FAIL walking threshold. Its wall-time wait was replaced in the lab-only harness with a held-input simulation-time interval. Retry26PASS, 2.500648m/1.212281 simulation seconds. First log/results retained; no original gameplay controller was changed for it.
- First four-preset harness failed compilation because a runtime assembly referenced an Editor helper. Replaced with callback; subsequent benchmark and actual-main compilation passed. Four_Preset_First_Compile_Failure.log retained.
- A capture retry had42PASS/1FAIL: one-frame synthetic RMB event missed. Held/repeated event submission in the test harness produced final43PASS. Capture_Retry_With_RMB_Failure*.txt/log retained. This does not certify real combined-key input.
- Legacy viewport cleanup used relative custom-size indices when Unity expects absolute indices. Corrected in disposable lab helper; new ProductionViewport utility has its own corrected cleanup. Original CinematicHUD helper source is preserved. Earlier responsive/performance logs contain cleanup errors despite assertions passing; do not call those logs error-free.
- Disposable lab logs contain UnityEditor.Search.SearchDatabase startup ArgumentOutOfRangeException. Final controlled assertions still passed. Actual-main import audit has no matching compile/NullReference/MissingReference diagnostics. This upstream Editor indexing issue remains recorded, not silently excluded.

Lab-only harness changes: output redirects, target new scene for HUD/layout, Native measured held-input interval, corrected viewport cleanup. They are NOT installed over any existing main-project files.

## Native UI verification
Actual main Editor opened Mars_ProductionVisual_v1 and entered Play Mode via OS Cmd+P. Native screenshot showed rendered astronaut/terrain/dome and running Statistics. Subsequent maximize/click action failed `Computer Use server error -10005: noWindowsAvailable`. UI-dependent tests stopped. No hands-on movement/diagonal/run/RMB/rover cycle is claimed. Last observed state: Play Mode running; operator should press Stop. No scene was saved from native Play Mode.

No gameplay video, physical multi-touch, IL2CPP APK regression, live authentication or remote service tests in this milestone.
