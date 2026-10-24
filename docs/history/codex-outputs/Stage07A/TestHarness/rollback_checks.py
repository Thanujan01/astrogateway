import asyncio,json
from pathlib import Path
from lab_common import connect
checks=[]
def mark(n,ok):
 checks.append({'name':n,'status':'PASS' if ok else 'FAIL'})
 if not ok:raise AssertionError(n)
async def run():
 c=await connect();auth=await c.fetchval('SELECT count(*) FROM auth.users');missions=await c.fetchval('SELECT count(*) FROM training.mission_sessions');old=await c.fetchval('SELECT count(*) FROM training_ai.events')
 try:await c.execute(Path('outputs/Stage07A/Development/database/007a_local_adaptive_candidate.sql').read_text());collision=False
 except Exception:collision=True;await c.execute('ROLLBACK')
 mark('candidate collision fails closed without event loss',collision and await c.fetchval('SELECT count(*) FROM training_ai.events')==old)
 await c.execute("BEGIN;DROP FUNCTION mission_api.ai_ingest(uuid,jsonb);DROP FUNCTION mission_api.ai_snapshot();DROP TABLE training_ai.attestations;DROP TABLE training_ai.events;DROP FUNCTION training_ai.valid_event(jsonb);DROP SCHEMA training_ai;COMMIT;")
 mark('local AI teardown preserves Auth',await c.fetchval('SELECT count(*) FROM auth.users')==auth)
 mark('local AI teardown preserves mission records',await c.fetchval('SELECT count(*) FROM training.mission_sessions')==missions)
 await c.execute(Path('outputs/Stage07A/Development/database/007a_local_adaptive_candidate.sql').read_text())
 mark('fresh final candidate reinstalls',await c.fetchval("SELECT to_regprocedure('mission_api.ai_snapshot()') IS NOT NULL"))
 await c.execute("NOTIFY pgrst,'reload schema'");await c.close()
 Path('outputs/Stage07A/Evidence/ai-rollback.json').write_text(json.dumps({'tests':checks,'local_only':True},indent=2));print('AI local rollback checks:',len(checks),'PASS')
asyncio.run(run())
