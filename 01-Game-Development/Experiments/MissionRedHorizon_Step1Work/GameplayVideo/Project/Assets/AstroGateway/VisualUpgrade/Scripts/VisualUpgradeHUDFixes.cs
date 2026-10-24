using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using AstroGateway.CinematicHUD;
using AstroGateway.ProductionVisual;

namespace AstroGateway.VisualUpgrade
{
    // Laptop-playtest HUD defects, fixed for Mars_VisualUpgrade_v1 only. Runs after AstroGatewayHUDController (300)
    // and ProductionPresentation (440) and corrects what they produce; neither shared script is modified.
    //  - weather panel, "local only" footer and INVENTORY label no longer clip at 1512x982
    //  - MENU mode label hidden while a modal / pause / tuning window is open
    //  - rover hint says "Stop to exit" only while actually moving
    //  - touch controls hidden on desktop (shown on mobile, a touchscreen, or with showTouchControls)
    //  - own save file so this scene never overwrites the Production Visual v1 save
    [DefaultExecutionOrder(600)]
    public class VisualUpgradeHUDFixes : MonoBehaviour
    {
        public AstroGatewayHUDController hud;
        public ProductionPresentation presentation;
        public bool showTouchControls;
        public const string SaveFile = "AstroGateway_VisualUpgrade_v1.json";
        static readonly string[] TouchOnly = { "Analog movement joystick", "Run or accelerate", "Rover brake", "Scan", "Context interact" };

        Text modeLabel, contextLabel, syncLabel;
        RectTransform weatherPanel, scanDetail;
        bool styled;

        public static bool TouchDevice => Application.isMobilePlatform || Touchscreen.current != null;
        public bool TouchVisible => showTouchControls || TouchDevice;

        void Start()
        {
            hud.game.testSave = Path.Combine(Application.persistentDataPath, SaveFile);
        }

        void LateUpdate()
        {
            if (!hud.safe || !hud.game.initialized) return;
            if (!styled) Bind();
            var p = hud.game.prototype;
            bool hidden = hud.settings.visibility >= 3;

            // MENU label bleeding through modal headers.
            bool overlay = hud.modalOpen || hud.environment.paused || presentation.tuningWindow;
            if (modeLabel) modeLabel.gameObject.SetActive(!hidden && !overlay);

            // Touch controls: desktop uses keyboard/mouse; prompts below carry the key hints instead.
            // Hidden with a CanvasGroup, not SetActive: the HUD controller reads their child Text every refresh.
            foreach (var name in TouchOnly)
            {
                var t = hud.safe.Find(name);
                if (!t) continue;
                var group = t.GetComponent<CanvasGroup>();
                if (!group) group = t.gameObject.AddComponent<CanvasGroup>();
                bool show = TouchVisible;
                group.alpha = show ? 1 : 0;
                group.interactable = group.blocksRaycasts = show;
            }

            // Context prompt, with the corrected rover-exit rule (exit is allowed below 0.2 m/s).
            if (contextLabel)
            {
                var target = hud.CurrentTarget();
                bool nearRover = Vector3.Distance(p.ActivePosition(), p.rover.transform.position) <= 5;
                bool keys = !TouchVisible;
                string text = target ? (keys ? "[K] SCAN  •  " : "TARGET • ") + target.displayName
                    : p.driving ? (Mathf.Abs(p.speed) > .2f ? "Stop the rover to exit" : keys ? "[E] EXIT ROVER" : "Rover stopped • exit available")
                    : nearRover ? (keys ? "[E] BOARD  •  " : "") + hud.game.registry.rover
                    : "No interaction in range";
                if (!string.IsNullOrEmpty(hud.lastResult)) text += "\n" + hud.lastResult;
                contextLabel.text = text;
            }

            // Weather card: shorter honest lines in a slightly taller card.
            if (weatherPanel)
            {
                hud.weatherReadout.text = p.WeatherName + "  •  SESSION " + TimeSpan.FromSeconds(p.elapsed).ToString(@"mm\:ss")
                    + "\nTemp / wind: not connected\nSol clock: not implemented";
            }
        }

        void Bind()
        {
            styled = true;
            modeLabel = hud.safe.Find("Mode and camera")?.GetComponent<Text>();
            contextLabel = hud.safe.Find("Interaction target")?.GetComponent<Text>();
            weatherPanel = hud.safe.Find("Weather and clock") as RectTransform;
            syncLabel = hud.safe.Find("Recent scientific scan/Truthful local sync")?.GetComponent<Text>();
            scanDetail = hud.safe.Find("Recent scientific scan/Scan detail") as RectTransform;
            if (contextLabel) Fit(contextLabel, 11, 16);
            if (syncLabel) Fit(syncLabel, 14, 14);
            if (hud.weatherReadout) Fit(hud.weatherReadout, 11, 17);
            for (int i = 0; i < 4; i++)
            {
                var label = hud.safe.Find("Shortcut " + i + "/Shortcut " + i + " label")?.GetComponent<Text>();
                if (label) { label.horizontalOverflow = HorizontalWrapMode.Wrap; Fit(label, 10, 17); }
            }
        }

        static void Fit(Text t, int min, int max)
        {
            t.resizeTextForBestFit = true;
            t.resizeTextMinSize = min;
            t.resizeTextMaxSize = max;
            t.verticalOverflow = VerticalWrapMode.Truncate;
        }

        static void Anchor(RectTransform r, Vector2 a, Vector2 b)
        {
            if (r.anchorMin == a && r.anchorMax == b) return;
            r.anchorMin = a; r.anchorMax = b; r.offsetMin = r.offsetMax = Vector2.zero;
        }
    }
}
