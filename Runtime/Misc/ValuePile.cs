using System;
using System.Collections.Generic;

namespace Rehawk.Foundation.Misc
{
    public class ValuePile<T>
    {
        private readonly List<object> keys = new List<object>();
        private readonly List<T> values = new List<T>();
        private readonly T defaultValue;
        
        public event Action TopValueChanged;

        public ValuePile(T defaultValue = default)
        {
            this.defaultValue = defaultValue;
        }
        
        public T TopValue
        {
            get { return values.Count > 0 ? values[0] : defaultValue; }
        }

        public int Count
        {
            get { return keys.Count; }
        }

        public void Push(object key, T value)
        {
            T previousValue = TopValue;

            int index = keys.IndexOf(key);

            if (index >= 0)
            {
                keys.RemoveAt(index);
                values.RemoveAt(index);
            }
            
            keys.Insert(0, key);
            values.Insert(0, value);
            
            if (previousValue == null || !previousValue.Equals(TopValue))
            {
                TopValueChanged?.Invoke();
            }
        }

        public void Remove(object key)
        {
            int index = keys.IndexOf(key);

            if (index >= 0)
            {
                T previousValue = TopValue;

                keys.RemoveAt(index);
                values.RemoveAt(index);
            
                if (previousValue == null || !previousValue.Equals(TopValue))
                {
                    TopValueChanged?.Invoke();
                }
            }
        }

        public bool Remove(object key, T value)
        {
            int index = keys.IndexOf(key);

            if (index >= 0 && values[index].Equals(value))
            {
                T previousValue = TopValue;

                keys.RemoveAt(index);
                values.RemoveAt(index);
            
                if (previousValue == null || !previousValue.Equals(TopValue))
                {
                    TopValueChanged?.Invoke();
                }

                return true;
            }

            return false;
        }

        public void Clear()
        {
            T previousValue = TopValue;

            keys.Clear();
            values.Clear();
            
            if (previousValue == null || !previousValue.Equals(TopValue))
            {
                TopValueChanged?.Invoke();
            }
        }

        public static implicit operator T(ValuePile<T> valuePile)
        {
            return valuePile.TopValue;
        }

        public override string ToString()
        {
            return TopValue.ToString();
        }
    }
}