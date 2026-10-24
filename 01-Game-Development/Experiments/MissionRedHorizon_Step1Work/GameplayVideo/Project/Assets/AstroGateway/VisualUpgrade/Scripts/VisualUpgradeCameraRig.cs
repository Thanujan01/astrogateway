using UnityEngine;
using AstroGateway.CinematicHUD;
using AstroGateway.ProductionVisual;

namespace AstroGateway.VisualUpgrade
{
    // Camera for Mars_VisualUpgrade_v1 only. Keeps the ProductionCameraRig framing (same offsets, FOVs, chase
    // distances, scanner and cinematic shots). The original rig stays in the scene, disabled, and still owns
    // nearChase / captureOverride so the CHASE DISTANCE button and existing test harnesses keep working.
    // Fixes the laptop-playtest FAIL: the old rig clamped obstruction pull-in to 0.35 m, which put the camera
    // inside the Crew Dome airlock. This rig never ends inside geometry: when the wall is too close it raises the
    // pivot over the shoulder, and as a last resort it sits at a verified-free pivot. Pull-in is immediate,
    // release is smoothed, and cinematic enter/exit eases instead of jumping.
    [DefaultExecutionOrder(520)]
    public class VisualUpgradeCameraRig : MonoBehaviour
    {
        public AstroGatewayHUDController hud;
        public ProductionModeCoordinator modes;
        public ProductionCameraRig legacy;
        public float clearance = .22f, margin = .12f, minDistance = 1.2f, nearClip = .08f;
        public float followSharpness = 7f, cinematicSharpness = 2.2f, transitionSeconds = 1.6f;
        public LayerMask mask = 1;
        public bool LastObstructed, LastRaised;
        public int InsideFrames;

        Vector3 smoothed;
        Quaternion rotation;
        bool initialized;
        ProductionMode lastMode;
        float modeChangedAt = -10;
        readonly Collider[] overlap = new Collider[16];
        static readonly float[] Raise = { .6f, 1.2f, 1.8f, 2.4f, 3.0f }; // up to 3 m: clears the 3.2 m airlocks

        void LateUpdate()
        {
            var p = hud.game.prototype;
            var cam = p.follow;
            if (!cam) return;
            if (cam.nearClipPlane > nearClip) cam.nearClipPlane = nearClip;

            var mode = modes.Current;
            if (mode != lastMode && (mode == ProductionMode.Cinematic || lastMode == ProductionMode.Cinematic)) modeChangedAt = Time.unscaledTime;
            lastMode = mode;

            Vector3 target = p.ActivePosition() + Vector3.up * (p.driving ? 1.45f : 1.4f), desired;
            float fov = p.driving ? 61 : 55;
            bool free = false; // free shots (capture, cinematic) are not collision-resolved against the player
            if (legacy.captureOverride) { desired = legacy.capturePosition; target = legacy.captureTarget; fov = 58; free = true; }
            else if (mode == ProductionMode.Cinematic) { desired = hud.environment.scenic.transform.position; target = new Vector3(-36, 4, -43); fov = 58; free = true; }
            else if (mode == ProductionMode.Scanner && hud.CurrentTarget())
            {
                var t = hud.CurrentTarget().transform.position + Vector3.up * .5f;
                target = Vector3.Lerp(target, t, .6f);
                desired = p.ActivePosition() + Quaternion.Euler(12, p.yaw - 25, 0) * new Vector3(0, 2, -3.4f);
                fov = 49;
            }
            else desired = target + Quaternion.Euler(p.pitch, p.yaw, 0) * new Vector3(.35f, .15f, -(p.driving ? (legacy.nearChase ? 5.5f : 7.5f) : 4.8f));

            LastObstructed = false; LastRaised = false;
            Vector3 pivot = target;
            if (!free) desired = Resolve(target, desired, out pivot);

            if (!initialized) { smoothed = cam.transform.position; rotation = cam.transform.rotation; initialized = true; }
            float sharp = Time.unscaledTime - modeChangedAt < transitionSeconds ? cinematicSharpness : followSharpness;
            float blend = 1 - Mathf.Exp(-sharp * Time.unscaledDeltaTime);
            smoothed = Vector3.Lerp(smoothed, desired, blend);
            if (!free) smoothed = Sweep(pivot, smoothed);
            if (!free && Blocked(smoothed, clearance * .5f)) { smoothed = pivot; InsideFrames++; }

            var aim = Quaternion.LookRotation(target - smoothed, Vector3.up);
            rotation = Quaternion.Slerp(rotation, aim, blend);
            cam.transform.SetPositionAndRotation(smoothed, rotation);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fov, blend);
            hud.environment.scenic.enabled = false;
            cam.enabled = true;
            legacy.LastTarget = target;
            legacy.LastObstructed = LastObstructed;
        }

        // Collision-safe goal. Keeps the full distance when clear; pulls in when blocked; raises the pivot when
        // the remaining distance would be shorter than minDistance (the airlock case).
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
                // Is there head room above the player?
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
            Vector3 d = position - pivot;
            float m = d.magnitude;
            if (m < .01f) return position;
            float a = Allowed(pivot, d / m, m);
            if (a < m) LastObstructed = true;
            return pivot + d / m * a;
        }

        bool Blocked(Vector3 position, float radius)
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
