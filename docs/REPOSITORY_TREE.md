# Complete tracked repository tree

[README](../README.md) · [Architecture](ARCHITECTURE.md) · [Setup](SETUP_GUIDE.md)

Generated from the actual Git index plus the team-plan document being added in this update. Every leaf below is an actual tracked/being-committed file; directory nodes are derived solely from those paths. The tree includes itself, all Unity assets and `.meta` files, experimental copies, hidden tracked configuration and historical source. There are no wildcard placeholders or elided children in the complete tree.

The source baseline is `678bf88c3fec3be7dedad343eb3bf775528df744`; the inventory describes this team-plan documentation commit's resulting file set. Local caches, secrets, Git internals and ignored/untracked files are not repository source and are not listed. Counts will change with later commits. The fixed migration manifest describes the earlier import, not this live inventory.

Use GitHub's Raw/download view or a local text editor if the rendered viewer truncates this large inventory. From the clone root, `git ls-files` prints the full repository-relative paths; the hierarchy below is the same complete set arranged as a terminal tree.

## Coverage counts

| Top-level entry | Tracked file count |
|---|---:|
| `.gitattributes` | 1 |
| `.gitignore` | 1 |
| `.gitleaks.toml` | 1 |
| `01-Game-Development` | 7,925 |
| `02-Software-Development` | 523 |
| `MIGRATION.md` | 1 |
| `README.md` | 1 |
| `docs` | 1,154 |
| **Total** | **9,607** |

## Orientation

- `01-Game-Development/MissionRedHorizon` is the primary Unity root, with Assets/Packages/ProjectSettings.
- `01-Game-Development/SourceArt` contains rover/suit authoring assets and variants.
- `01-Game-Development/Experiments` contains independent Unity copies, harnesses and historical results.
- `02-Software-Development` contains the API, web app, candidate SQL, Android host and separate Stage07 prototypes.
- `docs/history` preserves Codex outputs/work, recovered unique files and backup differences; it is not one runnable application.
- Root configuration and guides are listed individually below.

## Complete tree

```text
Kabylash-Development-SK2LB/
├── 01-Game-Development/
│   ├── Experiments/
│   │   └── MissionRedHorizon_Step1Work/
│   │       ├── backup/
│   │       │   ├── LaptopPlaytest_PreChange_20261017_2218/
│   │       │   │   ├── git_head.txt
│   │       │   │   ├── prefs.sha256
│   │       │   │   ├── saves.sha256
│   │       │   │   └── scene_settings.sha256
│   │       │   ├── Stage2_PreChange_20261016_2225/
│   │       │   │   └── hashes.txt
│   │       │   ├── Step3_Backend_PreChange_20261017_0726/
│   │       │   │   └── hashes.txt
│   │       │   ├── VisualUpgrade_PreChange_20261018_1005/
│   │       │   │   ├── git_head.txt
│   │       │   │   └── manifest.sha256
│   │       │   ├── After_Hashes_Main.txt
│   │       │   ├── Before_Hashes_Main.txt
│   │       │   ├── MarsPrototype.cs.original
│   │       │   └── Resume_Hashes_Main.txt
│   │       ├── FirstMission/
│   │       │   ├── all_tests.sh
│   │       │   ├── build_both.sh
│   │       │   ├── encode_video.swift
│   │       │   ├── finish.sh
│   │       │   ├── finish2.sh
│   │       │   ├── finish3.sh
│   │       │   ├── finish4.sh
│   │       │   ├── finish5.sh
│   │       │   ├── playtest.sh
│   │       │   ├── record.sh
│   │       │   ├── run_android.sh
│   │       │   └── run_unity.sh
│   │       ├── FirstMissionAndroid/
│   │       │   ├── Assets/
│   │       │   │   ├── _Recovery/
│   │       │   │   │   ├── 0.unity
│   │       │   │   │   └── 0.unity.meta
│   │       │   │   ├── AstroGateway/
│   │       │   │   │   ├── CinematicHUD/
│   │       │   │   │   │   ├── Documentation/
│   │       │   │   │   │   │   ├── README.md
│   │       │   │   │   │   │   └── README.md.meta
│   │       │   │   │   │   ├── Prefabs/
│   │       │   │   │   │   │   ├── AstroGatewayCinematicHUD.prefab
│   │       │   │   │   │   │   └── AstroGatewayCinematicHUD.prefab.meta
│   │       │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   ├── Mars_CinematicHUD_v1.unity
│   │       │   │   │   │   │   └── Mars_CinematicHUD_v1.unity.meta
│   │       │   │   │   │   ├── Scripts/
│   │       │   │   │   │   │   ├── AstroGatewayHUDController.cs
│   │       │   │   │   │   │   ├── AstroGatewayHUDController.cs.meta
│   │       │   │   │   │   │   ├── CinematicEnvironmentAdapter.cs
│   │       │   │   │   │   │   ├── CinematicEnvironmentAdapter.cs.meta
│   │       │   │   │   │   │   ├── CinematicGameAdapter.cs
│   │       │   │   │   │   │   ├── CinematicGameAdapter.cs.meta
│   │       │   │   │   │   │   ├── HUDInputRouter.cs
│   │       │   │   │   │   │   ├── HUDInputRouter.cs.meta
│   │       │   │   │   │   │   ├── HUDLayoutProbe.cs
│   │       │   │   │   │   │   ├── HUDLayoutProbe.cs.meta
│   │       │   │   │   │   │   ├── HUDPerformanceProbe.cs
│   │       │   │   │   │   │   ├── HUDPerformanceProbe.cs.meta
│   │       │   │   │   │   │   ├── HUDPointerControl.cs
│   │       │   │   │   │   │   ├── HUDPointerControl.cs.meta
│   │       │   │   │   │   │   ├── HUDTelemetryAdapter.cs
│   │       │   │   │   │   │   ├── HUDTelemetryAdapter.cs.meta
│   │       │   │   │   │   │   ├── HUDTheme.cs
│   │       │   │   │   │   │   └── HUDTheme.cs.meta
│   │       │   │   │   │   ├── Settings/
│   │       │   │   │   │   │   ├── MissionRegistry.asset
│   │       │   │   │   │   │   └── MissionRegistry.asset.meta
│   │       │   │   │   │   ├── Tests/
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── CinematicHUDBuilder.cs
│   │       │   │   │   │   │   │   ├── CinematicHUDBuilder.cs.meta
│   │       │   │   │   │   │   │   ├── HUDResponsiveQA.cs
│   │       │   │   │   │   │   │   └── HUDResponsiveQA.cs.meta
│   │       │   │   │   │   │   └── Editor.meta
│   │       │   │   │   │   ├── Themes/
│   │       │   │   │   │   │   ├── AstroGatewayHUDTheme.asset
│   │       │   │   │   │   │   └── AstroGatewayHUDTheme.asset.meta
│   │       │   │   │   │   ├── UI/
│   │       │   │   │   │   │   ├── CircularControl.png
│   │       │   │   │   │   │   ├── CircularControl.png.meta
│   │       │   │   │   │   │   ├── HeadingArrow.png
│   │       │   │   │   │   │   ├── HeadingArrow.png.meta
│   │       │   │   │   │   │   ├── OriginalContourMap.png
│   │       │   │   │   │   │   ├── OriginalContourMap.png.meta
│   │       │   │   │   │   │   ├── RoundedPanel.png
│   │       │   │   │   │   │   └── RoundedPanel.png.meta
│   │       │   │   │   │   ├── Documentation.meta
│   │       │   │   │   │   ├── link.xml
│   │       │   │   │   │   ├── link.xml.meta
│   │       │   │   │   │   ├── Prefabs.meta
│   │       │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   ├── Scripts.meta
│   │       │   │   │   │   ├── Settings.meta
│   │       │   │   │   │   ├── Tests.meta
│   │       │   │   │   │   ├── Themes.meta
│   │       │   │   │   │   └── UI.meta
│   │       │   │   │   ├── Environment/
│   │       │   │   │   │   ├── Architecture/
│   │       │   │   │   │   │   ├── AgricultureG01_ChamferShell.asset
│   │       │   │   │   │   │   ├── AgricultureG01_ChamferShell.asset.meta
│   │       │   │   │   │   │   ├── AgricultureG01_Ring.asset
│   │       │   │   │   │   │   ├── AgricultureG01_Ring.asset.meta
│   │       │   │   │   │   │   ├── AirlockShell.asset
│   │       │   │   │   │   │   ├── AirlockShell.asset.meta
│   │       │   │   │   │   │   ├── C_ScienceStaging_Accent_Cyan_Combined.asset
│   │       │   │   │   │   │   ├── C_ScienceStaging_Accent_Cyan_Combined.asset.meta
│   │       │   │   │   │   │   ├── C_ScienceStaging_Accent_SafetyOrange_Combined.asset
│   │       │   │   │   │   │   ├── C_ScienceStaging_Accent_SafetyOrange_Combined.asset.meta
│   │       │   │   │   │   │   ├── C_ScienceStaging_Glass_Utility_Combined.asset
│   │       │   │   │   │   │   ├── C_ScienceStaging_Glass_Utility_Combined.asset.meta
│   │       │   │   │   │   │   ├── C_ScienceStaging_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   ├── C_ScienceStaging_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   ├── C_ScienceStaging_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   ├── C_ScienceStaging_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   ├── C_ScienceStaging_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   ├── C_ScienceStaging_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   ├── G_Greenhouse_Accent_Cyan_Combined.asset
│   │       │   │   │   │   │   ├── G_Greenhouse_Accent_Cyan_Combined.asset.meta
│   │       │   │   │   │   │   ├── G_Greenhouse_Glass_Utility_Combined.asset
│   │       │   │   │   │   │   ├── G_Greenhouse_Glass_Utility_Combined.asset.meta
│   │       │   │   │   │   │   ├── G_Greenhouse_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   ├── G_Greenhouse_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   ├── G_Greenhouse_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   ├── G_Greenhouse_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   ├── G_Greenhouse_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   ├── G_Greenhouse_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   ├── H_CommandHabitat_Accent_Cyan_Combined.asset
│   │       │   │   │   │   │   ├── H_CommandHabitat_Accent_Cyan_Combined.asset.meta
│   │       │   │   │   │   │   ├── H_CommandHabitat_Glass_Utility_Combined.asset
│   │       │   │   │   │   │   ├── H_CommandHabitat_Glass_Utility_Combined.asset.meta
│   │       │   │   │   │   │   ├── H_CommandHabitat_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   ├── H_CommandHabitat_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   ├── H_CommandHabitat_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   ├── H_CommandHabitat_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   ├── H_CommandHabitat_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   ├── H_CommandHabitat_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   ├── HabitatH01_ChamferShell.asset
│   │       │   │   │   │   │   ├── HabitatH01_ChamferShell.asset.meta
│   │       │   │   │   │   │   ├── HabitatH01_Ring.asset
│   │       │   │   │   │   │   ├── HabitatH01_Ring.asset.meta
│   │       │   │   │   │   │   ├── P_PowerStation_Accent_Cyan_Combined.asset
│   │       │   │   │   │   │   ├── P_PowerStation_Accent_Cyan_Combined.asset.meta
│   │       │   │   │   │   │   ├── P_PowerStation_Accent_SafetyOrange_Combined.asset
│   │       │   │   │   │   │   ├── P_PowerStation_Accent_SafetyOrange_Combined.asset.meta
│   │       │   │   │   │   │   ├── P_PowerStation_Glass_Utility_Combined.asset
│   │       │   │   │   │   │   ├── P_PowerStation_Glass_Utility_Combined.asset.meta
│   │       │   │   │   │   │   ├── P_PowerStation_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   ├── P_PowerStation_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   ├── P_PowerStation_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   ├── P_PowerStation_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   ├── P_PowerStation_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   ├── P_PowerStation_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   ├── R_RoverBay_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   ├── R_RoverBay_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   ├── R_RoverBay_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   ├── R_RoverBay_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   ├── R_RoverBay_Mars_Regolith_Dark_Combined.asset
│   │       │   │   │   │   │   ├── R_RoverBay_Mars_Regolith_Dark_Combined.asset.meta
│   │       │   │   │   │   │   ├── R_RoverBay_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── R_RoverBay_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── R_RoverBay_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   ├── R_RoverBay_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   ├── S_SolarArray_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   ├── S_SolarArray_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   ├── S_SolarArray_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   ├── S_SolarArray_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   ├── S_SolarArray_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   ├── S_SolarArray_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   ├── S_SolarArray_SolarPanel_Combined.asset
│   │       │   │   │   │   │   └── S_SolarArray_SolarPanel_Combined.asset.meta
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── MarsEnvironmentBuilder.cs
│   │       │   │   │   │   │   └── MarsEnvironmentBuilder.cs.meta
│   │       │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   ├── Accent_Cyan.mat
│   │       │   │   │   │   │   ├── Accent_Cyan.mat.meta
│   │       │   │   │   │   │   ├── Accent_SafetyOrange.mat
│   │       │   │   │   │   │   ├── Accent_SafetyOrange.mat.meta
│   │       │   │   │   │   │   ├── Basalt_Surface_512.png
│   │       │   │   │   │   │   ├── Basalt_Surface_512.png.meta
│   │       │   │   │   │   │   ├── Composite_DustPaint_512.png
│   │       │   │   │   │   │   ├── Composite_DustPaint_512.png.meta
│   │       │   │   │   │   │   ├── Delta_Bedding_512.png
│   │       │   │   │   │   │   ├── Delta_Bedding_512.png.meta
│   │       │   │   │   │   │   ├── Delta_FracturedStrata_512.png
│   │       │   │   │   │   │   ├── Delta_FracturedStrata_512.png.meta
│   │       │   │   │   │   │   ├── DustSky.shader
│   │       │   │   │   │   │   ├── DustSky.shader.meta
│   │       │   │   │   │   │   ├── Glass_Utility.mat
│   │       │   │   │   │   │   ├── Glass_Utility.mat.meta
│   │       │   │   │   │   │   ├── Habitat_CompositeDark.mat
│   │       │   │   │   │   │   ├── Habitat_CompositeDark.mat.meta
│   │       │   │   │   │   │   ├── Habitat_OffWhite.mat
│   │       │   │   │   │   │   ├── Habitat_OffWhite.mat.meta
│   │       │   │   │   │   │   ├── Mars_DustSky.mat
│   │       │   │   │   │   │   ├── Mars_DustSky.mat.meta
│   │       │   │   │   │   │   ├── Mars_Regolith.mat
│   │       │   │   │   │   │   ├── Mars_Regolith.mat.meta
│   │       │   │   │   │   │   ├── Mars_Regolith_Dark.mat
│   │       │   │   │   │   │   ├── Mars_Regolith_Dark.mat.meta
│   │       │   │   │   │   │   ├── Mars_Rock_Basaltic.mat
│   │       │   │   │   │   │   ├── Mars_Rock_Basaltic.mat.meta
│   │       │   │   │   │   │   ├── Mars_Rock_Layered.mat
│   │       │   │   │   │   │   ├── Mars_Rock_Layered.mat.meta
│   │       │   │   │   │   │   ├── Metal_Industrial.mat
│   │       │   │   │   │   │   ├── Metal_Industrial.mat.meta
│   │       │   │   │   │   │   ├── Regolith_Grain_512.png
│   │       │   │   │   │   │   ├── Regolith_Grain_512.png.meta
│   │       │   │   │   │   │   ├── Regolith_Macro_Pathwear_1024.png
│   │       │   │   │   │   │   ├── Regolith_Macro_Pathwear_1024.png.meta
│   │       │   │   │   │   │   ├── Regolith_MicroNormal_256.png
│   │       │   │   │   │   │   ├── Regolith_MicroNormal_256.png.meta
│   │       │   │   │   │   │   ├── Solar_Cells_Dust_512.png
│   │       │   │   │   │   │   ├── Solar_Cells_Dust_512.png.meta
│   │       │   │   │   │   │   ├── SolarPanel.mat
│   │       │   │   │   │   │   └── SolarPanel.mat.meta
│   │       │   │   │   │   ├── Prefabs/
│   │       │   │   │   │   │   ├── AngularBasalt.prefab
│   │       │   │   │   │   │   ├── AngularBasalt.prefab.meta
│   │       │   │   │   │   │   ├── C_ScienceStaging.prefab
│   │       │   │   │   │   │   ├── C_ScienceStaging.prefab.meta
│   │       │   │   │   │   │   ├── ErodedLandmark.prefab
│   │       │   │   │   │   │   ├── ErodedLandmark.prefab.meta
│   │       │   │   │   │   │   ├── G_Greenhouse.prefab
│   │       │   │   │   │   │   ├── G_Greenhouse.prefab.meta
│   │       │   │   │   │   │   ├── H_CommandHabitat.prefab
│   │       │   │   │   │   │   ├── H_CommandHabitat.prefab.meta
│   │       │   │   │   │   │   ├── LayeredDelta.prefab
│   │       │   │   │   │   │   ├── LayeredDelta.prefab.meta
│   │       │   │   │   │   │   ├── P_PowerStation.prefab
│   │       │   │   │   │   │   ├── P_PowerStation.prefab.meta
│   │       │   │   │   │   │   ├── R_RoverBay.prefab
│   │       │   │   │   │   │   ├── R_RoverBay.prefab.meta
│   │       │   │   │   │   │   ├── RegolithStone.prefab
│   │       │   │   │   │   │   ├── RegolithStone.prefab.meta
│   │       │   │   │   │   │   ├── S_SolarArray.prefab
│   │       │   │   │   │   │   └── S_SolarArray.prefab.meta
│   │       │   │   │   │   ├── Props/
│   │       │   │   │   │   │   ├── ApproachStoneClusters_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── ApproachStoneClusters_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── HabitatExternalKit_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   ├── HabitatExternalKit_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   ├── HabitatExternalKit_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   ├── HabitatExternalKit_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   ├── HabitatExternalKit_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   ├── HabitatExternalKit_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   ├── PowerExternalKit_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   ├── PowerExternalKit_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   ├── PowerExternalKit_Mars_Regolith_Dark_Combined.asset
│   │       │   │   │   │   │   ├── PowerExternalKit_Mars_Regolith_Dark_Combined.asset.meta
│   │       │   │   │   │   │   ├── UtilityRoutePosts_Accent_Cyan_Combined.asset
│   │       │   │   │   │   │   ├── UtilityRoutePosts_Accent_Cyan_Combined.asset.meta
│   │       │   │   │   │   │   ├── UtilityRoutePosts_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   └── UtilityRoutePosts_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   ├── Rocks/
│   │       │   │   │   │   │   ├── AngularBasalt_LOD0.asset
│   │       │   │   │   │   │   ├── AngularBasalt_LOD0.asset.meta
│   │       │   │   │   │   │   ├── AngularBasalt_LOD1.asset
│   │       │   │   │   │   │   ├── AngularBasalt_LOD1.asset.meta
│   │       │   │   │   │   │   ├── AngularBasalt_LOD2.asset
│   │       │   │   │   │   │   ├── AngularBasalt_LOD2.asset.meta
│   │       │   │   │   │   │   ├── DeltaFractured_LOD0.asset
│   │       │   │   │   │   │   ├── DeltaFractured_LOD0.asset.meta
│   │       │   │   │   │   │   ├── DeltaFractured_LOD1.asset
│   │       │   │   │   │   │   ├── DeltaFractured_LOD1.asset.meta
│   │       │   │   │   │   │   ├── DeltaFractured_LOD2.asset
│   │       │   │   │   │   │   ├── DeltaFractured_LOD2.asset.meta
│   │       │   │   │   │   │   ├── ErodedLandmark_LOD0.asset
│   │       │   │   │   │   │   ├── ErodedLandmark_LOD0.asset.meta
│   │       │   │   │   │   │   ├── ErodedLandmark_LOD1.asset
│   │       │   │   │   │   │   ├── ErodedLandmark_LOD1.asset.meta
│   │       │   │   │   │   │   ├── ErodedLandmark_LOD2.asset
│   │       │   │   │   │   │   ├── ErodedLandmark_LOD2.asset.meta
│   │       │   │   │   │   │   ├── LayeredDelta_LOD0.asset
│   │       │   │   │   │   │   ├── LayeredDelta_LOD0.asset.meta
│   │       │   │   │   │   │   ├── LayeredDelta_LOD1.asset
│   │       │   │   │   │   │   ├── LayeredDelta_LOD1.asset.meta
│   │       │   │   │   │   │   ├── LayeredDelta_LOD2.asset
│   │       │   │   │   │   │   ├── LayeredDelta_LOD2.asset.meta
│   │       │   │   │   │   │   ├── StonePatch_0_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── StonePatch_0_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── StonePatch_1_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── StonePatch_1_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── StonePatch_2_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── StonePatch_2_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── StonePatch_3_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── StonePatch_3_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── StonePatch_4_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── StonePatch_4_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── StonePatch_5_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── StonePatch_5_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── StonePatch_6_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── StonePatch_6_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── StonePatch_7_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── StonePatch_7_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── StonePatch_8_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   └── StonePatch_8_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   ├── Terrain/
│   │       │   │   │   │   │   ├── Regolith_250m_PreservedCorridor.asset
│   │       │   │   │   │   │   └── Regolith_250m_PreservedCorridor.asset.meta
│   │       │   │   │   │   ├── Architecture.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   ├── Prefabs.meta
│   │       │   │   │   │   ├── Props.meta
│   │       │   │   │   │   ├── Rocks.meta
│   │       │   │   │   │   └── Terrain.meta
│   │       │   │   │   ├── FirstMission/
│   │       │   │   │   │   ├── Config/
│   │       │   │   │   │   │   ├── sensor_catalog_v1.json
│   │       │   │   │   │   │   └── sensor_catalog_v1.json.meta
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── FirstMissionAndroidBuild.cs
│   │       │   │   │   │   │   ├── FirstMissionAndroidBuild.cs.meta
│   │       │   │   │   │   │   ├── FirstMissionBackendClientTest.cs
│   │       │   │   │   │   │   ├── FirstMissionBackendClientTest.cs.meta
│   │       │   │   │   │   │   ├── FirstMissionBuilder.cs
│   │       │   │   │   │   │   ├── FirstMissionBuilder.cs.meta
│   │       │   │   │   │   │   ├── FirstMissionDemoRecorder.cs
│   │       │   │   │   │   │   ├── FirstMissionDemoRecorder.cs.meta
│   │       │   │   │   │   │   ├── FirstMissionLayoutProbe.cs
│   │       │   │   │   │   │   ├── FirstMissionLayoutProbe.cs.meta
│   │       │   │   │   │   │   ├── FirstMissionPlaytest.cs
│   │       │   │   │   │   │   ├── FirstMissionPlaytest.cs.meta
│   │       │   │   │   │   │   ├── MissionStructuresBuilder.cs
│   │       │   │   │   │   │   └── MissionStructuresBuilder.cs.meta
│   │       │   │   │   │   ├── Meshes/
│   │       │   │   │   │   │   ├── MissionTent.asset
│   │       │   │   │   │   │   └── MissionTent.asset.meta
│   │       │   │   │   │   ├── Prefabs/
│   │       │   │   │   │   │   ├── ARES_VI_FirstMission.prefab
│   │       │   │   │   │   │   └── ARES_VI_FirstMission.prefab.meta
│   │       │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   ├── Mars_FirstMission_Explorer.unity
│   │       │   │   │   │   │   ├── Mars_FirstMission_Explorer.unity.meta
│   │       │   │   │   │   │   ├── Mars_FirstMission_HUDv2.unity
│   │       │   │   │   │   │   ├── Mars_FirstMission_HUDv2.unity.meta
│   │       │   │   │   │   │   ├── Mars_FirstMission_Missions.unity
│   │       │   │   │   │   │   ├── Mars_FirstMission_Missions.unity.meta
│   │       │   │   │   │   │   ├── Mars_FirstMission_v1.unity
│   │       │   │   │   │   │   └── Mars_FirstMission_v1.unity.meta
│   │       │   │   │   │   ├── Scripts/
│   │       │   │   │   │   │   ├── HUD/
│   │       │   │   │   │   │   │   ├── ExplorerCinematics.cs
│   │       │   │   │   │   │   │   ├── ExplorerCinematics.cs.meta
│   │       │   │   │   │   │   │   ├── ExplorerHUD.cs
│   │       │   │   │   │   │   │   ├── ExplorerHUD.cs.meta
│   │       │   │   │   │   │   │   ├── HudFx.cs
│   │       │   │   │   │   │   │   ├── HudFx.cs.meta
│   │       │   │   │   │   │   │   ├── HudIcons.cs
│   │       │   │   │   │   │   │   ├── HudIcons.cs.meta
│   │       │   │   │   │   │   │   ├── HudStatus.cs
│   │       │   │   │   │   │   │   ├── HudStatus.cs.meta
│   │       │   │   │   │   │   │   ├── HudWidgets.cs
│   │       │   │   │   │   │   │   ├── HudWidgets.cs.meta
│   │       │   │   │   │   │   │   ├── MissionsHUD.cs
│   │       │   │   │   │   │   │   ├── MissionsHUD.cs.meta
│   │       │   │   │   │   │   │   ├── PremiumHUD.cs
│   │       │   │   │   │   │   │   └── PremiumHUD.cs.meta
│   │       │   │   │   │   │   ├── Missions/
│   │       │   │   │   │   │   │   ├── ChargingPadState.cs
│   │       │   │   │   │   │   │   ├── ChargingPadState.cs.meta
│   │       │   │   │   │   │   │   ├── CheckpointState.cs
│   │       │   │   │   │   │   │   ├── CheckpointState.cs.meta
│   │       │   │   │   │   │   │   ├── GeneratorState.cs
│   │       │   │   │   │   │   │   ├── GeneratorState.cs.meta
│   │       │   │   │   │   │   │   ├── MissionDirector.cs
│   │       │   │   │   │   │   │   ├── MissionDirector.cs.meta
│   │       │   │   │   │   │   │   ├── MissionFx.cs
│   │       │   │   │   │   │   │   ├── MissionFx.cs.meta
│   │       │   │   │   │   │   │   ├── MissionStructure.cs
│   │       │   │   │   │   │   │   └── MissionStructure.cs.meta
│   │       │   │   │   │   │   ├── Nav/
│   │       │   │   │   │   │   │   ├── FirstMissionNavigation.cs
│   │       │   │   │   │   │   │   └── FirstMissionNavigation.cs.meta
│   │       │   │   │   │   │   ├── AresRoverPresentation.cs
│   │       │   │   │   │   │   ├── AresRoverPresentation.cs.meta
│   │       │   │   │   │   │   ├── FirstMissionCameraRig.cs
│   │       │   │   │   │   │   ├── FirstMissionCameraRig.cs.meta
│   │       │   │   │   │   │   ├── FirstMissionControlLayer.cs
│   │       │   │   │   │   │   ├── FirstMissionControlLayer.cs.meta
│   │       │   │   │   │   │   ├── FirstMissionController.cs
│   │       │   │   │   │   │   ├── FirstMissionController.cs.meta
│   │       │   │   │   │   │   ├── FirstMissionData.cs
│   │       │   │   │   │   │   ├── FirstMissionData.cs.meta
│   │       │   │   │   │   │   ├── FirstMissionHUD.cs
│   │       │   │   │   │   │   ├── FirstMissionHUD.cs.meta
│   │       │   │   │   │   │   ├── HUD.meta
│   │       │   │   │   │   │   ├── Missions.meta
│   │       │   │   │   │   │   ├── Nav.meta
│   │       │   │   │   │   │   ├── RoverSensorSuite.cs
│   │       │   │   │   │   │   └── RoverSensorSuite.cs.meta
│   │       │   │   │   │   ├── Settings/
│   │       │   │   │   │   │   ├── FirstMissionRegistry.asset
│   │       │   │   │   │   │   └── FirstMissionRegistry.asset.meta
│   │       │   │   │   │   ├── Config.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── Meshes.meta
│   │       │   │   │   │   ├── Prefabs.meta
│   │       │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   ├── Scripts.meta
│   │       │   │   │   │   └── Settings.meta
│   │       │   │   │   ├── Greybox/
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── MarsGreyboxBuilder.cs
│   │       │   │   │   │   │   └── MarsGreyboxBuilder.cs.meta
│   │       │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   ├── DarkRock.mat
│   │       │   │   │   │   │   ├── DarkRock.mat.meta
│   │       │   │   │   │   │   ├── HabitatPale.mat
│   │       │   │   │   │   │   ├── HabitatPale.mat.meta
│   │       │   │   │   │   │   ├── IndustrialGraphite.mat
│   │       │   │   │   │   │   ├── IndustrialGraphite.mat.meta
│   │       │   │   │   │   │   ├── LayeredStone.mat
│   │       │   │   │   │   │   ├── LayeredStone.mat.meta
│   │       │   │   │   │   │   ├── SandTan.mat
│   │       │   │   │   │   │   ├── SandTan.mat.meta
│   │       │   │   │   │   │   ├── TerrainOchre.mat
│   │       │   │   │   │   │   ├── TerrainOchre.mat.meta
│   │       │   │   │   │   │   ├── WayfindingAmber.mat
│   │       │   │   │   │   │   ├── WayfindingAmber.mat.meta
│   │       │   │   │   │   │   ├── WayfindingCyan.mat
│   │       │   │   │   │   │   └── WayfindingCyan.mat.meta
│   │       │   │   │   │   ├── Meshes/
│   │       │   │   │   │   │   ├── Ground.asset
│   │       │   │   │   │   │   └── Ground.asset.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── MarsOutpost_Greybox.unity
│   │       │   │   │   │   ├── MarsOutpost_Greybox.unity.meta
│   │       │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   ├── Meshes.meta
│   │       │   │   │   │   ├── PreGreybox_Untitled_Backup.unity
│   │       │   │   │   │   └── PreGreybox_Untitled_Backup.unity.meta
│   │       │   │   │   ├── Integration/
│   │       │   │   │   │   ├── Contracts/
│   │       │   │   │   │   │   ├── IntegrationContracts.cs
│   │       │   │   │   │   │   └── IntegrationContracts.cs.meta
│   │       │   │   │   │   ├── Http/
│   │       │   │   │   │   │   ├── HttpMissionSessionService.cs
│   │       │   │   │   │   │   └── HttpMissionSessionService.cs.meta
│   │       │   │   │   │   ├── Local/
│   │       │   │   │   │   │   ├── GameplayEventBridge.cs
│   │       │   │   │   │   │   ├── GameplayEventBridge.cs.meta
│   │       │   │   │   │   │   ├── LifeSupportModelAdapter.cs
│   │       │   │   │   │   │   ├── LifeSupportModelAdapter.cs.meta
│   │       │   │   │   │   │   ├── LocalServices.cs
│   │       │   │   │   │   │   └── LocalServices.cs.meta
│   │       │   │   │   │   ├── Contracts.meta
│   │       │   │   │   │   ├── Http.meta
│   │       │   │   │   │   └── Local.meta
│   │       │   │   │   ├── MarsEnvironment/
│   │       │   │   │   │   ├── Documentation/
│   │       │   │   │   │   │   ├── MissionRegistry.asset
│   │       │   │   │   │   │   ├── MissionRegistry.asset.meta
│   │       │   │   │   │   │   ├── README.md
│   │       │   │   │   │   │   └── README.md.meta
│   │       │   │   │   │   ├── Lighting/
│   │       │   │   │   │   │   ├── Dusty_Conditions.asset
│   │       │   │   │   │   │   ├── Dusty_Conditions.asset.meta
│   │       │   │   │   │   │   ├── MarsGradient.shader
│   │       │   │   │   │   │   ├── MarsGradient.shader.meta
│   │       │   │   │   │   │   ├── MarsSky.mat
│   │       │   │   │   │   │   ├── MarsSky.mat.meta
│   │       │   │   │   │   │   ├── MarsVolume.asset
│   │       │   │   │   │   │   ├── MarsVolume.asset.meta
│   │       │   │   │   │   │   ├── Midday.asset
│   │       │   │   │   │   │   ├── Midday.asset.meta
│   │       │   │   │   │   │   ├── Morning.asset
│   │       │   │   │   │   │   ├── Morning.asset.meta
│   │       │   │   │   │   │   ├── Sunset.asset
│   │       │   │   │   │   │   └── Sunset.asset.meta
│   │       │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   ├── DistantDust.asset
│   │       │   │   │   │   │   ├── DistantDust.asset.meta
│   │       │   │   │   │   │   ├── Dusty solar cells.mat
│   │       │   │   │   │   │   ├── Dusty solar cells.mat.meta
│   │       │   │   │   │   │   ├── EnvironmentDust.mat
│   │       │   │   │   │   │   ├── EnvironmentDust.mat.meta
│   │       │   │   │   │   │   ├── Fine Mars dust.mat
│   │       │   │   │   │   │   ├── Fine Mars dust.mat.meta
│   │       │   │   │   │   │   ├── Habitat ceramic composite.mat
│   │       │   │   │   │   │   ├── Habitat ceramic composite.mat.meta
│   │       │   │   │   │   │   ├── Observation panels — opaque approximation.mat
│   │       │   │   │   │   │   ├── Observation panels — opaque approximation.mat.meta
│   │       │   │   │   │   │   ├── Restrained identification ochre.mat
│   │       │   │   │   │   │   ├── Restrained identification ochre.mat.meta
│   │       │   │   │   │   │   ├── Rock07 dusty basalt.mat
│   │       │   │   │   │   │   ├── Rock07 dusty basalt.mat.meta
│   │       │   │   │   │   │   ├── Rock09 compact mineral.mat
│   │       │   │   │   │   │   ├── Rock09 compact mineral.mat.meta
│   │       │   │   │   │   │   ├── Rover_Cab.mat
│   │       │   │   │   │   │   ├── Rover_Cab.mat.meta
│   │       │   │   │   │   │   ├── Rover_Chassis.mat
│   │       │   │   │   │   │   ├── Rover_Chassis.mat.meta
│   │       │   │   │   │   │   ├── Rover_RoofMarker.mat
│   │       │   │   │   │   │   ├── Rover_RoofMarker.mat.meta
│   │       │   │   │   │   │   ├── Rover_Wheel_-1_0.mat
│   │       │   │   │   │   │   ├── Rover_Wheel_-1_0.mat.meta
│   │       │   │   │   │   │   ├── Rover_Wheel_-1_1.mat
│   │       │   │   │   │   │   ├── Rover_Wheel_-1_1.mat.meta
│   │       │   │   │   │   │   ├── Rover_Wheel_-1_2.mat
│   │       │   │   │   │   │   ├── Rover_Wheel_-1_2.mat.meta
│   │       │   │   │   │   │   ├── Rover_Wheel_1_0.mat
│   │       │   │   │   │   │   ├── Rover_Wheel_1_0.mat.meta
│   │       │   │   │   │   │   ├── Rover_Wheel_1_1.mat
│   │       │   │   │   │   │   ├── Rover_Wheel_1_1.mat.meta
│   │       │   │   │   │   │   ├── Rover_Wheel_1_2.mat
│   │       │   │   │   │   │   ├── Rover_Wheel_1_2.mat.meta
│   │       │   │   │   │   │   ├── Structural graphite.mat
│   │       │   │   │   │   │   └── Structural graphite.mat.meta
│   │       │   │   │   │   ├── Models/
│   │       │   │   │   │   │   ├── Crew_Dome_Ribs_LowPoly.asset
│   │       │   │   │   │   │   ├── Crew_Dome_Ribs_LowPoly.asset.meta
│   │       │   │   │   │   │   ├── HabitatHemisphere.asset
│   │       │   │   │   │   │   ├── HabitatHemisphere.asset.meta
│   │       │   │   │   │   │   ├── Laboratory_Dome_Ribs_LowPoly.asset
│   │       │   │   │   │   │   ├── Laboratory_Dome_Ribs_LowPoly.asset.meta
│   │       │   │   │   │   │   ├── Life_Support_Dome_Ribs_LowPoly.asset
│   │       │   │   │   │   │   ├── Life_Support_Dome_Ribs_LowPoly.asset.meta
│   │       │   │   │   │   │   ├── rock_07_LOD0.asset
│   │       │   │   │   │   │   ├── rock_07_LOD0.asset.meta
│   │       │   │   │   │   │   ├── rock_07_LOD1.asset
│   │       │   │   │   │   │   ├── rock_07_LOD1.asset.meta
│   │       │   │   │   │   │   ├── rock_07_LOD2.asset
│   │       │   │   │   │   │   ├── rock_07_LOD2.asset.meta
│   │       │   │   │   │   │   ├── rock_09_LOD0.asset
│   │       │   │   │   │   │   ├── rock_09_LOD0.asset.meta
│   │       │   │   │   │   │   ├── rock_09_LOD1.asset
│   │       │   │   │   │   │   ├── rock_09_LOD1.asset.meta
│   │       │   │   │   │   │   ├── rock_09_LOD2.asset
│   │       │   │   │   │   │   ├── rock_09_LOD2.asset.meta
│   │       │   │   │   │   │   ├── Terrain250m.asset
│   │       │   │   │   │   │   └── Terrain250m.asset.meta
│   │       │   │   │   │   ├── Prefabs/
│   │       │   │   │   │   │   ├── ARESYN_ClosedHabitatComplex_Optimized.prefab
│   │       │   │   │   │   │   ├── ARESYN_ClosedHabitatComplex_Optimized.prefab.meta
│   │       │   │   │   │   │   ├── SolarFarm_Optimized.prefab
│   │       │   │   │   │   │   └── SolarFarm_Optimized.prefab.meta
│   │       │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   ├── Mars_RealisticResearchBase_v1.unity
│   │       │   │   │   │   │   └── Mars_RealisticResearchBase_v1.unity.meta
│   │       │   │   │   │   ├── Scripts/
│   │       │   │   │   │   │   ├── EnvironmentPresentation.cs
│   │       │   │   │   │   │   ├── EnvironmentPresentation.cs.meta
│   │       │   │   │   │   │   ├── EnvironmentPreset.cs
│   │       │   │   │   │   │   ├── EnvironmentPreset.cs.meta
│   │       │   │   │   │   │   ├── ResearchTarget.cs
│   │       │   │   │   │   │   └── ResearchTarget.cs.meta
│   │       │   │   │   │   ├── Tests/
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── MarsResearchBaseBuilder.cs
│   │       │   │   │   │   │   │   └── MarsResearchBaseBuilder.cs.meta
│   │       │   │   │   │   │   └── Editor.meta
│   │       │   │   │   │   ├── Textures/
│   │       │   │   │   │   │   ├── rock_07_AO.png
│   │       │   │   │   │   │   ├── rock_07_AO.png.meta
│   │       │   │   │   │   │   ├── rock_07_Diffuse.png
│   │       │   │   │   │   │   ├── rock_07_Diffuse.png.meta
│   │       │   │   │   │   │   ├── rock_07_nor_gl.png
│   │       │   │   │   │   │   ├── rock_07_nor_gl.png.meta
│   │       │   │   │   │   │   ├── rock_07_Rough.png
│   │       │   │   │   │   │   ├── rock_07_Rough.png.meta
│   │       │   │   │   │   │   ├── rock_09_AO.png
│   │       │   │   │   │   │   ├── rock_09_AO.png.meta
│   │       │   │   │   │   │   ├── rock_09_Diffuse.png
│   │       │   │   │   │   │   ├── rock_09_Diffuse.png.meta
│   │       │   │   │   │   │   ├── rock_09_nor_gl.png
│   │       │   │   │   │   │   ├── rock_09_nor_gl.png.meta
│   │       │   │   │   │   │   ├── rock_09_Rough.png
│   │       │   │   │   │   │   ├── rock_09_Rough.png.meta
│   │       │   │   │   │   │   ├── sandy_gravel_AO.png
│   │       │   │   │   │   │   ├── sandy_gravel_AO.png.meta
│   │       │   │   │   │   │   ├── sandy_gravel_Diffuse.png
│   │       │   │   │   │   │   ├── sandy_gravel_Diffuse.png.meta
│   │       │   │   │   │   │   ├── sandy_gravel_nor_gl.png
│   │       │   │   │   │   │   ├── sandy_gravel_nor_gl.png.meta
│   │       │   │   │   │   │   ├── sandy_gravel_Rough.png
│   │       │   │   │   │   │   └── sandy_gravel_Rough.png.meta
│   │       │   │   │   │   ├── Documentation.meta
│   │       │   │   │   │   ├── Lighting.meta
│   │       │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   ├── Models.meta
│   │       │   │   │   │   ├── Prefabs.meta
│   │       │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   ├── Scripts.meta
│   │       │   │   │   │   ├── Tests.meta
│   │       │   │   │   │   ├── Textures.meta
│   │       │   │   │   │   └── VFX.meta
│   │       │   │   │   ├── MarsVisualPolish/
│   │       │   │   │   │   ├── Documentation/
│   │       │   │   │   │   │   ├── PolishRegistry.asset
│   │       │   │   │   │   │   └── PolishRegistry.asset.meta
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── VisualPolishBuilder.cs
│   │       │   │   │   │   │   └── VisualPolishBuilder.cs.meta
│   │       │   │   │   │   ├── Lighting/
│   │       │   │   │   │   │   ├── PolishSky.mat
│   │       │   │   │   │   │   ├── PolishSky.mat.meta
│   │       │   │   │   │   │   ├── PolishVolume.asset
│   │       │   │   │   │   │   ├── PolishVolume.asset.meta
│   │       │   │   │   │   │   ├── Preset_0.asset
│   │       │   │   │   │   │   ├── Preset_0.asset.meta
│   │       │   │   │   │   │   ├── Preset_1.asset
│   │       │   │   │   │   │   ├── Preset_1.asset.meta
│   │       │   │   │   │   │   ├── Preset_2.asset
│   │       │   │   │   │   │   ├── Preset_2.asset.meta
│   │       │   │   │   │   │   ├── Preset_3.asset
│   │       │   │   │   │   │   └── Preset_3.asset.meta
│   │       │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   ├── Brushed frame.mat
│   │       │   │   │   │   │   ├── Brushed frame.mat.meta
│   │       │   │   │   │   │   ├── Derivative_0.mat
│   │       │   │   │   │   │   ├── Derivative_0.mat.meta
│   │       │   │   │   │   │   ├── Derivative_1.mat
│   │       │   │   │   │   │   ├── Derivative_1.mat.meta
│   │       │   │   │   │   │   ├── Derivative_10.mat
│   │       │   │   │   │   │   ├── Derivative_10.mat.meta
│   │       │   │   │   │   │   ├── Derivative_11.mat
│   │       │   │   │   │   │   ├── Derivative_11.mat.meta
│   │       │   │   │   │   │   ├── Derivative_12.mat
│   │       │   │   │   │   │   ├── Derivative_12.mat.meta
│   │       │   │   │   │   │   ├── Derivative_13.mat
│   │       │   │   │   │   │   ├── Derivative_13.mat.meta
│   │       │   │   │   │   │   ├── Derivative_14.mat
│   │       │   │   │   │   │   ├── Derivative_14.mat.meta
│   │       │   │   │   │   │   ├── Derivative_15.mat
│   │       │   │   │   │   │   ├── Derivative_15.mat.meta
│   │       │   │   │   │   │   ├── Derivative_16.mat
│   │       │   │   │   │   │   ├── Derivative_16.mat.meta
│   │       │   │   │   │   │   ├── Derivative_17.mat
│   │       │   │   │   │   │   ├── Derivative_17.mat.meta
│   │       │   │   │   │   │   ├── Derivative_18.mat
│   │       │   │   │   │   │   ├── Derivative_18.mat.meta
│   │       │   │   │   │   │   ├── Derivative_19.mat
│   │       │   │   │   │   │   ├── Derivative_19.mat.meta
│   │       │   │   │   │   │   ├── Derivative_2.mat
│   │       │   │   │   │   │   ├── Derivative_2.mat.meta
│   │       │   │   │   │   │   ├── Derivative_20.mat
│   │       │   │   │   │   │   ├── Derivative_20.mat.meta
│   │       │   │   │   │   │   ├── Derivative_21.mat
│   │       │   │   │   │   │   ├── Derivative_21.mat.meta
│   │       │   │   │   │   │   ├── Derivative_3.mat
│   │       │   │   │   │   │   ├── Derivative_3.mat.meta
│   │       │   │   │   │   │   ├── Derivative_4.mat
│   │       │   │   │   │   │   ├── Derivative_4.mat.meta
│   │       │   │   │   │   │   ├── Derivative_5.mat
│   │       │   │   │   │   │   ├── Derivative_5.mat.meta
│   │       │   │   │   │   │   ├── Derivative_6.mat
│   │       │   │   │   │   │   ├── Derivative_6.mat.meta
│   │       │   │   │   │   │   ├── Derivative_7.mat
│   │       │   │   │   │   │   ├── Derivative_7.mat.meta
│   │       │   │   │   │   │   ├── Derivative_8.mat
│   │       │   │   │   │   │   ├── Derivative_8.mat.meta
│   │       │   │   │   │   │   ├── Derivative_9.mat
│   │       │   │   │   │   │   ├── Derivative_9.mat.meta
│   │       │   │   │   │   │   ├── Photovoltaic cell laminate.mat
│   │       │   │   │   │   │   ├── Photovoltaic cell laminate.mat.meta
│   │       │   │   │   │   │   ├── PhotovoltaicGrid.png
│   │       │   │   │   │   │   ├── PhotovoltaicGrid.png.meta
│   │       │   │   │   │   │   ├── Recessed seals.mat
│   │       │   │   │   │   │   ├── Recessed seals.mat.meta
│   │       │   │   │   │   │   ├── Safety ochre.mat
│   │       │   │   │   │   │   ├── Safety ochre.mat.meta
│   │       │   │   │   │   │   ├── Warm ceramic.mat
│   │       │   │   │   │   │   └── Warm ceramic.mat.meta
│   │       │   │   │   │   ├── Models/
│   │       │   │   │   │   │   ├── Crew Dome_Frame.asset
│   │       │   │   │   │   │   ├── Crew Dome_Frame.asset.meta
│   │       │   │   │   │   │   ├── Laboratory Dome_Frame.asset
│   │       │   │   │   │   │   ├── Laboratory Dome_Frame.asset.meta
│   │       │   │   │   │   │   ├── Life Support Dome_Frame.asset
│   │       │   │   │   │   │   └── Life Support Dome_Frame.asset.meta
│   │       │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   ├── Mars_VisualPolish_v2.unity
│   │       │   │   │   │   │   └── Mars_VisualPolish_v2.unity.meta
│   │       │   │   │   │   ├── Documentation.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── Lighting.meta
│   │       │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   ├── Models.meta
│   │       │   │   │   │   ├── PolishBenchmark.cs
│   │       │   │   │   │   ├── PolishBenchmark.cs.meta
│   │       │   │   │   │   ├── PolishHudTheme.cs
│   │       │   │   │   │   ├── PolishHudTheme.cs.meta
│   │       │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   ├── VisualPolishPresentation.cs
│   │       │   │   │   │   └── VisualPolishPresentation.cs.meta
│   │       │   │   │   ├── Mission01Milestone/
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── Mission01Builder.cs
│   │       │   │   │   │   │   └── Mission01Builder.cs.meta
│   │       │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   ├── Mission01_Continents.mat
│   │       │   │   │   │   │   ├── Mission01_Continents.mat.meta
│   │       │   │   │   │   │   ├── Mission01_Graphite.mat
│   │       │   │   │   │   │   ├── Mission01_Graphite.mat.meta
│   │       │   │   │   │   │   ├── Mission01_Ocean.mat
│   │       │   │   │   │   │   ├── Mission01_Ocean.mat.meta
│   │       │   │   │   │   │   ├── Mission01_Pale.mat
│   │       │   │   │   │   │   ├── Mission01_Pale.mat.meta
│   │       │   │   │   │   │   ├── Mission01_Safety.mat
│   │       │   │   │   │   │   └── Mission01_Safety.mat.meta
│   │       │   │   │   │   ├── Prefabs/
│   │       │   │   │   │   │   ├── Boarding_Console.prefab
│   │       │   │   │   │   │   ├── Boarding_Console.prefab.meta
│   │       │   │   │   │   │   ├── Trainee_Blockout.prefab
│   │       │   │   │   │   │   └── Trainee_Blockout.prefab.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   ├── Mission01_EarthLaunch_Milestone_v4.unity
│   │       │   │   │   │   ├── Mission01_EarthLaunch_Milestone_v4.unity.meta
│   │       │   │   │   │   ├── Mission01Flow.cs
│   │       │   │   │   │   ├── Mission01Flow.cs.meta
│   │       │   │   │   │   └── Prefabs.meta
│   │       │   │   │   ├── MissionIntegration/
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── MissionIntegrationBuilder.cs
│   │       │   │   │   │   │   └── MissionIntegrationBuilder.cs.meta
│   │       │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   ├── Mars_MissionIntegration_v1.unity
│   │       │   │   │   │   │   └── Mars_MissionIntegration_v1.unity.meta
│   │       │   │   │   │   ├── Scripts/
│   │       │   │   │   │   │   ├── MissionIntegrationController.cs
│   │       │   │   │   │   │   └── MissionIntegrationController.cs.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   └── Scripts.meta
│   │       │   │   │   ├── MissionRules/
│   │       │   │   │   │   ├── Config/
│   │       │   │   │   │   │   ├── mission_rules_v1.json
│   │       │   │   │   │   │   ├── mission_rules_v1.json.meta
│   │       │   │   │   │   │   ├── test_vectors_v1.json
│   │       │   │   │   │   │   └── test_vectors_v1.json.meta
│   │       │   │   │   │   ├── Scripts/
│   │       │   │   │   │   │   ├── MiniJson.cs
│   │       │   │   │   │   │   ├── MiniJson.cs.meta
│   │       │   │   │   │   │   ├── MissionRules.cs
│   │       │   │   │   │   │   └── MissionRules.cs.meta
│   │       │   │   │   │   ├── Config.meta
│   │       │   │   │   │   └── Scripts.meta
│   │       │   │   │   ├── NativeControlSlice/
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── NativeSliceBuilder.cs
│   │       │   │   │   │   │   └── NativeSliceBuilder.cs.meta
│   │       │   │   │   │   ├── Prefabs/
│   │       │   │   │   │   │   ├── AdaptiveTouch.prefab
│   │       │   │   │   │   │   └── AdaptiveTouch.prefab.meta
│   │       │   │   │   │   ├── AdaptiveTouch.cs
│   │       │   │   │   │   ├── AdaptiveTouch.cs.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── Mars_NativeControl_VerticalSlice.unity
│   │       │   │   │   │   ├── Mars_NativeControl_VerticalSlice.unity.meta
│   │       │   │   │   │   ├── MissionRegistry.asset
│   │       │   │   │   │   ├── MissionRegistry.asset.meta
│   │       │   │   │   │   ├── NativeSliceGame.cs
│   │       │   │   │   │   ├── NativeSliceGame.cs.meta
│   │       │   │   │   │   ├── NativeSystems.cs
│   │       │   │   │   │   ├── NativeSystems.cs.meta
│   │       │   │   │   │   └── Prefabs.meta
│   │       │   │   │   ├── ProductionVisual/
│   │       │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   ├── Brushed aluminum.mat
│   │       │   │   │   │   │   ├── Brushed aluminum.mat.meta
│   │       │   │   │   │   │   ├── Dusty tire elastomer.mat
│   │       │   │   │   │   │   ├── Dusty tire elastomer.mat.meta
│   │       │   │   │   │   │   ├── Inherited_0.mat
│   │       │   │   │   │   │   ├── Inherited_0.mat.meta
│   │       │   │   │   │   │   ├── Inherited_1.mat
│   │       │   │   │   │   │   ├── Inherited_1.mat.meta
│   │       │   │   │   │   │   ├── Inherited_10.mat
│   │       │   │   │   │   │   ├── Inherited_10.mat.meta
│   │       │   │   │   │   │   ├── Inherited_11.mat
│   │       │   │   │   │   │   ├── Inherited_11.mat.meta
│   │       │   │   │   │   │   ├── Inherited_12.mat
│   │       │   │   │   │   │   ├── Inherited_12.mat.meta
│   │       │   │   │   │   │   ├── Inherited_13.mat
│   │       │   │   │   │   │   ├── Inherited_13.mat.meta
│   │       │   │   │   │   │   ├── Inherited_14.mat
│   │       │   │   │   │   │   ├── Inherited_14.mat.meta
│   │       │   │   │   │   │   ├── Inherited_15.mat
│   │       │   │   │   │   │   ├── Inherited_15.mat.meta
│   │       │   │   │   │   │   ├── Inherited_16.mat
│   │       │   │   │   │   │   ├── Inherited_16.mat.meta
│   │       │   │   │   │   │   ├── Inherited_17.mat
│   │       │   │   │   │   │   ├── Inherited_17.mat.meta
│   │       │   │   │   │   │   ├── Inherited_18.mat
│   │       │   │   │   │   │   ├── Inherited_18.mat.meta
│   │       │   │   │   │   │   ├── Inherited_19.mat
│   │       │   │   │   │   │   ├── Inherited_19.mat.meta
│   │       │   │   │   │   │   ├── Inherited_2.mat
│   │       │   │   │   │   │   ├── Inherited_2.mat.meta
│   │       │   │   │   │   │   ├── Inherited_20.mat
│   │       │   │   │   │   │   ├── Inherited_20.mat.meta
│   │       │   │   │   │   │   ├── Inherited_21.mat
│   │       │   │   │   │   │   ├── Inherited_21.mat.meta
│   │       │   │   │   │   │   ├── Inherited_22.mat
│   │       │   │   │   │   │   ├── Inherited_22.mat.meta
│   │       │   │   │   │   │   ├── Inherited_23.mat
│   │       │   │   │   │   │   ├── Inherited_23.mat.meta
│   │       │   │   │   │   │   ├── Inherited_24.mat
│   │       │   │   │   │   │   ├── Inherited_24.mat.meta
│   │       │   │   │   │   │   ├── Inherited_25.mat
│   │       │   │   │   │   │   ├── Inherited_25.mat.meta
│   │       │   │   │   │   │   ├── Inherited_3.mat
│   │       │   │   │   │   │   ├── Inherited_3.mat.meta
│   │       │   │   │   │   │   ├── Inherited_4.mat
│   │       │   │   │   │   │   ├── Inherited_4.mat.meta
│   │       │   │   │   │   │   ├── Inherited_5.mat
│   │       │   │   │   │   │   ├── Inherited_5.mat.meta
│   │       │   │   │   │   │   ├── Inherited_6.mat
│   │       │   │   │   │   │   ├── Inherited_6.mat.meta
│   │       │   │   │   │   │   ├── Inherited_7.mat
│   │       │   │   │   │   │   ├── Inherited_7.mat.meta
│   │       │   │   │   │   │   ├── Inherited_8.mat
│   │       │   │   │   │   │   ├── Inherited_8.mat.meta
│   │       │   │   │   │   │   ├── Inherited_9.mat
│   │       │   │   │   │   │   ├── Inherited_9.mat.meta
│   │       │   │   │   │   │   ├── Machined graphite alloy.mat
│   │       │   │   │   │   │   ├── Machined graphite alloy.mat.meta
│   │       │   │   │   │   │   ├── Opaque protective visor glazing.mat
│   │       │   │   │   │   │   ├── Opaque protective visor glazing.mat.meta
│   │       │   │   │   │   │   ├── Pressure shell ceramic composite.mat
│   │       │   │   │   │   │   ├── Pressure shell ceramic composite.mat.meta
│   │       │   │   │   │   │   ├── Restrained identification orange.mat
│   │       │   │   │   │   │   └── Restrained identification orange.mat.meta
│   │       │   │   │   │   ├── Meshes/
│   │       │   │   │   │   │   ├── ChamferedEnclosure.asset
│   │       │   │   │   │   │   ├── ChamferedEnclosure.asset.meta
│   │       │   │   │   │   │   ├── CommsTruss.asset
│   │       │   │   │   │   │   ├── CommsTruss.asset.meta
│   │       │   │   │   │   │   ├── Crew_Dome_sealed_airlock_Rails.asset
│   │       │   │   │   │   │   ├── Crew_Dome_sealed_airlock_Rails.asset.meta
│   │       │   │   │   │   │   ├── CrewCabEnclosure.asset
│   │       │   │   │   │   │   ├── CrewCabEnclosure.asset.meta
│   │       │   │   │   │   │   ├── GeodesicFrame.asset
│   │       │   │   │   │   │   ├── GeodesicFrame.asset.meta
│   │       │   │   │   │   │   ├── GeodesicPanels.asset
│   │       │   │   │   │   │   ├── GeodesicPanels.asset.meta
│   │       │   │   │   │   │   ├── Laboratory_Dome_sealed_airlock_Rails.asset
│   │       │   │   │   │   │   ├── Laboratory_Dome_sealed_airlock_Rails.asset.meta
│   │       │   │   │   │   │   ├── Life_Support_Dome_sealed_airlock_Rails.asset
│   │       │   │   │   │   │   ├── Life_Support_Dome_sealed_airlock_Rails.asset.meta
│   │       │   │   │   │   │   ├── RoverMechanicalFrame.asset
│   │       │   │   │   │   │   ├── RoverMechanicalFrame.asset.meta
│   │       │   │   │   │   │   ├── RoverWheel_LOD0.asset
│   │       │   │   │   │   │   ├── RoverWheel_LOD0.asset.meta
│   │       │   │   │   │   │   ├── RoverWheel_LOD1.asset
│   │       │   │   │   │   │   ├── RoverWheel_LOD1.asset.meta
│   │       │   │   │   │   │   ├── RoverWheel_LOD2.asset
│   │       │   │   │   │   │   └── RoverWheel_LOD2.asset.meta
│   │       │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   ├── Mars_ProductionVisual_v1.unity
│   │       │   │   │   │   │   └── Mars_ProductionVisual_v1.unity.meta
│   │       │   │   │   │   ├── Scripts/
│   │       │   │   │   │   │   ├── ProductionCameraRig.cs
│   │       │   │   │   │   │   ├── ProductionCameraRig.cs.meta
│   │       │   │   │   │   │   ├── ProductionModeCoordinator.cs
│   │       │   │   │   │   │   ├── ProductionModeCoordinator.cs.meta
│   │       │   │   │   │   │   ├── ProductionPerformanceProbe.cs
│   │       │   │   │   │   │   ├── ProductionPerformanceProbe.cs.meta
│   │       │   │   │   │   │   ├── ProductionPresentation.cs
│   │       │   │   │   │   │   └── ProductionPresentation.cs.meta
│   │       │   │   │   │   ├── Settings/
│   │       │   │   │   │   │   ├── Lighting_0.asset
│   │       │   │   │   │   │   ├── Lighting_0.asset.meta
│   │       │   │   │   │   │   ├── Lighting_1.asset
│   │       │   │   │   │   │   ├── Lighting_1.asset.meta
│   │       │   │   │   │   │   ├── Lighting_2.asset
│   │       │   │   │   │   │   ├── Lighting_2.asset.meta
│   │       │   │   │   │   │   ├── Lighting_3.asset
│   │       │   │   │   │   │   ├── Lighting_3.asset.meta
│   │       │   │   │   │   │   ├── ProductionMissionRegistry.asset
│   │       │   │   │   │   │   └── ProductionMissionRegistry.asset.meta
│   │       │   │   │   │   ├── Tests/
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── ProductionMeshFactory.cs
│   │       │   │   │   │   │   │   ├── ProductionMeshFactory.cs.meta
│   │       │   │   │   │   │   │   ├── ProductionViewport.cs
│   │       │   │   │   │   │   │   ├── ProductionViewport.cs.meta
│   │       │   │   │   │   │   │   ├── ProductionVisualBuilder.cs
│   │       │   │   │   │   │   │   └── ProductionVisualBuilder.cs.meta
│   │       │   │   │   │   │   └── Editor.meta
│   │       │   │   │   │   ├── Textures/
│   │       │   │   │   │   │   ├── rock_07_MetallicSmoothness.png
│   │       │   │   │   │   │   ├── rock_07_MetallicSmoothness.png.meta
│   │       │   │   │   │   │   ├── rock_09_MetallicSmoothness.png
│   │       │   │   │   │   │   ├── rock_09_MetallicSmoothness.png.meta
│   │       │   │   │   │   │   ├── sandy_gravel_MetallicSmoothness.png
│   │       │   │   │   │   │   └── sandy_gravel_MetallicSmoothness.png.meta
│   │       │   │   │   │   ├── Themes/
│   │       │   │   │   │   │   ├── ProductionHUDTheme.asset
│   │       │   │   │   │   │   ├── ProductionHUDTheme.asset.meta
│   │       │   │   │   │   │   ├── ProductionSky.mat
│   │       │   │   │   │   │   ├── ProductionSky.mat.meta
│   │       │   │   │   │   │   ├── ProductionVolume.asset
│   │       │   │   │   │   │   └── ProductionVolume.asset.meta
│   │       │   │   │   │   ├── Documentation.meta
│   │       │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   ├── Meshes.meta
│   │       │   │   │   │   ├── Prefabs.meta
│   │       │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   ├── Scripts.meta
│   │       │   │   │   │   ├── Settings.meta
│   │       │   │   │   │   ├── Tests.meta
│   │       │   │   │   │   ├── Textures.meta
│   │       │   │   │   │   └── Themes.meta
│   │       │   │   │   ├── Scenes/
│   │       │   │   │   │   ├── MarsOutpost_Environment_v01.unity
│   │       │   │   │   │   └── MarsOutpost_Environment_v01.unity.meta
│   │       │   │   │   ├── Stage08B/
│   │       │   │   │   │   ├── Animation/
│   │       │   │   │   │   │   ├── Idle.anim
│   │       │   │   │   │   │   ├── Idle.anim.meta
│   │       │   │   │   │   │   ├── PrototypeLocomotion.controller
│   │       │   │   │   │   │   ├── PrototypeLocomotion.controller.meta
│   │       │   │   │   │   │   ├── Run.anim
│   │       │   │   │   │   │   ├── Run.anim.meta
│   │       │   │   │   │   │   ├── Walk.anim
│   │       │   │   │   │   │   └── Walk.anim.meta
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── Stage08BBuilder.cs
│   │       │   │   │   │   │   └── Stage08BBuilder.cs.meta
│   │       │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   ├── 08B_Accent_Cyan.mat
│   │       │   │   │   │   │   ├── 08B_Accent_Cyan.mat.meta
│   │       │   │   │   │   │   ├── 08B_Accent_SafetyOrange.mat
│   │       │   │   │   │   │   ├── 08B_Accent_SafetyOrange.mat.meta
│   │       │   │   │   │   │   ├── 08B_Glass_Utility.mat
│   │       │   │   │   │   │   ├── 08B_Glass_Utility.mat.meta
│   │       │   │   │   │   │   ├── 08B_Habitat_CompositeDark.mat
│   │       │   │   │   │   │   ├── 08B_Habitat_CompositeDark.mat.meta
│   │       │   │   │   │   │   ├── 08B_Habitat_OffWhite.mat
│   │       │   │   │   │   │   ├── 08B_Habitat_OffWhite.mat.meta
│   │       │   │   │   │   │   ├── 08B_Mars_Regolith.mat
│   │       │   │   │   │   │   ├── 08B_Mars_Regolith.mat.meta
│   │       │   │   │   │   │   ├── 08B_Mars_Regolith_Dark.mat
│   │       │   │   │   │   │   ├── 08B_Mars_Regolith_Dark.mat.meta
│   │       │   │   │   │   │   ├── 08B_Mars_Rock_Basaltic.mat
│   │       │   │   │   │   │   ├── 08B_Mars_Rock_Basaltic.mat.meta
│   │       │   │   │   │   │   ├── 08B_Mars_Rock_Layered.mat
│   │       │   │   │   │   │   ├── 08B_Mars_Rock_Layered.mat.meta
│   │       │   │   │   │   │   ├── 08B_Metal_Industrial.mat
│   │       │   │   │   │   │   ├── 08B_Metal_Industrial.mat.meta
│   │       │   │   │   │   │   ├── 08B_SolarPanel.mat
│   │       │   │   │   │   │   ├── 08B_SolarPanel.mat.meta
│   │       │   │   │   │   │   ├── BasaltVariationA.mat
│   │       │   │   │   │   │   ├── BasaltVariationA.mat.meta
│   │       │   │   │   │   │   ├── BasaltVariationB.mat
│   │       │   │   │   │   │   ├── BasaltVariationB.mat.meta
│   │       │   │   │   │   │   ├── BasaltVariationC.mat
│   │       │   │   │   │   │   ├── BasaltVariationC.mat.meta
│   │       │   │   │   │   │   ├── Dust_Particle.mat
│   │       │   │   │   │   │   ├── Dust_Particle.mat.meta
│   │       │   │   │   │   │   ├── Identification_Cyan.mat
│   │       │   │   │   │   │   ├── Identification_Cyan.mat.meta
│   │       │   │   │   │   │   ├── Safety_Orange.mat
│   │       │   │   │   │   │   ├── Safety_Orange.mat.meta
│   │       │   │   │   │   │   ├── Suit_Graphite.mat
│   │       │   │   │   │   │   ├── Suit_Graphite.mat.meta
│   │       │   │   │   │   │   ├── Suit_Pale.mat
│   │       │   │   │   │   │   └── Suit_Pale.mat.meta
│   │       │   │   │   │   ├── Animation.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── MarsOutpost_Playable_Stage08B.unity
│   │       │   │   │   │   ├── MarsOutpost_Playable_Stage08B.unity.meta
│   │       │   │   │   │   ├── MarsPrototype.cs
│   │       │   │   │   │   ├── MarsPrototype.cs.meta
│   │       │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   ├── Stage08B_MobilePreview.asset
│   │       │   │   │   │   └── Stage08B_MobilePreview.asset.meta
│   │       │   │   │   ├── Stage08C/
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── AndroidProbeBuild.cs
│   │       │   │   │   │   │   ├── AndroidProbeBuild.cs.meta
│   │       │   │   │   │   │   ├── TouchChecks.cs
│   │       │   │   │   │   │   └── TouchChecks.cs.meta
│   │       │   │   │   │   ├── AndroidProfileProbe.cs
│   │       │   │   │   │   ├── AndroidProfileProbe.cs.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── MobileControls.cs
│   │       │   │   │   │   └── MobileControls.cs.meta
│   │       │   │   │   ├── Stage08C1/
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── EditorPerformanceObserver.cs
│   │       │   │   │   │   │   ├── EditorPerformanceObserver.cs.meta
│   │       │   │   │   │   │   ├── ManualInputObserver.cs
│   │       │   │   │   │   │   └── ManualInputObserver.cs.meta
│   │       │   │   │   │   └── Editor.meta
│   │       │   │   │   ├── VehicleShowroom/
│   │       │   │   │   │   ├── ARES_VI/
│   │       │   │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   │   ├── ARES_Mat_CanopyGlass.mat
│   │       │   │   │   │   │   │   ├── ARES_Mat_CanopyGlass.mat.meta
│   │       │   │   │   │   │   │   ├── ARES_Mat_CarbonTreads.mat
│   │       │   │   │   │   │   │   ├── ARES_Mat_CarbonTreads.mat.meta
│   │       │   │   │   │   │   │   ├── ARES_Mat_CeramicWhite.mat
│   │       │   │   │   │   │   │   ├── ARES_Mat_CeramicWhite.mat.meta
│   │       │   │   │   │   │   │   ├── ARES_Mat_CyanEnergy.mat
│   │       │   │   │   │   │   │   ├── ARES_Mat_CyanEnergy.mat.meta
│   │       │   │   │   │   │   │   ├── ARES_Mat_GoldFoil.mat
│   │       │   │   │   │   │   │   ├── ARES_Mat_GoldFoil.mat.meta
│   │       │   │   │   │   │   │   ├── ARES_Mat_GraphiteTrim.mat
│   │       │   │   │   │   │   │   ├── ARES_Mat_GraphiteTrim.mat.meta
│   │       │   │   │   │   │   │   ├── ARES_Mat_MartianOrange.mat
│   │       │   │   │   │   │   │   ├── ARES_Mat_MartianOrange.mat.meta
│   │       │   │   │   │   │   │   ├── ARES_Mat_TitaniumFrame.mat
│   │       │   │   │   │   │   │   ├── ARES_Mat_TitaniumFrame.mat.meta
│   │       │   │   │   │   │   │   ├── Showroom_Floor.mat
│   │       │   │   │   │   │   │   └── Showroom_Floor.mat.meta
│   │       │   │   │   │   │   ├── Source/
│   │       │   │   │   │   │   │   ├── ares_vi_aaa_lod1.obj
│   │       │   │   │   │   │   │   ├── ares_vi_aaa_lod1.obj.meta
│   │       │   │   │   │   │   │   ├── ares_vi_aaa_lod2.obj
│   │       │   │   │   │   │   │   ├── ares_vi_aaa_lod2.obj.meta
│   │       │   │   │   │   │   │   ├── ares_vi_aaa_master.glb
│   │       │   │   │   │   │   │   ├── ares_vi_aaa_master.glb.meta
│   │       │   │   │   │   │   │   ├── ares_vi_aaa_master.mtl
│   │       │   │   │   │   │   │   ├── ares_vi_aaa_master.mtl.meta
│   │       │   │   │   │   │   │   ├── ares_vi_aaa_master.obj
│   │       │   │   │   │   │   │   └── ares_vi_aaa_master.obj.meta
│   │       │   │   │   │   │   ├── Upgraded/
│   │       │   │   │   │   │   │   ├── Animation/
│   │       │   │   │   │   │   │   │   ├── ARES_ArmDeploy.anim
│   │       │   │   │   │   │   │   │   ├── ARES_ArmDeploy.anim.meta
│   │       │   │   │   │   │   │   │   ├── ARES_ChemCamScan.anim
│   │       │   │   │   │   │   │   │   ├── ARES_ChemCamScan.anim.meta
│   │       │   │   │   │   │   │   │   ├── ARES_HoverActivation.anim
│   │       │   │   │   │   │   │   │   ├── ARES_HoverActivation.anim.meta
│   │       │   │   │   │   │   │   │   ├── ARES_HoverStabilization.anim
│   │       │   │   │   │   │   │   │   ├── ARES_HoverStabilization.anim.meta
│   │       │   │   │   │   │   │   │   ├── ARES_Landing.anim
│   │       │   │   │   │   │   │   │   ├── ARES_Landing.anim.meta
│   │       │   │   │   │   │   │   │   ├── ARES_RockerBogie.anim
│   │       │   │   │   │   │   │   │   ├── ARES_RockerBogie.anim.meta
│   │       │   │   │   │   │   │   │   ├── ARES_VI_Upgraded.controller
│   │       │   │   │   │   │   │   │   ├── ARES_VI_Upgraded.controller.meta
│   │       │   │   │   │   │   │   │   ├── ARES_WheelRotation.anim
│   │       │   │   │   │   │   │   │   └── ARES_WheelRotation.anim.meta
│   │       │   │   │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   │   │   ├── ARES2_CyanEmissive.mat
│   │       │   │   │   │   │   │   │   ├── ARES2_CyanEmissive.mat.meta
│   │       │   │   │   │   │   │   │   ├── ARES2_Glass.mat
│   │       │   │   │   │   │   │   │   ├── ARES2_Glass.mat.meta
│   │       │   │   │   │   │   │   │   ├── ARES2_GoldFoil.mat
│   │       │   │   │   │   │   │   │   ├── ARES2_GoldFoil.mat.meta
│   │       │   │   │   │   │   │   │   ├── ARES2_Graphite.mat
│   │       │   │   │   │   │   │   │   ├── ARES2_Graphite.mat.meta
│   │       │   │   │   │   │   │   │   ├── ARES2_HoverPlasma.mat
│   │       │   │   │   │   │   │   │   ├── ARES2_HoverPlasma.mat.meta
│   │       │   │   │   │   │   │   │   ├── ARES2_LED.mat
│   │       │   │   │   │   │   │   │   ├── ARES2_LED.mat.meta
│   │       │   │   │   │   │   │   │   ├── ARES2_MarsRegolith.mat
│   │       │   │   │   │   │   │   │   ├── ARES2_MarsRegolith.mat.meta
│   │       │   │   │   │   │   │   │   ├── ARES2_MarsSky.mat
│   │       │   │   │   │   │   │   │   ├── ARES2_MarsSky.mat.meta
│   │       │   │   │   │   │   │   │   ├── ARES2_PaintOrange.mat
│   │       │   │   │   │   │   │   │   ├── ARES2_PaintOrange.mat.meta
│   │       │   │   │   │   │   │   │   ├── ARES2_PaintWhite.mat
│   │       │   │   │   │   │   │   │   ├── ARES2_PaintWhite.mat.meta
│   │       │   │   │   │   │   │   │   ├── ARES2_Plume.mat
│   │       │   │   │   │   │   │   │   ├── ARES2_Plume.mat.meta
│   │       │   │   │   │   │   │   │   ├── ARES2_Rubber.mat
│   │       │   │   │   │   │   │   │   ├── ARES2_Rubber.mat.meta
│   │       │   │   │   │   │   │   │   ├── ARES2_Titanium.mat
│   │       │   │   │   │   │   │   │   └── ARES2_Titanium.mat.meta
│   │       │   │   │   │   │   │   ├── Textures/
│   │       │   │   │   │   │   │   │   ├── ARES_GoldFoil_Albedo.png
│   │       │   │   │   │   │   │   │   ├── ARES_GoldFoil_Albedo.png.meta
│   │       │   │   │   │   │   │   │   ├── ARES_GoldFoil_AO.png
│   │       │   │   │   │   │   │   │   ├── ARES_GoldFoil_AO.png.meta
│   │       │   │   │   │   │   │   │   ├── ARES_GoldFoil_MetalSmooth.png
│   │       │   │   │   │   │   │   │   ├── ARES_GoldFoil_MetalSmooth.png.meta
│   │       │   │   │   │   │   │   │   ├── ARES_GoldFoil_Normal.png
│   │       │   │   │   │   │   │   │   ├── ARES_GoldFoil_Normal.png.meta
│   │       │   │   │   │   │   │   │   ├── ARES_Graphite_Albedo.png
│   │       │   │   │   │   │   │   │   ├── ARES_Graphite_Albedo.png.meta
│   │       │   │   │   │   │   │   │   ├── ARES_Graphite_AO.png
│   │       │   │   │   │   │   │   │   ├── ARES_Graphite_AO.png.meta
│   │       │   │   │   │   │   │   │   ├── ARES_Graphite_MetalSmooth.png
│   │       │   │   │   │   │   │   │   ├── ARES_Graphite_MetalSmooth.png.meta
│   │       │   │   │   │   │   │   │   ├── ARES_Graphite_Normal.png
│   │       │   │   │   │   │   │   │   ├── ARES_Graphite_Normal.png.meta
│   │       │   │   │   │   │   │   │   ├── ARES_PaintOrange_Albedo.png
│   │       │   │   │   │   │   │   │   ├── ARES_PaintOrange_Albedo.png.meta
│   │       │   │   │   │   │   │   │   ├── ARES_PaintOrange_AO.png
│   │       │   │   │   │   │   │   │   ├── ARES_PaintOrange_AO.png.meta
│   │       │   │   │   │   │   │   │   ├── ARES_PaintOrange_MetalSmooth.png
│   │       │   │   │   │   │   │   │   ├── ARES_PaintOrange_MetalSmooth.png.meta
│   │       │   │   │   │   │   │   │   ├── ARES_PaintOrange_Normal.png
│   │       │   │   │   │   │   │   │   ├── ARES_PaintOrange_Normal.png.meta
│   │       │   │   │   │   │   │   │   ├── ARES_PaintWhite_Albedo.png
│   │       │   │   │   │   │   │   │   ├── ARES_PaintWhite_Albedo.png.meta
│   │       │   │   │   │   │   │   │   ├── ARES_PaintWhite_AO.png
│   │       │   │   │   │   │   │   │   ├── ARES_PaintWhite_AO.png.meta
│   │       │   │   │   │   │   │   │   ├── ARES_PaintWhite_MetalSmooth.png
│   │       │   │   │   │   │   │   │   ├── ARES_PaintWhite_MetalSmooth.png.meta
│   │       │   │   │   │   │   │   │   ├── ARES_PaintWhite_Normal.png
│   │       │   │   │   │   │   │   │   ├── ARES_PaintWhite_Normal.png.meta
│   │       │   │   │   │   │   │   │   ├── ARES_Regolith_Albedo.png
│   │       │   │   │   │   │   │   │   ├── ARES_Regolith_Albedo.png.meta
│   │       │   │   │   │   │   │   │   ├── ARES_Regolith_AO.png
│   │       │   │   │   │   │   │   │   ├── ARES_Regolith_AO.png.meta
│   │       │   │   │   │   │   │   │   ├── ARES_Regolith_MetalSmooth.png
│   │       │   │   │   │   │   │   │   ├── ARES_Regolith_MetalSmooth.png.meta
│   │       │   │   │   │   │   │   │   ├── ARES_Regolith_Normal.png
│   │       │   │   │   │   │   │   │   ├── ARES_Regolith_Normal.png.meta
│   │       │   │   │   │   │   │   │   ├── ARES_Rubber_Albedo.png
│   │       │   │   │   │   │   │   │   ├── ARES_Rubber_Albedo.png.meta
│   │       │   │   │   │   │   │   │   ├── ARES_Rubber_AO.png
│   │       │   │   │   │   │   │   │   ├── ARES_Rubber_AO.png.meta
│   │       │   │   │   │   │   │   │   ├── ARES_Rubber_MetalSmooth.png
│   │       │   │   │   │   │   │   │   ├── ARES_Rubber_MetalSmooth.png.meta
│   │       │   │   │   │   │   │   │   ├── ARES_Rubber_Normal.png
│   │       │   │   │   │   │   │   │   ├── ARES_Rubber_Normal.png.meta
│   │       │   │   │   │   │   │   │   ├── ARES_Titanium_Albedo.png
│   │       │   │   │   │   │   │   │   ├── ARES_Titanium_Albedo.png.meta
│   │       │   │   │   │   │   │   │   ├── ARES_Titanium_AO.png
│   │       │   │   │   │   │   │   │   ├── ARES_Titanium_AO.png.meta
│   │       │   │   │   │   │   │   │   ├── ARES_Titanium_MetalSmooth.png
│   │       │   │   │   │   │   │   │   ├── ARES_Titanium_MetalSmooth.png.meta
│   │       │   │   │   │   │   │   │   ├── ARES_Titanium_Normal.png
│   │       │   │   │   │   │   │   │   └── ARES_Titanium_Normal.png.meta
│   │       │   │   │   │   │   │   ├── Animation.meta
│   │       │   │   │   │   │   │   ├── ARES_Showroom_Volume.asset
│   │       │   │   │   │   │   │   ├── ARES_Showroom_Volume.asset.meta
│   │       │   │   │   │   │   │   ├── ARES_VI_Upgraded.prefab
│   │       │   │   │   │   │   │   ├── ARES_VI_Upgraded.prefab.meta
│   │       │   │   │   │   │   │   ├── ARES_VI_Upgraded_Meshes.asset
│   │       │   │   │   │   │   │   ├── ARES_VI_Upgraded_Meshes.asset.meta
│   │       │   │   │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   │   │   └── Textures.meta
│   │       │   │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   │   ├── Source.meta
│   │       │   │   │   │   │   └── Upgraded.meta
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── AresViShowroom.cs
│   │       │   │   │   │   │   ├── AresViShowroom.cs.meta
│   │       │   │   │   │   │   ├── AresViUpgrade.cs
│   │       │   │   │   │   │   ├── AresViUpgrade.cs.meta
│   │       │   │   │   │   │   ├── AresViUpgradeVerify.cs
│   │       │   │   │   │   │   └── AresViUpgradeVerify.cs.meta
│   │       │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   ├── ARES_VI_Showroom.unity
│   │       │   │   │   │   │   └── ARES_VI_Showroom.unity.meta
│   │       │   │   │   │   ├── ARES_VI.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   └── Scenes.meta
│   │       │   │   │   ├── Verification/
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── LaptopPlaytest.cs
│   │       │   │   │   │   │   ├── LaptopPlaytest.cs.meta
│   │       │   │   │   │   │   ├── Step1GameplayVerification.cs
│   │       │   │   │   │   │   ├── Step1GameplayVerification.cs.meta
│   │       │   │   │   │   │   ├── Step3BackendClientTest.cs
│   │       │   │   │   │   │   ├── Step3BackendClientTest.cs.meta
│   │       │   │   │   │   │   ├── Step4RulesTest.cs
│   │       │   │   │   │   │   └── Step4RulesTest.cs.meta
│   │       │   │   │   │   └── Editor.meta
│   │       │   │   │   ├── VisualUpgrade/
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── VisualUpgradeBuilder.cs
│   │       │   │   │   │   │   ├── VisualUpgradeBuilder.cs.meta
│   │       │   │   │   │   │   ├── VisualUpgradeCapture.cs
│   │       │   │   │   │   │   ├── VisualUpgradeCapture.cs.meta
│   │       │   │   │   │   │   ├── VisualUpgradeCharacters.cs
│   │       │   │   │   │   │   ├── VisualUpgradeCharacters.cs.meta
│   │       │   │   │   │   │   ├── VisualUpgradeEnvironment.cs
│   │       │   │   │   │   │   ├── VisualUpgradeEnvironment.cs.meta
│   │       │   │   │   │   │   ├── VisualUpgradeKit.cs
│   │       │   │   │   │   │   ├── VisualUpgradeKit.cs.meta
│   │       │   │   │   │   │   ├── VisualUpgradePlaytest.cs
│   │       │   │   │   │   │   ├── VisualUpgradePlaytest.cs.meta
│   │       │   │   │   │   │   ├── VisualUpgradeProps.cs
│   │       │   │   │   │   │   ├── VisualUpgradeProps.cs.meta
│   │       │   │   │   │   │   ├── VisualUpgradeResponsiveAudit.cs
│   │       │   │   │   │   │   └── VisualUpgradeResponsiveAudit.cs.meta
│   │       │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   ├── VisualUpgradeHaze.mat
│   │       │   │   │   │   │   ├── VisualUpgradeHaze.mat.meta
│   │       │   │   │   │   │   ├── VisualUpgradeMarsGround.mat
│   │       │   │   │   │   │   ├── VisualUpgradeMarsGround.mat.meta
│   │       │   │   │   │   │   ├── VisualUpgradeMarsSky.mat
│   │       │   │   │   │   │   ├── VisualUpgradeMarsSky.mat.meta
│   │       │   │   │   │   │   ├── VisualUpgradePhotovoltaic.mat
│   │       │   │   │   │   │   ├── VisualUpgradePhotovoltaic.mat.meta
│   │       │   │   │   │   │   ├── VisualUpgradeRock07.mat
│   │       │   │   │   │   │   ├── VisualUpgradeRock07.mat.meta
│   │       │   │   │   │   │   ├── VisualUpgradeRock09.mat
│   │       │   │   │   │   │   ├── VisualUpgradeRock09.mat.meta
│   │       │   │   │   │   │   ├── VisualUpgradeSky.mat
│   │       │   │   │   │   │   ├── VisualUpgradeSky.mat.meta
│   │       │   │   │   │   │   ├── VisualUpgradeWheelDust.mat
│   │       │   │   │   │   │   ├── VisualUpgradeWheelDust.mat.meta
│   │       │   │   │   │   │   ├── VU_Accent_orange.mat
│   │       │   │   │   │   │   ├── VU_Accent_orange.mat.meta
│   │       │   │   │   │   │   ├── VU_Beacon_red.mat
│   │       │   │   │   │   │   ├── VU_Beacon_red.mat.meta
│   │       │   │   │   │   │   ├── VU_Brushed_aluminium.mat
│   │       │   │   │   │   │   ├── VU_Brushed_aluminium.mat.meta
│   │       │   │   │   │   │   ├── VU_Equipment_white.mat
│   │       │   │   │   │   │   ├── VU_Equipment_white.mat.meta
│   │       │   │   │   │   │   ├── VU_Habitat_marker_light.mat
│   │       │   │   │   │   │   ├── VU_Habitat_marker_light.mat.meta
│   │       │   │   │   │   │   ├── VU_Habitat_shell.mat
│   │       │   │   │   │   │   ├── VU_Habitat_shell.mat.meta
│   │       │   │   │   │   │   ├── VU_Habitat_window_glow.mat
│   │       │   │   │   │   │   ├── VU_Habitat_window_glow.mat.meta
│   │       │   │   │   │   │   ├── VU_Helmet_lamp.mat
│   │       │   │   │   │   │   ├── VU_Helmet_lamp.mat.meta
│   │       │   │   │   │   │   ├── VU_Mission_emblem_(provisional).mat
│   │       │   │   │   │   │   ├── VU_Mission_emblem_(provisional).mat.meta
│   │       │   │   │   │   │   ├── VU_Rover_beacon.mat
│   │       │   │   │   │   │   ├── VU_Rover_beacon.mat.meta
│   │       │   │   │   │   │   ├── VU_Rover_body_panel.mat
│   │       │   │   │   │   │   ├── VU_Rover_body_panel.mat.meta
│   │       │   │   │   │   │   ├── VU_Rover_cockpit_glass.mat
│   │       │   │   │   │   │   ├── VU_Rover_cockpit_glass.mat.meta
│   │       │   │   │   │   │   ├── VU_Rover_graphite.mat
│   │       │   │   │   │   │   ├── VU_Rover_graphite.mat.meta
│   │       │   │   │   │   │   ├── VU_Rover_headlight_lens.mat
│   │       │   │   │   │   │   ├── VU_Rover_headlight_lens.mat.meta
│   │       │   │   │   │   │   ├── VU_Rover_screen.mat
│   │       │   │   │   │   │   ├── VU_Rover_screen.mat.meta
│   │       │   │   │   │   │   ├── VU_Rover_seat.mat
│   │       │   │   │   │   │   ├── VU_Rover_seat.mat.meta
│   │       │   │   │   │   │   ├── VU_Rover_solar_cells.mat
│   │       │   │   │   │   │   ├── VU_Rover_solar_cells.mat.meta
│   │       │   │   │   │   │   ├── VU_Rover_tail_light.mat
│   │       │   │   │   │   │   ├── VU_Rover_tail_light.mat.meta
│   │       │   │   │   │   │   ├── VU_Scan_ring.mat
│   │       │   │   │   │   │   ├── VU_Scan_ring.mat.meta
│   │       │   │   │   │   │   ├── VU_Sensor_lens.mat
│   │       │   │   │   │   │   ├── VU_Sensor_lens.mat.meta
│   │       │   │   │   │   │   ├── VU_Status_amber.mat
│   │       │   │   │   │   │   ├── VU_Status_amber.mat.meta
│   │       │   │   │   │   │   ├── VU_Status_cyan.mat
│   │       │   │   │   │   │   ├── VU_Status_cyan.mat.meta
│   │       │   │   │   │   │   ├── VU_Suit_fabric.mat
│   │       │   │   │   │   │   ├── VU_Suit_fabric.mat.meta
│   │       │   │   │   │   │   ├── VU_Suit_hard_shell.mat
│   │       │   │   │   │   │   ├── VU_Suit_hard_shell.mat.meta
│   │       │   │   │   │   │   ├── VU_Suit_joint_bearing.mat
│   │       │   │   │   │   │   ├── VU_Suit_joint_bearing.mat.meta
│   │       │   │   │   │   │   ├── VU_Suit_soft_grey.mat
│   │       │   │   │   │   │   ├── VU_Suit_soft_grey.mat.meta
│   │       │   │   │   │   │   ├── VU_Visor_gold_coating.mat
│   │       │   │   │   │   │   └── VU_Visor_gold_coating.mat.meta
│   │       │   │   │   │   ├── Meshes/
│   │       │   │   │   │   │   ├── VisualUpgradeDish.asset
│   │       │   │   │   │   │   ├── VisualUpgradeDish.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeFarTerrain.asset
│   │       │   │   │   │   │   ├── VisualUpgradeFarTerrain.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeRoverDish.asset
│   │       │   │   │   │   │   ├── VisualUpgradeRoverDish.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_0_0.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_0_0.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_0_1.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_0_1.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_0_2.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_0_2.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_0_3.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_0_3.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_0_4.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_0_4.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_0_5.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_0_5.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_1_0.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_1_0.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_1_1.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_1_1.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_1_2.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_1_2.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_1_3.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_1_3.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_1_4.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_1_4.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_1_5.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_1_5.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_2_0.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_2_0.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_2_1.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_2_1.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_2_2.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_2_2.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_2_3.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_2_3.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_2_4.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_2_4.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_2_5.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_2_5.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_3_0.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_3_0.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_3_1.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_3_1.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_3_2.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_3_2.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_3_3.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_3_3.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_3_4.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_3_4.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_3_5.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_3_5.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_4_0.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_4_0.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_4_1.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_4_1.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_4_2.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_4_2.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_4_3.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_4_3.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_4_4.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_4_4.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_4_5.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_4_5.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_5_0.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_5_0.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_5_1.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_5_1.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_5_2.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_5_2.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_5_3.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_5_3.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_5_4.asset
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_5_4.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeScatter_5_5.asset
│   │       │   │   │   │   │   └── VisualUpgradeScatter_5_5.asset.meta
│   │       │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   ├── Mars_VisualUpgrade_v1.unity
│   │       │   │   │   │   │   └── Mars_VisualUpgrade_v1.unity.meta
│   │       │   │   │   │   ├── Scripts/
│   │       │   │   │   │   │   ├── VisualUpgradeAstronautMotion.cs
│   │       │   │   │   │   │   ├── VisualUpgradeAstronautMotion.cs.meta
│   │       │   │   │   │   │   ├── VisualUpgradeAtmosphere.cs
│   │       │   │   │   │   │   ├── VisualUpgradeAtmosphere.cs.meta
│   │       │   │   │   │   │   ├── VisualUpgradeBlink.cs
│   │       │   │   │   │   │   ├── VisualUpgradeBlink.cs.meta
│   │       │   │   │   │   │   ├── VisualUpgradeCameraRig.cs
│   │       │   │   │   │   │   ├── VisualUpgradeCameraRig.cs.meta
│   │       │   │   │   │   │   ├── VisualUpgradeControlAssist.cs
│   │       │   │   │   │   │   ├── VisualUpgradeControlAssist.cs.meta
│   │       │   │   │   │   │   ├── VisualUpgradeHUDFixes.cs
│   │       │   │   │   │   │   ├── VisualUpgradeHUDFixes.cs.meta
│   │       │   │   │   │   │   ├── VisualUpgradeHUDStyle.cs
│   │       │   │   │   │   │   ├── VisualUpgradeHUDStyle.cs.meta
│   │       │   │   │   │   │   ├── VisualUpgradeRoverPresentation.cs
│   │       │   │   │   │   │   ├── VisualUpgradeRoverPresentation.cs.meta
│   │       │   │   │   │   │   ├── VisualUpgradeStationStatus.cs
│   │       │   │   │   │   │   └── VisualUpgradeStationStatus.cs.meta
│   │       │   │   │   │   ├── Settings/
│   │       │   │   │   │   │   ├── Lighting_0.asset
│   │       │   │   │   │   │   ├── Lighting_0.asset.meta
│   │       │   │   │   │   │   ├── Lighting_1.asset
│   │       │   │   │   │   │   ├── Lighting_1.asset.meta
│   │       │   │   │   │   │   ├── Lighting_2.asset
│   │       │   │   │   │   │   ├── Lighting_2.asset.meta
│   │       │   │   │   │   │   ├── Lighting_3.asset
│   │       │   │   │   │   │   ├── Lighting_3.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgrade_Renderer.asset
│   │       │   │   │   │   │   ├── VisualUpgrade_Renderer.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgrade_URP.asset
│   │       │   │   │   │   │   ├── VisualUpgrade_URP.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeHUDTheme.asset
│   │       │   │   │   │   │   ├── VisualUpgradeHUDTheme.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeMissionRegistry.asset
│   │       │   │   │   │   │   ├── VisualUpgradeMissionRegistry.asset.meta
│   │       │   │   │   │   │   ├── VisualUpgradeVolume.asset
│   │       │   │   │   │   │   └── VisualUpgradeVolume.asset.meta
│   │       │   │   │   │   ├── Shaders/
│   │       │   │   │   │   │   ├── MarsSky.shader
│   │       │   │   │   │   │   ├── MarsSky.shader.meta
│   │       │   │   │   │   │   ├── MarsTerrain.shader
│   │       │   │   │   │   │   └── MarsTerrain.shader.meta
│   │       │   │   │   │   ├── Textures/
│   │       │   │   │   │   │   ├── VisualUpgradeEmblem.png
│   │       │   │   │   │   │   ├── VisualUpgradeEmblem.png.meta
│   │       │   │   │   │   │   ├── VisualUpgradeSoftDust.png
│   │       │   │   │   │   │   └── VisualUpgradeSoftDust.png.meta
│   │       │   │   │   │   ├── Documentation.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   ├── Meshes.meta
│   │       │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   ├── Scripts.meta
│   │       │   │   │   │   ├── Settings.meta
│   │       │   │   │   │   ├── Shaders.meta
│   │       │   │   │   │   └── Textures.meta
│   │       │   │   │   ├── CinematicHUD.meta
│   │       │   │   │   ├── Environment.meta
│   │       │   │   │   ├── FirstMission.meta
│   │       │   │   │   ├── Greybox.meta
│   │       │   │   │   ├── Integration.meta
│   │       │   │   │   ├── MarsEnvironment.meta
│   │       │   │   │   ├── MarsVisualPolish.meta
│   │       │   │   │   ├── Mission01Milestone.meta
│   │       │   │   │   ├── MissionIntegration.meta
│   │       │   │   │   ├── MissionRules.meta
│   │       │   │   │   ├── NativeControlSlice.meta
│   │       │   │   │   ├── ProductionVisual.meta
│   │       │   │   │   ├── Scenes.meta
│   │       │   │   │   ├── Stage08B.meta
│   │       │   │   │   ├── Stage08C.meta
│   │       │   │   │   ├── Stage08C1.meta
│   │       │   │   │   ├── VehicleShowroom.meta
│   │       │   │   │   ├── Verification.meta
│   │       │   │   │   └── VisualUpgrade.meta
│   │       │   │   ├── Scenes/
│   │       │   │   │   ├── SampleScene.unity
│   │       │   │   │   └── SampleScene.unity.meta
│   │       │   │   ├── Settings/
│   │       │   │   │   ├── DefaultVolumeProfile.asset
│   │       │   │   │   ├── DefaultVolumeProfile.asset.meta
│   │       │   │   │   ├── Mobile_Renderer.asset
│   │       │   │   │   ├── Mobile_Renderer.asset.meta
│   │       │   │   │   ├── Mobile_RPAsset.asset
│   │       │   │   │   ├── Mobile_RPAsset.asset.meta
│   │       │   │   │   ├── PC_Renderer.asset
│   │       │   │   │   ├── PC_Renderer.asset.meta
│   │       │   │   │   ├── PC_RPAsset.asset
│   │       │   │   │   ├── PC_RPAsset.asset.meta
│   │       │   │   │   ├── SampleSceneProfile.asset
│   │       │   │   │   ├── SampleSceneProfile.asset.meta
│   │       │   │   │   ├── UniversalRenderPipelineGlobalSettings.asset
│   │       │   │   │   └── UniversalRenderPipelineGlobalSettings.asset.meta
│   │       │   │   ├── TutorialInfo/
│   │       │   │   │   ├── Editor/
│   │       │   │   │   │   ├── ReadmeEditor.cs
│   │       │   │   │   │   └── ReadmeEditor.cs.meta
│   │       │   │   │   ├── Icons/
│   │       │   │   │   │   ├── URP.png
│   │       │   │   │   │   └── URP.png.meta
│   │       │   │   │   ├── StyleSheets/
│   │       │   │   │   │   ├── ReadmeEditor.uss
│   │       │   │   │   │   ├── ReadmeEditor.uss.meta
│   │       │   │   │   │   ├── ReadmeEditorDark.uss
│   │       │   │   │   │   ├── ReadmeEditorDark.uss.meta
│   │       │   │   │   │   ├── ReadmeEditorLight.uss
│   │       │   │   │   │   └── ReadmeEditorLight.uss.meta
│   │       │   │   │   ├── Editor.meta
│   │       │   │   │   ├── Icons.meta
│   │       │   │   │   ├── Layout.wlt
│   │       │   │   │   ├── Layout.wlt.meta
│   │       │   │   │   ├── Readme.cs
│   │       │   │   │   ├── Readme.cs.meta
│   │       │   │   │   └── StyleSheets.meta
│   │       │   │   ├── _Recovery.meta
│   │       │   │   ├── AstroGateway.meta
│   │       │   │   ├── InputSystem_Actions.inputactions
│   │       │   │   ├── InputSystem_Actions.inputactions.meta
│   │       │   │   ├── Readme.asset
│   │       │   │   ├── Readme.asset.meta
│   │       │   │   ├── Scenes.meta
│   │       │   │   ├── Settings.meta
│   │       │   │   └── TutorialInfo.meta
│   │       │   ├── Data/
│   │       │   │   └── Plugins/
│   │       │   │       ├── lib_burst_generated.cpp
│   │       │   │       └── lib_burst_generated.wasm
│   │       │   ├── Packages/
│   │       │   │   ├── manifest.json
│   │       │   │   └── packages-lock.json
│   │       │   ├── ProjectSettings/
│   │       │   │   ├── AudioManager.asset
│   │       │   │   ├── ClusterInputManager.asset
│   │       │   │   ├── DynamicsManager.asset
│   │       │   │   ├── EditorBuildSettings.asset
│   │       │   │   ├── EditorSettings.asset
│   │       │   │   ├── GraphicsSettings.asset
│   │       │   │   ├── InputManager.asset
│   │       │   │   ├── MemorySettings.asset
│   │       │   │   ├── MultiplayerManager.asset
│   │       │   │   ├── NavMeshAreas.asset
│   │       │   │   ├── PackageManagerSettings.asset
│   │       │   │   ├── Physics2DSettings.asset
│   │       │   │   ├── PhysicsCoreProjectSettings2D.asset
│   │       │   │   ├── PresetManager.asset
│   │       │   │   ├── ProjectAuditorSettings.asset
│   │       │   │   ├── ProjectSettings.asset
│   │       │   │   ├── ProjectVersion.txt
│   │       │   │   ├── QualitySettings.asset
│   │       │   │   ├── SceneTemplateSettings.json
│   │       │   │   ├── ShaderGraphSettings.asset
│   │       │   │   ├── TagManager.asset
│   │       │   │   ├── TimeManager.asset
│   │       │   │   ├── UnityConnectSettings.asset
│   │       │   │   ├── URPProjectSettings.asset
│   │       │   │   ├── VersionControlSettings.asset
│   │       │   │   ├── VFXManager.asset
│   │       │   │   └── XRSettings.asset
│   │       │   └── .gitignore
│   │       ├── GameplayVideo/
│   │       │   ├── Project/
│   │       │   │   ├── Assets/
│   │       │   │   │   ├── AstroGateway/
│   │       │   │   │   │   ├── CinematicHUD/
│   │       │   │   │   │   │   ├── Documentation/
│   │       │   │   │   │   │   │   ├── README.md
│   │       │   │   │   │   │   │   └── README.md.meta
│   │       │   │   │   │   │   ├── Prefabs/
│   │       │   │   │   │   │   │   ├── AstroGatewayCinematicHUD.prefab
│   │       │   │   │   │   │   │   └── AstroGatewayCinematicHUD.prefab.meta
│   │       │   │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   │   ├── Mars_CinematicHUD_v1.unity
│   │       │   │   │   │   │   │   └── Mars_CinematicHUD_v1.unity.meta
│   │       │   │   │   │   │   ├── Scripts/
│   │       │   │   │   │   │   │   ├── AstroGatewayHUDController.cs
│   │       │   │   │   │   │   │   ├── AstroGatewayHUDController.cs.meta
│   │       │   │   │   │   │   │   ├── CinematicEnvironmentAdapter.cs
│   │       │   │   │   │   │   │   ├── CinematicEnvironmentAdapter.cs.meta
│   │       │   │   │   │   │   │   ├── CinematicGameAdapter.cs
│   │       │   │   │   │   │   │   ├── CinematicGameAdapter.cs.meta
│   │       │   │   │   │   │   │   ├── HUDInputRouter.cs
│   │       │   │   │   │   │   │   ├── HUDInputRouter.cs.meta
│   │       │   │   │   │   │   │   ├── HUDLayoutProbe.cs
│   │       │   │   │   │   │   │   ├── HUDLayoutProbe.cs.meta
│   │       │   │   │   │   │   │   ├── HUDPerformanceProbe.cs
│   │       │   │   │   │   │   │   ├── HUDPerformanceProbe.cs.meta
│   │       │   │   │   │   │   │   ├── HUDPointerControl.cs
│   │       │   │   │   │   │   │   ├── HUDPointerControl.cs.meta
│   │       │   │   │   │   │   │   ├── HUDTelemetryAdapter.cs
│   │       │   │   │   │   │   │   ├── HUDTelemetryAdapter.cs.meta
│   │       │   │   │   │   │   │   ├── HUDTheme.cs
│   │       │   │   │   │   │   │   └── HUDTheme.cs.meta
│   │       │   │   │   │   │   ├── Settings/
│   │       │   │   │   │   │   │   ├── MissionRegistry.asset
│   │       │   │   │   │   │   │   └── MissionRegistry.asset.meta
│   │       │   │   │   │   │   ├── Tests/
│   │       │   │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   │   ├── CinematicHUDBuilder.cs
│   │       │   │   │   │   │   │   │   ├── CinematicHUDBuilder.cs.meta
│   │       │   │   │   │   │   │   │   ├── HUDResponsiveQA.cs
│   │       │   │   │   │   │   │   │   └── HUDResponsiveQA.cs.meta
│   │       │   │   │   │   │   │   └── Editor.meta
│   │       │   │   │   │   │   ├── Themes/
│   │       │   │   │   │   │   │   ├── AstroGatewayHUDTheme.asset
│   │       │   │   │   │   │   │   └── AstroGatewayHUDTheme.asset.meta
│   │       │   │   │   │   │   ├── UI/
│   │       │   │   │   │   │   │   ├── CircularControl.png
│   │       │   │   │   │   │   │   ├── CircularControl.png.meta
│   │       │   │   │   │   │   │   ├── HeadingArrow.png
│   │       │   │   │   │   │   │   ├── HeadingArrow.png.meta
│   │       │   │   │   │   │   │   ├── OriginalContourMap.png
│   │       │   │   │   │   │   │   ├── OriginalContourMap.png.meta
│   │       │   │   │   │   │   │   ├── RoundedPanel.png
│   │       │   │   │   │   │   │   └── RoundedPanel.png.meta
│   │       │   │   │   │   │   ├── Documentation.meta
│   │       │   │   │   │   │   ├── link.xml
│   │       │   │   │   │   │   ├── link.xml.meta
│   │       │   │   │   │   │   ├── Prefabs.meta
│   │       │   │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   │   ├── Scripts.meta
│   │       │   │   │   │   │   ├── Settings.meta
│   │       │   │   │   │   │   ├── Tests.meta
│   │       │   │   │   │   │   ├── Themes.meta
│   │       │   │   │   │   │   └── UI.meta
│   │       │   │   │   │   ├── Environment/
│   │       │   │   │   │   │   ├── Architecture/
│   │       │   │   │   │   │   │   ├── AgricultureG01_ChamferShell.asset
│   │       │   │   │   │   │   │   ├── AgricultureG01_ChamferShell.asset.meta
│   │       │   │   │   │   │   │   ├── AgricultureG01_Ring.asset
│   │       │   │   │   │   │   │   ├── AgricultureG01_Ring.asset.meta
│   │       │   │   │   │   │   │   ├── AirlockShell.asset
│   │       │   │   │   │   │   │   ├── AirlockShell.asset.meta
│   │       │   │   │   │   │   │   ├── C_ScienceStaging_Accent_Cyan_Combined.asset
│   │       │   │   │   │   │   │   ├── C_ScienceStaging_Accent_Cyan_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── C_ScienceStaging_Accent_SafetyOrange_Combined.asset
│   │       │   │   │   │   │   │   ├── C_ScienceStaging_Accent_SafetyOrange_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── C_ScienceStaging_Glass_Utility_Combined.asset
│   │       │   │   │   │   │   │   ├── C_ScienceStaging_Glass_Utility_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── C_ScienceStaging_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   │   ├── C_ScienceStaging_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── C_ScienceStaging_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   │   ├── C_ScienceStaging_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── C_ScienceStaging_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   │   ├── C_ScienceStaging_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── G_Greenhouse_Accent_Cyan_Combined.asset
│   │       │   │   │   │   │   │   ├── G_Greenhouse_Accent_Cyan_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── G_Greenhouse_Glass_Utility_Combined.asset
│   │       │   │   │   │   │   │   ├── G_Greenhouse_Glass_Utility_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── G_Greenhouse_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   │   ├── G_Greenhouse_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── G_Greenhouse_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   │   ├── G_Greenhouse_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── G_Greenhouse_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   │   ├── G_Greenhouse_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── H_CommandHabitat_Accent_Cyan_Combined.asset
│   │       │   │   │   │   │   │   ├── H_CommandHabitat_Accent_Cyan_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── H_CommandHabitat_Glass_Utility_Combined.asset
│   │       │   │   │   │   │   │   ├── H_CommandHabitat_Glass_Utility_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── H_CommandHabitat_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   │   ├── H_CommandHabitat_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── H_CommandHabitat_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   │   ├── H_CommandHabitat_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── H_CommandHabitat_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   │   ├── H_CommandHabitat_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── HabitatH01_ChamferShell.asset
│   │       │   │   │   │   │   │   ├── HabitatH01_ChamferShell.asset.meta
│   │       │   │   │   │   │   │   ├── HabitatH01_Ring.asset
│   │       │   │   │   │   │   │   ├── HabitatH01_Ring.asset.meta
│   │       │   │   │   │   │   │   ├── P_PowerStation_Accent_Cyan_Combined.asset
│   │       │   │   │   │   │   │   ├── P_PowerStation_Accent_Cyan_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── P_PowerStation_Accent_SafetyOrange_Combined.asset
│   │       │   │   │   │   │   │   ├── P_PowerStation_Accent_SafetyOrange_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── P_PowerStation_Glass_Utility_Combined.asset
│   │       │   │   │   │   │   │   ├── P_PowerStation_Glass_Utility_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── P_PowerStation_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   │   ├── P_PowerStation_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── P_PowerStation_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   │   ├── P_PowerStation_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── P_PowerStation_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   │   ├── P_PowerStation_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── R_RoverBay_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   │   ├── R_RoverBay_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── R_RoverBay_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   │   ├── R_RoverBay_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── R_RoverBay_Mars_Regolith_Dark_Combined.asset
│   │       │   │   │   │   │   │   ├── R_RoverBay_Mars_Regolith_Dark_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── R_RoverBay_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   │   ├── R_RoverBay_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── R_RoverBay_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   │   ├── R_RoverBay_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── S_SolarArray_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   │   ├── S_SolarArray_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── S_SolarArray_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   │   ├── S_SolarArray_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── S_SolarArray_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   │   ├── S_SolarArray_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── S_SolarArray_SolarPanel_Combined.asset
│   │       │   │   │   │   │   │   └── S_SolarArray_SolarPanel_Combined.asset.meta
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── MarsEnvironmentBuilder.cs
│   │       │   │   │   │   │   │   └── MarsEnvironmentBuilder.cs.meta
│   │       │   │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   │   ├── Accent_Cyan.mat
│   │       │   │   │   │   │   │   ├── Accent_Cyan.mat.meta
│   │       │   │   │   │   │   │   ├── Accent_SafetyOrange.mat
│   │       │   │   │   │   │   │   ├── Accent_SafetyOrange.mat.meta
│   │       │   │   │   │   │   │   ├── Basalt_Surface_512.png
│   │       │   │   │   │   │   │   ├── Basalt_Surface_512.png.meta
│   │       │   │   │   │   │   │   ├── Composite_DustPaint_512.png
│   │       │   │   │   │   │   │   ├── Composite_DustPaint_512.png.meta
│   │       │   │   │   │   │   │   ├── Delta_Bedding_512.png
│   │       │   │   │   │   │   │   ├── Delta_Bedding_512.png.meta
│   │       │   │   │   │   │   │   ├── Delta_FracturedStrata_512.png
│   │       │   │   │   │   │   │   ├── Delta_FracturedStrata_512.png.meta
│   │       │   │   │   │   │   │   ├── DustSky.shader
│   │       │   │   │   │   │   │   ├── DustSky.shader.meta
│   │       │   │   │   │   │   │   ├── Glass_Utility.mat
│   │       │   │   │   │   │   │   ├── Glass_Utility.mat.meta
│   │       │   │   │   │   │   │   ├── Habitat_CompositeDark.mat
│   │       │   │   │   │   │   │   ├── Habitat_CompositeDark.mat.meta
│   │       │   │   │   │   │   │   ├── Habitat_OffWhite.mat
│   │       │   │   │   │   │   │   ├── Habitat_OffWhite.mat.meta
│   │       │   │   │   │   │   │   ├── Mars_DustSky.mat
│   │       │   │   │   │   │   │   ├── Mars_DustSky.mat.meta
│   │       │   │   │   │   │   │   ├── Mars_Regolith.mat
│   │       │   │   │   │   │   │   ├── Mars_Regolith.mat.meta
│   │       │   │   │   │   │   │   ├── Mars_Regolith_Dark.mat
│   │       │   │   │   │   │   │   ├── Mars_Regolith_Dark.mat.meta
│   │       │   │   │   │   │   │   ├── Mars_Rock_Basaltic.mat
│   │       │   │   │   │   │   │   ├── Mars_Rock_Basaltic.mat.meta
│   │       │   │   │   │   │   │   ├── Mars_Rock_Layered.mat
│   │       │   │   │   │   │   │   ├── Mars_Rock_Layered.mat.meta
│   │       │   │   │   │   │   │   ├── Metal_Industrial.mat
│   │       │   │   │   │   │   │   ├── Metal_Industrial.mat.meta
│   │       │   │   │   │   │   │   ├── Regolith_Grain_512.png
│   │       │   │   │   │   │   │   ├── Regolith_Grain_512.png.meta
│   │       │   │   │   │   │   │   ├── Regolith_Macro_Pathwear_1024.png
│   │       │   │   │   │   │   │   ├── Regolith_Macro_Pathwear_1024.png.meta
│   │       │   │   │   │   │   │   ├── Regolith_MicroNormal_256.png
│   │       │   │   │   │   │   │   ├── Regolith_MicroNormal_256.png.meta
│   │       │   │   │   │   │   │   ├── Solar_Cells_Dust_512.png
│   │       │   │   │   │   │   │   ├── Solar_Cells_Dust_512.png.meta
│   │       │   │   │   │   │   │   ├── SolarPanel.mat
│   │       │   │   │   │   │   │   └── SolarPanel.mat.meta
│   │       │   │   │   │   │   ├── Prefabs/
│   │       │   │   │   │   │   │   ├── AngularBasalt.prefab
│   │       │   │   │   │   │   │   ├── AngularBasalt.prefab.meta
│   │       │   │   │   │   │   │   ├── C_ScienceStaging.prefab
│   │       │   │   │   │   │   │   ├── C_ScienceStaging.prefab.meta
│   │       │   │   │   │   │   │   ├── ErodedLandmark.prefab
│   │       │   │   │   │   │   │   ├── ErodedLandmark.prefab.meta
│   │       │   │   │   │   │   │   ├── G_Greenhouse.prefab
│   │       │   │   │   │   │   │   ├── G_Greenhouse.prefab.meta
│   │       │   │   │   │   │   │   ├── H_CommandHabitat.prefab
│   │       │   │   │   │   │   │   ├── H_CommandHabitat.prefab.meta
│   │       │   │   │   │   │   │   ├── LayeredDelta.prefab
│   │       │   │   │   │   │   │   ├── LayeredDelta.prefab.meta
│   │       │   │   │   │   │   │   ├── P_PowerStation.prefab
│   │       │   │   │   │   │   │   ├── P_PowerStation.prefab.meta
│   │       │   │   │   │   │   │   ├── R_RoverBay.prefab
│   │       │   │   │   │   │   │   ├── R_RoverBay.prefab.meta
│   │       │   │   │   │   │   │   ├── RegolithStone.prefab
│   │       │   │   │   │   │   │   ├── RegolithStone.prefab.meta
│   │       │   │   │   │   │   │   ├── S_SolarArray.prefab
│   │       │   │   │   │   │   │   └── S_SolarArray.prefab.meta
│   │       │   │   │   │   │   ├── Props/
│   │       │   │   │   │   │   │   ├── ApproachStoneClusters_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   │   ├── ApproachStoneClusters_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── HabitatExternalKit_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   │   ├── HabitatExternalKit_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── HabitatExternalKit_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   │   ├── HabitatExternalKit_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── HabitatExternalKit_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   │   ├── HabitatExternalKit_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── PowerExternalKit_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   │   ├── PowerExternalKit_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── PowerExternalKit_Mars_Regolith_Dark_Combined.asset
│   │       │   │   │   │   │   │   ├── PowerExternalKit_Mars_Regolith_Dark_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── UtilityRoutePosts_Accent_Cyan_Combined.asset
│   │       │   │   │   │   │   │   ├── UtilityRoutePosts_Accent_Cyan_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── UtilityRoutePosts_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   │   └── UtilityRoutePosts_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   ├── Rocks/
│   │       │   │   │   │   │   │   ├── AngularBasalt_LOD0.asset
│   │       │   │   │   │   │   │   ├── AngularBasalt_LOD0.asset.meta
│   │       │   │   │   │   │   │   ├── AngularBasalt_LOD1.asset
│   │       │   │   │   │   │   │   ├── AngularBasalt_LOD1.asset.meta
│   │       │   │   │   │   │   │   ├── AngularBasalt_LOD2.asset
│   │       │   │   │   │   │   │   ├── AngularBasalt_LOD2.asset.meta
│   │       │   │   │   │   │   │   ├── DeltaFractured_LOD0.asset
│   │       │   │   │   │   │   │   ├── DeltaFractured_LOD0.asset.meta
│   │       │   │   │   │   │   │   ├── DeltaFractured_LOD1.asset
│   │       │   │   │   │   │   │   ├── DeltaFractured_LOD1.asset.meta
│   │       │   │   │   │   │   │   ├── DeltaFractured_LOD2.asset
│   │       │   │   │   │   │   │   ├── DeltaFractured_LOD2.asset.meta
│   │       │   │   │   │   │   │   ├── ErodedLandmark_LOD0.asset
│   │       │   │   │   │   │   │   ├── ErodedLandmark_LOD0.asset.meta
│   │       │   │   │   │   │   │   ├── ErodedLandmark_LOD1.asset
│   │       │   │   │   │   │   │   ├── ErodedLandmark_LOD1.asset.meta
│   │       │   │   │   │   │   │   ├── ErodedLandmark_LOD2.asset
│   │       │   │   │   │   │   │   ├── ErodedLandmark_LOD2.asset.meta
│   │       │   │   │   │   │   │   ├── LayeredDelta_LOD0.asset
│   │       │   │   │   │   │   │   ├── LayeredDelta_LOD0.asset.meta
│   │       │   │   │   │   │   │   ├── LayeredDelta_LOD1.asset
│   │       │   │   │   │   │   │   ├── LayeredDelta_LOD1.asset.meta
│   │       │   │   │   │   │   │   ├── LayeredDelta_LOD2.asset
│   │       │   │   │   │   │   │   ├── LayeredDelta_LOD2.asset.meta
│   │       │   │   │   │   │   │   ├── StonePatch_0_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   │   ├── StonePatch_0_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── StonePatch_1_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   │   ├── StonePatch_1_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── StonePatch_2_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   │   ├── StonePatch_2_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── StonePatch_3_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   │   ├── StonePatch_3_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── StonePatch_4_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   │   ├── StonePatch_4_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── StonePatch_5_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   │   ├── StonePatch_5_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── StonePatch_6_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   │   ├── StonePatch_6_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── StonePatch_7_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   │   ├── StonePatch_7_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── StonePatch_8_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   │   └── StonePatch_8_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── Terrain/
│   │       │   │   │   │   │   │   ├── Regolith_250m_PreservedCorridor.asset
│   │       │   │   │   │   │   │   └── Regolith_250m_PreservedCorridor.asset.meta
│   │       │   │   │   │   │   ├── Architecture.meta
│   │       │   │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   │   ├── Prefabs.meta
│   │       │   │   │   │   │   ├── Props.meta
│   │       │   │   │   │   │   ├── Rocks.meta
│   │       │   │   │   │   │   └── Terrain.meta
│   │       │   │   │   │   ├── Greybox/
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── MarsGreyboxBuilder.cs
│   │       │   │   │   │   │   │   └── MarsGreyboxBuilder.cs.meta
│   │       │   │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   │   ├── DarkRock.mat
│   │       │   │   │   │   │   │   ├── DarkRock.mat.meta
│   │       │   │   │   │   │   │   ├── HabitatPale.mat
│   │       │   │   │   │   │   │   ├── HabitatPale.mat.meta
│   │       │   │   │   │   │   │   ├── IndustrialGraphite.mat
│   │       │   │   │   │   │   │   ├── IndustrialGraphite.mat.meta
│   │       │   │   │   │   │   │   ├── LayeredStone.mat
│   │       │   │   │   │   │   │   ├── LayeredStone.mat.meta
│   │       │   │   │   │   │   │   ├── SandTan.mat
│   │       │   │   │   │   │   │   ├── SandTan.mat.meta
│   │       │   │   │   │   │   │   ├── TerrainOchre.mat
│   │       │   │   │   │   │   │   ├── TerrainOchre.mat.meta
│   │       │   │   │   │   │   │   ├── WayfindingAmber.mat
│   │       │   │   │   │   │   │   ├── WayfindingAmber.mat.meta
│   │       │   │   │   │   │   │   ├── WayfindingCyan.mat
│   │       │   │   │   │   │   │   └── WayfindingCyan.mat.meta
│   │       │   │   │   │   │   ├── Meshes/
│   │       │   │   │   │   │   │   ├── Ground.asset
│   │       │   │   │   │   │   │   └── Ground.asset.meta
│   │       │   │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   │   ├── MarsOutpost_Greybox.unity
│   │       │   │   │   │   │   ├── MarsOutpost_Greybox.unity.meta
│   │       │   │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   │   ├── Meshes.meta
│   │       │   │   │   │   │   ├── PreGreybox_Untitled_Backup.unity
│   │       │   │   │   │   │   └── PreGreybox_Untitled_Backup.unity.meta
│   │       │   │   │   │   ├── Integration/
│   │       │   │   │   │   │   ├── Contracts/
│   │       │   │   │   │   │   │   ├── IntegrationContracts.cs
│   │       │   │   │   │   │   │   └── IntegrationContracts.cs.meta
│   │       │   │   │   │   │   ├── Http/
│   │       │   │   │   │   │   │   ├── HttpMissionSessionService.cs
│   │       │   │   │   │   │   │   └── HttpMissionSessionService.cs.meta
│   │       │   │   │   │   │   ├── Local/
│   │       │   │   │   │   │   │   ├── GameplayEventBridge.cs
│   │       │   │   │   │   │   │   ├── GameplayEventBridge.cs.meta
│   │       │   │   │   │   │   │   ├── LifeSupportModelAdapter.cs
│   │       │   │   │   │   │   │   ├── LifeSupportModelAdapter.cs.meta
│   │       │   │   │   │   │   │   ├── LocalServices.cs
│   │       │   │   │   │   │   │   └── LocalServices.cs.meta
│   │       │   │   │   │   │   ├── Contracts.meta
│   │       │   │   │   │   │   ├── Http.meta
│   │       │   │   │   │   │   └── Local.meta
│   │       │   │   │   │   ├── MarsEnvironment/
│   │       │   │   │   │   │   ├── Documentation/
│   │       │   │   │   │   │   │   ├── MissionRegistry.asset
│   │       │   │   │   │   │   │   ├── MissionRegistry.asset.meta
│   │       │   │   │   │   │   │   ├── README.md
│   │       │   │   │   │   │   │   └── README.md.meta
│   │       │   │   │   │   │   ├── Lighting/
│   │       │   │   │   │   │   │   ├── Dusty_Conditions.asset
│   │       │   │   │   │   │   │   ├── Dusty_Conditions.asset.meta
│   │       │   │   │   │   │   │   ├── MarsGradient.shader
│   │       │   │   │   │   │   │   ├── MarsGradient.shader.meta
│   │       │   │   │   │   │   │   ├── MarsSky.mat
│   │       │   │   │   │   │   │   ├── MarsSky.mat.meta
│   │       │   │   │   │   │   │   ├── MarsVolume.asset
│   │       │   │   │   │   │   │   ├── MarsVolume.asset.meta
│   │       │   │   │   │   │   │   ├── Midday.asset
│   │       │   │   │   │   │   │   ├── Midday.asset.meta
│   │       │   │   │   │   │   │   ├── Morning.asset
│   │       │   │   │   │   │   │   ├── Morning.asset.meta
│   │       │   │   │   │   │   │   ├── Sunset.asset
│   │       │   │   │   │   │   │   └── Sunset.asset.meta
│   │       │   │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   │   ├── DistantDust.asset
│   │       │   │   │   │   │   │   ├── DistantDust.asset.meta
│   │       │   │   │   │   │   │   ├── Dusty solar cells.mat
│   │       │   │   │   │   │   │   ├── Dusty solar cells.mat.meta
│   │       │   │   │   │   │   │   ├── EnvironmentDust.mat
│   │       │   │   │   │   │   │   ├── EnvironmentDust.mat.meta
│   │       │   │   │   │   │   │   ├── Fine Mars dust.mat
│   │       │   │   │   │   │   │   ├── Fine Mars dust.mat.meta
│   │       │   │   │   │   │   │   ├── Habitat ceramic composite.mat
│   │       │   │   │   │   │   │   ├── Habitat ceramic composite.mat.meta
│   │       │   │   │   │   │   │   ├── Observation panels — opaque approximation.mat
│   │       │   │   │   │   │   │   ├── Observation panels — opaque approximation.mat.meta
│   │       │   │   │   │   │   │   ├── Restrained identification ochre.mat
│   │       │   │   │   │   │   │   ├── Restrained identification ochre.mat.meta
│   │       │   │   │   │   │   │   ├── Rock07 dusty basalt.mat
│   │       │   │   │   │   │   │   ├── Rock07 dusty basalt.mat.meta
│   │       │   │   │   │   │   │   ├── Rock09 compact mineral.mat
│   │       │   │   │   │   │   │   ├── Rock09 compact mineral.mat.meta
│   │       │   │   │   │   │   │   ├── Rover_Cab.mat
│   │       │   │   │   │   │   │   ├── Rover_Cab.mat.meta
│   │       │   │   │   │   │   │   ├── Rover_Chassis.mat
│   │       │   │   │   │   │   │   ├── Rover_Chassis.mat.meta
│   │       │   │   │   │   │   │   ├── Rover_RoofMarker.mat
│   │       │   │   │   │   │   │   ├── Rover_RoofMarker.mat.meta
│   │       │   │   │   │   │   │   ├── Rover_Wheel_-1_0.mat
│   │       │   │   │   │   │   │   ├── Rover_Wheel_-1_0.mat.meta
│   │       │   │   │   │   │   │   ├── Rover_Wheel_-1_1.mat
│   │       │   │   │   │   │   │   ├── Rover_Wheel_-1_1.mat.meta
│   │       │   │   │   │   │   │   ├── Rover_Wheel_-1_2.mat
│   │       │   │   │   │   │   │   ├── Rover_Wheel_-1_2.mat.meta
│   │       │   │   │   │   │   │   ├── Rover_Wheel_1_0.mat
│   │       │   │   │   │   │   │   ├── Rover_Wheel_1_0.mat.meta
│   │       │   │   │   │   │   │   ├── Rover_Wheel_1_1.mat
│   │       │   │   │   │   │   │   ├── Rover_Wheel_1_1.mat.meta
│   │       │   │   │   │   │   │   ├── Rover_Wheel_1_2.mat
│   │       │   │   │   │   │   │   ├── Rover_Wheel_1_2.mat.meta
│   │       │   │   │   │   │   │   ├── Structural graphite.mat
│   │       │   │   │   │   │   │   └── Structural graphite.mat.meta
│   │       │   │   │   │   │   ├── Models/
│   │       │   │   │   │   │   │   ├── Crew_Dome_Ribs_LowPoly.asset
│   │       │   │   │   │   │   │   ├── Crew_Dome_Ribs_LowPoly.asset.meta
│   │       │   │   │   │   │   │   ├── HabitatHemisphere.asset
│   │       │   │   │   │   │   │   ├── HabitatHemisphere.asset.meta
│   │       │   │   │   │   │   │   ├── Laboratory_Dome_Ribs_LowPoly.asset
│   │       │   │   │   │   │   │   ├── Laboratory_Dome_Ribs_LowPoly.asset.meta
│   │       │   │   │   │   │   │   ├── Life_Support_Dome_Ribs_LowPoly.asset
│   │       │   │   │   │   │   │   ├── Life_Support_Dome_Ribs_LowPoly.asset.meta
│   │       │   │   │   │   │   │   ├── rock_07_LOD0.asset
│   │       │   │   │   │   │   │   ├── rock_07_LOD0.asset.meta
│   │       │   │   │   │   │   │   ├── rock_07_LOD1.asset
│   │       │   │   │   │   │   │   ├── rock_07_LOD1.asset.meta
│   │       │   │   │   │   │   │   ├── rock_07_LOD2.asset
│   │       │   │   │   │   │   │   ├── rock_07_LOD2.asset.meta
│   │       │   │   │   │   │   │   ├── rock_09_LOD0.asset
│   │       │   │   │   │   │   │   ├── rock_09_LOD0.asset.meta
│   │       │   │   │   │   │   │   ├── rock_09_LOD1.asset
│   │       │   │   │   │   │   │   ├── rock_09_LOD1.asset.meta
│   │       │   │   │   │   │   │   ├── rock_09_LOD2.asset
│   │       │   │   │   │   │   │   ├── rock_09_LOD2.asset.meta
│   │       │   │   │   │   │   │   ├── Terrain250m.asset
│   │       │   │   │   │   │   │   └── Terrain250m.asset.meta
│   │       │   │   │   │   │   ├── Prefabs/
│   │       │   │   │   │   │   │   ├── ARESYN_ClosedHabitatComplex_Optimized.prefab
│   │       │   │   │   │   │   │   ├── ARESYN_ClosedHabitatComplex_Optimized.prefab.meta
│   │       │   │   │   │   │   │   ├── SolarFarm_Optimized.prefab
│   │       │   │   │   │   │   │   └── SolarFarm_Optimized.prefab.meta
│   │       │   │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   │   ├── Mars_RealisticResearchBase_v1.unity
│   │       │   │   │   │   │   │   └── Mars_RealisticResearchBase_v1.unity.meta
│   │       │   │   │   │   │   ├── Scripts/
│   │       │   │   │   │   │   │   ├── EnvironmentPresentation.cs
│   │       │   │   │   │   │   │   ├── EnvironmentPresentation.cs.meta
│   │       │   │   │   │   │   │   ├── EnvironmentPreset.cs
│   │       │   │   │   │   │   │   ├── EnvironmentPreset.cs.meta
│   │       │   │   │   │   │   │   ├── ResearchTarget.cs
│   │       │   │   │   │   │   │   └── ResearchTarget.cs.meta
│   │       │   │   │   │   │   ├── Tests/
│   │       │   │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   │   ├── MarsResearchBaseBuilder.cs
│   │       │   │   │   │   │   │   │   └── MarsResearchBaseBuilder.cs.meta
│   │       │   │   │   │   │   │   └── Editor.meta
│   │       │   │   │   │   │   ├── Textures/
│   │       │   │   │   │   │   │   ├── rock_07_AO.png
│   │       │   │   │   │   │   │   ├── rock_07_AO.png.meta
│   │       │   │   │   │   │   │   ├── rock_07_Diffuse.png
│   │       │   │   │   │   │   │   ├── rock_07_Diffuse.png.meta
│   │       │   │   │   │   │   │   ├── rock_07_nor_gl.png
│   │       │   │   │   │   │   │   ├── rock_07_nor_gl.png.meta
│   │       │   │   │   │   │   │   ├── rock_07_Rough.png
│   │       │   │   │   │   │   │   ├── rock_07_Rough.png.meta
│   │       │   │   │   │   │   │   ├── rock_09_AO.png
│   │       │   │   │   │   │   │   ├── rock_09_AO.png.meta
│   │       │   │   │   │   │   │   ├── rock_09_Diffuse.png
│   │       │   │   │   │   │   │   ├── rock_09_Diffuse.png.meta
│   │       │   │   │   │   │   │   ├── rock_09_nor_gl.png
│   │       │   │   │   │   │   │   ├── rock_09_nor_gl.png.meta
│   │       │   │   │   │   │   │   ├── rock_09_Rough.png
│   │       │   │   │   │   │   │   ├── rock_09_Rough.png.meta
│   │       │   │   │   │   │   │   ├── sandy_gravel_AO.png
│   │       │   │   │   │   │   │   ├── sandy_gravel_AO.png.meta
│   │       │   │   │   │   │   │   ├── sandy_gravel_Diffuse.png
│   │       │   │   │   │   │   │   ├── sandy_gravel_Diffuse.png.meta
│   │       │   │   │   │   │   │   ├── sandy_gravel_nor_gl.png
│   │       │   │   │   │   │   │   ├── sandy_gravel_nor_gl.png.meta
│   │       │   │   │   │   │   │   ├── sandy_gravel_Rough.png
│   │       │   │   │   │   │   │   └── sandy_gravel_Rough.png.meta
│   │       │   │   │   │   │   ├── Documentation.meta
│   │       │   │   │   │   │   ├── Lighting.meta
│   │       │   │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   │   ├── Models.meta
│   │       │   │   │   │   │   ├── Prefabs.meta
│   │       │   │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   │   ├── Scripts.meta
│   │       │   │   │   │   │   ├── Tests.meta
│   │       │   │   │   │   │   ├── Textures.meta
│   │       │   │   │   │   │   └── VFX.meta
│   │       │   │   │   │   ├── MarsVisualPolish/
│   │       │   │   │   │   │   ├── Documentation/
│   │       │   │   │   │   │   │   ├── PolishRegistry.asset
│   │       │   │   │   │   │   │   └── PolishRegistry.asset.meta
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── VisualPolishBuilder.cs
│   │       │   │   │   │   │   │   └── VisualPolishBuilder.cs.meta
│   │       │   │   │   │   │   ├── Lighting/
│   │       │   │   │   │   │   │   ├── PolishSky.mat
│   │       │   │   │   │   │   │   ├── PolishSky.mat.meta
│   │       │   │   │   │   │   │   ├── PolishVolume.asset
│   │       │   │   │   │   │   │   ├── PolishVolume.asset.meta
│   │       │   │   │   │   │   │   ├── Preset_0.asset
│   │       │   │   │   │   │   │   ├── Preset_0.asset.meta
│   │       │   │   │   │   │   │   ├── Preset_1.asset
│   │       │   │   │   │   │   │   ├── Preset_1.asset.meta
│   │       │   │   │   │   │   │   ├── Preset_2.asset
│   │       │   │   │   │   │   │   ├── Preset_2.asset.meta
│   │       │   │   │   │   │   │   ├── Preset_3.asset
│   │       │   │   │   │   │   │   └── Preset_3.asset.meta
│   │       │   │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   │   ├── Brushed frame.mat
│   │       │   │   │   │   │   │   ├── Brushed frame.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_0.mat
│   │       │   │   │   │   │   │   ├── Derivative_0.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_1.mat
│   │       │   │   │   │   │   │   ├── Derivative_1.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_10.mat
│   │       │   │   │   │   │   │   ├── Derivative_10.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_11.mat
│   │       │   │   │   │   │   │   ├── Derivative_11.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_12.mat
│   │       │   │   │   │   │   │   ├── Derivative_12.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_13.mat
│   │       │   │   │   │   │   │   ├── Derivative_13.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_14.mat
│   │       │   │   │   │   │   │   ├── Derivative_14.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_15.mat
│   │       │   │   │   │   │   │   ├── Derivative_15.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_16.mat
│   │       │   │   │   │   │   │   ├── Derivative_16.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_17.mat
│   │       │   │   │   │   │   │   ├── Derivative_17.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_18.mat
│   │       │   │   │   │   │   │   ├── Derivative_18.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_19.mat
│   │       │   │   │   │   │   │   ├── Derivative_19.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_2.mat
│   │       │   │   │   │   │   │   ├── Derivative_2.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_20.mat
│   │       │   │   │   │   │   │   ├── Derivative_20.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_21.mat
│   │       │   │   │   │   │   │   ├── Derivative_21.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_3.mat
│   │       │   │   │   │   │   │   ├── Derivative_3.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_4.mat
│   │       │   │   │   │   │   │   ├── Derivative_4.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_5.mat
│   │       │   │   │   │   │   │   ├── Derivative_5.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_6.mat
│   │       │   │   │   │   │   │   ├── Derivative_6.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_7.mat
│   │       │   │   │   │   │   │   ├── Derivative_7.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_8.mat
│   │       │   │   │   │   │   │   ├── Derivative_8.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_9.mat
│   │       │   │   │   │   │   │   ├── Derivative_9.mat.meta
│   │       │   │   │   │   │   │   ├── Photovoltaic cell laminate.mat
│   │       │   │   │   │   │   │   ├── Photovoltaic cell laminate.mat.meta
│   │       │   │   │   │   │   │   ├── PhotovoltaicGrid.png
│   │       │   │   │   │   │   │   ├── PhotovoltaicGrid.png.meta
│   │       │   │   │   │   │   │   ├── Recessed seals.mat
│   │       │   │   │   │   │   │   ├── Recessed seals.mat.meta
│   │       │   │   │   │   │   │   ├── Safety ochre.mat
│   │       │   │   │   │   │   │   ├── Safety ochre.mat.meta
│   │       │   │   │   │   │   │   ├── Warm ceramic.mat
│   │       │   │   │   │   │   │   └── Warm ceramic.mat.meta
│   │       │   │   │   │   │   ├── Models/
│   │       │   │   │   │   │   │   ├── Crew Dome_Frame.asset
│   │       │   │   │   │   │   │   ├── Crew Dome_Frame.asset.meta
│   │       │   │   │   │   │   │   ├── Laboratory Dome_Frame.asset
│   │       │   │   │   │   │   │   ├── Laboratory Dome_Frame.asset.meta
│   │       │   │   │   │   │   │   ├── Life Support Dome_Frame.asset
│   │       │   │   │   │   │   │   └── Life Support Dome_Frame.asset.meta
│   │       │   │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   │   ├── Mars_VisualPolish_v2.unity
│   │       │   │   │   │   │   │   └── Mars_VisualPolish_v2.unity.meta
│   │       │   │   │   │   │   ├── Documentation.meta
│   │       │   │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   │   ├── Lighting.meta
│   │       │   │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   │   ├── Models.meta
│   │       │   │   │   │   │   ├── PolishBenchmark.cs
│   │       │   │   │   │   │   ├── PolishBenchmark.cs.meta
│   │       │   │   │   │   │   ├── PolishHudTheme.cs
│   │       │   │   │   │   │   ├── PolishHudTheme.cs.meta
│   │       │   │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   │   ├── VisualPolishPresentation.cs
│   │       │   │   │   │   │   └── VisualPolishPresentation.cs.meta
│   │       │   │   │   │   ├── Mission01Milestone/
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── Mission01Builder.cs
│   │       │   │   │   │   │   │   └── Mission01Builder.cs.meta
│   │       │   │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   │   ├── Mission01_Continents.mat
│   │       │   │   │   │   │   │   ├── Mission01_Continents.mat.meta
│   │       │   │   │   │   │   │   ├── Mission01_Graphite.mat
│   │       │   │   │   │   │   │   ├── Mission01_Graphite.mat.meta
│   │       │   │   │   │   │   │   ├── Mission01_Ocean.mat
│   │       │   │   │   │   │   │   ├── Mission01_Ocean.mat.meta
│   │       │   │   │   │   │   │   ├── Mission01_Pale.mat
│   │       │   │   │   │   │   │   ├── Mission01_Pale.mat.meta
│   │       │   │   │   │   │   │   ├── Mission01_Safety.mat
│   │       │   │   │   │   │   │   └── Mission01_Safety.mat.meta
│   │       │   │   │   │   │   ├── Prefabs/
│   │       │   │   │   │   │   │   ├── Boarding_Console.prefab
│   │       │   │   │   │   │   │   ├── Boarding_Console.prefab.meta
│   │       │   │   │   │   │   │   ├── Trainee_Blockout.prefab
│   │       │   │   │   │   │   │   └── Trainee_Blockout.prefab.meta
│   │       │   │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   │   ├── Mission01_EarthLaunch_Milestone_v4.unity
│   │       │   │   │   │   │   ├── Mission01_EarthLaunch_Milestone_v4.unity.meta
│   │       │   │   │   │   │   ├── Mission01Flow.cs
│   │       │   │   │   │   │   ├── Mission01Flow.cs.meta
│   │       │   │   │   │   │   └── Prefabs.meta
│   │       │   │   │   │   ├── MissionIntegration/
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── MissionIntegrationBuilder.cs
│   │       │   │   │   │   │   │   └── MissionIntegrationBuilder.cs.meta
│   │       │   │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   │   ├── Mars_MissionIntegration_v1.unity
│   │       │   │   │   │   │   │   └── Mars_MissionIntegration_v1.unity.meta
│   │       │   │   │   │   │   ├── Scripts/
│   │       │   │   │   │   │   │   ├── MissionIntegrationController.cs
│   │       │   │   │   │   │   │   └── MissionIntegrationController.cs.meta
│   │       │   │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   │   └── Scripts.meta
│   │       │   │   │   │   ├── MissionRules/
│   │       │   │   │   │   │   ├── Config/
│   │       │   │   │   │   │   │   ├── mission_rules_v1.json
│   │       │   │   │   │   │   │   ├── mission_rules_v1.json.meta
│   │       │   │   │   │   │   │   ├── test_vectors_v1.json
│   │       │   │   │   │   │   │   └── test_vectors_v1.json.meta
│   │       │   │   │   │   │   ├── Scripts/
│   │       │   │   │   │   │   │   ├── MiniJson.cs
│   │       │   │   │   │   │   │   ├── MiniJson.cs.meta
│   │       │   │   │   │   │   │   ├── MissionRules.cs
│   │       │   │   │   │   │   │   └── MissionRules.cs.meta
│   │       │   │   │   │   │   ├── Config.meta
│   │       │   │   │   │   │   └── Scripts.meta
│   │       │   │   │   │   ├── NativeControlSlice/
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── NativeSliceBuilder.cs
│   │       │   │   │   │   │   │   └── NativeSliceBuilder.cs.meta
│   │       │   │   │   │   │   ├── Prefabs/
│   │       │   │   │   │   │   │   ├── AdaptiveTouch.prefab
│   │       │   │   │   │   │   │   └── AdaptiveTouch.prefab.meta
│   │       │   │   │   │   │   ├── AdaptiveTouch.cs
│   │       │   │   │   │   │   ├── AdaptiveTouch.cs.meta
│   │       │   │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   │   ├── Mars_NativeControl_VerticalSlice.unity
│   │       │   │   │   │   │   ├── Mars_NativeControl_VerticalSlice.unity.meta
│   │       │   │   │   │   │   ├── MissionRegistry.asset
│   │       │   │   │   │   │   ├── MissionRegistry.asset.meta
│   │       │   │   │   │   │   ├── NativeSliceGame.cs
│   │       │   │   │   │   │   ├── NativeSliceGame.cs.meta
│   │       │   │   │   │   │   ├── NativeSystems.cs
│   │       │   │   │   │   │   ├── NativeSystems.cs.meta
│   │       │   │   │   │   │   └── Prefabs.meta
│   │       │   │   │   │   ├── ProductionVisual/
│   │       │   │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   │   ├── Brushed aluminum.mat
│   │       │   │   │   │   │   │   ├── Brushed aluminum.mat.meta
│   │       │   │   │   │   │   │   ├── Dusty tire elastomer.mat
│   │       │   │   │   │   │   │   ├── Dusty tire elastomer.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_0.mat
│   │       │   │   │   │   │   │   ├── Inherited_0.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_1.mat
│   │       │   │   │   │   │   │   ├── Inherited_1.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_10.mat
│   │       │   │   │   │   │   │   ├── Inherited_10.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_11.mat
│   │       │   │   │   │   │   │   ├── Inherited_11.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_12.mat
│   │       │   │   │   │   │   │   ├── Inherited_12.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_13.mat
│   │       │   │   │   │   │   │   ├── Inherited_13.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_14.mat
│   │       │   │   │   │   │   │   ├── Inherited_14.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_15.mat
│   │       │   │   │   │   │   │   ├── Inherited_15.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_16.mat
│   │       │   │   │   │   │   │   ├── Inherited_16.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_17.mat
│   │       │   │   │   │   │   │   ├── Inherited_17.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_18.mat
│   │       │   │   │   │   │   │   ├── Inherited_18.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_19.mat
│   │       │   │   │   │   │   │   ├── Inherited_19.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_2.mat
│   │       │   │   │   │   │   │   ├── Inherited_2.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_20.mat
│   │       │   │   │   │   │   │   ├── Inherited_20.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_21.mat
│   │       │   │   │   │   │   │   ├── Inherited_21.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_22.mat
│   │       │   │   │   │   │   │   ├── Inherited_22.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_23.mat
│   │       │   │   │   │   │   │   ├── Inherited_23.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_24.mat
│   │       │   │   │   │   │   │   ├── Inherited_24.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_25.mat
│   │       │   │   │   │   │   │   ├── Inherited_25.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_3.mat
│   │       │   │   │   │   │   │   ├── Inherited_3.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_4.mat
│   │       │   │   │   │   │   │   ├── Inherited_4.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_5.mat
│   │       │   │   │   │   │   │   ├── Inherited_5.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_6.mat
│   │       │   │   │   │   │   │   ├── Inherited_6.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_7.mat
│   │       │   │   │   │   │   │   ├── Inherited_7.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_8.mat
│   │       │   │   │   │   │   │   ├── Inherited_8.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_9.mat
│   │       │   │   │   │   │   │   ├── Inherited_9.mat.meta
│   │       │   │   │   │   │   │   ├── Machined graphite alloy.mat
│   │       │   │   │   │   │   │   ├── Machined graphite alloy.mat.meta
│   │       │   │   │   │   │   │   ├── Opaque protective visor glazing.mat
│   │       │   │   │   │   │   │   ├── Opaque protective visor glazing.mat.meta
│   │       │   │   │   │   │   │   ├── Pressure shell ceramic composite.mat
│   │       │   │   │   │   │   │   ├── Pressure shell ceramic composite.mat.meta
│   │       │   │   │   │   │   │   ├── Restrained identification orange.mat
│   │       │   │   │   │   │   │   └── Restrained identification orange.mat.meta
│   │       │   │   │   │   │   ├── Meshes/
│   │       │   │   │   │   │   │   ├── ChamferedEnclosure.asset
│   │       │   │   │   │   │   │   ├── ChamferedEnclosure.asset.meta
│   │       │   │   │   │   │   │   ├── CommsTruss.asset
│   │       │   │   │   │   │   │   ├── CommsTruss.asset.meta
│   │       │   │   │   │   │   │   ├── Crew_Dome_sealed_airlock_Rails.asset
│   │       │   │   │   │   │   │   ├── Crew_Dome_sealed_airlock_Rails.asset.meta
│   │       │   │   │   │   │   │   ├── CrewCabEnclosure.asset
│   │       │   │   │   │   │   │   ├── CrewCabEnclosure.asset.meta
│   │       │   │   │   │   │   │   ├── GeodesicFrame.asset
│   │       │   │   │   │   │   │   ├── GeodesicFrame.asset.meta
│   │       │   │   │   │   │   │   ├── GeodesicPanels.asset
│   │       │   │   │   │   │   │   ├── GeodesicPanels.asset.meta
│   │       │   │   │   │   │   │   ├── Laboratory_Dome_sealed_airlock_Rails.asset
│   │       │   │   │   │   │   │   ├── Laboratory_Dome_sealed_airlock_Rails.asset.meta
│   │       │   │   │   │   │   │   ├── Life_Support_Dome_sealed_airlock_Rails.asset
│   │       │   │   │   │   │   │   ├── Life_Support_Dome_sealed_airlock_Rails.asset.meta
│   │       │   │   │   │   │   │   ├── RoverMechanicalFrame.asset
│   │       │   │   │   │   │   │   ├── RoverMechanicalFrame.asset.meta
│   │       │   │   │   │   │   │   ├── RoverWheel_LOD0.asset
│   │       │   │   │   │   │   │   ├── RoverWheel_LOD0.asset.meta
│   │       │   │   │   │   │   │   ├── RoverWheel_LOD1.asset
│   │       │   │   │   │   │   │   ├── RoverWheel_LOD1.asset.meta
│   │       │   │   │   │   │   │   ├── RoverWheel_LOD2.asset
│   │       │   │   │   │   │   │   └── RoverWheel_LOD2.asset.meta
│   │       │   │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   │   ├── Mars_ProductionVisual_v1.unity
│   │       │   │   │   │   │   │   └── Mars_ProductionVisual_v1.unity.meta
│   │       │   │   │   │   │   ├── Scripts/
│   │       │   │   │   │   │   │   ├── ProductionCameraRig.cs
│   │       │   │   │   │   │   │   ├── ProductionCameraRig.cs.meta
│   │       │   │   │   │   │   │   ├── ProductionModeCoordinator.cs
│   │       │   │   │   │   │   │   ├── ProductionModeCoordinator.cs.meta
│   │       │   │   │   │   │   │   ├── ProductionPerformanceProbe.cs
│   │       │   │   │   │   │   │   ├── ProductionPerformanceProbe.cs.meta
│   │       │   │   │   │   │   │   ├── ProductionPresentation.cs
│   │       │   │   │   │   │   │   └── ProductionPresentation.cs.meta
│   │       │   │   │   │   │   ├── Settings/
│   │       │   │   │   │   │   │   ├── Lighting_0.asset
│   │       │   │   │   │   │   │   ├── Lighting_0.asset.meta
│   │       │   │   │   │   │   │   ├── Lighting_1.asset
│   │       │   │   │   │   │   │   ├── Lighting_1.asset.meta
│   │       │   │   │   │   │   │   ├── Lighting_2.asset
│   │       │   │   │   │   │   │   ├── Lighting_2.asset.meta
│   │       │   │   │   │   │   │   ├── Lighting_3.asset
│   │       │   │   │   │   │   │   ├── Lighting_3.asset.meta
│   │       │   │   │   │   │   │   ├── ProductionMissionRegistry.asset
│   │       │   │   │   │   │   │   └── ProductionMissionRegistry.asset.meta
│   │       │   │   │   │   │   ├── Tests/
│   │       │   │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   │   ├── ProductionMeshFactory.cs
│   │       │   │   │   │   │   │   │   ├── ProductionMeshFactory.cs.meta
│   │       │   │   │   │   │   │   │   ├── ProductionViewport.cs
│   │       │   │   │   │   │   │   │   ├── ProductionViewport.cs.meta
│   │       │   │   │   │   │   │   │   ├── ProductionVisualBuilder.cs
│   │       │   │   │   │   │   │   │   └── ProductionVisualBuilder.cs.meta
│   │       │   │   │   │   │   │   └── Editor.meta
│   │       │   │   │   │   │   ├── Textures/
│   │       │   │   │   │   │   │   ├── rock_07_MetallicSmoothness.png
│   │       │   │   │   │   │   │   ├── rock_07_MetallicSmoothness.png.meta
│   │       │   │   │   │   │   │   ├── rock_09_MetallicSmoothness.png
│   │       │   │   │   │   │   │   ├── rock_09_MetallicSmoothness.png.meta
│   │       │   │   │   │   │   │   ├── sandy_gravel_MetallicSmoothness.png
│   │       │   │   │   │   │   │   └── sandy_gravel_MetallicSmoothness.png.meta
│   │       │   │   │   │   │   ├── Themes/
│   │       │   │   │   │   │   │   ├── ProductionHUDTheme.asset
│   │       │   │   │   │   │   │   ├── ProductionHUDTheme.asset.meta
│   │       │   │   │   │   │   │   ├── ProductionSky.mat
│   │       │   │   │   │   │   │   ├── ProductionSky.mat.meta
│   │       │   │   │   │   │   │   ├── ProductionVolume.asset
│   │       │   │   │   │   │   │   └── ProductionVolume.asset.meta
│   │       │   │   │   │   │   ├── Documentation.meta
│   │       │   │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   │   ├── Meshes.meta
│   │       │   │   │   │   │   ├── Prefabs.meta
│   │       │   │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   │   ├── Scripts.meta
│   │       │   │   │   │   │   ├── Settings.meta
│   │       │   │   │   │   │   ├── Tests.meta
│   │       │   │   │   │   │   ├── Textures.meta
│   │       │   │   │   │   │   └── Themes.meta
│   │       │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   ├── MarsOutpost_Environment_v01.unity
│   │       │   │   │   │   │   └── MarsOutpost_Environment_v01.unity.meta
│   │       │   │   │   │   ├── Stage08B/
│   │       │   │   │   │   │   ├── Animation/
│   │       │   │   │   │   │   │   ├── Idle.anim
│   │       │   │   │   │   │   │   ├── Idle.anim.meta
│   │       │   │   │   │   │   │   ├── PrototypeLocomotion.controller
│   │       │   │   │   │   │   │   ├── PrototypeLocomotion.controller.meta
│   │       │   │   │   │   │   │   ├── Run.anim
│   │       │   │   │   │   │   │   ├── Run.anim.meta
│   │       │   │   │   │   │   │   ├── Walk.anim
│   │       │   │   │   │   │   │   └── Walk.anim.meta
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── Stage08BBuilder.cs
│   │       │   │   │   │   │   │   └── Stage08BBuilder.cs.meta
│   │       │   │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   │   ├── 08B_Accent_Cyan.mat
│   │       │   │   │   │   │   │   ├── 08B_Accent_Cyan.mat.meta
│   │       │   │   │   │   │   │   ├── 08B_Accent_SafetyOrange.mat
│   │       │   │   │   │   │   │   ├── 08B_Accent_SafetyOrange.mat.meta
│   │       │   │   │   │   │   │   ├── 08B_Glass_Utility.mat
│   │       │   │   │   │   │   │   ├── 08B_Glass_Utility.mat.meta
│   │       │   │   │   │   │   │   ├── 08B_Habitat_CompositeDark.mat
│   │       │   │   │   │   │   │   ├── 08B_Habitat_CompositeDark.mat.meta
│   │       │   │   │   │   │   │   ├── 08B_Habitat_OffWhite.mat
│   │       │   │   │   │   │   │   ├── 08B_Habitat_OffWhite.mat.meta
│   │       │   │   │   │   │   │   ├── 08B_Mars_Regolith.mat
│   │       │   │   │   │   │   │   ├── 08B_Mars_Regolith.mat.meta
│   │       │   │   │   │   │   │   ├── 08B_Mars_Regolith_Dark.mat
│   │       │   │   │   │   │   │   ├── 08B_Mars_Regolith_Dark.mat.meta
│   │       │   │   │   │   │   │   ├── 08B_Mars_Rock_Basaltic.mat
│   │       │   │   │   │   │   │   ├── 08B_Mars_Rock_Basaltic.mat.meta
│   │       │   │   │   │   │   │   ├── 08B_Mars_Rock_Layered.mat
│   │       │   │   │   │   │   │   ├── 08B_Mars_Rock_Layered.mat.meta
│   │       │   │   │   │   │   │   ├── 08B_Metal_Industrial.mat
│   │       │   │   │   │   │   │   ├── 08B_Metal_Industrial.mat.meta
│   │       │   │   │   │   │   │   ├── 08B_SolarPanel.mat
│   │       │   │   │   │   │   │   ├── 08B_SolarPanel.mat.meta
│   │       │   │   │   │   │   │   ├── BasaltVariationA.mat
│   │       │   │   │   │   │   │   ├── BasaltVariationA.mat.meta
│   │       │   │   │   │   │   │   ├── BasaltVariationB.mat
│   │       │   │   │   │   │   │   ├── BasaltVariationB.mat.meta
│   │       │   │   │   │   │   │   ├── BasaltVariationC.mat
│   │       │   │   │   │   │   │   ├── BasaltVariationC.mat.meta
│   │       │   │   │   │   │   │   ├── Dust_Particle.mat
│   │       │   │   │   │   │   │   ├── Dust_Particle.mat.meta
│   │       │   │   │   │   │   │   ├── Identification_Cyan.mat
│   │       │   │   │   │   │   │   ├── Identification_Cyan.mat.meta
│   │       │   │   │   │   │   │   ├── Safety_Orange.mat
│   │       │   │   │   │   │   │   ├── Safety_Orange.mat.meta
│   │       │   │   │   │   │   │   ├── Suit_Graphite.mat
│   │       │   │   │   │   │   │   ├── Suit_Graphite.mat.meta
│   │       │   │   │   │   │   │   ├── Suit_Pale.mat
│   │       │   │   │   │   │   │   └── Suit_Pale.mat.meta
│   │       │   │   │   │   │   ├── Animation.meta
│   │       │   │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   │   ├── MarsOutpost_Playable_Stage08B.unity
│   │       │   │   │   │   │   ├── MarsOutpost_Playable_Stage08B.unity.meta
│   │       │   │   │   │   │   ├── MarsPrototype.cs
│   │       │   │   │   │   │   ├── MarsPrototype.cs.meta
│   │       │   │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   │   ├── Stage08B_MobilePreview.asset
│   │       │   │   │   │   │   └── Stage08B_MobilePreview.asset.meta
│   │       │   │   │   │   ├── Stage08C/
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── AndroidProbeBuild.cs
│   │       │   │   │   │   │   │   ├── AndroidProbeBuild.cs.meta
│   │       │   │   │   │   │   │   ├── TouchChecks.cs
│   │       │   │   │   │   │   │   └── TouchChecks.cs.meta
│   │       │   │   │   │   │   ├── AndroidProfileProbe.cs
│   │       │   │   │   │   │   ├── AndroidProfileProbe.cs.meta
│   │       │   │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   │   ├── MobileControls.cs
│   │       │   │   │   │   │   └── MobileControls.cs.meta
│   │       │   │   │   │   ├── Stage08C1/
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── EditorPerformanceObserver.cs
│   │       │   │   │   │   │   │   ├── EditorPerformanceObserver.cs.meta
│   │       │   │   │   │   │   │   ├── ManualInputObserver.cs
│   │       │   │   │   │   │   │   └── ManualInputObserver.cs.meta
│   │       │   │   │   │   │   └── Editor.meta
│   │       │   │   │   │   ├── Verification/
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── LaptopPlaytest.cs
│   │       │   │   │   │   │   │   ├── LaptopPlaytest.cs.meta
│   │       │   │   │   │   │   │   ├── Step1GameplayVerification.cs
│   │       │   │   │   │   │   │   ├── Step1GameplayVerification.cs.meta
│   │       │   │   │   │   │   │   ├── Step3BackendClientTest.cs
│   │       │   │   │   │   │   │   ├── Step3BackendClientTest.cs.meta
│   │       │   │   │   │   │   │   ├── Step4RulesTest.cs
│   │       │   │   │   │   │   │   └── Step4RulesTest.cs.meta
│   │       │   │   │   │   │   └── Editor.meta
│   │       │   │   │   │   ├── VisualUpgrade/
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── VisualUpgradeBuilder.cs
│   │       │   │   │   │   │   │   ├── VisualUpgradeBuilder.cs.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeCapture.cs
│   │       │   │   │   │   │   │   ├── VisualUpgradeCapture.cs.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeCharacters.cs
│   │       │   │   │   │   │   │   ├── VisualUpgradeCharacters.cs.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeEnvironment.cs
│   │       │   │   │   │   │   │   ├── VisualUpgradeEnvironment.cs.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeKit.cs
│   │       │   │   │   │   │   │   ├── VisualUpgradeKit.cs.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradePlaytest.cs
│   │       │   │   │   │   │   │   ├── VisualUpgradePlaytest.cs.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeProps.cs
│   │       │   │   │   │   │   │   └── VisualUpgradeProps.cs.meta
│   │       │   │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   │   ├── VisualUpgradeHaze.mat
│   │       │   │   │   │   │   │   ├── VisualUpgradeHaze.mat.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeMarsGround.mat
│   │       │   │   │   │   │   │   ├── VisualUpgradeMarsGround.mat.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeMarsSky.mat
│   │       │   │   │   │   │   │   ├── VisualUpgradeMarsSky.mat.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradePhotovoltaic.mat
│   │       │   │   │   │   │   │   ├── VisualUpgradePhotovoltaic.mat.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeRock07.mat
│   │       │   │   │   │   │   │   ├── VisualUpgradeRock07.mat.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeRock09.mat
│   │       │   │   │   │   │   │   ├── VisualUpgradeRock09.mat.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeSky.mat
│   │       │   │   │   │   │   │   ├── VisualUpgradeSky.mat.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeWheelDust.mat
│   │       │   │   │   │   │   │   ├── VisualUpgradeWheelDust.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Accent_orange.mat
│   │       │   │   │   │   │   │   ├── VU_Accent_orange.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Beacon_red.mat
│   │       │   │   │   │   │   │   ├── VU_Beacon_red.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Brushed_aluminium.mat
│   │       │   │   │   │   │   │   ├── VU_Brushed_aluminium.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Equipment_white.mat
│   │       │   │   │   │   │   │   ├── VU_Equipment_white.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Habitat_marker_light.mat
│   │       │   │   │   │   │   │   ├── VU_Habitat_marker_light.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Habitat_shell.mat
│   │       │   │   │   │   │   │   ├── VU_Habitat_shell.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Habitat_window_glow.mat
│   │       │   │   │   │   │   │   ├── VU_Habitat_window_glow.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Helmet_lamp.mat
│   │       │   │   │   │   │   │   ├── VU_Helmet_lamp.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Mission_emblem_(provisional).mat
│   │       │   │   │   │   │   │   ├── VU_Mission_emblem_(provisional).mat.meta
│   │       │   │   │   │   │   │   ├── VU_Rover_beacon.mat
│   │       │   │   │   │   │   │   ├── VU_Rover_beacon.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Rover_body_panel.mat
│   │       │   │   │   │   │   │   ├── VU_Rover_body_panel.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Rover_cockpit_glass.mat
│   │       │   │   │   │   │   │   ├── VU_Rover_cockpit_glass.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Rover_graphite.mat
│   │       │   │   │   │   │   │   ├── VU_Rover_graphite.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Rover_headlight_lens.mat
│   │       │   │   │   │   │   │   ├── VU_Rover_headlight_lens.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Rover_screen.mat
│   │       │   │   │   │   │   │   ├── VU_Rover_screen.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Rover_seat.mat
│   │       │   │   │   │   │   │   ├── VU_Rover_seat.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Rover_solar_cells.mat
│   │       │   │   │   │   │   │   ├── VU_Rover_solar_cells.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Rover_tail_light.mat
│   │       │   │   │   │   │   │   ├── VU_Rover_tail_light.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Sensor_lens.mat
│   │       │   │   │   │   │   │   ├── VU_Sensor_lens.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Status_amber.mat
│   │       │   │   │   │   │   │   ├── VU_Status_amber.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Status_cyan.mat
│   │       │   │   │   │   │   │   ├── VU_Status_cyan.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Suit_fabric.mat
│   │       │   │   │   │   │   │   ├── VU_Suit_fabric.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Suit_hard_shell.mat
│   │       │   │   │   │   │   │   ├── VU_Suit_hard_shell.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Suit_joint_bearing.mat
│   │       │   │   │   │   │   │   ├── VU_Suit_joint_bearing.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Suit_soft_grey.mat
│   │       │   │   │   │   │   │   ├── VU_Suit_soft_grey.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Visor_gold_coating.mat
│   │       │   │   │   │   │   │   └── VU_Visor_gold_coating.mat.meta
│   │       │   │   │   │   │   ├── Meshes/
│   │       │   │   │   │   │   │   ├── VisualUpgradeDish.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeDish.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeFarTerrain.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeFarTerrain.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeRoverDish.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeRoverDish.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_0_0.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_0_0.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_0_1.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_0_1.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_0_2.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_0_2.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_0_3.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_0_3.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_0_4.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_0_4.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_0_5.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_0_5.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_1_0.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_1_0.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_1_1.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_1_1.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_1_2.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_1_2.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_1_3.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_1_3.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_1_4.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_1_4.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_1_5.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_1_5.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_2_0.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_2_0.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_2_1.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_2_1.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_2_2.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_2_2.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_2_3.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_2_3.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_2_4.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_2_4.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_2_5.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_2_5.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_3_0.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_3_0.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_3_1.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_3_1.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_3_2.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_3_2.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_3_3.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_3_3.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_3_4.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_3_4.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_3_5.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_3_5.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_4_0.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_4_0.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_4_1.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_4_1.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_4_2.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_4_2.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_4_3.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_4_3.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_4_4.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_4_4.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_4_5.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_4_5.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_5_0.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_5_0.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_5_1.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_5_1.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_5_2.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_5_2.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_5_3.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_5_3.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_5_4.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_5_4.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_5_5.asset
│   │       │   │   │   │   │   │   └── VisualUpgradeScatter_5_5.asset.meta
│   │       │   │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   │   ├── Mars_VisualUpgrade_v1.unity
│   │       │   │   │   │   │   │   └── Mars_VisualUpgrade_v1.unity.meta
│   │       │   │   │   │   │   ├── Scripts/
│   │       │   │   │   │   │   │   ├── VisualUpgradeAstronautMotion.cs
│   │       │   │   │   │   │   │   ├── VisualUpgradeAstronautMotion.cs.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeAtmosphere.cs
│   │       │   │   │   │   │   │   ├── VisualUpgradeAtmosphere.cs.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeBlink.cs
│   │       │   │   │   │   │   │   ├── VisualUpgradeBlink.cs.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeCameraRig.cs
│   │       │   │   │   │   │   │   ├── VisualUpgradeCameraRig.cs.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeHUDFixes.cs
│   │       │   │   │   │   │   │   ├── VisualUpgradeHUDFixes.cs.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeHUDStyle.cs
│   │       │   │   │   │   │   │   ├── VisualUpgradeHUDStyle.cs.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeRoverPresentation.cs
│   │       │   │   │   │   │   │   └── VisualUpgradeRoverPresentation.cs.meta
│   │       │   │   │   │   │   ├── Settings/
│   │       │   │   │   │   │   │   ├── Lighting_0.asset
│   │       │   │   │   │   │   │   ├── Lighting_0.asset.meta
│   │       │   │   │   │   │   │   ├── Lighting_1.asset
│   │       │   │   │   │   │   │   ├── Lighting_1.asset.meta
│   │       │   │   │   │   │   │   ├── Lighting_2.asset
│   │       │   │   │   │   │   │   ├── Lighting_2.asset.meta
│   │       │   │   │   │   │   │   ├── Lighting_3.asset
│   │       │   │   │   │   │   │   ├── Lighting_3.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgrade_Renderer.asset
│   │       │   │   │   │   │   │   ├── VisualUpgrade_Renderer.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgrade_URP.asset
│   │       │   │   │   │   │   │   ├── VisualUpgrade_URP.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeHUDTheme.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeHUDTheme.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeMissionRegistry.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeMissionRegistry.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeVolume.asset
│   │       │   │   │   │   │   │   └── VisualUpgradeVolume.asset.meta
│   │       │   │   │   │   │   ├── Shaders/
│   │       │   │   │   │   │   │   ├── MarsSky.shader
│   │       │   │   │   │   │   │   ├── MarsSky.shader.meta
│   │       │   │   │   │   │   │   ├── MarsTerrain.shader
│   │       │   │   │   │   │   │   └── MarsTerrain.shader.meta
│   │       │   │   │   │   │   ├── Textures/
│   │       │   │   │   │   │   │   ├── VisualUpgradeEmblem.png
│   │       │   │   │   │   │   │   ├── VisualUpgradeEmblem.png.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeSoftDust.png
│   │       │   │   │   │   │   │   └── VisualUpgradeSoftDust.png.meta
│   │       │   │   │   │   │   ├── Documentation.meta
│   │       │   │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   │   ├── Meshes.meta
│   │       │   │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   │   ├── Scripts.meta
│   │       │   │   │   │   │   ├── Settings.meta
│   │       │   │   │   │   │   ├── Shaders.meta
│   │       │   │   │   │   │   └── Textures.meta
│   │       │   │   │   │   ├── CinematicHUD.meta
│   │       │   │   │   │   ├── Environment.meta
│   │       │   │   │   │   ├── Greybox.meta
│   │       │   │   │   │   ├── Integration.meta
│   │       │   │   │   │   ├── MarsEnvironment.meta
│   │       │   │   │   │   ├── MarsVisualPolish.meta
│   │       │   │   │   │   ├── Mission01Milestone.meta
│   │       │   │   │   │   ├── MissionIntegration.meta
│   │       │   │   │   │   ├── MissionRules.meta
│   │       │   │   │   │   ├── NativeControlSlice.meta
│   │       │   │   │   │   ├── ProductionVisual.meta
│   │       │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   ├── Stage08B.meta
│   │       │   │   │   │   ├── Stage08C.meta
│   │       │   │   │   │   ├── Stage08C1.meta
│   │       │   │   │   │   ├── Verification.meta
│   │       │   │   │   │   └── VisualUpgrade.meta
│   │       │   │   │   ├── BuildOnly/
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── VisualUpgradeMacBuild.cs
│   │       │   │   │   │   │   └── VisualUpgradeMacBuild.cs.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── GameplayRecorder.cs
│   │       │   │   │   │   ├── GameplayRecorder.cs.meta
│   │       │   │   │   │   ├── StandaloneProbe.cs
│   │       │   │   │   │   └── StandaloneProbe.cs.meta
│   │       │   │   │   ├── Scenes/
│   │       │   │   │   │   ├── SampleScene.unity
│   │       │   │   │   │   └── SampleScene.unity.meta
│   │       │   │   │   ├── Settings/
│   │       │   │   │   │   ├── DefaultVolumeProfile.asset
│   │       │   │   │   │   ├── DefaultVolumeProfile.asset.meta
│   │       │   │   │   │   ├── Mobile_Renderer.asset
│   │       │   │   │   │   ├── Mobile_Renderer.asset.meta
│   │       │   │   │   │   ├── Mobile_RPAsset.asset
│   │       │   │   │   │   ├── Mobile_RPAsset.asset.meta
│   │       │   │   │   │   ├── PC_Renderer.asset
│   │       │   │   │   │   ├── PC_Renderer.asset.meta
│   │       │   │   │   │   ├── PC_RPAsset.asset
│   │       │   │   │   │   ├── PC_RPAsset.asset.meta
│   │       │   │   │   │   ├── SampleSceneProfile.asset
│   │       │   │   │   │   ├── SampleSceneProfile.asset.meta
│   │       │   │   │   │   ├── UniversalRenderPipelineGlobalSettings.asset
│   │       │   │   │   │   └── UniversalRenderPipelineGlobalSettings.asset.meta
│   │       │   │   │   ├── TutorialInfo/
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── ReadmeEditor.cs
│   │       │   │   │   │   │   └── ReadmeEditor.cs.meta
│   │       │   │   │   │   ├── Icons/
│   │       │   │   │   │   │   ├── URP.png
│   │       │   │   │   │   │   └── URP.png.meta
│   │       │   │   │   │   ├── StyleSheets/
│   │       │   │   │   │   │   ├── ReadmeEditor.uss
│   │       │   │   │   │   │   ├── ReadmeEditor.uss.meta
│   │       │   │   │   │   │   ├── ReadmeEditorDark.uss
│   │       │   │   │   │   │   ├── ReadmeEditorDark.uss.meta
│   │       │   │   │   │   │   ├── ReadmeEditorLight.uss
│   │       │   │   │   │   │   └── ReadmeEditorLight.uss.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── Icons.meta
│   │       │   │   │   │   ├── Layout.wlt
│   │       │   │   │   │   ├── Layout.wlt.meta
│   │       │   │   │   │   ├── Readme.cs
│   │       │   │   │   │   ├── Readme.cs.meta
│   │       │   │   │   │   └── StyleSheets.meta
│   │       │   │   │   ├── AstroGateway.meta
│   │       │   │   │   ├── BuildOnly.meta
│   │       │   │   │   ├── InputSystem_Actions.inputactions
│   │       │   │   │   ├── InputSystem_Actions.inputactions.meta
│   │       │   │   │   ├── Readme.asset
│   │       │   │   │   ├── Readme.asset.meta
│   │       │   │   │   ├── Scenes.meta
│   │       │   │   │   ├── Settings.meta
│   │       │   │   │   └── TutorialInfo.meta
│   │       │   │   ├── Packages/
│   │       │   │   │   ├── manifest.json
│   │       │   │   │   └── packages-lock.json
│   │       │   │   └── ProjectSettings/
│   │       │   │       ├── AudioManager.asset
│   │       │   │       ├── ClusterInputManager.asset
│   │       │   │       ├── DynamicsManager.asset
│   │       │   │       ├── EditorBuildSettings.asset
│   │       │   │       ├── EditorSettings.asset
│   │       │   │       ├── GraphicsSettings.asset
│   │       │   │       ├── InputManager.asset
│   │       │   │       ├── MemorySettings.asset
│   │       │   │       ├── MultiplayerManager.asset
│   │       │   │       ├── NavMeshAreas.asset
│   │       │   │       ├── PackageManagerSettings.asset
│   │       │   │       ├── Physics2DSettings.asset
│   │       │   │       ├── PhysicsCoreProjectSettings2D.asset
│   │       │   │       ├── PresetManager.asset
│   │       │   │       ├── ProjectAuditorSettings.asset
│   │       │   │       ├── ProjectSettings.asset
│   │       │   │       ├── ProjectVersion.txt
│   │       │   │       ├── QualitySettings.asset
│   │       │   │       ├── SceneTemplateSettings.json
│   │       │   │       ├── ShaderGraphSettings.asset
│   │       │   │       ├── TagManager.asset
│   │       │   │       ├── TimeManager.asset
│   │       │   │       ├── UnityConnectSettings.asset
│   │       │   │       ├── URPProjectSettings.asset
│   │       │   │       ├── VersionControlSettings.asset
│   │       │   │       ├── VFXManager.asset
│   │       │   │       └── XRSettings.asset
│   │       │   ├── encode
│   │       │   └── encode.swift
│   │       ├── harness_backup/
│   │       │   ├── CinematicHUD/
│   │       │   │   └── Tests/
│   │       │   │       └── Editor/
│   │       │   │           ├── CinematicHUDBuilder.cs
│   │       │   │           └── HUDResponsiveQA.cs
│   │       │   ├── MarsEnvironment/
│   │       │   │   └── Tests/
│   │       │   │       └── Editor/
│   │       │   │           └── MarsResearchBaseBuilder.cs
│   │       │   ├── Mission01Milestone/
│   │       │   │   └── Editor/
│   │       │   │       └── Mission01Builder.cs
│   │       │   ├── NativeControlSlice/
│   │       │   │   └── Editor/
│   │       │   │       └── NativeSliceBuilder.cs
│   │       │   └── Stage08B/
│   │       │       └── Editor/
│   │       │           └── Stage08BBuilder.cs
│   │       ├── Lab/
│   │       │   ├── Assets/
│   │       │   │   ├── AstroGateway/
│   │       │   │   │   ├── CinematicHUD/
│   │       │   │   │   │   ├── Documentation/
│   │       │   │   │   │   │   ├── README.md
│   │       │   │   │   │   │   └── README.md.meta
│   │       │   │   │   │   ├── Prefabs/
│   │       │   │   │   │   │   ├── AstroGatewayCinematicHUD.prefab
│   │       │   │   │   │   │   └── AstroGatewayCinematicHUD.prefab.meta
│   │       │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   ├── Mars_CinematicHUD_v1.unity
│   │       │   │   │   │   │   └── Mars_CinematicHUD_v1.unity.meta
│   │       │   │   │   │   ├── Scripts/
│   │       │   │   │   │   │   ├── AstroGatewayHUDController.cs
│   │       │   │   │   │   │   ├── AstroGatewayHUDController.cs.meta
│   │       │   │   │   │   │   ├── CinematicEnvironmentAdapter.cs
│   │       │   │   │   │   │   ├── CinematicEnvironmentAdapter.cs.meta
│   │       │   │   │   │   │   ├── CinematicGameAdapter.cs
│   │       │   │   │   │   │   ├── CinematicGameAdapter.cs.meta
│   │       │   │   │   │   │   ├── HUDInputRouter.cs
│   │       │   │   │   │   │   ├── HUDInputRouter.cs.meta
│   │       │   │   │   │   │   ├── HUDLayoutProbe.cs
│   │       │   │   │   │   │   ├── HUDLayoutProbe.cs.meta
│   │       │   │   │   │   │   ├── HUDPerformanceProbe.cs
│   │       │   │   │   │   │   ├── HUDPerformanceProbe.cs.meta
│   │       │   │   │   │   │   ├── HUDPointerControl.cs
│   │       │   │   │   │   │   ├── HUDPointerControl.cs.meta
│   │       │   │   │   │   │   ├── HUDTelemetryAdapter.cs
│   │       │   │   │   │   │   ├── HUDTelemetryAdapter.cs.meta
│   │       │   │   │   │   │   ├── HUDTheme.cs
│   │       │   │   │   │   │   └── HUDTheme.cs.meta
│   │       │   │   │   │   ├── Settings/
│   │       │   │   │   │   │   ├── MissionRegistry.asset
│   │       │   │   │   │   │   └── MissionRegistry.asset.meta
│   │       │   │   │   │   ├── Tests/
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── CinematicHUDBuilder.cs
│   │       │   │   │   │   │   │   ├── CinematicHUDBuilder.cs.meta
│   │       │   │   │   │   │   │   ├── HUDResponsiveQA.cs
│   │       │   │   │   │   │   │   └── HUDResponsiveQA.cs.meta
│   │       │   │   │   │   │   └── Editor.meta
│   │       │   │   │   │   ├── Themes/
│   │       │   │   │   │   │   ├── AstroGatewayHUDTheme.asset
│   │       │   │   │   │   │   └── AstroGatewayHUDTheme.asset.meta
│   │       │   │   │   │   ├── UI/
│   │       │   │   │   │   │   ├── CircularControl.png
│   │       │   │   │   │   │   ├── CircularControl.png.meta
│   │       │   │   │   │   │   ├── HeadingArrow.png
│   │       │   │   │   │   │   ├── HeadingArrow.png.meta
│   │       │   │   │   │   │   ├── OriginalContourMap.png
│   │       │   │   │   │   │   ├── OriginalContourMap.png.meta
│   │       │   │   │   │   │   ├── RoundedPanel.png
│   │       │   │   │   │   │   └── RoundedPanel.png.meta
│   │       │   │   │   │   ├── Documentation.meta
│   │       │   │   │   │   ├── link.xml
│   │       │   │   │   │   ├── link.xml.meta
│   │       │   │   │   │   ├── Prefabs.meta
│   │       │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   ├── Scripts.meta
│   │       │   │   │   │   ├── Settings.meta
│   │       │   │   │   │   ├── Tests.meta
│   │       │   │   │   │   ├── Themes.meta
│   │       │   │   │   │   └── UI.meta
│   │       │   │   │   ├── Environment/
│   │       │   │   │   │   ├── Architecture/
│   │       │   │   │   │   │   ├── AgricultureG01_ChamferShell.asset
│   │       │   │   │   │   │   ├── AgricultureG01_ChamferShell.asset.meta
│   │       │   │   │   │   │   ├── AgricultureG01_Ring.asset
│   │       │   │   │   │   │   ├── AgricultureG01_Ring.asset.meta
│   │       │   │   │   │   │   ├── AirlockShell.asset
│   │       │   │   │   │   │   ├── AirlockShell.asset.meta
│   │       │   │   │   │   │   ├── C_ScienceStaging_Accent_Cyan_Combined.asset
│   │       │   │   │   │   │   ├── C_ScienceStaging_Accent_Cyan_Combined.asset.meta
│   │       │   │   │   │   │   ├── C_ScienceStaging_Accent_SafetyOrange_Combined.asset
│   │       │   │   │   │   │   ├── C_ScienceStaging_Accent_SafetyOrange_Combined.asset.meta
│   │       │   │   │   │   │   ├── C_ScienceStaging_Glass_Utility_Combined.asset
│   │       │   │   │   │   │   ├── C_ScienceStaging_Glass_Utility_Combined.asset.meta
│   │       │   │   │   │   │   ├── C_ScienceStaging_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   ├── C_ScienceStaging_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   ├── C_ScienceStaging_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   ├── C_ScienceStaging_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   ├── C_ScienceStaging_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   ├── C_ScienceStaging_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   ├── G_Greenhouse_Accent_Cyan_Combined.asset
│   │       │   │   │   │   │   ├── G_Greenhouse_Accent_Cyan_Combined.asset.meta
│   │       │   │   │   │   │   ├── G_Greenhouse_Glass_Utility_Combined.asset
│   │       │   │   │   │   │   ├── G_Greenhouse_Glass_Utility_Combined.asset.meta
│   │       │   │   │   │   │   ├── G_Greenhouse_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   ├── G_Greenhouse_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   ├── G_Greenhouse_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   ├── G_Greenhouse_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   ├── G_Greenhouse_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   ├── G_Greenhouse_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   ├── H_CommandHabitat_Accent_Cyan_Combined.asset
│   │       │   │   │   │   │   ├── H_CommandHabitat_Accent_Cyan_Combined.asset.meta
│   │       │   │   │   │   │   ├── H_CommandHabitat_Glass_Utility_Combined.asset
│   │       │   │   │   │   │   ├── H_CommandHabitat_Glass_Utility_Combined.asset.meta
│   │       │   │   │   │   │   ├── H_CommandHabitat_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   ├── H_CommandHabitat_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   ├── H_CommandHabitat_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   ├── H_CommandHabitat_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   ├── H_CommandHabitat_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   ├── H_CommandHabitat_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   ├── HabitatH01_ChamferShell.asset
│   │       │   │   │   │   │   ├── HabitatH01_ChamferShell.asset.meta
│   │       │   │   │   │   │   ├── HabitatH01_Ring.asset
│   │       │   │   │   │   │   ├── HabitatH01_Ring.asset.meta
│   │       │   │   │   │   │   ├── P_PowerStation_Accent_Cyan_Combined.asset
│   │       │   │   │   │   │   ├── P_PowerStation_Accent_Cyan_Combined.asset.meta
│   │       │   │   │   │   │   ├── P_PowerStation_Accent_SafetyOrange_Combined.asset
│   │       │   │   │   │   │   ├── P_PowerStation_Accent_SafetyOrange_Combined.asset.meta
│   │       │   │   │   │   │   ├── P_PowerStation_Glass_Utility_Combined.asset
│   │       │   │   │   │   │   ├── P_PowerStation_Glass_Utility_Combined.asset.meta
│   │       │   │   │   │   │   ├── P_PowerStation_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   ├── P_PowerStation_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   ├── P_PowerStation_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   ├── P_PowerStation_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   ├── P_PowerStation_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   ├── P_PowerStation_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   ├── R_RoverBay_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   ├── R_RoverBay_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   ├── R_RoverBay_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   ├── R_RoverBay_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   ├── R_RoverBay_Mars_Regolith_Dark_Combined.asset
│   │       │   │   │   │   │   ├── R_RoverBay_Mars_Regolith_Dark_Combined.asset.meta
│   │       │   │   │   │   │   ├── R_RoverBay_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── R_RoverBay_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── R_RoverBay_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   ├── R_RoverBay_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   ├── S_SolarArray_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   ├── S_SolarArray_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   ├── S_SolarArray_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   ├── S_SolarArray_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   ├── S_SolarArray_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   ├── S_SolarArray_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   ├── S_SolarArray_SolarPanel_Combined.asset
│   │       │   │   │   │   │   └── S_SolarArray_SolarPanel_Combined.asset.meta
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── MarsEnvironmentBuilder.cs
│   │       │   │   │   │   │   └── MarsEnvironmentBuilder.cs.meta
│   │       │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   ├── Accent_Cyan.mat
│   │       │   │   │   │   │   ├── Accent_Cyan.mat.meta
│   │       │   │   │   │   │   ├── Accent_SafetyOrange.mat
│   │       │   │   │   │   │   ├── Accent_SafetyOrange.mat.meta
│   │       │   │   │   │   │   ├── Basalt_Surface_512.png
│   │       │   │   │   │   │   ├── Basalt_Surface_512.png.meta
│   │       │   │   │   │   │   ├── Composite_DustPaint_512.png
│   │       │   │   │   │   │   ├── Composite_DustPaint_512.png.meta
│   │       │   │   │   │   │   ├── Delta_Bedding_512.png
│   │       │   │   │   │   │   ├── Delta_Bedding_512.png.meta
│   │       │   │   │   │   │   ├── Delta_FracturedStrata_512.png
│   │       │   │   │   │   │   ├── Delta_FracturedStrata_512.png.meta
│   │       │   │   │   │   │   ├── DustSky.shader
│   │       │   │   │   │   │   ├── DustSky.shader.meta
│   │       │   │   │   │   │   ├── Glass_Utility.mat
│   │       │   │   │   │   │   ├── Glass_Utility.mat.meta
│   │       │   │   │   │   │   ├── Habitat_CompositeDark.mat
│   │       │   │   │   │   │   ├── Habitat_CompositeDark.mat.meta
│   │       │   │   │   │   │   ├── Habitat_OffWhite.mat
│   │       │   │   │   │   │   ├── Habitat_OffWhite.mat.meta
│   │       │   │   │   │   │   ├── Mars_DustSky.mat
│   │       │   │   │   │   │   ├── Mars_DustSky.mat.meta
│   │       │   │   │   │   │   ├── Mars_Regolith.mat
│   │       │   │   │   │   │   ├── Mars_Regolith.mat.meta
│   │       │   │   │   │   │   ├── Mars_Regolith_Dark.mat
│   │       │   │   │   │   │   ├── Mars_Regolith_Dark.mat.meta
│   │       │   │   │   │   │   ├── Mars_Rock_Basaltic.mat
│   │       │   │   │   │   │   ├── Mars_Rock_Basaltic.mat.meta
│   │       │   │   │   │   │   ├── Mars_Rock_Layered.mat
│   │       │   │   │   │   │   ├── Mars_Rock_Layered.mat.meta
│   │       │   │   │   │   │   ├── Metal_Industrial.mat
│   │       │   │   │   │   │   ├── Metal_Industrial.mat.meta
│   │       │   │   │   │   │   ├── Regolith_Grain_512.png
│   │       │   │   │   │   │   ├── Regolith_Grain_512.png.meta
│   │       │   │   │   │   │   ├── Regolith_Macro_Pathwear_1024.png
│   │       │   │   │   │   │   ├── Regolith_Macro_Pathwear_1024.png.meta
│   │       │   │   │   │   │   ├── Regolith_MicroNormal_256.png
│   │       │   │   │   │   │   ├── Regolith_MicroNormal_256.png.meta
│   │       │   │   │   │   │   ├── Solar_Cells_Dust_512.png
│   │       │   │   │   │   │   ├── Solar_Cells_Dust_512.png.meta
│   │       │   │   │   │   │   ├── SolarPanel.mat
│   │       │   │   │   │   │   └── SolarPanel.mat.meta
│   │       │   │   │   │   ├── Prefabs/
│   │       │   │   │   │   │   ├── AngularBasalt.prefab
│   │       │   │   │   │   │   ├── AngularBasalt.prefab.meta
│   │       │   │   │   │   │   ├── C_ScienceStaging.prefab
│   │       │   │   │   │   │   ├── C_ScienceStaging.prefab.meta
│   │       │   │   │   │   │   ├── ErodedLandmark.prefab
│   │       │   │   │   │   │   ├── ErodedLandmark.prefab.meta
│   │       │   │   │   │   │   ├── G_Greenhouse.prefab
│   │       │   │   │   │   │   ├── G_Greenhouse.prefab.meta
│   │       │   │   │   │   │   ├── H_CommandHabitat.prefab
│   │       │   │   │   │   │   ├── H_CommandHabitat.prefab.meta
│   │       │   │   │   │   │   ├── LayeredDelta.prefab
│   │       │   │   │   │   │   ├── LayeredDelta.prefab.meta
│   │       │   │   │   │   │   ├── P_PowerStation.prefab
│   │       │   │   │   │   │   ├── P_PowerStation.prefab.meta
│   │       │   │   │   │   │   ├── R_RoverBay.prefab
│   │       │   │   │   │   │   ├── R_RoverBay.prefab.meta
│   │       │   │   │   │   │   ├── RegolithStone.prefab
│   │       │   │   │   │   │   ├── RegolithStone.prefab.meta
│   │       │   │   │   │   │   ├── S_SolarArray.prefab
│   │       │   │   │   │   │   └── S_SolarArray.prefab.meta
│   │       │   │   │   │   ├── Props/
│   │       │   │   │   │   │   ├── ApproachStoneClusters_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── ApproachStoneClusters_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── HabitatExternalKit_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   ├── HabitatExternalKit_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   ├── HabitatExternalKit_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   ├── HabitatExternalKit_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   ├── HabitatExternalKit_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   ├── HabitatExternalKit_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   ├── PowerExternalKit_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   ├── PowerExternalKit_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   ├── PowerExternalKit_Mars_Regolith_Dark_Combined.asset
│   │       │   │   │   │   │   ├── PowerExternalKit_Mars_Regolith_Dark_Combined.asset.meta
│   │       │   │   │   │   │   ├── UtilityRoutePosts_Accent_Cyan_Combined.asset
│   │       │   │   │   │   │   ├── UtilityRoutePosts_Accent_Cyan_Combined.asset.meta
│   │       │   │   │   │   │   ├── UtilityRoutePosts_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   └── UtilityRoutePosts_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   ├── Rocks/
│   │       │   │   │   │   │   ├── AngularBasalt_LOD0.asset
│   │       │   │   │   │   │   ├── AngularBasalt_LOD0.asset.meta
│   │       │   │   │   │   │   ├── AngularBasalt_LOD1.asset
│   │       │   │   │   │   │   ├── AngularBasalt_LOD1.asset.meta
│   │       │   │   │   │   │   ├── AngularBasalt_LOD2.asset
│   │       │   │   │   │   │   ├── AngularBasalt_LOD2.asset.meta
│   │       │   │   │   │   │   ├── DeltaFractured_LOD0.asset
│   │       │   │   │   │   │   ├── DeltaFractured_LOD0.asset.meta
│   │       │   │   │   │   │   ├── DeltaFractured_LOD1.asset
│   │       │   │   │   │   │   ├── DeltaFractured_LOD1.asset.meta
│   │       │   │   │   │   │   ├── DeltaFractured_LOD2.asset
│   │       │   │   │   │   │   ├── DeltaFractured_LOD2.asset.meta
│   │       │   │   │   │   │   ├── ErodedLandmark_LOD0.asset
│   │       │   │   │   │   │   ├── ErodedLandmark_LOD0.asset.meta
│   │       │   │   │   │   │   ├── ErodedLandmark_LOD1.asset
│   │       │   │   │   │   │   ├── ErodedLandmark_LOD1.asset.meta
│   │       │   │   │   │   │   ├── ErodedLandmark_LOD2.asset
│   │       │   │   │   │   │   ├── ErodedLandmark_LOD2.asset.meta
│   │       │   │   │   │   │   ├── LayeredDelta_LOD0.asset
│   │       │   │   │   │   │   ├── LayeredDelta_LOD0.asset.meta
│   │       │   │   │   │   │   ├── LayeredDelta_LOD1.asset
│   │       │   │   │   │   │   ├── LayeredDelta_LOD1.asset.meta
│   │       │   │   │   │   │   ├── LayeredDelta_LOD2.asset
│   │       │   │   │   │   │   ├── LayeredDelta_LOD2.asset.meta
│   │       │   │   │   │   │   ├── StonePatch_0_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── StonePatch_0_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── StonePatch_1_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── StonePatch_1_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── StonePatch_2_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── StonePatch_2_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── StonePatch_3_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── StonePatch_3_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── StonePatch_4_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── StonePatch_4_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── StonePatch_5_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── StonePatch_5_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── StonePatch_6_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── StonePatch_6_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── StonePatch_7_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── StonePatch_7_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── StonePatch_8_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   └── StonePatch_8_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   ├── Terrain/
│   │       │   │   │   │   │   ├── Regolith_250m_PreservedCorridor.asset
│   │       │   │   │   │   │   └── Regolith_250m_PreservedCorridor.asset.meta
│   │       │   │   │   │   ├── Architecture.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   ├── Prefabs.meta
│   │       │   │   │   │   ├── Props.meta
│   │       │   │   │   │   ├── Rocks.meta
│   │       │   │   │   │   └── Terrain.meta
│   │       │   │   │   ├── Greybox/
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── MarsGreyboxBuilder.cs
│   │       │   │   │   │   │   └── MarsGreyboxBuilder.cs.meta
│   │       │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   ├── DarkRock.mat
│   │       │   │   │   │   │   ├── DarkRock.mat.meta
│   │       │   │   │   │   │   ├── HabitatPale.mat
│   │       │   │   │   │   │   ├── HabitatPale.mat.meta
│   │       │   │   │   │   │   ├── IndustrialGraphite.mat
│   │       │   │   │   │   │   ├── IndustrialGraphite.mat.meta
│   │       │   │   │   │   │   ├── LayeredStone.mat
│   │       │   │   │   │   │   ├── LayeredStone.mat.meta
│   │       │   │   │   │   │   ├── SandTan.mat
│   │       │   │   │   │   │   ├── SandTan.mat.meta
│   │       │   │   │   │   │   ├── TerrainOchre.mat
│   │       │   │   │   │   │   ├── TerrainOchre.mat.meta
│   │       │   │   │   │   │   ├── WayfindingAmber.mat
│   │       │   │   │   │   │   ├── WayfindingAmber.mat.meta
│   │       │   │   │   │   │   ├── WayfindingCyan.mat
│   │       │   │   │   │   │   └── WayfindingCyan.mat.meta
│   │       │   │   │   │   ├── Meshes/
│   │       │   │   │   │   │   ├── Ground.asset
│   │       │   │   │   │   │   └── Ground.asset.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── MarsOutpost_Greybox.unity
│   │       │   │   │   │   ├── MarsOutpost_Greybox.unity.meta
│   │       │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   ├── Meshes.meta
│   │       │   │   │   │   ├── PreGreybox_Untitled_Backup.unity
│   │       │   │   │   │   └── PreGreybox_Untitled_Backup.unity.meta
│   │       │   │   │   ├── Integration/
│   │       │   │   │   │   ├── Contracts/
│   │       │   │   │   │   │   ├── IntegrationContracts.cs
│   │       │   │   │   │   │   └── IntegrationContracts.cs.meta
│   │       │   │   │   │   ├── Local/
│   │       │   │   │   │   │   ├── GameplayEventBridge.cs
│   │       │   │   │   │   │   ├── GameplayEventBridge.cs.meta
│   │       │   │   │   │   │   ├── LifeSupportModelAdapter.cs
│   │       │   │   │   │   │   ├── LifeSupportModelAdapter.cs.meta
│   │       │   │   │   │   │   ├── LocalServices.cs
│   │       │   │   │   │   │   └── LocalServices.cs.meta
│   │       │   │   │   │   ├── Contracts.meta
│   │       │   │   │   │   └── Local.meta
│   │       │   │   │   ├── MarsEnvironment/
│   │       │   │   │   │   ├── Documentation/
│   │       │   │   │   │   │   ├── MissionRegistry.asset
│   │       │   │   │   │   │   ├── MissionRegistry.asset.meta
│   │       │   │   │   │   │   ├── README.md
│   │       │   │   │   │   │   └── README.md.meta
│   │       │   │   │   │   ├── Lighting/
│   │       │   │   │   │   │   ├── Dusty_Conditions.asset
│   │       │   │   │   │   │   ├── Dusty_Conditions.asset.meta
│   │       │   │   │   │   │   ├── MarsGradient.shader
│   │       │   │   │   │   │   ├── MarsGradient.shader.meta
│   │       │   │   │   │   │   ├── MarsSky.mat
│   │       │   │   │   │   │   ├── MarsSky.mat.meta
│   │       │   │   │   │   │   ├── MarsVolume.asset
│   │       │   │   │   │   │   ├── MarsVolume.asset.meta
│   │       │   │   │   │   │   ├── Midday.asset
│   │       │   │   │   │   │   ├── Midday.asset.meta
│   │       │   │   │   │   │   ├── Morning.asset
│   │       │   │   │   │   │   ├── Morning.asset.meta
│   │       │   │   │   │   │   ├── Sunset.asset
│   │       │   │   │   │   │   └── Sunset.asset.meta
│   │       │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   ├── DistantDust.asset
│   │       │   │   │   │   │   ├── DistantDust.asset.meta
│   │       │   │   │   │   │   ├── Dusty solar cells.mat
│   │       │   │   │   │   │   ├── Dusty solar cells.mat.meta
│   │       │   │   │   │   │   ├── EnvironmentDust.mat
│   │       │   │   │   │   │   ├── EnvironmentDust.mat.meta
│   │       │   │   │   │   │   ├── Fine Mars dust.mat
│   │       │   │   │   │   │   ├── Fine Mars dust.mat.meta
│   │       │   │   │   │   │   ├── Habitat ceramic composite.mat
│   │       │   │   │   │   │   ├── Habitat ceramic composite.mat.meta
│   │       │   │   │   │   │   ├── Observation panels — opaque approximation.mat
│   │       │   │   │   │   │   ├── Observation panels — opaque approximation.mat.meta
│   │       │   │   │   │   │   ├── Restrained identification ochre.mat
│   │       │   │   │   │   │   ├── Restrained identification ochre.mat.meta
│   │       │   │   │   │   │   ├── Rock07 dusty basalt.mat
│   │       │   │   │   │   │   ├── Rock07 dusty basalt.mat.meta
│   │       │   │   │   │   │   ├── Rock09 compact mineral.mat
│   │       │   │   │   │   │   ├── Rock09 compact mineral.mat.meta
│   │       │   │   │   │   │   ├── Rover_Cab.mat
│   │       │   │   │   │   │   ├── Rover_Cab.mat.meta
│   │       │   │   │   │   │   ├── Rover_Chassis.mat
│   │       │   │   │   │   │   ├── Rover_Chassis.mat.meta
│   │       │   │   │   │   │   ├── Rover_RoofMarker.mat
│   │       │   │   │   │   │   ├── Rover_RoofMarker.mat.meta
│   │       │   │   │   │   │   ├── Rover_Wheel_-1_0.mat
│   │       │   │   │   │   │   ├── Rover_Wheel_-1_0.mat.meta
│   │       │   │   │   │   │   ├── Rover_Wheel_-1_1.mat
│   │       │   │   │   │   │   ├── Rover_Wheel_-1_1.mat.meta
│   │       │   │   │   │   │   ├── Rover_Wheel_-1_2.mat
│   │       │   │   │   │   │   ├── Rover_Wheel_-1_2.mat.meta
│   │       │   │   │   │   │   ├── Rover_Wheel_1_0.mat
│   │       │   │   │   │   │   ├── Rover_Wheel_1_0.mat.meta
│   │       │   │   │   │   │   ├── Rover_Wheel_1_1.mat
│   │       │   │   │   │   │   ├── Rover_Wheel_1_1.mat.meta
│   │       │   │   │   │   │   ├── Rover_Wheel_1_2.mat
│   │       │   │   │   │   │   ├── Rover_Wheel_1_2.mat.meta
│   │       │   │   │   │   │   ├── Structural graphite.mat
│   │       │   │   │   │   │   └── Structural graphite.mat.meta
│   │       │   │   │   │   ├── Models/
│   │       │   │   │   │   │   ├── Crew_Dome_Ribs_LowPoly.asset
│   │       │   │   │   │   │   ├── Crew_Dome_Ribs_LowPoly.asset.meta
│   │       │   │   │   │   │   ├── HabitatHemisphere.asset
│   │       │   │   │   │   │   ├── HabitatHemisphere.asset.meta
│   │       │   │   │   │   │   ├── Laboratory_Dome_Ribs_LowPoly.asset
│   │       │   │   │   │   │   ├── Laboratory_Dome_Ribs_LowPoly.asset.meta
│   │       │   │   │   │   │   ├── Life_Support_Dome_Ribs_LowPoly.asset
│   │       │   │   │   │   │   ├── Life_Support_Dome_Ribs_LowPoly.asset.meta
│   │       │   │   │   │   │   ├── rock_07_LOD0.asset
│   │       │   │   │   │   │   ├── rock_07_LOD0.asset.meta
│   │       │   │   │   │   │   ├── rock_07_LOD1.asset
│   │       │   │   │   │   │   ├── rock_07_LOD1.asset.meta
│   │       │   │   │   │   │   ├── rock_07_LOD2.asset
│   │       │   │   │   │   │   ├── rock_07_LOD2.asset.meta
│   │       │   │   │   │   │   ├── rock_09_LOD0.asset
│   │       │   │   │   │   │   ├── rock_09_LOD0.asset.meta
│   │       │   │   │   │   │   ├── rock_09_LOD1.asset
│   │       │   │   │   │   │   ├── rock_09_LOD1.asset.meta
│   │       │   │   │   │   │   ├── rock_09_LOD2.asset
│   │       │   │   │   │   │   ├── rock_09_LOD2.asset.meta
│   │       │   │   │   │   │   ├── Terrain250m.asset
│   │       │   │   │   │   │   └── Terrain250m.asset.meta
│   │       │   │   │   │   ├── Prefabs/
│   │       │   │   │   │   │   ├── ARESYN_ClosedHabitatComplex_Optimized.prefab
│   │       │   │   │   │   │   ├── ARESYN_ClosedHabitatComplex_Optimized.prefab.meta
│   │       │   │   │   │   │   ├── SolarFarm_Optimized.prefab
│   │       │   │   │   │   │   └── SolarFarm_Optimized.prefab.meta
│   │       │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   ├── Mars_RealisticResearchBase_v1.unity
│   │       │   │   │   │   │   └── Mars_RealisticResearchBase_v1.unity.meta
│   │       │   │   │   │   ├── Scripts/
│   │       │   │   │   │   │   ├── EnvironmentPresentation.cs
│   │       │   │   │   │   │   ├── EnvironmentPresentation.cs.meta
│   │       │   │   │   │   │   ├── EnvironmentPreset.cs
│   │       │   │   │   │   │   ├── EnvironmentPreset.cs.meta
│   │       │   │   │   │   │   ├── ResearchTarget.cs
│   │       │   │   │   │   │   └── ResearchTarget.cs.meta
│   │       │   │   │   │   ├── Tests/
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── MarsResearchBaseBuilder.cs
│   │       │   │   │   │   │   │   └── MarsResearchBaseBuilder.cs.meta
│   │       │   │   │   │   │   └── Editor.meta
│   │       │   │   │   │   ├── Textures/
│   │       │   │   │   │   │   ├── rock_07_AO.png
│   │       │   │   │   │   │   ├── rock_07_AO.png.meta
│   │       │   │   │   │   │   ├── rock_07_Diffuse.png
│   │       │   │   │   │   │   ├── rock_07_Diffuse.png.meta
│   │       │   │   │   │   │   ├── rock_07_nor_gl.png
│   │       │   │   │   │   │   ├── rock_07_nor_gl.png.meta
│   │       │   │   │   │   │   ├── rock_07_Rough.png
│   │       │   │   │   │   │   ├── rock_07_Rough.png.meta
│   │       │   │   │   │   │   ├── rock_09_AO.png
│   │       │   │   │   │   │   ├── rock_09_AO.png.meta
│   │       │   │   │   │   │   ├── rock_09_Diffuse.png
│   │       │   │   │   │   │   ├── rock_09_Diffuse.png.meta
│   │       │   │   │   │   │   ├── rock_09_nor_gl.png
│   │       │   │   │   │   │   ├── rock_09_nor_gl.png.meta
│   │       │   │   │   │   │   ├── rock_09_Rough.png
│   │       │   │   │   │   │   ├── rock_09_Rough.png.meta
│   │       │   │   │   │   │   ├── sandy_gravel_AO.png
│   │       │   │   │   │   │   ├── sandy_gravel_AO.png.meta
│   │       │   │   │   │   │   ├── sandy_gravel_Diffuse.png
│   │       │   │   │   │   │   ├── sandy_gravel_Diffuse.png.meta
│   │       │   │   │   │   │   ├── sandy_gravel_nor_gl.png
│   │       │   │   │   │   │   ├── sandy_gravel_nor_gl.png.meta
│   │       │   │   │   │   │   ├── sandy_gravel_Rough.png
│   │       │   │   │   │   │   └── sandy_gravel_Rough.png.meta
│   │       │   │   │   │   ├── Documentation.meta
│   │       │   │   │   │   ├── Lighting.meta
│   │       │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   ├── Models.meta
│   │       │   │   │   │   ├── Prefabs.meta
│   │       │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   ├── Scripts.meta
│   │       │   │   │   │   ├── Tests.meta
│   │       │   │   │   │   ├── Textures.meta
│   │       │   │   │   │   └── VFX.meta
│   │       │   │   │   ├── MarsVisualPolish/
│   │       │   │   │   │   ├── Documentation/
│   │       │   │   │   │   │   ├── PolishRegistry.asset
│   │       │   │   │   │   │   └── PolishRegistry.asset.meta
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── VisualPolishBuilder.cs
│   │       │   │   │   │   │   └── VisualPolishBuilder.cs.meta
│   │       │   │   │   │   ├── Lighting/
│   │       │   │   │   │   │   ├── PolishSky.mat
│   │       │   │   │   │   │   ├── PolishSky.mat.meta
│   │       │   │   │   │   │   ├── PolishVolume.asset
│   │       │   │   │   │   │   ├── PolishVolume.asset.meta
│   │       │   │   │   │   │   ├── Preset_0.asset
│   │       │   │   │   │   │   ├── Preset_0.asset.meta
│   │       │   │   │   │   │   ├── Preset_1.asset
│   │       │   │   │   │   │   ├── Preset_1.asset.meta
│   │       │   │   │   │   │   ├── Preset_2.asset
│   │       │   │   │   │   │   ├── Preset_2.asset.meta
│   │       │   │   │   │   │   ├── Preset_3.asset
│   │       │   │   │   │   │   └── Preset_3.asset.meta
│   │       │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   ├── Brushed frame.mat
│   │       │   │   │   │   │   ├── Brushed frame.mat.meta
│   │       │   │   │   │   │   ├── Derivative_0.mat
│   │       │   │   │   │   │   ├── Derivative_0.mat.meta
│   │       │   │   │   │   │   ├── Derivative_1.mat
│   │       │   │   │   │   │   ├── Derivative_1.mat.meta
│   │       │   │   │   │   │   ├── Derivative_10.mat
│   │       │   │   │   │   │   ├── Derivative_10.mat.meta
│   │       │   │   │   │   │   ├── Derivative_11.mat
│   │       │   │   │   │   │   ├── Derivative_11.mat.meta
│   │       │   │   │   │   │   ├── Derivative_12.mat
│   │       │   │   │   │   │   ├── Derivative_12.mat.meta
│   │       │   │   │   │   │   ├── Derivative_13.mat
│   │       │   │   │   │   │   ├── Derivative_13.mat.meta
│   │       │   │   │   │   │   ├── Derivative_14.mat
│   │       │   │   │   │   │   ├── Derivative_14.mat.meta
│   │       │   │   │   │   │   ├── Derivative_15.mat
│   │       │   │   │   │   │   ├── Derivative_15.mat.meta
│   │       │   │   │   │   │   ├── Derivative_16.mat
│   │       │   │   │   │   │   ├── Derivative_16.mat.meta
│   │       │   │   │   │   │   ├── Derivative_17.mat
│   │       │   │   │   │   │   ├── Derivative_17.mat.meta
│   │       │   │   │   │   │   ├── Derivative_18.mat
│   │       │   │   │   │   │   ├── Derivative_18.mat.meta
│   │       │   │   │   │   │   ├── Derivative_19.mat
│   │       │   │   │   │   │   ├── Derivative_19.mat.meta
│   │       │   │   │   │   │   ├── Derivative_2.mat
│   │       │   │   │   │   │   ├── Derivative_2.mat.meta
│   │       │   │   │   │   │   ├── Derivative_20.mat
│   │       │   │   │   │   │   ├── Derivative_20.mat.meta
│   │       │   │   │   │   │   ├── Derivative_21.mat
│   │       │   │   │   │   │   ├── Derivative_21.mat.meta
│   │       │   │   │   │   │   ├── Derivative_3.mat
│   │       │   │   │   │   │   ├── Derivative_3.mat.meta
│   │       │   │   │   │   │   ├── Derivative_4.mat
│   │       │   │   │   │   │   ├── Derivative_4.mat.meta
│   │       │   │   │   │   │   ├── Derivative_5.mat
│   │       │   │   │   │   │   ├── Derivative_5.mat.meta
│   │       │   │   │   │   │   ├── Derivative_6.mat
│   │       │   │   │   │   │   ├── Derivative_6.mat.meta
│   │       │   │   │   │   │   ├── Derivative_7.mat
│   │       │   │   │   │   │   ├── Derivative_7.mat.meta
│   │       │   │   │   │   │   ├── Derivative_8.mat
│   │       │   │   │   │   │   ├── Derivative_8.mat.meta
│   │       │   │   │   │   │   ├── Derivative_9.mat
│   │       │   │   │   │   │   ├── Derivative_9.mat.meta
│   │       │   │   │   │   │   ├── Photovoltaic cell laminate.mat
│   │       │   │   │   │   │   ├── Photovoltaic cell laminate.mat.meta
│   │       │   │   │   │   │   ├── PhotovoltaicGrid.png
│   │       │   │   │   │   │   ├── PhotovoltaicGrid.png.meta
│   │       │   │   │   │   │   ├── Recessed seals.mat
│   │       │   │   │   │   │   ├── Recessed seals.mat.meta
│   │       │   │   │   │   │   ├── Safety ochre.mat
│   │       │   │   │   │   │   ├── Safety ochre.mat.meta
│   │       │   │   │   │   │   ├── Warm ceramic.mat
│   │       │   │   │   │   │   └── Warm ceramic.mat.meta
│   │       │   │   │   │   ├── Models/
│   │       │   │   │   │   │   ├── Crew Dome_Frame.asset
│   │       │   │   │   │   │   ├── Crew Dome_Frame.asset.meta
│   │       │   │   │   │   │   ├── Laboratory Dome_Frame.asset
│   │       │   │   │   │   │   ├── Laboratory Dome_Frame.asset.meta
│   │       │   │   │   │   │   ├── Life Support Dome_Frame.asset
│   │       │   │   │   │   │   └── Life Support Dome_Frame.asset.meta
│   │       │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   ├── Mars_VisualPolish_v2.unity
│   │       │   │   │   │   │   └── Mars_VisualPolish_v2.unity.meta
│   │       │   │   │   │   ├── Documentation.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── Lighting.meta
│   │       │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   ├── Models.meta
│   │       │   │   │   │   ├── PolishBenchmark.cs
│   │       │   │   │   │   ├── PolishBenchmark.cs.meta
│   │       │   │   │   │   ├── PolishHudTheme.cs
│   │       │   │   │   │   ├── PolishHudTheme.cs.meta
│   │       │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   ├── VisualPolishPresentation.cs
│   │       │   │   │   │   └── VisualPolishPresentation.cs.meta
│   │       │   │   │   ├── Mission01Milestone/
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── Mission01Builder.cs
│   │       │   │   │   │   │   └── Mission01Builder.cs.meta
│   │       │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   ├── Mission01_Continents.mat
│   │       │   │   │   │   │   ├── Mission01_Continents.mat.meta
│   │       │   │   │   │   │   ├── Mission01_Graphite.mat
│   │       │   │   │   │   │   ├── Mission01_Graphite.mat.meta
│   │       │   │   │   │   │   ├── Mission01_Ocean.mat
│   │       │   │   │   │   │   ├── Mission01_Ocean.mat.meta
│   │       │   │   │   │   │   ├── Mission01_Pale.mat
│   │       │   │   │   │   │   ├── Mission01_Pale.mat.meta
│   │       │   │   │   │   │   ├── Mission01_Safety.mat
│   │       │   │   │   │   │   └── Mission01_Safety.mat.meta
│   │       │   │   │   │   ├── Prefabs/
│   │       │   │   │   │   │   ├── Boarding_Console.prefab
│   │       │   │   │   │   │   ├── Boarding_Console.prefab.meta
│   │       │   │   │   │   │   ├── Trainee_Blockout.prefab
│   │       │   │   │   │   │   └── Trainee_Blockout.prefab.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   ├── Mission01_EarthLaunch_Milestone_v4.unity
│   │       │   │   │   │   ├── Mission01_EarthLaunch_Milestone_v4.unity.meta
│   │       │   │   │   │   ├── Mission01Flow.cs
│   │       │   │   │   │   ├── Mission01Flow.cs.meta
│   │       │   │   │   │   └── Prefabs.meta
│   │       │   │   │   ├── MissionIntegration/
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── MissionIntegrationBuilder.cs
│   │       │   │   │   │   │   └── MissionIntegrationBuilder.cs.meta
│   │       │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   ├── Mars_MissionIntegration_v1.unity
│   │       │   │   │   │   │   └── Mars_MissionIntegration_v1.unity.meta
│   │       │   │   │   │   ├── Scripts/
│   │       │   │   │   │   │   ├── MissionIntegrationController.cs
│   │       │   │   │   │   │   └── MissionIntegrationController.cs.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   └── Scripts.meta
│   │       │   │   │   ├── NativeControlSlice/
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── NativeSliceBuilder.cs
│   │       │   │   │   │   │   └── NativeSliceBuilder.cs.meta
│   │       │   │   │   │   ├── Prefabs/
│   │       │   │   │   │   │   ├── AdaptiveTouch.prefab
│   │       │   │   │   │   │   └── AdaptiveTouch.prefab.meta
│   │       │   │   │   │   ├── AdaptiveTouch.cs
│   │       │   │   │   │   ├── AdaptiveTouch.cs.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── Mars_NativeControl_VerticalSlice.unity
│   │       │   │   │   │   ├── Mars_NativeControl_VerticalSlice.unity.meta
│   │       │   │   │   │   ├── MissionRegistry.asset
│   │       │   │   │   │   ├── MissionRegistry.asset.meta
│   │       │   │   │   │   ├── NativeSliceGame.cs
│   │       │   │   │   │   ├── NativeSliceGame.cs.meta
│   │       │   │   │   │   ├── NativeSystems.cs
│   │       │   │   │   │   ├── NativeSystems.cs.meta
│   │       │   │   │   │   └── Prefabs.meta
│   │       │   │   │   ├── ProductionVisual/
│   │       │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   ├── Brushed aluminum.mat
│   │       │   │   │   │   │   ├── Brushed aluminum.mat.meta
│   │       │   │   │   │   │   ├── Dusty tire elastomer.mat
│   │       │   │   │   │   │   ├── Dusty tire elastomer.mat.meta
│   │       │   │   │   │   │   ├── Inherited_0.mat
│   │       │   │   │   │   │   ├── Inherited_0.mat.meta
│   │       │   │   │   │   │   ├── Inherited_1.mat
│   │       │   │   │   │   │   ├── Inherited_1.mat.meta
│   │       │   │   │   │   │   ├── Inherited_10.mat
│   │       │   │   │   │   │   ├── Inherited_10.mat.meta
│   │       │   │   │   │   │   ├── Inherited_11.mat
│   │       │   │   │   │   │   ├── Inherited_11.mat.meta
│   │       │   │   │   │   │   ├── Inherited_12.mat
│   │       │   │   │   │   │   ├── Inherited_12.mat.meta
│   │       │   │   │   │   │   ├── Inherited_13.mat
│   │       │   │   │   │   │   ├── Inherited_13.mat.meta
│   │       │   │   │   │   │   ├── Inherited_14.mat
│   │       │   │   │   │   │   ├── Inherited_14.mat.meta
│   │       │   │   │   │   │   ├── Inherited_15.mat
│   │       │   │   │   │   │   ├── Inherited_15.mat.meta
│   │       │   │   │   │   │   ├── Inherited_16.mat
│   │       │   │   │   │   │   ├── Inherited_16.mat.meta
│   │       │   │   │   │   │   ├── Inherited_17.mat
│   │       │   │   │   │   │   ├── Inherited_17.mat.meta
│   │       │   │   │   │   │   ├── Inherited_18.mat
│   │       │   │   │   │   │   ├── Inherited_18.mat.meta
│   │       │   │   │   │   │   ├── Inherited_19.mat
│   │       │   │   │   │   │   ├── Inherited_19.mat.meta
│   │       │   │   │   │   │   ├── Inherited_2.mat
│   │       │   │   │   │   │   ├── Inherited_2.mat.meta
│   │       │   │   │   │   │   ├── Inherited_20.mat
│   │       │   │   │   │   │   ├── Inherited_20.mat.meta
│   │       │   │   │   │   │   ├── Inherited_21.mat
│   │       │   │   │   │   │   ├── Inherited_21.mat.meta
│   │       │   │   │   │   │   ├── Inherited_22.mat
│   │       │   │   │   │   │   ├── Inherited_22.mat.meta
│   │       │   │   │   │   │   ├── Inherited_23.mat
│   │       │   │   │   │   │   ├── Inherited_23.mat.meta
│   │       │   │   │   │   │   ├── Inherited_24.mat
│   │       │   │   │   │   │   ├── Inherited_24.mat.meta
│   │       │   │   │   │   │   ├── Inherited_25.mat
│   │       │   │   │   │   │   ├── Inherited_25.mat.meta
│   │       │   │   │   │   │   ├── Inherited_3.mat
│   │       │   │   │   │   │   ├── Inherited_3.mat.meta
│   │       │   │   │   │   │   ├── Inherited_4.mat
│   │       │   │   │   │   │   ├── Inherited_4.mat.meta
│   │       │   │   │   │   │   ├── Inherited_5.mat
│   │       │   │   │   │   │   ├── Inherited_5.mat.meta
│   │       │   │   │   │   │   ├── Inherited_6.mat
│   │       │   │   │   │   │   ├── Inherited_6.mat.meta
│   │       │   │   │   │   │   ├── Inherited_7.mat
│   │       │   │   │   │   │   ├── Inherited_7.mat.meta
│   │       │   │   │   │   │   ├── Inherited_8.mat
│   │       │   │   │   │   │   ├── Inherited_8.mat.meta
│   │       │   │   │   │   │   ├── Inherited_9.mat
│   │       │   │   │   │   │   ├── Inherited_9.mat.meta
│   │       │   │   │   │   │   ├── Machined graphite alloy.mat
│   │       │   │   │   │   │   ├── Machined graphite alloy.mat.meta
│   │       │   │   │   │   │   ├── Opaque protective visor glazing.mat
│   │       │   │   │   │   │   ├── Opaque protective visor glazing.mat.meta
│   │       │   │   │   │   │   ├── Pressure shell ceramic composite.mat
│   │       │   │   │   │   │   ├── Pressure shell ceramic composite.mat.meta
│   │       │   │   │   │   │   ├── Restrained identification orange.mat
│   │       │   │   │   │   │   └── Restrained identification orange.mat.meta
│   │       │   │   │   │   ├── Meshes/
│   │       │   │   │   │   │   ├── ChamferedEnclosure.asset
│   │       │   │   │   │   │   ├── ChamferedEnclosure.asset.meta
│   │       │   │   │   │   │   ├── CommsTruss.asset
│   │       │   │   │   │   │   ├── CommsTruss.asset.meta
│   │       │   │   │   │   │   ├── Crew_Dome_sealed_airlock_Rails.asset
│   │       │   │   │   │   │   ├── Crew_Dome_sealed_airlock_Rails.asset.meta
│   │       │   │   │   │   │   ├── CrewCabEnclosure.asset
│   │       │   │   │   │   │   ├── CrewCabEnclosure.asset.meta
│   │       │   │   │   │   │   ├── GeodesicFrame.asset
│   │       │   │   │   │   │   ├── GeodesicFrame.asset.meta
│   │       │   │   │   │   │   ├── GeodesicPanels.asset
│   │       │   │   │   │   │   ├── GeodesicPanels.asset.meta
│   │       │   │   │   │   │   ├── Laboratory_Dome_sealed_airlock_Rails.asset
│   │       │   │   │   │   │   ├── Laboratory_Dome_sealed_airlock_Rails.asset.meta
│   │       │   │   │   │   │   ├── Life_Support_Dome_sealed_airlock_Rails.asset
│   │       │   │   │   │   │   ├── Life_Support_Dome_sealed_airlock_Rails.asset.meta
│   │       │   │   │   │   │   ├── RoverMechanicalFrame.asset
│   │       │   │   │   │   │   ├── RoverMechanicalFrame.asset.meta
│   │       │   │   │   │   │   ├── RoverWheel_LOD0.asset
│   │       │   │   │   │   │   ├── RoverWheel_LOD0.asset.meta
│   │       │   │   │   │   │   ├── RoverWheel_LOD1.asset
│   │       │   │   │   │   │   ├── RoverWheel_LOD1.asset.meta
│   │       │   │   │   │   │   ├── RoverWheel_LOD2.asset
│   │       │   │   │   │   │   └── RoverWheel_LOD2.asset.meta
│   │       │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   ├── Mars_ProductionVisual_v1.unity
│   │       │   │   │   │   │   └── Mars_ProductionVisual_v1.unity.meta
│   │       │   │   │   │   ├── Scripts/
│   │       │   │   │   │   │   ├── ProductionCameraRig.cs
│   │       │   │   │   │   │   ├── ProductionCameraRig.cs.meta
│   │       │   │   │   │   │   ├── ProductionModeCoordinator.cs
│   │       │   │   │   │   │   ├── ProductionModeCoordinator.cs.meta
│   │       │   │   │   │   │   ├── ProductionPerformanceProbe.cs
│   │       │   │   │   │   │   ├── ProductionPerformanceProbe.cs.meta
│   │       │   │   │   │   │   ├── ProductionPresentation.cs
│   │       │   │   │   │   │   └── ProductionPresentation.cs.meta
│   │       │   │   │   │   ├── Settings/
│   │       │   │   │   │   │   ├── Lighting_0.asset
│   │       │   │   │   │   │   ├── Lighting_0.asset.meta
│   │       │   │   │   │   │   ├── Lighting_1.asset
│   │       │   │   │   │   │   ├── Lighting_1.asset.meta
│   │       │   │   │   │   │   ├── Lighting_2.asset
│   │       │   │   │   │   │   ├── Lighting_2.asset.meta
│   │       │   │   │   │   │   ├── Lighting_3.asset
│   │       │   │   │   │   │   ├── Lighting_3.asset.meta
│   │       │   │   │   │   │   ├── ProductionMissionRegistry.asset
│   │       │   │   │   │   │   └── ProductionMissionRegistry.asset.meta
│   │       │   │   │   │   ├── Tests/
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── ProductionMeshFactory.cs
│   │       │   │   │   │   │   │   ├── ProductionMeshFactory.cs.meta
│   │       │   │   │   │   │   │   ├── ProductionViewport.cs
│   │       │   │   │   │   │   │   ├── ProductionViewport.cs.meta
│   │       │   │   │   │   │   │   ├── ProductionVisualBuilder.cs
│   │       │   │   │   │   │   │   └── ProductionVisualBuilder.cs.meta
│   │       │   │   │   │   │   └── Editor.meta
│   │       │   │   │   │   ├── Textures/
│   │       │   │   │   │   │   ├── rock_07_MetallicSmoothness.png
│   │       │   │   │   │   │   ├── rock_07_MetallicSmoothness.png.meta
│   │       │   │   │   │   │   ├── rock_09_MetallicSmoothness.png
│   │       │   │   │   │   │   ├── rock_09_MetallicSmoothness.png.meta
│   │       │   │   │   │   │   ├── sandy_gravel_MetallicSmoothness.png
│   │       │   │   │   │   │   └── sandy_gravel_MetallicSmoothness.png.meta
│   │       │   │   │   │   ├── Themes/
│   │       │   │   │   │   │   ├── ProductionHUDTheme.asset
│   │       │   │   │   │   │   ├── ProductionHUDTheme.asset.meta
│   │       │   │   │   │   │   ├── ProductionSky.mat
│   │       │   │   │   │   │   ├── ProductionSky.mat.meta
│   │       │   │   │   │   │   ├── ProductionVolume.asset
│   │       │   │   │   │   │   └── ProductionVolume.asset.meta
│   │       │   │   │   │   ├── Documentation.meta
│   │       │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   ├── Meshes.meta
│   │       │   │   │   │   ├── Prefabs.meta
│   │       │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   ├── Scripts.meta
│   │       │   │   │   │   ├── Settings.meta
│   │       │   │   │   │   ├── Tests.meta
│   │       │   │   │   │   ├── Textures.meta
│   │       │   │   │   │   └── Themes.meta
│   │       │   │   │   ├── Scenes/
│   │       │   │   │   │   ├── MarsOutpost_Environment_v01.unity
│   │       │   │   │   │   └── MarsOutpost_Environment_v01.unity.meta
│   │       │   │   │   ├── Stage08B/
│   │       │   │   │   │   ├── Animation/
│   │       │   │   │   │   │   ├── Idle.anim
│   │       │   │   │   │   │   ├── Idle.anim.meta
│   │       │   │   │   │   │   ├── PrototypeLocomotion.controller
│   │       │   │   │   │   │   ├── PrototypeLocomotion.controller.meta
│   │       │   │   │   │   │   ├── Run.anim
│   │       │   │   │   │   │   ├── Run.anim.meta
│   │       │   │   │   │   │   ├── Walk.anim
│   │       │   │   │   │   │   └── Walk.anim.meta
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── Stage08BBuilder.cs
│   │       │   │   │   │   │   └── Stage08BBuilder.cs.meta
│   │       │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   ├── 08B_Accent_Cyan.mat
│   │       │   │   │   │   │   ├── 08B_Accent_Cyan.mat.meta
│   │       │   │   │   │   │   ├── 08B_Accent_SafetyOrange.mat
│   │       │   │   │   │   │   ├── 08B_Accent_SafetyOrange.mat.meta
│   │       │   │   │   │   │   ├── 08B_Glass_Utility.mat
│   │       │   │   │   │   │   ├── 08B_Glass_Utility.mat.meta
│   │       │   │   │   │   │   ├── 08B_Habitat_CompositeDark.mat
│   │       │   │   │   │   │   ├── 08B_Habitat_CompositeDark.mat.meta
│   │       │   │   │   │   │   ├── 08B_Habitat_OffWhite.mat
│   │       │   │   │   │   │   ├── 08B_Habitat_OffWhite.mat.meta
│   │       │   │   │   │   │   ├── 08B_Mars_Regolith.mat
│   │       │   │   │   │   │   ├── 08B_Mars_Regolith.mat.meta
│   │       │   │   │   │   │   ├── 08B_Mars_Regolith_Dark.mat
│   │       │   │   │   │   │   ├── 08B_Mars_Regolith_Dark.mat.meta
│   │       │   │   │   │   │   ├── 08B_Mars_Rock_Basaltic.mat
│   │       │   │   │   │   │   ├── 08B_Mars_Rock_Basaltic.mat.meta
│   │       │   │   │   │   │   ├── 08B_Mars_Rock_Layered.mat
│   │       │   │   │   │   │   ├── 08B_Mars_Rock_Layered.mat.meta
│   │       │   │   │   │   │   ├── 08B_Metal_Industrial.mat
│   │       │   │   │   │   │   ├── 08B_Metal_Industrial.mat.meta
│   │       │   │   │   │   │   ├── 08B_SolarPanel.mat
│   │       │   │   │   │   │   ├── 08B_SolarPanel.mat.meta
│   │       │   │   │   │   │   ├── BasaltVariationA.mat
│   │       │   │   │   │   │   ├── BasaltVariationA.mat.meta
│   │       │   │   │   │   │   ├── BasaltVariationB.mat
│   │       │   │   │   │   │   ├── BasaltVariationB.mat.meta
│   │       │   │   │   │   │   ├── BasaltVariationC.mat
│   │       │   │   │   │   │   ├── BasaltVariationC.mat.meta
│   │       │   │   │   │   │   ├── Dust_Particle.mat
│   │       │   │   │   │   │   ├── Dust_Particle.mat.meta
│   │       │   │   │   │   │   ├── Identification_Cyan.mat
│   │       │   │   │   │   │   ├── Identification_Cyan.mat.meta
│   │       │   │   │   │   │   ├── Safety_Orange.mat
│   │       │   │   │   │   │   ├── Safety_Orange.mat.meta
│   │       │   │   │   │   │   ├── Suit_Graphite.mat
│   │       │   │   │   │   │   ├── Suit_Graphite.mat.meta
│   │       │   │   │   │   │   ├── Suit_Pale.mat
│   │       │   │   │   │   │   └── Suit_Pale.mat.meta
│   │       │   │   │   │   ├── Animation.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── MarsOutpost_Playable_Stage08B.unity
│   │       │   │   │   │   ├── MarsOutpost_Playable_Stage08B.unity.meta
│   │       │   │   │   │   ├── MarsPrototype.cs
│   │       │   │   │   │   ├── MarsPrototype.cs.meta
│   │       │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   ├── Stage08B_MobilePreview.asset
│   │       │   │   │   │   └── Stage08B_MobilePreview.asset.meta
│   │       │   │   │   ├── Stage08C/
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── AndroidProbeBuild.cs
│   │       │   │   │   │   │   ├── AndroidProbeBuild.cs.meta
│   │       │   │   │   │   │   ├── TouchChecks.cs
│   │       │   │   │   │   │   └── TouchChecks.cs.meta
│   │       │   │   │   │   ├── AndroidProfileProbe.cs
│   │       │   │   │   │   ├── AndroidProfileProbe.cs.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── MobileControls.cs
│   │       │   │   │   │   └── MobileControls.cs.meta
│   │       │   │   │   ├── Stage08C1/
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── EditorPerformanceObserver.cs
│   │       │   │   │   │   │   ├── EditorPerformanceObserver.cs.meta
│   │       │   │   │   │   │   ├── ManualInputObserver.cs
│   │       │   │   │   │   │   └── ManualInputObserver.cs.meta
│   │       │   │   │   │   └── Editor.meta
│   │       │   │   │   ├── Verification/
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── FrameRateProbe.cs
│   │       │   │   │   │   │   ├── FrameRateProbe.cs.meta
│   │       │   │   │   │   │   ├── Step1GameplayVerification.cs
│   │       │   │   │   │   │   └── Step1GameplayVerification.cs.meta
│   │       │   │   │   │   └── Editor.meta
│   │       │   │   │   ├── CinematicHUD.meta
│   │       │   │   │   ├── Environment.meta
│   │       │   │   │   ├── Greybox.meta
│   │       │   │   │   ├── Integration.meta
│   │       │   │   │   ├── MarsEnvironment.meta
│   │       │   │   │   ├── MarsVisualPolish.meta
│   │       │   │   │   ├── Mission01Milestone.meta
│   │       │   │   │   ├── MissionIntegration.meta
│   │       │   │   │   ├── NativeControlSlice.meta
│   │       │   │   │   ├── ProductionVisual.meta
│   │       │   │   │   ├── Scenes.meta
│   │       │   │   │   ├── Stage08B.meta
│   │       │   │   │   ├── Stage08C.meta
│   │       │   │   │   ├── Stage08C1.meta
│   │       │   │   │   └── Verification.meta
│   │       │   │   ├── Scenes/
│   │       │   │   │   ├── SampleScene.unity
│   │       │   │   │   └── SampleScene.unity.meta
│   │       │   │   ├── Settings/
│   │       │   │   │   ├── DefaultVolumeProfile.asset
│   │       │   │   │   ├── DefaultVolumeProfile.asset.meta
│   │       │   │   │   ├── Mobile_Renderer.asset
│   │       │   │   │   ├── Mobile_Renderer.asset.meta
│   │       │   │   │   ├── Mobile_RPAsset.asset
│   │       │   │   │   ├── Mobile_RPAsset.asset.meta
│   │       │   │   │   ├── PC_Renderer.asset
│   │       │   │   │   ├── PC_Renderer.asset.meta
│   │       │   │   │   ├── PC_RPAsset.asset
│   │       │   │   │   ├── PC_RPAsset.asset.meta
│   │       │   │   │   ├── SampleSceneProfile.asset
│   │       │   │   │   ├── SampleSceneProfile.asset.meta
│   │       │   │   │   ├── UniversalRenderPipelineGlobalSettings.asset
│   │       │   │   │   └── UniversalRenderPipelineGlobalSettings.asset.meta
│   │       │   │   ├── TutorialInfo/
│   │       │   │   │   ├── Editor/
│   │       │   │   │   │   ├── ReadmeEditor.cs
│   │       │   │   │   │   └── ReadmeEditor.cs.meta
│   │       │   │   │   ├── Icons/
│   │       │   │   │   │   ├── URP.png
│   │       │   │   │   │   └── URP.png.meta
│   │       │   │   │   ├── StyleSheets/
│   │       │   │   │   │   ├── ReadmeEditor.uss
│   │       │   │   │   │   ├── ReadmeEditor.uss.meta
│   │       │   │   │   │   ├── ReadmeEditorDark.uss
│   │       │   │   │   │   ├── ReadmeEditorDark.uss.meta
│   │       │   │   │   │   ├── ReadmeEditorLight.uss
│   │       │   │   │   │   └── ReadmeEditorLight.uss.meta
│   │       │   │   │   ├── Editor.meta
│   │       │   │   │   ├── Icons.meta
│   │       │   │   │   ├── Layout.wlt
│   │       │   │   │   ├── Layout.wlt.meta
│   │       │   │   │   ├── Readme.cs
│   │       │   │   │   ├── Readme.cs.meta
│   │       │   │   │   └── StyleSheets.meta
│   │       │   │   ├── AstroGateway.meta
│   │       │   │   ├── InputSystem_Actions.inputactions
│   │       │   │   ├── InputSystem_Actions.inputactions.meta
│   │       │   │   ├── Readme.asset
│   │       │   │   ├── Readme.asset.meta
│   │       │   │   ├── Scenes.meta
│   │       │   │   ├── Settings.meta
│   │       │   │   └── TutorialInfo.meta
│   │       │   ├── Data/
│   │       │   │   └── Plugins/
│   │       │   │       ├── lib_burst_generated.cpp
│   │       │   │       └── lib_burst_generated.wasm
│   │       │   ├── Docs/
│   │       │   │   ├── Design/
│   │       │   │   │   ├── 01_Vision_And_Art_Direction.md
│   │       │   │   │   ├── 02_Mars_World_Layout.md
│   │       │   │   │   ├── 03_Environment_Asset_List.md
│   │       │   │   │   ├── 04_Unity_URP_Technical_Standards.md
│   │       │   │   │   ├── 05_Gameplay_Environment_Integration.md
│   │       │   │   │   ├── 06_References_And_Attribution.md
│   │       │   │   │   └── 07_Implementation_Roadmap.md
│   │       │   │   ├── Stage04/
│   │       │   │   │   ├── Assets_And_Attribution.md
│   │       │   │   │   ├── Stage04_Completion_Report.md
│   │       │   │   │   └── Validation_Evidence_Paths.txt
│   │       │   │   ├── Integration_Architecture.md
│   │       │   │   └── PROGRESS_REPORT.md
│   │       │   ├── Packages/
│   │       │   │   ├── manifest.json
│   │       │   │   └── packages-lock.json
│   │       │   └── ProjectSettings/
│   │       │       ├── AudioManager.asset
│   │       │       ├── ClusterInputManager.asset
│   │       │       ├── DynamicsManager.asset
│   │       │       ├── EditorBuildSettings.asset
│   │       │       ├── EditorSettings.asset
│   │       │       ├── GraphicsSettings.asset
│   │       │       ├── InputManager.asset
│   │       │       ├── MemorySettings.asset
│   │       │       ├── MultiplayerManager.asset
│   │       │       ├── NavMeshAreas.asset
│   │       │       ├── PackageManagerSettings.asset
│   │       │       ├── Physics2DSettings.asset
│   │       │       ├── PhysicsCoreProjectSettings2D.asset
│   │       │       ├── PresetManager.asset
│   │       │       ├── ProjectAuditorSettings.asset
│   │       │       ├── ProjectSettings.asset
│   │       │       ├── ProjectVersion.txt
│   │       │       ├── QualitySettings.asset
│   │       │       ├── SceneTemplateSettings.json
│   │       │       ├── ShaderGraphSettings.asset
│   │       │       ├── TagManager.asset
│   │       │       ├── TimeManager.asset
│   │       │       ├── UnityConnectSettings.asset
│   │       │       ├── URPProjectSettings.asset
│   │       │       ├── VersionControlSettings.asset
│   │       │       ├── VFXManager.asset
│   │       │       └── XRSettings.asset
│   │       ├── Lab2/
│   │       │   ├── Assets/
│   │       │   │   ├── AstroGateway/
│   │       │   │   │   ├── CinematicHUD/
│   │       │   │   │   │   ├── Documentation/
│   │       │   │   │   │   │   ├── README.md
│   │       │   │   │   │   │   └── README.md.meta
│   │       │   │   │   │   ├── Prefabs/
│   │       │   │   │   │   │   ├── AstroGatewayCinematicHUD.prefab
│   │       │   │   │   │   │   └── AstroGatewayCinematicHUD.prefab.meta
│   │       │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   ├── Mars_CinematicHUD_v1.unity
│   │       │   │   │   │   │   └── Mars_CinematicHUD_v1.unity.meta
│   │       │   │   │   │   ├── Scripts/
│   │       │   │   │   │   │   ├── AstroGatewayHUDController.cs
│   │       │   │   │   │   │   ├── AstroGatewayHUDController.cs.meta
│   │       │   │   │   │   │   ├── CinematicEnvironmentAdapter.cs
│   │       │   │   │   │   │   ├── CinematicEnvironmentAdapter.cs.meta
│   │       │   │   │   │   │   ├── CinematicGameAdapter.cs
│   │       │   │   │   │   │   ├── CinematicGameAdapter.cs.meta
│   │       │   │   │   │   │   ├── HUDInputRouter.cs
│   │       │   │   │   │   │   ├── HUDInputRouter.cs.meta
│   │       │   │   │   │   │   ├── HUDLayoutProbe.cs
│   │       │   │   │   │   │   ├── HUDLayoutProbe.cs.meta
│   │       │   │   │   │   │   ├── HUDPerformanceProbe.cs
│   │       │   │   │   │   │   ├── HUDPerformanceProbe.cs.meta
│   │       │   │   │   │   │   ├── HUDPointerControl.cs
│   │       │   │   │   │   │   ├── HUDPointerControl.cs.meta
│   │       │   │   │   │   │   ├── HUDTelemetryAdapter.cs
│   │       │   │   │   │   │   ├── HUDTelemetryAdapter.cs.meta
│   │       │   │   │   │   │   ├── HUDTheme.cs
│   │       │   │   │   │   │   └── HUDTheme.cs.meta
│   │       │   │   │   │   ├── Settings/
│   │       │   │   │   │   │   ├── MissionRegistry.asset
│   │       │   │   │   │   │   └── MissionRegistry.asset.meta
│   │       │   │   │   │   ├── Tests/
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── CinematicHUDBuilder.cs
│   │       │   │   │   │   │   │   ├── CinematicHUDBuilder.cs.meta
│   │       │   │   │   │   │   │   ├── HUDResponsiveQA.cs
│   │       │   │   │   │   │   │   └── HUDResponsiveQA.cs.meta
│   │       │   │   │   │   │   └── Editor.meta
│   │       │   │   │   │   ├── Themes/
│   │       │   │   │   │   │   ├── AstroGatewayHUDTheme.asset
│   │       │   │   │   │   │   └── AstroGatewayHUDTheme.asset.meta
│   │       │   │   │   │   ├── UI/
│   │       │   │   │   │   │   ├── CircularControl.png
│   │       │   │   │   │   │   ├── CircularControl.png.meta
│   │       │   │   │   │   │   ├── HeadingArrow.png
│   │       │   │   │   │   │   ├── HeadingArrow.png.meta
│   │       │   │   │   │   │   ├── OriginalContourMap.png
│   │       │   │   │   │   │   ├── OriginalContourMap.png.meta
│   │       │   │   │   │   │   ├── RoundedPanel.png
│   │       │   │   │   │   │   └── RoundedPanel.png.meta
│   │       │   │   │   │   ├── Documentation.meta
│   │       │   │   │   │   ├── link.xml
│   │       │   │   │   │   ├── link.xml.meta
│   │       │   │   │   │   ├── Prefabs.meta
│   │       │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   ├── Scripts.meta
│   │       │   │   │   │   ├── Settings.meta
│   │       │   │   │   │   ├── Tests.meta
│   │       │   │   │   │   ├── Themes.meta
│   │       │   │   │   │   └── UI.meta
│   │       │   │   │   ├── Environment/
│   │       │   │   │   │   ├── Architecture/
│   │       │   │   │   │   │   ├── AgricultureG01_ChamferShell.asset
│   │       │   │   │   │   │   ├── AgricultureG01_ChamferShell.asset.meta
│   │       │   │   │   │   │   ├── AgricultureG01_Ring.asset
│   │       │   │   │   │   │   ├── AgricultureG01_Ring.asset.meta
│   │       │   │   │   │   │   ├── AirlockShell.asset
│   │       │   │   │   │   │   ├── AirlockShell.asset.meta
│   │       │   │   │   │   │   ├── C_ScienceStaging_Accent_Cyan_Combined.asset
│   │       │   │   │   │   │   ├── C_ScienceStaging_Accent_Cyan_Combined.asset.meta
│   │       │   │   │   │   │   ├── C_ScienceStaging_Accent_SafetyOrange_Combined.asset
│   │       │   │   │   │   │   ├── C_ScienceStaging_Accent_SafetyOrange_Combined.asset.meta
│   │       │   │   │   │   │   ├── C_ScienceStaging_Glass_Utility_Combined.asset
│   │       │   │   │   │   │   ├── C_ScienceStaging_Glass_Utility_Combined.asset.meta
│   │       │   │   │   │   │   ├── C_ScienceStaging_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   ├── C_ScienceStaging_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   ├── C_ScienceStaging_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   ├── C_ScienceStaging_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   ├── C_ScienceStaging_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   ├── C_ScienceStaging_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   ├── G_Greenhouse_Accent_Cyan_Combined.asset
│   │       │   │   │   │   │   ├── G_Greenhouse_Accent_Cyan_Combined.asset.meta
│   │       │   │   │   │   │   ├── G_Greenhouse_Glass_Utility_Combined.asset
│   │       │   │   │   │   │   ├── G_Greenhouse_Glass_Utility_Combined.asset.meta
│   │       │   │   │   │   │   ├── G_Greenhouse_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   ├── G_Greenhouse_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   ├── G_Greenhouse_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   ├── G_Greenhouse_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   ├── G_Greenhouse_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   ├── G_Greenhouse_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   ├── H_CommandHabitat_Accent_Cyan_Combined.asset
│   │       │   │   │   │   │   ├── H_CommandHabitat_Accent_Cyan_Combined.asset.meta
│   │       │   │   │   │   │   ├── H_CommandHabitat_Glass_Utility_Combined.asset
│   │       │   │   │   │   │   ├── H_CommandHabitat_Glass_Utility_Combined.asset.meta
│   │       │   │   │   │   │   ├── H_CommandHabitat_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   ├── H_CommandHabitat_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   ├── H_CommandHabitat_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   ├── H_CommandHabitat_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   ├── H_CommandHabitat_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   ├── H_CommandHabitat_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   ├── HabitatH01_ChamferShell.asset
│   │       │   │   │   │   │   ├── HabitatH01_ChamferShell.asset.meta
│   │       │   │   │   │   │   ├── HabitatH01_Ring.asset
│   │       │   │   │   │   │   ├── HabitatH01_Ring.asset.meta
│   │       │   │   │   │   │   ├── P_PowerStation_Accent_Cyan_Combined.asset
│   │       │   │   │   │   │   ├── P_PowerStation_Accent_Cyan_Combined.asset.meta
│   │       │   │   │   │   │   ├── P_PowerStation_Accent_SafetyOrange_Combined.asset
│   │       │   │   │   │   │   ├── P_PowerStation_Accent_SafetyOrange_Combined.asset.meta
│   │       │   │   │   │   │   ├── P_PowerStation_Glass_Utility_Combined.asset
│   │       │   │   │   │   │   ├── P_PowerStation_Glass_Utility_Combined.asset.meta
│   │       │   │   │   │   │   ├── P_PowerStation_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   ├── P_PowerStation_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   ├── P_PowerStation_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   ├── P_PowerStation_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   ├── P_PowerStation_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   ├── P_PowerStation_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   ├── R_RoverBay_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   ├── R_RoverBay_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   ├── R_RoverBay_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   ├── R_RoverBay_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   ├── R_RoverBay_Mars_Regolith_Dark_Combined.asset
│   │       │   │   │   │   │   ├── R_RoverBay_Mars_Regolith_Dark_Combined.asset.meta
│   │       │   │   │   │   │   ├── R_RoverBay_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── R_RoverBay_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── R_RoverBay_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   ├── R_RoverBay_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   ├── S_SolarArray_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   ├── S_SolarArray_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   ├── S_SolarArray_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   ├── S_SolarArray_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   ├── S_SolarArray_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   ├── S_SolarArray_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   ├── S_SolarArray_SolarPanel_Combined.asset
│   │       │   │   │   │   │   └── S_SolarArray_SolarPanel_Combined.asset.meta
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── MarsEnvironmentBuilder.cs
│   │       │   │   │   │   │   └── MarsEnvironmentBuilder.cs.meta
│   │       │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   ├── Accent_Cyan.mat
│   │       │   │   │   │   │   ├── Accent_Cyan.mat.meta
│   │       │   │   │   │   │   ├── Accent_SafetyOrange.mat
│   │       │   │   │   │   │   ├── Accent_SafetyOrange.mat.meta
│   │       │   │   │   │   │   ├── Basalt_Surface_512.png
│   │       │   │   │   │   │   ├── Basalt_Surface_512.png.meta
│   │       │   │   │   │   │   ├── Composite_DustPaint_512.png
│   │       │   │   │   │   │   ├── Composite_DustPaint_512.png.meta
│   │       │   │   │   │   │   ├── Delta_Bedding_512.png
│   │       │   │   │   │   │   ├── Delta_Bedding_512.png.meta
│   │       │   │   │   │   │   ├── Delta_FracturedStrata_512.png
│   │       │   │   │   │   │   ├── Delta_FracturedStrata_512.png.meta
│   │       │   │   │   │   │   ├── DustSky.shader
│   │       │   │   │   │   │   ├── DustSky.shader.meta
│   │       │   │   │   │   │   ├── Glass_Utility.mat
│   │       │   │   │   │   │   ├── Glass_Utility.mat.meta
│   │       │   │   │   │   │   ├── Habitat_CompositeDark.mat
│   │       │   │   │   │   │   ├── Habitat_CompositeDark.mat.meta
│   │       │   │   │   │   │   ├── Habitat_OffWhite.mat
│   │       │   │   │   │   │   ├── Habitat_OffWhite.mat.meta
│   │       │   │   │   │   │   ├── Mars_DustSky.mat
│   │       │   │   │   │   │   ├── Mars_DustSky.mat.meta
│   │       │   │   │   │   │   ├── Mars_Regolith.mat
│   │       │   │   │   │   │   ├── Mars_Regolith.mat.meta
│   │       │   │   │   │   │   ├── Mars_Regolith_Dark.mat
│   │       │   │   │   │   │   ├── Mars_Regolith_Dark.mat.meta
│   │       │   │   │   │   │   ├── Mars_Rock_Basaltic.mat
│   │       │   │   │   │   │   ├── Mars_Rock_Basaltic.mat.meta
│   │       │   │   │   │   │   ├── Mars_Rock_Layered.mat
│   │       │   │   │   │   │   ├── Mars_Rock_Layered.mat.meta
│   │       │   │   │   │   │   ├── Metal_Industrial.mat
│   │       │   │   │   │   │   ├── Metal_Industrial.mat.meta
│   │       │   │   │   │   │   ├── Regolith_Grain_512.png
│   │       │   │   │   │   │   ├── Regolith_Grain_512.png.meta
│   │       │   │   │   │   │   ├── Regolith_Macro_Pathwear_1024.png
│   │       │   │   │   │   │   ├── Regolith_Macro_Pathwear_1024.png.meta
│   │       │   │   │   │   │   ├── Regolith_MicroNormal_256.png
│   │       │   │   │   │   │   ├── Regolith_MicroNormal_256.png.meta
│   │       │   │   │   │   │   ├── Solar_Cells_Dust_512.png
│   │       │   │   │   │   │   ├── Solar_Cells_Dust_512.png.meta
│   │       │   │   │   │   │   ├── SolarPanel.mat
│   │       │   │   │   │   │   └── SolarPanel.mat.meta
│   │       │   │   │   │   ├── Prefabs/
│   │       │   │   │   │   │   ├── AngularBasalt.prefab
│   │       │   │   │   │   │   ├── AngularBasalt.prefab.meta
│   │       │   │   │   │   │   ├── C_ScienceStaging.prefab
│   │       │   │   │   │   │   ├── C_ScienceStaging.prefab.meta
│   │       │   │   │   │   │   ├── ErodedLandmark.prefab
│   │       │   │   │   │   │   ├── ErodedLandmark.prefab.meta
│   │       │   │   │   │   │   ├── G_Greenhouse.prefab
│   │       │   │   │   │   │   ├── G_Greenhouse.prefab.meta
│   │       │   │   │   │   │   ├── H_CommandHabitat.prefab
│   │       │   │   │   │   │   ├── H_CommandHabitat.prefab.meta
│   │       │   │   │   │   │   ├── LayeredDelta.prefab
│   │       │   │   │   │   │   ├── LayeredDelta.prefab.meta
│   │       │   │   │   │   │   ├── P_PowerStation.prefab
│   │       │   │   │   │   │   ├── P_PowerStation.prefab.meta
│   │       │   │   │   │   │   ├── R_RoverBay.prefab
│   │       │   │   │   │   │   ├── R_RoverBay.prefab.meta
│   │       │   │   │   │   │   ├── RegolithStone.prefab
│   │       │   │   │   │   │   ├── RegolithStone.prefab.meta
│   │       │   │   │   │   │   ├── S_SolarArray.prefab
│   │       │   │   │   │   │   └── S_SolarArray.prefab.meta
│   │       │   │   │   │   ├── Props/
│   │       │   │   │   │   │   ├── ApproachStoneClusters_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── ApproachStoneClusters_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── HabitatExternalKit_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   ├── HabitatExternalKit_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   ├── HabitatExternalKit_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   ├── HabitatExternalKit_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   ├── HabitatExternalKit_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   ├── HabitatExternalKit_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   ├── PowerExternalKit_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   ├── PowerExternalKit_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   ├── PowerExternalKit_Mars_Regolith_Dark_Combined.asset
│   │       │   │   │   │   │   ├── PowerExternalKit_Mars_Regolith_Dark_Combined.asset.meta
│   │       │   │   │   │   │   ├── UtilityRoutePosts_Accent_Cyan_Combined.asset
│   │       │   │   │   │   │   ├── UtilityRoutePosts_Accent_Cyan_Combined.asset.meta
│   │       │   │   │   │   │   ├── UtilityRoutePosts_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   └── UtilityRoutePosts_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   ├── Rocks/
│   │       │   │   │   │   │   ├── AngularBasalt_LOD0.asset
│   │       │   │   │   │   │   ├── AngularBasalt_LOD0.asset.meta
│   │       │   │   │   │   │   ├── AngularBasalt_LOD1.asset
│   │       │   │   │   │   │   ├── AngularBasalt_LOD1.asset.meta
│   │       │   │   │   │   │   ├── AngularBasalt_LOD2.asset
│   │       │   │   │   │   │   ├── AngularBasalt_LOD2.asset.meta
│   │       │   │   │   │   │   ├── DeltaFractured_LOD0.asset
│   │       │   │   │   │   │   ├── DeltaFractured_LOD0.asset.meta
│   │       │   │   │   │   │   ├── DeltaFractured_LOD1.asset
│   │       │   │   │   │   │   ├── DeltaFractured_LOD1.asset.meta
│   │       │   │   │   │   │   ├── DeltaFractured_LOD2.asset
│   │       │   │   │   │   │   ├── DeltaFractured_LOD2.asset.meta
│   │       │   │   │   │   │   ├── ErodedLandmark_LOD0.asset
│   │       │   │   │   │   │   ├── ErodedLandmark_LOD0.asset.meta
│   │       │   │   │   │   │   ├── ErodedLandmark_LOD1.asset
│   │       │   │   │   │   │   ├── ErodedLandmark_LOD1.asset.meta
│   │       │   │   │   │   │   ├── ErodedLandmark_LOD2.asset
│   │       │   │   │   │   │   ├── ErodedLandmark_LOD2.asset.meta
│   │       │   │   │   │   │   ├── LayeredDelta_LOD0.asset
│   │       │   │   │   │   │   ├── LayeredDelta_LOD0.asset.meta
│   │       │   │   │   │   │   ├── LayeredDelta_LOD1.asset
│   │       │   │   │   │   │   ├── LayeredDelta_LOD1.asset.meta
│   │       │   │   │   │   │   ├── LayeredDelta_LOD2.asset
│   │       │   │   │   │   │   ├── LayeredDelta_LOD2.asset.meta
│   │       │   │   │   │   │   ├── StonePatch_0_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── StonePatch_0_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── StonePatch_1_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── StonePatch_1_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── StonePatch_2_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── StonePatch_2_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── StonePatch_3_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── StonePatch_3_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── StonePatch_4_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── StonePatch_4_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── StonePatch_5_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── StonePatch_5_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── StonePatch_6_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── StonePatch_6_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── StonePatch_7_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   ├── StonePatch_7_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── StonePatch_8_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   └── StonePatch_8_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   ├── Terrain/
│   │       │   │   │   │   │   ├── Regolith_250m_PreservedCorridor.asset
│   │       │   │   │   │   │   └── Regolith_250m_PreservedCorridor.asset.meta
│   │       │   │   │   │   ├── Architecture.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   ├── Prefabs.meta
│   │       │   │   │   │   ├── Props.meta
│   │       │   │   │   │   ├── Rocks.meta
│   │       │   │   │   │   └── Terrain.meta
│   │       │   │   │   ├── Greybox/
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── MarsGreyboxBuilder.cs
│   │       │   │   │   │   │   └── MarsGreyboxBuilder.cs.meta
│   │       │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   ├── DarkRock.mat
│   │       │   │   │   │   │   ├── DarkRock.mat.meta
│   │       │   │   │   │   │   ├── HabitatPale.mat
│   │       │   │   │   │   │   ├── HabitatPale.mat.meta
│   │       │   │   │   │   │   ├── IndustrialGraphite.mat
│   │       │   │   │   │   │   ├── IndustrialGraphite.mat.meta
│   │       │   │   │   │   │   ├── LayeredStone.mat
│   │       │   │   │   │   │   ├── LayeredStone.mat.meta
│   │       │   │   │   │   │   ├── SandTan.mat
│   │       │   │   │   │   │   ├── SandTan.mat.meta
│   │       │   │   │   │   │   ├── TerrainOchre.mat
│   │       │   │   │   │   │   ├── TerrainOchre.mat.meta
│   │       │   │   │   │   │   ├── WayfindingAmber.mat
│   │       │   │   │   │   │   ├── WayfindingAmber.mat.meta
│   │       │   │   │   │   │   ├── WayfindingCyan.mat
│   │       │   │   │   │   │   └── WayfindingCyan.mat.meta
│   │       │   │   │   │   ├── Meshes/
│   │       │   │   │   │   │   ├── Ground.asset
│   │       │   │   │   │   │   └── Ground.asset.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── MarsOutpost_Greybox.unity
│   │       │   │   │   │   ├── MarsOutpost_Greybox.unity.meta
│   │       │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   ├── Meshes.meta
│   │       │   │   │   │   ├── PreGreybox_Untitled_Backup.unity
│   │       │   │   │   │   └── PreGreybox_Untitled_Backup.unity.meta
│   │       │   │   │   ├── Integration/
│   │       │   │   │   │   ├── Contracts/
│   │       │   │   │   │   │   ├── IntegrationContracts.cs
│   │       │   │   │   │   │   └── IntegrationContracts.cs.meta
│   │       │   │   │   │   ├── Http/
│   │       │   │   │   │   │   ├── HttpMissionSessionService.cs
│   │       │   │   │   │   │   └── HttpMissionSessionService.cs.meta
│   │       │   │   │   │   ├── Local/
│   │       │   │   │   │   │   ├── GameplayEventBridge.cs
│   │       │   │   │   │   │   ├── GameplayEventBridge.cs.meta
│   │       │   │   │   │   │   ├── LifeSupportModelAdapter.cs
│   │       │   │   │   │   │   ├── LifeSupportModelAdapter.cs.meta
│   │       │   │   │   │   │   ├── LocalServices.cs
│   │       │   │   │   │   │   └── LocalServices.cs.meta
│   │       │   │   │   │   ├── Contracts.meta
│   │       │   │   │   │   ├── Http.meta
│   │       │   │   │   │   └── Local.meta
│   │       │   │   │   ├── MarsEnvironment/
│   │       │   │   │   │   ├── Documentation/
│   │       │   │   │   │   │   ├── MissionRegistry.asset
│   │       │   │   │   │   │   ├── MissionRegistry.asset.meta
│   │       │   │   │   │   │   ├── README.md
│   │       │   │   │   │   │   └── README.md.meta
│   │       │   │   │   │   ├── Lighting/
│   │       │   │   │   │   │   ├── Dusty_Conditions.asset
│   │       │   │   │   │   │   ├── Dusty_Conditions.asset.meta
│   │       │   │   │   │   │   ├── MarsGradient.shader
│   │       │   │   │   │   │   ├── MarsGradient.shader.meta
│   │       │   │   │   │   │   ├── MarsSky.mat
│   │       │   │   │   │   │   ├── MarsSky.mat.meta
│   │       │   │   │   │   │   ├── MarsVolume.asset
│   │       │   │   │   │   │   ├── MarsVolume.asset.meta
│   │       │   │   │   │   │   ├── Midday.asset
│   │       │   │   │   │   │   ├── Midday.asset.meta
│   │       │   │   │   │   │   ├── Morning.asset
│   │       │   │   │   │   │   ├── Morning.asset.meta
│   │       │   │   │   │   │   ├── Sunset.asset
│   │       │   │   │   │   │   └── Sunset.asset.meta
│   │       │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   ├── DistantDust.asset
│   │       │   │   │   │   │   ├── DistantDust.asset.meta
│   │       │   │   │   │   │   ├── Dusty solar cells.mat
│   │       │   │   │   │   │   ├── Dusty solar cells.mat.meta
│   │       │   │   │   │   │   ├── EnvironmentDust.mat
│   │       │   │   │   │   │   ├── EnvironmentDust.mat.meta
│   │       │   │   │   │   │   ├── Fine Mars dust.mat
│   │       │   │   │   │   │   ├── Fine Mars dust.mat.meta
│   │       │   │   │   │   │   ├── Habitat ceramic composite.mat
│   │       │   │   │   │   │   ├── Habitat ceramic composite.mat.meta
│   │       │   │   │   │   │   ├── Observation panels — opaque approximation.mat
│   │       │   │   │   │   │   ├── Observation panels — opaque approximation.mat.meta
│   │       │   │   │   │   │   ├── Restrained identification ochre.mat
│   │       │   │   │   │   │   ├── Restrained identification ochre.mat.meta
│   │       │   │   │   │   │   ├── Rock07 dusty basalt.mat
│   │       │   │   │   │   │   ├── Rock07 dusty basalt.mat.meta
│   │       │   │   │   │   │   ├── Rock09 compact mineral.mat
│   │       │   │   │   │   │   ├── Rock09 compact mineral.mat.meta
│   │       │   │   │   │   │   ├── Rover_Cab.mat
│   │       │   │   │   │   │   ├── Rover_Cab.mat.meta
│   │       │   │   │   │   │   ├── Rover_Chassis.mat
│   │       │   │   │   │   │   ├── Rover_Chassis.mat.meta
│   │       │   │   │   │   │   ├── Rover_RoofMarker.mat
│   │       │   │   │   │   │   ├── Rover_RoofMarker.mat.meta
│   │       │   │   │   │   │   ├── Rover_Wheel_-1_0.mat
│   │       │   │   │   │   │   ├── Rover_Wheel_-1_0.mat.meta
│   │       │   │   │   │   │   ├── Rover_Wheel_-1_1.mat
│   │       │   │   │   │   │   ├── Rover_Wheel_-1_1.mat.meta
│   │       │   │   │   │   │   ├── Rover_Wheel_-1_2.mat
│   │       │   │   │   │   │   ├── Rover_Wheel_-1_2.mat.meta
│   │       │   │   │   │   │   ├── Rover_Wheel_1_0.mat
│   │       │   │   │   │   │   ├── Rover_Wheel_1_0.mat.meta
│   │       │   │   │   │   │   ├── Rover_Wheel_1_1.mat
│   │       │   │   │   │   │   ├── Rover_Wheel_1_1.mat.meta
│   │       │   │   │   │   │   ├── Rover_Wheel_1_2.mat
│   │       │   │   │   │   │   ├── Rover_Wheel_1_2.mat.meta
│   │       │   │   │   │   │   ├── Structural graphite.mat
│   │       │   │   │   │   │   └── Structural graphite.mat.meta
│   │       │   │   │   │   ├── Models/
│   │       │   │   │   │   │   ├── Crew_Dome_Ribs_LowPoly.asset
│   │       │   │   │   │   │   ├── Crew_Dome_Ribs_LowPoly.asset.meta
│   │       │   │   │   │   │   ├── HabitatHemisphere.asset
│   │       │   │   │   │   │   ├── HabitatHemisphere.asset.meta
│   │       │   │   │   │   │   ├── Laboratory_Dome_Ribs_LowPoly.asset
│   │       │   │   │   │   │   ├── Laboratory_Dome_Ribs_LowPoly.asset.meta
│   │       │   │   │   │   │   ├── Life_Support_Dome_Ribs_LowPoly.asset
│   │       │   │   │   │   │   ├── Life_Support_Dome_Ribs_LowPoly.asset.meta
│   │       │   │   │   │   │   ├── rock_07_LOD0.asset
│   │       │   │   │   │   │   ├── rock_07_LOD0.asset.meta
│   │       │   │   │   │   │   ├── rock_07_LOD1.asset
│   │       │   │   │   │   │   ├── rock_07_LOD1.asset.meta
│   │       │   │   │   │   │   ├── rock_07_LOD2.asset
│   │       │   │   │   │   │   ├── rock_07_LOD2.asset.meta
│   │       │   │   │   │   │   ├── rock_09_LOD0.asset
│   │       │   │   │   │   │   ├── rock_09_LOD0.asset.meta
│   │       │   │   │   │   │   ├── rock_09_LOD1.asset
│   │       │   │   │   │   │   ├── rock_09_LOD1.asset.meta
│   │       │   │   │   │   │   ├── rock_09_LOD2.asset
│   │       │   │   │   │   │   ├── rock_09_LOD2.asset.meta
│   │       │   │   │   │   │   ├── Terrain250m.asset
│   │       │   │   │   │   │   └── Terrain250m.asset.meta
│   │       │   │   │   │   ├── Prefabs/
│   │       │   │   │   │   │   ├── ARESYN_ClosedHabitatComplex_Optimized.prefab
│   │       │   │   │   │   │   ├── ARESYN_ClosedHabitatComplex_Optimized.prefab.meta
│   │       │   │   │   │   │   ├── SolarFarm_Optimized.prefab
│   │       │   │   │   │   │   └── SolarFarm_Optimized.prefab.meta
│   │       │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   ├── Mars_RealisticResearchBase_v1.unity
│   │       │   │   │   │   │   └── Mars_RealisticResearchBase_v1.unity.meta
│   │       │   │   │   │   ├── Scripts/
│   │       │   │   │   │   │   ├── EnvironmentPresentation.cs
│   │       │   │   │   │   │   ├── EnvironmentPresentation.cs.meta
│   │       │   │   │   │   │   ├── EnvironmentPreset.cs
│   │       │   │   │   │   │   ├── EnvironmentPreset.cs.meta
│   │       │   │   │   │   │   ├── ResearchTarget.cs
│   │       │   │   │   │   │   └── ResearchTarget.cs.meta
│   │       │   │   │   │   ├── Tests/
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── MarsResearchBaseBuilder.cs
│   │       │   │   │   │   │   │   └── MarsResearchBaseBuilder.cs.meta
│   │       │   │   │   │   │   └── Editor.meta
│   │       │   │   │   │   ├── Textures/
│   │       │   │   │   │   │   ├── rock_07_AO.png
│   │       │   │   │   │   │   ├── rock_07_AO.png.meta
│   │       │   │   │   │   │   ├── rock_07_Diffuse.png
│   │       │   │   │   │   │   ├── rock_07_Diffuse.png.meta
│   │       │   │   │   │   │   ├── rock_07_nor_gl.png
│   │       │   │   │   │   │   ├── rock_07_nor_gl.png.meta
│   │       │   │   │   │   │   ├── rock_07_Rough.png
│   │       │   │   │   │   │   ├── rock_07_Rough.png.meta
│   │       │   │   │   │   │   ├── rock_09_AO.png
│   │       │   │   │   │   │   ├── rock_09_AO.png.meta
│   │       │   │   │   │   │   ├── rock_09_Diffuse.png
│   │       │   │   │   │   │   ├── rock_09_Diffuse.png.meta
│   │       │   │   │   │   │   ├── rock_09_nor_gl.png
│   │       │   │   │   │   │   ├── rock_09_nor_gl.png.meta
│   │       │   │   │   │   │   ├── rock_09_Rough.png
│   │       │   │   │   │   │   ├── rock_09_Rough.png.meta
│   │       │   │   │   │   │   ├── sandy_gravel_AO.png
│   │       │   │   │   │   │   ├── sandy_gravel_AO.png.meta
│   │       │   │   │   │   │   ├── sandy_gravel_Diffuse.png
│   │       │   │   │   │   │   ├── sandy_gravel_Diffuse.png.meta
│   │       │   │   │   │   │   ├── sandy_gravel_nor_gl.png
│   │       │   │   │   │   │   ├── sandy_gravel_nor_gl.png.meta
│   │       │   │   │   │   │   ├── sandy_gravel_Rough.png
│   │       │   │   │   │   │   └── sandy_gravel_Rough.png.meta
│   │       │   │   │   │   ├── Documentation.meta
│   │       │   │   │   │   ├── Lighting.meta
│   │       │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   ├── Models.meta
│   │       │   │   │   │   ├── Prefabs.meta
│   │       │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   ├── Scripts.meta
│   │       │   │   │   │   ├── Tests.meta
│   │       │   │   │   │   ├── Textures.meta
│   │       │   │   │   │   └── VFX.meta
│   │       │   │   │   ├── MarsVisualPolish/
│   │       │   │   │   │   ├── Documentation/
│   │       │   │   │   │   │   ├── PolishRegistry.asset
│   │       │   │   │   │   │   └── PolishRegistry.asset.meta
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── VisualPolishBuilder.cs
│   │       │   │   │   │   │   └── VisualPolishBuilder.cs.meta
│   │       │   │   │   │   ├── Lighting/
│   │       │   │   │   │   │   ├── PolishSky.mat
│   │       │   │   │   │   │   ├── PolishSky.mat.meta
│   │       │   │   │   │   │   ├── PolishVolume.asset
│   │       │   │   │   │   │   ├── PolishVolume.asset.meta
│   │       │   │   │   │   │   ├── Preset_0.asset
│   │       │   │   │   │   │   ├── Preset_0.asset.meta
│   │       │   │   │   │   │   ├── Preset_1.asset
│   │       │   │   │   │   │   ├── Preset_1.asset.meta
│   │       │   │   │   │   │   ├── Preset_2.asset
│   │       │   │   │   │   │   ├── Preset_2.asset.meta
│   │       │   │   │   │   │   ├── Preset_3.asset
│   │       │   │   │   │   │   └── Preset_3.asset.meta
│   │       │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   ├── Brushed frame.mat
│   │       │   │   │   │   │   ├── Brushed frame.mat.meta
│   │       │   │   │   │   │   ├── Derivative_0.mat
│   │       │   │   │   │   │   ├── Derivative_0.mat.meta
│   │       │   │   │   │   │   ├── Derivative_1.mat
│   │       │   │   │   │   │   ├── Derivative_1.mat.meta
│   │       │   │   │   │   │   ├── Derivative_10.mat
│   │       │   │   │   │   │   ├── Derivative_10.mat.meta
│   │       │   │   │   │   │   ├── Derivative_11.mat
│   │       │   │   │   │   │   ├── Derivative_11.mat.meta
│   │       │   │   │   │   │   ├── Derivative_12.mat
│   │       │   │   │   │   │   ├── Derivative_12.mat.meta
│   │       │   │   │   │   │   ├── Derivative_13.mat
│   │       │   │   │   │   │   ├── Derivative_13.mat.meta
│   │       │   │   │   │   │   ├── Derivative_14.mat
│   │       │   │   │   │   │   ├── Derivative_14.mat.meta
│   │       │   │   │   │   │   ├── Derivative_15.mat
│   │       │   │   │   │   │   ├── Derivative_15.mat.meta
│   │       │   │   │   │   │   ├── Derivative_16.mat
│   │       │   │   │   │   │   ├── Derivative_16.mat.meta
│   │       │   │   │   │   │   ├── Derivative_17.mat
│   │       │   │   │   │   │   ├── Derivative_17.mat.meta
│   │       │   │   │   │   │   ├── Derivative_18.mat
│   │       │   │   │   │   │   ├── Derivative_18.mat.meta
│   │       │   │   │   │   │   ├── Derivative_19.mat
│   │       │   │   │   │   │   ├── Derivative_19.mat.meta
│   │       │   │   │   │   │   ├── Derivative_2.mat
│   │       │   │   │   │   │   ├── Derivative_2.mat.meta
│   │       │   │   │   │   │   ├── Derivative_20.mat
│   │       │   │   │   │   │   ├── Derivative_20.mat.meta
│   │       │   │   │   │   │   ├── Derivative_21.mat
│   │       │   │   │   │   │   ├── Derivative_21.mat.meta
│   │       │   │   │   │   │   ├── Derivative_3.mat
│   │       │   │   │   │   │   ├── Derivative_3.mat.meta
│   │       │   │   │   │   │   ├── Derivative_4.mat
│   │       │   │   │   │   │   ├── Derivative_4.mat.meta
│   │       │   │   │   │   │   ├── Derivative_5.mat
│   │       │   │   │   │   │   ├── Derivative_5.mat.meta
│   │       │   │   │   │   │   ├── Derivative_6.mat
│   │       │   │   │   │   │   ├── Derivative_6.mat.meta
│   │       │   │   │   │   │   ├── Derivative_7.mat
│   │       │   │   │   │   │   ├── Derivative_7.mat.meta
│   │       │   │   │   │   │   ├── Derivative_8.mat
│   │       │   │   │   │   │   ├── Derivative_8.mat.meta
│   │       │   │   │   │   │   ├── Derivative_9.mat
│   │       │   │   │   │   │   ├── Derivative_9.mat.meta
│   │       │   │   │   │   │   ├── Photovoltaic cell laminate.mat
│   │       │   │   │   │   │   ├── Photovoltaic cell laminate.mat.meta
│   │       │   │   │   │   │   ├── PhotovoltaicGrid.png
│   │       │   │   │   │   │   ├── PhotovoltaicGrid.png.meta
│   │       │   │   │   │   │   ├── Recessed seals.mat
│   │       │   │   │   │   │   ├── Recessed seals.mat.meta
│   │       │   │   │   │   │   ├── Safety ochre.mat
│   │       │   │   │   │   │   ├── Safety ochre.mat.meta
│   │       │   │   │   │   │   ├── Warm ceramic.mat
│   │       │   │   │   │   │   └── Warm ceramic.mat.meta
│   │       │   │   │   │   ├── Models/
│   │       │   │   │   │   │   ├── Crew Dome_Frame.asset
│   │       │   │   │   │   │   ├── Crew Dome_Frame.asset.meta
│   │       │   │   │   │   │   ├── Laboratory Dome_Frame.asset
│   │       │   │   │   │   │   ├── Laboratory Dome_Frame.asset.meta
│   │       │   │   │   │   │   ├── Life Support Dome_Frame.asset
│   │       │   │   │   │   │   └── Life Support Dome_Frame.asset.meta
│   │       │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   ├── Mars_VisualPolish_v2.unity
│   │       │   │   │   │   │   └── Mars_VisualPolish_v2.unity.meta
│   │       │   │   │   │   ├── Documentation.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── Lighting.meta
│   │       │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   ├── Models.meta
│   │       │   │   │   │   ├── PolishBenchmark.cs
│   │       │   │   │   │   ├── PolishBenchmark.cs.meta
│   │       │   │   │   │   ├── PolishHudTheme.cs
│   │       │   │   │   │   ├── PolishHudTheme.cs.meta
│   │       │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   ├── VisualPolishPresentation.cs
│   │       │   │   │   │   └── VisualPolishPresentation.cs.meta
│   │       │   │   │   ├── Mission01Milestone/
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── Mission01Builder.cs
│   │       │   │   │   │   │   └── Mission01Builder.cs.meta
│   │       │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   ├── Mission01_Continents.mat
│   │       │   │   │   │   │   ├── Mission01_Continents.mat.meta
│   │       │   │   │   │   │   ├── Mission01_Graphite.mat
│   │       │   │   │   │   │   ├── Mission01_Graphite.mat.meta
│   │       │   │   │   │   │   ├── Mission01_Ocean.mat
│   │       │   │   │   │   │   ├── Mission01_Ocean.mat.meta
│   │       │   │   │   │   │   ├── Mission01_Pale.mat
│   │       │   │   │   │   │   ├── Mission01_Pale.mat.meta
│   │       │   │   │   │   │   ├── Mission01_Safety.mat
│   │       │   │   │   │   │   └── Mission01_Safety.mat.meta
│   │       │   │   │   │   ├── Prefabs/
│   │       │   │   │   │   │   ├── Boarding_Console.prefab
│   │       │   │   │   │   │   ├── Boarding_Console.prefab.meta
│   │       │   │   │   │   │   ├── Trainee_Blockout.prefab
│   │       │   │   │   │   │   └── Trainee_Blockout.prefab.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   ├── Mission01_EarthLaunch_Milestone_v4.unity
│   │       │   │   │   │   ├── Mission01_EarthLaunch_Milestone_v4.unity.meta
│   │       │   │   │   │   ├── Mission01Flow.cs
│   │       │   │   │   │   ├── Mission01Flow.cs.meta
│   │       │   │   │   │   └── Prefabs.meta
│   │       │   │   │   ├── MissionIntegration/
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── MissionIntegrationBuilder.cs
│   │       │   │   │   │   │   └── MissionIntegrationBuilder.cs.meta
│   │       │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   ├── Mars_MissionIntegration_v1.unity
│   │       │   │   │   │   │   └── Mars_MissionIntegration_v1.unity.meta
│   │       │   │   │   │   ├── Scripts/
│   │       │   │   │   │   │   ├── MissionIntegrationController.cs
│   │       │   │   │   │   │   └── MissionIntegrationController.cs.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   └── Scripts.meta
│   │       │   │   │   ├── NativeControlSlice/
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── NativeSliceBuilder.cs
│   │       │   │   │   │   │   └── NativeSliceBuilder.cs.meta
│   │       │   │   │   │   ├── Prefabs/
│   │       │   │   │   │   │   ├── AdaptiveTouch.prefab
│   │       │   │   │   │   │   └── AdaptiveTouch.prefab.meta
│   │       │   │   │   │   ├── AdaptiveTouch.cs
│   │       │   │   │   │   ├── AdaptiveTouch.cs.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── Mars_NativeControl_VerticalSlice.unity
│   │       │   │   │   │   ├── Mars_NativeControl_VerticalSlice.unity.meta
│   │       │   │   │   │   ├── MissionRegistry.asset
│   │       │   │   │   │   ├── MissionRegistry.asset.meta
│   │       │   │   │   │   ├── NativeSliceGame.cs
│   │       │   │   │   │   ├── NativeSliceGame.cs.meta
│   │       │   │   │   │   ├── NativeSystems.cs
│   │       │   │   │   │   ├── NativeSystems.cs.meta
│   │       │   │   │   │   └── Prefabs.meta
│   │       │   │   │   ├── ProductionVisual/
│   │       │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   ├── Brushed aluminum.mat
│   │       │   │   │   │   │   ├── Brushed aluminum.mat.meta
│   │       │   │   │   │   │   ├── Dusty tire elastomer.mat
│   │       │   │   │   │   │   ├── Dusty tire elastomer.mat.meta
│   │       │   │   │   │   │   ├── Inherited_0.mat
│   │       │   │   │   │   │   ├── Inherited_0.mat.meta
│   │       │   │   │   │   │   ├── Inherited_1.mat
│   │       │   │   │   │   │   ├── Inherited_1.mat.meta
│   │       │   │   │   │   │   ├── Inherited_10.mat
│   │       │   │   │   │   │   ├── Inherited_10.mat.meta
│   │       │   │   │   │   │   ├── Inherited_11.mat
│   │       │   │   │   │   │   ├── Inherited_11.mat.meta
│   │       │   │   │   │   │   ├── Inherited_12.mat
│   │       │   │   │   │   │   ├── Inherited_12.mat.meta
│   │       │   │   │   │   │   ├── Inherited_13.mat
│   │       │   │   │   │   │   ├── Inherited_13.mat.meta
│   │       │   │   │   │   │   ├── Inherited_14.mat
│   │       │   │   │   │   │   ├── Inherited_14.mat.meta
│   │       │   │   │   │   │   ├── Inherited_15.mat
│   │       │   │   │   │   │   ├── Inherited_15.mat.meta
│   │       │   │   │   │   │   ├── Inherited_16.mat
│   │       │   │   │   │   │   ├── Inherited_16.mat.meta
│   │       │   │   │   │   │   ├── Inherited_17.mat
│   │       │   │   │   │   │   ├── Inherited_17.mat.meta
│   │       │   │   │   │   │   ├── Inherited_18.mat
│   │       │   │   │   │   │   ├── Inherited_18.mat.meta
│   │       │   │   │   │   │   ├── Inherited_19.mat
│   │       │   │   │   │   │   ├── Inherited_19.mat.meta
│   │       │   │   │   │   │   ├── Inherited_2.mat
│   │       │   │   │   │   │   ├── Inherited_2.mat.meta
│   │       │   │   │   │   │   ├── Inherited_20.mat
│   │       │   │   │   │   │   ├── Inherited_20.mat.meta
│   │       │   │   │   │   │   ├── Inherited_21.mat
│   │       │   │   │   │   │   ├── Inherited_21.mat.meta
│   │       │   │   │   │   │   ├── Inherited_22.mat
│   │       │   │   │   │   │   ├── Inherited_22.mat.meta
│   │       │   │   │   │   │   ├── Inherited_23.mat
│   │       │   │   │   │   │   ├── Inherited_23.mat.meta
│   │       │   │   │   │   │   ├── Inherited_24.mat
│   │       │   │   │   │   │   ├── Inherited_24.mat.meta
│   │       │   │   │   │   │   ├── Inherited_25.mat
│   │       │   │   │   │   │   ├── Inherited_25.mat.meta
│   │       │   │   │   │   │   ├── Inherited_3.mat
│   │       │   │   │   │   │   ├── Inherited_3.mat.meta
│   │       │   │   │   │   │   ├── Inherited_4.mat
│   │       │   │   │   │   │   ├── Inherited_4.mat.meta
│   │       │   │   │   │   │   ├── Inherited_5.mat
│   │       │   │   │   │   │   ├── Inherited_5.mat.meta
│   │       │   │   │   │   │   ├── Inherited_6.mat
│   │       │   │   │   │   │   ├── Inherited_6.mat.meta
│   │       │   │   │   │   │   ├── Inherited_7.mat
│   │       │   │   │   │   │   ├── Inherited_7.mat.meta
│   │       │   │   │   │   │   ├── Inherited_8.mat
│   │       │   │   │   │   │   ├── Inherited_8.mat.meta
│   │       │   │   │   │   │   ├── Inherited_9.mat
│   │       │   │   │   │   │   ├── Inherited_9.mat.meta
│   │       │   │   │   │   │   ├── Machined graphite alloy.mat
│   │       │   │   │   │   │   ├── Machined graphite alloy.mat.meta
│   │       │   │   │   │   │   ├── Opaque protective visor glazing.mat
│   │       │   │   │   │   │   ├── Opaque protective visor glazing.mat.meta
│   │       │   │   │   │   │   ├── Pressure shell ceramic composite.mat
│   │       │   │   │   │   │   ├── Pressure shell ceramic composite.mat.meta
│   │       │   │   │   │   │   ├── Restrained identification orange.mat
│   │       │   │   │   │   │   └── Restrained identification orange.mat.meta
│   │       │   │   │   │   ├── Meshes/
│   │       │   │   │   │   │   ├── ChamferedEnclosure.asset
│   │       │   │   │   │   │   ├── ChamferedEnclosure.asset.meta
│   │       │   │   │   │   │   ├── CommsTruss.asset
│   │       │   │   │   │   │   ├── CommsTruss.asset.meta
│   │       │   │   │   │   │   ├── Crew_Dome_sealed_airlock_Rails.asset
│   │       │   │   │   │   │   ├── Crew_Dome_sealed_airlock_Rails.asset.meta
│   │       │   │   │   │   │   ├── CrewCabEnclosure.asset
│   │       │   │   │   │   │   ├── CrewCabEnclosure.asset.meta
│   │       │   │   │   │   │   ├── GeodesicFrame.asset
│   │       │   │   │   │   │   ├── GeodesicFrame.asset.meta
│   │       │   │   │   │   │   ├── GeodesicPanels.asset
│   │       │   │   │   │   │   ├── GeodesicPanels.asset.meta
│   │       │   │   │   │   │   ├── Laboratory_Dome_sealed_airlock_Rails.asset
│   │       │   │   │   │   │   ├── Laboratory_Dome_sealed_airlock_Rails.asset.meta
│   │       │   │   │   │   │   ├── Life_Support_Dome_sealed_airlock_Rails.asset
│   │       │   │   │   │   │   ├── Life_Support_Dome_sealed_airlock_Rails.asset.meta
│   │       │   │   │   │   │   ├── RoverMechanicalFrame.asset
│   │       │   │   │   │   │   ├── RoverMechanicalFrame.asset.meta
│   │       │   │   │   │   │   ├── RoverWheel_LOD0.asset
│   │       │   │   │   │   │   ├── RoverWheel_LOD0.asset.meta
│   │       │   │   │   │   │   ├── RoverWheel_LOD1.asset
│   │       │   │   │   │   │   ├── RoverWheel_LOD1.asset.meta
│   │       │   │   │   │   │   ├── RoverWheel_LOD2.asset
│   │       │   │   │   │   │   └── RoverWheel_LOD2.asset.meta
│   │       │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   ├── Mars_ProductionVisual_v1.unity
│   │       │   │   │   │   │   └── Mars_ProductionVisual_v1.unity.meta
│   │       │   │   │   │   ├── Scripts/
│   │       │   │   │   │   │   ├── ProductionCameraRig.cs
│   │       │   │   │   │   │   ├── ProductionCameraRig.cs.meta
│   │       │   │   │   │   │   ├── ProductionModeCoordinator.cs
│   │       │   │   │   │   │   ├── ProductionModeCoordinator.cs.meta
│   │       │   │   │   │   │   ├── ProductionPerformanceProbe.cs
│   │       │   │   │   │   │   ├── ProductionPerformanceProbe.cs.meta
│   │       │   │   │   │   │   ├── ProductionPresentation.cs
│   │       │   │   │   │   │   └── ProductionPresentation.cs.meta
│   │       │   │   │   │   ├── Settings/
│   │       │   │   │   │   │   ├── Lighting_0.asset
│   │       │   │   │   │   │   ├── Lighting_0.asset.meta
│   │       │   │   │   │   │   ├── Lighting_1.asset
│   │       │   │   │   │   │   ├── Lighting_1.asset.meta
│   │       │   │   │   │   │   ├── Lighting_2.asset
│   │       │   │   │   │   │   ├── Lighting_2.asset.meta
│   │       │   │   │   │   │   ├── Lighting_3.asset
│   │       │   │   │   │   │   ├── Lighting_3.asset.meta
│   │       │   │   │   │   │   ├── ProductionMissionRegistry.asset
│   │       │   │   │   │   │   └── ProductionMissionRegistry.asset.meta
│   │       │   │   │   │   ├── Tests/
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── ProductionMeshFactory.cs
│   │       │   │   │   │   │   │   ├── ProductionMeshFactory.cs.meta
│   │       │   │   │   │   │   │   ├── ProductionViewport.cs
│   │       │   │   │   │   │   │   ├── ProductionViewport.cs.meta
│   │       │   │   │   │   │   │   ├── ProductionVisualBuilder.cs
│   │       │   │   │   │   │   │   └── ProductionVisualBuilder.cs.meta
│   │       │   │   │   │   │   └── Editor.meta
│   │       │   │   │   │   ├── Textures/
│   │       │   │   │   │   │   ├── rock_07_MetallicSmoothness.png
│   │       │   │   │   │   │   ├── rock_07_MetallicSmoothness.png.meta
│   │       │   │   │   │   │   ├── rock_09_MetallicSmoothness.png
│   │       │   │   │   │   │   ├── rock_09_MetallicSmoothness.png.meta
│   │       │   │   │   │   │   ├── sandy_gravel_MetallicSmoothness.png
│   │       │   │   │   │   │   └── sandy_gravel_MetallicSmoothness.png.meta
│   │       │   │   │   │   ├── Themes/
│   │       │   │   │   │   │   ├── ProductionHUDTheme.asset
│   │       │   │   │   │   │   ├── ProductionHUDTheme.asset.meta
│   │       │   │   │   │   │   ├── ProductionSky.mat
│   │       │   │   │   │   │   ├── ProductionSky.mat.meta
│   │       │   │   │   │   │   ├── ProductionVolume.asset
│   │       │   │   │   │   │   └── ProductionVolume.asset.meta
│   │       │   │   │   │   ├── Documentation.meta
│   │       │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   ├── Meshes.meta
│   │       │   │   │   │   ├── Prefabs.meta
│   │       │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   ├── Scripts.meta
│   │       │   │   │   │   ├── Settings.meta
│   │       │   │   │   │   ├── Tests.meta
│   │       │   │   │   │   ├── Textures.meta
│   │       │   │   │   │   └── Themes.meta
│   │       │   │   │   ├── Scenes/
│   │       │   │   │   │   ├── MarsOutpost_Environment_v01.unity
│   │       │   │   │   │   └── MarsOutpost_Environment_v01.unity.meta
│   │       │   │   │   ├── Stage08B/
│   │       │   │   │   │   ├── Animation/
│   │       │   │   │   │   │   ├── Idle.anim
│   │       │   │   │   │   │   ├── Idle.anim.meta
│   │       │   │   │   │   │   ├── PrototypeLocomotion.controller
│   │       │   │   │   │   │   ├── PrototypeLocomotion.controller.meta
│   │       │   │   │   │   │   ├── Run.anim
│   │       │   │   │   │   │   ├── Run.anim.meta
│   │       │   │   │   │   │   ├── Walk.anim
│   │       │   │   │   │   │   └── Walk.anim.meta
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── Stage08BBuilder.cs
│   │       │   │   │   │   │   └── Stage08BBuilder.cs.meta
│   │       │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   ├── 08B_Accent_Cyan.mat
│   │       │   │   │   │   │   ├── 08B_Accent_Cyan.mat.meta
│   │       │   │   │   │   │   ├── 08B_Accent_SafetyOrange.mat
│   │       │   │   │   │   │   ├── 08B_Accent_SafetyOrange.mat.meta
│   │       │   │   │   │   │   ├── 08B_Glass_Utility.mat
│   │       │   │   │   │   │   ├── 08B_Glass_Utility.mat.meta
│   │       │   │   │   │   │   ├── 08B_Habitat_CompositeDark.mat
│   │       │   │   │   │   │   ├── 08B_Habitat_CompositeDark.mat.meta
│   │       │   │   │   │   │   ├── 08B_Habitat_OffWhite.mat
│   │       │   │   │   │   │   ├── 08B_Habitat_OffWhite.mat.meta
│   │       │   │   │   │   │   ├── 08B_Mars_Regolith.mat
│   │       │   │   │   │   │   ├── 08B_Mars_Regolith.mat.meta
│   │       │   │   │   │   │   ├── 08B_Mars_Regolith_Dark.mat
│   │       │   │   │   │   │   ├── 08B_Mars_Regolith_Dark.mat.meta
│   │       │   │   │   │   │   ├── 08B_Mars_Rock_Basaltic.mat
│   │       │   │   │   │   │   ├── 08B_Mars_Rock_Basaltic.mat.meta
│   │       │   │   │   │   │   ├── 08B_Mars_Rock_Layered.mat
│   │       │   │   │   │   │   ├── 08B_Mars_Rock_Layered.mat.meta
│   │       │   │   │   │   │   ├── 08B_Metal_Industrial.mat
│   │       │   │   │   │   │   ├── 08B_Metal_Industrial.mat.meta
│   │       │   │   │   │   │   ├── 08B_SolarPanel.mat
│   │       │   │   │   │   │   ├── 08B_SolarPanel.mat.meta
│   │       │   │   │   │   │   ├── BasaltVariationA.mat
│   │       │   │   │   │   │   ├── BasaltVariationA.mat.meta
│   │       │   │   │   │   │   ├── BasaltVariationB.mat
│   │       │   │   │   │   │   ├── BasaltVariationB.mat.meta
│   │       │   │   │   │   │   ├── BasaltVariationC.mat
│   │       │   │   │   │   │   ├── BasaltVariationC.mat.meta
│   │       │   │   │   │   │   ├── Dust_Particle.mat
│   │       │   │   │   │   │   ├── Dust_Particle.mat.meta
│   │       │   │   │   │   │   ├── Identification_Cyan.mat
│   │       │   │   │   │   │   ├── Identification_Cyan.mat.meta
│   │       │   │   │   │   │   ├── Safety_Orange.mat
│   │       │   │   │   │   │   ├── Safety_Orange.mat.meta
│   │       │   │   │   │   │   ├── Suit_Graphite.mat
│   │       │   │   │   │   │   ├── Suit_Graphite.mat.meta
│   │       │   │   │   │   │   ├── Suit_Pale.mat
│   │       │   │   │   │   │   └── Suit_Pale.mat.meta
│   │       │   │   │   │   ├── Animation.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── MarsOutpost_Playable_Stage08B.unity
│   │       │   │   │   │   ├── MarsOutpost_Playable_Stage08B.unity.meta
│   │       │   │   │   │   ├── MarsPrototype.cs
│   │       │   │   │   │   ├── MarsPrototype.cs.meta
│   │       │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   ├── Stage08B_MobilePreview.asset
│   │       │   │   │   │   └── Stage08B_MobilePreview.asset.meta
│   │       │   │   │   ├── Stage08C/
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── AndroidProbeBuild.cs
│   │       │   │   │   │   │   ├── AndroidProbeBuild.cs.meta
│   │       │   │   │   │   │   ├── TouchChecks.cs
│   │       │   │   │   │   │   └── TouchChecks.cs.meta
│   │       │   │   │   │   ├── AndroidProfileProbe.cs
│   │       │   │   │   │   ├── AndroidProfileProbe.cs.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── MobileControls.cs
│   │       │   │   │   │   └── MobileControls.cs.meta
│   │       │   │   │   ├── Stage08C1/
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── EditorPerformanceObserver.cs
│   │       │   │   │   │   │   ├── EditorPerformanceObserver.cs.meta
│   │       │   │   │   │   │   ├── ManualInputObserver.cs
│   │       │   │   │   │   │   └── ManualInputObserver.cs.meta
│   │       │   │   │   │   └── Editor.meta
│   │       │   │   │   ├── Verification/
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── Step1GameplayVerification.cs
│   │       │   │   │   │   │   ├── Step1GameplayVerification.cs.meta
│   │       │   │   │   │   │   ├── Step3BackendClientTest.cs
│   │       │   │   │   │   │   └── Step3BackendClientTest.cs.meta
│   │       │   │   │   │   └── Editor.meta
│   │       │   │   │   ├── CinematicHUD.meta
│   │       │   │   │   ├── Environment.meta
│   │       │   │   │   ├── Greybox.meta
│   │       │   │   │   ├── Integration.meta
│   │       │   │   │   ├── MarsEnvironment.meta
│   │       │   │   │   ├── MarsVisualPolish.meta
│   │       │   │   │   ├── Mission01Milestone.meta
│   │       │   │   │   ├── MissionIntegration.meta
│   │       │   │   │   ├── NativeControlSlice.meta
│   │       │   │   │   ├── ProductionVisual.meta
│   │       │   │   │   ├── Scenes.meta
│   │       │   │   │   ├── Stage08B.meta
│   │       │   │   │   ├── Stage08C.meta
│   │       │   │   │   ├── Stage08C1.meta
│   │       │   │   │   └── Verification.meta
│   │       │   │   ├── Scenes/
│   │       │   │   │   ├── SampleScene.unity
│   │       │   │   │   └── SampleScene.unity.meta
│   │       │   │   ├── Settings/
│   │       │   │   │   ├── DefaultVolumeProfile.asset
│   │       │   │   │   ├── DefaultVolumeProfile.asset.meta
│   │       │   │   │   ├── Mobile_Renderer.asset
│   │       │   │   │   ├── Mobile_Renderer.asset.meta
│   │       │   │   │   ├── Mobile_RPAsset.asset
│   │       │   │   │   ├── Mobile_RPAsset.asset.meta
│   │       │   │   │   ├── PC_Renderer.asset
│   │       │   │   │   ├── PC_Renderer.asset.meta
│   │       │   │   │   ├── PC_RPAsset.asset
│   │       │   │   │   ├── PC_RPAsset.asset.meta
│   │       │   │   │   ├── SampleSceneProfile.asset
│   │       │   │   │   ├── SampleSceneProfile.asset.meta
│   │       │   │   │   ├── UniversalRenderPipelineGlobalSettings.asset
│   │       │   │   │   └── UniversalRenderPipelineGlobalSettings.asset.meta
│   │       │   │   ├── TutorialInfo/
│   │       │   │   │   ├── Editor/
│   │       │   │   │   │   ├── ReadmeEditor.cs
│   │       │   │   │   │   └── ReadmeEditor.cs.meta
│   │       │   │   │   ├── Icons/
│   │       │   │   │   │   ├── URP.png
│   │       │   │   │   │   └── URP.png.meta
│   │       │   │   │   ├── StyleSheets/
│   │       │   │   │   │   ├── ReadmeEditor.uss
│   │       │   │   │   │   ├── ReadmeEditor.uss.meta
│   │       │   │   │   │   ├── ReadmeEditorDark.uss
│   │       │   │   │   │   ├── ReadmeEditorDark.uss.meta
│   │       │   │   │   │   ├── ReadmeEditorLight.uss
│   │       │   │   │   │   └── ReadmeEditorLight.uss.meta
│   │       │   │   │   ├── Editor.meta
│   │       │   │   │   ├── Icons.meta
│   │       │   │   │   ├── Layout.wlt
│   │       │   │   │   ├── Layout.wlt.meta
│   │       │   │   │   ├── Readme.cs
│   │       │   │   │   ├── Readme.cs.meta
│   │       │   │   │   └── StyleSheets.meta
│   │       │   │   ├── AstroGateway.meta
│   │       │   │   ├── InputSystem_Actions.inputactions
│   │       │   │   ├── InputSystem_Actions.inputactions.meta
│   │       │   │   ├── Readme.asset
│   │       │   │   ├── Readme.asset.meta
│   │       │   │   ├── Scenes.meta
│   │       │   │   ├── Settings.meta
│   │       │   │   └── TutorialInfo.meta
│   │       │   ├── Data/
│   │       │   │   └── Plugins/
│   │       │   │       ├── lib_burst_generated.cpp
│   │       │   │       └── lib_burst_generated.wasm
│   │       │   ├── Docs/
│   │       │   │   ├── Design/
│   │       │   │   │   ├── 01_Vision_And_Art_Direction.md
│   │       │   │   │   ├── 02_Mars_World_Layout.md
│   │       │   │   │   ├── 03_Environment_Asset_List.md
│   │       │   │   │   ├── 04_Unity_URP_Technical_Standards.md
│   │       │   │   │   ├── 05_Gameplay_Environment_Integration.md
│   │       │   │   │   ├── 06_References_And_Attribution.md
│   │       │   │   │   └── 07_Implementation_Roadmap.md
│   │       │   │   ├── Stage04/
│   │       │   │   │   ├── Assets_And_Attribution.md
│   │       │   │   │   ├── Stage04_Completion_Report.md
│   │       │   │   │   └── Validation_Evidence_Paths.txt
│   │       │   │   ├── Step1/
│   │       │   │   │   └── Step1_Gameplay_Results.txt
│   │       │   │   ├── Integration_Architecture.md
│   │       │   │   └── PROGRESS_REPORT.md
│   │       │   ├── Packages/
│   │       │   │   ├── manifest.json
│   │       │   │   └── packages-lock.json
│   │       │   ├── ProjectSettings/
│   │       │   │   ├── AudioManager.asset
│   │       │   │   ├── ClusterInputManager.asset
│   │       │   │   ├── DynamicsManager.asset
│   │       │   │   ├── EditorBuildSettings.asset
│   │       │   │   ├── EditorSettings.asset
│   │       │   │   ├── GraphicsSettings.asset
│   │       │   │   ├── InputManager.asset
│   │       │   │   ├── MemorySettings.asset
│   │       │   │   ├── MultiplayerManager.asset
│   │       │   │   ├── NavMeshAreas.asset
│   │       │   │   ├── PackageManagerSettings.asset
│   │       │   │   ├── Physics2DSettings.asset
│   │       │   │   ├── PhysicsCoreProjectSettings2D.asset
│   │       │   │   ├── PresetManager.asset
│   │       │   │   ├── ProjectAuditorSettings.asset
│   │       │   │   ├── ProjectSettings.asset
│   │       │   │   ├── ProjectVersion.txt
│   │       │   │   ├── QualitySettings.asset
│   │       │   │   ├── SceneTemplateSettings.json
│   │       │   │   ├── ShaderGraphSettings.asset
│   │       │   │   ├── TagManager.asset
│   │       │   │   ├── TimeManager.asset
│   │       │   │   ├── UnityConnectSettings.asset
│   │       │   │   ├── URPProjectSettings.asset
│   │       │   │   ├── VersionControlSettings.asset
│   │       │   │   ├── VFXManager.asset
│   │       │   │   └── XRSettings.asset
│   │       │   └── .gitignore
│   │       ├── MacBuild/
│   │       │   └── Project/
│   │       │       ├── Assets/
│   │       │       │   ├── AstroGateway/
│   │       │       │   │   ├── CinematicHUD/
│   │       │       │   │   │   ├── Documentation/
│   │       │       │   │   │   │   ├── README.md
│   │       │       │   │   │   │   └── README.md.meta
│   │       │       │   │   │   ├── Prefabs/
│   │       │       │   │   │   │   ├── AstroGatewayCinematicHUD.prefab
│   │       │       │   │   │   │   └── AstroGatewayCinematicHUD.prefab.meta
│   │       │       │   │   │   ├── Scenes/
│   │       │       │   │   │   │   ├── Mars_CinematicHUD_v1.unity
│   │       │       │   │   │   │   └── Mars_CinematicHUD_v1.unity.meta
│   │       │       │   │   │   ├── Scripts/
│   │       │       │   │   │   │   ├── AstroGatewayHUDController.cs
│   │       │       │   │   │   │   ├── AstroGatewayHUDController.cs.meta
│   │       │       │   │   │   │   ├── CinematicEnvironmentAdapter.cs
│   │       │       │   │   │   │   ├── CinematicEnvironmentAdapter.cs.meta
│   │       │       │   │   │   │   ├── CinematicGameAdapter.cs
│   │       │       │   │   │   │   ├── CinematicGameAdapter.cs.meta
│   │       │       │   │   │   │   ├── HUDInputRouter.cs
│   │       │       │   │   │   │   ├── HUDInputRouter.cs.meta
│   │       │       │   │   │   │   ├── HUDLayoutProbe.cs
│   │       │       │   │   │   │   ├── HUDLayoutProbe.cs.meta
│   │       │       │   │   │   │   ├── HUDPerformanceProbe.cs
│   │       │       │   │   │   │   ├── HUDPerformanceProbe.cs.meta
│   │       │       │   │   │   │   ├── HUDPointerControl.cs
│   │       │       │   │   │   │   ├── HUDPointerControl.cs.meta
│   │       │       │   │   │   │   ├── HUDTelemetryAdapter.cs
│   │       │       │   │   │   │   ├── HUDTelemetryAdapter.cs.meta
│   │       │       │   │   │   │   ├── HUDTheme.cs
│   │       │       │   │   │   │   └── HUDTheme.cs.meta
│   │       │       │   │   │   ├── Settings/
│   │       │       │   │   │   │   ├── MissionRegistry.asset
│   │       │       │   │   │   │   └── MissionRegistry.asset.meta
│   │       │       │   │   │   ├── Tests/
│   │       │       │   │   │   │   ├── Editor/
│   │       │       │   │   │   │   │   ├── CinematicHUDBuilder.cs
│   │       │       │   │   │   │   │   ├── CinematicHUDBuilder.cs.meta
│   │       │       │   │   │   │   │   ├── HUDResponsiveQA.cs
│   │       │       │   │   │   │   │   └── HUDResponsiveQA.cs.meta
│   │       │       │   │   │   │   └── Editor.meta
│   │       │       │   │   │   ├── Themes/
│   │       │       │   │   │   │   ├── AstroGatewayHUDTheme.asset
│   │       │       │   │   │   │   └── AstroGatewayHUDTheme.asset.meta
│   │       │       │   │   │   ├── UI/
│   │       │       │   │   │   │   ├── CircularControl.png
│   │       │       │   │   │   │   ├── CircularControl.png.meta
│   │       │       │   │   │   │   ├── HeadingArrow.png
│   │       │       │   │   │   │   ├── HeadingArrow.png.meta
│   │       │       │   │   │   │   ├── OriginalContourMap.png
│   │       │       │   │   │   │   ├── OriginalContourMap.png.meta
│   │       │       │   │   │   │   ├── RoundedPanel.png
│   │       │       │   │   │   │   └── RoundedPanel.png.meta
│   │       │       │   │   │   ├── Documentation.meta
│   │       │       │   │   │   ├── link.xml
│   │       │       │   │   │   ├── link.xml.meta
│   │       │       │   │   │   ├── Prefabs.meta
│   │       │       │   │   │   ├── Scenes.meta
│   │       │       │   │   │   ├── Scripts.meta
│   │       │       │   │   │   ├── Settings.meta
│   │       │       │   │   │   ├── Tests.meta
│   │       │       │   │   │   ├── Themes.meta
│   │       │       │   │   │   └── UI.meta
│   │       │       │   │   ├── Environment/
│   │       │       │   │   │   ├── Architecture/
│   │       │       │   │   │   │   ├── AgricultureG01_ChamferShell.asset
│   │       │       │   │   │   │   ├── AgricultureG01_ChamferShell.asset.meta
│   │       │       │   │   │   │   ├── AgricultureG01_Ring.asset
│   │       │       │   │   │   │   ├── AgricultureG01_Ring.asset.meta
│   │       │       │   │   │   │   ├── AirlockShell.asset
│   │       │       │   │   │   │   ├── AirlockShell.asset.meta
│   │       │       │   │   │   │   ├── C_ScienceStaging_Accent_Cyan_Combined.asset
│   │       │       │   │   │   │   ├── C_ScienceStaging_Accent_Cyan_Combined.asset.meta
│   │       │       │   │   │   │   ├── C_ScienceStaging_Accent_SafetyOrange_Combined.asset
│   │       │       │   │   │   │   ├── C_ScienceStaging_Accent_SafetyOrange_Combined.asset.meta
│   │       │       │   │   │   │   ├── C_ScienceStaging_Glass_Utility_Combined.asset
│   │       │       │   │   │   │   ├── C_ScienceStaging_Glass_Utility_Combined.asset.meta
│   │       │       │   │   │   │   ├── C_ScienceStaging_Habitat_CompositeDark_Combined.asset
│   │       │       │   │   │   │   ├── C_ScienceStaging_Habitat_CompositeDark_Combined.asset.meta
│   │       │       │   │   │   │   ├── C_ScienceStaging_Habitat_OffWhite_Combined.asset
│   │       │       │   │   │   │   ├── C_ScienceStaging_Habitat_OffWhite_Combined.asset.meta
│   │       │       │   │   │   │   ├── C_ScienceStaging_Metal_Industrial_Combined.asset
│   │       │       │   │   │   │   ├── C_ScienceStaging_Metal_Industrial_Combined.asset.meta
│   │       │       │   │   │   │   ├── G_Greenhouse_Accent_Cyan_Combined.asset
│   │       │       │   │   │   │   ├── G_Greenhouse_Accent_Cyan_Combined.asset.meta
│   │       │       │   │   │   │   ├── G_Greenhouse_Glass_Utility_Combined.asset
│   │       │       │   │   │   │   ├── G_Greenhouse_Glass_Utility_Combined.asset.meta
│   │       │       │   │   │   │   ├── G_Greenhouse_Habitat_CompositeDark_Combined.asset
│   │       │       │   │   │   │   ├── G_Greenhouse_Habitat_CompositeDark_Combined.asset.meta
│   │       │       │   │   │   │   ├── G_Greenhouse_Habitat_OffWhite_Combined.asset
│   │       │       │   │   │   │   ├── G_Greenhouse_Habitat_OffWhite_Combined.asset.meta
│   │       │       │   │   │   │   ├── G_Greenhouse_Metal_Industrial_Combined.asset
│   │       │       │   │   │   │   ├── G_Greenhouse_Metal_Industrial_Combined.asset.meta
│   │       │       │   │   │   │   ├── H_CommandHabitat_Accent_Cyan_Combined.asset
│   │       │       │   │   │   │   ├── H_CommandHabitat_Accent_Cyan_Combined.asset.meta
│   │       │       │   │   │   │   ├── H_CommandHabitat_Glass_Utility_Combined.asset
│   │       │       │   │   │   │   ├── H_CommandHabitat_Glass_Utility_Combined.asset.meta
│   │       │       │   │   │   │   ├── H_CommandHabitat_Habitat_CompositeDark_Combined.asset
│   │       │       │   │   │   │   ├── H_CommandHabitat_Habitat_CompositeDark_Combined.asset.meta
│   │       │       │   │   │   │   ├── H_CommandHabitat_Habitat_OffWhite_Combined.asset
│   │       │       │   │   │   │   ├── H_CommandHabitat_Habitat_OffWhite_Combined.asset.meta
│   │       │       │   │   │   │   ├── H_CommandHabitat_Metal_Industrial_Combined.asset
│   │       │       │   │   │   │   ├── H_CommandHabitat_Metal_Industrial_Combined.asset.meta
│   │       │       │   │   │   │   ├── HabitatH01_ChamferShell.asset
│   │       │       │   │   │   │   ├── HabitatH01_ChamferShell.asset.meta
│   │       │       │   │   │   │   ├── HabitatH01_Ring.asset
│   │       │       │   │   │   │   ├── HabitatH01_Ring.asset.meta
│   │       │       │   │   │   │   ├── P_PowerStation_Accent_Cyan_Combined.asset
│   │       │       │   │   │   │   ├── P_PowerStation_Accent_Cyan_Combined.asset.meta
│   │       │       │   │   │   │   ├── P_PowerStation_Accent_SafetyOrange_Combined.asset
│   │       │       │   │   │   │   ├── P_PowerStation_Accent_SafetyOrange_Combined.asset.meta
│   │       │       │   │   │   │   ├── P_PowerStation_Glass_Utility_Combined.asset
│   │       │       │   │   │   │   ├── P_PowerStation_Glass_Utility_Combined.asset.meta
│   │       │       │   │   │   │   ├── P_PowerStation_Habitat_CompositeDark_Combined.asset
│   │       │       │   │   │   │   ├── P_PowerStation_Habitat_CompositeDark_Combined.asset.meta
│   │       │       │   │   │   │   ├── P_PowerStation_Habitat_OffWhite_Combined.asset
│   │       │       │   │   │   │   ├── P_PowerStation_Habitat_OffWhite_Combined.asset.meta
│   │       │       │   │   │   │   ├── P_PowerStation_Metal_Industrial_Combined.asset
│   │       │       │   │   │   │   ├── P_PowerStation_Metal_Industrial_Combined.asset.meta
│   │       │       │   │   │   │   ├── R_RoverBay_Habitat_CompositeDark_Combined.asset
│   │       │       │   │   │   │   ├── R_RoverBay_Habitat_CompositeDark_Combined.asset.meta
│   │       │       │   │   │   │   ├── R_RoverBay_Habitat_OffWhite_Combined.asset
│   │       │       │   │   │   │   ├── R_RoverBay_Habitat_OffWhite_Combined.asset.meta
│   │       │       │   │   │   │   ├── R_RoverBay_Mars_Regolith_Dark_Combined.asset
│   │       │       │   │   │   │   ├── R_RoverBay_Mars_Regolith_Dark_Combined.asset.meta
│   │       │       │   │   │   │   ├── R_RoverBay_Mars_Rock_Basaltic_Combined.asset
│   │       │       │   │   │   │   ├── R_RoverBay_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │       │   │   │   │   ├── R_RoverBay_Metal_Industrial_Combined.asset
│   │       │       │   │   │   │   ├── R_RoverBay_Metal_Industrial_Combined.asset.meta
│   │       │       │   │   │   │   ├── S_SolarArray_Habitat_CompositeDark_Combined.asset
│   │       │       │   │   │   │   ├── S_SolarArray_Habitat_CompositeDark_Combined.asset.meta
│   │       │       │   │   │   │   ├── S_SolarArray_Habitat_OffWhite_Combined.asset
│   │       │       │   │   │   │   ├── S_SolarArray_Habitat_OffWhite_Combined.asset.meta
│   │       │       │   │   │   │   ├── S_SolarArray_Metal_Industrial_Combined.asset
│   │       │       │   │   │   │   ├── S_SolarArray_Metal_Industrial_Combined.asset.meta
│   │       │       │   │   │   │   ├── S_SolarArray_SolarPanel_Combined.asset
│   │       │       │   │   │   │   └── S_SolarArray_SolarPanel_Combined.asset.meta
│   │       │       │   │   │   ├── Editor/
│   │       │       │   │   │   │   ├── MarsEnvironmentBuilder.cs
│   │       │       │   │   │   │   └── MarsEnvironmentBuilder.cs.meta
│   │       │       │   │   │   ├── Materials/
│   │       │       │   │   │   │   ├── Accent_Cyan.mat
│   │       │       │   │   │   │   ├── Accent_Cyan.mat.meta
│   │       │       │   │   │   │   ├── Accent_SafetyOrange.mat
│   │       │       │   │   │   │   ├── Accent_SafetyOrange.mat.meta
│   │       │       │   │   │   │   ├── Basalt_Surface_512.png
│   │       │       │   │   │   │   ├── Basalt_Surface_512.png.meta
│   │       │       │   │   │   │   ├── Composite_DustPaint_512.png
│   │       │       │   │   │   │   ├── Composite_DustPaint_512.png.meta
│   │       │       │   │   │   │   ├── Delta_Bedding_512.png
│   │       │       │   │   │   │   ├── Delta_Bedding_512.png.meta
│   │       │       │   │   │   │   ├── Delta_FracturedStrata_512.png
│   │       │       │   │   │   │   ├── Delta_FracturedStrata_512.png.meta
│   │       │       │   │   │   │   ├── DustSky.shader
│   │       │       │   │   │   │   ├── DustSky.shader.meta
│   │       │       │   │   │   │   ├── Glass_Utility.mat
│   │       │       │   │   │   │   ├── Glass_Utility.mat.meta
│   │       │       │   │   │   │   ├── Habitat_CompositeDark.mat
│   │       │       │   │   │   │   ├── Habitat_CompositeDark.mat.meta
│   │       │       │   │   │   │   ├── Habitat_OffWhite.mat
│   │       │       │   │   │   │   ├── Habitat_OffWhite.mat.meta
│   │       │       │   │   │   │   ├── Mars_DustSky.mat
│   │       │       │   │   │   │   ├── Mars_DustSky.mat.meta
│   │       │       │   │   │   │   ├── Mars_Regolith.mat
│   │       │       │   │   │   │   ├── Mars_Regolith.mat.meta
│   │       │       │   │   │   │   ├── Mars_Regolith_Dark.mat
│   │       │       │   │   │   │   ├── Mars_Regolith_Dark.mat.meta
│   │       │       │   │   │   │   ├── Mars_Rock_Basaltic.mat
│   │       │       │   │   │   │   ├── Mars_Rock_Basaltic.mat.meta
│   │       │       │   │   │   │   ├── Mars_Rock_Layered.mat
│   │       │       │   │   │   │   ├── Mars_Rock_Layered.mat.meta
│   │       │       │   │   │   │   ├── Metal_Industrial.mat
│   │       │       │   │   │   │   ├── Metal_Industrial.mat.meta
│   │       │       │   │   │   │   ├── Regolith_Grain_512.png
│   │       │       │   │   │   │   ├── Regolith_Grain_512.png.meta
│   │       │       │   │   │   │   ├── Regolith_Macro_Pathwear_1024.png
│   │       │       │   │   │   │   ├── Regolith_Macro_Pathwear_1024.png.meta
│   │       │       │   │   │   │   ├── Regolith_MicroNormal_256.png
│   │       │       │   │   │   │   ├── Regolith_MicroNormal_256.png.meta
│   │       │       │   │   │   │   ├── Solar_Cells_Dust_512.png
│   │       │       │   │   │   │   ├── Solar_Cells_Dust_512.png.meta
│   │       │       │   │   │   │   ├── SolarPanel.mat
│   │       │       │   │   │   │   └── SolarPanel.mat.meta
│   │       │       │   │   │   ├── Prefabs/
│   │       │       │   │   │   │   ├── AngularBasalt.prefab
│   │       │       │   │   │   │   ├── AngularBasalt.prefab.meta
│   │       │       │   │   │   │   ├── C_ScienceStaging.prefab
│   │       │       │   │   │   │   ├── C_ScienceStaging.prefab.meta
│   │       │       │   │   │   │   ├── ErodedLandmark.prefab
│   │       │       │   │   │   │   ├── ErodedLandmark.prefab.meta
│   │       │       │   │   │   │   ├── G_Greenhouse.prefab
│   │       │       │   │   │   │   ├── G_Greenhouse.prefab.meta
│   │       │       │   │   │   │   ├── H_CommandHabitat.prefab
│   │       │       │   │   │   │   ├── H_CommandHabitat.prefab.meta
│   │       │       │   │   │   │   ├── LayeredDelta.prefab
│   │       │       │   │   │   │   ├── LayeredDelta.prefab.meta
│   │       │       │   │   │   │   ├── P_PowerStation.prefab
│   │       │       │   │   │   │   ├── P_PowerStation.prefab.meta
│   │       │       │   │   │   │   ├── R_RoverBay.prefab
│   │       │       │   │   │   │   ├── R_RoverBay.prefab.meta
│   │       │       │   │   │   │   ├── RegolithStone.prefab
│   │       │       │   │   │   │   ├── RegolithStone.prefab.meta
│   │       │       │   │   │   │   ├── S_SolarArray.prefab
│   │       │       │   │   │   │   └── S_SolarArray.prefab.meta
│   │       │       │   │   │   ├── Props/
│   │       │       │   │   │   │   ├── ApproachStoneClusters_Mars_Rock_Basaltic_Combined.asset
│   │       │       │   │   │   │   ├── ApproachStoneClusters_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │       │   │   │   │   ├── HabitatExternalKit_Habitat_CompositeDark_Combined.asset
│   │       │       │   │   │   │   ├── HabitatExternalKit_Habitat_CompositeDark_Combined.asset.meta
│   │       │       │   │   │   │   ├── HabitatExternalKit_Habitat_OffWhite_Combined.asset
│   │       │       │   │   │   │   ├── HabitatExternalKit_Habitat_OffWhite_Combined.asset.meta
│   │       │       │   │   │   │   ├── HabitatExternalKit_Metal_Industrial_Combined.asset
│   │       │       │   │   │   │   ├── HabitatExternalKit_Metal_Industrial_Combined.asset.meta
│   │       │       │   │   │   │   ├── PowerExternalKit_Habitat_CompositeDark_Combined.asset
│   │       │       │   │   │   │   ├── PowerExternalKit_Habitat_CompositeDark_Combined.asset.meta
│   │       │       │   │   │   │   ├── PowerExternalKit_Mars_Regolith_Dark_Combined.asset
│   │       │       │   │   │   │   ├── PowerExternalKit_Mars_Regolith_Dark_Combined.asset.meta
│   │       │       │   │   │   │   ├── UtilityRoutePosts_Accent_Cyan_Combined.asset
│   │       │       │   │   │   │   ├── UtilityRoutePosts_Accent_Cyan_Combined.asset.meta
│   │       │       │   │   │   │   ├── UtilityRoutePosts_Metal_Industrial_Combined.asset
│   │       │       │   │   │   │   └── UtilityRoutePosts_Metal_Industrial_Combined.asset.meta
│   │       │       │   │   │   ├── Rocks/
│   │       │       │   │   │   │   ├── AngularBasalt_LOD0.asset
│   │       │       │   │   │   │   ├── AngularBasalt_LOD0.asset.meta
│   │       │       │   │   │   │   ├── AngularBasalt_LOD1.asset
│   │       │       │   │   │   │   ├── AngularBasalt_LOD1.asset.meta
│   │       │       │   │   │   │   ├── AngularBasalt_LOD2.asset
│   │       │       │   │   │   │   ├── AngularBasalt_LOD2.asset.meta
│   │       │       │   │   │   │   ├── DeltaFractured_LOD0.asset
│   │       │       │   │   │   │   ├── DeltaFractured_LOD0.asset.meta
│   │       │       │   │   │   │   ├── DeltaFractured_LOD1.asset
│   │       │       │   │   │   │   ├── DeltaFractured_LOD1.asset.meta
│   │       │       │   │   │   │   ├── DeltaFractured_LOD2.asset
│   │       │       │   │   │   │   ├── DeltaFractured_LOD2.asset.meta
│   │       │       │   │   │   │   ├── ErodedLandmark_LOD0.asset
│   │       │       │   │   │   │   ├── ErodedLandmark_LOD0.asset.meta
│   │       │       │   │   │   │   ├── ErodedLandmark_LOD1.asset
│   │       │       │   │   │   │   ├── ErodedLandmark_LOD1.asset.meta
│   │       │       │   │   │   │   ├── ErodedLandmark_LOD2.asset
│   │       │       │   │   │   │   ├── ErodedLandmark_LOD2.asset.meta
│   │       │       │   │   │   │   ├── LayeredDelta_LOD0.asset
│   │       │       │   │   │   │   ├── LayeredDelta_LOD0.asset.meta
│   │       │       │   │   │   │   ├── LayeredDelta_LOD1.asset
│   │       │       │   │   │   │   ├── LayeredDelta_LOD1.asset.meta
│   │       │       │   │   │   │   ├── LayeredDelta_LOD2.asset
│   │       │       │   │   │   │   ├── LayeredDelta_LOD2.asset.meta
│   │       │       │   │   │   │   ├── StonePatch_0_Mars_Rock_Basaltic_Combined.asset
│   │       │       │   │   │   │   ├── StonePatch_0_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │       │   │   │   │   ├── StonePatch_1_Mars_Rock_Basaltic_Combined.asset
│   │       │       │   │   │   │   ├── StonePatch_1_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │       │   │   │   │   ├── StonePatch_2_Mars_Rock_Basaltic_Combined.asset
│   │       │       │   │   │   │   ├── StonePatch_2_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │       │   │   │   │   ├── StonePatch_3_Mars_Rock_Basaltic_Combined.asset
│   │       │       │   │   │   │   ├── StonePatch_3_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │       │   │   │   │   ├── StonePatch_4_Mars_Rock_Basaltic_Combined.asset
│   │       │       │   │   │   │   ├── StonePatch_4_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │       │   │   │   │   ├── StonePatch_5_Mars_Rock_Basaltic_Combined.asset
│   │       │       │   │   │   │   ├── StonePatch_5_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │       │   │   │   │   ├── StonePatch_6_Mars_Rock_Basaltic_Combined.asset
│   │       │       │   │   │   │   ├── StonePatch_6_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │       │   │   │   │   ├── StonePatch_7_Mars_Rock_Basaltic_Combined.asset
│   │       │       │   │   │   │   ├── StonePatch_7_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │       │   │   │   │   ├── StonePatch_8_Mars_Rock_Basaltic_Combined.asset
│   │       │       │   │   │   │   └── StonePatch_8_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │       │   │   │   ├── Terrain/
│   │       │       │   │   │   │   ├── Regolith_250m_PreservedCorridor.asset
│   │       │       │   │   │   │   └── Regolith_250m_PreservedCorridor.asset.meta
│   │       │       │   │   │   ├── Architecture.meta
│   │       │       │   │   │   ├── Editor.meta
│   │       │       │   │   │   ├── Materials.meta
│   │       │       │   │   │   ├── Prefabs.meta
│   │       │       │   │   │   ├── Props.meta
│   │       │       │   │   │   ├── Rocks.meta
│   │       │       │   │   │   └── Terrain.meta
│   │       │       │   │   ├── Greybox/
│   │       │       │   │   │   ├── Editor/
│   │       │       │   │   │   │   ├── MarsGreyboxBuilder.cs
│   │       │       │   │   │   │   └── MarsGreyboxBuilder.cs.meta
│   │       │       │   │   │   ├── Materials/
│   │       │       │   │   │   │   ├── DarkRock.mat
│   │       │       │   │   │   │   ├── DarkRock.mat.meta
│   │       │       │   │   │   │   ├── HabitatPale.mat
│   │       │       │   │   │   │   ├── HabitatPale.mat.meta
│   │       │       │   │   │   │   ├── IndustrialGraphite.mat
│   │       │       │   │   │   │   ├── IndustrialGraphite.mat.meta
│   │       │       │   │   │   │   ├── LayeredStone.mat
│   │       │       │   │   │   │   ├── LayeredStone.mat.meta
│   │       │       │   │   │   │   ├── SandTan.mat
│   │       │       │   │   │   │   ├── SandTan.mat.meta
│   │       │       │   │   │   │   ├── TerrainOchre.mat
│   │       │       │   │   │   │   ├── TerrainOchre.mat.meta
│   │       │       │   │   │   │   ├── WayfindingAmber.mat
│   │       │       │   │   │   │   ├── WayfindingAmber.mat.meta
│   │       │       │   │   │   │   ├── WayfindingCyan.mat
│   │       │       │   │   │   │   └── WayfindingCyan.mat.meta
│   │       │       │   │   │   ├── Meshes/
│   │       │       │   │   │   │   ├── Ground.asset
│   │       │       │   │   │   │   └── Ground.asset.meta
│   │       │       │   │   │   ├── Editor.meta
│   │       │       │   │   │   ├── MarsOutpost_Greybox.unity
│   │       │       │   │   │   ├── MarsOutpost_Greybox.unity.meta
│   │       │       │   │   │   ├── Materials.meta
│   │       │       │   │   │   ├── Meshes.meta
│   │       │       │   │   │   ├── PreGreybox_Untitled_Backup.unity
│   │       │       │   │   │   └── PreGreybox_Untitled_Backup.unity.meta
│   │       │       │   │   ├── Integration/
│   │       │       │   │   │   ├── Contracts/
│   │       │       │   │   │   │   ├── IntegrationContracts.cs
│   │       │       │   │   │   │   └── IntegrationContracts.cs.meta
│   │       │       │   │   │   ├── Http/
│   │       │       │   │   │   │   ├── HttpMissionSessionService.cs
│   │       │       │   │   │   │   └── HttpMissionSessionService.cs.meta
│   │       │       │   │   │   ├── Local/
│   │       │       │   │   │   │   ├── GameplayEventBridge.cs
│   │       │       │   │   │   │   ├── GameplayEventBridge.cs.meta
│   │       │       │   │   │   │   ├── LifeSupportModelAdapter.cs
│   │       │       │   │   │   │   ├── LifeSupportModelAdapter.cs.meta
│   │       │       │   │   │   │   ├── LocalServices.cs
│   │       │       │   │   │   │   └── LocalServices.cs.meta
│   │       │       │   │   │   ├── Contracts.meta
│   │       │       │   │   │   ├── Http.meta
│   │       │       │   │   │   └── Local.meta
│   │       │       │   │   ├── MarsEnvironment/
│   │       │       │   │   │   ├── Documentation/
│   │       │       │   │   │   │   ├── MissionRegistry.asset
│   │       │       │   │   │   │   ├── MissionRegistry.asset.meta
│   │       │       │   │   │   │   ├── README.md
│   │       │       │   │   │   │   └── README.md.meta
│   │       │       │   │   │   ├── Lighting/
│   │       │       │   │   │   │   ├── Dusty_Conditions.asset
│   │       │       │   │   │   │   ├── Dusty_Conditions.asset.meta
│   │       │       │   │   │   │   ├── MarsGradient.shader
│   │       │       │   │   │   │   ├── MarsGradient.shader.meta
│   │       │       │   │   │   │   ├── MarsSky.mat
│   │       │       │   │   │   │   ├── MarsSky.mat.meta
│   │       │       │   │   │   │   ├── MarsVolume.asset
│   │       │       │   │   │   │   ├── MarsVolume.asset.meta
│   │       │       │   │   │   │   ├── Midday.asset
│   │       │       │   │   │   │   ├── Midday.asset.meta
│   │       │       │   │   │   │   ├── Morning.asset
│   │       │       │   │   │   │   ├── Morning.asset.meta
│   │       │       │   │   │   │   ├── Sunset.asset
│   │       │       │   │   │   │   └── Sunset.asset.meta
│   │       │       │   │   │   ├── Materials/
│   │       │       │   │   │   │   ├── DistantDust.asset
│   │       │       │   │   │   │   ├── DistantDust.asset.meta
│   │       │       │   │   │   │   ├── Dusty solar cells.mat
│   │       │       │   │   │   │   ├── Dusty solar cells.mat.meta
│   │       │       │   │   │   │   ├── EnvironmentDust.mat
│   │       │       │   │   │   │   ├── EnvironmentDust.mat.meta
│   │       │       │   │   │   │   ├── Fine Mars dust.mat
│   │       │       │   │   │   │   ├── Fine Mars dust.mat.meta
│   │       │       │   │   │   │   ├── Habitat ceramic composite.mat
│   │       │       │   │   │   │   ├── Habitat ceramic composite.mat.meta
│   │       │       │   │   │   │   ├── Observation panels — opaque approximation.mat
│   │       │       │   │   │   │   ├── Observation panels — opaque approximation.mat.meta
│   │       │       │   │   │   │   ├── Restrained identification ochre.mat
│   │       │       │   │   │   │   ├── Restrained identification ochre.mat.meta
│   │       │       │   │   │   │   ├── Rock07 dusty basalt.mat
│   │       │       │   │   │   │   ├── Rock07 dusty basalt.mat.meta
│   │       │       │   │   │   │   ├── Rock09 compact mineral.mat
│   │       │       │   │   │   │   ├── Rock09 compact mineral.mat.meta
│   │       │       │   │   │   │   ├── Rover_Cab.mat
│   │       │       │   │   │   │   ├── Rover_Cab.mat.meta
│   │       │       │   │   │   │   ├── Rover_Chassis.mat
│   │       │       │   │   │   │   ├── Rover_Chassis.mat.meta
│   │       │       │   │   │   │   ├── Rover_RoofMarker.mat
│   │       │       │   │   │   │   ├── Rover_RoofMarker.mat.meta
│   │       │       │   │   │   │   ├── Rover_Wheel_-1_0.mat
│   │       │       │   │   │   │   ├── Rover_Wheel_-1_0.mat.meta
│   │       │       │   │   │   │   ├── Rover_Wheel_-1_1.mat
│   │       │       │   │   │   │   ├── Rover_Wheel_-1_1.mat.meta
│   │       │       │   │   │   │   ├── Rover_Wheel_-1_2.mat
│   │       │       │   │   │   │   ├── Rover_Wheel_-1_2.mat.meta
│   │       │       │   │   │   │   ├── Rover_Wheel_1_0.mat
│   │       │       │   │   │   │   ├── Rover_Wheel_1_0.mat.meta
│   │       │       │   │   │   │   ├── Rover_Wheel_1_1.mat
│   │       │       │   │   │   │   ├── Rover_Wheel_1_1.mat.meta
│   │       │       │   │   │   │   ├── Rover_Wheel_1_2.mat
│   │       │       │   │   │   │   ├── Rover_Wheel_1_2.mat.meta
│   │       │       │   │   │   │   ├── Structural graphite.mat
│   │       │       │   │   │   │   └── Structural graphite.mat.meta
│   │       │       │   │   │   ├── Models/
│   │       │       │   │   │   │   ├── Crew_Dome_Ribs_LowPoly.asset
│   │       │       │   │   │   │   ├── Crew_Dome_Ribs_LowPoly.asset.meta
│   │       │       │   │   │   │   ├── HabitatHemisphere.asset
│   │       │       │   │   │   │   ├── HabitatHemisphere.asset.meta
│   │       │       │   │   │   │   ├── Laboratory_Dome_Ribs_LowPoly.asset
│   │       │       │   │   │   │   ├── Laboratory_Dome_Ribs_LowPoly.asset.meta
│   │       │       │   │   │   │   ├── Life_Support_Dome_Ribs_LowPoly.asset
│   │       │       │   │   │   │   ├── Life_Support_Dome_Ribs_LowPoly.asset.meta
│   │       │       │   │   │   │   ├── rock_07_LOD0.asset
│   │       │       │   │   │   │   ├── rock_07_LOD0.asset.meta
│   │       │       │   │   │   │   ├── rock_07_LOD1.asset
│   │       │       │   │   │   │   ├── rock_07_LOD1.asset.meta
│   │       │       │   │   │   │   ├── rock_07_LOD2.asset
│   │       │       │   │   │   │   ├── rock_07_LOD2.asset.meta
│   │       │       │   │   │   │   ├── rock_09_LOD0.asset
│   │       │       │   │   │   │   ├── rock_09_LOD0.asset.meta
│   │       │       │   │   │   │   ├── rock_09_LOD1.asset
│   │       │       │   │   │   │   ├── rock_09_LOD1.asset.meta
│   │       │       │   │   │   │   ├── rock_09_LOD2.asset
│   │       │       │   │   │   │   ├── rock_09_LOD2.asset.meta
│   │       │       │   │   │   │   ├── Terrain250m.asset
│   │       │       │   │   │   │   └── Terrain250m.asset.meta
│   │       │       │   │   │   ├── Prefabs/
│   │       │       │   │   │   │   ├── ARESYN_ClosedHabitatComplex_Optimized.prefab
│   │       │       │   │   │   │   ├── ARESYN_ClosedHabitatComplex_Optimized.prefab.meta
│   │       │       │   │   │   │   ├── SolarFarm_Optimized.prefab
│   │       │       │   │   │   │   └── SolarFarm_Optimized.prefab.meta
│   │       │       │   │   │   ├── Scenes/
│   │       │       │   │   │   │   ├── Mars_RealisticResearchBase_v1.unity
│   │       │       │   │   │   │   └── Mars_RealisticResearchBase_v1.unity.meta
│   │       │       │   │   │   ├── Scripts/
│   │       │       │   │   │   │   ├── EnvironmentPresentation.cs
│   │       │       │   │   │   │   ├── EnvironmentPresentation.cs.meta
│   │       │       │   │   │   │   ├── EnvironmentPreset.cs
│   │       │       │   │   │   │   ├── EnvironmentPreset.cs.meta
│   │       │       │   │   │   │   ├── ResearchTarget.cs
│   │       │       │   │   │   │   └── ResearchTarget.cs.meta
│   │       │       │   │   │   ├── Tests/
│   │       │       │   │   │   │   ├── Editor/
│   │       │       │   │   │   │   │   ├── MarsResearchBaseBuilder.cs
│   │       │       │   │   │   │   │   └── MarsResearchBaseBuilder.cs.meta
│   │       │       │   │   │   │   └── Editor.meta
│   │       │       │   │   │   ├── Textures/
│   │       │       │   │   │   │   ├── rock_07_AO.png
│   │       │       │   │   │   │   ├── rock_07_AO.png.meta
│   │       │       │   │   │   │   ├── rock_07_Diffuse.png
│   │       │       │   │   │   │   ├── rock_07_Diffuse.png.meta
│   │       │       │   │   │   │   ├── rock_07_nor_gl.png
│   │       │       │   │   │   │   ├── rock_07_nor_gl.png.meta
│   │       │       │   │   │   │   ├── rock_07_Rough.png
│   │       │       │   │   │   │   ├── rock_07_Rough.png.meta
│   │       │       │   │   │   │   ├── rock_09_AO.png
│   │       │       │   │   │   │   ├── rock_09_AO.png.meta
│   │       │       │   │   │   │   ├── rock_09_Diffuse.png
│   │       │       │   │   │   │   ├── rock_09_Diffuse.png.meta
│   │       │       │   │   │   │   ├── rock_09_nor_gl.png
│   │       │       │   │   │   │   ├── rock_09_nor_gl.png.meta
│   │       │       │   │   │   │   ├── rock_09_Rough.png
│   │       │       │   │   │   │   ├── rock_09_Rough.png.meta
│   │       │       │   │   │   │   ├── sandy_gravel_AO.png
│   │       │       │   │   │   │   ├── sandy_gravel_AO.png.meta
│   │       │       │   │   │   │   ├── sandy_gravel_Diffuse.png
│   │       │       │   │   │   │   ├── sandy_gravel_Diffuse.png.meta
│   │       │       │   │   │   │   ├── sandy_gravel_nor_gl.png
│   │       │       │   │   │   │   ├── sandy_gravel_nor_gl.png.meta
│   │       │       │   │   │   │   ├── sandy_gravel_Rough.png
│   │       │       │   │   │   │   └── sandy_gravel_Rough.png.meta
│   │       │       │   │   │   ├── Documentation.meta
│   │       │       │   │   │   ├── Lighting.meta
│   │       │       │   │   │   ├── Materials.meta
│   │       │       │   │   │   ├── Models.meta
│   │       │       │   │   │   ├── Prefabs.meta
│   │       │       │   │   │   ├── Scenes.meta
│   │       │       │   │   │   ├── Scripts.meta
│   │       │       │   │   │   ├── Tests.meta
│   │       │       │   │   │   ├── Textures.meta
│   │       │       │   │   │   └── VFX.meta
│   │       │       │   │   ├── MarsVisualPolish/
│   │       │       │   │   │   ├── Documentation/
│   │       │       │   │   │   │   ├── PolishRegistry.asset
│   │       │       │   │   │   │   └── PolishRegistry.asset.meta
│   │       │       │   │   │   ├── Editor/
│   │       │       │   │   │   │   ├── VisualPolishBuilder.cs
│   │       │       │   │   │   │   └── VisualPolishBuilder.cs.meta
│   │       │       │   │   │   ├── Lighting/
│   │       │       │   │   │   │   ├── PolishSky.mat
│   │       │       │   │   │   │   ├── PolishSky.mat.meta
│   │       │       │   │   │   │   ├── PolishVolume.asset
│   │       │       │   │   │   │   ├── PolishVolume.asset.meta
│   │       │       │   │   │   │   ├── Preset_0.asset
│   │       │       │   │   │   │   ├── Preset_0.asset.meta
│   │       │       │   │   │   │   ├── Preset_1.asset
│   │       │       │   │   │   │   ├── Preset_1.asset.meta
│   │       │       │   │   │   │   ├── Preset_2.asset
│   │       │       │   │   │   │   ├── Preset_2.asset.meta
│   │       │       │   │   │   │   ├── Preset_3.asset
│   │       │       │   │   │   │   └── Preset_3.asset.meta
│   │       │       │   │   │   ├── Materials/
│   │       │       │   │   │   │   ├── Brushed frame.mat
│   │       │       │   │   │   │   ├── Brushed frame.mat.meta
│   │       │       │   │   │   │   ├── Derivative_0.mat
│   │       │       │   │   │   │   ├── Derivative_0.mat.meta
│   │       │       │   │   │   │   ├── Derivative_1.mat
│   │       │       │   │   │   │   ├── Derivative_1.mat.meta
│   │       │       │   │   │   │   ├── Derivative_10.mat
│   │       │       │   │   │   │   ├── Derivative_10.mat.meta
│   │       │       │   │   │   │   ├── Derivative_11.mat
│   │       │       │   │   │   │   ├── Derivative_11.mat.meta
│   │       │       │   │   │   │   ├── Derivative_12.mat
│   │       │       │   │   │   │   ├── Derivative_12.mat.meta
│   │       │       │   │   │   │   ├── Derivative_13.mat
│   │       │       │   │   │   │   ├── Derivative_13.mat.meta
│   │       │       │   │   │   │   ├── Derivative_14.mat
│   │       │       │   │   │   │   ├── Derivative_14.mat.meta
│   │       │       │   │   │   │   ├── Derivative_15.mat
│   │       │       │   │   │   │   ├── Derivative_15.mat.meta
│   │       │       │   │   │   │   ├── Derivative_16.mat
│   │       │       │   │   │   │   ├── Derivative_16.mat.meta
│   │       │       │   │   │   │   ├── Derivative_17.mat
│   │       │       │   │   │   │   ├── Derivative_17.mat.meta
│   │       │       │   │   │   │   ├── Derivative_18.mat
│   │       │       │   │   │   │   ├── Derivative_18.mat.meta
│   │       │       │   │   │   │   ├── Derivative_19.mat
│   │       │       │   │   │   │   ├── Derivative_19.mat.meta
│   │       │       │   │   │   │   ├── Derivative_2.mat
│   │       │       │   │   │   │   ├── Derivative_2.mat.meta
│   │       │       │   │   │   │   ├── Derivative_20.mat
│   │       │       │   │   │   │   ├── Derivative_20.mat.meta
│   │       │       │   │   │   │   ├── Derivative_21.mat
│   │       │       │   │   │   │   ├── Derivative_21.mat.meta
│   │       │       │   │   │   │   ├── Derivative_3.mat
│   │       │       │   │   │   │   ├── Derivative_3.mat.meta
│   │       │       │   │   │   │   ├── Derivative_4.mat
│   │       │       │   │   │   │   ├── Derivative_4.mat.meta
│   │       │       │   │   │   │   ├── Derivative_5.mat
│   │       │       │   │   │   │   ├── Derivative_5.mat.meta
│   │       │       │   │   │   │   ├── Derivative_6.mat
│   │       │       │   │   │   │   ├── Derivative_6.mat.meta
│   │       │       │   │   │   │   ├── Derivative_7.mat
│   │       │       │   │   │   │   ├── Derivative_7.mat.meta
│   │       │       │   │   │   │   ├── Derivative_8.mat
│   │       │       │   │   │   │   ├── Derivative_8.mat.meta
│   │       │       │   │   │   │   ├── Derivative_9.mat
│   │       │       │   │   │   │   ├── Derivative_9.mat.meta
│   │       │       │   │   │   │   ├── Photovoltaic cell laminate.mat
│   │       │       │   │   │   │   ├── Photovoltaic cell laminate.mat.meta
│   │       │       │   │   │   │   ├── PhotovoltaicGrid.png
│   │       │       │   │   │   │   ├── PhotovoltaicGrid.png.meta
│   │       │       │   │   │   │   ├── Recessed seals.mat
│   │       │       │   │   │   │   ├── Recessed seals.mat.meta
│   │       │       │   │   │   │   ├── Safety ochre.mat
│   │       │       │   │   │   │   ├── Safety ochre.mat.meta
│   │       │       │   │   │   │   ├── Warm ceramic.mat
│   │       │       │   │   │   │   └── Warm ceramic.mat.meta
│   │       │       │   │   │   ├── Models/
│   │       │       │   │   │   │   ├── Crew Dome_Frame.asset
│   │       │       │   │   │   │   ├── Crew Dome_Frame.asset.meta
│   │       │       │   │   │   │   ├── Laboratory Dome_Frame.asset
│   │       │       │   │   │   │   ├── Laboratory Dome_Frame.asset.meta
│   │       │       │   │   │   │   ├── Life Support Dome_Frame.asset
│   │       │       │   │   │   │   └── Life Support Dome_Frame.asset.meta
│   │       │       │   │   │   ├── Scenes/
│   │       │       │   │   │   │   ├── Mars_VisualPolish_v2.unity
│   │       │       │   │   │   │   └── Mars_VisualPolish_v2.unity.meta
│   │       │       │   │   │   ├── Documentation.meta
│   │       │       │   │   │   ├── Editor.meta
│   │       │       │   │   │   ├── Lighting.meta
│   │       │       │   │   │   ├── Materials.meta
│   │       │       │   │   │   ├── Models.meta
│   │       │       │   │   │   ├── PolishBenchmark.cs
│   │       │       │   │   │   ├── PolishBenchmark.cs.meta
│   │       │       │   │   │   ├── PolishHudTheme.cs
│   │       │       │   │   │   ├── PolishHudTheme.cs.meta
│   │       │       │   │   │   ├── Scenes.meta
│   │       │       │   │   │   ├── VisualPolishPresentation.cs
│   │       │       │   │   │   └── VisualPolishPresentation.cs.meta
│   │       │       │   │   ├── Mission01Milestone/
│   │       │       │   │   │   ├── Editor/
│   │       │       │   │   │   │   ├── Mission01Builder.cs
│   │       │       │   │   │   │   └── Mission01Builder.cs.meta
│   │       │       │   │   │   ├── Materials/
│   │       │       │   │   │   │   ├── Mission01_Continents.mat
│   │       │       │   │   │   │   ├── Mission01_Continents.mat.meta
│   │       │       │   │   │   │   ├── Mission01_Graphite.mat
│   │       │       │   │   │   │   ├── Mission01_Graphite.mat.meta
│   │       │       │   │   │   │   ├── Mission01_Ocean.mat
│   │       │       │   │   │   │   ├── Mission01_Ocean.mat.meta
│   │       │       │   │   │   │   ├── Mission01_Pale.mat
│   │       │       │   │   │   │   ├── Mission01_Pale.mat.meta
│   │       │       │   │   │   │   ├── Mission01_Safety.mat
│   │       │       │   │   │   │   └── Mission01_Safety.mat.meta
│   │       │       │   │   │   ├── Prefabs/
│   │       │       │   │   │   │   ├── Boarding_Console.prefab
│   │       │       │   │   │   │   ├── Boarding_Console.prefab.meta
│   │       │       │   │   │   │   ├── Trainee_Blockout.prefab
│   │       │       │   │   │   │   └── Trainee_Blockout.prefab.meta
│   │       │       │   │   │   ├── Editor.meta
│   │       │       │   │   │   ├── Materials.meta
│   │       │       │   │   │   ├── Mission01_EarthLaunch_Milestone_v4.unity
│   │       │       │   │   │   ├── Mission01_EarthLaunch_Milestone_v4.unity.meta
│   │       │       │   │   │   ├── Mission01Flow.cs
│   │       │       │   │   │   ├── Mission01Flow.cs.meta
│   │       │       │   │   │   └── Prefabs.meta
│   │       │       │   │   ├── MissionIntegration/
│   │       │       │   │   │   ├── Editor/
│   │       │       │   │   │   │   ├── MissionIntegrationBuilder.cs
│   │       │       │   │   │   │   └── MissionIntegrationBuilder.cs.meta
│   │       │       │   │   │   ├── Scenes/
│   │       │       │   │   │   │   ├── Mars_MissionIntegration_v1.unity
│   │       │       │   │   │   │   └── Mars_MissionIntegration_v1.unity.meta
│   │       │       │   │   │   ├── Scripts/
│   │       │       │   │   │   │   ├── MissionIntegrationController.cs
│   │       │       │   │   │   │   └── MissionIntegrationController.cs.meta
│   │       │       │   │   │   ├── Editor.meta
│   │       │       │   │   │   ├── Scenes.meta
│   │       │       │   │   │   └── Scripts.meta
│   │       │       │   │   ├── MissionRules/
│   │       │       │   │   │   ├── Config/
│   │       │       │   │   │   │   ├── mission_rules_v1.json
│   │       │       │   │   │   │   ├── mission_rules_v1.json.meta
│   │       │       │   │   │   │   ├── test_vectors_v1.json
│   │       │       │   │   │   │   └── test_vectors_v1.json.meta
│   │       │       │   │   │   ├── Scripts/
│   │       │       │   │   │   │   ├── MiniJson.cs
│   │       │       │   │   │   │   ├── MiniJson.cs.meta
│   │       │       │   │   │   │   ├── MissionRules.cs
│   │       │       │   │   │   │   └── MissionRules.cs.meta
│   │       │       │   │   │   ├── Config.meta
│   │       │       │   │   │   └── Scripts.meta
│   │       │       │   │   ├── NativeControlSlice/
│   │       │       │   │   │   ├── Editor/
│   │       │       │   │   │   │   ├── NativeSliceBuilder.cs
│   │       │       │   │   │   │   └── NativeSliceBuilder.cs.meta
│   │       │       │   │   │   ├── Prefabs/
│   │       │       │   │   │   │   ├── AdaptiveTouch.prefab
│   │       │       │   │   │   │   └── AdaptiveTouch.prefab.meta
│   │       │       │   │   │   ├── AdaptiveTouch.cs
│   │       │       │   │   │   ├── AdaptiveTouch.cs.meta
│   │       │       │   │   │   ├── Editor.meta
│   │       │       │   │   │   ├── Mars_NativeControl_VerticalSlice.unity
│   │       │       │   │   │   ├── Mars_NativeControl_VerticalSlice.unity.meta
│   │       │       │   │   │   ├── MissionRegistry.asset
│   │       │       │   │   │   ├── MissionRegistry.asset.meta
│   │       │       │   │   │   ├── NativeSliceGame.cs
│   │       │       │   │   │   ├── NativeSliceGame.cs.meta
│   │       │       │   │   │   ├── NativeSystems.cs
│   │       │       │   │   │   ├── NativeSystems.cs.meta
│   │       │       │   │   │   └── Prefabs.meta
│   │       │       │   │   ├── ProductionVisual/
│   │       │       │   │   │   ├── Materials/
│   │       │       │   │   │   │   ├── Brushed aluminum.mat
│   │       │       │   │   │   │   ├── Brushed aluminum.mat.meta
│   │       │       │   │   │   │   ├── Dusty tire elastomer.mat
│   │       │       │   │   │   │   ├── Dusty tire elastomer.mat.meta
│   │       │       │   │   │   │   ├── Inherited_0.mat
│   │       │       │   │   │   │   ├── Inherited_0.mat.meta
│   │       │       │   │   │   │   ├── Inherited_1.mat
│   │       │       │   │   │   │   ├── Inherited_1.mat.meta
│   │       │       │   │   │   │   ├── Inherited_10.mat
│   │       │       │   │   │   │   ├── Inherited_10.mat.meta
│   │       │       │   │   │   │   ├── Inherited_11.mat
│   │       │       │   │   │   │   ├── Inherited_11.mat.meta
│   │       │       │   │   │   │   ├── Inherited_12.mat
│   │       │       │   │   │   │   ├── Inherited_12.mat.meta
│   │       │       │   │   │   │   ├── Inherited_13.mat
│   │       │       │   │   │   │   ├── Inherited_13.mat.meta
│   │       │       │   │   │   │   ├── Inherited_14.mat
│   │       │       │   │   │   │   ├── Inherited_14.mat.meta
│   │       │       │   │   │   │   ├── Inherited_15.mat
│   │       │       │   │   │   │   ├── Inherited_15.mat.meta
│   │       │       │   │   │   │   ├── Inherited_16.mat
│   │       │       │   │   │   │   ├── Inherited_16.mat.meta
│   │       │       │   │   │   │   ├── Inherited_17.mat
│   │       │       │   │   │   │   ├── Inherited_17.mat.meta
│   │       │       │   │   │   │   ├── Inherited_18.mat
│   │       │       │   │   │   │   ├── Inherited_18.mat.meta
│   │       │       │   │   │   │   ├── Inherited_19.mat
│   │       │       │   │   │   │   ├── Inherited_19.mat.meta
│   │       │       │   │   │   │   ├── Inherited_2.mat
│   │       │       │   │   │   │   ├── Inherited_2.mat.meta
│   │       │       │   │   │   │   ├── Inherited_20.mat
│   │       │       │   │   │   │   ├── Inherited_20.mat.meta
│   │       │       │   │   │   │   ├── Inherited_21.mat
│   │       │       │   │   │   │   ├── Inherited_21.mat.meta
│   │       │       │   │   │   │   ├── Inherited_22.mat
│   │       │       │   │   │   │   ├── Inherited_22.mat.meta
│   │       │       │   │   │   │   ├── Inherited_23.mat
│   │       │       │   │   │   │   ├── Inherited_23.mat.meta
│   │       │       │   │   │   │   ├── Inherited_24.mat
│   │       │       │   │   │   │   ├── Inherited_24.mat.meta
│   │       │       │   │   │   │   ├── Inherited_25.mat
│   │       │       │   │   │   │   ├── Inherited_25.mat.meta
│   │       │       │   │   │   │   ├── Inherited_3.mat
│   │       │       │   │   │   │   ├── Inherited_3.mat.meta
│   │       │       │   │   │   │   ├── Inherited_4.mat
│   │       │       │   │   │   │   ├── Inherited_4.mat.meta
│   │       │       │   │   │   │   ├── Inherited_5.mat
│   │       │       │   │   │   │   ├── Inherited_5.mat.meta
│   │       │       │   │   │   │   ├── Inherited_6.mat
│   │       │       │   │   │   │   ├── Inherited_6.mat.meta
│   │       │       │   │   │   │   ├── Inherited_7.mat
│   │       │       │   │   │   │   ├── Inherited_7.mat.meta
│   │       │       │   │   │   │   ├── Inherited_8.mat
│   │       │       │   │   │   │   ├── Inherited_8.mat.meta
│   │       │       │   │   │   │   ├── Inherited_9.mat
│   │       │       │   │   │   │   ├── Inherited_9.mat.meta
│   │       │       │   │   │   │   ├── Machined graphite alloy.mat
│   │       │       │   │   │   │   ├── Machined graphite alloy.mat.meta
│   │       │       │   │   │   │   ├── Opaque protective visor glazing.mat
│   │       │       │   │   │   │   ├── Opaque protective visor glazing.mat.meta
│   │       │       │   │   │   │   ├── Pressure shell ceramic composite.mat
│   │       │       │   │   │   │   ├── Pressure shell ceramic composite.mat.meta
│   │       │       │   │   │   │   ├── Restrained identification orange.mat
│   │       │       │   │   │   │   └── Restrained identification orange.mat.meta
│   │       │       │   │   │   ├── Meshes/
│   │       │       │   │   │   │   ├── ChamferedEnclosure.asset
│   │       │       │   │   │   │   ├── ChamferedEnclosure.asset.meta
│   │       │       │   │   │   │   ├── CommsTruss.asset
│   │       │       │   │   │   │   ├── CommsTruss.asset.meta
│   │       │       │   │   │   │   ├── Crew_Dome_sealed_airlock_Rails.asset
│   │       │       │   │   │   │   ├── Crew_Dome_sealed_airlock_Rails.asset.meta
│   │       │       │   │   │   │   ├── CrewCabEnclosure.asset
│   │       │       │   │   │   │   ├── CrewCabEnclosure.asset.meta
│   │       │       │   │   │   │   ├── GeodesicFrame.asset
│   │       │       │   │   │   │   ├── GeodesicFrame.asset.meta
│   │       │       │   │   │   │   ├── GeodesicPanels.asset
│   │       │       │   │   │   │   ├── GeodesicPanels.asset.meta
│   │       │       │   │   │   │   ├── Laboratory_Dome_sealed_airlock_Rails.asset
│   │       │       │   │   │   │   ├── Laboratory_Dome_sealed_airlock_Rails.asset.meta
│   │       │       │   │   │   │   ├── Life_Support_Dome_sealed_airlock_Rails.asset
│   │       │       │   │   │   │   ├── Life_Support_Dome_sealed_airlock_Rails.asset.meta
│   │       │       │   │   │   │   ├── RoverMechanicalFrame.asset
│   │       │       │   │   │   │   ├── RoverMechanicalFrame.asset.meta
│   │       │       │   │   │   │   ├── RoverWheel_LOD0.asset
│   │       │       │   │   │   │   ├── RoverWheel_LOD0.asset.meta
│   │       │       │   │   │   │   ├── RoverWheel_LOD1.asset
│   │       │       │   │   │   │   ├── RoverWheel_LOD1.asset.meta
│   │       │       │   │   │   │   ├── RoverWheel_LOD2.asset
│   │       │       │   │   │   │   └── RoverWheel_LOD2.asset.meta
│   │       │       │   │   │   ├── Scenes/
│   │       │       │   │   │   │   ├── Mars_ProductionVisual_v1.unity
│   │       │       │   │   │   │   └── Mars_ProductionVisual_v1.unity.meta
│   │       │       │   │   │   ├── Scripts/
│   │       │       │   │   │   │   ├── ProductionCameraRig.cs
│   │       │       │   │   │   │   ├── ProductionCameraRig.cs.meta
│   │       │       │   │   │   │   ├── ProductionModeCoordinator.cs
│   │       │       │   │   │   │   ├── ProductionModeCoordinator.cs.meta
│   │       │       │   │   │   │   ├── ProductionPerformanceProbe.cs
│   │       │       │   │   │   │   ├── ProductionPerformanceProbe.cs.meta
│   │       │       │   │   │   │   ├── ProductionPresentation.cs
│   │       │       │   │   │   │   └── ProductionPresentation.cs.meta
│   │       │       │   │   │   ├── Settings/
│   │       │       │   │   │   │   ├── Lighting_0.asset
│   │       │       │   │   │   │   ├── Lighting_0.asset.meta
│   │       │       │   │   │   │   ├── Lighting_1.asset
│   │       │       │   │   │   │   ├── Lighting_1.asset.meta
│   │       │       │   │   │   │   ├── Lighting_2.asset
│   │       │       │   │   │   │   ├── Lighting_2.asset.meta
│   │       │       │   │   │   │   ├── Lighting_3.asset
│   │       │       │   │   │   │   ├── Lighting_3.asset.meta
│   │       │       │   │   │   │   ├── ProductionMissionRegistry.asset
│   │       │       │   │   │   │   └── ProductionMissionRegistry.asset.meta
│   │       │       │   │   │   ├── Tests/
│   │       │       │   │   │   │   ├── Editor/
│   │       │       │   │   │   │   │   ├── ProductionMeshFactory.cs
│   │       │       │   │   │   │   │   ├── ProductionMeshFactory.cs.meta
│   │       │       │   │   │   │   │   ├── ProductionViewport.cs
│   │       │       │   │   │   │   │   ├── ProductionViewport.cs.meta
│   │       │       │   │   │   │   │   ├── ProductionVisualBuilder.cs
│   │       │       │   │   │   │   │   └── ProductionVisualBuilder.cs.meta
│   │       │       │   │   │   │   └── Editor.meta
│   │       │       │   │   │   ├── Textures/
│   │       │       │   │   │   │   ├── rock_07_MetallicSmoothness.png
│   │       │       │   │   │   │   ├── rock_07_MetallicSmoothness.png.meta
│   │       │       │   │   │   │   ├── rock_09_MetallicSmoothness.png
│   │       │       │   │   │   │   ├── rock_09_MetallicSmoothness.png.meta
│   │       │       │   │   │   │   ├── sandy_gravel_MetallicSmoothness.png
│   │       │       │   │   │   │   └── sandy_gravel_MetallicSmoothness.png.meta
│   │       │       │   │   │   ├── Themes/
│   │       │       │   │   │   │   ├── ProductionHUDTheme.asset
│   │       │       │   │   │   │   ├── ProductionHUDTheme.asset.meta
│   │       │       │   │   │   │   ├── ProductionSky.mat
│   │       │       │   │   │   │   ├── ProductionSky.mat.meta
│   │       │       │   │   │   │   ├── ProductionVolume.asset
│   │       │       │   │   │   │   └── ProductionVolume.asset.meta
│   │       │       │   │   │   ├── Documentation.meta
│   │       │       │   │   │   ├── Materials.meta
│   │       │       │   │   │   ├── Meshes.meta
│   │       │       │   │   │   ├── Prefabs.meta
│   │       │       │   │   │   ├── Scenes.meta
│   │       │       │   │   │   ├── Scripts.meta
│   │       │       │   │   │   ├── Settings.meta
│   │       │       │   │   │   ├── Tests.meta
│   │       │       │   │   │   ├── Textures.meta
│   │       │       │   │   │   └── Themes.meta
│   │       │       │   │   ├── Scenes/
│   │       │       │   │   │   ├── MarsOutpost_Environment_v01.unity
│   │       │       │   │   │   └── MarsOutpost_Environment_v01.unity.meta
│   │       │       │   │   ├── Stage08B/
│   │       │       │   │   │   ├── Animation/
│   │       │       │   │   │   │   ├── Idle.anim
│   │       │       │   │   │   │   ├── Idle.anim.meta
│   │       │       │   │   │   │   ├── PrototypeLocomotion.controller
│   │       │       │   │   │   │   ├── PrototypeLocomotion.controller.meta
│   │       │       │   │   │   │   ├── Run.anim
│   │       │       │   │   │   │   ├── Run.anim.meta
│   │       │       │   │   │   │   ├── Walk.anim
│   │       │       │   │   │   │   └── Walk.anim.meta
│   │       │       │   │   │   ├── Editor/
│   │       │       │   │   │   │   ├── Stage08BBuilder.cs
│   │       │       │   │   │   │   └── Stage08BBuilder.cs.meta
│   │       │       │   │   │   ├── Materials/
│   │       │       │   │   │   │   ├── 08B_Accent_Cyan.mat
│   │       │       │   │   │   │   ├── 08B_Accent_Cyan.mat.meta
│   │       │       │   │   │   │   ├── 08B_Accent_SafetyOrange.mat
│   │       │       │   │   │   │   ├── 08B_Accent_SafetyOrange.mat.meta
│   │       │       │   │   │   │   ├── 08B_Glass_Utility.mat
│   │       │       │   │   │   │   ├── 08B_Glass_Utility.mat.meta
│   │       │       │   │   │   │   ├── 08B_Habitat_CompositeDark.mat
│   │       │       │   │   │   │   ├── 08B_Habitat_CompositeDark.mat.meta
│   │       │       │   │   │   │   ├── 08B_Habitat_OffWhite.mat
│   │       │       │   │   │   │   ├── 08B_Habitat_OffWhite.mat.meta
│   │       │       │   │   │   │   ├── 08B_Mars_Regolith.mat
│   │       │       │   │   │   │   ├── 08B_Mars_Regolith.mat.meta
│   │       │       │   │   │   │   ├── 08B_Mars_Regolith_Dark.mat
│   │       │       │   │   │   │   ├── 08B_Mars_Regolith_Dark.mat.meta
│   │       │       │   │   │   │   ├── 08B_Mars_Rock_Basaltic.mat
│   │       │       │   │   │   │   ├── 08B_Mars_Rock_Basaltic.mat.meta
│   │       │       │   │   │   │   ├── 08B_Mars_Rock_Layered.mat
│   │       │       │   │   │   │   ├── 08B_Mars_Rock_Layered.mat.meta
│   │       │       │   │   │   │   ├── 08B_Metal_Industrial.mat
│   │       │       │   │   │   │   ├── 08B_Metal_Industrial.mat.meta
│   │       │       │   │   │   │   ├── 08B_SolarPanel.mat
│   │       │       │   │   │   │   ├── 08B_SolarPanel.mat.meta
│   │       │       │   │   │   │   ├── BasaltVariationA.mat
│   │       │       │   │   │   │   ├── BasaltVariationA.mat.meta
│   │       │       │   │   │   │   ├── BasaltVariationB.mat
│   │       │       │   │   │   │   ├── BasaltVariationB.mat.meta
│   │       │       │   │   │   │   ├── BasaltVariationC.mat
│   │       │       │   │   │   │   ├── BasaltVariationC.mat.meta
│   │       │       │   │   │   │   ├── Dust_Particle.mat
│   │       │       │   │   │   │   ├── Dust_Particle.mat.meta
│   │       │       │   │   │   │   ├── Identification_Cyan.mat
│   │       │       │   │   │   │   ├── Identification_Cyan.mat.meta
│   │       │       │   │   │   │   ├── Safety_Orange.mat
│   │       │       │   │   │   │   ├── Safety_Orange.mat.meta
│   │       │       │   │   │   │   ├── Suit_Graphite.mat
│   │       │       │   │   │   │   ├── Suit_Graphite.mat.meta
│   │       │       │   │   │   │   ├── Suit_Pale.mat
│   │       │       │   │   │   │   └── Suit_Pale.mat.meta
│   │       │       │   │   │   ├── Animation.meta
│   │       │       │   │   │   ├── Editor.meta
│   │       │       │   │   │   ├── MarsOutpost_Playable_Stage08B.unity
│   │       │       │   │   │   ├── MarsOutpost_Playable_Stage08B.unity.meta
│   │       │       │   │   │   ├── MarsPrototype.cs
│   │       │       │   │   │   ├── MarsPrototype.cs.meta
│   │       │       │   │   │   ├── Materials.meta
│   │       │       │   │   │   ├── Stage08B_MobilePreview.asset
│   │       │       │   │   │   └── Stage08B_MobilePreview.asset.meta
│   │       │       │   │   ├── Stage08C/
│   │       │       │   │   │   ├── Editor/
│   │       │       │   │   │   │   ├── AndroidProbeBuild.cs
│   │       │       │   │   │   │   ├── AndroidProbeBuild.cs.meta
│   │       │       │   │   │   │   ├── TouchChecks.cs
│   │       │       │   │   │   │   └── TouchChecks.cs.meta
│   │       │       │   │   │   ├── AndroidProfileProbe.cs
│   │       │       │   │   │   ├── AndroidProfileProbe.cs.meta
│   │       │       │   │   │   ├── Editor.meta
│   │       │       │   │   │   ├── MobileControls.cs
│   │       │       │   │   │   └── MobileControls.cs.meta
│   │       │       │   │   ├── Stage08C1/
│   │       │       │   │   │   ├── Editor/
│   │       │       │   │   │   │   ├── EditorPerformanceObserver.cs
│   │       │       │   │   │   │   ├── EditorPerformanceObserver.cs.meta
│   │       │       │   │   │   │   ├── ManualInputObserver.cs
│   │       │       │   │   │   │   └── ManualInputObserver.cs.meta
│   │       │       │   │   │   └── Editor.meta
│   │       │       │   │   ├── Verification/
│   │       │       │   │   │   ├── Editor/
│   │       │       │   │   │   │   ├── LaptopPlaytest.cs
│   │       │       │   │   │   │   ├── LaptopPlaytest.cs.meta
│   │       │       │   │   │   │   ├── Step1GameplayVerification.cs
│   │       │       │   │   │   │   ├── Step1GameplayVerification.cs.meta
│   │       │       │   │   │   │   ├── Step3BackendClientTest.cs
│   │       │       │   │   │   │   ├── Step3BackendClientTest.cs.meta
│   │       │       │   │   │   │   ├── Step4RulesTest.cs
│   │       │       │   │   │   │   └── Step4RulesTest.cs.meta
│   │       │       │   │   │   └── Editor.meta
│   │       │       │   │   ├── CinematicHUD.meta
│   │       │       │   │   ├── Environment.meta
│   │       │       │   │   ├── Greybox.meta
│   │       │       │   │   ├── Integration.meta
│   │       │       │   │   ├── MarsEnvironment.meta
│   │       │       │   │   ├── MarsVisualPolish.meta
│   │       │       │   │   ├── Mission01Milestone.meta
│   │       │       │   │   ├── MissionIntegration.meta
│   │       │       │   │   ├── MissionRules.meta
│   │       │       │   │   ├── NativeControlSlice.meta
│   │       │       │   │   ├── ProductionVisual.meta
│   │       │       │   │   ├── Scenes.meta
│   │       │       │   │   ├── Stage08B.meta
│   │       │       │   │   ├── Stage08C.meta
│   │       │       │   │   ├── Stage08C1.meta
│   │       │       │   │   └── Verification.meta
│   │       │       │   ├── LaptopBuildOnly/
│   │       │       │   │   ├── Editor/
│   │       │       │   │   │   ├── LaptopMacBuild.cs
│   │       │       │   │   │   └── LaptopMacBuild.cs.meta
│   │       │       │   │   └── Editor.meta
│   │       │       │   ├── Scenes/
│   │       │       │   │   ├── SampleScene.unity
│   │       │       │   │   └── SampleScene.unity.meta
│   │       │       │   ├── Settings/
│   │       │       │   │   ├── DefaultVolumeProfile.asset
│   │       │       │   │   ├── DefaultVolumeProfile.asset.meta
│   │       │       │   │   ├── Mobile_Renderer.asset
│   │       │       │   │   ├── Mobile_Renderer.asset.meta
│   │       │       │   │   ├── Mobile_RPAsset.asset
│   │       │       │   │   ├── Mobile_RPAsset.asset.meta
│   │       │       │   │   ├── PC_Renderer.asset
│   │       │       │   │   ├── PC_Renderer.asset.meta
│   │       │       │   │   ├── PC_RPAsset.asset
│   │       │       │   │   ├── PC_RPAsset.asset.meta
│   │       │       │   │   ├── SampleSceneProfile.asset
│   │       │       │   │   ├── SampleSceneProfile.asset.meta
│   │       │       │   │   ├── UniversalRenderPipelineGlobalSettings.asset
│   │       │       │   │   └── UniversalRenderPipelineGlobalSettings.asset.meta
│   │       │       │   ├── TutorialInfo/
│   │       │       │   │   ├── Editor/
│   │       │       │   │   │   ├── ReadmeEditor.cs
│   │       │       │   │   │   └── ReadmeEditor.cs.meta
│   │       │       │   │   ├── Icons/
│   │       │       │   │   │   ├── URP.png
│   │       │       │   │   │   └── URP.png.meta
│   │       │       │   │   ├── StyleSheets/
│   │       │       │   │   │   ├── ReadmeEditor.uss
│   │       │       │   │   │   ├── ReadmeEditor.uss.meta
│   │       │       │   │   │   ├── ReadmeEditorDark.uss
│   │       │       │   │   │   ├── ReadmeEditorDark.uss.meta
│   │       │       │   │   │   ├── ReadmeEditorLight.uss
│   │       │       │   │   │   └── ReadmeEditorLight.uss.meta
│   │       │       │   │   ├── Editor.meta
│   │       │       │   │   ├── Icons.meta
│   │       │       │   │   ├── Layout.wlt
│   │       │       │   │   ├── Layout.wlt.meta
│   │       │       │   │   ├── Readme.cs
│   │       │       │   │   ├── Readme.cs.meta
│   │       │       │   │   └── StyleSheets.meta
│   │       │       │   ├── AstroGateway.meta
│   │       │       │   ├── InputSystem_Actions.inputactions
│   │       │       │   ├── InputSystem_Actions.inputactions.meta
│   │       │       │   ├── LaptopBuildOnly.meta
│   │       │       │   ├── Readme.asset
│   │       │       │   ├── Readme.asset.meta
│   │       │       │   ├── Scenes.meta
│   │       │       │   ├── Settings.meta
│   │       │       │   └── TutorialInfo.meta
│   │       │       ├── Packages/
│   │       │       │   ├── manifest.json
│   │       │       │   └── packages-lock.json
│   │       │       └── ProjectSettings/
│   │       │           ├── AudioManager.asset
│   │       │           ├── ClusterInputManager.asset
│   │       │           ├── DynamicsManager.asset
│   │       │           ├── EditorBuildSettings.asset
│   │       │           ├── EditorSettings.asset
│   │       │           ├── GraphicsSettings.asset
│   │       │           ├── InputManager.asset
│   │       │           ├── MemorySettings.asset
│   │       │           ├── MultiplayerManager.asset
│   │       │           ├── NavMeshAreas.asset
│   │       │           ├── PackageManagerSettings.asset
│   │       │           ├── Physics2DSettings.asset
│   │       │           ├── PhysicsCoreProjectSettings2D.asset
│   │       │           ├── PresetManager.asset
│   │       │           ├── ProjectAuditorSettings.asset
│   │       │           ├── ProjectSettings.asset
│   │       │           ├── ProjectVersion.txt
│   │       │           ├── QualitySettings.asset
│   │       │           ├── SceneTemplateSettings.json
│   │       │           ├── ShaderGraphSettings.asset
│   │       │           ├── TagManager.asset
│   │       │           ├── TimeManager.asset
│   │       │           ├── UnityConnectSettings.asset
│   │       │           ├── URPProjectSettings.asset
│   │       │           ├── VersionControlSettings.asset
│   │       │           ├── VFXManager.asset
│   │       │           └── XRSettings.asset
│   │       ├── results/
│   │       │   ├── Final/
│   │       │   │   ├── Step1/
│   │       │   │   │   └── Step1_Gameplay_Results.txt
│   │       │   │   └── Step2/
│   │       │   │       └── Mission_Integration_Results.txt
│   │       │   ├── Regression/
│   │       │   │   ├── HUD/
│   │       │   │   │   ├── HUD_Controlled_Tests.txt
│   │       │   │   │   ├── HUD_Test_Save.json
│   │       │   │   │   ├── Responsive_Layout_Checks.txt
│   │       │   │   │   └── Runtime_UI_Inventory.txt
│   │       │   │   ├── Mars/
│   │       │   │   │   └── Evidence/
│   │       │   │   │       ├── Console_Counts.txt
│   │       │   │   │       ├── Editor_Frame_Samples.csv
│   │       │   │   │       ├── Provisional_Synthetic_Telemetry.json
│   │       │   │   │       ├── Resolved_Pretest_Console_Counts.txt
│   │       │   │   │       ├── Scripted_Play_Mode_Tests.txt
│   │       │   │   │       └── Walking_Provisional_Synthetic_Telemetry.json
│   │       │   │   ├── Mission01/
│   │       │   │   │   ├── PlayMode_Controlled_Tests.txt
│   │       │   │   │   └── Test_Local_Save.json
│   │       │   │   ├── Native/
│   │       │   │   │   ├── Evidence/
│   │       │   │   │   │   ├── Export/
│   │       │   │   │   │   │   ├── AstroGateway_Simulated_Scans.csv
│   │       │   │   │   │   │   ├── AstroGateway_Simulated_Scans.json
│   │       │   │   │   │   │   └── AstroGateway_Simulated_Telemetry.json
│   │       │   │   │   │   └── Local_Test_Snapshot.json
│   │       │   │   │   └── Controlled_PlayMode_Tests.txt
│   │       │   │   └── VisualPolish/
│   │       │   │       ├── Export/
│   │       │   │       │   ├── AstroGateway_Simulated_Scans.csv
│   │       │   │       │   ├── AstroGateway_Simulated_Scans.json
│   │       │   │       │   └── AstroGateway_Simulated_Telemetry.json
│   │       │   │       ├── Controlled_PlayMode_Tests.txt
│   │       │   │       ├── Editor_Asset_Metrics.txt
│   │       │   │       └── Local_Test_Save.json
│   │       │   ├── Regression_WithFix/
│   │       │   │   ├── Suites/
│   │       │   │   │   ├── HUD/
│   │       │   │   │   │   ├── HUD_Controlled_Tests.txt
│   │       │   │   │   │   ├── HUD_Test_Save.json
│   │       │   │   │   │   ├── Responsive_Layout_Checks.txt
│   │       │   │   │   │   └── Runtime_UI_Inventory.txt
│   │       │   │   │   ├── Mars/
│   │       │   │   │   │   └── Evidence/
│   │       │   │   │   │       ├── Console_Counts.txt
│   │       │   │   │   │       ├── Editor_Frame_Samples.csv
│   │       │   │   │   │       ├── Provisional_Synthetic_Telemetry.json
│   │       │   │   │   │       ├── Resolved_Pretest_Console_Counts.txt
│   │       │   │   │   │       ├── Scripted_Play_Mode_Tests.txt
│   │       │   │   │   │       └── Walking_Provisional_Synthetic_Telemetry.json
│   │       │   │   │   ├── Mission01/
│   │       │   │   │   │   ├── PlayMode_Controlled_Tests.txt
│   │       │   │   │   │   └── Test_Local_Save.json
│   │       │   │   │   ├── Native/
│   │       │   │   │   │   ├── Evidence/
│   │       │   │   │   │   │   ├── Export/
│   │       │   │   │   │   │   │   ├── AstroGateway_Simulated_Scans.csv
│   │       │   │   │   │   │   │   ├── AstroGateway_Simulated_Scans.json
│   │       │   │   │   │   │   │   └── AstroGateway_Simulated_Telemetry.json
│   │       │   │   │   │   │   └── Local_Test_Snapshot.json
│   │       │   │   │   │   └── Controlled_PlayMode_Tests.txt
│   │       │   │   │   └── VisualPolish/
│   │       │   │   │       ├── Export/
│   │       │   │   │       │   ├── AstroGateway_Simulated_Scans.csv
│   │       │   │   │       │   ├── AstroGateway_Simulated_Scans.json
│   │       │   │   │       │   └── AstroGateway_Simulated_Telemetry.json
│   │       │   │   │       ├── Controlled_PlayMode_Tests.txt
│   │       │   │   │       ├── Editor_Asset_Metrics.txt
│   │       │   │   │       └── Local_Test_Save.json
│   │       │   │   └── Execution.txt
│   │       │   ├── Step1_rerun/
│   │       │   │   └── Step1_Gameplay_Results.txt
│   │       │   ├── Step2/
│   │       │   │   └── Mission_Integration_Results.txt
│   │       │   ├── Step3/
│   │       │   │   ├── backend_after_hashes.txt
│   │       │   │   ├── Backend_pytest_results.txt
│   │       │   │   └── Step3_Client_Results.txt
│   │       │   ├── Step4/
│   │       │   │   ├── backend_after_hashes.txt
│   │       │   │   ├── Backend_pytest_results.txt
│   │       │   │   └── Step4_Unity_Rules_Results.txt
│   │       │   ├── frame_probe.txt
│   │       │   ├── frame_probe_lab2.txt
│   │       │   ├── Step1_Gameplay_Results.txt
│   │       │   └── Step1_Gameplay_Results_FINAL.txt
│   │       ├── staging/
│   │       │   ├── Docs/
│   │       │   │   ├── Backend_Mission_Endpoints_Plan.md
│   │       │   │   └── Integration_Architecture.md
│   │       │   ├── Integration/
│   │       │   │   ├── Contracts/
│   │       │   │   │   ├── IntegrationContracts.cs
│   │       │   │   │   └── IntegrationContracts.cs.meta
│   │       │   │   ├── Local/
│   │       │   │   │   ├── GameplayEventBridge.cs
│   │       │   │   │   ├── GameplayEventBridge.cs.meta
│   │       │   │   │   ├── LifeSupportModelAdapter.cs
│   │       │   │   │   ├── LocalServices.cs
│   │       │   │   │   └── LocalServices.cs.meta
│   │       │   │   ├── Contracts.meta
│   │       │   │   └── Local.meta
│   │       │   ├── MissionIntegration/
│   │       │   │   ├── Editor/
│   │       │   │   │   └── MissionIntegrationBuilder.cs
│   │       │   │   └── Scripts/
│   │       │   │       └── MissionIntegrationController.cs
│   │       │   └── Verification/
│   │       │       └── Editor/
│   │       │           └── Step1GameplayVerification.cs
│   │       ├── VisualUpgradeMacBuild/
│   │       │   ├── Project/
│   │       │   │   ├── Assets/
│   │       │   │   │   ├── AstroGateway/
│   │       │   │   │   │   ├── CinematicHUD/
│   │       │   │   │   │   │   ├── Documentation/
│   │       │   │   │   │   │   │   ├── README.md
│   │       │   │   │   │   │   │   └── README.md.meta
│   │       │   │   │   │   │   ├── Prefabs/
│   │       │   │   │   │   │   │   ├── AstroGatewayCinematicHUD.prefab
│   │       │   │   │   │   │   │   └── AstroGatewayCinematicHUD.prefab.meta
│   │       │   │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   │   ├── Mars_CinematicHUD_v1.unity
│   │       │   │   │   │   │   │   └── Mars_CinematicHUD_v1.unity.meta
│   │       │   │   │   │   │   ├── Scripts/
│   │       │   │   │   │   │   │   ├── AstroGatewayHUDController.cs
│   │       │   │   │   │   │   │   ├── AstroGatewayHUDController.cs.meta
│   │       │   │   │   │   │   │   ├── CinematicEnvironmentAdapter.cs
│   │       │   │   │   │   │   │   ├── CinematicEnvironmentAdapter.cs.meta
│   │       │   │   │   │   │   │   ├── CinematicGameAdapter.cs
│   │       │   │   │   │   │   │   ├── CinematicGameAdapter.cs.meta
│   │       │   │   │   │   │   │   ├── HUDInputRouter.cs
│   │       │   │   │   │   │   │   ├── HUDInputRouter.cs.meta
│   │       │   │   │   │   │   │   ├── HUDLayoutProbe.cs
│   │       │   │   │   │   │   │   ├── HUDLayoutProbe.cs.meta
│   │       │   │   │   │   │   │   ├── HUDPerformanceProbe.cs
│   │       │   │   │   │   │   │   ├── HUDPerformanceProbe.cs.meta
│   │       │   │   │   │   │   │   ├── HUDPointerControl.cs
│   │       │   │   │   │   │   │   ├── HUDPointerControl.cs.meta
│   │       │   │   │   │   │   │   ├── HUDTelemetryAdapter.cs
│   │       │   │   │   │   │   │   ├── HUDTelemetryAdapter.cs.meta
│   │       │   │   │   │   │   │   ├── HUDTheme.cs
│   │       │   │   │   │   │   │   └── HUDTheme.cs.meta
│   │       │   │   │   │   │   ├── Settings/
│   │       │   │   │   │   │   │   ├── MissionRegistry.asset
│   │       │   │   │   │   │   │   └── MissionRegistry.asset.meta
│   │       │   │   │   │   │   ├── Tests/
│   │       │   │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   │   ├── CinematicHUDBuilder.cs
│   │       │   │   │   │   │   │   │   ├── CinematicHUDBuilder.cs.meta
│   │       │   │   │   │   │   │   │   ├── HUDResponsiveQA.cs
│   │       │   │   │   │   │   │   │   └── HUDResponsiveQA.cs.meta
│   │       │   │   │   │   │   │   └── Editor.meta
│   │       │   │   │   │   │   ├── Themes/
│   │       │   │   │   │   │   │   ├── AstroGatewayHUDTheme.asset
│   │       │   │   │   │   │   │   └── AstroGatewayHUDTheme.asset.meta
│   │       │   │   │   │   │   ├── UI/
│   │       │   │   │   │   │   │   ├── CircularControl.png
│   │       │   │   │   │   │   │   ├── CircularControl.png.meta
│   │       │   │   │   │   │   │   ├── HeadingArrow.png
│   │       │   │   │   │   │   │   ├── HeadingArrow.png.meta
│   │       │   │   │   │   │   │   ├── OriginalContourMap.png
│   │       │   │   │   │   │   │   ├── OriginalContourMap.png.meta
│   │       │   │   │   │   │   │   ├── RoundedPanel.png
│   │       │   │   │   │   │   │   └── RoundedPanel.png.meta
│   │       │   │   │   │   │   ├── Documentation.meta
│   │       │   │   │   │   │   ├── link.xml
│   │       │   │   │   │   │   ├── link.xml.meta
│   │       │   │   │   │   │   ├── Prefabs.meta
│   │       │   │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   │   ├── Scripts.meta
│   │       │   │   │   │   │   ├── Settings.meta
│   │       │   │   │   │   │   ├── Tests.meta
│   │       │   │   │   │   │   ├── Themes.meta
│   │       │   │   │   │   │   └── UI.meta
│   │       │   │   │   │   ├── Environment/
│   │       │   │   │   │   │   ├── Architecture/
│   │       │   │   │   │   │   │   ├── AgricultureG01_ChamferShell.asset
│   │       │   │   │   │   │   │   ├── AgricultureG01_ChamferShell.asset.meta
│   │       │   │   │   │   │   │   ├── AgricultureG01_Ring.asset
│   │       │   │   │   │   │   │   ├── AgricultureG01_Ring.asset.meta
│   │       │   │   │   │   │   │   ├── AirlockShell.asset
│   │       │   │   │   │   │   │   ├── AirlockShell.asset.meta
│   │       │   │   │   │   │   │   ├── C_ScienceStaging_Accent_Cyan_Combined.asset
│   │       │   │   │   │   │   │   ├── C_ScienceStaging_Accent_Cyan_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── C_ScienceStaging_Accent_SafetyOrange_Combined.asset
│   │       │   │   │   │   │   │   ├── C_ScienceStaging_Accent_SafetyOrange_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── C_ScienceStaging_Glass_Utility_Combined.asset
│   │       │   │   │   │   │   │   ├── C_ScienceStaging_Glass_Utility_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── C_ScienceStaging_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   │   ├── C_ScienceStaging_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── C_ScienceStaging_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   │   ├── C_ScienceStaging_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── C_ScienceStaging_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   │   ├── C_ScienceStaging_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── G_Greenhouse_Accent_Cyan_Combined.asset
│   │       │   │   │   │   │   │   ├── G_Greenhouse_Accent_Cyan_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── G_Greenhouse_Glass_Utility_Combined.asset
│   │       │   │   │   │   │   │   ├── G_Greenhouse_Glass_Utility_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── G_Greenhouse_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   │   ├── G_Greenhouse_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── G_Greenhouse_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   │   ├── G_Greenhouse_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── G_Greenhouse_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   │   ├── G_Greenhouse_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── H_CommandHabitat_Accent_Cyan_Combined.asset
│   │       │   │   │   │   │   │   ├── H_CommandHabitat_Accent_Cyan_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── H_CommandHabitat_Glass_Utility_Combined.asset
│   │       │   │   │   │   │   │   ├── H_CommandHabitat_Glass_Utility_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── H_CommandHabitat_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   │   ├── H_CommandHabitat_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── H_CommandHabitat_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   │   ├── H_CommandHabitat_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── H_CommandHabitat_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   │   ├── H_CommandHabitat_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── HabitatH01_ChamferShell.asset
│   │       │   │   │   │   │   │   ├── HabitatH01_ChamferShell.asset.meta
│   │       │   │   │   │   │   │   ├── HabitatH01_Ring.asset
│   │       │   │   │   │   │   │   ├── HabitatH01_Ring.asset.meta
│   │       │   │   │   │   │   │   ├── P_PowerStation_Accent_Cyan_Combined.asset
│   │       │   │   │   │   │   │   ├── P_PowerStation_Accent_Cyan_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── P_PowerStation_Accent_SafetyOrange_Combined.asset
│   │       │   │   │   │   │   │   ├── P_PowerStation_Accent_SafetyOrange_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── P_PowerStation_Glass_Utility_Combined.asset
│   │       │   │   │   │   │   │   ├── P_PowerStation_Glass_Utility_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── P_PowerStation_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   │   ├── P_PowerStation_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── P_PowerStation_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   │   ├── P_PowerStation_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── P_PowerStation_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   │   ├── P_PowerStation_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── R_RoverBay_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   │   ├── R_RoverBay_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── R_RoverBay_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   │   ├── R_RoverBay_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── R_RoverBay_Mars_Regolith_Dark_Combined.asset
│   │       │   │   │   │   │   │   ├── R_RoverBay_Mars_Regolith_Dark_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── R_RoverBay_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   │   ├── R_RoverBay_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── R_RoverBay_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   │   ├── R_RoverBay_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── S_SolarArray_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   │   ├── S_SolarArray_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── S_SolarArray_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   │   ├── S_SolarArray_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── S_SolarArray_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   │   ├── S_SolarArray_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── S_SolarArray_SolarPanel_Combined.asset
│   │       │   │   │   │   │   │   └── S_SolarArray_SolarPanel_Combined.asset.meta
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── MarsEnvironmentBuilder.cs
│   │       │   │   │   │   │   │   └── MarsEnvironmentBuilder.cs.meta
│   │       │   │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   │   ├── Accent_Cyan.mat
│   │       │   │   │   │   │   │   ├── Accent_Cyan.mat.meta
│   │       │   │   │   │   │   │   ├── Accent_SafetyOrange.mat
│   │       │   │   │   │   │   │   ├── Accent_SafetyOrange.mat.meta
│   │       │   │   │   │   │   │   ├── Basalt_Surface_512.png
│   │       │   │   │   │   │   │   ├── Basalt_Surface_512.png.meta
│   │       │   │   │   │   │   │   ├── Composite_DustPaint_512.png
│   │       │   │   │   │   │   │   ├── Composite_DustPaint_512.png.meta
│   │       │   │   │   │   │   │   ├── Delta_Bedding_512.png
│   │       │   │   │   │   │   │   ├── Delta_Bedding_512.png.meta
│   │       │   │   │   │   │   │   ├── Delta_FracturedStrata_512.png
│   │       │   │   │   │   │   │   ├── Delta_FracturedStrata_512.png.meta
│   │       │   │   │   │   │   │   ├── DustSky.shader
│   │       │   │   │   │   │   │   ├── DustSky.shader.meta
│   │       │   │   │   │   │   │   ├── Glass_Utility.mat
│   │       │   │   │   │   │   │   ├── Glass_Utility.mat.meta
│   │       │   │   │   │   │   │   ├── Habitat_CompositeDark.mat
│   │       │   │   │   │   │   │   ├── Habitat_CompositeDark.mat.meta
│   │       │   │   │   │   │   │   ├── Habitat_OffWhite.mat
│   │       │   │   │   │   │   │   ├── Habitat_OffWhite.mat.meta
│   │       │   │   │   │   │   │   ├── Mars_DustSky.mat
│   │       │   │   │   │   │   │   ├── Mars_DustSky.mat.meta
│   │       │   │   │   │   │   │   ├── Mars_Regolith.mat
│   │       │   │   │   │   │   │   ├── Mars_Regolith.mat.meta
│   │       │   │   │   │   │   │   ├── Mars_Regolith_Dark.mat
│   │       │   │   │   │   │   │   ├── Mars_Regolith_Dark.mat.meta
│   │       │   │   │   │   │   │   ├── Mars_Rock_Basaltic.mat
│   │       │   │   │   │   │   │   ├── Mars_Rock_Basaltic.mat.meta
│   │       │   │   │   │   │   │   ├── Mars_Rock_Layered.mat
│   │       │   │   │   │   │   │   ├── Mars_Rock_Layered.mat.meta
│   │       │   │   │   │   │   │   ├── Metal_Industrial.mat
│   │       │   │   │   │   │   │   ├── Metal_Industrial.mat.meta
│   │       │   │   │   │   │   │   ├── Regolith_Grain_512.png
│   │       │   │   │   │   │   │   ├── Regolith_Grain_512.png.meta
│   │       │   │   │   │   │   │   ├── Regolith_Macro_Pathwear_1024.png
│   │       │   │   │   │   │   │   ├── Regolith_Macro_Pathwear_1024.png.meta
│   │       │   │   │   │   │   │   ├── Regolith_MicroNormal_256.png
│   │       │   │   │   │   │   │   ├── Regolith_MicroNormal_256.png.meta
│   │       │   │   │   │   │   │   ├── Solar_Cells_Dust_512.png
│   │       │   │   │   │   │   │   ├── Solar_Cells_Dust_512.png.meta
│   │       │   │   │   │   │   │   ├── SolarPanel.mat
│   │       │   │   │   │   │   │   └── SolarPanel.mat.meta
│   │       │   │   │   │   │   ├── Prefabs/
│   │       │   │   │   │   │   │   ├── AngularBasalt.prefab
│   │       │   │   │   │   │   │   ├── AngularBasalt.prefab.meta
│   │       │   │   │   │   │   │   ├── C_ScienceStaging.prefab
│   │       │   │   │   │   │   │   ├── C_ScienceStaging.prefab.meta
│   │       │   │   │   │   │   │   ├── ErodedLandmark.prefab
│   │       │   │   │   │   │   │   ├── ErodedLandmark.prefab.meta
│   │       │   │   │   │   │   │   ├── G_Greenhouse.prefab
│   │       │   │   │   │   │   │   ├── G_Greenhouse.prefab.meta
│   │       │   │   │   │   │   │   ├── H_CommandHabitat.prefab
│   │       │   │   │   │   │   │   ├── H_CommandHabitat.prefab.meta
│   │       │   │   │   │   │   │   ├── LayeredDelta.prefab
│   │       │   │   │   │   │   │   ├── LayeredDelta.prefab.meta
│   │       │   │   │   │   │   │   ├── P_PowerStation.prefab
│   │       │   │   │   │   │   │   ├── P_PowerStation.prefab.meta
│   │       │   │   │   │   │   │   ├── R_RoverBay.prefab
│   │       │   │   │   │   │   │   ├── R_RoverBay.prefab.meta
│   │       │   │   │   │   │   │   ├── RegolithStone.prefab
│   │       │   │   │   │   │   │   ├── RegolithStone.prefab.meta
│   │       │   │   │   │   │   │   ├── S_SolarArray.prefab
│   │       │   │   │   │   │   │   └── S_SolarArray.prefab.meta
│   │       │   │   │   │   │   ├── Props/
│   │       │   │   │   │   │   │   ├── ApproachStoneClusters_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   │   ├── ApproachStoneClusters_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── HabitatExternalKit_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   │   ├── HabitatExternalKit_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── HabitatExternalKit_Habitat_OffWhite_Combined.asset
│   │       │   │   │   │   │   │   ├── HabitatExternalKit_Habitat_OffWhite_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── HabitatExternalKit_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   │   ├── HabitatExternalKit_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── PowerExternalKit_Habitat_CompositeDark_Combined.asset
│   │       │   │   │   │   │   │   ├── PowerExternalKit_Habitat_CompositeDark_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── PowerExternalKit_Mars_Regolith_Dark_Combined.asset
│   │       │   │   │   │   │   │   ├── PowerExternalKit_Mars_Regolith_Dark_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── UtilityRoutePosts_Accent_Cyan_Combined.asset
│   │       │   │   │   │   │   │   ├── UtilityRoutePosts_Accent_Cyan_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── UtilityRoutePosts_Metal_Industrial_Combined.asset
│   │       │   │   │   │   │   │   └── UtilityRoutePosts_Metal_Industrial_Combined.asset.meta
│   │       │   │   │   │   │   ├── Rocks/
│   │       │   │   │   │   │   │   ├── AngularBasalt_LOD0.asset
│   │       │   │   │   │   │   │   ├── AngularBasalt_LOD0.asset.meta
│   │       │   │   │   │   │   │   ├── AngularBasalt_LOD1.asset
│   │       │   │   │   │   │   │   ├── AngularBasalt_LOD1.asset.meta
│   │       │   │   │   │   │   │   ├── AngularBasalt_LOD2.asset
│   │       │   │   │   │   │   │   ├── AngularBasalt_LOD2.asset.meta
│   │       │   │   │   │   │   │   ├── DeltaFractured_LOD0.asset
│   │       │   │   │   │   │   │   ├── DeltaFractured_LOD0.asset.meta
│   │       │   │   │   │   │   │   ├── DeltaFractured_LOD1.asset
│   │       │   │   │   │   │   │   ├── DeltaFractured_LOD1.asset.meta
│   │       │   │   │   │   │   │   ├── DeltaFractured_LOD2.asset
│   │       │   │   │   │   │   │   ├── DeltaFractured_LOD2.asset.meta
│   │       │   │   │   │   │   │   ├── ErodedLandmark_LOD0.asset
│   │       │   │   │   │   │   │   ├── ErodedLandmark_LOD0.asset.meta
│   │       │   │   │   │   │   │   ├── ErodedLandmark_LOD1.asset
│   │       │   │   │   │   │   │   ├── ErodedLandmark_LOD1.asset.meta
│   │       │   │   │   │   │   │   ├── ErodedLandmark_LOD2.asset
│   │       │   │   │   │   │   │   ├── ErodedLandmark_LOD2.asset.meta
│   │       │   │   │   │   │   │   ├── LayeredDelta_LOD0.asset
│   │       │   │   │   │   │   │   ├── LayeredDelta_LOD0.asset.meta
│   │       │   │   │   │   │   │   ├── LayeredDelta_LOD1.asset
│   │       │   │   │   │   │   │   ├── LayeredDelta_LOD1.asset.meta
│   │       │   │   │   │   │   │   ├── LayeredDelta_LOD2.asset
│   │       │   │   │   │   │   │   ├── LayeredDelta_LOD2.asset.meta
│   │       │   │   │   │   │   │   ├── StonePatch_0_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   │   ├── StonePatch_0_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── StonePatch_1_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   │   ├── StonePatch_1_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── StonePatch_2_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   │   ├── StonePatch_2_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── StonePatch_3_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   │   ├── StonePatch_3_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── StonePatch_4_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   │   ├── StonePatch_4_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── StonePatch_5_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   │   ├── StonePatch_5_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── StonePatch_6_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   │   ├── StonePatch_6_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── StonePatch_7_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   │   ├── StonePatch_7_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   │   ├── StonePatch_8_Mars_Rock_Basaltic_Combined.asset
│   │       │   │   │   │   │   │   └── StonePatch_8_Mars_Rock_Basaltic_Combined.asset.meta
│   │       │   │   │   │   │   ├── Terrain/
│   │       │   │   │   │   │   │   ├── Regolith_250m_PreservedCorridor.asset
│   │       │   │   │   │   │   │   └── Regolith_250m_PreservedCorridor.asset.meta
│   │       │   │   │   │   │   ├── Architecture.meta
│   │       │   │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   │   ├── Prefabs.meta
│   │       │   │   │   │   │   ├── Props.meta
│   │       │   │   │   │   │   ├── Rocks.meta
│   │       │   │   │   │   │   └── Terrain.meta
│   │       │   │   │   │   ├── Greybox/
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── MarsGreyboxBuilder.cs
│   │       │   │   │   │   │   │   └── MarsGreyboxBuilder.cs.meta
│   │       │   │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   │   ├── DarkRock.mat
│   │       │   │   │   │   │   │   ├── DarkRock.mat.meta
│   │       │   │   │   │   │   │   ├── HabitatPale.mat
│   │       │   │   │   │   │   │   ├── HabitatPale.mat.meta
│   │       │   │   │   │   │   │   ├── IndustrialGraphite.mat
│   │       │   │   │   │   │   │   ├── IndustrialGraphite.mat.meta
│   │       │   │   │   │   │   │   ├── LayeredStone.mat
│   │       │   │   │   │   │   │   ├── LayeredStone.mat.meta
│   │       │   │   │   │   │   │   ├── SandTan.mat
│   │       │   │   │   │   │   │   ├── SandTan.mat.meta
│   │       │   │   │   │   │   │   ├── TerrainOchre.mat
│   │       │   │   │   │   │   │   ├── TerrainOchre.mat.meta
│   │       │   │   │   │   │   │   ├── WayfindingAmber.mat
│   │       │   │   │   │   │   │   ├── WayfindingAmber.mat.meta
│   │       │   │   │   │   │   │   ├── WayfindingCyan.mat
│   │       │   │   │   │   │   │   └── WayfindingCyan.mat.meta
│   │       │   │   │   │   │   ├── Meshes/
│   │       │   │   │   │   │   │   ├── Ground.asset
│   │       │   │   │   │   │   │   └── Ground.asset.meta
│   │       │   │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   │   ├── MarsOutpost_Greybox.unity
│   │       │   │   │   │   │   ├── MarsOutpost_Greybox.unity.meta
│   │       │   │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   │   ├── Meshes.meta
│   │       │   │   │   │   │   ├── PreGreybox_Untitled_Backup.unity
│   │       │   │   │   │   │   └── PreGreybox_Untitled_Backup.unity.meta
│   │       │   │   │   │   ├── Integration/
│   │       │   │   │   │   │   ├── Contracts/
│   │       │   │   │   │   │   │   ├── IntegrationContracts.cs
│   │       │   │   │   │   │   │   └── IntegrationContracts.cs.meta
│   │       │   │   │   │   │   ├── Http/
│   │       │   │   │   │   │   │   ├── HttpMissionSessionService.cs
│   │       │   │   │   │   │   │   └── HttpMissionSessionService.cs.meta
│   │       │   │   │   │   │   ├── Local/
│   │       │   │   │   │   │   │   ├── GameplayEventBridge.cs
│   │       │   │   │   │   │   │   ├── GameplayEventBridge.cs.meta
│   │       │   │   │   │   │   │   ├── LifeSupportModelAdapter.cs
│   │       │   │   │   │   │   │   ├── LifeSupportModelAdapter.cs.meta
│   │       │   │   │   │   │   │   ├── LocalServices.cs
│   │       │   │   │   │   │   │   └── LocalServices.cs.meta
│   │       │   │   │   │   │   ├── Contracts.meta
│   │       │   │   │   │   │   ├── Http.meta
│   │       │   │   │   │   │   └── Local.meta
│   │       │   │   │   │   ├── MarsEnvironment/
│   │       │   │   │   │   │   ├── Documentation/
│   │       │   │   │   │   │   │   ├── MissionRegistry.asset
│   │       │   │   │   │   │   │   ├── MissionRegistry.asset.meta
│   │       │   │   │   │   │   │   ├── README.md
│   │       │   │   │   │   │   │   └── README.md.meta
│   │       │   │   │   │   │   ├── Lighting/
│   │       │   │   │   │   │   │   ├── Dusty_Conditions.asset
│   │       │   │   │   │   │   │   ├── Dusty_Conditions.asset.meta
│   │       │   │   │   │   │   │   ├── MarsGradient.shader
│   │       │   │   │   │   │   │   ├── MarsGradient.shader.meta
│   │       │   │   │   │   │   │   ├── MarsSky.mat
│   │       │   │   │   │   │   │   ├── MarsSky.mat.meta
│   │       │   │   │   │   │   │   ├── MarsVolume.asset
│   │       │   │   │   │   │   │   ├── MarsVolume.asset.meta
│   │       │   │   │   │   │   │   ├── Midday.asset
│   │       │   │   │   │   │   │   ├── Midday.asset.meta
│   │       │   │   │   │   │   │   ├── Morning.asset
│   │       │   │   │   │   │   │   ├── Morning.asset.meta
│   │       │   │   │   │   │   │   ├── Sunset.asset
│   │       │   │   │   │   │   │   └── Sunset.asset.meta
│   │       │   │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   │   ├── DistantDust.asset
│   │       │   │   │   │   │   │   ├── DistantDust.asset.meta
│   │       │   │   │   │   │   │   ├── Dusty solar cells.mat
│   │       │   │   │   │   │   │   ├── Dusty solar cells.mat.meta
│   │       │   │   │   │   │   │   ├── EnvironmentDust.mat
│   │       │   │   │   │   │   │   ├── EnvironmentDust.mat.meta
│   │       │   │   │   │   │   │   ├── Fine Mars dust.mat
│   │       │   │   │   │   │   │   ├── Fine Mars dust.mat.meta
│   │       │   │   │   │   │   │   ├── Habitat ceramic composite.mat
│   │       │   │   │   │   │   │   ├── Habitat ceramic composite.mat.meta
│   │       │   │   │   │   │   │   ├── Observation panels — opaque approximation.mat
│   │       │   │   │   │   │   │   ├── Observation panels — opaque approximation.mat.meta
│   │       │   │   │   │   │   │   ├── Restrained identification ochre.mat
│   │       │   │   │   │   │   │   ├── Restrained identification ochre.mat.meta
│   │       │   │   │   │   │   │   ├── Rock07 dusty basalt.mat
│   │       │   │   │   │   │   │   ├── Rock07 dusty basalt.mat.meta
│   │       │   │   │   │   │   │   ├── Rock09 compact mineral.mat
│   │       │   │   │   │   │   │   ├── Rock09 compact mineral.mat.meta
│   │       │   │   │   │   │   │   ├── Rover_Cab.mat
│   │       │   │   │   │   │   │   ├── Rover_Cab.mat.meta
│   │       │   │   │   │   │   │   ├── Rover_Chassis.mat
│   │       │   │   │   │   │   │   ├── Rover_Chassis.mat.meta
│   │       │   │   │   │   │   │   ├── Rover_RoofMarker.mat
│   │       │   │   │   │   │   │   ├── Rover_RoofMarker.mat.meta
│   │       │   │   │   │   │   │   ├── Rover_Wheel_-1_0.mat
│   │       │   │   │   │   │   │   ├── Rover_Wheel_-1_0.mat.meta
│   │       │   │   │   │   │   │   ├── Rover_Wheel_-1_1.mat
│   │       │   │   │   │   │   │   ├── Rover_Wheel_-1_1.mat.meta
│   │       │   │   │   │   │   │   ├── Rover_Wheel_-1_2.mat
│   │       │   │   │   │   │   │   ├── Rover_Wheel_-1_2.mat.meta
│   │       │   │   │   │   │   │   ├── Rover_Wheel_1_0.mat
│   │       │   │   │   │   │   │   ├── Rover_Wheel_1_0.mat.meta
│   │       │   │   │   │   │   │   ├── Rover_Wheel_1_1.mat
│   │       │   │   │   │   │   │   ├── Rover_Wheel_1_1.mat.meta
│   │       │   │   │   │   │   │   ├── Rover_Wheel_1_2.mat
│   │       │   │   │   │   │   │   ├── Rover_Wheel_1_2.mat.meta
│   │       │   │   │   │   │   │   ├── Structural graphite.mat
│   │       │   │   │   │   │   │   └── Structural graphite.mat.meta
│   │       │   │   │   │   │   ├── Models/
│   │       │   │   │   │   │   │   ├── Crew_Dome_Ribs_LowPoly.asset
│   │       │   │   │   │   │   │   ├── Crew_Dome_Ribs_LowPoly.asset.meta
│   │       │   │   │   │   │   │   ├── HabitatHemisphere.asset
│   │       │   │   │   │   │   │   ├── HabitatHemisphere.asset.meta
│   │       │   │   │   │   │   │   ├── Laboratory_Dome_Ribs_LowPoly.asset
│   │       │   │   │   │   │   │   ├── Laboratory_Dome_Ribs_LowPoly.asset.meta
│   │       │   │   │   │   │   │   ├── Life_Support_Dome_Ribs_LowPoly.asset
│   │       │   │   │   │   │   │   ├── Life_Support_Dome_Ribs_LowPoly.asset.meta
│   │       │   │   │   │   │   │   ├── rock_07_LOD0.asset
│   │       │   │   │   │   │   │   ├── rock_07_LOD0.asset.meta
│   │       │   │   │   │   │   │   ├── rock_07_LOD1.asset
│   │       │   │   │   │   │   │   ├── rock_07_LOD1.asset.meta
│   │       │   │   │   │   │   │   ├── rock_07_LOD2.asset
│   │       │   │   │   │   │   │   ├── rock_07_LOD2.asset.meta
│   │       │   │   │   │   │   │   ├── rock_09_LOD0.asset
│   │       │   │   │   │   │   │   ├── rock_09_LOD0.asset.meta
│   │       │   │   │   │   │   │   ├── rock_09_LOD1.asset
│   │       │   │   │   │   │   │   ├── rock_09_LOD1.asset.meta
│   │       │   │   │   │   │   │   ├── rock_09_LOD2.asset
│   │       │   │   │   │   │   │   ├── rock_09_LOD2.asset.meta
│   │       │   │   │   │   │   │   ├── Terrain250m.asset
│   │       │   │   │   │   │   │   └── Terrain250m.asset.meta
│   │       │   │   │   │   │   ├── Prefabs/
│   │       │   │   │   │   │   │   ├── ARESYN_ClosedHabitatComplex_Optimized.prefab
│   │       │   │   │   │   │   │   ├── ARESYN_ClosedHabitatComplex_Optimized.prefab.meta
│   │       │   │   │   │   │   │   ├── SolarFarm_Optimized.prefab
│   │       │   │   │   │   │   │   └── SolarFarm_Optimized.prefab.meta
│   │       │   │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   │   ├── Mars_RealisticResearchBase_v1.unity
│   │       │   │   │   │   │   │   └── Mars_RealisticResearchBase_v1.unity.meta
│   │       │   │   │   │   │   ├── Scripts/
│   │       │   │   │   │   │   │   ├── EnvironmentPresentation.cs
│   │       │   │   │   │   │   │   ├── EnvironmentPresentation.cs.meta
│   │       │   │   │   │   │   │   ├── EnvironmentPreset.cs
│   │       │   │   │   │   │   │   ├── EnvironmentPreset.cs.meta
│   │       │   │   │   │   │   │   ├── ResearchTarget.cs
│   │       │   │   │   │   │   │   └── ResearchTarget.cs.meta
│   │       │   │   │   │   │   ├── Tests/
│   │       │   │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   │   ├── MarsResearchBaseBuilder.cs
│   │       │   │   │   │   │   │   │   └── MarsResearchBaseBuilder.cs.meta
│   │       │   │   │   │   │   │   └── Editor.meta
│   │       │   │   │   │   │   ├── Textures/
│   │       │   │   │   │   │   │   ├── rock_07_AO.png
│   │       │   │   │   │   │   │   ├── rock_07_AO.png.meta
│   │       │   │   │   │   │   │   ├── rock_07_Diffuse.png
│   │       │   │   │   │   │   │   ├── rock_07_Diffuse.png.meta
│   │       │   │   │   │   │   │   ├── rock_07_nor_gl.png
│   │       │   │   │   │   │   │   ├── rock_07_nor_gl.png.meta
│   │       │   │   │   │   │   │   ├── rock_07_Rough.png
│   │       │   │   │   │   │   │   ├── rock_07_Rough.png.meta
│   │       │   │   │   │   │   │   ├── rock_09_AO.png
│   │       │   │   │   │   │   │   ├── rock_09_AO.png.meta
│   │       │   │   │   │   │   │   ├── rock_09_Diffuse.png
│   │       │   │   │   │   │   │   ├── rock_09_Diffuse.png.meta
│   │       │   │   │   │   │   │   ├── rock_09_nor_gl.png
│   │       │   │   │   │   │   │   ├── rock_09_nor_gl.png.meta
│   │       │   │   │   │   │   │   ├── rock_09_Rough.png
│   │       │   │   │   │   │   │   ├── rock_09_Rough.png.meta
│   │       │   │   │   │   │   │   ├── sandy_gravel_AO.png
│   │       │   │   │   │   │   │   ├── sandy_gravel_AO.png.meta
│   │       │   │   │   │   │   │   ├── sandy_gravel_Diffuse.png
│   │       │   │   │   │   │   │   ├── sandy_gravel_Diffuse.png.meta
│   │       │   │   │   │   │   │   ├── sandy_gravel_nor_gl.png
│   │       │   │   │   │   │   │   ├── sandy_gravel_nor_gl.png.meta
│   │       │   │   │   │   │   │   ├── sandy_gravel_Rough.png
│   │       │   │   │   │   │   │   └── sandy_gravel_Rough.png.meta
│   │       │   │   │   │   │   ├── Documentation.meta
│   │       │   │   │   │   │   ├── Lighting.meta
│   │       │   │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   │   ├── Models.meta
│   │       │   │   │   │   │   ├── Prefabs.meta
│   │       │   │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   │   ├── Scripts.meta
│   │       │   │   │   │   │   ├── Tests.meta
│   │       │   │   │   │   │   ├── Textures.meta
│   │       │   │   │   │   │   └── VFX.meta
│   │       │   │   │   │   ├── MarsVisualPolish/
│   │       │   │   │   │   │   ├── Documentation/
│   │       │   │   │   │   │   │   ├── PolishRegistry.asset
│   │       │   │   │   │   │   │   └── PolishRegistry.asset.meta
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── VisualPolishBuilder.cs
│   │       │   │   │   │   │   │   └── VisualPolishBuilder.cs.meta
│   │       │   │   │   │   │   ├── Lighting/
│   │       │   │   │   │   │   │   ├── PolishSky.mat
│   │       │   │   │   │   │   │   ├── PolishSky.mat.meta
│   │       │   │   │   │   │   │   ├── PolishVolume.asset
│   │       │   │   │   │   │   │   ├── PolishVolume.asset.meta
│   │       │   │   │   │   │   │   ├── Preset_0.asset
│   │       │   │   │   │   │   │   ├── Preset_0.asset.meta
│   │       │   │   │   │   │   │   ├── Preset_1.asset
│   │       │   │   │   │   │   │   ├── Preset_1.asset.meta
│   │       │   │   │   │   │   │   ├── Preset_2.asset
│   │       │   │   │   │   │   │   ├── Preset_2.asset.meta
│   │       │   │   │   │   │   │   ├── Preset_3.asset
│   │       │   │   │   │   │   │   └── Preset_3.asset.meta
│   │       │   │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   │   ├── Brushed frame.mat
│   │       │   │   │   │   │   │   ├── Brushed frame.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_0.mat
│   │       │   │   │   │   │   │   ├── Derivative_0.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_1.mat
│   │       │   │   │   │   │   │   ├── Derivative_1.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_10.mat
│   │       │   │   │   │   │   │   ├── Derivative_10.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_11.mat
│   │       │   │   │   │   │   │   ├── Derivative_11.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_12.mat
│   │       │   │   │   │   │   │   ├── Derivative_12.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_13.mat
│   │       │   │   │   │   │   │   ├── Derivative_13.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_14.mat
│   │       │   │   │   │   │   │   ├── Derivative_14.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_15.mat
│   │       │   │   │   │   │   │   ├── Derivative_15.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_16.mat
│   │       │   │   │   │   │   │   ├── Derivative_16.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_17.mat
│   │       │   │   │   │   │   │   ├── Derivative_17.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_18.mat
│   │       │   │   │   │   │   │   ├── Derivative_18.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_19.mat
│   │       │   │   │   │   │   │   ├── Derivative_19.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_2.mat
│   │       │   │   │   │   │   │   ├── Derivative_2.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_20.mat
│   │       │   │   │   │   │   │   ├── Derivative_20.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_21.mat
│   │       │   │   │   │   │   │   ├── Derivative_21.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_3.mat
│   │       │   │   │   │   │   │   ├── Derivative_3.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_4.mat
│   │       │   │   │   │   │   │   ├── Derivative_4.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_5.mat
│   │       │   │   │   │   │   │   ├── Derivative_5.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_6.mat
│   │       │   │   │   │   │   │   ├── Derivative_6.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_7.mat
│   │       │   │   │   │   │   │   ├── Derivative_7.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_8.mat
│   │       │   │   │   │   │   │   ├── Derivative_8.mat.meta
│   │       │   │   │   │   │   │   ├── Derivative_9.mat
│   │       │   │   │   │   │   │   ├── Derivative_9.mat.meta
│   │       │   │   │   │   │   │   ├── Photovoltaic cell laminate.mat
│   │       │   │   │   │   │   │   ├── Photovoltaic cell laminate.mat.meta
│   │       │   │   │   │   │   │   ├── PhotovoltaicGrid.png
│   │       │   │   │   │   │   │   ├── PhotovoltaicGrid.png.meta
│   │       │   │   │   │   │   │   ├── Recessed seals.mat
│   │       │   │   │   │   │   │   ├── Recessed seals.mat.meta
│   │       │   │   │   │   │   │   ├── Safety ochre.mat
│   │       │   │   │   │   │   │   ├── Safety ochre.mat.meta
│   │       │   │   │   │   │   │   ├── Warm ceramic.mat
│   │       │   │   │   │   │   │   └── Warm ceramic.mat.meta
│   │       │   │   │   │   │   ├── Models/
│   │       │   │   │   │   │   │   ├── Crew Dome_Frame.asset
│   │       │   │   │   │   │   │   ├── Crew Dome_Frame.asset.meta
│   │       │   │   │   │   │   │   ├── Laboratory Dome_Frame.asset
│   │       │   │   │   │   │   │   ├── Laboratory Dome_Frame.asset.meta
│   │       │   │   │   │   │   │   ├── Life Support Dome_Frame.asset
│   │       │   │   │   │   │   │   └── Life Support Dome_Frame.asset.meta
│   │       │   │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   │   ├── Mars_VisualPolish_v2.unity
│   │       │   │   │   │   │   │   └── Mars_VisualPolish_v2.unity.meta
│   │       │   │   │   │   │   ├── Documentation.meta
│   │       │   │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   │   ├── Lighting.meta
│   │       │   │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   │   ├── Models.meta
│   │       │   │   │   │   │   ├── PolishBenchmark.cs
│   │       │   │   │   │   │   ├── PolishBenchmark.cs.meta
│   │       │   │   │   │   │   ├── PolishHudTheme.cs
│   │       │   │   │   │   │   ├── PolishHudTheme.cs.meta
│   │       │   │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   │   ├── VisualPolishPresentation.cs
│   │       │   │   │   │   │   └── VisualPolishPresentation.cs.meta
│   │       │   │   │   │   ├── Mission01Milestone/
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── Mission01Builder.cs
│   │       │   │   │   │   │   │   └── Mission01Builder.cs.meta
│   │       │   │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   │   ├── Mission01_Continents.mat
│   │       │   │   │   │   │   │   ├── Mission01_Continents.mat.meta
│   │       │   │   │   │   │   │   ├── Mission01_Graphite.mat
│   │       │   │   │   │   │   │   ├── Mission01_Graphite.mat.meta
│   │       │   │   │   │   │   │   ├── Mission01_Ocean.mat
│   │       │   │   │   │   │   │   ├── Mission01_Ocean.mat.meta
│   │       │   │   │   │   │   │   ├── Mission01_Pale.mat
│   │       │   │   │   │   │   │   ├── Mission01_Pale.mat.meta
│   │       │   │   │   │   │   │   ├── Mission01_Safety.mat
│   │       │   │   │   │   │   │   └── Mission01_Safety.mat.meta
│   │       │   │   │   │   │   ├── Prefabs/
│   │       │   │   │   │   │   │   ├── Boarding_Console.prefab
│   │       │   │   │   │   │   │   ├── Boarding_Console.prefab.meta
│   │       │   │   │   │   │   │   ├── Trainee_Blockout.prefab
│   │       │   │   │   │   │   │   └── Trainee_Blockout.prefab.meta
│   │       │   │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   │   ├── Mission01_EarthLaunch_Milestone_v4.unity
│   │       │   │   │   │   │   ├── Mission01_EarthLaunch_Milestone_v4.unity.meta
│   │       │   │   │   │   │   ├── Mission01Flow.cs
│   │       │   │   │   │   │   ├── Mission01Flow.cs.meta
│   │       │   │   │   │   │   └── Prefabs.meta
│   │       │   │   │   │   ├── MissionIntegration/
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── MissionIntegrationBuilder.cs
│   │       │   │   │   │   │   │   └── MissionIntegrationBuilder.cs.meta
│   │       │   │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   │   ├── Mars_MissionIntegration_v1.unity
│   │       │   │   │   │   │   │   └── Mars_MissionIntegration_v1.unity.meta
│   │       │   │   │   │   │   ├── Scripts/
│   │       │   │   │   │   │   │   ├── MissionIntegrationController.cs
│   │       │   │   │   │   │   │   └── MissionIntegrationController.cs.meta
│   │       │   │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   │   └── Scripts.meta
│   │       │   │   │   │   ├── MissionRules/
│   │       │   │   │   │   │   ├── Config/
│   │       │   │   │   │   │   │   ├── mission_rules_v1.json
│   │       │   │   │   │   │   │   ├── mission_rules_v1.json.meta
│   │       │   │   │   │   │   │   ├── test_vectors_v1.json
│   │       │   │   │   │   │   │   └── test_vectors_v1.json.meta
│   │       │   │   │   │   │   ├── Scripts/
│   │       │   │   │   │   │   │   ├── MiniJson.cs
│   │       │   │   │   │   │   │   ├── MiniJson.cs.meta
│   │       │   │   │   │   │   │   ├── MissionRules.cs
│   │       │   │   │   │   │   │   └── MissionRules.cs.meta
│   │       │   │   │   │   │   ├── Config.meta
│   │       │   │   │   │   │   └── Scripts.meta
│   │       │   │   │   │   ├── NativeControlSlice/
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── NativeSliceBuilder.cs
│   │       │   │   │   │   │   │   └── NativeSliceBuilder.cs.meta
│   │       │   │   │   │   │   ├── Prefabs/
│   │       │   │   │   │   │   │   ├── AdaptiveTouch.prefab
│   │       │   │   │   │   │   │   └── AdaptiveTouch.prefab.meta
│   │       │   │   │   │   │   ├── AdaptiveTouch.cs
│   │       │   │   │   │   │   ├── AdaptiveTouch.cs.meta
│   │       │   │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   │   ├── Mars_NativeControl_VerticalSlice.unity
│   │       │   │   │   │   │   ├── Mars_NativeControl_VerticalSlice.unity.meta
│   │       │   │   │   │   │   ├── MissionRegistry.asset
│   │       │   │   │   │   │   ├── MissionRegistry.asset.meta
│   │       │   │   │   │   │   ├── NativeSliceGame.cs
│   │       │   │   │   │   │   ├── NativeSliceGame.cs.meta
│   │       │   │   │   │   │   ├── NativeSystems.cs
│   │       │   │   │   │   │   ├── NativeSystems.cs.meta
│   │       │   │   │   │   │   └── Prefabs.meta
│   │       │   │   │   │   ├── ProductionVisual/
│   │       │   │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   │   ├── Brushed aluminum.mat
│   │       │   │   │   │   │   │   ├── Brushed aluminum.mat.meta
│   │       │   │   │   │   │   │   ├── Dusty tire elastomer.mat
│   │       │   │   │   │   │   │   ├── Dusty tire elastomer.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_0.mat
│   │       │   │   │   │   │   │   ├── Inherited_0.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_1.mat
│   │       │   │   │   │   │   │   ├── Inherited_1.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_10.mat
│   │       │   │   │   │   │   │   ├── Inherited_10.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_11.mat
│   │       │   │   │   │   │   │   ├── Inherited_11.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_12.mat
│   │       │   │   │   │   │   │   ├── Inherited_12.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_13.mat
│   │       │   │   │   │   │   │   ├── Inherited_13.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_14.mat
│   │       │   │   │   │   │   │   ├── Inherited_14.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_15.mat
│   │       │   │   │   │   │   │   ├── Inherited_15.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_16.mat
│   │       │   │   │   │   │   │   ├── Inherited_16.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_17.mat
│   │       │   │   │   │   │   │   ├── Inherited_17.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_18.mat
│   │       │   │   │   │   │   │   ├── Inherited_18.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_19.mat
│   │       │   │   │   │   │   │   ├── Inherited_19.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_2.mat
│   │       │   │   │   │   │   │   ├── Inherited_2.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_20.mat
│   │       │   │   │   │   │   │   ├── Inherited_20.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_21.mat
│   │       │   │   │   │   │   │   ├── Inherited_21.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_22.mat
│   │       │   │   │   │   │   │   ├── Inherited_22.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_23.mat
│   │       │   │   │   │   │   │   ├── Inherited_23.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_24.mat
│   │       │   │   │   │   │   │   ├── Inherited_24.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_25.mat
│   │       │   │   │   │   │   │   ├── Inherited_25.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_3.mat
│   │       │   │   │   │   │   │   ├── Inherited_3.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_4.mat
│   │       │   │   │   │   │   │   ├── Inherited_4.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_5.mat
│   │       │   │   │   │   │   │   ├── Inherited_5.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_6.mat
│   │       │   │   │   │   │   │   ├── Inherited_6.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_7.mat
│   │       │   │   │   │   │   │   ├── Inherited_7.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_8.mat
│   │       │   │   │   │   │   │   ├── Inherited_8.mat.meta
│   │       │   │   │   │   │   │   ├── Inherited_9.mat
│   │       │   │   │   │   │   │   ├── Inherited_9.mat.meta
│   │       │   │   │   │   │   │   ├── Machined graphite alloy.mat
│   │       │   │   │   │   │   │   ├── Machined graphite alloy.mat.meta
│   │       │   │   │   │   │   │   ├── Opaque protective visor glazing.mat
│   │       │   │   │   │   │   │   ├── Opaque protective visor glazing.mat.meta
│   │       │   │   │   │   │   │   ├── Pressure shell ceramic composite.mat
│   │       │   │   │   │   │   │   ├── Pressure shell ceramic composite.mat.meta
│   │       │   │   │   │   │   │   ├── Restrained identification orange.mat
│   │       │   │   │   │   │   │   └── Restrained identification orange.mat.meta
│   │       │   │   │   │   │   ├── Meshes/
│   │       │   │   │   │   │   │   ├── ChamferedEnclosure.asset
│   │       │   │   │   │   │   │   ├── ChamferedEnclosure.asset.meta
│   │       │   │   │   │   │   │   ├── CommsTruss.asset
│   │       │   │   │   │   │   │   ├── CommsTruss.asset.meta
│   │       │   │   │   │   │   │   ├── Crew_Dome_sealed_airlock_Rails.asset
│   │       │   │   │   │   │   │   ├── Crew_Dome_sealed_airlock_Rails.asset.meta
│   │       │   │   │   │   │   │   ├── CrewCabEnclosure.asset
│   │       │   │   │   │   │   │   ├── CrewCabEnclosure.asset.meta
│   │       │   │   │   │   │   │   ├── GeodesicFrame.asset
│   │       │   │   │   │   │   │   ├── GeodesicFrame.asset.meta
│   │       │   │   │   │   │   │   ├── GeodesicPanels.asset
│   │       │   │   │   │   │   │   ├── GeodesicPanels.asset.meta
│   │       │   │   │   │   │   │   ├── Laboratory_Dome_sealed_airlock_Rails.asset
│   │       │   │   │   │   │   │   ├── Laboratory_Dome_sealed_airlock_Rails.asset.meta
│   │       │   │   │   │   │   │   ├── Life_Support_Dome_sealed_airlock_Rails.asset
│   │       │   │   │   │   │   │   ├── Life_Support_Dome_sealed_airlock_Rails.asset.meta
│   │       │   │   │   │   │   │   ├── RoverMechanicalFrame.asset
│   │       │   │   │   │   │   │   ├── RoverMechanicalFrame.asset.meta
│   │       │   │   │   │   │   │   ├── RoverWheel_LOD0.asset
│   │       │   │   │   │   │   │   ├── RoverWheel_LOD0.asset.meta
│   │       │   │   │   │   │   │   ├── RoverWheel_LOD1.asset
│   │       │   │   │   │   │   │   ├── RoverWheel_LOD1.asset.meta
│   │       │   │   │   │   │   │   ├── RoverWheel_LOD2.asset
│   │       │   │   │   │   │   │   └── RoverWheel_LOD2.asset.meta
│   │       │   │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   │   ├── Mars_ProductionVisual_v1.unity
│   │       │   │   │   │   │   │   └── Mars_ProductionVisual_v1.unity.meta
│   │       │   │   │   │   │   ├── Scripts/
│   │       │   │   │   │   │   │   ├── ProductionCameraRig.cs
│   │       │   │   │   │   │   │   ├── ProductionCameraRig.cs.meta
│   │       │   │   │   │   │   │   ├── ProductionModeCoordinator.cs
│   │       │   │   │   │   │   │   ├── ProductionModeCoordinator.cs.meta
│   │       │   │   │   │   │   │   ├── ProductionPerformanceProbe.cs
│   │       │   │   │   │   │   │   ├── ProductionPerformanceProbe.cs.meta
│   │       │   │   │   │   │   │   ├── ProductionPresentation.cs
│   │       │   │   │   │   │   │   └── ProductionPresentation.cs.meta
│   │       │   │   │   │   │   ├── Settings/
│   │       │   │   │   │   │   │   ├── Lighting_0.asset
│   │       │   │   │   │   │   │   ├── Lighting_0.asset.meta
│   │       │   │   │   │   │   │   ├── Lighting_1.asset
│   │       │   │   │   │   │   │   ├── Lighting_1.asset.meta
│   │       │   │   │   │   │   │   ├── Lighting_2.asset
│   │       │   │   │   │   │   │   ├── Lighting_2.asset.meta
│   │       │   │   │   │   │   │   ├── Lighting_3.asset
│   │       │   │   │   │   │   │   ├── Lighting_3.asset.meta
│   │       │   │   │   │   │   │   ├── ProductionMissionRegistry.asset
│   │       │   │   │   │   │   │   └── ProductionMissionRegistry.asset.meta
│   │       │   │   │   │   │   ├── Tests/
│   │       │   │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   │   ├── ProductionMeshFactory.cs
│   │       │   │   │   │   │   │   │   ├── ProductionMeshFactory.cs.meta
│   │       │   │   │   │   │   │   │   ├── ProductionViewport.cs
│   │       │   │   │   │   │   │   │   ├── ProductionViewport.cs.meta
│   │       │   │   │   │   │   │   │   ├── ProductionVisualBuilder.cs
│   │       │   │   │   │   │   │   │   └── ProductionVisualBuilder.cs.meta
│   │       │   │   │   │   │   │   └── Editor.meta
│   │       │   │   │   │   │   ├── Textures/
│   │       │   │   │   │   │   │   ├── rock_07_MetallicSmoothness.png
│   │       │   │   │   │   │   │   ├── rock_07_MetallicSmoothness.png.meta
│   │       │   │   │   │   │   │   ├── rock_09_MetallicSmoothness.png
│   │       │   │   │   │   │   │   ├── rock_09_MetallicSmoothness.png.meta
│   │       │   │   │   │   │   │   ├── sandy_gravel_MetallicSmoothness.png
│   │       │   │   │   │   │   │   └── sandy_gravel_MetallicSmoothness.png.meta
│   │       │   │   │   │   │   ├── Themes/
│   │       │   │   │   │   │   │   ├── ProductionHUDTheme.asset
│   │       │   │   │   │   │   │   ├── ProductionHUDTheme.asset.meta
│   │       │   │   │   │   │   │   ├── ProductionSky.mat
│   │       │   │   │   │   │   │   ├── ProductionSky.mat.meta
│   │       │   │   │   │   │   │   ├── ProductionVolume.asset
│   │       │   │   │   │   │   │   └── ProductionVolume.asset.meta
│   │       │   │   │   │   │   ├── Documentation.meta
│   │       │   │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   │   ├── Meshes.meta
│   │       │   │   │   │   │   ├── Prefabs.meta
│   │       │   │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   │   ├── Scripts.meta
│   │       │   │   │   │   │   ├── Settings.meta
│   │       │   │   │   │   │   ├── Tests.meta
│   │       │   │   │   │   │   ├── Textures.meta
│   │       │   │   │   │   │   └── Themes.meta
│   │       │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   ├── MarsOutpost_Environment_v01.unity
│   │       │   │   │   │   │   └── MarsOutpost_Environment_v01.unity.meta
│   │       │   │   │   │   ├── Stage08B/
│   │       │   │   │   │   │   ├── Animation/
│   │       │   │   │   │   │   │   ├── Idle.anim
│   │       │   │   │   │   │   │   ├── Idle.anim.meta
│   │       │   │   │   │   │   │   ├── PrototypeLocomotion.controller
│   │       │   │   │   │   │   │   ├── PrototypeLocomotion.controller.meta
│   │       │   │   │   │   │   │   ├── Run.anim
│   │       │   │   │   │   │   │   ├── Run.anim.meta
│   │       │   │   │   │   │   │   ├── Walk.anim
│   │       │   │   │   │   │   │   └── Walk.anim.meta
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── Stage08BBuilder.cs
│   │       │   │   │   │   │   │   └── Stage08BBuilder.cs.meta
│   │       │   │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   │   ├── 08B_Accent_Cyan.mat
│   │       │   │   │   │   │   │   ├── 08B_Accent_Cyan.mat.meta
│   │       │   │   │   │   │   │   ├── 08B_Accent_SafetyOrange.mat
│   │       │   │   │   │   │   │   ├── 08B_Accent_SafetyOrange.mat.meta
│   │       │   │   │   │   │   │   ├── 08B_Glass_Utility.mat
│   │       │   │   │   │   │   │   ├── 08B_Glass_Utility.mat.meta
│   │       │   │   │   │   │   │   ├── 08B_Habitat_CompositeDark.mat
│   │       │   │   │   │   │   │   ├── 08B_Habitat_CompositeDark.mat.meta
│   │       │   │   │   │   │   │   ├── 08B_Habitat_OffWhite.mat
│   │       │   │   │   │   │   │   ├── 08B_Habitat_OffWhite.mat.meta
│   │       │   │   │   │   │   │   ├── 08B_Mars_Regolith.mat
│   │       │   │   │   │   │   │   ├── 08B_Mars_Regolith.mat.meta
│   │       │   │   │   │   │   │   ├── 08B_Mars_Regolith_Dark.mat
│   │       │   │   │   │   │   │   ├── 08B_Mars_Regolith_Dark.mat.meta
│   │       │   │   │   │   │   │   ├── 08B_Mars_Rock_Basaltic.mat
│   │       │   │   │   │   │   │   ├── 08B_Mars_Rock_Basaltic.mat.meta
│   │       │   │   │   │   │   │   ├── 08B_Mars_Rock_Layered.mat
│   │       │   │   │   │   │   │   ├── 08B_Mars_Rock_Layered.mat.meta
│   │       │   │   │   │   │   │   ├── 08B_Metal_Industrial.mat
│   │       │   │   │   │   │   │   ├── 08B_Metal_Industrial.mat.meta
│   │       │   │   │   │   │   │   ├── 08B_SolarPanel.mat
│   │       │   │   │   │   │   │   ├── 08B_SolarPanel.mat.meta
│   │       │   │   │   │   │   │   ├── BasaltVariationA.mat
│   │       │   │   │   │   │   │   ├── BasaltVariationA.mat.meta
│   │       │   │   │   │   │   │   ├── BasaltVariationB.mat
│   │       │   │   │   │   │   │   ├── BasaltVariationB.mat.meta
│   │       │   │   │   │   │   │   ├── BasaltVariationC.mat
│   │       │   │   │   │   │   │   ├── BasaltVariationC.mat.meta
│   │       │   │   │   │   │   │   ├── Dust_Particle.mat
│   │       │   │   │   │   │   │   ├── Dust_Particle.mat.meta
│   │       │   │   │   │   │   │   ├── Identification_Cyan.mat
│   │       │   │   │   │   │   │   ├── Identification_Cyan.mat.meta
│   │       │   │   │   │   │   │   ├── Safety_Orange.mat
│   │       │   │   │   │   │   │   ├── Safety_Orange.mat.meta
│   │       │   │   │   │   │   │   ├── Suit_Graphite.mat
│   │       │   │   │   │   │   │   ├── Suit_Graphite.mat.meta
│   │       │   │   │   │   │   │   ├── Suit_Pale.mat
│   │       │   │   │   │   │   │   └── Suit_Pale.mat.meta
│   │       │   │   │   │   │   ├── Animation.meta
│   │       │   │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   │   ├── MarsOutpost_Playable_Stage08B.unity
│   │       │   │   │   │   │   ├── MarsOutpost_Playable_Stage08B.unity.meta
│   │       │   │   │   │   │   ├── MarsPrototype.cs
│   │       │   │   │   │   │   ├── MarsPrototype.cs.meta
│   │       │   │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   │   ├── Stage08B_MobilePreview.asset
│   │       │   │   │   │   │   └── Stage08B_MobilePreview.asset.meta
│   │       │   │   │   │   ├── Stage08C/
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── AndroidProbeBuild.cs
│   │       │   │   │   │   │   │   ├── AndroidProbeBuild.cs.meta
│   │       │   │   │   │   │   │   ├── TouchChecks.cs
│   │       │   │   │   │   │   │   └── TouchChecks.cs.meta
│   │       │   │   │   │   │   ├── AndroidProfileProbe.cs
│   │       │   │   │   │   │   ├── AndroidProfileProbe.cs.meta
│   │       │   │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   │   ├── MobileControls.cs
│   │       │   │   │   │   │   └── MobileControls.cs.meta
│   │       │   │   │   │   ├── Stage08C1/
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── EditorPerformanceObserver.cs
│   │       │   │   │   │   │   │   ├── EditorPerformanceObserver.cs.meta
│   │       │   │   │   │   │   │   ├── ManualInputObserver.cs
│   │       │   │   │   │   │   │   └── ManualInputObserver.cs.meta
│   │       │   │   │   │   │   └── Editor.meta
│   │       │   │   │   │   ├── Verification/
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── LaptopPlaytest.cs
│   │       │   │   │   │   │   │   ├── LaptopPlaytest.cs.meta
│   │       │   │   │   │   │   │   ├── Step1GameplayVerification.cs
│   │       │   │   │   │   │   │   ├── Step1GameplayVerification.cs.meta
│   │       │   │   │   │   │   │   ├── Step3BackendClientTest.cs
│   │       │   │   │   │   │   │   ├── Step3BackendClientTest.cs.meta
│   │       │   │   │   │   │   │   ├── Step4RulesTest.cs
│   │       │   │   │   │   │   │   └── Step4RulesTest.cs.meta
│   │       │   │   │   │   │   └── Editor.meta
│   │       │   │   │   │   ├── VisualUpgrade/
│   │       │   │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   │   ├── VisualUpgradeBuilder.cs
│   │       │   │   │   │   │   │   ├── VisualUpgradeBuilder.cs.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeCapture.cs
│   │       │   │   │   │   │   │   ├── VisualUpgradeCapture.cs.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeCharacters.cs
│   │       │   │   │   │   │   │   ├── VisualUpgradeCharacters.cs.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeEnvironment.cs
│   │       │   │   │   │   │   │   ├── VisualUpgradeEnvironment.cs.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeKit.cs
│   │       │   │   │   │   │   │   ├── VisualUpgradeKit.cs.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradePlaytest.cs
│   │       │   │   │   │   │   │   ├── VisualUpgradePlaytest.cs.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeProps.cs
│   │       │   │   │   │   │   │   └── VisualUpgradeProps.cs.meta
│   │       │   │   │   │   │   ├── Materials/
│   │       │   │   │   │   │   │   ├── VisualUpgradeHaze.mat
│   │       │   │   │   │   │   │   ├── VisualUpgradeHaze.mat.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeMarsGround.mat
│   │       │   │   │   │   │   │   ├── VisualUpgradeMarsGround.mat.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeMarsSky.mat
│   │       │   │   │   │   │   │   ├── VisualUpgradeMarsSky.mat.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradePhotovoltaic.mat
│   │       │   │   │   │   │   │   ├── VisualUpgradePhotovoltaic.mat.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeRock07.mat
│   │       │   │   │   │   │   │   ├── VisualUpgradeRock07.mat.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeRock09.mat
│   │       │   │   │   │   │   │   ├── VisualUpgradeRock09.mat.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeSky.mat
│   │       │   │   │   │   │   │   ├── VisualUpgradeSky.mat.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeWheelDust.mat
│   │       │   │   │   │   │   │   ├── VisualUpgradeWheelDust.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Accent_orange.mat
│   │       │   │   │   │   │   │   ├── VU_Accent_orange.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Beacon_red.mat
│   │       │   │   │   │   │   │   ├── VU_Beacon_red.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Brushed_aluminium.mat
│   │       │   │   │   │   │   │   ├── VU_Brushed_aluminium.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Equipment_white.mat
│   │       │   │   │   │   │   │   ├── VU_Equipment_white.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Habitat_marker_light.mat
│   │       │   │   │   │   │   │   ├── VU_Habitat_marker_light.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Habitat_shell.mat
│   │       │   │   │   │   │   │   ├── VU_Habitat_shell.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Habitat_window_glow.mat
│   │       │   │   │   │   │   │   ├── VU_Habitat_window_glow.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Helmet_lamp.mat
│   │       │   │   │   │   │   │   ├── VU_Helmet_lamp.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Mission_emblem_(provisional).mat
│   │       │   │   │   │   │   │   ├── VU_Mission_emblem_(provisional).mat.meta
│   │       │   │   │   │   │   │   ├── VU_Rover_beacon.mat
│   │       │   │   │   │   │   │   ├── VU_Rover_beacon.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Rover_body_panel.mat
│   │       │   │   │   │   │   │   ├── VU_Rover_body_panel.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Rover_cockpit_glass.mat
│   │       │   │   │   │   │   │   ├── VU_Rover_cockpit_glass.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Rover_graphite.mat
│   │       │   │   │   │   │   │   ├── VU_Rover_graphite.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Rover_headlight_lens.mat
│   │       │   │   │   │   │   │   ├── VU_Rover_headlight_lens.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Rover_screen.mat
│   │       │   │   │   │   │   │   ├── VU_Rover_screen.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Rover_seat.mat
│   │       │   │   │   │   │   │   ├── VU_Rover_seat.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Rover_solar_cells.mat
│   │       │   │   │   │   │   │   ├── VU_Rover_solar_cells.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Rover_tail_light.mat
│   │       │   │   │   │   │   │   ├── VU_Rover_tail_light.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Sensor_lens.mat
│   │       │   │   │   │   │   │   ├── VU_Sensor_lens.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Status_amber.mat
│   │       │   │   │   │   │   │   ├── VU_Status_amber.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Status_cyan.mat
│   │       │   │   │   │   │   │   ├── VU_Status_cyan.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Suit_fabric.mat
│   │       │   │   │   │   │   │   ├── VU_Suit_fabric.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Suit_hard_shell.mat
│   │       │   │   │   │   │   │   ├── VU_Suit_hard_shell.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Suit_joint_bearing.mat
│   │       │   │   │   │   │   │   ├── VU_Suit_joint_bearing.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Suit_soft_grey.mat
│   │       │   │   │   │   │   │   ├── VU_Suit_soft_grey.mat.meta
│   │       │   │   │   │   │   │   ├── VU_Visor_gold_coating.mat
│   │       │   │   │   │   │   │   └── VU_Visor_gold_coating.mat.meta
│   │       │   │   │   │   │   ├── Meshes/
│   │       │   │   │   │   │   │   ├── VisualUpgradeDish.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeDish.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeFarTerrain.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeFarTerrain.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeRoverDish.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeRoverDish.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_0_0.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_0_0.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_0_1.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_0_1.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_0_2.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_0_2.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_0_3.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_0_3.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_0_4.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_0_4.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_0_5.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_0_5.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_1_0.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_1_0.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_1_1.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_1_1.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_1_2.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_1_2.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_1_3.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_1_3.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_1_4.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_1_4.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_1_5.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_1_5.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_2_0.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_2_0.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_2_1.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_2_1.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_2_2.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_2_2.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_2_3.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_2_3.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_2_4.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_2_4.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_2_5.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_2_5.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_3_0.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_3_0.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_3_1.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_3_1.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_3_2.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_3_2.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_3_3.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_3_3.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_3_4.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_3_4.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_3_5.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_3_5.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_4_0.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_4_0.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_4_1.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_4_1.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_4_2.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_4_2.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_4_3.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_4_3.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_4_4.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_4_4.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_4_5.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_4_5.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_5_0.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_5_0.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_5_1.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_5_1.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_5_2.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_5_2.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_5_3.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_5_3.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_5_4.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_5_4.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeScatter_5_5.asset
│   │       │   │   │   │   │   │   └── VisualUpgradeScatter_5_5.asset.meta
│   │       │   │   │   │   │   ├── Scenes/
│   │       │   │   │   │   │   │   ├── Mars_VisualUpgrade_v1.unity
│   │       │   │   │   │   │   │   └── Mars_VisualUpgrade_v1.unity.meta
│   │       │   │   │   │   │   ├── Scripts/
│   │       │   │   │   │   │   │   ├── VisualUpgradeAstronautMotion.cs
│   │       │   │   │   │   │   │   ├── VisualUpgradeAstronautMotion.cs.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeAtmosphere.cs
│   │       │   │   │   │   │   │   ├── VisualUpgradeAtmosphere.cs.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeBlink.cs
│   │       │   │   │   │   │   │   ├── VisualUpgradeBlink.cs.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeCameraRig.cs
│   │       │   │   │   │   │   │   ├── VisualUpgradeCameraRig.cs.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeHUDFixes.cs
│   │       │   │   │   │   │   │   ├── VisualUpgradeHUDFixes.cs.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeHUDStyle.cs
│   │       │   │   │   │   │   │   ├── VisualUpgradeHUDStyle.cs.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeRoverPresentation.cs
│   │       │   │   │   │   │   │   └── VisualUpgradeRoverPresentation.cs.meta
│   │       │   │   │   │   │   ├── Settings/
│   │       │   │   │   │   │   │   ├── Lighting_0.asset
│   │       │   │   │   │   │   │   ├── Lighting_0.asset.meta
│   │       │   │   │   │   │   │   ├── Lighting_1.asset
│   │       │   │   │   │   │   │   ├── Lighting_1.asset.meta
│   │       │   │   │   │   │   │   ├── Lighting_2.asset
│   │       │   │   │   │   │   │   ├── Lighting_2.asset.meta
│   │       │   │   │   │   │   │   ├── Lighting_3.asset
│   │       │   │   │   │   │   │   ├── Lighting_3.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgrade_Renderer.asset
│   │       │   │   │   │   │   │   ├── VisualUpgrade_Renderer.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgrade_URP.asset
│   │       │   │   │   │   │   │   ├── VisualUpgrade_URP.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeHUDTheme.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeHUDTheme.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeMissionRegistry.asset
│   │       │   │   │   │   │   │   ├── VisualUpgradeMissionRegistry.asset.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeVolume.asset
│   │       │   │   │   │   │   │   └── VisualUpgradeVolume.asset.meta
│   │       │   │   │   │   │   ├── Shaders/
│   │       │   │   │   │   │   │   ├── MarsSky.shader
│   │       │   │   │   │   │   │   ├── MarsSky.shader.meta
│   │       │   │   │   │   │   │   ├── MarsTerrain.shader
│   │       │   │   │   │   │   │   └── MarsTerrain.shader.meta
│   │       │   │   │   │   │   ├── Textures/
│   │       │   │   │   │   │   │   ├── VisualUpgradeEmblem.png
│   │       │   │   │   │   │   │   ├── VisualUpgradeEmblem.png.meta
│   │       │   │   │   │   │   │   ├── VisualUpgradeSoftDust.png
│   │       │   │   │   │   │   │   └── VisualUpgradeSoftDust.png.meta
│   │       │   │   │   │   │   ├── Documentation.meta
│   │       │   │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   │   ├── Materials.meta
│   │       │   │   │   │   │   ├── Meshes.meta
│   │       │   │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   │   ├── Scripts.meta
│   │       │   │   │   │   │   ├── Settings.meta
│   │       │   │   │   │   │   ├── Shaders.meta
│   │       │   │   │   │   │   └── Textures.meta
│   │       │   │   │   │   ├── CinematicHUD.meta
│   │       │   │   │   │   ├── Environment.meta
│   │       │   │   │   │   ├── Greybox.meta
│   │       │   │   │   │   ├── Integration.meta
│   │       │   │   │   │   ├── MarsEnvironment.meta
│   │       │   │   │   │   ├── MarsVisualPolish.meta
│   │       │   │   │   │   ├── Mission01Milestone.meta
│   │       │   │   │   │   ├── MissionIntegration.meta
│   │       │   │   │   │   ├── MissionRules.meta
│   │       │   │   │   │   ├── NativeControlSlice.meta
│   │       │   │   │   │   ├── ProductionVisual.meta
│   │       │   │   │   │   ├── Scenes.meta
│   │       │   │   │   │   ├── Stage08B.meta
│   │       │   │   │   │   ├── Stage08C.meta
│   │       │   │   │   │   ├── Stage08C1.meta
│   │       │   │   │   │   ├── Verification.meta
│   │       │   │   │   │   └── VisualUpgrade.meta
│   │       │   │   │   ├── BuildOnly/
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── VisualUpgradeMacBuild.cs
│   │       │   │   │   │   │   └── VisualUpgradeMacBuild.cs.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── StandaloneProbe.cs
│   │       │   │   │   │   └── StandaloneProbe.cs.meta
│   │       │   │   │   ├── Scenes/
│   │       │   │   │   │   ├── SampleScene.unity
│   │       │   │   │   │   └── SampleScene.unity.meta
│   │       │   │   │   ├── Settings/
│   │       │   │   │   │   ├── DefaultVolumeProfile.asset
│   │       │   │   │   │   ├── DefaultVolumeProfile.asset.meta
│   │       │   │   │   │   ├── Mobile_Renderer.asset
│   │       │   │   │   │   ├── Mobile_Renderer.asset.meta
│   │       │   │   │   │   ├── Mobile_RPAsset.asset
│   │       │   │   │   │   ├── Mobile_RPAsset.asset.meta
│   │       │   │   │   │   ├── PC_Renderer.asset
│   │       │   │   │   │   ├── PC_Renderer.asset.meta
│   │       │   │   │   │   ├── PC_RPAsset.asset
│   │       │   │   │   │   ├── PC_RPAsset.asset.meta
│   │       │   │   │   │   ├── SampleSceneProfile.asset
│   │       │   │   │   │   ├── SampleSceneProfile.asset.meta
│   │       │   │   │   │   ├── UniversalRenderPipelineGlobalSettings.asset
│   │       │   │   │   │   └── UniversalRenderPipelineGlobalSettings.asset.meta
│   │       │   │   │   ├── TutorialInfo/
│   │       │   │   │   │   ├── Editor/
│   │       │   │   │   │   │   ├── ReadmeEditor.cs
│   │       │   │   │   │   │   └── ReadmeEditor.cs.meta
│   │       │   │   │   │   ├── Icons/
│   │       │   │   │   │   │   ├── URP.png
│   │       │   │   │   │   │   └── URP.png.meta
│   │       │   │   │   │   ├── StyleSheets/
│   │       │   │   │   │   │   ├── ReadmeEditor.uss
│   │       │   │   │   │   │   ├── ReadmeEditor.uss.meta
│   │       │   │   │   │   │   ├── ReadmeEditorDark.uss
│   │       │   │   │   │   │   ├── ReadmeEditorDark.uss.meta
│   │       │   │   │   │   │   ├── ReadmeEditorLight.uss
│   │       │   │   │   │   │   └── ReadmeEditorLight.uss.meta
│   │       │   │   │   │   ├── Editor.meta
│   │       │   │   │   │   ├── Icons.meta
│   │       │   │   │   │   ├── Layout.wlt
│   │       │   │   │   │   ├── Layout.wlt.meta
│   │       │   │   │   │   ├── Readme.cs
│   │       │   │   │   │   ├── Readme.cs.meta
│   │       │   │   │   │   └── StyleSheets.meta
│   │       │   │   │   ├── AstroGateway.meta
│   │       │   │   │   ├── BuildOnly.meta
│   │       │   │   │   ├── InputSystem_Actions.inputactions
│   │       │   │   │   ├── InputSystem_Actions.inputactions.meta
│   │       │   │   │   ├── Readme.asset
│   │       │   │   │   ├── Readme.asset.meta
│   │       │   │   │   ├── Scenes.meta
│   │       │   │   │   ├── Settings.meta
│   │       │   │   │   └── TutorialInfo.meta
│   │       │   │   ├── Packages/
│   │       │   │   │   ├── manifest.json
│   │       │   │   │   └── packages-lock.json
│   │       │   │   └── ProjectSettings/
│   │       │   │       ├── AudioManager.asset
│   │       │   │       ├── ClusterInputManager.asset
│   │       │   │       ├── DynamicsManager.asset
│   │       │   │       ├── EditorBuildSettings.asset
│   │       │   │       ├── EditorSettings.asset
│   │       │   │       ├── GraphicsSettings.asset
│   │       │   │       ├── InputManager.asset
│   │       │   │       ├── MemorySettings.asset
│   │       │   │       ├── MultiplayerManager.asset
│   │       │   │       ├── NavMeshAreas.asset
│   │       │   │       ├── PackageManagerSettings.asset
│   │       │   │       ├── Physics2DSettings.asset
│   │       │   │       ├── PhysicsCoreProjectSettings2D.asset
│   │       │   │       ├── PresetManager.asset
│   │       │   │       ├── ProjectAuditorSettings.asset
│   │       │   │       ├── ProjectSettings.asset
│   │       │   │       ├── ProjectVersion.txt
│   │       │   │       ├── QualitySettings.asset
│   │       │   │       ├── SceneTemplateSettings.json
│   │       │   │       ├── ShaderGraphSettings.asset
│   │       │   │       ├── TagManager.asset
│   │       │   │       ├── TimeManager.asset
│   │       │   │       ├── UnityConnectSettings.asset
│   │       │   │       ├── URPProjectSettings.asset
│   │       │   │       ├── VersionControlSettings.asset
│   │       │   │       ├── VFXManager.asset
│   │       │   │       └── XRSettings.asset
│   │       │   ├── last_out.txt
│   │       │   └── StandaloneProbe_v1_backup.cs.txt
│   │       ├── run_lab2.sh
│   │       └── run_regression.sh
│   ├── MissionRedHorizon/
│   │   ├── Assets/
│   │   │   ├── _Recovery/
│   │   │   │   ├── 0.unity
│   │   │   │   └── 0.unity.meta
│   │   │   ├── AstroGateway/
│   │   │   │   ├── CinematicHUD/
│   │   │   │   │   ├── Documentation/
│   │   │   │   │   │   ├── README.md
│   │   │   │   │   │   └── README.md.meta
│   │   │   │   │   ├── Prefabs/
│   │   │   │   │   │   ├── AstroGatewayCinematicHUD.prefab
│   │   │   │   │   │   └── AstroGatewayCinematicHUD.prefab.meta
│   │   │   │   │   ├── Scenes/
│   │   │   │   │   │   ├── Mars_CinematicHUD_v1.unity
│   │   │   │   │   │   └── Mars_CinematicHUD_v1.unity.meta
│   │   │   │   │   ├── Scripts/
│   │   │   │   │   │   ├── AstroGatewayHUDController.cs
│   │   │   │   │   │   ├── AstroGatewayHUDController.cs.meta
│   │   │   │   │   │   ├── CinematicEnvironmentAdapter.cs
│   │   │   │   │   │   ├── CinematicEnvironmentAdapter.cs.meta
│   │   │   │   │   │   ├── CinematicGameAdapter.cs
│   │   │   │   │   │   ├── CinematicGameAdapter.cs.meta
│   │   │   │   │   │   ├── HUDInputRouter.cs
│   │   │   │   │   │   ├── HUDInputRouter.cs.meta
│   │   │   │   │   │   ├── HUDLayoutProbe.cs
│   │   │   │   │   │   ├── HUDLayoutProbe.cs.meta
│   │   │   │   │   │   ├── HUDPerformanceProbe.cs
│   │   │   │   │   │   ├── HUDPerformanceProbe.cs.meta
│   │   │   │   │   │   ├── HUDPointerControl.cs
│   │   │   │   │   │   ├── HUDPointerControl.cs.meta
│   │   │   │   │   │   ├── HUDTelemetryAdapter.cs
│   │   │   │   │   │   ├── HUDTelemetryAdapter.cs.meta
│   │   │   │   │   │   ├── HUDTheme.cs
│   │   │   │   │   │   └── HUDTheme.cs.meta
│   │   │   │   │   ├── Settings/
│   │   │   │   │   │   ├── MissionRegistry.asset
│   │   │   │   │   │   └── MissionRegistry.asset.meta
│   │   │   │   │   ├── Tests/
│   │   │   │   │   │   ├── Editor/
│   │   │   │   │   │   │   ├── CinematicHUDBuilder.cs
│   │   │   │   │   │   │   ├── CinematicHUDBuilder.cs.meta
│   │   │   │   │   │   │   ├── HUDResponsiveQA.cs
│   │   │   │   │   │   │   └── HUDResponsiveQA.cs.meta
│   │   │   │   │   │   └── Editor.meta
│   │   │   │   │   ├── Themes/
│   │   │   │   │   │   ├── AstroGatewayHUDTheme.asset
│   │   │   │   │   │   └── AstroGatewayHUDTheme.asset.meta
│   │   │   │   │   ├── UI/
│   │   │   │   │   │   ├── CircularControl.png
│   │   │   │   │   │   ├── CircularControl.png.meta
│   │   │   │   │   │   ├── HeadingArrow.png
│   │   │   │   │   │   ├── HeadingArrow.png.meta
│   │   │   │   │   │   ├── OriginalContourMap.png
│   │   │   │   │   │   ├── OriginalContourMap.png.meta
│   │   │   │   │   │   ├── RoundedPanel.png
│   │   │   │   │   │   └── RoundedPanel.png.meta
│   │   │   │   │   ├── Documentation.meta
│   │   │   │   │   ├── link.xml
│   │   │   │   │   ├── link.xml.meta
│   │   │   │   │   ├── Prefabs.meta
│   │   │   │   │   ├── Scenes.meta
│   │   │   │   │   ├── Scripts.meta
│   │   │   │   │   ├── Settings.meta
│   │   │   │   │   ├── Tests.meta
│   │   │   │   │   ├── Themes.meta
│   │   │   │   │   └── UI.meta
│   │   │   │   ├── Environment/
│   │   │   │   │   ├── Architecture/
│   │   │   │   │   │   ├── AgricultureG01_ChamferShell.asset
│   │   │   │   │   │   ├── AgricultureG01_ChamferShell.asset.meta
│   │   │   │   │   │   ├── AgricultureG01_Ring.asset
│   │   │   │   │   │   ├── AgricultureG01_Ring.asset.meta
│   │   │   │   │   │   ├── AirlockShell.asset
│   │   │   │   │   │   ├── AirlockShell.asset.meta
│   │   │   │   │   │   ├── C_ScienceStaging_Accent_Cyan_Combined.asset
│   │   │   │   │   │   ├── C_ScienceStaging_Accent_Cyan_Combined.asset.meta
│   │   │   │   │   │   ├── C_ScienceStaging_Accent_SafetyOrange_Combined.asset
│   │   │   │   │   │   ├── C_ScienceStaging_Accent_SafetyOrange_Combined.asset.meta
│   │   │   │   │   │   ├── C_ScienceStaging_Glass_Utility_Combined.asset
│   │   │   │   │   │   ├── C_ScienceStaging_Glass_Utility_Combined.asset.meta
│   │   │   │   │   │   ├── C_ScienceStaging_Habitat_CompositeDark_Combined.asset
│   │   │   │   │   │   ├── C_ScienceStaging_Habitat_CompositeDark_Combined.asset.meta
│   │   │   │   │   │   ├── C_ScienceStaging_Habitat_OffWhite_Combined.asset
│   │   │   │   │   │   ├── C_ScienceStaging_Habitat_OffWhite_Combined.asset.meta
│   │   │   │   │   │   ├── C_ScienceStaging_Metal_Industrial_Combined.asset
│   │   │   │   │   │   ├── C_ScienceStaging_Metal_Industrial_Combined.asset.meta
│   │   │   │   │   │   ├── G_Greenhouse_Accent_Cyan_Combined.asset
│   │   │   │   │   │   ├── G_Greenhouse_Accent_Cyan_Combined.asset.meta
│   │   │   │   │   │   ├── G_Greenhouse_Glass_Utility_Combined.asset
│   │   │   │   │   │   ├── G_Greenhouse_Glass_Utility_Combined.asset.meta
│   │   │   │   │   │   ├── G_Greenhouse_Habitat_CompositeDark_Combined.asset
│   │   │   │   │   │   ├── G_Greenhouse_Habitat_CompositeDark_Combined.asset.meta
│   │   │   │   │   │   ├── G_Greenhouse_Habitat_OffWhite_Combined.asset
│   │   │   │   │   │   ├── G_Greenhouse_Habitat_OffWhite_Combined.asset.meta
│   │   │   │   │   │   ├── G_Greenhouse_Metal_Industrial_Combined.asset
│   │   │   │   │   │   ├── G_Greenhouse_Metal_Industrial_Combined.asset.meta
│   │   │   │   │   │   ├── H_CommandHabitat_Accent_Cyan_Combined.asset
│   │   │   │   │   │   ├── H_CommandHabitat_Accent_Cyan_Combined.asset.meta
│   │   │   │   │   │   ├── H_CommandHabitat_Glass_Utility_Combined.asset
│   │   │   │   │   │   ├── H_CommandHabitat_Glass_Utility_Combined.asset.meta
│   │   │   │   │   │   ├── H_CommandHabitat_Habitat_CompositeDark_Combined.asset
│   │   │   │   │   │   ├── H_CommandHabitat_Habitat_CompositeDark_Combined.asset.meta
│   │   │   │   │   │   ├── H_CommandHabitat_Habitat_OffWhite_Combined.asset
│   │   │   │   │   │   ├── H_CommandHabitat_Habitat_OffWhite_Combined.asset.meta
│   │   │   │   │   │   ├── H_CommandHabitat_Metal_Industrial_Combined.asset
│   │   │   │   │   │   ├── H_CommandHabitat_Metal_Industrial_Combined.asset.meta
│   │   │   │   │   │   ├── HabitatH01_ChamferShell.asset
│   │   │   │   │   │   ├── HabitatH01_ChamferShell.asset.meta
│   │   │   │   │   │   ├── HabitatH01_Ring.asset
│   │   │   │   │   │   ├── HabitatH01_Ring.asset.meta
│   │   │   │   │   │   ├── P_PowerStation_Accent_Cyan_Combined.asset
│   │   │   │   │   │   ├── P_PowerStation_Accent_Cyan_Combined.asset.meta
│   │   │   │   │   │   ├── P_PowerStation_Accent_SafetyOrange_Combined.asset
│   │   │   │   │   │   ├── P_PowerStation_Accent_SafetyOrange_Combined.asset.meta
│   │   │   │   │   │   ├── P_PowerStation_Glass_Utility_Combined.asset
│   │   │   │   │   │   ├── P_PowerStation_Glass_Utility_Combined.asset.meta
│   │   │   │   │   │   ├── P_PowerStation_Habitat_CompositeDark_Combined.asset
│   │   │   │   │   │   ├── P_PowerStation_Habitat_CompositeDark_Combined.asset.meta
│   │   │   │   │   │   ├── P_PowerStation_Habitat_OffWhite_Combined.asset
│   │   │   │   │   │   ├── P_PowerStation_Habitat_OffWhite_Combined.asset.meta
│   │   │   │   │   │   ├── P_PowerStation_Metal_Industrial_Combined.asset
│   │   │   │   │   │   ├── P_PowerStation_Metal_Industrial_Combined.asset.meta
│   │   │   │   │   │   ├── R_RoverBay_Habitat_CompositeDark_Combined.asset
│   │   │   │   │   │   ├── R_RoverBay_Habitat_CompositeDark_Combined.asset.meta
│   │   │   │   │   │   ├── R_RoverBay_Habitat_OffWhite_Combined.asset
│   │   │   │   │   │   ├── R_RoverBay_Habitat_OffWhite_Combined.asset.meta
│   │   │   │   │   │   ├── R_RoverBay_Mars_Regolith_Dark_Combined.asset
│   │   │   │   │   │   ├── R_RoverBay_Mars_Regolith_Dark_Combined.asset.meta
│   │   │   │   │   │   ├── R_RoverBay_Mars_Rock_Basaltic_Combined.asset
│   │   │   │   │   │   ├── R_RoverBay_Mars_Rock_Basaltic_Combined.asset.meta
│   │   │   │   │   │   ├── R_RoverBay_Metal_Industrial_Combined.asset
│   │   │   │   │   │   ├── R_RoverBay_Metal_Industrial_Combined.asset.meta
│   │   │   │   │   │   ├── S_SolarArray_Habitat_CompositeDark_Combined.asset
│   │   │   │   │   │   ├── S_SolarArray_Habitat_CompositeDark_Combined.asset.meta
│   │   │   │   │   │   ├── S_SolarArray_Habitat_OffWhite_Combined.asset
│   │   │   │   │   │   ├── S_SolarArray_Habitat_OffWhite_Combined.asset.meta
│   │   │   │   │   │   ├── S_SolarArray_Metal_Industrial_Combined.asset
│   │   │   │   │   │   ├── S_SolarArray_Metal_Industrial_Combined.asset.meta
│   │   │   │   │   │   ├── S_SolarArray_SolarPanel_Combined.asset
│   │   │   │   │   │   └── S_SolarArray_SolarPanel_Combined.asset.meta
│   │   │   │   │   ├── Editor/
│   │   │   │   │   │   ├── MarsEnvironmentBuilder.cs
│   │   │   │   │   │   └── MarsEnvironmentBuilder.cs.meta
│   │   │   │   │   ├── Materials/
│   │   │   │   │   │   ├── Accent_Cyan.mat
│   │   │   │   │   │   ├── Accent_Cyan.mat.meta
│   │   │   │   │   │   ├── Accent_SafetyOrange.mat
│   │   │   │   │   │   ├── Accent_SafetyOrange.mat.meta
│   │   │   │   │   │   ├── Basalt_Surface_512.png
│   │   │   │   │   │   ├── Basalt_Surface_512.png.meta
│   │   │   │   │   │   ├── Composite_DustPaint_512.png
│   │   │   │   │   │   ├── Composite_DustPaint_512.png.meta
│   │   │   │   │   │   ├── Delta_Bedding_512.png
│   │   │   │   │   │   ├── Delta_Bedding_512.png.meta
│   │   │   │   │   │   ├── Delta_FracturedStrata_512.png
│   │   │   │   │   │   ├── Delta_FracturedStrata_512.png.meta
│   │   │   │   │   │   ├── DustSky.shader
│   │   │   │   │   │   ├── DustSky.shader.meta
│   │   │   │   │   │   ├── Glass_Utility.mat
│   │   │   │   │   │   ├── Glass_Utility.mat.meta
│   │   │   │   │   │   ├── Habitat_CompositeDark.mat
│   │   │   │   │   │   ├── Habitat_CompositeDark.mat.meta
│   │   │   │   │   │   ├── Habitat_OffWhite.mat
│   │   │   │   │   │   ├── Habitat_OffWhite.mat.meta
│   │   │   │   │   │   ├── Mars_DustSky.mat
│   │   │   │   │   │   ├── Mars_DustSky.mat.meta
│   │   │   │   │   │   ├── Mars_Regolith.mat
│   │   │   │   │   │   ├── Mars_Regolith.mat.meta
│   │   │   │   │   │   ├── Mars_Regolith_Dark.mat
│   │   │   │   │   │   ├── Mars_Regolith_Dark.mat.meta
│   │   │   │   │   │   ├── Mars_Rock_Basaltic.mat
│   │   │   │   │   │   ├── Mars_Rock_Basaltic.mat.meta
│   │   │   │   │   │   ├── Mars_Rock_Layered.mat
│   │   │   │   │   │   ├── Mars_Rock_Layered.mat.meta
│   │   │   │   │   │   ├── Metal_Industrial.mat
│   │   │   │   │   │   ├── Metal_Industrial.mat.meta
│   │   │   │   │   │   ├── Regolith_Grain_512.png
│   │   │   │   │   │   ├── Regolith_Grain_512.png.meta
│   │   │   │   │   │   ├── Regolith_Macro_Pathwear_1024.png
│   │   │   │   │   │   ├── Regolith_Macro_Pathwear_1024.png.meta
│   │   │   │   │   │   ├── Regolith_MicroNormal_256.png
│   │   │   │   │   │   ├── Regolith_MicroNormal_256.png.meta
│   │   │   │   │   │   ├── Solar_Cells_Dust_512.png
│   │   │   │   │   │   ├── Solar_Cells_Dust_512.png.meta
│   │   │   │   │   │   ├── SolarPanel.mat
│   │   │   │   │   │   └── SolarPanel.mat.meta
│   │   │   │   │   ├── Prefabs/
│   │   │   │   │   │   ├── AngularBasalt.prefab
│   │   │   │   │   │   ├── AngularBasalt.prefab.meta
│   │   │   │   │   │   ├── C_ScienceStaging.prefab
│   │   │   │   │   │   ├── C_ScienceStaging.prefab.meta
│   │   │   │   │   │   ├── ErodedLandmark.prefab
│   │   │   │   │   │   ├── ErodedLandmark.prefab.meta
│   │   │   │   │   │   ├── G_Greenhouse.prefab
│   │   │   │   │   │   ├── G_Greenhouse.prefab.meta
│   │   │   │   │   │   ├── H_CommandHabitat.prefab
│   │   │   │   │   │   ├── H_CommandHabitat.prefab.meta
│   │   │   │   │   │   ├── LayeredDelta.prefab
│   │   │   │   │   │   ├── LayeredDelta.prefab.meta
│   │   │   │   │   │   ├── P_PowerStation.prefab
│   │   │   │   │   │   ├── P_PowerStation.prefab.meta
│   │   │   │   │   │   ├── R_RoverBay.prefab
│   │   │   │   │   │   ├── R_RoverBay.prefab.meta
│   │   │   │   │   │   ├── RegolithStone.prefab
│   │   │   │   │   │   ├── RegolithStone.prefab.meta
│   │   │   │   │   │   ├── S_SolarArray.prefab
│   │   │   │   │   │   └── S_SolarArray.prefab.meta
│   │   │   │   │   ├── Props/
│   │   │   │   │   │   ├── ApproachStoneClusters_Mars_Rock_Basaltic_Combined.asset
│   │   │   │   │   │   ├── ApproachStoneClusters_Mars_Rock_Basaltic_Combined.asset.meta
│   │   │   │   │   │   ├── HabitatExternalKit_Habitat_CompositeDark_Combined.asset
│   │   │   │   │   │   ├── HabitatExternalKit_Habitat_CompositeDark_Combined.asset.meta
│   │   │   │   │   │   ├── HabitatExternalKit_Habitat_OffWhite_Combined.asset
│   │   │   │   │   │   ├── HabitatExternalKit_Habitat_OffWhite_Combined.asset.meta
│   │   │   │   │   │   ├── HabitatExternalKit_Metal_Industrial_Combined.asset
│   │   │   │   │   │   ├── HabitatExternalKit_Metal_Industrial_Combined.asset.meta
│   │   │   │   │   │   ├── PowerExternalKit_Habitat_CompositeDark_Combined.asset
│   │   │   │   │   │   ├── PowerExternalKit_Habitat_CompositeDark_Combined.asset.meta
│   │   │   │   │   │   ├── PowerExternalKit_Mars_Regolith_Dark_Combined.asset
│   │   │   │   │   │   ├── PowerExternalKit_Mars_Regolith_Dark_Combined.asset.meta
│   │   │   │   │   │   ├── UtilityRoutePosts_Accent_Cyan_Combined.asset
│   │   │   │   │   │   ├── UtilityRoutePosts_Accent_Cyan_Combined.asset.meta
│   │   │   │   │   │   ├── UtilityRoutePosts_Metal_Industrial_Combined.asset
│   │   │   │   │   │   └── UtilityRoutePosts_Metal_Industrial_Combined.asset.meta
│   │   │   │   │   ├── Rocks/
│   │   │   │   │   │   ├── AngularBasalt_LOD0.asset
│   │   │   │   │   │   ├── AngularBasalt_LOD0.asset.meta
│   │   │   │   │   │   ├── AngularBasalt_LOD1.asset
│   │   │   │   │   │   ├── AngularBasalt_LOD1.asset.meta
│   │   │   │   │   │   ├── AngularBasalt_LOD2.asset
│   │   │   │   │   │   ├── AngularBasalt_LOD2.asset.meta
│   │   │   │   │   │   ├── DeltaFractured_LOD0.asset
│   │   │   │   │   │   ├── DeltaFractured_LOD0.asset.meta
│   │   │   │   │   │   ├── DeltaFractured_LOD1.asset
│   │   │   │   │   │   ├── DeltaFractured_LOD1.asset.meta
│   │   │   │   │   │   ├── DeltaFractured_LOD2.asset
│   │   │   │   │   │   ├── DeltaFractured_LOD2.asset.meta
│   │   │   │   │   │   ├── ErodedLandmark_LOD0.asset
│   │   │   │   │   │   ├── ErodedLandmark_LOD0.asset.meta
│   │   │   │   │   │   ├── ErodedLandmark_LOD1.asset
│   │   │   │   │   │   ├── ErodedLandmark_LOD1.asset.meta
│   │   │   │   │   │   ├── ErodedLandmark_LOD2.asset
│   │   │   │   │   │   ├── ErodedLandmark_LOD2.asset.meta
│   │   │   │   │   │   ├── LayeredDelta_LOD0.asset
│   │   │   │   │   │   ├── LayeredDelta_LOD0.asset.meta
│   │   │   │   │   │   ├── LayeredDelta_LOD1.asset
│   │   │   │   │   │   ├── LayeredDelta_LOD1.asset.meta
│   │   │   │   │   │   ├── LayeredDelta_LOD2.asset
│   │   │   │   │   │   ├── LayeredDelta_LOD2.asset.meta
│   │   │   │   │   │   ├── StonePatch_0_Mars_Rock_Basaltic_Combined.asset
│   │   │   │   │   │   ├── StonePatch_0_Mars_Rock_Basaltic_Combined.asset.meta
│   │   │   │   │   │   ├── StonePatch_1_Mars_Rock_Basaltic_Combined.asset
│   │   │   │   │   │   ├── StonePatch_1_Mars_Rock_Basaltic_Combined.asset.meta
│   │   │   │   │   │   ├── StonePatch_2_Mars_Rock_Basaltic_Combined.asset
│   │   │   │   │   │   ├── StonePatch_2_Mars_Rock_Basaltic_Combined.asset.meta
│   │   │   │   │   │   ├── StonePatch_3_Mars_Rock_Basaltic_Combined.asset
│   │   │   │   │   │   ├── StonePatch_3_Mars_Rock_Basaltic_Combined.asset.meta
│   │   │   │   │   │   ├── StonePatch_4_Mars_Rock_Basaltic_Combined.asset
│   │   │   │   │   │   ├── StonePatch_4_Mars_Rock_Basaltic_Combined.asset.meta
│   │   │   │   │   │   ├── StonePatch_5_Mars_Rock_Basaltic_Combined.asset
│   │   │   │   │   │   ├── StonePatch_5_Mars_Rock_Basaltic_Combined.asset.meta
│   │   │   │   │   │   ├── StonePatch_6_Mars_Rock_Basaltic_Combined.asset
│   │   │   │   │   │   ├── StonePatch_6_Mars_Rock_Basaltic_Combined.asset.meta
│   │   │   │   │   │   ├── StonePatch_7_Mars_Rock_Basaltic_Combined.asset
│   │   │   │   │   │   ├── StonePatch_7_Mars_Rock_Basaltic_Combined.asset.meta
│   │   │   │   │   │   ├── StonePatch_8_Mars_Rock_Basaltic_Combined.asset
│   │   │   │   │   │   └── StonePatch_8_Mars_Rock_Basaltic_Combined.asset.meta
│   │   │   │   │   ├── Terrain/
│   │   │   │   │   │   ├── Regolith_250m_PreservedCorridor.asset
│   │   │   │   │   │   └── Regolith_250m_PreservedCorridor.asset.meta
│   │   │   │   │   ├── Architecture.meta
│   │   │   │   │   ├── Editor.meta
│   │   │   │   │   ├── Materials.meta
│   │   │   │   │   ├── Prefabs.meta
│   │   │   │   │   ├── Props.meta
│   │   │   │   │   ├── Rocks.meta
│   │   │   │   │   └── Terrain.meta
│   │   │   │   ├── FirstMission/
│   │   │   │   │   ├── Config/
│   │   │   │   │   │   ├── sensor_catalog_v1.json
│   │   │   │   │   │   └── sensor_catalog_v1.json.meta
│   │   │   │   │   ├── Editor/
│   │   │   │   │   │   ├── FirstMissionAndroidBuild.cs
│   │   │   │   │   │   ├── FirstMissionAndroidBuild.cs.meta
│   │   │   │   │   │   ├── FirstMissionBackendClientTest.cs
│   │   │   │   │   │   ├── FirstMissionBackendClientTest.cs.meta
│   │   │   │   │   │   ├── FirstMissionBuilder.cs
│   │   │   │   │   │   ├── FirstMissionBuilder.cs.meta
│   │   │   │   │   │   ├── FirstMissionDemoRecorder.cs
│   │   │   │   │   │   ├── FirstMissionDemoRecorder.cs.meta
│   │   │   │   │   │   ├── FirstMissionLayoutProbe.cs
│   │   │   │   │   │   ├── FirstMissionLayoutProbe.cs.meta
│   │   │   │   │   │   ├── FirstMissionOpen.cs
│   │   │   │   │   │   ├── FirstMissionOpen.cs.meta
│   │   │   │   │   │   ├── FirstMissionPlaytest.cs
│   │   │   │   │   │   ├── FirstMissionPlaytest.cs.meta
│   │   │   │   │   │   ├── MissionStructuresBuilder.cs
│   │   │   │   │   │   └── MissionStructuresBuilder.cs.meta
│   │   │   │   │   ├── Meshes/
│   │   │   │   │   │   ├── MissionTent.asset
│   │   │   │   │   │   └── MissionTent.asset.meta
│   │   │   │   │   ├── Prefabs/
│   │   │   │   │   │   ├── ARES_VI_FirstMission.prefab
│   │   │   │   │   │   └── ARES_VI_FirstMission.prefab.meta
│   │   │   │   │   ├── Scenes/
│   │   │   │   │   │   ├── Mars_FirstMission_Explorer.unity
│   │   │   │   │   │   ├── Mars_FirstMission_Explorer.unity.meta
│   │   │   │   │   │   ├── Mars_FirstMission_HUDv2.unity
│   │   │   │   │   │   ├── Mars_FirstMission_HUDv2.unity.meta
│   │   │   │   │   │   ├── Mars_FirstMission_Missions.unity
│   │   │   │   │   │   ├── Mars_FirstMission_Missions.unity.meta
│   │   │   │   │   │   ├── Mars_FirstMission_v1.unity
│   │   │   │   │   │   └── Mars_FirstMission_v1.unity.meta
│   │   │   │   │   ├── Scripts/
│   │   │   │   │   │   ├── HUD/
│   │   │   │   │   │   │   ├── ExplorerCinematics.cs
│   │   │   │   │   │   │   ├── ExplorerCinematics.cs.meta
│   │   │   │   │   │   │   ├── ExplorerHUD.cs
│   │   │   │   │   │   │   ├── ExplorerHUD.cs.meta
│   │   │   │   │   │   │   ├── HudFx.cs
│   │   │   │   │   │   │   ├── HudFx.cs.meta
│   │   │   │   │   │   │   ├── HudIcons.cs
│   │   │   │   │   │   │   ├── HudIcons.cs.meta
│   │   │   │   │   │   │   ├── HudStatus.cs
│   │   │   │   │   │   │   ├── HudStatus.cs.meta
│   │   │   │   │   │   │   ├── HudWidgets.cs
│   │   │   │   │   │   │   ├── HudWidgets.cs.meta
│   │   │   │   │   │   │   ├── MissionsHUD.cs
│   │   │   │   │   │   │   ├── MissionsHUD.cs.meta
│   │   │   │   │   │   │   ├── PremiumHUD.cs
│   │   │   │   │   │   │   └── PremiumHUD.cs.meta
│   │   │   │   │   │   ├── Missions/
│   │   │   │   │   │   │   ├── ChargingPadState.cs
│   │   │   │   │   │   │   ├── ChargingPadState.cs.meta
│   │   │   │   │   │   │   ├── CheckpointState.cs
│   │   │   │   │   │   │   ├── CheckpointState.cs.meta
│   │   │   │   │   │   │   ├── GeneratorState.cs
│   │   │   │   │   │   │   ├── GeneratorState.cs.meta
│   │   │   │   │   │   │   ├── MissionDirector.cs
│   │   │   │   │   │   │   ├── MissionDirector.cs.meta
│   │   │   │   │   │   │   ├── MissionFx.cs
│   │   │   │   │   │   │   ├── MissionFx.cs.meta
│   │   │   │   │   │   │   ├── MissionStructure.cs
│   │   │   │   │   │   │   └── MissionStructure.cs.meta
│   │   │   │   │   │   ├── Nav/
│   │   │   │   │   │   │   ├── FirstMissionNavigation.cs
│   │   │   │   │   │   │   └── FirstMissionNavigation.cs.meta
│   │   │   │   │   │   ├── AresRoverPresentation.cs
│   │   │   │   │   │   ├── AresRoverPresentation.cs.meta
│   │   │   │   │   │   ├── FirstMissionCameraRig.cs
│   │   │   │   │   │   ├── FirstMissionCameraRig.cs.meta
│   │   │   │   │   │   ├── FirstMissionControlLayer.cs
│   │   │   │   │   │   ├── FirstMissionControlLayer.cs.meta
│   │   │   │   │   │   ├── FirstMissionController.cs
│   │   │   │   │   │   ├── FirstMissionController.cs.meta
│   │   │   │   │   │   ├── FirstMissionData.cs
│   │   │   │   │   │   ├── FirstMissionData.cs.meta
│   │   │   │   │   │   ├── FirstMissionHUD.cs
│   │   │   │   │   │   ├── FirstMissionHUD.cs.meta
│   │   │   │   │   │   ├── HUD.meta
│   │   │   │   │   │   ├── Missions.meta
│   │   │   │   │   │   ├── Nav.meta
│   │   │   │   │   │   ├── RoverSensorSuite.cs
│   │   │   │   │   │   └── RoverSensorSuite.cs.meta
│   │   │   │   │   ├── Settings/
│   │   │   │   │   │   ├── FirstMissionRegistry.asset
│   │   │   │   │   │   └── FirstMissionRegistry.asset.meta
│   │   │   │   │   ├── Config.meta
│   │   │   │   │   ├── Editor.meta
│   │   │   │   │   ├── Meshes.meta
│   │   │   │   │   ├── Prefabs.meta
│   │   │   │   │   ├── Scenes.meta
│   │   │   │   │   ├── Scripts.meta
│   │   │   │   │   └── Settings.meta
│   │   │   │   ├── Greybox/
│   │   │   │   │   ├── Editor/
│   │   │   │   │   │   ├── MarsGreyboxBuilder.cs
│   │   │   │   │   │   └── MarsGreyboxBuilder.cs.meta
│   │   │   │   │   ├── Materials/
│   │   │   │   │   │   ├── DarkRock.mat
│   │   │   │   │   │   ├── DarkRock.mat.meta
│   │   │   │   │   │   ├── HabitatPale.mat
│   │   │   │   │   │   ├── HabitatPale.mat.meta
│   │   │   │   │   │   ├── IndustrialGraphite.mat
│   │   │   │   │   │   ├── IndustrialGraphite.mat.meta
│   │   │   │   │   │   ├── LayeredStone.mat
│   │   │   │   │   │   ├── LayeredStone.mat.meta
│   │   │   │   │   │   ├── SandTan.mat
│   │   │   │   │   │   ├── SandTan.mat.meta
│   │   │   │   │   │   ├── TerrainOchre.mat
│   │   │   │   │   │   ├── TerrainOchre.mat.meta
│   │   │   │   │   │   ├── WayfindingAmber.mat
│   │   │   │   │   │   ├── WayfindingAmber.mat.meta
│   │   │   │   │   │   ├── WayfindingCyan.mat
│   │   │   │   │   │   └── WayfindingCyan.mat.meta
│   │   │   │   │   ├── Meshes/
│   │   │   │   │   │   ├── Ground.asset
│   │   │   │   │   │   └── Ground.asset.meta
│   │   │   │   │   ├── Editor.meta
│   │   │   │   │   ├── MarsOutpost_Greybox.unity
│   │   │   │   │   ├── MarsOutpost_Greybox.unity.meta
│   │   │   │   │   ├── Materials.meta
│   │   │   │   │   ├── Meshes.meta
│   │   │   │   │   ├── PreGreybox_Untitled_Backup.unity
│   │   │   │   │   └── PreGreybox_Untitled_Backup.unity.meta
│   │   │   │   ├── Integration/
│   │   │   │   │   ├── Contracts/
│   │   │   │   │   │   ├── IntegrationContracts.cs
│   │   │   │   │   │   └── IntegrationContracts.cs.meta
│   │   │   │   │   ├── Http/
│   │   │   │   │   │   ├── HttpMissionSessionService.cs
│   │   │   │   │   │   └── HttpMissionSessionService.cs.meta
│   │   │   │   │   ├── Local/
│   │   │   │   │   │   ├── GameplayEventBridge.cs
│   │   │   │   │   │   ├── GameplayEventBridge.cs.meta
│   │   │   │   │   │   ├── LifeSupportModelAdapter.cs
│   │   │   │   │   │   ├── LifeSupportModelAdapter.cs.meta
│   │   │   │   │   │   ├── LocalServices.cs
│   │   │   │   │   │   └── LocalServices.cs.meta
│   │   │   │   │   ├── Contracts.meta
│   │   │   │   │   ├── Http.meta
│   │   │   │   │   └── Local.meta
│   │   │   │   ├── MarsEnvironment/
│   │   │   │   │   ├── Documentation/
│   │   │   │   │   │   ├── MissionRegistry.asset
│   │   │   │   │   │   ├── MissionRegistry.asset.meta
│   │   │   │   │   │   ├── README.md
│   │   │   │   │   │   └── README.md.meta
│   │   │   │   │   ├── Lighting/
│   │   │   │   │   │   ├── Dusty_Conditions.asset
│   │   │   │   │   │   ├── Dusty_Conditions.asset.meta
│   │   │   │   │   │   ├── MarsGradient.shader
│   │   │   │   │   │   ├── MarsGradient.shader.meta
│   │   │   │   │   │   ├── MarsSky.mat
│   │   │   │   │   │   ├── MarsSky.mat.meta
│   │   │   │   │   │   ├── MarsVolume.asset
│   │   │   │   │   │   ├── MarsVolume.asset.meta
│   │   │   │   │   │   ├── Midday.asset
│   │   │   │   │   │   ├── Midday.asset.meta
│   │   │   │   │   │   ├── Morning.asset
│   │   │   │   │   │   ├── Morning.asset.meta
│   │   │   │   │   │   ├── Sunset.asset
│   │   │   │   │   │   └── Sunset.asset.meta
│   │   │   │   │   ├── Materials/
│   │   │   │   │   │   ├── DistantDust.asset
│   │   │   │   │   │   ├── DistantDust.asset.meta
│   │   │   │   │   │   ├── Dusty solar cells.mat
│   │   │   │   │   │   ├── Dusty solar cells.mat.meta
│   │   │   │   │   │   ├── EnvironmentDust.mat
│   │   │   │   │   │   ├── EnvironmentDust.mat.meta
│   │   │   │   │   │   ├── Fine Mars dust.mat
│   │   │   │   │   │   ├── Fine Mars dust.mat.meta
│   │   │   │   │   │   ├── Habitat ceramic composite.mat
│   │   │   │   │   │   ├── Habitat ceramic composite.mat.meta
│   │   │   │   │   │   ├── Observation panels — opaque approximation.mat
│   │   │   │   │   │   ├── Observation panels — opaque approximation.mat.meta
│   │   │   │   │   │   ├── Restrained identification ochre.mat
│   │   │   │   │   │   ├── Restrained identification ochre.mat.meta
│   │   │   │   │   │   ├── Rock07 dusty basalt.mat
│   │   │   │   │   │   ├── Rock07 dusty basalt.mat.meta
│   │   │   │   │   │   ├── Rock09 compact mineral.mat
│   │   │   │   │   │   ├── Rock09 compact mineral.mat.meta
│   │   │   │   │   │   ├── Rover_Cab.mat
│   │   │   │   │   │   ├── Rover_Cab.mat.meta
│   │   │   │   │   │   ├── Rover_Chassis.mat
│   │   │   │   │   │   ├── Rover_Chassis.mat.meta
│   │   │   │   │   │   ├── Rover_RoofMarker.mat
│   │   │   │   │   │   ├── Rover_RoofMarker.mat.meta
│   │   │   │   │   │   ├── Rover_Wheel_-1_0.mat
│   │   │   │   │   │   ├── Rover_Wheel_-1_0.mat.meta
│   │   │   │   │   │   ├── Rover_Wheel_-1_1.mat
│   │   │   │   │   │   ├── Rover_Wheel_-1_1.mat.meta
│   │   │   │   │   │   ├── Rover_Wheel_-1_2.mat
│   │   │   │   │   │   ├── Rover_Wheel_-1_2.mat.meta
│   │   │   │   │   │   ├── Rover_Wheel_1_0.mat
│   │   │   │   │   │   ├── Rover_Wheel_1_0.mat.meta
│   │   │   │   │   │   ├── Rover_Wheel_1_1.mat
│   │   │   │   │   │   ├── Rover_Wheel_1_1.mat.meta
│   │   │   │   │   │   ├── Rover_Wheel_1_2.mat
│   │   │   │   │   │   ├── Rover_Wheel_1_2.mat.meta
│   │   │   │   │   │   ├── Structural graphite.mat
│   │   │   │   │   │   └── Structural graphite.mat.meta
│   │   │   │   │   ├── Models/
│   │   │   │   │   │   ├── Crew_Dome_Ribs_LowPoly.asset
│   │   │   │   │   │   ├── Crew_Dome_Ribs_LowPoly.asset.meta
│   │   │   │   │   │   ├── HabitatHemisphere.asset
│   │   │   │   │   │   ├── HabitatHemisphere.asset.meta
│   │   │   │   │   │   ├── Laboratory_Dome_Ribs_LowPoly.asset
│   │   │   │   │   │   ├── Laboratory_Dome_Ribs_LowPoly.asset.meta
│   │   │   │   │   │   ├── Life_Support_Dome_Ribs_LowPoly.asset
│   │   │   │   │   │   ├── Life_Support_Dome_Ribs_LowPoly.asset.meta
│   │   │   │   │   │   ├── rock_07_LOD0.asset
│   │   │   │   │   │   ├── rock_07_LOD0.asset.meta
│   │   │   │   │   │   ├── rock_07_LOD1.asset
│   │   │   │   │   │   ├── rock_07_LOD1.asset.meta
│   │   │   │   │   │   ├── rock_07_LOD2.asset
│   │   │   │   │   │   ├── rock_07_LOD2.asset.meta
│   │   │   │   │   │   ├── rock_09_LOD0.asset
│   │   │   │   │   │   ├── rock_09_LOD0.asset.meta
│   │   │   │   │   │   ├── rock_09_LOD1.asset
│   │   │   │   │   │   ├── rock_09_LOD1.asset.meta
│   │   │   │   │   │   ├── rock_09_LOD2.asset
│   │   │   │   │   │   ├── rock_09_LOD2.asset.meta
│   │   │   │   │   │   ├── Terrain250m.asset
│   │   │   │   │   │   └── Terrain250m.asset.meta
│   │   │   │   │   ├── Prefabs/
│   │   │   │   │   │   ├── ARESYN_ClosedHabitatComplex_Optimized.prefab
│   │   │   │   │   │   ├── ARESYN_ClosedHabitatComplex_Optimized.prefab.meta
│   │   │   │   │   │   ├── SolarFarm_Optimized.prefab
│   │   │   │   │   │   └── SolarFarm_Optimized.prefab.meta
│   │   │   │   │   ├── Scenes/
│   │   │   │   │   │   ├── Mars_RealisticResearchBase_v1.unity
│   │   │   │   │   │   └── Mars_RealisticResearchBase_v1.unity.meta
│   │   │   │   │   ├── Scripts/
│   │   │   │   │   │   ├── EnvironmentPresentation.cs
│   │   │   │   │   │   ├── EnvironmentPresentation.cs.meta
│   │   │   │   │   │   ├── EnvironmentPreset.cs
│   │   │   │   │   │   ├── EnvironmentPreset.cs.meta
│   │   │   │   │   │   ├── ResearchTarget.cs
│   │   │   │   │   │   └── ResearchTarget.cs.meta
│   │   │   │   │   ├── Tests/
│   │   │   │   │   │   ├── Editor/
│   │   │   │   │   │   │   ├── MarsResearchBaseBuilder.cs
│   │   │   │   │   │   │   └── MarsResearchBaseBuilder.cs.meta
│   │   │   │   │   │   └── Editor.meta
│   │   │   │   │   ├── Textures/
│   │   │   │   │   │   ├── rock_07_AO.png
│   │   │   │   │   │   ├── rock_07_AO.png.meta
│   │   │   │   │   │   ├── rock_07_Diffuse.png
│   │   │   │   │   │   ├── rock_07_Diffuse.png.meta
│   │   │   │   │   │   ├── rock_07_nor_gl.png
│   │   │   │   │   │   ├── rock_07_nor_gl.png.meta
│   │   │   │   │   │   ├── rock_07_Rough.png
│   │   │   │   │   │   ├── rock_07_Rough.png.meta
│   │   │   │   │   │   ├── rock_09_AO.png
│   │   │   │   │   │   ├── rock_09_AO.png.meta
│   │   │   │   │   │   ├── rock_09_Diffuse.png
│   │   │   │   │   │   ├── rock_09_Diffuse.png.meta
│   │   │   │   │   │   ├── rock_09_nor_gl.png
│   │   │   │   │   │   ├── rock_09_nor_gl.png.meta
│   │   │   │   │   │   ├── rock_09_Rough.png
│   │   │   │   │   │   ├── rock_09_Rough.png.meta
│   │   │   │   │   │   ├── sandy_gravel_AO.png
│   │   │   │   │   │   ├── sandy_gravel_AO.png.meta
│   │   │   │   │   │   ├── sandy_gravel_Diffuse.png
│   │   │   │   │   │   ├── sandy_gravel_Diffuse.png.meta
│   │   │   │   │   │   ├── sandy_gravel_nor_gl.png
│   │   │   │   │   │   ├── sandy_gravel_nor_gl.png.meta
│   │   │   │   │   │   ├── sandy_gravel_Rough.png
│   │   │   │   │   │   └── sandy_gravel_Rough.png.meta
│   │   │   │   │   ├── Documentation.meta
│   │   │   │   │   ├── Lighting.meta
│   │   │   │   │   ├── Materials.meta
│   │   │   │   │   ├── Models.meta
│   │   │   │   │   ├── Prefabs.meta
│   │   │   │   │   ├── Scenes.meta
│   │   │   │   │   ├── Scripts.meta
│   │   │   │   │   ├── Tests.meta
│   │   │   │   │   ├── Textures.meta
│   │   │   │   │   └── VFX.meta
│   │   │   │   ├── MarsVisualPolish/
│   │   │   │   │   ├── Documentation/
│   │   │   │   │   │   ├── PolishRegistry.asset
│   │   │   │   │   │   └── PolishRegistry.asset.meta
│   │   │   │   │   ├── Editor/
│   │   │   │   │   │   ├── VisualPolishBuilder.cs
│   │   │   │   │   │   └── VisualPolishBuilder.cs.meta
│   │   │   │   │   ├── Lighting/
│   │   │   │   │   │   ├── PolishSky.mat
│   │   │   │   │   │   ├── PolishSky.mat.meta
│   │   │   │   │   │   ├── PolishVolume.asset
│   │   │   │   │   │   ├── PolishVolume.asset.meta
│   │   │   │   │   │   ├── Preset_0.asset
│   │   │   │   │   │   ├── Preset_0.asset.meta
│   │   │   │   │   │   ├── Preset_1.asset
│   │   │   │   │   │   ├── Preset_1.asset.meta
│   │   │   │   │   │   ├── Preset_2.asset
│   │   │   │   │   │   ├── Preset_2.asset.meta
│   │   │   │   │   │   ├── Preset_3.asset
│   │   │   │   │   │   └── Preset_3.asset.meta
│   │   │   │   │   ├── Materials/
│   │   │   │   │   │   ├── Brushed frame.mat
│   │   │   │   │   │   ├── Brushed frame.mat.meta
│   │   │   │   │   │   ├── Derivative_0.mat
│   │   │   │   │   │   ├── Derivative_0.mat.meta
│   │   │   │   │   │   ├── Derivative_1.mat
│   │   │   │   │   │   ├── Derivative_1.mat.meta
│   │   │   │   │   │   ├── Derivative_10.mat
│   │   │   │   │   │   ├── Derivative_10.mat.meta
│   │   │   │   │   │   ├── Derivative_11.mat
│   │   │   │   │   │   ├── Derivative_11.mat.meta
│   │   │   │   │   │   ├── Derivative_12.mat
│   │   │   │   │   │   ├── Derivative_12.mat.meta
│   │   │   │   │   │   ├── Derivative_13.mat
│   │   │   │   │   │   ├── Derivative_13.mat.meta
│   │   │   │   │   │   ├── Derivative_14.mat
│   │   │   │   │   │   ├── Derivative_14.mat.meta
│   │   │   │   │   │   ├── Derivative_15.mat
│   │   │   │   │   │   ├── Derivative_15.mat.meta
│   │   │   │   │   │   ├── Derivative_16.mat
│   │   │   │   │   │   ├── Derivative_16.mat.meta
│   │   │   │   │   │   ├── Derivative_17.mat
│   │   │   │   │   │   ├── Derivative_17.mat.meta
│   │   │   │   │   │   ├── Derivative_18.mat
│   │   │   │   │   │   ├── Derivative_18.mat.meta
│   │   │   │   │   │   ├── Derivative_19.mat
│   │   │   │   │   │   ├── Derivative_19.mat.meta
│   │   │   │   │   │   ├── Derivative_2.mat
│   │   │   │   │   │   ├── Derivative_2.mat.meta
│   │   │   │   │   │   ├── Derivative_20.mat
│   │   │   │   │   │   ├── Derivative_20.mat.meta
│   │   │   │   │   │   ├── Derivative_21.mat
│   │   │   │   │   │   ├── Derivative_21.mat.meta
│   │   │   │   │   │   ├── Derivative_3.mat
│   │   │   │   │   │   ├── Derivative_3.mat.meta
│   │   │   │   │   │   ├── Derivative_4.mat
│   │   │   │   │   │   ├── Derivative_4.mat.meta
│   │   │   │   │   │   ├── Derivative_5.mat
│   │   │   │   │   │   ├── Derivative_5.mat.meta
│   │   │   │   │   │   ├── Derivative_6.mat
│   │   │   │   │   │   ├── Derivative_6.mat.meta
│   │   │   │   │   │   ├── Derivative_7.mat
│   │   │   │   │   │   ├── Derivative_7.mat.meta
│   │   │   │   │   │   ├── Derivative_8.mat
│   │   │   │   │   │   ├── Derivative_8.mat.meta
│   │   │   │   │   │   ├── Derivative_9.mat
│   │   │   │   │   │   ├── Derivative_9.mat.meta
│   │   │   │   │   │   ├── Photovoltaic cell laminate.mat
│   │   │   │   │   │   ├── Photovoltaic cell laminate.mat.meta
│   │   │   │   │   │   ├── PhotovoltaicGrid.png
│   │   │   │   │   │   ├── PhotovoltaicGrid.png.meta
│   │   │   │   │   │   ├── Recessed seals.mat
│   │   │   │   │   │   ├── Recessed seals.mat.meta
│   │   │   │   │   │   ├── Safety ochre.mat
│   │   │   │   │   │   ├── Safety ochre.mat.meta
│   │   │   │   │   │   ├── Warm ceramic.mat
│   │   │   │   │   │   └── Warm ceramic.mat.meta
│   │   │   │   │   ├── Models/
│   │   │   │   │   │   ├── Crew Dome_Frame.asset
│   │   │   │   │   │   ├── Crew Dome_Frame.asset.meta
│   │   │   │   │   │   ├── Laboratory Dome_Frame.asset
│   │   │   │   │   │   ├── Laboratory Dome_Frame.asset.meta
│   │   │   │   │   │   ├── Life Support Dome_Frame.asset
│   │   │   │   │   │   └── Life Support Dome_Frame.asset.meta
│   │   │   │   │   ├── Scenes/
│   │   │   │   │   │   ├── Mars_VisualPolish_v2.unity
│   │   │   │   │   │   └── Mars_VisualPolish_v2.unity.meta
│   │   │   │   │   ├── Documentation.meta
│   │   │   │   │   ├── Editor.meta
│   │   │   │   │   ├── Lighting.meta
│   │   │   │   │   ├── Materials.meta
│   │   │   │   │   ├── Models.meta
│   │   │   │   │   ├── PolishBenchmark.cs
│   │   │   │   │   ├── PolishBenchmark.cs.meta
│   │   │   │   │   ├── PolishHudTheme.cs
│   │   │   │   │   ├── PolishHudTheme.cs.meta
│   │   │   │   │   ├── Scenes.meta
│   │   │   │   │   ├── VisualPolishPresentation.cs
│   │   │   │   │   └── VisualPolishPresentation.cs.meta
│   │   │   │   ├── Mission01Milestone/
│   │   │   │   │   ├── Editor/
│   │   │   │   │   │   ├── Mission01Builder.cs
│   │   │   │   │   │   └── Mission01Builder.cs.meta
│   │   │   │   │   ├── Materials/
│   │   │   │   │   │   ├── Mission01_Continents.mat
│   │   │   │   │   │   ├── Mission01_Continents.mat.meta
│   │   │   │   │   │   ├── Mission01_Graphite.mat
│   │   │   │   │   │   ├── Mission01_Graphite.mat.meta
│   │   │   │   │   │   ├── Mission01_Ocean.mat
│   │   │   │   │   │   ├── Mission01_Ocean.mat.meta
│   │   │   │   │   │   ├── Mission01_Pale.mat
│   │   │   │   │   │   ├── Mission01_Pale.mat.meta
│   │   │   │   │   │   ├── Mission01_Safety.mat
│   │   │   │   │   │   └── Mission01_Safety.mat.meta
│   │   │   │   │   ├── Prefabs/
│   │   │   │   │   │   ├── Boarding_Console.prefab
│   │   │   │   │   │   ├── Boarding_Console.prefab.meta
│   │   │   │   │   │   ├── Trainee_Blockout.prefab
│   │   │   │   │   │   └── Trainee_Blockout.prefab.meta
│   │   │   │   │   ├── Editor.meta
│   │   │   │   │   ├── Materials.meta
│   │   │   │   │   ├── Mission01_EarthLaunch_Milestone_v4.unity
│   │   │   │   │   ├── Mission01_EarthLaunch_Milestone_v4.unity.meta
│   │   │   │   │   ├── Mission01Flow.cs
│   │   │   │   │   ├── Mission01Flow.cs.meta
│   │   │   │   │   └── Prefabs.meta
│   │   │   │   ├── MissionIntegration/
│   │   │   │   │   ├── Editor/
│   │   │   │   │   │   ├── MissionIntegrationBuilder.cs
│   │   │   │   │   │   └── MissionIntegrationBuilder.cs.meta
│   │   │   │   │   ├── Scenes/
│   │   │   │   │   │   ├── Mars_MissionIntegration_v1.unity
│   │   │   │   │   │   └── Mars_MissionIntegration_v1.unity.meta
│   │   │   │   │   ├── Scripts/
│   │   │   │   │   │   ├── MissionIntegrationController.cs
│   │   │   │   │   │   └── MissionIntegrationController.cs.meta
│   │   │   │   │   ├── Editor.meta
│   │   │   │   │   ├── Scenes.meta
│   │   │   │   │   └── Scripts.meta
│   │   │   │   ├── MissionRules/
│   │   │   │   │   ├── Config/
│   │   │   │   │   │   ├── mission_rules_v1.json
│   │   │   │   │   │   ├── mission_rules_v1.json.meta
│   │   │   │   │   │   ├── test_vectors_v1.json
│   │   │   │   │   │   └── test_vectors_v1.json.meta
│   │   │   │   │   ├── Scripts/
│   │   │   │   │   │   ├── MiniJson.cs
│   │   │   │   │   │   ├── MiniJson.cs.meta
│   │   │   │   │   │   ├── MissionRules.cs
│   │   │   │   │   │   └── MissionRules.cs.meta
│   │   │   │   │   ├── Config.meta
│   │   │   │   │   └── Scripts.meta
│   │   │   │   ├── NativeControlSlice/
│   │   │   │   │   ├── Editor/
│   │   │   │   │   │   ├── NativeSliceBuilder.cs
│   │   │   │   │   │   └── NativeSliceBuilder.cs.meta
│   │   │   │   │   ├── Prefabs/
│   │   │   │   │   │   ├── AdaptiveTouch.prefab
│   │   │   │   │   │   └── AdaptiveTouch.prefab.meta
│   │   │   │   │   ├── AdaptiveTouch.cs
│   │   │   │   │   ├── AdaptiveTouch.cs.meta
│   │   │   │   │   ├── Editor.meta
│   │   │   │   │   ├── Mars_NativeControl_VerticalSlice.unity
│   │   │   │   │   ├── Mars_NativeControl_VerticalSlice.unity.meta
│   │   │   │   │   ├── MissionRegistry.asset
│   │   │   │   │   ├── MissionRegistry.asset.meta
│   │   │   │   │   ├── NativeSliceGame.cs
│   │   │   │   │   ├── NativeSliceGame.cs.meta
│   │   │   │   │   ├── NativeSystems.cs
│   │   │   │   │   ├── NativeSystems.cs.meta
│   │   │   │   │   └── Prefabs.meta
│   │   │   │   ├── ProductionVisual/
│   │   │   │   │   ├── Materials/
│   │   │   │   │   │   ├── Brushed aluminum.mat
│   │   │   │   │   │   ├── Brushed aluminum.mat.meta
│   │   │   │   │   │   ├── Dusty tire elastomer.mat
│   │   │   │   │   │   ├── Dusty tire elastomer.mat.meta
│   │   │   │   │   │   ├── Inherited_0.mat
│   │   │   │   │   │   ├── Inherited_0.mat.meta
│   │   │   │   │   │   ├── Inherited_1.mat
│   │   │   │   │   │   ├── Inherited_1.mat.meta
│   │   │   │   │   │   ├── Inherited_10.mat
│   │   │   │   │   │   ├── Inherited_10.mat.meta
│   │   │   │   │   │   ├── Inherited_11.mat
│   │   │   │   │   │   ├── Inherited_11.mat.meta
│   │   │   │   │   │   ├── Inherited_12.mat
│   │   │   │   │   │   ├── Inherited_12.mat.meta
│   │   │   │   │   │   ├── Inherited_13.mat
│   │   │   │   │   │   ├── Inherited_13.mat.meta
│   │   │   │   │   │   ├── Inherited_14.mat
│   │   │   │   │   │   ├── Inherited_14.mat.meta
│   │   │   │   │   │   ├── Inherited_15.mat
│   │   │   │   │   │   ├── Inherited_15.mat.meta
│   │   │   │   │   │   ├── Inherited_16.mat
│   │   │   │   │   │   ├── Inherited_16.mat.meta
│   │   │   │   │   │   ├── Inherited_17.mat
│   │   │   │   │   │   ├── Inherited_17.mat.meta
│   │   │   │   │   │   ├── Inherited_18.mat
│   │   │   │   │   │   ├── Inherited_18.mat.meta
│   │   │   │   │   │   ├── Inherited_19.mat
│   │   │   │   │   │   ├── Inherited_19.mat.meta
│   │   │   │   │   │   ├── Inherited_2.mat
│   │   │   │   │   │   ├── Inherited_2.mat.meta
│   │   │   │   │   │   ├── Inherited_20.mat
│   │   │   │   │   │   ├── Inherited_20.mat.meta
│   │   │   │   │   │   ├── Inherited_21.mat
│   │   │   │   │   │   ├── Inherited_21.mat.meta
│   │   │   │   │   │   ├── Inherited_22.mat
│   │   │   │   │   │   ├── Inherited_22.mat.meta
│   │   │   │   │   │   ├── Inherited_23.mat
│   │   │   │   │   │   ├── Inherited_23.mat.meta
│   │   │   │   │   │   ├── Inherited_24.mat
│   │   │   │   │   │   ├── Inherited_24.mat.meta
│   │   │   │   │   │   ├── Inherited_25.mat
│   │   │   │   │   │   ├── Inherited_25.mat.meta
│   │   │   │   │   │   ├── Inherited_3.mat
│   │   │   │   │   │   ├── Inherited_3.mat.meta
│   │   │   │   │   │   ├── Inherited_4.mat
│   │   │   │   │   │   ├── Inherited_4.mat.meta
│   │   │   │   │   │   ├── Inherited_5.mat
│   │   │   │   │   │   ├── Inherited_5.mat.meta
│   │   │   │   │   │   ├── Inherited_6.mat
│   │   │   │   │   │   ├── Inherited_6.mat.meta
│   │   │   │   │   │   ├── Inherited_7.mat
│   │   │   │   │   │   ├── Inherited_7.mat.meta
│   │   │   │   │   │   ├── Inherited_8.mat
│   │   │   │   │   │   ├── Inherited_8.mat.meta
│   │   │   │   │   │   ├── Inherited_9.mat
│   │   │   │   │   │   ├── Inherited_9.mat.meta
│   │   │   │   │   │   ├── Machined graphite alloy.mat
│   │   │   │   │   │   ├── Machined graphite alloy.mat.meta
│   │   │   │   │   │   ├── Opaque protective visor glazing.mat
│   │   │   │   │   │   ├── Opaque protective visor glazing.mat.meta
│   │   │   │   │   │   ├── Pressure shell ceramic composite.mat
│   │   │   │   │   │   ├── Pressure shell ceramic composite.mat.meta
│   │   │   │   │   │   ├── Restrained identification orange.mat
│   │   │   │   │   │   └── Restrained identification orange.mat.meta
│   │   │   │   │   ├── Meshes/
│   │   │   │   │   │   ├── ChamferedEnclosure.asset
│   │   │   │   │   │   ├── ChamferedEnclosure.asset.meta
│   │   │   │   │   │   ├── CommsTruss.asset
│   │   │   │   │   │   ├── CommsTruss.asset.meta
│   │   │   │   │   │   ├── Crew_Dome_sealed_airlock_Rails.asset
│   │   │   │   │   │   ├── Crew_Dome_sealed_airlock_Rails.asset.meta
│   │   │   │   │   │   ├── CrewCabEnclosure.asset
│   │   │   │   │   │   ├── CrewCabEnclosure.asset.meta
│   │   │   │   │   │   ├── GeodesicFrame.asset
│   │   │   │   │   │   ├── GeodesicFrame.asset.meta
│   │   │   │   │   │   ├── GeodesicPanels.asset
│   │   │   │   │   │   ├── GeodesicPanels.asset.meta
│   │   │   │   │   │   ├── Laboratory_Dome_sealed_airlock_Rails.asset
│   │   │   │   │   │   ├── Laboratory_Dome_sealed_airlock_Rails.asset.meta
│   │   │   │   │   │   ├── Life_Support_Dome_sealed_airlock_Rails.asset
│   │   │   │   │   │   ├── Life_Support_Dome_sealed_airlock_Rails.asset.meta
│   │   │   │   │   │   ├── RoverMechanicalFrame.asset
│   │   │   │   │   │   ├── RoverMechanicalFrame.asset.meta
│   │   │   │   │   │   ├── RoverWheel_LOD0.asset
│   │   │   │   │   │   ├── RoverWheel_LOD0.asset.meta
│   │   │   │   │   │   ├── RoverWheel_LOD1.asset
│   │   │   │   │   │   ├── RoverWheel_LOD1.asset.meta
│   │   │   │   │   │   ├── RoverWheel_LOD2.asset
│   │   │   │   │   │   └── RoverWheel_LOD2.asset.meta
│   │   │   │   │   ├── Scenes/
│   │   │   │   │   │   ├── Mars_ProductionVisual_v1.unity
│   │   │   │   │   │   └── Mars_ProductionVisual_v1.unity.meta
│   │   │   │   │   ├── Scripts/
│   │   │   │   │   │   ├── ProductionCameraRig.cs
│   │   │   │   │   │   ├── ProductionCameraRig.cs.meta
│   │   │   │   │   │   ├── ProductionModeCoordinator.cs
│   │   │   │   │   │   ├── ProductionModeCoordinator.cs.meta
│   │   │   │   │   │   ├── ProductionPerformanceProbe.cs
│   │   │   │   │   │   ├── ProductionPerformanceProbe.cs.meta
│   │   │   │   │   │   ├── ProductionPresentation.cs
│   │   │   │   │   │   └── ProductionPresentation.cs.meta
│   │   │   │   │   ├── Settings/
│   │   │   │   │   │   ├── Lighting_0.asset
│   │   │   │   │   │   ├── Lighting_0.asset.meta
│   │   │   │   │   │   ├── Lighting_1.asset
│   │   │   │   │   │   ├── Lighting_1.asset.meta
│   │   │   │   │   │   ├── Lighting_2.asset
│   │   │   │   │   │   ├── Lighting_2.asset.meta
│   │   │   │   │   │   ├── Lighting_3.asset
│   │   │   │   │   │   ├── Lighting_3.asset.meta
│   │   │   │   │   │   ├── ProductionMissionRegistry.asset
│   │   │   │   │   │   └── ProductionMissionRegistry.asset.meta
│   │   │   │   │   ├── Tests/
│   │   │   │   │   │   ├── Editor/
│   │   │   │   │   │   │   ├── ProductionMeshFactory.cs
│   │   │   │   │   │   │   ├── ProductionMeshFactory.cs.meta
│   │   │   │   │   │   │   ├── ProductionViewport.cs
│   │   │   │   │   │   │   ├── ProductionViewport.cs.meta
│   │   │   │   │   │   │   ├── ProductionVisualBuilder.cs
│   │   │   │   │   │   │   └── ProductionVisualBuilder.cs.meta
│   │   │   │   │   │   └── Editor.meta
│   │   │   │   │   ├── Textures/
│   │   │   │   │   │   ├── rock_07_MetallicSmoothness.png
│   │   │   │   │   │   ├── rock_07_MetallicSmoothness.png.meta
│   │   │   │   │   │   ├── rock_09_MetallicSmoothness.png
│   │   │   │   │   │   ├── rock_09_MetallicSmoothness.png.meta
│   │   │   │   │   │   ├── sandy_gravel_MetallicSmoothness.png
│   │   │   │   │   │   └── sandy_gravel_MetallicSmoothness.png.meta
│   │   │   │   │   ├── Themes/
│   │   │   │   │   │   ├── ProductionHUDTheme.asset
│   │   │   │   │   │   ├── ProductionHUDTheme.asset.meta
│   │   │   │   │   │   ├── ProductionSky.mat
│   │   │   │   │   │   ├── ProductionSky.mat.meta
│   │   │   │   │   │   ├── ProductionVolume.asset
│   │   │   │   │   │   └── ProductionVolume.asset.meta
│   │   │   │   │   ├── Documentation.meta
│   │   │   │   │   ├── Materials.meta
│   │   │   │   │   ├── Meshes.meta
│   │   │   │   │   ├── Prefabs.meta
│   │   │   │   │   ├── Scenes.meta
│   │   │   │   │   ├── Scripts.meta
│   │   │   │   │   ├── Settings.meta
│   │   │   │   │   ├── Tests.meta
│   │   │   │   │   ├── Textures.meta
│   │   │   │   │   └── Themes.meta
│   │   │   │   ├── Scenes/
│   │   │   │   │   ├── MarsOutpost_Environment_v01.unity
│   │   │   │   │   └── MarsOutpost_Environment_v01.unity.meta
│   │   │   │   ├── Stage08B/
│   │   │   │   │   ├── Animation/
│   │   │   │   │   │   ├── Idle.anim
│   │   │   │   │   │   ├── Idle.anim.meta
│   │   │   │   │   │   ├── PrototypeLocomotion.controller
│   │   │   │   │   │   ├── PrototypeLocomotion.controller.meta
│   │   │   │   │   │   ├── Run.anim
│   │   │   │   │   │   ├── Run.anim.meta
│   │   │   │   │   │   ├── Walk.anim
│   │   │   │   │   │   └── Walk.anim.meta
│   │   │   │   │   ├── Editor/
│   │   │   │   │   │   ├── Stage08BBuilder.cs
│   │   │   │   │   │   └── Stage08BBuilder.cs.meta
│   │   │   │   │   ├── Materials/
│   │   │   │   │   │   ├── 08B_Accent_Cyan.mat
│   │   │   │   │   │   ├── 08B_Accent_Cyan.mat.meta
│   │   │   │   │   │   ├── 08B_Accent_SafetyOrange.mat
│   │   │   │   │   │   ├── 08B_Accent_SafetyOrange.mat.meta
│   │   │   │   │   │   ├── 08B_Glass_Utility.mat
│   │   │   │   │   │   ├── 08B_Glass_Utility.mat.meta
│   │   │   │   │   │   ├── 08B_Habitat_CompositeDark.mat
│   │   │   │   │   │   ├── 08B_Habitat_CompositeDark.mat.meta
│   │   │   │   │   │   ├── 08B_Habitat_OffWhite.mat
│   │   │   │   │   │   ├── 08B_Habitat_OffWhite.mat.meta
│   │   │   │   │   │   ├── 08B_Mars_Regolith.mat
│   │   │   │   │   │   ├── 08B_Mars_Regolith.mat.meta
│   │   │   │   │   │   ├── 08B_Mars_Regolith_Dark.mat
│   │   │   │   │   │   ├── 08B_Mars_Regolith_Dark.mat.meta
│   │   │   │   │   │   ├── 08B_Mars_Rock_Basaltic.mat
│   │   │   │   │   │   ├── 08B_Mars_Rock_Basaltic.mat.meta
│   │   │   │   │   │   ├── 08B_Mars_Rock_Layered.mat
│   │   │   │   │   │   ├── 08B_Mars_Rock_Layered.mat.meta
│   │   │   │   │   │   ├── 08B_Metal_Industrial.mat
│   │   │   │   │   │   ├── 08B_Metal_Industrial.mat.meta
│   │   │   │   │   │   ├── 08B_SolarPanel.mat
│   │   │   │   │   │   ├── 08B_SolarPanel.mat.meta
│   │   │   │   │   │   ├── BasaltVariationA.mat
│   │   │   │   │   │   ├── BasaltVariationA.mat.meta
│   │   │   │   │   │   ├── BasaltVariationB.mat
│   │   │   │   │   │   ├── BasaltVariationB.mat.meta
│   │   │   │   │   │   ├── BasaltVariationC.mat
│   │   │   │   │   │   ├── BasaltVariationC.mat.meta
│   │   │   │   │   │   ├── Dust_Particle.mat
│   │   │   │   │   │   ├── Dust_Particle.mat.meta
│   │   │   │   │   │   ├── Identification_Cyan.mat
│   │   │   │   │   │   ├── Identification_Cyan.mat.meta
│   │   │   │   │   │   ├── Safety_Orange.mat
│   │   │   │   │   │   ├── Safety_Orange.mat.meta
│   │   │   │   │   │   ├── Suit_Graphite.mat
│   │   │   │   │   │   ├── Suit_Graphite.mat.meta
│   │   │   │   │   │   ├── Suit_Pale.mat
│   │   │   │   │   │   └── Suit_Pale.mat.meta
│   │   │   │   │   ├── Animation.meta
│   │   │   │   │   ├── Editor.meta
│   │   │   │   │   ├── MarsOutpost_Playable_Stage08B.unity
│   │   │   │   │   ├── MarsOutpost_Playable_Stage08B.unity.meta
│   │   │   │   │   ├── MarsPrototype.cs
│   │   │   │   │   ├── MarsPrototype.cs.meta
│   │   │   │   │   ├── Materials.meta
│   │   │   │   │   ├── Stage08B_MobilePreview.asset
│   │   │   │   │   └── Stage08B_MobilePreview.asset.meta
│   │   │   │   ├── Stage08C/
│   │   │   │   │   ├── Editor/
│   │   │   │   │   │   ├── AndroidProbeBuild.cs
│   │   │   │   │   │   ├── AndroidProbeBuild.cs.meta
│   │   │   │   │   │   ├── TouchChecks.cs
│   │   │   │   │   │   └── TouchChecks.cs.meta
│   │   │   │   │   ├── AndroidProfileProbe.cs
│   │   │   │   │   ├── AndroidProfileProbe.cs.meta
│   │   │   │   │   ├── Editor.meta
│   │   │   │   │   ├── MobileControls.cs
│   │   │   │   │   └── MobileControls.cs.meta
│   │   │   │   ├── Stage08C1/
│   │   │   │   │   ├── Editor/
│   │   │   │   │   │   ├── EditorPerformanceObserver.cs
│   │   │   │   │   │   ├── EditorPerformanceObserver.cs.meta
│   │   │   │   │   │   ├── ManualInputObserver.cs
│   │   │   │   │   │   └── ManualInputObserver.cs.meta
│   │   │   │   │   └── Editor.meta
│   │   │   │   ├── VehicleShowroom/
│   │   │   │   │   ├── ARES_VI/
│   │   │   │   │   │   ├── Materials/
│   │   │   │   │   │   │   ├── ARES_Mat_CanopyGlass.mat
│   │   │   │   │   │   │   ├── ARES_Mat_CanopyGlass.mat.meta
│   │   │   │   │   │   │   ├── ARES_Mat_CarbonTreads.mat
│   │   │   │   │   │   │   ├── ARES_Mat_CarbonTreads.mat.meta
│   │   │   │   │   │   │   ├── ARES_Mat_CeramicWhite.mat
│   │   │   │   │   │   │   ├── ARES_Mat_CeramicWhite.mat.meta
│   │   │   │   │   │   │   ├── ARES_Mat_CyanEnergy.mat
│   │   │   │   │   │   │   ├── ARES_Mat_CyanEnergy.mat.meta
│   │   │   │   │   │   │   ├── ARES_Mat_GoldFoil.mat
│   │   │   │   │   │   │   ├── ARES_Mat_GoldFoil.mat.meta
│   │   │   │   │   │   │   ├── ARES_Mat_GraphiteTrim.mat
│   │   │   │   │   │   │   ├── ARES_Mat_GraphiteTrim.mat.meta
│   │   │   │   │   │   │   ├── ARES_Mat_MartianOrange.mat
│   │   │   │   │   │   │   ├── ARES_Mat_MartianOrange.mat.meta
│   │   │   │   │   │   │   ├── ARES_Mat_TitaniumFrame.mat
│   │   │   │   │   │   │   ├── ARES_Mat_TitaniumFrame.mat.meta
│   │   │   │   │   │   │   ├── Showroom_Floor.mat
│   │   │   │   │   │   │   └── Showroom_Floor.mat.meta
│   │   │   │   │   │   ├── Source/
│   │   │   │   │   │   │   ├── ares_vi_aaa_lod1.obj
│   │   │   │   │   │   │   ├── ares_vi_aaa_lod1.obj.meta
│   │   │   │   │   │   │   ├── ares_vi_aaa_lod2.obj
│   │   │   │   │   │   │   ├── ares_vi_aaa_lod2.obj.meta
│   │   │   │   │   │   │   ├── ares_vi_aaa_master.glb
│   │   │   │   │   │   │   ├── ares_vi_aaa_master.glb.meta
│   │   │   │   │   │   │   ├── ares_vi_aaa_master.mtl
│   │   │   │   │   │   │   ├── ares_vi_aaa_master.mtl.meta
│   │   │   │   │   │   │   ├── ares_vi_aaa_master.obj
│   │   │   │   │   │   │   └── ares_vi_aaa_master.obj.meta
│   │   │   │   │   │   ├── Upgraded/
│   │   │   │   │   │   │   ├── Animation/
│   │   │   │   │   │   │   │   ├── ARES_ArmDeploy.anim
│   │   │   │   │   │   │   │   ├── ARES_ArmDeploy.anim.meta
│   │   │   │   │   │   │   │   ├── ARES_ChemCamScan.anim
│   │   │   │   │   │   │   │   ├── ARES_ChemCamScan.anim.meta
│   │   │   │   │   │   │   │   ├── ARES_HoverActivation.anim
│   │   │   │   │   │   │   │   ├── ARES_HoverActivation.anim.meta
│   │   │   │   │   │   │   │   ├── ARES_HoverStabilization.anim
│   │   │   │   │   │   │   │   ├── ARES_HoverStabilization.anim.meta
│   │   │   │   │   │   │   │   ├── ARES_Landing.anim
│   │   │   │   │   │   │   │   ├── ARES_Landing.anim.meta
│   │   │   │   │   │   │   │   ├── ARES_RockerBogie.anim
│   │   │   │   │   │   │   │   ├── ARES_RockerBogie.anim.meta
│   │   │   │   │   │   │   │   ├── ARES_VI_Upgraded.controller
│   │   │   │   │   │   │   │   ├── ARES_VI_Upgraded.controller.meta
│   │   │   │   │   │   │   │   ├── ARES_WheelRotation.anim
│   │   │   │   │   │   │   │   └── ARES_WheelRotation.anim.meta
│   │   │   │   │   │   │   ├── Materials/
│   │   │   │   │   │   │   │   ├── ARES2_CyanEmissive.mat
│   │   │   │   │   │   │   │   ├── ARES2_CyanEmissive.mat.meta
│   │   │   │   │   │   │   │   ├── ARES2_Glass.mat
│   │   │   │   │   │   │   │   ├── ARES2_Glass.mat.meta
│   │   │   │   │   │   │   │   ├── ARES2_GoldFoil.mat
│   │   │   │   │   │   │   │   ├── ARES2_GoldFoil.mat.meta
│   │   │   │   │   │   │   │   ├── ARES2_Graphite.mat
│   │   │   │   │   │   │   │   ├── ARES2_Graphite.mat.meta
│   │   │   │   │   │   │   │   ├── ARES2_HoverPlasma.mat
│   │   │   │   │   │   │   │   ├── ARES2_HoverPlasma.mat.meta
│   │   │   │   │   │   │   │   ├── ARES2_LED.mat
│   │   │   │   │   │   │   │   ├── ARES2_LED.mat.meta
│   │   │   │   │   │   │   │   ├── ARES2_MarsRegolith.mat
│   │   │   │   │   │   │   │   ├── ARES2_MarsRegolith.mat.meta
│   │   │   │   │   │   │   │   ├── ARES2_MarsSky.mat
│   │   │   │   │   │   │   │   ├── ARES2_MarsSky.mat.meta
│   │   │   │   │   │   │   │   ├── ARES2_PaintOrange.mat
│   │   │   │   │   │   │   │   ├── ARES2_PaintOrange.mat.meta
│   │   │   │   │   │   │   │   ├── ARES2_PaintWhite.mat
│   │   │   │   │   │   │   │   ├── ARES2_PaintWhite.mat.meta
│   │   │   │   │   │   │   │   ├── ARES2_Plume.mat
│   │   │   │   │   │   │   │   ├── ARES2_Plume.mat.meta
│   │   │   │   │   │   │   │   ├── ARES2_Rubber.mat
│   │   │   │   │   │   │   │   ├── ARES2_Rubber.mat.meta
│   │   │   │   │   │   │   │   ├── ARES2_Titanium.mat
│   │   │   │   │   │   │   │   └── ARES2_Titanium.mat.meta
│   │   │   │   │   │   │   ├── Textures/
│   │   │   │   │   │   │   │   ├── ARES_GoldFoil_Albedo.png
│   │   │   │   │   │   │   │   ├── ARES_GoldFoil_Albedo.png.meta
│   │   │   │   │   │   │   │   ├── ARES_GoldFoil_AO.png
│   │   │   │   │   │   │   │   ├── ARES_GoldFoil_AO.png.meta
│   │   │   │   │   │   │   │   ├── ARES_GoldFoil_MetalSmooth.png
│   │   │   │   │   │   │   │   ├── ARES_GoldFoil_MetalSmooth.png.meta
│   │   │   │   │   │   │   │   ├── ARES_GoldFoil_Normal.png
│   │   │   │   │   │   │   │   ├── ARES_GoldFoil_Normal.png.meta
│   │   │   │   │   │   │   │   ├── ARES_Graphite_Albedo.png
│   │   │   │   │   │   │   │   ├── ARES_Graphite_Albedo.png.meta
│   │   │   │   │   │   │   │   ├── ARES_Graphite_AO.png
│   │   │   │   │   │   │   │   ├── ARES_Graphite_AO.png.meta
│   │   │   │   │   │   │   │   ├── ARES_Graphite_MetalSmooth.png
│   │   │   │   │   │   │   │   ├── ARES_Graphite_MetalSmooth.png.meta
│   │   │   │   │   │   │   │   ├── ARES_Graphite_Normal.png
│   │   │   │   │   │   │   │   ├── ARES_Graphite_Normal.png.meta
│   │   │   │   │   │   │   │   ├── ARES_PaintOrange_Albedo.png
│   │   │   │   │   │   │   │   ├── ARES_PaintOrange_Albedo.png.meta
│   │   │   │   │   │   │   │   ├── ARES_PaintOrange_AO.png
│   │   │   │   │   │   │   │   ├── ARES_PaintOrange_AO.png.meta
│   │   │   │   │   │   │   │   ├── ARES_PaintOrange_MetalSmooth.png
│   │   │   │   │   │   │   │   ├── ARES_PaintOrange_MetalSmooth.png.meta
│   │   │   │   │   │   │   │   ├── ARES_PaintOrange_Normal.png
│   │   │   │   │   │   │   │   ├── ARES_PaintOrange_Normal.png.meta
│   │   │   │   │   │   │   │   ├── ARES_PaintWhite_Albedo.png
│   │   │   │   │   │   │   │   ├── ARES_PaintWhite_Albedo.png.meta
│   │   │   │   │   │   │   │   ├── ARES_PaintWhite_AO.png
│   │   │   │   │   │   │   │   ├── ARES_PaintWhite_AO.png.meta
│   │   │   │   │   │   │   │   ├── ARES_PaintWhite_MetalSmooth.png
│   │   │   │   │   │   │   │   ├── ARES_PaintWhite_MetalSmooth.png.meta
│   │   │   │   │   │   │   │   ├── ARES_PaintWhite_Normal.png
│   │   │   │   │   │   │   │   ├── ARES_PaintWhite_Normal.png.meta
│   │   │   │   │   │   │   │   ├── ARES_Regolith_Albedo.png
│   │   │   │   │   │   │   │   ├── ARES_Regolith_Albedo.png.meta
│   │   │   │   │   │   │   │   ├── ARES_Regolith_AO.png
│   │   │   │   │   │   │   │   ├── ARES_Regolith_AO.png.meta
│   │   │   │   │   │   │   │   ├── ARES_Regolith_MetalSmooth.png
│   │   │   │   │   │   │   │   ├── ARES_Regolith_MetalSmooth.png.meta
│   │   │   │   │   │   │   │   ├── ARES_Regolith_Normal.png
│   │   │   │   │   │   │   │   ├── ARES_Regolith_Normal.png.meta
│   │   │   │   │   │   │   │   ├── ARES_Rubber_Albedo.png
│   │   │   │   │   │   │   │   ├── ARES_Rubber_Albedo.png.meta
│   │   │   │   │   │   │   │   ├── ARES_Rubber_AO.png
│   │   │   │   │   │   │   │   ├── ARES_Rubber_AO.png.meta
│   │   │   │   │   │   │   │   ├── ARES_Rubber_MetalSmooth.png
│   │   │   │   │   │   │   │   ├── ARES_Rubber_MetalSmooth.png.meta
│   │   │   │   │   │   │   │   ├── ARES_Rubber_Normal.png
│   │   │   │   │   │   │   │   ├── ARES_Rubber_Normal.png.meta
│   │   │   │   │   │   │   │   ├── ARES_Titanium_Albedo.png
│   │   │   │   │   │   │   │   ├── ARES_Titanium_Albedo.png.meta
│   │   │   │   │   │   │   │   ├── ARES_Titanium_AO.png
│   │   │   │   │   │   │   │   ├── ARES_Titanium_AO.png.meta
│   │   │   │   │   │   │   │   ├── ARES_Titanium_MetalSmooth.png
│   │   │   │   │   │   │   │   ├── ARES_Titanium_MetalSmooth.png.meta
│   │   │   │   │   │   │   │   ├── ARES_Titanium_Normal.png
│   │   │   │   │   │   │   │   └── ARES_Titanium_Normal.png.meta
│   │   │   │   │   │   │   ├── Animation.meta
│   │   │   │   │   │   │   ├── ARES_Showroom_Volume.asset
│   │   │   │   │   │   │   ├── ARES_Showroom_Volume.asset.meta
│   │   │   │   │   │   │   ├── ARES_VI_Upgraded.prefab
│   │   │   │   │   │   │   ├── ARES_VI_Upgraded.prefab.meta
│   │   │   │   │   │   │   ├── ARES_VI_Upgraded_Meshes.asset
│   │   │   │   │   │   │   ├── ARES_VI_Upgraded_Meshes.asset.meta
│   │   │   │   │   │   │   ├── Materials.meta
│   │   │   │   │   │   │   └── Textures.meta
│   │   │   │   │   │   ├── Materials.meta
│   │   │   │   │   │   ├── Source.meta
│   │   │   │   │   │   └── Upgraded.meta
│   │   │   │   │   ├── Editor/
│   │   │   │   │   │   ├── AresViShowroom.cs
│   │   │   │   │   │   ├── AresViShowroom.cs.meta
│   │   │   │   │   │   ├── AresViUpgrade.cs
│   │   │   │   │   │   ├── AresViUpgrade.cs.meta
│   │   │   │   │   │   ├── AresViUpgradeVerify.cs
│   │   │   │   │   │   └── AresViUpgradeVerify.cs.meta
│   │   │   │   │   ├── Scenes/
│   │   │   │   │   │   ├── ARES_VI_Showroom.unity
│   │   │   │   │   │   └── ARES_VI_Showroom.unity.meta
│   │   │   │   │   ├── ARES_VI.meta
│   │   │   │   │   ├── Editor.meta
│   │   │   │   │   └── Scenes.meta
│   │   │   │   ├── Verification/
│   │   │   │   │   ├── Editor/
│   │   │   │   │   │   ├── LaptopPlaytest.cs
│   │   │   │   │   │   ├── LaptopPlaytest.cs.meta
│   │   │   │   │   │   ├── Step1GameplayVerification.cs
│   │   │   │   │   │   ├── Step1GameplayVerification.cs.meta
│   │   │   │   │   │   ├── Step3BackendClientTest.cs
│   │   │   │   │   │   ├── Step3BackendClientTest.cs.meta
│   │   │   │   │   │   ├── Step4RulesTest.cs
│   │   │   │   │   │   └── Step4RulesTest.cs.meta
│   │   │   │   │   └── Editor.meta
│   │   │   │   ├── VisualUpgrade/
│   │   │   │   │   ├── Editor/
│   │   │   │   │   │   ├── VisualUpgradeBuilder.cs
│   │   │   │   │   │   ├── VisualUpgradeBuilder.cs.meta
│   │   │   │   │   │   ├── VisualUpgradeCapture.cs
│   │   │   │   │   │   ├── VisualUpgradeCapture.cs.meta
│   │   │   │   │   │   ├── VisualUpgradeCharacters.cs
│   │   │   │   │   │   ├── VisualUpgradeCharacters.cs.meta
│   │   │   │   │   │   ├── VisualUpgradeEnvironment.cs
│   │   │   │   │   │   ├── VisualUpgradeEnvironment.cs.meta
│   │   │   │   │   │   ├── VisualUpgradeKit.cs
│   │   │   │   │   │   ├── VisualUpgradeKit.cs.meta
│   │   │   │   │   │   ├── VisualUpgradePlaytest.cs
│   │   │   │   │   │   ├── VisualUpgradePlaytest.cs.meta
│   │   │   │   │   │   ├── VisualUpgradeProps.cs
│   │   │   │   │   │   ├── VisualUpgradeProps.cs.meta
│   │   │   │   │   │   ├── VisualUpgradeResponsiveAudit.cs
│   │   │   │   │   │   └── VisualUpgradeResponsiveAudit.cs.meta
│   │   │   │   │   ├── Materials/
│   │   │   │   │   │   ├── VisualUpgradeHaze.mat
│   │   │   │   │   │   ├── VisualUpgradeHaze.mat.meta
│   │   │   │   │   │   ├── VisualUpgradeMarsGround.mat
│   │   │   │   │   │   ├── VisualUpgradeMarsGround.mat.meta
│   │   │   │   │   │   ├── VisualUpgradeMarsSky.mat
│   │   │   │   │   │   ├── VisualUpgradeMarsSky.mat.meta
│   │   │   │   │   │   ├── VisualUpgradePhotovoltaic.mat
│   │   │   │   │   │   ├── VisualUpgradePhotovoltaic.mat.meta
│   │   │   │   │   │   ├── VisualUpgradeRock07.mat
│   │   │   │   │   │   ├── VisualUpgradeRock07.mat.meta
│   │   │   │   │   │   ├── VisualUpgradeRock09.mat
│   │   │   │   │   │   ├── VisualUpgradeRock09.mat.meta
│   │   │   │   │   │   ├── VisualUpgradeSky.mat
│   │   │   │   │   │   ├── VisualUpgradeSky.mat.meta
│   │   │   │   │   │   ├── VisualUpgradeWheelDust.mat
│   │   │   │   │   │   ├── VisualUpgradeWheelDust.mat.meta
│   │   │   │   │   │   ├── VU_Accent_orange.mat
│   │   │   │   │   │   ├── VU_Accent_orange.mat.meta
│   │   │   │   │   │   ├── VU_Beacon_red.mat
│   │   │   │   │   │   ├── VU_Beacon_red.mat.meta
│   │   │   │   │   │   ├── VU_Brushed_aluminium.mat
│   │   │   │   │   │   ├── VU_Brushed_aluminium.mat.meta
│   │   │   │   │   │   ├── VU_Equipment_white.mat
│   │   │   │   │   │   ├── VU_Equipment_white.mat.meta
│   │   │   │   │   │   ├── VU_Habitat_marker_light.mat
│   │   │   │   │   │   ├── VU_Habitat_marker_light.mat.meta
│   │   │   │   │   │   ├── VU_Habitat_shell.mat
│   │   │   │   │   │   ├── VU_Habitat_shell.mat.meta
│   │   │   │   │   │   ├── VU_Habitat_window_glow.mat
│   │   │   │   │   │   ├── VU_Habitat_window_glow.mat.meta
│   │   │   │   │   │   ├── VU_Helmet_lamp.mat
│   │   │   │   │   │   ├── VU_Helmet_lamp.mat.meta
│   │   │   │   │   │   ├── VU_Mission_emblem_(provisional).mat
│   │   │   │   │   │   ├── VU_Mission_emblem_(provisional).mat.meta
│   │   │   │   │   │   ├── VU_Rover_beacon.mat
│   │   │   │   │   │   ├── VU_Rover_beacon.mat.meta
│   │   │   │   │   │   ├── VU_Rover_body_panel.mat
│   │   │   │   │   │   ├── VU_Rover_body_panel.mat.meta
│   │   │   │   │   │   ├── VU_Rover_cockpit_glass.mat
│   │   │   │   │   │   ├── VU_Rover_cockpit_glass.mat.meta
│   │   │   │   │   │   ├── VU_Rover_graphite.mat
│   │   │   │   │   │   ├── VU_Rover_graphite.mat.meta
│   │   │   │   │   │   ├── VU_Rover_headlight_lens.mat
│   │   │   │   │   │   ├── VU_Rover_headlight_lens.mat.meta
│   │   │   │   │   │   ├── VU_Rover_screen.mat
│   │   │   │   │   │   ├── VU_Rover_screen.mat.meta
│   │   │   │   │   │   ├── VU_Rover_seat.mat
│   │   │   │   │   │   ├── VU_Rover_seat.mat.meta
│   │   │   │   │   │   ├── VU_Rover_solar_cells.mat
│   │   │   │   │   │   ├── VU_Rover_solar_cells.mat.meta
│   │   │   │   │   │   ├── VU_Rover_tail_light.mat
│   │   │   │   │   │   ├── VU_Rover_tail_light.mat.meta
│   │   │   │   │   │   ├── VU_Scan_ring.mat
│   │   │   │   │   │   ├── VU_Scan_ring.mat.meta
│   │   │   │   │   │   ├── VU_Sensor_lens.mat
│   │   │   │   │   │   ├── VU_Sensor_lens.mat.meta
│   │   │   │   │   │   ├── VU_Status_amber.mat
│   │   │   │   │   │   ├── VU_Status_amber.mat.meta
│   │   │   │   │   │   ├── VU_Status_cyan.mat
│   │   │   │   │   │   ├── VU_Status_cyan.mat.meta
│   │   │   │   │   │   ├── VU_Suit_fabric.mat
│   │   │   │   │   │   ├── VU_Suit_fabric.mat.meta
│   │   │   │   │   │   ├── VU_Suit_hard_shell.mat
│   │   │   │   │   │   ├── VU_Suit_hard_shell.mat.meta
│   │   │   │   │   │   ├── VU_Suit_joint_bearing.mat
│   │   │   │   │   │   ├── VU_Suit_joint_bearing.mat.meta
│   │   │   │   │   │   ├── VU_Suit_soft_grey.mat
│   │   │   │   │   │   ├── VU_Suit_soft_grey.mat.meta
│   │   │   │   │   │   ├── VU_Visor_gold_coating.mat
│   │   │   │   │   │   └── VU_Visor_gold_coating.mat.meta
│   │   │   │   │   ├── Meshes/
│   │   │   │   │   │   ├── VisualUpgradeDish.asset
│   │   │   │   │   │   ├── VisualUpgradeDish.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeFarTerrain.asset
│   │   │   │   │   │   ├── VisualUpgradeFarTerrain.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeRoverDish.asset
│   │   │   │   │   │   ├── VisualUpgradeRoverDish.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_0_0.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_0_0.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_0_1.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_0_1.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_0_2.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_0_2.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_0_3.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_0_3.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_0_4.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_0_4.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_0_5.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_0_5.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_1_0.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_1_0.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_1_1.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_1_1.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_1_2.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_1_2.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_1_3.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_1_3.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_1_4.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_1_4.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_1_5.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_1_5.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_2_0.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_2_0.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_2_1.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_2_1.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_2_2.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_2_2.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_2_3.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_2_3.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_2_4.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_2_4.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_2_5.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_2_5.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_3_0.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_3_0.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_3_1.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_3_1.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_3_2.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_3_2.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_3_3.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_3_3.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_3_4.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_3_4.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_3_5.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_3_5.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_4_0.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_4_0.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_4_1.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_4_1.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_4_2.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_4_2.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_4_3.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_4_3.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_4_4.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_4_4.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_4_5.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_4_5.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_5_0.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_5_0.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_5_1.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_5_1.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_5_2.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_5_2.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_5_3.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_5_3.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_5_4.asset
│   │   │   │   │   │   ├── VisualUpgradeScatter_5_4.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeScatter_5_5.asset
│   │   │   │   │   │   └── VisualUpgradeScatter_5_5.asset.meta
│   │   │   │   │   ├── Scenes/
│   │   │   │   │   │   ├── Mars_VisualUpgrade_v1.unity
│   │   │   │   │   │   └── Mars_VisualUpgrade_v1.unity.meta
│   │   │   │   │   ├── Scripts/
│   │   │   │   │   │   ├── VisualUpgradeAstronautMotion.cs
│   │   │   │   │   │   ├── VisualUpgradeAstronautMotion.cs.meta
│   │   │   │   │   │   ├── VisualUpgradeAtmosphere.cs
│   │   │   │   │   │   ├── VisualUpgradeAtmosphere.cs.meta
│   │   │   │   │   │   ├── VisualUpgradeBlink.cs
│   │   │   │   │   │   ├── VisualUpgradeBlink.cs.meta
│   │   │   │   │   │   ├── VisualUpgradeCameraRig.cs
│   │   │   │   │   │   ├── VisualUpgradeCameraRig.cs.meta
│   │   │   │   │   │   ├── VisualUpgradeControlAssist.cs
│   │   │   │   │   │   ├── VisualUpgradeControlAssist.cs.meta
│   │   │   │   │   │   ├── VisualUpgradeHUDFixes.cs
│   │   │   │   │   │   ├── VisualUpgradeHUDFixes.cs.meta
│   │   │   │   │   │   ├── VisualUpgradeHUDStyle.cs
│   │   │   │   │   │   ├── VisualUpgradeHUDStyle.cs.meta
│   │   │   │   │   │   ├── VisualUpgradeRoverPresentation.cs
│   │   │   │   │   │   ├── VisualUpgradeRoverPresentation.cs.meta
│   │   │   │   │   │   ├── VisualUpgradeStationStatus.cs
│   │   │   │   │   │   └── VisualUpgradeStationStatus.cs.meta
│   │   │   │   │   ├── Settings/
│   │   │   │   │   │   ├── Lighting_0.asset
│   │   │   │   │   │   ├── Lighting_0.asset.meta
│   │   │   │   │   │   ├── Lighting_1.asset
│   │   │   │   │   │   ├── Lighting_1.asset.meta
│   │   │   │   │   │   ├── Lighting_2.asset
│   │   │   │   │   │   ├── Lighting_2.asset.meta
│   │   │   │   │   │   ├── Lighting_3.asset
│   │   │   │   │   │   ├── Lighting_3.asset.meta
│   │   │   │   │   │   ├── VisualUpgrade_Renderer.asset
│   │   │   │   │   │   ├── VisualUpgrade_Renderer.asset.meta
│   │   │   │   │   │   ├── VisualUpgrade_URP.asset
│   │   │   │   │   │   ├── VisualUpgrade_URP.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeHUDTheme.asset
│   │   │   │   │   │   ├── VisualUpgradeHUDTheme.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeMissionRegistry.asset
│   │   │   │   │   │   ├── VisualUpgradeMissionRegistry.asset.meta
│   │   │   │   │   │   ├── VisualUpgradeVolume.asset
│   │   │   │   │   │   └── VisualUpgradeVolume.asset.meta
│   │   │   │   │   ├── Shaders/
│   │   │   │   │   │   ├── MarsSky.shader
│   │   │   │   │   │   ├── MarsSky.shader.meta
│   │   │   │   │   │   ├── MarsTerrain.shader
│   │   │   │   │   │   └── MarsTerrain.shader.meta
│   │   │   │   │   ├── Textures/
│   │   │   │   │   │   ├── VisualUpgradeEmblem.png
│   │   │   │   │   │   ├── VisualUpgradeEmblem.png.meta
│   │   │   │   │   │   ├── VisualUpgradeSoftDust.png
│   │   │   │   │   │   └── VisualUpgradeSoftDust.png.meta
│   │   │   │   │   ├── Documentation.meta
│   │   │   │   │   ├── Editor.meta
│   │   │   │   │   ├── Materials.meta
│   │   │   │   │   ├── Meshes.meta
│   │   │   │   │   ├── Scenes.meta
│   │   │   │   │   ├── Scripts.meta
│   │   │   │   │   ├── Settings.meta
│   │   │   │   │   ├── Shaders.meta
│   │   │   │   │   └── Textures.meta
│   │   │   │   ├── CinematicHUD.meta
│   │   │   │   ├── Environment.meta
│   │   │   │   ├── FirstMission.meta
│   │   │   │   ├── Greybox.meta
│   │   │   │   ├── Integration.meta
│   │   │   │   ├── MarsEnvironment.meta
│   │   │   │   ├── MarsVisualPolish.meta
│   │   │   │   ├── Mission01Milestone.meta
│   │   │   │   ├── MissionIntegration.meta
│   │   │   │   ├── MissionRules.meta
│   │   │   │   ├── NativeControlSlice.meta
│   │   │   │   ├── ProductionVisual.meta
│   │   │   │   ├── Scenes.meta
│   │   │   │   ├── Stage08B.meta
│   │   │   │   ├── Stage08C.meta
│   │   │   │   ├── Stage08C1.meta
│   │   │   │   ├── VehicleShowroom.meta
│   │   │   │   ├── Verification.meta
│   │   │   │   └── VisualUpgrade.meta
│   │   │   ├── Scenes/
│   │   │   │   ├── SampleScene.unity
│   │   │   │   └── SampleScene.unity.meta
│   │   │   ├── Settings/
│   │   │   │   ├── DefaultVolumeProfile.asset
│   │   │   │   ├── DefaultVolumeProfile.asset.meta
│   │   │   │   ├── Mobile_Renderer.asset
│   │   │   │   ├── Mobile_Renderer.asset.meta
│   │   │   │   ├── Mobile_RPAsset.asset
│   │   │   │   ├── Mobile_RPAsset.asset.meta
│   │   │   │   ├── PC_Renderer.asset
│   │   │   │   ├── PC_Renderer.asset.meta
│   │   │   │   ├── PC_RPAsset.asset
│   │   │   │   ├── PC_RPAsset.asset.meta
│   │   │   │   ├── SampleSceneProfile.asset
│   │   │   │   ├── SampleSceneProfile.asset.meta
│   │   │   │   ├── UniversalRenderPipelineGlobalSettings.asset
│   │   │   │   └── UniversalRenderPipelineGlobalSettings.asset.meta
│   │   │   ├── TutorialInfo/
│   │   │   │   ├── Editor/
│   │   │   │   │   ├── ReadmeEditor.cs
│   │   │   │   │   └── ReadmeEditor.cs.meta
│   │   │   │   ├── Icons/
│   │   │   │   │   ├── URP.png
│   │   │   │   │   └── URP.png.meta
│   │   │   │   ├── StyleSheets/
│   │   │   │   │   ├── ReadmeEditor.uss
│   │   │   │   │   ├── ReadmeEditor.uss.meta
│   │   │   │   │   ├── ReadmeEditorDark.uss
│   │   │   │   │   ├── ReadmeEditorDark.uss.meta
│   │   │   │   │   ├── ReadmeEditorLight.uss
│   │   │   │   │   └── ReadmeEditorLight.uss.meta
│   │   │   │   ├── Editor.meta
│   │   │   │   ├── Icons.meta
│   │   │   │   ├── Layout.wlt
│   │   │   │   ├── Layout.wlt.meta
│   │   │   │   ├── Readme.cs
│   │   │   │   ├── Readme.cs.meta
│   │   │   │   └── StyleSheets.meta
│   │   │   ├── _Recovery.meta
│   │   │   ├── AstroGateway.meta
│   │   │   ├── InputSystem_Actions.inputactions
│   │   │   ├── InputSystem_Actions.inputactions.meta
│   │   │   ├── Readme.asset
│   │   │   ├── Readme.asset.meta
│   │   │   ├── Scenes.meta
│   │   │   ├── Settings.meta
│   │   │   └── TutorialInfo.meta
│   │   ├── Data/
│   │   │   └── Plugins/
│   │   │       ├── lib_burst_generated.cpp
│   │   │       └── lib_burst_generated.wasm
│   │   ├── Docs/
│   │   │   ├── Design/
│   │   │   │   ├── 01_Vision_And_Art_Direction.md
│   │   │   │   ├── 02_Mars_World_Layout.md
│   │   │   │   ├── 03_Environment_Asset_List.md
│   │   │   │   ├── 04_Unity_URP_Technical_Standards.md
│   │   │   │   ├── 05_Gameplay_Environment_Integration.md
│   │   │   │   ├── 06_References_And_Attribution.md
│   │   │   │   └── 07_Implementation_Roadmap.md
│   │   │   ├── FirstMission/
│   │   │   │   ├── Backend/
│   │   │   │   │   └── Backend_Client_Results.txt
│   │   │   │   ├── Playtest/
│   │   │   │   │   ├── TestStore/
│   │   │   │   │   │   ├── bounds/
│   │   │   │   │   │   │   └── sensor_records_v1.json
│   │   │   │   │   │   ├── mock/
│   │   │   │   │   │   │   └── sensor_records_v1.json
│   │   │   │   │   │   └── sensor_records_v1.json
│   │   │   │   │   ├── FirstMission_Playtest_Results.txt
│   │   │   │   │   └── Touch_Target_Audit.txt
│   │   │   │   ├── PlaytestExplorer/
│   │   │   │   │   ├── TestStore/
│   │   │   │   │   │   ├── bounds/
│   │   │   │   │   │   │   └── sensor_records_v1.json
│   │   │   │   │   │   ├── mock/
│   │   │   │   │   │   │   └── sensor_records_v1.json
│   │   │   │   │   │   └── sensor_records_v1.json
│   │   │   │   │   ├── FirstMission_Playtest_Results.txt
│   │   │   │   │   └── Touch_Target_Audit.txt
│   │   │   │   ├── PlaytestHUDv2/
│   │   │   │   │   ├── TestStore/
│   │   │   │   │   │   ├── bounds/
│   │   │   │   │   │   │   └── sensor_records_v1.json
│   │   │   │   │   │   ├── mock/
│   │   │   │   │   │   │   └── sensor_records_v1.json
│   │   │   │   │   │   └── sensor_records_v1.json
│   │   │   │   │   ├── FirstMission_Playtest_Results.txt
│   │   │   │   │   └── Touch_Target_Audit.txt
│   │   │   │   ├── PlaytestMissions/
│   │   │   │   │   ├── TestStore/
│   │   │   │   │   │   ├── bounds/
│   │   │   │   │   │   │   └── sensor_records_v1.json
│   │   │   │   │   │   ├── mock/
│   │   │   │   │   │   │   └── sensor_records_v1.json
│   │   │   │   │   │   └── sensor_records_v1.json
│   │   │   │   │   ├── FirstMission_Playtest_Results.txt
│   │   │   │   │   └── Touch_Target_Audit.txt
│   │   │   │   ├── Regression/
│   │   │   │   │   ├── VisualUpgrade/
│   │   │   │   │   │   ├── HUD_Audit.txt
│   │   │   │   │   │   └── VisualUpgradePlaytest_Results.txt
│   │   │   │   │   ├── VisualUpgrade_controls/
│   │   │   │   │   │   ├── HUD_Audit.txt
│   │   │   │   │   │   └── VisualUpgradePlaytest_Results.txt
│   │   │   │   │   ├── VisualUpgrade_ex/
│   │   │   │   │   │   ├── HUD_Audit.txt
│   │   │   │   │   │   └── VisualUpgradePlaytest_Results.txt
│   │   │   │   │   ├── VisualUpgrade_ex2/
│   │   │   │   │   │   ├── HUD_Audit.txt
│   │   │   │   │   │   └── VisualUpgradePlaytest_Results.txt
│   │   │   │   │   ├── VisualUpgrade_hud/
│   │   │   │   │   │   ├── HUD_Audit.txt
│   │   │   │   │   │   └── VisualUpgradePlaytest_Results.txt
│   │   │   │   │   └── VisualUpgrade_run2/
│   │   │   │   │       ├── HUD_Audit.txt
│   │   │   │   │       └── VisualUpgradePlaytest_Results.txt
│   │   │   │   ├── SQL/
│   │   │   │   │   ├── 006_sensor_records.sql
│   │   │   │   │   └── 906_sensor_records_rollback.sql
│   │   │   │   ├── Build.txt
│   │   │   │   ├── Build_Explorer.txt
│   │   │   │   ├── Build_HUDv2.txt
│   │   │   │   ├── Build_Missions.txt
│   │   │   │   ├── Controls_Report.md
│   │   │   │   ├── Explorer_Report.md
│   │   │   │   ├── Explorer_Stage1_Audit.md
│   │   │   │   ├── HUDv2_Report.md
│   │   │   │   ├── Missions_Report.md
│   │   │   │   ├── Phase1_Audit.md
│   │   │   │   └── Phase2_Report.md
│   │   │   ├── GitHubCollaboration/
│   │   │   │   └── GitHub_Collaboration_Readiness.md
│   │   │   ├── LaptopPlaytest/
│   │   │   │   ├── Run1_SavedHUDHidden/
│   │   │   │   │   ├── HUD_Audit.txt
│   │   │   │   │   └── LaptopPlaytest_Results.txt
│   │   │   │   ├── HUD_Audit.txt
│   │   │   │   ├── Laptop_Acceptance_Report.md
│   │   │   │   └── LaptopPlaytest_Results.txt
│   │   │   ├── Regression/
│   │   │   │   ├── HUD_Controlled_Tests.txt
│   │   │   │   ├── Mars_Stage08B_Scripted_Play_Mode_Tests.txt
│   │   │   │   ├── Mission01_PlayMode_Controlled_Tests.txt
│   │   │   │   ├── Native_Controlled_PlayMode_Tests.txt
│   │   │   │   ├── Responsive_Layout_Checks.txt
│   │   │   │   └── VisualPolish_Controlled_PlayMode_Tests.txt
│   │   │   ├── Stage04/
│   │   │   │   ├── Assets_And_Attribution.md
│   │   │   │   ├── Stage04_Completion_Report.md
│   │   │   │   └── Validation_Evidence_Paths.txt
│   │   │   ├── Step1/
│   │   │   │   └── Step1_Gameplay_Results.txt
│   │   │   ├── Step2/
│   │   │   │   └── Mission_Integration_Results.txt
│   │   │   ├── Step3/
│   │   │   │   ├── SQL/
│   │   │   │   │   ├── 005_mission_commands_reports.sql
│   │   │   │   │   └── 905_mission_commands_reports_rollback.sql
│   │   │   │   ├── Backend_pytest_results.txt
│   │   │   │   ├── Step3_Client_Results.txt
│   │   │   │   └── Step3_Report.md
│   │   │   ├── Step4/
│   │   │   │   ├── Backend_pytest_results.txt
│   │   │   │   ├── Mission_Rules_Design.md
│   │   │   │   └── Step4_Unity_Rules_Results.txt
│   │   │   ├── VehicleShowroom/
│   │   │   │   └── ARES_VI/
│   │   │   │       ├── Upgrade/
│   │   │   │       │   ├── ARES_VI_Upgrade_Report.md
│   │   │   │       │   └── Upgrade_Build_Report.txt
│   │   │   │       ├── ARES_VI_Showroom_Verification.md
│   │   │   │       └── Unity_Import_Report.txt
│   │   │   ├── VisualEngineering/
│   │   │   │   ├── Batch2_Report.md
│   │   │   │   ├── Batch3_3D_Animation_Plan.md
│   │   │   │   ├── Batch4_Controls_Report.md
│   │   │   │   ├── Implementation_Plan.md
│   │   │   │   ├── Object_Inventory.md
│   │   │   │   ├── UI_Component_Inventory.md
│   │   │   │   ├── UI_Design_System.md
│   │   │   │   └── Visual_Defect_Register.md
│   │   │   ├── VisualUpgrade/
│   │   │   │   ├── Baseline_ProductionVisual/
│   │   │   │   │   ├── HUD_Audit.txt
│   │   │   │   │   └── LaptopPlaytest_Results.txt
│   │   │   │   ├── Batch2/
│   │   │   │   │   ├── After/
│   │   │   │   │   │   └── Responsive_HUD_Audit.txt
│   │   │   │   │   ├── Before/
│   │   │   │   │   │   └── Responsive_HUD_Audit.txt
│   │   │   │   │   ├── Playtest/
│   │   │   │   │   │   ├── HUD_Audit.txt
│   │   │   │   │   │   └── VisualUpgradePlaytest_Results.txt
│   │   │   │   │   ├── chain_status.txt
│   │   │   │   │   └── verify_status.txt
│   │   │   │   ├── Batch3/
│   │   │   │   │   ├── Playtest/
│   │   │   │   │   │   ├── HUD_Audit.txt
│   │   │   │   │   │   └── VisualUpgradePlaytest_Results.txt
│   │   │   │   │   ├── Responsive/
│   │   │   │   │   │   └── Responsive_HUD_Audit.txt
│   │   │   │   │   ├── Build.txt
│   │   │   │   │   └── chain_status.txt
│   │   │   │   ├── Batch4/
│   │   │   │   │   ├── Playtest/
│   │   │   │   │   │   ├── HUD_Audit.txt
│   │   │   │   │   │   └── VisualUpgradePlaytest_Results.txt
│   │   │   │   │   ├── Responsive/
│   │   │   │   │   │   └── Responsive_HUD_Audit.txt
│   │   │   │   │   ├── Build.txt
│   │   │   │   │   └── chain_status.txt
│   │   │   │   ├── Batch5/
│   │   │   │   │   ├── Playtest/
│   │   │   │   │   │   ├── HUD_Audit.txt
│   │   │   │   │   │   └── VisualUpgradePlaytest_Results.txt
│   │   │   │   │   ├── Responsive/
│   │   │   │   │   │   └── Responsive_HUD_Audit.txt
│   │   │   │   │   ├── Build.txt
│   │   │   │   │   └── chain_status.txt
│   │   │   │   ├── FPS_After_VisualUpgrade/
│   │   │   │   │   ├── HUD_Audit.txt
│   │   │   │   │   └── VisualUpgradePlaytest_Results.txt
│   │   │   │   ├── FPS_Before_ProductionVisual/
│   │   │   │   │   ├── HUD_Audit.txt
│   │   │   │   │   └── VisualUpgradePlaytest_Results.txt
│   │   │   │   ├── Playtest_Final/
│   │   │   │   │   ├── HUD_Audit.txt
│   │   │   │   │   └── VisualUpgradePlaytest_Results.txt
│   │   │   │   ├── Playtest_Run1/
│   │   │   │   │   ├── HUD_Audit.txt
│   │   │   │   │   └── VisualUpgradePlaytest_Results.txt
│   │   │   │   ├── UIPass2/
│   │   │   │   │   ├── Playtest/
│   │   │   │   │   │   ├── HUD_Audit.txt
│   │   │   │   │   │   └── VisualUpgradePlaytest_Results.txt
│   │   │   │   │   └── Playtest_Run1/
│   │   │   │   │       ├── HUD_Audit.txt
│   │   │   │   │       └── VisualUpgradePlaytest_Results.txt
│   │   │   │   ├── Build.txt
│   │   │   │   ├── Scene_Diagnostics.txt
│   │   │   │   ├── Scene_Inventory.txt
│   │   │   │   └── Visual_Upgrade_Report.md
│   │   │   ├── Backend_Mission_Endpoints_Plan.md
│   │   │   ├── Integration_Architecture.md
│   │   │   └── PROGRESS_REPORT.md
│   │   ├── Packages/
│   │   │   ├── manifest.json
│   │   │   └── packages-lock.json
│   │   ├── ProjectSettings/
│   │   │   ├── AudioManager.asset
│   │   │   ├── ClusterInputManager.asset
│   │   │   ├── DynamicsManager.asset
│   │   │   ├── EditorBuildSettings.asset
│   │   │   ├── EditorSettings.asset
│   │   │   ├── GraphicsSettings.asset
│   │   │   ├── InputManager.asset
│   │   │   ├── MemorySettings.asset
│   │   │   ├── MultiplayerManager.asset
│   │   │   ├── NavMeshAreas.asset
│   │   │   ├── PackageManagerSettings.asset
│   │   │   ├── Physics2DSettings.asset
│   │   │   ├── PhysicsCoreProjectSettings2D.asset
│   │   │   ├── PresetManager.asset
│   │   │   ├── ProjectAuditorSettings.asset
│   │   │   ├── ProjectSettings.asset
│   │   │   ├── ProjectVersion.txt
│   │   │   ├── QualitySettings.asset
│   │   │   ├── SceneTemplateSettings.json
│   │   │   ├── ShaderGraphSettings.asset
│   │   │   ├── TagManager.asset
│   │   │   ├── TimeManager.asset
│   │   │   ├── UnityConnectSettings.asset
│   │   │   ├── URPProjectSettings.asset
│   │   │   ├── VersionControlSettings.asset
│   │   │   ├── VFXManager.asset
│   │   │   └── XRSettings.asset
│   │   └── .gitignore
│   └── SourceArt/
│       ├── 3d-game-assets/
│       │   ├── ares_vi_aaa_lod0.obj
│       │   ├── ares_vi_aaa_lod1.obj
│       │   ├── ares_vi_aaa_lod2.obj
│       │   ├── ares_vi_aaa_master.glb
│       │   ├── ares_vi_aaa_master.mtl
│       │   ├── ares_vi_aaa_master.obj
│       │   ├── ares_vi_aaa_studio_data.json
│       │   ├── ARES_VI_AAA_Variant.prefab
│       │   ├── ARES_VI_AAA_Variant.prefab.meta
│       │   ├── ares_vi_master.mtl
│       │   ├── ares_vi_master.obj
│       │   ├── ares_vi_studio_data.json
│       │   ├── AresViHoverVisualController.cs
│       │   ├── AresViLODConfig.json
│       │   ├── build_ares_vi_aaa_production.py
│       │   ├── build_ares_vi_master.py
│       │   ├── build_model.py
│       │   ├── build_rover.py
│       │   ├── generate_ares_vi_blender.py
│       │   ├── generate_in_blender.py
│       │   ├── generate_rover_in_blender.py
│       │   ├── generate_studio_html.py
│       │   ├── generate_unity_prefab.py
│       │   ├── generate_viewer.py
│       │   ├── geometry_quality_gate.py
│       │   ├── loot_chest.mtl
│       │   ├── loot_chest.obj
│       │   ├── mars_rover.mtl
│       │   ├── mars_rover.obj
│       │   ├── mesh_data.json
│       │   ├── reimport_and_verify.py
│       │   ├── render_aaa_views.py
│       │   ├── render_ares_vi_views.py
│       │   ├── rover_mesh.json
│       │   ├── test.png
│       │   ├── test_geometry_engine.py
│       │   ├── test_png_writer.py
│       │   └── update_palette.py
│       ├── ARES_EVA_MK1/
│       │   ├── blender/
│       │   │   ├── ARES_EVA_MK1_Master.blend
│       │   │   ├── ARES_EVA_MK1_Master.blend1
│       │   │   ├── generate_ares_eva_mk1_master.py
│       │   │   ├── render_character_views.py
│       │   │   ├── test_armature_builder.py
│       │   │   └── test_blender_capabilities.py
│       │   ├── concept/
│       │   │   ├── build_concept_blockout.py
│       │   │   ├── concept_blockout.json
│       │   │   ├── generate_concept_html.py
│       │   │   └── render_astronaut_views.py
│       │   ├── models/
│       │   │   ├── ARES_EVA_MK1_LOD0.mtl
│       │   │   ├── ARES_EVA_MK1_LOD0.obj
│       │   │   ├── ARES_EVA_MK1_LOD1.mtl
│       │   │   ├── ARES_EVA_MK1_LOD1.obj
│       │   │   ├── ARES_EVA_MK1_LOD2.mtl
│       │   │   ├── ARES_EVA_MK1_LOD2.obj
│       │   │   ├── ARES_EVA_MK1_LOD3.mtl
│       │   │   ├── ARES_EVA_MK1_LOD3.obj
│       │   │   ├── ARES_EVA_MK1_Rigged.fbx
│       │   │   └── ARES_EVA_MK1_Rigged.glb
│       │   └── unity_package/
│       │       ├── ARES_EVA_MK1.prefab
│       │       ├── ARES_EVA_MK1.prefab.meta
│       │       ├── ARES_EVA_MK1_Showroom.unity
│       │       ├── ARES_EVA_MK1_Showroom.unity.meta
│       │       ├── AresEvaVisualAdapter.cs
│       │       ├── AresShowroomController.cs
│       │       ├── generate_showroom_scene.py
│       │       └── generate_unity_character_prefab.py
│       ├── ARES_EVA_MK1_v1_baseline/
│       │   ├── blender/
│       │   │   ├── ARES_EVA_MK1_Master.blend
│       │   │   ├── ARES_EVA_MK1_Master.blend1
│       │   │   ├── generate_ares_eva_mk1_master.py
│       │   │   ├── render_character_views.py
│       │   │   ├── test_armature_builder.py
│       │   │   └── test_blender_capabilities.py
│       │   ├── concept/
│       │   │   ├── build_concept_blockout.py
│       │   │   ├── concept_blockout.json
│       │   │   ├── generate_concept_html.py
│       │   │   └── render_astronaut_views.py
│       │   ├── models/
│       │   │   ├── ARES_EVA_MK1_LOD0.mtl
│       │   │   ├── ARES_EVA_MK1_LOD0.obj
│       │   │   ├── ARES_EVA_MK1_LOD1.mtl
│       │   │   ├── ARES_EVA_MK1_LOD1.obj
│       │   │   ├── ARES_EVA_MK1_LOD2.mtl
│       │   │   ├── ARES_EVA_MK1_LOD2.obj
│       │   │   ├── ARES_EVA_MK1_LOD3.mtl
│       │   │   ├── ARES_EVA_MK1_LOD3.obj
│       │   │   ├── ARES_EVA_MK1_Rigged.fbx
│       │   │   └── ARES_EVA_MK1_Rigged.glb
│       │   ├── unity_package/
│       │   │   ├── ARES_EVA_MK1.prefab
│       │   │   ├── ARES_EVA_MK1.prefab.meta
│       │   │   ├── ARES_EVA_MK1_Showroom.unity
│       │   │   ├── ARES_EVA_MK1_Showroom.unity.meta
│       │   │   ├── AresEvaVisualAdapter.cs
│       │   │   ├── AresShowroomController.cs
│       │   │   ├── generate_showroom_scene.py
│       │   │   └── generate_unity_character_prefab.py
│       │   └── baseline_manifest.sha256
│       ├── ARES_EVA_MK1_v2/
│       │   ├── blender/
│       │   │   ├── ARES_EVA_MK1_v2_Master.blend
│       │   │   ├── ARES_EVA_MK1_v2_Master.blend1
│       │   │   └── generate_ares_eva_mk1_v2_production.py
│       │   ├── models/
│       │   │   ├── ARES_EVA_MK1_v2_LOD0.mtl
│       │   │   ├── ARES_EVA_MK1_v2_LOD0.obj
│       │   │   ├── ARES_EVA_MK1_v2_LOD1.mtl
│       │   │   ├── ARES_EVA_MK1_v2_LOD1.obj
│       │   │   ├── ARES_EVA_MK1_v2_LOD2.mtl
│       │   │   ├── ARES_EVA_MK1_v2_LOD2.obj
│       │   │   ├── ARES_EVA_MK1_v2_LOD3.mtl
│       │   │   ├── ARES_EVA_MK1_v2_LOD3.obj
│       │   │   ├── ARES_EVA_MK1_v2_Rigged.fbx
│       │   │   └── ARES_EVA_MK1_v2_Rigged.glb
│       │   └── renders/
│       │       ├── 01_v2_hero_front.png
│       │       ├── 02_v2_rear_plss.png
│       │       ├── 03_v2_helmet_closeup.png
│       │       ├── 04_v2_elbow_bellows_closeup.png
│       │       ├── 05_v2_glove_fingers_closeup.png
│       │       ├── 06_v2_knee_bellows_closeup.png
│       │       ├── 07_v2_boot_cleats_closeup.png
│       │       ├── 08_v2_wireframe_topology.png
│       │       └── render_v2_obj_views.py
│       ├── ARES_EVA_MK1_v2_gate3_baseline/
│       │   ├── blender/
│       │   │   ├── ARES_EVA_MK1_v2_Master.blend
│       │   │   ├── ARES_EVA_MK1_v2_Master.blend1
│       │   │   └── generate_ares_eva_mk1_v2_production.py
│       │   ├── models/
│       │   │   ├── ARES_EVA_MK1_v2_LOD0.mtl
│       │   │   ├── ARES_EVA_MK1_v2_LOD0.obj
│       │   │   ├── ARES_EVA_MK1_v2_LOD1.mtl
│       │   │   ├── ARES_EVA_MK1_v2_LOD1.obj
│       │   │   ├── ARES_EVA_MK1_v2_LOD2.mtl
│       │   │   ├── ARES_EVA_MK1_v2_LOD2.obj
│       │   │   ├── ARES_EVA_MK1_v2_LOD3.mtl
│       │   │   ├── ARES_EVA_MK1_v2_LOD3.obj
│       │   │   ├── ARES_EVA_MK1_v2_Rigged.fbx
│       │   │   └── ARES_EVA_MK1_v2_Rigged.glb
│       │   ├── renders/
│       │   │   ├── 01_v2_hero_front.png
│       │   │   ├── 02_v2_rear_plss.png
│       │   │   ├── 03_v2_helmet_closeup.png
│       │   │   ├── 04_v2_elbow_bellows_closeup.png
│       │   │   ├── 05_v2_glove_fingers_closeup.png
│       │   │   ├── 06_v2_knee_bellows_closeup.png
│       │   │   ├── 07_v2_boot_cleats_closeup.png
│       │   │   ├── 08_v2_wireframe_topology.png
│       │   │   └── render_v2_obj_views.py
│       │   └── gate3_baseline_manifest.sha256
│       ├── ARES_EVA_MK1_v3/
│       │   ├── blender/
│       │   │   ├── ARES_EVA_MK1_v2_Master.blend
│       │   │   ├── ARES_EVA_MK1_v2_Master.blend1
│       │   │   ├── ARES_EVA_MK1_v3_Master.blend
│       │   │   ├── bake_pbr_textures.py
│       │   │   ├── generate_ares_eva_mk1_v2_production.py
│       │   │   ├── generate_ares_eva_mk1_v3_production.py
│       │   │   ├── render_preview_v3.py
│       │   │   └── render_preview_v3_enhanced.py
│       │   ├── models/
│       │   │   ├── ARES_EVA_MK1_v2_LOD0.mtl
│       │   │   ├── ARES_EVA_MK1_v2_LOD0.obj
│       │   │   ├── ARES_EVA_MK1_v2_LOD1.mtl
│       │   │   ├── ARES_EVA_MK1_v2_LOD1.obj
│       │   │   ├── ARES_EVA_MK1_v2_LOD2.mtl
│       │   │   ├── ARES_EVA_MK1_v2_LOD2.obj
│       │   │   ├── ARES_EVA_MK1_v2_LOD3.mtl
│       │   │   ├── ARES_EVA_MK1_v2_LOD3.obj
│       │   │   ├── ARES_EVA_MK1_v2_Rigged.fbx
│       │   │   ├── ARES_EVA_MK1_v2_Rigged.glb
│       │   │   ├── ARES_EVA_MK1_v3_LOD0.mtl
│       │   │   ├── ARES_EVA_MK1_v3_LOD0.obj
│       │   │   ├── ARES_EVA_MK1_v3_LOD1.mtl
│       │   │   ├── ARES_EVA_MK1_v3_LOD1.obj
│       │   │   ├── ARES_EVA_MK1_v3_LOD2.mtl
│       │   │   ├── ARES_EVA_MK1_v3_LOD2.obj
│       │   │   ├── ARES_EVA_MK1_v3_LOD3.mtl
│       │   │   ├── ARES_EVA_MK1_v3_LOD3.obj
│       │   │   ├── ARES_EVA_MK1_v3_Rigged.fbx
│       │   │   └── ARES_EVA_MK1_v3_Rigged.glb
│       │   ├── renders/
│       │   │   ├── 01_v2_hero_front.png
│       │   │   ├── 02_v2_rear_plss.png
│       │   │   ├── 03_v2_helmet_closeup.png
│       │   │   ├── 04_v2_elbow_bellows_closeup.png
│       │   │   ├── 05_v2_glove_fingers_closeup.png
│       │   │   ├── 06_v2_knee_bellows_closeup.png
│       │   │   ├── 07_v2_boot_cleats_closeup.png
│       │   │   ├── 08_v2_wireframe_topology.png
│       │   │   ├── ARES_EVA_MK1_v3_preview.png
│       │   │   ├── ARES_EVA_MK1_v3_preview_enhanced.png
│       │   │   └── render_v2_obj_views.py
│       │   └── textures/
│       │       ├── ARES_EVA_BaseColor.png
│       │       ├── ARES_EVA_DustMask.png
│       │       ├── ARES_EVA_Emission.png
│       │       ├── ARES_EVA_MetallicSmoothness.png
│       │       ├── ARES_EVA_Normal.png
│       │       ├── ARES_EVA_Occlusion.png
│       │       └── test.png
│       └── AstroGatewayAstronaut/
│           └── README.md
├── 02-Software-Development/
│   ├── Backend/
│   │   └── astrogateway-api/
│   │       ├── certs/
│   │       │   ├── prod-ca-2021 (1).crt
│   │       │   └── supabase-ca.crt
│   │       ├── rules/
│   │       │   ├── __init__.py
│   │       │   ├── catalog.py
│   │       │   ├── config.py
│   │       │   ├── life_support.py
│   │       │   ├── mission_rules_v1.json
│   │       │   ├── recommendations.py
│   │       │   ├── scoring.py
│   │       │   ├── sensor_catalog_v1.json
│   │       │   ├── skills.py
│   │       │   ├── test_vectors_v1.json
│   │       │   ├── timeline.py
│   │       │   └── weather.py
│   │       ├── tests/
│   │       │   ├── local_mock_server.py
│   │       │   ├── test_hardening.py
│   │       │   ├── test_mission_commands.py
│   │       │   ├── test_postgrest_repository.py
│   │       │   ├── test_rules.py
│   │       │   ├── test_security_contract.py
│   │       │   ├── test_sensor_records.py
│   │       │   └── test_unified_profile.py
│   │       ├── .env.example
│   │       ├── database.py
│   │       ├── main.py
│   │       ├── README.md
│   │       ├── requirements-dev.txt
│   │       ├── requirements.txt
│   │       ├── run_local.py
│   │       └── sensor_records.py
│   ├── Integrations/
│   │   ├── android-unity-host-poc/
│   │   │   ├── app/
│   │   │   │   ├── src/
│   │   │   │   │   ├── androidTest/
│   │   │   │   │   │   └── java/
│   │   │   │   │   │       └── com/
│   │   │   │   │   │           └── sk2lb/
│   │   │   │   │   │               └── astrogateway/
│   │   │   │   │   │                   └── poc/
│   │   │   │   │   │                       ├── ProfileEditSmokeInstrumentation.kt
│   │   │   │   │   │                       └── SecuritySmokeInstrumentation.kt
│   │   │   │   │   ├── debug/
│   │   │   │   │   │   └── res/
│   │   │   │   │   │       └── xml/
│   │   │   │   │   │           └── network_security.xml
│   │   │   │   │   └── main/
│   │   │   │   │       ├── java/
│   │   │   │   │       │   └── com/
│   │   │   │   │       │       └── sk2lb/
│   │   │   │   │       │           └── astrogateway/
│   │   │   │   │       │               └── poc/
│   │   │   │   │       │                   ├── AuthCoordinator.kt
│   │   │   │   │       │                   ├── MainActivity.kt
│   │   │   │   │       │                   ├── PocUnityActivity.kt
│   │   │   │   │       │                   └── SecureSessionStore.kt
│   │   │   │   │       ├── res/
│   │   │   │   │       │   ├── values/
│   │   │   │   │       │   │   └── astro_theme.xml
│   │   │   │   │       │   └── xml/
│   │   │   │   │       │       └── network_security.xml
│   │   │   │   │       └── AndroidManifest.xml
│   │   │   │   └── build.gradle
│   │   │   ├── RuntimeValidation/
│   │   │   │   ├── Back_Navigation_Attempt/
│   │   │   │   │   ├── 01-dashboard.xml
│   │   │   │   │   ├── 01-native-dashboard.png
│   │   │   │   │   ├── 02-unity-bridge-result.png
│   │   │   │   │   ├── 02-unity-diagnostic.png
│   │   │   │   │   ├── 02-unity-diagnostic.xml
│   │   │   │   │   ├── 03-dashboard-cycle-1.xml
│   │   │   │   │   ├── 03-dashboard-result-cycle-1.png
│   │   │   │   │   ├── back-gesture.xml
│   │   │   │   │   ├── failure-screen.png
│   │   │   │   │   ├── failure-ui.xml
│   │   │   │   │   └── runtime-result.json
│   │   │   │   ├── CaseSensitive_Check_Attempt/
│   │   │   │   │   ├── 01-dashboard.xml
│   │   │   │   │   ├── 01-native-dashboard.png
│   │   │   │   │   ├── 02-unity-bridge-result.png
│   │   │   │   │   ├── 02-unity-diagnostic.png
│   │   │   │   │   ├── 02-unity-diagnostic.xml
│   │   │   │   │   ├── failure-screen.png
│   │   │   │   │   ├── failure-ui.xml
│   │   │   │   │   └── runtime-result.json
│   │   │   │   ├── Initial_Attempt/
│   │   │   │   │   ├── 01-dashboard.xml
│   │   │   │   │   ├── 01-native-dashboard.png
│   │   │   │   │   ├── 02-unity-diagnostic.png
│   │   │   │   │   ├── 02-unity-diagnostic.xml
│   │   │   │   │   └── runtime-result.json
│   │   │   │   ├── 01-dashboard.xml
│   │   │   │   ├── 01-native-dashboard.png
│   │   │   │   ├── 02-unity-bridge-result.png
│   │   │   │   ├── 02-unity-diagnostic.png
│   │   │   │   ├── 02-unity-diagnostic.xml
│   │   │   │   ├── 03-dashboard-cycle-1.xml
│   │   │   │   ├── 03-dashboard-cycle-10.xml
│   │   │   │   ├── 03-dashboard-cycle-5.xml
│   │   │   │   ├── 03-dashboard-result-cycle-1.png
│   │   │   │   ├── 03-dashboard-result-cycle-10.png
│   │   │   │   ├── 03-dashboard-result-cycle-5.png
│   │   │   │   ├── back-gesture.xml
│   │   │   │   ├── failure-screen.png
│   │   │   │   ├── failure-ui.xml
│   │   │   │   └── runtime-result.json
│   │   │   ├── Stage05A/
│   │   │   │   └── backup/
│   │   │   │       ├── AndroidManifest.xml
│   │   │   │       ├── MainActivity.kt
│   │   │   │       └── PocUnityActivity.kt
│   │   │   ├── Stage05B/
│   │   │   │   └── backup/
│   │   │   │       ├── AndroidManifest.xml
│   │   │   │       ├── build.gradle
│   │   │   │       ├── MainActivity.kt
│   │   │   │       └── PocUnityActivity.kt
│   │   │   ├── tools/
│   │   │   │   ├── build.sh
│   │   │   │   ├── configure_test_db_admin.py
│   │   │   │   ├── device_smoke.py
│   │   │   │   ├── export.sh
│   │   │   │   ├── prepare_host.py
│   │   │   │   └── setup_backend.sh
│   │   │   ├── unity-export/
│   │   │   │   ├── gradle/
│   │   │   │   │   └── wrapper/
│   │   │   │   │       └── gradle-wrapper.properties
│   │   │   │   ├── launcher/
│   │   │   │   │   ├── src/
│   │   │   │   │   │   └── main/
│   │   │   │   │   │       ├── res/
│   │   │   │   │   │       │   ├── mipmap-anydpi-v26/
│   │   │   │   │   │       │   │   └── app_icon.xml
│   │   │   │   │   │       │   ├── mipmap-mdpi/
│   │   │   │   │   │       │   │   ├── app_icon.png
│   │   │   │   │   │       │   │   ├── ic_launcher_background.png
│   │   │   │   │   │       │   │   └── ic_launcher_foreground.png
│   │   │   │   │   │       │   └── values/
│   │   │   │   │   │       │       └── strings.xml
│   │   │   │   │   │       └── AndroidManifest.xml
│   │   │   │   │   ├── build.gradle
│   │   │   │   │   └── setupSymbols.gradle
│   │   │   │   ├── shared/
│   │   │   │   │   ├── common.gradle
│   │   │   │   │   └── keepUnitySymbols.gradle
│   │   │   │   ├── unityLibrary/
│   │   │   │   │   ├── libs/
│   │   │   │   │   │   └── unity-classes.jar
│   │   │   │   │   ├── src/
│   │   │   │   │   │   └── main/
│   │   │   │   │   │       ├── assets/
│   │   │   │   │   │       │   └── bin/
│   │   │   │   │   │       │       └── Data/
│   │   │   │   │   │       │           ├── Managed/
│   │   │   │   │   │       │           │   ├── Metadata/
│   │   │   │   │   │       │           │   │   └── global-metadata.dat
│   │   │   │   │   │       │           │   └── Resources/
│   │   │   │   │   │       │           │       └── mscorlib.dll-resources.dat
│   │   │   │   │   │       │           ├── Resources/
│   │   │   │   │   │       │           │   ├── unity default resources
│   │   │   │   │   │       │           │   └── unity_builtin_extra
│   │   │   │   │   │       │           ├── 621226d1ca35a45e3bbbf62e3fc890d8
│   │   │   │   │   │       │           ├── boot.config
│   │   │   │   │   │       │           ├── c28ec6aa9a49f47bb80d78eb2e6e6aff
│   │   │   │   │   │       │           ├── globalgamemanagers
│   │   │   │   │   │       │           ├── globalgamemanagers.assets.split0
│   │   │   │   │   │       │           ├── globalgamemanagers.assets.split1
│   │   │   │   │   │       │           ├── globalgamemanagers.assets.split2
│   │   │   │   │   │       │           ├── level0
│   │   │   │   │   │       │           ├── RuntimeInitializeOnLoads.json
│   │   │   │   │   │       │           ├── ScriptingAssemblies.json
│   │   │   │   │   │       │           ├── sharedassets0.assets
│   │   │   │   │   │       │           └── unity_app_guid
│   │   │   │   │   │       ├── java/
│   │   │   │   │   │       │   └── com/
│   │   │   │   │   │       │       └── unity3d/
│   │   │   │   │   │       │           └── player/
│   │   │   │   │   │       │               └── UnityPlayerActivity.java
│   │   │   │   │   │       ├── jniLibs/
│   │   │   │   │   │       │   └── arm64-v8a/
│   │   │   │   │   │       │       ├── lib_burst_generated.so
│   │   │   │   │   │       │       ├── libil2cpp.so
│   │   │   │   │   │       │       ├── libmain.so
│   │   │   │   │   │       │       └── libunity.so
│   │   │   │   │   │       ├── jniStaticLibs/
│   │   │   │   │   │       │   └── arm64-v8a/
│   │   │   │   │   │       │       └── baselib.a
│   │   │   │   │   │       ├── res/
│   │   │   │   │   │       │   ├── values/
│   │   │   │   │   │       │   │   ├── colors.xml
│   │   │   │   │   │       │   │   ├── freeformwindow.xml
│   │   │   │   │   │       │   │   ├── ids.xml
│   │   │   │   │   │       │   │   ├── strings.xml
│   │   │   │   │   │       │   │   └── styles.xml
│   │   │   │   │   │       │   ├── values-v21/
│   │   │   │   │   │       │   │   └── styles.xml
│   │   │   │   │   │       │   ├── values-v30/
│   │   │   │   │   │       │   │   └── freeformwindow.xml
│   │   │   │   │   │       │   └── values-v31/
│   │   │   │   │   │       │       └── styles.xml
│   │   │   │   │   │       ├── resources/
│   │   │   │   │   │       │   └── META-INF/
│   │   │   │   │   │       │       └── com.android.games.engine.build_fingerprint
│   │   │   │   │   │       └── AndroidManifest.xml
│   │   │   │   │   ├── symbols/
│   │   │   │   │   │   └── arm64-v8a/
│   │   │   │   │   │       └── libil2cpp.so
│   │   │   │   │   ├── build.gradle
│   │   │   │   │   └── proguard-unity.txt
│   │   │   │   ├── build.gradle
│   │   │   │   ├── gradle.properties
│   │   │   │   ├── settings.gradle
│   │   │   │   └── settings.unity-original.gradle.txt
│   │   │   ├── UnityIntegrationProject/
│   │   │   │   ├── Assets/
│   │   │   │   │   ├── AstroGateway/
│   │   │   │   │   │   ├── Environment/
│   │   │   │   │   │   │   ├── Architecture/
│   │   │   │   │   │   │   │   ├── AgricultureG01_ChamferShell.asset
│   │   │   │   │   │   │   │   ├── AgricultureG01_ChamferShell.asset.meta
│   │   │   │   │   │   │   │   ├── AgricultureG01_Ring.asset
│   │   │   │   │   │   │   │   ├── AgricultureG01_Ring.asset.meta
│   │   │   │   │   │   │   │   ├── AirlockShell.asset
│   │   │   │   │   │   │   │   ├── AirlockShell.asset.meta
│   │   │   │   │   │   │   │   ├── C_ScienceStaging_Accent_Cyan_Combined.asset
│   │   │   │   │   │   │   │   ├── C_ScienceStaging_Accent_Cyan_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── C_ScienceStaging_Accent_SafetyOrange_Combined.asset
│   │   │   │   │   │   │   │   ├── C_ScienceStaging_Accent_SafetyOrange_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── C_ScienceStaging_Glass_Utility_Combined.asset
│   │   │   │   │   │   │   │   ├── C_ScienceStaging_Glass_Utility_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── C_ScienceStaging_Habitat_CompositeDark_Combined.asset
│   │   │   │   │   │   │   │   ├── C_ScienceStaging_Habitat_CompositeDark_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── C_ScienceStaging_Habitat_OffWhite_Combined.asset
│   │   │   │   │   │   │   │   ├── C_ScienceStaging_Habitat_OffWhite_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── C_ScienceStaging_Metal_Industrial_Combined.asset
│   │   │   │   │   │   │   │   ├── C_ScienceStaging_Metal_Industrial_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── G_Greenhouse_Accent_Cyan_Combined.asset
│   │   │   │   │   │   │   │   ├── G_Greenhouse_Accent_Cyan_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── G_Greenhouse_Glass_Utility_Combined.asset
│   │   │   │   │   │   │   │   ├── G_Greenhouse_Glass_Utility_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── G_Greenhouse_Habitat_CompositeDark_Combined.asset
│   │   │   │   │   │   │   │   ├── G_Greenhouse_Habitat_CompositeDark_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── G_Greenhouse_Habitat_OffWhite_Combined.asset
│   │   │   │   │   │   │   │   ├── G_Greenhouse_Habitat_OffWhite_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── G_Greenhouse_Metal_Industrial_Combined.asset
│   │   │   │   │   │   │   │   ├── G_Greenhouse_Metal_Industrial_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── H_CommandHabitat_Accent_Cyan_Combined.asset
│   │   │   │   │   │   │   │   ├── H_CommandHabitat_Accent_Cyan_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── H_CommandHabitat_Glass_Utility_Combined.asset
│   │   │   │   │   │   │   │   ├── H_CommandHabitat_Glass_Utility_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── H_CommandHabitat_Habitat_CompositeDark_Combined.asset
│   │   │   │   │   │   │   │   ├── H_CommandHabitat_Habitat_CompositeDark_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── H_CommandHabitat_Habitat_OffWhite_Combined.asset
│   │   │   │   │   │   │   │   ├── H_CommandHabitat_Habitat_OffWhite_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── H_CommandHabitat_Metal_Industrial_Combined.asset
│   │   │   │   │   │   │   │   ├── H_CommandHabitat_Metal_Industrial_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── HabitatH01_ChamferShell.asset
│   │   │   │   │   │   │   │   ├── HabitatH01_ChamferShell.asset.meta
│   │   │   │   │   │   │   │   ├── HabitatH01_Ring.asset
│   │   │   │   │   │   │   │   ├── HabitatH01_Ring.asset.meta
│   │   │   │   │   │   │   │   ├── P_PowerStation_Accent_Cyan_Combined.asset
│   │   │   │   │   │   │   │   ├── P_PowerStation_Accent_Cyan_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── P_PowerStation_Accent_SafetyOrange_Combined.asset
│   │   │   │   │   │   │   │   ├── P_PowerStation_Accent_SafetyOrange_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── P_PowerStation_Glass_Utility_Combined.asset
│   │   │   │   │   │   │   │   ├── P_PowerStation_Glass_Utility_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── P_PowerStation_Habitat_CompositeDark_Combined.asset
│   │   │   │   │   │   │   │   ├── P_PowerStation_Habitat_CompositeDark_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── P_PowerStation_Habitat_OffWhite_Combined.asset
│   │   │   │   │   │   │   │   ├── P_PowerStation_Habitat_OffWhite_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── P_PowerStation_Metal_Industrial_Combined.asset
│   │   │   │   │   │   │   │   ├── P_PowerStation_Metal_Industrial_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── R_RoverBay_Habitat_CompositeDark_Combined.asset
│   │   │   │   │   │   │   │   ├── R_RoverBay_Habitat_CompositeDark_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── R_RoverBay_Habitat_OffWhite_Combined.asset
│   │   │   │   │   │   │   │   ├── R_RoverBay_Habitat_OffWhite_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── R_RoverBay_Mars_Regolith_Dark_Combined.asset
│   │   │   │   │   │   │   │   ├── R_RoverBay_Mars_Regolith_Dark_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── R_RoverBay_Mars_Rock_Basaltic_Combined.asset
│   │   │   │   │   │   │   │   ├── R_RoverBay_Mars_Rock_Basaltic_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── R_RoverBay_Metal_Industrial_Combined.asset
│   │   │   │   │   │   │   │   ├── R_RoverBay_Metal_Industrial_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── S_SolarArray_Habitat_CompositeDark_Combined.asset
│   │   │   │   │   │   │   │   ├── S_SolarArray_Habitat_CompositeDark_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── S_SolarArray_Habitat_OffWhite_Combined.asset
│   │   │   │   │   │   │   │   ├── S_SolarArray_Habitat_OffWhite_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── S_SolarArray_Metal_Industrial_Combined.asset
│   │   │   │   │   │   │   │   ├── S_SolarArray_Metal_Industrial_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── S_SolarArray_SolarPanel_Combined.asset
│   │   │   │   │   │   │   │   └── S_SolarArray_SolarPanel_Combined.asset.meta
│   │   │   │   │   │   │   ├── Editor/
│   │   │   │   │   │   │   │   ├── MarsEnvironmentBuilder.cs
│   │   │   │   │   │   │   │   └── MarsEnvironmentBuilder.cs.meta
│   │   │   │   │   │   │   ├── Materials/
│   │   │   │   │   │   │   │   ├── Accent_Cyan.mat
│   │   │   │   │   │   │   │   ├── Accent_Cyan.mat.meta
│   │   │   │   │   │   │   │   ├── Accent_SafetyOrange.mat
│   │   │   │   │   │   │   │   ├── Accent_SafetyOrange.mat.meta
│   │   │   │   │   │   │   │   ├── Basalt_Surface_512.png
│   │   │   │   │   │   │   │   ├── Basalt_Surface_512.png.meta
│   │   │   │   │   │   │   │   ├── Composite_DustPaint_512.png
│   │   │   │   │   │   │   │   ├── Composite_DustPaint_512.png.meta
│   │   │   │   │   │   │   │   ├── Delta_Bedding_512.png
│   │   │   │   │   │   │   │   ├── Delta_Bedding_512.png.meta
│   │   │   │   │   │   │   │   ├── Delta_FracturedStrata_512.png
│   │   │   │   │   │   │   │   ├── Delta_FracturedStrata_512.png.meta
│   │   │   │   │   │   │   │   ├── DustSky.shader
│   │   │   │   │   │   │   │   ├── DustSky.shader.meta
│   │   │   │   │   │   │   │   ├── Glass_Utility.mat
│   │   │   │   │   │   │   │   ├── Glass_Utility.mat.meta
│   │   │   │   │   │   │   │   ├── Habitat_CompositeDark.mat
│   │   │   │   │   │   │   │   ├── Habitat_CompositeDark.mat.meta
│   │   │   │   │   │   │   │   ├── Habitat_OffWhite.mat
│   │   │   │   │   │   │   │   ├── Habitat_OffWhite.mat.meta
│   │   │   │   │   │   │   │   ├── Mars_DustSky.mat
│   │   │   │   │   │   │   │   ├── Mars_DustSky.mat.meta
│   │   │   │   │   │   │   │   ├── Mars_Regolith.mat
│   │   │   │   │   │   │   │   ├── Mars_Regolith.mat.meta
│   │   │   │   │   │   │   │   ├── Mars_Regolith_Dark.mat
│   │   │   │   │   │   │   │   ├── Mars_Regolith_Dark.mat.meta
│   │   │   │   │   │   │   │   ├── Mars_Rock_Basaltic.mat
│   │   │   │   │   │   │   │   ├── Mars_Rock_Basaltic.mat.meta
│   │   │   │   │   │   │   │   ├── Mars_Rock_Layered.mat
│   │   │   │   │   │   │   │   ├── Mars_Rock_Layered.mat.meta
│   │   │   │   │   │   │   │   ├── Metal_Industrial.mat
│   │   │   │   │   │   │   │   ├── Metal_Industrial.mat.meta
│   │   │   │   │   │   │   │   ├── Regolith_Grain_512.png
│   │   │   │   │   │   │   │   ├── Regolith_Grain_512.png.meta
│   │   │   │   │   │   │   │   ├── Regolith_Macro_Pathwear_1024.png
│   │   │   │   │   │   │   │   ├── Regolith_Macro_Pathwear_1024.png.meta
│   │   │   │   │   │   │   │   ├── Regolith_MicroNormal_256.png
│   │   │   │   │   │   │   │   ├── Regolith_MicroNormal_256.png.meta
│   │   │   │   │   │   │   │   ├── Solar_Cells_Dust_512.png
│   │   │   │   │   │   │   │   ├── Solar_Cells_Dust_512.png.meta
│   │   │   │   │   │   │   │   ├── SolarPanel.mat
│   │   │   │   │   │   │   │   └── SolarPanel.mat.meta
│   │   │   │   │   │   │   ├── Prefabs/
│   │   │   │   │   │   │   │   ├── AngularBasalt.prefab
│   │   │   │   │   │   │   │   ├── AngularBasalt.prefab.meta
│   │   │   │   │   │   │   │   ├── C_ScienceStaging.prefab
│   │   │   │   │   │   │   │   ├── C_ScienceStaging.prefab.meta
│   │   │   │   │   │   │   │   ├── ErodedLandmark.prefab
│   │   │   │   │   │   │   │   ├── ErodedLandmark.prefab.meta
│   │   │   │   │   │   │   │   ├── G_Greenhouse.prefab
│   │   │   │   │   │   │   │   ├── G_Greenhouse.prefab.meta
│   │   │   │   │   │   │   │   ├── H_CommandHabitat.prefab
│   │   │   │   │   │   │   │   ├── H_CommandHabitat.prefab.meta
│   │   │   │   │   │   │   │   ├── LayeredDelta.prefab
│   │   │   │   │   │   │   │   ├── LayeredDelta.prefab.meta
│   │   │   │   │   │   │   │   ├── P_PowerStation.prefab
│   │   │   │   │   │   │   │   ├── P_PowerStation.prefab.meta
│   │   │   │   │   │   │   │   ├── R_RoverBay.prefab
│   │   │   │   │   │   │   │   ├── R_RoverBay.prefab.meta
│   │   │   │   │   │   │   │   ├── RegolithStone.prefab
│   │   │   │   │   │   │   │   ├── RegolithStone.prefab.meta
│   │   │   │   │   │   │   │   ├── S_SolarArray.prefab
│   │   │   │   │   │   │   │   └── S_SolarArray.prefab.meta
│   │   │   │   │   │   │   ├── Props/
│   │   │   │   │   │   │   │   ├── ApproachStoneClusters_Mars_Rock_Basaltic_Combined.asset
│   │   │   │   │   │   │   │   ├── ApproachStoneClusters_Mars_Rock_Basaltic_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── HabitatExternalKit_Habitat_CompositeDark_Combined.asset
│   │   │   │   │   │   │   │   ├── HabitatExternalKit_Habitat_CompositeDark_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── HabitatExternalKit_Habitat_OffWhite_Combined.asset
│   │   │   │   │   │   │   │   ├── HabitatExternalKit_Habitat_OffWhite_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── HabitatExternalKit_Metal_Industrial_Combined.asset
│   │   │   │   │   │   │   │   ├── HabitatExternalKit_Metal_Industrial_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── PowerExternalKit_Habitat_CompositeDark_Combined.asset
│   │   │   │   │   │   │   │   ├── PowerExternalKit_Habitat_CompositeDark_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── PowerExternalKit_Mars_Regolith_Dark_Combined.asset
│   │   │   │   │   │   │   │   ├── PowerExternalKit_Mars_Regolith_Dark_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── UtilityRoutePosts_Accent_Cyan_Combined.asset
│   │   │   │   │   │   │   │   ├── UtilityRoutePosts_Accent_Cyan_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── UtilityRoutePosts_Metal_Industrial_Combined.asset
│   │   │   │   │   │   │   │   └── UtilityRoutePosts_Metal_Industrial_Combined.asset.meta
│   │   │   │   │   │   │   ├── Rocks/
│   │   │   │   │   │   │   │   ├── AngularBasalt_LOD0.asset
│   │   │   │   │   │   │   │   ├── AngularBasalt_LOD0.asset.meta
│   │   │   │   │   │   │   │   ├── AngularBasalt_LOD1.asset
│   │   │   │   │   │   │   │   ├── AngularBasalt_LOD1.asset.meta
│   │   │   │   │   │   │   │   ├── AngularBasalt_LOD2.asset
│   │   │   │   │   │   │   │   ├── AngularBasalt_LOD2.asset.meta
│   │   │   │   │   │   │   │   ├── DeltaFractured_LOD0.asset
│   │   │   │   │   │   │   │   ├── DeltaFractured_LOD0.asset.meta
│   │   │   │   │   │   │   │   ├── DeltaFractured_LOD1.asset
│   │   │   │   │   │   │   │   ├── DeltaFractured_LOD1.asset.meta
│   │   │   │   │   │   │   │   ├── DeltaFractured_LOD2.asset
│   │   │   │   │   │   │   │   ├── DeltaFractured_LOD2.asset.meta
│   │   │   │   │   │   │   │   ├── ErodedLandmark_LOD0.asset
│   │   │   │   │   │   │   │   ├── ErodedLandmark_LOD0.asset.meta
│   │   │   │   │   │   │   │   ├── ErodedLandmark_LOD1.asset
│   │   │   │   │   │   │   │   ├── ErodedLandmark_LOD1.asset.meta
│   │   │   │   │   │   │   │   ├── ErodedLandmark_LOD2.asset
│   │   │   │   │   │   │   │   ├── ErodedLandmark_LOD2.asset.meta
│   │   │   │   │   │   │   │   ├── LayeredDelta_LOD0.asset
│   │   │   │   │   │   │   │   ├── LayeredDelta_LOD0.asset.meta
│   │   │   │   │   │   │   │   ├── LayeredDelta_LOD1.asset
│   │   │   │   │   │   │   │   ├── LayeredDelta_LOD1.asset.meta
│   │   │   │   │   │   │   │   ├── LayeredDelta_LOD2.asset
│   │   │   │   │   │   │   │   ├── LayeredDelta_LOD2.asset.meta
│   │   │   │   │   │   │   │   ├── StonePatch_0_Mars_Rock_Basaltic_Combined.asset
│   │   │   │   │   │   │   │   ├── StonePatch_0_Mars_Rock_Basaltic_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── StonePatch_1_Mars_Rock_Basaltic_Combined.asset
│   │   │   │   │   │   │   │   ├── StonePatch_1_Mars_Rock_Basaltic_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── StonePatch_2_Mars_Rock_Basaltic_Combined.asset
│   │   │   │   │   │   │   │   ├── StonePatch_2_Mars_Rock_Basaltic_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── StonePatch_3_Mars_Rock_Basaltic_Combined.asset
│   │   │   │   │   │   │   │   ├── StonePatch_3_Mars_Rock_Basaltic_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── StonePatch_4_Mars_Rock_Basaltic_Combined.asset
│   │   │   │   │   │   │   │   ├── StonePatch_4_Mars_Rock_Basaltic_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── StonePatch_5_Mars_Rock_Basaltic_Combined.asset
│   │   │   │   │   │   │   │   ├── StonePatch_5_Mars_Rock_Basaltic_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── StonePatch_6_Mars_Rock_Basaltic_Combined.asset
│   │   │   │   │   │   │   │   ├── StonePatch_6_Mars_Rock_Basaltic_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── StonePatch_7_Mars_Rock_Basaltic_Combined.asset
│   │   │   │   │   │   │   │   ├── StonePatch_7_Mars_Rock_Basaltic_Combined.asset.meta
│   │   │   │   │   │   │   │   ├── StonePatch_8_Mars_Rock_Basaltic_Combined.asset
│   │   │   │   │   │   │   │   └── StonePatch_8_Mars_Rock_Basaltic_Combined.asset.meta
│   │   │   │   │   │   │   ├── Terrain/
│   │   │   │   │   │   │   │   ├── Regolith_250m_PreservedCorridor.asset
│   │   │   │   │   │   │   │   └── Regolith_250m_PreservedCorridor.asset.meta
│   │   │   │   │   │   │   ├── Architecture.meta
│   │   │   │   │   │   │   ├── Editor.meta
│   │   │   │   │   │   │   ├── Materials.meta
│   │   │   │   │   │   │   ├── Prefabs.meta
│   │   │   │   │   │   │   ├── Props.meta
│   │   │   │   │   │   │   ├── Rocks.meta
│   │   │   │   │   │   │   └── Terrain.meta
│   │   │   │   │   │   ├── Greybox/
│   │   │   │   │   │   │   ├── Editor/
│   │   │   │   │   │   │   │   ├── MarsGreyboxBuilder.cs
│   │   │   │   │   │   │   │   └── MarsGreyboxBuilder.cs.meta
│   │   │   │   │   │   │   ├── Materials/
│   │   │   │   │   │   │   │   ├── DarkRock.mat
│   │   │   │   │   │   │   │   ├── DarkRock.mat.meta
│   │   │   │   │   │   │   │   ├── HabitatPale.mat
│   │   │   │   │   │   │   │   ├── HabitatPale.mat.meta
│   │   │   │   │   │   │   │   ├── IndustrialGraphite.mat
│   │   │   │   │   │   │   │   ├── IndustrialGraphite.mat.meta
│   │   │   │   │   │   │   │   ├── LayeredStone.mat
│   │   │   │   │   │   │   │   ├── LayeredStone.mat.meta
│   │   │   │   │   │   │   │   ├── SandTan.mat
│   │   │   │   │   │   │   │   ├── SandTan.mat.meta
│   │   │   │   │   │   │   │   ├── TerrainOchre.mat
│   │   │   │   │   │   │   │   ├── TerrainOchre.mat.meta
│   │   │   │   │   │   │   │   ├── WayfindingAmber.mat
│   │   │   │   │   │   │   │   ├── WayfindingAmber.mat.meta
│   │   │   │   │   │   │   │   ├── WayfindingCyan.mat
│   │   │   │   │   │   │   │   └── WayfindingCyan.mat.meta
│   │   │   │   │   │   │   ├── Meshes/
│   │   │   │   │   │   │   │   ├── Ground.asset
│   │   │   │   │   │   │   │   └── Ground.asset.meta
│   │   │   │   │   │   │   ├── Editor.meta
│   │   │   │   │   │   │   ├── MarsOutpost_Greybox.unity
│   │   │   │   │   │   │   ├── MarsOutpost_Greybox.unity.meta
│   │   │   │   │   │   │   ├── Materials.meta
│   │   │   │   │   │   │   ├── Meshes.meta
│   │   │   │   │   │   │   ├── PreGreybox_Untitled_Backup.unity
│   │   │   │   │   │   │   └── PreGreybox_Untitled_Backup.unity.meta
│   │   │   │   │   │   ├── IntegrationPOC/
│   │   │   │   │   │   │   ├── Editor/
│   │   │   │   │   │   │   │   ├── AndroidPocExporter.cs
│   │   │   │   │   │   │   │   └── AndroidPocExporter.cs.meta
│   │   │   │   │   │   │   ├── AstroGatewayPocBridge.cs
│   │   │   │   │   │   │   ├── AstroGatewayPocBridge.cs.meta
│   │   │   │   │   │   │   ├── Editor.meta
│   │   │   │   │   │   │   ├── link.xml
│   │   │   │   │   │   │   ├── link.xml.meta
│   │   │   │   │   │   │   ├── POC_Cube.mat
│   │   │   │   │   │   │   ├── POC_Cube.mat.meta
│   │   │   │   │   │   │   ├── UnityIntegration_Test.unity
│   │   │   │   │   │   │   └── UnityIntegration_Test.unity.meta
│   │   │   │   │   │   ├── Scenes/
│   │   │   │   │   │   │   ├── MarsOutpost_Environment_v01.unity
│   │   │   │   │   │   │   └── MarsOutpost_Environment_v01.unity.meta
│   │   │   │   │   │   ├── Environment.meta
│   │   │   │   │   │   ├── Greybox.meta
│   │   │   │   │   │   ├── IntegrationPOC.meta
│   │   │   │   │   │   └── Scenes.meta
│   │   │   │   │   ├── Scenes/
│   │   │   │   │   │   ├── SampleScene.unity
│   │   │   │   │   │   └── SampleScene.unity.meta
│   │   │   │   │   ├── Settings/
│   │   │   │   │   │   ├── DefaultVolumeProfile.asset
│   │   │   │   │   │   ├── DefaultVolumeProfile.asset.meta
│   │   │   │   │   │   ├── Mobile_Renderer.asset
│   │   │   │   │   │   ├── Mobile_Renderer.asset.meta
│   │   │   │   │   │   ├── Mobile_RPAsset.asset
│   │   │   │   │   │   ├── Mobile_RPAsset.asset.meta
│   │   │   │   │   │   ├── PC_Renderer.asset
│   │   │   │   │   │   ├── PC_Renderer.asset.meta
│   │   │   │   │   │   ├── PC_RPAsset.asset
│   │   │   │   │   │   ├── PC_RPAsset.asset.meta
│   │   │   │   │   │   ├── SampleSceneProfile.asset
│   │   │   │   │   │   ├── SampleSceneProfile.asset.meta
│   │   │   │   │   │   ├── UniversalRenderPipelineGlobalSettings.asset
│   │   │   │   │   │   └── UniversalRenderPipelineGlobalSettings.asset.meta
│   │   │   │   │   ├── TutorialInfo/
│   │   │   │   │   │   ├── Editor/
│   │   │   │   │   │   │   ├── ReadmeEditor.cs
│   │   │   │   │   │   │   └── ReadmeEditor.cs.meta
│   │   │   │   │   │   ├── Icons/
│   │   │   │   │   │   │   ├── URP.png
│   │   │   │   │   │   │   └── URP.png.meta
│   │   │   │   │   │   ├── StyleSheets/
│   │   │   │   │   │   │   ├── ReadmeEditor.uss
│   │   │   │   │   │   │   ├── ReadmeEditor.uss.meta
│   │   │   │   │   │   │   ├── ReadmeEditorDark.uss
│   │   │   │   │   │   │   ├── ReadmeEditorDark.uss.meta
│   │   │   │   │   │   │   ├── ReadmeEditorLight.uss
│   │   │   │   │   │   │   └── ReadmeEditorLight.uss.meta
│   │   │   │   │   │   ├── Editor.meta
│   │   │   │   │   │   ├── Icons.meta
│   │   │   │   │   │   ├── Layout.wlt
│   │   │   │   │   │   ├── Layout.wlt.meta
│   │   │   │   │   │   ├── Readme.cs
│   │   │   │   │   │   ├── Readme.cs.meta
│   │   │   │   │   │   └── StyleSheets.meta
│   │   │   │   │   ├── AstroGateway.meta
│   │   │   │   │   ├── InputSystem_Actions.inputactions
│   │   │   │   │   ├── InputSystem_Actions.inputactions.meta
│   │   │   │   │   ├── Readme.asset
│   │   │   │   │   ├── Readme.asset.meta
│   │   │   │   │   ├── Scenes.meta
│   │   │   │   │   ├── Settings.meta
│   │   │   │   │   └── TutorialInfo.meta
│   │   │   │   ├── Packages/
│   │   │   │   │   ├── manifest.json
│   │   │   │   │   └── packages-lock.json
│   │   │   │   └── ProjectSettings/
│   │   │   │       ├── AudioManager.asset
│   │   │   │       ├── ClusterInputManager.asset
│   │   │   │       ├── DynamicsManager.asset
│   │   │   │       ├── EditorBuildSettings.asset
│   │   │   │       ├── EditorSettings.asset
│   │   │   │       ├── GraphicsSettings.asset
│   │   │   │       ├── InputManager.asset
│   │   │   │       ├── MemorySettings.asset
│   │   │   │       ├── MultiplayerManager.asset
│   │   │   │       ├── NavMeshAreas.asset
│   │   │   │       ├── PackageManagerSettings.asset
│   │   │   │       ├── Physics2DSettings.asset
│   │   │   │       ├── PhysicsCoreProjectSettings2D.asset
│   │   │   │       ├── PresetManager.asset
│   │   │   │       ├── ProjectAuditorSettings.asset
│   │   │   │       ├── ProjectSettings.asset
│   │   │   │       ├── ProjectVersion.txt
│   │   │   │       ├── QualitySettings.asset
│   │   │   │       ├── SceneTemplateSettings.json
│   │   │   │       ├── ShaderGraphSettings.asset
│   │   │   │       ├── TagManager.asset
│   │   │   │       ├── TimeManager.asset
│   │   │   │       ├── UnityConnectSettings.asset
│   │   │   │       ├── URPProjectSettings.asset
│   │   │   │       ├── VersionControlSettings.asset
│   │   │   │       ├── VFXManager.asset
│   │   │   │       └── XRSettings.asset
│   │   │   ├── .gitignore
│   │   │   ├── auth.local.properties.example
│   │   │   ├── README.md
│   │   │   └── test-accounts.example.json
│   │   └── experimental/
│   │       ├── stage07a-adaptive/
│   │       │   ├── backend/
│   │       │   │   ├── adaptive/
│   │       │   │   │   ├── __init__.py
│   │       │   │   │   ├── api.py
│   │       │   │   │   ├── engine.py
│   │       │   │   │   ├── repository.py
│   │       │   │   │   └── schemas.py
│   │       │   │   ├── tests/
│   │       │   │   │   ├── test_adaptive.py
│   │       │   │   │   ├── test_hardening.py
│   │       │   │   │   ├── test_postgrest_repository.py
│   │       │   │   │   └── test_security_contract.py
│   │       │   │   ├── database.py
│   │       │   │   ├── main.py
│   │       │   │   ├── requirements.txt
│   │       │   │   └── run_local.py
│   │       │   ├── database/
│   │       │   │   └── 007a_local_adaptive_candidate.sql
│   │       │   ├── .gitignore
│   │       │   └── README.md
│   │       └── stage07b-aria/
│   │           ├── stage07b/
│   │           │   ├── __init__.py
│   │           │   ├── aria.py
│   │           │   ├── dataset.py
│   │           │   ├── ingest.py
│   │           │   ├── model.py
│   │           │   ├── privacy.py
│   │           │   └── witness.py
│   │           ├── tests/
│   │           │   └── test_pipeline.py
│   │           ├── offline.sb
│   │           ├── probe.py
│   │           └── run_local.py
│   ├── Sentinel-Database/
│   │   ├── 001_identity_and_mission_sessions.sql
│   │   └── admin.env.example
│   └── Web-Dashboards/
│       └── astrogateway-web/
│           ├── public/
│           │   ├── favicon.svg
│           │   └── icons.svg
│           ├── src/
│           │   ├── account/
│           │   │   ├── account.css
│           │   │   ├── AccountApp.test.tsx
│           │   │   ├── AccountApp.tsx
│           │   │   ├── AccountContext.tsx
│           │   │   ├── accountState.ts
│           │   │   ├── client.test.ts
│           │   │   ├── client.ts
│           │   │   └── test-setup.ts
│           │   ├── assets/
│           │   │   ├── hero.png
│           │   │   ├── react.svg
│           │   │   └── vite.svg
│           │   ├── App.css
│           │   ├── App.tsx
│           │   ├── index.css
│           │   └── main.tsx
│           ├── .env.example
│           ├── .gitignore
│           ├── .oxlintrc.json
│           ├── index.html
│           ├── package-lock.json
│           ├── package.json
│           ├── README.md
│           ├── tsconfig.app.json
│           ├── tsconfig.json
│           ├── tsconfig.node.json
│           ├── vite.config.ts
│           └── vitest.config.ts
├── docs/
│   ├── diagrams/
│   │   ├── architecture-flowchart.svg
│   │   └── architecture-mindmap.svg
│   ├── history/
│   │   ├── codex-outputs/
│   │   │   ├── AndroidPOC/
│   │   │   │   ├── APK_Inspection.json
│   │   │   │   ├── Completion_Report.md
│   │   │   │   └── Preservation_Check.json
│   │   │   ├── AndroidValidation/
│   │   │   │   ├── Evidence/
│   │   │   │   │   ├── 01-dashboard.xml
│   │   │   │   │   ├── 03-dashboard-cycle-10.xml
│   │   │   │   │   ├── back-failure-attempt.json
│   │   │   │   │   ├── back-gesture.xml
│   │   │   │   │   └── runtime-result.json
│   │   │   │   ├── Continuation_Test.py
│   │   │   │   ├── Device_Connection_Guide.md
│   │   │   │   ├── Preservation_Check.json
│   │   │   │   ├── Runtime_Test.py
│   │   │   │   └── Runtime_Validation_Report.md
│   │   │   ├── Architecture/
│   │   │   │   ├── 00_Read_Me.md
│   │   │   │   ├── 01_System_And_Platform.md
│   │   │   │   ├── 02_Identity_And_User_Journey.md
│   │   │   │   ├── 03_Data_API_And_Learning.md
│   │   │   │   ├── 04_Delivery_And_Roadmap.md
│   │   │   │   ├── 05_Evidence_And_Decisions.md
│   │   │   │   └── Preservation_Check.json
│   │   │   ├── CinematicHUD/
│   │   │   │   ├── HistoricalTrials/
│   │   │   │   │   └── Capture_Timeout_Exception.txt
│   │   │   │   ├── MarsRegression/
│   │   │   │   │   └── Evidence/
│   │   │   │   │       ├── Console_Counts.txt
│   │   │   │   │       ├── Editor_Frame_Samples.csv
│   │   │   │   │       ├── Provisional_Synthetic_Telemetry.json
│   │   │   │   │       ├── Resolved_Pretest_Console_Counts.txt
│   │   │   │   │       ├── Scripted_Play_Mode_Tests.txt
│   │   │   │   │       └── Walking_Provisional_Synthetic_Telemetry.json
│   │   │   │   ├── Mission01Regression/
│   │   │   │   │   ├── PlayMode_Controlled_Tests.txt
│   │   │   │   │   └── Test_Local_Save.json
│   │   │   │   ├── NativeRegression/
│   │   │   │   │   ├── Evidence/
│   │   │   │   │   │   ├── Export/
│   │   │   │   │   │   │   ├── AstroGateway_Simulated_Scans.csv
│   │   │   │   │   │   │   ├── AstroGateway_Simulated_Scans.json
│   │   │   │   │   │   │   └── AstroGateway_Simulated_Telemetry.json
│   │   │   │   │   │   └── Local_Test_Snapshot.json
│   │   │   │   │   └── Controlled_PlayMode_Tests.txt
│   │   │   │   ├── VisualPolishRegression/
│   │   │   │   │   ├── Export/
│   │   │   │   │   │   ├── AstroGateway_Simulated_Scans.csv
│   │   │   │   │   │   ├── AstroGateway_Simulated_Scans.json
│   │   │   │   │   │   └── AstroGateway_Simulated_Telemetry.json
│   │   │   │   │   ├── Controlled_PlayMode_Tests.txt
│   │   │   │   │   ├── Editor_Asset_Metrics.txt
│   │   │   │   │   └── Local_Test_Save.json
│   │   │   │   ├── Android_Device_Status.json
│   │   │   │   ├── Android_Testing_Status.md
│   │   │   │   ├── Architecture_Report.md
│   │   │   │   ├── Asset_And_Font_Provenance.md
│   │   │   │   ├── Execution_Results.json
│   │   │   │   ├── Existing_File_Preservation_Report.md
│   │   │   │   ├── Final_Asset_Audit.txt
│   │   │   │   ├── Final_Execution_Summary.json
│   │   │   │   ├── Final_Report.md
│   │   │   │   ├── HUD_Controlled_Tests.txt
│   │   │   │   ├── HUD_Editor_Performance.txt
│   │   │   │   ├── HUD_Test_Save.json
│   │   │   │   ├── Known_Limitations.md
│   │   │   │   ├── Manual_Launch_Instructions.md
│   │   │   │   ├── Native_Control_Integration.md
│   │   │   │   ├── New_Files.json
│   │   │   │   ├── Performance_Report.md
│   │   │   │   ├── Performance_Trial_With_Long_Stall.txt
│   │   │   │   ├── Preservation_Check.json
│   │   │   │   ├── Reference_Comparison_Report.md
│   │   │   │   ├── Responsive_Layout_And_Accessibility.md
│   │   │   │   ├── Responsive_Layout_Checks.txt
│   │   │   │   ├── Responsive_Trial_Editor_Context_Mismatch.txt
│   │   │   │   ├── Runtime_UI_Inventory.txt
│   │   │   │   ├── Scene_Save_Reload.txt
│   │   │   │   ├── Screenshot_Inventory.json
│   │   │   │   ├── Telemetry_Objectives_Scanning_Sync.md
│   │   │   │   ├── Test_Report.md
│   │   │   │   ├── UI_Settings_Contract.md
│   │   │   │   └── Visual_QA_Report.md
│   │   │   ├── Design/
│   │   │   │   ├── 01_Vision_And_Art_Direction.md
│   │   │   │   ├── 02_Mars_World_Layout.md
│   │   │   │   ├── 03_Environment_Asset_List.md
│   │   │   │   ├── 04_Unity_URP_Technical_Standards.md
│   │   │   │   ├── 05_Gameplay_Environment_Integration.md
│   │   │   │   ├── 06_References_And_Attribution.md
│   │   │   │   └── 07_Implementation_Roadmap.md
│   │   │   ├── MarsEnvironment/
│   │   │   │   ├── Export/
│   │   │   │   │   ├── AstroGateway_Simulated_Scans.csv
│   │   │   │   │   ├── AstroGateway_Simulated_Scans.json
│   │   │   │   │   └── AstroGateway_Simulated_Telemetry.json
│   │   │   │   ├── MarsRegression/
│   │   │   │   │   └── Evidence/
│   │   │   │   │       ├── Console_Counts.txt
│   │   │   │   │       ├── Editor_Frame_Samples.csv
│   │   │   │   │       ├── Provisional_Synthetic_Telemetry.json
│   │   │   │   │       ├── Resolved_Pretest_Console_Counts.txt
│   │   │   │   │       ├── Scripted_Play_Mode_Tests.txt
│   │   │   │   │       └── Walking_Provisional_Synthetic_Telemetry.json
│   │   │   │   ├── Mission01Regression/
│   │   │   │   │   ├── PlayMode_Controlled_Tests.txt
│   │   │   │   │   └── Test_Local_Save.json
│   │   │   │   ├── NativeRegression/
│   │   │   │   │   ├── Evidence/
│   │   │   │   │   │   ├── Export/
│   │   │   │   │   │   │   ├── AstroGateway_Simulated_Scans.csv
│   │   │   │   │   │   │   ├── AstroGateway_Simulated_Scans.json
│   │   │   │   │   │   │   └── AstroGateway_Simulated_Telemetry.json
│   │   │   │   │   │   └── Local_Test_Snapshot.json
│   │   │   │   │   └── Controlled_PlayMode_Tests.txt
│   │   │   │   ├── Approved_Source_Hash_Checks.json
│   │   │   │   ├── Asset_Reuse_And_Licenses.md
│   │   │   │   ├── Controlled_PlayMode_Tests.txt
│   │   │   │   ├── Editor_Asset_Metrics.txt
│   │   │   │   ├── Final_Report.md
│   │   │   │   ├── Geometry_Stats.txt
│   │   │   │   ├── Launch_Instructions.md
│   │   │   │   ├── Local_Test_Save.json
│   │   │   │   ├── Manual_Visual_QA.md
│   │   │   │   ├── New_Files.json
│   │   │   │   ├── Performance_Notes.md
│   │   │   │   ├── Preservation_Check.json
│   │   │   │   ├── Rendering_Readback.txt
│   │   │   │   ├── Test_Report.md
│   │   │   │   └── World_Layout_And_Integration.md
│   │   │   ├── MarsVisualPolish/
│   │   │   │   ├── MarsRegression/
│   │   │   │   │   └── Evidence/
│   │   │   │   │       ├── Console_Counts.txt
│   │   │   │   │       ├── Editor_Frame_Samples.csv
│   │   │   │   │       ├── Provisional_Synthetic_Telemetry.json
│   │   │   │   │       ├── Resolved_Pretest_Console_Counts.txt
│   │   │   │   │       ├── Scripted_Play_Mode_Tests.txt
│   │   │   │   │       └── Walking_Provisional_Synthetic_Telemetry.json
│   │   │   │   ├── Mission01Regression/
│   │   │   │   │   ├── PlayMode_Controlled_Tests.txt
│   │   │   │   │   └── Test_Local_Save.json
│   │   │   │   ├── NativeRegression/
│   │   │   │   │   ├── Evidence/
│   │   │   │   │   │   ├── Export/
│   │   │   │   │   │   │   ├── AstroGateway_Simulated_Scans.csv
│   │   │   │   │   │   │   ├── AstroGateway_Simulated_Scans.json
│   │   │   │   │   │   │   └── AstroGateway_Simulated_Telemetry.json
│   │   │   │   │   │   └── Local_Test_Snapshot.json
│   │   │   │   │   └── Controlled_PlayMode_Tests.txt
│   │   │   │   ├── NewSceneTests/
│   │   │   │   │   ├── Export/
│   │   │   │   │   │   ├── AstroGateway_Simulated_Scans.csv
│   │   │   │   │   │   ├── AstroGateway_Simulated_Scans.json
│   │   │   │   │   │   └── AstroGateway_Simulated_Telemetry.json
│   │   │   │   │   ├── Controlled_PlayMode_Tests.txt
│   │   │   │   │   ├── Editor_Asset_Metrics.txt
│   │   │   │   │   └── Local_Test_Save.json
│   │   │   │   ├── Asset_Provenance_Report.md
│   │   │   │   ├── Editor_Frame_Cadence.txt
│   │   │   │   ├── Execution_Results.json
│   │   │   │   ├── Final_Report.md
│   │   │   │   ├── Graphics_Presets_And_Performance.md
│   │   │   │   ├── Known_Limitations.md
│   │   │   │   ├── Lighting_Camera_And_UI.md
│   │   │   │   ├── Manual_Launch_Instructions.md
│   │   │   │   ├── New_Files.json
│   │   │   │   ├── Preservation_Check.json
│   │   │   │   ├── Test_Report.md
│   │   │   │   └── Visual_Audit_And_Comparison.md
│   │   │   ├── Mission01Milestone/
│   │   │   │   ├── MarsRegression/
│   │   │   │   │   └── Evidence/
│   │   │   │   │       ├── Console_Counts.txt
│   │   │   │   │       ├── Editor_Frame_Samples.csv
│   │   │   │   │       ├── Provisional_Synthetic_Telemetry.json
│   │   │   │   │       ├── Resolved_Pretest_Console_Counts.txt
│   │   │   │   │       ├── Scripted_Play_Mode_Tests.txt
│   │   │   │   │       └── Walking_Provisional_Synthetic_Telemetry.json
│   │   │   │   ├── Asset_Reuse_Inventory.md
│   │   │   │   ├── Challenge_And_Mission_Architecture.md
│   │   │   │   ├── Final_Report.md
│   │   │   │   ├── Known_Limitations.md
│   │   │   │   ├── Launch_And_Test_Instructions.md
│   │   │   │   ├── New_Files.json
│   │   │   │   ├── PlayMode_Controlled_Tests.txt
│   │   │   │   ├── Preservation_Check.json
│   │   │   │   ├── Project_Audit_And_Plan.md
│   │   │   │   ├── Scene_Validation.txt
│   │   │   │   └── Test_Local_Save.json
│   │   │   ├── NativeControlSlice/
│   │   │   │   ├── Evidence/
│   │   │   │   │   ├── Export/
│   │   │   │   │   │   ├── AstroGateway_Simulated_Scans.csv
│   │   │   │   │   │   ├── AstroGateway_Simulated_Scans.json
│   │   │   │   │   │   └── AstroGateway_Simulated_Telemetry.json
│   │   │   │   │   └── Local_Test_Snapshot.json
│   │   │   │   ├── MarsRegression/
│   │   │   │   │   └── Evidence/
│   │   │   │   │       ├── Console_Counts.txt
│   │   │   │   │       ├── Editor_Frame_Samples.csv
│   │   │   │   │       ├── Provisional_Synthetic_Telemetry.json
│   │   │   │   │       ├── Resolved_Pretest_Console_Counts.txt
│   │   │   │   │       ├── Scripted_Play_Mode_Tests.txt
│   │   │   │   │       └── Walking_Provisional_Synthetic_Telemetry.json
│   │   │   │   ├── Milestone01Regression/
│   │   │   │   │   ├── PlayMode_Controlled_Tests.txt
│   │   │   │   │   └── Test_Local_Save.json
│   │   │   │   ├── Android_Manual_Test_Plan.md
│   │   │   │   ├── Architecture_And_Plan.md
│   │   │   │   ├── Controlled_PlayMode_Tests.txt
│   │   │   │   ├── Final_Report.md
│   │   │   │   ├── Known_Limitations.md
│   │   │   │   ├── Launch_Instructions.md
│   │   │   │   ├── New_Files.json
│   │   │   │   ├── Preservation_Check.json
│   │   │   │   ├── Scene_Validation.txt
│   │   │   │   ├── Telemetry_And_Save_Contract.md
│   │   │   │   └── Test_Results.md
│   │   │   ├── PreMeetingDemo/
│   │   │   │   ├── Demo_Launch_Instructions.md
│   │   │   │   ├── Graphics_and_FPS_Status.md
│   │   │   │   ├── Graphics_Validation.txt
│   │   │   │   ├── Known_Issues.md
│   │   │   │   ├── LOD_Candidate_Evidence.json
│   │   │   │   ├── Meeting_Demo_Ready_Report.md
│   │   │   │   ├── Preservation_Results.json
│   │   │   │   ├── Rover_Repair_Status.md
│   │   │   │   ├── Team_Work_Assignment.md
│   │   │   │   └── Web_Test_Results.txt
│   │   │   ├── ProductionVisual/
│   │   │   │   ├── Baseline/
│   │   │   │   │   ├── HUD/
│   │   │   │   │   │   ├── HUD_Controlled_Tests.txt
│   │   │   │   │   │   ├── HUD_Test_Save.json
│   │   │   │   │   │   ├── Responsive_Layout_Checks.txt
│   │   │   │   │   │   └── Runtime_UI_Inventory.txt
│   │   │   │   │   ├── Mars/
│   │   │   │   │   │   └── Evidence/
│   │   │   │   │   │       ├── Console_Counts.txt
│   │   │   │   │   │       ├── Editor_Frame_Samples.csv
│   │   │   │   │   │       ├── Provisional_Synthetic_Telemetry.json
│   │   │   │   │   │       ├── Resolved_Pretest_Console_Counts.txt
│   │   │   │   │   │       ├── Scripted_Play_Mode_Tests.txt
│   │   │   │   │   │       └── Walking_Provisional_Synthetic_Telemetry.json
│   │   │   │   │   ├── Mission01/
│   │   │   │   │   │   ├── PlayMode_Controlled_Tests.txt
│   │   │   │   │   │   └── Test_Local_Save.json
│   │   │   │   │   ├── Native/
│   │   │   │   │   │   ├── Evidence/
│   │   │   │   │   │   │   ├── Export/
│   │   │   │   │   │   │   │   ├── AstroGateway_Simulated_Scans.csv
│   │   │   │   │   │   │   │   ├── AstroGateway_Simulated_Scans.json
│   │   │   │   │   │   │   │   └── AstroGateway_Simulated_Telemetry.json
│   │   │   │   │   │   │   └── Local_Test_Snapshot.json
│   │   │   │   │   │   └── Controlled_PlayMode_Tests.txt
│   │   │   │   │   ├── VisualPolish/
│   │   │   │   │   │   ├── Export/
│   │   │   │   │   │   │   ├── AstroGateway_Simulated_Scans.csv
│   │   │   │   │   │   │   ├── AstroGateway_Simulated_Scans.json
│   │   │   │   │   │   │   └── AstroGateway_Simulated_Telemetry.json
│   │   │   │   │   │   ├── Controlled_PlayMode_Tests.txt
│   │   │   │   │   │   ├── Editor_Asset_Metrics.txt
│   │   │   │   │   │   └── Local_Test_Save.json
│   │   │   │   │   ├── Execution.json
│   │   │   │   │   └── Native_First_Run_Tests.txt
│   │   │   │   ├── Regression/
│   │   │   │   │   ├── HUD/
│   │   │   │   │   │   ├── HUD_Controlled_Tests.txt
│   │   │   │   │   │   ├── HUD_Editor_Performance.txt
│   │   │   │   │   │   ├── HUD_Test_Save.json
│   │   │   │   │   │   ├── Responsive_Layout_Checks.txt
│   │   │   │   │   │   └── Runtime_UI_Inventory.txt
│   │   │   │   │   ├── Mars/
│   │   │   │   │   │   └── Evidence/
│   │   │   │   │   │       ├── Console_Counts.txt
│   │   │   │   │   │       ├── Editor_Frame_Samples.csv
│   │   │   │   │   │       ├── Provisional_Synthetic_Telemetry.json
│   │   │   │   │   │       ├── Resolved_Pretest_Console_Counts.txt
│   │   │   │   │   │       ├── Scripted_Play_Mode_Tests.txt
│   │   │   │   │   │       └── Walking_Provisional_Synthetic_Telemetry.json
│   │   │   │   │   ├── Mission01/
│   │   │   │   │   │   ├── PlayMode_Controlled_Tests.txt
│   │   │   │   │   │   └── Test_Local_Save.json
│   │   │   │   │   ├── Native/
│   │   │   │   │   │   ├── Evidence/
│   │   │   │   │   │   │   ├── Export/
│   │   │   │   │   │   │   │   ├── AstroGateway_Simulated_Scans.csv
│   │   │   │   │   │   │   │   ├── AstroGateway_Simulated_Scans.json
│   │   │   │   │   │   │   │   └── AstroGateway_Simulated_Telemetry.json
│   │   │   │   │   │   │   └── Local_Test_Snapshot.json
│   │   │   │   │   │   └── Controlled_PlayMode_Tests.txt
│   │   │   │   │   ├── VisualPolish/
│   │   │   │   │   │   ├── Export/
│   │   │   │   │   │   │   ├── AstroGateway_Simulated_Scans.csv
│   │   │   │   │   │   │   ├── AstroGateway_Simulated_Scans.json
│   │   │   │   │   │   │   └── AstroGateway_Simulated_Telemetry.json
│   │   │   │   │   │   ├── Controlled_PlayMode_Tests.txt
│   │   │   │   │   │   ├── Editor_Asset_Metrics.txt
│   │   │   │   │   │   └── Local_Test_Save.json
│   │   │   │   │   └── Execution.json
│   │   │   │   ├── Android_Physical_Testing_Status.md
│   │   │   │   ├── Asset_Provenance_Report.md
│   │   │   │   ├── Automated_And_Regression_Test_Report.md
│   │   │   │   ├── Camera_Control_Telemetry_Report.md
│   │   │   │   ├── Capture_Retry_With_RMB_Failure.txt
│   │   │   │   ├── Character_Upgrade_Status.md
│   │   │   │   ├── Controller_Dependency_Map.md
│   │   │   │   ├── Current_Project_Audit.md
│   │   │   │   ├── Environment_Upgrade_Report.md
│   │   │   │   ├── Final_Report.md
│   │   │   │   ├── Four_Preset_Editor_Performance.txt
│   │   │   │   ├── Graphics_Performance_Findings.md
│   │   │   │   ├── Installed_New_Files.json
│   │   │   │   ├── Known_Limitations.md
│   │   │   │   ├── Main_Project_Import_Audit.txt
│   │   │   │   ├── Manual_Launch_Instructions.md
│   │   │   │   ├── New_Mesh_Inventory.txt
│   │   │   │   ├── Original_File_Preservation_Report.md
│   │   │   │   ├── Preservation_Results.json
│   │   │   │   ├── Production_Controlled_Tests.txt
│   │   │   │   ├── Production_Test_Save.json
│   │   │   │   ├── Reference_Comparison.md
│   │   │   │   ├── Rover_Upgrade_Report.md
│   │   │   │   ├── Scene_Save_Reload.txt
│   │   │   │   ├── Test_Counts.json
│   │   │   │   └── UI_Architecture_Map.md
│   │   │   ├── Stage03/
│   │   │   │   ├── Web/
│   │   │   │   │   ├── TemplateData/
│   │   │   │   │   │   ├── favicon.ico
│   │   │   │   │   │   ├── fullscreen-button.png
│   │   │   │   │   │   ├── MemoryProfiler.png
│   │   │   │   │   │   ├── progress-bar-empty-dark.png
│   │   │   │   │   │   ├── progress-bar-empty-light.png
│   │   │   │   │   │   ├── progress-bar-full-dark.png
│   │   │   │   │   │   ├── progress-bar-full-light.png
│   │   │   │   │   │   ├── style.css
│   │   │   │   │   │   ├── unity-logo-dark.png
│   │   │   │   │   │   ├── unity-logo-light.png
│   │   │   │   │   │   ├── unity-logo-title-footer.png
│   │   │   │   │   │   └── webmemd-icon.png
│   │   │   │   │   └── index.html
│   │   │   │   ├── creation-status.txt
│   │   │   │   ├── Geometry_Validation.md
│   │   │   │   ├── Stage03_Completion_Report.md
│   │   │   │   ├── Web_Build_Status.txt
│   │   │   │   └── Zone_Coordinates.json
│   │   │   ├── Stage04/
│   │   │   │   ├── Web/
│   │   │   │   │   ├── TemplateData/
│   │   │   │   │   │   ├── favicon.ico
│   │   │   │   │   │   ├── fullscreen-button.png
│   │   │   │   │   │   ├── MemoryProfiler.png
│   │   │   │   │   │   ├── progress-bar-empty-dark.png
│   │   │   │   │   │   ├── progress-bar-empty-light.png
│   │   │   │   │   │   ├── progress-bar-full-dark.png
│   │   │   │   │   │   ├── progress-bar-full-light.png
│   │   │   │   │   │   ├── style.css
│   │   │   │   │   │   ├── unity-logo-dark.png
│   │   │   │   │   │   ├── unity-logo-light.png
│   │   │   │   │   │   ├── unity-logo-title-footer.png
│   │   │   │   │   │   └── webmemd-icon.png
│   │   │   │   │   ├── index.html
│   │   │   │   │   ├── performance.html
│   │   │   │   │   └── web_probe.js
│   │   │   │   ├── Asset_Manifest.json
│   │   │   │   ├── Asset_Statistics.md
│   │   │   │   ├── Assets_And_Attribution.md
│   │   │   │   ├── Browser_Console.json
│   │   │   │   ├── Browser_Performance.json
│   │   │   │   ├── capture-status.txt
│   │   │   │   ├── creation-status.txt
│   │   │   │   ├── FSR_Investigation.md
│   │   │   │   ├── Geometry_Validation.md
│   │   │   │   ├── Initial_Web_Build_Status.txt
│   │   │   │   ├── Preservation_Check.json
│   │   │   │   ├── Resolved_Authoring_Command_Error.txt
│   │   │   │   ├── Review.html
│   │   │   │   ├── Scene_Reopen_Status.txt
│   │   │   │   ├── Stage04_Completion_Report.md
│   │   │   │   ├── Validation_Evidence_Paths.txt
│   │   │   │   └── Web_Build_Status.txt
│   │   │   ├── Stage05A/
│   │   │   │   ├── initial-validation/
│   │   │   │   │   └── validation.json
│   │   │   │   ├── failure.xml
│   │   │   │   ├── final-overlay-validation.json
│   │   │   │   ├── preservation.json
│   │   │   │   ├── responsive-validation.json
│   │   │   │   ├── Screenshots.html
│   │   │   │   ├── Stage05A_Report.md
│   │   │   │   └── validation.json
│   │   │   ├── Stage05B/
│   │   │   │   ├── database/
│   │   │   │   │   └── 001_identity_and_mission_sessions.sql
│   │   │   │   ├── source/
│   │   │   │   │   ├── backend/
│   │   │   │   │   │   ├── tests/
│   │   │   │   │   │   │   └── test_security_contract.py
│   │   │   │   │   │   ├── .env.example
│   │   │   │   │   │   ├── database.py
│   │   │   │   │   │   ├── main.py
│   │   │   │   │   │   ├── README.md
│   │   │   │   │   │   ├── requirements-dev.txt
│   │   │   │   │   │   └── requirements.txt
│   │   │   │   │   ├── auth.local.properties.example
│   │   │   │   │   ├── AuthCoordinator.kt
│   │   │   │   │   ├── build.gradle
│   │   │   │   │   ├── MainActivity.kt
│   │   │   │   │   ├── PocUnityActivity.kt
│   │   │   │   │   ├── SecureSessionStore.kt
│   │   │   │   │   └── SecuritySmokeInstrumentation.kt
│   │   │   │   ├── apk.json
│   │   │   │   ├── backend-http-smoke.json
│   │   │   │   ├── Configuration_And_Security.md
│   │   │   │   ├── navigation-validation.json
│   │   │   │   ├── preservation.json
│   │   │   │   ├── rls-validation.json
│   │   │   │   ├── runtime-summary.json
│   │   │   │   ├── Screenshots.html
│   │   │   │   └── Stage05B_Report.md
│   │   │   ├── Stage05B1/
│   │   │   │   ├── Connection_Readiness/
│   │   │   │   │   ├── android-build-install.json
│   │   │   │   │   ├── api-connectivity.json
│   │   │   │   │   ├── ca-discovery-verification.json
│   │   │   │   │   ├── configure_test_db_admin.py
│   │   │   │   │   ├── emulator-connectivity.json
│   │   │   │   │   ├── ignore-and-dns-check.json
│   │   │   │   │   ├── local-readiness.json
│   │   │   │   │   ├── secure-database-verification.json
│   │   │   │   │   ├── secure-db-prerequisites.json
│   │   │   │   │   └── session-pooler-probe.json
│   │   │   │   ├── Database_Plan_SQL/
│   │   │   │   │   ├── 002_runtime_role.draft.sql
│   │   │   │   │   └── 003_post_migration_metadata.sql
│   │   │   │   ├── Environment_Setup/
│   │   │   │   │   ├── templates/
│   │   │   │   │   │   ├── .env.example
│   │   │   │   │   │   ├── auth.local.properties.example
│   │   │   │   │   │   ├── run_local.py
│   │   │   │   │   │   ├── setup_backend.sh
│   │   │   │   │   │   └── test-accounts.example.json
│   │   │   │   │   ├── connectivity-validation.json
│   │   │   │   │   ├── Environment_Setup_Guide.md
│   │   │   │   │   ├── helper-validation.json
│   │   │   │   │   ├── latest-config-presence.json
│   │   │   │   │   └── setup-status.json
│   │   │   │   ├── Hardening/
│   │   │   │   │   ├── Before/
│   │   │   │   │   │   ├── .env.example
│   │   │   │   │   │   ├── database.py
│   │   │   │   │   │   ├── main.py
│   │   │   │   │   │   └── run_local.py
│   │   │   │   │   ├── Evidence/
│   │   │   │   │   │   ├── final-checks.json
│   │   │   │   │   │   ├── initial-harness-error.json
│   │   │   │   │   │   ├── pg18-exploratory-sql-rehearsal.json
│   │   │   │   │   │   ├── source-after.json
│   │   │   │   │   │   ├── source-before.json
│   │   │   │   │   │   ├── sql-manifest.json
│   │   │   │   │   │   └── sql-rehearsal.json
│   │   │   │   │   ├── Source/
│   │   │   │   │   │   ├── tests/
│   │   │   │   │   │   │   ├── test_hardening.py
│   │   │   │   │   │   │   └── test_security_contract.py
│   │   │   │   │   │   ├── .env.example
│   │   │   │   │   │   ├── database.py
│   │   │   │   │   │   ├── main.py
│   │   │   │   │   │   └── run_local.py
│   │   │   │   │   ├── SQL/
│   │   │   │   │   │   ├── 000_read_only_preflight.sql
│   │   │   │   │   │   ├── 001_identity_and_mission_sessions.sql
│   │   │   │   │   │   ├── 002_public_acl_isolation.sql
│   │   │   │   │   │   ├── 003_runtime_role.sql
│   │   │   │   │   │   ├── 004_post_migration_metadata.sql
│   │   │   │   │   │   ├── 900_local_fixture_only.sql
│   │   │   │   │   │   └── 910_local_down_only.sql
│   │   │   │   │   ├── pg17-package-lock.json
│   │   │   │   │   ├── rehearse.mjs
│   │   │   │   │   └── tls_fixtures.py
│   │   │   │   ├── Review_Evidence/
│   │   │   │   │   └── catalogue.json
│   │   │   │   ├── Supabase_Configuration/
│   │   │   │   │   ├── admin.env.example
│   │   │   │   │   ├── Configuration_Status_And_Next_Steps.md
│   │   │   │   │   ├── connectivity.json
│   │   │   │   │   ├── inspection.json
│   │   │   │   │   └── Read_Only_Database_Preflight.sql
│   │   │   │   ├── build-config-presence.json
│   │   │   │   ├── configuration-inspection.json
│   │   │   │   ├── Database_Execution_Plan.md
│   │   │   │   ├── Database_Migration_Security_Review.md
│   │   │   │   ├── Final_SQL_Manifest.md
│   │   │   │   ├── Local_Rehearsal_Results.md
│   │   │   │   ├── other-project-config-presence.json
│   │   │   │   ├── Rollback_And_Recovery_Plan.md
│   │   │   │   ├── Security_Hardening_Report.md
│   │   │   │   ├── Stage05B1_Validation_Report.md
│   │   │   │   └── Supabase_Connection_Readiness_Report.md
│   │   │   ├── Stage05B2/
│   │   │   │   ├── Before/
│   │   │   │   │   ├── .env.example
│   │   │   │   │   ├── database.py
│   │   │   │   │   ├── main.py
│   │   │   │   │   ├── README.md
│   │   │   │   │   ├── requirements.txt
│   │   │   │   │   ├── run_local.py
│   │   │   │   │   ├── test_hardening.py
│   │   │   │   │   └── test_security_contract.py
│   │   │   │   ├── Evidence/
│   │   │   │   │   ├── actual-service-isolation.json
│   │   │   │   │   ├── api-gateway-integration.json
│   │   │   │   │   ├── lab-shutdown.json
│   │   │   │   │   ├── native-binary-setup.json
│   │   │   │   │   ├── native-setup.json
│   │   │   │   │   ├── postgrest-binary.json
│   │   │   │   │   ├── repeat-rehearsal.json
│   │   │   │   │   ├── revised-sql-manifest.json
│   │   │   │   │   ├── security-regression-failure.json
│   │   │   │   │   ├── security-regression.json
│   │   │   │   │   ├── source-manifest.json
│   │   │   │   │   └── stale-server.json
│   │   │   │   ├── Source/
│   │   │   │   │   ├── tests/
│   │   │   │   │   │   ├── test_hardening.py
│   │   │   │   │   │   ├── test_postgrest_repository.py
│   │   │   │   │   │   └── test_security_contract.py
│   │   │   │   │   ├── .env.example
│   │   │   │   │   ├── database.py
│   │   │   │   │   ├── main.py
│   │   │   │   │   ├── README.md
│   │   │   │   │   ├── requirements.txt
│   │   │   │   │   └── run_local.py
│   │   │   │   ├── SQL/
│   │   │   │   │   ├── 000_read_only_preflight.sql
│   │   │   │   │   ├── 001_verified_jwt_training.sql
│   │   │   │   │   ├── 002_retire_direct_runtime.sql
│   │   │   │   │   ├── 003_read_only_verification.sql
│   │   │   │   │   ├── 900_local_fixture_only.sql
│   │   │   │   │   └── 910_local_down_only.sql
│   │   │   │   ├── api_integration.py
│   │   │   │   ├── Identity_Authorization_Architecture.md
│   │   │   │   ├── Impersonation_Fix_Report.md
│   │   │   │   ├── lab_common.py
│   │   │   │   ├── prepare_lab.py
│   │   │   │   ├── regression.py
│   │   │   │   ├── repeat_rehearsal.py
│   │   │   │   ├── Revised_Final_SQL_Manifest.md
│   │   │   │   ├── run_gateway.py
│   │   │   │   ├── Security_Regression_Results.md
│   │   │   │   └── start_lab.py
│   │   │   ├── Stage05B3/
│   │   │   │   ├── Evidence/
│   │   │   │   │   ├── artifact-manifest.json
│   │   │   │   │   ├── final-checks.json
│   │   │   │   │   ├── isolated-api-integration.json
│   │   │   │   │   ├── isolated-service-health.json
│   │   │   │   │   ├── isolation-probe.json
│   │   │   │   │   ├── local-tls.json
│   │   │   │   │   ├── repeat-rehearsal.json
│   │   │   │   │   ├── security-regression-failure.json
│   │   │   │   │   ├── security-regression.json
│   │   │   │   │   ├── shutdown.json
│   │   │   │   │   └── staged-source.json
│   │   │   │   ├── Isolation/
│   │   │   │   │   ├── fixture-verification-public.pem
│   │   │   │   │   ├── Hosted_Read_Only_Inventory.sql
│   │   │   │   │   ├── integration_probe.py
│   │   │   │   │   ├── launch.py
│   │   │   │   │   ├── prepare_isolation.py
│   │   │   │   │   ├── prepare_runtime_tests.py
│   │   │   │   │   ├── probe.py
│   │   │   │   │   ├── runtime.sb
│   │   │   │   │   ├── serve.py
│   │   │   │   │   ├── service_health.py
│   │   │   │   │   ├── test_isolated_api.py
│   │   │   │   │   └── unit_probe.py
│   │   │   │   ├── Stage05B3_Hosted_Compatibility_Plan.md
│   │   │   │   ├── Stage05B3_Remote_Execution_Readiness.md
│   │   │   │   ├── Stage05B3_Security_Test_Results.md
│   │   │   │   └── Stage05B3_Service_Isolation_Report.md
│   │   │   ├── Stage07A/
│   │   │   │   ├── Evidence/
│   │   │   │   │   ├── ai-api.json
│   │   │   │   │   ├── ai-native.json
│   │   │   │   │   ├── ai-rollback.json
│   │   │   │   │   ├── ai-trusted.json
│   │   │   │   │   ├── final-checks.json
│   │   │   │   │   ├── final-native-regression.json
│   │   │   │   │   ├── isolation.json
│   │   │   │   │   ├── sample-validation.json
│   │   │   │   │   ├── shutdown.json
│   │   │   │   │   └── source-manifest.json
│   │   │   │   ├── Samples/
│   │   │   │   │   ├── hirise_metadata_sample.lbl
│   │   │   │   │   └── provenance.json
│   │   │   │   ├── TestHarness/
│   │   │   │   │   ├── ai_checks.py
│   │   │   │   │   ├── baseline_final_checks.py
│   │   │   │   │   ├── native_checks.py
│   │   │   │   │   ├── probe.py
│   │   │   │   │   ├── rollback_checks.py
│   │   │   │   │   ├── run_checks.py
│   │   │   │   │   ├── runtime.sb
│   │   │   │   │   └── trusted_checks.py
│   │   │   │   ├── Adaptive_AI_Architecture.md
│   │   │   │   ├── advanced_example_plan.json
│   │   │   │   ├── beginner_example_plan.json
│   │   │   │   ├── intermediate_example_plan.json
│   │   │   │   ├── Local_Test_Results.md
│   │   │   │   ├── Mars_Weather_Incident_Simulator.md
│   │   │   │   ├── Model_Training_Evaluation_Plan.md
│   │   │   │   ├── Open_Data_Source_Catalog.md
│   │   │   │   ├── Player_Telemetry_Schema.md
│   │   │   │   ├── Skill_Assessment_Design.md
│   │   │   │   ├── Telemetry_JSON_Schema.json
│   │   │   │   └── Thirty_Day_Training_Plan.md
│   │   │   ├── Stage07B/
│   │   │   │   ├── Datasets/
│   │   │   │   │   ├── synthetic_manifest.json
│   │   │   │   │   ├── synthetic_scenarios.json
│   │   │   │   │   └── synthetic_training.jsonl
│   │   │   │   ├── Evidence/
│   │   │   │   │   ├── aria_evaluation.json
│   │   │   │   │   ├── artifact_checksums.json
│   │   │   │   │   ├── assessor_source_provenance.json
│   │   │   │   │   ├── baseline_evaluation.json
│   │   │   │   │   ├── baseline_model.json
│   │   │   │   │   ├── hirise_ingestion.json
│   │   │   │   │   ├── isolation-probes.json
│   │   │   │   │   ├── pre-replay-hashes.json
│   │   │   │   │   ├── reproducibility.json
│   │   │   │   │   └── training_gate.json
│   │   │   │   ├── ARIA_Grounding_Evaluation.md
│   │   │   │   ├── Baseline_Model_Evaluation.md
│   │   │   │   ├── Dataset_Ingestion_Report.md
│   │   │   │   ├── Evaluation_Protocol.md
│   │   │   │   ├── Mission_Witness_Trust_Design.md
│   │   │   │   ├── NASA_ESA_Dataset_Catalog.md
│   │   │   │   ├── Player_Consent_Privacy_Design.md
│   │   │   │   ├── README.md
│   │   │   │   ├── Stage07B_Local_Test_Results.md
│   │   │   │   └── Training_Dataset_Schema.md
│   │   │   ├── Stage08A/
│   │   │   │   ├── Evidence/
│   │   │   │   │   ├── Asset_Inventory.json
│   │   │   │   │   ├── Camera_Capture_Timestamp.txt
│   │   │   │   │   ├── Editor_Diagnostics.txt
│   │   │   │   │   ├── preservation_baseline.json
│   │   │   │   │   └── Preservation_Check.json
│   │   │   │   ├── Current_Assets_Inventory.md
│   │   │   │   ├── Play_Mode_Test_Results.md
│   │   │   │   ├── Screenshot_Index.md
│   │   │   │   └── Unity_Visual_Preview_Report.md
│   │   │   ├── Stage08B/
│   │   │   │   ├── Evidence/
│   │   │   │   │   ├── Final_Isolated_Run/
│   │   │   │   │   │   ├── Console_Counts.txt
│   │   │   │   │   │   ├── creation.txt
│   │   │   │   │   │   ├── Editor_Frame_Samples.csv
│   │   │   │   │   │   ├── Final_Lab_Provenance.json
│   │   │   │   │   │   ├── Isolated_Lab_Provenance.json
│   │   │   │   │   │   ├── Preservation_Check.json
│   │   │   │   │   │   ├── Provisional_Synthetic_Telemetry.json
│   │   │   │   │   │   ├── Resolved_Pretest_Console_Counts.txt
│   │   │   │   │   │   ├── Scripted_Play_Mode_Tests.txt
│   │   │   │   │   │   ├── Telemetry_Compatibility.txt
│   │   │   │   │   │   └── Walking_Provisional_Synthetic_Telemetry.json
│   │   │   │   │   ├── Trial01/
│   │   │   │   │   │   ├── Console_Counts.txt
│   │   │   │   │   │   ├── Editor_Frame_Samples.csv
│   │   │   │   │   │   └── Scripted_Play_Mode_Tests.txt
│   │   │   │   │   ├── Trial02/
│   │   │   │   │   │   ├── Console_Counts.txt
│   │   │   │   │   │   ├── Editor_Frame_Samples.csv
│   │   │   │   │   │   └── Scripted_Play_Mode_Tests.txt
│   │   │   │   │   ├── Console_Counts.txt
│   │   │   │   │   ├── creation.txt
│   │   │   │   │   ├── Editor_Frame_Samples.csv
│   │   │   │   │   ├── Final_Lab_Provenance.json
│   │   │   │   │   ├── Isolated_Lab_Provenance.json
│   │   │   │   │   ├── MainEditor_Telemetry_Compatibility.txt
│   │   │   │   │   ├── Preservation_Check.json
│   │   │   │   │   ├── Provisional_Synthetic_Telemetry.json
│   │   │   │   │   ├── Resolved_Pretest_Console_Counts.txt
│   │   │   │   │   ├── Scripted_Play_Mode_Tests.txt
│   │   │   │   │   ├── Telemetry_Compatibility.txt
│   │   │   │   │   └── Walking_Provisional_Synthetic_Telemetry.json
│   │   │   │   ├── Android_Performance_Report.md
│   │   │   │   ├── Character_Controller_Report.md
│   │   │   │   ├── Rover_Driving_Report.md
│   │   │   │   ├── Stage08B_Playable_Prototype_Report.md
│   │   │   │   └── Weather_Visuals_Report.md
│   │   │   ├── Stage08C/
│   │   │   │   ├── Evidence/
│   │   │   │   │   ├── Timing_Trial/
│   │   │   │   │   │   ├── Console_Counts.txt
│   │   │   │   │   │   ├── Editor_Frame_Samples.csv
│   │   │   │   │   │   └── Scripted_Play_Mode_Tests.txt
│   │   │   │   │   ├── Android_Collection_Status.json
│   │   │   │   │   ├── Android_Device.json
│   │   │   │   │   ├── Android_Memory_0.txt
│   │   │   │   │   ├── Android_Memory_1.txt
│   │   │   │   │   ├── Android_Memory_2.txt
│   │   │   │   │   ├── Android_Memory_3.txt
│   │   │   │   │   ├── Android_Performance_Summary.json
│   │   │   │   │   ├── Android_Profile_0.json
│   │   │   │   │   ├── Android_Profile_1.json
│   │   │   │   │   ├── Android_Profile_2.json
│   │   │   │   │   ├── Android_Profile_3.json
│   │   │   │   │   ├── Console_Counts.txt
│   │   │   │   │   ├── Editor_Frame_Samples.csv
│   │   │   │   │   ├── Preservation_Check.json
│   │   │   │   │   ├── Provisional_Synthetic_Telemetry.json
│   │   │   │   │   ├── Resolved_Pretest_Console_Counts.txt
│   │   │   │   │   ├── Runtime_Source_Provenance.json
│   │   │   │   │   ├── Scripted_Play_Mode_Tests.txt
│   │   │   │   │   ├── Telemetry_Compatibility.txt
│   │   │   │   │   ├── Touch_Unit_Checks.txt
│   │   │   │   │   ├── Video_Container_Check.json
│   │   │   │   │   └── Walking_Provisional_Synthetic_Telemetry.json
│   │   │   │   ├── Android_Touch_Control_Report.md
│   │   │   │   ├── Manual_Gameplay_Verification.md
│   │   │   │   ├── Mobile_Performance_Report.md
│   │   │   │   ├── Regression_Test_Results.md
│   │   │   │   ├── Stage08C_Readiness_Report.md
│   │   │   │   └── Visual_QA_Report.md
│   │   │   ├── Stage08C1/
│   │   │   │   ├── Evidence/
│   │   │   │   │   ├── Host_Attempt/
│   │   │   │   │   │   └── Collection_Status.json
│   │   │   │   │   ├── Android_Collection_Status.json
│   │   │   │   │   ├── Android_Device.json
│   │   │   │   │   ├── Android_GfxInfo.txt
│   │   │   │   │   ├── Android_Memory_0.txt
│   │   │   │   │   ├── Android_Memory_1.txt
│   │   │   │   │   ├── Android_Memory_2.txt
│   │   │   │   │   ├── Android_Memory_3.txt
│   │   │   │   │   ├── Android_Profile_0.json
│   │   │   │   │   ├── Android_Profile_1.json
│   │   │   │   │   ├── Android_Profile_2.json
│   │   │   │   │   ├── Android_Profile_3.json
│   │   │   │   │   ├── AndroidProbeBuild.cs
│   │   │   │   │   ├── AndroidProfileProbe.cs
│   │   │   │   │   ├── Baseline_Summary.json
│   │   │   │   │   ├── Console_Counts.txt
│   │   │   │   │   ├── CPU_Acceleration.txt
│   │   │   │   │   ├── Editor_Concurrent_Lab_Trial.json
│   │   │   │   │   ├── Editor_Frame_Samples.csv
│   │   │   │   │   ├── Editor_Performance.json
│   │   │   │   │   ├── Emulator_Last_ANR.txt
│   │   │   │   │   ├── Emulator_System_Memory.txt
│   │   │   │   │   ├── Emulator_Thermal.txt
│   │   │   │   │   ├── Manual_Input_First_Session.jsonl
│   │   │   │   │   ├── Manual_Input_Trace.jsonl
│   │   │   │   │   ├── Manual_Trace_Summary.json
│   │   │   │   │   ├── Preservation_Check.json
│   │   │   │   │   ├── Provisional_Synthetic_Telemetry.json
│   │   │   │   │   ├── Resolved_Pretest_Console_Counts.txt
│   │   │   │   │   ├── Scene_Inspection.json
│   │   │   │   │   ├── Scripted_Play_Mode_Tests.txt
│   │   │   │   │   ├── Source_Provenance.json
│   │   │   │   │   ├── Telemetry_Compatibility.txt
│   │   │   │   │   ├── Texture_Inventory.json
│   │   │   │   │   ├── Touch_Unit_Checks.txt
│   │   │   │   │   ├── Video_Inspection.txt
│   │   │   │   │   ├── Video_Inspection_Retry.txt
│   │   │   │   │   └── Walking_Provisional_Synthetic_Telemetry.json
│   │   │   │   ├── Resolution_Reversion/
│   │   │   │   │   └── Evidence/
│   │   │   │   │       ├── Android_Collection_Status.json
│   │   │   │   │       ├── Android_Device.json
│   │   │   │   │       ├── Android_Memory_0.txt
│   │   │   │   │       └── Android_Profile_0.json
│   │   │   │   ├── Resolution_Trial/
│   │   │   │   │   └── Evidence/
│   │   │   │   │       ├── Android_Collection_Status.json
│   │   │   │   │       ├── Android_Device.json
│   │   │   │   │       ├── Android_Memory_0.txt
│   │   │   │   │       ├── Android_Memory_1.txt
│   │   │   │   │       ├── Android_Memory_2.txt
│   │   │   │   │       ├── Android_Memory_3.txt
│   │   │   │   │       ├── Android_Profile_0.json
│   │   │   │   │       ├── Android_Profile_1.json
│   │   │   │   │       ├── Android_Profile_2.json
│   │   │   │   │       └── Android_Profile_3.json
│   │   │   │   ├── Android_Renderer_Diagnosis.md
│   │   │   │   ├── Device_Readiness_Matrix.md
│   │   │   │   ├── Gameplay_Input_Verification.md
│   │   │   │   ├── Performance_Optimization_Results.md
│   │   │   │   ├── Stage08C1_Final_Report.md
│   │   │   │   └── Unity_Profiler_Baseline.md
│   │   │   ├── Stage08D/
│   │   │   │   ├── Acquired_Asset_Inspection.json
│   │   │   │   ├── Android_Optimization_Report.md
│   │   │   │   ├── Asset_Acquisition_Evaluation.md
│   │   │   │   ├── Astronaut_Upgrade_Report.md
│   │   │   │   ├── Before_After_Comparison.md
│   │   │   │   ├── Inspected_Source_Hashes.json
│   │   │   │   ├── Licensed_Asset_Shortlist.md
│   │   │   │   ├── Mars_Environment_Upgrade_Report.md
│   │   │   │   ├── No_Login_Astronaut_Candidates.md
│   │   │   │   ├── Perseverance_File_Inspection.json
│   │   │   │   ├── Preservation_Check_2026-10-11.json
│   │   │   │   ├── Rover_Upgrade_Report.md
│   │   │   │   ├── Stage08D_Continuation_Report.md
│   │   │   │   ├── Staging_Optimization_Results.md
│   │   │   │   ├── URP_Staging_Integration_Plan.md
│   │   │   │   └── Visual_Quality_Audit.md
│   │   │   ├── Stage08D1/
│   │   │   │   ├── Acquired_Source_Preservation.json
│   │   │   │   ├── Android_Performance_Status.md
│   │   │   │   ├── GLB_Compatibility_Inspection.json
│   │   │   │   ├── Import_Validation_Report.md
│   │   │   │   ├── Importer_Reports.json
│   │   │   │   ├── LOD_Quality_Comparison.md
│   │   │   │   ├── Preservation_Check.json
│   │   │   │   ├── Rover_Visual_Review.md
│   │   │   │   ├── Stage08D1_Final_Report.md
│   │   │   │   └── URP_Scene_Validation.md
│   │   │   ├── Stage08D11/
│   │   │   │   ├── Conversion_Evidence.json
│   │   │   │   ├── GLB_Conversion_Report.md
│   │   │   │   ├── GLTF_Validation_Results.md
│   │   │   │   ├── Image_Difference_Metrics.json
│   │   │   │   ├── Rover_Material_Comparison.md
│   │   │   │   ├── Scene_Checks.txt
│   │   │   │   ├── Source_Preservation_Checks.json
│   │   │   │   ├── Stage08D11_Final_Report.md
│   │   │   │   ├── Unity_Asset_Checks.txt
│   │   │   │   ├── Unity_Import_Results.md
│   │   │   │   ├── Unity_Importer_Diagnostics.json
│   │   │   │   └── WebP_Texture_Inventory.md
│   │   │   ├── Stage08D2/
│   │   │   │   ├── LOD_Artifact_Repair_Report.md
│   │   │   │   ├── LOD_Repair_Evidence.json
│   │   │   │   ├── LOD_Repair_FirstAttempt.json
│   │   │   │   ├── Material_Adapter_Evidence.json
│   │   │   │   ├── Material_Fidelity_Audit.md
│   │   │   │   ├── Preserved_Stage08D1_Scene_SHA256.txt
│   │   │   │   ├── Scene_Validation_Report.md
│   │   │   │   ├── Source_Preservation_Checks.json
│   │   │   │   ├── Stage08D2_Final_Report.md
│   │   │   │   ├── Unity_Regression_Checks.txt
│   │   │   │   └── URP_Material_Conversion_Report.md
│   │   │   ├── Stage09A/
│   │   │   │   ├── Gameplay_Regression_Report.md
│   │   │   │   ├── Graphics_Quality_System.md
│   │   │   │   ├── Mars_Environment_Integration.md
│   │   │   │   ├── Performance_Benchmark.md
│   │   │   │   ├── Preservation_Audit.json
│   │   │   │   ├── Project_Graphics_Audit.md
│   │   │   │   ├── Rover_Fidelity_Status.md
│   │   │   │   └── Stage09A_Final_Report.md
│   │   │   └── UnifiedAuth/
│   │   │       ├── Evidence/
│   │   │       │   ├── android-anr-summary.txt
│   │   │       │   ├── android-build.txt
│   │   │       │   ├── android-profile-crash.txt
│   │   │       │   ├── android-profile-ui-test-retry.txt
│   │   │       │   ├── android-profile-ui-test.txt
│   │   │       │   ├── android-runner-diagnostics.txt
│   │   │       │   ├── android-test-build.txt
│   │   │       │   ├── backend-tests.txt
│   │   │       │   ├── final-validation-summary.json
│   │   │       │   ├── isolated-api-integration.json
│   │   │       │   ├── local-connectivity.json
│   │   │       │   ├── local-db-tests.txt
│   │   │       │   ├── local-gateway-pid.txt
│   │   │       │   ├── local-profile-rehearsal.json
│   │   │       │   ├── modified-source-manifest.json
│   │   │       │   ├── pre-change-hashes.json
│   │   │       │   ├── preservation-and-config.json
│   │   │       │   ├── shared-project-config-check.json
│   │   │       │   ├── web-build.txt
│   │   │       │   ├── web-lint.txt
│   │   │       │   └── web-tests.txt
│   │   │       ├── SQL/
│   │   │       │   ├── 004_profile_display_name.sql
│   │   │       │   └── 904_profile_display_name_rollback.sql
│   │   │       ├── Android_Profile_Edit_Report.md
│   │   │       ├── Architecture_Report.md
│   │   │       ├── Cross_Platform_Test_Report.md
│   │   │       ├── Final_Report.md
│   │   │       ├── Security_Test_Report.md
│   │   │       ├── Shared_Identity_Contract.md
│   │   │       └── Web_Implementation_Report.md
│   │   ├── codex-work-source/
│   │   │   └── work/
│   │   │       ├── android-poc/
│   │   │       │   ├── official/
│   │   │       │   │   ├── AndroidManifest.xml
│   │   │       │   │   ├── build.gradle
│   │   │       │   │   ├── MainUnityActivity.java
│   │   │       │   │   └── SharedClass.java
│   │   │       │   ├── create_source.py
│   │   │       │   ├── device_smoke.py
│   │   │       │   ├── modules-dry-run.json
│   │   │       │   ├── official-tree.json
│   │   │       │   └── preservation-baseline.json
│   │   │       ├── android-validation/
│   │   │       │   └── preservation-baseline.json
│   │   │       ├── architecture/
│   │   │       │   ├── project-baseline.json
│   │   │       │   └── write_proposal.py
│   │   │       ├── cinematichud/
│   │   │       │   └── Before_Hashes.json
│   │   │       ├── marsenvironment/
│   │   │       │   └── Before_Hashes.json
│   │   │       ├── mission01/
│   │   │       │   └── Before_Hashes.json
│   │   │       ├── nativecontrol/
│   │   │       │   └── Before_Hashes.json
│   │   │       ├── premeeting/
│   │   │       │   ├── LOD_Candidates/
│   │   │       │   │   ├── Perseverance_RepairedV2_LOD0.glb
│   │   │       │   │   ├── Perseverance_RepairedV2_LOD1.glb
│   │   │       │   │   └── Perseverance_RepairedV2_LOD2.glb
│   │   │       │   ├── MeetingGraphics.restoration.cs
│   │   │       │   └── preservation.json
│   │   │       ├── productionvisual/
│   │   │       │   └── Before_Hashes.json
│   │   │       ├── stage03/
│   │   │       │   ├── check_docs.py
│   │   │       │   ├── design-hashes.json
│   │   │       │   ├── final_report.py
│   │   │       │   └── serve_web.py
│   │   │       ├── stage04/
│   │   │       │   ├── asset_ledger.py
│   │   │       │   ├── make_review.py
│   │   │       │   ├── preserved-hashes.json
│   │   │       │   ├── refine.py
│   │   │       │   ├── save_capture.py
│   │   │       │   └── serve_web.py
│   │   │       ├── stage05a/
│   │   │       │   ├── final_overlay_test.py
│   │   │       │   ├── responsive.py
│   │   │       │   └── validate.py
│   │   │       ├── stage05b/
│   │   │       │   ├── pg-lab/
│   │   │       │   │   ├── package-lock.json
│   │   │       │   │   ├── package.json
│   │   │       │   │   └── rls-test.mjs
│   │   │       │   ├── current.xml
│   │   │       │   ├── preservation.json
│   │   │       │   ├── sdk-3.2.6-versions.toml
│   │   │       │   ├── sdk-3.4.1-versions.toml
│   │   │       │   ├── sdk-3.8.0-versions.toml
│   │   │       │   ├── update_main.py
│   │   │       │   └── validate_ui.py
│   │   │       ├── stage05b1-hardening/
│   │   │       │   ├── pg17/
│   │   │       │   │   └── package.json
│   │   │       │   ├── tls/
│   │   │       │   │   ├── ca.cnf
│   │   │       │   │   ├── ca.crt
│   │   │       │   │   ├── ca.srl
│   │   │       │   │   ├── leaf.cnf
│   │   │       │   │   ├── leaf.crt
│   │   │       │   │   └── leaf.csr
│   │   │       │   ├── final_checks.py
│   │   │       │   ├── prepare.py
│   │   │       │   ├── rehearse.mjs
│   │   │       │   ├── sql_prepare.py
│   │   │       │   └── write_reports.py
│   │   │       ├── stage05b1-review/
│   │   │       │   └── catalogue.py
│   │   │       ├── stage05b1-setup/
│   │   │       │   └── run_local.py
│   │   │       ├── stage05b2/
│   │   │       │   ├── postgrest-release.json
│   │   │       │   ├── preflight.mjs
│   │   │       │   ├── prepare_backend.py
│   │   │       │   └── prepare_sql.py
│   │   │       ├── stage05b3/
│   │   │       │   ├── scratch/
│   │   │       │   │   └── fixture-signature.bin
│   │   │       │   ├── debug.sb
│   │   │       │   ├── debug2.sb
│   │   │       │   ├── enhance_probes.py
│   │   │       │   ├── regression.py
│   │   │       │   ├── repeat_rehearsal.py
│   │   │       │   └── write_reports.py
│   │   │       ├── stage07a/
│   │   │       │   ├── scratch/
│   │   │       │   │   └── sign.bin
│   │   │       │   └── write_docs.py
│   │   │       ├── stage07b/
│   │   │       │   └── finish.py
│   │   │       ├── stage08a/
│   │   │       │   ├── inspect.py
│   │   │       │   └── report.py
│   │   │       ├── stage08b/
│   │   │       │   ├── PlayModeLab/
│   │   │       │   │   └── Assets/
│   │   │       │   │       └── AstroGateway/
│   │   │       │   │           ├── CinematicHUD/
│   │   │       │   │           │   └── Tests/
│   │   │       │   │           │       └── Editor/
│   │   │       │   │           │           ├── CinematicHUDBuilder.cs
│   │   │       │   │           │           └── HUDResponsiveQA.cs
│   │   │       │   │           ├── MarsEnvironment/
│   │   │       │   │           │   ├── Documentation/
│   │   │       │   │           │   │   └── README.md.meta
│   │   │       │   │           │   ├── Models/
│   │   │       │   │           │   │   ├── Crew_Dome_Ribs.asset
│   │   │       │   │           │   │   ├── Crew_Dome_Ribs.asset.meta
│   │   │       │   │           │   │   ├── Laboratory_Dome_Ribs.asset
│   │   │       │   │           │   │   ├── Laboratory_Dome_Ribs.asset.meta
│   │   │       │   │           │   │   ├── Life_Support_Dome_Ribs.asset
│   │   │       │   │           │   │   └── Life_Support_Dome_Ribs.asset.meta
│   │   │       │   │           │   ├── Prefabs/
│   │   │       │   │           │   │   ├── ARESYN_ClosedHabitatComplex.prefab
│   │   │       │   │           │   │   ├── ARESYN_ClosedHabitatComplex.prefab.meta
│   │   │       │   │           │   │   ├── SolarFarm.prefab
│   │   │       │   │           │   │   └── SolarFarm.prefab.meta
│   │   │       │   │           │   └── Tests/
│   │   │       │   │           │       └── Editor/
│   │   │       │   │           │           └── MarsResearchBaseBuilder.cs
│   │   │       │   │           ├── MarsVisualPolish/
│   │   │       │   │           │   └── Scenes/
│   │   │       │   │           │       └── Mars_VisualPolish_v2.unity
│   │   │       │   │           ├── Mission01Milestone/
│   │   │       │   │           │   ├── Editor/
│   │   │       │   │           │   │   └── Mission01Builder.cs
│   │   │       │   │           │   ├── Mission01_EarthLaunch_Milestone.unity
│   │   │       │   │           │   ├── Mission01_EarthLaunch_Milestone.unity.meta
│   │   │       │   │           │   ├── Mission01_EarthLaunch_Milestone_v2.unity
│   │   │       │   │           │   ├── Mission01_EarthLaunch_Milestone_v2.unity.meta
│   │   │       │   │           │   ├── Mission01_EarthLaunch_Milestone_v3.unity
│   │   │       │   │           │   └── Mission01_EarthLaunch_Milestone_v3.unity.meta
│   │   │       │   │           ├── NativeControlSlice/
│   │   │       │   │           │   └── Editor/
│   │   │       │   │           │       └── NativeSliceBuilder.cs
│   │   │       │   │           └── Stage08B/
│   │   │       │   │               └── Editor/
│   │   │       │   │                   └── Stage08BBuilder.cs
│   │   │       │   └── baseline.json
│   │   │       ├── stage08c/
│   │   │       │   ├── baseline.json
│   │   │       │   ├── collect_android.py
│   │   │       │   └── MarsPrototype.before.cs
│   │   │       ├── stage08c1/
│   │   │       │   ├── baseline.json
│   │   │       │   ├── collect_android.py
│   │   │       │   ├── collect_resolution.py
│   │   │       │   ├── collect_reversion.py
│   │   │       │   ├── current_manual_state.json
│   │   │       │   ├── Stage08BBuilder.before.cs
│   │   │       │   ├── video_inspect.swift
│   │   │       │   └── write_reports.py
│   │   │       ├── stage08d/
│   │   │       │   ├── Acquired/
│   │   │       │   │   ├── rock_07/
│   │   │       │   │   │   ├── textures/
│   │   │       │   │   │   │   ├── rock_07_arm_2k.jpg
│   │   │       │   │   │   │   ├── rock_07_diff_2k.jpg
│   │   │       │   │   │   │   └── rock_07_nor_gl_2k.jpg
│   │   │       │   │   │   ├── rock_07.bin
│   │   │       │   │   │   ├── rock_07.fbx
│   │   │       │   │   │   └── rock_07.gltf
│   │   │       │   │   ├── rock_09/
│   │   │       │   │   │   ├── textures/
│   │   │       │   │   │   │   ├── rock_09_arm_2k.jpg
│   │   │       │   │   │   │   ├── rock_09_diff_2k.jpg
│   │   │       │   │   │   │   └── rock_09_nor_gl_2k.jpg
│   │   │       │   │   │   ├── rock_09.bin
│   │   │       │   │   │   ├── rock_09.fbx
│   │   │       │   │   │   └── rock_09.gltf
│   │   │       │   │   ├── Acquisition_Manifest.json
│   │   │       │   │   ├── CC0_Legalcode.html
│   │   │       │   │   ├── NASA_Embedded_0.jpg
│   │   │       │   │   ├── NASA_Embedded_1.jpg
│   │   │       │   │   ├── NASA_Embedded_2.jpg
│   │   │       │   │   ├── NASA_README.md
│   │   │       │   │   ├── Perseverance.glb
│   │   │       │   │   ├── PolyHaven_License.html
│   │   │       │   │   ├── rock_07_files.json
│   │   │       │   │   ├── rock_09_files.json
│   │   │       │   │   └── sandy_gravel_files.json
│   │   │       │   ├── Optimized/
│   │   │       │   │   ├── Optimization_Metrics.json
│   │   │       │   │   └── Validation_Results.json
│   │   │       │   ├── tools/
│   │   │       │   │   ├── convert_webp.mjs
│   │   │       │   │   ├── image_diff.mjs
│   │   │       │   │   ├── material_adapter.mjs
│   │   │       │   │   ├── meeting_lod_candidates.mjs
│   │   │       │   │   ├── optimize.mjs
│   │   │       │   │   ├── originals.mjs
│   │   │       │   │   ├── package-lock.json
│   │   │       │   │   ├── package.json
│   │   │       │   │   ├── repair_lod.mjs
│   │   │       │   │   └── validate.mjs
│   │   │       │   ├── acquire.py
│   │   │       │   ├── grab3d_catalog.json
│   │   │       │   ├── inspect.py
│   │   │       │   └── write_followup.py
│   │   │       ├── stage08d1/
│   │   │       │   ├── VisualEvaluation/
│   │   │       │   │   ├── Assets/
│   │   │       │   │   │   ├── ApprovedAssets/
│   │   │       │   │   │   │   ├── Ground/
│   │   │       │   │   │   │   │   ├── Generated_Smoothness.png
│   │   │       │   │   │   │   │   ├── Generated_Smoothness.png.meta
│   │   │       │   │   │   │   │   ├── sandy_gravel_AO.png.meta
│   │   │       │   │   │   │   │   ├── sandy_gravel_Diffuse.png.meta
│   │   │       │   │   │   │   │   ├── sandy_gravel_nor_gl.png.meta
│   │   │       │   │   │   │   │   └── sandy_gravel_Rough.png.meta
│   │   │       │   │   │   │   ├── Ground.meta
│   │   │       │   │   │   │   ├── Perseverance_LOD0.glb
│   │   │       │   │   │   │   ├── Perseverance_LOD0.glb.meta
│   │   │       │   │   │   │   ├── Perseverance_LOD0_PNG.glb.meta
│   │   │       │   │   │   │   ├── Perseverance_LOD1.glb
│   │   │       │   │   │   │   ├── Perseverance_LOD1.glb.meta
│   │   │       │   │   │   │   ├── Perseverance_LOD1_PNG.glb
│   │   │       │   │   │   │   ├── Perseverance_LOD1_PNG.glb.meta
│   │   │       │   │   │   │   ├── Perseverance_LOD2.glb
│   │   │       │   │   │   │   ├── Perseverance_LOD2.glb.meta
│   │   │       │   │   │   │   ├── Perseverance_LOD2_PNG.glb
│   │   │       │   │   │   │   ├── Perseverance_LOD2_PNG.glb.meta
│   │   │       │   │   │   │   ├── Perseverance_Original.glb
│   │   │       │   │   │   │   ├── Perseverance_Original.glb.meta
│   │   │       │   │   │   │   ├── Perseverance_Original_PNG.glb
│   │   │       │   │   │   │   ├── Perseverance_Original_PNG.glb.meta
│   │   │       │   │   │   │   ├── rock_07_LOD0.glb
│   │   │       │   │   │   │   ├── rock_07_LOD0.glb.meta
│   │   │       │   │   │   │   ├── rock_07_LOD1.glb
│   │   │       │   │   │   │   ├── rock_07_LOD1.glb.meta
│   │   │       │   │   │   │   ├── rock_07_LOD2.glb
│   │   │       │   │   │   │   ├── rock_07_LOD2.glb.meta
│   │   │       │   │   │   │   ├── rock_07_Original.glb
│   │   │       │   │   │   │   ├── rock_07_Original.glb.meta
│   │   │       │   │   │   │   ├── rock_09_LOD0.glb
│   │   │       │   │   │   │   ├── rock_09_LOD0.glb.meta
│   │   │       │   │   │   │   ├── rock_09_LOD1.glb
│   │   │       │   │   │   │   ├── rock_09_LOD1.glb.meta
│   │   │       │   │   │   │   ├── rock_09_LOD2.glb
│   │   │       │   │   │   │   ├── rock_09_LOD2.glb.meta
│   │   │       │   │   │   │   ├── rock_09_Original.glb
│   │   │       │   │   │   │   └── rock_09_Original.glb.meta
│   │   │       │   │   │   ├── Editor/
│   │   │       │   │   │   │   ├── MarsRockExport.cs
│   │   │       │   │   │   │   ├── MarsRockExport.cs.meta
│   │   │       │   │   │   │   ├── MeetingBuild.cs
│   │   │       │   │   │   │   ├── MeetingBuild.cs.meta
│   │   │       │   │   │   │   ├── Stage08D11Probe.cs
│   │   │       │   │   │   │   ├── Stage08D11Probe.cs.meta
│   │   │       │   │   │   │   ├── Stage08D1Builder.cs
│   │   │       │   │   │   │   ├── Stage08D1Builder.cs.meta
│   │   │       │   │   │   │   ├── Stage08D2Repair.cs
│   │   │       │   │   │   │   └── Stage08D2Repair.cs.meta
│   │   │       │   │   │   ├── Evaluation/
│   │   │       │   │   │   │   ├── Ground.mat
│   │   │       │   │   │   │   ├── Ground.mat.meta
│   │   │       │   │   │   │   ├── MarsVolume.asset.meta
│   │   │       │   │   │   │   ├── Stage08D1_Visual_Evaluation.unity
│   │   │       │   │   │   │   └── Stage08D1_Visual_Evaluation.unity.meta
│   │   │       │   │   │   ├── PreMeeting/
│   │   │       │   │   │   │   ├── Materials/
│   │   │       │   │   │   │   │   ├── Dust.mat
│   │   │       │   │   │   │   │   ├── Dust.mat.meta
│   │   │       │   │   │   │   │   ├── Dust_v2.mat
│   │   │       │   │   │   │   │   ├── Dust_v2.mat.meta
│   │   │       │   │   │   │   │   ├── glass_2_Transparent.mat
│   │   │       │   │   │   │   │   ├── glass_2_Transparent.mat.meta
│   │   │       │   │   │   │   │   ├── glass_lens_Transparent.mat
│   │   │       │   │   │   │   │   ├── glass_lens_Transparent.mat.meta
│   │   │       │   │   │   │   │   ├── glass_trans_Transparent.mat
│   │   │       │   │   │   │   │   └── glass_trans_Transparent.mat.meta
│   │   │       │   │   │   │   ├── Materials.meta
│   │   │       │   │   │   │   ├── PreMeeting_Mars_Visual_Preview.unity
│   │   │       │   │   │   │   ├── PreMeeting_Mars_Visual_Preview.unity.meta
│   │   │       │   │   │   │   ├── PreMeeting_Mars_Visual_Preview_v2.unity
│   │   │       │   │   │   │   └── PreMeeting_Mars_Visual_Preview_v2.unity.meta
│   │   │       │   │   │   ├── Stage08D2/
│   │   │       │   │   │   │   ├── Materials/
│   │   │       │   │   │   │   │   ├── blades.mat
│   │   │       │   │   │   │   │   ├── blades.mat.meta
│   │   │       │   │   │   │   │   ├── glass_2.mat
│   │   │       │   │   │   │   │   ├── glass_2.mat.meta
│   │   │       │   │   │   │   │   ├── glass_lens.mat
│   │   │       │   │   │   │   │   ├── glass_lens.mat.meta
│   │   │       │   │   │   │   │   ├── glass_trans.mat
│   │   │       │   │   │   │   │   ├── glass_trans.mat.meta
│   │   │       │   │   │   │   │   ├── mars_2020_06.mat
│   │   │       │   │   │   │   │   ├── mars_2020_06.mat.meta
│   │   │       │   │   │   │   │   ├── mars_2020_06_n.mat
│   │   │       │   │   │   │   │   ├── mars_2020_06_n.mat.meta
│   │   │       │   │   │   │   │   ├── transparent.mat
│   │   │       │   │   │   │   │   └── transparent.mat.meta
│   │   │       │   │   │   │   ├── Models/
│   │   │       │   │   │   │   │   ├── Perseverance_RepairedV2_LOD0.glb.meta
│   │   │       │   │   │   │   │   ├── Perseverance_RepairedV2_LOD1.glb
│   │   │       │   │   │   │   │   ├── Perseverance_RepairedV2_LOD1.glb.meta
│   │   │       │   │   │   │   │   ├── Perseverance_RepairedV2_LOD2.glb
│   │   │       │   │   │   │   │   └── Perseverance_RepairedV2_LOD2.glb.meta
│   │   │       │   │   │   │   ├── Scenes/
│   │   │       │   │   │   │   │   ├── Stage08D2_Rover_Fidelity_Evaluation.unity
│   │   │       │   │   │   │   │   └── Stage08D2_Rover_Fidelity_Evaluation.unity.meta
│   │   │       │   │   │   │   ├── Textures/
│   │   │       │   │   │   │   │   ├── blades_Base.png
│   │   │       │   │   │   │   │   ├── blades_Base.png.meta
│   │   │       │   │   │   │   │   ├── blades_SpecSmooth.png
│   │   │       │   │   │   │   │   ├── blades_SpecSmooth.png.meta
│   │   │       │   │   │   │   │   ├── glass_2_Base.png
│   │   │       │   │   │   │   │   ├── glass_2_Base.png.meta
│   │   │       │   │   │   │   │   ├── glass_2_SpecSmooth.png
│   │   │       │   │   │   │   │   ├── glass_2_SpecSmooth.png.meta
│   │   │       │   │   │   │   │   ├── glass_lens_Base.png
│   │   │       │   │   │   │   │   ├── glass_lens_Base.png.meta
│   │   │       │   │   │   │   │   ├── glass_lens_SpecSmooth.png
│   │   │       │   │   │   │   │   ├── glass_lens_SpecSmooth.png.meta
│   │   │       │   │   │   │   │   ├── glass_trans_Base.png.meta
│   │   │       │   │   │   │   │   ├── glass_trans_SpecSmooth.png
│   │   │       │   │   │   │   │   ├── glass_trans_SpecSmooth.png.meta
│   │   │       │   │   │   │   │   ├── mars_2020_06_Base.png
│   │   │       │   │   │   │   │   ├── mars_2020_06_Base.png.meta
│   │   │       │   │   │   │   │   ├── mars_2020_06_n_Base.png
│   │   │       │   │   │   │   │   ├── mars_2020_06_n_Base.png.meta
│   │   │       │   │   │   │   │   ├── mars_2020_06_n_Normal.png
│   │   │       │   │   │   │   │   ├── mars_2020_06_n_Normal.png.meta
│   │   │       │   │   │   │   │   ├── mars_2020_06_n_SpecSmooth.png
│   │   │       │   │   │   │   │   ├── mars_2020_06_n_SpecSmooth.png.meta
│   │   │       │   │   │   │   │   ├── mars_2020_06_SpecSmooth.png
│   │   │       │   │   │   │   │   ├── mars_2020_06_SpecSmooth.png.meta
│   │   │       │   │   │   │   │   ├── transparent_Base.png
│   │   │       │   │   │   │   │   ├── transparent_Base.png.meta
│   │   │       │   │   │   │   │   ├── transparent_SpecSmooth.png
│   │   │       │   │   │   │   │   └── transparent_SpecSmooth.png.meta
│   │   │       │   │   │   │   ├── Material_Adapter_Manifest.json.meta
│   │   │       │   │   │   │   ├── Materials.meta
│   │   │       │   │   │   │   ├── Models.meta
│   │   │       │   │   │   │   ├── Scenes.meta
│   │   │       │   │   │   │   └── Textures.meta
│   │   │       │   │   │   ├── ApprovedAssets.meta
│   │   │       │   │   │   ├── Editor.meta
│   │   │       │   │   │   ├── Evaluation.meta
│   │   │       │   │   │   ├── MarsEnvironmentRockExport.meta
│   │   │       │   │   │   ├── MeetingGraphics.cs
│   │   │       │   │   │   ├── MeetingGraphics.cs.meta
│   │   │       │   │   │   ├── PreMeeting.meta
│   │   │       │   │   │   ├── Settings.meta
│   │   │       │   │   │   └── Stage08D2.meta
│   │   │       │   │   ├── Packages/
│   │   │       │   │   │   ├── manifest.json
│   │   │       │   │   │   └── packages-lock.json
│   │   │       │   │   └── ProjectSettings/
│   │   │       │   │       ├── GraphicsSettings.asset
│   │   │       │   │       └── QualitySettings.asset
│   │   │       │   ├── inspect_glb.py
│   │   │       │   └── write_reports.py
│   │   │       ├── stage08d11/
│   │   │       │   └── reports.py
│   │   │       ├── stage08d2/
│   │   │       │   └── Derived/
│   │   │       │       ├── Perseverance_Repaired_LOD1.glb
│   │   │       │       └── Perseverance_Repaired_LOD2.glb
│   │   │       ├── unifiedauth/
│   │   │       │   ├── backups/
│   │   │       │   │   ├── 1_main.tsx
│   │   │       │   │   ├── 2_vite.config.ts
│   │   │       │   │   └── 3_package.json
│   │   │       │   ├── runtime/
│   │   │       │   │   ├── database.py
│   │   │       │   │   ├── integration_probe.py
│   │   │       │   │   ├── main.py
│   │   │       │   │   └── serve_unified.py
│   │   │       │   ├── scratch/
│   │   │       │   │   ├── api-integration-failure.json
│   │   │       │   │   └── fixture-signature.bin
│   │   │       │   ├── tls/
│   │   │       │   │   ├── ca.cnf
│   │   │       │   │   ├── ca.crt
│   │   │       │   │   ├── ca.srl
│   │   │       │   │   ├── leaf.crt
│   │   │       │   │   └── leaf.csr
│   │   │       │   ├── .gitignore
│   │   │       │   ├── isolated_regression.py
│   │   │       │   ├── rehearse_profile.py
│   │   │       │   ├── renew_lab_tls.py
│   │   │       │   ├── runtime.sb
│   │   │       │   ├── service.sb
│   │   │       │   └── start_api.py
│   │   │       ├── visualpolish/
│   │   │       │   └── Before_Hashes.json
│   │   │       └── create_design.py
│   │   ├── recovered-archives/
│   │   │   ├── Controls_PreChange_20261020/
│   │   │   │   └── firstmission_untracked/
│   │   │   │       ├── Assets/
│   │   │   │       │   └── AstroGateway/
│   │   │   │       │       └── FirstMission/
│   │   │   │       │           ├── Editor/
│   │   │   │       │           │   ├── FirstMissionAndroidBuild.cs
│   │   │   │       │           │   ├── FirstMissionBuilder.cs
│   │   │   │       │           │   └── FirstMissionPlaytest.cs
│   │   │   │       │           ├── Scenes/
│   │   │   │       │           │   ├── Mars_FirstMission_v1.unity
│   │   │   │       │           │   └── Mars_FirstMission_v1.unity.meta
│   │   │   │       │           └── Scripts/
│   │   │   │       │               ├── FirstMissionController.cs
│   │   │   │       │               ├── FirstMissionHUD.cs
│   │   │   │       │               └── RoverSensorSuite.cs
│   │   │   │       └── Docs/
│   │   │   │           └── FirstMission/
│   │   │   │               ├── Playtest/
│   │   │   │               │   ├── TestStore/
│   │   │   │               │   │   ├── mock/
│   │   │   │               │   │   │   └── sensor_records_v1.json
│   │   │   │               │   │   └── sensor_records_v1.json
│   │   │   │               │   ├── FirstMission_Playtest_Results.txt
│   │   │   │               │   └── Touch_Target_Audit.txt
│   │   │   │               └── Build.txt
│   │   │   ├── Explorer_PreChange_20261021/
│   │   │   │   └── firstmission_untracked/
│   │   │   │       ├── Assets/
│   │   │   │       │   └── AstroGateway/
│   │   │   │       │       └── FirstMission/
│   │   │   │       │           ├── Editor/
│   │   │   │       │           │   ├── FirstMissionBuilder.cs
│   │   │   │       │           │   └── FirstMissionPlaytest.cs
│   │   │   │       │           └── Scripts/
│   │   │   │       │               └── HUD/
│   │   │   │       │                   ├── HudIcons.cs
│   │   │   │       │                   ├── HudWidgets.cs
│   │   │   │       │                   └── PremiumHUD.cs
│   │   │   │       └── Docs/
│   │   │   │           └── FirstMission/
│   │   │   │               └── Playtest/
│   │   │   │                   ├── TestStore/
│   │   │   │                   │   ├── mock/
│   │   │   │                   │   │   └── sensor_records_v1.json
│   │   │   │                   │   └── sensor_records_v1.json
│   │   │   │                   └── FirstMission_Playtest_Results.txt
│   │   │   ├── FirstMission_PreChange_20261020/
│   │   │   │   └── backend_snapshot/
│   │   │   │       └── backend/
│   │   │   │           ├── tests/
│   │   │   │           │   └── local_mock_server.py
│   │   │   │           ├── database.py
│   │   │   │           └── main.py
│   │   │   ├── HUD_PreChange_20261021/
│   │   │   │   └── firstmission_untracked/
│   │   │   │       ├── Assets/
│   │   │   │       │   └── AstroGateway/
│   │   │   │       │       └── FirstMission/
│   │   │   │       │           ├── Editor/
│   │   │   │       │           │   ├── FirstMissionBuilder.cs
│   │   │   │       │           │   └── FirstMissionPlaytest.cs
│   │   │   │       │           ├── Scenes/
│   │   │   │       │           │   ├── Mars_FirstMission_v1.unity
│   │   │   │       │           │   └── Mars_FirstMission_v1.unity.meta
│   │   │   │       │           └── Scripts/
│   │   │   │       │               ├── FirstMissionControlLayer.cs
│   │   │   │       │               └── FirstMissionHUD.cs
│   │   │   │       └── Docs/
│   │   │   │           └── FirstMission/
│   │   │   │               ├── Playtest/
│   │   │   │               │   ├── TestStore/
│   │   │   │               │   │   ├── mock/
│   │   │   │               │   │   │   └── sensor_records_v1.json
│   │   │   │               │   │   └── sensor_records_v1.json
│   │   │   │               │   └── FirstMission_Playtest_Results.txt
│   │   │   │               └── Build.txt
│   │   │   ├── Missions_PreChange_20261022/
│   │   │   │   └── firstmission_untracked/
│   │   │   │       └── Assets/
│   │   │   │           └── AstroGateway/
│   │   │   │               └── FirstMission/
│   │   │   │                   ├── Editor/
│   │   │   │                   │   ├── FirstMissionBuilder.cs
│   │   │   │                   │   └── FirstMissionPlaytest.cs
│   │   │   │                   └── Scripts/
│   │   │   │                       ├── HUD/
│   │   │   │                       │   ├── ExplorerCinematics.cs
│   │   │   │                       │   ├── ExplorerHUD.cs
│   │   │   │                       │   └── HudIcons.cs
│   │   │   │                       └── Nav/
│   │   │   │                           └── FirstMissionNavigation.cs
│   │   │   ├── Stage2_PreChange_20261016_2225/
│   │   │   │   └── project_source/
│   │   │   │       └── Assets/
│   │   │   │           └── AstroGateway/
│   │   │   │               ├── Integration/
│   │   │   │               │   ├── Contracts/
│   │   │   │               │   │   └── IntegrationContracts.cs
│   │   │   │               │   └── Local/
│   │   │   │               │       ├── GameplayEventBridge.cs
│   │   │   │               │       └── LocalServices.cs
│   │   │   │               └── Verification/
│   │   │   │                   └── Editor/
│   │   │   │                       └── Step1GameplayVerification.cs
│   │   │   └── VisualUpgrade_PreChange_20261018_1005/
│   │   │       └── project_assets_settings_docs/
│   │   │           └── Docs/
│   │   │               └── PROGRESS_REPORT.md
│   │   └── unique-project-backups/
│   │       ├── ARES_VI_Showroom_PreChange_20261020/
│   │       │   ├── HEAD.txt
│   │       │   ├── settings.sha256
│   │       │   └── status.txt
│   │       ├── Controls_PreChange_20261020/
│   │       │   └── SHA256SUMS.txt
│   │       ├── Explorer_PreChange_20261021/
│   │       │   └── SHA256SUMS.txt
│   │       ├── FirstMission_PreChange_20261020/
│   │       │   └── SHA256SUMS.txt
│   │       ├── HUD_PreChange_20261021/
│   │       │   └── SHA256SUMS.txt
│   │       └── Unity_Recovery_20261019/
│   │           ├── _Recovery/
│   │           │   └── 0.unity.meta
│   │           └── _Recovery.meta
│   ├── migration/
│   │   ├── file-manifest.csv
│   │   └── staging-verification.json
│   ├── reference-material/
│   │   └── Downloads/
│   │       ├── Astrogateway Astronaut Asset Design Sheet-1.png
│   │       ├── AstroGateway_3_Professional_PDF_Style_Samples.pdf
│   │       ├── AstroGateway_3D_Artist_Complete_Production_Brief.pdf
│   │       ├── AstroGateway_3D_Asset_Design_Bible_Team_Handover_v3.pdf
│   │       ├── AstroGateway_3D_Characters_Objects_Design_Bible.pdf
│   │       ├── AstroGateway_AI_ML_Engineer_Complete_Production_Brief.pdf
│   │       ├── AstroGateway_Desktop_3D_Visual_Design_Bible.pdf
│   │       ├── AstroGateway_Desktop_Professional_3D_Design_Bible_V5 (1).pdf
│   │       ├── AstroGateway_Desktop_Professional_3D_Design_Bible_V5.pdf
│   │       ├── AstroGateway_Individual_3D_Outlook_Reference_Desktop.pdf
│   │       ├── AstroGateway_Mission_Red_Horizon_Visual_Master_Report.pdf
│   │       ├── AstroGateway_Mobile_3D_Visual_Design_Bible.pdf
│   │       ├── AstroGateway_Professional_3D_Asset_Team_Bible_V4.pdf
│   │       ├── AstroGateway_Realistic_2D_Reference_Master_Report.pdf
│   │       ├── AstroGateway_Style02_Technical_Asset_Bible_Desktop.pdf
│   │       └── AstroGateway_Team_Development_Master_Plan.pdf
│   ├── ARCHITECTURE.md
│   ├── DEVELOPER_RESPONSIBILITIES.md
│   ├── DEVELOPMENT_WORKFLOW.md
│   ├── REPOSITORY_TREE.md
│   ├── SETUP_GUIDE.md
│   └── TEAM_PLAN.md
├── .gitattributes
├── .gitignore
├── .gitleaks.toml
├── MIGRATION.md
└── README.md
```
