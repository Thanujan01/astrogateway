using UnityEngine;
using UnityEngine.InputSystem;
using AstroGateway.CinematicHUD;
using AstroGateway.ProductionVisual;

namespace AstroGateway.VisualUpgrade
{
    // Control feel for Mars_VisualUpgrade_v1 only (shared controllers untouched). Runs after NativeSliceGame (-100)
    // has written the frame's input into MarsPrototype.test* and before MarsPrototype (0) consumes it:
    //  - arrow keys work like W A S D;
    //  - keyboard movement eases in/out (about 0.15 s) instead of snapping to full speed, and rover steering eases
    //    in (about 0.25 s) so keyboard turns are less twitchy; top speeds and turn rates are unchanged;
    //  - gamepad: left stick move/steer, right stick camera, RT run/accelerate, LT or B brake, A board/exit,
    //    X scan, Y cinematic overview, Start pause, Select HUD profile.
    // Same rules as the keyboard: nothing happens while a menu, pause or scan blocks input (hud.InputAllowed).
    [DefaultExecutionOrder(-50)]
    public class VisualUpgradeControlAssist : MonoBehaviour
    {
        public AstroGatewayHUDController hud;
        public ProductionModeCoordinator modes;
        public float walkEaseIn = 7f, walkEaseOut = 9f, steerEase = 4.5f, stickLookSpeed = 650f;
        Vector2 smoothMove;
        float smoothSteer;

        void Update()
        {
            if (!hud || !hud.game || !hud.game.initialized) return;
            var p = hud.game.prototype;
            var pad = Gamepad.current;
            var k = Keyboard.current;
            bool allowed = hud.InputAllowed;
            float dt = Time.unscaledDeltaTime;

            // Gamepad buttons mirror the keyboard shortcuts (ignored while the harness drives the game).
            if (pad != null && !hud.game.testing)
            {
                if (pad.startButton.wasPressedThisFrame) hud.TogglePause();
                if (pad.selectButton.wasPressedThisFrame) hud.CycleVisibility();
                if (allowed)
                {
                    if (pad.buttonSouth.wasPressedThisFrame && Vector3.Distance(p.ActivePosition(), p.rover.transform.position) <= 5) hud.Interact();
                    if (pad.buttonWest.wasPressedThisFrame) hud.BeginScan();
                    if (pad.buttonNorth.wasPressedThisFrame && modes) modes.ToggleCinematic();
                }
            }

            if (!allowed) { smoothMove = Vector2.zero; smoothSteer = 0; return; }

            Vector2 move = p.testMove;
            bool digital = true;
            if (k != null && !hud.game.testing && !k.leftMetaKey.isPressed && !k.leftCtrlKey.isPressed)
                move += new Vector2((k.rightArrowKey.isPressed ? 1 : 0) - (k.leftArrowKey.isPressed ? 1 : 0), (k.upArrowKey.isPressed ? 1 : 0) - (k.downArrowKey.isPressed ? 1 : 0));
            if (pad != null && !hud.game.testing)
            {
                var stick = pad.leftStick.ReadValue();
                if (stick.sqrMagnitude > .02f) { move += stick; digital = false; }
                p.testLook += pad.rightStick.ReadValue() * stickLookSpeed * dt;
                if (pad.rightTrigger.ReadValue() > .3f) { p.testRun = true; if (p.driving) move.y = Mathf.Max(move.y, pad.rightTrigger.ReadValue()); }
                if (pad.leftTrigger.ReadValue() > .3f || pad.buttonEast.isPressed) p.testBrake = true;
            }
            move = Vector2.ClampMagnitude(move, 1);

            if (p.driving)
            {
                // Throttle stays direct (the rover already ramps speed); steering eases in.
                float rate = Mathf.Abs(move.x) > Mathf.Abs(smoothSteer) ? steerEase : steerEase * 2;
                smoothSteer = digital ? Mathf.MoveTowards(smoothSteer, move.x, rate * dt) : move.x;
                p.testMove = new Vector2(smoothSteer, move.y);
                smoothMove = Vector2.zero;
            }
            else
            {
                float rate = move.sqrMagnitude > smoothMove.sqrMagnitude ? walkEaseIn : walkEaseOut;
                smoothMove = digital ? Vector2.MoveTowards(smoothMove, move, rate * dt) : move;
                p.testMove = smoothMove;
                smoothSteer = 0;
            }
        }
    }
}
