using UnityEngine;
using Unity.Netcode.Custom;
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
    public class RoomContent : ScriptableObject, INetworkSerializable, IEquatable<RoomContent>
    {
        public delegate void LoadedListener(RoomContent sender);

        [field: SerializeField] private FixedString64Bytes _id;
        [field: SerializeField] public RoomAction RoomAction { get; private set; } 
        [field: SerializeField] public int Layer { get; private set; } = 0;

        public event LoadedListener Loaded;

        public bool Equals(RoomContent other)
        {
            return _id == other._id;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _id);

            AssetReferenceT<RoomContent> assetReference = new(_id.ToString());
            AsyncOperationHandle<RoomContent> loadHandle = assetReference.LoadAssetAsync();

            loadHandle.Completed += (handle) => 
            {
                _id = handle.Result._id;
                RoomAction = handle.Result.RoomAction;
                if (string.IsNullOrEmpty(name))
                {
                    name = $"{handle.Result.name} (net loaded)";
                }
                Loaded?.Invoke(this);
            };
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (Application.isPlaying == false)
            {
                string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(this));
                AssetReference reference = new AssetReference(guid);
                _id = new(reference.RuntimeKey.ToString());
            }
        }
#endif
    }
}
