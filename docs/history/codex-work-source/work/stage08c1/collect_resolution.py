from pathlib import Path
import subprocess,time,json
adb='/Users/kapilash/Projects/AstroGatewayAndroid/EmulatorSDK/platform-tools/adb';pkg='com.sk2lb.astrogateway.stage08cprobe';out=Path('outputs/Stage08C1/Resolution_Trial');storage='/sdcard/Android/data/'+pkg+'/files/'
def cmd(*args):return subprocess.run([adb,'-s','emulator-5554',*args],capture_output=True,timeout=15)
info={k:cmd('shell','getprop',v).stdout.decode().strip() for k,v in {'model':'ro.product.model','android':'ro.build.version.release','api':'ro.build.version.sdk','abi':'ro.product.cpu.abi'}.items()};info['emulator']=True;(out/'Evidence/Android_Device.json').write_text(json.dumps(info,indent=2))
seen=set();deadline=time.monotonic()+155
while time.monotonic()<deadline:
 for i in range(4):
  if i in seen:continue
  raw=cmd('shell','cat',storage+'stage08c_'+str(i)+'.json')
  try:data=json.loads(raw.stdout)
  except Exception:continue
  (out/'Evidence'/f'Android_Profile_{i}.json').write_text(json.dumps(data,indent=2));mem=cmd('shell','dumpsys','meminfo',pkg);(out/'Evidence'/f'Android_Memory_{i}.txt').write_bytes(mem.stdout)
  pic=cmd('pull',storage+f'stage08c_{i}.png',str(out/'Screenshots'/f'Android_{i}_{data["label"]}.png'))
  print('Captured state',i,data['label'],round(data['average_fps'],2),'FPS',flush=True);seen.add(i)
 if len(seen)==4:break
 time.sleep(4)
pid=cmd('shell','pidof',pkg).stdout.decode().strip().split()
if pid:(out/'Evidence/Android_App_Logcat.txt').write_bytes(cmd('logcat','-d','--pid='+pid[0],'-v','threadtime').stdout)
(out/'Evidence/Android_Collection_Status.json').write_text(json.dumps({'captured_states':sorted(seen),'all_four_complete':len(seen)==4},indent=2));print('Collection finished:',sorted(seen),flush=True)
