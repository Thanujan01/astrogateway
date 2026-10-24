from pathlib import Path
import shutil
project=Path('/Users/kapilash/Projects/AstroGatewayAndroid/backend');out=Path('outputs/Stage05B2/Source');before=Path('outputs/Stage05B2/Before')
for n in ('database.py','main.py','run_local.py','.env.example','requirements.txt'):
 shutil.copyfile(project/n,before/n);shutil.copyfile(project/n,out/n)
(out/'database.py').write_text('''"""User-scoped PostgREST repository. No SQL pool, DB password or identity-setting API."""
from fastapi import HTTPException

class Database:
    def __init__(self,http,settings):self.http,self.settings=http,settings
    async def rpc(self,user,name,body):
        if name not in ('session_revoked','current_profile','initialize_mission','complete_mission','revoke_session'):
            raise RuntimeError('Unsupported data operation.')
        # Only the ORIGINAL user token crosses the independent JWT gateway.
        # Never forward incoming headers, synthesized claims, role/user/session overrides.
        try:
            response=await self.http.post(self.settings.supabase_url+'/rest/v1/rpc/'+name,
                headers={'apikey':self.settings.publishable_key,'Authorization':'Bearer '+user.access_token,
                         'Content-Profile':'mission_api','Accept-Profile':'mission_api'},json=body)
        except Exception:
            raise HTTPException(503,'Training data service unavailable.') from None
        if len(response.content)>65536:raise HTTPException(502,'Invalid training data response.')
        if response.status_code>=400:
            # SQL business codes are fixed constants. Never echo gateway/detail/log tokens.
            try:code=response.json().get('code')
            except (ValueError,AttributeError):code=None
            if response.status_code==401 or code=='PT401':raise HTTPException(401,'Session invalid or expired.')
            if code=='PT404':raise HTTPException(404,'Mission or profile unavailable.')
            if code in ('PT409','23505'):raise HTTPException(409,'Mission launch/session conflict.')
            if response.status_code==403 or code=='42501':raise HTTPException(403,'Training authorization failed.')
            if response.status_code==429:raise HTTPException(429,'Too many requests. Retry later.')
            raise HTTPException(503,'Training data service unavailable.')
        if response.status_code!=200:raise HTTPException(502,'Invalid training data response.')
        try:return response.json()
        except ValueError:raise HTTPException(502,'Invalid training data response.') from None
    async def revoked(self,user):
        value=await self.rpc(user,'session_revoked',{})
        if type(value) is not bool:raise HTTPException(502,'Invalid training data response.')
        return value
    async def profile(self,user):
        row=await self.rpc(user,'current_profile',{})
        if not isinstance(row,dict):raise HTTPException(502,'Invalid profile response.')
        return row
    async def initialize(self,user,payload):
        row=await self.rpc(user,'initialize_mission',{'p_launch_id':str(payload.launch_id),'p_mission_id':payload.mission_id})
        if not isinstance(row,dict):raise HTTPException(502,'Invalid mission response.')
        return row
    async def complete(self,user,session_id,payload):
        row=await self.rpc(user,'complete_mission',{'p_session_id':str(session_id),'p_launch_id':str(payload.launch_id),'p_outcome':payload.outcome})
        if not isinstance(row,dict):raise HTTPException(502,'Invalid mission response.')
        return row
    async def revoke(self,user):
        if await self.rpc(user,'revoke_session',{}) is not True:raise HTTPException(502,'Invalid revocation response.')
''')
p=out/'main.py';s=p.read_text().replace('from database import Database, create_restricted_pool','from database import Database\nfrom pathlib import Path')
s=s.replace("        if self.database_url and urlparse(self.database_url).scheme not in ('postgres','postgresql'):\n            raise ValueError('A server-only PostgreSQL DSN is required.')", "        if self.database_url:raise ValueError('Direct database credentials are forbidden in the user API.')\n        if self.database_allow_legacy_ca or self.database_ssl_root_cert:\n            raise ValueError('Database TLS settings are retired; the user API uses verified HTTPS only.')")
s=s.replace('bool(self.supabase_url and self.publishable_key and self.database_url)','bool(self.supabase_url and self.publishable_key)')
s=s.replace("        pool=None\n        if test_repository is None and settings.database_url:pool=await create_restricted_pool(settings.database_url,settings.database_ssl_root_cert,settings.database_allow_legacy_ca)", "        if test_repository is None and transport is None and settings.configured:\n            # An operator-owned admin file must not be readable by this service account.\n            # This guard covers the known development layout, not arbitrary secret discovery.\n            admin=Path(__file__).resolve().parent.parent/'database'/'admin.local.env'\n            if admin.exists() and os.access(admin,os.R_OK):\n                raise RuntimeError('Run the user API in an isolated service environment without readable administrative credentials.')")
s=s.replace("app.state.repository=test_repository if test_repository is not None else Database(pool) if pool else None", "app.state.repository=test_repository if test_repository is not None else Database(http,settings) if settings.configured else None")
s=s.replace('        if pool:await pool.close()\n','').replace("version='0.2'","version='0.3'")
p.write_text(s)
p=out/'run_local.py';s=p.read_text().replace("required=['SUPABASE_URL','SUPABASE_PUBLISHABLE_KEY','DATABASE_URL']","required=['SUPABASE_URL','SUPABASE_PUBLISHABLE_KEY']").replace('Fill all three required server settings together','Fill both required server settings together')
a=s.index("            db=urlparse(os.environ['DATABASE_URL'])");b=s.index('        from main import app',a);s=s[:a]+"        if any(os.getenv(k) for k in ('DATABASE_URL','DATABASE_SSL_ROOT_CERT')) or os.getenv('DATABASE_ALLOW_LEGACY_CA','false')!='false':\n            raise ValueError('Remove retired raw-database settings from the user API environment.')\n"+s[b:];s=s.replace('restricted TLS database access','independent gateway and administrative-secret separation');p.write_text(s)
(out/'.env.example').write_text('''# Owner-only TEST config. No database/signing/service-role secrets in the user API.
ASTROGATEWAY_ENVIRONMENT=test
ASTROGATEWAY_APPROVED_PROJECT_REF=
SUPABASE_URL=
SUPABASE_PUBLISHABLE_KEY=
SUPABASE_JWT_ISSUER=
CORS_ALLOWED_ORIGINS=
# Deprecated variables tolerated only as empty/false to detect stale configuration.
DATABASE_URL=
DATABASE_SSL_ROOT_CERT=
DATABASE_ALLOW_LEGACY_CA=false
# mission_api exposed through Data API only after separately approved SQL/configuration.
# training remains private. Run service in a filesystem/OS identity with NO readable admin secrets.
''')
(out/'requirements.txt').write_text((out/'requirements.txt').read_text().replace('asyncpg==0.32.0\n',''))
print('PostgREST backend candidate prepared; direct DB configuration rejected.')
