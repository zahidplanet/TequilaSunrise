using System;
using UnityEngine;
using UnityEngine.Events;

namespace TequilaSunrise.Avatar
{
    /// <summary>
    /// Manages mobile input components and translates them into usable input values for character controllers
    /// </summary>
    public class MobileInputController : MonoBehaviour
    {
        [Header("Input Components")]
        [SerializeField] private Joystick movementJoystick;
        [SerializeField] private Joystick lookJoystick;
        [SerializeField] private TouchButton jumpButton;
        [SerializeField] private TouchButton interactButton;
        [SerializeField] private TouchButton crouchButton;
        [SerializeField] private TouchButton sprintButton;
        
        [Header("Configuration")]
        [SerializeField] private bool useLookJoystick = true;
        [SerializeField] private bool invertYLook = false;
        [SerializeField] private float lookSensitivity = 1.0f;
        [SerializeField] private float movementDeadZone = 0.1f;
        [SerializeField] private float lookDeadZone = 0.2f;
        
        [Header("Events")]
        public UnityEvent OnJumpStart;
        public UnityEvent OnJumpEnd;
        public UnityEvent OnInteractStart;
        public UnityEvent OnInteractEnd;
        public UnityEvent OnCrouchStart;
        public UnityEvent OnCrouchEnd;
        public UnityEvent OnSprintStart;
        public UnityEvent OnSprintEnd;
        
        // Input properties
        private Vector2 _movementInput = Vector2.zero;
        private Vector2 _lookInput = Vector2.zero;
        private bool _jumpInput = false;
        private bool _interactInput = false;
        private bool _crouchInput = false;
        private bool _sprintInput = false;
        
        // Public accessors
        public Vector2 MovementInput => _movementInput;
        public Vector2 LookInput => _lookInput;
        public bool JumpInput => _jumpInput;
        public bool InteractInput => _interactInput;
        public bool CrouchInput => _crouchInput;
        public bool SprintInput => _sprintInput;
        
        // Enables/disables the controller
        public bool IsEnabled { get; private set; } = true;
        
        private void Awake()
        {
            // Validate required components
            if (movementJoystick == null)
            {
                Debug.LogError("Movement joystick is not assigned to MobileInputController.");
            }
            
            // Set up button events
            SetupButtonEvents();
        }
        
        private void Start()
        {
            // Initialize UI components
            if (movementJoystick != null) movementJoystick.gameObject.SetActive(true);
            if (useLookJoystick && lookJoystick != null) lookJoystick.gameObject.SetActive(true);
            if (jumpButton != null) jumpButton.gameObject.SetActive(true);
            if (interactButton != null) interactButton.gameObject.SetActive(true);
            if (crouchButton != null) crouchButton.gameObject.SetActive(true);
            if (sprintButton != null) sprintButton.gameObject.SetActive(true);
        }
        
        private void Update()
        {
            if (!IsEnabled) return;
            
            // Update movement input
            if (movementJoystick != null)
            {
                _movementInput = new Vector2(
                    Mathf.Abs(movementJoystick.Horizontal) > movementDeadZone ? movementJoystick.Horizontal : 0f,
                    Mathf.Abs(movementJoystick.Vertical) > movementDeadZone ? movementJoystick.Vertical : 0f
                );
            }
            
            // Update look input
            if (useLookJoystick && lookJoystick != null)
            {
                float lookX = Mathf.Abs(lookJoystick.Horizontal) > lookDeadZone ? lookJoystick.Horizontal : 0f;
                float lookY = Mathf.Abs(lookJoystick.Vertical) > lookDeadZone ? lookJoystick.Vertical : 0f;
                
                // Apply inversion if needed
                if (invertYLook) lookY = -lookY;
                
                _lookInput = new Vector2(lookX, lookY) * lookSensitivity;
            }
            else
            {
                _lookInput = Vector2.zero;
            }
        }
        
        private void SetupButtonEvents()
        {
            // Jump button events
            if (jumpButton != null)
            {
                jumpButton.OnPress.AddListener(() => {
                    _jumpInput = true;
                    OnJumpStart?.Invoke();
                });
                
                jumpButton.OnRelease.AddListener(() => {
                    _jumpInput = false;
                    OnJumpEnd?.Invoke();
                });
            }
            
            // Interact button events
            if (interactButton != null)
            {
                interactButton.OnPress.AddListener(() => {
                    _interactInput = true;
                    OnInteractStart?.Invoke();
                });
                
                interactButton.OnRelease.AddListener(() => {
                    _interactInput = false;
                    OnInteractEnd?.Invoke();
                });
            }
            
            // Crouch button events
            if (crouchButton != null)
            {
                crouchButton.OnPress.AddListener(() => {
                    _crouchInput = true;
                    OnCrouchStart?.Invoke();
                });
                
                crouchButton.OnRelease.AddListener(() => {
                    _crouchInput = false;
                    OnCrouchEnd?.Invoke();
                });
            }
            
            // Sprint button events
            if (sprintButton != null)
            {
                sprintButton.OnPress.AddListener(() => {
                    _sprintInput = true;
                    OnSprintStart?.Invoke();
                });
                
                sprintButton.OnRelease.AddListener(() => {
                    _sprintInput = false;
                    OnSprintEnd?.Invoke();
                });
            }
        }
        
        /// <summary>
        /// Enables or disables the mobile input controller and all associated UI elements
        /// </summary>
        public void SetEnabled(bool enabled)
        {
            IsEnabled = enabled;
            
            // Enable/disable UI components
            if (movementJoystick != null) movementJoystick.gameObject.SetActive(enabled);
            if (useLookJoystick && lookJoystick != null) lookJoystick.gameObject.SetActive(enabled);
            if (jumpButton != null) jumpButton.gameObject.SetActive(enabled);
            if (interactButton != null) interactButton.gameObject.SetActive(enabled);
            if (crouchButton != null) crouchButton.gameObject.SetActive(enabled);
            if (sprintButton != null) sprintButton.gameObject.SetActive(enabled);
            
            // Reset input values when disabled
            if (!enabled)
            {
                _movementInput = Vector2.zero;
                _lookInput = Vector2.zero;
                _jumpInput = false;
                _interactInput = false;
                _crouchInput = false;
                _sprintInput = false;
            }
        }
        
        /// <summary>
        /// Simulates a button press for testing or custom triggers
        /// </summary>
        public void SimulateButtonPress(string buttonName)
        {
            switch (buttonName.ToLower())
            {
                case "jump":
                    if (jumpButton != null) jumpButton.SimulatePress();
                    break;
                case "interact":
                    if (interactButton != null) interactButton.SimulatePress();
                    break;
                case "crouch":
                    if (crouchButton != null) crouchButton.SimulatePress();
                    break;
                case "sprint":
                    if (sprintButton != null) sprintButton.SimulatePress();
                    break;
            }
        }
        
        /// <summary>
        /// Simulates a button release for testing or custom triggers
        /// </summary>
        public void SimulateButtonRelease(string buttonName)
        {
            switch (buttonName.ToLower())
            {
                case "jump":
                    if (jumpButton != null) jumpButton.SimulateRelease();
                    break;
                case "interact":
                    if (interactButton != null) interactButton.SimulateRelease();
                    break;
                case "crouch":
                    if (crouchButton != null) crouchButton.SimulateRelease();
                    break;
                case "sprint":
                    if (sprintButton != null) sprintButton.SimulateRelease();
                    break;
            }
        }
    }
} 