using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using AstroGateway.CinematicHUD;
using static AstroGateway.VisualUpgrade.EditorTools.VisualUpgradeKit;

namespace AstroGateway.VisualUpgrade.EditorTools
{
    // Visual system 2 — astronaut and rover presentation (TEMPORARY procedural art; the design team's rigged
    // astronaut and final rover were not found in the project). Gameplay objects, CharacterControllers, seat anchor,
    // wheel transforms and colliders are kept; only the original primitive renderers are switched off and new
    // collider-free parts are added beneath the existing transforms, so the limb pivots still drive the suit.
    public static class VisualUpgradeCharacters
    {
        public static readonly List<string> Notes = new List<string>();

        public static void Apply(AstroGatewayHUDController hud, GameObject adapters)
        {
            Notes.Clear();
            var p = hud.game.prototype;
            Astronaut(p, adapters);
            Rover(p, adapters);
        }

        // ---------------- astronaut ----------------
        static void Astronaut(Stage08B.MarsPrototype p, GameObject adapters)
        {
            var rig = p.astronaut.transform.Find("SuitRig");
            var hidden = rig.GetComponentsInChildren<Renderer>(true).ToList();
            foreach (var r in hidden) r.enabled = false;

            var fabric = Mat("Suit fabric", new Color(.86f, .85f, .80f), .22f);
            var hard = Mat("Suit hard shell", new Color(.92f, .92f, .90f), .62f);
            var grey = Mat("Suit soft grey", new Color(.50f, .52f, .53f), .32f, .1f);
            var joint = Mat("Suit joint bearing", new Color(.15f, .16f, .17f), .45f, .4f);
            var visor = Mat("Visor gold coating", new Color(.95f, .70f, .32f), .88f, .75f, new Color(.16f, .10f, .03f));
            var orange = Mat("Accent orange", new Color(.95f, .38f, .08f), .3f);
            var cyan = Mat("Status cyan", new Color(.05f, .25f, .3f), .5f, 0, new Color(.2f, 1.4f, 1.8f));
            var amber = Mat("Status amber", new Color(.3f, .15f, .02f), .5f, 0, new Color(1.8f, .8f, .1f));
            var lamp = Mat("Helmet lamp", new Color(.9f, .9f, .85f), .9f, 0, new Color(2.2f, 2.1f, 1.9f));
            var emblemMat = Mat("Mission emblem (provisional)", Color.white, .3f);
            emblemMat.SetTexture("_BaseMap", Emblem());
            emblemMat.SetFloat("_AlphaClip", 1); emblemMat.SetFloat("_Cutoff", .5f); Keywords(emblemMat);
            var enclosure = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/AstroGateway/ProductionVisual/Meshes/ChamferedEnclosure.asset");

            var suit = Group("Visual Upgrade suit — TEMPORARY procedural art (design-team asset pending)", rig);
            Part("Torso", suit, PrimitiveType.Capsule, new Vector3(0, 1.12f, 0), Vector3.zero, new Vector3(.50f, .37f, .34f), fabric);
            Part("Hip brief", suit, PrimitiveType.Capsule, new Vector3(0, .82f, 0), Vector3.zero, new Vector3(.42f, .20f, .30f), fabric);
            Part("Waist bearing", suit, PrimitiveType.Cylinder, new Vector3(0, .93f, 0), Vector3.zero, new Vector3(.43f, .03f, .32f), joint);
            Part("Neck ring", suit, PrimitiveType.Cylinder, new Vector3(0, 1.47f, 0), Vector3.zero, new Vector3(.30f, .035f, .30f), joint);
            Part("Display and control module", suit, enclosure, new Vector3(0, 1.22f, .17f), Vector3.zero, new Vector3(.30f, .19f, .09f), grey);
            for (int i = 0; i < 3; i++) Part("DCM status light " + i, suit, PrimitiveType.Cube, new Vector3(-.08f + i * .08f, 1.27f, .217f), Vector3.zero, new Vector3(.035f, .025f, .01f), i == 1 ? amber : cyan, false);
            Part("Chest emblem (provisional)", suit, PrimitiveType.Quad, new Vector3(.13f, 1.33f, .158f), new Vector3(0, 180, 0), Vector3.one * .085f, emblemMat, false);
            Part("Helmet shell", suit, PrimitiveType.Sphere, new Vector3(0, 1.70f, -.01f), Vector3.zero, Vector3.one * .44f, hard);
            Part("Gold visor", suit, PrimitiveType.Sphere, new Vector3(0, 1.69f, .065f), Vector3.zero, new Vector3(.36f, .30f, .34f), visor);
            foreach (int side in new[] { -1, 1 })
            {
                Part("Helmet lamp housing " + side, suit, PrimitiveType.Cylinder, new Vector3(side * .215f, 1.73f, .02f), new Vector3(90, 0, 0), new Vector3(.055f, .05f, .055f), grey);
                Part("Helmet lamp " + side, suit, PrimitiveType.Cylinder, new Vector3(side * .215f, 1.73f, .075f), new Vector3(90, 0, 0), new Vector3(.045f, .005f, .045f), lamp, false);
            }
            // Portable life-support backpack.
            Part("Life-support backpack", suit, enclosure, new Vector3(0, 1.20f, -.30f), Vector3.zero, new Vector3(.46f, .62f, .22f), hard);
            Part("Backpack top cap", suit, enclosure, new Vector3(0, 1.53f, -.30f), Vector3.zero, new Vector3(.40f, .07f, .19f), grey);
            Part("Backpack service panel", suit, PrimitiveType.Cube, new Vector3(0, 1.12f, -.415f), Vector3.zero, new Vector3(.28f, .26f, .01f), grey);
            foreach (int side in new[] { -1, 1 })
            {
                Part("Oxygen bottle " + side, suit, PrimitiveType.Capsule, new Vector3(side * .17f, 1.16f, -.42f), Vector3.zero, new Vector3(.08f, .2f, .08f), grey);
                Part("Hose " + side, suit, PrimitiveType.Cylinder, new Vector3(side * .16f, 1.05f, .05f), new Vector3(70, 0, side * 10), new Vector3(.03f, .2f, .03f), joint);
            }
            Part("Backpack antenna", suit, PrimitiveType.Cylinder, new Vector3(.15f, 1.72f, -.36f), Vector3.zero, new Vector3(.012f, .16f, .012f), joint);
            Part("Backpack beacon", suit, PrimitiveType.Sphere, new Vector3(.15f, 1.89f, -.36f), Vector3.zero, Vector3.one * .03f, amber, false);

            // Limbs under the existing pivots so the procedural motion swings them.
            foreach (var (pivot, side) in new[] { (p.leftArm, -1), (p.rightArm, 1) })
            {
                var g = Group("Visual Upgrade arm (temporary)", pivot);
                Part("Shoulder bearing", g, PrimitiveType.Sphere, new Vector3(0, -.02f, 0), Vector3.zero, Vector3.one * .19f, fabric);
                Part("Upper arm", g, PrimitiveType.Capsule, new Vector3(0, -.18f, 0), Vector3.zero, new Vector3(.165f, .17f, .165f), fabric);
                Part("Elbow bearing", g, PrimitiveType.Cylinder, new Vector3(0, -.34f, 0), Vector3.zero, new Vector3(.15f, .02f, .15f), joint);
                Part("Forearm", g, PrimitiveType.Capsule, new Vector3(0, -.47f, .01f), new Vector3(-6, 0, 0), new Vector3(.15f, .15f, .15f), fabric);
                Part("Wrist bearing", g, PrimitiveType.Cylinder, new Vector3(0, -.60f, .02f), Vector3.zero, new Vector3(.12f, .018f, .12f), joint);
                Part("Glove", g, enclosure, new Vector3(0, -.67f, .03f), Vector3.zero, new Vector3(.10f, .12f, .085f), grey);
                if (side < 0)
                {
                    Part("Commander stripe", g, PrimitiveType.Cylinder, new Vector3(0, -.10f, 0), Vector3.zero, new Vector3(.172f, .022f, .172f), orange);
                    Part("Shoulder emblem (provisional)", g, PrimitiveType.Quad, new Vector3(-.086f, -.14f, 0), new Vector3(0, 90, 0), Vector3.one * .08f, emblemMat, false);
                }
                Part("Wrist display", g, PrimitiveType.Cube, new Vector3(0, -.52f, .075f), Vector3.zero, new Vector3(.07f, .05f, .02f), side < 0 ? cyan : grey, false);
            }
            foreach (var pivot in new[] { p.leftLeg, p.rightLeg })
            {
                var g = Group("Visual Upgrade leg (temporary)", pivot);
                Part("Thigh", g, PrimitiveType.Capsule, new Vector3(0, -.18f, 0), Vector3.zero, new Vector3(.20f, .19f, .20f), fabric);
                Part("Knee bearing", g, PrimitiveType.Cylinder, new Vector3(0, -.36f, 0), Vector3.zero, new Vector3(.18f, .025f, .18f), joint);
                Part("Knee pad", g, enclosure, new Vector3(0, -.36f, .08f), Vector3.zero, new Vector3(.12f, .12f, .05f), grey);
                Part("Shin", g, PrimitiveType.Capsule, new Vector3(0, -.51f, 0), Vector3.zero, new Vector3(.18f, .17f, .18f), fabric);
                Part("Ankle bearing", g, PrimitiveType.Cylinder, new Vector3(0, -.64f, 0), Vector3.zero, new Vector3(.17f, .02f, .17f), joint);
                Part("EVA boot", g, enclosure, new Vector3(0, -.70f, .04f), Vector3.zero, new Vector3(.18f, .12f, .28f), grey);
                Part("Boot sole", g, PrimitiveType.Cube, new Vector3(0, -.755f, .04f), Vector3.zero, new Vector3(.19f, .03f, .29f), joint);
            }
            var motion = adapters.AddComponent<VisualUpgradeAstronautMotion>();
            motion.prototype = p; motion.suitRig = rig; motion.leftArm = p.leftArm; motion.rightArm = p.rightArm; motion.leftLeg = p.leftLeg; motion.rightLeg = p.rightLeg;
            Notes.Add($"Astronaut: {hidden.Count} primitive renderers hidden (kept), procedural suit + helmet/visor/backpack/emblem added, procedural walk/run blend");
        }

        // ---------------- rover ----------------
        static void Rover(Stage08B.MarsPrototype p, GameObject adapters)
        {
            var v = p.roverVisual;
            var enclosure = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/AstroGateway/ProductionVisual/Meshes/ChamferedEnclosure.asset");
            var graphite = Mat("Rover graphite", new Color(.18f, .19f, .2f), .35f, .5f);
            var panel = Mat("Rover body panel", new Color(.84f, .83f, .79f), .38f);
            var orange = Mat("Accent orange", new Color(.95f, .38f, .08f), .3f);
            var glass = Mat("Rover cockpit glass", new Color(.10f, .16f, .19f), .95f, .1f, null, .42f);
            var seat = Mat("Rover seat", new Color(.22f, .24f, .26f), .25f);
            var screen = Mat("Rover screen", new Color(.02f, .06f, .07f), .8f, 0, new Color(.15f, .9f, 1.1f));
            var head = Mat("Rover headlight lens", new Color(.95f, .95f, .9f), .95f, 0, new Color(3.5f, 3.3f, 3f));
            var tail = Mat("Rover tail light", new Color(.4f, .02f, .01f), .8f, 0, new Color(1.6f, .05f, .02f));
            var beacon = Mat("Rover beacon", new Color(.5f, .25f, .02f), .8f, 0, new Color(2.2f, 1.1f, .2f));
            var solar = Mat("Rover solar cells", new Color(.08f, .12f, .2f), .88f, .3f);

            // Cockpit: swap the opaque glazing submesh for tinted glass so the interior reads.
            var cab = v.Find("Cab").GetComponent<Renderer>();
            var cabMats = cab.sharedMaterials; if (cabMats.Length > 1) { cabMats[1] = glass; cab.sharedMaterials = cabMats; }
            foreach (var n in new[] { "Headlight -1", "Headlight 1" }) { var r = v.Find(n)?.GetComponent<Renderer>(); if (r) r.enabled = false; }
            var roof = v.Find("RoofMarker")?.GetComponent<Renderer>(); if (roof) roof.sharedMaterial = solar;

            var kit = Group("Visual Upgrade rover detail — TEMPORARY procedural art (final rover pending)", v);
            var lights = new List<Light>(); var screens = new List<Renderer>(); var brakes = new List<Renderer>(); var beacons = new List<Renderer>();
            foreach (int side in new[] { -1, 1 })
            {
                Part("Headlight housing " + side, kit, enclosure, new Vector3(side * .62f, 1.0f, 1.62f), Vector3.zero, new Vector3(.30f, .17f, .10f), graphite);
                Part("Headlight lens " + side, kit, PrimitiveType.Cube, new Vector3(side * .62f, 1.0f, 1.675f), Vector3.zero, new Vector3(.24f, .10f, .01f), head, false);
                var lg = new GameObject("Headlight spot " + side).transform; lg.SetParent(kit, false);
                lg.localPosition = new Vector3(side * .62f, 1.0f, 1.72f); lg.localEulerAngles = new Vector3(9, 0, 0);
                var l = lg.gameObject.AddComponent<Light>(); l.type = LightType.Spot; l.range = 28; l.spotAngle = 62; l.innerSpotAngle = 30; l.color = new Color(1, .96f, .9f); l.intensity = 0; l.shadows = LightShadows.None; l.enabled = false;
                lights.Add(l);
                brakes.Add(Part("Tail light " + side, kit, PrimitiveType.Cube, new Vector3(side * .72f, .95f, -1.615f), Vector3.zero, new Vector3(.22f, .07f, .02f), tail, false).GetComponent<Renderer>());
                Part("Side identification stripe " + side, kit, PrimitiveType.Cube, new Vector3(side * .958f, .93f, 0), Vector3.zero, new Vector3(.01f, .06f, 2.9f), orange, false);
                Part("Side storage locker " + side, kit, enclosure, new Vector3(side * .99f, .80f, -.9f), Vector3.zero, new Vector3(.08f, .32f, .9f), panel);
                Part("Hand rail " + side, kit, PrimitiveType.Cylinder, new Vector3(side * .8f, 1.78f, -.6f), new Vector3(90, 0, 0), new Vector3(.03f, .55f, .03f), graphite);
                for (int i = 0; i < 3; i++)
                {
                    float z = -1.05f + i * 1.05f;
                    Part($"Shock absorber {side}_{i}", kit, PrimitiveType.Cylinder, new Vector3(side * .98f, .62f, z + .14f), new Vector3(0, 0, side * -28), new Vector3(.07f, .17f, .07f), graphite);
                    Part($"Shock spring {side}_{i}", kit, PrimitiveType.Cylinder, new Vector3(side * .98f, .62f, z + .14f), new Vector3(0, 0, side * -28), new Vector3(.10f, .09f, .10f), orange);
                    Part($"Fender {side}_{i}", kit, enclosure, new Vector3(side * 1.1f, .86f, z), Vector3.zero, new Vector3(.32f, .05f, .82f), panel);
                }
            }
            Part("Front bumper and winch", kit, enclosure, new Vector3(0, .62f, 1.67f), Vector3.zero, new Vector3(1.6f, .18f, .16f), graphite);
            Part("Rear equipment rack", kit, PrimitiveType.Cube, new Vector3(0, 1.25f, -1.63f), Vector3.zero, new Vector3(1.4f, .05f, .06f), graphite);
            Part("Boarding step", kit, PrimitiveType.Cube, new Vector3(1.02f, .48f, .55f), Vector3.zero, new Vector3(.22f, .04f, .5f), graphite);
            Part("Roof solar frame", kit, PrimitiveType.Cube, new Vector3(0, 1.75f, .35f), Vector3.zero, new Vector3(1.46f, .03f, 1.26f), graphite);
            beacons.Add(Part("Mast beacon", kit, PrimitiveType.Sphere, new Vector3(0, 2.72f, -1.0f), Vector3.zero, Vector3.one * .11f, beacon, false).GetComponent<Renderer>());
            Part("High-gain antenna", kit, SaveMesh(Dish(), "VisualUpgradeRoverDish"), new Vector3(-.55f, 1.95f, -1.15f), new Vector3(-35, 0, 0), Vector3.one * .45f, panel);
            Part("Antenna mast", kit, PrimitiveType.Cylinder, new Vector3(-.55f, 1.85f, -1.15f), Vector3.zero, new Vector3(.03f, .1f, .03f), graphite);
            // Cockpit interior (visible through the tinted glass).
            foreach (int side in new[] { -1, 1 })
            {
                Part("Seat back " + side, kit, enclosure, new Vector3(side * .36f, 1.38f, .05f), new Vector3(-12, 0, 0), new Vector3(.42f, .5f, .1f), seat);
                Part("Seat base " + side, kit, enclosure, new Vector3(side * .36f, 1.12f, .25f), Vector3.zero, new Vector3(.42f, .08f, .42f), seat);
            }
            Part("Dashboard", kit, enclosure, new Vector3(0, 1.25f, .92f), new Vector3(-20, 0, 0), new Vector3(1.25f, .22f, .22f), graphite);
            for (int i = 0; i < 3; i++)
                screens.Add(Part("Cockpit display " + i, kit, PrimitiveType.Cube, new Vector3(-.4f + i * .4f, 1.33f, .84f), new Vector3(-20, 0, 0), new Vector3(.3f, .13f, .01f), screen, false).GetComponent<Renderer>());

            // Wheel dust: softer, larger, warmer particles (rate is still driven by EnvironmentPresentation).
            var dust = p.rover.transform.Find("Bounded rover wheel dust")?.GetComponent<ParticleSystem>();
            if (dust)
            {
                var hazeMat = AssetDatabase.LoadAssetAtPath<Material>(VisualUpgradeBuilder.F + "/Materials/VisualUpgradeHaze.mat");
                var dm = new Material(hazeMat) { name = "VisualUpgrade wheel dust" };
                dm.SetColor("_BaseColor", new Color(.78f, .55f, .38f, .38f));
                Keywords(dm);
                AssetDatabase.DeleteAsset(VisualUpgradeBuilder.F + "/Materials/VisualUpgradeWheelDust.mat");
                AssetDatabase.CreateAsset(dm, VisualUpgradeBuilder.F + "/Materials/VisualUpgradeWheelDust.mat");
                dust.GetComponent<ParticleSystemRenderer>().sharedMaterial = dm;
                var main = dust.main; main.startSize = new ParticleSystem.MinMaxCurve(.5f, 1.4f); main.startLifetime = new ParticleSystem.MinMaxCurve(1.2f, 2.4f); main.startSpeed = new ParticleSystem.MinMaxCurve(.4f, 1.2f); main.maxParticles = 140;
                var size = dust.sizeOverLifetime; size.enabled = true; size.size = new ParticleSystem.MinMaxCurve(1, AnimationCurve.Linear(0, .5f, 1, 1.8f));
                var col = dust.colorOverLifetime; col.enabled = true;
                var grad = new Gradient(); grad.SetKeys(new[] { new GradientColorKey(Color.white, 0), new GradientColorKey(Color.white, 1) }, new[] { new GradientAlphaKey(.0f, 0), new GradientAlphaKey(1, .15f), new GradientAlphaKey(0, 1) });
                col.color = grad;
                var shape = dust.shape; shape.shapeType = ParticleSystemShapeType.Box; shape.scale = new Vector3(2.2f, .2f, .6f);
            }

            // Seated driver shown through the cockpit glass while crewed (the walking suit is hidden then: it is
            // taller than the cab). Visual only; the real astronaut object stays parented to the seat anchor.
            var driver = Group("Seated driver (visual only, TEMPORARY)", kit, new Vector3(-.36f, 0, .22f));
            var fabric = Mat("Suit fabric", new Color(.86f, .85f, .80f), .22f);
            var hard = Mat("Suit hard shell", new Color(.92f, .92f, .90f), .62f);
            var visorMat = Mat("Visor gold coating", new Color(.95f, .70f, .32f), .88f, .75f, new Color(.16f, .10f, .03f));
            Part("Driver torso", driver, PrimitiveType.Capsule, new Vector3(0, 1.33f, 0), new Vector3(-8, 0, 0), new Vector3(.34f, .2f, .26f), fabric);
            Part("Driver helmet", driver, PrimitiveType.Sphere, new Vector3(0, 1.58f, .02f), Vector3.zero, Vector3.one * .25f, hard);
            Part("Driver visor", driver, PrimitiveType.Sphere, new Vector3(0, 1.575f, .07f), Vector3.zero, new Vector3(.2f, .16f, .19f), visorMat);
            foreach (int side in new[] { -1, 1 })
                Part("Driver arm " + side, driver, PrimitiveType.Capsule, new Vector3(side * .19f, 1.3f, .2f), new Vector3(70, 0, 0), new Vector3(.1f, .17f, .1f), fabric);
            driver.gameObject.SetActive(false);

            var pres = adapters.AddComponent<VisualUpgradeRoverPresentation>();
            pres.driver = driver.gameObject;
            pres.prototype = p; pres.headlights = lights.ToArray(); pres.screens = screens.ToArray(); pres.brakeLights = brakes.ToArray(); pres.beacons = beacons.ToArray();
            Notes.Add($"Rover: tinted cockpit glass + interior, headlights ({lights.Count} spot lights, on while aboard), tail/brake lights, beacon, shocks, fenders, antenna, softer wheel dust");
        }
    }
}
