using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
namespace TequilaSunrise.UI.Editor
{
    /// <summary>
    /// Utility script to help setup the mobile input UI system
    /// </summary>
    public class MobileInputSetup : MonoBehaviour
    {
        [MenuItem("TequilaSunrise/UI/Create Mobile Controls")]
        public static void CreateMobileControls()
        {
            // Create canvas if it doesn't exist
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("Mobile Input Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
                
                // Setup canvas scaler
                CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1080, 1920);
                scaler.matchWidthOrHeight = 0.5f;
            }

            // Create a controls container
            GameObject controlsContainer = new GameObject("Mobile Controls");
            controlsContainer.transform.SetParent(canvas.transform, false);
            RectTransform containerRect = controlsContainer.AddComponent<RectTransform>();
            containerRect.anchorMin = Vector2.zero;
            containerRect.anchorMax = Vector2.one;
            containerRect.offsetMin = Vector2.zero;
            containerRect.offsetMax = Vector2.zero;
            
            // Add canvas group for fading
            CanvasGroup canvasGroup = controlsContainer.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0.8f;
            
            // Create left joystick area
            GameObject leftArea = CreateControlArea("Left Control Area", containerRect, true);
            
            // Create right joystick area
            GameObject rightArea = CreateControlArea("Right Control Area", containerRect, false);
            
            // Create movement joystick
            GameObject movementJoystick = CreateJoystick("Movement Joystick", leftArea.GetComponent<RectTransform>());
            
            // Create look joystick
            GameObject lookJoystick = CreateJoystick("Look Joystick", rightArea.GetComponent<RectTransform>());
            
            // Create action buttons
            GameObject jumpButton = CreateActionButton("Jump Button", rightArea.GetComponent<RectTransform>(), new Vector2(-100, 250));
            GameObject interactButton = CreateActionButton("Interact Button", rightArea.GetComponent<RectTransform>(), new Vector2(-250, 100));
            GameObject sprintButton = CreateActionButton("Sprint Button", rightArea.GetComponent<RectTransform>(), new Vector2(-100, 100));
            
            // Set button IDs
            if (jumpButton.TryGetComponent<ActionButton>(out var jumpActionButton))
            {
                jumpActionButton.SetButtonId("jump");
            }
            
            if (interactButton.TryGetComponent<ActionButton>(out var interactActionButton))
            {
                interactActionButton.SetButtonId("interact");
            }
            
            if (sprintButton.TryGetComponent<ActionButton>(out var sprintActionButton))
            {
                sprintActionButton.SetButtonId("sprint");
            }
            
            // Create the controller
            GameObject controllerObj = new GameObject("Mobile Input Controller");
            controllerObj.transform.SetParent(controlsContainer.transform, false);
            MobileInputController controller = controllerObj.AddComponent<MobileInputController>();
            
            // Hook up references
            if (controller != null)
            {
                SerializedObject serializedObject = new SerializedObject(controller);
                serializedObject.FindProperty("movementJoystick").objectReferenceValue = movementJoystick.GetComponent<Joystick>();
                serializedObject.FindProperty("lookJoystick").objectReferenceValue = lookJoystick.GetComponent<Joystick>();
                serializedObject.FindProperty("jumpButton").objectReferenceValue = jumpButton.GetComponent<ActionButton>();
                serializedObject.FindProperty("interactButton").objectReferenceValue = interactButton.GetComponent<ActionButton>();
                serializedObject.FindProperty("sprintButton").objectReferenceValue = sprintButton.GetComponent<ActionButton>();
                serializedObject.FindProperty("controlsParent").objectReferenceValue = controlsContainer;
                
                // Create array for action buttons
                SerializedProperty actionButtonsProperty = serializedObject.FindProperty("actionButtons");
                actionButtonsProperty.arraySize = 3;
                actionButtonsProperty.GetArrayElementAtIndex(0).objectReferenceValue = jumpButton.GetComponent<ActionButton>();
                actionButtonsProperty.GetArrayElementAtIndex(1).objectReferenceValue = interactButton.GetComponent<ActionButton>();
                actionButtonsProperty.GetArrayElementAtIndex(2).objectReferenceValue = sprintButton.GetComponent<ActionButton>();
                
                serializedObject.ApplyModifiedProperties();
            }
            
            // Create UI status display for testing
            GameObject statusPanel = CreateStatusPanel(containerRect);
            
            // Setup for testing
            CreateTestEnvironment(controlsContainer, controller, statusPanel);
            
            Debug.Log("Mobile controls created successfully!");
        }
        
        private static GameObject CreateControlArea(string name, RectTransform parent, bool isLeft)
        {
            GameObject area = new GameObject(name);
            area.transform.SetParent(parent, false);
            
            RectTransform rectTransform = area.AddComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(isLeft ? 0 : 0.5f, 0);
            rectTransform.anchorMax = new Vector2(isLeft ? 0.5f : 1, 1);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            
            return area;
        }
        
        private static GameObject CreateJoystick(string name, RectTransform parent)
        {
            // Create joystick container
            GameObject joystickObj = new GameObject(name);
            joystickObj.transform.SetParent(parent, false);
            
            RectTransform rectTransform = joystickObj.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(200, 200);
            rectTransform.anchorMin = new Vector2(0.5f, 0.2f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.2f);
            rectTransform.anchoredPosition = Vector2.zero;
            
            // Create background
            GameObject background = new GameObject("Background");
            background.transform.SetParent(joystickObj.transform, false);
            
            RectTransform bgRect = background.AddComponent<RectTransform>();
            bgRect.sizeDelta = new Vector2(150, 150);
            bgRect.anchoredPosition = Vector2.zero;
            
            Image bgImage = background.AddComponent<Image>();
            bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
            bgImage.raycastTarget = true;
            
            // Create handle
            GameObject handle = new GameObject("Handle");
            handle.transform.SetParent(background.transform, false);
            
            RectTransform handleRect = handle.AddComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(80, 80);
            handleRect.anchoredPosition = Vector2.zero;
            
            Image handleImage = handle.AddComponent<Image>();
            handleImage.color = new Color(0.8f, 0.8f, 0.8f, 0.8f);
            handleImage.raycastTarget = false;
            
            // Add joystick component
            Joystick joystick = joystickObj.AddComponent<Joystick>();
            
            // Set references
            SerializedObject serializedObject = new SerializedObject(joystick);
            serializedObject.FindProperty("background").objectReferenceValue = bgRect;
            serializedObject.FindProperty("handle").objectReferenceValue = handleRect;
            serializedObject.FindProperty("backgroundImage").objectReferenceValue = bgImage;
            serializedObject.FindProperty("handleImage").objectReferenceValue = handleImage;
            serializedObject.ApplyModifiedProperties();
            
            return joystickObj;
        }
        
        private static GameObject CreateActionButton(string name, RectTransform parent, Vector2 position)
        {
            // Create button game object
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent, false);
            
            RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(120, 120);
            rectTransform.anchorMin = new Vector2(1, 0);
            rectTransform.anchorMax = new Vector2(1, 0);
            rectTransform.anchoredPosition = position;
            
            // Create visual
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.8f, 0.8f, 0.8f, 0.7f);
            buttonImage.raycastTarget = true;
            
            // Create text label
            GameObject textObj = new GameObject("Label");
            textObj.transform.SetParent(buttonObj.transform, false);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            TextMeshProUGUI tmpText = textObj.AddComponent<TextMeshProUGUI>();
            tmpText.text = name.Replace(" Button", "");
            tmpText.color = Color.black;
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.fontSize = 20;
            
            // Add action button component
            ActionButton actionButton = buttonObj.AddComponent<ActionButton>();
            
            // Set references
            SerializedObject serializedObject = new SerializedObject(actionButton);
            serializedObject.FindProperty("buttonImage").objectReferenceValue = buttonImage;
            serializedObject.ApplyModifiedProperties();
            
            return buttonObj;
        }
        
        private static GameObject CreateStatusPanel(RectTransform parent)
        {
            // Create panel
            GameObject panel = new GameObject("Status Panel");
            panel.transform.SetParent(parent, false);
            
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 1);
            panelRect.anchorMax = new Vector2(1, 1);
            panelRect.offsetMin = new Vector2(20, -200);
            panelRect.offsetMax = new Vector2(-20, -20);
            
            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.5f);
            
            // Create status text
            GameObject statusObj = new GameObject("Status Text");
            statusObj.transform.SetParent(panel.transform, false);
            
            RectTransform statusRect = statusObj.AddComponent<RectTransform>();
            statusRect.anchorMin = new Vector2(0, 0.5f);
            statusRect.anchorMax = new Vector2(1, 1);
            statusRect.offsetMin = new Vector2(10, 0);
            statusRect.offsetMax = new Vector2(-10, -10);
            
            TextMeshProUGUI statusText = statusObj.AddComponent<TextMeshProUGUI>();
            statusText.text = "Mobile Input Status";
            statusText.fontSize = 24;
            statusText.color = Color.white;
            statusText.alignment = TextAlignmentOptions.Left;
            
            // Create button status text
            GameObject buttonStatusObj = new GameObject("Button Status");
            buttonStatusObj.transform.SetParent(panel.transform, false);
            
            RectTransform buttonStatusRect = buttonStatusObj.AddComponent<RectTransform>();
            buttonStatusRect.anchorMin = new Vector2(0, 0);
            buttonStatusRect.anchorMax = new Vector2(1, 0.5f);
            buttonStatusRect.offsetMin = new Vector2(10, 10);
            buttonStatusRect.offsetMax = new Vector2(-10, 0);
            
            TextMeshProUGUI buttonStatusText = buttonStatusObj.AddComponent<TextMeshProUGUI>();
            buttonStatusText.text = "Button States";
            buttonStatusText.fontSize = 20;
            buttonStatusText.color = Color.white;
            buttonStatusText.alignment = TextAlignmentOptions.Left;
            
            return panel;
        }
        
        private static void CreateTestEnvironment(GameObject controlsContainer, MobileInputController controller, GameObject statusPanel)
        {
            // Create test object
            GameObject testObj = new GameObject("Mobile Input Test");
            testObj.transform.SetParent(controlsContainer.transform.parent, false);
            
            MobileInputTester tester = testObj.AddComponent<MobileInputTester>();
            
            // Set up references
            SerializedObject serializedObject = new SerializedObject(tester);
            serializedObject.FindProperty("inputController").objectReferenceValue = controller;
            serializedObject.FindProperty("statusText").objectReferenceValue = statusPanel.transform.Find("Status Text").GetComponent<TextMeshProUGUI>();
            serializedObject.FindProperty("buttonStatusText").objectReferenceValue = statusPanel.transform.Find("Button Status").GetComponent<TextMeshProUGUI>();
            
            // Create indicators
            GameObject movementIndicator = CreateIndicator("Movement Indicator", statusPanel.transform, Color.green);
            GameObject lookIndicator = CreateIndicator("Look Indicator", statusPanel.transform, Color.blue);
            
            // Set indicators
            serializedObject.FindProperty("movementIndicator").objectReferenceValue = movementIndicator.GetComponent<RectTransform>();
            serializedObject.FindProperty("lookIndicator").objectReferenceValue = lookIndicator.GetComponent<RectTransform>();
            
            // Create test cube
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Test Cube";
            cube.transform.position = new Vector3(0, 0.5f, 0);
            
            // Add material
            MeshRenderer renderer = cube.GetComponent<MeshRenderer>();
            if (renderer != null && renderer.sharedMaterial != null)
            {
                renderer.sharedMaterial = new Material(renderer.sharedMaterial);
                renderer.sharedMaterial.color = Color.white;
            }
            
            serializedObject.FindProperty("testCube").objectReferenceValue = cube.transform;
            serializedObject.ApplyModifiedProperties();
            
            // Create reset button
            GameObject resetButton = new GameObject("Reset Button");
            resetButton.transform.SetParent(statusPanel.transform, false);
            
            RectTransform resetRect = resetButton.AddComponent<RectTransform>();
            resetRect.anchorMin = new Vector2(0.8f, 0);
            resetRect.anchorMax = new Vector2(1, 0.2f);
            resetRect.offsetMin = new Vector2(10, 10);
            resetRect.offsetMax = new Vector2(-10, -10);
            
            Image resetImage = resetButton.AddComponent<Image>();
            resetImage.color = new Color(0.8f, 0.2f, 0.2f, 0.8f);
            
            GameObject resetText = new GameObject("Text");
            resetText.transform.SetParent(resetButton.transform, false);
            
            RectTransform resetTextRect = resetText.AddComponent<RectTransform>();
            resetTextRect.anchorMin = Vector2.zero;
            resetTextRect.anchorMax = Vector2.one;
            resetTextRect.offsetMin = Vector2.zero;
            resetTextRect.offsetMax = Vector2.zero;
            
            TextMeshProUGUI resetTmpText = resetText.AddComponent<TextMeshProUGUI>();
            resetTmpText.text = "RESET";
            resetTmpText.color = Color.white;
            resetTmpText.alignment = TextAlignmentOptions.Center;
            resetTmpText.fontSize = 24;
            
            // Add button component
            Button resetButtonComponent = resetButton.AddComponent<Button>();
            resetButtonComponent.targetGraphic = resetImage;
            
            // Add listener
            resetButtonComponent.onClick.AddListener(() => {
                if (testObj != null && testObj.TryGetComponent<MobileInputTester>(out var inputTester))
                {
                    inputTester.ResetTest();
                }
            });
            
            // Create ground plane
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(10, 1, 10);
            
            // Create test camera if none exists
            if (Camera.main == null)
            {
                GameObject cameraObj = new GameObject("Main Camera");
                cameraObj.tag = "MainCamera";
                Camera camera = cameraObj.AddComponent<Camera>();
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(0.2f, 0.2f, 0.2f);
                camera.transform.position = new Vector3(0, 5, -10);
                camera.transform.LookAt(Vector3.zero);
            }
        }
        
        private static GameObject CreateIndicator(string name, Transform parent, Color color)
        {
            GameObject indicator = new GameObject(name);
            indicator.transform.SetParent(parent, false);
            
            RectTransform rect = indicator.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(20, 20);
            rect.anchoredPosition = Vector2.zero;
            
            Image image = indicator.AddComponent<Image>();
            image.color = color;
            
            return indicator;
        }
    }
}
#endif 