using System.Linq;
using UnityEngine;
using AstroGateway.CinematicHUD;
using AstroGateway.MarsEnvironment;

namespace AstroGateway.VisualUpgrade
{
    // Research-station status presentation (batch 3, visual only): each station's status light shows its state —
    // amber slow pulse = not yet surveyed, cyan fast pulse = being scanned now, steady green = a local simulated
    // scan was recorded within 3 m of it (same rule the environment uses to link scans to stations). While a scan
    // runs, a thin ring expands from the station on the ground. Reads HUD/game state only; never writes it.
    public class VisualUpgradeStationStatus : MonoBehaviour
    {
        public AstroGatewayHUDController hud;
        public ResearchTarget[] stations;
        public Renderer[] lights;
        public Transform[] rings;
        static readonly int Emission = Shader.PropertyToID("_EmissionColor");
        static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        MaterialPropertyBlock block;
        bool[] recorded;
        int seenScans = -1;

        void LateUpdate()
        {
            if (!hud || !hud.game || !hud.game.initialized || stations == null) return;
            block ??= new MaterialPropertyBlock();
            recorded ??= new bool[stations.Length];
            var game = hud.game;
            if (game.scans.Count != seenScans)
            {
                seenScans = game.scans.Count;
                for (int i = 0; i < stations.Length; i++)
                    recorded[i] = stations[i] && game.scans.Any(s => Vector3.Distance(s.location, stations[i].transform.position) <= 3);
            }
            var pos = game.prototype.ActivePosition();
            float t = Time.time;
            for (int i = 0; i < stations.Length; i++)
            {
                if (!stations[i]) continue;
                bool active = hud.scanning && Vector3.Distance(pos, stations[i].transform.position) <= 3;
                Color c;
                if (active) c = new Color(.2f, 1.4f, 1.8f) * Mathf.Lerp(.4f, 1.6f, .5f + .5f * Mathf.Sin(t * 12));
                else if (recorded[i]) c = new Color(.3f, 1.8f, .5f);
                else c = new Color(1.8f, .8f, .1f) * Mathf.Lerp(.25f, 1f, .5f + .5f * Mathf.Sin(t * 2.6f + i));
                if (i < lights.Length && lights[i]) { lights[i].GetPropertyBlock(block); block.SetColor(Emission, c); lights[i].SetPropertyBlock(block); }

                if (rings != null && i < rings.Length && rings[i])
                {
                    var ring = rings[i];
                    if (ring.gameObject.activeSelf != active) ring.gameObject.SetActive(active);
                    if (active)
                    {
                        float k = Mathf.Repeat(t * .8f, 1);
                        float d = Mathf.Lerp(.6f, 6f, k);
                        ring.localScale = new Vector3(d, ring.localScale.y, d);
                        var r = ring.GetComponent<Renderer>();
                        r.GetPropertyBlock(block);
                        block.SetColor(Emission, new Color(.2f, 1.4f, 1.8f) * (1 - k) * 1.5f);
                        block.SetColor(BaseColor, new Color(.2f, .8f, 1f, .5f * (1 - k)));
                        r.SetPropertyBlock(block);
                    }
                }
            }
        }
    }
}
