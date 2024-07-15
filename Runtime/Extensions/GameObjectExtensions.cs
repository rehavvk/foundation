using UnityEngine;

namespace Rehawk.Foundation.Extensions
{
    public static class GameObjectExtensions
    {
        public static bool HasComponent<T>(this GameObject gameObject)
        {
            return gameObject.TryGetComponent(out T _);
        }
        
        public static T AddOrGetComponent<T>(this GameObject gameObject) where T : Component 
        {
            if (gameObject.TryGetComponent(out T newComponent))
            {
                return newComponent;
            }

            return gameObject.AddComponent<T>();
        }
    }
}