using UnityEngine;

namespace Rehawk.Foundation.CustomYieldInstructions
{
    public class WaitForCoroutineHelper : MonoBehaviour
    {
        private static WaitForCoroutineHelper instance;

        public static WaitForCoroutineHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    var obj = new GameObject(nameof(WaitForCoroutineHelper));
                    instance = obj.AddComponent<WaitForCoroutineHelper>();
                }

                return instance;
            }
        }

        private void Start()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}