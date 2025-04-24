# Contributing to Tequila Sunrise

Thank you for your interest in contributing to Tequila Sunrise! This document provides guidelines and workflows to make the contribution process smooth and effective.

## Code of Conduct

Please read and follow our [Code of Conduct](CODE_OF_CONDUCT.md) to foster an inclusive and respectful community.

## Getting Started

1. Fork the repository
2. Clone your fork: `git clone https://github.com/yourusername/TequilaSunrise.git`
3. Create a branch for your work: `git checkout -b feature/your-feature-name`

## Development Workflow

### Branch Naming Convention

- `feature/` - For new features
- `bugfix/` - For bug fixes
- `docs/` - For documentation changes
- `refactor/` - For code refactoring
- `test/` - For adding or modifying tests

Example: `feature/add-user-authentication`

### Commit Messages

Follow the [Conventional Commits](https://www.conventionalcommits.org/) standard:

```
<type>(<scope>): <description>

[optional body]

[optional footer]
```

Types:
- `feat`: A new feature
- `fix`: A bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, missing semicolons, etc)
- `refactor`: Code changes that neither fix bugs nor add features
- `test`: Adding or modifying tests
- `chore`: Changes to the build process or auxiliary tools

Example: `feat(auth): implement login functionality`

### Pull Request Process

1. Update your branch with the latest changes from main: `git pull origin main`
2. Resolve any merge conflicts
3. Ensure your code passes all tests
4. Submit a pull request with a clear title and description
5. Link the PR to any relevant issues
6. Wait for code review and address any feedback

## Issue Tracking

- Check the existing issues before creating a new one
- Use issue templates when available
- Be specific and provide steps to reproduce for bug reports
- For feature requests, clearly explain the use case and expected outcome

## Code Style and Quality

- Follow the existing code style of the project
- Write clear, readable, and maintainable code
- Include appropriate comments and documentation
- Write tests for new features and bug fixes
- Ensure all tests pass before submitting a PR

## Review Process

All submissions require review before being merged:

1. At least one core team member must approve the changes
2. CI checks must pass
3. All review comments must be resolved

## Resources

- [Project Documentation](docs/)
- [Issue Tracker](https://github.com/yourusername/TequilaSunrise/issues)

Thank you for contributing to Tequila Sunrise! 