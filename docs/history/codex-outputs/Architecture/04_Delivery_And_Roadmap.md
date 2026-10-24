# Delivery strategy, folders and seven-day roadmap

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
