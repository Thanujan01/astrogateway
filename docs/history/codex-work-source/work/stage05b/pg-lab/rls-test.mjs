// Real PostgreSQL WASM semantics; auth.users is an isolated fixture, NOT live Supabase Auth.
import { PGlite } from '@electric-sql/pglite';
import { readFileSync, writeFileSync } from 'node:fs';
const db=new PGlite();const results=[];
const A='11111111-1111-4111-8111-111111111111',B='22222222-2222-4222-8222-222222222222',C='33333333-3333-4333-8333-333333333333';
const SA='aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa',SB='bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbbb';
const record=(name,value)=>{if(!value)throw Error(name+' failed');results.push({name,passed:true});console.log('PASS',name)};
try {
 await db.exec(`CREATE ROLE anon NOLOGIN; CREATE ROLE authenticated NOLOGIN; CREATE SCHEMA auth;
 CREATE TABLE auth.users(id uuid PRIMARY KEY,created_at timestamptz NOT NULL DEFAULT now(),raw_user_meta_data jsonb,email_confirmed_at timestamptz,is_anonymous boolean DEFAULT false);
 INSERT INTO auth.users(id,raw_user_meta_data,email_confirmed_at) VALUES('${A}','{"display_name":"Astronaut A","training_level":10}',now()),('${B}','{"display_name":"Astronaut B"}',now());`);
 await db.exec(readFileSync('work/stage05b/database/001_identity_and_mission_sessions.sql','utf8'));
 record('migration executes',true);
 let row=await db.query('SELECT training_level FROM training.profiles WHERE user_id=$1',[A]);record('untrusted metadata cannot award a level',row.rows[0].training_level===0);
 await db.exec(`INSERT INTO auth.users(id,raw_user_meta_data,email_confirmed_at) VALUES('${C}','{"display_name":"New profile","training_level":10}',now())`);
 row=await db.query('SELECT training_level FROM training.profiles WHERE user_id=$1',[C]);record('auth insert trigger creates default trainee profile',row.rows[0].training_level===0);
 async function asUser(uid,sid,task) {
  await db.exec('BEGIN; SET LOCAL ROLE astrogateway_api;');
  await db.query("SELECT set_config('astrogateway.user_id',$1,true)",[uid]);await db.query("SELECT set_config('astrogateway.auth_session_id',$1,true)",[sid]);
  try {const value=await task();await db.exec('COMMIT');return value} catch(e) {await db.exec('ROLLBACK');throw e}
 }
 async function denied(name,role,sql,uid=A,sid=SA) {
  let rejected=false;
  try {
   if(role==='astrogateway_api')await asUser(uid,sid,()=>db.exec(sql));
   else {await db.exec(`BEGIN; SET LOCAL ROLE ${role};`);try{await db.exec(sql)}finally{await db.exec('ROLLBACK')}}
  }catch(e){rejected=/permission denied|row-level security/i.test(e.message)}
  record(name,rejected);
 }
 for(const role of ['anon','authenticated'])await denied(role+' cannot access private profiles',role,'SELECT * FROM training.profiles');
 row=await asUser(A,SA,()=>db.query('SELECT user_id FROM training.profiles'));record('profile selection isolated to verified context',row.rows.length===1&&row.rows[0].user_id===A);
 row=await asUser(A,SA,()=>db.query('SELECT * FROM training.profiles WHERE user_id=$1',[B]));record('cross-account profile hidden',row.rows.length===0);
 await denied('training level cannot be escalated','astrogateway_api',`UPDATE training.profiles SET training_level=10 WHERE user_id='${A}'`);
 await denied('API cannot insert alternate profiles','astrogateway_api',`INSERT INTO training.profiles(user_id,display_name) VALUES('${A}','Replacement')`);
 await denied('ownership cannot be changed','astrogateway_api',`INSERT INTO training.mission_sessions(user_id,auth_session_id,launch_id,mission_id) VALUES('${B}','${SA}','44444444-4444-4444-8444-444444444444','mission-red-horizon-poc')`);
 await denied('session cannot bind another auth session','astrogateway_api',`INSERT INTO training.mission_sessions(user_id,auth_session_id,launch_id,mission_id) VALUES('${A}','${SB}','44444444-4444-4444-8444-444444444444','mission-red-horizon-poc')`);
 const created=await asUser(A,SA,()=>db.query("INSERT INTO training.mission_sessions(user_id,auth_session_id,launch_id,mission_id) VALUES($1,$2,'44444444-4444-4444-8444-444444444444','mission-red-horizon-poc') RETURNING id",[A,SA]));const id=created.rows[0].id;record('authorized mission initializes',!!id);
 row=await asUser(B,SB,()=>db.query('SELECT * FROM training.mission_sessions WHERE id=$1',[id]));record('cross-account mission hidden',row.rows.length===0);
 row=await asUser(B,SB,()=>db.query("UPDATE training.mission_sessions SET status='integration_check_complete',completed_at=now() WHERE id=$1 RETURNING id",[id]));record('cross-account completion changes nothing',row.rows.length===0);
 await denied('mission expiry cannot be extended by API role','astrogateway_api',`UPDATE training.mission_sessions SET expires_at=now()+interval '1 year' WHERE id='${id}'`);
 row=await asUser(A,SA,()=>db.query("UPDATE training.mission_sessions SET status='integration_check_complete',completed_at=now() WHERE id=$1 RETURNING status",[id]));record('authorized completion transition succeeds',row.rows[0].status==='integration_check_complete');
 row=await asUser(A,SA,()=>db.query('SELECT * FROM training.mission_sessions WHERE id=$1 FOR UPDATE',[id]));record('completed row remains available for idempotent receipt',row.rows.length===1);
 const expired=await db.query("INSERT INTO training.mission_sessions(user_id,auth_session_id,launch_id,mission_id,expires_at) VALUES($1,$2,'55555555-5555-4555-8555-555555555555','mission-red-horizon-poc',now()-interval '1 minute') RETURNING id",[A,SA]);
 row=await asUser(A,SA,()=>db.query("UPDATE training.mission_sessions SET status='integration_check_complete',completed_at=now() WHERE id=$1 RETURNING id",[expired.rows[0].id]));record('expired initialization cannot complete',row.rows.length===0);
 await asUser(A,SA,()=>db.query("INSERT INTO training.auth_session_revocations(auth_session_id,user_id,expires_at) VALUES($1,$2,now()+interval '1 hour')",[SA,A]));
 row=await asUser(A,SA,()=>db.query('SELECT * FROM training.profiles'));record('revoked auth session denied by RLS',row.rows.length===0);
 row=await asUser(A,SB,()=>db.query('SELECT * FROM training.profiles'));record('another unrevoked session of same account remains allowed',row.rows.length===1);
 await db.exec('BEGIN; SET LOCAL ROLE astrogateway_api;');row=await db.query('SELECT * FROM training.profiles');await db.exec('ROLLBACK');record('transaction context cannot leak into next user',row.rows.length===0);
 writeFileSync('outputs/Stage05B/rls-validation.json',JSON.stringify({engine:'PGlite PostgreSQL WASM',live_supabase:false,passed:true,tests:results},null,2));
} catch(error) {
 writeFileSync('outputs/Stage05B/rls-validation.json',JSON.stringify({engine:'PGlite PostgreSQL WASM',live_supabase:false,passed:false,error:error.message,tests:results},null,2));throw error;
} finally { await db.close(); }
