from pathlib import Path
from urllib.parse import urlparse,unquote
import shlex,json,hashlib
base=Path.cwd();out=base/'outputs/Stage05B1';hard=out/'Hardening';project=Path('/Users/kapilash/Projects/AstroGatewayAndroid')
secrets=[]
for private in (project/'database/admin.local.env',project/'backend/.env'):
 if private.exists():
  for line in private.read_text().splitlines():
   if not line.strip() or line.lstrip().startswith('#') or '=' not in line:continue
   key,raw=line.split('=',1)
   try:parts=shlex.split(raw,comments=True);value=parts[0] if parts else ''
   except ValueError:continue
   if not value:continue
   if any(x in key.upper() for x in ('PASSWORD','SECRET','TOKEN')):secrets.append(value)
   if 'DATABASE' in key.upper() or 'DSN' in key.upper():
    if urlparse(value).password:secrets.extend([value,unquote(urlparse(value).password)])
files=list(hard.rglob('*'))+[out/n for n in ['Security_Hardening_Report.md','Local_Rehearsal_Results.md','Final_SQL_Manifest.md','Rollback_And_Recovery_Plan.md']]
hits=[]
for file in files:
 if not file.is_file() or '__pycache__' in str(file):continue
 content=file.read_text(errors='replace')
 if any(len(v)>=8 and v in content for v in secrets):hits.append(str(file.relative_to(base)))
 if '-----BEGIN PRIVATE KEY-----' in content or '-----BEGIN RSA PRIVATE KEY-----' in content:hits.append('private-key:'+str(file.relative_to(base)))
m=json.loads((hard/'Evidence/sql-manifest.json').read_text())['files']
sha_ok=all(hashlib.sha256((hard/'SQL'/n).read_bytes()).hexdigest()==sha for n,sha in m.items())
source_ok=all((hard/'Source'/n).read_bytes()==(project/'backend'/n).read_bytes() for n in ('database.py','main.py','run_local.py','.env.example','tests/test_hardening.py'))
orig=hashlib.sha256((base/'outputs/Stage05B/database/001_identity_and_mission_sessions.sql').read_bytes()).hexdigest()
proj=hashlib.sha256((project/'database/001_identity_and_mission_sessions.sql').read_bytes()).hexdigest()
r=json.loads((hard/'Evidence/sql-rehearsal.json').read_text())
result={'secret_scan_files':sum(p.is_file() and '__pycache__' not in str(p) for p in files),'secret_leak_hits':hits,'sql_manifest_matches':sha_ok,'project_source_matches_review_copy':source_ok,'original_migration_preserved':orig==proj=='269a983fe659e98f6083f3a8e46197ebe166d32e77eecc4a520539f1c686b59d','sql_pass':sum(t['status']=='PASS' for t in r['tests']),'sql_fail':sum(t['status']=='FAIL' for t in r['tests']),'remote_connection_attempted':False}
(hard/'Evidence/final-checks.json').write_text(json.dumps(result,indent=2)+'\n')
print(json.dumps(result,indent=2))
assert not hits and sha_ok and source_ok and result['original_migration_preserved']
