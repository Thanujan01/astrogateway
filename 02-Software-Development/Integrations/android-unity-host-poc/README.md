# AstroGateway native account + Missions host

The current export path uses the primary game's **Mars_FirstMission_Missions** scene, not the historical cube integration scene. The isolated `UnityIntegrationProject` and Stage05 backups are retained as historical prototypes; they are no longer the default export input.

## Development build

Use a staging copy of the main Unity project. `ASTROGATEWAY_GAME_PROJECT` selects its absolute path; `ASTROGATEWAY_UNITY_EDITOR` can override the Unity executable. Then run `zsh tools/export.sh` and `zsh tools/build.sh`. The exporter sets Android Activity entry, ARM64, API26 minimum/API36 target, application ID `com.sk2lb.astrogateway`, development version 0.3.0. Native Kotlin namespace remains `.poc` for class/bridge compatibility; host application version is 0.3-missions. Release signing is not configured or implied by a debug APK.

The main game's `MissionsAndroidBuild.ExportHost` export replaces the Gradle Unity library. `prepare_host.py` preserves the fresh Unity settings and idempotently substitutes the native `:app` launcher. It does not restore stale exporter configuration over a newly generated file.

## Identity and gameplay boundary

Native Supabase Auth still verifies the account and initializes a backend mission context before launching the private Unity Activity. `NativeMissionBridge` freezes the scene roots before Start until it receives a bounded descriptor (canonical user/session/launch UUIDs, fixed mission ID, expiry within one hour). It sends ACK only after context validation. FirstMission progress, Missions progress and sensor files use an account-specific local namespace; anonymous standalone/editor play keeps its existing offline namespace. No old anonymous progress is assigned to a signed-in user automatically.

A warm new launch reloads the scene before enabling a new account's gameplay objects. Local expiry stops the scene and the native expiry/revocation handling returns to the dashboard. Native callbacks carry no password, access token or refresh token. This is an account-context/local-save integration, not proof that the game is authenticated to the remote API.

Six local mission completions produce `LOCAL_MISSIONS_COMPLETE`, explicitly not `POC_RESULT_OK`. They do not award a server-verified score or pretend that offline mission outcomes have been synchronized. Game HTTP adapters/scoring still require their separately verified live integration.

## Validation

See [game integration report](../../../docs/GAME_INTEGRATION_UPDATE.md). Graphical Missions regression tests and build/package checks are separate from physical device acceptance. Run `python3 tools/tests/test_prepare_host.py` for the export-wiring regression.

The old `device_smoke.py` is cube-only and stops by default. Its automatic immediate RESULT expectation is incorrect for a real mission. It can be opted into explicitly for the historical cube project only. Updated Missions acceptance requires a real verified account, two different users, launch/return/expiry/revocation checks, full gameplay completion, and confirmation that local completion is not submitted as a verified training score. No device test is claimed without a connected device.
