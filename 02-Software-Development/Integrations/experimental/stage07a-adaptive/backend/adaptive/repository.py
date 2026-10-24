"""Original user bearer only; no memory/SQL fallback. Requires separately reviewed AI RPCs."""
from fastapi import HTTPException
class AIRepository:
 def __init__(self,http,settings):self.http,self.settings=http,settings
 async def rpc(self,user,name,body):
  if name not in ('ai_ingest','ai_snapshot'):raise RuntimeError('Unapproved AI RPC')
  try:r=await self.http.post(self.settings.supabase_url+'/rest/v1/rpc/'+name,headers={'Authorization':'Bearer '+user.access_token,'apikey':self.settings.publishable_key,'Accept-Profile':'mission_api','Content-Profile':'mission_api'},json=body)
  except Exception:raise HTTPException(503,'Adaptive data service unavailable.') from None
  if len(r.content)>2_000_000:raise HTTPException(502,'Adaptive data response too large.')
  if r.status_code!=200:
   if r.status_code in (401,403):raise HTTPException(r.status_code,'Adaptive authorization failed.')
   try:code=r.json().get('code')
   except Exception:code=None
   if code=='PT404':raise HTTPException(404,'Mission unavailable.')
   if code in ('PT409','23505'):raise HTTPException(409,'Telemetry conflict.')
   if code in ('PT422','23514'):raise HTTPException(422,'Telemetry rejected.')
   raise HTTPException(503,'Adaptive RPCs require approved configuration.')
  try:return r.json()
  except ValueError:raise HTTPException(502,'Invalid adaptive response.') from None
 async def snapshot(self,user):
  value=await self.rpc(user,'ai_snapshot',{})
  if not isinstance(value,dict) or set(value)!={'missions','events','attestations'} or not all(isinstance(value[k],list) for k in value):raise HTTPException(502,'Invalid adaptive snapshot.')
  if len(value['events'])>2048 or len(value['missions'])>100:raise HTTPException(502,'Adaptive snapshot limit.')
  return value
 async def ingest(self,user,batch):return await self.rpc(user,'ai_ingest',{'p_mission':str(batch.mission_session_id),'p_events':[e.model_dump(mode='json') for e in batch.events]})
