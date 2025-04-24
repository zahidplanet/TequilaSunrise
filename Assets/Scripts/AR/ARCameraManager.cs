using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections;

namespace TequilaSunrise.AR
{
    [RequireComponent(typeof(ARCameraManager))]
    public class ARCameraManager : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private bool enableAutoFocus = true;
        [SerializeField] private Light directionalLight;
        [SerializeField] private float lightIntensityMultiplier = 1.0f;
        
        [Header("Performance")]
        [SerializeField] private bool enableHDR = false;
        [SerializeField] private int targetFrameRate = 60;
        
        private UnityEngine.XR.ARFoundation.ARCameraManager arCameraManager;
        private ARCameraBackground arCameraBackground;
        private Camera arCamera;
        private float originalLightIntensity;
        
        private void Awake()
        {
            SetupComponents();
            ConfigurePerformance();
        }
        
        private void OnEnable()
        {
            if (arCameraManager != null)
            {
                arCameraManager.frameReceived += OnFrameReceived;
            }
            
            StartCoroutine(WaitForCameraReady());
        }
        
        private void OnDisable()
        {
            if (arCameraManager != null)
            {
                arCameraManager.frameReceived -= OnFrameReceived;
            }
        }
        
        private void SetupComponents()
        {
            arCameraManager = GetComponent<UnityEngine.XR.ARFoundation.ARCameraManager>();
            arCameraBackground = GetComponent<ARCameraBackground>();
            arCamera = GetComponent<Camera>();
            
            if (directionalLight != null)
            {
                originalLightIntensity = directionalLight.intensity;
            }
            
            ValidateSetup();
        }
        
        private void ValidateSetup()
        {
            if (arCameraManager == null)
            {
                Debug.LogError("ARCameraManager component not found!");
                enabled = false;
                return;
            }
            
            if (arCamera == null)
            {
                Debug.LogError("Camera component not found!");
                enabled = false;
                return;
            }
        }
        
        private void ConfigurePerformance()
        {
            Application.targetFrameRate = targetFrameRate;
            
            if (arCamera != null)
            {
                arCamera.allowHDR = enableHDR;
            }
        }
        
        private void OnFrameReceived(ARCameraFrameEventArgs args)
        {
            if (args.lightEstimation.averageBrightness.HasValue)
            {
                UpdateLighting(args.lightEstimation);
            }
            
            if (enableAutoFocus && args.camera.focusMode != CameraFocusMode.Auto)
            {
                StartCoroutine(EnableAutoFocus());
            }
        }
        
        private void UpdateLighting(XRCameraFrame frame)
        {
            if (directionalLight == null) return;
            
            if (frame.averageColorTemperature.HasValue)
            {
                directionalLight.colorTemperature = frame.averageColorTemperature.Value;
            }
            
            if (frame.averageBrightness.HasValue)
            {
                directionalLight.intensity = originalLightIntensity * 
                    frame.averageBrightness.Value * lightIntensityMultiplier;
            }
        }
        
        private IEnumerator EnableAutoFocus()
        {
            yield return new WaitForSeconds(0.5f);
            
            var config = arCameraManager.currentConfiguration;
            if (config.HasValue)
            {
                config.Value.focusMode = CameraFocusMode.Auto;
                yield return arCameraManager.TrySetConfiguration(config.Value);
            }
        }
        
        private IEnumerator WaitForCameraReady()
        {
            while (arCameraManager.permissionGranted == false)
            {
                yield return null;
            }
            
            ConfigureCamera();
        }
        
        private void ConfigureCamera()
        {
            if (arCameraBackground != null)
            {
                arCameraBackground.useCustomMaterial = false;
            }
            
            if (arCamera != null)
            {
                arCamera.clearFlags = CameraClearFlags.SolidColor;
                arCamera.backgroundColor = Color.black;
            }
        }
        
        public Camera GetARCamera()
        {
            return arCamera;
        }
        
        public bool IsCameraReady()
        {
            return arCameraManager != null && 
                   arCameraManager.permissionGranted && 
                   arCameraManager.subsystem?.running == true;
        }
        
        public void SetLightEstimationMultiplier(float multiplier)
        {
            lightIntensityMultiplier = multiplier;
        }
        
        public void ToggleHDR(bool enable)
        {
            enableHDR = enable;
            if (arCamera != null)
            {
                arCamera.allowHDR = enable;
            }
        }
    }
}