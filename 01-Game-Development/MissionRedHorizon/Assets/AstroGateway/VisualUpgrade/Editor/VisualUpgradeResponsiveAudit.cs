using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using AstroGateway.CinematicHUD;
using AstroGateway.ProductionVisual;

namespace AstroGateway.VisualUpgrade.EditorTools
{
    // Responsive HUD audit for Mars_VisualUpgrade_v1 (graphical Editor only). For each Game View size
    // (env VU_SIZES, default 1512x982,1920x1080,2560x1440) it walks the HUD states (on foot, near rover, driving,
    // scanning, recent scan, pause, settings, and settings with touch controls forced on to check the mobile
    // configuration), captures each with ScreenCapture and measures every visible Text, Button and HUD panel
    // against the Game View rectangle: clipped/overflowing text, text under 11 px, off-screen buttons, overlapping
    // panels. Also checks that gameplay input is ignored while a modal is open.
    // Output: env VU_OUT (default Docs/VisualUpgrade/Batch2/Responsive). Env VU_QUIT=1 quits when done.
    // Never saves the scene; HUD profile and settings are changed for this run only (not persisted).
    [InitializeOnLoad]
    public static class VisualUpgradeResponsiveAudit
    {
        const string Flag = "VisualUpgradeResponsiveAudit.Run", QuitFlag = "VisualUpgradeResponsiveAudit.Quit";
        static string Out => Environment.GetEnvironmentVariable("VU_OUT") ?? Path.GetFullPath("Docs/VisualUpgrade/Batch2/Responsive");
        static IEnumerable<Vector2Int> Sizes => (Environment.GetEnvironmentVariable("VU_SIZES") ?? "1512x982,1920x1080,2560x1440")
            .Split(',').Select(s => s.Split('x')).Select(p => new Vector2Int(int.Parse(p[0]), int.Parse(p[1])));
        static readonly string[] TouchOnly = { "Analog movement joystick", "Run or accelerate", "Rover brake", "Scan", "Context interact" };
        static IEnumerator routine; static readonly Stack<IEnumerator> stack = new Stack<IEnumerator>(); static double waitUntil; static int frameWait;
        static readonly List<string> log = new List<string>();
        static int issues, checksFailed;
        static readonly List<InputDevice> disabledReal = new List<InputDevice>();

        static VisualUpgradeResponsiveAudit() { EditorApplication.update += Tick; }

        [MenuItem("AstroGateway/Visual Upgrade Responsive HUD Audit")]
        public static void Run()
        {
            EditorSceneManager.OpenScene(VisualUpgradeBuilder.Scene);
            try { PlayModeWindow.SetViewType(PlayModeWindow.PlayModeViewTypes.GameView); } catch { }
            Selection.activeObject = null;
            SetGizmos(false);
            SessionState.SetBool(Flag, true);
            Application.runInBackground = true;
            EditorApplication.isPaused = false;
            EditorApplication.isPlaying = true;
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
                EditorApplication.Exit(checksFailed > 0 ? 1 : 0);
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
                    routine = All(hud);
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
            catch (Exception e) { log.Add("EXCEPTION " + e); checksFailed++; Finish(); }
        }

        static void Finish()
        {
            routine = null;
            foreach (var d in disabledReal) if (d != null && d.added) InputSystem.EnableDevice(d);
            disabledReal.Clear();
            log.Insert(0, $"Responsive HUD audit — {DateTime.Now:yyyy-MM-dd HH:mm:ss} — layout issues {issues}, failed checks {checksFailed}");
            Directory.CreateDirectory(Out);
            File.WriteAllLines(Path.Combine(Out, "Responsive_HUD_Audit.txt"), log);
            try { ProductionViewport.Cleanup(); } catch { }
            EditorApplication.isPlaying = false;
            if (Environment.GetEnvironmentVariable("VU_QUIT") == "1") SessionState.SetBool(QuitFlag, true);
        }

        static void Check(string name, bool ok, string detail)
        {
            if (!ok) checksFailed++;
            log.Add($"{(ok ? "PASS " : "FAIL ")}    {name}  [{detail}]");
        }

        static void Teleport(CharacterController c, Vector3 pos, float yaw)
        {
            if (Physics.Raycast(pos + Vector3.up * 30, Vector3.down, out var hit, 80, 1, QueryTriggerInteraction.Ignore)) pos.y = hit.point.y + .05f;
            c.enabled = false; c.transform.SetPositionAndRotation(pos, Quaternion.Euler(0, yaw, 0)); c.enabled = true; Physics.SyncTransforms();
        }

        static IEnumerator Shot(Vector2Int size, string name)
        {
            yield return .4f;
            string dir = Path.Combine(Out, $"{size.x}x{size.y}");
            Directory.CreateDirectory(dir);
            string path = Path.Combine(dir, name + ".png");
            ScreenCapture.CaptureScreenshot(path);
            yield return .8f;
            Audit(size, name);
        }

        // ---------- geometry audit in Game View pixels (overlay canvas world space = Game View pixels) ----------
        static Rect Bounds(RectTransform r) { var c = new Vector3[4]; r.GetWorldCorners(c); return Rect.MinMaxRect(c[0].x, c[0].y, c[2].x, c[2].y); }
        static bool Visible(Component c) => c.gameObject.activeInHierarchy && c.GetComponentsInParent<CanvasGroup>().All(g => g.alpha > .05f);
        static string PathOf(Transform t) => t.parent ? (t.parent.parent ? t.parent.name + "/" : "") + t.name : t.name;

        static void Audit(Vector2Int size, string state)
        {
            var hud = UnityEngine.Object.FindAnyObjectByType<AstroGatewayHUDController>();
            var canvas = hud.safe.GetComponentInParent<Canvas>().rootCanvas;
            var screen = canvas.pixelRect;
            float scale = canvas.scaleFactor;
            int before = issues;
            log.Add($"--- {size.x}x{size.y} {state}: canvas {screen.width:F0}x{screen.height:F0} scale {scale:F2}");
            if (Mathf.Abs(screen.width - size.x) > 2 || Mathf.Abs(screen.height - size.y) > 2) Issue($"GAME VIEW SIZE {screen.width:F0}x{screen.height:F0} != requested");

            var texts = hud.safe.GetComponentsInChildren<Text>(false).Where(t => t.isActiveAndEnabled && !string.IsNullOrWhiteSpace(t.text) && t.color.a > .05f && Visible(t)).ToList();
            foreach (var t in texts)
            {
                var r = Bounds(t.rectTransform);
                var found = new List<string>();
                if (r.xMin < screen.xMin - 2 || r.yMin < screen.yMin - 2 || r.xMax > screen.xMax + 2 || r.yMax > screen.yMax + 2) found.Add("OFF-SCREEN");
                var gen = t.cachedTextGenerator;
                int px = Mathf.RoundToInt((t.resizeTextForBestFit && gen.characterCount > 0 ? gen.fontSizeUsedForBestFit / Mathf.Max(scale, .01f) : t.fontSize) * scale);
                if (px > 0 && px < 11) found.Add($"SMALL {px}px");
                if (t.verticalOverflow == VerticalWrapMode.Truncate && gen.characterCount > 0)
                {
                    // Regenerate the same text at the size actually used, without vertical truncation; fewer visible
                    // characters in the real layout means lines were cut off.
                    var gs = t.GetGenerationSettings(t.rectTransform.rect.size);
                    if (t.resizeTextForBestFit) { gs.resizeTextForBestFit = false; gs.fontSize = Mathf.RoundToInt(gen.fontSizeUsedForBestFit / Mathf.Max(gs.scaleFactor, .01f)); }
                    gs.verticalOverflow = VerticalWrapMode.Overflow;
                    var full = new TextGenerator();
                    full.Populate(t.text, gs);
                    if (gen.characterCountVisible < full.characterCountVisible) found.Add($"TRUNCATED {gen.characterCountVisible}/{full.characterCountVisible} chars");
                }
                if (t.horizontalOverflow == HorizontalWrapMode.Overflow && t.preferredWidth * scale > r.width + 4) found.Add($"OVERFLOW-H {t.preferredWidth * scale:F0}>{r.width:F0}px");
                string text = t.text.Replace("\n", " | ");
                if (found.Count > 0) Issue($"'{PathOf(t.transform)}' {r.width:F0}x{r.height:F0} {px}px \"{text.Substring(0, Math.Min(70, text.Length))}\" {string.Join("; ", found)}");
                else log.Add($"ok    '{PathOf(t.transform)}' {px}px \"{text.Substring(0, Math.Min(50, text.Length))}\"");
            }

            foreach (var b in hud.safe.GetComponentsInChildren<Button>(false).Where(b => b.isActiveAndEnabled && Visible(b)))
            {
                var r = Bounds((RectTransform)b.transform);
                if (r.xMin < screen.xMin - 1 || r.yMin < screen.yMin - 1 || r.xMax > screen.xMax + 1 || r.yMax > screen.yMax + 1) Issue($"BUTTON OFF-SCREEN '{PathOf(b.transform)}'");
                if (r.height < 20 * scale * .9f || r.width < 20 * scale * .9f) Issue($"BUTTON SMALL '{PathOf(b.transform)}' {r.width:F0}x{r.height:F0}px");
            }

            // Top-level HUD panels must not overlap each other (modal shield and its card are meant to cover the HUD).
            var panels = Enumerable.Range(0, hud.safe.childCount).Select(i => hud.safe.GetChild(i) as RectTransform)
                .Where(r => r && r.gameObject.activeInHierarchy && r.name != "Modal input shield" && r.GetComponent<Image>() && r.GetComponent<Image>().color.a > .05f && Visible(r) && !TouchOnly.Contains(r.name))
                .Select(r => (r, b: Bounds(r))).Where(p => p.b.width > 2 && p.b.height > 2).ToList();
            for (int i = 0; i < panels.Count; i++)
                for (int j = i + 1; j < panels.Count; j++)
                {
                    var a = panels[i].b; var c = panels[j].b;
                    float w = Mathf.Min(a.xMax, c.xMax) - Mathf.Max(a.xMin, c.xMin), h = Mathf.Min(a.yMax, c.yMax) - Mathf.Max(a.yMin, c.yMin);
                    if (w > 1 && h > 1) Issue($"PANEL OVERLAP '{panels[i].r.name}' x '{panels[j].r.name}' {w:F0}x{h:F0}px");
                }
            log.Add($"    {texts.Count} texts, {panels.Count} panels, {issues - before} issues");
        }

        static void Issue(string s) { issues++; log.Add("ISSUE " + s); }

        // ---------- states ----------
        static IEnumerator All(AstroGatewayHUDController hud)
        {
            var p = hud.game.prototype;
            var modes = UnityEngine.Object.FindAnyObjectByType<ProductionModeCoordinator>();
            var fixes = UnityEngine.Object.FindAnyObjectByType<VisualUpgradeHUDFixes>();
            hud.game.testing = true; // gameplay input routers idle; the game still simulates
            // Physical keyboard/mouse would still reach the HUD (menus, buttons) if someone uses the Mac during the run.
            foreach (var d in InputSystem.devices.ToList())
                if (d.enabled && (d is Keyboard || d is Pointer || d is Gamepad)) { InputSystem.DisableDevice(d); disabledReal.Add(d); }
            log.Add($"Physical input devices disabled for this run: {disabledReal.Count}");
            hud.settings.visibility = 0; hud.ApplySettings();
            log.Add("Scene " + VisualUpgradeBuilder.Scene + " quality " + hud.settings.quality + " " + hud.environment.Readback());
            yield return 2f;
            var station = hud.environment.targets.First(t => t);
            Vector3 spawn = new Vector3(-30, 0, -78);
            bool firstSize = true;
            foreach (var size in Sizes)
            {
                ProductionViewport.SetSize(size.x, size.y);
                yield return 1.5f;

                Teleport(p.astronaut, spawn, 20); p.yaw = 20; p.pitch = 14;
                yield return 1.5f; yield return Shot(size, "01_gameplay_hud_on_foot");

                Teleport(p.astronaut, p.rover.transform.position + p.rover.transform.right * 3f, 0);
                yield return 1f; yield return Shot(size, "02_near_rover_prompt");

                hud.game.Interact();
                p.yaw = p.rover.transform.eulerAngles.y; p.pitch = 12;
                float t0 = Time.time;
                while (Time.time - t0 < 2.5f) { hud.game.modes.Submit(new AstroGateway.NativeSlice.NativeInput { movement = Vector2.up, accelerate = true }); yield return 1; }
                yield return Shot(size, "03_rover_hud_driving");
                float t1 = Time.time;
                while (Time.time - t1 < 2f) { hud.game.modes.Submit(new AstroGateway.NativeSlice.NativeInput { brake = true }); yield return 1; }
                hud.game.Interact();
                yield return 1f;

                var s = station.transform.position;
                Teleport(p.astronaut, s + new Vector3(1.6f, 0, -1.2f), -50); p.yaw = -50;
                yield return 1f;
                hud.BeginScan();
                yield return 1;
                Check($"Scan starts beside {station.displayName} ({size.x}x{size.y})", hud.scanning, hud.lastResult);
                yield return Shot(size, "04_scanning");
                double end = EditorApplication.timeSinceStartup + 10;
                while (hud.scanning && EditorApplication.timeSinceStartup < end) yield return .2f;
                yield return 1f; yield return Shot(size, "05_recent_scan_after_scan");

                // Pause card; gameplay input must not move the astronaut while it is open.
                hud.TogglePause();
                yield return 1.2f;
                var before = p.astronaut.transform.position;
                double t2 = EditorApplication.timeSinceStartup; // game time is frozen while paused
                while (EditorApplication.timeSinceStartup - t2 < 1) { hud.game.modes.Submit(new AstroGateway.NativeSlice.NativeInput { movement = Vector2.up, accelerate = true }); yield return 1; }
                Check($"Input blocked under pause card ({size.x}x{size.y})", Vector3.Distance(before, p.astronaut.transform.position) < .05f, $"moved {Vector3.Distance(before, p.astronaut.transform.position):F2} m, modal {hud.modalOpen}, paused {hud.environment.paused}");
                yield return Shot(size, "06_pause_menu");
                hud.CloseModal();
                yield return .8f;
                Check($"Pause closes and game resumes ({size.x}x{size.y})", !hud.modalOpen && !hud.environment.paused, $"modal {hud.modalOpen}, paused {hud.environment.paused}");

                hud.OpenSettings();
                yield return 1.2f; yield return Shot(size, "07_settings_desktop");
                var joy = hud.safe.Find("Modal input shield/Modal card/Joystick size track");
                if (fixes && !fixes.TouchVisible) Check($"Touch-only settings hidden on desktop ({size.x}x{size.y})", !joy || !Visible(joy), joy ? (Visible(joy) ? "visible" : "hidden") : "not found");
                hud.CloseModal();
                yield return .8f;

                if (firstSize && fixes)
                {
                    // Mobile configuration: force touch controls on (as on a phone/touchscreen) and check they and their settings return.
                    fixes.showTouchControls = true;
                    yield return .6f;
                    var stick = hud.safe.Find("Analog movement joystick");
                    Check("Touch controls visible when touch is enabled", stick && Visible(stick), stick ? "alpha " + stick.GetComponent<CanvasGroup>()?.alpha : "not found");
                    yield return Shot(size, "08_touch_config_hud");
                    hud.OpenSettings();
                    yield return 1.2f;
                    joy = hud.safe.Find("Modal input shield/Modal card/Joystick size track");
                    Check("Touch settings visible when touch is enabled", joy && Visible(joy), joy ? (Visible(joy) ? "visible" : "hidden") : "not found");
                    yield return Shot(size, "09_touch_config_settings");
                    hud.CloseModal();
                    fixes.showTouchControls = false;
                    yield return .8f;
                }
                firstSize = false;
            }
            log.Add("Done");
        }
    }
}
