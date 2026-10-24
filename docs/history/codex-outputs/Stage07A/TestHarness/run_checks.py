import json,sys,subprocess,os,uuid,time
from pathlib import Path
sys.path.insert(0,str(Path('work/stage05b2').resolve()))
from lab_common import signed_token,A,B,URL,CFG
root=Path('work/stage07a').resolve();a=signed_token(A,str(uuid.uuid4()));h,b,s=a.split('.');claims=json.loads(__import__('base64').urlsafe_b64decode(b+'='*(-len(b)%4)));claims['sub']=B;forged=h+'.'+__import__('base64').urlsafe_b64encode(json.dumps(claims).encode()).decode().rstrip('=')+'.'+s
payload=json.dumps({'url':URL,'A':A,'B':B,'port':CFG['rest_port'],'tokens':{'A':a,'B':signed_token(B,str(uuid.uuid4())),'expired':signed_token(A,str(uuid.uuid4()),exp=int(time.time())-100),'forged':forged}})
env={'PATH':'/usr/bin:/bin','HOME':str(root/'scratch'),'TMPDIR':str(root/'scratch'),'PYTHONDONTWRITEBYTECODE':'1','PYTHONNOUSERSITE':'1','LANG':'en_US.UTF-8'}
r=subprocess.run(['/usr/bin/sandbox-exec','-f',str(root/'runtime.sb'),str(Path('work/stage05b/venv').resolve()/'bin/python'),str(root/'runtime'/('trusted_checks.py' if '--trusted' in sys.argv else 'ai_checks.py'))],env=env,cwd=root/'runtime',input=payload,capture_output=True,text=True,timeout=90)
# Use venv executable path, not resolved real interpreter: retain site-packages.
out=Path('outputs/Stage07A/Evidence')
(out/('ai-trusted.log' if '--trusted' in sys.argv else 'ai-api.log')).write_text(r.stderr)
try:d=json.loads(r.stdout)
except ValueError:d={'failure_type':'RuntimeStartup','tests':[]}
(out/('ai-trusted.json' if '--trusted' in sys.argv else 'ai-api.json')).write_text(json.dumps(d,indent=2));print('AI API checks:',len(d['tests']),'PASS' if r.returncode==0 else 'FAIL');raise SystemExit(r.returncode)
