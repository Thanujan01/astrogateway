import json
import math
import os
import struct
import zlib
import time

renders_dir = "/Users/kapilash/.gemini/antigravity/brain/fe04a22b-3e01-409f-a35c-ed439568d853/renders_astronaut"
os.makedirs(renders_dir, exist_ok=True)

with open("/Users/kapilash/.gemini/antigravity/scratch/ARES_EVA_MK1/concept/concept_blockout.json") as f:
    concept_data = json.load(f)

vertices = concept_data['vertices']
subsystems = concept_data['subsystems']

# NASA Aerospace Palette
materials = {
    'Mat_CeramicWhite': {'r': 229, 'g': 230, 'b': 226, 'edge': (175, 180, 185), 'spec': 0.55}, # #E5E6E2
    'Mat_Graphite':     {'r': 37,  'g': 43,  'b': 50,  'edge': (25, 30, 36),     'spec': 0.35}, # #252B32
    'Mat_Titanium':     {'r': 137, 'g': 147, 'b': 155, 'edge': (95, 102, 110),   'spec': 0.88}, # #89939B
    'Mat_SafetyOrange': {'r': 217, 'g': 106, 'b': 41,  'edge': (175, 75, 25),    'spec': 0.65}, # #D96A29
    'Mat_GoldVisor':    {'r': 242, 'g': 196, 'b': 38,  'edge': (250, 220, 120), 'spec': 0.98}, # #F2C426
    'Mat_CyanEnergy':   {'r': 0,   'g': 201, 'b': 232, 'edge': (180, 240, 255), 'spec': 1.0, 'glow': True} # #00C9E8
}

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

def render_view(filename, title, yaw, pitch, zoom, pan=(0, 0), pose_type="standing"):
    w, h = 800, 600
    buf = bytearray(w * h * 3)

    # Dark studio background gradient
    for y in range(h):
        ratio = y / h
        r = int(14 * (1.0 - ratio * 0.7))
        g = int(18 * (1.0 - ratio * 0.7))
        b = int(26 * (1.0 - ratio * 0.7))
        row_offset = y * w * 3
        pixel = bytes([r, g, b])
        buf[row_offset:row_offset + w * 3] = pixel * w

    pan_x, pan_y = pan
    cx = (w / 2) + pan_x
    cy = (h / 2) + pan_y + (160 * zoom)

    # Floor shadow
    shadow_rx = int(115 * zoom)
    shadow_ry = int(35 * zoom)
    ground_y = int(cy + 15 * zoom)

    y_min = max(0, ground_y - shadow_ry)
    y_max = min(h, ground_y + shadow_ry)
    for y in range(y_min, y_max):
        dy = (y - ground_y) / shadow_ry
        dx_max = math.sqrt(max(0, 1.0 - dy * dy)) * shadow_rx
        x0 = max(0, int(cx - dx_max))
        x1 = min(w, int(cx + dx_max))
        for x in range(x0, x1):
            idx = (y * w + x) * 3
            dist_norm = math.sqrt(((x - cx)/shadow_rx)**2 + dy**2)
            alpha = max(0.0, 1.0 - dist_norm) * 0.65
            buf[idx]   = int(buf[idx] * (1.0 - alpha * 0.8))
            buf[idx+1] = int(buf[idx+1] * (1.0 - alpha * 0.8))
            buf[idx+2] = int(buf[idx+2] * (1.0 - alpha * 0.8))

    cos_y, sin_y = math.cos(yaw), math.sin(yaw)
    cos_p, sin_p = math.cos(pitch), math.sin(pitch)
    dist = 3.6
    base_scale = (min(w, h) * 1.05) * zoom
    center_z = 0.95

    draw_list = []

    for sub_id, sub in subsystems.items():
        mat = materials.get(sub['mat'], materials['Mat_CeramicWhite'])

        for face in sub['faces']:
            if len(face) < 3:
                continue

            p_trans = []
            p_proj = []
            avg_y = 0.0

            for v_idx in face:
                v = vertices[v_idx]
                vx, vy, vz = v[0], v[1], v[2]

                # Procedural pose deformation for seated / walking tests
                if pose_type == "seated":
                    if vz < 0.96 and vz > 0.48: # Thighs forward
                        vy += 0.28 * (1.0 - (vz - 0.48) / 0.48)
                        vz = 0.58 + (vz - 0.48) * 0.15
                    elif vz <= 0.48: # Lower legs down
                        vy += 0.32
                        vz = vz * 0.85
                    else: # Upper body lowered
                        vz -= 0.36
                elif pose_type == "walk":
                    if "Leg_L" in sub_id or "Boot_L" in sub_id:
                        vy += 0.16 * math.sin(vz * 3.0)
                    elif "Leg_R" in sub_id or "Boot_R" in sub_id:
                        vy -= 0.16 * math.sin(vz * 3.0)

                x0 = vx
                y0 = vy
                z0 = vz - center_z

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

            # Normal & Culling
            p0, p1, p2 = p_trans[0], p_trans[1], p_trans[2]
            ax, ay, az = p1[0] - p0[0], p1[1] - p0[1], p1[2] - p0[2]
            bx, by, bz = p2[0] - p0[0], p2[1] - p0[1], p2[2] - p0[2]
            nx = ay * bz - az * by
            ny = az * bx - ax * bz
            nz = ax * by - ay * bx

            dot_view = nx * p0[0] + ny * (p0[1] + dist) + nz * p0[2]
            if not mat.get('glow') and dot_view <= 0:
                continue

            n_len = math.sqrt(nx*nx + ny*ny + nz*nz) or 1.0
            norm_x, norm_y, norm_z = nx / n_len, ny / n_len, nz / n_len

            dot_light = max(0.0, norm_x * 0.48 + norm_y * -0.72 + norm_z * 0.65)
            spec_power = (max(0.0, norm_x * 0.35 + norm_y * -0.65 + norm_z * 0.70) ** 10) * mat['spec']

            draw_list.append({
                'pts': p_proj,
                'avg_y': avg_y,
                'mat': mat,
                'dot_light': dot_light,
                'spec_power': spec_power
            })

    draw_list.sort(key=lambda item: item['avg_y'], reverse=True)

    for item in draw_list:
        pts = item['pts']
        mat = item['mat']
        is_glow = mat.get('glow', False)

        if is_glow:
            base_r, base_g, base_b = 0, 201, 232
        else:
            ambient = 0.30
            intensity = ambient + 0.70 * item['dot_light']
            spec_add = int(item['spec_power'] * 95)
            base_r = min(255, int(mat['r'] * intensity + spec_add))
            base_g = min(255, int(mat['g'] * intensity + spec_add))
            base_b = min(255, int(mat['b'] * intensity + spec_add))

        # Scanline polygon rasterization
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

        # Edge line stroke
        edge_col = mat.get('edge', (100, 110, 120))
        for i in range(n_pts):
            p_a = pts[i]
            p_b = pts[(i + 1) % n_pts]
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

    # Header title bar
    for y in range(32):
        for x in range(w):
            idx = (y * w + x) * 3
            buf[idx]   = int(buf[idx] * 0.4)
            buf[idx+1] = int(buf[idx+1] * 0.4)
            buf[idx+2] = int(buf[idx+2] * 0.4)

    out_file = os.path.join(renders_dir, filename)
    write_png(out_file, w, h, buf)
    print(f"Rendered [{title}] -> {out_file}")

views = [
    ("01_astronaut_front_hero.png", "View 1: Front 3/4 Standing Hero Perspective", 0.55, 0.15, 1.0, (0, 0), "standing"),
    ("02_astronaut_rear_plss.png",  "View 2: Rear PLSS Life Support & O2 Canisters", 3.45, 0.15, 1.0, (0, 0), "standing"),
    ("03_astronaut_side_profile.png", "View 3: Side Profile Elevation & Joint Alignment", 1.57, 0.05, 1.0, (0, 0), "standing"),
    ("04_astronaut_helmet_closeup.png", "View 4: Close-Up — Helmet, Gold Visor & Spotlights", 0.35, 0.10, 2.3, (-10, 180), "standing"),
    ("05_astronaut_rover_seated.png", "View 5: Rover Seated Posture for ARES-VI Cockpit", 0.65, 0.20, 1.1, (0, 40), "seated"),
    ("06_astronaut_walk_passing.png", "View 6: Mars Walk Cycle Striding Pose", 0.60, 0.12, 1.0, (0, 0), "walk")
]

t0 = time.time()
for item in views:
    render_view(*item)
print(f"All Astronaut Renders Completed in {time.time() - t0:.2f}s!")
