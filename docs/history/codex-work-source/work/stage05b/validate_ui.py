import subprocess,time,re,json,xml.etree.ElementTree as ET
from pathlib import Path
A='/Applications/Unity/Hub/Editor/6000.6.4f1/PlaybackEngines/AndroidPlayer/SDK/platform-tools/adb';P='com.sk2lb.astrogateway.poc';O=Path('outputs/Stage05B');S=O/'screenshots'
def adb(*x,binary=False,timeout=40):
 r=subprocess.run([A,'-s','emulator-5556',*x],capture_output=True,timeout=timeout);r.check_returncode();return r.stdout if binary else r.stdout.decode()
def ui():
 for attempt in range(3):
  try:
   adb('shell','uiautomator','dump','/sdcard/stage05b.xml');return adb('shell','cat','/sdcard/stage05b.xml')
  except subprocess.CalledProcessError:
   if attempt==2:raise
   time.sleep(1)
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

S.mkdir(parents=True,exist_ok=True)
r={'live_authentication':False,'checks':[]}
adb('shell','am','force-stop',P)
adb('shell','am','start','-W','-n',P+'/.MainActivity');time.sleep(2)
expect('Prepare for a world beyond.');expect('Account service is not configured');shot('01-welcome')
assert 'Use demo workspace' not in ui();r['checks'].append('No demo entry; unconfigured provider explicit')
tap('Create account');expect('Create your profile');r['checks'].append('Signup opens')
tap('Register account');expect('Enter a valid email');tap('OK');r['checks'].append('Signup empty input rejected locally')
back();expect('Prepare for a world beyond.')
tap('Sign in');expect('Welcome back');tap('Sign in to AstroGateway');expect('Enter your valid email');tap('OK');r['checks'].append('Signin empty input rejected locally')
tap('Forgot password?');expect('Recover your account');shot('02-password-reset')
tap('Send recovery email');expect('Enter a valid email');tap('OK');r['checks'].append('Reset invalid email rejected locally')
back();expect('Welcome back');back();expect('Prepare for a world beyond.');r['checks'].append('System Back reset → signin → Welcome')
adb('shell','am','start','-W','-n',P+'/.MainActivity','--es','launchId','11111111-1111-4111-8111-111111111111','--ez','returned','true');time.sleep(1)
expect('Prepare for a world beyond.');assert 'Your next horizon.' not in ui();r['checks'].append('Forged Unity return does not unlock dashboard')
adb('shell','am','start','-W','-a','android.intent.action.VIEW','-d','astrogateway://auth/callback?code=NONSECRET_INVALID_FIXTURE',P);time.sleep(1)
expect('This link has no pending request');r['checks'].append('Unsolicited PKCE callback denied')
adb('shell','am','force-stop',P);adb('shell','am','start','-W','-n',P+'/.MainActivity');time.sleep(1)
expect('Prepare for a world beyond.');r['checks'].append('Restart remains unauthenticated')
tap('Clear local session');time.sleep(1);expect('Local and training sessions cleared');r['checks'].append('Empty-session signout safe')
shot('03-signed-out')
r['model']=adb('shell','getprop','ro.product.model').strip();r['android']=adb('shell','getprop','ro.build.version.release').strip();r['passed']=True
(O/'navigation-validation.json').write_text(json.dumps(r,indent=2));print(json.dumps(r,indent=2))
