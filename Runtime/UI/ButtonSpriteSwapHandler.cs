using UnityEngine;
using UnityEngine.UI;

namespace Rehawk.Foundation.UI
{
    public class ButtonSpriteSwapHandler : ButtonStateHandlerBase
    {
        [SerializeField] private Graphic targetGraphic;
        [SerializeField] private SpriteState spriteState;
        
        private Image Image
        {
            get { return targetGraphic as Image; }
            set { targetGraphic = value; }
        }
        
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (isActiveAndEnabled)
            {
                DoSpriteSwap(null);
            }
            
            base.OnValidate();
        }
#endif
        
        protected override void HandleStateTransition(SelectionState state, bool instant)
        {
            Sprite newSprite;
            
            switch (state)
            {
                case SelectionState.Normal:
                    newSprite = null;
                    break;
                case SelectionState.Highlighted:
                    newSprite = spriteState.highlightedSprite;
                    break;
                case SelectionState.Pressed:
                    newSprite = spriteState.pressedSprite;
                    break;
                case SelectionState.Selected:
                    newSprite = spriteState.selectedSprite;
                    break;
                case SelectionState.Disabled:
                    newSprite = spriteState.disabledSprite;
                    break;
                default:
                    newSprite = null;
                    break;
            }
            
            DoSpriteSwap(newSprite);
        }
        
        private void DoSpriteSwap(Sprite newSprite)
        {
            if (Image == null)
                return;

            Image.overrideSprite = newSprite;
        }
    }
}