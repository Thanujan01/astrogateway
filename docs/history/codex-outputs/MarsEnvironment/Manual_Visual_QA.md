# Manual visual QA and limitations

Completed visual review: actual overview, habitat-ground and rover camera renders; actual scripted Play Mode HUD capture. Found and corrected: edge-on solar orientation, excessive seam geometry, ground-contact offsets and small-window HUD overlap. Dense seam pattern/aliasing and simple silhouettes remain.

Still requires operator testing:
- [ ] Supply primary reference image for matching; current image was absent.
- [ ] Walk every zone/route, slopes, crater rim and ±119m boundaries; check no trapping.
- [ ] Drive between habitat/solar/science zones, brake, exit on uneven terrain and verify same astronaut.
- [ ] Orbit camera against conservative dome/airlock colliders; inspect clipping and camera transition stability.
- [ ] Observe six wheel contact/rotation, sensor clearance and seated body overlap; suspension links are static, not dynamics.
- [ ] Inspect terrain tiling, rock normals/LOD transitions, floating objects and distant massif repetition from more viewpoints.
- [ ] Inspect lighting/shadow bias/exposure at all presets and all weather states. Dusty lighting screenshot is not proof of gameplay storm VFX.
- [ ] Check typography/touch targets over bright terrain, safe areas and landscape/left-handed layouts on hardware.
- [ ] Run physical multitouch, pause/resume/backgrounding, scan/save/load/export and inspect logcat/ANRs.
- [ ] Measure sustained per-quality performance on defined hardware.

Known visual limits: triangulated hemispheres are geodesic-inspired, not an engineered geodesic habitat; primitive airlocks, tanks, crates, marker stations, character and rover remain. Observation panels are opaque; interiors unavailable. Headlamp/sensor geometry is cosmetic. No full astronaut rig upgrade, boarding animation, active suspension, scientifically sourced terrain elevation, physical dust/solar model or advanced survival.

New shader/scene controls passed automated checks but Unity QuickSearch indexing errors remain in validation logs. Original source remains unchanged. User reference fidelity, photorealism, cinematic quality and Android readiness are not signed off.
