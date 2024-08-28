using UnityEngine;
using UnityEditor;

namespace Rehawk.Foundation.Misc
{
    [CustomPropertyDrawer(typeof(DisabledAttribute))]
    public class DisabledDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool previousGUIState = GUI.enabled;
            
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = previousGUIState;
        }
    }
}