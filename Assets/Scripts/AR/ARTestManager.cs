using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using System.Collections.Generic;

namespace TequilaSunrise.AR
{
    [RequireComponent(typeof(ARSessionOrigin))]
    public class ARTestManager : MonoBehaviour
    {
        [Header("AR Components")]
        [SerializeField] private ARSession arSession;
        [SerializeField] private ARPlaneManager planeManager;
        [SerializeField] private ARRaycastManager raycastManager;

        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private GameObject instructionsPanel;
        [SerializeField] private GameObject debugPanel;

        private bool isInitialized = false;
        private List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();

        private void Awake()
        {
            if (!ValidateComponents()) return;
            
            // Subscribe to session state changes
            ARSession.stateChanged += OnARSessionStateChanged;
        }

        private void OnDestroy()
        {
            ARSession.stateChanged -= OnARSessionStateChanged;
        }

        private bool ValidateComponents()
        {
            if (arSession == null)
            {
                Debug.LogError("ARSession not assigned!");
                return false;
            }

            if (planeManager == null)
            {
                planeManager = FindObjectOfType<ARPlaneManager>();
                if (planeManager == null)
                {
                    Debug.LogError("ARPlaneManager not found in scene!");
                    return false;
                }
            }

            if (raycastManager == null)
            {
                raycastManager = FindObjectOfType<ARRaycastManager>();
                if (raycastManager == null)
                {
                    Debug.LogError("ARRaycastManager not found in scene!");
                    return false;
                }
            }

            return true;
        }

        private void OnARSessionStateChanged(ARSessionStateChangedEventArgs args)
        {
            UpdateStatusText(args.state);
            
            switch (args.state)
            {
                case ARSessionState.None:
                case ARSessionState.CheckingAvailability:
                    ShowInstructions(false);
                    break;
                    
                case ARSessionState.NeedsInstall:
                    UpdateStatusText("AR software installation required");
                    break;
                    
                case ARSessionState.Installing:
                    UpdateStatusText("Installing AR software...");
                    break;
                    
                case ARSessionState.Ready:
                    UpdateStatusText("Ready to start AR session");
                    break;
                    
                case ARSessionState.SessionInitializing:
                    UpdateStatusText("Initializing AR session...");
                    break;
                    
                case ARSessionState.SessionTracking:
                    if (!isInitialized)
                    {
                        isInitialized = true;
                        ShowInstructions(true);
                    }
                    UpdateStatusText("Session tracking active");
                    break;
            }
        }

        private void UpdateStatusText(ARSessionState state)
        {
            if (statusText != null)
            {
                statusText.text = $"AR State: {state}";
            }
        }

        private void UpdateStatusText(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
        }

        private void ShowInstructions(bool show)
        {
            if (instructionsPanel != null)
            {
                instructionsPanel.SetActive(show);
            }
        }

        public void ToggleDebugPanel()
        {
            if (debugPanel != null)
            {
                debugPanel.SetActive(!debugPanel.activeSelf);
            }
        }

        public void TogglePlaneVisualization()
        {
            if (planeManager != null)
            {
                planeManager.planePrefab.GetComponent<MeshRenderer>().enabled = 
                    !planeManager.planePrefab.GetComponent<MeshRenderer>().enabled;
            }
        }

        public void RestartSession()
        {
            if (arSession != null)
            {
                arSession.Reset();
                isInitialized = false;
                UpdateStatusText("Restarting AR session...");
            }
        }
    }
}