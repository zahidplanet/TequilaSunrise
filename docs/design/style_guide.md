# Design Style Guide

## Visual Identity

### Color Palette

#### Primary Colors
- **Sunset Orange** `#FF6B35` - Main brand color, used for CTAs and important UI elements
- **Desert Sand** `#F7C59F` - Secondary color, used for backgrounds and accents
- **Night Blue** `#2D3142` - Text and UI elements
- **Twilight Purple** `#8D86C9` - Accent color for special features
- **Cactus Green** `#7CAA2D` - Success states and progression indicators

#### Secondary Colors
- **Light Sand** `#FFF1E6` - Background color
- **Dusk Gray** `#4F5D75` - Secondary text
- **Sunset Red** `#E94F37` - Error states
- **Sky Blue** `#7FB7BE` - Information states
- **Desert Gold** `#FFB400` - Warning states

### Typography

#### Fonts
- **Primary Font**: Pixellari
  - Used for headings and important UI elements
  - Weights: Regular (400), Bold (700)
  
- **Secondary Font**: Press Start 2P
  - Used for game text and UI elements
  - Weight: Regular (400)
  
- **Body Font**: VT323
  - Used for body text and descriptions
  - Weight: Regular (400)

#### Text Sizes
- H1: 32px
- H2: 24px
- H3: 20px
- Body: 16px
- Small: 14px
- Caption: 12px

### Pixel Art Style

#### Character Design
- 32x32px base size for character sprites
- Limited color palette (max 16 colors per sprite)
- Clear silhouettes
- Consistent pixel size (no mixing resolutions)
- 2:1 pixel ratio for natural proportions

#### Vehicle Design
- 64x32px base size for motorcycles
- Detailed pixel shading for metal surfaces
- Animated elements (wheels, exhaust effects)
- Clear read of vehicle state
- Consistent perspective (¾ view)

#### Environment Design
- Modular 32x32px tiles
- Seamless transitions
- Depth indicated through parallax
- Weather effects using particle systems
- Dynamic lighting system

## UI/UX Design

### Interface Elements

#### Buttons
- Minimum touch target: 44x44px
- Clear hover/pressed states
- Pixel art borders (2px)
- Icon + text for clarity
- Consistent padding (8px)

#### Icons
- 16x16px base size
- Single color with optional highlight
- Clear silhouettes
- Consistent stroke width
- Pixel-perfect alignment

#### Menus
- Grid-based layout
- Clear hierarchy
- Smooth transitions
- Consistent spacing
- Pixel art decorative elements

### Navigation

#### Main Menu
- Clear primary action
- Quick access to key features
- Visual feedback
- Consistent back button
- Breadcrumb navigation

#### HUD Elements
- Minimal intrusion
- Clear status indicators
- Quick access to core features
- Contextual information
- Smooth fade transitions

### Feedback Systems

#### Visual Feedback
- Clear success/failure states
- Particle effects for interactions
- Screen shake for impacts
- Flash effects for important events
- Progress indicators

#### Audio Feedback
- 8-bit style sound effects
- Consistent volume levels
- Stereo positioning
- Clear action mapping
- Layered sound design

## AR Design Guidelines

### AR Interface

#### Placement Indicators
- Clear boundary visualization
- Surface detection feedback
- Scale references
- Orientation guides
- Error state visualization

#### Interactive Elements
- Clear touch targets
- Depth-aware interactions
- Gesture visualization
- State transitions
- Feedback animations

### AR Content

#### 3D Models
- Optimized polygon count
- Pixel art textures
- Clear LOD transitions
- Consistent scale
- Physics-based interactions

#### Effects
- Particle systems
- Environment reflections
- Shadow casting
- Lighting integration
- Weather effects

### Performance Guidelines

#### Optimization
- Maximum draw calls: 100
- Texture size limits: 1024x1024
- Polygon budget per scene: 100k
- Shader complexity limits
- Memory usage targets

#### Quality Settings
- Dynamic resolution scaling
- LOD system configuration
- Shadow quality levels
- Effect density scaling
- Texture streaming settings

## Animation Guidelines

### Character Animation
- 8 frame walk cycle
- 4 frame idle animation
- 12 frame action sequences
- Consistent frame timing
- Smooth transitions

### Vehicle Animation
- Wheel rotation speeds
- Suspension movement
- Engine effects
- Impact reactions
- Environmental interaction

### UI Animation
- 200ms transition duration
- Ease-in-out timing
- Consistent direction
- Scale and fade effects
- Pixel-perfect movement

## Accessibility

### Visual Design
- High contrast mode
- Colorblind friendly palette
- Scalable UI elements
- Clear text hierarchy
- Alternative text indicators

### Interaction Design
- Multiple input methods
- Adjustable timing
- Clear feedback
- Error prevention
- Recovery options

## Asset Creation Guidelines

### File Organization
- Clear naming convention
- Folder structure
- Version control
- Asset categories
- Reference files

### Export Settings
- PNG format for sprites
- GLTF format for 3D
- Compressed audio formats
- Optimized textures
- Quality presets

## Documentation

### Asset Documentation
- Creation guidelines
- Usage examples
- Technical specifications
- Performance impact
- Implementation notes

### Style Updates
- Version control
- Change documentation
- Update process
- Review system
- Distribution method

---

This style guide is maintained by the Design Team and updated regularly to reflect current best practices and technical capabilities. 