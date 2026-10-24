import asyncio,base64,json,time
from uuid import UUID,uuid4
from datetime import datetime,timezone,timedelta
import httpx,pytest
from fastapi import HTTPException
from fastapi.testclient import TestClient
from main import Settings,Principal,MissionInit,Completion,create_app
from database import Database
from test_security_contract import URL,U,S,token

def user():return Principal(U,S,datetime.now(timezone.utc)+timedelta(minutes=5),token())
@pytest.mark.asyncio
async def test_repository_sends_original_token_no_identity_overrides():
 calls=[]
 def gateway(request):
  calls.append((request.url.path,dict(request.headers),json.loads(request.content)))
  if request.url.path.endswith('session_revoked'):return httpx.Response(200,json=False)
  if request.url.path.endswith('revoke_session'):return httpx.Response(200,json=True)
  return httpx.Response(200,json={'user_id':str(U)})
 async with httpx.AsyncClient(transport=httpx.MockTransport(gateway)) as http:
  repository=Database(http,Settings(URL,'sb_publishable_fixture'));u=user()
  await repository.revoked(u);await repository.profile(u)
  await repository.initialize(u,MissionInit(launch_id=uuid4(),mission_id='mission-red-horizon-poc'))
  await repository.complete(u,uuid4(),Completion(launch_id=uuid4(),outcome='integration_check'));await repository.revoke(u)
 assert len(calls)==5
 for path,headers,body in calls:
  assert path.startswith('/rest/v1/rpc/') and headers['authorization']=='Bearer '+u.access_token
  assert headers['content-profile']==headers['accept-profile']=='mission_api'
  assert not any(k in body for k in ('user_id','auth_session_id','p_user_id','p_auth_session_id'))
  assert not any(k in headers for k in ('x-user-id','request.jwt.claim.sub','request.jwt.claims'))

@pytest.mark.asyncio
@pytest.mark.parametrize('status,code,expected',[(401,'PGRST301',401),(403,'42501',403),(404,'PT404',404),(409,'PT409',409),(409,'23505',409),(429,None,429),(500,'XX000',503)])
async def test_gateway_errors_sanitized(status,code,expected):
 def fail(request):return httpx.Response(status,json={'code':code,'message':'sensitive-fixture-canary','details':'never display'})
 async with httpx.AsyncClient(transport=httpx.MockTransport(fail)) as http:
  with pytest.raises(HTTPException) as e:await Database(http,Settings(URL,'sb_publishable_fixture')).profile(user())
 assert e.value.status_code==expected and 'sensitive-fixture-canary' not in e.value.detail

@pytest.mark.asyncio
async def test_no_direct_sql_fallback_on_network_failure():
 def offline(request):raise httpx.ConnectError('fixture connection unavailable')
 async with httpx.AsyncClient(transport=httpx.MockTransport(offline)) as http:
  with pytest.raises(HTTPException) as e:await Database(http,Settings(URL,'sb_publishable_fixture')).profile(user())
 assert e.value.status_code==503

@pytest.mark.asyncio
@pytest.mark.parametrize('body',[[],None,'not-a-row'])
async def test_invalid_gateway_shape_rejected(body):
 async with httpx.AsyncClient(transport=httpx.MockTransport(lambda _:httpx.Response(200,json=body))) as http:
  with pytest.raises(HTTPException) as e:await Database(http,Settings(URL,'sb_publishable_fixture')).profile(user())
 assert e.value.status_code==502

@pytest.mark.asyncio
async def test_unbounded_response_and_arbitrary_rpc_rejected():
 async with httpx.AsyncClient(transport=httpx.MockTransport(lambda _:httpx.Response(200,content=b'x'*65537))) as http:
  repository=Database(http,Settings(URL,'sb_publishable_fixture'))
  with pytest.raises(HTTPException):await repository.profile(user())
  with pytest.raises(RuntimeError):await repository.rpc(user(),'set_config',{})

def test_configuration_does_not_require_database_credential():assert Settings(URL,'sb_publishable_fixture').configured

def test_known_admin_file_startup_guard(monkeypatch):
 import main
 class Node:
  def resolve(self):return self
  @property
  def parent(self):return self
  def __truediv__(self,_):return self
  def exists(self):return True
 monkeypatch.setattr(main,'Path',lambda _:Node());monkeypatch.setattr(main.os,'access',lambda *_:True)
 with pytest.raises(RuntimeError,match='isolated service environment'):
  with TestClient(create_app(Settings(URL,'sb_publishable_fixture'))):pass
