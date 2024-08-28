using UnityEditor;

namespace Rehawk.Foundation.GUID
{
    [CustomEditor(typeof(GUID))]
    public class GUIDDrawer : Editor
    {
        private GUID guid;

        public override void OnInspectorGUI()
        {
            if (guid == null)
            {
                guid = (GUID) target;
            }
       
            // Draw label
            EditorGUILayout.LabelField("Guid:", guid.Value);
        }
    }
}