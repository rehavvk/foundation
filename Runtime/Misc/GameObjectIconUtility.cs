using UnityEngine;

namespace Rehawk.Foundation.Misc
{
    public static class GameObjectIconUtility
    {
        public static void SetIcon(GameObject gameObject, LabelIcon icon)
        {
#if UNITY_EDITOR
            var iconContent = UnityEditor.EditorGUIUtility.IconContent($"sv_label_{(int)icon}");
            UnityEditor.EditorGUIUtility.SetIconForObject(gameObject, (Texture2D) iconContent.image);
#endif
        }

        public static void SetIcon(GameObject gameObject, Icon icon)
        {
#if UNITY_EDITOR
            var iconContent = UnityEditor.EditorGUIUtility.IconContent($"sv_icon_dot{(int)icon}_pix16_gizmo");
            UnityEditor.EditorGUIUtility.SetIconForObject(gameObject, (Texture2D) iconContent.image);
#endif
        }

        public enum LabelIcon
        {
            Gray = 0,
            Blue,
            Teal,
            Green,
            Yellow,
            Orange,
            Red,
            Purple
        }

        public enum Icon
        {
            CircleGray = 0,
            CircleBlue,
            CircleTeal,
            CircleGreen,
            CircleYellow,
            CircleOrange,
            CircleRed,
            CirclePurple,
            DiamondGray,
            DiamondBlue,
            DiamondTeal,
            DiamondGreen,
            DiamondYellow,
            DiamondOrange,
            DiamondRed,
            DiamondPurple
        }
    }
}