import fs from 'node:fs';
import validator from 'gltf-validator';
const dir='work/stage08d/Optimized';const results=[];
for(const f of fs.readdirSync(dir).filter(f=>f.endsWith('.glb'))){const r=await validator.validateBytes(new Uint8Array(fs.readFileSync(dir+'/'+f)),{uri:f});results.push({file:f,issues:r.issues});console.log(f,r.issues.numErrors,r.issues.numWarnings);}
fs.writeFileSync(dir+'/Validation_Results.json',JSON.stringify(results,null,2));
