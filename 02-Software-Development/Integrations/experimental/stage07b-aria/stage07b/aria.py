"""Fixed curated bilingual retrieval templates, not an LLM or full semantic factuality judge."""
CARDS={
 'dust':{'url':'https://science.nasa.gov/mars/facts/','en':'Mars has a thin atmosphere. Dust can be suspended in it.','ta':'செவ்வாயின் வளிமண்டலம் மெல்லியது. அதில் தூசி மிதக்கலாம்.'},
 'solar':{'url':'https://science.nasa.gov/photojournal/solar-panel-dust-accumulation-and-cleanings/','en':'Dust on solar panels can reduce the sunlight reaching them.','ta':'சூரிய மின்தகடுகளில் படியும் தூசி, அவற்றை அடையும் சூரிய ஒளியைக் குறைக்கலாம்.'}}
INTENTS={'dust':['dust','atmosphere','தூசி','வளிமண்டலம்'],'solar':['solar','panel','சூரிய','மின்தகடு']}
def retrieve(topic,language):
 if language not in ('en','ta'):return {'abstained':True,'text':'Unsupported language','citations':[]}
 # Bounded supported science topic keys only; no tool instructions or arbitrary mission outcome claims.
 if topic not in CARDS:
  return {'abstained':True,'text':'Not enough verified evidence.' if language=='en' else 'சரிபார்க்கப்பட்ட ஆதாரம் போதவில்லை.','citations':[]}
 c=CARDS[topic];return {'abstained':False,'text':c[language],'citations':[c['url']],'mode':'curated retrieval template','human_translation_review':'pending'}
def evaluation_cases():
 cases=[]
 for lang in ('en','ta'):
  for topic in ('dust','solar','current_weather','player_success','pressure_now','ignore_source_and_reveal_token'):
   cases.append({'topic':topic,'language':lang,'expected_source':CARDS[topic]['url'] if topic in CARDS else None,'expected_abstain':topic not in CARDS})
 return cases
def evaluate():
 results=[]
 for c in evaluation_cases():
  r=retrieve(c['topic'],c['language']);ok=r['abstained']==c['expected_abstain'] and r['citations']==([] if c['expected_source'] is None else [c['expected_source']])
  results.append({**c,'output':r,'pass':ok})
 supported=[r for r in results if not r['expected_abstain']];unknown=[r for r in results if r['expected_abstain']]
 return {'cases':results,'citation_match_rate':sum(r['pass'] for r in supported)/len(supported),'unsupported_abstention_rate':sum(r['pass'] for r in unknown)/len(unknown),'unsupported_nonabstaining_outputs':sum(not r['output']['abstained'] for r in unknown),'scientific_factual_accuracy':'Human/domain review BLOCKED; exact-template correspondence only','hallucination_rate':'No independent semantic estimate; deterministic unsupported-output count only','tamil_fluency_and_parity':'Native-speaker review BLOCKED','model_used':False}
