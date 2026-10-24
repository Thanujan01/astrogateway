"""Offline verifier prototype; no Unity client keys or live attestation writer."""
import base64,hashlib,json,subprocess,tempfile,threading
from uuid import UUID
from .ingest import unique_json
FIELDS={'version','issuer','audience','key_id','result_id','player_id','auth_session_id','mission_id','launch_id','nonce','scenario_sha256','telemetry_sha256','rules','issued_at','expires_at','evidence_kind','scores'}
def canonical(record):return json.dumps(record,sort_keys=True,separators=(',',':'),allow_nan=False).encode()
class WitnessVerifier:
 def __init__(self,public_key,key_id):self.public_key=public_key;self.key_id=key_id;self.used=set();self.lock=threading.Lock()
 def verify(self,envelope,challenge,player,session,now):
  if set(envelope)!={'record','signature'}:raise ValueError('Envelope fields')
  r=envelope['record']
  if not isinstance(r,dict) or set(r)!=FIELDS or len(canonical(r))>8192:raise ValueError('Record shape')
  if r['version']!=1 or r['issuer']!='astrogateway-witness-local' or r['audience']!='training-attestation' or r['key_id']!=self.key_id or r['rules']!='07a.1':raise ValueError('Wrong authority')
  for k in ('result_id','player_id','auth_session_id','launch_id'):UUID(r[k])
  if r['player_id']!=player or r['auth_session_id']!=session:raise ValueError('Identity binding failed')
  if not all(r[k]==challenge[k] for k in ('mission_id','launch_id','nonce','scenario_sha256')):raise ValueError('Challenge binding failed')
  if r['mission_id']!='mission-red-horizon-poc' or r['evidence_kind']!='server_simulation':raise ValueError('Client assertion cannot attest')
  if type(r['issued_at']) is not int or type(r['expires_at']) is not int or not r['issued_at']<=now<r['expires_at'] or not 0<r['expires_at']-r['issued_at']<=300:raise ValueError('Expired/invalid window')
  if not all(isinstance(r[k],str) and len(r[k])==64 and all(c in '0123456789abcdef' for c in r[k]) for k in ('scenario_sha256','telemetry_sha256')):raise ValueError('Digest format')
  if set(r['scores'])!={'navigation','survival','equipment','decision_making','emergency'} or any(type(v) not in (int,float) or not 0<=v<=100 for v in r['scores'].values()):raise ValueError('Score schema')
  try:sig=base64.b64decode(envelope['signature'],validate=True)
  except Exception:raise ValueError('Invalid detached signature') from None
  if len(sig)!=256:raise ValueError('Expected pinned RSA2048 signature')
  with tempfile.TemporaryDirectory() as temp:
   from pathlib import Path
   p=Path(temp)/'signature';p.write_bytes(sig)
   result=subprocess.run(['/usr/bin/openssl','dgst','-sha256','-verify',str(self.public_key),'-signature',str(p)],input=canonical(r),capture_output=True,timeout=5)
  if result.returncode:raise ValueError('Signature rejected')
  # Atomic in-process prototype. Durable DB consume + attestation transaction is future design.
  with self.lock:
   key=(r['player_id'],r['nonce']);rid=r['result_id']
   if key in self.used or rid in self.used:raise ValueError('Replay rejected')
   self.used.update((key,rid))
  return r
