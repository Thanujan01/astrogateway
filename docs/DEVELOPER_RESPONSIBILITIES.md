# Proposed developer responsibilities and access map

[README](../README.md) · [Architecture](ARCHITECTURE.md) · [Workflow](DEVELOPMENT_WORKFLOW.md)

This is a recommendation for future ownership discussions. It assigns no real collaborator, grants no access, creates no CODEOWNERS file and asserts no existing enforcement policy.

## Module recommendations

| Development role | Suggested primary folders | Review responsibilities / collaboration |
|---|---|---|
| Unity / Gameplay Developer | [MissionRedHorizon](../01-Game-Development/MissionRedHorizon), especially FirstMission, Integration, MissionIntegration, MissionRules | MissionDirector/control/HUD/navigation behavior, scenes and prefab references. Pair with backend on request/record contracts and artist on assets/GUIDs. |
| Backend Developer | [astrogateway-api](../02-Software-Development/Backend/astrogateway-api), reviewed candidate SQL | FastAPI routes, identity ownership, typed RPCs, revocation, record validation, tests. Involve Android/web for auth and profile changes. |
| Web Developer | [astrogateway-web](../02-Software-Development/Web-Dashboards/astrogateway-web) | AccountApp/AccountContext, PKCE callbacks, profile client, accessibility, Vite proxy and locked npm dependencies. |
| Android Developer | [android-unity-host-poc](../02-Software-Development/Integrations/android-unity-host-poc), [FirstMissionAndroid](../01-Game-Development/Experiments/MissionRedHorizon_Step1Work/FirstMissionAndroid) | Native lifecycle, auth/session storage, private Unity Activity, descriptor/return protocol, Gradle export and real-device checks. Coordinate with Unity before regenerating exports. |
| AI / Mission Systems Developer | [Stage07A/07B](../02-Software-Development/Integrations/experimental), [backend rules](../02-Software-Development/Backend/astrogateway-api/rules), [Unity mission systems](../01-Game-Development/MissionRedHorizon/Assets/AstroGateway/FirstMission/Scripts/Missions) | Keep synthetic/offline research separate; review metrics, provenance, consent/witness limits, mission identifiers, rules and pending assessment integration. |
| Technical Artist | [SourceArt](../01-Game-Development/SourceArt), Unity environment/visual/showroom modules | Models, materials, textures, lighting, LODs, licensing/provenance, prefab import and matching `.meta` files. Avoid editing another Unity project copy by accident. |
| DevOps / Repository Maintainer | Root Git/LFS/scan configuration, [docs](../docs), dependency/configuration review | Private repo access decisions, release/build reproducibility, documentation/tree maintenance and proposed CI. Coordinate any remote SQL or deployment approval separately. |

Historical `docs/history` and Step1Work reports are evidence. Change current guides for clarification; preserve historical claims with their original context rather than rewriting old results as new validation.

## Repository permissions versus review routing

GitHub repository permissions govern access at repository scope. A developer who can write to this repository is not restricted to a particular source folder by this table. CODEOWNERS identifies reviewers for matching paths; it is not a per-directory access-control list. Enforcing required reviews is a separate branch/rules configuration and can depend on repository/account features. No such enforcement was inspected or changed here. See [GitHub CODEOWNERS](https://docs.github.com/en/repositories/managing-your-repositorys-settings-and-features/customizing-your-repository/about-code-owners) and [personal repository permission levels](https://docs.github.com/en/repositories/managing-your-repositorys-settings-and-features/repository-access-and-collaboration/permission-levels-for-a-personal-account-repository).

Do not fill an ownership file with invented handles. Once the owner chooses actual eligible reviewers, discuss routing for Unity assets, API/auth, web, Android, experimental AI, root configuration and the ownership file itself. True access isolation, if required, needs a deliberate repository/access design rather than a CODEOWNERS pattern.

## Collaboration workflow

1. Branch from current main and identify the exact Unity project/component being changed.
2. Agree cross-module contracts before editing authentication, mission/session identifiers, record schema, JSON catalogs or native launch messages.
3. Keep source assets with `.meta` files and use LFS attributes. Avoid concurrent edits to the same Unity scene/prefab.
4. Run appropriate local tests/builds. Label unavailable provider/device checks NOT TESTED; do not inherit historical PASS claims.
5. Open a focused pull request describing behavior, affected modules, configuration/migration implications and actual checks.
6. Request the relevant role reviewers manually until real review routing is configured. Owner-approved repository policy governs merging.
7. Update the tree/setup/architecture documents when paths, dependencies or boundaries change. Preserve the fixed migration manifest as import evidence.

## Current handoff priorities

- Verify current-clone Unity/Android builds and explicit scene selection before release claims.
- Resolve original-layout test/tool paths through a separately scoped source change.
- Verify isolated provider configuration and approved RPC/RLS deployment before live account/mission integration.
- Keep proposed scoring and scientific-analysis seams visibly separate from authoritative assessment.
- Establish actual ownership, CI and review rules through a separate repository administration decision.
