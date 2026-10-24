// Separate QA harness only. No scene/runtime gameplay script or final HUD.
(() => {
 let draws=0,triangles=0,programs=0,started=0,lastDraw=0,rows=[],armed=false;
 const contexts=new Set(); const original=HTMLCanvasElement.prototype.getContext;
 HTMLCanvasElement.prototype.getContext=function(...args){const gl=original.apply(this,args);if(!gl||!String(args[0]).includes('webgl')||contexts.has(gl))return gl;contexts.add(gl);
  const info=gl.getExtension('WEBGL_debug_renderer_info'); const renderer=info?gl.getParameter(info.UNMASKED_RENDERER_WEBGL):gl.getParameter(gl.RENDERER);
  document.getElementById('qa-hardware').textContent=JSON.stringify({userAgent:navigator.userAgent,renderer,canvas:[this.width,this.height],devicePixelRatio:devicePixelRatio});
  for(const n of ['drawElements','drawArrays','drawElementsInstanced','drawArraysInstanced']){const f=gl[n];if(!f)continue;gl[n]=function(...p){draws++;const count=n.includes('Elements')?p[1]:p[2];const instances=n.endsWith('Instanced')?p[p.length-1]:1;if(p[0]===gl.TRIANGLES)triangles+=count/3*instances;else if(p[0]===gl.TRIANGLE_STRIP||p[0]===gl.TRIANGLE_FAN)triangles+=Math.max(0,count-2)*instances;return f.apply(this,p);};}
  const use=gl.useProgram;gl.useProgram=function(...p){programs++;return use.apply(this,p);};return gl;};
 window.qaUnityLoaded=()=>{armed=true;started=performance.now();document.getElementById('qa-result').textContent='Unity loaded. Warming 5 seconds, then sampling 20 seconds.';};
 function frame(now){if(armed&&now-started>5000&&now-started<25000&&lastDraw&&draws>0)rows.push({ms:now-lastDraw,draws,triangles,programs});if(draws>0)lastDraw=now;if(armed&&now-started>=25000){armed=false;const sorted=rows.map(r=>r.ms).sort((a,b)=>a-b);const mean=k=>rows.reduce((a,r)=>a+r[k],0)/Math.max(1,rows.length);const out={scope:'Stationary arrival camera, 5s warmup + 20s sample; browser presentation cadence and actual intercepted WebGL submissions. Instrumentation overhead present. Not Unity Profiler SetPass/batches or GPU timings.',samples:rows.length,meanFrameMs:mean('ms'),p50FrameMs:sorted[Math.floor(sorted.length*.5)],p95FrameMs:sorted[Math.floor(sorted.length*.95)],approxPresentedFPS:1000/mean('ms'),meanGLDrawSubmissions:mean('draws'),meanSubmittedTrianglesIncludingPasses:mean('triangles'),meanUseProgramCalls:mean('programs'),canvas:[document.getElementById('unity-canvas').width,document.getElementById('unity-canvas').height]};document.getElementById('qa-result').textContent=JSON.stringify(out,null,2);}
 draws=0;triangles=0;programs=0;requestAnimationFrame(frame);}
 requestAnimationFrame(frame);
})();
