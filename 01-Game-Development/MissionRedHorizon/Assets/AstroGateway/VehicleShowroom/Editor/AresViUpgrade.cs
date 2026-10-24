using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace AstroGateway.VehicleShowroom.EditorTools
{
    // ARES-VI upgraded showroom copy. Reads the original OBJ (never modified) and writes a repaired, re-pivoted,
    // animated prefab under Assets/AstroGateway/VehicleShowroom/ARES_VI/Upgraded, then adds it to the showroom
    // scene next to the untouched original import. Batch-safe:
    //   Unity -batchmode -quit -projectPath <p> -executeMethod AstroGateway.VehicleShowroom.EditorTools.AresViUpgrade.Build
    // Rover space: metres, +Y up, +Z forward, -X left, wheels on y = 0.
    public static class AresViUpgrade
    {
        public const string Dir = AresViShowroom.Root + "/ARES_VI/Upgraded";
        const string TexDir = Dir + "/Textures", MatDir = Dir + "/Materials", AnimDir = Dir + "/Animation";
        public const string PrefabPath = Dir + "/ARES_VI_Upgraded.prefab";
        const string MeshAsset = Dir + "/ARES_VI_Upgraded_Meshes.asset";
        public const string ControllerPath = AnimDir + "/ARES_VI_Upgraded.controller";
        public static readonly string[] ClipNames = { "ARES_WheelRotation", "ARES_RockerBogie", "ARES_ArmDeploy", "ARES_ChemCamScan", "ARES_HoverActivation", "ARES_HoverStabilization", "ARES_Landing" };
        static string Out => Path.GetFullPath("Docs/VehicleShowroom/ARES_VI/Upgrade");
        static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
        static StringBuilder report;

        // Moves that remove the source clashes: the mast column ran through the canopy glass and the arm shoulder
        // sat inside the canopy footprint. The mast goes to the rear-right deck, the arm forward onto a bracket.
        static readonly Vector3 MastShift = new Vector3(0, 0, -1.20f), ArmShift = new Vector3(0, 0, .25f);

        // ------------------------------------------------------------------ source
        class Part { public string name, mat; public List<Vector3> tris = new List<Vector3>(); public bool generated; }

        static List<Part> LoadObj(string path)
        {
            var v = new List<Vector3>(); var parts = new List<Part>(); Part cur = null; string mat = null;
            foreach (var raw in File.ReadLines(path))
            {
                var p = raw.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                if (p.Length == 0) continue;
                switch (p[0])
                {
                    case "v": // OBJ is Z-up with the nose on +Y; swap to Unity's Y-up, nose on +Z (a reflection, so handedness converts too)
                        v.Add(new Vector3(F(p[1]), F(p[3]), F(p[2]))); break;
                    case "g": cur = new Part { name = p.Length > 1 ? p[1] : "unnamed", mat = mat }; parts.Add(cur); break;
                    case "usemtl": mat = p[1]; if (cur != null) cur.mat = mat; break;
                    case "f":
                        var idx = p.Skip(1).Select(t => { int i = int.Parse(t.Split('/')[0], Inv); return i > 0 ? i - 1 : v.Count + i; }).ToArray();
                        for (int k = 1; k + 1 < idx.Length; k++) { cur.tris.Add(v[idx[0]]); cur.tris.Add(v[idx[k]]); cur.tris.Add(v[idx[k + 1]]); }
                        break;
                }
            }
            return parts;
        }
        static float F(string s) => float.Parse(s, Inv);

        static Bounds TB(List<Vector3> t) { var b = new Bounds(t[0], Vector3.zero); foreach (var p in t) b.Encapsulate(p); return b; }

        // ------------------------------------------------------------------ generators (winding is normalised later)
        static void Quad(List<Vector3> t, Vector3 a, Vector3 b, Vector3 c, Vector3 d) { t.Add(a); t.Add(b); t.Add(c); t.Add(a); t.Add(c); t.Add(d); }

        static List<Vector3> Box(Vector3 c, Vector3 ax, Vector3 ay, Vector3 az, Vector3 h)
        {
            Vector3 P(int i, int j, int k) => c + ax * h.x * i + ay * h.y * j + az * h.z * k;
            var t = new List<Vector3>();
            Quad(t, P(-1, -1, -1), P(1, -1, -1), P(1, 1, -1), P(-1, 1, -1));
            Quad(t, P(-1, -1, 1), P(-1, 1, 1), P(1, 1, 1), P(1, -1, 1));
            Quad(t, P(-1, -1, -1), P(-1, 1, -1), P(-1, 1, 1), P(-1, -1, 1));
            Quad(t, P(1, -1, -1), P(1, -1, 1), P(1, 1, 1), P(1, 1, -1));
            Quad(t, P(-1, -1, -1), P(-1, -1, 1), P(1, -1, 1), P(1, -1, -1));
            Quad(t, P(-1, 1, -1), P(1, 1, -1), P(1, 1, 1), P(-1, 1, 1));
            return t;
        }
        static List<Vector3> AABox(Vector3 c, Vector3 size) => Box(c, Vector3.right, Vector3.up, Vector3.forward, size / 2);

        static List<Vector3> Beam(Vector3 a, Vector3 b, float w, float h)
        {
            var az = (b - a).normalized; var ax = Vector3.ProjectOnPlane(Vector3.right, az).normalized; var ay = Vector3.Cross(az, ax);
            return Box((a + b) / 2, ax, ay, az, new Vector3(w / 2, h / 2, (b - a).magnitude / 2));
        }

        static void Basis(Vector3 axis, out Vector3 u, out Vector3 w)
        {
            u = Vector3.Cross(axis, Mathf.Abs(axis.y) < .9f ? Vector3.up : Vector3.right).normalized; w = Vector3.Cross(axis, u);
        }

        static List<Vector3> Cone(Vector3 a, Vector3 b, float ra, float rb, int seg, bool caps = true)
        {
            var t = new List<Vector3>(); var axis = (b - a).normalized; Basis(axis, out var u, out var w);
            for (int i = 0; i < seg; i++)
            {
                float a0 = 2 * Mathf.PI * i / seg, a1 = 2 * Mathf.PI * (i + 1) / seg;
                Vector3 d0 = u * Mathf.Cos(a0) + w * Mathf.Sin(a0), d1 = u * Mathf.Cos(a1) + w * Mathf.Sin(a1);
                Quad(t, a + d0 * ra, b + d0 * rb, b + d1 * rb, a + d1 * ra);
                if (caps) { t.Add(a); t.Add(a + d1 * ra); t.Add(a + d0 * ra); t.Add(b); t.Add(b + d0 * rb); t.Add(b + d1 * rb); }
            }
            return t;
        }
        static List<Vector3> Cyl(Vector3 a, Vector3 b, float r, int seg = 20) => Cone(a, b, r, r, seg);

        // Lathed tyre about the X axis with a rounded shoulder, plus chevron lugs fused into the tread.
        static List<Vector3> Tyre(Vector3 c, float rOut, float rIn, float width, int seg, int lugs)
        {
            float hw = width / 2;
            var prof = new[] {
                new Vector2(rIn, -hw), new Vector2(rOut - .045f, -hw), new Vector2(rOut - .014f, -hw + .012f), new Vector2(rOut, -hw + .04f),
                new Vector2(rOut, hw - .04f), new Vector2(rOut - .014f, hw - .012f), new Vector2(rOut - .045f, hw), new Vector2(rIn, hw) };
            var t = new List<Vector3>();
            Vector3 P(Vector2 pr, float ang) => c + new Vector3(pr.y, pr.x * Mathf.Cos(ang), pr.x * Mathf.Sin(ang));
            for (int i = 0; i < seg; i++)
            {
                float a0 = 2 * Mathf.PI * i / seg, a1 = 2 * Mathf.PI * (i + 1) / seg;
                for (int k = 0; k < prof.Length; k++) { var p0 = prof[k]; var p1 = prof[(k + 1) % prof.Length]; Quad(t, P(p0, a0), P(p1, a0), P(p1, a1), P(p0, a1)); }
            }
            for (int i = 0; i < lugs; i++)
            {
                float ang = 2 * Mathf.PI * i / lugs;
                foreach (int s in new[] { -1, 1 })
                {
                    float a = ang + s * .045f;
                    var radial = new Vector3(0, Mathf.Cos(a), Mathf.Sin(a)); var tang = new Vector3(0, -Mathf.Sin(a), Mathf.Cos(a));
                    float tilt = s * 24f * Mathf.Deg2Rad;
                    var lat = Vector3.right * Mathf.Cos(tilt) + tang * Mathf.Sin(tilt); var tan2 = Vector3.Cross(radial, lat).normalized;
                    t.AddRange(Box(c + Vector3.right * s * .062f + radial * (rOut + .009f), lat, radial, tan2, new Vector3(.066f, .014f, .017f)));
                }
            }
            return t;
        }

        // ------------------------------------------------------------------ repair
        struct Corner { public Vector3 p, n; public Vector2 uv; }
        static int flippedTotal, componentsFlipped, openComponents, backfacesAdded, trisIn;

        static List<Corner> Repair(List<Vector3> src, float smoothDeg = 40f)
        {
            var tris = new List<Vector3>(src.Count);
            for (int i = 0; i < src.Count; i += 3)
                if (Vector3.Cross(src[i + 1] - src[i], src[i + 2] - src[i]).sqrMagnitude > 1e-14f) { tris.Add(src[i]); tris.Add(src[i + 1]); tris.Add(src[i + 2]); }
            int n = tris.Count / 3; trisIn += n;
            var ids = Weld(tris);
            var edges = new Dictionary<long, List<int>>();
            for (int f = 0; f < n; f++) for (int k = 0; k < 3; k++)
                { long key = EKey(ids[3 * f + k], ids[3 * f + (k + 1) % 3]); if (!edges.TryGetValue(key, out var l)) edges[key] = l = new List<int>(2); l.Add(f); }
            bool Forward(int f, int a, int b) { for (int k = 0; k < 3; k++) if (ids[3 * f + k] == a && ids[3 * f + (k + 1) % 3] == b) return true; return false; }
            var flip = new bool[n]; var comp = Enumerable.Repeat(-1, n).ToArray(); var compOpen = new List<bool>(); int nc = 0;
            for (int s = 0; s < n; s++)
            {
                if (comp[s] >= 0) continue;
                bool open = false; var q = new Queue<int>(); q.Enqueue(s); comp[s] = nc;
                while (q.Count > 0)
                {
                    int f = q.Dequeue();
                    for (int k = 0; k < 3; k++)
                    {
                        int a = ids[3 * f + k], b = ids[3 * f + (k + 1) % 3];
                        var l = edges[EKey(a, b)];
                        if (l.Count != 2) { open = true; continue; }
                        int g = l[0] == f ? l[1] : l[0];
                        if (comp[g] >= 0) continue;
                        // f traverses a->b in raw order; g's effective direction must oppose f's effective direction
                        bool gRawA2B = Forward(g, a, b);
                        flip[g] = flip[f] ? !gRawA2B : gRawA2B;
                        comp[g] = nc; q.Enqueue(g);
                    }
                }
                compOpen.Add(open); nc++;
            }
            var vol = new double[nc];
            for (int f = 0; f < n; f++)
            {
                Vector3 a = tris[3 * f], b = tris[3 * f + 1], c = tris[3 * f + 2]; if (flip[f]) { var tmp = b; b = c; c = tmp; }
                vol[comp[f]] += Vector3.Dot(a, Vector3.Cross(b, c));
            }
            var outT = new List<Vector3>(tris.Count);
            for (int f = 0; f < n; f++)
            {
                bool fl = flip[f] ^ (vol[comp[f]] < 0);
                Vector3 a = tris[3 * f], b = tris[3 * f + 1], c = tris[3 * f + 2];
                if (fl) { flippedTotal++; outT.Add(a); outT.Add(c); outT.Add(b); } else { outT.Add(a); outT.Add(b); outT.Add(c); }
            }
            for (int c = 0; c < nc; c++) { if (vol[c] < 0) componentsFlipped++; if (compOpen[c]) openComponents++; }
            int baseCount = outT.Count / 3;
            for (int f = 0; f < baseCount; f++)
                if (compOpen[comp[f]]) { outT.Add(outT[3 * f]); outT.Add(outT[3 * f + 2]); outT.Add(outT[3 * f + 1]); backfacesAdded++; }

            // Smoothed normals with a hard-edge angle, box-projected UVs in metres (texture tile = 2 m).
            int m = outT.Count / 3; var fn = new Vector3[m]; var fu = new Vector3[m];
            for (int f = 0; f < m; f++) { fn[f] = Vector3.Cross(outT[3 * f + 1] - outT[3 * f], outT[3 * f + 2] - outT[3 * f]); fu[f] = fn[f].normalized; }
            var wid = Weld(outT); var inc = new Dictionary<int, List<int>>();
            for (int i = 0; i < wid.Length; i++) { if (!inc.TryGetValue(wid[i], out var l)) inc[wid[i]] = l = new List<int>(); l.Add(i / 3); }
            float cos = Mathf.Cos(smoothDeg * Mathf.Deg2Rad);
            var res = new List<Corner>(outT.Count);
            for (int f = 0; f < m; f++)
            {
                var nn = fu[f]; var an = new Vector3(Mathf.Abs(nn.x), Mathf.Abs(nn.y), Mathf.Abs(nn.z));
                for (int k = 0; k < 3; k++)
                {
                    var sum = Vector3.zero;
                    foreach (int g in inc[wid[3 * f + k]]) if (Vector3.Dot(fu[g], fu[f]) > cos) sum += fn[g];
                    var p = outT[3 * f + k];
                    Vector2 uv = an.x >= an.y && an.x >= an.z ? new Vector2(p.z, p.y) : an.y >= an.z ? new Vector2(p.x, p.z) : new Vector2(p.x, p.y);
                    res.Add(new Corner { p = p, n = sum.sqrMagnitude > 0 ? sum.normalized : fu[f], uv = uv / 2f });
                }
            }
            return res;
        }

        static int[] Weld(List<Vector3> pts)
        {
            var map = new Dictionary<Vector3Int, int>(); var ids = new int[pts.Count];
            for (int i = 0; i < pts.Count; i++)
            {
                var key = Vector3Int.RoundToInt(pts[i] * 100000f);
                if (!map.TryGetValue(key, out int id)) { id = map.Count; map[key] = id; }
                ids[i] = id;
            }
            return ids;
        }
        static long EKey(int a, int b) => a < b ? (long)a * 10000000L + b : (long)b * 10000000L + a;

        // ------------------------------------------------------------------ groups
        class Group { public string name; public Group parent; public Vector3 pivot; public List<(string mat, List<Vector3> tris)> parts = new List<(string, List<Vector3>)>(); public GameObject go; public bool shadows = true; public Vector3 restScale = Vector3.one; }
        static readonly List<Group> groups = new List<Group>();
        static Group G(string name, Group parent, Vector3 pivot) { var g = new Group { name = name, parent = parent, pivot = pivot }; groups.Add(g); return g; }

        static readonly Dictionary<string, string> MatMap = new Dictionary<string, string> {
            { "Mat_CeramicWhite", "PaintWhite" }, { "Mat_MartianOrange", "PaintOrange" }, { "Mat_GraphiteTrim", "Graphite" }, { "Mat_TitaniumFrame", "Titanium" },
            { "Mat_GoldFoil", "GoldFoil" }, { "Mat_CarbonTreads", "Rubber" }, { "Mat_CanopyGlass", "Glass" }, { "Mat_CyanEnergy", "CyanEmissive" } };

        public static void Build()
        {
            groups.Clear(); flippedTotal = componentsFlipped = openComponents = backfacesAdded = trisIn = 0;
            report = new StringBuilder();
            Directory.CreateDirectory(Out);
            foreach (var d in new[] { Dir, TexDir, MatDir, AnimDir }) Directory.CreateDirectory(d);
            report.AppendLine("ARES-VI upgraded build (Unity " + Application.unityVersion + ", " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ")");

            var src = LoadObj(AresViShowroom.Src + "/ares_vi_aaa_master.obj");
            var byName = src.ToDictionary(p => p.name);
            report.AppendLine($"source parts {src.Count}, source triangles {src.Sum(p => p.tris.Count / 3)}");
            foreach (var p in src)
            {
                if (p.name.StartsWith("Mast_")) for (int i = 0; i < p.tris.Count; i++) p.tris[i] += MastShift;
                if (p.name.StartsWith("Arm_")) for (int i = 0; i < p.tris.Count; i++) p.tris[i] += ArmShift;
            }
            Vector3 C(string n) => TB(byName[n].tris).center;

            // Static clash check after the moves (axis-aligned bounds).
            var canopy = TB(byName["Cockpit_CanopyGlass"].tris);
            foreach (var pre in new[] { "Mast_", "Arm_" })
            {
                var hits = src.Where(p => p.name.StartsWith(pre) && TB(p.tris).Intersects(canopy)).Select(p => p.name).ToList();
                report.AppendLine($"{pre}* parts overlapping canopy glass bounds after move: {hits.Count}" + (hits.Count > 0 ? " " + string.Join(",", hits) : ""));
            }

            var used = new HashSet<string>();
            void Take(Group g, string n) { var p = byName[n]; g.parts.Add((MatMap[p.mat], p.tris)); used.Add(n); }
            void Gen(Group g, string mat, List<Vector3> t) => g.parts.Add((mat, t));

            var root = G("ARES_VI_Upgraded", null, Vector3.zero);
            var chassis = G("Chassis", root, Vector3.zero);
            var body = G("Body", chassis, Vector3.zero);

            // Rocker-bogie: new linkage geometry (the source had straight bars at axle height that could not articulate).
            string[] rockerDrop = { "RockerArm", "RockerCutout", "BogieBridge", "RearArm", "ShockF_Body", "ShockF_Shaft", "ShockF_Spring", "ShockR_Body", "ShockR_Shaft", "ShockR_Spring", "TieRodF", "TieRodR" };
            foreach (var side in new[] { "Left", "Right" }) foreach (var d in rockerDrop) used.Add($"Suspension_{side}_{d}");
            foreach (var side in new[] { ("L", "Left", -1f), ("R", "Right", 1f) })
            {
                float sx = side.Item3, bx = sx * 1.03f;
                var pr = new Vector3(sx * .74f, .76f, 0); var pb = new Vector3(bx, .60f, -.62f);
                var rocker = G("Rocker_" + side.Item1, chassis, pr);
                Gen(rocker, "Graphite", Cyl(new Vector3(sx * .83f, .76f, 0), new Vector3(sx * 1.08f, .76f, 0), .05f));
                Gen(rocker, "Titanium", Cyl(new Vector3(sx * .99f, .76f, 0), new Vector3(sx * 1.075f, .76f, 0), .072f, 24));
                var knee = new Vector3(bx, .70f, .72f); var front = new Vector3(bx, .38f, 1.18f);
                Gen(rocker, "Titanium", Beam(new Vector3(bx, .76f, 0), knee, .06f, .09f));
                Gen(rocker, "Titanium", Beam(knee, front, .06f, .085f));
                Gen(rocker, "Titanium", Beam(new Vector3(bx, .76f, 0), pb, .06f, .085f));
                Gen(rocker, "Graphite", Cyl(knee + Vector3.right * -.04f * sx, knee + Vector3.right * .04f * sx, .052f));
                Gen(rocker, "PaintOrange", Beam(new Vector3(bx, .79f, .05f), knee + new Vector3(0, .035f, -.05f), .064f, .012f)); // safety stripe
                var bogie = G("Bogie_" + side.Item1, rocker, pb);
                Gen(bogie, "Graphite", Cyl(pb - Vector3.right * .05f * sx, pb + Vector3.right * .05f * sx, .058f));
                Gen(bogie, "Titanium", Beam(pb, new Vector3(bx, .38f, 0), .055f, .08f));
                Gen(bogie, "Titanium", Beam(pb, new Vector3(bx, .38f, -1.18f), .055f, .08f));
                foreach (var w in new[] { ("F", 1.18f, rocker), ("M", 0f, bogie), ("R", -1.18f, bogie) })
                {
                    string wn = $"Wheel_{w.Item1}{side.Item1}";
                    var hub = new Vector3(sx * 1.26f, .38f, w.Item2);
                    var carrier = w.Item3;
                    // Knuckle (non-rotating) and steering actuator on corner wheels.
                    Gen(carrier, "Titanium", AABox(new Vector3(sx * 1.075f, .38f, w.Item2), new Vector3(.09f, .1f, .1f)));
                    if (w.Item1 != "M") Gen(carrier, "Graphite", Cyl(new Vector3(bx, .30f, w.Item2), new Vector3(bx, .52f, w.Item2), .045f));
                    Take(carrier, wn + "_BrakeCaliper");
                    var wheel = G(wn, carrier, hub);
                    foreach (var piece in new[] { "BeadlockRing", "RimBolts", "RimDish", "HubMotor", "BrakeRotor", "CenterNut" }) Take(wheel, wn + "_" + piece);
                    Gen(wheel, "Rubber", Tyre(hub, .355f, .255f, .27f, 64, 22));
                    used.Add(wn + "_TireBody"); used.Add(wn + "_LugTreads");
                }
            }

            // Science arm: yaw -> shoulder pitch -> elbow -> wrist -> drill spin.
            var armYaw = G("Arm_Yaw", chassis, C("Arm_ShoulderBase"));
            Take(armYaw, "Arm_ShoulderBase");
            var armUpper = G("Arm_Upper", armYaw, C("Arm_ShoulderBase"));
            foreach (var n in new[] { "Arm_UpperBoom", "Arm_LiftCylinder", "Arm_LiftPiston" }) Take(armUpper, n);
            var armFore = G("Arm_Fore", armUpper, C("Arm_ElbowActuator"));
            foreach (var n in new[] { "Arm_ElbowActuator", "Arm_ForearmBoom", "Arm_CableClamp1", "Arm_CableClamp2" }) Take(armFore, n);
            var armWrist = G("Arm_Wrist", armFore, C("Arm_WristRoll"));
            foreach (var n in new[] { "Arm_WristRoll", "Arm_WristPitch", "Arm_DrillHousing" }) Take(armWrist, n);
            var drill = G("Arm_Drill", armWrist, C("Arm_DrillBit_Core"));
            foreach (var n in new[] { "Arm_DrillBit_Core", "Arm_DrillTeeth" }) Take(drill, n);
            var sb = TB(byName["Arm_ShoulderBase"].tris);
            Gen(body, "Titanium", AABox(new Vector3(sb.center.x, sb.center.y - .03f, .93f), new Vector3(.22f, .15f, .16f))); // mounting bracket to the hull face

            // Mast: azimuth pan -> elevation tilt.
            var mastPan = G("Mast_Pan", chassis, C("Mast_AzimuthTurret"));
            foreach (var n in new[] { "Mast_AzimuthTurret", "Mast_AzimuthBolts", "Mast_Stage1_Col", "Mast_CollarClamp", "Mast_Stage2_Col", "Mast_CableHarness" }) Take(mastPan, n);
            var mastTilt = G("Mast_Tilt", mastPan, C("Mast_ElevationGimbal"));
            foreach (var n in new[] { "Mast_ElevationGimbal", "Mast_ElevationServo", "Mast_ChemCamHead", "Mast_LaserAperture", "Mast_LaserLens", "Mast_NavCam_L_Hood", "Mast_NavCam_L_Lens", "Mast_NavCam_R_Hood", "Mast_NavCam_R_Lens" }) Take(mastTilt, n);

            // Hover modules: nacelle (tilts), vane ring (turns), plasma plume (scales).
            foreach (var h in new[] { "FL", "FR", "RL", "RR" })
            {
                var nac = TB(byName[$"Hover_{h}_Nacelle"].tris);
                var mod = G("Hover_" + h, chassis, nac.center);
                Take(mod, $"Hover_{h}_Nacelle"); Take(mod, $"Hover_{h}_StatorPoles"); Take(mod, $"Hover_{h}_PlasmaNozzle");
                mod.parts.Add(("HoverPlasma", byName[$"Hover_{h}_StatorCoil"].tris)); used.Add($"Hover_{h}_StatorCoil");
                var vaneNames = Enumerable.Range(0, 8).Select(i => $"Hover_{h}_VectorVane_{i}").ToList();
                var vb = TB(vaneNames.SelectMany(n => byName[n].tris).ToList());
                var vanes = G($"Hover_{h}_Vanes", mod, vb.center);
                foreach (var n in vaneNames) Take(vanes, n);
                var noz = TB(byName[$"Hover_{h}_PlasmaNozzle"].tris);
                var top = new Vector3(noz.center.x, noz.min.y, noz.center.z);
                var plume = G($"Hover_{h}_Plume", mod, top); plume.shadows = false; plume.restScale = new Vector3(.4f, .001f, .4f);
                Gen(plume, "Plume", Cone(top, top + Vector3.down * .30f, .07f, .15f, 24));
            }

            // Static detail: LED bars on the bumper and a differential bar on the deck.
            var bumper = TB(byName["FrontBumper_MainBar"].tris);
            foreach (float s in new[] { -1f, 1f })
                Gen(body, "LED", AABox(new Vector3(s * .42f, bumper.max.y + .016f, bumper.center.z), new Vector3(.30f, .03f, .05f)));
            Gen(body, "Titanium", Cyl(new Vector3(-.62f, 1.245f, -.2f), new Vector3(.62f, 1.245f, -.2f), .028f, 16));
            Gen(body, "Graphite", AABox(new Vector3(0, 1.245f, -.2f), new Vector3(.12f, .08f, .1f)));

            foreach (var p in src) if (!used.Contains(p.name)) Take(body, p.name);
            report.AppendLine($"static body source parts {body.parts.Count(x => true)} (incl. generated detail); replaced source parts: rocker/bogie bars, shocks, tie rods ({rockerDrop.Length * 2}), 6 tyre bodies, 6 lug rings");

            // Materials and textures.
            var mats = MakeMaterials();

            // Meshes and hierarchy.
            AssetDatabase.DeleteAsset(MeshAsset);
            var meshes = new List<Mesh>();
            foreach (var g in groups)
            {
                g.go = new GameObject(g.name);
                if (g.parent != null) { g.go.transform.SetParent(g.parent.go.transform, false); g.go.transform.localPosition = g.pivot - g.parent.pivot; g.go.transform.localScale = g.restScale; }
                if (g.parts.Count == 0) continue;
                var bySub = new Dictionary<string, List<Corner>>();
                foreach (var (mat, tris) in g.parts) { if (!bySub.TryGetValue(mat, out var l)) bySub[mat] = l = new List<Corner>(); l.AddRange(Repair(tris)); }
                var order = bySub.Keys.OrderBy(k => k).ToList();
                var mesh = new Mesh { name = "ARES_" + g.name };
                var verts = new List<Vector3>(); var norms = new List<Vector3>(); var uvs = new List<Vector2>();
                foreach (var k in order) foreach (var c in bySub[k]) { verts.Add(c.p - g.pivot); norms.Add(c.n); uvs.Add(c.uv); }
                if (verts.Count > 65000) mesh.indexFormat = IndexFormat.UInt32;
                mesh.SetVertices(verts); mesh.SetNormals(norms); mesh.SetUVs(0, uvs);
                mesh.subMeshCount = order.Count; int start = 0;
                for (int s = 0; s < order.Count; s++) { int cnt = bySub[order[s]].Count; mesh.SetTriangles(Enumerable.Range(start, cnt).ToArray(), s); start += cnt; }
                mesh.RecalculateTangents(); mesh.RecalculateBounds(); mesh.UploadMeshData(false);
                meshes.Add(mesh);
                g.go.AddComponent<MeshFilter>().sharedMesh = mesh;
                var mr = g.go.AddComponent<MeshRenderer>(); mr.sharedMaterials = order.Select(k => mats[k]).ToArray();
                mr.shadowCastingMode = g.shadows ? ShadowCastingMode.On : ShadowCastingMode.Off;
            }
            AssetDatabase.CreateAsset(meshes[0], MeshAsset);
            foreach (var m in meshes.Skip(1)) AssetDatabase.AddObjectToAsset(m, MeshAsset);

            report.AppendLine($"repair: input triangles {trisIn}, triangles re-wound {flippedTotal}, inside-out components flipped {componentsFlipped}, open components {openComponents} (back faces added {backfacesAdded}), smoothing angle 40 deg, box-projected UVs (2 m tile)");
            report.AppendLine($"output: {meshes.Count} meshes/renderers, {meshes.Sum(m => m.subMeshCount)} submeshes, {meshes.Sum(m => m.triangles.Length / 3)} triangles, {meshes.Sum(m => m.vertexCount)} vertices");
            foreach (var g in groups) report.AppendLine($"  {GPath(g)} pivot {V(g.pivot)}" + (g.go.GetComponent<MeshFilter>() ? $" tris {g.go.GetComponent<MeshFilter>().sharedMesh.triangles.Length / 3} submeshes {g.go.GetComponent<MeshFilter>().sharedMesh.subMeshCount}" : ""));

            var rootGo = groups[0].go;
            var animator = rootGo.AddComponent<Animator>();
            animator.runtimeAnimatorController = MakeAnimation(rootGo);
            animator.applyRootMotion = false;
            AssetDatabase.DeleteAsset(PrefabPath);
            PrefabUtility.SaveAsPrefabAsset(rootGo, PrefabPath);
            UnityEngine.Object.DestroyImmediate(rootGo);
            AssetDatabase.SaveAssets();

            UpdateScene();
            File.WriteAllText(Path.Combine(Out, "Upgrade_Build_Report.txt"), report.ToString());
            Debug.Log("[ARES-VI] Upgrade build finished");
        }

        static string GPath(Group g) => g.parent == null ? g.name : GPath(g.parent) + "/" + g.name;
        static string V(Vector3 v) => $"({v.x.ToString("F3", Inv)}, {v.y.ToString("F3", Inv)}, {v.z.ToString("F3", Inv)})";

        // ------------------------------------------------------------------ textures and materials
        enum Kind { Paint, Graphite, Titanium, Gold, Rubber, Regolith }
        const int TexSize = 1024;
        static readonly Color Dust = new Color(.60f, .40f, .27f);

        static float Hash(int x, int y, int s) { unchecked { uint h = (uint)x * 374761393u + (uint)y * 668265263u + (uint)s * 2246822519u; h = (h ^ (h >> 13)) * 1274126177u; h ^= h >> 16; return (h & 0xFFFFFF) / 16777215f; } }
        static float Value(float x, float y, int px, int py, int s)
        {
            int x0 = Mathf.FloorToInt(x), y0 = Mathf.FloorToInt(y); float fx = x - x0, fy = y - y0;
            fx = fx * fx * (3 - 2 * fx); fy = fy * fy * (3 - 2 * fy);
            int xa = ((x0 % px) + px) % px, xb = (xa + 1) % px, ya = ((y0 % py) + py) % py, yb = (ya + 1) % py;
            return Mathf.Lerp(Mathf.Lerp(Hash(xa, ya, s), Hash(xb, ya, s), fx), Mathf.Lerp(Hash(xa, yb, s), Hash(xb, yb, s), fx), fy);
        }
        static float Fbm(float u, float v, int fx, int fy, int oct, int s)
        {
            float sum = 0, amp = .5f, norm = 0;
            for (int o = 0; o < oct; o++) { sum += amp * Value(u * fx, v * fy, fx, fy, s + o * 31); norm += amp; amp *= .5f; fx *= 2; fy *= 2; }
            return sum / norm;
        }

        class Maps { public Texture2D albedo, normal, mask, ao; }

        static Maps MakeTextures(string name, Kind kind, Color baseCol, float smooth, float metal, float dustAmt, float normalStrength, int seed)
        {
            int N = TexSize; var h = new float[N * N]; var alb = new Color32[N * N]; var msk = new Color32[N * N]; var occ = new Color32[N * N];
            for (int y = 0; y < N; y++)
                for (int x = 0; x < N; x++)
                {
                    float u = (float)x / N, v = (float)y / N; int i = y * N + x;
                    float height = 0, groove = 0, dust = 0, sm = smooth, var1 = Fbm(u, v, 8, 8, 4, seed);
                    Color c = baseCol; float mt = metal;
                    switch (kind)
                    {
                        case Kind.Paint:
                            {
                                float row = Mathf.Floor(v * 4); float gu = Frac(u * 4 + (row % 2) * .5f), gv = Frac(v * 4);
                                float du = Mathf.Min(gu, 1 - gu) * N / 4, dv = Mathf.Min(gv, 1 - gv) * N / 4;
                                groove = 1 - Mathf.Clamp01(Mathf.Min(du, dv) / 2.2f);
                                float rivet = 0;
                                float r1 = new Vector2(du - 6f, (Frac(v * 32) - .5f) * N / 32).magnitude, r2 = new Vector2(dv - 6f, (Frac(u * 32) - .5f) * N / 32).magnitude;
                                rivet = Mathf.Clamp01(1 - Mathf.Min(r1, r2) / 2.2f);
                                height = -groove + .55f * rivet + .1f * Fbm(u, v, 64, 64, 2, seed + 5);
                                dust = Mathf.SmoothStep(0, 1, Mathf.InverseLerp(.5f, .78f, Fbm(u, v, 5, 5, 5, seed + 9))) * .6f + groove * .45f + (Hash(x, y, seed) > .993f ? .5f : 0);
                                c = baseCol * (.93f + .1f * var1) * (1 - .35f * groove);
                                break;
                            }
                        case Kind.Graphite:
                            height = .25f * Fbm(u, v, 96, 96, 2, seed + 3);
                            dust = Mathf.InverseLerp(.55f, .8f, Fbm(u, v, 6, 6, 5, seed + 9)) * .7f + (Hash(x, y, seed) > .992f ? .5f : 0);
                            c = baseCol * (.9f + .2f * var1); break;
                        case Kind.Titanium:
                            {
                                float s = Fbm(u, v, 2, 128, 3, seed + 2);
                                height = .2f * s; c = baseCol * (.82f + .3f * s); sm = smooth * (.8f + .35f * s);
                                dust = Mathf.InverseLerp(.62f, .85f, Fbm(u, v, 6, 6, 4, seed + 9)) * .5f; break;
                            }
                        case Kind.Gold:
                            {
                                float cr = 1 - Mathf.Abs(2 * Fbm(u, v, 10, 10, 4, seed + 4) - 1);
                                height = cr; c = baseCol * (.86f + .2f * cr); sm = smooth * (.75f + .3f * cr);
                                dust = Mathf.InverseLerp(.65f, .85f, Fbm(u, v, 6, 6, 4, seed + 9)) * .4f; break;
                            }
                        case Kind.Rubber:
                            height = .35f * Fbm(u, v, 128, 128, 2, seed + 6);
                            dust = Mathf.InverseLerp(.35f, .75f, Fbm(u, v, 8, 8, 5, seed + 9)) * .9f + (Hash(x, y, seed) > .985f ? .4f : 0);
                            c = baseCol * (.9f + .2f * var1); break;
                        case Kind.Regolith:
                            {
                                float big = Fbm(u, v, 4, 4, 6, seed + 7), small = Fbm(u, v, 48, 48, 3, seed + 8);
                                float pebble = Mathf.Clamp01((Value(u * 40, v * 40, 40, 40, seed + 11) - .78f) * 6f);
                                height = big * .6f + small * .5f + pebble * 1.2f;
                                c = Color.Lerp(new Color(.36f, .19f, .11f), new Color(.72f, .46f, .30f), Mathf.Clamp01(big * 1.3f - .15f)) * (.85f + .25f * small);
                                c = Color.Lerp(c, new Color(.25f, .17f, .13f), pebble * .7f); dust = 0; break;
                            }
                    }
                    dust = Mathf.Clamp01(dust) * dustAmt;
                    c = Color.Lerp(c, Dust * (.9f + .2f * var1), dust);
                    sm = Mathf.Clamp01(sm * (1 - .75f * dust));
                    mt *= 1 - dust;
                    h[i] = height;
                    alb[i] = (Color32)new Color(Mathf.Clamp01(c.r), Mathf.Clamp01(c.g), Mathf.Clamp01(c.b), 1);
                    msk[i] = new Color32((byte)(Mathf.Clamp01(mt) * 255), 0, 0, (byte)(sm * 255));
                    byte a = (byte)(Mathf.Clamp01(1 - .6f * groove) * 255); occ[i] = new Color32(a, a, a, 255);
                }
            var nrm = new Color32[N * N];
            for (int y = 0; y < N; y++)
                for (int x = 0; x < N; x++)
                {
                    float dx = h[y * N + (x + 1) % N] - h[y * N + (x - 1 + N) % N], dy = h[((y + 1) % N) * N + x] - h[((y - 1 + N) % N) * N + x];
                    var n = new Vector3(-dx * normalStrength, -dy * normalStrength, 1).normalized;
                    nrm[y * N + x] = new Color32((byte)((n.x * .5f + .5f) * 255), (byte)((n.y * .5f + .5f) * 255), (byte)((n.z * .5f + .5f) * 255), 255);
                }
            return new Maps
            {
                albedo = SaveTex(name + "_Albedo", alb, true, false),
                normal = SaveTex(name + "_Normal", nrm, false, true),
                mask = SaveTex(name + "_MetalSmooth", msk, false, false),
                ao = SaveTex(name + "_AO", occ, false, false)
            };
        }
        static float Frac(float f) => f - Mathf.Floor(f);

        static Texture2D SaveTex(string name, Color32[] px, bool srgb, bool normal)
        {
            var t = new Texture2D(TexSize, TexSize, TextureFormat.RGBA32, false, !srgb); t.SetPixels32(px); t.Apply();
            string path = TexDir + "/ARES_" + name + ".png";
            File.WriteAllBytes(path, t.EncodeToPNG()); UnityEngine.Object.DestroyImmediate(t);
            AssetDatabase.ImportAsset(path);
            var ti = (TextureImporter)AssetImporter.GetAtPath(path);
            ti.textureType = normal ? TextureImporterType.NormalMap : TextureImporterType.Default;
            ti.sRGBTexture = srgb; ti.wrapMode = TextureWrapMode.Repeat; ti.mipmapEnabled = true; ti.anisoLevel = 4;
            ti.alphaSource = TextureImporterAlphaSource.FromInput; ti.alphaIsTransparency = false;
            ti.textureCompression = TextureImporterCompression.CompressedHQ; ti.maxTextureSize = TexSize;
            ti.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        static Material Lit(string name, Color baseCol, Maps maps, float bump, Color? emission = null, float alpha = 1, bool bothFaces = false, float smooth = .5f)
        {
            var m = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = "ARES2_" + name, enableInstancing = true };
            m.SetColor("_BaseColor", new Color(maps != null ? 1 : baseCol.r, maps != null ? 1 : baseCol.g, maps != null ? 1 : baseCol.b, alpha));
            if (maps != null)
            {
                m.SetTexture("_BaseMap", maps.albedo); m.SetTexture("_BumpMap", maps.normal); m.SetFloat("_BumpScale", bump);
                m.SetTexture("_MetallicGlossMap", maps.mask); m.SetFloat("_Smoothness", 1f); m.SetFloat("_Metallic", 1f);
                m.SetTexture("_OcclusionMap", maps.ao); m.SetFloat("_OcclusionStrength", 1f);
            }
            else { m.SetFloat("_Smoothness", smooth); m.SetFloat("_Metallic", 0); }
            if (emission.HasValue)
            {
                m.EnableKeyword("_EMISSION"); m.SetColor("_EmissionColor", emission.Value);
                m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            }
            if (alpha < 1)
            {
                m.SetFloat("_Surface", 1); m.SetFloat("_Blend", 0);
                m.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha); m.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                m.SetFloat("_ZWrite", 0); m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                m.SetOverrideTag("RenderType", "Transparent"); m.renderQueue = (int)RenderQueue.Transparent;
            }
            if (bothFaces) m.SetFloat("_Cull", (float)CullMode.Off);
            string path = MatDir + "/" + m.name + ".mat";
            AssetDatabase.DeleteAsset(path); AssetDatabase.CreateAsset(m, path);
            m = AssetDatabase.LoadAssetAtPath<Material>(path);
            BaseShaderGUI.SetMaterialKeywords(m, UnityEditor.Rendering.Universal.ShaderGUI.LitGUI.SetMaterialKeywords);
            EditorUtility.SetDirty(m);
            return m;
        }

        static Dictionary<string, Material> MakeMaterials()
        {
            var d = new Dictionary<string, Material>();
            d["PaintWhite"] = Lit("PaintWhite", default, MakeTextures("PaintWhite", Kind.Paint, new Color(.86f, .865f, .85f), .62f, 0, .45f, 3f, 11), 1f);
            d["PaintOrange"] = Lit("PaintOrange", default, MakeTextures("PaintOrange", Kind.Paint, new Color(.80f, .38f, .15f), .58f, 0, .5f, 3f, 23), 1f);
            d["Graphite"] = Lit("Graphite", default, MakeTextures("Graphite", Kind.Graphite, new Color(.15f, .17f, .19f), .42f, 0, .55f, 2f, 37), .8f);
            d["Titanium"] = Lit("Titanium", default, MakeTextures("Titanium", Kind.Titanium, new Color(.62f, .64f, .66f), .62f, 1, .35f, 1.5f, 41), .6f);
            d["GoldFoil"] = Lit("GoldFoil", default, MakeTextures("GoldFoil", Kind.Gold, new Color(.86f, .66f, .26f), .78f, 1, .12f, 4f, 53), 1.1f);
            d["Rubber"] = Lit("Rubber", default, MakeTextures("Rubber", Kind.Rubber, new Color(.12f, .12f, .13f), .2f, 0, .5f, 2.5f, 67), 1f);
            d["Glass"] = Lit("Glass", new Color(.10f, .16f, .20f), null, 0, null, .38f, false, .96f);
            d["CyanEmissive"] = Lit("CyanEmissive", new Color(0, .55f, .65f), null, 0, new Color(0, .79f, .91f) * 2.2f, 1, false, .85f);
            d["HoverPlasma"] = Lit("HoverPlasma", new Color(.04f, .2f, .25f), null, 0, new Color(0, .79f, .91f) * .25f, 1, false, .8f);
            d["Plume"] = Lit("Plume", new Color(.4f, .9f, 1f), null, 0, new Color(0, .79f, .91f) * 3f, .22f, true, .9f);
            d["LED"] = Lit("LED", new Color(1f, .96f, .88f), null, 0, new Color(1f, .93f, .82f) * 4f, 1, false, .9f);
            var regolith = MakeTextures("Regolith", Kind.Regolith, Color.white, .12f, 0, 0, 5f, 79);
            var ground = Lit("MarsRegolith", default, regolith, 1.2f);
            ground.SetTextureScale("_BaseMap", new Vector2(14, 14));
            EditorUtility.SetDirty(ground);
            AssetDatabase.SaveAssets();
            report.AppendLine("materials: " + string.Join(", ", d.Values.Select(m => m.name)) + ", " + ground.name + "; procedural 1024px albedo/normal/metal-smoothness/AO sets for paint (panel lines, rivets, Mars dust), graphite, brushed titanium, crinkled gold foil, dusty rubber, regolith");
            return d;
        }

        // ------------------------------------------------------------------ animation
        static AnimatorController MakeAnimation(GameObject root)
        {
            Transform T(string n) => root.GetComponentsInChildren<Transform>(true).First(t => t.name == n);
            string P(string n) => AnimationUtility.CalculateTransformPath(T(n), root.transform);
            AnimationClip Clip(string name, bool loop)
            {
                var c = new AnimationClip { name = name, frameRate = 60 };
                var s = AnimationUtility.GetAnimationClipSettings(c); s.loopTime = loop; AnimationUtility.SetAnimationClipSettings(c, s);
                return c;
            }
            void Curve(AnimationClip c, string path, Type type, string prop, float[] t, float[] v, bool linear)
            {
                var curve = new AnimationCurve(t.Select((x, i) => new Keyframe(x, v[i])).ToArray());
                for (int i = 0; i < curve.length; i++)
                {
                    var mode = linear ? AnimationUtility.TangentMode.Linear : AnimationUtility.TangentMode.ClampedAuto;
                    AnimationUtility.SetKeyLeftTangentMode(curve, i, mode); AnimationUtility.SetKeyRightTangentMode(curve, i, mode);
                }
                AnimationUtility.SetEditorCurve(c, EditorCurveBinding.FloatCurve(path, type, prop), curve);
            }
            void Rot(AnimationClip c, string n, float[] t, Vector3[] e, bool linear = false)
            { var p = P(n); Curve(c, p, typeof(Transform), "localEulerAnglesRaw.x", t, e.Select(x => x.x).ToArray(), linear); Curve(c, p, typeof(Transform), "localEulerAnglesRaw.y", t, e.Select(x => x.y).ToArray(), linear); Curve(c, p, typeof(Transform), "localEulerAnglesRaw.z", t, e.Select(x => x.z).ToArray(), linear); }
            void Pos(AnimationClip c, string n, float[] t, Vector3[] q)
            { var p = P(n); Curve(c, p, typeof(Transform), "m_LocalPosition.x", t, q.Select(x => x.x).ToArray(), false); Curve(c, p, typeof(Transform), "m_LocalPosition.y", t, q.Select(x => x.y).ToArray(), false); Curve(c, p, typeof(Transform), "m_LocalPosition.z", t, q.Select(x => x.z).ToArray(), false); }
            void Scl(AnimationClip c, string n, float[] t, Vector3[] q)
            { var p = P(n); Curve(c, p, typeof(Transform), "m_LocalScale.x", t, q.Select(x => x.x).ToArray(), false); Curve(c, p, typeof(Transform), "m_LocalScale.y", t, q.Select(x => x.y).ToArray(), false); Curve(c, p, typeof(Transform), "m_LocalScale.z", t, q.Select(x => x.z).ToArray(), false); }
            void Emi(AnimationClip c, string n, float[] t, float[] k)
            {
                var p = P(n); var col = new Color(0, .79f, .91f);
                Curve(c, p, typeof(MeshRenderer), "material._EmissionColor.r", t, k.Select(x => col.r * x).ToArray(), false);
                Curve(c, p, typeof(MeshRenderer), "material._EmissionColor.g", t, k.Select(x => col.g * x).ToArray(), false);
                Curve(c, p, typeof(MeshRenderer), "material._EmissionColor.b", t, k.Select(x => col.b * x).ToArray(), false);
                Curve(c, p, typeof(MeshRenderer), "material._EmissionColor.a", t, k.Select(x => 1f).ToArray(), false);
            }
            Vector3 X(float a) => new Vector3(a, 0, 0); Vector3 Y(float a) => new Vector3(0, a, 0);
            string[] wheels = { "Wheel_FL", "Wheel_ML", "Wheel_RL", "Wheel_FR", "Wheel_MR", "Wheel_RR" };
            string[] hovers = { "Hover_FL", "Hover_FR", "Hover_RL", "Hover_RR" };
            var clips = new List<AnimationClip>();
            const float HoverY = .38f;

            var wheel = Clip("ARES_WheelRotation", true);
            foreach (var w in wheels) Rot(wheel, w, new[] { 0f, 2f }, new[] { X(0), X(360) }, true);
            clips.Add(wheel);

            var rb = Clip("ARES_RockerBogie", true);
            // Dense keys (every 0.25 s) so the forward-kinematics body lift stays in step with the joint angles.
            float[] t5 = Enumerable.Range(0, 17).Select(i => i * .25f).ToArray();
            float[] rl = t5.Select(t => 6f * Mathf.Sin(t * Mathf.PI / 2)).ToArray(), bl = rl.Select(a => -1.5f * a).ToArray();
            Rot(rb, "Rocker_L", t5, rl.Select(X).ToArray()); Rot(rb, "Bogie_L", t5, bl.Select(X).ToArray());
            Rot(rb, "Rocker_R", t5, rl.Select(a => X(-a)).ToArray()); Rot(rb, "Bogie_R", t5, bl.Select(a => X(-a)).ToArray());
            // Terrain articulation on a flat floor: lift the body by the forward-kinematics amount so the lowest wheel stays on y = 0.
            var lift = t5.Select((_, i) => Mathf.Max(LowestHubDrop(rl[i], bl[i]), LowestHubDrop(-rl[i], -bl[i]))).ToArray();
            Pos(rb, "Chassis", t5, lift.Select(y => Y(y)).ToArray());
            report.AppendLine("rocker-bogie body lift per key (m): " + string.Join(", ", lift.Select(y => y.ToString("F3", Inv))));
            foreach (var w in wheels) Rot(rb, w, new[] { 0f, 4f }, new[] { X(0), X(360) }, true);
            clips.Add(rb);

            var arm = Clip("ARES_ArmDeploy", false);
            Rot(arm, "Arm_Yaw", new[] { 0f, .3f, 1.8f, 4.8f }, new[] { Y(90), Y(90), Y(0), Y(0) });
            Rot(arm, "Arm_Fore", new[] { 0f, 1.8f, 3.4f, 4.8f }, new[] { X(-90), X(-90), X(0), X(0) });
            Rot(arm, "Arm_Upper", new[] { 0f, 1.6f, 2.6f, 3.4f, 4.8f }, new[] { X(0), X(0), X(-12), X(0), X(0) });
            Rot(arm, "Arm_Drill", new[] { 0f, 3.4f, 4.8f }, new[] { Y(0), Y(0), Y(1080) }, true);
            clips.Add(arm);

            var cam = Clip("ARES_ChemCamScan", true);
            float[] tc = { 0, 1.2f, 2.2f, 3.8f, 4.6f, 6f };
            Rot(cam, "Mast_Pan", tc, new[] { Y(0), Y(-55), Y(-55), Y(55), Y(55), Y(0) });
            Rot(cam, "Mast_Tilt", tc, new[] { X(0), X(-12), X(-12), X(10), X(10), X(0) });
            Emi(cam, "Mast_Tilt", new[] { 0, 2.3f, 2.4f, 2.6f, 2.7f, 4.7f, 4.8f, 5.0f, 5.1f, 6 }, new[] { 2.2f, 2.2f, 10, 10, 2.2f, 2.2f, 10, 10, 2.2f, 2.2f });
            clips.Add(cam);

            var act = Clip("ARES_HoverActivation", false);
            float[] ta = { 0, .6f, 2.4f, 3f };
            Pos(act, "Chassis", ta, new[] { Vector3.zero, Vector3.zero, Y(HoverY), Y(HoverY) });
            Rot(act, "Rocker_L", ta, new[] { X(0), X(0), X(-8), X(-8) }); Rot(act, "Rocker_R", ta, new[] { X(0), X(0), X(-8), X(-8) });
            Rot(act, "Bogie_L", ta, new[] { X(0), X(0), X(10), X(10) }); Rot(act, "Bogie_R", ta, new[] { X(0), X(0), X(10), X(10) });
            foreach (var h in hovers)
            {
                Emi(act, h, new[] { 0f, .6f, 1.4f, 3f }, new[] { .25f, 2f, 6f, 6f });
                Scl(act, h + "_Plume", new[] { 0f, .5f, 1.4f, 3f }, new[] { new Vector3(.4f, .001f, .4f), new Vector3(.4f, .001f, .4f), Vector3.one, Vector3.one });
                Emi(act, h + "_Plume", new[] { 0f, .5f, 1.4f, 3f }, new[] { 0f, 0f, 3f, 3f });
                Rot(act, h + "_Vanes", new[] { 0f, 3f }, new[] { Y(0), Y(90) });
            }
            clips.Add(act);

            var stab = Clip("ARES_HoverStabilization", true);
            float[] ts = { 0, 1, 2, 3, 4 };
            Pos(stab, "Chassis", ts, new[] { Y(HoverY), Y(HoverY + .03f), Y(HoverY), Y(HoverY - .03f), Y(HoverY) });
            Rot(stab, "Chassis", ts, new[] { new Vector3(0, 0, 0), new Vector3(1.2f, 0, -.8f), new Vector3(0, 0, 0), new Vector3(-1.2f, 0, .8f), new Vector3(0, 0, 0) });
            foreach (var r in new[] { "Rocker_L", "Rocker_R" }) Rot(stab, r, new[] { 0f, 4f }, new[] { X(-8), X(-8) });
            foreach (var b in new[] { "Bogie_L", "Bogie_R" }) Rot(stab, b, new[] { 0f, 4f }, new[] { X(10), X(10) });
            foreach (var h in hovers)
            {
                bool front = h.Contains("F");
                Rot(stab, h, ts, new[] { X(0), X(front ? -6 : 6), X(0), X(front ? 6 : -6), X(0) });
                Emi(stab, h, new[] { 0f, .5f, 1.1f, 1.9f, 2.6f, 3.3f, 4f }, new[] { 6f, 5.2f, 6.8f, 5.6f, 6.6f, 5.4f, 6f });
                Scl(stab, h + "_Plume", ts, new[] { Vector3.one, new Vector3(1, 1.1f, 1), Vector3.one, new Vector3(1, .9f, 1), Vector3.one });
                Emi(stab, h + "_Plume", new[] { 0f, 4f }, new[] { 3f, 3f });
                Rot(stab, h + "_Vanes", new[] { 0f, 4f }, new[] { Y(90), Y(450) }, true);
            }
            clips.Add(stab);

            var land = Clip("ARES_Landing", false);
            Pos(land, "Chassis", new[] { 0f, 2.3f, 2.6f, 3.0f }, new[] { Y(HoverY), Y(0), Y(.012f), Y(0) });
            foreach (var r in new[] { "Rocker_L", "Rocker_R" }) Rot(land, r, new[] { 0f, 1.2f, 2.2f, 3.5f }, new[] { X(-8), X(-8), X(0), X(0) });
            foreach (var b in new[] { "Bogie_L", "Bogie_R" }) Rot(land, b, new[] { 0f, 1.2f, 2.2f, 3.5f }, new[] { X(10), X(10), X(0), X(0) });
            foreach (var h in hovers)
            {
                Emi(land, h, new[] { 0f, 1.8f, 2.6f, 3.5f }, new[] { 6f, 6f, 1f, .25f });
                Scl(land, h + "_Plume", new[] { 0f, 1.8f, 2.6f, 3.5f }, new[] { Vector3.one, Vector3.one, new Vector3(.4f, .001f, .4f), new Vector3(.4f, .001f, .4f) });
                Emi(land, h + "_Plume", new[] { 0f, 1.8f, 2.6f, 3.5f }, new[] { 3f, 3f, 0f, 0f });
                Rot(land, h + "_Vanes", new[] { 0f, 3.5f }, new[] { Y(90), Y(0) });
            }
            clips.Add(land);

            AssetDatabase.DeleteAsset(ControllerPath);
            foreach (var c in clips) { var p = AnimDir + "/" + c.name + ".anim"; AssetDatabase.DeleteAsset(p); AssetDatabase.CreateAsset(c, p); }
            var ctrl = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
            var sm = ctrl.layers[0].stateMachine;
            var idle = sm.AddState("Idle"); sm.defaultState = idle;
            foreach (var c in clips) sm.AddState(c.name).motion = c;
            AssetDatabase.SaveAssets();
            report.AppendLine("animation clips: " + string.Join(", ", clips.Select(c => $"{c.name} {c.length.ToString("F1", Inv)}s{(c.isLooping ? " loop" : "")} ({AnimationUtility.GetCurveBindings(c).Length} curves)")));
            return ctrl;
        }

        // Hub drop below axle height for given rocker/bogie angles (one side), from the build-time pivots.
        static float LowestHubDrop(float rocker, float bogie)
        {
            Vector3 pr = new Vector3(0, .76f, 0), pb = new Vector3(0, .60f, -.62f);
            Vector3 F = new Vector3(0, .38f, 1.18f), M = new Vector3(0, .38f, 0), R = new Vector3(0, .38f, -1.18f);
            var qr = Quaternion.Euler(rocker, 0, 0); var qb = qr * Quaternion.Euler(bogie, 0, 0);
            var pb2 = pr + qr * (pb - pr);
            float f = (pr + qr * (F - pr)).y, m = (pb2 + qb * (M - pb)).y, r = (pb2 + qb * (R - pb)).y;
            return Mathf.Max(0, .38f - Mathf.Min(f, Mathf.Min(m, r)));
        }

        // ------------------------------------------------------------------ scene
        static void UpdateScene()
        {
            var scene = EditorSceneManager.OpenScene(AresViShowroom.Scene);
            foreach (var n in new[] { "ARES_VI_Upgraded", "Showroom Volume", "Mars Preset" })
                foreach (var g in scene.GetRootGameObjects().Where(g => g.name == n).ToList()) UnityEngine.Object.DestroyImmediate(g);
            var original = scene.GetRootGameObjects().FirstOrDefault(g => g.name == "ARES_VI_Showroom_Root");
            if (original) { original.transform.position += new Vector3(0, 0, 0); original.SetActive(false); }
            var inst = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath), scene);
            inst.transform.position = Vector3.zero;

            var profilePath = Dir + "/ARES_Showroom_Volume.asset";
            AssetDatabase.DeleteAsset(profilePath);
            var profile = ScriptableObject.CreateInstance<VolumeProfile>();
            AssetDatabase.CreateAsset(profile, profilePath);
            var tm = profile.Add<Tonemapping>(true); tm.mode.Override(TonemappingMode.ACES);
            var bloom = profile.Add<Bloom>(true); bloom.intensity.Override(.55f); bloom.threshold.Override(1.05f); bloom.scatter.Override(.6f);
            var ca = profile.Add<ColorAdjustments>(true); ca.postExposure.Override(.35f); ca.contrast.Override(6f);
            var vig = profile.Add<Vignette>(true); vig.intensity.Override(.18f);
            foreach (var c in profile.components) { c.name = c.GetType().Name; AssetDatabase.AddObjectToAsset(c, profile); }
            EditorUtility.SetDirty(profile);
            var vol = new GameObject("Showroom Volume").AddComponent<Volume>(); vol.isGlobal = true; vol.sharedProfile = profile;

            var cam = scene.GetRootGameObjects().First(g => g.name == "Showroom Camera").GetComponent<Camera>();
            var data = cam.GetUniversalAdditionalCameraData(); data.renderPostProcessing = true; data.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;

            // Mars preset (off by default; the verification run switches to it for two shots).
            var mars = new GameObject("Mars Preset"); mars.SetActive(false);
            var ground = GameObject.CreatePrimitive(PrimitiveType.Cylinder); ground.name = "Mars Ground"; ground.transform.SetParent(mars.transform);
            ground.transform.localPosition = new Vector3(0, -.05f, 0); ground.transform.localScale = new Vector3(60, .05f, 60);
            ground.GetComponent<Renderer>().sharedMaterial = AssetDatabase.LoadAssetAtPath<Material>(MatDir + "/ARES2_MarsRegolith.mat");
            UnityEngine.Object.DestroyImmediate(ground.GetComponent<Collider>());
            var sun = new GameObject("Mars Sun"); sun.transform.SetParent(mars.transform); sun.transform.rotation = Quaternion.Euler(24, -50, 0);
            var l = sun.AddComponent<Light>(); l.type = LightType.Directional; l.intensity = 1.9f; l.color = new Color(1f, .86f, .70f); l.shadows = LightShadows.Soft;
            var skyPath = MatDir + "/ARES2_MarsSky.mat";
            AssetDatabase.DeleteAsset(skyPath);
            var sky = new Material(Shader.Find("Skybox/Procedural")) { name = "ARES2_MarsSky" };
            sky.SetColor("_SkyTint", new Color(.95f, .58f, .42f)); sky.SetColor("_GroundColor", new Color(.45f, .27f, .18f));
            sky.SetFloat("_AtmosphereThickness", 1.6f); sky.SetFloat("_Exposure", .9f); sky.SetFloat("_SunSize", .03f);
            AssetDatabase.CreateAsset(sky, skyPath);

            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            report.AppendLine("scene: " + AresViShowroom.Scene + " now holds ARES_VI_Upgraded (active), the original import (inactive, for before/after), a post-processing volume (ACES, bloom, vignette) and an inactive Mars preset");
        }
    }
}
