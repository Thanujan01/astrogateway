# Local automated validation

These are controlled Unity Editor tests in the existing isolated PlayModeLab, not hands-on device results. Final logs and result files are the evidence; historical reports were not reused as current passes.

| Suite | PASS | FAIL | Evidence |
|---|---:|---:|---|
| Cinematic HUD | 42 | 0 | HUD_Controlled_Tests.txt |
| Native Control | 26 | 0 | NativeRegression/Controlled_PlayMode_Tests.txt |
| Mars | 20 | 0 | MarsRegression/Evidence/Scripted_Play_Mode_Tests.txt |
| Mission 01 | 13 | 0 | Mission01Regression/PlayMode_Controlled_Tests.txt |
| Visual Polish | 34 | 0 | VisualPolishRegression/Controlled_PlayMode_Tests.txt |
| Responsive | 44 | 0 | Responsive_Layout_Checks.txt |

Total: 179 PASS, 0 FAIL.

HUD coverage includes real Canvas/EventSystem/panels, missing providers, actual oxygen, anchors and safe area, analog joystick routing, independent camera finger, ownership rejection/recenter, movement, actual rover entry/driving/exit and avatar identity, source speed/battery, valid/invalid scans, UI/counter updates, no invented chemistry, save/failure/load/export status, settings persistence, pause/input lock, inventory empty state, expanded map, settings sliders, supported UI shaders, scan cancellation and timed completion/duplicate station handling. Responsive tests use a runtime probe because Editor callbacks expose a different Screen context; final PNG headers are independently checked.

Historical failures: the first builder trial transferred the MonoBehaviour script identity incorrectly; reference transfer was corrected before any main-project installation. The first pointer test ran before Canvas layout and revealed an oversized joystick fitter; both were fixed. A movement trial lost its one-shot test input on focus clearing; sustained controlled input is now submitted explicitly, retaining focus-loss clearing in gameplay. Some capture runs timed out across long process stalls; final screenshots/results are reported separately. Old Quick Search indexing exceptions persist at startup. No C# compile failures or missing scripts are accepted in the final scene.

No claim that controlled pointer events establish physical Android touch or human keyboard/camera acceptance. No cloud/auth/backend tests were required or performed in this Unity-only milestone.
