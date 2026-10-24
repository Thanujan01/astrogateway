"""Five independent logistic heads; deterministic full-batch gradient descent, stdlib only."""
import math
from .dataset import SKILLS

def sigmoid(x):return 1/(1+math.exp(-max(-35,min(35,x))))
def fit(rows,epochs=300,learning_rate=.15):
 train=[r for r in rows if r['split']=='train'];n=len(train)
 means=[sum(r['features'][j] for r in train)/n for j in range(6)]
 scale=[max(.05,math.sqrt(sum((r['features'][j]-means[j])**2 for r in train)/n)) for j in range(6)]
 x=[[1]+[(v-means[j])/scale[j] for j,v in enumerate(r['features'])] for r in train];heads=[]
 for skill in range(5):
  w=[0.0]*7
  for _ in range(epochs):
   grad=[0.0]*7
   for features,r in zip(x,train):
    err=sigmoid(sum(a*b for a,b in zip(w,features)))-r['labels'][skill]
    for j,v in enumerate(features):grad[j]+=err*v
   w=[v-learning_rate*(grad[j]/n+(.005*v if j else 0)) for j,v in enumerate(w)]
  heads.append(w)
 return {'model':'five-head logistic regression','features':'five rule-score proxies + difficulty index; no ID/day/language/seed features','means':means,'scales':scale,'weights':heads,'epochs':epochs,'learning_rate':learning_rate}
def predict(model,row):
 x=[1]+[(v-model['means'][j])/model['scales'][j] for j,v in enumerate(row['features'])]
 return [sigmoid(sum(a*b for a,b in zip(w,x))) for w in model['weights']]
def metrics(predictions,labels):
 n=len(labels)
 if not n:return {'n':0}
 accuracy=sum((p>=.5)==bool(y) for p,y in zip(predictions,labels))/n
 brier=sum((p-y)**2 for p,y in zip(predictions,labels))/n
 ece=0
 for i in range(10):
  pairs=[(p,y) for p,y in zip(predictions,labels) if i/10<=p<(i+1)/10 or i==9 and p==1]
  if pairs:ece+=len(pairs)/n*abs(sum(p for p,y in pairs)/len(pairs)-sum(y for p,y in pairs)/len(pairs))
 return {'n':n,'accuracy':round(accuracy,4),'brier':round(brier,4),'ece_10_bins':round(ece,4),'log_loss':round(-sum(y*math.log(max(1e-8,p))+(1-y)*math.log(max(1e-8,1-p)) for p,y in zip(predictions,labels))/n,4)}
def calibrate(pred,rows):
 temps=[]
 for skill in range(5):
  def score(t):
   ps=[sigmoid(math.log(max(1e-7,min(1-1e-7,pred(r)[skill]))/(1-max(1e-7,min(1-1e-7,pred(r)[skill]))))/t) for r in rows]
   return metrics(ps,[r['labels'][skill] for r in rows])['log_loss']
  temps.append(min([.5,.75,1,1.25,1.5,2],key=score))
 return temps
def probabilities(pred,r,temps):
 return [sigmoid(math.log(max(1e-7,min(1-1e-7,p))/(1-max(1e-7,min(1-1e-7,p))))/t) for p,t in zip(pred(r),temps)]
def evaluate(model,rows):
 validation=[r for r in rows if r['split']=='validation'];test=[r for r in rows if r['split']=='test']
 methods={'rules_raw':lambda r:r['features'][:5],'logistic_raw':lambda r:predict(model,r)};result={};temperatures={}
 for name,pred in list(methods.items()):
  ts=calibrate(pred,validation);temperatures[name]=ts
  methods[name+'_calibrated']=lambda r,pred=pred,ts=ts:probabilities(pred,r,ts)
 for name,pred in methods.items():
  overall={s:metrics([pred(r)[i] for r in test],[r['labels'][i] for r in test]) for i,s in enumerate(SKILLS)}
  subgroups={}
  for field,values in [('difficulty',['Easy','Medium','Hard']),('language',['en','ta'])]:
   for v in values:
    group=[r for r in test if r[field]==v]
    subgroups[field+'='+v]={s:metrics([pred(r)[i] for r in group],[r['labels'][i] for r in group]) for i,s in enumerate(SKILLS)}
  result[name]={'test':overall,'synthetic_subgroups':subgroups,'macro_accuracy':round(sum(v['accuracy'] for v in overall.values())/5,4),'macro_brier':round(sum(v['brier'] for v in overall.values())/5,4)}
 return {'methods':result,'validation_temperatures':temperatures,'test_used_for_fit_or_selection':False,'scope':'synthetic next-task success only; not astronaut proficiency or human language fairness'}
