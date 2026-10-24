using System;
using UnityEngine;

namespace AstroGateway.Showroom
{
    /// <summary>
    /// AstroGateway Showroom Controller
    /// Manages real-time staging, animation triggering, cockpit docking, and performance
    /// telemetry for the ARES EVA MK-I astronaut alongside the ARES-VI rover.
    /// </summary>
    public class AresShowroomController : MonoBehaviour
    {
        [Header("Target Character & Rover References")]
        [SerializeField] private GameObject astronautInstance;
        [SerializeField] private GameObject roverInstance;
        [SerializeField] private Transform cockpitSeatAnchor;
        [SerializeField] private Transform groundStagingAnchor;

        [Header("Camera Rig Controls")]
        [SerializeField] private Camera showroomCamera;
        [SerializeField] private Transform cameraOrbitPivot;
        [SerializeField] private float orbitSpeed = 45f;
        [SerializeField] private float cameraDistance = 3.5f;

        [Header("Lighting Controls")]
        [SerializeField] private Light marsSunLight;
        [SerializeField] private Light[] astronautSpotlights;

        private Animator astronautAnimator;
        private LODGroup astronautLODGroup;
        private bool isSeatedInRover = false;
        private float currentYaw = 30f;
        private float currentPitch = 15f;
        private int forcedLOD = -1; // -1 = Automatic

        private static readonly int AnimHashState = Animator.StringToHash("CharacterState");
        private static readonly int AnimHashSpeed = Animator.StringToHash("ForwardSpeed");

        private void Awake()
        {
            if (astronautInstance != null)
            {
                astronautAnimator = astronautInstance.GetComponentInChildren<Animator>();
                astronautLODGroup = astronautInstance.GetComponentInChildren<LODGroup>();
            }

            if (showroomCamera == null)
            {
                showroomCamera = Camera.main;
            }
        }

        private void Update()
        {
            HandleInput();
            UpdateCameraOrbit();
        }

        private void HandleInput()
        {
            // Animation hotkeys
            if (Input.GetKeyDown(KeyCode.Alpha1)) PlayAnimation("Idle");
            if (Input.GetKeyDown(KeyCode.Alpha2)) PlayAnimation("Walk");
            if (Input.GetKeyDown(KeyCode.Alpha3)) PlayAnimation("Scan");
            if (Input.GetKeyDown(KeyCode.Alpha4)) PlayAnimation("Alarm");
            if (Input.GetKeyDown(KeyCode.Alpha5)) ToggleRoverCockpitDocking();

            // LOD Force Toggles (F1 to F4)
            if (Input.GetKeyDown(KeyCode.F1)) SetForcedLOD(0);
            if (Input.GetKeyDown(KeyCode.F2)) SetForcedLOD(1);
            if (Input.GetKeyDown(KeyCode.F3)) SetForcedLOD(2);
            if (Input.GetKeyDown(KeyCode.F4)) SetForcedLOD(3);
            if (Input.GetKeyDown(KeyCode.F5)) SetForcedLOD(-1); // Auto LOD

            // Night / Day Toggle
            if (Input.GetKeyDown(KeyCode.N)) ToggleNightMode();
        }

        public void PlayAnimation(string animName)
        {
            if (astronautAnimator == null) return;

            switch (animName)
            {
                case "Idle":
                    if (isSeatedInRover) ToggleRoverCockpitDocking();
                    astronautAnimator.CrossFade("ARES_EVA_Idle_Breathing", 0.2f);
                    break;
                case "Walk":
                    if (isSeatedInRover) ToggleRoverCockpitDocking();
                    astronautAnimator.CrossFade("ARES_EVA_Walk_Forward", 0.2f);
                    break;
                case "Scan":
                    if (isSeatedInRover) ToggleRoverCockpitDocking();
                    astronautAnimator.CrossFade("ARES_EVA_Scanner_Inspect", 0.2f);
                    break;
                case "Alarm":
                    astronautAnimator.CrossFade("ARES_EVA_Emergency_Alarm", 0.2f);
                    break;
                case "RoverSeated":
                    astronautAnimator.CrossFade("ARES_EVA_Rover_Seated", 0.2f);
                    break;
            }
        }

        public void ToggleRoverCockpitDocking()
        {
            isSeatedInRover = !isSeatedInRover;

            if (astronautInstance == null) return;

            if (isSeatedInRover)
            {
                if (cockpitSeatAnchor != null)
                {
                    astronautInstance.transform.SetParent(cockpitSeatAnchor, false);
                    astronautInstance.transform.localPosition = Vector3.zero;
                    astronautInstance.transform.localRotation = Quaternion.identity;
                }
                else if (roverInstance != null)
                {
                    // Default cockpit seat offset inside ARES-VI rover
                    astronautInstance.transform.SetParent(roverInstance.transform, false);
                    astronautInstance.transform.localPosition = new Vector3(-0.45f, 0.95f, 0.40f);
                    astronautInstance.transform.localRotation = Quaternion.identity;
                }

                if (astronautAnimator != null)
                {
                    astronautAnimator.CrossFade("ARES_EVA_Rover_Seated", 0.15f);
                }
            }
            else
            {
                if (groundStagingAnchor != null)
                {
                    astronautInstance.transform.SetParent(null, true);
                    astronautInstance.transform.position = groundStagingAnchor.position;
                    astronautInstance.transform.rotation = groundStagingAnchor.rotation;
                }
                else
                {
                    astronautInstance.transform.SetParent(null, true);
                    astronautInstance.transform.position = new Vector3(-1.8f, 0f, 0f);
                    astronautInstance.transform.rotation = Quaternion.Euler(0, 30f, 0);
                }

                if (astronautAnimator != null)
                {
                    astronautAnimator.CrossFade("ARES_EVA_Idle_Breathing", 0.2f);
                }
            }
        }

        public void SetForcedLOD(int lodLevel)
        {
            forcedLOD = lodLevel;
            if (astronautLODGroup != null)
            {
                if (lodLevel < 0)
                {
                    astronautLODGroup.ForceLOD(-1);
                }
                else
                {
                    astronautLODGroup.ForceLOD(lodLevel);
                }
            }
        }

        public void ToggleNightMode()
        {
            if (marsSunLight != null)
            {
                marsSunLight.intensity = marsSunLight.intensity > 0.5f ? 0.05f : 1.25f;
            }

            if (astronautSpotlights != null)
            {
                foreach (var light in astronautSpotlights)
                {
                    if (light != null) light.enabled = !light.enabled;
                }
            }
        }

        private void UpdateCameraOrbit()
        {
            if (cameraOrbitPivot == null || showroomCamera == null) return;

            if (Input.GetMouseButton(0))
            {
                currentYaw += Input.GetAxis("Mouse X") * orbitSpeed * 0.1f;
                currentPitch = Mathf.Clamp(currentPitch - Input.GetAxis("Mouse Y") * orbitSpeed * 0.1f, -10f, 75f);
            }

            cameraDistance = Mathf.Clamp(cameraDistance - Input.GetAxis("Mouse ScrollWheel") * 2.0f, 1.2f, 8.0f);

            Quaternion rot = Quaternion.Euler(currentPitch, currentYaw, 0);
            Vector3 pos = cameraOrbitPivot.position + rot * new Vector3(0, 0, -cameraDistance);

            showroomCamera.transform.position = pos;
            showroomCamera.transform.LookAt(cameraOrbitPivot.position + Vector3.up * 1.1f);
        }

        private void OnGUI()
        {
            // Showroom HUD Overlay
            GUILayout.BeginArea(new Rect(20, 20, 320, 380), GUI.skin.box);
            GUILayout.Label("<b>ASTROGATEWAY SHOWROOM</b>", GUI.skin.label);
            GUILayout.Label("ARES EVA MK-I + ARES-VI Staging", GUI.skin.label);
            GUILayout.Space(6);

            GUILayout.Label("<b>Animation Action Triggers:</b>");
            if (GUILayout.Button("[1] Idle Breathing")) PlayAnimation("Idle");
            if (GUILayout.Button("[2] Walk Forward (Mars 0.38g)")) PlayAnimation("Walk");
            if (GUILayout.Button("[3] Science Scanner Inspect")) PlayAnimation("Scan");
            if (GUILayout.Button("[4] Emergency Telemetry Alarm")) PlayAnimation("Alarm");
            if (GUILayout.Button(isSeatedInRover ? "[5] Disembark Rover (Stand)" : "[5] Board Rover Cockpit")) ToggleRoverCockpitDocking();

            GUILayout.Space(6);
            GUILayout.Label("<b>LOD & Visual Debug:</b>");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Auto")) SetForcedLOD(-1);
            if (GUILayout.Button("LOD0")) SetForcedLOD(0);
            if (GUILayout.Button("LOD1")) SetForcedLOD(1);
            if (GUILayout.Button("LOD2")) SetForcedLOD(2);
            if (GUILayout.Button("LOD3")) SetForcedLOD(3);
            GUILayout.EndHorizontal();

            GUILayout.Space(6);
            if (GUILayout.Button("Toggle Sol / Night (N)")) ToggleNightMode();

            GUILayout.Space(6);
            GUILayout.Label($"Active LOD: {(forcedLOD == -1 ? "Automatic (Camera Distance)" : "LOD" + forcedLOD)}");
            GUILayout.Label($"Framerate: {(1.0f / Time.smoothDeltaTime):0} FPS (Target 60)");
            GUILayout.EndArea();
        }
    }
}
