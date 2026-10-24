using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using AstroGateway.CinematicHUD;
using AstroGateway.MarsEnvironment;
using AstroGateway.NativeSlice;
using AstroGateway.ProductionVisual;
using AstroGateway.Stage08B;
using AstroGateway.Integration;

// Step 1 gameplay verification for Mars_ProductionVisual_v1.
// Batch-mode only. Drives the existing gameplay through its public API and a virtual keyboard,
// and never saves the scene. Results go to the folder in the STEP1_OUT environment variable.
namespace AstroGateway.Verification
{
    [InitializeOnLoad]
    public static class Step1GameplayVerification
    {
        const string Scene = "Assets/AstroGateway/ProductionVisual/Scenes/Mars_ProductionVisual_v1.unity";
        const string Flag = "Step1.Gameplay.Run";
        static string Out => Environment.GetEnvironmentVariable("STEP1_OUT") ?? Path.Combine(Path.GetTempPath(), "Step1Gameplay");

        static readonly List<string> lines = new List<string>();
        static int pass, fail;
        static IEnumerator routine;
        static readonly Stack<IEnumerator> stack = new Stack<IEnumerator>();
        static object waiting;
        static int lastFrame = -1;
        static double startedAt;

        static AstroGatewayHUDController hud;
        static ProductionModeCoordinator modes;
        static ProductionCameraRig rig;
        static ProductionPresentation art;
        static MarsPrototype p;
        static Keyboard keyboard;
        static InputSettings.BackgroundBehavior priorBackground;
        static InputSettings.EditorInputBehaviorInPlayMode priorEditorInput;

        // Per-frame monitors.
        static bool monitoring;
        static int cameraInside, astronautPenetration, roverPenetration, belowGround, frames;
        static readonly List<string> monitorSamples = new List<string>();
        static readonly List<string> logErrors = new List<string>();

        static Step1GameplayVerification()
        {
            EditorApplication.update += Tick;
        }

        public static void Run()
        {
            EditorSceneManager.OpenScene(Scene);
            SessionState.SetBool(Flag, true);
            Application.runInBackground = true;
            EditorApplication.isPaused = false;
            EditorApplication.isPlaying = true;
        }

        static void Tick()
        {
            if (!Application.isBatchMode || !EditorApplication.isPlaying) return;
            try
            {
                if (routine == null)
                {
                    if (!SessionState.GetBool(Flag, false)) return;
                    hud = UnityEngine.Object.FindAnyObjectByType<AstroGatewayHUDController>();
                    if (!hud || !hud.safe || !hud.game || !hud.game.initialized) return;
                    SessionState.SetBool(Flag, false);
                    modes = UnityEngine.Object.FindAnyObjectByType<ProductionModeCoordinator>();
                    rig = UnityEngine.Object.FindAnyObjectByType<ProductionCameraRig>();
                    art = UnityEngine.Object.FindAnyObjectByType<ProductionPresentation>();
                    p = hud.game.prototype;
                    Application.logMessageReceived += OnLog;
                    startedAt = EditorApplication.timeSinceStartup;
                    routine = Tests();
                    stack.Push(routine);
                }
                if (EditorApplication.timeSinceStartup - startedAt > 1800) throw new Exception("Verification timeout");
                if (Time.frameCount == lastFrame) return;
                lastFrame = Time.frameCount;
                if (monitoring) Monitor();
                if (!Ready()) return;
                waiting = null;
                while (stack.Count > 0)
                {
                    var top = stack.Peek();
                    if (!top.MoveNext()) { stack.Pop(); continue; }
                    if (top.Current is IEnumerator nested) { stack.Push(nested); continue; }
                    waiting = top.Current;
                    break;
                }
                if (stack.Count == 0) Finish();
            }
            catch (Exception e)
            {
                Line("EXCEPTION: " + e);
                fail++;
                Finish();
            }
        }

        // Wait objects: float = simulation seconds (prototype.elapsed), Realtime = unscaled seconds, Func<bool> = condition.
        class Realtime { public double until; }
        class SimWait { public float until; public double deadline; }
        static object Sim(float s) => new SimWait { until = p.elapsed + s, deadline = EditorApplication.timeSinceStartup + s * 6 + 10 };
        static object Real(float s) => new Realtime { until = EditorApplication.timeSinceStartup + s };
        static bool Ready()
        {
            if (waiting == null) return true;
            if (waiting is SimWait w)
            {
                if (p.elapsed >= w.until) return true;
                if (EditorApplication.timeSinceStartup < w.deadline) return false;
                Line("WARN  simulation clock stalled during a wait: " + State());
                return true;
            }
            if (waiting is Realtime r) return EditorApplication.timeSinceStartup >= r.until;
            if (waiting is Func<bool> f) return f();
            return true;
        }

        static void OnLog(string message, string stack, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
                logErrors.Add(type + ": " + message.Split('\n')[0]);
        }

        static void Check(string name, bool ok, string detail = "")
        {
            if (ok) pass++; else fail++;
            Line((ok ? "PASS  " : "FAIL  ") + name + (string.IsNullOrEmpty(detail) ? "" : "  [" + detail + "]"));
        }

        static void Line(string s) { lines.Add(s); Debug.Log("[Step1] " + s); }
        static string V(Vector3 v) => $"({v.x:F2}, {v.y:F2}, {v.z:F2})";

        // ---------- helpers ----------
        static void Input(Vector2 move, bool run = false, bool brake = false, bool accelerate = false)
        {
            hud.game.modes.Submit(new NativeInput { movement = move, run = run, brake = brake, accelerate = accelerate });
        }

        static bool OwnCollider(Collider c) => c == p.astronaut || c == p.rover || c.transform.IsChildOf(p.rover.transform) || c.transform.IsChildOf(p.astronaut.transform);

        static float GroundY(Vector3 pos)
        {
            var hits = Physics.RaycastAll(pos + Vector3.up * 30, Vector3.down, 80, 1 << 0, QueryTriggerInteraction.Ignore);
            var valid = hits.Where(h => !OwnCollider(h.collider)).OrderBy(h => h.distance).ToArray();
            return valid.Length > 0 ? valid[0].point.y : float.NaN;
        }

        static void Teleport(CharacterController c, Vector3 pos, float yawDeg)
        {
            float g = GroundY(pos);
            if (!float.IsNaN(g)) pos.y = g + 0.05f;
            c.enabled = false;
            c.transform.SetPositionAndRotation(pos, Quaternion.Euler(0, yawDeg, 0));
            c.enabled = true;
            Physics.SyncTransforms();
        }

        static Collider[] Overlaps(CharacterController c)
        {
            if (!c.enabled || !c.gameObject.activeInHierarchy) return new Collider[0];
            var t = c.transform;
            float scale = Mathf.Max(t.lossyScale.x, t.lossyScale.z);
            float r = c.radius * scale * 0.8f;
            float half = Mathf.Max(0, c.height * t.lossyScale.y / 2 - c.radius * scale);
            Vector3 centre = t.TransformPoint(c.center);
            // Lift the probe slightly so resting on sloped ground is not reported as penetration.
            centre += Vector3.up * 0.15f;
            return Physics.OverlapCapsule(centre + Vector3.up * half, centre - Vector3.up * half, r, 1 << 0, QueryTriggerInteraction.Ignore)
                .Where(x => !OwnCollider(x) && !(x is TerrainCollider)).ToArray();
        }

        static void Monitor()
        {
            frames++;
            var cam = p.follow;
            bool cinematic = modes.Current == ProductionMode.Cinematic;
            if (cam && !cinematic && Physics.CheckSphere(cam.transform.position, 0.1f, 1 << 0, QueryTriggerInteraction.Ignore))
            {
                var hits = Physics.OverlapSphere(cam.transform.position, 0.1f, 1 << 0, QueryTriggerInteraction.Ignore).Where(x => !OwnCollider(x)).ToArray();
                if (hits.Length > 0) { cameraInside++; Sample("camera inside " + string.Join(",", hits.Select(h => h.name)) + " at " + V(cam.transform.position) + " mode " + modes.Current); }
            }
            var a = Overlaps(p.astronaut);
            if (!p.driving && a.Length > 0) { astronautPenetration++; Sample("astronaut overlaps " + string.Join(",", a.Select(h => h.name)) + " at " + V(p.astronaut.transform.position)); }
            var r = Overlaps(p.rover);
            if (r.Length > 0) { roverPenetration++; Sample("rover overlaps " + string.Join(",", r.Select(h => h.name)) + " at " + V(p.rover.transform.position)); }
            var active = p.driving ? p.rover.transform.position : p.astronaut.transform.position;
            float g = GroundY(active);
            if (!float.IsNaN(g) && active.y < g - 0.5f) { belowGround++; Sample("below ground by " + (g - active.y).ToString("F2") + " at " + V(active)); }
        }

        static void Sample(string s) { if (monitorSamples.Count < 40) monitorSamples.Add("frame " + Time.frameCount + ": " + s); }

        static Collider LargestStructureNear(Vector3 from)
        {
            return UnityEngine.Object.FindObjectsByType<Collider>(FindObjectsSortMode.None)
                .Where(c => c.enabled && !c.isTrigger && !(c is TerrainCollider) && !OwnCollider(c) && c.gameObject.layer == 0)
                .Where(c => c.bounds.size.x > 6 && c.bounds.size.z > 6 && c.bounds.size.y > 2 && c.bounds.size.x < 60 && c.bounds.size.z < 60)
                .OrderBy(c => Vector3.Distance(from, c.bounds.center)).FirstOrDefault();
        }

        static Vector3 OpenSpot(Vector3 hint, float clear = 7f)
        {
            for (int ring = 0; ring < 12; ring++)
                for (int i = 0; i < 8; i++)
                {
                    float ang = i * 45 * Mathf.Deg2Rad;
                    Vector3 c = hint + new Vector3(Mathf.Cos(ang), 0, Mathf.Sin(ang)) * ring * 6;
                    if (Mathf.Abs(c.x) > 105 || Mathf.Abs(c.z) > 105) continue;
                    float g = GroundY(c); if (float.IsNaN(g)) continue; c.y = g;
                    if (Physics.OverlapSphere(c + Vector3.up * 1.2f, clear, 1 << 0, QueryTriggerInteraction.Ignore).All(x => OwnCollider(x) || x is TerrainCollider || x.bounds.size.y < 0.3f || x.bounds.size.x > 200)) return c;
                }
            return hint;
        }

        static void PressKey(Key key)
        {
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(key));
        }

        static void ReleaseKeys()
        {
            InputSystem.QueueStateEvent(keyboard, new KeyboardState());
        }

        static string State() => $"mode {modes.Current}/{hud.game.modes.Mode}, driving {p.driving}, paused {hud.environment.paused}, modal {hud.modalOpen}, scanning {hud.scanning}, cinematic {modes.cinematic}, parent {(p.astronaut.transform.parent ? p.astronaut.transform.parent.name : "none")}, result '{hud.lastResult}'";

        // Bring the game back to plain astronaut control so one failed section cannot cascade into the next.
        static IEnumerator Normalize()
        {
            Input(Vector2.zero, brake: true);
            if (hud.scanning) hud.CancelScan();
            if (art.tuningWindow) art.CloseTuning();
            if (hud.modalOpen) hud.CloseModal();
            if (hud.environment.paused) hud.environment.SetPaused(false);
            if (modes.cinematic) modes.LeaveCinematic();
            if (p.driving)
            {
                p.speed = 0;
                yield return Sim(0.3f);
                if (!hud.game.Interact())
                {
                    Teleport(p.rover, rSpot, 0);
                    yield return Sim(0.3f);
                    hud.game.Interact();
                }
            }
            Input(Vector2.zero);
            yield return Sim(0.2f);
            if (p.driving || modes.Current != ProductionMode.Astronaut) Line("WARN  could not normalize: " + State());
        }

        // ---------- the test sequence ----------
        static Vector3 rSpot, wSpot;
        static IEnumerator Tests()
        {
            hud.game.testing = true;
            hud.settings = new HUDSettings();
            hud.ApplySettings();
            var identity = p.astronaut.gameObject;
            var cc = p.astronaut;
            Line("Scene: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().path);
            Line("Astronaut start " + V(cc.transform.position) + " radius " + cc.radius + " height " + cc.height + " scale " + V(cc.transform.lossyScale));
            Line("Rover start " + V(p.rover.transform.position) + " radius " + p.rover.radius + " height " + p.rover.height + " scale " + V(p.rover.transform.lossyScale));
            foreach (var t in hud.environment.targets) Line("Research target " + (t ? t.displayName + " " + V(t.transform.position) : "MISSING"));
            Line("Airlocks " + hud.environment.closedAirlocks.Length);
            rSpot = OpenSpot(new Vector3(-30, 0, -10), 9f);
            wSpot = OpenSpot(new Vector3(-20, 0, -40), 7f);
            Line("Open test spots: rover " + V(rSpot) + ", walk " + V(wSpot));
            Check("Scene starts in astronaut mode", modes.Current == ProductionMode.Astronaut, modes.Current.ToString());
            Check("No research target missing", hud.environment.targets.All(t => t));
            IntegrationServices.ResetToOffline();
            var bridge = new GameObject("Step1 integration bridge").AddComponent<GameplayEventBridge>();
            bridge.game = hud.game;
            yield return Sim(1f);
            Check("Integration bridge starts a local (non-authoritative) session", IntegrationServices.Sessions.Session.status == "active" && !IntegrationServices.Sessions.Session.authoritative && !IntegrationServices.Identity.Current.authenticated);
            monitoring = true;

            // 1. Astronaut movement --------------------------------------------------------------
            Teleport(cc, new Vector3(-10, 0, -20), 0);
            if (Overlaps(cc).Length > 0) Teleport(cc, new Vector3(-20, 0, -30), 0);
            yield return Sim(0.5f);
            p.yaw = 0;
            Vector3 a0 = cc.transform.position; float e0 = p.elapsed;
            Input(new Vector2(0, 1));
            yield return Sim(2f);
            Vector3 a1 = cc.transform.position; float walk = Vector3.ProjectOnPlane(a1 - a0, Vector3.up).magnitude / (p.elapsed - e0);
            Check("Walk forward moves astronaut", walk > 1.5f, $"{walk:F2} m/s, expected ~2.2");
            Check("Walk follows camera heading (yaw 0 → +Z)", a1.z - a0.z > 2f, V(a1 - a0));
            Check("Astronaut stays grounded while walking", cc.isGrounded || Mathf.Abs(a1.y - GroundY(a1)) < 0.3f, $"y {a1.y:F2} ground {GroundY(a1):F2}");
            e0 = p.elapsed; a0 = cc.transform.position;
            Input(new Vector2(0, 1), run: true);
            yield return Sim(2f);
            float run = Vector3.ProjectOnPlane(cc.transform.position - a0, Vector3.up).magnitude / (p.elapsed - e0);
            Check("Run is faster than walk", run > walk + 0.8f, $"run {run:F2} m/s vs walk {walk:F2}");
            Teleport(cc, OpenSpot(wSpot), 0);
            yield return Sim(0.3f);
            p.yaw = 90; a0 = cc.transform.position;
            Input(new Vector2(0, 1));
            yield return Sim(1.5f);
            Check("Turning the camera turns the walk direction (yaw 90 → +X)", cc.transform.position.x - a0.x > 1.5f, V(cc.transform.position - a0));
            a0 = cc.transform.position;
            Input(Vector2.zero);
            yield return Sim(1f);
            Check("Astronaut stops when input is released", Vector3.ProjectOnPlane(cc.transform.position - a0, Vector3.up).magnitude < 0.3f);

            // 2. Astronaut collisions --------------------------------------------------------------
            var structure = LargestStructureNear(cc.transform.position);
            if (structure)
            {
                var b = structure.bounds;
                Vector3 dir = (new Vector3(-10, 0, -20) - b.center); dir.y = 0; dir = dir.sqrMagnitude < .01f ? Vector3.back : dir.normalized;
                Vector3 start = b.center + dir * (Mathf.Max(b.extents.x, b.extents.z) + 4);
                float yawTo = Mathf.Atan2(-dir.x, -dir.z) * Mathf.Rad2Deg;
                Teleport(cc, start, yawTo);
                p.yaw = yawTo;
                int before = astronautPenetration;
                Input(new Vector2(0, 1), run: true);
                yield return Sim(6f);
                Input(Vector2.zero);
                Vector3 end = cc.transform.position;
                float horiz = Vector2.Distance(new Vector2(end.x, end.z), new Vector2(b.center.x, b.center.z));
                Check("Astronaut is blocked by structure '" + structure.name + "'", astronautPenetration == before && horiz > Mathf.Min(b.extents.x, b.extents.z) * 0.5f, $"stopped {horiz:F1} m from centre, extents {b.extents.x:F1}/{b.extents.z:F1}, overlaps {astronautPenetration - before}");
            }
            else Check("Found a structure to test astronaut collision", false);

            Teleport(cc, new Vector3(114, 0, 0), 90);
            p.yaw = 90;
            Input(new Vector2(0, 1), run: true);
            yield return Sim(3f);
            Input(Vector2.zero);
            Check("World boundary holds the astronaut inside ±119 m", Mathf.Abs(cc.transform.position.x) <= 119.01f, V(cc.transform.position));

            yield return Normalize();
            // 3. Rover boarding ---------------------------------------------------------------------
            var rover = p.rover;
            Teleport(rover, rSpot, 0);
            Teleport(cc, rover.transform.position + rover.transform.right * 9, 0);
            yield return Sim(0.5f);
            hud.Interact();
            if (hud.modalOpen) hud.CloseModal();
            Check("Boarding is refused when the rover is 9 m away", !p.driving);
            Teleport(cc, rover.transform.position + rover.transform.right * 3.5f, 0);
            yield return Sim(0.5f);
            hud.Interact();
            yield return Sim(0.2f);
            Check("Boarding works within 5 m", p.driving, hud.lastResult);
            Check("Mode switches to Rover", modes.Current == ProductionMode.Rover && hud.game.modes.Mode == ControlMode.Rover, modes.Current.ToString());
            Check("Astronaut is seated in the rover (same object)", p.astronaut.gameObject == identity && p.astronaut.transform.parent == hud.game.seat && !p.astronaut.enabled);

            // 4. Driving ----------------------------------------------------------------------------
            p.yaw = rover.transform.eulerAngles.y;
            Vector3 r0 = rover.transform.position; e0 = p.elapsed;
            Input(new Vector2(0, 1), accelerate: true);
            yield return Sim(4f);
            float topSpeed = p.speed;
            float travelled = Vector3.ProjectOnPlane(rover.transform.position - r0, Vector3.up).magnitude;
            Check("Rover accelerates forward", topSpeed > 3f && travelled > 6f, $"speed {topSpeed:F2} m/s (max 3.8), travelled {travelled:F1} m");
            Check("Rover stays on the ground while driving", Mathf.Abs(rover.transform.position.y - GroundY(rover.transform.position)) < 1.5f, $"y {rover.transform.position.y:F2} ground {GroundY(rover.transform.position):F2}");
            Check("Exit is refused while the rover is moving", !hud.game.Interact() && p.driving);
            float h0 = rover.transform.eulerAngles.y;
            Input(new Vector2(1, 1), accelerate: true);
            yield return Sim(2f);
            Check("Steering turns the rover", Mathf.Abs(Mathf.DeltaAngle(h0, rover.transform.eulerAngles.y)) > 20, $"{Mathf.DeltaAngle(h0, rover.transform.eulerAngles.y):F1}°");
            Input(Vector2.zero, brake: true);
            yield return Sim(1.5f);
            Check("Brake stops the rover", Mathf.Abs(p.speed) < 0.2f, $"{p.speed:F2} m/s");
            Input(new Vector2(0, -1));
            yield return Sim(2f);
            Check("Rover reverses", p.speed < -1f, $"{p.speed:F2} m/s (max -1.8)");
            Input(Vector2.zero, brake: true);
            yield return Sim(1.5f);
            Check("Wheels roll with measured travel", Mathf.Abs(art.wheelRollDegrees) > 1f, $"{art.wheelRollDegrees:F1}°");

            // 5. Rover collision ---------------------------------------------------------------------
            var roverTarget = LargestStructureNear(rover.transform.position);
            if (roverTarget)
            {
                var b = roverTarget.bounds;
                Vector3 dir = rover.transform.position - b.center; dir.y = 0; dir = dir.sqrMagnitude < .01f ? Vector3.back : dir.normalized;
                float yawTo = Mathf.Atan2(-dir.x, -dir.z) * Mathf.Rad2Deg;
                Teleport(rover, b.center + dir * (Mathf.Max(b.extents.x, b.extents.z) + 9), yawTo);
                p.speed = 0;
                yield return Sim(0.5f);
                int before = roverPenetration;
                Input(new Vector2(0, 1), accelerate: true);
                yield return Sim(7f);
                Vector3 end = rover.transform.position;
                float horiz = Vector2.Distance(new Vector2(end.x, end.z), new Vector2(b.center.x, b.center.z));
                Check("Rover is blocked by structure '" + roverTarget.name + "'", roverPenetration == before && horiz > Mathf.Min(b.extents.x, b.extents.z) * 0.5f, $"stopped {horiz:F1} m from centre, overlaps {roverPenetration - before}, speed {p.speed:F2}");
                Check("Rover speed drops to zero against a wall", Mathf.Abs(p.speed) < 0.5f, $"{p.speed:F2} m/s");
                Input(Vector2.zero, brake: true);
                yield return Sim(1f);
            }
            else Check("Found a structure to test rover collision", false);

            // 6. Exit --------------------------------------------------------------------------------
            Teleport(rover, rSpot, 0);
            p.speed = 0;
            Input(Vector2.zero, brake: true);
            yield return Sim(1f);
            hud.Interact();
            yield return Sim(0.3f);
            Check("Exit works once stopped", !p.driving, hud.lastResult);
            Check("Exit returns the same astronaut, enabled and unparented", p.astronaut.gameObject == identity && cc.enabled && cc.transform.parent != hud.game.seat);
            Check("Mode returns to Astronaut", modes.Current == ProductionMode.Astronaut && hud.game.modes.Mode == ControlMode.Astronaut, modes.Current.ToString());
            Check("Astronaut exits clear of the rover and grounded", Overlaps(cc).Length == 0 && Mathf.Abs(cc.transform.position.y - GroundY(cc.transform.position)) < 0.4f, V(cc.transform.position) + " ground " + GroundY(cc.transform.position).ToString("F2"));
            a0 = cc.transform.position; p.yaw = 0;
            Input(new Vector2(0, 1));
            yield return Sim(1f);
            Check("Astronaut can walk after exiting", Vector3.Distance(a0, cc.transform.position) > 1f);
            Input(Vector2.zero);

            yield return Normalize();
            // Exit blocked on both sides.
            Teleport(cc, rover.transform.position + rover.transform.right * 3.5f, 0);
            yield return Sim(0.3f);
            hud.Interact();
            yield return Sim(0.3f);
            Line("Before walls: " + State());
            var wallL = GameObject.CreatePrimitive(PrimitiveType.Cube); wallL.name = "Step1 test wall L";
            var wallR = GameObject.CreatePrimitive(PrimitiveType.Cube); wallR.name = "Step1 test wall R";
            foreach (var (w, side) in new[] { (wallL, -1f), (wallR, 1f) })
            {
                w.transform.SetPositionAndRotation(rover.transform.position + rover.transform.right * side * 3f + Vector3.up, rover.transform.rotation);
                w.transform.localScale = new Vector3(1.2f, 20, 8);
            }
            Physics.SyncTransforms();
            bool exitBlocked = !hud.game.Interact();
            Check("Exit is refused when both sides are blocked", exitBlocked && p.driving && p.astronaut.transform.parent == hud.game.seat, State());
            UnityEngine.Object.Destroy(wallL); UnityEngine.Object.Destroy(wallR);
            yield return Sim(0.3f);
            hud.Interact();
            yield return Sim(0.3f);
            Check("Exit works again after the blockage is removed", !p.driving, State());

            yield return Normalize();
            // Exit next to a low obstacle (4 m): the astronaut should land on the ground, not on top of it.
            yield return Normalize();
            Teleport(rover, rSpot, 0);
            Teleport(cc, rover.transform.position + rover.transform.right * 3.5f, 0);
            yield return Sim(0.3f);
            hud.Interact();
            yield return Sim(0.3f);
            var lowL = GameObject.CreatePrimitive(PrimitiveType.Cube); lowL.name = "Step1 low wall L";
            var lowR = GameObject.CreatePrimitive(PrimitiveType.Cube); lowR.name = "Step1 low wall R";
            foreach (var (w, side) in new[] { (lowL, -1f), (lowR, 1f) })
            {
                w.transform.SetPositionAndRotation(rover.transform.position + rover.transform.right * side * 3f + Vector3.up * 1f, rover.transform.rotation);
                w.transform.localScale = new Vector3(1.2f, 4, 8);
            }
            Physics.SyncTransforms();
            float roverGround = GroundY(rover.transform.position);
            bool lowExit = hud.game.Interact();
            float lift = cc.transform.position.y - roverGround;
            Check("Exit beside 4 m obstacles does not put the astronaut on top of them", !lowExit || lift < 1f, $"exit {lowExit}, astronaut {lift:F2} m above rover ground, " + State());
            UnityEngine.Object.Destroy(lowL); UnityEngine.Object.Destroy(lowR);
            yield return Sim(0.3f);

            // Exit with the rover parked against the Laboratory Dome on its left (the side tried first).
            yield return Normalize();
            var dome = UnityEngine.Object.FindObjectsByType<Collider>(FindObjectsSortMode.None).FirstOrDefault(c => c.name == "Laboratory Dome");
            if (dome)
            {
                var b = dome.bounds;
                Vector3 dir = Vector3.ProjectOnPlane(rSpot - b.center, Vector3.up).normalized;
                float yawRight = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg - 90f;   // rover.right points away from the dome
                bool parked = false;
                for (float gap = 1.4f; gap < 6f && !parked; gap += 0.2f)
                {
                    Teleport(rover, b.center + dir * (b.extents.x + gap), yawRight);
                    parked = Overlaps(rover).Length == 0;
                }
                Teleport(cc, rover.transform.position + rover.transform.right * 3.5f, 0);
                yield return Sim(0.3f);
                hud.Interact();
                yield return Sim(0.3f);
                Line("Parked beside dome at " + V(rover.transform.position) + ", parked clear " + parked + ", left probe ground " + GroundY(rover.transform.position - rover.transform.right * 3f).ToString("F2") + ", " + State());
                p.speed = 0;
                float g0 = GroundY(rover.transform.position);
                bool exited = hud.game.Interact();
                yield return Sim(0.5f);
                float liftDome = cc.transform.position.y - g0;
                Check("Exit beside the Laboratory Dome lands on the ground", exited && liftDome < 1f, $"exit {exited}, astronaut at {V(cc.transform.position)}, {liftDome:F2} m above rover ground, " + State());
            }
            else Check("Found Laboratory Dome for exit test", false);

            // 7. Scientific scanning ------------------------------------------------------------------
            int scansBefore = hud.game.scans.Count;
            int scanned = 0;
            foreach (var target in hud.environment.targets.Where(t => t))
            {
                Vector3 tp = target.transform.position;
                Vector3 offset = Vector3.ProjectOnPlane(tp - rover.transform.position, Vector3.up).normalized;
                if (offset.sqrMagnitude < .01f) offset = Vector3.forward;
                Teleport(cc, tp - offset * 2f, 0);
                if (Overlaps(cc).Length > 0) Teleport(cc, tp + offset * 2f, 0);
                yield return Sim(0.4f);
                float d = Vector3.Distance(cc.transform.position, tp);
                int n = hud.game.scans.Count;
                hud.BeginScan();
                bool began = hud.scanning && modes.Current == ProductionMode.Scanner;
                Vector3 still = cc.transform.position;
                Input(new Vector2(0, 1));
                yield return Real(1.4f);
                Input(Vector2.zero);
                bool recorded = hud.game.scans.Count == n + 1;
                Check("Scan '" + target.displayName + "'", began && recorded && modes.Current == ProductionMode.Astronaut, $"distance {d:F2} m, began {began}, recorded {recorded}, mode {modes.Current}, result '{hud.lastResult}'");
                Check("Movement is locked during scan of '" + target.displayName + "'", Vector3.ProjectOnPlane(cc.transform.position - still, Vector3.up).magnitude < 0.2f);
                if (recorded) scanned++;
            }
            hud.Refresh();
            Check("Research station objective counts scanned targets", hud.objectiveScans >= scanned && scanned > 0, $"{hud.objectiveScans} of {hud.environment.targets.Length}");
            Teleport(cc, new Vector3(-60, 0, 60), 0);
            yield return Sim(0.3f);
            int c0 = hud.game.scans.Count;
            hud.BeginScan();
            Check("Scan is refused with no target in range", !hud.scanning && hud.game.scans.Count == c0, hud.lastResult);
            var first = hud.environment.targets.First(t => t);
            Teleport(cc, first.transform.position - Vector3.forward * 2f, 0);
            if (Overlaps(cc).Length > 0) Teleport(cc, first.transform.position + Vector3.forward * 2f, 0);
            yield return Sim(0.3f);
            hud.BeginScan();
            bool started = hud.scanning;
            hud.CancelScan();
            yield return Real(1.2f);
            Check("Cancelled scan records nothing and restores control", started && !hud.scanning && hud.game.scans.Count == c0 && modes.Current == ProductionMode.Astronaut, modes.Current.ToString());
            Check("Scan records are labelled simulated", hud.game.scans.Skip(scansBefore).All(s => s.analysis == "unverified_simulated"));

            yield return Normalize();
            // 8. Camera modes -------------------------------------------------------------------------
            Teleport(cc, wSpot, 0);
            p.yaw = 0; p.pitch = 18;
            yield return Sim(2f);
            float camWalk = Vector3.Distance(p.follow.transform.position, rig.LastTarget);
            Check("Walking chase camera sits behind the astronaut", (camWalk > 3f && camWalk < 6f) || rig.LastObstructed, $"{camWalk:F2} m, obstructed {rig.LastObstructed}");
            Teleport(rover, OpenSpot(new Vector3(-24, 0, -40), 9f), 0);
            Teleport(cc, rover.transform.position + rover.transform.right * 3.5f, 0);
            yield return Sim(0.3f);
            hud.Interact();
            Line("Chase test boarding: " + State());
            p.yaw = rover.transform.eulerAngles.y;
            rig.nearChase = false;
            yield return Sim(2.5f);
            float far = Vector3.Distance(p.follow.transform.position, rig.LastTarget);
            rig.nearChase = true;
            yield return Sim(2.5f);
            float near = Vector3.Distance(p.follow.transform.position, rig.LastTarget);
            rig.nearChase = false;
            Check("Rover chase distance toggle (far vs near)", far > near + 1f, $"far {far:F2} m, near {near:F2} m, " + State());
            Vector3 beforeCine = rover.transform.position;
            bool cine = modes.Request(ProductionMode.Cinematic);
            Input(new Vector2(0, 1), accelerate: true);
            yield return Sim(2.5f);
            Check("Cinematic overview takes the camera and locks driving input", cine && modes.Current == ProductionMode.Cinematic && Vector3.Distance(p.follow.transform.position, hud.environment.scenic.transform.position) < 3f && Vector3.Distance(beforeCine, rover.transform.position) < 0.5f,
                $"camera {Vector3.Distance(p.follow.transform.position, hud.environment.scenic.transform.position):F2} m from scenic point, rover moved {Vector3.Distance(beforeCine, rover.transform.position):F2} m");
            modes.Request(ProductionMode.Cinematic);
            yield return Sim(2.5f);
            Check("Leaving cinematic restores Rover mode and chase camera", modes.Current == ProductionMode.Rover && Vector3.Distance(p.follow.transform.position, rig.LastTarget) < 9f, modes.Current.ToString());
            hud.Interact();
            yield return Sim(0.3f);

            yield return Normalize();
            // Camera against a wall.
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube); wall.name = "Step1 camera wall";
            Teleport(cc, wSpot, 0); p.yaw = 0; p.pitch = 18;
            wall.transform.SetPositionAndRotation(cc.transform.position + Vector3.back * 2.2f + Vector3.up * 2, Quaternion.identity);
            wall.transform.localScale = new Vector3(8, 6, 0.4f);
            Physics.SyncTransforms();
            yield return Sim(2f);
            Check("Camera pulls in when a wall is behind the astronaut", rig.LastObstructed && p.follow.transform.position.z > wall.transform.position.z, $"camera z {p.follow.transform.position.z:F2}, wall z {wall.transform.position.z:F2}");
            UnityEngine.Object.Destroy(wall);
            yield return Sim(0.5f);

            yield return Normalize();
            // 9. Pause / menu ---------------------------------------------------------------------------
            hud.TogglePause();
            float pausedAt = p.elapsed;
            Vector3 pausedPos = cc.transform.position;
            Input(new Vector2(0, 1));
            yield return Real(1f);
            Check("Pause stops simulation and input", hud.environment.paused && Time.timeScale == 0 && Mathf.Abs(p.elapsed - pausedAt) < 0.01f && Vector3.Distance(pausedPos, cc.transform.position) < 0.05f && modes.Current == ProductionMode.Menu);
            hud.TogglePause();
            yield return Sim(0.3f);
            Check("Resume restores Astronaut mode", !hud.environment.paused && modes.Current == ProductionMode.Astronaut && Time.timeScale > 0, State() + $", timeScale {Time.timeScale}");
            Input(new Vector2(0, 1));
            a0 = cc.transform.position;
            yield return Sim(1f);
            Check("Astronaut moves after resume", Vector3.Distance(a0, cc.transform.position) > 1f);
            Input(Vector2.zero);

            yield return Normalize();
            // Cinematic → pause → resume (state hand-off between coordinator and environment).
            modes.Request(ProductionMode.Cinematic);
            yield return Sim(0.3f);
            hud.TogglePause();
            yield return Real(0.5f);
            hud.TogglePause();
            yield return Sim(0.5f);
            Check("Pause during cinematic resumes into normal gameplay", !modes.cinematic && modes.Current == ProductionMode.Astronaut && hud.InputAllowed, modes.Current.ToString());

            yield return Normalize();
            // Control tuning modal.
            art.OpenTuning();
            yield return Real(0.3f);
            Check("Control tuning pauses and blocks input", modes.Current == ProductionMode.Menu && !hud.InputAllowed);
            art.CloseTuning();
            yield return Sim(0.3f);
            Check("Closing control tuning restores input", modes.Current == ProductionMode.Astronaut && hud.InputAllowed, State());

            yield return Normalize();
            // 10. Save / load (lab persistent path only) ------------------------------------------------
            Teleport(cc, new Vector3(-15, 0, -35), 0);
            yield return Sim(0.3f);
            Vector3 saved = cc.transform.position;
            int savedScans = hud.game.scans.Count;
            bool ok = hud.SaveLocal();
            Teleport(cc, new Vector3(10, 0, 10), 0);
            yield return Sim(0.3f);
            ok &= hud.LoadLocal();
            yield return Sim(0.3f);
            Check("Local save and load restores position and scans", ok && Vector3.Distance(saved, cc.transform.position) < 0.5f && hud.game.scans.Count == savedScans, $"ok {ok}, scans {savedScans} → {hud.game.scans.Count}, {V(saved)} → {V(cc.transform.position)}, sync '{hud.syncStatus}', notice '{hud.game.notice}', path {hud.game.testSave}");

            yield return Normalize();
            // 11. Real keyboard path (HUDInputRouter + coordinator) -------------------------------------
            priorBackground = InputSystem.settings.backgroundBehavior;
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            priorEditorInput = InputSystem.settings.editorInputBehaviorInPlayMode;
            InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            keyboard = InputSystem.AddDevice<Keyboard>("Step1 virtual keyboard");
            keyboard.MakeCurrent();
            hud.game.testing = false;
            yield return Sim(0.2f);
            Teleport(cc, wSpot, 0);
            p.yaw = 0;
            yield return Sim(0.2f);
            a0 = cc.transform.position;
            PressKey(Key.W);
            yield return Sim(1.5f);
            ReleaseKeys();
            Check("Keyboard W walks the astronaut", Vector3.Distance(a0, cc.transform.position) > 1.5f, $"{Vector3.Distance(a0, cc.transform.position):F2} m, " + State());
            Teleport(rover, rSpot, 0);
            Teleport(cc, rover.transform.position + rover.transform.right * 3.5f, 0);
            yield return Sim(0.3f);
            PressKey(Key.E); yield return Sim(0.1f); ReleaseKeys();
            yield return Sim(0.3f);
            Check("Keyboard E boards the rover", p.driving, State());
            r0 = rover.transform.position;
            PressKey(Key.W); yield return Sim(2f); ReleaseKeys();
            Check("Keyboard W drives the rover", Vector3.Distance(r0, rover.transform.position) > 2f, $"{Vector3.Distance(r0, rover.transform.position):F2} m");
            PressKey(Key.Space); yield return Sim(2f); ReleaseKeys();
            PressKey(Key.E); yield return Sim(0.1f); ReleaseKeys();
            yield return Sim(0.3f);
            Check("Keyboard Space then E stops and exits", !p.driving, hud.lastResult);
            var kt = hud.environment.targets.First(t => t);
            Teleport(cc, kt.transform.position - Vector3.forward * 2f, 0);
            if (Overlaps(cc).Length > 0) Teleport(cc, kt.transform.position + Vector3.forward * 2f, 0);
            yield return Sim(0.3f);
            int k0 = hud.game.scans.Count;
            PressKey(Key.K); yield return Sim(0.1f); ReleaseKeys();
            yield return Real(1.4f);
            Check("Keyboard K scans a nearby target", hud.game.scans.Count == k0 + 1, State());
            PressKey(Key.C); yield return Sim(0.1f); ReleaseKeys();
            yield return Sim(0.3f);
            bool cOn = modes.Current == ProductionMode.Cinematic;
            PressKey(Key.C); yield return Sim(0.1f); ReleaseKeys();
            yield return Sim(0.3f);
            Check("Keyboard C toggles the cinematic overview", cOn && modes.Current == ProductionMode.Astronaut, $"on {cOn}, " + State());
            PressKey(Key.Escape); yield return Real(0.2f); ReleaseKeys();
            yield return Real(0.3f);
            bool escPaused = hud.environment.paused;
            PressKey(Key.Escape); yield return Real(0.2f); ReleaseKeys();
            yield return Real(0.5f);
            Check("Keyboard Esc pauses and resumes", escPaused && !hud.environment.paused && modes.Current == ProductionMode.Astronaut, $"paused after first Esc {escPaused}, " + State());
            hud.game.testing = true;

            yield return Normalize();
            // 12. Long drive across terrain ---------------------------------------------------------------
            Teleport(rover, rSpot, 0);
            Teleport(cc, rover.transform.position + rover.transform.right * 3.5f, 0);
            yield return Sim(0.3f);
            hud.Interact();
            int below = belowGround;
            Input(new Vector2(0.35f, 1), accelerate: true);
            yield return Sim(20f);
            Input(Vector2.zero, brake: true);
            yield return Sim(2f);
            Check("20 s steering drive stays on terrain", belowGround == below && p.driving, $"end {V(rover.transform.position)}");
            hud.Interact();
            yield return Sim(0.3f);

            var kinds = IntegrationServices.Sessions.Pending.Select(c => c.kind).ToList();
            Check("Bridge emitted board, exit and scan commands", kinds.Contains(CommandKinds.BoardRover) && kinds.Contains(CommandKinds.ExitRover) && kinds.Contains(CommandKinds.ScanTarget), string.Join(",", kinds.GroupBy(k => k).Select(g => g.Key + "×" + g.Count())));
            Check("Command ids are unique (idempotency keys)", IntegrationServices.Sessions.Pending.Select(c => c.commandId).Distinct().Count() == IntegrationServices.Sessions.Pending.Count);
            p.SetWeather(2);
            yield return Sim(0.3f);
            Check("Weather feed reports the dust storm", IntegrationServices.Weather != null && IntegrationServices.Weather.Current.dust == DustLevel.DustStorm);
            Check("Life-support model matches existing storm oxygen rate", Mathf.Approximately(IntegrationServices.LifeSupport.OxygenUsePerSecond(Activity.Walking, IntegrationServices.Weather.Current), 0.035f));
            p.SetWeather(0);
            MissionReport report = null;
            IntegrationServices.Assessment.RequestReport(IntegrationServices.Sessions.Session.sessionId, r => report = r);
            Check("Local assessment preview is marked provisional", report != null && !report.authoritative && report.rubricVersion.Contains("unreviewed"), report != null ? "score " + report.score : "null");
            UnityEngine.Object.Destroy(bridge.gameObject);

            monitoring = false;
            Check("Camera never inside geometry (gameplay modes)", cameraInside == 0, $"{cameraInside} of {frames} frames");
            Check("Astronaut never overlapped scenery", astronautPenetration == 0, $"{astronautPenetration} of {frames} frames");
            Check("Rover never overlapped scenery", roverPenetration == 0, $"{roverPenetration} of {frames} frames");
            Check("Nothing fell below the ground", belowGround == 0, $"{belowGround} of {frames} frames");
            Check("No errors or exceptions logged during play", logErrors.Count == 0, logErrors.Count + " logged");
        }

        static void Finish()
        {
            routine = null;
            monitoring = false;
            try
            {
                if (keyboard != null) { InputSystem.RemoveDevice(keyboard); keyboard = null; InputSystem.settings.backgroundBehavior = priorBackground; InputSystem.settings.editorInputBehaviorInPlayMode = priorEditorInput; }
            }
            catch { }
            Application.logMessageReceived -= OnLog;
            Directory.CreateDirectory(Out);
            var all = new List<string> { $"Step 1 gameplay verification — {DateTime.Now:yyyy-MM-dd HH:mm:ss}", $"PASS {pass}  FAIL {fail}", "" };
            all.AddRange(lines);
            all.Add(""); all.Add("Monitor samples (first 40):"); all.AddRange(monitorSamples);
            all.Add(""); all.Add("Logged errors:"); all.AddRange(logErrors.Distinct().Take(60));
            File.WriteAllLines(Path.Combine(Out, "Step1_Gameplay_Results.txt"), all);
            EditorApplication.Exit(fail == 0 ? 0 : 1);
        }
    }
}
