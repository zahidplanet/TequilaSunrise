using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections;
using TequilaSunrise.UI.Utilities;

namespace TequilaSunrise.UI
{
    /// <summary>
    /// Interactive button for mobile controls that provides touch-based action inputs
    /// Flexible action button for mobile input interfaces that supports different interaction modes
    /// and visual feedback
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class ActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public enum ButtonBehavior
        {
            Momentary,   // Active only while pressed
            Toggle,      // Toggles between active/inactive states
            Hold         // Requires holding for specified duration
        }
        
        [Header("Button Identification")]
        [SerializeField] private string actionName = "Action";
        [SerializeField] private ButtonBehavior behavior = ButtonBehavior.Momentary;
        
        [Header("Visual Settings")]
        [SerializeField] private bool useVisualFeedback = true;
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite pressedSprite;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        [SerializeField] private Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        [SerializeField] private float colorTransitionSpeed = 8f;
        
        [Header("Animation")]
        [SerializeField] private bool useAnimation = true;
        [SerializeField] private float pressedScale = 0.9f;
        [SerializeField] private float animationSpeed = 8f;
        
        [Header("Hold Settings")]
        [SerializeField] private float holdDuration = 1.0f;
        [SerializeField] private bool showHoldProgress = true;
        [SerializeField] private Image progressImage;
        [SerializeField] private Color progressColor = new Color(1f, 1f, 1f, 0.5f);
        
        [Header("Haptic Feedback")]
        [SerializeField] private bool useHapticFeedback = true;
        
        [Header("Events")]
        public UnityEvent OnButtonDown;
        public UnityEvent OnButtonUp;
        public UnityEvent OnButtonHeld;
        public UnityEvent<bool> OnButtonToggled;
        
        [SerializeField] private UnityEvent onPress;
        [SerializeField] private UnityEvent onRelease;
        
        // Private variables
        private Image _buttonImage;
        private Vector3 _initialScale;
        private Color _targetColor;
        private float _holdTimer = 0f;
        private bool _isPressed = false;
        private bool _isToggled = false;
        private bool _isHolding = false;
        private bool _holdComplete = false;
        private bool _interactable = true;
        
        // Properties
        public string ButtonId => actionName;
        public bool IsPressed => _isPressed;
        public bool IsToggled => _isToggled;
        public bool IsInteractable => _interactable;
        public bool IsHolding => _isHolding;
        public UnityEvent OnPress => onPress;
        public UnityEvent OnRelease => onRelease;
        
        private void Awake()
        {
            _buttonImage = GetComponent<Image>();
            _initialScale = transform.localScale;
            _targetColor = normalColor;
            
            // Setup progress indicator if available
            if (progressImage != null)
            {
                progressImage.type = Image.Type.Filled;
                progressImage.fillMethod = Image.FillMethod.Radial360;
                progressImage.fillOrigin = 2; // Bottom
                progressImage.fillAmount = 0f;
                progressImage.color = progressColor;
                progressImage.gameObject.SetActive(false);
            }
        }
        
        private void Update()
        {
            // Update visual state
            UpdateVisuals();
            
            // Handle hold behavior
            if (_isPressed && behavior == ButtonBehavior.Hold && !_holdComplete)
            {
                _holdTimer += Time.deltaTime;
                
                // Update progress indicator
                if (showHoldProgress && progressImage != null)
                {
                    progressImage.fillAmount = Mathf.Clamp01(_holdTimer / holdDuration);
                }
                
                // Check if hold completed
                if (_holdTimer >= holdDuration)
                {
                    _holdComplete = true;
                    OnButtonHeld?.Invoke();
                    
                    // Vibrate if haptic feedback enabled
                    if (useHapticFeedback)
                    {
                        TriggerHapticFeedback(HapticTypes.Success);
                    }
                }
            }
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_interactable) return;
            
            _isPressed = true;
            _holdTimer = 0f;
            _holdComplete = false;
            
            // Handle behavior
            switch (behavior)
            {
                case ButtonBehavior.Momentary:
                    OnButtonDown?.Invoke();
                    break;
                    
                case ButtonBehavior.Toggle:
                    _isToggled = !_isToggled;
                    OnButtonToggled?.Invoke(_isToggled);
                    OnButtonDown?.Invoke();
                    break;
                    
                case ButtonBehavior.Hold:
                    _isHolding = true;
                    if (showHoldProgress && progressImage != null)
                    {
                        progressImage.gameObject.SetActive(true);
                        progressImage.fillAmount = 0f;
                    }
                    OnButtonDown?.Invoke();
                    break;
            }
            
            // Update visual state
            if (useVisualFeedback)
            {
                if (pressedSprite != null)
                    _buttonImage.sprite = pressedSprite;
                
                _targetColor = pressedColor;
            }
            
            // Haptic feedback
            if (useHapticFeedback)
            {
                TriggerHapticFeedback(HapticTypes.Selection);
            }
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_interactable) return;
            
            _isPressed = false;
            
            // Handle behavior
            switch (behavior)
            {
                case ButtonBehavior.Momentary:
                    OnButtonUp?.Invoke();
                    break;
                    
                case ButtonBehavior.Hold:
                    _isHolding = false;
                    if (showHoldProgress && progressImage != null)
                    {
                        progressImage.gameObject.SetActive(false);
                    }
                    OnButtonUp?.Invoke();
                    break;
                    
                case ButtonBehavior.Toggle:
                    // Toggle state is already set in OnPointerDown
                    OnButtonUp?.Invoke();
                    break;
            }
            
            // Reset hold state
            _holdTimer = 0f;
            
            // Update visual state
            if (useVisualFeedback)
            {
                if (normalSprite != null)
                    _buttonImage.sprite = normalSprite;
                
                _targetColor = _isToggled ? pressedColor : normalColor;
            }
        }
        
        private void UpdateVisuals()
        {
            // Update color
            if (useVisualFeedback)
            {
                _buttonImage.color = Color.Lerp(_buttonImage.color, _targetColor, Time.deltaTime * colorTransitionSpeed);
            }
            
            // Update scale animation
            if (useAnimation)
            {
                float targetScale = _isPressed ? pressedScale : 1.0f;
                transform.localScale = Vector3.Lerp(transform.localScale, _initialScale * targetScale, 
                    Time.deltaTime * animationSpeed);
            }
        }
        
        /// <summary>
        /// Sets the interactable state of the button
        /// </summary>
        public void SetInteractable(bool interactable)
        {
            _interactable = interactable;
            
            // Update visual state
            if (useVisualFeedback)
            {
                _targetColor = interactable ? 
                    (_isToggled ? pressedColor : normalColor) : 
                    disabledColor;
            }
            
            // If being disabled while pressed, cancel the press
            if (!interactable && _isPressed)
            {
                _isPressed = false;
                _isHolding = false;
                _holdTimer = 0f;
                
                if (showHoldProgress && progressImage != null)
                {
                    progressImage.gameObject.SetActive(false);
                }
            }
        }
        
        /// <summary>
        /// Triggers a haptic feedback effect
        /// </summary>
        private void TriggerHapticFeedback(HapticTypes type)
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            switch (type)
            {
                case HapticTypes.Selection:
                    // Android vibration API
                    try {
                        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                        AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                        vibrator.Call("vibrate", 20L); // 20ms short vibration
                    } catch (Exception) { }
                    break;
                case HapticTypes.Success:
                    try {
                        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                        AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                        long[] pattern = new long[] { 0, 40, 60, 40 }; // pattern for success vibration
                        vibrator.Call("vibrate", pattern, -1);
                    } catch (Exception) { }
                    break;
            }
            #elif UNITY_IOS && !UNITY_EDITOR
            // iOS haptic feedback would be implemented here
            // Requires native plugin integration
            #endif
        }
        
        /// <summary>
        /// Forcibly sets the button state
        /// </summary>
        public void SetButtonState(bool active)
        {
            if (behavior != ButtonBehavior.Toggle) return;
            
            _isToggled = active;
            _targetColor = active ? pressedColor : normalColor;
            OnButtonToggled?.Invoke(_isToggled);
        }
        
        /// <summary>
        /// Sets the action name at runtime
        /// </summary>
        public void SetActionName(string name)
        {
            actionName = name;
        }
        
        /// <summary>
        /// Simulates a button press
        /// </summary>
        public void SimulateButtonPress()
        {
            if (!_interactable) return;
            
            // Create an empty event data to simulate user input
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            OnPointerDown(eventData);
            
            // For momentary buttons, also simulate release after a short delay
            if (behavior == ButtonBehavior.Momentary)
            {
                StartCoroutine(SimulateButtonRelease(0.1f, eventData));
            }
        }
        
        /// <summary>
        /// Coroutine to simulate button release after a delay
        /// </summary>
        private System.Collections.IEnumerator SimulateButtonRelease(float delay, PointerEventData eventData)
        {
            yield return new WaitForSeconds(delay);
            OnPointerUp(eventData);
        }
        
        // Haptic feedback types
        private enum HapticTypes
        {
            Selection,
            Success,
            Warning,
            Failure
        }
    }
} 