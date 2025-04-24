using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TequilaSunrise.UI
{
    public class MobileControls : MonoBehaviour
    {
        [Header("Joystick Settings")]
        [SerializeField] private RectTransform joystickBackground;
        [SerializeField] private RectTransform joystickHandle;
        [SerializeField] private float joystickRange = 50f;
        
        [Header("Action Buttons")]
        [SerializeField] private Button jumpButton;
        [SerializeField] private Button interactButton;
        
        [Header("Control Settings")]
        [SerializeField] private bool hideWhenNotInUse = true;
        [SerializeField] private float fadeSpeed = 5f;

        private Vector2 joystickInput;
        private bool isDragging;
        private CanvasGroup canvasGroup;
        private Vector2 startPos;

        public Vector2 Input => joystickInput;
        public bool IsJumpPressed { get; private set; }
        public bool IsInteractPressed { get; private set; }

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (!canvasGroup) canvasGroup = gameObject.AddComponent<CanvasGroup>();

            if (jumpButton)
            {
                jumpButton.onClick.AddListener(() => IsJumpPressed = true);
            }

            if (interactButton)
            {
                interactButton.onClick.AddListener(() => IsInteractPressed = true);
            }

            if (joystickBackground)
            {
                startPos = joystickBackground.position;
            }
        }

        private void Update()
        {
            UpdateJoystickVisibility();
            ResetButtonStates();
        }

        public void OnJoystickDrag(BaseEventData eventData)
        {
            if (!joystickBackground || !joystickHandle) return;

            PointerEventData pointerData = (PointerEventData)eventData;
            Vector2 position = pointerData.position;

            // Calculate joystick position
            Vector2 direction = (position - startPos).normalized;
            float distance = Vector2.Distance(position, startPos);
            float clampedDistance = Mathf.Min(distance, joystickRange);
            
            // Update handle position
            joystickHandle.position = startPos + direction * clampedDistance;
            
            // Calculate input values (-1 to 1)
            joystickInput = new Vector2(
                direction.x * (clampedDistance / joystickRange),
                direction.y * (clampedDistance / joystickRange)
            );

            isDragging = true;
        }

        public void OnJoystickEnd()
        {
            if (!joystickHandle) return;

            // Reset handle position and input
            joystickHandle.position = startPos;
            joystickInput = Vector2.zero;
            isDragging = false;
        }

        private void UpdateJoystickVisibility()
        {
            if (!hideWhenNotInUse || !canvasGroup) return;

            float targetAlpha = isDragging ? 1f : 0.5f;
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
        }

        private void ResetButtonStates()
        {
            // Reset one-shot button states
            IsJumpPressed = false;
            IsInteractPressed = false;
        }

        private void OnDisable()
        {
            // Reset all states when disabled
            OnJoystickEnd();
            IsJumpPressed = false;
            IsInteractPressed = false;
        }
    }
}