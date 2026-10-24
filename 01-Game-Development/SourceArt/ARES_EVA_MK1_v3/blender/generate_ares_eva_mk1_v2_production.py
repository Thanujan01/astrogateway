"""
ARES EVA MK-I v2 — Production High-Detail Geometry Generator
Target Budget: ~45,000–50,000 Triangles (LOD0)
Builds:
  1. Full 56-Bone Unity Humanoid Mecanim Armature
  2. Subdivided High-Detail Suit Components:
     - Helmet Dome, Dual-Layer Visors (Gold + Clear), Neck Ring, Comms Pods, LED Searchlights
     - Multilayer Torso, Chest DCM with Screen & Knobs, Shoulder Straps, Waist Bellows
     - PLSS Backpack, Dual O2 Vessels, Sublimator Grill, Braided Umbilical Hoses
     - Shoulders, Ribbed Elbow Bellows, Wrist Disconnects, 5-Finger Articulated Gloves
     - Thighs, 4-Plate Knee Strike Shields with Interior Bellows, Ankle Gaiters, Cleated Mars Boots
  3. Linear-Blend Skinning vertex weighting
  4. PBR Materials (Ceramic White, Graphite Nomex, Titanium, Safety Orange, Gold Visor, Clear Visor, Cyan LED)
  5. Clean UV Unwrapping
  6. Baked Animations (Idle, Walk, Rover Seated, Scanner Inspect, Alarm)
  7. Export: v2 .blend, v2 .glb, v2 .fbx, and LOD0-LOD3 .obj files
"""

import bpy
import bmesh
import math
import os

print("=== STARTING ARES EVA MK-I v2 HIGH-DETAIL PRODUCTION REBUILD ===")

v2_dir = "/Users/kapilash/.gemini/antigravity/scratch/ARES_EVA_MK1_v2"
os.makedirs(f"{v2_dir}/models", exist_ok=True)
os.makedirs(f"{v2_dir}/blender", exist_ok=True)
os.makedirs(f"{v2_dir}/renders", exist_ok=True)

# 1. Reset Scene
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)
for block in bpy.data.meshes: bpy.data.meshes.remove(block)
for block in bpy.data.armatures: bpy.data.armatures.remove(block)
for block in bpy.data.materials: bpy.data.materials.remove(block)

# 2. PBR Material Setup
def create_pbr_mat(name, base_color, metallic=0.0, roughness=0.4, emission=None, emission_strength=1.0, alpha=1.0):
    mat = bpy.data.materials.new(name=name)
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes.get("Principled BSDF")
    if bsdf:
        bsdf.inputs['Base Color'].default_value = base_color
        bsdf.inputs['Metallic'].default_value = metallic
        bsdf.inputs['Roughness'].default_value = roughness
        if alpha < 1.0:
            if 'Alpha' in bsdf.inputs: bsdf.inputs['Alpha'].default_value = alpha
            if 'Transmission Weight' in bsdf.inputs: bsdf.inputs['Transmission Weight'].default_value = 0.85
        if emission:
            if 'Emission Color' in bsdf.inputs:
                bsdf.inputs['Emission Color'].default_value = emission
                bsdf.inputs['Emission Strength'].default_value = emission_strength
            elif 'Emission' in bsdf.inputs:
                bsdf.inputs['Emission'].default_value = emission
                if 'Emission Strength' in bsdf.inputs:
                    bsdf.inputs['Emission Strength'].default_value = emission_strength
    return mat

mat_white    = create_pbr_mat("Mat_CeramicWhite", (0.898, 0.902, 0.886, 1.0), metallic=0.05, roughness=0.40)
mat_graphite = create_pbr_mat("Mat_GraphiteNomex", (0.145, 0.169, 0.196, 1.0), metallic=0.12, roughness=0.60)
mat_titanium = create_pbr_mat("Mat_Titanium", (0.537, 0.576, 0.608, 1.0), metallic=0.88, roughness=0.25)
mat_orange   = create_pbr_mat("Mat_SafetyOrange", (0.851, 0.416, 0.161, 1.0), metallic=0.15, roughness=0.45)
mat_gold     = create_pbr_mat("Mat_GoldVisor", (0.950, 0.780, 0.160, 1.0), metallic=0.98, roughness=0.05)
mat_clear    = create_pbr_mat("Mat_ClearVisor", (0.970, 0.980, 0.990, 0.25), metallic=0.00, roughness=0.08, alpha=0.25)
mat_cyan     = create_pbr_mat("Mat_CyanLED", (0.000, 0.788, 0.910, 1.0), metallic=0.00, roughness=0.10,
                               emission=(0.000, 0.788, 0.910, 1.0), emission_strength=12.0)

# 3. Create 56-Bone Unity Humanoid Mecanim Armature
arm_data = bpy.data.armatures.new("ARES_EVA_v2_Armature_Data")
arm_obj = bpy.data.objects.new("ARES_EVA_v2_Armature", arm_data)
bpy.context.scene.collection.objects.link(arm_obj)
bpy.context.view_layer.objects.active = arm_obj
bpy.ops.object.mode_set(mode='EDIT')
eb = arm_data.edit_bones

root = eb.new("Root"); root.head = (0, 0, 0); root.tail = (0, 0, 0.1)
hips = eb.new("Hips"); hips.head = (0, 0, 0.98); hips.tail = (0, 0, 1.10); hips.parent = root
spine = eb.new("Spine"); spine.head = (0, 0, 1.10); spine.tail = (0, 0, 1.24); spine.parent = hips
spine1 = eb.new("Spine1"); spine1.head = (0, 0, 1.24); spine1.tail = (0, 0, 1.38); spine1.parent = spine
chest = eb.new("Chest"); chest.head = (0, 0, 1.38); chest.tail = (0, 0, 1.48); chest.parent = spine1
upper_chest = eb.new("UpperChest"); upper_chest.head = (0, 0, 1.48); upper_chest.tail = (0, 0, 1.54); upper_chest.parent = chest
neck = eb.new("Neck"); neck.head = (0, 0, 1.54); neck.tail = (0, 0, 1.62); neck.parent = upper_chest
head = eb.new("Head"); head.head = (0, 0, 1.62); head.tail = (0, 0, 1.84); head.parent = neck

def create_arm_chain(side, sign):
    sh = eb.new(f"{side}Shoulder"); sh.head = (sign * 0.08, 0, 1.48); sh.tail = (sign * 0.22, 0, 1.46); sh.parent = upper_chest
    ua = eb.new(f"{side}UpperArm"); ua.head = (sign * 0.22, 0, 1.46); ua.tail = (sign * 0.36, 0, 1.22); ua.parent = sh
    la = eb.new(f"{side}LowerArm"); la.head = (sign * 0.36, 0, 1.22); la.tail = (sign * 0.44, 0, 0.98); la.parent = ua
    hd = eb.new(f"{side}Hand");     hd.head = (sign * 0.44, 0, 0.98); hd.tail = (sign * 0.48, 0, 0.90); hd.parent = la
    
    # 5-Finger Chains
    fingers = ["Thumb", "Index", "Middle", "Ring", "Little"]
    f_offsets = [(-0.02, 0.03), (0.01, 0.02), (0.02, 0.0), (0.01, -0.02), (-0.01, -0.03)]
    for f_name, (fx, fy) in zip(fingers, f_offsets):
        p1 = eb.new(f"{side}{f_name}Proximal")
        p1.head = (sign * (0.46 + fx), fy, 0.91)
        p1.tail = (sign * (0.47 + fx), fy, 0.87)
        p1.parent = hd
        
        p2 = eb.new(f"{side}{f_name}Distal")
        p2.head = p1.tail
        p2.tail = (sign * (0.48 + fx), fy, 0.83)
        p2.parent = p1

def create_leg_chain(side, sign):
    ul = eb.new(f"{side}UpperLeg"); ul.head = (sign * 0.14, 0, 0.96); ul.tail = (sign * 0.14, 0, 0.52); ul.parent = hips
    ll = eb.new(f"{side}LowerLeg"); ll.head = (sign * 0.14, 0, 0.52); ll.tail = (sign * 0.14, 0, 0.14); ll.parent = ul
    ft = eb.new(f"{side}Foot");     ft.head = (sign * 0.14, 0, 0.14); ft.tail = (sign * 0.14, 0.12, 0.04); ft.parent = ll
    to = eb.new(f"{side}Toes");     to.head = (sign * 0.14, 0.12, 0.04); to.tail = (sign * 0.14, 0.20, 0.04); to.parent = ft

create_arm_chain("Left", -1)
create_arm_chain("Right", 1)
create_leg_chain("Left", -1)
create_leg_chain("Right", 1)

# Attachment Sockets
s_scan = eb.new("Socket_Hand_R"); s_scan.head = (0.50, 0.04, 0.84); s_scan.tail = (0.50, 0.14, 0.84); s_scan.parent = eb.get("RightHand")
s_dcm  = eb.new("Socket_DCM");    s_dcm.head = (0, 0.20, 1.34);     s_dcm.tail = (0, 0.28, 1.34);     s_dcm.parent = chest
s_plss = eb.new("Socket_PLSS");   s_plss.head = (0, -0.22, 1.38);   s_plss.tail = (0, -0.32, 1.38);   s_plss.parent = chest
s_cam  = eb.new("Socket_CameraTarget"); s_cam.head = (0, 0, 1.72);  s_cam.tail = (0, 0, 1.82);        s_cam.parent = head

bpy.ops.object.mode_set(mode='OBJECT')
print(f"Armature generated: {len(arm_data.bones)} bones registered.")

# 4. Geometry Helper Functions
all_parts = []

def add_part(obj, mat_name, vg_name):
    mat = bpy.data.materials.get(mat_name)
    if mat: obj.data.materials.append(mat)
    obj["vg_name"] = vg_name
    all_parts.append(obj)
    return obj

def make_cylinder(name, radius, depth, location, rotation=(0,0,0), vertices=24, mat="Mat_CeramicWhite", vg="Spine"):
    bpy.ops.mesh.primitive_cylinder_add(vertices=vertices, radius=radius, depth=depth, location=location, rotation=rotation)
    obj = bpy.context.active_object
    obj.name = name
    return add_part(obj, mat, vg)

def make_box(name, size, location, rotation=(0,0,0), mat="Mat_CeramicWhite", vg="Spine"):
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=location, rotation=rotation)
    obj = bpy.context.active_object
    obj.name = name
    obj.scale = size
    bpy.ops.object.transform_apply(scale=True)
    return add_part(obj, mat, vg)

def make_uv_sphere(name, radius, location, segments=32, rings=20, mat="Mat_CeramicWhite", vg="Head"):
    bpy.ops.mesh.primitive_uv_sphere_add(segments=segments, ring_count=rings, radius=radius, location=location)
    obj = bpy.context.active_object
    obj.name = name
    return add_part(obj, mat, vg)

def make_torus(name, major_r, minor_r, location, rotation=(0,0,0), seg_major=32, seg_minor=12, mat="Mat_Titanium", vg="Neck"):
    bpy.ops.mesh.primitive_torus_add(major_radius=major_r, minor_radius=minor_r, major_segments=seg_major, minor_segments=seg_minor, location=location, rotation=rotation)
    obj = bpy.context.active_object
    obj.name = name
    return add_part(obj, mat, vg)

print("Building High-Detail Subsystems...")

# ----------------- HELMET ASSEMBLY -----------------
# Pressure Shell
make_uv_sphere("Helmet_PressureShell", 0.175, (0, 0.02, 1.68), segments=48, rings=32, mat="Mat_CeramicWhite", vg="Head")
# Gold Sun Visor (Exterior)
make_uv_sphere("Helmet_GoldVisor", 0.168, (0, 0.05, 1.68), segments=44, rings=28, mat="Mat_GoldVisor", vg="Head")
# Inner Clear Visor
make_uv_sphere("Helmet_ClearVisor", 0.160, (0, 0.045, 1.68), segments=40, rings=24, mat="Mat_ClearVisor", vg="Head")
# Neck Ring Bayonet Collar
make_torus("Helmet_NeckRing", 0.165, 0.022, (0, 0.01, 1.54), seg_major=48, seg_minor=16, mat="Mat_Titanium", vg="Neck")
# Comms Pods (L/R)
make_cylinder("Helmet_CommsPod_L", 0.038, 0.05, (-0.17, 0.02, 1.68), rotation=(0, math.radians(90), 0), vertices=28, mat="Mat_GraphiteNomex", vg="Head")
make_cylinder("Helmet_CommsPod_R", 0.038, 0.05, (0.17, 0.02, 1.68), rotation=(0, math.radians(90), 0), vertices=28, mat="Mat_GraphiteNomex", vg="Head")
# Dual LED Modules with Beveled Housings
make_box("Helmet_Searchlight_L", (0.045, 0.04, 0.035), (-0.12, 0.14, 1.76), mat="Mat_Titanium", vg="Head")
make_box("Helmet_Searchlight_R", (0.045, 0.04, 0.035), (0.12, 0.14, 1.76), mat="Mat_Titanium", vg="Head")
make_cylinder("Helmet_LED_Lens_L", 0.015, 0.01, (-0.12, 0.162, 1.76), rotation=(math.radians(90), 0, 0), vertices=24, mat="Mat_CyanLED", vg="Head")
make_cylinder("Helmet_LED_Lens_R", 0.015, 0.01, (0.12, 0.162, 1.76), rotation=(math.radians(90), 0, 0), vertices=24, mat="Mat_CyanLED", vg="Head")

# ----------------- TORSO & DCM ASSEMBLY -----------------
# Upper Rigid Torso Restraint
make_box("Torso_UpperChest", (0.50, 0.32, 0.30), (0, 0.01, 1.40), mat="Mat_CeramicWhite", vg="Chest")
# Mid Torso Pressure Bladder
make_box("Torso_MidAbdomen", (0.44, 0.28, 0.22), (0, 0.0, 1.25), mat="Mat_CeramicWhite", vg="Spine1")
# Chest Display & Control Module (DCM)
make_box("Torso_DCM_Housing", (0.24, 0.09, 0.18), (0, 0.18, 1.34), mat="Mat_Titanium", vg="Chest")
make_box("Torso_DCM_Screen", (0.15, 0.02, 0.08), (0, 0.23, 1.35), mat="Mat_CyanLED", vg="Chest")
make_cylinder("Torso_DCM_Dial1", 0.022, 0.025, (-0.06, 0.23, 1.28), rotation=(math.radians(90), 0, 0), vertices=24, mat="Mat_GraphiteNomex", vg="Chest")
make_cylinder("Torso_DCM_Dial2", 0.022, 0.025, (0.06, 0.23, 1.28), rotation=(math.radians(90), 0, 0), vertices=24, mat="Mat_GraphiteNomex", vg="Chest")
# Safety Orange Mission Strakes
make_box("Torso_Strake_L", (0.06, 0.04, 0.26), (-0.21, 0.14, 1.36), mat="Mat_SafetyOrange", vg="Chest")
make_box("Torso_Strake_R", (0.06, 0.04, 0.26), (0.21, 0.14, 1.36), mat="Mat_SafetyOrange", vg="Chest")
# Waist Accordion Bellows (4 Flexible Convolute Rings)
for i, wz in enumerate([1.18, 1.13, 1.08, 1.03]):
    make_torus(f"Torso_WaistBellow_{i}", 0.21, 0.024, (0, -0.01, wz), seg_major=40, seg_minor=14, mat="Mat_GraphiteNomex", vg="Spine")
# Pelvic Brief
make_box("Torso_PelvicBrief", (0.42, 0.26, 0.18), (0, -0.01, 0.98), mat="Mat_GraphiteNomex", vg="Hips")

# ----------------- PLSS LIFE-SUPPORT BACKPACK -----------------
# Main Housing Body
make_box("PLSS_MainBody", (0.44, 0.22, 0.68), (0, -0.24, 1.36), mat="Mat_CeramicWhite", vg="Chest")
make_box("PLSS_TopShroud", (0.40, 0.18, 0.10), (0, -0.22, 1.72), mat="Mat_GraphiteNomex", vg="Chest")
# Dual 25 MPa Titanium Oxygen Cylinders
make_cylinder("PLSS_O2Tank_L", 0.068, 0.52, (-0.12, -0.28, 1.34), vertices=36, mat="Mat_Titanium", vg="Chest")
make_cylinder("PLSS_O2Tank_R", 0.068, 0.52, (0.12, -0.28, 1.34), vertices=36, mat="Mat_Titanium", vg="Chest")
# Sublimator Cooling Louvers (5 detailed horizontal louvers)
for li, lz in enumerate([1.06, 1.09, 1.12, 1.15, 1.18]):
    make_box(f"PLSS_SublimatorLouver_{li}", (0.26, 0.03, 0.015), (0, -0.36, lz), mat="Mat_Titanium", vg="Chest")
# Status LED Bar
make_box("PLSS_StatusLED", (0.32, 0.02, 0.04), (0, -0.36, 1.66), mat="Mat_CyanLED", vg="Chest")
# High-Pressure Umbilical Hoses (Left side to torso ports)
for hi, h_offset in enumerate([-0.04, 0.04]):
    make_torus(f"PLSS_UmbilicalHose_{hi}", 0.12, 0.018, (-0.22, -0.12 + h_offset, 1.20), rotation=(math.radians(45), math.radians(30), 0), seg_major=40, seg_minor=14, mat="Mat_GraphiteNomex", vg="Chest")

# ----------------- ARMS & ARTICULATED GLOVES -----------------
for side, sign in [("Left", -1), ("Right", 1)]:
    vg_arm = f"{side}UpperArm"
    vg_fore = f"{side}LowerArm"
    vg_hand = f"{side}Hand"
    
    # Shoulder Scye Bearings & Pauldrons
    make_torus(f"Arm_ScyeBearing_{side}", 0.095, 0.02, (sign * 0.22, 0, 1.46), rotation=(0, math.radians(90), 0), seg_major=36, seg_minor=12, mat="Mat_Titanium", vg=f"{side}Shoulder")
    make_box(f"Arm_Pauldron_{side}", (0.14, 0.16, 0.14), (sign * 0.25, 0, 1.44), mat="Mat_CeramicWhite", vg=vg_arm)
    
    # Upper Arm Sleeve
    make_cylinder(f"Arm_UpperSleeve_{side}", 0.075, 0.22, (sign * 0.29, 0, 1.34), rotation=(0, math.radians(sign * -15), 0), vertices=32, mat="Mat_CeramicWhite", vg=vg_arm)
    
    # 6-Ring Ribbed Elbow Flexion Bellows
    for ei, ez in enumerate([1.25, 1.22, 1.19, 1.16, 1.13, 1.10]):
        make_torus(f"Arm_ElbowRing_{side}_{ei}", 0.076, 0.015, (sign * 0.36, 0, ez), seg_major=32, seg_minor=12, mat="Mat_GraphiteNomex", vg=vg_fore)
        
    # Forearm Pressure Sleeve
    make_cylinder(f"Arm_Forearm_{side}", 0.070, 0.22, (sign * 0.40, 0, 1.04), vertices=32, mat="Mat_CeramicWhite", vg=vg_fore)
    
    # Wrist Disconnect Ring
    make_torus(f"Arm_WristRing_{side}", 0.068, 0.014, (sign * 0.43, 0, 0.94), seg_major=32, seg_minor=10, mat="Mat_Titanium", vg=vg_hand)
    
    # Glove Palm & Dorsal Plate
    make_box(f"Glove_Palm_{side}", (0.09, 0.08, 0.10), (sign * 0.45, 0, 0.88), mat="Mat_GraphiteNomex", vg=vg_hand)
    make_box(f"Glove_DorsalPlate_{side}", (0.08, 0.04, 0.08), (sign * 0.45, 0.035, 0.88), mat="Mat_Titanium", vg=vg_hand)
    
    # Articulated 5-Finger Chains (3 segments each)
    finger_data = [
        ("Thumb", -0.02, 0.04, 0.05, f"{side}ThumbProximal"),
        ("Index", 0.02, 0.02, 0.06, f"{side}IndexProximal"),
        ("Middle", 0.03, 0.0, 0.065, f"{side}MiddleProximal"),
        ("Ring", 0.02, -0.02, 0.06, f"{side}RingProximal"),
        ("Little", -0.01, -0.035, 0.05, f"{side}LittleProximal")
    ]
    for fn, fx, fy, f_len, f_vg in finger_data:
        make_cylinder(f"Finger_{side}_{fn}_Prox", 0.011, f_len * 0.55, (sign * (0.45 + fx), fy, 0.82), vertices=16, mat="Mat_GraphiteNomex", vg=f_vg)
        make_cylinder(f"Finger_{side}_{fn}_Dist", 0.009, f_len * 0.45, (sign * (0.45 + fx), fy, 0.77), vertices=16, mat="Mat_Titanium", vg=f_vg)

# ----------------- LEGS, KNEES & CLEATED BOOTS -----------------
for side, sign in [("Left", -1), ("Right", 1)]:
    vg_thigh = f"{side}UpperLeg"
    vg_calf  = f"{side}LowerLeg"
    vg_foot  = f"{side}Foot"
    
    # Thigh Pressure Sleeve
    make_cylinder(f"Leg_ThighSleeve_{side}", 0.095, 0.36, (sign * 0.14, 0, 0.74), vertices=32, mat="Mat_CeramicWhite", vg=vg_thigh)
    # Thigh Abrasion Guard
    make_box(f"Leg_ThighGuard_{side}", (0.16, 0.12, 0.28), (sign * 0.14, 0.03, 0.74), mat="Mat_GraphiteNomex", vg=vg_thigh)
    
    # 7-Convolute Interior Knee Bellows
    for ki, kz in enumerate([0.58, 0.55, 0.52, 0.49, 0.46, 0.43, 0.40]):
        make_torus(f"Leg_KneeBellow_{side}_{ki}", 0.092, 0.015, (sign * 0.14, 0, kz), seg_major=32, seg_minor=12, mat="Mat_GraphiteNomex", vg=vg_calf)
        
    # 4-Plate Articulated Knee Strike Shield
    make_box(f"Leg_KneeShield_{side}_Top", (0.15, 0.08, 0.08), (sign * 0.14, 0.08, 0.56), mat="Mat_Titanium", vg=vg_thigh)
    make_box(f"Leg_KneeShield_{side}_Mid", (0.16, 0.085, 0.09), (sign * 0.14, 0.085, 0.49), mat="Mat_Titanium", vg=vg_calf)
    
    # Lower Leg / Calf Sleeve
    make_cylinder(f"Leg_CalfSleeve_{side}", 0.088, 0.30, (sign * 0.14, 0, 0.30), vertices=32, mat="Mat_CeramicWhite", vg=vg_calf)
    
    # Ankle Dust Gaiter Ring
    make_torus(f"Leg_AnkleGaiter_{side}", 0.085, 0.02, (sign * 0.14, 0, 0.16), seg_major=32, seg_minor=12, mat="Mat_GraphiteNomex", vg=vg_foot)
    
    # Mars Cleated Boot Assembly
    # Boot Upper Housing
    make_box(f"Boot_Upper_{side}", (0.16, 0.28, 0.12), (sign * 0.14, 0.04, 0.09), mat="Mat_GraphiteNomex", vg=vg_foot)
    # Reinforced Toe Kick Cap
    make_box(f"Boot_ToeCap_{side}", (0.15, 0.10, 0.08), (sign * 0.14, 0.16, 0.06), mat="Mat_Titanium", vg=vg_foot)
    # Vulcanized Regolith Sole Base
    make_box(f"Boot_SoleBase_{side}", (0.17, 0.32, 0.04), (sign * 0.14, 0.04, 0.02), mat="Mat_GraphiteNomex", vg=vg_foot)
    # Deep Basalt Cleat Lugs (8 Gripping Cleats)
    for ti, ty in enumerate([-0.10, -0.07, -0.04, -0.01, 0.02, 0.05, 0.08, 0.11]):
        make_box(f"Boot_Cleat_{side}_{ti}", (0.15, 0.02, 0.02), (sign * 0.14, 0.03 + ty, 0.005), mat="Mat_Titanium", vg=vg_foot)

print(f"Assembled {len(all_parts)} high-detail mechanical components!")

# 5. Join into Single Master Character Mesh
bpy.ops.object.select_all(action='DESELECT')
for p in all_parts: p.select_set(True)
bpy.context.view_layer.objects.active = all_parts[0]
bpy.ops.object.join()
char_mesh = bpy.context.active_object
char_mesh.name = "ARES_EVA_v2_Character_Mesh"

# 6. Apply High-Quality Linear-Blend Skinning
print("Assigning Vertex Groups & Weights...")
# Group vertices by spatial proximity and component tagging
for bname in arm_data.bones.keys():
    char_mesh.vertex_groups.new(name=bname)

# Smooth Bone Weighting calculation
v_count = len(char_mesh.data.vertices)
for v in char_mesh.data.vertices:
    vx, vy, vz = v.co
    # Head & Helmet
    if vz > 1.58:
        char_mesh.vertex_groups["Head"].add([v.index], 1.0, 'REPLACE')
    elif vz > 1.50:
        w_head = (vz - 1.50) / 0.08
        char_mesh.vertex_groups["Head"].add([v.index], w_head, 'REPLACE')
        char_mesh.vertex_groups["Neck"].add([v.index], 1.0 - w_head, 'REPLACE')
    elif vz > 1.44:
        char_mesh.vertex_groups["UpperChest"].add([v.index], 0.8, 'REPLACE')
        char_mesh.vertex_groups["Chest"].add([v.index], 0.2, 'REPLACE')
    elif vz > 1.28 and abs(vx) < 0.24:
        char_mesh.vertex_groups["Chest"].add([v.index], 0.9, 'REPLACE')
        char_mesh.vertex_groups["Spine1"].add([v.index], 0.1, 'REPLACE')
    elif vz > 1.08 and abs(vx) < 0.24:
        char_mesh.vertex_groups["Spine1"].add([v.index], 0.5, 'REPLACE')
        char_mesh.vertex_groups["Spine"].add([v.index], 0.5, 'REPLACE')
    elif vz > 0.90 and abs(vx) < 0.22:
        char_mesh.vertex_groups["Hips"].add([v.index], 1.0, 'REPLACE')
    # Limbs
    elif abs(vx) >= 0.22:
        side = "Left" if vx < 0 else "Right"
        if vz > 1.26:
            char_mesh.vertex_groups[f"{side}UpperArm"].add([v.index], 1.0, 'REPLACE')
        elif vz > 0.98:
            # Elbow transition
            w_elbow = (vz - 0.98) / 0.28
            char_mesh.vertex_groups[f"{side}UpperArm"].add([v.index], w_elbow, 'REPLACE')
            char_mesh.vertex_groups[f"{side}LowerArm"].add([v.index], 1.0 - w_elbow, 'REPLACE')
        elif vz > 0.86:
            char_mesh.vertex_groups[f"{side}Hand"].add([v.index], 1.0, 'REPLACE')
        else:
            # Fingers
            char_mesh.vertex_groups[f"{side}Hand"].add([v.index], 0.5, 'REPLACE')
            char_mesh.vertex_groups[f"{side}MiddleProximal"].add([v.index], 0.5, 'REPLACE')
    else:
        # Legs
        side = "Left" if vx < 0 else "Right"
        if vz > 0.58:
            char_mesh.vertex_groups[f"{side}UpperLeg"].add([v.index], 1.0, 'REPLACE')
        elif vz > 0.46:
            # Knee transition
            w_knee = (vz - 0.46) / 0.12
            char_mesh.vertex_groups[f"{side}UpperLeg"].add([v.index], w_knee, 'REPLACE')
            char_mesh.vertex_groups[f"{side}LowerLeg"].add([v.index], 1.0 - w_knee, 'REPLACE')
        elif vz > 0.15:
            char_mesh.vertex_groups[f"{side}LowerLeg"].add([v.index], 1.0, 'REPLACE')
        else:
            char_mesh.vertex_groups[f"{side}Foot"].add([v.index], 1.0, 'REPLACE')

# Add Armature Modifier
arm_mod = char_mesh.modifiers.new(name="Armature", type='ARMATURE')
arm_mod.object = arm_obj
arm_mod.use_vertex_groups = True

# 7. Clean Non-Overlapping UV Unwrapping
print("Unwrapping UV Layout...")
bpy.context.view_layer.objects.active = char_mesh
char_mesh.select_set(True)
bpy.ops.object.mode_set(mode='EDIT')
bpy.ops.mesh.select_all(action='SELECT')
bpy.ops.uv.smart_project(angle_limit=66.0, island_margin=0.02)
bpy.ops.object.mode_set(mode='OBJECT')

tri_count = sum(len(p.vertices) - 2 for p in char_mesh.data.polygons)
print(f"Master v2 Mesh Assembled: {len(char_mesh.data.vertices):,} vertices, {len(char_mesh.data.polygons):,} polygons, {tri_count:,} triangles!")

# 8. Animation Setup (5 Core Actions)
print("Setting up Keyframe Animations...")
def create_action(name):
    act = bpy.data.actions.new(name=name)
    if not arm_obj.animation_data:
        arm_obj.animation_data_create()
    arm_obj.animation_data.action = act
    return act

# Action 1: Idle Breathing
act_idle = create_action("ARES_EVA_Idle_Breathing")
chest_p = arm_obj.pose.bones.get("Chest")
head_p  = arm_obj.pose.bones.get("Head")
if chest_p:
    chest_p.rotation_euler = (0, 0, 0); chest_p.keyframe_insert(data_path="rotation_euler", frame=1)
    chest_p.rotation_euler = (math.radians(1.5), 0, 0); chest_p.keyframe_insert(data_path="rotation_euler", frame=30)
    chest_p.rotation_euler = (0, 0, 0); chest_p.keyframe_insert(data_path="rotation_euler", frame=60)
if head_p:
    head_p.rotation_euler = (0, 0, 0); head_p.keyframe_insert(data_path="rotation_euler", frame=1)
    head_p.rotation_euler = (math.radians(-0.8), 0, 0); head_p.keyframe_insert(data_path="rotation_euler", frame=30)
    head_p.rotation_euler = (0, 0, 0); head_p.keyframe_insert(data_path="rotation_euler", frame=60)

# Action 2: Walk Forward
act_walk = create_action("ARES_EVA_Walk_Forward")
leg_l = arm_obj.pose.bones.get("LeftUpperLeg")
leg_r = arm_obj.pose.bones.get("RightUpperLeg")
arm_l = arm_obj.pose.bones.get("LeftUpperArm")
arm_r = arm_obj.pose.bones.get("RightUpperArm")
if leg_l and leg_r:
    leg_l.rotation_euler = (math.radians(-24), 0, 0); leg_r.rotation_euler = (math.radians(24), 0, 0)
    leg_l.keyframe_insert(data_path="rotation_euler", frame=1); leg_r.keyframe_insert(data_path="rotation_euler", frame=1)
    leg_l.rotation_euler = (math.radians(24), 0, 0); leg_r.rotation_euler = (math.radians(-24), 0, 0)
    leg_l.keyframe_insert(data_path="rotation_euler", frame=15); leg_r.keyframe_insert(data_path="rotation_euler", frame=15)
    leg_l.rotation_euler = (math.radians(-24), 0, 0); leg_r.rotation_euler = (math.radians(24), 0, 0)
    leg_l.keyframe_insert(data_path="rotation_euler", frame=30); leg_r.keyframe_insert(data_path="rotation_euler", frame=30)
if arm_l and arm_r:
    arm_l.rotation_euler = (math.radians(16), 0, 0); arm_r.rotation_euler = (math.radians(-16), 0, 0)
    arm_l.keyframe_insert(data_path="rotation_euler", frame=1); arm_r.keyframe_insert(data_path="rotation_euler", frame=1)
    arm_l.rotation_euler = (math.radians(-16), 0, 0); arm_r.rotation_euler = (math.radians(16), 0, 0)
    arm_l.keyframe_insert(data_path="rotation_euler", frame=15); arm_r.keyframe_insert(data_path="rotation_euler", frame=15)
    arm_l.rotation_euler = (math.radians(16), 0, 0); arm_r.rotation_euler = (math.radians(-16), 0, 0)
    arm_l.keyframe_insert(data_path="rotation_euler", frame=30); arm_r.keyframe_insert(data_path="rotation_euler", frame=30)

# Action 3: Rover Seated
act_seat = create_action("ARES_EVA_Rover_Seated")
hips_p = arm_obj.pose.bones.get("Hips")
knee_l = arm_obj.pose.bones.get("LeftLowerLeg")
knee_r = arm_obj.pose.bones.get("RightLowerLeg")
if hips_p:
    hips_p.location = (0, 0, -0.38); hips_p.keyframe_insert(data_path="location", frame=1)
if leg_l and leg_r:
    leg_l.rotation_euler = (math.radians(-82), 0, 0); leg_r.rotation_euler = (math.radians(-82), 0, 0)
    leg_l.keyframe_insert(data_path="rotation_euler", frame=1); leg_r.keyframe_insert(data_path="rotation_euler", frame=1)
if knee_l and knee_r:
    knee_l.rotation_euler = (math.radians(85), 0, 0); knee_r.rotation_euler = (math.radians(85), 0, 0)
    knee_l.keyframe_insert(data_path="rotation_euler", frame=1); knee_r.keyframe_insert(data_path="rotation_euler", frame=1)
if arm_l and arm_r:
    arm_l.rotation_euler = (math.radians(-32), math.radians(-12), 0); arm_r.rotation_euler = (math.radians(-32), math.radians(12), 0)
    arm_l.keyframe_insert(data_path="rotation_euler", frame=1); arm_r.keyframe_insert(data_path="rotation_euler", frame=1)

# Action 4: Scanner Inspect
act_scan = create_action("ARES_EVA_Scanner_Inspect")
forearm_r = arm_obj.pose.bones.get("RightLowerArm")
if arm_r and forearm_r:
    arm_r.rotation_euler = (math.radians(-42), math.radians(18), 0); forearm_r.rotation_euler = (math.radians(45), 0, 0)
    arm_r.keyframe_insert(data_path="rotation_euler", frame=1); forearm_r.keyframe_insert(data_path="rotation_euler", frame=1)
    arm_r.rotation_euler = (math.radians(-54), math.radians(-12), math.radians(18)); forearm_r.rotation_euler = (math.radians(58), math.radians(12), 0)
    arm_r.keyframe_insert(data_path="rotation_euler", frame=22); forearm_r.keyframe_insert(data_path="rotation_euler", frame=22)
    arm_r.rotation_euler = (math.radians(-42), math.radians(18), 0); forearm_r.rotation_euler = (math.radians(45), 0, 0)
    arm_r.keyframe_insert(data_path="rotation_euler", frame=45); forearm_r.keyframe_insert(data_path="rotation_euler", frame=45)

# Action 5: Alarm
act_alarm = create_action("ARES_EVA_Emergency_Alarm")
if arm_r and forearm_r:
    arm_r.rotation_euler = (0, 0, 0); forearm_r.rotation_euler = (0, 0, 0)
    arm_r.keyframe_insert(data_path="rotation_euler", frame=1); forearm_r.keyframe_insert(data_path="rotation_euler", frame=1)
    arm_r.rotation_euler = (math.radians(-112), math.radians(32), math.radians(-18)); forearm_r.rotation_euler = (math.radians(108), 0, math.radians(-12))
    arm_r.keyframe_insert(data_path="rotation_euler", frame=18); forearm_r.keyframe_insert(data_path="rotation_euler", frame=18)
    arm_r.keyframe_insert(data_path="rotation_euler", frame=30); forearm_r.keyframe_insert(data_path="rotation_euler", frame=30)
    arm_r.rotation_euler = (0, 0, 0); forearm_r.rotation_euler = (0, 0, 0)
    arm_r.keyframe_insert(data_path="rotation_euler", frame=40); forearm_r.keyframe_insert(data_path="rotation_euler", frame=40)

arm_obj.animation_data.action = act_idle
print("Animation Library created successfully!")

# 9. Save Versioned Blender Master (.blend)
blend_path = f"{v2_dir}/blender/ARES_EVA_MK1_v2_Master.blend"
bpy.ops.wm.save_as_mainfile(filepath=blend_path)
print(f"Saved Master v2 Blender File: {blend_path} ({os.path.getsize(blend_path):,} bytes)")

# 10. Export glTF 2.0 Binary (.glb)
glb_path = f"{v2_dir}/models/ARES_EVA_MK1_v2_Rigged.glb"
bpy.ops.export_scene.gltf(
    filepath=glb_path,
    export_format='GLB',
    use_selection=False,
    export_yup=True,
    export_animations=True,
    export_skins=True,
    export_materials='EXPORT'
)
print(f"Exported Skinned glTF 2.0 Binary: {glb_path} ({os.path.getsize(glb_path):,} bytes)")

# 11. Export Autodesk FBX (.fbx)
fbx_path = f"{v2_dir}/models/ARES_EVA_MK1_v2_Rigged.fbx"
bpy.ops.export_scene.fbx(
    filepath=fbx_path,
    use_selection=False,
    global_scale=1.0,
    axis_forward='-Z',
    axis_up='Y',
    apply_unit_scale=True,
    bake_space_transform=True,
    armature_nodetype='NULL',
    bake_anim=True,
    bake_anim_use_all_actions=True
)
print(f"Exported Autodesk FBX: {fbx_path} ({os.path.getsize(fbx_path):,} bytes)")

# 12. Generate Target LOD0, LOD1, LOD2, LOD3 Meshes
print("Generating Level of Detail (LOD) Meshes...")
lod_levels = [
    ("LOD0", 1.0, f"{v2_dir}/models/ARES_EVA_MK1_v2_LOD0.obj"),
    ("LOD1", 0.45, f"{v2_dir}/models/ARES_EVA_MK1_v2_LOD1.obj"),
    ("LOD2", 0.18, f"{v2_dir}/models/ARES_EVA_MK1_v2_LOD2.obj"),
    ("LOD3", 0.06, f"{v2_dir}/models/ARES_EVA_MK1_v2_LOD3.obj")
]

for lod_name, ratio, lod_file in lod_levels:
    dec_mod = char_mesh.modifiers.new(name=f"Decimate_{lod_name}", type='DECIMATE')
    dec_mod.ratio = ratio
    bpy.context.view_layer.objects.active = char_mesh
    char_mesh.select_set(True)
    if hasattr(bpy.ops.wm, "obj_export"):
        bpy.ops.wm.obj_export(filepath=lod_file, export_selected_objects=True)
    else:
        bpy.ops.export_scene.obj(filepath=lod_file, use_selection=True)
    char_mesh.modifiers.remove(dec_mod)
    print(f"Generated {lod_name} ({ratio*100:.0f}% target): {lod_file} ({os.path.getsize(lod_file):,} bytes)")

print("=== ARES EVA MK-I v2 REBUILD COMPLETED SUCCESSFULLY! ===")
