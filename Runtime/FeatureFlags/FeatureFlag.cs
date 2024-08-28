using UnityEngine;

namespace Rehawk.Foundation.FeatureFlags
{
    [CreateAssetMenu(menuName = "Foundation/Feature Flag", order = 800)]
    public class FeatureFlag : ScriptableObject
    {
        [SerializeField] private string uid;
        [SerializeField] private bool defaultValue;

        public string Uid
        {
            get { return uid; }
        }
        
        public bool Value
        {
            get { return Application.isEditor ? GetOverrideValue() : defaultValue; }
        }
        
        public bool GetOverrideValue()
        {
            return PlayerPrefs.GetInt(Uid, defaultValue ? 1 : 0) >= 1;
        }

        public void SetOverrideValue(bool value)
        {
            PlayerPrefs.SetInt(Uid, value ? 1 : 0);
        }

        public static bool IsSet(FeatureFlag featureFlag)
        {
            return featureFlag && featureFlag.Value;
        }
    }
}