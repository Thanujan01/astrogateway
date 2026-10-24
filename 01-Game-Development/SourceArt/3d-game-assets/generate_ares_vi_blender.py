"""
ARES-VI Anti-Gravity Mars Research Rover — Master Blender 3.x / 4.x Python Automation
Sets up the complete AAA vehicle hierarchy, PBR materials, keyframed Ground-to-Hover transition,
and cinematic Mars studio lighting.
"""

import bpy
import os
import math

def setup_ares_vi_scene():
    # 1. Reset Scene
    bpy.ops.object.select_all(action='SELECT')
    bpy.ops.object.delete(use_global=False)

    # Clean existing collections
    for col in list(bpy.data.collections):
        bpy.data.collections.remove(col)

    # 2. Setup Collections Hierarchy
    root_col = bpy.data.collections.new("00_ARES_VI_ROOT")
    bpy.context.scene.collection.children.link(root_col)

    col_chassis = bpy.data.collections.new("01_CHASSIS_ARMOR")
    col_cockpit = bpy.data.collections.new("02_COCKPIT_INTERIOR")
    col_suspension = bpy.data.collections.new("03_SUSPENSION_WHEELS")
    col_antigrav = bpy.data.collections.new("04_ANTIGRAV_PROPULSION")
    col_science = bpy.data.collections.new("05_SCIENCE_EQUIPMENT")
    col_studio = bpy.data.collections.new("06_STUDIO_LIGHTING")

    for c in [col_chassis, col_cockpit, col_suspension, col_antigrav, col_science]:
        root_col.children.link(c)
    bpy.context.scene.collection.children.link(col_studio)

    # 3. Create AAA PBR Materials
    def get_or_create_mat(name, base_color, metallic=0.0, roughness=0.4, emission=None, emission_strength=1.0, alpha=1.0):
        mat = bpy.data.materials.get(name) or bpy.data.materials.new(name=name)
        mat.use_nodes = True
        nodes = mat.node_tree.nodes
        bsdf = nodes.get("Principled BSDF")
        if bsdf:
            bsdf.inputs['Base Color'].default_value = base_color
            bsdf.inputs['Metallic'].default_value = metallic
            bsdf.inputs['Roughness'].default_value = roughness
            if alpha < 1.0:
                if 'Alpha' in bsdf.inputs:
                    bsdf.inputs['Alpha'].default_value = alpha
                if 'Transmission Weight' in bsdf.inputs:
                    bsdf.inputs['Transmission Weight'].default_value = 0.85
                elif 'Transmission' in bsdf.inputs:
                    bsdf.inputs['Transmission'].default_value = 0.85
                mat.blend_method = 'BLEND'
            if emission:
                if 'Emission Color' in bsdf.inputs:
                    bsdf.inputs['Emission Color'].default_value = emission
                    bsdf.inputs['Emission Strength'].default_value = emission_strength
                elif 'Emission' in bsdf.inputs:
                    bsdf.inputs['Emission'].default_value = emission
                    if 'Emission Strength' in bsdf.inputs:
                        bsdf.inputs['Emission Strength'].default_value = emission_strength
        return mat

    mat_ceramic = get_or_create_mat("Mat_CeramicWhite", (0.92, 0.94, 0.96, 1.0), metallic=0.05, roughness=0.35)
    mat_graphite = get_or_create_mat("Mat_GraphiteTrim", (0.18, 0.20, 0.22, 1.0), metallic=0.30, roughness=0.50)
    mat_titanium = get_or_create_mat("Mat_TitaniumFrame", (0.40, 0.44, 0.48, 1.0), metallic=0.88, roughness=0.28)
    mat_orange = get_or_create_mat("Mat_MartianOrange", (0.98, 0.35, 0.05, 1.0), metallic=0.20, roughness=0.45)
    mat_gold = get_or_create_mat("Mat_GoldFoil", (0.95, 0.78, 0.16, 1.0), metallic=0.95, roughness=0.18)
    mat_carbon = get_or_create_mat("Mat_CarbonTreads", (0.12, 0.13, 0.15, 1.0), metallic=0.10, roughness=0.70)
    mat_glass = get_or_create_mat("Mat_CanopyGlass", (0.20, 0.35, 0.45, 1.0), metallic=0.10, roughness=0.08, alpha=0.55)
    mat_energy = get_or_create_mat("Mat_CyanEnergy", (0.10, 0.85, 1.0, 1.0), metallic=0.0, roughness=0.10,
                                   emission=(0.0, 0.92, 1.0, 1.0), emission_strength=10.0)

    # 4. Import Master Model OBJ
    obj_path = "/Users/kapilash/.gemini/antigravity/scratch/3d-game-assets/ares_vi_master.obj"
    if os.path.exists(obj_path):
        if hasattr(bpy.ops.wm, "obj_import"):
            bpy.ops.wm.obj_import(filepath=obj_path)
        else:
            bpy.ops.import_scene.obj(filepath=obj_path)

        imported_objects = list(bpy.context.selected_objects)
        print(f"Imported {len(imported_objects)} mesh objects from OBJ.")

        # Organize into collections
        for obj in imported_objects:
            n = obj.name.lower()
            target_col = col_chassis
            if any(k in n for k in ['cockpit', 'canopy', 'seat', 'hud', 'navdisplay']):
                target_col = col_cockpit
            elif any(k in n for k in ['wheel', 'suspension', 'rocker', 'bogie', 'trailing']):
                target_col = col_suspension
            elif any(k in n for k in ['hover', 'stabilizer']):
                target_col = col_antigrav
            elif any(k in n for k in ['mast', 'chemcam', 'arm', 'drill', 'mmrtg', 'antenna']):
                target_col = col_science

            for c in list(obj.users_collection):
                c.objects.unlink(obj)
            target_col.objects.link(obj)

    # 5. Root Parent Controller (Vehicle Rig)
    bpy.ops.object.empty_add(type='CUBE', location=(0, 0, 0))
    root_ctrl = bpy.context.active_object
    root_ctrl.name = "ARES_VI_Root_Controller"
    root_col.objects.link(root_ctrl)
    if bpy.context.scene.collection in root_ctrl.users_collection:
        bpy.context.scene.collection.objects.unlink(root_ctrl)

    # Parent imported objects to Root Controller
    for obj in bpy.data.objects:
        if obj != root_ctrl and obj.type == 'MESH':
            obj.parent = root_ctrl

    # 6. Animation: Ground-to-Hover Transition (180 Frames)
    bpy.context.scene.frame_start = 1
    bpy.context.scene.frame_end = 180
    
    # Root Vehicle Float Animation
    root_ctrl.location = (0, 0, 0.0)
    root_ctrl.keyframe_insert(data_path="location", frame=1)
    root_ctrl.keyframe_insert(data_path="location", frame=60)
    
    # Lift to Hover Height (+0.35m)
    root_ctrl.location = (0, 0, 0.35)
    root_ctrl.keyframe_insert(data_path="location", frame=100)
    
    # Hover Idle Bobbing (Frames 100-180)
    for f in range(100, 181, 10):
        t = (f - 100) / 80.0 * 2 * math.pi
        bob_z = 0.35 + 0.04 * math.sin(t)
        bob_pitch = 0.015 * math.cos(t)
        root_ctrl.location = (0, 0, bob_z)
        root_ctrl.rotation_euler = (bob_pitch, 0, 0)
        root_ctrl.keyframe_insert(data_path="location", frame=f)
        root_ctrl.keyframe_insert(data_path="rotation_euler", frame=f)

    # 7. Cinematic Studio & Martian Environment Lighting
    # Key Light (Warm Martian Sunlight)
    bpy.ops.object.light_add(type='SUN', location=(6.0, -5.0, 10.0))
    sun = bpy.context.active_object
    sun.name = "KeyLight_MartianSun"
    sun.data.energy = 5.5
    sun.data.color = (1.0, 0.90, 0.78)
    sun.rotation_euler = (math.radians(52), math.radians(18), math.radians(-38))
    col_studio.objects.link(sun)
    bpy.context.scene.collection.objects.unlink(sun)

    # Fill Light (Cool Atmospheric Sky Bounce)
    bpy.ops.object.light_add(type='AREA', location=(-5.0, -3.0, 4.0))
    fill = bpy.context.active_object
    fill.name = "FillLight_Atmosphere"
    fill.data.energy = 160.0
    fill.data.color = (0.75, 0.85, 1.0)
    col_studio.objects.link(fill)
    bpy.context.scene.collection.objects.unlink(fill)

    # Underbody Cyan Emitter Accent Light (Emulates ground hover glow)
    bpy.ops.object.light_add(type='POINT', location=(0.0, 0.0, 0.20))
    under_glow = bpy.context.active_object
    under_glow.name = "HoverGroundGlow_Cyan"
    under_glow.data.energy = 250.0
    under_glow.data.color = (0.0, 0.88, 1.0)
    col_studio.objects.link(under_glow)
    bpy.context.scene.collection.objects.unlink(under_glow)

    # Camera (Hero 3/4 Perspective framed perfectly on rover)
    bpy.ops.object.camera_add(location=(4.8, -5.4, 3.2))
    cam = bpy.context.active_object
    cam.name = "Camera_ARES_VI_Hero"
    cam.rotation_euler = (math.radians(65), 0, math.radians(40))
    cam.data.lens = 50.0
    col_studio.objects.link(cam)
    bpy.context.scene.collection.objects.unlink(cam)
    bpy.context.scene.camera = cam

    # Ground Plane (Dark Tech Studio Floor)
    bpy.ops.mesh.primitive_plane_add(size=30, location=(0, 0, 0))
    floor = bpy.context.active_object
    floor.name = "Studio_Floor"
    mat_floor = get_or_create_mat("Mat_StudioFloor", (0.05, 0.06, 0.08, 1.0), metallic=0.5, roughness=0.35)
    floor.data.materials.append(mat_floor)
    col_studio.objects.link(floor)
    bpy.context.scene.collection.objects.unlink(floor)

    print("ARES-VI Master Scene Successfully Configured in Blender!")

if __name__ == "__main__":
    setup_ares_vi_scene()
