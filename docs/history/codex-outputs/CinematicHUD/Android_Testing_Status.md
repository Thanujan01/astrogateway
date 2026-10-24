# Android testing status

ADB check completed successfully; zero devices/emulators listed. No physical-device input, FPS, thermal, memory or Android OS safe-area acceptance is claimed. No APK/IL2CPP build was performed for this milestone. Android landscape orientation is requested by the runtime HUD, but the actual activity/orientation lifecycle is untested on device.

19.5:9, 20:9 and notch-margin captures are Unity Game View simulations. The public pointer handlers were tested with synthetic EventSystem PointerEventData, including independent fingers and ownership rejection, not injected physical touches.

Next: build the new scene into an Android development player, verify delegate/linker compatibility, test real simultaneous joystick/camera/run/scan/rover gestures, system interruptions and screen cutouts, then collect on-device Profiler traces. Preserve the existing Android host/Unity bridge; it was not modified or rebuilt here.
