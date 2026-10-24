from pathlib import Path
import re,json,hashlib,collections
r=Path('/Users/kapilash/Projects/MissionRedHorizon');o=Path('outputs/Stage08A/Evidence');s=r/'Assets/AstroGateway/Scenes/MarsOutpost_Environment_v01.unity';text=s.read_text()
assets=[p for p in (r/'Assets').rglob('*') if p.is_file() and p.suffix!='.meta']
guid={}
for area in ('Assets','Library/PackageCache'):
 for p in (r/area).rglob('*.meta'):
  try:
   match=re.search(r'^guid: ([0-9a-f]{32})$',p.read_text(errors='replace'),re.M)
   if match:guid[match[1]]=str(p.relative_to(r))[:-5]
  except OSError:pass
refs=set(re.findall(r'guid: ([0-9a-f]{32})',text));missing=[g for g in refs if g not in guid and not g.startswith('0000000000000000')]
deps=json.loads((r/'Packages/manifest.json').read_text())['dependencies'];installed={}
for p in (r/'Library/PackageCache').glob('*/package.json'):
 try:
  q=json.loads(p.read_text());installed[q['name']]=q['version']
 except (KeyError,json.JSONDecodeError):pass
checks={k:{'required':v,'installed':installed.get(k),'status':'ENGINE_MODULE' if k.startswith('com.unity.modules.') else 'MATCH' if installed.get(k)==v else 'MISMATCH_OR_UNRESOLVED'} for k,v in deps.items()}
materials=list((r/'Assets').rglob('*.mat'));unresolvedmat=[]
for p in materials:
 for g in re.findall(r'm_Shader:.*guid: ([0-9a-f]{32})',p.read_text(errors='replace')):
  if g not in guid and not g.startswith('0000000000000000'):unresolvedmat.append(str(p.relative_to(r)))
result={'project':str(r),'editor_version':(r/'ProjectSettings/ProjectVersion.txt').read_text().splitlines()[0],'main_scene':str(s),'asset_count_excluding_meta':len(assets),'extension_counts':dict(collections.Counter(p.suffix for p in assets)),'scenes':[str(p.relative_to(r)) for p in assets if p.suffix=='.unity'],'prefabs':[str(p.relative_to(r)) for p in assets if p.suffix=='.prefab'],'scripts':[str(p.relative_to(r)) for p in assets if p.suffix=='.cs'],'character_model_files':[str(p.relative_to(r)) for p in assets if p.suffix.lower() in ('.fbx','.blend','.obj','.gltf','.glb')],'animation_files':[str(p.relative_to(r)) for p in assets if p.suffix in ('.anim','.controller','.overrideController')],'scene_unresolved_external_guids':missing,'material_unresolved_shader_guids':unresolvedmat,'scene_component_counts':{k:len(re.findall(r'^'+k+r':$',text,re.M)) for k in ('Camera','Light','Animator','Animation','CharacterController','Canvas','ParticleSystem','NavMeshAgent','MonoBehaviour','MeshRenderer','MeshCollider','LODGroup')},'dependencies':checks,'all_assets':[str(p.relative_to(r)) for p in assets]}
(o/'Asset_Inventory.json').write_text(json.dumps(result,indent=2)+'\n')
base=json.loads((o/'preservation_baseline.json').read_text());changed=[p for p,h in base.items() if not (r/p).exists() or hashlib.sha256((r/p).read_bytes()).hexdigest()!=h];current={str(p.relative_to(r)) for area in ('Assets','Packages','ProjectSettings') for p in (r/area).rglob('*') if p.is_file()}
(o/'Preservation_Check.json').write_text(json.dumps({'files_checked':len(base),'changed':changed,'added':sorted(current-set(base)),'limitation':'Baseline captured during preview before camera capture; no scene-save operations performed before or afterward'},indent=2)+'\n')
log=(r/'Logs/Editor.log').read_text(errors='replace');diag=[x for x in log.splitlines() if any(v in x for v in ('Entering Playmode','Reload Domain disabled','Reload Scene disabled','ASTROGATEWAY_ART_CAPTURE_OK','error CS','Exception:','Shader error'))]
(o/'Editor_Diagnostics.txt').write_text('\n'.join(diag)+'\n')
print(json.dumps({k:result[k] for k in ('asset_count_excluding_meta','extension_counts','scene_component_counts','scene_unresolved_external_guids','material_unresolved_shader_guids')}));print('Preservation changes:',len(changed))
