using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using AstroGateway.CinematicHUD;

// Android-first landscape HUD for Mars_FirstMission_v1 (uGUI, runtime-built like the existing HUD; no new UI
// framework). Replaces the older HUD's presentation in this scene only: that HUD keeps running (its state, scans,
// pause and settings logic are reused) but its canvas is not drawn here. Inputs go into the same
// AstroGatewayHUDController fields the original touch controls write, so movement, boarding and exit rules are
// the existing ones.
//  - Astronaut mode: move joystick, RUN, BOARD (near the rover), on-foot SCAN, camera drag.
//  - Rover mode: tabs DRIVE / SCIENCE / DATA / MISSION / SYSTEMS. DRIVE shows the driving controls; the other
//    tabs show a translucent panel and release the throttle (the rover coasts to a stop with its normal physics).
// Look: dark translucent panels with a thin cyan edge (no real-time blur), cyan highlights, orange warnings,
// large touch targets (>= 112 reference px at 1080 p ≈ 7 mm on a 6.5" phone), safe-area aware.
namespace AstroGateway.FirstMission
{
    [DefaultExecutionOrder(720)]
    public class FirstMissionHUD : MonoBehaviour
    {
        public FirstMissionController mission;
        public enum Tab { Drive, Science, Data, Mission, Systems }
        public Tab Current { get; private set; } = Tab.Drive;
        public bool simulateSafeArea;          // tests: emulate a display cutout / rounded corners
        public Rect simulatedSafeArea;         // normalised 0..1 of the screen (independent of Editor window size)

        // exposed for tests
        public RectTransform Safe { get; private set; }
        public GameObject AstronautControls { get; private set; }
        public GameObject DriveControls { get; private set; }
        public GameObject TabBar { get; private set; }
        public GameObject PausedOverlay { get; private set; }
        public readonly Dictionary<Tab, GameObject> Panels = new Dictionary<Tab, GameObject>();
        public readonly Dictionary<Tab, Button> TabButtons = new Dictionary<Tab, Button>();
        public Button ExitButton, InteractButton, ObserveButton, RetryButton, PauseButton, TargetButton, CancelButton, ScienceShortcut, SettingsButton;
        public Button BoardButton => InteractButton;   // context button: BOARD / SCAN / INSPECT
        public FirstMissionCameraRig cameraRig;
        public FirstMissionControlLayer control;
        public bool SettingsOpen { get; private set; }
        public float Throttle { get; private set; }
        public bool RunLatched { get; private set; }
        public string Toast { get; private set; } = "";
        public HoldControl Accelerate, Brake, Run;
        public Toggle ReverseToggle;
        public VirtualStick MoveStick, SteerStick;
        public string SelectedInstrument { get; private set; } = "rover.terrain_profiler";

        static readonly Color Panel = new Color(.02f, .05f, .08f, .72f), Edge = new Color(.30f, .86f, .96f, .55f), Cyan = new Color(.36f, .88f, .98f),
            Orange = new Color(1f, .58f, .18f), TextMain = new Color(.92f, .95f, .97f), TextDim = new Color(.62f, .71f, .77f), Ok = new Color(.45f, .92f, .62f);
        Font font;
        Sprite rounded, circle;
        AstroGatewayHUDController hud;
        Canvas oldCanvas;
        Text modeChip, objectiveStrip, speedReadout, headingReadout, scienceText, dataText, missionText, systemsText, feedbackText, pausedText, footScanText;
        Image observeFill, missionFill;
        CanvasGroup panelGroup;
        RectTransform scienceList;
        readonly Dictionary<string, Button> instrumentButtons = new Dictionary<string, Button>();
        readonly Dictionary<string, Text> settingLabels = new Dictionary<string, Text>();
        GameObject settingsGrid; Text toastText; float toastUntil; bool runWasHeld; string interactKind = "";
        CanvasGroup astronautGroup, driveGroup;
        bool lastDriving, built;
        float nextRefresh, panelFade = 1;

        void Start()
        {
            hud = mission.hud;
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); // ships with every player (Avenir is macOS-only)
            rounded = hud.theme ? hud.theme.rounded : null; circle = hud.theme ? hud.theme.circle : null;
            if (hud.uiRoot) oldCanvas = hud.uiRoot.GetComponent<Canvas>();
            Build();
            built = true;
            lastDriving = !hud.game.prototype.driving; // force a mode refresh on the first frame
        }

        // ---------------- build ----------------
        void Build()
        {
            var root = new GameObject("First Mission HUD Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            root.transform.SetParent(transform, false);
            var canvas = root.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 20;
            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1920, 1080); scaler.matchWidthOrHeight = 1;
            Safe = R("Safe area", root.transform, Vector2.zero, Vector2.one);

            // Camera drag zone sits underneath every control.
            var look = Img("Camera drag zone", Safe, new Vector2(.22f, .18f), new Vector2(.80f, .86f), null, new Color(0, 0, 0, 0), true);
            var lz = look.gameObject.AddComponent<LookZone>(); lz.hud = hud; lz.ui = this;

            // Top bar
            var chip = Box("Mode chip", Safe, new Vector2(0, 1), new Vector2(0, 1), new Vector2(16, -112), new Vector2(436, -16));
            modeChip = T("Mode", chip, "", 30, TextAnchor.MiddleLeft, Cyan, new Vector2(.06f, .52f), new Vector2(.97f, .97f), FontStyle.Bold);
            objectiveStrip = T("Objective", chip, "", 21, TextAnchor.MiddleLeft, TextMain, new Vector2(.06f, .04f), new Vector2(.97f, .52f));
            PauseButton = Btn("Pause", Safe, "II", new Vector2(1, 1), new Vector2(1, 1), new Vector2(-128, -112), new Vector2(-16, -16), () => hud.TogglePause(), 34);

            // Tab bar (rover only)
            TabBar = R("Tab bar", Safe, new Vector2(.5f, 1), new Vector2(.5f, 1)).gameObject;
            var tb = (RectTransform)TabBar.transform; tb.offsetMin = new Vector2(-460, -112); tb.offsetMax = new Vector2(460, -16);
            string[] names = { "DRIVE", "SCIENCE", "DATA", "MISSION", "SYSTEMS" };
            for (int i = 0; i < 5; i++)
            {
                var tab = (Tab)i;
                var b = Btn("Tab " + names[i], tb, names[i], new Vector2(i / 5f, 0), new Vector2((i + 1) / 5f, 1), new Vector2(4, 0), new Vector2(-4, 0), () => Select(tab), 25);
                var line = Img("Selected underline", b.transform, new Vector2(.1f, 0), new Vector2(.9f, .06f), null, Cyan);
                line.raycastTarget = false;
                TabButtons[tab] = b;
            }

            var toast = Box("Action feedback", Safe, new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(-330, -186), new Vector2(330, -124));
            toastText = T("Toast", toast, "", 24, TextAnchor.MiddleCenter, Cyan, Vector2.zero, Vector2.one, FontStyle.Bold);
            toast.gameObject.SetActive(false);

            BuildAstronaut();
            BuildDrive();
            BuildPanels();

            // On-foot scan progress (the existing scan rules: 3 m, 0.8 s, on foot only)
            var fs = Box("On-foot scan progress", Safe, new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(-300, 40), new Vector2(300, 120));
            footScanText = T("Scan text", fs, "", 24, TextAnchor.MiddleCenter, Cyan, Vector2.zero, Vector2.one);
            fs.gameObject.SetActive(false);

            // Pause overlay mirrors the original pause modal (whose canvas is hidden here).
            var shade = Img("Paused overlay", Safe, Vector2.zero, Vector2.one, null, new Color(0, 0, 0, .6f), true);
            PausedOverlay = shade.gameObject;
            var card = Box("Paused card", shade.transform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(-420, -220), new Vector2(420, 220));
            pausedText = T("Paused text", card, "", 26, TextAnchor.UpperCenter, TextMain, new Vector2(.06f, .38f), new Vector2(.94f, .94f));
            Btn("Resume", card, "RESUME", new Vector2(.5f, 0), new Vector2(.5f, 0), new Vector2(-180, 28), new Vector2(180, 148), () => { if (hud.modalOpen) hud.CloseModal(); else if (hud.environment.paused) hud.TogglePause(); }, 30);
            PausedOverlay.SetActive(false);
            ApplySettings();
        }

        void BuildAstronaut()
        {
            AstronautControls = R("Astronaut controls", Safe, Vector2.zero, Vector2.one).gameObject;
            var t = AstronautControls.transform;
            MoveStick = Stick("Move joystick", t, "MOVE");
            Run = Hold("Run", t, "RUN", new Vector2(1, 0), new Vector2(-260, 40), new Vector2(-40, 200));
            // One context-sensitive button: BOARD ROVER near the rover, SCAN at a research station, INSPECT at an airlock.
            InteractButton = Btn("Interact", t, "INTERACT", new Vector2(1, 0), new Vector2(1, 0), new Vector2(-560, 40), new Vector2(-280, 200), ContextInteract, 26);
            Btn("Astronaut camera", t, "CAMERA", new Vector2(1, 1), new Vector2(1, 1), new Vector2(-400, -240), new Vector2(-220, -128), () => { cameraRig.CycleAstronautView(); Say("Camera: " + cameraRig.ViewLabel); }, 24);
            Btn("Astronaut recenter", t, "RECENTER", new Vector2(1, 1), new Vector2(1, 1), new Vector2(-204, -240), new Vector2(-16, -128), () => { cameraRig.Recenter(); Say("Camera recentred"); }, 22);
        }

        void BuildDrive()
        {
            DriveControls = R("Drive controls", Safe, Vector2.zero, Vector2.one).gameObject;
            var t = DriveControls.transform;
            SteerStick = Stick("Steering joystick", t, "STEER / THROTTLE");
            Accelerate = Hold("Accelerate", t, "ACCEL", new Vector2(1, 0), new Vector2(-260, 40), new Vector2(-40, 300));
            Brake = Hold("Brake", t, "BRAKE", new Vector2(1, 0), new Vector2(-500, 40), new Vector2(-280, 200));
            Brake.GetComponent<Image>().color = new Color(.30f, .12f, .04f, .78f);
            Brake.GetComponentInChildren<Text>().color = Orange;
            ReverseToggle = ToggleBox("Reverse", t, "REVERSE", new Vector2(1, 0), new Vector2(-500, 220), new Vector2(-280, 320));
            ExitButton = Btn("Exit rover", t, "EXIT ROVER", new Vector2(1, 1), new Vector2(1, 1), new Vector2(-400, -240), new Vector2(-16, -128), () => hud.Interact(), 27);
            Btn("Camera distance", t, "CAMERA", new Vector2(1, 1), new Vector2(1, 1), new Vector2(-400, -372), new Vector2(-220, -256), () => { cameraRig.CycleRoverView(); Say("Camera: " + cameraRig.ViewLabel); }, 24);
            Btn("Overview", t, "VIEW", new Vector2(1, 1), new Vector2(1, 1), new Vector2(-204, -372), new Vector2(-16, -256), () => { if (mission.modes) mission.modes.ToggleCinematic(); }, 26);
            Btn("Rover recenter", t, "RECENTER", new Vector2(1, 1), new Vector2(1, 1), new Vector2(-400, -504), new Vector2(-16, -388), () => { cameraRig.Recenter(); Say("Camera behind the rover"); }, 24);
            // Contextual science: appears when the rover is stopped within 15 m of a research station.
            ScienceShortcut = Btn("Science shortcut", t, "SCIENCE AT STATION", new Vector2(.5f, 0), new Vector2(.5f, 0), new Vector2(-230, 186), new Vector2(230, 300), () => Select(Tab.Science), 24);
            var gauge = Box("Speed and orientation", t, new Vector2(.5f, 0), new Vector2(.5f, 0), new Vector2(-300, 24), new Vector2(300, 168));
            speedReadout = T("Speed", gauge, "", 40, TextAnchor.MiddleCenter, TextMain, new Vector2(.03f, .48f), new Vector2(.97f, .98f), FontStyle.Bold);
            headingReadout = T("Heading", gauge, "", 22, TextAnchor.MiddleCenter, Cyan, new Vector2(.03f, .04f), new Vector2(.97f, .48f));
        }

        void BuildPanels()
        {
            var host = Box("Tab panel", Safe, new Vector2(1, 0), new Vector2(1, 1), new Vector2(-900, 24), new Vector2(-16, -128));
            panelGroup = host.gameObject.AddComponent<CanvasGroup>();
            foreach (Tab tab in Enum.GetValues(typeof(Tab))) if (tab != Tab.Drive) Panels[tab] = R(tab + " panel", host, Vector2.zero, Vector2.one).gameObject;

            // SCIENCE
            var sci = Panels[Tab.Science].transform;
            T("Title", sci, "SCIENCE • rover instruments", 26, TextAnchor.UpperLeft, Cyan, new Vector2(.04f, .92f), new Vector2(.96f, .985f), FontStyle.Bold);
            scienceList = R("Instrument list", sci, new Vector2(.03f, .12f), new Vector2(.40f, .91f));
            int n = mission.Catalog.Instruments.Count, i = 0;
            foreach (var inst in mission.Catalog.Instruments)
            {
                var id = inst.id;
                float top = 1 - i / (float)n, bottom = 1 - (i + 1) / (float)n;
                var b = Btn("Instrument " + id, scienceList, ShortName(inst), new Vector2(0, bottom), new Vector2(1, top), new Vector2(0, 4), new Vector2(0, -4), () => SelectedInstrument = id, 21);
                instrumentButtons[id] = b; i++;
            }
            scienceText = T("Instrument detail", sci, "", 21, TextAnchor.UpperLeft, TextMain, new Vector2(.44f, .54f), new Vector2(.97f, .91f));
            TargetButton = Btn("Select target", sci, "TARGET", new Vector2(.44f, .40f), new Vector2(.70f, .52f), Vector2.zero, Vector2.zero, () =>
            {
                var t = mission.sensors.CycleTarget(25);
                Say(t ? "Target: " + t.displayName : "No research station within 25 m");
            }, 22);
            CancelButton = Btn("Cancel observation", sci, "CANCEL", new Vector2(.72f, .40f), new Vector2(.97f, .52f), Vector2.zero, Vector2.zero, () => { if (mission.Observing != null) mission.Cancel("Observation cancelled by the player"); }, 22);
            ObserveButton = Btn("Observe", sci, "RUN OBSERVATION", new Vector2(.44f, .25f), new Vector2(.97f, .38f), Vector2.zero, Vector2.zero, () => mission.BeginObservation(mission.Catalog.Instrument(SelectedInstrument)), 28);
            var track = Img("Observation progress", sci, new Vector2(.44f, .215f), new Vector2(.97f, .235f), rounded, new Color(1, 1, 1, .12f));
            observeFill = Img("Fill", track.transform, Vector2.zero, new Vector2(0, 1), rounded, Cyan);
            feedbackText = T("Feedback", sci, "", 20, TextAnchor.UpperLeft, Ok, new Vector2(.44f, .01f), new Vector2(.97f, .20f));

            // DATA
            var data = Panels[Tab.Data].transform;
            T("Title", data, "DATA • collected records", 26, TextAnchor.UpperLeft, Cyan, new Vector2(.04f, .92f), new Vector2(.7f, .985f), FontStyle.Bold);
            RetryButton = Btn("Retry sync", data, "RETRY SYNC", new Vector2(.70f, .89f), new Vector2(.97f, .99f), Vector2.zero, Vector2.zero, () => mission.Sync.RetryNow(), 21);
            dataText = T("Data body", data, "", 19, TextAnchor.UpperLeft, TextMain, new Vector2(.04f, .02f), new Vector2(.97f, .9f));

            // MISSION
            var mis = Panels[Tab.Mission].transform;
            T("Title", mis, "MISSION • Red Horizon surface operations", 26, TextAnchor.UpperLeft, Cyan, new Vector2(.04f, .92f), new Vector2(.97f, .985f), FontStyle.Bold);
            var mtrack = Img("Mission progress", mis, new Vector2(.04f, .885f), new Vector2(.96f, .905f), rounded, new Color(1, 1, 1, .12f));
            missionFill = Img("Fill", mtrack.transform, Vector2.zero, new Vector2(0, 1), rounded, Cyan);
            missionText = T("Mission body", mis, "", 20, TextAnchor.UpperLeft, TextMain, new Vector2(.04f, .02f), new Vector2(.97f, .87f));

            // SYSTEMS
            var sys = Panels[Tab.Systems].transform;
            T("Title", sys, "SYSTEMS • rover and link status", 26, TextAnchor.UpperLeft, Cyan, new Vector2(.04f, .92f), new Vector2(.97f, .985f), FontStyle.Bold);
            SettingsButton = Btn("Settings", sys, "SETTINGS", new Vector2(.70f, .89f), new Vector2(.97f, .99f), Vector2.zero, Vector2.zero, () => { SettingsOpen = !SettingsOpen; Refresh(); }, 21);
            systemsText = T("Systems body", sys, "", 20, TextAnchor.UpperLeft, TextMain, new Vector2(.04f, .02f), new Vector2(.97f, .9f));
            settingsGrid = R("Settings grid", sys, new Vector2(.03f, .02f), new Vector2(.97f, .875f)).gameObject;
            var s = mission.Settings;
            (string id, Action act)[] items =
            {
                ("Handedness", () => s.leftHanded = !s.leftHanded), ("Joystick position", () => s.joystickPosition = (s.joystickPosition + 1) % 3),
                ("Controls smaller", () => s.controlScale = Mathf.Clamp(s.controlScale - .1f, .85f, 1.25f)), ("Controls larger", () => s.controlScale = Mathf.Clamp(s.controlScale + .1f, .85f, 1.25f)),
                ("Opacity lower", () => s.opacity = Mathf.Clamp(s.opacity - .1f, .4f, 1f)), ("Opacity higher", () => s.opacity = Mathf.Clamp(s.opacity + .1f, .4f, 1f)),
                ("Look slower", () => s.lookSensitivity = Mathf.Clamp(s.lookSensitivity - .1f, .3f, 2.5f)), ("Look faster", () => s.lookSensitivity = Mathf.Clamp(s.lookSensitivity + .1f, .3f, 2.5f)),
                ("Invert camera", () => s.invertY = !s.invertY), ("Reduced motion", () => s.reducedMotion = !s.reducedMotion),
                ("Auto-sprint", () => s.autoSprint = !s.autoSprint), ("Run mode", () => s.runToggle = !s.runToggle),
                ("Shoulder side", () => s.shoulderSide = -s.shoulderSide), ("Graphics", () => hud.environment.ApplyQuality((hud.environment.quality + 1) % 4)),
            };
            for (int k = 0; k < items.Length; k++)
            {
                int row = k / 2, col = k % 2; var item = items[k];
                float top = 1 - row / 7f, bottom = 1 - (row + 1) / 7f;
                var b = Btn("Setting " + item.id, settingsGrid.transform, item.id, new Vector2(col * .5f, bottom), new Vector2(col * .5f + .5f, top), new Vector2(4, 4), new Vector2(-4, -4), () => { item.act(); mission.SaveSettings(); ApplySettings(); Refresh(); }, 20);
                settingLabels[item.id] = b.GetComponentInChildren<Text>();
            }
        }

        public void Say(string text) { Toast = text; toastUntil = Time.unscaledTime + 1.6f; if (toastText) { toastText.text = text; toastText.transform.parent.gameObject.SetActive(true); } }

        void ContextInteract()
        {
            if (interactKind == "scan") hud.BeginScan(); else hud.Interact();
            if (!string.IsNullOrEmpty(hud.lastResult)) Say(hud.lastResult);
        }

        static string ShortName(InstrumentDef i) => (i.available ? "" : "(N/A) ") + i.name.Replace(" (hazard cameras)", "");

        // ---------------- runtime ----------------
        public void Select(Tab tab)
        {
            if (!hud.game.prototype.driving && tab != Tab.Drive) return; // tabs exist in rover mode only
            if (tab == Current) return;
            Current = tab;
            panelFade = 0;
            ReleaseDriveInput();
            Refresh();
        }

        void ReleaseDriveInput()
        {
            if (SteerStick) SteerStick.Clear();
            if (Accelerate) Accelerate.Clear();
            if (Brake) Brake.Clear();
            hud.movement = Vector2.zero; hud.runHeld = false; hud.brakeHeld = false;
        }

        public double LastCostMs { get; private set; }
        readonly System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        void Update() { watch.Restart(); Tick(); LastCostMs = watch.Elapsed.TotalMilliseconds; }

        void Tick()
        {
            if (!built) return;
            // Safe area (display cutouts / rounded corners) every frame it changes.
            var size = new Vector2(Screen.width, Screen.height);
            var area = simulateSafeArea ? new Rect(simulatedSafeArea.x * size.x, simulatedSafeArea.y * size.y, simulatedSafeArea.width * size.x, simulatedSafeArea.height * size.y) : Screen.safeArea;
            if (size.x > 0 && size.y > 0)
            {
                Safe.anchorMin = new Vector2(area.xMin / size.x, area.yMin / size.y);
                Safe.anchorMax = new Vector2(area.xMax / size.x, area.yMax / size.y);
                Safe.offsetMin = Safe.offsetMax = Vector2.zero;
            }

            bool driving = hud.game.prototype.driving;
            if (driving != lastDriving)
            {
                lastDriving = driving;
                Current = Tab.Drive; panelFade = 0;
                MoveStick.Clear(); Run.Clear(); ReleaseDriveInput();
                if (ReverseToggle) ReverseToggle.isOn = false;
            }
            bool paused = hud.environment.paused || hud.modalOpen;
            PausedOverlay.SetActive(paused);
            AstronautControls.SetActive(!driving && !paused);
            TabBar.SetActive(driving && !paused);
            DriveControls.SetActive(driving && Current == Tab.Drive && !paused);
            panelGroup.gameObject.SetActive(driving && Current != Tab.Drive && !paused);
            foreach (var kv in Panels) kv.Value.SetActive(kv.Key == Current);
            panelFade = Mathf.MoveTowards(panelFade, 1, Time.unscaledDeltaTime / .15f);
            panelGroup.alpha = Mathf.SmoothStep(0, 1, panelFade);
            foreach (var kv in TabButtons) kv.Value.transform.Find("Selected underline").gameObject.SetActive(kv.Key == Current);

            // Feed the existing controller's input fields (read by HUDInputRouter next frame).
            if (!paused)
            {
                if (driving && Current == Tab.Drive)
                {
                    // Progressive throttle: ACCEL ramps 0 -> 1 in ~0.7 s and releases in ~0.25 s (the rover's own
                    // speed ramp still applies on top). REVERSE flips the throttle direction.
                    Throttle = Mathf.MoveTowards(Throttle, Accelerate.Held ? 1 : 0, Time.unscaledDeltaTime / (Accelerate.Held ? .7f : .25f));
                    var v = SteerStick.Value;
                    if (Throttle > .01f) v.y = ReverseToggle.isOn ? -Throttle : Mathf.Max(v.y, Throttle);
                    hud.movement = new Vector2(Mathf.Clamp(v.x, -1, 1), Mathf.Clamp(v.y, -1, 1));
                    hud.runHeld = false;
                    hud.brakeHeld = Brake.Held;
                }
                else if (!driving)
                {
                    Throttle = 0;
                    bool held = Run.Held;
                    if (mission.Settings.runToggle && held && !runWasHeld) { RunLatched = !RunLatched; Say(RunLatched ? "Run ON" : "Run OFF"); }
                    runWasHeld = held;
                    hud.movement = MoveStick.Value; hud.runHeld = mission.Settings.runToggle ? RunLatched : held; hud.brakeHeld = false;
                }
                else { Throttle = 0; hud.movement = Vector2.zero; hud.runHeld = false; hud.brakeHeld = false; }
            }

            // Old HUD canvas is not drawn in this scene (its logic keeps running).
            if (oldCanvas && oldCanvas.enabled) oldCanvas.enabled = false;

            var fs = footScanText.transform.parent.gameObject;
            fs.SetActive(hud.scanning);
            if (hud.scanning) footScanText.text = "SCANNING • on-foot simulated scan";

            if (toastText && Time.unscaledTime > toastUntil && toastText.transform.parent.gameObject.activeSelf) toastText.transform.parent.gameObject.SetActive(false);
            if (Time.unscaledTime >= nextRefresh) { nextRefresh = Time.unscaledTime + .2f; Refresh(); }
            observeFill.rectTransform.anchorMax = new Vector2(mission.ObservationProgress, 1);
        }

        void Refresh()
        {
            var g = hud.game; var p = g.prototype;
            bool driving = p.driving;
            modeChip.text = driving ? "ROVER • ARES-VI" : "ASTRONAUT • EVA";
            objectiveStrip.text = mission.Instructions();
            bool nearRover = Vector3.Distance(p.ActivePosition(), p.rover.transform.position) <= 5;
            var footTarget = driving ? null : hud.CurrentTarget();
            interactKind = driving ? "" : nearRover ? "board" : footTarget ? "scan" : NearAirlock(p.ActivePosition()) ? "inspect" : "";
            InteractButton.gameObject.SetActive(interactKind != "");
            InteractButton.GetComponentInChildren<Text>().text = interactKind == "board" ? "BOARD ROVER" : interactKind == "scan" ? "SCAN\n" + footTarget.displayName : "INSPECT AIRLOCK";
            if (ScienceShortcut) ScienceShortcut.gameObject.SetActive(driving && Mathf.Abs(p.speed) < .2f && mission.sensors.NearestStation(out float nd) && nd <= 15);
            ExitButton.interactable = driving && Mathf.Abs(p.speed) <= .2f;
            ExitButton.GetComponentInChildren<Text>().text = Mathf.Abs(p.speed) > .2f ? "STOP TO EXIT" : "EXIT ROVER";

            float kmh = Mathf.Abs(p.speed) * 3.6f;
            speedReadout.text = $"{kmh:0.0} km/h  {(p.speed < -.05f ? "REV" : p.speed > .05f ? "FWD" : "STOP")}";
            mission.sensors.Terrain(out float slope, out float rough);
            float hdg = mission.sensors.Heading;
            headingReadout.text = $"HDG {hdg:000}° {Compass(hdg)}  •  ground slope {slope:0}°  •  {(ReverseToggle && ReverseToggle.isOn ? "REVERSE SELECTED" : "DRIVE")}";
            headingReadout.color = slope >= mission.Catalog.HazardSlope ? Orange : Cyan;

            if (!driving) return;
            switch (Current)
            {
                case Tab.Science: RefreshScience(); break;
                case Tab.Data: RefreshData(); break;
                case Tab.Mission: RefreshMission(); break;
                case Tab.Systems: RefreshSystems(); break;
            }
        }

        static string Compass(float h) => new[] { "N", "NE", "E", "SE", "S", "SW", "W", "NW" }[Mathf.RoundToInt(h / 45f) % 8];

        void RefreshScience()
        {
            var inst = mission.Catalog.Instrument(SelectedInstrument);
            foreach (var kv in instrumentButtons)
            {
                var img = kv.Value.GetComponent<Image>();
                var def = mission.Catalog.Instrument(kv.Key);
                img.color = kv.Key == SelectedInstrument ? new Color(.10f, .32f, .40f, .92f) : new Color(.12f, .16f, .20f, .78f);
                kv.Value.GetComponentInChildren<Text>().color = def.available ? TextMain : TextDim;
            }
            var station = mission.sensors.Target(out float range);
            string reason = mission.Check(inst);
            CancelButton.interactable = mission.Observing != null;
            var b = new StringBuilder();
            b.AppendLine(inst.name.ToUpperInvariant());
            b.AppendLine(inst.available ? "Sensor status: AVAILABLE" : "Sensor status: UNAVAILABLE • " + inst.unavailableReason);
            b.AppendLine(inst.maxRange > 0 ? $"Scan range: max {inst.maxRange:0} m from a station" : "Scan range: at the rover");
            if (inst.requiresStopped) b.AppendLine("Requires the rover to be stopped");
            b.AppendLine(station ? $"{(mission.sensors.selected ? "Selected" : "Nearest")} target: {station.displayName} ({station.stableId}) {range:0.0} m" : "No station registered");
            if (inst.maxRange > 0 && station) b.AppendLine(range <= inst.maxRange ? "RANGE: IN RANGE" : $"RANGE: OUT OF RANGE ({range - inst.maxRange:0.0} m too far)");
            b.AppendLine("Measures: " + string.Join(", ", inst.sensors.Where(s => s != "nav.position").Select(s => mission.Catalog.Sensors[s].implemented ? s : s + " (unavailable)")));
            scienceText.text = b.ToString();
            ObserveButton.interactable = reason == null;
            ObserveButton.GetComponentInChildren<Text>().text = mission.Observing != null ? "OBSERVING " + Mathf.RoundToInt(mission.ObservationProgress * 100) + "%" : reason == null ? "RUN OBSERVATION" : "NOT AVAILABLE";
            var sample = mission.LastObservation.FirstOrDefault(r => r.sensorId == "sci.sample_id" && r.provenance == Provenance.Simulated);
            feedbackText.text = (reason != null && mission.Observing == null ? reason + "\n" : "") + mission.LastFeedback + (sample != null ? "\nSample ID " + sample.valueText.Substring(0, 8) + " • " + sample.stationId + " (game-assigned)" : "");
            feedbackText.color = (reason != null && mission.Observing == null) || mission.LastFeedback.StartsWith("UNAVAILABLE") || mission.LastFeedback.StartsWith("Observation cancelled") || mission.LastFeedback.StartsWith("Not stored") ? Orange : Ok;
        }

        void RefreshData()
        {
            var s = mission.Store; var recs = s.Records;
            int uploaded = recs.Count(r => r.uploadState == UploadState.Uploaded), failed = recs.Count(r => r.uploadState == UploadState.Failed || r.uploadState == UploadState.Rejected);
            var b = new StringBuilder();
            b.AppendLine($"{recs.Count} records on device • {s.Pending} pending upload • {uploaded} uploaded • {failed} failed/rejected");
            b.AppendLine("Sync: " + mission.Sync.Status);
            b.AppendLine(mission.Analysis.StatusLabel);
            var last = recs.LastOrDefault(r => r.provenance == Provenance.Simulated && r.sensorId != "nav.position") ?? recs.LastOrDefault();
            if (last == null) b.AppendLine("\nNo records yet. Open SCIENCE and run an observation.");
            else
            {
                b.AppendLine("\nLATEST READING");
                b.AppendLine($"{last.sensorId} = {last.ValueLabel}");
                b.AppendLine($"Type {last.sensorType} • unit {last.unit} • source {last.provenance} ({last.method})");
                b.AppendLine($"Collected {Local(last.capturedAtUtc)} • sim {last.simTimeS:0.0} s • at ({last.location.x:0.0}, {last.location.z:0.0}) m{(string.IsNullOrEmpty(last.stationId) ? "" : " • " + last.stationId)}");
                b.AppendLine($"Upload: {last.uploadState}{(string.IsNullOrEmpty(last.lastError) ? "" : " (" + last.lastError + ")")}");
                b.AppendLine($"Backend processing: {last.analysisState}");
                b.AppendLine(last.analysisState == AnalysisState.Complete
                    ? $"AI result: {last.analysisSummary} • model {last.analysisModelVersion}" + (last.hasConfidence ? $" • confidence {last.confidence:0.00}" : " • confidence not provided")
                    : "AI result: none • model version: none • explanation: no scientific model is connected");
            }
            b.AppendLine("\nRECENT");
            foreach (var r in recs.Reverse().Take(7))
                b.AppendLine($"{Local(r.capturedAtUtc)}  {r.sensorId,-22} {Clip(r.ValueLabel, 18),-18} {r.provenance,-11} {r.uploadState}");
            dataText.text = b.ToString();
            RetryButton.interactable = recs.Any(r => r.uploadState == UploadState.Failed) || mission.Sync.Status.StartsWith("Sign-in");
        }

        static string Clip(string s, int n) => s.Length <= n ? s : s.Substring(0, n - 1) + "…";
        static string Local(string iso) => DateTime.TryParse(iso, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var t) ? t.ToLocalTime().ToString("HH:mm:ss") : "--:--:--";

        void RefreshMission()
        {
            var objectives = mission.Objectives();
            var real = objectives.Where(o => !o.info).ToList();
            missionFill.rectTransform.anchorMax = new Vector2(real.Count(o => o.done) / (float)real.Count, 1);
            var b = new StringBuilder();
            b.AppendLine($"Progress {real.Count(o => o.done)}/{real.Count} objectives • local guidance, not scored");
            foreach (var o in objectives) b.AppendLine((o.info ? "  •  " : o.done ? "DONE  " : "TO DO  ") + o.text);
            int observations = mission.Store.Records.Select(r => r.observationId).Distinct().Count();
            b.AppendLine($"\nCompleted observations: {observations} • on-foot scans: {hud.game.scans.Count}");
            b.AppendLine("RESEARCH LOCATIONS");
            var rover = hud.game.prototype.rover.transform.position;
            foreach (var st in mission.sensors.stations.Where(x => x))
                b.AppendLine($"{(st == mission.Designated ? "> " : "   ")}{st.displayName} ({st.stableId})  {RoverSensorSuite.Planar(rover, st.transform.position):0} m  brg {RoverSensorSuite.Bearing(rover, st.transform.position):000}°");
            b.AppendLine("\nDiscoveries: none confirmed. Interpreting observations needs the backend science model (not connected).");
            b.AppendLine("Instructions: " + mission.Instructions());
            missionText.text = b.ToString();
        }

        void RefreshSystems()
        {
            var g = hud.game; var b = new StringBuilder();
            b.AppendLine($"Rover condition: {g.health:0}% (simulated)    Power: {g.battery:0.0}% (simulated)");
            b.AppendLine($"Suit oxygen: {g.prototype.oxygen:0.0}% (simulated, no consequence rules)");
            b.AppendLine("Communications: " + mission.Sync.Uploader.Label);
            b.AppendLine(mission.Analysis.StatusLabel + " • interface " + mission.Analysis.InterfaceVersion);
            b.AppendLine("State: " + (mission.Sync.Uploader.Online ? "ONLINE" : "LOCAL / OFFLINE") + $" • storage {mission.Store.Records.Count}/{mission.Store.MaxRecords} records{(string.IsNullOrEmpty(mission.Store.LastError) ? "" : " • " + mission.Store.LastError)}");
            b.AppendLine("SENSOR AVAILABILITY");
            foreach (var i in mission.Catalog.Instruments)
                b.AppendLine($"{(i.available ? "AVAILABLE    " : "UNAVAILABLE  ")}{i.name}" + (i.sensors.Any(s => !mission.Catalog.Sensors[s].implemented) && i.available ? " (some channels not modelled)" : ""));
            b.AppendLine($"Graphics preset {hud.environment.quality} • controls ×{mission.Settings.controlScale:0.00}");
            b.AppendLine($"Camera {cameraRig.ViewLabel} • zoom ×{cameraRig.zoom:0.00} • control state {(control ? control.State.ToString() : "-")}");
            systemsText.text = b.ToString();
            systemsText.gameObject.SetActive(!SettingsOpen);
            settingsGrid.SetActive(SettingsOpen);
            SettingsButton.GetComponentInChildren<Text>().text = SettingsOpen ? "STATUS" : "SETTINGS";
            var st = mission.Settings;
            string[] pos = { "LOW", "STANDARD", "RAISED" };
            settingLabels["Handedness"].text = st.leftHanded ? "LAYOUT: LEFT-HANDED" : "LAYOUT: RIGHT-HANDED";
            settingLabels["Joystick position"].text = "JOYSTICK: " + pos[st.joystickPosition];
            settingLabels["Controls smaller"].text = $"BUTTON SIZE - ({st.controlScale:0.00})";
            settingLabels["Controls larger"].text = $"BUTTON SIZE + ({st.controlScale:0.00})";
            settingLabels["Opacity lower"].text = $"OPACITY - ({st.opacity * 100:0}%)";
            settingLabels["Opacity higher"].text = $"OPACITY + ({st.opacity * 100:0}%)";
            settingLabels["Look slower"].text = $"LOOK SPEED - ({st.lookSensitivity:0.0})";
            settingLabels["Look faster"].text = $"LOOK SPEED + ({st.lookSensitivity:0.0})";
            settingLabels["Invert camera"].text = "INVERT CAMERA: " + (st.invertY ? "ON" : "OFF");
            settingLabels["Reduced motion"].text = "REDUCED MOTION: " + (st.reducedMotion ? "ON" : "OFF");
            settingLabels["Auto-sprint"].text = "AUTO-SPRINT: " + (st.autoSprint ? "ON" : "OFF");
            settingLabels["Run mode"].text = "RUN BUTTON: " + (st.runToggle ? "TOGGLE" : "HOLD");
            settingLabels["Shoulder side"].text = "SHOULDER CAM: " + (st.shoulderSide > 0 ? "RIGHT" : "LEFT");
            settingLabels["Graphics"].text = "GRAPHICS PRESET " + hud.environment.quality;
        }

        bool NearAirlock(Vector3 pos)
        {
            foreach (var a in hud.environment.closedAirlocks) if (a && Vector3.Distance(pos, a.position) < 4) return true;
            return false;
        }

        void ApplySettings()
        {
            float s = mission.Settings.controlScale; bool left = mission.Settings.leftHanded;
            foreach (var root in new[] { AstronautControls, DriveControls })
                foreach (RectTransform c in root.transform)
                {
                    c.localScale = Vector3.one * s;
                    // Mirror horizontally for left-handed play: sticks go right, buttons go left.
                    var baseMin = c.GetComponent<LayoutMemo>() ?? c.gameObject.AddComponent<LayoutMemo>();
                    baseMin.Remember(c);
                    baseMin.Apply(c, left);
                    // Joystick height preset (low / standard / raised).
                    if (c.GetComponent<VirtualStick>()) { float dy = new[] { -24f, 0f, 150f }[mission.Settings.joystickPosition]; c.offsetMin += new Vector2(0, dy); c.offsetMax += new Vector2(0, dy); }
                }
            if (!astronautGroup) astronautGroup = AstronautControls.AddComponent<CanvasGroup>();
            if (!driveGroup) driveGroup = DriveControls.AddComponent<CanvasGroup>();
            astronautGroup.alpha = driveGroup.alpha = mission.Settings.opacity;
            pausedText.text = "PAUSED\n\nSimulation clock stopped.\nSensor records stay on this device.\nAndroid back / Esc also resumes.";
        }

        // ---------------- widgets ----------------
        RectTransform R(string name, Transform parent, Vector2 a, Vector2 b)
        {
            var o = new GameObject(name, typeof(RectTransform)); var r = (RectTransform)o.transform;
            r.SetParent(parent, false); r.anchorMin = a; r.anchorMax = b; r.offsetMin = r.offsetMax = Vector2.zero; return r;
        }
        Image Img(string name, Transform parent, Vector2 a, Vector2 b, Sprite sprite, Color c, bool ray = false)
        {
            var r = R(name, parent, a, b); var im = r.gameObject.AddComponent<Image>();
            im.sprite = sprite; im.type = sprite ? Image.Type.Sliced : Image.Type.Simple; im.color = c; im.raycastTarget = ray; return im;
        }
        RectTransform Box(string name, Transform parent, Vector2 a, Vector2 b, Vector2 min, Vector2 max)
        {
            var im = Img(name, parent, a, b, rounded, Panel, true);
            var r = im.rectTransform; r.offsetMin = min; r.offsetMax = max;
            var edge = Img("Glass edge", r, new Vector2(.02f, 1), new Vector2(.98f, 1), null, Edge); edge.rectTransform.offsetMin = new Vector2(0, -2);
            return r;
        }
        Text T(string name, Transform parent, string text, int size, TextAnchor anchor, Color c, Vector2 a, Vector2 b, FontStyle style = FontStyle.Normal)
        {
            var r = R(name, parent, a, b); var t = r.gameObject.AddComponent<Text>();
            t.font = font; t.fontSize = size; t.alignment = anchor; t.color = c; t.text = text; t.fontStyle = style; t.raycastTarget = false;
            t.horizontalOverflow = HorizontalWrapMode.Wrap; t.verticalOverflow = VerticalWrapMode.Truncate;
            t.resizeTextForBestFit = true; t.resizeTextMinSize = Mathf.Min(size, 16); t.resizeTextMaxSize = size;
            return t;
        }
        Button Btn(string name, Transform parent, string label, Vector2 a, Vector2 b, Vector2 min, Vector2 max, Action click, int size)
        {
            var im = Img(name, parent, a, b, rounded, new Color(.12f, .16f, .20f, .80f), true);
            im.rectTransform.offsetMin = min; im.rectTransform.offsetMax = max;
            var btn = im.gameObject.AddComponent<Button>();
            var cb = btn.colors; cb.normalColor = Color.white; cb.highlightedColor = new Color(1.1f, 1.15f, 1.2f); cb.pressedColor = new Color(.55f, 1.25f, 1.45f);
            cb.selectedColor = Color.white; cb.disabledColor = new Color(.6f, .6f, .6f, .55f); cb.fadeDuration = .06f; btn.colors = cb;
            btn.onClick.AddListener(() => click());
            T(name + " label", im.transform, label, size, TextAnchor.MiddleCenter, TextMain, new Vector2(.04f, .05f), new Vector2(.96f, .95f), FontStyle.Bold);
            return btn;
        }
        HoldControl Hold(string name, Transform parent, string label, Vector2 anchor, Vector2 min, Vector2 max)
        {
            var im = Img(name, parent, anchor, anchor, circle ? circle : rounded, new Color(.08f, .22f, .28f, .78f), true);
            im.type = Image.Type.Simple; im.rectTransform.offsetMin = min; im.rectTransform.offsetMax = max;
            var h = im.gameObject.AddComponent<HoldControl>();
            T(name + " label", im.transform, label, 30, TextAnchor.MiddleCenter, Cyan, Vector2.zero, Vector2.one, FontStyle.Bold);
            return h;
        }
        Toggle ToggleBox(string name, Transform parent, string label, Vector2 anchor, Vector2 min, Vector2 max)
        {
            var im = Img(name, parent, anchor, anchor, rounded, new Color(.12f, .16f, .20f, .80f), true);
            im.rectTransform.offsetMin = min; im.rectTransform.offsetMax = max;
            var tg = im.gameObject.AddComponent<Toggle>(); tg.targetGraphic = im;
            var mark = Img("On", im.transform, new Vector2(0, 0), new Vector2(1, .08f), null, Orange); tg.graphic = mark;
            T(name + " label", im.transform, label, 24, TextAnchor.MiddleCenter, TextMain, Vector2.zero, Vector2.one, FontStyle.Bold);
            tg.isOn = false;
            return tg;
        }
        VirtualStick Stick(string name, Transform parent, string label)
        {
            var im = Img(name, parent, Vector2.zero, Vector2.zero, circle ? circle : rounded, new Color(.06f, .12f, .16f, .55f), true);
            im.type = Image.Type.Simple; im.rectTransform.offsetMin = new Vector2(48, 48); im.rectTransform.offsetMax = new Vector2(408, 408);
            var knob = Img("Knob", im.transform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), circle ? circle : rounded, new Color(.75f, .92f, 1f, .55f));
            knob.type = Image.Type.Simple; knob.rectTransform.sizeDelta = new Vector2(130, 130);
            var s = im.gameObject.AddComponent<VirtualStick>(); s.knob = knob.rectTransform;
            T(name + " label", im.transform, label, 18, TextAnchor.MiddleCenter, TextDim, new Vector2(.1f, .05f), new Vector2(.9f, .2f));
            return s;
        }
    }

    // ---------- small input widgets (touch and mouse via the EventSystem; multi-touch safe) ----------
    public class HoldControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        int owner = int.MinValue;
        Image image; Color baseColor;
        public bool Held => owner != int.MinValue;
        void Awake() { image = GetComponent<Image>(); }
        public void OnPointerDown(PointerEventData e) { if (owner == int.MinValue) { owner = e.pointerId; Pressed(true); } }
        public void OnPointerUp(PointerEventData e) { if (e.pointerId == owner) Clear(); }
        void Pressed(bool on)
        {
            if (!image) return;
            if (on) { baseColor = image.color; image.color = new Color(Mathf.Min(1, baseColor.r * 2.2f + .1f), Mathf.Min(1, baseColor.g * 2.2f + .1f), Mathf.Min(1, baseColor.b * 2.2f + .1f), .95f); }
            else image.color = baseColor;
        }
        public void Clear() { if (owner != int.MinValue) Pressed(false); owner = int.MinValue; }
        public void SetForTest(bool held) { if (held && owner == int.MinValue) { owner = -99; Pressed(true); } else if (!held) Clear(); }
        void OnDisable() { Clear(); }
        void OnApplicationPause(bool p) { if (p) Clear(); }
    }

    public class VirtualStick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public RectTransform knob;
        public float deadZone = .12f;
        public Vector2 Value { get; private set; }
        int owner = int.MinValue;
        public void OnPointerDown(PointerEventData e) { if (owner != int.MinValue) return; owner = e.pointerId; Move(e); }
        public void OnDrag(PointerEventData e) { if (e.pointerId == owner) Move(e); }
        public void OnPointerUp(PointerEventData e) { if (e.pointerId == owner) Clear(); }
        void Move(PointerEventData e)
        {
            var r = (RectTransform)transform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(r, e.position, e.pressEventCamera, out var p);
            float radius = r.rect.width * .4f;
            var v = Vector2.ClampMagnitude(p / Mathf.Max(1, radius), 1);
            Value = v.magnitude < deadZone ? Vector2.zero : v;
            if (knob) knob.anchoredPosition = Value * radius;
        }
        public void Clear() { owner = int.MinValue; Value = Vector2.zero; if (knob) knob.anchoredPosition = Vector2.zero; }
        public void SetForTest(Vector2 v) { Value = Vector2.ClampMagnitude(v, 1); owner = v == Vector2.zero ? int.MinValue : -98; }
        void OnDisable() { Clear(); }
        void OnApplicationPause(bool p) { if (p) Clear(); }
    }

    // Gameplay touch zone (separate from the buttons above it): one finger orbits the camera, two fingers pinch-zoom.
    public class LookZone : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public AstroGatewayHUDController hud;
        public FirstMissionHUD ui;
        readonly Dictionary<int, Vector2> fingers = new Dictionary<int, Vector2>();
        float lastPinch;
        public int Fingers => fingers.Count;
        public void OnPointerDown(PointerEventData e) { fingers[e.pointerId] = e.position; lastPinch = PinchDistance(); }
        public void OnPointerUp(PointerEventData e) { fingers.Remove(e.pointerId); lastPinch = PinchDistance(); }
        public void OnDrag(PointerEventData e)
        {
            if (!fingers.ContainsKey(e.pointerId)) fingers[e.pointerId] = e.position - e.delta;
            fingers[e.pointerId] = e.position;
            if (fingers.Count >= 2)
            {
                float d = PinchDistance();
                if (lastPinch > 0 && ui && ui.cameraRig) ui.cameraRig.Zoom(-(d - lastPinch) / Mathf.Max(200f, Screen.height) * 1.5f);
                lastPinch = d;
                return;
            }
            if (hud && hud.InputAllowed) hud.lookDelta += e.delta * .8f;
        }
        float PinchDistance() { if (fingers.Count < 2) return 0; var v = fingers.Values.Take(2).ToArray(); return Vector2.Distance(v[0], v[1]); }
        void OnDisable() { fingers.Clear(); }
    }

    // Stores a control's right-handed anchors so left-handed mode can mirror them horizontally.
    public class LayoutMemo : MonoBehaviour
    {
        Vector2 aMin, aMax, oMin, oMax; bool saved;
        public void Remember(RectTransform r) { if (saved) return; saved = true; aMin = r.anchorMin; aMax = r.anchorMax; oMin = r.offsetMin; oMax = r.offsetMax; }
        public void Apply(RectTransform r, bool mirror)
        {
            if (!mirror) { r.anchorMin = aMin; r.anchorMax = aMax; r.offsetMin = oMin; r.offsetMax = oMax; return; }
            r.anchorMin = new Vector2(1 - aMax.x, aMin.y); r.anchorMax = new Vector2(1 - aMin.x, aMax.y);
            r.offsetMin = new Vector2(-oMax.x, oMin.y); r.offsetMax = new Vector2(-oMin.x, oMax.y);
        }
    }
}
