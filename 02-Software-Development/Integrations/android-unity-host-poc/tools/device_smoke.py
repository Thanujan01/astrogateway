import json, os, re, subprocess, time, xml.etree.ElementTree as ET
from pathlib import Path
if os.environ.get('ASTROGATEWAY_LEGACY_CUBE_SMOKE') != '1':
 raise SystemExit('Legacy cube-only smoke script: not valid for the Missions host. Follow docs/GAME_INTEGRATION_UPDATE.md device acceptance steps; no install was performed.')
ROOT=Path('/Users/kapilash/Projects/AstroGatewayAndroid')
ADB='/Applications/Unity/Hub/Editor/6000.6.4f1/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb'
PACKAGE='com.sk2lb.astrogateway.poc'

def call(*args,timeout=30):
 r=subprocess.run([ADB,*args],capture_output=True,timeout=timeout)
 if r.returncode:raise RuntimeError(r.stderr.decode(errors='replace'))
 return r.stdout
inventory=call('devices','-l').decode()
devices=[l.split()[0] for l in inventory.splitlines()[1:] if len(l.split())>1 and l.split()[1]=='device']
if len(devices)!=1:raise SystemExit('Need exactly one authorized device; found '+str(len(devices)))
SERIAL=devices[0]
def adb(*args,timeout=30):return call('-s',SERIAL,*args,timeout=timeout)
def logs():return adb('logcat','-d','-s','AstroGatewayPOC:I','AndroidRuntime:E','*:S').decode(errors='replace')
def screenshot(name):
 (ROOT/'logs'/name).write_bytes(adb('exec-out','screencap','-p'))
def tap_button(label):
 adb('shell','uiautomator','dump','/sdcard/astrogateway-poc-ui.xml',timeout=45)
 xml=adb('shell','cat','/sdcard/astrogateway-poc-ui.xml').decode()
 nodes=ET.fromstring(xml).iter('node')
 for node in nodes:
  if node.attrib.get('text','').lower()==label.lower() and node.attrib.get('package')==PACKAGE:
   bounds=list(map(int,re.findall(r'\d+',node.attrib['bounds'])))
   adb('shell','input','tap',str((bounds[0]+bounds[2])//2),str((bounds[1]+bounds[3])//2))
   return
 raise RuntimeError('Button missing: '+label)

def wait_for(predicate,seconds=45):
 deadline=time.time()+seconds
 while time.time()<deadline:
  text=logs()
  if 'FATAL EXCEPTION' in text:raise RuntimeError('Android runtime crash; see device-logcat.log')
  result=predicate(text)
  if result:return result
  time.sleep(0.5)
 raise RuntimeError('Timed out waiting for expected test evidence')

result={'device_model':adb('shell','getprop','ro.product.model').decode().strip(),'android_release':adb('shell','getprop','ro.build.version.release').decode().strip(),'api':adb('shell','getprop','ro.build.version.sdk').decode().strip(),'physical_device':not SERIAL.startswith('emulator-'),'cycles':[]}
try:
 apk=ROOT/'app/build/outputs/apk/debug/app-debug.apk'
 if not apk.exists():
  apk=ROOT/'unity-export/app/build/outputs/apk/debug/app-debug.apk'
 install=adb('install','-r',str(apk),timeout=90).decode();(ROOT/'logs/adb-install.log').write_text(install)
 adb('logcat','-c')
 launch=adb('shell','am','start','-W','-n',PACKAGE+'/.MainActivity').decode();(ROOT/'logs/adb-launch.log').write_text(launch)
 time.sleep(1)
 screenshot('01-dashboard.png')
 for cycle in range(1,11):
  before=set(re.findall(r'HOST LAUNCH ([a-f0-9-]+)',logs()))
  tap_button('Launch Mission Red Horizon')
  launchid=wait_for(lambda text: next(iter(set(re.findall(r'HOST LAUNCH ([a-f0-9-]+)',text))-before),None))
  wait_for(lambda text: 'UNITY_TO_NATIVE RESULT launch='+launchid in text)
  text=logs()
  ack='UNITY_TO_NATIVE ACK launch='+launchid in text
  if not ack:raise RuntimeError('Result received without ACK evidence')
  unitypid=adb('shell','pidof',PACKAGE+':unity').decode().strip()
  hostpid=adb('shell','pidof',PACKAGE).decode().strip()
  if cycle==1:screenshot('02-unity-message.png')
  tap_button('Return to AstroGateway dashboard')
  wait_for(lambda text: 'HOST DASHBOARD_RETURN launch='+launchid in text and 'HOST RESULT launch='+launchid in text)
  time.sleep(.5)
  if cycle in (1,10):screenshot(f'03-dashboard-result-cycle-{cycle}.png')
  result['cycles'].append({'cycle':cycle,'launch_id':launchid,'ack':True,'result':True,'dashboard_return':True,'host_pid':hostpid,'unity_pid':unitypid})
  print('PASS cycle',cycle,flush=True)
 result['passed']=True
except Exception as error:
 result['passed']=False;result['error']=str(error);print('FAIL:',error,flush=True)
finally:
 (ROOT/'logs/device-logcat.log').write_text(logs())
 (ROOT/'logs/device-smoke-result.json').write_text(json.dumps(result,indent=2))
 try:adb('shell','rm','/sdcard/astrogateway-poc-ui.xml')
 except Exception:pass
if not result['passed']:raise SystemExit(1)
