from pathlib import Path
import os,subprocess,json,time,urllib.request
r=Path(__file__).resolve().parent
p=None
try:
 env={'PATH':'/usr/bin:/bin','HOME':str(r/'scratch'),'TMPDIR':str(r/'scratch'),'PYTHONDONTWRITEBYTECODE':'1','PYTHONNOUSERSITE':'1','LANG':'en_US.UTF-8'}
 p=subprocess.Popen(['/usr/bin/sandbox-exec','-f',str(r/'runtime.sb'),str(r.parent/'stage05b/venv/bin/python'),str(r/'runtime/serve.py')],env=env,cwd=r/'runtime',stdin=subprocess.DEVNULL,stdout=subprocess.DEVNULL,stderr=subprocess.DEVNULL,close_fds=True)
 data=None
 for i in range(50):
  try:
   with urllib.request.urlopen('http://127.0.0.1:8001/health',timeout=.3) as response:data=json.loads(response.read())
   break
  except Exception:time.sleep(.1)
 ok=bool(data) and p.poll() is None
 Path('outputs/Stage05B3/Evidence/isolated-service-health.json').write_text(json.dumps({'actual_application_lifespan':ok,'kernel_sandbox':True,'configuration':'connectivity-only; no hosted calls','database_credentials_supplied':False,'health':data},indent=2))
 print('Actual isolated local health service:', 'PASS' if ok else 'FAIL')
 if not ok:raise SystemExit(1)
finally:
 if p and p.poll() is None:p.terminate();p.wait(timeout=5)
