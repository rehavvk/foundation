using System;
using System.Collections.Generic;

namespace Rehawk.Foundation.Extensions
{
    public static class IListExtensions
    {
        public static T GetRandom<T>(this IReadOnlyList<T> list)
        {
            int index = UnityEngine.Random.Range(0, list.Count);

            if (list.TryGetValue(index, out T result))
            {
                return result;
            }
            
            return default;
        }
        
        public static T GetRandom<T>(this IReadOnlyList<T> list, Random random)
        {
            int index = random.Next(0, list.Count);

            if (list.TryGetValue(index, out T result))
            {
                return result;
            }
            
            return default;
        }
        
        public static T GetNext<T>(this IReadOnlyList<T> list, T element)
        {
            int index = 0;
            if (element != null)
            {
                index = list.IndexOf(element);
            }
            
            index = ((index + 1) % list.Count + list.Count) % list.Count;
            return list[index];
        }
        
        public static T GetPrevious<T>(this IReadOnlyList<T> list, T element)
        {
            int index = 0;
            if (element != null)
            {
                index = list.IndexOf(element);
            }

            index = ((index - 1) % list.Count + list.Count) % list.Count;
            return list[index];
        }
        
        public static bool HasIndex<T>(this IReadOnlyList<T> list, int index)
        {
            return index >= 0 && index < list.Count;
        }
        
        public static bool TryGetValue<T>(this IReadOnlyList<T> list, int index, out T item)
        {
            if (list.HasIndex(index))
            {
                item = list[index];
                return true;
            }

            item = default;
            return false;
        }

        public static void AddRange<T>(this IList<T> initial, IEnumerable<T> collection)
        {
            if (initial == null)
            {
                throw new ArgumentNullException(nameof(initial));
            }

            if (collection == null)
            {
                return;
            }

            foreach (T value in collection)
            {
                initial.Add(value);
            }
        }
    }
}