using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TequilaSunrise.UI;

namespace TequilaSunrise.Tests
{
    /// <summary>
    /// Unit tests for the mobile input system
    /// </summary>
    public class MobileInputTests
    {
        private GameObject _canvas;
        private EventSystem _eventSystem;
        private GameObject _joystickObj;
        private GameObject _buttonObj;
        private Joystick _joystick;
        private ActionButton _actionButton;
        private RectTransform _background;
        private RectTransform _handle;
        private Image _buttonImage;
        
        [SetUp]
        public void Setup()
        {
            // Create test canvas
            _canvas = new GameObject("TestCanvas");
            Canvas canvas = _canvas.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.AddComponent<CanvasScaler>();
            _canvas.AddComponent<GraphicRaycaster>();
            
            // Create event system
            _eventSystem = new GameObject("EventSystem").AddComponent<EventSystem>();
            _eventSystem.gameObject.AddComponent<StandaloneInputModule>();
            
            // Setup joystick
            SetupJoystick();
            
            // Setup action button
            SetupActionButton();
        }
        
        [TearDown]
        public void Teardown()
        {
            Object.Destroy(_canvas);
            Object.Destroy(_eventSystem.gameObject);
        }
        
        private void SetupJoystick()
        {
            _joystickObj = new GameObject("TestJoystick");
            _joystickObj.transform.SetParent(_canvas.transform, false);
            _joystick = _joystickObj.AddComponent<Joystick>();
            
            // Add background
            GameObject bgObj = new GameObject("Background");
            bgObj.transform.SetParent(_joystickObj.transform, false);
            _background = bgObj.AddComponent<RectTransform>();
            Image bgImage = bgObj.AddComponent<Image>();
            
            // Add handle
            GameObject handleObj = new GameObject("Handle");
            handleObj.transform.SetParent(bgObj.transform, false);
            _handle = handleObj.AddComponent<RectTransform>();
            Image handleImage = handleObj.AddComponent<Image>();
            
            // Setup references
            _joystick.background = _background;
            _joystick.handle = _handle;
            _joystick.backgroundImage = bgImage;
            _joystick.handleImage = handleImage;
            
            // Set sizes
            _background.sizeDelta = new Vector2(100, 100);
            _handle.sizeDelta = new Vector2(40, 40);
        }
        
        private void SetupActionButton()
        {
            _buttonObj = new GameObject("TestButton");
            _buttonObj.transform.SetParent(_canvas.transform, false);
            _actionButton = _buttonObj.AddComponent<ActionButton>();
            
            _buttonImage = _buttonObj.AddComponent<Image>();
            RectTransform buttonRect = _buttonObj.GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(100, 100);
            
            // Set references
            _actionButton.buttonImage = _buttonImage;
            _actionButton.SetButtonId("testButton");
        }
        
        #region Joystick Tests
        
        [Test]
        public void Joystick_InitialState_ReturnsZeroInput()
        {
            // Assert
            Assert.AreEqual(Vector2.zero, _joystick.Direction);
            Assert.AreEqual(0f, _joystick.Horizontal);
            Assert.AreEqual(0f, _joystick.Vertical);
            Assert.IsFalse(_joystick.IsPressed);
        }
        
        [UnityTest]
        public IEnumerator Joystick_PointerDown_UpdatesIsPressed()
        {
            // Arrange
            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = new Vector2(Screen.width / 2, Screen.height / 2);
            
            // Act
            _joystick.OnPointerDown(eventData);
            
            // Allow for any delayed processing
            yield return null;
            
            // Assert
            Assert.IsTrue(_joystick.IsPressed);
        }
        
        [UnityTest]
        public IEnumerator Joystick_PointerUp_ResetsPosition()
        {
            // Arrange
            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = new Vector2(Screen.width / 2, Screen.height / 2);
            
            // Act - First press down, then release
            _joystick.OnPointerDown(eventData);
            yield return null;
            
            // Move the joystick
            eventData.position = new Vector2(Screen.width / 2 + 20, Screen.height / 2 + 20);
            _joystick.OnDrag(eventData);
            yield return null;
            
            // Record handle position
            Vector2 handlePosBeforeRelease = _handle.anchoredPosition;
            
            // Release the joystick
            _joystick.OnPointerUp(eventData);
            yield return null;
            
            // Assert
            Assert.IsFalse(_joystick.IsPressed);
            Assert.AreEqual(Vector2.zero, _handle.anchoredPosition);
            Assert.AreNotEqual(handlePosBeforeRelease, _handle.anchoredPosition);
            Assert.AreEqual(Vector2.zero, _joystick.Direction);
        }
        
        [UnityTest]
        public IEnumerator Joystick_Drag_UpdatesDirection()
        {
            // Arrange
            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = new Vector2(Screen.width / 2, Screen.height / 2);
            
            // Act - Press down
            _joystick.OnPointerDown(eventData);
            yield return null;
            
            // Move right
            eventData.position = new Vector2(Screen.width / 2 + 30, Screen.height / 2);
            _joystick.OnDrag(eventData);
            yield return null;
            
            // Assert horizontal movement
            Assert.Greater(_joystick.Horizontal, 0);
            Assert.AreEqual(0, _joystick.Vertical, 0.1f);
            
            // Move up
            eventData.position = new Vector2(Screen.width / 2, Screen.height / 2 + 30);
            _joystick.OnDrag(eventData);
            yield return null;
            
            // Assert vertical movement
            Assert.AreEqual(0, _joystick.Horizontal, 0.1f);
            Assert.Greater(_joystick.Vertical, 0);
        }
        
        [UnityTest]
        public IEnumerator Joystick_ExtremeMovement_ClampsMagnitude()
        {
            // Arrange
            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = new Vector2(Screen.width / 2, Screen.height / 2);
            
            // Act - Press down
            _joystick.OnPointerDown(eventData);
            yield return null;
            
            // Move far away (extreme movement)
            eventData.position = new Vector2(Screen.width, Screen.height);
            _joystick.OnDrag(eventData);
            yield return null;
            
            // Assert the magnitude is clamped to 1
            Assert.LessOrEqual(_joystick.Direction.magnitude, 1.01f); // Allow small floating point error
        }
        
        #endregion
        
        #region ActionButton Tests
        
        [Test]
        public void ActionButton_InitialState_IsNotPressed()
        {
            // Assert
            Assert.IsFalse(_actionButton.IsPressed);
            Assert.IsFalse(_actionButton.IsToggled);
            Assert.IsFalse(_actionButton.IsHolding);
        }
        
        [UnityTest]
        public IEnumerator ActionButton_PointerDown_UpdatesIsPressed()
        {
            // Arrange
            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = new Vector2(Screen.width / 2, Screen.height / 2);
            bool buttonPressed = false;
            
            // Subscribe to event
            _actionButton.OnButtonDown.AddListener(() => buttonPressed = true);
            
            // Act
            _actionButton.OnPointerDown(eventData);
            
            // Allow for any delayed processing
            yield return null;
            
            // Assert
            Assert.IsTrue(_actionButton.IsPressed);
            Assert.IsTrue(buttonPressed);
        }
        
        [UnityTest]
        public IEnumerator ActionButton_PointerUp_UpdatesIsPressed()
        {
            // Arrange
            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = new Vector2(Screen.width / 2, Screen.height / 2);
            bool buttonReleased = false;
            
            // Subscribe to events
            _actionButton.OnButtonUp.AddListener(() => buttonReleased = true);
            
            // Act - First press down, then release
            _actionButton.OnPointerDown(eventData);
            yield return null;
            
            _actionButton.OnPointerUp(eventData);
            yield return null;
            
            // Assert
            Assert.IsFalse(_actionButton.IsPressed);
            Assert.IsTrue(buttonReleased);
        }
        
        [UnityTest]
        public IEnumerator ActionButton_Toggle_UpdatesToggleState()
        {
            // Arrange - Make button a toggle
            _actionButton.isToggle = true;
            
            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = new Vector2(Screen.width / 2, Screen.height / 2);
            bool toggleStateChanged = false;
            
            // Subscribe to event
            _actionButton.OnButtonToggled.AddListener(() => toggleStateChanged = true);
            
            // Act - Press to toggle on
            _actionButton.OnPointerDown(eventData);
            yield return null;
            
            _actionButton.OnPointerUp(eventData);
            yield return null;
            
            // Assert toggle is on
            Assert.IsTrue(_actionButton.IsToggled);
            Assert.IsTrue(toggleStateChanged);
            
            // Reset event tracking
            toggleStateChanged = false;
            
            // Act - Press to toggle off
            _actionButton.OnPointerDown(eventData);
            yield return null;
            
            _actionButton.OnPointerUp(eventData);
            yield return null;
            
            // Assert toggle is off
            Assert.IsFalse(_actionButton.IsToggled);
            Assert.IsTrue(toggleStateChanged);
        }
        
        [UnityTest]
        public IEnumerator ActionButton_Hold_TriggersHoldEvent()
        {
            // Arrange - Make button holdable with short threshold for testing
            _actionButton.holdable = true;
            _actionButton.holdThreshold = 0.1f;
            
            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = new Vector2(Screen.width / 2, Screen.height / 2);
            bool holdTriggered = false;
            
            // Subscribe to hold event
            _actionButton.OnButtonHold.AddListener(() => holdTriggered = true);
            
            // Act - Press down and hold
            _actionButton.OnPointerDown(eventData);
            
            // Wait longer than hold threshold
            yield return new WaitForSeconds(0.2f);
            
            // Assert
            Assert.IsTrue(holdTriggered);
            Assert.IsTrue(_actionButton.IsHolding);
            
            // Release
            _actionButton.OnPointerUp(eventData);
            yield return null;
            
            // Assert no longer holding
            Assert.IsFalse(_actionButton.IsHolding);
        }
        
        [UnityTest]
        public IEnumerator ActionButton_Cooldown_PreventsReactivation()
        {
            // Arrange - Enable cooldown
            _actionButton.cooldownEnabled = true;
            _actionButton.cooldownDuration = 0.2f;
            
            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = new Vector2(Screen.width / 2, Screen.height / 2);
            int pressCount = 0;
            
            // Subscribe to event
            _actionButton.OnButtonDown.AddListener(() => pressCount++);
            
            // Act - Press and release
            _actionButton.OnPointerDown(eventData);
            yield return null;
            
            _actionButton.OnPointerUp(eventData);
            yield return null;
            
            // Try to press again immediately during cooldown
            _actionButton.OnPointerDown(eventData);
            yield return null;
            
            // Assert button didn't register second press during cooldown
            Assert.AreEqual(1, pressCount);
            Assert.IsTrue(_actionButton.IsInCooldown);
            
            // Wait for cooldown to finish
            yield return new WaitForSeconds(0.3f);
            
            // Try to press again after cooldown
            _actionButton.OnPointerDown(eventData);
            yield return null;
            
            // Assert the button now registers press
            Assert.AreEqual(2, pressCount);
        }
        
        #endregion
        
        #region Integration Tests
        
        [UnityTest]
        public IEnumerator MobileInputController_ConnectsWithComponents()
        {
            // Arrange
            GameObject controllerObj = new GameObject("InputController");
            controllerObj.transform.SetParent(_canvas.transform, false);
            MobileInputController controller = controllerObj.AddComponent<MobileInputController>();
            
            // Create array for action buttons
            ActionButton[] actionButtons = new ActionButton[1] { _actionButton };
            
            // Set references via reflection (since we can't set non-serialized fields directly in tests)
            var movementJoystickField = typeof(MobileInputController).GetField("movementJoystick", 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var actionButtonsField = typeof(MobileInputController).GetField("actionButtons", 
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                
            movementJoystickField.SetValue(controller, _joystick);
            actionButtonsField.SetValue(controller, actionButtons);
            
            // Give time for controller to initialize
            yield return null;
            
            // Act - Move joystick
            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = new Vector2(Screen.width / 2, Screen.height / 2);
            
            _joystick.OnPointerDown(eventData);
            yield return null;
            
            eventData.position = new Vector2(Screen.width / 2 + 30, Screen.height / 2);
            _joystick.OnDrag(eventData);
            yield return null;
            
            // Assert controller receives joystick input
            Assert.Greater(controller.MovementInput.x, 0);
            
            // Cleanup
            Object.Destroy(controllerObj);
        }
        
        #endregion
    }
} 