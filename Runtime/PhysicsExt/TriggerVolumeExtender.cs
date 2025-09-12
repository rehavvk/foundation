using Rehawk.Foundation.Misc;
using UnityEngine;

namespace Rehawk.Foundation.PhysicsExt
{
    public class TriggerVolumeExtender : MonoBehaviour
    {
        [GetComponent(true)]
        [SerializeField] private TriggerVolume triggerVolume;
        
        private void OnTriggerEnter(Collider other)
        {
            triggerVolume.HandleEnter(other.attachedRigidbody);
        }

        private void OnTriggerExit(Collider other)
        {
            triggerVolume.HandleLeft(other.attachedRigidbody);
        }
    }
}