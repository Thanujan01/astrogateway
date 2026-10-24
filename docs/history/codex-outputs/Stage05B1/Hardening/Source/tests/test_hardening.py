"""Local-only security tests. Synthetic Auth transport; TLS memory BIO uses real OpenSSL."""
from pathlib import Path
import ssl,json,base64,time,hmac,hashlib
import pytest,httpx
from fastapi.testclient import TestClient
from main import Settings,create_app,_strict_bool
from database import verified_ssl_context,enforce_role_boundary
from test_security_contract import Repo,U,S,URL,setup,token
ROOT=Path('work/stage05b1-hardening/tls')
DSN='postgresql://astrogateway_runtime@localhost/postgres'
CA=ROOT/'ca.crt'

def handshake(context,hostname):
    serverctx=ssl.SSLContext(ssl.PROTOCOL_TLS_SERVER)
    serverctx.load_cert_chain(ROOT/'leaf.crt',ROOT/'leaf.key')
    ci,co,si,so=[ssl.MemoryBIO() for _ in range(4)]
    c=context.wrap_bio(ci,co,server_side=False,server_hostname=hostname)
    s=serverctx.wrap_bio(si,so,server_side=True)
    cd=sd=False
    for _ in range(20):
        try:c.do_handshake();cd=True
        except ssl.SSLWantReadError:pass
        b=co.read()
        if b:si.write(b)
        try:s.do_handshake();sd=True
        except ssl.SSLWantReadError:pass
        b=so.read()
        if b:ci.write(b)
        if cd and sd:return c.version()
    raise AssertionError('Handshake did not complete')

def test_tls_verified_chain_and_hostname():
    ctx=verified_ssl_context(DSN,str(CA))
    assert ctx.check_hostname and ctx.verify_mode==ssl.CERT_REQUIRED
    assert ctx.verify_flags & ssl.VERIFY_X509_STRICT
    assert handshake(ctx,'localhost') in ('TLSv1.2','TLSv1.3')

def test_tls_wrong_hostname_rejected():
    with pytest.raises(ssl.SSLCertVerificationError):handshake(verified_ssl_context(DSN,str(CA)),'wrong.example')

def test_tls_untrusted_chain_rejected():
    # Real approved Supabase CA cannot trust the disposable local server.
    ctx=verified_ssl_context(DSN,'/Users/kapilash/Projects/AstroGatewayAndroid/backend/certs/supabase-ca.crt',True)
    with pytest.raises(ssl.SSLCertVerificationError):handshake(ctx,'localhost')

@pytest.mark.parametrize('dsn',[DSN+'?sslmode=disable',DSN+'?sslmode=prefer',DSN+'?sslmode=require',DSN+'?sslrootcert=other',DSN+'?sslmode=verify-full&sslmode=disable','postgresql://postgres@localhost/postgres','postgresql:///postgres'])
def test_insecure_or_admin_configuration_rejected(dsn):
    with pytest.raises(ValueError):verified_ssl_context(dsn,str(CA))

def test_missing_ca_rejected():
    with pytest.raises(ValueError):verified_ssl_context(DSN,'')

def test_legacy_exception_pinned():
    with pytest.raises(ValueError):verified_ssl_context(DSN,str(CA),True)
    path='/Users/kapilash/Projects/AstroGatewayAndroid/backend/certs/supabase-ca.crt'
    strict=verified_ssl_context(DSN,path)
    legacy=verified_ssl_context(DSN,path,True)
    assert strict.verify_flags & ssl.VERIFY_X509_STRICT
    assert not legacy.verify_flags & ssl.VERIFY_X509_STRICT
    assert legacy.verify_mode==ssl.CERT_REQUIRED and legacy.check_hostname

@pytest.mark.parametrize('value',['1','yes','TRUE',''])
def test_legacy_boolean_requires_explicit_value(value):
    with pytest.raises(ValueError):_strict_bool(value)

def test_cross_project_issuer_forbidden():
    with pytest.raises(ValueError):Settings(URL,'sb_publishable_fixture',jwt_issuer='https://other.example/auth/v1')

@pytest.mark.parametrize('claims',[{'nbf':int(time.time())+1000},{'exp':str(int(time.time())+300)},{'exp':True}])
def test_claim_types_and_not_before(claims):
    app,_=setup()
    with TestClient(app) as c:assert c.get('/v1/me',headers={'Authorization':'Bearer '+token(**claims)}).status_code==401

def test_duplicate_signed_payload_keys_rejected():
    app,_=setup();parts=token().split('.')
    parts[1]=base64.urlsafe_b64encode(b'{"sub":"one","sub":"two"}').decode().rstrip('=')
    with TestClient(app) as c:assert c.get('/v1/me',headers={'Authorization':'Bearer '+'.'.join(parts)}).status_code==401

def test_signed_fixture_tamper_rejected_by_auth_service():
    # Fixture Auth server validates a real HMAC, never deployed and not a live Supabase test.
    key=b'local-fixture-only-not-a-real-signing-secret'
    parts=token().split('.');parts[0]=base64.urlsafe_b64encode(b'{"alg":"HS256"}').decode().rstrip('=')
    signing='.'.join(parts[:2])
    signed=signing+'.'+base64.urlsafe_b64encode(hmac.new(key,signing.encode(),hashlib.sha256).digest()).decode().rstrip('=')
    def auth(req):
        presented=req.headers['Authorization'].removeprefix('Bearer ');body,signature=presented.rsplit('.',1)
        expected=base64.urlsafe_b64encode(hmac.new(key,body.encode(),hashlib.sha256).digest()).decode().rstrip('=')
        if not hmac.compare_digest(signature,expected):return httpx.Response(401)
        return httpx.Response(200,json={'id':str(U),'email_confirmed_at':'2026-01-01'})
    app=create_app(Settings(URL,'sb_publishable_fixture'),httpx.MockTransport(auth),Repo())
    with TestClient(app) as c:
        assert c.get('/v1/me',headers={'Authorization':'Bearer '+signed}).status_code==200
        assert c.get('/v1/me',headers={'Authorization':'Bearer '+signed[:-2]+'xx'}).status_code==401

class BoundaryConnection:
    def __init__(self,unsafe=None):self.unsafe=unsafe;self.role='astrogateway_runtime'
    async def fetch(self,query,*args):
        return [{'rolname':'astrogateway_api','admin_option':self.unsafe=='admin_membership','inherit_option':False,'set_option':True}]
    async def fetchrow(self,query,name):
        return dict(rolcanlogin=name=='astrogateway_runtime',rolinherit=False,rolsuper=self.unsafe=='superuser',rolbypassrls=self.unsafe=='bypass',rolcreaterole=self.unsafe=='createrole',rolcreatedb=self.unsafe=='createdb',rolreplication=self.unsafe=='replication')
    async def fetchval(self,query,*args):
        if query=='SELECT current_user':return self.role
        if 'count(*)' in query:return 3
        if 'pg_auth_members' in query:return self.unsafe=='api_membership'
        if 'relowner' in query:return self.unsafe=='owner'
        if 'has_database_privilege' in query:return self.unsafe=='db_create'
        if 'pg_namespace' in query:return self.unsafe=='unrelated_schema'
        if 'has_schema_privilege' in query:return self.unsafe=='training_create'
        raise AssertionError(query)
    async def execute(self,query):self.role='astrogateway_api' if query=='SET ROLE astrogateway_api' else 'astrogateway_runtime'

@pytest.mark.asyncio
async def test_role_guard_safe_contract():await enforce_role_boundary(BoundaryConnection())
@pytest.mark.asyncio
@pytest.mark.parametrize('unsafe',['admin_membership','superuser','bypass','createrole','createdb','replication','api_membership','owner','db_create','unrelated_schema','training_create'])
async def test_role_guard_fails_closed(unsafe):
    with pytest.raises(RuntimeError):await enforce_role_boundary(BoundaryConnection(unsafe))
