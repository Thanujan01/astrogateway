using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using AstroGateway.CinematicHUD;
using AstroGateway.ProductionVisual;

namespace AstroGateway.VisualUpgrade
{
    // Presentation-only atmosphere for Mars_VisualUpgrade_v1: keeps the sky's sun disk aligned with the directional
    // light, follows the player with the distant dust haze, enables depth of field only in the cinematic overview,
    // and refreshes the realtime reflection probe once after start. No gameplay state is read for decisions.
    [DefaultExecutionOrder(610)]
    public class VisualUpgradeAtmosphere : MonoBehaviour
    {
        public AstroGatewayHUDController hud;
        public ProductionModeCoordinator modes;
        public Volume volume;
        public ParticleSystem haze;
        public ReflectionProbe probe;
        public Transform farTerrain;
        static readonly int SunDir = Shader.PropertyToID("_SunDir");
        DepthOfField dof;
        float dofWeight;
        int lastQuality = -1;

        void Start()
        {
            if (volume && volume.profile.TryGet(out dof)) { dof.active = false; }
            if (probe) probe.RenderProbe();
        }

        void LateUpdate()
        {
            var sun = hud.environment.sun;
            var sky = RenderSettings.skybox;
            if (sun && sky && sky.HasProperty(SunDir)) sky.SetVector(SunDir, -sun.transform.forward);

            var p = hud.game.prototype;
            if (haze) haze.transform.position = new Vector3(p.ActivePosition().x, 0, p.ActivePosition().z);

            // Cinematic depth of field (Gaussian, far background softening), eased in and out.
            if (dof != null)
            {
                bool want = modes.Current == ProductionMode.Cinematic && hud.environment.quality >= 2;
                dofWeight = Mathf.MoveTowards(dofWeight, want ? 1 : 0, Time.unscaledDeltaTime * 1.5f);
                dof.active = dofWeight > 0.01f;
                dof.gaussianMaxRadius.value = Mathf.Lerp(0.1f, 1f, dofWeight);
            }

            if (lastQuality != hud.environment.quality)
            {
                lastQuality = hud.environment.quality;
                if (haze) { var e = haze.emission; e.rateOverTimeMultiplier = new[] { 0f, 1.5f, 3f, 4f }[lastQuality]; }
                if (probe) probe.RenderProbe();
            }
        }
    }
}
