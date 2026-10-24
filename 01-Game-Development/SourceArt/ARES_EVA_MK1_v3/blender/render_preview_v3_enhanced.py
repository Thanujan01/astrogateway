import bpy, os

# Paths
blend_path = '/Users/kapilash/.gemini/antigravity/scratch/ARES_EVA_MK1_v3/blender/ARES_EVA_MK1_v3_Master.blend'
output_dir = '/Users/kapilash/.gemini/antigravity/scratch/ARES_EVA_MK1_v3/renders'
os.makedirs(output_dir, exist_ok=True)
output_path = os.path.join(output_dir, 'ARES_EVA_MK1_v3_preview_enhanced.png')

# Load the master blend file
bpy.ops.wm.open_mainfile(filepath=blend_path)

# Ensure there is a camera; create if missing
if not any(obj.type == 'CAMERA' for obj in bpy.data.objects):
    cam_data = bpy.data.cameras.new('PreviewCam')
    cam_obj = bpy.data.objects.new('PreviewCam', cam_data)
    bpy.context.collection.objects.link(cam_obj)
    # Position camera for a dynamic three‑quarter view
    cam_obj.location = (2.5, -4.0, 2.0)
    cam_obj.rotation_euler = (1.2, 0.0, 0.9)
    bpy.context.scene.camera = cam_obj
else:
    cam_obj = next(obj for obj in bpy.data.objects if obj.type == 'CAMERA')
    bpy.context.scene.camera = cam_obj

# Add a simple ground plane for context
if 'Ground' not in bpy.data.objects:
    bpy.ops.mesh.primitive_plane_add(size=10, location=(0,0,0))
    ground = bpy.context.active_object
    ground.name = 'Ground'
    # Dark gray material
    mat = bpy.data.materials.new(name='GroundMat')
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes.get('Principled BSDF')
    bsdf.inputs['Base Color'].default_value = (0.1, 0.1, 0.1, 1)
    ground.data.materials.append(mat)

# Lighting – HDRI style using a simple world gradient and a sun lamp
# World
world = bpy.context.scene.world
if not world:
    world = bpy.data.worlds.new('World')
    bpy.context.scene.world = world
world.use_nodes = True
bg = world.node_tree.nodes.get('Background')
bg.inputs['Color'].default_value = (0.02, 0.02, 0.03, 1)  # dark night sky
# Sun lamp
if not any(obj.type == 'LIGHT' for obj in bpy.data.objects):
    light_data = bpy.data.lights.new(name='Sun', type='SUN')
    light_data.energy = 5.0
    light_obj = bpy.data.objects.new(name='Sun', object_data=light_data)
    bpy.context.collection.objects.link(light_obj)
    light_obj.rotation_euler = (0.785398, 0, 0.785398)  # 45° angle

# Render settings – use Cycles (CPU) for high‑quality
scene = bpy.context.scene
scene.render.engine = 'CYCLES'
scene.cycles.device = 'CPU'
scene.cycles.samples = 128
scene.cycles.use_denoising = True
scene.render.image_settings.file_format = 'PNG'
scene.render.filepath = output_path
scene.render.resolution_x = 1920
scene.render.resolution_y = 1080
scene.render.resolution_percentage = 100

# Optional – set film exposure for brighter look
scene.view_settings.exposure = 1.0

# Render
bpy.ops.render.render(write_still=True)
print('Enhanced render saved to', output_path)
