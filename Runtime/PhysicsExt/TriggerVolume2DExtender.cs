using Rehawk.Foundation.Misc;
using UnityEngine;

namespace Rehawk.Foundation.PhysicsExt
{
    public class TriggerVolume2DExtender : MonoBehaviour
    {
        [GetComponent(true)]
        [SerializeField] private TriggerVolume2D triggerVolume;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            triggerVolume.HandleEnter(other.attachedRigidbody);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            triggerVolume.HandleLeft(other.attachedRigidbody);
        }
    }
}