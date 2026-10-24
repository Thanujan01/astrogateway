import json,struct
from pathlib import Path
results=[]
for p in Path('work/stage08d1/VisualEvaluation/Assets/ApprovedAssets').glob('*.glb'):
 b=p.read_bytes();n=struct.unpack_from('<I',b,12)[0];j=json.loads(b[20:20+n]);results.append({'asset':p.name,'extensionsUsed':j.get('extensionsUsed',[]),'extensionsRequired':j.get('extensionsRequired',[]),'imageMimes':sorted(set(i.get('mimeType','') for i in j.get('images',[]))),'materials':len(j.get('materials',[])),'skins':len(j.get('skins',[])),'animations':len(j.get('animations',[]))})
Path('outputs/Stage08D1/GLB_Compatibility_Inspection.json').write_text(json.dumps(results,indent=2))
print(json.dumps(results[:3],indent=2))
