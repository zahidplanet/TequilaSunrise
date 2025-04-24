using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TequilaSunrise.AR;

namespace TequilaSunrise.UI
{
    /// <summary>
    /// Manages the AR user interface elements
    /// </summary>
    public class ARUIManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ARSessionManager arSessionManager;
        [SerializeField] private Canvas mainCanvas;
        
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private GameObject instructionsPanel;
        [SerializeField] private Button resetButton;
        [SerializeField] private Button togglePlaneDetectionButton;
        [SerializeField] private TextMeshProUGUI planeDetectionButtonText;
        
        [Header("UI Settings")]
        [SerializeField] private float instructionsDuration = 5f;
        [SerializeField] private string defaultInstructions = "Point your device at a flat surface";
        
        private void Awake()
        {
            ValidateReferences();
            SetupUI();
        }
        
        private void Start()
        {
            ShowInstructions(defaultInstructions, instructionsDuration);
        }
        
        private void ValidateReferences()
        {
            if (!arSessionManager)
            {
                arSessionManager = FindObjectOfType<ARSessionManager>();
                if (!arSessionManager)
                {
                    Debug.LogError("ARUIManager: ARSessionManager reference not set!");
                    enabled = false;
                    return;
                }
            }
            
            if (!mainCanvas)
            {
                mainCanvas = GetComponent<Canvas>();
                if (!mainCanvas)
                {
                    Debug.LogError("ARUIManager: Canvas reference not set!");
                    enabled = false;
                    return;
                }
            }
        }
        
        private void SetupUI()
        {
            if (resetButton)
            {
                resetButton.onClick.AddListener(OnResetButtonPressed);
            }
            
            if (togglePlaneDetectionButton)
            {
                togglePlaneDetectionButton.onClick.AddListener(OnTogglePlaneDetectionPressed);
            }
            
            if (instructionsPanel)
            {
                instructionsPanel.SetActive(false);
            }
            
            UpdateStatusText("Initializing AR...");
        }
        
        private void OnEnable()
        {
            if (arSessionManager)
            {
                arSessionManager.onSessionInitialized.AddListener(OnARSessionInitialized);
                arSessionManager.onSessionError.AddListener(OnARSessionError);
                arSessionManager.onPlaneDetected.AddListener(OnPlaneDetected);
            }
        }
        
        private void OnDisable()
        {
            if (arSessionManager)
            {
                arSessionManager.onSessionInitialized.RemoveListener(OnARSessionInitialized);
                arSessionManager.onSessionError.RemoveListener(OnARSessionError);
                arSessionManager.onPlaneDetected.RemoveListener(OnPlaneDetected);
            }
        }
        
        #region Event Handlers
        
        private void OnARSessionInitialized()
        {
            UpdateStatusText("AR Session Ready");
            ShowInstructions(defaultInstructions, instructionsDuration);
        }
        
        private void OnARSessionError()
        {
            UpdateStatusText("AR Session Error");
            ShowInstructions("There was an error with AR. Please restart the app.", 0f);
        }
        
        private void OnPlaneDetected(UnityEngine.XR.ARFoundation.ARPlane plane)
        {
            UpdateStatusText("Surface detected");
        }
        
        private void OnResetButtonPressed()
        {
            if (arSessionManager)
            {
                arSessionManager.ResetSession();
                UpdateStatusText("Resetting AR session...");
            }
        }
        
        private void OnTogglePlaneDetectionPressed()
        {
            if (arSessionManager)
            {
                bool newState = !togglePlaneDetectionButton.gameObject.activeSelf;
                arSessionManager.TogglePlaneDetection(newState);
                UpdatePlaneDetectionButton(newState);
            }
        }
        
        #endregion
        
        #region UI Updates
        
        public void UpdateStatusText(string message)
        {
            if (statusText)
            {
                statusText.text = message;
            }
        }
        
        public void ShowInstructions(string message, float duration = 0f)
        {
            if (instructionsPanel && instructionsPanel.GetComponentInChildren<TextMeshProUGUI>())
            {
                instructionsPanel.GetComponentInChildren<TextMeshProUGUI>().text = message;
                instructionsPanel.SetActive(true);
                
                if (duration > 0f)
                {
                    Invoke(nameof(HideInstructions), duration);
                }
            }
        }
        
        public void HideInstructions()
        {
            if (instructionsPanel)
            {
                instructionsPanel.SetActive(false);
            }
        }
        
        private void UpdatePlaneDetectionButton(bool enabled)
        {
            if (planeDetectionButtonText)
            {
                planeDetectionButtonText.text = enabled ? "Disable Plane Detection" : "Enable Plane Detection";
            }
        }
        
        #endregion
    }
}