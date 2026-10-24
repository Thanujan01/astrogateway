import math

def generate_chest():
    vertices = []
    normals = []
    texcoords = []
    
    # helper to add a quad
    # returns face vertex indices (1-based)
    def add_quad(v1, v2, v3, v4, n):
        # n is normal vector
        ni = len(normals) + 1
        normals.append(n)
        
        idx = len(vertices) + 1
        vertices.extend([v1, v2, v3, v4])
        # Two triangles or quad
        return [
            (idx, ni), (idx+1, ni), (idx+2, ni), (idx+3, ni)
        ]

    # Let's write structured OBJ content with groups
    obj_lines = ["# Stylized Game Object - Sci-Fi / Fantasy Loot Chest", "mtllib chest.mtl"]
    
    # We will collect mesh parts: (group_name, material_name, quads/tris)
    # Each quad is 4 points: [(x,y,z), (x,y,z), (x,y,z), (x,y,z)] with normal
    parts = []

    # 1. MAIN BODY (Wood/Metal Panels)
    # Box from x:[-0.9, 0.9], y:[-0.6, 0.6], z:[0.1, 0.85]
    bx0, bx1 = -0.9, 0.9
    by0, by1 = -0.6, 0.6
    bz0, bz1 = 0.1, 0.85

    body_quads = [
        # Front face (-Y)
        ([(bx0, by0, bz0), (bx1, by0, bz0), (bx1, by0, bz1), (bx0, by0, bz1)], (0, -1, 0)),
        # Back face (+Y)
        ([(bx1, by1, bz0), (bx0, by1, bz0), (bx0, by1, bz1), (bx1, by1, bz1)], (0, 1, 0)),
        # Left face (-X)
        ([(bx0, by1, bz0), (bx0, by0, bz0), (bx0, by0, bz1), (bx0, by1, bz1)], (-1, 0, 0)),
        # Right face (+X)
        ([(bx1, by0, bz0), (bx1, by1, bz0), (bx1, by1, bz1), (bx1, by0, bz1)], (1, 0, 0)),
        # Bottom face (-Z)
        ([(bx0, by1, bz0), (bx1, by1, bz0), (bx1, by0, bz0), (bx0, by0, bz0)], (0, 0, -1)),
    ]
    parts.append(("ChestBody", "BodyMaterial", body_quads))

    # 2. LID (Curved Vault)
    # Arc along Y axis from -0.62 to +0.62
    # X from -0.92 to +0.92
    # Radius ~ 0.65, center at (0, y, 0.85)
    lid_segments = 12
    lid_quads = []
    r = 0.65
    cy = 0.85
    lx0, lx1 = -0.92, 0.92
    ly0, ly1 = -0.62, 0.62

    # Arc angles from 0 to pi
    arc_points = []
    for i in range(lid_segments + 1):
        angle = math.pi * i / lid_segments
        # y goes from -r * cos(angle)
        y = - (r * math.cos(angle))
        z = cy + (r * math.sin(angle)) * 0.75 # slightly flattened arch
        arc_points.append((y, z, angle))

    for i in range(lid_segments):
        y_a, z_a, ang_a = arc_points[i]
        y_b, z_b, ang_b = arc_points[i+1]
        mid_ang = (ang_a + ang_b) / 2.0
        ny = -math.cos(mid_ang)
        nz = math.sin(mid_ang)
        # Quad along X
        lid_quads.append(([
            (lx0, y_a, z_a), (lx1, y_a, z_a), (lx1, y_b, z_b), (lx0, y_b, z_b)
        ], (0, ny, nz)))

    # Lid left & right caps (semi-cylinders / fans)
    # We can do triangles from center
    lid_tris = []
    center_cap_left = (lx0, 0.0, cy)
    center_cap_right = (lx1, 0.0, cy)
    for i in range(lid_segments):
        y_a, z_a, _ = arc_points[i]
        y_b, z_b, _ = arc_points[i+1]
        # left cap (-X normal)
        lid_tris.append(([center_cap_left, (lx0, y_b, z_b), (lx0, y_a, z_a)], (-1, 0, 0)))
        # right cap (+X normal)
        lid_tris.append(([center_cap_right, (lx1, y_a, z_a), (lx1, y_b, z_b)], (1, 0, 0)))

    parts.append(("ChestLid", "BodyMaterial", lid_quads, lid_tris))

    # 3. REINFORCED GOLD/BRONZE BANDS & CORNERS
    # Corner braces: 4 vertical pillars at corners
    trim_quads = []
    t_w = 0.12 # trim width
    t_h = 0.88 # height
    t_d = 0.04 # thickness extrusion

    corners = [
        (-0.92, -0.62, -1, -1),
        ( 0.92, -0.62,  1, -1),
        ( 0.92,  0.62,  1,  1),
        (-0.92,  0.62, -1,  1),
    ]
    for cx, cy_c, sx, sy in corners:
        # vertical pillar
        x1 = cx
        x2 = cx - sx * t_w
        y1 = cy_c
        y2 = cy_c - sy * t_w
        z1 = 0.08
        z2 = 0.86
        # Outer corner faces
        trim_quads.append(([
            (x1, y1, z1), (x2, y1, z1), (x2, y1, z2), (x1, y1, z2)
        ], (0, sy, 0)))
        trim_quads.append(([
            (x1, y1, z1), (x1, y2, z1), (x1, y2, z2), (x1, y1, z2)
        ], (sx, 0, 0)))

    # Arch Trim Bands (3 bands over the lid: left, middle, right)
    for bx_center in [-0.85, 0.0, 0.85]:
        bw = 0.1
        b_x0 = bx_center - bw/2
        b_x1 = bx_center + bw/2
        b_r = r + 0.025
        for i in range(lid_segments):
            ang_a = math.pi * i / lid_segments
            ang_b = math.pi * (i + 1) / lid_segments
            y_a = - (b_r * math.cos(ang_a))
            z_a = cy + (b_r * math.sin(ang_a)) * 0.75 + 0.015
            y_b = - (b_r * math.cos(ang_b))
            z_b = cy + (b_r * math.sin(ang_b)) * 0.75 + 0.015
            mid_ang = (ang_a + ang_b) / 2.0
            ny = -math.cos(mid_ang)
            nz = math.sin(mid_ang)
            trim_quads.append(([
                (b_x0, y_a, z_a), (b_x1, y_a, z_a), (b_x1, y_b, z_b), (b_x0, y_b, z_b)
            ], (0, ny, nz)))

    # Base Trim Frame (bottom edge all around)
    for edge in [
        ((-0.93, -0.63, 0.05), (0.93, -0.63, 0.05), (0.93, -0.63, 0.16), (-0.93, -0.63, 0.16), (0, -1, 0)),
        ((0.93, 0.63, 0.05), (-0.93, 0.63, 0.05), (-0.93, 0.63, 0.16), (0.93, 0.63, 0.16), (0, 1, 0)),
        ((-0.93, 0.63, 0.05), (-0.93, -0.63, 0.05), (-0.93, -0.63, 0.16), (-0.93, 0.63, 0.16), (-1, 0, 0)),
        ((0.93, -0.63, 0.05), (0.93, 0.63, 0.05), (0.93, 0.63, 0.16), (0.93, -0.63, 0.16), (1, 0, 0)),
    ]:
        trim_quads.append(([edge[0], edge[1], edge[2], edge[3]], edge[4]))

    parts.append(("GoldTrim", "TrimMaterial", trim_quads))

    # 4. GLOWING ENERGY LOCK & EMBEDDED CRYSTAL GEM
    # Lock plate
    lock_quads = [
        ([(-0.16, -0.65, 0.65), (0.16, -0.65, 0.65), (0.16, -0.65, 0.95), (-0.16, -0.65, 0.95)], (0, -1, 0)),
        ([(-0.16, -0.65, 0.65), (-0.16, -0.61, 0.65), (-0.16, -0.61, 0.95), (-0.16, -0.65, 0.95)], (-1, 0, 0)),
        ([(0.16, -0.61, 0.65), (0.16, -0.65, 0.65), (0.16, -0.65, 0.95), (0.16, -0.61, 0.95)], (1, 0, 0)),
    ]
    parts.append(("LockPlate", "TrimMaterial", lock_quads))

    # Glowing Crystal Core (Octahedron diamond at front center)
    gem_center = (0.0, -0.67, 0.80)
    gw = 0.10
    gh = 0.14
    gd = 0.10
    gem_top = (gem_center[0], gem_center[1], gem_center[2] + gh)
    gem_bot = (gem_center[0], gem_center[1], gem_center[2] - gh)
    gem_pts = [
        (gem_center[0] - gw, gem_center[1], gem_center[2]),
        (gem_center[0], gem_center[1] - gd, gem_center[2]),
        (gem_center[0] + gw, gem_center[1], gem_center[2]),
        (gem_center[0], gem_center[1] + gd, gem_center[2]),
    ]
    gem_tris = []
    # top 4 triangles
    for i in range(4):
        p1 = gem_pts[i]
        p2 = gem_pts[(i+1)%4]
        # normal roughly outwards
        nx = (p1[0] + p2[0] + gem_top[0])/3 - gem_center[0]
        ny = (p1[1] + p2[1] + gem_top[1])/3 - gem_center[1]
        nz = (p1[2] + p2[2] + gem_top[2])/3 - gem_center[2]
        l = math.sqrt(nx*nx + ny*ny + nz*nz) or 1
        gem_tris.append(([gem_top, p1, p2], (nx/l, ny/l, nz/l)))
    # bottom 4 triangles
    for i in range(4):
        p1 = gem_pts[i]
        p2 = gem_pts[(i+1)%4]
        nx = (p1[0] + p2[0] + gem_bot[0])/3 - gem_center[0]
        ny = (p1[1] + p2[1] + gem_bot[1])/3 - gem_center[1]
        nz = (p1[2] + p2[2] + gem_bot[2])/3 - gem_center[2]
        l = math.sqrt(nx*nx + ny*ny + nz*nz) or 1
        gem_tris.append(([p2, p1, gem_bot], (nx/l, ny/l, nz/l)))

    parts.append(("GlowingCore", "CrystalMaterial", [], gem_tris))

    # 5. ASSEMBLE OBJ
    all_v = []
    all_vn = []
    f_lines = []

    v_map = {}
    vn_map = {}

    def get_v_idx(pt):
        pt_key = (round(pt[0], 5), round(pt[1], 5), round(pt[2], 5))
        if pt_key not in v_map:
            all_v.append(pt_key)
            v_map[pt_key] = len(all_v)
        return v_map[pt_key]

    def get_vn_idx(norm):
        norm_key = (round(norm[0], 4), round(norm[1], 4), round(norm[2], 4))
        if norm_key not in vn_map:
            all_vn.append(norm_key)
            vn_map[norm_key] = len(all_vn)
        return vn_map[norm_key]

    for item in parts:
        gname = item[0]
        matname = item[1]
        quads = item[2]
        tris = item[3] if len(item) > 3 else []

        f_lines.append(f"g {gname}")
        f_lines.append(f"usemtl {matname}")

        for pts, norm in quads:
            vi = [get_v_idx(p) for p in pts]
            ni = get_vn_idx(norm)
            f_lines.append(f"f {vi[0]}//{ni} {vi[1]}//{ni} {vi[2]}//{ni} {vi[3]}//{ni}")

        for pts, norm in tris:
            vi = [get_v_idx(p) for p in pts]
            ni = get_vn_idx(norm)
            f_lines.append(f"f {vi[0]}//{ni} {vi[1]}//{ni} {vi[2]}//{ni}")

    with open("/Users/kapilash/.gemini/antigravity/scratch/3d-game-assets/loot_chest.obj", "w") as f:
        f.write("# Stylized Game Asset: Sci-Fi / Fantasy Loot Chest\n")
        f.write("mtllib loot_chest.mtl\n\n")
        for v in all_v:
            f.write(f"v {v[0]:.4f} {v[1]:.4f} {v[2]:.4f}\n")
        f.write("\n")
        for vn in all_vn:
            f.write(f"vn {vn[0]:.4f} {vn[1]:.4f} {vn[2]:.4f}\n")
        f.write("\n")
        for line in f_lines:
            f.write(line + "\n")

    print(f"Generated OBJ with {len(all_v)} vertices, {len(all_vn)} normals.")

generate_chest()
