"""Reproduce local prototype checks; never installs, deploys or handles account credentials."""
import argparse
from datetime import datetime, timezone
import json
from pathlib import Path
import shutil
import subprocess
import sys
import time

ROOT = Path(__file__).resolve().parents[2]
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--backend-python', default=sys.executable, help='Python environment with requirements-dev.txt already installed')
parser.add_argument('--output', required=True, type=Path, help='External evidence directory; do not commit machine logs')
args = parser.parse_args()
out = args.output.resolve()
if out == ROOT or ROOT in out.parents:
    parser.error('--output must be outside the source repository')
out.mkdir(parents=True, exist_ok=True)
api = ROOT / '02-Software-Development/Backend/astrogateway-api'
web = ROOT / '02-Software-Development/Web-Dashboards/astrogateway-web'
checks = [
    ('backend', [args.backend_python, '-m', 'pytest', 'tests', '-q'], api),
    ('web-tests', ['npm', 'test'], web),
    ('web-lint', ['npm', 'run', 'lint'], web),
    ('web-build', ['npm', 'run', 'build'], web),
    ('offline-coach', [sys.executable, 'tools/offline-coach/validate_mono.py'], ROOT),
    ('progress-state', [sys.executable, 'tools/prototype/validate_progress.py'], ROOT),
    ('android-export-tools', [sys.executable, '-m', 'unittest', 'discover', '-s', '02-Software-Development/Integrations/android-unity-host-poc/tools/tests', '-v'], ROOT),
]
results = []
for name, command, cwd in checks:
    print(f'Running {name}', flush=True)
    started = time.monotonic()
    log = out / f'{name}.log'
    try:
        # Resolve installed binaries explicitly; no shell command interpolation.
        executable = shutil.which(command[0])
        if not executable:
            raise FileNotFoundError(f'Executable not found: {command[0]}')
        with log.open('w') as stream:
            completed = subprocess.run([executable, *command[1:]], cwd=cwd, stdout=stream, stderr=subprocess.STDOUT, timeout=300)
        result = {'check': name, 'status': 'PASS' if completed.returncode == 0 else 'FAIL', 'exit_code': completed.returncode}
    except (OSError, subprocess.TimeoutExpired) as error:
        log.write_text(str(error))
        result = {'check': name, 'status': 'BLOCKED', 'reason': str(error)}
    result.update(seconds=round(time.monotonic() - started, 2), log=log.name)
    results.append(result)
    print(f'{name}: {result["status"]}', flush=True)
summary = {'checked_at_utc': datetime.now(timezone.utc).isoformat(), 'scope': 'local automated checks only', 'checks': results,
           'not_tested': ['Unity graphical gameplay', 'Android APK rebuild/install', 'physical-device touch/performance', 'hosted Auth/database/storage', 'live end-to-end scoring', 'multiplayer/media'],
           'status': 'PASS' if all(r['status'] == 'PASS' for r in results) else 'INCOMPLETE'}
(out / 'summary.json').write_text(json.dumps(summary, indent=2) + '\n')
print(json.dumps(summary, indent=2))
raise SystemExit(0 if summary['status'] == 'PASS' else 1)
