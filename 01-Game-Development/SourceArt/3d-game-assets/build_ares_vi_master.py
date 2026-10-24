"""
ARES-VI Ultra-Detailed Anti-Gravity Mars Research Rover
Master 3D Hard-Surface Procedural Mesh Generator
Generates:
  1. ares_vi_master.obj (Standard Wavefront OBJ with full hierarchical groupings)
  2. ares_vi_master.mtl (Complete PBR Material Definitions)
  3. ares_vi_studio_data.json (High-performance mesh data for Interactive 3D Viewer with animation & exploded offsets)
"""

import math
import json
import os

def create_ares_vi():
    all_vertices = []
    all_normals = []
    v_map = {}
    vn_map = {}
    
    subsystems = {}
    
    def get_v_idx(pt):
        k = (round(pt[0], 4), round(pt[1], 4), round(pt[2], 4))
        if k not in v_map:
            all_vertices.append(k)
            v_map[k] = len(all_vertices)
        return v_map[k]

    def get_vn_idx(norm):
        k = (round(norm[0], 3), round(norm[1], 3), round(norm[2], 3))
        if k not in vn_map:
            all_normals.append(k)
            vn_map[k] = len(all_normals)
        return vn_map[k]

    def add_quad(part_id, mat_id, p1, p2, p3, p4, norm=None, explode=(0,0,0), hover=(0,0,0)):
        if norm is None:
            # calculate normal
            ax, ay, az = p2[0]-p1[0], p2[1]-p1[1], p2[2]-p1[2]
            bx, by, bz = p3[0]-p1[0], p3[1]-p1[1], p3[2]-p1[2]
            nx, ny, nz = ay*bz - az*by, az*bx - ax*bz, ax*by - ay*bx
            l = math.sqrt(nx*nx + ny*ny + nz*nz) or 1
            norm = (nx/l, ny/l, nz/l)
        
        v1 = get_v_idx(p1)
        v2 = get_v_idx(p2)
        v3 = get_v_idx(p3)
        v4 = get_v_idx(p4)
        ni = get_vn_idx(norm)
        
        if part_id not in subsystems:
            subsystems[part_id] = {
                'material': mat_id,
                'quads': [],
                'tris': [],
                'explode': explode,
                'hover_delta': hover
            }
        subsystems[part_id]['quads'].append((v1, v2, v3, v4, ni))

    def add_tri(part_id, mat_id, p1, p2, p3, norm=None, explode=(0,0,0), hover=(0,0,0)):
        if norm is None:
            ax, ay, az = p2[0]-p1[0], p2[1]-p1[1], p2[2]-p1[2]
            bx, by, bz = p3[0]-p1[0], p3[1]-p1[1], p3[2]-p1[2]
            nx, ny, nz = ay*bz - az*by, az*bx - ax*bz, ax*by - ay*bx
            l = math.sqrt(nx*nx + ny*ny + nz*nz) or 1
            norm = (nx/l, ny/l, nz/l)
            
        v1 = get_v_idx(p1)
        v2 = get_v_idx(p2)
        v3 = get_v_idx(p3)
        ni = get_vn_idx(norm)
        
        if part_id not in subsystems:
            subsystems[part_id] = {
                'material': mat_id,
                'quads': [],
                'tris': [],
                'explode': explode,
                'hover_delta': hover
            }
        subsystems[part_id]['tris'].append((v1, v2, v3, ni))

    # Helper: Box
    def add_box(part_id, mat_id, x0, x1, y0, y1, z0, z1, explode=(0,0,0), hover=(0,0,0)):
        # 6 faces
        add_quad(part_id, mat_id, (x0, y1, z0), (x1, y1, z0), (x1, y1, z1), (x0, y1, z1), (0, 1, 0), explode, hover) # Front
        add_quad(part_id, mat_id, (x1, y0, z0), (x0, y0, z0), (x0, y0, z1), (x1, y0, z1), (0, -1, 0), explode, hover) # Back
        add_quad(part_id, mat_id, (x0, y0, z0), (x0, y1, z0), (x0, y1, z1), (x0, y0, z1), (-1, 0, 0), explode, hover) # Left
        add_quad(part_id, mat_id, (x1, y1, z0), (x1, y0, z0), (x1, y0, z1), (x1, y1, z1), (1, 0, 0), explode, hover) # Right
        add_quad(part_id, mat_id, (x0, y0, z1), (x1, y0, z1), (x1, y1, z1), (x0, y1, z1), (0, 0, 1), explode, hover) # Top
        add_quad(part_id, mat_id, (x0, y1, z0), (x1, y1, z0), (x1, y0, z0), (x0, y0, z0), (0, 0, -1), explode, hover) # Bottom

    # Helper: Cylinder along Axis
    def add_cylinder(part_id, mat_id, axis, c1, c2, c3, r, length, segs=8, explode=(0,0,0), hover=(0,0,0)):
        step = 2 * math.pi / segs
        pts_a, pts_b = [], []
        for i in range(segs):
            ang = i * step
            ca = r * math.cos(ang)
            sa = r * math.sin(ang)
            if axis == 'X':
                pts_a.append((c1 - length/2, c2 + ca, c3 + sa))
                pts_b.append((c1 + length/2, c2 + ca, c3 + sa))
            elif axis == 'Y':
                pts_a.append((c1 + ca, c2 - length/2, c3 + sa))
                pts_b.append((c1 + ca, c2 + length/2, c3 + sa))
            else:
                pts_a.append((c1 + ca, c2 + sa, c3 - length/2))
                pts_b.append((c1 + ca, c2 + sa, c3 + length/2))
        
        # side walls
        for i in range(segs):
            next_i = (i + 1) % segs
            p_a1, p_a2 = pts_a[i], pts_a[next_i]
            p_b1, p_b2 = pts_b[i], pts_b[next_i]
            ang_mid = (i + 0.5) * step
            cm = math.cos(ang_mid)
            sm = math.sin(ang_mid)
            if axis == 'X': norm = (0, cm, sm)
            elif axis == 'Y': norm = (cm, 0, sm)
            else: norm = (cm, sm, 0)
            add_quad(part_id, mat_id, p_a1, p_b1, p_b2, p_a2, norm, explode, hover)
            
        # end caps
        if axis == 'X':
            c_a, c_b = (c1 - length/2, c2, c3), (c1 + length/2, c2, c3)
            norm_a, norm_b = (-1, 0, 0), (1, 0, 0)
        elif axis == 'Y':
            c_a, c_b = (c1, c2 - length/2, c3), (c1, c2 + length/2, c3)
            norm_a, norm_b = (0, -1, 0), (0, 1, 0)
        else:
            c_a, c_b = (c1, c2, c3 - length/2), (c1, c2, c3 + length/2)
            norm_a, norm_b = (0, 0, -1), (0, 0, 1)

        for i in range(segs):
            next_i = (i + 1) % segs
            add_tri(part_id, mat_id, c_a, pts_a[next_i], pts_a[i], norm_a, explode, hover)
            add_tri(part_id, mat_id, c_b, pts_b[i], pts_b[next_i], norm_b, explode, hover)

    # =========================================================================
    # 1. CENTRAL HULL & AEROSPACE COMPOSITE ARMOR
    # =========================================================================
    # Lower chassis core (Titanium structural spine)
    add_box("ChassisSpine", "Mat_TitaniumFrame", -0.55, 0.55, -1.10, 1.05, 0.50, 0.72, explode=(0,0,-0.4))
    
    # Main Ceramic Armor Hull (Angled stealth/aerospace chamfered hull)
    add_box("MainHull", "Mat_CeramicWhite", -0.72, 0.72, -0.90, 0.85, 0.72, 1.18, explode=(0,0,0.1))
    
    # Rear Avionics & Sensor Bay module
    add_box("AvionicsModule", "Mat_GraphiteTrim", -0.65, 0.65, -1.25, -0.85, 0.75, 1.25, explode=(0,-0.5,0.2))

    # Martian Orange Safety Trim & Armor Strakes (High-visibility aerospace livery)
    add_box("SafetyStrakesL", "Mat_MartianOrange", -0.75, -0.71, -0.85, 0.75, 0.88, 0.98, explode=(-0.4,0,0))
    add_box("SafetyStrakesR", "Mat_MartianOrange",  0.71,  0.75, -0.85, 0.75, 0.88, 0.98, explode=( 0.4,0,0))
    add_box("FrontBumperStrake", "Mat_MartianOrange", -0.60, 0.60, 1.05, 1.15, 0.55, 0.65, explode=(0,0.5,0))

    # Heavy-duty Bumper & Hazard Winch Assembly (Titanium Bullbar)
    add_box("FrontPushBar", "Mat_TitaniumFrame", -0.75, 0.75, 1.08, 1.18, 0.65, 0.82, explode=(0,0.6,-0.1))

    # =========================================================================
    # 2. COCKPIT & CREW MODULE (Astronaut-Scale Panoramic Command Pod)
    # =========================================================================
    # Panoramic Front Windshield / Canopy (Polarized gold/tinted glass)
    # Angled windshield from y=0.45 to y=0.85, z=1.18 to 1.62
    c_p1 = (-0.58, 0.45, 1.62)
    c_p2 = ( 0.58, 0.45, 1.62)
    c_p3 = ( 0.62, 0.88, 1.18)
    c_p4 = (-0.62, 0.88, 1.18)
    add_quad("CockpitCanopy", "Mat_CanopyGlass", c_p1, c_p2, c_p3, c_p4, explode=(0,0.4,0.5))

    # Cockpit Roof canopy section
    add_box("CockpitRoof", "Mat_CeramicWhite", -0.58, 0.58, -0.20, 0.45, 1.58, 1.68, explode=(0,0,0.6))
    
    # Cockpit Side Windows (Left & Right)
    add_quad("CanopySideL", "Mat_CanopyGlass", (-0.58, -0.20, 1.58), (-0.58, 0.45, 1.58), (-0.62, 0.88, 1.18), (-0.62, -0.20, 1.18), explode=(-0.4,0.2,0.4))
    add_quad("CanopySideR", "Mat_CanopyGlass", ( 0.58, 0.45, 1.58), ( 0.58, -0.20, 1.58), ( 0.62, -0.20, 1.18), ( 0.62, 0.88, 1.18), explode=( 0.4,0.2,0.4))

    # Cockpit Interior: Dual Astronaut Seats (Pilot & Science Specialist)
    add_box("SeatPilot", "Mat_GraphiteTrim", -0.42, -0.10, 0.05, 0.35, 0.82, 1.35, explode=(0,0,0.2))
    add_box("SeatSpecialist", "Mat_GraphiteTrim", 0.10, 0.42, 0.05, 0.35, 0.82, 1.35, explode=(0,0,0.2))
    
    # Digital Flight Dash & Navigation Instrument Cluster
    add_box("FlightDashboard", "Mat_TitaniumFrame", -0.52, 0.52, 0.52, 0.72, 1.05, 1.25, explode=(0,0.2,0.2))
    # Glowing Holographic Nav/Science Displays
    add_box("NavDisplayCenter", "Mat_CyanEnergy", -0.20, 0.20, 0.68, 0.70, 1.16, 1.28, explode=(0,0.2,0.2))
    add_box("HudDisplayL", "Mat_CyanEnergy", -0.45, -0.25, 0.68, 0.70, 1.14, 1.24, explode=(0,0.2,0.2))
    add_box("HudDisplayR", "Mat_CyanEnergy",  0.25,  0.45, 0.68, 0.70, 1.14, 1.24, explode=(0,0.2,0.2))

    # =========================================================================
    # 3. ANTI-GRAVITY PROPULSION SYSTEM (Mode B - 4 Primary + 2 Aux Stabilizers)
    # =========================================================================
    # In Hover Mode: Emitter covers retract, vectoring rings extend downwards,
    # and segmented electromagnetic coils emit intense cyan-white plasma glow!
    
    # 4 Primary Underbody Hover Modules (FL, FR, RL, RR)
    hover_coords = [
        (-0.55,  0.65, "HoverEmitter_FL", (-0.6, 0.4, -0.5)),
        ( 0.55,  0.65, "HoverEmitter_FR", ( 0.6, 0.4, -0.5)),
        (-0.55, -0.65, "HoverEmitter_RL", (-0.6,-0.4, -0.5)),
        ( 0.55, -0.65, "HoverEmitter_RR", ( 0.6,-0.4, -0.5)),
    ]
    for hx, hy, hid, hexplode in hover_coords:
        # Hover Nacelle housing
        add_box(f"{hid}_Housing", "Mat_TitaniumFrame", hx-0.18, hx+0.18, hy-0.18, hy+0.18, 0.42, 0.55, explode=hexplode, hover=(0,0,-0.08))
        # Concentric Electromagnetic Emitter Ring (Glowing Cyan Energy)
        add_cylinder(f"{hid}_EmitterRing", "Mat_CyanEnergy", 'Z', hx, hy, 0.40, r=0.16, length=0.06, segs=10, explode=hexplode, hover=(0,0,-0.10))
        # Central Anti-Grav Graviton Focus Core
        add_cylinder(f"{hid}_FocusCore", "Mat_CeramicWhite", 'Z', hx, hy, 0.41, r=0.07, length=0.08, segs=8, explode=hexplode, hover=(0,0,-0.10))
        # Vectoring gimbal vanes (4 aerodynamic directional fins per module)
        add_box(f"{hid}_VaneX", "Mat_GraphiteTrim", hx-0.16, hx+0.16, hy-0.02, hy+0.02, 0.35, 0.42, explode=hexplode, hover=(0,0,-0.12))
        add_box(f"{hid}_VaneY", "Mat_GraphiteTrim", hx-0.02, hx+0.02, hy-0.16, hy+0.16, 0.35, 0.42, explode=hexplode, hover=(0,0,-0.12))

    # 2 Auxiliary Lateral Stabilization Emitters (Mid Left & Mid Right)
    add_cylinder("AuxStabilizer_ML", "Mat_CyanEnergy", 'X', -0.68, 0.0, 0.52, r=0.12, length=0.08, segs=8, explode=(-0.6, 0, -0.2), hover=(-0.08,0,-0.05))
    add_cylinder("AuxStabilizer_MR", "Mat_CyanEnergy", 'X',  0.68, 0.0, 0.52, r=0.12, length=0.08, segs=8, explode=( 0.6, 0, -0.2), hover=( 0.08,0,-0.05))

    # =========================================================================
    # 4. ROCKER-BOGIE SUSPENSION & 6 HEAVY-DUTY WHEELS (Mode A - Ground Exploration)
    # =========================================================================
    # Rocker Differential Pivot Hubs & Main Titanium Booms
    for sx, side in [(-1, "Left"), (1, "Right")]:
        px = sx * 0.72
        ex_side = (sx * 0.5, 0, -0.2)
        
        # Differential Pivot Joint
        add_cylinder(f"SuspensionPivot_{side}", "Mat_TitaniumFrame", 'X', px, 0.0, 0.76, r=0.09, length=0.14, segs=8, explode=ex_side)
        
        # Rocker Strut (Pivots to front bogie and rear link)
        add_box(f"RockerStrut_{side}", "Mat_TitaniumFrame", px - 0.06, px + 0.06, -0.15, 0.15, 0.68, 0.84, explode=ex_side)
        
        # Bogie Bridge front link
        add_box(f"BogieFrontLink_{side}", "Mat_TitaniumFrame", sx*1.02, sx*1.12, 0.0, 1.15, 0.38, 0.48, explode=ex_side, hover=(0,0,0.15))
        # Rear suspension trailing arm
        add_box(f"RearTrailingArm_{side}", "Mat_TitaniumFrame", sx*1.02, sx*1.12, -1.15, 0.0, 0.38, 0.48, explode=ex_side, hover=(0,0,0.15))

    # 6 Exploration Wheels with In-Hub Brushless Motors & Deep Martian Cleats
    wheels_meta = [
        (-1.22,  1.15, 0.38, "Wheel_FL", (-0.7,  0.5, -0.2)),
        (-1.22,  0.00, 0.38, "Wheel_ML", (-0.7,  0.0, -0.2)),
        (-1.22, -1.15, 0.38, "Wheel_RL", (-0.7, -0.5, -0.2)),
        ( 1.22,  1.15, 0.38, "Wheel_FR", ( 0.7,  0.5, -0.2)),
        ( 1.22,  0.00, 0.38, "Wheel_MR", ( 0.7,  0.0, -0.2)),
        ( 1.22, -1.15, 0.38, "Wheel_RR", ( 0.7, -0.5, -0.2)),
    ]
    for wx, wy, wz, wid, wexplode in wheels_meta:
        # Main Carbon/Rubber Tire tread (Wide all-terrain Martian wheel)
        # In hover mode, wheels lift/retract up by +0.18m
        hover_lift = (0, 0, 0.18)
        add_cylinder(f"{wid}_Tire", "Mat_CarbonTreads", 'X', wx, wy, wz, r=0.36, length=0.30, segs=12, explode=wexplode, hover=hover_lift)
        
        # Armored In-Hub Motor & Planetary Gearbox
        add_cylinder(f"{wid}_HubMotor", "Mat_TitaniumFrame", 'X', wx, wy, wz, r=0.18, length=0.32, segs=10, explode=wexplode, hover=hover_lift)
        
        # Martian Orange Center Hub Nut & Diagnostic Ring
        add_cylinder(f"{wid}_CenterCap", "Mat_MartianOrange", 'X', wx + (0.16 if wx > 0 else -0.16), wy, wz, r=0.08, length=0.04, segs=8, explode=wexplode, hover=hover_lift)

    # =========================================================================
    # 5. SCIENTIFIC RESEARCH EQUIPMENT (ChemCam, Arm, MMRTG, Antenna)
    # =========================================================================
    
    # A. ChemCam Remote Sensing Mast (Front Right Deck, Elevating)
    mast_ex = (0.3, 0.3, 0.5)
    # Mast Base & Heavy-Duty Azimuth Swivel Turret
    add_cylinder("MastBaseTurret", "Mat_TitaniumFrame", 'Z', 0.40, 0.65, 1.22, r=0.10, length=0.12, segs=8, explode=mast_ex)
    # Telescoping Structural Mast Column
    add_cylinder("MastPillar", "Mat_GraphiteTrim", 'Z', 0.40, 0.65, 1.75, r=0.055, length=0.96, segs=8, explode=mast_ex)
    # Elevation Tilt Gimbal
    add_box("MastGimbal", "Mat_TitaniumFrame", 0.32, 0.48, 0.58, 0.72, 2.22, 2.34, explode=mast_ex)
    # ChemCam Sensor Housing Head
    add_box("ChemCamHead", "Mat_CeramicWhite", 0.25, 0.55, 0.52, 0.82, 2.34, 2.62, explode=mast_ex)
    
    # Optical Lenses:
    # 1. Primary Laser Induced Breakdown Spectrometer (LIBS) Lens - Cyan Halo
    add_cylinder("ChemCamLaserLens", "Mat_CyanEnergy", 'Y', 0.40, 0.83, 2.50, r=0.08, length=0.04, segs=8, explode=mast_ex)
    # 2. Stereo Navigation MastCams (Left & Right)
    add_cylinder("NavCamL", "Mat_CyanEnergy", 'Y', 0.29, 0.83, 2.40, r=0.045, length=0.03, segs=6, explode=mast_ex)
    add_cylinder("NavCamR", "Mat_CyanEnergy", 'Y', 0.51, 0.83, 2.40, r=0.045, length=0.03, segs=6, explode=mast_ex)
    # 3. Microscopic Imager Window
    add_cylinder("MicroImager", "Mat_CyanEnergy", 'Y', 0.40, 0.83, 2.37, r=0.03, length=0.02, segs=6, explode=mast_ex)

    # B. Articulated Robotic Science Arm (Front Left Deck)
    arm_ex = (-0.4, 0.5, 0.3)
    # Shoulder Turret (Rotating)
    add_cylinder("ArmShoulder", "Mat_TitaniumFrame", 'Z', -0.42, 0.80, 1.15, r=0.11, length=0.14, segs=8, explode=arm_ex)
    # Upper Arm Boom (Gold Thermal Foil insulation)
    add_box("ArmUpperBoom", "Mat_GoldFoil", -0.46, -0.38, 0.80, 1.35, 1.05, 1.18, explode=arm_ex)
    # Articulated Elbow Actuator
    add_cylinder("ArmElbow", "Mat_TitaniumFrame", 'X', -0.42, 1.36, 1.11, r=0.08, length=0.12, segs=8, explode=arm_ex)
    # Forearm Boom (Descending toward terrain)
    add_box("ArmForearm", "Mat_GoldFoil", -0.45, -0.39, 1.32, 1.42, 0.65, 1.11, explode=arm_ex)
    # Wrist 2-Axis Gimbal
    add_cylinder("ArmWrist", "Mat_TitaniumFrame", 'Z', -0.42, 1.37, 0.60, r=0.07, length=0.10, segs=8, explode=arm_ex)
    # Rotary Rock Core Drill & Planetary Spectrometer Turret
    add_cylinder("ScienceDrillTurret", "Mat_GraphiteTrim", 'Z', -0.42, 1.37, 0.45, r=0.13, length=0.18, segs=8, explode=arm_ex)
    # Tungsten-Carbide Coring Drill Bit
    add_cylinder("CoringDrillBit", "Mat_TitaniumFrame", 'Z', -0.42, 1.37, 0.28, r=0.035, length=0.16, segs=6, explode=arm_ex)

    # C. Rear Multi-Mission Radioisotope Thermoelectric Generator (MMRTG)
    rtg_ex = (0, -0.6, 0.1)
    # Main Cylindrical Core (Nuclear thermoelectric cell)
    add_cylinder("MMRTG_Core", "Mat_GraphiteTrim", 'Y', 0.0, -1.25, 1.05, r=0.22, length=0.55, segs=10, explode=rtg_ex)
    # Radiative Heat Dissipation Cooling Fins (4 cruciform panels)
    add_box("MMRTG_FinsH", "Mat_TitaniumFrame", -0.44, 0.44, -1.50, -1.00, 1.03, 1.07, explode=rtg_ex)
    add_box("MMRTG_FinsV", "Mat_TitaniumFrame", -0.02, 0.02, -1.50, -1.00, 0.81, 1.29, explode=rtg_ex)
    # Thermal Warning Stripe
    add_box("MMRTG_Stripe", "Mat_MartianOrange", -0.23, 0.23, -1.05, -1.01, 1.24, 1.28, explode=rtg_ex)

    # D. High-Gain Communications Dish Antenna (Deep Space Earth Comms)
    ant_ex = (-0.3, -0.4, 0.6)
    # Mast Strut
    add_cylinder("AntennaMast", "Mat_TitaniumFrame", 'Z', -0.38, -0.62, 1.45, r=0.04, length=0.45, segs=6, explode=ant_ex)
    # Parabolic Hexagonal Dish (Gold thermal reflective mesh, tilted back 35 deg)
    dish_cx, dish_cy, dish_cz = -0.38, -0.65, 1.82
    dish_r = 0.32
    hex_pts = []
    for i in range(6):
        ang = i * math.pi / 3
        hx = dish_cx + dish_r * math.cos(ang)
        hy = dish_cy - dish_r * math.sin(ang) * 0.75
        hz = dish_cz + dish_r * math.sin(ang) * 0.75
        hex_pts.append((hx, hy, hz))
    dish_center = (dish_cx, dish_cy + 0.08, dish_cz - 0.06)
    for i in range(6):
        next_i = (i + 1) % 6
        add_tri("AntennaDish", "Mat_GoldFoil", dish_center, hex_pts[i], hex_pts[next_i], (0, 0.8, -0.6), explode=ant_ex)
    # Subreflector Feed Horn
    add_cylinder("AntennaSubreflector", "Mat_TitaniumFrame", 'Y', dish_cx, dish_cy + 0.16, dish_cz - 0.05, r=0.035, length=0.14, segs=6, explode=ant_ex)

    # =========================================================================
    # 6. EXPORT WAVEFRONT OBJ & MTL
    # =========================================================================
    base_dir = "/Users/kapilash/.gemini/antigravity/scratch/3d-game-assets"
    obj_path = os.path.join(base_dir, "ares_vi_master.obj")
    mtl_path = os.path.join(base_dir, "ares_vi_master.mtl")
    json_path = os.path.join(base_dir, "ares_vi_studio_data.json")

    # Generate MTL
    mtl_content = """# ARES-VI Anti-Gravity Mars Research Rover PBR Materials
# Master Material Definition

newmtl Mat_CeramicWhite
Ka 0.25 0.26 0.28
Kd 0.92 0.94 0.96
Ks 0.45 0.48 0.50
Ns 40.0
d 1.0
illum 2

newmtl Mat_GraphiteTrim
Ka 0.08 0.09 0.10
Kd 0.18 0.20 0.22
Ks 0.35 0.38 0.40
Ns 30.0
d 1.0
illum 2

newmtl Mat_TitaniumFrame
Ka 0.12 0.14 0.16
Kd 0.40 0.44 0.48
Ks 0.85 0.88 0.90
Ns 70.0
d 1.0
illum 2

newmtl Mat_MartianOrange
Ka 0.30 0.10 0.02
Kd 0.98 0.35 0.05
Ks 0.60 0.50 0.40
Ns 50.0
d 1.0
illum 2

newmtl Mat_GoldFoil
Ka 0.28 0.20 0.05
Kd 0.95 0.78 0.16
Ks 0.98 0.88 0.45
Ns 90.0
d 1.0
illum 2

newmtl Mat_CarbonTreads
Ka 0.05 0.05 0.06
Kd 0.12 0.13 0.15
Ks 0.20 0.20 0.22
Ns 20.0
d 1.0
illum 2

newmtl Mat_CanopyGlass
Ka 0.10 0.15 0.20
Kd 0.20 0.35 0.45
Ks 0.95 0.98 1.00
Ns 120.0
d 0.55
illum 2

newmtl Mat_CyanEnergy
Ka 0.00 0.60 0.80
Kd 0.10 0.85 1.00
Ks 0.95 0.98 1.00
Ke 0.00 0.92 1.00
Ns 150.0
d 0.95
illum 2
"""
    with open(mtl_path, "w") as f:
        f.write(mtl_content)

    # Generate OBJ
    with open(obj_path, "w") as f:
        f.write("# ARES-VI Advanced Anti-Gravity Mars Research Rover (AAA Master Model)\n")
        f.write("mtllib ares_vi_master.mtl\n\n")
        for v in all_vertices:
            f.write(f"v {v[0]:.4f} {v[1]:.4f} {v[2]:.4f}\n")
        f.write("\n")
        for vn in all_normals:
            f.write(f"vn {vn[0]:.4f} {vn[1]:.4f} {vn[2]:.4f}\n")
        f.write("\n")

        for part_id, data in subsystems.items():
            f.write(f"g {part_id}\n")
            f.write(f"usemtl {data['material']}\n")
            for q in data['quads']:
                f.write(f"f {q[0]}//{q[4]} {q[1]}//{q[4]} {q[2]}//{q[4]} {q[3]}//{q[4]}\n")
            for t in data['tris']:
                f.write(f"f {t[0]}//{t[3]} {t[1]}//{t[3]} {t[2]}//{t[3]}\n")

    # Generate JSON for High-Performance Interactive Viewer
    # Re-map quad and tri indices to 0-based
    json_subsystems = {}
    for part_id, data in subsystems.items():
        q_faces = [[q[0]-1, q[1]-1, q[2]-1, q[3]-1] for q in data['quads']]
        t_faces = [[t[0]-1, t[1]-1, t[2]-1] for t in data['tris']]
        json_subsystems[part_id] = {
            'material': data['material'],
            'faces': q_faces + t_faces,
            'explode': data['explode'],
            'hover_delta': data['hover_delta']
        }

    export_json = {
        'vertices': all_vertices,
        'subsystems': json_subsystems,
        'stats': {
            'total_vertices': len(all_vertices),
            'total_normals': len(all_normals),
            'subsystems_count': len(subsystems),
            'total_faces': sum(len(s['faces']) for s in json_subsystems.values())
        }
    }
    with open(json_path, "w") as f:
        json.dump(export_json, f)

    print(f"Master Rover Successfully Created!")
    print(f"Vertices: {len(all_vertices)}, Normals: {len(all_normals)}, Faces: {export_json['stats']['total_faces']}")
    print(f"Subsystems: {len(subsystems)}")

create_ares_vi()
