using System;
using Rehawk.Foundation.Misc;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Rehawk.Foundation.GUID
{
    [ExecuteInEditMode, DisallowMultipleComponent]
    public class GUID : MonoBehaviour, ISerializationCallbackReceiver
    {
        // System guid we use for comparison and generation
        private Guid guid = Guid.Empty;

        // Unity's serialization system doesn't know about System.Guid, so we convert to a byte array
        // Fun fact, we tried using strings at first, but that allocated memory and was twice as slow
        [SerializeField]
        private byte[] serializedGuid;

        public string Value
        {
            get { return GetGuid().ToString(); }
        }
        
        private void Awake()
        {
            CreateGuid();
        }
        
        // let the manager know we are gone, so other objects no longer find this
        private void OnDisable()
        {
            GuidManager.Remove(guid);
        }

        public bool IsGuidAssigned()
        {
            return guid != System.Guid.Empty;
        }
        
        // When de-serializing or creating this component, we want to either restore our serialized GUID
        // or create a new one.
        private void CreateGuid()
        {
            // if our serialized data is invalid, then we are a new object and need a new GUID
            if (serializedGuid == null || serializedGuid.Length != 16)
            {
    #if UNITY_EDITOR
                // if in editor, make sure we aren't a prefab of some kind
                if (IsAssetOnDisk())
                {
                    return;
                }
                Undo.RecordObject(this, "Added GUID");
    #endif
                guid = Guid.NewGuid();
                serializedGuid = guid.ToByteArray();

    #if UNITY_EDITOR
                // If we are creating a new GUID for a prefab instance of a prefab, but we have somehow lost our prefab connection
                // force a save of the modified prefab instance properties
                if (PrefabUtility.IsPartOfNonAssetPrefabInstance(this))
                {
                    PrefabUtility.RecordPrefabInstancePropertyModifications(this);
                }
    #endif
            }
            else if (guid == Guid.Empty)
            {
                // otherwise, we should set our system guid to our serialized guid
                guid = new Guid(serializedGuid);
            }

            // register with the GUID Manager so that other components can access this
            if (guid != Guid.Empty)
            {
                if (!GuidManager.Add(this))
                {
                    // if registration fails, we probably have a duplicate or invalid GUID, get us a new one.
                    serializedGuid = null;
                    guid = System.Guid.Empty;
                    CreateGuid();
                }
            }
        }

    #if UNITY_EDITOR
        private bool IsEditingInPrefabMode()
        {
            if (EditorUtility.IsPersistent(this))
            {
                // if the game object is stored on disk, it is a prefab of some kind, despite not returning true for IsPartOfPrefabAsset =/
                return true;
            }
            else
            {
                // If the GameObject is not persistent let's determine which stage we are in first because getting Prefab info depends on it
                var mainStage = StageUtility.GetMainStageHandle();
                var currentStage = StageUtility.GetStageHandle(gameObject);
                if (currentStage != mainStage)
                {
                    var prefabStage = PrefabStageUtility.GetPrefabStage(gameObject);
                    if (prefabStage != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsAssetOnDisk()
        {
            return ObjectUtility.IsNotNull(this) && (PrefabUtility.IsPartOfPrefabAsset(this) || IsEditingInPrefabMode());
        }
    #endif

        // We cannot allow a GUID to be saved into a prefab, and we need to convert to byte[]
        public void OnBeforeSerialize()
        {
    #if UNITY_EDITOR
            // This lets us detect if we are a prefab instance or a prefab asset.
            // A prefab asset cannot contain a GUID since it would then be duplicated when instanced.
            if (IsAssetOnDisk())
            {
                serializedGuid = null;
                guid = Guid.Empty;
            }
            else
    #endif
            {
                if (guid != Guid.Empty)
                {
                    serializedGuid = guid.ToByteArray();
                }
            }
        }

        // On load, we can go head a restore our system guid for later use
        public void OnAfterDeserialize()
        {
            if (serializedGuid != null && serializedGuid.Length == 16)
            {
                guid = new Guid(serializedGuid);
            }
        }

        private void OnValidate()
        {
    #if UNITY_EDITOR
            // similar to on Serialize, but gets called on Copying a Component or Applying a Prefab
            // at a time that lets us detect what we are
            if (IsAssetOnDisk())
            {
                serializedGuid = null;
                guid = Guid.Empty;
            }
            else
    #endif
            {
                CreateGuid();
            }
        }

        // Never return an invalid GUID
        public Guid GetGuid()
        {
            if (guid == Guid.Empty && serializedGuid != null && serializedGuid.Length == 16)
            {
                guid = new Guid(serializedGuid);
            }

            return guid;
        }

        public void SetGuid(string guid)
        {
            if (Guid.TryParse(guid, out Guid newGuid))
            {
                if (this.guid != Guid.Empty)
                {
                    GuidManager.Remove(this.guid);
                }
                
                this.guid = newGuid;
                serializedGuid = this.guid.ToByteArray();

                GuidManager.Add(this);
            }
            else
            {
                Debug.LogError("Tried to set an invalid guid. Previous guid will be kept.");
            }
        }
    }
}