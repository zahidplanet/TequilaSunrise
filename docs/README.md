# Tequila Sunrise - AR Mobile Experience

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)]()
[![Test Status](https://img.shields.io/badge/tests-passing-brightgreen)]()
[![Release](https://img.shields.io/badge/release-v0.1.0--alpha-blue)]()

Tequila Sunrise is an innovative AR mobile application that combines pixel art aesthetics with modern augmented reality technology. The app creates an immersive experience where users can interact with stylized motorcycles and environments in their real-world space.

## Quick Links
- [Technical Documentation](technical/README.md)
- [Design Guidelines](design/README.md)
- [Development Workflow](workflow/README.md)
- [Project Roadmap](roadmap/README.md)
- [API Documentation](api/README.md)
- [Build Guide](build/README.md)
- [Testing Strategy](testing/README.md)
- [Monetization Plan](monetization/README.md)

## Getting Started

### Prerequisites
- Unity 2022.3 LTS
- Xcode 15+ (for iOS builds)
- Android Studio (for Android builds)
- Git LFS
- Visual Studio or VS Code

### Setup
1. Clone the repository:
```bash
git clone https://github.com/yourusername/TequilaSunrise.git
git lfs pull
```

2. Open the project in Unity 2022.3 LTS
3. Install required packages through the Package Manager
4. Open the ARTest scene in Assets/Scenes/
5. Configure your development environment using the Build Configuration tool

## Core Features
- AR Foundation integration for robust AR experiences
- Pixel art style rendering in AR space
- Physics-based motorcycle controls
- Environmental interaction system
- Cross-platform support (iOS/Android)
- Performance-optimized rendering
- Modular architecture for extensibility

## Technology Stack
- Unity 2022.3 LTS
- AR Foundation 5.0
- Universal Render Pipeline (URP)
- TextMeshPro
- DOTween
- UniTask
- Zenject (Dependency Injection)

## Project Structure
```
TequilaSunrise/
├── Assets/
│   ├── Scripts/
│   │   ├── AR/
│   │   ├── Core/
│   │   ├── Motorcycle/
│   │   └── UI/
│   ├── Scenes/
│   ├── Art/
│   └── Prefabs/
├── Packages/
└── docs/
```

## Development Workflow
We follow a feature-branch workflow:
1. Create feature branch from `dev`
2. Implement and test changes
3. Submit PR for review
4. Merge to `dev` after approval
5. Regular releases to `main`

## Contributing
1. Review our [contribution guidelines](CONTRIBUTING.md)
2. Check the [development workflow](workflow/README.md)
3. Follow our [coding standards](technical/coding-standards.md)
4. Submit PRs with detailed descriptions

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support
For technical support or feature requests:
- Create an issue in the GitHub repository
- Contact the development team at dev@tequilasunrise.com

---
Last updated: 2024-03-21 