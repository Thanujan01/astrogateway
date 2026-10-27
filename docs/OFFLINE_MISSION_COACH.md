# Offline mission coach: first implementation

The Unity/Android-compatible `OfflineMissionCoach` evaluates the existing difficulty policy locally, with no network, inference SDK, downloaded weights or personal-data collection. This is an explainable rules model, **not a trained ML model**. Backend scoring remains authoritative. The coach never awards points, launches a mission or changes a running mission.

Inputs are chronological completed reports for the current account, deduplicated by session ID, plus the number of weather-safety assessments. The caller must obtain those facts from the existing rubric; activity counts are not skill scores. Reports must match the bundled rules version. Invalid/nonfinite/out-of-range scores, duplicate sessions, incompatible versions and invalid difficulty cause abstention with introductory difficulty. Empty history returns introductory advice. The caller must keep caches separated by authenticated account and clear active state on logout. This class does not authenticate or verify cached reports.

Policy: two scores >=85 raise difficulty; two scores <50 lower it; otherwise retain it. If weather safety has not been assessed, last score >=70 suggests the storm scenario, except after lowering difficulty. Values come from the shared mission configuration, not a new scoring rubric. There is one canonical mission; this does not reorder the six dependent story chapters. Focus-skill selection and learning-module recommendations remain in the backend engine.

Validation: `python3 tools/offline-coach/generate_vectors.py` generates 438 synthetic boundary cases directly from the existing backend recommendation engine. Unity `-executeMethod OfflineCoachValidation.Run` compares the C# evaluator against those expected difficulties and tests eight invalid-input cases. Fixtures include 49.999/50, 69.999/70 and 84.999/85. These are policy parity checks, not evidence of improved player outcomes.

## Integration still required

There is currently no completed, account-scoped verified score-history feed from the gameplay scene into this coach and no visible coach panel. Consequently this commit does not claim personalized suggestions are active in the shipped game. Next integration must pass verified cached reports for the current user, show advice explicitly as provisional, retain an offline introduction fallback, and validate account switching on a physical Android device. Physical-device performance and offline APK behavior have not yet been measured for this addition.

A learned replacement needs approved, consented player evidence and evaluation by player-separated datasets. NASA Mars environmental measurements cannot serve as player-skill labels. Existing simulated weather values must remain labelled simulated. No observed NASA dataset was downloaded or used to train this implementation.

## Executed validation

2026-10-27: `python3 tools/offline-coach/validate_mono.py` compiled the actual C# coach, JSON parser and extracted existing config class using Unity bundled Mono: **446/446 PASS**. Unity Editor batch validation could not finish because licensing initialization stalled; it is not counted as a pass. No Android APK/device test was performed for this commit.
