# Preservation — PASS
SHA-256 comparison after actual-main import and native preview:

- Current baseline692 files:0 changed/missing.
- Historical original420 files:0 changed/missing.
- New isolated installed138 files:all match staged installation hashes.

Evidence: Preservation_Results.json; Installed_New_Files.json; `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/work/productionvisual/Before_Hashes.json`; verified backup `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/work/productionvisual/Backup`.

New files only under `/Users/kapilash/Projects/MissionRedHorizon/Assets/AstroGateway/ProductionVisual/` and its root.meta. Existing playable/Mars/Mission01/CinematicHUD scenes, source assets, Packages and ProjectSettings remain byte-identical. No existing scripts, authentication/backend/AI, Supabase, React or Android host files were edited. No remote writes, deployment, account access or new external download.

Validation lab has deliberately modified legacy test harnesses for output redirection and timing; these are not copied back. Unity Library/user layout/cache state is outside source preservation scope. Native preview ended with automation unavailable; last observed Play Mode was running. Stop Play Mode manually, without saving changes to original scenes.
