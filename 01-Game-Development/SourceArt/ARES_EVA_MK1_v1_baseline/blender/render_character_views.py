import bpy
import math
import os

renders_dir = "/Users/kapilash/.gemini/antigravity/brain/fe04a22b-3e01-409f-a35c-ed439568d853/renders_astronaut"
os.makedirs(renders_dir, exist_ok=True)

# Set render settings for clean, fast EEVEE or Workbench rendering
scene = bpy.context.scene
scene.render.resolution_x = 800
scene.render.resolution_y = 600
scene.render.resolution_percentage = 100
scene.render.image_settings.file_format = 'PNG'

# Setup camera
cam_data = bpy.data.cameras.new("RenderCamera")
cam_obj = bpy.data.objects.new("RenderCamera", cam_data)
scene.collection.objects.link(cam_obj)
scene.camera = cam_obj
cam_data.lens = 50.0

# Setup 3-point Studio Lighting
# Key light
light_key = bpy.data.lights.new("Light_Key", 'SUN')
light_key.energy = 4.0
light_key.color = (1.0, 0.95, 0.90)
obj_key = bpy.data.objects.new("Light_Key", light_key)
obj_key.rotation_euler = (math.radians(50), math.radians(20), math.radians(-35))
scene.collection.objects.link(obj_key)

# Fill light
light_fill = bpy.data.lights.new("Light_Fill", 'AREA')
light_fill.energy = 80.0
light_fill.color = (0.75, 0.85, 1.0)
obj_fill = bpy.data.objects.new("Light_Fill", light_fill)
obj_fill.location = (-2.5, -2.0, 2.0)
scene.collection.objects.link(obj_fill)

# Back rim light
light_rim = bpy.data.lights.new("Light_Rim", 'POINT')
light_rim.energy = 150.0
light_rim.color = (0.0, 0.85, 1.0)
obj_rim = bpy.data.objects.new("Light_Rim", light_rim)
obj_rim.location = (0, 2.5, 2.2)
scene.collection.objects.link(obj_rim)

# Render viewpoints
views = [
    # filename, cam_loc, cam_rot, target_action, frame
    ("01_astronaut_front_hero.png", (1.6, -2.4, 1.5), (math.radians(75), 0, math.radians(35)), "ARES_EVA_Idle_Breathing", 1),
    ("02_astronaut_rear_plss.png",  (-1.4, 2.6, 1.4), (math.radians(75), 0, math.radians(215)), "ARES_EVA_Idle_Breathing", 1),
    ("03_astronaut_side_profile.png", (3.2, 0, 1.2),  (math.radians(90), 0, math.radians(90)),  "ARES_EVA_Idle_Breathing", 1),
    ("04_astronaut_helmet_closeup.png", (0.5, -0.9, 1.7), (math.radians(82), 0, math.radians(28)), "ARES_EVA_Idle_Breathing", 1),
    ("05_astronaut_rover_seated.png", (1.8, -2.0, 1.2), (math.radians(75), 0, math.radians(40)), "ARES_EVA_Rover_Seated", 1),
    ("06_astronaut_walk_cycle.png",  (1.6, -2.4, 1.2), (math.radians(78), 0, math.radians(35)), "ARES_EVA_Walk_Forward", 15),
]

arm_obj = bpy.data.objects.get("ARES_EVA_Armature")

for fname, loc, rot, act_name, frame in views:
    cam_obj.location = loc
    cam_obj.rotation_euler = rot
    
    if arm_obj and act_name in bpy.data.actions:
        arm_obj.animation_data.action = bpy.data.actions[act_name]
        scene.frame_set(frame)
        
    scene.render.filepath = os.path.join(renders_dir, fname)
    bpy.ops.render.render(write_still=True)
    print(f"Rendered [{fname}] -> {scene.render.filepath}")

print("All Blender Camera Renders Completed!")
