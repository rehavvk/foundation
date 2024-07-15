using System;
using System.Collections.Generic;

namespace Rehawk.Foundation.Extensions
{
    public static class IDictionaryExtensions
    {
        public static void AddOrSet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }
        
        public static void Append<T, K>(this IDictionary<T, K> dictionary, IDictionary<T, K> source)
        {
            if (dictionary == null)
                throw new ArgumentNullException(nameof(dictionary));
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            
            foreach (KeyValuePair<T, K> element in source)
            {
                dictionary.Add(element.Key, element.Value);
            }
        }
    }
}