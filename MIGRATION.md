# AstroGateway source migration

This repository preserves its original README commit and imports source snapshots from this Mac. Original development folders remain untouched. Historical local Git databases and raw backup archives are not imported; unique source files recovered from backups are included under docs/history/recovered-archives.

## Source layout

- `01-Game-Development/MissionRedHorizon`: primary Unity Assets, Packages, ProjectSettings and documentation.
- `01-Game-Development/SourceArt`: ARES-VI rover generators, ARES EVA suit variants and astronaut source material.
- `01-Game-Development/Experiments`: experimental Unity projects and harness scripts.
- `02-Software-Development/Backend/astrogateway-api`: backend source and tests.
- `02-Software-Development/Web-Dashboards/astrogateway-web`: web source and configuration.
- `02-Software-Development/Sentinel-Database`: database migrations.
- `02-Software-Development/Integrations`: Android/Unity host and independent Stage07A adaptive / Stage07B ARIA prototypes.
- `docs/history`: Codex stage material, unique work files and recovered historical source.
- `docs/reference-material`: explicitly named AstroGateway references from Desktop/Downloads.

## Restore and develop

Install Git LFS, clone this repository, and run `git lfs pull`. Open a Unity project from its own folder containing Assets, Packages and ProjectSettings. Install web dependencies using its package lock and backend dependencies from its requirements file. Supply local credentials through the included example configurations.

The migration keeps source and Unity GUIDs intact. Credential-shaped fixture tokens in historical result documents and previously identified email/host identities were redacted in the staging copy. Credentials, private keys, local environment files, raw logs, caches, emulator installations, personal runtime state and unrelated AutoVRL/VPN projects are excluded. Backup archives were inspected and unique eligible files recovered individually rather than uploading opaque archives. Generated validation screenshots and video frames remain in the original folders; source textures, models and other required project assets are retained. Binary source assets use Git LFS.

The manifest in docs/migration records original and staged SHA-256 checksums for every imported file. Repeated asset paths remain where needed to preserve independent Unity project structure; Git/LFS deduplicate identical content.

Verification covers source checksums, secret scanning, Git/LFS object integrity and remote commit/tree checks. Unity compilation, Android builds, backend tests and web tests were not run as part of this source migration. Some existing development scripts reference absolute Mac paths. No portability fixes or history rewrites were applied.
