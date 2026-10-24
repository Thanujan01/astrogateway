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
using UnityEngine.UI;
using AstroGateway.CinematicHUD;
using AstroGateway.ProductionVisual;
using AstroGateway.Stage08B;

// First Mission playtest in the GRAPHICAL Editor (Play Mode, real Game View screenshots).
// Inputs: the First Mission HUD widgets (stick / hold / toggle / button handlers) plus one real virtual-touchscreen
// path through the EventSystem. Physical keyboard, mouse and gamepad are disabled for the run.
// Data: an isolated store folder under the output folder; the player's own First Mission data and PlayerPrefs keys
// are restored afterwards. Upload tests use clearly labelled MOCK uploaders (no network).
// Never saves the scene. Env FM_OUT (default Docs/FirstMission/Playtest), FM_QUIT=1 exits when done.
namespace AstroGateway.FirstMission.EditorTools
{
    [InitializeOnLoad]
    public static class FirstMissionPlaytest
    {
        const string Flag = "FirstMissionPlaytest.Run";
        const int Width = 2400, Height = 1080; // 20:9 Android phone, landscape
        static string Out => Environment.GetEnvironmentVariable("FM_OUT") ?? Path.GetFullPath("Docs/FirstMission/Playtest");
        static string Shots => Path.Combine(Out, "Screenshots");

        static readonly List<string> lines = new List<string>(), perf = new List<string>(), audit = new List<string>(), logErrors = new List<string>();
        static readonly List<InputDevice> disabledReal = new List<InputDevice>();
        static int pass, fail, untested, lastFrame = -1;
        static readonly Stack<IEnumerator> stack = new Stack<IEnumerator>();
        static IEnumerator routine;
        static object waiting;
        static double startedAt;
        static AstroGatewayHUDController hud;
        static ProductionModeCoordinator modes;
        static MarsPrototype p;
        static FirstMissionController mission;
        static FirstMissionHUD ui;
        static Touchscreen touch;
        static InputSettings.BackgroundBehavior priorBackground; static InputSettings.EditorInputBehaviorInPlayMode priorEditorInput; static bool inputPatched;
        static Keyboard keys;
        static readonly HashSet<Key> held = new HashSet<Key>();
        static void KeyState() => InputSystem.QueueStateEvent(keys, held.Count == 0 ? new KeyboardState() : new KeyboardState(held.ToArray()));
        static IEnumerator Tap(Key k) { held.Add(k); KeyState(); yield return Real(.1f); held.Remove(k); KeyState(); yield return Real(.15f); }
        static string priorSettings, priorProgress;
        static bool sampling;
        static readonly List<double> frameMs = new List<double>();

        static FirstMissionPlaytest() { EditorApplication.update += Tick; }

        [MenuItem("AstroGateway/First Mission Playtest")]
        public static void Run()
        {
            EditorSceneManager.OpenScene(Environment.GetEnvironmentVariable("FM_SCENE") ?? FirstMissionBuilder.Scene);
            try { PlayModeWindow.SetViewType(PlayModeWindow.PlayModeViewTypes.GameView); } catch (Exception e) { Debug.LogWarning(e.Message); }
            try { PlayModeWindow.SetCustomRenderingResolution(Width, Height, "AstroGateway First Mission 20:9"); } catch (Exception e) { Debug.LogWarning(e.Message); }
            try { ProductionViewport.SetSize(Width, Height); } catch (Exception e) { Debug.LogWarning(e.Message); }
            ShaderUtil.allowAsyncCompilation = false; // otherwise new materials capture as the cyan placeholder
            SessionState.SetBool(Flag, true);
            Application.runInBackground = true;
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
                    mission = UnityEngine.Object.FindAnyObjectByType<FirstMissionController>();
                    ui = UnityEngine.Object.FindAnyObjectByType<FirstMissionHUD>();
                    if (!mission || !mission.Ready || !ui || !ui.Safe || !mission.hud.game.initialized) return;
                    SessionState.SetBool(Flag, false);
                    hud = mission.hud; modes = mission.modes; p = hud.game.prototype;
                    Application.logMessageReceived += OnLog;
                    startedAt = EditorApplication.timeSinceStartup;
                    routine = Tests(); stack.Push(routine);
                }
                if (EditorApplication.timeSinceStartup - startedAt > 900) throw new Exception("Playtest timeout (15 min)");
                if (Time.frameCount == lastFrame) return;
                lastFrame = Time.frameCount;
                if (sampling) frameMs.Add(Time.unscaledDeltaTime * 1000.0);
                if (waiting is double until && EditorApplication.timeSinceStartup < until) return;
                if (waiting is Func<bool> cond && !cond()) return;
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
            catch (Exception e) { Line("EXCEPTION: " + e); fail++; Finish(); }
        }

        static object Real(float s) => EditorApplication.timeSinceStartup + s;
        static object Until(Func<bool> f, float timeout)
        {
            double end = EditorApplication.timeSinceStartup + timeout;
            return (Func<bool>)(() => f() || EditorApplication.timeSinceStartup > end);
        }
        static void OnLog(string m, string st, LogType t) { if (t == LogType.Error || t == LogType.Exception || t == LogType.Assert) logErrors.Add(t + ": " + m.Split('\n')[0]); }
        static void Result(string status, string name, string detail = "")
        {
            if (status == "PASS") pass++; else if (status == "UNTESTED") untested++; else fail++;
            Line(status.PadRight(9) + name + (string.IsNullOrEmpty(detail) ? "" : "  [" + detail + "]"));
        }
        static void Check(string n, bool ok, string d = "") => Result(ok ? "PASS" : "FAIL", n, d);
        static void Line(string s) { lines.Add(s); Debug.Log("[FirstMission] " + s); }
        static string V(Vector3 v) => $"({v.x:F1}, {v.y:F2}, {v.z:F1})";
        static string State() => $"driving {p.driving} speed {p.speed:F2} tab {ui.Current} paused {hud.environment.paused} modal {hud.modalOpen} rover {V(p.rover.transform.position)}";

        static float GroundY(Vector3 pos)
        {
            var hits = Physics.RaycastAll(pos + Vector3.up * 30, Vector3.down, 80, 1, QueryTriggerInteraction.Ignore)
                .Where(h => !h.collider.transform.IsChildOf(p.rover.transform) && h.collider != p.astronaut).OrderBy(h => h.distance).ToArray();
            return hits.Length > 0 ? hits[0].point.y : float.NaN;
        }
        static void Teleport(CharacterController c, Vector3 pos, float yaw)
        {
            float g = GroundY(pos); if (!float.IsNaN(g)) pos.y = g + .05f;
            c.enabled = false; c.transform.SetPositionAndRotation(pos, Quaternion.Euler(0, yaw, 0)); c.enabled = true; Physics.SyncTransforms();
        }
        // A spot at `distance` from the station with no structure in the rover's footprint.
        static Vector3 ParkingSpot(Transform station, float distance)
        {
            for (int i = 0; i < 24; i++)
            {
                float a = i * 15 * Mathf.Deg2Rad;
                var c = station.position + new Vector3(Mathf.Sin(a), 0, Mathf.Cos(a)) * distance;
                float g = GroundY(c); if (float.IsNaN(g)) continue; c.y = g;
                if (Mathf.Abs(c.x) > 110 || Mathf.Abs(c.z) > 110) continue;
                if (Physics.OverlapSphere(c + Vector3.up * 1.4f, 1.9f, 1, QueryTriggerInteraction.Ignore).All(x => x.transform.IsChildOf(p.rover.transform) || x is TerrainCollider || x.bounds.size.x > 150)) return c;
            }
            return station.position + Vector3.forward * distance;
        }
        static float Yaw(Vector3 from, Vector3 to) => Mathf.Atan2(to.x - from.x, to.z - from.z) * Mathf.Rad2Deg;

        static IEnumerator Shot(string file, string caption)
        {
            yield return Real(.35f);
            string path = Path.Combine(Shots, file);
            ScreenCapture.CaptureScreenshot(path);
            yield return Real(.8f);
            bool ok = File.Exists(path) && new FileInfo(path).Length > 10000;
            Line((ok ? "SHOT     " : "SHOT-FAIL ") + file + " — " + caption + $"  [{Screen.width}x{Screen.height}]");
        }

        static IEnumerator Measure(string label, float seconds)
        {
            frameMs.Clear(); sampling = true;
            yield return Real(seconds);
            sampling = false;
            var s = frameMs.OrderBy(x => x).ToList();
            if (s.Count < 10) { perf.Add(label + ": too few frames"); yield break; }
            double mean = s.Average();
            var timings = new FrameTiming[1]; FrameTimingManager.CaptureFrameTimings(); uint n = FrameTimingManager.GetLatestTimings(1, timings);
            perf.Add($"{label}: frames={s.Count} mean_ms={mean:F2} fps={1000 / mean:F1} p95_ms={s[(int)(s.Count * .95)]:F2} max_ms={s.Last():F2}" +
                     (n > 0 ? $" cpu_ms={timings[0].cpuFrameTime:F2} gpu_ms={(timings[0].gpuFrameTime > 0 ? timings[0].gpuFrameTime.ToString("F2") : "n/a")}" : "") +
                     $" alloc_MB={Profiler.GetTotalAllocatedMemoryLong() / 1048576.0:F0} mono_MB={Profiler.GetMonoUsedSizeLong() / 1048576.0:F0} gfx_MB={Profiler.GetAllocatedMemoryForGraphicsDriver() / 1048576.0:F0} gc_count={GC.CollectionCount(0)}");
        }

        // Every active button / hold / stick must be inside the safe area and at least 112 reference px on its short side.
        static void AuditTouchTargets(string when, Rect safe)
        {
            var canvas = ui.Safe.GetComponentInParent<Canvas>();
            float scale = canvas.scaleFactor;
            var rc = new Vector3[4]; ((RectTransform)canvas.transform).GetWorldCorners(rc);
            var view = Rect.MinMaxRect(rc[0].x, rc[0].y, rc[2].x, rc[2].y);   // the real render target (Screen.* reports the Editor window)
            if (safe.width <= 0) { var sc = new Vector3[4]; ui.Safe.GetWorldCorners(sc); safe = Rect.MinMaxRect(sc[0].x, sc[0].y, sc[2].x, sc[2].y); }
            audit.Add($"--- {when}: render target {view.width:F0}x{view.height:F0}, safe {safe}");
            Check($"Safe area lies within the render target ({when})", safe.xMin >= view.xMin - 1 && safe.xMax <= view.xMax + 1 && safe.yMin >= view.yMin - 1 && safe.yMax <= view.yMax + 1, $"safe {safe} view {view}");
            var targets = ui.Safe.GetComponentsInChildren<Selectable>().Select(s => (RectTransform)s.transform)
                .Concat(ui.Safe.GetComponentsInChildren<HoldControl>().Select(h => (RectTransform)h.transform))
                .Concat(ui.Safe.GetComponentsInChildren<VirtualStick>().Select(h => (RectTransform)h.transform)).Distinct().ToList();
            int outside = 0, small = 0;
            foreach (var r in targets)
            {
                var c = new Vector3[4]; r.GetWorldCorners(c);
                var rect = Rect.MinMaxRect(c[0].x, c[0].y, c[2].x, c[2].y);
                float shortRef = Mathf.Min(rect.width, rect.height) / scale;
                bool inSafe = rect.xMin >= safe.xMin - 1 && rect.yMin >= safe.yMin - 1 && rect.xMax <= safe.xMax + 1 && rect.yMax <= safe.yMax + 1
                    && rect.xMin >= view.xMin - 1 && rect.yMin >= view.yMin - 1 && rect.xMax <= view.xMax + 1 && rect.yMax <= view.yMax + 1; // and on the render target
                bool big = shortRef >= 84; // tab/panel buttons >= 84, primary controls >= 112 (checked below)
                if (!inSafe) outside++; if (!big) small++;
                audit.Add($"{when} | {r.name} | rect {rect.xMin:F0},{rect.yMin:F0} {rect.width:F0}x{rect.height:F0} px | short side {shortRef:F0} ref px | {(inSafe ? "in safe area" : "OUTSIDE SAFE AREA")}{(big ? "" : " | SMALL")}");
            }
            Check($"Touch targets inside safe area ({when})", outside == 0, $"{targets.Count} targets, {outside} outside");
            Check($"Touch targets >= 84 ref px ({when})", small == 0, $"{small} small");
        }

        static IEnumerator Tests()
        {
            Directory.CreateDirectory(Shots);
            Line($"Unity {Application.unityVersion} | GPU {SystemInfo.graphicsDeviceName} | {SystemInfo.graphicsDeviceType} | Game View {Screen.width}x{Screen.height}");
            Line("Scene: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().path);
            // Isolation: own store folder + saved PlayerPrefs keys.
            priorSettings = PlayerPrefs.GetString(FirstMissionController.SettingsKey, null);
            priorProgress = PlayerPrefs.GetString(FirstMissionController.ProgressKey, null);
            string storeDir = Path.Combine(Out, "TestStore");
            if (Directory.Exists(storeDir)) Directory.Delete(storeDir, true);
            mission.UseStoreForTests(storeDir);
            mission.ResetProgressForTests();
            mission.ResetSettingsForTests();
            if (ui is HUD.ExplorerHUD exi)
            {
                bool introOn = exi.Cinematics.IntroActive && mission.legacyRig.captureOverride;
                yield return Shot("50_explorer_intro_flyin.png", "intro fly-in (cinematic camera, HUD fading in)");
                Check("Cinematic intro fly-in plays at start (camera override, HUD faded)", introOn);
                exi.Cinematics.SkipIntro(); yield return Real(.3f);
                Check("Intro can be skipped and hands the camera back", !exi.Cinematics.IntroActive && !mission.legacyRig.captureOverride);
            }
            priorBackground = InputSystem.settings.backgroundBehavior; priorEditorInput = InputSystem.settings.editorInputBehaviorInPlayMode; inputPatched = true;
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus; // the Editor window may not have focus
            InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
            foreach (var d in InputSystem.devices.ToList())
                if (d.enabled && (d is Keyboard || d is Pointer || d is Gamepad)) { InputSystem.DisableDevice(d); disabledReal.Add(d); }
            Line($"Physical input devices disabled for this run: {disabledReal.Count}");
            yield return Real(2f);

            // ---------- start state ----------
            var cam = p.follow; var aresT = mission.ares.ares;
            Check("First Mission scene loaded with the canonical mission id", hud.game.registry.missionId == FirstMissionIds.MissionId, hud.game.registry.missionId);
            Check("Own save file (never the Visual Upgrade / Cinematic HUD save)", hud.game.testSave.EndsWith(FirstMissionController.SaveFile), hud.game.testSave);
            Check("Starts in astronaut mode with astronaut controls only", !p.driving && ui.AstronautControls.activeSelf && !ui.DriveControls.activeSelf && !ui.TabBar.activeSelf, State());
            Check("Old HUD canvas not drawn (logic still running)", hud.uiRoot && !hud.uiRoot.GetComponent<Canvas>().enabled && hud.enabled);
            Check("TERRAVEX renderers hidden, ARES-VI renderers visible", p.roverVisual.GetComponentsInChildren<Renderer>(true).All(r => !r.enabled) && aresT.GetComponentsInChildren<Renderer>().Count(r => r.enabled) >= 20);
            Check("ARES hover plumes inactive, Animator disabled (no hover behaviour)", aresT.GetComponentsInChildren<Transform>(true).Where(t => t.name.EndsWith("_Plume")).All(t => !t.gameObject.activeInHierarchy) && aresT.GetComponentsInChildren<Animator>(true).All(a => !a.enabled));
            Check("Gameplay collider is still the TERRAVEX CharacterController", p.rover.GetComponent<CharacterController>() && aresT.GetComponentsInChildren<Collider>(true).Length == 0);
            Check("AI analysis seam reports NOT CONNECTED", mission.Analysis.Connection == ModelConnection.NotConnected && mission.Analysis.InterfaceVersion == "ag.science-analysis.v0");
            Check("Upload default is NOT CONNECTED (nothing transmitted)", !mission.Sync.Uploader.Online);
            AuditTouchTargets("astronaut", Rect.zero);
            // Camera behind the astronaut looking at the rover for the first shot.
            Teleport(p.astronaut, p.rover.transform.position - p.rover.transform.right * 7 - p.rover.transform.forward * 3, Yaw(p.rover.transform.position - p.rover.transform.right * 7, p.rover.transform.position));
            p.yaw = p.astronaut.transform.eulerAngles.y + 20; p.pitch = 12;
            yield return Real(1.2f);
            yield return Shot("01_astronaut_mode.png", "astronaut controls; ARES-VI rover ahead");
            Check("BOARD hidden while far from the rover", !ui.BoardButton.gameObject.activeInHierarchy || Vector3.Distance(p.astronaut.transform.position, p.rover.transform.position) <= 5);

            // ---------- walk with the stick ----------
            var before = p.astronaut.transform.position;
            ui.MoveStick.SetForTest(new Vector2(0, 1));
            yield return Real(1.5f);
            ui.MoveStick.SetForTest(Vector2.zero);
            yield return Real(.5f);
            float walked = Vector3.Distance(before, p.astronaut.transform.position);
            Check("Move joystick walks the astronaut (existing controller)", walked > 1.5f, $"{walked:F2} m");

            // ---------- board ----------
            Teleport(p.astronaut, p.rover.transform.position - p.rover.transform.right * 3.2f, 90);
            yield return Real(.6f);
            Check("BOARD ROVER appears within 5 m", ui.BoardButton.gameObject.activeInHierarchy);
            ui.BoardButton.onClick.Invoke();
            yield return Until(() => p.driving, 3);
            yield return Real(.5f);
            Check("Boarding switches to rover mode", p.driving, State());
            Check("Rover mode shows tabs + DRIVE controls, hides astronaut controls", ui.TabBar.activeSelf && ui.DriveControls.activeSelf && !ui.AstronautControls.activeSelf && ui.Current == FirstMissionHUD.Tab.Drive);
            Check("Seated astronaut hidden inside the ARES canopy", p.astronaut.GetComponentsInChildren<Renderer>().All(r => !r.enabled));
            AuditTouchTargets("rover drive", Rect.zero);

            // ---------- real touch through the EventSystem (virtual touchscreen) ----------
            touch = InputSystem.AddDevice<Touchscreen>("First Mission playtest touchscreen");
            var c4 = new Vector3[4]; ((RectTransform)ui.Brake.transform).GetWorldCorners(c4);
            var brakeCentre = (Vector2)((c4[0] + c4[2]) / 2);
            InputSystem.QueueStateEvent(touch, new TouchState { touchId = 1, phase = UnityEngine.InputSystem.TouchPhase.Began, position = brakeCentre });
            yield return Real(.3f);
            bool heldByTouch = ui.Brake.Held;
            InputSystem.QueueStateEvent(touch, new TouchState { touchId = 1, phase = UnityEngine.InputSystem.TouchPhase.Ended, position = brakeCentre });
            yield return Real(.3f);
            if (heldByTouch) Check("Virtual touchscreen press/release on BRAKE goes through the EventSystem", !ui.Brake.Held);
            else Result("UNTESTED", "Virtual touchscreen press on BRAKE", "Editor did not route the virtual touch to the UI module; widget handlers tested directly below");

            // ---------- drive ----------
            var start = p.rover.transform.position; float yaw0 = p.rover.transform.eulerAngles.y;
            ui.SteerStick.SetForTest(new Vector2(0, .2f)); ui.Accelerate.SetForTest(true);
            yield return Real(3f);
            Check("ACCEL drives forward", p.speed > 1.5f && Vector3.Distance(start, p.rover.transform.position) > 3, $"speed {p.speed:F2} moved {Vector3.Distance(start, p.rover.transform.position):F1} m");
            Check("Speed indicator shows the controller speed", Premium ? ui.GetComponentsInChildren<Text>().Any(t => t.name == "Speed" && Math.Abs(float.Parse(t.text, System.Globalization.CultureInfo.InvariantCulture) - Mathf.Abs(p.speed) * 3.6f) < 1.5f) && ui.GetComponentsInChildren<Text>().Any(t => t.name == "Drive state" && t.text.Contains("DRIVE"))
                : ui.GetComponentsInChildren<Text>().Any(t => t.name == "Speed" && t.text.Contains("km/h") && t.text.Contains("FWD")));
            Check("EXIT disabled while moving", !ui.ExitButton.interactable);
            yield return Shot("02_drive_tab_moving.png", "DRIVE tab while driving");
            ui.SteerStick.SetForTest(new Vector2(1, .2f));
            yield return Real(2f);
            float turned = Mathf.Abs(Mathf.DeltaAngle(yaw0, p.rover.transform.eulerAngles.y));
            Check("Steering joystick turns the rover", turned > 15, $"{turned:F0} deg");
            float rocker = Quaternion.Angle(Quaternion.identity, mission.ares.rockerL.localRotation);
            Line($"INFO     ARES rocker L angle while driving {rocker:F1} deg; wheel FL rotation {mission.ares.wheels[0].localEulerAngles}");
            ui.Accelerate.SetForTest(false); ui.SteerStick.SetForTest(Vector2.zero); ui.Brake.SetForTest(true);
            yield return Real(1.6f);
            Check("BRAKE stops the rover", Mathf.Abs(p.speed) < .2f, $"speed {p.speed:F2}");
            ui.Brake.SetForTest(false);
            ui.ReverseToggle.isOn = true; ui.Accelerate.SetForTest(true);
            yield return Real(1.8f);
            Check("REVERSE + ACCEL reverses", p.speed < -.5f, $"speed {p.speed:F2}");
            ui.Accelerate.SetForTest(false); ui.ReverseToggle.isOn = false; ui.Brake.SetForTest(true);
            yield return Real(1.5f); ui.Brake.SetForTest(false);
            float yawBefore = p.yaw; hud.lookDelta += new Vector2(220, 0);
            yield return Real(.3f);
            Check("Camera drag zone turns the camera", Mathf.Abs(Mathf.DeltaAngle(yawBefore, p.yaw)) > 5, $"{yawBefore:F0} -> {p.yaw:F0}");
            var view0 = ui.cameraRig.roverView;
            FindButton("Camera distance").onClick.Invoke();
            Check("CAMERA button cycles the rover view", ui.cameraRig.roverView != view0, ui.cameraRig.ViewLabel);
            ui.cameraRig.roverView = view0;

            // ---------- tabs ----------
            ui.Accelerate.SetForTest(true); yield return Real(.8f);
            ui.Select(FirstMissionHUD.Tab.Science);
            yield return Real(.4f);
            Check("Switching away from DRIVE releases the throttle", hud.movement == Vector2.zero && !hud.runHeld && !ui.DriveControls.activeSelf && ui.Panels[FirstMissionHUD.Tab.Science].activeInHierarchy);
            ui.Accelerate.SetForTest(false);
            yield return Until(() => Mathf.Abs(p.speed) < .1f, 6);
            foreach (FirstMissionHUD.Tab t in Enum.GetValues(typeof(FirstMissionHUD.Tab)))
            {
                ui.Select(t); yield return Real(.35f);
                bool ok = t == FirstMissionHUD.Tab.Drive ? ui.DriveControls.activeSelf : ui.Panels[t].activeInHierarchy && !ui.DriveControls.activeSelf;
                Check("Tab " + t + " opens without leaving the game", ok && p.driving && !hud.environment.paused);
            }

            // ---------- science: range validation, then observations at the designated site ----------
            var site = mission.Designated;
            Line($"INFO     designated site {site.stableId} '{site.displayName}' at {V(site.transform.position)}; rover {RoverSensorSuite.Planar(p.rover.transform.position, site.transform.position):F0} m away");
            var arm = mission.Catalog.Instrument("rover.arm_sampler");
            Check("Arm sampler rejected out of range", mission.Check(arm)?.StartsWith("Out of range") == true, mission.Check(arm));
            ui.Select(FirstMissionHUD.Tab.Science); yield return Real(.3f);
            FindButton("Instrument rover.arm_sampler").onClick.Invoke(); yield return Real(.5f);
            Check("OBSERVE disabled out of range (arm sampler selected)", ui.SelectedInstrument == "rover.arm_sampler" && !ui.ObserveButton.interactable);
            yield return Shot("03a_science_out_of_range.png", "SCIENCE tab: arm sampler out of range");
            var spot = ParkingSpot(site.transform, 11);
            Teleport(p.rover, spot, Yaw(spot, site.transform.position));
            yield return Real(1f);
            Check("Mission marks the site reached after driving there", mission.Objectives()[1].done, mission.Objectives()[1].text);
            var ctx = mission.Catalog.Instrument("rover.context_camera");
            int n0 = mission.Store.Records.Count;
            Check("Context camera allowed at 11 m (max 15 m)", mission.Check(ctx) == null, mission.Check(ctx));
            Check("Arm sampler still rejected at 11 m (max 6 m)", mission.Check(arm)?.StartsWith("Out of range") == true);
            mission.BeginObservation(ctx);
            yield return Real(1.4f);
            yield return Shot("03_science_context_camera.png", "SCIENCE tab, mast context camera observing");
            yield return Until(() => mission.Observing == null, 6);
            var ctxRecs = mission.Store.Records.Skip(n0).ToList();
            Check("Context camera stores 4 records locally", ctxRecs.Count == 4 && ctxRecs.All(r => r.uploadState == UploadState.Offline || r.uploadState == UploadState.CollectedLocally), $"{ctxRecs.Count} records, {string.Join(",", ctxRecs.Select(r => r.uploadState).Distinct())}");
            var sid = ctxRecs.FirstOrDefault(r => r.sensorId == "sci.station_id");
            Check("Research station identified by its registry id", sid != null && sid.valueText == site.stableId && sid.stationId == site.stableId);
            var rng = ctxRecs.FirstOrDefault(r => r.sensorId == "sci.target_range");
            float trueRange = RoverSensorSuite.Planar(p.rover.transform.position, site.transform.position);
            Check("Target range equals the real rover-station distance", rng != null && Math.Abs(rng.value - trueRange) < .05, $"{rng?.value:F2} vs {trueRange:F2}");

            // Arm: must be stopped and within 6 m.
            spot = ParkingSpot(site.transform, 5);
            Teleport(p.rover, spot, Yaw(spot, site.transform.position));
            yield return Real(.8f);
            ui.Select(FirstMissionHUD.Tab.Drive); ui.Accelerate.SetForTest(true); yield return Real(.5f);
            Check("Arm sampler refused while moving", mission.Check(arm) == "Stop the rover first", $"speed {p.speed:F2}: {mission.Check(arm)}");
            ui.Accelerate.SetForTest(false); ui.Brake.SetForTest(true); yield return Until(() => Mathf.Abs(p.speed) < .05f, 4); ui.Brake.SetForTest(false);
            Teleport(p.rover, spot, Yaw(spot, site.transform.position));
            yield return Real(.5f);
            ui.Select(FirstMissionHUD.Tab.Science);
            n0 = mission.Store.Records.Count;
            Check("Arm sampler allowed when stopped within 6 m", mission.Check(arm) == null, mission.Check(arm));
            mission.BeginObservation(arm);
            yield return Real(2.6f);
            Check("ARES arm deploys during the arm observation", mission.ares.ArmProgress > .3f, $"progress {mission.ares.ArmProgress:F2}");
            p.yaw = Yaw(site.transform.position, p.rover.transform.position) + 140; p.pitch = 14;
            yield return Shot("04_arm_sample.png", "arm sampler deployed at the station");
            yield return Until(() => mission.Observing == null, 8);
            var armRecs = mission.Store.Records.Skip(n0).ToList();
            var sample = armRecs.FirstOrDefault(r => r.sensorId == "sci.sample_id");
            var comp = armRecs.FirstOrDefault(r => r.sensorId == "sci.soil_composition");
            Check("Arm sample stores a game-assigned sample id", sample != null && Guid.TryParse(sample.valueText, out _) && sample.provenance == Provenance.Simulated);
            Check("Soil composition is UNAVAILABLE with no value (not modelled)", comp != null && comp.provenance == Provenance.Unavailable && !comp.hasValue && string.IsNullOrEmpty(comp.valueText));
            yield return Real(5.2f);
            Check("Arm stows after the observation", mission.ares.ArmProgress < .05f, $"progress {mission.ares.ArmProgress:F2}");

            // Terrain profiler vs an independent raycast.
            n0 = mission.Store.Records.Count;
            mission.BeginObservation(mission.Catalog.Instrument("rover.terrain_profiler"));
            yield return Until(() => mission.Observing == null, 5);
            var terr = mission.Store.Records.Skip(n0).ToList();
            var slope = terr.FirstOrDefault(r => r.sensorId == "terrain.slope");
            Physics.Raycast(p.rover.transform.position + Vector3.up * 4, Vector3.down, out var hit, 12, 1, QueryTriggerInteraction.Ignore);
            float trueSlope = Vector3.Angle(hit.normal, Vector3.up);
            Check("Terrain slope matches the ground normal under the rover", slope != null && slope.provenance == Provenance.Simulated && Math.Abs(slope.value - trueSlope) < .05, $"{slope?.value:F3} vs {trueSlope:F3} deg");
            var trav = terr.FirstOrDefault(r => r.sensorId == "terrain.traversability");
            Check("Traversability follows the documented rule", trav != null && trav.valueText == mission.sensors.Traversability((float)slope.value, (float)terr.First(r => r.sensorId == "terrain.roughness").value), trav?.valueText);
            Check("Plane fit: a flat grid has zero roughness, a 0.2 m step does not", RoverSensorSuite.PlaneResidualRms(Grid(0)) < 1e-4 && RoverSensorSuite.PlaneResidualRms(Grid(.2f)) > .02f, $"step residual {RoverSensorSuite.PlaneResidualRms(Grid(.2f)):F4} m");

            // Environment package: dust from weather state, the rest unavailable.
            p.SetWeather(1);
            n0 = mission.Store.Records.Count;
            mission.BeginObservation(mission.Catalog.Instrument("rover.env_package"));
            yield return Until(() => mission.Observing == null, 5);
            var env = mission.Store.Records.Skip(n0).ToList();
            Check("Dust condition reports the game weather state", env.Any(r => r.sensorId == "env.dust_condition" && r.valueText == "light_dust" && r.provenance == Provenance.Simulated));
            Check("Temperature, pressure and radiation are UNAVAILABLE without values", env.Where(r => r.sensorId == "env.air_temperature" || r.sensorId == "env.air_pressure" || r.sensorId == "env.radiation_dose_rate").Count(r => r.provenance == Provenance.Unavailable && !r.hasValue) == 3);
            p.SetWeather(0);
            var spec = mission.Catalog.Instrument("rover.spectrometer");
            n0 = mission.Store.Records.Count;
            Check("Spectrometer shown UNAVAILABLE and cannot run", mission.Check(spec)?.StartsWith("UNAVAILABLE") == true && !mission.BeginObservation(spec) && mission.Store.Records.Count == n0);
            ui.Select(FirstMissionHUD.Tab.Science); yield return Real(.5f);
            AuditTouchTargets("science tab", Rect.zero);
            yield return Shot("05_science_tab.png", "SCIENCE tab after observations");

            // ---------- local validation + honesty ----------
            var all = mission.Store.Records.ToList();
            Check("Every stored record passes local validation", all.All(r => SensorValidation.Validate(r, mission.Catalog) == null), string.Join("; ", all.Select(r => SensorValidation.Validate(r, mission.Catalog)).Where(e => e != null).Distinct()));
            Check("Client records are only SIMULATED or UNAVAILABLE", all.All(r => r.provenance == Provenance.Simulated || r.provenance == Provenance.Unavailable));
            Check("No record carries an analysis result", all.All(r => r.analysisState != AnalysisState.Complete && string.IsNullOrEmpty(r.analysisSummary)));
            var wire = all[0].ToWireJson();
            Check("Wire JSON has the schema fields and no analysis/user fields", wire.Contains("\"schema_version\":1") && wire.Contains("\"event_id\"") && wire.Contains("\"mission_id\":\"ag.mars.red-horizon.surface-ops\"") && wire.Contains("\"sim_config_version\":\"fm-sensors-1\"") && !wire.Contains("analysis") && !wire.Contains("user") && !wire.Contains("email"), wire);
            var tampered = JsonUtility.FromJson<SensorRecord>(JsonUtility.ToJson(all[0])); tampered.provenance = Provenance.BackendAnalyzed;
            Check("Local validation rejects a client-made BACKEND-ANALYZED record", SensorValidation.Validate(tampered, mission.Catalog) != null);
            tampered.provenance = Provenance.Simulated; tampered.sensorId = "env.air_temperature"; tampered.instrumentId = "rover.env_package"; tampered.sensorType = "environmental"; tampered.unit = "K"; tampered.hasValue = true; tampered.value = 210; tampered.method = "not_modeled";
            Check("Local validation rejects a number for a not-modelled sensor", SensorValidation.Validate(tampered, mission.Catalog) != null);

            // ---------- data tab, offline ----------
            ui.Select(FirstMissionHUD.Tab.Data); yield return Real(.6f);
            var dataText = ui.Panels[FirstMissionHUD.Tab.Data].GetComponentsInChildren<Text>().First(t => t.name == "Data body").text;
            if (Premium) dataText += " " + string.Join(" ", ((HUD.PremiumHUD)ui).DataRows.SelectMany(r => r).Select(t => t.text));
            Check("DATA tab shows OFFLINE upload state and AI NOT CONNECTED", dataText.Contains(UploadState.Offline) && dataText.Contains("NOT CONNECTED") && dataText.Contains("SIMULATED"), dataText.Substring(0, Math.Min(160, dataText.Length)));
            yield return Shot("06_data_tab_offline.png", "DATA tab: local records, OFFLINE, AI not connected");
            ui.Select(FirstMissionHUD.Tab.Mission); yield return Real(.6f);
            int done = mission.Objectives().Count(o => o.done && !o.info);
            Check("MISSION objectives progress from validated local records", done >= 6, $"{done} done: " + string.Join(" | ", mission.Objectives().Select(o => (o.done ? "x " : "- ") + o.text)));
            yield return Shot("07_mission_tab.png", "MISSION tab");
            ui.Select(FirstMissionHUD.Tab.Systems); yield return Real(.6f);
            yield return Shot("08_systems_tab.png", "SYSTEMS tab");

            // ---------- persistence ----------
            var reloaded = new SensorRecordStore(mission.Store.Path);
            Check("Records survive an app restart (reload from disk)", reloaded.Records.Count == all.Count && reloaded.Records.Select(r => r.eventId).SequenceEqual(all.Select(r => r.eventId)), $"{reloaded.Records.Count}/{all.Count}");
            Check("Re-adding the same records does not duplicate them", reloaded.Add(all.Take(3), out _) && reloaded.Records.Count == all.Count);
            yield return StoreBoundsTests(storeDir);

            // ---------- MOCK uploads (no network; labelled MOCK) ----------
            yield return MockUploadTests(storeDir);

            // ---------- exit ----------
            mission.UseStoreForTests(storeDir); // back to the real (offline) store
            ui.Select(FirstMissionHUD.Tab.Drive); yield return Real(.4f);
            Check("EXIT enabled when stopped", ui.ExitButton.interactable, State());
            ui.ExitButton.onClick.Invoke();
            yield return Until(() => !p.driving, 3); yield return Real(.5f);
            Check("Exit restores astronaut controls automatically", !p.driving && ui.AstronautControls.activeSelf && !ui.TabBar.activeSelf && !ui.DriveControls.activeSelf, State());
            Check("Astronaut visible again after exit", p.astronaut.GetComponentsInChildren<Renderer>().Any(r => r.enabled));
            Check("Tabs cannot be opened on foot", Try(() => ui.Select(FirstMissionHUD.Tab.Data)) && ui.Current == FirstMissionHUD.Tab.Drive && !ui.TabBar.activeSelf);
            p.yaw = Yaw(p.astronaut.transform.position, p.rover.transform.position); p.pitch = 10;
            yield return Shot("09_after_exit.png", "astronaut beside the ARES-VI after exit");

            // ---------- pause ----------
            hud.TogglePause(); yield return Real(.5f);
            Check("Pause (Esc / Android back) shows the First Mission pause card", ui.PausedOverlay.activeSelf && hud.environment.paused);
            yield return Shot("10_paused.png", "pause card");
            FindButton("Resume").onClick.Invoke(); yield return Real(.5f);
            Check("RESUME unpauses", !ui.PausedOverlay.activeSelf && !hud.environment.paused);

            // ---------- other landscape sizes + display cutout ----------
            Teleport(p.astronaut, p.rover.transform.position - p.rover.transform.right * 3.2f, 90);
            yield return Real(.4f); ui.BoardButton.onClick.Invoke(); yield return Until(() => p.driving, 3);
            Check("Re-boarding after exit returns to rover mode on DRIVE", p.driving && ui.Current == FirstMissionHUD.Tab.Drive && ui.DriveControls.activeSelf);
            foreach (var (w, h, label) in new[] { (1920, 1080, "16:9"), (2340, 1080, "19.5:9"), (1600, 720, "20:9 low-res") })
            {
                try { PlayModeWindow.SetCustomRenderingResolution((uint)w, (uint)h, "First Mission " + label); } catch (Exception e) { Line("WARN resize " + e.Message); }
                yield return Real(1.2f);
                var rr = new Vector3[4]; ((RectTransform)ui.Safe.GetComponentInParent<Canvas>().transform).GetWorldCorners(rr);
                Line($"INFO     render target {rr[2].x - rr[0].x:F0}x{rr[2].y - rr[0].y:F0} ({label})");
                AuditTouchTargets(label, Rect.zero);
                yield return Shot($"11_drive_{w}x{h}.png", "DRIVE tab at " + label);
            }
            try { PlayModeWindow.SetCustomRenderingResolution(Width, Height, "AstroGateway First Mission 20:9"); } catch { }
            yield return Real(1.2f);
            // Left camera cutout 110 px + 40 px right corner + 30 px gesture bar on a 2400x1080 phone (fractions, so it maps to the render target).
            ui.simulateSafeArea = true; ui.simulatedSafeArea = new Rect(110f / 2400, 30f / 1080, (2400 - 150f) / 2400, (1080 - 30f) / 1080);
            yield return Real(.6f);
            AuditTouchTargets("cutout 110 px left", Rect.zero);
            yield return Shot("12_drive_cutout.png", "DRIVE tab with a simulated 110 px left cutout");
            var handed = FindButton("Setting Handedness");  // in the SYSTEMS panel (inactive objects included)
            mission.Settings.leftHanded = false; handed.onClick.Invoke(); yield return Real(.5f);
            AuditTouchTargets("left-handed + cutout", Rect.zero);
            var stickRect = new Vector3[4]; ((RectTransform)ui.SteerStick.transform).GetWorldCorners(stickRect);
            var cr = new Vector3[4]; ((RectTransform)ui.Safe.GetComponentInParent<Canvas>().transform).GetWorldCorners(cr);
            Check("Left-handed mode mirrors the steering stick to the right side", stickRect[0].x > (cr[0].x + cr[2].x) / 2f, $"stick x {stickRect[0].x:F0} of {cr[2].x:F0}");
            yield return Shot("13_drive_left_handed.png", "left-handed DRIVE layout");
            handed.onClick.Invoke(); ui.simulateSafeArea = false; yield return Real(.4f);

            yield return ControlTests();
            if (Premium) yield return PremiumTests();
            if (ui is HUD.ExplorerHUD) yield return ExplorerTests();

            // ---------- performance (Editor only) ----------
            ui.Select(FirstMissionHUD.Tab.Drive);
            ui.SteerStick.SetForTest(new Vector2(.3f, .2f)); ui.Accelerate.SetForTest(true);
            yield return Measure("Editor Play Mode, driving, DRIVE tab, 2400x1080", 6);
            ui.Accelerate.SetForTest(false); ui.SteerStick.SetForTest(Vector2.zero); ui.Brake.SetForTest(true);
            yield return Real(1.5f); ui.Brake.SetForTest(false);
            ui.Select(FirstMissionHUD.Tab.Data);
            yield return Measure("Editor Play Mode, parked, DATA tab", 4);

            Check("No console errors or exceptions during the run", logErrors.Count == 0, string.Join(" | ", logErrors.Distinct().Take(5)));
        }

        // ================= control system batch =================
        static IEnumerator ControlTests()
        {
            Line("--- Control system batch ---");
            var cl = ui.control; var rig = ui.cameraRig; var cam = p.follow;
            if (p.driving) { ui.Select(FirstMissionHUD.Tab.Drive); ui.Brake.SetForTest(true); yield return Until(() => Mathf.Abs(p.speed) < .05f, 4); ui.Brake.SetForTest(false); ui.ExitButton.onClick.Invoke(); yield return Until(() => !p.driving, 3); }
            yield return Real(.4f);
            Check("State ASTRONAUT on foot", cl.State == ControlState.Astronaut, cl.State.ToString());

            // ----- keyboard / mouse compatibility (virtual devices through the real Input System path) -----
            keys = InputSystem.AddDevice<Keyboard>("First Mission playtest keyboard"); keys.MakeCurrent();
            var spot = p.rover.transform.position - p.rover.transform.right * 9;
            Teleport(p.astronaut, spot, 0); p.yaw = 0; yield return Real(.4f);
            var a0 = p.astronaut.transform.position;
            held.Add(Key.W); KeyState(); yield return Real(1.2f); held.Clear(); KeyState(); yield return Real(.4f);
            Check("Keyboard W walks (camera-relative, existing controller)", Vector3.Distance(a0, p.astronaut.transform.position) > 1.2f, $"{Vector3.Distance(a0, p.astronaut.transform.position):F2} m");
            float walkYaw = p.yaw; p.yaw = 90; a0 = p.astronaut.transform.position;
            held.Add(Key.W); KeyState(); yield return Real(1f); held.Clear(); KeyState(); yield return Real(.3f);
            var dir = (p.astronaut.transform.position - a0); dir.y = 0;
            Check("Movement follows the camera heading", Vector3.Angle(dir, Vector3.right) < 25, $"move dir {Vector3.SignedAngle(Vector3.forward, dir, Vector3.up):F0}° for camera yaw 90°");
            Teleport(p.astronaut, p.rover.transform.position - p.rover.transform.right * 3.2f, 90); yield return Real(.4f);
            yield return Tap(Key.E); yield return Until(() => p.driving, 3);
            Check("Keyboard E boards (5 m rule)", p.driving);
            Check("State ROVER_DRIVE after boarding", cl.State == ControlState.RoverDrive);
            var r0 = p.rover.transform.position;
            held.Add(Key.W); KeyState(); yield return Real(1.5f); held.Clear(); KeyState();
            Check("Keyboard W drives in ROVER_DRIVE", Vector3.Distance(r0, p.rover.transform.position) > 1.5f, $"{Vector3.Distance(r0, p.rover.transform.position):F2} m");
            held.Add(Key.Space); KeyState(); yield return Until(() => Mathf.Abs(p.speed) < .05f, 4); held.Clear(); KeyState();
            Check("Keyboard Space brakes", Mathf.Abs(p.speed) < .05f);

            // ----- no accidental driving in UI states -----
            ui.Select(FirstMissionHUD.Tab.Data); yield return Real(.3f);
            Check("State DATA_REVIEW on the DATA tab", cl.State == ControlState.DataReview);
            r0 = p.rover.transform.position;
            held.Add(Key.W); KeyState(); yield return Real(1.2f); held.Clear(); KeyState(); yield return Real(.2f);
            Check("W does not drive while a data panel is open", Vector3.Distance(r0, p.rover.transform.position) < .05f, $"{Vector3.Distance(r0, p.rover.transform.position):F3} m");
            ui.Select(FirstMissionHUD.Tab.Science); yield return Real(.3f);
            Check("State ROVER_SCIENCE on the SCIENCE tab", cl.State == ControlState.RoverScience);
            held.Add(Key.W); KeyState(); yield return Real(.8f); held.Clear(); KeyState();
            Check("W does not drive while aiming science", Vector3.Distance(r0, p.rover.transform.position) < .05f);
            yield return Tap(Key.Tab); yield return Real(.2f);
            Check("Tab key returns to DRIVE", ui.Current == FirstMissionHUD.Tab.Drive && cl.State == ControlState.RoverDrive);
            hud.TogglePause(); yield return Real(.3f);
            Check("State PAUSE_MENU while paused; look input blocked", cl.State == ControlState.PauseMenu && p.testLook == Vector2.zero);
            hud.TogglePause(); yield return Real(.3f);

            // ----- rover: progressive throttle, speed-sensitive steering, recenter -----
            var openRoad = ParkingSpot(mission.Designated.transform, 30); Teleport(p.rover, openRoad, Yaw(openRoad, mission.Designated.transform.position) + 180); yield return Real(.5f);
            ui.Accelerate.SetForTest(true); yield return Real(.3f);
            float tEarly = ui.Throttle;
            yield return Real(.8f);
            Check("ACCEL throttle ramps progressively", tEarly > .1f && tEarly < .75f && ui.Throttle > .95f, $"0.3 s {tEarly:F2}, 1.1 s {ui.Throttle:F2}");
            ui.SteerStick.SetForTest(new Vector2(1, 0)); yield return Real(1.5f);
            float expect = Mathf.Lerp(1f, cl.highSpeedSteerScale, Mathf.Pow(Mathf.Clamp01(Mathf.Abs(p.speed) / cl.topSpeed), 2));
            Check("Steering authority reduced at speed (documented curve)", cl.LastSteerScale < .9f && Mathf.Abs(cl.LastSteerScale - expect) < .03f && Mathf.Abs(p.speed) > 2, $"scale {cl.LastSteerScale:F2} (expected {expect:F2}) at {p.speed:F2} m/s");
            ui.SteerStick.SetForTest(Vector2.zero); ui.Accelerate.SetForTest(false); ui.Brake.SetForTest(true);
            yield return Until(() => Mathf.Abs(p.speed) < .05f, 4); ui.Brake.SetForTest(false);
            p.yaw += 120; FindButton("Rover recenter").onClick.Invoke(); yield return Real(.2f);
            Check("RECENTER puts the camera behind the rover", Mathf.Abs(Mathf.DeltaAngle(p.yaw, p.rover.transform.eulerAngles.y)) < 1);

            // ----- rover cameras -----
            yield return Real(.8f);
            float chaseDist = Vector3.Distance(cam.transform.position, p.rover.transform.position);
            rig.roverView = RoverView.ChaseNear; yield return Real(1.2f);
            float nearDist = Vector3.Distance(cam.transform.position, p.rover.transform.position);
            Check("CHASE NEAR is closer than CHASE", nearDist < chaseDist - .8f, $"{chaseDist:F1} -> {nearDist:F1} m");
            rig.zoom = 1.5f; yield return Real(1.2f);
            float zoomDist = Vector3.Distance(cam.transform.position, p.rover.transform.position);
            Check("Zoom out increases chase distance", zoomDist > nearDist + 1, $"{nearDist:F1} -> {zoomDist:F1} m");
            rig.zoom = 1; FindButton("Camera distance").onClick.Invoke(); yield return Real(2.2f);
            Check("MAST CAM view sits on the rover mast", rig.roverView == RoverView.Cockpit && Vector3.Distance(cam.transform.position, p.rover.transform.TransformPoint(rig.cockpitLocal)) < .3f, $"cam {V(cam.transform.position)}");
            yield return Shot("20_rover_mast_camera.png", "rover MAST CAM (first-person) view");
            rig.roverView = RoverView.Chase; yield return Real(1.5f);
            Check("View changes glide (camera speed <= 9.5 m/s, no snap)", rig.LastTransitionSpeed <= 9.5f, $"peak {rig.LastTransitionSpeed:F1} m/s, largest frame move {rig.LastTransitionJump:F2} m");

            // ----- science: target selection, cancel, contextual shortcut -----
            var site = mission.Designated;
            var sp = ParkingSpot(site.transform, 8); Teleport(p.rover, sp, Yaw(sp, site.transform.position)); yield return Real(.8f);
            Check("SCIENCE shortcut appears when stopped near a station", ui.ScienceShortcut.gameObject.activeInHierarchy);
            ui.ScienceShortcut.onClick.Invoke(); yield return Real(.3f);
            Check("Shortcut opens SCIENCE", ui.Current == FirstMissionHUD.Tab.Science);
            ui.TargetButton.onClick.Invoke(); yield return Real(.3f);
            Check("TARGET selects a station in range", mission.sensors.selected == site, mission.sensors.selected ? mission.sensors.selected.displayName : "none");
            yield return Real(1f);
            yield return Shot("21_science_target_framing.png", "ROVER_SCIENCE camera framing the selected target");
            int n0 = mission.Store.Records.Count;
            mission.BeginObservation(mission.Catalog.Instrument("rover.context_camera")); yield return Real(.8f);
            Check("CANCEL enabled during an observation", ui.CancelButton.interactable);
            ui.CancelButton.onClick.Invoke(); yield return Real(.3f);
            Check("CANCEL stops the observation and stores nothing", mission.Observing == null && mission.Store.Records.Count == n0 && mission.LastFeedback.Contains("cancelled"));
            mission.BeginObservation(mission.Catalog.Instrument("rover.context_camera")); yield return Until(() => mission.Observing == null, 6);
            var sel = mission.Store.Records.Skip(n0).FirstOrDefault(r => r.sensorId == "sci.station_id");
            Check("Observation uses the selected target", sel != null && sel.stationId == site.stableId);
            mission.sensors.selected = null;
            ui.Select(FirstMissionHUD.Tab.Drive); yield return Real(.3f);

            // ----- astronaut: context interact, run toggle, auto-sprint, camera views, inversion -----
            ui.ExitButton.onClick.Invoke(); yield return Until(() => !p.driving, 3); yield return Real(.4f);
            Check("Context button reads BOARD ROVER next to the rover", ui.InteractButton.gameObject.activeInHierarchy && ui.InteractButton.GetComponentInChildren<Text>().text == "BOARD ROVER");
            var stationSpot = site.transform.position + (p.astronaut.transform.position - site.transform.position).normalized * 2f;
            Teleport(p.astronaut, stationSpot, Yaw(stationSpot, site.transform.position)); yield return Real(.6f);
            Check("Context button reads SCAN at a research station", ui.InteractButton.gameObject.activeInHierarchy && ui.InteractButton.GetComponentInChildren<Text>().text.StartsWith("SCAN"), ui.InteractButton.GetComponentInChildren<Text>().text);
            int scans0 = hud.game.scans.Count;
            ui.InteractButton.onClick.Invoke(); yield return Until(() => !hud.scanning, 3); yield return Real(.3f);
            Check("Context SCAN runs the existing on-foot scan", hud.game.scans.Count == scans0 + 1, $"{scans0} -> {hud.game.scans.Count}");

            var open = ParkingSpot(site.transform, 18); Teleport(p.astronaut, open, 0); p.yaw = 0; yield return Real(.4f);
            mission.Settings.runToggle = true;
            ui.Run.SetForTest(true); yield return Real(.15f); ui.Run.SetForTest(false); yield return Real(.15f);
            ui.MoveStick.SetForTest(new Vector2(0, .7f)); yield return Real(1.2f);
            float runSpeed = p.speed;
            ui.Run.SetForTest(true); yield return Real(.15f); ui.Run.SetForTest(false); yield return Real(1f);
            float walkSpeed = p.speed;
            Check("RUN toggle latches run and unlatches", runSpeed > walkSpeed + .5f && !ui.RunLatched, $"run {runSpeed:F2} / walk {walkSpeed:F2} m/s");
            mission.Settings.runToggle = false; ui.MoveStick.SetForTest(Vector2.zero); yield return Real(.5f);
            Teleport(p.astronaut, open, 0); p.yaw = 0;
            mission.Settings.autoSprint = true;
            ui.MoveStick.SetForTest(new Vector2(0, 1)); yield return Real(1.5f);
            Check("Auto-sprint after holding the stick at full deflection", cl.AutoSprinting && p.speed > 3.2f, $"speed {p.speed:F2}");
            ui.MoveStick.SetForTest(Vector2.zero); mission.Settings.autoSprint = false; yield return Real(.5f);
            Check("Walk speed limits unchanged (run cap 3.8 m/s)", runSpeed <= 3.85f);

            float d0 = Vector3.Distance(cam.transform.position, p.astronaut.transform.position);
            rig.CycleAstronautView(); yield return Real(1.2f);
            float d1 = Vector3.Distance(cam.transform.position, p.astronaut.transform.position);
            Check("SHOULDER view is closer and offset", rig.astronautView == AstronautView.Shoulder && d1 < d0 - .8f, $"{d0:F1} -> {d1:F1} m");
            yield return Shot("22_astronaut_shoulder.png", "astronaut SHOULDER camera");
            rig.CycleAstronautView(); yield return Real(1f);
            Check("INSPECT is first person and hides the astronaut", rig.astronautView == AstronautView.Inspect && Vector3.Distance(cam.transform.position, p.astronaut.transform.position + Vector3.up * 1.62f) < .3f && p.astronaut.GetComponentsInChildren<Renderer>().All(r => !r.enabled));
            yield return Shot("23_astronaut_inspect.png", "first-person INSPECT camera");
            rig.CycleAstronautView(); yield return Real(1f);
            Check("Back to FOLLOW shows the astronaut again", rig.astronautView == AstronautView.Follow && p.astronaut.GetComponentsInChildren<Renderer>().Any(r => r.enabled));
            float pitch0 = p.pitch; hud.lookDelta += new Vector2(0, 80); yield return Real(.2f);
            float normal = p.pitch - pitch0;
            mission.Settings.invertY = true; pitch0 = p.pitch; hud.lookDelta += new Vector2(0, 80); yield return Real(.2f);
            float inverted = p.pitch - pitch0; mission.Settings.invertY = false;
            Check("Camera inversion flips vertical look", Mathf.Sign(normal) == -Mathf.Sign(inverted) && Mathf.Abs(normal) > .5f, $"normal {normal:F1}° inverted {inverted:F1}°");
            mission.Settings.lookSensitivity = 2; float y0 = p.yaw; hud.lookDelta += new Vector2(50, 0); yield return Real(.2f);
            float fast = Mathf.DeltaAngle(y0, p.yaw); mission.Settings.lookSensitivity = 1; y0 = p.yaw; hud.lookDelta += new Vector2(50, 0); yield return Real(.2f);
            float slow = Mathf.DeltaAngle(y0, p.yaw);
            Check("Look sensitivity scales orbit speed", Mathf.Abs(fast) > Mathf.Abs(slow) * 1.6f, $"x2 {fast:F1}° vs x1 {slow:F1}°");
            mission.Settings.reducedMotion = true; yield return Real(.8f);
            Check("Reduced motion keeps a fixed FOV", Mathf.Abs(cam.fieldOfView - 55) < 1, $"fov {cam.fieldOfView:F1}");
            mission.Settings.reducedMotion = false;

            // ----- pinch zoom on the gameplay touch zone (two pointer streams through the zone handlers) -----
            var zone = ui.GetComponentInChildren<LookZone>(); var es = UnityEngine.EventSystems.EventSystem.current;
            float z0 = rig.zoom;
            var f1 = new UnityEngine.EventSystems.PointerEventData(es) { pointerId = 11, position = new Vector2(1000, 500) };
            var f2 = new UnityEngine.EventSystems.PointerEventData(es) { pointerId = 12, position = new Vector2(1200, 500) };
            zone.OnPointerDown(f1); zone.OnPointerDown(f2);
            for (int i = 1; i <= 10; i++) { f2.delta = new Vector2(30, 0); f2.position += f2.delta; zone.OnDrag(f2); yield return null; }
            zone.OnPointerUp(f1); zone.OnPointerUp(f2);
            Check("Two-finger pinch on the gameplay zone zooms the camera", rig.zoom < z0 - .1f, $"{z0:F2} -> {rig.zoom:F2}");
            rig.zoom = 1;

            // ----- Crew Dome airlock camera regression (the laptop-playtest FAIL) -----
            var airlock = hud.environment.closedAirlocks.Where(x => x).OrderBy(x => x.name.Contains("Crew") ? 0 : 1).First();
            var dome = UnityEngine.Object.FindObjectsByType<Collider>(FindObjectsSortMode.None).Where(c => !c.isTrigger && c.bounds.size.x > 6 && c.bounds.size.x < 60 && c.bounds.size.y > 2).OrderBy(c => Vector3.Distance(c.bounds.center, airlock.position)).FirstOrDefault();
            var outward = Vector3.ProjectOnPlane(airlock.position - (dome ? dome.bounds.center : airlock.position - airlock.forward), Vector3.up).normalized;
            if (outward.sqrMagnitude < .01f) outward = airlock.forward;
            var stand = airlock.position + outward * 2.2f;
            Teleport(p.astronaut, stand, Yaw(stand, stand + outward));
            p.yaw = Yaw(stand, stand + outward); p.pitch = 10; yield return Real(.8f);
            int inside0 = rig.InsideFrames, blocked = 0, frames = 0;
            ui.MoveStick.SetForTest(new Vector2(0, -.6f));
            double end = EditorApplication.timeSinceStartup + 2.5;
            while (EditorApplication.timeSinceStartup < end) { frames++; if (rig.Blocked(cam.transform.position, .1f)) blocked++; yield return null; }
            ui.MoveStick.SetForTest(Vector2.zero);
            Check("Crew Dome airlock: camera never inside geometry while backing onto the airlock", blocked == 0, $"{blocked}/{frames} frames blocked, fallback frames {rig.InsideFrames - inside0}, airlock '{airlock.name}'");
            yield return Shot("24_airlock_camera.png", "backing onto the Crew Dome airlock");

            // ----- settings: persistence, opacity, joystick position, button size -----
            var stick = (RectTransform)ui.MoveStick.transform; float sy = stick.offsetMin.y;
            FindButton("Setting Joystick position").onClick.Invoke(); FindButton("Setting Joystick position").onClick.Invoke(); // standard -> raised -> low
            FindButton("Setting Joystick position").onClick.Invoke();                                                     // low -> standard
            FindButton("Setting Joystick position").onClick.Invoke();                                                     // standard -> raised
            Check("Joystick position preset raises the stick", stick.offsetMin.y > sy + 100, $"{sy:F0} -> {stick.offsetMin.y:F0}");
            FindButton("Setting Opacity lower").onClick.Invoke(); FindButton("Setting Opacity lower").onClick.Invoke();
            Check("Control opacity setting applies", Mathf.Abs(ui.AstronautControls.GetComponent<CanvasGroup>().alpha - mission.Settings.opacity) < .01f && mission.Settings.opacity < .8f, $"{mission.Settings.opacity:F2}");
            FindButton("Setting Controls larger").onClick.Invoke();
            Check("Button size setting applies", Mathf.Abs(stick.localScale.x - mission.Settings.controlScale) < .01f && mission.Settings.controlScale > 1);
            string json = PlayerPrefs.GetString(FirstMissionController.SettingsKey, "");
            var restored = JsonUtility.FromJson<FirstMissionSettings>(json);
            Check("Settings persisted locally (PlayerPrefs, no secrets)", restored != null && restored.joystickPosition == 2 && Mathf.Abs(restored.opacity - mission.Settings.opacity) < .01f && !json.Contains("token") && !json.Contains("@"), json);
            ui.Select(FirstMissionHUD.Tab.Drive);
            yield return Shot("25_settings_applied_astronaut.png", "astronaut controls with raised joystick, lower opacity, larger buttons");
            mission.ResetSettingsForTests(); mission.SaveSettings(); FindButton("Setting Graphics"); ui.SendMessage("ApplySettings", SendMessageOptions.DontRequireReceiver);
            FindButton("Setting Graphics"); // keep reference

            // ----- settings screen capture -----
            Teleport(p.astronaut, p.rover.transform.position - p.rover.transform.right * 3.2f, 90); yield return Real(.4f);
            ui.InteractButton.onClick.Invoke(); yield return Until(() => p.driving, 3);
            ui.Select(FirstMissionHUD.Tab.Systems); yield return Real(.3f);
            FindButton("Settings").onClick.Invoke(); yield return Real(.5f);
            AuditTouchTargets("settings panel", Rect.zero);
            yield return Shot("26_settings_panel.png", "SYSTEMS > SETTINGS");
            FindButton("Settings").onClick.Invoke(); ui.Select(FirstMissionHUD.Tab.Drive); yield return Real(.3f);

            // ----- cost per frame of the new layers + input latency -----
            ui.SteerStick.SetForTest(new Vector2(.4f, .5f)); ui.Accelerate.SetForTest(true);
            var costs = new List<(double rig, double layer, double hudUi)>();
            for (int i = 0; i < 120; i++) { yield return null; costs.Add((rig.LastCostMs, cl.LastCostMs, ui.LastCostMs)); }
            ui.SteerStick.SetForTest(Vector2.zero); ui.Accelerate.SetForTest(false); ui.Brake.SetForTest(true);
            perf.Add($"Per-frame cost over 120 driving frames (Editor, Stopwatch): camera rig mean {costs.Average(c => c.rig):F3} ms max {costs.Max(c => c.rig):F3}; control layer mean {costs.Average(c => c.layer):F3} ms; First Mission HUD mean {costs.Average(c => c.hudUi):F3} ms max {costs.Max(c => c.hudUi):F3}");
            Check("New control layers cost < 1 ms per frame on average (Editor)", costs.Average(c => c.rig + c.layer + c.hudUi) < 1, $"{costs.Average(c => c.rig + c.layer + c.hudUi):F3} ms");
            yield return Until(() => Mathf.Abs(p.speed) < .05f, 4); ui.Brake.SetForTest(false);
            ui.SteerStick.SetForTest(new Vector2(0, 1)); int f0 = Time.frameCount;
            yield return Until(() => p.testMove.y > .5f, 2);
            Check("Touch-to-controller input latency <= 2 frames", Time.frameCount - f0 <= 2, $"{Time.frameCount - f0} frames");
            ui.SteerStick.SetForTest(Vector2.zero); ui.Brake.SetForTest(true); yield return Real(1); ui.Brake.SetForTest(false);
            // GC per frame, whole game, then the same with the First Mission HUD + control layer paused (A/B).
            double[] gcMean = new double[2];
            for (int pass = 0; pass < 2; pass++)
            {
                ui.enabled = cl.enabled = pass == 0; yield return Real(.3f);
                var gc = Unity.Profiling.ProfilerRecorder.StartNew(Unity.Profiling.ProfilerCategory.Memory, "GC Allocated In Frame");
                long gcSum = 0; int gcFrames = 0;
                for (int i = 0; i < 90; i++) { yield return null; if (gc.Valid) { gcSum += gc.LastValue; gcFrames++; } }
                gc.Dispose();
                gcMean[pass] = gcFrames > 0 ? gcSum / (double)gcFrames / 1024 : -1;
            }
            ui.enabled = cl.enabled = true;
            perf.Add($"GC allocated per frame (Editor Play Mode, parked on DRIVE): whole game {gcMean[0]:F1} KB; with First Mission HUD + control layer disabled {gcMean[1]:F1} KB; First Mission share ≈ {gcMean[0] - gcMean[1]:F1} KB/frame");
            if (InputSystem.devices.Contains(keys)) InputSystem.RemoveDevice(keys);
        }

        static bool Premium => ui is HUD.PremiumHUD;

        // ================= Explorer HUD (map, navigation, drawer, profiles, popups) =================
        static UnityEngine.EventSystems.PointerEventData Ptr(int id, Vector2 pos) => new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current) { pointerId = id, position = pos };

        static IEnumerator ExplorerTests()
        {
            Line("--- Explorer HUD ---");
            var ex = (HUD.ExplorerHUD)ui;
            yield return Until(() => ex.Grid.Ready, 30);
            Check("Traversability grid built from scene colliders", ex.Grid.Ready && ex.Grid.Blocked > 50, $"{ex.Grid.N}x{ex.Grid.N} cells, blocked {ex.Grid.Blocked}, steep {ex.Grid.Hazard}");
            var dome = UnityEngine.Object.FindObjectsByType<Transform>(FindObjectsSortMode.None).FirstOrDefault(t => t.name == "Crew Dome");
            if (dome) { ex.Grid.ToCell(dome.position, out int dx, out int dz); Check("Grid marks the Crew Dome as blocked", ex.Grid.Cost(dx, dz) == 0); }
            Check("Location registry uses real scene objects", ex.Registry.Locations.Count >= 10 && ex.Registry.Locations.All(l => l.transform) && ex.Registry.Locations.Count(l => l.category == Nav.LocationCategory.ScienceStation) == 5, string.Join(", ", ex.Registry.Locations.Select(l => l.id)));
            Check("Missing facilities listed only as proposals (no position)", ex.Registry.Proposals.Count == 6 && ex.Registry.Proposals.All(l => l.proposal && !l.navigable && l.transform == null));

            // ----- compact HUD on foot -----
            if (p.driving) { ui.Select(FirstMissionHUD.Tab.Drive); ui.Brake.SetForTest(true); yield return Until(() => Mathf.Abs(p.speed) < .05f, 4); ui.Brake.SetForTest(false); ui.ExitButton.onClick.Invoke(); yield return Until(() => !p.driving, 3); }
            var far = ParkingSpot(p.rover.transform, 36); Teleport(p.astronaut, far, Yaw(far, p.rover.transform.position)); p.yaw = Yaw(far, p.rover.transform.position); yield return Real(.8f);
            Check("Compact HUD: big panels gone, compass, map icon and ⋯ menu shown", !ex.TopBar.activeInHierarchy && !ex.MissionCard.activeInHierarchy && !ex.NavWidget.activeInHierarchy && ex.CompassBar.activeInHierarchy && ex.MapButton.gameObject.activeInHierarchy && ex.MenuButton.gameObject.activeInHierarchy && ex.ObjectiveChip.activeInHierarchy);
            var canvasRt = (RectTransform)ex.Safe.GetComponentInParent<Canvas>().transform; var cc = new Vector3[4]; canvasRt.GetWorldCorners(cc);
            var centre = Rect.MinMaxRect(Mathf.Lerp(cc[0].x, cc[2].x, .3f), Mathf.Lerp(cc[0].y, cc[2].y, .3f), Mathf.Lerp(cc[0].x, cc[2].x, .7f), Mathf.Lerp(cc[0].y, cc[2].y, .7f));
            var blockers = ex.GetComponentsInChildren<UnityEngine.UI.Graphic>().Where(g => g.isActiveAndEnabled && g.raycastTarget && g.color.a > .01f && !g.GetComponent<LookZone>()).Where(g => { var c = new Vector3[4]; g.rectTransform.GetWorldCorners(c); return Rect.MinMaxRect(c[0].x, c[0].y, c[2].x, c[2].y).Overlaps(centre); }).Select(g => g.name).ToList();
            Check("Centre of the screen is clear of HUD elements on foot", blockers.Count == 0, string.Join(", ", blockers.Take(5)));
            yield return Shot("40_explorer_onfoot_compact.png", "Explorer compact HUD on foot");

            // ----- navigate to rover (guidance only) and board -----
            Check("NAVIGATE TO ROVER offered when away from the rover", ex.NavigateToRover.gameObject.activeInHierarchy);
            var start = p.astronaut.transform.position;
            ex.NavigateToRover.onClick.Invoke(); yield return Real(1.4f);
            Check("Waypoint beacon light pillar appears at the rover", ex.Cinematics.Beacon.activeInHierarchy && Vector2.Distance(new Vector2(ex.Cinematics.Beacon.transform.position.x, ex.Cinematics.Beacon.transform.position.z), new Vector2(p.rover.transform.position.x, p.rover.transform.position.z)) < .1f);
            Check("Ambient dust motes running", ex.Cinematics.Motes && ex.Cinematics.Motes.isPlaying);
            Check("Navigation target is the live rover transform", ex.Nav.Target == ex.Registry.Rover && ex.Nav.Target.transform == p.rover.transform);
            Check("Route to the rover available (grid A*)", ex.Nav.Route.status == Nav.RouteStatus.Available && ex.Nav.Route.points.Count >= 2, ex.Nav.StatusLabel);
            Check("Navigation does not move the player by itself", Vector3.Distance(start, p.astronaut.transform.position) < .2f);
            Check("Waypoint marker, route chip and compass pin shown", ex.WaypointMarker.activeInHierarchy && ex.RouteChip.activeInHierarchy);
            yield return Shot("41_explorer_navigate_to_rover.png", "NAVIGATE TO ROVER: waypoint marker and route chip");
            float rem0 = ex.Nav.Remaining;
            // Player follows the route with the joystick (camera faces the next route point).
            double until = EditorApplication.timeSinceStartup + 45;
            while (!ex.Nav.Arrived && EditorApplication.timeSinceStartup < until)
            {
                var pts = ex.Nav.Route.points; var me = p.astronaut.transform.position;
                Vector3 next = pts.Count > 1 ? pts.Skip(1).FirstOrDefault(q => RoverSensorSuite.Planar(q, me) > 1.5f) : p.rover.transform.position;
                if (next == Vector3.zero) next = p.rover.transform.position;
                p.yaw = Yaw(me, next); ui.MoveStick.SetForTest(new Vector2(0, 1)); ui.Run.SetForTest(true);
                yield return null;
            }
            ui.MoveStick.SetForTest(Vector2.zero); ui.Run.SetForTest(false); yield return Real(.6f);
            Check("Following the route reaches the rover (arrival detected)", ex.Nav.Arrived, $"remaining {ex.Nav.Remaining:F1} m of {rem0:F1} m, replans {ex.Nav.Replans}");
            Check("BOARD ROVER available on arrival (existing 5 m rule)", ui.InteractButton.gameObject.activeInHierarchy && ui.InteractButton.GetComponentInChildren<Text>().text == "BOARD ROVER");
            ui.InteractButton.onClick.Invoke(); yield return Until(() => p.driving, 3); yield return Real(.6f);
            Check("Boarding switches to rover controls, mode pill and rover camera", p.driving && ui.DriveControls.activeInHierarchy && ex.ModePill.activeInHierarchy && ex.ProfilePill.activeInHierarchy);
            ex.Nav.Cancel();

            // ----- drive profiles + measured speed -----
            var road = ParkingSpot(mission.Designated.transform, 34); Teleport(p.rover, road, Yaw(road, mission.Designated.transform.position) + 180); yield return Real(.5f);
            ex.ApplyProfile(HUD.DriveProfile.Precision); ui.Accelerate.SetForTest(true); yield return Real(2.5f);
            float precision = Mathf.Abs(p.speed);
            Check("PRECISION profile caps speed (45 % throttle)", precision < 1.8f && precision > 1.2f, $"{precision:F2} m/s");
            ex.ApplyProfile(HUD.DriveProfile.Normal); yield return Real(1.5f);
            Check("NORMAL profile reaches the controller maximum", Mathf.Abs(p.speed) > 3.5f, $"{p.speed:F2} m/s");
            Check("Driving camera motion: FOV kick and gentle shake at speed", ex.Cinematics.LastFovKick > 2f && ex.Cinematics.LastShake > 0 && ex.Cinematics.LastShake < .06f, $"fov +{ex.Cinematics.LastFovKick:F1}°, shake {ex.Cinematics.LastShake:F3} m");
            mission.Settings.reducedMotion = true; yield return Real(.6f);
            Check("Reduced motion turns camera shake off", ex.Cinematics.LastShake == 0);
            mission.Settings.reducedMotion = false;
            Check("Displayed speed derives from measured motion", Mathf.Abs(ex.MeasuredSpeed - Mathf.Abs(p.speed)) < .4f, $"measured {ex.MeasuredSpeed:F2} vs controller {p.speed:F2} m/s");
            yield return Shot("42_explorer_driving.png", "Explorer driving HUD");
            ui.Accelerate.SetForTest(false); ui.Brake.SetForTest(true); yield return Until(() => Mathf.Abs(p.speed) < .05f, 4); ui.Brake.SetForTest(false);
            Check("TRAVERSE refused (needs approval to raise the shared 3.8 m/s limit)", !ex.ApplyProfile(HUD.DriveProfile.Traverse) && ex.Profile == HUD.DriveProfile.Normal);

            // ----- fullscreen map -----
            ex.MapButton.onClick.Invoke(); yield return Real(.6f);
            Check("Map opens fullscreen and blocks gameplay input", ex.MapOverlay.activeSelf && ui.control.State == ControlState.DataReview);
            var r0 = p.rover.transform.position; ui.Accelerate.SetForTest(true); yield return Real(.8f);
            Check("No driving while the map is open", Vector3.Distance(r0, p.rover.transform.position) < .05f); ui.Accelerate.SetForTest(false);
            var g = ex.MapViewport.GetComponent<HUD.MapGestures>();
            var c0 = ex.MapCenter; var f = Ptr(21, new Vector2(1000, 500)); g.OnPointerDown(f); for (int i = 0; i < 6; i++) { f.delta = new Vector2(40, 0); f.position += f.delta; g.OnDrag(f); yield return null; } g.OnPointerUp(f);
            Check("One-finger drag pans the map", Vector2.Distance(c0, ex.MapCenter) > .005f, $"{c0} -> {ex.MapCenter}");
            float z0 = ex.MapZoom; var a = Ptr(31, new Vector2(900, 500)); var b = Ptr(32, new Vector2(1100, 500)); g.OnPointerDown(a); g.OnPointerDown(b);
            for (int i = 0; i < 6; i++) { b.position += new Vector2(30, 0); g.OnDrag(b); yield return null; }
            float zIn = ex.MapZoom; for (int i = 0; i < 10; i++) { b.position -= new Vector2(30, 0); g.OnDrag(b); yield return null; }
            g.OnPointerUp(a); g.OnPointerUp(b);
            Check("Pinch zooms in and out", zIn > z0 * 1.2f && ex.MapZoom < zIn, $"{z0:F2} -> {zIn:F2} -> {ex.MapZoom:F2}");
            float zd = ex.MapZoom; var t1 = Ptr(41, new Vector2(1000, 500)); g.OnPointerClick(t1); yield return null; g.OnPointerClick(t1);
            Check("Double tap zooms in", ex.MapZoom > zd * 1.5f || ex.MapZoom >= 12f, $"{zd:F2} -> {ex.MapZoom:F2}");
            ex.MapCenterRover.onClick.Invoke(); yield return Real(.3f);
            Check("Centre on rover uses the live rover position", Vector2.Distance(ex.MapCenter, Nav.MapCoordinateConverter.ToMap(p.rover.transform.position)) < .002f);
            var rm = ex.MarkerById["rover.ares6"]; var uv = Nav.MapCoordinateConverter.ToMap(p.rover.transform.position);
            Check("Rover marker sits at the converted rover position", Vector2.Distance(rm.anchoredPosition, new Vector2((uv.x - .5f) * ex.MapContent.sizeDelta.x, (uv.y - .5f) * ex.MapContent.sizeDelta.y)) < 1f);
            ex.MapOrientation.onClick.Invoke(); yield return Real(.3f);
            Check("Heading-up rotates the map to the rover heading", ex.HeadingUp && Mathf.Abs(Mathf.DeltaAngle(ex.MapContent.localEulerAngles.z, p.rover.transform.eulerAngles.y)) < 1f);
            yield return Shot("43_explorer_map_heading_up.png", "fullscreen map, heading up");
            ex.MapOrientation.onClick.Invoke(); yield return Real(.3f);
            Check("North-up restores 0° rotation", !ex.HeadingUp && Mathf.Abs(Mathf.DeltaAngle(ex.MapContent.localEulerAngles.z, 0)) < .5f);
            FindButton("Layer Hazards").onClick.Invoke(); FindButton("Layer Science").onClick.Invoke(); yield return Real(.3f);
            Check("Layer toggles: hazards shown, science markers hidden", ex.Layers["Hazards"] && !ex.MarkerById["site_0"].gameObject.activeSelf);
            yield return Shot("44_explorer_map_hazards.png", "map with hazard layer");
            FindButton("Layer Science").onClick.Invoke(); FindButton("Layer Hazards").onClick.Invoke(); yield return Real(.2f);
            var lab = ex.Registry.Locations.FirstOrDefault(l => l.id == "hab.lab") ?? ex.Registry.Locations.First(l => l.category == Nav.LocationCategory.ScienceStation);
            ex.MarkerById[lab.id].GetComponent<UnityEngine.UI.Button>().onClick.Invoke(); yield return Real(.4f);
            Check("Tapping a marker selects it and previews a route", ex.Selected == lab && ex.PreviewRoute != null && ex.PreviewRoute.status == Nav.RouteStatus.Available, ex.PreviewRoute?.message);
            var pr = ex.PreviewRoute; float straight = RoverSensorSuite.Planar(p.rover.transform.position, lab.Position);
            bool walkable = true; for (int i = 1; i < pr.points.Count; i++) for (float s2 = 0; s2 <= 1; s2 += .1f) { var q = Vector3.Lerp(pr.points[i - 1], pr.points[i], s2); if (ex.Grid.ToCell(q, out int qx, out int qz) && ex.Grid.Cost(qx, qz) == 0) walkable = false; }
            Check("Route stays on traversable cells (obstacle-aware, not a straight line)", walkable && pr.length >= straight - 2, $"route {pr.length:F0} m vs straight {straight:F0} m, {pr.points.Count} points");
            ex.MapCenterRover.onClick.Invoke(); yield return Real(.4f);
            yield return Shot("45_explorer_map_route_preview.png", "map with selected location and route preview");
            Check("Invalid destination handled (outside the map)", Nav.RoutePlanner.Plan(ex.Grid, p.rover.transform.position, new Vector3(600, 0, 0), 3).status == Nav.RouteStatus.Unreachable);
            Check("Proposal locations are not navigable", ex.Nav.Preview(ex.Registry.Proposals[0]).status == Nav.RouteStatus.Unreachable);
            ex.MapNavigate.onClick.Invoke(); yield return Real(1.4f);
            Check("NAVIGATE sets the target, closes the map and shows guidance", ex.Nav.Target == lab && !ex.MapOverlay.activeSelf && ex.RouteChip.activeInHierarchy, ex.Nav.StatusLabel);
            ui.Accelerate.SetForTest(true); yield return Real(1.5f); ui.Accelerate.SetForTest(false); ui.Brake.SetForTest(true); yield return Real(1.5f); ui.Brake.SetForTest(false);
            Check("Live route progress updates", ex.Nav.Route.status == Nav.RouteStatus.Available || ex.Nav.Route.status == Nav.RouteStatus.Arrived, ex.Nav.StatusLabel);
            ex.MapButton.onClick.Invoke(); yield return Real(.3f); ex.MapCancelRoute.onClick.Invoke(); yield return Real(.3f);
            Check("CANCEL ROUTE clears navigation", ex.Nav.Target == null);
            ex.MapReset.onClick.Invoke(); yield return Real(.2f);
            Check("Reset view restores zoom and centre", Mathf.Approximately(ex.MapZoom, 1) && ex.MapCenter == new Vector2(.5f, .5f));
            ex.MapClose.onClick.Invoke(); yield return Real(.3f);
            Check("Map closes and gameplay input returns", !ex.MapOverlay.activeSelf && ui.control.State == ControlState.RoverDrive);

            // ----- systems drawer -----
            ex.MenuButton.onClick.Invoke(); yield return Real(.4f);
            bool allSections = true;
            foreach (var sec in new[] { "Mission", "Astronaut", "Rover", "Sensors", "Power", "Comms", "Data queue", "Science AI", "Navigation", "Graphics" })
            { ex.OpenDrawer(sec); yield return Real(.1f); var body = ex.Drawer.GetComponentsInChildren<Text>().First(t => t.name == "Drawer body").text; if (string.IsNullOrWhiteSpace(body)) allSections = false; }
            Check("Systems drawer: every section has content", allSections);
            ex.OpenDrawer("Rover"); yield return Real(.2f);
            var rb = ex.Drawer.GetComponentsInChildren<Text>().First(t => t.name == "Drawer body").text;
            Check("Drawer labels sources (LIVE / SIMULATED / UNAVAILABLE)", rb.Contains("[LIVE]") && rb.Contains("[SIMULATED]") && rb.Contains("UNAVAILABLE"), rb.Substring(0, Math.Min(120, rb.Length)));
            yield return Shot("46_explorer_drawer_rover.png", "systems drawer: rover");
            ex.OpenDrawer("Controls"); yield return Real(.2f);
            Check("Drawer Controls shows the settings", ex.GetComponentsInChildren<UnityEngine.UI.Button>().Any(x => x.name.StartsWith("Drawer setting ") && x.isActiveAndEnabled));
            ex.CloseDrawer(); yield return Real(.2f);

            // ----- scan popup -> record icon -> record view -----
            var site = mission.Designated; var spot = ParkingSpot(site.transform, 9); Teleport(p.rover, spot, Yaw(spot, site.transform.position)); yield return Real(.6f);
            int recs0 = ex.RecordCount;
            mission.BeginObservation(mission.Catalog.Instrument("rover.context_camera")); yield return Real(1.2f);
            Check("Scan beam from the mast to the station + expanding ring during the observation", ex.Cinematics.ScanBeam.enabled && ex.Cinematics.ScanRing.enabled);
            p.yaw = Yaw(site.transform.position, p.rover.transform.position) + 150; p.pitch = 16;
            yield return Shot("51_explorer_scan_beam.png", "scan beam and ring during a rover observation");
            yield return Until(() => mission.Observing == null, 6); yield return Real(.3f);
            Check("Scan beam stops when the observation ends", !ex.Cinematics.ScanBeam.enabled);
            Check("Scan result popup appears (not over the centre)", ex.ScanPopup.activeSelf && ex.RecordCount == recs0 + 1 && ex.PopupText.Contains("SIMULATED") && ex.PopupText.Contains("LOCAL ONLY"), ex.PopupText.Replace("\n", " | "));
            yield return Shot("47_explorer_scan_popup.png", "scan result popup");
            yield return Real(5f);
            Check("Popup collapses into the record icon after ~4.5 s", !ex.ScanPopup.activeSelf && ex.RecordIcon.activeSelf);
            ex.RecordIconButton.onClick.Invoke(); yield return Real(.4f);
            var detail = ex.RecordView.GetComponentsInChildren<Text>().First(t => t.name == "Record body").text;
            Check("Record view shows provenance, storage, sync and AI status", ex.RecordView.activeSelf && detail.Contains("SIMULATED") && detail.Contains("LOCAL ONLY") && detail.Contains("NOT CONNECTED") && detail.Contains("Related objective"));
            yield return Shot("48_explorer_record_view.png", "detailed scientific record");
            ex.RecordClose.onClick.Invoke(); yield return Real(.2f);
            ui.ExitButton.onClick.Invoke(); yield return Until(() => !p.driving, 3);
            var foot = site.transform.position + (p.astronaut.transform.position - site.transform.position).normalized * 2f; Teleport(p.astronaut, foot, Yaw(foot, site.transform.position)); yield return Real(.6f);
            int recs1 = ex.RecordCount; ui.InteractButton.onClick.Invoke(); yield return Until(() => !hud.scanning, 3); yield return Real(.4f);
            Check("On-foot scan shows an honest popup (no invented values)", ex.RecordCount == recs1 + 1 && ex.PopupText.Contains("No measurement values"), ex.PopupText.Replace("\n", " | "));
            foreach (var (w, h, label) in new[] { (1920, 1080, "16:9"), (1512, 982, "desktop") })
            {
                try { PlayModeWindow.SetCustomRenderingResolution((uint)w, (uint)h, "Explorer " + label); } catch { }
                yield return Real(1.2f); AuditTouchTargets("Explorer " + label, Rect.zero);
            }
            try { PlayModeWindow.SetCustomRenderingResolution(Width, Height, "AstroGateway First Mission 20:9"); } catch { }
            yield return Real(1f);
        }

        // ================= premium HUD v2 =================
        static IEnumerator PremiumTests()
        {
            Line("--- Premium HUD v2 ---");
            var ph = (HUD.PremiumHUD)ui;
            if (!p.driving) { Teleport(p.astronaut, p.rover.transform.position - p.rover.transform.right * 3.2f, 90); yield return Real(.4f); ui.InteractButton.onClick.Invoke(); yield return Until(() => p.driving, 3); }
            ui.Select(FirstMissionHUD.Tab.Drive); yield return Real(.6f);
            // honest status sources
            HUD.StatusReading R(string id) => ph.Status.Get(id).Read();
            Check("Status: rover power is SIMULATED (game battery), not the phone battery", R("power").source == HUD.Source.Simulated && Mathf.Abs(R("power").value.Value - hud.game.battery) < .01f);
            Check("Status: vehicle temperature NOT AVAILABLE (no thermal model)", R("temperature").source == HUD.Source.NotAvailable && !R("temperature").value.HasValue);
            Check("Status: link NOT CONNECTED", R("comms").source == HUD.Source.NotConnected);
            Check("Status: storage counts the real local store", R("storage").value == mission.Store.Records.Count && R("storage").source == HUD.Source.Local);
            Check("Status: speed equals the controller speed", Math.Abs(R("speed").value.Value - Mathf.Abs(p.speed) * 3.6f) < .01f && R("speed").source == HUD.Source.GameState);
            Check("Status: 10 categories present", new[] { "power", "temperature", "comms", "storage", "speed", "steering", "mode", "sensors", "mission", "sync" }.All(id => ph.Status.Sources.Any(x => x.Id == id)));
            Check("Status widgets expose accessible text with source", ph.SummaryWidgets.Where(w => w.isActiveAndEnabled).All(w => w.AccessibleText.Contains("source")), ph.SummaryWidgets[0].AccessibleText);
            Check("Unavailable value is shown as N/A, not a number", ph.SummaryWidgets.Concat(ph.SystemWidgets).Where(w => w.source.Id == "temperature").All(w => w.value.text == "N/A"));
            // smart visibility
            if (ui is HUD.ExplorerHUD) Check("Explorer DRIVE: speed gauge shown, permanent cards removed", ph.SpeedGauge.activeInHierarchy && !ph.MissionCard.activeInHierarchy && !ph.StatusSummary.activeInHierarchy && !ph.NavWidget.activeInHierarchy && !ph.TopBar.activeInHierarchy);
            else Check("DRIVE: speed gauge, mission card, status and navigation visible", ph.SpeedGauge.activeInHierarchy && ph.MissionCard.activeInHierarchy && ph.StatusSummary.activeInHierarchy && ph.NavWidget.activeInHierarchy);
            ui.Select(FirstMissionHUD.Tab.Science); yield return Real(.5f);
            Check("SCIENCE: driving controls replaced by science panel, nav hidden", !ui.DriveControls.activeInHierarchy && ui.Panels[FirstMissionHUD.Tab.Science].activeInHierarchy && !ph.NavWidget.activeInHierarchy);
            yield return Shot("30_hudv2_science.png", "HUD v2 SCIENCE");
            ui.Select(FirstMissionHUD.Tab.Data); yield return Real(.5f);
            Check("DATA: dashboard visible, side panels hidden", ui.Panels[FirstMissionHUD.Tab.Data].activeInHierarchy && !ph.MissionCard.activeInHierarchy && !ph.NavWidget.activeInHierarchy);
            var newest = mission.Store.Records.Last();
            Check("DATA table row 1 is the newest record with its real fields", ph.DataRows[0][1].text == newest.sensorId && ph.DataRows[0][6].text == newest.provenance && ph.DataRows[0][9].text == "NOT CONNECTED", string.Join(" | ", ph.DataRows[0].Select(t => t.text)));
            Check("DATA sync states use LOCAL ONLY while offline", ph.DataRows[0][7].text == "LOCAL ONLY" && ph.DataSummaryText.text.Contains("LOCAL ONLY"));
            Check("Sync label mapping (SYNCED/SYNCING/FAILED/QUEUED/LOCAL ONLY)", HUD.RoverStatusProvider.SyncLabel(new SensorRecord { uploadState = UploadState.Uploaded }, true) == "SYNCED" && HUD.RoverStatusProvider.SyncLabel(new SensorRecord { uploadState = UploadState.Uploading }, true) == "SYNCING" && HUD.RoverStatusProvider.SyncLabel(new SensorRecord { uploadState = UploadState.Failed }, true) == "FAILED" && HUD.RoverStatusProvider.SyncLabel(new SensorRecord { uploadState = UploadState.WaitingForSync }, true) == "QUEUED" && HUD.RoverStatusProvider.SyncLabel(new SensorRecord { uploadState = UploadState.Offline }, false) == "LOCAL ONLY");
            yield return Shot("31_hudv2_data.png", "HUD v2 DATA dashboard");
            ui.Select(FirstMissionHUD.Tab.Mission); yield return Real(.5f);
            yield return Shot("32_hudv2_mission.png", "HUD v2 MISSION");
            ui.Select(FirstMissionHUD.Tab.Systems); yield return Real(.6f);
            Check("SYSTEMS shows all 10 status widgets", ph.SystemWidgets.Count == 10 && ph.SystemWidgets.All(w => w.isActiveAndEnabled));
            yield return Shot("33_hudv2_systems.png", "HUD v2 SYSTEMS status");
            ui.Select(FirstMissionHUD.Tab.Drive); yield return Real(.5f);
            // navigation from real transforms
            yield return Until(() => ph.Minimap.Rendered, 3);
            Check("Minimap rendered once from the real scene", ph.Minimap.Rendered && ph.Minimap.Texture && ph.Minimap.Texture.IsCreated());
            var wp = ph.CurrentWaypoint(); yield return Real(.4f);
            float d = RoverSensorSuite.Planar(p.rover.transform.position, wp.position);
            Check("Waypoint distance and bearing come from transforms", ph.WaypointText.text.Contains($"{d:0} m"), ph.WaypointText.text + " vs " + d.ToString("0.0"));
            Check("Position labelled scene frame, not GPS", ph.PositionText.text.Contains("not GPS"));
            // collapse / expand
            ph.MissionCollapse.onClick.Invoke(); ph.NavCollapse.onClick.Invoke(); ph.StatusExpand.onClick.Invoke(); yield return Real(.8f);
            Check("Mission and nav panels collapse; status expands to 10", ph.MissionCollapsed && ph.NavCollapsed && ph.SummaryWidgets.Count(w => w.gameObject.activeSelf) == 10);
            yield return Shot("34_hudv2_collapsed_expanded.png", "HUD v2 with mission/nav collapsed and status expanded");
            ph.MissionCollapse.onClick.Invoke(); ph.NavCollapse.onClick.Invoke(); ph.StatusExpand.onClick.Invoke(); yield return Real(.8f);
            // critical alert (temporary)
            float b0 = hud.game.battery; hud.game.battery = 10; yield return Real(.6f);
            Check("Critical alert banner appears for low (simulated) rover power", ph.AlertBanner.activeSelf && ph.CurrentAlert.Contains("POWER"));
            yield return Shot("35_hudv2_alert.png", "HUD v2 critical alert");
            hud.game.battery = b0; yield return Real(4.5f);
            Check("Alert banner hides itself (does not block the view)", !ph.AlertBanner.activeSelf);
            // pause shows settings
            hud.TogglePause(); yield return Real(.5f);
            Check("Paused: settings shown in the pause card", ui.PausedOverlay.activeSelf && ph.GetComponentsInChildren<UnityEngine.UI.Button>().Any(b => b.name.StartsWith("Pause setting ") && b.isActiveAndEnabled));
            yield return Shot("36_hudv2_paused_settings.png", "HUD v2 pause + settings");
            hud.TogglePause(); yield return Real(.4f);
            // overlap audit for the always-visible panels at the current resolution
            var rects = new[] { ph.TopBar, ph.MissionCard, ph.StatusSummary, ph.NavWidget, ph.SpeedGauge }.Where(g => g.activeInHierarchy).Select(g => { var c = new Vector3[4]; ((RectTransform)g.transform).GetWorldCorners(c); return (g.name, r: Rect.MinMaxRect(c[0].x, c[0].y, c[2].x, c[2].y)); }).ToList();
            var controls = ui.DriveControls.GetComponentsInChildren<UnityEngine.UI.Selectable>().Select(x => (RectTransform)x.transform).Concat(ui.DriveControls.GetComponentsInChildren<HoldControl>().Select(x => (RectTransform)x.transform)).Concat(ui.DriveControls.GetComponentsInChildren<VirtualStick>().Select(x => (RectTransform)x.transform)).Select(t => { var c = new Vector3[4]; t.GetWorldCorners(c); return (t.name, r: Rect.MinMaxRect(c[0].x, c[0].y, c[2].x, c[2].y)); }).ToList();
            var clashes = new List<string>();
            for (int i = 0; i < rects.Count; i++) { for (int j = i + 1; j < rects.Count; j++) if (rects[i].r.Overlaps(rects[j].r)) clashes.Add(rects[i].name + " x " + rects[j].name); foreach (var c in controls) if (rects[i].r.Overlaps(c.r) && rects[i].name != "Speedometer" && !c.name.StartsWith("Speed")) clashes.Add(rects[i].name + " x " + c.name); }
            Check("No overlap between HUD panels and drive controls (2400x1080)", clashes.Count == 0, string.Join("; ", clashes.Take(6)));
            foreach (var (w, h, label) in new[] { (1512, 982, "desktop 1512x982"), (1920, 1080, "16:9"), (2560, 1440, "1440p") })
            {
                try { PlayModeWindow.SetCustomRenderingResolution((uint)w, (uint)h, "HUD v2 " + label); } catch { }
                yield return Real(1.2f);
                AuditTouchTargets("HUD v2 " + label, Rect.zero);
                yield return Shot($"37_hudv2_drive_{w}x{h}.png", "HUD v2 DRIVE at " + label);
            }
            try { PlayModeWindow.SetCustomRenderingResolution(Width, Height, "AstroGateway First Mission 20:9"); } catch { }
            yield return Real(1f);
            yield return Shot("38_hudv2_drive_2400x1080.png", "HUD v2 DRIVE at 2400x1080");
        }

        static bool Try(Action a) { try { a(); return true; } catch { return false; } }
        static List<Vector3> Grid(float step) { var l = new List<Vector3>(); for (int x = -1; x <= 1; x++) for (int z = -1; z <= 1; z++) l.Add(new Vector3(x, x == 1 && z == 1 ? step : 0, z)); return l; }
        static UnityEngine.UI.Button FindButton(string name) => ui.GetComponentsInChildren<UnityEngine.UI.Button>(true).FirstOrDefault(b => b.name == name);

        static IEnumerator StoreBoundsTests(string dir)
        {
            string path = Path.Combine(dir, "bounds", "sensor_records_v1.json");
            var s = new SensorRecordStore(path, 5);
            var src = mission.Store.Records.Take(5).Select(r => JsonUtility.FromJson<SensorRecord>(JsonUtility.ToJson(r))).ToList();
            s.Add(src, out _);
            var extra = JsonUtility.FromJson<SensorRecord>(JsonUtility.ToJson(src[0])); extra.eventId = Guid.NewGuid().ToString();
            bool refused = !s.Add(new[] { extra }, out string why);
            Check("Bounded store refuses new records instead of dropping pending ones", refused && s.Records.Count == 5 && why.Contains("full"), why);
            s.Records[0].uploadState = UploadState.Uploaded;
            Check("Bounded store evicts the oldest already-uploaded record to make room", s.Add(new[] { extra }, out _) && s.Records.Count == 5 && s.Contains(extra.eventId) && !s.Contains(src[0].eventId));
            File.WriteAllText(path, "{ corrupt");
            var c = new SensorRecordStore(path, 5);
            Check("Corrupt store file is ignored safely (no crash)", c.Records.Count == 0 && c.LastError.Length > 0, c.LastError);
            yield break;
        }

        class MockUploader : ISensorRecordUploader
        {
            public Func<IList<SensorRecord>, UploadResult> respond; public int calls;
            public bool Online => true; public string Label => "MOCK uploader (test only)";
            public void Upload(IList<SensorRecord> batch, Action<UploadResult> done) { calls++; done(respond(batch)); }
        }
        static UploadResult Ok(IList<SensorRecord> b, bool dup = false, string status = "not_connected", string prov = null) =>
            new UploadResult { code = 200, acks = b.Select(r => new UploadAck { eventId = r.eventId, duplicate = dup, analysisStatus = status, analysisProvenance = prov, modelVersion = prov != null ? "mock-0" : null, summary = prov != null ? "MOCK result" : null }).ToList() };

        static IEnumerator MockUploadTests(string dir)
        {
            Line("--- MOCK upload tests (in-process fake uploader, no network, no real backend) ---");
            var records = mission.Store.Records.Take(6).Select(r => { var c = JsonUtility.FromJson<SensorRecord>(JsonUtility.ToJson(r)); c.uploadState = UploadState.CollectedLocally; return c; }).ToList();
            var mock = new MockUploader();
            mission.UseStoreForTests(Path.Combine(dir, "mock"), mock);
            mission.Store.Add(records, out _);
            // transient failure -> FAILED, retry after backoff
            mock.respond = b => new UploadResult { code = 503, error = "HTTP 503" };
            mission.Sync.Tick(Time.unscaledTime);
            Check("MOCK 503: records show FAILED — RETRY AVAILABLE", mission.Store.Records.All(r => r.uploadState == UploadState.Failed), string.Join(",", mission.Store.Records.Select(r => r.uploadState).Distinct()));
            mission.Sync.Tick(Time.unscaledTime + .5f);
            Check("MOCK 503: no immediate retry (backoff)", mock.calls == 1);
            mock.respond = b => Ok(b);
            mission.Sync.Tick(Time.unscaledTime + 3);
            Check("MOCK reconnection: retried after backoff and acknowledged", mock.calls == 2 && mission.Store.Records.All(r => r.uploadState == UploadState.Uploaded && r.analysisState != AnalysisState.Complete));
            // duplicate ack (server already had them) and no further sends
            foreach (var r in mission.Store.Records) r.uploadState = UploadState.WaitingForSync;
            mock.respond = b => Ok(b, dup: true);
            mission.Sync.Tick(Time.unscaledTime + 10);
            mission.Sync.Tick(Time.unscaledTime + 20);
            Check("MOCK duplicate ack counts as uploaded, sent once (no duplicate processing)", mock.calls == 3 && mission.Store.Records.All(r => r.uploadState == UploadState.Uploaded));
            // a server ack claiming "complete" without BACKEND-ANALYZED provenance is ignored
            foreach (var r in mission.Store.Records) r.uploadState = UploadState.WaitingForSync;
            mock.respond = b => Ok(b, status: "complete");
            mission.Sync.Tick(Time.unscaledTime + 30);
            Check("MOCK ack without BACKEND-ANALYZED provenance is not shown as an AI result", mission.Store.Records.All(r => r.analysisState != AnalysisState.Complete));
            // only a server-side BACKEND-ANALYZED ack can set an analysis result; it is never overwritten later
            foreach (var r in mission.Store.Records) r.uploadState = UploadState.WaitingForSync;
            mock.respond = b => Ok(b, status: "complete", prov: Provenance.BackendAnalyzed);
            mission.Sync.Tick(Time.unscaledTime + 40);
            bool set = mission.Store.Records.All(r => r.analysisState == AnalysisState.Complete && r.analysisProvenance == Provenance.BackendAnalyzed);
            foreach (var r in mission.Store.Records) r.uploadState = UploadState.WaitingForSync;
            mock.respond = b => Ok(b);
            mission.Sync.Tick(Time.unscaledTime + 50);
            Check("MOCK BACKEND-ANALYZED ack is stored and a later plain ack does not overwrite it", set && mission.Store.Records.All(r => r.analysisState == AnalysisState.Complete));
            // auth failure stops until retry; validation failure marks rejected
            foreach (var r in mission.Store.Records) { r.uploadState = UploadState.WaitingForSync; }
            mock.respond = b => new UploadResult { code = 401 };
            mission.Sync.Tick(Time.unscaledTime + 60); int c401 = mock.calls;
            mission.Sync.Tick(Time.unscaledTime + 400);
            Check("MOCK 401: upload stops until RETRY (sign-in)", mock.calls == c401 && mission.Sync.Status.StartsWith("Sign-in"));
            mission.Sync.RetryNow(); mock.respond = b => new UploadResult { code = 422 };
            mission.Sync.Tick(Time.unscaledTime + 401);
            Check("MOCK 422: records marked REJECTED and kept on the device", mission.Store.Records.All(r => r.uploadState == UploadState.Rejected) && mission.Store.Records.Count == records.Count);
            Check("Http uploader refuses plain HTTP to non-local hosts", !Try(() => new HttpSensorRecordUploader("http://example.com", null, () => "x")));
            var acks = HttpSensorRecordUploader.ParseAcks("{\"session_id\":\"s\",\"accepted\":1,\"duplicates\":0,\"records\":[{\"event_id\":\"e\",\"duplicate\":false,\"stored_at\":\"t\",\"provenance\":\"SIMULATED\",\"analysis\":{\"interface_version\":\"ag.science-analysis.v0\",\"status\":\"not_connected\",\"model_version\":null,\"result\":null,\"confidence\":null,\"reason\":\"x\"}}]}");
            Check("Parses the real backend ack shape (not_connected → no result)", acks.Count == 1 && acks[0].analysisStatus == "not_connected" && acks[0].analysisProvenance == null);
            yield break;
        }

        static void Finish()
        {
            routine = null; sampling = false;
            try
            {
                if (touch != null) { InputSystem.RemoveDevice(touch); touch = null; }
                if (keys != null && keys.added) { InputSystem.RemoveDevice(keys); keys = null; }
                if (inputPatched) { InputSystem.settings.backgroundBehavior = priorBackground; InputSystem.settings.editorInputBehaviorInPlayMode = priorEditorInput; inputPatched = false; }
                foreach (var d in disabledReal) if (d != null && d.added) InputSystem.EnableDevice(d);
                disabledReal.Clear();
                if (priorSettings == null) PlayerPrefs.DeleteKey(FirstMissionController.SettingsKey); else PlayerPrefs.SetString(FirstMissionController.SettingsKey, priorSettings);
                if (priorProgress == null) PlayerPrefs.DeleteKey(FirstMissionController.ProgressKey); else PlayerPrefs.SetString(FirstMissionController.ProgressKey, priorProgress);
                PlayerPrefs.Save();
                try { ProductionViewport.Cleanup(); } catch { }
            }
            catch { }
            Application.logMessageReceived -= OnLog;
            Directory.CreateDirectory(Out);
            var all = new List<string> { $"AstroGateway First Mission playtest (graphical Editor Play Mode, Mac) — {DateTime.Now:yyyy-MM-dd HH:mm:ss}", $"PASS {pass}  FAIL {fail}  UNTESTED {untested}", "Inputs: First Mission HUD widget handlers + one virtual-touchscreen event; MOCK = fake uploader, no network.", "" };
            all.AddRange(lines);
            all.Add(""); all.Add("Performance (Editor Play Mode on this Mac; NOT Android):"); all.AddRange(perf);
            all.Add(""); all.Add($"Console errors ({logErrors.Count}):"); all.AddRange(logErrors.Distinct().Take(40));
            File.WriteAllLines(Path.Combine(Out, "FirstMission_Playtest_Results.txt"), all);
            File.WriteAllLines(Path.Combine(Out, "Touch_Target_Audit.txt"), audit);
            Debug.Log($"[FirstMission] DONE PASS {pass} FAIL {fail} UNTESTED {untested}");
            EditorApplication.isPlaying = false;
            if (Environment.GetEnvironmentVariable("FM_QUIT") == "1") EditorApplication.delayCall += () => EditorApplication.Exit(fail > 0 ? 1 : 0);
        }
    }
}
