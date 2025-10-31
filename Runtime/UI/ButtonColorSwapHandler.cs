using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Rehawk.Foundation.UI
{
    public class ButtonColorSwapHandler : ButtonStateHandlerBase
    {
        [SerializeField] private Graphic targetGraphic;
        [SerializeField] private ColorBlock colors = ColorBlock.defaultColorBlock;
        [SerializeField] private AnimationCurve easing = AnimationCurve.EaseInOut(0, 0, 1, 1);
        
#if UNITY_EDITOR
        [Space]
        
        [SerializeField] private SelectionState previewState = SelectionState.Normal;
#endif

        private Coroutine currentTween;
        
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            colors.fadeDuration = Mathf.Max(colors.fadeDuration, 0.0f);
            
            if (isActiveAndEnabled && targetGraphic)
            {
                targetGraphic.color = GetColorForState(SelectionState.Normal);
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
            Color targetColor = GetColorForState(state);

            if (!targetGraphic)
                return;

            if (currentTween != null)
                StopCoroutine(currentTween);

            if (instant || colors.fadeDuration <= 0f)
            {
                targetGraphic.color = targetColor;
                return;
            }

            currentTween = StartCoroutine(TweenColorRoutine(targetGraphic, targetColor, colors.fadeDuration));
        }
        
        private Color GetColorForState(SelectionState state)
        {
            Color swapColor = state switch
            {
                SelectionState.Normal => colors.normalColor,
                SelectionState.Highlighted => colors.highlightedColor,
                SelectionState.Pressed => colors.pressedColor,
                SelectionState.Selected => colors.selectedColor,
                SelectionState.Disabled => colors.disabledColor,
                _ => Color.black
            };
            
            return swapColor * colors.colorMultiplier;
        }
        
        
#if UNITY_EDITOR
        private void ApplyPreviewColor()
        {
            targetGraphic.color = GetColorForState(previewState);
        }
#endif
        
        private IEnumerator TweenColorRoutine(Graphic graphic, Color target, float duration)
        {
            Color start = graphic.color;
            float time = 0f;

            while (time < duration)
            {
                time += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(time / duration);
                float easedT = easing.Evaluate(t);
                graphic.color = Color.Lerp(start, target, easedT);
                yield return null;
            }

            graphic.color = target;
            currentTween = null;
        }
    }
}