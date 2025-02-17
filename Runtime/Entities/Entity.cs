using System;
using Rehawk.Foundation.Spawning;
using Rehawk.Foundation.Misc;
using UnityEngine;
using Semaphore = Rehawk.Foundation.Misc.Semaphore;

#if ODIN_INSPECTOR_3
using Sirenix.OdinInspector;
#endif

namespace Rehawk.Foundation.Entities
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GUID.GUID))]
    [SelectionBase]
    public sealed class Entity : EntityBehaviour
    {
#if ODIN_INSPECTOR_3
        [BoxGroup("General", false), ReadOnly]
#endif
        [SerializeField] private string definitionUid;
        
#if ODIN_INSPECTOR_3
        [BoxGroup("Visuals", false)] 
#endif
        [SerializeField] private Transform visibilityRoot;

        private EntityDefinitionDirectory entityDefinitionDirectory;
        
        private GUID.GUID guid;

        private EntityDefinition definition;
        private EntityState state;

        public event Action<Entity> Activated;
        public event Action<Entity> Deactivated;

        public string Uid
        {
            get
            {
                if (guid == null)
                {
                    guid = GetComponent<GUID.GUID>();
                }
                
                if (guid.IsGuidAssigned())
                {
                    return guid.Value;
                }

                return definitionUid;
            }
        }

        public EntityDefinition Definition
        {
            get
            {
                if (!definition.IsValid())
                {
                    definition = EntityDefinitionDirectory.Find(definitionUid);
                }
                
                return definition;
            }
        }

        public Vector3 Position
        {
            get { return transform.position; }
            set { transform.position = value; }
        }
        
        public Quaternion Rotation
        {
            get { return transform.rotation; }
            set { transform.rotation = value; }
        }
        
        public Transform Transform
        {
            get { return transform; }
        }
        
        public float BornTimestamp { get; private set; } = -1;
        public float LastActiveTimestamp { get; private set; } = -1;
        
        public float SpawnTimestamp { get; private set; } = -1;
        public Vector3 SpawnPosition { get; private set; }

        public Semaphore IsInvisible { get; } = new Semaphore();
        public Semaphore InUse { get; } = new Semaphore();

        private EntityDefinitionDirectory EntityDefinitionDirectory
        {
            get { return entityDefinitionDirectory ??= EntitiesInterface.GetDirectory(); }
        }

        protected override void Awake()
        {
            base.Awake();

            UpdateVisibility();
            
            IsInvisible.StateChanged += OnIsInvisibleStateChanged;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            IsInvisible.StateChanged -= OnIsInvisibleStateChanged;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            LastActiveTimestamp = Time.realtimeSinceStartup;
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                definition = new EntityDefinition();
            }
        }

        public void SetState(EntityState state)
        {
            this.state = state;

            SpawnTimestamp = Time.realtimeSinceStartup;
            SpawnPosition = Position;

            if (state != null && !string.IsNullOrEmpty(state.Uid) && Uid != state.Uid)
            {
                guid.SetGuid(state.Uid);
            }

            if (!IsActivated || state == null)
                return;

            if (state.BornTimestamp <= 0)
            {
                state.BornTimestamp = Time.realtimeSinceStartup;
                state.LastActiveTimestamp = Time.realtimeSinceStartup;
            }
            
            BornTimestamp = state.BornTimestamp;
            LastActiveTimestamp = state.LastActiveTimestamp;
            
            EntityCompositBase[] composits = GetComponents<EntityCompositBase>();
            foreach (EntityCompositBase composit in composits)
            {
                if (state.TryGetCompositState(composit.GetType(), out EntityCompositStateBase compositState))
                {
                    composit.SetState(compositState);
                }
            }
        }

        public EntityState GetState()
        {
            ValidateState();
            UpdateState();

            return state;
        }

        protected override void OnSetupForActivation()
        {
            base.OnSetupForActivation();
            
            EntityRegister.RegisterEntity(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            
            // Set state if state was set before activation.
            if (state != null)
            {
                SetState(state);
            }
            else
            {
                ValidateState();
            }

            UpdateVisibility();
            
            Activated?.Invoke(this);
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();

            InUse.Clear();
            
            UpdateVisibility();
            
            Deactivated?.Invoke(this);
            
            EntityRegister.UnregisterEntity(this);
            
            state = null;
        }

        private void ValidateState()
        {
            if (ObjectUtility.IsNull(state))
            {
                SetState(CreateState());
            }
        }

        public EntityState CreateState()
        {
            var newState = new EntityState();

            UpdateStatePrefab(newState);

            return newState;
        }

        private void UpdateState()
        {
            state.Uid = Uid;

            UpdateStatePrefab(state);
            
            state.BornTimestamp = BornTimestamp;
            state.LastActiveTimestamp = LastActiveTimestamp;
            
            state.Position = transform.position;
            state.Rotation = transform.rotation;
            state.Scale = transform.localScale;

            EntityCompositBase[] composits = GetComponents<EntityCompositBase>();
            foreach (EntityCompositBase composit in composits)
            {
                state.SetCompositState(composit.GetType(), composit.GetState());
            }
        }

        private void UpdateStatePrefab(EntityState state)
        {
            if (Definition.IsValid())
            {
                state.Prefab = Definition.Prefab;
            }
            else if (TryGetComponent(out PrefabInstance prefabInstance))
            {
                state.Prefab = prefabInstance.Prefab;
            }
            else
            {
                state.Prefab = gameObject;
            }
        }

        private void UpdateVisibility()
        {
            if (visibilityRoot == null)
                return;
            
            visibilityRoot.gameObject.SetActive(IsActivated && !IsInvisible);
        }
        
        private void OnIsInvisibleStateChanged()
        {
            UpdateVisibility();
        }
    }
}