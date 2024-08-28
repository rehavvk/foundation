using System;
using UnityEngine;

namespace Rehawk.Foundation.Extensions
{
    public static class ComponentExtensions
    {
        public static bool HasComponent<T>(this Component component)
        {
            return component.TryGetComponent(out T _);
        }
        
        public static bool HasComponent(this Component component, Type type)
        {
            return component.TryGetComponent(type, out Component _);
        }
        
        public static T AddOrGetComponent<T>(this Component component) where T : Component 
        {
            if (component.TryGetComponent(out T newComponent))
            {
                return newComponent;
            }

            return component.gameObject.AddComponent<T>();
        }
        
        public static Component AddOrGetComponent(this Component component, Type type)
        {
            if (component.TryGetComponent(type, out Component newComponent))
            {
                return newComponent;
            }

            return component.gameObject.AddComponent(type);
        }
    }
}