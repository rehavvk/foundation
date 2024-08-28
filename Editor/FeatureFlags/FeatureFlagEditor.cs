using UnityEditor;
using UnityEngine;

namespace Rehawk.Foundation.FeatureFlags
{
    [CustomEditor(typeof(FeatureFlag))]
    public class FeatureFlagEditor : Editor
    {
        private SerializedProperty uidProperty;
        private SerializedProperty defaultValueProperty;
    
        private void OnEnable()
        {
            uidProperty = serializedObject.FindProperty("uid");
            defaultValueProperty = serializedObject.FindProperty("defaultValue");
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            var featureFlag = (FeatureFlag)target;
            bool overrideValue = featureFlag.GetOverrideValue();

            EditorGUI.BeginDisabledGroup(true);
            {
                EditorGUILayout.PropertyField(uidProperty);
            }
            EditorGUI.EndDisabledGroup();
            
            EditorGUILayout.PropertyField(defaultValueProperty);

            EditorGUI.BeginChangeCheck();
            {
                overrideValue = EditorGUILayout.Toggle("Override", overrideValue);
            }
            if (EditorGUI.EndChangeCheck())
            {
                featureFlag.SetOverrideValue(overrideValue);
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}