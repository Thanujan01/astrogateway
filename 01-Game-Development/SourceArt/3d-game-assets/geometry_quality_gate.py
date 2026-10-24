import math
import os

obj_path = "/Users/kapilash/.gemini/antigravity/scratch/3d-game-assets/ares_vi_aaa_master.obj"

verts = []
normals = []
faces = []
groups = {}
current_g = "default"

with open(obj_path, 'r') as f:
    for line in f:
        line = line.strip()
        if line.startswith('v '):
            parts = [float(x) for x in line.split()[1:]]
            verts.append(parts)
        elif line.startswith('vn '):
            parts = [float(x) for x in line.split()[1:]]
            normals.append(parts)
        elif line.startswith('g '):
            current_g = line.split()[1]
            if current_g not in groups:
                groups[current_g] = []
        elif line.startswith('f '):
            parts = line.split()[1:]
            indices = [int(p.split('/')[0]) - 1 for p in parts]
            faces.append((current_g, indices))
            if current_g not in groups:
                groups[current_g] = []
            groups[current_g].append(indices)

print(f"=== STAGE 2: GEOMETRY QUALITY GATE REPORT ===")
print(f"Total Vertices: {len(verts):,}")
print(f"Total Triangles: {len(faces):,}")
print(f"Total Mechanical Subsystems: {len(groups):,}")

# 1. Bounding Box & Dimensions Check
min_x = min(v[0] for v in verts)
max_x = max(v[0] for v in verts)
min_y = min(v[1] for v in verts)
max_y = max(v[1] for v in verts)
min_z = min(v[2] for v in verts)
max_z = max(v[2] for v in verts)

length_y = max_y - min_y
width_x  = max_x - min_x
height_z = max_z - min_z

print(f"\n1. Scale & Metric Bounds:")
print(f"  Length (Y): {length_y:.3f} m  (Min: {min_y:.3f}, Max: {max_y:.3f})")
print(f"  Width  (X): {width_x:.3f} m  (Min: {min_x:.3f}, Max: {max_x:.3f})")
print(f"  Height (Z): {height_z:.3f} m  (Min: {min_z:.3f}, Max: {max_z:.3f})")
ground_clearance = min_z
print(f"  Lowest ground point (tread contact): {min_z:.3f} m")

# 2. Degenerate Triangles Check
zero_area_count = 0
for g, f_idx in faces:
    p0 = verts[f_idx[0]]
    p1 = verts[f_idx[1]]
    p2 = verts[f_idx[2]]
    # Cross product area
    ax, ay, az = p1[0]-p0[0], p1[1]-p0[1], p1[2]-p0[2]
    bx, by, bz = p2[0]-p0[0], p2[1]-p0[1], p2[2]-p0[2]
    cross_x = ay*bz - az*by
    cross_y = az*bx - ax*bz
    cross_z = ax*by - ay*bx
    area = 0.5 * math.sqrt(cross_x**2 + cross_y**2 + cross_z**2)
    if area < 1e-7:
        zero_area_count += 1

print(f"\n2. Mesh Topology & Integrity:")
print(f"  Zero-area / degenerate triangles: {zero_area_count}")

# 3. Wheel Symmetries & Alignment
wheel_prefixes = ['Wheel_FL', 'Wheel_ML', 'Wheel_RL', 'Wheel_FR', 'Wheel_MR', 'Wheel_RR']
print(f"\n3. Wheel Subsystems Verification:")
for wp in wheel_prefixes:
    wheel_groups = [g for g in groups.keys() if g.startswith(wp)]
    w_verts_idx = set()
    for wg in wheel_groups:
        for f_idx in groups[wg]:
            w_verts_idx.update(f_idx)
    sub_verts = [verts[i] for i in w_verts_idx]
    if sub_verts:
        c_x = sum(v[0] for v in sub_verts) / len(sub_verts)
        c_y = sum(v[1] for v in sub_verts) / len(sub_verts)
        c_z = sum(v[2] for v in sub_verts) / len(sub_verts)
        print(f"  {wp}: Centroid = ({c_x:.2f}, {c_y:.2f}, {c_z:.2f}) m across {len(wheel_groups)} sub-parts ({len(w_verts_idx)} verts)")

# 4. Anti-Gravity Thruster Modules
hover_prefixes = ['Hover_FL', 'Hover_FR', 'Hover_RL', 'Hover_RR', 'AuxThruster_ML', 'AuxThruster_MR']
print(f"\n4. Anti-Gravity Thrusters Verification:")
for hp in hover_prefixes:
    h_groups = [g for g in groups.keys() if g.startswith(hp)]
    h_verts_idx = set()
    for hg in h_groups:
        for f_idx in groups[hg]:
            h_verts_idx.update(f_idx)
    sub_verts = [verts[i] for i in h_verts_idx]
    if sub_verts:
        c_x = sum(v[0] for v in sub_verts) / len(sub_verts)
        c_y = sum(v[1] for v in sub_verts) / len(sub_verts)
        c_z = sum(v[2] for v in sub_verts) / len(sub_verts)
        print(f"  {hp}: Centroid = ({c_x:.2f}, {c_y:.2f}, {c_z:.2f}) m across {len(h_groups)} sub-parts")

