# Integration architecture: backend, accounts, scoring, weather, oxygen, recommendations

Status (2026-10-17): client-side seams, wired into ONE isolated scene `Assets/AstroGateway/MissionIntegration/Scenes/Mars_MissionIntegration_v1.unity` (a copy of Production Visual v1). No network calls, no cloud deployment. The original production scene is unchanged.
Backend design source: `Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Architecture/02_Identity_And_User_Journey.md`, `03_Data_API_And_Learning.md`, and `outputs/UnifiedAuth/Shared_Identity_Contract.md`.

## Where the code lives
| Path | What it is |
|---|---|
| `Assets/AstroGateway/Integration/Contracts/IntegrationContracts.cs` | Interfaces and data shapes: identity, mission session + commands, assessment report, weather, life support, recommendations |
| `Assets/AstroGateway/Integration/Local/LocalServices.cs` | Offline defaults (`IntegrationServices`). Same behaviour as today; every result is marked non-authoritative |
| `Assets/AstroGateway/Integration/Local/GameplayEventBridge.cs` | Read-only observer of `NativeSliceGame` that turns board/exit/scan/pause/weather changes into semantic commands |
| `Assets/AstroGateway/Integration/Local/LifeSupportModelAdapter.cs` | Lets `ILifeSupportModel` set suit oxygen use without editing MarsPrototype (default model = identical legacy rates) |
| `Assets/AstroGateway/MissionIntegration/Scripts/MissionIntegrationController.cs` | Scene glue + small read-only "MISSION LINK • PROVISIONAL" status panel (toggle: I key; hides with the HUD) |
| `Assets/AstroGateway/MissionIntegration/Editor/MissionIntegrationBuilder.cs` | `Build()` copies the production scene (refuses to overwrite) and adds the components; `Validate()` runs 26 integration checks + Game View captures (env `STEP2_OUT`) |
| `Assets/AstroGateway/Verification/Editor/Step1GameplayVerification.cs` | Batch-mode gameplay + integration test (lab copy only; see the progress report) |

## How it maps to the planned backend
| Game system | Contract | Planned backend (FastAPI + Supabase) | Today (offline default) |
|---|---|---|---|
| Unified user accounts | `IIdentityProvider` | Supabase Auth UUID; `GET /v1/me`; token from the web/Android shell | `OfflineIdentityProvider`, not authenticated |
| Mission progress | `IMissionSessionService` + `MissionCommand` | `POST /v1/mission-sessions`, `/commands` with `commandId` + `expectedRevision` (idempotent, 409 on conflict) | `LocalMissionSessionService`, memory-only queue (max 256) |
| Mission scoring / skill assessment | `IMissionAssessment` → `MissionReport` (+ `SkillEvidence`) | Server rubric on `/complete`; `GET /v1/reports/{id}`, `/v1/skills` | `LocalProvisionalAssessment`: evidence counts only, score = not scored (rubric pending approval) |
| Weather | `IWeatherFeed` → `WeatherState` | Authored mission weather timeline or science data feed (source TBD) | `PrototypeWeatherFeed` reads the existing 3-state dust weather |
| Oxygen | `ILifeSupportModel` + `LifeSupportModelAdapter` | Validated oxygen/time model (needs science review) | `PrototypeLifeSupportModel`, identical to MarsPrototype rates; no consequence at 0% (unapproved survival rules not implemented) |
| Recommendations | `IRecommendationService` | `GET /v1/recommendations` (deterministic, server-side) | `FixedRecommendationService`, first mission |

Command wire format (snake_case, like the existing FastAPI models): `{"command_id","expected_revision","kind","payload"}` via `MissionCommand.ToBrokerJson()`. Commands are only queued on the device; nothing is transmitted.

Rules carried over from the architecture docs: the client sends semantic actions, never scores or resource totals; no per-frame API traffic; server time is authoritative; nothing local is labelled NASA data or an official score.

## Not done yet (needs approval / inputs)
1. Real HTTP client and shell token hand-off (Step 3 plan: `Docs/Backend_Mission_Endpoints_Plan.md`).
2. Backend endpoints for commands/state/reports/skills/recommendations.
3. Approved rules (Step 4): oxygen consequence, weather timeline, scoring rubric, recommendation logic.
4. Weather "vis" in the panel is the rendered fog distance, not a science value; wind/temperature are not connected.
5. Hosted environment approval (Stage05B3 gates in the UnifiedAuth report are still open).
