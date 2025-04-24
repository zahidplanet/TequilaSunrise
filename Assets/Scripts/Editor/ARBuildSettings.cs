using UnityEngine;
using UnityEditor;
using UnityEditor.XR.Management;
using UnityEditor.XR.Management.Metadata;

namespace TequilaSunrise.Editor
{
    public class ARBuildSettings : EditorWindow
    {
        [MenuItem("TequilaSunrise/Build/Configure AR Settings")]
        public static void ShowWindow()
        {
            GetWindow<ARBuildSettings>("AR Build Settings");
        }

        private void OnGUI()
        {
            GUILayout.Label("AR Build Configuration", EditorStyles.boldLabel);
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Setup AR Foundation"))
            {
                SetupARFoundation();
            }
            
            if (GUILayout.Button("Configure iOS Settings"))
            {
                ConfigureIOSSettings();
            }
            
            if (GUILayout.Button("Configure Android Settings"))
            {
                ConfigureAndroidSettings();
            }
            
            if (GUILayout.Button("Add Test Scene to Build"))
            {
                AddTestSceneToBuild();
            }
        }
        
        private void SetupARFoundation()
        {
            // Enable XR Plugin Management
            XRGeneralSettingsPerBuildTarget buildTargetSettings = null;
            if (EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey, out buildTargetSettings))
            {
                var settings = buildTargetSettings.SettingsForBuildTarget(BuildTargetGroup.iOS);
                if (settings == null)
                {
                    settings = ScriptableObject.CreateInstance<XRManagerSettings>();
                    buildTargetSettings.SetSettingsForBuildTarget(BuildTargetGroup.iOS, settings);
                }
                
                // Configure AR settings here
                EditorUtility.SetDirty(buildTargetSettings);
                AssetDatabase.SaveAssets();
            }
            
            Debug.Log("AR Foundation setup completed");
        }
        
        private void ConfigureIOSSettings()
        {
            // Configure iOS Player Settings
            PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneOnly;
            PlayerSettings.iOS.requiresFullScreen = true;
            PlayerSettings.iOS.allowedAutorotateToPortrait = true;
            PlayerSettings.iOS.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.iOS.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.iOS.allowedAutorotateToLandscapeRight = false;
            
            // Set minimum iOS version for AR
            PlayerSettings.iOS.targetOSVersionString = "13.0";
            
            // Camera Usage Description
            PlayerSettings.iOS.cameraUsageDescription = "Camera access is required for AR features";
            
            Debug.Log("iOS settings configured");
        }
        
        private void ConfigureAndroidSettings()
        {
            // Configure Android Player Settings
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            
            PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.Auto;
            PlayerSettings.Android.forceSDCardPermission = true;
            
            // Configure orientation settings
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.Android.useAPKExpansionFiles = false;
            
            // AR Core required
            PlayerSettings.Android.requiresARCoreSupport = true;
            
            Debug.Log("Android settings configured");
        }
        
        private void AddTestSceneToBuild()
        {
            string testScenePath = "Assets/Scenes/ARTest.unity";
            if (System.IO.File.Exists(testScenePath))
            {
                EditorBuildSettings.scenes = new EditorBuildSettingsScene[]
                {
                    new EditorBuildSettingsScene(testScenePath, true)
                };
                Debug.Log("Test scene added to build settings");
            }
            else
            {
                Debug.LogError("Test scene not found at: " + testScenePath);
            }
        }
    }
}