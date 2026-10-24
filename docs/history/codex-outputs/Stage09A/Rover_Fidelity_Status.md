# Rover fidelity gate

BLOCKED: glass_trans, glass lens and glass_2 require glTF transmission=1. The existing URP Lit adapters preserve source OPAQUE semantics but cannot reproduce transmission/refraction. Stage08D2 explicitly withheld full fidelity approval. No acceptance of that approximation or approval of a separate transmission solution is recorded.

Repaired LOD0/1/2 triangle counts: 91,916 / 91,902 / 91,792; 43 materials each. LOD2 saves only 124 triangles (0.135%). Earlier aggressively simplified LODs had unacceptable wheel/housing artifacts and must not replace the repaired versions. Geometry passed prior inspected static views; hidden surfaces/articulation remain incompletely reviewed.

Recommended decision: approve a separately scoped isolated material repair and review before main-project integration. Alternatively explicitly accept the named opaque-glass limitation for a prototype. Neither option has been executed. The copied screenshot is prior actual Stage08D2 evidence, not a Stage09 capture.
