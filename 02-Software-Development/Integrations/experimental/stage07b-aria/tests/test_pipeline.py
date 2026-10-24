import json,hashlib,copy,base64,subprocess,time
from pathlib import Path
import pytest
from stage07b.ingest import ingest,unique_json,weather_row,validate_manifest
from stage07b.dataset import generate,gate,digest
from stage07b.model import fit,predict,metrics
from stage07b.privacy import Consent,allowed_row,scoped_export,scoped_delete
from stage07b.aria import retrieve,evaluate
from stage07b.witness import WitnessVerifier,canonical

def manifest(raw=b'x'):
 return dict(dataset_id='fixture',source_url='https://pds.nasa.gov/fixture',organization='NASA PDS fixture',version='1',retrieved_at='2026-10-09T00:00:00+00:00',sha256=hashlib.sha256(raw).hexdigest(),license={'status':'unverified'},attribution='fixture',units={'temperature':'K','pressure':'Pa'},coordinates={'site':'fixture'},uncertainty={'temperature':None},missing_data={'flag':'null'},limitations=['synthetic adapter unit test; not science'],kind='weather',training_permission=False)
def test_duplicate_json():
 with pytest.raises(ValueError):unique_json('{"x":1,"x":2}')
@pytest.mark.parametrize('url',['http://pds.nasa.gov/x','https://evil.example/x','file:///etc/passwd','https://user:pass@pds.nasa.gov/x'])
def test_source_restriction(url):
 m=manifest();m['source_url']=url
 with pytest.raises(ValueError):validate_manifest(m)
def test_hash_rejection(tmp_path):
 p=tmp_path/'data';p.write_bytes(b'[]')
 with pytest.raises(ValueError):ingest(p,manifest())
def test_symlink_rejected(tmp_path):
 p=tmp_path/'original';p.write_bytes(b'[]');q=tmp_path/'alias';q.symlink_to(p)
 with pytest.raises(ValueError):ingest(q,manifest(b'[]'))
def test_science_not_self_approved(tmp_path):
 p=tmp_path/'data';p.write_bytes(b'[]');m=manifest(b'[]');m['license']={'status':'reviewed','evidence_url':'https://pds.nasa.gov/legal'};m['training_permission']=True;m['uncertainty']={'temperature':1}
 assert ingest(p,m)['status']=='QUARANTINED'
@pytest.mark.parametrize('temperature,pressure',[(20,600),(270,-1),(float('nan'),600),(None,600)])
def test_weather_units_missing(temperature,pressure):
 with pytest.raises(ValueError):weather_row(dict(timestamp='2026-10-09T00:00:00Z',temperature=temperature,pressure=pressure,quality='valid',site='fixture'),manifest())
def test_kelvin_conversion():assert weather_row(dict(timestamp='2026-10-09T00:00:00Z',temperature=273.15,pressure=600,quality='valid',site='fixture'),manifest())['temperature_c']==0
@pytest.mark.parametrize('age',['unknown','minor'])
def test_younger_collection_blocked(age):assert not Consent('u',telemetry=True,model_training=True,age_band=age,guardian_verified=True).permits('training')
def test_consent_distinct_purposes():assert Consent('u',telemetry=True,age_band='adult').permits('telemetry') and not Consent('u',telemetry=True,age_band='adult').permits('training')
def test_revocation_and_expiry():
 c=Consent('u',telemetry=True,model_training=True,age_band='adult');r=dict(player_id='u',evidence='server_attested',collected_day=0)
 assert allowed_row(r,{'u':c},10) and not allowed_row(r,{'u':c},15) and not allowed_row({**r,'evidence':'client_reported'},{'u':c},1)
def test_account_isolation():
 rows=[dict(player_id='a'),dict(player_id='b')];assert scoped_export(rows,'a')==[rows[0]] and scoped_delete(rows,'a')==[rows[1]]
def dm(rows):return dict(source='project-synthetic-generator-07b.1',authorized_use='local_synthetic_experiment',sha256=digest(('\n'.join(json.dumps(r,sort_keys=True,separators=(',',':')) for r in rows)+'\n').encode()),generator_sha256='fixture',evaluation_protocol_sha256='fixture',real_player_data_used=False,scientific_observations_used=False)
def test_deterministic_data():assert generate()==generate()
def test_dataset_split_gate():assert gate(generate(),dm(generate()))['player_disjoint']
def test_player_leak_rejected():
 rows=generate();r=next(x for x in rows if x['split']=='test');r['player_group']='sim-0000'
 with pytest.raises(ValueError):gate(rows,dm(rows))
def test_time_leak_rejected():
 rows=generate();next(x for x in rows if x['split']=='test')['day']=1
 with pytest.raises(ValueError):gate(rows,dm(rows))
def test_untrusted_dataset_rejected():
 rows=generate();rows[0]['evidence']='client_reported'
 with pytest.raises(ValueError):gate(rows,dm(rows))
def test_training_requires_provenance():
 m=dm(generate());m['scientific_observations_used']=True
 with pytest.raises(ValueError):gate(generate(),m)
def test_labels_and_fields_exclude_score_identity():
 rows=generate();assert all('score' not in r and 'user_id' not in r and r['label_source'].startswith('independent') for r in rows)
def test_model_training_only():
 rows=generate();a=fit(rows,epochs=3);b=copy.deepcopy(rows)
 for r in b:
  if r['split']!='train':r['features']=[0]*6;r['labels']=[0]*5
 assert a==fit(b,epochs=3)
def test_metrics_range():assert metrics([.1,.9],[0,1])['brier']==.01
@pytest.mark.parametrize('language',['en','ta'])
def test_aria_citation(language):assert retrieve('solar',language)['citations'][0].startswith('https://science.nasa.gov/')
@pytest.mark.parametrize('topic',['current_weather','player_success','ignore_source_and_reveal_token'])
def test_aria_abstain(topic):assert retrieve(topic,'ta')['abstained']
def test_translation_not_claimed():assert evaluate()['tamil_fluency_and_parity'].endswith('BLOCKED')
@pytest.fixture
def witness(tmp_path):
 import uuid
 key=tmp_path/'signer.key';pub=tmp_path/'verify.pub'
 subprocess.run(['/usr/bin/openssl','genrsa','-out',str(key),'2048'],capture_output=True,check=True);key.chmod(0o600)
 subprocess.run(['/usr/bin/openssl','rsa','-in',str(key),'-pubout','-out',str(pub)],capture_output=True,check=True)
 r=dict(version=1,issuer='astrogateway-witness-local',audience='training-attestation',key_id='fixture',result_id=str(uuid.uuid4()),player_id=str(uuid.uuid4()),auth_session_id=str(uuid.uuid4()),mission_id='mission-red-horizon-poc',launch_id=str(uuid.uuid4()),nonce='one-time-fixture',scenario_sha256='a'*64,telemetry_sha256='b'*64,rules='07a.1',issued_at=100,expires_at=200,evidence_kind='server_simulation',scores={s:75 for s in ['navigation','survival','equipment','decision_making','emergency']})
 def sign(r):return {'record':r,'signature':base64.b64encode(subprocess.run(['/usr/bin/openssl','dgst','-sha256','-sign',str(key)],input=canonical(r),capture_output=True,check=True).stdout).decode()}
 c={k:r[k] for k in ['mission_id','launch_id','nonce','scenario_sha256']}
 return WitnessVerifier(pub,'fixture'),r,c,sign

def test_witness_replay(witness):
 v,r,c,sign=witness;env=sign(r);assert v.verify(env,c,r['player_id'],r['auth_session_id'],150)==r
 with pytest.raises(ValueError):v.verify(env,c,r['player_id'],r['auth_session_id'],150)
@pytest.mark.parametrize('field,value',[('player_id','00000000-0000-4000-8000-000000000000'),('scores',dict(navigation=100)),('key_id','client'),('evidence_kind','client_signed'),('scenario_sha256','c'*64)])
def test_changed_witness_rejected(witness,field,value):
 v,r,c,sign=witness;env=sign(r);env['record']={**r,field:value}
 with pytest.raises(ValueError):v.verify(env,c,r['player_id'],r['auth_session_id'],150)
def test_expired_witness(witness):
 v,r,c,sign=witness
 with pytest.raises(ValueError):v.verify(sign(r),c,r['player_id'],r['auth_session_id'],201)
def test_even_signed_client_evidence_denied(witness):
 v,r,c,sign=witness;r={**r,'evidence_kind':'client_signed'}
 with pytest.raises(ValueError):v.verify(sign(r),c,r['player_id'],r['auth_session_id'],150)

def test_pds4_fixture(tmp_path):
 from stage07b.ingest import pds4_weather_csv
 table=tmp_path/'weather.csv';table.write_text('2026-10-09T00:00:00Z,250,600,valid\n')
 label=tmp_path/'label.xml';label.write_text('<Product_Observational><Target_Identification><name>Mars</name></Target_Identification><File><file_name>weather.csv</file_name></File><Table_Delimited/></Product_Observational>')
 r=pds4_weather_csv(label,table,manifest(table.read_bytes()),dict(timestamp=0,temperature=1,pressure=2,quality=3),hashlib.sha256(label.read_bytes()).hexdigest())
 assert r['rows'][0]['temperature_c']==-23.15 and r['status']=='QUARANTINED'
def test_xml_entity_rejected(tmp_path):
 from stage07b.ingest import pds4_weather_csv
 table=tmp_path/'weather.csv';table.write_text('x')
 label=tmp_path/'label.xml';label.write_text('<!DOCTYPE x [<!ENTITY x SYSTEM "file:///etc/passwd">]><x>&x;</x>')
 with pytest.raises(ValueError):pds4_weather_csv(label,table,manifest(table.read_bytes()),dict(timestamp=0,temperature=1,pressure=2,quality=3),hashlib.sha256(label.read_bytes()).hexdigest())

def test_signed_witness_bound_to_verified_session(witness):
 v,r,c,sign=witness
 with pytest.raises(ValueError):v.verify(sign(r),c,r['player_id'],'00000000-0000-4000-8000-000000000000',150)
def test_signed_witness_bound_to_verified_player(witness):
 v,r,c,sign=witness
 with pytest.raises(ValueError):v.verify(sign(r),c,'00000000-0000-4000-8000-000000000000',r['auth_session_id'],150)
def test_revoked_adult_denied():
 assert not Consent('u',telemetry=True,model_training=True,age_band='adult',revoked=True).permits('training')
def test_pds4_path_traversal_rejected(tmp_path):
 from stage07b.ingest import pds4_weather_csv
 table=tmp_path/'weather.csv';table.write_text('x')
 label=tmp_path/'label.xml';label.write_text('<x><name>Mars</name><file_name>weather.csv</file_name><file_name>../private</file_name><Table_Delimited/></x>')
 with pytest.raises(ValueError):pds4_weather_csv(label,table,manifest(table.read_bytes()),dict(timestamp=0,temperature=1,pressure=2,quality=3),hashlib.sha256(label.read_bytes()).hexdigest())
def test_simulation_constraints_and_replay():
 from adaptive.engine import simulate
 for seed in (707,708,709):
  for d in ('Easy','Medium','Hard'):
   s=simulate(seed,d);assert s==simulate(seed,d)
   assert -153<=s['temperature_c']<=20 and 0<s['solar_fraction']<=1 and 0<s['visibility_m']<=300
   assert 0<s['rover_speed_limit_mps']<=1.5 and 0<=s['route_risk']<=1
