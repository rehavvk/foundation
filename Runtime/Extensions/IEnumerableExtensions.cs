using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rehawk.Foundation.Extensions
{
    public static class IEnumerableExtensions
    {
        public static int IndexOf<T>(this IEnumerable<T> enumerable, T elementToFind)
        {
            int i = 0;

            foreach (T element in enumerable)
            {
                if (Equals(element, elementToFind))
                    return i;

                i++;
            }

            return -1;
        }
        
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }
        
        public static T GetRandom<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.OrderBy(_ => Random.value)
                             .FirstOrDefault();
        }
        
        public static T GetRandom<T>(this IEnumerable<T> enumerable, System.Random random)
        {
            return enumerable.OrderBy(_ => random.Next())
                             .FirstOrDefault();
        }
    }
}