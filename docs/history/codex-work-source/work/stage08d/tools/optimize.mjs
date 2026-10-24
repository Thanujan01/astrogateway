import {NodeIO} from '@gltf-transform/core';
import {ALL_EXTENSIONS} from '@gltf-transform/extensions';
import {dedup,weld,simplify,textureCompress} from '@gltf-transform/functions';
import {MeshoptSimplifier} from 'meshoptimizer';
import sharp from 'sharp';
import draco from 'draco3dgltf';
import fs from 'node:fs';
import crypto from 'node:crypto';
const base='work/stage08d/Acquired', out='work/stage08d/Optimized';
const walk=p=>fs.readdirSync(p,{withFileTypes:true}).flatMap(e=>e.isDirectory()?walk(p+'/'+e.name):[p+'/'+e.name]);
const hashes=()=>Object.fromEntries(walk(base).map(p=>[p,crypto.createHash('sha256').update(fs.readFileSync(p)).digest('hex')]));
const before=hashes(); const io=new NodeIO().registerExtensions(ALL_EXTENSIONS); io.registerDependencies({'draco3d.decoder':await draco.createDecoderModule()}); await MeshoptSimplifier.ready;
function stats(d){const r=d.getRoot(); return {triangles:r.listMeshes().reduce((n,m)=>n+m.listPrimitives().reduce((s,p)=>s+(p.getIndices()?.getCount()??p.getAttribute('POSITION').getCount())/3,0),0),meshes:r.listMeshes().length,materials:r.listMaterials().length,textures:r.listTextures().length,skins:r.listSkins().length,animations:r.listAnimations().length,nodes:r.listNodes().length};}
const results=[];
for(const [name,path] of [['rock_07',base+'/rock_07/rock_07.gltf'],['rock_09',base+'/rock_09/rock_09.gltf'],['Perseverance',base+'/Perseverance.glb']]){
 const original=stats(await io.read(path));
 for(const [lod,ratio,error,size] of [[0,.4,.005,1024],[1,.15,.01,512],[2,.05,.02,256]]){
 const d=await io.read(path); await d.transform(dedup(),weld(),simplify({simplifier:MeshoptSimplifier,ratio,error}),textureCompress({encoder:sharp,resize:[size,size],chromaSubsampling:'4:4:4'}));
 for(const m of d.getRoot().listMeshes())for(const p of m.listPrimitives()){
 const w=p.getAttribute('WEIGHTS_0');if(w){const a=w.getArray();for(let i=0;i<a.length;i+=4){const sum=a[i]+a[i+1]+a[i+2]+a[i+3];if(sum>0)for(let j=0;j<4;j++)a[i+j]/=sum;}w.setArray(a);}
 }
 for(const ext of d.getRoot().listExtensionsUsed()) if(ext.extensionName==='KHR_draco_mesh_compression')ext.dispose();
 const file=`${out}/${name}_LOD${lod}.glb`; await io.write(file,d);
 const loaded=await io.read(file); const metrics=stats(loaded);
 const dims=await Promise.all(loaded.getRoot().listTextures().map(async t=>{const m=await sharp(t.getImage()).metadata();return [m.width,m.height];}));
 results.push({name,lod,requestedRatio:ratio,errorLimit:error,original,...metrics,file,bytes:fs.statSync(file).size,textureDimensions:dims,sha256:crypto.createHash('sha256').update(fs.readFileSync(file)).digest('hex')});
 console.log(name,lod,metrics.triangles,metrics.materials);
 }
}
const unchanged=JSON.stringify(before)===JSON.stringify(hashes());
fs.writeFileSync(out+'/Optimization_Metrics.json',JSON.stringify({sourceHashes:before,sourceUnchanged:unchanged,results},null,2));
if(!unchanged)throw Error('Source changed');
