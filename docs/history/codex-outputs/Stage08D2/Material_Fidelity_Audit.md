# Stage08D.2 material fidelity audit

## Result: partial repair; full transmission fidelity BLOCKED

All eleven Stage08D11 captures were inspected: Composition, Rock_Ground, Rover_LOD0, Rover_LOD1, Rover_LOD2, Rover_Original, Rover_Rear, rock_07_LOD0, rock_07_Original, rock_09_LOD0 and rock_09_Original. These are actual Unity Editor camera readbacks, not native Editor-window screenshots or physical-device evidence.

The original lower rover LODs show damaged wheel treads, incomplete silhouettes and dark triangular housing artifacts. LOD0 retains substantially better shape. Rock07 has a glossy appearance; rock/ground fidelity is outside this rover repair. This remains an isolated composition, not a finished Mars world.

Seven of the 43 optimized rover materials use optional KHR_materials_ior / KHR_materials_specular properties not handled by the existing importer:

| Material | IOR | Metallic | Roughness | Additional issue |
|---|---:|---:|---:|---|
| transparent | 1.45 | 0 | 0.5 | Specular factor 0; source is OPAQUE |
| glass_trans | 1.39 | 0 | 0.005 | Tinted specular texture; transmission 1 |
| glass lens | 1.39 | 0 | 0.005 | Tinted specular factor; transmission 1 |
| blades | 1.45 | 0 | 1 | Specular factor 0 |
| mars_2020_06 | 1.45 | 0.25 | 0.5 | Specular factor 0.3090909 |
| glass_2 | 1.39 | 0 | 0.2727273 | Tinted specular texture; transmission 1 |
| mars_2020_06_n | 1.45 | 0.45 | 0.25 | Specular factor 0.3090909; normal scale 1.5 |

All seven are double-sided and have glTF alphaMode OPAQUE. A material name containing “transparent” is not evidence of an alpha-blended source. Complete source material JSON and texture references are recorded in Material_Adapter_Evidence.json.

The three transmission materials remain opaque in the Lit approximation. Refraction, transmitted background and optical path length are not reproduced. Full physical glass fidelity is therefore unresolved; no blanket acceptance PASS is declared.
