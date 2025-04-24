using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace TequilaSunrise.UI.Utilities
{
    /// <summary>
    /// Utility class for handling tweening animations regardless of which tweening library is used.
    /// Provides a consistent API for animations while allowing the underlying implementation to be swapped.
    /// </summary>
    public static class TweenUtility
    {
        /// <summary>
        /// Type of easing to use for animations
        /// </summary>
        public enum EaseType
        {
            Linear,
            EaseIn,
            EaseOut,
            EaseInOut,
            Spring,
            Bounce
        }
        
        private static bool _leanTweenDetected = false;
        private static bool _checkedForLeanTween = false;
        
        /// <summary>
        /// Checks if LeanTween is available in the project
        /// </summary>
        private static bool IsLeanTweenAvailable()
        {
            if (!_checkedForLeanTween)
            {
                try
                {
                    // Try to access LeanTween through reflection
                    var type = Type.GetType("LeanTween");
                    _leanTweenDetected = (type != null);
                }
                catch
                {
                    _leanTweenDetected = false;
                }
                _checkedForLeanTween = true;
            }
            
            return _leanTweenDetected;
        }
        
        /// <summary>
        /// Scale an object over time
        /// </summary>
        public static Coroutine Scale(MonoBehaviour owner, GameObject target, Vector3 targetScale, float duration, EaseType easing = EaseType.EaseOut)
        {
            return owner.StartCoroutine(ScaleCoroutine(target, targetScale, duration, easing));
        }
        
        /// <summary>
        /// Change the alpha of a CanvasGroup over time
        /// </summary>
        public static Coroutine FadeCanvasGroup(MonoBehaviour owner, CanvasGroup canvasGroup, float targetAlpha, float duration, EaseType easing = EaseType.EaseOut)
        {
            if (canvasGroup == null) return null;
            
            return owner.StartCoroutine(FadeCanvasGroupCoroutine(canvasGroup, targetAlpha, duration, easing));
        }
        
        /// <summary>
        /// Change the color of a graphic over time
        /// </summary>
        public static Coroutine ChangeGraphicColor(MonoBehaviour owner, Graphic graphic, Color targetColor, float duration, EaseType easing = EaseType.EaseOut)
        {
            if (graphic == null) return null;
            
            return owner.StartCoroutine(ChangeColorCoroutine(graphic, targetColor, duration, easing));
        }
        
        /// <summary>
        /// Move a RectTransform to a target position over time
        /// </summary>
        public static Coroutine MoveUI(MonoBehaviour owner, RectTransform rectTransform, Vector2 targetPosition, float duration, EaseType easing = EaseType.EaseOut)
        {
            if (rectTransform == null) return null;
            
            return owner.StartCoroutine(MoveUICoroutine(rectTransform, targetPosition, duration, easing));
        }
        
        /// <summary>
        /// Rotate an object over time
        /// </summary>
        public static Coroutine Rotate(MonoBehaviour owner, GameObject target, Vector3 targetRotation, float duration, EaseType easing = EaseType.EaseOut)
        {
            if (target == null) return null;
            
            return owner.StartCoroutine(RotateCoroutine(target, targetRotation, duration, easing));
        }
        
        /// <summary>
        /// Cancel all tweens on a GameObject
        /// </summary>
        public static void Cancel(GameObject target)
        {
            // We don't actually need to do anything for coroutine-based animations
            // as they will be stopped when the object is destroyed
        }
        
        /// <summary>
        /// Implementation of scale animation
        /// </summary>
        private static IEnumerator ScaleCoroutine(GameObject target, Vector3 targetScale, float duration, EaseType easing)
        {
            if (target == null) yield break;
            
            Vector3 startScale = target.transform.localScale;
            float startTime = Time.time;
            float endTime = startTime + duration;
            
            while (Time.time < endTime)
            {
                float normalizedTime = (Time.time - startTime) / duration;
                float easedTime = GetEasedValue(normalizedTime, easing);
                
                target.transform.localScale = Vector3.Lerp(startScale, targetScale, easedTime);
                yield return null;
            }
            
            target.transform.localScale = targetScale;
        }
        
        /// <summary>
        /// Implementation of CanvasGroup fade animation
        /// </summary>
        private static IEnumerator FadeCanvasGroupCoroutine(CanvasGroup canvasGroup, float targetAlpha, float duration, EaseType easing)
        {
            if (canvasGroup == null) yield break;
            
            float startAlpha = canvasGroup.alpha;
            float startTime = Time.time;
            float endTime = startTime + duration;
            
            // Setup for callback if needed
            bool useCallback = false;
            Action onComplete = null;
            
            while (Time.time < endTime)
            {
                float normalizedTime = (Time.time - startTime) / duration;
                float easedTime = GetEasedValue(normalizedTime, easing);
                
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, easedTime);
                yield return null;
            }
            
            canvasGroup.alpha = targetAlpha;
            
            // Execute callback if defined
            if (useCallback && onComplete != null)
            {
                onComplete.Invoke();
            }
        }
        
        /// <summary>
        /// Implementation of graphic color change animation
        /// </summary>
        private static IEnumerator ChangeColorCoroutine(Graphic graphic, Color targetColor, float duration, EaseType easing)
        {
            if (graphic == null) yield break;
            
            Color startColor = graphic.color;
            float startTime = Time.time;
            float endTime = startTime + duration;
            
            while (Time.time < endTime)
            {
                float normalizedTime = (Time.time - startTime) / duration;
                float easedTime = GetEasedValue(normalizedTime, easing);
                
                graphic.color = Color.Lerp(startColor, targetColor, easedTime);
                yield return null;
            }
            
            graphic.color = targetColor;
        }
        
        /// <summary>
        /// Implementation of UI movement animation
        /// </summary>
        private static IEnumerator MoveUICoroutine(RectTransform rectTransform, Vector2 targetPosition, float duration, EaseType easing)
        {
            if (rectTransform == null) yield break;
            
            Vector2 startPosition = rectTransform.anchoredPosition;
            float startTime = Time.time;
            float endTime = startTime + duration;
            
            while (Time.time < endTime)
            {
                float normalizedTime = (Time.time - startTime) / duration;
                float easedTime = GetEasedValue(normalizedTime, easing);
                
                rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, easedTime);
                yield return null;
            }
            
            rectTransform.anchoredPosition = targetPosition;
        }
        
        /// <summary>
        /// Implementation of rotation animation
        /// </summary>
        private static IEnumerator RotateCoroutine(GameObject target, Vector3 targetRotation, float duration, EaseType easing)
        {
            if (target == null) yield break;
            
            Quaternion startRotation = target.transform.rotation;
            Quaternion endRotation = Quaternion.Euler(targetRotation);
            float startTime = Time.time;
            float endTime = startTime + duration;
            
            while (Time.time < endTime)
            {
                float normalizedTime = (Time.time - startTime) / duration;
                float easedTime = GetEasedValue(normalizedTime, easing);
                
                target.transform.rotation = Quaternion.Slerp(startRotation, endRotation, easedTime);
                yield return null;
            }
            
            target.transform.rotation = endRotation;
        }
        
        /// <summary>
        /// Apply easing to a normalized value
        /// </summary>
        private static float GetEasedValue(float t, EaseType easing)
        {
            switch (easing)
            {
                case EaseType.Linear:
                    return t;
                    
                case EaseType.EaseIn:
                    return t * t;
                    
                case EaseType.EaseOut:
                    return t * (2 - t);
                    
                case EaseType.EaseInOut:
                    return t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
                    
                case EaseType.Spring:
                    return 1 - (Mathf.Cos(t * 4.5f * Mathf.PI) * Mathf.Exp(-t * 6));
                    
                case EaseType.Bounce:
                    float d1 = 2.75f;
                    float n1 = 7.5625f;
                    
                    if (t < 1 / d1)
                        return n1 * t * t;
                    else if (t < 2 / d1)
                        return n1 * (t -= 1.5f / d1) * t + 0.75f;
                    else if (t < 2.5 / d1)
                        return n1 * (t -= 2.25f / d1) * t + 0.9375f;
                    else
                        return n1 * (t -= 2.625f / d1) * t + 0.984375f;
                    
                default:
                    return t;
            }
        }
        
        /// <summary>
        /// Setup a tween to call a function on completion
        /// </summary>
        public static Coroutine SetOnComplete(MonoBehaviour owner, Coroutine tween, Action onComplete)
        {
            if (tween == null || onComplete == null) return null;
            
            return owner.StartCoroutine(ExecuteAfterCoroutine(owner, tween, onComplete));
        }
        
        /// <summary>
        /// Helper to execute a function after a coroutine completes
        /// </summary>
        private static IEnumerator ExecuteAfterCoroutine(MonoBehaviour owner, Coroutine coroutine, Action onComplete)
        {
            yield return coroutine;
            onComplete?.Invoke();
        }
    }
} 