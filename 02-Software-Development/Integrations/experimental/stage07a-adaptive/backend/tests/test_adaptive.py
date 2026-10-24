import uuid,json
import pytest
from pydantic import ValidationError
from adaptive.schemas import Batch
from adaptive.engine import assess,validate_events,recommend,simulate,plan,SKILLS,route_error,debrief

def event(kind='movement',elapsed=1,**fields):
 defaults={'movement':dict(x_m=-45,z_m=-60,speed_mps=2.5),'reaction':dict(reaction_s=2),'resource':dict(oxygen_used_l=40,water_used_l=.2,power_used_wh=100),'equipment':dict(equipment='solar_array',correct=True),'emergency':dict(incident='dust',resolved=True,response_s=12),'mistake':dict(category='navigation',retries=1),'completion':dict(completed=True)}
 return dict(event_id=str(uuid.uuid4()),elapsed_s=elapsed,kind=kind,**{**defaults[kind],**fields})
def batch(events):return Batch.model_validate(dict(mission_session_id=str(uuid.uuid4()),events=events))
@pytest.mark.parametrize('kind',['movement','reaction','resource','equipment','emergency','mistake','completion'])
def test_each_schema(kind):assert batch([event(kind)]).events[0].kind==kind
@pytest.mark.parametrize('field,value',[('score',100),('user_id',str(uuid.uuid4())),('attested',True),('x_m',126),('speed_mps',float('nan')),('speed_mps',5),('z_m',float('inf'))])
def test_bad_movement(field,value):
 with pytest.raises(ValidationError):batch([event(**{field:value})])
@pytest.mark.parametrize('events',[[],[event(),event(elapsed=1)],[event('completion'),event(elapsed=2)],[event('equipment',correct='true')]])
def test_invalid_batch(events):
 with pytest.raises(ValidationError):batch(events)
def test_duplicate_id():
 a=event();b={**a,'elapsed_s':2}
 with pytest.raises(ValidationError):batch([a,b])
def test_impossible_movement():
 with pytest.raises(ValueError):validate_events(batch([event(),event(elapsed=2,x_m=125)]).events)
def test_resources_cannot_decrease():
 with pytest.raises(ValueError):validate_events(batch([event('resource'),event('resource',elapsed=2,oxygen_used_l=1)]).events)
def test_missing_data():
 a=assess([]);assert all(s['score'] is None and s['status']=='insufficient_data' for s in a.values())
def test_penalty_explainable():
 a=assess(batch([event(),event('mistake',elapsed=2)]).events);assert a['navigation']['score']==90 and a['navigation']['penalty']==10
def test_route_reference():assert route_error(-45,-60)==0 and route_error(125,125)>0
def test_provisional_confidence_capped():assert assess(batch([event(elapsed=i) for i in range(1,31)]).events)['navigation']['confidence']<=.225
def trusted(n,**kwargs):return [dict(attested=True,success=True,aggregate=90,difficulty='Easy',**kwargs) for _ in range(n)]
def test_cannot_promote_client_scores():assert recommend([dict(attested=False,success=True,aggregate=100)]*10,'Hard')['difficulty']=='Easy'
def test_hysteresis():assert recommend(trusted(2))['difficulty']=='Easy' and recommend(trusted(3))['difficulty']=='Medium'
def test_no_easy_to_hard():assert recommend(trusted(10))['difficulty']=='Medium'
def test_requires_medium_evidence():assert recommend(trusted(3),'Medium')['difficulty']=='Medium'
def test_downshift():assert recommend([dict(attested=True,success=False,aggregate=20)]*2,'Hard')['difficulty']=='Medium'
@pytest.mark.parametrize('seed',[0,1,42,4294967295])
@pytest.mark.parametrize('difficulty',['Easy','Medium','Hard'])
def test_simulation_replay(seed,difficulty):
 a=simulate(seed,difficulty);assert a==simulate(seed,difficulty) and 0<=a['solar_fraction']<=1 and a['interpretation'].startswith('synthetic')
def test_variation():assert simulate(1,'Easy')!=simulate(2,'Easy')
@pytest.mark.parametrize('seed',[-1,2**32,1.2,True])
def test_bad_seed(seed):
 with pytest.raises(ValueError):simulate(seed,'Easy')
def skills(score=None,confidence=0):return {s:dict(score=score,confidence=confidence) for s in SKILLS}
def test_thirty_days_replay():
 a=plan(skills(),42);assert a==plan(skills(),42) and len(a['days'])==30 and a['level']=='beginner' and all(d['difficulty']=='Easy' for d in a['days'])
def test_controlled_cycles():assert plan(skills(),42,0)['days']!=plan(skills(),42,1)['days']
def test_intermediate_advanced():
 assert plan(skills(80,.7),1,history=trusted(3))['level']=='intermediate'
 assert plan(skills(90,.9),1,history=trusted(12))['level']=='advanced'
def test_recovery():
 h=[dict(attested=True,success=False,aggregate=20)]
 assert plan(skills(),1,history=h)['days'][0]['mission']=='recovery_review'
def test_aria_never_trained():assert debrief(assess([]))['generated_by_model'] is False

def test_weakest_gets_extra_practice():
 s=skills(90,.8);s['survival']['score']=10
 p=plan(s,123)
 assert sum(d['focus']=='survival' for d in p['days'])>sum(d['focus']=='navigation' for d in p['days'])
def test_low_scores_do_not_make_intermediate():
 h=[dict(attested=True,success=True,aggregate=30,difficulty='Easy')]*3
 assert plan(skills(30,.5),1,history=h)['level']=='beginner'
def test_advanced_plan_cannot_skip_medium_evidence():
 p=plan(skills(90,.9),1,history=trusted(12))
 assert all(d['difficulty']!='Hard' for d in p['days'])
