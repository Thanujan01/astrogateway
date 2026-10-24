# ARES EVA MK-I v3 — 2048x2048 PBR Texture Authoring & Baking Suite

import numpy as np
import bpy
import os
import time

print("=== STARTING 2048x2048 PBR TEXTURE GENERATION ===")
t0 = time.time()

tex_dir = "/Users/kapilash/.gemini/antigravity/scratch/ARES_EVA_MK1_v3/textures"
os.makedirs(tex_dir, exist_ok=True)

w, h = 2048, 2048

# Coordinate grids for procedural spatial texturing
u = np.linspace(0.0, 1.0, w, dtype=np.float32)
v = np.linspace(0.0, 1.0, h, dtype=np.float32)
uu, vv = np.meshgrid(u, v)

# Base Color Buffer (RGBA float32)
base_color = np.ones((h, w, 4), dtype=np.float32)
base_color[:, :, 0] = 0.898  # Ceramic White #E5E6E2
base_color[:, :, 1] = 0.902
base_color[:, :, 2] = 0.886
base_color[:, :, 3] = 1.0

# 1. Micro-weave Ortho-fabric texture
weave_x = np.sin(uu * 2500.0) * 0.02
weave_y = np.cos(vv * 2500.0) * 0.02
base_color[:, :, :3] += (weave_x + weave_y)[:, :, None]

# 2. Graphite Nomex Panels (Pelvic brief, joints, backpack shroud)
graphite_mask = (
    ((uu > 0.0) & (uu < 0.4) & (vv > 0.5) & (vv < 0.6)) |  # PLSS shroud
    ((uu > 0.4) & (uu < 0.8) & (vv > 0.6) & (vv < 0.7)) |  # Pelvic brief
    ((uu > 0.45) & (uu < 0.75) & (vv > 0.22) & (vv < 0.32)) | # Elbow bellows
    ((vv > 0.08) & (vv < 0.14)) # Knee bellows
)
base_color[graphite_mask, 0] = 0.145  # #252B32
base_color[graphite_mask, 1] = 0.169
base_color[graphite_mask, 2] = 0.196
# Diamond ripstop grid on graphite
ripstop = (np.sin((uu + vv) * 800.0) * np.sin((uu - vv) * 800.0) > 0.35) & graphite_mask
base_color[ripstop, :3] += 0.035

# 3. Gold-Vapor Sun Visor
gold_mask = (uu > 0.08) & (uu < 0.32) & (vv > 0.72) & (vv < 0.94)
base_color[gold_mask, 0] = 0.950  # #F2C426
base_color[gold_mask, 1] = 0.780
base_color[gold_mask, 2] = 0.160

# 4. Titanium Hardware (Neck ring, bearings, O2 tanks, DCM housing, cleats)
titanium_mask = (
    ((uu > 0.82) & (vv > 0.2)) |  # Hardware atlas
    ((uu > 0.15) & (uu < 0.38) & (vv > 0.25) & (vv < 0.48)) | # O2 tanks
    ((uu > 0.50) & (uu < 0.70) & (vv > 0.72) & (vv < 0.84)) # DCM frame
)
base_color[titanium_mask, 0] = 0.537  # #89939B
base_color[titanium_mask, 1] = 0.576
base_color[titanium_mask, 2] = 0.608

# 5. Safety Orange Aerospace Mission Strakes
orange_mask = (
    ((uu > 0.42) & (uu < 0.46) & (vv > 0.70) & (vv < 0.92)) |
    ((uu > 0.74) & (uu < 0.78) & (vv > 0.70) & (vv < 0.92))
)
base_color[orange_mask, 0] = 0.851  # #D96A29
base_color[orange_mask, 1] = 0.416
base_color[orange_mask, 2] = 0.161

# 6. Restrained Mars Regolith Dust Accumulation
# Heavy dust on boots (vv < 0.06), fading up through shins (vv < 0.18)
dust_mask = np.clip(1.0 - (vv / 0.18), 0.0, 1.0) ** 1.8
# Regolith noise variation
dust_noise = 0.5 + 0.5 * np.sin(uu * 120.0 + vv * 90.0) * np.cos(uu * 240.0)
dust_factor = (dust_mask * dust_noise)[:, :, None]
mars_dust_color = np.array([0.72, 0.32, 0.18], dtype=np.float32) # Mars iron-oxide red
base_color[:, :, :3] = base_color[:, :, :3] * (1.0 - dust_factor * 0.75) + mars_dust_color * (dust_factor * 0.75)

# Clip to 0-1
base_color = np.clip(base_color, 0.0, 1.0)

# ----------------- NORMAL MAP (Tangent Space OpenGL Y+) -----------------
# Flat normal is (0.5, 0.5, 1.0) -> RGB (128, 128, 255)
normal_map = np.zeros((h, w, 4), dtype=np.float32)
normal_map[:, :, 0] = 0.5
normal_map[:, :, 1] = 0.5
normal_map[:, :, 2] = 1.0
normal_map[:, :, 3] = 1.0

# Seam ridges (horizontal & vertical panel seams)
seam_grid = (np.abs(np.sin(uu * 16.0 * np.pi)) < 0.04) | (np.abs(np.sin(vv * 16.0 * np.pi)) < 0.04)
normal_map[seam_grid, 0] += 0.12 * np.cos(uu[seam_grid] * 32.0 * np.pi)
normal_map[seam_grid, 1] += 0.12 * np.sin(vv[seam_grid] * 32.0 * np.pi)

# Bellows ring normals (sinusoidal ridge on elbows and knees)
bellows_region = graphite_mask & (((vv > 0.08) & (vv < 0.14)) | ((vv > 0.22) & (vv < 0.32)))
normal_map[bellows_region, 1] += np.sin(vv[bellows_region] * 350.0) * 0.25

# Renormalize
n_len = np.sqrt((normal_map[:, :, 0] - 0.5)**2 + (normal_map[:, :, 1] - 0.5)**2 + normal_map[:, :, 2]**2)
normal_map[:, :, 0] = ((normal_map[:, :, 0] - 0.5) / n_len) * 0.5 + 0.5
normal_map[:, :, 1] = ((normal_map[:, :, 1] - 0.5) / n_len) * 0.5 + 0.5
normal_map[:, :, 2] = (normal_map[:, :, 2] / n_len)
normal_map = np.clip(normal_map, 0.0, 1.0)

# ----------------- METALLIC / SMOOTHNESS (Unity URP Mask Map) -----------------
# R: Metallic, G: AO, B: Detail/Dust, A: Smoothness (1 - Roughness)
met_smooth = np.zeros((h, w, 4), dtype=np.float32)
# R: Metallic
met_smooth[titanium_mask, 0] = 0.88
met_smooth[gold_mask, 0] = 0.98
met_smooth[orange_mask, 0] = 0.15
met_smooth[graphite_mask, 0] = 0.12
# G: Ambient Occlusion
met_smooth[:, :, 1] = 1.0 - (seam_grid * 0.35)
met_smooth[bellows_region, 1] -= np.abs(np.sin(vv[bellows_region] * 350.0)) * 0.25
# B: Dust Mask
met_smooth[:, :, 2] = dust_mask * dust_noise
# A: Smoothness
met_smooth[:, :, 3] = 0.40  # Fabric baseline (Roughness 0.60)
met_smooth[graphite_mask, 3] = 0.25 # Rougher Nomex
met_smooth[titanium_mask, 3] = 0.75 # Polished titanium
met_smooth[gold_mask, 3] = 0.95     # Glossy gold visor
met_smooth[bellows_region, 3] = 0.18 # Matte rubber
met_smooth = np.clip(met_smooth, 0.0, 1.0)

# ----------------- OCCLUSION MAP (Grayscale) -----------------
occlusion = np.ones((h, w, 4), dtype=np.float32)
occlusion[:, :, 0] = met_smooth[:, :, 1]
occlusion[:, :, 1] = met_smooth[:, :, 1]
occlusion[:, :, 2] = met_smooth[:, :, 1]
occlusion[:, :, 3] = 1.0

# ----------------- EMISSION MAP -----------------
emission = np.zeros((h, w, 4), dtype=np.float32)
# Cyan Headlamp LED Lenses (UU: 0.10 to 0.14 and 0.26 to 0.30 at VV: 0.88)
led_mask_l = (uu > 0.10) & (uu < 0.14) & (vv > 0.86) & (vv < 0.90)
led_mask_r = (uu > 0.26) & (uu < 0.30) & (vv > 0.86) & (vv < 0.90)
# Chest DCM Screen (UU: 0.54 to 0.66 at VV: 0.76 to 0.80)
dcm_screen_mask = (uu > 0.54) & (uu < 0.66) & (vv > 0.76) & (vv < 0.80)
# PLSS Rear Status Bar
plss_led_mask = (uu > 0.12) & (uu < 0.28) & (vv > 0.54) & (vv < 0.57)

emiss_active = led_mask_l | led_mask_r | dcm_screen_mask | plss_led_mask
emission[emiss_active, 0] = 0.000
emission[emiss_active, 1] = 0.788
emission[emiss_active, 2] = 0.910
emission[:, :, 3] = 1.0

# ----------------- DUST MASK (Grayscale) -----------------
dust_tex = np.zeros((h, w, 4), dtype=np.float32)
dust_tex[:, :, 0] = dust_mask * dust_noise
dust_tex[:, :, 1] = dust_mask * dust_noise
dust_tex[:, :, 2] = dust_mask * dust_noise
dust_tex[:, :, 3] = 1.0

# Save using Blender's high-speed Image API
def save_texture(name, np_data):
    img = bpy.data.images.new(name, width=w, height=h, alpha=True)
    # Convert RGBA from bottom-to-top for Blender texture alignment
    flipped = np.flipud(np_data)
    img.pixels.foreach_set(flipped.ravel())
    filepath = f"{tex_dir}/{name}.png"
    img.filepath_raw = filepath
    img.file_format = 'PNG'
    img.save()
    print(f"Baked {name}.png ({os.path.getsize(filepath):,} bytes)")

save_texture("ARES_EVA_BaseColor", base_color)
save_texture("ARES_EVA_Normal", normal_map)
save_texture("ARES_EVA_MetallicSmoothness", met_smooth)
save_texture("ARES_EVA_Occlusion", occlusion)
save_texture("ARES_EVA_Emission", emission)
save_texture("ARES_EVA_DustMask", dust_tex)

print(f"=== ALL 6 PBR 2048x2048 TEXTURES BAKED SUCCESSFULLY IN {time.time()-t0:.2f}s! ===")
