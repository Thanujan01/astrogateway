import json,subprocess,hashlib
from pathlib import Path
p=Path(__file__).parent/'Acquired'; manifest=[]
def get(url,path,md5=None):
 path.parent.mkdir(parents=True,exist_ok=True)
 subprocess.run(['curl','-fsSL','--max-time','60',url,'-o',str(path)],check=True)
 b=path.read_bytes(); ok=not md5 or hashlib.md5(b).hexdigest()==md5
 if not ok: raise ValueError('Publisher checksum mismatch')
 manifest.append(dict(path=str(path.resolve()),source_url=url,bytes=len(b),sha256=hashlib.sha256(b).hexdigest(),publisher_md5_matched=ok if md5 else None))
for a in ['rock_07','rock_09','sandy_gravel']:
 d=json.loads((p/(a+'_files.json')).read_text());dest=p/a
 if a!='sandy_gravel':
  f=d['gltf']['2k']['gltf'];get(f['url'],dest/(a+'.gltf'),f.get('md5'))
  for name,v in f.get('include',{}).items():
   if name.endswith('.bin'):get(v['url'],dest/name,v.get('md5'))
  f=d['fbx']['2k']['fbx'];get(f['url'],dest/(a+'.fbx'),f.get('md5'))
 for channel in ['Diffuse','nor_gl','Rough','AO']:
  f=d[channel]['2k']; k='png' if 'png' in f else 'jpg';v=f[k]
  get(v['url'],dest/(a+'_'+channel+'.'+k),v.get('md5'))
get('https://polyhaven.com/license',p/'PolyHaven_License.html')
get('https://creativecommons.org/publicdomain/zero/1.0/legalcode',p/'CC0_Legalcode.html')
(p/'Acquisition_Manifest.json').write_text(json.dumps(manifest,indent=2))
print('Acquired',len(manifest),'files; bytes',sum(x['bytes'] for x in manifest),'publisher checksums matched.')
