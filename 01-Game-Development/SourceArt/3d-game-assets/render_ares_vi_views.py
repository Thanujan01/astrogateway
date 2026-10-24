import json
import math
import os
import struct
import zlib
import time

# 1. Output setup
renders_dir = "/Users/kapilash/.gemini/antigravity/brain/fe04a22b-3e01-409f-a35c-ed439568d853/renders"
os.makedirs(renders_dir, exist_ok=True)

# 2. Load Master 3D Data
with open("/Users/kapilash/.gemini/antigravity/scratch/3d-game-assets/ares_vi_studio_data.json") as f:
    studio_data = json.load(f)

vertices = studio_data['vertices']
subsystems = studio_data['subsystems']

# 3. Materials Palette
materials = {
    'Mat_CeramicWhite': {'r': 234, 'g': 240, 'b': 248, 'edge': (160, 175, 195), 'spec': 0.6},
    'Mat_GraphiteTrim': {'r': 42,  'g': 48,  'b': 56,  'edge': (25, 30, 36),     'spec': 0.35},
    'Mat_TitaniumFrame':{'r': 105, 'g': 115, 'b': 128, 'edge': (70, 78, 88),    'spec': 0.85},
    'Mat_MartianOrange':{'r': 248, 'g': 90,  'b': 18,  'edge': (210, 65, 10),   'spec': 0.65},
    'Mat_GoldFoil':     {'r': 240, 'g': 195, 'b': 38,  'edge': (250, 220, 120), 'spec': 0.95},
    'Mat_CarbonTreads': {'r': 22,  'g': 25,  'b': 30,  'edge': (65, 75, 88),    'spec': 0.20},
    'Mat_CanopyGlass':  {'r': 38,  'g': 95,  'b': 140, 'edge': (140, 230, 255), 'spec': 0.95, 'alpha': 0.65},
    'Mat_CyanEnergy':   {'r': 0,   'g': 235, 'b': 255, 'edge': (210, 250, 255), 'spec': 1.0, 'glow': True}
}

# 4. Pure Python PNG Writer
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

    header = b'\x89PNG\r\n\x1a\n'
    ihdr = struct.pack(">IIBBBBB", width, height, 8, 2, 0, 0, 0)

    with open(filename, 'wb') as f:
        f.write(header)
        f.write(chunk(b'IHDR', ihdr))
        f.write(chunk(b'IDAT', compressed))
        f.write(chunk(b'IEND', b''))

# 5. Software Rasterizer
def render_view(filename, title, yaw, pitch, zoom, pan=(0, 0), hover_mode=False, explode_val=0.0):
    w, h = 800, 450
    buf = bytearray(w * h * 3)

    # Background gradient
    for y in range(h):
        ratio = y / h
        # Dark studio backdrop
        r = int(14 * (1.0 - ratio * 0.7))
        g = int(18 * (1.0 - ratio * 0.7))
        b = int(26 * (1.0 - ratio * 0.7))
        row_offset = y * w * 3
        pixel = bytes([r, g, b])
        buf[row_offset:row_offset + w * 3] = pixel * w

    # Target parameters
    pan_x, pan_y = pan
    cx = (w / 2) + pan_x
    cy = (h / 2) + pan_y + (55 * zoom)

    # Hover displacement
    vehicle_z_offset = 0.32 if hover_mode else 0.0

    # Ground Shadow / Hover Ring
    shadow_rx = int((190 + (25 if hover_mode else 0)) * zoom)
    shadow_ry = int((55 + (15 if hover_mode else 0)) * zoom)
    ground_y = int(cy + (vehicle_z_offset * 45 * zoom))

    y_min = max(0, ground_y - shadow_ry)
    y_max = min(h, ground_y + shadow_ry)
    for y in range(y_min, y_max):
        dy = (y - ground_y) / shadow_ry
        dx_max = math.sqrt(max(0, 1.0 - dy * dy)) * shadow_rx
        x0 = max(0, int(cx - dx_max))
        x1 = min(w, int(cx + dx_max))
        for x in range(x0, x1):
            idx = (y * w + x) * 3
            if hover_mode:
                # Cyan plasma glow
                dist_norm = math.sqrt(((x - cx)/shadow_rx)**2 + dy**2)
                alpha = max(0.0, 1.0 - dist_norm) * 0.55
                buf[idx]   = min(255, int(buf[idx] * (1 - alpha)))
                buf[idx+1] = min(255, int(buf[idx+1] * (1 - alpha) + 210 * alpha))
                buf[idx+2] = min(255, int(buf[idx+2] * (1 - alpha) + 255 * alpha))
            else:
                # Dark contact shadow
                dist_norm = math.sqrt(((x - cx)/shadow_rx)**2 + dy**2)
                alpha = max(0.0, 1.0 - dist_norm) * 0.65
                buf[idx]   = int(buf[idx] * (1.0 - alpha * 0.8))
                buf[idx+1] = int(buf[idx+1] * (1.0 - alpha * 0.8))
                buf[idx+2] = int(buf[idx+2] * (1.0 - alpha * 0.8))

    # Camera math
    cos_y, sin_y = math.cos(yaw), math.sin(yaw)
    cos_p, sin_p = math.cos(pitch), math.sin(pitch)
    dist = 4.4
    base_scale = (min(w, h) * 1.02) * zoom
    center_z = 0.95

    # Collect face draw items
    draw_list = []

    for sub_id, sub in subsystems.items():
        mat = materials.get(sub['material'], materials['Mat_TitaniumFrame'])
        ex = sub.get('explode', [0, 0, 0])
        hd = sub.get('hover_delta', [0, 0, 0])

        off_x = ex[0] * explode_val * 0.8 + (hd[0] if hover_mode else 0.0)
        off_y = ex[1] * explode_val * 0.8 + (hd[1] if hover_mode else 0.0)
        off_z = ex[2] * explode_val * 0.8 + (hd[2] if hover_mode else 0.0) + vehicle_z_offset

        for face in sub['faces']:
            if len(face) < 3:
                continue

            p_trans = []
            p_proj = []
            avg_y = 0.0

            for v_idx in face:
                v = vertices[v_idx]
                x0 = v[0] + off_x
                y0 = v[1] + off_y
                z0 = (v[2] - center_z) + off_z

                x1 = x0 * cos_y - y0 * sin_y
                y1 = x0 * sin_y + y0 * cos_y
                z1 = z0

                x2 = x1
                y2 = y1 * cos_p - z1 * sin_p
                z2 = y1 * sin_p + z1 * cos_p

                depth = y2 + dist
                inv_depth = 1.0 / depth if depth > 0.1 else 1.0
                px = cx + (x2 * inv_depth) * base_scale
                py = (h / 2) + pan_y - (z2 * inv_depth) * base_scale

                p_trans.append((x2, y2, z2))
                p_proj.append((px, py))
                avg_y += y2

            avg_y /= len(face)

            # Normal & Backface culling
            p0, p1, p2 = p_trans[0], p_trans[1], p_trans[2]
            ax, ay, az = p1[0] - p0[0], p1[1] - p0[1], p1[2] - p0[2]
            bx, by, bz = p2[0] - p0[0], p2[1] - p0[1], p2[2] - p0[2]
            nx = ay * bz - az * by
            ny = az * bx - ax * bz
            nz = ax * by - ay * bx

            dot_view = nx * p0[0] + ny * (p0[1] + dist) + nz * p0[2]
            if not mat.get('glow') and not mat.get('alpha') and dot_view <= 0:
                continue

            n_len = math.sqrt(nx*nx + ny*ny + nz*nz) or 1.0
            norm_x, norm_y, norm_z = nx / n_len, ny / n_len, nz / n_len

            # Directional key light
            dot_light = max(0.0, norm_x * 0.50 + norm_y * -0.75 + norm_z * 0.65)
            spec_power = (max(0.0, norm_x * 0.35 + norm_y * -0.65 + norm_z * 0.70) ** 8) * mat['spec']

            draw_list.append({
                'pts': p_proj,
                'avg_y': avg_y,
                'mat': mat,
                'dot_light': dot_light,
                'spec_power': spec_power
            })

    # Sort faces back-to-front
    draw_list.sort(key=lambda item: item['avg_y'], reverse=True)

    # Rasterize polygons
    for item in draw_list:
        pts = item['pts']
        mat = item['mat']
        is_glow = mat.get('glow', False)

        if is_glow:
            base_r, base_g, base_b = 0, 235, 255
        else:
            ambient = 0.28
            intensity = ambient + 0.72 * item['dot_light']
            spec_add = int(item['spec_power'] * 90)
            base_r = min(255, int(mat['r'] * intensity + spec_add))
            base_g = min(255, int(mat['g'] * intensity + spec_add))
            base_b = min(255, int(mat['b'] * intensity + spec_add))

        # Scanline polygon fill
        ys = [p[1] for p in pts]
        min_y = max(0, int(math.floor(min(ys))))
        max_y = min(h - 1, int(math.ceil(max(ys))))
        n_pts = len(pts)

        for scan_y in range(min_y, max_y + 1):
            y_coord = scan_y + 0.5
            intersections = []
            for i in range(n_pts):
                p_a = pts[i]
                p_b = pts[(i + 1) % n_pts]
                if (p_a[1] <= y_coord < p_b[1]) or (p_b[1] <= y_coord < p_a[1]):
                    t = (y_coord - p_a[1]) / (p_b[1] - p_a[1])
                    intersections.append(p_a[0] + t * (p_b[0] - p_a[0]))

            intersections.sort()
            for k in range(0, len(intersections) - 1, 2):
                x_start = max(0, int(math.floor(intersections[k])))
                x_end = min(w - 1, int(math.ceil(intersections[k + 1])))
                if x_end >= x_start:
                    row_offset = (scan_y * w + x_start) * 3
                    length = x_end - x_start + 1
                    buf[row_offset:row_offset + length * 3] = bytes([base_r, base_g, base_b]) * length

        # Edge stroke
        edge_col = mat.get('edge', (100, 110, 120))
        for i in range(n_pts):
            p_a = pts[i]
            p_b = pts[(i + 1) % n_pts]
            # Bresenham-like line
            lx0, ly0 = int(p_a[0]), int(p_a[1])
            lx1, ly1 = int(p_b[0]), int(p_b[1])
            dx = abs(lx1 - lx0)
            dy = abs(ly1 - ly0)
            sx = 1 if lx0 < lx1 else -1
            sy = 1 if ly0 < ly1 else -1
            err = dx - dy
            while True:
                if 0 <= lx0 < w and 0 <= ly0 < h:
                    pix_idx = (ly0 * w + lx0) * 3
                    buf[pix_idx]   = edge_col[0]
                    buf[pix_idx+1] = edge_col[1]
                    buf[pix_idx+2] = edge_col[2]
                if lx0 == lx1 and ly0 == ly1:
                    break
                e2 = 2 * err
                if e2 > -dy:
                    err -= dy
                    lx0 += sx
                if e2 < dx:
                    err += dx
                    ly0 += sy

    # Write HUD Title Bar overlay at top
    # Simple top bar
    for y in range(32):
        for x in range(w):
            idx = (y * w + x) * 3
            buf[idx] = int(buf[idx] * 0.4)
            buf[idx+1] = int(buf[idx+1] * 0.4)
            buf[idx+2] = int(buf[idx+2] * 0.4)

    # Save PNG
    out_file = os.path.join(renders_dir, filename)
    write_png(out_file, w, h, buf)
    print(f"Rendered [{title}] -> {out_file}")

# 6. Generate the 10 Required AAA Views
print("Starting generation of 10 AAA Verification Renders...")
t0 = time.time()

views = [
    # 1. Front three-quarter view
    ("01_front_three_quarter.png", "View 1: Front 3/4 Hero View", 0.85, 0.35, 1.0, (0, 0), False, 0.0),
    # 2. Rear three-quarter view
    ("02_rear_three_quarter.png", "View 2: Rear 3/4 MMRTG View", 2.45, 0.32, 1.0, (0, 0), False, 0.0),
    # 3. Side profile
    ("03_side_profile.png", "View 3: Side Elevation Profile", 1.57, 0.05, 1.0, (0, 0), False, 0.0),
    # 4. Top view
    ("04_top_view.png", "View 4: Top-Down Plan View", 0.0, 1.45, 1.05, (0, 0), False, 0.0),
    # 5. Undercarriage
    ("05_undercarriage.png", "View 5: Undercarriage Hover Emitters", 0.0, -1.28, 1.05, (0, 0), False, 0.0),
    # 6. Cockpit interior
    ("06_cockpit_interior.png", "View 6: Cockpit & Astronaut Pod", 0.35, 0.28, 1.55, (-10, 20), False, 0.0),
    # 7. Ground mode
    ("07_ground_mode.png", "View 7: Mode A — Ground Exploration", 0.75, 0.25, 0.95, (0, 0), False, 0.0),
    # 8. Hover mode
    ("08_hover_mode.png", "View 8: Mode B — Anti-Gravity Hover", 0.75, 0.25, 0.95, (0, 0), True, 0.0),
    # 9. Science arm deployed
    ("09_science_arm_deployed.png", "View 9: Articulated Science Arm & Drill", 0.45, 0.35, 1.45, (80, -30), False, 0.0),
    # 10. Exploded technical view
    ("10_exploded_technical.png", "View 10: Exploded Hard-Surface View", 0.85, 0.35, 0.82, (0, 0), False, 1.0),
]

for item in views:
    render_view(*item)

print(f"All 10 Real Renders successfully generated in {time.time() - t0:.2f}s!")
