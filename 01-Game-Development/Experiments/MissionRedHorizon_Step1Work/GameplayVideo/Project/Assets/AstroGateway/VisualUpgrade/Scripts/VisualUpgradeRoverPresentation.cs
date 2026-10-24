using UnityEngine;
using AstroGateway.Stage08B;

namespace AstroGateway.VisualUpgrade
{
    // TEMPORARY rover presentation: headlights and cockpit screens come on when someone is aboard, brake lights
    // brighten while slowing, the mast beacon blinks, and the body rolls/pitches slightly with acceleration and
    // steering. Visual only; reads MarsPrototype state, never writes it.
    [DefaultExecutionOrder(660)]
    public class VisualUpgradeRoverPresentation : MonoBehaviour
    {
        public MarsPrototype prototype;
        public Light[] headlights;
        public Renderer[] screens, brakeLights, beacons;
        public Transform body;
        public GameObject driver;
        public float headlightIntensity = 9f;
        static readonly int Emission = Shader.PropertyToID("_EmissionColor");
        MaterialPropertyBlock block;
        float lightLevel, lastSpeed, pitch, roll, lastYaw;
        Quaternion bodyBase;

        void Start()
        {
            block = new MaterialPropertyBlock();
            if (body) bodyBase = body.localRotation;
            lastYaw = prototype.rover.transform.eulerAngles.y;
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
            Set(screens, new Color(.15f, .9f, 1.1f) * (.2f + 1.6f * lightLevel));

            float speed = prototype.speed;
            bool braking = aboard && Mathf.Abs(speed) < Mathf.Abs(lastSpeed) - .0005f;
            Set(brakeLights, new Color(1.6f, .05f, .02f) * (aboard ? (braking ? 3.2f : 1f) : .25f));
            bool blink = Mathf.Repeat(Time.time, 1.4f) < .18f;
            Set(beacons, new Color(2.2f, 1.1f, .2f) * (blink ? 2.5f : .15f));

            if (body && dt > 0)
            {
                float accel = (speed - lastSpeed) / dt;
                float yaw = prototype.rover.transform.eulerAngles.y;
                float yawRate = Mathf.DeltaAngle(lastYaw, yaw) / dt;
                lastYaw = yaw;
                pitch = Mathf.Lerp(pitch, Mathf.Clamp(-accel * .9f, -2.5f, 2.5f), 1 - Mathf.Exp(-6 * dt));
                roll = Mathf.Lerp(roll, Mathf.Clamp(-yawRate * Mathf.Abs(speed) * .02f, -3f, 3f), 1 - Mathf.Exp(-5 * dt));
                float rumble = Mathf.Abs(speed) > .2f ? (Mathf.PerlinNoise(Time.time * 7, 0) - .5f) * .5f : 0;
                body.localRotation = bodyBase * Quaternion.Euler(pitch + rumble, 0, roll);
            }
            lastSpeed = speed;
        }
    }
}
