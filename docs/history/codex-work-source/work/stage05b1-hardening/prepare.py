from pathlib import Path
import shutil
root=Path('/Users/kapilash/Projects/AstroGatewayAndroid/backend')
out=Path('outputs/Stage05B1/Hardening/Source'); out.mkdir(parents=True,exist_ok=True)
for name in ('database.py','main.py','run_local.py','.env.example'):
 shutil.copyfile(root/name,out/name)
p=out/'database.py'; s=p.read_text(); s=s.replace('import asyncpg','import asyncpg\nimport hashlib\nimport ssl\nfrom pathlib import Path\nfrom urllib.parse import urlparse, parse_qs')
s=s[:s.index('async def create_restricted_pool')]+"""# Public CA fingerprint, never a credential. Scoped legacy-profile exception only.
LEGACY_CA_SHA256 = '700723581420dd1ac98fd7e9ac529f0ef210eadcaf87fc868a3ad7d114c2f3b7'

def verified_ssl_context(dsn, ca_file, allow_legacy=False):
    parsed=urlparse(dsn)
    if parsed.scheme not in ('postgres','postgresql') or not parsed.hostname or parsed.hostname.startswith('/'):
        raise ValueError('A TCP PostgreSQL endpoint is required.')
    if parsed.username not in ('astrogateway_runtime',) and not (parsed.username or '').startswith('astrogateway_runtime.'):
        raise ValueError('Only the dedicated runtime login is supported.')
    query=parse_qs(parsed.query,keep_blank_values=True)
    if any(k.startswith('ssl') and (k!='sslmode' or v!=['verify-full']) for k,v in query.items()):
        raise ValueError('TLS configuration belongs to the verified runtime context.')
    path=Path(ca_file)
    if not ca_file or not path.is_file():raise ValueError('An explicit approved CA file is required.')
    ctx=ssl.create_default_context(cafile=str(path))
    ctx.minimum_version=ssl.TLSVersion.TLSv1_2
    ctx.verify_mode=ssl.CERT_REQUIRED
    ctx.check_hostname=True
    ctx.verify_flags |= ssl.VERIFY_X509_STRICT
    if allow_legacy:
        if hashlib.sha256(path.read_bytes()).hexdigest()!=LEGACY_CA_SHA256:
            raise ValueError('Legacy profile exception is limited to the reviewed CA.')
        ctx.verify_flags &= ~ssl.VERIFY_X509_STRICT
    return ctx

async def enforce_role_boundary(connection):
    # Fail closed for this dedicated architecture; provider/admin memberships are forbidden.
    login=await connection.fetchval('SELECT current_user')
    if login!='astrogateway_runtime':raise RuntimeError('Unexpected runtime role.')
    members=await connection.fetch('''SELECT parent.rolname,m.admin_option,m.inherit_option,m.set_option
        FROM pg_auth_members m JOIN pg_roles parent ON parent.oid=m.roleid
        JOIN pg_roles child ON child.oid=m.member WHERE child.rolname=$1''',login)
    if len(members)!=1 or members[0]['rolname']!='astrogateway_api' or members[0]['admin_option'] or members[0]['inherit_option'] or not members[0]['set_option']:
        raise RuntimeError('Unexpected role membership options.')
    if await connection.fetchval("SELECT EXISTS(SELECT 1 FROM pg_auth_members WHERE member=(SELECT oid FROM pg_roles WHERE rolname='astrogateway_api'))"):
        raise RuntimeError('API role must not inherit other roles.')
    for name in (login,'astrogateway_api'):
        role=await connection.fetchrow('SELECT rolcanlogin,rolinherit,rolsuper,rolbypassrls,rolcreaterole,rolcreatedb,rolreplication FROM pg_roles WHERE rolname=$1',name)
        if not role or any(role[k] for k in ('rolsuper','rolbypassrls','rolcreaterole','rolcreatedb','rolreplication')) or role['rolinherit'] or role['rolcanlogin']!=(name==login):
            raise RuntimeError('Unsafe database role attributes.')
        if await connection.fetchval('''SELECT EXISTS(SELECT 1 FROM pg_class WHERE relowner=(SELECT oid FROM pg_roles WHERE rolname=$1))
            OR EXISTS(SELECT 1 FROM pg_namespace WHERE nspowner=(SELECT oid FROM pg_roles WHERE rolname=$1))
            OR EXISTS(SELECT 1 FROM pg_proc WHERE proowner=(SELECT oid FROM pg_roles WHERE rolname=$1))
            OR EXISTS(SELECT 1 FROM pg_database WHERE datdba=(SELECT oid FROM pg_roles WHERE rolname=$1))''',name):
            raise RuntimeError('Runtime/API must not own database objects.')
        if await connection.fetchval("SELECT has_database_privilege($1,current_database(),'CREATE') OR has_database_privilege($1,current_database(),'TEMP')",name):
            raise RuntimeError('Runtime/API cannot create schemas or temporary objects.')
        if await connection.fetchval('''SELECT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname NOT IN ('training','pg_catalog','information_schema')
            AND nspname NOT LIKE 'pg_%' AND (has_schema_privilege($1,oid,'USAGE') OR has_schema_privilege($1,oid,'CREATE')))''',name):
            raise RuntimeError('Unrelated schema privileges remain.')
        if await connection.fetchval("SELECT has_schema_privilege($1,'training','CREATE')",name):
            raise RuntimeError('Training schema is not writable by runtime roles.')
    await connection.execute('SET ROLE astrogateway_api')
    try:
        if await connection.fetchval("SELECT current_user")!='astrogateway_api':raise RuntimeError('Role switch failed.')
        if await connection.fetchval("SELECT count(*) FROM pg_class c JOIN pg_namespace n ON n.oid=c.relnamespace WHERE n.nspname='training' AND c.relkind='r' AND c.relrowsecurity")!=3:
            raise RuntimeError('Expected RLS tables missing.')
    finally:await connection.execute('RESET ROLE')

async def create_restricted_pool(dsn, ca_file='', allow_legacy=False):
    pool=None
    try:
        context=verified_ssl_context(dsn,ca_file,allow_legacy)
        pool=await asyncpg.create_pool(dsn,min_size=1,max_size=5,command_timeout=10,ssl=context,statement_cache_size=0)
        async with pool.acquire() as connection:await enforce_role_boundary(connection)
        return pool
    except Exception:
        if pool:await pool.close()
        # Never emit the DSN, password or underlying driver exception.
        raise RuntimeError('Restricted database connection failed. Review server-only credentials, role grants and verified TLS.') from None
"""; p.write_text(s)
p=out/'main.py';s=p.read_text().replace("    jwt_issuer: str=''", "    jwt_issuer: str=''\n    database_ssl_root_cert: str=''\n    database_allow_legacy_ca: bool=False")
s=s.replace("if self.jwt_issuer and not self.jwt_issuer.startswith('https://'):raise ValueError('JWT issuer must be HTTPS.')", "if self.jwt_issuer and self.jwt_issuer!=self.supabase_url.rstrip('/')+'/auth/v1':raise ValueError('JWT issuer must match the configured project.')")
s=s.replace("os.getenv('SUPABASE_JWT_ISSUER',''))", "os.getenv('SUPABASE_JWT_ISSUER',''),os.getenv('DATABASE_SSL_ROOT_CERT',''),_strict_bool(os.getenv('DATABASE_ALLOW_LEGACY_CA','false')))")
s=s.replace('@dataclass(frozen=True)\nclass Settings:', '''def _strict_bool(value):
    if value not in ('true','false'):raise ValueError('Boolean settings require true or false.')
    return value=='true'

def _unique_json(value):
    def pairs(items):
        result={}
        for key,item in items:
            if key in result:raise ValueError('Duplicate JSON key.')
            result[key]=item
        return result
    return json.loads(value,object_pairs_hook=pairs)

@dataclass(frozen=True)
class Settings:''')
s=s.replace("if not self.settings.supabase_url or not self.settings.publishable_key:","if not self.settings.supabase_url or not self.settings.publishable_key:")
s=s.replace("            parts=token.split('.')", "            if not isinstance(token,str) or not 1<=len(token)<=8192:raise ValueError()\n            parts=token.split('.')")
s=s.replace("json.loads(base64.urlsafe_b64decode(p+'='*(-len(p)%4)))","_unique_json(base64.urlsafe_b64decode(p+'='*(-len(p)%4)))")
s=s.replace("            expires=datetime.fromtimestamp(claims['exp'],timezone.utc)","            if type(claims['exp']) is not int:raise ValueError()\n            expires=datetime.fromtimestamp(claims['exp'],timezone.utc)\n            if 'nbf' in claims and (type(claims['nbf']) is not int or claims['nbf']>datetime.now(timezone.utc).timestamp()):raise ValueError()")
s=s.replace("except (ValueError,KeyError,TypeError,OverflowError):", "except (ValueError,KeyError,TypeError,OverflowError,AttributeError,OSError):")
s=s.replace("create_restricted_pool(settings.database_url)","create_restricted_pool(settings.database_url,settings.database_ssl_root_cert,settings.database_allow_legacy_ca)")
p.write_text(s)
p=out/'run_local.py';s=p.read_text().replace("ALLOWED={'SUPABASE_URL'", "ALLOWED={'DATABASE_SSL_ROOT_CERT','DATABASE_ALLOW_LEGACY_CA','SUPABASE_URL'");p.write_text(s)
p=out/'.env.example';p.write_text(p.read_text()+'''\n# Runtime uses an explicit CA context; no plaintext/prefer/require-only fallback.
DATABASE_SSL_ROOT_CERT=/Users/kapilash/Projects/AstroGatewayAndroid/backend/certs/supabase-ca.crt
# Defaults to strict X509 profile. Set true ONLY after reviewing the pinned Root2021 exception.
DATABASE_ALLOW_LEGACY_CA=false
''')
