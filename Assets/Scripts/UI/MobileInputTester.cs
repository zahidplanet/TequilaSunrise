using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TequilaSunrise.UI
{
    /// <summary>
    /// Test script to demonstrate the mobile input system functionality
    /// </summary>
    public class MobileInputTester : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MobileInputController inputController;
        [SerializeField] private GameObject visualIndicator;
        [SerializeField] private RectTransform movementIndicator;
        [SerializeField] private RectTransform lookIndicator;
        [SerializeField] private TextMeshProUGUI statusText;
        [SerializeField] private TextMeshProUGUI buttonStatusText;
        
        [Header("Test Object")]
        [SerializeField] private Transform testCube;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotateSpeed = 100f;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color actionColor = Color.red;
        [SerializeField] private float jumpHeight = 2f;
        
        // Private variables
        private Renderer _cubeRenderer;
        private Vector3 _startPosition;
        private bool _isJumping = false;
        private float _jumpVelocity = 0f;
        private float _gravity = -9.8f;
        
        private void Start()
        {
            // Initialize
            if (testCube != null)
            {
                _cubeRenderer = testCube.GetComponent<Renderer>();
                _startPosition = testCube.position;
            }
            
            // Subscribe to input events
            if (inputController != null)
            {
                inputController.OnMovementChanged.AddListener(OnMovementChanged);
                inputController.OnLookChanged.AddListener(OnLookChanged);
                inputController.OnButtonPressed.AddListener(OnButtonPressed);
                inputController.OnButtonReleased.AddListener(OnButtonReleased);
                inputController.OnButtonHeld.AddListener(OnButtonHeld);
            }
            else
            {
                Debug.LogError("InputController not assigned to MobileInputTester");
            }
            
            UpdateStatusText("Ready");
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from input events
            if (inputController != null)
            {
                inputController.OnMovementChanged.RemoveListener(OnMovementChanged);
                inputController.OnLookChanged.RemoveListener(OnLookChanged);
                inputController.OnButtonPressed.RemoveListener(OnButtonPressed);
                inputController.OnButtonReleased.RemoveListener(OnButtonReleased);
                inputController.OnButtonHeld.RemoveListener(OnButtonHeld);
            }
        }
        
        private void Update()
        {
            if (inputController == null || testCube == null) return;
            
            // Movement based on joystick input
            MoveCube();
            
            // Handle jumping
            HandleJump();
            
            // Update UI indicators
            UpdateIndicators();
            
            // Update button status text
            UpdateButtonStatusText();
        }
        
        private void MoveCube()
        {
            if (testCube == null) return;
            
            // Apply movement using controller's helper method
            Vector3 moveDirection = inputController.GetMovementInputWorld();
            
            if (moveDirection.magnitude > 0.1f)
            {
                // Move the cube
                testCube.position += moveDirection * moveSpeed * Time.deltaTime;
                
                // Update status
                UpdateStatusText("Moving: " + moveDirection.ToString("F2"));
            }
            
            // Apply rotation from look input
            Vector2 lookInput = inputController.LookInput;
            if (lookInput.magnitude > 0.1f)
            {
                testCube.Rotate(0, lookInput.x * rotateSpeed * Time.deltaTime, 0);
                UpdateStatusText("Rotating: " + lookInput.ToString("F2"));
            }
        }
        
        private void HandleJump()
        {
            if (testCube == null) return;
            
            // Check if jump button is pressed
            if (inputController.IsJumping && !_isJumping)
            {
                _isJumping = true;
                _jumpVelocity = Mathf.Sqrt(2 * jumpHeight * -_gravity);
                UpdateStatusText("Jumping!");
            }
            
            // Apply jump physics
            if (_isJumping)
            {
                _jumpVelocity += _gravity * Time.deltaTime;
                testCube.position += new Vector3(0, _jumpVelocity * Time.deltaTime, 0);
                
                // Check if landed
                if (testCube.position.y <= _startPosition.y)
                {
                    testCube.position = new Vector3(testCube.position.x, _startPosition.y, testCube.position.z);
                    _isJumping = false;
                    UpdateStatusText("Landed");
                }
            }
        }
        
        private void UpdateIndicators()
        {
            if (movementIndicator != null)
            {
                // Update movement indicator position
                Vector2 normalizedPos = inputController.MovementInput * 50f; // Scale for visibility
                movementIndicator.anchoredPosition = normalizedPos;
                
                // Update size based on input magnitude
                float size = Mathf.Lerp(10f, 20f, inputController.MovementInput.magnitude);
                movementIndicator.sizeDelta = new Vector2(size, size);
            }
            
            if (lookIndicator != null)
            {
                // Update look indicator position
                Vector2 normalizedPos = inputController.LookInput * 50f; // Scale for visibility
                lookIndicator.anchoredPosition = normalizedPos;
                
                // Update size based on input magnitude
                float size = Mathf.Lerp(10f, 20f, inputController.LookInput.magnitude);
                lookIndicator.sizeDelta = new Vector2(size, size);
            }
        }
        
        private void UpdateButtonStatusText()
        {
            if (buttonStatusText == null) return;
            
            string status = "";
            
            // Add status for all known buttons
            status += "Jump: " + (inputController.IsJumping ? "PRESSED" : "Released") + "\n";
            status += "Interact: " + (inputController.IsInteracting ? "PRESSED" : "Released") + "\n";
            status += "Sprint: " + (inputController.IsSprinting ? "PRESSED" : "Released") + "\n";
            
            // Update the text
            buttonStatusText.text = status;
        }
        
        #region Input Event Handlers
        
        private void OnMovementChanged(Vector2 movement)
        {
            // This is handled in Update
        }
        
        private void OnLookChanged(Vector2 look)
        {
            // This is handled in Update
        }
        
        private void OnButtonPressed(string buttonId, float value)
        {
            UpdateStatusText("Button Pressed: " + buttonId);
            
            // Change cube color based on button
            if (_cubeRenderer != null)
            {
                _cubeRenderer.material.color = actionColor;
            }
            
            // Vibrate when button is pressed
            inputController.Vibrate(50);
        }
        
        private void OnButtonReleased(string buttonId, float value)
        {
            UpdateStatusText("Button Released: " + buttonId);
            
            // Restore cube color
            if (_cubeRenderer != null)
            {
                _cubeRenderer.material.color = normalColor;
            }
        }
        
        private void OnButtonHeld(string buttonId, float value)
        {
            UpdateStatusText("Button Held: " + buttonId);
            
            // Make the cube larger when button is held
            if (testCube != null)
            {
                testCube.localScale = Vector3.one * 1.5f;
            }
        }
        
        #endregion
        
        private void UpdateStatusText(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
        }
        
        public void ResetTest()
        {
            if (testCube != null)
            {
                testCube.position = _startPosition;
                testCube.rotation = Quaternion.identity;
                testCube.localScale = Vector3.one;
            }
            
            if (_cubeRenderer != null)
            {
                _cubeRenderer.material.color = normalColor;
            }
            
            _isJumping = false;
            _jumpVelocity = 0f;
            
            UpdateStatusText("Test Reset");
            
            // Reset input
            inputController.ResetAllInput();
        }
    }
} 