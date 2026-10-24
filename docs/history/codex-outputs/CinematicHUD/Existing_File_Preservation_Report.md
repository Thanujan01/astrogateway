# Verified preservation

Before implementation: verified backup of Assets, Packages and ProjectSettings at `/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/work/cinematichud/Backup/`; SHA-256 baseline in work/cinematichud/Before_Hashes.json.

After installation: **640 existing files unchanged**, zero missing; all **420 earlier original files unchanged**. This includes all original playable, Native Control, Mars environment, Mission 01 and Visual Polish scenes and shared code/assets. Packages/ProjectSettings unchanged.

Only `52` files were added under Assets/AstroGateway/CinematicHUD/ and its root .meta. Installed new files match the validated lab files exactly. New_Files.json and Preservation_Check.json contain exact evidence. Test-only edits to legacy regression helpers stayed in PlayModeLab and were not installed.

No React, native Android host, backend, Supabase, credentials or remote services were changed. Existing save files were not opened for writing; this scene uses a separate local save namespace.
