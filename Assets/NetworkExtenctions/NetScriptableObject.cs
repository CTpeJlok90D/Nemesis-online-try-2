using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Unity.Collections;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

namespace Unity.Netcode.Custom
{
    [Serializable]
    public class NetScriptableObject<T> where T : UnityEngine.Object, INetworkSerializable, IEquatable<T>
    {
        public delegate void LoadedListener(T result);

        [SerializeField] private AssetReferenceT<T> _selfAssetReference;

        public event LoadedListener Loaded;

        public AssetReferenceT<T> SelfAssetReference => _selfAssetReference;

        public string RuntimeLoadKey => _selfAssetReference.RuntimeKey.ToString();
        
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
                Loaded?.Invoke(handle.Result);
            };
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(NetScriptableObject<>))]
    internal class CEditor : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement root = new();

            SerializedProperty serializedProperty = property.FindPropertyRelative("_selfAssetReference");
            ValidateAssetReference(serializedProperty);


            PropertyField assetReferenceField = new(serializedProperty, "Net key");
            assetReferenceField.enabledSelf = false;
            root.Add(assetReferenceField);
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
    }
#endif
}
