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
            int index = list.IndexOf(element);
            index = ((index + 1) % list.Count + list.Count) % list.Count;
            return list[index];
        }
        
        public static T GetPrevious<T>(this IList<T> list, T element)
        {
            int index = list.IndexOf(element);
            index = ((index - 1) % list.Count + list.Count) % list.Count;
            return list[index];
        }
    }
}