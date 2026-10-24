# Validation report — 2026-10-15

All execution used Unity 6000.6.4f1 and the existing cached isolated project at work/stage08b/PlayModeLab. Only newly owned assets were then installed into the main project. Original main-project scene files were not opened for writing or replaced. No package installation, remote service change or gameplay source rewrite.

| Suite | PASS | FAIL | Evidence |
|---|---:|---:|---|
| New scene, final post-theme run | 34 | 0 | NewSceneTests/Controlled_PlayMode_Tests.txt |
| Native Control | 26 | 0 | NativeRegression/Controlled_PlayMode_Tests.txt |
| Mars | 20 | 0 | MarsRegression/Evidence/Scripted_Play_Mode_Tests.txt |
| Mission 01 | 13 | 0 | Mission01Regression/PlayMode_Controlled_Tests.txt |

Total 93 controlled checks passed. These are scripted input/state tests, not hands-on keyboard/touch certification. The 34 new-scene checks were repeated after the HUD theme addition. The other 59 suites validate unchanged scenes/components. Logs retain Editor Quick Search indexing exceptions; no C# compilation errors or missing/null reference exceptions were found in the final new-scene log search.

New-scene coverage: save/reload, dependencies, grounded movement, follow camera liveness, rover boarding/driving/braking/exit and avatar identity, wheel clearance tolerance, telemetry, scan/export/save/load, proximity gates/association, shaders, four actual quality configurations, four lighting states, pause/resume. Scene reload resolves all five ResearchTarget references. FOV policy and translucent styling are visually inspected; original orbit/collision algorithms are retained.

Not established: manual Shift+diagonal/RMB combinations, touch-camera response on physical hardware, exhaustive camera collision/boundary cases, complete save-state fidelity, all mission completion routes by a human. Native UI observation timed out. Do not convert those untested items into PASS.

Preservation_Check.json: all 547 current baseline files unchanged and present; all 420 earlier originals unchanged. Backup: work/visualpolish/Backup. Original acquired assets were never written by this milestone; only newly owned native derivatives are installed.

Actual before/after Game View captures exist for all five viewpoints. ScreenCapture's first batch attempt produced no files; retry explicitly opened the Game View and succeeded. CameraOnly captures are separate supporting renders, never substituted for Game View evidence. Final visual captures used fixed scenic camera poses in Play Mode; gameplay-camera dynamics are not demonstrated by them.
