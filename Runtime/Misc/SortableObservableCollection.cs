using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine.Pool;

namespace Rehawk.Foundation.Misc
{
    public class SortableObservableCollection<T> : ObservableCollection<T>
    {
        /// <summary>
        /// Sorts the collection using the specified comparer.
        /// </summary>
        public void Sort(IComparer<T> comparer)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));

            List<T> sorted = ListPool<T>.Get();
            sorted.AddRange(this);
            sorted.Sort(comparer);

            ApplySortedList(sorted);
            
            ListPool<T>.Release(sorted);
        }

        /// <summary>
        /// Sorts the collection using a comparison delegate.
        /// </summary>
        public void Sort(Comparison<T> comparison)
        {
            if (comparison == null) throw new ArgumentNullException(nameof(comparison));

            List<T> sorted = ListPool<T>.Get();
            sorted.AddRange(this);
            sorted.Sort(comparison);

            ApplySortedList(sorted);
            
            ListPool<T>.Release(sorted);
        }

        private void ApplySortedList(List<T> sorted)
        {
            for (int i = 0; i < sorted.Count; i++)
            {
                int oldIndex = IndexOf(sorted[i]);
                if (oldIndex != i)
                {
                    Move(oldIndex, i);
                }
            }
        }
    }
}