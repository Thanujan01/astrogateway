using UnityEngine;
using AstroGateway.CinematicHUD;
using AstroGateway.Stage08B;

namespace AstroGateway.VisualUpgrade
{
    // TEMPORARY procedural walk/run presentation for the primitive astronaut (no rigged asset is available yet).
    // Swings the existing limb pivots from the measured movement speed, blending idle -> walk -> run, with a small
    // body bob and forward lean. Purely visual: reads MarsPrototype.speed/driving, never writes gameplay state.
    // Batch 3: knee and elbow joints (child pivots added by the builder), a Mars low-gravity lope at run speed
    // (longer float, softer landing), counter-rotating shoulders, a scanning pose (right arm raised toward the
    // station, body turned to it, helmet tilted) and small dust puffs at each foot plant.
    [DefaultExecutionOrder(650)]
    public class VisualUpgradeAstronautMotion : MonoBehaviour
    {
        public MarsPrototype prototype;
        public AstroGatewayHUDController hud;
        public Transform suitRig, leftArm, rightArm, leftLeg, rightLeg;
        public Transform leftKnee, rightKnee, leftElbow, rightElbow;
        public ParticleSystem footDust;
        public float walkSpeed = 2.2f, runSpeed = 3.8f;
        float phase, amplitude, runBlend, idleTime, scanBlend, lastStep;
        bool seated;
        Renderer[] suitRenderers;
        Vector3 rigBase;
        Quaternion la, ra, ll, rl, rigRot, lk, rk, le, re;

        void Start()
        {
            rigBase = suitRig.localPosition; rigRot = suitRig.localRotation;
            suitRenderers = System.Array.FindAll(suitRig.GetComponentsInChildren<Renderer>(true), r => r.enabled);
            la = leftArm.localRotation; ra = rightArm.localRotation; ll = leftLeg.localRotation; rl = rightLeg.localRotation;
            if (leftKnee) lk = leftKnee.localRotation; if (rightKnee) rk = rightKnee.localRotation;
            if (leftElbow) le = leftElbow.localRotation; if (rightElbow) re = rightElbow.localRotation;
        }

        static void Joint(Transform t, Quaternion rest, float degrees) { if (t) t.localRotation = rest * Quaternion.Euler(degrees, 0, 0); }

        void LateUpdate()
        {
            if (prototype.driving)
            {
                // The game parents the astronaut to the rover seat anchor; the procedural suit is taller than the cab,
                // so it is hidden while driving (a seated driver is shown in the cab instead).
                if (!seated) { seated = true; foreach (var r in suitRenderers) if (r) r.enabled = false; }
                return;
            }
            if (seated) { seated = false; foreach (var r in suitRenderers) if (r) r.enabled = true; amplitude = 0; }
            float dt = Time.deltaTime;
            if (dt <= 0) return; // paused
            float speed = Mathf.Abs(prototype.speed);
            float moving = Mathf.Clamp01(speed / walkSpeed);
            amplitude = Mathf.MoveTowards(amplitude, moving, dt * 4);
            runBlend = Mathf.MoveTowards(runBlend, Mathf.Clamp01((speed - walkSpeed) / (runSpeed - walkSpeed)), dt * 3);
            bool scanning = hud && hud.scanning;
            scanBlend = Mathf.MoveTowards(scanBlend, scanning ? 1 : 0, dt * 3.5f);
            float stride = Mathf.Lerp(1.25f, 2.3f, runBlend);           // metres per full cycle (Mars lope is longer)
            phase += speed / stride * dt * Mathf.PI * 2;
            idleTime += dt;

            float legSwing = Mathf.Lerp(24, 34, runBlend) * amplitude;
            float armSwing = Mathf.Lerp(18, 30, runBlend) * amplitude;
            float s = Mathf.Sin(phase);
            // Knee flexes while its leg swings forward (recovery) and slightly at contact; more when running.
            // Leg angle is -s*swing (left), so the left leg swings forward while cos(phase) > 0; peak flex mid-swing.
            float flex = Mathf.Lerp(38, 70, runBlend) * amplitude;
            float kneeL = Mathf.Max(0, Mathf.Cos(phase + .2f)) * flex + 4 * amplitude;
            float kneeR = Mathf.Max(0, -Mathf.Cos(phase + .2f)) * flex + 4 * amplitude;
            leftLeg.localRotation = ll * Quaternion.Euler(-s * legSwing, 0, 0);
            rightLeg.localRotation = rl * Quaternion.Euler(s * legSwing, 0, 0);
            Joint(leftKnee, lk, kneeL);
            Joint(rightKnee, rk, kneeR);

            // Arms counter-swing; elbows stay bent a little (pressurised suit), more when running.
            float elbow = -(12 + Mathf.Lerp(10, 45, runBlend) * amplitude);
            leftArm.localRotation = la * Quaternion.Euler(s * armSwing, 0, -4 * amplitude - 3);
            var rightWalk = ra * Quaternion.Euler(-s * armSwing, 0, 4 * amplitude + 3);
            // Scan pose: right arm forward and up holding the scanner toward the station.
            var rightScan = ra * Quaternion.Euler(-72, 0, 6);
            rightArm.localRotation = Quaternion.Slerp(rightWalk, rightScan, Smooth(scanBlend));
            Joint(leftElbow, le, elbow);
            Joint(rightElbow, re, Mathf.Lerp(elbow, -28, Smooth(scanBlend)));

            // Low-gravity bounce: higher, longer float at run speed; gentle breathing at rest.
            float bounce = Mathf.Abs(Mathf.Cos(phase));
            float bob = Mathf.Pow(bounce, Mathf.Lerp(1, .6f, runBlend)) * Mathf.Lerp(.03f, .1f, runBlend) * amplitude;
            float breathe = Mathf.Sin(idleTime * 1.6f) * .006f * (1 - amplitude);
            float idleSway = Mathf.Sin(idleTime * .7f) * 1.2f * (1 - amplitude) * (1 - scanBlend);
            suitRig.localPosition = rigBase + Vector3.up * (bob + breathe);
            suitRig.localRotation = rigRot * Quaternion.Euler(Mathf.Lerp(3, 10, runBlend) * amplitude + 6 * Smooth(scanBlend), idleSway, s * 1.5f * amplitude);

            // Foot plant dust: one puff per foot contact (twice per cycle) while moving.
            if (footDust && amplitude > .4f)
            {
                float step = Mathf.Floor(phase / Mathf.PI);
                if (step != lastStep)
                {
                    lastStep = step;
                    var foot = ((int)step & 1) == 0 ? leftLeg : rightLeg;
                    var p = foot.position; p.y = prototype.astronaut.transform.position.y + .05f;
                    var e = new ParticleSystem.EmitParams { position = p, applyShapeToPosition = true };
                    footDust.Emit(e, runBlend > .5f ? 6 : 3);
                }
            }
        }

        static float Smooth(float x) => x * x * (3 - 2 * x);
    }
}
