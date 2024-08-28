using System;

namespace Rehawk.Foundation.Pooling
{
    public interface IPool
    {
        int NumTotal { get; }
        int NumActive { get; }
        int NumInactive { get; }

        Type ItemType { get; }

        void SetFactory(Func<object> factory);
        void SetMaxSize(int maxSize);
        
        void Prewarm(int initialSize);

        void Resize(int desiredPoolSize);

        void Clear();

        void ExpandBy(int numToAdd);

        void ShrinkBy(int numToRemove);

        bool Return(object obj);

        void Remove(object obj);
    }
    
    public interface IReturnablePool<TValue> : IPool
    {
        bool Return(TValue item);
    }

    public interface IPool<TValue> : IReturnablePool<TValue>
    {
        TValue Pop();
    }
    
    public interface IPool<in TParam1, TValue> : IReturnablePool<TValue>
    {
        TValue Pop(TParam1 param);
    }

    public interface IPool<in TParam1, in TParam2, TValue> : IReturnablePool<TValue>
    {
        TValue Pop(TParam1 param1, TParam2 param2);
    }

    public interface IPool<in TParam1, in TParam2, in TParam3, TValue> : IReturnablePool<TValue>
    {
        TValue Pop(TParam1 param1, TParam2 param2, TParam3 param3);
    }

    public interface IPool<in TParam1, in TParam2, in TParam3, in TParam4, TValue> : IReturnablePool<TValue>
    {
        TValue Pop(TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4);
    }
}