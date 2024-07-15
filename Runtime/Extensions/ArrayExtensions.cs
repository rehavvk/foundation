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
    }
}