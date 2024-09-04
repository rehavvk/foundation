using System;
using UnityEngine;

namespace Rehawk.Foundation.Misc
{
    /// <summary>
    ///   <para>The GetComponent attribute automatically sets the field to the first found assignable component of the GameObject. If the field already has a non-null value, the attribute has no effect.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class GetComponentAttribute : PropertyAttribute
    {
        public bool FromParents;
        
        public GetComponentAttribute() {}

        public GetComponentAttribute(bool fromParents)
        {
            FromParents = fromParents;
        }
    }
}