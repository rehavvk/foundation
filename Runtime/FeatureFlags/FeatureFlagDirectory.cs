using System;
using System.Collections.Generic;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace Rehawk.Foundation.FeatureFlags
{
    public class FeatureFlagDirectory : ScriptableObject
    {
#if ODIN_INSPECTOR
        [ListDrawerSettings(IsReadOnly = true), ReadOnly, LabelText(" ")]
#else
        [Rehawk.Foundation.Misc.Disabled]
#endif
        [SerializeField] private FeatureFlag[] featureFlags;

        public IReadOnlyList<FeatureFlag> All => featureFlags;
        
        public int GetId(FeatureFlag featureFlag)
        {
            return Array.IndexOf(featureFlags, featureFlag);
        }

        public FeatureFlag GetById(int id)
        {
            if (id >= 0 && id < featureFlags.Length)
            {
                return featureFlags[id];
            }

            return null;
        }
    }
}