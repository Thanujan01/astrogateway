using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Mission visual effects (Missions scene only, light for Android): expanding ground rings and spark bursts when a
// step, gate or mission completes, a power wave when the generator comes back online, and an energy stream from the
// charging pylon into the rover while it charges. One small particle system + a pool of line rings; reduced motion
// keeps only the static parts (no rings, smaller bursts).
namespace AstroGateway.FirstMission.Missions
{
    public class MissionFx : MonoBehaviour
    {
        MissionDirector d;
        ParticleSystem sparks, stream;
        Material lineMat;
        readonly List<(LineRenderer lr, float t, float dur, float radius, Color c)> rings = new List<(LineRenderer, float, float, float, Color)>();
        readonly Stack<LineRenderer> pool = new Stack<LineRenderer>();
        int lastGates; bool lastRepaired; float streamAcc;
        // storm build-up: thicker, browner haze and a dimmer sun as the second storm approaches (runtime only)
        float baseDensity, baseEnd, baseSun = -1, storm; Color baseFog; Light sun;
        public float StormFactor => storm;
        public int RingsShown { get; private set; }
        public int BurstsShown { get; private set; }
        public int StreamParticles => stream ? stream.particleCount : 0;
        bool Reduced => d.mission.Settings.reducedMotion;
        Stage08B.MarsPrototype P => d.mission.hud.game.prototype;

        public void Init(MissionDirector director)
        {
            d = director;
            var template = d.particleTemplate;
            lineMat = StructureFx.Glow(Color.white, template);
            sparks = MakePs("Mission burst FX", 160, template);
            var m = sparks.main; m.startLifetime = new ParticleSystem.MinMaxCurve(.6f, 1.3f); m.startSpeed = new ParticleSystem.MinMaxCurve(3f, 7f); m.startSize = new ParticleSystem.MinMaxCurve(.06f, .16f); m.gravityModifier = .5f;
            var sh = sparks.shape; sh.shapeType = ParticleSystemShapeType.Hemisphere; sh.radius = .6f;
            stream = MakePs("Charge stream FX", 120, template);
            var sm = stream.main; sm.startLifetime = .55f; sm.startSpeed = 0; sm.startSize = new ParticleSystem.MinMaxCurve(.08f, .16f); sm.gravityModifier = 0;
            var ssh = stream.shape; ssh.enabled = false;
            d.StepCompleted += (mi, s) => { if (P) Ring(P.ActivePosition(), new Color(.33f, 1f, .55f), 4f, .9f); };
            d.MissionCompleted += mi => { if (!P) return; Burst(P.ActivePosition() + Vector3.up, new Color(.4f, 1f, .6f), 70); Ring(P.ActivePosition(), new Color(.4f, 1f, .6f), 9f, 1.4f); };
            d.AllMissionsCompleted += () => { if (!P) return; Burst(P.ActivePosition() + Vector3.up, new Color(1f, .75f, .3f), 120); Ring(P.ActivePosition(), new Color(1f, .75f, .3f), 16f, 2f); };
            lastGates = d.CheckpointsPassed; lastRepaired = d.Repaired;
            baseDensity = RenderSettings.fogDensity; baseEnd = RenderSettings.fogEndDistance; baseFog = RenderSettings.fogColor;
            sun = RenderSettings.sun; if (!sun) sun = FindObjectsByType<Light>(FindObjectsSortMode.None).FirstOrDefault(l => l.type == LightType.Directional);
            if (sun) baseSun = sun.intensity;
        }

        ParticleSystem MakePs(string n, int max, Material template)
        {
            var go = new GameObject(n); go.transform.SetParent(transform, false);
            var ps = go.AddComponent<ParticleSystem>(); ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            var main = ps.main; main.maxParticles = max; main.simulationSpace = ParticleSystemSimulationSpace.World; main.playOnAwake = false;
            var em = ps.emission; em.rateOverTime = 0;
            var col = ps.colorOverLifetime; col.enabled = true;
            var g = new Gradient(); g.SetKeys(new[] { new GradientColorKey(Color.white, 0), new GradientColorKey(Color.white, 1) }, new[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(.8f, .6f), new GradientAlphaKey(0, 1) });
            col.color = g;
            var r = go.GetComponent<ParticleSystemRenderer>(); r.sharedMaterial = template ? new Material(template) : StructureFx.Glow(Color.white);
            if (r.sharedMaterial.HasProperty("_BaseColor")) r.sharedMaterial.SetColor("_BaseColor", Color.white);
            ps.Play();
            return ps;
        }

        public void Burst(Vector3 at, Color c, int count)
        {
            if (Reduced) count /= 3;
            var ep = new ParticleSystem.EmitParams { position = at, startColor = c, applyShapeToPosition = true };
            sparks.Emit(ep, count); BurstsShown++;
        }

        public void Ring(Vector3 at, Color c, float radius, float seconds)
        {
            if (Reduced) return;
            var lr = pool.Count > 0 ? pool.Pop() : NewRing();
            lr.gameObject.SetActive(true); lr.transform.position = at + Vector3.up * .15f;
            rings.Add((lr, 0, seconds, radius, c)); RingsShown++;
        }

        LineRenderer NewRing()
        {
            var go = new GameObject("FX ring"); go.transform.SetParent(transform, false); go.transform.rotation = Quaternion.Euler(90, 0, 0);
            var lr = go.AddComponent<LineRenderer>(); lr.useWorldSpace = false; lr.loop = true; lr.positionCount = 64; lr.alignment = LineAlignment.TransformZ;
            lr.sharedMaterial = new Material(lineMat); lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; lr.receiveShadows = false;
            for (int i = 0; i < 64; i++) { float a = i / 64f * Mathf.PI * 2; lr.SetPosition(i, new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0)); }
            return lr;
        }

        void OnDestroy()
        {
            if (baseSun < 0) return;
            RenderSettings.fogDensity = baseDensity; RenderSettings.fogEndDistance = baseEnd; RenderSettings.fogColor = baseFog;
            if (sun) sun.intensity = baseSun;
        }

        void Update()
        {
            if (!d || !d.Ready) return;
            float dt = Time.deltaTime;
            // gate passed -> burst at the gate
            if (d.CheckpointsPassed > lastGates && d.CheckpointsPassed - 1 < d.checkpoints.Count)
            {
                var g = d.checkpoints[d.CheckpointsPassed - 1].transform.position;
                Burst(g + Vector3.up * 1.6f, new Color(1f, .6f, .2f), 50); Ring(g, new Color(1f, .6f, .2f), 7f, 1.1f);
            }
            lastGates = d.CheckpointsPassed;
            // generator back online -> power wave across the ground
            if (d.Repaired && !lastRepaired && d.generator)
            {
                var c = d.generator.transform.TransformPoint(new Vector3(10, 0, 5));
                Ring(c, new Color(.3f, 1f, .75f), 30f, 2.2f); Ring(c, new Color(0, .86f, 1f), 18f, 1.6f);
                Burst(d.generator.console.position + Vector3.up, new Color(.4f, 1f, .7f), 60);
            }
            lastRepaired = d.Repaired;
            // charge stream: particles flow from the pylon head into the rover
            if (d.charging && d.charging.Charging)
            {
                var from = d.charging.transform.TransformPoint(new Vector3(4.3f, 2.75f, 0));
                var to = P.rover.transform.position + Vector3.up * 1.3f;
                streamAcc += dt * (Reduced ? 25 : 70);
                var life = .55f;
                while (streamAcc >= 1)
                {
                    streamAcc -= 1;
                    var jitter = Random.insideUnitSphere * .15f;
                    stream.Emit(new ParticleSystem.EmitParams { position = from + jitter, velocity = (to - from) / life + Vector3.up * Random.Range(-.4f, .9f), startColor = new Color(0, .86f, 1f), startLifetime = life }, 1);
                }
            }
            // storm haze
            float target = d.AllComplete ? (d.StormActive || d.MissionIndex >= d.Missions.Count ? 1 : 0) : d.StormActive ? Mathf.Clamp01(1 - d.StormEta / MissionDirector.StormSeconds) : 0;
            storm = Mathf.MoveTowards(storm, target, dt * .25f);
            RenderSettings.fogDensity = baseDensity * (1 + 3.5f * storm);
            RenderSettings.fogEndDistance = Mathf.Lerp(baseEnd, baseEnd * .3f, storm);
            RenderSettings.fogColor = Color.Lerp(baseFog, new Color(.55f, .33f, .2f), storm * .7f);
            if (sun && baseSun >= 0) sun.intensity = baseSun * Mathf.Lerp(1, .5f, storm);
            // rings
            for (int i = rings.Count - 1; i >= 0; i--)
            {
                var r = rings[i]; r.t += dt; rings[i] = r;
                float k = Mathf.Clamp01(r.t / r.dur), e = 1 - Mathf.Pow(1 - k, 3);
                float rad = Mathf.Lerp(.3f, r.radius, e);
                r.lr.transform.localScale = new Vector3(rad, rad, 1); r.lr.widthMultiplier = Mathf.Lerp(.35f, .06f, k);
                var c = r.c; c.a = 1 - k; var m = r.lr.sharedMaterial; if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c); else m.color = c;
                if (k >= 1) { r.lr.gameObject.SetActive(false); pool.Push(r.lr); rings.RemoveAt(i); }
            }
        }
    }
}
