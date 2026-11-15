"""Start the existing local API, account portal and playable Missions prototype.

Runs on macOS and Windows. Platform-specific pieces (process groups, venv layout, the
mongod binary name and flags, the Unity editor path) are selected from WINDOWS below.
"""
import argparse
import json
import os
from pathlib import Path
import shutil
import signal
import socket
import subprocess
import sys
import time
import urllib.error
import urllib.request
import webbrowser

ROOT = Path(__file__).resolve().parents[2]
API = ROOT / '02-Software-Development/Backend/astrogateway-api'
WEB = ROOT / '02-Software-Development/Web-Dashboards/astrogateway-web'
GAME = ROOT / '01-Game-Development/MissionRedHorizon'
WINDOWS = os.name == 'nt'
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--backend-python', default=os.environ.get('ASTROGATEWAY_BACKEND_PYTHON'))
parser.add_argument('--no-game', action='store_true')
parser.add_argument('--no-browser', action='store_true')
parser.add_argument('--check', action='store_true', help='Verify startup, then stop only processes started by this command')
args = parser.parse_args()
PROVIDER = os.environ.get("ASTROGATEWAY_ACCOUNT_PROVIDER", "mongodb")
if PROVIDER not in ("mongodb", "supabase"): parser.error("Unsupported account provider")
children = []
logs = []

def response(url):
    try:
        with urllib.request.urlopen(url, timeout=2) as result:
            return result.read(2 * 1024 * 1024).decode()
    except (OSError, urllib.error.URLError):
        return None

def start(name, command, cwd):
    folder = ROOT / 'prototype-builds/run-logs'
    folder.mkdir(parents=True, exist_ok=True)
    log = (folder / (name + '.log')).open('w')
    logs.append(log)
    env = dict(os.environ, VITE_AUTH_PROVIDER=PROVIDER) if name == "web" else None
    # Isolate children so Ctrl+C in this terminal does not race our own cleanup.
    if WINDOWS:
        grouping = {'creationflags': subprocess.CREATE_NEW_PROCESS_GROUP}
    else:
        grouping = {'start_new_session': True}
    child = subprocess.Popen(command, cwd=cwd, stdout=log, stderr=subprocess.STDOUT, env=env, **grouping)
    children.append(child)
    return child

def wait_for(url, child, label):
    deadline = time.monotonic() + 40
    while time.monotonic() < deadline:
        body = response(url)
        if body is not None:
            return body
        if child.poll() is not None:
            break
        time.sleep(.2)
    raise RuntimeError(f'{label} did not start. See prototype-builds/run-logs/{label}.log')

def python_environment():
    venv = 'Scripts/python.exe' if WINDOWS else 'bin/python'
    candidates = [args.backend_python, str(API / '.venv' / venv), str(ROOT.parent / 'identity-audit-venv' / venv), sys.executable]
    for candidate in candidates:
        if candidate and Path(candidate).exists():
            result = subprocess.run([candidate, '-c', 'import fastapi, uvicorn, httpx' + ('; import pymongo, argon2' if PROVIDER == 'mongodb' else '')], stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
            if result.returncode == 0:
                return candidate
    raise RuntimeError('Backend dependencies are missing. Run "python tools/prototype/setup_mongodb.py" for MongoDB mode, or install requirements-dev.txt in the backend .venv for legacy Supabase mode.')

def mongo_running():
    try:
        with socket.create_connection(('127.0.0.1', 27018), timeout=1): return True
    except OSError: return False

try:
    if PROVIDER == 'mongodb' and not mongo_running():
        pattern = '*/bin/mongod.exe' if WINDOWS else '*/bin/mongod'
        binaries = list((ROOT / 'prototype-builds/mongodb').glob(pattern))
        installed = shutil.which('mongod')
        binary = str(binaries[0]) if binaries else installed
        if not binary:
            raise RuntimeError('Local MongoDB is missing. Run "' + ('py -3' if WINDOWS else 'python3') + ' tools/prototype/setup_mongodb.py" once, then retry.')
        data = ROOT / 'prototype-builds/mongodb/data'; data.mkdir(parents=True, exist_ok=True, mode=0o700)
        if not WINDOWS:
            data.chmod(0o700)  # No POSIX mode bits on Windows; NTFS ACLs apply instead.
        # --nounixsocket is POSIX-only; Windows mongod rejects the flag.
        flags = [] if WINDOWS else ['--nounixsocket']
        process = start('mongodb', [binary, '--dbpath', str(data), '--bind_ip', '127.0.0.1', '--port', '27018', *flags, '--logpath', str(data.parent / 'mongod.log')], ROOT)
        deadline = time.monotonic() + 20
        while not mongo_running():
            if process.poll() is not None or time.monotonic() > deadline: raise RuntimeError('Local MongoDB did not start. See prototype-builds/mongodb/mongod.log')
            time.sleep(.2)
    api = response('http://127.0.0.1:8000/openapi.json')
    if api is None:
        process = start('backend', [python_environment(), 'run_mongodb.py' if PROVIDER == 'mongodb' else 'run_local.py'], API)
        api = wait_for('http://127.0.0.1:8000/openapi.json', process, 'backend')
    if '/v1/me' not in json.loads(api).get('paths', {}):
        raise RuntimeError('Port 8000 is occupied by another service. Stop that service and retry.')
    if PROVIDER == 'mongodb':
        provider = json.loads(response('http://127.0.0.1:8000/v1/auth/provider') or '{}')
        if provider.get('provider') != 'mongodb-local': raise RuntimeError('Port 8000 is running the old provider. Stop that API and retry with MongoDB mode.')
    package = response('http://127.0.0.1:5173/package.json')
    if package is None:
        npm = shutil.which('npm')
        if not npm or not (WEB / 'node_modules').is_dir():
            raise RuntimeError('Web dependencies are missing. Run npm ci in the web directory, then retry.')
        process = start('web', [npm, 'run', 'dev', '--', '--host', '127.0.0.1', '--port', '5173', '--strictPort'], WEB)
        package = wait_for('http://127.0.0.1:5173/package.json', process, 'web')
    if json.loads(package).get('name') != 'astrogateway':
        raise RuntimeError('Port 5173 is occupied by another service. Stop that service and retry.')
    health = json.loads(response('http://127.0.0.1:8000/health') or '{}')
    proxy = json.loads(response('http://127.0.0.1:5173/account-api/health') or '{}')
    if health.get('status') not in ('ok', 'configuration_required') or health != proxy:
        raise RuntimeError('The account portal cannot reach its existing API proxy.')
    print('Account portal: http://127.0.0.1:5173/welcome', flush=True)
    print('API: http://127.0.0.1:8000/docs', flush=True)
    print('Account provider: local MongoDB. Signup/login work without Supabase.' if PROVIDER == 'mongodb' else 'Account provider: Supabase (existing configuration).', flush=True)
    if not args.check and not args.no_browser:
        webbrowser.open('http://127.0.0.1:5173/welcome')
    if not args.no_game:
        default_app = 'prototype-builds/AstroGateway.exe' if WINDOWS else 'prototype-builds/AstroGateway.app'
        app = Path(os.environ.get('ASTROGATEWAY_PROTOTYPE_APP', str(ROOT / default_app)))
        if app.is_dir() or (WINDOWS and app.is_file()):
            if not args.check:
                if WINDOWS:
                    start('game', [str(app)], ROOT)
                else:
                    subprocess.run(['open', '-n', str(app)], check=True)
            print('Playable Missions app: ' + str(app), flush=True)
        else:
            default_editor = ('C:/Program Files/Unity/Hub/Editor/6000.6.4f1/Editor/Unity.exe' if WINDOWS
                              else '/Applications/Unity/Hub/Editor/6000.6.4f1/Unity.app/Contents/MacOS/Unity')
            editor = Path(os.environ.get('ASTROGATEWAY_UNITY_EDITOR', default_editor))
            if not editor.is_file():
                raise RuntimeError('Build the prototype app, or install the pinned Unity Editor and set ASTROGATEWAY_UNITY_EDITOR, before launching the game.')
            if not args.check:
                start('game-editor', [str(editor), '-projectPath', str(GAME), '-executeMethod', 'PrototypeBuild.Play'], ROOT)
            print('Missions scene launches in Unity; an active Unity license is required.', flush=True)
    print('READY: local API and web proxy respond. Gameplay is local and unscored.', flush=True)
    if not args.check:
        print('Keep this terminal open. Press Ctrl+C to stop services started here.', flush=True)
        while True:
            failed = [p for p in children if p.poll() is not None]
            if failed:
                raise RuntimeError('A prototype process stopped. See prototype-builds/run-logs.')
            time.sleep(1)
except KeyboardInterrupt:
    pass
except (RuntimeError, OSError, ValueError, subprocess.SubprocessError) as error:
    print('Prototype launch: ' + str(error), file=sys.stderr)
    raise SystemExit(1)
finally:
    for child in children:
        if child.poll() is None:
            if WINDOWS:
                # No process groups to signal; taskkill /T reaches the whole tree (npm -> node).
                subprocess.run(['taskkill', '/T', '/F', '/PID', str(child.pid)],
                               stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
            else:
                os.killpg(child.pid, signal.SIGTERM)
    for child in children:
        try:
            child.wait(timeout=5)
        except subprocess.TimeoutExpired:
            if not WINDOWS:
                os.killpg(child.pid, signal.SIGKILL)
    for log in logs:
        log.close()
