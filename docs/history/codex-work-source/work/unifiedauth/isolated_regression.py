from pathlib import Path
import shutil,sys,json,subprocess,os,uuid
root=Path('work/unifiedauth').resolve();runtime=root/'runtime';scratch=root/'scratch';runtime.mkdir(exist_ok=True);scratch.mkdir(mode=0o700,exist_ok=True)
old=Path('work/stage05b3').resolve();backend=Path('/Users/kapilash/Projects/AstroGatewayAndroid/backend')
for name in ['main.py','database.py']:shutil.copy2(backend/name,runtime/name)
shutil.copytree(backend/'tests',runtime/'tests',dirs_exist_ok=True)
for name in ['probe.py','unit_probe.py','serve.py','issuer.pub']:shutil.copy2(old/'runtime'/name,runtime/name)
# Existing confinement applies to a new isolated immutable copy; no management directory is exposed.
policy=(old/'runtime.sb').read_text().replace(str(old/'runtime'),str(runtime)).replace(str(old/'scratch'),str(scratch));(root/'runtime.sb').write_text(policy)
(scratch/'admin-alias').symlink_to('/Users/kapilash/Projects/AstroGatewayAndroid/database/admin.local.env') if not (scratch/'admin-alias').is_symlink() else None
p=runtime/'probe.py';s=p.read_text().replace(str(old/'runtime.sb'),str(root/'runtime.sb'));p.write_text(s)
s=(old/'runtime/integration_probe.py').read_text();needle="  launch=str(uuid.uuid4());payload="
extra='''  r=client.post('/v1/me/display-name',headers=ha,json={'display_name':'Unified A'})
  mark('unified profile update through isolated API and real gateway keeps A UUID',r.status_code==200 and r.json()['user_id']==A and r.json()['display_name']=='Unified A')
  r=client.post('/v1/me/display-name',headers=hb,json={'display_name':'Unified B'})
  mark('B own update and independent profile persistence',r.status_code==200 and r.json()['user_id']==B and client.get('/v1/me',headers=ha).json()['display_name']=='Unified A')
  mark('user-ID override in profile request forbidden',client.post('/v1/me/display-name',headers=ha,json={'display_name':'Attack','user_id':B}).status_code==422)
  mark('profile endpoint rejects unauthenticated and malformed requests',client.post('/v1/me/display-name',json={'display_name':'Attack'}).status_code==401)
  mark('immutable level update forbidden in profile contract',client.post('/v1/me/display-name',headers=ha,json={'display_name':'Attack','training_level':10}).status_code==422)
'''
s=s.replace(needle,extra+needle);p=runtime/'integration_probe.py';p.write_text(s)
sys.path.insert(0,str(Path('work/stage05b2').resolve()));from lab_common import signed_token,A,B,C,URL,CFG
out=Path('outputs/UnifiedAuth/Evidence')
env={'PATH':'/usr/bin:/bin','HOME':str(scratch),'TMPDIR':str(scratch),'PYTHONDONTWRITEBYTECODE':'1','PYTHONNOUSERSITE':'1','LANG':'en_US.UTF-8','LOCAL_GATEWAY_PORT':str(CFG['rest_port'])}
for name in (['integration_probe.py'] if '--integration-only' in sys.argv else ['probe.py','unit_probe.py','integration_probe.py']):
 request=None
 if name=='integration_probe.py':
  pools={u:[signed_token(u,str(uuid.uuid4())) for _ in range(12)] for u in (A,B,C)};pools['expired']=[signed_token(A,str(uuid.uuid4()),exp=1)]
  request=json.dumps({'url':URL,'A':A,'B':B,'C':C,'tokens':pools})
 r=subprocess.run(['/usr/bin/sandbox-exec','-f',str(root/'runtime.sb'),str(Path.cwd()/'work/stage05b/venv/bin/python'),str(runtime/name)],cwd=runtime,env=env,input=request,text=True,capture_output=True,timeout=120)
 (out/('isolated-'+name+'.log')).write_text(r.stdout if name!='integration_probe.py' else r.stderr)
 if name=='integration_probe.py' and r.returncode==0:(out/'isolated-api-integration.json').write_text(r.stdout)
 (out/('isolated-'+name+'.stderr.txt')).write_text(r.stderr)
 print(name,'PASS' if r.returncode==0 else 'FAIL')
 if r.returncode:raise SystemExit(1)
