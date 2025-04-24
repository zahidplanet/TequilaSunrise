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
        [Header("AR Components")]
        [SerializeField] private ARSession arSession;
        [SerializeField] private ARSessionOrigin sessionOrigin;
        [SerializeField] private ARPlaneManager planeManager;
        [SerializeField] private ARRaycastManager raycastManager;
        
        [Header("AR Settings")]
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
            if (!arSession) arSession = GetComponent<ARSession>();
            if (!sessionOrigin) sessionOrigin = GetComponent<ARSessionOrigin>();
            if (!planeManager) planeManager = GetComponent<ARPlaneManager>();
            if (!raycastManager) raycastManager = GetComponent<ARRaycastManager>();
            
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
            if (autoFocusOnStart)
            {
                ConfigureAutoFocus();
            }
            
            planeManager.enabled = enablePlaneDetection;
            planeManager.requestedDetectionMode = planeDetectionMode;
            
            isInitialized = true;
            onSessionInitialized?.Invoke();
        }
        
        private void ConfigureAutoFocus()
        {
            var cameraManager = sessionOrigin.camera.GetComponent<ARCameraManager>();
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
        }
        
        private void UnsubscribeFromEvents()
        {
            if (planeManager != null)
            {
                planeManager.planesChanged -= OnPlanesChanged;
            }
        }
        
        private void OnPlanesChanged(ARPlanesChangedEventArgs args)
        {
            foreach (ARPlane plane in args.added)
            {
                onPlaneDetected?.Invoke(plane);
            }
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Performs a raycast against detected AR planes
        /// </summary>
        /// <param name="screenPoint">Screen point to raycast from</param>
        /// <returns>True if hit detected, false otherwise</returns>
        public bool Raycast(Vector2 screenPoint, out ARRaycastHit hitResult)
        {
            if (raycastManager.Raycast(screenPoint, raycastHits, TrackableType.PlaneWithinPolygon))
            {
                hitResult = raycastHits[0];
                return true;
            }
            
            hitResult = default;
            return false;
        }
        
        /// <summary>
        /// Resets the AR session
        /// </summary>
        public void ResetSession()
        {
            arSession.Reset();
            ConfigureARSession();
        }
        
        /// <summary>
        /// Toggles plane detection
        /// </summary>
        public void TogglePlaneDetection(bool enable)
        {
            if (planeManager != null)
            {
                planeManager.enabled = enable;
                enablePlaneDetection = enable;
            }
        }
        
        /// <summary>
        /// Gets the camera pose in world space
        /// </summary>
        public Pose GetCameraPose()
        {
            return new Pose(sessionOrigin.camera.transform.position, 
                          sessionOrigin.camera.transform.rotation);
        }
        
        #endregion
    }
}