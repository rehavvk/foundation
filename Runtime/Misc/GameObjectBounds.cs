using Rehawk.Foundation.Extensions;
using UnityEngine;

namespace Rehawk.Foundation.Misc
{
    [DisallowMultipleComponent]
    public class GameObjectBounds : MonoBehaviour
    {
        [SerializeField] private Vector3 offset = new Vector3(0, 0, 0);
        [SerializeField] private Vector3 size = new Vector3(1, 1, 1);
        
        public Vector3 Offset => offset;
        public Vector3 Size => size;
        
        private void Reset()
        {
            ResetToRendererBounds();
        }
        
        /// <summary>
        /// Returns the local bounds.
        /// </summary>
        public Bounds GetLocalBounds()
        {
            return new Bounds(offset, size);
        }
              
        /// <summary>
        /// Returns the bounds in world space.
        /// </summary>  
        public Bounds GetBounds()
        {
            Vector3[] corners = GetCorners();

            var bounds = new Bounds(corners[0], Vector3.zero);
            for (int i = 1; i < corners.Length; i++)
            {
                var corner = corners[i];
                bounds.Encapsulate(corner);
            }

            return bounds;
        }

        /// <summary>
        /// Returns the bounds applied to the given parent transform matrix.
        /// </summary>
        public Bounds GetBounds(Matrix4x4 matrix)
        {
            Vector3[] corners = GetCorners(matrix);

            var bounds = new Bounds(corners[0], Vector3.zero);
            for (int i = 1; i < corners.Length; i++)
            {
                Vector3 corner = corners[i];
                bounds.Encapsulate(corner);
            }

            return bounds;
        }
        
        /// <summary>
        /// Returns the bounds with rotation information in world space.
        /// </summary>
        public OrientedBounds GetOrientedBounds()
        {
            return GetOrientedBounds(transform.localToWorldMatrix);
        }

        /// <summary>
        /// Returns the bounds with rotation information applied to the given parent transform matrix.
        /// </summary>
        public OrientedBounds GetOrientedBounds(Matrix4x4 matrix)
        {
            var corners = GetCorners(matrix);

            var bounds = new OrientedBounds(corners[0], matrix.rotation, Vector3.zero);
            for (int i = 1; i < corners.Length; i++)
            {
                var corner = corners[i];
                bounds.Encapsulate(corner);
            }

            return bounds;
        }

        /// <summary>
        /// Gets the eight corners of the bounds in world space.
        /// </summary>
        public Vector3[] GetCorners()
        {
            return GetCorners(transform.localToWorldMatrix);
        }
                
        /// <summary>
        /// Gets the eight corners of the bounds applied to the given parent transform matrix.
        /// </summary>
        public Vector3[] GetCorners(Matrix4x4 matrix)
        {
            Vector3 halfSize = size * 0.5f;

            return new[]
            {
                matrix.MultiplyPoint3x4(offset + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z)),
                matrix.MultiplyPoint3x4(offset + new Vector3(-halfSize.x, -halfSize.y, halfSize.z)),
                matrix.MultiplyPoint3x4(offset + new Vector3(-halfSize.x, halfSize.y, -halfSize.z)),
                matrix.MultiplyPoint3x4(offset + new Vector3(-halfSize.x, halfSize.y, halfSize.z)),
                matrix.MultiplyPoint3x4(offset + new Vector3(halfSize.x, -halfSize.y, -halfSize.z)),
                matrix.MultiplyPoint3x4(offset + new Vector3(halfSize.x, -halfSize.y, halfSize.z)),
                matrix.MultiplyPoint3x4(offset + new Vector3(halfSize.x, halfSize.y, -halfSize.z)),
                matrix.MultiplyPoint3x4(offset + new Vector3(halfSize.x, halfSize.y, halfSize.z))
            };
        }

        /// <summary>
        /// Resets the bounds of the object to match the combined bounds of all child renderers.
        /// Recomputes the offset and size values based on the renderer bounds and the object's transform.
        /// </summary>
        [ContextMenu("Reset To Renderer Bounds")]
        public void ResetToRendererBounds()
        {
            Bounds worldBounds = gameObject.CalculateRendererBounds();
            offset = transform.InverseTransformPoint(worldBounds.center);
            size = worldBounds.size.InverseScale(transform.lossyScale);
        }
    }
}