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
        public static void MakeScriptableObjectIdUnique<T>(string[] importedAssets, string idFieldName) where T : ScriptableObject
        {
            Type type = typeof(T);
            
            string projectPath = Path.GetDirectoryName(Application.dataPath);

            FieldInfo guidFieldInfo = type.GetField(idFieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            T[] objects = AssetHelper.FindAssetsOfType<T>();

            for (int i = 0; i < importedAssets.Length; i++)
            {
                if (type.IsAssignableFrom(AssetDatabase.GetMainAssetTypeAtPath(importedAssets[i])))
                {
                    var importedObject = AssetDatabase.LoadAssetAtPath<T>(importedAssets[i]);

                    string importedGuid = guidFieldInfo.GetValue(importedObject)?.ToString();

                    if (string.IsNullOrEmpty(importedGuid))
                    {
                        guidFieldInfo.SetValue(importedObject, Guid.NewGuid().ToString());
                        EditorUtility.SetDirty(importedObject);
                    }
                    else
                    {
                        T objectWithSameGuid = objects.FirstOrDefault(obj =>
                        {
                            if (obj != importedObject)
                            {
                                object guid = guidFieldInfo.GetValue(obj);

                                return guid.Equals(importedGuid);
                            }
                            
                            return false;
                        });
                        
                        if (objectWithSameGuid != null)
                        {
                            var importedCreationTime = File.GetCreationTimeUtc(importedAssets[i]);

                            string pathToLoadedObject = Path.Combine(projectPath, AssetDatabase.GetAssetPath(objectWithSameGuid));
                            
                            var loadedCreationTime = File.GetCreationTimeUtc(pathToLoadedObject);

                            if (importedCreationTime >= loadedCreationTime)
                            {
                                guidFieldInfo.SetValue(importedObject, Guid.NewGuid().ToString());
                                EditorUtility.SetDirty(importedObject);
                            }
                            else
                            {
                                guidFieldInfo.SetValue(importedObject, Guid.NewGuid().ToString());
                                EditorUtility.SetDirty(objectWithSameGuid);
                            }
                        }
                    }
                }
            }
        }
        
        public static void MakeComponentIdUnique<T>(string[] importedAssets, string idFieldName, IReadOnlyList<T> allComponents) where T : MonoBehaviour
        {
            Type type = typeof(T);

            string projectPath = Path.GetDirectoryName(Application.dataPath);

            FieldInfo guidFieldInfo = type.GetField(idFieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            for (int i = 0; i < importedAssets.Length; i++)
            {
                var componentObject = AssetDatabase.LoadAssetAtPath<GameObject>(importedAssets[i]);

                if (componentObject && componentObject.TryGetComponent(out T importedComponent))
                {
                    string importedGuid = guidFieldInfo.GetValue(importedComponent)?.ToString();

                    if (string.IsNullOrEmpty(importedGuid))
                    {
                        guidFieldInfo.SetValue(importedComponent, Guid.NewGuid().ToString());
                        EditorUtility.SetDirty(componentObject);
                    }
                    else
                    {
                        T componentWithSameGuid = allComponents.FirstOrDefault(component =>
                        {
                            if (component != importedComponent)
                            {
                                object guid = guidFieldInfo.GetValue(component);

                                return guid.Equals(importedGuid);
                            }
                        
                            return false;
                        });
                    
                        if (componentWithSameGuid != null)
                        {
                            var importedCreationTime = File.GetCreationTimeUtc(importedAssets[i]);

                            string pathToLoadedObject = Path.Combine(projectPath, AssetDatabase.GetAssetPath(componentWithSameGuid));
                        
                            var loadedCreationTime = File.GetCreationTimeUtc(pathToLoadedObject);

                            if (importedCreationTime >= loadedCreationTime)
                            {
                                guidFieldInfo.SetValue(importedComponent, Guid.NewGuid().ToString());
                                EditorUtility.SetDirty(importedComponent);
                            }
                            else
                            {
                                guidFieldInfo.SetValue(importedComponent, Guid.NewGuid().ToString());
                                EditorUtility.SetDirty(componentWithSameGuid);
                            }
                        }
                    }
                }
            }
        }
        
        public static void HandleScriptableObjectDirectory<T, TDirectory>(string[] importedAssets, string directoryPath, string arrayFieldName) 
            where T : Object 
            where TDirectory : ScriptableObject
        {
            Type type = typeof(T);
            Type directoryType = typeof(TDirectory);
            
            FieldInfo arrayFieldInfo = directoryType.GetField(arrayFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            
            var directory = AssetHelper.LoadOrCreateAssetAtPath<TDirectory>(directoryPath);

            T[] array = (T[]) arrayFieldInfo.GetValue(directory);
            List<T> list = array?.ToList();

            if (list == null)
            {
                list = new List<T>();
            }
            else
            {
                list.RemoveAll(e => e == null);
            }
            
            for (int i = 0; i < importedAssets.Length; i++)
            {
                if (type.IsAssignableFrom(AssetDatabase.GetMainAssetTypeAtPath(importedAssets[i])))
                {
                    var importedObject = AssetDatabase.LoadAssetAtPath<T>(importedAssets[i]);

                    if (!list.Contains(importedObject))
                    {
                        list.Add(importedObject);
                    }
                }
            }
            
            arrayFieldInfo.SetValue(directory, list.ToArray());
            EditorUtility.SetDirty(directory);
        }
        
        public static void HandleComponentDirectory<T, TDirectory>(string[] importedAssets, string directoryPath, string arrayFieldName) 
            where T : Component 
            where TDirectory : ScriptableObject
        {
            Type directoryType = typeof(TDirectory);
            
            FieldInfo arrayFieldInfo = directoryType.GetField(arrayFieldName, BindingFlags.Instance | BindingFlags.NonPublic);

            var directory = AssetHelper.LoadOrCreateAssetAtPath<TDirectory>(directoryPath);

            T[] array = (T[]) arrayFieldInfo.GetValue(directory);
            List<T> list = array?.ToList();

            if (list == null)
            {
                list = new List<T>();
            }
            else
            {
                list.RemoveAll(e => e == null);
            }
            
            for (int i = 0; i < importedAssets.Length; i++)
            {
                var entityObject = AssetDatabase.LoadAssetAtPath<GameObject>(importedAssets[i]);
                if (entityObject && entityObject.TryGetComponent(out T component))
                {
                    if (!list.Contains(component))
                    {
                        list.Add(component);
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