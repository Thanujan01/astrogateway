from pathlib import Path
import json,hashlib
p=Path('/Users/kapilash/Projects/MissionRedHorizon');o=Path('outputs/Stage04');a=p/'Assets/AstroGateway/Environment'
files=sorted(f for f in a.rglob('*') if f.is_file() and f.suffix!='.meta')
lines=['# Stage 04 original asset and rights ledger','','All new meshes, textures, shader code, stencil glyphs and assemblies were procedurally authored for SK2LB-AstroLab with Codex during this task. They are original project work; no external asset pack, photograph, font, sound, NASA model or purchased content was imported. Unity primitive meshes and URP Lit are supplied by the installed Unity Editor/package and remain subject to the project’s Unity terms. No separate third-party asset license was assumed or granted.','','Geology and palette are artistic interpretations informed by the approved research. Architecture is a fictional training-outpost proposal, not a NASA design or engineering certification. No NASA logos or endorsement marks were used.','','Reference links (no source files copied):','','- [NASA Jezero panorama, PIA24921](https://science.nasa.gov/photojournal/detailed-panorama-of-mars-jezero-crater-delta/): bedding, heterogeneous eroded rocks; main image enhanced color, Figure A natural color; NASA/JPL-Caltech/ASU/MSSS.','- [NASA Mars facts](https://science.nasa.gov/mars/facts/): muted heterogeneous dusty setting.','- [USGS Jezero floor study](https://www.usgs.gov/publications/aqueously-altered-igneous-rocks-sampled-floor-jezero-crater-mars): igneous comparison-site context.','- [NASA greenhouse prototype](https://www.nasa.gov/science-research/lunar-martian-greenhouses-designed-to-mimic-those-on-earth/): protected controlled-environment module concept, not a deployed Mars habitat.','- [NASA media guidance](https://www.nasa.gov/nasa-brand-center/images-and-media/): future media imports require item-specific checks; none performed/imported here.','','## Complete new Environment asset manifest','','|Path|Bytes|Provenance|','|---|---:|---|']
for f in files: lines.append(f'|{f.relative_to(p)}|{f.stat().st_size}|Original Stage 04 authoring / Unity primitive assembly|')
(o/'Assets_And_Attribution.md').write_text('\n'.join(lines)+'\n')
manifest=[{'path':str(f.relative_to(p)),'bytes':f.stat().st_size,'sha256':hashlib.sha256(f.read_bytes()).hexdigest()} for f in files]
(o/'Asset_Manifest.json').write_text(json.dumps(manifest,indent=2))
base=json.loads(Path('work/stage04/preserved-hashes.json').read_text());changed=[]
for fn,h in base.items():
 f=Path(fn)
 if not f.exists() or hashlib.sha256(f.read_bytes()).hexdigest()!=h:changed.append(fn)
(o/'Preservation_Check.json').write_text(json.dumps({'baselineFiles':len(base),'changed':changed},indent=2))
print('Asset files:',len(files),'preserved baseline changes:',changed)
