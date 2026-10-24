using System.Collections.Generic;
using UnityEngine;

namespace AstroGateway.FirstMission.Missions
{
    // Generator: FAULT (blinking amber/red beacons + sparks at the service console) until repaired, then ONLINE
    // (steady green beacons, soft hum light). Reduced motion stops the blinking.
    public class GeneratorState : MonoBehaviour
    {
        public Transform console;           // fault panel the astronaut repairs
        public bool Repaired { get; private set; }
        public float RepairProgress { get; set; }
        public System.Func<bool> reduced;
        readonly List<Renderer> beacons = new List<Renderer>();
        Light glow; ParticleSystem sparks; Material beaconMat;

        public void Init(Material particleTemplate)
        {
            beaconMat = StructureFx.Glow(new Color(1f, .35f, .1f, 1f));
            var b = Bounds();
            foreach (var x in new[] { -.42f, -.14f, .14f, .42f })
                beacons.Add(StructureFx.Lamp(transform, transform.InverseTransformPoint(new Vector3(b.center.x + b.size.x * x, b.max.y + .25f, b.center.z)), .45f, beaconMat, "Status beacon").GetComponent<Renderer>());
            var lg = new GameObject("Generator light"); lg.transform.SetParent(transform, false); lg.transform.position = new Vector3(b.center.x, b.max.y + 2f, b.center.z);
            glow = lg.AddComponent<Light>(); glow.type = LightType.Point; glow.range = 16; glow.intensity = 1.6f; glow.shadows = LightShadows.None; glow.color = new Color(1f, .4f, .15f);
            if (console)
            {
                var sp = new GameObject("Fault sparks"); sp.transform.SetParent(console, false); sp.transform.localPosition = new Vector3(0, .45f, 0);
                sparks = sp.AddComponent<ParticleSystem>(); sparks.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                var main = sparks.main; main.maxParticles = 40; main.startLifetime = new ParticleSystem.MinMaxCurve(.25f, .6f); main.startSpeed = new ParticleSystem.MinMaxCurve(1.5f, 4f);
                main.startSize = new ParticleSystem.MinMaxCurve(.04f, .09f); main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, .85f, .4f), new Color(1f, .45f, .1f)); main.gravityModifier = 1.2f; main.simulationSpace = ParticleSystemSimulationSpace.World;
                var em = sparks.emission; em.rateOverTime = 0; em.SetBursts(new[] { new ParticleSystem.Burst(0, 6, 12, 0, .9f) { probability = .7f } });
                var sh = sparks.shape; sh.shapeType = ParticleSystemShapeType.Cone; sh.angle = 35; sh.radius = .05f;
                var r = sp.GetComponent<ParticleSystemRenderer>(); r.sharedMaterial = particleTemplate ? new Material(particleTemplate) : StructureFx.Glow(Color.white);
                if (r.sharedMaterial.HasProperty("_BaseColor")) r.sharedMaterial.SetColor("_BaseColor", new Color(1f, .8f, .4f, 1f));
                sparks.Play();
            }
        }

        Bounds Bounds()
        {
            var rs = GetComponentsInChildren<Renderer>(); if (rs.Length == 0) return new Bounds(transform.position, Vector3.one);
            var b = rs[0].bounds; foreach (var r in rs) b.Encapsulate(r.bounds); return b;
        }

        public void SetRepaired(bool on)
        {
            Repaired = on;
            if (sparks) { if (on) sparks.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); else sparks.Play(); }
            if (glow) glow.color = on ? new Color(.35f, 1f, .6f) : new Color(1f, .4f, .15f);
        }

        void Update()
        {
            bool still = reduced != null && reduced();
            float blink = Repaired || still ? 1 : (Mathf.Repeat(Time.time * 1.6f, 1) < .5f ? 1 : .15f);
            var c = Repaired ? new Color(.35f, 1f, .55f) : Color.Lerp(new Color(1f, .2f, .1f), new Color(1f, .6f, .1f), RepairProgress);
            if (beaconMat) { var cc = c * blink; cc.a = 1; if (beaconMat.HasProperty("_BaseColor")) beaconMat.SetColor("_BaseColor", cc); else beaconMat.color = cc; }
            if (glow) glow.intensity = Repaired ? 1.4f : 1.6f * blink;
        }
    }
}
