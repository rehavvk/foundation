using System;
using UnityEngine;
using UnityEngine.Pool;

namespace Rehawk.Foundation.Misc
{
    public class PolygonXZ : MonoBehaviour
    {
        [SerializeField] private Vector2[] points = Array.Empty<Vector2>();

        public Vector2[] Points
        {
            get => points;
            set => points = value;
        }

        private void OnDrawGizmos()
        {
            if (points == null || points.Length < 2)
                return;

            Gizmos.color = Color.red;

            for (int i = 0; i < points.Length; i++)
            {
                int j = (i + 1) % points.Length;
                Gizmos.DrawLine(transform.TransformPoint(new Vector3(points[i].x, 0, points[i].y)),
                                transform.TransformPoint(new Vector3(points[j].x, 0, points[j].y)));
            }
        }
        
        public bool Contains(Vector3 point)
        {
            Vector3 localPoint = transform.InverseTransformPoint(point);
            return Contains(localPoint, points);
        }

        public Vector3 ClosestPoint(Vector3 point)
        {
            Vector3 localPoint = transform.InverseTransformPoint(point);
            Vector3 localClosestPoint = ClosestPoint(localPoint, points);
            return transform.TransformPoint(localClosestPoint);
        }

        public Vector3 ClosestPointOnEdge(Vector3 point)
        {
            Vector3 localPoint = transform.InverseTransformPoint(point);
            Vector3 localClosestPoint = ClosestPointOnEdge(localPoint, points);
            return transform.TransformPoint(localClosestPoint);
        }

        public Vector3 GetLineIntersection(Vector3 start, Vector3 end)
        {
            Vector3 localStart = transform.InverseTransformPoint(start);
            Vector3 localEnd = transform.InverseTransformPoint(end);

            var p1 = new Vector2(localStart.x, localStart.z);
            var p2 = new Vector2(localEnd.x, localEnd.z);

            for (int i = 0; i < points.Length; i++)
            {
                int j = (i + 1) % points.Length;

                Vector2 p3 = points[i];
                Vector2 p4 = points[j];

                if (TryGetLineIntersection(p1, p2, p3, p4, out Vector2 intersection))
                {
                    return transform.TransformPoint(new Vector3(intersection.x, localStart.y, intersection.y));
                }
            }

            return Vector3.zero;
        }

        public static bool Contains(Vector3 point, Vector3[] points)
        {
            var point2D = new Vector2(point.x, point.z);
            bool inside = false;

            for (int i = 0, j = points.Length - 1; i < points.Length; j = i++)
            {
                var pi = new Vector2(points[i].x, points[i].z);
                var pj = new Vector2(points[j].x, points[j].z);

                if ((pi.y > point2D.y) != (pj.y > point2D.y) &&
                    (point2D.x < (pj.x - pi.x) * (point2D.y - pi.y) / (pj.y - pi.y) + pi.x))
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        public static bool Contains(Vector3 point, Vector2[] points)
        {
            var point2D = new Vector2(point.x, point.z);
            bool inside = false;

            for (int i = 0, j = points.Length - 1; i < points.Length; j = i++)
            {
                var pi = points[i];
                var pj = points[j];

                if ((pi.y > point2D.y) != (pj.y > point2D.y) &&
                    (point2D.x < (pj.x - pi.x) * (point2D.y - pi.y) / (pj.y - pi.y) + pi.x))
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        public static Vector3 ClosestPoint(Vector3 point, Vector3[] points)
        {
            if (Contains(point, points))
            {
                return point;
            }

            return ClosestPointOnEdge(point, points);
        }

        public static Vector3 ClosestPoint(Vector3 point, Vector2[] points)
        {
            if (Contains(point, points))
            {
                return point;
            }

            return ClosestPointOnEdge(point, points);
        }

        public static Vector3 ClosestPointOnEdge(Vector3 point, Vector3[] points)
        {
            var point2D = new Vector2(point.x, point.z);
            Vector2 closestPoint = new Vector2(points[0].x, points[0].z);
            float minDistance = float.MaxValue;

            for (int i = 0; i < points.Length; i++)
            {
                int j = (i + 1) % points.Length;
                Vector2 pi = new Vector2(points[i].x, points[i].z);
                Vector2 pj = new Vector2(points[j].x, points[j].z);
                Vector2 closestPointOnEdge = ClosestPointOnLineSegment(pi, pj, point2D);
                float distance = (closestPointOnEdge - point2D).sqrMagnitude;

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPoint = closestPointOnEdge;
                }
            }

            return new Vector3(closestPoint.x, point.y, closestPoint.y);
        }

        public static Vector3 ClosestPointOnEdge(Vector3 point, Vector2[] points)
        {
            var point2D = new Vector2(point.x, point.z);
            Vector2 closestPoint = points[0];
            float minDistance = float.MaxValue;

            for (int i = 0; i < points.Length; i++)
            {
                int j = (i + 1) % points.Length;
                Vector2 pi = points[i];
                Vector2 pj = points[j];
                Vector2 closestPointOnEdge = ClosestPointOnLineSegment(pi, pj, point2D);
                float distance = (closestPointOnEdge - point2D).sqrMagnitude;

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPoint = closestPointOnEdge;
                }
            }

            return new Vector3(closestPoint.x, point.y, closestPoint.y);
        }
        
        public static Vector2 ClosestPointOnLineSegment(Vector2 a, Vector2 b, Vector2 point)
        {
            Vector2 ab = b - a;
            float t = Vector2.Dot(point - a, ab) / Vector2.Dot(ab, ab);
            t = Mathf.Clamp01(t);
            return a + t * ab;
        }

        /// <summary>
        /// Smooths the polygon by adding new points between each pair of points.
        /// Chaikin's Algorithm: https://en.wikipedia.org/wiki/Chaikin%27s_algorithm
        /// </summary>
        public static Vector2[] SmoothPolygonChaikin(Vector2[] polygon, int iterations)
        {
            var smoothedPolygon = ListPool<Vector2>.Get();
            smoothedPolygon.AddRange(polygon);

            var newPolygon = ListPool<Vector2>.Get();

            for (int i = 0; i < iterations; i++)
            {
                newPolygon.Clear();
                
                for (int j = 0; j < smoothedPolygon.Count; j++)
                {
                    Vector2 p0 = smoothedPolygon[j];
                    Vector2 p1 = smoothedPolygon[(j + 1) % smoothedPolygon.Count];

                    Vector2 q = 0.75f * p0 + 0.25f * p1;
                    Vector2 r = 0.25f * p0 + 0.75f * p1;

                    newPolygon.Add(q);
                    newPolygon.Add(r);
                }

                smoothedPolygon.Clear();
                smoothedPolygon.AddRange(newPolygon);
            }
            
            Vector2[] result = smoothedPolygon.ToArray();
            
            ListPool<Vector2>.Release(smoothedPolygon);
            ListPool<Vector2>.Release(newPolygon);

            return result;
        }
        
        /// <summary>
        /// Smooths the polygon by adding new points between each pair of points.
        /// Polynomial Approximation with Exponential Kernel (PAEK) Algorithm: https://www.researchgate.net/publication/220678258_Polynomial_Approximation_with_Exponential_Kernel
        /// </summary>
        public static Vector2[] SmoothPolygonPAEK(Vector2[] polygon, float tolerance)
        {
            if (polygon == null || polygon.Length < 3)
            {
                return polygon;
            }

            var smoothedPolygon = ListPool<Vector2>.Get();
            int pointCount = polygon.Length;

            for (int i = 0; i < pointCount; i++)
            {
                Vector2 currentPoint = polygon[i];
                Vector2 previousPoint = polygon[(i - 1 + pointCount) % pointCount]; // Wrap around for the first point
                Vector2 nextPoint = polygon[(i + 1) % pointCount]; // Wrap around for the last point

                // Calculate the direction vectors
                Vector2 prevDir = (currentPoint - previousPoint).normalized;
                Vector2 nextDir = (nextPoint - currentPoint).normalized;

                // Calculate the angle between the directions
                float angle = Mathf.Acos(Mathf.Clamp(Vector2.Dot(prevDir, nextDir), -1f, 1f));

                // If the angle is sharp enough, add the original point
                if (angle > Mathf.PI * tolerance / 180f || tolerance == 0f) // Tolerance in degrees
                {
                    smoothedPolygon.Add(currentPoint);
                }
                else
                {
                    // Calculate the midpoint of the line segment between the previous and next points
                    Vector2 midPoint = (previousPoint + nextPoint) * 0.5f;

                    // Calculate the offset vector
                    Vector2 offset = (currentPoint - midPoint);

                    // Calculate the new point based on the offset and tolerance
                    Vector2 newPoint = currentPoint + offset * (1 - angle / Mathf.PI);

                    smoothedPolygon.Add(newPoint);

                }
            }

            Vector2[] result = smoothedPolygon.ToArray();

            ListPool<Vector2>.Release(smoothedPolygon);
            
            return result;
        }

        private static bool TryGetLineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 intersection)
        {
            float topX = (p1.x * p2.y - p2.x * p1.y) * (p3.x - p4.x) - (p3.x * p4.y - p4.x * p3.y) * (p1.x - p2.x);
            float topY = (p1.x * p2.y - p2.x * p1.y) * (p3.y - p4.y) - (p3.x * p4.y - p4.x * p3.y) * (p1.y - p2.y);
            float bottom = (p1.x - p2.x) * (p3.y - p4.y) - (p1.y - p2.y) * (p3.x - p4.x);

            if (Mathf.Abs(bottom) <= 0)
            {
                intersection = Vector2.zero;
                return false;
            }

            float pX = topX / bottom;
            float pY = topY / bottom;

            intersection = new Vector3(pX, pY, 0f);

            bool isInBoundsLine1 = IsIntersectionInBounds(p1, p2, intersection);
            bool isInBoundsLine2 = IsIntersectionInBounds(p3, p4, intersection);

            if (!isInBoundsLine1 || !isInBoundsLine2)
            {
                return false;
            }

            return true;
        }

        private static bool IsIntersectionInBounds(Vector3 lineStart, Vector3 lineEnd, Vector3 intersection)
        {
            float distAC = Vector3.Distance(lineStart, intersection);
            float distBC = Vector3.Distance(lineEnd, intersection);
            float distAB = Vector3.Distance(lineStart, lineEnd);
            if (Mathf.Abs(distAC + distBC - distAB) > 0.001f)
            {
                return false;
            }

            return true;
        }
    }
}