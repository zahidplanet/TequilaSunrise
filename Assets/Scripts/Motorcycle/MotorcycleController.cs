using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TequilaSunrise.Avatar;
using TequilaSunrise.UI;

namespace TequilaSunrise.Motorcycle
{
    /// <summary>
    /// Controls motorcycle physics and interaction with the avatar
    /// </summary>
    public class MotorcycleController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform riderSeatTransform;
        [SerializeField] private ParticleSystem exhaustParticles;
        [SerializeField] private AudioSource engineAudioSource;
        
        [Header("Movement Settings")]
        [SerializeField] private float maxSpeed = 10f;
        [SerializeField] private float acceleration = 5f;
        [SerializeField] private float deceleration = 8f;
        [SerializeField] private float turnSpeed = 120f;
        [SerializeField] private float groundCheckDistance = 0.5f;
        [SerializeField] private LayerMask groundLayers;
        
        [Header("Interaction")]
        [SerializeField] private float interactionRadius = 2.0f;
        
        private Rigidbody _rigidbody;
        private MobileInputController _inputController;
        private AvatarController _currentRider;
        private float _currentSpeed = 0f;
        private bool _isGrounded = false;
        private bool _isRiderMounted = false;
        
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            
            // Configure rigidbody for motorcycle physics
            if (_rigidbody != null)
            {
                _rigidbody.centerOfMass = new Vector3(0, -0.5f, 0); // Lower center of mass for stability
                _rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;
            }
        }
        
        private void Start()
        {
            // Find the input controller in the scene if not assigned
            if (_inputController == null)
            {
                _inputController = FindObjectOfType<MobileInputController>();
            }
            
            // Initialize particles and audio
            if (exhaustParticles != null)
            {
                var emission = exhaustParticles.emission;
                emission.enabled = false;
            }
            
            if (engineAudioSource != null)
            {
                engineAudioSource.volume = 0.2f;
                engineAudioSource.pitch = 0.5f;
                engineAudioSource.Play();
            }
        }
        
        private void Update()
        {
            CheckGrounded();
            
            // Only process input if a rider is mounted
            if (_isRiderMounted && _inputController != null)
            {
                HandleMovementInput();
                UpdateEffects();
            }
            else
            {
                // Slow down if no rider
                _currentSpeed = Mathf.Lerp(_currentSpeed, 0, Time.deltaTime * deceleration);
            }
        }
        
        private void FixedUpdate()
        {
            // Apply motorcycle physics
            if (_isGrounded)
            {
                // Move forward based on current speed
                Vector3 moveDirection = transform.forward * _currentSpeed;
                _rigidbody.velocity = new Vector3(moveDirection.x, _rigidbody.velocity.y, moveDirection.z);
            }
        }
        
        private void CheckGrounded()
        {
            RaycastHit hit;
            Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
            _isGrounded = Physics.Raycast(rayOrigin, Vector3.down, out hit, groundCheckDistance, groundLayers);
        }
        
        private void HandleMovementInput()
        {
            // Get input from the mobile input controller
            Vector2 movement = _inputController.MovementInput;
            
            // Accelerate/decelerate based on vertical input
            float targetSpeed = movement.y * maxSpeed;
            _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed,
                (targetSpeed > _currentSpeed ? acceleration : deceleration) * Time.deltaTime);
            
            // Turn based on horizontal input
            if (Mathf.Abs(_currentSpeed) > 0.5f)
            {
                float turnAmount = movement.x * turnSpeed * Time.deltaTime;
                transform.Rotate(0, turnAmount, 0);
            }
        }
        
        private void UpdateEffects()
        {
            if (exhaustParticles != null)
            {
                var emission = exhaustParticles.emission;
                emission.enabled = Mathf.Abs(_currentSpeed) > 1.0f;
                
                // Adjust emission rate based on speed
                var rate = emission.rateOverTime;
                rate.constant = Mathf.Abs(_currentSpeed) * 5f;
            }
            
            if (engineAudioSource != null)
            {
                // Adjust engine sound based on speed
                float targetPitch = 0.5f + (Mathf.Abs(_currentSpeed) / maxSpeed) * 1.0f;
                engineAudioSource.pitch = Mathf.Lerp(engineAudioSource.pitch, targetPitch, Time.deltaTime * 2f);
                
                float targetVolume = 0.2f + (Mathf.Abs(_currentSpeed) / maxSpeed) * 0.5f;
                engineAudioSource.volume = Mathf.Lerp(engineAudioSource.volume, targetVolume, Time.deltaTime * 2f);
            }
        }
        
        /// <summary>
        /// Check if the avatar can interact with this motorcycle
        /// </summary>
        public bool CanInteractWith(Transform avatarTransform)
        {
            if (avatarTransform == null)
                return false;
                
            float distance = Vector3.Distance(transform.position, avatarTransform.position);
            return distance <= interactionRadius;
        }
        
        /// <summary>
        /// Mount the rider on the motorcycle
        /// </summary>
        public void MountRider(AvatarController avatarController)
        {
            if (avatarController == null || _isRiderMounted)
                return;
                
            _currentRider = avatarController;
            _isRiderMounted = true;
            
            // Position the rider on the motorcycle
            if (riderSeatTransform != null)
            {
                avatarController.OnMotorcycleMount(riderSeatTransform);
            }
            else
            {
                avatarController.OnMotorcycleMount(transform);
            }
            
            // Start engine sound
            if (engineAudioSource != null)
            {
                engineAudioSource.volume = 0.4f;
                engineAudioSource.pitch = 0.7f;
            }
        }
        
        /// <summary>
        /// Dismount the rider from the motorcycle
        /// </summary>
        public void DismountRider()
        {
            if (_currentRider == null || !_isRiderMounted)
                return;
                
            // Position the rider next to the motorcycle
            _currentRider.OnMotorcycleDismount();
            
            _currentRider = null;
            _isRiderMounted = false;
            
            // Idle engine sound
            if (engineAudioSource != null)
            {
                engineAudioSource.volume = 0.2f;
                engineAudioSource.pitch = 0.5f;
            }
        }
    }
} 