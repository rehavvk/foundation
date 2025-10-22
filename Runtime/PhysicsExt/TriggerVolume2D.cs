using System;
using System.Collections.Generic;
using Rehawk.Foundation.Extensions;
using UnityEngine;

namespace Rehawk.Foundation.Misc
{
    public class TriggerVolume2D : MonoBehaviour
    {
        private List<Rigidbody2D> detectedRigidbodies = new List<Rigidbody2D>();
        
        private bool isQuitting;
        
        private readonly Dictionary<Rigidbody2D, int> rigidbodyCounts = new Dictionary<Rigidbody2D, int>();
        
        public event Action<Rigidbody2D> Entered;
        public event Action<Rigidbody2D> Left;

        public int Count => detectedRigidbodies.Count;

        public IReadOnlyList<Rigidbody2D> DetectedRigidbodies => detectedRigidbodies;

        private void OnApplicationQuit()
        {
            isQuitting = true;
            
            rigidbodyCounts.Clear();
            detectedRigidbodies.Clear();
        }

        public void HandleEnter(Rigidbody2D rigidbody)
        {
            if (isQuitting || !rigidbody || !gameObject || !gameObject.activeInHierarchy)
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
        
        public void HandleLeft(Rigidbody2D rigidbody)
        {
            if (isQuitting || !rigidbody || !gameObject || !gameObject.activeInHierarchy)
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
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            HandleEnter(other.attachedRigidbody);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            HandleLeft(other.attachedRigidbody);
        }
        
        private void OnOtherDisabled(GameObject other)
        {
            HandleLeft(other.GetComponent<Rigidbody2D>());
        }
    }
}