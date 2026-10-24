import fs from 'node:fs';import sharp from 'sharp';import crypto from 'node:crypto';
const target='work/stage08d1/VisualEvaluation/Assets/Stage08D2';fs.mkdirSync(target+'/Textures',{recursive:true});fs.mkdirSync(target+'/Materials',{recursive:true});
const src='work/stage08d11/Converted/Perseverance_LOD0_PNG.glb',b=fs.readFileSync(src),len=b.readUInt32LE(12),j=JSON.parse(b.subarray(20,20+len)),bin=b.subarray(28+len);
const rawTex=async ti=>{const t=j.textures[ti],im=j.images[t.source],bv=j.bufferViews[im.bufferView];return sharp(bin.subarray(bv.byteOffset||0,(bv.byteOffset||0)+bv.byteLength)).ensureAlpha().raw().toBuffer({resolveWithObject:true});};
const linear=x=>x<=.04045?x/12.92:Math.pow((x+.055)/1.055,2.4);const srgb=x=>x<=.0031308?x*12.92:1.055*Math.pow(x,1/2.4)-.055;const enc=x=>Math.round(Math.max(0,Math.min(1,srgb(x)))*255);
let manifest={source:src,materials:[]};
for(const [index,m] of j.materials.entries()){
 const e=m.extensions||{};if(!e.KHR_materials_ior&&!e.KHR_materials_specular)continue;
 const p=m.pbrMetallicRoughness,metal=p.metallicFactor??1,rough=p.roughnessFactor??1,base=p.baseColorFactor??[1,1,1,1],spec=e.KHR_materials_specular||{},ior=e.KHR_materials_ior?.ior??1.5;
 if(p.metallicRoughnessTexture||m.occlusionTexture||m.emissiveTexture)throw Error('Unhandled channels in adapted material');
 const color=p.baseColorTexture?await rawTex(p.baseColorTexture.index):{data:Buffer.from([255,255,255,255]),info:{width:1,height:1}},scol=spec.specularColorTexture?await rawTex(spec.specularColorTexture.index):null;
 const {width:w,height:h}=color.info;let diffuse=Buffer.alloc(w*h*4),reflect=Buffer.alloc(w*h*4);const f0=Math.pow((ior-1)/(ior+1),2),factor=spec.specularFactor??1,sc=spec.specularColorFactor??[1,1,1];
 for(let y=0;y<h;y++)for(let x=0;x<w;x++){let C=[],D=[],F=[];for(let c=0;c<3;c++){C[c]=linear(color.data[(y*w+x)*4+c]/255)*base[c];const S=scol?linear(scol.data[(Math.min(scol.info.height-1,Math.floor(y/h*scol.info.height))*scol.info.width+Math.min(scol.info.width-1,Math.floor(x/w*scol.info.width)))*4+c]/255):1;D[c]=Math.min(1,f0*sc[c]*S)*factor;F[c]=D[c]*(1-metal)+C[c]*metal;}
 const maxD=Math.max(...D),maxF=Math.max(...F);for(let c=0;c<3;c++){diffuse[(y*w+x)*4+c]=enc(C[c]*(1-metal)*(1-maxD)/Math.max(.0001,1-maxF));reflect[(y*w+x)*4+c]=enc(F[c]);}diffuse[(y*w+x)*4+3]=Math.round(color.data[(y*w+x)*4+3]*base[3]);reflect[(y*w+x)*4+3]=Math.round((1-rough)*255);}
 const safe=m.name.replace(/[^a-zA-Z0-9_-]/g,'_'),prefix=target+'/Textures/'+safe;await sharp(diffuse,{raw:{width:w,height:h,channels:4}}).png().toFile(prefix+'_Base.png');await sharp(reflect,{raw:{width:w,height:h,channels:4}}).png().toFile(prefix+'_SpecSmooth.png');
 let normalPath='';if(m.normalTexture){const n=await rawTex(m.normalTexture.index);await sharp(n.data,{raw:{width:n.info.width,height:n.info.height,channels:4}}).png().toFile(prefix+'_Normal.png');normalPath='Assets/Stage08D2/Textures/'+safe+'_Normal.png';}
 manifest.materials.push({index,name:m.name,unityName:safe,basePath:'Assets/Stage08D2/Textures/'+safe+'_Base.png',specPath:'Assets/Stage08D2/Textures/'+safe+'_SpecSmooth.png',normalPath,normalScale:m.normalTexture?.scale??1,alphaMode:m.alphaMode??'OPAQUE',doubleSided:!!m.doubleSided,ior,dielectricF0:f0,specularFactor:factor,metallicFactor:metal,roughnessFactor:rough,transmission:e.KHR_materials_transmission?.transmissionFactor??0,width:w,height:h,sourceMaterial:m});
}
fs.writeFileSync(target+'/Material_Adapter_Manifest.json',JSON.stringify(manifest,null,2));fs.writeFileSync('outputs/Stage08D2/Material_Adapter_Evidence.json',JSON.stringify(manifest,null,2));console.log(manifest.materials.map(m=>[m.name,m.width,m.height]));
