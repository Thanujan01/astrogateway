"""Run synthetic credential-boundary scenarios; no real configuration or network access."""
from pathlib import Path
import importlib.util,tempfile,os,json,shutil
root=Path(__file__).resolve().parents[3];p=Path(__file__).with_name('check_credentials.py');spec=importlib.util.spec_from_file_location('credential_check',p);m=importlib.util.module_from_spec(spec);spec.loader.exec_module(m)
key='sb_publishable_synthetic_fixture_for_validation_only'
base={m.BACKEND:{'ASTROGATEWAY_ENVIRONMENT':'test','ASTROGATEWAY_APPROVED_PROJECT_REF':m.REF,'SUPABASE_URL':m.URL,'SUPABASE_PUBLISHABLE_KEY':key,'SUPABASE_JWT_ISSUER':m.URL+'/auth/v1','CORS_ALLOWED_ORIGINS':'','DATABASE_URL':'','DATABASE_SSL_ROOT_CERT':'','DATABASE_ALLOW_LEGACY_CA':'false'},m.WEB:{'VITE_SUPABASE_URL':m.URL,'VITE_SUPABASE_PUBLISHABLE_KEY':key,'VITE_AUTH_API_BASE':'/account-api'},m.ANDROID:{'supabase.url':m.URL,'supabase.publishableKey':key,'auth.backendUrl':'http://10.0.2.2:8000'}}
results=[]
def scenario(name, change=None, mode=None, duplicate=False, symlink=False, expect=True):
 with tempfile.TemporaryDirectory() as temp:
  r=Path(temp);configs={k:v.copy() for k,v in base.items()}
  if change:change(configs)
  for namepath,values in configs.items():
   dest=r/namepath;dest.parent.mkdir(parents=True,exist_ok=True);dest.write_text('\n'.join(k+'='+v for k,v in values.items())+'\n');dest.chmod(0o600)
  target=r/m.BACKEND
  if mode:target.chmod(mode)
  if duplicate:
   with target.open('a') as out:out.write('SUPABASE_URL='+m.URL+'\n')
  if symlink:
   real=target.with_suffix('.fixture');target.rename(real);target.symlink_to(real)
  checks,actual=m.audit(r);passed=(actual is not None)==expect;assert passed,name;results.append({'scenario':name,'passed':passed})
scenario('valid test-only fixture')
scenario('secret key rejected',lambda c:[d.update({k:'sb_secret_synthetic_fixture_only'}) for d,k in [(c[m.BACKEND],'SUPABASE_PUBLISHABLE_KEY'),(c[m.WEB],'VITE_SUPABASE_PUBLISHABLE_KEY'),(c[m.ANDROID],'supabase.publishableKey')]],expect=False)
scenario('mismatched client key rejected',lambda c:c[m.WEB].update(VITE_SUPABASE_PUBLISHABLE_KEY=key+'_other'),expect=False)
scenario('wrong project rejected',lambda c:c[m.WEB].update(VITE_SUPABASE_URL='https://different.supabase.co'),expect=False)
scenario('database DSN rejected',lambda c:c[m.BACKEND].update(DATABASE_URL='postgresql://fixture.invalid/example'),expect=False)
scenario('production marker rejected',lambda c:c[m.BACKEND].update(ASTROGATEWAY_ENVIRONMENT='production'),expect=False)
scenario('issuer mismatch rejected',lambda c:c[m.BACKEND].update(SUPABASE_JWT_ISSUER='https://different.invalid/auth/v1'),expect=False)
scenario('world-readable file rejected',mode=0o644,expect=False)
scenario('duplicate key rejected',duplicate=True,expect=False)
scenario('symlink rejected',symlink=True,expect=False)
scenario('unknown field rejected',lambda c:c[m.BACKEND].update(DB_ADMIN_PASSWORD='synthetic_fixture_only'),expect=False)
scenario('non-emulator HTTP rejected',lambda c:c[m.ANDROID].update({'auth.backendUrl':'http://192.0.2.1:8000'}),expect=False)
scenario('partial key configuration rejected',lambda c:c[m.WEB].update(VITE_SUPABASE_PUBLISHABLE_KEY=''),expect=False)
print('Credential audit scenarios passed:',len(results))
