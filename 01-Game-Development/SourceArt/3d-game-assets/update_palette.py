import re

# Update render_aaa_views.py
with open('/Users/kapilash/.gemini/antigravity/scratch/3d-game-assets/render_aaa_views.py', 'r') as f:
    text = f.read()

# Exact NASA Aerospace Palette
new_mats = """materials = {
    'Mat_CeramicWhite': {'r': 229, 'g': 230, 'b': 226, 'edge': (175, 180, 185), 'spec': 0.65}, # #E5E6E2
    'Mat_GraphiteTrim': {'r': 37,  'g': 43,  'b': 50,  'edge': (25, 30, 36),     'spec': 0.35}, # #252B32
    'Mat_TitaniumFrame':{'r': 137, 'g': 147, 'b': 155, 'edge': (95, 102, 110),   'spec': 0.88}, # #89939B
    'Mat_MartianOrange':{'r': 217, 'g': 106, 'b': 41,  'edge': (175, 75, 25),    'spec': 0.65}, # #D96A29
    'Mat_GoldFoil':     {'r': 242, 'g': 196, 'b': 38,  'edge': (250, 220, 120), 'spec': 0.95},
    'Mat_CarbonTreads': {'r': 28,  'g': 30,  'b': 34,  'edge': (55, 60, 68),     'spec': 0.20},
    'Mat_CanopyGlass':  {'r': 45,  'g': 90,  'b': 125, 'edge': (120, 210, 245), 'spec': 0.95, 'alpha': 0.55},
    'Mat_CyanEnergy':   {'r': 0,   'g': 201, 'b': 232, 'edge': (180, 240, 255), 'spec': 1.0, 'glow': True}  # #00C9E8
}"""

text = re.sub(r"materials = \{[\s\S]*?'Mat_CyanEnergy':[\s\S]*?\}", new_mats, text)
with open('/Users/kapilash/.gemini/antigravity/scratch/3d-game-assets/render_aaa_views.py', 'w') as f:
    f.write(text)

# Update ares_vi_studio.html
with open('/Users/kapilash/.gemini/antigravity/brain/fe04a22b-3e01-409f-a35c-ed439568d853/ares_vi_studio.html', 'r') as f:
    studio_html = f.read()

new_studio_mats = """    const materials = {
      Mat_CeramicWhite: { r: 229, g: 230, b: 226, stroke: '#9aa0a6', spec: 0.65 }, // #E5E6E2
      Mat_GraphiteTrim: { r: 37, g: 43, b: 50, stroke: '#1d222a', spec: 0.35 },     // #252B32
      Mat_TitaniumFrame: { r: 137, g: 147, b: 155, stroke: '#616972', spec: 0.88 },// #89939B
      Mat_MartianOrange: { r: 217, g: 106, b: 41, stroke: '#b0521c', spec: 0.65 },  // #D96A29
      Mat_GoldFoil: { r: 242, g: 196, b: 38, stroke: '#fbe282', spec: 0.95 },
      Mat_CarbonTreads: { r: 28, g: 30, b: 34, stroke: '#4a5360', spec: 0.20 },
      Mat_CanopyGlass: { r: 45, g: 90, b: 125, stroke: '#70d6ff', spec: 0.95, alpha: 0.55 },
      Mat_CyanEnergy: { r: 0, g: 201, b: 232, stroke: '#99f0ff', glow: true, spec: 1.0 } // #00C9E8
    };"""

studio_html = re.sub(r"const materials = \{[\s\S]*?Mat_CyanEnergy:[\s\S]*?\};", new_studio_mats, studio_html)
with open('/Users/kapilash/.gemini/antigravity/brain/fe04a22b-3e01-409f-a35c-ed439568d853/ares_vi_studio.html', 'w') as f:
    f.write(studio_html)

print("Updated NASA palette in renderers and interactive studio!")
