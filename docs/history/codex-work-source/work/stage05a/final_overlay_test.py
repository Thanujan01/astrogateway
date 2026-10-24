exec(open('work/stage05a/validate.py').read().split("r={'screens'")[0])
import hashlib
r={'cycles':[],'passed':False};stream=open(O/'final-overlay-logcat.log','w');proc=subprocess.Popen([A,'-s','emulator-5556','logcat','-v','threadtime','-T','1','AstroGatewayPOC:I','AstroGatewayUI:I','Unity:I','AndroidRuntime:E','DEBUG:E','libc:F','*:S'],stdout=stream,stderr=subprocess.STDOUT)
def log():return (O/'final-overlay-logcat.log').read_text(errors='replace')
try:
 apk=Path('/Users/kapilash/Projects/AstroGatewayAndroid/app/build/outputs/apk/debug/app-debug.apk');r['apk_sha256']=hashlib.sha256(apk.read_bytes()).hexdigest();r['installation']=adb('install','-r',str(apk),timeout=110);adb('shell','am','start','-W','-n',P+'/.MainActivity');time.sleep(2);tap('Explore demo dashboard');hp=adb('shell','pidof',P).strip();up=None
 for cycle,method in enumerate(['button','gesture'],1):
  tap('Continue training');before=set(re.findall(r'HOST LAUNCH ([a-f0-9-]+)',log()));tap('Launch simulator preview');wait(lambda l:bool(set(re.findall(r'HOST LAUNCH ([a-f0-9-]+)',l))-before));ident=next(iter(set(re.findall(r'HOST LAUNCH ([a-f0-9-]+)',log()))-before));wait(lambda l:'UNITY_RECEIVED '+ident in l and 'UNITY_TO_NATIVE ACK launch='+ident in l and 'UNITY_TO_NATIVE RESULT launch='+ident in l);time.sleep(1)
  x=ui()
  if 'Got it' in x:tap('Got it')
  b=next(n for n in nodes() if n.get('text')=='Return to AstroGateway dashboard');bounds=list(map(int,re.findall(r'\d+',b.get('bounds'))));shot('11-unity-bridge');assert bounds[3]-bounds[1]>=145,('Return target clipped',bounds)
  uid=adb('shell','pidof',P+':unity').strip();up=up or uid
  if method=='button':tap('Return to AstroGateway dashboard')
  else:adb('shell','input','swipe','60','540','550','540','300');time.sleep(1)
  wait(lambda l:'HOST DASHBOARD_RETURN launch='+ident in l);expect('Your next horizon.');shot('16-final-dashboard-return-'+method);r['cycles'].append({'launch_id':ident,'return':method,'ack':True,'result':True,'return_target_bounds':bounds,'host_reused':adb('shell','pidof',P).strip()==hp,'unity_reused':uid==up});print('PASS final',method,flush=True)
 r['passed']=True
except Exception as e:r['error']=str(e);print('FAIL',e,flush=True)
finally:
 proc.terminate();proc.wait(timeout=5);stream.close();r['fatal_exception']='FATAL EXCEPTION' in log();r['fatal_signal']='Fatal signal' in log();r['passed']=r['passed'] and not r['fatal_exception'] and not r['fatal_signal'];(O/'final-overlay-validation.json').write_text(json.dumps(r,indent=2));print(json.dumps(r,indent=2))
