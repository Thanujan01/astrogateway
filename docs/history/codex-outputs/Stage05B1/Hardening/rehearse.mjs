import {PGlite} from '/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/work/stage05b1-hardening/pg17/node_modules/@electric-sql/pglite/dist/index.js';
import {readFileSync,writeFileSync} from 'node:fs';
const dir='outputs/Stage05B1/Hardening/SQL/';const tests=[];const issues=[];
const A='11111111-1111-4111-8111-111111111111', B='22222222-2222-4222-8222-222222222222', SA='aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa',SB='bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbbb';
const db=new PGlite();
function mark(name,ok,kind='PASS'){tests.push({name,status:ok?kind:'FAIL'});console.log(ok?kind:'FAIL',name);if(!ok)throw Error(name);}
const execfile=name=>db.exec(readFileSync(dir+name,'utf8'));
async function runtime(task,uid=null,sid=null){
 await db.exec('SET SESSION AUTHORIZATION astrogateway_runtime; BEGIN; SET LOCAL ROLE astrogateway_api;');
 try {if(uid)await db.query("SELECT set_config('astrogateway.user_id',$1,true),set_config('astrogateway.auth_session_id',$2,true)",[uid,sid]);const r=await task();await db.exec('COMMIT');return r;}
 catch(e){await db.exec('ROLLBACK');throw e;}finally{await db.exec('SET SESSION AUTHORIZATION postgres; RESET ROLE');}
}
async function denied(name,sql,args=[],uid=A,sid=SA){let yes=false;try{await runtime(()=>db.query(sql,args),uid,sid);}catch(e){yes=['42501'].includes(e.code);}mark(name,yes);}
async function snapshot(){return (await db.query("SELECT nspname,nspacl::text FROM pg_namespace WHERE nspname IN ('public','auth','storage','pgcustom') ORDER BY 1")).rows;}
try{
 const version=(await db.query('SELECT version()')).rows[0].version;
 await execfile('900_local_fixture_only.sql');
 await execfile('000_read_only_preflight.sql');mark('read-only preflight executes against fixture',true);
 const beforeSchemas=await snapshot();const beforeDb=(await db.query("SELECT datacl::text FROM pg_database WHERE datname=current_database()")).rows[0].datacl;
 await db.exec(`INSERT INTO auth.users VALUES('${A}',now(),'{"display_name":"A trainee","training_level":10}'),('${B}',now(),'{"display_name":"B trainee"}')`);
 await execfile('001_identity_and_mission_sessions.sql');mark('baseline migration and Auth backfill',true);
 let guardDenied=false;try{await execfile('003_runtime_role.sql')}catch(e){guardDenied=e.message.includes('Unresolved PUBLIC schema');await db.exec('ROLLBACK');}
 mark('runtime provisioning refuses unresolved PUBLIC access',guardDenied);
 await execfile('002_public_acl_isolation.sql');await execfile('003_runtime_role.sql');mark('ACL hardening and restricted-role provisioning',true);
 await execfile('004_post_migration_metadata.sql');mark('post-migration metadata SQL executes',true);
 let r=await db.query("SELECT rolsuper,rolbypassrls,rolcreatedb,rolcreaterole,rolreplication,rolinherit FROM pg_roles WHERE rolname='astrogateway_runtime'");mark('runtime has no administrative or bypass attributes',Object.values(r.rows[0]).every(x=>x===false));
 r=await db.query("SELECT m.admin_option,m.inherit_option,m.set_option,p.rolname FROM pg_auth_members m JOIN pg_roles p ON p.oid=m.roleid WHERE member=(SELECT oid FROM pg_roles WHERE rolname='astrogateway_runtime')");mark('membership is API only: SET true ADMIN/INHERIT false',r.rows.length===1 && r.rows[0].rolname==='astrogateway_api'&&!r.rows[0].admin_option&&!r.rows[0].inherit_option&&r.rows[0].set_option);
 await db.exec('BEGIN; SET LOCAL ROLE supabase_auth_admin;');
 await db.exec("INSERT INTO auth.users VALUES('33333333-3333-4333-8333-333333333333',now(),'{\"display_name\":\"New trainee\",\"training_level\":10}')");await db.exec('COMMIT');
 r=await db.query('SELECT count(*)::int n FROM training.profiles WHERE training_level=0');mark('preserved fixture Auth insert trigger and default training level',r.rows[0].n===3);
 for (const [uid,sid,label,other] of [[A,SA,'A',B],[B,SB,'B',A]]){
 r=await runtime(()=>db.query('SELECT user_id FROM training.profiles'),uid,sid);mark(label+' sees only own profile',r.rows.length===1&&r.rows[0].user_id===uid);
 r=await runtime(()=>db.query('SELECT * FROM training.profiles WHERE user_id=$1',[other]),uid,sid);mark(label+' cannot read other profile',r.rows.length===0);
 await denied(label+' cannot insert another owner',"INSERT INTO training.mission_sessions(user_id,auth_session_id,launch_id,mission_id) VALUES($1,$2,gen_random_uuid(),'mission-red-horizon-poc')",[other,sid],uid,sid);
 }
 const own=await runtime(()=>db.query("INSERT INTO training.mission_sessions(user_id,auth_session_id,launch_id,mission_id) VALUES($1,$2,gen_random_uuid(),'mission-red-horizon-poc') RETURNING id",[A,SA]),A,SA);const id=own.rows[0].id;mark('own mission insert permitted',!!id);
 r=await runtime(()=>db.query('SELECT * FROM training.mission_sessions WHERE id=$1',[id]),B,SB);mark('B cannot read A mission',r.rows.length===0);
 r=await runtime(()=>db.query("UPDATE training.mission_sessions SET status='integration_check_complete',completed_at=now() WHERE id=$1 RETURNING id",[id]),B,SB);mark('B cannot complete A mission',r.rows.length===0);
 r=await runtime(()=>db.query("UPDATE training.mission_sessions SET status='integration_check_complete',completed_at=now() WHERE id=$1 RETURNING id",[id]),A,SA);mark('A completion allowed',r.rows.length===1);
 for(const [name,sql] of [
 ['profile escalation',"UPDATE training.profiles SET training_level=10"],['profile insertion',`INSERT INTO training.profiles VALUES('${A}','Override','astronaut_candidate',0,now())`],
 ['delete missions','DELETE FROM training.mission_sessions'],['truncate profiles','TRUNCATE training.profiles'],['lease extension',"UPDATE training.mission_sessions SET expires_at=now()+interval '1 year'"],
 ['Auth administration','SELECT * FROM auth.users'],['unrelated table','SELECT * FROM public.unrelated'],['lookalike pg-prefix schema','SELECT * FROM pgcustom.unrelated'],['unrelated definer routine','SELECT public.fixture_read()'],
 ['storage schema','CREATE TABLE storage.bad(id int)'],['training schema CREATE','CREATE TABLE training.bad(id int)'],['temporary object','CREATE TEMP TABLE bad(id int)'],['role creation','CREATE ROLE bad'],['grant API role','GRANT astrogateway_api TO anon'],['change owner column',"UPDATE training.mission_sessions SET user_id='22222222-2222-4222-8222-222222222222'"],
 ['SET provider admin','SET ROLE supabase_auth_admin'],['SET postgres','SET ROLE postgres']])await denied('denied '+name,sql);
 for(const role of ['anon','authenticated']){await db.exec(`BEGIN;SET LOCAL ROLE ${role}`);let rejected=false;try{await db.query('SELECT * FROM training.profiles')}catch(e){rejected=e.code==='42501'}finally{await db.exec('ROLLBACK')}mark(role+' cannot access private training',rejected);}
 await db.exec("CREATE FUNCTION training.future_private() RETURNS int LANGUAGE sql AS 'SELECT 1'; CREATE TABLE training.future_private_table(id int);");
 r=await db.query("SELECT has_function_privilege('astrogateway_api','training.future_private()','EXECUTE') x,has_table_privilege('astrogateway_api','training.future_private_table','SELECT') y");mark('new owner objects do not auto-grant API execution/read',!r.rows[0].x&&!r.rows[0].y);
 await db.exec('DROP FUNCTION training.future_private();DROP TABLE training.future_private_table;');
 r=await runtime(()=>db.query('SELECT * FROM training.profiles'));mark('no context after prior commits yields no rows',r.rows.length===0);
 await denied('intentional error rollback',"UPDATE training.profiles SET training_level=10");r=await runtime(()=>db.query('SELECT * FROM training.profiles'));mark('identity cleared after rollback',r.rows.length===0);
 // Explicit compromise test: session identity is still caller-controlled inside DB.
 r=await runtime(async()=>{await db.query("SELECT set_config('astrogateway.user_id',$1,true),set_config('astrogateway.auth_session_id',$2,true)",[B,SB]);return db.query('SELECT user_id FROM training.profiles');},A,SA);
 const impersonated=r.rows.length===1&&r.rows[0].user_id===B;
 tests.push({name:'compromised runtime cannot impersonate B',status:impersonated?'FAIL':'PASS',expected_negative_demonstration:true});issues.push('BLOCKING: direct runtime SQL changed identity settings and read B without B JWT.');console.log('FAIL compromised runtime cannot impersonate B (demonstrated blocking boundary)');
 await runtime(()=>db.query('INSERT INTO training.auth_session_revocations(auth_session_id,user_id,expires_at) VALUES($1,$2,now()+interval \'1 hour\')',[SA,A]),A,SA);
 r=await runtime(()=>db.query('SELECT * FROM training.profiles'),A,SA);mark('revoked session blocked',r.rows.length===0);
 r=await runtime(()=>db.query('SELECT * FROM training.profiles'),A,SB);mark('other session still permitted',r.rows.length===1);
 // Harden ACL SQL is repeatable. Baseline/provision scripts intentionally reject collisions.
 await execfile('002_public_acl_isolation.sql');mark('ACL hardening repeat execution',true);
 for(const name of ['001_identity_and_mission_sessions.sql','003_runtime_role.sql']){let collision=false;try{await execfile(name)}catch(e){collision=e.code==='42710';await db.exec('ROLLBACK')}mark(name+' collision fails transaction without overwrite',collision);}
 mark('existing profiles survive rejected repeat',(await db.query('SELECT count(*)::int n FROM training.profiles')).rows[0].n===3);
 await execfile('910_local_down_only.sql');mark('dependency-ordered local down migration',true);
 mark('Auth fixture users preserved after local down',(await db.query('SELECT count(*)::int n FROM auth.users')).rows[0].n===3);
 // Restore exact ACL snapshots using GRANT/REVOKE, not system catalogue writes.
 const names=(await db.query("SELECT rolname FROM pg_roles WHERE left(rolname,3) <> 'pg_'")).rows.map(r=>r.rolname);
 const qi=s=>'"'+s.replaceAll('"','""')+'"';
 for(const schema of beforeSchemas){
  await db.exec(`REVOKE ALL ON SCHEMA ${qi(schema.nspname)} FROM PUBLIC,${names.map(qi).join(',')};`);
  // Parse original ACL through PostgreSQL, preserving owner + PUBLIC grant options.
  const acl=await db.query("SELECT CASE WHEN a.grantee=0 THEN 'PUBLIC' ELSE pg_get_userbyid(a.grantee) END grantee,a.privilege_type,a.is_grantable FROM aclexplode(COALESCE($1::aclitem[],acldefault('n',(SELECT nspowner FROM pg_namespace WHERE nspname=$2)))) a",[schema.nspacl,schema.nspname]);
  for(const a of acl.rows)await db.exec(`GRANT ${a.privilege_type} ON SCHEMA ${qi(schema.nspname)} TO ${a.grantee==='PUBLIC'?'PUBLIC':qi(a.grantee)}${a.is_grantable?' WITH GRANT OPTION':''};`);
 }
 await db.exec(`REVOKE TEMPORARY ON DATABASE postgres FROM PUBLIC,${names.map(qi).join(',')};GRANT TEMPORARY ON DATABASE postgres TO PUBLIC;`);
 for(const name of names)await db.exec(`ALTER DEFAULT PRIVILEGES REVOKE EXECUTE ON FUNCTIONS FROM ${qi(name)};`);
 await db.exec('ALTER DEFAULT PRIVILEGES GRANT EXECUTE ON FUNCTIONS TO PUBLIC;');
 await db.exec("CREATE FUNCTION public.restored_default_probe() RETURNS int LANGUAGE sql AS 'SELECT 1';");
 mark('fixture global function EXECUTE default restored',(await db.query("SELECT has_function_privilege('anon','public.restored_default_probe()','EXECUTE') ok")).rows[0].ok);
 await db.exec('DROP FUNCTION public.restored_default_probe();');
 const perms=await db.query("SELECT has_schema_privilege('anon','public','USAGE') u,has_schema_privilege('authenticated','storage','USAGE') s,has_database_privilege('anon',current_database(),'TEMP') t");mark('fixture ACL/default privilege recovery restores baseline behavior',Object.values(perms.rows[0]).every(Boolean));
 await execfile('001_identity_and_mission_sessions.sql');await execfile('002_public_acl_isolation.sql');await execfile('003_runtime_role.sql');mark('clean install after local rollback succeeds',true);
 const failuresDb=new PGlite();try{
 await failuresDb.exec(readFileSync(dir+'900_local_fixture_only.sql','utf8'));await failuresDb.exec('CREATE ROLE astrogateway_api NOLOGIN;');
 let failed=false;try{await failuresDb.exec(readFileSync(dir+'001_identity_and_mission_sessions.sql','utf8'))}catch(e){failed=e.code==='42710';await failuresDb.exec('ROLLBACK')}
 const n=(await failuresDb.query("SELECT count(*)::int n FROM pg_namespace WHERE nspname='training'")).rows[0].n;
 mark('preexisting role collision leaves no partial training schema',failed&&n===0);
 }finally{await failuresDb.close();}
 for(const failure of ['trigger_collision','backfill_null']){
 const late=new PGlite();try{
 await late.exec(readFileSync(dir+'900_local_fixture_only.sql','utf8'));
 if(failure==='trigger_collision')await late.exec("CREATE FUNCTION auth.fixture_trigger() RETURNS trigger LANGUAGE plpgsql AS $$BEGIN RETURN NEW; END$$; CREATE TRIGGER astrogateway_auth_user_profile AFTER INSERT ON auth.users FOR EACH ROW EXECUTE FUNCTION auth.fixture_trigger();");
 else await late.exec("ALTER TABLE auth.users ALTER COLUMN created_at DROP NOT NULL;INSERT INTO auth.users VALUES('99999999-9999-4999-8999-999999999999',NULL,'{}');");
 let failed=false;try{await late.exec(readFileSync(dir+'001_identity_and_mission_sessions.sql','utf8'))}catch(e){failed=(failure==='trigger_collision'?e.code==='42710':e.code==='23502');await late.exec('ROLLBACK');}
 const state=(await late.query("SELECT NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname='training') AND NOT EXISTS(SELECT 1 FROM pg_roles WHERE rolname='astrogateway_api') clean")).rows[0].clean;
 mark(failure+' late failure rolls back schema and role atomically',failed&&state);
 }finally{await late.close();}
 }
 writeFileSync('outputs/Stage05B1/Hardening/Evidence/sql-rehearsal.json',JSON.stringify({engine:version,live_supabase:false,remote_connections:0,tests,blocking_issues:issues,limitations:['Embedded PostgreSQL WASM, no TCP authentication or asyncpg pool','Auth schema/roles are minimal fixtures, not a live Supabase stack','Local ACL recovery is not approved remote rollback']},null,2));
} catch(e){writeFileSync('outputs/Stage05B1/Hardening/Evidence/sql-rehearsal-error.json',JSON.stringify({error:e.message,code:e.code,tests},null,2));console.error(e.message);process.exitCode=1;}finally{await db.close();}
