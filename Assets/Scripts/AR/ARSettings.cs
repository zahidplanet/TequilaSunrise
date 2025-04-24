using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace TequilaSunrise.AR
{
    [CreateAssetMenu(fileName = "ARSettings", menuName = "TequilaSunrise/AR/Settings")]
    public class ARSettings : ScriptableObject
    {
        [Header("AR Session Settings")]
        [Tooltip("Whether to attempt auto-focus on session start")]
        public bool autoFocus = true;

        [Tooltip("Whether to match the frame rate to the display")]
        public bool matchFrameRate = true;

        [Header("Plane Detection Settings")]
        [Tooltip("Whether to enable plane detection")]
        public bool enablePlaneDetection = true;

        [Tooltip("Minimum plane size in square meters")]
        public float minPlaneSize = 0.2f;

        [Header("Point Cloud Settings")]
        [Tooltip("Whether to enable point cloud visualization")]
        public bool enablePointCloud = false;

        [Tooltip("Point cloud visualization prefab")]
        public GameObject pointCloudPrefab;

        [Header("Image Tracking Settings")]
        [Tooltip("Whether to enable image tracking")]
        public bool enableImageTracking = false;

        [Tooltip("Maximum number of moving images to track")]
        public int maxNumMovingImages = 4;

        [Header("Performance Settings")]
        [Tooltip("Target frame rate for AR session")]
        public int targetFrameRate = 60;

        [Tooltip("Whether to enable CPU images")]
        public bool enableCpuImages = false;

        [Header("UI Settings")]
        [Tooltip("Whether to show AR session status")]
        public bool showSessionStatus = true;

        [Tooltip("Whether to show plane detection instructions")]
        public bool showPlaneInstructions = true;

        [Header("Debug Settings")]
        [Tooltip("Whether to enable debug logging")]
        public bool enableDebugLogging = false;

        [Tooltip("Whether to show debug visualizations")]
        public bool showDebugVisuals = false;

        private static ARSettings instance;
        public static ARSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<ARSettings>("ARSettings");
                    if (instance == null)
                    {
                        Debug.LogWarning("ARSettings not found in Resources folder. Using default settings.");
                        instance = CreateInstance<ARSettings>();
                    }
                }
                return instance;
            }
        }

        public void ApplySettingsToSession(ARSession session)
        {
            if (session == null) return;

            session.attemptUpdate = true;
            session.matchFrameRate = matchFrameRate;
            
            var subsystems = session.subsystem;
            if (subsystems != null)
            {
                if (autoFocus)
                {
                    subsystems.focusMode = UnityEngine.XR.ARSubsystems.CameraFocusMode.Auto;
                }
                
                Application.targetFrameRate = targetFrameRate;
            }
        }

        public void ApplySettingsToPlaneManager(ARPlaneManager planeManager)
        {
            if (planeManager == null) return;

            planeManager.enabled = enablePlaneDetection;
        }

        public void ResetToDefaults()
        {
            autoFocus = true;
            matchFrameRate = true;
            enablePlaneDetection = true;
            minPlaneSize = 0.2f;
            enablePointCloud = false;
            pointCloudPrefab = null;
            enableImageTracking = false;
            maxNumMovingImages = 4;
            targetFrameRate = 60;
            enableCpuImages = false;
            showSessionStatus = true;
            showPlaneInstructions = true;
            enableDebugLogging = false;
            showDebugVisuals = false;
        }
    }
}