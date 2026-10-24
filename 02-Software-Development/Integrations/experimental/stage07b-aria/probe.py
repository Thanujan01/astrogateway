import os,socket,json,errno
checks={}
for name,path in [('administrative_env','/Users/kapilash/Projects/AstroGatewayAndroid/backend/.env'),('android_private_config','/Users/kapilash/Projects/AstroGatewayAndroid/auth.local.properties')]:
 try:
  with open(path,'rb') as f: f.read(1)
  checks[name]='FAIL readable'
 except PermissionError:checks[name]='PASS denied by runtime sandbox'
 except FileNotFoundError:checks[name]='BLOCKED path absent'
try:
 socket.create_connection(('127.0.0.1',8000),timeout=.2)
 checks['network']='FAIL allowed'
except PermissionError:checks['network']='PASS denied'
except OSError as e:checks['network']='PASS denied' if e.errno==errno.EPERM else 'BLOCKED cannot prove sandbox denial'
checks['environment']='PASS cleared' if set(os.environ)<= {'PATH','HOME','TMPDIR','LANG','PYTHONPATH','PYTHONDONTWRITEBYTECODE','PYTHONNOUSERSITE','LC_CTYPE'} else 'FAIL unexpected names'
try:
 open('outputs/Stage07B/Development/stage07b/privacy.py','ab').close()
 checks['source_write']='FAIL writable'
except PermissionError:checks['source_write']='PASS denied'
print(json.dumps(checks,indent=2))
assert all(x.startswith('PASS') for x in checks.values())
