using UnityEngine;
using UnityEngine.InputSystem;
using AstroGateway.CinematicHUD;
using AstroGateway.Stage08B;

// Control layer for Mars_FirstMission_v1. Same pattern as VisualUpgradeControlAssist: it runs after the input
// routers have written the frame's input into MarsPrototype.test* (HUDInputRouter -200, NativeSliceGame -100,
// ControlAssist -50) and before MarsPrototype (0) consumes it. It only reshapes input; speeds, turn rates,
// gravity, collisions, boundaries, oxygen and the boarding/exit rules stay in the shared controllers.
//  - explicit control state: ASTRONAUT / ROVER_DRIVE / ROVER_SCIENCE / DATA_REVIEW / PAUSE_MENU
//  - only the state's inputs reach the game (no driving while a science/data panel is open)
//  - look sensitivity, camera inversion and reduced camera motion (all look sources: touch, mouse, gamepad)
//  - astronaut auto-sprint (optional) after the stick is held at full deflection
//  - rover speed-sensitive steering (less stick authority at speed; the turn-rate formula itself is unchanged)
namespace AstroGateway.FirstMission
{
    public enum ControlState { Astronaut, RoverDrive, RoverScience, DataReview, PauseMenu }

    [DefaultExecutionOrder(-40)]
    public class FirstMissionControlLayer : MonoBehaviour
    {
        public FirstMissionController mission;
        public FirstMissionHUD ui;
        public float autoSprintDelay = .45f, autoSprintThreshold = .95f, highSpeedSteerScale = .6f, topSpeed = 3.8f;
        public ControlState State { get; private set; }
        public bool AutoSprinting { get; private set; }
        public float LastSteerScale { get; private set; } = 1;
        float fullStickTime;
        AstroGatewayHUDController hud;
        MarsPrototype p;

        void Start() { hud = mission.hud; p = hud.game.prototype; }

        public static ControlState Resolve(bool paused, bool driving, FirstMissionHUD.Tab tab) =>
            paused ? ControlState.PauseMenu : !driving ? ControlState.Astronaut :
            tab == FirstMissionHUD.Tab.Drive ? ControlState.RoverDrive : tab == FirstMissionHUD.Tab.Science ? ControlState.RoverScience : ControlState.DataReview;

        public double LastCostMs { get; private set; }
        readonly System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        void Update() { watch.Restart(); Step(); LastCostMs = watch.Elapsed.TotalMilliseconds; }

        void Step()
        {
            if (!mission || !mission.Ready || !ui || !hud.game.initialized) return;
            var s = mission.Settings;
            State = Resolve(hud.environment.paused || hud.modalOpen, p.driving, ui.Current);

            // Keyboard: Tab returns to DRIVE from any rover panel (desktop players are not stuck in a panel).
            var k = Keyboard.current;
            if (k != null && k.tabKey.wasPressedThisFrame && p.driving && ui.Current != FirstMissionHUD.Tab.Drive) ui.Select(FirstMissionHUD.Tab.Drive);

            // Look: sensitivity / inversion / reduced motion for every source that reached testLook.
            Vector2 look = p.testLook;
            float sens = s.lookSensitivity * (s.reducedMotion ? .6f : 1f);
            look *= sens;
            if (s.invertY) look.y = -look.y;
            p.testLook = State == ControlState.PauseMenu || State == ControlState.DataReview ? Vector2.zero : look;

            switch (State)
            {
                case ControlState.PauseMenu:
                case ControlState.DataReview:
                case ControlState.RoverScience:
                    // UI states: no accidental driving or walking. The brake stays available in science mode.
                    p.testMove = Vector2.zero; p.testRun = false;
                    if (State != ControlState.RoverScience) p.testBrake = false;
                    AutoSprinting = false; fullStickTime = 0;
                    break;
                case ControlState.Astronaut:
                    if (s.autoSprint && p.testMove.magnitude >= autoSprintThreshold) fullStickTime += Time.deltaTime; else fullStickTime = 0;
                    AutoSprinting = s.autoSprint && fullStickTime >= autoSprintDelay;
                    if (AutoSprinting) p.testRun = true;
                    break;
                case ControlState.RoverDrive:
                    float speed01 = Mathf.Clamp01(Mathf.Abs(p.speed) / topSpeed);
                    LastSteerScale = Mathf.Lerp(1f, highSpeedSteerScale, speed01 * speed01);
                    p.testMove = new Vector2(p.testMove.x * LastSteerScale, p.testMove.y);
                    break;
            }
        }
    }
}
