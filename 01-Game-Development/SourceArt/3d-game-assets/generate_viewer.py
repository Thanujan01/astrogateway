import json

with open('/Users/kapilash/.gemini/antigravity/scratch/3d-game-assets/mesh_data.json') as f:
    mesh_data = json.load(f)

mesh_json_str = json.dumps(mesh_data)

html_content = f"""<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <script src="https://www.gstatic.com/antigravity/web/dev/tailwindcss.min.js"></script>
  <style>
    canvas {{
      touch-action: none;
      user-select: none;
      -webkit-user-select: none;
    }}
  </style>
</head>
<body class="bg-transparent text-[var(--foreground)] antialiased p-2">
  <div class="bg-[var(--card)] text-[var(--foreground)] border border-[var(--border)] rounded-2xl p-4 shadow-md max-w-xl mx-auto">
    <!-- Header -->
    <div class="flex items-center justify-between pb-3 border-b border-[var(--border)]">
      <div>
        <div class="flex items-center gap-2">
          <span class="inline-block w-2.5 h-2.5 rounded-full bg-cyan-400 animate-pulse"></span>
          <h2 class="text-sm font-bold tracking-wide uppercase text-[var(--foreground)]">Stylized Loot Chest</h2>
        </div>
        <p class="text-xs text-[var(--muted-foreground)]">Game Ready Asset &bull; 160 Verts &bull; PBR Materials</p>
      </div>
      <div class="flex items-center gap-1.5 text-xs">
        <span class="px-2 py-0.5 rounded-md bg-[var(--background)] border border-[var(--border)] text-[var(--muted-foreground)]">3D Interactive</span>
      </div>
    </div>

    <!-- 3D Canvas Area -->
    <div class="relative mt-3 rounded-xl overflow-hidden bg-slate-950/80 border border-[var(--border)] flex items-center justify-center">
      <canvas id="renderCanvas" width="520" height="260" class="w-full h-[260px] cursor-grab active:cursor-grabbing block"></canvas>
      
      <!-- Controls Overlay -->
      <div class="absolute bottom-2 left-2 flex gap-1.5 z-10">
        <button id="btnAutoRotate" class="px-2.5 py-1 text-xs font-medium rounded-lg bg-black/60 backdrop-blur-md text-white border border-white/20 hover:bg-black/80 transition">
          Auto-Rotate: <span id="rotateState" class="text-cyan-400">ON</span>
        </button>
        <button id="btnWireframe" class="px-2.5 py-1 text-xs font-medium rounded-lg bg-black/60 backdrop-blur-md text-white border border-white/20 hover:bg-black/80 transition">
          Wireframe: <span id="wireState" class="text-slate-400">OFF</span>
        </button>
        <button id="btnReset" class="px-2 py-1 text-xs font-medium rounded-lg bg-black/60 backdrop-blur-md text-white border border-white/20 hover:bg-black/80 transition" title="Reset View">
          Reset
        </button>
      </div>

      <div class="absolute top-2 right-2 px-2 py-1 rounded bg-black/60 backdrop-blur-md text-[11px] text-cyan-300 border border-cyan-500/30">
        Drag to Rotate &bull; Scroll to Zoom
      </div>
    </div>

    <!-- Quick Palette / Specs -->
    <div class="mt-3 grid grid-cols-3 gap-2 text-xs">
      <div class="p-2 rounded-lg bg-[var(--background)] border border-[var(--border)] flex items-center gap-2">
        <div class="w-3.5 h-3.5 rounded bg-[#1e2430] border border-white/20 shadow-inner"></div>
        <div>
          <div class="font-medium text-[var(--foreground)]">Base Plating</div>
          <div class="text-[10px] text-[var(--muted-foreground)]">Dark Steel/Wood</div>
        </div>
      </div>
      <div class="p-2 rounded-lg bg-[var(--background)] border border-[var(--border)] flex items-center gap-2">
        <div class="w-3.5 h-3.5 rounded bg-[#e5b026] border border-amber-300/40 shadow-inner"></div>
        <div>
          <div class="font-medium text-[var(--foreground)]">Trim & Braces</div>
          <div class="text-[10px] text-[var(--muted-foreground)]">Polished Gold</div>
        </div>
      </div>
      <div class="p-2 rounded-lg bg-[var(--background)] border border-[var(--border)] flex items-center gap-2">
        <div class="w-3.5 h-3.5 rounded bg-[#00d2ff] shadow-[0_0_8px_rgba(0,210,255,0.7)]"></div>
        <div>
          <div class="font-medium text-[var(--foreground)]">Power Core</div>
          <div class="text-[10px] text-[var(--muted-foreground)]">Emissive Cyan</div>
        </div>
      </div>
    </div>
  </div>

  <script>
    const meshData = {mesh_json_str};

    const canvas = document.getElementById('renderCanvas');
    const ctx = canvas.getContext('2d');

    // Camera & interaction state
    let yaw = 0.8;
    let pitch = 0.35;
    let zoom = 1.0;
    let autoRotate = true;
    let wireframe = false;

    let isDragging = false;
    let lastMouseX = 0;
    let lastMouseY = 0;

    // Resize canvas for devicePixelRatio
    function resizeCanvas() {{
      const rect = canvas.getBoundingClientRect();
      const dpr = window.devicePixelRatio || 1;
      canvas.width = rect.width * dpr;
      canvas.height = rect.height * dpr;
    }}
    resizeCanvas();
    window.addEventListener('resize', resizeCanvas);

    // Mouse & Touch events
    canvas.addEventListener('mousedown', (e) => {{
      isDragging = true;
      lastMouseX = e.clientX;
      lastMouseY = e.clientY;
      autoRotate = false;
      document.getElementById('rotateState').innerText = 'OFF';
      document.getElementById('rotateState').className = 'text-slate-400';
    }});

    window.addEventListener('mouseup', () => isDragging = false);

    window.addEventListener('mousemove', (e) => {{
      if (!isDragging) return;
      const dx = e.clientX - lastMouseX;
      const dy = e.clientY - lastMouseY;
      lastMouseX = e.clientX;
      lastMouseY = e.clientY;
      yaw += dx * 0.012;
      pitch = Math.max(-1.1, Math.min(1.1, pitch + dy * 0.012));
    }});

    // Wheel zoom
    canvas.addEventListener('wheel', (e) => {{
      e.preventDefault();
      zoom = Math.max(0.6, Math.min(2.4, zoom - e.deltaY * 0.0015));
    }}, {{ passive: false }});

    // Touch support
    canvas.addEventListener('touchstart', (e) => {{
      if (e.touches.length === 1) {{
        isDragging = true;
        lastMouseX = e.touches[0].clientX;
        lastMouseY = e.touches[0].clientY;
        autoRotate = false;
        document.getElementById('rotateState').innerText = 'OFF';
        document.getElementById('rotateState').className = 'text-slate-400';
      }}
    }});
    window.addEventListener('touchend', () => isDragging = false);
    window.addEventListener('touchmove', (e) => {{
      if (!isDragging || e.touches.length !== 1) return;
      const dx = e.touches[0].clientX - lastMouseX;
      const dy = e.touches[0].clientY - lastMouseY;
      lastMouseX = e.touches[0].clientX;
      lastMouseY = e.touches[0].clientY;
      yaw += dx * 0.015;
      pitch = Math.max(-1.1, Math.min(1.1, pitch + dy * 0.015));
    }});

    // Button controls
    document.getElementById('btnAutoRotate').addEventListener('click', () => {{
      autoRotate = !autoRotate;
      document.getElementById('rotateState').innerText = autoRotate ? 'ON' : 'OFF';
      document.getElementById('rotateState').className = autoRotate ? 'text-cyan-400' : 'text-slate-400';
    }});

    document.getElementById('btnWireframe').addEventListener('click', () => {{
      wireframe = !wireframe;
      document.getElementById('wireState').innerText = wireframe ? 'ON' : 'OFF';
      document.getElementById('wireState').className = wireframe ? 'text-cyan-400' : 'text-slate-400';
    }});

    document.getElementById('btnReset').addEventListener('click', () => {{
      yaw = 0.8;
      pitch = 0.35;
      zoom = 1.0;
    }});

    // Colors
    const colors = {{
      BodyMaterial: {{ r: 35, g: 42, b: 54, stroke: '#2e3848' }},
      TrimMaterial: {{ r: 232, g: 178, b: 42, stroke: '#f7d368' }},
      CrystalMaterial: {{ r: 0, g: 215, b: 255, stroke: '#70e7ff', glow: true }}
    }};

    // Directional light vector in world space
    const light = [0.55, -0.75, 0.9];
    const lightLen = Math.sqrt(light[0]*light[0] + light[1]*light[1] + light[2]*light[2]);
    const lx = light[0] / lightLen;
    const ly = light[1] / lightLen;
    const lz = light[2] / lightLen;

    // Center offset of chest model
    const chestCenterZ = 0.65;

    function render() {{
      if (autoRotate) {{
        yaw += 0.01;
      }}

      const w = canvas.width;
      const h = canvas.height;
      ctx.clearRect(0, 0, w, h);

      // Subtle background gradient
      const bgGrad = ctx.createRadialGradient(w/2, h/2, 20, w/2, h/2, Math.max(w,h)*0.6);
      bgGrad.addColorStop(0, '#101522');
      bgGrad.addColorStop(1, '#06080e');
      ctx.fillStyle = bgGrad;
      ctx.fillRect(0, 0, w, h);

      // Ground shadow ellipse
      const cx = w / 2;
      const cy = h / 2 + 50 * zoom;
      ctx.save();
      ctx.beginPath();
      ctx.ellipse(cx, cy, 140 * zoom, 35 * zoom, 0, 0, Math.PI * 2);
      ctx.fillStyle = 'rgba(0, 0, 0, 0.45)';
      ctx.filter = 'blur(8px)';
      ctx.fill();
      ctx.restore();

      const cosY = Math.cos(yaw), sinY = Math.sin(yaw);
      const cosP = Math.cos(pitch), sinP = Math.sin(pitch);

      // Transform vertices
      const transformed = [];
      const projected = [];
      const dist = 3.6;
      const baseScale = (Math.min(w, h) * 0.72) * zoom;

      for (let i = 0; i < meshData.vertices.length; i++) {{
        const v = meshData.vertices[i];
        const x0 = v[0];
        const y0 = v[1];
        const z0 = v[2] - chestCenterZ;

        // Yaw around Z
        const x1 = x0 * cosY - y0 * sinY;
        const y1 = x0 * sinY + y0 * cosY;
        const z1 = z0;

        // Pitch around X
        const x2 = x1;
        const y2 = y1 * cosP - z1 * sinP;
        const z2 = y1 * sinP + z1 * cosP;

        transformed.push([x2, y2, z2]);

        // Perspective
        const depth = y2 + dist;
        const invDepth = depth > 0.1 ? 1 / depth : 1;
        const px = cx + (x2 * invDepth) * baseScale;
        const py = (h / 2) - (z2 * invDepth) * baseScale;
        projected.push([px, py, y2]);
      }}

      // Collect and sort faces
      const drawList = [];

      for (const [matName, faces] of Object.entries(meshData.materials)) {{
        const mat = colors[matName] || colors.BodyMaterial;

        for (let fi = 0; fi < faces.length; fi++) {{
          const face = faces[fi];
          if (face.length < 3) continue;

          // Compute transformed normal and depth
          const p0 = transformed[face[0]];
          const p1 = transformed[face[1]];
          const p2 = transformed[face[2]];

          // Cross product in camera space
          const ax = p1[0] - p0[0], ay = p1[1] - p0[1], az = p1[2] - p0[2];
          const bx = p2[0] - p0[0], by = p2[1] - p0[1], bz = p2[2] - p0[2];
          const nx = ay * bz - az * by;
          const ny = az * bx - ax * bz;
          const nz = ax * by - ay * bx;

          // Backface culling: vector from camera to point (camera at (0, -dist, 0))
          const viewX = p0[0];
          const viewY = p0[1] + dist;
          const viewZ = p0[2];
          const dotView = nx * viewX + ny * viewY + nz * viewZ;

          // Don't cull crystal for jewel transparency/glow
          if (!mat.glow && dotView <= 0) continue;

          const nLen = Math.sqrt(nx*nx + ny*ny + nz*nz) || 1;
          const normX = nx / nLen, normY = ny / nLen, normZ = nz / nLen;

          // World normal for lighting (rotate norm back or light into view space)
          // Simplified camera-space key light:
          const lightCamX = 0.45, lightCamY = -0.75, lightCamZ = 0.65;
          const dotLight = Math.max(0, normX * lightCamX + normY * lightCamY + normZ * lightCamZ);

          // Average depth
          let avgY = 0;
          for (let vi = 0; vi < face.length; vi++) {{
            avgY += transformed[face[vi]][1];
          }}
          avgY /= face.length;

          drawList.push({{
            face,
            mat,
            avgY,
            dotLight
          }});
        }}
      }}

      // Sort faces back-to-front (Painter's algorithm)
      drawList.sort((a, b) => b.avgY - a.avgY);

      // Draw faces
      for (let i = 0; i < drawList.length; i++) {{
        const item = drawList[i];
        const face = item.face;
        const mat = item.mat;

        ctx.beginPath();
        const pStart = projected[face[0]];
        ctx.moveTo(pStart[0], pStart[1]);
        for (let j = 1; j < face.length; j++) {{
          const p = projected[face[j]];
          ctx.lineTo(p[0], p[1]);
        }}
        ctx.closePath();

        if (wireframe) {{
          ctx.strokeStyle = mat.glow ? '#00e5ff' : '#64748b';
          ctx.lineWidth = mat.glow ? 2 : 1;
          ctx.stroke();
        }} else {{
          if (mat.glow) {{
            // Crystal glowing shader
            ctx.fillStyle = 'rgba(0, 210, 255, 0.88)';
            ctx.shadowColor = '#00d2ff';
            ctx.shadowBlur = 14;
            ctx.fill();
            ctx.shadowBlur = 0;
            ctx.strokeStyle = '#a6f2ff';
            ctx.lineWidth = 1.2;
            ctx.stroke();
          }} else {{
            // Lambertian + Specular Shading
            const ambient = 0.28;
            const intensity = ambient + 0.72 * item.dotLight;
            const r = Math.min(255, Math.floor(mat.r * intensity));
            const g = Math.min(255, Math.floor(mat.g * intensity));
            const b = Math.min(255, Math.floor(mat.b * intensity));

            ctx.fillStyle = `rgb(${r},${g},${b})`;
            ctx.fill();

            // Edge highlight
            ctx.strokeStyle = mat.stroke;
            ctx.lineWidth = 0.8;
            ctx.stroke();
          }}
        }}
      }}

      requestAnimationFrame(render);
    }}

    requestAnimationFrame(render);
  </script>
</body>
</html>
"""

with open('/Users/kapilash/.gemini/antigravity/brain/fe04a22b-3e01-409f-a35c-ed439568d853/viewer.html', 'w') as f:
    f.write(html_content)

print('Viewer HTML written successfully!')
