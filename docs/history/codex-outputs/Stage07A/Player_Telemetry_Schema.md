# Player telemetry schema
Version1; rules07a.1. JSON Schema is exported separately in `Telemetry_JSON_Schema.json`. Validated by Pydantic and native SQL per-kind field/type/range checks; unknown keys rejected. No names/email/passwords/tokens, arbitrary owner IDs, final score or trust flags in telemetry. No collector installed in Android/Unity; tests are synthetic.

Batch: schema_version=1, mission_session_id UUID, events1…128. Each event: event_id UUID, elapsed_s0…7200 relative to mission start, strictly increasing within/across batches. World X/Z in meters within approved −125…125. Approved primary route from Unity Docs/Design/02_Mars_World_Layout.md is retained for distance-to-route metrics; no scene changed.

| Kind | Payload fields / bounds | Meaning |
|---|---|---|
| movement | x_m,z_m ±125; speed_mps0…4 | client sample; route error computed server-side |
| reaction | reaction_s .1…60 | game prompt-response duration |
| resource | oxygen_used_l0…1000; water_used_l0…20; power_used_wh0…5000 | cumulative simulated use; nondecreasing |
| equipment | airlock/solar_array/rover/water_recycler/sampler; correct strict boolean | reported interaction outcome |
| emergency | dust/power_loss/oxygen_leak/rover_fault; resolved strict boolean; response_s .1…600 | reported emergency result |
| mistake | navigation/survival/decision_making/equipment/emergency; retries integer0…5 | reported mistake/retry count |
| completion | completed strict boolean | client-reported completion, last event only |

Movement interval must satisfy planar displacement <=4×dt+1m. Resource totals cannot decrease. AI API validates new data against stored same-mission history; readback revalidates persisted events before use. SQL enforces payload shape, ownership, session/lease, chronological insert, batch/event capacity and stream completion. SQL does not prove physical game truth or enforce every movement/resource semantic independently; direct gateway caller can poison ONLY their provisional stream, which fails subsequent semantic readback rather than becoming authoritative evidence. Production server witness remains required.

Whole exact batch replay accepts0 and reports replay count; partial/modified replay conflicts; duplicate IDs/timestamps invalid. Mission and caller are checked even on replay. Native RPC has no caller owner/attestation parameter. POST requests >128KiB rejected413. Batch128 event limit and private data API surface constrain resource use, but internet ingress/rate-limit policies remain future gates.

Scoring inputs are subject to consent and retention before any real collection; no unauthorized user data fetched. Suggested future retention: short raw-event window, deletion/export workflow, minimal derived aggregates with rule version and provenance—policy requires approval, not silently implemented. Example shapes/tests are fictional, never evidence of actual player performance.
