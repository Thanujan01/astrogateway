from pathlib import Path
import sys,json,hashlib,shutil,subprocess
R=Path.cwd();O=R/'outputs/Stage07A';D=O/'Development';sys.path.insert(0,str(D/'backend'))
from adaptive.engine import plan,SKILLS
for level,n in [('beginner',0),('intermediate',3),('advanced',12)]:
 hist=[{'attested':True,'success':True,'aggregate':90,'difficulty':'Easy' if i<3 else 'Medium' if i<6 else 'Hard'} for i in range(n)]
 skills={s:{'score':None if n==0 else 90,'confidence':0 if n==0 else .9} for s in SKILLS}
 (O/f'{level}_example_plan.json').write_text(json.dumps({'fixture':'synthetic example, not a player program or outcome',**plan(skills,42,0,hist)},indent=2))
meta=json.loads((O/'Samples/provenance.json').read_text());label=json.loads((O/'Evidence/sample-validation.json').read_text())['metadata']
(O/'Adaptive_AI_Architecture.md').write_text(f'''# Stage07A — Adaptive AI architecture
2026-10-09. **Local foundation implemented. No learned model, remote SQL, production deployment or real player collection.**

## Project inspection and isolation
Android: Kotlin native UI, Supabase Auth/PKCE callback, Keystore session storage, original-user FastAPI bearer, private Unity activity/bridge; Unity receives mission/launch/user/session IDs and lease, not credentials. Unity production project is 6000.6.4f1/URP and approved Mars scenes remain unchanged. React is Vite/React with Three/Rapier; no web source changed. Supabase is sole identity provider; Stage05B3 hosted compatibility, callbacks and full-path TLS remain blocked/unverified.

Neither original Android project nor this workspace has a Git repository. Safe backend-only snapshot created at `{D}`: baseline-stage05b3 commit followed by `stage07/adaptive-ai-foundation` branch. It contains no .env/Android config/admin files. Actual original backend/UI/Unity/React were not modified. This avoids initializing a repository over private project files. Stage05B3 reports were read; unsandboxed operator-account service is still prohibited.

## Implemented pipeline
Verified Supabase identity → existing training-session revocation RPC → original JWT forwarded to fixed AI PostgREST RPCs → private training_ai RLS → validated/consistent telemetry → deterministic explainable skills → bounded adaptation → seeded scenario + personalized 30-day plan → source-card/template ARIA debrief. No direct PG connection or service-role credential in AI code. PostgreSQL passwords/signing keys remain operator-side outside confined runtime. No in-memory/SQL fallback replaces RLS.

Five endpoints are mounted on the copied existing FastAPI application; original v1 routes and authentication dependencies remain intact. New Python modules: adaptive/schemas.py, engine.py, repository.py, api.py. Existing main.py changes only mount the AI router. Existing requirements unchanged. Absent AI RPCs fail503, never create data or substitute a demo store. In this stage the candidate SQL was executed ONLY in the disposable localhost PostgreSQL17.11/PostgREST16.4 lab. Original backend is not silently replaced or live-enabled.

## Trust and data authority
Client measurements have a strict schema and consistency checks but are still **client_reported/provisional**. Movement/route/resources/reactions are not proven honest by validation. Client final scores, owner IDs, trust/attestation flags and unknown fields are rejected. Uploaded completions cannot credit trusted days or unlock Medium/Hard. Rules derive provisional coaching estimates; private server-attestation records independently govern progression.

Private attestation table has no runtime/client INSERT/UPDATE/DELETE grant or exposed privileged writer. Only the trusted operator inserted synthetic fixtures during local tests. A real authoritative mission-witness/validation producer is **not implemented** because existing Unity gameplay/telemetry instrumentation has not been approved. Before live promotion, implement separately isolated semantic evidence validation; do not give FastAPI an admin key to populate it. Valid user-token theft/replay and compromised response manipulation remain residual risks. No claim of absolute backend-compromise immunity.

## SQL/permission contract
`{D/'database/007a_local_adaptive_candidate.sql'}` depends on exact Stage05B2 training schema/owner/native Auth helpers. Creates private events + attestations, strict per-kind event validator, two exposed **invoker** RPCs ai_ingest/ai_snapshot with empty search_path. RLS binds user/session/lease; backend supplies no owner/SID identity override. Native authenticated grants: SELECT own rows and bounded event columns INSERT; no attestation mutation, telemetry edits/deletes/DDL or owner membership. training_ai stays outside exposed Data API schemas; only existing mission_api gains two routines. Default/exposed-function inventory must be re-reviewed before remote approval. Candidate is fresh-install/non-idempotent: collisions abort. Local rollback/reinstall was tested; no remote rollback is authorized.

## Runtime and limits
Actual AI app tested inside inherited macOS kernel sandbox, staged immutable code, cleared env, scratch writes only, localhost gateway outbound only. Same UID501 with enforceable confinement, not a new permanent account. Operator regression harness administers/signs only private local fixtures and passes USER fixture tokens over stdin; the runtime cannot read those keys/configs.18 isolation probes repeated. Test Auth is a public RSA fixture; data gateway is real local PostgREST. No live Supabase claims/JWKS/Android gameplay validation.

Bounds: 128 events/batch, POST AI body131072 bytes, <=2048 events/snapshot, <=100 missions/snapshot. Events immutable, whole replay idempotent, partial/changed replay conflicts, completion closes stream. Local foundation has no production pagination/retention/rate limiting or collection consent flow; these are deployment gates.30-day cycles are deterministic templates with witness-backed distinct day credits/retries; they are not an astronaut certification.
''')
(O/'Player_Telemetry_Schema.md').write_text(f'''# Player telemetry schema
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
''')
(O/'Skill_Assessment_Design.md').write_text('''# Explainable skill assessment
Deterministic rule07a.1, no trained model. Scalar scores0…100 are game coaching indices, not validated astronaut competence. All coefficients are synthetic design assumptions.

| Skill | Per-mission score rule |
|---|---|
| Navigation | mean clamp(100−10×nearest-route deviation_m−20×max(0,speed_mps−2.5)) |
| Survival | last cumulative use: clamp(100−100×max(oxygen/600,water/4,power/1800)); budgets are synthetic units/mission design, not medical requirements |
| Decision-making | mean clamp(100−20×max(0,reaction_s−1)); reaction is a limited proxy, not holistic judgment |
| Equipment handling | mean100 for reported correct,0 otherwise |
| Emergency response | unresolved0; resolved clamp(100−2×max(0,response_s−10)) |

Mistakes subtract5×(1+retries) from associated skill. Missing category gives score=null. Per-mission engine emits observation counts/penalty; API combines per-mission category means (not raw oversampled movements) to reduce sampling-frequency domination. At least3 mission observations for assessed/provisional status; otherwise insufficient_data. Confidence=min(.9,n/10), multiplied by.25 for client-only evidence, maximum.225; heuristic evidence completeness, NOT statistically calibrated probability. Private trusted score records supersede client estimates for matching categories. UI must expose basis/status and avoid comparing raw provisional confidence as real skill certainty.

Adaptive recommendation replays trusted chronological history from Easy; the client's requested previous value is ignored. Three successful attested missions at the CURRENT tier, aggregate>=75, unlock only one tier. Medium requires its own three qualifying results before Hard. Two recent attested failures reduce one tier; otherwise hold. Untrusted completion never advances difficulty. All tests use synthetic private attestations; no real witnessing pipeline exists.

Daily plan emphasizes the two weakest/unknown categories twice per seven-day rotation; other categories once. Recovery/review occurs every7th day and first2 days after a reported/attested failure. Reported failure can conservatively add review, but cannot grant higher difficulty or trusted credits. Future real calibration must assess assistive-control bias, device latency, uncertainty, sample dependence and game exploit resistance with approved representative users; current heuristics are not certification or clinical inference.
''')
(O/'Mars_Weather_Incident_Simulator.md').write_text('''# Mars conditions and incident simulator
Rule07a.1 uses isolated random.Random(uint32 seed); same seed/difficulty/rule/runtime gives identical JSON. Store seed+rule version and pin runtime for cross-release replay. No global RNG, current clock, external weather feed or trained forecast model. No mission state controlled by ARIA language output.

**Observed/general science:** Mars is cold with a thin atmosphere; suspended dust affects its appearance; NASA describes broad planetary temperature extremes around−153…20°C. This is not a Jezero seasonal series or a scenario probability distribution. [NASA Mars facts](https://science.nasa.gov/mars/facts/).

Dust accumulating on solar arrays reduces incoming sunlight; missions have observed cleaning events. [NASA rover solar-panel dust](https://science.nasa.gov/photojournal/solar-panel-dust-accumulation-and-cleanings/). No numerical simulator coefficient is inferred from that observation.

**Synthetic game parameters:** difficulty index d=0/1/2; temperature uniform(−85,−20)−3d°C; dust index uniform(.05,.35)+.18d; visibility=300(1−dust)^2m; solar fraction=max(.1,1−.85dust); slope uniform(0,8)+4d degrees; rover limit=1.5(1−.4dust)/(1+.2d)m/s; route risk=min(1,.5dust+.15d). Dust index is a dimensionless game knob, not measured optical depth. Rover limit is a game rule, not rover operational speed data. No Mars pressure/radiation/EVA life-support physics is claimed.

Incident probability=.15+.15d, then uniform selection from none/dust/power_loss/rover_fault/oxygen_leak. Thus actual non-none incident probability=.8×that knob. These are fictional frequencies; no NASA failure rates or reliability models. Hazard severity/duration and equipment recovery rules require later mission implementation. Current output is a scenario descriptor only; Unity physics/VFX/environment remain unchanged.

Use fixed seeds0,1,42,uint32 max across all tiers; tests cover repeatability and range. Examples embedded in 30-day JSONs. Approved PDS sample is an image METADATA label from Uranius Fossae, not a weather observation or terrain DEM; it is never used to tune these parameters. Future calibrated use requires matching location, solar longitude, local time, processing level, units and uncertainty; orbital atmospheric temperature is not interchangeable with surface temperature.
''')
(O/'Open_Data_Source_Catalog.md').write_text(f'''# Open planetary-data source catalog
Research accessed2026-10-09. Source availability is not proof of universal license, model-training rights, scientific suitability or endorsement. No bulk imagery/DEM/ESA download.

| Resource / official source | Products and access | Provenance / limitations | Rights status |
|---|---|---|---|
| [NASA PDS](https://pds.nasa.gov/), [Search API](https://nasa-pds.github.io/pds-api/guides/search/quickstart.html) | PDS3/PDS4 labels, instrument/calibrated products; JSON/XML registry metadata | LID/LIDVID or dataset/product/version, mission/instrument, observation date and processing level; registry not complete; API sample request503 | [PDS policies](https://pds.nasa.gov/datastandards/documents/policy/) and individual product terms must be checked; [PDS/PPI FAQ](https://pds-ppi.igpp.ucla.edu/faq.jsp) describes archive public-domain data, not a blanket third-party asset endorsement |
| [NASA Mars Trek](https://trek.nasa.gov/mars/), [official tile API](https://trek.nasa.gov/tiles/apidoc/trekAPI.html?body=mars) | layered imagery/topography, tiles and portal products; no layer downloaded | Record source layer ID/native upstream instrument, projection/resolution/extent and processing; rendered tiles may not be raw elevation; coarse global MOLA cannot resolve250m-site obstacles | Portal does not establish every layer's license; upstream product and credit review pending |
| [HiRISE DTM catalog](https://www.uahirise.org/dtm/), [DTM explanation](https://www.uahirise.org/dtm/about.php) | stereo-derived terrain IMG and ortho JP2/labels; large products, browse previews | UA/MRO/PDS; native stereo geometry/vertical datum/NoData; typically1–2m posting from high-resolution images; DTM is not exact sub-meter collider truth everywhere | [HiRISE FAQ](https://www.uahirise.org/faq/) permits download/processing; specific redistribution/training terms require label/catalog review; credit NASA/JPL/University of Arizona as applicable, no endorsement |
| [ESA PSA](https://www.cosmos.esa.int/web/psa), [official archive](https://archives.esac.esa.int/psa/), [PSA tools](https://www.cosmos.esa.int/web/psa/psa-tools) | Mars Express HRSC/OMEGA and ExoMars instrument archives, PDS-compatible labels; portal/programmatic products | [ESA archive-download description](https://esdcnews.esac.esa.int/news/PSA/2018/) confirms download services; COSMOS direct browsing returned451 here; archive document links discoverable | Per-instrument dataset license/credit/proprietary-release status UNVERIFIED; do not invent CC terms or assume website-content license covers data |

## Exactly one approved downloaded sample
Operator explicitly approved one public NASA PDS metadata sample <=32KiB. Initial public API GET limit1 returned503 and saved no sample. Successful fallback label:
- URL: [PDS HiRISE label]({meta['source_url']})
- Local file: `{O/'Samples/hirise_metadata_sample.lbl'}`
- Size: {meta['bytes']} bytes; SHA256 `{meta['sha256']}`
- Product `{label['PRODUCT_ID']}`, version `{label['PRODUCT_VERSION_ID']}`, dataset `{label['DATA_SET_ID']}`, PDS3, targetMARS, instrumentHIRISE, producer UNIVERSITY OF ARIZONA.
- Region label says Uranius Fossae, **not Jezero**. This is a projected-image label, not a DTM/weather dataset. Associated pixels were not downloaded.
- Provenance `{O/'Samples/provenance.json'}`; parser/hash/size/target checks `{O/'Evidence/sample-validation.json'}` all passed. Exact retrieval timestamp retained. License review remains pending at individual-product level; this sample is for metadata validation only. No training/evaluation of a learned model used it.

NASA scientific/agency logos are not an endorsement of this application. [NASA media guidelines](https://www.nasa.gov/nasa-brand-center/images-and-media/) must be reviewed for any later imagery/model-output usage. Do not acquire large products until source/terms/size/location and intended transformation are approved; retain checksums and upstream attribution, mask data gaps, document resampling and derived-product changes.
''')
(O/'Thirty_Day_Training_Plan.md').write_text('''# Personalized 30-day training cycles
Implemented GET/ai/training-plan/me, rules07a.1. Seed=first32 bits SHA256(canonical verified user UUID); daily seed=SHA256(rule:seed:cycle:day). User UUID is not leaked into telemetry or science requests; this seed is reproducibility metadata, not secret authorization. Same inputs yield same plan; changing cycle produces controlled scenario variation.

30 daily slots; the two weakest/unknown categories get extra practice with seven-day rotation [weakest,second,weakest,third,second,fourth,fifth]. Every7th day is recovery/review. Missions target15/20/25 minutes by allowed tier, reviews10; synthetic curriculum targets, not health recommendations. Existing mission-red-horizon-poc is the mission ID; no new Unity gameplay/collector added.

Beginner: insufficient trusted history; Easy. Intermediate: >=3 recent trusted successes aggregate>=75; Medium practice only when adaptive gate permits. Advanced: >=10 trusted missions, recent5 successful, all category scores>=80 and confidence>=.5; difficulty still capped by replayed tier progression. Labels do not authorize skipping three Medium outcomes. Failure schedules first2 review days; retry can credit the same planned day later. No automatic difficulty escalation simply because calendar time passed.

Private server attestations carry cycle/training_day1…30 and success; distinct successful days count once, retries cannot double-credit. Fully completed cycle advances index; days already credited are marked completed. Unassigned missions improve evidence but do not falsely mark a calendar day. Client completions are reported separately as provisional_completed_missions and never grant trusted day completion. API computes state from RLS-protected authoritative records; no arbitrary progress-update endpoint. The real witness producing records is not yet implemented.

Generated examples beginner_example_plan.json, intermediate_example_plan.json, advanced_example_plan.json are **synthetic illustrative profiles**, not real players' outcomes. Recovery, repeats, weakness weighting, level gates and replay tests passed. No daily notifications/scheduler or background collection is enabled; thirty slots are a repeatable curriculum, not a claim that thirty real days were observed.
''')
(O/'Model_Training_Evaluation_Plan.md').write_text('''# Model training and evaluation plan
**No model was trained, downloaded, fine-tuned or benchmarked.** Current system is deterministic rules and a small curated NASA source card for ARIA. Product label “AI foundation” does not imply a learned policy.

ARIA presently retrieves the versioned source card and current validated, provenance-labelled skill summary, fills a fixed feedback template, and states unknown outcomes/weather. It does not invent NASA measurements, link an incident to actual weather, assign final scores or change mission state. No free-form user prompt/internet retrieval/LLM tool execution is exposed. Debrief is scoped to an owned mission and reports insufficient/provisional data honestly. Question enum provides a bounded requested focus; current template chooses weakest category and includes requested_focus, not a full conversational tutor.

Potential later architecture: source allowlist + checksum/version/provenance registry → curated chunk retrieval with source IDs → language explanation constrained to cited observations. Deterministic score/state APIs remain authoritative; generative text is advisory only. Refuse unsupported facts; quote only within licenses; isolate retrieved text as untrusted data, prohibit tool/credential instructions. Never transmit access/refresh tokens or raw identifying telemetry to a model.

Before learned skill/difficulty models: approved collection consent/retention and independent mission evidence; confirmed dataset rights/provenance; unbiased expert labels and reliability; split by player and mission/region/time to prevent leakage; retain unseen players/hardware; compare to fixed rule baseline. Measure calibration/error, failure recovery, latency/device bias, uncertainty, unsafe promotion and cross-user leakage. Pre-register acceptance: no unverified client-score promotion, no unauthenticated/user-mixed rows, deterministic replay where required, and expert-reviewed educational validity. Thresholds need real calibration; fabricated accuracy is forbidden.

Scientific terrain/weather data are not labels for human competence. One image metadata label cannot train navigation or weather inference. Planet-wide temperature extrema cannot serve as Jezero forecasts. Synthetic data useful for software tests only; simulated test accuracy must not be marketed as real-world validation. No training planned until approved ground truth and rights are available.
''')
# Generate strict schema + compact runnable README.
from adaptive.schemas import Batch
(O/'Telemetry_JSON_Schema.json').write_text(json.dumps(Batch.model_json_schema(),indent=2))
(D/'README.md').write_text(f'''# AstroGateway Stage07A local development snapshot
Branch stage07/adaptive-ai-foundation; original projects untouched. No private configuration.
Run backend tests with the existing approved virtualenv and PYTHONPATH pointing to `{D/'backend'}`. Native/API rehearsal requires separately started localhost private fixture lab; runners in outputs/Stage07A/TestHarness are review copies with authoritative work/stage07a paths. Never run database/007a_local_adaptive_candidate.sql against Supabase without a separate security/compatibility review and explicit remote approval. The app requires public provider configuration + original user JWT and approved RPCs; otherwise fails closed. No AI service-role/admin key or raw DB password.
''')
H=O/'TestHarness';H.mkdir(exist_ok=True)
for n in ['run_checks.py','native_checks.py','rollback_checks.py','baseline_final_checks.py']:shutil.copy2(R/'work/stage07a'/n,H/n)
for n in ['ai_checks.py','trusted_checks.py','probe.py']:shutil.copy2(R/'work/stage07a/runtime'/n,H/n)
shutil.copy2(R/'work/stage07a/runtime.sb',H/'runtime.sb')
manifest={str(p.relative_to(D)):hashlib.sha256(p.read_bytes()).hexdigest() for p in D.rglob('*') if p.is_file() and '.git' not in p.parts and '__pycache__' not in p.parts}
(O/'Evidence/source-manifest.json').write_text(json.dumps(manifest,indent=2))
print('Seven design documents, schema and synthetic 30-day examples generated; test report pending cleanup.')
