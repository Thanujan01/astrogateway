# Offline training schema

Version 07b.1, canonical JSONL with SHA-256 manifest. Exact row keys: schema, row_id, player_group, mission_group, day, split, evidence, language, difficulty, features, labels, label_source, seed. Manifest binds generator and pre-benchmark protocol hashes, seed, permitted local synthetic use and explicit no-real/no-science flags. Never ingest arbitrary client final scores as targets.

Five labels in order: navigation, survival, decision-making, equipment handling, emergency response. Labels are binary simulated next-task success draws from independent latent generator parameters; they are not validated competency or rule-score teacher truth. Six features are five actual Stage07A score proxies and severity index, all finite [0,1]. Difficulty progression is represented by severity/difficulty and longitudinal day; no deployment of learned difficulty policy. Existing gradual difficulty rules remain authoritative.

Player groups train sim-0000…0059/day1…10; validation sim-0060…0079/day11…15; test sim-0080…0099/day16…20. Mission IDs unique per generated task. Group overlap and time reversal rejected. Means/scales fit train only; calibration uses validation only. IDs/day/seed/language excluded from model inputs. Assignment of en/ta is synthetic parity, not a real language subgroup.

Future real row extension must include pseudonymous verified subject linkage held separately, session/witness IDs, scenario/rules versions, trusted outcome origin, consent version/purpose, retention deadline, event-feature transform version and dataset lineage. Requires review and new schema; unsupported evidence rejected now. Account deletion must invalidate downstream partitions and retraining lineage. Scientific observed data remains separately scoped with its own units/uncertainty/provenance, never mixed into human labels.

Scope: local preparation only, 2026-10-09. No remote database operations, live accounts, player collection or changes to Android, Unity or React. Stage05B original-user-JWT → PostgREST validation → native RLS remains the supported user-data path. No service-role/admin credentials in this package.
