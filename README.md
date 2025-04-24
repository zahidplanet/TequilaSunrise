# Tequila Sunrise

A Unity AR project that lets users control a pixel avatar and interact with a motorcycle in augmented reality.

## Description

Tequila Sunrise is an AR mobile application built with Unity. It allows users to place a pixel-style avatar in AR environments, control its movement, and interact with a motorcycle that spawns on flat surfaces. The project is set up with Justin Barnett's Unity MCP package, allowing Claude to interact with Unity directly.

## Features

- AR surface detection and mapping
- Pixel avatar with mobile controls (joystick, jump button)
- Physically accurate motorcycle simulation
- Mounting/dismounting interaction between avatar and motorcycle
- One motorcycle spawns per scene on suitable flat surfaces

## Setup

The project includes:
- Unity project with Universal Render Pipeline
- Unity MCP integration for AI assistance
- XR capabilities with ARKit, ARCore, and XR Interaction Toolkit
- Complete project management and workflow documentation

## Requirements

- Unity Editor 2022.3 LTS or newer
- Compatible AR device (iOS with ARKit or Android with ARCore)
- Claude Desktop (with MCP configured) for AI assistance

## Project Structure

- **Assets/**: Unity project assets
  - **Models/**: 3D models including avatar and motorcycle
  - **Scripts/**: C# scripts organized by feature
  - **Prefabs/**: Reusable game objects
  - **Scenes/**: Unity scenes
  
- **Documentation/**:
  - [Project Roadmap](PROJECT_ROADMAP.md): Milestones and task breakdowns
  - [Development Style Guide](DEVELOPMENT_STYLE_GUIDE.md): Coding standards and best practices
  - [Workflow Guide](WORKFLOW.md): Git branching strategy and issue management

## Development Workflow

This project follows a structured development workflow:

1. Tasks are tracked as GitHub issues with the format `TS-<number>`
2. Feature branches are created following the pattern `TS-<number>-<description>`
3. Completed features are merged into the `dev` branch
4. Stable releases are merged from `dev` to `main`

See [WORKFLOW.md](WORKFLOW.md) for detailed instructions on the development process.

## GitHub Issue Management

The repository includes a Python script to automate the creation of GitHub issues from the BACKLOG.md file:

```bash
# Run the script to create all issues
python3 create_github_issues.py

# Run in debug mode (only creates a few issues for testing)
python3 create_github_issues.py --debug

# Run in dry-run mode (shows what would be created without making changes)
python3 create_github_issues.py --dry-run
```

This script:
1. Creates appropriate labels for milestones, priorities, and statuses
2. Creates milestones for each project phase
3. Creates GitHub issues for each task in the BACKLOG.md file
4. Assigns appropriate labels to each issue

You need the GitHub CLI (`gh`) installed and authenticated to use this script.

## Getting Started

1. Clone the repository
2. Open the project in Unity
3. Import the required models from the Models folder
4. Follow the setup instructions in Assets/Scripts/PrefabSetup.md
5. Build to your AR-capable device

## Contributing

Please read [WORKFLOW.md](WORKFLOW.md) and [DEVELOPMENT_STYLE_GUIDE.md](DEVELOPMENT_STYLE_GUIDE.md) before contributing to the project.

## License

This project is licensed under the MIT License - see the LICENSE file for details. 