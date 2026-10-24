using UnityEngine;
using AstroGateway.Stage08B;

namespace AstroGateway.VisualUpgrade
{
    // TEMPORARY procedural walk/run presentation for the primitive astronaut (no rigged asset is available yet).
    // Swings the existing limb pivots from the measured movement speed, blending idle -> walk -> run, with a small
    // body bob and forward lean. Purely visual: reads MarsPrototype.speed/driving, never writes gameplay state.
    [DefaultExecutionOrder(650)]
    public class VisualUpgradeAstronautMotion : MonoBehaviour
    {
        public MarsPrototype prototype;
        public Transform suitRig, leftArm, rightArm, leftLeg, rightLeg;
        public float walkSpeed = 2.2f, runSpeed = 3.8f;
        float phase, amplitude, runBlend, idleTime;
        bool seated;
        Renderer[] suitRenderers;
        Vector3 rigBase;
        Quaternion la, ra, ll, rl, rigRot;

        void Start()
        {
            rigBase = suitRig.localPosition; rigRot = suitRig.localRotation;
            suitRenderers = System.Array.FindAll(suitRig.GetComponentsInChildren<Renderer>(true), r => r.enabled);
            la = leftArm.localRotation; ra = rightArm.localRotation; ll = leftLeg.localRotation; rl = rightLeg.localRotation;
        }

        void LateUpdate()
        {
            if (prototype.driving)
            {
                // The game parents the astronaut to the rover seat anchor; the procedural suit is taller than the cab,
                // so it is hidden while driving (cockpit screens and headlights show the rover is crewed).
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
            float stride = Mathf.Lerp(1.25f, 1.9f, runBlend);           // metres per full cycle
            phase += speed / stride * dt * Mathf.PI * 2;
            idleTime += dt;

            float legSwing = Mathf.Lerp(26, 38, runBlend) * amplitude;
            float armSwing = Mathf.Lerp(20, 34, runBlend) * amplitude;
            float s = Mathf.Sin(phase);
            float knee = Mathf.Max(0, Mathf.Sin(phase + 1.2f)); // used for slight extra lift
            leftLeg.localRotation = ll * Quaternion.Euler(s * legSwing, 0, 0);
            rightLeg.localRotation = rl * Quaternion.Euler(-s * legSwing, 0, 0);
            leftArm.localRotation = la * Quaternion.Euler(-s * armSwing, 0, -4 * amplitude);
            rightArm.localRotation = ra * Quaternion.Euler(s * armSwing, 0, 4 * amplitude);

            float bob = Mathf.Abs(Mathf.Cos(phase)) * Mathf.Lerp(.035f, .06f, runBlend) * amplitude;
            float breathe = Mathf.Sin(idleTime * 1.6f) * .006f * (1 - amplitude);
            suitRig.localPosition = rigBase + Vector3.up * (bob + breathe + knee * .005f);
            suitRig.localRotation = rigRot * Quaternion.Euler(Mathf.Lerp(3, 9, runBlend) * amplitude, 0, s * 1.5f * amplitude);
        }
    }
}
