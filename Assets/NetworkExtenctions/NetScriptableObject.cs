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
    [Serializable]
    public class NetScriptableObject<T> where T : UnityEngine.Object, INetworkSerializable, IEquatable<T>
    {
        public delegate void LoadedListener(T result);

        [SerializeField] private AssetReferenceT<T> _selfAssetReference;

        public event LoadedListener Loaded;

        public AssetReferenceT<T> SelfAssetReference => _selfAssetReference;
        
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
        
#if UNITY_EDITOR
        public void OnValidate(UnityEngine.Object target)
        {
            if (Application.isPlaying == false)
            {
                string guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(target));
                _selfAssetReference = new(guid);
            }
        }
#endif
    }
}
