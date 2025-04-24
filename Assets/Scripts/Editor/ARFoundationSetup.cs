using UnityEditor;
using UnityEngine;
using UnityEditor.XR.Management;
using UnityEngine.XR.Management;
using UnityEngine.XR.ARFoundation;

namespace TequilaSunrise.Editor
{
    public class ARFoundationSetup : EditorWindow
    {
        [MenuItem("TequilaSunrise/Setup/Configure AR Foundation")]
        public static void ShowWindow()
        {
            GetWindow<ARFoundationSetup>("AR Setup");
        }

        void OnGUI()
        {
            GUILayout.Label("AR Foundation Setup", EditorStyles.boldLabel);

            if (GUILayout.Button("1. Install Required Packages"))
            {
                InstallRequiredPackages();
            }

            if (GUILayout.Button("2. Configure XR Plugin Management"))
            {
                ConfigureXRPluginManagement();
            }

            if (GUILayout.Button("3. Setup AR Camera"))
            {
                SetupARCamera();
            }

            if (GUILayout.Button("4. Create Basic AR Scene"))
            {
                CreateBasicARScene();
            }

            if (GUILayout.Button("5. Configure Build Settings"))
            {
                ConfigureBuildSettings();
            }
        }

        private void InstallRequiredPackages()
        {
            // Note: Package installation should be done through Package Manager UI
            // This is just a reminder of required packages
            EditorUtility.DisplayDialog("Required Packages",
                "Please install the following packages through Package Manager:\n\n" +
                "- AR Foundation (com.unity.xr.arfoundation)\n" +
                "- AR Subsystems (com.unity.xr.arsubsystems)\n" +
                "- Apple ARKit XR Plugin (com.unity.xr.arkit)\n" +
                "- Google ARCore XR Plugin (com.unity.xr.arcore)\n" +
                "- Universal RP (com.unity.render-pipelines.universal)",
                "OK");
        }

        private void ConfigureXRPluginManagement()
        {
            var generalSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.iOS);
            if (generalSettings == null)
            {
                var settings = ScriptableObject.CreateInstance<XRGeneralSettings>();
                var managerSettings = ScriptableObject.CreateInstance<XRManagerSettings>();
                settings.Manager = managerSettings;
                
                // Create settings asset
                if (!AssetDatabase.IsValidFolder("Assets/XR"))
                    AssetDatabase.CreateFolder("Assets", "XR");
                
                AssetDatabase.CreateAsset(settings, "Assets/XR/XRGeneralSettings.asset");
                AssetDatabase.CreateAsset(managerSettings, "Assets/XR/XRManagerSettings.asset");
                AssetDatabase.SaveAssets();
            }

            EditorUtility.DisplayDialog("XR Plugin Management",
                "XR Plugin Management has been configured.\n\n" +
                "Please enable ARKit in iOS and ARCore in Android through:\n" +
                "Project Settings > XR Plug-in Management",
                "OK");
        }

        private void SetupARCamera()
        {
            // Create AR Camera setup
            var arSessionGO = new GameObject("AR Session");
            arSessionGO.AddComponent<ARSession>();

            var arSessionOriginGO = new GameObject("AR Session Origin");
            var arSessionOrigin = arSessionOriginGO.AddComponent<ARSessionOrigin>();
            
            var cameraGO = new GameObject("AR Camera");
            var camera = cameraGO.AddComponent<Camera>();
            var arCamera = cameraGO.AddComponent<ARCameraManager>();
            var arCameraBackground = cameraGO.AddComponent<ARCameraBackground>();
            
            cameraGO.transform.SetParent(arSessionOriginGO.transform);
            arSessionOrigin.camera = camera;

            // Basic camera setup
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.clear;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 20f;

            Debug.Log("AR Camera setup completed");
        }

        private void CreateBasicARScene()
        {
            // Create a new scene with AR setup
            var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(UnityEditor.SceneManagement.NewSceneSetup.EmptyScene);
            
            SetupARCamera();

            // Add essential AR components
            var sessionOrigin = GameObject.Find("AR Session Origin");
            if (sessionOrigin != null)
            {
                sessionOrigin.AddComponent<ARPlaneManager>();
                sessionOrigin.AddComponent<ARRaycastManager>();
            }

            // Save the scene
            string scenePath = "Assets/Scenes/ARScene.unity";
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log($"Basic AR scene created and saved at: {scenePath}");
        }

        private void ConfigureBuildSettings()
        {
            // Configure player settings for AR
            PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, 2); // ARM64
            PlayerSettings.SetArchitecture(BuildTargetGroup.Android, 2); // ARM64

            // iOS specific settings
            PlayerSettings.iOS.requiresARKit = true;
            PlayerSettings.iOS.targetOSVersionString = "14.0";

            // Android specific settings
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel33;

            Debug.Log("Build settings configured for AR");
        }
    }
}