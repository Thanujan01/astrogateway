from pathlib import Path
import shutil
r=Path(__file__).resolve().parent
old=r.parent/'stage05b2'
s=(old/'api_integration.py').read_text()
s=s.replace('from lab_common import URL,CFG,A,B,C,signed_token,fixture_authenticate', '''import subprocess,os
INPUT=json.loads(sys.stdin.read())
URL=INPUT['url'];CFG={'rest_port':int(os.environ['LOCAL_GATEWAY_PORT'])}
A=INPUT['A'];B=INPUT['B'];C=INPUT['C']
POOLS=INPUT['tokens']
def signed_token(uid=A,sid=None,**changes):
 bucket='expired' if changes.get('exp',int(time.time())+600)<=time.time() else uid
 return POOLS[bucket].pop(0)
def fixture_authenticate(token):
 try:
  h,b,s=token.split('.')
  header=json.loads(base64.urlsafe_b64decode(h+'='*(-len(h)%4)))
  claims=json.loads(base64.urlsafe_b64decode(b+'='*(-len(b)%4)))
  if header.get('alg')!='RS256' or claims.get('iss')!=URL+'/auth/v1' or claims.get('aud')!='authenticated' or claims.get('role')!='authenticated' or claims['exp']<=time.time():return None
  sig=Path(os.environ['TMPDIR'])/'fixture-signature.bin';sig.write_bytes(base64.urlsafe_b64decode(s+'='*(-len(s)%4)))
  v=subprocess.run(['/usr/bin/openssl','dgst','-sha256','-verify',str(Path(__file__).parent/'issuer.pub'),'-signature',str(sig)],input=(h+'.'+b).encode(),capture_output=True)
  if v.returncode or claims.get('sub') not in (A,B,C):return None
  return claims
 except (ValueError,KeyError,TypeError):return None''')
s=s.replace("OUT=Path('outputs/Stage05B2/Evidence')","OUT=Path(os.environ['TMPDIR'])")
s=s.replace(" (OUT/'api-gateway-integration.json').write_text(json.dumps("," print(json.dumps(")
s=s.replace("},indent=2))\nexcept", "},indent=2))\nexcept") # print closes same nesting as removed write_text
# All mark progress to stderr; stdout contains only sanitized JSON.
s=s.replace(";print('PASS' if ok else 'FAIL',name)",";print('PASS' if ok else 'FAIL',name,file=sys.stderr)")
(r/'runtime/integration_probe.py').write_text(s)
shutil.copy2(old/'private/issuer.pub',r/'runtime/issuer.pub')
# Review copies of old scripts with distinct output location, no old evidence overwrite.
for n in ('regression.py','repeat_rehearsal.py'):
 s=(old/n).read_text().replace("OUT=Path('outputs/Stage05B2/Evidence')","OUT=Path('outputs/Stage05B3/Evidence')")
 (r/n).write_text(s)
print('Isolated test adapter uses public verification key and stdin fixture bearers only.')
