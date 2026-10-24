"""Local-only HTTP server for client integration tests (Unity). NOT for deployment.
Runs the real FastAPI routes with the offline mock Auth transport and the in-memory MissionRepo from
test_mission_commands. Accepts only the synthetic fixture tokens those tests build; binds to 127.0.0.1."""
import sys
from pathlib import Path
import httpx,uvicorn
sys.path.insert(0,str(Path(__file__).resolve().parent.parent));sys.path.insert(0,str(Path(__file__).resolve().parent))
from main import Settings,create_app
from test_sensor_records import SensorRepo
from test_security_contract import U,URL

def build():
    repo=SensorRepo()  # MissionRepo + First Mission sensor records
    def auth(request):
        # Mirrors the fixture: the mock Auth server confirms the fixture user for any well-formed bearer.
        return httpx.Response(200,json={'id':str(U),'email_confirmed_at':'2026-01-01'})
    return create_app(Settings(URL,'sb_publishable_fixture'),httpx.MockTransport(auth),repo)

if __name__=='__main__':
    port=int(sys.argv[1]) if len(sys.argv)>1 else 8766
    uvicorn.run(build(),host='127.0.0.1',port=port,log_level='warning')
