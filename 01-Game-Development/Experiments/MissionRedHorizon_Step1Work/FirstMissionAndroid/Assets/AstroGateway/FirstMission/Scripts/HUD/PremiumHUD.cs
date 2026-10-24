using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using AstroGateway.MarsEnvironment;

// Premium HUD v2 for Mars_FirstMission_HUDv2 (a separate scene; the First Mission HUD v1 scene is unchanged).
// Inherits the v1 HUD's input feeding, mode switching, safe-area handling, pause routing and touch widgets,
// and replaces only the presentation:
//   TOP BAR      branding + control mode | DRIVE SCIENCE DATA MISSION SYSTEMS | session time, link, rover power, pause
//   LEFT         mission card (objective, progress, collapsible) + rover status summary (4 or all 10 widgets)
//   RIGHT        navigation: compass strip, real-scene minimap, waypoint, terrain summary (collapsible)
//   BOTTOM LEFT  joystick + CAMERA + VIEW      BOTTOM CENTRE speedometer, drive state, steering feedback
//   BOTTOM RIGHT ACCEL, BRAKE, REVERSE, contextual EXIT / SCIENCE / RECENTER
// Mode panels: SCIENCE (instruments, target, range, scan, result), DATA (records table + 5 sync states + AI),
// MISSION (objectives), SYSTEMS (10 status widgets + settings). Paused: settings in the pause card.
// No value is invented: every widget shows its source (GAME STATE / SIMULATED / LOCAL / NOT AVAILABLE / NOT CONNECTED).
namespace AstroGateway.FirstMission.HUD
{
    // Drives the speed glow arc from the main arc's fill.
    public class ArcGlowLink : MonoBehaviour { public Image glow; Image arc; void LateUpdate() { if (!arc) arc = GetComponent<Image>(); if (glow) { glow.fillAmount = arc.fillAmount; glow.color = new Color(arc.color.r, arc.color.g, arc.color.b, .16f); } } }

    public class PremiumHUD : FirstMissionHUD
    {
        // exposed for tests
        public RoverStatusProvider Status { get; private set; }
        public readonly List<HudStatusWidget> SummaryWidgets = new List<HudStatusWidget>(), SystemWidgets = new List<HudStatusWidget>();
        public HudMinimap Minimap { get; protected set; }
        public GameObject TopBar, MissionCard, StatusSummary, NavWidget, AlertBanner, SpeedGauge;
        public Button MissionCollapse, NavCollapse, StatusExpand, DataPrev, DataNext, ScanButton;
        public bool MissionCollapsed { get; private set; }
        public bool NavCollapsed { get; private set; }
        public bool StatusExpanded { get; private set; }
        public string CurrentAlert { get; private set; } = "";
        public Transform Waypoint { get; private set; }
        public readonly List<Text[]> DataRows = new List<Text[]>();
        public int DataPage { get; private set; }
        public Text DataSummaryText, MissionTitleText, ScienceResultText, WaypointText, TerrainText, PositionText;

        Sprite glass, glassEdge;
        Text brand, modeText, sessionText, linkText, powerText, missionObjective, missionProgressText, speedValue, speedUnit, driveState, scienceTarget, scienceRange, scanLabel, alertText;
        Image linkIcon, powerIcon, missionBar, speedArc, steerMarker, rangeBar, scanFill, alertIcon;
        RectTransform missionBody, statusBody, navBody, compassStrip, compassWaypoint;
        readonly List<(Text t, float deg)> compassMarks = new List<(Text, float)>();
        readonly List<RectTransform> objectiveRows = new List<RectTransform>();
        readonly List<Text> objectiveTexts = new List<Text>();
        readonly List<Image> objectiveDots = new List<Image>();
        readonly Dictionary<string, Button> sciInstrumentButtons = new Dictionary<string, Button>();
        readonly Dictionary<string, Text> pauseSettingLabels = new Dictionary<string, Text>();
        RectTransform settingsHost, systemsWidgetsHost;
        bool[] lastDone = new bool[0];
        float[] pulse = new float[16];
        float alertUntil, missionH = 1, navH = 1, statusH = 0, lastTick;
        string lastAlertKey = "";

        static readonly string[] SummaryIds = { "power", "comms", "storage", "sync" };
        static readonly string[] AllIds = { "power", "temperature", "comms", "storage", "speed", "steering", "mode", "sensors", "mission", "sync" };

        // ======================= BUILD =======================
        protected override void Build()
        {
            glass = HudIcons.Panel(HudTokens.Radius); glassEdge = HudIcons.Panel(HudTokens.Radius, true, 2);
            rounded = glass; circle = HudIcons.Get("disc") ?? circle;
            Status = new RoverStatusProvider(mission, control);

            var root = new GameObject("Premium HUD Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            root.transform.SetParent(transform, false);
            var canvas = root.GetComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 20;
            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize; scaler.referenceResolution = new Vector2(1920, 1080); scaler.matchWidthOrHeight = 1;
            Safe = R("Safe area", root.transform, Vector2.zero, Vector2.one);

            var look = Img("Camera drag zone", Safe, new Vector2(.24f, .22f), new Vector2(.76f, .84f), null, new Color(0, 0, 0, 0), true);
            var lz = look.gameObject.AddComponent<LookZone>(); lz.hud = hud; lz.ui = this;

            BuildTopBar();
            BuildMissionCard();
            BuildStatusSummary();
            BuildNav();
            BuildAstronautControls();
            BuildDriveControls();
            BuildModePanels();
            BuildOverlays();
            ApplySettings();
        }

        // ---------- primitives ----------
        // Panel: transparent hit area + soft shadow + gradient glass + thin edge + aerospace corner brackets.
        protected RectTransform Glass(string name, Transform parent, Vector2 a, Vector2 b, Vector2 min, Vector2 max, float alpha = .72f, bool corners = true)
        {
            var im = Img(name, parent, a, b, null, new Color(0, 0, 0, 0), true);
            im.rectTransform.offsetMin = min; im.rectTransform.offsetMax = max;
            var sh = Img("Shadow", im.transform, Vector2.zero, Vector2.one, HudFx.Shadow, Color.white); sh.type = Image.Type.Sliced; sh.raycastTarget = false;
            sh.rectTransform.offsetMin = new Vector2(-26, -34); sh.rectTransform.offsetMax = new Vector2(26, 18);
            var fill = Img("Glass", im.transform, Vector2.zero, Vector2.one, HudFx.GlassFill, HudTokens.PanelGlass(alpha)); fill.type = Image.Type.Sliced; fill.raycastTarget = false;
            var e = Img("Edge", im.transform, Vector2.zero, Vector2.one, glassEdge, HudTokens.Edge(.28f)); e.raycastTarget = false;
            if (corners) { var c = Img("Corners", im.transform, Vector2.zero, Vector2.one, HudFx.Corners, HudTokens.Edge(.75f)); c.type = Image.Type.Sliced; c.raycastTarget = false; }
            return im.rectTransform;
        }

        // Header toggles: 94 px transparent hit area (touch-friendly) with the glyph kept in the header strip.
        protected void HeaderButton(Button b)
        {
            b.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            var t = b.GetComponentInChildren<Text>(); t.color = HudTokens.Cyan;
            t.rectTransform.anchorMin = new Vector2(0, .52f); t.rectTransform.anchorMax = new Vector2(1, 1);
            var cb = b.colors; cb.pressedColor = new Color(1, 1, 1, .2f); b.colors = cb;
            b.gameObject.AddComponent<HudPressFx>();
        }

        // Shared button finish: gradient glass, thin edge, press glow + scale.
        protected void Finish(Selectable sel, Color accent, float alpha = .82f)
        {
            var img = sel.GetComponent<Image>(); img.sprite = HudFx.GlassFill; img.type = Image.Type.Sliced; img.color = HudTokens.PanelGlass(alpha);
            var e = Img("Edge", sel.transform, Vector2.zero, Vector2.one, glassEdge, new Color(accent.r, accent.g, accent.b, .45f)); e.raycastTarget = false; e.transform.SetSiblingIndex(0);
            var glow = Img("Press glow", sel.transform, Vector2.zero, Vector2.one, HudFx.Glow, new Color(accent.r, accent.g, accent.b, 0)); glow.raycastTarget = false; glow.transform.SetSiblingIndex(1);
            glow.rectTransform.offsetMin = new Vector2(-30, -30); glow.rectTransform.offsetMax = new Vector2(30, 30);
            var fx = sel.gameObject.AddComponent<HudPressFx>(); fx.glow = glow;
        }
        protected Image Icon(string name, Transform parent, string id, Vector2 a, Vector2 b, Color c)
        {
            var im = Img(name, parent, a, b, HudIcons.Get(id), c); im.preserveAspect = true; im.raycastTarget = false; return im;
        }
        protected Text Tx(string name, Transform parent, string text, int size, TextAnchor anchor, Color c, Vector2 a, Vector2 b, FontStyle st = FontStyle.Normal) => T(name, parent, text, size, anchor, c, a, b, st);
        protected Button IconButton(string name, Transform parent, string icon, string label, Vector2 a, Vector2 b, Vector2 min, Vector2 max, Action click)
        {
            var btn = Btn(name, parent, label, a, b, min, max, click, 20);
            Finish(btn, HudTokens.Cyan);
            var lbl = btn.GetComponentInChildren<Text>(); lbl.color = HudTokens.Text;
            if (!string.IsNullOrEmpty(icon))
            {
                Icon("Icon", btn.transform, icon, new Vector2(.1f, .42f), new Vector2(.9f, .92f), HudTokens.Cyan);
                lbl.rectTransform.anchorMin = new Vector2(.02f, .04f); lbl.rectTransform.anchorMax = new Vector2(.98f, .40f);
                lbl.fontSize = 18; lbl.resizeTextMaxSize = 18;
            }
            return btn;
        }

        // ---------- top bar ----------
        void BuildTopBar()
        {
            var bar = Glass("Top bar", Safe, new Vector2(0, 1), new Vector2(1, 1), new Vector2(12, -116), new Vector2(-12, -10), .62f);
            TopBar = bar.gameObject;
            var brandBox = R("Brand", bar, new Vector2(0, 0), new Vector2(0, 1)); brandBox.offsetMin = new Vector2(20, 0); brandBox.offsetMax = new Vector2(390, 0);
            brand = Tx("Branding", brandBox, "ASTROGATEWAY", 24, TextAnchor.MiddleLeft, HudTokens.Text, new Vector2(0, .5f), new Vector2(1, .96f), FontStyle.Bold);
            modeText = Tx("Control mode", brandBox, "", 20, TextAnchor.MiddleLeft, HudTokens.Cyan, new Vector2(0, .06f), new Vector2(1, .5f), FontStyle.Bold);

            TabBar = R("Tab bar", bar, new Vector2(0, 0), new Vector2(1, 1)).gameObject;   // re-laid out each frame between brand and the right cluster
            var tb = (RectTransform)TabBar.transform; tb.offsetMin = new Vector2(400, 6); tb.offsetMax = new Vector2(-600, -6);
            string[] names = { "DRIVE", "SCIENCE", "DATA", "MISSION", "SYSTEMS" };
            string[] icons = { "wheel", "scan", "database", "flag", "mode" };
            var pill = Img("Tab pill", tb, new Vector2(.5f, .5f), new Vector2(.5f, .5f), HudFx.GlassFill, new Color(HudTokens.Cyan.r, HudTokens.Cyan.g, HudTokens.Cyan.b, .22f)); pill.type = Image.Type.Sliced; pill.raycastTarget = false;
            Img("Pill edge", pill.transform, Vector2.zero, Vector2.one, glassEdge, HudTokens.Edge(.8f)).raycastTarget = false;
            var tabFx = tb.gameObject.AddComponent<HudTabIndicator>(); tabFx.ui = this; tabFx.pill = pill.rectTransform;
            for (int i = 0; i < 5; i++)
            {
                var tab = (Tab)i;
                var b = Btn("Tab " + names[i], tb, names[i], new Vector2(i / 5f, 0), new Vector2((i + 1) / 5f, 1), new Vector2(4, 0), new Vector2(-4, 0), () => Select(tab), 22);
                var img = b.GetComponent<Image>(); img.sprite = glass; img.color = new Color(1, 1, 1, .06f);
                var lbl = b.GetComponentInChildren<Text>(); lbl.rectTransform.anchorMin = new Vector2(.30f, .1f); lbl.alignment = TextAnchor.MiddleLeft;
                Icon("Tab icon", b.transform, icons[i], new Vector2(.06f, .22f), new Vector2(.27f, .78f), HudTokens.Cyan);
                var line = Img("Selected underline", b.transform, new Vector2(.08f, 0), new Vector2(.92f, .07f), null, HudTokens.Cyan); line.raycastTarget = false;
                TabButtons[tab] = b; tabFx.tabs[tab] = (RectTransform)b.transform;
                b.gameObject.AddComponent<HudPressFx>();
            }

            // right cluster (fixed width, anchored right): session time, link, rover power, pause
            var rc = R("Status cluster", bar, new Vector2(1, 0), new Vector2(1, 1)); rc.offsetMin = new Vector2(-600, 0); rc.offsetMax = new Vector2(-102, 0);
            Icon("Session icon", rc, "clock", new Vector2(0, .25f), new Vector2(.07f, .75f), HudTokens.TextDim);
            sessionText = Tx("Session time", rc, "", 18, TextAnchor.MiddleLeft, HudTokens.Text, new Vector2(.075f, .1f), new Vector2(.40f, .9f));
            linkIcon = Icon("Link icon", rc, "nosignal", new Vector2(.40f, .25f), new Vector2(.47f, .75f), HudTokens.Amber);
            linkText = Tx("Link status", rc, "", 16, TextAnchor.MiddleLeft, HudTokens.Amber, new Vector2(.475f, .1f), new Vector2(.70f, .9f), FontStyle.Bold);
            powerIcon = Icon("Rover power icon", rc, "battery", new Vector2(.70f, .25f), new Vector2(.78f, .75f), HudTokens.Green);
            powerText = Tx("Rover power", rc, "", 16, TextAnchor.MiddleLeft, HudTokens.Green, new Vector2(.79f, .05f), new Vector2(1f, .95f), FontStyle.Bold);
            PauseButton = Btn("Pause", bar, "", new Vector2(1, .5f), new Vector2(1, .5f), new Vector2(-96, -46), new Vector2(-6, 46), () => hud.TogglePause(), 20);
            PauseButton.GetComponent<Image>().sprite = glass; PauseButton.GetComponent<Image>().color = new Color(1, 1, 1, .08f);
            Icon("Pause icon", PauseButton.transform, "pause", new Vector2(.2f, .2f), new Vector2(.8f, .8f), HudTokens.Text);

            var toast = Glass("Action feedback", Safe, new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(-300, -170), new Vector2(300, -116), .8f);
            toastText = Tx("Toast", toast, "", 22, TextAnchor.MiddleCenter, HudTokens.Cyan, Vector2.zero, Vector2.one, FontStyle.Bold);
            toast.gameObject.SetActive(false);
        }

        // ---------- left: mission card ----------
        void BuildMissionCard()
        {
            var card = Glass("Mission card", Safe, new Vector2(0, 1), new Vector2(0, 1), new Vector2(12, -342), new Vector2(452, -128));
            MissionCard = card.gameObject;
            Icon("Mission icon", card, "flag", new Vector2(.03f, .80f), new Vector2(.09f, .96f), HudTokens.Orange);
            MissionTitleText = Tx("Mission title", card, "RED HORIZON • FIRST MISSION", 18, TextAnchor.MiddleLeft, HudTokens.Orange, new Vector2(.11f, .80f), new Vector2(.84f, .97f), FontStyle.Bold);
            MissionCollapse = Btn("Mission collapse", card, "–", new Vector2(1, 1), new Vector2(1, 1), new Vector2(-94, -94), new Vector2(-4, -4), () => { MissionCollapsed = !MissionCollapsed; }, 26);
            HeaderButton(MissionCollapse);
            missionBody = R("Mission body", card, new Vector2(0, 0), new Vector2(1, .78f));
            missionObjective = Tx("Current objective", missionBody, "", 22, TextAnchor.UpperLeft, HudTokens.Text, new Vector2(.04f, .38f), new Vector2(.96f, .98f), FontStyle.Bold);
            var track = Img("Mission progress track", missionBody, new Vector2(.04f, .24f), new Vector2(.96f, .30f), glass, new Color(1, 1, 1, .10f));
            missionBar = Img("Fill", track.transform, Vector2.zero, new Vector2(0, 1), glass, HudTokens.Orange);
            missionProgressText = Tx("Mission progress", missionBody, "", 17, TextAnchor.MiddleLeft, HudTokens.TextDim, new Vector2(.04f, .02f), new Vector2(.96f, .22f));
        }

        // ---------- left: rover status summary ----------
        void BuildStatusSummary()
        {
            var card = Glass("Rover status", Safe, new Vector2(0, 1), new Vector2(0, 1), new Vector2(12, -564), new Vector2(452, -354));
            StatusSummary = card.gameObject;
            Tx("Status title", card, "ROVER STATUS", 18, TextAnchor.MiddleLeft, HudTokens.Cyan, new Vector2(.04f, .86f), new Vector2(.7f, .98f), FontStyle.Bold);
            StatusExpand = Btn("Status expand", card, "ALL", new Vector2(1, 1), new Vector2(1, 1), new Vector2(-98, -94), new Vector2(-4, -4), () => { StatusExpanded = !StatusExpanded; LayoutSummary(); Refresh(); }, 16);
            HeaderButton(StatusExpand);
            statusBody = R("Status body", card, new Vector2(0, 0), new Vector2(1, .80f));
            foreach (var id in SummaryIds.Concat(AllIds.Except(SummaryIds)))
                SummaryWidgets.Add(Widget(statusBody, Status.Get(id), Vector2.zero, Vector2.one, true));
            LayoutSummary();
        }

        // Compact: 4 rows in one column. Expanded: all 10 widgets in 2 columns (wider card, same height band).
        void LayoutSummary()
        {
            int shown = StatusExpanded ? SummaryWidgets.Count : SummaryIds.Length, cols = StatusExpanded ? 2 : 1, rows = Mathf.CeilToInt(shown / (float)cols);
            for (int i = 0; i < SummaryWidgets.Count; i++)
            {
                var w = SummaryWidgets[i]; bool on = i < shown; w.gameObject.SetActive(on);
                if (!on) continue;
                int col = i / rows, row = i % rows;
                var r = (RectTransform)w.transform;
                r.anchorMin = new Vector2(col / (float)cols + .02f, 1 - (row + 1) / (float)rows + .01f); r.anchorMax = new Vector2((col + 1) / (float)cols - .02f, 1 - row / (float)rows - .01f);
                r.offsetMin = r.offsetMax = Vector2.zero;
            }
        }

        public HudStatusWidget Widget(Transform parent, IStatusSource src, Vector2 a, Vector2 b, bool compact)
        {
            var row = R("Status " + src.Id, parent, a, b);
            var w = row.gameObject.AddComponent<HudStatusWidget>(); w.source = src;
            w.icon = Img("Icon", row, new Vector2(.0f, .18f), new Vector2(.12f, .82f), null, HudTokens.TextDim); w.icon.preserveAspect = true; w.icon.raycastTarget = false;
            w.label = Tx("Label", row, "", compact ? 15 : 16, TextAnchor.UpperLeft, HudTokens.TextDim, new Vector2(.14f, .52f), new Vector2(.50f, .98f));
            w.tag = Tx("Source", row, "", 12, TextAnchor.UpperRight, HudTokens.TextDim, new Vector2(.50f, .56f), new Vector2(1f, .98f));
            w.value = Tx("Value", row, "", compact ? 22 : 24, TextAnchor.MiddleLeft, HudTokens.Text, new Vector2(.14f, .14f), new Vector2(.70f, .60f), FontStyle.Bold);
            w.unit = Tx("Unit", row, "", 14, TextAnchor.MiddleLeft, HudTokens.TextDim, new Vector2(.70f, .14f), new Vector2(1f, .52f));
            var track = Img("Track", row, new Vector2(.14f, .02f), new Vector2(1f, .10f), glass, new Color(1, 1, 1, .08f));
            w.fill = Img("Fill", track.transform, Vector2.zero, new Vector2(0, 1), glass, HudTokens.Cyan);
            w.Poll(true);
            return w;
        }

        // ---------- right: navigation ----------
        void BuildNav()
        {
            var card = Glass("Navigation", Safe, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-412, -572), new Vector2(-12, -128));
            NavWidget = card.gameObject;
            Icon("Compass icon", card, "compass", new Vector2(.03f, .92f), new Vector2(.09f, .985f), HudTokens.Cyan);
            Tx("Nav title", card, "NAVIGATION", 18, TextAnchor.MiddleLeft, HudTokens.Cyan, new Vector2(.11f, .92f), new Vector2(.7f, .99f), FontStyle.Bold);
            NavCollapse = Btn("Nav collapse", card, "–", new Vector2(1, 1), new Vector2(1, 1), new Vector2(-94, -94), new Vector2(-4, -4), () => { NavCollapsed = !NavCollapsed; }, 26);
            HeaderButton(NavCollapse);
            navBody = R("Nav body", card, Vector2.zero, new Vector2(1, .91f));
            // compass strip
            compassStrip = (RectTransform)Img("Compass strip", navBody, new Vector2(.04f, .88f), new Vector2(.96f, .99f), glass, new Color(1, 1, 1, .05f)).transform;
            compassStrip.gameObject.AddComponent<RectMask2D>();
            string[] marks = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
            for (int i = 0; i < 8; i++) compassMarks.Add((Tx("Mark " + marks[i], compassStrip, marks[i], i % 2 == 0 ? 20 : 15, TextAnchor.MiddleCenter, i == 0 ? HudTokens.Orange : HudTokens.TextDim, new Vector2(.5f, 0), new Vector2(.5f, 1), FontStyle.Bold), i * 45));
            foreach (var m in compassMarks) m.t.rectTransform.sizeDelta = new Vector2(48, 0);
            compassWaypoint = (RectTransform)Icon("Waypoint on strip", compassStrip, "pin", new Vector2(.5f, 0), new Vector2(.5f, 1), HudTokens.Orange).transform; compassWaypoint.sizeDelta = new Vector2(26, 0);
            Img("Heading tick", compassStrip, new Vector2(.5f, 0), new Vector2(.5f, .3f), null, HudTokens.Cyan).rectTransform.sizeDelta = new Vector2(3, 0);
            // minimap (square, masked to a rounded rect)
            var view = Img("Minimap viewport", navBody, new Vector2(.5f, .34f), new Vector2(.5f, .34f), glass, Color.white, false);
            view.rectTransform.anchorMin = view.rectTransform.anchorMax = new Vector2(.5f, .585f); view.rectTransform.sizeDelta = new Vector2(224, 224); view.rectTransform.anchoredPosition = Vector2.zero;
            view.gameObject.AddComponent<Mask>().showMaskGraphic = false;
            var raw = new GameObject("Minimap image", typeof(RectTransform), typeof(RawImage)).GetComponent<RawImage>();
            raw.transform.SetParent(view.transform, false); var rr = raw.rectTransform; rr.anchorMin = Vector2.zero; rr.anchorMax = Vector2.one; rr.offsetMin = rr.offsetMax = Vector2.zero;
            raw.color = new Color(1, 1, 1, .92f); raw.raycastTarget = false;
            Img("Map edge", view.transform, Vector2.zero, Vector2.one, glassEdge, HudTokens.Edge(.5f)).raycastTarget = false;
            Minimap = navBody.gameObject.AddComponent<HudMinimap>();
            Minimap.map = raw; Minimap.viewport = view.rectTransform;
            Minimap.rover = hud.game.prototype.rover.transform; Minimap.astronaut = hud.game.prototype.astronaut.transform;
            foreach (var st in mission.sensors.stations.Where(s => s))
            {
                Minimap.stations.Add(st.transform);
                var mk = Img("Station marker " + st.stableId, view.transform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), HudIcons.Get("disc"), HudTokens.Text); mk.rectTransform.sizeDelta = new Vector2(12, 12); mk.raycastTarget = false;
                Minimap.stationMarkers.Add(mk.rectTransform);
            }
            var wp = Icon("Waypoint marker", view.transform, "pin", new Vector2(.5f, .5f), new Vector2(.5f, .5f), HudTokens.Orange); wp.rectTransform.sizeDelta = new Vector2(30, 30);
            var astro = Img("Astronaut marker", view.transform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), HudIcons.Get("disc"), HudTokens.Green); astro.rectTransform.sizeDelta = new Vector2(14, 14); astro.raycastTarget = false;
            var rover = Icon("Rover marker", view.transform, "chevron", new Vector2(.5f, .5f), new Vector2(.5f, .5f), HudTokens.Cyan); rover.rectTransform.sizeDelta = new Vector2(30, 30);
            Minimap.focus = () => hud.game.prototype.ActivePosition(); Minimap.showAstronaut = () => !hud.game.prototype.driving;
            Minimap.waypointMarker = wp.rectTransform; Minimap.astronautMarker = astro.rectTransform; Minimap.roverMarker = rover.rectTransform;
            WaypointText = Tx("Waypoint", navBody, "", 17, TextAnchor.UpperLeft, HudTokens.Orange, new Vector2(.05f, .19f), new Vector2(.97f, .29f), FontStyle.Bold);
            TerrainText = Tx("Terrain", navBody, "", 15, TextAnchor.UpperLeft, HudTokens.Text, new Vector2(.05f, .075f), new Vector2(.97f, .19f));
            PositionText = Tx("Position", navBody, "", 13, TextAnchor.UpperLeft, HudTokens.TextDim, new Vector2(.05f, 0), new Vector2(.97f, .075f));
        }

        // ---------- bottom controls: telemetry-glass spec (2026-10-21) ----------
        // CSS spec -> uGUI: background rgba(10,20,30,.55) = HudFx.ControlFill; 1px rgba(0,210,255,.3) = hairline
        // sprites tinted HudFx.ControlBorder; 2px stroke icons = thin procedural icons drawn 1:1 (preserveAspect);
        // 44x44 px circular buttons = 116 reference units (44 dp on a 1080-px-tall ~2.6x Android screen).
        // backdrop-filter: blur(8px) is NOT applied: a live blur is a full-screen grab pass on mobile GPUs; the
        // 55 % dark fill gives the glass separation instead (documented limitation).
        const float CircleSize = 116;

        protected Image Hairline(Transform parent, int radius, Color c) { var h = Img("Hairline", parent, Vector2.zero, Vector2.one, radius >= 0 ? HudFx.Hairline(radius) : HudFx.CircleHairline, c); h.type = radius >= 0 ? Image.Type.Sliced : Image.Type.Simple; h.raycastTarget = false; return h; }

        protected Button CircleIconButton(string name, Transform parent, string icon, Vector2 anchor, Vector2 centre, Action click)
        {
            var b = Btn(name, parent, "", anchor, anchor, centre - Vector2.one * CircleSize / 2, centre + Vector2.one * CircleSize / 2, click, 1);
            var img = b.GetComponent<Image>(); img.sprite = HudFx.Circle; img.type = Image.Type.Simple; img.color = HudFx.ControlFill;
            var lbl = b.GetComponentInChildren<Text>(); if (lbl) lbl.gameObject.SetActive(false);   // icon-only
            Hairline(b.transform, -1, HudFx.ControlBorder);
            Icon("Icon", b.transform, icon, new Vector2(.3f, .3f), new Vector2(.7f, .7f), HudTokens.Cyan);
            var cb = b.colors; cb.pressedColor = new Color(.6f, 1.3f, 1.5f); b.colors = cb;
            b.gameObject.AddComponent<HudPressFx>().pressedScale = .92f;
            return b;
        }

        protected Button PillButton(string name, Transform parent, string icon, string label, Vector2 anchor, Vector2 min, Vector2 max, Action click, Color accent)
        {
            var b = Btn(name, parent, label, anchor, anchor, min, max, click, 18);
            var img = b.GetComponent<Image>(); img.sprite = HudFx.Round(30); img.type = Image.Type.Sliced; img.color = HudFx.ControlFill;
            Hairline(b.transform, 30, new Color(accent.r, accent.g, accent.b, .30f));
            var lbl = b.GetComponentInChildren<Text>(); lbl.color = HudTokens.Text; lbl.fontSize = 18; lbl.resizeTextMaxSize = 18; lbl.fontStyle = FontStyle.Bold;
            if (!string.IsNullOrEmpty(icon))
            {
                var ic = Icon("Icon", b.transform, icon, new Vector2(0, .5f), new Vector2(0, .5f), accent); ic.rectTransform.sizeDelta = new Vector2(40, 40); ic.rectTransform.anchoredPosition = new Vector2(38, 0);
                lbl.rectTransform.anchorMin = new Vector2(0, 0); lbl.rectTransform.offsetMin = new Vector2(64, 0); lbl.alignment = TextAnchor.MiddleLeft;
            }
            b.gameObject.AddComponent<HudPressFx>().pressedScale = .95f;
            return b;
        }

        void BuildAstronautControls()
        {
            AstronautControls = R("Astronaut controls", Safe, Vector2.zero, Vector2.one).gameObject;
            var t = AstronautControls.transform;
            MoveStick = Stick("Move joystick", t, "MOVE");
            StyleStick(MoveStick);
            CircleIconButton("Astronaut camera", t, "cam2", Vector2.zero, new Vector2(408, 244), () => { cameraRig.CycleAstronautView(); Say("Camera: " + cameraRig.ViewLabel); });
            CircleIconButton("Astronaut recenter", t, "eye2", Vector2.zero, new Vector2(408, 114), () => { cameraRig.Recenter(); Say("Camera recentred"); });
            Run = Hold("Run", t, "RUN", new Vector2(1, 0), new Vector2(-196, 48), new Vector2(-40, 204));
            StyleRound(Run, HudTokens.Cyan);
            InteractButton = PillButton("Interact", t, "target2", "INTERACT", new Vector2(1, 0), new Vector2(-520, 48), new Vector2(-216, 140), ContextInteract, HudTokens.Cyan);
        }

        void BuildDriveControls()
        {
            DriveControls = R("Drive controls", Safe, Vector2.zero, Vector2.one).gameObject;
            var t = DriveControls.transform;
            SteerStick = Stick("Steering joystick", t, "STEER");
            StyleStick(SteerStick);
            CircleIconButton("Camera distance", t, "cam2", Vector2.zero, new Vector2(408, 244), () => { cameraRig.CycleRoverView(); Say("Camera: " + cameraRig.ViewLabel); });
            CircleIconButton("Overview", t, "eye2", Vector2.zero, new Vector2(408, 114), () => { if (mission.modes) mission.modes.ToggleCinematic(); });

            // Floating semicircle gauge: no box, no fill, no brackets.
            var g = R("Speedometer", t, new Vector2(.5f, 0), new Vector2(.5f, 0)); g.offsetMin = new Vector2(-170, 20); g.offsetMax = new Vector2(170, 236);
            SpeedGauge = g.gameObject;
            Image ArcImg(string n, Sprite sp, Color c, float size)
            {
                var im = Img(n, g, new Vector2(.5f, 0), new Vector2(.5f, 0), sp, c); im.raycastTarget = false;
                im.rectTransform.sizeDelta = new Vector2(size, size); im.rectTransform.anchoredPosition = new Vector2(0, 66); return im;
            }
            ArcImg("Speed ticks", HudFx.ArcTicks(19, 3), new Color(1, 1, 1, .8f), 320);
            ArcImg("Speed arc track", HudFx.Arc(.86f, .90f), new Color(1, 1, 1, .14f), 320);
            var glowArc = ArcImg("Speed glow", HudFx.Arc(.82f, .96f), new Color(0, 210 / 255f, 1f, .16f), 320);
            speedArc = ArcImg("Speed arc", HudFx.Arc(.855f, .905f), HudTokens.Cyan, 320);
            foreach (var a in new[] { glowArc, speedArc }) { a.type = Image.Type.Filled; a.fillMethod = Image.FillMethod.Radial360; a.fillOrigin = (int)Image.Origin360.Left; a.fillClockwise = true; a.fillAmount = 0; }
            speedArc.gameObject.AddComponent<ArcGlowLink>().glow = glowArc;
            speedValue = Tx("Speed", g, "", 58, TextAnchor.MiddleCenter, HudTokens.Text, new Vector2(.2f, .50f), new Vector2(.8f, .86f), FontStyle.Bold);
            speedValue.resizeTextForBestFit = false;
            speedUnit = Tx("Speed unit", g, "km/h", 18, TextAnchor.MiddleCenter, new Color(1, 1, 1, .6f), new Vector2(.3f, .38f), new Vector2(.7f, .50f));
            var badge = Img("Gear badge", g, new Vector2(.5f, 0), new Vector2(.5f, 0), HudFx.Round(30), HudFx.ControlFill); badge.type = Image.Type.Sliced; badge.raycastTarget = false;
            badge.rectTransform.sizeDelta = new Vector2(150, 38); badge.rectTransform.anchoredPosition = new Vector2(0, 26);
            Hairline(badge.transform, 30, HudFx.ControlBorder);
            driveState = Tx("Drive state", badge.transform, "", 17, TextAnchor.MiddleCenter, HudTokens.Cyan, Vector2.zero, Vector2.one, FontStyle.Bold);
            var steerTrack = Img("Steering track", g, new Vector2(.5f, 0), new Vector2(.5f, 0), null, new Color(1, 1, 1, .14f)); steerTrack.raycastTarget = false;
            steerTrack.rectTransform.sizeDelta = new Vector2(240, 2); steerTrack.rectTransform.anchoredPosition = new Vector2(0, 58);
            steerMarker = Img("Steering marker", steerTrack.transform, new Vector2(.5f, -3f), new Vector2(.5f, 4f), HudFx.Round(30), HudTokens.Cyan); steerMarker.rectTransform.sizeDelta = new Vector2(10, 0); steerMarker.raycastTarget = false;

            // Right cluster: glass pedals, segmented gear pill, circular recenter, exit pill.
            Accelerate = Hold("Accelerate", t, "ACCEL", new Vector2(1, 0), new Vector2(-176, 48), new Vector2(-40, 340));
            StyleGlassPedal(Accelerate, HudTokens.Cyan, () => Throttle);
            Brake = Hold("Brake", t, "BRAKE", new Vector2(1, 0), new Vector2(-332, 48), new Vector2(-196, 244));
            StyleGlassPedal(Brake, HudFx.Amber, null);
            ReverseToggle = ToggleBox("Reverse", t, "", new Vector2(1, 0), new Vector2(-332, 252), new Vector2(-196, 342));
            StyleGearPill(ReverseToggle);
            CircleIconButton("Rover recenter", t, "recenter2", new Vector2(1, 0), new Vector2(-410, 106), () => { cameraRig.Recenter(); Say("Camera behind the rover"); });
            ExitButton = PillButton("Exit rover", t, "exit2", "EXIT ROVER", new Vector2(1, 0), new Vector2(-560, 180), new Vector2(-352, 272), () => hud.Interact(), HudTokens.Cyan);
            ScienceShortcut = PillButton("Science shortcut", t, "scan2", "SCIENCE AT STATION", new Vector2(.5f, 0), new Vector2(-170, 252), new Vector2(170, 340), () => Select(Tab.Science), HudTokens.Cyan);
        }

        // Joystick (25 % smaller): matte well, 1 px outer ring, subtle crosshair ticks, matte dark-cyan thumb.
        void StyleStick(VirtualStick s)
        {
            var r = (RectTransform)s.transform; r.offsetMin = new Vector2(56, 56); r.offsetMax = new Vector2(326, 326);
            var im = s.GetComponent<Image>(); im.sprite = HudFx.Circle; im.color = HudFx.ControlFill;
            var ring = Hairline(s.transform, -1, new Color(0, 210 / 255f, 1f, .35f));
            Img("Crosshair", s.transform, Vector2.zero, Vector2.one, HudFx.Crosshair, new Color(0, 210 / 255f, 1f, .32f)).raycastTarget = false;
            var knob = s.knob.GetComponent<Image>(); knob.sprite = HudFx.Circle; knob.color = new Color(0f, .30f, .36f, .92f);
            s.knob.sizeDelta = new Vector2(98, 98);
            Img("Thumb hairline", s.knob, Vector2.zero, Vector2.one, HudFx.CircleHairline, new Color(0, 210 / 255f, 1f, .55f)).raycastTarget = false;
            s.knob.SetAsLastSibling();
            var lbl = s.GetComponentsInChildren<Text>().FirstOrDefault(); if (lbl) { lbl.color = new Color(1, 1, 1, .45f); lbl.fontSize = 14; lbl.resizeTextMaxSize = 14; }
            var fx = s.gameObject.AddComponent<HudMinimalStickFx>(); fx.stick = s; fx.ring = ring;
        }

        // Round glass hold button (astronaut RUN).
        void StyleRound(HoldControl h, Color c)
        {
            var im = h.GetComponent<Image>(); im.sprite = HudFx.Circle; im.color = HudFx.ControlFill;
            var ring = Hairline(h.transform, -1, new Color(c.r, c.g, c.b, .3f));
            var glow = Img("Press glow", h.transform, Vector2.zero, Vector2.one, HudFx.Glow, new Color(c.r, c.g, c.b, 0)); glow.raycastTarget = false; glow.rectTransform.offsetMin = new Vector2(-26, -26); glow.rectTransform.offsetMax = new Vector2(26, 26); glow.transform.SetAsFirstSibling();
            var t = h.GetComponentInChildren<Text>(); t.color = c; t.fontSize = 22; t.resizeTextMaxSize = 22; t.transform.SetAsLastSibling();
            var fx = h.gameObject.AddComponent<HudGlassPedalFx>(); fx.hold = h; fx.body = im; fx.border = ring; fx.glow = glow; fx.accent = c;
        }

        // Vertical glass pedal: 55 % dark fill, hairline border, horizontal grip lines, slim level bar;
        // cyan (ACCEL) or amber #FF8C00 (BRAKE) outline + soft glow only while pressed.
        void StyleGlassPedal(HoldControl h, Color c, Func<float> value)
        {
            var im = h.GetComponent<Image>(); im.sprite = HudFx.Round(22); im.type = Image.Type.Sliced; im.color = HudFx.ControlFill;
            var glow = Img("Press glow", h.transform, Vector2.zero, Vector2.one, HudFx.Glow, new Color(c.r, c.g, c.b, 0)); glow.raycastTarget = false; glow.rectTransform.offsetMin = new Vector2(-34, -34); glow.rectTransform.offsetMax = new Vector2(34, 34); glow.transform.SetAsFirstSibling();
            var border = Hairline(h.transform, 22, new Color(c.r, c.g, c.b, .3f));
            for (int i = 0; i < 7; i++)
            {
                float y = .30f + i * .07f;
                var grip = Img("Grip", h.transform, new Vector2(.30f, y), new Vector2(.70f, y), HudFx.Round(30), new Color(1, 1, 1, .16f)); grip.rectTransform.sizeDelta = new Vector2(0, 3); grip.raycastTarget = false;
            }
            var track = Img("Level track", h.transform, new Vector2(0, .08f), new Vector2(0, .92f), HudFx.Round(30), new Color(1, 1, 1, .08f)); track.rectTransform.sizeDelta = new Vector2(4, 0); track.rectTransform.anchoredPosition = new Vector2(12, 0); track.raycastTarget = false;
            var level = Img("Level", h.transform, new Vector2(0, .08f), new Vector2(0, .08f), HudFx.Round(30), c); level.rectTransform.sizeDelta = new Vector2(4, 0); level.rectTransform.anchoredPosition = new Vector2(12, 0); level.raycastTarget = false;
            var t = h.GetComponentInChildren<Text>(); t.color = c; t.fontSize = 18; t.resizeTextMaxSize = 18; t.fontStyle = FontStyle.Bold;
            t.rectTransform.anchorMin = new Vector2(0, .05f); t.rectTransform.anchorMax = new Vector2(1, .22f); t.transform.SetAsLastSibling();
            var fx = h.gameObject.AddComponent<HudGlassPedalFx>(); fx.hold = h; fx.body = im; fx.border = border; fx.glow = glow; fx.level = level; fx.value = value; fx.accent = c;
        }

        // Segmented D | R pill toggle (isOn = reverse).
        void StyleGearPill(Toggle tg)
        {
            var im = tg.GetComponent<Image>(); im.sprite = HudFx.Round(30); im.type = Image.Type.Sliced; im.color = HudFx.ControlFill;
            if (tg.graphic) { tg.graphic.gameObject.SetActive(false); tg.graphic = null; }
            foreach (var old in tg.GetComponentsInChildren<Text>()) old.gameObject.SetActive(false);
            var d = Img("D segment", tg.transform, new Vector2(0, 0), new Vector2(.5f, 1), HudFx.Round(30), Color.clear); d.type = Image.Type.Sliced; d.raycastTarget = false; d.rectTransform.offsetMin = new Vector2(5, 5); d.rectTransform.offsetMax = new Vector2(-2, -5);
            var r = Img("R segment", tg.transform, new Vector2(.5f, 0), new Vector2(1, 1), HudFx.Round(30), Color.clear); r.type = Image.Type.Sliced; r.raycastTarget = false; r.rectTransform.offsetMin = new Vector2(2, 5); r.rectTransform.offsetMax = new Vector2(-5, -5);
            Img("Divider", tg.transform, new Vector2(.5f, .25f), new Vector2(.5f, .75f), null, HudFx.ControlBorder).rectTransform.sizeDelta = new Vector2(1, 0);
            Hairline(tg.transform, 30, HudFx.ControlBorder);
            var dt = Tx("D", d.transform, "D", 24, TextAnchor.MiddleCenter, HudTokens.Cyan, Vector2.zero, Vector2.one, FontStyle.Bold);
            var rt = Tx("R", r.transform, "R", 24, TextAnchor.MiddleCenter, HudFx.Amber, Vector2.zero, Vector2.one, FontStyle.Bold);
            var fx = tg.gameObject.AddComponent<HudGearPillFx>(); fx.toggle = tg; fx.dSeg = d; fx.rSeg = r; fx.dText = dt; fx.rText = rt;
            tg.gameObject.AddComponent<HudPressFx>().pressedScale = .96f;
        }

        // ---------- mode panels ----------
        void BuildModePanels()
        {
            var host = R("Tab panel", Safe, Vector2.zero, Vector2.one);
            panelGroup = host.gameObject.AddComponent<CanvasGroup>();
            Panels[Tab.Science] = BuildScience(host);
            Panels[Tab.Data] = BuildData(host);
            Panels[Tab.Mission] = BuildMissionPanel(host);
            Panels[Tab.Systems] = BuildSystems(host);
        }

        GameObject BuildScience(Transform host)
        {
            var p = Glass("Science panel", host, new Vector2(1, 0), new Vector2(1, 1), new Vector2(-700, 16), new Vector2(-12, -128));
            Tx("Title", p, "SCIENCE • ROVER INSTRUMENTS", 22, TextAnchor.MiddleLeft, HudTokens.Cyan, new Vector2(.04f, .93f), new Vector2(.96f, .99f), FontStyle.Bold);
            var list = R("Instrument list", p, new Vector2(.03f, .52f), new Vector2(.97f, .92f));
            int n = mission.Catalog.Instruments.Count, i = 0;
            foreach (var inst in mission.Catalog.Instruments)
            {
                var id = inst.id; int row = i / 2, col = i % 2; float rows = Mathf.Ceil(n / 2f);
                var b = Btn("Instrument " + id, list, (inst.available ? "" : "UNAVAILABLE • ") + inst.name.Replace(" (hazard cameras)", ""), new Vector2(col * .5f, 1 - (row + 1) / rows), new Vector2(col * .5f + .5f, 1 - row / rows), new Vector2(4, 4), new Vector2(-4, -4), () => SelectInstrument(id), 18);
                b.GetComponent<Image>().sprite = glass;
                b.interactable = true;
                sciInstrumentButtons[id] = b; instrumentButtons[id] = b; i++;
            }
            Icon("Target icon", p, "target", new Vector2(.04f, .43f), new Vector2(.09f, .50f), HudTokens.Orange);
            scienceTarget = Tx("Target", p, "", 19, TextAnchor.MiddleLeft, HudTokens.Text, new Vector2(.10f, .43f), new Vector2(.66f, .50f), FontStyle.Bold);
            TargetButton = Btn("Select target", p, "TARGET", new Vector2(.68f, .42f), new Vector2(.97f, .51f), Vector2.zero, Vector2.zero, () =>
            {
                var tg = mission.sensors.CycleTarget(25); Say(tg ? "Target: " + tg.displayName : "No research station within 25 m");
            }, 20);
            var rtrack = Img("Range track", p, new Vector2(.04f, .385f), new Vector2(.97f, .405f), glass, new Color(1, 1, 1, .10f));
            rangeBar = Img("Fill", rtrack.transform, Vector2.zero, new Vector2(0, 1), glass, HudTokens.Green);
            scienceRange = Tx("Range status", p, "", 17, TextAnchor.MiddleLeft, HudTokens.TextDim, new Vector2(.04f, .33f), new Vector2(.97f, .38f));
            ObserveButton = Btn("Observe", p, "", new Vector2(.04f, .19f), new Vector2(.70f, .32f), Vector2.zero, Vector2.zero, () => mission.BeginObservation(mission.Catalog.Instrument(SelectedInstrument)), 24);
            ObserveButton.GetComponent<Image>().sprite = glass; ObserveButton.GetComponent<Image>().color = new Color(0, .45f, .55f, .85f);
            ScanButton = ObserveButton;
            scanFill = Img("Scan fill", ObserveButton.transform, Vector2.zero, new Vector2(0, 1), glass, new Color(HudTokens.Cyan.r, HudTokens.Cyan.g, HudTokens.Cyan.b, .35f)); scanFill.raycastTarget = false;
            scanLabel = ObserveButton.GetComponentInChildren<Text>(); scanLabel.transform.SetAsLastSibling();
            observeFill = scanFill;
            CancelButton = Btn("Cancel observation", p, "CANCEL", new Vector2(.72f, .19f), new Vector2(.97f, .32f), Vector2.zero, Vector2.zero, () => { if (mission.Observing != null) mission.Cancel("Observation cancelled by the player"); }, 20);
            ScienceResultText = Tx("Scan result", p, "", 16, TextAnchor.UpperLeft, HudTokens.Text, new Vector2(.04f, .015f), new Vector2(.97f, .18f));
            return p.gameObject;
        }

        void SelectInstrument(string id)
        {
            var inst = mission.Catalog.Instrument(id);
            if (!inst.available) { Say("UNAVAILABLE • " + inst.unavailableReason); return; }
            SelectedInstrument = id;
            Refresh();
        }

        static readonly string[] DataCols = { "TIME", "SENSOR", "STATION", "TYPE", "VALUE", "UNIT", "SOURCE", "QUEUE", "BACKEND", "AI" };
        static readonly float[] DataColX = { 0, .08f, .26f, .34f, .45f, .58f, .65f, .76f, .86f, .94f, 1 };

        GameObject BuildData(Transform host)
        {
            var p = Glass("Data panel", host, new Vector2(0, 0), new Vector2(1, 1), new Vector2(12, 16), new Vector2(-12, -128));
            Tx("Title", p, "DATA • SCIENTIFIC RECORDS", 22, TextAnchor.MiddleLeft, HudTokens.Cyan, new Vector2(.02f, .93f), new Vector2(.5f, .99f), FontStyle.Bold);
            RetryButton = Btn("Retry sync", p, "RETRY SYNC", new Vector2(.84f, .905f), new Vector2(.985f, .995f), Vector2.zero, Vector2.zero, () => mission.Sync.RetryNow(), 18);
            DataSummaryText = Tx("Data body", p, "", 18, TextAnchor.MiddleLeft, HudTokens.Text, new Vector2(.02f, .83f), new Vector2(.98f, .92f));
            var head = R("Table header", p, new Vector2(.02f, .76f), new Vector2(.98f, .82f));
            for (int c = 0; c < DataCols.Length; c++) Tx("H " + DataCols[c], head, DataCols[c], 14, TextAnchor.MiddleLeft, HudTokens.Cyan, new Vector2(DataColX[c], 0), new Vector2(DataColX[c + 1], 1), FontStyle.Bold);
            var body = R("Table rows", p, new Vector2(.02f, .10f), new Vector2(.98f, .76f));
            const int rows = 10;
            for (int r = 0; r < rows; r++)
            {
                var row = R("Row " + r, body, new Vector2(0, 1 - (r + 1) / (float)rows), new Vector2(1, 1 - r / (float)rows));
                if (r % 2 == 0) Img("Stripe", row, Vector2.zero, Vector2.one, null, new Color(1, 1, 1, .03f));
                var cells = new Text[DataCols.Length];
                for (int c = 0; c < DataCols.Length; c++) cells[c] = Tx(DataCols[c], row, "", 14, TextAnchor.MiddleLeft, HudTokens.Text, new Vector2(DataColX[c], 0), new Vector2(DataColX[c + 1], 1));
                DataRows.Add(cells);
            }
            DataPrev = Btn("Data newer", p, "NEWER", new Vector2(.70f, .015f), new Vector2(.84f, .09f), Vector2.zero, Vector2.zero, () => { DataPage = Mathf.Max(0, DataPage - 1); Refresh(); }, 18);
            DataNext = Btn("Data older", p, "OLDER", new Vector2(.85f, .015f), new Vector2(.985f, .09f), Vector2.zero, Vector2.zero, () => { DataPage++; Refresh(); }, 18);
            Tx("AI note", p, "AI ANALYSIS: NOT CONNECTED — the team's scientific model is not available yet; no results are shown or invented.", 15, TextAnchor.MiddleLeft, HudTokens.Amber, new Vector2(.02f, .015f), new Vector2(.68f, .09f));
            return p.gameObject;
        }

        GameObject BuildMissionPanel(Transform host)
        {
            var p = Glass("Mission panel", host, new Vector2(0, 0), new Vector2(1, 1), new Vector2(160, 16), new Vector2(-160, -128));
            Tx("Title", p, "MISSION • RED HORIZON SURFACE OPERATIONS  (ag.mars.red-horizon.surface-ops v1)", 20, TextAnchor.MiddleLeft, HudTokens.Orange, new Vector2(.03f, .93f), new Vector2(.97f, .99f), FontStyle.Bold);
            var list = R("Objectives", p, new Vector2(.03f, .26f), new Vector2(.97f, .91f));
            const int max = 9;
            for (int i = 0; i < max; i++)
            {
                var row = R("Objective " + i, list, new Vector2(0, 1 - (i + 1) / (float)max), new Vector2(1, 1 - i / (float)max));
                var bg = Img("Highlight", row, Vector2.zero, Vector2.one, glass, new Color(0, 0, 0, 0)); bg.raycastTarget = false;
                objectiveDots.Add(Img("State", row, new Vector2(.005f, .25f), new Vector2(.025f, .75f), HudIcons.Get("disc"), HudTokens.TextDim));
                objectiveTexts.Add(Tx("Text", row, "", 19, TextAnchor.MiddleLeft, HudTokens.Text, new Vector2(.04f, 0), new Vector2(1, 1)));
                objectiveRows.Add(row);
            }
            missionText = Tx("Mission body", p, "", 17, TextAnchor.UpperLeft, HudTokens.Text, new Vector2(.03f, .02f), new Vector2(.97f, .25f));
            return p.gameObject;
        }

        GameObject BuildSystems(Transform host)
        {
            var p = Glass("Systems panel", host, new Vector2(1, 0), new Vector2(1, 1), new Vector2(-900, 16), new Vector2(-12, -128));
            Tx("Title", p, "SYSTEMS • ROVER AND LINK STATUS", 22, TextAnchor.MiddleLeft, HudTokens.Cyan, new Vector2(.03f, .93f), new Vector2(.7f, .99f), FontStyle.Bold);
            SettingsButton = Btn("Settings", p, "SETTINGS", new Vector2(.74f, .90f), new Vector2(.97f, .995f), Vector2.zero, Vector2.zero, () => { SettingsOpen = !SettingsOpen; Refresh(); }, 18);
            systemsWidgetsHost = R("Status widgets", p, new Vector2(.03f, .03f), new Vector2(.97f, .90f));
            for (int i = 0; i < AllIds.Length; i++)
            {
                int row = i / 2, col = i % 2;
                SystemWidgets.Add(Widget(systemsWidgetsHost, Status.Get(AllIds[i]), new Vector2(col * .5f + .01f, 1 - (row + 1) / 5f + .01f), new Vector2(col * .5f + .49f, 1 - row / 5f - .01f), false));
            }
            settingsHost = R("Settings grid", p, new Vector2(.03f, .03f), new Vector2(.97f, .90f));
            settingsGrid = settingsHost.gameObject;
            BuildSettingsGrid(settingsHost, settingLabels, "Setting ", 18);
            systemsText = Tx("Systems body", p, "", 1, TextAnchor.UpperLeft, new Color(0, 0, 0, 0), Vector2.zero, Vector2.zero); // kept for API parity (not shown)
            return p.gameObject;
        }

        // ---------- overlays: pause (with settings), alerts, on-foot scan ----------
        void BuildOverlays()
        {
            var fs = Glass("On-foot scan progress", Safe, new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(-300, 40), new Vector2(300, 120), .8f);
            footScanText = Tx("Scan text", fs, "", 22, TextAnchor.MiddleCenter, HudTokens.Cyan, Vector2.zero, Vector2.one);
            fs.gameObject.SetActive(false);

            var banner = Glass("Alert banner", Safe, new Vector2(.5f, 1), new Vector2(.5f, 1), new Vector2(-420, -250), new Vector2(420, -180), .86f);
            AlertBanner = banner.gameObject;
            alertIcon = Icon("Alert icon", banner, "warning", new Vector2(.02f, .15f), new Vector2(.09f, .85f), HudTokens.Red);
            alertText = Tx("Alert text", banner, "", 20, TextAnchor.MiddleLeft, HudTokens.Text, new Vector2(.11f, .05f), new Vector2(.98f, .95f), FontStyle.Bold);
            AlertBanner.SetActive(false);

            var shade = Img("Paused overlay", Safe, Vector2.zero, Vector2.one, null, new Color(HudTokens.Background.r, HudTokens.Background.g, HudTokens.Background.b, .72f), true);
            PausedOverlay = shade.gameObject;
            var card = Glass("Paused card", shade.transform, new Vector2(.5f, .5f), new Vector2(.5f, .5f), new Vector2(-620, -420), new Vector2(620, 420), .9f);
            pausedText = Tx("Paused text", card, "", 26, TextAnchor.MiddleLeft, HudTokens.Text, new Vector2(.04f, .88f), new Vector2(.6f, .98f), FontStyle.Bold);
            Btn("Resume", card, "RESUME", new Vector2(.66f, .88f), new Vector2(.96f, .98f), Vector2.zero, Vector2.zero, () => { if (hud.modalOpen) hud.CloseModal(); else if (hud.environment.paused) hud.TogglePause(); }, 24);
            var grid = R("Pause settings", card, new Vector2(.03f, .03f), new Vector2(.97f, .86f));
            BuildSettingsGrid(grid, pauseSettingLabels, "Pause setting ", 18);
            PausedOverlay.SetActive(false);
        }

        // ======================= RUNTIME =======================
        public Transform CurrentWaypoint()
        {
            var obj = mission.Objectives();
            bool siteDone = obj.Where(o => !o.info).Take(6).All(o => o.done);
            if (!siteDone || !mission.Designated) return mission.Designated ? mission.Designated.transform : null;
            var rover = hud.game.prototype.rover.transform.position;
            var next = mission.sensors.stations.Where(s => s && s != mission.Designated).OrderBy(s => RoverSensorSuite.Planar(s.transform.position, rover)).FirstOrDefault();
            return next ? next.transform : mission.Designated.transform;
        }

        protected virtual bool ShowClassicPanels => true;   // the Explorer HUD hides the big panels
        protected static void SetActiveIfChanged(GameObject g, bool on) { if (g && g.activeSelf != on) g.SetActive(on); }

        protected virtual void LateUpdate()
        {
            if (!built) return;
            float dt = Time.unscaledDeltaTime; bool reduced = mission.Settings.reducedMotion;
            var p = hud.game.prototype; bool driving = p.driving;
            bool paused = hud.environment.paused || hud.modalOpen;
            var state = control ? control.State : ControlState.Astronaut;
            Status.Tick(Time.deltaTime);

            // Smart visibility: what each control state shows.
            bool driveView = state == ControlState.RoverDrive, astronaut = state == ControlState.Astronaut;
            bool classic = ShowClassicPanels;
            SetActiveIfChanged(TopBar, classic && !paused);
            SetActiveIfChanged(MissionCard, classic && !paused && (driveView || astronaut || state == ControlState.RoverScience));
            SetActiveIfChanged(StatusSummary, classic && !paused && (driveView || astronaut));
            SetActiveIfChanged(NavWidget, classic && !paused && (driveView || astronaut));
            if (!driving) { modeText.text = "ASTRONAUT • EVA"; }

            // Collapse / expand animation (height lerp).
            float k = reduced ? 1 : 1 - Mathf.Exp(-14 * dt);
            missionH = Mathf.Lerp(missionH, MissionCollapsed ? 0 : 1, k);
            missionBody.localScale = new Vector3(1, Mathf.Max(.001f, missionH), 1); missionBody.gameObject.SetActive(missionH > .02f);
            ((RectTransform)MissionCard.transform).offsetMin = new Vector2(12, Mathf.Lerp(-182, -342, missionH));
            MissionCollapse.GetComponentInChildren<Text>().text = MissionCollapsed ? "+" : "–";
            navH = Mathf.Lerp(navH, NavCollapsed ? 0 : 1, k);
            navBody.localScale = new Vector3(1, Mathf.Max(.001f, navH), 1); navBody.gameObject.SetActive(navH > .02f);
            ((RectTransform)NavWidget.transform).offsetMin = new Vector2(-412, Mathf.Lerp(-184, -572, navH));
            NavCollapse.GetComponentInChildren<Text>().text = NavCollapsed ? "+" : "–";
            statusH = Mathf.Lerp(statusH, StatusExpanded ? 1 : 0, k);
            var sr = (RectTransform)StatusSummary.transform;
            float top = MissionCollapsed ? -194 : -354;
            sr.offsetMax = new Vector2(Mathf.Lerp(452, 900, statusH), Mathf.Lerp(sr.offsetMax.y, top, k));
            sr.offsetMin = new Vector2(12, Mathf.Lerp(sr.offsetMin.y, top - Mathf.Lerp(210, 360, statusH), k));

            // Tab bar: centred in the space between the branding and the right status cluster, at most 900 wide.
            var bar = (RectTransform)TopBar.transform; var tb = (RectTransform)TabBar.transform;
            float avail = bar.rect.width - 400 - 600, wTabs = Mathf.Min(900, Mathf.Max(500, avail));
            float left = 400 + (avail - wTabs) / 2;
            tb.offsetMin = new Vector2(left, 6); tb.offsetMax = new Vector2(-(bar.rect.width - left - wTabs), -6);
            StatusExpand.GetComponentInChildren<Text>().text = StatusExpanded ? "LESS" : "ALL";

            foreach (var kv in TabButtons) { var lt = kv.Value.GetComponentInChildren<Text>(); lt.color = kv.Key == Current ? HudTokens.Text : HudTokens.TextDim; }
            HudFx.ReducedMotion = () => mission.Settings.reducedMotion;
            // Speedometer (every frame, smooth).
            if (driving && SpeedGauge.activeInHierarchy)
            {
                float target = Mathf.Clamp01(Mathf.Abs(p.speed) / 3.8f) * .5f;   // semicircle = half of Radial360
                speedArc.fillAmount = Mathf.Lerp(speedArc.fillAmount, target, reduced ? 1 : 1 - Mathf.Exp(-10 * dt));
                speedArc.color = p.speed < -.05f ? HudFx.Amber : HudTokens.Cyan;
                float steer = Mathf.Clamp(p.testMove.x, -1, 1);
                var sm = steerMarker.rectTransform; sm.anchorMin = new Vector2(.5f + steer * .5f, -3); sm.anchorMax = new Vector2(.5f + steer * .5f, 4);
            }

            // Compass strip and map waypoint.
            Waypoint = CurrentWaypoint();
            Minimap.waypoint = Waypoint;
            if (NavWidget.activeInHierarchy && navBody.gameObject.activeSelf)
            {
                float hdg = driving ? p.rover.transform.eulerAngles.y : p.yaw;
                float w = compassStrip.rect.width;
                foreach (var (t, deg) in compassMarks)
                {
                    float d = Mathf.DeltaAngle(hdg, deg);
                    t.gameObject.SetActive(Mathf.Abs(d) < 75);
                    t.rectTransform.anchoredPosition = new Vector2(d / 150f * w, 0);
                }
                if (Waypoint)
                {
                    float d = Mathf.DeltaAngle(hdg, RoverSensorSuite.Bearing(p.ActivePosition(), Waypoint.position));
                    compassWaypoint.anchoredPosition = new Vector2(Mathf.Clamp(d, -72, 72) / 150f * w, 0);
                }
            }

            // Objective completion pulse + active highlight.
            for (int i = 0; i < objectiveRows.Count; i++)
            {
                pulse[i] = Mathf.MoveTowards(pulse[i], 0, dt * 1.5f);
                objectiveRows[i].localScale = Vector3.one * (1 + (reduced ? 0 : pulse[i] * .03f));
            }

            // Alert banner timing.
            if (AlertBanner.activeSelf && Time.unscaledTime > alertUntil) AlertBanner.SetActive(false);
            if (scanFill) scanFill.rectTransform.anchorMax = new Vector2(mission.ObservationProgress, 1);
        }

        protected override void Refresh()
        {
            var g = hud.game; var p = g.prototype; bool driving = p.driving;
            var state = control ? control.State : ControlState.Astronaut;

            // top bar
            modeText.text = RoverStatusProvider.ModeName(state);
            sessionText.text = "SESSION " + TimeSpan.FromSeconds(p.elapsed).ToString(@"mm\:ss");
            var link = Status.Get("comms").Read();
            linkText.text = link.text; linkText.color = link.Available ? HudTokens.Green : HudTokens.Amber;
            linkIcon.sprite = HudIcons.Get(link.Available ? "signal" : "nosignal"); linkIcon.color = linkText.color;
            var pw = Status.Get("power").Read();
            powerText.text = $"{pw.text}%\nSIM"; powerText.color = HudTokens.ForLevel(pw.level);

            // mission card
            var obj = mission.Objectives(); var real = obj.Where(o => !o.info).ToList(); int done = real.Count(o => o.done);
            missionObjective.text = mission.Instructions();
            missionBar.rectTransform.anchorMax = new Vector2(done / (float)real.Count, 1);
            var sync = RoverStatusProvider.SyncSummary(mission);
            missionProgressText.text = $"{done}/{real.Count} objectives • local, not scored • data {sync.label}";

            // context controls
            bool nearRover = Vector3.Distance(p.ActivePosition(), p.rover.transform.position) <= 5;
            var footTarget = driving ? null : hud.CurrentTarget();
            interactKind = driving ? "" : nearRover ? "board" : footTarget ? "scan" : NearAirlock(p.ActivePosition()) ? "inspect" : "";
            InteractButton.gameObject.SetActive(interactKind != "");
            InteractButton.GetComponentInChildren<Text>().text = interactKind == "board" ? "BOARD ROVER" : interactKind == "scan" ? "SCAN " + footTarget.displayName.ToUpperInvariant() : "INSPECT AIRLOCK";
            ExitButton.gameObject.SetActive(driving && Mathf.Abs(p.speed) <= .2f);
            ExitButton.interactable = driving && Mathf.Abs(p.speed) <= .2f;
            ScienceShortcut.gameObject.SetActive(driving && Mathf.Abs(p.speed) < .2f && mission.sensors.NearestStation(out float nd) && nd <= 15);

            // speedometer text
            float kmh = Mathf.Abs(p.speed) * 3.6f;
            speedValue.text = kmh.ToString("0.0", CultureInfo.InvariantCulture);
            bool rev = ReverseToggle && ReverseToggle.isOn;
            driveState.text = p.speed < -.05f ? "R  REVERSE" : p.speed > .05f ? "D  DRIVE" : rev ? "R  READY" : "N  NEUTRAL";
            driveState.color = p.speed < -.05f || rev ? HudFx.Amber : p.speed > .05f ? HudTokens.Cyan : new Color(1, 1, 1, .7f);

            // navigation text
            var wp = CurrentWaypoint();
            if (wp)
            {
                var st = wp.GetComponent<ResearchTarget>();
                float dist = RoverSensorSuite.Planar(p.ActivePosition(), wp.position), brg = RoverSensorSuite.Bearing(p.ActivePosition(), wp.position);
                WaypointText.text = $"{(st ? st.displayName.ToUpperInvariant() : wp.name)} • {dist:0} m • BRG {brg:000}°";
            }
            else WaypointText.text = "WAYPOINT —";
            bool terrainOk = mission.sensors.Terrain(out float slope, out float rough);
            TerrainText.text = terrainOk ? $"TERRAIN slope {slope:0}° • roughness {rough:0.00} m • {mission.sensors.Traversability(slope, rough).ToUpperInvariant()} (simulated)" : "TERRAIN not available here";
            TerrainText.color = terrainOk && slope >= mission.Catalog.HazardSlope ? HudTokens.Red : HudTokens.Text;
            var pos = p.ActivePosition();
            PositionText.text = $"POS x {pos.x:0.0} z {pos.z:0.0} y {pos.y:0.0} m • scene frame, not GPS";

            // alerts (critical only, transient)
            string alert = null, key = "";
            if (pw.level == StatusLevel.Critical) { alert = $"ROVER POWER LOW • {pw.text}% (simulated)"; key = "power"; }
            else if (Status.Get("storage").Read().level == StatusLevel.Critical) { alert = "LOCAL STORAGE FULL • records waiting for sync"; key = "storage"; }
            else if (sync.failed > 0) { alert = $"SYNC FAILED • {sync.failed} record(s) • open DATA to retry"; key = "sync" + sync.failed; }
            else if (driving && terrainOk && slope >= mission.Catalog.HazardSlope) { alert = $"TERRAIN HAZARD • slope {slope:0}°"; key = "terrain"; }
            CurrentAlert = alert ?? "";
            if (alert != null && key != lastAlertKey) { alertText.text = alert; AlertBanner.SetActive(true); alertUntil = Time.unscaledTime + 4; }
            lastAlertKey = key;

            if (!driving) return;
            switch (Current)
            {
                case Tab.Science: RefreshSciencePanel(); break;
                case Tab.Data: RefreshDataPanel(); break;
                case Tab.Mission: RefreshMissionPanel(obj); break;
                case Tab.Systems: RefreshSystemsPanel(); break;
            }
            // pause settings labels stay in sync too
            UpdateSettingLabels(pauseSettingLabels);
            pausedText.text = "PAUSED • SETTINGS";
        }

        void RefreshSciencePanel()
        {
            var inst = mission.Catalog.Instrument(SelectedInstrument);
            foreach (var kv in sciInstrumentButtons)
            {
                var def = mission.Catalog.Instrument(kv.Key);
                kv.Value.GetComponent<Image>().color = kv.Key == SelectedInstrument ? new Color(0, .42f, .52f, .92f) : HudTokens.PanelGlass(def.available ? .85f : .45f);
                kv.Value.GetComponentInChildren<Text>().color = def.available ? HudTokens.Text : HudTokens.TextDim;
            }
            var station = mission.sensors.Target(out float range);
            scienceTarget.text = station ? $"{(mission.sensors.selected ? "TARGET" : "NEAREST")}: {station.displayName.ToUpperInvariant()} ({station.stableId})" : "NO RESEARCH STATION";
            string reason = mission.Check(inst);
            float max = inst.maxRange > 0 ? inst.maxRange : 0;
            rangeBar.rectTransform.anchorMax = new Vector2(max > 0 && station ? Mathf.Clamp01(1 - Mathf.Max(0, range - max) / 25f) : 1, 1);
            bool inRange = max == 0 || (station && range <= max);
            rangeBar.color = inRange ? HudTokens.Green : HudTokens.Amber;
            scienceRange.text = max > 0 ? $"RANGE {(station ? range.ToString("0.0") : "—")} m / max {max:0} m • {(inRange ? "IN RANGE" : "OUT OF RANGE")}" + (inst.requiresStopped ? " • rover must be stopped" : "") : "Measures at the rover (no range limit)";
            CancelButton.interactable = mission.Observing != null;
            ObserveButton.interactable = reason == null;
            scanLabel.text = mission.Observing != null ? $"SCANNING {Mathf.RoundToInt(mission.ObservationProgress * 100)}%" : reason == null ? "SCAN" : "SCAN UNAVAILABLE";
            var last = mission.LastObservation;
            var b = new StringBuilder();
            if (reason != null && mission.Observing == null) b.AppendLine(reason);
            b.AppendLine(mission.LastFeedback);
            if (last.Count > 0)
            {
                foreach (var r in last.Where(r => r.sensorId != "nav.position").Take(3))
                    b.Append($"{r.sensorId} = {r.ValueLabel} [{r.provenance}]   ");
                b.AppendLine();
                b.Append($"Collection: stored on this device • {RoverStatusProvider.SyncLabel(last[0], mission.Sync.Uploader.Online)} • AI NOT CONNECTED");
            }
            ScienceResultText.text = b.ToString();
            ScienceResultText.color = reason != null && mission.Observing == null ? HudTokens.Amber : HudTokens.Text;
        }

        void RefreshDataPanel()
        {
            var recs = mission.Store.Records; bool online = mission.Sync.Uploader.Online;
            int local = 0, queued = 0, syncing = 0, synced = 0, failed = 0;
            foreach (var r in recs)
                switch (RoverStatusProvider.SyncLabel(r, online)) { case "SYNCED": synced++; break; case "SYNCING": syncing++; break; case "FAILED": failed++; break; case "QUEUED": queued++; break; default: local++; break; }
            DataSummaryText.text = $"{recs.Count} records   LOCAL ONLY {local}   QUEUED {queued}   SYNCING {syncing}   SYNCED {synced}   FAILED {failed}   •   backend: {(online ? "connected" : "OFFLINE / NOT CONNECTED")}   •   AI: NOT CONNECTED";
            int pages = Mathf.Max(1, Mathf.CeilToInt(recs.Count / (float)DataRows.Count));
            DataPage = Mathf.Clamp(DataPage, 0, pages - 1);
            var view = recs.Reverse().Skip(DataPage * DataRows.Count).Take(DataRows.Count).ToList();
            for (int i = 0; i < DataRows.Count; i++)
            {
                var c = DataRows[i];
                if (i >= view.Count) { foreach (var t in c) t.text = ""; continue; }
                var r = view[i];
                string sync = RoverStatusProvider.SyncLabel(r, online);
                c[0].text = Local(r.capturedAtUtc); c[1].text = r.sensorId; c[2].text = string.IsNullOrEmpty(r.stationId) ? "—" : r.stationId; c[3].text = r.sensorType;
                c[4].text = r.provenance == Provenance.Unavailable ? "N/A" : r.hasValue ? r.value.ToString("0.###", CultureInfo.InvariantCulture) : string.IsNullOrEmpty(r.valueText) ? "(location)" : Clip(r.valueText, 14);
                c[5].text = r.unit; c[6].text = r.provenance; c[7].text = sync; c[8].text = r.uploadState; c[9].text = r.analysisState == AnalysisState.Complete ? "COMPLETE" : "NOT CONNECTED";
                c[6].color = r.provenance == Provenance.Unavailable ? HudTokens.Amber : HudTokens.Text;
                c[7].color = sync == "SYNCED" ? HudTokens.Green : sync == "FAILED" ? HudTokens.Red : sync == "SYNCING" ? HudTokens.Cyan : HudTokens.TextDim;
                c[9].color = HudTokens.Amber;
            }
            DataPrev.interactable = DataPage > 0; DataNext.interactable = DataPage < pages - 1;
            RetryButton.interactable = failed > 0 || mission.Sync.Status.StartsWith("Sign-in");
        }

        void RefreshMissionPanel(List<Objective> obj)
        {
            if (lastDone.Length != obj.Count) lastDone = new bool[obj.Count];
            int active = obj.FindIndex(o => !o.done && !o.info);
            for (int i = 0; i < objectiveRows.Count; i++)
            {
                bool has = i < obj.Count; objectiveRows[i].gameObject.SetActive(has);
                if (!has) continue;
                var o = obj[i];
                if (o.done && !lastDone[i]) pulse[i] = 1;   // completion animation
                lastDone[i] = o.done;
                objectiveTexts[i].text = (o.info ? "INFO  " : o.done ? "DONE  " : i == active ? "ACTIVE  " : "PENDING  ") + o.text;
                objectiveTexts[i].color = o.info ? HudTokens.Amber : o.done ? HudTokens.Green : i == active ? HudTokens.Orange : HudTokens.TextDim;
                objectiveDots[i].color = objectiveTexts[i].color;
                objectiveRows[i].GetComponentInChildren<Image>().color = i == active ? new Color(HudTokens.Orange.r, HudTokens.Orange.g, HudTokens.Orange.b, .14f) : new Color(0, 0, 0, 0);
            }
            var wp = CurrentWaypoint(); var real = obj.Where(o => !o.info).ToList();
            var sync = RoverStatusProvider.SyncSummary(mission);
            missionText.text = $"Current waypoint: {(wp ? wp.GetComponent<ResearchTarget>()?.displayName ?? wp.name : "—")}\n" +
                $"Completion: {(real.All(o => o.done) ? "COMPLETE (local, unscored)" : $"{real.Count(o => o.done)}/{real.Count} objectives")}\n" +
                $"Backend sync: {sync.label} ({sync.pending} pending, {sync.failed} failed) • canonical mission id resolved from registry '{hud.game.registry.missionId}'\n" +
                "Prompt: " + mission.Instructions();
        }

        void RefreshSystemsPanel()
        {
            systemsWidgetsHost.gameObject.SetActive(!SettingsOpen);
            settingsHost.gameObject.SetActive(SettingsOpen);
            SettingsButton.GetComponentInChildren<Text>().text = SettingsOpen ? "STATUS" : "SETTINGS";
            UpdateSettingLabels(settingLabels);
        }

        protected override void ApplySettings()
        {
            base.ApplySettings();
            float o = mission.Settings.opacity;
            foreach (var go in new[] { TopBar, MissionCard, StatusSummary, NavWidget })
            {
                if (!go) continue;
                var cg = go.GetComponent<CanvasGroup>(); if (!cg) cg = go.AddComponent<CanvasGroup>();
                cg.alpha = Mathf.Lerp(.7f, 1f, (o - .4f) / .6f);
            }
        }
    }
}
