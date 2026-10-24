# Active JWT regression cases retained; retired raw-DB tests preserved in Before/.
import ssl,json,base64,time,hmac,hashlib
import pytest,httpx
from fastapi.testclient import TestClient
from main import Settings,create_app,_strict_bool
from test_security_contract import Repo,U,S,URL,setup,token
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


@pytest.mark.parametrize('dsn',['postgresql://astrogateway_runtime@localhost/postgres','postgresql://postgres@localhost/postgres'])
def test_raw_database_credentials_forbidden(dsn):
    with pytest.raises(ValueError):Settings(URL,'sb_publishable_fixture',dsn)

def test_retired_tls_override_forbidden():
    with pytest.raises(ValueError):Settings(URL,'sb_publishable_fixture',database_ssl_root_cert='cert.crt')
    with pytest.raises(ValueError):Settings(URL,'sb_publishable_fixture',database_allow_legacy_ca=True)
