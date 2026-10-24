"""Local policy prototype; no real users, UI or remote collection."""
from dataclasses import dataclass
@dataclass(frozen=True)
class Consent:
 player_id:str
 version:str='07b.1'
 telemetry:bool=False
 model_training:bool=False
 age_band:str='unknown'
 guardian_verified:bool=False
 revoked:bool=False
 def permits(self,purpose):
  if self.revoked or self.age_band not in ('adult','minor'):return False
  if self.age_band=='minor':return False # Under18 research enrollment blocked in this preparation stage.
  return self.telemetry if purpose=='telemetry' else self.telemetry and self.model_training if purpose=='training' else False
RAW_TTL_DAYS=14
DERIVED_TTL_DAYS=90
BACKUP_MAX_DAYS=30
def allowed_row(row,consents,now_day):
 if row.get('evidence')=='synthetic':return True
 c=consents.get(row.get('player_id'))
 return bool(c and c.permits('training') and row.get('evidence')=='server_attested' and 0<=now_day-row['collected_day']<=RAW_TTL_DAYS)
def scoped_export(records,verified_id):return [dict(r) for r in records if r['player_id']==verified_id]
def scoped_delete(records,verified_id):return [r for r in records if r['player_id']!=verified_id]
