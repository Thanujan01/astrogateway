# Performance evidence

Raw completed sample:

```text
Actual Unity Editor batch Play Mode. Not physical Android, not UI-only GPU timing, not an improvement claim.
Unity=6000.6.4f1 GPU=Apple M4 Pro API=Metal framebuffer=1920x1080
Medium graphics; fixed astronaut spawn; clear weather. 2s warmup + >=4s per profile; no screenshots or deliberate sleeps during samples.
Full: frames=117 max_ms=173.26 sample_valid=True mean_ms=34.46 effective_fps=29.02 p95_ms=35.18 MainThread_recorder_ms=34.456 GC_recorder_bytes_per_frame=87446.9 active_graphics=83 actual_profile_index=0 active_cards=Independent camera gesture zone,Mission branding,Survival resources,Weather and clock,Mission objectives,Circular minimap panel,Action 0,Action 1,Action 2,Action 3,Analog movement joystick,Shortcut 0,Shortcut 1,Shortcut 2,Shortcut 3,Rover telemetry,Recent scientific scan,Scan,Context interact,Interaction target,Run or accelerate
Compact: frames=121 max_ms=35.71 sample_valid=True mean_ms=33.32 effective_fps=30.01 p95_ms=33.93 MainThread_recorder_ms=33.321 GC_recorder_bytes_per_frame=343472.8 active_graphics=47 actual_profile_index=1 active_cards=Independent camera gesture zone,Survival resources,Action 0,Action 1,Action 2,Action 3,Analog movement joystick,Rover telemetry,Scan,Context interact,Interaction target,Run or accelerate
Hidden: frames=120 max_ms=35.48 sample_valid=True mean_ms=33.33 effective_fps=30.00 p95_ms=34.03 MainThread_recorder_ms=33.331 GC_recorder_bytes_per_frame=343748.8 active_graphics=3 actual_profile_index=3 active_cards=Independent camera gesture zone,Restore HUD
Unity reported total allocated bytes=304573808 (Editor process scope; not Android RSS or UI-only memory)
GPU timing and physical-device thermal behavior not measured. Short capped Editor samples are not proof of Android readiness.
```

This is the Apple M4 Pro Editor batch Game View at Medium quality and a 30 FPS cap, not an Android/player benchmark or UI-only cost. The Main Thread recorder includes the whole Editor frame and cap/wait overhead. GC counters and total allocated bytes are process-wide, not attributed to HUD code. High counters require a standalone Development Player/Profiler investigation before performance acceptance. Short capped samples cannot establish scalability or an FPS improvement.

A prior Full sample included a roughly 15-minute wall-clock stall and was invalid for performance interpretation. Performance_Trial_With_Long_Stall.txt preserves it; it was not silently treated as valid. The final probe records max interval and sample-valid flag, actual visibility index and active cards. Full/Compact/Hidden have 83/47/3 active graphics in the final idle sample; Runtime_UI_Inventory.txt was captured before pending test dialog objects were destroyed and is not the steady-state count.

The minimap has no extra rendering camera; four tiny generated UI sprites, cached transforms and 5Hz updates are used. Bars animate without rebuilding the hierarchy; resources and map use subcanvases. This describes implementation choices, not a demonstrated mobile performance gain.
