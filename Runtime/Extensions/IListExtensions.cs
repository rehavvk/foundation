using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Rehawk.Foundation.Extensions
{
    public static class IListExtensions
    {
        public static T GetRandom<T>(this IList<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }
        
        public static T GetNext<T>(this IList<T> list, T element)
        {
            int index = 0;
            if (element != null)
            {
                index = list.IndexOf(element);
            }
            
            index = ((index + 1) % list.Count + list.Count) % list.Count;
            return list[index];
        }
        
        public static T GetPrevious<T>(this IList<T> list, T element)
        {
            int index = 0;
            if (element != null)
            {
                index = list.IndexOf(element);
            }

            index = ((index - 1) % list.Count + list.Count) % list.Count;
            return list[index];
        }
        
        public static bool HasIndex<T>(this IList<T> list, int index)
        {
            return index >= 0 && index < list.Count;
        }
        
        public static bool TryGetValue<T>(this IList<T> list, int index, out T item)
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