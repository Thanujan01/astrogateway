import json

with open('/Users/kapilash/.gemini/antigravity/scratch/3d-game-assets/ares_vi_studio_data.json') as f:
    studio_json = f.read()

template = '''<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <script src="https://www.gstatic.com/antigravity/web/dev/tailwindcss.min.js"></script>
  <style>
    canvas {
      touch-action: none;
      user-select: none;
      -webkit-user-select: none;
    }
    .scrollbar-none::-webkit-scrollbar { display: none; }
    .scrollbar-none { -ms-overflow-style: none; scrollbar-width: none; }
  </style>
</head>
<body class="bg-transparent text-[var(--foreground)] antialiased p-2">
  <div class="bg-[var(--card)] text-[var(--foreground)] border border-[var(--border)] rounded-2xl p-3.5 shadow-xl max-w-2xl mx-auto backdrop-blur-md">
    
    <!-- Top Bar: Title & Mode Toggle -->
    <div class="flex items-center justify-between pb-2.5 border-b border-[var(--border)]">
      <div class="flex items-center gap-2.5">
        <div class="w-8 h-8 rounded-lg bg-orange-500/20 border border-orange-500/40 flex items-center justify-center font-bold text-orange-400 text-xs">
          A-VI
        </div>
        <div>
          <div class="flex items-center gap-2">
            <h2 class="text-sm font-bold tracking-wide uppercase text-[var(--foreground)]">ARES-VI Anti-Gravity Rover</h2>
            <span id="badgeMode" class="px-1.5 py-0.5 rounded text-[10px] font-semibold tracking-wider bg-orange-500/20 text-orange-400 border border-orange-500/40">MODE A: GROUND</span>
          </div>
          <p class="text-[11px] text-[var(--muted-foreground)]">AstroGateway Mars Exploration &bull; AAA Hard-Surface Platform</p>
        </div>
      </div>

      <!-- Mode Switcher Pill -->
      <div class="flex items-center bg-[var(--background)] p-0.5 rounded-xl border border-[var(--border)]">
        <button id="btnModeGround" class="px-2.5 py-1 text-xs font-semibold rounded-lg bg-orange-600 text-white transition shadow-sm">
          Ground
        </button>
        <button id="btnModeHover" class="px-2.5 py-1 text-xs font-semibold rounded-lg text-[var(--muted-foreground)] hover:text-[var(--foreground)] transition">
          Anti-Grav
        </button>
      </div>
    </div>

    <!-- 3D Canvas Viewport -->
    <div class="relative mt-2.5 rounded-xl overflow-hidden bg-slate-950 border border-[var(--border)] flex items-center justify-center">
      <canvas id="renderCanvas" width="600" height="270" class="w-full h-[270px] cursor-grab active:cursor-grabbing block"></canvas>
      
      <!-- Top HUD Overlay (Telemetry & Status) -->
      <div class="absolute top-2 left-2 flex gap-1.5 pointer-events-none z-10">
        <div class="px-2 py-0.5 rounded bg-black/70 backdrop-blur-md text-[10px] text-slate-300 border border-white/10 font-mono">
          TRIS: <span class="text-cyan-400 font-bold">1,401</span> | VERTS: <span class="text-cyan-400 font-bold">1,172</span>
        </div>
        <div id="hoverStatusHud" class="px-2 py-0.5 rounded bg-black/70 backdrop-blur-md text-[10px] text-slate-400 border border-white/10 font-mono hidden">
          THRUST: <span class="text-cyan-300 font-bold">4x EMITTERS ACTIVE</span>
        </div>
      </div>

      <!-- Quick Camera Angles Strip (Top Right) -->
      <div class="absolute top-2 right-2 flex gap-1 z-10">
        <button data-cam="hero" class="btnCam px-1.5 py-0.5 text-[10px] font-medium rounded bg-black/60 text-slate-300 border border-white/10 hover:bg-black/90 hover:text-white transition">Hero</button>
        <button data-cam="front" class="btnCam px-1.5 py-0.5 text-[10px] font-medium rounded bg-black/60 text-slate-300 border border-white/10 hover:bg-black/90 hover:text-white transition">Front</button>
        <button data-cam="side" class="btnCam px-1.5 py-0.5 text-[10px] font-medium rounded bg-black/60 text-slate-300 border border-white/10 hover:bg-black/90 hover:text-white transition">Side</button>
        <button data-cam="top" class="btnCam px-1.5 py-0.5 text-[10px] font-medium rounded bg-black/60 text-slate-300 border border-white/10 hover:bg-black/90 hover:text-white transition">Top</button>
        <button data-cam="under" class="btnCam px-1.5 py-0.5 text-[10px] font-medium rounded bg-black/60 text-cyan-300 border border-cyan-500/30 hover:bg-black/90 hover:text-white transition" title="Inspect Underbody Hover Emitters">Belly</button>
      </div>

      <!-- Bottom Controls Overlay -->
      <div class="absolute bottom-2 left-2 flex gap-1.5 z-10">
        <button id="btnAutoRotate" class="px-2 py-1 text-[11px] font-medium rounded-lg bg-black/70 backdrop-blur-md text-white border border-white/15 hover:bg-black transition flex items-center gap-1">
          <span>Rotate:</span><span id="rotateStatus" class="text-orange-400 font-bold">ON</span>
        </button>
        <button id="btnWireframe" class="px-2 py-1 text-[11px] font-medium rounded-lg bg-black/70 backdrop-blur-md text-white border border-white/15 hover:bg-black transition flex items-center gap-1">
          <span>Wire:</span><span id="wireStatus" class="text-slate-400 font-bold">OFF</span>
        </button>
        <button id="btnExplode" class="px-2 py-1 text-[11px] font-medium rounded-lg bg-black/70 backdrop-blur-md text-white border border-white/15 hover:bg-black transition flex items-center gap-1">
          <span>Exploded:</span><span id="explodeStatus" class="text-slate-400 font-bold">0%</span>
        </button>
        <button id="btnResetView" class="px-2 py-1 text-[11px] font-medium rounded-lg bg-black/70 backdrop-blur-md text-white border border-white/15 hover:bg-black transition">
          Reset
        </button>
      </div>

      <!-- Interaction Tip -->
      <div class="absolute bottom-2 right-2 text-[10px] text-slate-400 bg-black/60 px-2 py-0.5 rounded backdrop-blur-md border border-white/10 pointer-events-none">
        Drag: Orbit &bull; Right-Drag: Pan &bull; Wheel: Zoom
      </div>
    </div>

    <!-- Subsystems Inspector & Info Strip -->
    <div class="mt-2.5 grid grid-cols-5 gap-1.5 text-xs">
      <button data-sub="hull" class="subsystemBtn p-1.5 rounded-lg bg-[var(--background)] border border-[var(--border)] text-left hover:border-orange-500/50 transition active:scale-95">
        <div class="text-[9px] text-[var(--muted-foreground)] uppercase font-semibold">Chassis</div>
        <div class="text-[11px] font-bold text-[var(--foreground)] truncate">Ceramic Armor</div>
      </button>

      <button data-sub="cockpit" class="subsystemBtn p-1.5 rounded-lg bg-[var(--background)] border border-[var(--border)] text-left hover:border-orange-500/50 transition active:scale-95">
        <div class="text-[9px] text-[var(--muted-foreground)] uppercase font-semibold">Cockpit</div>
        <div class="text-[11px] font-bold text-[var(--foreground)] truncate">Dual HUD Pod</div>
      </button>

      <button data-sub="suspension" class="subsystemBtn p-1.5 rounded-lg bg-[var(--background)] border border-[var(--border)] text-left hover:border-orange-500/50 transition active:scale-95">
        <div class="text-[9px] text-[var(--muted-foreground)] uppercase font-semibold">Mobility</div>
        <div class="text-[11px] font-bold text-[var(--foreground)] truncate">6-Wheel Bogie</div>
      </button>

      <button data-sub="antigrav" class="subsystemBtn p-1.5 rounded-lg bg-cyan-950/20 border border-cyan-500/30 text-left hover:border-cyan-400 transition active:scale-95">
        <div class="text-[9px] text-cyan-400 uppercase font-semibold">Anti-Gravity</div>
        <div class="text-[11px] font-bold text-cyan-200 truncate">4x Vector Emitters</div>
      </button>

      <button data-sub="science" class="subsystemBtn p-1.5 rounded-lg bg-[var(--background)] border border-[var(--border)] text-left hover:border-orange-500/50 transition active:scale-95">
        <div class="text-[9px] text-[var(--muted-foreground)] uppercase font-semibold">Payload</div>
        <div class="text-[11px] font-bold text-[var(--foreground)] truncate">ChemCam & Arm</div>
      </button>
    </div>

    <!-- Inspector Details Drawer -->
    <div id="inspectorPanel" class="mt-2 p-2 rounded-xl bg-[var(--background)] border border-[var(--border)] text-xs flex items-center justify-between">
      <div class="flex items-center gap-2.5">
        <div id="inspectIcon" class="w-6 h-6 rounded bg-orange-500/20 border border-orange-500/40 flex items-center justify-center text-orange-400 font-bold text-xs">
          &#9881;
        </div>
        <div>
          <span id="inspectTitle" class="font-bold text-[var(--foreground)]">ARES-VI Master Platform</span>
          <span class="text-[var(--muted-foreground)] mx-1">&bull;</span>
          <span id="inspectDesc" class="text-[var(--muted-foreground)]">Ready for deployment in AstroGateway Mars Exploration Simulation.</span>
        </div>
      </div>
      <div class="text-[10px] font-mono text-cyan-400 bg-cyan-950/40 border border-cyan-800/40 px-2 py-0.5 rounded">
        STATUS: NOMINAL
      </div>
    </div>
  </div>

  <script>
    const studioData = %STUDIO_DATA%;

    const canvas = document.getElementById('renderCanvas');
    const ctx = canvas.getContext('2d');

    // Camera State
    let yaw = 0.85;
    let pitch = 0.35;
    let zoom = 1.0;
    let panX = 0;
    let panY = 0;

    let targetYaw = yaw;
    let targetPitch = pitch;
    let targetZoom = zoom;

    let autoRotate = true;
    let wireframe = false;
    let isHoverMode = false;
    let hoverBlend = 0.0; // 0.0 = Ground, 1.0 = Hover
    let explodeBlend = 0.0; // 0.0 = Normal, 1.0 = Exploded

    let isDragging = false;
    let isPanning = false;
    let lastX = 0;
    let lastY = 0;

    // Subsystem descriptions
    const subsystemInfo = {
      hull: { title: "Titanium Chassis & Ceramic Plating", desc: "Aerospace composite hull with thermal protection and Martian orange safety trim." },
      cockpit: { title: "Crew Module & Dual Panoramic HUD", desc: "Astronaut-scale pressurized pod with pilot/science stations and digital displays." },
      suspension: { title: "Rocker-Bogie 6-Wheel Mobility", desc: "Independent in-hub brushless motors and articulated terrain articulation." },
      antigrav: { title: "Anti-Gravity Emitter Modules", desc: "4 underbody electromagnetic vectoring thrusters + 2 lateral auxiliary stabilizers." },
      science: { title: "ChemCam Mast & Science Arm", desc: "Laser spectrometer, stereo NavCams, 5-axis articulated boom, and core drill." }
    };

    function resizeCanvas() {
      const rect = canvas.getBoundingClientRect();
      const dpr = window.devicePixelRatio || 1;
      canvas.width = rect.width * dpr;
      canvas.height = rect.height * dpr;
    }
    resizeCanvas();
    window.addEventListener('resize', resizeCanvas);

    // Mouse & Touch Controls
    canvas.addEventListener('mousedown', (e) => {
      if (e.button === 2 || e.shiftKey) {
        isPanning = true;
      } else {
        isDragging = true;
        autoRotate = false;
        document.getElementById('rotateStatus').innerText = 'OFF';
        document.getElementById('rotateStatus').className = 'text-slate-400 font-bold';
      }
      lastX = e.clientX;
      lastY = e.clientY;
    });

    window.addEventListener('mouseup', () => {
      isDragging = false;
      isPanning = false;
    });

    canvas.addEventListener('contextmenu', e => e.preventDefault());

    window.addEventListener('mousemove', (e) => {
      const dx = e.clientX - lastX;
      const dy = e.clientY - lastY;
      lastX = e.clientX;
      lastY = e.clientY;

      if (isDragging) {
        targetYaw += dx * 0.010;
        targetPitch = Math.max(-1.45, Math.min(1.45, targetPitch + dy * 0.010));
      } else if (isPanning) {
        panX += dx * 0.8;
        panY += dy * 0.8;
      }
    });

    canvas.addEventListener('wheel', (e) => {
      e.preventDefault();
      targetZoom = Math.max(0.45, Math.min(2.8, targetZoom - e.deltaY * 0.0015));
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
      targetPitch = Math.max(-1.45, Math.min(1.45, targetPitch + dy * 0.012));
    });

    // UI Buttons: Mode Switching
    const btnGround = document.getElementById('btnModeGround');
    const btnHover = document.getElementById('btnModeHover');
    const badgeMode = document.getElementById('badgeMode');
    const hoverStatusHud = document.getElementById('hoverStatusHud');

    btnGround.addEventListener('click', () => {
      isHoverMode = false;
      btnGround.className = "px-2.5 py-1 text-xs font-semibold rounded-lg bg-orange-600 text-white transition shadow-sm";
      btnHover.className = "px-2.5 py-1 text-xs font-semibold rounded-lg text-[var(--muted-foreground)] hover:text-[var(--foreground)] transition";
      badgeMode.className = "px-1.5 py-0.5 rounded text-[10px] font-semibold tracking-wider bg-orange-500/20 text-orange-400 border border-orange-500/40";
      badgeMode.innerText = "MODE A: GROUND";
      hoverStatusHud.classList.add('hidden');
    });

    btnHover.addEventListener('click', () => {
      isHoverMode = true;
      btnHover.className = "px-2.5 py-1 text-xs font-semibold rounded-lg bg-cyan-600 text-white transition shadow-sm";
      btnGround.className = "px-2.5 py-1 text-xs font-semibold rounded-lg text-[var(--muted-foreground)] hover:text-[var(--foreground)] transition";
      badgeMode.className = "px-1.5 py-0.5 rounded text-[10px] font-semibold tracking-wider bg-cyan-500/20 text-cyan-300 border border-cyan-500/40";
      badgeMode.innerText = "MODE B: ANTI-GRAV HOVER";
      hoverStatusHud.classList.remove('hidden');
    });

    // Camera Presets
    document.querySelectorAll('.btnCam').forEach(btn => {
      btn.addEventListener('click', () => {
        const c = btn.getAttribute('data-cam');
        autoRotate = false;
        document.getElementById('rotateStatus').innerText = 'OFF';
        document.getElementById('rotateStatus').className = 'text-slate-400 font-bold';
        panX = 0; panY = 0;
        if (c === 'hero') { targetYaw = 0.85; targetPitch = 0.35; targetZoom = 1.0; }
        else if (c === 'front') { targetYaw = 0.0; targetPitch = 0.08; targetZoom = 1.1; }
        else if (c === 'side') { targetYaw = Math.PI / 2; targetPitch = 0.05; targetZoom = 1.05; }
        else if (c === 'top') { targetYaw = 0.0; targetPitch = 1.40; targetZoom = 1.15; }
        else if (c === 'under') { targetYaw = 0.0; targetPitch = -1.25; targetZoom = 1.1; }
      });
    });

    // Auto Rotate, Wireframe, Explode, Reset
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

    document.getElementById('btnExplode').addEventListener('click', () => {
      explodeBlend = explodeBlend > 0.5 ? 0.0 : 1.0;
      document.getElementById('explodeStatus').innerText = explodeBlend > 0.5 ? '100%' : '0%';
      document.getElementById('explodeStatus').className = explodeBlend > 0.5 ? 'text-orange-400 font-bold' : 'text-slate-400 font-bold';
    });

    document.getElementById('btnResetView').addEventListener('click', () => {
      targetYaw = 0.85;
      targetPitch = 0.35;
      targetZoom = 1.0;
      panX = 0;
      panY = 0;
    });

    // Subsystem inspector buttons
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

    // Material Color definitions
    const materials = {
      Mat_CeramicWhite: { r: 232, g: 238, b: 245, stroke: '#9aa8ba', spec: 0.6 },
      Mat_GraphiteTrim: { r: 42, g: 48, b: 58, stroke: '#1d222a', spec: 0.4 },
      Mat_TitaniumFrame: { r: 102, g: 112, b: 126, stroke: '#4f5966', spec: 0.85 },
      Mat_MartianOrange: { r: 248, g: 90, b: 20, stroke: '#d64708', spec: 0.7 },
      Mat_GoldFoil: { r: 238, g: 192, b: 40, stroke: '#fbe282', spec: 0.95 },
      Mat_CarbonTreads: { r: 24, g: 27, b: 32, stroke: '#4a5360', spec: 0.2 },
      Mat_CanopyGlass: { r: 35, g: 95, b: 140, stroke: '#8ce6ff', spec: 0.95, alpha: 0.65 },
      Mat_CyanEnergy: { r: 0, g: 235, b: 255, stroke: '#b3f5ff', glow: true, spec: 1.0 }
    };

    let animTime = 0;
    let curExplode = 0;

    function render() {
      animTime += 0.025;

      // Smooth camera interpolation
      yaw += (targetYaw - yaw) * 0.12;
      pitch += (targetPitch - pitch) * 0.12;
      zoom += (targetZoom - zoom) * 0.12;

      if (autoRotate && !isDragging) {
        targetYaw += 0.007;
        yaw = targetYaw;
      }

      // Smooth hover & explode blending
      hoverBlend += ((isHoverMode ? 1.0 : 0.0) - hoverBlend) * 0.08;
      curExplode += (explodeBlend - curExplode) * 0.08;

      const w = canvas.width;
      const h = canvas.height;
      ctx.clearRect(0, 0, w, h);

      // Studio Background: Deep charcoal gradient with warm Mars horizon glow
      const bgGrad = ctx.createRadialGradient(w/2, h/2, 20, w/2, h/2, Math.max(w,h)*0.65);
      bgGrad.addColorStop(0, '#10141d');
      bgGrad.addColorStop(0.65, '#07090e');
      bgGrad.addColorStop(1, '#020305');
      ctx.fillStyle = bgGrad;
      ctx.fillRect(0, 0, w, h);

      // Camera Center & Vehicle Offset
      // Height bobbing in hover mode
      const hoverBob = hoverBlend * 0.04 * Math.sin(animTime * 2.2);
      const vehicleZOffset = hoverBlend * 0.28 + hoverBob;

      const cx = (w / 2) + panX;
      const cy = (h / 2) + panY + (45 * zoom);

      // Ground Shadow / Anti-Grav Energy Disk
      ctx.save();
      ctx.beginPath();
      const shadowRadius = (165 + hoverBlend * 20) * zoom;
      ctx.ellipse(cx, cy + (vehicleZOffset * 40 * zoom), shadowRadius, (48 + hoverBlend * 12) * zoom, 0, 0, Math.PI * 2);
      
      if (hoverBlend > 0.05) {
        // Cyan anti-gravity ground emission ring
        const glowGrad = ctx.createRadialGradient(cx, cy, 10, cx, cy, shadowRadius);
        glowGrad.addColorStop(0, `rgba(0, 235, 255, ${0.45 * hoverBlend})`);
        glowGrad.addColorStop(0.5, `rgba(0, 180, 255, ${0.25 * hoverBlend})`);
        glowGrad.addColorStop(1, 'rgba(0, 0, 0, 0)');
        ctx.fillStyle = glowGrad;
        ctx.fill();
      } else {
        ctx.fillStyle = 'rgba(0, 0, 0, 0.55)';
        ctx.filter = 'blur(10px)';
        ctx.fill();
      }
      ctx.restore();

      const cosY = Math.cos(yaw), sinY = Math.sin(yaw);
      const cosP = Math.cos(pitch), sinP = Math.sin(pitch);

      // Calibrated scale to fill ~82% of viewport nicely
      const baseScale = (Math.min(w, h) * 0.96) * zoom;
      const dist = 4.4;
      const centerZ = 0.95;

      // Transform all vertices
      const transformedVerts = new Array(studioData.vertices.length);
      const projectedVerts = new Array(studioData.vertices.length);

      // Pre-calculate face render list
      const drawList = [];

      for (const [subId, sub] of Object.entries(studioData.subsystems)) {
        const mat = materials[sub.material] || materials.Mat_TitaniumFrame;
        const ex = sub.explode || [0,0,0];
        const hd = sub.hover_delta || [0,0,0];

        // Total offset for this subsystem
        const offX = ex[0] * curExplode * 0.8 + hd[0] * hoverBlend;
        const offY = ex[1] * curExplode * 0.8 + hd[1] * hoverBlend;
        const offZ = ex[2] * curExplode * 0.8 + hd[2] * hoverBlend + vehicleZOffset;

        for (let fi = 0; fi < sub.faces.length; fi++) {
          const face = sub.faces[fi];
          if (face.length < 3) continue;

          // Compute transformed coordinates for face vertices
          const pTrans = [];
          const pProj = [];
          let avgY = 0;

          for (let vi = 0; vi < face.length; vi++) {
            const vIdx = face[vi];
            const v = studioData.vertices[vIdx];

            const x0 = v[0] + offX;
            const y0 = v[1] + offY;
            const z0 = (v[2] - centerZ) + offZ;

            // Yaw around Z
            const x1 = x0 * cosY - y0 * sinY;
            const y1 = x0 * sinY + y0 * cosY;
            const z1 = z0;

            // Pitch around X
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

          // Normal & Backface Culling
          const p0 = pTrans[0], p1 = pTrans[1], p2 = pTrans[2];
          const ax = p1[0] - p0[0], ay = p1[1] - p0[1], az = p1[2] - p0[2];
          const bx = p2[0] - p0[0], by = p2[1] - p0[1], bz = p2[2] - p0[2];
          const nx = ay * bz - az * by;
          const ny = az * bx - ax * bz;
          const nz = ax * by - ay * bx;

          // Camera vector
          const dotView = nx * p0[0] + ny * (p0[1] + dist) + nz * p0[2];

          // Allow transparency / glow on both sides
          if (!mat.glow && !mat.alpha && dotView <= 0) continue;

          const nLen = Math.sqrt(nx*nx + ny*ny + nz*nz) || 1;
          const normX = nx / nLen, normY = ny / nLen, normZ = nz / nLen;

          // Key light in camera space
          const dotLight = Math.max(0, normX * 0.50 + normY * -0.75 + normZ * 0.65);
          
          // Specular reflection
          const specPower = Math.pow(Math.max(0, normX * 0.35 + normY * -0.65 + normZ * 0.70), 8) * (mat.spec || 0.4);

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

      // Sort faces back-to-front (Painter's Algorithm)
      drawList.sort((a, b) => b.avgY - a.avgY);

      // Render Faces
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
          ctx.lineWidth = mat.glow ? 1.8 : 0.8;
          ctx.stroke();
        } else {
          if (mat.glow) {
            // Animated pulsing cyan energy
            const pulse = 0.85 + 0.15 * Math.sin(animTime * 5.0);
            ctx.fillStyle = `rgba(0, 235, 255, ${0.92 * pulse})`;
            ctx.shadowColor = '#00f0ff';
            ctx.shadowBlur = 12 * pulse;
            ctx.fill();
            ctx.shadowBlur = 0;
            ctx.strokeStyle = '#cffafe';
            ctx.lineWidth = 1.0;
            ctx.stroke();
          } else {
            // PBR lighting with specular highlight
            const ambient = 0.28;
            const diffuse = 0.72 * item.dotLight;
            const intensity = ambient + diffuse;
            const r = Math.min(255, Math.floor(mat.r * intensity + item.specPower * 90));
            const g = Math.min(255, Math.floor(mat.g * intensity + item.specPower * 90));
            const b = Math.min(255, Math.floor(mat.b * intensity + item.specPower * 90));

            if (mat.alpha) {
              ctx.fillStyle = `rgba(${r},${g},${b},${mat.alpha})`;
            } else {
              ctx.fillStyle = `rgb(${r},${g},${b})`;
            }
            ctx.fill();

            // Edge highlight stroke
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

output = template.replace('%STUDIO_DATA%', studio_json)

# Save to Brain Artifact directory
artifact_path = '/Users/kapilash/.gemini/antigravity/brain/fe04a22b-3e01-409f-a35c-ed439568d853/ares_vi_studio.html'
with open(artifact_path, 'w') as f:
    f.write(output)

print(f'ARES-VI Interactive Studio generated successfully at: {artifact_path}')
