# URP scene validation

PASS: isolated project resolved Unity 6000.6.4f1 + URP17.6.0 + official glTFast6.20.0; editor builder compiled with deprecated FindObjectsByType overload warnings (no C# compile errors observed). Rock importer diagnostics empty; approved ground maps/material imported.

FAIL: rover GameObject missing following unsupported required WebP extension; builder aborted with exit1. **Assets/Evaluation/Stage08D1_Visual_Evaluation.unity was not created.** Ground.mat and an unsaved partial setup are not a completed scene.

Prepared local builder: Assets/Editor/Stage08D1Builder.cs. Proposed 60m material test surface, genuine small stones, robotic rover, one directional afternoon light, inexpensive exponential-squared fog, restrained saturation/contrast and Neutral tonemapping. No weather particles or astronaut. None of these final composition/lighting/rendering checks passed because build stopped before scene save and rendering.

BLOCKED: missing-material/shader rendering checks, mesh transform and skin validation, final LODGroups, actual camera captures and scene reload. UI screenshot attempt reached the original project only, then Mac lock prevented selecting/inspecting the evaluation window. Original project was not used as evaluation proof.

Preservation: original playable scene, MarsPrototype.cs and manifest.json match prior recorded SHA256 hashes. All acquired source-file hashes unchanged. No Android/React/backend/Supabase modifications.
