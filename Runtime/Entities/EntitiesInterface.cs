using System;
using UnityEngine;

namespace Rehawk.Foundation.Entities
{
    public static class EntitiesInterface
    {
        private static Func<EntityDefinitionDirectory> getDirectoryCallback;
        private static Func<GameObject, Vector3, Quaternion, GameObject> spawnCallback;
        private static Action<GameObject> despawnCallback;
        
        public static void Initialize(Func<EntityDefinitionDirectory> getDirectoryCallback, 
                                      Func<GameObject, Vector3, Quaternion, GameObject> spawnCallback, 
                                      Action<GameObject> despawnCallback)
        {
            EntitiesInterface.getDirectoryCallback = getDirectoryCallback;
            EntitiesInterface.spawnCallback = spawnCallback;
            EntitiesInterface.despawnCallback = despawnCallback;
        }

        public static EntityDefinitionDirectory GetDirectory()
        {
            return getDirectoryCallback?.Invoke();
        }
        
        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return spawnCallback?.Invoke(prefab, position, rotation);
        }
        
        public static void Despawn(GameObject obj)
        {
            despawnCallback?.Invoke(obj);
        }
    }
}