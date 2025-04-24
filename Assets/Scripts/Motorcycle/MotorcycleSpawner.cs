using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MotorcycleSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ARPlaneManager planeManager;
    [SerializeField] private GameObject motorcyclePrefab;
    
    [Header("Spawn Settings")]
    [SerializeField] private float minPlaneArea = 2.0f; // Minimum area of plane in square meters
    [SerializeField] private float maxSlopeAngle = 5.0f; // Maximum angle in degrees from horizontal
    [SerializeField] private float surfaceOffset = 0.05f; // Offset from surface to prevent clipping
    
    // Current instance
    private GameObject spawnedMotorcycle;
    private bool hasSpawnedMotorcycle = false;
    
    private void OnEnable()
    {
        planeManager.planesChanged += OnPlanesChanged;
    }
    
    private void OnDisable()
    {
        planeManager.planesChanged -= OnPlanesChanged;
    }
    
    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        // If we've already spawned a motorcycle, don't spawn another
        if (hasSpawnedMotorcycle)
            return;
            
        // Check for suitable planes to spawn the motorcycle
        foreach (ARPlane plane in args.added)
        {
            if (IsSuitableForSpawning(plane))
            {
                SpawnMotorcycle(plane);
                break;
            }
        }
        
        // Also check updated planes if we haven't spawned yet
        if (!hasSpawnedMotorcycle)
        {
            foreach (ARPlane plane in args.updated)
            {
                if (IsSuitableForSpawning(plane))
                {
                    SpawnMotorcycle(plane);
                    break;
                }
            }
        }
    }
    
    private bool IsSuitableForSpawning(ARPlane plane)
    {
        // Check if plane is horizontal (floor)
        if (plane.alignment != PlaneAlignment.HorizontalUp)
            return false;
            
        // Check if plane has sufficient area
        if (plane.size.x * plane.size.y < minPlaneArea)
            return false;
            
        // Check the slope/angle of the plane
        Vector3 planeNormal = plane.normal;
        float angle = Vector3.Angle(planeNormal, Vector3.up);
        
        if (angle > maxSlopeAngle)
            return false;
            
        // All checks passed
        return true;
    }
    
    private void SpawnMotorcycle(ARPlane plane)
    {
        if (motorcyclePrefab == null)
        {
            Debug.LogError("Motorcycle prefab is not assigned!");
            return;
        }
        
        // Calculate spawn position
        Vector3 spawnPosition = plane.center;
        spawnPosition.y += surfaceOffset; // Add slight offset to prevent clipping
        
        // Spawn motorcycle
        spawnedMotorcycle = Instantiate(motorcyclePrefab, spawnPosition, Quaternion.identity);
        
        // Orient motorcycle to face along plane's longer axis
        if (plane.size.x > plane.size.y)
        {
            // Try to align the motorcycle with the longer side of the plane
            Vector2 planeDirection = plane.extents.x > plane.extents.y ? new Vector2(1, 0) : new Vector2(0, 1);
            Vector3 forward = new Vector3(planeDirection.x, 0, planeDirection.y);
            spawnedMotorcycle.transform.forward = forward;
        }
        
        hasSpawnedMotorcycle = true;
        
        // Optional: Disable visualization of this plane
        // plane.gameObject.SetActive(false);
    }
    
    public void RespawnMotorcycle()
    {
        // Find a new suitable plane
        if (spawnedMotorcycle != null)
        {
            Destroy(spawnedMotorcycle);
            spawnedMotorcycle = null;
            hasSpawnedMotorcycle = false;
        }
        
        // Check all available planes
        foreach (ARPlane plane in planeManager.trackables)
        {
            if (IsSuitableForSpawning(plane))
            {
                SpawnMotorcycle(plane);
                break;
            }
        }
    }
    
    public GameObject GetSpawnedMotorcycle()
    {
        return spawnedMotorcycle;
    }
} 