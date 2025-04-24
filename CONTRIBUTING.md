# Contributing to Tequila Sunrise

First off, thank you for considering contributing to Tequila Sunrise! It's people like you that make Tequila Sunrise such a great project.

## Code of Conduct

This project and everyone participating in it is governed by our [Code of Conduct](CODE_OF_CONDUCT.md). By participating, you are expected to uphold this code.

## How Can I Contribute?

### Reporting Bugs

Before creating bug reports, please check the issue list as you might find out that you don't need to create one. When you are creating a bug report, please include as many details as possible:

* Use a clear and descriptive title
* Describe the exact steps to reproduce the problem
* Provide specific examples to demonstrate the steps
* Describe the behavior you observed after following the steps
* Explain which behavior you expected to see instead and why
* Include screenshots and animated GIFs if possible
* Include your environment details (OS, Unity version, etc.)

### Suggesting Enhancements

Enhancement suggestions are tracked as GitHub issues. When creating an enhancement suggestion, please include:

* Use a clear and descriptive title
* Provide a step-by-step description of the suggested enhancement
* Provide specific examples to demonstrate the steps
* Describe the current behavior and explain the behavior you expected to see
* Explain why this enhancement would be useful
* List some other applications where this enhancement exists, if applicable
* Include screenshots and animated GIFs if relevant

### Pull Requests

1. Fork the repo and create your branch from `dev`
2. If you've added code that should be tested, add tests
3. If you've changed APIs, update the documentation
4. Ensure the test suite passes
5. Make sure your code follows the existing style
6. Issue that pull request!

## Development Process

### Setting up the Development Environment

1. Install Unity 2022.3 LTS
2. Clone the repository
3. Install Git LFS
4. Open the project in Unity
5. Install required packages through the Package Manager

### Coding Standards

Please follow our [Development Style Guide](docs/development/workflow.md) for:

* Code formatting
* Naming conventions
* Architecture patterns
* Documentation requirements
* Testing standards

### Commit Messages

* Use the present tense ("Add feature" not "Added feature")
* Use the imperative mood ("Move cursor to..." not "Moves cursor to...")
* Limit the first line to 72 characters or less
* Reference issues and pull requests liberally after the first line
* Format: `TS-{issue-number}: {type}: {description}`

Example:
```
TS-42: feat: Add motorcycle lean mechanics

- Implement physics-based leaning
- Add user input controls
- Update documentation
```

### Branch Naming Convention

* Feature branches: `feature/TS-{issue-number}-{description}`
* Bug fix branches: `bugfix/TS-{issue-number}-{description}`
* Release branches: `release/v{version}`
* Hotfix branches: `hotfix/v{version}`

### Testing

* Write unit tests for new features
* Update existing tests when modifying code
* Ensure all tests pass before submitting PR
* Include integration tests for AR features
* Test on multiple devices when possible

### Documentation

When contributing, please update the documentation accordingly:

* Update README.md if needed
* Add/update API documentation
* Update technical documentation
* Include code comments
* Update changelog

### Review Process

1. Submit your PR
2. Wait for review from two team members
3. Address any comments/requests
4. Update your PR based on feedback
5. Wait for final approval
6. Merge after CI passes

## Project Structure

```
TequilaSunrise/
├── Assets/
│   ├── Scripts/
│   │   ├── AR/          # AR-related scripts
│   │   ├── Core/        # Core game systems
│   │   ├── UI/          # UI scripts
│   │   └── Utils/       # Utility scripts
│   ├── Scenes/          # Unity scenes
│   ├── Prefabs/         # Prefab assets
│   ├── Materials/       # Material assets
│   └── Tests/           # Test scripts
├── Packages/            # Unity packages
└── ProjectSettings/     # Unity settings
```

## Communication

* Use GitHub issues for bug reports and feature requests
* Use PR comments for code-related discussions
* Join our Discord server for general discussion
* Check the wiki for additional documentation

## Recognition

Contributors will be recognized in:

* The project README
* Release notes
* Documentation
* Our website (coming soon)

## Questions?

* Check our [FAQ](docs/FAQ.md)
* Join our Discord server
* Create a GitHub issue
* Contact the maintainers

Thank you for contributing to Tequila Sunrise! 🌅 