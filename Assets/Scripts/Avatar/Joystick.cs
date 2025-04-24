using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace TequilaSunrise.Avatar
{
    /// <summary>
    /// Virtual joystick for mobile controls that provides position data for character movement or camera control
    /// </summary>
    public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Header("Components")]
        [SerializeField] private RectTransform backgroundRect;
        [SerializeField] private RectTransform handleRect;
        
        [Header("Settings")]
        [SerializeField] private float joystickRange = 50f;
        [SerializeField] private bool fixedPosition = true;
        [SerializeField] private bool snapToCenter = true;
        [SerializeField] private bool hideOnRelease = false;
        [Tooltip("Time to reset joystick to center after release")]
        [SerializeField] private float resetSpeed = 5.0f;
        
        [Header("Visual Feedback")]
        [SerializeField] private bool showVisualFeedback = true;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color activeColor = new Color(0.8f, 0.8f, 1.0f);
        
        [Header("Events")]
        public UnityEvent OnJoystickDown;
        public UnityEvent OnJoystickUp;
        public UnityEvent<Vector2> OnJoystickMove;
        
        // Private variables
        private Vector2 _inputVector = Vector2.zero;
        private Vector2 _pointerDownPosition;
        private bool _isDragging = false;
        private Canvas _canvas;
        private Camera _uiCamera;
        private UnityEngine.UI.Image _backgroundImage;
        private UnityEngine.UI.Image _handleImage;
        
        // Public accessors
        public float Horizontal => _inputVector.x;
        public float Vertical => _inputVector.y;
        public Vector2 Direction => _inputVector;
        public float Magnitude => _inputVector.magnitude;
        public bool IsDragging => _isDragging;
        
        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
            _backgroundImage = backgroundRect?.GetComponent<UnityEngine.UI.Image>();
            _handleImage = handleRect?.GetComponent<UnityEngine.UI.Image>();
            
            // Get UI camera
            if (_canvas.renderMode == RenderMode.ScreenSpaceCamera)
                _uiCamera = _canvas.worldCamera;
            
            // Validate required components
            if (backgroundRect == null)
                Debug.LogError("Background RectTransform is not assigned to Joystick");
            
            if (handleRect == null)
                Debug.LogError("Handle RectTransform is not assigned to Joystick");
        }
        
        private void Start()
        {
            _pointerDownPosition = backgroundRect.position;
            UpdateVisuals(false);
            
            // Hide joystick initially if set to hide when not in use
            if (hideOnRelease) 
            {
                SetJoystickVisibility(false);
            }
        }
        
        private void Update()
        {
            // Reset joystick position if not being used and snap to center is enabled
            if (!_isDragging && snapToCenter && _inputVector.magnitude > 0.01f)
            {
                _inputVector = Vector2.Lerp(_inputVector, Vector2.zero, Time.deltaTime * resetSpeed);
                UpdateHandlePosition();
            }
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _isDragging = true;
            
            // If not fixed position, move joystick to pointer position
            if (!fixedPosition)
            {
                backgroundRect.position = eventData.position;
                _pointerDownPosition = eventData.position;
            }
            
            // Make visible if set to hide when not in use
            if (hideOnRelease) 
            {
                SetJoystickVisibility(true);
            }
            
            OnJoystickDown?.Invoke();
            UpdateVisuals(true);
            
            // Process the initial touch
            OnDrag(eventData);
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (_canvas == null || backgroundRect == null || handleRect == null)
                return;
            
            // Calculate local position based on canvas scaling
            Vector2 position = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                backgroundRect,
                eventData.position,
                _uiCamera,
                out position);
            
            // Calculate input vector (normalized direction)
            _inputVector = position / joystickRange;
            
            // Clamp magnitude to 1
            if (_inputVector.magnitude > 1)
                _inputVector = _inputVector.normalized;
            
            // Update handle position
            UpdateHandlePosition();
            
            // Fire event
            OnJoystickMove?.Invoke(_inputVector);
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            _isDragging = false;
            
            // Reset handle position instantly or start smooth reset in Update
            if (!snapToCenter)
            {
                _inputVector = Vector2.zero;
                UpdateHandlePosition();
            }
            
            // Hide joystick if set to hide when not in use
            if (hideOnRelease) 
            {
                SetJoystickVisibility(false);
            }
            
            OnJoystickUp?.Invoke();
            UpdateVisuals(false);
        }
        
        private void UpdateHandlePosition()
        {
            if (handleRect != null)
            {
                handleRect.anchoredPosition = _inputVector * joystickRange;
            }
        }
        
        private void UpdateVisuals(bool isActive)
        {
            if (!showVisualFeedback)
                return;
                
            if (_backgroundImage != null)
                _backgroundImage.color = isActive ? activeColor : normalColor;
                
            if (_handleImage != null)
                _handleImage.color = isActive ? activeColor : normalColor;
        }
        
        private void SetJoystickVisibility(bool visible)
        {
            if (_backgroundImage != null)
                _backgroundImage.enabled = visible;
                
            if (_handleImage != null)
                _handleImage.enabled = visible;
        }
        
        /// <summary>
        /// Resets the joystick to its default state
        /// </summary>
        public void ResetJoystick()
        {
            _inputVector = Vector2.zero;
            _isDragging = false;
            UpdateHandlePosition();
            UpdateVisuals(false);
            
            // Return to original position if not fixed
            if (!fixedPosition)
            {
                backgroundRect.position = _pointerDownPosition;
            }
            
            // Hide if necessary
            if (hideOnRelease)
            {
                SetJoystickVisibility(false);
            }
        }
        
        /// <summary>
        /// Manually sets the joystick's input vector - useful for custom input methods
        /// </summary>
        public void SetInputVector(Vector2 inputVector)
        {
            _inputVector = inputVector.magnitude > 1 ? inputVector.normalized : inputVector;
            UpdateHandlePosition();
            UpdateVisuals(_inputVector.magnitude > 0.01f);
            OnJoystickMove?.Invoke(_inputVector);
        }
    }
} 