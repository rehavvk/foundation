using System.Collections;
using Rehawk.Foundation.CustomYieldInstructions;
using UnityEngine;

namespace Rehawk.Foundation.Extensions
{
    public static class IEnumeratorExtensions
    {
        public static CustomYieldInstruction WaitFor(this IEnumerator enumerator)
        {
            return new WaitForCoroutine(enumerator);
        }
    }
}