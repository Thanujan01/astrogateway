from pathlib import Path
import json,hashlib,sys
root=Path('outputs/Stage07B');sys.path.insert(0,str(root/'Development'))
from stage07b.ingest import ingest
p=Path('outputs/Stage07A/Samples/hirise_metadata_sample.lbl');old=json.loads(p.with_name('provenance.json').read_text())
m={'dataset_id':'MRO-M-HIRISE-3-RDR-V1.1/PSP_010502_2090_RED','source_url':old['source_url'],'organization':'NASA PDS / University of Arizona HiRISE','version':'2.0','retrieved_at':old['retrieved_at_utc'],'sha256':old['sha256'],'license':{'status':'unverified','evidence_url':'https://www.uahirise.org/faq/'},'attribution':'NASA/JPL/University of Arizona; individual-product review pending','units':{'map_scale':'label-declared; not decoded elevation'},'coordinates':{'site':'Uranius Fossae, not Jezero','frame':'retain original label; reviewed transform pending'},'uncertainty':{'vertical_m':None},'missing_data':{'policy':'No raster values loaded'},'limitations':'Projected image label, not DEM or Mars weather observations','kind':'terrain_metadata','training_permission':False}
r=ingest(p,m);(root/'Evidence/hirise_ingestion.json').write_text(json.dumps(r,indent=2)+'\n')
e=json.loads((root/'Evidence/baseline_evaluation.json').read_text())
metrics='\n'.join(f"| {k} | {v['macro_accuracy']:.4f} | {v['macro_brier']:.4f} |" for k,v in e['methods'].items())
common='\n\nScope: local preparation only, 2026-10-09. No remote database operations, live accounts, player collection or changes to Android, Unity or React. Stage05B original-user-JWT → PostgREST validation → native RLS remains the supported user-data path. No service-role/admin credentials in this package.\n'
docs={
'Mission_Witness_Trust_Design.md':'''# Mission witness trust design

Status: DESIGN plus offline RSA verification prototype; authoritative Unity producer, durable ledger and attestation writer NOT implemented. Existing client telemetry remains non-authoritative.

Android obtains Supabase session; FastAPI validates issuer, audience, signature, expiration and subject using existing security boundary. Server creates a short-lived launch challenge binding canonical player ID, verified Auth session ID, mission ID, launch UUID, nonce, scenario hash and rules version. Client-supplied player IDs are never authorization evidence. Forward original user JWT to PostgREST for all user-scoped records.

Recommended producer: separate trusted server simulation/witness service observes authoritative equipment/resource/mission transitions and computes scores itself. Unity sends bounded action proposals; a client result or action transcript cannot prove real human performance. Server adjudication validates virtual state only. Hosting latency/cost, movement validation, clock rules and simulator equivalence must be designed before attestation deployment. A hacked client can automate valid actions, lie about sensors or steal its own bearer token. Client-held signatures, obfuscation and device attestation do not eliminate these risks.

Record v07b.1 binds result_id/player_id/auth_session_id/mission_id/launch_id/nonce/scenario_sha256/telemetry_sha256/rules/issued_at/expires_at/evidence_kind/five server scores plus issuer/audience/key_id. Canonical sorted compact JSON and detached RSA-2048 signature; trusted public-key registry; maximum five-minute TTL. Prototype requires server_simulation evidence and matching externally verified player/session/challenge. Signed client evidence is rejected. Key rotation/revocation, bounded canonicalization and immutable source hashes required. Production signer keys belong in isolated key management, never APK, Unity, AI worker or source control.

Consumer verifies signature, bounds, identifiers, timestamps, challenge binding and evidence kind before atomic nonce/result consumption and attestation insert. Offline prototype detects replay under a lock in one process only. Restart-resistant replay prevention needs a durable UNIQUE(result_id) and UNIQUE(player,launch/nonce) ledger in the same transaction as result insertion; no schema changes approved here. Test concurrent and crash-after-consume cases before production. Hashes bind bytes, not truth. Verified producer is the authority.

Separate witness writer must only insert approved attestations through a bounded interface; AI user runtime cannot mint evidence or obtain signer/admin credentials. A compromised runtime with stolen valid user JWT can act as that user within token lifetime; credentials cannot authorize a different user by changing PostgreSQL identity settings. No runtime-controlled database identity is introduced.

Tests: signature tampering, identity/session mismatch, expiration, wrong evidence/key/digest, replay. Missing: actual Unity producer, authenticated launch issuance, trusted observer, durable transaction, signer operational isolation and hosted regression. These block authoritative promotion.''',
'Player_Consent_Privacy_Design.md':'''# Player consent and privacy

Status: offline policy prototype, not deployed consent UI or compliance certification. Default collection OFF; model-training consent separate and OFF. Current preparation policy denies collection/training for unknown ages and all minors, even a asserted guardian flag; no real collection occurs.

Proposed explicit notice: exact event categories, learning purpose, storage/retention, withdrawal, account access/deletion, external processors and optional research/model use. Refusal must permit non-personalized practice. Do not bundle telemetry permission with authentication or marketing. Versioned adult opt-in plus revocation, server enforcement and audit receipts required before activation. Determine jurisdiction/age assurance and verified guardian process with legal/privacy review before enabling younger players. Do not collect exact birth date unnecessarily or use self-declared guardian flags as proof.

Minimize to scoped mission/session identifiers, bounded measurements, event offsets, equipment outcomes and validated provenance. Exclude email, name, location, voice, chat, advertising/device IDs, passwords and tokens from offline datasets. Hashing a real player ID is pseudonymization, not anonymization. Reidentification/linkage risk remains. Synthetic IDs here represent no person.

Proposed retention: raw telemetry 14 days, derived progress 90 days, backup expiry within 30 days; these are product-policy proposals, not deployed jobs. Separate consent/operational security records need an approved retention basis. Account export and deletion require verified JWT subject and RLS; never arbitrary target ID. Prototype owner filtering on in-memory rows is not a live endpoint or RLS validation. Deletion must cover raw records, derived features, exports, retrieval indexes, caches and future dataset versions; tombstones prevent restoration from backups. Models require lineage and retraining/unlearning policy; cannot claim a row delete removes learned influence.

Training gate rejects real/client-only data in this milestone. Future authorized real data requires current purpose-specific consent, retention checks, trusted labels, review of subgroup risks and audit trail. Only synthetic rows bypass real-person consent checks.

References: [FTC COPPA guidance](https://www.ftc.gov/business-guidance/resources/complying-coppa-frequently-asked-questions) (updated Rule and applicability require legal review; do not rely on older FAQ alone); [UNICEF AI and children](https://www.unicef.org/innocenti/reports/policy-guidance-ai-children). No legal compliance claim.''',
'NASA_ESA_Dataset_Catalog.md':'''# Scientific dataset catalogue

Scientific sources are environmental reference data, never player skill labels. No blanket license, NASA endorsement or ESA Creative Commons status is assumed.

| Source/product | Intended fields | Rights/provenance and limitations | Status |
|---|---|---|---|
| [PDS InSight atmosphere retrieval](https://atmos.nmsu.edu/data_and_services/atmospheres_data/INSIGHT/retrieving_insight.html), TWINS calibrated collection example urn:nasa:pds:insight_twins:data_calibrated:twins_calib_0004 | Air temperature/wind, original time/quality flags | Product label/version/DOI and units must be checked; InSight landing site is not Jezero; sample intervals/missing values and instrument uncertainty required | No weather observation downloaded; adapter fixture only |
| [InSight archive](https://pds-geosciences.wustl.edu/insight/), [bundle catalogue](https://an.rsl.wustl.edu/help/Content/About%20the%20data/Data%20sets/INS/InSight%20data%20bundles.htm) | PS pressure (urn:nasa:pds:insight_ps); RAD surface temperature | Air and surface temperatures are different measurements; no merging without time/site/quality alignment | Product-level rights/uncertainty pending |
| [NASA Mars Trek](https://trek.nasa.gov/), [API](https://trek.nasa.gov/tiles/apidoc/trekAPI.html?body=mars) | Layer references, coordinates, terrain profiles | Tiles are visualization products, not automatically DEM samples; record source layer/product, datum, longitude direction and resolution | Catalogue only, no download |
| [HiRISE DTM documentation](https://www.uahirise.org/dtm/about.php), [FAQ](https://www.uahirise.org/faq/) | Stereo DTM metadata, map scale, projection, elevation units and uncertainty | Typical documented resolution cannot replace per-product uncertainty. Stereo image labels are not DTMs. Preserve instrument/platform credits and product rights review | Existing 7,729-byte PDS3 image label reused, quarantined |
| [ESA PSA](https://www.cosmos.esa.int/web/psa), [archive](https://archives.esac.esa.int/psa/) | Mars Express/HRSC or other released planetary product metadata | Release/proprietary state, product credits and license must be individually established; public availability alone does not authorize training | COSMOS access restricted during research; rights unverified; nothing downloaded |

HiRISE sample URL: https://hirise-pds.lpl.arizona.edu/PDS/RDR/PSP/ORB_010500_010599/PSP_010502_2090/PSP_010502_2090_RED.LBL . Product PSP_010502_2090_RED v2.0, MRO-M-HIRISE-3-RDR-V1.1; Uranius Fossae, not Jezero. Approved Stage07A metadata-only use; no new downloads in Stage07B. Its checksum and original retrieval timestamp are retained in Evidence/hirise_ingestion.json.

[PDS search guide](https://nasa-pds.github.io/pds-api/guides/search/quickstart.html) and [PDS policy](https://pds.nasa.gov/datastandards/documents/policy/) support catalogue discovery and archive review. Record original label, collection/version, checksum, retrieval timestamp, instrument, measurement type, SI conversion, coordinates, uncertainty, quality flags, missing sentinels and limitations before admitting any scientific observations.''',
'Dataset_Ingestion_Report.md':'''# Dataset ingestion report

PASS: bounded offline ingestion implemented; HTTPS source-origin allowlist, exact provenance fields, timezone-aware dates, SHA-256 checks, no symlinks, duplicate-key/nonfinite JSON rejection, explicit SI units/site/quality and missing-value handling. PDS3 label subset and PDS4 delimited weather label/table subset supported. XML DTD/entities and path traversal rejected. No raster decoder or complete PDS importer claimed.

Actual external artifact: existing approved HiRISE label, SHA-256 726e8c59a4379c40512408bc8a9681ba04bd4235d139b953487ed091e15da654. Parsing succeeded; QUARANTINED because rights, uncertainty, training approval are unresolved and it is image metadata only. See Evidence/hirise_ingestion.json. No NASA/ESA observed weather row entered training. PDS4 fixtures are synthetic parser tests; product-specific units/column mappings still require operator review. JSON caller metadata is not independent scientific validation.

Admission requires a trusted approval-registry entry bound to exact dataset ID and hash, reviewed product-license evidence, permitted use and uncertainty. Even eligible weather is environmental calibration only, never human-skill evidence. Defaults deny admission. Unknown metadata is retained as unknown; no uncertainty or coordinate transform invented. Weather guards 100–350 K and 0–2000 Pa detect gross errors only, not a calibrated Jezero distribution.

Synthetic benchmark: 800 generated rows, checksummed manifest, 600/100/100 splits; no real users or observed science. Nine deterministic scenarios (three seeds × three difficulties) reuse Stage07A simulator. Temperature is generated −85…−20°C minus difficulty offset; dust index, visibility=300(1−dust)^2 and solar fraction=max(.1,1−.85dust) are synthetic game assumptions. Rover limits/slopes/failure probability are designer rules, not NASA coefficients or mission forecasts. Bounds are unit/plausibility guards; no coupled physical dynamics or measured calibration claimed. [NASA Mars facts](https://science.nasa.gov/mars/facts/) and [solar dust observations](https://science.nasa.gov/photojournal/solar-panel-dust-accumulation-and-cleanings/) support qualitative context only.

BLOCKED: weather/DEM acquisition approval and native product review, ESA license/access evidence, actual science normalization, uncertainty propagation, regional/seasonal applicability and full ingestion formats. No additional dependency install or network acquisition needed for local pipeline tests.''',
'Training_Dataset_Schema.md':'''# Offline training schema

Version 07b.1, canonical JSONL with SHA-256 manifest. Exact row keys: schema, row_id, player_group, mission_group, day, split, evidence, language, difficulty, features, labels, label_source, seed. Manifest binds generator and pre-benchmark protocol hashes, seed, permitted local synthetic use and explicit no-real/no-science flags. Never ingest arbitrary client final scores as targets.

Five labels in order: navigation, survival, decision-making, equipment handling, emergency response. Labels are binary simulated next-task success draws from independent latent generator parameters; they are not validated competency or rule-score teacher truth. Six features are five actual Stage07A score proxies and severity index, all finite [0,1]. Difficulty progression is represented by severity/difficulty and longitudinal day; no deployment of learned difficulty policy. Existing gradual difficulty rules remain authoritative.

Player groups train sim-0000…0059/day1…10; validation sim-0060…0079/day11…15; test sim-0080…0099/day16…20. Mission IDs unique per generated task. Group overlap and time reversal rejected. Means/scales fit train only; calibration uses validation only. IDs/day/seed/language excluded from model inputs. Assignment of en/ta is synthetic parity, not a real language subgroup.

Future real row extension must include pseudonymous verified subject linkage held separately, session/witness IDs, scenario/rules versions, trusted outcome origin, consent version/purpose, retention deadline, event-feature transform version and dataset lineage. Requires review and new schema; unsupported evidence rejected now. Account deletion must invalidate downstream partitions and retraining lineage. Scientific observed data remains separately scoped with its own units/uncertainty/provenance, never mixed into human labels.''',
'Baseline_Model_Evaluation.md':f'''# Baseline model evaluation

A local five-head logistic regression was actually fitted on 600 synthetic rows after the recorded protocol/provenance gate passed. Validation:100 rows; held-out test:100 rows/20 disjoint synthetic players. Rules vs model comparison uses actual Stage07A score proxies, not an invented replacement baseline. No trained model is integrated into FastAPI or product inference.

| Method | Macro test accuracy | Macro Brier |
|---|---:|---:|
{metrics}

Full per-skill accuracy/Brier/log-loss/ECE, synthetic difficulty/language counts and validation temperatures: Evidence/baseline_evaluation.json. Weights/normalization: Evidence/baseline_model.json. Protocol: Evaluation_Protocol.md; data/manifest: Datasets/. Run command: PYTHONDONTWRITEBYTECODE=1 PYTHONPATH=outputs/Stage07B/Development work/stage05b/venv/bin/python outputs/Stage07B/Development/run_local.py . The package relies on the existing Stage07A local assessor and existing Python environment; no new libraries installed.

Gate checks source/protocol hashes in runner, canonical data hash, synthetic authorization, exact schema, finite bounds, duplicate IDs, class presence, player-disjoint splits and strictly ordered time windows. Numerical unit tests previously exercised three optimization iterations on generated fixtures; they are separate from this benchmark. Hyperparameters fixed before benchmark, temperatures selected on validation only, no test-driven tuning. Plain JSON serialization avoids executable-model formats.

Results establish pipeline operation only. Synthetic latent structure and label choices determine outcomes; accuracy is not real astronaut performance. Language parity is not human fairness. Small subgroup counts and repeated rows per player reduce effective sample size; confidence intervals, independent cohorts, genuine outcome labels, consent and drift monitoring are unavailable. No deployment acceptance threshold claimed. Scientific observations are not used to train this skill model.''',
'ARIA_Grounding_Evaluation.md':'''# ARIA grounding evaluation

Implemented bounded offline curated retrieval templates for two NASA-supported topics, dust/atmosphere and solar-panel dust, in English/Tamil. Unknown scientific measurements, player outcomes and instruction/token requests abstain. No LLM or trained language model involved.

12 fixed cases: four supported responses match their expected source URLs; eight unsupported requests abstain; zero unsupported non-abstaining outputs. These are template/URL correspondence checks, not independently measured factual accuracy or semantic hallucination rates. See Evidence/aria_evaluation.json for individual outputs and expectations. Test gold expectations come from the same curated cards; independent adversarial corpus is still required.

References: [NASA Mars facts](https://science.nasa.gov/mars/facts/) and [NASA solar-panel dust](https://science.nasa.gov/photojournal/solar-panel-dust-accumulation-and-cleanings/). Authoritative state must always come from deterministic rules/verified witness, never language feedback. Missing evidence leads to abstention, not fabricated weather or mission outcomes.

BLOCKED: domain expert factual adjudication, native Tamil fluency and English–Tamil scientific parity review, paraphrase/prompt injection corpus, citation passage entailment, source-age checks and real-user comprehension. Proposed rubric: blinded reviewers score factual support, exact citation entailment, unsupported claims, appropriate abstention, terminology/units and readable Tamil/English; report disagreements and denominators. Translation text here is a draft, not certified scientific localization.''',
'Stage07B_Local_Test_Results.md':'''# Stage07B local test results

PASS: 43 pytest checks; Evidence/unit-tests.log. Tests cover manifest/source/hash/units/missing data, XML entity and traversal handling, consent denial/revocation/purpose/retention/owner filtering, finite synthetic schema, player/time leakage rejection, repeatable generation, train-only normalization, bounded evaluation, unsupported retrieval, witness signature/identity/TTL/evidence/replay rejection. Unit tests do not constitute live Auth/RLS tests.

PASS: training provenance gate for 800 synthetic rows, 600 train/100 validation/100 test, disjoint players and temporal windows; Evidence/training_gate.json. Actual local logistic fit and held-out synthetic comparison completed; benchmark-run.log, baseline_model.json, baseline_evaluation.json. PASS: existing HiRISE checksum/parser validation; artifact remained QUARANTINED. PASS: four curated citation and eight abstention cases; not independent semantic factuality validation.

Preserved Stage05B architecture; no API, DB grants, administrative path, Android, Unity or React edits. No remote SQL, no real accounts and no telemetry collection. This package is offline and opens no sockets; source review finds no credential loading. No new database/RLS isolation claim: existing Stage07A results are historical regression evidence, not rerun hosted checks.

BLOCKED: authoritative Unity producer/launch integration/durable replay storage, independent signer operational isolation, deployed consent/access/deletion/guardian workflow, weather/terrain licensing and measured calibration, real-player model validity, expert/native Tamil evaluation, hosted Supabase and runtime deployment tests. Client signatures do not eliminate client cheating; stolen valid JWTs remain a bounded authorization risk. No model deployed.

Reproduction: pytest command in Evidence/unit-tests.log context; run Development/run_local.py after reading Evaluation_Protocol.md. Generated outputs contain synthetic IDs only. Initial collection error (Path import order) corrected before final 43-pass run. Stop after local evaluation; future implementation/remote execution needs a separate instruction.'''
}
for name,body in docs.items():(root/name).write_text(body+common)
print('Created eight reports; quarantined existing metadata; no remote operations')
