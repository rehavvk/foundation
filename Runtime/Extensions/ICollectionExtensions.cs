using System.Collections.Generic;

namespace Rehawk.Foundation.Extensions
{
    public static class ICollectionExtensions
    {
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            return collection == null || collection.Count <= 0;
        }
    }
}