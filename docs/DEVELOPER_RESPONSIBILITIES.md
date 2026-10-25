# Team responsibilities and access map

[README](../README.md) · [Architecture](ARCHITECTURE.md) · [Workflow](DEVELOPMENT_WORKFLOW.md)

The named roles below were supplied by Kapilash. The detailed work allocation and delivery sequence are planning recommendations; they do not imply that every team member has accepted a schedule. GitHub usernames and access assignments have not been provided or changed.

## Named team roles

| Team member | Role supplied by Kapilash | Main responsibility |
|---|---|---|
| **Kapilash** | Lead game developer | Build the Unity game, gameplay and mission systems; integrate accepted art and software changes into a playable experience. |
| **Bawantha** | Software developer | Develop software services and integrations; proposed work areas are backend, web, database contracts and Android integration. |
| **Layanthini** | 3D objects / character developer; planning and research contributor | Create 3D objects and characters; also contribute project ideas, data gathering, PPTs and documents with Shinojan. |
| **Shinojan** | Project planning, ideas, data gathering, PPTs and documents | Work with Layanthini on the project brief, references, requirements, presentation and documentation. |
| **Jkutralan** | Innovation contributor | Suggest innovative ideas and explain their user benefit; help the team compare and refine concepts before implementation. |

Layanthini and Shinojan share planning, idea development, research/data gathering, PPTs and documents. Layanthini also has a separate 3D production role. To balance that workload, the [structured team plan](TEAM_PLAN.md) proposes Shinojan coordinating the document/presentation draft while Layanthini contributes research and visual material. This coordination split is a proposal, not an additional role supplied by Kapilash.

## Working agreements

- Kapilash and Bawantha agree game/software contracts before integration work.
- Kapilash gives Layanthini a small asset brief before modelling; Layanthini returns a preview and a Unity-ready asset package with agreed scale, naming and materials.
- Layanthini and Shinojan keep sources, project decisions and presentation claims traceable to evidence.
- Jkutralan adds ideas to the shared shortlist; planning and technical feasibility review happen before a feature enters implementation.
- Each contributor records their work and checks; relevant teammates review cross-module changes. No dedicated QA, DevOps or deployed AI owner has been assigned by this plan.

See [TEAM_PLAN.md](TEAM_PLAN.md) for workstreams, handoffs, milestone exit criteria and a proposed weekly rhythm.

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

## Sentinel database validation ownership

As part of the software workstream, **Bawantha** is the proposed primary contributor for Sentinel validation, with **Kapilash** reviewing game payload contracts. **Layanthini and Shinojan** record evidence and limitations in the project documents/PPT. The [validation report](SENTINEL_DATABASE_VALIDATION.md) records a disposable PostgreSQL 17.11 rehearsal: **25 checks passed, 6 failed**. Resolve and re-test the payload/error-code gaps before sign-off. This task added validation tooling and evidence; it did not modify existing application SQL or access a hosted database.

## Current handoff priorities

- Verify current-clone Unity/Android builds and explicit scene selection before release claims.
- Resolve original-layout test/tool paths through a separately scoped source change.
- Verify isolated provider configuration and approved RPC/RLS deployment before live account/mission integration.
- Keep proposed scoring and scientific-analysis seams visibly separate from authoritative assessment.
- Establish actual ownership, CI and review rules through a separate repository administration decision.
