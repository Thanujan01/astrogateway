"""Project-generated synthetic benchmark; independent latent labels, no real accounts."""
import hashlib,json,math,random,sys
from types import SimpleNamespace
from pathlib import Path
BASE=Path(__file__).resolve().parents[3]
sys.path.insert(0,str(BASE/'Stage07A/Development/backend'))
from adaptive.engine import assess
from pathlib import Path
SKILLS=('navigation','survival','decision_making','equipment','emergency')
def digest(data):return hashlib.sha256(data).hexdigest()
def generate(seed=707,players=100):
 rows=[]
 for p in range(players):
  split='train' if p<60 else 'validation' if p<80 else 'test'
  days=range(1,11) if split=='train' else range(11,16) if split=='validation' else range(16,21)
  rng=random.Random(seed*1000+p);latent=[rng.uniform(.15,.95) for _ in SKILLS]
  for day in days:
   severity=rng.randrange(3);perf=[max(0,min(1,x+rng.gauss(0,.14))) for x in latent]
   scores=[perf[0],.4+.6*perf[1],perf[2],sum(rng.random()<perf[3] for _ in range(5))/5,.1+.9*perf[4]]
   probs=[1/(1+math.exp(-(8*(x-.55)-.5*severity))) for x in latent]
   labels=[int(rng.random()<q) for q in probs]
   # Actual Stage07A assessor on synthetic primitive measurements. Labels remain independent latent draws.
   events=[SimpleNamespace(kind='movement',elapsed_s=1,x_m=-45-10*(1-perf[0]),z_m=-40,speed_mps=2.5),
    SimpleNamespace(kind='resource',elapsed_s=2,oxygen_used_l=600*(1-scores[1]),water_used_l=0,power_used_wh=0),
    SimpleNamespace(kind='reaction',elapsed_s=3,reaction_s=1+5*(1-perf[2])),
    SimpleNamespace(kind='emergency',elapsed_s=4,resolved=True,response_s=10+45*(1-perf[4]))]
   for i in range(5):events.append(SimpleNamespace(kind='equipment',elapsed_s=5+i,correct=i<round(scores[3]*5)))
   actual=assess(events)
   scores=[actual[k]['score']/100 for k in SKILLS]
   row={'schema':'07b.1','row_id':f'sim-{p:04d}-{day:02d}','player_group':f'sim-{p:04d}','mission_group':f'synthetic-mission-{p:04d}-{day:02d}','day':day,'split':split,'evidence':'synthetic','language':'ta' if p%2 else 'en','difficulty':['Easy','Medium','Hard'][severity],'features':scores+[severity/2],'labels':labels,'label_source':'independent latent Bernoulli simulation; not human or teacher-score truth','seed':seed}
   rows.append(row)
 return rows
def gate(rows,manifest):
 if manifest.get('source')!='project-synthetic-generator-07b.1' or manifest.get('authorized_use')!='local_synthetic_experiment' or manifest.get('scientific_observations_used') or manifest.get('real_player_data_used'):raise ValueError('Training authorization/provenance failed')
 data=('\n'.join(json.dumps(r,sort_keys=True,separators=(',',':')) for r in rows)+'\n').encode()
 if digest(data)!=manifest['sha256']:raise ValueError('Dataset hash mismatch')
 if not manifest.get('evaluation_protocol_sha256') or not manifest.get('generator_sha256'):raise ValueError('Protocol/source provenance missing')
 groups={s:set() for s in ('train','validation','test')};times={s:[] for s in groups};ids=set();targets={s:[set() for _ in SKILLS] for s in groups}
 for r in rows:
  if r['row_id'] in ids:raise ValueError('Duplicate row');
  ids.add(r['row_id'])
  if r['evidence']!='synthetic' or not r['player_group'].startswith('sim-'):raise ValueError('Real/untrusted records excluded')
  if r['split'] not in groups or set(r)!={'schema','row_id','player_group','mission_group','day','split','evidence','language','difficulty','features','labels','label_source','seed'}:raise ValueError('Dataset schema')
  if len(r['features'])!=6 or any(type(x) not in (int,float) or not math.isfinite(x) or not 0<=x<=1 for x in r['features']):raise ValueError('Feature bounds')
  if len(r['labels'])!=5 or any(type(v) is not int or v not in (0,1) for v in r['labels']):raise ValueError('Label types')
  groups[r['split']].add(r['player_group']);times[r['split']].append(r['day'])
  for i,v in enumerate(r['labels']):targets[r['split']][i].add(v)
 if any(groups[a]&groups[b] for a,b in [('train','validation'),('train','test'),('validation','test')]):raise ValueError('Player leakage')
 if not all(times.values()) or not max(times['train'])<min(times['validation']) or not max(times['validation'])<min(times['test']):raise ValueError('Time leakage')
 if any(v!={0,1} for t in targets.values() for v in t):raise ValueError('Missing target classes')
 return {'status':'PASS','rows':len(rows),'split_sizes':{s:len(times[s]) for s in times},'player_disjoint':True,'strict_time_order':True,'scientific_observations_used':False,'real_player_data_used':False}
