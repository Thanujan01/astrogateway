import math
import json

def generate_mars_rover():
    vertices = []
    normals = []
    
    parts = [] # (group_name, material_name, quads, tris)
    
    # helper for box
    def make_box(x0, x1, y0, y1, z0, z1):
        quads = [
            # Front (+Y)
            ([(x0, y1, z0), (x1, y1, z0), (x1, y1, z1), (x0, y1, z1)], (0, 1, 0)),
            # Back (-Y)
            ([(x1, y0, z0), (x0, y0, z0), (x0, y0, z1), (x1, y0, z1)], (0, -1, 0)),
            # Left (-X)
            ([(x0, y0, z0), (x0, y1, z0), (x0, y1, z1), (x0, y0, z1)], (-1, 0, 0)),
            # Right (+X)
            ([(x1, y1, z0), (x1, y0, z0), (x1, y0, z1), (x1, y1, z1)], (1, 0, 0)),
            # Top (+Z)
            ([(x0, y0, z1), (x1, y0, z1), (x1, y1, z1), (x0, y1, z1)], (0, 0, 1)),
            # Bottom (-Z)
            ([(x0, y1, z0), (x1, y1, z0), (x1, y0, z0), (x0, y0, z0)], (0, 0, -1)),
        ]
        return quads

    # helper for cylinder along X, Y, or Z
    def make_cylinder(axis, c1, c2, c3, r, length, segs=8):
        # axis: 'X', 'Y', or 'Z'
        quads = []
        tris = []
        step = 2 * math.pi / segs
        
        pts_a = []
        pts_b = []
        for i in range(segs):
            ang = i * step
            ca = r * math.cos(ang)
            sa = r * math.sin(ang)
            if axis == 'X':
                # length along X from c1 - length/2 to c1 + length/2
                pts_a.append((c1 - length/2, c2 + ca, c3 + sa))
                pts_b.append((c1 + length/2, c2 + ca, c3 + sa))
            elif axis == 'Y':
                pts_a.append((c1 + ca, c2 - length/2, c3 + sa))
                pts_b.append((c1 + ca, c2 + length/2, c3 + sa))
            else: # 'Z'
                pts_a.append((c1 + ca, c2 + sa, c3 - length/2))
                pts_b.append((c1 + ca, c2 + sa, c3 + length/2))
                
        # side quads
        for i in range(segs):
            next_i = (i + 1) % segs
            p_a1 = pts_a[i]
            p_a2 = pts_a[next_i]
            p_b1 = pts_b[i]
            p_b2 = pts_b[next_i]
            # normal
            ang_mid = (i + 0.5) * step
            cm = math.cos(ang_mid)
            sm = math.sin(ang_mid)
            if axis == 'X': norm = (0, cm, sm)
            elif axis == 'Y': norm = (cm, 0, sm)
            else: norm = (cm, sm, 0)
            
            quads.append(([p_a1, p_b1, p_b2, p_a2], norm))
            
        # end caps
        if axis == 'X':
            c_a = (c1 - length/2, c2, c3)
            c_b = (c1 + length/2, c2, c3)
            norm_a = (-1, 0, 0)
            norm_b = (1, 0, 0)
        elif axis == 'Y':
            c_a = (c1, c2 - length/2, c3)
            c_b = (c1, c2 + length/2, c3)
            norm_a = (0, -1, 0)
            norm_b = (0, 1, 0)
        else:
            c_a = (c1, c2, c3 - length/2)
            c_b = (c1, c2, c3 + length/2)
            norm_a = (0, 0, -1)
            norm_b = (0, 0, 1)

        for i in range(segs):
            next_i = (i + 1) % segs
            tris.append(([c_a, pts_a[next_i], pts_a[i]], norm_a))
            tris.append(([c_b, pts_b[i], pts_b[next_i]], norm_b))
            
        return quads, tris

    # 1. MAIN CHASSIS (Body) - Aerogel insulated science body
    # Chamfered angular equipment deck
    body_quads = make_box(-0.65, 0.65, -0.85, 0.85, 0.65, 1.15)
    # Top electronics bay module
    deck_quads = make_box(-0.55, 0.55, -0.75, 0.45, 1.15, 1.28)
    parts.append(("RoverBody", "BodyMaterial", body_quads + deck_quads, []))

    # 2. SUSPENSION & STRUCTURAL CHASSIS (Dark Titanium Struts)
    strut_quads = []
    # Rocker-bogie main diagonal struts left & right
    # Left main rocker
    strut_quads += make_box(-0.75, -0.65, -0.1, 0.1, 0.75, 0.95)
    strut_quads += make_box(-1.05, -0.75, -0.05, 0.05, 0.70, 0.80)
    # Suspension links to wheels
    # Left front & rear links
    strut_quads += make_box(-1.10, -1.02, 0.0, 1.05, 0.35, 0.45)
    strut_quads += make_box(-1.10, -1.02, -1.05, 0.0, 0.35, 0.45)
    # Right main rocker
    strut_quads += make_box(0.65, 0.75, -0.1, 0.1, 0.75, 0.95)
    strut_quads += make_box(0.75, 1.05, -0.05, 0.05, 0.70, 0.80)
    # Right front & rear links
    strut_quads += make_box(1.02, 1.10, 0.0, 1.05, 0.35, 0.45)
    strut_quads += make_box(1.02, 1.10, -1.05, 0.0, 0.35, 0.45)

    # Front bumper / hazard sensor bar
    strut_quads += make_box(-0.60, 0.60, 0.85, 0.95, 0.70, 0.80)
    parts.append(("SuspensionStruts", "ChassisMaterial", strut_quads, []))

    # 3. SIX EXPLORATION TREAD WHEELS (Rough terrain Mars wheels)
    wheel_quads = []
    wheel_tris = []
    wheel_coords = [
        # Left 3 wheels (X = -1.15)
        (-1.15,  1.05, 0.35),
        (-1.15,  0.00, 0.35),
        (-1.15, -1.05, 0.35),
        # Right 3 wheels (X = 1.15)
        ( 1.15,  1.05, 0.35),
        ( 1.15,  0.00, 0.35),
        ( 1.15, -1.05, 0.35),
    ]
    for wx, wy, wz in wheel_coords:
        q, t = make_cylinder('X', wx, wy, wz, r=0.32, length=0.28, segs=8)
        wheel_quads += q
        wheel_tris += t
    parts.append(("Wheels", "WheelMaterial", wheel_quads, wheel_tris))

    # 4. REMOTE SENSING MAST & SENSOR HEAD (ChemCam & Stereo NavCams)
    mast_quads = []
    mast_tris = []
    # Vertical mast tube (Front Right deck at x=0.32, y=0.55, z=1.28 to 2.15)
    mq, mt = make_cylinder('Z', 0.32, 0.55, 1.72, r=0.06, length=0.86, segs=6)
    mast_quads += mq
    mast_tris += mt
    # Pan-Tilt Gimbal Base
    mast_quads += make_box(0.24, 0.40, 0.47, 0.63, 2.12, 2.22)
    # Mast Head Sensor Unit (Curiosity/Perseverance style head)
    mast_quads += make_box(0.18, 0.46, 0.42, 0.68, 2.22, 2.45)
    parts.append(("MastAssembly", "ChassisMaterial", mast_quads, mast_tris))

    # Sensor Optics & ChemCam Laser Lenses (Emissive Cyan / Lidar)
    lens_quads = []
    lens_tris = []
    # Main Laser ChemCam Lens (Center front of mast head)
    lq, lt = make_cylinder('Y', 0.32, 0.69, 2.36, r=0.07, length=0.04, segs=6)
    lens_quads += lq
    lens_tris += lt
    # Stereo NavCams (Left and Right lenses)
    lq2, lt2 = make_cylinder('Y', 0.22, 0.69, 2.27, r=0.04, length=0.03, segs=6)
    lens_quads += lq2
    lens_tris += lt2
    lq3, lt3 = make_cylinder('Y', 0.42, 0.69, 2.27, r=0.04, length=0.03, segs=6)
    lens_quads += lq3
    lens_tris += lt3
    # Front HazCam stereo sensors on lower chassis
    lq4, lt4 = make_cylinder('Y', -0.25, 0.86, 0.75, r=0.04, length=0.03, segs=6)
    lens_quads += lq4
    lens_tris += lt4
    lq5, lt5 = make_cylinder('Y', 0.25, 0.86, 0.75, r=0.04, length=0.03, segs=6)
    lens_quads += lq5
    lens_tris += lt5
    parts.append(("SensorLenses", "OpticsMaterial", lens_quads, lens_tris))

    # 5. ROBOTIC ARTICULATED SCIENCE ARM & DRILL TURRET
    arm_quads = []
    arm_tris = []
    # Shoulder joint (Front Left deck at x=-0.35, y=0.75, z=0.95)
    sq, st = make_cylinder('Y', -0.35, 0.78, 0.95, r=0.08, length=0.08, segs=6)
    arm_quads += sq
    arm_tris += st
    # Upper arm boom extending forward and down
    arm_quads += make_box(-0.38, -0.32, 0.78, 1.20, 0.78, 0.88)
    # Elbow turret
    eq, et = make_cylinder('X', -0.35, 1.22, 0.83, r=0.07, length=0.10, segs=6)
    arm_quads += eq
    arm_tris += et
    # Forearm extending downward towards ground
    arm_quads += make_box(-0.38, -0.32, 1.18, 1.28, 0.45, 0.83)
    # Science Drill & Spectrometer Turret tool head at end
    tq, tt = make_cylinder('Z', -0.35, 1.23, 0.40, r=0.11, length=0.16, segs=6)
    arm_quads += tq
    arm_tris += tt
    # Drill bit pointing to ground
    dq, dt = make_cylinder('Z', -0.35, 1.23, 0.26, r=0.03, length=0.12, segs=6)
    arm_quads += dq
    arm_tris += dt
    parts.append(("RoboticArm", "GoldFoilMaterial", arm_quads, arm_tris))

    # 6. REAR MMRTG NUCLEAR POWER GENERATOR & COOLING FINS
    rtg_quads = []
    rtg_tris = []
    # Cylindrical generator body at rear (x=0.0, y=-1.12, z=0.92)
    rq, rt = make_cylinder('Y', 0.0, -1.05, 0.92, r=0.18, length=0.42, segs=8)
    rtg_quads += rq
    rtg_tris += rt
    # Radiator cooling fins
    rtg_quads += make_box(-0.35, 0.35, -1.22, -0.88, 0.90, 0.94)
    rtg_quads += make_box(-0.02, 0.02, -1.22, -0.88, 0.72, 1.12)
    parts.append(("PowerGenerator", "ChassisMaterial", rtg_quads, rtg_tris))

    # 7. HIGH-GAIN DISH ANTENNA (Hexagonal parabolic dish for Earth comms)
    dish_quads = []
    dish_tris = []
    # Antenna mast support
    dish_quads += make_box(-0.42, -0.32, -0.55, -0.45, 1.28, 1.62)
    # Hexagonal dish (tilted back 35 degrees)
    dish_r = 0.28
    dish_cx, dish_cy, dish_cz = -0.37, -0.55, 1.75
    # hex points
    hex_pts = []
    hex_inner = []
    for i in range(6):
        ang = i * math.pi / 3
        # tilted dish orientation
        hx = dish_cx + dish_r * math.cos(ang)
        hy = dish_cy - dish_r * math.sin(ang) * 0.7
        hz = dish_cz + dish_r * math.sin(ang) * 0.7
        hex_pts.append((hx, hy, hz))
        
        # concavity center
        hxi = dish_cx + (dish_r * 0.35) * math.cos(ang)
        hyi = dish_cy - (dish_r * 0.35) * math.sin(ang) * 0.7 + 0.04
        hzi = dish_cz + (dish_r * 0.35) * math.sin(ang) * 0.7 - 0.04
        hex_inner.append((hxi, hyi, hzi))
        
    dish_center = (dish_cx, dish_cy + 0.06, dish_cz - 0.05)
    for i in range(6):
        next_i = (i + 1) % 6
        dish_tris.append(([dish_center, hex_pts[i], hex_pts[next_i]], (0, 0.7, -0.7)))
        
    # Central subreflector horn
    hq, ht = make_cylinder('Y', dish_cx, dish_cy + 0.12, dish_cz - 0.04, r=0.03, length=0.12, segs=6)
    dish_quads += hq
    dish_tris += ht
    parts.append(("HighGainAntenna", "GoldFoilMaterial", dish_quads, dish_tris))

    # 8. ASSEMBLE VERTICES, NORMALS, AND FACES
    all_v = []
    all_vn = []
    f_lines = []

    v_map = {}
    vn_map = {}

    def get_v_idx(pt):
        pt_key = (round(pt[0], 4), round(pt[1], 4), round(pt[2], 4))
        if pt_key not in v_map:
            all_v.append(pt_key)
            v_map[pt_key] = len(all_v)
        return v_map[pt_key]

    def get_vn_idx(norm):
        norm_key = (round(norm[0], 3), round(norm[1], 3), round(norm[2], 3))
        if norm_key not in vn_map:
            all_vn.append(norm_key)
            vn_map[norm_key] = len(all_vn)
        return vn_map[norm_key]

    mesh_json_mats = {}

    for item in parts:
        gname = item[0]
        matname = item[1]
        quads = item[2]
        tris = item[3] if len(item) > 3 else []

        if matname not in mesh_json_mats:
            mesh_json_mats[matname] = []

        f_lines.append(f"g {gname}")
        f_lines.append(f"usemtl {matname}")

        for pts, norm in quads:
            vi = [get_v_idx(p) for p in pts]
            ni = get_vn_idx(norm)
            f_lines.append(f"f {vi[0]}//{ni} {vi[1]}//{ni} {vi[2]}//{ni} {vi[3]}//{ni}")
            mesh_json_mats[matname].append([vi[0]-1, vi[1]-1, vi[2]-1, vi[3]-1])

        for pts, norm in tris:
            vi = [get_v_idx(p) for p in pts]
            ni = get_vn_idx(norm)
            f_lines.append(f"f {vi[0]}//{ni} {vi[1]}//{ni} {vi[2]}//{ni}")
            mesh_json_mats[matname].append([vi[0]-1, vi[1]-1, vi[2]-1])

    # Write OBJ
    with open("/Users/kapilash/.gemini/antigravity/scratch/3d-game-assets/mars_rover.obj", "w") as f:
        f.write("# Advanced Mars Research & Exploration Rover 3D Game Object\n")
        f.write("mtllib mars_rover.mtl\n\n")
        for v in all_v:
            f.write(f"v {v[0]:.4f} {v[1]:.4f} {v[2]:.4f}\n")
        f.write("\n")
        for vn in all_vn:
            f.write(f"vn {vn[0]:.4f} {vn[1]:.4f} {vn[2]:.4f}\n")
        f.write("\n")
        for line in f_lines:
            f.write(line + "\n")

    # Export JSON for real-time web viewer
    rover_data = {
        'vertices': all_v,
        'materials': mesh_json_mats
    }
    with open("/Users/kapilash/.gemini/antigravity/scratch/3d-game-assets/rover_mesh.json", "w") as f:
        json.dump(rover_data, f)

    print(f"Generated Mars Rover: {len(all_v)} vertices, {len(all_vn)} normals, {sum(len(f) for f in mesh_json_mats.values())} total faces.")
    for m, fl in mesh_json_mats.items():
        print(f" - {m}: {len(fl)} faces")

generate_mars_rover()
