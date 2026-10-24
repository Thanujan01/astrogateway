import json

with open('/Users/kapilash/.gemini/antigravity/scratch/ARES_EVA_MK1/concept/concept_blockout.json') as f:
    concept_json = f.read()

template = '''<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <script src="https://www.gstatic.com/antigravity/web/dev/tailwindcss.min.js"></script>
  <style>
    canvas { touch-action: none; user-select: none; -webkit-user-select: none; }
  </style>
</head>
<body class="bg-transparent text-[var(--foreground)] antialiased p-2">
  <div class="bg-[var(--card)] text-[var(--foreground)] border border-[var(--border)] rounded-2xl p-3.5 shadow-xl max-w-2xl mx-auto backdrop-blur-md">
    
    <!-- Top Bar -->
    <div class="flex items-center justify-between pb-2.5 border-b border-[var(--border)]">
      <div class="flex items-center gap-2.5">
        <div class="w-8 h-8 rounded-lg bg-orange-500/20 border border-orange-500/40 flex items-center justify-center font-bold text-orange-400 text-xs">
          EVA
        </div>
        <div>
          <div class="flex items-center gap-2">
            <h2 class="text-sm font-bold tracking-wide uppercase text-[var(--foreground)]">ARES EVA MK-I Astronaut</h2>
            <span class="px-1.5 py-0.5 rounded text-[10px] font-semibold tracking-wider bg-orange-500/20 text-orange-400 border border-orange-500/40">CONCEPT STAGE</span>
          </div>
          <p class="text-[11px] text-[var(--muted-foreground)]">AstroGateway Mars Exploration Suit &bull; 1.84m Suited Humanoid</p>
        </div>
      </div>

      <div class="flex items-center gap-1.5 text-xs">
        <span class="px-2 py-0.5 rounded-md bg-[var(--background)] border border-[var(--border)] text-cyan-400 font-mono text-[11px]">3D Interactive Turnaround</span>
      </div>
    </div>

    <!-- 3D Canvas Viewport -->
    <div class="relative mt-2.5 rounded-xl overflow-hidden bg-slate-950 border border-[var(--border)] flex items-center justify-center">
      <canvas id="renderCanvas" width="600" height="260" class="w-full h-[260px] cursor-grab active:cursor-grabbing block"></canvas>
      
      <!-- Top HUD Overlay -->
      <div class="absolute top-2 left-2 flex gap-1.5 pointer-events-none z-10">
        <div class="px-2 py-0.5 rounded bg-black/70 backdrop-blur-md text-[10px] text-slate-300 border border-white/10 font-mono">
          HEIGHT: <span class="text-orange-400 font-bold">1.84m</span> | TRIS: <span class="text-cyan-400 font-bold">2,032</span>
        </div>
      </div>

      <!-- Quick Camera Angles Strip (Top Right) -->
      <div class="absolute top-2 right-2 flex gap-1 z-10">
        <button data-cam="full" class="btnCam px-1.5 py-0.5 text-[10px] font-medium rounded bg-black/60 text-slate-300 border border-white/10 hover:bg-black/90 hover:text-white transition">Full Body</button>
        <button data-cam="helmet" class="btnCam px-1.5 py-0.5 text-[10px] font-medium rounded bg-black/60 text-slate-300 border border-white/10 hover:bg-black/90 hover:text-white transition">Helmet</button>
        <button data-cam="chest" class="btnCam px-1.5 py-0.5 text-[10px] font-medium rounded bg-black/60 text-slate-300 border border-white/10 hover:bg-black/90 hover:text-white transition">Chest DCM</button>
        <button data-cam="plss" class="btnCam px-1.5 py-0.5 text-[10px] font-medium rounded bg-black/60 text-slate-300 border border-white/10 hover:bg-black/90 hover:text-white transition">Backpack</button>
        <button data-cam="boots" class="btnCam px-1.5 py-0.5 text-[10px] font-medium rounded bg-black/60 text-slate-300 border border-white/10 hover:bg-black/90 hover:text-white transition">Boots</button>
      </div>

      <!-- Bottom Controls Overlay -->
      <div class="absolute bottom-2 left-2 flex gap-1.5 z-10">
        <button id="btnAutoRotate" class="px-2 py-1 text-[11px] font-medium rounded-lg bg-black/70 backdrop-blur-md text-white border border-white/15 hover:bg-black transition flex items-center gap-1">
          <span>Rotate:</span><span id="rotateStatus" class="text-orange-400 font-bold">ON</span>
        </button>
        <button id="btnWireframe" class="px-2 py-1 text-[11px] font-medium rounded-lg bg-black/70 backdrop-blur-md text-white border border-white/15 hover:bg-black transition flex items-center gap-1">
          <span>Wire:</span><span id="wireStatus" class="text-slate-400 font-bold">OFF</span>
        </button>
        <button id="btnResetView" class="px-2 py-1 text-[11px] font-medium rounded-lg bg-black/70 backdrop-blur-md text-white border border-white/15 hover:bg-black transition">
          Reset
        </button>
      </div>

      <div class="absolute bottom-2 right-2 text-[10px] text-slate-400 bg-black/60 px-2 py-0.5 rounded backdrop-blur-md border border-white/10 pointer-events-none">
        Drag: Orbit &bull; Wheel: Zoom
      </div>
    </div>

    <!-- Subsystems Inspector -->
    <div class="mt-2.5 grid grid-cols-5 gap-1.5 text-xs">
      <button data-sub="helmet" class="subsystemBtn p-1.5 rounded-lg bg-[var(--background)] border border-[var(--border)] text-left hover:border-orange-500/50 transition">
        <div class="text-[9px] text-[var(--muted-foreground)] uppercase font-semibold">Helmet</div>
        <div class="text-[11px] font-bold text-[var(--foreground)] truncate">Gold Sun Visor</div>
      </button>

      <button data-sub="torso" class="subsystemBtn p-1.5 rounded-lg bg-[var(--background)] border border-[var(--border)] text-left hover:border-orange-500/50 transition">
        <div class="text-[9px] text-[var(--muted-foreground)] uppercase font-semibold">Torso</div>
        <div class="text-[11px] font-bold text-[var(--foreground)] truncate">Chest DCM Module</div>
      </button>

      <button data-sub="plss" class="subsystemBtn p-1.5 rounded-lg bg-[var(--background)] border border-[var(--border)] text-left hover:border-orange-500/50 transition">
        <div class="text-[9px] text-[var(--muted-foreground)] uppercase font-semibold">Backpack</div>
        <div class="text-[11px] font-bold text-[var(--foreground)] truncate">Dual O2 Life Support</div>
      </button>

      <button data-sub="arms" class="subsystemBtn p-1.5 rounded-lg bg-[var(--background)] border border-[var(--border)] text-left hover:border-orange-500/50 transition">
        <div class="text-[9px] text-[var(--muted-foreground)] uppercase font-semibold">Limbs</div>
        <div class="text-[11px] font-bold text-[var(--foreground)] truncate">Elbow Bellows & Gloves</div>
      </button>

      <button data-sub="boots" class="subsystemBtn p-1.5 rounded-lg bg-[var(--background)] border border-[var(--border)] text-left hover:border-orange-500/50 transition">
        <div class="text-[9px] text-[var(--muted-foreground)] uppercase font-semibold">Boots</div>
        <div class="text-[11px] font-bold text-[var(--foreground)] truncate">Mars Cleated Soles</div>
      </button>
    </div>

    <!-- Inspector Drawer -->
    <div id="inspectorPanel" class="mt-2 p-2 rounded-xl bg-[var(--background)] border border-[var(--border)] text-xs flex items-center justify-between">
      <div class="flex items-center gap-2">
        <div class="w-5 h-5 rounded bg-orange-500/20 border border-orange-500/40 flex items-center justify-center text-orange-400 font-bold text-xs">
          &#9776;
        </div>
        <div>
          <span id="inspectTitle" class="font-bold text-[var(--foreground)]">ARES EVA MK-I Suit Proportions</span>
          <span class="text-[var(--muted-foreground)] mx-1">&bull;</span>
          <span id="inspectDesc" class="text-[var(--muted-foreground)]">1.84m Height, broad EVA silhouette, compatible with rover cockpit.</span>
        </div>
      </div>
      <div class="text-[10px] font-mono text-orange-400 bg-orange-950/40 border border-orange-800/40 px-2 py-0.5 rounded">
        SCALE: 1:1 METRIC
      </div>
    </div>
  </div>

  <script>
    const conceptData = %CONCEPT_DATA%;

    const canvas = document.getElementById('renderCanvas');
    const ctx = canvas.getContext('2d');

    let yaw = 0.55;
    let pitch = 0.15;
    let zoom = 1.0;
    let panY = 0;

    let targetYaw = yaw;
    let targetPitch = pitch;
    let targetZoom = zoom;
    let targetPanY = panY;

    let autoRotate = true;
    let wireframe = false;
    let isDragging = false;
    let lastX = 0, lastY = 0;

    const subsystemInfo = {
      helmet: { title: "Helmet & Gold-Vapor Bubble Visor", desc: "Polycarbonate pressure dome, gold reflective layer, dual comm pods, and high-output LED spotlights." },
      torso: { title: "Layered Pressure Torso & Chest DCM", desc: "4-layer ortho-fabric, chest Display & Control Module with OLED telemetry readouts and orange strakes." },
      plss: { title: "Portable Life Support System (PLSS)", desc: "Dual high-pressure 25MPa O2 tanks, sublimator cooling, CO2 scrubbers, and umbilical chest hoses." },
      arms: { title: "Shoulders, Elbow Bellows & EVA Gloves", desc: "Rotating shoulder disconnects, accordion elbow joints for 120 deg flexion, and tactile silicone grip gloves." },
      boots: { title: "Mars Exploration Boots & Knee Pads", desc: "Articulated knee armor with flex bellows, vulcanized cleated soles, and dust-resistant ankle gaiters." }
    };

    function resizeCanvas() {
      const rect = canvas.getBoundingClientRect();
      const dpr = window.devicePixelRatio || 1;
      canvas.width = rect.width * dpr;
      canvas.height = rect.height * dpr;
    }
    resizeCanvas();
    window.addEventListener('resize', resizeCanvas);

    canvas.addEventListener('mousedown', (e) => {
      isDragging = true;
      autoRotate = false;
      document.getElementById('rotateStatus').innerText = 'OFF';
      document.getElementById('rotateStatus').className = 'text-slate-400 font-bold';
      lastX = e.clientX;
      lastY = e.clientY;
    });

    window.addEventListener('mouseup', () => isDragging = false);
    canvas.addEventListener('contextmenu', e => e.preventDefault());

    window.addEventListener('mousemove', (e) => {
      if (!isDragging) return;
      const dx = e.clientX - lastX;
      const dy = e.clientY - lastY;
      lastX = e.clientX;
      lastY = e.clientY;
      targetYaw += dx * 0.010;
      targetPitch = Math.max(-1.1, Math.min(1.1, targetPitch + dy * 0.010));
    });

    canvas.addEventListener('wheel', (e) => {
      e.preventDefault();
      targetZoom = Math.max(0.6, Math.min(2.8, targetZoom - e.deltaY * 0.0015));
    }, { passive: false });

    // Touch Support
    canvas.addEventListener('touchstart', (e) => {
      if (e.touches.length === 1) {
        isDragging = true;
        autoRotate = false;
        document.getElementById('rotateStatus').innerText = 'OFF';
        document.getElementById('rotateStatus').className = 'text-slate-400 font-bold';
        lastX = e.touches[0].clientX;
        lastY = e.touches[0].clientY;
      }
    });
    window.addEventListener('touchend', () => isDragging = false);
    window.addEventListener('touchmove', (e) => {
      if (!isDragging || e.touches.length !== 1) return;
      const dx = e.touches[0].clientX - lastX;
      const dy = e.touches[0].clientY - lastY;
      lastX = e.touches[0].clientX;
      lastY = e.touches[0].clientY;
      targetYaw += dx * 0.012;
      targetPitch = Math.max(-1.1, Math.min(1.1, targetPitch + dy * 0.012));
    });

    // Camera Presets
    document.querySelectorAll('.btnCam').forEach(btn => {
      btn.addEventListener('click', () => {
        const c = btn.getAttribute('data-cam');
        autoRotate = false;
        document.getElementById('rotateStatus').innerText = 'OFF';
        document.getElementById('rotateStatus').className = 'text-slate-400 font-bold';
        if (c === 'full') { targetYaw = 0.55; targetPitch = 0.15; targetZoom = 1.0; targetPanY = 0; }
        else if (c === 'helmet') { targetYaw = 0.20; targetPitch = 0.10; targetZoom = 2.4; targetPanY = 160; }
        else if (c === 'chest') { targetYaw = 0.30; targetPitch = 0.12; targetZoom = 1.9; targetPanY = 80; }
        else if (c === 'plss') { targetYaw = Math.PI + 0.3; targetPitch = 0.15; targetZoom = 1.7; targetPanY = 70; }
        else if (c === 'boots') { targetYaw = 0.40; targetPitch = -0.20; targetZoom = 2.0; targetPanY = -140; }
      });
    });

    document.getElementById('btnAutoRotate').addEventListener('click', () => {
      autoRotate = !autoRotate;
      document.getElementById('rotateStatus').innerText = autoRotate ? 'ON' : 'OFF';
      document.getElementById('rotateStatus').className = autoRotate ? 'text-orange-400 font-bold' : 'text-slate-400 font-bold';
    });

    document.getElementById('btnWireframe').addEventListener('click', () => {
      wireframe = !wireframe;
      document.getElementById('wireStatus').innerText = wireframe ? 'ON' : 'OFF';
      document.getElementById('wireStatus').className = wireframe ? 'text-cyan-400 font-bold' : 'text-slate-400 font-bold';
    });

    document.getElementById('btnResetView').addEventListener('click', () => {
      targetYaw = 0.55; targetPitch = 0.15; targetZoom = 1.0; targetPanY = 0;
    });

    document.querySelectorAll('.subsystemBtn').forEach(btn => {
      btn.addEventListener('click', () => {
        const key = btn.getAttribute('data-sub');
        const info = subsystemInfo[key];
        if (info) {
          document.getElementById('inspectTitle').innerText = info.title;
          document.getElementById('inspectDesc').innerText = info.desc;
        }
      });
    });

    const materials = {
      Mat_CeramicWhite: { r: 229, g: 230, b: 226, stroke: '#9aa0a6', spec: 0.55 }, # #E5E6E2
      Mat_Graphite:     { r: 37,  g: 43,  b: 50,  stroke: '#1d222a', spec: 0.35 }, # #252B32
      Mat_Titanium:     { r: 137, g: 147, b: 155, stroke: '#616972', spec: 0.88 }, # #89939B
      Mat_SafetyOrange: { r: 217, g: 106, b: 41,  stroke: '#b0521c', spec: 0.65 }, # #D96A29
      Mat_GoldVisor:    { r: 242, g: 196, b: 38,  stroke: '#ffe27d', spec: 0.98 },
      Mat_CyanEnergy:   { r: 0,   g: 201, b: 232, stroke: '#99f0ff', glow: true, spec: 1.0 }
    };

    function render() {
      yaw += (targetYaw - yaw) * 0.12;
      pitch += (targetPitch - pitch) * 0.12;
      zoom += (targetZoom - zoom) * 0.12;
      panY += (targetPanY - panY) * 0.12;

      if (autoRotate && !isDragging) {
        targetYaw += 0.008;
        yaw = targetYaw;
      }

      const w = canvas.width;
      const h = canvas.height;
      ctx.clearRect(0, 0, w, h);

      // Studio Background
      const bgGrad = ctx.createRadialGradient(w/2, h/2, 20, w/2, h/2, Math.max(w,h)*0.65);
      bgGrad.addColorStop(0, '#10141d');
      bgGrad.addColorStop(0.65, '#07090e');
      bgGrad.addColorStop(1, '#020305');
      ctx.fillStyle = bgGrad;
      ctx.fillRect(0, 0, w, h);

      const cx = w / 2;
      const cy = (h / 2) + panY + (85 * zoom);

      // Floor Shadow
      ctx.save();
      ctx.beginPath();
      ctx.ellipse(cx, cy + (10 * zoom), 95 * zoom, 28 * zoom, 0, 0, Math.PI * 2);
      ctx.fillStyle = 'rgba(0, 0, 0, 0.65)';
      ctx.filter = 'blur(8px)';
      ctx.fill();
      ctx.restore();

      const cosY = Math.cos(yaw), sinY = Math.sin(yaw);
      const cosP = Math.cos(pitch), sinP = Math.sin(pitch);

      const dist = 3.8;
      const baseScale = (Math.min(w, h) * 1.15) * zoom;
      const centerZ = 0.98; // Humanoid midpoint

      const drawList = [];

      for (const [subId, sub] of Object.entries(conceptData.subsystems)) {
        const mat = materials[sub.mat] || materials.Mat_CeramicWhite;

        for (let fi = 0; fi < sub.faces.length; fi++) {
          const face = sub.faces[fi];
          const pTrans = [];
          const pProj = [];
          let avgY = 0;

          for (let vi = 0; vi < face.length; vi++) {
            const vIdx = face[vi];
            const v = conceptData.vertices[vIdx];

            const x0 = v[0];
            const y0 = v[1];
            const z0 = v[2] - centerZ;

            const x1 = x0 * cosY - y0 * sinY;
            const y1 = x0 * sinY + y0 * cosY;
            const z1 = z0;

            const x2 = x1;
            const y2 = y1 * cosP - z1 * sinP;
            const z2 = y1 * sinP + z1 * cosP;

            const depth = y2 + dist;
            const invDepth = depth > 0.1 ? 1 / depth : 1;
            const px = cx + (x2 * invDepth) * baseScale;
            const py = (h / 2) + panY - (z2 * invDepth) * baseScale;

            pTrans.push([x2, y2, z2]);
            pProj.push([px, py]);
            avgY += y2;
          }
          avgY /= face.length;

          // Normal & Culling
          const p0 = pTrans[0], p1 = pTrans[1], p2 = pTrans[2];
          const ax = p1[0] - p0[0], ay = p1[1] - p0[1], az = p1[2] - p0[2];
          const bx = p2[0] - p0[0], by = p2[1] - p0[1], bz = p2[2] - p0[2];
          const nx = ay * bz - az * by;
          const ny = az * bx - ax * bz;
          const nz = ax * by - ay * bx;

          const dotView = nx * p0[0] + ny * (p0[1] + dist) + nz * p0[2];
          if (!mat.glow && dotView <= 0) continue;

          const nLen = Math.sqrt(nx*nx + ny*ny + nz*nz) || 1;
          const normX = nx / nLen, normY = ny / nLen, normZ = nz / nLen;

          const dotLight = Math.max(0, normX * 0.45 + normY * -0.70 + normZ * 0.65);
          const specPower = Math.pow(Math.max(0, normX * 0.35 + normY * -0.65 + normZ * 0.70), 10) * mat.spec;

          drawList.push({
            pProj,
            mat,
            avgY,
            dotLight,
            specPower,
            glow: mat.glow
          });
        }
      }

      drawList.sort((a, b) => b.avgY - a.avgY);

      for (let i = 0; i < drawList.length; i++) {
        const item = drawList[i];
        const pts = item.pProj;
        const mat = item.mat;

        ctx.beginPath();
        ctx.moveTo(pts[0][0], pts[0][1]);
        for (let j = 1; j < pts.length; j++) {
          ctx.lineTo(pts[j][0], pts[j][1]);
        }
        ctx.closePath();

        if (wireframe) {
          ctx.strokeStyle = mat.glow ? '#00f0ff' : (mat.r > 200 ? '#ffffff' : '#64748b');
          ctx.lineWidth = mat.glow ? 1.5 : 0.8;
          ctx.stroke();
        } else {
          if (mat.glow) {
            ctx.fillStyle = '#00c9e8';
            ctx.shadowColor = '#00e5ff';
            ctx.shadowBlur = 10;
            ctx.fill();
            ctx.shadowBlur = 0;
            ctx.strokeStyle = '#cffafe';
            ctx.lineWidth = 0.8;
            ctx.stroke();
          } else {
            const ambient = 0.32;
            const diffuse = 0.68 * item.dotLight;
            const intensity = ambient + diffuse;
            const r = Math.min(255, Math.floor(mat.r * intensity + item.specPower * 90));
            const g = Math.min(255, Math.floor(mat.g * intensity + item.specPower * 90));
            const b = Math.min(255, Math.floor(mat.b * intensity + item.specPower * 90));

            ctx.fillStyle = `rgb(${r},${g},${b})`;
            ctx.fill();
            ctx.strokeStyle = mat.stroke;
            ctx.lineWidth = 0.55;
            ctx.stroke();
          }
        }
      }

      requestAnimationFrame(render);
    }

    requestAnimationFrame(render);
  </script>
</body>
</html>
'''

output = template.replace('%CONCEPT_DATA%', concept_json)
out_path = '/Users/kapilash/.gemini/antigravity/brain/fe04a22b-3e01-409f-a35c-ed439568d853/ares_eva_mk1_viewer.html'
with open(out_path, 'w') as f:
    f.write(output)

print(f"Generated ARES EVA MK-I Concept HTML at {out_path} ({len(output):,} bytes)")
