using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace Rehawk.Foundation.Pooling
{
    public class PoolBase<TContract> : IPool
    {
        private readonly List<TContract> activeItems = new List<TContract>();
        private Stack<TContract> inactiveItems = new Stack<TContract>();
        
        private bool isPrewarmed;

        private int maxSize = int.MaxValue;

        private Func<object> factory = () => Activator.CreateInstance<TContract>();
        
        public IEnumerable<TContract> ActiveItems
        {
            get { return activeItems; }
        }

        public IEnumerable<TContract> InactiveItems
        {
            get { return inactiveItems; }
        }

        public int NumTotal
        {
            get { return NumInactive + NumActive; }
        }

        public int NumInactive
        {
            get { return inactiveItems.Count; }
        }

        public int NumActive
        {
            get { return activeItems.Count; }
        }

        public Type ItemType
        {
            get { return typeof(TContract); }
        }

        void IPool.SetFactory(Func<object> factory)
        {
            this.factory = factory;
        }

        public void SetFactory(Func<TContract> factory)
        {
            this.factory = () => factory.Invoke();
        }

        public void SetMaxSize(int maxSize)
        {
            this.maxSize = maxSize;
        }
        
        public void Prewarm(int initialSize)
        {
            if (isPrewarmed)
            {
                Resize(initialSize);
                return;
            }
            
            isPrewarmed = true;
            inactiveItems = new Stack<TContract>(initialSize);

            for (int i = 0; i < initialSize; i++)
            {
                inactiveItems.Push(AllocNew());
            }
        }

        bool IPool.Return(object item)
        {
            return Return((TContract)item);
        }

        public bool Return(TContract item)
        {
            if (activeItems.Contains(item))
            {
                activeItems.Remove(item);
                
                OnReturn(item);

                inactiveItems.Push(item);
                
                if (inactiveItems.Count > maxSize)
                {
                    Resize(maxSize);
                }
                
                return true;
            }
            
            return false;
        }

        void IPool.Remove(object item)
        {
            Remove((TContract)item);
        }

        public void Remove(TContract item)
        {
            if (activeItems.Contains(item))
            {
                activeItems.Remove(item);
            }
            
            if (inactiveItems.Contains(item))
            {
                inactiveItems = new Stack<TContract>(inactiveItems.Where(i => !i.Equals(item)));
            
                if (inactiveItems.Count > maxSize)
                {
                    Resize(maxSize);
                }
            }

            OnRemove(item);
        }

        public void Clear()
        {
            Resize(0);
        }

        public void ShrinkBy(int numToRemove)
        {
            Resize(NumTotal - numToRemove);
        }

        public void ExpandBy(int numToAdd)
        {
            Resize(NumTotal + numToAdd);
        }

        private TContract AllocNew()
        {
            var item = (TContract) factory.Invoke();
            OnCreate(item);
            return item;
        }
        
        protected TContract GetInternal()
        {
            if (inactiveItems.Count == 0)
            {
                ExpandPool();
            }

            // Pop and allocate a new instance if the popped instance was null due to disposing without notice (example GameObject magic)
            TContract item = inactiveItems.Pop() ?? AllocNew();

            activeItems.Add(item);
            
            OnPop(item);
            return item;
        }

        public void Resize(int desiredPoolSize)
        {
            if (NumTotal == desiredPoolSize)
                return;

            Assert.IsFalse(desiredPoolSize < 0, "Attempted to resize the pool to a negative amount");

            while (NumTotal > desiredPoolSize && NumInactive > 0)
            {
                TContract obj = inactiveItems.Pop();
                if (obj != null)
                {
                    OnDestroy(obj);
                }
            }

            while (desiredPoolSize > NumTotal)
            {
                inactiveItems.Push(AllocNew());
            }
        }

        private void ExpandPool()
        {
            if (NumTotal == 0)
            {
                ExpandBy(1);
            }
            else
            {
                ExpandBy(NumTotal);
            }
        }

        /// <summary>
        ///     Is called when the pooled object is created and stored in the pool.
        /// </summary>
        protected virtual void OnCreate(TContract item)
        {
            // Optional
        }

        /// <summary>
        ///     Is called when the pooled object is popped from pool.
        /// </summary>
        protected virtual void OnPop(TContract item)
        {
            // Optional
        }

        /// <summary>
        ///     Is called when the pooled object is pushed back in the pool.
        /// </summary>
        protected virtual void OnReturn(TContract item)
        {
            // Optional
        }

        /// <summary>
        ///     Is called when the pooled object is removed from pool.
        /// </summary>
        protected virtual void OnRemove(TContract item)
        {
            // Optional
        }
        
        /// <summary>
        ///     Is called when the pooled object is finally destroyed.
        /// </summary>
        protected virtual void OnDestroy(TContract item)
        {
            // Optional
        }
    }
}