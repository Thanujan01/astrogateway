import bpy

# Reset scene
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete()

# Create Armature
arm_data = bpy.data.armatures.new("Humanoid_Armature")
arm_obj = bpy.data.objects.new("ARES_EVA_Armature", arm_data)
bpy.context.scene.collection.objects.link(arm_obj)
bpy.context.view_layer.objects.active = arm_obj

bpy.ops.object.mode_set(mode='EDIT')
eb = arm_data.edit_bones

# Root & Hips
root = eb.new("Root")
root.head = (0, 0, 0)
root.tail = (0, 0, 0.1)

hips = eb.new("Hips")
hips.head = (0, 0, 0.95)
hips.tail = (0, 0, 1.05)
hips.parent = root

spine = eb.new("Spine")
spine.head = (0, 0, 1.05)
spine.tail = (0, 0, 1.25)
spine.parent = hips

chest = eb.new("Chest")
chest.head = (0, 0, 1.25)
chest.tail = (0, 0, 1.45)
chest.parent = spine

neck = eb.new("Neck")
neck.head = (0, 0, 1.45)
neck.tail = (0, 0, 1.55)
neck.parent = chest

head = eb.new("Head")
head.head = (0, 0, 1.55)
head.tail = (0, 0, 1.80)
head.parent = neck

bpy.ops.object.mode_set(mode='OBJECT')

print(f"Created Armature successfully with {len(arm_data.bones)} bones!")

# Check export operators
print("Export glTF:", hasattr(bpy.ops.export_scene, "gltf"))
print("Export FBX:", hasattr(bpy.ops.export_scene, "fbx"))
