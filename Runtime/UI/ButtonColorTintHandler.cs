using UnityEngine;
using UnityEngine.UI;

namespace Rehawk.Foundation.UI
{
    public class ButtonColorTintHandler : ButtonStateHandlerBase
    {
        [SerializeField] private Graphic targetGraphic;
        [SerializeField] private ColorBlock colors = ColorBlock.defaultColorBlock;

#if UNITY_EDITOR
        [Space]

        [SerializeField] private SelectionState previewState = SelectionState.Normal;
#endif

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            colors.fadeDuration = Mathf.Max(colors.fadeDuration, 0.0f);
            
            if (isActiveAndEnabled)
            {
                StartColorTween(Color.white, true);
            }
            
            if (!Application.isPlaying && targetGraphic != null)
            {
                ApplyPreviewColor();
            }
            
            base.OnValidate();
        }
#endif

        protected override void HandleStateTransition(SelectionState state, bool instant)
        {
            Color tintColor = GetColorForState(previewState);
            StartColorTween(tintColor, instant);
        }
        
#if UNITY_EDITOR
        private void ApplyPreviewColor()
        {
            Color tintColor = GetColorForState(previewState);
            StartColorTween(tintColor, true);
        }
#endif
        
        private Color GetColorForState(SelectionState state)
        {
            Color tintColor = state switch
            {
                SelectionState.Normal => colors.normalColor,
                SelectionState.Highlighted => colors.highlightedColor,
                SelectionState.Pressed => colors.pressedColor,
                SelectionState.Selected => colors.selectedColor,
                SelectionState.Disabled => colors.disabledColor,
                _ => Color.black
            };
            
            return tintColor * colors.colorMultiplier;
        }
        
        private void StartColorTween(Color targetColor, bool instant)
        {
            if (!targetGraphic)
                return;

            targetGraphic.CrossFadeColor(targetColor, instant ? 0f : colors.fadeDuration, true, true);
        }
    }
}