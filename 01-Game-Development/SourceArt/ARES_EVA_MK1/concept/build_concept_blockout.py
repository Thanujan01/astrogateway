import math
import json
import os

all_verts = []
all_faces = []
groups = {}

def get_v_idx(pt):
    k = (round(pt[0], 4), round(pt[1], 4), round(pt[2], 4))
    if k not in v_map:
        all_verts.append(k)
        v_map[k] = len(all_verts)
    return v_map[k]

v_map = {}

def add_tri(group, mat, p1, p2, p3):
    v1 = get_v_idx(p1)
    v2 = get_v_idx(p2)
    v3 = get_v_idx(p3)
    if group not in groups:
        groups[group] = {'mat': mat, 'faces': []}
    groups[group]['faces'].append([v1-1, v2-1, v3-1])

def add_quad(group, mat, p1, p2, p3, p4):
    add_tri(group, mat, p1, p2, p3)
    add_tri(group, mat, p1, p3, p4)

def add_box(group, mat, x0, x1, y0, y1, z0, z1):
    add_quad(group, mat, (x0, y1, z0), (x1, y1, z0), (x1, y1, z1), (x0, y1, z1))
    add_quad(group, mat, (x1, y0, z0), (x0, y0, z0), (x0, y0, z1), (x1, y0, z1))
    add_quad(group, mat, (x0, y0, z0), (x0, y1, z0), (x0, y1, z1), (x0, y0, z1))
    add_quad(group, mat, (x1, y1, z0), (x1, y0, z0), (x1, y0, z1), (x1, y1, z1))
    add_quad(group, mat, (x0, y0, z1), (x1, y0, z1), (x1, y1, z1), (x0, y1, z1))
    add_quad(group, mat, (x0, y1, z0), (x1, y1, z0), (x1, y0, z0), (x0, y0, z0))

def add_cylinder(group, mat, axis, c1, c2, c3, r, length, segs=12):
    step = 2 * math.pi / segs
    pts_a, pts_b = [], []
    for i in range(segs):
        ang = i * step
        ca = r * math.cos(ang)
        sa = r * math.sin(ang)
        if axis == 'X':
            pts_a.append((c1 - length/2, c2 + ca, c3 + sa))
            pts_b.append((c1 + length/2, c2 + ca, c3 + sa))
        elif axis == 'Y':
            pts_a.append((c1 + ca, c2 - length/2, c3 + sa))
            pts_b.append((c1 + ca, c2 + length/2, c3 + sa))
        else:
            pts_a.append((c1 + ca, c2 + sa, c3 - length/2))
            pts_b.append((c1 + ca, c2 + sa, c3 + length/2))
            
    for i in range(segs):
        ni = (i + 1) % segs
        add_quad(group, mat, pts_a[i], pts_b[i], pts_b[ni], pts_a[ni])
    # caps
    if axis == 'X':
        ca, cb = (c1 - length/2, c2, c3), (c1 + length/2, c2, c3)
    elif axis == 'Y':
        ca, cb = (c1, c2 - length/2, c3), (c1, c2 + length/2, c3)
    else:
        ca, cb = (c1, c2, c3 - length/2), (c1, c2, c3 + length/2)
    for i in range(segs):
        ni = (i + 1) % segs
        add_tri(group, mat, ca, pts_a[ni], pts_a[i])
        add_tri(group, mat, cb, pts_b[i], pts_b[ni])

def add_sphere(group, mat, cx, cy, cz, r, segs=12, rings=8):
    for i in range(rings):
        phi1 = math.pi * i / rings
        phi2 = math.pi * (i + 1) / rings
        for j in range(segs):
            theta1 = 2 * math.pi * j / segs
            theta2 = 2 * math.pi * (j + 1) / segs
            
            p1 = (cx + r * math.sin(phi1) * math.cos(theta1), cy + r * math.sin(phi1) * math.sin(theta1), cz + r * math.cos(phi1))
            p2 = (cx + r * math.sin(phi1) * math.cos(theta2), cy + r * math.sin(phi1) * math.sin(theta2), cz + r * math.cos(phi1))
            p3 = (cx + r * math.sin(phi2) * math.cos(theta2), cy + r * math.sin(phi2) * math.sin(theta2), cz + r * math.cos(phi2))
            p4 = (cx + r * math.sin(phi2) * math.cos(theta1), cy + r * math.sin(phi2) * math.sin(theta1), cz + r * math.cos(phi2))
            
            if i == 0:
                add_tri(group, mat, p1, p3, p4)
            elif i == rings - 1:
                add_tri(group, mat, p1, p2, p3)
            else:
                add_quad(group, mat, p1, p2, p3, p4)

print("Building ARES EVA MK-I Concept Blockout...")

# 1. HELMET & VISOR (Z: 1.55 to 1.84m)
# Outer Helmet Shell (White Ceramic)
add_sphere("Helmet_Shell", "Mat_CeramicWhite", 0, 0.02, 1.68, 0.17, segs=16, rings=10)
# Gold-Vapor Bubble Visor (Front face)
add_sphere("Helmet_Visor", "Mat_GoldVisor", 0, 0.08, 1.68, 0.145, segs=16, rings=8)
# Neck Disconnect Bearing Ring
add_cylinder("Helmet_NeckRing", "Mat_Titanium", 'Z', 0, 0.02, 1.52, r=0.155, length=0.04, segs=14)
# Side Comm Pods (Left & Right)
add_box("Helmet_CommL", "Mat_Graphite", -0.19, -0.16, -0.02, 0.06, 1.64, 1.74)
add_box("Helmet_CommR", "Mat_Graphite",  0.16,  0.19, -0.02, 0.06, 1.64, 1.74)
# Dual LED Headlamps
add_cylinder("Helmet_LightL", "Mat_CyanEnergy", 'Y', -0.12, 0.14, 1.78, r=0.025, length=0.04, segs=8)
add_cylinder("Helmet_LightR", "Mat_CyanEnergy", 'Y',  0.12, 0.14, 1.78, r=0.025, length=0.04, segs=8)

# 2. TORSO & CHEST CONTROL MODULE (Z: 1.05 to 1.50m)
# Main Upper Torso (Orthofabric White)
add_box("Torso_Upper", "Mat_CeramicWhite", -0.26, 0.26, -0.16, 0.14, 1.25, 1.50)
# Lower Torso & Waist (Graphite Reinforced)
add_box("Torso_Lower", "Mat_Graphite", -0.22, 0.22, -0.14, 0.12, 1.02, 1.25)
# Display & Control Module (DCM on chest)
add_box("Torso_DCM", "Mat_Titanium", -0.12, 0.12, 0.14, 0.19, 1.26, 1.44)
add_box("Torso_DCM_Screen", "Mat_CyanEnergy", -0.08, 0.08, 0.185, 0.195, 1.34, 1.42)
# Orange Mission Identification Strakes
add_box("Torso_StrakeL", "Mat_SafetyOrange", -0.24, -0.16, 0.12, 0.145, 1.26, 1.48)
add_box("Torso_StrakeR", "Mat_SafetyOrange",  0.16,  0.24, 0.12, 0.145, 1.26, 1.48)

# 3. PLSS LIFE SUPPORT BACKPACK (Z: 1.08 to 1.60m, Y: -0.16 to -0.38m)
# Main Modular Enclosure
add_box("PLSS_MainHousing", "Mat_CeramicWhite", -0.22, 0.22, -0.36, -0.16, 1.10, 1.56)
# Secondary Avionics Cap (Top)
add_box("PLSS_TopCap", "Mat_Graphite", -0.20, 0.20, -0.34, -0.18, 1.56, 1.64)
# Dual O2 Storage Vessels (Protected side canisters)
add_cylinder("PLSS_O2TankL", "Mat_Titanium", 'Z', -0.14, -0.37, 1.33, r=0.055, length=0.38, segs=10)
add_cylinder("PLSS_O2TankR", "Mat_Titanium", 'Z',  0.14, -0.37, 1.33, r=0.055, length=0.38, segs=10)
# Status Indicator LED Bar
add_box("PLSS_StatusLED", "Mat_CyanEnergy", -0.12, 0.12, -0.365, -0.355, 1.50, 1.53)
# Umbilical Life-Support Hoses (Backpack to Chest)
add_cylinder("PLSS_HoseL", "Mat_Graphite", 'X', -0.25, 0.00, 1.28, r=0.025, length=0.14, segs=8)
add_cylinder("PLSS_HoseR", "Mat_Graphite", 'X',  0.25, 0.00, 1.28, r=0.025, length=0.14, segs=8)

# 4. SHOULDERS & ARMS (Left & Right)
for sx, side in [(-1, "L"), (1, "R")]:
    # Shoulder Armor Pauldron
    sh_x = sx * 0.30
    add_sphere(f"Arm_{side}_Shoulder", "Mat_CeramicWhite", sh_x, -0.01, 1.42, 0.09, segs=10, rings=6)
    # Upper Arm Biceps
    add_cylinder(f"Arm_{side}_Upper", "Mat_CeramicWhite", 'Z', sh_x, -0.01, 1.26, r=0.07, length=0.22, segs=10)
    # Elbow Articulated Bellows & Protective Cap
    add_cylinder(f"Arm_{side}_ElbowBellows", "Mat_Graphite", 'Z', sh_x, -0.01, 1.13, r=0.065, length=0.08, segs=10)
    add_box(f"Arm_{side}_ElbowCap", "Mat_Titanium", sh_x - 0.04, sh_x + 0.04, -0.08, -0.04, 1.10, 1.16)
    # Forearm
    add_cylinder(f"Arm_{side}_Forearm", "Mat_CeramicWhite", 'Z', sh_x, -0.01, 0.98, r=0.062, length=0.22, segs=10)
    # Wrist Disconnect Ring & Glove
    add_cylinder(f"Arm_{side}_WristRing", "Mat_Titanium", 'Z', sh_x, -0.01, 0.85, r=0.068, length=0.03, segs=10)
    add_box(f"Arm_{side}_GloveHand", "Mat_Graphite", sh_x - 0.04, sh_x + 0.04, -0.03, 0.05, 0.72, 0.84)
    # Orange Finger Grips
    add_box(f"Arm_{side}_Fingers", "Mat_SafetyOrange", sh_x - 0.035, sh_x + 0.035, -0.02, 0.04, 0.64, 0.72)

# 5. PELVIS, LEGS & BOOTS (Left & Right)
# Pelvis Brief Joint
add_box("Pelvis_Brief", "Mat_Graphite", -0.21, 0.21, -0.13, 0.11, 0.86, 1.02)

for sx, side in [(-1, "L"), (1, "R")]:
    leg_x = sx * 0.14
    # Hip Rotating Bearing
    add_cylinder(f"Leg_{side}_HipRing", "Mat_Titanium", 'Z', leg_x, 0.00, 0.86, r=0.09, length=0.04, segs=10)
    # Thigh
    add_cylinder(f"Leg_{side}_Thigh", "Mat_CeramicWhite", 'Z', leg_x, 0.00, 0.66, r=0.085, length=0.36, segs=10)
    # Knee Articulated Bellows & Heavy Protective Pad
    add_cylinder(f"Leg_{side}_KneeBellows", "Mat_Graphite", 'Z', leg_x, 0.00, 0.46, r=0.08, length=0.08, segs=10)
    add_box(f"Leg_{side}_KneePad", "Mat_Titanium", leg_x - 0.05, leg_x + 0.05, 0.06, 0.10, 0.42, 0.50)
    add_box(f"Leg_{side}_KneeOrange", "Mat_SafetyOrange", leg_x - 0.04, leg_x + 0.04, 0.095, 0.105, 0.43, 0.49)
    # Shin / Calf
    add_cylinder(f"Leg_{side}_Shin", "Mat_CeramicWhite", 'Z', leg_x, 0.00, 0.26, r=0.078, length=0.32, segs=10)
    # Exploration Boots (Ankle, Foot, Vulcanized Cleat Sole)
    add_cylinder(f"Boot_{side}_Ankle", "Mat_Titanium", 'Z', leg_x, 0.00, 0.12, r=0.082, length=0.05, segs=10)
    add_box(f"Boot_{side}_Upper", "Mat_Graphite", leg_x - 0.075, leg_x + 0.075, -0.10, 0.16, 0.04, 0.12)
    add_box(f"Boot_{side}_ToeCap", "Mat_Titanium", leg_x - 0.07, leg_x + 0.07, 0.08, 0.17, 0.04, 0.10)
    add_box(f"Boot_{side}_LugSole", "Mat_Graphite", leg_x - 0.08, leg_x + 0.08, -0.12, 0.18, 0.00, 0.04)

total_tris = sum(len(g['faces']) for g in groups.values())
print(f"Concept Blockout Generated: {len(all_verts)} vertices, {total_tris} triangles across {len(groups)} components.")

# Export JSON for real-time concept viewer
concept_export = {
    'vertices': all_verts,
    'subsystems': groups,
    'stats': {
        'total_vertices': len(all_verts),
        'total_triangles': total_tris,
        'total_components': len(groups),
        'proportions': {
            'height_m': 1.84,
            'shoulder_width_m': 0.60,
            'chest_depth_m': 0.35,
            'backpack_depth_m': 0.22
        }
    }
}

out_json = "/Users/kapilash/.gemini/antigravity/scratch/ARES_EVA_MK1/concept/concept_blockout.json"
with open(out_json, "w") as f:
    json.dump(concept_export, f)

print(f"Saved concept JSON: {out_json}")
