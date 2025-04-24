using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.ARFoundation;

namespace TequilaSunrise.AR
{
    public class ARTestManager : MonoBehaviour
    {
        [Header("AR Components")]
        [SerializeField] private ARSessionManager sessionManager;
        [SerializeField] private ARCharacterPlacer characterPlacer;
        [SerializeField] private ARPlaneManager planeManager;
        
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private Button resetButton;
        [SerializeField] private GameObject instructionsPanel;
        [SerializeField] private GameObject debugPanel;
        
        [Header("Debug Info")]
        [SerializeField] private TextMeshProUGUI fpsText;
        [SerializeField] private TextMeshProUGUI deviceInfoText;
        [SerializeField] private bool showDebugInfo = true;

        private float fpsUpdateInterval = 0.5f;
        private float fpsTimer;
        private int frameCount;

        private void Start()
        {
            if (resetButton)
            {
                resetButton.onClick.AddListener(ResetScene);
            }

            if (deviceInfoText)
            {
                UpdateDeviceInfo();
            }

            if (debugPanel)
            {
                debugPanel.SetActive(showDebugInfo);
            }

            if (instructionsPanel)
            {
                ShowInstructions(true);
                Invoke(nameof(HideInstructions), 5f);
            }
        }

        private void Update()
        {
            UpdateStatus();
            if (showDebugInfo)
            {
                UpdateFPS();
            }
        }

        private void UpdateStatus()
        {
            if (!statusText) return;

            string status = "Looking for surface...";
            
            if (planeManager && planeManager.trackables.count > 0)
            {
                status = characterPlacer && characterPlacer.GetSpawnedCharacter() 
                    ? "Character placed - Touch to move" 
                    : "Surface found - Touch to place character";
            }

            statusText.text = status;
        }

        private void UpdateFPS()
        {
            if (!fpsText) return;

            frameCount++;
            fpsTimer += Time.deltaTime;

            if (fpsTimer >= fpsUpdateInterval)
            {
                float fps = frameCount / fpsTimer;
                fpsText.text = $"FPS: {fps:F1}";
                fpsTimer = 0;
                frameCount = 0;
            }
        }

        private void UpdateDeviceInfo()
        {
            if (!deviceInfoText) return;

            string info = $"Device: {SystemInfo.deviceModel}\n" +
                         $"OS: {SystemInfo.operatingSystem}\n" +
                         $"Memory: {SystemInfo.systemMemorySize}MB\n" +
                         $"Graphics: {SystemInfo.graphicsDeviceName}";
            
            deviceInfoText.text = info;
        }

        private void ResetScene()
        {
            if (characterPlacer)
            {
                characterPlacer.ResetCharacter();
            }

            if (planeManager)
            {
                foreach (var plane in planeManager.trackables)
                {
                    plane.gameObject.SetActive(true);
                }
            }

            if (sessionManager)
            {
                sessionManager.RestartSession();
            }

            ShowInstructions(true);
        }

        private void ShowInstructions(bool show)
        {
            if (instructionsPanel)
            {
                instructionsPanel.SetActive(show);
            }
        }

        private void HideInstructions()
        {
            ShowInstructions(false);
        }

        private void OnEnable()
        {
            if (planeManager)
            {
                planeManager.planesChanged += OnPlanesChanged;
            }
        }

        private void OnDisable()
        {
            if (planeManager)
            {
                planeManager.planesChanged -= OnPlanesChanged;
            }
        }

        private void OnPlanesChanged(ARPlanesChangedEventArgs args)
        {
            if (characterPlacer && characterPlacer.GetSpawnedCharacter())
            {
                foreach (var plane in args.added)
                {
                    plane.gameObject.SetActive(false);
                }
            }
        }
    }
}