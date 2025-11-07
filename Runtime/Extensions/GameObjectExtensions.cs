using System;
using Rehawk.Foundation.Misc;
using UnityEngine;
using UnityEngine.VFX;

namespace Rehawk.Foundation.Extensions
{
    public static class GameObjectExtensions
    {
        public static bool HasComponent<T>(this GameObject gameObject)
        {
            return gameObject.TryGetComponent(out T _);
        }
        
        public static T GetOrAddComponent<T>(this GameObject gameObject) 
            where T : Component 
        {
            if (gameObject.TryGetComponent(out T newComponent))
            {
                return newComponent;
            }

            return gameObject.AddComponent<T>();
        }

        public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T result)
        {
            result = gameObject.GetComponentInParent<T>();
            return result != null;
        }

        public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T result)
        {
            result = gameObject.GetComponentInChildren<T>();
            return result != null;
        }

        public static void AddDisableListener(this GameObject gameObject, Action<GameObject> listener)
        {
            gameObject.GetOrAddComponent<EventBeforeDisable>().BeforeDisabled += listener;
        }

        public static void RemoveDisableListener(this GameObject gameObject, Action<GameObject> listener)
        {
            gameObject.GetOrAddComponent<EventBeforeDisable>().BeforeDisabled -= listener;
        }
        
        public static Bounds CalculateRendererBounds(this GameObject gameObject)
        {
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

            Bounds combinedBounds;
            
            if (renderers.Length == 0 || !IsBoundsRenderer(renderers[0]))
            {
                combinedBounds = new Bounds(gameObject.transform.position, Vector3.zero);
            }
            else
            {
                combinedBounds = renderers[0].bounds;
            }

            for (int i = 1; i < renderers.Length; i++)
            {
                Renderer renderer = renderers[i];
                
                if (!IsBoundsRenderer(renderer))
                    continue;
                
                combinedBounds.Encapsulate(renderer.bounds);
            }

            return combinedBounds;
        }

        private static bool IsBoundsRenderer(Renderer renderer)
        {
            return renderer is not ParticleSystemRenderer && renderer is not VFXRenderer;
        }
    }
}