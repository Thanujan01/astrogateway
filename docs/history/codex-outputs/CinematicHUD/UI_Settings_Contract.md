# Local settings and quality contract

Unique key: AstroGateway.CinematicHUD.v1.Settings. JSON contains only HUD preferences. No passwords, tokens or account data. Values are clamped on application; missing/invalid JSON falls back to defaults. Tests preserve and restore any pre-existing value of this key.

Opacity .55–.85; UI scale .85–1.15; text scale .85–1.25; joystick size .8–1.3; sensitivity .3–2; zoom 1–2; visibility Full/Compact/Minimal/Hidden. Handedness changes the joystick's reserved side, not every control. Floating changes the joystick origin within its region. Graphics Low/Medium/High/Desktop delegate to the original EnvironmentPresentation runtime pipeline clone; no original quality asset is edited. Source preset settings remain .65/.8/1/1 render scale, 512/1024/2048/4096 shadows, 25/45/65/90m shadow distance, 1/2/4/4 MSAA, mips 2/1/0/0, LOD .65/1/1.5/2, 30/30/60/60 caps.

No production/mobile FPS guarantee. Local UI preferences are not account-synchronized.
