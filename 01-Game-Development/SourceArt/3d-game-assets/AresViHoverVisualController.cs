using System;
using UnityEngine;

namespace AstroGateway.Vehicles
{
    /// <summary>
    /// ARES-VI Advanced Anti-Gravity Mars Research Rover
    /// Unity 6 (6000.6.4f1) URP Visual Controller
    /// 
    /// SAFETY & NON-DESTRUCTIVE INTEGRATION GUARANTEE:
    /// - This component handles purely visual and aesthetic transformations (Mode A Ground <-> Mode B Hover).
    /// - Does NOT modify WheelColliders, Rigidbody mass properties, or AstroGateway gameplay physics.
    /// - Designed to be attached to the ARES_VI_Upgraded_Variant prefab in an isolated showroom scene.
    /// </summary>
    [DisallowMultipleComponent]
    [SelectionBase]
    public class AresViHoverVisualController : MonoBehaviour
    {
        public enum VehicleMode
        {
            GroundExploration, // Mode A: 6-Wheel Rocker-Bogie active on terrain
            AntiGravityHover   // Mode B: Underbody hover modules deployed with cyan plasma emission
        }

        [Header("Operational Configuration")]
        [SerializeField] private VehicleMode currentMode = VehicleMode.GroundExploration;
        [SerializeField, Range(0.1f, 5.0f)] private float transitionSpeed = 1.8f;

        [Header("Transform Hierarchies")]
        [Tooltip("Root vehicle visual chassis to lift during hover")]
        [SerializeField] private Transform vehicleVisualBody;
        [Tooltip("The 6 exploration wheel assemblies to retract/lift slightly during hover")]
        [SerializeField] private Transform[] wheelAssemblies = new Transform[6];
        [Tooltip("Underbody anti-gravity emitter modules (4 primary + 2 aux)")]
        [SerializeField] private Transform[] hoverEmitterModules = new Transform[6];

        [Header("Hover Physics Simulation (Visual Only)")]
        [SerializeField] private float hoverHeightOffset = 0.32f;
        [SerializeField] private float wheelRetractionHeight = 0.18f;
        [SerializeField] private float hoverBobFrequency = 2.4f;
        [SerializeField] private float hoverBobAmplitude = 0.045f;
        [SerializeField] private float hoverTiltLag = 1.2f;

        [Header("VFX & Emission Materials")]
        [SerializeField] private Material cyanEmissionMaterial;
        [SerializeField] private Color groundEmissionColor = new Color(0f, 0.2f, 0.3f, 1f);
        [SerializeField] private Color hoverEmissionColor = new Color(0f, 0.92f, 1.0f, 1f);
        [SerializeField] private float hoverEmissionIntensity = 8.5f;
        [SerializeField] private Light underbodyGroundLight;
        [SerializeField] private ParticleSystem[] thrusterDustParticles;

        [Header("Audio Hooks (Optional)")]
        [SerializeField] private AudioSource propulsionAudioSource;
        [SerializeField] private AudioClip hoverEngageSfx;
        [SerializeField] private AudioClip hoverDisengageSfx;

        // Internal State Lerps
        private float _hoverBlend = 0f; // 0 = Ground, 1 = Hover
        private Vector3 _bodyInitialLocalPos;
        private Vector3[] _wheelInitialLocalPositions;
        private Vector3[] _emitterInitialLocalPositions;
        private MaterialPropertyBlock _propBlock;
        private static readonly int EmissionColorProp = Shader.PropertyToID("_EmissionColor");

        private void Awake()
        {
            _propBlock = new MaterialPropertyBlock();

            if (vehicleVisualBody != null)
            {
                _bodyInitialLocalPos = vehicleVisualBody.localPosition;
            }

            if (wheelAssemblies != null && wheelAssemblies.Length > 0)
            {
                _wheelInitialLocalPositions = new Vector3[wheelAssemblies.Length];
                for (int i = 0; i < wheelAssemblies.Length; i++)
                {
                    if (wheelAssemblies[i] != null)
                    {
                        _wheelInitialLocalPositions[i] = wheelAssemblies[i].localPosition;
                    }
                }
            }

            if (hoverEmitterModules != null && hoverEmitterModules.Length > 0)
            {
                _emitterInitialLocalPositions = new Vector3[hoverEmitterModules.Length];
                for (int i = 0; i < hoverEmitterModules.Length; i++)
                {
                    if (hoverEmitterModules[i] != null)
                    {
                        _emitterInitialLocalPositions[i] = hoverEmitterModules[i].localPosition;
                    }
                }
            }
        }

        private void Update()
        {
            // Target blend based on active mode
            float targetBlend = (currentMode == VehicleMode.AntiGravityHover) ? 1.0f : 0.0f;
            _hoverBlend = Mathf.MoveTowards(_hoverBlend, targetBlend, Time.deltaTime * transitionSpeed);

            UpdateBodyHoverAnimation();
            UpdateWheelTucking();
            UpdateEmitterEmissions();
        }

        private void UpdateBodyHoverAnimation()
        {
            if (vehicleVisualBody == null) return;

            // Sinusoidal floating oscillation
            float bob = (_hoverBlend > 0.05f) 
                ? Mathf.Sin(Time.time * hoverBobFrequency) * hoverBobAmplitude * _hoverBlend 
                : 0f;

            Vector3 targetPos = _bodyInitialLocalPos + Vector3.up * (_hoverBlend * hoverHeightOffset + bob);
            vehicleVisualBody.localPosition = targetPos;
        }

        private void UpdateWheelTucking()
        {
            if (wheelAssemblies == null || _wheelInitialLocalPositions == null) return;

            for (int i = 0; i < wheelAssemblies.Length; i++)
            {
                if (wheelAssemblies[i] == null) continue;
                // Wheels tuck up slightly during hover to clear terrain
                Vector3 targetWheelPos = _wheelInitialLocalPositions[i] + Vector3.up * (_hoverBlend * wheelRetractionHeight);
                wheelAssemblies[i].localPosition = targetWheelPos;
            }
        }

        private void UpdateEmitterEmissions()
        {
            // Modulate underbody spotlight intensity
            if (underbodyGroundLight != null)
            {
                underbodyGroundLight.intensity = Mathf.Lerp(0f, 6.5f, _hoverBlend);
                underbodyGroundLight.enabled = _hoverBlend > 0.02f;
            }

            // Modulate Material Emission color
            if (cyanEmissionMaterial != null)
            {
                Color targetCol = Color.Lerp(groundEmissionColor, hoverEmissionColor * hoverEmissionIntensity, _hoverBlend);
                cyanEmissionMaterial.SetColor(EmissionColorProp, targetCol);
            }

            // Particle dust emission toggle
            if (thrusterDustParticles != null)
            {
                bool emitDust = _hoverBlend > 0.5f;
                foreach (var ps in thrusterDustParticles)
                {
                    if (ps == null) continue;
                    var emission = ps.emission;
                    emission.enabled = emitDust;
                }
            }
        }

        /// <summary>
        /// Public API: Safely toggle vehicle mode without interfering with standard gameplay.
        /// </summary>
        [ContextMenu("Toggle Vehicle Mode")]
        public void ToggleMode()
        {
            SetMode(currentMode == VehicleMode.GroundExploration ? VehicleMode.AntiGravityHover : VehicleMode.GroundExploration);
        }

        public void SetMode(VehicleMode newMode)
        {
            if (currentMode == newMode) return;
            currentMode = newMode;

            if (propulsionAudioSource != null)
            {
                AudioClip clip = (currentMode == VehicleMode.AntiGravityHover) ? hoverEngageSfx : hoverDisengageSfx;
                if (clip != null) propulsionAudioSource.PlayOneShot(clip);
            }
        }
    }
}
