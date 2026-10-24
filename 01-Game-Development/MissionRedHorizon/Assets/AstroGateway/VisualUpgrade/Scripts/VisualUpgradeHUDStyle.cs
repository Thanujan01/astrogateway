using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        public Color dim = new Color(.62f, .71f, .77f, 1);
        // Batch 5 "tempered glass" tokens: semantic status colours and the glass edge/sheen.
        public Color good = new Color(.36f, .93f, .62f, 1), caution = new Color(1f, .76f, .22f, 1), critical = new Color(1f, .36f, .32f, 1);
        public Color glassEdge = new Color(.78f, .92f, 1f, .20f), glassSheen = new Color(1f, 1f, 1f, .085f), glassButton = new Color(.05f, .11f, .16f, .62f);
        static Sprite edgeSprite, sheenSprite;
        public static readonly string[] FontNames = { "Avenir Next", "Avenir", "Helvetica Neue", "Arial" };

        Font font;
        bool built;
        RectTransform speedPanel, scannerPanel, progressPanel, promptPill;
        Image speedFill, scanFill, progressFill;
        Text speedValue, speedLabel, scannerText, progressText, promptText;
        Text context;
        GameObject styledModal;
        VisualUpgradeHUDFixes fixes;
        Text telemetry;
        readonly List<CanvasGroup> dimmed = new List<CanvasGroup>();
        static readonly string[] TouchOnly = { "Analog movement joystick", "Run or accelerate", "Rover brake", "Scan", "Context interact" };
        static readonly string[] ActionLabels = { "PAUSE", "SETTINGS", "MAP", "INVENTORY" };
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
            // Hidden while scanning: the scan progress card carries that state (the pill would still say "[K] SCAN").
            promptPill.gameObject.SetActive(v <= 2 && !overlay && !modes.cinematic && !hud.scanning && !string.IsNullOrEmpty(promptText.text));
            DimBehindModal(overlay);
        }

        // Batch 2: the gameplay HUD fades to 30% behind pause/settings/report cards so the card reads first.
        // CanvasGroup alpha only (no blur, no extra render pass). Input is already blocked by the modal input shield
        // and by the paused simulation; raycast settings are left untouched. Touch controls keep their own CanvasGroup
        // (owned by VisualUpgradeHUDFixes) and are skipped.
        void DimBehindModal(bool overlay)
        {
            float a = overlay ? .3f : 1f;
            if (dimmed.Count == 0)
                for (int i = 0; i < hud.safe.childCount; i++)
                {
                    var c = hud.safe.GetChild(i);
                    if (c.name.StartsWith("Modal") || TouchOnly.Contains(c.name)) continue;
                    var g = c.GetComponent<CanvasGroup>();
                    if (!g) g = c.gameObject.AddComponent<CanvasGroup>();
                    dimmed.Add(g);
                }
            foreach (var g in dimmed) if (g && !Mathf.Approximately(g.alpha, a)) g.alpha = a;
        }

        // ---------------- one-time styling ----------------
        void Build()
        {
            built = true;
            try { font = Font.CreateDynamicFontFromOSFont(FontNames, 18); } catch { font = null; }
            if (!font) font = hud.theme.font;
            Restyle(hud.safe);

            fixes = FindAnyObjectByType<VisualUpgradeHUDFixes>();
            telemetry = hud.safe.Find("Rover telemetry/Rover live telemetry")?.GetComponent<Text>();
            if (telemetry) { telemetry.resizeTextForBestFit = true; telemetry.resizeTextMinSize = 13; telemetry.resizeTextMaxSize = 20; }
            context = hud.safe.Find("Interaction target")?.GetComponent<Text>();
            if (context) context.color = new Color(0, 0, 0, 0); // shown through the prompt pill instead

            // UI pass 2: objectives no longer clip ("ORBIS upload: unavailable").
            var objectives = hud.safe.Find("Mission objectives/Objectives live")?.GetComponent<Text>();
            if (objectives) { objectives.resizeTextForBestFit = true; objectives.resizeTextMinSize = 13; objectives.resizeTextMaxSize = 19; objectives.verticalOverflow = VerticalWrapMode.Truncate; }

            // Batch 2: branding lost its "LOCAL TRAINING • PROVISIONAL" line at 1920×1080 and 2560×1440; recent-scan
            // text was cut at 1512×982; footer fits one line in the taller card below.
            Fit(hud.safe.Find("Mission branding/Branding"), 12, 22);
            Fit(hud.safe.Find("Recent scientific scan/Recent scan live"), 12, 16);
            Fit(hud.safe.Find("Recent scientific scan/Truthful local sync"), 11, 14);
            var sync = hud.safe.Find("Recent scientific scan/Truthful local sync")?.GetComponent<Text>();
            if (sync) sync.alignment = TextAnchor.MiddleLeft;

            // Batch 2: side buttons read as words, on the same rounded pill as every other HUD button.
            for (int i = 0; i < 4; i++)
            {
                var b = hud.safe.Find("Action " + i);
                if (!b) continue;
                var img = b.GetComponent<Image>();
                if (img) { img.sprite = hud.theme.rounded; img.type = Image.Type.Sliced; }
                var label = b.GetComponentInChildren<Text>(true);
                if (label) { label.text = ActionLabels[i]; label.resizeTextMinSize = 11; label.resizeTextMaxSize = 14; label.fontStyle = FontStyle.Normal; }
            }

            // UI pass 2: prompt pill, speed gauge and telemetry share one centre column (.36–.64).
            speedPanel = Panel("VU Rover speed gauge", new Vector2(.36f, .12f), new Vector2(.64f, .19f));
            speedLabel = Label("Speed label", speedPanel, "ROVER SPEED • LOCAL SIM", new Vector2(.05f, .64f), new Vector2(.95f, .98f), 13, dim, TextAnchor.MiddleLeft);
            speedValue = Label("Speed value", speedPanel, "0.0 m/s", new Vector2(.05f, .22f), new Vector2(.95f, .70f), 22, text, TextAnchor.MiddleLeft);
            speedFill = Bar("Speed bar", speedPanel, new Vector2(.05f, .08f), new Vector2(.95f, .17f), cyan);
            compact.Add(speedPanel.gameObject);

            scannerPanel = Panel("VU Scanner status", new Vector2(.79f, .395f), new Vector2(.988f, .455f));
            scannerText = Label("Scanner text", scannerPanel, "", new Vector2(.05f, .30f), new Vector2(.97f, .97f), 13, text, TextAnchor.MiddleLeft);
            scanFill = Bar("Scanner bar", scannerPanel, new Vector2(.05f, .1f), new Vector2(.95f, .22f), orange);
            compact.Add(scannerPanel.gameObject);

            progressPanel = Panel("VU Mission progress", new Vector2(.012f, .60f), new Vector2(.19f, .65f));
            progressText = Label("Progress text", progressPanel, "", new Vector2(.05f, .38f), new Vector2(.97f, .97f), 14, text, TextAnchor.MiddleLeft);
            progressFill = Bar("Progress bar", progressPanel, new Vector2(.05f, .12f), new Vector2(.95f, .30f), cyan);
            fullOnly.Add(progressPanel.gameObject);

            promptPill = Panel("VU Interaction prompt", new Vector2(.36f, .20f), new Vector2(.64f, .245f));
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
                if (img) { img.color = glassButton; Glass((RectTransform)b.transform, false); }
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
                if (p) Glass(p, true);
            }
        }

        // ---------------- "tempered glass" panels (batch 5) ----------------
        // A thin light edge and a soft top sheen over the translucent panel, drawn as two sliced sprites generated once
        // at runtime (no textures added to the project, no blur pass). Both sit behind the panel's own content.
        void Glass(RectTransform target, bool sheen)
        {
            if (!target || target.Find("VU glass edge")) return;
            MakeSprites();
            if (sheen) Overlay(target, "VU glass sheen", sheenSprite, glassSheen);
            Overlay(target, "VU glass edge", edgeSprite, glassEdge);
        }

        static void Overlay(RectTransform target, string name, Sprite sprite, Color color)
        {
            var g = new GameObject(name, typeof(RectTransform), typeof(Image));
            var r = (RectTransform)g.transform; r.SetParent(target, false);
            r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one; r.offsetMin = r.offsetMax = Vector2.zero;
            r.SetAsFirstSibling();
            var i = g.GetComponent<Image>(); i.sprite = sprite; i.type = Image.Type.Sliced; i.color = color; i.raycastTarget = false;
        }

        void MakeSprites()
        {
            if (edgeSprite) return;
            const int n = 128; const float radius = 20, border = 22;
            float ppu = hud.theme.rounded ? hud.theme.rounded.pixelsPerUnit : 100;
            var edge = new Texture2D(n, n, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp, name = "VU glass edge (runtime)" };
            var gloss = new Texture2D(n, n, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp, name = "VU glass sheen (runtime)" };
            var e = new Color[n * n]; var s = new Color[n * n];
            for (int y = 0; y < n; y++)
                for (int x = 0; x < n; x++)
                {
                    // signed distance to a rounded rectangle filling the texture
                    float px = Mathf.Abs(x + .5f - n / 2f) - (n / 2f - radius), py = Mathf.Abs(y + .5f - n / 2f) - (n / 2f - radius);
                    float d = new Vector2(Mathf.Max(px, 0), Mathf.Max(py, 0)).magnitude + Mathf.Min(Mathf.Max(px, py), 0) - radius;
                    float inside = Mathf.Clamp01(-d + .5f);
                    float ring = Mathf.Clamp01(1.6f - Mathf.Abs(d + 1.2f));
                    e[y * n + x] = new Color(1, 1, 1, ring * inside + ring * .4f * (1 - inside));
                    float top = Mathf.InverseLerp(n * .45f, n, y);           // sheen on the upper part, fading downward
                    s[y * n + x] = new Color(1, 1, 1, inside * top * top);
                }
            edge.SetPixels(e); edge.Apply(); gloss.SetPixels(s); gloss.Apply();
            var b = new Vector4(border, border, border, border);
            edgeSprite = Sprite.Create(edge, new Rect(0, 0, n, n), new Vector2(.5f, .5f), ppu, 0, SpriteMeshType.FullRect, b);
            sheenSprite = Sprite.Create(gloss, new Rect(0, 0, n, n), new Vector2(.5f, .5f), ppu, 0, SpriteMeshType.FullRect, b);
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
            Glass(r, true);
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

        static void Fit(Transform t, int min, int max)
        {
            var text = t ? t.GetComponent<Text>() : null;
            if (!text) return;
            text.resizeTextForBestFit = true; text.resizeTextMinSize = min; text.resizeTextMaxSize = max;
            text.horizontalOverflow = HorizontalWrapMode.Wrap; text.verticalOverflow = VerticalWrapMode.Truncate;
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
            for (int i = 0; i < 4; i++) Set(s, "Action " + i, new Vector2(.772f, .835f - i * .055f), new Vector2(.848f, .875f - i * .055f));
            Set(s, "Production overview", new Vector2(.37f, .862f), new Vector2(.46f, .897f));
            Set(s, "Production chase", new Vector2(.465f, .862f), new Vector2(.555f, .897f));
            Set(s, "Production control tuning", new Vector2(.56f, .862f), new Vector2(.65f, .897f));
            Set(s, "Mode and camera", new Vector2(.37f, .832f), new Vector2(.65f, .86f));
            Set(s, "Rover telemetry", new Vector2(.36f, .02f), new Vector2(.64f, .112f));
            // Batch 2: taller card (uses the empty space under the minimap) with three rows: scan text, OPEN REPORT
            // button at its own height, and the local-only footer across the full width.
            Set(s, "Recent scientific scan", new Vector2(.79f, .17f), new Vector2(.988f, .385f));
            Set(s, "Recent scientific scan/Recent scan live", new Vector2(.05f, .29f), new Vector2(.95f, .96f));
            Set(s, "Recent scientific scan/Scan detail", new Vector2(.60f, .135f), new Vector2(.97f, .27f));
            Set(s, "Recent scientific scan/Truthful local sync", new Vector2(.04f, .015f), new Vector2(.97f, .125f));
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
            speedFill.color = cyan;

            var target = hud.CurrentTarget();
            if (hud.scanning) { scannerText.text = "SCANNER • ACQUIRING (local simulated sensor)"; scanFill.color = orange; }
            else if (target) { scannerText.text = "SCANNER • READY  " + target.displayName; scanFill.color = cyan; SetFill(scanFill, 1); }
            else { scannerText.text = p.driving ? "SCANNER • STOWED WHILE DRIVING" : "SCANNER • NO TARGET IN 3 m"; SetFill(scanFill, 0); }
            if (hud.scanning)
            {
                var progress = hud.safe.Find("Scan presentation progress/Scan fill") as RectTransform;
                SetFill(scanFill, progress ? Mathf.InverseLerp(.05f, .95f, progress.anchorMax.x) : .5f);
            }

            // Batch 2: rover speed lives in the speed gauge only; telemetry keeps battery and condition.
            if (telemetry && telemetry.text.Contains("BAT ") && telemetry.text.Contains(" m/s"))
                telemetry.text = Regex.Replace(telemetry.text, @"\s+-?\d+(\.\d+)? m/s", "");

            int total = hud.environment.targets.Length, done = hud.objectiveScans;
            progressText.text = $"MISSION PROGRESS • {done}/{total} STATIONS (local)";
            SetFill(progressFill, total > 0 ? done / (float)total : 0);

            // Pill shows only something actionable: the idle "No interaction in range" and the recent-scan placeholder
            // (already in the RECENT SCAN card) are dropped, so an empty pill hides itself.
            if (context)
                promptText.text = string.Join("   •   ", context.text.Split('\n')
                    .Where(l => l.Length > 0 && l != "No interaction in range" && l != "No scientific samples recorded"));

            // Resources with no data source keep "NOT CONNECTED" but their empty bar no longer reads as a 0% gauge.
            foreach (var id in ResourceIds)
            {
                var r = hud.safe.Find("Survival resources/" + id);
                var value = r ? r.Find("Resource value")?.GetComponent<Text>() : null;
                var track = r ? r.Find("Track")?.GetComponent<Image>() : null;
                if (!value || !track) continue;
                bool connected = value.text != "NOT CONNECTED";
                float a = connected ? .6f : .18f;
                if (!Mathf.Approximately(track.color.a, a)) { var c = track.color; c.a = a; track.color = c; }
                // Semantic fill: green nominal (>50%), amber 20-50%, red below 20%; symbols cyan when live, dim when not.
                var fill = track.transform.Find("Fill")?.GetComponent<Image>();
                if (fill && connected && float.TryParse(value.text.Split('/')[0].Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var v))
                    fill.color = v > 50 ? good : v > 20 ? caution : critical;
                var symbol = r.Find("Symbol")?.GetComponent<Text>();
                if (symbol) symbol.color = connected ? cyan : dim;
            }
        }

        static readonly string[] ResourceIds = { "oxygen", "water", "food", "power", "radiation" };

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
            card.GetComponent<Image>().color = new Color(.02f, .045f, .07f, .90f);
            Glass((RectTransform)card, true);
            // Slider fills are data, so cyan like every other gauge (they were orange, the action colour).
            foreach (var img in card.GetComponentsInChildren<Image>(true))
                if (img.name == "Fill" && img.transform.parent && img.transform.parent.name.EndsWith(" track")) img.color = cyan;
            Restyle(card);
            Accent((RectTransform)card, orange);
            var title = card.Find("Modal title")?.GetComponent<Text>();
            if (title) title.color = cyan;
            string t = title ? title.text : "";
            if (t.StartsWith("Simulation paused"))
            {
                if (title) title.text = "PAUSED • LOCAL TRAINING SIMULATION";
                Label("VU controls reference", card,
                    "CONTROLS\nW A S D  move / drive     Shift  run     Space  rover brake\nE  board / exit rover (within 5 m, stopped)     K  scan (within 3 m)\nC  cinematic overview     F1  HUD profile     1 / 2 / 3  weather preview\nRight mouse + drag  orbit camera     Esc  pause / close     Arrow keys also move\nGAMEPAD  L-stick move/steer   R-stick camera   RT run/accelerate   LT/B brake   A board/exit   X scan   Y cinematic   Start pause",
                    new Vector2(.06f, .25f), new Vector2(.94f, .66f), 17, text, TextAnchor.UpperLeft).lineSpacing = 1.25f;
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
                var note = Label("VU graphics note", card, "Graphics preset (Low/Medium cap 30 fps, High/Ultra cap 60 fps)", new Vector2(.06f, .095f), new Vector2(.94f, .125f), 13, dim, TextAnchor.MiddleLeft);
                if (fixes && !fixes.TouchVisible) DesktopSettings(card, note.rectTransform, names);
            }
        }

        // Batch 2: on desktop (touch controls hidden) the joystick size / side / floating settings do nothing visible,
        // so they are hidden and the remaining rows close up. Their values and handlers are untouched, and on a phone,
        // touchscreen or with showTouchControls they are shown exactly as before.
        void DesktopSettings(Transform card, RectTransform note, string[] presetNames)
        {
            foreach (var name in new[] { "Joystick size", "Joystick size track", "Handedness", "Floating joystick" })
            {
                var t = card.Find(name);
                if (t) t.gameObject.SetActive(false);
            }
            void Row(string name, float y)
            {
                Set(card, name, new Vector2(.06f, y), new Vector2(.43f, y + .055f));
                Set(card, name + " track", new Vector2(.46f, y), new Vector2(.91f, y + .06f));
            }
            Row("Control sensitivity", .52f);
            Row("Map zoom", .44f);
            Set(card, "Weather cycle", new Vector2(.66f, .24f), new Vector2(.94f, .32f));
            note.anchorMin = new Vector2(.06f, .17f); note.anchorMax = new Vector2(.94f, .21f);
            for (int q = 0; q < 4; q++) Set(card, "VU graphics " + presetNames[q], new Vector2(.06f + q * .15f, .09f), new Vector2(.20f + q * .15f, .16f));
        }
    }
}
