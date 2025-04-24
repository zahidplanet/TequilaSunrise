using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;
using TequilaSunrise.UI.Utilities;

namespace TequilaSunrise.UI
{
    /// <summary>
    /// Virtual joystick for mobile controls that provides direction input through touch
    /// </summary>
    public class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Header("References")]
        [SerializeField] private RectTransform background;
        [SerializeField] private RectTransform handle;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image handleImage;

        [Header("Behavior")]
        [SerializeField] private JoystickType joystickType = JoystickType.Fixed;
        [SerializeField] private float handleRange = 1f;
        [Tooltip("Deadzone in normalized units (0-1)")]
        [SerializeField, Range(0f, 1f)] private float deadZone = 0.1f;
        [SerializeField] private bool snapX = false;
        [SerializeField] private bool snapY = false;
        [SerializeField] private bool invertXAxis = false;
        [SerializeField] private bool invertYAxis = false;

        [Header("Visual Feedback")]
        [SerializeField] private bool fadeWhenReleased = true;
        [SerializeField, Range(0f, 1f)] private float idleAlpha = 0.5f;
        [SerializeField, Range(0f, 1f)] private float activeAlpha = 1f;
        [SerializeField] private Color idleColor = new Color(1f, 1f, 1f, 0.5f);
        [SerializeField] private Color activeColor = Color.white;
        [SerializeField] private bool scaleOnPress = true;
        [SerializeField] private float pressedScale = 1.1f;
        [SerializeField] private float releaseScale = 1f;
        [SerializeField] private float scaleDuration = 0.2f;

        [Header("Dynamic Position")]
        [SerializeField] private Vector2 movementArea = new Vector2(0f, 0f);
        [SerializeField] private float moveThreshold = 1f;
        [SerializeField] private float dynamicPositionSpeed = 10f;

        // Events
        [System.Serializable] public class JoystickEvent : UnityEvent<Vector2> { }
        public JoystickEvent OnJoystickMoved = new JoystickEvent();
        public UnityEvent OnJoystickDown = new UnityEvent();
        public UnityEvent OnJoystickUp = new UnityEvent();

        // Properties
        public Vector2 Direction { get { return new Vector2(
            (invertXAxis ? -1 : 1) * (snapX ? SnapAxis(_input.x) : _input.x),
            (invertYAxis ? -1 : 1) * (snapY ? SnapAxis(_input.y) : _input.y)); 
        }}
        
        public Vector2 RawInput => _input;
        public float Horizontal { get { return (invertXAxis ? -1 : 1) * (snapX ? SnapAxis(_input.x) : _input.x); } }
        public float Vertical { get { return (invertYAxis ? -1 : 1) * (snapY ? SnapAxis(_input.y) : _input.y); } }
        public bool IsPressed { get; private set; }
        
        public enum JoystickType { Fixed, Floating, Dynamic }

        // Private fields
        private Canvas _canvas;
        private RectTransform _canvasRectTransform;
        private RectTransform _rectTransform;
        private Vector2 _input = Vector2.zero;
        private Vector2 _initialPosition;
        private Vector2 _currentTargetPosition;
        private Vector3 _defaultScale;
        private Vector2 _pointerDownPosition;
        private bool _joystickMoved = false;

        protected virtual void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _initialPosition = _rectTransform.anchoredPosition;
            _currentTargetPosition = _initialPosition;
            _defaultScale = transform.localScale;
        }

        protected virtual void Start()
        {
            _canvas = GetComponentInParent<Canvas>();
            if (_canvas != null)
            {
                _canvasRectTransform = _canvas.GetComponent<RectTransform>();
            }

            SetMoveThreshold(moveThreshold);
            
            // Initialize visual state
            UpdateVisualState(false);
        }

        protected virtual void Update()
        {
            // Handle dynamic position update
            if (joystickType == JoystickType.Dynamic && _rectTransform.anchoredPosition != _currentTargetPosition)
            {
                _rectTransform.anchoredPosition = Vector2.Lerp(
                    _rectTransform.anchoredPosition, 
                    _currentTargetPosition, 
                    Time.deltaTime * dynamicPositionSpeed);
            }
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            IsPressed = true;
            _pointerDownPosition = GetLocalPointFrom(eventData);
            _joystickMoved = false;
            
            // Handle different joystick types
            if (joystickType != JoystickType.Fixed)
            {
                // For floating/dynamic joysticks, move to touch position
                Vector2 touchPos = GetLocalPointFrom(eventData);
                background.anchoredPosition = touchPos;
                _currentTargetPosition = touchPos;
            }
            
            // Update visuals
            UpdateVisualState(true);
            
            // Handle input
            OnDrag(eventData);
            
            // Trigger events
            OnJoystickDown?.Invoke();
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            IsPressed = false;
            _input = Vector2.zero;
            handle.anchoredPosition = Vector2.zero;
            
            // Reset position for floating/dynamic joysticks
            if (joystickType != JoystickType.Fixed)
            {
                _currentTargetPosition = _initialPosition;
                if (joystickType == JoystickType.Floating)
                {
                    _rectTransform.anchoredPosition = _initialPosition;
                }
            }
            
            // Update visuals
            UpdateVisualState(false);
            
            // Trigger events
            OnJoystickUp?.Invoke();
            OnJoystickMoved?.Invoke(Vector2.zero);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            Vector2 touchPos = GetLocalPointFrom(eventData);
            
            // For dynamic positioning, check if we should move the joystick
            if (joystickType == JoystickType.Dynamic && _joystickMoved)
            {
                // Check if we've dragged beyond the background
                Vector2 direction = touchPos - background.anchoredPosition;
                if (direction.magnitude > background.sizeDelta.x * handleRange)
                {
                    _currentTargetPosition = background.anchoredPosition + direction.normalized * moveThreshold;
                }
            }
            
            // Calculate input based on joystick type
            Vector2 position;
            if (joystickType == JoystickType.Fixed)
            {
                position = touchPos;
            }
            else 
            {
                position = touchPos - background.anchoredPosition;
            }
            
            // Calculate joystick position
            position = Vector2.ClampMagnitude(position, background.sizeDelta.x * handleRange);
            handle.anchoredPosition = position;
            
            // Calculate input vector
            Vector2 normalizedPosition = position / (background.sizeDelta.x * handleRange);
            _input = (normalizedPosition.magnitude > deadZone) ? normalizedPosition : Vector2.zero;
            
            // If input magnitude > deadzone, mark joystick as moved (for dynamic repositioning)
            if (_input.magnitude > 0)
            {
                _joystickMoved = true;
            }
            
            // If outside deadzone, normalize input
            if (_input.magnitude > deadZone)
            {
                _input = _input.normalized * (((_input.magnitude - deadZone) / (1 - deadZone)));
            }
            
            // Trigger moved event
            OnJoystickMoved?.Invoke(Direction);
        }

        private float SnapAxis(float value)
        {
            if (value > 0)
                return 1;
            if (value < 0)
                return -1;
            return 0;
        }

        private Vector2 GetLocalPointFrom(PointerEventData eventData)
        {
            if (_canvas == null || _canvasRectTransform == null)
                return Vector2.zero;
                
            Vector2 localPoint;
            
            // Convert screen point to local point
            if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    background, eventData.position, null, out localPoint))
                {
                    return localPoint;
                }
            }
            else if (_canvas.renderMode == RenderMode.ScreenSpaceCamera || 
                     _canvas.renderMode == RenderMode.WorldSpace)
            {
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    background, eventData.position, _canvas.worldCamera, out localPoint))
                {
                    return localPoint;
                }
            }
            
            return Vector2.zero;
        }

        public void SetMoveThreshold(float threshold)
        {
            moveThreshold = Mathf.Abs(threshold);
        }

        public void SetHandleRange(float range)
        {
            handleRange = Mathf.Abs(range);
        }

        public void SetDeadZone(float zone)
        {
            deadZone = Mathf.Clamp01(zone);
        }

        private void UpdateVisualState(bool active)
        {
            if (fadeWhenReleased)
            {
                float targetAlpha = active ? activeAlpha : idleAlpha;
                Color targetColor = active ? activeColor : idleColor;
                
                if (backgroundImage != null)
                {
                    TweenUtility.ChangeGraphicColor(this, backgroundImage, targetColor, 0.2f);
                }
                
                if (handleImage != null)
                {
                    TweenUtility.ChangeGraphicColor(this, handleImage, targetColor, 0.2f);
                }
            }
            
            if (scaleOnPress)
            {
                float targetScale = active ? pressedScale : releaseScale;
                TweenUtility.Scale(this, gameObject, _defaultScale * targetScale, scaleDuration);
            }
        }
        
        public void SetActiveVisualState(bool active)
        {
            UpdateVisualState(active);
        }
        
        public void ResetPosition()
        {
            _rectTransform.anchoredPosition = _initialPosition;
            _currentTargetPosition = _initialPosition;
            handle.anchoredPosition = Vector2.zero;
            _input = Vector2.zero;
        }
        
        public void SetHandleColor(Color color)
        {
            if (handleImage != null)
            {
                handleImage.color = color;
            }
        }
        
        public void SetBackgroundColor(Color color)
        {
            if (backgroundImage != null)
            {
                backgroundImage.color = color;
            }
        }
    }
} 