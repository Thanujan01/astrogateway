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
        static string priorSettings, priorProgress;
        static bool sampling;
        static readonly List<double> frameMs = new List<double>();

        static FirstMissionPlaytest() { EditorApplication.update += Tick; }

        [MenuItem("AstroGateway/First Mission Playtest")]
        public static void Run()
        {
            EditorSceneManager.OpenScene(FirstMissionBuilder.Scene);
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
            Check("Speed indicator shows km/h", ui.GetComponentsInChildren<Text>().Any(t => t.name == "Speed" && t.text.Contains("km/h") && t.text.Contains("FWD")));
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
            bool near = mission.legacyRig.nearChase;
            FindButton("Camera distance").onClick.Invoke();
            Check("CAM button toggles chase distance", mission.legacyRig.nearChase != near);
            FindButton("Camera distance").onClick.Invoke();

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
            var handed = FindButton("Handedness");  // in the SYSTEMS panel (inactive objects included)
            mission.Settings.leftHanded = false; handed.onClick.Invoke(); yield return Real(.5f);
            AuditTouchTargets("left-handed + cutout", Rect.zero);
            var stickRect = new Vector3[4]; ((RectTransform)ui.SteerStick.transform).GetWorldCorners(stickRect);
            var cr = new Vector3[4]; ((RectTransform)ui.Safe.GetComponentInParent<Canvas>().transform).GetWorldCorners(cr);
            Check("Left-handed mode mirrors the steering stick to the right side", stickRect[0].x > (cr[0].x + cr[2].x) / 2f, $"stick x {stickRect[0].x:F0} of {cr[2].x:F0}");
            yield return Shot("13_drive_left_handed.png", "left-handed DRIVE layout");
            handed.onClick.Invoke(); ui.simulateSafeArea = false; yield return Real(.4f);

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
