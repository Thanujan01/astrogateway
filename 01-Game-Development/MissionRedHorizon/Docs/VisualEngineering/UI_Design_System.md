# AstroGateway UI design system (v0.1, documents what the HUD uses today)

Implemented in uGUI (the project's existing UI framework). No second framework is introduced. Tokens currently live as fields in `VisualUpgradeHUDStyle.cs`; moving them into the existing `HUDTheme` ScriptableObject (`VisualUpgradeHUDTheme.asset`) is planned (batch 2) so every panel reads one source.

## Colour tokens
| Token | RGBA (0–1) | Use |
|---|---|---|
| surface.panel | .015 .035 .055 .64 | HUD panels (opacity follows the player's Panel opacity setting) |
| surface.modal | .015 .035 .055 .94 | Modal cards |
| surface.scrim | 0 .01 .02 .55 | Behind modals |
| surface.button | .03 .07 .10 .80 | Buttons at rest |
| accent.primary (cyan) | .30 .86 .96 1 | Live data, own-system status, focus, titles |
| accent.secondary (Mars orange) | 1 .56 .16 1 | Prompts, scan activity, mission accents — sparingly |
| text.primary | .90 .94 .96 1 | Values and body |
| text.secondary | .58 .68 .74 1 | Labels, units, provenance ("local sim") |
| status.nominal | theme.oxygen | Nominal readings |
| status.critical | theme.critical | Critical/warnings only |
| track | .25 .32 .36 .55 | Bar tracks; .18 alpha when the source is NOT CONNECTED |

Rules: cyan = data, orange = action/attention, red only for real critical states. No neon glow; outlines disabled (they tint translucent panels).

## Typography
Avenir Next (macOS system font; fallback Avenir, Helvetica Neue, Arial). Not embedded — Windows/Android builds fall back to the theme font.
| Role | Size (px at 1512×982 reference) | Min with best-fit |
|---|---|---|
| Modal title | 27 | — |
| Value / primary readout | 20–22 | 16 |
| Body | 16–19 | 13 |
| Label / caption | 13–14 | 11 |
Minimum rendered size 11 px; captions in upper case with " • " separators.

## Spacing and shape
- Panels anchored in safe-area fractions; outer margin 1.2% width; gap between stacked panels ≥ .5% height.
- Rounded sliced sprite `RoundedPanel.png` for every panel/button; 2 px accent bar top-left (cyan for systems, orange for mission).
- Bottom centre column .36–.64 width: prompt pill → speed gauge → telemetry.

## Components
Panel (accent + translucent surface) · Label (best-fit, wrap, truncate) · Bar (track + fill, anchors drive fill) · Prompt pill (key in brackets: "[E] BOARD", "[K] SCAN"; hidden when nothing actionable) · Button (rest .80 surface, highlight cyan-white, pressed orange) · Modal card (scrim + card + orange accent + cyan title).

## States
Button: rest / highlighted / pressed / selected / disabled (Unity ColorBlock; disabled keeps default dimming). Unavailable data: text "NOT CONNECTED" or "not implemented", dimmed track, never a fake value.

## Motion
Panels appear instantly; camera transitions ease (existing). Planned: 150 ms fade for modals and pill.

## Content rules
Every number states its provenance ("local sim", "simulated"). Key hints must match Input System bindings: W/A/S/D, Shift, Space, E, K, C, F1, 1/2/3, Esc, right mouse.
