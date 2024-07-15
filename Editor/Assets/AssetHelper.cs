using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Rehawk.Foundation.Assets
{
    public static class AssetHelper
    {
        public static T LoadAssetFromGUID<T>(string guid) where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
        }
        
        public static T[] FindAssetsOfType<T>() where T : Object
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            T[] a = new T[guids.Length];
            for(int i =0;i<guids.Length;i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }
 
            return a;
        }

        public static Object[] FindAssetsOfType(Type type)
        {
            string[] guids = AssetDatabase.FindAssets("t:" + type.Name);
            var a = new Object[guids.Length];
            for(int i =0;i<guids.Length;i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath(path, type);
            }
 
            return a;
        }

        public static GameObject[] LoadPrefabsWithComponent<T>(bool includeChildren = false) where T : Component
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab");
            List<GameObject> result = new List<GameObject>();
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Object[] toCheck = AssetDatabase.LoadAllAssetsAtPath(path);
                foreach (Object obj in toCheck)
                {
                    GameObject go = obj as GameObject;
                    if (go == null)
                    {
                        continue;
                    }

                    Component comp = go.GetComponent(typeof(T));
                    if (comp != null)
                    {
                        result.Add(go);
                    }
                    else if(includeChildren)
                    {
                        Component[] comps = go.GetComponentsInChildren(typeof(T));
                        if (comps.Length > 0)
                        {
                            result.Add(go);
                        }
                    }
                }
            }

            return result.ToArray();
        }
        
        public static void CreateScriptableObject<T>() where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
 
            string path = "Assets";
  
            foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    path = Path.GetDirectoryName(path);
                    break;
                }
            }
 
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New" + typeof(T).Name + ".asset");
 
            AssetDatabase.CreateAsset(asset, assetPathAndName);
 
            Selection.activeObject = asset;
        }
    }
}