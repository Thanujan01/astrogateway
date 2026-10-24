# Mission rules design — Step 4 (2026-10-17)

**Status: PROPOSED DEFAULTS. Simulation values, not Mars measurements. Not enabled in any scene, endpoint or database.**
Single source of configuration: `mission_rules_v1.json` (`rules_version: mission-rules-1.0.0-proposed`), kept identical in
`AstroGatewayAndroid/backend/rules/` and `MissionRedHorizon/Assets/AstroGateway/MissionRules/Config/` (a Unity test checks they match).

## 1. Canonical mission identifier (proposal, nothing rewritten)
| Stored id (unchanged) | Where it lives | Resolves to | Build variant |
|---|---|---|---|
| `mission-red-horizon-poc` | backend DB check constraint, Android shell, Step 3 endpoints | `ag.mars.red-horizon.surface-ops` | — |
| `mission.red_horizon.slice` | Unity NativeControlSlice registry | same | native_control_slice |
| `mission.red_horizon.research_base.v1` | Unity MarsEnvironment registry | same | research_base_v1 |
| `mission.red_horizon.visual_polish.v2` | Unity MarsVisualPolish registry | same | visual_polish_v2 |
| `mission.red_horizon.cinematic_hud.v1` | Unity CinematicHUD registry | same | cinematic_hud_v1 |
| `mission.red_horizon.production_visual.v1` | Unity ProductionVisual registry (+ MissionIntegration copy) | same | production_visual_v1 |

- Canonical form: `ag.<body>.<campaign>.<mission>` with the version kept as a separate integer (`version: 1`), so content updates don't change the id.
- The Unity ids name build variants of one mission, not separate missions, so the variant is kept beside the canonical id.
- `MissionCatalog.resolve()` (Python) and `MissionCatalog.TryResolve()` (C#) return both `original_id` and `canonical_id`. Reports carry both.
- Backward compatibility: the database keeps accepting `mission-red-horizon-poc`. Adding the canonical id to the DB would be a separate, approved migration (alias table, then widen the CHECK). No existing data is changed.

## 2. Principles
1. **Server authority.** Scores, skills and recommendations are computed only on the server from server-recorded, server-timestamped commands. Client-sent oxygen, ticks, positions or "score" fields are ignored (tamper test E).
2. **Simulation is labelled.** Every output has `provisional: true` and/or `source: simulation`. Reference ranges below are context, not inputs from instruments.
3. **Config-driven.** All numbers live in the JSON and are validated on load (weights sum to 1, thresholds ordered, scenarios well-formed, aliases resolve).
4. **Nothing switched on.** `penalties_enabled: false`, `effects_enabled: false`. No scene, prefab, endpoint or migration uses the engines yet.

## 3. Oxygen / life support (simulation)
Units: kg O₂, kg per simulated hour, seconds. Simulated time = real time × `time_compression` (20 → 15 real minutes = 5 simulated hours).

`rate(activity, weather) = activity_rate[activity] × workload[weather] + leak`
`O₂(t+Δ) = max(0, O₂(t) − rate × Δ_real × time_compression / 3600)`

| Parameter | Proposed default | Basis (to be confirmed by science reviewer) |
|---|---|---|
| Suit O₂ capacity | 0.60 kg | Order of magnitude of a current EVA suit primary O₂ supply (~0.5 kg) |
| Idle / scanning / driving / walking / running | 0.035 / 0.045 / 0.040 / 0.060 / 0.095 kg/h | Metabolic O₂ ≈ power ÷ ~20 kJ per litre O₂: ~140–370 W |
| Leak | 0.002 kg/h | Small suit leakage allowance (game assumption) |
| Workload multiplier | clear 1.00, light dust 1.05, storm 1.15 | Gameplay assumption (harder movement/visibility) |
| Thresholds | caution 30%, warning 15%, critical 5% | Gameplay assumption |

Derived (% of tank per real second, matching the HUD): walking clear 0.0574, storm 0.0657; idle 0.0343; driving 0.0389; running 0.0898. Full-tank endurance in real minutes: running 18.6, walking 29.0, driving 42.9, idle 48.6.
The server cannot see speed, so EVA time is costed as walking and rover time as driving. Pauses consume nothing.
Limitations: no CO₂/thermal/pressure model, no refill/airlock rules, constant rates per activity, no individual physiology. **No consequence at 0% is implemented** (needs approval).

## 4. Mars weather (simulation)
States (gameplay values): clear τ 0.5, wind 5 m/s, visibility 280 m; light dust τ 1.5, 10 m/s, 130 m; dust storm τ 4.0, 18 m/s (gusts 28), 48 m.
- Optical depth τ context: background dust is usually below about 1; the 2018 global storm exceeded 10. τ 4 is a strong regional-storm stand-in.
- Temperature: cosine diurnal cycle, `T = min + (max−min)·(cos(2π(h−14)/24)+1)/2 + offset`, min −80 °C, max −10 °C (Gale-crater-like order of magnitude). Dust adds +2/+6 °C (warmer nights).
- Solar power factor: `exp(−0.35·τ)` — a rough approximation including diffuse light; storm → 0.25.
- Advisory: clear nominal, light dust caution, storm shelter.
- Scenarios (active mission seconds): difficulty 0 clear; difficulty 1 light dust 300–600 s; difficulty 2 light dust 240 s, storm 360–600 s, light dust to 720 s, then clear. The server uses the scenario, not client weather reports.
Limitations: deterministic and scripted; no pressure, seasons, location, dust devils or radiation; wind has no dynamic-pressure effect on gameplay.

## 5. Mission score (transparent)
`score = 100 × Σ wᵢ·sᵢ` with sᵢ ∈ [0,1]. If a criterion cannot be assessed (no storm in the scenario), it is excluded and the other weights are renormalised; the report lists `not_assessed` and `weights_applied`.

| Criterion | Weight | Subscore sᵢ (server evidence only) |
|---|---|---|
| Observation | 0.35 | min(1, distinct simulated scans during EVA ÷ required 5) |
| Life support | 0.25 | 0 if O₂ ever reached 0; else clamp((end fraction − 5%) ÷ (30% − 5%)) using the server oxygen estimate |
| Weather safety | 0.20 | rover-sheltered seconds ÷ dust-storm seconds (only when the scenario has a storm) |
| Rover operation | 0.10 | min(1, completed board→exit cycles ÷ 1) |
| Mission planning | 0.10 | 0 if not completed; 1 if active time ≤ 900 s; linear to 0 at 1800 s |

Grade bands: ≥85 Excellent, ≥70 Proficient, ≥50 Developing, else Needs practice.
Known gap: scans carry no station id, so "distinct stations" can't be verified server-side; adding `station_id` to the scan payload is a proposed API change (needs approval).

### Sample scenarios (computed by the engine)
| Scenario | Subscores | Score / grade |
|---|---|---|
| A: 5 scans, 1 rover cycle, done in 600 s, difficulty 0 | obs 1, O₂ 1 (67% left), rover 1, plan 1; weather not assessed | 100 Excellent |
| B1: same at difficulty 2, on foot through the storm, 700 s | weather 0, rest 1 | 80 Proficient |
| B2: same, sheltered in rover 360–600 s | all 1 | 100 Excellent |
| C: 1 scan, abandoned, evaluated at 3000 s | obs 0.2, O₂ 0 (ran out), rover 0, plan 0 | 8.8 Needs practice |
| D: A but finished at 1350 s | O₂ 0.74 (24% left), plan 0.5 | 85.8 Excellent |
| F: 3 scans, no rover, 1000 s | obs 0.6, rover 0, plan 0.89 | 68.6 Developing |
| E: A with forged client `oxygen`/`score` fields | identical to A | unchanged |

### Multiplayer compatibility (not implemented)
Every command carries `participant_id` (always `solo` today) and scoring takes one participant's timeline, so a team mode can score each participant with the same formula. Proposed team rule for later approval: team score = mean of participant scores, with shared objectives (e.g. scans) counted once per team.

## 6. Skills, proficiency and levels
Skills = the five criteria. Each completed session adds one evidence value per assessed skill (its subscore). Playtime is never used.
A **demonstration** is a value ≥ 0.70. Rolling mean = mean of the last 5 assessed values. A level is reached only when all of its criteria hold:

| Level | Name | Demonstrations | Distinct sessions | Rolling mean | Hardest difficulty demonstrated |
|---|---|---|---|---|---|
| 0 | Novice | 0 | 0 | — | — |
| 1 | Developing | 1 | 1 | 0.50 | 0 |
| 2 | Proficient | 3 | 3 | 0.75 | 1 |
| 3 | Advanced | 5 | 5 | 0.85 | 2 |

Trainee level = minimum level across all five skills (conservative): a trainee who has never faced a dust storm stays Novice overall. Each skill reports what is missing for the next level, in plain language.

## 7. Recommendations (rules engine, replaceable)
Interface `RecommendationEngine.recommend(history, skills) → {mission_id, difficulty, focus_skill, learning_modules, rules_fired, reasons, engine, rules_version}`. A future ML model implements the same method; the rules engine stays as the fallback.

| Rule | Condition | Action |
|---|---|---|
| R1 New trainee | no completed missions | canonical mission, difficulty 0 |
| R2 Difficulty | last 2 scores ≥ 85 → +1 (max 2); last 2 < 50 → −1; otherwise keep | |
| R3 Focus | weakest skill: lowest level, then lowest rolling mean, then config order | sets focus_skill |
| R4 Learning | any last-mission subscore < 0.5 | add that skill's learning module |
| R5 Unseen skill | weather safety never assessed, last score ≥ 70, not lowered by R2 | raise to the storm difficulty (2) |
Every fired rule adds a human-readable reason.

## 8. Acceptance tests (all passing locally)
Backend `tests/test_rules.py` (36): config validation incl. 6 invalid configs; all 6 legacy ids resolve without rewriting, unknown ids fail; oxygen units, thresholds, ordering idle<driving<scanning<walking<running; weather timeline/intervals/effects; pauses and post-completion commands excluded; duplicate samples and rover scans not counted; scenarios A–F; forged client values don't change the score; skills need repeated evidence across distinct sessions and difficulty; playtime alone gives no level; one session counts once; Advanced needs difficulty 2; trainee level conservative; R1–R5; engine replaceable; shared vectors match.
Unity `Step4RulesTest` (12): config identical to backend; 15/15 oxygen and 18/18 weather vectors match the backend; existing default model unchanged; all 5 MissionRegistry assets resolve; no scene or prefab references the rules.

## 9. Needs scientific or owner approval
1. All numeric defaults in §3–§7 (rates, capacity, time compression, τ/wind/temperature values, weights, thresholds, level criteria, recommendation thresholds).
2. Canonical mission id and the DB alias migration plan.
3. Consequences at low/zero oxygen and weather effects on gameplay (currently disabled).
4. Adding `station_id` to scan commands.
5. Team scoring rule before any multiplayer work.
6. Wiring the engines into `/v1/reports` and `/v1/skills`, and into the integration scene.
