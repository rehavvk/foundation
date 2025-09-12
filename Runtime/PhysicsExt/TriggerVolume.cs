using System;
using System.Collections.Generic;
using Rehawk.Foundation.Extensions;
using UnityEngine;

namespace Rehawk.Foundation.Misc
{
    public class TriggerVolume : MonoBehaviour
    {
        private List<Rigidbody> detectedRigidbodies = new List<Rigidbody>();
        
        private readonly Dictionary<Rigidbody, int> rigidbodyCounts = new Dictionary<Rigidbody, int>();
        
        public event Action<Rigidbody> Entered;
        public event Action<Rigidbody> Left;

        public int Count => detectedRigidbodies.Count;

        public IReadOnlyList<Rigidbody> DetectedRigidbodies => detectedRigidbodies;

        private void OnApplicationQuit()
        {
            rigidbodyCounts.Clear();
            detectedRigidbodies.Clear();
        }

        public void HandleEnter(Rigidbody rigidbody)
        {
            if (!rigidbody || !gameObject.activeInHierarchy)
                return;

            int count = rigidbodyCounts.GetValueOrDefault(rigidbody, 0);
            count += 1;
            
            rigidbodyCounts[rigidbody] = count;

            if (count != 1) 
                return;
            
            detectedRigidbodies.Add(rigidbody);
            rigidbody.AddDisableListener(OnOtherDisabled);
                
            Entered?.Invoke(rigidbody);
        }
        
        public void HandleLeft(Rigidbody rigidbody)
        {
            if (!rigidbody || !gameObject.activeInHierarchy)
                return;
            
            int count = rigidbodyCounts.GetValueOrDefault(rigidbody, 0);
            
            if (count <= 0) 
                return;
            
            count -= 1;

            if (count > 0)
            {
                rigidbodyCounts[rigidbody] = count;
                return;
            }

            rigidbodyCounts.Remove(rigidbody);
            detectedRigidbodies.Remove(rigidbody);
            rigidbody.RemoveDisableListener(OnOtherDisabled);
            
            Left?.Invoke(rigidbody);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            HandleEnter(other.attachedRigidbody);
        }

        private void OnTriggerExit(Collider other)
        {
            HandleLeft(other.attachedRigidbody);
        }
        
        private void OnOtherDisabled(GameObject other)
        {
            HandleLeft(other.GetComponent<Rigidbody>());
        }
    }
}