using System;
using System.Collections.Generic;
using UnityEngine;

#if ODIN_INSPECTOR_3
using Sirenix.OdinInspector;
#endif

namespace Rehawk.Foundation.Misc
{
    /// <summary>
    /// Draws each configured marble once in random order before automatically refilling.
    /// Drawing from an empty bag returns the default value of T.
    /// </summary>
    [Serializable]
#if ODIN_INSPECTOR_3
    [InlineProperty]
#endif
    public sealed class MarbleBag<T>
    {
#if ODIN_INSPECTOR_3
        [LabelText(" ")]
        [OnValueChanged(nameof(Reset), true)]
#endif
        [Tooltip("Relative percentages define a draw-without-replacement bag at 0.01% precision. Values are normalized if they do not total 100.")]
        [SerializeField] private MarbleRef<T>[] _references = Array.Empty<MarbleRef<T>>();

        [NonSerialized] private int[] _remaining;
        [NonSerialized] private int _remainingMarbles;

        public int Count => _references.Length;
        public IEnumerable<MarbleRef<T>> References
        {
            get
            {
                foreach (MarbleRef<T> reference in _references)
                    yield return reference;
            }
        }

        public int TotalMarbles
        {
            get
            {
                int divisor = CalculateDivisor();
                int total = 0;
                foreach (MarbleRef<T> reference in _references)
                    total = checked(total + GetUnits(reference) / divisor);

                return total;
            }
        }

        public MarbleBag()
        {
        }

        public MarbleBag(params MarbleRef<T>[] references)
        {
            if (references == null)
                throw new ArgumentNullException(nameof(references));

            foreach (MarbleRef<T> reference in references)
            {
                if (reference == null)
                    throw new ArgumentException("Marble references cannot be null.", nameof(references));
            }

            _references = (MarbleRef<T>[])references.Clone();
        }

        public T Draw()
        {
            if (!PrepareDraw())
                return default;

            return DrawAt(UnityEngine.Random.Range(0, _remainingMarbles));
        }

        public T Draw(System.Random random)
        {
            if (random == null)
                throw new ArgumentNullException(nameof(random));

            if (!PrepareDraw())
                return default;

            return DrawAt(random.Next(_remainingMarbles));
        }

        /// <summary>Discards the current cycle. The next draw starts with a full bag.</summary>
        public void Reset()
        {
            _remainingMarbles = 0;
        }

        public void Add(MarbleRef<T> reference)
        {
            if (reference == null)
                throw new ArgumentNullException(nameof(reference));

            Array.Resize(ref _references, _references.Length + 1);
            _references[_references.Length - 1] = reference;
            Reset();
        }

        public bool Remove(MarbleRef<T> reference)
        {
            int index = Array.IndexOf(_references, reference);
            if (index < 0)
                return false;

            Array.Copy(_references, index + 1, _references, index, _references.Length - index - 1);
            Array.Resize(ref _references, _references.Length - 1);
            Reset();
            return true;
        }

        public void Clear()
        {
            _references = Array.Empty<MarbleRef<T>>();
            _remaining = null;
            Reset();
        }

        private bool PrepareDraw()
        {
            if (_remainingMarbles > 0)
                return true;

            if (_remaining == null || _remaining.Length != _references.Length)
                _remaining = new int[_references.Length];

            int divisor = CalculateDivisor();
            int total = 0;
            for (int i = 0; i < _references.Length; i++)
            {
                _remaining[i] = GetUnits(_references[i]) / divisor;
                total = checked(total + _remaining[i]);
            }

            _remainingMarbles = total;
            return total > 0;
        }

        private T DrawAt(int index)
        {
            int referenceIndex = 0;
            while (index >= _remaining[referenceIndex])
            {
                index -= _remaining[referenceIndex];
                referenceIndex++;
            }

            _remaining[referenceIndex]--;
            _remainingMarbles--;
            return _references[referenceIndex].Value;
        }

        private int CalculateDivisor()
        {
            int divisor = 0;
            foreach (MarbleRef<T> reference in _references)
            {
                int units = GetUnits(reference);
                while (units != 0)
                {
                    int remainder = divisor % units;
                    divisor = units;
                    units = remainder;
                }
            }

            return Math.Max(1, divisor);
        }

        private static int GetUnits(MarbleRef<T> reference)
        {
            return Mathf.RoundToInt(Mathf.Clamp(reference.Percentage, 0f, 100f) * 100f);
        }
    }
}
