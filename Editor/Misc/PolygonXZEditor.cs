using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Rehawk.Foundation.Misc
{
    [CustomEditor(typeof(PolygonXZ))]
    public class PolygonXZEditor : Editor
    {
        private bool isEditingPoints;
        private int selectedPointIndex = -1;
        
        private SerializedProperty pointsProperty;
    
        private void OnEnable()
        {
            pointsProperty = serializedObject.FindProperty("points");
        }

        private void OnSceneGUI()
        {
            if (isEditingPoints)
            {
                var polygon = (PolygonXZ) target;

                // Reset selectedPointIndex on mouse release
                Event e = Event.current;
                if (e.type == EventType.MouseUp)
                {
                    selectedPointIndex = -1;
                }
                
                Handles.color = Color.red;

                for (int i = 0; i < polygon.Points.Length; i++)
                {
                    EditorGUI.BeginChangeCheck();

                    var currentPoint = new Vector3(polygon.Points[i].x, 0, polygon.Points[i].y);
                    Vector3 newPoint = Handles.FreeMoveHandle(polygon.transform.TransformPoint(currentPoint), 0.25f, Vector3.zero, Handles.DotHandleCap);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(polygon, "Move Point");
                        Vector3 localPoint = polygon.transform.InverseTransformPoint(newPoint);
                        polygon.Points[i] = new Vector2(localPoint.x, localPoint.z);
                        EditorUtility.SetDirty(polygon);
                        selectedPointIndex = i;
                    }
                }

                HandleAddPoint(polygon);
                HandleRemovePoint(polygon);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            GUIContent editButtonContent = EditorGUIUtility.IconContent("d_EditCollider");

            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("Edit Points");
                
                if (GUILayout.Button(editButtonContent, GUILayout.Width(35), GUILayout.Height(23)))
                {
                    isEditingPoints = !isEditingPoints;
                    SceneView.RepaintAll();
                }
                
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            
            EditorGUILayout.PropertyField(pointsProperty);
            
            serializedObject.ApplyModifiedProperties();
        }

        private void HandleAddPoint(PolygonXZ polygon)
        {
            Event e = Event.current;
            if (e.control && e.type == EventType.MouseDown && e.button == 0)
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                Plane plane = new Plane(Vector3.up, Vector3.zero);

                if (plane.Raycast(ray, out float enter))
                {
                    Vector3 hitPoint = ray.GetPoint(enter);
                    Vector3 localHitPoint = polygon.transform.InverseTransformPoint(hitPoint);
                    Vector2 newPoint = new Vector2(localHitPoint.x, localHitPoint.z);

                    // Find the closest edge
                    int insertIndex = 0;
                    float minDistance = float.MaxValue;

                    for (int i = 0; i < polygon.Points.Length; i++)
                    {
                        int j = (i + 1) % polygon.Points.Length;
                        Vector2 edgeStart = polygon.Points[i];
                        Vector2 edgeEnd = polygon.Points[j];

                        Vector2 closestPointOnEdge = PolygonXZ.ClosestPointOnLineSegment(edgeStart, edgeEnd, newPoint);
                        float distance = (closestPointOnEdge - newPoint).sqrMagnitude;

                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            insertIndex = j;
                        }
                    }

                    // Insert the new point at the correct position
                    Undo.RecordObject(polygon, "Add Point");
                    var pointsList = new List<Vector2>(polygon.Points);
                    pointsList.Insert(insertIndex, newPoint);
                    polygon.Points = pointsList.ToArray();
                    EditorUtility.SetDirty(polygon);

                    e.Use();
                }
            }
        }
        
        private void HandleRemovePoint(PolygonXZ polygon)
        {
            Event e = Event.current;
            if (selectedPointIndex >= 0 && e.type == EventType.KeyDown && e.keyCode == KeyCode.Delete)
            {
                Undo.RecordObject(polygon, "Remove Point");
                var pointsList = new List<Vector2>(polygon.Points);
                pointsList.RemoveAt(selectedPointIndex);
                polygon.Points = pointsList.ToArray();
                EditorUtility.SetDirty(polygon);

                selectedPointIndex = -1;
                e.Use();
            }
        }
    }
}