# Known limitations

- Reference image missing; visual resemblance/pixel matching not accepted.
- Android device, APK/IL2CPP build, physical multi-touch, mobile FPS/thermal/memory remain UNTESTED.
- Private-method lifecycle delegates are version-pinned; link.xml helps retention but does not establish AOT/platform compatibility.
- Original world and primitive astronaut/rover are unchanged, not production/cinematic models.
- Water/food/radiation/temperature/wind/Sol and cloud sync are not implemented; unavailable fields are explicit.
- Scan progress is presentation-only; original scans are proximity-based simulated data, not chemistry or authoritative witness results.
- Existing saves are partial snapshots; separate HUD namespace has no automatic previous-save migration.
- HUD prefab builds its Canvas once at runtime; layout is code-anchored, not a full designer-authored nested prefab. No sprite atlas or localization/screen-reader certification.
- Only joystick side presets, not free positioning of all buttons. Floating origin stays within the reserved joystick region.
- Viewport containment is tested at four sizes, but exhaustive typography/settings/overlap combinations and hands-on ergonomics remain unverified.
- Editor global allocation counters are substantial and do not isolate runtime HUD allocations; a Player/Profiler capture is needed. No performance improvement or mobile readiness claim.
- Quick Search indexing exceptions remain in Editor startup logs. Historical failed trials and contaminated profiling evidence are retained separately; no zero-error claim for the entire log directory.
