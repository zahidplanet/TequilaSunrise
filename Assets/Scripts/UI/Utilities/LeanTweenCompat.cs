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
                return new LTDescr { _coroutine = tween, _target = target, _owner = owner };
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
                return new LTDescr { _coroutine = tween, _target = canvasGroup.gameObject, _owner = owner };
            }
            return new LTDescr();
        }
        
        /// <summary>
        /// Move a RectTransform
        /// </summary>
        public static LTDescr move(RectTransform rectTransform, Vector3 to, float time)
        {
            var owner = rectTransform.GetComponent<MonoBehaviour>();
            if (owner != null)
            {
                var tween = TweenUtility.MoveUI(owner, rectTransform, to, time);
                return new LTDescr { _coroutine = tween, _target = rectTransform.gameObject, _owner = owner };
            }
            return new LTDescr();
        }
        
        /// <summary>
        /// Move a GameObject
        /// </summary>
        public static LTDescr move(GameObject target, Vector3 to, float time)
        {
            var owner = target.GetComponent<MonoBehaviour>();
            if (owner == null) return new LTDescr();
            
            var rectTransform = target.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                return move(rectTransform, to, time);
            }
            
            // For non-UI objects, we would need a position tween
            // This is just a placeholder as we don't have a position tween in TweenUtility yet
            Debug.LogWarning("LeanTween.move for non-UI GameObjects is not fully implemented in the compatibility layer.");
            return new LTDescr();
        }
        
        /// <summary>
        /// Rotate a GameObject
        /// </summary>
        public static LTDescr rotateLocal(GameObject target, Vector3 to, float time)
        {
            var owner = target.GetComponent<MonoBehaviour>();
            if (owner != null)
            {
                var tween = TweenUtility.Rotate(owner, target, to, time);
                return new LTDescr { _coroutine = tween, _target = target, _owner = owner };
            }
            return new LTDescr();
        }
        
        /// <summary>
        /// Change the color of a graphic
        /// </summary>
        public static LTDescr color(Graphic graphic, Color to, float time)
        {
            var owner = graphic.GetComponent<MonoBehaviour>();
            if (owner != null)
            {
                var tween = TweenUtility.ChangeGraphicColor(owner, graphic, to, time);
                return new LTDescr { _coroutine = tween, _target = graphic.gameObject, _owner = owner };
            }
            return new LTDescr();
        }
        
        /// <summary>
        /// Delay execution
        /// </summary>
        public static LTDescr delayedCall(GameObject target, float delayTime, Action callback)
        {
            var owner = target.GetComponent<MonoBehaviour>();
            if (owner != null)
            {
                var coroutine = owner.StartCoroutine(DelayedCallCoroutine(delayTime, callback));
                return new LTDescr { _coroutine = coroutine, _target = target, _owner = owner, _onComplete = callback };
            }
            return new LTDescr();
        }
        
        private static System.Collections.IEnumerator DelayedCallCoroutine(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
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
        public MonoBehaviour _owner;
        public Action _onComplete;
        
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
            
            if (_target != null && _coroutine != null && _owner != null)
            {
                TweenUtility.SetOnComplete(_owner, _coroutine, onComplete);
            }
            
            return this;
        }
        
        /// <summary>
        /// Set the delay before the tween starts
        /// </summary>
        public LTDescr setDelay(float delay)
        {
            // Delay is not implemented in the compatibility layer
            Debug.LogWarning("setDelay is not fully implemented in the LeanTween compatibility layer");
            return this;
        }
        
        /// <summary>
        /// Set from value
        /// </summary>
        public LTDescr setFrom(float from)
        {
            // From value is not implemented in the compatibility layer
            Debug.LogWarning("setFrom is not fully implemented in the LeanTween compatibility layer");
            return this;
        }
        
        /// <summary>
        /// Set from value
        /// </summary>
        public LTDescr setFrom(Vector3 from)
        {
            // From value is not implemented in the compatibility layer
            Debug.LogWarning("setFrom is not fully implemented in the LeanTween compatibility layer");
            return this;
        }
        
        /// <summary>
        /// Set ignore time scale
        /// </summary>
        public LTDescr setIgnoreTimeScale(bool ignoreTimeScale)
        {
            // Ignore time scale is not implemented in the compatibility layer
            Debug.LogWarning("setIgnoreTimeScale is not fully implemented in the LeanTween compatibility layer");
            return this;
        }
    }
}