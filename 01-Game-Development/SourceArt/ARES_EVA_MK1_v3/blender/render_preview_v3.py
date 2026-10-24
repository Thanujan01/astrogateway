import bpy, os

# Paths
blend_path = '/Users/kapilash/.gemini/antigravity/scratch/ARES_EVA_MK1_v3/blender/ARES_EVA_MK1_v3_Master.blend'
output_dir = '/Users/kapilash/.gemini/antigravity/scratch/ARES_EVA_MK1_v3/renders'
os.makedirs(output_dir, exist_ok=True)
output_path = os.path.join(output_dir, 'ARES_EVA_MK1_v3_preview.png')

# Load the master blend file
bpy.ops.wm.open_mainfile(filepath=blend_path)

# Ensure there is a camera; create if missing
if not any(obj.type == 'CAMERA' for obj in bpy.data.objects):
    cam_data = bpy.data.cameras.new('PreviewCam')
    cam_obj = bpy.data.objects.new('PreviewCam', cam_data)
    bpy.context.collection.objects.link(cam_obj)
    # Position camera to front view
    cam_obj.location = (0.0, -5.0, 1.5)
    cam_obj.rotation_euler = (1.308996938, 0, 0)  # approx 75 deg downwards
    bpy.context.scene.camera = cam_obj
else:
    # Use first camera
    cam_obj = next(obj for obj in bpy.data.objects if obj.type == 'CAMERA')
    bpy.context.scene.camera = cam_obj

# Set render settings
scene = bpy.context.scene
scene.render.engine = 'BLENDER_EEVEE'
scene.render.image_settings.file_format = 'PNG'
scene.render.filepath = output_path
scene.render.resolution_x = 1920
scene.render.resolution_y = 1080
scene.render.resolution_percentage = 100

# Optional: set lighting (use existing lights or add a simple sun)
if not any(obj.type == 'LIGHT' for obj in bpy.data.objects):
    light_data = bpy.data.lights.new(name='Sun', type='SUN')
    light_obj = bpy.data.objects.new(name='Sun', object_data=light_data)
    bpy.context.collection.objects.link(light_obj)
    light_obj.rotation_euler = (0.785398, 0, 0.785398)  # 45° angles

# Render
bpy.ops.render.render(write_still=True)
print('Render saved to', output_path)
