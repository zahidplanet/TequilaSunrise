using UnityEngine;
using UnityEngine.UI;
using System;

namespace TequilaSunrise.UI.Utilities
{
    /// <summary>
    /// Backward compatibility layer for LeanTween references
    /// </summary>
    public static class LeanTween
    {
        /// <summary>
        /// Cancel all tweens on a GameObject
        /// </summary>
        public static void cancel(GameObject target)
        {
            TweenUtility.Cancel(target);
        }
        
        /// <summary>
        /// Scale a GameObject
        /// </summary>
        public static LTDescr scale(GameObject target, Vector3 to, float time)
        {
            var owner = target.GetComponent<MonoBehaviour>();
            if (owner != null)
            {
                var tween = TweenUtility.Scale(owner, target, to, time);
                return new LTDescr { _coroutine = tween, _target = target };
            }
            return new LTDescr();
        }
        
        /// <summary>
        /// Change the alpha of a CanvasGroup
        /// </summary>
        public static LTDescr alphaCanvas(CanvasGroup canvasGroup, float to, float time)
        {
            var owner = canvasGroup.GetComponent<MonoBehaviour>();
            if (owner != null)
            {
                var tween = TweenUtility.FadeCanvasGroup(owner, canvasGroup, to, time);
                return new LTDescr { _coroutine = tween, _target = canvasGroup.gameObject };
            }
            return new LTDescr();
        }
    }
    
    /// <summary>
    /// Enum to match LeanTween ease types
    /// </summary>
    public enum LeanTweenType
    {
        notUsed, linear, easeOutQuad, easeInQuad, easeInOutQuad,
        easeInCubic, easeOutCubic, easeInOutCubic, easeInQuart,
        easeOutQuart, easeInOutQuart, easeInQuint, easeOutQuint,
        easeInOutQuint, easeInSine, easeOutSine, easeInOutSine,
        easeInExpo, easeOutExpo, easeInOutExpo, easeInCirc,
        easeOutCirc, easeInOutCirc, easeInBounce, easeOutBounce,
        easeInOutBounce, easeInBack, easeOutBack, easeInOutBack,
        easeInElastic, easeOutElastic, easeInOutElastic, punch
    }
    
    /// <summary>
    /// LeanTween descriptor class for chained methods
    /// </summary>
    public class LTDescr
    {
        public GameObject _target;
        public Coroutine _coroutine;
        private Action _onComplete;
        
        /// <summary>
        /// Set the ease type
        /// </summary>
        public LTDescr setEase(LeanTweenType easeType)
        {
            // Easing is handled within TweenUtility
            return this;
        }
        
        /// <summary>
        /// Set the completion callback
        /// </summary>
        public LTDescr setOnComplete(Action onComplete)
        {
            _onComplete = onComplete;
            
            if (_target != null && _coroutine != null)
            {
                var owner = _target.GetComponent<MonoBehaviour>();
                if (owner != null)
                {
                    TweenUtility.SetOnComplete(owner, _coroutine, onComplete);
                }
            }
            
            return this;
        }
    }
}