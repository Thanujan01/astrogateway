using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using AstroGateway.FirstMission.Nav;

// Explorer HUD (Mars_FirstMission_Explorer). Builds on the premium HUD (controls, gauges, mode panels) but removes
// the permanent clutter: the large top bar, mission card, rover status card and navigation card are replaced by
//   - a thin compass strip with waypoint pin, a small objective chip, a map icon, a ⋯ systems drawer and pause;
//   - a fullscreen circular interactive map (pan, pinch, double-tap, reset, centre on astronaut/rover,
//     north-up / heading-up, live compass, scale bar, layers, selectable markers, route preview/cancel/progress);
//   - waypoint guidance in the gameplay view (projected marker, edge arrow, distance, route progress);
//   - NAVIGATE TO ROVER on foot (guidance only, never moves the player);
//   - rover mode switch (DRIVE | SCIENCE) and drive profiles (PRECISION | NORMAL; TRAVERSE needs approval);
//   - scan result popups that collapse into a record icon and a detailed record view.
// All values carry a source label: LIVE, SIMULATED, CACHED, LOCAL ONLY, NOT CONNECTED or UNAVAILABLE.
namespace AstroGateway.FirstMission.HUD
{
    public enum DriveProfile { Precision, Normal, Traverse }

    public class ExplorerHUD : PremiumHUD
    {
        // ---------- exposed for tests ----------
        public ExplorerCinematics Cinematics { get; private set; }
        public WorldLocationRegistry Registry { get; private set; }
        public TraversabilityGrid Grid { get; private set; }
        public NavigationTargetManager Nav { get; private set; }
        public GameObject MapOverlay, Drawer, ScanPopup, RecordIcon, RecordView, CompassBar, ObjectiveChip, WaypointMarker, RouteChip, ModePill, ProfilePill;
        public Button MapButton, MenuButton, ExplorerPause, NavigateToRover, MapClose, MapCenterAstronaut, MapCenterRover, MapReset, MapOrientation, MapNavigate, MapCancelRoute, RecordIconButton, RecordClose, DrawerClose;
        public RectTransform MapViewport, MapContent;
        public float MapZoom { get; private set; } = 1;
        public Vector2 MapCenter { get; private set; } = new Vector2(.5f, .5f);   // map uv at the viewport centre
        public bool HeadingUp { get; private set; }
        public MapLocation Selected { get; private set; }
        public RouteResult PreviewRoute { get; private set; }
        public DriveProfile Profile { get; private set; } = DriveProfile.Normal;
        public float MeasuredSpeed { get; private set; }                         // m/s from rover displacement
        public readonly Dictionary<string, bool> Layers = new Dictionary<string, bool>
        { { "Terrain", true }, { "Base", true }, { "Mission", true }, { "Rover", true }, { "Science", true }, { "Collected", true }, { "Unvisited", true }, { "Hazards", false }, { "Routes", true } };
        public readonly Dictionary<string, RectTransform> MarkerById = new Dictionary<string, RectTransform>();
        public string PopupText => popupBody ? popupBody.text : "";
        public int RecordCount => records.Count;
        public string DrawerSection { get; private set; } = "Mission";
        public string LastMapAction { get; private set; } = "";

        // ---------- private ----------
        RawImage mapImage, hazardImage; RectTransform markerLayer, routeLayer, compassNeedle, scaleBar, playerMark, roverMark, edgeArrow, compassPin, mapPreviewLayer;
        Text compassHeading, objectiveText, routeChipText, waypointText, scaleText, infoTitle, infoBody, drawerBody, popupTitle, popupBody, recordBadge, recordTitle, recordBody, mapStatus;
        readonly List<(Text t, float deg)> marks = new List<(Text, float)>();
        readonly List<Image> routeSegs = new List<Image>(), previewSegs = new List<Image>();
        readonly Dictionary<string, Button> layerButtons = new Dictionary<string, Button>(), sectionButtons = new Dictionary<string, Button>(), profileButtons = new Dictionary<string, Button>(), modeButtons = new Dictionary<string, Button>();
        readonly Dictionary<string, Text> drawerSettingLabels = new Dictionary<string, Text>();
        GameObject drawerSettings, infoCard, proposalsCard;
        readonly List<RecordEntry> records = new List<RecordEntry>(); int recordIndex = -1;
        float popupUntil, mapOpenedAt; int lastScanCount = -1; List<SensorRecord> lastObservationRef;
        Vector3 lastRoverPos; bool roverInit; float nextRegistryRefresh;

        class RecordEntry { public string title, summary, detail; public DateTime time; }

        protected override void Build()
        {
            base.Build();
            var p = hud.game.prototype;
            // ---- navigation core ----
            Registry = new WorldLocationRegistry();
            Registry.Discover(p, mission.sensors.stations, id => mission.Store.Records.Any(r => r.stationId == id && r.provenance == Provenance.Simulated) || hud.game.scans.Any(sc => sc != null && StationAt(sc.location) == id));
            Grid = new TraversabilityGrid(c => c.transform.IsChildOf(p.rover.transform) || c.transform.IsChildOf(p.astronaut.transform));
            StartCoroutine(BuildGridThenHazards());
            Nav = new NavigationTargetManager(Grid, () => p.ActivePosition());
            Nav.ArrivedAt += l => Say(l.category == LocationCategory.Rover ? "Arrived at the rover • BOARD ROVER available" : "Arrived: " + l.name);

            // The nav card (and its minimap) is hidden in this HUD, so the one-time scene capture runs on an
            // always-active object instead; the fullscreen map uses its texture.
            var capImg = new GameObject("Map capture target", typeof(RectTransform), typeof(RawImage)).GetComponent<RawImage>();
            capImg.transform.SetParent(Safe, false); capImg.enabled = false; capImg.raycastTarget = false;
            var cap = Safe.gameObject.AddComponent<HudMinimap>(); cap.map = capImg; cap.rover = p.rover.transform; cap.astronaut = p.astronaut.transform;
            Minimap = cap;
            // ---- remove permanent clutter (objects kept, never shown) ----
            foreach (var go in new[] { TopBar, MissionCard, StatusSummary, NavWidget }) go.SetActive(false);
            BuildCompactTop();
            BuildRoverPills();
            BuildNavigateToRover();
            BuildWaypointHud();
            BuildScanPopup();
            BuildMapOverlay();
            BuildDrawer();
            BuildRecordView();
            ApplyProfile(DriveProfile.Normal);
            PausedOverlay.transform.SetAsLastSibling();
            // Cinematic motion layer (Explorer only) + UI slide/fade-in for overlays.
            Cinematics = gameObject.AddComponent<ExplorerCinematics>(); Cinematics.Init(this);
            Func<bool> reduced = () => mission.Settings.reducedMotion;
            void Slide(GameObject go, Vector2 from) { var sl = go.AddComponent<HudSlideIn>(); sl.from = from; sl.reduced = reduced; }
            Slide(MapOverlay.transform.Find("Map info").gameObject, new Vector2(80, 0));
            Slide(Drawer.transform.Find("Drawer panel").gameObject, new Vector2(140, 0));
            Slide(ScanPopup, new Vector2(-80, 0));
            Slide(RecordView.transform.Find("Record card").gameObject, new Vector2(-120, 0));
            Slide(MapViewport.gameObject, new Vector2(0, -30));
        }

        string StationAt(Vector3 pos) { var st = mission.sensors.stations.Where(s => s).OrderBy(s => Vector3.Distance(s.transform.position, pos)).FirstOrDefault(); return st && Vector3.Distance(st.transform.position, pos) < 4 ? st.stableId : ""; }

        System.Collections.IEnumerator BuildGridThenHazards()
        {
            yield return null; yield return null;   // let physics settle
            yield return Grid.Build();
            var tex = new Texture2D(Grid.N, Grid.N, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp, name = "Hazard layer" };
            var px = new Color32[Grid.N * Grid.N];
            for (int i = 0; i < px.Length; i++) { byte c = Grid.cost[i]; px[i] = c == 0 ? new Color32(255, 90, 95, 120) : c == 3 ? new Color32(255, 190, 69, 110) : new Color32(0, 0, 0, 0); }
            tex.SetPixels32(px); tex.Apply();
            hazardImage.texture = tex;
        }

        // ======================= compact HUD =======================
        void BuildCompactTop()
        {
            // compass strip (top centre)
            var bar = Img("Compass strip", Safe, new Vector2(.5f, 1), new Vector2(.5f, 1), HudFx.Round(30), HudFx.ControlFill); bar.type = Image.Type.Sliced; bar.raycastTarget = false;
            bar.rectTransform.sizeDelta = new Vector2(620, 54); bar.rectTransform.anchoredPosition = new Vector2(0, -44);
            Hairline(bar.transform, 30, HudFx.ControlBorder);
            CompassBar = bar.gameObject; bar.gameObject.AddComponent<RectMask2D>();
            string[] names = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
            for (int i = 0; i < 8; i++)
            {
                var t = Tx("Mark " + names[i], bar.transform, names[i], i % 2 == 0 ? 20 : 15, TextAnchor.MiddleCenter, i == 0 ? HudTokens.Orange : new Color(1, 1, 1, .7f), new Vector2(.5f, 0), new Vector2(.5f, 1), FontStyle.Bold);
                t.rectTransform.sizeDelta = new Vector2(50, 0); marks.Add((t, i * 45));
            }
            for (int i = 0; i < 24; i++) { var tick = Img("Tick", bar.transform, new Vector2(.5f, 0), new Vector2(.5f, .22f), null, new Color(1, 1, 1, .25f)); tick.rectTransform.sizeDelta = new Vector2(1, 0); tick.raycastTarget = false; marks.Add((null, i * 15)); tick.name = "Tick " + (i * 15); }
            compassPin = (RectTransform)Icon("Waypoint pin", bar.transform, "pin", new Vector2(.5f, .1f), new Vector2(.5f, .9f), HudTokens.Orange).transform; compassPin.sizeDelta = new Vector2(30, 0);
            var centre = Img("Heading caret", bar.transform, new Vector2(.5f, 0), new Vector2(.5f, .35f), null, HudTokens.Cyan); centre.rectTransform.sizeDelta = new Vector2(2, 0);
            compassHeading = Tx("Heading", Safe, "", 15, TextAnchor.MiddleCenter, new Color(1, 1, 1, .75f), new Vector2(.5f, 1), new Vector2(.5f, 1)); compassHeading.rectTransform.sizeDelta = new Vector2(200, 24); compassHeading.rectTransform.anchoredPosition = new Vector2(0, -86);

            // objective chip (top-left, one line; tap opens the Mission section)
            var chip = Btn("Objective chip", Safe, "", new Vector2(0, 1), new Vector2(0, 1), new Vector2(16, -108), new Vector2(560, -16), () => OpenDrawer("Mission"), 18);
            var ci = chip.GetComponent<Image>(); ci.sprite = HudFx.Round(30); ci.type = Image.Type.Sliced; ci.color = HudFx.ControlFill;
            Hairline(chip.transform, 30, new Color(HudTokens.Orange.r, HudTokens.Orange.g, HudTokens.Orange.b, .35f));
            var ft = Icon("Flag", chip.transform, "flag", new Vector2(0, .5f), new Vector2(0, .5f), HudTokens.Orange); ft.rectTransform.sizeDelta = new Vector2(34, 34); ft.rectTransform.anchoredPosition = new Vector2(36, 0);
            objectiveText = chip.GetComponentInChildren<Text>(); objectiveText.alignment = TextAnchor.MiddleLeft; objectiveText.color = HudTokens.Text; objectiveText.fontStyle = FontStyle.Bold;
            objectiveText.rectTransform.anchorMin = Vector2.zero; objectiveText.rectTransform.anchorMax = Vector2.one; objectiveText.rectTransform.offsetMin = new Vector2(62, 0); objectiveText.rectTransform.offsetMax = new Vector2(-16, 0);
            ObjectiveChip = chip.gameObject;

            // right: map icon, ⋯ menu, pause (44 dp circles)
            MapButton = CircleIconButton("Map", Safe, "map", new Vector2(1, 1), new Vector2(-346, -74), OpenMap);
            MenuButton = CircleIconButton("Systems menu", Safe, "dots", new Vector2(1, 1), new Vector2(-212, -74), () => OpenDrawer(DrawerSection));
            ExplorerPause = CircleIconButton("Pause", Safe, "pause", new Vector2(1, 1), new Vector2(-78, -74), () => hud.TogglePause());
            PauseButton = ExplorerPause;
        }

        void BuildRoverPills()
        {
            // DRIVE | SCIENCE (rover only); DATA / MISSION / SYSTEMS live in the ⋯ drawer.
            var pill = Img("Mode pill", Safe, new Vector2(.5f, 1), new Vector2(.5f, 1), HudFx.Round(30), HudFx.ControlFill, true); pill.type = Image.Type.Sliced;
            pill.rectTransform.sizeDelta = new Vector2(400, 100); pill.rectTransform.anchoredPosition = new Vector2(0, -204);
            Hairline(pill.transform, 30, HudFx.ControlBorder); ModePill = pill.gameObject;
            foreach (var (tab, label, i) in new[] { (Tab.Drive, "DRIVE", 0), (Tab.Science, "SCIENCE", 1) })
            {
                var b = Btn("Mode " + label, pill.transform, label, new Vector2(i * .5f, 0), new Vector2(i * .5f + .5f, 1), new Vector2(5, 5), new Vector2(-5, -5), () => Select(tab), 20);
                var im = b.GetComponent<Image>(); im.sprite = HudFx.Round(30); im.type = Image.Type.Sliced; im.color = Color.clear; modeButtons[label] = b;
            }
            // Drive profiles (rover only).
            var prof = Img("Drive profile", Safe, new Vector2(0, 1), new Vector2(0, 1), HudFx.Round(30), HudFx.ControlFill, true); prof.type = Image.Type.Sliced;
            prof.rectTransform.anchorMin = prof.rectTransform.anchorMax = new Vector2(0, 1); prof.rectTransform.offsetMin = new Vector2(16, -226); prof.rectTransform.offsetMax = new Vector2(560, -122);
            Hairline(prof.transform, 30, HudFx.ControlBorder); ProfilePill = prof.gameObject;
            foreach (var (pf, label, i) in new[] { (DriveProfile.Precision, "PRECISION", 0), (DriveProfile.Normal, "NORMAL", 1), (DriveProfile.Traverse, "TRAVERSE", 2) })
            {
                var b = Btn("Profile " + label, prof.transform, label, new Vector2(i / 3f, 0), new Vector2((i + 1) / 3f, 1), new Vector2(4, 5), new Vector2(-4, -5), () => ApplyProfile(pf), 17);
                var im = b.GetComponent<Image>(); im.sprite = HudFx.Round(30); im.type = Image.Type.Sliced; im.color = Color.clear; profileButtons[label] = b;
            }
        }

        // PRECISION: throttle capped at 45 % (1.7 m/s) and finer steering. NORMAL: unchanged controller.
        // TRAVERSE would need the shared controller's 3.8 m/s maximum raised -> not enabled without approval.
        public bool ApplyProfile(DriveProfile pf)
        {
            if (pf == DriveProfile.Traverse) { Say("TRAVERSE needs approval: top speed is fixed at 13.7 km/h in the shared rover controller"); return false; }
            Profile = pf;
            if (control) { control.throttleCap = pf == DriveProfile.Precision ? .45f : 1f; control.steerProfile = pf == DriveProfile.Precision ? .6f : 1f; }
            Say("Drive profile: " + pf.ToString().ToUpperInvariant());
            return true;
        }

        void BuildNavigateToRover()
        {
            NavigateToRover = PillButton("Navigate to rover", AstronautControls.transform, "rover", "NAVIGATE TO ROVER", new Vector2(1, 0), new Vector2(-520, 156), new Vector2(-216, 248), () =>
            {
                if (Nav.Target == Registry.Rover) { Nav.Cancel(); Say("Navigation cancelled"); }
                else { Nav.SetTarget(Registry.Rover); Say("Navigating to the ARES-VI rover"); }
            }, HudTokens.Orange);
        }

        void BuildWaypointHud()
        {
            var m = Img("Waypoint marker", Safe, new Vector2(0, 0), new Vector2(0, 0), null, Color.clear); m.raycastTarget = false; m.rectTransform.sizeDelta = new Vector2(140, 90);
            WaypointMarker = m.gameObject;
            var pin = Icon("Pin", m.transform, "pin", new Vector2(.5f, .45f), new Vector2(.5f, .45f), HudTokens.Orange); pin.rectTransform.sizeDelta = new Vector2(44, 44);
            waypointText = Tx("Distance", m.transform, "", 18, TextAnchor.MiddleCenter, HudTokens.Text, new Vector2(0, 0), new Vector2(1, .25f), FontStyle.Bold);
            edgeArrow = (RectTransform)Icon("Edge arrow", m.transform, "chevron", new Vector2(.5f, .45f), new Vector2(.5f, .45f), HudTokens.Orange).transform; edgeArrow.sizeDelta = new Vector2(40, 40);
            WaypointMarker.SetActive(false);
            var chip = Btn("Route chip", Safe, "", new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(-300, -136), new Vector2(300, -100), () => OpenMap(), 16);
            var ci = chip.GetComponent<Image>(); ci.sprite = HudFx.Round(30); ci.type = Image.Type.Sliced; ci.color = HudFx.ControlFill;
            routeChipText = chip.GetComponentInChildren<Text>(); routeChipText.color = HudTokens.Orange;
            RouteChip = chip.gameObject; RouteChip.SetActive(false);
        }

        // ======================= scan popup + records =======================
        void BuildScanPopup()
        {
            var p = Img("Scan popup", Safe, new Vector2(0, .5f), new Vector2(0, .5f), HudFx.Round(22), HudFx.ControlFill, true); p.type = Image.Type.Sliced;
            p.rectTransform.offsetMin = new Vector2(16, -60); p.rectTransform.offsetMax = new Vector2(556, 170);
            Hairline(p.transform, 22, new Color(0, 210 / 255f, 1f, .45f));
            var ic = Icon("Scan icon", p.transform, "scan2", new Vector2(0, 1), new Vector2(0, 1), HudTokens.Cyan); ic.rectTransform.sizeDelta = new Vector2(40, 40); ic.rectTransform.anchoredPosition = new Vector2(36, -34);
            popupTitle = Tx("Popup title", p.transform, "", 20, TextAnchor.MiddleLeft, HudTokens.Cyan, new Vector2(0, .78f), new Vector2(1, 1), FontStyle.Bold); popupTitle.rectTransform.offsetMin = new Vector2(66, 0);
            popupBody = Tx("Popup body", p.transform, "", 16, TextAnchor.UpperLeft, HudTokens.Text, new Vector2(.04f, .05f), new Vector2(.96f, .78f));
            ScanPopup = p.gameObject; ScanPopup.SetActive(false);
            var btn = Btn("Popup tap", p.transform, "", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, () => OpenRecords(records.Count - 1), 1);
            btn.GetComponent<Image>().color = Color.clear; btn.transform.SetAsFirstSibling();

            RecordIconButton = CircleIconButton("Record icon", Safe, "database", new Vector2(0, .5f), new Vector2(76, 120), () => OpenRecords(records.Count - 1));
            RecordIcon = RecordIconButton.gameObject;
            var badge = Img("Badge", RecordIcon.transform, new Vector2(1, 1), new Vector2(1, 1), HudFx.Circle, HudTokens.Orange); badge.rectTransform.sizeDelta = new Vector2(38, 38); badge.rectTransform.anchoredPosition = new Vector2(-14, -14); badge.raycastTarget = false;
            recordBadge = Tx("Count", badge.transform, "0", 16, TextAnchor.MiddleCenter, Color.black, Vector2.zero, Vector2.one, FontStyle.Bold);
            RecordIcon.SetActive(false);
        }

        void BuildRecordView()
        {
            var shade = Img("Record view", Safe, Vector2.zero, Vector2.one, null, new Color(HudTokens.Background.r, HudTokens.Background.g, HudTokens.Background.b, .55f), true);
            RecordView = shade.gameObject;
            var card = Glass("Record card", shade.transform, new Vector2(0, 0), new Vector2(0, 1), new Vector2(16, 24), new Vector2(900, -124), .88f);
            recordTitle = Tx("Record title", card, "", 22, TextAnchor.MiddleLeft, HudTokens.Cyan, new Vector2(.04f, .90f), new Vector2(.7f, .98f), FontStyle.Bold);
            RecordClose = Btn("Record close", card, "CLOSE", new Vector2(.74f, .89f), new Vector2(.97f, .985f), Vector2.zero, Vector2.zero, () => RecordView.SetActive(false), 18);
            recordBody = Tx("Record body", card, "", 17, TextAnchor.UpperLeft, HudTokens.Text, new Vector2(.04f, .14f), new Vector2(.96f, .88f));
            Btn("Record newer", card, "NEWER", new Vector2(.04f, .02f), new Vector2(.32f, .12f), Vector2.zero, Vector2.zero, () => OpenRecords(recordIndex + 1), 18);
            Btn("Record older", card, "OLDER", new Vector2(.35f, .02f), new Vector2(.63f, .12f), Vector2.zero, Vector2.zero, () => OpenRecords(recordIndex - 1), 18);
            RecordView.SetActive(false);
        }

        public void OpenRecords(int index)
        {
            if (records.Count == 0) { Say("No scan records yet"); return; }
            recordIndex = Mathf.Clamp(index, 0, records.Count - 1);
            var r = records[recordIndex];
            recordTitle.text = $"SCIENTIFIC RECORD {recordIndex + 1}/{records.Count}";
            recordBody.text = r.detail;
            RecordView.SetActive(true);
        }

        void ShowPopup(string title, string body, string detail)
        {
            records.Add(new RecordEntry { title = title, summary = body, detail = detail, time = DateTime.Now });
            if (records.Count > 50) records.RemoveAt(0);
            popupTitle.text = title; popupBody.text = body;
            ScanPopup.SetActive(true); RecordIcon.SetActive(false);
            popupUntil = Time.unscaledTime + 4.5f;
            recordBadge.text = records.Count.ToString();
        }

        void WatchScans()
        {
            // Rover instruments: a new observation list from the controller.
            if (mission.LastObservation != null && mission.LastObservation.Count > 0 && !ReferenceEquals(mission.LastObservation, lastObservationRef))
            {
                if (lastObservationRef != null || lastScanCount >= 0)
                {
                    var obs = mission.LastObservation; var inst = mission.Catalog.Instrument(obs[0].instrumentId);
                    bool online = mission.Sync.Uploader.Online;
                    var lines = obs.Where(r => r.sensorId != "nav.position").Take(3).Select(r => $"{r.sensorId}: {r.ValueLabel}  [{(r.provenance == Provenance.Simulated ? "SIMULATED" : "UNAVAILABLE")}]");
                    string body = string.Join("\n", lines) + $"\nSaved: LOCAL ONLY • sync {RoverStatusProvider.SyncLabel(obs[0], online)} • {LocalTime(obs[0].capturedAtUtc)}";
                    var d = new StringBuilder();
                    d.AppendLine($"Instrument: {inst.name} ({inst.id})");
                    d.AppendLine($"Station: {(string.IsNullOrEmpty(obs[0].stationId) ? "none" : obs[0].stationId)} • observation {obs[0].observationId.Substring(0, 8)}");
                    d.AppendLine($"Collected: {LocalTime(obs[0].capturedAtUtc)} (device clock) • sim time {obs[0].simTimeS:0.0} s");
                    d.AppendLine($"Location: x {obs[0].location.x:0.0} m, z {obs[0].location.z:0.0} m (scene frame, not GPS)\n");
                    foreach (var r in obs) d.AppendLine($"{r.sensorId,-24} {r.ValueLabel,-22} unit {r.unit,-9} {r.provenance} ({r.method})");
                    d.AppendLine($"\nLocal storage: saved on this device (LOCAL ONLY) • upload: {obs[0].uploadState} ({RoverStatusProvider.SyncLabel(obs[0], online)})");
                    d.AppendLine("Scientific interpretation: NOT CONNECTED (no scientific AI model is available)");
                    d.AppendLine("Related objective: " + mission.Objectives().FirstOrDefault(o => !o.done && !o.info)?.text);
                    ShowPopup(inst.name.ToUpperInvariant() + " • COMPLETE", body, d.ToString());
                }
                lastObservationRef = mission.LastObservation;
            }
            // On-foot scans: the existing synthetic scan (no measurement values exist for it).
            int n = hud.game.scans.Count;
            if (lastScanCount < 0) lastScanCount = n;
            if (n > lastScanCount)
            {
                var sc = hud.game.scans[n - 1]; string st = StationAt(sc.location);
                var target = mission.sensors.stations.FirstOrDefault(s => s && s.stableId == st);
                string body = $"{(target ? target.displayName : "Surface")} • SIMULATED surface scan recorded\nNo measurement values are modelled for on-foot scans\nSaved: game session (local) • not in the sensor upload queue";
                string detail = $"Sensor: {sc.instrument} • type {sc.type}\nStation: {(target ? target.displayName + " (" + st + ")" : "none")}\nSample id: {sc.sampleId}\nCollected: tick {sc.tick} • {DateTime.Now:HH:mm:ss}\nLocation: x {sc.location.x:0.0} m, z {sc.location.z:0.0} m (scene frame, not GPS)\n\nMeasurement values: NONE (the prototype scanner has no physical model; its 'synthetic index' is not a measurement and is not shown)\nData provenance: {sc.analysis.ToUpperInvariant()} (SIMULATED)\nLocal storage: game session snapshot (LOCAL ONLY)\nSync: not part of the sensor-record queue\nScientific interpretation: NOT CONNECTED\nRelated objective: {mission.Objectives().FirstOrDefault(o => !o.done && !o.info)?.text}";
                ShowPopup("SURFACE SCAN • COMPLETE", body, detail);
            }
            lastScanCount = n;
            if (ScanPopup.activeSelf && Time.unscaledTime > popupUntil) { ScanPopup.SetActive(false); RecordIcon.SetActive(true); }
        }

        static string LocalTime(string iso) => DateTime.TryParse(iso, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var t) ? t.ToLocalTime().ToString("HH:mm:ss") : "--:--:--";

        // ======================= systems drawer =======================
        static readonly string[] Sections = { "Mission", "Astronaut", "Rover", "Sensors", "Power", "Comms", "Data queue", "Science AI", "Navigation", "Controls", "Graphics" };

        void BuildDrawer()
        {
            var shade = Img("Systems drawer", Safe, Vector2.zero, Vector2.one, null, new Color(0, 0, 0, .35f), true);
            Drawer = shade.gameObject;
            var panel = Glass("Drawer panel", shade.transform, new Vector2(1, 0), new Vector2(1, 1), new Vector2(-980, 16), new Vector2(-16, -16), .9f);
            Tx("Drawer title", panel, "SYSTEMS", 24, TextAnchor.MiddleLeft, HudTokens.Cyan, new Vector2(.04f, .92f), new Vector2(.6f, .99f), FontStyle.Bold);
            DrawerClose = Btn("Drawer close", panel, "CLOSE", new Vector2(.78f, .915f), new Vector2(.97f, .99f), Vector2.zero, Vector2.zero, CloseDrawer, 18);
            var list = R("Sections", panel, new Vector2(.02f, .02f), new Vector2(.30f, .90f));
            for (int i = 0; i < Sections.Length; i++)
            {
                var sec = Sections[i];
                var b = Btn("Section " + sec, list, sec.ToUpperInvariant(), new Vector2(0, 1 - (i + 1) / (float)Sections.Length), new Vector2(1, 1 - i / (float)Sections.Length), new Vector2(0, 3), new Vector2(0, -3), () => OpenDrawer(sec), 17);
                b.GetComponent<Image>().color = new Color(1, 1, 1, .05f); sectionButtons[sec] = b;
            }
            drawerBody = Tx("Drawer body", panel, "", 17, TextAnchor.UpperLeft, HudTokens.Text, new Vector2(.33f, .02f), new Vector2(.97f, .90f));
            drawerSettings = R("Drawer settings", panel, new Vector2(.33f, .02f), new Vector2(.97f, .90f)).gameObject;
            BuildSettingsGrid(drawerSettings.transform, drawerSettingLabels, "Drawer setting ", 16);
            Drawer.SetActive(false);
        }

        public void OpenDrawer(string section)
        {
            DrawerSection = section;
            Drawer.SetActive(true); MapOverlay.SetActive(false);
            foreach (var kv in sectionButtons) kv.Value.GetComponent<Image>().color = kv.Key == section ? new Color(0, .42f, .52f, .9f) : new Color(1, 1, 1, .05f);
            RefreshDrawer();
        }
        public void CloseDrawer() => Drawer.SetActive(false);

        static string Tag(StatusReading r) => r.source switch { Source.GameState => "LIVE", Source.Simulated => "SIMULATED", Source.Local => "LOCAL ONLY", Source.NotConnected => "NOT CONNECTED", _ => "UNAVAILABLE" };

        void RefreshDrawer()
        {
            if (!Drawer.activeSelf) return;
            bool settings = DrawerSection == "Controls";
            drawerSettings.SetActive(settings); drawerBody.gameObject.SetActive(!settings);
            if (settings) { UpdateSettingLabels(drawerSettingLabels); return; }
            var g = hud.game; var p = g.prototype; var b = new StringBuilder();
            string Line(string id) { var r = Status.Get(id).Read(); return $"{r.label,-10} {(r.Available ? r.text + " " + r.unit : r.source)}   [{Tag(r)}]   {r.detail}"; }
            switch (DrawerSection)
            {
                case "Mission":
                    b.AppendLine("RED HORIZON • ag.mars.red-horizon.surface-ops v1 (local guidance, not scored)\n");
                    foreach (var o in mission.Objectives()) b.AppendLine((o.info ? "INFO     " : o.done ? "DONE     " : "PENDING  ") + o.text);
                    b.AppendLine("\n" + Line("mission")); break;
                case "Astronaut":
                    b.AppendLine($"Suit oxygen   {p.oxygen:0.0} %   [SIMULATED]   game rate, no consequence rules");
                    b.AppendLine($"Suit power    {p.power:0.0} %   [SIMULATED]");
                    b.AppendLine($"State         {(p.driving ? "In rover" : "EVA on foot")}   [LIVE]");
                    b.AppendLine($"Health        {g.astronautHealth:0} %   [SIMULATED]   never changes in this prototype"); break;
                case "Rover":
                    foreach (var id in new[] { "power", "temperature", "speed", "steering", "mode" }) b.AppendLine(Line(id));
                    b.AppendLine($"\nMeasured speed  {MeasuredSpeed * 3.6f:0.0} km/h   [LIVE]   from rover displacement");
                    b.AppendLine($"Drive profile   {Profile.ToString().ToUpperInvariant()}   [LIVE]");
                    b.AppendLine("Physics: kinematic CharacterController (TERRAVEX); max 3.8 m/s forward, 1.8 m/s reverse; brake 6 m/s².\nWheels/rocker-bogie: visual only (roll + ground-follow tilt). No traction, torque or rollover model."); break;
                case "Sensors":
                    b.AppendLine(Line("sensors") + "\n");
                    foreach (var i in mission.Catalog.Instruments) b.AppendLine($"{(i.available ? "AVAILABLE    " : "UNAVAILABLE  ")}{i.name}" + (i.available ? "" : " — " + i.unavailableReason)); break;
                case "Power":
                    b.AppendLine(Line("power"));
                    b.AppendLine("\nBase power (solar farm, distribution cabinet): status NOT MODELLED [UNAVAILABLE]");
                    b.AppendLine("This is the rover's game battery, not the Android device battery."); break;
                case "Comms":
                    b.AppendLine(Line("comms")); b.AppendLine(Line("sync")); break;
                case "Data queue":
                    var sum = RoverStatusProvider.SyncSummary(mission);
                    b.AppendLine(Line("storage"));
                    b.AppendLine($"Queue: {sum.label} • pending {sum.pending} • synced {sum.synced} • failed {sum.failed}   [{(mission.Sync.Uploader.Online ? "LIVE" : "LOCAL ONLY")}]");
                    b.AppendLine($"Scan records this session: {records.Count} (tap the record icon to open)"); break;
                case "Science AI":
                    b.AppendLine($"{mission.Analysis.StatusLabel}\nInterface {mission.Analysis.InterfaceVersion}   [NOT CONNECTED]\nNo results are shown or invented until the team's model is verified."); break;
                case "Navigation":
                    b.AppendLine($"Grid {(Grid.Ready ? "READY" : $"building {Grid.Progress * 100:0}%")} • {Grid.N}×{Grid.N} cells of {TraversabilityGrid.Cell} m • blocked {Grid.Blocked} • steep {Grid.Hazard}   [LIVE]");
                    b.AppendLine($"Target: {(Nav.Target != null ? Nav.Target.name : "none")} • {Nav.StatusLabel}");
                    b.AppendLine($"Locations in the world: {Registry.Locations.Count} • design proposals (not in the world): {Registry.Proposals.Count}");
                    b.AppendLine("Coordinates are scene metres (+Z = scene north), not Mars GPS."); break;
                case "Graphics":
                    b.AppendLine($"Graphics preset {hud.environment.quality} • {hud.environment.Readback()}\nChange it in CONTROLS (GRAPHICS PRESET button)."); break;
            }
            drawerBody.text = b.ToString();
        }

        // ======================= fullscreen map =======================
        const float MapDiameter = 860;

        void BuildMapOverlay()
        {
            var shade = Img("Map overlay", Safe, Vector2.zero, Vector2.one, null, new Color(HudTokens.Background.r, HudTokens.Background.g, HudTokens.Background.b, .96f), true);
            MapOverlay = shade.gameObject;
            // circular viewport with gestures
            var vp = Img("Map viewport", shade.transform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), HudFx.Circle, Color.white, true);
            vp.rectTransform.sizeDelta = new Vector2(MapDiameter, MapDiameter); vp.rectTransform.anchoredPosition = new Vector2(-170, -20);
            vp.gameObject.AddComponent<Mask>().showMaskGraphic = false;
            MapViewport = vp.rectTransform;
            var gest = vp.gameObject.AddComponent<MapGestures>(); gest.hud = this;
            MapContent = R("Map content", vp.transform, new Vector2(.5f, .5f), new Vector2(.5f, .5f)); MapContent.sizeDelta = new Vector2(MapDiameter, MapDiameter);
            var bg = Img("Grid background", MapContent, Vector2.zero, Vector2.one, null, new Color(.06f, .10f, .13f, 1)); bg.raycastTarget = false;
            mapImage = new GameObject("Terrain layer", typeof(RectTransform), typeof(RawImage)).GetComponent<RawImage>();
            mapImage.transform.SetParent(MapContent, false); Stretch(mapImage.rectTransform); mapImage.raycastTarget = false; mapImage.texture = Minimap.Texture;
            hazardImage = new GameObject("Hazard layer", typeof(RectTransform), typeof(RawImage)).GetComponent<RawImage>();
            hazardImage.transform.SetParent(MapContent, false); Stretch(hazardImage.rectTransform); hazardImage.raycastTarget = false; hazardImage.color = new Color(1, 1, 1, 0);
            routeLayer = R("Route layer", MapContent, Vector2.zero, Vector2.one);
            mapPreviewLayer = R("Preview layer", MapContent, Vector2.zero, Vector2.one);
            markerLayer = R("Marker layer", MapContent, Vector2.zero, Vector2.one);
            foreach (var l in Registry.Locations) MarkerById[l.id] = BuildMarker(l);
            playerMark = (RectTransform)Icon("Astronaut", markerLayer, "chevron", new Vector2(.5f, .5f), new Vector2(.5f, .5f), HudTokens.Green).transform; playerMark.sizeDelta = new Vector2(34, 34);
            roverMark = MarkerById[Registry.Rover.id];
            Img("Rim", vp.transform, Vector2.zero, Vector2.one, HudFx.CircleHairline, new Color(0, 210 / 255f, 1f, .6f)).raycastTarget = false;
            Img("Ticks", vp.transform, Vector2.zero, Vector2.one, HudFx.Ticks(72, 6), new Color(0, 210 / 255f, 1f, .18f)).raycastTarget = false;

            // compass (rotates with the map), scale bar, status
            var comp = Img("Map compass", shade.transform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), HudFx.Circle, HudFx.ControlFill); comp.rectTransform.sizeDelta = new Vector2(96, 96);
            comp.rectTransform.anchoredPosition = new Vector2(-170 + MapDiameter / 2 - 40, MapDiameter / 2 - 60); comp.raycastTarget = false;
            Img("Compass rim", comp.transform, Vector2.zero, Vector2.one, HudFx.CircleHairline, HudFx.ControlBorder).raycastTarget = false;
            compassNeedle = R("Needle", comp.transform, Vector2.zero, Vector2.one);
            var nTxt = Tx("N", compassNeedle, "N", 20, TextAnchor.UpperCenter, HudTokens.Orange, new Vector2(0, .5f), new Vector2(1, 1), FontStyle.Bold);
            Icon("Needle arrow", compassNeedle, "accelarrow", new Vector2(.35f, .3f), new Vector2(.65f, .75f), HudTokens.Orange);
            scaleBar = (RectTransform)Img("Scale bar", shade.transform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), null, HudTokens.Text).transform; scaleBar.sizeDelta = new Vector2(100, 3);
            scaleBar.anchoredPosition = new Vector2(-170 - MapDiameter / 2 + 120, -MapDiameter / 2 + 10);
            scaleText = Tx("Scale", shade.transform, "", 16, TextAnchor.MiddleLeft, HudTokens.Text, new Vector2(.5f, .5f), new Vector2(.5f, .5f)); scaleText.rectTransform.sizeDelta = new Vector2(240, 30);
            scaleText.rectTransform.anchoredPosition = new Vector2(-170 - MapDiameter / 2 + 190, -MapDiameter / 2 + 34);
            mapStatus = Tx("Map status", shade.transform, "", 16, TextAnchor.MiddleLeft, new Color(1, 1, 1, .7f), new Vector2(0, 1), new Vector2(0, 1)); mapStatus.rectTransform.offsetMin = new Vector2(24, -150); mapStatus.rectTransform.offsetMax = new Vector2(760, -100);
            var mt = Tx("Map title", shade.transform, "NAVIGATION • scene frame (not GPS)", 24, TextAnchor.MiddleLeft, HudTokens.Cyan, new Vector2(0, 1), new Vector2(0, 1), FontStyle.Bold);
            mt.rectTransform.offsetMin = new Vector2(24, -96); mt.rectTransform.offsetMax = new Vector2(760, -40);

            // left control column
            var col = R("Map controls", shade.transform, new Vector2(0, .5f), new Vector2(0, .5f)); col.sizeDelta = new Vector2(140, 640); col.anchoredPosition = new Vector2(100, -40);
            MapCenterAstronaut = CircleIconButton("Map centre astronaut", col, "astronaut", new Vector2(.5f, 1), new Vector2(0, -60), () => { CenterOn(hud.game.prototype.astronaut.transform.position); LastMapAction = "centre astronaut"; });
            MapCenterRover = CircleIconButton("Map centre rover", col, "rover", new Vector2(.5f, 1), new Vector2(0, -190), () => { CenterOn(hud.game.prototype.rover.transform.position); LastMapAction = "centre rover"; });
            MapOrientation = CircleIconButton("Map orientation", col, "compass", new Vector2(.5f, 1), new Vector2(0, -320), () => { HeadingUp = !HeadingUp; Say(HeadingUp ? "Map: heading up" : "Map: north up"); });
            MapReset = CircleIconButton("Map reset", col, "recenter2", new Vector2(.5f, 1), new Vector2(0, -450), ResetMap);
            MapClose = CircleIconButton("Map close", shade.transform, "close", new Vector2(1, 1), new Vector2(-78, -74), CloseMap);

            // layer toggles (bottom row)
            var row = R("Layers", shade.transform, new Vector2(0, 0), new Vector2(0, 0)); row.offsetMin = new Vector2(16, 16); row.offsetMax = new Vector2(16 + 9 * 152, 88);
            int k = 0;
            foreach (var key in Layers.Keys.ToList())
            {
                var kk = key;
                var b = Btn("Layer " + key, row, key.ToUpperInvariant(), new Vector2(k / 9f, 0), new Vector2((k + 1) / 9f, 1), new Vector2(3, 0), new Vector2(-3, 0), () => { Layers[kk] = !Layers[kk]; ApplyLayers(); }, 15);
                var im = b.GetComponent<Image>(); im.sprite = HudFx.Round(30); im.type = Image.Type.Sliced; layerButtons[key] = b; k++;
            }

            // info card (right)
            var card = Glass("Map info", shade.transform, new Vector2(1, .5f), new Vector2(1, .5f), new Vector2(-560, -330), new Vector2(-16, 300), .9f);
            infoCard = card.gameObject;
            infoTitle = Tx("Info title", card, "", 22, TextAnchor.UpperLeft, HudTokens.Orange, new Vector2(.05f, .82f), new Vector2(.95f, .97f), FontStyle.Bold);
            infoBody = Tx("Info body", card, "", 17, TextAnchor.UpperLeft, HudTokens.Text, new Vector2(.05f, .27f), new Vector2(.95f, .82f));
            MapNavigate = PillButton("Map navigate", card, "pin", "NAVIGATE", new Vector2(0, 0), new Vector2(20, 104), new Vector2(256, 196), StartNavigationToSelected, HudTokens.Orange);
            MapCancelRoute = PillButton("Map cancel route", card, "close", "CANCEL ROUTE", new Vector2(0, 0), new Vector2(272, 104), new Vector2(524, 196), () => { Nav.Cancel(); PreviewRoute = null; Say("Route cancelled"); RefreshInfo(); }, HudTokens.Cyan);
            var prop = Btn("Map proposals", card, "DESIGN PROPOSALS (not in the world)", new Vector2(0, 0), new Vector2(0, 0), new Vector2(20, 12), new Vector2(524, 92), () => ShowProposals(), 15);
            prop.GetComponent<Image>().color = new Color(1, 1, 1, .05f);
            MapOverlay.SetActive(false);
            ApplyLayers();
        }

        static void Stretch(RectTransform r) { r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one; r.offsetMin = r.offsetMax = Vector2.zero; }

        RectTransform BuildMarker(MapLocation l)
        {
            var c = l.category == LocationCategory.Rover ? HudTokens.Cyan : l.category == LocationCategory.ScienceStation ? HudTokens.Green : HudTokens.Text;
            var b = Btn("Marker " + l.id, markerLayer, "", new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(-30, -30), new Vector2(30, 30), () => Select(l), 1);
            var im = b.GetComponent<Image>(); im.sprite = HudFx.Circle; im.color = HudFx.ControlFill;
            Img("Ring", b.transform, Vector2.zero, Vector2.one, HudFx.CircleHairline, new Color(c.r, c.g, c.b, .9f)).raycastTarget = false;
            Icon("Icon", b.transform, l.icon, new Vector2(.2f, .2f), new Vector2(.8f, .8f), c);
            var lbl = Tx("Label", b.transform, l.name, 14, TextAnchor.UpperCenter, new Color(1, 1, 1, .85f), new Vector2(-1.5f, -.9f), new Vector2(2.5f, -.05f));
            lbl.rectTransform.anchorMin = new Vector2(.5f, 0); lbl.rectTransform.anchorMax = new Vector2(.5f, 0); lbl.rectTransform.sizeDelta = new Vector2(200, 24); lbl.rectTransform.anchoredPosition = new Vector2(0, -14);
            return (RectTransform)b.transform;
        }

        public void OpenMap()
        {
            MapOverlay.SetActive(true); Drawer.SetActive(false); mapOpenedAt = Time.unscaledTime;
            if (MapZoom <= 1.01f && MapCenter == new Vector2(.5f, .5f)) { MapZoom = 3f; CenterOn(hud.game.prototype.ActivePosition()); }
            if (Selected == null) Select(Nav.Target ?? Registry.Rover);
            RefreshInfo();
        }
        public void CloseMap() { MapOverlay.SetActive(false); }
        public void ResetMap() { MapZoom = 1; MapCenter = new Vector2(.5f, .5f); HeadingUp = false; LastMapAction = "reset"; }
        public void CenterOn(Vector3 world) => MapCenter = MapCoordinateConverter.ToMap(world);
        public void PanBy(Vector2 screenDelta)
        {
            var cs = MapViewport.GetComponentInParent<Canvas>().scaleFactor;
            var d = (Vector2)(Quaternion.Euler(0, 0, -MapContent.localEulerAngles.z) * (screenDelta / cs));
            MapCenter -= d / (MapDiameter * MapZoom);
            MapCenter = new Vector2(Mathf.Clamp01(MapCenter.x), Mathf.Clamp01(MapCenter.y));
        }
        public void ZoomBy(float factor) { MapZoom = Mathf.Clamp(MapZoom * factor, 1f, 12f); }

        public void Select(MapLocation l)
        {
            Selected = l;
            PreviewRoute = l != null && l.navigable ? Nav.Preview(l) : null;
            RefreshInfo();
        }

        void StartNavigationToSelected()
        {
            if (Selected == null || !Selected.navigable) { Say("Not a navigable location"); return; }
            Nav.SetTarget(Selected); Say("Navigating to " + Selected.name); CloseMap();
        }

        void ShowProposals()
        {
            Selected = null; PreviewRoute = null;
            infoTitle.text = "DESIGN PROPOSALS";
            infoBody.text = "These facilities do NOT exist in the playable world yet. They need approval before they are built:\n\n" +
                string.Join("\n", Registry.Proposals.Select(p => "• " + p.name + " — " + p.state)) + "\n\nRepair gameplay: no repairable state exists yet (proposal).";
            MapNavigate.interactable = false;
        }

        void RefreshInfo()
        {
            if (!MapOverlay || !MapOverlay.activeSelf || infoTitle == null) return;
            var p = hud.game.prototype; var l = Selected;
            if (l == null) { infoTitle.text = "SELECT A LOCATION"; infoBody.text = "Tap a marker on the map."; MapNavigate.interactable = false; return; }
            float dist = RoverSensorSuite.Planar(p.ActivePosition(), l.Position);
            var b = new StringBuilder();
            b.AppendLine($"{l.CategoryLabel}");
            b.AppendLine($"Distance {dist:0} m • bearing {RoverSensorSuite.Bearing(p.ActivePosition(), l.Position):000}° (scene north)");
            b.AppendLine($"State: {(string.IsNullOrEmpty(l.state) ? "—" : l.state)}");
            b.AppendLine($"Interaction: {l.interaction}");
            if (!string.IsNullOrEmpty(l.mission)) b.AppendLine($"Mission: {l.mission}");
            var r = Nav.Target == l ? Nav.Route : PreviewRoute;
            if (r != null)
                b.AppendLine("Route: " + (r.status == RouteStatus.Available ? $"AVAILABLE • {r.length:0} m path ({r.points.Count} legs)" : r.status == RouteStatus.Arrived ? "ARRIVED" : r.status.ToString().ToUpperInvariant() + " • " + r.message));
            if (Nav.Target == l) b.AppendLine($"Active navigation: {Nav.StatusLabel}");
            infoTitle.text = l.name.ToUpperInvariant();
            infoBody.text = b.ToString();
            MapNavigate.interactable = l.navigable;
            MapCancelRoute.interactable = Nav.Target != null;
        }

        void ApplyLayers()
        {
            foreach (var kv in layerButtons)
            {
                var on = Layers[kv.Key];
                kv.Value.GetComponent<Image>().color = on ? new Color(0, .42f, .52f, .85f) : HudFx.ControlFill;
                kv.Value.GetComponentInChildren<Text>().color = on ? HudTokens.Text : new Color(1, 1, 1, .5f);
            }
            if (mapImage) mapImage.enabled = Layers["Terrain"];
            if (hazardImage) hazardImage.color = new Color(1, 1, 1, Layers["Hazards"] ? 1 : 0);
            if (routeLayer) routeLayer.gameObject.SetActive(Layers["Routes"]);
            if (mapPreviewLayer) mapPreviewLayer.gameObject.SetActive(Layers["Routes"]);
            foreach (var l in Registry.Locations) if (MarkerById.TryGetValue(l.id, out var m)) m.gameObject.SetActive(LayerVisible(l));
        }

        bool LayerVisible(MapLocation l)
        {
            switch (l.category)
            {
                case LocationCategory.Rover: return Layers["Rover"];
                case LocationCategory.ScienceStation:
                    bool collected = l.state == "DATA COLLECTED";
                    bool missionSite = mission.Designated && l.id == mission.Designated.stableId;
                    return (Layers["Science"] && (collected ? Layers["Collected"] : Layers["Unvisited"])) || (Layers["Mission"] && missionSite);
                default: return Layers["Base"];
            }
        }

        void UpdateMap()
        {
            if (!MapOverlay.activeSelf) return;
            if (!mapImage.texture && Minimap.Texture) mapImage.texture = Minimap.Texture;
            var p = hud.game.prototype;
            float rot = HeadingUp ? (p.driving ? p.rover.transform.eulerAngles.y : p.yaw) : 0;
            MapContent.sizeDelta = Vector2.one * MapDiameter * MapZoom;
            MapContent.pivot = MapCenter; MapContent.anchoredPosition = Vector2.zero;
            MapContent.localEulerAngles = new Vector3(0, 0, rot);
            compassNeedle.localEulerAngles = new Vector3(0, 0, rot);
            Vector2 Local(Vector3 w) { var uv = MapCoordinateConverter.ToMap(w); return new Vector2((uv.x - .5f) * MapContent.sizeDelta.x, (uv.y - .5f) * MapContent.sizeDelta.y); }
            foreach (var l in Registry.Locations)
            {
                if (!MarkerById.TryGetValue(l.id, out var m) || !l.transform) continue;
                m.anchoredPosition = Local(l.Position); m.localEulerAngles = new Vector3(0, 0, -rot);
                m.localScale = Vector3.one * (Selected == l ? 1.25f : 1f);
            }
            roverMark.localEulerAngles = new Vector3(0, 0, -rot);
            roverMark.Find("Icon").localEulerAngles = new Vector3(0, 0, -p.rover.transform.eulerAngles.y + rot);
            playerMark.gameObject.SetActive(!p.driving);
            playerMark.anchoredPosition = Local(p.astronaut.transform.position);
            playerMark.localEulerAngles = new Vector3(0, 0, -p.astronaut.transform.eulerAngles.y + 90);
            DrawPolyline(routeSegs, routeLayer, Nav.Target != null && Nav.Route.status == RouteStatus.Available ? Nav.Route.points : null, HudTokens.Orange, Local);
            DrawPolyline(previewSegs, mapPreviewLayer, PreviewRoute != null && PreviewRoute.status == RouteStatus.Available && Nav.Target != Selected ? PreviewRoute.points : null, new Color(0, 210 / 255f, 1f, .8f), Local);
            // scale bar: the largest nice distance that fits in ~220 px
            float pxPerM = MapContent.sizeDelta.x / (MapCoordinateConverter.HalfExtent * 2);
            float metres = new[] { 5f, 10f, 20f, 25f, 50f, 100f }.LastOrDefault(m => m * pxPerM <= 240); if (metres <= 0) metres = 5;
            scaleBar.sizeDelta = new Vector2(metres * pxPerM, 3); scaleText.text = $"{metres:0} m  •  zoom ×{MapZoom:0.0}  •  {(HeadingUp ? "HEADING UP" : "NORTH UP")}";
            mapStatus.text = Grid.Ready ? $"Route grid ready • {Nav.StatusLabel}" : $"Building route grid {Grid.Progress * 100:0}% • NAVIGATION UNAVAILABLE until ready";
        }

        void DrawPolyline(List<Image> pool, RectTransform layer, List<Vector3> pts, Color c, Func<Vector3, Vector2> local)
        {
            int need = pts == null ? 0 : Mathf.Max(0, pts.Count - 1);
            while (pool.Count < need) { var s = Img("Seg", layer, new Vector2(.5f, .5f), new Vector2(.5f, .5f), null, c); s.raycastTarget = false; pool.Add(s); }
            for (int i = 0; i < pool.Count; i++)
            {
                bool on = i < need; pool[i].gameObject.SetActive(on); if (!on) continue;
                var a = local(pts[i]); var b2 = local(pts[i + 1]); var d = b2 - a;
                var rt = pool[i].rectTransform; rt.anchoredPosition = (a + b2) / 2; rt.sizeDelta = new Vector2(d.magnitude, 5); rt.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg);
                pool[i].color = c;
            }
        }

        // ======================= per-frame =======================
        protected override bool ShowClassicPanels => false;

        protected override void LateUpdate()
        {
            base.LateUpdate();
            if (!built) return;
            var p = hud.game.prototype; bool driving = p.driving;
            bool paused = hud.environment.paused || hud.modalOpen;
            bool overlay = MapOverlay.activeSelf || Drawer.activeSelf || RecordView.activeSelf;
            if (control) control.uiBlocking = overlay;
            if (overlay) ReleaseDriveInputs();
            // Gameplay controls are hidden and inert behind fullscreen overlays (no active-state churn).
            foreach (var root in new[] { AstronautControls, DriveControls })
            {
                var cg = root.GetComponent<CanvasGroup>(); if (!cg) cg = root.AddComponent<CanvasGroup>();
                cg.alpha = overlay ? 0 : mission.Settings.opacity; cg.blocksRaycasts = cg.interactable = !overlay;
            }
            if (overlay && RecordIcon.activeSelf) RecordIcon.SetActive(false);
            else if (!overlay && !ScanPopup.activeSelf && records.Count > 0 && !RecordIcon.activeSelf) RecordIcon.SetActive(true);

            // measured rover speed (from motion, not the commanded value)
            var rp = p.rover.transform.position;
            if (roverInit && Time.deltaTime > 0) MeasuredSpeed = Mathf.Lerp(MeasuredSpeed, Vector2.Distance(new Vector2(rp.x, rp.z), new Vector2(lastRoverPos.x, lastRoverPos.z)) / Time.deltaTime, .25f);
            lastRoverPos = rp; roverInit = true;

            CompassBar.SetActive(!paused && !overlay);
            compassHeading.gameObject.SetActive(!paused && !overlay);
            ObjectiveChip.SetActive(!paused && !overlay && Current == Tab.Drive);
            MapButton.gameObject.SetActive(!paused && !overlay); MenuButton.gameObject.SetActive(!paused && !overlay); ExplorerPause.gameObject.SetActive(!paused && !overlay);
            ModePill.SetActive(driving && !paused && !overlay && (Current == Tab.Drive || Current == Tab.Science));
            ProfilePill.SetActive(driving && !paused && !overlay && Current == Tab.Drive);
            NavigateToRover.gameObject.SetActive(!driving && Vector3.Distance(p.astronaut.transform.position, p.rover.transform.position) > 5);
            NavigateToRover.GetComponentInChildren<Text>().text = Nav.Target == Registry.Rover ? "STOP NAVIGATION" : "NAVIGATE TO ROVER";
            foreach (var kv in modeButtons) { bool on = (kv.Key == "DRIVE" && Current == Tab.Drive) || (kv.Key == "SCIENCE" && Current == Tab.Science); kv.Value.GetComponent<Image>().color = on ? new Color(0, .42f, .52f, .85f) : Color.clear; }
            foreach (var kv in profileButtons) { bool on = kv.Key == Profile.ToString().ToUpperInvariant(); kv.Value.GetComponent<Image>().color = on ? new Color(0, .42f, .52f, .85f) : Color.clear; kv.Value.GetComponentInChildren<Text>().color = kv.Key == "TRAVERSE" ? new Color(1, 1, 1, .35f) : HudTokens.Text; }

            // compass
            float hdg = driving ? p.rover.transform.eulerAngles.y : p.yaw;
            float w = ((RectTransform)CompassBar.transform).rect.width;
            foreach (Transform child in CompassBar.transform)
            {
                float deg;
                if (child.name.StartsWith("Mark ")) deg = marks.First(m => m.t && m.t.transform == child).deg;
                else if (child.name.StartsWith("Tick ")) deg = float.Parse(child.name.Substring(5), CultureInfo.InvariantCulture);
                else continue;
                float d = Mathf.DeltaAngle(hdg, deg); var rt = (RectTransform)child;
                child.gameObject.SetActive(Mathf.Abs(d) < 80); rt.anchoredPosition = new Vector2(d / 170f * w, 0);
            }
            compassHeading.text = $"{Mathf.Repeat(hdg, 360):000}°";

            // navigation tick + waypoint guidance
            Nav.Tick(Time.unscaledTime);
            var target = Nav.Target;
            compassPin.gameObject.SetActive(target != null);
            RouteChip.SetActive(target != null && !overlay && !paused);
            WaypointMarker.SetActive(target != null && !overlay && !paused);
            if (target != null)
            {
                float brg = RoverSensorSuite.Bearing(p.ActivePosition(), target.Position);
                compassPin.anchoredPosition = new Vector2(Mathf.Clamp(Mathf.DeltaAngle(hdg, brg), -78, 78) / 170f * w, 0);
                routeChipText.text = $"{target.name.ToUpperInvariant()} • {Nav.StatusLabel}";
                PlaceWaypoint(target);
            }

            if (Time.unscaledTime >= nextRegistryRefresh) { nextRegistryRefresh = Time.unscaledTime + 1; Registry.Refresh(id => mission.Store.Records.Any(r => r.stationId == id && r.provenance == Provenance.Simulated) || hud.game.scans.Any(sc => sc != null && StationAt(sc.location) == id)); RefreshInfo(); RefreshDrawer(); ApplyLayers(); }
            UpdateMap();
            WatchScans();
            // mouse wheel zoom on the map (desktop)
            var m2 = Mouse.current; if (MapOverlay.activeSelf && m2 != null && !hud.game.testing) { float sc = m2.scroll.ReadValue().y; if (Mathf.Abs(sc) > .01f) ZoomBy(sc > 0 ? 1.15f : 1 / 1.15f); }
        }

        void ReleaseDriveInputs()
        {
            if (SteerStick) SteerStick.Clear(); if (MoveStick) MoveStick.Clear();
            if (Accelerate) Accelerate.Clear(); if (Brake) Brake.Clear(); if (Run) Run.Clear();
            hud.movement = Vector2.zero; hud.runHeld = false; hud.brakeHeld = false;
        }

        void PlaceWaypoint(MapLocation target)
        {
            var cam = hud.game.prototype.follow; if (!cam) return;
            var canvas = Safe.GetComponentInParent<Canvas>(); var cr = (RectTransform)canvas.transform;
            var world = target.Position + Vector3.up * 2.5f;
            var sp = cam.WorldToScreenPoint(world);
            bool behind = sp.z < 0; if (behind) sp = -sp;
            var size = cr.rect.size * canvas.scaleFactor;
            var screen = new Vector2(sp.x, sp.y);
            var margin = 90f * canvas.scaleFactor;
            bool onScreen = !behind && screen.x > margin && screen.x < Screen.width - margin && screen.y > margin && screen.y < Screen.height - margin;
            var centre = new Vector2(Screen.width, Screen.height) / 2;
            if (!onScreen)
            {
                var dir = (screen - centre); if (behind) dir = -dir; if (dir.sqrMagnitude < 1) dir = Vector2.up;
                dir.Normalize();
                float sx = (Screen.width / 2f - margin) / Mathf.Max(.001f, Mathf.Abs(dir.x)), sy = (Screen.height / 2f - margin) / Mathf.Max(.001f, Mathf.Abs(dir.y));
                screen = centre + dir * Mathf.Min(sx, sy);
                edgeArrow.gameObject.SetActive(true); edgeArrow.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
            }
            else edgeArrow.gameObject.SetActive(false);
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)WaypointMarker.transform.parent, screen, null, out var local);
            var wr = (RectTransform)WaypointMarker.transform; wr.anchorMin = wr.anchorMax = new Vector2(.5f, .5f); wr.anchoredPosition = local;
            float dist = RoverSensorSuite.Planar(hud.game.prototype.ActivePosition(), target.Position);
            waypointText.text = Nav.Arrived ? "ARRIVED" : $"{dist:0} m";
        }

        protected override void Refresh()
        {
            base.Refresh();
            if (objectiveText) objectiveText.text = mission.Instructions();
        }
    }

    // One finger pans, two fingers pinch-zoom, double tap zooms in (all on the circular viewport only).
    public class MapGestures : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerClickHandler
    {
        public ExplorerHUD hud;
        readonly Dictionary<int, Vector2> fingers = new Dictionary<int, Vector2>();
        float lastPinch, lastTap;
        public void OnPointerDown(PointerEventData e) { fingers[e.pointerId] = e.position; lastPinch = Pinch(); }
        public void OnPointerUp(PointerEventData e) { fingers.Remove(e.pointerId); lastPinch = Pinch(); }
        public void OnDrag(PointerEventData e)
        {
            fingers[e.pointerId] = e.position;
            if (fingers.Count >= 2) { float d = Pinch(); if (lastPinch > 0 && d > 0) hud.ZoomBy(d / lastPinch); lastPinch = d; return; }
            hud.PanBy(e.delta);
        }
        public void OnPointerClick(PointerEventData e)
        {
            if (e.dragging) return;
            if (Time.unscaledTime - lastTap < .3f) { hud.ZoomBy(2f); lastTap = 0; } else lastTap = Time.unscaledTime;
        }
        float Pinch() { if (fingers.Count < 2) return 0; var v = new List<Vector2>(fingers.Values); return Vector2.Distance(v[0], v[1]); }
        void OnDisable() { fingers.Clear(); }
    }
}
