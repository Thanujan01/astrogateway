"""
Blender 3.x / 4.x Python Script
Run this script inside Blender's Scripting Tab to generate the Stylized Game Chest with PBR Materials & Studio Lighting.
"""

import bpy
import math

def create_chest_scene():
    # 1. Reset / Clear existing default scene objects
    bpy.ops.object.select_all(action='SELECT')
    bpy.ops.object.delete(use_global=False)

    # 2. Material Helper
    def create_material(name, base_color, metallic=0.0, roughness=0.5, emission=None, emission_strength=1.0):
        mat = bpy.data.materials.new(name=name)
        mat.use_nodes = True
        nodes = mat.node_tree.nodes
        bsdf = nodes.get("Principled BSDF")
        if bsdf:
            bsdf.inputs['Base Color'].default_value = base_color
            bsdf.inputs['Metallic'].default_value = metallic
            bsdf.inputs['Roughness'].default_value = roughness
            if emission:
                # Blender 4.0+ uses 'Emission Color' or 'Emission', handles both
                if 'Emission Color' in bsdf.inputs:
                    bsdf.inputs['Emission Color'].default_value = emission
                    bsdf.inputs['Emission Strength'].default_value = emission_strength
                elif 'Emission' in bsdf.inputs:
                    bsdf.inputs['Emission'].default_value = emission
                    if 'Emission Strength' in bsdf.inputs:
                        bsdf.inputs['Emission Strength'].default_value = emission_strength
        return mat

    mat_body = create_material("Mat_ChestBody", (0.12, 0.14, 0.18, 1.0), metallic=0.05, roughness=0.55)
    mat_gold = create_material("Mat_GoldTrim", (0.95, 0.72, 0.18, 1.0), metallic=0.95, roughness=0.22)
    mat_crystal = create_material("Mat_GlowingCrystal", (0.05, 0.8, 1.0, 1.0), metallic=0.1, roughness=0.1, 
                                  emission=(0.0, 0.85, 1.0, 1.0), emission_strength=6.0)

    # 3. Chest Base
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=(0, 0, 0.45))
    chest_base = bpy.context.active_object
    chest_base.name = "Chest_Base"
    chest_base.scale = (1.8, 1.2, 0.75)
    bpy.ops.object.transform_apply(scale=True)
    
    # Add subtle bevel modifier for stylized edge highlights
    bev = chest_base.modifiers.new(name="Bevel", type='BEVEL')
    bev.width = 0.04
    bev.segments = 2
    chest_base.data.materials.append(mat_body)

    # 4. Chest Lid (Vaulted arch)
    bpy.ops.mesh.primitive_cylinder_add(vertices=24, radius=0.62, depth=1.84, location=(0, 0, 0.82), rotation=(0, math.radians(90), 0))
    chest_lid = bpy.context.active_object
    chest_lid.name = "Chest_Lid"
    chest_lid.scale = (1.0, 1.0, 0.72)
    bpy.ops.object.transform_apply(scale=True)
    chest_lid.data.materials.append(mat_body)

    # 5. Gold Trim Bands (Reinforcement)
    # Front/Back/Side edge bands
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=(0, 0, 0.10))
    base_trim = bpy.context.active_object
    base_trim.name = "Chest_Base_Trim"
    base_trim.scale = (1.86, 1.26, 0.12)
    bpy.ops.object.transform_apply(scale=True)
    base_trim.data.materials.append(mat_gold)

    # Center Arch Band
    bpy.ops.mesh.primitive_cylinder_add(vertices=24, radius=0.64, depth=0.18, location=(0, 0, 0.82), rotation=(0, math.radians(90), 0))
    arch_band = bpy.context.active_object
    arch_band.name = "Chest_Lid_Band"
    arch_band.scale = (1.0, 1.0, 0.74)
    bpy.ops.object.transform_apply(scale=True)
    arch_band.data.materials.append(mat_gold)

    # 6. Lock Plate & Crystal Gem
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=(0, -0.63, 0.78))
    lock_plate = bpy.context.active_object
    lock_plate.name = "Lock_Plate"
    lock_plate.scale = (0.28, 0.08, 0.28)
    bpy.ops.object.transform_apply(scale=True)
    lock_plate.data.materials.append(mat_gold)

    # Glowing Crystal (Octahedron)
    bpy.ops.mesh.primitive_circle_add(vertices=4, radius=0.12, location=(0, -0.68, 0.78))
    bpy.ops.mesh.primitive_ico_sphere_add(subdivisions=1, radius=0.12, location=(0, -0.68, 0.78))
    crystal = bpy.context.active_object
    crystal.name = "Glowing_Power_Crystal"
    crystal.scale = (0.9, 0.7, 1.4)
    bpy.ops.object.transform_apply(scale=True)
    crystal.data.materials.append(mat_crystal)

    # 7. Studio 3-Point Lighting
    # Key Light (Warm)
    bpy.ops.object.light_add(type='AREA', location=(3.0, -3.5, 3.5))
    key_light = bpy.context.active_object
    key_light.data.energy = 450
    key_light.data.color = (1.0, 0.95, 0.88)
    key_light.rotation_euler = (math.radians(45), 0, math.radians(35))

    # Fill Light (Cool Blue)
    bpy.ops.object.light_add(type='AREA', location=(-3.0, -2.5, 2.0))
    fill_light = bpy.context.active_object
    fill_light.data.energy = 180
    fill_light.data.color = (0.75, 0.85, 1.0)
    fill_light.rotation_euler = (math.radians(55), 0, math.radians(-40))

    # Rim / Backlight (Cyan accent to complement crystal)
    bpy.ops.object.light_add(type='POINT', location=(0.0, 2.8, 2.8))
    rim_light = bpy.context.active_object
    rim_light.data.energy = 320
    rim_light.data.color = (0.2, 0.8, 1.0)

    # 8. Camera (Isometric 3/4 perspective)
    bpy.ops.object.camera_add(location=(3.6, -4.2, 3.2))
    camera = bpy.context.active_object
    camera.rotation_euler = (math.radians(62), 0, math.radians(40))
    bpy.context.scene.camera = camera

    print("Stylized Loot Chest successfully created!")

if __name__ == "__main__":
    create_chest_scene()
