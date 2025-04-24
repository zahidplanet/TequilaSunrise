using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

namespace TequilaSunrise.AR
{
    /// <summary>
    /// Manages the AR scene components and setup
    /// </summary>
    public class ARSceneManager : MonoBehaviour
    {
        [Header("AR Components")]
        [SerializeField] private ARSession arSession;
        [SerializeField] private ARSessionOrigin arSessionOrigin;
        [SerializeField] private ARPlaneManager planeManager;
        [SerializeField] private ARRaycastManager raycastManager;
        [SerializeField] private AvatarPlacementManager placementManager;
        
        [Header("UI References")]
        [SerializeField] private Canvas mainCanvas;
        [SerializeField] private GameObject statusPanel;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private GameObject instructionsPanel;
        [SerializeField] private TextMeshProUGUI instructionsText;
        
        [Header("Settings")]
        [SerializeField] private ARSettings arSettings;
        [SerializeField] private bool autoInitialize = true;
        
        private void Awake()
        {
            // Find components if not assigned
            if (arSession == null)
                arSession = FindObjectOfType<ARSession>();
                
            if (arSessionOrigin == null)
                arSessionOrigin = FindObjectOfType<ARSessionOrigin>();
                
            if (planeManager == null)
                planeManager = FindObjectOfType<ARPlaneManager>();
                
            if (raycastManager == null)
                raycastManager = FindObjectOfType<ARRaycastManager>();
            
            if (placementManager == null)
                placementManager = FindObjectOfType<AvatarPlacementManager>();
                
            // Load settings if needed
            if (arSettings == null)
                arSettings = ARSettings.Instance;
        }
        
        private void Start()
        {
            if (autoInitialize)
            {
                InitializeAR();
            }
            
            UpdateStatusText("Initializing AR session...");
            ShowInstructions("Scan your surroundings to detect surfaces");
        }
        
        public void InitializeAR()
        {
            // Apply settings
            if (arSettings != null)
            {
                if (arSession != null)
                {
                    arSettings.ApplySettingsToSession(arSession);
                }
                
                if (planeManager != null)
                {
                    arSettings.ApplySettingsToPlaneManager(planeManager);
                    planeManager.enabled = arSettings.enablePlaneDetection;
                }
            }
            
            // Subscribe to AR session state changes
            if (arSession != null)
            {
                arSession.stateChanged += OnARSessionStateChanged;
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (arSession != null)
            {
                arSession.stateChanged -= OnARSessionStateChanged;
            }
        }
        
        private void OnARSessionStateChanged(ARSessionStateChangedEventArgs args)
        {
            string stateMessage = "";
            
            switch (args.state)
            {
                case ARSessionState.None:
                    stateMessage = "AR session not initialized";
                    break;
                case ARSessionState.Unsupported:
                    stateMessage = "AR not supported on this device";
                    break;
                case ARSessionState.CheckingAvailability:
                    stateMessage = "Checking AR availability...";
                    break;
                case ARSessionState.NeedsInstall:
                    stateMessage = "AR needs additional software";
                    break;
                case ARSessionState.Installing:
                    stateMessage = "Installing AR software...";
                    break;
                case ARSessionState.Ready:
                    stateMessage = "AR session ready";
                    ShowInstructions("Point your camera at flat surfaces");
                    break;
                case ARSessionState.SessionInitializing:
                    stateMessage = "Initializing AR session...";
                    break;
                case ARSessionState.SessionTracking:
                    stateMessage = "AR tracking active";
                    if (planeManager && planeManager.trackables.count > 0)
                    {
                        ShowInstructions("Tap on a surface to place your avatar");
                    }
                    else
                    {
                        ShowInstructions("Move your device to scan the environment");
                    }
                    break;
            }
            
            UpdateStatusText(stateMessage);
        }
        
        private void UpdateStatusText(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
            
            if (statusPanel != null)
            {
                statusPanel.SetActive(arSettings == null || arSettings.showSessionStatus);
            }
            
            Debug.Log($"AR Status: {message}");
        }
        
        private void ShowInstructions(string message)
        {
            if (instructionsText != null)
            {
                instructionsText.text = message;
            }
            
            if (instructionsPanel != null)
            {
                instructionsPanel.SetActive(arSettings == null || arSettings.showPlaneInstructions);
            }
        }
        
        public void RestartSession()
        {
            if (arSession != null)
            {
                arSession.Reset();
                UpdateStatusText("Restarting AR session...");
            }
            
            if (placementManager != null)
            {
                placementManager.ResetPlacement();
            }
        }
        
        public void TogglePlaneVisualization(bool show)
        {
            if (planeManager != null)
            {
                foreach (var plane in planeManager.trackables)
                {
                    plane.gameObject.SetActive(show);
                }
            }
        }
    }
} 