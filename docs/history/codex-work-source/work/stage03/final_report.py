from pathlib import Path
import re,json,hashlib
base=Path('/Users/kapilash/Projects/MissionRedHorizon')
out=Path('outputs/Stage03')
scene=(base/'Assets/AstroGateway/Greybox/MarsOutpost_Greybox.unity').read_text()
objects={};positions={}
for b in scene.split('--- !u!'):
 h=re.match(r'(\d+) &(\d+)',b)
 if not h:continue
 typ,idx=map(int,h.groups())
 if typ==1:
  n=re.search(r'  m_Name: (.*)',b)
  if n:objects[idx]=n.group(1)
 if typ==4:
  g=re.search(r'  m_GameObject: \{fileID: (\d+)\}',b)
  p=re.search(r'  m_LocalPosition: \{x: ([^,]+), y: ([^,]+), z: ([^}]+)\}',b)
  if g and p:positions[int(g.group(1))]=list(map(float,p.groups()))
expected={'H_CommandHabitat':(-65,-60,36,28),'G_Greenhouse':(-65,-10,30,22),'R_RoverBay':(-15,-95,32,20),'P_PowerStation':(25,-45,24,24),'S_SolarArray':(75,-55,48,40),'C_ScienceStaging':(-5,20,24,20),'D_LayeredDeltaTerrain':(0,85,70,40),'B_DarkRockComparison':(75,35,30,26)}
rows=[];coords={}
for n,(x,z,w,d) in expected.items():
 idx=next(i for i,name in objects.items() if name==n);p=positions[idx]
 assert abs(p[0]-x)<.001 and abs(p[2]-z)<.001
 coords[n]=p
 assert any(name==f'ReservedFootprint_{w}x{d}m_NoCollision' for name in objects.values())
 rows.append(f'| {n} | ({p[0]:g}, {p[1]:.2f}, {p[2]:g}) | {w} × {d} m |')
assert 1<=coords['B_DarkRockComparison'][1]<=2
for f,h in json.loads(Path('work/stage03/design-hashes.json').read_text()).items():assert hashlib.sha256(Path(f).read_bytes()).hexdigest()==h
(out/'Zone_Coordinates.json').write_text(json.dumps(coords,indent=2))
text='''# Stage 03 completion report

Completed 8 October 2026. Unity 6000.6.4f1; URP 17.6.0. Original primitives and one lightweight procedural ground mesh; no imported assets, gameplay controller, mission logic, packages, or React changes.

## Scene and hierarchy

Scene: `/Users/kapilash/Projects/MissionRedHorizon/Assets/AstroGateway/Greybox/MarsOutpost_Greybox.unity`

```
MarsOutpost_Greybox_1unit_1meter
  01_Terrain_250x250m
    ContinuousWalkableGround
    PlayableBoundary_250x250_NoCollision
  02_EnvironmentZones
    H_CommandHabitat
    G_Greenhouse
    R_RoverBay
    P_PowerStation
    S_SolarArray
    C_ScienceStaging
    D_LayeredDeltaTerrain
    B_DarkRockComparison
  03_MissionRoute_4mClearCorridor
    Waypoint_00 … Waypoint_11
    MissionSegment_00 … MissionSegment_10
    RoutePost_* (non-colliding, offset from corridor)
  04_Lighting
    AfternoonSun_Elevation30_FromSW
  05_TemporaryValidation
    PlayerScaleReference_1p8m_NoController
    CompositionCamera_Overview (active)
    CompositionCamera_Spawn / Power / Delta (disabled reference cameras)
```

Working bounds X/Z −125…125 m, north +Z, east +X; 1 unit = 1 m. A 101 × 101 vertex mesh has 2.5 m grid spacing, 20,000 triangles and continuous collision. Flat southern pad; 4 m northern working terrace; rear ledges and rising ground provide the delta silhouette. Reserved footprints include work aprons, distinct from building sizes. Habitat shell is 14 × 8 × 4 m; greenhouse body is 18 m long and 7 m diameter; canopy is 18 × 10 m at 4 m height. Power equipment is placed north of the unchanged route within its approved reservation.

## Saved zone coordinates

| Zone | Center (X,Y,Z), meters | Reserved footprint |
|---|---|---|
'''+ '\n'.join(rows)+'''

All X/Z centers and reserved footprints were checked against the approved table. B ground elevation is now within its 1–2 m range. The source documents were hash-checked and remain unchanged.

## Route and sightlines

The exact approved 12-waypoint route measures 421.34 m in plan and 421.62 m sampled along terrain. At the explicit test speed of 2.5 m/s, estimated traversal is 168.6 seconds (2 minutes 49 seconds), excluding stops. This is a distance-based estimate, not a timed player-controller test.

857 terrain/corridor samples at approximately 0.5 m spacing find ground and no static obstacle intersecting the conservative 4 m corridor test. Maximum sampled main-route grade is 4.87°, below the 12° design limit. Scene landmarks tested by raycasts: spawn→power mast clear, spawn→delta hits its intended ledge, delta→habitat hits the intended roof. Captures visually confirm the spawn composition. No NavMesh or final controller was created; camera collision, secondary-route walking, first-time user navigation and accessibility playtests remain future work.

## Materials and light

Shared opaque URP Lit materials: ochre ground, pale architecture, layered stone, dark rocks, graphite equipment. URP Unlit sand-colored route guide and restrained cyan/amber markers distinguish diagnostic layout features. No final textures or expensive effects.

One fixed directional light, intensity 1.6 (art-tuned Unity value), warm-neutral color, soft shadows, source southwest at 30° elevation. Light travels northeast and downward; shadows extend northeast. Trilight ambient colors provide readable sky/equator/ground fill. Dusty solid camera background; fog and post-processing disabled. No physical irradiance calibration, lightmap bake, or atmosphere simulation is claimed.

## Compilation, loading, and Web test

Fresh batch validation exits 0; no C# compiler errors, missing scripts, or invalid materials found. Normal Editor reopen log confirms the saved scene loads and final URP captures complete. The initial unnamed scene was preserved at `Assets/AstroGateway/Greybox/PreGreybox_Untitled_Backup.unity`; existing SampleScene was preserved.

Final Web build succeeded: zero errors, three build warnings, reported build size 10,565,904 bytes (~10.1 MiB). Build output is `outputs/Stage03/Web/` beside this report. It contains a fixed overview camera and no runtime movement/gameplay. Build logs contain Unity-toolchain WebGPU compatibility/deprecation notices; no custom code compilation failure.

Browser smoke test: the final output actually loaded and rendered terrain, structures, route guide and pale/dark URP material distinctions in Codex's in-app browser at `http://127.0.0.1:8766/` (1280 × 720 screenshot viewport; default Unity canvas 960 × 600). Served with Brotli/MIME headers on loopback. The first attempt while files were rebuilding produced an invalid data-file-size alert; a completed-output reload rendered successfully, and the refined final build also rendered after reload. A non-blocking unsupported/stripped FSR shader warning remains in the inherited rendering configuration. No post-processing effects are requested. This is a rendering/load smoke test only, not a full browser compatibility or performance certification. Chrome/Safari device matrix, FPS/p95, memory, thermal behavior and mobile input were not measured.

## Captures and evidence

- [Overview](CompositionCamera_Overview.png)
- [Spawn](CompositionCamera_Spawn.png)
- [Power approach](CompositionCamera_Power.png)
- [Delta return sightline](CompositionCamera_Delta.png)
- [Geometry checks](Geometry_Validation.md)
- [Editor batch log](Final_Editor_Validation.log)
- [Web build log](Final_Web_Build.log)
- [Normal scene reopening log](Scene_Reopen.log)

These PNGs are Unity-rendered final scene captures, not concept images or browser screenshots.

## Limitations and next milestone

This is intentionally primitive greybox geometry. Broad sand strips and footprint outlines are diagnostic guides, not final paths. Boundary outlines do not constrain movement. No detailed geology meshes, astronaut rig, controller, mission interactions, tactical map, dust event, audio, final textures, LOD authoring, light bake or optimized browser profiles were implemented. The basic lighting and target scene render correctly, but the inherited FSR warning should be resolved when explicit desktop/mobile profiles are validated.

Next recommended work, subject to approval: review the spawn/power/delta compositions and actual traversal with a temporary test rig; then begin one modular habitat/power/solar art kit and a layered-rock material/mesh family. Keep the 250 m bounds, coordinates and approved route stable. Stop at Stage 03; no environment art pass has begun.
'''
(out/'Stage03_Completion_Report.md').write_text(text)
print('Final report, coordinates and design hashes verified.')
