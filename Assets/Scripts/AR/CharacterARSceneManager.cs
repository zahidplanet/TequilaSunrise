using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TequilaSunrise.Avatar;
using TequilaSunrise.Motorcycle;
using TequilaSunrise.UI;

namespace TequilaSunrise.AR
{
    /// <summary>
    /// Manages the AR scene with character and motorcycle interactions
    /// </summary>
    public class CharacterARSceneManager : MonoBehaviour
    {
        [Header("AR Components")]
        [SerializeField] private ARSession arSession;
        [SerializeField] private ARPlaneManager planeManager;
        [SerializeField] private ARRaycastManager raycastManager;
        
        [Header("Character")]
        [SerializeField] private GameObject characterPrefab;
        [SerializeField] private float characterPlacementHeight = 0.05f;
        [SerializeField] private float spawnDistanceFromCamera = 1.5f;
        
        [Header("Motorcycle")]
        [SerializeField] private MotorcycleSpawner motorcycleSpawner;
        
        [Header("UI")]
        [SerializeField] private MobileInputController inputController;
        [SerializeField] private GameObject instructionsPanel;
        [SerializeField] private GameObject placementIndicator;
        
        [Header("Session Settings")]
        [SerializeField] private float minPlaneAreaToStart = 0.25f; // Square meters
        [SerializeField] private int planesRequiredToStart = 1;
        
        private GameObject _spawnedCharacter;
        private AvatarController _avatarController;
        private Camera _arCamera;
        private bool _hasSpawnedCharacter = false;
        private bool _isPlacingCharacter = false;
        private bool _sceneInitialized = false;
        private List<ARRaycastHit> _raycastHits = new List<ARRaycastHit>();
        private Pose _placementPose = new Pose();
        private bool _isPoseValid = false;
        
        private enum SceneState
        {
            Initializing,
            WaitingForPlanes,
            PlacingCharacter,
            Ready
        }
        
        private SceneState _currentState = SceneState.Initializing;
        
        private void Awake()
        {
            _arCamera = Camera.main;
        }
        
        private void Start()
        {
            if (instructionsPanel != null)
            {
                instructionsPanel.SetActive(true);
            }
            
            if (placementIndicator != null)
            {
                placementIndicator.SetActive(false);
            }
            
            StartCoroutine(InitializeARScene());
        }
        
        private void Update()
        {
            switch (_currentState)
            {
                case SceneState.WaitingForPlanes:
                    CheckForPlanes();
                    break;
                    
                case SceneState.PlacingCharacter:
                    UpdatePlacementPose();
                    UpdatePlacementIndicator();
                    
                    // Check for touch input to place character
                    if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && _isPoseValid)
                    {
                        PlaceCharacter();
                        _currentState = SceneState.Ready;
                    }
                    break;
                    
                case SceneState.Ready:
                    // Scene is ready for interaction
                    break;
            }
        }
        
        private IEnumerator InitializeARScene()
        {
            // Wait for AR session to initialize
            yield return new WaitForSeconds(0.5f);
            
            _currentState = SceneState.WaitingForPlanes;
            
            if (instructionsPanel != null)
            {
                instructionsPanel.SetActive(true);
            }
        }
        
        private void CheckForPlanes()
        {
            // Check if enough planes have been detected
            int planeCount = 0;
            bool hasSuitablePlane = false;
            
            foreach (ARPlane plane in planeManager.trackables)
            {
                planeCount++;
                
                // Check if plane is horizontal and has sufficient area
                if (plane.alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.HorizontalUp && 
                    plane.size.x * plane.size.y >= minPlaneAreaToStart)
                {
                    hasSuitablePlane = true;
                }
            }
            
            if (planeCount >= planesRequiredToStart && hasSuitablePlane)
            {
                // Found suitable planes for placement
                _currentState = SceneState.PlacingCharacter;
                
                if (instructionsPanel != null)
                {
                    instructionsPanel.SetActive(false);
                }
                
                if (placementIndicator != null)
                {
                    placementIndicator.SetActive(true);
                }
            }
        }
        
        private void UpdatePlacementPose()
        {
            // Use screen center for placement
            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
            
            // Raycast against planes
            _isPoseValid = raycastManager.Raycast(screenCenter, _raycastHits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon);
            
            if (_isPoseValid)
            {
                _placementPose = _raycastHits[0].pose;
                
                // Adjust height to prevent sinking
                Vector3 position = _placementPose.position;
                position.y += characterPlacementHeight;
                _placementPose.position = position;
                
                // Make character face the camera
                Vector3 cameraForward = _arCamera.transform.forward;
                Vector3 cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
                _placementPose.rotation = Quaternion.LookRotation(cameraBearing);
            }
        }
        
        private void UpdatePlacementIndicator()
        {
            if (placementIndicator != null)
            {
                if (_isPoseValid)
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
        
        private void PlaceCharacter()
        {
            if (characterPrefab == null)
            {
                Debug.LogError("Character prefab not assigned!");
                return;
            }
            
            // If character already exists, destroy it
            if (_spawnedCharacter != null)
            {
                Destroy(_spawnedCharacter);
            }
            
            // Create character
            _spawnedCharacter = Instantiate(characterPrefab, _placementPose.position, _placementPose.rotation);
            _avatarController = _spawnedCharacter.GetComponent<AvatarController>();
            
            // Hide placement indicator
            if (placementIndicator != null)
            {
                placementIndicator.SetActive(false);
            }
            
            _hasSpawnedCharacter = true;
            
            // Connect input controller to avatar if present
            if (_avatarController != null && inputController != null)
            {
                ConnectInputToAvatar();
            }
            
            // Enable motorcycle spawning
            if (motorcycleSpawner != null)
            {
                motorcycleSpawner.enabled = true;
            }
            
            _sceneInitialized = true;
        }
        
        private void ConnectInputToAvatar()
        {
            if (_avatarController == null || inputController == null)
                return;

            // Set joystick references
            if (_avatarController.joystick == null)
            {
                _avatarController.joystick = inputController.Joystick;
            }
            
            // Set up button listeners
            ActionButton[] buttons = inputController.GetComponentsInChildren<ActionButton>();
            if (buttons != null)
            {
                foreach (var button in buttons)
                {
                    if (button == null) continue;
                    
                    string buttonId = button.ButtonId;
                    if (string.IsNullOrEmpty(buttonId)) continue;

                    if (buttonId.Equals("jump", System.StringComparison.OrdinalIgnoreCase))
                    {
                        button.OnPress.AddListener(_avatarController.Jump);
                    }
                    else if (buttonId.Equals("sprint", System.StringComparison.OrdinalIgnoreCase))
                    {
                        button.OnPress.AddListener(() => _avatarController.ToggleSprint(true));
                        button.OnRelease.AddListener(() => _avatarController.ToggleSprint(false));
                    }
                }
            }
        }
        
        /// <summary>
        /// Reset the entire AR scene
        /// </summary>
        public void ResetARScene()
        {
            // Destroy spawned objects
            if (_spawnedCharacter != null)
            {
                Destroy(_spawnedCharacter);
                _spawnedCharacter = null;
                _avatarController = null;
                _hasSpawnedCharacter = false;
            }
            
            // Reset AR session
            if (arSession != null)
            {
                arSession.Reset();
            }
            
            // Reset state
            _currentState = SceneState.Initializing;
            _sceneInitialized = false;
            
            // Restart initialization
            StartCoroutine(InitializeARScene());
        }
        
        /// <summary>
        /// Get the spawned character controller
        /// </summary>
        public AvatarController GetAvatarController()
        {
            return _avatarController;
        }
    }
} 