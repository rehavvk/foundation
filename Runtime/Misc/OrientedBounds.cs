using System;
using System.Globalization;
using UnityEngine;

namespace Rehawk.Foundation.Misc
{
    [Serializable]
    public struct OrientedBounds : IEquatable<OrientedBounds>, IFormattable
    {
        private Vector3 center;
        private Vector3 size;
        private Quaternion rotation;
        
        public Vector3 Center
        {
            get => center;
            set => center = value;
        }
        
        public Vector3 Size
        {
            get => size;
            set => size = value;
        }
        
        public Quaternion Rotation
        {
            get => rotation;
            set => rotation = value;
        }
        
        public Vector3 Extents
        {
            get => size * 0.5f;
            set => size = value * 2f;
        }
        
        public OrientedBounds(Vector3 center, Quaternion rotation, Vector3 size)
        {
            this.center = center;
            this.size = size;
            this.rotation = rotation;
        }

        public OrientedBounds(Vector3 center, Vector3 size) : this(center, Quaternion.identity, size) { }

        public OrientedBounds(Bounds bounds)
        {
            center = bounds.center;
            size = bounds.size;
            rotation = Quaternion.identity;
        }
        
        public OrientedBounds(OrientedBounds bounds)
        {
            center = bounds.Center;
            size = bounds.Size;
            rotation = bounds.Rotation;
        }
        
        public Vector3[] GetCorners()
        {
            var corners = new Vector3[8];
            GetCorners(corners);
            return corners;
        }

        public bool Contains(Vector3 point)
        {
            Vector3 localPoint = Quaternion.Inverse(rotation) * (point - center);
            Vector3 extents = Extents;

            return Mathf.Abs(localPoint.x) <= extents.x &&
                   Mathf.Abs(localPoint.y) <= extents.y &&
                   Mathf.Abs(localPoint.z) <= extents.z;
        }

        public Vector3 ClosestPoint(Vector3 point)
        {
            Vector3 localPoint = Quaternion.Inverse(rotation) * (point - center);

            var clampedLocalPoint = new Vector3(
                Mathf.Clamp(localPoint.x, -Extents.x, Extents.x),
                Mathf.Clamp(localPoint.y, -Extents.y, Extents.y),
                Mathf.Clamp(localPoint.z, -Extents.z, Extents.z)
            );

            return center + rotation * clampedLocalPoint;
        }

        public void Encapsulate(Vector3 point)
        {
            Vector3 localPoint = Quaternion.Inverse(rotation) * (point - center);

            Vector3 localMin = -Extents;
            Vector3 localMax = Extents;

            localMin = Vector3.Min(localMin, localPoint);
            localMax = Vector3.Max(localMax, localPoint);

            size = localMax - localMin;
            center += rotation * ((localMin + localMax) * 0.5f);
        }

        public void Encapsulate(OrientedBounds other)
        {
            Span<Vector3> ownCorners = stackalloc Vector3[8];
            Span<Vector3> otherCorners = stackalloc Vector3[8];
            GetCorners(ownCorners);
            other.GetCorners(otherCorners);

            Vector3 worldMin = ownCorners[0];
            Vector3 worldMax = ownCorners[0];

            for (int i = 0; i < ownCorners.Length; i++)
            {
                Vector3 corner = ownCorners[i];
                worldMin = Vector3.Min(worldMin, corner);
                worldMax = Vector3.Max(worldMax, corner);
            }

            for (int i = 0; i < otherCorners.Length; i++)
            {
                Vector3 corner = otherCorners[i];
                worldMin = Vector3.Min(worldMin, corner);
                worldMax = Vector3.Max(worldMax, corner);
            }

            center = (worldMin + worldMax) * 0.5f;
            size = worldMax - worldMin;
            // Reset rotation to world-aligned
            rotation = Quaternion.identity;
        }

        public void Encapsulate(Bounds bounds)
        {
            Span<Vector3> corners = stackalloc Vector3[8];
            var boundsMin = bounds.min;
            var boundsMax = bounds.max;

            corners[0] = new Vector3(boundsMin.x, boundsMin.y, boundsMin.z);
            corners[1] = new Vector3(boundsMax.x, boundsMin.y, boundsMin.z);
            corners[2] = new Vector3(boundsMin.x, boundsMax.y, boundsMin.z);
            corners[3] = new Vector3(boundsMax.x, boundsMax.y, boundsMin.z);
            corners[4] = new Vector3(boundsMin.x, boundsMin.y, boundsMax.z);
            corners[5] = new Vector3(boundsMax.x, boundsMin.y, boundsMax.z);
            corners[6] = new Vector3(boundsMin.x, boundsMax.y, boundsMax.z);
            corners[7] = new Vector3(boundsMax.x, boundsMax.y, boundsMax.z);

            for (int i = 0; i < corners.Length; i++)
            {
                var corner = corners[i];
                Encapsulate(corner);
            }
        }

        /// <summary>
        ///     Checks if another OrientedBounds intersects with this one.
        ///     Uses the Separating Axis Theorem (SAT) for OBB-OBB intersection.
        /// </summary>
        public bool Intersects(OrientedBounds other)
        {
            Vector3[] axes = GetAxes();
            Vector3[] otherAxes = other.GetAxes();

            // Check all possible separating axes (15 total)
            for (int i = 0; i < 3; i++)
            {
                if (IsSeparatingAxis(axes[i], this, other))
                    return false;

                if (IsSeparatingAxis(otherAxes[i], this, other))
                    return false;

                for (int j = 0; j < 3; j++)
                {
                    Vector3 crossAxis = Vector3.Cross(axes[i], otherAxes[j]);

                    if (!(crossAxis.sqrMagnitude > 1e-6f))
                        continue; // Ignore near-zero vectors

                    if (IsSeparatingAxis(crossAxis, this, other))
                        return false;
                }
            }

            return true;
        }

        private Vector3[] GetAxes()
        {
            return new[]
            {
                rotation * Vector3.right,
                rotation * Vector3.up,
                rotation * Vector3.forward
            };
        }

        private void GetCorners(Span<Vector3> corners)
        {
            if (corners.Length != 8)
            {
                throw new ArgumentException("The corners array must contain 8 corners.", nameof(corners));
            }

            Vector3 extents = Extents;
            corners[0] = center + rotation * new Vector3(-extents.x, -extents.y, -extents.z);
            corners[1] = center + rotation * new Vector3(extents.x, -extents.y, -extents.z);
            corners[2] = center + rotation * new Vector3(-extents.x, extents.y, -extents.z);
            corners[3] = center + rotation * new Vector3(extents.x, extents.y, -extents.z);
            corners[4] = center + rotation * new Vector3(-extents.x, -extents.y, extents.z);
            corners[5] = center + rotation * new Vector3(extents.x, -extents.y, extents.z);
            corners[6] = center + rotation * new Vector3(-extents.x, extents.y, extents.z);
            corners[7] = center + rotation * new Vector3(extents.x, extents.y, extents.z);
        }
        
        public bool Equals(OrientedBounds other)
        {
            return center.Equals(other.center) &&
                   size.Equals(other.size) &&
                   rotation.Equals(other.rotation);
        }

        public override bool Equals(object obj)
        {
            return obj is OrientedBounds other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(center, size, rotation);
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format(formatProvider,
                "OrientedBounds(Center: {0}, Size: {1}, Rotation: {2})",
                center.ToString(format, formatProvider),
                size.ToString(format, formatProvider),
                rotation.ToString(format, formatProvider));
        }

        public override string ToString()
        {
            return ToString(null, CultureInfo.InvariantCulture);
        }

        public static bool operator ==(OrientedBounds left, OrientedBounds right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(OrientedBounds left, OrientedBounds right)
        {
            return !left.Equals(right);
        }
        
        /// <summary>
        ///     Checks if a given axis separates the two bounding boxes.
        /// </summary>
        private static bool IsSeparatingAxis(Vector3 axis, OrientedBounds a, OrientedBounds b)
        {
            if (axis == Vector3.zero)
                return false;

            float aRadius = ProjectExtentsOntoAxis(axis, a);
            float bRadius = ProjectExtentsOntoAxis(axis, b);
            float distance = Mathf.Abs(Vector3.Dot(axis, b.center - a.center));

            return distance > aRadius + bRadius;
        }

        /// <summary>
        ///     Projects the extents of a rotated bounding box onto a given axis.
        /// </summary>
        private static float ProjectExtentsOntoAxis(Vector3 axis, OrientedBounds bounds)
        {
            Vector3[] axes = bounds.GetAxes();
            Vector3 extents = bounds.Extents;

            return Mathf.Abs(Vector3.Dot(axis, axes[0]) * extents.x) +
                   Mathf.Abs(Vector3.Dot(axis, axes[1]) * extents.y) +
                   Mathf.Abs(Vector3.Dot(axis, axes[2]) * extents.z);
        }
    }
}