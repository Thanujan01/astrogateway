from pathlib import Path
import json,struct,hashlib
p=Path(__file__).parent/'Acquired';out=[]
def dims(b):
 if b.startswith(b'\x89PNG'):return struct.unpack_from('>II',b,16)
 if b[:2]==b'\xff\xd8':
  i=2
  while i<len(b)-9:
   if b[i]!=255:i+=1;continue
   marker=b[i+1];i+=2
   if marker in [0xd8,0xd9]:continue
   n=int.from_bytes(b[i:i+2],'big')
   if marker in [0xc0,0xc1,0xc2]:return (int.from_bytes(b[i+5:i+7],'big'),int.from_bytes(b[i+3:i+5],'big'))
   i+=n
 return None
for a in ['rock_07','rock_09']:
 d=json.loads((p/a/(a+'.gltf')).read_text());pr=[q for m in d['meshes'] for q in m['primitives']];out.append(dict(asset=a,triangles=sum(d['accessors'][q['indices']]['count']//3 for q in pr),meshes=len(d['meshes']),materials=len(d['materials']),skins=len(d.get('skins',[])),animations=len(d.get('animations',[])),position_bounds=[{k:d['accessors'][q['attributes']['POSITION']].get(k) for k in ['min','max']} for q in pr]))
b=(p/'Perseverance.glb').read_bytes();n,t=struct.unpack_from('<II',b,12);d=json.loads(b[20:20+n]);binstart=20+n+8;images=[]
for i,x in enumerate(d['images']):
 v=d['bufferViews'][x['bufferView']];raw=b[binstart+v.get('byteOffset',0):binstart+v.get('byteOffset',0)+v['byteLength']];images.append(dict(name=x.get('name'),dimensions=dims(raw),bytes=len(raw)))
 if i<3:
  (p/('NASA_Embedded_'+str(i)+('.png' if raw.startswith(b'\x89PNG') else '.jpg'))).write_bytes(raw)
Path('outputs/Stage08D/Acquired_Asset_Inspection.json').write_text(json.dumps({'rocks':out,'nasa_embedded_images':images,'staged_textures':[dict(file=str(f),dimensions=dims(f.read_bytes())) for f in p.rglob('*.png')]},indent=2))
manifest=json.loads((p/'Acquisition_Manifest.json').read_text());manifest.append(dict(path=str((p/'Perseverance.glb').resolve()),source_url='https://raw.githubusercontent.com/nasa/NASA-3D-Resources/master/3D%20Models/Mars%202020%20Perseverance%20Rover/Mars%202020%20Perseverance%20Rover.glb',bytes=len(b),sha256=hashlib.sha256(b).hexdigest(),publisher_md5_matched=None));(p/'Acquisition_Manifest.json').write_text(json.dumps(manifest,indent=2));print(json.dumps(out));print('NASA texture dimensions',sorted(set(str(x['dimensions']) for x in images)))
