# Android touch controls

## Implementation
New Assets/AstroGateway/Stage08C/MobileControls.cs plus a minimal hook in Stage08B/MarsPrototype.cs. The existing scene file, keyboard/mouse read path, movement/collision and provisional event format are retained. Runtime-created controls require no scene rewrite or packages.

Left fixed virtual joystick supplies normalized movement; in rover mode it supplies steering/forward/reverse. RUN is held, ENTER/EXIT is one interaction per touch-begin, DRIVE holds forward throttle and BRAKE overrides acceleration. Right-side camera drag supplies pixel delta normalized by layout scale. Dedicated touch IDs own each region; a second finger cannot steal a joystick; button, camera and movement fingers coexist. Cancellation, pause and focus loss clear held inputs. Fingers crossing regions retain original ownership until released.

Uses buffered EnhancedTouch activeTouches from installed InputSystem1.20, with balanced enable/disable. It does not poll raw Touchscreen touches as a legacy Input substitute. Joystick has12% deadzone and unit-circle normalization. Desktop keyboard and touch vectors combine then clamp; keyboard/RMB controls remain available. testControl explicitly bypasses touch for isolated legacy regressions.

Touch controls are visible on Android or explicit preview mode. Layout derives from Screen.safeArea, converts bottom-origin screen coordinates to top-origin drawing coordinates, scales relative to1280×720, and keeps controls inside insets. Existing Canvas HUD gets a runtime safe-area parent. Android requests LandscapeLeft and restores the previous orientation when disabled. Mobile HUD provides touch instructions. LandscapeRight auto-rotation is not enabled; portrait UI is not the intended mode. System gestures, unusual foldables and physical cutouts require device testing.

This uses lightweight immediate-mode Unity GUI prototype controls, not a final accessible native/mobile UI. IMGUI text/control sizing is proportional; physical touch-target sizes and full accessibility have not been certified.

## Tests
12/12 local checks passed: safe-area conversion, inset bounds, normalized diagonal joystick, ownership, simultaneous run/move, release handling, camera finger, throttle/brake independence, cancellation, small landscape layout, virtual EnhancedTouch delivery and cancellation. Evidence/Touch_Unit_Checks.txt.

These are synthetic component/virtual-device checks in isolated Editor Play Mode, not human finger testing. Android hardware touch usability remains unverified. The separate profiling APK is a local standalone test player; it does not replace or modify the Android Unity-as-a-Library host or its authentication/bridge.

## References
[Unity Input System touch support](https://docs.unity.cn/Packages/com.unity.inputsystem@1.0/manual/Touch.html) explains EnhancedTouch and touch lifecycle. Installed1.20 package documentation/source was used for actual APIs. [Unity safe-area API](https://docs.unity.com/en-us/engine/7000.0/script-reference/unityengine/screen/safearea) documents bottom-left coordinates and Android window/cutout behavior; verify exact6000.6 behavior on target devices rather than assuming newer platform behavior.
