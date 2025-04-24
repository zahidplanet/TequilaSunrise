using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR
public class AvatarSetup : EditorWindow
{
    private GameObject avatarModelPrefab;
    private float characterHeight = 1.8f;
    private float characterRadius = 0.3f;
    private float stepOffset = 0.3f;
    private float avatarScale = 1.0f;

    [MenuItem("Tools/Setup Avatar Prefab")]
    public static void ShowWindow()
    {
        GetWindow<AvatarSetup>("Avatar Prefab Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Avatar Prefab Setup", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        avatarModelPrefab = (GameObject)EditorGUILayout.ObjectField("Avatar Model Prefab:", avatarModelPrefab, typeof(GameObject), false);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Character Controller Settings:", EditorStyles.boldLabel);
        characterHeight = EditorGUILayout.FloatField("Height:", characterHeight);
        characterRadius = EditorGUILayout.FloatField("Radius:", characterRadius);
        stepOffset = EditorGUILayout.FloatField("Step Offset:", stepOffset);
        
        EditorGUILayout.Space();
        avatarScale = EditorGUILayout.FloatField("Avatar Scale:", avatarScale);
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("This will create an AvatarPrefab based on the selected model and settings.", MessageType.Info);
        
        if (GUILayout.Button("Create Avatar Prefab"))
        {
            if (avatarModelPrefab == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select an avatar model prefab first!", "OK");
                return;
            }
            
            CreateAvatarPrefab();
        }
    }

    private void CreateAvatarPrefab()
    {
        // Create Prefabs directory if it doesn't exist
        if (!Directory.Exists("Assets/Prefabs"))
        {
            Directory.CreateDirectory("Assets/Prefabs");
        }
        
        // Create AvatarPrefab GameObject
        GameObject avatarPrefab = new GameObject("AvatarPrefab");
        
        // Instantiate the model as a child
        GameObject modelInstance = Instantiate(avatarModelPrefab, avatarPrefab.transform);
        modelInstance.name = avatarModelPrefab.name;
        
        // Set the scale of the model
        modelInstance.transform.localScale = Vector3.one * avatarScale;
        
        // Add Character Controller
        CharacterController characterController = avatarPrefab.AddComponent<CharacterController>();
        characterController.height = characterHeight;
        characterController.radius = characterRadius;
        characterController.stepOffset = stepOffset;
        
        // Add Capsule Collider for physics interactions
        CapsuleCollider capsuleCollider = avatarPrefab.AddComponent<CapsuleCollider>();
        capsuleCollider.height = characterHeight;
        capsuleCollider.radius = characterRadius;
        capsuleCollider.center = new Vector3(0, characterHeight / 2, 0);
        
        // Add Animator component
        Animator animator = avatarPrefab.AddComponent<Animator>();
        // Note: Animation controller should be assigned manually or automatically found
        
        // Add AvatarController script
        avatarPrefab.AddComponent<AvatarController>();
        
        // Save as prefab
        string prefabPath = "Assets/Prefabs/AvatarPrefab.prefab";
        GameObject prefabAsset = PrefabUtility.SaveAsPrefabAsset(avatarPrefab, prefabPath);
        
        // Destroy the scene instance
        DestroyImmediate(avatarPrefab);
        
        EditorUtility.DisplayDialog("Success", "Avatar prefab created at " + prefabPath, "OK");
        
        // Select the created prefab in the Project window
        Selection.activeObject = prefabAsset;
    }
}
#endif 