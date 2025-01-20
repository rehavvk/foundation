using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Rehawk.Foundation.UI
{
    public abstract class ButtonStateHandlerBase : UIBehaviour,
                                                   IPointerDownHandler, IPointerUpHandler,
                                                   IPointerEnterHandler, IPointerExitHandler,
                                                   ISelectHandler, IDeselectHandler
    {
        private Button button;
        
        private bool enableCalled = false;
        private SelectionState previousSelectionState;

        private bool isPointerInside;
        private bool isPointerDown;
        private bool hasSelection;

        protected SelectionState PreviousSelectionState
        {
            get { return previousSelectionState; }
        }
        
        protected SelectionState CurrentSelectionState
        {
            get
            {
                if (button != null && !button.IsInteractable())
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

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            
            // OnValidate can be called before OnEnable, this makes it unsafe to access other components
            // since they might not have been initialized yet.
            if (isActiveAndEnabled)
            {
                if ((button == null || !button.IsInteractable()) && EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                }

                DoStateTransition(CurrentSelectionState, true);
            }
        }
#endif
        
        protected override void OnEnable()
        {
            if (enableCalled)
                return;
            
            base.OnEnable();

            DoInitialTransition();
            
            enableCalled = true;
        }

        protected override void Start()
        {
            base.Start();

            DoInitialTransition();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            isPointerInside = false;
            isPointerDown = false;
            hasSelection = false;
            
            DoStateTransition(CurrentSelectionState, true);
        }

        private void Update()
        {
            DoStateTransition(CurrentSelectionState, false);
        }

        private void DoInitialTransition()
        {
            if (EventSystem.current && EventSystem.current.currentSelectedGameObject == gameObject)
            {
                hasSelection = true;
            }
            
            DoStateTransition(CurrentSelectionState, true);
        }

        private void DoStateTransition(SelectionState state, bool instant)
        {
            if (PreviousSelectionState == state)
                return;

            previousSelectionState = state;

            HandleStateTransition(state, instant);
        }

        protected abstract void HandleStateTransition(SelectionState state, bool instant);
        
        private void EvaluateAndTransitionToSelectionState()
        {
            if (!IsActive() || (button != null && !button.IsInteractable()))
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
        
        protected enum SelectionState
        {
            Normal,
            Highlighted,
            Pressed,
            Selected,
            Disabled,
        }
    }
}