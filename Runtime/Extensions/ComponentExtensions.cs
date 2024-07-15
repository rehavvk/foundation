using UnityEngine;

namespace Rehawk.Foundation.Extensions
{
    public static class ComponentExtensions
    {
        public static bool HasComponent<T>(this Component component)
        {
            return component.TryGetComponent(out T _);
        }
        
        public static T AddOrGetComponent<T>(this Component component) where T : Component 
        {
            if (component.TryGetComponent(out T newComponent))
            {
                return newComponent;
            }

            return component.gameObject.AddComponent<T>();
        }
    }
}