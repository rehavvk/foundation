using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Rehawk.Foundation.GameObjectPooling
{
    public class GameObjectPoolManager : MonoBehaviour
    {
        [SerializeField] private PoolDefinition[] definitions;

        private int nextPrewarmQueueIndex = 0;
        private readonly List<PrewarmInfo> prewarmQueue = new List<PrewarmInfo>();
        
        private int incrementIndex;
        private readonly Dictionary<GameObject, int> prefabToId = new Dictionary<GameObject, int>();
        private readonly Dictionary<int, GameObject> idToPrefab = new Dictionary<int, GameObject>();
        
        private readonly Dictionary<GameObject, GameObjectPool> prefabToPool = new Dictionary<GameObject, GameObjectPool>();
        private readonly List<GameObjectPool> pools = new List<GameObjectPool>();
        
        private readonly List<GameObject> activeObjects = new List<GameObject>();
        
        public IReadOnlyCollection<GameObjectPool> Pools
        {
            get { return pools; }
        }

        public IReadOnlyList<GameObject> ActiveObjects
        {
            get { return activeObjects; }
        }

        private void Start()
        {
            for (int i = 0; i < definitions.Length; i++)
            {
                PoolDefinition definition = definitions[i];
                
                AddPoolFor(definition.Prefab, definition.Size);
            }
        }

        private void OnDestroy()
        {
            foreach (GameObjectPool pool in pools)
            {
                pool.Clear();
            }
        }

        private void Update()
        {
            if (prewarmQueue.Count > 0)
            {
                PrewarmInfo prewarmInfo = prewarmQueue[nextPrewarmQueueIndex];
                
                if (prewarmInfo.LeftAmount > 0)
                {
                    prewarmInfo.Pool.Resize(prewarmInfo.Pool.NumTotal + prewarmInfo.PrewarmStep);
                    prewarmInfo.LeftAmount -= prewarmInfo.PrewarmStep;
                }
                else
                {
                    prewarmQueue.Remove(prewarmInfo);
                }

                if (prewarmQueue.Count > 0)
                {
                    nextPrewarmQueueIndex = ((nextPrewarmQueueIndex + 1) % prewarmQueue.Count + prewarmQueue.Count) % prewarmQueue.Count;
                }
            }
        }

        public int PrefabToId(GameObject prefab)
        {
            if (prefabToId.TryGetValue(prefab, out int index))
            {
                return index;
            }
            else
            {
                Debug.LogError($"<b>{nameof(GameObjectPoolManager)}: </b> Could not get id of prefab. [prefab={prefab.name}]");
            }

            return -1;
        }

        public GameObject IdToPrefab(int prefabId)
        {
            if (idToPrefab.TryGetValue(prefabId, out GameObject prefab))
            {
                return prefab;
            }
            else
            {
                Debug.LogError($"<b>{nameof(GameObjectPoolManager)}: </b> Could not get prefab from id. [id={prefabId}]");
            }

            return null;
        }

        public void AddPoolFor(GameObject prefab, int prewarmAmount)
        {
            if (prefabToPool.TryGetValue(prefab, out GameObjectPool pool))
            {
                AddToOrIncreasePrewarmQueue(pool, prewarmAmount);
                return;
            }

            int id = incrementIndex;
            incrementIndex++;
            
            var backupParent = new GameObject("POOL : " + prefab.name);
#if UNITY_EDITOR
            backupParent.transform.hierarchyCapacity = prewarmAmount;
#endif
            Object.DontDestroyOnLoad(backupParent);
            
            pool = new GameObjectPool(backupParent.transform, prefab);

            prefab.SetActive(false);
#if UNITY_EDITOR
            GameObject original = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, backupParent.transform);
#else
            GameObject original = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
            Object.DontDestroyOnLoad(original);
#endif
            original.name = $"{prefab.name}_Original";
            prefab.SetActive(true);
            
            pool.SetFactory(() =>
            {
#if UNITY_EDITOR
                GameObject cloned = Object.Instantiate(original, Vector3.zero, Quaternion.identity, backupParent.transform);
#else
                GameObject cloned = Object.Instantiate(original, Vector3.zero, Quaternion.identity);
                Object.DontDestroyOnLoad(cloned);
#endif
                cloned.name = $"{prefab.name}_{pool.NumTotal}";
                cloned.SetActive(true);

                return cloned;
            });
            
            pool.Prewarm(1);

            AddToOrIncreasePrewarmQueue(pool, prewarmAmount);

            prefabToId.Add(prefab, id);
            idToPrefab.Add(id, prefab);
            
            prefabToPool.Add(prefab, pool);
            pools.Add(pool);
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            if (prefab)
            {
                if (prefabToPool.TryGetValue(prefab, out GameObjectPool pool))
                {
                    GameObject obj = pool.Pop(position, rotation, parent);
                    activeObjects.Add(obj);
                    return obj;
                }
            }
            else
            {
                Debug.LogError($"<b>{nameof(GameObjectPoolManager)}: </b> Tried to spawn null.");
            }

            return null;
        }

        public GameObject Spawn(GameObject prefab, Transform parent = null)
        {
            return Spawn(prefab, Vector3.zero, Quaternion.identity, parent);
        }

        public GameObject Spawn(int prefabId, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            GameObject prefab = IdToPrefab(prefabId);
            
            return Spawn(prefab, position, rotation, parent);
        }

        public bool Return(GameObject instance)
        {
            if (instance && instance.TryGetComponent(out PooledGameObject pooledGameObject) && 
                pooledGameObject.Prefab && prefabToPool.TryGetValue(pooledGameObject.Prefab, out GameObjectPool pool))
            {
                try
                {
                    activeObjects.Remove(instance);
                    pool.Return(instance);
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogError($"<b>{nameof(GameObjectPoolManager)}:</b> Exception while returning GameObject {e.Message}");    
                
                    return false;
                }
            }

            return false;
        }

        private void AddToOrIncreasePrewarmQueue(GameObjectPool pool, int prewarmAmount)
        {
            PrewarmInfo prewarmInfo = prewarmQueue.FirstOrDefault(e => e.Pool == pool);
                
            if (prewarmInfo == null)
            {
                prewarmQueue.Add(new PrewarmInfo
                {
                    Pool = pool,
                    LeftAmount = GetLeftAmount(pool, prewarmAmount - 1),
                    PrewarmStep = 1
                });
            }
            else
            {
                prewarmInfo.LeftAmount = GetLeftAmount(pool, prewarmAmount - prewarmInfo.LeftAmount);
            }
        }
        
        private int GetLeftAmount(GameObjectPool pool, int poolWantedSize)
        {
            return Mathf.Max(0, poolWantedSize - pool.NumTotal);
        }
        
        [Serializable]
        private class PoolDefinition
        {
            public GameObject Prefab;
            public int Size;
        }
        
        private class PrewarmInfo
        {
            public GameObjectPool Pool;
            public int LeftAmount;
            public int PrewarmStep;
        }
    }
}