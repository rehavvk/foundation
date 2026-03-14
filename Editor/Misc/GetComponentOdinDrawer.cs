using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR && ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
#endif

namespace Rehawk.Foundation.Misc
{
#if UNITY_EDITOR && ODIN_INSPECTOR
    public sealed class GetComponentOdinDrawer : OdinAttributeDrawer<GetComponentAttribute>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            CallNextDrawer(label);

            if (Property.Info?.GetMemberInfo() is not FieldInfo fieldInfo)
                return;

            if (!typeof(Component).IsAssignableFrom(fieldInfo.FieldType))
                return;

            var ownerComponents = new List<Component>();
            var seenInstanceIds = new HashSet<int>();

            object parentValue = Property.Parent?.ValueEntry?.WeakSmartValue;
            GetComponentDrawerUtility.CollectOwnerComponents(parentValue as Object, ownerComponents, seenInstanceIds);

            if (Property.Tree?.WeakTargets != null)
            {
                for (int i = 0; i < Property.Tree.WeakTargets.Count; i++)
                    GetComponentDrawerUtility.CollectOwnerComponents(Property.Tree.WeakTargets[i] as Object, ownerComponents, seenInstanceIds);
            }

            if (ownerComponents.Count == 0)
                return;

            bool fromParents = Attribute.FromParents;
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

                UnityEditor.Undo.RecordObject(ownerComponent, "Auto Assign Component");
                fieldInfo.SetValue(ownerComponent, assigned);
                UnityEditor.EditorUtility.SetDirty(ownerComponent);
            }
        }
    }
#endif
}
