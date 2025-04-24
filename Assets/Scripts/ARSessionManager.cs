using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

namespace TequilaSunrise.AR
{
    [RequireComponent(typeof(ARSession))]
    [RequireComponent(typeof(ARPlaneManager))]
    public class ARSessionManager : MonoBehaviour
    {
        [Header("AR Components")]
        [SerializeField]
        private ARSession arSession;
        
        [SerializeField]
        private ARPlaneManager planeManager;

        [Header("Visualization")]
        [SerializeField]
        private GameObject planePrefab;

        [SerializeField]
        private bool showPlanes = true;

        private readonly Dictionary<TrackableId, ARPlane> detectedPlanes = new Dictionary<TrackableId, ARPlane>();

        private void Awake()
        {
            if (arSession == null)
                arSession = GetComponent<ARSession>();
                
            if (planeManager == null)
                planeManager = GetComponent<ARPlaneManager>();

            // Ensure we have the required components
            Debug.Assert(arSession != null, "AR Session component is required");
            Debug.Assert(planeManager != null, "AR Plane Manager component is required");
            Debug.Assert(planePrefab != null, "Plane prefab is required");
        }

        private void OnEnable()
        {
            planeManager.planesChanged += OnPlanesChanged;
            EnablePlaneDetection(showPlanes);
        }

        private void OnDisable()
        {
            planeManager.planesChanged -= OnPlanesChanged;
            EnablePlaneDetection(false);
        }

        private void Start()
        {
            ConfigureARSession();
            ConfigurePlaneDetection();
        }

        public void TogglePlaneVisualization(bool enable)
        {
            showPlanes = enable;
            foreach (var plane in detectedPlanes.Values)
            {
                if (plane != null)
                {
                    var visualizer = plane.GetComponent<ARPlaneController>();
                    if (visualizer != null)
                    {
                        visualizer.gameObject.SetActive(enable);
                    }
                }
            }
        }

        private void ConfigureARSession()
        {
            if (ARSession.state == ARSessionState.None)
            {
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
        }

        private void ConfigurePlaneDetection()
        {
            if (planeManager != null)
            {
                planeManager.enabled = true;
                planeManager.requestedDetectionMode = PlaneDetectionMode.Horizontal | PlaneDetectionMode.Vertical;
                
                if (planePrefab != null)
                {
                    planeManager.planePrefab = planePrefab;
                    Debug.Log("Plane detection configured with visualization");
                }
                else
                {
                    Debug.LogWarning("No plane prefab assigned for visualization");
                }
            }
            else
            {
                Debug.LogError("ARPlaneManager not found");
            }
        }

        private void EnablePlaneDetection(bool enable)
        {
            if (planeManager != null)
            {
                planeManager.enabled = enable;
                Debug.Log($"Plane detection {(enable ? "enabled" : "disabled")}");
            }
        }

        private void OnPlanesChanged(ARPlanesChangedEventArgs args)
        {
            // Handle added planes
            foreach (ARPlane plane in args.added)
            {
                detectedPlanes[plane.trackableId] = plane;
                Debug.Log($"New plane detected: {plane.trackableId} - Classification: {plane.classification}");
            }

            // Handle updated planes
            foreach (ARPlane plane in args.updated)
            {
                detectedPlanes[plane.trackableId] = plane;
                Debug.Log($"Plane updated: {plane.trackableId} - Size: {plane.size}");
            }

            // Handle removed planes
            foreach (ARPlane plane in args.removed)
            {
                if (detectedPlanes.ContainsKey(plane.trackableId))
                {
                    detectedPlanes.Remove(plane.trackableId);
                    Debug.Log($"Plane removed: {plane.trackableId}");
                }
            }
        }

        public List<ARPlane> GetDetectedPlanes()
        {
            return new List<ARPlane>(detectedPlanes.Values);
        }

        public ARPlane GetNearestPlane(Vector3 position)
        {
            ARPlane nearestPlane = null;
            float nearestDistance = float.MaxValue;

            foreach (var plane in detectedPlanes.Values)
            {
                if (plane == null || !plane.gameObject.activeSelf) continue;

                float distance = Vector3.Distance(position, plane.center);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPlane = plane;
                }
            }

            return nearestPlane;
        }
    }
}