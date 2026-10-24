# AstroGateway — GitHub Collaboration Readiness Report

Date: 2026-10-20 · Scope: read-only audit (Stage 1) plus proposals (Stages 2–4).
Nothing was modified, committed, pushed, created or invited. The only write is this report file.
Git was read with `GIT_OPTIONAL_LOCKS=0` so not even the index was refreshed (another session is working in this repo).

**Status: STOPPED — awaiting owner approval (see §9).**

---

## 1. Executive summary

| Area | Finding | Risk |
|---|---|---|
| Unity repo | Healthy local repo, 12 commits, 3 branches, no uncommitted tracked changes | Low |
| Remote | **No remote configured.** Nothing has ever been pushed | — |
| History size | `.git` = 863 MiB, pack 774 MiB. **627 MiB is PNG screenshot evidence in `Docs/`**, 139 MiB textures in `Assets/` | **High** (clone cost for every collaborator, permanent once pushed) |
| Git LFS | Not installed on this Mac, no `.gitattributes`. All binaries are plain git blobs | High |
| GitHub hard limits | Largest blob 22.7 MiB; none ≥ 50 MiB (warn) or ≥ 100 MiB (block). A push would technically succeed | Low |
| Unity `.meta` | All tracked assets have `.meta`, no orphans, no duplicate GUIDs, Force Text serialization | Low |
| `.gitignore` | Correct core Unity ignores; missing build-output/APK/keystore/env patterns | Medium |
| Secrets in Unity repo | No real keys. Test-fixture JWTs (issuer `fixture.supabase.co`) and an `sb_secret_…` fixture string appear in `Docs/Step3|4/Backend_pytest_results.txt` | Medium (GitHub push protection may block; looks alarming in review) |
| Personal data | Commit author emails: a work email (`@nia.gov.lk`) and two local hostnames (`kapilash@<redacted-host>`, `…MacBook-Pro.local`) | **Medium** — publishes these to anyone with repo access |
| Backend (FastAPI) | `~/Projects/AstroGatewayAndroid/backend` — **not under git**. Has `.env` (0600, not read) and Supabase CA certs | High (no history, no backup other than tgz snapshot) |
| Android host (Kotlin) | `~/Projects/AstroGatewayAndroid` (14 GiB incl. emulator SDK/AVD, Unity export) — **not under git**. Contains `auth.local.properties`, `live.test-secrets.json`, `database/admin.local.env` (all real secrets, all 0600, all listed in its `.gitignore`) | High |
| Web app (React+Vite+Supabase) | `~/Projects/astrogateway` — **not under git**. `.env.local` (0600) correctly ignored | Medium |
| Backups | `MissionRedHorizon_Backups/FirstMission_PreChange_20261020`: `repo_all.bundle` contains all 3 branches at current heads; SHA-256 of all 3 archives re-verified **OK** today | Low |

**Bottom line:** the Unity repo is safe to *preserve*, but pushing it as-is would permanently put ~630 MiB of screenshots and two personal email addresses on GitHub for every collaborator to download. I recommend a clean, LFS-enabled import of the current tree into a new private repo, with the existing local history kept intact and archived (no rewrite of the existing repo).

---

## 2. Stage 1 — Audit detail

### 2.1 Git state — `/Users/kapilash/Projects/MissionRedHorizon`

- Unity 6000.6.4f1, URP 17.6.0, Input System 1.20.0.
- Current branch `visual-upgrade`, HEAD `5e0c223` "Batch 5: tempered-glass HUD and semantic colours".
- Branches: `main` 572d3dc (Step 4), `laptop-playtest` 1ca6203, `visual-upgrade` 5e0c223 (linear on top of the others: main → laptop-playtest → visual-upgrade).
- No remotes, no tags, no stash. Tracked modifications: none.
- **224 untracked files** (~56 MiB): `Assets/AstroGateway/VehicleShowroom/` (ARES-VI, 22 MiB, includes OBJ/PNG/anim + a 6.7 MiB mesh `.asset`), `Docs/VehicleShowroom/` (32 MiB evidence), `Docs/FirstMission/`, and ~2 MiB of Unity `.log` files under `Docs/VisualUpgrade/Batch*/`. These are other threads' in-progress work awaiting owner approval; do not commit them as part of the GitHub setup.
- Tracked: 1,614 files (Assets 1,065, Docs 515, ProjectSettings 27, Packages 2, Data 2).
- One path contains a non-ASCII em-dash: `Assets/AstroGateway/MarsEnvironment/Materials/Observation panels — opaque approximation.mat` (+ `.meta`). Works on macOS; can break Windows tooling/CI scripts. Recommend renaming in a normal PR later (Unity keeps the GUID via `.meta`).
- `core.ignorecase=true` (macOS default). Collaborators on Linux CI are case-sensitive; no case-only collisions found.

### 2.2 Size breakdown at HEAD

| Type | Size | Files |
|---|---|---|
| `.png` in `Docs/` (screenshots, evidence) | 626.9 MiB | ~330 |
| `.png` in `Assets/` (textures) | 139.1 MiB | ~130 |
| `.asset` (meshes, terrain, profiles) | 74.2 MiB | 195 |
| `.unity` scenes | 11.1 MiB | 13 |
| Code (`.cs`) | 0.7 MiB | 74 |

Largest textures: `sandy_gravel_nor_gl.png` 22.7 MiB, `sandy_gravel_Diffuse.png` 22.0 MiB, `rock_07/09_*` 13.5 MiB each. Many 2560×1440 Docs screenshots are 3.5–4 MiB and duplicated across Batch2–5.

### 2.3 `.gitignore` (current)

Covers `Library/ Temp/ Logs/ obj/ Build/ Builds/ UserSettings/ ProfilerCaptures/ *.csproj *.sln *.slnx .vscode/ .DS_Store`. Good core.
Missing: `/[Mm]emoryCaptures/`, `/[Rr]ecordings/`, `*.apk`, `*.aab`, `*.keystore`, `*.jks`, `*.unitypackage`, `crashlytics-build.properties`, `.env*`, `*.pem`/`*.p12`, `/Docs/**/*.log`, `sysinfo.txt`, `.idea/`, `*.pdb`, `*.mdb`.

### 2.4 Secrets

- Unity C# contains no keys or URLs with credentials (design passes the user JWT in from the native host).
- Pattern scan over **all commits** (JWT, `sk-`, `sb_secret_`, `service_role`, AWS, GitHub PAT, private keys, password literals): hits only in pytest output in `Docs/Step3/Backend_pytest_results.txt`, `Docs/Step4/Backend_pytest_results.txt` and the word `service_role` in `Docs/Step3/Step3_Report.md`. Decoded JWT claims show `iss=https://fixture.supabase.co/auth/v1`, i.e. synthetic test tokens. Not exploitable, but GitHub push protection or secret scanning may flag the `sb_secret_` fixture. Recommend excluding or redacting those two log files in the import.
- No secret-like filenames ever committed.
- Real secrets live outside the Unity repo, all mode 0600: `AstroGatewayAndroid/backend/.env`, `AstroGatewayAndroid/auth.local.properties`, `AstroGatewayAndroid/live.test-secrets.json`, `AstroGatewayAndroid/database/admin.local.env`, `astrogateway/.env.local`. Their contents were **not** read. Each is already listed in the folder's `.gitignore`, but none of these folders is a git repo yet, so that has never been tested.

### 2.5 Backups

`FirstMission_PreChange_20261020/SHA256SUMS.txt` re-verified: `repo_all.bundle`, `backend_snapshot.tgz`, `untracked_VehicleShowroom_Docs.tgz` all **OK**. Bundle heads equal the live branch heads (main 572d3dc, laptop-playtest 1ca6203, visual-upgrade 5e0c223). Bundles are on the same disk as the repo; an off-machine copy is recommended before any migration.

---

## 3. Stage 2 — Repository architecture

### Existing reality

| Component | Location | Stack | Git |
|---|---|---|---|
| Mars game | `~/Projects/MissionRedHorizon` | Unity 6 + C# | yes, local |
| Android native host | `~/Projects/AstroGatewayAndroid/app` (+ `unity-export`) | Kotlin, Gradle, Unity-as-a-Library | no |
| Backend | `~/Projects/AstroGatewayAndroid/backend` | FastAPI 0.143, httpx; Supabase JWT, PostgREST RPC | no |
| DB | `~/Projects/AstroGatewayAndroid/database/001_*.sql` (+ revised SQL in Codex workspace) | Supabase PostgreSQL | no |
| Web app | `~/Projects/astrogateway` | React 19 + Vite + three.js + supabase-js | no |
| Bawantha's mobile app | not started / framework not final | (React Native + TS proposed) | — |

### Comparison

| Criterion | A. Monorepo | B. Separate repos in one org |
|---|---|---|
| Clone size for app/backend devs | Every dev pulls Unity textures (LFS can be filtered, but needs discipline) | Bawantha clones a small app repo only |
| Unity binary handling | LFS rules must coexist with JS/Python tooling | LFS only in the Unity repo |
| CI | One workflow file with path filters; Unity CI (GameCI, license secrets) runs only on `unity/**` | Simple per-repo CI; no Unity license exposure to app CI |
| Access control | GitHub grants per repo, not per folder; CODEOWNERS only gates review | Least privilege per repo (e.g. Bawantha write on app, read on contracts) |
| Shared contracts (API schema, mission ids) | Trivially atomic in one PR | Needs a `contracts` repo or versioned OpenAPI published from backend |
| Cross-cutting changes | One PR | Coordinated PRs across repos |
| Fit with current layout | Requires reorganising `MissionRedHorizon` into a subfolder (moves every Unity path, risky) | Unity repo stays exactly where it is |

### Recommendation: **B — separate private repos in one GitHub organization**

```
github.com/<astrogateway-org>/
  astrogateway-unity       Unity 6 game (LFS)               owner: Kapilash
  astrogateway-mobile      Bawantha's app (stack TBD)       owner: Bawantha
  astrogateway-android-host Kotlin host + Unity export tools owner: Kapilash (optional; may fold into mobile once stack is chosen)
  astrogateway-backend     FastAPI + SQL migrations + tests owner: shared
  astrogateway-contracts   OpenAPI spec, JSON Schemas, mission ids, generated clients  owner: shared
```

Why: the Unity project does not need reorganising, Bawantha never downloads gigabytes of textures, Unity license secrets stay out of app CI, and the contracts repo is the single source of truth both apps consume. If the team strongly prefers one repo, put Unity at the root of its own repo anyway and make the monorepo hold only mobile + backend + contracts.

---

## 4. Stage 3 — Collaboration setup plan

### 4.1 Organization and repos
- Create a free GitHub organization (e.g. `astrogateway-team`). Both members as org members, base permission **No access**.
- Each repo **Private**. Teams: `game` (Kapilash), `mobile` (Bawantha), `backend` (both). Grant per repo: Unity → game: Maintain, mobile: Read; Mobile → mobile: Maintain, game: Read; Backend/Contracts → both: Write.
- Enable: Dependabot alerts, secret scanning + push protection (free on private repos only with GitHub Advanced Security; on Free/Team plans enable what is available and rely on the pre-commit hook below).

### 4.2 Collaborator invitation (after approval only)
1. Owner creates org → Settings → People → Invite member → Bawantha's GitHub username (not email, to avoid exposing addresses).
2. Bawantha accepts, enables 2FA (make 2FA required at org level).
3. Add to team `mobile` / `backend`.

### 4.3 Branch protection (ruleset on `main` in every repo)
- Require pull request; 1 approval (with two people this means the *other* person reviews; for solo-area changes allow owner bypass but log it).
- Dismiss stale approvals on new commits; require conversation resolution.
- Require status checks (lint/test; Unity: compile + EditMode tests when CI exists).
- Block force pushes and deletions on `main` and `release/*`.
- Require linear history (squash merge only).

### 4.4 Branch / PR workflow
- `main` = always-buildable integration branch. Short-lived branches: `feat/<area>-<topic>`, `fix/<area>-<topic>`, `docs/…`, `chore/…`. Areas: `hud`, `rover`, `mission`, `showroom`, `api`, `auth`, `app`.
- Current local branches map as: `main` stays main; `laptop-playtest` and `visual-upgrade` become `feat/laptop-playtest`, `feat/visual-upgrade` (or merge visual-upgrade into main via one reviewed PR, since history is linear).
- PR template: what/why, screenshots for visual changes (attach to PR, **not** committed), test evidence, scenes touched, "production scene changed? yes/no".
- Squash merge, delete branch after merge.

### 4.5 Code review policy
- CODEOWNERS: `/Assets/AstroGateway/Scenes/ @kapilash`, `/ProjectSettings/ @kapilash`, `/Packages/ @kapilash`, backend `/rules/ @both`, contracts `* @both`.
- Unity-specific: never merge a PR that changes `ProjectSettings/` or a production `.unity` scene without the Unity owner; one person edits a given scene at a time (announce in the Issue, or use LFS file locking on `*.unity`/`*.prefab`).
- Reviewers check: `.meta` added for every new asset, no GUID changes, no `Library/` or logs, no secrets, contract version bumped when API changes.

### 4.6 Git LFS (Unity repo)
Install `brew install git-lfs && git lfs install` (needs approval). Proposed `.gitattributes`:
```
# Unity YAML: text, merge with UnityYAMLMerge
*.unity   text merge=unityyamlmerge eol=lf
*.prefab  text merge=unityyamlmerge eol=lf
*.asset   text merge=unityyamlmerge eol=lf
*.mat     text merge=unityyamlmerge eol=lf
*.anim    text merge=unityyamlmerge eol=lf
*.controller text merge=unityyamlmerge eol=lf
*.meta    text eol=lf
*.cs      text diff=csharp eol=lf
# Binaries → LFS
*.png filter=lfs diff=lfs merge=lfs -text
*.jpg filter=lfs diff=lfs merge=lfs -text
*.jpeg filter=lfs diff=lfs merge=lfs -text
*.tga filter=lfs diff=lfs merge=lfs -text
*.psd filter=lfs diff=lfs merge=lfs -text
*.exr filter=lfs diff=lfs merge=lfs -text
*.hdr filter=lfs diff=lfs merge=lfs -text
*.fbx filter=lfs diff=lfs merge=lfs -text
*.obj filter=lfs diff=lfs merge=lfs -text
*.glb filter=lfs diff=lfs merge=lfs -text
*.blend filter=lfs diff=lfs merge=lfs -text
*.wav filter=lfs diff=lfs merge=lfs -text
*.mp3 filter=lfs diff=lfs merge=lfs -text
*.ogg filter=lfs diff=lfs merge=lfs -text
*.mp4 filter=lfs diff=lfs merge=lfs -text
*.wasm filter=lfs diff=lfs merge=lfs -text
*.dll filter=lfs diff=lfs merge=lfs -text
*.so filter=lfs diff=lfs merge=lfs -text
*.a filter=lfs diff=lfs merge=lfs -text
# Large binary-serialized mesh/terrain assets
Assets/AstroGateway/VisualUpgrade/Meshes/*.asset filter=lfs diff=lfs merge=lfs -text
*.unity lockable
*.prefab lockable
```
GitHub Free LFS quota: 1 GiB storage / 1 GiB bandwidth per month for the org (data packs cost extra). Textures alone are ~140 MiB, so the quota fits **only if Docs screenshots stay out of git** (§4.8).

Note: Mesh `.asset` files are YAML in Force Text mode but can be multi-MiB; LFS them only if they are regenerated rather than hand-edited.

### 4.7 Unity-safe `.gitignore` additions
```
/[Mm]emoryCaptures/
/[Rr]ecordings/
/[Bb]uild*/
*.apk
*.aab
*.keystore
*.jks
*.unitypackage
*.pidb.meta
*.pdb
*.mdb
sysinfo.txt
crashlytics-build.properties
.idea/
.env
.env.*
!.env.example
*.pem
*.p12
Docs/**/*.log
```

### 4.8 Evidence screenshots policy
Keep `Docs/*.md` reports in git; move bulky PNG evidence to (a) PR attachments, (b) GitHub Release assets per milestone, or (c) LFS with a per-batch cap. Today's 627 MiB of Docs PNGs stay in the archived local history and the backup bundle.

### 4.9 Secrets management
- Never commit `.env`, `*.local.*`, `*secrets*.json`, keystores. Keep `.env.example` files with placeholder names only.
- GitHub Actions: repository/environment secrets (`SUPABASE_URL`, `SUPABASE_PUBLISHABLE_KEY`, `UNITY_LICENSE`, Android signing) — environment `staging` with required reviewer.
- Supabase service-role and DB admin credentials: **never** in any repo or CI used by the apps (matches the backend README's isolation rule).
- Local pre-commit hook (`gitleaks` or `detect-secrets`) in every repo; allowlist the `fixture.supabase.co` test tokens in the backend repo's tests.
- Rotate any key that has ever been pasted into chat, logs or screenshots before the team starts.

### 4.10 README and architecture docs
Each repo: `README.md` (purpose, setup, run, test), `CONTRIBUTING.md` (branching, PR rules), `CODEOWNERS`, `.github/pull_request_template.md`, issue templates (bug, feature, visual defect with ID like UI-xx/3D-xx). Contracts repo: `ARCHITECTURE.md` with the diagram in §5.

### 4.11 Issues and ownership
- One GitHub Project (board) for the org: columns Backlog → Ready → In progress → Review → Done.
- Labels: `area:unity`, `area:mobile`, `area:backend`, `area:contracts`, `type:bug|feature|visual|security`, `needs-decision`, `blocked`.
- Every PR links an Issue (`Closes #n`). Assignee = owner. Decisions currently blocked (mission id, sensor endpoint, orientation, model location — from the First Mission audit) become `needs-decision` Issues.

---

## 5. Stage 4 — Application integration contract

### 5.1 Topology
```
 Supabase Auth (only identity issuer)
      │  sign-in / PKCE / refresh (mobile app owns this)
      ▼
 Mobile app (RN+TS or Kotlin host) ──launch descriptor v1──► Unity (Unity-as-a-Library)
      │  user JWT (Bearer)                    ◄──ACK / RESULT──      │ (no keys; mission ids only,
      ▼                                                               │  or short-lived JWT passed in)
 FastAPI backend  ──original user JWT──►  Supabase PostgREST (mission_api schema, RLS)  ──► PostgreSQL
      ▲                                                               │
      └──────────── Unity HTTP client (optional, same Bearer) ────────┘
```
Rules: Supabase is the only issuer; FastAPI holds no service-role or DB credential; the backend is authoritative for mission results and any analysis; Unity never submits a trusted score or AI verdict.

### 5.2 Contract artifacts (in `astrogateway-contracts`)
- `openapi/astrogateway-api.v1.yaml` — generated from FastAPI (`/openapi.json`) and reviewed. Existing endpoints (verified from code in the First Mission audit, not live): `GET /health`, `GET /v1/me`, `POST /v1/me/display-name`, `POST /v1/mission-sessions`, `POST /v1/mission-sessions/{id}/commands`, `POST /v1/mission-sessions/{id}/complete`, `GET /v1/mission-sessions/{id}`, `GET /v1/reports/{id}`, `GET /v1/skills`, `GET /v1/recommendations`, `POST /v1/auth-session/revoke`.
- `schemas/launch-descriptor.v1.json` — native → Unity (`schemaVersion`, `launchId` UUID, `missionId`, `sessionId`, `locale`, `apiBaseUrl`; no tokens unless approved).
- `schemas/unity-result.v1.json` — Unity → native (`launchId`, `sessionId`, `status`, client-side summary marked untrusted).
- `schemas/mission-command.v1.json` — idempotent command (`eventId` UUID, `revision`, payload ≤ 1 KB, ≤ 500/session; 409 on revision conflict).
- `ids/missions.json` — canonical mission ids. **Open decision:** backend uses `mission-red-horizon-poc`, Unity `mission.red_horizon.slice`, Step 4 proposed `ag.mars.red-horizon.surface-ops`. Pick one before integration.
- Generated clients: TypeScript (`openapi-typescript`) for mobile/web; C# DTOs for Unity (hand-written or NSwag) checked by a CI test that round-trips sample JSON.

### 5.3 Consistency across platforms
- Identity: `sub` from Supabase JWT is the only user id; display name via `/v1/me`.
- Mission progress: server-side session with monotonic `revision`; clients send commands, never overwrite state; offline clients queue commands with stable `eventId` for idempotent retry.
- Versioning: semver on the contracts repo; breaking change = new `/v2` path or new schema version; both apps pin a contracts version.
- Environments: `local` (debug HTTP allowed), `staging` (separate Supabase project, HTTPS only), `production` (later). **The backend is not deployed today**: its README states real-service startup is blocked until isolation is set up, no hosted Supabase test project is approved, and SQL 005 has not been executed.

---

## 6. Exact safe migration steps (proposed, none executed)

Phase 0 — Safety (no network)
1. Copy `~/Projects/MissionRedHorizon_Backups/FirstMission_PreChange_20261020/` to an external drive or cloud backup; re-run `shasum -a 256 -c SHA256SUMS.txt` there.
2. Create a fresh bundle at migration time: `git bundle create ../MissionRedHorizon_Backups/pre-github_<date>.bundle --all` and `git bundle verify` it.
3. Wait until no other session/Unity instance is using the repo.

Phase 1 — Unity repo import (recommended path, existing history untouched)
4. `brew install git-lfs gitleaks` (approval).
5. In a **new folder** (not the live project): `git clone --no-hardlinks ~/Projects/MissionRedHorizon astrogateway-unity-import`, checkout the branch chosen as baseline (`visual-upgrade` 5e0c223 recommended, or `main`).
6. In the import copy: delete `.git`, run `git init -b main`, add `.gitattributes` (§4.6) and the extended `.gitignore` (§4.7), set author email to the GitHub noreply address (`git config user.email <id>+<user>@users.noreply.github.com`).
7. Exclude `Docs/**/*.png` evidence and `Docs/Step3|4/Backend_pytest_results.txt` (or redact fixture tokens); keep reports `.md`.
8. `gitleaks detect --no-git` on the copy; must be clean.
9. Open the import copy in Unity once to confirm it imports with zero missing-script/missing-GUID errors (Library rebuilt from scratch), then close Unity.
10. Single commit: "Import AstroGateway Unity from local history 5e0c223 (full history archived in bundle)". Tag `import-baseline`.
11. Owner creates private repo `astrogateway-unity` (empty, no README) — **approval**.
12. `git remote add origin …` and `git push -u origin main` — **approval**. Never `--force`.
13. Apply ruleset (§4.3), CODEOWNERS, templates via PR.
14. Keep `~/Projects/MissionRedHorizon` as the untouched archive; switch daily work to a clone of the new repo once verified.

Alternative (keeps full history visible on GitHub): add remote to the existing repo and push all branches as-is. No rewrite, but ships ~775 MiB of non-LFS PNGs and the commit emails above, and LFS would only apply to future commits. Converting history with `git lfs migrate import` would rewrite all 12 commits (new hashes) — only on explicit approval, and only in a copy.

Phase 2 — Other repos (each from a clean copy, never in place)
15. Backend: copy `backend/` (without `.env`, `.venv`, `__pycache__`, `certs/` if they are not public CA certs), plus `database/*.sql` and `admin.env.example` (never `admin.local.env`); add the revised SQL manifest from the Codex workspace; `gitleaks` clean; push to `astrogateway-backend`.
16. Android host: copy `app/`, `tools/`, `README.md`, `*.example` files; exclude `EmulatorSDK/`, `EmulatorAVD/`, `unity-export/`, `UnityIntegrationProject/`, `logs/`, `auth.local.properties`, `live.test-secrets.json`.
17. Web app (`~/Projects/astrogateway`): copy without `node_modules/`, `dist/`, `.env.local`.
18. Contracts: export OpenAPI from FastAPI code, add JSON Schemas (§5.2).

Phase 3 — Collaborators
19. Invite Bawantha to the org and teams (§4.2) — **approval**.

---

## 7. Proposed branch / PR workflow (summary)

```
main (protected, squash-only, 1 review, checks green)
 ├─ feat/hud-tab-switcher        → PR → review by other owner → squash
 ├─ feat/rover-first-mission     → PR (scene owner reviews; lock .unity via LFS)
 ├─ fix/api-session-409          → PR in backend + contracts bump
 └─ release/0.x (later)          → tagged builds
```
Daily loop: `git pull --rebase` on `main` → branch → small commits → push branch → open draft PR → CI → ready for review → squash merge → delete branch.

---

## 8. Things I did not verify

- GitHub account/org existence, Bawantha's username, plan level (affects secret scanning and LFS quota).
- Contents of any `.env`/secrets file (deliberately not read).
- Live backend behaviour (not running; blocked by its own startup guard).
- Unity import of a fresh clone (step 9 above).
- Whether the two `.crt` files in `backend/certs` are public CA certificates only (names suggest Supabase CA; verify before committing).

## 9. Decisions needed from the owner

1. Repo architecture: **B separate repos (recommended)** or A monorepo.
2. Unity history: **fresh LFS import, history archived locally (recommended)** or push existing history as-is.
3. Baseline branch for import: **`visual-upgrade` 5e0c223 (recommended)** or `main` 572d3dc.
4. Docs screenshots: **keep out of git, use PR/Release attachments (recommended)** or LFS.
5. Commit identity: **GitHub noreply emails (recommended)** or current addresses.
6. Org name and GitHub usernames for Kapilash and Bawantha.
7. Approval to install `git-lfs` and `gitleaks` on this Mac.
8. Canonical mission id (one of the three in §5.2).

No repository will be created, nothing pushed, no visibility changed and no one invited until these are approved.
