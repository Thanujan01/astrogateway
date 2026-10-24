exec(open('work/stage05a/validate.py').read().split("r={'screens'")[0])
result={}
try:
 top();back();x=ui();result['dashboard_root_back_backgrounds_task']=P not in x
 adb('shell','am','start','-W','-n',P+'/.MainActivity','-f','0x20020000');time.sleep(1);expect('Your next horizon.');result['launcher_reentry_restores_dashboard']=True
 tap('Settings');tap('Reduce interface motion');back();tap('Settings');ns=nodes();result['reduced_motion_preference_persists']=any(n.get('text')=='Reduce interface motion' and n.get('checked')=='true' for n in ns);tap('Reduce interface motion');back()
 adb('shell','wm','size','1920x1200');adb('shell','wm','density','240');time.sleep(2);expect('Your next horizon.');shot('15-dashboard-tablet');result['tablet_dashboard_visible']=True;result['emulated_tablet_size']='1920x1200, density 240 dpi; 1280x800 dp'
finally:
 adb('shell','wm','size','reset');adb('shell','wm','density','reset');time.sleep(1);(O/'responsive-validation.json').write_text(json.dumps(result,indent=2));print(json.dumps(result,indent=2))
