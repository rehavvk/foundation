using System;
using UnityEngine;

namespace Rehawk.Foundation.UI
{
    public class ButtonSetActiveHandler : ButtonStateHandlerBase
    {
        [SerializeField] private GameObject targetObject;
        [SerializeField] private ObjectState objectState = ObjectState.DefaultObjectState;
        
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (isActiveAndEnabled)
            {
                DoStateSwap(false);
            }
            
            base.OnValidate();
        }
#endif
        
        protected override void HandleStateTransition(SelectionState state, bool instant)
        {
            bool isActive;
            
            switch (state)
            {
                case SelectionState.Normal:
                    isActive = objectState.NormalState;
                    break;
                case SelectionState.Highlighted:
                    isActive = objectState.HighlightedState;
                    break;
                case SelectionState.Pressed:
                    isActive = objectState.PressedState;
                    break;
                case SelectionState.Selected:
                    isActive = objectState.SelectedState;
                    break;
                case SelectionState.Disabled:
                    isActive = objectState.DisabledState;
                    break;
                default:
                    isActive = true;
                    break;
            }
            
            DoStateSwap(isActive);
        }
        
        private void DoStateSwap(bool isActive)
        {
            if (targetObject == null)
                return;
            
            targetObject.SetActive(isActive);
        }
    }
}