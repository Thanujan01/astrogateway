# First Mission Rover — Phase 2 report (2026-10-20)

Scope approved by SSL on 2026-10-20: isolated First Mission scene, ARES-VI visuals on TERRAVEX gameplay, Android landscape tabbed controls, transparent dashboard, labelled sensor simulation, a local-only sensor-records endpoint, offline-safe collection and an AI interface with no fabricated inference.
Nothing was pushed, merged, deployed or migrated. No shared gameplay script, existing scene, project setting or backup was modified.

## Component status
| # | Component | Status | Evidence |
|---|---|---|---|
| 1 | First Mission scene `Assets/AstroGateway/FirstMission/Scenes/Mars_FirstMission_v1.unity` (built from Visual Upgrade v1, which is only read) | VERIFIED | `Build.txt`, playtest 105 pass |
| 2 | ARES-VI prefab variant `FirstMission/Prefabs/ARES_VI_FirstMission.prefab` (Animator off, hover plumes off) on the TERRAVEX CharacterController | VERIFIED (Editor + device render) | playtest checks; screenshots 02–05, 09 |
| 3 | Visual suspension, wheel roll, arm deploy and mast pan (visual only) | PARTIAL | Wheels roll and arm deploys/stows (checked). Rocker/bogie tilt only measured on flat ground (0°). The arm is not clearly visible in screenshot 04 |
| 4 | Android landscape controls: astronaut (MOVE, RUN, BOARD, SCAN) and rover (DRIVE tab: steer stick, ACCEL, BRAKE, REVERSE, EXIT, CAM, VIEW, speed, heading/slope) | VERIFIED (Editor); PARTIAL on device | Editor checks for drive, steer, brake, reverse, exit and camera. On the phone, touch moved the astronaut while someone was playing |
| 5 | Tab switching DRIVE / SCIENCE / DATA / MISSION / SYSTEMS; switches automatically on board and exit; no rover controls on foot | VERIFIED (Editor) | 5 tab checks; exit-restore check |
| 6 | Transparent dashboard (DATA tab: reading, unit, time, location, source, upload state, backend processing, AI result, model version) | VERIFIED (Editor) | screenshot 06 |
| 7 | Sensor simulation (`RoverSensorSuite`): navigation, terrain slope/roughness/traversability, dust state, station identification, range and bearing, sample id | VERIFIED | slope = ground normal (0.000 vs 0.000°); range = true distance (11.00 vs 11.00 m) |
| 8 | Not-modelled sensors (temperature, pressure, radiation, soil composition, spectra) are UNAVAILABLE with no value | VERIFIED | client and server checks |
| 9 | Record schema + client validation (`FirstMissionData.cs`), shared catalog `sensor_catalog_v1.json` (byte-identical in Unity and backend) | VERIFIED | pytest catalog check |
| 10 | Offline queue: persistent, bounded (400 records / 1 MB), atomic writes, dedupe, retry with backoff, 401 stop, 409/422 reject | VERIFIED | playtest store tests + MOCK uploads + local backend 12/12 |
| 11 | Local sensor-records endpoint `POST /v1/mission-sessions/{id}/sensor-records` + `GET /v1/science/analysis-status` | VERIFIED (local) | pytest 232/232 (50 new) |
| 12 | Candidate SQL `Docs/FirstMission/SQL/006_sensor_records.sql` + `906` rollback | UNTESTED (not executed, by design) | — |
| 13 | AI interface `ag.science-analysis.v0`: NOT CONNECTED. The client cannot create a result; only a server ack marked BACKEND-ANALYZED could | VERIFIED as an interface; real model BLOCKED | MOCK test only. No model was called |
| 14 | Mission flow: 7 objectives built from stored records, labelled "local guidance, not scored" | VERIFIED (Editor) | 6/7 done in the playtest; screenshot 07 |
| 15 | Android build (IL2CPP ARM64, landscape, dev, from a project copy) | VERIFIED build; PARTIAL device test | see Android below |
| 16 | Android performance | UNTESTED | Only memory was read on the device; no FPS measurement |

## Mission ID check (decision 5)
There is no conflict. `ag.mars.red-horizon.surface-ops` v1 is already canonical in `mission_rules_v1.json`, with read-only aliases for `mission-red-horizon-poc` and the Unity registry ids. The First Mission scene uses its own registry copy with the canonical id. Backend sessions keep `mission-red-horizon-poc` (unchanged), and the sensor endpoint accepts the canonical id or any alias. **One gap (not changed):** `mission.red_horizon.visual_upgrade.v1` is missing from the alias map. The scene has its own save file `AstroGateway_FirstMission_v1.json`, so no older save is overwritten.

## Tests
| Suite | Result | File |
|---|---|---|
| First Mission playtest (graphical Editor Play Mode, 2400×1080, plus 1920×1080, 2340×1080, 1600×720, a simulated 110 px cutout and left-handed mode) | **105 PASS / 0 FAIL / 1 UNTESTED** (the virtual touchscreen event did not reach the UI in this run; it passed in an earlier run) | `Playtest/FirstMission_Playtest_Results.txt`, `Playtest/Touch_Target_Audit.txt` |
| Backend pytest (all suites) | **232/232** (182 existing + 50 new) | run with `MissionRedHorizon_Step1Work/step3-venv` |
| Unity client vs local FastAPI (mock Auth, in-memory repository; not hosted, no AI) | **12/12** | `Backend/Backend_Client_Results.txt` |
| Regression: Visual Upgrade playtest (unchanged scene) | Run 1: 49/1 ("Key D right" moved 0.36 m). Run 2: **50/0**. Treated as a timing flake | `Regression/VisualUpgrade*/` |
| MOCK upload tests (fake uploader, no network) | 10/10, labelled MOCK | in the playtest file |

Bugs found and fixed during testing: a NaN in a sensor record crashed the 422 error body with a 500 (backend, fixed). Left-handed mirroring and a simulated-cutout test helper were wrong (the HUD fix was confirmed by screenshot 13).

## Screenshots (real Unity Game View, 2400×1080 unless named)
`Playtest/Screenshots/`: 01 astronaut mode; 02 driving; 03 / 03a science (observing / out of range); 04 arm sample; 05 science tab; 06 data tab offline; 07 mission; 08 systems; 09 after exit; 10 pause; 11 drive at 1920×1080, 2340×1080 and 1600×720; 12 cutout; 13 left-handed.
Device captures from the phone: `Device/01_launch.png`, `Device/02_after_20s.png`.

## Android
- Build: `MissionRedHorizon_Step1Work/FirstMissionAndroid/Build/AstroGateway_FirstMission_dev.apk`, package `com.astrogateway.firstmission.dev`, IL2CPP ARM64, minSdk 26, landscape only, development build. User assets are 82.5 MB; the 0.84 GB total is mostly development IL2CPP symbols.
- Device: moto g stylus 5G (2024), Android 15, 2400×1080. Installed beside the existing `com.sk2lb.astrogateway.poc`, which was not touched.
- **First run rendered black.** URP stripped the SSAO resources because the scene swaps in its own URP asset at runtime. The fix was applied **in the build copy only**: every quality level uses the scene's `VisualUpgrade_URP` asset. After the rebuild the game renders correctly on the phone, with safe-area offsets visible for the camera cutout. The same change is needed in the main project before any real Android release and needs your approval (project-wide graphics setting).
- On the device: no Unity errors in logcat apart from a harmless Play Asset Delivery class lookup. Memory was 381 MB PSS (31 MB graphics); battery temperature 31 °C.
- Touch on the device: someone was playing during the captures, and the stick moved the astronaut. RUN looks pressed in both captures, which is consistent with a held thumb, but please confirm it releases. Boarding, tabs, observations and FPS were **not** tested on the device.
- Not done: release signing, Play Store readiness, quality presets on device, thermals over time, CPU/GPU frame time on device.

## Editor performance (Mac M4 Pro, not Android)
Driving, DRIVE tab: 29.7 fps (game cap 30), CPU 33.4 ms, GPU 2.8 ms. Parked, DATA tab: 29.6 fps, GPU 6.6 ms. The ARES-VI adds 24 textures at 1024² (~48 MB). An Android 512 px texture override is recommended (not applied).

## Changed files
- New (Unity, untracked): `Assets/AstroGateway/FirstMission/` — Config/sensor_catalog_v1.json; Scripts/FirstMissionData.cs, RoverSensorSuite.cs, AresRoverPresentation.cs, FirstMissionController.cs, FirstMissionHUD.cs; Editor/FirstMissionBuilder.cs, FirstMissionPlaytest.cs, FirstMissionBackendClientTest.cs, FirstMissionAndroidBuild.cs; Prefabs/ARES_VI_FirstMission.prefab; Settings/FirstMissionRegistry.asset; Scenes/Mars_FirstMission_v1.unity.
- New docs: `Docs/FirstMission/` (this report, Build.txt, SQL/, Playtest/, Backend/, Regression/, Device/, logs/).
- Backend (not under git): changed `main.py` (2 lines: import + register), `database.py` (RPC allow-list, 3 error codes, 1 method), `tests/local_mock_server.py` (uses the sensor repository); added `sensor_records.py`, `rules/sensor_catalog_v1.json`, `tests/test_sensor_records.py`.
- Unchanged (verified): all tracked project files (`git status` clean after restoring the harness `runInBackground` flip), the ARES-VI showroom folder (matches backup), and the Visual Upgrade scene and shared scripts.
- Outside the project: Android build copy `MissionRedHorizon_Step1Work/FirstMissionAndroid` (2.7 GB) and the APK installed on the phone.

## Remaining blockers / decisions
1. Scientific AI model: location and contract still unknown. The interface stays NOT CONNECTED.
2. Hosted backend, sign-in hand-off and SQL 006: not approved. Uploads in the game stay OFFLINE.
3. Android graphics fix in the main project (quality levels / URP asset) and a 512 px Android texture override for ARES-VI.
4. Sensor units, ranges and traversability thresholds are PROPOSED and need science review.
5. A hands-on device check of boarding, tabs and science, plus device FPS.
6. Add `mission.red_horizon.visual_upgrade.v1` to the alias map (optional).
7. The seated astronaut is hidden in the rover (no seat model in the ARES canopy).

## Rollback
- Unity: delete `Assets/AstroGateway/FirstMission/` and `Assets/AstroGateway/FirstMission.meta`. Nothing else in the project references them.
- Backend: restore from `~/Projects/MissionRedHorizon_Backups/FirstMission_PreChange_20261020/backend_snapshot.tgz` (main.py, database.py, tests/local_mock_server.py), and delete sensor_records.py, rules/sensor_catalog_v1.json and tests/test_sensor_records.py.
- Phone: `adb uninstall com.astrogateway.firstmission.dev`.
- Build copy: delete `~/Projects/MissionRedHorizon_Step1Work/FirstMissionAndroid`.
- Full repository state before this work: `git clone ~/Projects/MissionRedHorizon_Backups/FirstMission_PreChange_20261020/repo_all.bundle` (verify with `shasum -a 256 -c SHA256SUMS.txt`).
- Player data: `<persistentDataPath>/AstroGateway_FirstMission/` and PlayerPrefs keys `AstroGateway.FirstMission.v1.*` belong to this scene only.
