"""Install a checksum-verified local MongoDB and isolated Python dependencies for this prototype.

The bundled, checksum-verified MongoDB download targets Apple Silicon macOS. On every other
platform install MongoDB Community yourself and this script still prepares the backend
virtual environment, so `tools/prototype/start.py` has everything else it needs.
"""
import hashlib
import os
from pathlib import Path
import platform
import shutil
import subprocess
import sys
import tarfile
import urllib.request

ROOT = Path(__file__).resolve().parents[2]
API = ROOT / '02-Software-Development/Backend/astrogateway-api'
WINDOWS = os.name == 'nt'
URL = 'https://fastdl.mongodb.org/osx/mongodb-macos-arm64-8.0.34.tgz'
SHA256 = '3255cbf2e70cc5de04220046e006067d20068656d1bfa674828768a51d20d5a6'
# Version/hash from MongoDB's official mongodb/homebrew-brew versioned 8.0 formula.
BUNDLED = platform.system() == 'Darwin' and platform.machine() == 'arm64'

# pymongo 4.19 (pinned in requirements-mongodb.txt) requires Python 3.11+, and the backend
# source uses 3.10+ union syntax throughout. Check before pip prints a resolver wall of text.
MINIMUM = (3, 11)
if sys.version_info < MINIMUM:
    raise SystemExit(
        'This prototype needs Python %d.%d or newer; this interpreter is %d.%d.%d (%s). '
        'Install a current Python and re-run with it, for example: py -3.13 tools/prototype/setup_mongodb.py'
        % (MINIMUM[0], MINIMUM[1], *sys.version_info[:3], sys.executable))

folder = ROOT / 'prototype-builds/mongodb'
binary = 'mongod.exe' if WINDOWS else 'mongod'

if BUNDLED:
    folder.mkdir(parents=True, exist_ok=True)
    if not list(folder.glob('*/bin/mongod')):
        archive = folder / 'server.tgz'
        urllib.request.urlretrieve(URL, archive)
        if hashlib.sha256(archive.read_bytes()).hexdigest() != SHA256:
            raise SystemExit('MongoDB checksum mismatch; archive was not executed.')
        with tarfile.open(archive) as package:
            # filter='data' needs Python 3.12+; older interpreters extract without it.
            if sys.version_info >= (3, 12):
                package.extractall(folder, filter='data')
            else:
                package.extractall(folder)
else:
    # No verified archive is published here, so never download an unchecked binary.
    found = list(folder.glob('*/bin/' + binary)) or ([shutil.which('mongod')] if shutil.which('mongod') else [])
    if not found:
        print(
            'No bundled MongoDB for this platform (' + platform.system() + '/' + platform.machine() + ').\n'
            'Install MongoDB Community 8.0 from https://www.mongodb.com/try/download/community,\n'
            'then make sure "mongod" is on PATH, or unpack it so that\n'
            '  ' + str(folder) + '/<any-name>/bin/' + binary + '\n'
            'exists. start.py finds it either way and runs it on 127.0.0.1:27018.\n'
            'Continuing with the Python environment setup.\n',
            file=sys.stderr)
    else:
        print('Found existing mongod: ' + str(found[0]))

venv = API / '.venv'
python = venv / ('Scripts/python.exe' if WINDOWS else 'bin/python')
if python.exists():
    # A venv left behind by an older interpreter would silently reinstall against that
    # version and fail the pymongo pin again. Rebuild it instead of reusing it.
    probe = subprocess.run([str(python), '-c', 'import sys;print("%d.%d" % sys.version_info[:2])'],
                           capture_output=True, text=True)
    current = '%d.%d' % sys.version_info[:2]
    if probe.returncode != 0 or probe.stdout.strip() != current:
        print('Replacing backend .venv built with Python ' + (probe.stdout.strip() or 'unknown')
              + '; this interpreter is ' + current + '.')
        shutil.rmtree(venv)
if not python.exists():
    subprocess.run([sys.executable, '-m', 'venv', str(venv)], check=True)
subprocess.run([str(python), '-m', 'pip', 'install', '-r', str(API / 'requirements-mongodb.txt')], check=True)

launcher = 'Run Prototype.cmd' if WINDOWS else 'Run Prototype.command'
print('Backend dependencies are ready. Start everything with "' + launcher + '".')
