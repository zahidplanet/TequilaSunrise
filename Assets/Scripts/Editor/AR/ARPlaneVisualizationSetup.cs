using UnityEngine;
using UnityEditor;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace TequilaSunrise.AR.Editor
{
    public class ARPlaneVisualizationSetup : EditorWindow
    {
        private Material planeMaterial;
        private float planeScale = 1f;
        private Color planeColor = new Color(1f, 1f, 1f, 0.5f);

        [MenuItem("TequilaSunrise/AR/Create Plane Visualization")]
        public static void ShowWindow()
        {
            GetWindow<ARPlaneVisualizationSetup>("AR Plane Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("AR Plane Visualization Setup", EditorStyles.boldLabel);

            planeMaterial = (Material)EditorGUILayout.ObjectField("Plane Material", planeMaterial, typeof(Material), false);
            planeScale = EditorGUILayout.FloatField("Plane Scale", planeScale);
            planeColor = EditorGUILayout.ColorField("Plane Color", planeColor);

            if (GUILayout.Button("Create Plane Prefab"))
            {
                CreatePlanePrefab();
            }
        }

        private void CreatePlanePrefab()
        {
            // Create the root GameObject with required components
            GameObject planeObject = new GameObject("AR Plane Visualization");
            ARPlane arPlane = planeObject.AddComponent<ARPlane>();
            MeshFilter meshFilter = planeObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = planeObject.AddComponent<MeshRenderer>();
            ARPlaneMeshVisualizer meshVisualizer = planeObject.AddComponent<ARPlaneMeshVisualizer>();

            // Set up the mesh renderer
            if (planeMaterial != null)
            {
                meshRenderer.material = planeMaterial;
                meshRenderer.material.color = planeColor;
            }

            // Set up transform
            planeObject.transform.localScale = Vector3.one * planeScale;

            // Add our custom plane controller
            ARPlaneController planeController = planeObject.AddComponent<ARPlaneController>();

            // Create the prefab
            string prefabPath = "Assets/Prefabs/AR/ARPlaneVisualization.prefab";
            
            // Ensure the directory exists
            string directory = System.IO.Path.GetDirectoryName(prefabPath);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            // Create the prefab
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(planeObject, prefabPath);
            if (prefab != null)
            {
                EditorUtility.DisplayDialog("Success", "AR Plane Visualization prefab created successfully!", "OK");
                Selection.activeObject = prefab;
            }

            // Clean up the scene object
            DestroyImmediate(planeObject);
        }
    }
}