using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR
public class MotorcycleSetup : EditorWindow
{
    private GameObject motorcycleModelPrefab;
    private float motorcycleMass = 250f;
    private float wheelRadius = 0.3f;
    private float suspensionDistance = 0.3f;
    private float motorcycleScale = 1.0f;

    [MenuItem("Tools/Setup Motorcycle Prefab")]
    public static void ShowWindow()
    {
        GetWindow<MotorcycleSetup>("Motorcycle Prefab Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Motorcycle Prefab Setup", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        motorcycleModelPrefab = (GameObject)EditorGUILayout.ObjectField("Motorcycle Model Prefab:", motorcycleModelPrefab, typeof(GameObject), false);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Physics Settings:", EditorStyles.boldLabel);
        motorcycleMass = EditorGUILayout.FloatField("Mass:", motorcycleMass);
        wheelRadius = EditorGUILayout.FloatField("Wheel Radius:", wheelRadius);
        suspensionDistance = EditorGUILayout.FloatField("Suspension Distance:", suspensionDistance);
        
        EditorGUILayout.Space();
        motorcycleScale = EditorGUILayout.FloatField("Motorcycle Scale:", motorcycleScale);
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("This will create a MotorcyclePrefab based on the selected model and settings.", MessageType.Info);
        
        if (GUILayout.Button("Create Motorcycle Prefab"))
        {
            if (motorcycleModelPrefab == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select a motorcycle model prefab first!", "OK");
                return;
            }
            
            CreateMotorcyclePrefab();
        }
    }

    private void CreateMotorcyclePrefab()
    {
        // Create Prefabs directory if it doesn't exist
        if (!Directory.Exists("Assets/Prefabs"))
        {
            Directory.CreateDirectory("Assets/Prefabs");
        }
        
        // Create MotorcyclePrefab GameObject
        GameObject motorcyclePrefab = new GameObject("MotorcyclePrefab");
        
        // Instantiate the model as a child
        GameObject modelInstance = Instantiate(motorcycleModelPrefab, motorcyclePrefab.transform);
        modelInstance.name = motorcycleModelPrefab.name;
        
        // Set the scale of the model
        modelInstance.transform.localScale = Vector3.one * motorcycleScale;
        
        // Add Rigidbody
        Rigidbody rb = motorcyclePrefab.AddComponent<Rigidbody>();
        rb.mass = motorcycleMass;
        
        // Add a BoxCollider to the motorcycle body
        BoxCollider bodyCollider = motorcyclePrefab.AddComponent<BoxCollider>();
        bodyCollider.center = new Vector3(0, 0.5f, 0);
        bodyCollider.size = new Vector3(0.5f, 0.8f, 2.0f);
        
        // Create wheel GameObjects
        GameObject frontWheel = new GameObject("FrontWheel");
        frontWheel.transform.SetParent(motorcyclePrefab.transform);
        frontWheel.transform.localPosition = new Vector3(0, 0.25f, 0.7f);
        
        GameObject rearWheel = new GameObject("RearWheel");
        rearWheel.transform.SetParent(motorcyclePrefab.transform);
        rearWheel.transform.localPosition = new Vector3(0, 0.25f, -0.7f);
        
        // Create center of mass GameObject
        GameObject centerOfMass = new GameObject("CenterOfMass");
        centerOfMass.transform.SetParent(motorcyclePrefab.transform);
        centerOfMass.transform.localPosition = new Vector3(0, 0.3f, 0);
        
        // Create mount position GameObject
        GameObject mountPosition = new GameObject("MountPosition");
        mountPosition.transform.SetParent(motorcyclePrefab.transform);
        mountPosition.transform.localPosition = new Vector3(0, 0.6f, 0);
        
        // Create handlebars GameObject
        GameObject handlebars = new GameObject("Handlebars");
        handlebars.transform.SetParent(motorcyclePrefab.transform);
        handlebars.transform.localPosition = new Vector3(0, 0.7f, 0.6f);
        
        // Add WheelColliders to wheels
        WheelCollider frontWheelCollider = frontWheel.AddComponent<WheelCollider>();
        frontWheelCollider.radius = wheelRadius;
        frontWheelCollider.suspensionDistance = suspensionDistance;
        WheelCollider.ConfigureVehicleSubsteps(5, 12, 15);
        
        WheelCollider rearWheelCollider = rearWheel.AddComponent<WheelCollider>();
        rearWheelCollider.radius = wheelRadius;
        rearWheelCollider.suspensionDistance = suspensionDistance;
        
        // Add MotorcycleController script
        MotorcycleController controller = motorcyclePrefab.AddComponent<MotorcycleController>();
        
        // Save as prefab
        string prefabPath = "Assets/Prefabs/MotorcyclePrefab.prefab";
        GameObject prefabAsset = PrefabUtility.SaveAsPrefabAsset(motorcyclePrefab, prefabPath);
        
        // Destroy the scene instance
        DestroyImmediate(motorcyclePrefab);
        
        EditorUtility.DisplayDialog("Success", "Motorcycle prefab created at " + prefabPath, "OK");
        
        // Select the created prefab in the Project window
        Selection.activeObject = prefabAsset;
    }
}
#endif 