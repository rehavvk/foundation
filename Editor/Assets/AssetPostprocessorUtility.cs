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
        public static void MakeIdUnique<T>(string[] importedAssets, string guidFieldName) where T : Object
        {
            Type type = typeof(T);
            
            string projectPath = Path.GetDirectoryName(Application.dataPath);

            FieldInfo guidFieldInfo = type.GetField(guidFieldName, BindingFlags.Instance | BindingFlags.NonPublic);

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

        public static void HandleDirectory<T, TDirectory>(string[] importedAssets, string directoryPath, string arrayFieldName) 
            where T : Object 
            where TDirectory : ScriptableObject
        {
            Type type = typeof(T);
            Type directoryType = typeof(TDirectory);
            
            FieldInfo arrayFieldInfo = directoryType.GetField(arrayFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            
            var directory = AssetDatabase.LoadAssetAtPath<TDirectory>(directoryPath);
            if (directory == null)
            {
                directory = ScriptableObject.CreateInstance<TDirectory>();
                
                AssetDatabase.CreateAsset(directory, directoryPath);
            }

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