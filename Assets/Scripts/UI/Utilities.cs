using UnityEngine;

namespace TequilaSunrise.UI.Utilities
{
    /// <summary>
    /// Utility class for UI components
    /// </summary>
    public static class InputEvent
    {
        public const string JoystickMoved = "JOYSTICK_MOVED";
        public const string ButtonPressed = "BUTTON_PRESSED";
        public const string ButtonReleased = "BUTTON_RELEASED";
    }

    /// <summary>
    /// Utility functions for UI animations and transitions
    /// </summary>
    public static class UIAnimationUtility
    {
        public static void ScaleUp(Transform target, float duration = 0.2f, float scale = 1.1f)
        {
            // Animation scale up implementation can be added later
        }

        public static void ScaleDown(Transform target, float duration = 0.2f, float scale = 1.0f)
        {
            // Animation scale down implementation can be added later
        }

        public static void FadeIn(CanvasGroup canvasGroup, float duration = 0.3f)
        {
            // Animation fade in implementation can be added later
        }

        public static void FadeOut(CanvasGroup canvasGroup, float duration = 0.3f)
        {
            // Animation fade out implementation can be added later
        }
    }
}