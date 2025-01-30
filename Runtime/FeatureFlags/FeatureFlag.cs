using UnityEngine;

namespace Rehawk.Foundation.FeatureFlags
{
    public delegate void FeatureFlagChangedDelegate(FeatureFlag featureFlag, bool previousValue, bool value);
    
    [CreateAssetMenu(menuName = "Foundation/Feature Flag", order = 800)]
    public class FeatureFlag : ScriptableObject
    {
        [SerializeField] private string uid;

        [SerializeField] private string title;
        [SerializeField] private bool defaultValue;

        public event FeatureFlagChangedDelegate Changed;
        
        public string Uid
        {
            get { return uid; }
        }

        public string Title
        {
            get { return !string.IsNullOrEmpty(title) ? title : name; }
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
            bool previousValue = GetOverrideValue();
            
            if (previousValue == value)
                return;
            
            PlayerPrefs.SetInt(Uid, value ? 1 : 0);
            
            Changed?.Invoke(this, previousValue, value);
        }

        public static bool IsSet(FeatureFlag featureFlag)
        {
            return featureFlag && featureFlag.Value;
        }
    }
}