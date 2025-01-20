using System;
using Rehawk.Foundation.Misc;
using UnityEngine;

namespace Rehawk.Foundation.Entities
{
    public static class EntityFactory
    {
        public static Entity SpawnByDefinition(EntityDefinition definition, Vector3 position, Quaternion rotation)
        {
            return SpawnByState(definition.CreateState(), position, rotation);
        }
        
        public static Entity SpawnByState(EntityState state, Vector3 position, Quaternion rotation)
        {
            GameObject prefab = null;
            if (ObjectUtility.IsNotNull(state.Definition))
            {
                prefab = state.Definition.Prefab;
            }

            if (prefab == null)
            {
                Debug.LogError(new Exception($"<b>{nameof(EntityFactory)}:</b> No prefab provided."));
            }
            else
            {
                GameObject entityObject = EntitiesInterface.Spawn(prefab, position, rotation);
                var entity = entityObject.GetComponent<Entity>();
                entity.SetState(state);

                return entity;
            }
            
            return null;
        }
        
        public static Entity SpawnByState(EntityState state)
        {
            return SpawnByState(state, state.Position, state.Rotation);
        }

        public static void Despawn(Entity entity)
        {
            EntitiesInterface.Despawn(entity.gameObject);
        }
    }
}