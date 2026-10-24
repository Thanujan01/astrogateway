using UnityEngine;
using AstroGateway.Stage08B;

namespace AstroGateway.VisualUpgrade
{
    // TEMPORARY rover presentation: headlights and cockpit screens come on when someone is aboard, brake lights
    // brighten while slowing, the mast beacon blinks, and the body rolls/pitches slightly with acceleration and
    // steering. Visual only; reads MarsPrototype state, never writes it.
    // Batch 3: the body sway pivot is now wired by the builder (it was never assigned before), each wheel follows
    // the ground under it within a short visual suspension travel (wheel roll stays with the existing scripts),
    // the whip antenna sways with acceleration and the high-gain antenna slowly tracks.
    [DefaultExecutionOrder(660)]
    public class VisualUpgradeRoverPresentation : MonoBehaviour
    {
        public MarsPrototype prototype;
        public Light[] headlights;
        public Renderer[] screens, brakeLights, beacons;
        public Transform body;
        public GameObject driver;
        public Transform[] wheels;
        public Transform whipAntenna, highGainAntenna;
        public float headlightIntensity = 9f, suspensionTravel = .09f;
        static readonly int Emission = Shader.PropertyToID("_EmissionColor");
        MaterialPropertyBlock block;
        float lightLevel, lastSpeed, pitch, roll, lastYaw, whip, whipVel;
        Quaternion bodyBase, whipBase, hgaBase;
        Vector3[] wheelBase;
        float[] wheelOffset;

        void Start()
        {
            block = new MaterialPropertyBlock();
            if (body) bodyBase = body.localRotation;
            if (whipAntenna) whipBase = whipAntenna.localRotation;
            if (highGainAntenna) hgaBase = highGainAntenna.localRotation;
            lastYaw = prototype.rover.transform.eulerAngles.y;
            if (wheels != null)
            {
                wheelBase = new Vector3[wheels.Length]; wheelOffset = new float[wheels.Length];
                for (int i = 0; i < wheels.Length; i++) if (wheels[i]) wheelBase[i] = wheels[i].localPosition;
            }
        }

        void Set(Renderer[] list, Color c)
        {
            if (list == null) return;
            foreach (var r in list) { if (!r) continue; r.GetPropertyBlock(block); block.SetColor(Emission, c); r.SetPropertyBlock(block); }
        }

        void LateUpdate()
        {
            float dt = Time.deltaTime;
            bool aboard = prototype.driving;
            if (driver && driver.activeSelf != aboard) driver.SetActive(aboard);
            lightLevel = Mathf.MoveTowards(lightLevel, aboard ? 1 : 0, Time.unscaledDeltaTime * 2.5f);
            foreach (var l in headlights) if (l) { l.intensity = headlightIntensity * lightLevel; l.enabled = lightLevel > .01f; }
            float flicker = aboard ? 1 + (Mathf.PerlinNoise(Time.time * 3, 1) - .5f) * .15f : 1;
            Set(screens, new Color(.15f, .9f, 1.1f) * (.2f + 1.6f * lightLevel) * flicker);

            float speed = prototype.speed;
            bool braking = aboard && Mathf.Abs(speed) < Mathf.Abs(lastSpeed) - .0005f;
            Set(brakeLights, new Color(1.6f, .05f, .02f) * (aboard ? (braking ? 3.2f : 1f) : .25f));
            bool blink = Mathf.Repeat(Time.time, 1.4f) < .18f;
            Set(beacons, new Color(2.2f, 1.1f, .2f) * (blink ? 2.5f : .15f));

            if (dt > 0)
            {
                float accel = (speed - lastSpeed) / dt;
                float yaw = prototype.rover.transform.eulerAngles.y;
                float yawRate = Mathf.DeltaAngle(lastYaw, yaw) / dt;
                lastYaw = yaw;
                pitch = Mathf.Lerp(pitch, Mathf.Clamp(-accel * .9f, -2.5f, 2.5f), 1 - Mathf.Exp(-6 * dt));
                roll = Mathf.Lerp(roll, Mathf.Clamp(-yawRate * Mathf.Abs(speed) * .02f, -3f, 3f), 1 - Mathf.Exp(-5 * dt));
                float rumble = Mathf.Abs(speed) > .2f ? (Mathf.PerlinNoise(Time.time * 7, 0) - .5f) * .5f : 0;
                if (body) body.localRotation = bodyBase * Quaternion.Euler(pitch + rumble, 0, roll);

                // Whip antenna: damped spring driven by acceleration.
                whipVel += (-whip * 40 - whipVel * 4 - accel * 6) * dt;
                whip += whipVel * dt;
                whip = Mathf.Clamp(whip, -12, 12);
                if (whipAntenna) whipAntenna.localRotation = whipBase * Quaternion.Euler(whip, 0, roll * 1.5f);
                if (highGainAntenna) highGainAntenna.localRotation = Quaternion.Euler(0, Mathf.Sin(Time.time * .25f) * 25, 0) * hgaBase;

                Suspension(dt);
            }
            lastSpeed = speed;
        }

        // Each wheel drops/rises to the terrain under it (layer Default, ignoring triggers) within ±travel.
        void Suspension(float dt)
        {
            if (wheels == null || wheelBase == null) return;
            var rover = prototype.rover.transform;
            for (int i = 0; i < wheels.Length; i++)
            {
                var w = wheels[i]; if (!w) continue;
                var restWorld = w.parent.TransformPoint(wheelBase[i]);
                float target = 0;
                if (Physics.Raycast(restWorld + rover.up * .6f, -rover.up, out var hit, 1.6f, 1, QueryTriggerInteraction.Ignore) && !hit.collider.transform.IsChildOf(rover))
                {
                    float wheelRadius = .36f;
                    float bottom = Vector3.Dot(restWorld - hit.point, rover.up) - wheelRadius; // >0: wheel above ground
                    target = Mathf.Clamp(-bottom, -suspensionTravel, suspensionTravel);
                }
                wheelOffset[i] = Mathf.Lerp(wheelOffset[i], target, 1 - Mathf.Exp(-12 * dt));
                w.localPosition = wheelBase[i] + w.parent.InverseTransformDirection(rover.up) * wheelOffset[i];
            }
        }
    }
}
