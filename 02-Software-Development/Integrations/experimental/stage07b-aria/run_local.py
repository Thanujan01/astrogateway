from pathlib import Path
import json,hashlib
from stage07b.dataset import generate,gate,digest
from stage07b.model import fit,evaluate
from stage07b.aria import evaluate as aria_eval
from adaptive.engine import simulate
root=Path(__file__).resolve().parents[1]
def write(path,obj):
 path.write_text(json.dumps(obj,indent=2,sort_keys=True)+'\n')
rows=generate()
data=('\n'.join(json.dumps(r,sort_keys=True,separators=(',',':')) for r in rows)+'\n').encode()
protocol=root/'Evaluation_Protocol.md';source=Path(__file__).parent/'stage07b/dataset.py'
manifest={'source':'project-synthetic-generator-07b.1','authorized_use':'local_synthetic_experiment','scientific_observations_used':False,'real_player_data_used':False,'sha256':digest(data),'evaluation_protocol_sha256':digest(protocol.read_bytes()),'generator_sha256':digest(source.read_bytes()),'seed':707,'license':'Project-generated local synthetic experiment; no external-data license inferred','created_at':'2026-10-09','limitations':'Synthetic labels are not measured astronaut skills'}
(root/'Datasets/synthetic_training.jsonl').write_bytes(data)
write(root/'Datasets/synthetic_manifest.json',manifest)
assert manifest['generator_sha256']==digest(source.read_bytes()) and manifest['evaluation_protocol_sha256']==digest(protocol.read_bytes())
checks=gate(rows,manifest);write(root/'Evidence/training_gate.json',checks)
model=fit(rows);write(root/'Evidence/baseline_model.json',model)
write(root/'Evidence/baseline_evaluation.json',evaluate(model,rows))
write(root/'Evidence/aria_evaluation.json',aria_eval())
scenarios=[{'seed':s,'difficulty':d,'synthetic':True,'scenario':simulate(s,d)} for s in (707,708,709) for d in ('Easy','Medium','Hard')]
write(root/'Datasets/synthetic_scenarios.json',scenarios)
print(json.dumps({'gate':checks,'model_sha256':digest((root/'Evidence/baseline_model.json').read_bytes()),'scope':'synthetic-only offline experiment'}))
