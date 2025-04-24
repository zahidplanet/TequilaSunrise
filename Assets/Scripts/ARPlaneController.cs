using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace TequilaSunrise.AR
{
    [RequireComponent(typeof(ARPlane))]
    [RequireComponent(typeof(MeshRenderer))]
    public class ARPlaneController : MonoBehaviour
    {
        private ARPlane arPlane;
        private MeshRenderer meshRenderer;
        private Material planeMaterial;

        [SerializeField]
        private float fadeSpeed = 2.0f;
        
        [SerializeField]
        private Color planeColor = new Color(0f, 0.7f, 1f, 0.5f);

        private void Awake()
        {
            arPlane = GetComponent<ARPlane>();
            meshRenderer = GetComponent<MeshRenderer>();
            planeMaterial = new Material(meshRenderer.sharedMaterial);
            meshRenderer.material = planeMaterial;
        }

        private void OnEnable()
        {
            arPlane.boundaryChanged += OnPlaneBoundaryChanged;
        }

        private void OnDisable()
        {
            arPlane.boundaryChanged -= OnPlaneBoundaryChanged;
        }

        private void Start()
        {
            UpdatePlaneVisualization();
        }

        private void OnPlaneBoundaryChanged(ARPlaneBoundaryChangedEventArgs args)
        {
            UpdatePlaneVisualization();
        }

        private void UpdatePlaneVisualization()
        {
            if (arPlane.subsumed)
            {
                // Fade out if the plane is subsumed by another
                StartCoroutine(FadeOut());
                return;
            }

            // Update material color based on plane alignment
            Color currentColor = planeColor;
            switch (arPlane.alignment)
            {
                case PlaneAlignment.HorizontalUp:
                    currentColor = planeColor;
                    break;
                case PlaneAlignment.HorizontalDown:
                    currentColor = new Color(planeColor.r * 0.8f, planeColor.g * 0.8f, planeColor.b * 0.8f, planeColor.a);
                    break;
                case PlaneAlignment.Vertical:
                    currentColor = new Color(planeColor.r * 0.6f, planeColor.g * 0.6f, planeColor.b * 0.6f, planeColor.a);
                    break;
            }

            planeMaterial.color = currentColor;

            // Update mesh to match plane boundaries
            var mesh = new Mesh();
            arPlane.boundary.ToArray(); // Get boundary points
            // Update mesh vertices and UVs based on boundary points
            // This will be handled by AR Foundation's plane subsystem
        }

        private System.Collections.IEnumerator FadeOut()
        {
            Color color = planeMaterial.color;
            while (color.a > 0)
            {
                color.a -= Time.deltaTime * fadeSpeed;
                planeMaterial.color = color;
                yield return null;
            }

            gameObject.SetActive(false);
        }
    }
}