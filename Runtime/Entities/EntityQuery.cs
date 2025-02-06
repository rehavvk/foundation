using System;
using System.Collections.Generic;
using System.Linq;

namespace Rehawk.Foundation.Entities
{
    public static class EntityQuery
    {
        public static IEnumerable<Entity> All(bool includeInactive = false)
        {
            return EntityRegister.Entities
                                 .Where(e => e.IsActivated || includeInactive);
        }
        
        public static Entity WithUid(this IEnumerable<Entity> query, string uid)
        {
            return query.FirstOrDefault(e => e.Uid == uid);
        }
        
        public static EntityState WithUid(this IEnumerable<EntityState> query, string uid)
        {
            return query.FirstOrDefault(e => e.Uid == uid);
        }
        
        public static IEnumerable<Entity> WithDefinition(this IEnumerable<Entity> query, EntityDefinition definition)
        {
            return query.Where(e => e.Definition == definition);
        }
        
        public static IEnumerable<EntityState> WithDefinition(this IEnumerable<EntityState> query, EntityDefinition definition)
        {
            return query.Where(e => e.Definition == definition);
        }
        
        public static IEnumerable<Entity> WithComponent<T>(this IEnumerable<Entity> query)
        {
            return query.Where(e => e.HasComponent<T>());
        }
        
        public static IEnumerable<Entity> WithComponent<T>(this IEnumerable<Entity> query, Predicate<T> predicate)
        {
            return query.Where(e => e.TryGetComponent<T>(out T component) && predicate.Invoke(component));
        }
    }
}