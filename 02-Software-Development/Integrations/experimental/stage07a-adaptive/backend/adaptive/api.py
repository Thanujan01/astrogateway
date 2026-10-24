from fastapi import APIRouter,Depends,HTTPException,Request
from starlette.responses import JSONResponse
from pydantic import TypeAdapter,ValidationError
from .schemas import Batch,Recommendation,Debrief,TelemetryEvent
from .engine import assess,recommend,plan,debrief,validate_events,SKILLS
from .repository import AIRepository
from collections import defaultdict
import hashlib
EVENT=TypeAdapter(TelemetryEvent)
def decoded(snapshot):
 groups=defaultdict(list)
 try:
  for row in snapshot['events']:groups[row['mission_session_id']].append(EVENT.validate_python(row['payload']))
  for values in groups.values():
   values.sort(key=lambda e:e.elapsed_s);validate_events(values)
 except (ValidationError,ValueError,KeyError,TypeError):raise HTTPException(502,'Stored telemetry failed validation.') from None
 return groups
def summarize(snapshot):
 groups=decoded(snapshot);skills={};reports=[assess(v) for v in groups.values()]
 # Trusted scores are never derived from uploaded client evidence. Only private server attestation records.
 trusted=snapshot['attestations']
 for skill in SKILLS:
  valid=[r[skill]['score'] for r in reports if r[skill]['score'] is not None]
  authoritative=[h['skill_scores'][skill] for h in trusted if skill in h.get('skill_scores',{})]
  values=authoritative or valid;n=len(values)
  skills[skill]={'score':round(sum(values)/n,2) if n else None,'observations':n,'confidence':round(min(.9,n/10)*(1 if authoritative else .25),3),'status':'insufficient_data' if n<3 else 'assessed' if authoritative else 'provisional','basis':'server_attested' if authoritative else 'client_reported'}
 return skills

def history(snapshot):
 trusted={h['mission_session_id']:h for h in snapshot['attestations']}
 groups=decoded(snapshot);rows=[]
 for mission in snapshot['missions']:
  mid=mission['id'];h=trusted.get(mid);events=groups.get(mid,[]);last=next((e for e in reversed(events) if e.kind=='completion'),None)
  if h:rows.append({'attested':True,'success':h['success'],'aggregate':h['aggregate'],'difficulty':h['difficulty']})
  elif last:rows.append({'attested':False,'success':last.completed,'aggregate':None})
 return rows

def mount(app,authorized_user):
 @app.middleware('http')
 async def bound_ai_body(request,call_next):
  if request.url.path.startswith('/ai/') and request.method=='POST':
   size=0;chunks=[]
   async for chunk in request.stream():
    size+=len(chunk)
    if size>131072:return JSONResponse({'detail':'Adaptive request too large.'},status_code=413)
    chunks.append(chunk)
   request._body=b''.join(chunks)
  return await call_next(request)
 router=APIRouter(prefix='/ai',tags=['adaptive-foundation'])
 def repo(request:Request):
  if request.app.state.repository is None:raise HTTPException(503,'Adaptive configuration required.')
  return AIRepository(request.app.state.repository.http,request.app.state.repository.settings)
 @router.post('/telemetry')
 async def telemetry(batch:Batch,user=Depends(authorized_user),store=Depends(repo)):
  snapshot=await store.snapshot(user)
  existing=decoded(snapshot).get(str(batch.mission_session_id),[])
  seen={e.event_id:e for e in existing}
  additions=[e for e in batch.events if e.event_id not in seen]
  try:
   validate_events(existing+additions)
  except ValueError:raise HTTPException(422,'Telemetry consistency check failed.') from None
  return await store.ingest(user,batch)
 @router.get('/skills/me')
 async def skills(user=Depends(authorized_user),store=Depends(repo)):
  snap=await store.snapshot(user);return {'rules':'07a.1','skills':summarize(snap),'client_data_is_provisional':True}
 @router.post('/mission/recommend')
 async def mission(payload:Recommendation,user=Depends(authorized_user),store=Depends(repo)):
  snap=await store.snapshot(user);h=history(snap)
  # Replay attested outcomes from Easy; user-submitted previous is never authority.
  previous='Easy'
  trusted=[x for x in h if x['attested']]
  for n in range(1,len(trusted)+1):previous=recommend(trusted[:n],previous)['difficulty']
  return {'difficulty':previous,'reason':'Replayed server-attested history; requested previous ignored','eligible_promotions':any(x['attested'] for x in h),'rules':'07a.1'}
 @router.get('/training-plan/me')
 async def training(user=Depends(authorized_user),store=Depends(repo)):
  snap=await store.snapshot(user);h=history(snap)
  completed=defaultdict(set)
  for a in snap['attestations']:
   if a['success'] and a.get('training_day') is not None:completed[a.get('cycle',0)].add(a['training_day'])
  cycle=0
  while len(completed[cycle])==30:cycle+=1
  seed=int.from_bytes(hashlib.sha256(str(user.user_id).encode()).digest()[:4],'big')
  result=plan(summarize(snap),seed,cycle,h);result['completed_days']=len(completed[cycle])
  for day in result['days']:
   if day['day'] in completed[cycle]:day['state']='completed'
  result['provisional_completed_missions']=sum(not x['attested'] and x['success'] for x in h)
  return result
 @router.post('/debrief')
 async def feedback(payload:Debrief,user=Depends(authorized_user),store=Depends(repo)):
  snap=await store.snapshot(user)
  if str(payload.mission_session_id) not in {m['id'] for m in snap['missions']}:raise HTTPException(404,'Mission unavailable.')
  scoped={**snap,'events':[e for e in snap['events'] if e['mission_session_id']==str(payload.mission_session_id)],'attestations':[h for h in snap['attestations'] if h['mission_session_id']==str(payload.mission_session_id)]}
  return {**debrief(summarize(scoped)),'mission_session_id':str(payload.mission_session_id),'requested_focus':payload.question}
 app.include_router(router)
