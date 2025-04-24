# Pull Request: TS-10 Fix Compiler Errors

## Description
This PR addresses compiler errors in the project by implementing a TweenUtility class and LeanTween compatibility layer, along with updating namespaces and fixing references in Avatar and Motorcycle scripts.

## Changes Made
- Added TweenUtility implementation for animation tweening
- Created LeanTweenCompat compatibility layer to avoid dependency on LeanTween
- Updated Avatar classes to use proper TequilaSunrise.Avatar namespace
- Updated Motorcycle controller to use linearVelocity instead of velocity
- Fixed namespace references and imports

## Test Results
- Addressed all compiler errors
- No runtime errors in the editor

## Checklist
- [x] Code follows coding standards and style (TS.*)
- [x] Unit tests pass (where applicable)
- [x] Documentation updated
- [x] Build completes successfully

## Related Issues
Closes #10 