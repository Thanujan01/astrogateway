using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using AstroGateway.FirstMission.Nav;

// Cinematic motion layer for the Explorer scene only (presentation; never changes gameplay state).
//  - Intro fly-in: a 4.5 s camera move from high over the base down to the astronaut, HUD fades in after; any
//    tap / key skips it.
//  - Waypoint beacon: a soft light pillar at the navigation target with a pulsing ground ring.
//  - Scan effects: a scan beam from the rover mast or arm (or the astronaut) to the station and an expanding
//    ring on the ground while an observation runs; a short shimmer when it completes.
//  - Ambient dust motes drifting around the camera (60 particles, world space).
//  - Driving camera motion: speed-based FOV kick and a gentle terrain-rumble shake.
//  - UI motion: slide/fade-in of the map, drawer, popups and record view (HudSlideIn).
// Everything respects the Reduced motion setting (no intro, no shake, no FOV kick, no slides).
// Cost: a handful of draw calls, one particle system, no post-processing, no per-frame allocation.
namespace AstroGateway.FirstMission.HUD
{
    [DefaultExecutionOrder(540)]   // after the camera rig (520)
    public class ExplorerCinematics : MonoBehaviour
    {
        public ExplorerHUD hud;
        public bool IntroActive { get; private set; }
        public GameObject Beacon { get; private set; }
        public LineRenderer ScanBeam { get; private set; }
        public LineRenderer ScanRing { get; private set; }
        public ParticleSystem Motes { get; private set; }
        public float LastFovKick { get; private set; }
        public float LastShake { get; private set; }
        float introT, shimmer, beaconPulse; Material beaconMat, beamMat, ringMat; CanvasGroup hudGroup; Transform beaconRing;
        float baseFov, appliedKick; bool skippedByInput;
        public bool SkippedByInput => skippedByInput;
        const float IntroSeconds = 4.5f;

        bool Reduced => hud.mission.Settings.reducedMotion;

        public void Init(ExplorerHUD h)
        {
            hud = h;
            var shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
            var dust = hud.mission.hud.environment.wheelDust;
            if (dust && dust.GetComponent<ParticleSystemRenderer>().sharedMaterial) shader = dust.GetComponent<ParticleSystemRenderer>().sharedMaterial.shader;
            Material Mat(string n, Color c, Texture tex)
            {
                // Copy the scene's transparent particle material so blending/keywords match (a bare
                // new Material(shader) renders opaque in URP).
                var src = dust ? dust.GetComponent<ParticleSystemRenderer>().sharedMaterial : null;
                var m = src ? new Material(src) { name = n } : new Material(shader) { name = n };
                if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c); else m.color = c;
                if (tex) { if (m.HasProperty("_BaseMap")) m.SetTexture("_BaseMap", tex); else m.mainTexture = tex; }
                m.renderQueue = 3100;
                return m;
            }
            var grad = Gradient();
            beaconMat = Mat("Beacon", new Color(1f, .55f, .15f, .35f), grad);
            var beaconRingMat = Mat("Beacon ring", new Color(1f, .55f, .15f, .8f), Gradient(true));
            beamMat = Mat("Scan beam", new Color(0, .86f, 1f, .7f), Gradient(true));
            ringMat = Mat("Scan ring", new Color(0, .86f, 1f, .6f), Gradient(true));

            // Waypoint beacon (unlit cylinder without collider + a flat ring).
            Beacon = GameObject.CreatePrimitive(PrimitiveType.Cylinder); Beacon.name = "Waypoint beacon (FX)";
            Destroy(Beacon.GetComponent<Collider>());
            Beacon.transform.localScale = new Vector3(.9f, 18f, .9f);
            var br = Beacon.GetComponent<Renderer>(); br.sharedMaterial = beaconMat; br.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; br.receiveShadows = false;
            beaconRing = new GameObject("Beacon ring").transform; beaconRing.SetParent(Beacon.transform, false);
            var lr = beaconRing.gameObject.AddComponent<LineRenderer>(); SetupRing(lr, beaconRingMat, 48); lr.widthMultiplier = .14f; lr.textureMode = LineTextureMode.Stretch;
            Beacon.SetActive(false);

            ScanBeam = new GameObject("Scan beam (FX)").AddComponent<LineRenderer>();
            ScanBeam.sharedMaterial = beamMat; ScanBeam.positionCount = 2; ScanBeam.widthCurve = new AnimationCurve(new Keyframe(0, .05f), new Keyframe(1, .18f)); ScanBeam.useWorldSpace = true;
            ScanBeam.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; ScanBeam.enabled = false;
            ScanRing = new GameObject("Scan ring (FX)").AddComponent<LineRenderer>(); SetupRing(ScanRing, ringMat, 64); ScanRing.enabled = false;

            // Ambient dust motes around the camera.
            var cam = hud.mission.hud.game.prototype.follow;
            var go = new GameObject("Ambient dust motes (FX)"); go.transform.SetParent(cam.transform, false); go.transform.localPosition = new Vector3(0, 0, 6);
            Motes = go.AddComponent<ParticleSystem>();
            var main = Motes.main; main.maxParticles = 60; main.startLifetime = 9; main.startSpeed = .15f; main.startSize = new ParticleSystem.MinMaxCurve(.02f, .06f);
            main.startColor = new Color(1f, .82f, .62f, .5f); main.simulationSpace = ParticleSystemSimulationSpace.World; main.gravityModifier = -.002f;
            var em = Motes.emission; em.rateOverTime = 7;
            var sh = Motes.shape; sh.shapeType = ParticleSystemShapeType.Box; sh.scale = new Vector3(14, 6, 10);
            var vel = Motes.velocityOverLifetime; vel.enabled = true; vel.space = ParticleSystemSimulationSpace.World; vel.x = new ParticleSystem.MinMaxCurve(.25f, .6f); vel.y = new ParticleSystem.MinMaxCurve(-.05f, .05f); vel.z = new ParticleSystem.MinMaxCurve(-.1f, .2f);
            var col = Motes.colorOverLifetime; col.enabled = true; var g = new Gradient(); g.SetKeys(new[] { new GradientColorKey(Color.white, 0), new GradientColorKey(Color.white, 1) }, new[] { new GradientAlphaKey(0, 0), new GradientAlphaKey(.6f, .3f), new GradientAlphaKey(0, 1) }); col.color = g;
            var pr = go.GetComponent<ParticleSystemRenderer>(); pr.sharedMaterial = dust ? dust.GetComponent<ParticleSystemRenderer>().sharedMaterial : Mat("Motes", Color.white, null);
            pr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            Motes.Play();

            // HUD fade + intro.
            hudGroup = hud.Safe.GetComponentInParent<Canvas>().gameObject.GetComponent<CanvasGroup>();
            if (!hudGroup) hudGroup = hud.Safe.GetComponentInParent<Canvas>().gameObject.AddComponent<CanvasGroup>();
            baseFov = cam.fieldOfView;
            if (!Reduced) { IntroActive = true; introT = 0; hudGroup.alpha = 0; }
        }

        // Vertical fade (beacon: bright at the ground, fading up) or a soft band across a line's width.
        static Texture2D Gradient(bool band = false)
        {
            var t = new Texture2D(band ? 4 : 32, 64, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp, name = band ? "Soft band" : "Beacon gradient" };
            for (int y = 0; y < 64; y++)
                for (int x = 0; x < t.width; x++)
                {
                    float v = y / 63f, a;
                    if (band) a = Mathf.Pow(Mathf.Sin(v * Mathf.PI), 1.5f);
                    else { float u = x / (float)(t.width - 1); a = Mathf.Pow(1 - v, 1.8f) * Mathf.Pow(Mathf.Sin(u * Mathf.PI), 2); }
                    t.SetPixel(x, y, new Color(1, 1, 1, a));
                }
            t.Apply(); return t;
        }

        static void SetupRing(LineRenderer lr, Material m, int n)
        {
            lr.sharedMaterial = m; lr.loop = true; lr.positionCount = n; lr.useWorldSpace = false; lr.widthMultiplier = .08f;
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            for (int i = 0; i < n; i++) { float a = i * Mathf.PI * 2 / n; lr.SetPosition(i, new Vector3(Mathf.Cos(a), 0, Mathf.Sin(a))); }
        }

        public void SkipIntro() { if (!IntroActive) return; IntroActive = false; var rig = hud.mission.legacyRig; rig.captureOverride = false; hudGroup.alpha = 1; }

        void LateUpdate()
        {
            if (hud == null) return;
            var p = hud.mission.hud.game.prototype; var cam = p.follow; float dt = Time.unscaledDeltaTime;
            UpdateIntro(p);
            UpdateBeacon(dt);
            UpdateScanFx(p, dt);
            if (Motes) { var em = Motes.emission; em.enabled = !Reduced && !hud.MapOverlay.activeSelf; }

            // Driving camera motion (after the rig placed the camera this frame). The rig rebuilds the camera
            // position from its own smoothed state each frame, so the shake offset never accumulates; the rig
            // blends FOV from the current value, so last frame's kick is removed before the new one is added.
            if (cam && !IntroActive)
            {
                cam.fieldOfView -= appliedKick; appliedKick = 0;
                if (p.driving && !Reduced && !hud.MapOverlay.activeSelf)
                {
                    float s01 = Mathf.Clamp01(Mathf.Abs(p.speed) / 3.8f);
                    hud.mission.sensors.Terrain(out _, out float rough);
                    float amp = s01 * (.012f + Mathf.Min(.04f, rough * .2f));
                    float t = Time.time * 13;
                    cam.transform.position += cam.transform.rotation * (new Vector3(Mathf.PerlinNoise(t, 0) - .5f, Mathf.PerlinNoise(0, t) - .5f, 0) * 2 * amp);
                    LastShake = amp;
                    LastFovKick = Mathf.Lerp(LastFovKick, s01 * 5f, 1 - Mathf.Exp(-3 * dt));
                }
                else { LastShake = 0; LastFovKick = Mathf.Lerp(LastFovKick, 0, 1 - Mathf.Exp(-4 * dt)); }
                appliedKick = LastFovKick; cam.fieldOfView += appliedKick;
            }
        }

        void UpdateIntro(Stage08B.MarsPrototype p)
        {
            if (!IntroActive) return;
            var rig = hud.mission.legacyRig;
            bool input = (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame) || (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame) || (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame);
            if (input) { skippedByInput = true; SkipIntro(); return; }
            introT += Time.unscaledDeltaTime / IntroSeconds;
            float e = Mathf.SmoothStep(0, 1, Mathf.Clamp01(introT));
            var target = p.astronaut.transform.position + Vector3.up * 1.4f;
            var behind = target + Quaternion.Euler(p.pitch, p.yaw, 0) * new Vector3(.35f, .15f, -4.8f);
            var high = target + Quaternion.Euler(0, p.yaw + 140, 0) * new Vector3(0, 0, -60) + Vector3.up * 38;
            var mid = Vector3.Lerp(high, behind, .6f) + Vector3.up * 10;
            var a1 = Vector3.Lerp(high, mid, e); var a2 = Vector3.Lerp(mid, behind, e);
            rig.captureOverride = true; rig.capturePosition = Vector3.Lerp(a1, a2, e);
            rig.captureTarget = Vector3.Lerp(target + Vector3.up * 2, target, e);
            hudGroup.alpha = Mathf.Clamp01((introT - .75f) / .25f);
            if (introT >= 1) SkipIntro();
        }

        void UpdateBeacon(float dt)
        {
            var target = hud.Nav.Target;
            bool on = target != null && target.transform && !hud.Nav.Arrived && !IntroActive;
            if (Beacon.activeSelf != on) Beacon.SetActive(on);
            if (!on) return;
            var pos = target.Position;
            if (Physics.Raycast(pos + Vector3.up * 30, Vector3.down, out var hit, 60, 1, QueryTriggerInteraction.Ignore) && target.category != LocationCategory.Rover) pos.y = hit.point.y;
            Beacon.transform.position = pos + Vector3.up * 18f;
            beaconPulse += dt * (Reduced ? 0 : 1);
            float pulse = .75f + .25f * Mathf.Sin(beaconPulse * 2.4f);
            var c = new Color(1f, .55f, .15f, .30f * pulse);
            if (beaconMat.HasProperty("_BaseColor")) beaconMat.SetColor("_BaseColor", c); else beaconMat.color = c;
            float r = 2.2f + (Reduced ? 0 : Mathf.Repeat(beaconPulse * .8f, 1) * 1.5f);
            beaconRing.localPosition = new Vector3(0, -1f + .02f, 0); beaconRing.localScale = new Vector3(r / .9f, 1 / 18f, r / .9f);
        }

        void UpdateScanFx(Stage08B.MarsPrototype p, float dt)
        {
            var m = hud.mission; bool rover = m.Observing != null; bool foot = m.hud.scanning;
            var station = rover ? m.sensors.Target(out _) : foot ? m.hud.CurrentTarget() : null;
            bool on = (rover || foot) && station;
            if (on)
            {
                Vector3 from = p.ActivePosition() + Vector3.up * (rover ? 2.3f : 1.3f);
                var ares = m.ares ? m.ares.ares : null;
                if (rover && ares)
                {
                    var head = ares.Find(m.Observing.id == "rover.arm_sampler" ? "Chassis/Arm_Yaw/Arm_Upper/Arm_Fore/Arm_Wrist/Arm_Drill" : "Chassis/Mast_Pan/Mast_Tilt");
                    if (head) from = head.position;
                }
                var to = station.transform.position + Vector3.up * .8f;
                ScanBeam.enabled = true; ScanBeam.SetPosition(0, from); ScanBeam.SetPosition(1, to);
                float flicker = Reduced ? 1 : .7f + .3f * Mathf.PerlinNoise(Time.time * 9, 0);
                var bc = new Color(0, .86f, 1f, .65f * flicker); if (beamMat.HasProperty("_BaseColor")) beamMat.SetColor("_BaseColor", bc); else beamMat.color = bc;
                float progress = rover ? m.ObservationProgress : .5f;
                ScanRing.enabled = true; ScanRing.transform.position = new Vector3(to.x, station.transform.position.y + .05f, to.z);
                float rr = .6f + (Reduced ? 1.4f : Mathf.Repeat(Time.time * .9f, 1) * 2.4f);
                ScanRing.transform.localScale = new Vector3(rr, 1, rr);
                shimmer = 1;
            }
            else
            {
                ScanBeam.enabled = false;
                shimmer = Mathf.MoveTowards(shimmer, 0, dt * 2);
                ScanRing.enabled = shimmer > 0;
                if (shimmer > 0) { ScanRing.transform.localScale *= 1 + dt * 2; var rc = new Color(0, .86f, 1f, .6f * shimmer); if (ringMat.HasProperty("_BaseColor")) ringMat.SetColor("_BaseColor", rc); else ringMat.color = rc; }
                else { var rc = new Color(0, .86f, 1f, .6f); if (ringMat.HasProperty("_BaseColor")) ringMat.SetColor("_BaseColor", rc); else ringMat.color = rc; }
            }
        }
    }

    // Slide + fade in when a panel becomes active (UI motion). Off with reduced motion.
    public class HudSlideIn : MonoBehaviour
    {
        public Vector2 from = new Vector2(0, -40); public float seconds = .28f; public System.Func<bool> reduced;
        RectTransform rt; CanvasGroup cg; Vector2 home; float t; bool init;
        void Awake() { rt = (RectTransform)transform; cg = GetComponent<CanvasGroup>(); if (!cg) cg = gameObject.AddComponent<CanvasGroup>(); }
        void OnEnable()
        {
            if (!init) { home = rt.anchoredPosition; init = true; }
            t = reduced != null && reduced() ? 1 : 0; Apply();
        }
        void Update() { if (t >= 1) return; t = Mathf.MoveTowards(t, 1, Time.unscaledDeltaTime / seconds); Apply(); }
        void Apply()
        {
            float e = 1 - Mathf.Pow(1 - t, 3);
            rt.anchoredPosition = home + from * (1 - e); cg.alpha = Mathf.Lerp(0, 1, e);
            rt.localScale = Vector3.one * Mathf.Lerp(.97f, 1, e);
        }
    }
}
