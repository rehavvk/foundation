using System.Collections;
using UnityEngine;

namespace Rehawk.Foundation.Entities
{
    public abstract class EntityBehaviour : MonoBehaviour
    {
        public bool IsInitialized { get; private set; }
        public bool IsSetupForActivation { get; private set; }
        public bool IsActivated { get; private set; }

        protected virtual void OnEnable() {}

        protected virtual void Awake() {}
        
        protected virtual void Start() {}

        protected virtual void OnDisable()
        {
            Deactivate();
        }

        protected virtual void OnDestroy() {}

        private void Update()
        {
            SetupForActivation();
            
            if (!IsActivated)
                return;
            
            OnUpdate();
        }

        private void FixedUpdate()
        {
            if (!IsActivated)
                return;
            
            OnFixedUpdate();
        }

        private void LateUpdate()
        {
            if (!IsActivated)
                return;
            
            OnLateUpdate();
        }

        protected virtual void OnUpdate() {}

        protected virtual void OnFixedUpdate() {}

        protected virtual void OnLateUpdate() {}

        public void Despawn()
        {
            Despawn(gameObject);
        }

        public void Despawn(GameObject obj)
        {
            EntitiesInterface.Despawn(obj);
        }

        /// <summary>
        /// Is called when the object is set up for activation.
        /// </summary>
        protected virtual void OnSetupForActivation() {}

        /// <summary>
        /// Is called when the object is activated the first time.
        /// </summary>
        protected virtual void OnInitialize() {}

        /// <summary>
        /// Is called when the object is activated.
        /// </summary>
        protected virtual void OnActivate() {}

        /// <summary>
        /// Is called when the object is deactivated.
        /// </summary>
        protected virtual void OnDeactivate() {}
        
        private void SetupForActivation()
        {
            if (IsSetupForActivation) 
                return;

            IsSetupForActivation = true;
            OnSetupForActivation();

            StartCoroutine(DelayedActivationRoutine());
        }

        private void Deactivate()
        {
            if (!IsSetupForActivation)
                return;
            
            IsSetupForActivation = false;
            StopAllCoroutines();
            
            if (!IsActivated)
                return;

            IsActivated = false;
            OnDeactivate();
        }

        private IEnumerator DelayedActivationRoutine()
        {
            yield return new WaitForEndOfFrame();
            
            if (!IsInitialized)
            {
                IsInitialized = true;
                OnInitialize();
            }

            IsActivated = true;
            OnActivate();
        }
    }
}