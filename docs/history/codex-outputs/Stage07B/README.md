# Stage07B local evaluation package

Completed offline prototypes and synthetic-only evaluation; no product integration or remote modifications.

## Deliverables

- [Mission witness trust](Mission_Witness_Trust_Design.md)
- [Consent and privacy](Player_Consent_Privacy_Design.md)
- [Scientific catalogue](NASA_ESA_Dataset_Catalog.md)
- [Ingestion evidence](Dataset_Ingestion_Report.md)
- [Training schema](Training_Dataset_Schema.md)
- [Baseline evaluation](Baseline_Model_Evaluation.md)
- [ARIA evaluation](ARIA_Grounding_Evaluation.md)
- [Local test results and blockers](Stage07B_Local_Test_Results.md)

151 pytest tests passed (48 new,103 backend regression); five isolation probes and four reproducibility comparisons reported separately. Protocol, synthetic JSONL, weights and per-case evidence included. Synthetic results do not establish real-player performance. Scientific acquisition/rights/calibration, authoritative witness and deployed consent remain blocked. English/Tamil domain and native-language review pending.

Use the existing local Python venv and Stage07A backend dependency. For stronger isolation use Development/offline.sb with `/usr/bin/env -i` and absolute executable paths, as recorded in local test report. Profile paths are specific to this Mac; review and regenerate for another machine. Sandbox denies all network and credential-file reads; permits evidence/dataset writes only. Low-level numerical functions are test helpers; run_local.py is the provenance-gated benchmark entry point. Do not expose these helpers as authenticated product endpoints.
