# Asset reuse and license record

Reused existing MarsPrototype controller, native input/boarding/telemetry/scan/save components, original character/rover roots and walking/idle animation controller. Original scenes and shared assets are unchanged.

Previously approved Poly Haven sandy_gravel, rock_07 and rock_09 are reused. [Poly Haven license](https://polyhaven.com/license) identifies its asset library as CC0, permits commercial use/redistribution and makes attribution optional. Courtesy credit: Poly Haven. Logos, website text and preview renders are not included as assets. Only previously acquired files were used; no new models/textures downloaded.

Exact original download URLs and acquisition hashes are in work/stage08d/Acquired/Acquisition_Manifest.json. Approved_Source_Hash_Checks.json verifies 27 acquired source files remain unchanged. Native rock meshes were newly exported from the already approved LOD0/1/2 GLBs in the isolated evaluation project; transforms were combined and uniformly normalized to a 1m largest dimension, original UV/normal data retained. No glTFast dependency added to the main project.

Native URP Lit materials use copied diffuse, OpenGL normal and AO maps, 1024 import limits and Android ASTC 6×6 overrides. AO is linear. Roughness uses a constant smoothness approximation; copied source roughness images remain available but are not faithfully reconstructed into the current materials. These are terrestrial visual proxies, not Mars measurement datasets.

New original assets: synthetic terrain mesh, geodesic-inspired hemisphere, batched low-poly seams, closed airlock/connector geometry, tanks/cabinets/mast/dish, solar modules, additive rover details and boots, custom URP sky gradient, lighting/quality components and five research markers. No distinctive third-party logos or habitat image copied. Habitat design is fictional/unvalidated near-future architecture.

Created prefabs:
- Prefabs/ARESYN_ClosedHabitatComplex_Optimized.prefab
- Prefabs/SolarFarm_Optimized.prefab

New scripts:
- Scripts/EnvironmentPresentation.cs
- Scripts/EnvironmentPreset.cs
- Scripts/ResearchTarget.cs
- Tests/Editor/MarsResearchBaseBuilder.cs

Lighting contains four preset assets, sky material/shader and restrained volume profile. Models contain terrain/dome/seam meshes plus six native approved rock LOD derivatives. Materials contain native URP derivatives and Textures copied approved source maps. New_Files.json lists exact added paths.

NASA references are art/science context, not habitat certification, source terrain heights or endorsement: [Mars facts](https://science.nasa.gov/mars/facts/) supports dusty desert/thin-atmosphere context; [Jezero delta panorama](https://science.nasa.gov/photojournal/detailed-panorama-of-mars-jezero-crater-delta/) supports rocky layered visual reference. No NASA Perseverance model imported into this gameplay scene; its separate material/LOD fidelity issues were not declared resolved.
