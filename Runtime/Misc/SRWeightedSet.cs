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
    public static class SRWeightedSet
    {
        public static T GetRandom<T>(IList<SRWeightedRef<T>> references)
        {
            float totalChance = CalculateTotalChance(references);
            float randomValue = Random.Range(0, totalChance);

            return GetRandom(references, randomValue).Value;
        }
        
        public static T GetRandom<T>(IList<SRWeightedRef<T>> references, System.Random random)
        {
            float totalChance = CalculateTotalChance(references);
            float randomValue = (float)random.NextDouble() * totalChance;

            return GetRandom(references, randomValue).Value;
        }
        
        public static SRWeightedRef<T> GetRandom<T>(IList<SRWeightedRef<T>> references, float randomValue)
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

        public static float CalculateTotalChance<T>(IList<SRWeightedRef<T>> references)
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
    public class SRWeightedSet<T>
    {
#if ODIN_INSPECTOR_3
        [OnValueChanged(nameof(PrecalculateTotalChance), true), LabelText(" ")]
#endif
        [SerializeField] private SRWeightedRef<T>[] references = Array.Empty<SRWeightedRef<T>>();

        [HideInInspector]
        [SerializeField] private float totalChance = -1;

        public int Count
        {
            get { return references.Length; }
        }

        public IEnumerable<SRWeightedRef<T>> References
        {
            get { return references; }
        }
        
        public T GetRandom()
        {
            PrecalculateTotalChanceIfNeeded();
            
            float randomValue = Random.Range(0, totalChance);
            SRWeightedRef<T> reference = SRWeightedSet.GetRandom(references, randomValue);
            
            return reference != null ? reference.Value : default;
        }
        
        public T GetRandom(System.Random random)
        {
            PrecalculateTotalChanceIfNeeded();

            float randomValue = (float)random.NextDouble() * totalChance;
            SRWeightedRef<T> reference = SRWeightedSet.GetRandom(references, randomValue);
            
            return reference != null ? reference.Value : default;
        }

        public void Add(SRWeightedRef<T> reference)
        {
            Array.Resize(ref references, references.Length + 1);
            references[^1] = reference;
            PrecalculateTotalChance();
        }
        
        public bool Remove(SRWeightedRef<T> reference)
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
            references = Array.Empty<SRWeightedRef<T>>();
            PrecalculateTotalChance();
        }
        
        private void PrecalculateTotalChance()
        {
            totalChance = SRWeightedSet.CalculateTotalChance(references);
        }

        private void PrecalculateTotalChanceIfNeeded()
        {
            if (totalChance >= 0)
                return;
            
            PrecalculateTotalChance();
        }
    }

    [Serializable]
    public class SRWeightedRef<T>
    {
#if ODIN_INSPECTOR_3
        [HorizontalGroup("Values"), HideLabel, FormerlySerializedAs("value")]
#endif
        [SerializeReference] public T Value;
        
#if ODIN_INSPECTOR_3
        [HorizontalGroup("Values"), Range(0, 100), HideLabel, SuffixLabel("%"), FormerlySerializedAs("Chance")]
#endif
        [SerializeField] private float chance = 100;

        public float Chance
        {
            get { return chance / 100; }
        }
    }
}