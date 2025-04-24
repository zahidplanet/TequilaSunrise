# Prefab Setup Guide for Character AR Scene

This document provides instructions for setting up the prefabs needed for the Character AR Test Scene.

## Required Prefabs

### 1. AR Components (Assets/Prefabs/AR)

#### AR Session
- Create an empty GameObject named `AR Session`
- Add `ARSession` component
- Add `ARInputManager` component
- Save as prefab in `Assets/Prefabs/AR/ARSession.prefab`

#### AR Session Origin
- Create an empty GameObject named `AR Session Origin`
- Add `ARSessionOrigin` component
- Add an `AR Camera` child with:
  - `Camera` component
  - `ARPoseDriver` component
  - Appropriate camera settings for AR
- Add `ARPlaneManager` component to AR Session Origin
- Add `ARRaycastManager` component to AR Session Origin
- Save as prefab in `Assets/Prefabs/AR/ARSessionOrigin.prefab`

#### Plane Visualization
- Create a prefab for AR plane visualization
- Use a simple mesh with transparent material
- Save as prefab in `Assets/Prefabs/AR/ARPlaneVisualization.prefab`
- Assign this to the `ARPlaneManager` prefab field

### 2. Character Components (Assets/Prefabs/Character)

#### Avatar Controller Prefab
- Create a GameObject named `AvatarController`
- Add a `CharacterController` component
- Add the `AvatarController` script
- Setup required colliders
- Setup the character model and materials
- Configure animations and animator controller
- Save as prefab in `Assets/Prefabs/Character/AvatarController.prefab`

### 3. Motorcycle Components (Assets/Prefabs/Motorcycle)

#### Motorcycle Controller Prefab
- Create a GameObject named `MotorcycleController`
- Add a `Rigidbody` component and configure it
- Add the `MotorcycleController` script
- Setup required colliders
- Setup the motorcycle model and materials
- Add effects like particle systems for exhaust
- Setup audio sources for engine sounds
- Save as prefab in `Assets/Prefabs/Motorcycle/MotorcycleController.prefab`

### 4. UI Components (Assets/Prefabs/UI)

#### Mobile Input UI
- Create a Canvas GameObject with:
  - `Canvas` component set to Screen Space - Overlay
  - `CanvasScaler` component properly configured for multiple devices
  - `GraphicRaycaster` component
- Add Joystick controls and action buttons as children
- Add the `MobileInputController` script to the Canvas
- Configure all input components
- Save as prefab in `Assets/Prefabs/UI/MobileInputUI.prefab`

#### AR Instructions UI
- Create a Canvas GameObject for instructions
- Add text elements explaining how to use the AR features
- Make it toggleable
- Save as prefab in `Assets/Prefabs/UI/ARInstructionsUI.prefab`

## Scene Setup

After creating all prefabs, set up the CharacterARTest scene as follows:

1. Add AR Session prefab
2. Add AR Session Origin prefab
3. Add Mobile Input UI prefab
4. Add AR Instructions UI prefab
5. Create an empty GameObject and add the `CharacterARSceneManager` script
6. Configure the `CharacterARSceneManager` with references to:
   - AR components
   - Character prefab
   - Motorcycle spawner
   - Input controller
   - UI elements

## Testing

Test the scene in both the Unity Editor (using XR Device Simulator if available) and on actual AR-capable devices.

Notes:
- Ensure all prefabs are properly configured
- Check that input systems work correctly
- Verify character movement and motorcycle interactions
- Make sure plane detection works properly 