# Explainable skill assessment
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
