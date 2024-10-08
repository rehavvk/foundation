﻿using System;
using Rehawk.Foundation.Misc;
using UnityEngine;

namespace Rehawk.Foundation.Extensions
{
    public static class GameObjectExtensions
    {
        public static bool HasComponent<T>(this GameObject gameObject)
        {
            return gameObject.TryGetComponent(out T _);
        }
        
        public static T GetOrAddComponent<T>(this GameObject gameObject) 
            where T : Component 
        {
            if (gameObject.TryGetComponent(out T newComponent))
            {
                return newComponent;
            }

            return gameObject.AddComponent<T>();
        }

        public static void AddDisableListener(this GameObject gameObject, Action<GameObject> listener)
        {
            gameObject.GetOrAddComponent<EventBeforeDisable>().BeforeDisabled += listener;
        }

        public static void RemoveDisableListener(this GameObject gameObject, Action<GameObject> listener)
        {
            gameObject.GetOrAddComponent<EventBeforeDisable>().BeforeDisabled -= listener;
        }
    }
}