# Batch 4 — controls (2026-10-20)

Asked by SSL on 2026-10-20 ("improve the game control systems"). Scene-only: new `VisualUpgradeControlAssist` on the Mars_VisualUpgrade_v1 scene; shared controllers (MarsPrototype, NativeSliceGame, HUDInputRouter), camera system, speeds and turn rates unchanged.

## What changed
- Arrow keys move/steer like W A S D.
- Keyboard walking eases in (~0.15 s) and out instead of snapping to full speed; keyboard rover steering eases in (~0.25 s) so turns are less twitchy. Top walk/run/drive speeds and turn rates are the same as before.
- Gamepad support: left stick move/steer (analog, not eased), right stick camera, RT run/accelerate (analog throttle), LT or B brake, A board/exit (same 5 m rule), X scan, Y cinematic overview, Start pause, Select HUD profile. All blocked while a menu, pause or scan blocks input, like the keyboard.
- Pause card controls reference lists arrow keys and the gamepad layout.

## Files
`Scripts/VisualUpgradeControlAssist.cs` (new), `Editor/VisualUpgradeBuilder.cs` (adds it), `Scripts/VisualUpgradeHUDStyle.cs` (controls text), `Editor/VisualUpgradePlaytest.cs` (3 new checks); scene rebuilt by the builder.

## Verification (`Docs/VisualUpgrade/Batch4/`)
Playtest **50 PASS / 0 FAIL** (the 47 baseline + up-arrow walk 2.11 m, ease-in 0.99 m/s after 0.05 s vs 2.20 m/s steady, gamepad left stick walks 4.49 m), 0 console errors; responsive HUD audit 0 issues at 3 sizes; captures in `Captures/`.
Not tested: gamepad buttons (A/X/Y/Start/Select), right stick and triggers with a real controller; feel was not judged by a person.

## Rollback
`git revert` this commit, or remove the assist line from the builder and rebuild.

## Known control issue (not changed)
In the shared code both `HUDInputRouter` and `NativeSliceGame` read the keyboard every frame (E/K/1-3 handled twice); playtests pass, but it is a candidate for a reviewed fix in the shared scripts.
