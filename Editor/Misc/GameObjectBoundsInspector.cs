using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Rehawk.Foundation.Misc
{
    [CustomEditor(typeof(GameObjectBounds))]
    public class GameObjectBoundsInspector : Editor
    {
        private GUIContent editButtonContent;
        private bool isEditingPoints;

        private SerializedProperty offsetProperty;
        private SerializedProperty sizeProperty;
        
        private readonly BoxBoundsHandle boundsHandle = new BoxBoundsHandle();

        private void OnEnable()
        {
            offsetProperty = serializedObject.FindProperty("offset");
            sizeProperty = serializedObject.FindProperty("size");

            editButtonContent = EditorGUIUtility.IconContent("d_EditCollider");

            boundsHandle.handleColor = Color.red;
            boundsHandle.wireframeColor = Color.clear;
        }

        private void OnSceneGUI()
        {
            if (!isEditingPoints)
                return;
            
            var gameObjectBounds = (GameObjectBounds)target;

            Matrix4x4 matrix = gameObjectBounds.transform.localToWorldMatrix;

            using (new Handles.DrawingScope(matrix))
            {
                boundsHandle.center = offsetProperty.vector3Value;
                boundsHandle.size = sizeProperty.vector3Value;

                EditorGUI.BeginChangeCheck();
                {
                    boundsHandle.DrawHandle();
                }
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(gameObjectBounds, "Change Bounds");

                    offsetProperty.vector3Value = boundsHandle.center;
                    sizeProperty.vector3Value = boundsHandle.size;

                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("Edit");

                if (GUILayout.Button(editButtonContent, GUILayout.Width(35), GUILayout.Height(23)))
                {
                    isEditingPoints = !isEditingPoints;
                    SceneView.RepaintAll();
                }

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(offsetProperty);
            EditorGUILayout.PropertyField(sizeProperty);

            serializedObject.ApplyModifiedProperties();
        }

        [DrawGizmo(GizmoType.Selected)]
        private static void DrawWireCube(GameObjectBounds gameObjectBounds, GizmoType gizmoType)
        {
            Color oldColor = Gizmos.color;

            Gizmos.color = Color.red;
            Gizmos.matrix = gameObjectBounds.transform.localToWorldMatrix;

            Gizmos.DrawWireCube(gameObjectBounds.Offset, gameObjectBounds.Size);

            Gizmos.matrix = Matrix4x4.identity;

            Gizmos.color = oldColor;
        }
    }
}