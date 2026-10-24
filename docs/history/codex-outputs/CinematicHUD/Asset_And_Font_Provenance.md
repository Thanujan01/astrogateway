# UI assets and provenance

No external downloads, account access, paid assets or new dependencies.

Original generated assets: rounded panel, circular control and heading-arrow PNG sprites; contour schematic derived from the existing local terrain mesh (approximately 2m sampling). The schematic is not NASA satellite imagery or a scientific elevation dataset. Marker positions come from actual scene transforms. North-up minimap background follows the player and zoom; out-of-range minimap icons are clamped as edge indicators. Expanded map is a paused full-world snapshot with global coordinates.

Font: existing Unity built-in LegacyRuntime.ttf loaded through Unity's resource API; no external font file was downloaded/copied or separately relicensed. Simple original text symbols and heading geometry are used, not a third-party icon pack. Four 128×128 RGBA sprites correspond to roughly 256KiB uncompressed pixel data before platform format/engine overhead; this is not measured device memory. Sprites are not atlased. Existing approved world assets and their provenance remain unchanged.

QA consulted Unity's reference source to understand active Game View size selection/callbacks; no Unity source implementation was copied into runtime assets: https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/GameView/GameView.cs and GameViewSizes.cs. Reflection-based editor helpers are version-pinned test tooling.
