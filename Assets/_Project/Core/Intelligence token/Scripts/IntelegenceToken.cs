using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.Maps.IntellegenceTokens
{
    [Icon("Assets/_Project/Core/Map/Editor/intellegence-token.png")]
    [CreateAssetMenu(menuName = "Game/Maps/Intelegence token")]
    public class IntelegenceToken : ScriptableObject, INetworkSerializable, IEquatable<IntelegenceToken>
    {
        [field: SerializeField] private string _loadKey;
        
        [field: SerializeField] public string Id { get; private set; }

        [field: SerializeField] public int LootCount { get; private set; }

        [field: SerializeField] public IntelegenceTokenAction Action { get; private set; }

        public bool Equals(IntelegenceToken other)
        {
            return other._loadKey == _loadKey && other.Id == Id;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (string.IsNullOrEmpty(Id) && serializer.IsWriter)
            {
                throw new ArgumentException($"ID is null");
            }

            if (string.IsNullOrEmpty(_loadKey) && serializer.IsWriter)
            {
                throw new ArgumentException($"Load key is null");
            }

            FixedString64Bytes loadKey = new();
            if (serializer.IsWriter)
            {
                loadKey = new(_loadKey);
            }
            serializer.SerializeValue(ref loadKey);
            if (serializer.IsReader)
            {
                _loadKey = loadKey.ToString();
            }

            FixedString64Bytes id = new();
            if (serializer.IsWriter)
            {
                id = new(Id);
            }
            serializer.SerializeValue(ref id);
            if (serializer.IsReader)
            {
                Id = id.ToString();
            }

            AssetReferenceT<IntelegenceToken> assetReference = new(_loadKey.ToString());
            AsyncOperationHandle<IntelegenceToken> loadHandle = assetReference.LoadAssetAsync();

            loadHandle.Completed += (handle) => 
            {
                _loadKey = handle.Result._loadKey;
                Action = handle.Result.Action;
                LootCount = handle.Result.LootCount;
                if (string.IsNullOrEmpty(name))
                {
                    name = $"{handle.Result.name} (net loaded)";
                }
            };
        }

        #if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (Application.isPlaying == false)
            {

                string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(this));
                AssetReference reference = new AssetReference(guid);

                if (string.IsNullOrEmpty(_loadKey))
                {
                    _loadKey = new(reference.RuntimeKey.ToString());
                    EditorUtility.SetDirty(this);
                }
                if (string.IsNullOrEmpty(Id))
                {
                    Id = name;
                    EditorUtility.SetDirty(this);
                }
            }
        }
#endif
    }
}
