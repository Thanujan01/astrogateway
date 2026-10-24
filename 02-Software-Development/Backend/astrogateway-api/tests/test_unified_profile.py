"""Local mock tests only; no hosted credentials or Auth accounts."""
import httpx,pytest
from uuid import uuid4
from fastapi.testclient import TestClient
from fastapi import HTTPException
from main import Settings,create_app,Principal
from database import Database
from test_security_contract import Repo,U,URL,token
class EditableRepo(Repo):
    def __init__(self):super().__init__();self.name='Fixture';self.calls=[]
    async def profile(self,u):
        row=await super().profile(u);row['display_name']=self.name;return row
    async def update_profile(self,u,name):
        self.calls.append((u.user_id,name));self.name=name;return await self.profile(u)
def setup():
    repo=EditableRepo()
    app=create_app(Settings(URL,'sb_publishable_fixture'),httpx.MockTransport(lambda _:httpx.Response(200,json={'id':str(U),'email_confirmed_at':'2026-01-01'})),repo)
    return app,repo
@pytest.mark.parametrize('name',['  Commander A  ','தமிழ் விண்வெளி வீரர்','🚀🌒','A'*64])
def test_update_and_refresh_same_identity(name):
    app,repo=setup();h={'Authorization':'Bearer '+token()}
    with TestClient(app) as c:
        r=c.post('/v1/me/display-name',headers=h,json={'display_name':name})
        assert r.status_code==200 and r.json()['display_name']==name.strip() and r.json()['user_id']==str(U)
        assert c.get('/v1/me',headers=h).json()['display_name']==name.strip()
    assert repo.calls==[(U,name.strip())]
@pytest.mark.parametrize('body',[{}, {'display_name':'x'},{'display_name':' '*4},{'display_name':'A'*65},{'display_name':'ab\ncd'},{'display_name':'ab\x7f'},{'display_name':None},{'display_name':'Valid','user_id':str(uuid4())},{'display_name':'Valid','training_level':10}])
def test_invalid_or_identity_override_body_rejected(body):
    app,repo=setup()
    with TestClient(app) as c:assert c.post('/v1/me/display-name',headers={'Authorization':'Bearer '+token()},json=body).status_code==422
    assert not repo.calls
@pytest.mark.parametrize('auth',[None,'Bearer broken','Bearer '+token(exp=1),'Bearer '+token(sub=str(uuid4()))])
def test_update_requires_verified_identity(auth):
    app,repo=setup()
    with TestClient(app) as c:assert c.post('/v1/me/display-name',headers={'Authorization':auth} if auth else {},json={'display_name':'Valid'}).status_code==401
    assert not repo.calls

def test_wrong_response_owner_fails_closed():
    app,repo=setup()
    async def bad(u,name):return dict(await repo.profile(u),user_id=uuid4())
    repo.update_profile=bad
    with TestClient(app) as c:assert c.post('/v1/me/display-name',headers={'Authorization':'Bearer '+token()},json={'display_name':'Valid'}).status_code==502

def test_revoked_session_cannot_update():
    app,repo=setup();h={'Authorization':'Bearer '+token()}
    with TestClient(app) as c:
        assert c.post('/v1/auth-session/revoke',headers=h).status_code==200
        assert c.post('/v1/me/display-name',headers=h,json={'display_name':'Valid'}).status_code==401
    assert not repo.calls
@pytest.mark.asyncio
async def test_update_forwards_original_jwt_not_user_id():
    from test_postgrest_repository import user
    principal=user();seen=[]
    def gateway(req):
        seen.append(req);return httpx.Response(200,json={'user_id':str(principal.user_id),'display_name':'New'})
    async with httpx.AsyncClient(transport=httpx.MockTransport(gateway)) as http:
        result=await Database(http,Settings(URL,'sb_publishable_fixture')).update_profile(principal,'New')
    import json
    assert result['user_id']==str(principal.user_id)
    assert seen[0].headers['Authorization']=='Bearer '+principal.access_token
    assert seen[0].headers['Content-Profile']=='mission_api'
    assert json.loads(seen[0].content)=={'p_display_name':'New'}
