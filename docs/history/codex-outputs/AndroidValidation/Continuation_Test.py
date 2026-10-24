from pathlib import Path
import json,subprocess,time,datetime,re,shutil
helper=Path('/Users/kapilash/Documents/Codex/2026-10-08/task-01-create-unity-6-urp/work/android-validation/runtime_test.py').read_text().split('started=datetime.datetime')[0]
exec(compile(helper,'runtime_test_helpers','exec'))
archive=RUN/'Back_Navigation_Attempt';archive.mkdir(exist_ok=True)
for f in RUN.iterdir():
 if f.is_file():shutil.copy2(f,archive/f.name)
result=json.loads((RUN/'runtime-result.json').read_text())
assert len(result['cycles'])==5 and sum(c['passed'] for c in result['cycles'])==4
result['test_interruptions']=[{'reason':'Injected Android Back key did not return from Unity','cycle':5,'error':result.pop('error',None)},{'reason':'Edge-swipe Back gesture also left Unity visible','evidence':'back-gesture.xml'}]
result['back_navigation']={'injected_key_passed':False,'edge_gesture_passed':False,'remaining_issue':'Android Back did not invoke native dashboard return; explicit native button works'}
initial_host_pid=result['cycles'][0]['host_pid'];initial_unity_pid=result['cycles'][0]['unity_pid']
stream=open(RUN/'runtime-logcat.log','a')
logger=subprocess.Popen([ADB,'-s',args.serial,'logcat','-v','threadtime','-T','1','AstroGatewayPOC:I','Unity:I','AndroidRuntime:E','DEBUG:E','libc:F','*:S'],stdout=stream,stderr=subprocess.STDOUT)
try:
 entry=result['cycles'][4];launchid=entry['launch_id']
 tap('Return to AstroGateway dashboard')
 wait_log(lambda t: 'HOST DASHBOARD_RETURN launch='+launchid in t and 'HOST RESULT launch='+launchid in t)
 xml=wait_dashboard_result();(RUN/'03-dashboard-cycle-5.xml').write_text(xml);screenshot('03-dashboard-result-cycle-5.png')
 entry.update({'return_method':'Native return button after failed Back tests','back_attempt_failed':True,'dashboard_return':True,'result_visibly_displayed':True,'passed':True})
 print('PASS cycle 5 Native return button; Back issue recorded',flush=True)
 for cycle in range(6,11):
  entry={'cycle':cycle,'passed':False};result['cycles'].append(entry);before=set(re.findall(r'HOST LAUNCH ([a-f0-9-]+)',logtext()));begin=time.monotonic()
  tap('Launch Mission Red Horizon')
  launchid=wait_log(lambda t: next(iter(set(re.findall(r'HOST LAUNCH ([a-f0-9-]+)',t))-before),None));entry['launch_id']=launchid
  wait_log(lambda t: 'UNITY_RECEIVED '+launchid in t)
  wait_log(lambda t: 'UNITY_TO_NATIVE ACK launch='+launchid in t)
  wait_log(lambda t: 'UNITY_TO_NATIVE RESULT launch='+launchid in t)
  find_button('Return to AstroGateway dashboard')
  hostpid=adb('shell','pidof',PKG).strip();unitypid=adb('shell','pidof',PKG+':unity').strip()
  if hostpid!=initial_host_pid or not unitypid:raise RuntimeError('Host restarted or Unity process missing')
  entry.update({'native_to_unity':True,'unity_ack_to_native':True,'unity_result_to_native':True,'host_pid':hostpid,'unity_pid':unitypid,'unity_process_reused':unitypid==initial_unity_pid,'return_method':'Native return button'})
  tap('Return to AstroGateway dashboard')
  wait_log(lambda t: 'HOST DASHBOARD_RETURN launch='+launchid in t and 'HOST RESULT launch='+launchid in t)
  xml=wait_dashboard_result()
  if adb('shell','pidof',PKG).strip()!=initial_host_pid:raise RuntimeError('Host restarted after dashboard return')
  if cycle==10:
   (RUN/'03-dashboard-cycle-10.xml').write_text(xml);screenshot('03-dashboard-result-cycle-10.png')
  entry.update({'dashboard_return':True,'result_visibly_displayed':True,'duration_seconds':round(time.monotonic()-begin,2),'passed':True})
  print('PASS cycle',cycle,flush=True)
 result['host_process_preserved']=True;result['unity_process_reused_all_cycles']=all(c.get('unity_process_reused') for c in result['cycles'])
 result['unity_relaunch_callbacks']=len(re.findall(r'UNITY_ACTIVITY_RELAUNCH',logtext()))
 result['passed']=True
except Exception as error:
 result['passed']=False;result['error']=str(error);print('FAIL:',error,flush=True)
finally:
 logger.terminate()
 try:logger.wait(timeout=5)
 except subprocess.TimeoutExpired:logger.kill();logger.wait()
 stream.close()
 content=logtext()
 if 'FATAL EXCEPTION' in content and 'Process: '+PKG in content:result['crashes_observed'].append('Java crash')
 if 'Fatal signal' in content and PKG in content:result['crashes_observed'].append('Native fatal signal')
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
