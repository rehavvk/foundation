using System;
using Rehawk.Foundation.Misc;
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
        
        public static T GetOrAddComponent<T>(this Component component)
            where T : Component 
        {
            if (component.TryGetComponent(out T newComponent))
            {
                return newComponent;
            }

            return component.gameObject.AddComponent<T>();
        }
        
        public static Component GetOrAddComponent(this Component component, Type type)
        {
            if (component.TryGetComponent(type, out Component newComponent))
            {
                return newComponent;
            }

            return component.gameObject.AddComponent(type);
        }

        public static bool TryGetComponentInParent<T>(this Component component, out T result)
        {
            result = component.GetComponentInParent<T>();
            return result != null;
        }

        public static bool TryGetComponentInChildren<T>(this Component component, out T result) 
        {
            result = component.GetComponentInChildren<T>();
            return result != null;
        }

        public static void AddDisableListener(this Component component, Action<GameObject> listener)
        {
            if (component == null)
                return;
            
            component.gameObject.AddDisableListener(listener);
        }

        public static void RemoveDisableListener(this Component component, Action<GameObject> listener)
        {
            if (component == null)
                return;

            component.gameObject.RemoveDisableListener(listener);
        }
    }
}