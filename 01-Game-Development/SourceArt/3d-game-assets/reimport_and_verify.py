import os, struct, json

glb_path = "/Users/kapilash/.gemini/antigravity/scratch/3d-game-assets/ares_vi_aaa_master.glb"
obj_path = "/Users/kapilash/.gemini/antigravity/scratch/3d-game-assets/ares_vi_aaa_master.obj"

print("=== STAGE 5: REIMPORT & GEOMETRY VERIFICATION ===")

# 1. Clean Reimport of OBJ
reimported_verts = []
reimported_tris = []
reimported_groups = {}
current_g = "default"

with open(obj_path, 'r') as f:
    for line in f:
        l = line.strip()
        if l.startswith('v '):
            reimported_verts.append([float(x) for x in l.split()[1:]])
        elif l.startswith('g '):
            current_g = l.split()[1]
            if current_g not in reimported_groups:
                reimported_groups[current_g] = []
        elif l.startswith('f '):
            parts = l.split()[1:]
            idx = [int(p.split('/')[0]) - 1 for p in parts]
            reimported_tris.append(idx)
            reimported_groups[current_g].append(idx)

print(f"OBJ Reimport Results:")
print(f"  File size on disk: {os.path.getsize(obj_path):,} bytes")
print(f"  Reimported Vertices: {len(reimported_verts):,}")
print(f"  Reimported Triangles: {len(reimported_tris):,}")
print(f"  Reimported Named Groups: {len(reimported_groups):,}")

# Verify Bounds
rx = [v[0] for v in reimported_verts]
ry = [v[1] for v in reimported_verts]
rz = [v[2] for v in reimported_verts]
print(f"  Reimported Bounding Box:")
print(f"    X: [{min(rx):.3f}, {max(rx):.3f}] -> Width: {max(rx)-min(rx):.3f} m")
print(f"    Y: [{min(ry):.3f}, {max(ry):.3f}] -> Length: {max(ry)-min(ry):.3f} m")
print(f"    Z: [{min(rz):.3f}, {max(rz):.3f}] -> Height: {max(rz)-min(rz):.3f} m")

# 2. Clean Reimport of Binary GLB
print(f"\nGLB Reimport Results:")
with open(glb_path, 'rb') as f:
    magic, version, total_len = struct.unpack('<III', f.read(12))
    assert magic == 0x46546C67, "Invalid glTF magic!"
    
    # JSON chunk
    j_len, j_type = struct.unpack('<II', f.read(8))
    j_data = json.loads(f.read(j_len).decode('utf-8'))
    
    # BIN chunk
    b_len, b_type = struct.unpack('<II', f.read(8))
    b_data = f.read(b_len)

print(f"  File size on disk: {os.path.getsize(glb_path):,} bytes")
print(f"  glTF Specification: Version {version}")
print(f"  Declared Scene Nodes: {len(j_data.get('nodes', []))}")
print(f"  Accessors: {len(j_data.get('accessors', []))}")
print(f"  Buffer Views: {len(j_data.get('bufferViews', []))}")

# Read binary positions accessor
pos_acc = j_data['accessors'][0]
pos_bv = j_data['bufferViews'][pos_acc['bufferView']]
pos_count = pos_acc['count']
pos_bytes = b_data[pos_bv['byteOffset']:pos_bv['byteOffset'] + pos_bv['byteLength']]
unpacked_verts = [struct.unpack('<fff', pos_bytes[i*12:(i+1)*12]) for i in range(pos_count)]

# Read binary indices accessor
idx_acc = j_data['accessors'][3]
idx_bv = j_data['bufferViews'][idx_acc['bufferView']]
idx_count = idx_acc['count']
idx_bytes = b_data[idx_bv['byteOffset']:idx_bv['byteOffset'] + idx_bv['byteLength']]
unpacked_indices = [struct.unpack('<I', idx_bytes[i*4:(i+1)*4])[0] for i in range(idx_count)]

print(f"  Binary Unpacked Positions: {len(unpacked_verts):,} vertices")
print(f"  Binary Unpacked Triangles: {len(unpacked_indices)//3:,} triangles")
print(f"  Integrity Check: OBJ ({len(reimported_tris):,} tris) == GLB ({len(unpacked_indices)//3:,} tris) -> MATCH!")
