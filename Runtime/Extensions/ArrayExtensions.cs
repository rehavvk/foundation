using System;

namespace Rehawk.Foundation.Extensions
{
    public static class ArrayExtensions
    {
        public static void Shuffle<T>(this T[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }
        }
        
        public static void Shuffle<T>(this T[] array, Random random)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                (array[i], array[j]) = (array[j], array[i]);
            }
        }

        public static bool HasIndex<T>(this T[] array, int index)
        {
            return index >= 0 && index < array.Length;
        }
        
        public static bool HasIndex<T>(this T[,] array, int x, int y)
        {
            return x >= 0 && x < array.GetLength(0) && y >= 0 && y < array.GetLength(1);
        }
        
        public static bool HasIndex<T>(this T[,,] array, int x, int y, int z)
        {
            return x >= 0 && x < array.GetLength(0) && y >= 0 && y < array.GetLength(1) && z >= 0 && z < array.GetLength(2);
        }
        
        public static bool TryGetValue<T>(this T[] array, int index, out T item)
        {
            if (array.HasIndex(index))
            {
                item = array[index];
                return true;
            }

            item = default;
            return false;
        }
    }
}