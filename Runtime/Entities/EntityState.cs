using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Rehawk.Foundation.Entities
{ 
    [Serializable]
    public sealed class EntityState
    {
        public string Uid;

        public GameObject Prefab;
        
        public float BornTimestamp;
        public float LastActiveTimestamp;
        
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        [SerializeField] private Dictionary<Type, EntityCompositStateBase> compositTypeToState = new Dictionary<Type, EntityCompositStateBase>();

        [JsonIgnore]
        public EntityDefinition Definition
        {
            get
            {
                if (Prefab)
                {
                    return EntityDefinition.From(Prefab.GetComponent<Entity>());
                }

                return EntityDefinition.None();
            }
        }
        
        // TODO: Maybe it would be a better approach to safe the composits based on the state type and not on the composit type.
        public void SetCompositState(Type compositType, EntityCompositStateBase compositState)
        {
            compositTypeToState[compositType] = compositState;
        }
        
        public bool TryGetCompositState(Type compositType, out EntityCompositStateBase compositState)
        {
            return compositTypeToState.TryGetValue(compositType, out compositState);
        }
        
        public bool TryGetCompositState<TState>(Type compositType, out TState compositState) 
            where TState : EntityCompositStateBase
        {
            compositState = null;
            
            if (TryGetCompositState(compositType, out EntityCompositStateBase boxedCompositState))
            {
                compositState = (TState) boxedCompositState;
                return true;
            }

            return false;
        }
    }
}