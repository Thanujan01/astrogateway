"""Offline peer comparison, using the existing mission scorer/skill assessor unchanged."""
from copy import deepcopy
from uuid import uuid4
import pytest
from rules.peers import CompletedEvidence, Peer, PeerMatcher
from test_rules import CFG, IDEAL, report
MATCHER = PeerMatcher(CFG)
def history(n=1, value=None):
    return tuple(CompletedEvidence(str(uuid4()),i,deepcopy(value or report(IDEAL,600))) for i in range(n))
def peer(evidence=None,discoverable=True):
    return Peer(str(uuid4()),'Synthetic trainee',discoverable,history() if evidence is None else evidence)
def test_uses_existing_assessor_and_rubric_version():
    own,other=peer(),peer();row=MATCHER.suggest(own,[other])[0]
    assert row['rules_version']==CFG.version and row['similarity']==1
    assert row['provisional'] and 'weather_safety' in row['uncompared_skills']
    assert MATCHER.assessment(own.history)['skills']['observation']['level']==1
def test_self_opt_out_and_bilateral_block_union_excluded():
    own,blocked,hidden=peer(),peer(),peer(discoverable=False)
    assert MATCHER.suggest(own,[own,blocked,hidden],blocked_ids=[blocked.user_id])==[]
def test_empty_histories_are_not_equal_skill():
    assert MATCHER.suggest(peer(()),[peer(())])==[]
    assert MATCHER.suggest(peer(),[peer(())])==[]
def test_duplicates_cannot_inflate_demonstrations():
    ev=history()[0];facts=MATCHER.assessment((ev,ev,ev))['skills']['observation']
    assert facts['assessments']==facts['demonstrations']==facts['distinct_sessions']==1
def test_conflicting_duplicate_session_rejected():
    ev=history()[0];altered=deepcopy(ev.report);altered['subscores']['observation']=0
    with pytest.raises(ValueError,match='Conflicting'):MATCHER.assessment((ev,CompletedEvidence(ev.session_id,ev.completed_order,altered)))
@pytest.mark.parametrize('value',[float('nan'),float('inf'),-0.1,1.1,True,'1'])
def test_invalid_subscores_rejected(value):
    r=report(IDEAL,600);r['subscores']['observation']=value
    with pytest.raises(ValueError):MATCHER.assessment(history(value=r))
def test_incompatible_versions_rejected():
    r=report(IDEAL,600);r['rules_version']='unapproved-version'
    with pytest.raises(ValueError,match='version'):MATCHER.assessment(history(value=r))
def test_unknown_skill_rejected():
    r=report(IDEAL,600);r['subscores']['invented_skill']=1
    with pytest.raises(ValueError):MATCHER.assessment(history(value=r))
def test_higher_coverage_beats_single_skill_similarity():
    full=report(IDEAL,600);single=deepcopy(full);single['subscores']={'observation':1}
    a,b=peer(history(value=full)),peer(history(value=single));rows=MATCHER.suggest(peer(),[b,a]);assert rows[0]['user_id']==a.user_id
def test_same_scores_do_not_replace_rubric_levels():
    weak=report([(10,'scan')],3000,completed=False);weak['score']=100
    row=MATCHER.suggest(peer(),[peer(history(value=weak))])[0]
    assert row['similarity']<1 and row['level_differences']['observation']>0
def test_candidate_identity_must_be_unique():
    candidate=peer()
    with pytest.raises(ValueError,match='Duplicate'):MATCHER.suggest(peer(),[candidate,candidate])
@pytest.mark.parametrize('limit',[0,21,True])
def test_bounded_results(limit):
    with pytest.raises(ValueError):MATCHER.suggest(peer(),[],limit=limit)
def test_results_do_not_expose_raw_reports_or_emails():
    row=MATCHER.suggest(peer(),[peer()])[0]
    assert not {'history','email','subscores','password','access_token'} & row.keys()
