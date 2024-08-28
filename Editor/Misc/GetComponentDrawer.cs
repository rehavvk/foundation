// SOURCE: https://discussions.unity.com/t/a-different-requirecomponent/663125

using UnityEditor;
using UnityEngine;

namespace Rehawk.Foundation.Misc
{
    [CustomPropertyDrawer(typeof(GetComponentAttribute))]
    public class GetComponentDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property);
            
            if (property.serializedObject.isEditingMultipleObjects)
                return;
            
            var monoBehaviour = (MonoBehaviour) property.serializedObject.targetObject;
            
            if (typeof(Component).IsAssignableFrom(fieldInfo.FieldType))
            {
                if (property.objectReferenceValue == null)
                {
                    if (attribute is GetComponentAttribute getComponentAttribute && getComponentAttribute.FromChildren)
                    {
                        property.objectReferenceValue = monoBehaviour.GetComponentInChildren(fieldInfo.FieldType);
                    }
                    else
                    {
                        property.objectReferenceValue = monoBehaviour.GetComponent(fieldInfo.FieldType);
                    }
                }
            }
            else
            {
                Debug.LogError($"Field <b>{fieldInfo.Name}</b> of {monoBehaviour.GetType()} is not a component!", monoBehaviour);
            }
        }
    }
}