# AR Setup Guide

## Overview
This guide covers the setup and configuration of AR functionality in the Tequila Sunrise project.

## Prerequisites
- Unity 2022.3 LTS or later
- AR Foundation 5.0 or later
- iOS 13+ or Android 7.0+ device with ARKit/ARCore support

## Initial Setup

### 1. AR Foundation Configuration
- AR Foundation package is already configured in the project
- XR Plugin Management is set up for both iOS (ARKit) and Android (ARCore)

### 2. Scene Setup
1. Open the ARTest scene in `Assets/Scenes/ARTest.unity`
2. Verify the following objects exist in the scene:
   - AR Session
   - AR Session Origin
   - AR Camera
   - AR Plane Manager

### 3. Plane Visualization Setup
1. Open Unity Editor
2. Navigate to TequilaSunrise > AR > Create Plane Visualization
3. Configure the plane visualization:
   - Set the plane material
   - Adjust plane scale if needed
   - Set the plane color (default: semi-transparent white)
4. Click "Create Plane Prefab" to generate the visualization prefab

### 4. Runtime Configuration
The `ARTestSceneSetup` script handles:
- AR session initialization
- Plane detection
- Status updates
- User instructions

## Testing
1. Build the project for your target platform (iOS/Android)
2. Install on a compatible device
3. Verify:
   - AR session initializes correctly
   - Plane detection works
   - Visualization appears for detected planes
   - Status updates are visible

## Troubleshooting
### Common Issues
1. AR Session not starting:
   - Check device compatibility
   - Verify camera permissions
   - Ensure proper platform settings

2. Planes not visible:
   - Check ARPlaneManager is enabled
   - Verify plane prefab is assigned
   - Check lighting settings

### Platform-Specific Notes
#### iOS
- Requires Privacy - Camera Usage Description in Info.plist
- ARKit compatibility check in Player Settings

#### Android
- Requires AR Core compatibility check
- Camera permission in AndroidManifest.xml

## Performance Considerations
- Keep mesh resolution balanced for performance
- Use occlusion planes sparingly
- Consider using plane merging for large areas

## Next Steps
- Implement plane selection
- Add interaction with detected planes
- Configure plane filtering by type 