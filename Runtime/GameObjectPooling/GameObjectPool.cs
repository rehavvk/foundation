using Rehawk.Foundation.Misc;
using Rehawk.Foundation.Pooling;
using UnityEngine;

namespace Rehawk.Foundation.GameObjectPooling
{
    public class GameObjectPool : Pool<Vector3, Quaternion, Transform, GameObject>
    {
        private readonly Transform parentForInactiveEffects;
        private readonly GameObject prefab;

        public GameObjectPool(Transform parentForInactiveEffects, GameObject prefab)
        {
            this.parentForInactiveEffects = parentForInactiveEffects;
            this.prefab = prefab;
        }

        protected override void OnCreate(GameObject item)
        {
            bool hadComponent = item.TryGetComponent(out PooledGameObject pooledGameObject);
            
            if (!hadComponent)
            {
                pooledGameObject = item.AddComponent<PooledGameObject>();
            }
            
            pooledGameObject.OnCreate(prefab, this);
            
            item.SetActive(false);
        }

        protected override void Reinitialize(Vector3 position, Quaternion rotation, Transform parent, GameObject item)
        {
            var pooledGameObject = item.GetComponent<PooledGameObject>();

            // Position + Rotation
            
            item.transform.SetPositionAndRotation(position, rotation);
            
            // Parenting
            
            if (parent)
            {
                if (item.TryGetComponent(out ParentConstraint constraint))
                {
                    constraint.InitWorld(parent, item.transform.position, item.transform.rotation);
                }
                else
                {
                    item.transform.SetParent(parent);
                }
            }

            // Scaling
            
            item.transform.localScale = Vector3.one;
            
            Vector3 lossyScale = item.transform.lossyScale;
            
            lossyScale.x = prefab.transform.lossyScale.x / lossyScale.x;
            lossyScale.y = prefab.transform.lossyScale.y / lossyScale.y;
            lossyScale.z = prefab.transform.lossyScale.z / lossyScale.z;

            item.transform.localScale = lossyScale;

            item.SetActive(true);
            
            pooledGameObject.OnReinitialize();
        }

        protected override void OnReturn(GameObject item)
        {
            var pooledGameObject = item.GetComponent<PooledGameObject>();

#if UNITY_EDITOR
            item.transform.SetParent(parentForInactiveEffects);
#endif
            
            pooledGameObject.OnReturn();
            
            item.SetActive(false);
        }

        protected override void OnDestroy(GameObject item)
        {
            if (item)
            {
                Object.Destroy(item);
            }
        }
    }
}