using System;
using System.Collections.Generic;

namespace Rehawk.Foundation.Misc
{
    public static class ArrayUtility
    {
        public static void Add<T>(ref T[] array, T item)
        {
            Array.Resize(ref array, array.Length + 1);
            array[^1] = item;
        }
        
        public static void Remove<T>(ref T[] array, T item)
        {
            var objList = new List<T>(array);
            objList.Remove(item);
            array = objList.ToArray();
        }
    }
}