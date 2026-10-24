using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using AstroGateway.CinematicHUD;
using AstroGateway.Integration;

// Scene glue for Mars_MissionIntegration_v1: starts the local mission session, wires the life-support and
// event adapters, and shows one small read-only status panel. Every value it shows is labelled provisional,
// and the backend is reported as not connected unless a real service says otherwise.
namespace AstroGateway.MissionIntegration
{
    [DefaultExecutionOrder(960)]
    public class MissionIntegrationController : MonoBehaviour
    {
        public AstroGatewayHUDController hud;
        public GameplayEventBridge bridge;
        public LifeSupportModelAdapter lifeSupport;
        public bool panelVisible = true;
        public float refreshSeconds = 0.5f;
        public MissionRecommendation recommendation;
        public MissionReport report;
        public Text panelText;
        GameObject panel;
        float next;

        void Start()
        {
            if (!hud) hud = FindAnyObjectByType<AstroGatewayHUDController>();
            if (!hud || !hud.game) { enabled = false; return; }
            if (bridge) bridge.game = hud.game;
            if (lifeSupport) lifeSupport.prototype = hud.game.prototype;
            IntegrationServices.Recommendations.Recommend(IntegrationServices.Identity.Current, r => recommendation = r);
            BuildPanel();
        }

        void BuildPanel()
        {
            panel = new GameObject("Mission integration status", typeof(RectTransform), typeof(Image));
            var r = (RectTransform)panel.transform;
            r.SetParent(hud.safe, false);
            r.anchorMin = new Vector2(.015f, .40f);
            r.anchorMax = new Vector2(.205f, .635f);
            r.offsetMin = r.offsetMax = Vector2.zero;
            var bg = panel.GetComponent<Image>();
            bg.sprite = hud.theme.rounded;
            bg.type = Image.Type.Sliced;
            bg.color = new Color(hud.theme.panel.r, hud.theme.panel.g, hud.theme.panel.b, .78f);
            bg.raycastTarget = false;
            var t = new GameObject("Status text", typeof(RectTransform), typeof(Text));
            var tr = (RectTransform)t.transform;
            tr.SetParent(r, false);
            tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one;
            tr.offsetMin = new Vector2(14, 8); tr.offsetMax = new Vector2(-12, -10);
            panelText = t.GetComponent<Text>();
            panelText.font = hud.theme.font;
            panelText.color = hud.theme.text;
            panelText.alignment = TextAnchor.UpperLeft;
            panelText.resizeTextForBestFit = true;
            panelText.resizeTextMinSize = 11;
            panelText.resizeTextMaxSize = 16;
            panelText.raycastTarget = false;
        }

        void Update()
        {
            if (!hud.game.testing && Keyboard.current?.iKey.wasPressedThisFrame == true) panelVisible = !panelVisible;
            if (Time.unscaledTime < next) return;
            next = Time.unscaledTime + refreshSeconds;
            Refresh();
        }

        public void Refresh()
        {
            if (!panel) return;
            panel.SetActive(panelVisible && hud.settings.visibility < 2);
            IntegrationServices.Assessment.RequestReport(IntegrationServices.Sessions.Session.sessionId, r => report = r);
            panelText.text = Compose();
        }

        public string Compose()
        {
            var id = IntegrationServices.Identity.Current;
            var sessions = IntegrationServices.Sessions;
            var s = sessions.Session;
            var w = IntegrationServices.Weather?.Current;
            var p = hud.game.prototype;
            string weather = w == null ? "not available" : $"{w.dust} • vis {w.visibilityMeters:F0} m";
            string evidence = report == null ? "" : string.Join(" • ", report.skillEvidence.Select(e => e.skillId.Split('.').Last().Replace("simulated_", "").Replace('_', ' ') + " " + e.count));
            string score = report != null && report.Scored ? $"{report.score:F0} ({(report.authoritative ? "server" : "provisional")})" : "not scored • rubric pending";
            string rec = recommendation == null ? "none" : recommendation.authoritative ? recommendation.missionId : "offline default • " + recommendation.reason.Replace("Offline default: ", "");
            return "MISSION LINK • PROVISIONAL\n" +
                   $"Account: {(id.authenticated ? id.displayName : "offline • not signed in")}\n" +
                   $"Backend: {(sessions.Connected ? "connected" : "NOT CONNECTED")}\n" +
                   $"Session: {(s.authoritative ? "server" : "local")} {Short(s.sessionId)} • {sessions.Pending.Count} queued\n" +
                   $"Weather: {weather}\n" +
                   $"O2 model: {(lifeSupport ? lifeSupport.lastModelRate : 0):F3} %/s • unapproved\n" +
                   $"Evidence: {evidence}\n" +
                   $"Score: {score}\n" +
                   $"Next: {rec}";
        }

        static string Short(string id) => string.IsNullOrEmpty(id) ? "-" : id.Substring(0, Mathf.Min(8, id.Length));
    }
}
