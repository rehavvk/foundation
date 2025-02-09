using System;
using UnityEngine;

namespace Rehawk.Foundation.Extensions
{
    public static class RandomExtensions
    {
        public static float Range(this System.Random random, float min, float max)
        {
            return (float)random.NextDouble() * (max - min) + min;
        }

        public static Vector3 GetNormalizedDirection(this System.Random random)
        {
            return random.GetRandomVector(-1f, 1f).normalized;
        }

        public static Quaternion GetRandomRotation(this System.Random random, Vector3 minEuler, Vector3 maxEuler)
        {
            return Quaternion.Euler(random.GetRandomEuler(minEuler, maxEuler));
        }

        public static Vector3 GetRandomEuler(this System.Random random, Vector3 minEuler, Vector3 maxEuler)
        {
            return random.GetRandomVector(minEuler, maxEuler);
        }

        public static Vector3 GetRandomVector(this System.Random random, Vector3 minVector, Vector3 maxVector)
        {
            return new Vector3(random.Range(minVector.x, maxVector.x), random.Range(minVector.y, maxVector.y), random.Range(minVector.z, maxVector.z));
        }

        public static int GetRandomSign(this System.Random random)
        {
            return (int)Mathf.Sign(random.Next(-1, 1));
        }

        public static Vector3 GetRandomVector(this System.Random random, float min, float max)
        {
            return new Vector3(random.Range(min, max), random.Range(min, max), random.Range(min, max));
        }
        
        public static Vector3 GetRandomPointInUnitSphere(this System.Random random)
        {
            double u = random.NextDouble();
            //Inverse of sphere volume growth curve (3/4PI * r^3)
            double w = Math.Pow(u, (double)1 / 3);

            Vector3 direction = GetNormalizedDirection(random);

            return direction * (float)w;
        }
    }
}