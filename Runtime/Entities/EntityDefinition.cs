using System;
using UnityEngine;

#if ODIN_INSPECTOR_3
using Sirenix.OdinInspector;
#endif

namespace Rehawk.Foundation.Entities
{
    [Serializable]
    [InlineProperty]
    public struct EntityDefinition : IEquatable<EntityDefinition>
    {
#if ODIN_INSPECTOR_3
        [HideLabel]
#endif
        [SerializeField] private Entity entity;

        public string Uid
        {
            get { return entity.Uid; }
        }

        public GameObject Prefab
        {
            get { return entity.gameObject; }
        }
        
        public bool IsValid()
        {
            return entity != null;
        }

        public EntityState CreateState()
        {
            return entity.CreateState();
        }
        
        public bool Equals(EntityDefinition other)
        {
            return entity == other.entity;
        }

        public override bool Equals(object obj)
        {
            return obj is EntityDefinition other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (IsValid() ? entity.GetHashCode() : 0);
        }
        
        public static bool operator ==(EntityDefinition definition1, EntityDefinition definition2)
        {
            return definition1.Equals(definition2);
        }

        public static bool operator !=(EntityDefinition definition1, EntityDefinition definition2)
        {
            return !definition1.Equals(definition2);
        }

        public static implicit operator bool(EntityDefinition definition)
        {
            return definition.entity != null;
        }
        
        public static EntityDefinition None()
        {
            return new EntityDefinition();
        }
        
        public static EntityDefinition From(Entity entity)
        {
            if (!string.IsNullOrEmpty(entity.gameObject.scene.name))
            { 
                entity = entity.Definition.entity;
            }

            return new EntityDefinition
                   {
                       entity = entity
                   };
        }
    }
}