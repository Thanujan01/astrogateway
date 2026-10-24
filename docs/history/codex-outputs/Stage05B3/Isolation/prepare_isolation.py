from pathlib import Path
import sys,json,os
BASE=Path(__file__).resolve().parent
WORK=BASE.parent
cfg=json.loads((WORK/'stage05b2/private/lab.json').read_text())
venv=WORK/'stage05b/venv'
profile=['(version 1)','(deny default)','(allow process-fork)','(allow process-exec)','(allow sysctl-read)','(allow file-read-metadata)']
paths=['/opt/homebrew/Cellar/python@3.14','/opt/homebrew/opt/python@3.14','/System','/usr','/Library/Apple','/private/var/db/dyld','/dev/null','/dev/urandom','/dev/random',str(venv),str(BASE/'runtime'),str(BASE/'scratch'),str(Path(sys.base_prefix).resolve()),'/opt/homebrew/Cellar/openssl@3','/opt/homebrew/opt/openssl@3','/opt/homebrew/lib']
for p in paths:profile.append('(allow file-read* (subpath '+json.dumps(p)+'))')
profile+=['(allow file-read* (require-not (require-any (subpath "/Users") (subpath "/Volumes") (subpath "/private/tmp") (subpath "/private/var/folders") (subpath "/Library/Keychains"))))','(allow mach-lookup (global-name "com.apple.system.notification_center"))','(allow file-write* (subpath '+json.dumps(str(BASE/'scratch'))+'))','(allow file-write-data (literal "/dev/null"))','(allow network-outbound (remote tcp "localhost:'+str(cfg['rest_port'])+'"))']
profile += ['(allow network-bind (local tcp "localhost:8001"))','(allow network-inbound (local tcp "localhost:8001"))']
(BASE/'runtime.sb').write_text('\n'.join(profile)+'\n')
print('Isolation profile prepared; loopback gateway only, no operator/admin paths.')
