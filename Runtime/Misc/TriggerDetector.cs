using System;
using System.Collections.Generic;
using Rehawk.Foundation.Extensions;
using UnityEngine;

namespace Rehawk.Foundation.Misc
{
    public class TriggerDetector : MonoBehaviour
    {
        private readonly HashSet<Collider> detectedColliders = new HashSet<Collider>();

        public event Action<Collider> ColliderEntered;
        public event Action<Collider> ColliderLeft;

        public int Count
        {
            get { return detectedColliders.Count; }
        }

        public IReadOnlyCollection<Collider> DetectedColliders
        {
            get { return detectedColliders; }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            other.AddDisableListener(OnOtherDisabled);

            detectedColliders.Add(other);
            
            ColliderEntered?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            other.RemoveDisableListener(OnOtherDisabled);

            detectedColliders.Remove(other);
            
            ColliderLeft?.Invoke(other);
        }
        
        private void OnOtherDisabled(GameObject other)
        {
            other.RemoveDisableListener(OnOtherDisabled);
            detectedColliders.Remove(other.GetComponent<Collider>());
        }
    }
}