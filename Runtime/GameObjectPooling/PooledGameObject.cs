using UnityEngine;

namespace Rehawk.Foundation.GameObjectPooling
{
    [DisallowMultipleComponent]
    public class PooledGameObject : MonoBehaviour
    {
        private IPoolCreateHandler[] createHandlers;
        private IPoolReinitializeHandler[] reinitializeHandlers;
        private IPoolReturnHandler[] returnHandlers;
        
        private GameObject prefab;
        private GameObjectPool pool;
        private float reinitializedTimestamp;
        private float returnedTimestamp;

        public GameObject Prefab
        {
            get { return prefab; }
        }

        public GameObjectPool Pool
        {
            get { return pool; }
        }

        public float ReinitializedTimestamp
        {
            get { return reinitializedTimestamp; }
        }

        public float ReturnedTimestamp
        {
            get { return returnedTimestamp; }
        }

        protected virtual void Awake()
        {
            createHandlers = GetComponentsInChildren<IPoolCreateHandler>();
            reinitializeHandlers = GetComponentsInChildren<IPoolReinitializeHandler>();
            returnHandlers = GetComponentsInChildren<IPoolReturnHandler>();
        }

        public virtual void OnCreate(GameObject prefab, GameObjectPool pool)
        {
            this.prefab = prefab;
            this.pool = pool;

            for (int i = 0; i < createHandlers.Length; i++)
            {
                createHandlers[i].OnCreate();
            }
        }
        
        public virtual void OnReinitialize()
        {
            reinitializedTimestamp = Time.realtimeSinceStartup;

            for (int i = 0; i < reinitializeHandlers.Length; i++)
            {
                reinitializeHandlers[i].OnReinitialize();
            }
        }

        public virtual void OnReturn()
        {
            returnedTimestamp = Time.realtimeSinceStartup;
            
            for (int i = 0; i < returnHandlers.Length; i++)
            {
                returnHandlers[i].OnReturn();
            }
        }
        
        private void OnDestroy()
        {
            if (Pool != null)
            {
                Pool.Remove(gameObject);
            }
        }
    }
}