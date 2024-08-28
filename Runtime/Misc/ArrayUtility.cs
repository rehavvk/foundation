using System;
using System.Collections.Generic;
using Rehawk.Foundation.Extensions;

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
            var slice = new T[width, height];

            int sliceX = 0;
            int sliceY = 0;
            
            for (int xIndex = x; xIndex < x + width; xIndex++)
            {
                for (int yIndex = y; yIndex < y + height; yIndex++)
                {
                    if (array.HasIndex(xIndex, yIndex) && slice.HasIndex(sliceX, sliceY))
                    {
                        slice[sliceX, sliceY] = array[xIndex, yIndex];
                    }

                    sliceY++;
                }

                sliceY = 0;
                sliceX++;
            }

            return slice;
        }
        
        public static void ApplySlice<T>(ref T[,] array, T[,] slice, int x, int y, int width, int height)
        {
            int sliceX = 0;
            int sliceY = 0;
            
            for (int xIndex = x; xIndex < x + width; xIndex++)
            {
                for (int yIndex = y; yIndex < y + height; yIndex++)
                {
                    if (array.HasIndex(xIndex, yIndex) && slice.HasIndex(sliceX, sliceY))
                    {
                        array[xIndex, yIndex] = slice[sliceX, sliceY];
                    }
                    
                    sliceY++;
                }

                sliceY = 0;
                sliceX++;
            }
        }
        
        public static T[,,] GetSlice<T>(ref T[,,] array, int x, int y, int z, int width, int height, int depth)
        {
            var slice = new T[width, height, depth];

            int sliceX = 0;
            int sliceY = 0;
            int sliceZ = 0;
            
            for (int xIndex = x; xIndex < x + width; xIndex++)
            {
                for (int yIndex = y; yIndex < y + height; yIndex++)
                {
                    for (int zIndex = z; zIndex < z + depth; zIndex++)
                    {
                        if (array.HasIndex(xIndex, yIndex, zIndex) && slice.HasIndex(sliceX, sliceY, sliceZ))
                        {
                            slice[sliceX, sliceY, sliceZ] = array[xIndex, yIndex, zIndex];
                        }

                        sliceZ++;
                    }
                    
                    sliceZ = 0;
                    sliceY++;
                }
                
                sliceY = 0;
                sliceX++;
            }

            return slice;
        }
        
        public static void ApplySlice<T>(ref T[,,] array, T[,,] slice, int x, int y, int z, int width, int height, int depth)
        {
            int sliceX = 0;
            int sliceY = 0;
            int sliceZ = 0;
            
            for (int xIndex = x; xIndex < x + width; xIndex++)
            {
                for (int yIndex = y; yIndex < y + height; yIndex++)
                {
                    for (int zIndex = z; zIndex < z + depth; zIndex++)
                    {
                        if (array.HasIndex(xIndex, yIndex, zIndex) && slice.HasIndex(sliceX, sliceY, sliceZ))
                        {
                            array[xIndex, yIndex, zIndex] = slice[sliceX, sliceY, sliceZ];
                        }

                        sliceZ++;
                    }
                    
                    sliceZ = 0;
                    sliceY++;
                }
                
                sliceY = 0;
                sliceX++;
            }
        }
    }
}