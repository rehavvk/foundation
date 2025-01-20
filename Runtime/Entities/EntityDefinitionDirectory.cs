using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if ODIN_INSPECTOR_3
using Sirenix.OdinInspector;
#endif

namespace Rehawk.Foundation.Entities
{
    public class EntityDefinitionDirectory : ScriptableObject
    {
#if ODIN_INSPECTOR_3
        [ReadOnly, ListDrawerSettings(DraggableItems = false, HideAddButton = true, HideRemoveButton = true, ShowFoldout = false), LabelText(" ")]
#endif
        [SerializeField] private EntityDefinition[] definitions = Array.Empty<EntityDefinition>();

        public IReadOnlyList<EntityDefinition> Definitions
        {
            get { return definitions; }
        }

        public EntityDefinition Find(string uid)
        {
            return Definitions.FirstOrDefault(definition => definition.Uid == uid);
        }
    }
}