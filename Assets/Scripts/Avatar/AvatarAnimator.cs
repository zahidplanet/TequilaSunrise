using UnityEngine;
using System.Collections.Generic;

namespace TequilaSunrise.Avatar
{
    /// <summary>
    /// Handles avatar animation states and transitions
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class AvatarAnimator : MonoBehaviour
    {
        [Header("Animation Parameters")]
        [Tooltip("Parameter name for movement speed")]
        [SerializeField] private string speedParameterName = "Speed";
        
        [Tooltip("Parameter name for grounded state")]
        [SerializeField] private string groundedParameterName = "Grounded";
        
        [Tooltip("Parameter name for jump trigger")]
        [SerializeField] private string jumpParameterName = "Jump";
        
        [Tooltip("Parameter name for mounted state")]
        [SerializeField] private string mountedParameterName = "IsMounted";
        
        [Header("Animation Settings")]
        [Tooltip("Smoothing for speed parameter changes")]
        [SerializeField] private float speedDampTime = 0.1f;
        
        [Tooltip("Threshold for movement detection")]
        [SerializeField] private float movementThreshold = 0.1f;
        
        [Header("State Debug")]
        [SerializeField] private bool showDebug = false;
        
        // Cached properties
        private Animator animator;
        private int speedParameterHash;
        private int groundedParameterHash;
        private int jumpParameterHash;
        private int mountedParameterHash;
        
        // Current state
        private float currentSpeed;
        private bool isGrounded = true;
        private bool isMounted = false;
        
        private void Awake()
        {
            // Get references
            animator = GetComponent<Animator>();
            
            // Cache parameter hashes for better performance
            speedParameterHash = Animator.StringToHash(speedParameterName);
            groundedParameterHash = Animator.StringToHash(groundedParameterName);
            jumpParameterHash = Animator.StringToHash(jumpParameterName);
            mountedParameterHash = Animator.StringToHash(mountedParameterName);
        }
        
        private void Update()
        {
            if (showDebug)
            {
                DebugCurrentState();
            }
        }
        
        /// <summary>
        /// Set the avatar movement speed
        /// </summary>
        /// <param name="speed">Speed value (0-1 = walk, >1 = run)</param>
        public void SetSpeed(float speed)
        {
            currentSpeed = speed;
            if (animator != null)
            {
                animator.SetFloat(speedParameterHash, speed, speedDampTime, Time.deltaTime);
            }
        }
        
        /// <summary>
        /// Set whether the avatar is grounded or in air
        /// </summary>
        /// <param name="grounded">True if on ground, false if in air</param>
        public void SetGrounded(bool grounded)
        {
            isGrounded = grounded;
            if (animator != null)
            {
                animator.SetBool(groundedParameterHash, grounded);
            }
        }
        
        /// <summary>
        /// Trigger jump animation
        /// </summary>
        public void TriggerJump()
        {
            if (animator != null && isGrounded)
            {
                animator.SetTrigger(jumpParameterHash);
                // Temporarily set grounded to false to avoid multiple jumps
                animator.SetBool(groundedParameterHash, false);
            }
        }
        
        /// <summary>
        /// Set whether the avatar is mounted on a vehicle
        /// </summary>
        /// <param name="mounted">True if mounted, false if not</param>
        public void SetMounted(bool mounted)
        {
            isMounted = mounted;
            if (animator != null)
            {
                animator.SetBool(mountedParameterHash, mounted);
            }
        }
        
        /// <summary>
        /// Check if avatar is currently in movement animation state
        /// </summary>
        public bool IsMoving()
        {
            return currentSpeed > movementThreshold;
        }
        
        /// <summary>
        /// Check if an animation tag is active
        /// </summary>
        /// <param name="tag">Animation tag to check</param>
        public bool HasAnimationTag(string tag)
        {
            return animator != null && animator.GetCurrentAnimatorStateInfo(0).IsTag(tag);
        }
        
        /// <summary>
        /// Play a specific animation by name
        /// </summary>
        /// <param name="stateName">Name of animation state</param>
        /// <param name="layer">Animation layer</param>
        /// <param name="normalizedTime">Start time (0-1)</param>
        public void PlayAnimation(string stateName, int layer = 0, float normalizedTime = 0)
        {
            if (animator != null)
            {
                animator.Play(stateName, layer, normalizedTime);
            }
        }
        
        /// <summary>
        /// Reset all animation triggers
        /// </summary>
        public void ResetTriggers()
        {
            if (animator != null)
            {
                animator.ResetTrigger(jumpParameterHash);
            }
        }
        
        /// <summary>
        /// Get current animation state name
        /// </summary>
        /// <returns>State name</returns>
        public string GetCurrentStateName()
        {
            if (animator == null) return "Unknown";
            
            AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
            return clipInfo.Length > 0 ? clipInfo[0].clip.name : "Unknown";
        }
        
        /// <summary>
        /// Debug logging for current animation state
        /// </summary>
        private void DebugCurrentState()
        {
            if (animator != null)
            {
                string currentState = GetCurrentStateName();
                Debug.Log($"Avatar Animator - State: {currentState}, Speed: {currentSpeed}, Grounded: {isGrounded}, Mounted: {isMounted}");
            }
        }
    }
} 