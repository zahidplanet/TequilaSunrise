using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TequilaSunrise.Avatar
{
    /// <summary>
    /// Handles the creation and setup of avatar prefabs at runtime
    /// </summary>
    public class AvatarPrefabCreator : MonoBehaviour
    {
        [Header("Avatar Model")]
        [Tooltip("The model to use for the avatar")]
        public GameObject avatarModel;
        
        [Header("Character Settings")]
        [Tooltip("Height of the character controller")]
        public float characterHeight = 1.8f;
        [Tooltip("Radius of the character controller")]
        public float characterRadius = 0.3f;
        [Tooltip("Step offset for the character controller")]
        public float stepOffset = 0.3f;
        [Tooltip("Scale of the avatar model")]
        public float avatarScale = 1.0f;
        
        [Header("Animation")]
        [Tooltip("Animator controller for the avatar")]
        public RuntimeAnimatorController animatorController;
        
        [Header("References")]
        [Tooltip("Joystick for movement control")]
        public GameObject joystickPrefab;
        [Tooltip("Jump button for jump control")]
        public GameObject jumpButtonPrefab;
        
        private CharacterController characterController;
        private Animator animator;
        
        public GameObject CreateAvatarPrefab()
        {
            if (avatarModel == null)
            {
                Debug.LogError("Avatar model not assigned!");
                return null;
            }
            
            // Create the avatar gameobject
            GameObject avatarObject = new GameObject("Avatar");
            
            // Add Character Controller
            characterController = avatarObject.AddComponent<CharacterController>();
            characterController.height = characterHeight;
            characterController.radius = characterRadius;
            characterController.stepOffset = stepOffset;
            
            // Add rigidbody for physics interactions
            Rigidbody rb = avatarObject.AddComponent<Rigidbody>();
            rb.isKinematic = true; // Let character controller handle movement
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            
            // Instantiate the model as child
            GameObject modelInstance = Instantiate(avatarModel, avatarObject.transform);
            modelInstance.name = "AvatarModel";
            modelInstance.transform.localScale = Vector3.one * avatarScale;
            modelInstance.transform.localPosition = Vector3.zero;
            
            // Add Animator
            animator = avatarObject.AddComponent<Animator>();
            if (animatorController != null)
            {
                animator.runtimeAnimatorController = animatorController;
            }
            else
            {
                Debug.LogWarning("No animator controller assigned to avatar!");
            }
            
            // Add AvatarController script
            AvatarController controller = avatarObject.AddComponent<AvatarController>();
            
            // Setup UI controls if available
            if (joystickPrefab != null && jumpButtonPrefab != null)
            {
                SetupControls(controller);
            }
            
            // Return the created avatar
            return avatarObject;
        }
        
        private void SetupControls(AvatarController controller)
        {
            // Create UI parent if needed
            GameObject uiCanvas = GameObject.FindGameObjectWithTag("UICanvas");
            if (uiCanvas == null)
            {
                uiCanvas = new GameObject("UI Canvas");
                Canvas canvas = uiCanvas.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                uiCanvas.AddComponent<UnityEngine.UI.CanvasScaler>();
                uiCanvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                uiCanvas.tag = "UICanvas";
            }
            
            // Instantiate joystick
            if (joystickPrefab != null)
            {
                GameObject joystick = Instantiate(joystickPrefab, uiCanvas.transform);
                joystick.name = "Joystick";
                
                // Find and assign the Joystick component to the AvatarController
                Joystick joystickComponent = joystick.GetComponent<Joystick>();
                if (joystickComponent != null && controller != null)
                {
                    // Access joystick property through reflection or serialized property
                    // as it might be private in the AvatarController
                    var serializedController = new UnityEditor.SerializedObject(controller);
                    var joystickProperty = serializedController.FindProperty("joystick");
                    if (joystickProperty != null)
                    {
                        joystickProperty.objectReferenceValue = joystickComponent;
                        serializedController.ApplyModifiedProperties();
                    }
                }
            }
            
            // Instantiate jump button
            if (jumpButtonPrefab != null)
            {
                GameObject jumpButton = Instantiate(jumpButtonPrefab, uiCanvas.transform);
                jumpButton.name = "JumpButton";
                
                // Find and assign the JumpButton component to the AvatarController
                JumpButton jumpButtonComponent = jumpButton.GetComponent<JumpButton>();
                if (jumpButtonComponent != null && controller != null)
                {
                    // Access jumpButton property through reflection or serialized property
                    var serializedController = new UnityEditor.SerializedObject(controller);
                    var jumpButtonProperty = serializedController.FindProperty("jumpButton");
                    if (jumpButtonProperty != null)
                    {
                        jumpButtonProperty.objectReferenceValue = jumpButton;
                        serializedController.ApplyModifiedProperties();
                    }
                    
                    // Set up the jump button to call the Jump method on the controller
                    UnityEngine.UI.Button button = jumpButton.GetComponent<UnityEngine.UI.Button>();
                    if (button != null)
                    {
                        button.onClick.AddListener(controller.Jump);
                    }
                }
            }
        }
    }
} 