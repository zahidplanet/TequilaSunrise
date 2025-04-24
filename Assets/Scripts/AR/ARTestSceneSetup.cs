using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

namespace TequilaSunrise.AR
{
    public class ARTestSceneSetup : MonoBehaviour
    {
        [Header("AR Components")]
        public ARSession arSession;
        public ARSessionOrigin sessionOrigin;
        public ARPlaneManager planeManager;
        public ARRaycastManager raycastManager;

        [Header("UI Elements")]
        public TextMeshProUGUI statusText;
        public GameObject instructionsPanel;

        private void Start()
        {
            // Ensure all required components are present
            ValidateSetup();
            
            // Subscribe to AR session state changes
            ARSession.stateChanged += OnARSessionStateChanged;
            
            // Initialize UI
            UpdateStatusText("Initializing AR...");
            ShowInstructions(true);
        }

        private void OnDestroy()
        {
            ARSession.stateChanged -= OnARSessionStateChanged;
        }

        private void ValidateSetup()
        {
            if (arSession == null)
            {
                Debug.LogError("AR Session not assigned!");
                return;
            }

            if (sessionOrigin == null)
            {
                Debug.LogError("AR Session Origin not assigned!");
                return;
            }

            if (planeManager == null)
            {
                Debug.LogError("AR Plane Manager not assigned!");
                return;
            }

            Debug.Log("AR components validated successfully");
        }

        private void OnARSessionStateChanged(ARSessionStateChangedEventArgs args)
        {
            string status = "AR Status: ";
            
            switch (args.state)
            {
                case ARSessionState.None:
                    status += "Initializing";
                    break;
                case ARSessionState.Checking:
                    status += "Checking device compatibility";
                    break;
                case ARSessionState.Installing:
                    status += "Installing AR components";
                    break;
                case ARSessionState.Ready:
                    status += "Ready";
                    ShowInstructions(true);
                    break;
                case ARSessionState.SessionInitializing:
                    status += "Starting AR session";
                    break;
                case ARSessionState.SessionTracking:
                    status += "Tracking";
                    ShowInstructions(false);
                    break;
                case ARSessionState.Unsupported:
                    status += "Device not supported";
                    break;
            }

            UpdateStatusText(status);
        }

        private void UpdateStatusText(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
                Debug.Log(message);
            }
        }

        private void ShowInstructions(bool show)
        {
            if (instructionsPanel != null)
            {
                instructionsPanel.SetActive(show);
            }
        }

        public void RestartSession()
        {
            if (arSession != null)
            {
                arSession.Reset();
                UpdateStatusText("AR Session restarted");
            }
        }
    }
}