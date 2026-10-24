import argparse,datetime,hashlib,json,re,subprocess,time,xml.etree.ElementTree as ET
from pathlib import Path

parser=argparse.ArgumentParser();parser.add_argument('--serial',required=True);parser.add_argument('--skip-install',action='store_true');args=parser.parse_args()
ROOT=Path('/Users/kapilash/Projects/AstroGatewayAndroid')
RUN=ROOT/'RuntimeValidation';RUN.mkdir(exist_ok=True)
ADB='/Applications/Unity/Hub/Editor/6000.6.4f1/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb'
PKG='com.sk2lb.astrogateway.poc';APK=ROOT/'app/build/outputs/apk/debug/app-debug.apk'

def adb(*commands,timeout=40,binary=False,check=True):
 r=subprocess.run([ADB,'-s',args.serial,*commands],capture_output=True,timeout=timeout)
 if check and r.returncode:raise RuntimeError('ADB failed: '+r.stderr.decode(errors='replace')+' '+r.stdout.decode(errors='replace'))
 return r.stdout if binary else r.stdout.decode(errors='replace')

def ui():
 adb('shell','uiautomator','dump','/sdcard/astrogateway-validation.xml',timeout=25)
 xml=adb('shell','cat','/sdcard/astrogateway-validation.xml')
 return ET.fromstring(xml),xml

def find_button(label,seconds=20):
 deadline=time.monotonic()+seconds
 while time.monotonic()<deadline:
  try:
   root,xml=ui()
   for n in root.iter('node'):
    if n.attrib.get('package')==PKG and n.attrib.get('text','').lower()==label.lower() and n.attrib.get('enabled')=='true':
     return n,xml
  except (RuntimeError,ET.ParseError,subprocess.TimeoutExpired):pass
  time.sleep(.5)
 raise RuntimeError('Expected native button not found: '+label)

def tap(label):
 node,_=find_button(label)
 bounds=list(map(int,re.findall(r'\d+',node.attrib['bounds'])))
 adb('shell','input','tap',str((bounds[0]+bounds[2])//2),str((bounds[1]+bounds[3])//2))

def screenshot(name):
 (RUN/name).write_bytes(adb('exec-out','screencap','-p',binary=True))

def texts(root):return '\n'.join(n.attrib.get('text','') for n in root.iter('node') if n.attrib.get('package')==PKG)

def logtext():return (RUN/'runtime-logcat.log').read_text(errors='replace')

def wait_log(predicate,seconds=50):
 deadline=time.monotonic()+seconds
 while time.monotonic()<deadline:
  content=logtext();answer=predicate(content)
  if answer:return answer
  if 'Process: '+PKG in content and 'FATAL EXCEPTION' in content:raise RuntimeError('App Java crash observed; see runtime logcat')
  time.sleep(.2)
 raise RuntimeError('Timed out waiting for runtime evidence')

def wait_dashboard_result(seconds=20):
 deadline=time.monotonic()+seconds
 while time.monotonic()<deadline:
  try:
   root,xml=ui();visible=texts(root)
   if 'launch mission red horizon' in visible.lower() and 'POC_RESULT_OK' in visible and 'Dashboard returned.' in visible:return xml
  except (RuntimeError,ET.ParseError,subprocess.TimeoutExpired):pass
  time.sleep(.3)
 raise RuntimeError('Dashboard did not visibly display returned result')

started=datetime.datetime.now(datetime.timezone.utc).isoformat()
result={'started_utc':started,'apk_path':str(APK),'apk_sha256':hashlib.sha256(APK.read_bytes()).hexdigest(),'device_model':adb('shell','getprop','ro.product.model').strip(),'android_version':adb('shell','getprop','ro.build.version.release').strip(),'api_level':adb('shell','getprop','ro.build.version.sdk').strip(),'emulator':adb('shell','getprop','ro.kernel.qemu').strip()=='1' or args.serial.startswith('emulator-'),'adb_authorization':'device','cycles':[],'passed':False,'crashes_observed':[],'anrs_observed':[]}
stream=open(RUN/'runtime-logcat.log','w')
logger=subprocess.Popen([ADB,'-s',args.serial,'logcat','-v','threadtime','-T','1','AstroGatewayPOC:I','Unity:I','AndroidRuntime:E','DEBUG:E','libc:F','*:S'],stdout=stream,stderr=subprocess.STDOUT)
try:
 if args.skip_install:
  path=adb('shell','pm','path',PKG).strip().removeprefix('package:')
  installed_hash=adb('shell','sha256sum',path).split()[0]
  if installed_hash!=result['apk_sha256']:raise RuntimeError('Installed APK differs from expected existing APK')
  result['installed_apk_sha256']=installed_hash
  result['apk_installation']='success in initial attempt; same installed APK reused'
 else:
  install=adb('install','-r',str(APK),timeout=100);(RUN/'apk-install.log').write_text(install)
  if 'Success' not in install:raise RuntimeError('Installation did not report Success')
  result['apk_installation']='success'
 launch=adb('shell','am','start','-W','-n',PKG+'/.MainActivity','-f','0x20020000');(RUN/'host-launch.log').write_text(launch)
 if 'Error:' in launch or 'Status: ok' not in launch:raise RuntimeError('Host Activity failed to launch')
 _,xml=find_button('Launch Mission Red Horizon');(RUN/'01-dashboard.xml').write_text(xml)
 if 'AstroGateway' not in xml:raise RuntimeError('Dashboard title absent')
 screenshot('01-native-dashboard.png');result['host_launch']='visually verified'
 initial_host_pid=adb('shell','pidof',PKG).strip();initial_unity_pid=None
 for cycle in range(1,11):
  entry={'cycle':cycle,'passed':False};result['cycles'].append(entry);before=set(re.findall(r'HOST LAUNCH ([a-f0-9-]+)',logtext()));begin=time.monotonic()
  tap('Launch Mission Red Horizon')
  launchid=wait_log(lambda t: next(iter(set(re.findall(r'HOST LAUNCH ([a-f0-9-]+)',t))-before),None));entry['launch_id']=launchid
  wait_log(lambda t: 'UNITY_RECEIVED '+launchid in t)
  wait_log(lambda t: 'UNITY_TO_NATIVE ACK launch='+launchid in t)
  wait_log(lambda t: 'UNITY_TO_NATIVE RESULT launch='+launchid in t)
  find_button('Return to AstroGateway dashboard')
  hostpid=adb('shell','pidof',PKG).strip();unitypid=adb('shell','pidof',PKG+':unity').strip()
  if not hostpid or not unitypid:raise RuntimeError('Host or Unity process missing')
  if hostpid!=initial_host_pid:raise RuntimeError('Host restarted during launch cycle')
  if initial_unity_pid is None:initial_unity_pid=unitypid
  entry.update({'native_to_unity':True,'unity_ack_to_native':True,'unity_result_to_native':True,'host_pid':hostpid,'unity_pid':unitypid,'unity_process_reused':unitypid==initial_unity_pid})
  if cycle==1:screenshot('02-unity-bridge-result.png')
  if cycle==5:
   entry['return_method']='Android Back';adb('shell','input','keyevent','KEYCODE_BACK')
  else:
   entry['return_method']='Native return button';tap('Return to AstroGateway dashboard')
  wait_log(lambda t: 'HOST DASHBOARD_RETURN launch='+launchid in t and 'HOST RESULT launch='+launchid in t)
  xml=wait_dashboard_result()
  if adb('shell','pidof',PKG).strip()!=initial_host_pid:raise RuntimeError('Host process changed after dashboard return')
  if cycle in (1,5,10):
   (RUN/f'03-dashboard-cycle-{cycle}.xml').write_text(xml);screenshot(f'03-dashboard-result-cycle-{cycle}.png')
  entry.update({'dashboard_return':True,'result_visibly_displayed':True,'duration_seconds':round(time.monotonic()-begin,2),'passed':True})
  print('PASS cycle',cycle,entry['return_method'],flush=True)
 result['host_process_preserved']=True
 result['unity_process_reused_all_cycles']=all(e.get('unity_process_reused') for e in result['cycles'])
 result['unity_relaunch_callbacks']=len(re.findall(r'UNITY_ACTIVITY_RELAUNCH',logtext()))
 result['passed']=True
except Exception as error:
 result['error']=str(error);print('FAIL:',error,flush=True)
 try:
  root,xml=ui()
  if any(n.attrib.get('package')==PKG for n in root.iter('node')):screenshot('failure-screen.png');(RUN/'failure-ui.xml').write_text(xml)
 except Exception:pass
finally:
 logger.terminate()
 try:logger.wait(timeout=5)
 except subprocess.TimeoutExpired:logger.kill();logger.wait()
 stream.close()
 content=logtext()
 if 'FATAL EXCEPTION' in content and ('Process: '+PKG in content):result['crashes_observed'].append('Java crash in tested application')
 if 'Fatal signal' in content and PKG in content:result['crashes_observed'].append('Native fatal signal in application log window')
 system=adb('logcat','-d','-v','brief','-s','ActivityManager:E','ActivityTaskManager:E','*:S',check=False)
 relevant=[l for l in system.splitlines() if PKG in l];(RUN/'app-system-errors.log').write_text('\n'.join(relevant)+'\n')
 result['anrs_observed']=[l for l in relevant if 'ANR' in l]
 (RUN/'app-exit-info.log').write_text(adb('shell','dumpsys','activity','exit-info',PKG,check=False))
 result['finished_utc']=datetime.datetime.now(datetime.timezone.utc).isoformat()
 if result['crashes_observed'] or result['anrs_observed']:result['passed']=False
 (RUN/'runtime-result.json').write_text(json.dumps(result,indent=2)+'\n')
 adb('shell','rm','/sdcard/astrogateway-validation.xml',check=False)
print(json.dumps(result,indent=2),flush=True)
raise SystemExit(0 if result['passed'] else 1)
