using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace TequilaSunrise.AR
{
    [RequireComponent(typeof(ARPlane))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class ARPlaneVisualizer : MonoBehaviour
    {
        [Header("Visualization")]
        [SerializeField] private Material planeMaterial;
        [SerializeField] private float gridSize = 0.1f;
        [SerializeField] private Color planeColor = new Color(1f, 1f, 1f, 0.5f);
        
        private ARPlane plane;
        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;
        private Material materialInstance;
        
        private void Awake()
        {
            plane = GetComponent<ARPlane>();
            meshRenderer = GetComponent<MeshRenderer>();
            meshFilter = GetComponent<MeshFilter>();
            
            SetupMaterial();
        }
        
        private void OnEnable()
        {
            plane.boundaryChanged += OnPlaneBoundaryChanged;
        }
        
        private void OnDisable()
        {
            plane.boundaryChanged -= OnPlaneBoundaryChanged;
        }
        
        private void SetupMaterial()
        {
            if (planeMaterial != null)
            {
                materialInstance = new Material(planeMaterial);
                materialInstance.color = planeColor;
                
                if (materialInstance.HasProperty("_GridSize"))
                {
                    materialInstance.SetFloat("_GridSize", gridSize);
                }
                
                meshRenderer.material = materialInstance;
            }
        }
        
        private void OnPlaneBoundaryChanged(ARPlaneBoundaryChangedEventArgs eventArgs)
        {
            if (!plane.subsumed && plane.trackingState == TrackingState.Tracking)
            {
                UpdatePlaneMesh();
                UpdatePlaneVisibility();
            }
        }
        
        private void UpdatePlaneMesh()
        {
            meshFilter.mesh = plane.mesh;
            
            // Update material properties based on plane alignment
            if (materialInstance != null)
            {
                if (plane.alignment == PlaneAlignment.HorizontalUp)
                {
                    materialInstance.SetFloat("_VerticalAlignment", 0f);
                }
                else if (plane.alignment == PlaneAlignment.HorizontalDown)
                {
                    materialInstance.SetFloat("_VerticalAlignment", 180f);
                }
                else
                {
                    materialInstance.SetFloat("_VerticalAlignment", 90f);
                }
            }
        }
        
        private void UpdatePlaneVisibility()
        {
            var planeVisible = plane.trackingState == TrackingState.Tracking && 
                             !plane.subsumed;
                             
            meshRenderer.enabled = planeVisible;
        }
        
        public void SetColor(Color color)
        {
            if (materialInstance != null)
            {
                planeColor = color;
                materialInstance.color = color;
            }
        }
        
        public void SetGridSize(float size)
        {
            if (materialInstance != null && materialInstance.HasProperty("_GridSize"))
            {
                gridSize = size;
                materialInstance.SetFloat("_GridSize", size);
            }
        }
        
        private void OnDestroy()
        {
            if (materialInstance != null)
            {
                Destroy(materialInstance);
            }
        }
    }
}