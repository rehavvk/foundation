using System.Linq;
using Rehawk.Foundation.Assets;
using UnityEditor;

namespace Rehawk.Foundation.Entities.Editor
{
    public class EntityPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            // TODO: Make less static.
            string directoryPath = "Assets/Content/SerializedData/EntityDefinitions.asset";
            
            var entityDefinitionDirectory = AssetHelper.LoadOrCreateAssetAtPath<EntityDefinitionDirectory>(directoryPath);
            AssetPostprocessorUtility.HandleComponentDirectory<Entity, EntityDefinition, EntityDefinitionDirectory>(importedAssets, directoryPath, "definitions", entity => EntityDefinition.From(entity));
            
            Entity[] definitionEntities = entityDefinitionDirectory.Definitions
                                                                   .Select(definition => definition.GetComponent<Entity>())
                                                                   .ToArray();
            
            AssetPostprocessorUtility.MakeComponentIdUnique<Entity>(importedAssets, "definitionUid", definitionEntities);
        }
    }
}