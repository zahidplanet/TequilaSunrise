# Asset Structure

## Directory Organization

```
Assets/
├── Models/
│   ├── Characters/    # Character FBX models
│   ├── Vehicles/      # Motorcycle and vehicle FBX models
│   ├── Props/         # Interactive objects and props
│   └── Environments/  # Environmental models and decorations
│
├── Animations/
│   ├── Characters/    # Character animation clips
│   └── Vehicles/      # Vehicle animation clips
│
├── Materials/
│   ├── Characters/    # Character materials and shaders
│   ├── Vehicles/      # Vehicle materials
│   ├── Props/         # Prop materials
│   └── Environments/  # Environment materials
│
├── Textures/
│   ├── Characters/    # Character textures and sprites
│   ├── Vehicles/      # Vehicle textures
│   ├── Props/         # Prop textures
│   └── Environments/  # Environment textures
│
├── Prefabs/
│   ├── Characters/    # Character prefab variants
│   ├── Vehicles/      # Vehicle prefab variants
│   ├── Props/         # Prop prefabs
│   └── UI/            # UI element prefabs
│
├── Scripts/          # C# scripts (organized by domain)
├── Scenes/           # Unity scenes
└── Resources/        # Runtime-loaded assets
```

## Import Guidelines

### Models
- FBX format preferred
- Normalized scale (1 unit = 1 meter)
- Y-up orientation
- Forward = Z+ axis
- Triangulated meshes
- Named materials

### Textures
- PBR textures: 2048x2048 or 1024x1024
- UI textures: Power of 2 dimensions
- Compressed formats:
  - Android: ETC2
  - iOS: ASTC
  - Normal maps: BC5

### Animations
- 30 FPS sample rate
- Normalized time (0-1 range)
- Labeled animation events
- Root motion when applicable

### Materials
- URP/Lit shader base
- PBR workflow
- Material variants in prefabs
- Shared materials when possible

## Naming Conventions

### Models
- Character: `CH_[Name]_[Variant]`
- Vehicle: `VH_[Type]_[Variant]`
- Prop: `PR_[Category]_[Name]`
- Environment: `EN_[Category]_[Name]`

### Animations
- Character: `CH_[Name]_[Action]_[Variant]`
- Vehicle: `VH_[Type]_[Action]_[Variant]`

### Materials
- `MAT_[Category]_[Name]_[Variant]`

### Textures
- Albedo: `T_[Name]_D`
- Normal: `T_[Name]_N`
- Metallic/Roughness: `T_[Name]_MR`
- Emission: `T_[Name]_E`

### Prefabs
- `PF_[Category]_[Name]_[Variant]`

## Version Control

- Large binary files tracked with Git LFS
- FBX files: Git LFS
- Texture source files: Git LFS
- Unity meta files: Git standard

## Quality Settings

### Models
- Max triangle count per asset:
  - Characters: 10k
  - Vehicles: 15k
  - Props: 5k
  - Environment: Based on LOD level

### Textures
- Maximum sizes:
  - Characters: 2048x2048
  - Vehicles: 2048x2048
  - Props: 1024x1024
  - Environment: 1024x1024
  - UI: 512x512

### Materials
- Maximum texture maps per material: 4
- Shader complexity: Medium
- Mobile-optimized variants available 