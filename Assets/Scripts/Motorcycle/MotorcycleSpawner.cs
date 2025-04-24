using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;

namespace TequilaSunrise.Motorcycle
{
    /// <summary>
    /// Responsible for spawning motorcycles on AR planes
    /// </summary>
    public class MotorcycleSpawner : MonoBehaviour
    {
        [Header("AR References")]
        [SerializeField] private ARRaycastManager raycastManager;
        [SerializeField] private ARPlaneManager planeManager;
        
        [Header("Spawn Settings")]
        [SerializeField] private GameObject motorcyclePrefab;
        [SerializeField] private float minDistanceFromCamera = 1.0f;
        [SerializeField] private float maxDistanceFromCamera = 5.0f;
        [SerializeField] private float spawnHeight = 0.1f;
        
        [Header("UI")]
        [SerializeField] private Button spawnButton;
        [SerializeField] private GameObject placementIndicator;
        
        private GameObject _spawnedMotorcycle;
        private Camera _arCamera;
        private bool _isPlacing = false;
        private Pose _placementPose;
        private bool _poseIsValid = false;
        
        public GameObject GetSpawnedMotorcycle()
        {
            return _spawnedMotorcycle;
        }
        
        private void Awake()
        {
            _arCamera = Camera.main;
        }
        
        private void Start()
        {
            if (spawnButton != null)
            {
                spawnButton.onClick.AddListener(TogglePlacementMode);
            }
            
            if (placementIndicator != null)
            {
                placementIndicator.SetActive(false);
            }
        }
        
        private void Update()
        {
            if (_isPlacing)
            {
                UpdatePlacementPose();
                UpdatePlacementIndicator();
                
                // Check for tap to place
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    if (_poseIsValid)
                    {
                        PlaceMotorcycle();
                    }
                }
            }
        }
        
        private void UpdatePlacementPose()
        {
            // Use the AR raycast to find a valid placement position
            var screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
            var hits = new System.Collections.Generic.List<ARRaycastHit>();
            
            if (raycastManager.Raycast(screenCenter, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                _poseIsValid = true;
                _placementPose = hits[0].pose;
                
                // Adjust height to be slightly above the plane
                var position = _placementPose.position;
                position.y += spawnHeight;
                _placementPose.position = position;
                
                // Ensure the motorcycle faces the user
                var cameraForward = _arCamera.transform.forward;
                var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
                _placementPose.rotation = Quaternion.LookRotation(cameraBearing);
            }
            else
            {
                _poseIsValid = false;
            }
        }
        
        private void UpdatePlacementIndicator()
        {
            if (placementIndicator != null)
            {
                if (_poseIsValid)
                {
                    placementIndicator.SetActive(true);
                    placementIndicator.transform.SetPositionAndRotation(_placementPose.position, _placementPose.rotation);
                }
                else
                {
                    placementIndicator.SetActive(false);
                }
            }
        }
        
        private void PlaceMotorcycle()
        {
            // If there's already a motorcycle, destroy it
            if (_spawnedMotorcycle != null)
            {
                Destroy(_spawnedMotorcycle);
            }
            
            // Instantiate the motorcycle at the placement pose
            _spawnedMotorcycle = Instantiate(motorcyclePrefab, _placementPose.position, _placementPose.rotation);
            
            // Exit placement mode
            _isPlacing = false;
            
            if (placementIndicator != null)
            {
                placementIndicator.SetActive(false);
            }
        }
        
        /// <summary>
        /// Toggle motorcycle placement mode on/off
        /// </summary>
        public void TogglePlacementMode()
        {
            _isPlacing = !_isPlacing;
            
            if (placementIndicator != null)
            {
                placementIndicator.SetActive(_isPlacing);
            }
        }
        
        /// <summary>
        /// Spawns a motorcycle at the given position
        /// </summary>
        public GameObject SpawnMotorcycleAt(Vector3 position, Quaternion rotation)
        {
            if (_spawnedMotorcycle != null)
            {
                Destroy(_spawnedMotorcycle);
            }
            
            _spawnedMotorcycle = Instantiate(motorcyclePrefab, position, rotation);
            return _spawnedMotorcycle;
        }
    }
} 