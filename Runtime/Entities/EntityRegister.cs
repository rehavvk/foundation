using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rehawk.Foundation.Entities
{
    public static class EntityRegister
    {
        private static List<Entity> allEntities;

        public static event Action<Entity> EntityAdded;
        public static event Action<Entity> EntityRemoved;

        public static IReadOnlyList<Entity> Entities
        {
            get { return allEntities; }
        }
        
        public static void RegisterEntity(Entity entity)
        {
            if (allEntities.Contains(entity))
                return;

            allEntities.Add(entity);
            EntityAdded?.Invoke(entity);
        }
        
        public static void UnregisterEntity(Entity entity)
        {
            if (!allEntities.Contains(entity))
                return;

            allEntities.Remove(entity);
            EntityRemoved?.Invoke(entity);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            allEntities = new List<Entity>();
            EntityAdded = null;
            EntityRemoved = null;
        }
    }
}