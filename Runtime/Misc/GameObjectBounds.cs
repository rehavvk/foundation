using Rehawk.Foundation.Extensions;
using UnityEngine;

namespace Rehawk.Foundation.Misc
{
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
        
        public Bounds GetWorldBounds()
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

        public Vector3[] GetCorners()
        {
            Vector3 scaledSize = Vector3.Scale(size, transform.lossyScale);
            Vector3 halfSize = scaledSize * 0.5f;

            return new[]
            {
                transform.TransformPoint(offset + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z)),
                transform.TransformPoint(offset + new Vector3(-halfSize.x, -halfSize.y, halfSize.z)),
                transform.TransformPoint(offset + new Vector3(-halfSize.x, halfSize.y, -halfSize.z)),
                transform.TransformPoint(offset + new Vector3(-halfSize.x, halfSize.y, halfSize.z)),
                transform.TransformPoint(offset + new Vector3(halfSize.x, -halfSize.y, -halfSize.z)),
                transform.TransformPoint(offset + new Vector3(halfSize.x, -halfSize.y, halfSize.z)),
                transform.TransformPoint(offset + new Vector3(halfSize.x, halfSize.y, -halfSize.z)),
                transform.TransformPoint(offset + new Vector3(halfSize.x, halfSize.y, halfSize.z))
            };
        }

        [ContextMenu("Reset To Renderer Bounds")]
        private void ResetToRendererBounds()
        {
            Bounds worldBounds = gameObject.CalculateRendererBounds();
            offset = transform.InverseTransformPoint(worldBounds.center);
            size = worldBounds.size.InverseScale(transform.lossyScale);
        }
    }
}