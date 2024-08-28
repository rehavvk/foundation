namespace Rehawk.Foundation.Pooling
{
    // Zero parameters
    public class Pool<TValue> : PoolBase<TValue>, IPool<TValue>
    {
        public TValue Pop()
        {
            var item = GetInternal();
            Reinitialize(item);
            return item;
        }

        /// <summary>
        ///     Is called when the pooled object gets spawned and filled with new values.
        /// </summary>
        protected virtual void Reinitialize(TValue item)
        {
            // Optional
        }
    }
    
    // One parameter
    public class Pool<TParam, TValue> : PoolBase<TValue>, IPool<TParam, TValue>
    {
        public TValue Pop(TParam param)
        {
            var item = GetInternal();
            Reinitialize(param, item);
            return item;
        }

        /// <summary>
        ///     Is called when the pooled object gets spawned and filled with new values.
        /// </summary>
        protected virtual void Reinitialize(TParam param, TValue item)
        {
            // Optional
        }
    }
    
    // Two parameters
    public class Pool<TParam1, TParam2, TValue> : PoolBase<TValue>, IPool<TParam1, TParam2, TValue>
    {
        public TValue Pop(TParam1 param1, TParam2 param2)
        {
            var item = GetInternal();
            Reinitialize(param1, param2, item);
            return item;
        }

        /// <summary>
        ///     Is called when the pooled object gets spawned and filled with new values.
        /// </summary>
        protected virtual void Reinitialize(TParam1 param1, TParam2 param2, TValue item)
        {
            // Optional
        }
    }
    
    // Three parameters
    public class Pool<TParam1, TParam2, TParam3, TValue> : PoolBase<TValue>, IPool<TParam1, TParam2, TParam3, TValue>
    {
        public TValue Pop(TParam1 param1, TParam2 param2, TParam3 param3)
        {
            var item = GetInternal();
            Reinitialize(param1, param2, param3, item);
            return item;
        }

        /// <summary>
        ///     Is called when the pooled object gets spawned and filled with new values.
        /// </summary>
        protected virtual void Reinitialize(TParam1 param1, TParam2 param2, TParam3 param3, TValue item)
        {
            // Optional
        }
    }
    
    // Four parameters
    public class Pool<TParam1, TParam2, TParam3, TParam4, TValue> : PoolBase<TValue>, IPool<TParam1, TParam2, TParam3, TParam4, TValue>
    {
        public TValue Pop(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4)
        {
            var item = GetInternal();
            Reinitialize(param1, param2, param3, param4, item);
            return item;
        }

        /// <summary>
        ///     Is called when the pooled object gets spawned and filled with new values.
        /// </summary>
        protected virtual void Reinitialize(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TValue item)
        {
            // Optional
        }
    }
}