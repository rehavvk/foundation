using System;
using UnityEngine;

namespace Rehawk.Foundation.Misc
{
    [DisallowMultipleComponent]
    public class EventBeforeDisable : MonoBehaviour
    {
        public event Action<GameObject> BeforeDisabled;
        
        private void OnDisable()
        {
            BeforeDisabled?.Invoke(gameObject);
        }
    }
}