import subprocess,time,re,json,xml.etree.ElementTree as ET
from pathlib import Path
A='/Applications/Unity/Hub/Editor/6000.6.4f1/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb';P='com.sk2lb.astrogateway.poc';O=Path('outputs/Stage05A');S=O/'screenshots'
def adb(*x,binary=False,timeout=40):
 r=subprocess.run([A,'-s','emulator-5556',*x],capture_output=True,timeout=timeout);r.check_returncode();return r.stdout if binary else r.stdout.decode()
def ui():
 adb('shell','uiautomator','dump','/sdcard/stage05a.xml');return adb('shell','cat','/sdcard/stage05a.xml')
def nodes():return list(ET.fromstring(ui()).iter('node'))
def shot(n): (S/(n+'.png')).write_bytes(adb('exec-out','screencap','-p',binary=True))
def swipe(up=True):adb('shell','input','swipe','500','1700' if up else '700','500','700' if up else '1700','350')
def tap(label):
 for i in range(7):
  for n in nodes():
   if n.get('package')==P and n.get('text','').casefold()==label.casefold() and n.get('enabled')=='true':
    b=list(map(int,re.findall(r'\d+',n.get('bounds'))));
    if b[3]>b[1] and b[2]>b[0]:adb('shell','input','tap',str((b[0]+b[2])//2),str((b[1]+b[3])//2));time.sleep(.6);return
  swipe()
 raise Exception('Button absent '+label)
def expect(text):
 x=ui();assert text in x,(text,x[-500:]);return x
def back():adb('shell','input','keyevent','KEYCODE_BACK');time.sleep(.5)
def top():
 for _ in range(3):swipe(False)
def log():return (O/'runtime-logcat.log').read_text(errors='replace')
def wait(f,t=45):
 end=time.monotonic()+t
 while time.monotonic()<end:
  if f(log()):return
  time.sleep(.3)
 raise Exception('Log timeout')
r={'screens':[],'navigation':[],'cycles':[],'passed':False}
stream=open(O/'runtime-logcat.log','w');proc=subprocess.Popen([A,'-s','emulator-5556','logcat','-v','threadtime','-T','1','AstroGatewayPOC:I','AstroGatewayUI:I','Unity:I','AndroidRuntime:E','DEBUG:E','libc:F','*:S'],stdout=stream,stderr=subprocess.STDOUT)
try:
 r['installation']=adb('install','-r','/Users/kapilash/Projects/AstroGatewayAndroid/app/build/outputs/apk/debug/app-debug.apk',timeout=110);r['model']=adb('shell','getprop','ro.product.model').strip();r['android']=adb('shell','getprop','ro.build.version.release').strip()
 adb('shell','am','force-stop',P);r['host_launch']=adb('shell','am','start','-W','-n',P+'/.MainActivity');time.sleep(.4);shot('01-splash');r['screens'].append('Splash');time.sleep(2)
 expect('Prepare for a world beyond.');shot('02-welcome');r['screens'].append('Welcome')
 tap('Create account');expect('Create your profile');shot('03-sign-up');r['screens'].append('Sign up');back();expect('Prepare for a world beyond.');r['navigation'].append('Sign up → Welcome via Back')
 tap('Sign in');expect('Welcome back');shot('04-sign-in');r['screens'].append('Sign in');tap('Authentication unavailable');expect('Preview only');shot('authentication-unavailable');tap('Understood');tap('Use demo workspace');expect('Your next horizon.');shot('05-dashboard');r['screens'].append('Dashboard')
 for button,title,name in [('Astronaut profile','Astronaut profile','08-profile'),('Training progress','Training progress','09-progress'),('Settings','Settings','10-settings')]:
  tap(button);expect(title);shot(name);r['screens'].append(title);back();expect('Your next horizon.');r['navigation'].append(title+' → Dashboard via Back')
 tap('Browse missions');expect('Mission control');shot('06-missions');r['screens'].append('Missions');tap('Review Mission Red Horizon');expect('SIMULATOR CONNECTION');shot('07-launch');r['screens'].append('Launch');back();expect('Mission control');back();expect('Your next horizon.');r['navigation'].append('Launch → Missions → Dashboard via Back')
 initialhost=adb('shell','pidof',P).strip();unitypid=None
 for cycle,method in enumerate(['button','key','gesture'],1):
  tap('Continue training');before=set(re.findall(r'HOST LAUNCH ([a-f0-9-]+)',log()));tap('Launch simulator preview');wait(lambda l:bool(set(re.findall(r'HOST LAUNCH ([a-f0-9-]+)',l))-before));ident=next(iter(set(re.findall(r'HOST LAUNCH ([a-f0-9-]+)',log()))-before))
  wait(lambda l:'UNITY_RECEIVED '+ident in l and 'UNITY_TO_NATIVE ACK launch='+ident in l and 'UNITY_TO_NATIVE RESULT launch='+ident in l)
  time.sleep(1)
  x=ui()
  if 'Got it' in x:tap('Got it')
  if cycle==1:shot('11-unity-bridge')
  if method=='button':tap('Return to AstroGateway dashboard')
  elif method=='key':back()
  else:adb('shell','input','swipe','60','540','550','540','300');time.sleep(1)
  wait(lambda l:'HOST DASHBOARD_RETURN launch='+ident in l,12);expect('Your next horizon.');top();shot('12-dashboard-return-'+method)
  up=adb('shell','pidof',P+':unity').strip();unitypid=unitypid or up
  r['cycles'].append({'cycle':cycle,'launch_id':ident,'return_method':method,'native_to_unity':True,'ack':True,'result':True,'dashboard_return':True,'host_pid_preserved':adb('shell','pidof',P).strip()==initialhost,'unity_pid_reused':up==unitypid});print('PASS Unity',cycle,method,flush=True)
 # Native edge Back and large text resilience.
 tap('Astronaut profile');adb('shell','input','swipe','60','1000','550','1000','300');time.sleep(1);expect('Your next horizon.');r['navigation'].append('Profile → Dashboard via edge gesture')
 adb('shell','settings','put','system','font_scale','1.3');time.sleep(1);shot('13-dashboard-large-text');tap('Settings');expect('Settings');shot('14-settings-large-text');back();adb('shell','settings','put','system','font_scale','1.0')
 r['passed']=True
except Exception as e:
 r['error']=str(e);print('FAIL',e,flush=True)
 try:shot('failure');(O/'failure.xml').write_text(ui())
 except:pass
finally:
 adb('shell','settings','put','system','font_scale','1.0');time.sleep(.5);proc.terminate();proc.wait(timeout=5);stream.close();r['fatal_exception']='FATAL EXCEPTION' in log();r['fatal_signal']='Fatal signal' in log();(O/'exit-info.log').write_text(adb('shell','dumpsys','activity','exit-info',P));(O/'system-errors.log').write_text(adb('logcat','-d','-s','ActivityManager:E','ActivityTaskManager:E','*:S'));r['anr']=any(P in l and 'ANR' in l for l in (O/'system-errors.log').read_text().splitlines());r['passed']=r['passed'] and not r['fatal_exception'] and not r['fatal_signal'] and not r['anr'];(O/'validation.json').write_text(json.dumps(r,indent=2));print(json.dumps(r,indent=2),flush=True)
