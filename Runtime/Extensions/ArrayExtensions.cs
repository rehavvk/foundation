using Random = UnityEngine.Random;

namespace Rehawk.Foundation.Extensions
{
    public static class ArrayExtensions
    {
        public static void Shuffle<T>(this T[] array)
        {
            for (int t = 0; t < array.Length; t++ )
            {
                T tmp = array[t];
                int r = Random.Range(t, array.Length);
                array[t] = array[r];
                array[r] = tmp;
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