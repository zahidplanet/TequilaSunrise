using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TequilaSunrise.Avatar
{
    /// <summary>
    /// Physics-based character controller with enhanced movement and collision handling
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PhysicsCharacterController : MonoBehaviour
    {
        [Header("Physics References")]
        [SerializeField] private Rigidbody rb;
        [SerializeField] private CapsuleCollider capsuleCollider;
        [SerializeField] private Transform modelTransform;
        [SerializeField] private AvatarAnimator animatorController;
        
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 2.0f;
        [SerializeField] private float runSpeed = 4.0f;
        [SerializeField] private float sprintSpeed = 6.0f;
        [SerializeField] private float acceleration = 15.0f;
        [SerializeField] private float deceleration = 20.0f;
        [SerializeField] private float rotationSpeed = 12.0f;
        [SerializeField] private float airControl = 0.3f;
        
        [Header("Jump Settings")]
        [SerializeField] private float jumpForce = 7.0f;
        [SerializeField] private float jumpCooldown = 0.25f;
        [SerializeField] private float jumpBufferTime = 0.1f;
        [SerializeField] private float coyoteTime = 0.15f;
        [SerializeField] private float fallMultiplier = 2.5f;
        [SerializeField] private float lowJumpMultiplier = 2.0f;
        [SerializeField] private int maxAirJumps = 0;
        
        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask groundLayers;
        [SerializeField] private float groundedGravityForce = -1.0f;
        [SerializeField] private float groundSnapForce = 5.0f;
        [SerializeField] private float slopeLimit = 45f;
        
        [Header("Sprint Settings")]
        [SerializeField] private bool enableSprint = true;
        [SerializeField] private float sprintAccelerationMultiplier = 1.5f;
        [SerializeField] private float sprintStaminaMax = 100f;
        [SerializeField] private float sprintStaminaDrain = 15f;
        [SerializeField] private float sprintStaminaRegenRate = 25f;
        [SerializeField] private float sprintStaminaRegenDelay = 1.5f;
        
        [Header("Audio")]
        [SerializeField] private AudioSource footstepAudio;
        [SerializeField] private AudioSource jumpAudio;
        [SerializeField] private AudioSource landAudio;
        [SerializeField] private float footstepRate = 0.3f;
        
        [Header("Mobile Controls")]
        [SerializeField] private bool isMobile = true;
        [SerializeField] private Joystick joystick;
        [SerializeField] private bool enableAutoRun = false;
        [SerializeField] private float autoRunDeadzone = 0.2f;
        [SerializeField] private float autoRunSpeed = 2.0f;
        
        [Header("Events")]
        [SerializeField] private UnityEngine.Events.UnityEvent onJump;
        [SerializeField] private UnityEngine.Events.UnityEvent onLand;
        
        // State variables
        private Vector3 moveDirection;
        private Vector3 currentVelocity;
        private float currentSpeed;
        private float targetSpeed;
        private bool isGrounded;
        private bool isSprinting;
        private bool isJumping;
        private bool jumpRequested;
        private int airJumpCount;
        private float jumpBufferTimer;
        private float coyoteTimer;
        private float jumpCooldownTimer;
        private float sprintStamina;
        private float lastSprintTime;
        private float footstepTimer;
        private float lastGroundedTime;
        private Vector3 groundNormal = Vector3.up;
        
        // Cached values
        private Vector3 originalCenter;
        private float originalHeight;
        private bool isMounted;
        
        // Input
        private Vector2 inputVector;
        private bool jumpInput;
        private bool sprintInput;
        
        #region Unity Methods
        
        private void Awake()
        {
            InitializeComponents();
            footstepTimer = 0f;
            sprintStamina = sprintStaminaMax;
            lastSprintTime = -sprintStaminaRegenDelay;
            originalCenter = capsuleCollider.center;
            originalHeight = capsuleCollider.height;
        }
        
        private void Update()
        {
            GetInput();
            UpdateJumpTimers();
            UpdateAnimation();
            
            // Update sprint stamina
            if (isSprinting)
            {
                lastSprintTime = Time.time;
                sprintStamina = Mathf.Max(0, sprintStamina - sprintStaminaDrain * Time.deltaTime);
                if (sprintStamina <= 0f)
                {
                    isSprinting = false;
                }
            }
            else if (Time.time - lastSprintTime >= sprintStaminaRegenDelay)
            {
                sprintStamina = Mathf.Min(sprintStaminaMax, sprintStamina + sprintStaminaRegenRate * Time.deltaTime);
            }
        }
        
        private void FixedUpdate()
        {
            CheckGrounded();
            
            if (!isMounted)
            {
                Vector3 movementForce = CalculateMovement();
                ApplyGravity();
                HandleJump();
                
                // Apply all forces
                rb.AddForce(movementForce, ForceMode.Acceleration);
                
                // Rotate model to face movement direction
                if (modelTransform != null && moveDirection.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                    modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
                }
                
                // Handle footstep audio
                HandleFootstepAudio();
            }
        }
        
        #endregion
        
        #region Initialization
        
        private void InitializeComponents()
        {
            if (rb == null) rb = GetComponent<Rigidbody>();
            if (capsuleCollider == null) capsuleCollider = GetComponent<CapsuleCollider>();
            if (modelTransform == null && transform.childCount > 0) modelTransform = transform.GetChild(0);
            if (animatorController == null) animatorController = GetComponentInChildren<AvatarAnimator>();
            
            if (groundCheck == null)
            {
                groundCheck = new GameObject("GroundCheck").transform;
                groundCheck.SetParent(transform);
                groundCheck.localPosition = new Vector3(0, -capsuleCollider.height / 2f, 0);
            }
            
            // Configure rigidbody for character controller
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.mass = 70f;
            rb.linearDamping = 0f;
        }
        
        #endregion
        
        #region Input Handling
        
        private void GetInput()
        {
            // Mobile input
            if (isMobile && joystick != null)
            {
                inputVector = new Vector2(joystick.Horizontal, joystick.Vertical);
                
                // Auto-run logic
                if (enableAutoRun && inputVector.magnitude > autoRunDeadzone)
                {
                    // If joystick is pushed beyond deadzone, auto-run in that direction
                    inputVector = inputVector.normalized;
                }
                else if (enableAutoRun)
                {
                    // When joystick is released, continue in forward direction at auto-run speed
                    inputVector = Vector2.up * (autoRunSpeed / runSpeed);
                }
            }
            else
            {
                // Keyboard input
                inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                
                // Jump and sprint inputs from keyboard
                jumpInput = jumpInput || Input.GetButtonDown("Jump");
                sprintInput = Input.GetKey(KeyCode.LeftShift);
            }
            
            // Jump buffer - remember jump input for short time
            if (jumpInput && jumpBufferTimer <= 0)
            {
                jumpBufferTimer = jumpBufferTime;
                jumpRequested = true;
            }
            
            // Sprint logic
            if (enableSprint && sprintInput && sprintStamina > 0f && isGrounded && inputVector.magnitude > 0.7f)
            {
                isSprinting = true;
            }
            else if (!sprintInput || inputVector.magnitude < 0.1f)
            {
                isSprinting = false;
            }
            
            // Reset input for next frame
            jumpInput = false;
        }
        
        #endregion
        
        #region Movement
        
        private Vector3 CalculateMovement()
        {
            // Calculate movement direction in world space
            Vector3 forward = Camera.main.transform.forward;
            Vector3 right = Camera.main.transform.right;
            
            // Project vectors onto horizontal plane
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();
            
            moveDirection = (forward * inputVector.y + right * inputVector.x).normalized;
            
            // Adjust to slope normal
            if (isGrounded && Vector3.Angle(groundNormal, Vector3.up) <= slopeLimit)
            {
                moveDirection = Vector3.ProjectOnPlane(moveDirection, groundNormal).normalized;
            }
            
            // Determine target speed
            float speedMultiplier = 1.0f;
            
            if (isSprinting && sprintStamina > 0)
            {
                targetSpeed = sprintSpeed * inputVector.magnitude;
                speedMultiplier = sprintAccelerationMultiplier;
            }
            else if (inputVector.magnitude > 0.7f)
            {
                targetSpeed = runSpeed * inputVector.magnitude;
            }
            else
            {
                targetSpeed = walkSpeed * inputVector.magnitude;
            }
            
            // Apply acceleration or deceleration
            float accelRate = (targetSpeed > currentSpeed) ? acceleration * speedMultiplier : deceleration;
            
            // Reduce acceleration in air
            if (!isGrounded) accelRate *= airControl;
            
            // Smoothly adjust current speed
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accelRate * Time.fixedDeltaTime);
            
            // Calculate final movement force
            Vector3 movementForce = moveDirection * currentSpeed;
            
            // Calculate planar velocity for comparison
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            
            // If we want to change direction, apply counter force
            if (horizontalVelocity.magnitude > 0.1f && Vector3.Dot(horizontalVelocity.normalized, movementForce.normalized) < 0.9f)
            {
                float counterForce = deceleration * 2.0f; // Extra force for direction change
                movementForce += -horizontalVelocity.normalized * counterForce;
            }
            
            return movementForce;
        }
        
        private void ApplyGravity()
        {
            if (isGrounded)
            {
                // Apply slight downforce to keep character grounded
                rb.AddForce(Vector3.up * groundedGravityForce, ForceMode.Acceleration);
                
                // Ground snap force to stick to slopes
                if (Vector3.Angle(groundNormal, Vector3.up) > 0)
                {
                    rb.AddForce(-groundNormal * groundSnapForce, ForceMode.Acceleration);
                }
            }
            else
            {
                // Apply stronger gravity while falling
                float gravityMultiplier = rb.linearVelocity.y < 0 ? fallMultiplier : lowJumpMultiplier;
                rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);
            }
        }
        
        #endregion
        
        #region Jumping
        
        private void UpdateJumpTimers()
        {
            // Update jump buffer timer
            if (jumpBufferTimer > 0)
            {
                jumpBufferTimer -= Time.deltaTime;
            }
            
            // Update coyote time timer
            if (!isGrounded && lastGroundedTime > 0)
            {
                coyoteTimer -= Time.deltaTime;
            }
            
            // Update jump cooldown
            if (jumpCooldownTimer > 0)
            {
                jumpCooldownTimer -= Time.deltaTime;
            }
        }
        
        private void HandleJump()
        {
            bool canJump = (isGrounded || (coyoteTimer > 0) || (airJumpCount < maxAirJumps)) && jumpCooldownTimer <= 0;
            
            if (jumpRequested && canJump)
            {
                // Apply jump force
                Vector3 jumpVelocity = Vector3.up * jumpForce;
                
                // Add some of the current horizontal velocity to maintain momentum
                Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                jumpVelocity += horizontalVelocity * 0.5f;
                
                // Set the velocity directly for consistent jump height
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z); // Zero out vertical velocity
                rb.AddForce(jumpVelocity, ForceMode.Impulse);
                
                // Update state
                isJumping = true;
                jumpRequested = false;
                jumpCooldownTimer = jumpCooldown;
                
                // If jumping from air, count as air jump
                if (!isGrounded && coyoteTimer <= 0)
                {
                    airJumpCount++;
                }
                
                // Reset coyote timer
                coyoteTimer = 0;
                
                // Trigger animation
                if (animatorController != null)
                {
                    animatorController.TriggerJump();
                }
                
                // Play jump sound
                if (jumpAudio != null)
                {
                    jumpAudio.Play();
                }
                
                // Fire jump event
                onJump?.Invoke();
            }
        }
        
        #endregion
        
        #region Ground Detection
        
        private void CheckGrounded()
        {
            bool wasGrounded = isGrounded;
            
            // Sphere cast for ground detection
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayers, QueryTriggerInteraction.Ignore);
            
            // Get ground normal for slope detection
            if (isGrounded)
            {
                RaycastHit hit;
                if (Physics.SphereCast(groundCheck.position + Vector3.up * 0.1f, groundCheckRadius, Vector3.down, out hit, 0.2f, groundLayers, QueryTriggerInteraction.Ignore))
                {
                    groundNormal = hit.normal;
                }
                else
                {
                    groundNormal = Vector3.up;
                }
                
                // Reset jump variables
                airJumpCount = 0;
                coyoteTimer = coyoteTime;
                lastGroundedTime = Time.time;
                
                // If just landed
                if (!wasGrounded)
                {
                    isJumping = false;
                    
                    // Play land sound
                    if (landAudio != null && rb.linearVelocity.y < -3f)
                    {
                        landAudio.Play();
                    }
                    
                    // Fire land event
                    onLand?.Invoke();
                }
            }
            else
            {
                // Start coyote timer if just left ground
                if (wasGrounded)
                {
                    coyoteTimer = coyoteTime;
                }
            }
            
            // Update animator grounded state
            if (animatorController != null)
            {
                animatorController.SetGrounded(isGrounded);
            }
        }
        
        #endregion
        
        #region Animation & Audio
        
        private void UpdateAnimation()
        {
            if (animatorController == null) return;
            
            // Calculate normalized speed for animation (0-1 for walk/run, >1 for sprint)
            float normalizedSpeed = isSprinting ? 
                                    (currentSpeed / walkSpeed) : 
                                    (currentSpeed / runSpeed);
            
            animatorController.SetSpeed(normalizedSpeed);
        }
        
        private void HandleFootstepAudio()
        {
            if (footstepAudio == null || !isGrounded || currentSpeed < 0.1f) return;
            
            // Calculate footstep rate based on speed
            float stepRate = footstepRate / (currentSpeed / walkSpeed);
            
            footstepTimer -= Time.fixedDeltaTime;
            if (footstepTimer <= 0)
            {
                footstepAudio.pitch = Random.Range(0.9f, 1.1f);
                footstepAudio.volume = Mathf.Lerp(0.2f, 0.6f, currentSpeed / runSpeed);
                footstepAudio.Play();
                footstepTimer = stepRate;
            }
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Trigger a jump from external sources (like UI button)
        /// </summary>
        public void Jump()
        {
            jumpInput = true;
        }
        
        /// <summary>
        /// Set sprint state from external sources
        /// </summary>
        public void SetSprint(bool sprint)
        {
            sprintInput = sprint;
        }
        
        /// <summary>
        /// Get current controller state data
        /// </summary>
        public ControllerState GetState()
        {
            return new ControllerState
            {
                IsGrounded = isGrounded,
                IsSprinting = isSprinting,
                IsJumping = isJumping,
                CurrentSpeed = currentSpeed,
                NormalizedSpeed = currentSpeed / runSpeed,
                SprintStaminaPercent = sprintStamina / sprintStaminaMax,
                MoveDirection = moveDirection,
                Velocity = rb.linearVelocity
            };
        }
        
        /// <summary>
        /// Mount the character on a vehicle
        /// </summary>
        public void Mount(Transform vehicleTransform, Vector3 seatPosition)
        {
            if (isMounted) return;
            
            // Store current state
            Vector3 worldPos = transform.position;
            Quaternion worldRot = transform.rotation;
            
            // Disable physics
            rb.isKinematic = true;
            capsuleCollider.enabled = false;
            
            // Parent to vehicle
            transform.SetParent(vehicleTransform);
            transform.localPosition = seatPosition;
            transform.localRotation = Quaternion.identity;
            
            // Update state
            isMounted = true;
            
            // Update animation
            if (animatorController != null)
            {
                animatorController.SetMounted(true);
            }
        }
        
        /// <summary>
        /// Dismount from the vehicle
        /// </summary>
        public void Dismount()
        {
            if (!isMounted) return;
            
            // Store current world position
            Vector3 worldPos = transform.position;
            Quaternion worldRot = transform.rotation;
            
            // Unparent from vehicle
            transform.SetParent(null);
            transform.position = worldPos;
            transform.rotation = worldRot;
            
            // Re-enable physics
            rb.isKinematic = false;
            capsuleCollider.enabled = true;
            
            // Update state
            isMounted = false;
            
            // Update animation
            if (animatorController != null)
            {
                animatorController.SetMounted(false);
            }
            
            // Apply small upward force to prevent sticking
            rb.AddForce(Vector3.up * 2f, ForceMode.Impulse);
        }
        
        /// <summary>
        /// Crouch the character
        /// </summary>
        public void SetCrouch(bool crouch)
        {
            if (crouch)
            {
                capsuleCollider.height = originalHeight * 0.6f;
                capsuleCollider.center = new Vector3(originalCenter.x, originalCenter.y * 0.6f, originalCenter.z);
            }
            else
            {
                capsuleCollider.height = originalHeight;
                capsuleCollider.center = originalCenter;
            }
        }
        
        #endregion
        
        #region Types
        
        /// <summary>
        /// Struct containing character controller state data
        /// </summary>
        public struct ControllerState
        {
            public bool IsGrounded;
            public bool IsSprinting;
            public bool IsJumping;
            public float CurrentSpeed;
            public float NormalizedSpeed;
            public float SprintStaminaPercent;
            public Vector3 MoveDirection;
            public Vector3 Velocity;
        }
        
        #endregion
        
        #region Debug
        
        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null) return;
            
            // Draw ground check sphere
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            
            // Draw movement direction
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, moveDirection.normalized);
            
            // Draw ground normal
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(groundCheck.position, groundNormal);
        }
        
        #endregion
    }
} 