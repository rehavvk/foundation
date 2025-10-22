using System;
using System.Collections.Generic;

namespace Rehawk.Foundation.Misc
{
    public class Semaphore
    {
        private readonly HashSet<object> keys = new HashSet<object>();

        public event Action StateChanged;
        
        public bool AsBool
        {
            get { return keys.Count > 0; }
        }

        public bool Contains(object key)
        {
            return keys.Contains(key);
        }
        
        public void Add(object key)
        {
            bool previousState = AsBool;

            keys.Add(key);
            
            if (previousState != AsBool)
            {
                StateChanged?.Invoke();
            }
        }

        public void Remove(object key)
        {
            bool previousState = AsBool;
            
            keys.Remove(key);

            if (previousState != AsBool)
            {
                StateChanged?.Invoke();
            }
        }

        public void Clear()
        {
            keys.Clear();
            StateChanged?.Invoke();
        }

        public static implicit operator bool(Semaphore semaphore)
        {
            return semaphore.AsBool;
        }

        public override string ToString()
        {
            return AsBool.ToString();
        }
    }
}