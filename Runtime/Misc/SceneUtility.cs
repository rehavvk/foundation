using UnityEngine.SceneManagement;

namespace Rehawk.Foundation.Misc
{
    public static class SceneUtility
    {
        public static int GetBuildIndexByName(string sceneName)
        {
            int sceneCount = SceneManager.sceneCountInBuildSettings;

            for (int i = 0; i < sceneCount; i++)
            {
                string path = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
                string name = System.IO.Path.GetFileNameWithoutExtension(path);

                if (name == sceneName)
                    return i;
            }

            return -1;
        }
    }
}