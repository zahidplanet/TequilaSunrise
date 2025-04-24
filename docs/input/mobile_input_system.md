# Mobile Input System

This documentation describes the mobile input system designed for the Tequila Sunrise AR application. The system provides a flexible and customizable touch-based input solution optimized for mobile AR experiences.

## Overview

The mobile input system consists of the following core components:

1. **MobileInputController**: Central manager for all mobile input elements
2. **Joystick**: Virtual joystick for directional input
3. **ActionButton**: Configurable buttons for game actions
4. **MobileInputTester**: Test harness for input system validation
5. **MobileInputSetup**: Editor utility to quickly create input layouts

## Installation

The mobile input system is included in the Tequila Sunrise project. To add mobile controls to your scene:

1. Open Unity Editor
2. Navigate to the menu: **TequilaSunrise → UI → Create Mobile Controls**
3. This will create a complete mobile input setup in your current scene

Alternatively, you can manually add the components to existing UI elements.

## Components

### MobileInputController

The `MobileInputController` is the central hub for all mobile input processing. It manages joysticks, action buttons, and provides a clean API for game systems to consume input.

**Key Features:**
- Manages movement and look joysticks
- Tracks button states (pressed, held, released)
- Provides events for input changes
- Handles device-specific adaptations
- Manages control visibility and opacity
- Provides haptic feedback

**Usage Example:**
```csharp
// Get reference to the controller
MobileInputController inputController = FindObjectOfType<MobileInputController>();

// Subscribe to events
inputController.OnMovementChanged.AddListener(OnMovementInput);
inputController.OnButtonPressed.AddListener(OnButtonPressed);

// Get input in Update
void Update() {
    // Get movement as world-space vector (camera-relative by default)
    Vector3 movementDirection = inputController.GetMovementInputWorld();
    
    // Check if specific button is pressed
    bool isJumping = inputController.IsJumping;
    bool isInteracting = inputController.IsInteracting;
    
    // Or check any button by ID
    bool isFirePressed = inputController.IsButtonPressed("fire");
}
```

### Joystick

The `Joystick` component provides touch-based directional input. It supports multiple joystick behaviors: fixed position, floating (appears at touch location), and dynamic (repositions during use).

**Key Features:**
- Multiple joystick types (Fixed, Floating, Dynamic)
- Configurable dead zone and handle range
- Visual feedback options
- Input axis inversion
- Optional axis snapping (for digital-style input)
- Custom events for joystick states

**Usage Example:**
```csharp
// Create joystick reference
[SerializeField] private Joystick moveJoystick;

// Get input values
void Update() {
    // Get raw directional values
    float horizontal = moveJoystick.Horizontal; // -1 to 1
    float vertical = moveJoystick.Vertical; // -1 to 1
    
    // Or as a Vector2
    Vector2 direction = moveJoystick.Direction;
    
    // Check if joystick is being touched
    bool isActive = moveJoystick.IsPressed;
}
```

### ActionButton

The `ActionButton` component provides touch-based button input with extensive customization options.

**Key Features:**
- Toggle or momentary button modes
- Hold detection with configurable threshold
- Cooldown functionality 
- Visual and audio feedback
- Ripple effect option
- Haptic feedback
- Custom events for button states

**Usage Example:**
```csharp
// Create button reference
[SerializeField] private ActionButton jumpButton;

// Subscribe to events
void Start() {
    jumpButton.OnButtonDown.AddListener(OnJumpPressed);
    jumpButton.OnButtonUp.AddListener(OnJumpReleased);
    jumpButton.OnButtonHold.AddListener(OnJumpHeld);
}

// Button callbacks
void OnJumpPressed() {
    // Start jump
}

void OnJumpHeld() {
    // For long press actions
}

// Check button state directly
void Update() {
    if (jumpButton.IsPressed) {
        // Button is currently pressed
    }
    
    if (jumpButton.IsToggled) {
        // Button is in toggled state (for toggle buttons)
    }
}
```

## Integration with AR

The mobile input system is designed to work seamlessly with AR Foundation. Here's how to integrate it with AR interactions:

1. **AR Raycast**: Use the joystick or buttons to control AR raycasting
   ```csharp
   if (inputController.IsButtonPressed("place")) {
       // Perform AR raycast at screen center
       if (arRaycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon)) {
           // Place object at hit position
       }
   }
   ```

2. **Object Manipulation**: Move and rotate AR objects
   ```csharp
   // Move selected object
   if (selectedObject != null) {
       // Use joystick input to move object on detected planes
       Vector3 movement = inputController.GetMovementInputWorld();
       selectedObject.Translate(movement * Time.deltaTime * moveSpeed);
       
       // Use look joystick to rotate object
       float rotationInput = inputController.LookInput.x;
       selectedObject.Rotate(Vector3.up, rotationInput * Time.deltaTime * rotateSpeed);
   }
   ```

3. **UI Integration**: Show/hide UI based on context
   ```csharp
   // Show controls when object is selected
   inputController.SetControlsVisibility(selectedObject != null);
   ```

## Customization

### Visual Theming

The mobile input system supports visual customization through Unity's standard UI components:

1. **Colors and Sprites**: Modify the images and colors on Joystick and ActionButton components
2. **Positioning**: Adjust the RectTransform properties for layout
3. **Opacity**: Change the control opacity for better AR visibility
4. **Animations**: Modify scale and fade animations for visual feedback

### Input Sensitivity

Adjust input sensitivity through the component inspectors:

1. **Joystick**: Modify deadzone, handle range, and snap settings
2. **ActionButton**: Change hold threshold and cooldown durations
3. **MobileInputController**: Adjust overall sensitivity scaling

## Best Practices

1. **Performance**: Minimize UI overdraw
   - Use simple shapes and minimal transparency
   - Avoid complex backgrounds behind controls
   - Use sprite atlases for button images

2. **Visibility**: Ensure controls are visible over AR content
   - Maintain sufficient contrast
   - Use partial transparency
   - Consider dynamic opacity based on content

3. **Placement**: Optimize for comfortable thumb reach
   - Place movement controls in left corner (for right-handed users)
   - Keep action buttons within comfortable thumb reach
   - Support left/right hand configuration

4. **Feedback**: Provide clear input feedback
   - Visual button state changes
   - Optional sound effects
   - Haptic feedback where appropriate

## Troubleshooting

### Common Issues

1. **Controls not responding**
   - Ensure Canvas has a GraphicRaycaster component
   - Check that UI elements are on the correct sorting layer
   - Verify Canvas render mode is appropriate for AR (typically ScreenSpaceOverlay)

2. **Joystick behaving unexpectedly**
   - Check joystick type (Fixed, Floating, Dynamic)
   - Verify that handle range and dead zone are set correctly
   - Make sure reference connections (background, handle) are set

3. **Button triggering multiple times**
   - Check for duplicate event subscriptions
   - Verify cooldown settings if enabled
   - Inspect scene for duplicate button components

### Debug Tools

The project includes a test scene (`MobileInputTest`) that demonstrates all input components and provides visual feedback for debugging. Use this scene to verify input behavior before integration with AR features.

## API Reference

### MobileInputController

| Property | Type | Description |
|----------|------|-------------|
| MovementInput | Vector2 | Current movement joystick input (normalized) |
| LookInput | Vector2 | Current look joystick input (normalized) |
| IsJumping | bool | State of the jump button |
| IsSprinting | bool | State of the sprint button |
| IsInteracting | bool | State of the interact button |

| Method | Return Type | Description |
|--------|-------------|-------------|
| IsButtonPressed(string buttonId) | bool | Check if a button is currently pressed |
| IsButtonHeld(string buttonId) | bool | Check if a button is being held |
| GetMovementInputWorld([Transform relativeTo]) | Vector3 | Get movement input as a world space direction |
| SetControlsVisibility(bool visible) | void | Show or hide the controls |
| Vibrate([long milliseconds]) | void | Trigger device vibration |
| ResetAllInput() | void | Reset all input states to default |

| Event | Parameters | Description |
|-------|------------|-------------|
| OnMovementChanged | Vector2 | Fired when movement joystick position changes |
| OnLookChanged | Vector2 | Fired when look joystick position changes |
| OnButtonPressed | string, float | Fired when a button is pressed |
| OnButtonReleased | string, float | Fired when a button is released |
| OnButtonHeld | string, float | Fired when a button is held | 