using UnityEngine;
using System;
using Unity.Netcode;
using Unity.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Core.Maps
{
    [CreateAssetMenu(menuName = "Game/Maps/Room content")]
    [Icon("Assets/_Project/Core/Map/Editor/icons8-room-96.png")]
    public class RoomType : ScriptableObject, INetworkSerializable, IEquatable<RoomType>
    {
        public delegate void LoadedListener(RoomType sender);

        [field: SerializeField] private string _loadKey;
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public RoomAction RoomAction { get; private set; } 
        [field: SerializeField] public int Layer { get; private set; } = 0;
        [field: SerializeField] public LootType Loot { get; private set; } = 0;

        public event LoadedListener Loaded;

        public bool Equals(RoomType other)
        {
            return _loadKey == other._loadKey;
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

            AssetReferenceT<RoomType> assetReference = new(_loadKey.ToString());
            AsyncOperationHandle<RoomType> loadHandle = assetReference.LoadAssetAsync();

            loadHandle.Completed += (handle) => 
            {
                _loadKey = handle.Result._loadKey;
                RoomAction = handle.Result.RoomAction;
                if (string.IsNullOrEmpty(name))
                {
                    name = $"{handle.Result.name} (net loaded)";
                }
                Loaded?.Invoke(this);
            };
        }
        
        public enum LootType
        {
            MedicineRoom,
            BattleRoom,
            TechnicalRoom,
            UniversalRoom,
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
