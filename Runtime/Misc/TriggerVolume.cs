using System;
using System.Collections.Generic;
using Rehawk.Foundation.Extensions;
using UnityEngine;

namespace Rehawk.Foundation.Misc
{
    public class TriggerVolume : MonoBehaviour
    {
        [SerializeField] private bool onlyUniqueRigidbodies = true;
        
        private readonly List<Collider> detectedColliders = new List<Collider>();
        private readonly List<Rigidbody> detectedRigidbodies = new List<Rigidbody>();

        public event Action<Collider> Entered;
        public event Action<Collider> Left;

        public int Count
        {
            get { return detectedColliders.Count; }
        }

        public IReadOnlyList<Collider> DetectedColliders
        {
            get { return detectedColliders; }
        }

        private void HandleEnter(Collider other)
        {
            if (onlyUniqueRigidbodies)
            {
                if (detectedRigidbodies.Contains(other.attachedRigidbody))
                    return;

                detectedRigidbodies.Add(other.attachedRigidbody);
            }

            if (!detectedColliders.Contains(other))
            {
                detectedColliders.Add(other);
                other.AddDisableListener(OnOtherDisabled);
                Entered?.Invoke(other);
            }
        }
        
        private void HandleLeft(Collider other)
        {
            other.RemoveDisableListener(OnOtherDisabled);

            detectedRigidbodies.Remove(other.attachedRigidbody);
            
            bool removedCollider = detectedColliders.Remove(other);
            if (removedCollider)
            {
                Left?.Invoke(other);
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            HandleEnter(other);
        }

        private void OnTriggerExit(Collider other)
        {
            HandleLeft(other);
        }
        
        private void OnOtherDisabled(GameObject other)
        {
            HandleLeft(other.GetComponent<Collider>());
        }
    }
}