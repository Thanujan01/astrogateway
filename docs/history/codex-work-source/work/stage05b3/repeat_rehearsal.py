import asyncio,json,uuid
from pathlib import Path
import asyncpg
from lab_common import connect,A,B,C
SQL=Path('outputs/Stage05B2/SQL');OUT=Path('outputs/Stage05B3/Evidence');tests=[]
def mark(name,value):
 tests.append({'name':name,'status':'PASS' if value else 'FAIL'});print('PASS' if value else 'FAIL',name)
 if not value:raise AssertionError(name)
async def main():
 c=await connect()
 for n in ('001_verified_jwt_training.sql',):
  collision=False
  try:await c.execute((SQL/n).read_text())
  except asyncpg.DuplicateObjectError:collision=True;await c.execute('ROLLBACK')
  mark('fresh migration refuses installed-object collision without overwrite',collision)
 count=await c.fetchval('SELECT count(*) FROM auth.users')
 await c.execute((SQL/'910_local_down_only.sql').read_text());mark('local dependency-ordered teardown preserves Auth rows',await c.fetchval('SELECT count(*) FROM auth.users')==count)
 mark('no application schema/owner left after local down',not await c.fetchval("SELECT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname IN ('training','mission_api')) OR EXISTS(SELECT 1 FROM pg_roles WHERE rolname='astrogateway_schema_owner')"))
 # Late trigger collision must roll back all preceding DDL and role/default creation.
 await c.execute("CREATE FUNCTION auth.fixture_collision() RETURNS trigger LANGUAGE plpgsql AS $$BEGIN RETURN NEW;END$$;CREATE TRIGGER astrogateway_auth_user_profile AFTER INSERT ON auth.users FOR EACH ROW EXECUTE FUNCTION auth.fixture_collision();")
 failed=False
 try:await c.execute((SQL/'001_verified_jwt_training.sql').read_text())
 except asyncpg.DuplicateObjectError:failed=True;await c.execute('ROLLBACK')
 mark('late trigger collision rolls back tables/schema/owner atomically',failed and not await c.fetchval("SELECT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname='training') OR EXISTS(SELECT 1 FROM pg_roles WHERE rolname='astrogateway_schema_owner')"))
 await c.execute('DROP TRIGGER astrogateway_auth_user_profile ON auth.users;DROP FUNCTION auth.fixture_collision();')
 await c.execute('ALTER TABLE auth.users ALTER COLUMN created_at DROP NOT NULL')
 null_id=uuid.UUID('eeeeeeee-eeee-4eee-8eee-eeeeeeeeeeee');await c.execute("INSERT INTO auth.users(id,created_at,raw_user_meta_data) VALUES($1,NULL,'{}')",null_id)
 failed=False
 try:await c.execute((SQL/'001_verified_jwt_training.sql').read_text())
 except asyncpg.NotNullViolationError:failed=True;await c.execute('ROLLBACK')
 mark('null backfill late failure rolls back new role/schema/trigger',failed and not await c.fetchval("SELECT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname='training') OR EXISTS(SELECT 1 FROM pg_roles WHERE rolname='astrogateway_schema_owner')"))
 await c.execute('DELETE FROM auth.users WHERE id=$1',null_id);await c.execute('ALTER TABLE auth.users ALTER COLUMN created_at SET NOT NULL')
 await c.execute((SQL/'001_verified_jwt_training.sql').read_text());await c.execute((SQL/'002_retire_direct_runtime.sql').read_text());await c.execute((SQL/'003_read_only_verification.sql').read_text())
 mark('clean candidate install/re-quarantine after local rollback succeeds',await c.fetchval('SELECT count(*) FROM training.profiles')==count)
 async with c.transaction():
  await c.execute('SET LOCAL ROLE supabase_auth_admin');await c.execute("INSERT INTO auth.users(id,raw_user_meta_data,email_confirmed_at) VALUES('dddddddd-dddd-4ddd-8ddd-dddddddddddd','{\"training_level\":10}',now()) ON CONFLICT(id) DO NOTHING")
 mark('fixture Auth administrator trigger still creates bounded default trainee',await c.fetchval("SELECT training_level=0 AND trainee_profile='astronaut_candidate' FROM training.profiles WHERE user_id='dddddddd-dddd-4ddd-8ddd-dddddddddddd'"))
 # Runtime quarantine is retained across rollback; unsafe old identity path is never restored.
 mark('quarantine remains NOLOGIN with no memberships',await c.fetchval("SELECT NOT rolcanlogin AND NOT EXISTS(SELECT 1 FROM pg_auth_members WHERE member=pg_roles.oid) FROM pg_roles WHERE rolname='astrogateway_runtime'"))
 await c.execute("NOTIFY pgrst,'reload schema'");await c.close()
 (OUT/'repeat-rehearsal.json').write_text(json.dumps({'localhost_only':True,'auth_users_retained':True,'tests':tests},indent=2))
try:asyncio.run(main())
except Exception as e:
 (OUT/'repeat-rehearsal-failure.json').write_text(json.dumps({'failure_type':type(e).__name__,'tests':tests},indent=2));print('FAIL local rehearsal; sanitized failure recorded.');raise SystemExit(1)
