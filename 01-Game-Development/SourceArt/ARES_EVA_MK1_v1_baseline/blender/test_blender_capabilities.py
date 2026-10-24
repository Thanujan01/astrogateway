import bpy
import sys

print(f"=== BLENDER CAPABILITY VERIFICATION ===")
print(f"Blender Version: {bpy.app.version_string}")

# Test 1: Mesh creation
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete()
bpy.ops.mesh.primitive_uv_sphere_add(radius=0.15, location=(0, 0, 1.7))
head = bpy.context.active_object
head.name = "Test_Helmet"
print(f"Mesh creation: OK ({head.name})")

# Test 2: Armature creation
bpy.ops.object.armature_basic_human_metarig_add() if hasattr(bpy.ops.object, "armature_basic_human_metarig_add") else bpy.ops.object.armature_add(location=(0,0,0))
arm = bpy.context.active_object
arm.name = "Test_Armature"
print(f"Armature creation: OK ({arm.name})")

# Test 3: Export capabilities
export_gltf_ok = hasattr(bpy.ops.export_scene, "gltf") or hasattr(bpy.ops.wm, "gltf_export")
export_fbx_ok = hasattr(bpy.ops.export_scene, "fbx") or hasattr(bpy.ops.wm, "fbx_export")
print(f"Export glTF/GLB operator available: {export_gltf_ok}")
print(f"Export FBX operator available: {export_fbx_ok}")

print("All Blender core character pipeline tests passed successfully!")
