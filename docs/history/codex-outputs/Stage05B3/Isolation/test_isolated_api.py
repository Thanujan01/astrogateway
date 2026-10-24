import sys,json,time,uuid
from pathlib import Path
from launch import run
sys.path.insert(0,str(Path(__file__).resolve().parent.parent/'stage05b2'))
from lab_common import signed_token,A,B,C,URL,CFG
# Operator-only signer. No private key/password is passed or mounted into child.
pools={u:[signed_token(u,str(uuid.uuid4())) for i in range(12)] for u in (A,B,C)}
pools['expired']=[signed_token(A,str(uuid.uuid4()),exp=int(time.time())-90)]
request=json.dumps({'url':URL,'A':A,'B':B,'C':C,'tokens':pools})
r=run('integration_probe.py',request,{'LOCAL_GATEWAY_PORT':str(CFG['rest_port'])})
out=Path('outputs/Stage05B3/Evidence')
(out/'isolated-api.log').write_text(r.stderr)
if r.returncode:
 print('FAIL isolated API; diagnostic log contains names only.');raise SystemExit(1)
data=json.loads(r.stdout);data['kernel_sandbox']=True;data['private_keys_or_db_credentials_supplied']=False
(out/'isolated-api-integration.json').write_text(json.dumps(data,indent=2))
print('Isolated FastAPI-to-gateway checks passed:',len(data['tests']))
