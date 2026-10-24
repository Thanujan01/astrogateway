from pathlib import Path
r=Path(__file__).resolve().parent
p=r/'launch.py';s=p.read_text().replace("('probe.py','integration_probe.py')","('probe.py','integration_probe.py','serve.py')")
s=s.replace("os.environ['ADMIN_CANARY']='disposable-nonsecret-parent-canary'","\n for k in ('ADMIN_CANARY','DATABASE_URL','DATABASE_PASSWORD','PGPASSWORD','SUPABASE_SERVICE_ROLE_KEY','AWS_SECRET_ACCESS_KEY','DYLD_INSERT_LIBRARIES','PYTHONPATH'):os.environ[k]='disposable-nonsecret-parent-canary'")
p.write_text(s)
p=r/'runtime/probe.py';s=p.read_text().replace("import os,json,subprocess,socket,pathlib","import os,json,subprocess,socket,pathlib")
s=s.replace("import main,database,httpx",'''symlink=pathlib.Path(os.environ['TMPDIR'])/'admin-alias'
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
import main,database,httpx''')
p.write_text(s)
alias=r/'scratch/admin-alias'
if not alias.exists() and not alias.is_symlink():alias.symlink_to('/Users/kapilash/Projects/AstroGatewayAndroid/database/admin.local.env')
