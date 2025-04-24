# Design Guidelines

## Visual Style Guide

### Pixel Art Style
- Resolution: 16x16 base grid for characters, 32x32 for vehicles
- Color Palette: Limited to 32 colors per asset
- Dithering: Used sparingly for gradients and shadows
- Animation: 8-12 frames per second for character/vehicle animations

### Color Scheme
Primary Colors:
- Sunset Orange: `#FF6B35`
- Night Blue: `#004E89`
- Desert Sand: `#FFBC42`
- Cactus Green: `#2E933C`

Secondary Colors:
- UI Highlight: `#FF4B6B`
- Background: `#1A1A1A`
- Text Primary: `#FFFFFF`
- Text Secondary: `#CCCCCC`

### Typography
- Headers: Press Start 2P (pixel font)
- Body Text: Roboto for readability
- UI Elements: Combination of pixel fonts and modern sans-serif

## UI/UX Design

### Interface Principles
1. Minimal HUD elements
2. Touch-friendly button sizes (minimum 44x44 points)
3. Clear visual feedback for all interactions
4. Consistent button placement across screens

### AR Interface Guidelines
1. Clear AR surface indicators
2. Minimal screen overlay during AR sessions
3. Intuitive gesture controls
4. Visual guides for optimal scanning distance

### Menu Structure
```
Main Menu
├── Start Experience
├── Garage
│   ├── Motorcycle Selection
│   ├── Customization
│   └── Stats
├── Settings
│   ├── Graphics
│   ├── Controls
│   └── Audio
└── Profile
```

## Asset Creation Standards

### Motorcycles
- Base Resolution: 32x32 pixels
- Animation States:
  - Idle
  - Acceleration
  - Braking
  - Turning (Left/Right)
- Customizable Elements:
  - Color Schemes
  - Decals
  - Accessories

### Environment Assets
- Modular design for easy assembly
- Consistent scale with motorcycle assets
- Clear collision boundaries
- Performance-optimized geometry

### UI Assets
- Vector-based UI frames and borders
- Pixel-perfect icons at 16x16/32x32
- Scalable button states
- Consistent padding and margins

## Animation Guidelines

### Character Animations
- 8 directional sprites
- Key poses:
  - Idle
  - Walking
  - Running
  - Mounting/Dismounting
- Smooth transitions between states

### Effects
- Particle systems using pixel art textures
- Limited particle count for performance
- Clear visual feedback for interactions
- Screen-space effects for AR transitions

## AR Integration

### Visual Markers
- High contrast for reliable detection
- Minimal design that fits aesthetic
- Clear scanning feedback
- Fallback detection methods

### Environmental Blending
- Soft shadows for grounding
- Ambient occlusion consideration
- Light estimation integration
- Real-world scale matching

## Performance Considerations

### Asset Optimization
- Texture atlasing for similar elements
- Minimal draw calls per frame
- Efficient UV mapping
- LOD implementation for complex models

### Memory Management
- Texture compression guidelines
- Asset streaming recommendations
- Cache management
- Resource pooling

## Documentation Requirements

### Asset Documentation
- Naming conventions
- File organization
- Version control guidelines
- Metadata requirements

### Style Guide Maintenance
- Regular updates process
- Change documentation
- Team review procedures
- Version history

## Tools and Resources

### Recommended Software
- Aseprite for pixel art
- Unity 2022.3 LTS
- TexturePacker for atlasing
- Git LFS for version control

### Asset Templates
- Character sprite template
- Motorcycle base template
- UI element templates
- Effect sprite sheets

## Quality Assurance

### Review Process
1. Initial design review
2. Technical feasibility check
3. Performance impact assessment
4. Final approval workflow

### Testing Requirements
- Cross-device testing
- Performance benchmarking
- Visual consistency checks
- AR compatibility verification

---

This document is maintained by the Design Team and should be reviewed monthly for updates and improvements. 