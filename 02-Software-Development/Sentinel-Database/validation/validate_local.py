#!/usr/bin/env python3
"""Rehearse tracked candidates ONLY in a newly initialized disposable PostgreSQL cluster.

No DSN/remote-host argument; no existing database or operator credential is used.
Synthetic JWT claims exercise SQL policies, NOT gateway signature verification.
Exit 1 means at least one validation expectation failed. Existing SQL is never edited.
"""
import argparse
import ast
from datetime import datetime, timezone
import hashlib
import json
import os
from pathlib import Path
import re
import shutil
import subprocess
import tempfile
import time
import uuid

ROOT = Path(__file__).resolve().parents[3]
SOFTWARE = ROOT / '02-Software-Development'
BASE = ROOT / 'docs/history/codex-outputs/Stage05B2/SQL'
CANDIDATES = [BASE / '900_local_fixture_only.sql', BASE / '001_verified_jwt_training.sql',
              BASE / '002_retire_direct_runtime.sql',
              ROOT / 'docs/history/codex-outputs/UnifiedAuth/SQL/004_profile_display_name.sql',
              ROOT / '01-Game-Development/MissionRedHorizon/Docs/Step3/SQL/005_mission_commands_reports.sql',
              ROOT / '01-Game-Development/MissionRedHorizon/Docs/FirstMission/SQL/006_sensor_records.sql']


def quoted(value):
    return "'" + str(value).replace("'", "''") + "'"


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--pg-bin', required=True, type=Path, help='Directory with PostgreSQL 17 initdb/pg_ctl/psql')
    parser.add_argument('--report', required=True, type=Path)
    args = parser.parse_args()
    pg = args.pg_bin.resolve()
    for name in ['initdb', 'pg_ctl', 'psql', 'postgres']:
        if not (pg / name).is_file():
            parser.error('Missing PostgreSQL executable: ' + name)
    if not all(p.is_file() for p in CANDIDATES):
        parser.error('Run from a complete repository checkout; required candidate SQL is missing.')
    pg_environment = {k: v for k, v in os.environ.items() if not k.startswith('PG')}
    version = subprocess.check_output([str(pg / 'postgres'), '--version'], text=True, env=pg_environment).strip()
    if not re.search(r'PostgreSQL\) 17\.', version):
        parser.error('This rehearsal targets PostgreSQL 17; other majors require separate verification.')
    started = False
    checks = []
    installs = []
    report = {'run_at_utc': datetime.now(timezone.utc).isoformat(), 'postgres': version,
              'scope': 'disposable local SQL rehearsal; synthetic identity helpers; not hosted validation',
              'remote_database_access': False, 'existing_sql_modified': False,
              'migrations': installs, 'checks': checks,
              'not_tested': ['Hosted Supabase Auth/JWT signature verification and PostgREST error translation',
                             'Live deployment/schema exposure/TLS', 'Concurrent requests and per-session caps under race',
                             'Backup/restore, production rollback and load/performance', 'Stage07A experimental schema']}
    # A short private directory keeps the Unix socket below platform path limits.
    work = Path(tempfile.mkdtemp(prefix='ag-sentinel-', dir='/private/tmp' if Path('/private/tmp').exists() else None))
    socket = work / 'socket'
    socket.mkdir(mode=0o700)
    data = work / 'data'
    ctl = [str(pg / 'pg_ctl'), '-D', str(data)]
    psql = [str(pg / 'psql'), '-X', '-qAt', '-v', 'ON_ERROR_STOP=1', '-v', 'VERBOSITY=verbose',
            '-h', str(socket), '-p', '55479', '-U', 'sentinel_validator', '-d', 'postgres']

    def sql(query=None, file=None):
        return subprocess.run(psql + (['-f', str(file)] if file else ['-c', query]), capture_output=True, text=True, timeout=40, env=pg_environment)

    def claim(user, session, expired=False):
        return {'sub': user, 'session_id': session, 'role': 'authenticated', 'aud': 'authenticated',
                'iss': 'https://jvkwofmxtxjhdwtwdboz.supabase.co/auth/v1',
                'exp': int(time.time()) + (-60 if expired else 3600), 'is_anonymous': False}

    def as_user(query, claims):
        return 'BEGIN; SET LOCAL request.jwt.claims = ' + quoted(json.dumps(claims)) + '; SET LOCAL ROLE authenticated; ' + query + '; COMMIT;'

    def state(result):
        match = re.search(r'ERROR:\s+([A-Z0-9]{5}):', result.stderr)
        return match[1] if match else None

    def check(name, query, expected=None, error=None, claims=None):
        r = sql(as_user(query, claims) if claims else query)
        observed = state(r) if r.returncode else r.stdout.strip()
        passed = (r.returncode != 0 and observed == error) if error else (r.returncode == 0 and (expected is None or observed == str(expected)))
        row = {'name': name, 'status': 'PASS' if passed else 'FAIL',
               'expected_sqlstate': error} if error else {'name': name, 'status': 'PASS' if passed else 'FAIL', 'expected_result': expected}
        row['observed'] = observed
        if r.returncode:
            # First error only; no full SQL, claims, raw logs or private paths are published.
            row['error_summary'] = next((l.strip() for l in r.stderr.splitlines() if 'ERROR:' in l), 'SQL execution failed')[:220]
        checks.append(row)
        return r

    try:
        r = subprocess.run([str(pg / 'initdb'), '-D', str(data), '-U', 'sentinel_validator', '-A', 'trust', '--no-locale'], capture_output=True, text=True, timeout=40, env=pg_environment)
        if r.returncode:
            raise RuntimeError('Disposable initdb failed; no existing database was used.')
        options = f"-k {socket} -p 55479 -c listen_addresses='' -c unix_socket_permissions=0700"
        r = subprocess.run(ctl + ['-l', str(work / 'postgres.log'), '-o', options, '-w', 'start'], capture_output=True, text=True, timeout=40, env=pg_environment)
        if r.returncode:
            raise RuntimeError('Disposable cluster start failed; local socket permission may require execution approval.')
        started = True
        for path in CANDIDATES:
            r = sql(file=path)
            installs.append({'file': str(path.relative_to(ROOT)), 'sha256': hashlib.sha256(path.read_bytes()).hexdigest(),
                             'status': 'PASS' if r.returncode == 0 else 'FAIL'})
            if r.returncode:
                raise RuntimeError('Candidate installation failed: ' + str(path.relative_to(ROOT)))
        backend = (SOFTWARE / 'Backend/astrogateway-api/database.py').read_text()
        required = set(ast.literal_eval(re.search(r'if name not in (\([\s\S]*?\)):', backend)[1]))
        r = sql("SELECT proname FROM pg_proc p JOIN pg_namespace n ON n.oid=p.pronamespace WHERE n.nspname='mission_api' ORDER BY proname")
        installed = set(r.stdout.splitlines())
        report['rpc_coverage'] = {'required': sorted(required), 'installed': sorted(installed), 'missing': sorted(required-installed)}
        checks.append({'name': 'Current backend RPC names are present', 'status': 'PASS' if not required-installed else 'FAIL'})
        legacy = (SOFTWARE / 'Sentinel-Database/001_identity_and_mission_sessions.sql').read_text()
        report['legacy_candidate'] = {'file': '02-Software-Development/Sentinel-Database/001_identity_and_mission_sessions.sql',
                                      'status': 'SUPERSEDED; not a standalone current-backend installation',
                                      'mission_api_rpc_definitions': len(re.findall(r'CREATE\s+FUNCTION\s+mission_api\.', legacy, re.I))}
        check('RLS enabled on all five training tables', "SELECT count(*) FROM pg_class c JOIN pg_namespace n ON n.oid=c.relnamespace WHERE n.nspname='training' AND c.relkind='r' AND c.relrowsecurity", 5)
        check('No exposed SECURITY DEFINER RPCs', "SELECT count(*) FROM pg_proc p JOIN pg_namespace n ON n.oid=p.pronamespace WHERE n.nspname='mission_api' AND p.prosecdef", 0)
        check('Anonymous cannot execute current_profile', "SELECT has_function_privilege('anon','mission_api.current_profile()','EXECUTE')", 'f')
        check('Legacy login role retired', "SELECT NOT rolcanlogin AND NOT rolbypassrls AND NOT rolsuper FROM pg_roles WHERE rolname='astrogateway_runtime'", 't')
        user_a, user_b, auth_a, auth_b = [str(uuid.uuid4()) for _ in range(4)]
        ca, cb = claim(user_a, auth_a), claim(user_b, auth_b)
        r = sql("INSERT INTO auth.users(id,raw_user_meta_data,email_confirmed_at) VALUES (" + quoted(user_a) + ",'{\"display_name\":\"Fixture A\"}',now()),(" + quoted(user_b) + ",'{\"display_name\":\"Fixture B\"}',now())")
        if r.returncode:
            raise RuntimeError('Synthetic user preparation failed.')
        launch = str(uuid.uuid4())
        r = sql(as_user("SELECT mission_api.initialize_mission("+quoted(launch)+",'mission-red-horizon-poc')->>'id'", ca))
        if r.returncode:
            raise RuntimeError('Synthetic session preparation failed.')
        session = str(uuid.UUID(r.stdout.strip()))
        check('Profile returns the current owner', "SELECT mission_api.current_profile()->>'user_id'", user_a, claims=ca)
        check('Other user profile is invisible', "SELECT count(*) FROM training.profiles WHERE user_id="+quoted(user_b), 0, claims=ca)
        check('Cross-user mission insert denied by RLS', "INSERT INTO training.mission_sessions(user_id,auth_session_id,launch_id,mission_id) VALUES ("+quoted(user_b)+','+quoted(auth_a)+','+quoted(str(uuid.uuid4()))+",'mission-red-horizon-poc')", error='42501', claims=ca)
        check('Profile RPC rejects short display name', "SELECT mission_api.update_display_name('x')", error='PT422', claims=ca)
        check('Database enforces display-name length', "UPDATE training.profiles SET display_name='x' WHERE user_id="+quoted(user_a), error='23514')
        check('Database enforces training-level range', "UPDATE training.profiles SET training_level=11 WHERE user_id="+quoted(user_a), error='23514')
        check('Profile requires an Auth user FK', "INSERT INTO training.profiles(user_id,display_name) VALUES ("+quoted(str(uuid.uuid4()))+",'Fixture missing')", error='23503')
        check('Duplicate mission launch is rejected', "SELECT mission_api.initialize_mission("+quoted(launch)+",'mission-red-horizon-poc')", error='23505', claims=ca)
        check('Completion status requires completed_at', "UPDATE training.mission_sessions SET status='integration_check_complete' WHERE id="+quoted(session), error='23514')
        check('Expired SQL claims are rejected', 'SELECT mission_api.current_profile()', error='PT401', claims=claim(user_a, auth_a, True))
        command = str(uuid.uuid4())
        call = "mission_api.submit_mission_command("+quoted(session)+','+quoted(command)+",0,'board_rover','{}')"
        check('First semantic command accepted', 'SELECT '+call+"->>'duplicate'", 'false', claims=ca)
        check('Identical command replay is idempotent', 'SELECT '+call+"->>'duplicate'", 'true', claims=ca)
        check('Stale command revision rejected', "SELECT mission_api.submit_mission_command("+quoted(session)+','+quoted(str(uuid.uuid4()))+",0,'pause','{}')", error='PC409', claims=ca)
        check('Unknown command kind rejected', "SELECT mission_api.submit_mission_command("+quoted(session)+','+quoted(str(uuid.uuid4()))+",1,'unknown','{}')", error='PC422', claims=ca)
        for label, payload in [('Scan command missing tick is rejected', {'sample_id': str(uuid.uuid4()), 'analysis': 'unverified_simulated', 'extra': 1}),
                               ('Scan command fractional tick is rejected', {'sample_id': str(uuid.uuid4()), 'analysis': 'unverified_simulated', 'tick': 1.5})]:
            revision = "(SELECT count(*)::integer FROM training.mission_commands WHERE session_id="+quoted(session)+')'
            check(label, "SELECT mission_api.submit_mission_command("+quoted(session)+','+quoted(str(uuid.uuid4()))+','+revision+",'scan_target',"+quoted(json.dumps(payload))+')', error='PC422', claims=ca)
        record = {'event_id': str(uuid.uuid4()), 'sensor_id': 'environment.temperature', 'provenance': 'SIMULATED'}
        def sensor(records):
            value = 'NULL::jsonb' if records is None else quoted(json.dumps(records))+'::jsonb'
            return 'mission_api.submit_sensor_records('+quoted(session)+','+value+')'
        check('SQL-layer sensor fixture accepted', 'SELECT '+sensor([record])+"->0->>'duplicate'", 'false', claims=ca)
        check('Identical sensor replay is idempotent', 'SELECT '+sensor([record])+"->0->>'duplicate'", 'true', claims=ca)
        check('Sensor conflict uses backend-expected SQLSTATE', 'SELECT '+sensor([{**record,'extra':1}]), error='PS409', claims=ca)
        check('Forged analysis provenance uses expected rejection', 'SELECT '+sensor([{**record,'event_id':str(uuid.uuid4()),'provenance':'BACKEND-ANALYZED'}]), error='PS422', claims=ca)
        check('Null sensor batch rejected', 'SELECT '+sensor(None), error='PS422', claims=ca)
        for label, records in [('Object sensor batch rejected', {}), ('Empty sensor batch rejected', []), ('Null sensor element rejected', [None]), ('Missing sensor provenance rejected', [{'event_id':str(uuid.uuid4()),'sensor_id':'environment.temperature'}])]:
            check(label, 'SELECT '+sensor(records), error='PS422', claims=ca)
        check('Cross-user sensor RPC uses expected denial code', 'SELECT '+sensor([{**record,'event_id':str(uuid.uuid4())}]), error='PT404', claims=cb)
        check('Cross-user sensor read returns no rows', 'SELECT count(*) FROM training.sensor_records WHERE session_id='+quoted(session), 0, claims=cb)
        check('Auth session can be revoked', 'SELECT mission_api.revoke_session()', 't', claims=ca)
        check('Revoked session cannot read its profile', 'SELECT mission_api.current_profile()', error='PT401', claims=ca)
        r = sql(file=BASE/'001_verified_jwt_training.sql')
        checks.append({'name': 'Fresh-install candidate refuses reapplication/collisions', 'status': 'PASS' if r.returncode and state(r) == '42710' else 'FAIL', 'observed_sqlstate': state(r)})
    except Exception as exc:
        report['harness_error'] = str(exc)
    finally:
        stopped = True
        if started:
            stop = subprocess.run(ctl + ['-m', 'immediate', '-w', 'stop'], capture_output=True, timeout=40, env=pg_environment)
            stopped = stop.returncode == 0
        if stopped:
            shutil.rmtree(work)
        else:
            report['harness_error'] = 'Could not stop the disposable cluster; its private temporary files were preserved for operator cleanup.'
        failed = sum(c['status']=='FAIL' for c in checks)
        report['summary'] = {'passed': sum(c['status']=='PASS' for c in checks), 'failed': failed,
                             'overall': 'FAIL' if failed or 'harness_error' in report else 'PASS',
                             'disposable_cluster_removed': stopped}
        args.report.parent.mkdir(parents=True, exist_ok=True)
        args.report.write_text(json.dumps(report, indent=2)+'\n')
        print(json.dumps(report['summary'], indent=2))
        if 'harness_error' in report:
            print(report['harness_error'])
    return 1 if report['summary']['overall']=='FAIL' else 0


if __name__ == '__main__':
    raise SystemExit(main())
