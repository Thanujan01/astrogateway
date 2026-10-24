"""Offline, deny-by-default ingestion. No network, credentials, pickle or executable data."""
from datetime import datetime,timezone
from pathlib import Path
from urllib.parse import urlparse
import hashlib,json,math,re
ALLOWED=('atmos.nmsu.edu','pds.nasa.gov','pds-geosciences.wustl.edu','trek.nasa.gov','hirise-pds.lpl.arizona.edu','www.uahirise.org','archives.esac.esa.int','www.cosmos.esa.int')
REQUIRED={'dataset_id','source_url','organization','version','retrieved_at','sha256','license','attribution','units','coordinates','uncertainty','missing_data','limitations','kind','training_permission'}
def unique_json(data):
 def pairs(items):
  obj={}
  for k,v in items:
   if k in obj:raise ValueError('Duplicate JSON key')
   obj[k]=v
  return obj
 return json.loads(data,object_pairs_hook=pairs,parse_constant=lambda _:(_ for _ in ()).throw(ValueError('Nonfinite JSON')))
def validate_manifest(m):
 if set(m)!=REQUIRED:raise ValueError('Manifest fields mismatch')
 u=urlparse(m['source_url'])
 if u.scheme!='https' or u.hostname not in ALLOWED or u.username or u.password or u.fragment:raise ValueError('Source origin not approved')
 if not re.fullmatch(r'[a-f0-9]{64}',m['sha256']):raise ValueError('Invalid checksum')
 d=datetime.fromisoformat(m['retrieved_at'])
 if d.tzinfo is None:raise ValueError('UTC-aware retrieval time required')
 if m['kind'] not in ('weather','terrain_metadata','elevation_metadata','esa_metadata'):raise ValueError('Unsupported adapter')
 if type(m['training_permission']) is not bool:raise ValueError('Explicit permission required')
 if not all(isinstance(m[k],dict) for k in ('units','coordinates','uncertainty','missing_data','license')):raise ValueError('Typed metadata required')
 if not m['license'].get('status') in ('reviewed','unverified'):raise ValueError('License status missing')
 if not m['dataset_id'] or not m['version'] or not m['attribution'] or not m['limitations']:raise ValueError('Provenance incomplete')
 return m
def ingest(path,m,max_bytes=65536,approved=()):
 validate_manifest(m);path=Path(path)
 if path.is_symlink():raise ValueError('Symlink source disallowed')
 if path.stat().st_size>max_bytes:raise ValueError('Input size limit')
 raw=path.read_bytes()
 if hashlib.sha256(raw).hexdigest()!=m['sha256']:raise ValueError('Checksum mismatch')
 if m['kind']=='weather':
  payload=unique_json(raw)
  if not isinstance(payload,list) or len(payload)>256:raise ValueError('Weather records bounded')
  normalized=[weather_row(v,m) for v in payload]
 else:
  # PDS3 label subset adapter, metadata only. No XML entities or raster decoding.
  text=raw.decode('ascii');normalized=[]
  if 'PDS_VERSION_ID' not in text:raise ValueError('Expected PDS3 label')
  fields={k:(re.search(r'^'+k+r'\s*=\s*(.+)$',text,re.M).group(1).strip().strip('"') if re.search(r'^'+k+r'\s*=\s*(.+)$',text,re.M) else None) for k in ('PRODUCT_ID','PRODUCT_VERSION_ID','TARGET_NAME','MAP_PROJECTION_TYPE','MAP_SCALE','CENTER_LATITUDE','CENTER_LONGITUDE')}
  if fields['TARGET_NAME']!='MARS':raise ValueError('Wrong target')
  normalized=[fields]
 reason=[]
 if (m['dataset_id'],m['sha256']) not in approved:reason.append('no trusted operator approval registry entry')
 if m['license']['status']!='reviewed' or not m['license'].get('evidence_url'):reason.append('license review incomplete')
 if not m['training_permission']:reason.append('training permission absent')
 if m['kind']!='weather':reason.append('metadata-only; not observations or elevation samples')
 if any(v is None for v in m['uncertainty'].values()) or not m['uncertainty']:reason.append('uncertainty unresolved')
 return {'dataset_id':m['dataset_id'],'rows':normalized,'status':'QUARANTINED' if reason else 'ELIGIBLE_SCIENCE_ONLY','reasons':reason,'provenance':m,'human_skill_training':False}
def weather_row(row,m):
 if not isinstance(row,dict) or set(row)!={'timestamp','temperature','pressure','quality','site'}:raise ValueError('Weather row schema')
 if m['units']!={'temperature':'K','pressure':'Pa'}:raise ValueError('Explicit supported SI units required')
 if row['quality'] not in ('valid','missing','rejected'):raise ValueError('Unknown quality flag')
 d=datetime.fromisoformat(row['timestamp'])
 if d.tzinfo is None:raise ValueError('Observation timezone required')
 if not row['site'] or row['site']!=m['coordinates'].get('site'):raise ValueError('Site mismatch')
 values={}
 for key in ('temperature','pressure'):
  x=row[key]
  if x is None:
   if row['quality']=='valid':raise ValueError('Valid flag with missing observation')
   values[key]=None;continue
  if type(x) not in (int,float) or not math.isfinite(x):raise ValueError('Nonfinite observation')
  # Broad integrity guards, not calibrated Jezero distribution.
  if key=='temperature' and not 100<=x<=350:raise ValueError('Temperature unit/range error')
  if key=='pressure' and not 0<x<=2000:raise ValueError('Pressure unit/range error')
  values[key]=x
 return {**row,**values,'temperature_c':None if values['temperature'] is None else round(values['temperature']-273.15,3),'usable':row['quality']=='valid'}


def pds4_weather_csv(label_path,table_path,m,columns,label_sha256,headers=0):
 """Bounded PDS4 delimited subset adapter; unsupported formats fail closed, not auto-guessed."""
 import csv,io,xml.etree.ElementTree as ET
 validate_manifest(m)
 label_path,table_path=Path(label_path),Path(table_path)
 if label_path.is_symlink() or table_path.is_symlink():raise ValueError('Symlink input')
 if label_path.stat().st_size>65536 or table_path.stat().st_size>65536:raise ValueError('Approved sample size bound')
 raw=label_path.read_bytes();table=table_path.read_bytes()
 if hashlib.sha256(raw).hexdigest()!=label_sha256 or hashlib.sha256(table).hexdigest()!=m['sha256']:raise ValueError('Bound label/table hashes')
 if b'<!DOCTYPE' in raw.upper() or b'<!ENTITY' in raw.upper():raise ValueError('DTD/entities forbidden')
 root=ET.fromstring(raw)
 vals=lambda tag:[e.text.strip() for e in root.iter() if e.tag.split('}')[-1]==tag and e.text]
 if 'Mars' not in vals('name') and 'MARS' not in vals('name'):raise ValueError('Mars target not declared')
 if table_path.name not in vals('file_name'):raise ValueError('Wrong referenced table')
 if any('/' in x or '..' in x for x in vals('file_name')):raise ValueError('File reference traversal')
 if not vals('Table_Delimited') and not any(e.tag.split('}')[-1]=='Table_Delimited' for e in root.iter()):raise ValueError('Only delimited subset supported')
 if set(columns)!={'timestamp','temperature','pressure','quality'} or any(type(v) is not int or v<0 for v in columns.values()):raise ValueError('Explicit column mappings required')
 rows=[]
 for i,parts in enumerate(csv.reader(io.StringIO(table.decode('ascii')))):
  if i<headers:continue
  if len(rows)>=256:raise ValueError('Record limit')
  try:
   values={k:parts[v] for k,v in columns.items()}
   for k in ('temperature','pressure'):values[k]=None if values[k] in m['missing_data'].get('sentinels',[]) else float(values[k])
   values['site']=m['coordinates']['site'];rows.append(weather_row(values,m))
  except (IndexError,KeyError):raise ValueError('CSV column metadata mismatch') from None
 return {'rows':rows,'label_sha256':label_sha256,'table_sha256':m['sha256'],'status':'QUARANTINED','reason':'native subset parsed; requires label/unit/column/rights operator review before science use','human_skill_training':False}
