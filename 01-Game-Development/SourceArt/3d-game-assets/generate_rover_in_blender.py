"""
Blender 3.x / 4.x Python Automation Script
Procedurally generates the Advanced Mars Research & Exploration Rover with PBR shaders,
rocker-bogie 6-wheel suspension, ChemCam mast, robotic science arm, MMRTG generator, and Martian lighting.
"""

import bpy
import math

def build_mars_rover():
    # 1. Clear existing scene objects
    bpy.ops.object.select_all(action='SELECT')
    bpy.ops.object.delete(use_global=False)

    # 2. PBR Material Helper
    def create_pbr_mat(name, base_color, metallic=0.0, roughness=0.4, emission=None, emission_strength=1.0):
        mat = bpy.data.materials.new(name=name)
        mat.use_nodes = True
        bsdf = mat.node_tree.nodes.get("Principled BSDF")
        if bsdf:
            bsdf.inputs['Base Color'].default_value = base_color
            bsdf.inputs['Metallic'].default_value = metallic
            bsdf.inputs['Roughness'].default_value = roughness
            if emission:
                if 'Emission Color' in bsdf.inputs:
                    bsdf.inputs['Emission Color'].default_value = emission
                    bsdf.inputs['Emission Strength'].default_value = emission_strength
                elif 'Emission' in bsdf.inputs:
                    bsdf.inputs['Emission'].default_value = emission
                    if 'Emission Strength' in bsdf.inputs:
                        bsdf.inputs['Emission Strength'].default_value = emission_strength
        return mat

    mat_body = create_pbr_mat("Mat_CeramicHull", (0.90, 0.92, 0.95, 1.0), metallic=0.1, roughness=0.35)
    mat_chassis = create_pbr_mat("Mat_TitaniumFrame", (0.18, 0.20, 0.24, 1.0), metallic=0.85, roughness=0.3)
    mat_wheels = create_pbr_mat("Mat_CarbonTreads", (0.10, 0.11, 0.12, 1.0), metallic=0.2, roughness=0.6)
    mat_foil = create_pbr_mat("Mat_GoldThermalFoil", (0.95, 0.76, 0.14, 1.0), metallic=0.95, roughness=0.2)
    mat_optics = create_pbr_mat("Mat_CyanLaserOptics", (0.05, 0.8, 1.0, 1.0), metallic=0.1, roughness=0.05,
                                emission=(0.0, 0.85, 1.0, 1.0), emission_strength=8.0)

    # 3. Main Equipment Chassis (Body)
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=(0, 0, 0.90))
    body = bpy.context.active_object
    body.name = "Rover_Body_Hull"
    body.scale = (1.30, 1.70, 0.50)
    bpy.ops.object.transform_apply(scale=True)
    body.data.materials.append(mat_body)
    
    # Add bevel for smooth aerospace edges
    bev = body.modifiers.new(name="Bevel", type='BEVEL')
    bev.width = 0.05
    bev.segments = 2

    # Top science deck
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=(0, -0.15, 1.22))
    deck = bpy.context.active_object
    deck.name = "Rover_Electronics_Deck"
    deck.scale = (1.10, 1.20, 0.14)
    bpy.ops.object.transform_apply(scale=True)
    deck.data.materials.append(mat_body)

    # 4. Rocker-Bogie Suspension & Links
    for sx in [-1, 1]:
        # Main pivot hub
        bpy.ops.mesh.primitive_cylinder_add(vertices=16, radius=0.08, depth=0.15, location=(sx * 0.70, 0, 0.85), rotation=(0, math.radians(90), 0))
        pivot = bpy.context.active_object
        pivot.name = f"Suspension_Pivot_{'L' if sx < 0 else 'R'}"
        pivot.data.materials.append(mat_chassis)

        # Rocker link bar
        bpy.ops.mesh.primitive_cube_add(size=1.0, location=(sx * 0.95, 0.0, 0.75))
        rocker = bpy.context.active_object
        rocker.name = f"Rocker_Strut_{'L' if sx < 0 else 'R'}"
        rocker.scale = (0.08, 0.12, 0.12)
        bpy.ops.object.transform_apply(scale=True)
        rocker.data.materials.append(mat_chassis)

        # Bogie bridge links to front and rear
        bpy.ops.mesh.primitive_cube_add(size=1.0, location=(sx * 1.06, 0.52, 0.40))
        link_f = bpy.context.active_object
        link_f.scale = (0.07, 1.05, 0.08)
        bpy.ops.object.transform_apply(scale=True)
        link_f.data.materials.append(mat_chassis)

        bpy.ops.mesh.primitive_cube_add(size=1.0, location=(sx * 1.06, -0.52, 0.40))
        link_r = bpy.context.active_object
        link_r.scale = (0.07, 1.05, 0.08)
        bpy.ops.object.transform_apply(scale=True)
        link_r.data.materials.append(mat_chassis)

    # 5. Six Exploration Wheels
    wheel_positions = [
        (-1.18,  1.05, 0.35, "FL"),
        (-1.18,  0.00, 0.35, "ML"),
        (-1.18, -1.05, 0.35, "RL"),
        ( 1.18,  1.05, 0.35, "FR"),
        ( 1.18,  0.00, 0.35, "MR"),
        ( 1.18, -1.05, 0.35, "RR"),
    ]
    for x, y, z, label in wheel_positions:
        bpy.ops.mesh.primitive_cylinder_add(vertices=20, radius=0.32, depth=0.28, location=(x, y, z), rotation=(0, math.radians(90), 0))
        w = bpy.context.active_object
        w.name = f"Wheel_{label}"
        w.data.materials.append(mat_wheels)
        
        # Hub cap accent
        bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.12, depth=0.30, location=(x, y, z), rotation=(0, math.radians(90), 0))
        hub = bpy.context.active_object
        hub.name = f"Wheel_Hub_{label}"
        hub.data.materials.append(mat_chassis)

    # 6. Remote Sensing Mast & ChemCam Sensor Head
    bpy.ops.mesh.primitive_cylinder_add(vertices=16, radius=0.06, depth=0.90, location=(0.35, 0.55, 1.72))
    mast = bpy.context.active_object
    mast.name = "Remote_Sensing_Mast"
    mast.data.materials.append(mat_chassis)

    # Gimbal sensor housing
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=(0.32, 0.55, 2.32))
    head = bpy.context.active_object
    head.name = "ChemCam_Sensor_Head"
    head.scale = (0.28, 0.26, 0.24)
    bpy.ops.object.transform_apply(scale=True)
    head.data.materials.append(mat_chassis)

    # Main ChemCam Laser Lens
    bpy.ops.mesh.primitive_cylinder_add(vertices=16, radius=0.07, depth=0.08, location=(0.32, 0.69, 2.36), rotation=(math.radians(90), 0, 0))
    lens_main = bpy.context.active_object
    lens_main.name = "Optics_ChemCam_Laser"
    lens_main.data.materials.append(mat_optics)

    # Stereo NavCams (Left & Right)
    for off_x in [-0.10, 0.10]:
        bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.04, depth=0.06, location=(0.32 + off_x, 0.69, 2.27), rotation=(math.radians(90), 0, 0))
        nc = bpy.context.active_object
        nc.name = f"Optics_NavCam_{'L' if off_x < 0 else 'R'}"
        nc.data.materials.append(mat_optics)

    # 7. Articulated Robotic Science Arm
    # Shoulder turret
    bpy.ops.mesh.primitive_cylinder_add(vertices=16, radius=0.09, depth=0.12, location=(-0.35, 0.78, 0.95))
    shoulder = bpy.context.active_object
    shoulder.name = "Arm_Shoulder_Joint"
    shoulder.data.materials.append(mat_foil)

    # Upper Boom
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=(-0.35, 1.00, 0.83))
    boom = bpy.context.active_object
    boom.name = "Arm_Upper_Boom"
    boom.scale = (0.07, 0.44, 0.08)
    bpy.ops.object.transform_apply(scale=True)
    boom.data.materials.append(mat_foil)

    # Forearm
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=(-0.35, 1.23, 0.64))
    forearm = bpy.context.active_object
    forearm.name = "Arm_Forearm"
    forearm.scale = (0.07, 0.08, 0.38)
    bpy.ops.object.transform_apply(scale=True)
    forearm.data.materials.append(mat_foil)

    # Drill / Spectrometer Turret
    bpy.ops.mesh.primitive_cylinder_add(vertices=16, radius=0.12, depth=0.18, location=(-0.35, 1.23, 0.40))
    turret = bpy.context.active_object
    turret.name = "Arm_Science_Drill_Turret"
    turret.data.materials.append(mat_chassis)

    # Drill Bit
    bpy.ops.mesh.primitive_cone_add(vertices=8, radius1=0.04, depth=0.14, location=(-0.35, 1.23, 0.25))
    drill = bpy.context.active_object
    drill.name = "Arm_Drill_Bit"
    drill.data.materials.append(mat_chassis)

    # 8. MMRTG Nuclear Generator & Radiator Fins
    bpy.ops.mesh.primitive_cylinder_add(vertices=20, radius=0.18, depth=0.45, location=(0, -1.05, 0.92), rotation=(math.radians(90), 0, 0))
    rtg = bpy.context.active_object
    rtg.name = "MMRTG_Generator"
    rtg.data.materials.append(mat_chassis)

    # Radiator cooling fins
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=(0, -1.05, 0.92))
    fin_h = bpy.context.active_object
    fin_h.scale = (0.70, 0.38, 0.03)
    bpy.ops.object.transform_apply(scale=True)
    fin_h.data.materials.append(mat_chassis)

    bpy.ops.mesh.primitive_cube_add(size=1.0, location=(0, -1.05, 0.92))
    fin_v = bpy.context.active_object
    fin_v.scale = (0.03, 0.38, 0.45)
    bpy.ops.object.transform_apply(scale=True)
    fin_v.data.materials.append(mat_chassis)

    # 9. High-Gain Parabolic Dish Antenna
    bpy.ops.mesh.primitive_cylinder_add(vertices=12, radius=0.03, depth=0.35, location=(-0.38, -0.50, 1.45))
    ant_mast = bpy.context.active_object
    ant_mast.data.materials.append(mat_chassis)

    bpy.ops.mesh.primitive_cylinder_add(vertices=16, radius=0.28, depth=0.05, location=(-0.38, -0.52, 1.70), rotation=(math.radians(35), 0, 0))
    dish = bpy.context.active_object
    dish.name = "High_Gain_Antenna_Dish"
    dish.data.materials.append(mat_foil)

    # 10. Martian Environment Lighting & Camera
    # Martian Sun (Warm sunlight)
    bpy.ops.object.light_add(type='SUN', location=(5.0, -4.0, 8.0))
    sun = bpy.context.active_object
    sun.data.energy = 4.5
    sun.data.color = (1.0, 0.88, 0.74)
    sun.rotation_euler = (math.radians(50), math.radians(15), math.radians(-35))

    # Ambient Fill (Martian Red Sky bounce)
    bpy.ops.object.light_add(type='AREA', location=(-3.0, 2.0, 3.0))
    fill = bpy.context.active_object
    fill.data.energy = 80
    fill.data.color = (0.95, 0.55, 0.40)

    # Ground Plane (Martian Terrain)
    bpy.ops.mesh.primitive_plane_add(size=25, location=(0, 0, 0.03))
    ground = bpy.context.active_object
    ground.name = "Martian_Terrain"
    mat_ground = create_pbr_mat("Mat_MartianRegolith", (0.55, 0.22, 0.12, 1.0), roughness=0.9)
    ground.data.materials.append(mat_ground)

    # Camera (Hero 3/4 Perspective)
    bpy.ops.object.camera_add(location=(4.2, -4.8, 3.2))
    camera = bpy.context.active_object
    camera.rotation_euler = (math.radians(65), 0, math.radians(42))
    bpy.context.scene.camera = camera

    print("Mars Research Rover successfully generated in Blender!")

if __name__ == "__main__":
    build_mars_rover()
