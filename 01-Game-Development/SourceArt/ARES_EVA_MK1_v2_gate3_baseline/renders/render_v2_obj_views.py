import os
import math
import struct
import zlib
import time

obj_path = "/Users/kapilash/.gemini/antigravity/scratch/ARES_EVA_MK1_v2/models/ARES_EVA_MK1_v2_LOD0.obj"
out_dir = "/Users/kapilash/.gemini/antigravity/brain/fe04a22b-3e01-409f-a35c-ed439568d853/renders_v2"
local_out = "/Users/kapilash/.gemini/antigravity/scratch/ARES_EVA_MK1_v2/renders"
os.makedirs(out_dir, exist_ok=True)
os.makedirs(local_out, exist_ok=True)

print("Parsing OBJ:", obj_path)
vertices = []
faces = [] # list of (v_indices, group_name)
current_group = "default"

with open(obj_path, "r") as f:
    for line in f:
        if line.startswith("v "):
            parts = line.strip().split()
            vertices.append((float(parts[1]), float(parts[2]), float(parts[3])))
        elif line.startswith("o ") or line.startswith("g ") or line.startswith("usemtl "):
            current_group = line.strip().split()[-1]
        elif line.startswith("f "):
            parts = line.strip().split()[1:]
            v_idx = [int(p.split("/")[0]) - 1 for p in parts]
            if len(v_idx) == 3:
                faces.append((v_idx, current_group))
            elif len(v_idx) == 4:
                faces.append(([v_idx[0], v_idx[1], v_idx[2]], current_group))
                faces.append(([v_idx[0], v_idx[2], v_idx[3]], current_group))

print(f"Loaded {len(vertices):,} vertices and {len(faces):,} triangles.")

# NASA PBR Palette
palette = {
    'white':    (229, 230, 226, 0.45),
    'graphite': (37, 43, 50, 0.35),
    'titanium': (137, 147, 155, 0.85),
    'orange':   (217, 106, 41, 0.65),
    'gold':     (242, 196, 38, 0.98),
    'cyan':     (0, 201, 232, 1.0)
}

def get_color(grp, avg_z, avg_x, avg_y):
    g = grp.lower()
    if 'gold' in g or 'visor' in g:
        if 'clear' in g: return palette['white']
        return palette['gold']
    if 'cyan' in g or 'lens' in g or 'screen' in g or 'led' in g:
        return palette['cyan']
    if 'orange' in g or 'strake' in g:
        return palette['orange']
    if 'titanium' in g or 'ring' in g or 'plate' in g or 'bearing' in g or 'cap' in g or 'tank' in g or 'cleat' in g or 'shield' in g:
        return palette['titanium']
    if 'graphite' in g or 'bellow' in g or 'boot' in g or 'sole' in g or 'brief' in g:
        return palette['graphite']
    # Fallback spatial coloring
    if avg_z > 1.55: return palette['white']
    if abs(avg_x) > 0.35 and avg_z < 0.92: return palette['graphite'] # gloves
    if avg_z < 0.12: return palette['graphite'] # boots
    return palette['white']

def write_png(filename, width, height, rgb_buffer):
    raw_scanlines = bytearray()
    row_bytes = width * 3
    for y in range(height):
        raw_scanlines.append(0)
        start = y * row_bytes
        raw_scanlines.extend(rgb_buffer[start:start + row_bytes])

    compressed = zlib.compress(bytes(raw_scanlines), level=6)

    def chunk(chunk_type, data):
        c = chunk_type + data
        crc = zlib.crc32(c) & 0xffffffff
        return struct.pack(">I", len(data)) + c + struct.pack(">I", crc)

    png_bytes = b"\x89PNG\r\n\x1a\n"
    png_bytes += chunk(b"IHDR", struct.pack(">IIBBBBB", width, height, 8, 2, 0, 0, 0))
    png_bytes += chunk(b"IDAT", compressed)
    png_bytes += chunk(b"IEND", b"")

    with open(filename, "wb") as fp:
        fp.write(png_bytes)

def render_view(filename, yaw_deg, pitch_deg, target=(0, 0, 0.98), dist=3.2, fov=45, w=800, h=600, wireframe=False):
    t0 = time.time()
    yaw = math.radians(yaw_deg)
    pitch = math.radians(pitch_deg)
    cos_y, sin_y = math.cos(yaw), math.sin(yaw)
    cos_p, sin_p = math.cos(pitch), math.sin(pitch)

    # Gradient background
    buffer = bytearray(w * h * 3)
    for py in range(h):
        r_bg = int(14 + (py / h) * 12)
        g_bg = int(18 + (py / h) * 16)
        b_bg = int(26 + (py / h) * 22)
        row_offset = py * w * 3
        for px in range(w):
            idx = row_offset + px * 3
            buffer[idx] = r_bg
            buffer[idx+1] = g_bg
            buffer[idx+2] = b_bg

    z_buffer = [float('inf')] * (w * h)

    # Lighting direction
    lx, ly, lz = 0.45, -0.65, 0.60
    l_len = math.sqrt(lx*lx + ly*ly + lz*lz)
    lx, ly, lz = lx/l_len, ly/l_len, lz/l_len

    tx, ty, tz = target
    aspect = w / h
    tan_half_fov = math.tan(math.radians(fov) / 2)

    # Transform triangles
    transformed_faces = []
    for (v_idx, grp) in faces:
        p0 = vertices[v_idx[0]]
        p1 = vertices[v_idx[1]]
        p2 = vertices[v_idx[2]]

        cam_pts = []
        for (vx, vy, vz) in (p0, p1, p2):
            dx = vx - tx
            dy = vy - ty
            dz = vz - tz

            # Rotate Yaw (Y up in Blender OBJ coordinates: Z is up, Y is depth)
            x1 = dx * cos_y - dy * sin_y
            y1 = dx * sin_y + dy * cos_y
            z1 = dz

            # Rotate Pitch
            x2 = x1
            y2 = y1 * cos_p - z1 * sin_p
            z2 = y1 * sin_p + z1 * cos_p

            depth = y2 + dist
            if depth <= 0.1:
                cam_pts = []
                break

            # Project
            inv_d = 1.0 / (depth * tan_half_fov)
            sx = int((x2 * inv_d / aspect + 1.0) * 0.5 * w)
            sy = int((1.0 - (z2 * inv_d + 1.0) * 0.5) * h)
            cam_pts.append((sx, sy, depth, x2, y2, z2))

        if not cam_pts:
            continue

        # Normal calculation
        (s0x, s0y, d0, x0, y0, z0), (s1x, s1y, d1, x1, y1, z1), (s2x, s2y, d2, x2, y2, z2) = cam_pts
        ax, ay, az = x1 - x0, y1 - y0, z1 - z0
        bx, by, bz = x2 - x0, y2 - y0, z2 - z0
        nx = ay * bz - az * by
        ny = az * bx - ax * bz
        nz = ax * by - ay * bx
        n_len = math.sqrt(nx*nx + ny*ny + nz*nz)
        if n_len < 1e-6: continue
        nx, ny, nz = nx/n_len, ny/n_len, nz/n_len

        # Backface culling
        if ny > 0.05 and not wireframe: continue

        dot_l = max(0.0, nx * lx + ny * ly + nz * lz)
        avg_z = (p0[2] + p1[2] + p2[2]) / 3.0
        avg_x = (p0[0] + p1[0] + p2[0]) / 3.0
        avg_y = (p0[1] + p1[1] + p2[1]) / 3.0

        r_base, g_base, b_base, spec_w = get_color(grp, avg_z, avg_x, avg_y)

        # Specular highlight
        spec_dot = max(0.0, nx * 0.35 + ny * -0.65 + nz * 0.70)
        spec = math.pow(spec_dot, 8) * spec_w * 70

        ambient = 0.35
        diffuse = 0.65 * dot_l
        intensity = ambient + diffuse

        r = min(255, int(r_base * intensity + spec))
        g = min(255, int(g_base * intensity + spec))
        b = min(255, int(b_base * intensity + spec))

        # Rasterize triangle via bounding box
        min_x = max(0, min(s0x, s1x, s2x))
        max_x = min(w - 1, max(s0x, s1x, s2x))
        min_y = max(0, min(s0y, s1y, s2y))
        max_y = min(h - 1, max(s0y, s1y, s2y))

        denom = (s1y - s2y) * (s0x - s2x) + (s2x - s1x) * (s0y - s2y)
        if abs(denom) < 1e-5: continue
        inv_denom = 1.0 / denom

        avg_depth = (d0 + d1 + d2) / 3.0

        if wireframe:
            # Draw edges
            def draw_line(x_a, y_a, x_b, y_b):
                dx_l = abs(x_b - x_a)
                dy_l = abs(y_b - y_a)
                sx_l = 1 if x_a < x_b else -1
                sy_l = 1 if y_a < y_b else -1
                err = dx_l - dy_l
                curr_x, curr_y = x_a, y_a
                while True:
                    if 0 <= curr_x < w and 0 <= curr_y < h:
                        pix_idx = (curr_y * w + curr_x) * 3
                        buffer[pix_idx] = 0
                        buffer[pix_idx+1] = 230
                        buffer[pix_idx+2] = 255
                    if curr_x == x_b and curr_y == y_b: break
                    e2 = 2 * err
                    if e2 > -dy_l: err -= dy_l; curr_x += sx_l
                    if e2 < dx_l: err += dx_l; curr_y += sy_l

            draw_line(s0x, s0y, s1x, s1y)
            draw_line(s1x, s1y, s2x, s2y)
            draw_line(s2x, s2y, s0x, s0y)
            continue

        for py in range(min_y, max_y + 1):
            w0_row = (s1y - s2y) * (min_x - s2x) + (s2x - s1x) * (py - s2y)
            w1_row = (s2y - s0y) * (min_x - s2x) + (s0x - s2x) * (py - s2y)
            row_idx = py * w

            for px in range(min_x, max_x + 1):
                w0 = ((s1y - s2y) * (px - s2x) + (s2x - s1x) * (py - s2y)) * inv_denom
                w1 = ((s2y - s0y) * (px - s2x) + (s0x - s2x) * (py - s2y)) * inv_denom
                w2 = 1.0 - w0 - w1

                if w0 >= 0 and w1 >= 0 and w2 >= 0:
                    depth = w0 * d0 + w1 * d1 + w2 * d2
                    pix = row_idx + px
                    if depth < z_buffer[pix]:
                        z_buffer[pix] = depth
                        p_offset = pix * 3
                        buffer[p_offset] = r
                        buffer[p_offset+1] = g
                        buffer[p_offset+2] = b

    write_png(filename, w, h, buffer)
    # Duplicate to brain artifact dir
    base_n = os.path.basename(filename)
    brain_copy = os.path.join(out_dir, base_n)
    with open(filename, 'rb') as src, open(brain_copy, 'wb') as dst:
        dst.write(src.read())

    print(f"Rendered {base_n} in {time.time()-t0:.2f}s ({os.path.getsize(filename):,} bytes)")

print("=== GENERATING HIGH-RESOLUTION GATE 3 VERIFICATION RENDERS ===")

# 1. Full Body Hero Front
render_view(f"{local_out}/01_v2_hero_front.png", yaw_deg=20, pitch_deg=8, target=(0, 0, 0.98), dist=2.8, fov=44)

# 2. Rear PLSS Backpack & Dual O2 Tanks
render_view(f"{local_out}/02_v2_rear_plss.png", yaw_deg=195, pitch_deg=10, target=(0, -0.15, 1.34), dist=2.2, fov=40)

# 3. Helmet, Dual-Layer Visor & Comms Pod Close-Up
render_view(f"{local_out}/03_v2_helmet_closeup.png", yaw_deg=25, pitch_deg=6, target=(0, 0.05, 1.68), dist=1.1, fov=32)

# 4. Elbow Joint 6-Ring Bellows Close-Up
render_view(f"{local_out}/04_v2_elbow_bellows_closeup.png", yaw_deg=35, pitch_deg=-10, target=(0.36, 0, 1.18), dist=1.0, fov=30)

# 5. Articulated 5-Finger Glove Close-Up
render_view(f"{local_out}/05_v2_glove_fingers_closeup.png", yaw_deg=15, pitch_deg=12, target=(0.46, 0.02, 0.85), dist=0.85, fov=28)

# 6. Knee Strike Shields & Convolute Bellows Close-Up
render_view(f"{local_out}/06_v2_knee_bellows_closeup.png", yaw_deg=-15, pitch_deg=-8, target=(-0.14, 0.05, 0.52), dist=1.1, fov=32)

# 7. Mars Cleated Lug Boot Close-Up
render_view(f"{local_out}/07_v2_boot_cleats_closeup.png", yaw_deg=-25, pitch_deg=15, target=(-0.14, 0.04, 0.08), dist=0.9, fov=30)

# 8. Full Body Topology Wireframe
render_view(f"{local_out}/08_v2_wireframe_topology.png", yaw_deg=30, pitch_deg=10, target=(0, 0, 0.98), dist=2.8, fov=44, wireframe=True)

print("=== ALL GATE 3 VERIFICATION RENDERS GENERATED SUCCESSFULLY! ===")
