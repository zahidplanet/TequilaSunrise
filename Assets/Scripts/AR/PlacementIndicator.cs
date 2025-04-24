using UnityEngine;

namespace TequilaSunrise.AR
{
    /// <summary>
    /// Visual indicator for AR placement
    /// </summary>
    public class PlacementIndicator : MonoBehaviour
    {
        [Header("Visual Settings")]
        [SerializeField] private Color validColor = Color.green;
        [SerializeField] private Color invalidColor = Color.red;
        [SerializeField] private float pulseSpeed = 1.0f;
        [SerializeField] private float pulseMinScale = 0.95f;
        [SerializeField] private float pulseMaxScale = 1.05f;
        [SerializeField] private float rotationSpeed = 50.0f;
        
        [Header("Renderer References")]
        [SerializeField] private Renderer indicatorRenderer;
        
        private Material indicatorMaterial;
        private bool isValidPlacement = true;
        private float pulseTimer = 0.0f;
        
        private void Start()
        {
            // Get renderer if not assigned
            if (indicatorRenderer == null)
                indicatorRenderer = GetComponentInChildren<Renderer>();
                
            // Create instance of the material to modify at runtime
            if (indicatorRenderer != null && indicatorRenderer.material != null)
            {
                indicatorMaterial = new Material(indicatorRenderer.material);
                indicatorRenderer.material = indicatorMaterial;
                SetValidPlacement(true);
            }
        }
        
        private void Update()
        {
            // Rotate the indicator
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            
            // Pulse the scale
            pulseTimer += Time.deltaTime * pulseSpeed;
            float scaleFactor = Mathf.Lerp(pulseMinScale, pulseMaxScale, (Mathf.Sin(pulseTimer) + 1) * 0.5f);
            transform.localScale = Vector3.one * scaleFactor;
        }
        
        /// <summary>
        /// Set whether the current placement is valid
        /// </summary>
        public void SetValidPlacement(bool isValid)
        {
            isValidPlacement = isValid;
            
            if (indicatorMaterial != null)
            {
                // Set color based on validity
                Color color = isValid ? validColor : invalidColor;
                
                // Try to set color via properties common in shader materials
                if (indicatorMaterial.HasProperty("_Color"))
                {
                    indicatorMaterial.SetColor("_Color", color);
                }
                else if (indicatorMaterial.HasProperty("_BaseColor"))
                {
                    indicatorMaterial.SetColor("_BaseColor", color);
                }
                else if (indicatorMaterial.HasProperty("_EmissionColor"))
                {
                    indicatorMaterial.SetColor("_EmissionColor", color);
                }
            }
        }
        
        /// <summary>
        /// Check if current placement is valid
        /// </summary>
        public bool IsValidPlacement()
        {
            return isValidPlacement;
        }
    }
} 