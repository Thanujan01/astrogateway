# LOD quality comparison

**BLOCKED: no identical-camera Unity comparison captures generated.** The evaluation builder stopped before camera creation. No visual fidelity result is invented.

| Asset | Original | LOD0 | LOD1 | LOD2 |
|---|---:|---:|---:|---:|
| Rock07 triangles | 14,844 | 5,936 | 2,226 | 742 |
| Rock09 triangles | 12,416 | 4,966 | 1,862 | 620 |
| Rover triangles | 199,521 | 91,916 | 47,280 | 27,088 |

Imported files are staged, but **LODGroups are not configured in a saved scene**. Builder proposes projected-height thresholds .5/.2/.035 and restores automatic LOD after forced-LOD0 captures. Those thresholds require visual tuning for small stones and rover silhouette; not approved as measured quality settings. Texture derivatives cap edges at 1024/512/256; source references retain originals.

After resolving WebP, compare source and LOD0 at fixed position, FOV42 and 1600×1000 resolution, identical sunlight. Check rock silhouette and normal-detail retention, rover fine instruments, wire/antenna continuity, alpha, moving pivots and resting wheel transforms. Capture each lower LOD as well before accepting distant simplification. No performance benefit inferred from triangle reduction alone.
