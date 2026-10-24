using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace AstroGateway.FirstMission.HUD
{
    // Reusable status widget: icon, label, value (smoothly animated), unit, provenance tag, optional bar,
    // level colour (normal / warning / critical / unknown) and an accessibility sentence.
    public class HudStatusWidget : MonoBehaviour
    {
        public IStatusSource source;
        public Image icon, fill, levelDot;
        public Text label, value, unit, tag;
        public float refreshInterval = .25f;
        public StatusReading Last { get; private set; }
        public string AccessibleText => Last.Accessible;
        float shownValue, shownFill, next; bool first = true; Color shownColor = HudTokens.TextDim;

        public void Poll(bool force = false)
        {
            if (source == null || (!force && Time.unscaledTime < next)) return;
            next = Time.unscaledTime + refreshInterval;
            Last = source.Read();
            var r = Last;
            if (label) label.text = r.label;
            if (tag) { tag.text = r.source; tag.color = r.Available ? HudTokens.TextDim : HudTokens.Amber; }
            if (unit) unit.text = r.Available ? r.unit ?? "" : "";
            if (icon && icon.sprite == null) icon.sprite = HudIcons.Get(r.icon);
            if (!r.value.HasValue || first) { shownValue = r.value ?? 0; first = false; }
            if (value && (!r.value.HasValue || !r.Available)) value.text = r.Available ? r.text : (r.source == Source.NotConnected ? "NOT CONNECTED" : "N/A");
        }

        void Update()
        {
            Poll();
            var r = Last;
            float dt = Time.unscaledDeltaTime;
            var target = HudTokens.ForLevel(r.level);
            shownColor = Color.Lerp(shownColor, target, 1 - Mathf.Exp(-10 * dt));
            if (icon) icon.color = shownColor;
            if (levelDot) levelDot.color = shownColor;
            if (r.value.HasValue && r.Available && value)
            {
                shownValue = Mathf.Lerp(shownValue, r.value.Value, 1 - Mathf.Exp(-12 * dt));
                // Keep the formatted text when it is not a plain number (e.g. "12/400"), otherwise animate.
                value.text = float.TryParse(r.text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out _)
                    ? shownValue.ToString(r.text.Contains(".") ? "0.0" : "0", System.Globalization.CultureInfo.InvariantCulture) : r.text;
            }
            if (fill)
            {
                float f = r.fill.HasValue && r.Available ? Mathf.Clamp01(r.fill.Value) : 0;
                shownFill = Mathf.Lerp(shownFill, f, 1 - Mathf.Exp(-10 * dt));
                fill.rectTransform.anchorMax = new Vector2(shownFill, 1);
                fill.color = shownColor;
            }
        }
    }

    // Navigation widget: a north-up map rendered ONCE from the real scene (top-down orthographic camera over
    // the 240 m playable area), with live markers from Unity transforms: rover, astronaut, research stations
    // and the mission waypoint. Positions are scene metres, not GPS.
    public class HudMinimap : MonoBehaviour
    {
        public Transform rover, astronaut, waypoint;
        public System.Func<Vector3> focus;      // player position (rover while driving, astronaut on foot)
        public System.Func<bool> showAstronaut;
        public List<Transform> stations = new List<Transform>();
        public RawImage map;
        public RectTransform roverMarker, astronautMarker, waypointMarker, viewport;
        public readonly List<RectTransform> stationMarkers = new List<RectTransform>();
        public float worldHalfExtent = 125f;   // map covers x,z in [-125, 125]
        public float zoomRadius = 45f;         // metres shown from centre to edge in the widget
        public int resolution = 1024;          // also used by the fullscreen map
        public bool Rendered { get; private set; }
        public RenderTexture Texture { get; private set; }
        Camera cam; bool pending;
        bool priorFog;

        void Start()
        {
            Texture = new RenderTexture(resolution, resolution, 16, RenderTextureFormat.ARGB32) { name = "HudMinimap" };
            var go = new GameObject("HUD minimap capture camera (renders once)");
            cam = go.AddComponent<Camera>();
            cam.orthographic = true; cam.orthographicSize = worldHalfExtent;
            cam.transform.SetPositionAndRotation(new Vector3(0, 400, 0), Quaternion.Euler(90, 0, 0));
            cam.nearClipPlane = 1; cam.farClipPlane = 800; cam.targetTexture = Texture;
            cam.clearFlags = CameraClearFlags.SolidColor; cam.backgroundColor = HudTokens.Background;
            cam.cullingMask = ~(1 << 5);   // everything except UI
            cam.depth = -50;
            if (map) map.texture = Texture;
            RenderPipelineManager.beginCameraRendering += Begin;
            RenderPipelineManager.endCameraRendering += End;
            pending = true;
        }

        void Begin(ScriptableRenderContext c, Camera camera) { if (camera != cam) return; priorFog = RenderSettings.fog; RenderSettings.fog = false; }
        void End(ScriptableRenderContext c, Camera camera)
        {
            if (camera != cam) return;
            RenderSettings.fog = priorFog;
            Rendered = true;
        }

        void LateUpdate()
        {
            if (cam && Rendered && pending) { pending = false; cam.enabled = false; RenderPipelineManager.beginCameraRendering -= Begin; RenderPipelineManager.endCameraRendering -= End; }
            if (!rover) return;
            var centre = focus != null ? focus() : rover.position;
            // Show a window of +-zoomRadius metres around the player by moving the UV rect.
            float span = zoomRadius * 2 / (worldHalfExtent * 2);
            float u = (centre.x + worldHalfExtent) / (worldHalfExtent * 2) - span / 2, v = (centre.z + worldHalfExtent) / (worldHalfExtent * 2) - span / 2;
            if (map) map.uvRect = new Rect(u, v, span, span);
            if (!roverMarker || !viewport) return;   // capture-only use (fullscreen map texture)
            Place(roverMarker, rover.position, centre, true);
            roverMarker.localEulerAngles = new Vector3(0, 0, -rover.eulerAngles.y);
            if (astronautMarker && astronaut) { bool show = showAstronaut == null || showAstronaut(); astronautMarker.gameObject.SetActive(show); if (show) Place(astronautMarker, astronaut.position, centre, true); }
            if (waypointMarker && waypoint) Place(waypointMarker, waypoint.position, centre, true);
            for (int i = 0; i < stations.Count && i < stationMarkers.Count; i++) if (stations[i]) Place(stationMarkers[i], stations[i].position, centre, false);
        }

        // Clamp to the widget edge (waypoint shown on the rim when outside the window).
        void Place(RectTransform marker, Vector3 world, Vector3 centre, bool clamp)
        {
            if (!marker) return;
            var d = new Vector2(world.x - centre.x, world.z - centre.z) / zoomRadius;   // -1..1
            bool outside = d.magnitude > .92f;
            if (outside && clamp) d = d.normalized * .92f;
            marker.gameObject.SetActive(clamp || !outside);
            var half = viewport.rect.size / 2;
            marker.anchoredPosition = new Vector2(d.x * half.x, d.y * half.y);
        }

        void OnDestroy()
        {
            RenderPipelineManager.beginCameraRendering -= Begin; RenderPipelineManager.endCameraRendering -= End;
            if (Texture) Texture.Release();
        }
    }
}
