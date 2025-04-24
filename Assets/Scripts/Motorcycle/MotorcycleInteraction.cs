using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TequilaSunrise.Avatar;

namespace TequilaSunrise.Motorcycle
{
    public class MotorcycleInteraction : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AvatarController avatarController;
        [SerializeField] private MotorcycleSpawner motorcycleSpawner;
        [SerializeField] private Button interactButton;
        [SerializeField] private Text interactButtonText;
        
        [Header("Interaction Settings")]
        [SerializeField] private float interactionCheckInterval = 0.5f;
        
        private MotorcycleController currentMotorcycle;
        private bool isRiding = false;
        private float interactionTimer;
        
        private void Start()
        {
            if (interactButton != null)
            {
                interactButton.onClick.AddListener(HandleInteraction);
                interactButton.gameObject.SetActive(false);
            }
        }
        
        private void Update()
        {
            interactionTimer += Time.deltaTime;
            
            if (interactionTimer >= interactionCheckInterval)
            {
                CheckForNearbyMotorcycle();
                interactionTimer = 0f;
            }
        }
        
        private void CheckForNearbyMotorcycle()
        {
            // Get the current motorcycle from the spawner
            GameObject motorcycleObj = motorcycleSpawner.GetSpawnedMotorcycle();
            
            if (motorcycleObj == null)
                return;
                
            MotorcycleController motorcycle = motorcycleObj.GetComponent<MotorcycleController>();
            
            if (motorcycle == null)
                return;
                
            // Check if we're close enough to interact
            bool canInteract = motorcycle.CanInteractWith(avatarController.transform);
            
            if (canInteract)
            {
                // Show interaction button
                if (interactButton != null)
                {
                    interactButton.gameObject.SetActive(true);
                    
                    // Update text based on whether we're currently riding
                    if (isRiding)
                    {
                        interactButtonText.text = "Dismount";
                    }
                    else
                    {
                        interactButtonText.text = "Ride Motorcycle";
                    }
                }
                
                currentMotorcycle = motorcycle;
            }
            else
            {
                // Hide interaction button
                if (interactButton != null)
                {
                    interactButton.gameObject.SetActive(false);
                }
                
                currentMotorcycle = null;
            }
        }
        
        public void HandleInteraction()
        {
            if (currentMotorcycle == null)
                return;
                
            if (isRiding)
            {
                // Dismount
                currentMotorcycle.DismountRider();
                isRiding = false;
                
                // Update button text
                if (interactButtonText != null)
                {
                    interactButtonText.text = "Ride Motorcycle";
                }
            }
            else
            {
                // Mount
                currentMotorcycle.MountRider(avatarController);
                isRiding = true;
                
                // Update button text
                if (interactButtonText != null)
                {
                    interactButtonText.text = "Dismount";
                }
            }
        }
    }
    
    public class MotorcycleController : MonoBehaviour
    {
        // This is a stub class - actual implementation will be in a separate file
        public bool CanInteractWith(Transform target) 
        {
            // Implementation depends on the actual MotorcycleController
            return true;
        }
        
        public void MountRider(AvatarController avatarController)
        {
            // Implementation depends on the actual MotorcycleController
        }
        
        public void DismountRider()
        {
            // Implementation depends on the actual MotorcycleController
        }
    }
    
    public class MotorcycleSpawner : MonoBehaviour
    {
        // This is a stub class - actual implementation will be in a separate file
        public GameObject GetSpawnedMotorcycle()
        {
            // Implementation depends on the actual MotorcycleSpawner
            return null;
        }
    }
} 