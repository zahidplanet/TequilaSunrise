using UnityEngine;

namespace TequilaSunrise.Avatar
{
    /// <summary>
    /// Controls avatar animations based on character state and input
    /// </summary>
    public class AvatarAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        
        // Animation parameter hashes for efficiency
        private int isWalkingHash;
        private int isRunningHash;
        private int isJumpingHash;
        private int isIdleHash;

        private void Awake()
        {
            // If not assigned in inspector, try to get from this gameObject
            if (animator == null)
                animator = GetComponent<Animator>();
            
            // Cache animation parameter hashes
            isWalkingHash = Animator.StringToHash("IsWalking");
            isRunningHash = Animator.StringToHash("IsRunning");
            isJumpingHash = Animator.StringToHash("IsJumping");
            isIdleHash = Animator.StringToHash("IsIdle");
        }

        public void SetWalking(bool isWalking)
        {
            if (animator != null)
                animator.SetBool(isWalkingHash, isWalking);
        }

        public void SetRunning(bool isRunning)
        {
            if (animator != null)
                animator.SetBool(isRunningHash, isRunning);
        }

        public void SetJumping(bool isJumping)
        {
            if (animator != null)
                animator.SetBool(isJumpingHash, isJumping);
        }

        public void SetIdle(bool isIdle)
        {
            if (animator != null)
                animator.SetBool(isIdleHash, isIdle);
        }

        public void TriggerAnimation(string triggerName)
        {
            if (animator != null)
                animator.SetTrigger(triggerName);
        }
    }
}