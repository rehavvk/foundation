namespace Rehawk.Foundation.Misc
{
    public static class ObjectUtility
    {
        public static bool IsNull<T>(T instance)
        {
            return instance == null || instance.Equals(null);
        }
        
        public static bool IsNotNull<T>(T instance)
        {
            return !IsNull(instance);
        }
    }
}