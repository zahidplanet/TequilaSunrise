using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace TequilaSunrise.AR
{
    /// <summary>
    /// Manages the placement of avatar in AR space
    /// </summary>
    public class AvatarPlacementManager : MonoBehaviour
    {
        [Header("AR Components")]
        [SerializeField] private ARRaycastManager raycastManager;
        [SerializeField] private ARPlaneManager planeManager;
        
        [Header("Avatar Settings")]
        [SerializeField] private GameObject avatarPrefab;
        [SerializeField] private float placementHeight = 0.0f;
        
        [Header("Placement Settings")]
        [SerializeField] private bool autoPlace = false;
        [SerializeField] private float autoPlaceDelay = 2.0f;
        [SerializeField] private GameObject placementIndicator;
        
        [Header("UI References")]
        [SerializeField] private GameObject placementUI;
        [SerializeField] private UnityEngine.UI.Text placementInstructionsText;
        
        private GameObject spawnedAvatar;
        private List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();
        private bool isPlaced = false;
        private float autoPlaceTimer;
        
        private void Start()
        {
            if (raycastManager == null)
                raycastManager = FindObjectOfType<ARRaycastManager>();
                
            if (planeManager == null)
                planeManager = FindObjectOfType<ARPlaneManager>();
                
            if (placementIndicator)
                placementIndicator.SetActive(false);
                
            if (placementUI)
                placementUI.SetActive(true);
                
            SetPlacementInstructions("Scan your surroundings to detect surfaces");
            
            autoPlaceTimer = autoPlaceDelay;
        }
        
        private void Update()
        {
            if (isPlaced)
                return;
                
            // Check if we have detected planes
            if (planeManager.trackables.count > 0)
            {
                SetPlacementInstructions("Tap on a surface to place your avatar");
                
                // Handle auto-placement
                if (autoPlace)
                {
                    autoPlaceTimer -= Time.deltaTime;
                    if (autoPlaceTimer <= 0)
                    {
                        TryAutoPlaceAvatar();
                        autoPlaceTimer = autoPlaceDelay;
                    }
                }
                
                // Update placement indicator
                UpdatePlacementPose();
                
                // Handle tap to place
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    if (!IsPointerOverUI())
                    {
                        PlaceAvatar();
                    }
                }
                
                // For editor testing with mouse
                if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
                {
                    PlaceAvatar();
                }
            }
        }
        
        private void UpdatePlacementPose()
        {
            // Raycast from center of screen
            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
            
            if (raycastManager.Raycast(screenCenter, raycastHits, TrackableType.PlaneWithinPolygon))
            {
                // Get the closest hit (first in list)
                var hitPose = raycastHits[0].pose;
                
                // Adjust height if needed
                hitPose.position = new Vector3(hitPose.position.x, hitPose.position.y + placementHeight, hitPose.position.z);
                
                // Update placement indicator
                if (placementIndicator)
                {
                    placementIndicator.SetActive(true);
                    placementIndicator.transform.position = hitPose.position;
                }
            }
            else
            {
                if (placementIndicator)
                {
                    placementIndicator.SetActive(false);
                }
            }
        }
        
        private void PlaceAvatar()
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
            
            if (raycastManager.Raycast(screenCenter, raycastHits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = raycastHits[0].pose;
                hitPose.position = new Vector3(hitPose.position.x, hitPose.position.y + placementHeight, hitPose.position.z);
                
                if (spawnedAvatar == null)
                {
                    spawnedAvatar = Instantiate(avatarPrefab, hitPose.position, hitPose.rotation);
                }
                else
                {
                    spawnedAvatar.transform.position = hitPose.position;
                    spawnedAvatar.transform.rotation = hitPose.rotation;
                    spawnedAvatar.SetActive(true);
                }
                
                isPlaced = true;
                
                // Hide placement UI and indicator
                if (placementIndicator)
                    placementIndicator.SetActive(false);
                    
                if (placementUI)
                    placementUI.SetActive(false);
                    
                // Optionally disable plane visualization after placement
                if (planeManager)
                {
                    foreach (var plane in planeManager.trackables)
                    {
                        plane.gameObject.SetActive(false);
                    }
                    planeManager.enabled = false;
                }
            }
        }
        
        private void TryAutoPlaceAvatar()
        {
            // Only auto-place if we have plane detections
            if (planeManager.trackables.count > 0)
            {
                // Find a suitable horizontal plane
                ARPlane bestPlane = null;
                float largestArea = 0;
                
                foreach (ARPlane plane in planeManager.trackables)
                {
                    // Only use horizontal planes that are facing up
                    if (plane.alignment == PlaneAlignment.HorizontalUp)
                    {
                        float area = plane.size.x * plane.size.y;
                        if (area > largestArea)
                        {
                            largestArea = area;
                            bestPlane = plane;
                        }
                    }
                }
                
                if (bestPlane != null && largestArea > 1.0f) // Minimum size check
                {
                    // Get the center of the plane
                    Vector3 planeCenter = bestPlane.center;
                    Pose placementPose = new Pose(
                        new Vector3(planeCenter.x, planeCenter.y + placementHeight, planeCenter.z),
                        Quaternion.identity);
                        
                    if (spawnedAvatar == null)
                    {
                        spawnedAvatar = Instantiate(avatarPrefab, placementPose.position, placementPose.rotation);
                    }
                    else
                    {
                        spawnedAvatar.transform.position = placementPose.position;
                        spawnedAvatar.transform.rotation = placementPose.rotation;
                        spawnedAvatar.SetActive(true);
                    }
                    
                    isPlaced = true;
                    
                    // Hide placement UI and indicator
                    if (placementIndicator)
                        placementIndicator.SetActive(false);
                        
                    if (placementUI)
                        placementUI.SetActive(false);
                        
                    // Optionally disable plane visualization after placement
                    if (planeManager)
                    {
                        foreach (var plane in planeManager.trackables)
                        {
                            plane.gameObject.SetActive(false);
                        }
                        planeManager.enabled = false;
                    }
                }
            }
        }
        
        public void ResetPlacement()
        {
            isPlaced = false;
            
            if (spawnedAvatar)
            {
                spawnedAvatar.SetActive(false);
            }
            
            // Re-enable plane manager and visualization
            if (planeManager)
            {
                planeManager.enabled = true;
                foreach (var plane in planeManager.trackables)
                {
                    plane.gameObject.SetActive(true);
                }
            }
            
            // Show placement UI
            if (placementUI)
                placementUI.SetActive(true);
                
            SetPlacementInstructions("Tap on a surface to place your avatar");
        }
        
        private bool IsPointerOverUI()
        {
            if (EventSystem.current == null)
                return false;
                
            return EventSystem.current.IsPointerOverGameObject();
        }
        
        private void SetPlacementInstructions(string message)
        {
            if (placementInstructionsText != null)
            {
                placementInstructionsText.text = message;
            }
        }
        
        public GameObject GetPlacedAvatar()
        {
            return spawnedAvatar;
        }
        
        public bool IsAvatarPlaced()
        {
            return isPlaced;
        }
    }
} 