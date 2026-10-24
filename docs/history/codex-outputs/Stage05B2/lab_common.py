from pathlib import Path
import json,ssl,asyncpg,base64,subprocess,time
ROOT=Path(__file__).resolve().parent
CFG=json.loads((ROOT/'private/lab.json').read_text())
URL='https://jvkwofmxtxjhdwtwdboz.supabase.co'
A='11111111-1111-4111-8111-111111111111';B='22222222-2222-4222-8222-222222222222';C='33333333-3333-4333-8333-333333333333'
SA='aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa';SB='bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbbb'
def context():
 c=ssl.create_default_context(cafile=CFG['ca_file']);c.verify_flags|=ssl.VERIFY_X509_STRICT;c.check_hostname=True;c.verify_mode=ssl.CERT_REQUIRED;return c
async def connect(role='lab_admin',password=None):
 if role not in ('lab_admin','lab_gateway','astrogateway_runtime','lab_attacker','authenticated','astrogateway_schema_owner'):raise ValueError('Unexpected fixture role')
 p=password if password is not None else CFG['admin_password'] if role=='lab_admin' else CFG['old_password']
 return await asyncpg.connect(host='localhost',port=CFG['port'],database='postgres',user=role,password=p,ssl=context(),command_timeout=10)
def signed_token(uid=A,sid=SA,**changes):
 c={'iss':URL+'/auth/v1','role':'authenticated','aud':'authenticated','sub':uid,'session_id':sid,'exp':int(time.time())+600,'iat':int(time.time()),'is_anonymous':False};c.update(changes)
 enc=lambda b:base64.urlsafe_b64encode(b).decode().rstrip('=')
 head=enc(json.dumps({'alg':'RS256','kid':'disposable-stage05b2','typ':'JWT'},separators=(',',':')).encode());body=enc(json.dumps(c,separators=(',',':')).encode());data=(head+'.'+body).encode()
 result=subprocess.run(['openssl','dgst','-sha256','-sign',CFG['issuer_key']],input=data,capture_output=True,check=True)
 return head+'.'+body+'.'+enc(result.stdout)
def fixture_authenticate(token):
 try:
  h,b,s=token.split('.');header=json.loads(base64.urlsafe_b64decode(h+'='*(-len(h)%4)));claims=json.loads(base64.urlsafe_b64decode(b+'='*(-len(b)%4)))
  if header.get('alg')!='RS256' or claims.get('iss')!=URL+'/auth/v1' or claims.get('role')!='authenticated' or claims.get('aud')!='authenticated' or claims['exp']<=time.time():return None
  signature=ROOT/'private/verify-signature.bin';signature.write_bytes(base64.urlsafe_b64decode(s+'='*(-len(s)%4)));signature.chmod(0o600)
  pub=ROOT/'private/issuer.pub'
  r=subprocess.run(['openssl','dgst','-sha256','-verify',str(pub),'-signature',str(signature)],input=(h+'.'+b).encode(),capture_output=True)
  if r.returncode or claims.get('sub') not in (A,B,C):return None
  return claims
 except (ValueError,KeyError,TypeError):return None
