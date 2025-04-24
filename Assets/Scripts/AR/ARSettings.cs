using UnityEngine;

namespace TequilaSunrise.AR
{
    [CreateAssetMenu(fileName = "ARSettings", menuName = "TequilaSunrise/AR/Settings")]
    public class ARSettings : ScriptableObject
    {
        [Header("Plane Detection")]
        public bool enablePlaneDetection = true;
        public bool enableVerticalPlanes = true;
        public bool enableHorizontalPlanes = true;
        
        [Header("Point Cloud")]
        public bool enablePointCloud = false;
        public Color pointCloudColor = Color.white;
        
        [Header("AR Session")]
        public bool autoFocus = true;
        public bool enableLightEstimation = true;
        public bool enableEnvironmentProbes = false;
        
        [Header("Performance")]
        public bool enableCpuImages = false;
        public bool enableOcclusion = false;
        
        [Header("Debug")]
        public bool showLogs = true;
        public bool showPlanePolygons = true;
        
        private static ARSettings instance;
        public static ARSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<ARSettings>("ARSettings");
                    if (instance == null)
                    {
                        Debug.LogWarning("ARSettings not found in Resources folder. Using default settings.");
                        instance = CreateInstance<ARSettings>();
                    }
                }
                return instance;
            }
        }
    }
}