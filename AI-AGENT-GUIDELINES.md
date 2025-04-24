# AI Agent Project Guidelines for TequilaSunrise

## Core Principles
1. **Code Quality**: Ensure all code follows C# best practices and Unity conventions.
2. **Clean Architecture**: Maintain proper separation of concerns and clear namespaces.
3. **Documentation**: Document all public APIs and complex functionality.
4. **Testing**: Create appropriate unit and integration tests for critical components.
5. **Error Handling**: Implement robust error handling and logging.

## Development Workflow
1. **Branch Management**:
   - Create a branch for each task/issue (TS-X-description)
   - Don't merge until all compiler errors are fixed
   - Keep branches focused on a single task
   
2. **Commit Strategy**:
   - Commit frequently with clear messages
   - Prefix commits with task number (TS-X: description)
   - Keep commits focused on logical changes

3. **Pull Requests**:
   - Create descriptive PR using the PR template
   - Ensure all tests pass
   - Fix all compiler errors before requesting review

## Code Standards
1. **Namespaces**:
   - Use `TequilaSunrise.{Module}` namespace pattern
   - Keep namespace structure clean and logical

2. **Script Organization**:
   - Group scripts in appropriate folders by functionality
   - Use partial classes for complex behaviors when appropriate
   - Keep files under 500 lines when possible

3. **Dependencies**:
   - Document all external dependencies
   - Create compatibility layers for third-party assets when needed
   - Prefer Unity's built-in solutions when available

## AR Development Specifics
1. **AR Foundation**:
   - Follow Unity's AR Foundation best practices
   - Test on both iOS and Android devices
   - Handle device compatibility gracefully

2. **Performance**:
   - Optimize mobile performance
   - Minimize garbage collection
   - Profile regularly on target devices

3. **User Experience**:
   - Design mobile-first interfaces
   - Ensure intuitive AR interactions
   - Add visual feedback for all user actions

## Project Structure
1. **Assets Organization**:
   - Scripts: Organized by domain/feature
   - Prefabs: Reusable components with clear naming
   - Scenes: Well-structured with descriptive names
   - Materials/Textures: Organized by object/theme

2. **Build Configuration**:
   - Maintain separate configurations for debug/release
   - Document platform-specific settings
   - Validate builds for target platforms

## Documentation
1. **Code Documentation**:
   - Comment all public methods and properties
   - Explain complex algorithms and logic
   - Document expected inputs/outputs

2. **Project Documentation**:
   - Maintain README with project overview
   - Document setup instructions
   - Keep task tracking up to date

## Task Completion Checklist
- [x] All compiler errors resolved
- [x] Code follows namespace conventions
- [x] Unit tests pass
- [x] Documentation updated
- [x] Performance validated
- [x] Builds correctly on target platforms 