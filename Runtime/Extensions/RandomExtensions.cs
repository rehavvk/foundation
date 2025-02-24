using System;
using UnityEngine;

namespace Rehawk.Foundation.Extensions
{
    public static class RandomExtensions
    {
        public static float NextFloat(this System.Random random, float min, float max)
        {
            return (float)random.NextDouble() * (max - min) + min;
        }
        
        public static bool NextBool(this System.Random random)
        {
            return random.Next(0, 2) == 0;
        }

        public static int GetRandomSign(this System.Random random)
        {
            return (int)Mathf.Sign(random.Next(-1, 1));
        }

        public static Vector3 GetNormalizedDirection3(this System.Random random)
        {
            return random.GetRandomVector3(-1f, 1f).normalized;
        }

        public static Vector2 GetNormalizedDirection2(this System.Random random)
        {
            return random.GetRandomVector2(-1f, 1f).normalized;
        }

        public static Quaternion GetRandomRotation(this System.Random random, Vector3 minEuler, Vector3 maxEuler)
        {
            return Quaternion.Euler(random.GetRandomEuler(minEuler, maxEuler));
        }

        public static Vector3 GetRandomEuler(this System.Random random, Vector3 minEuler, Vector3 maxEuler)
        {
            return random.GetRandomVector3(minEuler, maxEuler);
        }

        public static Vector3 GetRandomVector3(this System.Random random, Vector3 minVector, Vector3 maxVector)
        {
            return new Vector3(random.NextFloat(minVector.x, maxVector.x), random.NextFloat(minVector.y, maxVector.y), random.NextFloat(minVector.z, maxVector.z));
        }

        public static Vector3 GetRandomVector3(this System.Random random, float min, float max)
        {
            return new Vector3(random.NextFloat(min, max), random.NextFloat(min, max), random.NextFloat(min, max));
        }

        public static Vector2 GetRandomVector2(this System.Random random, Vector2 minVector, Vector2 maxVector)
        {
            return new Vector2(random.NextFloat(minVector.x, maxVector.x), random.NextFloat(minVector.y, maxVector.y));
        }

        public static Vector2 GetRandomVector2(this System.Random random, float min, float max)
        {
            return new Vector2(random.NextFloat(min, max), random.NextFloat(min, max));
        }

        public static Vector3 GetRandomPointInUnitSphere(this System.Random random)
        {
            double u = random.NextDouble();
            //Inverse of sphere volume growth curve (3/4PI * r^3)
            double w = Math.Pow(u, (double) 1 / 3);

            Vector3 direction = GetNormalizedDirection3(random);

            return direction * (float) w;
        }
        
        public static Vector2 GetRandomPointInUnitCircle(this System.Random random)
        {
            double u = random.NextDouble();
            double w = Math.Sqrt(u);

            Vector3 direction = GetNormalizedDirection2(random);

            return direction * (float) w;
        }
    }
}