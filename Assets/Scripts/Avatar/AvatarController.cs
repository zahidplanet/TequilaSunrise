using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TequilaSunrise.UI;

namespace TequilaSunrise.Avatar
{
    [RequireComponent(typeof(CharacterController))]
    public class AvatarController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float sprintSpeedMultiplier = 1.5f;
        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float rotationSpeed = 10f;

        [Header("References")]
        [SerializeField] private MobileInputController inputController;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private ARPlaneManager planeManager;
        
        [Header("Mobile Controls")]
        [SerializeField] private Joystick joystickInput;
        
        private Vector3 velocity;
        private bool isGrounded;
        private bool isSprinting;
        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
        
        public Joystick joystick
        {
            get => joystickInput;
            set => joystickInput = value;
        }
        
        private void Awake()
        {
            if (animator == null) animator = GetComponent<Animator>();
            if (characterController == null) characterController = GetComponent<CharacterController>();
            
            if (inputController != null)
            {
                inputController.OnJumpPressed.AddListener(OnJump);
                inputController.OnSprintPressed.AddListener(() => isSprinting = true);
                inputController.OnSprintReleased.AddListener(() => isSprinting = false);
            }
        }
        
        private void OnDestroy()
        {
            if (inputController != null)
            {
                inputController.OnJumpPressed.RemoveListener(OnJump);
                inputController.OnSprintPressed.RemoveListener(() => isSprinting = true);
                inputController.OnSprintReleased.RemoveListener(() => isSprinting = false);
            }
        }
        
        private void Update()
        {
            HandleMovement();
            UpdateAnimator();
        }
        
        private void HandleMovement()
        {
            isGrounded = characterController.isGrounded;
            
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            Vector2 input = inputController != null ? inputController.GetMovementInput() : Vector2.zero;
            Vector3 move = new Vector3(input.x, 0, input.y);

            if (cameraTransform != null)
            {
                move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
                move.y = 0f;
            }

            if (move != Vector3.zero)
            {
                float currentSpeed = moveSpeed * (isSprinting ? sprintSpeedMultiplier : 1f);
                characterController.Move(move * (currentSpeed * Time.deltaTime));

                Quaternion targetRotation = Quaternion.LookRotation(move);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }
        
        private void UpdateAnimator()
        {
            if (animator != null)
            {
                Vector2 input = inputController != null ? inputController.GetMovementInput() : Vector2.zero;
                float speed = input.magnitude * (isSprinting ? sprintSpeedMultiplier : 1f);
                animator.SetFloat(SpeedHash, speed);
                animator.SetBool(IsGroundedHash, isGrounded);
            }
        }
        
        private void OnJump()
        {
            if (isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            }
        }
        
        public void Jump()
        {
            OnJump();
        }
        
        public void ToggleSprint(bool sprinting)
        {
            isSprinting = sprinting;
        }
        
        public void OnMotorcycleMount(Transform motorcycleTransform)
        {
            characterController.enabled = false;
            velocity = Vector3.zero;
            isSprinting = false;
            
            transform.SetParent(motorcycleTransform);
            transform.localPosition = new Vector3(0, 0.5f, 0);
            transform.localRotation = Quaternion.identity;
        }
        
        public void OnMotorcycleDismount()
        {
            transform.SetParent(null);
            characterController.enabled = true;
            velocity = Vector3.zero;
        }
    }
}