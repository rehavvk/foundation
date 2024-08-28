using System.Collections.Generic;
using Rehawk.Foundation.Misc;
using UnityEngine;

namespace Rehawk.Foundation.FeatureFlags
{
    public class FeatureFlagDirectory : ScriptableObject
    {
        [Disabled]
        [SerializeField] private FeatureFlag[] featureFlags;

        public IReadOnlyList<FeatureFlag> FeatureFlags 
        {
            get { return featureFlags; }
        }
    }
}