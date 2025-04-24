# Tequila Sunrise Development Workflow

This document outlines the workflow for the Tequila Sunrise project, including Git branching strategy, issue management, and development process.

## Table of Contents
1. [Git Branching Strategy](#git-branching-strategy)
2. [Issue Management](#issue-management)
3. [Pull Request Process](#pull-request-process)
4. [Testing and Quality Assurance](#testing-and-quality-assurance)
5. [Release Process](#release-process)

## Git Branching Strategy

The project uses a modified Git Flow approach with the following branches:

### Main Branch (`main`)
- Represents the production-ready state of the project
- Protected: no direct commits allowed
- Only merged from `dev` branch after thorough testing
- Tagged with version numbers for releases

### Development Branch (`dev`)
- Main integration branch for feature development
- Represents the latest delivered development changes
- Protected: no direct commits allowed
- Feature branches are merged into `dev` upon completion
- Should be in a stable state

### Feature Branches (`TS-<issue-number>-<brief-description>`)
- Created for each task or feature
- Always branch off from `dev`
- Named according to the pattern: `TS-<issue-number>-<brief-description>`
  - Example: `TS-7-avatar-animation-controller`
- Merged back into `dev` via pull request when complete
- Deleted after successful merge

### Hotfix Branches (`hotfix-<issue-number>-<brief-description>`)
- Created to fix critical bugs in production
- Branch from `main`
- Named according to the pattern: `hotfix-<issue-number>-<brief-description>`
  - Example: `hotfix-42-crash-on-startup`
- Merged into both `main` and `dev`
- Deleted after successful merge

## Branch Creation Example

```bash
# Make sure you're on the dev branch
git checkout dev

# Pull the latest changes
git pull origin dev

# Create a new feature branch
git checkout -b TS-7-avatar-animation-controller

# Make your changes, commit them
git add .
git commit -m "TS-7: Create avatar animation controller"

# Push the branch to remote
git push -u origin TS-7-avatar-animation-controller

# When complete, create a pull request to merge into dev
```

## Issue Management

The project uses GitHub Issues for task tracking. Each task is labeled with a unique identifier following the pattern `TS-<number>`.

### Issue Types
- **Feature**: New functionality
- **Bug**: Something that doesn't work as expected
- **Improvement**: Enhancement to existing functionality
- **Task**: General project tasks
- **Documentation**: Documentation updates

### Issue Workflow States
1. **Backlog**: Issues that are not yet scheduled for work
2. **Ready**: Issues that are ready to be worked on
3. **In Progress**: Issues currently being worked on
4. **Review**: Issues with completed work awaiting review
5. **Done**: Issues that are completed and merged

### Issue Creation Guidelines
- Use clear, descriptive titles
- Include detailed descriptions
- Add appropriate labels
- Assign to a milestone
- Link related issues
- Add acceptance criteria when applicable

### Issue Template

```markdown
## Description
[Detailed description of the issue or feature]

## Acceptance Criteria
- [ ] [Criterion 1]
- [ ] [Criterion 2]
- [ ] [Criterion 3]

## Additional Information
[Any other relevant information, screenshots, etc.]

## Tasks
- [ ] [Task 1]
- [ ] [Task 2]
- [ ] [Task 3]
```

## Pull Request Process

1. Create a pull request from your feature branch to the `dev` branch
2. Use a title format of `[TS-<number>] Brief description`
3. Include details about the changes and link to the related issue
4. Request reviews from at least one team member
5. Address review feedback promptly
6. Ensure all checks pass (if CI/CD is set up)
7. Merge only after approval from reviewer
8. Delete the feature branch after merging

### Pull Request Template

```markdown
## Description
[Detailed description of the changes]

## Related Issue
Closes #[issue-number]

## Type of Change
- [ ] New feature
- [ ] Bug fix
- [ ] Enhancement
- [ ] Documentation update

## Testing Performed
- [ ] Unit tests
- [ ] Integration tests
- [ ] Manual testing

## Checklist
- [ ] Code follows the project style guide
- [ ] Documentation has been updated
- [ ] All tests pass
- [ ] No new warnings or errors introduced
```

## Testing and Quality Assurance

### Testing Requirements
- All features should include appropriate unit tests
- AR functionality should be tested on target devices
- Performance testing on lower-end devices is required
- Test on both iOS and Android when applicable

### Code Review Guidelines
- Review for adherence to the [style guide](DEVELOPMENT_STYLE_GUIDE.md)
- Check for potential performance issues, especially in AR contexts
- Verify proper implementation of task requirements
- Look for security concerns or best practice violations
- Ensure proper documentation

## Release Process

1. **Prepare Release**
   - Merge `dev` into `main`
   - Update version numbers in relevant files
   - Create release notes

2. **Tag Release**
   - Create a Git tag for the version
   - Example: `git tag -a v1.0.0 -m "Version 1.0.0"`
   - Push the tag: `git push origin v1.0.0`

3. **Build Release**
   - Create builds for targeted platforms
   - Test the builds thoroughly

4. **Publish Release**
   - Create a GitHub Release with release notes
   - Attach build artifacts if applicable

5. **Post-Release**
   - Monitor for critical issues
   - Create hotfix branches if needed

## Sprint Cadence

The project follows a two-week sprint cycle:

- **Sprint Planning**: Beginning of each sprint
- **Daily Stand-ups**: Brief updates on progress and blockers
- **Sprint Review**: Demo of completed features at the end of sprint
- **Sprint Retrospective**: Team reflection and process improvement 