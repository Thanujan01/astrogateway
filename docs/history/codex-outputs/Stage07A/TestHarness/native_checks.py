import asyncio,json,uuid
from pathlib import Path
from lab_common import connect,A,B,URL
tests=[]
def mark(n,ok):
 tests.append({'name':n,'status':'PASS' if ok else 'FAIL'})
 if not ok:raise AssertionError(n)
async def run():
 c=await connect();sid=str(uuid.uuid4());claim={'sub':A,'session_id':sid,'iss':URL+'/auth/v1','aud':'authenticated','role':'authenticated','exp':4102444800,'is_anonymous':False}
 async def deny(sql):
  async with c.transaction():
   await c.execute('SET LOCAL ROLE authenticated');await c.fetchval("SELECT set_config('request.jwt.claims',$1,true)",json.dumps(claim))
   try:
    async with c.transaction():await c.execute(sql)
   except Exception:return True
   return False
 for name,sql in [('client cannot create server attestation',f"INSERT INTO training_ai.attestations VALUES('{uuid.uuid4()}','{A}',true,100,'Hard','{{}}')"),('client cannot update telemetry','UPDATE training_ai.events SET payload=\'{}\''),('client cannot delete telemetry','DELETE FROM training_ai.events'),('client cannot disable AI RLS','ALTER TABLE training_ai.events DISABLE ROW LEVEL SECURITY'),('client cannot grant role','GRANT astrogateway_schema_owner TO authenticated')]:mark(name,await deny(sql))
 attacker=await connect('lab_attacker');await attacker.fetchval("SELECT set_config('request.jwt.claims',$1,false)",json.dumps(claim))
 for name,sql in [('raw attacker cannot read AI','SELECT * FROM training_ai.events'),('raw attacker cannot invoke AI','SELECT mission_api.ai_snapshot()'),('raw attacker cannot read attestations','SELECT * FROM training_ai.attestations')]:
  denied=False
  try:await attacker.execute(sql)
  except Exception:denied=True
  mark(name,denied)
 await attacker.close()
 mark('AI exposed functions invoker with fixed search_path',await c.fetchval("SELECT count(*)=2 AND bool_and(NOT prosecdef AND proconfig IS NOT NULL) FROM pg_proc p JOIN pg_namespace n ON n.oid=p.pronamespace WHERE n.nspname='mission_api' AND proname IN ('ai_ingest','ai_snapshot')"))
 for kind,extra in [('movement',{'x_m':0,'z_m':0,'speed_mps':-1}),('equipment',{'equipment':'solar_array','correct':'true'}),('completion',{'completed':True,'score':100}),('reaction',{'reaction_s':0})]:
  e={'event_id':str(uuid.uuid4()),'elapsed_s':1,'kind':kind,**extra};mark('native schema denies malformed '+kind,not await c.fetchval('SELECT training_ai.valid_event($1::jsonb)',json.dumps(e)))
 async with c.transaction():
  await c.execute('SET LOCAL ROLE authenticated');await c.fetchval("SELECT set_config('request.jwt.claims',$1,true)",json.dumps({**claim,'sub':B,'session_id':str(uuid.uuid4())}))
  mark('RLS direct B sees no A telemetry',not await c.fetchval('SELECT EXISTS(SELECT 1 FROM training_ai.events WHERE user_id=$1)',uuid.UUID(A)))
 # Private operator-created trusted fixtures test actual progression; no API runtime has these privileges.
 ids=[]
 for i in range(6):
  mid=uuid.uuid4();ids.append(mid)
  await c.execute("INSERT INTO training.mission_sessions(id,user_id,auth_session_id,launch_id,mission_id) VALUES($1,$2,$3,$4,'mission-red-horizon-poc')",mid,uuid.UUID(A),uuid.uuid4(),uuid.uuid4())
  await c.execute('INSERT INTO training_ai.attestations(mission_session_id,user_id,success,aggregate,difficulty,skill_scores,training_day) VALUES($1,$2,true,90,$3,$4::jsonb,$5)',mid,uuid.UUID(A),'Easy' if i<3 else 'Medium',json.dumps({s:90 for s in ['navigation','survival','decision_making','equipment','emergency']}),i+1)
 mark('six trusted fixtures recorded operator-only',len(ids)==6)
 await c.close()
 Path('outputs/Stage07A/Evidence/ai-native.json').write_text(json.dumps({'tests':tests,'local_only':True,'trusted_records':'operator synthetic fixtures, not real mission outcomes'},indent=2))
 print('Native AI permission checks PASS:',len(tests))
try:asyncio.run(run())
except Exception as e:
 Path('outputs/Stage07A/Evidence/ai-native-failure.json').write_text(json.dumps({'error_type':type(e).__name__,'tests':tests},indent=2));print('FAIL native AI checks.');raise SystemExit(1)
