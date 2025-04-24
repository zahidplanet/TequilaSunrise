using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace TequilaSunrise.AR
{
    [RequireComponent(typeof(ARSession))]
    public class ARSessionManager : MonoBehaviour
    {
        public static ARSessionManager Instance { get; private set; }

        [Header("AR Components")]
        [SerializeField] private ARSession arSession;
        [SerializeField] private ARSessionOrigin sessionOrigin;
        [SerializeField] private ARPlaneManager planeManager;
        [SerializeField] private ARRaycastManager raycastManager;
        [SerializeField] private ARCameraManager cameraManager;

        [Header("Settings")]
        [SerializeField] private ARSettings settings;

        private bool isInitialized = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            Initialize();
        }

        private void Initialize()
        {
            if (isInitialized) return;

            // Get components if not assigned
            if (arSession == null) arSession = GetComponent<ARSession>();
            if (sessionOrigin == null) sessionOrigin = FindObjectOfType<ARSessionOrigin>();
            if (planeManager == null) planeManager = FindObjectOfType<ARPlaneManager>();
            if (raycastManager == null) raycastManager = FindObjectOfType<ARRaycastManager>();
            if (cameraManager == null) cameraManager = sessionOrigin?.camera?.GetComponent<ARCameraManager>();

            // Load settings
            if (settings == null) settings = ARSettings.Instance;

            ConfigureARSession();
            isInitialized = true;
        }

        private void ConfigureARSession()
        {
            if (settings == null) return;

            // Configure plane detection
            if (planeManager != null)
            {
                planeManager.enabled = settings.enablePlaneDetection;
                var planeDetectionMode = PlaneDetectionMode.None;
                if (settings.enableHorizontalPlanes) planeDetectionMode |= PlaneDetectionMode.Horizontal;
                if (settings.enableVerticalPlanes) planeDetectionMode |= PlaneDetectionMode.Vertical;
                planeManager.requestedDetectionMode = planeDetectionMode;
            }

            // Configure camera
            if (cameraManager != null)
            {
                cameraManager.requestedFocusMode = settings.autoFocus ? 
                    UnityEngine.XR.ARSubsystems.CameraFocusMode.Auto : 
                    UnityEngine.XR.ARSubsystems.CameraFocusMode.Fixed;
                
                cameraManager.requestedLightEstimation = settings.enableLightEstimation ? 
                    LightEstimation.AmbientIntensity : 
                    LightEstimation.None;
            }
        }

        public void RestartSession()
        {
            if (arSession != null)
            {
                arSession.Reset();
                ConfigureARSession();
            }
        }

        public bool Raycast(Vector2 screenPoint, out ARRaycastHit hitResult)
        {
            hitResult = default;
            if (raycastManager == null) return false;

            var hits = new System.Collections.Generic.List<ARRaycastHit>();
            if (raycastManager.Raycast(screenPoint, hits, TrackableType.All))
            {
                hitResult = hits[0];
                return true;
            }

            return false;
        }

        private void OnEnable()
        {
            if (cameraManager != null)
                cameraManager.frameReceived += OnCameraFrameReceived;
        }

        private void OnDisable()
        {
            if (cameraManager != null)
                cameraManager.frameReceived -= OnCameraFrameReceived;
        }

        private void OnCameraFrameReceived(ARCameraFrameEventArgs args)
        {
            // Handle camera frame updates if needed
            if (settings.showLogs)
            {
                Debug.Log($"Camera frame received. Light estimation: {args.lightEstimation.averageBrightness}");
            }
        }
    }
}