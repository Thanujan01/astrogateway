"""Rule version07a.1. Synthetic mission rules; no learned model or NASA weather forecast."""
import hashlib,math,random
SKILLS=('navigation','survival','decision_making','equipment','emergency')
ROUTE=((-45,-60),(0,-60),(13,-45),(53,-55),(13,-45),(0,0),(-5,10),(0,55),(0,75),(-35,40),(-45,0),(-45,-60))
SOURCE={'id':'nasa-mars-facts','url':'https://science.nasa.gov/mars/facts/','fact':'Mars has a thin atmosphere; suspended dust affects sky appearance.'}
def clamp(x):return max(0,min(100,x))
def route_error(x,z):
 distances=[]
 for (ax,az),(bx,bz) in zip(ROUTE,ROUTE[1:]):
  dx,dz=bx-ax,bz-az;t=max(0,min(1,((x-ax)*dx+(z-az)*dz)/(dx*dx+dz*dz)))
  distances.append(math.hypot(x-ax-t*dx,z-az-t*dz))
 return min(distances)
def validate_events(events):
 last_move=None;last_resources=(0,0,0)
 for e in events:
  if e.kind=='movement':
   if last_move:
    dt=e.elapsed_s-last_move.elapsed_s;distance=math.hypot(e.x_m-last_move.x_m,e.z_m-last_move.z_m)
    if dt<=0 or distance>4*dt+1:raise ValueError('Impossible movement interval')
   last_move=e
  if e.kind=='resource':
   values=(e.oxygen_used_l,e.water_used_l,e.power_used_wh)
   if any(a<b for a,b in zip(values,last_resources)):raise ValueError('Cumulative resource use cannot decrease')
   last_resources=values
 return True
def assess(events,attested=False):
 validate_events(events);observations={s:[] for s in SKILLS};penalties={s:0 for s in SKILLS}
 for e in events:
  if e.kind=='movement':observations['navigation'].append(clamp(100-10*route_error(e.x_m,e.z_m)-20*max(0,e.speed_mps-2.5)))
  elif e.kind=='reaction':observations['decision_making'].append(clamp(100-20*max(0,e.reaction_s-1)))
  elif e.kind=='resource':observations['survival']=[clamp(100-100*max(e.oxygen_used_l/600,e.water_used_l/4,e.power_used_wh/1800))]
  elif e.kind=='equipment':observations['equipment'].append(100 if e.correct else 0)
  elif e.kind=='emergency':observations['emergency'].append(clamp(100-2*max(0,e.response_s-10)) if e.resolved else 0)
  elif e.kind=='mistake':penalties[e.category]+=5*(1+e.retries)
 result={}
 for s,values in observations.items():
  n=len(values);score=round(clamp(sum(values)/n-penalties[s]),2) if n else None
  result[s]={'score':score,'observations':n,'confidence':round(min(.9,n/10)* (1 if attested else .25),3),'status':'insufficient_data' if n<3 else 'provisional' if not attested else 'assessed','basis':'server_attested_fixture' if attested else 'client_reported','penalty':penalties[s]}
 return result
def recommend(history,previous='Easy'):
 levels=['Easy','Medium','Hard'];index=levels.index(previous)
 # Clients cannot choose an elevated prior: callers must use stored server recommendation.
 trusted=[h for h in history if h['attested']][-5:]
 stage=[h for h in trusted if h.get('difficulty',previous)==previous]
 if not trusted:return {'difficulty':'Easy','reason':'No server-attested outcomes; safe baseline','eligible_promotions':False}
 successes=[h['success'] for h in trusted]
 scores=[h['aggregate'] for h in trusted if h['aggregate'] is not None]
 if len(stage)>=3 and all(h['success'] and h['aggregate'] is not None and h['aggregate']>=75 for h in stage[-3:]):index=min(2,index+1);reason='Three attested successful missions above threshold'
 elif len(trusted)>=2 and not any(successes[-2:]):index=max(0,index-1);reason='Two attested failures; recovery step'
 else:reason='Hold difficulty while evidence accumulates'
 return {'difficulty':levels[index],'reason':reason,'eligible_promotions':True}
def simulate(seed,difficulty):
 if type(seed) is not int or not 0<=seed<=2**32-1:raise ValueError('Seed outside uint32')
 severity={'Easy':0,'Medium':1,'Hard':2}[difficulty];r=random.Random(seed)
 temperature=round(r.uniform(-85,-20)+severity*-3,1);dust=round(r.uniform(.05,.35)+severity*.18,3)
 incident=r.choice(['none','dust','power_loss','rover_fault','oxygen_leak']) if r.random()<.15+.15*severity else 'none'
 return {'rules':'07a.1','seed':seed,'difficulty':difficulty,'temperature_c':temperature,'dust_index':dust,'visibility_m':round(300*(1-dust)**2,1),'solar_fraction':round(max(.1,1-.85*dust),3),'terrain_slope_deg':round(r.uniform(0,8)+severity*4,1),'rover_speed_limit_mps':round(1.5*(1-.4*dust)/(1+.2*severity),3),'route_risk':round(min(1,dust*.5+severity*.15),3),'incident':incident,'interpretation':'synthetic game scenario, not measured Jezero weather','scientific_reference':SOURCE['url']}
def plan(skills,seed,cycle=0,history=()):
 if type(cycle) is not int or not 0<=cycle<=10000:raise ValueError('Cycle outside bounds')
 weakest=sorted(SKILLS,key=lambda s:(skills.get(s,{}).get('score') if skills.get(s,{}).get('score') is not None else -1,s))
 attested=[h for h in history if h['attested']]
 level='advanced' if len(attested)>=10 and all(h['success'] for h in attested[-5:]) and all(skills[s]['score'] is not None and skills[s]['score']>=80 and skills[s]['confidence']>=.5 for s in SKILLS) else 'intermediate' if len(attested)>=3 and all(h['success'] and h['aggregate'] is not None and h['aggregate']>=75 for h in attested[-3:]) else 'beginner'
 base=0 if level=='beginner' else 1 if level=='intermediate' else 2
 recovery=bool(history) and not history[-1]['success'];days=[]
 approved='Easy'
 for n in range(1,len(attested)+1):approved=recommend(attested[:n],approved)['difficulty']
 base=min(base,['Easy','Medium','Hard'].index(approved))
 for day in range(1,31):
  focus=weakest[(0,1,0,2,1,3,4)[(day-1)%7]];rest=day%7==0
  child=int.from_bytes(hashlib.sha256(f'07a.1:{seed}:{cycle}:{day}'.encode()).digest()[:4],'big')
  difficulty=['Easy','Medium','Hard'][max(0,base-(1 if recovery and day<=2 else 0))]
  days.append({'day':day,'focus':focus,'mission':'recovery_review' if rest or recovery and day<=2 else 'mission-red-horizon-poc','difficulty':difficulty,'seed':child,'target_minutes':10 if rest else 15+base*5,'state':'planned','scenario':simulate(child,difficulty)})
 return {'rules':'07a.1','cycle':cycle,'seed':seed,'level':level,'days':days,'completed_days':0,'continuation':'Recompute after attested outcome; planned Hard is never immediate promotion'}
def debrief(skills):
 weakest=min(SKILLS,key=lambda s:skills[s]['score'] if skills[s]['score'] is not None else -1)
 return {'instructor':'ARIA deterministic retrieval template','observations':skills,'feedback':f'Practice {weakest}. These metrics are provisional until server-attested evidence exists.','science':[SOURCE],'not_measured':['real Mars weather','unobserved mission outcomes'],'generated_by_model':False}
