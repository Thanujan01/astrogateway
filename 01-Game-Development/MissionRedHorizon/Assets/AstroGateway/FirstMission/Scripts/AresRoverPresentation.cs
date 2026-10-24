using UnityEngine;
using AstroGateway.Stage08B;

// ARES-VI visuals on the TERRAVEX rover (First Mission scene only). Visual only: reads MarsPrototype state and
// never writes it, so the kinematic CharacterController physics, colliders, boarding and exit rules are unchanged.
//  - six wheels roll with the controller's signed speed (no slip model);
//  - rocker and bogie tilt to follow the ground under each wheel (visual suspension, clamped, smoothed);
//  - the robotic arm deploys (showroom ARES_ArmDeploy clip) during arm-sampler observations;
//  - the mast pans (ARES_ChemCamScan clip) during mast-camera observations;
//  - the seated astronaut is hidden while driving (the closed canopy has no seat model yet).
// The hover modules stay inert (plumes removed in the prefab variant). No anti-gravity behaviour exists.
namespace AstroGateway.FirstMission
{
    [DefaultExecutionOrder(670)]
    public class AresRoverPresentation : MonoBehaviour
    {
        public MarsPrototype prototype;
        public Transform ares;
        public Transform[] wheels;            // FL, ML, RL, FR, MR, RR
        public Transform rockerL, rockerR, bogieL, bogieR;
        public AnimationClip armDeploy, mastScan;
        public float wheelRadius = .34f, maxTilt = 14f, tiltSharpness = 10f;
        public LayerMask groundMask = 1;
        [Range(0, 1)] public float armTarget;
        public bool mastScanning;
        public float ArmProgress => armT;

        Quaternion[] wheelBase;
        Quaternion rockerLBase, rockerRBase, bogieLBase, bogieRBase;
        float roll, armT = -1, mastT, rockL, rockR, bogL, bogR;
        Renderer[] astronautRenderers;
        bool astronautHidden;

        void Start()
        {
            wheelBase = new Quaternion[wheels.Length];
            for (int i = 0; i < wheels.Length; i++) wheelBase[i] = wheels[i].localRotation;
            rockerLBase = rockerL.localRotation; rockerRBase = rockerR.localRotation;
            bogieLBase = bogieL.localRotation; bogieRBase = bogieR.localRotation;
            astronautRenderers = prototype.astronaut.GetComponentsInChildren<Renderer>(true);
        }

        void LateUpdate()
        {
            float dt = Time.deltaTime;
            bool driving = prototype.driving;
            if (driving != astronautHidden)
            {
                astronautHidden = driving;
                foreach (var r in astronautRenderers) if (r) r.enabled = !driving;
            }

            // Animation clips first (they only write arm / mast curves), procedural wheels and suspension after.
            float targetT = Mathf.Clamp01(armTarget);
            if (armDeploy && !Mathf.Approximately(armT, targetT))
            {
                armT = armT < 0 ? targetT : Mathf.MoveTowards(armT, targetT, dt / armDeploy.length);
                armDeploy.SampleAnimation(ares.gameObject, armT * armDeploy.length);
            }
            if (mastScan && (mastScanning || mastT > 0))
            {
                mastT += dt;
                if (!mastScanning && mastT >= mastScan.length) mastT = 0; // finish the current sweep, then rest
                else if (mastT >= mastScan.length) mastT -= mastScan.length;
                mastScan.SampleAnimation(ares.gameObject, mastT);
            }

            float speed = driving ? prototype.speed : 0;
            roll = Mathf.Repeat(roll + speed * dt / Mathf.Max(.05f, wheelRadius * ares.lossyScale.y) * Mathf.Rad2Deg, 360);
            for (int i = 0; i < wheels.Length; i++) wheels[i].localRotation = wheelBase[i] * Quaternion.Euler(roll, 0, 0);

            float k = 1 - Mathf.Exp(-tiltSharpness * dt);
            Side(wheels[0], wheels[1], wheels[2], ref rockL, ref bogL, k);
            Side(wheels[3], wheels[4], wheels[5], ref rockR, ref bogR, k);
            rockerL.localRotation = rockerLBase * Quaternion.Euler(rockL, 0, 0);
            rockerR.localRotation = rockerRBase * Quaternion.Euler(rockR, 0, 0);
            bogieL.localRotation = bogieLBase * Quaternion.Euler(bogL - rockL, 0, 0);
            bogieR.localRotation = bogieRBase * Quaternion.Euler(bogR - rockR, 0, 0);
        }

        // Positive X rotation lowers the front, so a higher front wheel gives a negative angle.
        void Side(Transform front, Transform mid, Transform rear, ref float rocker, ref float bogie, float k)
        {
            if (!Ground(front, out float hf) || !Ground(mid, out float hm) || !Ground(rear, out float hr)) return;
            var up = ares.up; var fwd = ares.forward;
            float zf = Vector3.Dot(front.position - ares.position, fwd), zm = Vector3.Dot(mid.position - ares.position, fwd), zr = Vector3.Dot(rear.position - ares.position, fwd);
            float bogieTarget = -Mathf.Atan2(hm - hr, Mathf.Max(.1f, zm - zr)) * Mathf.Rad2Deg;
            float rockerTarget = -Mathf.Atan2(hf - (hm + hr) * .5f, Mathf.Max(.1f, zf - (zm + zr) * .5f)) * Mathf.Rad2Deg;
            rocker = Mathf.Lerp(rocker, Mathf.Clamp(rockerTarget, -maxTilt, maxTilt), k);
            bogie = Mathf.Lerp(bogie, Mathf.Clamp(bogieTarget, -maxTilt, maxTilt), k);
        }

        bool Ground(Transform wheel, out float height)
        {
            height = 0;
            var origin = new Vector3(wheel.position.x, ares.position.y + 2.5f, wheel.position.z);
            if (!Physics.Raycast(origin, Vector3.down, out var hit, 6, groundMask, QueryTriggerInteraction.Ignore)) return false;
            height = Vector3.Dot(hit.point - ares.position, ares.up);
            return true;
        }
    }
}
