# Inspection evidence, references and unresolved decisions

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
