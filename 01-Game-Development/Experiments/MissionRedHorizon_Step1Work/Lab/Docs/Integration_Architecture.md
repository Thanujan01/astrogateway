# Integration architecture: backend, accounts, scoring, weather, oxygen, recommendations

Status: client-side seams only (Step 1, 2026-10-16). No network calls, no cloud deployment, nothing added to any scene.
Backend design source: `Documents/Codex/2026-10-08/task-01-create-unity-6-urp/outputs/Architecture/02_Identity_And_User_Journey.md`, `03_Data_API_And_Learning.md`, and `outputs/UnifiedAuth/Shared_Identity_Contract.md`.

## Where the code lives
| Path | What it is |
|---|---|
| `Assets/AstroGateway/Integration/Contracts/IntegrationContracts.cs` | Interfaces and data shapes: identity, mission session + commands, assessment report, weather, life support, recommendations |
| `Assets/AstroGateway/Integration/Local/LocalServices.cs` | Offline defaults (`IntegrationServices`). Same behaviour as today; every result is marked non-authoritative |
| `Assets/AstroGateway/Integration/Local/GameplayEventBridge.cs` | Read-only observer of `NativeSliceGame` that turns board/exit/scan/pause/weather changes into semantic commands. Not placed in any scene yet |
| `Assets/AstroGateway/Verification/Editor/Step1GameplayVerification.cs` | Batch-mode gameplay + integration test (lab copy only; see the progress report) |

## How it maps to the planned backend
| Game system | Contract | Planned backend (FastAPI + Supabase) | Today (offline default) |
|---|---|---|---|
| Unified user accounts | `IIdentityProvider` | Supabase Auth UUID; `GET /v1/me`; token from the web/Android shell | `OfflineIdentityProvider`, not authenticated |
| Mission progress | `IMissionSessionService` + `MissionCommand` | `POST /v1/mission-sessions`, `/commands` with `commandId` + `expectedRevision` (idempotent, 409 on conflict) | `LocalMissionSessionService`, memory-only queue (max 256) |
| Mission scoring / skill assessment | `IMissionAssessment` → `MissionReport` | Server rubric on `/complete`; `GET /v1/reports/{id}`, `/v1/skills` | `LocalProvisionalAssessment`, preview only, "unreviewed" rubric |
| Weather | `IWeatherFeed` → `WeatherState` | Authored mission weather timeline or science data feed (source TBD) | `PrototypeWeatherFeed` reads the existing 3-state dust weather |
| Oxygen | `ILifeSupportModel` | Validated oxygen/time model (needs science review) | `PrototypeLifeSupportModel`, identical to MarsPrototype rates |
| Recommendations | `IRecommendationService` | `GET /v1/recommendations` (deterministic, server-side) | `FixedRecommendationService`, first mission |

Rules carried over from the architecture docs: the client sends semantic actions, never scores or resource totals; no per-frame API traffic; server time is authoritative; nothing local is labelled NASA data or an official score.

## Not done yet (needs approval / inputs)
1. Real HTTP client for the broker (`UnityWebRequest`), token hand-off from the Android/web shell, retry and offline replay of `Pending`.
2. Wiring: add `GameplayEventBridge` to the production scene, and route the HUD's oxygen, weather and score panels through the contracts. The HUD currently reads the simulated values directly.
3. Backend endpoints for commands/complete/reports/recommendations (only `/v1/me` and display name exist and were tested locally).
4. Science sign-off for the oxygen model and scoring rubric weights.
5. Hosted environment approval (Stage05B3 gates in the UnifiedAuth report are still open).
