using UnityEngine;

namespace Rehawk.Foundation.Misc
{
    public static class Probability
    {
        public static bool Check(float probability)
        {
            return Random.value < probability;
        }
    }
}