# AstroGateway Missions: structures, mission chain and in-game guide popups (2026-10-22)

The user asked for missions that the player completes, a full set of structures, and popup text inside the game that guides each mission. This answers the open decision on repair gameplay and the missing buildings.

The work is a new scene, **`Assets/AstroGateway/FirstMission/Scenes/Mars_FirstMission_Missions.unity`**, built with `FirstMissionBuilder.BuildMissions`. It is the Explorer scene (map, navigation, cinematics) with missions added. The Explorer, HUD v2 and v1 scenes are unchanged and remain rollback points. Nothing was pushed, merged or deployed.

## Structures (new, placed in the world)

Each structure has real colliders, so the rover and the route grid treat it as an obstacle. Each one also appears on the map as a facility and replaces the matching "design proposal" entry.

| Structure | Where | What it does |
|---|---|---|
| Research tents (crew camp) | South-west of the domes (-58, -86) | Three A-frame tents, a table, supply crates and two lamp posts. This is the check-in point. |
| Rover charging station | East of the rover's start (5, -96) | A pad that glows and pulses, a charging pylon with a screen, and a battery cabinet. It charges the existing game battery by 4 % per second while the rover is stopped on the pad. |
| Power generator | Near the solar farm (22, -84) | The project's existing `P_PowerStation` model. It starts in FAULT, with flashing amber beacons, a warning light and sparks at the service panel. After repair it shows ONLINE, with green beacons and the sparks stopped. |
| Rover dock | South-west of the rover's start (-22, -108) | A covered two-bay shelter with bay markings, lights, a back wall, a spares crate and a tool rack. |
| 3 checkpoint gates | On the survey route: (60, -40), (66, -96), (30, -108) | Each gate is two posts. The active gate shows an orange light curtain between its posts, and a passed gate turns green. |

## Mission chain (6 missions, 18 steps)

| # | Mission | Steps |
|---|---|---|
| 1 | BASE ORIENTATION | Check in at the tents → walk to the rover → board it |
| 2 | POWER UP | Drive onto the charging pad → hold still until the charge cycle and battery reach 100 % |
| 3 | GENERATOR REPAIR | Drive to the generator → exit the rover → walk to the sparking panel → hold REPAIR |
| 4 | SCIENCE SURVEY | Board the rover → drive to Solar inspection → take a mast camera survey → take an arm sample. These steps use the real sensor records. |
| 5 | CHECKPOINT RUN | Drive through gates 1, 2 and 3 in order |
| 6 | RETURN TO DOCK | Drive into the dock → stop inside it |

The guide UI works like this:

- **Step popups.** Every step opens a guide popup on the right of the screen with:
  - the mission number and title, and the step number;
  - the objective, the how-to text and a tip;
  - step dots;
  - a progress bar for charging, repair and the checkpoints.

  The popup slides in and collapses after 9 seconds. The GOT IT button closes it, and tapping the objective chip reopens it.
- **Step completion.** Finishing a step shows a green STEP COMPLETE flash.
- **Mission completion.** Finishing a mission shows a MISSION COMPLETE banner with the reward and the next mission.
- **Navigation.** Each step sets the navigation target, which shows the orange light pillar, the route and the compass pin. This is guidance only and never moves the player.
- **Repair.** A HOLD TO REPAIR button appears when you stand at the generator's fault panel.
- **Mission list.** The ⋯ menu's MISSION section lists the six missions as DONE, ACTIVE or LOCKED, with the steps of the active mission.
- **Saving.** Progress is saved on the device (PlayerPrefs key `AstroGateway.FirstMission.v1.Missions`). It is local and unscored.

All values are labelled honestly. The charge, the battery and the generator state are game values (SIMULATED), science values stay SIMULATED and LOCAL ONLY, and the AI analysis is still NOT CONNECTED.

## Tests

| Suite | Result |
|---|---|
| Missions scene: full playtest, including all earlier checks and about 35 new mission checks | **279 PASS / 0 FAIL** |
| Explorer scene (regression) | **245 / 0** |
| First Mission v1 (regression) | **155 / 0** |

The mission checks drive the real controls:

- walk to the tents;
- board the rover;
- drive onto the pad and charge from 70 % to 100 %;
- drive to the generator, exit, hold REPAIR and see it come back ONLINE;
- complete the science survey from the stored records;
- drive through all three gates in order;
- park in the dock until ALL MISSIONS COMPLETE.

The playtest moves the rover between missions as a test shortcut. The game itself never teleports the player.

The cinematic intro check failed once out of the last three runs. It is a timing check: on a slow start the 4.5 second intro can finish before the test reaches it.

## Android
- APK: `~/Projects/MissionRedHorizon_Step1Work/FirstMissionAndroid/Build/Missions/AstroGateway_FirstMission_Missions_dev.apk` (development build).
- It was not installed, because the phone was not connected. Missions are **NOT TESTED** on hardware.

## New and changed files
- New:
  - `Scripts/Missions/MissionDirector.cs`
  - `Scripts/Missions/MissionStructure.cs`
  - `Scripts/Missions/GeneratorState.cs`
  - `Scripts/Missions/ChargingPadState.cs`
  - `Scripts/Missions/CheckpointState.cs`
  - `Scripts/HUD/MissionsHUD.cs`
  - `Editor/MissionStructuresBuilder.cs`
  - `Editor/FirstMissionLayoutProbe.cs` (read-only layout renders)
  - `Meshes/MissionTent.asset`
- Changed:
  - `Nav/FirstMissionNavigation.cs`: registers the built structures and drops the matching proposals.
  - `HUD/ExplorerHUD.cs`: hooks for the objective chip and the mission section; the route chip is now a status label, not a small tap target.
  - `HUD/HudIcons.cs`: new icons check, tent, wrench and gate.
  - `Editor/FirstMissionBuilder.cs`: `BuildMissions`.
  - `Editor/FirstMissionPlaytest.cs`: mission tests.
- Unity requires a MonoBehaviour's class name to match its file name. Putting several components in one file broke them in the saved scene, so each structure component has its own file.

## Still needs your decision
- **TRAVERSE speed** (above 3.8 m/s): this changes the shared rover controller.
- **Jump.**

## Rollback
- Missions only: delete `Scenes/Mars_FirstMission_Missions.unity`, `Scripts/Missions/`, `HUD/MissionsHUD.cs` and `Editor/MissionStructuresBuilder.cs`.
- Everything back to before this change: extract `~/Projects/MissionRedHorizon_Backups/Missions_PreChange_20261022/firstmission_untracked.tgz`.

## Update: video and extra visuals (2026-10-22)
- New visuals in the Missions scene (`Scripts/Missions/MissionFx.cs`, `HUD/MissionsHUD.cs`):
  - a cinematic title card for each mission (letterbox bars and a big title);
  - an energy stream from the charging pylon into the rover;
  - a power-wave ring when the generator comes back online;
  - spark bursts and ground rings for completed steps, gates and missions;
  - a higher dock roof so the chase camera stays clear.
  Reduced motion turns off the rings and keeps the bursts small.
- Playtest: **282 PASS / 0 FAIL**, which includes new checks for the title card, the charge stream and the effects.
- Video: `~/Projects/MissionRedHorizon_Step1Work/FirstMission/Video/AstroGateway_Missions_Gameplay_v3.mp4` (1920×1080, 30 fps, 4 min 30 s).
  - An autopilot (`Editor/FirstMissionDemoRecorder.cs`) plays all six missions through the normal HUD controls and along the real routes, with no teleporting.
  - Frames were captured on a fixed 30 fps game clock and encoded with `encode_video.swift` (AVFoundation).
  - Recorded in the Mac Editor, not on the phone.

## Update: survival story "storm day" (2026-10-23)
The user asked for proper survival missions with a purpose. The six missions now tell one story: last night's storm knocked out base power, life support is running on a draining oxygen reserve, and a second storm is coming.

| # | Mission | Purpose |
|---|---|---|
| 1 | WAKE-UP CALL | Collect the EVA kit at the camp, then run to the rover |
| 2 | POWER UP THE ROVER | The storm drained the battery to 35 %; charge it for range |
| 3 | RESTORE LIFE SUPPORT | Repair the generator before the oxygen reserve runs out |
| 4 | SCOUT THE SUPPLY SITE | Survey Solar inspection as the landing site for tomorrow's lander |
| 5 | LIGHT THE STORM BEACONS | Drive through the 3 gates to switch on navigation beacons |
| 6 | TAKE SHELTER | Get into the dock before the storm hits; the ending is "You survived the storm" |

Survival rules (game values, SIMULATED):
- **BASE O2** starts at 40 % and drains by 0.12 % per second until the generator is repaired, then refills by 2 % per second. At 0 the mission fails with LIFE SUPPORT FAILED.
- **STORM countdown** of 3:20 starts with mission 5. Dust weather begins at 1:40, and the haze thickens, turns browner and the sun dims as it nears. At 0:00 outside the dock the mission fails with CAUGHT IN THE STORM.
- **MISSION FAILED** screen with a **RETRY** button. Retry keeps the progress inside the mission and gives 30 % oxygen or 2:30 more storm time.
- A survival panel at the top left shows BASE O2 and either the rover battery or the storm timer, and pulses red when critical.

Tests: **290 PASS / 0 FAIL** (new checks cover oxygen drain and refill, both failure types, retry, the storm warning and the storm build-up). A new APK was built. The video was re-recorded: `Video/AstroGateway_Missions_Gameplay_story1.mp4` (1080p), plus two 720p parts.
