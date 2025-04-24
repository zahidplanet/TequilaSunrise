using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class ARSceneController : MonoBehaviour
{
    [Header("AR Components")]
    [SerializeField] private ARSession arSession;
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private ARRaycastManager raycastManager;
    
    [Header("Player")]
    [SerializeField] private GameObject avatarPrefab;
    [SerializeField] private float avatarScale = 0.5f;
    [SerializeField] private Button placeAvatarButton;
    
    [Header("UI")]
    [SerializeField] private GameObject instructionsPanel;
    [SerializeField] private Text instructionsText;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject debugPanel;
    [SerializeField] private Toggle showPlanesToggle;
    
    private GameObject spawnedAvatar;
    private List<ARRaycastHit> raycastHits = new List<ARRaycastHit>();
    private bool isAvatarPlaced = false;
    
    private void Start()
    {
        // Initialize UI
        if (placeAvatarButton != null)
            placeAvatarButton.onClick.AddListener(PlaceAvatarAtCamera);
            
        if (showPlanesToggle != null)
            showPlanesToggle.onValueChanged.AddListener(TogglePlaneVisualization);
            
        // Show initial instructions
        ShowInstructions("Scan your environment. Look around to detect surfaces.");
        
        // Hide controls initially
        if (controlsPanel != null)
            controlsPanel.SetActive(false);
    }
    
    private void Update()
    {
        // Update instructions based on plane detection
        if (!isAvatarPlaced && planeManager.trackables.count > 0)
        {
            ShowInstructions("Surfaces detected. Tap the 'Place Avatar' button to start.");
            if (placeAvatarButton != null)
                placeAvatarButton.gameObject.SetActive(true);
        }
    }
    
    private void ShowInstructions(string message)
    {
        if (instructionsPanel != null && instructionsText != null)
        {
            instructionsPanel.SetActive(true);
            instructionsText.text = message;
        }
    }
    
    private void HideInstructions()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);
    }
    
    public void PlaceAvatarAtCamera()
    {
        if (avatarPrefab == null || isAvatarPlaced)
            return;
        
        // Try to raycast to find a valid plane
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        
        if (raycastManager.Raycast(screenCenter, raycastHits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            // Get the first hit pose
            Pose hitPose = raycastHits[0].pose;
            
            // Place avatar at raycast position
            spawnedAvatar = Instantiate(avatarPrefab, hitPose.position, hitPose.rotation);
            spawnedAvatar.transform.localScale = Vector3.one * avatarScale;
            
            isAvatarPlaced = true;
            
            // Hide place button and show controls
            if (placeAvatarButton != null)
                placeAvatarButton.gameObject.SetActive(false);
                
            if (controlsPanel != null)
                controlsPanel.SetActive(true);
                
            // Hide instructions
            HideInstructions();
        }
        else
        {
            // Couldn't find a plane - place at camera position with offset
            Vector3 cameraPosition = Camera.main.transform.position;
            Vector3 cameraForward = Camera.main.transform.forward;
            
            // Place avatar 1 meter in front of the camera
            Vector3 placementPosition = cameraPosition + cameraForward * 1.0f;
            // Keep avatar at the same height as camera
            placementPosition.y = cameraPosition.y - 1.0f; // Adjust based on avatar height
            
            spawnedAvatar = Instantiate(avatarPrefab, placementPosition, Quaternion.identity);
            spawnedAvatar.transform.localScale = Vector3.one * avatarScale;
            
            // Make avatar face away from camera
            spawnedAvatar.transform.rotation = Quaternion.LookRotation(new Vector3(cameraForward.x, 0, cameraForward.z));
            
            isAvatarPlaced = true;
            
            // Hide place button and show controls
            if (placeAvatarButton != null)
                placeAvatarButton.gameObject.SetActive(false);
                
            if (controlsPanel != null)
                controlsPanel.SetActive(true);
                
            // Hide instructions
            HideInstructions();
        }
    }
    
    public void TogglePlaneVisualization(bool showPlanes)
    {
        foreach (ARPlane plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(showPlanes);
        }
    }
    
    public void ToggleDebugPanel()
    {
        if (debugPanel != null)
            debugPanel.SetActive(!debugPanel.activeSelf);
    }
    
    public void ResetARSession()
    {
        // Destroy avatar
        if (spawnedAvatar != null)
        {
            Destroy(spawnedAvatar);
            spawnedAvatar = null;
        }
        
        // Reset AR session
        if (arSession != null)
        {
            arSession.Reset();
        }
        
        // Reset UI state
        isAvatarPlaced = false;
        if (placeAvatarButton != null)
            placeAvatarButton.gameObject.SetActive(false);
            
        if (controlsPanel != null)
            controlsPanel.SetActive(false);
            
        // Show instructions again
        ShowInstructions("Resetting AR session. Scan your environment again.");
    }
} 