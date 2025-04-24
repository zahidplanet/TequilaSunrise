using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

namespace TequilaSunrise.AR
{
    /// <summary>
    /// Utility functions for AR-related operations
    /// </summary>
    public static class ARUtility
    {
        /// <summary>
        /// Checks if the device supports AR functionality
        /// </summary>
        public static bool IsARSupported()
        {
            #if UNITY_IOS
            return UnityEngine.XR.ARKit.ARKitLoader.IsSupported;
            #elif UNITY_ANDROID
            return UnityEngine.XR.ARCore.ARCoreLoader.IsSupported;
            #else
            return false;
            #endif
        }
        
        /// <summary>
        /// Finds the best placement position on a plane
        /// </summary>
        public static bool TryGetPlacementPose(ARRaycastManager raycastManager, Vector2 screenPosition, out Pose pose)
        {
            var hits = new List<ARRaycastHit>();
            
            if (raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                pose = hits[0].pose;
                
                // Adjust pose to ensure object sits on the plane
                var cameraForward = Camera.main.transform.forward;
                var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
                pose.rotation = Quaternion.LookRotation(cameraBearing);
                
                return true;
            }
            
            pose = default;
            return false;
        }
        
        /// <summary>
        /// Checks if a plane is suitable for object placement
        /// </summary>
        public static bool IsSuitablePlane(ARPlane plane, float minSize = 0.5f)
        {
            if (plane == null) return false;
            
            // Check plane size
            var bounds = plane.bounds;
            float area = bounds.size.x * bounds.size.y;
            
            return area >= (minSize * minSize);
        }
        
        /// <summary>
        /// Gets the angle between the device and the ground plane
        /// </summary>
        public static float GetDeviceGroundAngle()
        {
            if (Camera.main == null) return 0f;
            
            Vector3 deviceForward = -Camera.main.transform.forward;
            return Vector3.Angle(deviceForward, Vector3.up);
        }
        
        /// <summary>
        /// Calculates a stable position for object placement
        /// </summary>
        public static Vector3 GetStablePosition(Vector3 position, float heightOffset = 0f)
        {
            return new Vector3(position.x, position.y + heightOffset, position.z);
        }
        
        /// <summary>
        /// Checks if the camera is moving too fast for reliable tracking
        /// </summary>
        public static bool IsCameraMovingTooFast(float threshold = 0.1f)
        {
            if (Camera.main == null) return false;
            
            var velocity = Camera.main.velocity;
            return velocity.magnitude > threshold;
        }
        
        /// <summary>
        /// Gets the distance from the camera to a world position
        /// </summary>
        public static float GetDistanceFromCamera(Vector3 worldPosition)
        {
            if (Camera.main == null) return 0f;
            
            return Vector3.Distance(Camera.main.transform.position, worldPosition);
        }
        
        /// <summary>
        /// Converts a screen position to a ray in world space
        /// </summary>
        public static Ray ScreenPointToRay(Vector2 screenPoint)
        {
            if (Camera.main == null) return new Ray();
            
            return Camera.main.ScreenPointToRay(screenPoint);
        }
        
        /// <summary>
        /// Checks if a point is within the camera's field of view
        /// </summary>
        public static bool IsInCameraView(Vector3 worldPosition)
        {
            if (Camera.main == null) return false;
            
            var viewportPoint = Camera.main.WorldToViewportPoint(worldPosition);
            return viewportPoint.x >= 0 && viewportPoint.x <= 1 &&
                   viewportPoint.y >= 0 && viewportPoint.y <= 1 &&
                   viewportPoint.z > 0;
        }
    }
}