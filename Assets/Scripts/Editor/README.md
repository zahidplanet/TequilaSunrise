# Editor Tools for Tequila Sunrise

This directory contains Unity Editor tools to help with the development of Tequila Sunrise. These tools automate common tasks and make it easier to set up the AR environment.

## Available Tools

### Create Placeholder Models

This tool creates simple placeholder models for the avatar and motorcycle using Unity primitive shapes. These models can be used for development until the final art assets are ready.

**To use:**
1. Open the tool via `Tools > Create Placeholder Models`
2. Select the model type (Avatar or Motorcycle)
3. Customize the color if needed
4. Click "Create Placeholder Model"

The tool will generate prefabs in the `Assets/Models` folder.

### Create Avatar Animations

Creates a basic animation controller for the avatar with states for idle, walk, run, and jump animations.

**To use:**
1. Open the tool via `Tools > Create Avatar Animations`
2. Set a name for the controller (default: "AvatarAnimationController")
3. Set a save location (default: "Assets/Animations")
4. Click "Create Animation Controller"

The controller will include:
- States: Idle, Walk, Run, Jump
- Parameters: Speed, Jump, Grounded, FreeFall
- Transitions between states based on parameter values

### Setup Avatar Prefab

This tool creates a fully configured avatar prefab with all necessary components according to the specifications in PrefabSetup.md.

**To use:**
1. First create a model using the Create Placeholder Models tool or import your own
2. Open the tool via `Tools > Setup Avatar Prefab`
3. Select the avatar model prefab
4. Adjust character controller settings if needed
5. Set the avatar scale
6. Click "Create Avatar Prefab"

The tool will create a prefab in `Assets/Prefabs` with:
- Character Controller component
- Capsule Collider
- Animator component
- AvatarController script

### Setup Motorcycle Prefab

Creates a fully configured motorcycle prefab with physics components according to the specifications in PrefabSetup.md.

**To use:**
1. First create a model using the Create Placeholder Models tool or import your own
2. Open the tool via `Tools > Setup Motorcycle Prefab`
3. Select the motorcycle model prefab
4. Adjust physics settings if needed
5. Set the motorcycle scale
6. Click "Create Motorcycle Prefab"

The tool will create a prefab in `Assets/Prefabs` with:
- Rigidbody
- Box Collider
- Wheel Colliders
- Empty GameObjects for mounting points
- MotorcycleController script 