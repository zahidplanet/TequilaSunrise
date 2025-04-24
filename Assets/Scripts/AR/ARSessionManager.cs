using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;
using UnityEngine.Events;

namespace TequilaSunrise.AR
{
    /// <summary>
    /// Manages the AR session and core AR functionality
    /// </summary>
    [RequireComponent(typeof(ARSession))]
    [RequireComponent(typeof(ARSessionOrigin))]
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
        [SerializeField] private bool autoFocusOnStart = true;
        [SerializeField] private bool enablePlaneDetection = true;
        [SerializeField] private PlaneDetectionMode planeDetectionMode = PlaneDetectionMode.Horizontal;
        
        [Header("Events")]
        public UnityEvent onSessionInitialized;
        public UnityEvent onSessionError;
        public UnityEvent<ARPlane> onPlaneDetected;
        
        private bool isInitialized = false;
        private List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();
        
        #region Unity Methods
        
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
            
            SetupComponents();
        }
        
        private void Start()
        {
            ConfigureARSession();
        }
        
        private void OnEnable()
        {
            SubscribeToEvents();
        }
        
        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }
        
        #endregion
        
        #region Setup Methods
        
        private void SetupComponents()
        {
            // Get components if not assigned
            if (arSession == null) arSession = GetComponent<ARSession>();
            if (sessionOrigin == null) sessionOrigin = GetComponent<ARSessionOrigin>();
            if (planeManager == null) planeManager = GetComponent<ARPlaneManager>();
            if (raycastManager == null) raycastManager = GetComponent<ARRaycastManager>();
            if (cameraManager == null) cameraManager = sessionOrigin?.camera?.GetComponent<ARCameraManager>();
            
            // Load settings
            if (settings == null) settings = ARSettings.Instance;
            
            ValidateSetup();
        }
        
        private void ValidateSetup()
        {
            if (!arSession || !sessionOrigin || !planeManager || !raycastManager)
            {
                Debug.LogError("ARSessionManager: Missing required AR components!");
                enabled = false;
                return;
            }
        }
        
        private void ConfigureARSession()
        {
            if (settings != null)
            {
                // Apply settings from ARSettings scriptable object
                autoFocusOnStart = settings.autoFocus;
                enablePlaneDetection = settings.enablePlaneDetection;
                
                var planeDetectionMode = PlaneDetectionMode.None;
                if (settings.enablePlaneDetection) 
                {
                    // Support for both horizontal and vertical planes
                    planeDetectionMode |= PlaneDetectionMode.Horizontal;
                    // Optionally enable vertical planes if needed
                    // planeDetectionMode |= PlaneDetectionMode.Vertical;
                }
                this.planeDetectionMode = planeDetectionMode;
            }
            
            if (autoFocusOnStart)
            {
                ConfigureAutoFocus();
            }
            
            if (planeManager != null)
            {
                planeManager.enabled = enablePlaneDetection;
                planeManager.requestedDetectionMode = planeDetectionMode;
            }
            
            isInitialized = true;
            onSessionInitialized?.Invoke();
        }
        
        private void ConfigureAutoFocus()
        {
            if (cameraManager != null)
            {
                cameraManager.focusMode = UnityEngine.XR.ARSubsystems.CameraFocusMode.Auto;
            }
        }
        
        #endregion
        
        #region Event Handling
        
        private void SubscribeToEvents()
        {
            if (planeManager != null)
            {
                planeManager.planesChanged += OnPlanesChanged;
            }
            
            if (cameraManager != null)
            {
                cameraManager.frameReceived += OnCameraFrameReceived;
            }
        }
        
        private void UnsubscribeFromEvents()
        {
            if (planeManager != null)
            {
                planeManager.planesChanged -= OnPlanesChanged;
            }
            
            if (cameraManager != null)
            {
                cameraManager.frameReceived -= OnCameraFrameReceived;
            }
        }
        
        private void OnPlanesChanged(ARPlanesChangedEventArgs args)
        {
            foreach (var plane in args.added)
            {
                onPlaneDetected?.Invoke(plane);
                if (settings != null && settings.enableDebugLogging)
                {
                    Debug.Log($"Plane detected: {plane.trackableId} - {plane.alignment}");
                }
            }
        }
        
        private void OnCameraFrameReceived(ARCameraFrameEventArgs args)
        {
            // Handle camera frame updates if needed
            if (settings != null && settings.enableDebugLogging)
            {
                Debug.Log($"Camera frame received. Light estimation: {args.lightEstimation.averageBrightness}");
            }
        }
        
        #endregion
        
        #region Public Methods
        
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
            
            raycastHits.Clear();
            if (raycastManager.Raycast(screenPoint, raycastHits, TrackableType.All))
            {
                hitResult = raycastHits[0];
                return true;
            }
            
            return false;
        }
        
        public void TogglePlaneVisualization(bool visible)
        {
            if (planeManager != null)
            {
                foreach (var plane in planeManager.trackables)
                {
                    plane.gameObject.SetActive(visible);
                }
            }
        }
        
        #endregion
    }
}