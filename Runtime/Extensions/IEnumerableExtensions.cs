using System.Collections.Generic;
using System.Linq;

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
    }
}