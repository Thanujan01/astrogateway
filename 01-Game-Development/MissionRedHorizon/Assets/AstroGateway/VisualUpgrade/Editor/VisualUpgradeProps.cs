using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using AstroGateway.CinematicHUD;
using static AstroGateway.VisualUpgrade.EditorTools.VisualUpgradeKit;

namespace AstroGateway.VisualUpgrade.EditorTools
{
    // Visual system 3 — research stations, habitat exteriors, communications tower and solar farm detailing
    // (TEMPORARY procedural art). Station colliders and ResearchTarget components stay where they are; only their
    // orange placeholder cylinders are hidden. Nothing new has a collider.
    public static class VisualUpgradeProps
    {
        public static readonly List<string> Notes = new List<string>();

        public static void Apply(AstroGatewayHUDController hud, GameObject adapters)
        {
            Notes.Clear();
            var root = GameObject.Find("Visual Upgrade v1 — environment (temporary art, no colliders)").transform;
            var enclosure = AssetDatabase.LoadAssetAtPath<Mesh>("Assets/AstroGateway/ProductionVisual/Meshes/ChamferedEnclosure.asset");
            var white = Mat("Equipment white", new Color(.85f, .84f, .80f), .4f);
            var graphite = Mat("Rover graphite", new Color(.18f, .19f, .2f), .35f, .5f);
            var aluminium = Mat("Brushed aluminium", new Color(.62f, .64f, .66f), .55f, .8f);
            var orange = Mat("Accent orange", new Color(.95f, .38f, .08f), .3f);
            var cyan = Mat("Status cyan", new Color(.05f, .25f, .3f), .5f, 0, new Color(.2f, 1.4f, 1.8f));
            var lens = Mat("Sensor lens", new Color(.02f, .03f, .05f), .97f, .2f);
            var solar = Mat("Rover solar cells", new Color(.08f, .12f, .2f), .88f, .3f);
            var amber = Mat("Status amber", new Color(.3f, .15f, .02f), .5f, 0, new Color(1.8f, .8f, .1f));
            var red = Mat("Beacon red", new Color(.4f, .02f, .01f), .8f, 0, new Color(2.4f, .1f, .05f));
            var warm = Mat("Habitat marker light", new Color(.4f, .3f, .2f), .7f, 0, new Color(2.0f, 1.3f, .6f));
            var dish = SaveMesh(Dish(10, 32, .3f), "VisualUpgradeDish");
            var blinkTargets = new List<Renderer>();
            var statusStations = new List<AstroGateway.MarsEnvironment.ResearchTarget>(); var statusRings = new List<Transform>();
            var ringMat = Mat("Scan ring", new Color(.2f, .8f, 1f), .2f, 0, new Color(.2f, 1.4f, 1.8f), .5f);

            // ---- Research stations: tripod instrument with sensor head, panel, dish and status light ----
            var stations = Group("Research station instruments (visual only)", root);
            int index = 0;
            foreach (var t in hud.environment.targets.Where(t => t))
            {
                var r = t.GetComponent<Renderer>(); if (r) r.enabled = false;
                float baseY = t.transform.position.y - t.transform.lossyScale.y; // cylinder half height = scale.y
                var g = Group("Instrument — " + t.displayName, stations);
                g.position = new Vector3(t.transform.position.x, baseY, t.transform.position.z);
                g.eulerAngles = new Vector3(0, index * 67 + 20, 0);
                for (int leg = 0; leg < 3; leg++)
                {
                    float a = leg * 120;
                    var dir = Quaternion.Euler(0, a, 0) * Vector3.forward;
                    Vector3 top = Vector3.up * .98f, foot = dir * .5f;
                    var axis = top - foot;
                    // local direction: the group is rotated, so build the rotation in local space
                    Part("Tripod leg " + leg, g, PrimitiveType.Cylinder, (top + foot) / 2, Quaternion.FromToRotation(Vector3.up, axis.normalized).eulerAngles, new Vector3(.035f, axis.magnitude / 2, .035f), aluminium);
                    Part("Foot " + leg, g, PrimitiveType.Cylinder, dir * .5f + Vector3.up * .02f, Vector3.zero, new Vector3(.09f, .02f, .09f), graphite);
                }
                Part("Sensor head", g, enclosure, new Vector3(0, 1.08f, 0), Vector3.zero, new Vector3(.42f, .26f, .32f), white);
                Part("Head stripe", g, PrimitiveType.Cube, new Vector3(0, 1.13f, .162f), Vector3.zero, new Vector3(.40f, .03f, .01f), orange, false);
                Part("Spectrometer lens", g, PrimitiveType.Cylinder, new Vector3(0, 1.04f, .2f), new Vector3(90, 0, 0), new Vector3(.12f, .05f, .12f), lens);
                Part("Lens ring", g, PrimitiveType.Cylinder, new Vector3(0, 1.04f, .175f), new Vector3(90, 0, 0), new Vector3(.15f, .02f, .15f), graphite);
                Part("Solar panel", g, PrimitiveType.Cube, new Vector3(0, 1.32f, -.12f), new Vector3(-35, 0, 0), new Vector3(.55f, .02f, .34f), solar);
                Part("Panel arm", g, PrimitiveType.Cylinder, new Vector3(0, 1.24f, -.08f), Vector3.zero, new Vector3(.025f, .05f, .025f), aluminium);
                Part("Relay dish", g, dish, new Vector3(.2f, 1.42f, .05f), new Vector3(-30, 40, 0), Vector3.one * .28f, white);
                Part("Dish mast", g, PrimitiveType.Cylinder, new Vector3(.2f, 1.3f, .05f), Vector3.zero, new Vector3(.02f, .1f, .02f), aluminium);
                blinkTargets.Add(Part("Status light", g, PrimitiveType.Sphere, new Vector3(-.17f, 1.23f, .1f), Vector3.zero, Vector3.one * .05f, cyan, false).GetComponent<Renderer>());
                Part("Sample case", g, enclosure, new Vector3(.45f, .12f, .25f), new Vector3(0, 25, 0), new Vector3(.36f, .2f, .26f), orange);
                for (int m = 0; m < 8; m++)
                {
                    var d = Quaternion.Euler(0, m * 45, 0) * Vector3.forward * 1.6f;
                    Part("Survey marker " + m, g, PrimitiveType.Cylinder, d + Vector3.up * .06f, Vector3.zero, new Vector3(.04f, .07f, .04f), m % 2 == 0 ? orange : white, false);
                }
                statusStations.Add(t);
                var ring = Part("Scan ring (visual only)", g, PrimitiveType.Cylinder, new Vector3(0, .03f, 0), Vector3.zero, new Vector3(1, .004f, 1), ringMat, false);
                ring.SetActive(false);
                statusRings.Add(ring.transform);
                index++;
            }
            // Batch 3: per-station state light (amber = not surveyed, cyan pulse = scanning, green = recorded) and a
            // ground ring during a scan; replaces the uniform cyan pulse.
            var status = adapters.AddComponent<VisualUpgradeStationStatus>();
            status.hud = hud; status.stations = statusStations.ToArray(); status.lights = blinkTargets.ToArray(); status.rings = statusRings.ToArray();

            // ---- Habitats: lit windows, foundation marker lights, airlock status lights and floodlights ----
            var glazing = AssetDatabase.LoadAssetAtPath<Material>("Assets/AstroGateway/ProductionVisual/Materials/Opaque protective visor glazing.mat");
            var window = Mat("Habitat window glow", new Color(.04f, .05f, .06f), .93f, .1f, new Color(.42f, .26f, .12f));
            var shellMat = Mat("Habitat shell", new Color(.88f, .86f, .81f), .35f);
            int windows = 0, markers = 0;
            foreach (var name in new[] { "Crew Dome", "Laboratory Dome", "Life Support Dome" })
            {
                var dome = GameObject.Find(name); if (!dome) continue;
                var r = dome.GetComponent<Renderer>();
                r.sharedMaterials = r.sharedMaterials.Select(m => m == glazing ? window : m && m.name.StartsWith("Pressure shell") ? shellMat : m).ToArray();
                windows++;
                var b = r.bounds; var c = new Vector3(b.center.x, 0, b.center.z); float rad = b.extents.x;
                var g = Group(name + " exterior detail (visual only)", root);
                for (int i = 0; i < 16; i++)
                {
                    var d = Quaternion.Euler(0, i * 22.5f, 0) * Vector3.forward;
                    Part("Foundation light " + i, g, PrimitiveType.Cube, c + d * (rad + .05f) + Vector3.up * .25f, Quaternion.LookRotation(d).eulerAngles, new Vector3(.25f, .05f, .03f), warm, false);
                    markers++;
                }
                var airlock = GameObject.Find(name + " sealed airlock");
                if (airlock)
                {
                    var a = airlock.transform; var ab = airlock.GetComponent<Collider>().bounds;
                    var outward = (ab.center - c); outward.y = 0; outward.Normalize();
                    var front = ab.center + outward * (Mathf.Abs(Vector3.Dot(ab.extents, new Vector3(Mathf.Abs(outward.x), 0, Mathf.Abs(outward.z)))) + .03f);
                    var rot = Quaternion.LookRotation(outward).eulerAngles;
                    var side = Vector3.Cross(Vector3.up, outward);
                    Part("Airlock status light", g, PrimitiveType.Cube, front + Vector3.up * .9f + side * .9f, rot, new Vector3(.12f, .3f, .03f), cyan, false);
                    Part("Airlock emblem (provisional)", g, PrimitiveType.Quad, front + Vector3.up * .55f - side * .9f, rot + new Vector3(0, 180, 0), Vector3.one * .45f, AssetDatabase.LoadAssetAtPath<Material>(VisualUpgradeBuilder.F + "/Materials/VU_Mission_emblem_(provisional).mat"), false);
                    Part("Airlock hazard stripe", g, PrimitiveType.Cube, front + Vector3.down * 1.35f + Vector3.up * 1.6f, rot, new Vector3(2.6f, .08f, .02f), orange, false);
                    var lg = new GameObject("Airlock floodlight").transform; lg.SetParent(g, false);
                    lg.position = front + Vector3.up * 1.85f + outward * .4f; lg.rotation = Quaternion.LookRotation(outward * .5f + Vector3.down);
                    var l = lg.gameObject.AddComponent<Light>(); l.type = LightType.Spot; l.range = 12; l.spotAngle = 95; l.intensity = 3.2f; l.color = new Color(1, .9f, .78f); l.shadows = LightShadows.None;
                    Part("Floodlight housing", g, enclosure, front + Vector3.up * 1.85f + outward * .2f, rot, new Vector3(.4f, .12f, .25f), graphite);
                    Part("Floodlight lens", g, PrimitiveType.Cube, front + Vector3.up * 1.79f + outward * .2f, rot, new Vector3(.32f, .01f, .18f), warm, false);
                }
            }

            // ---- Communications tower: parabolic dish, feed horn, guy wires, red beacon ----
            var reflector = GameObject.Find("Communication reflector");
            if (reflector)
            {
                reflector.GetComponent<Renderer>().enabled = false;
                var g = Group("Communications tower detail (visual only)", root);
                var top = reflector.transform.position;
                Part("Parabolic dish", g, dish, top + new Vector3(0, .1f, 0), new Vector3(-40, 25, 0), Vector3.one * 3.2f, white);
                Part("Feed support", g, PrimitiveType.Cylinder, top + new Vector3(0, .55f, .5f), new Vector3(-40, 25, 0), new Vector3(.05f, .6f, .05f), aluminium);
                Part("Feed horn", g, PrimitiveType.Cylinder, top + new Vector3(.25f, 1.1f, .9f), new Vector3(-40, 25, 0), new Vector3(.18f, .14f, .18f), graphite);
                var beacon = Part("Aviation beacon", g, PrimitiveType.Sphere, top + new Vector3(0, -.2f, 0) + Vector3.up * 1.6f, Vector3.zero, Vector3.one * .22f, red, false);
                var blink = adapters.AddComponent<VisualUpgradeBlink>(); blink.targets = new[] { beacon.GetComponent<Renderer>() }; blink.color = new Color(2.4f, .1f, .05f); blink.period = 1.8f; blink.onFraction = .25f; blink.offLevel = .08f;
                var mastBase = new Vector3(top.x, 0, top.z);
                for (int i = 0; i < 3; i++)
                {
                    var d = Quaternion.Euler(0, i * 120 + 30, 0) * Vector3.forward;
                    var a = mastBase + Vector3.up * 8.5f; var bpt = mastBase + d * 6f;
                    var mid = (a + bpt) / 2; float len = Vector3.Distance(a, bpt);
                    Part("Guy wire " + i, g, PrimitiveType.Cylinder, mid, Quaternion.FromToRotation(Vector3.up, (a - bpt).normalized).eulerAngles, new Vector3(.025f, len / 2, .025f), graphite, false);
                    Part("Anchor " + i, g, enclosure, bpt + Vector3.up * .1f, Vector3.zero, new Vector3(.4f, .2f, .4f), graphite);
                }
                Notes.Add("Comms tower: dish, feed, guy wires, blinking beacon");
            }

            // ---- Solar farm: blue-black photovoltaic material copy with grid texture ----
            var pv = AssetDatabase.LoadAssetAtPath<Material>("Assets/AstroGateway/ProductionVisual/Materials/Inherited_2.mat");
            int panels = 0;
            if (pv)
            {
                var copy = new Material(pv) { name = "VisualUpgrade photovoltaic" };
                copy.SetColor("_BaseColor", new Color(.52f, .62f, .8f)); copy.SetFloat("_Smoothness", .86f); copy.SetFloat("_Metallic", .35f);
                AssetDatabase.DeleteAsset(VisualUpgradeBuilder.F + "/Materials/VisualUpgradePhotovoltaic.mat");
                AssetDatabase.CreateAsset(copy, VisualUpgradeBuilder.F + "/Materials/VisualUpgradePhotovoltaic.mat");
                foreach (var rr in Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None))
                    if (rr.sharedMaterials.Contains(pv)) { rr.sharedMaterials = rr.sharedMaterials.Select(m => m == pv ? copy : m).ToArray(); panels++; }
            }
            Notes.Add($"Stations: {index} instruments; habitats: {windows} domes with lit windows, {markers} foundation lights, airlock lights/floods; solar renderers updated: {panels}");
        }
    }
}
