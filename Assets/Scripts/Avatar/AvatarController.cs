using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;

public class AvatarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private ARPlaneManager planeManager;
    
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float rotationSpeed = 10.0f;
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravity = -9.81f;
    
    [Header("Mobile Controls")]
    [SerializeField] private Joystick joystick;
    [SerializeField] private GameObject jumpButton;
    
    // Animation parameters
    private int animIDSpeed;
    private int animIDGrounded;
    private int animIDJump;
    
    // Movement variables
    private Vector3 moveDirection;
    private float verticalVelocity;
    private bool isJumping;
    private bool isGrounded;
    
    private void Awake()
    {
        // Get components if not assigned
        if (animator == null)
            animator = GetComponent<Animator>();
        
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
    }
    
    private void Start()
    {
        // Set up animation IDs
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDJump = Animator.StringToHash("Jump");
    }
    
    private void Update()
    {
        HandleMovement();
        HandleGravity();
        UpdateAnimator();
    }
    
    private void HandleMovement()
    {
        // Get joystick input
        float horizontal = joystick.Horizontal;
        float vertical = joystick.Vertical;
        
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
            characterController.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
        }
        else
        {
            moveDirection = Vector3.zero;
        }
    }
    
    private void HandleGravity()
    {
        isGrounded = characterController.isGrounded;
        
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
        
        // Apply gravity
        verticalVelocity += gravity * Time.deltaTime;
        
        // Move the character vertically
        Vector3 verticalMove = new Vector3(0, verticalVelocity, 0) * Time.deltaTime;
        characterController.Move(verticalMove);
    }
    
    public void Jump()
    {
        if (isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
            isJumping = true;
            
            // Trigger jump animation
            animator.SetTrigger(animIDJump);
        }
    }
    
    private void UpdateAnimator()
    {
        // Update animator parameters
        float speed = moveDirection.magnitude * moveSpeed;
        animator.SetFloat(animIDSpeed, speed);
        animator.SetBool(animIDGrounded, isGrounded);
    }
    
    public void OnMotorcycleMount(Transform motorcycleTransform)
    {
        // Disable character controller when mounting the motorcycle
        characterController.enabled = false;
        
        // Parent avatar to motorcycle
        transform.SetParent(motorcycleTransform);
        
        // Set position on the motorcycle seat
        transform.localPosition = new Vector3(0, 0.5f, 0); // Adjust based on your motorcycle model
        transform.localRotation = Quaternion.identity;
        
        // Trigger mount animation (if you have one)
        // animator.SetBool("IsRiding", true);
    }
    
    public void OnMotorcycleDismount()
    {
        // Unparent from motorcycle
        transform.SetParent(null);
        
        // Re-enable character controller
        characterController.enabled = true;
        
        // Trigger dismount animation (if you have one)
        // animator.SetBool("IsRiding", false);
    }
} 