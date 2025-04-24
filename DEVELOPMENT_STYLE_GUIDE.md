# Tequila Sunrise Development Style Guide

## Introduction
This document outlines the coding standards, project organization, and best practices for the Tequila Sunrise AR project. Following these guidelines ensures consistency across the codebase and facilitates collaboration among team members.

## Table of Contents
1. [Unity Project Structure](#unity-project-structure)
2. [C# Coding Standards](#c-coding-standards)
3. [Asset Naming Conventions](#asset-naming-conventions)
4. [Code Documentation](#code-documentation)
5. [Scene Organization](#scene-organization)
6. [Prefab Creation](#prefab-creation)
7. [Version Control](#version-control)
8. [Performance Guidelines](#performance-guidelines)

## Unity Project Structure

The project follows this folder structure:

```
Assets/
├── Animations/          # Animation clips and controllers
├── Models/              # 3D models and avatars
├── Materials/           # Material assets
├── Prefabs/             # Prefab assets
├── Scenes/              # Scene files
├── Scripts/             # C# scripts, organized by feature
│   ├── AR/              # AR functionality scripts
│   ├── Avatar/          # Avatar control scripts
│   ├── Motorcycle/      # Motorcycle scripts
│   ├── UI/              # UI scripts
│   └── Utils/           # Utility scripts
├── Settings/            # Project settings and configurations
├── Textures/            # Texture assets
└── UI/                  # UI assets and prefabs
```

Keep all assets in their appropriate folders. Create new folders as needed, but maintain the overall organization.

## C# Coding Standards

### Naming Conventions
- **PascalCase** for class names, method names, properties, and public fields
  - Example: `MotorcycleController`, `HandleMovement()`
- **camelCase** for local variables and private/protected fields
  - Example: `float moveSpeed`, `private Vector3 moveDirection`
- **UPPER_CASE** for constants
  - Example: `const float MAX_SPEED = 30f`
- Prefix private fields with an underscore (`_`)
  - Example: `private float _jumpForce`

### Script Structure
- Organize scripts with the following sections:
  1. Field declarations (grouped by visibility and functionality)
  2. Unity lifecycle methods in chronological order (Awake, Start, Update, etc.)
  3. Public methods
  4. Private methods
  5. Coroutines
  6. Utility functions

```csharp
using UnityEngine;

public class ExampleClass : MonoBehaviour
{
    // Fields
    [Header("References")]
    [SerializeField] private Transform _target;
    
    [Header("Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    
    // Private variables
    private Vector3 _initialPosition;
    
    // Unity lifecycle methods
    private void Awake()
    {
        // Initialize components
    }
    
    private void Start()
    {
        _initialPosition = transform.position;
    }
    
    private void Update()
    {
        HandleMovement();
    }
    
    // Public methods
    public void Reset()
    {
        transform.position = _initialPosition;
    }
    
    // Private methods
    private void HandleMovement()
    {
        // Implementation
    }
}
```

### Code Formatting
- Use 4 spaces for indentation (no tabs)
- Put braces on their own line
- Use a space before opening braces
- Use a space after commas and around operators

### Best Practices
- Keep classes small and focused on a single responsibility
- Prefer composition over inheritance
- Use [SerializeField] instead of public for Unity Inspector fields
- Group related fields with [Header] attributes
- Cache component references in Awake when possible
- Avoid using Update for infrequent checks (use coroutines or InvokeRepeating instead)
- Avoid direct GameObject.Find calls; reference via Inspector or other methods
- Use meaningful variable and method names that describe their purpose

## Asset Naming Conventions

- **Prefabs**: `PFB_Category_Name`
  - Example: `PFB_Motorcycle_Sport`
- **Materials**: `MAT_Category_Name`
  - Example: `MAT_Avatar_Body`
- **Textures**: `TEX_Category_Name_Type`
  - Example: `TEX_Motorcycle_Body_Diffuse`
- **Animations**: `ANIM_Character_Action`
  - Example: `ANIM_Avatar_Jump`
- **Scripts**: `CategoryName` (without prefix)
  - Example: `AvatarController.cs`
- **Scenes**: `SCN_SceneName`
  - Example: `SCN_MainARScene`

## Code Documentation

- Add summary comments to all classes and public methods
- Include parameter descriptions for methods
- Explain complex algorithms or non-obvious functionality
- Use XML documentation style for C# scripts:

```csharp
/// <summary>
/// Controls the movement and physics of the motorcycle.
/// </summary>
public class MotorcycleController : MonoBehaviour
{
    /// <summary>
    /// Applies force to the motorcycle based on input.
    /// </summary>
    /// <param name="force">Amount of force to apply</param>
    /// <param name="forcePosition">Position to apply the force</param>
    public void ApplyForce(float force, Vector3 forcePosition)
    {
        // Implementation
    }
}
```

## Scene Organization

- Organize scene hierarchies using empty GameObjects as folders
- Use a consistent naming pattern for hierarchy organization:
  ```
  - AR (AR-specific components)
  - Environment (environmental objects)
  - UI (UI elements)
  - Managers (controller scripts)
  - Lighting (light sources)
  ```
- Keep the hierarchy clean and organized
- Use prefabs for complex objects rather than building them in the scene

## Prefab Creation

- Make prefabs modular and reusable
- Set up proper pivot points for all objects
- Configure colliders to match visual representation
- Ensure all necessary components are attached and configured
- Use nested prefabs for complex objects
- Document any special setup or requirements in prefab script references

## Version Control

- The project uses Git with GitHub
- Follow the branching strategy described in WORKFLOW.md
- Never commit directly to main or dev branches
- Create feature branches for each task
- Use meaningful commit messages that describe the changes
- Keep commits small and focused on a single change
- Avoid committing sensitive or auto-generated files
- Resolve merge conflicts promptly

## Performance Guidelines

### Mobile Optimization
- Target 60fps on mid-range devices
- Use the profiler regularly to identify bottlenecks
- Minimize draw calls by using texture atlases
- Keep polygon counts reasonable for mobile devices
- Limit real-time lights and use baked lighting where possible
- Avoid excessive physics calculations
- Use object pooling for frequently spawned objects
- Implement LOD (Level of Detail) for complex models

### AR-Specific Optimization
- Limit the number of tracked planes
- Use efficient raycasting by limiting frequency
- Reduce CPU load from vision processing by limiting unnecessary features
- Optimize texture sizes for mobile devices
- Use occlusion planes sparingly
- Test regularly on target devices, not just in the Unity Editor

By following these guidelines, we ensure a consistent and maintainable codebase, facilitating collaboration and improving the overall quality of the Tequila Sunrise project. 