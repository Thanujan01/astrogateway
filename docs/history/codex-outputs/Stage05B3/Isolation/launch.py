"""Operator launcher; fixed sandbox and cleared environment, never imports private configuration."""
from pathlib import Path
import subprocess,os,sys
ROOT=Path(__file__).resolve().parent
PYTHON=ROOT.parent/'stage05b/venv/bin/python'
def run(script='probe.py',stdin=None,extra=None):
 if script not in ('probe.py','integration_probe.py','serve.py','unit_probe.py'):raise ValueError('Unsupported isolated program')
 env={'PATH':'/usr/bin:/bin','HOME':str(ROOT/'scratch'),'TMPDIR':str(ROOT/'scratch'),'PYTHONDONTWRITEBYTECODE':'1','PYTHONNOUSERSITE':'1','LANG':'en_US.UTF-8'}
 # No parent environment is merged. Only non-secret fixed local fixture metadata allowed.
 if extra:
  if set(extra)-{'LOCAL_GATEWAY_PORT'}:raise ValueError('Unsupported fixture config')
  env.update(extra)
 return subprocess.run(['/usr/bin/sandbox-exec','-f',str(ROOT/'runtime.sb'),str(PYTHON),str(ROOT/'runtime'/script)],input=stdin,text=True,capture_output=True,env=env,cwd=ROOT/'runtime',timeout=120)
if __name__=='__main__':
 
 for k in ('ADMIN_CANARY','DATABASE_URL','DATABASE_PASSWORD','PGPASSWORD','SUPABASE_SERVICE_ROLE_KEY','AWS_SECRET_ACCESS_KEY','DYLD_INSERT_LIBRARIES','PYTHONPATH'):os.environ[k]='disposable-nonsecret-parent-canary'
 r=run();sys.stdout.write(r.stdout)
 if r.returncode:print('Isolation probe failed; no private output emitted.',file=sys.stderr)
 raise SystemExit(r.returncode)
