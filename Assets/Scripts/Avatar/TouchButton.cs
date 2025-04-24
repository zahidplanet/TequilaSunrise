using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TequilaSunrise.Avatar
{
    /// <summary>
    /// Enhanced touch button for mobile controls with visual feedback and multiple interaction states
    /// </summary>
    public class TouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [Header("Button Settings")]
        [Tooltip("Allow holding button for continuous activation")]
        [SerializeField] private bool allowButtonHold = false;
        [Tooltip("Allow swiping onto button to activate")]
        [SerializeField] private bool activateOnEnter = false;
        [Tooltip("How long to wait before considering a press a 'hold'")]
        [SerializeField] private float holdStartDelay = 0.2f;
        
        [Header("Visual Feedback")]
        [SerializeField] private Graphic targetGraphic;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color pressedColor = new Color(0.8f, 0.8f, 0.8f);
        [SerializeField] private Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        [SerializeField] private float colorTransitionDuration = 0.1f;
        [SerializeField] private bool useScale = true;
        [SerializeField] private Vector3 pressedScale = new Vector3(0.95f, 0.95f, 0.95f);
        [SerializeField] private float scaleTransitionDuration = 0.05f;
        
        [Header("Haptic Feedback")]
        [SerializeField] private bool useHaptics = true;
        [SerializeField] private HapticTypes hapticType = HapticTypes.LightImpact;
        
        [Header("Sound Feedback")]
        [SerializeField] private bool useSound = false;
        [SerializeField] private AudioClip pressSound;
        [SerializeField] private AudioClip releaseSound;
        [Tooltip("Source to play sounds through")]
        [SerializeField] private AudioSource audioSource;
        
        [Header("Events")]
        public UnityEvent OnPress = new UnityEvent();
        public UnityEvent OnRelease = new UnityEvent();
        public UnityEvent OnHoldStart = new UnityEvent();
        public UnityEvent<float> WhileHolding = new UnityEvent<float>();
        
        public enum HapticTypes { Selection, LightImpact, MediumImpact, HeavyImpact, Success, Warning, Failure }
        
        // State tracking
        private bool isPressed = false;
        private bool isHolding = false;
        private float holdTimer = 0f;
        private Vector3 originalScale;
        private bool isInteractable = true;
        private Coroutine colorTransition;
        private Coroutine scaleTransition;
        
        // Properties
        public bool IsPressed => isPressed;
        public bool IsHolding => isHolding;
        public bool IsInteractable
        {
            get => isInteractable;
            set
            {
                if (isInteractable != value)
                {
                    isInteractable = value;
                    UpdateVisualState(isInteractable ? (isPressed ? pressedColor : normalColor) : disabledColor);
                    
                    // If becoming non-interactable while pressed, reset state
                    if (!isInteractable && isPressed)
                    {
                        isPressed = false;
                        isHolding = false;
                        holdTimer = 0f;
                    }
                }
            }
        }
        
        private void Awake()
        {
            // Cache the original scale
            originalScale = transform.localScale;
            
            // Find target graphic if not assigned
            if (targetGraphic == null)
            {
                targetGraphic = GetComponent<Graphic>();
                if (targetGraphic == null)
                {
                    targetGraphic = GetComponentInChildren<Graphic>();
                }
            }
            
            // Set initial color
            if (targetGraphic != null)
            {
                targetGraphic.color = isInteractable ? normalColor : disabledColor;
            }
            
            // Find audio source if using sound but none assigned
            if (useSound && audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null && (pressSound != null || releaseSound != null))
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                    audioSource.playOnAwake = false;
                }
            }
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            if (!isInteractable) return;
            
            isPressed = true;
            holdTimer = 0f;
            isHolding = false;
            
            // Visual feedback
            UpdateVisualState(pressedColor);
            if (useScale)
            {
                UpdateScaleState(pressedScale);
            }
            
            // Sound feedback
            if (useSound && pressSound != null && audioSource != null)
            {
                audioSource.clip = pressSound;
                audioSource.Play();
            }
            
            // Haptic feedback
            if (useHaptics)
            {
                TriggerHapticFeedback(hapticType);
            }
            
            // Fire press event
            OnPress.Invoke();
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isInteractable || !isPressed) return;
            
            isPressed = false;
            isHolding = false;
            holdTimer = 0f;
            
            // Visual feedback
            UpdateVisualState(normalColor);
            if (useScale)
            {
                UpdateScaleState(originalScale);
            }
            
            // Sound feedback
            if (useSound && releaseSound != null && audioSource != null)
            {
                audioSource.clip = releaseSound;
                audioSource.Play();
            }
            
            // Fire release event
            OnRelease.Invoke();
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            // Only reset if we're not allowing activation on enter (swiping)
            if (!activateOnEnter && isPressed)
            {
                OnPointerUp(eventData);
            }
        }
        
        private void Update()
        {
            if (isPressed && allowButtonHold)
            {
                holdTimer += Time.deltaTime;
                
                // Transition to holding state
                if (!isHolding && holdTimer >= holdStartDelay)
                {
                    isHolding = true;
                    OnHoldStart.Invoke();
                }
                
                // While holding
                if (isHolding)
                {
                    WhileHolding.Invoke(holdTimer - holdStartDelay);
                }
            }
        }
        
        private void UpdateVisualState(Color targetColor)
        {
            if (targetGraphic == null) return;
            
            // Stop any running color transition
            if (colorTransition != null)
            {
                StopCoroutine(colorTransition);
            }
            
            // Start new transition if duration > 0
            if (colorTransitionDuration > 0)
            {
                colorTransition = StartCoroutine(TransitionColor(targetColor));
            }
            else
            {
                targetGraphic.color = targetColor;
            }
        }
        
        private System.Collections.IEnumerator TransitionColor(Color targetColor)
        {
            Color startColor = targetGraphic.color;
            float elapsedTime = 0f;
            
            while (elapsedTime < colorTransitionDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / colorTransitionDuration);
                targetGraphic.color = Color.Lerp(startColor, targetColor, t);
                yield return null;
            }
            
            targetGraphic.color = targetColor;
            colorTransition = null;
        }
        
        private void UpdateScaleState(Vector3 targetScale)
        {
            // Stop any running scale transition
            if (scaleTransition != null)
            {
                StopCoroutine(scaleTransition);
            }
            
            // Start new transition if duration > 0
            if (scaleTransitionDuration > 0)
            {
                scaleTransition = StartCoroutine(TransitionScale(targetScale));
            }
            else
            {
                transform.localScale = targetScale;
            }
        }
        
        private System.Collections.IEnumerator TransitionScale(Vector3 targetScale)
        {
            Vector3 startScale = transform.localScale;
            float elapsedTime = 0f;
            
            while (elapsedTime < scaleTransitionDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / scaleTransitionDuration);
                transform.localScale = Vector3.Lerp(startScale, targetScale, t);
                yield return null;
            }
            
            transform.localScale = targetScale;
            scaleTransition = null;
        }
        
        private void TriggerHapticFeedback(HapticTypes type)
        {
            #if UNITY_ANDROID || UNITY_IOS
            // Using Handheld.Vibrate() for basic vibration
            // For a more sophisticated haptic system, you might want to use platform-specific APIs
            switch (type)
            {
                case HapticTypes.LightImpact:
                case HapticTypes.Selection:
                    Handheld.Vibrate();
                    break;
                case HapticTypes.MediumImpact:
                case HapticTypes.HeavyImpact:
                case HapticTypes.Success:
                case HapticTypes.Warning:
                case HapticTypes.Failure:
                    Handheld.Vibrate();
                    break;
            }
            #endif
        }
        
        // Public method to simulate a button press
        public void SimulatePress()
        {
            if (!isInteractable) return;
            
            // Create a dummy pointer event data
            PointerEventData dummyEventData = new PointerEventData(EventSystem.current);
            OnPointerDown(dummyEventData);
        }
        
        // Public method to simulate a button release
        public void SimulateRelease()
        {
            if (!isInteractable || !isPressed) return;
            
            // Create a dummy pointer event data
            PointerEventData dummyEventData = new PointerEventData(EventSystem.current);
            OnPointerUp(dummyEventData);
        }
    }
} 