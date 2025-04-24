using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace TequilaSunrise.AR
{
    [RequireComponent(typeof(ARPlane))]
    [RequireComponent(typeof(MeshRenderer))]
    public class ARPlaneController : MonoBehaviour
    {
        private ARPlane plane;
        private MeshRenderer meshRenderer;

        private void Awake()
        {
            plane = GetComponent<ARPlane>();
            meshRenderer = GetComponent<MeshRenderer>();
        }

        private void OnEnable()
        {
            plane.boundaryChanged += OnPlaneBoundaryChanged;
        }

        private void OnDisable()
        {
            plane.boundaryChanged -= OnPlaneBoundaryChanged;
        }

        private void OnPlaneBoundaryChanged(ARPlaneBoundaryChangedEventArgs eventArgs)
        {
            // Update plane visualization when boundaries change
            UpdatePlaneVisualization();
        }

        private void UpdatePlaneVisualization()
        {
            // You can customize the plane appearance here
            Color planeColor = new Color(1f, 1f, 1f, 0.3f); // Semi-transparent white
            meshRenderer.material.color = planeColor;
        }
    }
}