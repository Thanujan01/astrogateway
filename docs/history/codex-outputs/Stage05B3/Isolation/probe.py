import os,json,subprocess,socket,pathlib
results=[]
def mark(n,v):results.append({'name':n,'status':'PASS' if v else 'FAIL'})
paths=[ '/Users/kapilash/Projects/AstroGatewayAndroid/database/admin.local.env', '/Users/kapilash/Projects/AstroGatewayAndroid/backend/.env', '/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/work/stage05b2/private/lab.json', '/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/work/stage05b2/private/issuer.key','/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/work/stage05b2/private/postgrest.conf']
for p in paths:
 try:pathlib.Path(p).read_bytes();ok=False
 except PermissionError:ok=True
 except FileNotFoundError:ok=False
 mark('kernel read denial: '+p,ok)
mark('no forbidden inherited environment',all(k not in os.environ for k in ['DATABASE_URL','DATABASE_PASSWORD','PGPASSWORD','SUPABASE_SERVICE_ROLE_KEY','AWS_SECRET_ACCESS_KEY','ADMIN_CANARY','LD_PRELOAD','DYLD_INSERT_LIBRARIES','PYTHONPATH']))
r=subprocess.run(['/bin/cat',paths[0]],capture_output=True)
mark('exec child retains file confinement',r.returncode!=0 and not r.stdout)
for host,port in [('127.0.0.1',5432),('127.0.0.1',9),('1.1.1.1',443)]:
 s=socket.socket();s.settimeout(.2)
 try:s.connect((host,port));ok=False
 except PermissionError:ok=True
 except OSError:ok=False
 finally:s.close()
 mark('kernel outbound denial '+host+':'+str(port),ok)
try:subprocess.run(['/bin/ps','e','-p',str(os.getppid())],check=True,capture_output=True);ok=False
except (subprocess.CalledProcessError,PermissionError):ok=True
mark('parent environment process inspection denied',ok)
try:os.kill(os.getppid(),0);ok=False
except PermissionError:ok=True
mark('cannot signal operator parent',ok)
symlink=pathlib.Path(os.environ['TMPDIR'])/'admin-alias'
try:symlink.read_bytes();ok=False
except PermissionError:ok=True
except FileNotFoundError:ok=False
mark('scratch symlink cannot escape file boundary',ok)
for p in [str(pathlib.Path(__file__).parent/'main.py'),str(pathlib.Path(__file__).parent.parent/'runtime.sb')]:
 try:
  with open(p,'ab'):pass
  ok=False
 except PermissionError:ok=True
 mark('cannot modify trusted code or confinement policy: '+p,ok)
try:subprocess.run(['/usr/bin/sudo','-n','/usr/bin/true'],capture_output=True,check=True);ok=False
except (PermissionError,subprocess.CalledProcessError):ok=True
mark('privileged system operation unavailable',ok)
r=subprocess.run(['/usr/bin/sandbox-exec','-p','(version 1)(allow default)','/bin/cat',paths[0]],capture_output=True)
mark('nested sandbox cannot widen child access',r.returncode!=0 and not r.stdout)
import main,database,httpx
mark('staged actual application imports',main.Settings().configured is False)
print(json.dumps({'uid':os.getuid(),'isolation':'macOS kernel sandbox; same UID; inherited confinement','tests':results,'environment_keys':sorted(os.environ)},indent=2))
raise SystemExit(0 if all(x['status']=='PASS' for x in results) else 1)
