# Setup, build, run and troubleshooting

[README](../README.md) · [Architecture](ARCHITECTURE.md) · [Workflow](DEVELOPMENT_WORKFLOW.md)

## Validation scope

Application dependency installation, execution, tests, Unity compilation, Android/device checks and provider deployment were **NOT TESTED in this documentation update**. The commands below were derived from tracked entry points and configurations; they are not claims of successful builds. Documentation checks are separate. Historical stage reports describe their own snapshots and environments.

## Credential configuration status

See [CREDENTIALS_GUIDE.md](CREDENTIALS_GUIDE.md) for the approved test profile: local app files are configured and ignored; 13 offline checks and 13 synthetic scenarios passed. Live credential verification remains blocked by the configured project hostname DNS failure and dashboard sign-in availability. This does not establish admin/service isolation or establish hosted database readiness.

## Clone and prerequisites

```bash
# NOT TESTED here: new developer clone/setup example
git clone https://github.com/kapilashkapilash2025-netizen/Kabylash-Development-SK2LB.git
cd Kabylash-Development-SK2LB
git lfs install
git lfs pull
git lfs fsck
export REPO_ROOT="$PWD"
```

`REPO_ROOT` is a convenience for the examples, not an application setting. Recompute it from the clone root in each terminal. The repository contains multiple Unity roots and Python services; keep their environments separate.

| Tool | Source basis / requirement |
|---|---|
| Git / Git LFS | LFS patterns in [.gitattributes](../.gitattributes). Binary pointers require authorized LFS downloads. |
| Unity | [ProjectVersion.txt](../01-Game-Development/MissionRedHorizon/ProjectSettings/ProjectVersion.txt) pins 6000.6.4f1. Install matching platform modules via Unity Hub. |
| Unity packages | [manifest.json](../01-Game-Development/MissionRedHorizon/Packages/manifest.json): URP 17.6.0, Input System 1.20.0, Test Framework 1.8.0, AI Navigation 2.0.14, Timeline 6.6.0; preserve the package lock. |
| Python | No pyproject/requirements interpreter constraint pins a universal version. Original scripts record Python 3.14. Choose a compatible interpreter and report the version actually tested. |
| Backend libraries | FastAPI 0.143.0, uvicorn 0.54.0, httpx 0.28.1; dev requirements add pytest 9.1.1 and pytest-asyncio 1.4.0. |
| Web Node/npm | Locked Vite 8.3.3 permits `^20.19.0 || >=22.12.0`; Vitest 5.0.3 permits `^22.12.0 || ^24.0.0 || >=26.0.0`; jsdom 30.1.2 requires `^22.22.2 || ^24.15.0 || >=26.0.0`. A Node 24.x release at least 24.15 satisfies these inspected constraints. No `.nvmrc` pin is tracked. |
| Web frameworks | React 19, TypeScript ~6.0.2, Vite 8, Supabase JS, Three.js/react-three packages and Vitest; use `npm ci` with [package-lock.json](../02-Software-Development/Web-Dashboards/astrogateway-web/package-lock.json) rather than replacing the lock. |
| Android | JDK 17, SDK 36/build-tools 36.0.0, min SDK 26, target 36, NDK 27.2.12479018, ARM64, Gradle 9.3.1, AGP 9.1.1. Kotlin stdlib 2.2.21, Supabase BOM 3.2.6, Ktor OkHttp 3.3.1. Android Studio is optional. |
| Art generators | Python; scripts importing `bpy` must run in Blender's Python environment. Blender version and generator portability are not pinned/validated. |

PowerShell users need their platform's venv activation and environment-variable syntax. zsh/Swift helpers and the sandbox profiles are Mac-specific. This guide does not claim Windows/Linux execution was verified.

## Unity primary project

Open [MissionRedHorizon](../01-Game-Development/MissionRedHorizon) through Unity Hub with 6000.6.4f1. Let packages resolve and LFS assets restore before diagnosing missing scripts/materials. Keep source `.meta` GUIDs.

**NOT TESTED:** choose `Assets/AstroGateway/FirstMission/Scenes/Mars_FirstMission_Missions.unity`, or use **AstroGateway → Open Missions Scene**, then enter Play Mode. The four tracked variants are base v1, HUDv2, Explorer and Missions. [EditorBuildSettings.asset](../01-Game-Development/MissionRedHorizon/ProjectSettings/EditorBuildSettings.asset) currently enables SampleScene, so Build Settings is not a reliable indication of the intended mission scene. Select/review the scene for a build explicitly.

### Playtest and build entry points

**NOT TESTED:** in the graphical Editor, **AstroGateway → First Mission Playtest** calls `AstroGateway.FirstMission.EditorTools.FirstMissionPlaytest.Run`. It uses simulated input, an isolated data directory and mock uploaders; it does not verify live Auth/backend integration. The source restores player data and does not save the scene. Inspect [FirstMissionPlaytest.cs](../01-Game-Development/MissionRedHorizon/Assets/AstroGateway/FirstMission/Editor/FirstMissionPlaytest.cs) before running.

For an Android development build, work on an **isolated project copy**. `FirstMissionAndroidBuild.Build` changes the target, Player/quality settings, URP assignments and build scenes. Its original-path refusal guard is not a general detector of every migrated primary folder. Do not rely on that guard as permission to mutate the main working project.

```bash
# NOT TESTED. Point these at your installed editor and an isolated project copy.
# UNITY_EDITOR and UNITY_PROJECT_COPY must already be set to valid local paths.
mkdir -p "$REPO_ROOT/Build"
FM_APK="$REPO_ROOT/Build/AstroGateway_FirstMission_dev.apk" \
FM_BUILD_SCENE="Assets/AstroGateway/FirstMission/Scenes/Mars_FirstMission_Missions.unity" \
"$UNITY_EDITOR" -batchmode -quit -projectPath "$UNITY_PROJECT_COPY" \
  -executeMethod AstroGateway.FirstMission.EditorTools.FirstMissionAndroidBuild.Build \
  -logFile "$REPO_ROOT/Build/firstmission-android.log"
```

Create the output directory first (`mkdir -p "$REPO_ROOT/Build"`). The build helper defaults to the base v1 scene if `FM_BUILD_SCENE` is omitted. This is a development ARM64 build, not a signed release. General Test Runner availability comes from the Unity test package; it is distinct from custom playtest/builder methods. No universal release pipeline is configured.

## Primary backend

Work inside [astrogateway-api](../02-Software-Development/Backend/astrogateway-api). **NOT TESTED:**

```bash
cd "$REPO_ROOT/02-Software-Development/Backend/astrogateway-api"
python3 -m venv .venv
. .venv/bin/activate
python -m pip install -r requirements-dev.txt
cp .env.example .env
chmod 600 .env
python -m pytest tests -q
# After reviewing/configuring the test environment:
python run_local.py
```

On Windows, activate `.venv` using that platform's activation script; owner-only POSIX mode behavior requires separate verification. Do not source the `.env` file as shell code. `run_local.py` parses an allowlist of keys, requires a regular non-symlink owner-only file and starts loopback port **8000** with access logging off.

| Template key | Purpose / constraint |
|---|---|
| `ASTROGATEWAY_ENVIRONMENT` | Must be `test` for the local runner. |
| `ASTROGATEWAY_APPROVED_PROJECT_REF` | Must match the approved test project when provider settings are supplied. |
| `SUPABASE_URL`, `SUPABASE_PUBLISHABLE_KEY` | Supply both approved public configuration values together or leave both blank for connectivity-only mode. No secret/service-role key. |
| `SUPABASE_JWT_ISSUER` | Optional same-project issuer; validated by Settings. |
| `CORS_ALLOWED_ORIGINS` | Approved origins subject to Settings validation. |
| `DATABASE_URL`, `DATABASE_SSL_ROOT_CERT` | Deprecated; remain empty. |
| `DATABASE_ALLOW_LEGACY_CA` | Remains `false`. |

Blank provider configuration cannot authenticate users and is not live readiness. Even `/health` only reports configuration presence. Provide an independently isolated service identity/filesystem without readable admin/gateway/signing credentials. The original guard checks a known relative admin-file path; the migrated layout does not make that a complete isolation boundary.

Tests use local/mock transports; they do not establish a hosted provider deployment. `tests/test_sensor_records.py` cross-checks the Unity and backend copies of `sensor_catalog_v1.json`; it resolves the Unity path repo-relatively, so it works on any clone as long as both copies stay byte-identical. No source fix is included here. A test-vector regeneration path exists in `tests/test_rules.py`; avoid using it casually because it can rewrite an existing JSON file.

For Unity/client mock integration only, the tracked mock server binds loopback **8766** by default (**NOT TESTED**):

```bash
# From the backend directory with the dev environment active:
PYTHONPATH=.:tests python tests/local_mock_server.py 8766
```

It uses synthetic fixture tokens and in-memory/mock Auth/data. It is not a deployment server and its success cannot establish real provider security or data persistence.

## SQL and provider configuration

[Sentinel-Database](../02-Software-Development/Sentinel-Database) is an earlier candidate, not a complete migration runner. Review the [Stage05B2 revised SQL manifest](../docs/history/codex-outputs/Stage05B2/Revised_Final_SQL_Manifest.md), later [Step3 candidate](../01-Game-Development/MissionRedHorizon/Docs/Step3/SQL/005_mission_commands_reports.sql) and [sensor-record candidate](../01-Game-Development/MissionRedHorizon/Docs/FirstMission/SQL/006_sensor_records.sql) with the current provider state and rollback requirements. Stage07A is a separate candidate. Do not combine old SQL bundles or apply them as a numbered production migration chain. No SQL or remote settings were changed in this documentation task.

## Web account UI

**NOT TESTED:**

```bash
cd "$REPO_ROOT/02-Software-Development/Web-Dashboards/astrogateway-web"
node --version
npm --version
npm ci
cp .env.example .env.local
# Fill approved public client configuration in .env.local, then:
npm run dev
# In the same directory, when checking a change:
npm test
npm run lint
npm run build
npm run preview
```

`src/main.tsx` renders AccountApp; `App.tsx` is the separate earlier 3D preview. Use the actual URL printed by Vite. The config binds **127.0.0.1** and proxies `/account-api` to **127.0.0.1:8000**, stripping the prefix. The template uses `VITE_AUTH_API_BASE=/account-api`; a different API base must be HTTPS. `VITE_SUPABASE_URL` / `VITE_SUPABASE_PUBLISHABLE_KEY` are public client configuration, and `client.ts` pins readiness to the approved project URL/key shape. Do not put database or service-role credentials in `VITE_*`. The `/auth/callback` redirect must be configured for the intended origin; live provider callback behavior remains unverified.

## Android Missions account host

The [host directory](../02-Software-Development/Integrations/android-unity-host-poc) contains `app`, `UnityIntegrationProject`, `unity-export` and `tools`. The Gradle build root is `unity-export`; its settings points `:app` to `../app`. `MainActivity` is the launcher; `PocUnityActivity` is private and runs Unity in a separate process. The default exporter now uses the actual Missions scene from the primary project. The old isolated cube project is historical. See [GAME_INTEGRATION_UPDATE.md](GAME_INTEGRATION_UPDATE.md) for current build results and limits.

**NOT TESTED:** copy the host tree to an isolated working directory before running generators/build tools. Its shell scripts hard-code the original Unity installation location on macOS. Review/adapt your working copy to the installed matching editor/JDK/Gradle/NDK paths first; this documentation update changes no scripts.

```bash
# From your isolated android-unity-host-poc working directory:
cp auth.local.properties.example auth.local.properties
chmod 600 auth.local.properties
mkdir -p logs
zsh tools/export.sh
zsh tools/build.sh
```

The template supplies approved Supabase URL/publishable key and backend URL. Source restricts backend transport to HTTPS except the debug emulator host. Match the exact property names in the template. Keep local configuration out of Git. The scripts regenerate exported files and `prepare_host.py` modifies exported Gradle settings; inspect the diff and do not stage generated changes as part of unrelated work.

`unity-export/gradle/wrapper/gradle-wrapper.properties` identifies Gradle 9.3.1, but the shell build explicitly launches Unity's Gradle JAR; do not assume a standalone `./gradlew` is available. Exported settings include Mac NDK paths. Generated IL2CPP toolchain sources/caches were excluded during migration, so the imported export is not a portable guarantee of a clean rebuild. Regenerate from a staging copy of the primary Missions project with Android modules installed; the host now rejects unstamped historical cube exports.

Expected debug APK path: `app/build/outputs/apk/debug/app-debug.apk` under the working host copy. A compiled APK is not real-device validation. With **one authorized unlocked ARM64 device** connected, source provides (**NOT TESTED; performs install/launch actions**):

```bash
python3 tools/device_smoke.py
```

Do not run the device script until the target and intended install are clear. Authenticated account/refresh, process death, retained-Unity return/relaunch, provider callback and remote result submission need their own device checks.

## Experimental prototypes

Use separate environments and report the exact tested source. Neither prototype is a production-service dependency.

### Stage07A adaptive foundation

[Stage07A README](../02-Software-Development/Integrations/experimental/stage07a-adaptive/README.md) describes a historical local fixture setup. For isolated unit/API tests, from the repository root (**NOT TESTED**):

```bash
python3 -m venv .venv-stage07a
. .venv-stage07a/bin/activate
python -m pip install -r 02-Software-Development/Backend/astrogateway-api/requirements-dev.txt
cd 02-Software-Development/Integrations/experimental/stage07a-adaptive/backend
PYTHONPATH=. python -m pytest tests -q
```

The primary backend dev dependency list supplies test tooling; the prototype also has its own runtime `requirements.txt` with matching inspected runtime pins. Verify compatibility before use. Live API rehearsal and native PostgREST fixtures need separately configured isolated infrastructure; no provider migration or historical private lab is recreated by these commands. Candidate `007a_local_adaptive_candidate.sql` is not applied.

### Stage07B synthetic model and ARIA

From the repository root in a new terminal (**NOT TESTED**):

```bash
python3 -m venv .venv-stage07b
. .venv-stage07b/bin/activate
python -m pip install -r 02-Software-Development/Backend/astrogateway-api/requirements-dev.txt
cd 02-Software-Development/Integrations/experimental/stage07b-aria
PYTHONPATH=. python -m pytest tests -q
```

Runtime model/data/template logic uses the Python standard library; tests use pytest and invoke `/usr/bin/openssl` for local witness fixtures, so that path is another portability constraint. These tests generate temporary test keys; never commit them.

Do **not** treat `python run_local.py` as a turnkey migrated command: it imports the Stage07A `adaptive` package and reads/writes `Evaluation_Protocol.md`, `Datasets` and `Evidence` relative to its historical parent. The actual protocol/evidence now lives under [Stage07B history](../docs/history/codex-outputs/Stage07B). Reconstruct a disposable historical layout and review its sandbox/paths in a separately scoped task before executing it. `offline.sb` hard-codes original machine directories. Tamil parity, external scientific-data training authorization and deployed inference remain unverified/blocked as described in the prototype evidence.

## Source-art and other historical tools

[SourceArt](../01-Game-Development/SourceArt) holds Python generators, Blender scripts, geometry checks, exported models, material files and suit baselines. Select the intended variant before running authoring scripts: several write their outputs beside their source. No all-assets build command or Blender version is pinned. Use an isolated copy, inspect imports/output paths, and validate LODs/prefab GUIDs manually. **NOT TESTED:** generator execution, Blender rendering or geometry checks in this update.

[Step1Work](../01-Game-Development/Experiments/MissionRedHorizon_Step1Work) shell/Swift harnesses, Codex work tools and recovered snapshots can depend on original working folders. They are not part of a single supported bootstrap. Prefer the primary project entry points and treat historical PASS files as evidence of older runs only.

## Troubleshooting

| Problem | Diagnostic / current limitation |
|---|---|
| LFS pointers / missing textures | Check `git lfs version`, account access, `git lfs pull`, `git lfs fsck` and any LFS quota/network errors. Do not recreate GUIDs to compensate. |
| Unity package or shader mismatch | Use the pinned editor/URP and package lock; finish package resolution and LFS download before changing assets. |
| Wrong Unity scene | Build Settings selects SampleScene; open the exact FirstMission scene/menu. |
| Android missing IL2CPP/NDK/JDK | Install matching Unity Android modules and regenerate the integration export. Existing scripts/export paths are Mac-specific. |
| Backend configuration rejection | Use the allowlisted template, owner-only regular `.env`, test marker, complete provider settings and empty retired DB options. Isolation is independent of the known-path guard. |
| Catalog cross-check test fails | The Unity and backend `sensor_catalog_v1.json` copies differ. Compare them and re-sync; the path itself is repo-relative and portable. |
| Web sign-in disabled / profile call fails | Check fixed provider readiness check, public key, callback origin, local backend and Vite proxy. Main entry is the account UI. |
| npm engine error | Inspect locked package `engines`; use a mutually compatible Node version and `npm ci`. |
| ARIA protocol/adaptive import missing | Historical parent paths/PYTHONPATH differ after migration; keep the experiment isolated and reconstruct dependencies deliberately. |
| Source art overwrites outputs | Authoring scripts are generators; run the intended version in a disposable copy. |
| Cross-platform execution differs | zsh, Swift, POSIX file modes, hard-coded OpenSSL/Unity paths and sandbox profiles need explicit platform work. No Windows/Linux run is certified. |
