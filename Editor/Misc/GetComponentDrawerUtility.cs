using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Rehawk.Foundation.Misc
{
    internal static class GetComponentDrawerUtility
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static void CollectOwnerComponents(Object targetObject, List<Component> results, HashSet<int> seenInstanceIds)
        {
            if (!targetObject)
                return;

            if (targetObject is Component component)
            {
                AddComponentUnique(component, results, seenInstanceIds);
                return;
            }

            if (targetObject is GameObject gameObject)
            {
                AddGameObjectComponents(gameObject, results, seenInstanceIds);
                return;
            }

            Type targetType = targetObject.GetType();

            FieldInfo[] fields = targetType.GetFields(Flags);
            for (int i = 0; i < fields.Length; i++)
            {
                object value = fields[i].GetValue(targetObject);
                if (value is Component fieldComponent)
                {
                    AddComponentUnique(fieldComponent, results, seenInstanceIds);
                    continue;
                }

                if (value is GameObject fieldGameObject)
                {
                    AddGameObjectComponents(fieldGameObject, results, seenInstanceIds);
                }
            }

            PropertyInfo[] properties = targetType.GetProperties(Flags);
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];
                if (!property.CanRead || property.GetIndexParameters().Length > 0)
                    continue;

                object value;
                try
                {
                    value = property.GetValue(targetObject, null);
                }
                catch
                {
                    continue;
                }

                if (value is Component propertyComponent)
                {
                    AddComponentUnique(propertyComponent, results, seenInstanceIds);
                    continue;
                }

                if (value is GameObject propertyGameObject)
                {
                    AddGameObjectComponents(propertyGameObject, results, seenInstanceIds);
                }
            }
        }

        private static void AddGameObjectComponents(GameObject gameObject, List<Component> results, HashSet<int> seenInstanceIds)
        {
            if (!gameObject)
                return;

            Component[] components = gameObject.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
                AddComponentUnique(components[i], results, seenInstanceIds);
        }

        private static void AddComponentUnique(Component component, List<Component> results, HashSet<int> seenInstanceIds)
        {
            if (!component)
                return;

            int instanceId = component.GetInstanceID();
            if (seenInstanceIds.Add(instanceId))
                results.Add(component);
        }
    }
}
