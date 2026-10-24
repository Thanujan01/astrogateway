using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using AstroGateway.FirstMission.Missions;

// Places the mission structures into the Missions scene (called by FirstMissionBuilder.BuildMissions on its own
// scene copy only). Uses the project's existing environment prefabs and materials where they fit (power station,
// rover bay) and simple modelled props for the rest (tents, charging pad, checkpoint gates). Every structure is
// grounded with a raycast and gets real colliders, so the route grid and the rover collide with it.
namespace AstroGateway.FirstMission.EditorTools
{
    public static class MissionStructuresBuilder
    {
        const string Env = "Assets/AstroGateway/Environment/";
        static Material OffWhite, Dark, Metal, Cyan, Orange, Glass;

        public static MissionDirector Place(GameObject systems, FirstMissionController mission, HUD.MissionsHUD ui, List<string> notes)
        {
            Material M(string n) => AssetDatabase.LoadAssetAtPath<Material>(Env + "Materials/" + n + ".mat") ?? throw new Exception("material missing " + n);
            OffWhite = M("Habitat_OffWhite"); Dark = M("Habitat_CompositeDark"); Metal = M("Metal_Industrial"); Cyan = M("Accent_Cyan"); Orange = M("Accent_SafetyOrange"); Glass = M("Glass_Utility");
            Physics.SyncTransforms();
            var root = new GameObject("Mission structures").transform;
            var director = systems.AddComponent<MissionDirector>();
            director.mission = mission; ui.director = director;
            var dust = mission.hud.environment.wheelDust;
            director.particleTemplate = dust ? dust.GetComponent<ParticleSystemRenderer>().sharedMaterial : null;

            // ---- research tents (crew camp), south-west of the domes ----
            var camp = Root(root, "Research tents (crew camp)", new Vector3(-58, 0, -86), 0);
            var tentMesh = TentMesh();
            foreach (var (pos, yaw, name) in new[] { (new Vector3(-4, 0, 2.5f), 15f, "Tent A"), (new Vector3(4.5f, 0, 3.5f), -12f, "Tent B"), (new Vector3(-1, 0, -5), 82f, "Tent C") }) Tent(camp, pos, yaw, name, tentMesh);
            Prim(PrimitiveType.Cube, camp, "Camp table", new Vector3(3.5f, .45f, -2.5f), new Vector3(2.2f, .1f, 1.1f), Metal, true);
            foreach (var x in new[] { 2.6f, 4.4f }) Prim(PrimitiveType.Cube, camp, "Table leg", new Vector3(x, .2f, -2.5f), new Vector3(.08f, .4f, .9f), Metal, false);
            for (int i = 0; i < 3; i++) Prim(PrimitiveType.Cube, camp, "Supply crate", new Vector3(-6.8f + (i == 2 ? .5f : i * 1.2f), i == 2 ? 1.2f : .4f, -3.5f), new Vector3(1.1f, .8f, .9f), i == 1 ? Orange : Dark, true);
            foreach (var lp in new[] { new Vector3(6.5f, 0, 0), new Vector3(-7, 0, 4) }) LampPost(camp, lp);
            var tentsAnchor = Anchor(camp, "Check-in point", new Vector3(6, 0, -2));
            Mark(camp, "fac.tents", "Research tents (crew camp)", StructureKind.Tents, "tent", "Walk here to check in", tentsAnchor, 6);
            director.tentsAnchor = tentsAnchor;

            // ---- rover charging station, east of the rover ----
            var chg = Root(root, "Rover charging station", new Vector3(5, 0, -96), 0);
            var rim = Prim(PrimitiveType.Cylinder, chg, "Pad rim", new Vector3(0, .03f, 0), new Vector3(6.6f, .03f, 6.6f), Metal, false);
            var pad = Prim(PrimitiveType.Cylinder, chg, "Charging pad", new Vector3(0, .07f, 0), new Vector3(6f, .04f, 6f), Dark, false);
            Prim(PrimitiveType.Cube, chg, "Pylon", new Vector3(4.3f, 1.3f, 0), new Vector3(.9f, 2.6f, .7f), OffWhite, true);
            Prim(PrimitiveType.Cube, chg, "Pylon light strip", new Vector3(3.84f, 1.5f, 0), new Vector3(.04f, 1.6f, .18f), Cyan, false);
            Prim(PrimitiveType.Cube, chg, "Pylon head", new Vector3(4.3f, 2.75f, 0), new Vector3(1.2f, .3f, .95f), Dark, false);
            Prim(PrimitiveType.Cube, chg, "Pylon screen", new Vector3(3.84f, 2.1f, 0), new Vector3(.03f, .45f, .55f), Glass, false);
            var cable = Prim(PrimitiveType.Cylinder, chg, "Charge cable", new Vector3(3.4f, .08f, .25f), new Vector3(.09f, .5f, .09f), Dark, false); cable.transform.localRotation = Quaternion.Euler(0, 0, 90);
            Prim(PrimitiveType.Cube, chg, "Battery cabinet", new Vector3(4.6f, .6f, 1.9f), new Vector3(1.3f, 1.2f, 1f), Dark, true);
            Prim(PrimitiveType.Cube, chg, "Cabinet stripe", new Vector3(4.6f, 1.05f, 1.39f), new Vector3(1.32f, .1f, .03f), Orange, false);
            var cs = chg.gameObject.AddComponent<ChargingPadState>(); cs.pad = pad.transform;
            Mark(chg, "fac.charging", "Rover charging station", StructureKind.ChargingStation, "bolt", "Drive onto the pad and stop to charge", pad.transform, 3);
            director.charging = cs;

            // ---- power generator (existing P_PowerStation prefab) near the solar farm ----
            var genPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(Env + "Prefabs/P_PowerStation.prefab") ?? throw new Exception("P_PowerStation missing");
            var gen = (GameObject)PrefabUtility.InstantiatePrefab(genPrefab, root);
            gen.name = "Power generator"; gen.transform.position = Ground(new Vector3(22, 0, -84)); gen.transform.rotation = Quaternion.identity;
            int meshCols = 0;
            foreach (var mf in gen.GetComponentsInChildren<MeshFilter>(true)) if (!mf.GetComponent<Collider>() && mf.sharedMesh) { var mc = mf.gameObject.AddComponent<MeshCollider>(); mc.sharedMesh = mf.sharedMesh; meshCols++; }
            var console = gen.transform.Find("ExternalServiceConsole") ?? throw new Exception("generator console missing");
            var gs = gen.AddComponent<GeneratorState>(); gs.console = console;
            var genAnchor = Anchor(gen.transform, "Fault panel approach", console.localPosition + new Vector3(-2.6f, -console.localPosition.y, 0));
            Mark(gen.transform, "fac.generator", "Power generator", StructureKind.Generator, "bolt", "REPAIR at the fault panel (on foot)", genAnchor, 4);
            director.generator = gs;
            notes.Add($"generator: P_PowerStation at {gen.transform.position}; console {console.position}; mesh colliders added {meshCols}");

            // ---- rover dock: covered two-bay shelter south-west of the rover's start ----
            var dockT = Root(root, "Rover dock", new Vector3(-22, 0, -108), 0);
            Prim(PrimitiveType.Cube, dockT, "Floor slab", new Vector3(0, .03f, 0), new Vector3(17.6f, .06f, 9.6f), Dark, false);
            // No collider (4 m clearance) and no shadow: the bay stays readable from the chase camera.
            Prim(PrimitiveType.Cube, dockT, "Canopy", new Vector3(0, 5.6f, -.3f), new Vector3(18.4f, .22f, 10.4f), OffWhite, false).GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            foreach (var z in new[] { 4.9f, -5.5f }) Prim(PrimitiveType.Cube, dockT, "Canopy fascia", new Vector3(0, 5.35f, z), new Vector3(18.4f, .5f, .25f), Dark, false);
            for (int i = 0; i < 5; i++) Prim(PrimitiveType.Cube, dockT, "Roof rib", new Vector3(-8 + i * 4, 5.75f, -.3f), new Vector3(.12f, .1f, 10.4f), Metal, false);
            foreach (var x in new[] { -8.8f, 8.8f }) foreach (var z in new[] { 4.6f, -5.2f }) Prim(PrimitiveType.Cube, dockT, "Support", new Vector3(x, 2.75f, z), new Vector3(.35f, 5.5f, .35f), Metal, true);
            Prim(PrimitiveType.Cube, dockT, "Back wall", new Vector3(0, 1.8f, -5.3f), new Vector3(17.2f, 3.6f, .2f), OffWhite, true);
            Prim(PrimitiveType.Cube, dockT, "Back wall stripe", new Vector3(0, 2.6f, -5.18f), new Vector3(17.2f, .14f, .03f), Orange, false);
            foreach (var bx in new[] { -4.3f, 4.3f })
            {
                foreach (var x in new[] { -1.9f, 1.9f }) Prim(PrimitiveType.Cube, dockT, "Bay line", new Vector3(bx + x, .07f, -.2f), new Vector3(.12f, .02f, 7.4f), Orange, false);
                Prim(PrimitiveType.Cube, dockT, "Bay stop line", new Vector3(bx, .07f, -3.8f), new Vector3(3.9f, .02f, .12f), Orange, false);
                var lamp = new GameObject("Bay light").AddComponent<Light>(); lamp.transform.SetParent(dockT, false); lamp.transform.localPosition = new Vector3(bx, 5.1f, -.5f);
                lamp.type = LightType.Point; lamp.range = 10; lamp.intensity = 1.6f; lamp.color = new Color(.75f, .9f, 1f); lamp.shadows = LightShadows.None;
                Prim(PrimitiveType.Cube, dockT, "Bay lamp", new Vector3(bx, 5.45f, -.5f), new Vector3(1.6f, .06f, .3f), Cyan, false);
            }
            for (int i = 0; i < 3; i++) Prim(PrimitiveType.Cube, dockT, "Spares crate", new Vector3(-7.6f + i * 1.15f, .5f, -4.4f), new Vector3(1f, 1f, .9f), i == 1 ? Orange : Metal, true);
            Prim(PrimitiveType.Cube, dockT, "Tool rack", new Vector3(7.4f, 1.1f, -4.7f), new Vector3(2.2f, 2.2f, .5f), Dark, true);
            var bay = Anchor(dockT, "Dock bay", new Vector3(4.3f, 0, -.5f));
            Mark(dockT, "fac.dock", "Rover dock", StructureKind.Dock, "rover", "Drive into the right bay and stop", bay, 4);
            director.dockBay = bay;
            notes.Add($"dock: built shelter at {dockT.position}; bay {bay.position}");

            // ---- checkpoint gates on the survey route ----
            var site = mission.Designated ? mission.Designated.transform.position : new Vector3(25, 0, -43);
            var pts = new[] { site, new Vector3(60, 0, -40), new Vector3(66, 0, -96), new Vector3(30, 0, -108), bay.position };
            for (int i = 1; i <= 3; i++)
            {
                var dir = (pts[i + 1] - pts[i - 1]); dir.y = 0; dir.Normalize();
                var yaw = Quaternion.LookRotation(dir).eulerAngles.y;
                var g = Root(root, "Mission checkpoint " + i, pts[i], yaw);
                Transform Post(float side)
                {
                    var p = new GameObject(side < 0 ? "Post left" : "Post right").transform; p.SetParent(g, false); p.localPosition = new Vector3(side * 4.5f, 0, 0);
                    Prim(PrimitiveType.Cube, p, "Footing", new Vector3(0, .1f, 0), new Vector3(1f, .2f, 1f), Metal, false);
                    Prim(PrimitiveType.Cylinder, p, "Post", new Vector3(0, 1.6f, 0), new Vector3(.36f, 1.6f, .36f), OffWhite, true);
                    Prim(PrimitiveType.Cylinder, p, "Band", new Vector3(0, 2.4f, 0), new Vector3(.38f, .12f, .38f), Orange, false);
                    Prim(PrimitiveType.Cylinder, p, "Cap", new Vector3(0, 3.25f, 0), new Vector3(.5f, .06f, .5f), Dark, false);
                    var lampAt = new GameObject("Lamp mount").transform; lampAt.SetParent(p, false); lampAt.localPosition = new Vector3(0, 3.0f, 0);
                    return lampAt;
                }
                var cp = g.gameObject.AddComponent<CheckpointState>(); cp.order = i; cp.postA = Post(-1); cp.postB = Post(1);
                Mark(g, "fac.checkpoint" + i, "Checkpoint " + i, StructureKind.Checkpoint, "gate", "Drive between the posts (in order)", g, 5.5f);
                director.checkpoints.Add(cp);
                notes.Add($"checkpoint {i} at {g.position} yaw {yaw:0}");
            }
            notes.Add($"mission structures: tents {camp.position}, charging pad {pad.transform.position}");
            return director;
        }

        // ---------- helpers ----------
        static Vector3 Ground(Vector3 p)
        {
            if (Physics.Raycast(new Vector3(p.x, 120, p.z), Vector3.down, out var h, 300, 1, QueryTriggerInteraction.Ignore)) p.y = h.point.y; else p.y = 0;
            return p;
        }
        static Transform Root(Transform parent, string name, Vector3 at, float yaw)
        {
            var t = new GameObject(name).transform; t.SetParent(parent, false); t.position = Ground(at); t.rotation = Quaternion.Euler(0, yaw, 0); return t;
        }
        static Transform Anchor(Transform parent, string name, Vector3 local)
        {
            var a = new GameObject(name).transform; a.SetParent(parent, false); a.localPosition = local; return a;
        }
        static void Mark(Transform t, string id, string name, StructureKind kind, string icon, string interaction, Transform anchor, float radius)
        {
            var m = t.gameObject.AddComponent<MissionStructure>(); m.id = id; m.displayName = name; m.kind = kind; m.icon = icon; m.interaction = interaction; m.anchor = anchor; m.arrivalRadius = radius;
        }
        static GameObject Prim(PrimitiveType type, Transform parent, string name, Vector3 local, Vector3 scale, Material mat, bool collider)
        {
            var go = GameObject.CreatePrimitive(type); go.name = name;
            go.transform.SetParent(parent, false); go.transform.localPosition = local; go.transform.localScale = scale;
            go.GetComponent<Renderer>().sharedMaterial = mat;
            if (!collider) UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());
            return go;
        }
        static void LampPost(Transform parent, Vector3 local)
        {
            var p = new GameObject("Lamp post").transform; p.SetParent(parent, false); p.localPosition = local;
            Prim(PrimitiveType.Cylinder, p, "Pole", new Vector3(0, 1.3f, 0), new Vector3(.12f, 1.3f, .12f), Metal, true);
            Prim(PrimitiveType.Cube, p, "Lamp head", new Vector3(0, 2.65f, .2f), new Vector3(.3f, .14f, .6f), Dark, false);
            Prim(PrimitiveType.Cube, p, "Lamp lens", new Vector3(0, 2.57f, .2f), new Vector3(.24f, .03f, .5f), Cyan, false);
            var l = new GameObject("Lamp light").AddComponent<Light>(); l.transform.SetParent(p, false); l.transform.localPosition = new Vector3(0, 2.4f, .3f);
            l.type = LightType.Point; l.range = 7; l.intensity = .8f; l.color = new Color(.55f, .85f, 1f); l.shadows = LightShadows.None;
        }
        static void Tent(Transform parent, Vector3 local, float yaw, string name, Mesh mesh)
        {
            var t = new GameObject(name).transform; t.SetParent(parent, false); t.localPosition = local; t.localRotation = Quaternion.Euler(0, yaw, 0);
            var body = new GameObject("Shell", typeof(MeshFilter), typeof(MeshRenderer)); body.transform.SetParent(t, false);
            body.GetComponent<MeshFilter>().sharedMesh = mesh; body.GetComponent<MeshRenderer>().sharedMaterial = OffWhite;
            Prim(PrimitiveType.Cube, t, "Ridge stripe", new Vector3(0, 2.32f, 0), new Vector3(.18f, .06f, 4.3f), Orange, false);
            var door = Prim(PrimitiveType.Cube, t, "Door", new Vector3(0, .7f, 2.12f), new Vector3(1f, 1.4f, .03f), Dark, false);
            Prim(PrimitiveType.Cube, t, "Ground sheet", new Vector3(0, .02f, 0), new Vector3(3.6f, .03f, 4.8f), Metal, false);
            foreach (var z in new[] { -2.2f, 2.2f }) foreach (var x in new[] { -1.9f, 1.9f }) Prim(PrimitiveType.Cylinder, t, "Peg", new Vector3(x, .15f, z), new Vector3(.06f, .15f, .06f), Orange, false);
            var c = t.gameObject.AddComponent<BoxCollider>(); c.center = new Vector3(0, .9f, 0); c.size = new Vector3(2.9f, 1.8f, 4.1f);
        }

        // A-frame tent shell (3.2 m wide, 2.3 m high, 4.2 m long), flat-shaded, saved as an asset.
        static Mesh TentMesh()
        {
            const string path = FirstMissionBuilder.F + "/Meshes/MissionTent.asset";
            var existing = AssetDatabase.LoadAssetAtPath<Mesh>(path); if (existing) AssetDatabase.DeleteAsset(path);
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            float w = 1.6f, h = 2.3f, l = 2.1f;
            var v = new List<Vector3>(); var tris = new List<int>(); var uv = new List<Vector2>();
            void Quad(Vector3 a, Vector3 b, Vector3 c, Vector3 d) { int i = v.Count; v.AddRange(new[] { a, b, c, d }); uv.AddRange(new[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) }); tris.AddRange(new[] { i, i + 2, i + 1, i, i + 3, i + 2 }); }
            void Tri(Vector3 a, Vector3 b, Vector3 c) { int i = v.Count; v.AddRange(new[] { a, b, c }); uv.AddRange(new[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(.5f, 1) }); tris.AddRange(new[] { i, i + 1, i + 2 }); }
            var top0 = new Vector3(0, h, -l); var top1 = new Vector3(0, h, l);
            Quad(new Vector3(-w, 0, -l), new Vector3(-w, 0, l), top1, top0);           // left slope
            Quad(new Vector3(w, 0, l), new Vector3(w, 0, -l), top0, top1);             // right slope
            Tri(new Vector3(-w, 0, l), new Vector3(w, 0, l), top1);                    // front
            Tri(new Vector3(w, 0, -l), new Vector3(-w, 0, -l), top0);                  // back
            var m = new Mesh { name = "MissionTent" }; m.SetVertices(v); m.SetTriangles(tris, 0); m.SetUVs(0, uv);
            // flip winding where needed so normals face outwards
            m.RecalculateNormals();
            var n = m.normals; var t = m.triangles;
            for (int i = 0; i < t.Length; i += 3)
            {
                var c = (v[t[i]] + v[t[i + 1]] + v[t[i + 2]]) / 3; var fn = Vector3.Cross(v[t[i + 1]] - v[t[i]], v[t[i + 2]] - v[t[i]]);
                if (Vector3.Dot(fn, c - new Vector3(0, h * .35f, 0)) < 0) { int tmp = t[i + 1]; t[i + 1] = t[i + 2]; t[i + 2] = tmp; }
            }
            m.triangles = t; m.RecalculateNormals(); m.RecalculateBounds(); m.RecalculateTangents();
            AssetDatabase.CreateAsset(m, path);
            return m;
        }
    }
}
