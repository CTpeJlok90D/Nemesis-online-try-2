using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Unity.Collections;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;
using System.Threading.Tasks;


#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace Unity.Netcode.Custom
{
    [Serializable]
    public class NetScriptableObject<T> where T : UnityEngine.Object, INetworkSerializable, IEquatable<T>, INetScriptableObjectArrayElement<T>
    {
        public delegate void LoadedListener(T result);

        [SerializeField] private AssetReferenceT<T> _selfAssetReference;
        [SerializeField] private bool _isLoaded;

        public event LoadedListener Preloaded;
        public event LoadedListener Loaded;

        public AssetReferenceT<T> SelfAssetReference => _selfAssetReference;

        public string RuntimeLoadKey => _selfAssetReference.RuntimeKey.ToString();

        public bool IsLoaded => _isLoaded;
        
        public void OnNetworkSerialize<T1>(BufferSerializer<T1> serializer, ScriptableObject sender) where T1 : IReaderWriter
        {
            FixedString64Bytes loadKey = "";

            if (serializer.IsWriter)
            {
                loadKey = new (_selfAssetReference.RuntimeKey.ToString());
                serializer.SerializeValue(ref loadKey);
                return;
            }

            serializer.SerializeValue(ref loadKey);

            AssetReferenceT<T> assetReference = new(loadKey.ToString());
            AsyncOperationHandle<T> loadHandle = assetReference.LoadAssetAsync();
            _selfAssetReference = assetReference;

            loadHandle.Completed += (handle) => 
            {
                if (string.IsNullOrEmpty(sender.name))
                {
                    sender.name = $"{handle.Result.name} (net loaded)";
                }

                INetScriptableObjectArrayElement<T> element = sender as INetScriptableObjectArrayElement<T>;

                Preloaded?.Invoke(handle.Result);
                element.Net._isLoaded = true;
                Loaded?.Invoke(handle.Result);
            };
        }

        public async Task AwaitForLoad()
        {
            while (IsLoaded == false)
            {
                await Awaitable.NextFrameAsync();
            }
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(NetScriptableObject<>))]
    internal class CEditor : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new();

            SerializedProperty selfAssetReferenceSerializedProperty = property.FindPropertyRelative("_selfAssetReference");
            ValidateAssetReference(selfAssetReferenceSerializedProperty);
            ValidateLoadBoolean(property);


            PropertyField assetReferenceField = new(selfAssetReferenceSerializedProperty, "Net key");
            assetReferenceField.enabledSelf = false;
            root.Add(assetReferenceField);
            EditorUtility.SetDirty(property.serializedObject.targetObject);
            return root;
        }

        private void ValidateAssetReference(SerializedProperty property)
        {
            SerializedProperty assetGUIDProperty = property.FindPropertyRelative("m_AssetGUID");
            string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(assetGUIDProperty.serializedObject.targetObject));
            if (assetGUIDProperty.stringValue != guid)
            {
                assetGUIDProperty.stringValue = guid;
                assetGUIDProperty.serializedObject.ApplyModifiedProperties();
            }
        }

        private void ValidateLoadBoolean(SerializedProperty property)
        {
            SerializedProperty isLoadedProperty = property.FindPropertyRelative("_isLoaded");
            isLoadedProperty.boolValue = true;
            isLoadedProperty.serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
