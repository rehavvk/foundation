using UnityEngine;
using Random = UnityEngine.Random;

namespace Rehawk.Foundation.Extensions
{
    public static class VectorExtensions
    {
        public static float GetRandom(this Vector2 vector)
        {
            return Random.Range(vector.x, vector.y);
        }
        
        public static int GetRandom(this Vector2Int vector)
        {
            return Random.Range(vector.x, vector.y);
        }
        
        public static Vector3 GetClosestPointOnFiniteLine(this Vector3 point, Vector3 start, Vector3 end)
        {
            Vector3 direction = end - start;
            float length = direction.magnitude;
            direction.Normalize();
            float projectLength = Mathf.Clamp(Vector3.Dot(point - start, direction), 0f, length);
            return start + direction * projectLength;
        }
        
        public static Vector3 GetClosestPointOnInfiniteLine(this Vector3 point, Vector3 start, Vector3 direction)
        {
            return start - point - Vector3.Dot(start - point, direction) * direction;
        }
        
        public static Vector3 GetClosestPointInSphere(this Vector3 vector, Vector3 center, float radius)
        {
            Vector3 v = vector - center;
            v = Vector3.ClampMagnitude(v, radius);
            return center + v;
        }
        
        public static Vector3 GetClosestPointOnSphereSkin(this Vector3 vector, Vector3 center, float radius)
        {
            Vector3 v = vector - center;
            v.Normalize();
            return center + v * radius;
        }
        
        public static float GetFlattenDistance(this Vector3 vector, Vector3 to)
        {
            return Mathf.Abs(Vector3.Distance(new Vector3(vector.x, 0, vector.z), new Vector3(to.x, 0, to.z)));
        }
    }
}