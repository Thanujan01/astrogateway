# Baseline model evaluation

A local five-head logistic regression was actually fitted on 600 synthetic rows after the recorded protocol/provenance gate passed. Validation:100 rows; held-out test:100 rows/20 disjoint synthetic players. Rules vs model comparison uses actual Stage07A score proxies, not an invented replacement baseline. No trained model is integrated into FastAPI or product inference.

| Method | Macro test accuracy | Macro Brier |
|---|---:|---:|
| logistic_raw | 0.6840 | 0.1982 |
| logistic_raw_calibrated | 0.6840 | 0.1978 |
| rules_raw | 0.6620 | 0.2269 |
| rules_raw_calibrated | 0.6620 | 0.2318 |

Full per-skill accuracy/Brier/log-loss/ECE, synthetic difficulty/language counts and validation temperatures: Evidence/baseline_evaluation.json. Weights/normalization: Evidence/baseline_model.json. Protocol: Evaluation_Protocol.md; data/manifest: Datasets/. Run command: PYTHONDONTWRITEBYTECODE=1 PYTHONPATH=outputs/Stage07B/Development work/stage05b/venv/bin/python outputs/Stage07B/Development/run_local.py . The package relies on the existing Stage07A local assessor and existing Python environment; no new libraries installed.

Gate checks source/protocol hashes in runner, canonical data hash, synthetic authorization, exact schema, finite bounds, duplicate IDs, class presence, player-disjoint splits and strictly ordered time windows. Numerical unit tests previously exercised three optimization iterations on generated fixtures; they are separate from this benchmark. Hyperparameters fixed before benchmark, temperatures selected on validation only, no test-driven tuning. Plain JSON serialization avoids executable-model formats.

Results establish pipeline operation only. Synthetic latent structure and label choices determine outcomes; accuracy is not real astronaut performance. Language parity is not human fairness. Small subgroup counts and repeated rows per player reduce effective sample size; confidence intervals, independent cohorts, genuine outcome labels, consent and drift monitoring are unavailable. No deployment acceptance threshold claimed. Scientific observations are not used to train this skill model.

Scope: local preparation only, 2026-10-09. No remote database operations, live accounts, player collection or changes to Android, Unity or React. Stage05B original-user-JWT → PostgREST validation → native RLS remains the supported user-data path. No service-role/admin credentials in this package.
