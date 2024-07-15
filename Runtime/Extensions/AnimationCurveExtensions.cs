using UnityEngine;

namespace Rehawk.Foundation.Extensions
{
    public static class AnimationCurveExtensions
    {
        public static float TimeFromValue(this AnimationCurve curve, float value, float precision = 1e-6f)
        {
            float minTime = curve.keys[0].time;
            float maxTime = curve.keys[curve.keys.Length - 1].time;
            float best = (maxTime + minTime) / 2;
            float bestVal = curve.Evaluate(best);
            int it = 0;
            const int maxIt = 1000;
            float sign = Mathf.Sign(curve.keys[curve.keys.Length - 1].value - curve.keys[0].value);
            while (it < maxIt && Mathf.Abs(minTime - maxTime) > precision)
            {
                if ((bestVal - value) * sign > 0)
                {
                    maxTime = best;
                }
                else
                {
                    minTime = best;
                }

                best = (maxTime + minTime) / 2;
                bestVal = curve.Evaluate(best);
                it++;
            }

            return best;
        }

    }
}