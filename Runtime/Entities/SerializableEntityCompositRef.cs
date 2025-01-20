using System;
using Newtonsoft.Json;
using Rehawk.Foundation.Misc;
using UnityEngine;

namespace Rehawk.Foundation.Entities
{
    public delegate void SerializedEntityCompositRefChangeHandler<in T>(T previousValue, T value) where T : IEntityComposit;
    
    [Serializable]
    public class SerializableEntityCompositRef<T> where T : IEntityComposit
    {
        [SerializeField] private string entityUid;
        
        private Entity entity;
        private T entityComposit;

        private SerializedEntityCompositRefChangeHandler<T> onChangedCallback;
        
        public SerializableEntityCompositRef()
        {
            EntityRegister.EntityAdded += OnEntityAdded;
            EntityRegister.EntityRemoved += OnEntityRemoved;
        }

        public SerializableEntityCompositRef(T composit) : this()
        {
            Value = composit;
        }

        ~SerializableEntityCompositRef()
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
        public T Value
        {
            get
            {
                if (!string.IsNullOrEmpty(entityUid) && (ObjectUtility.IsNull(entity) || ObjectUtility.IsNull(entityComposit)))
                {
                    var newEntity = EntityQuery.All().WithUid(entityUid);
                    if (newEntity)
                    {
                        Value = newEntity.GetComponent<T>();
                    }
                }
                
                return entityComposit;
            }
            set
            {
                if (ReferenceEquals(entityComposit, value))
                    return;

                var previousEntityComposit = entityComposit;
                
                entityComposit = value;

                if (ObjectUtility.IsNotNull(entityComposit))
                {
                    entity = entityComposit.Entity;
                    entityUid = entity.Uid;
                }
                
                onChangedCallback?.Invoke(previousEntityComposit, entityComposit);
            }
        }

        public void Clear()
        {
            Value = default;
        }

        public void OnChanged(SerializedEntityCompositRefChangeHandler<T> onChangedCallback)
        {
            this.onChangedCallback = onChangedCallback;
            onChangedCallback?.Invoke(default, entityComposit);
        }
        
        private void OnEntityAdded(Entity entity)
        {
            if (HasValue && Value.Entity.Uid != entityUid)
            {
                // Object references have changed. 
                Clear();
            }
        }
        
        private void OnEntityRemoved(Entity entity)
        {
            if (entityUid == entity.Uid)
            {
                // Reset object references but not the serialized uid.
                this.entity = null;
                this.entityComposit = default;
            }
        }
    }
}