using UnityEngine;

namespace Rehawk.Foundation.Misc
{
    public class CustomParentConstraint : MonoBehaviour
    {
        [Disabled]
        [SerializeField] private Transform parent;

        private Vector3 localPosition;
        private Quaternion localRotation;

        private void Update()
        {
            if (parent)
            {
                transform.SetPositionAndRotation(parent.TransformPoint(localPosition), parent.rotation * localRotation);
            }
            else
            {
                enabled = false;
            }
        }
        
        public void InitLocal(Transform parent, Vector3 localPosition, Quaternion localRotation)
        {
            this.parent = parent;
            this.localPosition = localPosition;
            this.localRotation = localRotation;
            enabled = true;
        }
        
        public void InitWorld(Transform parent, Vector3 position, Quaternion rotation)
        {
            InitLocal(parent, parent.InverseTransformPoint(position), rotation * Quaternion.Inverse(parent.rotation));
        }
    }
}