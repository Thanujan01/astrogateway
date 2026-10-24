# UI architecture
Existing HUDRoot Canvas and SafeArea, HUDTheme, HUDInputRouter/PointerControl, HUDTelemetryAdapter and AstroGatewayHUDController are reused. One ProductionPresentation component adjusts scene-local layout and adds a control-tuning modal, contextual suit status, mode label and camera buttons. Presentation state is separate from simulation.

| Responsibility | Actual implementation |
|---|---|
| Root/theme/layout | Existing runtime uGUI bootstrap; cloned theme; ProductionPresentation anchor overrides |
| Objectives, notifications, scans, minimap, local save | Existing AstroGatewayHUDController |
| Provider values | Existing HUDTelemetryAdapter / Native telemetry |
| Authoritative gameplay modes | Native manager; Production coordinator projects scan/cinematic/menu |
| Desktop / mobile adapters | Existing router and independent pointer controls |
| Camera | Existing follow camera, new final presentation rig |
| Preferences | Existing HUD preferences retained; new ProductionVisual.ControlTuning.v1 preferences |

Compact top resource strip, narrower objective panel, right map/actions and bottom contextual status expose more world. Panels remain visibly grey/translucent in captures; this is not the requested final art-direction match. Existing legacy runtime font is retained, not a newly licensed professional typeface.

Control tuning implements dead zone .05–.30, opacity .20–.60, button scale .80–1.08. Existing settings retain camera sensitivity, joystick size/side/floating origin, UI/text scale, panel opacity, visibility and four graphics presets. Responsive tests measure actual Game View framebuffer dimensions and essential panel containment. They do not certify every glyph, arbitrary DPI, real-device safe area or all possible setting combinations.

No fabricated live survival values. Full-screen tuning shields input and pauses; close restores prior pause state. Existing real menu buttons/settings/report/save/export actions are preserved. No browser authentication UI is embedded in Unity.
