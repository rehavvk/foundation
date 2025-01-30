using System;
using System.Collections.Generic;
using Rehawk.Foundation.Extensions;
using UnityEngine;

namespace Rehawk.Foundation.Misc
{
    public class TriggerVolume : MonoBehaviour
    {
        [SerializeField] private bool onlyUniqueRigidbodies = true;
        
        private readonly HashSet<Collider> detectedColliders = new HashSet<Collider>();
        private readonly HashSet<Rigidbody> detectedRigidbodies = new HashSet<Rigidbody>();

        public event Action<Collider> Entered;
        public event Action<Collider> Left;

        public int Count
        {
            get { return detectedColliders.Count; }
        }

        public IReadOnlyCollection<Collider> DetectedColliders
        {
            get { return detectedColliders; }
        }

        private void HandleEnter(Collider other)
        {
            if (onlyUniqueRigidbodies)
            {
                if (!detectedRigidbodies.Add(other.attachedRigidbody))
                    return;
            }

            if (detectedColliders.Add(other))
            {
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