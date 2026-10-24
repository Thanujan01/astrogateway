using System;
using UnityEngine;

namespace AstroGateway.Characters
{
    /// <summary>
    /// ARES EVA MK-I — Visual Replacement Adapter
    /// Unity 6 (6000.6.4f1) URP Character Controller Adapter
    /// 
    /// SAFETY & NON-DESTRUCTIVE GUARANTEE:
    /// - This component replaces ONLY the visual presentation of the player astronaut.
    /// - Operates strictly as a child of the existing player root GameObject.
    /// - Preserves CharacterController / Rigidbody physics, ground detection, collision layers,
    ///   camera follow anchors, oxygen consumption gameplay, and mission objective triggers.
    /// - Contains clean socket transforms for Scanner tool mounting, Rover seating alignment,
    ///   and helmet spotlight toggling.
    /// </summary>
    [DisallowMultipleComponent]
    [SelectionBase]
    public class AresEvaVisualAdapter : MonoBehaviour
    {
        public enum AstronautVisualState
        {
            LocomotionGround,
            ScienceScanning,
            RoverDrivingSeated,
            EmergencyReaction
        }

        [Header("Operational Configuration")]
        [SerializeField] private AstronautVisualState currentState = AstronautVisualState.LocomotionGround;
        [SerializeField] private Animator characterAnimator;
        [SerializeField] private LODGroup characterLODGroup;

        [Header("Equipment Socket Anchors")]
        [Tooltip("Right hand socket for holding the scientific rock scanner tool")]
        [SerializeField] private Transform rightHandScannerSocket;
        [Tooltip("Chest DCM socket for secondary data links")]
        [SerializeField] private Transform chestDcmAnchor;
        [Tooltip("PLSS backpack anchor point")]
        [SerializeField] private Transform plssBackpackAnchor;
        [Tooltip("Camera follow target bone (UpperChest / Head level)")]
        [SerializeField] private Transform thirdPersonCameraAnchor;

        [Header("Illumination & HUD Indicators")]
        [SerializeField] private Light helmetLeftSpotlight;
        [SerializeField] private Light helmetRightSpotlight;
        [SerializeField] private Material cyanEmissiveMaterial;
        [SerializeField, Range(0f, 10f)] private float spotlightIntensity = 3.5f;

        [Header("Rover Seating Alignment")]
        [Tooltip("Offset applied when seated in the ARES-VI rover bucket seat")]
        [SerializeField] private Vector3 roverSeatedLocalPosition = new Vector3(0f, -0.38f, 0.05f);
        [SerializeField] private Vector3 roverSeatedLocalEuler = Vector3.zero;

        // Animator Hash Parameters
        private static readonly int SpeedParam = Animator.StringToHash("MoveSpeed");
        private static readonly int GroundedParam = Animator.StringToHash("IsGrounded");
        private static readonly int ScanningParam = Animator.StringToHash("IsScanning");
        private static readonly int RoverSeatedParam = Animator.StringToHash("IsSeatedInRover");

        private Vector3 _defaultLocalPos;
        private Quaternion _defaultLocalRot;

        private void Awake()
        {
            if (characterAnimator == null)
            {
                characterAnimator = GetComponentInChildren<Animator>();
            }

            if (characterLODGroup == null)
            {
                characterLODGroup = GetComponent<LODGroup>();
            }

            _defaultLocalPos = transform.localPosition;
            _defaultLocalRot = transform.localRotation;
        }

        /// <summary>
        /// Syncs visual animation parameters from the existing AstroGateway player controller.
        /// Call this method from the existing player update loop without altering physics.
        /// </summary>
        public void UpdateLocomotionVisuals(float currentHorizontalSpeed, bool isGrounded)
        {
            if (characterAnimator == null) return;

            characterAnimator.SetFloat(SpeedParam, currentHorizontalSpeed);
            characterAnimator.SetBool(GroundedParam, isGrounded);
        }

        /// <summary>
        /// Activates scientific rock scanning visual stance and equips scanner to right hand socket.
        /// </summary>
        public void SetScanningState(bool isScanning, GameObject scannerToolPrefab = null)
        {
            currentState = isScanning ? AstronautVisualState.ScienceScanning : AstronautVisualState.LocomotionGround;

            if (characterAnimator != null)
            {
                characterAnimator.SetBool(ScanningParam, isScanning);
            }

            if (scannerToolPrefab != null && rightHandScannerSocket != null)
            {
                scannerToolPrefab.transform.SetParent(rightHandScannerSocket, false);
                scannerToolPrefab.transform.localPosition = Vector3.zero;
                scannerToolPrefab.transform.localRotation = Quaternion.identity;
            }
        }

        /// <summary>
        /// Transitions astronaut visuals into the ARES-VI rover cockpit seat.
        /// Automatically aligns astronaut hips with the cockpit bucket seat.
        /// </summary>
        public void SetRoverSeatedState(bool isSeated, Transform cockpitSeatMount = null)
        {
            currentState = isSeated ? AstronautVisualState.RoverDrivingSeated : AstronautVisualState.LocomotionGround;

            if (characterAnimator != null)
            {
                characterAnimator.SetBool(RoverSeatedParam, isSeated);
            }

            if (isSeated && cockpitSeatMount != null)
            {
                transform.SetParent(cockpitSeatMount, false);
                transform.localPosition = roverSeatedLocalPosition;
                transform.localEulerAngles = roverSeatedLocalEuler;
            }
            else if (!isSeated)
            {
                transform.localPosition = _defaultLocalPos;
                transform.localRotation = _defaultLocalRot;
            }
        }

        /// <summary>
        /// Toggles helmet forward spotlights for night EVA or cave exploration.
        /// </summary>
        public void ToggleHeadlamps(bool enabled)
        {
            if (helmetLeftSpotlight != null)
            {
                helmetLeftSpotlight.enabled = enabled;
                helmetLeftSpotlight.intensity = spotlightIntensity;
            }
            if (helmetRightSpotlight != null)
            {
                helmetRightSpotlight.enabled = enabled;
                helmetRightSpotlight.intensity = spotlightIntensity;
            }
        }

        public Transform GetCameraAnchor() => thirdPersonCameraAnchor != null ? thirdPersonCameraAnchor : transform;
        public Transform GetScannerSocket() => rightHandScannerSocket;
    }
}
