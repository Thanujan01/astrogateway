using UnityEngine;
using UnityEngine.InputSystem;
using AstroGateway.CinematicHUD;
using AstroGateway.ProductionVisual;

// Camera for Mars_FirstMission_v1 (replaces VisualUpgradeCameraRig in this scene only; that rig stays in the
// Visual Upgrade scene unchanged). Keeps the Visual Upgrade framing and its collision solver (sphere-cast pull-in,
// pivot raise for tight spaces such as the Crew Dome airlock, sweep, "never inside geometry" fallback) and adds:
//  - astronaut views: FOLLOW (default) / SHOULDER (left or right) / INSPECT (first person)
//  - rover views: CHASE / CHASE NEAR / MAST CAM (first person from the navigation mast; the canopy has no interior)
//  - zoom (pinch, mouse wheel, or the settings), recenter behind the rover
//  - ROVER_SCIENCE framing of the selected research station
//  - reduced-motion option (slower blends, no FOV changes)
// Look input itself (orbit) still comes through MarsPrototype.yaw / pitch, shaped by FirstMissionControlLayer.
namespace AstroGateway.FirstMission
{
    public enum AstronautView { Follow, Shoulder, Inspect }
    public enum RoverView { Chase, ChaseNear, Cockpit }

    [DefaultExecutionOrder(520)]
    public class FirstMissionCameraRig : MonoBehaviour
    {
        public AstroGatewayHUDController hud;
        public ProductionModeCoordinator modes;
        public ProductionCameraRig legacy;          // still owns captureOverride for the existing harnesses
        public FirstMissionController mission;
        public FirstMissionControlLayer control;
        public float clearance = .22f, margin = .12f, minDistance = 1.2f, nearClip = .08f;
        public float followSharpness = 7f, cinematicSharpness = 2.2f, transitionSeconds = 1.6f;
        public LayerMask mask = 1;
        public AstronautView astronautView;
        public RoverView roverView;
        [Range(.6f, 1.6f)] public float zoom = 1;
        public Vector3 cockpitLocal = new Vector3(0, 1.55f, .55f);   // in ARES rover space (rover scale applied)
        public bool LastObstructed, LastRaised;
        public int InsideFrames;
        public float LastTransitionJump { get; private set; }  // largest per-frame camera move (m) after a view change
        public float LastTransitionSpeed { get; private set; } // largest camera speed (m/s) after a view change

        Vector3 smoothed; Quaternion rotation; bool initialized;
        ProductionMode lastMode; float modeChangedAt = -10, viewChangedAt = -10;
        readonly Collider[] overlap = new Collider[16];
        Renderer[] astronautRenderers; bool hidingAstronaut;
        static readonly float[] Raise = { .6f, 1.2f, 1.8f, 2.4f, 3.0f };

        public void CycleAstronautView() { astronautView = (AstronautView)(((int)astronautView + 1) % 3); viewChangedAt = Time.unscaledTime; LastTransitionJump = 0; LastTransitionSpeed = 0; }
        public void CycleRoverView() { roverView = (RoverView)(((int)roverView + 1) % 3); viewChangedAt = Time.unscaledTime; LastTransitionJump = 0; LastTransitionSpeed = 0; }
        public void Recenter()
        {
            var p = hud.game.prototype;
            p.yaw = p.driving ? p.rover.transform.eulerAngles.y : p.astronaut.transform.eulerAngles.y;
            p.pitch = p.driving ? 14 : 12;
            viewChangedAt = Time.unscaledTime; LastTransitionJump = 0; LastTransitionSpeed = 0;
        }
        public void Zoom(float delta) => zoom = Mathf.Clamp(zoom + delta, .6f, 1.6f);
        public string ViewLabel => hud.game.prototype.driving ? roverView switch { RoverView.ChaseNear => "CHASE NEAR", RoverView.Cockpit => "MAST CAM", _ => "CHASE" }
            : astronautView switch { AstronautView.Shoulder => "SHOULDER", AstronautView.Inspect => "INSPECT", _ => "FOLLOW" };

        void Start() { astronautRenderers = hud.game.prototype.astronaut.GetComponentsInChildren<Renderer>(true); }

        public double LastCostMs { get; private set; }
        readonly System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

        void LateUpdate()
        {
            watch.Restart();
            Step();
            LastCostMs = watch.Elapsed.TotalMilliseconds;
        }

        void Step()
        {
            var p = hud.game.prototype;
            var cam = p.follow;
            if (!cam || !mission || !mission.Ready) return;
            if (cam.nearClipPlane > nearClip) cam.nearClipPlane = nearClip;
            var s = mission.Settings;
            bool reduced = s.reducedMotion;

            var mode = modes.Current;
            if (mode != lastMode && (mode == ProductionMode.Cinematic || lastMode == ProductionMode.Cinematic)) modeChangedAt = Time.unscaledTime;
            lastMode = mode;

            // Mouse wheel zoom (desktop) when no modal is open.
            var m = Mouse.current;
            if (m != null && !hud.modalOpen && !hud.game.testing) { float w = m.scroll.ReadValue().y; if (Mathf.Abs(w) > .01f) Zoom(-Mathf.Sign(w) * .08f); }

            Vector3 target = p.ActivePosition() + Vector3.up * (p.driving ? 1.45f : 1.4f), desired;
            float fov = p.driving ? 61 : 55;
            bool free = false, firstPerson = false;
            float stationRange = float.MaxValue;
            var station = control && control.State == ControlState.RoverScience ? mission.sensors.Target(out stationRange) : null;
            if (legacy.captureOverride) { desired = legacy.capturePosition; target = legacy.captureTarget; fov = 58; free = true; }
            else if (mode == ProductionMode.Cinematic) { desired = hud.environment.scenic.transform.position; target = new Vector3(-36, 4, -43); fov = 58; free = true; }
            else if (mode == ProductionMode.Scanner && hud.CurrentTarget())
            {
                var t = hud.CurrentTarget().transform.position + Vector3.up * .5f;
                target = Vector3.Lerp(target, t, .6f);
                desired = p.ActivePosition() + Quaternion.Euler(12, p.yaw - 25, 0) * new Vector3(0, 2, -3.4f);
                fov = 49;
            }
            else if (station && stationRange <= 25)
            {
                // Science framing: rover and the selected station both in view, from behind the rover's shoulder.
                var st = station.transform.position + Vector3.up * .8f;
                target = Vector3.Lerp(target, st, .45f);
                var away = Vector3.ProjectOnPlane(p.rover.transform.position - st, Vector3.up).normalized;
                if (away.sqrMagnitude < .01f) away = -p.rover.transform.forward;
                desired = p.rover.transform.position + away * (6.5f * zoom) + Vector3.up * 3.2f + Vector3.Cross(Vector3.up, away) * 2.2f;
                fov = 52;
            }
            else if (p.driving && roverView == RoverView.Cockpit)
            {
                desired = p.rover.transform.TransformPoint(cockpitLocal);
                target = desired + Quaternion.Euler(Mathf.Clamp(p.pitch - 14, -20, 30), p.yaw, 0) * Vector3.forward * 10;
                fov = 66; free = true; firstPerson = true;
            }
            else if (!p.driving && astronautView == AstronautView.Inspect)
            {
                desired = p.astronaut.transform.position + Vector3.up * 1.62f;
                target = desired + Quaternion.Euler(Mathf.Clamp(p.pitch - 12, -40, 40), p.yaw, 0) * Vector3.forward * 10;
                fov = 60; free = true; firstPerson = true;
            }
            else
            {
                float dist = p.driving ? (roverView == RoverView.ChaseNear || legacy.nearChase ? 5.5f : 7.5f) : astronautView == AstronautView.Shoulder ? 2.6f : 4.8f;
                float side = !p.driving && astronautView == AstronautView.Shoulder ? 1.0f * s.shoulderSide : .35f;
                desired = target + Quaternion.Euler(p.pitch, p.yaw, 0) * new Vector3(side, .15f, -dist * zoom);
                if (!p.driving && astronautView == AstronautView.Shoulder) target += Quaternion.Euler(0, p.yaw, 0) * new Vector3(.95f * s.shoulderSide, .1f, 0);
            }
            if (reduced) fov = p.driving ? 61 : 55;

            // First-person views hide the astronaut so the camera never sees the inside of the helmet.
            bool hide = firstPerson && !p.driving;
            if (hide != hidingAstronaut)
            {
                hidingAstronaut = hide;
                foreach (var r in astronautRenderers) if (r) r.enabled = !hide && !p.driving;
            }

            LastObstructed = false; LastRaised = false;
            Vector3 pivot = target;
            if (!free) desired = Resolve(target, desired, out pivot);

            if (!initialized) { smoothed = cam.transform.position; rotation = cam.transform.rotation; initialized = true; }
            bool transitioning = Time.unscaledTime - modeChangedAt < transitionSeconds;
            bool viewChanging = Time.unscaledTime - viewChangedAt < 1.2f;   // camera view / recenter: glide, never snap
            float sharp = transitioning ? cinematicSharpness : viewChanging ? 4.5f : firstPerson ? 18f : followSharpness;
            if (reduced) sharp *= .55f;
            float blend = 1 - Mathf.Exp(-sharp * Time.unscaledDeltaTime);
            var before = smoothed;
            smoothed = Vector3.Lerp(smoothed, desired, blend);
            // During a view change the camera glides at no more than 9 m/s, whatever the frame time.
            if (viewChanging) smoothed = Vector3.MoveTowards(before, smoothed, 9f * Mathf.Max(Time.unscaledDeltaTime, .001f));
            if (!free) smoothed = Sweep(pivot, smoothed);
            if (!free && Blocked(smoothed, clearance * .5f)) { smoothed = pivot; InsideFrames++; }
            if (Time.unscaledTime - viewChangedAt < 1.5f) { LastTransitionJump = Mathf.Max(LastTransitionJump, Vector3.Distance(before, smoothed)); LastTransitionSpeed = Mathf.Max(LastTransitionSpeed, Vector3.Distance(before, smoothed) / Mathf.Max(Time.unscaledDeltaTime, .001f)); }

            var aim = Quaternion.LookRotation(target - smoothed, Vector3.up);
            rotation = Quaternion.Slerp(rotation, aim, firstPerson && !viewChanging ? 1 - Mathf.Exp(-20 * Time.unscaledDeltaTime) : blend);
            cam.transform.SetPositionAndRotation(smoothed, rotation);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, blend);
            hud.environment.scenic.enabled = false;
            cam.enabled = true;
            legacy.LastTarget = target;
            legacy.LastObstructed = LastObstructed;
        }

        // ---- collision solver: same behaviour as VisualUpgradeCameraRig ----
        Vector3 Resolve(Vector3 target, Vector3 desired, out Vector3 pivot)
        {
            pivot = target;
            Vector3 offset = desired - target;
            float ideal = offset.magnitude;
            if (ideal < .01f) return desired;
            float allowed = Allowed(target, offset / ideal, ideal);
            if (allowed >= ideal) return desired;
            LastObstructed = true;
            if (allowed >= minDistance) return target + offset / ideal * allowed;
            Vector3 best = target + offset / ideal * allowed; float bestAllowed = allowed; Vector3 bestPivot = target;
            Vector3 flat = Vector3.ProjectOnPlane(offset, Vector3.up).normalized;
            if (flat.sqrMagnitude < .01f) flat = -hud.game.prototype.follow.transform.forward;
            foreach (float h in Raise)
            {
                if (Physics.SphereCast(target, clearance, Vector3.up, out _, h + clearance, mask, QueryTriggerInteraction.Ignore)) break;
                Vector3 raised = target + Vector3.up * h;
                Vector3 dir = (flat * ideal + Vector3.up * (offset.y * .5f)).normalized;
                float a = Allowed(raised, dir, ideal);
                if (a > bestAllowed) { bestAllowed = a; best = raised + dir * a; bestPivot = raised; }
                if (a >= minDistance) break;
            }
            LastRaised = bestPivot != target;
            pivot = bestPivot;
            return best;
        }

        float Allowed(Vector3 from, Vector3 dir, float distance)
        {
            if (Physics.SphereCast(from, clearance, dir, out var hit, distance, mask, QueryTriggerInteraction.Ignore) && !Own(hit.collider))
                return Mathf.Max(0, hit.distance - margin);
            return distance;
        }

        Vector3 Sweep(Vector3 pivot, Vector3 position)
        {
            Vector3 d = position - pivot; float m = d.magnitude;
            if (m < .01f) return position;
            float a = Allowed(pivot, d / m, m);
            if (a < m) LastObstructed = true;
            return pivot + d / m * a;
        }

        public bool Blocked(Vector3 position, float radius)
        {
            int n = Physics.OverlapSphereNonAlloc(position, radius, overlap, mask, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < n; i++) if (!Own(overlap[i])) return true;
            return false;
        }

        bool Own(Collider c)
        {
            var p = hud.game.prototype;
            return c.transform.IsChildOf(p.astronaut.transform) || c.transform.IsChildOf(p.rover.transform);
        }
    }
}
