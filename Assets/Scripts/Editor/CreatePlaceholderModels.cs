using UnityEngine;
using UnityEditor;
using System.IO;

public class CreatePlaceholderModels : EditorWindow
{
    private enum ModelType
    {
        Avatar,
        Motorcycle
    }

    private ModelType selectedModelType = ModelType.Avatar;
    private Color avatarColor = new Color(0.2f, 0.7f, 1.0f, 1.0f); // Light blue
    private Color motorcycleColor = new Color(0.8f, 0.2f, 0.2f, 1.0f); // Red

    [MenuItem("Tools/Create Placeholder Models")]
    public static void ShowWindow()
    {
        GetWindow<CreatePlaceholderModels>("Create Placeholder Models");
    }

    private void OnGUI()
    {
        GUILayout.Label("Create Placeholder Models for Development", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        selectedModelType = (ModelType)EditorGUILayout.EnumPopup("Model Type:", selectedModelType);
        
        if (selectedModelType == ModelType.Avatar)
        {
            avatarColor = EditorGUILayout.ColorField("Avatar Color:", avatarColor);
        }
        else
        {
            motorcycleColor = EditorGUILayout.ColorField("Motorcycle Color:", motorcycleColor);
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("This will create a simple placeholder model in the Models folder for development purposes.", MessageType.Info);
        
        if (GUILayout.Button("Create Placeholder Model"))
        {
            if (selectedModelType == ModelType.Avatar)
            {
                CreateAvatarPlaceholder();
            }
            else
            {
                CreateMotorcyclePlaceholder();
            }
        }
    }

    private void CreateAvatarPlaceholder()
    {
        // Create directory if it doesn't exist
        Directory.CreateDirectory("Assets/Models");
        
        // Create a simple GameObject with primitive shapes
        GameObject avatar = new GameObject("TS_PixelAvatarMain");
        
        // Body (capsule)
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.name = "Body";
        body.transform.SetParent(avatar.transform);
        body.transform.localPosition = new Vector3(0, 0.5f, 0);
        body.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        
        // Head (sphere)
        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        head.name = "Head";
        head.transform.SetParent(avatar.transform);
        head.transform.localPosition = new Vector3(0, 1.2f, 0);
        head.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        
        // Arms (cubes)
        GameObject leftArm = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftArm.name = "LeftArm";
        leftArm.transform.SetParent(avatar.transform);
        leftArm.transform.localPosition = new Vector3(-0.4f, 0.5f, 0);
        leftArm.transform.localScale = new Vector3(0.2f, 0.6f, 0.2f);
        
        GameObject rightArm = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightArm.name = "RightArm";
        rightArm.transform.SetParent(avatar.transform);
        rightArm.transform.localPosition = new Vector3(0.4f, 0.5f, 0);
        rightArm.transform.localScale = new Vector3(0.2f, 0.6f, 0.2f);
        
        // Legs (cubes)
        GameObject leftLeg = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leftLeg.name = "LeftLeg";
        leftLeg.transform.SetParent(avatar.transform);
        leftLeg.transform.localPosition = new Vector3(-0.2f, -0.25f, 0);
        leftLeg.transform.localScale = new Vector3(0.2f, 0.6f, 0.2f);
        
        GameObject rightLeg = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rightLeg.name = "RightLeg";
        rightLeg.transform.SetParent(avatar.transform);
        rightLeg.transform.localPosition = new Vector3(0.2f, -0.25f, 0);
        rightLeg.transform.localScale = new Vector3(0.2f, 0.6f, 0.2f);
        
        // Create a material
        Material avatarMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        avatarMaterial.color = avatarColor;
        AssetDatabase.CreateAsset(avatarMaterial, "Assets/Models/AvatarMaterial.mat");
        
        // Apply the material to all parts
        foreach (Renderer renderer in avatar.GetComponentsInChildren<Renderer>())
        {
            renderer.material = avatarMaterial;
        }
        
        // Save the prefab
        if (!Directory.Exists("Assets/Prefabs"))
        {
            Directory.CreateDirectory("Assets/Prefabs");
        }
        
        // Create the prefab
        PrefabUtility.SaveAsPrefabAsset(avatar, "Assets/Models/TS_PixelAvatarMain.prefab");
        
        // Destroy the scene instance
        DestroyImmediate(avatar);
        
        Debug.Log("Created placeholder avatar model at Assets/Models/TS_PixelAvatarMain.prefab");
        AssetDatabase.Refresh();
    }

    private void CreateMotorcyclePlaceholder()
    {
        // Create directory if it doesn't exist
        Directory.CreateDirectory("Assets/Models");
        
        // Create a simple GameObject with primitive shapes
        GameObject motorcycle = new GameObject("TS_Motorcycle");
        
        // Body (cube)
        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.name = "Body";
        body.transform.SetParent(motorcycle.transform);
        body.transform.localPosition = new Vector3(0, 0.5f, 0);
        body.transform.localScale = new Vector3(0.5f, 0.2f, 1.5f);
        
        // Seat (cube)
        GameObject seat = GameObject.CreatePrimitive(PrimitiveType.Cube);
        seat.name = "Seat";
        seat.transform.SetParent(motorcycle.transform);
        seat.transform.localPosition = new Vector3(0, 0.6f, 0);
        seat.transform.localScale = new Vector3(0.4f, 0.1f, 0.5f);
        
        // Handlebars (cylinder)
        GameObject handlebars = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        handlebars.name = "Handlebars";
        handlebars.transform.SetParent(motorcycle.transform);
        handlebars.transform.localPosition = new Vector3(0, 0.7f, 0.6f);
        handlebars.transform.localRotation = Quaternion.Euler(90, 0, 0);
        handlebars.transform.localScale = new Vector3(0.05f, 0.4f, 0.05f);
        
        // Front wheel (cylinder)
        GameObject frontWheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        frontWheel.name = "FrontWheel";
        frontWheel.transform.SetParent(motorcycle.transform);
        frontWheel.transform.localPosition = new Vector3(0, 0.25f, 0.7f);
        frontWheel.transform.localRotation = Quaternion.Euler(0, 0, 90);
        frontWheel.transform.localScale = new Vector3(0.5f, 0.1f, 0.5f);
        
        // Rear wheel (cylinder)
        GameObject rearWheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rearWheel.name = "RearWheel";
        rearWheel.transform.SetParent(motorcycle.transform);
        rearWheel.transform.localPosition = new Vector3(0, 0.25f, -0.7f);
        rearWheel.transform.localRotation = Quaternion.Euler(0, 0, 90);
        rearWheel.transform.localScale = new Vector3(0.5f, 0.1f, 0.5f);
        
        // Create a material
        Material motorcycleMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        motorcycleMaterial.color = motorcycleColor;
        AssetDatabase.CreateAsset(motorcycleMaterial, "Assets/Models/MotorcycleMaterial.mat");
        
        // Apply the material to all parts
        foreach (Renderer renderer in motorcycle.GetComponentsInChildren<Renderer>())
        {
            renderer.material = motorcycleMaterial;
        }
        
        // Save the prefab
        if (!Directory.Exists("Assets/Prefabs"))
        {
            Directory.CreateDirectory("Assets/Prefabs");
        }
        
        // Create the prefab
        PrefabUtility.SaveAsPrefabAsset(motorcycle, "Assets/Models/TS_Motorcycle.prefab");
        
        // Destroy the scene instance
        DestroyImmediate(motorcycle);
        
        Debug.Log("Created placeholder motorcycle model at Assets/Models/TS_Motorcycle.prefab");
        AssetDatabase.Refresh();
    }
} 