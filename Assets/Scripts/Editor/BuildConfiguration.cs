using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;
using System;

namespace TequilaSunrise.Editor
{
    public class BuildConfiguration : EditorWindow
    {
        private const string VERSION = "0.1.0";
        private const string BUNDLE_ID = "com.tequilasunrise.ar";
        private const string BUILD_PATH = "Builds/iOS/TequilaSunrise";

        [MenuItem("TequilaSunrise/Build/Configure iOS")]
        public static void ConfigureIOSBuild()
        {
            // Set build target to iOS
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
            }

            // Configure player settings
            PlayerSettings.companyName = "TequilaSunrise";
            PlayerSettings.productName = "TequilaSunrise";
            PlayerSettings.bundleVersion = VERSION;
            PlayerSettings.iOS.applicationIdentifier = BUNDLE_ID;
            PlayerSettings.iOS.buildNumber = "1";
            PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneOnly;
            PlayerSettings.iOS.targetOSVersionString = "14.0";
            
            // AR Configuration
            PlayerSettings.iOS.requiresARKit = true;
            PlayerSettings.iOS.cameraUsageDescription = "AR features require camera access";
            PlayerSettings.iOS.locationUsageDescription = "AR features require location access";

            // Graphics and Performance
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, 2); // ARM64
            PlayerSettings.iOS.targetGraphics = iOSTargetGraphics.Metal;
            
            // Orientation settings
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.allowedAutorotateToPortrait = true;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.allowedAutorotateToLandscapeLeft = false;
            PlayerSettings.allowedAutorotateToLandscapeRight = false;

            // Quality settings
            QualitySettings.SetQualityLevel(0, true);
            QualitySettings.vSyncCount = 0;
            QualitySettings.antiAliasing = 0;

            Debug.Log("iOS build settings configured successfully");
        }

        [MenuItem("TequilaSunrise/Build/Build iOS")]
        public static void BuildIOS()
        {
            ConfigureIOSBuild();

            // Get all scenes from build settings
            var scenes = new List<string>();
            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled)
                    scenes.Add(scene.path);
            }

            if (scenes.Count == 0)
            {
                Debug.LogError("No scenes in build settings!");
                return;
            }

            try
            {
                // Configure build options
                var buildPlayerOptions = new BuildPlayerOptions
                {
                    scenes = scenes.ToArray(),
                    locationPathName = BUILD_PATH,
                    target = BuildTarget.iOS,
                    options = BuildOptions.Development
                };

                // Build the project
                BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
                BuildSummary summary = report.summary;

                if (summary.result == BuildResult.Succeeded)
                {
                    Debug.Log($"Build succeeded: {summary.totalSize} bytes");
                    EditorUtility.RevealInFinder(BUILD_PATH);
                }
                else if (summary.result == BuildResult.Failed)
                {
                    Debug.LogError($"Build failed: {summary.totalErrors} errors");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Build error: {e.Message}");
            }
        }

        [MenuItem("TequilaSunrise/Build/Show Build Settings")]
        public static void ShowBuildSettings()
        {
            EditorWindow.GetWindow<BuildConfiguration>("Build Settings");
        }

        private void OnGUI()
        {
            GUILayout.Label("TequilaSunrise Build Configuration", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Configure iOS Settings"))
            {
                ConfigureIOSBuild();
            }

            if (GUILayout.Button("Build iOS"))
            {
                BuildIOS();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Current Settings:");
            EditorGUILayout.LabelField("Version", PlayerSettings.bundleVersion);
            EditorGUILayout.LabelField("Bundle ID", PlayerSettings.iOS.applicationIdentifier);
            EditorGUILayout.LabelField("Target iOS Version", PlayerSettings.iOS.targetOSVersionString);
        }
    }
}