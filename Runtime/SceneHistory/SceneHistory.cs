using System.Collections.Generic;
using Rehawk.Foundation.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rehawk.Foundation.SceneHistory
{
    public static class SceneHistory
    {
        private static readonly List<SceneHistoryRecord> history = new List<SceneHistoryRecord>();

        static SceneHistory()
        {
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        public static SceneHistoryRecord GetPrevious()
        {
            return Get(1);
        }
        
        public static SceneHistoryRecord Get(int stepsBack)
        {
            int index = history.Count - 1 - stepsBack;
            if (history.TryGetValue(index, out SceneHistoryRecord record))
            {
                return record;
            }

            return SceneHistoryRecord.Empty;
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void OnSubsystemRegistration()
        {
            history.Clear();
        }

        private static void OnActiveSceneChanged(Scene previousScene, Scene newScene)
        {
            history.Add(new SceneHistoryRecord
            {
                SceneName = newScene.name,
                BuildIndex = newScene.buildIndex
            });
        }
    }
}