using System;
using UnityEngine;

#if ODIN_INSPECTOR_3
using Sirenix.OdinInspector;
#endif

namespace Rehawk.Foundation.Misc
{
    [Serializable]
    public sealed class MarbleChance
    {
        [HideInInspector]
        [SerializeField] private int _hits = 1;
        [HideInInspector]
        [SerializeField] private int _misses;

        [NonSerialized] private MarbleBag<bool> _bag;

#if ODIN_INSPECTOR_3
        [ShowInInspector]
        [PropertyRange(0f, 100f)]
        [SuffixLabel("%")]
#endif
        public float Percentage
        {
            get => _hits + (float)_misses > 0f ? 100f * _hits / (_hits + (float)_misses) : 0f;
            set
            {
                _hits = Mathf.RoundToInt(Mathf.Clamp(value, 0f, 100f) * 100f);
                _misses = 10000 - _hits;
                _bag = null;
            }
        }

        public MarbleChance()
        {
        }

        public MarbleChance(int hits, int misses)
        {
            if (hits < 0)
                throw new ArgumentOutOfRangeException(nameof(hits));

            if (misses < 0)
                throw new ArgumentOutOfRangeException(nameof(misses));

            _hits = hits;
            _misses = misses;
        }

        public bool Draw()
        {
            float scale = Mathf.Max(_hits, _misses) > 100 ? (_hits + (float)_misses) / 100f : 1f;
            _bag ??= new MarbleBag<bool>(
                new MarbleRef<bool>(true, _hits / scale),
                new MarbleRef<bool>(false, _misses / scale));

            return _bag.Draw();
        }
    }
}
