using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Unity.Profiling;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace AstroGateway.VehicleShowroom.EditorTools
{
    // Isolated ARES-VI showroom. Touches only Assets/AstroGateway/VehicleShowroom and Docs/VehicleShowroom.
    // Build (batch-safe): configures the OBJ importers, records what Unity actually imported, makes URP Lit
    // materials from the MTL values, and writes ARES_VI_Showroom.unity.
    // Verify (graphical Editor): Play Mode review shots with ScreenCapture, pivot/animation checks, performance
    // samples (rover on/off), then the same counters on the Visual Upgrade Mars scene for comparison (never saved).
    [InitializeOnLoad]
    public static class AresViShowroom
    {
        public const string Root = "Assets/AstroGateway/VehicleShowroom";
        public const string Src = Root + "/ARES_VI/Source";
        public const string MatDir = Root + "/ARES_VI/Materials";
        public const string Scene = Root + "/Scenes/ARES_VI_Showroom.unity";
        const string BaselineScene = "Assets/AstroGateway/VisualUpgrade/Scenes/Mars_VisualUpgrade_v1.unity";
        static readonly string[] Models = { "ares_vi_aaa_master.obj", "ares_vi_aaa_lod1.obj", "ares_vi_aaa_lod2.obj" };
        static string Out => Environment.GetEnvironmentVariable("ARES_OUT") ?? Path.GetFullPath("Docs/VehicleShowroom/ARES_VI");
        static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

        // ---------------------------------------------------------------- Build
        public static void Build()
        {
            Directory.CreateDirectory(Out);
            var r = new StringBuilder();
            r.AppendLine("ARES-VI Unity import report (measured in Unity " + Application.unityVersion + ", " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ")");
            AssetDatabase.Refresh();

            // 1. What the importer gives with its own defaults.
            r.AppendLine("\n== Default import (before any material remap)");
            foreach (var m in Models)
            {
                var path = Src + "/" + m;
                var imp = (ModelImporter)AssetImporter.GetAtPath(path);
                imp.isReadable = true; imp.importNormals = ModelImporterNormals.Import; imp.importAnimation = true;
                foreach (var id in imp.GetExternalObjectMap().Keys.ToList()) imp.RemoveRemap(id);
                imp.SaveAndReimport();
                var mats = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Material>().ToList();
                r.AppendLine($"{m}: embedded materials {mats.Count}: " + string.Join(", ", mats.Select(x => x.name + " [" + x.shader.name + (x.shader.isSupported ? "" : " UNSUPPORTED") + "]")));
            }
            var glb = AssetImporter.GetAtPath(Src + "/ares_vi_aaa_master.glb");
            var glbMain = AssetDatabase.LoadMainAssetAtPath(Src + "/ares_vi_aaa_master.glb");
            r.AppendLine("ares_vi_aaa_master.glb: importer " + (glb ? glb.GetType().Name : "none") + ", main asset type " + (glbMain ? glbMain.GetType().Name : "none") +
                         " (no glTF importer package in Packages/manifest.json; GLB is not importable as a model here)");

            // 2. URP Lit materials from the MTL values, remapped onto every model.
            Directory.CreateDirectory(MatDir);
            var mtl = ParseMtl(Src + "/ares_vi_aaa_master.mtl");
            var urp = new Dictionary<string, Material>();
            r.AppendLine("\n== URP materials created from ares_vi_aaa_master.mtl (no texture maps referenced by the MTL)");
            foreach (var kv in mtl)
            {
                var m = MakeMaterial(kv.Key, kv.Value);
                urp[kv.Key] = m;
                r.AppendLine($"{kv.Key}: base {Hex(m.GetColor("_BaseColor"))} metallic {m.GetFloat("_Metallic"):F2} smoothness {m.GetFloat("_Smoothness"):F2} surface {(m.GetFloat("_Surface") > 0 ? "Transparent" : "Opaque")} emission {(m.IsKeywordEnabled("_EMISSION") ? Hex(m.GetColor("_EmissionColor")) : "off")} maps: none");
            }
            foreach (var m in Models)
            {
                var path = Src + "/" + m;
                var imp = (ModelImporter)AssetImporter.GetAtPath(path);
                foreach (var mat in AssetDatabase.LoadAllAssetsAtPath(path).OfType<Material>().ToList())
                    if (urp.TryGetValue(mat.name, out var target)) imp.AddRemap(new AssetImporter.SourceAssetIdentifier(typeof(Material), mat.name), target);
                imp.SaveAndReimport();
            }

            // 3. Measured import statistics.
            foreach (var m in Models) DescribeModel(Src + "/" + m, r);

            // 4. Scene.
            BuildScene(r);
            File.WriteAllText(Path.Combine(Out, "Unity_Import_Report.txt"), r.ToString());
            Debug.Log("[ARES-VI] Build finished: " + Scene);
        }

        static void DescribeModel(string path, StringBuilder r)
        {
            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var imp = (ModelImporter)AssetImporter.GetAtPath(path);
            r.AppendLine($"\n== {Path.GetFileName(path)} (globalScale {imp.globalScale.ToString(Inv)}, useFileScale {imp.useFileScale}, fileScale {imp.fileScale.ToString(Inv)})");
            var filters = go.GetComponentsInChildren<MeshFilter>(true);
            var renderers = go.GetComponentsInChildren<MeshRenderer>(true);
            int tris = 0, verts = 0, sub = 0, opp = 0, noNormals = 0, noUv = 0, depth = 0;
            foreach (var f in filters)
            {
                var mesh = f.sharedMesh; if (!mesh) continue;
                verts += mesh.vertexCount; sub += mesh.subMeshCount;
                var v = mesh.vertices; var n = mesh.normals; var t = mesh.triangles; tris += t.Length / 3;
                if (n == null || n.Length == 0) noNormals++;
                if (mesh.uv == null || mesh.uv.Length == 0) noUv++;
                if (n != null && n.Length == v.Length)
                    for (int i = 0; i < t.Length; i += 3)
                    {
                        var fn = Vector3.Cross(v[t[i + 1]] - v[t[i]], v[t[i + 2]] - v[t[i]]);
                        if (Vector3.Dot(fn, n[t[i]] + n[t[i + 1]] + n[t[i + 2]]) < 0) opp++;
                    }
            }
            foreach (Transform tr in go.GetComponentsInChildren<Transform>(true)) depth = Mathf.Max(depth, Depth(tr, go.transform));
            var clips = AssetDatabase.LoadAllAssetsAtPath(path).OfType<AnimationClip>().Where(c => !c.name.StartsWith("__preview__")).ToList();
            var mats = renderers.SelectMany(x => x.sharedMaterials).Where(x => x).Distinct().ToList();
            var b = Bounds(go);
            r.AppendLine($"GameObjects {go.GetComponentsInChildren<Transform>(true).Length} (root + children), hierarchy depth {depth}, mesh renderers {renderers.Length}");
            r.AppendLine($"triangles {tris}, vertices {verts}, submeshes {sub}, meshes without normals {noNormals}, meshes without UVs {noUv}");
            r.AppendLine($"triangles whose imported normal opposes Unity front-face winding: {opp} ({(tris > 0 ? 100f * opp / tris : 0):F1}%)");
            r.AppendLine($"materials used {mats.Count}: " + string.Join(", ", mats.Select(x => x.name + " [" + x.shader.name + "]")));
            r.AppendLine($"animation clips {clips.Count}" + (clips.Count > 0 ? ": " + string.Join(", ", clips.Select(c => c.name + " " + c.length.ToString("F2", Inv) + "s")) : " (none)"));
            r.AppendLine($"bounds as imported (no rotation): centre {V(b.center)} size {V(b.size)}");
            if (path.EndsWith("master.obj"))
            {
                int atOrigin = 0;
                r.AppendLine("children and pivots (localPosition / mesh-bounds centre / pivot-to-centre distance):");
                foreach (var f in filters)
                {
                    var mb = f.sharedMesh ? f.sharedMesh.bounds : new Bounds();
                    var lp = f.transform.localPosition; if (lp.sqrMagnitude < 1e-8f) atOrigin++;
                    r.AppendLine($"  {f.name}: pivot {V(lp)} centre {V(mb.center)} offset {(mb.center).magnitude:F3} m tris {(f.sharedMesh ? f.sharedMesh.triangles.Length / 3 : 0)} mat {string.Join("/", f.GetComponent<MeshRenderer>().sharedMaterials.Select(x => x ? x.name : "NONE"))}");
                }
                r.AppendLine($"children whose pivot is the model origin: {atOrigin} / {filters.Length}");
            }
        }

        static int Depth(Transform t, Transform root) { int d = 0; while (t && t != root) { d++; t = t.parent; } return d; }

        static Dictionary<string, Dictionary<string, float[]>> ParseMtl(string path)
        {
            var res = new Dictionary<string, Dictionary<string, float[]>>(); Dictionary<string, float[]> cur = null;
            foreach (var raw in File.ReadAllLines(path))
            {
                var p = raw.Trim().Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                if (p.Length == 0 || p[0].StartsWith("#")) continue;
                if (p[0] == "newmtl") { cur = new Dictionary<string, float[]>(); res[p[1]] = cur; continue; }
                if (cur == null) continue;
                var vals = p.Skip(1).Select(x => float.TryParse(x, NumberStyles.Float, Inv, out var f) ? f : float.NaN).ToArray();
                cur[p[0]] = vals;
            }
            return res;
        }

        static Material MakeMaterial(string name, Dictionary<string, float[]> d)
        {
            float[] Get(string k, params float[] def) => d.TryGetValue(k, out var v) ? v : def;
            var kd = Get("Kd", .8f, .8f, .8f); var ks = Get("Ks", 0, 0, 0); float ns = Get("Ns", 10)[0]; float alpha = Get("d", 1)[0];
            var m = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = name, enableInstancing = true };
            // MTL has no PBR channels: metallic only for the two metal finishes named in the MTL, smoothness from Ns.
            bool metal = name.Contains("Titanium") || name.Contains("Gold");
            m.SetColor("_BaseColor", new Color(kd[0], kd[1], kd[2], alpha));
            m.SetFloat("_Metallic", metal ? .85f : 0f);
            m.SetFloat("_Smoothness", Mathf.Clamp01(Mathf.Sqrt(ns / 200f)));
            if (d.TryGetValue("Ke", out var ke) && ke.Max() > 0)
            {
                m.EnableKeyword("_EMISSION");
                m.SetColor("_EmissionColor", new Color(ke[0], ke[1], ke[2]) * 2f);
                m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            }
            if (alpha < .999f)
            {
                m.SetFloat("_Surface", 1); m.SetFloat("_Blend", 0);
                m.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha); m.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
                m.SetFloat("_ZWrite", 0); m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                m.SetOverrideTag("RenderType", "Transparent"); m.renderQueue = (int)RenderQueue.Transparent;
            }
            string path = MatDir + "/ARES_" + name + ".mat";
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.CreateAsset(m, path);
            m = AssetDatabase.LoadAssetAtPath<Material>(path);
            BaseShaderGUI.SetMaterialKeywords(m, UnityEditor.Rendering.Universal.ShaderGUI.LitGUI.SetMaterialKeywords);
            EditorUtility.SetDirty(m);
            AssetDatabase.SaveAssets();
            return m;
        }

        static Material Plain(string name, Color c, float smooth)
        {
            var m = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = name };
            m.SetColor("_BaseColor", c); m.SetFloat("_Smoothness", smooth); m.SetFloat("_Metallic", 0);
            string path = MatDir + "/" + name + ".mat";
            AssetDatabase.DeleteAsset(path); AssetDatabase.CreateAsset(m, path);
            m = AssetDatabase.LoadAssetAtPath<Material>(path);
            BaseShaderGUI.SetMaterialKeywords(m, UnityEditor.Rendering.Universal.ShaderGUI.LitGUI.SetMaterialKeywords);
            return m;
        }

        static void BuildScene(StringBuilder r)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(.62f, .64f, .70f);
            RenderSettings.ambientEquatorColor = new Color(.42f, .38f, .36f);
            RenderSettings.ambientGroundColor = new Color(.20f, .17f, .15f);
            RenderSettings.skybox = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Skybox.mat");

            var stage = new GameObject("Showroom Stage");
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            floor.name = "Turntable Floor"; floor.transform.SetParent(stage.transform);
            floor.transform.localPosition = new Vector3(0, -.06f, 0); floor.transform.localScale = new Vector3(14, .05f, 14);
            floor.GetComponent<Renderer>().sharedMaterial = Plain("Showroom_Floor", new Color(.30f, .30f, .32f), .35f);
            UnityEngine.Object.DestroyImmediate(floor.GetComponent<Collider>());

            Light L(string n, Vector3 euler, float intensity, Color c, LightShadows sh)
            {
                var g = new GameObject(n); g.transform.SetParent(stage.transform); g.transform.rotation = Quaternion.Euler(euler);
                var l = g.AddComponent<Light>(); l.type = LightType.Directional; l.intensity = intensity; l.color = c; l.shadows = sh; return l;
            }
            var key = L("Key Light", new Vector3(42, -35, 0), 1.6f, new Color(1f, .96f, .9f), LightShadows.Soft);
            RenderSettings.sun = key;
            L("Fill Light", new Vector3(20, 140, 0), .45f, new Color(.75f, .82f, 1f), LightShadows.None);
            L("Rim Light", new Vector3(15, 175, 0), .6f, Color.white, LightShadows.None);

            var cam = new GameObject("Showroom Camera") { tag = "MainCamera" };
            var c = cam.AddComponent<Camera>(); c.fieldOfView = 40; c.nearClipPlane = .05f; c.farClipPlane = 200;
            c.clearFlags = CameraClearFlags.Skybox;
            cam.AddComponent<AudioListener>();
            cam.transform.position = new Vector3(4.5f, 2.4f, 4.8f);

            // OBJ is Z-up with the nose on +Y. Unity's OBJ importer does not convert the up axis, so the showroom
            // parent stands it up; the asset itself is not edited.
            var root = new GameObject("ARES_VI_Showroom_Root");
            var src = AssetDatabase.LoadAssetAtPath<GameObject>(Src + "/ares_vi_aaa_master.obj");
            var inst = (GameObject)PrefabUtility.InstantiatePrefab(src, root.transform);
            inst.name = "ARES_VI_LOD0";
            root.transform.rotation = Quaternion.Euler(-90, 0, 0);
            Vector3 Centre(string n) { var t = inst.GetComponentsInChildren<Renderer>(true).FirstOrDefault(x => x.name == n); return t ? t.bounds.center : new Vector3(float.NaN, 0, 0); }
            var mast = Centre("Mast_ChemCamHead"); var bump = Centre("FrontBumper_MainBar"); var fl = Centre("Wheel_FL_TireBody"); var fr = Centre("Wheel_FR_TireBody");
            bool upOk = mast.y > Bounds(inst).center.y;
            if (bump.z < 0) root.transform.rotation = Quaternion.Euler(0, 180, 0) * root.transform.rotation;
            bump = Centre("FrontBumper_MainBar"); fl = Centre("Wheel_FL_TireBody"); fr = Centre("Wheel_FR_TireBody"); mast = Centre("Mast_ChemCamHead");
            var b = Bounds(inst);
            root.transform.position = new Vector3(-b.center.x, -b.min.y, -b.center.z);
            b = Bounds(inst);
            r.AppendLine("\n== Showroom placement (measured after placing LOD0)");
            r.AppendLine($"parent rotation {V(root.transform.eulerAngles)}; mast above body centre: {upOk}; front bumper z {bump.z:F2} (front = +Z)");
            r.AppendLine($"Wheel_FL x {fl.x:F2}, Wheel_FR x {fr.x:F2} -> " + (fl.x < fr.x ? "left/right consistent (FL on -X when facing +Z)" : "MIRRORED: FL is on +X"));
            r.AppendLine($"world bounds size (W x H x L) {b.size.x:F3} x {b.size.y:F3} x {b.size.z:F3} m, lowest point y {b.min.y:F3}");

            // LOD lineup for the comparison shot (inactive unless the harness turns it on).
            var lineup = new GameObject("LOD Lineup"); lineup.SetActive(false);
            for (int i = 0; i < Models.Length; i++)
            {
                var holder = new GameObject("Lineup " + Models[i]); holder.transform.SetParent(lineup.transform);
                var g = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Src + "/" + Models[i]), holder.transform);
                holder.transform.rotation = root.transform.rotation;
                holder.transform.position = root.transform.position + new Vector3((i - 1) * 3.6f, 0, 0);
            }

            EditorSceneManager.SaveScene(scene, Scene);
            AssetDatabase.SaveAssets();
        }

        public static Bounds Bounds(GameObject g)
        {
            var rs = g.GetComponentsInChildren<Renderer>(true); if (rs.Length == 0) return new Bounds(g.transform.position, Vector3.zero);
            var b = rs[0].bounds; foreach (var x in rs) b.Encapsulate(x.bounds); return b;
        }
        static string V(Vector3 v) => $"({v.x.ToString("F3", Inv)}, {v.y.ToString("F3", Inv)}, {v.z.ToString("F3", Inv)})";
        static string Hex(Color c) => "#" + ColorUtility.ToHtmlStringRGB(c);

        // ---------------------------------------------------------------- Verify (graphical Editor)
        const string StageKey = "AresViShowroom.Stage";
        static IEnumerator routine; static readonly Stack<IEnumerator> stack = new Stack<IEnumerator>();
        static double waitUntil; static int frameWait;
        static readonly List<string> log = new List<string>();

        static AresViShowroom() { EditorApplication.update += Tick; }

        public static void Verify()
        {
            File.Delete(Path.Combine(Out, "Verification_Log.txt"));
            Directory.CreateDirectory(Path.Combine(Out, "Screenshots"));
            Prepare(Scene);
            SessionState.SetInt(StageKey, 1);
            EditorApplication.isPlaying = true;
        }

        static void Prepare(string scenePath)
        {
            EditorSceneManager.OpenScene(scenePath);
            try { PlayModeWindow.SetViewType(PlayModeWindow.PlayModeViewTypes.GameView); } catch { }
            try { PlayModeWindow.SetCustomRenderingResolution(1512, 982, "ARES-VI Showroom"); } catch { }
            try { ProductionViewport.SetSize(1512, 982); } catch (Exception e) { Debug.LogWarning("Game View size: " + e.Message); }
            Selection.activeObject = null;
            SetGizmos(false);
            Application.runInBackground = true;
            EditorApplication.isPaused = false;
        }

        static void SetGizmos(bool on)
        {
            try
            {
                var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView");
                var view = EditorWindow.GetWindow(type, false, null, false);
                type.GetField("m_Gizmos", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(view, on);
                view.Repaint();
            }
            catch (Exception e) { log.Add("gizmo toggle failed: " + e.Message); }
        }

        static void Tick()
        {
            int stage = SessionState.GetInt(StageKey, 0);
            if (stage == 0) return;
            if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                if (stage == 2) { SessionState.SetInt(StageKey, 3); Prepare(BaselineScene); EditorApplication.isPlaying = true; }
                else if (stage == 4)
                {
                    SessionState.SetInt(StageKey, 0);
                    try { ProductionViewport.Cleanup(); } catch { }
                    if (Environment.GetEnvironmentVariable("ARES_QUIT") == "1") EditorApplication.Exit(0);
                }
                return;
            }
            if (!EditorApplication.isPlaying) return;
            if (EditorApplication.isPaused) EditorApplication.isPaused = false;
            try
            {
                if (routine == null)
                {
                    if (stage == 1) routine = ShowroomRun();
                    else if (stage == 3) routine = BaselineRun();
                    else return;
                    stack.Clear(); stack.Push(routine);
                }
                if (EditorApplication.timeSinceStartup < waitUntil) return;
                if (frameWait > 0) { frameWait--; return; }
                while (stack.Count > 0)
                {
                    var top = stack.Peek();
                    if (!top.MoveNext()) { stack.Pop(); continue; }
                    if (top.Current is IEnumerator nested) { stack.Push(nested); continue; }
                    if (top.Current is float s) waitUntil = EditorApplication.timeSinceStartup + s;
                    else if (top.Current is int f) frameWait = f;
                    break;
                }
                if (stack.Count == 0) Done(stage);
            }
            catch (Exception e) { log.Add("EXCEPTION " + e); Done(stage); }
        }

        static void Done(int stage)
        {
            routine = null; stack.Clear();
            Directory.CreateDirectory(Out);
            File.AppendAllLines(Path.Combine(Out, "Verification_Log.txt"), log); log.Clear();
            SessionState.SetInt(StageKey, stage == 1 ? 2 : 4);
            EditorApplication.isPlaying = false;
        }

        static Camera cam;
        static IEnumerator Shot(string name, Vector3 pos, Vector3 target, string note)
        {
            cam.transform.position = pos; cam.transform.LookAt(target);
            yield return 1.0f;
            string path = Path.Combine(Out, "Screenshots", name);
            ScreenCapture.CaptureScreenshot(path);
            yield return 1.0f;
            log.Add($"shot {name}: {note} cam {V(pos)} -> {V(target)} [{Screen.width}x{Screen.height}, saved={File.Exists(path)}]");
        }

        static Bounds PartBounds(GameObject rover, string prefix)
        {
            var rs = rover.GetComponentsInChildren<Renderer>(true).Where(x => x.name.StartsWith(prefix)).ToList();
            if (rs.Count == 0) return new Bounds(new Vector3(float.NaN, float.NaN, float.NaN), Vector3.zero);
            var b = rs[0].bounds; foreach (var x in rs) b.Encapsulate(x.bounds); return b;
        }

        static IEnumerator ShowroomRun()
        {
            log.Add("=== ARES-VI showroom run " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            log.Add($"Unity {Application.unityVersion} | GPU {SystemInfo.graphicsDeviceName} | API {SystemInfo.graphicsDeviceType} | CPU {SystemInfo.processorType} | RAM {SystemInfo.systemMemorySize} MB | quality '{QualitySettings.names[QualitySettings.GetQualityLevel()]}' | pipeline {GraphicsSettings.currentRenderPipeline?.name} | vSync {QualitySettings.vSyncCount} targetFrameRate {Application.targetFrameRate}");
            yield return 1.5f;
            cam = Camera.main;
            var rover = GameObject.Find("ARES_VI_LOD0");
            var floor = GameObject.Find("Turntable Floor");
            if (!cam || !rover) { log.Add("FAIL camera or rover not found"); yield break; }
            var b = Bounds(rover); var c = b.center; var s = b.size;
            log.Add($"rover world bounds centre {V(c)} size {V(s)}");

            // Material check on the live renderers.
            var rends = rover.GetComponentsInChildren<MeshRenderer>(true);
            var mats = rends.SelectMany(x => x.sharedMaterials).ToList();
            int missing = mats.Count(x => !x), bad = mats.Count(x => x && (!x.shader.isSupported || x.shader.name.Contains("InternalErrorShader") || !x.shader.name.StartsWith("Universal Render Pipeline")));
            log.Add($"materials: {rends.Length} renderers, {mats.Count} material slots, distinct {mats.Where(x => x).Distinct().Count()}, missing {missing}, non-URP/unsupported {bad}");

            float d = Mathf.Max(s.x, s.z) * 1.75f;
            yield return Shot("01_front.png", c + new Vector3(0, .35f, d), c, "front (+Z)");
            yield return Shot("02_rear.png", c + new Vector3(0, .35f, -d), c, "rear");
            yield return Shot("03_left.png", c + new Vector3(-d, .35f, 0), c, "left side (-X)");
            yield return Shot("04_right.png", c + new Vector3(d, .35f, 0), c, "right side (+X)");
            yield return Shot("05_top.png", c + new Vector3(0, d * 1.6f, .01f), c, "top-down");
            yield return Shot("06_three_quarter.png", c + new Vector3(d * .7f, s.y * .8f, d * .75f), c, "front-right three-quarter");
            floor.SetActive(false);
            yield return Shot("07_undercarriage.png", c + new Vector3(.4f, -d * .9f, d * .35f), c, "undercarriage from below (floor hidden for this shot)");
            floor.SetActive(true);
            var canopy = PartBounds(rover, "Cockpit_");
            yield return Shot("08_cockpit.png", canopy.center + new Vector3(.9f, 1.1f, 1.6f), canopy.center, "cockpit / canopy close-up");
            var susp = PartBounds(rover, "Suspension_Left");
            yield return Shot("09_suspension_left.png", susp.center + new Vector3(-2.4f, .3f, .9f), susp.center, "left rocker-bogie close-up");
            var arm = PartBounds(rover, "Arm_");
            yield return Shot("10_science_arm.png", arm.center + new Vector3(-.6f, .5f, 1.9f), arm.center, "science arm / drill close-up");
            var mast = PartBounds(rover, "Mast_");
            yield return Shot("11_chemcam_mast.png", mast.center + new Vector3(1.2f, .5f, 1.5f), mast.center + Vector3.up * .3f, "ChemCam mast close-up");
            var hover = PartBounds(rover, "Hover_");
            floor.SetActive(false);
            yield return Shot("12_hover_modules_below.png", hover.center + new Vector3(.8f, -2.6f, 1.2f), hover.center, "anti-gravity hover nacelles from below (floor hidden)");
            floor.SetActive(true);

            // Same front three-quarter with every rover material rendering both faces: shows what back-face
            // culling removes when the winding is inside-out. Materials are restored afterwards.
            var urpMats = mats.Where(x => x).Distinct().ToList();
            var saved = urpMats.ToDictionary(x => x, x => x.GetFloat("_Cull"));
            foreach (var m in urpMats) m.SetFloat("_Cull", (float)CullMode.Off);
            yield return Shot("13_three_quarter_both_faces.png", c + new Vector3(d * .7f, s.y * .8f, d * .75f), c, "diagnostic: same as 06 with Render Face = Both (not a fix)");
            foreach (var kv in saved) kv.Key.SetFloat("_Cull", kv.Value);

            var lineup = GameObject.Find("LOD Lineup") ?? Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(x => x.name == "LOD Lineup" && x.scene.IsValid());
            if (lineup)
            {
                rover.SetActive(false); lineup.SetActive(true);
                yield return Shot("14_lod_lineup.png", new Vector3(0, 3.5f, 11f), new Vector3(0, 1.1f, 0), "LOD0 / LOD1 / LOD2 left to right");
                lineup.SetActive(false); rover.SetActive(true);
            }

            // Animation / articulation evidence.
            log.Add("--- animation and articulation");
            var animators = rover.GetComponentsInChildren<Animator>(true); var legacy = rover.GetComponentsInChildren<Animation>(true);
            var clipCount = AssetDatabase.LoadAllAssetsAtPath(Src + "/ares_vi_aaa_master.obj").OfType<AnimationClip>().Count(x => !x.name.StartsWith("__preview__"));
            log.Add($"Animator components {animators.Length}, Animation components {legacy.Length}, imported clips in model {clipCount}");
            foreach (var part in new[] { "Wheel_FL_TireBody", "Suspension_Left_RockerArm", "Arm_UpperBoom", "Mast_ChemCamHead", "Hover_FL_VectorVane_0" })
            {
                var t = rover.GetComponentsInChildren<Renderer>(true).FirstOrDefault(x => x.name == part);
                if (!t) { log.Add($"pivot test {part}: NOT FOUND"); continue; }
                var before = t.bounds.center; var rot = t.transform.localRotation;
                t.transform.Rotate(Vector3.right, 90f, Space.Self);
                yield return 2;
                var after = t.bounds.center;
                log.Add($"pivot test {part}: pivot at world {V(t.transform.position)}, part centre {V(before)}; rotating 90 deg about its own X moves the part centre by {(after - before).magnitude:F3} m (0 = pivots in place)");
                t.transform.localRotation = rot;
            }

            // Performance: fixed camera, rover on vs off.
            var view = c + new Vector3(d * .7f, s.y * .8f, d * .75f);
            cam.transform.position = view; cam.transform.LookAt(c);
            yield return Sample("showroom, ARES-VI LOD0 visible", 2f, 360);
            rover.SetActive(false);
            yield return Sample("showroom, rover hidden (empty stage)", 2f, 360);
            rover.SetActive(true);
            log.Add("=== showroom run finished");
        }

        static IEnumerator BaselineRun()
        {
            log.Add("=== Baseline: " + BaselineScene + " (opened read-only, Play Mode, not saved) quality '" + QualitySettings.names[QualitySettings.GetQualityLevel()] + "' targetFrameRate " + Application.targetFrameRate + " vSync " + QualitySettings.vSyncCount);
            yield return 8f;
            log.Add("after init: quality '" + QualitySettings.names[QualitySettings.GetQualityLevel()] + "' targetFrameRate " + Application.targetFrameRate + " vSync " + QualitySettings.vSyncCount);
            yield return Sample("AstroGateway Visual Upgrade Mars scene, default gameplay camera, idle", 2f, 360);
            log.Add("=== baseline finished");
        }

        static IEnumerator Sample(string label, float settle, int frames)
        {
            yield return settle;
            var draw = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
            var batches = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Batches Count");
            var setpass = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count");
            var tris = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count");
            var verts = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Vertices Count");
            var ft = new List<double>(); var gpu = new List<double>(); var dc = new List<long>(); var bc = new List<long>(); var sp = new List<long>(); var tc = new List<long>(); var vc = new List<long>();
            var timings = new FrameTiming[1];
            int last = Time.frameCount;
            while (ft.Count < frames)
            {
                yield return 1;
                if (Time.frameCount == last) continue;
                last = Time.frameCount;
                ft.Add(Time.unscaledDeltaTime * 1000.0);
                FrameTimingManager.CaptureFrameTimings();
                if (FrameTimingManager.GetLatestTimings(1, timings) > 0 && timings[0].gpuFrameTime > 0) gpu.Add(timings[0].gpuFrameTime);
                if (draw.Valid) dc.Add(draw.LastValue); if (batches.Valid) bc.Add(batches.LastValue); if (setpass.Valid) sp.Add(setpass.LastValue);
                if (tris.Valid) tc.Add(tris.LastValue); if (verts.Valid) vc.Add(verts.LastValue);
            }
            string Stat(List<double> l) { if (l.Count == 0) return "unavailable"; var o = l.OrderBy(x => x).ToList(); return $"mean {l.Average():F2} p50 {o[o.Count / 2]:F2} p95 {o[(int)(o.Count * .95)]:F2} max {o.Last():F2} ms (n={l.Count})"; }
            string Med(List<long> l) => l.Count == 0 ? "unavailable" : l.OrderBy(x => x).ElementAt(l.Count / 2).ToString();
            log.Add($"--- perf: {label}");
            log.Add($"frame time {Stat(ft)} -> FPS {(1000.0 / ft.Average()):F1}; Game View {Screen.width}x{Screen.height}");
            log.Add($"GPU frame time {Stat(gpu)}");
            log.Add($"profiler (median): draw calls {Med(dc)}, batches {Med(bc)}, SetPass {Med(sp)}, triangles {Med(tc)}, vertices {Med(vc)}");
            log.Add($"UnityStats (editor, last frame): drawCalls {UnityStats.drawCalls} setPass {UnityStats.setPassCalls} tris {UnityStats.triangles} verts {UnityStats.vertices}");
            log.Add($"memory: total allocated {Profiler.GetTotalAllocatedMemoryLong() / 1048576f:F1} MB, reserved {Profiler.GetTotalReservedMemoryLong() / 1048576f:F1} MB, graphics driver {Profiler.GetAllocatedMemoryForGraphicsDriver() / 1048576f:F1} MB, mono used {Profiler.GetMonoUsedSizeLong() / 1048576f:F1} MB");
            draw.Dispose(); batches.Dispose(); setpass.Dispose(); tris.Dispose(); verts.Dispose();
        }
    }
}
