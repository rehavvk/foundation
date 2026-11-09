using System;
using UnityEngine;

#if ODIN_INSPECTOR_3
using Sirenix.OdinInspector;
#endif

namespace Rehawk.Foundation.Misc
{
    [Serializable]
    public sealed class MarbleRef<T>
    {
#if ODIN_INSPECTOR_3
        [HorizontalGroup("Values")]
        [HideLabel]
        [SuffixLabel("%")]
#endif
        [Tooltip("Relative percentage, rounded to two decimals. Values are normalized when their total differs from 100.")]
        [Range(0f, 100f)]
        [SerializeField] private float _percentage = 100f;

#if ODIN_INSPECTOR_3
        [PropertyOrder(-1)]
        [HorizontalGroup("Values")]
        [HideLabel]
#endif
        public T Value;

        public float Percentage => _percentage;

        public MarbleRef()
        {
        }

        public MarbleRef(T value, float percentage)
        {
            if (float.IsNaN(percentage) || percentage < 0f || percentage > 100f)
                throw new ArgumentOutOfRangeException(nameof(percentage));

            Value = value;
            _percentage = percentage;
        }
    }
}
