using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace TequilaSunrise.AR
{
    public class ARSessionManager : MonoBehaviour
    {
        [SerializeField]
        private ARSession arSession;
        
        [SerializeField]
        private ARPlaneManager planeManager;
        
        [SerializeField]
        private GameObject planePrefab;

        private void Awake()
        {
            if (arSession == null)
                arSession = FindObjectOfType<ARSession>();
                
            if (planeManager == null)
                planeManager = FindObjectOfType<ARPlaneManager>();
        }

        private void OnEnable()
        {
            planeManager.planesChanged += OnPlanesChanged;
        }

        private void OnDisable()
        {
            planeManager.planesChanged -= OnPlanesChanged;
        }

        private void Start()
        {
            // Configure AR session
            ConfigureARSession();
            
            // Configure plane detection
            ConfigurePlaneDetection();
        }

        private void ConfigureARSession()
        {
            if (!ARSession.state.Equals(ARSessionState.None))
            {
                Debug.Log("AR Session already initialized");
                return;
            }

            try
            {
                arSession.enabled = true;
                Debug.Log("AR Session initialized successfully");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to initialize AR Session: {e.Message}");
            }
        }

        private void ConfigurePlaneDetection()
        {
            if (planeManager != null)
            {
                planeManager.enabled = true;
                planeManager.requestedDetectionMode = PlaneDetectionMode.Horizontal;
                
                if (planePrefab != null)
                    planeManager.planePrefab = planePrefab;
                    
                Debug.Log("Plane detection configured successfully");
            }
            else
            {
                Debug.LogError("ARPlaneManager not found");
            }
        }

        private void OnPlanesChanged(ARPlanesChangedEventArgs args)
        {
            foreach (ARPlane plane in args.added)
            {
                Debug.Log($"New plane detected: {plane.trackableId}");
            }

            foreach (ARPlane plane in args.updated)
            {
                Debug.Log($"Plane updated: {plane.trackableId}");
            }

            foreach (ARPlane plane in args.removed)
            {
                Debug.Log($"Plane removed: {plane.trackableId}");
            }
        }
    }
}