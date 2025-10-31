using UnityEngine;
using UnityEngine.UI;

namespace Rehawk.Foundation.UI
{
    public class ButtonSpriteSwapHandler : ButtonStateHandlerBase
    {
        [SerializeField] private Graphic targetGraphic;
        [SerializeField] private SpriteState spriteState;
        
#if UNITY_EDITOR
        [Space]

        [SerializeField] private SelectionState previewState = SelectionState.Normal;
#endif
        
        private Image Image
        {
            get => targetGraphic as Image;
            set => targetGraphic = value;
        }
        
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (isActiveAndEnabled)
            {
                DoSpriteSwap(null);
            }
            
            if (!Application.isPlaying && Image != null)
            {
                ApplyPreviewSprite();
            }
            
            base.OnValidate();
        }
#endif
        
        protected override void HandleStateTransition(SelectionState state, bool instant)
        {
            Sprite newSprite = GetSpriteForState(state);
            DoSpriteSwap(newSprite);
        }
        
        private Sprite GetSpriteForState(SelectionState state)
        {
            return state switch
            {
                SelectionState.Normal => null,
                SelectionState.Highlighted => spriteState.highlightedSprite,
                SelectionState.Pressed => spriteState.pressedSprite,
                SelectionState.Selected => spriteState.selectedSprite,
                SelectionState.Disabled => spriteState.disabledSprite,
                _ => null
            };
        }
        
        private void ApplyPreviewSprite()
        {
            Sprite sprite = GetSpriteForState(previewState);
            DoSpriteSwap(sprite);
        }
        
        private void DoSpriteSwap(Sprite newSprite)
        {
            if (!Image)
                return;

            Image.overrideSprite = newSprite;
        }
    }
}