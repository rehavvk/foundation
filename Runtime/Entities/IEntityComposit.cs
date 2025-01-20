using System;
using Rehawk.Foundation.Misc;
using UnityEngine;

namespace Rehawk.Foundation.Entities
{
    public interface IEntityComposit
    {
        public event Action<IEntityComposit> Activated;
        public event Action<IEntityComposit> Deactivated;
        
        Entity Entity { get; }
        EntityDefinition Definition { get; }

        void Despawn();
        void SetState(EntityCompositStateBase state);
        EntityCompositStateBase GetState();
    }
    
    [RequireComponent(typeof(Entity))]
    public abstract class EntityCompositBase : EntityBehaviour, IEntityComposit
    {
        private Entity unconcreteEntity;

        private EntityCompositStateBase state;
        
        public event Action<IEntityComposit> Activated;
        public event Action<IEntityComposit> Deactivated;

        public Entity Entity
        {
            get { return unconcreteEntity ??= GetComponent<Entity>(); }
        }
        
        public EntityDefinition Definition
        {
            get { return Entity.Definition; }
        }

        public void SetState(EntityCompositStateBase state)
        {
            this.state = state;
            
            if (!IsActivated || state == null)
                return;

            OnApplyState(state);
        }

        public EntityCompositStateBase GetState()
        {
            ValidateState();
            UpdateState();

            return state;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            
            // Set state if state was set before setup.
            if (state != null)
            {
                SetState(state);
            }
            else
            {
                ValidateState();
            }
            
            Activated?.Invoke(this);
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();

            Deactivated?.Invoke(this);

            state = null;
        }

        protected virtual void OnApplyState(EntityCompositStateBase state) {}
        protected virtual void OnUpdateState(EntityCompositStateBase state) {}

        protected virtual EntityCompositStateBase CreateState()
        {
            return null;
        }
        
        private void ValidateState()
        {
            if (ObjectUtility.IsNull(state))
            {
                SetState(CreateState());
            }
        }
        
        private void UpdateState()
        {
            OnUpdateState(state);
        }
    }
}