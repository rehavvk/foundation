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
        
        public static T[,] GetSlice<T>(ref T[,] array, int x, int y, int width, int height)
        {
            // Validate input parameters
            if (x < 0 || y < 0 || x + width > array.GetLength(0) || y + height > array.GetLength(1))
            {
                throw new ArgumentOutOfRangeException("Invalid slice coordinates or dimensions.");
            }

            // Create a new slice array with the specified dimensions
            T[,] slice = new T[width, height];

            // Copy slice values from the original array
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    slice[i, j] = array[x + i, y + j];
                }
            }

            return slice;
        }
        
        public static void ApplySlice<T>(ref T[,] array, T[,] slice, int x, int y, int width, int height)
        {
            // Validate input parameters
            if (x < 0 || y < 0 || x + width > array.GetLength(0) || y + height > array.GetLength(1))
            {
                throw new ArgumentOutOfRangeException("Invalid slice coordinates or dimensions.");
            }

            if (slice.GetLength(0) != width || slice.GetLength(1) != height)
            {
                throw new ArgumentException("Slice dimensions must match specified width and height.");
            }

            // Copy slice values back to original array
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    array[x + i, y + j] = slice[i, j];
                }
            }
        }
        
        public static T[,,] GetSlice<T>(ref T[,,] array, int x, int y, int z, int width, int height, int depth)
        {
            // Validate input parameters
            if (x < 0 || y < 0 || z < 0 || x + width > array.GetLength(0) || y + height > array.GetLength(1) || z + depth > array.GetLength(2))
            {
                throw new ArgumentOutOfRangeException("Invalid slice coordinates or dimensions.");
            }

            // Create a new slice array with the specified dimensions
            T[,,] slice = new T[width, height, depth];

            // Copy slice values from the original array
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    for (int k = 0; k < depth; k++)
                    {
                        slice[i, j, k] = array[x + i, y + j, z + k];
                    }
                }
            }

            return slice;
        }
        
        public static void ApplySlice<T>(ref T[,,] array, T[,,] slice, int x, int y, int z, int width, int height, int depth)
        {
            // Validate input parameters
            if (x < 0 || y < 0 || z < 0 || x + width > array.GetLength(0) || y + height > array.GetLength(1) || z + depth > array.GetLength(2))
            {
                throw new ArgumentOutOfRangeException("Invalid slice coordinates or dimensions.");
            }

            if (slice.GetLength(0) != width || slice.GetLength(1) != height || slice.GetLength(2) != depth)
            {
                throw new ArgumentException("Slice dimensions must match specified width, height, and depth.");
            }

            // Copy slice values back to original array
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    for (int k = 0; k < depth; k++)
                    {
                        array[x + i, y + j, z + k] = slice[i, j, k];
                    }
                }
            }
        }
    }
}