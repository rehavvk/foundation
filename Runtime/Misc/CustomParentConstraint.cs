using UnityEngine;

namespace Rehawk.Foundation.Misc
{
    [ExecuteAlways]
    public class CustomParentConstraint : MonoBehaviour
    {
        private Vector3 localPosition;
        private Quaternion localRotation;

        private Transform parent;

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
        
        public void ActivateConstraint(Transform parent, Vector3 localPosition, Quaternion localRotation)
        {
            this.parent = parent;
            this.localPosition = localPosition;
            this.localRotation = localRotation;
            enabled = true;
        }
        
        [ContextMenu("Activate Constraint World")]
        public void ActivateConstraintWorld(Transform parent, Vector3 position, Quaternion rotation)
        {
            ActivateConstraint(parent, parent.InverseTransformPoint(position), rotation * Quaternion.Inverse(parent.rotation));
        }
    }
}