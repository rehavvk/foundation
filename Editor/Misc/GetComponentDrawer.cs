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

            if (fieldInfo == null || !typeof(Component).IsAssignableFrom(fieldInfo.FieldType))
                return;

            Object[] targets = property.serializedObject.targetObjects;
            if (targets == null || targets.Length == 0)
                return;

            var ownerComponents = new System.Collections.Generic.List<Component>(targets.Length);
            var seenInstanceIds = new System.Collections.Generic.HashSet<int>();
            for (int i = 0; i < targets.Length; i++)
                GetComponentDrawerUtility.CollectOwnerComponents(targets[i], ownerComponents, seenInstanceIds);

            if (ownerComponents.Count == 0)
                return;

            bool fromParents = attribute is GetComponentAttribute getComponentAttribute && getComponentAttribute.FromParents;
            for (int i = 0; i < ownerComponents.Count; i++)
            {
                Component ownerComponent = ownerComponents[i];
                if (!fieldInfo.DeclaringType.IsInstanceOfType(ownerComponent))
                    continue;

                if (fieldInfo.GetValue(ownerComponent) != null)
                    continue;

                Component assigned = fromParents
                    ? ownerComponent.GetComponentInParent(fieldInfo.FieldType)
                    : ownerComponent.GetComponentInChildren(fieldInfo.FieldType);

                if (!assigned)
                    continue;

                Undo.RecordObject(ownerComponent, "Auto Assign Component");
                fieldInfo.SetValue(ownerComponent, assigned);
                EditorUtility.SetDirty(ownerComponent);
            }
        }
    }
}
