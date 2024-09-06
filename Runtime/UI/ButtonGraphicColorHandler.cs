using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rehawk.Foundation.UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonGraphicColorHandler : UIBehaviour, 
        IPointerDownHandler, IPointerUpHandler,
        IPointerEnterHandler, IPointerExitHandler, 
        ISelectHandler, IDeselectHandler
    {
        [SerializeField] private Graphic targetGraphic;
        [SerializeField] private ColorBlock colors = ColorBlock.defaultColorBlock;
        
        private Button button;
        
        private bool enableCalled = false;

        private bool isPointerInside;
        private bool isPointerDown;
        private bool hasSelection;
        
        private SelectionState CurrentSelectionState
        {
            get
            {
                if (!button.IsInteractable())
                {
                    return SelectionState.Disabled;
                }

                if (isPointerDown)
                {
                    return SelectionState.Pressed;
                }

                if (hasSelection)
                {
                    return SelectionState.Selected;
                }

                if (isPointerInside)
                {
                    return SelectionState.Highlighted;
                }

                return SelectionState.Normal;
            }
        }
        
        protected override void Awake()
        {
            base.Awake();
            
            button = GetComponent<Button>();
        }

        protected override void OnEnable()
        {
            if (enableCalled)
                return;
            
            base.OnEnable();

            if (EventSystem.current && EventSystem.current.currentSelectedGameObject == gameObject)
            {
                hasSelection = true;
            }

            DoStateTransition(CurrentSelectionState, true);

            enableCalled = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            isPointerInside = false;
            isPointerDown = false;
            hasSelection = false;
            
            DoStateTransition(CurrentSelectionState, true);
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            
            colors.fadeDuration = Mathf.Max(colors.fadeDuration, 0.0f);
            
            if (isActiveAndEnabled)
            {
                StartColorTween(Color.white, true);
            }
        }

        private void DoStateTransition(SelectionState state, bool instant)
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
        
        private void EvaluateAndTransitionToSelectionState()
        {
            if (!IsActive() || !button.IsInteractable())
                return;

            DoStateTransition(CurrentSelectionState, false);
        }
        
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            isPointerDown = true;
            EvaluateAndTransitionToSelectionState();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            isPointerDown = false;
            EvaluateAndTransitionToSelectionState();
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            isPointerInside = true;
            EvaluateAndTransitionToSelectionState();
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            isPointerInside = false;
            EvaluateAndTransitionToSelectionState();
        }

        void ISelectHandler.OnSelect(BaseEventData eventData)
        {
            hasSelection = true;
            EvaluateAndTransitionToSelectionState();
        }

        void IDeselectHandler.OnDeselect(BaseEventData eventData)
        {
            hasSelection = false;
            EvaluateAndTransitionToSelectionState();
        }
        
        private enum SelectionState
        {
            Normal,
            Highlighted,
            Pressed,
            Selected,
            Disabled,
        }
    }
}