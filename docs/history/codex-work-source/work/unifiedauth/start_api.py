"""Operator-side launcher. Copies public client configuration only; no private DB file is loaded."""
import os,subprocess,json
from pathlib import Path
root=Path(__file__).resolve().parent;runtime=root/'runtime';scratch=root/'scratch'
values={}
for line in Path('/Users/kapilash/Projects/AstroGatewayAndroid/auth.local.properties').read_text().splitlines():
 if '=' in line and not line.lstrip().startswith('#'):
  k,v=line.split('=',1);values[k.strip()]=v.strip()
assert values['supabase.url']=='https://jvkwofmxtxjhdwtwdboz.supabase.co' and values['supabase.publishableKey'].startswith('sb_publishable_')
policy=(root/'runtime.sb').read_text().replace('localhost:8001','localhost:8000');(root/'service.sb').write_text(policy)
(runtime/'serve_unified.py').write_text("import uvicorn\nfrom main import app\nuvicorn.run(app,host='127.0.0.1',port=8000,access_log=False,log_level='critical')\n")
env={'PATH':'/usr/bin:/bin','HOME':str(scratch),'TMPDIR':str(scratch),'PYTHONDONTWRITEBYTECODE':'1','PYTHONNOUSERSITE':'1','LANG':'en_US.UTF-8','SUPABASE_URL':values['supabase.url'],'SUPABASE_PUBLISHABLE_KEY':values['supabase.publishableKey']}
print('Starting confined local API on 127.0.0.1:8000 with public test-project configuration only; no hosted validation claimed.',flush=True)
raise SystemExit(subprocess.call(['/usr/bin/sandbox-exec','-f',str(root/'service.sb'),str(root.parent/'stage05b/venv/bin/python'),str(runtime/'serve_unified.py')],cwd=runtime,env=env,stdin=subprocess.DEVNULL,close_fds=True))
