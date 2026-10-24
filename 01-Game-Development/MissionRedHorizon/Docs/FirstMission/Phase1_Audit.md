# First Mission Rover — Phase 1 audit (2026-10-20)

Read-only audit. No project, scene, script, setting or backend file was changed. The only write was a new backup (below) and this report.

## 1. Repository, processes, backups
| Check | Result | Status |
|---|---|---|
| Project | `/Users/kapilash/Projects/MissionRedHorizon` (Unity 6000.6.4f1, URP), own git repo, no remote | VERIFIED |
| Branch / HEAD | `visual-upgrade` @ `5e0c223` (Batch 5 HUD). Uncommitted: only untracked `Assets/AstroGateway/VehicleShowroom/` (ARES-VI), `Docs/VehicleShowroom/`, batch logs | VERIFIED |
| Concurrent processes | No Unity Editor or batch Unity running; no uvicorn/FastAPI running. VS Code C# language server is attached to the project (harmless, but close VS Code before batch runs if it triggers reimports) | VERIFIED |
| Previous backup | `MissionRedHorizon_Backups/visual-upgrade_20261019_1508.bundle` stops at `e3c67bf`: it did **not** cover the 5 commits since (43843eb…5e0c223) or the ARES-VI files | was STALE |
| New backup | `MissionRedHorizon_Backups/FirstMission_PreChange_20261020/`: `repo_all.bundle` (all branches, `git bundle verify` okay, contains 5e0c223), `untracked_VehicleShowroom_Docs.tgz` (213 entries), `backend_snapshot.tgz` (29 entries; `.env`, certs and venv excluded on purpose), `SHA256SUMS.txt` | VERIFIED |
| Android | AndroidPlayer module installed. `adb devices`: no device attached. Package id is still the Unity template placeholder `com.UnityTechnologies.com.unity.template.urpblank`; Build Settings list only `Assets/Scenes/SampleScene.unity` | VERIFIED (no device) |

## 2. Current rover (scene `VisualUpgrade/Scenes/Mars_VisualUpgrade_v1.unity`, newest playable)
| Item | What exists | Status |
|---|---|---|
| GameObject | `02_LocalProvisionalExploration/Rover_DrivableKinematicPrototype` (TERRAVEX-R1) | VERIFIED |
| Physics | **Kinematic `CharacterController`**, not Rigidbody/WheelColliders. `MarsPrototype.Drive()`: speed ramps to 3.8 m/s forward / 1.8 m/s reverse, brake decel 6 m/s², yaw 48°/s scaled by speed, Mars gravity 3.71, world clamp ±119 m. No suspension simulation, no slope/tilt following (chassis stays level) | VERIFIED |
| Wheels | 6 primitive wheels (3 per side) with LODs; visual roll only (`Rotate` by speed) | VERIFIED |
| Rocker-bogie | One static mesh "TERRAVEX static rocker and protective frame" + 6 suspension links, shock absorbers and springs — all static, no articulation | VERIFIED |
| Mast / cameras / antenna | "Scientific sensor mast", "Camera sensor", antenna mast, high-gain antenna, mast beacon (procedural, temporary) | VERIFIED |
| Robotic arm / sample tools | **None on the rover** | VERIFIED (missing) |
| Colliders | Only the CharacterController capsule; visual parts have no colliders | VERIFIED |
| Board / exit | `NativeSliceGame.Interact()` → `MarsPrototype.Interact()`: board within 5 m; exit only below 0.2 m/s, to a grounded collision-free point ±3 m sideways within 1.2 m height (roof-exit fix) | VERIFIED |
| Camera | `VisualUpgradeCameraRig` (scene) over `MarsPrototype.LateUpdate` follow; chase/near/overview/cinematic modes; spherecast obstruction | VERIFIED |
| ARES-VI upgraded rover | Separate showroom scene `VehicleShowroom/Scenes/ARES_VI_Showroom.unity`, untracked; not in the Mars scene (awaits your approval) | VERIFIED |
| Reference image | The message mentions an attached Mars Rover image, but **no image reached this thread** | BLOCKED |

## 3. Mobile controls and HUD
| Item | What exists | Status |
|---|---|---|
| Touch input | `NativeControlSlice/AdaptiveTouch.cs`: EnhancedTouch, floating joystick, BOARD/EXIT, RUN↔ACCELERATE, BRAKE, SCAN, right-half camera drag, left-handed mode, safe-area layout. Drawn with **IMGUI `GUI.Box`** (debug look) | VERIFIED (code), UNTESTED on device |
| Second touch layer | `Stage08C/MobileControls.cs` (older, created by MarsPrototype; disabled by NativeSliceGame) and uGUI touch controls inside the HUD (hidden on desktop via CanvasGroup) | VERIFIED |
| Mode switching | `ControlModeManager` enum `Astronaut, Rover, Menu, Robot, Cockpit` (Robot/Cockpit refused). Labels switch BOARD→EXIT, RUN→ACCELERATE, but the **same button set** shows in both modes; there are no tabs | VERIFIED |
| HUD | Runtime-built uGUI (`CinematicHUD/AstroGatewayHUDController.cs`, 31 KB, shared) restyled per scene by `VisualUpgradeHUDFixes/HUDStyle`. Legacy `Text`, no TextMeshPro, font Avenir Next loaded from macOS (**not embedded — will not exist on Android**). Visibility profiles via F1, settings in PlayerPrefs shared with older scenes | VERIFIED |
| Orientation | Player setting = auto-rotate (all 4); `AdaptiveTouch` forces LandscapeLeft at runtime on Android. No approved orientation is written down | PARTIAL — needs decision |
| Existing panels relevant to tabs | Survival resources, weather/clock, objectives, minimap, scanner status, recent scan card, rover speed gauge, rover telemetry, modals (pause, settings, samples, inventory, base, rover reports, map) | VERIFIED |

## 4. Scanning and "sensor" data today
- Scans happen only **on foot**: `CurrentTarget()` returns null while driving; range 3 m to a `ResearchTarget` (5 stations).
- `NativeSliceGame.Scan()` stores `ScientificScan{sampleId, location, tick, syntheticIndex=(count%7)/7}` labelled `simulated_surface_scan / prototype.synthetic_scanner / unverified_simulated`. The index is a **counter pattern, not a measurement**.
- Telemetry (`TelemetryHistory`, 256 records, 1 Hz): rover battery, speed, health, astronaut oxygen — all `valid_simulated` game state.
- No temperature, pressure, dust, radiation, slope, roughness, spectral or heading sensors exist. Weather is 3 manual states (fog/dust particles). `IWeatherFeed` reports temperature/wind as not connected.
- Units/ranges reviewed by a scientist: none yet (Step 4 rules config is still PROPOSED).

## 5. Mission session and backend contracts
| Item | What exists | Status |
|---|---|---|
| Unity contracts | `Integration/Contracts/IntegrationContracts.cs`: identity, mission session + `MissionCommand{command_id, expected_revision, kind, payload}`, assessment, weather, life support, recommendations | VERIFIED |
| Offline queue | `LocalMissionSessionService`: memory-only, max 256, **lost on app quit** | VERIFIED |
| HTTP client | `Integration/Http/HttpMissionSessionService.cs` + `ShellIdentityProvider` (token from native shell, not stored). Tested 18/18 against local server in Step 3; **not wired into any scene** | VERIFIED (local) |
| Wired scene | `MissionIntegration/Scenes/Mars_MissionIntegration_v1.unity` (copy of Production Visual, older than Visual Upgrade) | VERIFIED |
| FastAPI (`AstroGatewayAndroid/backend`, not under git) | `GET /health`, `GET /v1/me`, `POST /v1/me/display-name`, `POST /v1/mission-sessions`, `POST …/{id}/complete`, `POST …/{id}/commands`, `GET …/{id}`, `GET /v1/reports/{id}`, `GET /v1/skills`, `GET /v1/recommendations`, `POST /v1/auth-session/revoke`. Supabase JWT auth, per-user RPC/RLS, idempotent commands, 409 on revision conflict, 1 KB payload cap, 500 commands/session | VERIFIED (code) |
| Command kinds | `board_rover, exit_rover, scan_target, weather_changed, pause, resume`; scan payload `analysis` must be `unverified_simulated`. **No sensor-record or analysis endpoint** | VERIFIED |
| Mission id | Backend literal `mission-red-horizon-poc`; Unity `mission.red_horizon.slice`; Step 4 proposal `ag.mars.red-horizon.surface-ops` — **not approved** | BLOCKED (decision) |
| Running the real service | README: startup is refused in the current operator-owned layout until isolation is set up; no hosted Supabase test project approved; SQL 005 not executed | BLOCKED |
| Secrets | Backend `.env` exists (not read). Unity code contains no keys; design passes the user JWT from the native shell | VERIFIED |

## 6. Custom AI model — what actually exists
Searched `~/Projects` and `~/Documents` (code, docs, model files `.onnx/.pt/.pkl/.joblib/.tflite/.safetensors/.gguf/.sentis`). Findings:
- **No scientific sensor-analysis model exists on this Mac.** Backend dependencies are only FastAPI/uvicorn/httpx; no inference code or model route.
- Only model found: `Documents/Codex/2026-10-08/.../outputs/Stage07B/Development/stage07b/model.py` — a five-head **logistic regression for player skill assessment**, trained on 600 **synthetic** rows (macro test accuracy 0.684, Brier 0.198), JSON weights, stdlib only, explicitly "not integrated into FastAPI or product inference" and "scientific observations are not used to train this skill model". Wrong task for rover sensor analysis.
- "ARIA" (`stage07b/aria.py`) is two fixed bilingual (English/Tamil) template cards with NASA URLs and abstention — not a trained model.
- `.tflite` files found belong to an unrelated AutoVRL ML Kit barcode build.

Model purpose, input/output schema, tasks, version, latency, auth, error behaviour, uncertainty and validation status therefore **cannot be determined**: BLOCKED until your team shares where the model lives (repo/path/endpoint) and its contract.

## 7. What Phases 2–13 would need (and what I'll stop on)
| Phase | Can start now | Needs your approval / input first |
|---|---|---|
| 2 Rover visuals | Material/detail pass on a **prefab variant** in a new First Mission scene | Which base: current TERRAVEX procedural rover or the upgraded ARES-VI (moving ARES-VI into the game needs your OK); the reference image |
| 3 Tab controls | Yes, in the new scene only | Replacing IMGUI `AdaptiveTouch` drawing with uGUI tabs is a UI-architecture change → approve |
| 4 HUD | Yes, uGUI + existing design system | Embedding a font (Avenir is macOS-only; needs a licensed/OFL font) and landscape-only orientation |
| 5 Sensors | Navigation (position, heading, speed, distance) and terrain (slope from ground normal, roughness from raycast variance) can be derived from real Unity state | Environmental values (temperature, pressure, dust, radiation) need a science-reviewed model or must show UNAVAILABLE; enabling scans from the rover |
| 6/9 Pipeline + offline queue | Local schema, validation, persistent bounded queue, idempotent event ids | New backend endpoint `POST …/sensor-records` and SQL table = backend change → approve |
| 7/8 AI | Adapter interface + clearly-labelled MOCK responder for tests | The real model (blocked, §6) |
| 10 Mission flow | Local objectives in the new scene | Mission id choice; no scoring changes |
| 11/12 Android | Mac Editor measurements | A physical Android device; Android package id |

## Decisions needed
1. Rover base: TERRAVEX (current, keeps verified physics) or ARES-VI visuals on TERRAVEX physics.
2. Approve the UI change: new uGUI tabbed rover controls in an isolated First Mission scene (shared scripts untouched).
3. Where is the custom AI model (repo/path/URL) and who owns its contract?
4. Orientation: landscape-only on Android?
5. Canonical mission id.
6. Approve adding a backend sensor-records endpoint (local only, not deployed).

## Rollback
Nothing to roll back yet. Backup: `~/Projects/MissionRedHorizon_Backups/FirstMission_PreChange_20261020/` (restore repo with `git clone repo_all.bundle`; verify with `shasum -a 256 -c SHA256SUMS.txt`).
