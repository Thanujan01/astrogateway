import math
import sys

# Test helical spring generator
def make_helical_spring(center_x, center_y, z0, z1, coil_radius, wire_radius, coils=6, segments_per_coil=12, wire_segs=4):
    vertices = []
    quads = []
    height = z1 - z0
    total_steps = int(coils * segments_per_coil)
    dz = height / total_steps
    d_theta = 2 * math.pi / segments_per_coil
    
    # generate rings of points along the helical spine
    rings = []
    for step in range(total_steps + 1):
        theta = step * d_theta
        cx = center_x + coil_radius * math.cos(theta)
        cy = center_y + coil_radius * math.sin(theta)
        cz = z0 + step * dz
        
        # tangent vector
        tx = -coil_radius * math.sin(theta)
        ty = coil_radius * math.cos(theta)
        tz = dz / d_theta
        t_len = math.sqrt(tx*tx + ty*ty + tz*tz) or 1
        tx, ty, tz = tx/t_len, ty/t_len, tz/t_len
        
        # normal vector pointing outward from coil center
        nx = math.cos(theta)
        ny = math.sin(theta)
        nz = 0
        
        # binormal
        bx = ty*nz - tz*ny
        by = tz*nx - tx*nz
        bz = tx*ny - ty*nx
        
        ring = []
        for w in range(wire_segs):
            phi = 2 * math.pi * w / wire_segs
            wx = cx + wire_radius * (nx * math.cos(phi) + bx * math.sin(phi))
            wy = cy + wire_radius * (ny * math.cos(phi) + by * math.sin(phi))
            wz = cz + wire_radius * (bz * math.sin(phi))
            ring.append((wx, wy, wz))
        rings.append(ring)
        
    return len(rings), len(rings) * wire_segs

rings_count, pts_count = make_helical_spring(0, 0, 0, 1.0, 0.08, 0.015, coils=7)
print(f"Helical spring test: {rings_count} rings, {pts_count} vertices")
