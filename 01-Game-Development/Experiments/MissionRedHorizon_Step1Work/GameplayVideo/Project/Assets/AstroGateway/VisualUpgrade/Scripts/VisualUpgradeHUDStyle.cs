using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using AstroGateway.CinematicHUD;
using AstroGateway.ProductionVisual;

namespace AstroGateway.VisualUpgrade
{
    // Desktop sci-fi HUD presentation for Mars_VisualUpgrade_v1. Restyles and re-lays out the HUD that
    // AstroGatewayHUDController builds (same buttons, same handlers, same honest labels) and adds a few read-only
    // readouts: rover speed gauge, scanner status, mission progress bar, key prompt pill, controls reference in the
    // pause card and one-click graphics presets in the settings card. Every number shown comes from the existing
    // local simulation and is labelled as such; nothing is fetched or invented.
    [DefaultExecutionOrder(620)]
    public class VisualUpgradeHUDStyle : MonoBehaviour
    {
        public AstroGatewayHUDController hud;
        public ProductionModeCoordinator modes;
        public ProductionPresentation presentation;
        public Color panel = new Color(.015f, .035f, .055f, .64f);
        public Color cyan = new Color(.30f, .86f, .96f, 1);
        public Color orange = new Color(1f, .56f, .16f, 1);
        public Color text = new Color(.90f, .94f, .96f, 1);
        public Color dim = new Color(.58f, .68f, .74f, 1);
        public static readonly string[] FontNames = { "Avenir Next", "Avenir", "Helvetica Neue", "Arial" };

        Font font;
        bool built;
        RectTransform speedPanel, scannerPanel, progressPanel, promptPill;
        Image speedFill, scanFill, progressFill;
        Text speedValue, speedLabel, scannerText, progressText, promptText;
        Text context;
        GameObject styledModal;
        readonly List<GameObject> fullOnly = new List<GameObject>(), compact = new List<GameObject>();

        void LateUpdate()
        {
            if (!hud.safe || !hud.game.initialized) return;
            if (!built) Build();
            Layout();
            Readouts();
            StyleModal();
            int v = hud.settings.visibility;
            bool overlay = hud.modalOpen || hud.environment.paused;
            foreach (var g in fullOnly) { bool on = v == 0 && !overlay; if (g.activeSelf != on) g.SetActive(on); }
            foreach (var g in compact) { bool on = v <= 1 && !overlay; if (g.activeSelf != on) g.SetActive(on); }
            promptPill.gameObject.SetActive(v <= 2 && !overlay && !modes.cinematic && !string.IsNullOrEmpty(promptText.text));
        }

        // ---------------- one-time styling ----------------
        void Build()
        {
            built = true;
            try { font = Font.CreateDynamicFontFromOSFont(FontNames, 18); } catch { font = null; }
            if (!font) font = hud.theme.font;
            Restyle(hud.safe);

            var telemetry = hud.safe.Find("Rover telemetry/Rover live telemetry")?.GetComponent<Text>();
            if (telemetry) { telemetry.resizeTextForBestFit = true; telemetry.resizeTextMinSize = 13; telemetry.resizeTextMaxSize = 20; }
            context = hud.safe.Find("Interaction target")?.GetComponent<Text>();
            if (context) context.color = new Color(0, 0, 0, 0); // shown through the prompt pill instead

            speedPanel = Panel("VU Rover speed gauge", new Vector2(.415f, .115f), new Vector2(.585f, .19f));
            speedLabel = Label("Speed label", speedPanel, "ROVER SPEED • LOCAL SIM", new Vector2(.05f, .64f), new Vector2(.95f, .98f), 13, dim, TextAnchor.MiddleLeft);
            speedValue = Label("Speed value", speedPanel, "0.0 m/s", new Vector2(.05f, .22f), new Vector2(.95f, .70f), 22, text, TextAnchor.MiddleLeft);
            speedFill = Bar("Speed bar", speedPanel, new Vector2(.05f, .08f), new Vector2(.95f, .17f), cyan);
            compact.Add(speedPanel.gameObject);

            scannerPanel = Panel("VU Scanner status", new Vector2(.79f, .315f), new Vector2(.988f, .375f));
            scannerText = Label("Scanner text", scannerPanel, "", new Vector2(.05f, .30f), new Vector2(.97f, .97f), 13, text, TextAnchor.MiddleLeft);
            scanFill = Bar("Scanner bar", scannerPanel, new Vector2(.05f, .1f), new Vector2(.95f, .22f), orange);
            compact.Add(scannerPanel.gameObject);

            progressPanel = Panel("VU Mission progress", new Vector2(.012f, .60f), new Vector2(.19f, .65f));
            progressText = Label("Progress text", progressPanel, "", new Vector2(.05f, .38f), new Vector2(.97f, .97f), 14, text, TextAnchor.MiddleLeft);
            progressFill = Bar("Progress bar", progressPanel, new Vector2(.05f, .12f), new Vector2(.95f, .30f), cyan);
            fullOnly.Add(progressPanel.gameObject);

            promptPill = Panel("VU Interaction prompt", new Vector2(.36f, .205f), new Vector2(.64f, .25f));
            promptPill.GetComponent<Image>().color = new Color(.01f, .03f, .05f, .72f);
            promptText = Label("Prompt text", promptPill, "", new Vector2(.03f, .05f), new Vector2(.97f, .95f), 16, orange, TextAnchor.MiddleCenter);
        }

        void Restyle(Transform root)
        {
            foreach (var t in root.GetComponentsInChildren<Text>(true))
            {
                if (font) t.font = font;
                if (t.color.a > .05f && t.color != (Color)hud.theme.orange && t.color != (Color)hud.theme.oxygen && t.color != (Color)hud.theme.critical)
                    t.color = t.color.grayscale < .6f ? dim : text;
            }
            foreach (var b in root.GetComponentsInChildren<Button>(true))
            {
                var img = b.targetGraphic as Image;
                if (img) img.color = new Color(.03f, .07f, .10f, .80f);
                var colors = b.colors; colors.highlightedColor = new Color(.55f, .95f, 1f); colors.pressedColor = new Color(1f, .7f, .35f); colors.selectedColor = Color.white; b.colors = colors;
                var label = b.GetComponentInChildren<Text>(true);
                if (label) { label.horizontalOverflow = HorizontalWrapMode.Wrap; label.resizeTextForBestFit = true; label.resizeTextMinSize = 10; label.resizeTextMaxSize = 16; }
            }
            // Outline duplicates the whole mesh behind translucent panels and tints them; the accent bars replace it.
            foreach (var o in root.GetComponentsInChildren<Outline>(true)) o.enabled = false;
            foreach (var name in new[] { "Mission branding", "Survival resources", "Weather and clock", "Mission objectives", "Circular minimap panel", "Rover telemetry", "Recent scientific scan" })
            {
                var p = root.Find(name) as RectTransform;
                if (p && !p.Find("VU accent")) Accent(p, name == "Survival resources" || name == "Rover telemetry" ? cyan : orange);
            }
        }

        void Accent(RectTransform parent, Color c)
        {
            var g = new GameObject("VU accent", typeof(RectTransform), typeof(Image));
            var r = (RectTransform)g.transform; r.SetParent(parent, false);
            r.anchorMin = new Vector2(.03f, 1); r.anchorMax = new Vector2(.3f, 1); r.pivot = new Vector2(0, 1);
            r.sizeDelta = new Vector2(0, 2); r.anchoredPosition = new Vector2(0, -1);
            var i = g.GetComponent<Image>(); i.color = c; i.raycastTarget = false;
        }

        RectTransform Panel(string name, Vector2 a, Vector2 b)
        {
            var g = new GameObject(name, typeof(RectTransform), typeof(Image));
            var r = (RectTransform)g.transform; r.SetParent(hud.safe, false);
            r.anchorMin = a; r.anchorMax = b; r.offsetMin = r.offsetMax = Vector2.zero;
            var i = g.GetComponent<Image>(); i.sprite = hud.theme.rounded; i.type = Image.Type.Sliced; i.color = panel; i.raycastTarget = false;
            Accent(r, cyan);
            return r;
        }

        Text Label(string name, Transform parent, string content, Vector2 a, Vector2 b, int size, Color color, TextAnchor align)
        {
            var g = new GameObject(name, typeof(RectTransform), typeof(Text));
            var r = (RectTransform)g.transform; r.SetParent(parent, false);
            r.anchorMin = a; r.anchorMax = b; r.offsetMin = r.offsetMax = Vector2.zero;
            var t = g.GetComponent<Text>(); t.font = font; t.fontSize = size; t.color = color; t.alignment = align; t.text = content; t.raycastTarget = false;
            t.resizeTextForBestFit = true; t.resizeTextMinSize = Mathf.Max(11, size - 6); t.resizeTextMaxSize = size;
            t.horizontalOverflow = HorizontalWrapMode.Wrap; t.verticalOverflow = VerticalWrapMode.Truncate;
            return t;
        }

        Image Bar(string name, Transform parent, Vector2 a, Vector2 b, Color c)
        {
            var track = new GameObject(name, typeof(RectTransform), typeof(Image));
            var r = (RectTransform)track.transform; r.SetParent(parent, false);
            r.anchorMin = a; r.anchorMax = b; r.offsetMin = r.offsetMax = Vector2.zero;
            var ti = track.GetComponent<Image>(); ti.color = new Color(.25f, .32f, .36f, .55f); ti.raycastTarget = false;
            var fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
            var fr = (RectTransform)fill.transform; fr.SetParent(r, false);
            fr.anchorMin = Vector2.zero; fr.anchorMax = new Vector2(0, 1); fr.offsetMin = fr.offsetMax = Vector2.zero;
            var fi = fill.GetComponent<Image>(); fi.color = c; fi.raycastTarget = false;
            return fi;
        }

        static void SetFill(Image fill, float f) { var r = fill.rectTransform; r.anchorMax = new Vector2(Mathf.Clamp01(f), 1); }

        // ---------------- layout (applied after ProductionPresentation.ApplyLayout every frame) ----------------
        static void Set(Transform root, string name, Vector2 a, Vector2 b)
        {
            var r = root.Find(name) as RectTransform;
            if (!r || (r.anchorMin == a && r.anchorMax == b)) return;
            r.anchorMin = a; r.anchorMax = b; r.offsetMin = r.offsetMax = Vector2.zero;
        }

        void Layout()
        {
            var s = hud.safe;
            Set(s, "Mission branding", new Vector2(.012f, .918f), new Vector2(.19f, .985f));
            Set(s, "Survival resources", new Vector2(.205f, .905f), new Vector2(.785f, .985f));
            Set(s, "Weather and clock", new Vector2(.80f, .895f), new Vector2(.988f, .985f));
            Set(s, "Mission objectives", new Vector2(.012f, .66f), new Vector2(.19f, .905f));
            Set(s, "Circular minimap panel", new Vector2(.855f, .64f), new Vector2(.988f, .88f));
            for (int i = 0; i < 4; i++) Set(s, "Action " + i, new Vector2(.82f, .835f - i * .055f), new Vector2(.848f, .875f - i * .055f));
            Set(s, "Production overview", new Vector2(.37f, .862f), new Vector2(.46f, .897f));
            Set(s, "Production chase", new Vector2(.465f, .862f), new Vector2(.555f, .897f));
            Set(s, "Production control tuning", new Vector2(.56f, .862f), new Vector2(.65f, .897f));
            Set(s, "Mode and camera", new Vector2(.37f, .832f), new Vector2(.65f, .86f));
            Set(s, "Rover telemetry", new Vector2(.36f, .02f), new Vector2(.64f, .112f));
            Set(s, "Recent scientific scan", new Vector2(.79f, .17f), new Vector2(.988f, .305f));
            Set(s, "Recent scientific scan/Truthful local sync", new Vector2(.04f, .0f), new Vector2(.62f, .24f));
            Set(s, "Recent scientific scan/Scan detail", new Vector2(.64f, .02f), new Vector2(.97f, .2f));
            for (int i = 0; i < 4; i++) Set(s, "Shortcut " + i, new Vector2(.012f + i * .062f, .02f), new Vector2(.068f + i * .062f, .065f));
        }

        // ---------------- read-only readouts ----------------
        void Readouts()
        {
            var p = hud.game.prototype;
            float speed = p.driving ? Mathf.Abs(p.speed) : 0;
            speedValue.text = p.driving ? $"{speed:0.0} m/s   {speed * 3.6f:0} km/h" : $"ON FOOT   {Mathf.Abs(p.speed):0.0} m/s";
            speedLabel.text = p.driving ? "ROVER SPEED • LOCAL SIM" : "SUIT PACE • LOCAL SIM";
            SetFill(speedFill, (p.driving ? speed : Mathf.Abs(p.speed)) / 3.8f);
            speedFill.color = p.driving ? cyan : orange;

            var target = hud.CurrentTarget();
            if (hud.scanning) { scannerText.text = "SCANNER • ACQUIRING (local simulated sensor)"; scanFill.color = orange; }
            else if (target) { scannerText.text = "SCANNER • READY  " + target.displayName; scanFill.color = cyan; SetFill(scanFill, 1); }
            else { scannerText.text = p.driving ? "SCANNER • STOWED WHILE DRIVING" : "SCANNER • NO TARGET IN 3 m"; SetFill(scanFill, 0); }
            if (hud.scanning)
            {
                var progress = hud.safe.Find("Scan presentation progress/Scan fill") as RectTransform;
                SetFill(scanFill, progress ? Mathf.InverseLerp(.05f, .95f, progress.anchorMax.x) : .5f);
            }

            int total = hud.environment.targets.Length, done = hud.objectiveScans;
            progressText.text = $"MISSION PROGRESS • {done}/{total} STATIONS (local)";
            SetFill(progressFill, total > 0 ? done / (float)total : 0);

            if (context) promptText.text = context.text.Replace("\n", "   •   ");
        }

        // ---------------- modal cards (pause, settings, samples, map) ----------------
        void StyleModal()
        {
            var shield = hud.safe.Find("Modal input shield");
            if (!shield) { styledModal = null; return; }
            if (styledModal == shield.gameObject) return;
            styledModal = shield.gameObject;
            shield.GetComponent<Image>().color = new Color(0, .01f, .02f, .55f);
            var card = shield.Find("Modal card");
            if (!card) return;
            card.GetComponent<Image>().color = new Color(.015f, .035f, .055f, .94f);
            Restyle(card);
            Accent((RectTransform)card, orange);
            var title = card.Find("Modal title")?.GetComponent<Text>();
            if (title) title.color = cyan;
            string t = title ? title.text : "";
            if (t.StartsWith("Simulation paused"))
            {
                if (title) title.text = "PAUSED • LOCAL TRAINING SIMULATION";
                Label("VU controls reference", card,
                    "CONTROLS\nW A S D  move / drive     Shift  run     Space  rover brake\nE  board / exit rover (within 5 m, stopped)     K  scan (within 3 m)\nC  cinematic overview     F1  HUD profile     1 / 2 / 3  weather preview\nRight mouse + drag  orbit camera     Esc  pause / close",
                    new Vector2(.06f, .30f), new Vector2(.94f, .62f), 15, text, TextAnchor.UpperLeft);
            }
            if (t.StartsWith("HUD & GRAPHICS"))
            {
                string[] names = { "LOW", "MEDIUM", "HIGH", "ULTRA" };
                var images = new Image[4];
                void Paint() { for (int i = 0; i < 4; i++) images[i].color = hud.settings.quality == i ? new Color(.05f, .30f, .36f, .95f) : new Color(.03f, .07f, .10f, .85f); }
                for (int q = 0; q < 4; q++)
                {
                    int level = q;
                    var b = hud.Button("VU graphics " + names[q], card, names[q], new Vector2(.06f + q * .15f, .03f), new Vector2(.20f + q * .15f, .095f), () =>
                    {
                        hud.settings.quality = level; hud.environment.ApplyQuality(level); hud.PersistSettings(); Paint();
                    });
                    images[q] = (Image)b.targetGraphic;
                    var label = b.GetComponentInChildren<Text>(); if (font) label.font = font;
                }
                Paint();
                Label("VU graphics note", card, "Graphics preset (Low/Medium cap 30 fps, High/Ultra cap 60 fps)", new Vector2(.06f, .095f), new Vector2(.94f, .125f), 13, dim, TextAnchor.MiddleLeft);
            }
        }
    }
}
