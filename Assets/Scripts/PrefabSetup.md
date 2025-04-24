# Prefab Setup Guide

## Avatar Prefab Setup
1. Import `TS_PixelAvatarMain.fbx` into the `Assets/Models` folder
2. Create an empty GameObject and name it `AvatarPrefab`
3. Add the imported model as a child of `AvatarPrefab`
4. Add the following components to `AvatarPrefab`:
   - Character Controller
   - Animator (assign the avatar animation controller)
   - AvatarController script
5. Configure the Character Controller:
   - Height: 1.8
   - Radius: 0.3
   - Step Offset: 0.3
6. Add a capsule collider for physics interactions
7. Save as prefab in `Assets/Prefabs/AvatarPrefab.prefab`

## Motorcycle Prefab Setup
1. Import `TS_Motorcycle.fbx` into the `Assets/Models` folder
2. Create an empty GameObject and name it `MotorcyclePrefab`
3. Add the imported model as a child of `MotorcyclePrefab`
4. Add the following components to `MotorcyclePrefab`:
   - Rigidbody (adjust mass to around 200-300)
   - MotorcycleController script
5. Add empty GameObjects for:
   - Front Wheel (position at front wheel location)
   - Rear Wheel (position at rear wheel location)
   - Center of Mass (position slightly lower than the motorcycle's center)
   - Mount Position (where the avatar sits)
   - Handlebars (for steering)
6. Add WheelColliders to Front Wheel and Rear Wheel objects:
   - Radius: match visible wheel radius
   - Suspension Distance: 0.3
   - Adjust spring, damper, and other settings for desired physics
7. Set references in the MotorcycleController script
8. Add a BoxCollider to the motorcycle body
9. Save as prefab in `Assets/Prefabs/MotorcyclePrefab.prefab`

## AR Setup
1. Create an AR Scene with the following components:
   - AR Session
   - AR Session Origin
   - AR Plane Manager
   - AR Raycast Manager
2. Add UI components:
   - Canvas (Screen Space - Camera) with appropriate scaling
   - Instructions Panel with Text
   - Controls Panel with Joystick, Jump Button, and Interact Button
   - Place Avatar Button
   - Debug Panel (optional)
3. Add an empty GameObject and attach:
   - ARSceneController script
   - MotorcycleSpawner script
   - MotorcycleInteraction script
4. Set references for all scripts

## Final Adjustments
1. Adjust the avatar scale to fit your AR environment (usually between 0.1-1.0)
2. Configure physics settings in Project Settings:
   - Set Gravity to -9.81 on Y axis
   - Configure appropriate collision detection
3. Set up appropriate layers and collision matrix
4. Test and fine-tune physics behavior

When importing the models, make sure to:
1. Configure appropriate import settings for meshes
2. Set up animation clips and controllers
3. Adjust materials if needed
4. Configure colliders to closely match the visual mesh 