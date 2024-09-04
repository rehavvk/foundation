using UnityEngine;

namespace Rehawk.Foundation.Misc
{
    public class ParentConstraint : MonoBehaviour
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
        
        public void Init(Transform parent, Vector3 localPosition, Quaternion localRotation)
        {
            this.parent = parent;
            this.localPosition = localPosition;
            this.localRotation = localRotation;
            enabled = true;
        }
    }
}