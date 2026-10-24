"""Step 4 rule engines: offline unit tests and sample scenarios. No network, no database."""
import copy,json
from datetime import datetime,timezone,timedelta
from pathlib import Path
from uuid import uuid4
import pytest
from rules import (Command,MissionCatalog,LifeSupportModel,WeatherModel,RulesConfig,SkillAssessor,
                   RulesRecommendationEngine,build_timeline,load_config,score_session)
from rules.config import validate

CFG=load_config();CAT=MissionCatalog(CFG);MISSION=CAT.resolve('ag.mars.red-horizon.surface-ops')
T0=datetime(2026,10,17,8,0,tzinfo=timezone.utc)
def at(s):return T0+timedelta(seconds=s)
def scan(rev,s,sample=None,**extra):return Command(rev,'scan_target',at(s),{'sample_id':sample or str(uuid4()),'tick':1,'analysis':'unverified_simulated',**extra})
def run(script):
    """script: list of (seconds, kind[, payload]) -> Command list with increasing revisions."""
    out=[]
    for i,item in enumerate(script):
        s,kind=item[0],item[1]
        out.append(scan(i,s) if kind=='scan' else Command(i,kind,at(s),item[2] if len(item)>2 else {}))
    return out
def report(script,end_s,difficulty=0,completed=True,mission=MISSION):
    tl=build_timeline(T0,run(script),at(end_s) if completed else None,None if completed else at(end_s))
    return score_session(CFG,mission,difficulty,tl)
IDEAL=[(60,'board_rover'),(120,'exit_rover'),(150,'scan'),(200,'scan'),(250,'scan'),(300,'scan'),(350,'scan')]

# ---------- configuration ----------
def test_config_is_valid_and_labelled_proposed():
    assert 'PROPOSED' in CFG.raw['status'] and CFG.version.endswith('proposed')
    assert abs(sum(CFG.scoring['weights'].values())-1)<1e-9
    assert CFG.life_support['penalties_enabled'] is False and CFG.weather['effects_enabled'] is False
@pytest.mark.parametrize('path,value',[(('scoring','weights','observation'),0.9),(('life_support','thresholds_fraction','critical'),0.5),
    (('life_support','activity_rate_kg_per_h','walking'),-1),(('weather','scenarios','1'),[{'start_s':10,'state':'clear'}]),
    (('weather','scenarios','2'),[{'start_s':0,'state':'tornado'}]),(('skills','levels'),[{'level':0,'name':'A','min_demonstrations':3,'min_distinct_sessions':0,'min_rolling_mean':0,'min_difficulty_demonstrated':0},{'level':1,'name':'B','min_demonstrations':1,'min_distinct_sessions':0,'min_rolling_mean':0,'min_difficulty_demonstrated':0}])])
def test_invalid_configuration_is_rejected(path,value):
    raw=copy.deepcopy(CFG.raw);node=raw
    for k in path[:-1]:node=node[k]
    node[path[-1]]=value
    with pytest.raises(ValueError):validate(raw)

# ---------- mission identifiers ----------
@pytest.mark.parametrize('legacy',['mission-red-horizon-poc','mission.red_horizon.slice','mission.red_horizon.research_base.v1',
    'mission.red_horizon.visual_polish.v2','mission.red_horizon.cinematic_hud.v1','mission.red_horizon.production_visual.v1'])
def test_every_existing_id_maps_without_rewriting(legacy):
    r=CAT.resolve(legacy)
    assert r.canonical_id=='ag.mars.red-horizon.surface-ops' and r.original_id==legacy and r.is_alias
def test_canonical_and_unknown_ids():
    assert not CAT.resolve('ag.mars.red-horizon.surface-ops').is_alias
    with pytest.raises(KeyError):CAT.resolve('mission.unknown')
    raw=copy.deepcopy(CFG.raw);raw['missions']['aliases'].append({'alias':'x','canonical':'missing','source':'t'})
    with pytest.raises(ValueError):validate(raw)

# ---------- life support ----------
def test_oxygen_rates_units_and_thresholds():
    ls=LifeSupportModel(CFG)
    assert ls.rate_kg_per_h('walking','clear')==pytest.approx(0.062)
    assert ls.rate_kg_per_h('walking','dust_storm')==pytest.approx(0.06*1.15+0.002)
    # 15 real minutes of walking = 5 simulated hours = 0.31 kg of a 0.60 kg tank.
    assert ls.consume(0.6,'walking','clear',900)==pytest.approx(0.6-0.31)
    assert ls.consume(0.01,'running','dust_storm',10_000)==0.0
    assert [ls.status(x) for x in (0.6,0.17,0.08,0.02)]==['nominal','caution','warning','critical']
    assert ls.endurance_real_seconds(0.6,'walking','clear')==pytest.approx(0.6/0.062*3600/20)
    assert ls.percent_per_real_second('idle','clear')==pytest.approx(0.037*20/3600/0.6*100)
def test_activity_ordering_is_physically_sensible():
    ls=LifeSupportModel(CFG);r=lambda a:ls.rate_kg_per_h(a,'clear')
    assert r('idle')<r('driving')<r('scanning')<r('walking')<r('running')

# ---------- weather ----------
def test_weather_scenarios_and_effects():
    w=WeatherModel(CFG)
    assert [w.state_at(2,s) for s in (0,239,240,360,599,600,720,9999)]==['clear','clear','light_dust','dust_storm','dust_storm','light_dust','clear','clear']
    assert w.storm_intervals(2,1000)==[(360,600)] and w.storm_intervals(2,400)==[(360,400)] and w.storm_intervals(0,1000)==[]
    clear,storm=w.conditions(2,0),w.conditions(2,400)
    assert storm['solar_power_factor']<clear['solar_power_factor']<1 and storm['visibility_m']<clear['visibility_m']
    assert storm['advisory']=='shelter' and clear['source']=='simulation'
    assert w.conditions(0,0,local_hour=14)['temperature_c']==pytest.approx(-10) and w.conditions(0,0,local_hour=2)['temperature_c']==pytest.approx(-80)

# ---------- server timeline ----------
def test_timeline_excludes_pauses_and_post_completion_commands():
    tl=build_timeline(T0,run([(100,'pause'),(400,'resume'),(500,'scan'),(900,'scan')]),at(600))
    assert tl.active_seconds==pytest.approx(300) and len(tl.scans)==1
def test_duplicate_samples_and_rover_scans_do_not_count():
    sid=str(uuid4())
    tl=build_timeline(T0,[scan(0,10,sid),scan(1,20,sid),Command(2,'board_rover',at(30)),scan(3,40)],at(100))
    assert len(tl.scans)==1 and tl.board_exit_cycles==0

# ---------- scoring sample scenarios ----------
def test_scenario_A_ideal_easy_mission():
    r=report(IDEAL,600)
    assert r['score']==100.0 and r['grade']=='Excellent' and r['not_assessed']==['weather_safety']
    assert sum(r['weights_applied'].values())==pytest.approx(1) and r['provisional']
def test_scenario_B_storm_on_foot_vs_sheltered():
    exposed=report(IDEAL,700,difficulty=2)
    sheltered=report(IDEAL+[(360,'board_rover'),(600,'exit_rover')],700,difficulty=2)
    assert exposed['subscores']['weather_safety']==0 and sheltered['subscores']['weather_safety']==1
    assert sheltered['score']>exposed['score'] and 'not_assessed' in exposed and exposed['not_assessed']==[]
def test_scenario_C_incomplete_and_oxygen_exhausted():
    r=report([(10,'scan')],3000,completed=False)
    assert r['subscores']['mission_planning']==0 and r['subscores']['life_support']==0 and r['score']<20
def test_scenario_D_slow_mission_planning_is_linear():
    assert report(IDEAL,900)['subscores']['mission_planning']==1
    assert report(IDEAL,1350)['subscores']['mission_planning']==pytest.approx(0.5)
    assert report(IDEAL,1800)['subscores']['mission_planning']==0
def test_scenario_E_client_values_cannot_raise_score():
    honest=run(IDEAL)
    forged=[Command(c.revision,c.kind,c.accepted_at,{**c.payload,'oxygen':100,'score':100,'tick':0}) for c in honest]
    a=score_session(CFG,MISSION,0,build_timeline(T0,honest,at(600)))
    b=score_session(CFG,MISSION,0,build_timeline(T0,forged,at(600)))
    assert a['score']==b['score']
def test_score_explanations_and_alias_reporting():
    r=report(IDEAL,600,mission=CAT.resolve('mission-red-horizon-poc'))
    assert r['mission_id']=='ag.mars.red-horizon.surface-ops' and r['original_mission_id']=='mission-red-horizon-poc'
    assert all(isinstance(v,str) and v for v in r['explanations'].values())

# ---------- skills ----------
def sessions(n,script=IDEAL,end=600,difficulty=0):
    return [report(script,end,difficulty) for _ in range(n)]
def evidence(reports):
    return [e for i,r in enumerate(reports) for e in SkillAssessor.evidence_from_report(f's{i}',i,r)]
def test_skill_levels_need_repeated_evidence_across_sessions():
    sa=SkillAssessor(CFG)
    assert sa.assess(evidence([]))['trainee_level']==0
    one=sa.assess(evidence(sessions(1)))['skills']['observation']
    assert one['level']==1 and one['next_level']=='Proficient' and one['to_reach_next_level']
    # Three strong sessions at difficulty 0 are not enough for Proficient: difficulty 1 evidence is required.
    assert sa.assess(evidence(sessions(3)))['skills']['observation']['level']==1
    mixed=sessions(2)+sessions(1,difficulty=1)
    assert sa.assess(evidence(mixed))['skills']['observation']['level']==2
def test_playtime_alone_never_raises_a_level():
    sa=SkillAssessor(CFG)
    idle=[report([(10,'board_rover')],1700,completed=True) for _ in range(5)]
    assert sa.assess(evidence(idle))['skills']['observation']['level']==0
def test_one_session_counted_once():
    sa=SkillAssessor(CFG);r=report(IDEAL,600)
    ev=SkillAssessor.evidence_from_report('same',0,r)*5
    assert sa.assess(ev)['skills']['observation']['distinct_sessions']==1
def test_advanced_requires_hard_difficulty_and_trainee_level_is_conservative():
    sa=SkillAssessor(CFG)
    storm=IDEAL+[(360,'board_rover'),(600,'exit_rover')]
    reports=[report(storm,700,difficulty=2) for _ in range(5)]
    result=sa.assess(evidence(reports))
    assert result['skills']['observation']['level']==3 and result['skills']['weather_safety']['level']==3
    weak=sa.assess(evidence(sessions(5)))
    assert weak['skills']['weather_safety']['level']==0 and weak['trainee_level']==0

# ---------- recommendations ----------
ENGINE=RulesRecommendationEngine(CFG,CAT)
def rec(history):return ENGINE.recommend(history,SkillAssessor(CFG).assess(evidence(history)))
def test_R1_new_trainee():
    r=rec([]);assert r['rules_fired']==['R1_NEW_TRAINEE'] and r['difficulty']==0 and r['reasons']
def test_R5_unassessed_weather_skill_raises_to_storm_scenario():
    r=rec(sessions(1))
    assert 'R5_ASSESS_UNSEEN_SKILL' in r['rules_fired'] and r['difficulty']==2 and r['focus_skill']=='weather_safety'
def test_R5_not_fired_after_a_weak_mission():
    r=rec([report([(10,'scan')],600)])
    assert 'R5_ASSESS_UNSEEN_SKILL' not in r['rules_fired'] and r['difficulty']==0
def test_R2_raise_and_lower():
    strong=[report(IDEAL+[(360,'board_rover'),(600,'exit_rover')],700,difficulty=1) for _ in range(2)]
    assert 'R2_RAISE_DIFFICULTY' in rec(strong)['rules_fired'] and rec(strong)['difficulty']==2
    weak=[report([(10,'scan')],3000,difficulty=2,completed=False) for _ in range(2)]
    r=rec(weak);assert 'R2_LOWER_DIFFICULTY' in r['rules_fired'] and r['difficulty']==1
def test_R4_learning_modules_explain_why():
    r=rec([report([(10,'scan')],3000,completed=False)])
    assert 'R4_LEARNING_MODULES' in r['rules_fired'] and 'learn.suit-oxygen-budgeting' in r['learning_modules']
    assert any('suit-oxygen' in m or 'oxygen' in reason.lower() for m in r['learning_modules'] for reason in r['reasons'])
def test_engine_is_replaceable():
    class FakeML:
        def recommend(self,history,skills):return {'mission_id':'ag.mars.red-horizon.surface-ops','difficulty':0,'reasons':['model'],'engine':'ml-test'}
    def use(engine:'RecommendationEngine',h):return engine.recommend(h,{})
    assert use(FakeML(),[])['engine']=='ml-test' and use(ENGINE,[])['engine']=='rules-v1'

# ---------- shared vectors for the Unity implementation ----------
VECTORS=Path(__file__).resolve().parent.parent/'rules'/'test_vectors_v1.json'
def compute_vectors():
    ls,w=LifeSupportModel(CFG),WeatherModel(CFG)
    return {'rules_version':CFG.version,
            'oxygen_percent_per_real_second':{f'{a}|{s}':round(ls.percent_per_real_second(a,s),9) for a in CFG.life_support['activity_rate_kg_per_h'] for s in CFG.weather['states']},
            'weather':[{'difficulty':d,'t':t,**{k:v for k,v in w.conditions(d,t).items() if k in ('state','tau','visibility_m','solar_power_factor','advisory')}} for d in (0,1,2) for t in (0,250,300,400,650,800)]}
def test_shared_vectors_match_engine():
    assert json.loads(VECTORS.read_text())==compute_vectors()

if __name__=='__main__':
    VECTORS.write_text(json.dumps(compute_vectors(),indent=1));print('wrote',VECTORS)
