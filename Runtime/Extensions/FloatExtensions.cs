using UnityEngine;

namespace Rehawk.Foundation.Extensions
{
    public static class FloatExtensions
    {
        public static float ClampAngle(this float angle, float min, float max)
        {
            if (angle < -360f)
            {
                angle += 360f;
            }

            if (angle > 360f)
            {
                angle -= 360f;
            }

            return Mathf.Clamp(angle, min, max);
        }
    }
}