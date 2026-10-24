# Play Mode results

Actual existing project and MarsOutpost_Environment_v01 scene; Unity6000.6.4f1/URP17.6.0;2026-10-09.

| Check | Result | Evidence / limits |
|---|---|---|
| Editor/project compatibility | PASS | Window title and ProjectVersion match; package versions match |
| Main Mars scene open | PASS | Native window URL identifies exact scene |
| Fresh Play Mode entry | PASS | Command-P exit, then Command-P entry; blue Stop button and fresh Game View capture |
| Terrain/outpost/lighting/sky render | PASS for arrival view | Actual UI capture and six new URP camera renders; no complete visual sweep |
| Console | PASS at observed capture |0 errors,0 warnings; no Console Clear used; later capture adds utility log |
| Scene/material external references | PASS static scan |0 unresolved GUIDs; not exhaustive platform validation |
| Astronaut close-up/animations/movement | BLOCKED: absent |0 character model/animation/controller assets; no implementation |
| Rover presence | PASS inventory/Hierarchy | StaticRoverChassis_NoDriving; no production vehicle quality claim |
| Rover driving/navigation | BLOCKED: absent | No controller or NavMeshAgent |
| Dedicated rover close-up | BLOCKED: tooling | Coordinate targeting and search-selection automation failed; overview not presented as close-up |
| Dust storm/weather | BLOCKED: absent | No ParticleSystem/weather code; sky/fog only |
| Mission HUD/oxygen/power | BLOCKED: absent | No Canvas or HUD runtime code |
| Camera controls | BLOCKED: absent runtime controller | Arrival is fixed camera; Editor camera controls are not player controls |
| FPS | UNVERIFIED | Stats overlay could not be opened; no current measurement or old FPS reused |
| Gameplay video | NOT RECORDED | No exposed recording operation/installed Recorder used |
| Asset preservation | PASS scoped comparison | Assets/Packages/ProjectSettings baseline before camera capture unchanged afterward |

Reproduction: open project with matching Editor; open Assets/AstroGateway/Scenes/MarsOutpost_Environment_v01.unity; press Command-2 for Game, Command-P to enter Play; inspect Arrival view; use Window→General→Console. Stats must be enabled manually for current FPS. Runtime interaction cannot be validated until those components exist. No gameplay feature added to satisfy preview.

Evidence/Editor_Diagnostics.txt records actual Playmode entries, existing reload-domain/reload-scene-disabled configuration and ASTROGATEWAY_ART_CAPTURE_OK. Those reload settings may matter once stateful gameplay is added; no settings changed. A redundant launch's licensing refresh message is not treated as a compile or Play Mode failure: existing Editor rendered successfully. Full local logs remain in project Logs/Editor.log; deliverable diagnostics exclude licensing/environment details.

Initial Scene View was flat/outline and current Scene capture remains so. Game View correctly rendered. Smallest safe viewing adjustment is manual Shaded mode/Frame Selected, no source changes; full underlying Scene View issue not isolated. No crashes or compilation errors observed during successful Play/render capture, but no long-duration stress or gameplay test claimed.

Local preview stopped after evidence/report creation. No remote DB/deployment, authentication/security change, scene Save, asset replacement or source implementation.
