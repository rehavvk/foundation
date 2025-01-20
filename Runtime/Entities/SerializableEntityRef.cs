using System;
using Newtonsoft.Json;
using Rehawk.Foundation.Misc;
using UnityEngine;

namespace Rehawk.Foundation.Entities
{
    public delegate void SerializedEntityRefChangeHandler(Entity previousValue, Entity value);

    [Serializable]
    public class SerializableEntityRef
    {
        [SerializeField] private string entityUid;
        
        private Entity entity;
        
        private SerializedEntityRefChangeHandler onChangedCallback;

        public SerializableEntityRef()
        {
            EntityRegister.EntityAdded += OnEntityAdded;
            EntityRegister.EntityRemoved += OnEntityRemoved;
        }

        public SerializableEntityRef(Entity entity) : this()
        {
            Value = entity;
        }

        ~SerializableEntityRef()
        {
            EntityRegister.EntityAdded -= OnEntityAdded;
            EntityRegister.EntityRemoved -= OnEntityRemoved;
        }

        [JsonIgnore]
        public string EntityUid
        {
            get { return entityUid; }
        }

        [JsonIgnore]
        public bool HasValue
        {
            get { return ObjectUtility.IsNotNull(Value); }
        }

        [JsonIgnore]
        public Entity Value
        {
            get
            {
                if (!string.IsNullOrEmpty(entityUid) && ObjectUtility.IsNull(entity))
                {
                    Value = EntityQuery.All().WithUid(entityUid);
                }
                
                return entity;
            }
            set
            {
                if (ReferenceEquals(entity, value))
                    return;
                
                var previousEntity = entity;
                
                entity = value;
                entityUid = ObjectUtility.IsNotNull(entity) ? entity.Uid : string.Empty;
                
                onChangedCallback?.Invoke(previousEntity, entity);
            }
        }

        public void Clear()
        {
            Value = default;
        }
        
        public void OnChanged(SerializedEntityRefChangeHandler onChangedCallback)
        {
            this.onChangedCallback = onChangedCallback;
            onChangedCallback?.Invoke(default, entity);
        }

        private void OnEntityAdded(Entity entity)
        {
            if (HasValue && Value.Uid != entityUid)
            {
                // Object reference has changed. 
                Clear();
            }
        }
        
        private void OnEntityRemoved(Entity entity)
        {
            if (entityUid == entity.Uid)
            {
                // Reset object reference but not the serialized uid.
                this.entity = null;
            }
        }
    }
}