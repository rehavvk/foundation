using System;

namespace Rehawk.Foundation.UI
{
    [Serializable]
    public struct ObjectState
    {
        public bool NormalState;
        public bool HighlightedState;
        public bool PressedState;
        public bool SelectedState;
        public bool DisabledState;
            
        public static ObjectState DefaultObjectState;

        static ObjectState()
        {
            DefaultObjectState = new ObjectState
                                 {
                                     NormalState = true,
                                     HighlightedState = true,
                                     PressedState = true,
                                     SelectedState = true,
                                     DisabledState = false
                                 };
        }
    }
}