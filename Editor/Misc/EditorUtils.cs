using System.Linq;
using UnityEditor;

namespace Rehawk.Foundation.Misc
{
    public static class EditorUtils
    {
        /// <summary>
        /// Adds the scene with the given path to build settings as enabled.
        /// </summary>
        public static void AddSceneToBuild(string scenePath)
        {
            var tempScenes = EditorBuildSettings.scenes.ToList();
            tempScenes.Add(new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = tempScenes.ToArray();
        }
    }
}