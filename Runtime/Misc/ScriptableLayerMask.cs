using UnityEngine;

namespace Rehawk.Foundation.Misc
{
    [CreateAssetMenu(menuName = "Foundation/Scriptable LayerMask", order = 800)]
    public class ScriptableLayerMask : ScriptableObject
    {
        [SerializeField] private LayerMask mask;

        public LayerMask Mask
        {
            get { return mask; }
        }
        
        public static implicit operator LayerMask(ScriptableLayerMask scriptableLayerMask)
        {
            return scriptableLayerMask.Mask;
        }
        
        public static implicit operator int(ScriptableLayerMask scriptableLayerMask)
        {
            return scriptableLayerMask.Mask;
        }
    }
}