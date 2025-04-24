using UnityEditor;
using UnityEngine;

namespace TequilaSunrise.Editor
{
    public static class BuildCLI
    {
        public static void BuildAll()
        {
            Debug.Log("Starting build process...");
            
            string[] scenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .ToArray();
                
            if (scenes.Length == 0)
            {
                Debug.LogError("No scenes in build settings!");
                EditorApplication.Exit(1);
                return;
            }
            
            // Set up Android build settings
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel33;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            
            // Build Android
            BuildPlayerOptions androidOptions = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = "Build/Android/TequilaSunrise.apk",
                target = BuildTarget.Android,
                options = BuildOptions.None
            };
            
            BuildReport report = BuildPipeline.BuildPlayer(androidOptions);
            BuildSummary summary = report.summary;
            
            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"Build succeeded: {summary.totalSize / 1024 / 1024} MB");
                EditorApplication.Exit(0);
            }
            else
            {
                Debug.LogError($"Build failed with {summary.totalErrors} errors");
                EditorApplication.Exit(1);
            }
        }
    }
}