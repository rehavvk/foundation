using Rehawk.Foundation.Assets;
using UnityEditor;

namespace Rehawk.Foundation.FeatureFlags
{
    public class FeatureFlagPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            AssetPostprocessorUtility.MakeIdUnique<FeatureFlag>(importedAssets, "uid");
            AssetPostprocessorUtility.HandleDirectory<FeatureFlag, FeatureFlagDirectory>(importedAssets, "Assets/Content/SerializedData/FeatureFlags.asset", "featureFlags");
        }
    }
}