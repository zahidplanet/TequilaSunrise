using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;

namespace TequilaSunrise.Avatar
{
    /// <summary>
    /// Controls the avatar movement and interactions
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class AvatarController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private AvatarAnimator animatorController;
        [SerializeField] private Transform modelTransform;
        
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 1.5f;
        [SerializeField] private float runSpeed = 3.0f;
        [SerializeField] private float rotationSpeed = 10.0f;
        [SerializeField] private float jumpForce = 5.0f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float groundedGravity = -1.0f;
        [SerializeField] private float groundCheckDistance = 0.2f;
        
        [Header("Mobile Controls")]
        [SerializeField] private Joystick joystick;
        [SerializeField] private GameObject jumpButton;
        
        // Movement variables
        private Vector3 moveDirection;
        private float verticalVelocity;
        private float currentSpeed;
        private bool isJumping;
        private bool isGrounded;
        private bool isMounted;
        
        // Runtime references
        private Transform currentMount;
        
        private void Awake()
        {
            InitializeComponents();
        }
        
        private void Start()
        {
            SetupEventListeners();
        }
        
        private void InitializeComponents()
        {
            // Get components if not assigned
            if (characterController == null)
                characterController = GetComponent<CharacterController>();
                
            if (animator == null)
                animator = GetComponentInChildren<Animator>();
                
            if (animatorController == null)
                animatorController = GetComponentInChildren<AvatarAnimator>();
                
            if (modelTransform == null && transform.childCount > 0)
                modelTransform = transform.GetChild(0);
        }
        
        private void SetupEventListeners()
        {
            // Setup any event listeners here
            if (jumpButton != null)
            {
                UnityEngine.UI.Button button = jumpButton.GetComponent<UnityEngine.UI.Button>();
                if (button != null)
                {
                    button.onClick.AddListener(Jump);
                }
            }
        }
        
        private void Update()
        {
            CheckGrounded();
            HandleMovement();
            HandleGravity();
            UpdateAnimator();
        }
        
        private void CheckGrounded()
        {
            // Check if grounded using character controller
            isGrounded = characterController.isGrounded;
            
            // Additional ground check using raycast for better detection
            if (!isGrounded)
            {
                Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, groundCheckDistance + 0.1f))
                {
                    isGrounded = true;
                }
            }
            
            // Update animator with grounded state
            if (animatorController != null)
            {
                animatorController.SetGrounded(isGrounded);
            }
        }
        
        private void HandleMovement()
        {
            if (isMounted)
                return;
                
            // Get joystick input
            float horizontal = joystick != null ? joystick.Horizontal : Input.GetAxis("Horizontal");
            float vertical = joystick != null ? joystick.Vertical : Input.GetAxis("Vertical");
            
            // Calculate movement direction
            Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
            
            if (direction.magnitude >= 0.1f)
            {
                // Calculate move amount
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                float currentAngle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, Time.deltaTime * rotationSpeed);
                
                // Rotate to face movement direction
                transform.rotation = Quaternion.Euler(0, currentAngle, 0);
                
                // Apply movement
                moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
                
                // Determine speed based on input magnitude
                currentSpeed = direction.magnitude > 0.7f ? runSpeed : walkSpeed;
                
                characterController.Move(moveDirection.normalized * currentSpeed * Time.deltaTime);
            }
            else
            {
                moveDirection = Vector3.zero;
                currentSpeed = 0;
            }
        }
        
        private void HandleGravity()
        {
            if (isMounted)
                return;
                
            if (isGrounded && verticalVelocity < 0)
            {
                // Apply a small negative gravity when grounded to keep the controller grounded
                verticalVelocity = groundedGravity;
                isJumping = false;
            }
            else
            {
                // Apply gravity when in the air
                verticalVelocity += gravity * Time.deltaTime;
            }
            
            // Move the character vertically
            Vector3 verticalMove = new Vector3(0, verticalVelocity, 0) * Time.deltaTime;
            characterController.Move(verticalMove);
        }
        
        /// <summary>
        /// Make the avatar jump if grounded
        /// </summary>
        public void Jump()
        {
            if (isGrounded && !isJumping && !isMounted)
            {
                verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
                isJumping = true;
                
                // Trigger jump animation
                if (animatorController != null)
                {
                    animatorController.TriggerJump();
                }
            }
        }
        
        private void UpdateAnimator()
        {
            if (animatorController != null)
            {
                float speed = moveDirection.magnitude * (currentSpeed / runSpeed);
                animatorController.SetSpeed(speed);
            }
        }
        
        /// <summary>
        /// Mounts the avatar on a vehicle
        /// </summary>
        /// <param name="motorcycleTransform">Vehicle transform to mount on</param>
        /// <param name="seatPosition">Position relative to vehicle for the avatar</param>
        public void OnMotorcycleMount(Transform motorcycleTransform, Vector3 seatPosition)
        {
            if (isMounted || motorcycleTransform == null)
                return;
                
            // Disable character controller when mounting the motorcycle
            characterController.enabled = false;
            
            // Store current mount
            currentMount = motorcycleTransform;
            
            // Parent avatar to motorcycle
            transform.SetParent(motorcycleTransform);
            
            // Set position on the motorcycle seat
            transform.localPosition = seatPosition; 
            transform.localRotation = Quaternion.identity;
            
            // Set mounted state
            isMounted = true;
            
            // Trigger animation
            if (animatorController != null)
            {
                animatorController.SetMounted(true);
            }
        }
        
        /// <summary>
        /// Dismounts the avatar from the current vehicle
        /// </summary>
        public void OnMotorcycleDismount()
        {
            if (!isMounted || currentMount == null)
                return;
                
            // Save position before unparenting
            Vector3 worldPosition = transform.position;
            Quaternion worldRotation = transform.rotation;
            
            // Unparent from motorcycle
            transform.SetParent(null);
            
            // Restore world position
            transform.position = worldPosition;
            transform.rotation = worldRotation;
            
            // Re-enable character controller
            characterController.enabled = true;
            
            // Set state
            isMounted = false;
            currentMount = null;
            
            // Trigger animation
            if (animatorController != null)
            {
                animatorController.SetMounted(false);
            }
        }
        
        /// <summary>
        /// Gets whether the avatar is currently mounted
        /// </summary>
        public bool IsMounted()
        {
            return isMounted;
        }
        
        /// <summary>
        /// Gets the current movement speed as a percentage (0-1)
        /// </summary>
        public float GetNormalizedSpeed()
        {
            return moveDirection.magnitude * (currentSpeed / runSpeed);
        }
    }
} 