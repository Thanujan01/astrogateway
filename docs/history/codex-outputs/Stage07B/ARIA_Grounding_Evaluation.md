# ARIA grounding evaluation

Implemented bounded offline curated retrieval templates for two NASA-supported topics, dust/atmosphere and solar-panel dust, in English/Tamil. Unknown scientific measurements, player outcomes and instruction/token requests abstain. No LLM or trained language model involved.

12 fixed cases: four supported responses match their expected source URLs; eight unsupported requests abstain; zero unsupported non-abstaining outputs. These are template/URL correspondence checks, not independently measured factual accuracy or semantic hallucination rates. See Evidence/aria_evaluation.json for individual outputs and expectations. Test gold expectations come from the same curated cards; independent adversarial corpus is still required.

References: [NASA Mars facts](https://science.nasa.gov/mars/facts/) and [NASA solar-panel dust](https://science.nasa.gov/photojournal/solar-panel-dust-accumulation-and-cleanings/). Authoritative state must always come from deterministic rules/verified witness, never language feedback. Missing evidence leads to abstention, not fabricated weather or mission outcomes.

BLOCKED: domain expert factual adjudication, native Tamil fluency and English–Tamil scientific parity review, paraphrase/prompt injection corpus, citation passage entailment, source-age checks and real-user comprehension. Proposed rubric: blinded reviewers score factual support, exact citation entailment, unsupported claims, appropriate abstention, terminology/units and readable Tamil/English; report disagreements and denominators. Translation text here is a draft, not certified scientific localization.

Scope: local preparation only, 2026-10-09. No remote database operations, live accounts, player collection or changes to Android, Unity or React. Stage05B original-user-JWT → PostgREST validation → native RLS remains the supported user-data path. No service-role/admin credentials in this package.
