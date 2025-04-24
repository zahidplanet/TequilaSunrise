# TS-9: Character AR Test Scene Implementation Plan

## Overview
This implementation plan outlines the steps to create a complete AR test scene with the main pixel character and motorcycle controls, addressing the mismatch between code implementation and actual scene setup.

## Implementation Steps

### 1. Create Prefab Folder Structure
- [x] Create `Assets/Prefabs` folder
- [ ] Create `Assets/Prefabs/Character` subfolder
- [ ] Create `Assets/Prefabs/Motorcycle` subfolder
- [ ] Create `Assets/Prefabs/UI` subfolder
- [ ] Create `Assets/Prefabs/AR` subfolder

### 2. Set Up Character
- [ ] Import the TS_PixelAvatarMain model
- [ ] Set up character materials and textures
- [ ] Configure character animations
- [ ] Create AvatarController prefab with proper components:
  - [ ] CharacterController component
  - [ ] AvatarController script
  - [ ] Animator with animation controller
  - [ ] Colliders
  - [ ] Default animations

### 3. Create Motorcycle Prefab
- [ ] Import motorcycle model if not already present
- [ ] Set up motorcycle materials and textures
- [ ] Configure motorcycle components:
  - [ ] Rigidbody
  - [ ] MotorcycleController script
  - [ ] Colliders
  - [ ] Visual effects (optional)

### 4. Set Up AR Foundation Elements
- [ ] Create AR Session prefab
- [ ] Set up AR Session Origin
- [ ] Configure AR Camera
- [ ] Set up AR Plane Manager for plane detection
- [ ] Add AR Raycast Manager
- [ ] Create plane visualization prefab

### 5. Implement Mobile Controls
- [ ] Create mobile UI canvas
- [ ] Add joystick control for movement
- [ ] Add action buttons (jump, interact, etc.)
- [ ] Configure MobileInputController
- [ ] Connect input to character controller

### 6. Create AR Test Scene
- [ ] Create new scene "CharacterARTest"
- [ ] Add AR Session components
- [ ] Add plane detection visualization
- [ ] Add environment lighting
- [ ] Add character spawn system
- [ ] Add motorcycle spawn system
- [ ] Configure character-motorcycle interaction

### 7. Testing
- [ ] Test plane detection
- [ ] Test character movement
- [ ] Test character-motorcycle interaction
- [ ] Test on both iOS and Android devices
- [ ] Fix any bugs or issues

## Dependencies
- Mobile input system (TS-6) - Complete
- AR Foundation setup (TS-2) - Complete
- Compiler errors fixed (TS-10) - Complete

## Acceptance Criteria
- [ ] A functioning AR test scene with plane detection
- [ ] Character prefab with animations and controls
- [ ] Motorcycle prefab that the character can interact with
- [ ] Mobile controls that work properly
- [ ] No compiler errors 