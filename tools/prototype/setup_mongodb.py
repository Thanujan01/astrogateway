"""Install a checksum-verified local MongoDB and isolated Python dependencies for this prototype."""
import hashlib
from pathlib import Path
import platform
import subprocess
import sys
import tarfile
import urllib.request

ROOT = Path(__file__).resolve().parents[2]
API = ROOT / '02-Software-Development/Backend/astrogateway-api'
URL = 'https://fastdl.mongodb.org/osx/mongodb-macos-arm64-8.0.34.tgz'
SHA256 = '3255cbf2e70cc5de04220046e006067d20068656d1bfa674828768a51d20d5a6'
# Version/hash from MongoDB's official mongodb/homebrew-brew versioned 8.0 formula.
if platform.system() != 'Darwin' or platform.machine() != 'arm64':
    raise SystemExit('Install MongoDB Community using the official instructions for this platform; this bundled installer targets Apple Silicon Macs.')
folder = ROOT / 'prototype-builds/mongodb'
folder.mkdir(parents=True, exist_ok=True)
if not list(folder.glob('*/bin/mongod')):
    archive = folder / 'server.tgz'
    urllib.request.urlretrieve(URL, archive)
    if hashlib.sha256(archive.read_bytes()).hexdigest() != SHA256:
        raise SystemExit('MongoDB checksum mismatch; archive was not executed.')
    with tarfile.open(archive) as package:
        package.extractall(folder, filter='data')
venv = API / '.venv'
if not (venv / 'bin/python').exists(): subprocess.run([sys.executable, '-m', 'venv', str(venv)], check=True)
subprocess.run([str(venv / 'bin/python'), '-m', 'pip', 'install', '-r', str(API / 'requirements-mongodb.txt')], check=True)
print('Local MongoDB and backend dependencies are ready. Double-click Run Prototype.command.')
