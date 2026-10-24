# Android manual test plan — not executed

Prerequisite: an approved physical Android handset with Unity Android build support and an authorized ADB connection. Build a standalone isolated development player with this slice; do not replace the authenticated Android host or its existing Unity export without review. Record device model/OS/GPU/resolution/build hash and active quality profile.

1. Landscape and cutout safe area; joystick/buttons remain visible. Test left-handed inspector configuration, size 0.7–1.5, opacity 0.3–1 and sensitivity 0.1–3.
2. Begin joystick at different points (floating origin), release and cancel, drag beyond radius and confirm clamping/dead zone.
3. Hold movement with one finger, orbit with a second, run with a third. Verify ownership, no cross-stealing and no camera response over data-panel buttons.
4. Background/resume and mode change while fingers remain held; require release/repress without stuck movement.
5. Approach rover, board, inspect astronaut remains seated/visible, drive and brake, reject moving exit, stop and exit near terrain/walls; verify grounded/camera behavior.
6. Read battery/speed/health updates; record a scan, inspect simulated label/location/tick, save, restart, load walking and seated states.
7. Export deliberately; inspect files exclude credentials/identity. Do not upload during test.
8. Check Console/logcat crashes/ANRs, touch latency and memory. Measure FPS/frame-time only with defined hardware and workload; do not reuse software-emulator results as physical readiness.

Camera clipping, suit/seat overlap, fast mode-switching, UI scaling/labels and load failure recovery require human observations. Robot/cockpit/30-Sol scenarios should show unavailable/deferred, not a PASS.
