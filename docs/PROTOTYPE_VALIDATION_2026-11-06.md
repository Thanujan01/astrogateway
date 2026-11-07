# Prototype validation: 6 November 2026

Baseline `b790e63` plus local prototype changes. Scope: repeatable local automated checks and compilation; not complete gameplay/device/hosted sign-off.

| Check rerun in this update | Result |
|---|---|
| Backend pytest | 253 PASS; one existing Starlette test-client deprecation warning |
| Web/broker tests | 61 PASS |
| Web lint | PASS |
| TypeScript + Vite production build | PASS; existing large-chunk warning |
| Actual C# offline-coach/parity validation | 446 PASS |
| Actual C# local save-state validation | 21 PASS |
| Android export preparation regression | 1 PASS |
| Unity runtime and Editor C# assemblies | Both compile with pinned Unity Roslyn and staged Unity references; existing obsolete API/serialization warnings |

The Unity assembly check compiled the complete cached staging-project assembly source lists, including the changed mission director/HUD/harness and new state class. This bypasses Editor startup/licensing and is **not** a new Android build or a gameplay run. New save-state tests execute actual C# normalization code under Mono; they do not execute Unity JsonUtility, PlayerPrefs or physical device behavior.

## Follow-up: runnable prototype source

The existing credentials were retained. Added one atomic run checkpoint covering mission progress, player/rover state, resources and observations; explicit failure recovery; confirmed replay with an archived previous run; and a one-command API/web/game launcher.

The actual Missions-scene lifecycle run passed **18/18** checks, including scene reload, partial repair, resources/position/heading, record deduplication, persistent failure, retry, replay confirmation/cancel, clean replay and archive/settings preservation. The full graphical run reached all six missions and the completion debrief: **296 PASS / 2 FAIL / 1 UNTESTED**. One failure is the pre-existing Editor Search exception. The other was a keyboard distance check using wall-clock time while the simulation caps long frame deltas; its measurement was corrected to simulated time without lowering the distance threshold, and the focused control rerun passed all **55 functional control checks**, including keyboard driving at 2.40 m over 1.53 simulated seconds. Its only remaining failure is the same Editor Search exception; no checks were filtered out or relabelled.

A local development Mac build completed. The user's subsequent instruction is **source-code completion; no final APK**. No further packaging is required for this request.

## Runtime and external limits

- Early Editor attempts were blocked by a stale licensing helper. After the user signed in to Hub and the stale helper was stopped, the existing Personal license worked and the scene/runtime tests executed.
- Authorized ADB inventory returned an empty device list. No phone/emulator was available, and no application was installed. The existing provider was checked again: configuration checks pass, but the configured hostname still fails DNS resolution.
- Current graphical results and lifecycle evidence are recorded above. The Editor Search defect and physical touchscreen validation remain outstanding; they are not relabelled passes.
- No fresh APK was exported/built for these changes, in accordance with the latest source-only instruction. Existing APKs predate these changes.
- Hosted account/schema/storage, actual gameplay scoring, advisor history feed, multiplayer and media remain unverified/unimplemented as listed in the [completion plan](PROTOTYPE_COMPLETION_PLAN.md).
- Hosted SQL checks were not rerun in this update; the historical 59 isolated SQL passes are separate evidence.

Local machine evidence is stored outside the source repository at `migration-work/reports/prototype-2026-11-06/` in the surrounding workspace. `summary.json` covers the seven automated check groups; `Assembly-CSharp-compile.log` and `Assembly-CSharp-Editor-compile.log` record compile results separately. These logs, compiler outputs and paths are not portable deliverables and are not committed.

## Changed behavior requiring manual acceptance

Pause/modal/failure now gates mission progression and charging; partial repair restores from local progress; invalid save indices/numerics recover; local time/retries appear in a reopenable unscored debrief. See the explicit manual checklist and remaining tasks in the completion plan before marking the prototype done.
