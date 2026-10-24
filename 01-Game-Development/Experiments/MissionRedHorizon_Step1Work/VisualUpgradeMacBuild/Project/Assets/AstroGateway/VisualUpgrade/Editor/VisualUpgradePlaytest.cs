using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Profiling;
using AstroGateway.CinematicHUD;
using AstroGateway.NativeSlice;
using AstroGateway.ProductionVisual;
using AstroGateway.Stage08B;

// Copy of Verification/Editor/LaptopPlaytest.cs retargeted at Mars_VisualUpgrade_v1, in the GRAPHICAL Unity Editor.
// Adds the Crew Dome airlock camera check (the laptop playtest FAIL) and per-preset FPS on the new scene.
// Opens the scene, enters Play Mode, drives the game through the real keyboard/mouse input path
// (virtual Input System devices -> HUDInputRouter / ProductionModeCoordinator), captures Game View
// screenshots, measures frame times, then leaves Play Mode with the scene still open.
// Never saves the scene, never calls Save/Load, restores Input System settings it touches.
// Output folder: env VU_OUT, default <project>/Docs/VisualUpgrade/Playtest. Env VU_QUIT=1 quits the Editor when done.
namespace AstroGateway.VisualUpgrade.EditorTools
{
    [InitializeOnLoad]
    public static class VisualUpgradePlaytest
    {
        static string Scene => Environment.GetEnvironmentVariable("VU_SCENE") ?? "Assets/AstroGateway/VisualUpgrade/Scenes/Mars_VisualUpgrade_v1.unity"; // VU_SCENE allows the same harness on the original scene for before/after
        const string Flag = "VisualUpgradePlaytest.Run";
        const string LoadKey = "VisualUpgradePlaytest.SceneLoadMs";
        const string EnterKey = "VisualUpgradePlaytest.EnterRequested";
        const int Width = 1512, Height = 982; // 14" MacBook Pro default logical resolution
        static string Out => Environment.GetEnvironmentVariable("VU_OUT") ?? Path.GetFullPath("Docs/VisualUpgrade/Playtest");
        static string Shots => Path.Combine(Out, "Screenshots");

        static readonly List<string> lines = new List<string>();
        static readonly List<string> perf = new List<string>();
        static readonly List<string> hudAudit = new List<string>();
        static readonly List<string> logErrors = new List<string>();
        static readonly List<string> logWarnings = new List<string>();
        static int pass, fail, untested;
        static readonly Stack<IEnumerator> stack = new Stack<IEnumerator>();
        static IEnumerator routine;
        static object waiting;
        static int lastFrame = -1;
        static double startedAt;

        static AstroGatewayHUDController hud;
        static ProductionModeCoordinator modes;
        static ProductionCameraRig rig;
        static ProductionPresentation art;
        static MarsPrototype p;
        static Keyboard keyboard;
        static Mouse mouse;
        static InputSettings.BackgroundBehavior priorBackground;
        static InputSettings.EditorInputBehaviorInPlayMode priorEditorInput;
        static bool inputPatched;
        static int priorVisibility = -1;
        static bool f1Persisted;

        // Per-frame sampling
        static bool sampling;
        static readonly List<double> frameMs = new List<double>();
        static int belowGround, cameraInside, frames;
        static readonly List<string> cameraSamples = new List<string>();

        static VisualUpgradePlaytest() { EditorApplication.update += Tick; }

        [MenuItem("AstroGateway/Visual Upgrade Playtest (v1)")]
        public static void Run()
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            EditorSceneManager.OpenScene(Scene);
            sw.Stop();
            SessionState.SetFloat(LoadKey, (float)sw.Elapsed.TotalMilliseconds);
            try { PlayModeWindow.SetViewType(PlayModeWindow.PlayModeViewTypes.GameView); } catch (Exception e) { Debug.LogWarning("[VisualUpgrade] Game View select: " + e.Message); }
            try { PlayModeWindow.SetCustomRenderingResolution(Width, Height, "AstroGateway Visual Upgrade"); } catch (Exception e) { Debug.LogWarning("[VisualUpgrade] resolution: " + e.Message); }
            try { ProductionViewport.SetSize(Width, Height); } catch (Exception e) { Debug.LogWarning("[VisualUpgrade] Game View size: " + e.Message); }
            SessionState.SetBool(Flag, true);
            SessionState.SetFloat(EnterKey, (float)EditorApplication.timeSinceStartup);
            Application.runInBackground = true;
            EditorApplication.isPaused = false;
            EditorApplication.isPlaying = true;
        }

        static void Tick()
        {
            if (!EditorApplication.isPlaying) return;
            try
            {
                if (routine == null)
                {
                    if (!SessionState.GetBool(Flag, false)) return;
                    hud = UnityEngine.Object.FindAnyObjectByType<AstroGatewayHUDController>();
                    if (!hud || !hud.safe || !hud.game || !hud.game.initialized) return;
                    SessionState.SetBool(Flag, false);
                    modes = UnityEngine.Object.FindAnyObjectByType<ProductionModeCoordinator>();
                    rig = UnityEngine.Object.FindAnyObjectByType<ProductionCameraRig>();
                    art = UnityEngine.Object.FindAnyObjectByType<ProductionPresentation>();
                    p = hud.game.prototype;
                    Application.logMessageReceived += OnLog;
                    startedAt = EditorApplication.timeSinceStartup;
                    routine = Tests();
                    stack.Push(routine);
                }
                if (EditorApplication.timeSinceStartup - startedAt > 900) throw new Exception("Playtest timeout (15 min)");
                if (Time.frameCount == lastFrame) return;
                lastFrame = Time.frameCount;
                if (sampling) Sample();
                if (!Ready()) return;
                waiting = null;
                while (stack.Count > 0)
                {
                    var top = stack.Peek();
                    if (!top.MoveNext()) { stack.Pop(); continue; }
                    if (top.Current is IEnumerator nested) { stack.Push(nested); continue; }
                    waiting = top.Current;
                    break;
                }
                if (stack.Count == 0) Finish();
            }
            catch (Exception e)
            {
                Line("EXCEPTION: " + e);
                fail++;
                Finish();
            }
        }

        class Realtime { public double until; }
        class SimWait { public float until; public double deadline; }
        static object Sim(float s) => new SimWait { until = p.elapsed + s, deadline = EditorApplication.timeSinceStartup + s * 6 + 10 };
        static object Real(float s) => new Realtime { until = EditorApplication.timeSinceStartup + s };
        static bool Ready()
        {
            if (waiting == null) return true;
            if (waiting is SimWait w)
            {
                if (p.elapsed >= w.until) return true;
                if (EditorApplication.timeSinceStartup < w.deadline) return false;
                Line("WARN  simulation clock stalled during a wait: " + State());
                return true;
            }
            if (waiting is Realtime r) return EditorApplication.timeSinceStartup >= r.until;
            return true;
        }

        static void OnLog(string message, string st, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert) logErrors.Add(type + ": " + message.Split('\n')[0]);
            else if (type == LogType.Warning) logWarnings.Add(message.Split('\n')[0]);
        }

        static void Result(string status, string name, string detail = "")
        {
            if (status == "PASS") pass++; else if (status == "UNTESTED") untested++; else fail++;
            Line(status.PadRight(9) + name + (string.IsNullOrEmpty(detail) ? "" : "  [" + detail + "]"));
        }
        static void Check(string name, bool ok, string detail = "") => Result(ok ? "PASS" : "FAIL", name, detail);
        static void Line(string s) { lines.Add(s); Debug.Log("[VisualUpgrade] " + s); }
        static string V(Vector3 v) => $"({v.x:F2}, {v.y:F2}, {v.z:F2})";
        static string State() => $"mode {modes.Current}/{hud.game.modes.Mode}, driving {p.driving}, paused {hud.environment.paused}, modal {hud.modalOpen}, scanning {hud.scanning}, cinematic {modes.cinematic}, result '{hud.lastResult}'";

        // ---------- input through the real devices ----------
        static readonly HashSet<Key> held = new HashSet<Key>();
        static void Apply() { InputSystem.QueueStateEvent(keyboard, held.Count == 0 ? new KeyboardState() : new KeyboardState(held.ToArray())); }
        static void Hold(params Key[] keys) { foreach (var k in keys) held.Add(k); Apply(); }
        static void Release(params Key[] keys) { if (keys.Length == 0) held.Clear(); else foreach (var k in keys) held.Remove(k); Apply(); }
        static IEnumerator Tap(Key key)
        {
            Hold(key); yield return Real(0.08f);
            Release(key); yield return Real(0.12f);
        }

        // ---------- scene helpers (test setup only) ----------
        static bool OwnCollider(Collider c) => c == p.astronaut || c == p.rover || c.transform.IsChildOf(p.rover.transform) || c.transform.IsChildOf(p.astronaut.transform);
        static float GroundY(Vector3 pos)
        {
            var hits = Physics.RaycastAll(pos + Vector3.up * 30, Vector3.down, 80, 1 << 0, QueryTriggerInteraction.Ignore);
            var valid = hits.Where(h => !OwnCollider(h.collider)).OrderBy(h => h.distance).ToArray();
            return valid.Length > 0 ? valid[0].point.y : float.NaN;
        }
        static void Teleport(CharacterController c, Vector3 pos, float yawDeg)
        {
            float g = GroundY(pos);
            if (!float.IsNaN(g)) pos.y = g + 0.05f;
            c.enabled = false;
            c.transform.SetPositionAndRotation(pos, Quaternion.Euler(0, yawDeg, 0));
            c.enabled = true;
            Physics.SyncTransforms();
        }
        static Collider[] Overlaps(CharacterController c)
        {
            if (!c.enabled || !c.gameObject.activeInHierarchy) return new Collider[0];
            var t = c.transform;
            float scale = Mathf.Max(t.lossyScale.x, t.lossyScale.z);
            float r = c.radius * scale * 0.8f;
            float half = Mathf.Max(0, c.height * t.lossyScale.y / 2 - c.radius * scale);
            Vector3 centre = t.TransformPoint(c.center) + Vector3.up * 0.15f;
            return Physics.OverlapCapsule(centre + Vector3.up * half, centre - Vector3.up * half, r, 1 << 0, QueryTriggerInteraction.Ignore)
                .Where(x => !OwnCollider(x) && !(x is TerrainCollider)).ToArray();
        }
        static Vector3 OpenSpot(Vector3 hint, float clear = 7f)
        {
            for (int ring = 0; ring < 12; ring++)
                for (int i = 0; i < 8; i++)
                {
                    float ang = i * 45 * Mathf.Deg2Rad;
                    Vector3 c = hint + new Vector3(Mathf.Cos(ang), 0, Mathf.Sin(ang)) * ring * 6;
                    if (Mathf.Abs(c.x) > 105 || Mathf.Abs(c.z) > 105) continue;
                    float g = GroundY(c); if (float.IsNaN(g)) continue; c.y = g;
                    if (Physics.OverlapSphere(c + Vector3.up * 1.2f, clear, 1 << 0, QueryTriggerInteraction.Ignore).All(x => OwnCollider(x) || x is TerrainCollider || x.bounds.size.y < 0.3f || x.bounds.size.x > 200)) return c;
                }
            return hint;
        }
        static Collider LargestStructureNear(Vector3 from) =>
            UnityEngine.Object.FindObjectsByType<Collider>(FindObjectsSortMode.None)
                .Where(c => c.enabled && !c.isTrigger && !(c is TerrainCollider) && !OwnCollider(c) && c.gameObject.layer == 0)
                .Where(c => c.bounds.size.x > 6 && c.bounds.size.z > 6 && c.bounds.size.y > 2 && c.bounds.size.x < 60 && c.bounds.size.z < 60)
                .OrderBy(c => Vector3.Distance(from, c.bounds.center)).FirstOrDefault();
        static float YawTo(Vector3 from, Vector3 to) { var d = to - from; return Mathf.Atan2(d.x, d.z) * Mathf.Rad2Deg; }
        static UnityEngine.UI.Button FindButton(string name) => UnityEngine.Object.FindObjectsByType<UnityEngine.UI.Button>(FindObjectsSortMode.None).FirstOrDefault(b => b.name == name);

        static void Sample()
        {
            frames++;
            frameMs.Add(Time.unscaledDeltaTime * 1000.0);
            var active = p.ActivePosition();
            float g = GroundY(active);
            if (!float.IsNaN(g) && active.y < g - 0.5f) belowGround++;
            var cam = p.follow;
            if (cam && modes.Current != ProductionMode.Cinematic && !rig.captureOverride &&
                Physics.OverlapSphere(cam.transform.position, 0.1f, 1 << 0, QueryTriggerInteraction.Ignore).Any(x => !OwnCollider(x)))
            {
                cameraInside++;
                if (cameraSamples.Count < 15) cameraSamples.Add($"frame {Time.frameCount} mode {modes.Current} cam {V(cam.transform.position)} in " + string.Join(",", Physics.OverlapSphere(cam.transform.position, 0.1f, 1 << 0, QueryTriggerInteraction.Ignore).Where(x => !OwnCollider(x)).Select(x => x.name)));
            }
        }

        static IEnumerator Shot(string file, string caption)
        {
            yield return Real(0.15f);
            string path = Path.Combine(Shots, file);
            ScreenCapture.CaptureScreenshot(path);
            yield return Real(0.6f);
            bool ok = File.Exists(path) && new FileInfo(path).Length > 10000;
            Line((ok ? "SHOT     " : "SHOT-FAIL ") + file + " — " + caption + $"  [{Screen.width}x{Screen.height}, {State()}]");
        }

        static string Stats(List<double> ms)
        {
            if (ms.Count < 10) return "frames=" + ms.Count + " (too few)";
            var s = ms.OrderBy(x => x).ToList();
            double mean = s.Average();
            double sd = Math.Sqrt(s.Select(x => (x - mean) * (x - mean)).Average());
            return $"frames={s.Count} mean_ms={mean:F2} fps={1000 / mean:F1} p50_ms={s[s.Count / 2]:F2} p95_ms={s[(int)(s.Count * .95)]:F2} p99_ms={s[(int)(s.Count * .99)]:F2} max_ms={s.Last():F2} stdev_ms={sd:F2} frames_over_50ms={s.Count(x => x > 50)}";
        }

        static IEnumerator Measure(string label, float seconds)
        {
            // Measure with the Game View focused (keyboard focus inside the Editor), as a player would have it.
            try { GameView().Focus(); } catch { }
            yield return Real(0.5f);
            frameMs.Clear();
            FrameTimingManager.CaptureFrameTimings();
            yield return Real(seconds);
            var copy = new List<double>(frameMs);
            var timings = new FrameTiming[1];
            FrameTimingManager.CaptureFrameTimings();
            uint n = FrameTimingManager.GetLatestTimings(1, timings);
            string ft = n > 0 && timings[0].cpuFrameTime > 0 ? $" cpu_frame_ms={timings[0].cpuFrameTime:F2} gpu_frame_ms={(timings[0].gpuFrameTime > 0 ? timings[0].gpuFrameTime.ToString("F2") : "unavailable")}" : " FrameTimingManager=unavailable";
            perf.Add($"{label}: {Stats(copy)}{ft} alloc_MB={Profiler.GetTotalAllocatedMemoryLong() / 1048576.0:F0} reserved_MB={Profiler.GetTotalReservedMemoryLong() / 1048576.0:F0} mono_MB={Profiler.GetMonoUsedSizeLong() / 1048576.0:F0} gfx_MB={Profiler.GetAllocatedMemoryForGraphicsDriver() / 1048576.0:F0} targetFrameRate={Application.targetFrameRate} vSync={QualitySettings.vSyncCount} focused={Application.isFocused}");
        }

        // ---------- HUD audit: visible UI text rectangles in screen pixels ----------
        static void AuditHud(string when)
        {
            var texts = UnityEngine.Object.FindObjectsByType<UnityEngine.UI.Text>(FindObjectsSortMode.None)
                .Where(t => t.isActiveAndEnabled && !string.IsNullOrWhiteSpace(t.text) && t.color.a > 0.05f && t.canvas && t.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                .Where(t => t.GetComponentsInParent<CanvasGroup>().All(g => g.alpha > 0.05f))
                .ToList();
            var rects = texts.Select(t => { var c = new Vector3[4]; t.rectTransform.GetWorldCorners(c); return (t, r: Rect.MinMaxRect(c[0].x, c[0].y, c[2].x, c[2].y)); }).ToList();
            hudAudit.Add($"--- {when}: {texts.Count} visible text elements at {Screen.width}x{Screen.height}");
            var screen = new Rect(0, 0, Screen.width, Screen.height);
            foreach (var (t, r) in rects)
            {
                var issues = new List<string>();
                if (!screen.Overlaps(r) || r.xMin < -2 || r.yMin < -2 || r.xMax > Screen.width + 2 || r.yMax > Screen.height + 2) issues.Add("OFF-SCREEN/EDGE");
                float scale = t.canvas.scaleFactor;
                if (t.verticalOverflow == VerticalWrapMode.Truncate && t.preferredHeight * scale > r.height + 2 && !t.resizeTextForBestFit) issues.Add($"CLIPPED-V pref {t.preferredHeight * scale:F0}px > {r.height:F0}px");
                if (t.horizontalOverflow == HorizontalWrapMode.Overflow && t.preferredWidth * scale > r.width + 4) issues.Add($"OVERFLOW-H pref {t.preferredWidth * scale:F0}px > {r.width:F0}px");
                int px = Mathf.RoundToInt((t.resizeTextForBestFit ? t.cachedTextGenerator.fontSizeUsedForBestFit / Mathf.Max(scale, 0.01f) : t.fontSize) * scale);
                if (px > 0 && px < 11) issues.Add($"SMALL {px}px");
                hudAudit.Add($"{(issues.Count > 0 ? "ISSUE " : "ok    ")}'{UiPath(t.transform)}' rect=({r.xMin:F0},{r.yMin:F0},{r.width:F0}x{r.height:F0}) font~{px}px text=\"{t.text.Replace("\n", " | ").Substring(0, Math.Min(90, t.text.Replace("\n", " | ").Length))}\" {string.Join("; ", issues)}");
            }
            for (int i = 0; i < rects.Count; i++)
                for (int j = i + 1; j < rects.Count; j++)
                {
                    var a = rects[i]; var b = rects[j];
                    if (a.t.transform.IsChildOf(b.t.transform) || b.t.transform.IsChildOf(a.t.transform)) continue;
                    var ix = Rect.MinMaxRect(Mathf.Max(a.r.xMin, b.r.xMin), Mathf.Max(a.r.yMin, b.r.yMin), Mathf.Min(a.r.xMax, b.r.xMax), Mathf.Min(a.r.yMax, b.r.yMax));
                    if (ix.width <= 0 || ix.height <= 0) continue;
                    float frac = ix.width * ix.height / Mathf.Max(1, Mathf.Min(a.r.width * a.r.height, b.r.width * b.r.height));
                    if (frac > 0.25f) hudAudit.Add($"OVERLAP {frac:P0} '{UiPath(a.t.transform)}' x '{UiPath(b.t.transform)}'");
                }
        }
        static EditorWindow GameView() => EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView"), false, null, false);
        static System.Reflection.FieldInfo GizmoField => typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView").GetField("m_Gizmos", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        static bool GameViewGizmos() { try { return (bool)GizmoField.GetValue(GameView()); } catch { return false; } }
        static void SetGameViewGizmos(bool on) { try { GizmoField.SetValue(GameView(), on); GameView().Repaint(); } catch (Exception e) { Line("WARN     gizmo toggle: " + e.Message); } }
        static string UiPath(Transform t) => t.parent ? (t.parent.parent ? t.parent.name + "/" : "") + t.name : t.name;

        // ---------- the playtest ----------
        static IEnumerator Tests()
        {
            Directory.CreateDirectory(Shots);
            float enterS = (float)(EditorApplication.timeSinceStartup - SessionState.GetFloat(EnterKey, 0));
            Line("Unity " + Application.unityVersion + " | GPU " + SystemInfo.graphicsDeviceName + " | API " + SystemInfo.graphicsDeviceType + " | CPU " + SystemInfo.processorType + " x" + SystemInfo.processorCount + " | RAM " + SystemInfo.systemMemorySize + " MB");
            Line("Scene: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().path);
            Line($"Scene open (Editor) {SessionState.GetFloat(LoadKey, 0):F0} ms; Play Mode request -> game initialized {enterS:F1} s (includes domain reload)");
            Line($"Game View framebuffer {Screen.width}x{Screen.height}; editor focused {Application.isFocused}; batch {Application.isBatchMode}");
            Line($"HUD settings: quality {hud.settings.quality}, visibility {hud.settings.visibility}; graphics preset readback {hud.environment.Readback()}");
            Line("Research targets: " + string.Join(", ", hud.environment.targets.Where(t => t).Select(t => t.displayName + " " + V(t.transform.position))));
            Line("Objective at start: " + p.objective);

            priorBackground = InputSystem.settings.backgroundBehavior;
            priorEditorInput = InputSystem.settings.editorInputBehaviorInPlayMode;
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            inputPatched = true;
            keyboard = InputSystem.AddDevice<Keyboard>("Visual upgrade playtest keyboard");
            mouse = InputSystem.AddDevice<Mouse>("Visual upgrade playtest mouse");
            keyboard.MakeCurrent(); mouse.MakeCurrent();
            hud.game.testing = false; // real input routers active
            priorVisibility = hud.settings.visibility;
            Line($"Saved HUD visibility profile is {priorVisibility} (0 = full). Switching to full HUD for this run only (ApplySettings, not persisted).");
            hud.settings.visibility = 0; hud.ApplySettings();
            sampling = true;

            yield return Real(2.5f);
            yield return Shot("01_game_launch.png", "first frames after Play Mode, spawn view");
            AuditHud("launch, default HUD");
            Check("Scene starts in astronaut mode", modes.Current == ProductionMode.Astronaut, State());
            Check("Astronaut and rover visible (renderers enabled)", p.astronaut.GetComponentsInChildren<Renderer>().Any(r => r.enabled && r.isVisible) || p.body.GetComponentInChildren<Renderer>() != null,
                "astronaut renderers " + p.astronaut.GetComponentsInChildren<Renderer>().Length + ", rover renderers " + p.rover.GetComponentsInChildren<Renderer>().Length);

            // 1. Astronaut: W walk from spawn, Shift run, A/S/D ----------------------------------------
            var cc = p.astronaut;
            var spot = OpenSpot(cc.transform.position, 5f);
            if (Overlaps(cc).Length > 0) Teleport(cc, spot, p.yaw);
            p.yaw = cc.transform.eulerAngles.y;
            yield return Real(0.5f);
            Vector3 a0 = cc.transform.position; float e0 = p.elapsed;
            Hold(Key.W);
            yield return Sim(1.2f);
            yield return Shot("02_astronaut_walking.png", "holding W, third-person camera follows");
            yield return Sim(0.8f);
            Release();
            float walk = Vector3.ProjectOnPlane(cc.transform.position - a0, Vector3.up).magnitude / Mathf.Max(0.01f, p.elapsed - e0);
            Check("W walks the astronaut", walk > 1.5f, $"{walk:F2} m/s");
            a0 = cc.transform.position; e0 = p.elapsed;
            Hold(Key.W, Key.LeftShift);
            yield return Sim(2f);
            Release();
            float run = Vector3.ProjectOnPlane(cc.transform.position - a0, Vector3.up).magnitude / Mathf.Max(0.01f, p.elapsed - e0);
            Check("Shift+W runs faster than walking", run > walk + 0.8f, $"run {run:F2} vs walk {walk:F2} m/s");
            foreach (var (key, name) in new[] { (Key.S, "S back"), (Key.A, "A left"), (Key.D, "D right") })
            {
                a0 = cc.transform.position;
                Hold(key); yield return Sim(1f); Release();
                Check("Key " + name + " moves the astronaut", Vector3.Distance(a0, cc.transform.position) > 1f, $"{Vector3.Distance(a0, cc.transform.position):F2} m");
            }
            yield return Sim(0.3f);
            Check("Astronaut grounded after walking", Mathf.Abs(cc.transform.position.y - GroundY(cc.transform.position)) < 0.4f, $"y {cc.transform.position.y:F2} ground {GroundY(cc.transform.position):F2}");
            float camDist = Vector3.Distance(p.follow.transform.position, cc.transform.position);
            Check("Camera follows astronaut (3-8 m behind)", camDist > 2.5f && camDist < 9f, $"{camDist:F2} m");

            // Mouse look: right button + drag
            float yaw0 = p.yaw;
            for (int i = 0; i < 20; i++) { InputSystem.QueueStateEvent(mouse, new MouseState { delta = new Vector2(12, 0) }.WithButton(MouseButton.Right)); yield return null; }
            InputSystem.QueueStateEvent(mouse, new MouseState());
            yield return Real(0.2f);
            Check("Right-mouse drag rotates the camera", Mathf.Abs(Mathf.DeltaAngle(yaw0, p.yaw)) > 5f, $"yaw {yaw0:F1} -> {p.yaw:F1}");
            yaw0 = p.yaw;
            for (int i = 0; i < 10; i++) { InputSystem.QueueStateEvent(mouse, new MouseState { delta = new Vector2(12, 0) }); yield return null; }
            yield return Real(0.2f);
            Line($"INFO     Mouse move WITHOUT right button changes yaw by {Mathf.DeltaAngle(yaw0, p.yaw):F1} deg (design: look only while right button held)");

            // Collision with a structure
            var structure = LargestStructureNear(cc.transform.position);
            if (structure)
            {
                var b = structure.bounds;
                Vector3 dir = cc.transform.position - b.center; dir.y = 0; dir = dir.sqrMagnitude < .01f ? Vector3.back : dir.normalized;
                Teleport(cc, b.center + dir * (Mathf.Max(b.extents.x, b.extents.z) + 4), Mathf.Atan2(-dir.x, -dir.z) * Mathf.Rad2Deg);
                p.yaw = Mathf.Atan2(-dir.x, -dir.z) * Mathf.Rad2Deg;
                yield return Sim(0.3f);
                Hold(Key.W, Key.LeftShift); yield return Sim(5f); Release();
                var end = cc.transform.position;
                float horiz = Vector2.Distance(new Vector2(end.x, end.z), new Vector2(b.center.x, b.center.z));
                Check("Astronaut is blocked by '" + structure.name + "' (walked into it 5 s)", horiz > Mathf.Min(b.extents.x, b.extents.z) * 0.5f && Overlaps(cc).Length == 0, $"stopped {horiz:F1} m from centre");
                // camera collision: camera behind astronaut while astronaut faces away from the wall
                p.yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg; // look away from the wall so the chase camera swings into it
                yield return Real(1.2f);
                Check("Camera avoids clipping into the structure (obstruction handled)", !Physics.OverlapSphere(p.follow.transform.position, 0.1f, 1 << 0, QueryTriggerInteraction.Ignore).Any(x => !OwnCollider(x)),
                    $"rig obstructed {rig.LastObstructed}, camera {V(p.follow.transform.position)}, inside: " + string.Join(",", Physics.OverlapSphere(p.follow.transform.position, 0.1f, 1 << 0, QueryTriggerInteraction.Ignore).Where(x => !OwnCollider(x)).Select(x => x.name + (x is TerrainCollider ? "(terrain)" : ""))));
            }
            else Result("UNTESTED", "Structure collision (no structure found)");

            // Crew Dome airlock: the laptop-playtest FAIL. Astronaut backs onto the airlock, camera must stay outside.
            var airlock = GameObject.Find("Crew Dome sealed airlock");
            var airlockCol = airlock ? airlock.GetComponentInChildren<Collider>() : null;
            if (airlockCol)
            {
                var ab = airlockCol.bounds;
                foreach (float side in new[] { 0f, 90f, -90f, 180f })
                {
                    Vector3 outward = Quaternion.Euler(0, side, 0) * (ab.center - GameObject.Find("Crew Dome").transform.position);
                    outward.y = 0; outward = outward.normalized;
                    Vector3 lockSpot = ab.center + outward * (Mathf.Max(ab.extents.x, ab.extents.z) + 0.45f);
                    Teleport(cc, lockSpot, YawTo(lockSpot, lockSpot + outward));
                    p.yaw = YawTo(lockSpot, lockSpot + outward); // facing away: chase camera swings into the airlock
                    p.pitch = 18;
                    int inside0 = cameraInside;
                    yield return Real(1.5f);
                    var hits = Physics.OverlapSphere(p.follow.transform.position, 0.1f, 1 << 0, QueryTriggerInteraction.Ignore).Where(x => !OwnCollider(x)).Select(x => x.name).ToArray();
                    Check($"Camera stays outside Crew Dome airlock (approach {side:+0;-0;0} deg)", hits.Length == 0 && cameraInside == inside0,
                        $"camera {V(p.follow.transform.position)}, astronaut {V(cc.transform.position)}, frames inside {cameraInside - inside0}, in: {string.Join(",", hits)}");
                    if (side == 0f) yield return Shot("12_airlock_camera.png", "astronaut backed onto Crew Dome airlock, camera outside");
                }
            }
            else Result("UNTESTED", "Crew Dome airlock camera (airlock not found)");

            Teleport(cc, new Vector3(114, 0, 0), 90); p.yaw = 90;
            yield return Sim(0.3f);
            Hold(Key.W, Key.LeftShift); yield return Sim(3f); Release();
            Check("World boundary holds astronaut inside ±119 m", Mathf.Abs(cc.transform.position.x) <= 119.01f, V(cc.transform.position));

            // 2. Rover ------------------------------------------------------------------------------------
            var rover = p.rover;
            var rSpot = OpenSpot(new Vector3(-30, 0, -10), 9f);
            Teleport(rover, rSpot, 0);
            Teleport(cc, rover.transform.position + rover.transform.right * 4f, 0);
            p.yaw = YawTo(cc.transform.position, rover.transform.position) - 35f;
            yield return Real(1.5f);
            yield return Shot("03_rover_exterior.png", "astronaut 4 m beside the rover");
            Teleport(cc, rover.transform.position + rover.transform.right * 9f, 0);
            yield return Sim(0.3f);
            yield return Tap(Key.E);
            Check("E does not board from 9 m", !p.driving, State());
            Teleport(cc, rover.transform.position + rover.transform.right * 3.5f, 0);
            yield return Sim(0.3f);
            yield return Tap(Key.E);
            yield return Sim(0.2f);
            yield return Shot("04_boarding_rover.png", "immediately after pressing E within 3.5 m");
            Check("E boards the rover within 5 m", p.driving && modes.Current == ProductionMode.Rover, State());

            p.yaw = rover.transform.eulerAngles.y;
            Vector3 r0 = rover.transform.position; float wheel0 = art.wheelRollDegrees;
            Hold(Key.W);
            yield return Sim(2.5f);
            yield return Shot("05_rover_driving.png", "holding W in the rover, chase camera");
            yield return Sim(1f);
            float top = p.speed;
            Check("W drives the rover forward", top > 2.5f && Vector3.Distance(r0, rover.transform.position) > 5f, $"speed {top:F2} m/s, {Vector3.Distance(r0, rover.transform.position):F1} m");
            yield return Tap(Key.E);
            Check("E exit refused while moving", p.driving, hud.lastResult);
            float h0 = rover.transform.eulerAngles.y;
            Hold(Key.W, Key.D); yield return Sim(2f); Release(Key.D);
            Check("D steers the rover", Mathf.Abs(Mathf.DeltaAngle(h0, rover.transform.eulerAngles.y)) > 15, $"{Mathf.DeltaAngle(h0, rover.transform.eulerAngles.y):F1} deg");
            h0 = rover.transform.eulerAngles.y;
            Hold(Key.A); yield return Sim(2f); Release();
            Check("A steers the rover the other way", Mathf.DeltaAngle(h0, rover.transform.eulerAngles.y) < -10, $"{Mathf.DeltaAngle(h0, rover.transform.eulerAngles.y):F1} deg");
            Hold(Key.Space); yield return Sim(2f); Release();
            Check("Space brakes the rover to a stop", Mathf.Abs(p.speed) < 0.2f, $"{p.speed:F2} m/s");
            Hold(Key.S); yield return Sim(2f); Release();
            Check("S reverses the rover", p.speed < -0.8f, $"{p.speed:F2} m/s");
            Hold(Key.Space); yield return Sim(1.5f); Release();
            Check("Wheels animate with travel", Mathf.Abs(art.wheelRollDegrees - wheel0) > 30f, $"roll {art.wheelRollDegrees - wheel0:F0} deg");

            // chase distance toggle via the on-screen button
            var chase = FindButton("Production chase");
            if (chase)
            {
                bool n0 = rig.nearChase; yield return Real(1f);
                float d0 = Vector3.Distance(p.follow.transform.position, rover.transform.position);
                chase.onClick.Invoke(); yield return Real(1.5f);
                float d1 = Vector3.Distance(p.follow.transform.position, rover.transform.position);
                Check("CHASE DISTANCE button switches near/far chase", rig.nearChase != n0 && Mathf.Abs(d1 - d0) > 0.8f, $"near {n0}->{rig.nearChase}, distance {d0:F2} -> {d1:F2} m");
                chase.onClick.Invoke(); yield return Real(1f);
            }
            else Result("UNTESTED", "Chase distance button not found");

            // rover collision
            var rt = LargestStructureNear(rover.transform.position);
            if (rt)
            {
                var b = rt.bounds;
                Vector3 dir = rover.transform.position - b.center; dir.y = 0; dir = dir.normalized;
                float yawTo = Mathf.Atan2(-dir.x, -dir.z) * Mathf.Rad2Deg;
                Teleport(rover, b.center + dir * (Mathf.Max(b.extents.x, b.extents.z) + 9), yawTo);
                p.speed = 0; p.yaw = yawTo;
                yield return Sim(0.5f);
                Hold(Key.W); yield return Sim(7f); Release();
                var end = rover.transform.position;
                float horiz = Vector2.Distance(new Vector2(end.x, end.z), new Vector2(b.center.x, b.center.z));
                Check("Rover is blocked by '" + rt.name + "'", horiz > Mathf.Min(b.extents.x, b.extents.z) * 0.5f && Overlaps(rover).Length == 0, $"stopped {horiz:F1} m from centre, speed {p.speed:F2}");
                Hold(Key.Space); yield return Sim(1f); Release();
            }

            // exit beside the Laboratory Dome (previous roof bug)
            var dome = UnityEngine.Object.FindObjectsByType<Collider>(FindObjectsSortMode.None).FirstOrDefault(c => c.name == "Laboratory Dome");
            if (dome)
            {
                var b = dome.bounds;
                Vector3 dir = Vector3.ProjectOnPlane(rSpot - b.center, Vector3.up).normalized;
                float yawRight = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg - 90f;
                bool parked = false;
                for (float gap = 1.4f; gap < 6f && !parked; gap += 0.2f) { Teleport(rover, b.center + dir * (b.extents.x + gap), yawRight); parked = Overlaps(rover).Length == 0; }
                p.speed = 0;
                yield return Sim(0.5f);
                float g0 = GroundY(rover.transform.position);
                yield return Tap(Key.E);
                yield return Sim(0.5f);
                float lift = cc.transform.position.y - g0;
                Check("E exit beside Laboratory Dome lands on the ground (roof bug)", !p.driving && lift < 1f && Overlaps(cc).Length == 0, $"astronaut {V(cc.transform.position)}, {lift:F2} m above rover ground, {State()}");
            }
            else Result("UNTESTED", "Laboratory Dome not found");
            if (p.driving) { p.speed = 0; Teleport(rover, rSpot, 0); yield return Sim(0.3f); yield return Tap(Key.E); }
            Check("Astronaut back on foot and can walk", !p.driving && cc.enabled, State());

            // 3. Science ---------------------------------------------------------------------------------
            Teleport(cc, OpenSpot(new Vector3(-20, 0, -40)), 0);
            yield return Sim(0.3f);
            yield return Tap(Key.K);
            Check("K far from any station is rejected", !hud.scanning && hud.lastResult.Contains("No valid target"), hud.lastResult);
            string obj0 = p.objective; int scans0 = hud.game.scans.Count;
            var targets = hud.environment.targets.Where(t => t).ToArray();
            for (int i = 0; i < targets.Length; i++)
            {
                var t = targets[i];
                Vector3 tp = t.transform.position;
                Teleport(cc, tp - Vector3.forward * 2f, 0);
                if (Overlaps(cc).Length > 0) Teleport(cc, tp + Vector3.forward * 2f, 180);
                p.yaw = YawTo(cc.transform.position, tp);
                yield return Sim(0.5f);
                int n = hud.game.scans.Count;
                yield return Tap(Key.K);
                bool began = hud.scanning;
                if (i == 0) yield return Shot("06_research_scanning.png", "K pressed beside " + t.displayName + ", scan progress");
                yield return Real(1.3f);
                Check("K scan at '" + t.displayName + "' recorded", began && hud.game.scans.Count == n + 1, $"began {began}, {hud.lastResult}");
            }
            // move out of range mid-scan
            var t0 = targets[0];
            Teleport(cc, t0.transform.position - Vector3.forward * 2f, 0);
            if (Overlaps(cc).Length > 0) Teleport(cc, t0.transform.position + Vector3.forward * 2f, 180);
            yield return Sim(0.3f);
            int before = hud.game.scans.Count;
            yield return Tap(Key.K);
            Teleport(cc, t0.transform.position + Vector3.right * 8f, 0);
            yield return Real(1.3f);
            Check("Scan rejected when astronaut leaves 3 m range mid-scan", hud.game.scans.Count == before, hud.lastResult);
            Line($"INFO     Objective before scans: '{obj0}' | after: '{p.objective}' | scans {scans0} -> {hud.game.scans.Count}");
            Check("Completed scans recorded in mission data", hud.game.scans.Count >= scans0 + targets.Length, $"{scans0} -> {hud.game.scans.Count}");
            yield return Real(1f);
            yield return Shot("08_full_hud.png", "full HUD after science scans");
            AuditHud("after scans, full HUD");

            // 4. Cameras / pause --------------------------------------------------------------------------
            yield return Tap(Key.C);
            yield return Real(2.5f);
            yield return Shot("07_cinematic_camera.png", "C cinematic overview");
            Check("C enters cinematic overview", modes.Current == ProductionMode.Cinematic, State());
            yield return Tap(Key.C);
            yield return Real(1f);
            Check("C returns to gameplay camera", modes.Current == ProductionMode.Astronaut, State());

            float el = p.elapsed;
            yield return Tap(Key.Escape);
            yield return Real(1.2f);
            bool paused = hud.environment.paused;
            Check("Esc pauses (simulation clock frozen)", paused && Mathf.Abs(p.elapsed - el) < 0.3f, $"paused {paused}, clock +{p.elapsed - el:F2}s, {State()}");
            Hold(Key.W); yield return Real(0.8f); Release();
            Check("Gameplay input blocked while paused", !hud.InputAllowed, State());
            yield return Shot("09b_pause_menu.png", "Esc pause modal");
            yield return Tap(Key.Escape);
            yield return Real(0.8f);
            Check("Esc resumes", !hud.environment.paused && !hud.modalOpen && modes.Current == ProductionMode.Astronaut, State());

            var samples = FindButton("Shortcut 1");
            if (samples) { samples.onClick.Invoke(); yield return Real(1f); yield return Shot("09_mission_status.png", "SAMPLES shortcut: scientific sample records"); AuditHud("samples modal"); yield return Tap(Key.Escape); yield return Real(0.6f); }
            else Result("UNTESTED", "Samples shortcut button not found");
            Check("Modal closed and play resumed", !hud.modalOpen && !hud.environment.paused, State());

            f1Persisted = true; // CycleVisibility writes PlayerPrefs; the original profile is written back in Finish
            int v0 = hud.settings.visibility; var seen = new List<int>();
            for (int i = 0; i < 4; i++) { yield return Tap(Key.F1); yield return Real(0.4f); seen.Add(hud.settings.visibility); if (i == 1) yield return Shot("11_hud_profile_f1.png", "F1 pressed twice: reduced HUD profile " + hud.settings.visibility); }
            Check("F1 cycles HUD profiles and returns after 4 presses", seen.Distinct().Count() == 4 && hud.settings.visibility == v0, string.Join(">", seen));

            // weather keys 1/2/3 (runtime only), back to original
            int w0 = p.weather;
            yield return Tap(Key.Digit3); yield return Real(0.5f); int w3 = p.weather;
            yield return Tap(Key.Digit2); yield return Real(0.5f); int w2 = p.weather;
            yield return Tap(Key.Digit1); yield return Real(0.5f); int w1 = p.weather;
            Check("Keys 1/2/3 switch weather preview", w3 == 2 && w2 == 1 && w1 == 0, $"{w3}/{w2}/{w1}");
            p.SetWeather(w0);

            // overview capture: existing rig override (camera placement only)
            rig.captureOverride = true; rig.captureTarget = new Vector3(-20, 0, -20); rig.capturePosition = new Vector3(40, 45, -85);
            yield return Real(3f);
            // White sky line investigation: same view with the Editor state as found, then with Game View gizmos off and nothing selected.
            Line("INFO     Editor selection: " + (Selection.activeObject ? Selection.activeObject.name : "none") + "; Game View gizmos: " + GameViewGizmos());
            yield return Shot("10a_overview_editor_state_as_found.png", "overview with Editor gizmo/selection state as found");
            bool gizmos0 = GameViewGizmos(); var sel0 = Selection.objects;
            SetGameViewGizmos(false); Selection.activeObject = null;
            yield return Real(0.5f);
            yield return Shot("10_mars_overview.png", "high overview of the research base (Game View gizmos off, nothing selected)");
            SetGameViewGizmos(gizmos0); Selection.objects = sel0;
            rig.captureOverride = false;
            yield return Real(1f);

            // 5. Performance --------------------------------------------------------------------------------
            int q0 = hud.settings.quality;
            Teleport(cc, OpenSpot(new Vector3(-20, 0, -40)), 0);
            yield return Real(1f);
            yield return Measure($"Current preset (q={q0}) astronaut walking, full HUD", 10f);
            Hold(Key.W, Key.LeftShift); yield return Real(0.1f);
            yield return Measure($"Current preset (q={q0}) astronaut running loop", 6f);
            Release();
            Teleport(rover, OpenSpot(new Vector3(-30, 0, -10), 9f), 0);
            Teleport(cc, rover.transform.position + rover.transform.right * 3.5f, 0);
            yield return Sim(0.3f); yield return Tap(Key.E); yield return Sim(0.2f);
            Hold(Key.W, Key.D);
            yield return Measure($"Current preset (q={q0}) rover driving in circles", 10f);
            Hold(Key.Space); Release(Key.W, Key.D); yield return Sim(1.5f); Release();
            string[] names = { "Low", "Medium", "High", "Desktop/Ultra Preview" };
            for (int q = 0; q < 4; q++)
            {
                hud.environment.ApplyQuality(q);
                yield return Real(2f);
                yield return Measure($"Preset {names[q]} (stationary in rover, full HUD)", 5f);
            }
            hud.environment.ApplyQuality(q0);
            Line("Graphics preset restored to q=" + q0 + " (runtime pipeline copy only; project assets untouched)");
            yield return Real(0.5f);
            yield return Tap(Key.E);
            sampling = false;
            Check("No astronaut/rover below terrain during the session", belowGround == 0, $"{belowGround} of {frames} frames");
            Line($"INFO     camera-inside-geometry frames: {cameraInside} of {frames}");
            foreach (var c in cameraSamples) Line("         " + c);
        }

        static void Finish()
        {
            routine = null; sampling = false;
            try
            {
                if (keyboard != null) { InputSystem.RemoveDevice(keyboard); keyboard = null; }
                if (mouse != null) { InputSystem.RemoveDevice(mouse); mouse = null; }
                if (inputPatched) { InputSystem.settings.backgroundBehavior = priorBackground; InputSystem.settings.editorInputBehaviorInPlayMode = priorEditorInput; inputPatched = false; }
                if (hud) hud.game.testing = false;
                if (hud && priorVisibility >= 0) { hud.settings.visibility = priorVisibility; hud.ApplySettings(); if (f1Persisted) hud.PersistSettings(); }
            }
            catch { }
            Application.logMessageReceived -= OnLog;
            Directory.CreateDirectory(Out);
            var all = new List<string> { $"AstroGateway Visual Upgrade v1 playtest (graphical Editor, scripted real-input) — {DateTime.Now:yyyy-MM-dd HH:mm:ss}", $"PASS {pass}  FAIL {fail}  UNTESTED {untested}", "" };
            all.AddRange(lines);
            all.Add(""); all.Add("Performance (Editor Play Mode on this laptop; includes Editor overhead):"); all.AddRange(perf);
            all.Add(""); all.Add($"Console errors/exceptions during playtest ({logErrors.Count}):"); all.AddRange(logErrors.Distinct().Take(60));
            all.Add(""); all.Add($"Console warnings during playtest ({logWarnings.Count}, distinct first 30):"); all.AddRange(logWarnings.Distinct().Take(30));
            File.WriteAllLines(Path.Combine(Out, "VisualUpgradePlaytest_Results.txt"), all);
            File.WriteAllLines(Path.Combine(Out, "HUD_Audit.txt"), hudAudit);
            Debug.Log("[VisualUpgrade] DONE PASS " + pass + " FAIL " + fail + " UNTESTED " + untested);
            EditorApplication.isPlaying = false; // leave the scene open in Edit Mode
            if (Environment.GetEnvironmentVariable("VU_QUIT") == "1") EditorApplication.delayCall += () => EditorApplication.Exit(fail > 0 ? 1 : 0);
        }
    }
}
