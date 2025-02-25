using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Rehawk.Foundation.Assets
{
    public static class AssetPostprocessorUtility
    {
        public static void MakeScriptableObjectIdUnique<T>(string[] importedAssets, string uidFieldName) where T : ScriptableObject
        {
            Type type = typeof(T);
            
            T[] allObjects = AssetHelper.FindAssetsOfType<T>();

            for (int i = 0; i < importedAssets.Length; i++)
            {
                if (type.IsAssignableFrom(AssetDatabase.GetMainAssetTypeAtPath(importedAssets[i])))
                {
                    var importedObject = AssetDatabase.LoadAssetAtPath<T>(importedAssets[i]);
                    
                    MakeIdUnique(importedAssets[i], importedObject, uidFieldName, allObjects);
                }
            }
        }
        
        public static void MakeComponentIdUnique<T>(string[] importedAssets, string uidFieldName, IReadOnlyList<T> allComponents) where T : MonoBehaviour
        {
            for (int i = 0; i < importedAssets.Length; i++)
            {
                var componentObject = AssetDatabase.LoadAssetAtPath<GameObject>(importedAssets[i]);

                if (componentObject && componentObject.TryGetComponent(out T importedComponent))
                {
                    MakeIdUnique(importedAssets[i], importedComponent, uidFieldName, allComponents);
                }
            }
        }

        private static void MakeIdUnique<T>(string importedAssetPath, T importedObject, string uidFieldName, IReadOnlyList<T> allObjects) 
            where T : Object
        {
            Type type = typeof(T);

            FieldInfo uidFieldInfo = type.GetField(uidFieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            string projectPath = Path.GetDirectoryName(Application.dataPath);
            string importedGuid = uidFieldInfo.GetValue(importedObject)?.ToString();

            if (string.IsNullOrEmpty(importedGuid))
            {
                uidFieldInfo.SetValue(importedObject, Guid.NewGuid().ToString());
                EditorUtility.SetDirty(importedObject);
            }
            else
            {
                T objectWithSameGuid = allObjects.FirstOrDefault(component =>
                {
                    if (component != importedObject)
                    {
                        object guid = uidFieldInfo.GetValue(component);
                        return guid.Equals(importedGuid);
                    }
                        
                    return false;
                });
                    
                if (objectWithSameGuid != null)
                {
                    var importedCreationTime = File.GetCreationTimeUtc(importedAssetPath);

                    string objectWithSameGuidPath = Path.Combine(projectPath, AssetDatabase.GetAssetPath(objectWithSameGuid));
                        
                    DateTime objectWithSameGuidCreationTime = File.GetCreationTimeUtc(objectWithSameGuidPath);

                    if (importedCreationTime >= objectWithSameGuidCreationTime && 
                        (!PrefabUtility.IsPartOfPrefabAsset(importedObject) || ((PrefabUtility.IsPartOfRegularPrefab(importedObject) && PrefabUtility.IsPartOfRegularPrefab(objectWithSameGuid)) || (PrefabUtility.IsPartOfVariantPrefab(importedObject) && PrefabUtility.IsPartOfRegularPrefab(objectWithSameGuid))) || (PrefabUtility.IsPartOfVariantPrefab(importedObject) && PrefabUtility.IsPartOfVariantPrefab(objectWithSameGuid))))
                    {
                        uidFieldInfo.SetValue(importedObject, Guid.NewGuid().ToString());
                        EditorUtility.SetDirty(importedObject);
                    }
                    else
                    {
                        uidFieldInfo.SetValue(objectWithSameGuid, Guid.NewGuid().ToString());
                        EditorUtility.SetDirty(objectWithSameGuid);
                    }
                }
            }
        }

        public static void HandleScriptableObjectDirectory<T, TDirectory>(string[] importedAssets, string directoryPath, string arrayFieldName)
            where T : Object
            where TDirectory : ScriptableObject
        {
            HandleScriptableObjectDirectory<T, T, TDirectory>(importedAssets, directoryPath, arrayFieldName);
        }

        public static void HandleScriptableObjectDirectory<TSource, TResult, TDirectory>(string[] importedAssets, string directoryPath, string arrayFieldName, Func<TSource, TResult> converter = null) 
            where TSource : Object 
            where TResult : Object 
            where TDirectory : ScriptableObject
        {
            Type type = typeof(TSource);
            Type directoryType = typeof(TDirectory);
            
            FieldInfo arrayFieldInfo = directoryType.GetField(arrayFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            
            var directory = AssetHelper.LoadOrCreateAssetAtPath<TDirectory>(directoryPath);

            TResult[] array = (TResult[]) arrayFieldInfo.GetValue(directory);
            List<TResult> list = array?.ToList();

            if (list == null)
            {
                list = new List<TResult>();
            }
            else
            {
                list.RemoveAll(e => e.Equals(null));
            }
            
            for (int i = 0; i < importedAssets.Length; i++)
            {
                if (type.IsAssignableFrom(AssetDatabase.GetMainAssetTypeAtPath(importedAssets[i])))
                {
                    var importedObject = AssetDatabase.LoadAssetAtPath<TSource>(importedAssets[i]);

                    var objToAdd = importedObject as TResult;
                    
                    if (converter != null)
                    {
                        objToAdd = converter.Invoke(importedObject);
                    }
                    
                    if (!list.Contains(objToAdd))
                    {
                        list.Add(objToAdd);
                    }
                }
            }
            
            arrayFieldInfo.SetValue(directory, list.ToArray());
            EditorUtility.SetDirty(directory);
        }
        
        public static void HandleComponentDirectory<TSource, TResult, TDirectory>(string[] importedAssets, string directoryPath, string arrayFieldName, Func<TSource, TResult> converter = null) 
            where TSource : Component 
            where TDirectory : ScriptableObject
        {
            Type directoryType = typeof(TDirectory);
            
            FieldInfo arrayFieldInfo = directoryType.GetField(arrayFieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            var directory = AssetHelper.LoadOrCreateAssetAtPath<TDirectory>(directoryPath);

            TResult[] array = (TResult[]) arrayFieldInfo.GetValue(directory);
            List<TResult> list = array?.ToList();

            if (list == null)
            {
                list = new List<TResult>();
            }
            else
            {
                list.RemoveAll(e => e.Equals(default(TResult)));
            }
            
            for (int i = 0; i < importedAssets.Length; i++)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(importedAssets[i]);
                if (prefab && prefab.TryGetComponent(out TSource component))
                {
                    TResult objToAdd = default;

                    if (component is TResult result)
                    {
                        objToAdd = result;
                    }
                    
                    if (converter != null)
                    {
                        objToAdd = converter.Invoke(component);
                    }

                    if (!list.Contains(objToAdd))
                    {
                        list.Add(objToAdd);
                    }
                }
            }
            
            arrayFieldInfo.SetValue(directory, list.ToArray());
            EditorUtility.SetDirty(directory);
        }
        
        public static void HandleComponentDirectory<TSource, TResult, TDirectory>(string[] importedAssets, string directoryPath, string arrayFieldName, Func<TDirectory, TSource, TResult> converter = null) 
            where TSource : Component 
            where TDirectory : ScriptableObject
        {
            Type directoryType = typeof(TDirectory);
            
            FieldInfo arrayFieldInfo = directoryType.GetField(arrayFieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            var directory = AssetHelper.LoadOrCreateAssetAtPath<TDirectory>(directoryPath);

            TResult[] array = (TResult[]) arrayFieldInfo.GetValue(directory);
            List<TResult> list = array?.ToList();

            if (list == null)
            {
                list = new List<TResult>();
            }
            else
            {
                list.RemoveAll(e => e.Equals(default(TResult)));
            }
            
            for (int i = 0; i < importedAssets.Length; i++)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(importedAssets[i]);
                if (prefab && prefab.TryGetComponent(out TSource component))
                {
                    TResult objToAdd = default;

                    if (component is TResult result)
                    {
                        objToAdd = result;
                    }
                    
                    if (converter != null)
                    {
                        objToAdd = converter.Invoke(directory, component);
                    }

                    if (!list.Contains(objToAdd))
                    {
                        list.Add(objToAdd);
                    }
                }
            }
            
            arrayFieldInfo.SetValue(directory, list.ToArray());
            EditorUtility.SetDirty(directory);
        }
        
        public static void ForEach<T>(string[] importedAssets, Action<T> action) 
            where T : Object
        {
            Type type = typeof(T);
            
            for (int i = 0; i < importedAssets.Length; i++)
            {
                if (type.IsAssignableFrom(AssetDatabase.GetMainAssetTypeAtPath(importedAssets[i])))
                {
                    var importedObject = AssetDatabase.LoadAssetAtPath<T>(importedAssets[i]);
                    action.Invoke(importedObject);
                }
            }
        }
    }
}