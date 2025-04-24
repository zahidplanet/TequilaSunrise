using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace TequilaSunrise.UI
{
    /// <summary>
    /// Manages mobile input controls including joysticks and action buttons.
    /// Provides input data to gameplay systems through a clean API.
    /// </summary>
    public class MobileInputController : MonoBehaviour
    {
        [Serializable]
        public class JoystickMapping
        {
            public string joystickId;
            public Joystick joystick;
            [Tooltip("How to process the joystick values")]
            public JoystickUseType useType = JoystickUseType.MovementXZ;
            [Tooltip("Scale the raw joystick value")]
            public float sensitivity = 1.0f;
            [Tooltip("Clamp X/Y values to nearest cardinal direction")]
            public bool snapToCardinalDirections = false;
            [Tooltip("Minimum angle (degrees) between directions when snapping")]
            [Range(1f, 90f)]
            public float cardinalDeadZoneAngle = 45f;
        }

        [Serializable]
        public class ButtonMapping
        {
            public string buttonId;
            public ActionButton button;
            [Tooltip("System that will receive this button's input")]
            public InputReceiver targetSystem;
        }

        public enum JoystickUseType
        {
            MovementXZ,     // X/Z movement (typical character movement)
            RotationY,      // Y-axis rotation (character turning)
            Look,           // Camera look/rotation
            Custom          // Custom processing of values
        }

        public enum InputReceiver
        {
            Player,
            Vehicle,
            UI,
            Camera,
            Custom
        }

        [Header("Controls Setup")]
        [SerializeField] private bool mobileControlsEnabled = true;
        [SerializeField] private GameObject mobileControlsRoot;
        [SerializeField] private bool autoHideOnDesktop = true;
        [SerializeField] private float controlsAlpha = 0.6f;

        [Header("Joysticks")]
        [SerializeField] private List<JoystickMapping> joysticks = new List<JoystickMapping>();

        [Header("Buttons")]
        [SerializeField] private List<ButtonMapping> buttons = new List<ButtonMapping>();

        [Header("Adapting to Device")]
        [SerializeField] private bool adaptToSafeArea = true;
        [SerializeField] private RectTransform leftControlsArea;
        [SerializeField] private RectTransform rightControlsArea;

        [Header("Movement Controls")]
        [SerializeField] private Joystick movementJoystick;
        [SerializeField] private Joystick lookJoystick;
        [SerializeField] private bool invertYAxis = false;
        [SerializeField] private float joystickDeadZone = 0.1f;
        
        [Header("Action Buttons")]
        [SerializeField] private ActionButton[] actionButtons;
        [SerializeField] private ActionButton jumpButton;
        [SerializeField] private ActionButton interactButton;
        [SerializeField] private ActionButton sprintButton;
        
        [Header("Control Settings")]
        [SerializeField] private bool dynamicControls = true;
        [SerializeField] private float controlOpacity = 0.7f;
        [SerializeField] private bool fadeWhenIdle = true;
        [SerializeField] private float fadeDuration = 0.3f;
        [SerializeField] private float idleOpacity = 0.4f;
        [SerializeField] private GameObject controlsParent;
        
        [Header("Events")]
        public InputEvent OnButtonPressed;
        public InputEvent OnButtonReleased;
        public InputEvent OnButtonHeld;
        public UnityEvent<Vector2> OnMovementChanged;
        public UnityEvent<Vector2> OnLookChanged;
        
        // Events
        public event Action<string, Vector2> OnJoystickMoved;
        public event Action<string, bool> OnButtonStateChanged;
        
        // Movement values from joysticks
        private Dictionary<string, Vector2> _joystickValues = new Dictionary<string, Vector2>();
        private Dictionary<string, bool> _buttonStates = new Dictionary<string, bool>();
        private Dictionary<string, bool> _buttonHoldStates = new Dictionary<string, bool>();
        private bool _initialized = false;
        private CanvasGroup _canvasGroup;
        private bool _isControlsVisible = true;
        private float _lastInputTime;
        
        // Properties for external systems to access input
        public Vector2 MovementInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool IsJumping { get; private set; }
        public bool IsSprinting { get; private set; }
        public bool IsInteracting { get; private set; }

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null && mobileControlsRoot != null)
                _canvasGroup = mobileControlsRoot.GetComponent<CanvasGroup>();

            InitializeControls();
        }

        private void Start()
        {
            if (adaptToSafeArea)
            {
                ApplySafeArea();
            }

            // Auto-hide on desktop if configured
            if (autoHideOnDesktop && !Application.isMobilePlatform)
            {
                SetControlsVisible(false);
            }
            else
            {
                SetControlsVisible(mobileControlsEnabled);
            }
        }

        private void Update()
        {
            // Pressing Escape key toggles mobile controls visibility in editor/desktop
            if (Input.GetKeyDown(KeyCode.M) && !Application.isMobilePlatform)
            {
                ToggleControlsVisibility();
            }

            // Process joystick input
            ProcessMovementInput();
            ProcessLookInput();
            
            // Update UI visibility based on activity
            if (fadeWhenIdle && _canvasGroup != null)
            {
                UpdateControlsVisibility();
            }
        }

        /// <summary>
        /// Initializes all control input components
        /// </summary>
        private void InitializeControls()
        {
            if (_initialized) return;

            // Initialize joysticks
            foreach (var mapping in joysticks)
            {
                if (mapping.joystick == null) continue;

                _joystickValues[mapping.joystickId] = Vector2.zero;
                
                // Subscribe to joystick events
                mapping.joystick.OnJoystickMoved.AddListener((Vector2 value) => {
                    // Process value based on joystick use type
                    Vector2 processedValue = ProcessJoystickInput(value, mapping);
                    _joystickValues[mapping.joystickId] = processedValue;
                    OnJoystickMoved?.Invoke(mapping.joystickId, processedValue);
                });
            }

            // Initialize buttons
            foreach (var mapping in buttons)
            {
                if (mapping.button == null) continue;

                _buttonStates[mapping.buttonId] = false;
                _buttonHoldStates[mapping.buttonId] = false;
                
                // Subscribe to button events
                mapping.button.OnButtonDown.AddListener(() => {
                    _buttonStates[mapping.buttonId] = true;
                    OnButtonStateChanged?.Invoke(mapping.buttonId, true);
                });
                
                mapping.button.OnButtonUp.AddListener(() => {
                    _buttonStates[mapping.buttonId] = false;
                    _buttonHoldStates[mapping.buttonId] = false;
                    OnButtonStateChanged?.Invoke(mapping.buttonId, false);
                });
                
                // Handle toggle buttons
                mapping.button.OnButtonToggled.AddListener((bool toggled) => {
                    _buttonStates[mapping.buttonId] = toggled;
                    OnButtonStateChanged?.Invoke(mapping.buttonId, toggled);
                });
            }

            _initialized = true;
        }

        /// <summary>
        /// Process raw joystick input based on its configured use type
        /// </summary>
        private Vector2 ProcessJoystickInput(Vector2 rawInput, JoystickMapping mapping)
        {
            // Apply sensitivity
            Vector2 processed = rawInput * mapping.sensitivity;
            
            // Snap to cardinal directions if configured
            if (mapping.snapToCardinalDirections && rawInput.sqrMagnitude > 0.01f)
            {
                float angle = Mathf.Atan2(rawInput.y, rawInput.x) * Mathf.Rad2Deg;
                
                // Normalize angle to 0-360
                if (angle < 0) angle += 360f;
                
                // Determine the nearest cardinal direction
                float cardinalAngle;
                if (angle <= 45f || angle > 315f) cardinalAngle = 0f;      // Right
                else if (angle > 45f && angle <= 135f) cardinalAngle = 90f; // Up
                else if (angle > 135f && angle <= 225f) cardinalAngle = 180f; // Left
                else cardinalAngle = 270f; // Down
                
                // Check if the input angle is close enough to the cardinal angle
                float angleDiff = Mathf.Abs(Mathf.DeltaAngle(angle, cardinalAngle));
                if (angleDiff <= mapping.cardinalDeadZoneAngle)
                {
                    // Snap to the cardinal direction
                    float magnitude = processed.magnitude;
                    processed.x = Mathf.Cos(cardinalAngle * Mathf.Deg2Rad) * magnitude;
                    processed.y = Mathf.Sin(cardinalAngle * Mathf.Deg2Rad) * magnitude;
                }
            }
            
            return processed;
        }

        /// <summary>
        /// Toggles the visibility of mobile controls
        /// </summary>
        public void ToggleControlsVisibility()
        {
            SetControlsVisible(!mobileControlsEnabled);
        }

        /// <summary>
        /// Sets the visibility of mobile controls
        /// </summary>
        public void SetControlsVisible(bool visible)
        {
            mobileControlsEnabled = visible;
            
            // Update canvas group if it exists
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = visible ? controlsAlpha : 0f;
                _canvasGroup.blocksRaycasts = visible;
                _canvasGroup.interactable = visible;
            }
            else if (mobileControlsRoot != null)
            {
                mobileControlsRoot.SetActive(visible);
            }
        }

        /// <summary>
        /// Adapts the controls to the device's safe area
        /// </summary>
        private void ApplySafeArea()
        {
            if (!adaptToSafeArea) return;
            
            // Get the safe area
            Rect safeArea = Screen.safeArea;
            
            // Calculate anchor adjustments
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null) return;
            
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            
            // Calculate the safe area as normalized values (0-1)
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            
            // Apply to control areas if they exist
            if (leftControlsArea != null)
            {
                leftControlsArea.anchorMin = new Vector2(anchorMin.x, leftControlsArea.anchorMin.y);
                leftControlsArea.anchorMax = new Vector2(leftControlsArea.anchorMax.x, anchorMax.y);
            }
            
            if (rightControlsArea != null)
            {
                rightControlsArea.anchorMin = new Vector2(rightControlsArea.anchorMin.x, anchorMin.y);
                rightControlsArea.anchorMax = new Vector2(anchorMax.x, anchorMax.y);
            }
        }

        /// <summary>
        /// Gets the current value for a joystick
        /// </summary>
        public Vector2 GetJoystickValue(string joystickId)
        {
            if (_joystickValues.TryGetValue(joystickId, out Vector2 value))
            {
                return value;
            }
            return Vector2.zero;
        }

        /// <summary>
        /// Gets the current state of a button
        /// </summary>
        public bool GetButtonState(string buttonId)
        {
            if (_buttonStates.TryGetValue(buttonId, out bool state))
            {
                return state;
            }
            return false;
        }

        /// <summary>
        /// Simulates pressing a button
        /// </summary>
        public void SimulateButtonPress(string buttonId)
        {
            foreach (var mapping in buttons)
            {
                if (mapping.buttonId == buttonId && mapping.button != null)
                {
                    mapping.button.SimulateButtonPress();
                    break;
                }
            }
        }

        /// <summary>
        /// Sets a joystick's value directly (for external input sources)
        /// </summary>
        public void SetJoystickValue(string joystickId, Vector2 value)
        {
            foreach (var mapping in joysticks)
            {
                if (mapping.joystickId == joystickId && mapping.joystick != null)
                {
                    Vector2 processedValue = ProcessJoystickInput(value, mapping);
                    _joystickValues[joystickId] = processedValue;
                    OnJoystickMoved?.Invoke(joystickId, processedValue);
                    break;
                }
            }
        }

        /// <summary>
        /// Sets the active state of a specific control group
        /// </summary>
        public void SetControlGroupActive(string groupName, bool active)
        {
            // Assuming control groups are child GameObjects with matching names
            Transform group = transform.Find(groupName);
            if (group != null)
            {
                group.gameObject.SetActive(active);
            }
        }

        /// <summary>
        /// Gets a list of all available control IDs
        /// </summary>
        public string[] GetAllControlIds()
        {
            List<string> ids = new List<string>();
            
            foreach (var mapping in joysticks)
            {
                ids.Add(mapping.joystickId);
            }
            
            foreach (var mapping in buttons)
            {
                ids.Add(mapping.buttonId);
            }
            
            return ids.ToArray();
        }

        #region Input Processing
        
        private void ProcessMovementInput()
        {
            if (movementJoystick == null) return;
            
            Vector2 rawInput = new Vector2(movementJoystick.Horizontal, movementJoystick.Vertical);
            
            // Apply deadzone
            if (rawInput.magnitude < joystickDeadZone)
            {
                rawInput = Vector2.zero;
            }
            else
            {
                // Normalize and scale
                rawInput = rawInput.normalized * Mathf.InverseLerp(joystickDeadZone, 1f, rawInput.magnitude);
            }
            
            MovementInput = rawInput;
            OnMovementChanged?.Invoke(MovementInput);
            
            // Record input time for fading
            if (rawInput.magnitude > 0)
            {
                _lastInputTime = Time.time;
            }
        }
        
        private void ProcessLookInput()
        {
            if (lookJoystick == null) return;
            
            Vector2 rawInput = new Vector2(lookJoystick.Horizontal, lookJoystick.Vertical);
            
            // Apply inversion if needed
            if (invertYAxis)
            {
                rawInput.y = -rawInput.y;
            }
            
            // Apply deadzone
            if (rawInput.magnitude < joystickDeadZone)
            {
                rawInput = Vector2.zero;
            }
            else
            {
                // Normalize and scale
                rawInput = rawInput.normalized * Mathf.InverseLerp(joystickDeadZone, 1f, rawInput.magnitude);
            }
            
            LookInput = rawInput;
            OnLookChanged?.Invoke(LookInput);
            
            // Record input time for fading
            if (rawInput.magnitude > 0)
            {
                _lastInputTime = Time.time;
            }
        }
        
        #endregion
        
        #region UI Visibility Management
        
        private void UpdateControlsVisibility()
        {
            // Check for any input activity
            bool hasInput = MovementInput.magnitude > 0 || 
                           LookInput.magnitude > 0 || 
                           IsJumping || 
                           IsSprinting || 
                           IsInteracting;
            
            foreach (var state in _buttonStates.Values)
            {
                if (state)
                {
                    hasInput = true;
                    break;
                }
            }
            
            // Determine if controls should be fully visible
            bool shouldBeVisible = hasInput || (Time.time - _lastInputTime < 2.0f);
            
            // Only update if state changes
            if (shouldBeVisible != _isControlsVisible)
            {
                _isControlsVisible = shouldBeVisible;
                
                // Animate opacity change
                float targetOpacity = shouldBeVisible ? controlOpacity : idleOpacity;
                LeanTween.cancel(_canvasGroup.gameObject);
                LeanTween.alphaCanvas(_canvasGroup, targetOpacity, fadeDuration)
                    .setEase(LeanTweenType.easeInOutQuad);
            }
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Check if a specific button is currently pressed
        /// </summary>
        public bool IsButtonPressed(string buttonId)
        {
            if (string.IsNullOrEmpty(buttonId) || !_buttonStates.ContainsKey(buttonId))
                return false;
                
            return _buttonStates[buttonId];
        }
        
        /// <summary>
        /// Check if a specific button is being held
        /// </summary>
        public bool IsButtonHeld(string buttonId)
        {
            if (string.IsNullOrEmpty(buttonId) || !_buttonHoldStates.ContainsKey(buttonId))
                return false;
                
            return _buttonHoldStates[buttonId];
        }
        
        /// <summary>
        /// Show or hide the mobile controls
        /// </summary>
        public void SetControlsVisibility(bool visible)
        {
            if (_canvasGroup == null) return;
            
            LeanTween.cancel(_canvasGroup.gameObject);
            if (visible)
            {
                LeanTween.alphaCanvas(_canvasGroup, controlOpacity, fadeDuration)
                    .setEase(LeanTweenType.easeInOutQuad);
                _canvasGroup.blocksRaycasts = true;
                _canvasGroup.interactable = true;
            }
            else
            {
                LeanTween.alphaCanvas(_canvasGroup, 0f, fadeDuration)
                    .setEase(LeanTweenType.easeInOutQuad)
                    .setOnComplete(() => {
                        _canvasGroup.blocksRaycasts = false;
                        _canvasGroup.interactable = false;
                    });
            }
        }
        
        /// <summary>
        /// Get the movement input as a Vector3 (useful for character controllers)
        /// </summary>
        public Vector3 GetMovementInputWorld(Transform relativeTo = null)
        {
            if (relativeTo == null)
            {
                // Default to camera-relative if no transform specified
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    Vector3 forward = mainCamera.transform.forward;
                    Vector3 right = mainCamera.transform.right;
                    
                    // Project onto horizontal plane
                    forward.y = 0;
                    right.y = 0;
                    forward.Normalize();
                    right.Normalize();
                    
                    return forward * MovementInput.y + right * MovementInput.x;
                }
                else
                {
                    return new Vector3(MovementInput.x, 0, MovementInput.y);
                }
            }
            else
            {
                // Calculate movement relative to the provided transform
                Vector3 forward = relativeTo.forward;
                Vector3 right = relativeTo.right;
                
                // Project onto horizontal plane if needed for character movement
                forward.y = 0;
                right.y = 0;
                forward.Normalize();
                right.Normalize();
                
                return forward * MovementInput.y + right * MovementInput.x;
            }
        }
        
        /// <summary>
        /// Vibrate the device (if supported)
        /// </summary>
        public void Vibrate(long milliseconds = 100)
        {
            #if UNITY_ANDROID || UNITY_IOS
            if (milliseconds <= 0) 
            {
                Handheld.Vibrate();
            } 
            else 
            {
                // Only supported on Android
                #if UNITY_ANDROID
                if (Application.platform == RuntimePlatform.Android)
                {
                    using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                    using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    using (AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator"))
                    {
                        vibrator.Call("vibrate", milliseconds);
                    }
                }
                else
                {
                    Handheld.Vibrate();
                }
                #else
                Handheld.Vibrate();
                #endif
            }
            #endif
        }
        
        /// <summary>
        /// Simulates a button press programmatically
        /// </summary>
        public void SimulateButtonPress(string buttonId)
        {
            foreach (var button in actionButtons)
            {
                if (button != null && button.ButtonId == buttonId)
                {
                    button.Press();
                    return;
                }
            }
            
            // Check specialized buttons
            if (jumpButton != null && jumpButton.ButtonId == buttonId)
            {
                jumpButton.Press();
            }
            else if (interactButton != null && interactButton.ButtonId == buttonId)
            {
                interactButton.Press();
            }
            else if (sprintButton != null && sprintButton.ButtonId == buttonId)
            {
                sprintButton.Press();
            }
        }
        
        /// <summary>
        /// Reset all input states
        /// </summary>
        public void ResetAllInput()
        {
            MovementInput = Vector2.zero;
            LookInput = Vector2.zero;
            IsJumping = false;
            IsSprinting = false;
            IsInteracting = false;
            
            foreach (var buttonId in _buttonStates.Keys)
            {
                _buttonStates[buttonId] = false;
                _buttonHoldStates[buttonId] = false;
            }
            
            OnMovementChanged?.Invoke(Vector2.zero);
            OnLookChanged?.Invoke(Vector2.zero);
        }
        
        #endregion
    }
} 