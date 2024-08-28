using System.Collections.Generic;

namespace Rehawk.Foundation.Extensions
{
    public static class IEnumerableExtensions
    {
        public static int IndexOf<T>(this IEnumerable<T> collection, T elementToFind)
        {
            int i = 0;

            foreach (T element in collection)
            {
                if (Equals(element, elementToFind))
                    return i;

                i++;
            }

            return -1;
        }
    }
}