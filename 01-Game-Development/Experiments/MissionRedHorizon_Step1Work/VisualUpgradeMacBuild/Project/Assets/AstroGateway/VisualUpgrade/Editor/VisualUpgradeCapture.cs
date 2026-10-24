using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using AstroGateway.CinematicHUD;
using AstroGateway.ProductionVisual;

namespace AstroGateway.VisualUpgrade.EditorTools
{
    // Real Game View captures for review (graphical Editor only). Opens a scene (env VU_SCENE, default the Visual
    // Upgrade scene; the Production Visual v1 scene can be captured for before/after), enters Play Mode, places
    // the existing camera rig at fixed review shots, captures with ScreenCapture, and quits when VU_QUIT=1.
    // Game View gizmos are switched off and nothing is selected so Editor overlays never appear in captures.
    // Never saves the scene; the HUD profile is changed for this run only (not persisted).
    [InitializeOnLoad]
    public static class VisualUpgradeCapture
    {
        const string Flag = "VisualUpgradeCapture.Run", QuitFlag = "VisualUpgradeCapture.Quit";
        static string Out => Environment.GetEnvironmentVariable("VU_OUT") ?? Path.GetFullPath("Docs/VisualUpgrade/Captures");
        static string ScenePath => Environment.GetEnvironmentVariable("VU_SCENE") ?? VisualUpgradeBuilder.Scene;
        static int Quality => int.TryParse(Environment.GetEnvironmentVariable("VU_QUALITY"), out var q) ? q : 2;
        static IEnumerator routine; static readonly Stack<IEnumerator> stack = new Stack<IEnumerator>(); static double waitUntil; static int frameWait;
        static readonly List<string> log = new List<string>();

        static VisualUpgradeCapture() { EditorApplication.update += Tick; }

        public static void Run()
        {
            EditorSceneManager.OpenScene(ScenePath);
            try { PlayModeWindow.SetViewType(PlayModeWindow.PlayModeViewTypes.GameView); } catch { }
            try { PlayModeWindow.SetCustomRenderingResolution(1512, 982, "AstroGateway Visual Upgrade"); } catch { }
            try { ProductionViewport.SetSize(1512, 982); } catch (Exception e) { Debug.LogWarning("Game View size: " + e.Message); }
            Selection.activeObject = null;
            SetGizmos(false);
            SessionState.SetBool(Flag, true);
            Application.runInBackground = true;
            EditorApplication.isPaused = false;
            EditorApplication.isPlaying = true;
        }

        // Side-by-side before/after images from two capture folders (env VU_BEFORE, VU_AFTER, VU_OUT). Batch-safe.
        // Pixels are copied unchanged from the real Game View captures (half resolution); only a 6 px orange
        // divider is drawn between them.
        public static void Compare()
        {
            string before = Environment.GetEnvironmentVariable("VU_BEFORE"), after = Environment.GetEnvironmentVariable("VU_AFTER");
            Directory.CreateDirectory(Out);
            foreach (var path in Directory.GetFiles(after, "0*.png"))
            {
                string name = Path.GetFileName(path), other = Path.Combine(before, name);
                if (!File.Exists(other)) continue;
                var a = new Texture2D(2, 2); a.LoadImage(File.ReadAllBytes(other));
                var b = new Texture2D(2, 2); b.LoadImage(File.ReadAllBytes(path));
                int w = a.width / 2, h = a.height / 2, gap = 6;
                var outTex = new Texture2D(w * 2 + gap, h, TextureFormat.RGB24, false);
                outTex.SetPixels(Enumerable.Repeat(new Color(1f, .56f, .16f), (w * 2 + gap) * h).ToArray());
                for (int y = 0; y < h; y++)
                    for (int x = 0; x < w; x++)
                    {
                        outTex.SetPixel(x, y, a.GetPixel(x * 2, y * 2));
                        outTex.SetPixel(x + w + gap, y, b.GetPixel(Mathf.Min(b.width - 1, x * 2 * b.width / a.width), Mathf.Min(b.height - 1, y * 2 * b.height / a.height)));
                    }
                outTex.Apply();
                File.WriteAllBytes(Path.Combine(Out, "BeforeAfter_" + name), outTex.EncodeToPNG());
                UnityEngine.Object.DestroyImmediate(a); UnityEngine.Object.DestroyImmediate(b); UnityEngine.Object.DestroyImmediate(outTex);
            }
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
            if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode && SessionState.GetBool(QuitFlag, false))
            {
                SessionState.SetBool(QuitFlag, false);
                EditorApplication.Exit(0);
                return;
            }
            if (!EditorApplication.isPlaying) return;
            if (EditorApplication.isPaused && SessionState.GetBool(Flag, false)) EditorApplication.isPaused = false;
            try
            {
                if (routine == null)
                {
                    if (!SessionState.GetBool(Flag, false)) return;
                    var hud = UnityEngine.Object.FindAnyObjectByType<AstroGatewayHUDController>();
                    if (!hud || !hud.safe || !hud.game.initialized) return;
                    SessionState.SetBool(Flag, false);
                    routine = Shots(hud);
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
                if (stack.Count == 0) Finish();
            }
            catch (Exception e) { log.Add("EXCEPTION " + e); Finish(); }
        }

        static void Finish()
        {
            routine = null;
            Directory.CreateDirectory(Out);
            File.WriteAllLines(Path.Combine(Out, "Capture_Log.txt"), log);
            try { ProductionViewport.Cleanup(); } catch { }
            EditorApplication.isPlaying = false;
            if (Environment.GetEnvironmentVariable("VU_QUIT") == "1") SessionState.SetBool(QuitFlag, true);
        }

        static void Teleport(CharacterController c, Vector3 pos, float yaw)
        {
            if (Physics.Raycast(pos + Vector3.up * 30, Vector3.down, out var hit, 80, 1, QueryTriggerInteraction.Ignore)) pos.y = hit.point.y + .05f;
            c.enabled = false; c.transform.SetPositionAndRotation(pos, Quaternion.Euler(0, yaw, 0)); c.enabled = true; Physics.SyncTransforms();
        }

        static IEnumerator Shot(string name, string note)
        {
            yield return .25f;
            Directory.CreateDirectory(Out);
            string path = Path.Combine(Out, name);
            ScreenCapture.CaptureScreenshot(path);
            yield return .8f;
            log.Add($"{name}: {note} [{Screen.width}x{Screen.height}, exists={File.Exists(path)}]");
        }

        static IEnumerator Shots(AstroGatewayHUDController hud)
        {
            var p = hud.game.prototype;
            var rig = UnityEngine.Object.FindAnyObjectByType<ProductionCameraRig>();
            var modes = UnityEngine.Object.FindAnyObjectByType<ProductionModeCoordinator>();
            hud.game.testing = true; // gameplay input routers idle; the game still simulates
            hud.settings.visibility = 0; hud.ApplySettings();
            hud.environment.ApplyQuality(Quality);
            log.Add("Scene " + ScenePath + " quality " + Quality + " " + hud.environment.Readback());
            yield return 2f;

            // 1 Landscape (HUD hidden), 2 astronaut close-up, 3 rover close-up, 4 habitat, 5 station.
            hud.settings.visibility = 3; hud.ApplySettings();
            rig.captureOverride = true;
            rig.capturePosition = new Vector3(70, 9, -110); rig.captureTarget = new Vector3(-10, 2, -30);
            yield return 3f; yield return Shot("01_landscape.png", "landscape toward the base, HUD hidden");

            Teleport(p.astronaut, new Vector3(-30, 0, -78), 200); p.yaw = 200;
            var a = p.astronaut.transform.position;
            rig.capturePosition = a + new Vector3(1.6f, 1.55f, -2.3f); rig.captureTarget = a + Vector3.up * 1.15f;
            yield return 3f; yield return Shot("02_astronaut_closeup.png", "astronaut close-up");

            var r = p.rover.transform.position;
            rig.capturePosition = r + p.rover.transform.rotation * new Vector3(4.2f, 2.0f, 5.2f); rig.captureTarget = r + Vector3.up * 1.1f;
            yield return 3f; yield return Shot("03_rover_closeup.png", "rover close-up");

            rig.capturePosition = new Vector3(-28, 3.2f, -78); rig.captureTarget = new Vector3(-55, 4, -52);
            yield return 3f; yield return Shot("04_habitat.png", "habitat domes and airlocks");

            var station = hud.environment.targets.FirstOrDefault(t => t);
            if (station)
            {
                var s = station.transform.position;
                rig.capturePosition = s + new Vector3(2.6f, 1.7f, -3.2f); rig.captureTarget = s + Vector3.up * .6f;
                yield return 3f; yield return Shot("05_science_station.png", "research station " + station.displayName);
            }

            // 6 Gameplay HUD, 7 rover HUD (driving), 8 cinematic overview.
            rig.captureOverride = false;
            hud.settings.visibility = 0; hud.ApplySettings();
            Teleport(p.astronaut, new Vector3(-30, 0, -78), 20); p.yaw = 20; p.pitch = 14;
            yield return 3f; yield return Shot("06_gameplay_hud.png", "third-person gameplay with full HUD");

            Teleport(p.astronaut, p.rover.transform.position + p.rover.transform.right * 3f, 0);
            yield return .5f;
            hud.game.Interact();
            p.yaw = p.rover.transform.eulerAngles.y; p.pitch = 12;
            float t0 = Time.time;
            while (Time.time - t0 < 2.5f) { hud.game.modes.Submit(new AstroGateway.NativeSlice.NativeInput { movement = Vector2.up, accelerate = true }); yield return 1; }
            yield return Shot("07_rover_hud.png", "driving the rover, chase camera and rover HUD");
            float t1 = Time.time;
            while (Time.time - t1 < 1.5f) { hud.game.modes.Submit(new AstroGateway.NativeSlice.NativeInput { brake = true }); yield return 1; }
            hud.game.Interact();

            modes.ToggleCinematic();
            yield return 4f; yield return Shot("08_cinematic_overview.png", "C cinematic overview (DoF on High/Ultra)");
            modes.ToggleCinematic();
            yield return 1f;
            hud.TogglePause();
            yield return 1.5f; yield return Shot("09_pause_menu.png", "Esc pause card with controls reference");
            hud.CloseModal();
            yield return .5f;
            hud.OpenSettings();
            yield return 1.5f; yield return Shot("10_settings_graphics.png", "settings card with graphics presets");
            hud.CloseModal();
            yield return 1f;
            log.Add("Done");
        }
    }
}
