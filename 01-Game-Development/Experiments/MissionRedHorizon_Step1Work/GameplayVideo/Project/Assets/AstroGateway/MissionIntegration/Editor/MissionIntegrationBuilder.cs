using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using AstroGateway.CinematicHUD;
using AstroGateway.Integration;
using AstroGateway.NativeSlice;
using AstroGateway.ProductionVisual;
using AstroGateway.Stage08B;

// Builds Mars_MissionIntegration_v1 as a copy of Mars_ProductionVisual_v1 (the original is never opened for writing)
// and runs batch-mode integration tests with Game View captures. Output folder: env STEP2_OUT.
namespace AstroGateway.MissionIntegration
{
    [InitializeOnLoad]
    public static class MissionIntegrationBuilder
    {
        const string Source = "Assets/AstroGateway/ProductionVisual/Scenes/Mars_ProductionVisual_v1.unity";
        const string Folder = "Assets/AstroGateway/MissionIntegration/Scenes";
        const string Scene = Folder + "/Mars_MissionIntegration_v1.unity";
        const string Flag = "MissionIntegration.Validate";
        static string Out => Environment.GetEnvironmentVariable("STEP2_OUT") ?? Path.Combine(Path.GetTempPath(), "MissionIntegration");

        public static void Build()
        {
            if (File.Exists(Scene)) throw new Exception("Refusing to overwrite " + Scene);
            if (!Directory.Exists(Folder)) Directory.CreateDirectory(Folder);
            AssetDatabase.Refresh();
            if (!AssetDatabase.CopyAsset(Source, Scene)) throw new Exception("Scene copy failed");
            var scene = EditorSceneManager.OpenScene(Scene);
            var hud = UnityEngine.Object.FindAnyObjectByType<AstroGatewayHUDController>();
            if (!hud) throw new Exception("HUD controller not found in copied scene");
            var go = new GameObject("Mission Integration (provisional)");
            var bridge = go.AddComponent<GameplayEventBridge>();
            bridge.game = hud.game;
            var life = go.AddComponent<LifeSupportModelAdapter>();
            life.prototype = hud.game.prototype;
            var controller = go.AddComponent<MissionIntegrationController>();
            controller.hud = hud; controller.bridge = bridge; controller.lifeSupport = life;
            if (!EditorSceneManager.SaveScene(scene, Scene)) throw new Exception("Saving new scene failed");
            Debug.Log("[Step2] Built " + Scene);
            if (Application.isBatchMode) EditorApplication.Exit(0);
        }

        public static void Validate()
        {
            EditorSceneManager.OpenScene(Scene);
            Directory.CreateDirectory(Path.Combine(Out, "Screenshots"));
            EditorWindow.GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView")).Show();
            ProductionViewport.SetSize(1920, 1080);
            SessionState.SetBool(Flag, true);
            Application.runInBackground = true;
            EditorApplication.isPaused = false;
            EditorApplication.isPlaying = true;
        }

        // ---------- runner ----------
        static readonly List<string> lines = new List<string>();
        static readonly List<string> errors = new List<string>();
        static int pass, fail, lastFrame = -1;
        static readonly Stack<IEnumerator> stack = new Stack<IEnumerator>();
        static object waiting;
        static double started;
        static AstroGatewayHUDController hud;
        static MissionIntegrationController ctl;
        static MarsPrototype p;

        static MissionIntegrationBuilder() { EditorApplication.update += Tick; }

        static void Tick()
        {
            if (!Application.isBatchMode || !EditorApplication.isPlaying) return;
            try
            {
                if (stack.Count == 0)
                {
                    if (!SessionState.GetBool(Flag, false)) return;
                    hud = UnityEngine.Object.FindAnyObjectByType<AstroGatewayHUDController>();
                    ctl = UnityEngine.Object.FindAnyObjectByType<MissionIntegrationController>();
                    if (!hud || !hud.safe || !hud.game || !hud.game.initialized || !ctl || !ctl.panelText) return;
                    SessionState.SetBool(Flag, false);
                    p = hud.game.prototype;
                    Application.logMessageReceived += OnLog;
                    started = EditorApplication.timeSinceStartup;
                    stack.Push(Tests());
                }
                if (EditorApplication.timeSinceStartup - started > 2700) throw new Exception("Integration test timeout");
                if (Time.frameCount == lastFrame) return;
                lastFrame = Time.frameCount;
                if (!Ready()) return;
                waiting = null;
                while (stack.Count > 0)
                {
                    var top = stack.Peek();
                    if (!top.MoveNext()) { stack.Pop(); continue; }
                    if (top.Current is IEnumerator nested) { stack.Push(nested); continue; }
                    waiting = top.Current; break;
                }
                if (stack.Count == 0) Finish();
            }
            catch (Exception e) { Line("EXCEPTION: " + e); fail++; stack.Clear(); Finish(); }
        }

        class SimWait { public float until; public double deadline; }
        class RealWait { public double until; }
        static object Sim(float s) => new SimWait { until = p.elapsed + s, deadline = EditorApplication.timeSinceStartup + s * 6 + 10 };
        static object Real(float s) => new RealWait { until = EditorApplication.timeSinceStartup + s };
        static bool Ready()
        {
            if (waiting is SimWait w) return p.elapsed >= w.until || EditorApplication.timeSinceStartup >= w.deadline;
            if (waiting is RealWait r) return EditorApplication.timeSinceStartup >= r.until;
            return true;
        }

        static void OnLog(string m, string s, LogType t) { if (t == LogType.Error || t == LogType.Exception || t == LogType.Assert) errors.Add(t + ": " + m.Split('\n')[0]); }
        static void Line(string s) { lines.Add(s); Debug.Log("[Step2] " + s); }
        static void Check(string n, bool ok, string d = "") { if (ok) pass++; else fail++; Line((ok ? "PASS  " : "FAIL  ") + n + (string.IsNullOrEmpty(d) ? "" : "  [" + d + "]")); }
        static void Input(Vector2 m, bool brake = false, bool accelerate = false) => hud.game.modes.Submit(new NativeInput { movement = m, brake = brake, accelerate = accelerate });
        static int Count(string kind) => IntegrationServices.Sessions.Pending.Count(c => c.kind == kind);

        static void Place(CharacterController c, Vector3 pos)
        {
            if (Physics.Raycast(pos + Vector3.up * 30, Vector3.down, out var hit, 80, 1 << 0, QueryTriggerInteraction.Ignore) && hit.collider != c) pos.y = hit.point.y + .05f;
            c.enabled = false; c.transform.position = pos; c.enabled = true; Physics.SyncTransforms();
        }

        static IEnumerator Shot(string name)
        {
            ctl.Refresh();
            hud.Refresh();
            yield return Real(0.6f);
            ScreenCapture.CaptureScreenshot(Path.Combine(Out, "Screenshots", name + "_GameView.png"));
            yield return Real(1.0f);
            Line("Captured " + name);
        }

        class FasterTestModel : ILifeSupportModel
        {
            public string ModelVersion => "test-only-0.2";
            public float OxygenUsePerSecond(Activity a, WeatherState w) => 0.2f;
        }

        static IEnumerator Tests()
        {
            hud.game.testing = true;
            hud.settings = new HUDSettings();
            hud.ApplySettings();
            var cc = p.astronaut;
            var sessions = IntegrationServices.Sessions;
            Line("Scene: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().path);
            yield return Sim(1f);

            // Wiring and honesty
            Check("Running the isolated integration scene", UnityEngine.SceneManagement.SceneManager.GetActiveScene().path == Scene);
            Check("Bridge, life-support adapter and controller wired", ctl.bridge && ctl.bridge.game == hud.game && ctl.lifeSupport && ctl.lifeSupport.prototype == p);
            Check("Local mission session started from the registry", sessions.Session.status == "active" && sessions.Session.missionId == hud.game.registry.missionId, sessions.Session.missionId);
            Check("Backend reported as not connected", !sessions.Connected && !sessions.Session.authoritative && ctl.panelText.text.Contains("NOT CONNECTED"));
            Check("Account shown as offline, not signed in", !IntegrationServices.Identity.Current.authenticated && ctl.panelText.text.Contains("not signed in"));
            Check("Panel is labelled provisional", ctl.panelText.text.Contains("PROVISIONAL") && ctl.panelText.text.Contains("unapproved"));
            Check("No score is shown before the rubric is approved", ctl.report != null && !ctl.report.Scored && ctl.panelText.text.Contains("not scored"));
            Check("Recommendation is the offline default, not authoritative", ctl.recommendation != null && !ctl.recommendation.authoritative && ctl.panelText.text.Contains("offline default"));
            Check("No cloud sync claim in HUD", !hud.syncStatus.Contains("Synced") && !ctl.panelText.text.ToLowerInvariant().Contains("synced"));
            yield return Shot("01_Start_Astronaut_StatusPanel");

            // Rover board / drive / exit produce semantic commands
            Place(p.rover, new Vector3(-24, 0, -10));
            p.rover.transform.rotation = Quaternion.identity;
            Place(cc, p.rover.transform.position + p.rover.transform.right * 3.5f);
            yield return Sim(0.4f);
            long rev0 = sessions.Session.revision;
            int b0 = Count(CommandKinds.BoardRover), e0 = Count(CommandKinds.ExitRover);
            hud.Interact();
            yield return Sim(0.3f);
            Check("Boarding still works in the integration scene", p.driving, hud.lastResult);
            Check("Board command queued", Count(CommandKinds.BoardRover) == b0 + 1);
            p.yaw = p.rover.transform.eulerAngles.y;
            Vector3 r0 = p.rover.transform.position;
            Input(Vector2.up, accelerate: true);
            yield return Sim(3f);
            Check("Driving still works", Vector3.Distance(r0, p.rover.transform.position) > 5f, $"{Vector3.Distance(r0, p.rover.transform.position):F1} m");
            yield return Shot("02_Rover_Driving_StatusPanel");
            Input(Vector2.zero, brake: true);
            yield return Sim(2f);
            hud.Interact();
            yield return Sim(0.4f);
            Check("Exit still works", !p.driving, hud.lastResult);
            Check("Exit command queued and revision advanced", Count(CommandKinds.ExitRover) == e0 + 1 && sessions.Session.revision >= rev0 + 2, $"rev {rev0} → {sessions.Session.revision}");

            // Broker request shape
            var last = sessions.Pending.Last();
            string json = last.ToBrokerJson();
            Check("Command serialises to the planned FastAPI body", json.StartsWith("{\"command_id\":\"") && json.Contains("\"expected_revision\":") && json.Contains("\"kind\":\"exit_rover\"") && json.Contains("\"payload\":{") && Guid.TryParse(last.commandId, out _), json);
            Check("Command ids unique", sessions.Pending.Select(c => c.commandId).Distinct().Count() == sessions.Pending.Count);

            // Scans
            int s0 = Count(CommandKinds.ScanTarget);
            foreach (var t in hud.environment.targets.Where(x => x).Take(3))
            {
                Place(cc, t.transform.position - Vector3.forward * 2f);
                yield return Sim(0.4f);
                hud.BeginScan();
                yield return Real(1.4f);
            }
            Check("Scans recorded and queued as scan commands", Count(CommandKinds.ScanTarget) == s0 + 3 && hud.game.scans.Count >= 3, $"queued {Count(CommandKinds.ScanTarget) - s0}, game scans {hud.game.scans.Count}");
            var lastScan = sessions.Pending.Last(c => c.kind == CommandKinds.ScanTarget);
            Check("Scan command carries the sample id, labelled simulated", lastScan.payloadJson.Contains(hud.game.scans.Last().sampleId) && lastScan.payloadJson.Contains("unverified_simulated"));
            ctl.Refresh();
            var scansEvidence = ctl.report.skillEvidence.FirstOrDefault(e => e.skillId == "observation.simulated_scans");
            var roverEvidence = ctl.report.skillEvidence.FirstOrDefault(e => e.skillId == "equipment.rover_operation");
            Check("Skill evidence counts scans and board/exit cycles", scansEvidence != null && scansEvidence.count >= 3 && roverEvidence != null && roverEvidence.count >= 1 && !ctl.report.Scored, $"scans {scansEvidence?.count}, rover cycles {roverEvidence?.count}");
            yield return Shot("03_After_Scans_SkillEvidence");

            // Weather + oxygen
            int w0 = Count(CommandKinds.WeatherChanged);
            p.SetWeather(2);
            yield return Sim(0.5f);
            ctl.Refresh();
            Check("Weather feed and command follow the dust storm", IntegrationServices.Weather.Current.dust == DustLevel.DustStorm && Count(CommandKinds.WeatherChanged) == w0 + 1 && ctl.panelText.text.Contains("DustStorm"),
                $"feed {IntegrationServices.Weather.Current.dust}, weather commands {Count(CommandKinds.WeatherChanged) - w0}, session {sessions.Session.status}");
            Place(cc, new Vector3(-8, 0, -40));
            Input(Vector2.zero);
            yield return Sim(0.5f);
            float o0 = p.oxygen, t0 = p.elapsed;
            yield return Sim(8f);
            float used = o0 - p.oxygen, expected = (p.elapsed - t0) * 0.035f;
            Check("Default oxygen model leaves gameplay unchanged (storm rate)", Mathf.Abs(used - expected) < 0.01f, $"used {used:F4}%, legacy expects {expected:F4}%");
            yield return Shot("04_DustStorm_Oxygen_Model");
            var original = IntegrationServices.LifeSupport;
            IntegrationServices.LifeSupport = new FasterTestModel();
            o0 = p.oxygen; t0 = p.elapsed;
            yield return Sim(5f);
            used = o0 - p.oxygen; expected = (p.elapsed - t0) * 0.2f;
            Check("A different oxygen model can be plugged in without code changes", Mathf.Abs(used - expected) < 0.02f, $"used {used:F4}%, model expects {expected:F4}%");
            IntegrationServices.LifeSupport = original;
            p.SetWeather(0);
            yield return Sim(0.3f);

            // Pause / resume
            int pz = Count(CommandKinds.Pause), rz = Count(CommandKinds.Resume);
            hud.TogglePause();
            yield return Real(0.8f);
            hud.TogglePause();
            yield return Sim(0.5f);
            Check("Pause and resume queued, session active again", Count(CommandKinds.Pause) == pz + 1 && Count(CommandKinds.Resume) == rz + 1 && sessions.Session.status == "active");

            // Save / load keeps working, bridge copes with replaced scan list
            int before = hud.game.scans.Count;
            bool ok = hud.SaveLocal() && hud.LoadLocal();
            yield return Sim(0.5f);
            Check("Local save/load still works alongside the bridge", ok && hud.game.scans.Count == before, hud.game.testSave);

            // Panel toggle respects HUD visibility
            hud.settings.visibility = 3; hud.ApplySettings(); ctl.Refresh();
            bool hidden = !ctl.panelText.transform.parent.gameObject.activeSelf;
            hud.settings.visibility = 0; hud.ApplySettings(); ctl.Refresh();
            Check("Status panel hides with the HUD", hidden && ctl.panelText.transform.parent.gameObject.activeSelf);

            // Map view capture with panel
            hud.OpenMap();
            yield return Shot("05_Map_With_StatusPanel");
            hud.CloseModal();
            yield return Sim(0.3f);

            Check("No errors or exceptions logged", errors.Count == 0, errors.Count + " logged");
        }

        static void Finish()
        {
            Application.logMessageReceived -= OnLog;
            try { ProductionViewport.Cleanup(); } catch { }
            Directory.CreateDirectory(Out);
            var all = new List<string> { $"Mission integration tests — {DateTime.Now:yyyy-MM-dd HH:mm:ss}", $"PASS {pass}  FAIL {fail}", "" };
            all.AddRange(lines);
            all.Add(""); all.Add("Queued commands (local only, never sent):");
            all.AddRange(IntegrationServices.Sessions.Pending.Select(c => c.ToBrokerJson()));
            all.Add(""); all.Add("Logged errors:"); all.AddRange(errors.Distinct().Take(50));
            File.WriteAllLines(Path.Combine(Out, "Mission_Integration_Results.txt"), all);
            EditorApplication.Exit(fail == 0 ? 0 : 1);
        }
    }
}
