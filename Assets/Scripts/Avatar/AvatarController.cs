using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TequilaSunrise.UI;

namespace TequilaSunrise.Avatar
{
    [RequireComponent(typeof(CharacterController))]
    public class AvatarController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private ARPlaneManager planeManager;
        
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 2.0f;
        [SerializeField] private float sprintSpeed = 4.0f;
        [SerializeField] private float acceleration = 10.0f;
        [SerializeField] private float deceleration = 15.0f;
        [SerializeField] private float rotationSpeed = 10.0f;
        [SerializeField] private float airControlFactor = 0.5f;
        
        [Header("Jump Settings")]
        [SerializeField] private float jumpForce = 5.0f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float groundCheckRadius = 0.3f;
        [SerializeField] private LayerMask groundLayers;
        
        [Header("Mobile Controls")]
        [SerializeField] private Joystick _joystick;
        
        // Public property for the joystick
        public Joystick joystick {
            get { return _joystick; }
            set { _joystick = value; }
        }
        
        private int animIDSpeed;
        private int animIDGrounded;
        private int animIDJump;
        private int animIDSprint;
        
        private Vector3 moveDirection;
        private Vector3 currentVelocity;
        private float verticalVelocity;
        private float currentSpeed;
        private bool isJumping;
        private bool isGrounded;
        private bool isSprinting;
        
        private void Start()
        {
            if (animator == null) animator = GetComponent<Animator>();
            if (characterController == null) characterController = GetComponent<CharacterController>();
            
            animIDSpeed = Animator.StringToHash("Speed");
            animIDGrounded = Animator.StringToHash("Grounded");
            animIDJump = Animator.StringToHash("Jump");
            animIDSprint = Animator.StringToHash("Sprint");
            
            currentSpeed = walkSpeed;
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
            RaycastHit hit;
            Vector3 spherePosition = transform.position + Vector3.up * characterController.radius;
            isGrounded = Physics.SphereCast(spherePosition, groundCheckRadius, Vector3.down, out hit, 
                characterController.radius + 0.1f, groundLayers);
        }
        
        private void HandleMovement()
        {
            float horizontal = _joystick.Horizontal;
            float vertical = _joystick.Vertical;
            Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;
            
            if (direction.magnitude >= 0.1f)
            {
                float targetSpeed = isSprinting ? sprintSpeed : walkSpeed;
                currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * acceleration);
                
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                float angle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, Time.deltaTime * rotationSpeed);
                transform.rotation = Quaternion.Euler(0, angle, 0);
                
                moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
                if (!isGrounded) moveDirection *= airControlFactor;
                
                Vector3 targetVelocity = moveDirection * currentSpeed;
                currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.deltaTime * acceleration);
            }
            else
            {
                currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, Time.deltaTime * deceleration);
                currentSpeed = 0;
            }
            
            characterController.Move(currentVelocity * Time.deltaTime);
        }
        
        private void HandleGravity()
        {
            if (isGrounded && verticalVelocity < 0)
            {
                verticalVelocity = -2f;
                isJumping = false;
            }
            
            verticalVelocity += gravity * Time.deltaTime;
            characterController.Move(new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
        }
        
        public void Jump()
        {
            if (isGrounded && !isJumping)
            {
                verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
                isJumping = true;
                animator.SetTrigger(animIDJump);
            }
        }
        
        public void ToggleSprint(bool sprinting)
        {
            isSprinting = sprinting;
        }
        
        private void UpdateAnimator()
        {
            float normalizedSpeed = currentVelocity.magnitude / sprintSpeed;
            animator.SetFloat(animIDSpeed, normalizedSpeed);
            animator.SetBool(animIDGrounded, isGrounded);
            animator.SetBool(animIDSprint, isSprinting);
        }
        
        public void OnMotorcycleMount(Transform motorcycleTransform)
        {
            characterController.enabled = false;
            currentVelocity = Vector3.zero;
            verticalVelocity = 0f;
            isJumping = false;
            isSprinting = false;
            
            transform.SetParent(motorcycleTransform);
            transform.localPosition = new Vector3(0, 0.5f, 0);
            transform.localRotation = Quaternion.identity;
        }
        
        public void OnMotorcycleDismount()
        {
            transform.SetParent(null);
            characterController.enabled = true;
            currentVelocity = Vector3.zero;
            currentSpeed = walkSpeed;
        }
    }
}