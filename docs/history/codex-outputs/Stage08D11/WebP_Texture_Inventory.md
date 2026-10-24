# WebP texture inventory — 11 October 2026

Inspected acquired original GLB, predecoded source-reference GLB and three optimized GLBs. Acquired/source-reference assets contain 22 WebP images each; optimized variants contain 21 each. No new acquired-original derivative was saved: the acquired original was inventoried only. The already decoded reference copy was converted separately for Unity comparison.

All WebP image, texture, material-path, bufferView, byte offset, dimensions and conversion records are in Conversion_Evidence.json. The table below covers every WebP in every inspected input. Texture indices are zero-based; image references promoted from EXT_texture_webp.source to core texture.source. Existing material indices and texture slot definitions unchanged.

| Input | Image | Source bufferView | Offset / bytes | Texture indices | PNG dimensions | Material texture references |
|---|---:|---:|---|---|---|---|
| Perseverance_LOD0 | 0 | 0 | 3448060 / 37976 | 0 | 1024×1024 | materials[0].pbrMetallicRoughness.baseColorTexture.index=0 |
| Perseverance_LOD0 | 2 | 2 | 3585752 / 24574 | 2 | 1024×1024 | materials[4].normalTexture.index=2 |
| Perseverance_LOD0 | 4 | 4 | 3684992 / 58014 | 1 | 1024×1024 | materials[4].pbrMetallicRoughness.baseColorTexture.index=1; materials[18].pbrMetallicRoughness.baseColorTexture.index=1 |
| Perseverance_LOD0 | 6 | 6 | 3878648 / 108 | 3 | 64×64 | materials[5].pbrMetallicRoughness.baseColorTexture.index=3 |
| Perseverance_LOD0 | 8 | 8 | 3879024 / 38504 | 4 | 1024×1024 | materials[6].pbrMetallicRoughness.baseColorTexture.index=4; materials[14].pbrMetallicRoughness.baseColorTexture.index=4 |
| Perseverance_LOD0 | 10 | 10 | 4032560 / 134 | 5 | 128×128 | materials[8].pbrMetallicRoughness.baseColorTexture.index=5; materials[8].extensions.KHR_materials_specular.specularColorTexture.index=5; materials[40].pbrMetallicRoughness.baseColorTexture.index=5; materials[40].extensions.KHR_materials_specular.specularColorTexture.index=5 |
| Perseverance_LOD0 | 12 | 12 | 4033392 / 54912 | 7 | 1024×1024 | materials[9].normalTexture.index=7; materials[41].normalTexture.index=7 |
| Perseverance_LOD0 | 14 | 14 | 4206776 / 94394 | 6 | 1024×1024 | materials[9].pbrMetallicRoughness.baseColorTexture.index=6; materials[28].pbrMetallicRoughness.baseColorTexture.index=6; materials[41].pbrMetallicRoughness.baseColorTexture.index=6 |
| Perseverance_LOD0 | 16 | 16 | 4470976 / 99928 | 8 | 1024×1024 | materials[13].pbrMetallicRoughness.baseColorTexture.index=8; materials[23].pbrMetallicRoughness.baseColorTexture.index=8; materials[25].pbrMetallicRoughness.baseColorTexture.index=8; materials[32].pbrMetallicRoughness.baseColorTexture.index=8; materials[33].pbrMetallicRoughness.baseColorTexture.index=8; materials[37].pbrMetallicRoughness.baseColorTexture.index=8 |
| Perseverance_LOD0 | 18 | 18 | 4773264 / 57532 | 9 | 1024×1024 | materials[14].normalTexture.index=9 |
| Perseverance_LOD0 | 20 | 20 | 4963992 / 54392 | 10 | 1024×512 | materials[17].pbrMetallicRoughness.baseColorTexture.index=10 |
| Perseverance_LOD0 | 22 | 22 | 5142872 / 46294 | 11 | 1024×1024 | materials[19].pbrMetallicRoughness.baseColorTexture.index=11; materials[20].pbrMetallicRoughness.baseColorTexture.index=11 |
| Perseverance_LOD0 | 24 | 24 | 5308104 / 1696 | 12 | 256×256 | materials[21].normalTexture.index=12 |
| Perseverance_LOD0 | 26 | 26 | 5314896 / 4126 | 14 | 256×256 | materials[22].normalTexture.index=14 |
| Perseverance_LOD0 | 28 | 28 | 5329456 / 7314 | 13 | 256×256 | materials[22].pbrMetallicRoughness.baseColorTexture.index=13 |
| Perseverance_LOD0 | 30 | 30 | 5350904 / 23800 | 15 | 1024×1024 | materials[23].normalTexture.index=15; materials[32].normalTexture.index=15 |
| Perseverance_LOD0 | 32 | 32 | 6408016 / 11102 | 16 | 512×512 | materials[24].normalTexture.index=16; materials[29].normalTexture.index=16; materials[38].normalTexture.index=16 |
| Perseverance_LOD0 | 34 | 34 | 6456272 / 18050 | 17 | 512×512 | materials[27].pbrMetallicRoughness.baseColorTexture.index=17 |
| Perseverance_LOD0 | 36 | 36 | 6511664 / 17760 | 18 | 512×512 | materials[31].pbrMetallicRoughness.baseColorTexture.index=18 |
| Perseverance_LOD0 | 38 | 38 | 6567488 / 59584 | 19 | 1024×1024 | materials[37].pbrMetallicRoughness.metallicRoughnessTexture.index=19 |
| Perseverance_LOD0 | 40 | 40 | 7014408 / 6098 | 20 | 512×512 | materials[42].pbrMetallicRoughness.baseColorTexture.index=20 |
| Perseverance_LOD1 | 0 | 0 | 2190708 / 18334 | 0 | 512×512 | materials[0].pbrMetallicRoughness.baseColorTexture.index=0 |
| Perseverance_LOD1 | 2 | 2 | 2245408 / 9834 | 2 | 512×512 | materials[4].normalTexture.index=2 |
| Perseverance_LOD1 | 4 | 4 | 2279952 / 25202 | 1 | 512×512 | materials[4].pbrMetallicRoughness.baseColorTexture.index=1; materials[18].pbrMetallicRoughness.baseColorTexture.index=1 |
| Perseverance_LOD1 | 6 | 6 | 2349488 / 108 | 3 | 64×64 | materials[5].pbrMetallicRoughness.baseColorTexture.index=3 |
| Perseverance_LOD1 | 8 | 8 | 2349864 / 16668 | 4 | 512×512 | materials[6].pbrMetallicRoughness.baseColorTexture.index=4; materials[14].pbrMetallicRoughness.baseColorTexture.index=4 |
| Perseverance_LOD1 | 10 | 10 | 2402264 / 134 | 5 | 128×128 | materials[8].pbrMetallicRoughness.baseColorTexture.index=5; materials[8].extensions.KHR_materials_specular.specularColorTexture.index=5; materials[40].pbrMetallicRoughness.baseColorTexture.index=5; materials[40].extensions.KHR_materials_specular.specularColorTexture.index=5 |
| Perseverance_LOD1 | 12 | 12 | 2403096 / 14722 | 7 | 512×512 | materials[9].normalTexture.index=7; materials[41].normalTexture.index=7 |
| Perseverance_LOD1 | 14 | 14 | 2448856 / 27014 | 6 | 512×512 | materials[9].pbrMetallicRoughness.baseColorTexture.index=6; materials[28].pbrMetallicRoughness.baseColorTexture.index=6; materials[41].pbrMetallicRoughness.baseColorTexture.index=6 |
| Perseverance_LOD1 | 16 | 16 | 2519048 / 42276 | 8 | 512×512 | materials[13].pbrMetallicRoughness.baseColorTexture.index=8; materials[23].pbrMetallicRoughness.baseColorTexture.index=8; materials[25].pbrMetallicRoughness.baseColorTexture.index=8; materials[32].pbrMetallicRoughness.baseColorTexture.index=8; materials[33].pbrMetallicRoughness.baseColorTexture.index=8; materials[37].pbrMetallicRoughness.baseColorTexture.index=8 |
| Perseverance_LOD1 | 18 | 18 | 2627208 / 21290 | 9 | 512×512 | materials[14].normalTexture.index=9 |
| Perseverance_LOD1 | 20 | 20 | 2692248 / 24060 | 10 | 512×256 | materials[17].pbrMetallicRoughness.baseColorTexture.index=10 |
| Perseverance_LOD1 | 22 | 22 | 2756896 / 22030 | 11 | 512×512 | materials[19].pbrMetallicRoughness.baseColorTexture.index=11; materials[20].pbrMetallicRoughness.baseColorTexture.index=11 |
| Perseverance_LOD1 | 24 | 24 | 2819656 / 1696 | 12 | 256×256 | materials[21].normalTexture.index=12 |
| Perseverance_LOD1 | 26 | 26 | 2826448 / 4126 | 14 | 256×256 | materials[22].normalTexture.index=14 |
| Perseverance_LOD1 | 28 | 28 | 2841008 / 7314 | 13 | 256×256 | materials[22].pbrMetallicRoughness.baseColorTexture.index=13 |
| Perseverance_LOD1 | 30 | 30 | 2862456 / 10962 | 15 | 512×512 | materials[23].normalTexture.index=15; materials[32].normalTexture.index=15 |
| Perseverance_LOD1 | 32 | 32 | 3189552 / 11102 | 16 | 512×512 | materials[24].normalTexture.index=16; materials[29].normalTexture.index=16; materials[38].normalTexture.index=16 |
| Perseverance_LOD1 | 34 | 34 | 3237808 / 18050 | 17 | 512×512 | materials[27].pbrMetallicRoughness.baseColorTexture.index=17 |
| Perseverance_LOD1 | 36 | 36 | 3293200 / 17760 | 18 | 512×512 | materials[31].pbrMetallicRoughness.baseColorTexture.index=18 |
| Perseverance_LOD1 | 38 | 38 | 3349024 / 26146 | 19 | 512×512 | materials[37].pbrMetallicRoughness.metallicRoughnessTexture.index=19 |
| Perseverance_LOD1 | 40 | 40 | 3487952 / 6098 | 20 | 512×512 | materials[42].pbrMetallicRoughness.baseColorTexture.index=20 |
| Perseverance_LOD2 | 0 | 0 | 1594644 / 7542 | 0 | 256×256 | materials[0].pbrMetallicRoughness.baseColorTexture.index=0 |
| Perseverance_LOD2 | 2 | 2 | 1614896 / 2990 | 2 | 256×256 | materials[4].normalTexture.index=2 |
| Perseverance_LOD2 | 4 | 4 | 1625528 / 9240 | 1 | 256×256 | materials[4].pbrMetallicRoughness.baseColorTexture.index=1; materials[18].pbrMetallicRoughness.baseColorTexture.index=1 |
| Perseverance_LOD2 | 6 | 6 | 1649352 / 108 | 3 | 64×64 | materials[5].pbrMetallicRoughness.baseColorTexture.index=3 |
| Perseverance_LOD2 | 8 | 8 | 1649728 / 6176 | 4 | 256×256 | materials[6].pbrMetallicRoughness.baseColorTexture.index=4; materials[14].pbrMetallicRoughness.baseColorTexture.index=4 |
| Perseverance_LOD2 | 10 | 10 | 1668256 / 134 | 5 | 128×128 | materials[8].pbrMetallicRoughness.baseColorTexture.index=5; materials[8].extensions.KHR_materials_specular.specularColorTexture.index=5; materials[40].pbrMetallicRoughness.baseColorTexture.index=5; materials[40].extensions.KHR_materials_specular.specularColorTexture.index=5 |
| Perseverance_LOD2 | 12 | 12 | 1669088 / 2498 | 7 | 256×256 | materials[9].normalTexture.index=7; materials[41].normalTexture.index=7 |
| Perseverance_LOD2 | 14 | 14 | 1678944 / 6408 | 6 | 256×256 | materials[9].pbrMetallicRoughness.baseColorTexture.index=6; materials[28].pbrMetallicRoughness.baseColorTexture.index=6; materials[41].pbrMetallicRoughness.baseColorTexture.index=6 |
| Perseverance_LOD2 | 16 | 16 | 1696560 / 15302 | 8 | 256×256 | materials[13].pbrMetallicRoughness.baseColorTexture.index=8; materials[23].pbrMetallicRoughness.baseColorTexture.index=8; materials[25].pbrMetallicRoughness.baseColorTexture.index=8; materials[32].pbrMetallicRoughness.baseColorTexture.index=8; materials[33].pbrMetallicRoughness.baseColorTexture.index=8; materials[37].pbrMetallicRoughness.baseColorTexture.index=8 |
| Perseverance_LOD2 | 18 | 18 | 1733512 / 6182 | 9 | 256×256 | materials[14].normalTexture.index=9 |
| Perseverance_LOD2 | 20 | 20 | 1753384 / 9020 | 10 | 256×128 | materials[17].pbrMetallicRoughness.baseColorTexture.index=10 |
| Perseverance_LOD2 | 22 | 22 | 1776032 / 8446 | 11 | 256×256 | materials[19].pbrMetallicRoughness.baseColorTexture.index=11; materials[20].pbrMetallicRoughness.baseColorTexture.index=11 |
| Perseverance_LOD2 | 24 | 24 | 1798112 / 1696 | 12 | 256×256 | materials[21].normalTexture.index=12 |
| Perseverance_LOD2 | 26 | 26 | 1804904 / 4126 | 14 | 256×256 | materials[22].normalTexture.index=14 |
| Perseverance_LOD2 | 28 | 28 | 1819464 / 7314 | 13 | 256×256 | materials[22].pbrMetallicRoughness.baseColorTexture.index=13 |
| Perseverance_LOD2 | 30 | 30 | 1840912 / 3920 | 15 | 256×256 | materials[23].normalTexture.index=15; materials[32].normalTexture.index=15 |
| Perseverance_LOD2 | 32 | 32 | 1935592 / 5146 | 16 | 256×256 | materials[24].normalTexture.index=16; materials[29].normalTexture.index=16; materials[38].normalTexture.index=16 |
| Perseverance_LOD2 | 34 | 34 | 1953136 / 6878 | 17 | 256×256 | materials[27].pbrMetallicRoughness.baseColorTexture.index=17 |
| Perseverance_LOD2 | 36 | 36 | 1970784 / 6920 | 18 | 256×256 | materials[31].pbrMetallicRoughness.baseColorTexture.index=18 |
| Perseverance_LOD2 | 38 | 38 | 1990344 / 9804 | 19 | 256×256 | materials[37].pbrMetallicRoughness.metallicRoughnessTexture.index=19 |
| Perseverance_LOD2 | 40 | 40 | 2034256 / 2884 | 20 | 256×256 | materials[42].pbrMetallicRoughness.baseColorTexture.index=20 |
| Perseverance_Original | 0 | 0 | 6140772 / 35318 | 0 | 1024×1024 | materials[0].pbrMetallicRoughness.baseColorTexture.index=0; materials[7].pbrMetallicRoughness.baseColorTexture.index=0 |
| Perseverance_Original | 2 | 2 | 6298704 / 25070 | 2 | 1024×1024 | materials[4].normalTexture.index=2 |
| Perseverance_Original | 4 | 4 | 6383128 / 55612 | 1 | 1024×1024 | materials[4].pbrMetallicRoughness.baseColorTexture.index=1; materials[20].pbrMetallicRoughness.baseColorTexture.index=1 |
| Perseverance_Original | 6 | 6 | 6606752 / 102 | 3 | 64×64 | materials[5].pbrMetallicRoughness.baseColorTexture.index=3 |
| Perseverance_Original | 8 | 8 | 6607112 / 37192 | 4 | 1024×1024 | materials[8].pbrMetallicRoughness.baseColorTexture.index=4; materials[16].pbrMetallicRoughness.baseColorTexture.index=4 |
| Perseverance_Original | 10 | 10 | 6803088 / 126 | 5 | 128×128 | materials[10].pbrMetallicRoughness.baseColorTexture.index=5; materials[10].extensions.KHR_materials_specular.specularColorTexture.index=5; materials[44].pbrMetallicRoughness.baseColorTexture.index=5; materials[44].extensions.KHR_materials_specular.specularColorTexture.index=5 |
| Perseverance_Original | 12 | 12 | 6803608 / 55390 | 7 | 1024×1024 | materials[11].normalTexture.index=7; materials[45].normalTexture.index=7 |
| Perseverance_Original | 14 | 14 | 6956272 / 87542 | 6 | 1024×1024 | materials[11].pbrMetallicRoughness.baseColorTexture.index=6; materials[32].pbrMetallicRoughness.baseColorTexture.index=6; materials[45].pbrMetallicRoughness.baseColorTexture.index=6 |
| Perseverance_Original | 16 | 16 | 7253224 / 94974 | 8 | 1024×1024 | materials[15].pbrMetallicRoughness.baseColorTexture.index=8; materials[27].pbrMetallicRoughness.baseColorTexture.index=8; materials[29].pbrMetallicRoughness.baseColorTexture.index=8; materials[36].pbrMetallicRoughness.baseColorTexture.index=8; materials[37].pbrMetallicRoughness.baseColorTexture.index=8; materials[41].pbrMetallicRoughness.baseColorTexture.index=8 |
| Perseverance_Original | 18 | 18 | 7601584 / 56952 | 9 | 1024×1024 | materials[16].normalTexture.index=9 |
| Perseverance_Original | 20 | 20 | 7758816 / 52328 | 10 | 1024×512 | materials[19].pbrMetallicRoughness.baseColorTexture.index=10 |
| Perseverance_Original | 22 | 22 | 7984104 / 44344 | 11 | 1024×1024 | materials[21].pbrMetallicRoughness.baseColorTexture.index=11; materials[22].pbrMetallicRoughness.baseColorTexture.index=11; materials[25].pbrMetallicRoughness.baseColorTexture.index=11 |
| Perseverance_Original | 24 | 24 | 8190968 / 1686 | 12 | 256×256 | materials[23].normalTexture.index=12 |
| Perseverance_Original | 26 | 26 | 8197120 / 4062 | 14 | 256×256 | materials[26].normalTexture.index=14 |
| Perseverance_Original | 28 | 28 | 8208744 / 7008 | 13 | 256×256 | materials[26].pbrMetallicRoughness.baseColorTexture.index=13 |
| Perseverance_Original | 30 | 30 | 8233264 / 23804 | 15 | 1024×1024 | materials[27].normalTexture.index=15; materials[36].normalTexture.index=15 |
| Perseverance_Original | 32 | 32 | 9000448 / 11512 | 16 | 512×512 | materials[28].normalTexture.index=16; materials[33].normalTexture.index=16 |
| Perseverance_Original | 34 | 34 | 9055024 / 17174 | 17 | 512×512 | materials[31].pbrMetallicRoughness.baseColorTexture.index=17 |
| Perseverance_Original | 36 | 36 | 9161096 / 16834 | 18 | 512×512 | materials[35].pbrMetallicRoughness.baseColorTexture.index=18 |
| Perseverance_Original | 38 | 38 | 9241480 / 58592 | 19 | 1024×1024 | materials[41].pbrMetallicRoughness.metallicRoughnessTexture.index=19 |
| Perseverance_Original | 40 | 40 | 9758304 / 11512 | 20 | 512×512 | materials[42].normalTexture.index=20 |
| Perseverance_Original | 42 | 42 | 9812880 / 5920 | 21 | 512×512 | materials[46].pbrMetallicRoughness.baseColorTexture.index=21 |
| Acquired_Source_Inventory | 0 | 1 | 2424 / 35318 | 0,4 | 1024×1024 | materials[0].pbrMetallicRoughness.baseColorTexture.index=0; materials[7].pbrMetallicRoughness.baseColorTexture.index=4 |
| Acquired_Source_Inventory | 2 | 7 | 163840 / 25070 | 1 | 1024×1024 | materials[4].normalTexture.index=1 |
| Acquired_Source_Inventory | 4 | 9 | 248264 / 55612 | 2,14 | 1024×1024 | materials[4].pbrMetallicRoughness.baseColorTexture.index=2; materials[20].pbrMetallicRoughness.baseColorTexture.index=14 |
| Acquired_Source_Inventory | 6 | 13 | 473388 / 102 | 3 | 64×64 | materials[5].pbrMetallicRoughness.baseColorTexture.index=3 |
| Acquired_Source_Inventory | 8 | 18 | 476916 / 37192 | 5,12 | 1024×1024 | materials[8].pbrMetallicRoughness.baseColorTexture.index=5; materials[16].pbrMetallicRoughness.baseColorTexture.index=12 |
| Acquired_Source_Inventory | 10 | 22 | 675148 / 126 | 6,7,35,36 | 128×128 | materials[10].extensions.KHR_materials_specular.specularColorTexture.index=6; materials[10].pbrMetallicRoughness.baseColorTexture.index=7; materials[44].extensions.KHR_materials_specular.specularColorTexture.index=35; materials[44].pbrMetallicRoughness.baseColorTexture.index=36 |
| Acquired_Source_Inventory | 12 | 26 | 677516 / 55390 | 8,37 | 1024×1024 | materials[11].normalTexture.index=8; materials[45].normalTexture.index=37 |
| Acquired_Source_Inventory | 14 | 28 | 830180 / 87542 | 9,26,38 | 1024×1024 | materials[11].pbrMetallicRoughness.baseColorTexture.index=9; materials[32].pbrMetallicRoughness.baseColorTexture.index=26; materials[45].pbrMetallicRoughness.baseColorTexture.index=38 |
| Acquired_Source_Inventory | 16 | 42 | 1137392 / 94974 | 10,22,24,30,31,32 | 1024×1024 | materials[15].pbrMetallicRoughness.baseColorTexture.index=10; materials[27].pbrMetallicRoughness.baseColorTexture.index=22; materials[29].pbrMetallicRoughness.baseColorTexture.index=24; materials[36].pbrMetallicRoughness.baseColorTexture.index=30; materials[37].pbrMetallicRoughness.baseColorTexture.index=31; materials[41].pbrMetallicRoughness.baseColorTexture.index=32 |
| Acquired_Source_Inventory | 18 | 55 | 1503556 / 56952 | 11 | 1024×1024 | materials[16].normalTexture.index=11 |
| Acquired_Source_Inventory | 20 | 74 | 1698688 / 52328 | 13 | 1024×512 | materials[19].pbrMetallicRoughness.baseColorTexture.index=13 |
| Acquired_Source_Inventory | 22 | 84 | 1946088 / 44344 | 15,16,18 | 1024×1024 | materials[21].pbrMetallicRoughness.baseColorTexture.index=15; materials[22].pbrMetallicRoughness.baseColorTexture.index=16; materials[25].pbrMetallicRoughness.baseColorTexture.index=18 |
| Acquired_Source_Inventory | 24 | 89 | 2158232 / 1686 | 17 | 256×256 | materials[23].normalTexture.index=17 |
| Acquired_Source_Inventory | 26 | 114 | 2430460 / 4062 | 19 | 256×256 | materials[26].normalTexture.index=19 |
| Acquired_Source_Inventory | 28 | 116 | 2442084 / 7008 | 20 | 256×256 | materials[26].pbrMetallicRoughness.baseColorTexture.index=20 |
| Acquired_Source_Inventory | 30 | 122 | 2553436 / 23804 | 21,29 | 1024×1024 | materials[27].normalTexture.index=21; materials[36].normalTexture.index=29 |
| Acquired_Source_Inventory | 32 | 125 | 3327452 / 11512 | 23,27 | 512×512 | materials[28].normalTexture.index=23; materials[33].normalTexture.index=27 |
| Acquired_Source_Inventory | 34 | 158 | 3420040 / 17174 | 25 | 512×512 | materials[31].pbrMetallicRoughness.baseColorTexture.index=25 |
| Acquired_Source_Inventory | 36 | 169 | 3598328 / 16834 | 28 | 512×512 | materials[35].pbrMetallicRoughness.baseColorTexture.index=28 |
| Acquired_Source_Inventory | 38 | 214 | 3819788 / 58592 | 33 | 1024×1024 | materials[41].pbrMetallicRoughness.metallicRoughnessTexture.index=33 |
| Acquired_Source_Inventory | 40 | 217 | 4340428 / 11512 | 34 | 512×512 | materials[42].normalTexture.index=34 |
| Acquired_Source_Inventory | 42 | 280 | 4528856 / 5920 | 39 | 512×512 | materials[46].pbrMetallicRoughness.baseColorTexture.index=39 |
