using UnityEngine;

namespace AstroGateway.VisualUpgrade
{
    // Blinks or pulses emissive status lights (stations, comms beacon, habitat markers). Presentation only.
    public class VisualUpgradeBlink : MonoBehaviour
    {
        public Renderer[] targets;
        public Color color = new Color(.2f, 1.4f, 1.8f);
        public float period = 2f, onFraction = .15f, offLevel = .15f, offset;
        public bool pulse;
        static readonly int Emission = Shader.PropertyToID("_EmissionColor");
        MaterialPropertyBlock block;

        void LateUpdate()
        {
            block ??= new MaterialPropertyBlock();
            float t = Mathf.Repeat(Time.time + offset, period) / period;
            float level = pulse ? Mathf.Lerp(offLevel, 1, .5f + .5f * Mathf.Sin(t * Mathf.PI * 2)) : (t < onFraction ? 1 : offLevel);
            foreach (var r in targets)
            {
                if (!r) continue;
                r.GetPropertyBlock(block); block.SetColor(Emission, color * level); r.SetPropertyBlock(block);
            }
        }
    }
}
