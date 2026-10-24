from pathlib import Path
p=Path('outputs/Architecture')
files={
'00_Read_Me.md':'''# AstroGateway unified platform architecture proposal

Team SK2LB-AstroLab · 8 October 2026 · Planning only, awaiting approval.

**Recommendation: a native Kotlin Android host, a bundled Unity as a Library module, one FastAPI service, and one Supabase Auth/PostgreSQL project.** Start with Android Views for the small host interface. Preserve the existing React prototype for later web work. Do not introduce React Native in the seven-day MVP.

This proposal is based on read-only project inspection and official documentation. It is not proof that an integrated Android build works. Android Build Support is currently missing; the first implementation milestone must resolve that dependency and test the exact Unity export on a physical phone.

| Requested deliverable | Document |
|---|---|
| System architecture, Android recommendation, integration comparison | [01_System_And_Platform.md](01_System_And_Platform.md) |
| SSO, secure handoff, lifecycle and navigation | [02_Identity_And_User_Journey.md](02_Identity_And_User_Journey.md) |
| Database proposal, FastAPI contracts, educational assessment | [03_Data_API_And_Learning.md](03_Data_API_And_Learning.md) |
| Packaging, future platforms, folder structure, seven-day roadmap | [04_Delivery_And_Roadmap.md](04_Delivery_And_Roadmap.md) |
| Project findings, references, risks, decisions | [05_Evidence_And_Decisions.md](05_Evidence_And_Decisions.md) |

The MVP is one complete, measurable training mission: prepare for EVA, investigate solar generation and load priorities, record geological evidence, return safely, and receive a saved scientific reasoning report. Oxygen, water recycling, radiation and hydroponics receive short authored learning decisions; full simulations of all systems are outside the seven-day scope.

One account means one Supabase user identity and shared progress across clients. Within Android, the host and Unity share the host's authenticated session. Different devices have their own sessions for that identity; signing in on a phone does not automatically authenticate a browser on another device.

The plan assumes two implementation contributors and part-time scientific/QA review. With one developer, the complete scope is unlikely to fit seven days. Approval should precede any dependency installation, application creation, project edits, cloud configuration or build.

No application code, migrations, dependencies or builds were created in this planning stage. The preservation check records hashes of 334 existing project files.
''',
'01_System_And_Platform.md':'''# Unified system and platform decision

## Decision and boundaries

Use **Kotlin Android host + Unity as a Library + FastAPI + Supabase**. Package the dashboard and Mars trainer under one application ID and one installation. Unity opens full-screen inside an Activity in the same app, with its runtime in a private `:unity` process. The dashboard and authentication broker stay in the main process.

```mermaid
flowchart TD
    User[Android user] --> Host[Kotlin host: account, dashboard, progress, settings]
    Host --> Auth[Supabase Auth: one user identity]
    Host --> Broker[Main-process authenticated session broker]
    Host --> Game[Private Unity Activity and :unity process]
    Game <-->|Versioned private IPC; no credentials| Broker
    Broker -->|HTTPS and short-lived Supabase JWT| API[FastAPI: authorization, missions, simulation, assessment]
    API -->|Validate issuer and signing keys| Auth
    API --> DB[Supabase PostgreSQL: private training schema]
    Web[Future React web host] --> Auth
    Web --> API
    Web <-->|Validated JavaScript bridge| WebGame[Separate Unity Web build]
```

The Unity process boundary is a lifecycle choice, not an anti-cheat or secret-isolation guarantee: both processes belong to the same installed app. Keep secrets out of the game to reduce accidental leakage and avoid two refresh coordinators.

| Component | Responsibility | Does not own |
|---|---|---|
| Android host | Signup, signin, reset, dashboard, profile, secure session, navigation, preferences | Mission scoring rules |
| Unity | Movement, camera, scene interactions, local visual previews, mission presentation | Password verification, permanent credentials, authoritative score |
| Private broker | Allowed mission calls, refreshed authorization, IPC responses | Arbitrary proxy requests chosen by Unity |
| FastAPI | Ownership checks, state transitions, deterministic resource model, rubric, recommendations, reports | A second account/password system |
| Supabase | Central Auth and persistent PostgreSQL data | Automatically trusting client scores |

## Comparison for this seven-day sprint

| Dimension | React Native + Unity library | Native Kotlin + Unity library — selected | Single Unity Android application — contingency |
|---|---|---|---|
| Integration | RN/native/Unity boundaries; wrapper compatibility must be proven | Android/Unity boundary; official native example provides a starting point | Avoids host/runtime integration, but host UI must be built in Unity |
| Build | RN Gradle/JS tooling plus Unity export and bridge plugins | One Android Gradle host plus Unity export; still sensitive to AGP versions | Unity Android export; Android support still required |
| Performance | Extra JS runtime and bridge overhead; must measure | No additional JS runtime; Unity remains the main GPU/RAM cost | Unity resident during dashboards unless scene/resource management changes |
| Auth handoff | Native secure storage and Unity bridge still needed | One native Auth coordinator and private broker | One separate Auth service inside the app; Unity never verifies passwords |
| Navigation | RN routing plus Android Unity Activity lifecycle | Small native screen stack plus explicit game Activity | Unity screen/scene navigation |
| Lifecycle | RN and Unity state restoration; third-party adapter risk | Main host survives tested Unity process exit; IPC reconnect required | Fewer integration boundaries, all app state shares runtime |
| Maintenance | More frameworks and compatibility matrices | Android-specific UI; small, explicit bridge | Simpler packaging, weaker reuse of host UI on web |
| Future web | Share some TypeScript logic; RN views are not existing Vite DOM views | Share API contracts, content and design; rebuild host screens in React | Export game for web; separate web host still needed |

This is a judgment based on current project state, not a benchmark. The React project is a small Three.js scene without implemented account/dashboard features, so switching to React Native would not preserve a substantial existing host application. There is no verified RN wrapper in this proposal for Unity 6000.6.4f1 and its current Android toolchain. The [React Native integration guide](https://reactnative.dev/docs/integration-with-existing-apps.html) describes native application integration, not automatic Unity embedding.

The [Unity Android library documentation](https://docs.unity.com/en-us/engine/6000.6/manual/platform-specific/android/developing/unityasa-library) supports native embedding, limits the ordinary runtime to full-screen rendering and one instance, and distinguishes unloading from quitting. The [official Unity example](https://github.com/Unity-Technologies/uaal-example/blob/master/docs/android.md) uses a separate Unity Activity/process. It is a reference architecture, not a tested drop-in solution for this project.

## Integration plan

1. After approval, install Android Build Support with the matching SDK/NDK/JDK. Export a minimal scene using the existing Editor, before adding Auth.
2. Create a small native host and wire the generated `unityLibrary` module into it. Retain the generated launcher as an export reference; ship only the host launcher.
3. Select one supported generated Activity or GameActivity entry point after inspecting this version's export. Do not copy old UnityPlayer constructors or manifest snippets without verification.
4. Place the Unity Activity in `:unity`; keep it private. Configure host Application initialization to start the Auth coordinator only in the main process.
5. Implement a tiny private bound-service IPC bridge. For the MVP, Messenger with bounded, serialized requests is sufficient; AIDL is unnecessary unless concurrency requirements grow. Validate the platform-provided caller UID, protocol version, launch ID, message size and allowed command kinds. Never trust a supplied user ID.
6. Dispatch network work off UI threads and Unity callbacks on Unity's main thread. Attach correlation IDs, timeouts and explicit error responses. Reject obsolete launch messages after exit or account changes.
7. Test launch → ready → echo → return → relaunch before adding online mission logic. Verify process death, Android Back, focus loss and release stripping.

The bridge is custom application work, not provided by Unity automatically. Android explains these IPC choices in its [bound service](https://developer.android.com/develop/background-work/services/bound-services) and [AIDL guidance](https://developer.android.com/develop/background-work/services/aidl).

## MVP stack

| Layer | Proposed choice | Version/compatibility status |
|---|---|---|
| Android host | Kotlin, Android Views, one Activity screen stack plus Unity Activity | Exact Kotlin/Android library pins established during build gate |
| Game | Existing Unity 6000.6.4f1, URP 17.6.0; C# runtime components after approval | Locally inspected; Android integration untested |
| Auth client | Supabase Kotlin client behind an AuthRepository adapter | Community-maintained; version and secure-storage hook must be validated |
| API | Python FastAPI, typed request/response models, JWT verification library | No backend exists; pin compatible versions during implementation |
| Data | One Supabase project, Auth plus PostgreSQL private training schema | No cloud configuration inspected or created |
| Contracts | Versioned JSON DTOs, OpenAPI, mission content and rubric versions | Proposed; no generated clients or code yet |
| Later web | Existing React/Vite host plus a separate Unity Web build | Existing prototype preserved; account screens not present |

Compose, React Native, Realtime subscriptions, microservices and an LLM adaptation system add no essential value to the first complete mission. Reconsider them when the vertical slice is reliable.
''',
'02_Identity_And_User_Journey.md':'''# Identity, game handoff and application navigation

## Android journey

```mermaid
flowchart LR
    Launch[Open app] --> Restore{Restore valid session?}
    Restore -->|No| Login[Sign in / Sign up / Password reset]
    Login --> Confirm[Email confirmation or reset callback when required]
    Confirm --> Dashboard[Dashboard]
    Restore -->|Yes| Dashboard
    Dashboard --> Profile[Profile and skill progress]
    Dashboard --> History[Mission history and reports]
    Dashboard --> Settings[Account, audio, graphics, accessibility]
    Dashboard --> Brief[Mission briefing and controls]
    Brief --> Loading[Create or resume session; launch Unity]
    Loading --> Game[Full-screen Mars trainer]
    Game --> Pause[Pause / resume / return]
    Game --> Debrief[Committed mission report]
    Pause --> Dashboard
    Debrief --> Dashboard
```

Signup collects email/password through Supabase Auth and a minimal display alias. Show confirmation-pending state when the provider requires it. Password reset must include an approved app callback and a usable recovery screen; it is not merely a button sending email. Do not collect date of birth, precise location or health information for this demo.

Dashboard presents one playable mission, saved progress, the next recommended activity and a resume option. Incomplete missions must not appear completed. Settings include audio level, low/standard graphics, readable text, contrast-friendly markers and reduced motion. Confirm signout while a mission is active and offer save/return first.

## One session coordinator

The main-process host owns the Supabase session and refresh operation. Serialize refresh requests and atomically save the new refresh token before publishing the refreshed session. The Unity process must not initialize a second Auth client.

Persist an encrypted session record in app-private storage using a key protected by Android Keystore; Keystore protects keys, it is not a raw token database. Exclude the record from backups. Keep access tokens in memory where practical. Key invalidation, restore or corrupted storage returns the user to signin cleanly. Review the selected SDK's storage implementation rather than assuming its defaults meet this policy. [Android Keystore documentation](https://developer.android.com/privacy-and-security/keystore).

Supabase URL and publishable key can be client configuration. Database passwords, signing secrets, secret keys and service-role keys must remain server-side. [Supabase API key guidance](https://supabase.com/docs/guides/getting-started/api-keys).

The [Kotlin client is community-maintained](https://supabase.com/docs/reference/kotlin/introduction), although documented by Supabase. Its version, refresh behavior, callback handling and custom encrypted persistence need acceptance tests. Do not describe it as an official SDK or claim Android compatibility before compilation.

## Handoff without transferring credentials to Unity

```mermaid
sequenceDiagram
    participant H as Android host / broker
    participant A as Supabase Auth
    participant F as FastAPI
    participant U as Unity process
    H->>A: Signin or refresh
    A-->>H: Access JWT and rotated refresh token
    H->>F: Create mission session with Bearer access JWT
    F-->>H: Session ID, versioned configuration, revision
    H->>U: Launch ID and non-secret mission descriptor
    U->>H: Ready handshake through private IPC
    H-->>U: Mission snapshot and permitted controls
    U->>H: Typed decision, command ID, expected revision
    H->>F: Authorized mission command
    F-->>H: Canonical state and feedback
    H-->>U: Correlated response
    U->>H: Complete request
    H->>F: Validate and commit completion transaction
    F-->>H: Immutable report ID
    H-->>U: Completion receipt; allow return
    H->>H: Show refreshed dashboard/report
```

Launch descriptor: protocol version, launch UUID, mission-session UUID, mission/content/rubric versions, approved difficulty, quality preferences and initial state. Exclude passwords, JWTs, refresh tokens and email. Session IDs are identifiers, not bearer credentials.

The broker binds the launch to the current authenticated user and a specific session. It allows only mission operations; Unity cannot request arbitrary URLs, headers or account changes. HTTP requests use the host's short-lived access token. Small command envelopes have correlation IDs and bounded payloads. All input remains untrusted; FastAPI verifies ownership and valid transitions.

This is session continuity through an authenticated host broker, rather than a second Unity signin or custom permanent game token. Do not add a second identity provider. Avoid credentials in Intents, URLs, deep links, PlayerPrefs, logs, screenshots, analytics and build artifacts.

## API authorization and revocation

FastAPI must verify the Supabase JWT signature and allowed algorithm against the configured project's signing keys, plus issuer, expected audience, expiry and authenticated role; derive `user_id` from its validated `sub`. Reject anonymous users for saved training. Never authenticate by decoding alone or by trusting a body user ID. Use a fixed JWKS endpoint, bounded cache and a single refresh on unknown key IDs. For a legacy symmetric-signing project, use a supported Auth-server verification path until asymmetric signing is configured; do not ship its signing secret to clients. [Supabase JWT documentation](https://supabase.com/docs/guides/auth/jwts).

Refresh tokens rotate, so independent host/game refresh loops can race. Restore/refresh failures pause authenticated operations and route to signin without silently reassigning the mission. [Supabase session behavior](https://supabase.com/docs/guides/auth/sessions).

For explicit in-app signout, first ask FastAPI to revoke the current Auth `session_id` for training API access, then perform explicit local Supabase signout and erase storage. The backend checks its revocation ledger before serving protected calls. Abandon or pause active missions by documented user choice. Clear broker launch bindings and Unity state before another account signs in.

Supabase access JWTs can remain valid until expiry after signout. The training revocation ledger handles online signout through this app, not every external provider event automatically. If revocation cannot reach the backend, clear local credentials immediately and disclose that server-side revocation is pending/token-expiry bounded; do not claim instantaneous global logout. Remote logout/password recovery invalidation requires a separately validated provider-session check or short expiry window. Set the proposed access expiry to 10–15 minutes, verify project configuration, and do not invent a token TTL. [Supabase signout limitations](https://supabase.com/docs/guides/auth/signout).

## Lifecycle contract

| Event | Required behavior |
|---|---|
| Android Back | Game pause overlay: resume, save/return, or confirm abandon |
| Home/screen lock/focus loss | Pause input/audio and mission simulation; checkpoint at an allowed boundary |
| Network loss | Show offline state; local movement may continue, authoritative decisions/completion wait |
| Broker disconnect | Pause authenticated actions, reconnect with bounded retry, request canonical snapshot |
| Host process death | Restore encrypted Auth, verify session, rebind, reload server snapshot; no duplicate Auth clients |
| Unity process death | Dashboard offers resume; reload validated session and checkpoint |
| Account switch/signout | Invalidate launch ID and clear game state before new session |
| Completion response lost | Fetch existing report or retry same command ID; never award twice |
| Exit game | Wait for save receipt or explicitly show unsaved state; foreground host and test cleanup callback |

Unloading Unity does not promise complete memory release. Quitting ends the process hosting Unity; separate-process configuration must be verified so it does not end the dashboard. Use generated Unity lifecycle interfaces for this Editor version. Navigation recovery must also work when Android removes both processes. No seamless offline completion or untested crash-proof lifecycle claims are part of the MVP.
''',
'03_Data_API_And_Learning.md':'''# Data model, API contracts and measurable learning

All tables and endpoints below are proposals. No SQL migrations, databases, API server or application code have been created.

## PostgreSQL boundary

Use one Supabase database. Keep application tables in a private `training` schema that is not exposed through the public Data API. Android, Unity and future web access application data through FastAPI. Supabase Auth retains its own managed user records.

FastAPI uses a dedicated non-owner database role with only required privileges and no `BYPASSRLS`; keep credentials in server environment configuration. Every user-data query must include the user identity derived from verified authorization, including nested resources. Use parameterized queries and transactions. Do not let a client set ownership or score fields.

Enable RLS as defense in depth on user-owned tables with default-deny access. The restricted backend role may use transaction-local identity context set only from the verified JWT for owner policies; policies and connection-pool context cleanup must be tested. Catalog reads have explicit grants. No frontend role can insert results, skill evidence or awards. Do not assume service-role/secret credentials are protected by RLS; they bypass it. [Supabase RLS](https://supabase.com/docs/guides/database/postgres/row-level-security) and [secure-data guidance](https://supabase.com/docs/guides/database/secure-data).

## Schema proposal

| Table | Key and essential fields | Ownership / constraints |
|---|---|---|
| profiles | user_id UUID → auth.users; display_alias, preferences, created_at, updated_at | One per user; no passwords or duplicated email; restrict editable fields |
| missions | mission_id + version; title, content_version, rubric_version, references, required_steps, difficulty_rules | Read-only published catalog; existing sessions retain pinned versions |
| mission_sessions | id UUID; user_id, auth_session_id, mission_id/version, difficulty, status, revision, canonical_state, timestamps | Owner only; active/paused/completed/abandoned state; optional one active session per mission/user |
| mission_commands | session_id + command_id UUID; expected_revision, kind, bounded payload, accepted_at, response | Unique pair enables idempotency; derives owner from session; server sequence |
| mission_results | session_id primary key; user_id, score, rubric_version, evidence_summary, feedback, completed_at | At most one immutable result per session; score bounds 0–100 |
| skill_evidence | session_id + skill_id; user_id, criterion outcomes, hints_used, attempts, evidence_strength | At most one contribution per skill/session; traceable to report |
| user_skills | user_id + skill_id; derived_level, evidence_count, updated_at | Recomputable cache; clients cannot write mastery |
| auth_session_revocations | auth_session_id primary key; user_id, revoked_at, expires_at | Server-only; bound retention to remaining credential validity |
| achievement_definitions | achievement_id; rule_version, criteria, label | Optional; authored server catalog |
| user_achievements | user_id + achievement_id; qualifying_session_id, awarded_at | Optional; unique award, server-only writes |

`canonical_state` is versioned, validated JSON containing explicit resource units and step state. It is not an arbitrary client snapshot. Foreign keys, score/resource checks and unique command/result constraints must be enforced in migrations later.

```mermaid
erDiagram
    AUTH_USER ||--|| PROFILE : has
    AUTH_USER ||--o{ MISSION_SESSION : owns
    MISSION_VERSION ||--o{ MISSION_SESSION : configures
    MISSION_SESSION ||--o{ MISSION_COMMAND : accepts
    MISSION_SESSION ||--o| MISSION_RESULT : completes
    MISSION_RESULT ||--o{ SKILL_EVIDENCE : supports
    AUTH_USER ||--o{ USER_SKILL : accumulates
    AUTH_USER ||--o{ USER_ACHIEVEMENT : earns
```

Completion is one database transaction: validate state → create result → create evidence → update skill cache → optionally award achievement → mark completed. Replayed completion returns the same report. Do not commit progress and assessment separately. Profile creation must also be idempotent after confirmed signin.

## FastAPI endpoint proposal

All `/v1` user endpoints require a validated Supabase access JWT. Supabase handles signup/signin/reset directly; FastAPI must not introduce its own password database or signin endpoint. Use documented request/response schemas and publish OpenAPI during implementation.

| Method and path | Purpose / request |
|---|---|
| GET /health | Public minimal availability; no configuration/secrets |
| GET /v1/me | Current profile or onboarding state |
| PATCH /v1/me | Whitelisted alias/preferences only |
| GET /v1/missions | Published missions and eligibility |
| GET /v1/recommendations | Deterministic next mission/difficulty and reason |
| POST /v1/mission-sessions | Mission ID/version request; server selects allowed difficulty; idempotency key |
| GET /v1/mission-sessions/{id} | Owner's canonical state, revision and resume status |
| POST /v1/mission-sessions/{id}/commands | commandId, expectedRevision, kind, validated payload |
| POST /v1/mission-sessions/{id}/pause | Idempotent command ID/revision; persist pause |
| POST /v1/mission-sessions/{id}/resume | Recheck Auth/ownership; canonical snapshot |
| POST /v1/mission-sessions/{id}/complete | Command ID/revision; server validates required evidence; returns report ID |
| POST /v1/mission-sessions/{id}/abandon | Explicit confirmed abandonment; no completion credit |
| GET /v1/reports/{sessionId} | Saved score, reasoning feedback, resource outcomes and next steps |
| GET /v1/progress | Dashboard summary and resumable mission |
| GET /v1/skills | Skill evidence count and provisional proficiency |
| GET /v1/history | Paginated mission reports |
| POST /v1/auth-session/revoke | Revoke the caller's current Auth session for training API access |
| GET /v1/achievements | Optional badges; add only after core completion |

Return 401 for invalid/expired authorization, 404 for nonexistent or other-user resources, 409 with canonical revision for state conflict, 422 for invalid decisions, and 429 for rate limits. A duplicate command ID with identical content returns the saved response; reuse with altered content fails. Put strict payload bounds and per-user/session rate limits on mutations. Reject invalid enums and unknown mission steps. Persist authoritative UTC times; never trust client clocks for energy or assessment.

Unity sends semantic actions such as inspect_panel, select_load_priority and classify_observation through the broker. It does not send a desired score or arbitrary resource total. Camera and movement run locally; no per-frame API traffic. The backend cannot prove every physical action in an untrusted client; this is an educational trainer, not certified anti-cheat or astronaut qualification.

## Science and resource simulation

Preserve the approved observe → predict → act → compare → explain sequence from the existing design documents. Core route remains Habitat → Power → Solar → Power → Science/Delta → advisory → Habitat/debrief. Do not replace the approved world layout during platform integration.

For the first mission, use the approved educational energy model: 20 kWh capacity, 12 kWh initial reserve, 4 kW clear generation, 1 kW dusty generation, 2 kW essential load, 1 kW research load. These are fictional training parameters, not measured Mars outpost values. Server computes energy change as net power × explicitly labeled forecast hours and clamps to capacity. A one-hour forecast is a lesson action; do not quietly treat one minute of play as an hour of real mission physics. Pause/background must not consume fictional life support while the user cannot act.

| STEM concept | MVP learning evidence | Expansion later |
|---|---|---|
| Energy generation | Predict kW versus kWh; choose essential loads; explain dust loss | Variable sun, batteries, multi-day planning |
| Engineering trade-offs | Balance reserve and research load; compare predicted/actual state | Maintenance and component failures |
| Scientific decision-making | Separate observed bedding from inferred history; record provenance | More samples, uncertainty and competing hypotheses |
| Oxygen | Suit readiness/reserve checklist; choose safe return over needless EVA | Validated oxygen/time model |
| Water recycling | Short authored recovery-loop decision in briefing/debrief | Consumables and mass-balance simulation |
| Radiation | Distinguish radiation risk from dust visibility; select shelter response | Validated dose model and exposure planning |
| Hydroponics | Explain water/light/power trade-off through one greenhouse learning card | Crop yield and closed-loop systems |

The last four are short assessed micro-decisions, not full gameplay systems. If time slips, clearly report which are informative cards and which have scored evidence. A visually visited greenhouse is not evidence of hydroponics mastery.

Use authored explanations linked to the existing design's scientific references. Record content/rubric/source versions in each report. Reviewer signs off each criterion; never label a fictional battery value, dust timer or difficulty multiplier as NASA data.

## Adaptive learning and assessment

MVP adaptation is deterministic, transparent and server-side. On the first attempt provide explicit hints and two decision choices. After demonstrated success, reduce scaffolding or add one load trade-off. After repeated mistakes, give a worked example and repeat an equivalent decision. Never increase radiation or lethal hazards merely to manufacture difficulty.

Proposed report rubric: energy/unit reasoning 35%, engineering decisions 25%, observation versus inference 25%, readiness/return reasoning 15%. Score criterion evidence, not walking speed; rubric weights are product assumptions requiring educational review. Record first-attempt outcome, hints and retries separately. UI reports strengths and one next learning action.

Keep a skill profile across energy, oxygen readiness, water systems, radiation awareness, hydroponics and scientific reasoning. Display “initial evidence” when there is only one short decision. Do not claim a validated proficiency threshold or certification. Optional levels can be provisional and evidence-count based, with minimum distinct sessions and later validation. Recommendations must cite the missing skill/evidence criterion, not an opaque invented AI score.
''',
'04_Delivery_And_Roadmap.md':'''# Delivery strategy, folders and seven-day roadmap

## Android packaging

After approval, add matching Android Build Support, SDK/NDK and OpenJDK through Unity Hub. Conventional Android Studio and SDK paths were not found during inspection; alternative installations were not ruled out. Neither a host Gradle build nor a Unity Android export has been tested.

The [Unity version matrix](https://docs.unity.com/en-us/engine/6000.6/manual/platform-specific/android/introducing/gradle-overview/version-compatibility) specifies Gradle 9.3.1 and Android Gradle Plugin 9.1.1 for 6000.6.3f1 and later. Its [dependency matrix](https://docs.unity.com/en-us/engine/6000.6/manual/platform-specific/android/getting-started/sdksetup/supported-dependency-versions) lists OpenJDK 17, NDK r27c (27.2.12479018), Build Tools 36.0.0, command-line tools 16 and Platform Tools 37.0.1 for the relevant release ranges. These are documented expectations, not locally verified installed Android components.

Use the Unity-exported toolchain as the baseline for the native host. [AGP 9.1.1 documentation](https://developer.android.com/build/releases/agp-9-1-0-release-notes) also specifies Gradle 9.3.1/JDK 17, but its default NDK differs from Unity's; explicitly pin Unity's NDK. AGP 9's Kotlin/build-plugin changes require checking the generated export and selected host libraries. Do not paste older Gradle 7/8 or duplicate Kotlin plugin instructions into this project. Choose an Android Studio version that supports this AGP, not merely the older minimum in the sample.

Keep the current min SDK 26 provisionally, subject to the chosen Auth/network libraries and device target. Resolve compile/target SDK from the generated export and current distribution requirements during the build gate. Use ARM64 and IL2CPP for the initial phone build, verify native dependencies and stripping, and add other ABIs only when required. Select broadly supported ETC2 texture compression initially; test actual devices before choosing a narrower compression option.

Bundle the initial scene, essential textures, native bridge and Unity runtime in the installed application. One application ID, launcher and signing identity. No external game installation. Unity's runtime is not planned as a Play dynamic feature; the ordinary library documentation lists that limitation. Asset delivery can be revisited for larger data after the hackathon.

Produce a signed APK for controlled demo installation and an AAB only if store distribution is required. Secure signing credentials outside source control. Do not promise Play review within the sprint. Measure APK download/installed size and cold-start time; Web build size is not a reliable Android size estimate.

## Proposed future folder structure — not created

```text
/Users/kapilash/Projects/
  MissionRedHorizon/                    existing project, preserved
    Assets/AstroGateway/Scenes/          approved environment remains
    Assets/AstroGateway/Platform/        later C# bridge adapter
    Assets/AstroGateway/Gameplay/        later mission/movement components
    Assets/Plugins/Android/              later native bridge plugin
    Docs/Design/                         approved documents remain
  astrogateway/                          existing React project, preserved
    src/                                existing prototype remains during MVP
  astrogateway-platform/                proposed new repository after approval
    android-host/
      app/                              native screens and Auth coordinator
      bridge/                           versioned native IPC protocol
    backend/
      app/                              API, simulation, assessment, ownership
      migrations/                       reviewed schema and security policies
      tests/                            authorization/state/idempotency tests
    contracts/                          OpenAPI, JSON DTO schemas, examples
    content/                            versioned mission rubrics and sources
    tooling/                            reproducible export/assembly procedures
    generated/unity-export/             disposable Unity export, ignored
    docs/                               architecture, deployment, acceptance
```

Generated Unity exports are build products, not the source of manual fixes. Keep required integration settings/plugins in reviewed source and reproduce them on export. Establish Git backups for both existing projects before implementation; inspection found neither root to be a Git repository. Do not move or merge the existing projects during the sprint.

## Platform migration

| Platform | Host and game delivery | Shared versus platform-specific work |
|---|---|---|
| Android MVP | Native host + bundled Unity Android library in one APK/AAB | Auth/IPC/storage/lifecycle and touch input are Android-specific |
| Responsive web next | Existing React host; separate Unity Web export inside its mission route | Reuse API/content/rubrics/design tokens; web Auth and JS bridge need implementation |
| Windows future | Evaluate supported Windows embedding or a coordinated standalone Unity player | Native packaging, lifecycle and credential broker need a separate feasibility test |
| macOS future | Prefer a single distributed product containing host and standalone Unity player until embedding support is proven | Do not assume Android or Windows UaaL integration supports macOS |

A Unity Android binary cannot run in the browser. Web requires a Web target export, compatible shaders/input, correct compression/MIME headers and a real browser smoke test. The React host should keep Auth and call FastAPI; relay allowed game requests through a versioned JavaScript bridge. Prefer same-origin hosting. If an iframe is used, validate target origin, sender origin/window and message schemas; never broadcast credentials. Browser session persistence and XSS defenses need separate design review. Responsive dashboard support does not establish acceptable Unity mobile-browser performance.

Reuse contracts and server business logic, not the existing Three.js gameplay as a second authoritative simulator. A later decision can preserve that prototype as a legacy demo without replacing the Unity world. Desktop should preserve the same account and API; a fresh device session remains necessary. Native macOS library compatibility is not verified by this proposal.

## Scope

| Must-have MVP | Optional after core passes | Post-hackathon |
|---|---|---|
| Signup/signin/signout/reset/confirmation | Simple earned badges | Full achievement catalog |
| Dashboard/profile/history/skill evidence | Rich avatar/profile customization | Social/cohort dashboards |
| One bundled Mars mission with touch movement | Second difficulty variant | Multiple missions and validated assessment |
| Energy/science/engineering decisions and short safety/system learning cards | Additional greenhouse/rover interaction | Full oxygen/water/hydroponics/radiation simulations |
| Host broker, server ownership, secure persistence | Read-only web dashboard | Full Web game integration and desktop clients |
| Canonical saved report and deterministic recommendations | Extra ambient audio/art polish | Offline reconciliation, advanced analytics or adaptive AI |
| Low graphics, readable text, basic audio settings | Expanded accessibility customization | Comprehensive accessibility/device coverage |

The 250 × 250 m approved world exists, but gameplay and account systems still need implementation. Limit required interaction to the core route. Add no new environment art pass during platform integration.

## Seven-day plan and acceptance gates

Assumption: two contributors, approximately 80–100 combined development hours plus part-time QA/scientific review. Android/backend and Unity/gameplay work can progress in parallel after contracts stabilize. This is a risky vertical-slice plan, not a promise of production readiness. If seven days include setup delays or only one developer is available, narrow the mission and UI explicitly.

| Day | Work | Acceptance gate |
|---|---|---|
| 1 | Matching Android tools; minimal native host; Unity export/import; bridge echo; on-device return/relaunch | One installed APK launches Unity full-screen, exchanges a non-secret request/response and returns without killing host. Ten repeat cycles; capture lifecycle logs and toolchain versions |
| 2 | Supabase Auth flows and encrypted storage; FastAPI JWT validation; private schema/owner policies | Signup/confirmation/signin/reset/signout exercised; two users cannot read or mutate each other's sessions; cold restore works |
| 3 | Real broker/session create-resume; touch movement/camera on approved scene; pause/Back behavior | Host-authenticated command round trip; disconnect/reconnect and canonical revision recovery; playable core route on phone |
| 4 | Power/solar decision loop; deterministic energy simulation; checkpoint/idempotency | Invalid transitions rejected; duplicate commands do not repeat resource effects; kW/kWh explanation and forecast labels reviewed |
| 5 | Delta evidence decision, short system/safety prompts, return/debrief, profile recommendations | Completion transaction saves exactly one report and skill contribution; report visible after dashboard return and signin restore |
| 6 | Physical-device profiling and lifecycle/security regression; low quality; content QA | Background/relaunch/process death and token refresh tested; no credential leaks; sustained target frame rate with measured RAM/thermal behavior |
| 7 | Feature freeze; fix critical defects; signed demo package, rehearsed journey and release notes | Complete account → mission → saved debrief → dashboard journey on target phone; independent tester can reproduce; known limits documented |

Day 1 integration is timeboxed to one working day after tools are available. If it fails, do not spend the whole sprint fighting RN/native adapters. Present the evidence and request approval to switch to a single Unity Android application with Unity-built dashboard screens and the same Supabase/FastAPI identity architecture. That contingency still ships one app, but changes the selected host architecture and scope; it is not automatic permission to substitute it. Missing Android tools block either Android route.

## Performance and release validation

Start with 30 fps on a named midrange ARM64 phone, low quality by default on constrained devices, and an optional standard profile after testing. Measure a ten-minute route including interactions: frame-time percentiles, stalls, resident memory/PSS, thermal throttling, startup and repeated return/relaunch. Targets are provisional, not validated capabilities. Existing M4 Pro/Web performance does not demonstrate phone performance.

Use baked/static lighting where appropriate, bounded shadow distance, existing scene LODs, simple collision and modest effects. Any changes to project/render settings occur only after approval and documented baseline capture. Verify touch UI safe areas, landscape game transition and readable text on a small phone.

Security/state checks: altered user/session IDs, expired JWT, revoked in-app session, concurrent refresh, malformed bridge messages, duplicate completion, network loss during report commit, account switch, release-build stripping and unhandled callbacks. Reports must survive reinstall/signin through server data; local secrets must not survive signout or leak through backup.

First milestone is **a device-proven host/Unity packaging and lifecycle spike**, not a full dashboard or additional scenery. Its review artifact is the APK, exact toolchain, ten-cycle log and a short screen recording; none has been created in this planning stage.
''',
'05_Evidence_And_Decisions.md':'''# Inspection evidence, references and unresolved decisions

## Inspected state — 8 October 2026

| Area | Finding | Implication |
|---|---|---|
| Unity root | `/Users/kapilash/Projects/MissionRedHorizon` | Preserve project and approved layout |
| Editor/renderer | ProjectVersion 6000.6.4f1; URP manifest 17.6.0 | Exact Android export still needs testing |
| Scenes | MarsOutpost_Greybox and MarsOutpost_Environment_v01 present | Platform stage reuses approved environment |
| Runtime code | Located builder C# scripts are Editor-only; no mission/controller/Auth bridge found in inspected source | Budget for actual gameplay implementation |
| Build modules | Selected Editor contains MacStandaloneSupport and WebGLSupport; no Android PlaybackEngine found | Android Build Support is the first dependency |
| Android tooling | `/Applications/Android Studio.app` and `~/Library/Android/sdk` not found | Check alternate locations before installation; absence everywhere was not established |
| Unity settings | DefaultCompany, MissionRedHorizon product; min Android SDK 26 | App ID, branding and target SDK must be settled later; no settings changed |
| React root | `/Users/kapilash/Projects/astrogateway` | Preserve all current source |
| React implementation | React/Vite/Three.js prototype; keyboard exploration, placeholder outpost and hardcoded resource HUD | No implemented Auth/dashboard/backend integration found; not a ready RN host |
| Dependencies | React 19.2.8, Three 0.186.1, Vite dependency ^8.3.0 in package manifest | Existing environment not modified or updated |
| Version control | Neither project root identified as a Git repository | Create backups/version control after approval |
| Cloud/build status | No supplied Supabase project inspected; no backend, APK or integration created | Architecture is proposed, not deployed/tested |

Inspection used source/configuration reads, not Unity or React modifications. Sensitive environment files were not opened. The preservation baseline covers 334 source/configuration/design files; generated caches and dependencies are outside its scope. See `Preservation_Check.json` for the final hash comparison.

## Official reference ledger

References support the specific facts listed; architecture choices and budgets remain proposals. Accessed 8 October 2026. Some Unity manual pages were available through indexed official content when direct opening timed out; no local integration test substitutes for documentation.

| Organization / reference | Supports / limitation |
|---|---|
| [Unity 6000.6 Android UaaL](https://docs.unity.com/en-us/engine/6000.6/manual/platform-specific/android/developing/unityasa-library) | Native library embedding, ordinary full-screen/one-runtime limits, unload versus quit and dynamic feature limitation. Direct retrieval timed out; official indexed content was available |
| [Unity official Android example](https://github.com/Unity-Technologies/uaal-example/blob/master/docs/android.md) | Activity/GameActivity examples and separate Unity process. Does not prove this exact project's compatibility; verify license before copying source |
| [Unity Gradle compatibility](https://docs.unity.com/en-us/engine/6000.6/manual/platform-specific/android/introducing/gradle-overview/version-compatibility) | Version-specific Gradle/AGP expectations |
| [Unity Android dependencies](https://docs.unity.com/en-us/engine/6000.6/manual/platform-specific/android/getting-started/sdksetup/supported-dependency-versions) | JDK/NDK/SDK dependency versions; indexed official content used |
| [Unity Android external tools](https://docs.unity.com/en-us/engine/6000.6/manual/platform-specific/android/getting-started/sdksetup/external-tools-reference) | Bundled dependency configuration |
| [Unity Android build settings](https://docs.unity.com/en-us/engine/6000.6/manual/platform-specific/android/building-and-delivering/build-settings) | Gradle export, APK/AAB and texture compression options |
| [Android AGP 9.1.1](https://developer.android.com/build/releases/agp-9-1-0-release-notes) | Toolchain matrix; default NDK differs from Unity's |
| [Android bound services](https://developer.android.com/develop/background-work/services/bound-services) / [AIDL](https://developer.android.com/develop/background-work/services/aidl) | Cross-process communication options and thread/lifecycle considerations |
| [Android Keystore](https://developer.android.com/privacy-and-security/keystore) | Protected cryptographic key storage; not raw token storage. Official indexed content available after direct timeout |
| [React Native native-app integration](https://reactnative.dev/docs/integration-with-existing-apps.html) | Native setup is required; no Unity compatibility guarantee |
| [Supabase Kotlin introduction](https://supabase.com/docs/reference/kotlin/introduction) | Kotlin client is community-maintained |
| [Supabase JWTs](https://supabase.com/docs/guides/auth/jwts) / [signing keys](https://supabase.com/docs/guides/auth/signing-keys) | JWT verification, asymmetric JWKS, legacy verification and key rotation |
| [Supabase sessions](https://supabase.com/docs/guides/auth/sessions) / [signout](https://supabase.com/docs/guides/auth/signout) | Refresh/session behavior and remaining access-token validity |
| [Supabase API keys](https://supabase.com/docs/guides/getting-started/api-keys) | Publishable versus server-secret credentials |
| [Supabase RLS](https://supabase.com/docs/guides/database/postgres/row-level-security) / [secure data](https://supabase.com/docs/guides/database/secure-data) | Database access boundaries and privileged bypass risk |
| [Supabase mobile deep links](https://supabase.com/docs/guides/auth/native-mobile-deep-linking) | Native Auth callback configuration; exact Android callback must be tested |
| [FastAPI security documentation](https://fastapi.tiangolo.com/tutorial/security/oauth2-jwt/) | Framework JWT/security patterns; do not copy a separate password/identity server into this design |

## Scientific provenance

Existing `Docs/Design/05_Gameplay_Environment_Integration.md` and the approved design/reference documents remain the content source of truth. The educational route, energy values and observe/predict/act/compare/explain structure are taken from that local blueprint. No new scientific constants were introduced as verified data here.

For scientific review, retain the approved primary references: [NASA Mars facts](https://science.nasa.gov/mars/facts/), [NASA Jezero delta panorama](https://science.nasa.gov/photojournal/detailed-panorama-of-mars-jezero-crater-delta/), [NASA Jezero landing site](https://science.nasa.gov/resource/jezero-crater-mars-2020s-landing-site/) and [NASA Mars fact sheet](https://nssdc.gsfc.nasa.gov/planetary/factsheet/marsfact.html). These are retained references from the design brief, not evidence that this architecture stage revalidated every scientific claim or asset license.

The “Junior Astronaut Mission Trainer” challenge description is user-provided. The exact official 2026 challenge brief/rubric was not independently verified in the searches performed. Obtain its official URL before claiming complete submission alignment. NASA imagery/references do not constitute endorsement of this product; asset usage still follows the approved attribution review.

## Risks and decisions before implementation

| Risk / open decision | Proposed handling |
|---|---|
| Unity 6000.6.4f1 + native host + AGP 9.1.1 not proven | Day 1 device spike before building account UI |
| Android modules and toolchain unavailable | Approve matching installation, verify locations/disk space and record versions |
| Team capacity / remaining days | Confirm two builders and review capacity; otherwise reduce scope |
| Target phone unknown | Name primary and lower-end test phones; use physical devices, not M4 performance assumptions |
| Auth SDK compatibility and secret persistence | Pin only after compilation and session/secure-storage tests; adapter keeps replacement possible |
| Cloud/email callbacks/HTTPS not configured | Confirm Supabase project owner, hosting, reset/confirmation delivery and redirect allowlist |
| Children-oriented product | Minimal data, controlled demo accounts; public release requires a separate privacy/consent review |
| Lifecycle and memory pressure | Separate Unity process; checkpoint/reconnect tests; measure actual memory release |
| Large APK/slow cold start | Bundle one mission, audit assets, measure download/install/startup before promising sizes |
| Scientific breadth over seven days | One substantive energy/science mission; label short system prompts and provisional assessment honestly |
| Package ID/signing and brand | Confirm owned reverse-domain identifier and signing owner; keep credentials outside source |
| Provider-level revocation outside app | Document expiry window; validate an appropriate provider-session check before promising immediate global revocation |
| Paid/unapproved assets or third-party wrapper | None planned; check licenses before any later import |
| Challenge alignment | Obtain official brief/rubric link; no NASA endorsement or qualification claim |

Approval requested is for the proposed architecture and first implementation milestone. It does not mean the integrated app is already supported on every Android device or that every planned feature can be guaranteed in seven days. Stop at planning until approval arrives.
'''
}
for name,body in files.items():
    (p/name).write_text(body)
print('Created',len(files),'planning documents')
print('Total words:',sum(len(s.split()) for s in files.values()))
