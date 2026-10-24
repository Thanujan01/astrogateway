# glTF validator results

Khronos glTF Validator 2.0.0-dev.3.10, validateBytes on final output bytes. Complete diagnostics embedded in Conversion_Evidence.json.

| Asset | Errors | Warnings | Required extensions |
|---|---:|---:|---|
| Perseverance_LOD0 PNG | 0 | 36 | none |
| Perseverance_LOD1 PNG | 0 | 36 | none |
| Perseverance_LOD2 PNG | 0 | 35 | none |
| Perseverance_Original PNG | 35 | 36 | none |

All three optimized rover derivatives have zero errors. Existing warning classes include generated tangent space and non-root skinned-mesh nodes; unused data can generate informational diagnostics. These are not proofs of accurate articulation.

The converted original reference has inherited skin-weight normalization errors. It is included to document source limitations, not falsely labeled validator-clean. Conversion left source geometry/weights untouched. It imported and rendered as a static comparison, but mechanical animation acceptance is outside these checks. Acquired Draco original was inventoried only, not validated as a converted output.
