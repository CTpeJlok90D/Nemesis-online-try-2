using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Unity.Collections;
using UnityEngine.ResourceManagement.AsyncOperations;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Unity.Netcode.Custom
{
    public class NetScriptableObject : ScriptableObject, INetworkSerializable, IEquatable<NetScriptableObject>
    {
        public delegate void LoadedListener(NetScriptableObject sender);

        [SerializeField] private AssetReferenceT<NetScriptableObject> _selfAssetReference;

        public event LoadedListener Loaded;

        protected AssetReferenceT<NetScriptableObject> SelfAssetReference => _selfAssetReference;

        public bool Equals(NetScriptableObject other)
        {
            return _selfAssetReference.RuntimeKey == other._selfAssetReference.RuntimeKey;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            FixedString64Bytes loadKey = "";

            if (serializer.IsWriter)
            {
                loadKey = new (_selfAssetReference.RuntimeKey.ToString());
                serializer.SerializeValue(ref loadKey);
                return;
            }

            serializer.SerializeValue(ref loadKey);

            AssetReferenceT<NetScriptableObject> assetReference = new(loadKey.ToString());
            AsyncOperationHandle<NetScriptableObject> loadHandle = assetReference.LoadAssetAsync();

            loadHandle.Completed += (handle) => 
            {
                _selfAssetReference = handle.Result._selfAssetReference;
                OnNetworkSerialize(handle.Result);
                Loaded?.Invoke(this);
            };
        }

        protected virtual void OnNetworkSerialize(NetScriptableObject original) { }
        
#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (Application.isPlaying == false)
            {
                string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(this));
                _selfAssetReference = new(guid);
            }
        }
#endif
    }
}
