using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

#if ODIN_INSPECTOR_3
using Sirenix.OdinInspector;
#endif

namespace Rehawk.Foundation.Misc
{
    public static class WeightedSet
    {
        public static T GetRandom<T>(IList<WeightedRef<T>> references)
        {
            float totalChance = CalculateTotalChance(references);
            float randomValue = Random.Range(0, totalChance);

            return GetRandom(references, randomValue).Value;
        }
        
        public static T GetRandom<T>(IList<WeightedRef<T>> references, System.Random random)
        {
            float totalChance = CalculateTotalChance(references);
            float randomValue = (float)random.NextDouble() * totalChance;

            return GetRandom(references, randomValue).Value;
        }
        
        public static WeightedRef<T> GetRandom<T>(IList<WeightedRef<T>> references, float randomValue)
        {
            float currentMax = 0;
            for (int i = 0; i < references.Count; i++)
            {
                currentMax += references[i].Chance;
                if (randomValue <= currentMax)
                {
                    return references[i];
                }
            }

            Debug.LogError("The first element was returned as there was an issue with the randomization.");
            
            return references.FirstOrDefault();
        }

        public static float CalculateTotalChance<T>(IList<WeightedRef<T>> references)
        {
            float result = 0;

            if (references != null)
            {
                for (int i = 0; i < references.Count; i++)
                {
                    result += references[i].Chance;
                }
            }

            return result;
        }
    }
    
    [Serializable]
    [InlineProperty]
    public class WeightedSet<T>
    {
#if ODIN_INSPECTOR_3
        [OnValueChanged(nameof(PrecalculateTotalChance), true), LabelText(" ")]
#endif
        [SerializeField] private WeightedRef<T>[] references = Array.Empty<WeightedRef<T>>();

        [HideInInspector]
        [SerializeField] private float totalChance = -1;

        public int Count
        {
            get { return references.Length; }
        }

        public IEnumerable<WeightedRef<T>> References
        {
            get { return references; }
        }
        
        public T GetRandom()
        {
            PrecalculateTotalChanceIfNeeded();
            
            float randomValue = Random.Range(0, totalChance);
            WeightedRef<T> reference = WeightedSet.GetRandom(references, randomValue);
            
            return reference != null ? reference.Value : default;
        }
        
        public T GetRandom(System.Random random)
        {
            PrecalculateTotalChanceIfNeeded();

            float randomValue = (float)random.NextDouble() * totalChance;
            WeightedRef<T> reference = WeightedSet.GetRandom(references, randomValue);
            
            return reference != null ? reference.Value : default;
        }

        public void Add(WeightedRef<T> reference)
        {
            Array.Resize(ref references, references.Length + 1);
            references[^1] = reference;
            PrecalculateTotalChance();
        }
        
        public bool Remove(WeightedRef<T> reference)
        {
            int index = Array.IndexOf(references, reference);
            
            if (index < 0)
                return false;
            
            for (int a = index; a < references.Length - 1; a++)
            {
                references[a] = references[a + 1];
            }
            
            Array.Resize(ref references, references.Length - 1);
            PrecalculateTotalChance();
            
            return true;
        }

        public void Clear()
        {
            references = Array.Empty<WeightedRef<T>>();
            PrecalculateTotalChance();
        }
        
        private void PrecalculateTotalChance()
        {
            totalChance = WeightedSet.CalculateTotalChance(references);
        }

        private void PrecalculateTotalChanceIfNeeded()
        {
            if (totalChance >= 0)
                return;
            
            PrecalculateTotalChance();
        }
    }

    [Serializable]
    public class WeightedRef<T>
    {
        [HorizontalGroup("Values"), HideLabel, FormerlySerializedAs("value")]
        public T Value;
        
        [HorizontalGroup("Values"), Range(0, 1), HideLabel, SuffixLabel("%"), FormerlySerializedAs("chance")]
        public float Chance;
    }
}