using UnityEngine;
using UnityEngine.UI;

namespace Rehawk.Foundation.UI
{
    public class ButtonColorTintHandler : ButtonStateHandlerBase
    {
        [SerializeField] private Graphic targetGraphic;
        [SerializeField] private ColorBlock colors = ColorBlock.defaultColorBlock;

        protected override void OnValidate()
        {
            colors.fadeDuration = Mathf.Max(colors.fadeDuration, 0.0f);
            
            if (isActiveAndEnabled)
            {
                StartColorTween(Color.white, true);
            }
            
            base.OnValidate();
        }

        protected override void HandleStateTransition(SelectionState state, bool instant)
        {
            Color tintColor;
            
            switch (state)
            {
                case SelectionState.Normal:
                    tintColor = colors.normalColor;
                    break;
                case SelectionState.Highlighted:
                    tintColor = colors.highlightedColor;
                    break;
                case SelectionState.Pressed:
                    tintColor = colors.pressedColor;
                    break;
                case SelectionState.Selected:
                    tintColor = colors.selectedColor;
                    break;
                case SelectionState.Disabled:
                    tintColor = colors.disabledColor;
                    break;
                default:
                    tintColor = Color.black;
                    break;
            }
            
            StartColorTween(tintColor * colors.colorMultiplier, instant);
        }

        private void StartColorTween(Color targetColor, bool instant)
        {
            if (targetGraphic == null)
                return;

            targetGraphic.CrossFadeColor(targetColor, instant ? 0f : colors.fadeDuration, true, true);
        }
    }
}