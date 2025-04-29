using System.Collections;
using UnityEngine;

namespace Rehawk.Foundation.CustomYieldInstructions
{
    public class WaitForCoroutine : CustomYieldInstruction
    {
        private bool isDone;
        
        public override bool keepWaiting => !isDone;

        public WaitForCoroutine(IEnumerator routine)
        {
            WaitForCoroutineHelper.Instance.StartCoroutine(WrappedRoutine(routine));
        }
        
        private IEnumerator WrappedRoutine(IEnumerator routine)
        {
            yield return routine;
            isDone = true;
        }
    }
}