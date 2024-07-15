using System;
using UnityEngine;

namespace Rehawk.Foundation.Misc
{
    [DisallowMultipleComponent]
    public class EventBeforeDestroy : MonoBehaviour
    {
        public event Action<EventBeforeDestroy> BeforeDestroyed;

        private void OnDestroy()
        {
            BeforeDestroyed?.Invoke(this);
        }
    }
}