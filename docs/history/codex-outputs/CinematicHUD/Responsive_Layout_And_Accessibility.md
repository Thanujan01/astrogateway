# Responsive layout and accessibility

Canvas Scaler uses a 1920×1080 reference with balanced width/height scaling. Safe Area uses normalized screen anchors and is recomputed on viewport/safe-area changes. Major panels use normalized anchors; no single fixed screenshot is stretched over the game.

Verified runtime viewport sizes: 1920×1080 desktop, 2340×1080 (19.5:9), 2400×1080 (20:9, with explicitly simulated notch margins), 1600×1200 tablet (4:3). Responsive_Layout_Checks.txt contains actual runtime dimensions and ten essential bounds checks per viewport. PNG headers independently record capture sizes. These are Unity Game View tests, not Android OS/device tests.

Rounded translucent panels use the requested slate/orange/blue palette, thin borders, readable text and persistent state widgets. Opacity is configurable .55–.85; a bright-light heuristic raises it to at least .8 for high-intensity lighting. This is not a per-pixel luminance contrast analyzer. Text has a runtime minimum-size guard, configurable scale and explicit warning words as well as colors. There is no screen-reader certification or formal WCAG contrast measurement.

Settings: Full/Compact/Minimal/Hidden, UI density/scale, text scale, opacity, joystick size, sensitivity, map zoom, joystick side and fixed/floating origin, four graphics settings and weather preview. Hidden HUD retains a restore control; F1 also restores by cycling. UI scaling adjusts the Canvas content density while normalized panel coverage remains anchored. Joystick side has two presets; arbitrary drag-and-drop button repositioning is not implemented.

The layout test establishes containment, not exhaustive text localization, every font-scale combination, physical tap ergonomics or every panel-overlap case. Tablet visual QA found a long shortcut label wrapping; final button labels use fitting to keep them within their controls. Settings/report panels are scrollable where appropriate. Further 720p/low-DPI and accessibility-user testing is needed.
