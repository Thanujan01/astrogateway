using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Profiling;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace AstroGateway.VehicleShowroom.EditorTools
{
    // Graphical-Editor verification of the upgraded ARES-VI in the showroom: real Game View captures (ScreenCapture),
    // before/after against the untouched original import, animation clip tests (sampled and live Animator playback),
    // performance (upgraded / original / empty stage), then the Visual Upgrade Mars scene as baseline (never saved).
    //   Unity -projectPath <p> -executeMethod AstroGateway.VehicleShowroom.EditorTools.AresViUpgradeVerify.Run  (ARES_QUIT=1 to exit)
    [InitializeOnLoad]
    public static class AresViUpgradeVerify
    {
        const string StageKey = "AresViUpgradeVerify.Stage";
        const string BaselineScene = "Assets/AstroGateway/VisualUpgrade/Scenes/Mars_VisualUpgrade_v1.unity";
        static string Out => Path.GetFullPath("Docs/VehicleShowroom/ARES_VI/Upgrade");
        static IEnumerator routine; static readonly Stack<IEnumerator> stack = new Stack<IEnumerator>();
        static double waitUntil; static int frameWait;
        static readonly List<string> log = new List<string>();

        static AresViUpgradeVerify() { EditorApplication.update += Tick; }

        public static void Run()
        {
            Directory.CreateDirectory(Path.Combine(Out, "Screenshots"));
            File.Delete(Path.Combine(Out, "Verification_Log.txt"));
            Prepare(AresViShowroom.Scene);
            SessionState.SetInt(StageKey, 1);
            EditorApplication.isPlaying = true;
        }

        static void Prepare(string scene)
        {
            EditorSceneManager.OpenScene(scene);
            try { PlayModeWindow.SetViewType(PlayModeWindow.PlayModeViewTypes.GameView); } catch { }
            try { PlayModeWindow.SetCustomRenderingResolution(1512, 982, "ARES-VI Showroom"); } catch { }
            try { ProductionViewport.SetSize(1512, 982); } catch (Exception e) { Debug.LogWarning("Game View size: " + e.Message); }
            Selection.activeObject = null;
            try
            {
                var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView");
                var view = EditorWindow.GetWindow(type, false, null, false);
                type.GetField("m_Gizmos", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(view, false);
                view.Repaint();
            }
            catch (Exception e) { log.Add("gizmo toggle failed: " + e.Message); }
            Application.runInBackground = true;
            EditorApplication.isPaused = false;
            // Session-only: compile shader variants synchronously so captures never show the cyan async placeholder.
            ShaderUtil.allowAsyncCompilation = false;
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
                    if (stage == 1) routine = Showroom(); else if (stage == 3) routine = Baseline(); else return;
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
            File.AppendAllLines(Path.Combine(Out, "Verification_Log.txt"), log); log.Clear();
            SessionState.SetInt(StageKey, stage == 1 ? 2 : 4);
            EditorApplication.isPlaying = false;
        }

        static string V(Vector3 v) => $"({v.x:F3}, {v.y:F3}, {v.z:F3})";
        static Camera cam;
        static IEnumerator Shot(string name, Vector3 pos, Vector3 target, string note, float settle = 1f)
        {
            cam.transform.position = pos; cam.transform.LookAt(target);
            yield return settle;
            int guard = 0; while (ShaderUtil.anythingCompiling && guard++ < 60) yield return .25f;
            string path = Path.Combine(Out, "Screenshots", name);
            ScreenCapture.CaptureScreenshot(path);
            yield return 1.0f;
            log.Add($"shot {name}: {note} [{Screen.width}x{Screen.height}, saved={File.Exists(path)}]");
        }

        static Bounds RB(GameObject g, string prefix = null)
        {
            var rs = g.GetComponentsInChildren<Renderer>(true).Where(r => r.enabled && r.gameObject.activeInHierarchy && (prefix == null || r.name.StartsWith(prefix))).ToList();
            if (rs.Count == 0) return new Bounds(Vector3.zero, Vector3.zero);
            var b = rs[0].bounds; foreach (var r in rs) b.Encapsulate(r.bounds); return b;
        }

        static float Lowest(GameObject g)
        {
            float y = float.MaxValue;
            foreach (var f in g.GetComponentsInChildren<MeshFilter>(true))
            {
                if (!f.gameObject.activeInHierarchy || f.transform.lossyScale.y < .01f) continue;
                var m = f.transform.localToWorldMatrix; foreach (var v in f.sharedMesh.vertices) y = Mathf.Min(y, m.MultiplyPoint3x4(v).y);
            }
            return y;
        }

        static GameObject rover, original, studioFloor, mars; static Light[] studioLights;
        static Material studioSky; static AmbientMode studioAmbient; static Color sA, eA, gA;
        static Dictionary<Transform, (Vector3 p, Quaternion r, Vector3 s)> rest;

        static void RestorePose()
        {
            foreach (var kv in rest) { kv.Key.localPosition = kv.Value.p; kv.Key.localRotation = kv.Value.r; kv.Key.localScale = kv.Value.s; }
            foreach (var r in rover.GetComponentsInChildren<Renderer>(true)) r.SetPropertyBlock(null);
        }

        static void MarsPreset(bool on)
        {
            mars.SetActive(on); studioFloor.SetActive(!on);
            foreach (var l in studioLights) l.gameObject.SetActive(!on);
            if (on)
            {
                RenderSettings.skybox = AssetDatabase.LoadAssetAtPath<Material>(AresViUpgrade.Dir + "/Materials/ARES2_MarsSky.mat");
                RenderSettings.ambientMode = AmbientMode.Trilight;
                RenderSettings.ambientSkyColor = new Color(.62f, .48f, .38f); RenderSettings.ambientEquatorColor = new Color(.50f, .36f, .26f); RenderSettings.ambientGroundColor = new Color(.24f, .16f, .11f);
                RenderSettings.fog = true; RenderSettings.fogMode = FogMode.ExponentialSquared; RenderSettings.fogDensity = .012f; RenderSettings.fogColor = new Color(.72f, .52f, .38f);
                RenderSettings.sun = mars.GetComponentInChildren<Light>(true);
            }
            else
            {
                RenderSettings.skybox = studioSky; RenderSettings.ambientMode = studioAmbient;
                RenderSettings.ambientSkyColor = sA; RenderSettings.ambientEquatorColor = eA; RenderSettings.ambientGroundColor = gA; RenderSettings.fog = false;
                RenderSettings.sun = studioLights.FirstOrDefault(l => l.name == "Key Light");
            }
            DynamicGI.UpdateEnvironment();
        }

        static IEnumerator Showroom()
        {
            log.Add("=== ARES-VI UPGRADED showroom run " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            log.Add($"Unity {Application.unityVersion} | GPU {SystemInfo.graphicsDeviceName} | API {SystemInfo.graphicsDeviceType} | quality '{QualitySettings.names[QualitySettings.GetQualityLevel()]}' | pipeline {GraphicsSettings.currentRenderPipeline?.name} | vSync {QualitySettings.vSyncCount} targetFrameRate {Application.targetFrameRate}");
            yield return 1.5f;
            cam = Camera.main;
            var roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            rover = roots.First(g => g.name == "ARES_VI_Upgraded");
            original = roots.First(g => g.name == "ARES_VI_Showroom_Root");
            mars = roots.First(g => g.name == "Mars Preset");
            studioFloor = GameObject.Find("Turntable Floor");
            studioLights = roots.First(g => g.name == "Showroom Stage").GetComponentsInChildren<Light>(true);
            studioSky = RenderSettings.skybox; studioAmbient = RenderSettings.ambientMode; sA = RenderSettings.ambientSkyColor; eA = RenderSettings.ambientEquatorColor; gA = RenderSettings.ambientGroundColor;
            var animator = rover.GetComponent<Animator>();
            rest = rover.GetComponentsInChildren<Transform>(true).ToDictionary(t => t, t => (t.localPosition, t.localRotation, t.localScale));
            Transform T(string n) => rover.GetComponentsInChildren<Transform>(true).First(t => t.name == n);

            // --- structure and materials
            var rends = rover.GetComponentsInChildren<MeshRenderer>(true);
            var slots = rends.SelectMany(r => r.sharedMaterials).ToList();
            var mats = slots.Where(m => m).Distinct().ToList();
            var texs = mats.SelectMany(m => m.GetTexturePropertyNameIDs().Select(id => m.GetTexture(id))).Where(t => t).Distinct().ToList();
            int tris = rover.GetComponentsInChildren<MeshFilter>(true).Sum(f => f.sharedMesh.triangles.Length / 3);
            int verts = rover.GetComponentsInChildren<MeshFilter>(true).Sum(f => f.sharedMesh.vertexCount);
            long texMem = texs.Sum(t => Profiler.GetRuntimeMemorySizeLong(t)), meshMem = rover.GetComponentsInChildren<MeshFilter>(true).Sum(f => Profiler.GetRuntimeMemorySizeLong(f.sharedMesh));
            log.Add($"structure: {rover.GetComponentsInChildren<Transform>(true).Length} transforms, {rends.Length} renderers, {slots.Count} material slots, {mats.Count} distinct materials, missing {slots.Count(m => !m)}, non-URP {mats.Count(m => !m.shader.name.StartsWith("Universal Render Pipeline"))}, triangles {tris}, vertices {verts}");
            log.Add($"textures {texs.Count}: " + string.Join(", ", texs.Select(t => $"{t.name} {t.width}x{t.height} {(t as Texture2D)?.format}")));
            log.Add($"memory: textures {texMem / 1048576f:F1} MB, meshes {meshMem / 1048576f:F1} MB");
            int opp = 0, total = 0;
            foreach (var f in rover.GetComponentsInChildren<MeshFilter>(true))
            {
                var m = f.sharedMesh; var v = m.vertices; var n = m.normals; var t = m.triangles; total += t.Length / 3;
                for (int i = 0; i < t.Length; i += 3) if (Vector3.Dot(Vector3.Cross(v[t[i + 1]] - v[t[i]], v[t[i + 2]] - v[t[i]]), n[t[i]] + n[t[i + 1]] + n[t[i + 2]]) < 0) opp++;
            }
            log.Add($"normals vs winding: {opp} of {total} triangles oppose ({100f * opp / total:F2}%)");
            var b = RB(rover); log.Add($"bounds: centre {V(b.center)} size {V(b.size)}, lowest vertex y {Lowest(rover):F3}");
            var canopyR = rends.First(r => r.name == "Body");

            // --- before / after
            var c = b.center; float d = Mathf.Max(b.size.x, b.size.z) * 1.75f;
            var tq = c + new Vector3(d * .7f, b.size.y * .8f, d * .75f);
            rover.SetActive(false); original.SetActive(true);
            yield return Shot("B06_original_three_quarter.png", tq, c, "BEFORE: original import, same camera, same post-processing");
            original.SetActive(false); rover.SetActive(true);
            yield return Shot("U06_three_quarter.png", tq, c, "AFTER: upgraded");

            // --- views
            yield return Shot("U01_front.png", c + new Vector3(0, .35f, d), c, "front");
            yield return Shot("U02_rear.png", c + new Vector3(0, .35f, -d), c, "rear");
            yield return Shot("U03_left.png", c + new Vector3(-d, .35f, 0), c, "left");
            yield return Shot("U04_right.png", c + new Vector3(d, .35f, 0), c, "right");
            yield return Shot("U05_top.png", c + new Vector3(0, d * 1.6f, .01f), c, "top");
            studioFloor.SetActive(false);
            yield return Shot("U07_undercarriage.png", c + new Vector3(.4f, -d * .9f, d * .35f), c, "undercarriage (floor hidden)");
            yield return Shot("U12_hover_modules_below.png", new Vector3(.8f, -2.1f, 1.2f), new Vector3(0, .45f, 0), "hover modules from below (floor hidden)");
            studioFloor.SetActive(true);
            yield return Shot("U08_cockpit.png", new Vector3(.9f, 2.35f, 2.0f), new Vector3(0, 1.25f, .4f), "cockpit / canopy");
            yield return Shot("U09_suspension_left.png", new Vector3(-3.3f, .95f, .9f), new Vector3(-.95f, .62f, 0), "left rocker-bogie");
            yield return Shot("U10_science_arm.png", new Vector3(-1.2f, 1.25f, 3.4f), new Vector3(-.44f, .7f, 1.6f), "science arm (deployed rest pose)");
            yield return Shot("U11_chemcam_mast.png", new Vector3(1.9f, 2.55f, 1.0f), new Vector3(.42f, 2.3f, -.55f), "ChemCam mast (rear-right deck)");
            yield return Shot("U13_wheel_closeup.png", new Vector3(-2.3f, .6f, 2.0f), new Vector3(-1.26f, .38f, 1.18f), "front-left wheel / new tyre");

            // --- animation tests
            log.Add("--- animation tests");
            var ctrlClips = animator.runtimeAnimatorController.animationClips;
            animator.enabled = false;
            var results = new List<string>();
            AnimationClip Clip(string n) => ctrlClips.FirstOrDefault(x => x.name == n);
            float Ang(Transform t, int axis) { var e = t.localEulerAngles[axis]; return e > 180 ? e - 360 : e; }

            // Wheel rotation
            {
                var clip = Clip("ARES_WheelRotation"); var w = T("Wheel_FL"); var cent0 = RB(w.gameObject).center; float minY = 9, maxDrift = 0;
                for (float t = 0; t <= clip.length + 1e-3f; t += .1f) { clip.SampleAnimation(rover, t); minY = Mathf.Min(minY, Lowest(w.gameObject)); maxDrift = Mathf.Max(maxDrift, (RB(w.gameObject).center - cent0).magnitude); }
                clip.SampleAnimation(rover, .5f); float a = Ang(w, 0);
                results.Add($"Wheel rotation: clip {clip.length:F1}s loop={clip.isLooping}; Wheel_FL X at 0.5 s = {a:F1} deg (expect 90); wheel centre drift over the cycle {maxDrift * 1000:F1} mm; tyre lowest point {minY:F3} m");
                RestorePose();
            }
            // Rocker-bogie
            {
                var clip = Clip("ARES_RockerBogie"); clip.SampleAnimation(rover, 1f);
                float r = Ang(T("Rocker_L"), 0), bo = Ang(T("Bogie_L"), 0), rr = Ang(T("Rocker_R"), 0);
                var hub = T("Wheel_FL").position; var mid = T("Wheel_ML").position; float low = 9;
                for (float t = 0; t <= clip.length + 1e-3f; t += .1f) { clip.SampleAnimation(rover, t); low = Mathf.Min(low, Lowest(rover)); }
                clip.SampleAnimation(rover, 1f);
                results.Add($"Rocker-bogie: at 1.0 s Rocker_L {r:F1} deg, Bogie_L {bo:F1} deg, Rocker_R {rr:F1} deg; FL hub y {hub.y:F3} (rest .380), ML hub y {mid.y:F3}; lowest vertex over the cycle {low:F3} m");
                yield return Shot("A02_rocker_bogie_articulated.png", new Vector3(-3.6f, 1.0f, .5f), new Vector3(-1f, .55f, 0), "rocker-bogie at 1.0 s (left side articulated)");
                RestorePose();
            }
            // Arm deployment
            {
                var clip = Clip("ARES_ArmDeploy"); float minTip = 9; var body = canopyR.bounds; int clash = 0;
                var drill = T("Arm_Drill");
                for (float t = 0; t <= clip.length + 1e-3f; t += .1f)
                {
                    clip.SampleAnimation(rover, t);
                    var tip = RB(drill.gameObject); minTip = Mathf.Min(minTip, tip.min.y);
                    foreach (var wn in new[] { "Wheel_FL", "Wheel_FR" }) if (RB(T("Arm_Fore").gameObject).Intersects(RB(T(wn).gameObject))) clash++;
                }
                clip.SampleAnimation(rover, 0); var tip0 = RB(drill.gameObject).center;
                yield return Shot("A03a_arm_stowed.png", new Vector3(1.6f, 2.0f, 4.0f), new Vector3(.2f, 1.0f, 1.1f), "arm stowed (t = 0)");
                clip.SampleAnimation(rover, 2.6f);
                yield return Shot("A03b_arm_unfolding.png", new Vector3(1.6f, 2.0f, 4.4f), new Vector3(-.3f, 1.0f, 1.6f), "arm unfolding (t = 2.6 s)");
                clip.SampleAnimation(rover, clip.length); var tip1 = RB(drill.gameObject).center;
                results.Add($"Science arm deployment: {clip.length:F1}s; drill moves {V(tip0)} -> {V(tip1)}; lowest drill point during deploy {minTip:F3} m; forearm/front-wheel bounds overlaps {clash} of {Mathf.RoundToInt(clip.length / .1f) * 2 + 2} samples");
                RestorePose();
            }
            // ChemCam
            {
                var clip = Clip("ARES_ChemCamScan"); clip.SampleAnimation(rover, 1.2f);
                float pan = Ang(T("Mast_Pan"), 1), tilt = Ang(T("Mast_Tilt"), 0); var head = RB(T("Mast_Tilt").gameObject).center;
                yield return Shot("A04_chemcam_panned.png", new Vector3(2.4f, 3.0f, 1.6f), new Vector3(.42f, 2.4f, -.55f), "ChemCam panned -55 deg / tilted (t = 1.2 s)");
                RestorePose(); var head0 = RB(T("Mast_Tilt").gameObject).center;
                results.Add($"ChemCam movement: {clip.length:F1}s loop; at 1.2 s pan {pan:F1} deg, tilt {tilt:F1} deg; head centre moves {(head - head0).magnitude * 1000:F0} mm; laser pulse keys on the emission colour (2.4 s, 4.8 s)");
            }
            // Hover activation / stabilisation / landing
            {
                var act = Clip("ARES_HoverActivation"); act.SampleAnimation(rover, act.length);
                var lowA = Lowest(rover); var ch = T("Chassis").localPosition.y; var plume = T("Hover_FL_Plume").localScale;
                var mpb = new MaterialPropertyBlock(); T("Hover_FL").GetComponent<Renderer>().GetPropertyBlock(mpb); var em = mpb.GetColor("_EmissionColor");
                yield return Shot("A05_hover_active.png", tq, c + Vector3.up * .3f, "hover activation end (t = 3.0 s)");
                results.Add($"Hover activation: {act.length:F1}s; chassis y {ch:F3} m, lowest vertex {lowA:F3} m (0 = on ground), FL plume scale {V(plume)}, FL emission (animated) {em.r:F2},{em.g:F2},{em.b:F2}");
                RestorePose();
                var stab = Clip("ARES_HoverStabilization"); float ymin = 9, ymax = -9, pitch = 0;
                for (float t = 0; t <= stab.length; t += .1f) { stab.SampleAnimation(rover, t); var y = T("Chassis").localPosition.y; ymin = Mathf.Min(ymin, y); ymax = Mathf.Max(ymax, y); pitch = Mathf.Max(pitch, Mathf.Abs(Ang(T("Chassis"), 0))); }
                studioFloor.SetActive(false);
                stab.SampleAnimation(rover, 1f);
                yield return Shot("A06_hover_from_below.png", new Vector3(1.4f, -1.6f, 2.6f), new Vector3(0, .7f, 0), "hover stabilisation from below, plumes visible (floor hidden)");
                studioFloor.SetActive(true);
                results.Add($"Hover stabilization: {stab.length:F1}s loop={stab.isLooping}; chassis y {ymin:F3}..{ymax:F3} m, max pitch {pitch:F1} deg");
                RestorePose();
                var land = Clip("ARES_Landing"); float lowest = 9;
                for (float t = 0; t <= land.length + 1e-3f; t += .05f) { land.SampleAnimation(rover, t); lowest = Mathf.Min(lowest, Lowest(rover)); }
                land.SampleAnimation(rover, land.length); var endY = T("Chassis").localPosition.y;
                land.SampleAnimation(rover, 1.2f);
                yield return Shot("A07_landing_descent.png", tq, c + Vector3.up * .2f, "landing descent (t = 1.2 s)");
                results.Add($"Landing: {land.length:F1}s; ends at chassis y {endY:F3} m; lowest vertex during landing {lowest:F3} m");
                RestorePose();
            }
            // Live Animator playback of every state.
            animator.enabled = true;
            foreach (var name in AresViUpgrade.ClipNames)
            {
                RestorePose();
                var snapshot = rover.GetComponentsInChildren<Transform>(true).ToDictionary(t => t, t => (t.localPosition, t.localRotation, t.localScale));
                animator.Play(name, 0, 0f);
                yield return 1.2f;
                int moved = snapshot.Count(kv => (kv.Key.localPosition - kv.Value.Item1).sqrMagnitude > 1e-6f || Quaternion.Angle(kv.Key.localRotation, kv.Value.Item2) > .5f || (kv.Key.localScale - kv.Value.Item3).sqrMagnitude > 1e-6f);
                var info = animator.GetCurrentAnimatorStateInfo(0);
                results.Add($"live Animator '{name}': state active {info.IsName(name)}, normalized time {info.normalizedTime:F2}, transforms moved {moved}");
            }
            animator.Play("Idle", 0, 0f); yield return .5f; animator.enabled = false; RestorePose();
            log.AddRange(results);

            // --- Mars preset
            MarsPreset(true);
            yield return Shot("M01_mars_three_quarter.png", new Vector3(4.6f, 1.6f, 5.2f), new Vector3(0, 1.0f, 0), "Mars preset, ground pose", 2f);
            var stabClip = Clip("ARES_HoverStabilization"); stabClip.SampleAnimation(rover, 1f);
            yield return Shot("M02_mars_hover.png", new Vector3(-4.8f, 1.4f, 4.6f), new Vector3(0, 1.1f, 0), "Mars preset, hover stabilisation pose", 1.5f);
            RestorePose();
            Clip("ARES_ArmDeploy").SampleAnimation(rover, 4.8f);
            yield return Shot("M03_mars_front_low.png", new Vector3(1.4f, .7f, 5.2f), new Vector3(-.2f, 1.0f, 0), "Mars preset, low front view", 1.5f);
            RestorePose();
            MarsPreset(false);

            // --- performance (studio, fixed camera)
            cam.transform.position = tq; cam.transform.LookAt(c);
            yield return Sample("showroom, ARES-VI UPGRADED visible", 2f, 360);
            rover.SetActive(false); original.SetActive(true);
            yield return Sample("showroom, ORIGINAL import visible (same post-processing)", 2f, 360);
            original.SetActive(false);
            yield return Sample("showroom, empty stage", 2f, 360);
            rover.SetActive(true);
            animator.enabled = true; animator.Play("ARES_HoverStabilization", 0, 0);
            yield return Sample("showroom, ARES-VI UPGRADED animating (hover stabilisation)", 1f, 360);
            log.Add("=== showroom run finished");
        }

        static IEnumerator Baseline()
        {
            log.Add("=== Baseline: " + BaselineScene + " (Play Mode, not saved)");
            yield return 8f;
            yield return Sample("AstroGateway Visual Upgrade Mars scene, idle", 2f, 360);
            log.Add("=== baseline finished");
        }

        static IEnumerator Sample(string label, float settle, int frames)
        {
            yield return settle;
            var setpass = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count");
            var ft = new List<double>(); var gpu = new List<double>(); var dc = new List<int>(); var tri = new List<int>(); var sp = new List<long>();
            var timings = new FrameTiming[1]; int last = Time.frameCount;
            while (ft.Count < frames)
            {
                yield return 1;
                if (Time.frameCount == last) continue;
                last = Time.frameCount;
                ft.Add(Time.unscaledDeltaTime * 1000.0);
                FrameTimingManager.CaptureFrameTimings();
                if (FrameTimingManager.GetLatestTimings(1, timings) > 0 && timings[0].gpuFrameTime > 0) gpu.Add(timings[0].gpuFrameTime);
                dc.Add(UnityStats.drawCalls); tri.Add(UnityStats.triangles); if (setpass.Valid) sp.Add(setpass.LastValue);
            }
            string Stat(List<double> l) { if (l.Count == 0) return "unavailable"; var o = l.OrderBy(x => x).ToList(); return $"mean {l.Average():F2} p50 {o[o.Count / 2]:F2} p95 {o[(int)(o.Count * .95)]:F2} max {o.Last():F2} ms (n={l.Count})"; }
            T Med<T>(List<T> l) => l.Count == 0 ? default : l.OrderBy(x => x).ElementAt(l.Count / 2);
            log.Add($"--- perf: {label}");
            log.Add($"frame time {Stat(ft)} -> FPS {(1000.0 / ft.Average()):F1}; Game View {Screen.width}x{Screen.height}");
            log.Add($"GPU frame time {Stat(gpu)}");
            log.Add($"draw calls (median) {Med(dc)}, SetPass {Med(sp)}, triangles {Med(tri)}");
            log.Add($"memory: total allocated {Profiler.GetTotalAllocatedMemoryLong() / 1048576f:F1} MB, graphics driver {Profiler.GetAllocatedMemoryForGraphicsDriver() / 1048576f:F1} MB");
            setpass.Dispose();
        }
    }
}
