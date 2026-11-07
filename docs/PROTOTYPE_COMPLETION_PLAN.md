# SK2LB / AstroGateway: basic prototype completion plan

Assessment date: 6 November 2026 (Asia/Colombo). Follow-up: the user selected minimal runnable-prototype source updates, retaining credentials, and explicitly excluded a final APK. The run lifecycle/replay implementation now passes 18 actual-scene checks; see the current validation note. Remaining broad product tasks below are retained as a backlog, not requirements to expand this source-only prototype update. Baseline: `b790e63`; local changes described below are not yet committed or published. **Overall status: INCOMPLETE.** Passing automated checks do not mean the complete product or Android prototype has passed acceptance.

## 1. Goal and scope

The current request is to finish a coherent, useful basic prototype before expanding the complete concept. Recommended first acceptance slice: a player can enter the existing Mars Missions scene, learn the controls, finish the six-part survival mission, recover from failure, save/resume local progress, inspect collected observations and reopen an honest debrief. The Android account host must subsequently demonstrate the same flow using an actual account on a physical phone before the integrated prototype is called complete.

This is an implementation priority, not a decision to delete the larger concept. The full backlog below retains account integration, assessment/adaptation, scientific explanations, expanded survival systems, team co-op and media. No percent-complete estimate is supportable.

### Product states to keep visible

| State | What a player may reasonably expect |
|---|---|
| Offline standalone prototype | Local mission and records; no signup requirement; no cloud sync, verified training score or cross-device claim |
| Integrated account prototype | Same canonical account in native/web surfaces; account-bound game launch and saved data; live behavior must be demonstrated |
| Future assessed/team product | Server-derived assessment, personalized training, accepted team membership and shared-world play; these need separate implementation and acceptance |

Keep one primary game project and one primary Missions scene. Historical scene variants, the browser Mars preview and the old cube integration export are not interchangeable demonstrations of the game.

## 2. Evidence reviewed

- Primary Unity `FirstMission` controller, mission director/HUD, native context, sensor catalog/store, offline integration defaults and coach; scene/build configuration and existing graphical harness.
- FastAPI endpoint models, rules/scoring/recommendation/peer modules and tests; React account entry point and package scripts; Android host/bridge and export tests.
- Repository history, `ACCOUNT_IDENTITY.md`, `TEAM_COORDINATION.md`, `GAME_INTEGRATION_UPDATE.md`, `OFFLINE_MISSION_COACH.md`, architecture/setup/team documentation.
- Historical software/runtime reports, including their failures and NOT TESTED limits. Historical results are not rerun results.
- Reference `docs/reference-material/Downloads/AstroGateway_Team_Development_Master_Plan.pdf`, version 1.0, 11 October 2026. This is a planning reference, not a record that all its features shipped or that its role assignments override the named team plan.
- Available project-chat history includes broader Mars survival and UI/navigation ideas. Some detail originated as assistant suggestions; it is not sufficient evidence of final user acceptance of every suggested chapter or reward.

This is an engineering review of active entry points and integration boundaries, not a line-by-line certification of every historical script, binary asset or recovered archive.

## 3. Current implementation and gaps

| Area | Existing source/evidence | What remains before acceptance |
|---|---|---|
| Core mission | Six connected steps: EVA/rover, charging, generator, science, beacons, shelter | Rerun graphical regression after this change; complete it manually on phone |
| Mission lifecycle | Failure/retry, local progress, survival timers | Pause/failure guards and partial repair persistence added here; runtime verification pending |
| Debrief | Completion banner and mission list; persistent local time/retry debrief added here | Inspect layout, reopen after reload, confirm counters; no score conversion |
| Map/navigation | Expandable map, markers, pan/zoom, route guidance, facility states | Physical touch, legibility and obstruction checks; guidance is not auto-driving |
| Controls | Astronaut/rover/camera, braking, repair, accessibility settings | Resolve prior synthetic-touch UNTESTED result with real touch; lifecycle regression |
| Sensors | Simulated observation records, local store, retry seam | Confirm write failures/restart behavior and real per-account upload wiring |
| Identity | Native/web shared Supabase identity, protected surfaces, native descriptor | Actual two-account login/launch/logout/expiry/switch test; hosted target unverified |
| Profile | Web username broker/photo code and candidate SQL | Hosted broker/storage setup; native username/photo interface not complete |
| Backend | Profile/session/command/report/sensor routes and RPC adapter | Reconcile schema candidates; prove real gateway/RLS/Storage, reachable HTTPS and device access |
| Assessment | Existing proposed deterministic rubric and offline-tested rules | Gameplay event mapping, durable trusted evidence and report persistence; `/v1/skills` still has no assessed levels |
| Offline advisor | Explainable difficulty evaluator, 438 parity vectors plus invalid inputs | Verified account-scoped score-history feed and visible advisory UI; no fabricated history |
| Peer matching | Existing-rubric offline module | Authorized report feed, discovery consent, endpoint and UI |
| Visuals/assets | Mars/rover/suit sources, scene variants and quality settings | Accept one consistent asset set; rig/animation/material/license and device budget review |
| Android packaging | Prior actual-Missions native development APK build passed | Fresh export/build for changed source; real install/lifecycle/performance evidence |
| Scientific analysis | Explicit `not_connected` seam | Reviewed data/model and provenance before analytical claims |
| Co-op/media | Architecture contract exists | Invitations, roster, authoritative shared world, chat, attachments, voice/screen sharing are unimplemented |
| Expanded concept | Master-plan and historical chapter ideas | Dedicated playable shelter, greenhouse/resource loops, crew wellbeing, base development and training calendar need explicit feature work |

## 4. Changes completed in this local update

1. Mission simulation now returns before proximity/checkpoint/repair/charging progression while the game is paused, a modal is active or the mission has failed. `timeScale=0` alone does not make a true proximity predicate false.
2. Extracted a backward-compatible `MissionProgressState`: invalid mission indices recover to a new local run; step/checkpoint indices and numeric values are bounded; NaN/infinity cannot poison survival values. Completion is derived from the mission index.
3. Partial generator repair now enters the save and is restored into the generator. Pause/quit save existing state; no per-frame disk write was introduced.
4. Local active time and explicit retry count persist. Completing the chain makes the objective chip open the Mission drawer debrief, with observation count and an explicit unscored/unassessed label. These counters are activity facts, not player-skill labels.
5. Backend sensor-catalog parity test now reads the Unity catalog from this repository instead of silently testing/skipping a separate home-directory project.
6. Added a reproducible local validation runner, 20 actual-C# save-state checks and five graphical harness checks for paused/failed progression, counters and debrief. The new graphical checks still require a successful runtime run.

## 5. Ordered completion backlog

Statuses are at this review; assignments follow technical areas, not a claim of team availability. P0 blocks the first coherent local prototype; P1 blocks the connected account prototype; P2 expands the broader concept. Features not yet built remain explicit even if they follow the first milestone.

| ID | Priority / status | Work and dependencies | Completion evidence |
|---|---|---|---|
| L01 | P0 / local code done; runtime pending | Pause/failure progression fix; save recovery and partial repair | Graphical regression and reload during charging/repair; no progress while failed/paused |
| L02 | P0 / local code done; visual pending | Persistent unscored debrief and reopen action | Finish six missions, read debrief, reload, reopen; accurate local labels |
| L03 | P0 / implemented; scene test passed | New-run/replay flow with confirmation and owner-only reset | Cancel preserves progress; confirm resets only current run; retains other accounts and device settings |
| L04 | P0 / implemented; scene test passed | Consistent save/resume of player/rover, resources, objective and sensor state | Restart mid-mission and after failure without losing/duplicating progress; legacy saves remain usable |
| L05 | P0 / historical partial | Resolve Unity Editor Search exception without suppressing logs | Unchanged no-console-errors gate passes in graphical Editor |
| L06 | P0 / untested | Repeat six-mission control/navigation/sensor regression | All objectives completed through real interaction; failures and recovery recorded |
| L07 | P0 / historical build only | Fresh development APK from current source | Correct `com.sk2lb.astrogateway`, Missions scene stamp, ARM64, build revision/hash |
| L08 | P0 / device dependency | Physical phone smoke and touch/accessibility/performance | Install/start, joystick/brake/repair, map pinch, camera, Android Back, background/resume, no crash |
| L09 | P0 / open | Select one visual baseline and package the demo | No missing materials; legible HUD; approved asset provenance; repeatable launch/demo instructions |
| A01 | P1 / blocked externally | Confirm active approved Auth/database target | Endpoint reachable and approved schema/config known; no secrets in chat/source |
| A02 | P1 / source partial | Reconcile schema/RPC candidates in isolated local Supabase first; then reviewed target | Authenticated CRUD and cross-user deny tests through actual gateway/storage |
| A03 | P1 / unverified | Real signup/verify/login/reset/logout and two-account identity | Same UUID across native/web; expiry/revocation/callbacks tested |
| A04 | P1 / source partial | Native account launch and Unity session lifecycle; needs A01-A03 | ACK, completion, return, cold/warm start, process death, switch user; no cross-account files |
| A05 | P1 / unconnected | Wire actual Missions session/commands/sensor adapters; needs A04 | Ordered/idempotent events, offline queue/reconnect, ownership and expiry behavior |
| A06 | P1 / open | Durable server-derived report pipeline with existing rubric; needs A05 | Duplicate completion does not duplicate credit; forged client scores rejected; insufficient evidence unassessed |
| A07 | P1 / open | Feed account-scoped compatible reports to coach and show provisional advice; needs A06 | Empty/stale/invalid history fallback; account switch clears active state; existing parity tests pass |
| A08 | P1 / source partial | Native username/photo parity, hosted broker/storage; needs A02-A03 | Same identity via email/username, photo upload/remove, cross-user denial |
| A09 | P1 / incomplete | Mission/progress dashboard integrated with actual backend state | Loading/error/empty/offline/expired states; no invented skill charts |
| F01 | P2 / concept | Define shelter/resource/hydroponics/base-building gameplay contracts | One playable, measurable interaction loop per approved chapter; shared save/resource model |
| F02 | P2 / partial experiments | Training calendar, skill focus and learning modules | Existing rubric mapping, explainable plan and repeatable progression; no unearned unlocks |
| F03 | P2 / experiments | Environment/hazard/fault simulation and route constraints | Seeded scenarios, simulated labels, reviewed bounds and replay tests |
| F04 | P2 / disconnected | ARIA/scientific analysis and Tamil/English explanations | Traceable scientific products, grounded citations, bilingual evaluation and abstention |
| F05 | P2 / asset handoff | Rigged astronaut, animation/rover parts, habitat/greenhouse/props | Actual Unity import/animation tests, licenses, LODs and target-phone measurements |
| T01 | P2 / offline matcher only | Authorized player discovery and peer suggestions; needs A06 | Consent/block filtering; no private email/history leakage; explained provisional matching |
| T02 | P2 / unimplemented | Invitations and atomic accepted teams of 2-4; needs T01 | Recipient-only acceptance, expiry/idempotency, fifth concurrent acceptance rejected |
| T03 | P2 / unimplemented | Same-world multiplayer; needs T02 and provisioned game transport/server | Two/four separate clients share objectives; distinct avatars; server rejects forged outcomes |
| T04 | P2 / unimplemented | Member-only chat/private screenshots; needs T02 | Access revoked on leave/kick/logout; bounded private attachments |
| T05 | P2 / unimplemented | Voice and user-triggered screen sharing; needs T03-T04 and media infrastructure | Explicit capture action, visible stop/mute; nonmembers denied; tracks stop on leave |
| Q01 | Cross-cutting / partial | Automated validation and clean-checkout reproducibility | Local checks runner plus documented Unity/Android commands; machine-specific setup removed incrementally |
| Q02 | Cross-cutting / open | Evidence-backed completion record and final demonstration | Source revision, fresh APK checksum, actual device results, known limits and demo recording agree |

### Delivery sequence

1. Close L01-L06 in the existing standalone game; keep cloud features visibly unavailable.
2. Close L07-L09 on one actual target phone: this yields a demonstrable local basic prototype.
3. Close A01-A09 to finish the connected account prototype; preserve offline fallback.
4. Expand F01-F05 and T01-T05 in separate reviewable increments. Do not describe the first local demo as completing these remaining concept requirements.

L03/L04 now use an atomic, owner-scoped checkpoint and recoverable previous-run archive. Actual scene reload/retry/replay checks passed. The current user request stops at runnable source and does not require an APK or expansion into the P1/P2 product backlog.

## 6. Reproduce local checks

From the repository root, using an existing Python environment with backend development requirements and installed web dependencies:

```bash
python3 tools/prototype/validate.py \
  --backend-python /path/to/backend-venv/bin/python \
  --output /path/outside/repository/prototype-evidence
```

The runner executes backend pytest, web/broker tests, lint/build, offline coach parity, local save-state checks and Android export-tool tests. It does not install packages, start authenticated services, change a database or access credentials. `UNITY_MONO_ROOT` can point the save-state validator at another installed Unity Mono directory; the older coach validator still assumes the documented macOS Unity installation.

For the graphical suite use a staging copy, the pinned editor and `AstroGateway.FirstMission.EditorTools.FirstMissionPlaytest.Run`, with `FM_SCENE=Assets/AstroGateway/FirstMission/Scenes/Mars_FirstMission_Missions.unity`, `FM_OUT` outside source and `FM_QUIT=1`. This is a graphical harness: headless compilation is not its substitute. See [game build guidance](GAME_INTEGRATION_UPDATE.md#reproduce-safely).

### Manual acceptance checklist

- [ ] Start the selected scene with clear local/offline identity and readable controls.
- [ ] Walk/run, find/board/exit rover, steer/accelerate/brake and use camera.
- [ ] Complete all six missions in order using interactions, not test teleportation.
- [ ] Pause at an objective/checkpoint and while repairing/charging; no hidden completion.
- [ ] Fail oxygen and storm conditions; retry explicitly; no charging/checkpoint advancement while failed.
- [ ] Reload with partial repair and mid-mission progress; verify local resources and observations.
- [ ] Finish, reopen debrief, reload and reopen it again; no verified score is implied.
- [ ] Replay/cancel behavior works after L03; other accounts and settings remain intact.
- [ ] Expand/pan/zoom map, select facilities and navigate to rover on actual touch hardware.
- [ ] Background/resume, Android Back and process death have understandable recovery.
- [ ] Measure frame time/memory and sustained phone behavior; record hardware and quality preset.
- [ ] For connected milestone, repeat with two real accounts and actual backend authorization.

## 7. Research basis

The following official references inform the validation design; they are not evidence that this project's runtime checks passed.

- Unity [PlayerPrefs.Save](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/PlayerPrefs.Save.html): explicitly save at chosen lifecycle boundaries; avoid writing on every frame.
- Unity [OnApplicationPause](https://docs.unity.com/en-us/engine/6000.3/script-reference/unityengine/monobehaviour/onapplicationpause): lifecycle callbacks need testing separately from an in-game pause menu.
- Unity [collecting performance data](https://docs.unity.com/en-us/engine/6000.0/manual/analysis/profiler/profiling-applications): Editor and target-platform measurements are separate evidence.
- Supabase [row-level security](https://supabase.com/docs/guides/database/postgres/row-level-security) and [RLS tests](https://supabase.com/docs/guides/database/postgres/testing-row-level-security): exercise authenticated owner/member and non-owner denial paths against the actual schema.
- Supabase [local development workflow](https://supabase.com/docs/guides/local-development/cli-workflows): rehearse tracked schema and tests locally before claiming hosted integration.

## 8. Completion decision

**Local engineering foundation improved; full prototype completion is still pending.** No hosted migration/deployment, paid service, real-player model training, store release or GitHub push occurred in this update. See [current validation](PROTOTYPE_VALIDATION_2026-11-06.md) for executed results and precise limits.
