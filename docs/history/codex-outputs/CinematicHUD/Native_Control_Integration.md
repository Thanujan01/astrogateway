# Native controls integration

Only HUDInputRouter supplies normal gameplay input in this scene. Existing movement/rover physics and the authoritative ControlModeManager consume it. Desktop WASD, Shift, RMB, E, Space and 1/2/3 weather are retained; K starts a valid-target scan presentation. Escape opens/closes the paused menu, F1 cycles HUD visibility.

HUDPointerControl implements uGUI pointer down/up/drag/end-drag. Each region tracks one pointer ID; another finger cannot steal or release it. Joystick movement is analog, clamped and dead-zoned, with recentering and configurable sensitivity/size. The optional floating setting changes the input origin inside its reserved joystick region; it is not a free-floating fullscreen joystick. A separate camera gesture region accumulates look deltas independently. Run/accelerate and brake are separate held regions. Focus loss, application pause, mode switches and disabled controls clear ownership and input.

Astronaut: move, run, contextual scan/inspect/board. Rover: steering/movement, accelerate, brake, exit, actual telemetry; scan is unavailable. Scanning: progress/cancel, canonical input lock, no new avatar. Menu: existing pause architecture sets timeScale=0, disables game ticking and rejects gameplay input. Robot/Cockpit modes remain unsupported by the existing mode manager; no fake future control system.

Interact delegates boarding/exit to NativeSliceGame.Interact (including stop/clearance checks). Equipment details and sealed airlock messages are explicit. Navigation shortcuts open actual data or select world-space waypoints. Base waypoint resolves the existing Crew Dome transform. No new habitat interior or item collection system is invented.

Automated pointer and input-state tests exercise these adapters; they do not establish real Android multi-touch behavior or hands-on PC acceptance. Physical device testing remains unverified.
