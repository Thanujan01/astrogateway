# Mission01 first playable milestone — 2026-10-13

## Implemented and controlled-Play-Mode verified

New separate scene: /Users/kapilash/Projects/MissionRedHorizon/Assets/AstroGateway/Mission01Milestone/Mission01_EarthLaunch_Milestone_v4.unity

Flow: Main Menu → New Mission → local callsign/suit-color setup → Mission Briefing → Earth Launch blockout → keyboard movement → distance-gated boarding console → first objective completion. This is the requested first milestone, not the full Mission01 Earth-to-Mars game.

Six menu options exist. Continue is enabled only for a validated version-1 local checkpoint and restores completed boarding status. Profile describes local callsign and explicitly does not authenticate or alter the shared account. Training supplies control guidance. Settings provides requested 30/60 frame caps, not measured performance. Exit stops Editor Play Mode or quits a player build.

New Input System movement, sprint, jump, RMB orbit, camera sphere-cast occlusion, E boarding interaction and Esc pause operate only in the new scene. Automated Input System W movement and grounding verified. Existing Mars controller remains unchanged. Briefing includes objectives, equipment, hazards and survival prerequisites; reward is an explicitly provisional local checkpoint. Resources shown are briefing values, not simulated consumption.

## Test evidence

- Compilation and scene save/reload succeeded in a refreshed local validation copy with the existing project dependencies.
- 13/13 controlled Unity Play Mode checks passed (Unity_PlayMode_Final.log and PlayMode_Controlled_Tests.txt).
- 20/20 existing Mars controlled regression checks passed (MarsRegression/Evidence/Scripted_Play_Mode_Tests.txt).
- 379 original project files verified unchanged after integration; backup and SHA-256 manifest retained.
- Two actual Unity Play Mode camera readbacks saved. They exclude IMGUI and are not proof of menu UI interaction.

The validation copy is work/stage08b/PlayModeLab. Tests used real Unity Play Mode and queued synthetic Input System events; they are not hands-on physical keyboard/mouse checks. Automated menu tests invoke the same state-transition methods used by buttons. Main Editor human interaction, RMB orbit, jump/sprint/wall boundaries, menu layout and audible audio remain untested. No FPS or Android readiness claimed.

## Partial and deferred

Visual art is procedural blockout: illustrative rotating globe/land patches, orbital craft, rocket, launch tower, tanks, control building and capsule trainee. Not geographic Earth data, final astronaut, cinematic assets or production vehicle. The dark IMGUI theme and fade are basic prototype UI, not final accessible/mobile UI. Procedural original ambient tone exists but audible playback was not evaluated. Appearance selection has only one placeholder body; three suit colors are available. No polished character preview, rigged Earth astronaut or narration.

Spacecraft cockpit/system checks, launch countdown/flight, journey, landing, connection to Mars, expanded Mars objectives, inventory/map/journal/repair/flashlight/crouch, survival simulation, full MissionManager/ChallengeManager and authenticated save persistence are not implemented. Existing Mars scene remains independently playable. No AI model or authoritative mission completion is claimed.

## Errors and corrections

Initial compile error: unsupported three-argument File.Move in Unity's .NET profile; replaced with atomic File.Replace for existing saves. Early movement tests failed because batch focus/update setup was insufficient; final harness waits for initialization, enables background simulation and supplies queued test keyboard events. No game movement transforms were teleported to force success. Final observed walking displacement exceeded 10 m.

Unity QuickSearch indexing ArgumentOutOfRangeException remains in the validation Editor logs. It is an Editor indexing issue, not silently treated as a clean Console. No milestone script exception was observed in the passing final test. Project source and security architecture were preserved; no remote operations, packages or external assets added.

See Launch_And_Test_Instructions.md, Asset_Reuse_Inventory.md, Challenge_And_Mission_Architecture.md and Known_Limitations.md. Stop after this milestone; do not proceed to space travel/landing until manual review.
