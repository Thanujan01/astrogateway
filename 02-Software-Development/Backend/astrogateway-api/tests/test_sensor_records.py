"""First Mission sensor-records endpoint. Local mock Auth + in-memory repository only (mirrors candidate SQL 006)."""
import json
from datetime import datetime,timezone,timedelta
from pathlib import Path
from uuid import UUID,uuid4
import httpx,pytest
from fastapi import HTTPException
from fastapi.testclient import TestClient
from main import Settings,create_app
import sensor_records
from sensor_records import LIMITS,record_fingerprint
from test_mission_commands import MissionRepo,H,start
from test_security_contract import U,URL

class SensorRepo(MissionRepo):
    """Ownership, open-session, idempotency (same event_id + same body = duplicate, different body = 409) and cap."""
    def __init__(self):super().__init__();self.sensor={}
    async def submit_sensor_records(self,u,s,records):
        r=self.own(u,s);store=self.sensor.setdefault(s,{});out=[]
        if r['auth_session_id']!=u.auth_session_id or r['status']!='initialized' or r['expires_at']<=datetime.now(timezone.utc):raise HTTPException(409,'Mission launch/session conflict.')
        new=[x for x in records if x.event_id not in store]
        if len(store)+len(new)>LIMITS['max_records_per_session']:raise HTTPException(429,'Sensor record limit reached.')
        for x in records:
            fp=record_fingerprint(x)
            if x.event_id in store:
                if store[x.event_id]['fp']!=fp:raise HTTPException(409,'Sensor record conflict.')
                out.append(dict(session_id=s,event_id=x.event_id,duplicate=True,stored_at=store[x.event_id]['at']))
            else:
                store[x.event_id]=dict(fp=fp,at=datetime.now(timezone.utc));out.append(dict(session_id=s,event_id=x.event_id,duplicate=False,stored_at=store[x.event_id]['at']))
        return out

def setup(confirmed=True):
    repo=SensorRepo()
    def auth(req):return httpx.Response(200,json={'id':str(U),'email_confirmed_at':'2026-01-01' if confirmed else None})
    return create_app(Settings(URL,'sb_publishable_fixture'),httpx.MockTransport(auth),repo),repo

def rec(sensor='terrain.slope',instrument='rover.terrain_profiler',**k):
    base=dict(schema_version=1,event_id=str(uuid4()),observation_id=str(uuid4()),mission_id='ag.mars.red-horizon.surface-ops',mission_version=1,
              build_variant='first_mission_v1',sensor_id=sensor,sensor_type=sensor_records.SENSORS[sensor]['sensor_type'],instrument_id=instrument,
              station_id=None,sim_time_s=42.5,sim_tick=42,captured_at=datetime.now(timezone.utc).isoformat(),
              location=dict(x_m=-15.0,y_m=0.03,z_m=-93.0,frame='scene_local_m'),value=None,value_text=None,
              unit=sensor_records.SENSORS[sensor]['unit'],provenance='SIMULATED',method=sensor_records.SENSORS[sensor]['method'],sim_config_version='fm-sensors-1')
    if sensor_records.SENSORS[sensor]['kind']=='numeric':base['value']=3.5
    base.update(k);return base
P='/v1/mission-sessions/{s}/sensor-records'

@pytest.mark.parametrize('method,path',[('post',P),('get','/v1/science/analysis-status')])
@pytest.mark.parametrize('headers',[{},{'Authorization':'Bearer broken'},H(exp=1),H(role='service_role'),H(sub=str(uuid4()))])
def test_requires_verified_session(method,path,headers):
    app,_=setup()
    with TestClient(app) as c:
        kw={'json':{'records':[rec()]}} if method=='post' else {}
        assert getattr(c,method)(path.format(s=uuid4()),headers=headers,**kw).status_code==401
def test_requires_confirmed_email():
    app,_=setup(confirmed=False)
    with TestClient(app) as c:assert c.post(P.format(s=uuid4()),headers=H(),json={'records':[rec()]}).status_code==403
def test_revoked_session_refused():
    app,_=setup();h=H()
    with TestClient(app) as c:
        s=start(c,h);assert c.post('/v1/auth-session/revoke',headers=h).status_code==200
        assert c.post(P.format(s=s),headers=h,json={'records':[rec()]}).status_code==401

def test_accept_then_retry_is_duplicate_and_analysis_not_connected():
    app,repo=setup();h=H()
    with TestClient(app) as c:
        s=start(c,h);batch={'records':[rec(),rec('terrain.traversability',value_text='nominal'),rec('env.air_temperature','rover.env_package',provenance='UNAVAILABLE',value=None,method='not_modeled')]}
        r=c.post(P.format(s=s),headers=h,json=batch);assert r.status_code==200,r.text
        body=r.json();assert body['accepted']==3 and body['duplicates']==0
        for a in body['records']:
            assert a['analysis']=={'interface_version':'ag.science-analysis.v0','status':'not_connected','model_version':None,'result':None,'confidence':None,'reason':'No scientific analysis model is connected.'}
        r2=c.post(P.format(s=s),headers=h,json=batch);assert r2.status_code==200
        assert r2.json()['accepted']==0 and r2.json()['duplicates']==3 and len(repo.sensor[UUID(s)])==3
def test_same_event_id_different_body_conflicts():
    app,_=setup();h=H()
    with TestClient(app) as c:
        s=start(c,h);a=rec();assert c.post(P.format(s=s),headers=h,json={'records':[a]}).status_code==200
        assert c.post(P.format(s=s),headers=h,json={'records':[{**a,'value':4.0}]}).status_code==409
def test_legacy_alias_mission_id_accepted():
    app,_=setup();h=H()
    with TestClient(app) as c:
        s=start(c,h)
        for alias in ('mission-red-horizon-poc','mission.red_horizon.slice'):assert c.post(P.format(s=s),headers=h,json={'records':[rec(mission_id=alias)]}).status_code==200
def test_analysis_status_endpoint():
    app,_=setup()
    with TestClient(app) as c:
        r=c.get('/v1/science/analysis-status',headers=H());assert r.status_code==200 and r.json()['status']=='not_connected' and r.json()['model_version'] is None

BAD=[
    ('client_ai_verdict',dict(provenance='BACKEND-ANALYZED')),
    ('client_model_estimate',dict(provenance='MODEL-ESTIMATED')),
    ('extra_result_field',dict(analysis_result='basalt')),
    ('nan',dict(value=float('nan'))),
    ('infinity',dict(value=float('inf'))),
    ('nan_location',dict(location=dict(x_m=float('nan'),y_m=0,z_m=0,frame='scene_local_m'))),
    ('out_of_range',dict(value=91.0)),
    ('missing_value',dict(value=None)),
    ('unit_mismatch',dict(unit='rad')),
    ('type_mismatch',dict(sensor_type='environmental')),
    ('wrong_method',dict(method='made_up')),
    ('unknown_mission',dict(mission_id='ag.mars.other')),
    ('wrong_schema',dict(schema_version=2)),
    ('wrong_config',dict(sim_config_version='fm-sensors-0')),
    ('naive_time',dict(captured_at='2026-10-20T10:00:00')),
    ('future_time',dict(captured_at=(datetime.now(timezone.utc)+timedelta(hours=1)).isoformat())),
    ('old_time',dict(captured_at=(datetime.now(timezone.utc)-timedelta(days=40)).isoformat())),
    ('bad_frame',dict(location=dict(x_m=0,y_m=0,z_m=0,frame='mars_iau'))),
    ('outside_world',dict(location=dict(x_m=900,y_m=0,z_m=0,frame='scene_local_m'))),
    ('bad_station',dict(station_id='lab; drop')),
]
@pytest.mark.parametrize('name,change',BAD)
def test_invalid_records_rejected(name,change):
    app,repo=setup();h=H()
    with TestClient(app) as c:
        s=start(c,h)  # sent as raw JSON so NaN reaches the server unchanged
        r=c.post(P.format(s=s),headers={**h,'Content-Type':'application/json'},content=json.dumps({'records':[rec(**change)]}))
        assert r.status_code==422,(name,r.text);assert not repo.sensor.get(UUID(s))

@pytest.mark.parametrize('name,record',[
    ('unimplemented_with_value',lambda:rec('env.air_temperature','rover.env_package',value=210.0,method='not_modeled')),
    ('unimplemented_simulated',lambda:rec('env.air_pressure','rover.env_package',value=None,method='not_modeled')),
    ('spectrometer_number',lambda:rec('sci.spectral_reading','rover.spectrometer',value=.4,method='not_modeled')),
    ('sensor_not_on_instrument',lambda:rec('sci.sample_id','rover.terrain_profiler',value_text=str(uuid4()))),
    ('category_not_allowed',lambda:rec('terrain.traversability',value_text='excellent')),
    ('station_mismatch',lambda:rec('sci.station_id','rover.context_camera',station_id='site_1',value_text='site_2')),
    ('location_with_value',lambda:rec('nav.position','rover.navigation',value=1.0)),
])
def test_catalog_rules(name,record):
    app,_=setup();h=H()
    with TestClient(app) as c:
        s=start(c,h);assert c.post(P.format(s=s),headers=h,json={'records':[record()]}).status_code==422,name
def test_unavailable_spectrometer_record_accepted_without_value():
    app,_=setup();h=H()
    with TestClient(app) as c:
        s=start(c,h);assert c.post(P.format(s=s),headers=h,json={'records':[rec('sci.spectral_reading','rover.spectrometer',provenance='UNAVAILABLE',method='not_modeled',value=None)]}).status_code==200
def test_batch_limits():
    app,_=setup();h=H()
    with TestClient(app) as c:
        s=start(c,h);one=rec()
        assert c.post(P.format(s=s),headers=h,json={'records':[]}).status_code==422
        assert c.post(P.format(s=s),headers=h,json={'records':[one,one]}).status_code==422
        assert c.post(P.format(s=s),headers=h,json={'records':[rec() for _ in range(LIMITS['max_batch']+1)]}).status_code==422
def test_session_record_cap(monkeypatch):
    monkeypatch.setitem(LIMITS,'max_records_per_session',3)
    app,_=setup();h=H()
    with TestClient(app) as c:
        s=start(c,h);assert c.post(P.format(s=s),headers=h,json={'records':[rec(),rec(),rec()]}).status_code==200
        assert c.post(P.format(s=s),headers=h,json={'records':[rec()]}).status_code==429
def test_other_users_session_and_closed_session():
    app,repo=setup();h=H()
    with TestClient(app) as c:
        s=start(c,h);row=repo.rows[UUID(s)]
        row['user_id']=UUID('22222222-2222-4222-8222-222222222222')
        assert c.post(P.format(s=s),headers=h,json={'records':[rec()]}).status_code==404
        row['user_id']=U;row['status']='integration_check_complete'
        assert c.post(P.format(s=s),headers=h,json={'records':[rec()]}).status_code==409
        assert c.post(P.format(s=uuid4()),headers=h,json={'records':[rec()]}).status_code==404
def test_repository_response_mismatch_is_502():
    app,repo=setup();h=H()
    async def bad(u,s,records):return [dict(session_id=s,event_id=uuid4(),duplicate=False,stored_at=datetime.now(timezone.utc))]
    with TestClient(app) as c:
        s=start(c,h);repo.submit_sensor_records=bad
        assert c.post(P.format(s=s),headers=h,json={'records':[rec()]}).status_code==502
def test_unity_and_backend_catalogs_identical():
    root=Path(__file__).resolve().parents[4]
    unity=root/'01-Game-Development/MissionRedHorizon/Assets/AstroGateway/FirstMission/Config/sensor_catalog_v1.json'
    assert unity.is_file(), 'The repository Unity sensor catalog is missing'
    backend=Path(sensor_records.__file__).resolve().parent/'rules'/'sensor_catalog_v1.json'
    assert unity.read_bytes()==backend.read_bytes()
def test_every_catalog_sensor_belongs_to_an_instrument_and_unimplemented_are_not_modeled():
    used={s for i in sensor_records.INSTRUMENTS.values() for s in i['sensors']}
    assert used==set(sensor_records.SENSORS)
    for s in sensor_records.SENSORS.values():assert s['implemented'] or s['method']=='not_modeled'
