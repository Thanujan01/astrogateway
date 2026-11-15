#!/usr/bin/env python3
"""Stage the project's own 3D art into the web app's public/models folder.

The source art lives in Git LFS at full authoring resolution: a 7 MB rigged suit and 2048x2048
PNG terrain maps. Those are correct for Unity and far too heavy to serve raw, so after copying,
each GLB is run through `@gltf-transform/cli` (dedup, weld, resample, quantize -- all lossless or
visually-safe, verified to preserve the astronaut's skin/animations and the rover's skin/23 clips)
and the suit's own oversized 2048px PNG textures are additionally resized and recompressed to
JPEG. The standalone terrain textures are downscaled to a web budget the same way as before.

Output is gitignored on purpose: it is derived, and duplicating LFS binaries into the web app
would double their weight in the repository. Re-run this after pulling new art.

    python tools/web-assets/prepare_web_assets.py          # default 1024 textures
    python tools/web-assets/prepare_web_assets.py --size 2048
"""
import argparse
import shutil
import subprocess
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
OUT = ROOT / '02-Software-Development/Web-Dashboards/astrogateway-web/public/models'
ART = ROOT / '01-Game-Development/SourceArt'
TEX = ROOT / '01-Game-Development/MissionRedHorizon/Assets/AstroGateway/MarsEnvironment/Textures'
APPROVED = ROOT / 'docs/history/codex-work-source/work/stage08d1/VisualEvaluation/Assets/ApprovedAssets'

# published name -> source. LOD1/LOD2 variants are chosen deliberately: the Originals are
# 5-19 MB scans meant for offline rendering, not a browser.
MODELS = {
    'astronaut.glb': ART / 'ARES_EVA_MK1_v3/models/ARES_EVA_MK1_v3_Rigged.glb',
    'rover.glb':     APPROVED / 'Perseverance_LOD2.glb',
    'rock_a.glb':    APPROVED / 'rock_07_LOD1.glb',
    'rock_b.glb':    APPROVED / 'rock_09_LOD1.glb',
}

# published name -> (source, grayscale?, quality)
TEXTURES = {
    'ground_diffuse.jpg': (TEX / 'sandy_gravel_Diffuse.png', False, 88),
    'ground_normal.jpg':  (TEX / 'sandy_gravel_nor_gl.png',  False, 94),
    'ground_rough.jpg':   (TEX / 'sandy_gravel_Rough.png',   True,  88),
    'ground_ao.jpg':      (TEX / 'sandy_gravel_AO.png',      True,  88),
}

parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
parser.add_argument('--size', type=int, default=1024, help='square texture size (default 1024)')
parser.add_argument('--no-optimize', action='store_true', help='skip the gltf-transform pass; copy GLBs as-is')
args = parser.parse_args()

def _npx():
    # subprocess.run() (without shell=True) needs the resolved path on Windows: 'npx' itself is
    # npx.cmd, not a directly-executable file, and WinError 2 is silently indistinguishable from
    # "npx truly isn't installed" unless resolved first.
    return shutil.which('npx') or 'npx'

GLTF_TRANSFORM = [_npx(), '--yes', '@gltf-transform/cli']

def optimize_glb(path, recompress_textures):
    """Shrink a staged GLB in place. Geometry steps are lossless or loader-transparent
    (verified: skin joints/weights and animation clip counts are unchanged after quantize).
    Leaves the plain copy alone -- and only warns -- if npx/network is unavailable."""
    if args.no_optimize:
        return
    steps = [['dedup'], ['weld'], ['resample'], ['quantize']]
    if recompress_textures:
        # Only for textures authored far larger than this scene needs (see MODELS/TEXTURES
        # above); already-small textures (rover, rocks) are left alone, not re-encoded.
        steps += [['resize', '--width', str(args.size), '--height', str(args.size)], ['jpeg', '--quality', '92']]
    current = path
    try:
        for i, step in enumerate(steps):
            nxt = path.with_suffix('.step%d.glb' % i)
            subprocess.run([*GLTF_TRANSFORM, *step, str(current), str(nxt)], check=True, capture_output=True, text=True)
            if current != path:
                current.unlink()
            current = nxt
        current.replace(path)
    except (subprocess.CalledProcessError, FileNotFoundError, OSError) as error:
        print('  (skipped gltf-transform optimization for %s: %s)' % (path.name, error), file=sys.stderr)
        if current != path and current.exists():
            current.unlink()

try:
    from PIL import Image
except ImportError:
    raise SystemExit('Pillow is required for the texture step: python -m pip install Pillow')

OUT.mkdir(parents=True, exist_ok=True)
missing = []
total = 0

for name, src in MODELS.items():
    if not src.is_file():
        missing.append(src)
        continue
    if src.stat().st_size < 1024:
        missing.append(src)  # almost certainly an unfetched LFS pointer
        continue
    shutil.copyfile(src, OUT / name)
    optimize_glb(OUT / name, recompress_textures=(name == 'astronaut.glb'))
    size = (OUT / name).stat().st_size
    total += size
    print('model    %-18s %6.2f MB  <- %s' % (name, size / 1048576, src.relative_to(ROOT)))

for name, (src, gray, quality) in TEXTURES.items():
    if not src.is_file():
        missing.append(src)
        continue
    image = Image.open(src)
    image = image.convert('L' if gray else 'RGB').resize((args.size, args.size), Image.LANCZOS)
    image.save(OUT / name, 'JPEG', quality=quality, optimize=True)
    size = (OUT / name).stat().st_size
    total += size
    print('texture  %-18s %6.2f MB  <- %s (%dpx)' % (name, size / 1048576, src.name, args.size))

print('\ntotal staged: %.2f MB in %s' % (total / 1048576, OUT.relative_to(ROOT)))

if missing:
    print('\nMissing or unfetched sources (run `git lfs pull`):', file=sys.stderr)
    for m in missing:
        print('  ' + str(m.relative_to(ROOT)), file=sys.stderr)
    raise SystemExit(1)
