# AstroGateway structured team and delivery plan

[README](../README.md) · [Team responsibilities](DEVELOPER_RESPONSIBILITIES.md) · [Architecture](ARCHITECTURE.md) · [Setup](SETUP_GUIDE.md) · [Development workflow](DEVELOPMENT_WORKFLOW.md)

## Purpose and planning basis

Roles are based on Kapilash's team description. The workstreams, coordination split, milestones and meeting rhythm below are **proposed**, with no fixed dates or assumed availability. Start with one working Mars mission and its supporting software, then add approved improvements. A source snapshot, historical test report or presentation slide does not establish a current working build.

## Team structure

| Team member | Role supplied by Kapilash | Main responsibility |
|---|---|---|
| **Kapilash** | Lead game developer | Build the Unity game, gameplay and mission systems; integrate accepted art and software changes into a playable experience. |
| **Bawantha** | Software developer | Develop software services and integrations; proposed work areas are backend, web, database contracts and Android integration. |
| **Layanthini** | 3D objects / character developer; planning and research contributor | Create 3D objects and characters; also contribute project ideas, data gathering, PPTs and documents with Shinojan. |
| **Shinojan** | Project planning, ideas, data gathering, PPTs and documents | Work with Layanthini on the project brief, references, requirements, presentation and documentation. |
| **Jkutralan** | Innovation contributor | Suggest innovative ideas and explain their user benefit; help the team compare and refine concepts before implementation. |

**Shared planning responsibility:** Layanthini and Shinojan both contribute ideas, project planning, research/data gathering, PPTs and documents. Layanthini additionally produces 3D objects and characters. Proposed workload balance: Shinojan coordinates the written brief and presentation draft; Layanthini contributes references, design decisions and visuals alongside a limited asset batch. Confirm capacity before assigning both a large asset delivery and a presentation deadline.

Bawantha's backend/web/database/Android allocation below is a proposed interpretation of the software-developer role, not a claim that every specialty is staffed or already implemented. Jkutralan's innovation role is not an assignment as an AI engineer. GitHub identities, permissions and administrative ownership remain separate decisions.

## Workstreams and handoffs

| Workstream | Main contributor(s) | First useful output | Handoff / review |
|---|---|---|---|
| Game and mission development | Kapilash | One selected FirstMission scene with a clear playable objective, controls, mission flow and HUD | Layanthini provides assets; Bawantha reviews software-facing contracts; planning contributors check the agreed objective. |
| Software development | Bawantha | Reproducible local backend/web setup and a small documented integration contract | Agree identifiers, payloads, errors and offline behavior with Kapilash before connecting the game. |
| 3D objects and characters | Layanthini | A small agreed asset batch with a reference sheet, preview and import-ready files | Kapilash checks scale, materials, prefab references, controls/animation needs and performance in the target scene. |
| Project planning and research | Layanthini + Shinojan | A brief describing the audience, mission objective, feature shortlist and reference sources | Kapilash/Bawantha assess implementation effort and technical constraints; Jkutralan contributes alternatives. |
| PPTs and documentation | Layanthini + Shinojan | A draft presentation and current project notes grounded in the latest demonstration | Developers check technical claims; Layanthini supplies art previews; Shinojan coordinates the proposed draft/revision workflow. |
| Innovative ideas | Jkutralan, with team feedback | A concise idea card: problem, proposed feature, benefit, assumptions and simplest prototype | Planning contributors gather evidence; Kapilash/Bawantha assess feasibility; only an accepted idea enters a milestone. |
| Verification and demonstration | Each contributor for their work; Kapilash/Bawantha for their integration areas | Actual check results, a reproducible demo and a clear list of NOT TESTED items | Planning/presentation material reflects those results; no dedicated QA role is implied. |

## Repository working areas

| Contributor / purpose | Actual repository locations |
|---|---|
| Kapilash — game | [Primary Unity project](../01-Game-Development/MissionRedHorizon), [FirstMission](../01-Game-Development/MissionRedHorizon/Assets/AstroGateway/FirstMission), [integration contracts](../01-Game-Development/MissionRedHorizon/Assets/AstroGateway/Integration) |
| Bawantha — proposed software areas | [Backend](../02-Software-Development/Backend/astrogateway-api), [web](../02-Software-Development/Web-Dashboards/astrogateway-web), [Android host POC](../02-Software-Development/Integrations/android-unity-host-poc), [SQL candidates](../02-Software-Development/Sentinel-Database) |
| Layanthini — art | [SourceArt](../01-Game-Development/SourceArt), [ARES-VI showroom assets](../01-Game-Development/MissionRedHorizon/Assets/AstroGateway/VehicleShowroom/ARES_VI), [FirstMission prefabs](../01-Game-Development/MissionRedHorizon/Assets/AstroGateway/FirstMission/Prefabs) |
| Layanthini + Shinojan — planning / research / communication | [Current developer guides](../docs), [game documentation](../01-Game-Development/MissionRedHorizon/Docs), [design references](reference-material); preserve old reports as historical evidence. |
| Jkutralan — innovation input | Add ideas to the shortlist below through the team workflow. [Stage07A/07B](../02-Software-Development/Integrations/experimental) are optional research references, not ready-to-use production AI. |

Presentation and document outputs should be produced from the agreed brief and evidence. Choose their filenames, next task assignments and delivery dates when the team confirms the milestone scope and availability.

## Mandatory software acceptance gate: Sentinel database validation

Proposed primary contributor: **Bawantha**, with **Kapilash** reviewing Unity payloads and integration behavior. Validation must cover schema constraints, ownership/RLS, data shape, idempotency/error contracts, migrations and recovery. The [measured local report](SENTINEL_DATABASE_VALIDATION.md) currently records **59 PASS / 0 FAIL**. The six observed failures are fixed locally; hosted integration and deployment sign-off remain pending. Layanthini and Shinojan should keep the actual results and NOT TESTED limits visible in the project documents and PPT.

The older Sentinel SQL alone does not satisfy the current backend. Use the report to agree the reviewed baseline before fixing or deploying anything. Gate 3 requires the applicable validation failures to be resolved and re-tested, in addition to demonstrating the selected software flow.

## Proposed delivery sequence

These are milestone gates, **not promised calendar weeks**. A gate is complete only when its outputs have been reviewed and its checks recorded.

| Gate | Focus and contributor tasks | Exit criteria |
|---|---|---|
| 1 — Agree the target and establish a baseline | Kapilash selects the mission scene and checks current Unity setup. Bawantha checks local software setup. Layanthini/Shinojan write a short scope brief and reference list. Jkutralan contributes a small idea shortlist. | One agreed target demo; actual setup results recorded; known blockers and untested areas listed; a small prioritized task list exists. |
| 2 — Make one coherent playable mission | Kapilash implements/refines the mission path. Layanthini produces the agreed object/character batch. Bawantha defines supporting API/record contracts. Shinojan and Layanthini maintain requirements and visual references. | The selected scene demonstrates the agreed local objective; assets import correctly with their metadata; actual gameplay checks are recorded. |
| 3 — Connect the necessary software | Bawantha builds/verifies the selected software flow; Kapilash connects only agreed game adapters. Layanthini/Shinojan document behavior, limitations and test steps. | A repeatable integration demonstration exists for the selected flow; offline/error handling is shown; any live-service checks not performed are explicitly NOT TESTED. |
| 4 — Review and present | Each contributor checks their output. Kapilash/Bawantha reproduce the technical demo. Layanthini/Shinojan finish the PPT and supporting documents. Jkutralan gives improvement suggestions. | Presentation claims match the current build; sources and visuals are attributed; the demo steps, limitations and next tasks are documented. |
| 5 — Prototype one approved innovation | Jkutralan explains the selected idea; Layanthini/Shinojan research its value; Kapilash/Bawantha estimate and prototype the relevant parts; Layanthini adds art only if needed. | A small prototype answers a specific question. The team records adopt / revise / defer, rather than automatically adding the feature to the main game. |

## Suggested first task cards

These are proposed starting tasks, not claims that the work has already happened.

| Contributor(s) | Suggested next card | Evidence of completion |
|---|---|---|
| Kapilash | Open the primary project with the pinned Unity editor, select the Missions scene and define one end-to-end local objective. | Scene path, setup/play result and a short gameplay checklist. |
| Bawantha | Reproduce backend/web setup and identify the smallest useful software flow to demonstrate. | Commands/environment versions, actual test results and an integration contract draft. |
| Layanthini | Review existing rover/suit assets with Kapilash and select one small object or character improvement. | Asset brief, references, preview and agreed import/performance checks. |
| Layanthini + Shinojan | Write a one-page project brief and draft the PPT outline. | Target users, problem, mission objective, scope, source list and a slide outline linked to actual work. |
| Jkutralan | Suggest up to three innovative ideas, each with a user benefit and simplest test. | Three short idea cards with assumptions, dependencies and a proposed success measure. |

## Definition of a useful handoff

### Game / software

Agree the mission/session identifiers, request/response examples, offline behavior and expected errors before integration. Include reproduction steps and actual checks in the pull request. Review the existing distinction between local/provisional behavior and authoritative server results. Keep current scientific-analysis and experimental-AI limitations visible.

### 3D objects / characters

Start with a brief: intended scene, object purpose, visual references, scale, pivot/orientation, material/texture needs, animation/rig requirements where relevant, and an agreed performance budget. Do not invent a universal polygon budget before choosing the device and use case. Return editable source, agreed exports, preview images, attribution/provenance and Unity `.meta` files where applicable. Kapilash checks the result in the game before calling the asset integrated.

### Research, PPTs and documents

For each reference record its source/link, relevant date/version, what it supports, usage/attribution constraints and what remains uncertain. Separate scientific facts, simulated game behavior, proposed features and experimental findings. Do not describe simulated sensor readings as measured Mars observations or a prototype model as deployed AI.

Proposed PPT outline: problem and audience; project goal; the selected mission; verified architecture; current gameplay; 3D assets; demonstrated software flow; actual validation and limitations; innovation shortlist; next milestone and team responsibilities. Use screenshots from the current demonstrable build and label concepts clearly. Existing historical design documents are references, not proof of current implementation.

### Innovative ideas

Use this proposed card format:

| Field | Record |
|---|---|
| Idea / contributor | A short title and who suggested it |
| Problem and user benefit | Who it helps and what becomes easier/better |
| Evidence | Research/reference or feedback supporting the idea |
| Smallest prototype | The minimum experiment needed to test the assumption |
| Dependencies / effort | Game, software, art, data and device needs; estimate before scheduling |
| Success measure | An observable result, not a vague claim of innovation |
| Decision | Proposed, accepted for prototype, revise or defer |

**Initial shortlist: not yet supplied.** Jkutralan's specific ideas should be recorded when provided; none are invented or presented as approved features here.

## Proposed weekly rhythm

- **Planning checkpoint:** Kapilash, Bawantha, Layanthini and Shinojan agree a small deliverable per workstream; Jkutralan presents relevant new ideas. Adjust the size to actual availability.
- **Mid-cycle handoff:** review one game/software contract and one asset preview early, before large implementation effort.
- **Demo checkpoint:** show current work, record checks/blockers, update project notes and presentation evidence, then choose the next small task batch.

A lightweight task board can use **Ideas → Ready → In progress → Review → Verified/Done**, with **Blocked** recorded explicitly. Each task should name a main contributor, a reviewer, an expected output and a completion check. No issues or assignments have been created automatically.

## Current technical boundaries

The primary Unity integrations default offline; live provider/database configuration and end-to-end behavior remain unverified. Some tests/tools depend on the original machine layout. Scoring defaults are proposed, scientific analysis is not connected, and Stage07A/07B remain separate experiments. See [Architecture known limits](ARCHITECTURE.md#known-limits) and [Setup](SETUP_GUIDE.md) before planning a release or presentation claim.

This plan records team organization and proposed work only. It changes no application source, remote service configuration, repository permissions or team member's account access.

## Latest game integration evidence

The [Missions/native host update](GAME_INTEGRATION_UPDATE.md) records real scene selection, retry serialization fix, development APK build and two full graphical regression runs (292 PASS / 1 Editor Search FAIL / 1 UNTESTED). All six local missions completed in the automated sequence. Device/live identity/score integration, multiplayer/media and final release gates remain open; this does not mark every delivery gate complete.
