using Rehawk.Foundation.Extensions;
using UnityEngine;

namespace Rehawk.Foundation.Entities
{
    public static class EntityExtensions
    {
        public static bool HasComponent<T>(this EntityDefinition definition)
        {
            return definition.IsValid() && definition.Prefab.TryGetComponent(out T _);
        }

        public static bool TryGetComponent<T>(this EntityDefinition definition, out T component)
        {
            component = default;
            return definition.IsValid() && definition.Prefab.TryGetComponent(out component);
        }

        public static TComponent GetComponent<TComponent>(this EntityDefinition definition)
        {
            return definition.IsValid() ? definition.Prefab.GetComponent<TComponent>() : default;
        }
        
        public static TComponent[] GetComponents<TComponent>(this EntityDefinition definition)
        {
            return definition.IsValid() ? definition.Prefab.GetComponents<TComponent>() : default;
        }

        public static bool HasComponent<T>(this Entity entity)
        {
            return entity != null && entity.TryGetComponent(out T _);
        }

        public static bool TryGetComponent<T>(this Entity entity, out T component)
        {
            component = default;
            return entity != null && entity.gameObject.TryGetComponent(out component);
        }

        public static TComponent GetComponent<TComponent>(this Entity entity)
        {
            return entity != null ? entity.gameObject.GetComponent<TComponent>() : default;
        }
        
        public static TComponent[] GetComponents<TComponent>(this Entity entity)
        {
            return entity != null ? entity.gameObject.GetComponents<TComponent>() : default;
        }

        public static TComponent GetOrAddComponent<TComponent>(this Entity entity)
            where TComponent : Component
        {
            return entity != null ? entity.gameObject.GetOrAddComponent<TComponent>() : default;
        }
        
        public static bool HasComponent<T>(this IEntityComposit composit)
        {
            return composit != null && composit.Entity.HasComponent<T>();
        }

        public static bool TryGetComponent<T>(this IEntityComposit composit, out T component)
        {
            component = default;
            return composit != null ? composit.Entity.TryGetComponent(out component) : default;
        }

        public static TComponent GetComponent<TComponent>(this IEntityComposit composit)
        {
            return composit != null ? composit.Entity.GetComponent<TComponent>() : default;
        }
        
        public static TComponent[] GetComponents<TComponent>(this IEntityComposit composit)
        {
            return composit != null ? composit.Entity.GetComponents<TComponent>() : default;
        }

        public static TComponent GetOrAddComponent<TComponent>(this IEntityComposit composit)
            where TComponent : Component
        {
            return composit != null ? composit.Entity.GetOrAddComponent<TComponent>() : default;
        }
    }
}