# LOD artifact repair

## Geometry result: PASS for inspected static views

The previous LOD1/LOD2 simplification visibly damaged Wheels_objs and the front housing. The repair starts from the acceptable Stage08D11 LOD0 PNG derivative, not the already damaged lower LODs. Original files are never overwritten.

Thirty critical meshes preserve exact index and attribute byte signatures in both regenerated lower LODs. Protected parts include wheels, suspension, chassis/body, RTG fins and scientific equipment. Mesh indices and per-mesh hashes/checks appear in LOD_Repair_Evidence.json. Perseverance uses an RTG; no fictional solar array was added. [NASA rover components](https://science.nasa.gov/mission/mars-2020-perseverance/rover-components/) supports its power-system identity.

Other meshes weld identical vertices before border-locked simplification. LOD1 uses requested ratio 0.8/error 0.001; LOD2 ratio 0.6/error 0.003. Error constraints prevented most reduction. These ratios are requested limits, not achieved reduction measurements.

| Level | Prior triangles | Repaired triangles | Meshes | Materials |
|---|---:|---:|---:|---:|
| LOD0 | 91,916 | 91,916 | 68 | 43 |
| LOD1 | 47,280 | 91,902 | 68 | 43 |
| LOD2 | 27,088 | 91,792 | 68 | 43 |

LOD0 is copied unchanged. Selected derivatives are Perseverance_RepairedV2_LOD0/1/2.glb under work/stage08d2/Derived. An earlier conservative attempt is retained for traceability but is not selected in the scene.

Matched front captures show intact treads, housing and chassis, with no severe detached geometry or UV distortion visible. A rear LOD0 view and adapted high-poly source reference were also captured. This does not validate every hidden surface, animated articulation or all possible camera positions. The folded/rest configuration remains as supplied.

This is a fidelity fallback, not an effective mobile geometry optimization: LOD2 is only 124 triangles below LOD0. A future authored reduction with wheel/chassis-specific topology is preferable to loosening automatic error bounds blindly. No physical Android performance improvement is claimed.

The two regenerated GLBs have zero Khronos validator errors; inherited tangent-generation and skinned-transform warnings remain (see machine-readable evidence). Unity imports all three selected levels successfully.
