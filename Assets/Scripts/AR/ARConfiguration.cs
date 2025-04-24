using UnityEngine;
using UnityEngine.XR.ARSubsystems;

namespace TequilaSunrise.AR
{
    [CreateAssetMenu(fileName = "ARConfiguration", menuName = "TequilaSunrise/AR/Configuration")]
    public class ARConfiguration : ScriptableObject
    {
        [Header("Session Settings")]
        public bool enableAutoFocus = true;
        public bool matchFrameRate = true;
        public bool enableLightEstimation = true;
        
        [Header("Plane Detection")]
        public bool enablePlaneDetection = true;
        public PlaneDetectionMode planeDetectionMode = PlaneDetectionMode.Horizontal;
        public float minPlaneSize = 0.2f;
        
        [Header("Camera")]
        public bool enableHDR = false;
        public int targetFrameRate = 60;
        public float lightEstimationMultiplier = 1.0f;
        
        [Header("Performance")]
        public bool enableOcclusion = true;
        public bool enablePointCloud = false;
        public int maxPointCloudPoints = 1000;
        
        [Header("Debug")]
        public bool showDebugLogs = false;
        public bool visualizeTracking = false;
        
        private static ARConfiguration instance;
        public static ARConfiguration Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<ARConfiguration>("ARConfiguration");
                    if (instance == null)
                    {
                        instance = CreateInstance<ARConfiguration>();
                        #if UNITY_EDITOR
                        UnityEditor.AssetDatabase.CreateAsset(instance, "Assets/Resources/ARConfiguration.asset");
                        UnityEditor.AssetDatabase.SaveAssets();
                        #endif
                    }
                }
                return instance;
            }
        }
        
        public void ApplyConfiguration(ARSessionManager sessionManager)
        {
            if (sessionManager == null) return;
            
            // Apply session settings
            sessionManager.TogglePlaneDetection(enablePlaneDetection);
            
            // Apply camera settings if available
            var cameraManager = sessionManager.GetComponentInChildren<ARCameraManager>();
            if (cameraManager != null)
            {
                cameraManager.ToggleHDR(enableHDR);
                cameraManager.SetLightEstimationMultiplier(lightEstimationMultiplier);
            }
            
            if (showDebugLogs)
            {
                Debug.Log("AR Configuration applied successfully");
            }
        }
        
        public void SaveConfiguration()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
            #endif
        }
    }
}